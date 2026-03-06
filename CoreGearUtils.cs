/*
name: CoreGearUtils
description: Shared helper utilities for selecting and equipping best gear.
tags: core, gear, utility
*/
//cs_include Scripts/CoreBots.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class CoreGearUtilsMarker { }

/*
Usage
- Preset mode (recommended): CoreGearUtils.EquipBestGear(GearProfilePreset.Chaos);
- Custom mode: CoreGearUtils.EquipBestGear("gold,dmgAll,cp,rep,xp");
- Store gear: var snapshot = CoreGearUtils.CaptureEquipment();
- Restore gear: CoreGearUtils.RestoreEquipment(snapshot);

Rationale
- This utility is profile-driven, not race-only. Any script can optimize for damage, gold, xp, cp, rep, or race tags.
- Presets provide sane defaults and consistent behavior across scripts.
- Custom mode gives full control over meta priority order when a script needs special tuning.
- After baseline equip, a stacking pass attempts to equip the best valid pair across slots:
  one item for primary meta + one for secondary meta (when they can stack).

Preset Priority Table
- Damage      -> dmgAll, gold, cp, rep, xp
- Gold        -> gold, dmgAll, cp, rep, xp
- Exp         -> xp, dmgAll, gold, cp, rep
- ClassPoints -> cp, dmgAll, gold, rep, xp
- Reputation  -> rep, dmgAll, gold, cp, xp
- Chaos       -> Chaos, dmgAll, gold, cp, rep, xp
- Undead      -> Undead, dmgAll, gold, cp, rep, xp
- Elemental   -> Elemental, dmgAll, gold, cp, rep, xp
- Dragonkin   -> Dragonkin, dmgAll, gold, cp, rep, xp
- Human       -> Human, dmgAll, gold, cp, rep, xp
- Orc         -> Orc, dmgAll, gold, cp, rep, xp
*/
public enum GearProfilePreset
{
    Damage,
    Gold,
    Exp,
    ClassPoints,
    Reputation,
    Chaos,
    Undead,
    Elemental,
    Dragonkin,
    Human,
    Orc
}

public static class CoreGearUtils
{
    private static readonly string[] DamageFallbackOrder = { "dmgAll", "gold", "cp", "rep", "xp" };
    private static readonly string[] RestoreSlotOrder = { "Class", "Weapon", "Armor", "Helm", "Cape", "Pet" };

    public static void EquipBestGear(GearProfilePreset preset = GearProfilePreset.Damage)
        => EquipBestGear(IScriptInterface.Instance, CoreBots.Instance, preset);

    public static void EquipBestGear(IScriptInterface bot, CoreBots core, GearProfilePreset preset = GearProfilePreset.Damage)
        => EquipBestGearInternal(bot, core, BuildPlan(preset));

    // Accepts either a preset-like token (e.g. "gold", "chaos") or a custom order
    // (e.g. "gold,dmgAll,cp,rep,xp").
    public static void EquipBestGear(string profileOrMetaOrder = "damage")
        => EquipBestGear(IScriptInterface.Instance, CoreBots.Instance, profileOrMetaOrder);

    public static void EquipBestGear(IScriptInterface bot, CoreBots core, string profileOrMetaOrder = "damage")
        => EquipBestGearInternal(bot, core, BuildPlan(profileOrMetaOrder));

    public static EquipmentSnapshot CaptureEquipment()
        => CaptureEquipment(IScriptInterface.Instance);

    public static EquipmentSnapshot CaptureEquipment(IScriptInterface bot)
    {
        if (bot == null)
            return new EquipmentSnapshot(new Dictionary<string, string>(), Array.Empty<string>());

        var bySlot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var extras = new List<string>();

        foreach (InventoryItem item in bot.Inventory.Items.Where(i => i != null && i.Equipped))
        {
            string name = item.Name;
            string? slot = GetEquipmentSlotKey(item.Category.ToString());
            if (string.IsNullOrWhiteSpace(slot))
            {
                extras.Add(name);
                continue;
            }

            // Keep first seen item for a slot.
            if (!bySlot.ContainsKey(slot))
                bySlot[slot] = name;
            else
                extras.Add(name);
        }

        string[] ordered = BuildRestoreOrder(bySlot, extras);
        return new EquipmentSnapshot(bySlot, ordered);
    }

