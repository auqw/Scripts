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
// King's Echo
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
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
// Chrono ShadowSlayer (Taunter)
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
// Paladin Chronomancer
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

    string a, b;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath";

    public List<IOption> Options = new()
{
    new Option<string>("a", "Taunter Class (Primary)", "Class name that will taunt first", "ArchPaladin"),
    new Option<string>("b", "Taunter Class (Backup)", "Backup taunter class", "Chaos Slayer"),
    new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
    new Option<bool>("SoloTaunt", "Only 1 Taunter", "Only use a single Taunter", false),
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
        SoloTaunt = Bot.Config.Get<bool>("SoloTaunt");

        // FIXED VALIDATION
        if ((SoloTaunt && string.IsNullOrEmpty(a))
            || (!SoloTaunt && string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)))
        {
            Core.Log(
                "Setup",
                "Primary taunter is required. Backup is optional unless Solo Taunt is disabled."
            );
            Bot.Stop();
            return;
        }

        // Ignore backup completely when solo
        if (SoloTaunt)
            b = string.Empty;

        Core.Boot();
        C.Join("whitemap-100000");
        Prep();
        Fight();
        C.JumpWait();
        C.SetOptions(false);
    }

    bool IsTaunter()
    {
        return SoloTaunt
            ? Bot.Player.CurrentClass.Name.Contains(a)
            : (!string.IsNullOrEmpty(a) && Bot.Player.CurrentClass.Name.Contains(a))
              || (!string.IsNullOrEmpty(b) && Bot.Player.CurrentClass.Name.Contains(b));
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
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        bool[] tauntFired = new bool[8]; // 18–2M chunks

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Champion Drakath Defeated"), syncPath))
            {
                Bot.Sleep(2500);
                C.Jump("Enter", "Spawn");
                if (!Bot.Quests.IsDailyComplete(8300))
                    C.EnsureComplete(8300);
                break;
            }

            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);

            Bot.Sleep(500);

            if ((Core.HasClassEquipped(a) || Core.HasClassEquipped(b))
                && !Bot.Self.Auras.Any(x => x.Name == "Focus")
                && Bot.Player.Target?.HP > 0)
            {
                int hp = Bot.Player.Target.HP;

                // Normal chunks
                if (!tauntFired[0] && hp <= 18_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}"); while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[0] = true; Bot.Sleep(300);
                }
                else if (!tauntFired[1] && hp <= 16_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[1] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[2] && hp <= 14_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[2] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[3] && hp <= 12_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[3] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[4] && hp <= 10_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[4] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[5] && hp <= 8_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[5] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[6] && hp <= 6_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[6] = true;
                    Bot.Sleep(300);
                }
                else if (!tauntFired[7] && hp <= 4_000_000)
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
                    }
                    tauntFired[7] = true;
                    Bot.Sleep(300);
                }
                // After 2M → always taunt
                else if (hp <= 2_000_000 && Bot.Skills.CanUseSkill(5))
                {
                    Bot.Log($"Taunting at HP {hp:n0}");
                    while (!Bot.ShouldExit && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    {
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(500);
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
}
