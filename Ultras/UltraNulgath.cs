/*
name: UltraNulgath
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreGearUtils.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

// NOTE: In all compositions below, slot 1 and slot 2 are the taunter roles.

#region Fast Comp
// Chrono ShadowSlayer: Lucky | Vim | Valiance | Vainglory
// Verus DoomKnight: Lucky | Anima | Ravenous | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// Lord Of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region F2P Fast
// Dragon of Time (x2): Wizard | Pneuma | Elysium | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// Lord Of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Common Comp
// King's Echo: Lucky | Examen | Ravenous | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// ArchPaladin: Lucky | Forge | Valiance | Lament
// Lord Of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Balanced Comp
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// ArchPaladin: Lucky | Forge | Valiance | Lament
// StoneCrusher: Wizard | Pneuma | Valiance | Vainglory
// Lord Of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Other DPS Options
// Arcana Invoker: Lucky | Examen | Ravenous | Vainglory
// Archfiend: Lucky | Forge | Ravenous | Vainglory
// Lich: Lucky | Examen | Ravenous | Vainglory
#endregion

public class UltraNulgath
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
    public string OptionsStorage = "UltraNulgath";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    string a, b, c, d;
    string overrideA, overrideB, overrideC, overrideD;
    bool UseLifeSteal;

    private const string LifeStealScroll = "Scroll of Life Steal";
    private const string LifeStealShopMap = "terminatemple";
    private const int LifeStealShopId = 2328;
    private const int LifeStealMinStock = 10;
    private const int LifeStealRestockTo = 50;

    public List<IOption> Main = new()
    {
        new Option<NulgathComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Slots 1 and 2 are always taunters.\n"
                + "Fast: CSS / VDK / LR / LOO\n"
                + "F2PFast: DoT / DoT / LR / LOO\n"
                + "Common: KE / LR / AP / LOO\n"
                + "Balanced: LR / AP / SC / LOO\n"
                + "Unselected = off (use manual classes below).",
            NulgathComp.Fast
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-enhance for the currently equipped class", true),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Non-taunters equip/restock/use Scroll of Life Steal.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1 (taunter).", ""),
        new Option<string>("b", "Secondary Class Override", "Blank = use selected comp default for slot 2 (taunter).", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4.", ""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "Ultra Nulgath",
            "Deaths more then likely will happen, Suggested class and their enhs are in the script at the top"
        );

        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        NulgathComp comp = Bot.Config!.Get<NulgathComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty).Trim();
        overrideB = (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty).Trim();
        overrideC = (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty).Trim();
        overrideD = (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty).Trim();

        a = overrideA;
        b = overrideB;
        c = overrideC;
        d = overrideD;

        UseLifeSteal = Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");

        bool usingComp = comp != NulgathComp.Unselected;
        if (!usingComp && (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)))
        {
            C.Logger("Setup", "Fill taunter class overrides for slots 1 and 2.");
            Bot.StopSync();
            return;
        }

        EquipmentSnapshot equippedBefore = CoreGearUtils.CaptureEquipment(Bot);

        try
        {
            Core.Boot();
            Prep();
            Fight();
        }
        finally
        {
            CoreGearUtils.RestoreEquipment(Bot, C, equippedBefore);
            C.SetOptions(false);
        }
    }

    bool IsTaunter()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrWhiteSpace(currentClass))
            return false;

        if (!string.IsNullOrWhiteSpace(a) && currentClass.Equals(a, StringComparison.OrdinalIgnoreCase))
            return true;
        if (!string.IsNullOrWhiteSpace(b) && currentClass.Equals(b, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    void Prep()
    {
        NulgathComp comp = Bot.Config!.Get<NulgathComp>("Main", "DoEquipClasses");
        if (comp != NulgathComp.Unselected)
            ApplyCompAndEquip(comp, overrideA, overrideB, overrideC, overrideD);

        if (Bot.Config.Get<bool>("CoreSettings", "EquipBestGear"))
            CoreGearUtils.EquipBestGear(Bot, C, GearProfilePreset.Damage);

        if (Bot.Config.Get<bool>("CoreSettings", "DoEnh"))
            DoEnhs();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            EnsureLifeStealScroll();
    }

    void EnsureLifeStealScroll()
    {
        C.Unbank(LifeStealScroll);
        int qty = Bot.Inventory.GetQuantity(LifeStealScroll);
        if (qty < LifeStealMinStock)
        {
            C.BuyItem(LifeStealShopMap, LifeStealShopId, LifeStealScroll, LifeStealRestockTo);
            qty = Bot.Inventory.GetQuantity(LifeStealScroll);
        }

        if (qty > 0)
            Core.EquipConsumable(LifeStealScroll);
    }

    void ApplyCompAndEquip(NulgathComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        string[][] classes;
        switch (comp)
        {
            case NulgathComp.Fast:
                a = string.IsNullOrWhiteSpace(aOverride)
                    ? (C.CheckInventory("Chrono ShadowSlayer") ? "Chrono ShadowSlayer" : "Chrono ShadowHunter")
                    : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "Verus DoomKnight" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? "Legion Revenant" : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;
                classes = new[]
                {
                    string.IsNullOrWhiteSpace(aOverride)
                        ? new[] { "Chrono ShadowSlayer", "Chrono ShadowHunter" }
                        : new[] { aOverride },
                    new[] { b },
                    new[] { c },
                    new[] { d }
                };
                break;

            case NulgathComp.F2PFast:
                a = string.IsNullOrWhiteSpace(aOverride) ? "Dragon of Time" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "Dragon of Time" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? "Legion Revenant" : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;
                classes = new[]
                {
                    new[] { a },
                    new[] { b },
                    new[] { c },
                    new[] { d }
                };
                break;

            case NulgathComp.Common:
                a = string.IsNullOrWhiteSpace(aOverride) ? "King's Echo" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;
                classes = new[]
                {
                    new[] { a },
                    new[] { b },
                    new[] { c },
                    new[] { d }
                };
                break;

            case NulgathComp.Balanced:
                a = string.IsNullOrWhiteSpace(aOverride) ? "Legion Revenant" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? (C.CheckInventory("Infinity Titan") ? "Infinity Titan" : "StoneCrusher") : cOverride;
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
                throw new NotImplementedException();
        }

        Ultra.EquipClassSync(classes, 4, "nulgath_class.sync", allowDuplicates: true);
    }

    // Overfiend Blade = 1
    // Nulgath = 2
    void Fight()
    {
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_nulgath.sync");

        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Nulgath the Archfiend Defeated?", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                Core.DisableSkills();
                Bot.Sleep(200);
                C.Jump("Enter", "Spawn");
                C.Join("whitemap");
                if (!Bot.Quests.IsDailyComplete(8692))
                    C.EnsureComplete(8692);
                break;
            }

            if (IsTaunter())
            {
                Bot.Combat.Attack(2);
                Bot.Sleep(200);
            }
            else
            {
                if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
                    Bot.Combat.Attack(1);
                else
                    Bot.Combat.Attack(2);
                Bot.Sleep(200);
            }

            if (UseLifeSteal && !IsTaunter() && Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            if (Bot.Player.Alive
                && IsTaunter()
                && !Bot.Target.Auras.Any(x => x?.Name == "Focus")
                && Bot.Monsters.MapMonsters.Any(x => (x?.MapID == 2 || x?.MapID == 1) && x.HP > 0))
            {
                Core.DisableSkills();
                while (!Bot.ShouldExit && !Bot.Target.Auras.Any(x => x?.Name == "Focus"))
                {
                    if (!Bot.Player.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        continue;
                    }

                    if (!Bot.Target.Auras.Any(x => x != null && x.Name == "Focus") && Bot.Skills.CanUseSkill(5))
                        Bot.Skills.UseSkill(5);
                    else
                        break;

                    Bot.Sleep(500);
                }
                Core.EnableSkills();
            }
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            case "Chrono ShadowSlayer":
            case "Chrono ShadowHunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "StoneCrusher":
            case "Infinity Titan":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

    public enum NulgathComp
    {
        Unselected,
        Fast,
        F2PFast,
        Common,
        Balanced,
    }
}