    public static void RestoreEquipment(EquipmentSnapshot snapshot, bool loadBank = true)
        => RestoreEquipment(IScriptInterface.Instance, CoreBots.Instance, snapshot, loadBank);

    public static void RestoreEquipment(IScriptInterface bot, CoreBots core, EquipmentSnapshot snapshot, bool loadBank = true)
    {
        if (bot == null || core == null || snapshot == null || snapshot.OrderedItems.Length == 0)
            return;

        if (loadBank)
            TryLoadBank(bot);

        foreach (string itemName in snapshot.OrderedItems.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(itemName))
                continue;

            if (bot.Bank.Contains(itemName))
                core.Unbank(itemName);

            if (bot.Inventory.Contains(itemName))
                bot.Inventory.EquipItem(itemName);
        }
    }

    private static void EquipBestGearInternal(IScriptInterface bot, CoreBots core, GearPlan plan)
    {
        if (bot == null || core == null)
            return;

        try
        {
            TryLoadBank(bot);

            core.EquipBestItemsForMeta(
                new Dictionary<string, string[]>
                {
                    { "Weapon", plan.MetaPriority },
                    { "Armor", plan.MetaPriority },
                    { "Helm", plan.MetaPriority },
                    { "Cape", plan.MetaPriority },
                    { "Pet", plan.MetaPriority },
                }
            );

            EquipBestStackingPair(bot, core, plan.PrimaryMeta, plan.SecondaryMeta);
        }
        catch
        {
            bot.Log("Best gear equip failed. Continuing with current setup.");
        }
    }

    private static void TryLoadBank(IScriptInterface bot)
    {
        try
        {
            bot.Bank.Load(true);
        }
        catch
        {
            bot.Log("Bank load failed before best-gear selection; continuing.");
        }
    }

    private static GearPlan BuildPlan(GearProfilePreset preset)
    {
        return preset switch
        {
            GearProfilePreset.Gold => NewPlan("gold", new[] { "gold", "dmgAll", "cp", "rep", "xp" }),
            GearProfilePreset.Exp => NewPlan("xp", new[] { "xp", "dmgAll", "gold", "cp", "rep" }),
            GearProfilePreset.ClassPoints => NewPlan("cp", new[] { "cp", "dmgAll", "gold", "rep", "xp" }),
            GearProfilePreset.Reputation => NewPlan("rep", new[] { "rep", "dmgAll", "gold", "cp", "xp" }),
            GearProfilePreset.Chaos => NewPlan("Chaos", BuildMetaPriority("Chaos", DamageFallbackOrder)),
            GearProfilePreset.Undead => NewPlan("Undead", BuildMetaPriority("Undead", DamageFallbackOrder)),
            GearProfilePreset.Elemental => NewPlan("Elemental", BuildMetaPriority("Elemental", DamageFallbackOrder)),
            GearProfilePreset.Dragonkin => NewPlan("Dragonkin", BuildMetaPriority("Dragonkin", DamageFallbackOrder)),
            GearProfilePreset.Human => NewPlan("Human", BuildMetaPriority("Human", DamageFallbackOrder)),
            GearProfilePreset.Orc => NewPlan("Orc", BuildMetaPriority("Orc", DamageFallbackOrder)),
            _ => NewPlan("dmgAll", new[] { "dmgAll", "gold", "cp", "rep", "xp" }),
        };
    }

    private static GearPlan BuildPlan(string profileOrMetaOrder)
    {
        string raw = (profileOrMetaOrder ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return BuildPlan(GearProfilePreset.Damage);

        var tokens = ParseMetaList(raw);
        if (tokens.Count > 1)
        {
            string primary = tokens[0];
            return NewPlan(primary, BuildMetaPriority(primary, tokens.Skip(1).ToArray()));
        }

        string single = tokens.Count == 1 ? tokens[0] : raw;
        if (TryMapPreset(single, out GearProfilePreset preset))
            return BuildPlan(preset);

        string normalized = NormalizeMetaToken(single);
        return NewPlan(normalized, BuildMetaPriority(normalized, DamageFallbackOrder));
    }

    private static GearPlan NewPlan(string primaryMeta, string[] metaPriority)
        => new(primaryMeta, GetSecondaryMeta(metaPriority, primaryMeta), metaPriority);

    private static string[] BuildMetaPriority(string? primaryMeta, IEnumerable<string> fallbackOrder)
    {
        var metas = new List<string>();
        if (!string.IsNullOrWhiteSpace(primaryMeta))
            metas.Add(primaryMeta);

        foreach (string meta in fallbackOrder)
        {
            if (!metas.Any(x => x.Equals(meta, StringComparison.OrdinalIgnoreCase)))
                metas.Add(meta);
        }

        return metas.ToArray();
    }

    private static string GetSecondaryMeta(string[] metaPriority, string primaryMeta)
    {
        foreach (string meta in metaPriority)
        {
            if (!meta.Equals(primaryMeta, StringComparison.OrdinalIgnoreCase))
                return meta;
        }

        return primaryMeta.Equals("dmgAll", StringComparison.OrdinalIgnoreCase) ? "gold" : "dmgAll";
    }

    private static List<string> ParseMetaList(string raw)
    {
        char[] separators = { ',', ';', '|', '>' };
        return raw
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(NormalizeMetaToken)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeMetaToken(string token)
    {
        string value = (token ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "dmgAll";

        return value.ToLowerInvariant() switch
        {
            "dmg" => "dmgAll",
            "damage" => "dmgAll",
            "dmgall" => "dmgAll",
            "exp" => "xp",
            "experience" => "xp",
            "classpoints" => "cp",
            "class-points" => "cp",
            "reputation" => "rep",
            "undead" => "Undead",
            "chaos" => "Chaos",
            "elemental" => "Elemental",
            "dragonkin" => "Dragonkin",
            "human" => "Human",
            "orc" => "Orc",
            _ => value,
        };
    }

    private static bool TryMapPreset(string token, out GearProfilePreset preset)
    {
        switch ((token ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "damage":
            case "dmg":
            case "dmgall":
                preset = GearProfilePreset.Damage;
                return true;
            case "gold":
                preset = GearProfilePreset.Gold;
                return true;
            case "xp":
            case "exp":
            case "experience":
                preset = GearProfilePreset.Exp;
                return true;
            case "cp":
            case "classpoints":
                preset = GearProfilePreset.ClassPoints;
                return true;
            case "rep":
            case "reputation":
                preset = GearProfilePreset.Reputation;
                return true;
            case "chaos":
                preset = GearProfilePreset.Chaos;
                return true;
            case "undead":
                preset = GearProfilePreset.Undead;
                return true;
            case "elemental":
                preset = GearProfilePreset.Elemental;
                return true;
            case "dragonkin":
                preset = GearProfilePreset.Dragonkin;
                return true;
            case "human":
                preset = GearProfilePreset.Human;
                return true;
            case "orc":
                preset = GearProfilePreset.Orc;
                return true;
            default:
                preset = GearProfilePreset.Damage;
                return false;
        }
    }

    private static void EquipBestStackingPair(IScriptInterface bot, CoreBots core, string primaryMeta, string secondaryMeta)
    {
        if (string.IsNullOrWhiteSpace(primaryMeta))
            return;

        var candidates = bot.Inventory.Items
            .Concat(bot.Bank.Items)
            .Where(i => i != null)
            .Select(i => new BoostCandidate(
                i,
                GetSlotKey(i.Category.ToString()),
                core.GetBoostFloat(i, primaryMeta),
                core.GetBoostFloat(i, secondaryMeta)))
            .Where(c => c.Slot != null)
            .Where(c => !c.Item.Upgrade || bot.Player.IsMember)
            .Where(c => c.Slot != "Weapon" || c.Item.EnhancementLevel > 0)
            .ToList();

        var primaryPool = new List<BoostCandidate?>(candidates.Where(c => c.Primary > 0)) { null };
        var secondaryPool = new List<BoostCandidate?>(candidates.Where(c => c.Secondary > 0)) { null };

        BoostCandidate? bestPrimary = null;
        BoostCandidate? bestSecondary = null;
        double bestTotal = 0;

        foreach (BoostCandidate? primary in primaryPool)
        {
            foreach (BoostCandidate? secondary in secondaryPool)
            {
                if (primary == null && secondary == null)
                    continue;

                if (primary != null && secondary != null && primary.Slot == secondary.Slot && primary.Item.ID != secondary.Item.ID)
                    continue;

                double total = (primary?.Primary ?? 0) + (secondary?.Secondary ?? 0);
                if (total <= bestTotal)
                    continue;

                bestTotal = total;
                bestPrimary = primary;
                bestSecondary = secondary;
            }
        }

        if (bestTotal <= 0)
            return;

        if (bestPrimary != null)
            EquipCandidate(bot, core, bestPrimary);

        if (bestSecondary != null && (bestPrimary == null || bestSecondary.Item.ID != bestPrimary.Item.ID))
            EquipCandidate(bot, core, bestSecondary);
    }

    private static void EquipCandidate(IScriptInterface bot, CoreBots core, BoostCandidate candidate)
    {
        string name = candidate.Item.Name;
        if (bot.Inventory.IsEquipped(name))
            return;

        if (bot.Bank.Contains(name))
            core.Unbank(name);

        if (bot.Inventory.Contains(name))
            bot.Inventory.EquipItem(name);
    }

    private static string[] BuildRestoreOrder(Dictionary<string, string> bySlot, List<string> extras)
    {
        var ordered = new List<string>(bySlot.Count + extras.Count);

        foreach (string slot in RestoreSlotOrder)
        {
            if (bySlot.TryGetValue(slot, out string? item) && !string.IsNullOrWhiteSpace(item))
                ordered.Add(item);
        }

        ordered.AddRange(
            extras
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
        );

        return ordered.ToArray();
    }

    private static string? GetEquipmentSlotKey(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;

        return category switch
        {
            "Class" => "Class",
            _ => GetSlotKey(category),
        };
    }

    private static string? GetSlotKey(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;

        return category switch
        {
            "Armor" => "Armor",
            "Helm" => "Helm",
            "Cape" => "Cape",
            "Pet" => "Pet",
            "Sword" => "Weapon",
            "Axe" => "Weapon",
            "Dagger" => "Weapon",
            "Gun" => "Weapon",
            "HandGun" => "Weapon",
            "Rifle" => "Weapon",
            "Bow" => "Weapon",
            "Mace" => "Weapon",
            "Gauntlet" => "Weapon",
            "Polearm" => "Weapon",
            "Staff" => "Weapon",
            "Wand" => "Weapon",
            "Whip" => "Weapon",
            _ => null,
        };
    }

    private sealed class BoostCandidate
    {
        public BoostCandidate(InventoryItem item, string? slot, double primary, double secondary)
        {
            Item = item;
            Slot = slot;
            Primary = primary;
            Secondary = secondary;
        }

        public InventoryItem Item { get; }
        public string? Slot { get; }
        public double Primary { get; }
        public double Secondary { get; }
    }

    private sealed class GearPlan
    {
        public GearPlan(string primaryMeta, string secondaryMeta, string[] metaPriority)
        {
            PrimaryMeta = primaryMeta;
            SecondaryMeta = secondaryMeta;
            MetaPriority = metaPriority ?? Array.Empty<string>();
        }

        public string PrimaryMeta { get; }
        public string SecondaryMeta { get; }
        public string[] MetaPriority { get; }
    }
}

public sealed class EquipmentSnapshot
{
    public EquipmentSnapshot(Dictionary<string, string> bySlot, string[] orderedItems)
    {
        BySlot = bySlot ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        OrderedItems = orderedItems ?? Array.Empty<string>();
    }

    public Dictionary<string, string> BySlot { get; }
    public string[] OrderedItems { get; }
}
