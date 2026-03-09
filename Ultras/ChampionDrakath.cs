/*
name: ChampionDrakath
description: Champion Drakath helper with threshold-based taunt timing and army sync.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Options;
using System.Collections.Generic;


#region Class & Enhancement Setup
/// <summary>
/// Champion Drakath Enhancement Configurations
/// Organized by composition type: Safe, Fast, and Cheapest
/// </summary>
#region Safe Comp

/// <summary>
/// Safe Composition - Balanced approach for consistent performance
/// </summary>
// ArchPaladin (Taunter)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Legion Revenant
// ├─ Helm: Wizard / Healer
// ├─ Class: Wizard / Healer
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// StoneCrusher
// ├─ Helm: Anima
// ├─ Class: Fighter
// ├─ Weapon: Valiance
// └─ Cape: Absolution
//
// Lord Of Order
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region Fast Comp

/// <summary>
/// Fast Composition - Optimized for speed and burst damage
/// </summary>
// Chrono ShadowSlayer/ShadowHunter (Taunter)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Vainglory / Lament
//
// Legion Revenant (Taunter)
// ├─ Helm: Wizard / Healer
// ├─ Class: Wizard / Healer
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// Paladin Chronomancer / Obsidian Paladin Chronomancer
// ├─ Helm: Healer / Wizard / Pneuma
// ├─ Class: Healer / Wizard
// ├─ Weapon: Healer / Wizard Mana Vamp
// └─ Cape: Absolution
//
// Lord Of Order
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Absolution

#endregion

#region Cheapest Comp

/// <summary>
/// Cheapest Composition - Cost-effective setup with minimal investment
/// </summary>
// Chaos Slayer Berserker/Cleric/Mystic/Thief (taunt)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// ArchPaladin (Taunter)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// StoneCrusher
// ├─ Helm: Anima
// ├─ Class: Fighter
// ├─ Weapon: Valiance
// └─ Cape: Absolution
//
// Lord Of Order
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#endregion

public class ChampionDrakath
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
    public string OptionsStorage = "ChampionDrakath";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };
    string a, b, c, d;
    string overrideA, overrideB, overrideC, overrideD;
    int previousHP = 0;
    private static int[] hpThresholds = { 18100000, 16100000, 14100000, 12100000, 10100000, 8100000, 6100000, 4100000 };
    bool EquipBestGear;
    bool DoEnhancements;
    bool RestoreGear;
    int TaunterCount = 2;
    DrakathComp ActiveComp = DrakathComp.Safe;

    public List<IOption> Main = new()
    {
        new Option<DrakathComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Safe: AP / LR / SC / LOO\n"
                + "Fast: CSS/CSH / LR / PCM/OPCM / LOO\n"
                + "Cheapest: CS / AP / SC / LOO\n"
                + "Unselected = off (use manual classes below).",
            DrakathComp.Safe
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Non-taunters equip/restock/use Scroll of Life Steal.", true),
        new Option<HowManyTaunts>("HowManyTaunts", "How many taunters", "", HowManyTaunts.Two),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Secondary Class Override", "Blank = use selected comp default for slot 2.", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4.", ""),
    };

    bool UseLifeSteal;

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null
            && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        ActiveComp = Bot.Config == null ? DrakathComp.Safe : Bot.Config.Get<DrakathComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        a = overrideA;
        b = overrideB;
        c = overrideC;
        d = overrideD;
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");
        TaunterCount = Bot.Config == null ? 2 : (int)Bot.Config.Get<HowManyTaunts>("CoreSettings", "HowManyTaunts");

        bool usingComp = ActiveComp != DrakathComp.Unselected;
        if (!usingComp && (
            string.IsNullOrEmpty(a)
            || (TaunterCount >= 2 && string.IsNullOrEmpty(b))
            || (TaunterCount >= 3 && string.IsNullOrEmpty(c))
            || (TaunterCount >= 4 && string.IsNullOrEmpty(d))
        ))
        {
            Core.Log("Setup", "Fill taunter class overrides for all enabled taunter slots.");
            Bot.StopSync();
            return;
        }

        Adv.GearStore();

        try
        {
            Run(ActiveComp, EquipBestGear, DoEnhancements, UseLifeSteal, TaunterCount, overrideA, overrideB, overrideC, overrideD);
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
        DrakathComp comp = DrakathComp.Safe,
        bool equipBestGear = true,
        bool doEnhancements = true,
        bool useLifeSteal = true,
        int taunterCount = 2,
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
        TaunterCount = Math.Max(1, Math.Min(4, taunterCount));
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
        C.JumpWait();
    }

    bool IsTaunter()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;

        if (string.IsNullOrEmpty(currentClass))
            return false;

        // Check based on HowManyTaunts setting
        if (TaunterCount >= 1 && !string.IsNullOrEmpty(a) && currentClass.Contains(a))
            return true;
        if (TaunterCount >= 2 && !string.IsNullOrEmpty(b) && currentClass.Contains(b))
            return true;
        if (TaunterCount >= 3 && !string.IsNullOrEmpty(c) && currentClass.Contains(c))
            return true;
        if (TaunterCount >= 4 && !string.IsNullOrEmpty(d) && currentClass.Contains(d))
            return true;

        return false;
    }

    void Prep()
    {
        if (ActiveComp != DrakathComp.Unselected)
            ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnhs();

        Ultra.UseAlchemyPotions(
            Ultra.GetBestTonicPotion(),
            Ultra.GetBestElixirPotion()
        );
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
    }

    void ApplyCompAndEquip(DrakathComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        string[][] classes;
        switch (comp)
        {
            case DrakathComp.Safe:
                a = string.IsNullOrWhiteSpace(aOverride) ? "ArchPaladin" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride;
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

            case DrakathComp.Fast:
                a = string.IsNullOrWhiteSpace(aOverride) ? "Chrono Shadow" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? "Paladin Chronomancer" : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;
                classes = new[]
                {
                    string.IsNullOrWhiteSpace(aOverride)
                        ? new[] { "Chrono ShadowSlayer", "Chrono ShadowHunter" }
                        : new[] { aOverride },
                    new[] { b },
                    string.IsNullOrWhiteSpace(cOverride)
                        ? new[] { "Paladin Chronomancer", "Obsidian Paladin Chronomancer" }
                        : new[] { cOverride },
                    new[] { d }
                };
                break;

            case DrakathComp.Cheapest:
                a = string.IsNullOrWhiteSpace(aOverride) ? "Chaos Slayer" : aOverride;
                b = string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride;
                c = string.IsNullOrWhiteSpace(cOverride) ? (C.CheckInventory("Infinity Titan") ? "Infinity Titan" : "StoneCrusher") : cOverride;
                d = string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride;
                classes = new[]
                {
                    string.IsNullOrWhiteSpace(aOverride)
                        ? new[] { "Chaos Slayer", "Chaos Slayer Berserker", "Chaos Slayer Cleric", "Chaos Slayer Mystic", "Chaos Slayer Thief" }
                        : new[] { aOverride },
                    new[] { b },
                    new[] { c },
                    new[] { d }
                };
                break;

            default:
                throw new NotImplementedException();
        }

        Ultra.EquipClassSync(classes, 4, "champion_drakath_class.sync", allowDuplicates: true);
    }

    void Fight()
    {
        const string map = "championdrakath";
        const string boss = "Champion Drakath";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8300);
        C.AddDrop("Champion Drakath Insignia");

        Core.Join(map);
        Ultra.WaitForArmy(3, "champion_drakath.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        bool[] tauntFired = new bool[8]; // 18M-4M in 2M chunks
        previousHP = 0; // Reset at fight start

        while (!Bot.ShouldExit)
        {
            if (Bot.Map?.Name != map)
            {
                Core.Join(map);
                Core.ChooseBestCell(boss);
                Bot.Player.SetSpawnPoint();
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Champion Drakath Defeated"), syncPath))
            {
                Bot.Sleep(2500);
                C.Jump("Enter", "Spawn");
                if (!Bot.Quests.IsDailyComplete(8300))
                    C.EnsureComplete(8300);
                else Bot.Log("Daily already Complete");
                break;
            }

            Bot.Combat.Attack("*");

            Bot.Sleep(500);

            // Non-taunter role: use Scroll of Life Steal
            if (UseLifeSteal && !IsTaunter() && Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            // Only execute taunt logic if this account is a taunter
            if (IsTaunter()
                && Bot.Player.HasTarget
                && !Bot.Target.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Player.Target?.HP > 0)
            {
                Core.DisableSkills();
                Bot.Sleep(500);

                // Detect HP reset (boss respawned/wiped)
                if (Bot.Player.Target?.HP > previousHP + 1000000) // HP increased significantly
                {
                    C.Logger("Boss HP reset detected - clearing taunt flags");
                    for (int j = 0; j < tauntFired.Length; j++)
                    {
                        tauntFired[j] = false;
                    }
                }

                previousHP = Bot.Player.Target?.HP ?? 0;

                // Check thresholds (18M down to 4M)
                for (int i = 0; i < hpThresholds.Length; i++)
                {
                    if (!tauntFired[i] && Bot.Player.HasTarget && Bot.Player.Target?.HP <= hpThresholds[i])
                    {
                        C.Logger($"{hpThresholds[i] / 1000000}M - Taunting at HP {Bot.Player.Target?.HP:n0}");

                        while (!Bot.ShouldExit)
                        {
                            if (!Bot.Player.Alive)
                            {
                                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                                // Reset tauntFired from current threshold onwards
                                for (int j = i; j < tauntFired.Length; j++)
                                {
                                    tauntFired[j] = false;
                                }
                                break;
                            }

                            if (!Bot.Player.HasTarget)
                                break;

                            // Taunter role: use Scroll of Enrage to apply Focus.
                            if (Bot.Skills.CanUseSkill(5))
                                Bot.Skills.UseSkill(5);

                            Bot.Sleep(500);

                            if (Bot.Player.HasTarget && Bot.Target.Auras.Any(x => x != null && x.Name == "Focus"))
                            {
                                tauntFired[i] = true;
                                Bot.Sleep(500);
                                break;
                            }
                        }

                        Bot.Sleep(300);
                        break; // Exit after firing one taunt
                    }
                }

                // After 2M → always taunt
                if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 2100000 && Bot.Skills.CanUseSkill(5))
                {
                    C.Logger($"HP is < 2M, Taunting at HP {Bot.Player.Target?.HP:n0}");

                    while (!Bot.ShouldExit)
                    {
                        // Taunter role: keep using Scroll of Enrage below 2M.
                        if (Bot.Skills.CanUseSkill(5))
                            Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);

                        if (Bot.Player.HasTarget && Bot.Target.Auras.Any(x => x != null && x.Name == "Focus"))
                        {
                            Core.EnableSkills();
                            break;
                        }
                    }

                    Bot.Sleep(300);
                }

            }
        }

        C.JumpWait();
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Forge,              // Helm
                    wSpecial: WeaponSpecial.Valiance,               // Weapon
                    cSpecial: CapeSpecial.Lament                 // Cape
                );
                break;

            // Light Caster
            case "LightCaster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Pneuma,                // Helm
                    wSpecial: WeaponSpecial.Ravenous,            // Weapon // Praxis
                    cSpecial: CapeSpecial.Penitence              // Cape // Lament
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,                // Class // Healer
                    hSpecial: HelmSpecial.None,                // Helm
                    wSpecial: WeaponSpecial.Valiance,            // Weapon // Ravenous / Arcanas_Concerto
                    cSpecial: CapeSpecial.Vainglory              // Cape // Penitence
                );
                break;

            // Lord Of Order
            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Forge,                 // Helm
                    wSpecial: WeaponSpecial.Valiance,            // Weapon // Lucky_Aweblast
                    cSpecial: CapeSpecial.Absolution             // Cape
                );
                break;

            // StoneCrusher
            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,               // Class
                    hSpecial: HelmSpecial.Anima,                 // Helm
                    wSpecial: WeaponSpecial.Valiance,            // Weapon
                    cSpecial: CapeSpecial.Absolution             // Cape
                );
                break;

            // Chrono ShadowSlayer / ShadowHunter
            case "Chrono ShadowSlayer":
            case "Chrono ShadowHunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Forge,                 // Helm
                    wSpecial: WeaponSpecial.Valiance,            // Weapon // Arcanas_Concerto
                    cSpecial: CapeSpecial.Vainglory              // Cape // Lament
                );
                break;

            // Paladin Chronomancer
            case "Paladin Chronomancer":
            case "Obsidian Paladin Chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,                // Class // Wizard
                    hSpecial: HelmSpecial.Pneuma,                // Helm // Wizard
                    wSpecial: WeaponSpecial.Mana_Vamp,           // Weapon // Wizard Mana Vamp
                    cSpecial: CapeSpecial.Absolution             // Cape
                );
                break;

            // Alpha Omega / Alpha DOOMmega
            case "Alpha Omega":
            case "Alpha DOOMmega":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Vim,                   // Helm
                    wSpecial: WeaponSpecial.Praxis,              // Weapon
                    cSpecial: CapeSpecial.Avarice                // Cape
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Examen,                // Helm // Forge
                    wSpecial: WeaponSpecial.Ravenous,            // Weapon // Valiance
                    cSpecial: CapeSpecial.Lament                 // Cape
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Examen,                // Helm
                    wSpecial: WeaponSpecial.Ravenous,            // Weapon
                    cSpecial: CapeSpecial.Penitence              // Cape
                );
                break;

            // Hollowborn Vindicator
            case "Hollowborn VIndicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Forge,                 // Helm
                    wSpecial: WeaponSpecial.Dauntless,           // Weapon
                    cSpecial: CapeSpecial.Penitence              // Cape
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Examen,                // Helm
                    wSpecial: WeaponSpecial.Ravenous,            // Weapon
                    cSpecial: CapeSpecial.Lament                 // Cape
                );
                break;

            // Chaos Slayer
            case "Chaos Slayer":
            case "Chaos Slayer Berserker":
            case "Chaos Slayer Cleric":
            case "Chaos Slayer Mystic":
            case "Chaos Slayer Thief":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,                 // Class
                    hSpecial: HelmSpecial.Forge,                 // Helm
                    wSpecial: WeaponSpecial.Ravenous,            // Weapon // Valiance
                    cSpecial: CapeSpecial.Lament                 // Cape
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


    enum HowManyTaunts
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    public enum DrakathComp
    {
        Unselected,
        Safe,
        Fast,
        Cheapest
    }
}
