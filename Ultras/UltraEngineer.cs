/*
name: UltraEngineer
description: Ultra Engineer helper prioritizing drones with army synchronization and consumables.
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

#region Fast Comp

/// <summary>
/// Fast Composition - Maximum damage output for speed
/// </summary>
// Lich
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Ravenous
// └─ Cape: Lament
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// Arch Paladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord Of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region Safe Comp

/// <summary>
/// Safe Composition - Balanced survivability and damage
/// </summary>
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// StoneCrusher
// ├─ Class: Fighter
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// └─ Cape: Absolution
//
// Arch Paladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord Of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region F2P Fast (no Lich)

/// <summary>
/// F2P Fast Composition - Budget-friendly speed setup without Lich
/// </summary>
// Arcana Invoker
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Ravenous / Valiance
// └─ Cape: Vainglory
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// Arch Paladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord Of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region Other DPS Options

/// <summary>
/// Other DPS Options - Alternative single-class configurations
/// </summary>
// Chrono ShadowSlayer
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Vainglory / Lament
//
// Verus Doomknight
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Ravenous / Valiance
// └─ Cape: Vainglory
//
// Void Highlord
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Valiance / Ravenous
// └─ Cape: Vainglory
//
// Chaos Avenger
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// └─ Cape: Vainglory

#endregion

public class UltraEngineer
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
    public string OptionsStorage = "UltraEngineer";
    public List<IOption> Options = new()
    {
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

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
    }

    void Fight()
    {
        const string map = "ultraengineer";
        const string boss = "Ultra Engineer";
        const string priority1 = "Defense Drone";
        const string priority2 = "Attack Drone";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8154);
        C.AddDrop("Engineer Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_engineer.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Engineer Defeated", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                C.EnsureComplete(8154);
                Adv.GearStore(true, true);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }
            Ultra.KillWithPriority(boss, 3, priority1, 2, priority2, 1);
            Bot.Skills.UseSkill(5);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;
        Adv.GearStore();

        switch (className)
        {
            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Arch Paladin
            case "Arch Paladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Lord Of Order
            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // StoneCrusher
            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Verus Doomknight
            case "Verus Doomknight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Void Highlord
            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Chaos Avenger
            case "Chaos Avenger":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }
}
