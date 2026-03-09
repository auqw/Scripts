/*
name: UltraDarkon
description: Ultra Darkon spam taunt helper.
tags: ultra, darkon, taunt
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

// NOTE: In all compositions below, slot 2 and slot 4 are the taunter roles.
//
// Recommended Comp
// 1) LightCaster
// 2) Legion Revenant (Taunter)
// 3) StoneCrusher
// 4) Lord Of Order (Tauner)

// Light Caster:
// Weapon: Ravenous / Praxis
// Class: Lucky
// Helm: Pneuma
// Cape: Penitence / Lament
// Scroll: Enrage

// Legion Revenant:
// Weapon: Valiance / Ravenous / Arcana
// Class: Wizard
// Helm: Pneuma
// Cape: Penitence
// Scroll: Enrage

// Lord Of Order:
// Weapon: Lucky Aweblast / Valiance
// Class: Lucky
// Helm: Forge
// Cape: Absolution
// Scroll: Enrage

// StoneCrusher:
// Weapon: Valiance
// Class: Fighter
// Helm: Anima
// Cape: Absolution
// Scroll: Enrage
// Potion: Divine Elixir

// Alternate DPS Options (fallback order when LightCaster is unavailable):
// 1) Chrono ShadowSlayer / Chrono ShadowHunter
// 2) Arcana Invoker
// 3) King's Echo
// 4) Lich
// 5) Hollowborn Vindicator
// 6) Alpha Omega / Alpha DOOMmega


public class UltraDarkon
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDarkon";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    string a, b, c, d;
    string overrideA, overrideB, overrideC, overrideD;
    bool UseLifeSteal;
    bool EquipBestGear;
    bool DoEnhancements;
    bool RestoreGear;
    DarkonComp ActiveComp = DarkonComp.Recommended;

    public List<IOption> Main = new()
    {
        new Option<DarkonComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Slots 2 and 4 are always taunters.\n"
                + "Recommended: LC / LR / SC / LOO\n"
                + "Unselected = off (use manual classes below).",
            DarkonComp.Recommended
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-enhance for the currently equipped class", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Non-taunters equip/restock/use Scroll of Life Steal.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Secondary Class Override (T)", "Blank = use selected comp default for slot 2 (taunter).", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override (T)", "Blank = use selected comp default for slot 4 (taunter).", ""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("This script uses the spam taunt method.");

        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        ActiveComp = Bot.Config == null ? DarkonComp.Recommended : Bot.Config.Get<DarkonComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");

        a = overrideA;
        b = overrideB;
        c = overrideC;
        d = overrideD;

        bool usingComp = ActiveComp != DarkonComp.Unselected;
        if (!usingComp && (string.IsNullOrEmpty(b) || string.IsNullOrEmpty(d)))
        {
            C.Logger("Setup", "Fill taunter class overrides for slots 2 and 4.");
            Bot.StopSync();
            return;
        }

        Adv.GearStore();

        try
        {
            Run(ActiveComp, EquipBestGear, DoEnhancements, UseLifeSteal, overrideA, overrideB, overrideC, overrideD);
        }
        finally
        {
            if (RestoreGear)
                Adv.GearStore(true, true);
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    public void Run(
        DarkonComp comp = DarkonComp.Recommended,
        bool equipBestGear = true,
        bool doEnhancements = true,
        bool useLifeSteal = true,
        string? classAOverride = null,
        string? classBOverride = null,
        string? classCOverride = null,
        string? classDOverride = null
    )
    {
        ActiveComp = comp;
        EquipBestGear = equipBestGear;
        DoEnhancements = doEnhancements;
        UseLifeSteal = useLifeSteal;
        overrideA = classAOverride?.Trim() ?? string.Empty;
        overrideB = classBOverride?.Trim() ?? string.Empty;
        overrideC = classCOverride?.Trim() ?? string.Empty;
        overrideD = classDOverride?.Trim() ?? string.Empty;
        a = overrideA;
        b = overrideB;
        c = overrideC;
        d = overrideD;

        Core.Boot();
        Prep();
        Fight();
    }

    bool IsTaunter()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrWhiteSpace(currentClass))
            return false;

        if (!string.IsNullOrWhiteSpace(b) && currentClass.Equals(b, StringComparison.OrdinalIgnoreCase))
            return true;
        if (!string.IsNullOrWhiteSpace(d) && currentClass.Equals(d, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    void Prep()
    {
        if (ActiveComp != DarkonComp.Unselected)
            ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnhs();

        if (Bot.Player.CurrentClass?.Name == "StoneCrusher" || Bot.Player.CurrentClass?.Name == "Infinity Titan")
        {
            C.HuntMonster("poisonforest", "Xavier Lionfang", "Divine Elixir", 10, isTemp: false);
            Ultra.UseAlchemyPotions("Divine Elixir");
        }
        else
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();

        Bot.Sleep(2500);
    }

    void ApplyCompAndEquip(DarkonComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        string[][] classes;
        switch (comp)
        {
            case DarkonComp.Recommended:
                if (!string.IsNullOrWhiteSpace(aOverride))
                {
                    a = aOverride;
                }
                else if (C.CheckInventory("LightCaster"))
                {
                    a = "LightCaster";
                }
                else
                {
                    string[] fallbackOrder =
                    {
                        "Chrono ShadowSlayer",
                        "Chrono ShadowHunter",
                        "Arcana Invoker",
                        "King's Echo",
                        "Lich",
                        "Hollowborn Vindicator",
                        "Alpha Omega",
                        "Alpha DOOMmega",
                    };

                    a = fallbackOrder.FirstOrDefault(x => C.CheckInventory(x)) ?? "LightCaster";
                    C.Logger($"LightCaster not found. Using fallback DPS for slot 1: {a}");
                }
                b = string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride)
                    ? (C.CheckInventory("Infinity Titan") ? "Infinity Titan" : "StoneCrusher")
                    : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;

                classes = new[]
                {
                    new[] { a },
                    new[] { b },
                    new[] { c },
                    new[] { d }
                };
                break;

            default:
                throw new InvalidOperationException($"Unhandled DarkonComp value: {comp}");
        }

        Ultra.EquipClassSync(classes, 4, "darkon_class.sync", allowDuplicates: true);
    }

    void Fight()
    {
        if (!C.isCompletedBefore(8733))
        {
            C.Logger("Quest 8733 (The World) not completed. Run Story/ElegyofMadness(Darkon)/0CompleteAll.cs or use this script for kill support only.");
            Bot.Quests.UpdateQuest(8733);
        }

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8746);
        C.AddDrop("Darkon Insignia");

        Core.Join("ultradarkon");
        Ultra.WaitForArmy(3, "Ultra_Darkon.sync");
        Core.ChooseBestCell("Darkon the Conductor");
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Bot.Map?.Name != "ultradarkon")
            {
                Core.Join("ultradarkon");
                Core.ChooseBestCell("Darkon the Conductor");
                Bot.Player.SetSpawnPoint();
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Darkon the Conductor Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8746);
                Bot.Wait.ForPickup("Darkon Insignia");
                break;
            }

            if (Bot.Player?.Alive == false)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                if (Bot.Player.CurrentClass?.Name == "StoneCrusher" || Bot.Player.CurrentClass?.Name == "Infinity Titan")
                {
                    Ultra.UseAlchemyPotions("Divine Elixir");
                    Ultra.BuyAlchemyPotion("Potent Honor Potion");
                    Core.EquipConsumable("Potent Honor Potion");
                    Bot.Sleep(2500);
                }

                if (IsTaunter())
                    Core.EquipEnrage();
                else if (UseLifeSteal)
                    Ultra.GetScrollOfLifeSteal();

                continue;
            }

            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");

            Bot.Sleep(200);

            // Non-taunter role: use Scroll of Life Steal (equipped in Prep via CoreUltra helper).
            if (UseLifeSteal && !IsTaunter() && Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            // Taunter role: use Scroll of Enrage to apply Focus.
            if (IsTaunter()
                && Bot.Player?.Target != null
                && Bot.Player.Target.HP > 0
                && !Bot.Target.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            case "LightCaster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "StoneCrusher":
            case "Infinity Titan":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Chrono ShadowSlayer":
            case "Chrono ShadowHunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Paladin Chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Alpha Omega":
            case "Alpha DOOMmega":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Avarice
                );
                break;

            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Hollowborn Vindicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;
        }
    }

    void EquipBestDmgGear()
    {
        C.EquipBestItemsForMeta(
            new Dictionary<string, string[]>
            {
                { "Weapon", new[] { "dmgAll", "dmg", "damage" } },
                { "Armor", new[] { "dmgAll", "dmg", "damage" } },
                { "Helm", new[] { "dmgAll", "dmg", "damage" } },
                { "Cape", new[] { "dmgAll", "dmg", "damage" } },
                { "Pet", new[] { "dmgAll", "dmg", "damage" } },
            }
        );
    }

    public enum DarkonComp
    {
        Unselected,
        Recommended,
    }
}
