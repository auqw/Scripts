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

    string a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath";
    public List<IOption> Options = new()
    {
        new Option<string>("a", "Taunter Class (Primary)", "Class name that will taunt first", "ArchPaladin"),
        new Option<string>("b", "Taunter Class (Backup)", "Backup taunter class", "Chaos Slayer"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();

        if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
        {
            Core.Log(
                "Setup",
                "Fill at least one taunter class (Primary or Backup) in Script Options."
            );
            Bot.Stop();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    bool IsTaunter() => Bot.Player.CurrentClass.Name.Contains(a) || Bot.Player.CurrentClass.Name.Contains(b);

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else
        {
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
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

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Champion Drakath Defeated", 1), syncPath))
            {
                Bot.Sleep(2500);
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(8300))
                    C.EnsureComplete(8300);
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Core.HasClassEquipped(a) || Core.HasClassEquipped(b))
            {
                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack(boss);
                Ultra.DrakathTaunter();
                Bot.Sleep(500);
                continue;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);

            Bot.Sleep(250);
            if (Bot.Player?.Target?.HP < Bot.Player?.Target?.MaxHP * 0.1
            && (Core.HasClassEquipped(a) || Core.HasClassEquipped(b)))
            {
                Bot.Skills.UseSkill(5);
                Bot.Sleep(250);
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
                    type: EnhancementType.Wizard,                // Class // Healer
                    hSpecial: HelmSpecial.Pneuma,                // Helm
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
