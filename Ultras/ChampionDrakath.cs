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
// Lord of Order
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
// Lord of Order
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
// Lord of Order
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
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();


    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakathTauntSelect";
    string a, b, c, d;
    int previousHP = 0;
    private static int[] hpThresholds = { 18100000, 16100000, 14100000, 12100000, 10100000, 8100000, 6100000, 4100000 };

    public List<IOption> Options = new()
    {
        new Option<string>("a", "Taunter Class (Primary)", "", "ArchPaladin"),
        new Option<string>("b", "Taunter Class (Secondary)", "", "Legion Revenant"),
        new Option<string>("c", "Taunter Class (Tertiary)", "", "StoneCrusher"),
        new Option<string>("d", "Taunter Class (Quaternary)", "", "Lord of Order"),
        new Option<bool>("SoloTaunt", "Solo Taunt", "Only primary taunter", false),
        new Option<bool>("DoEnh", "Do Enhancements", "", true),
        new Option<HowManyTaunts>("HowManyTaunts", "How many taunters", "", HowManyTaunts.Two),
        CoreBots.Instance.SkipOptions,
    };

    bool SoloTaunt;

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions))
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? string.Empty).Trim();
        b = (Bot.Config.Get<string>("b") ?? string.Empty).Trim();
        c = (Bot.Config.Get<string>("c") ?? string.Empty).Trim();
        d = (Bot.Config.Get<string>("d") ?? string.Empty).Trim();
        SoloTaunt = Bot.Config.Get<bool>("SoloTaunt");

        if ((SoloTaunt && string.IsNullOrEmpty(a))
            || (!SoloTaunt && string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)))
        {
            Core.Log("Setup", "Primary taunter required.");
            Bot.StopSync();
            return;
        }

        if (SoloTaunt)
        {
            b = string.Empty;
            c = string.Empty;
            d = string.Empty;
        }

        Core.Boot();
        Prep();
        Fight();
        C.JumpWait();
        C.SetOptions(false);
    }

    bool IsTaunter()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;

        if (string.IsNullOrEmpty(currentClass))
            return false;

        // Check based on HowManyTaunts setting
        int taunterCount = (int)Bot.Config!.Get<HowManyTaunts>("HowManyTaunts");

        if (taunterCount >= 1 && !string.IsNullOrEmpty(a) && currentClass.Contains(a))
            return true;
        if (taunterCount >= 2 && !string.IsNullOrEmpty(b) && currentClass.Contains(b))
            return true;
        if (taunterCount >= 3 && !string.IsNullOrEmpty(c) && currentClass.Contains(c))
            return true;
        if (taunterCount >= 4 && !string.IsNullOrEmpty(d) && currentClass.Contains(d))
            return true;

        return false;
    }

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        Ultra.UseAlchemyPotions(
            Ultra.GetBestTonicPotion(),
            Ultra.GetBestElixirPotion()
        );

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
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
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        bool[] tauntFired = new bool[8]; // 18M-4M in 2M chunks
        previousHP = 0; // Reset at fight start

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != bestCell)
            {
                Bot.Sleep(200);
                Bot.Map.Jump(bestCell, bestPad);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Champion Drakath Defeated"), syncPath))
            {
                Bot.Sleep(2500);
                C.Jump("Enter", "Spawn");
                if (!Bot.Quests.IsDailyComplete(8300))
                    C.EnsureComplete(8300);
                else Bot.Log("Daily already Complete");
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            Bot.Combat.Attack("*");

            Bot.Sleep(500);

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

            // Lord of Order
            case "Lord of Order":
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


    enum HowManyTaunts
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
}
