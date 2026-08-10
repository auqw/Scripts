/*
name: UltraEzrajal
description: Ultra Ezrajal helper handling Counter Attack windows with army sync.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System.ComponentModel;
using System.Reflection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;
#nullable enable

#region Comps
/// <summary>
/// Champion Drakath Enhancement Configurations
/// Organized by composition type: Fast, Safe, F2P Fastest, and Solo Options
/// </summary>

#region Fast Comp

/// <summary>
/// Fast Composition - Optimized for speed and burst damage
/// </summary>
// Chrono ShadowSlayer
// ├─ Class: Lucky
// ├─ Helm: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Vainglory / Lament
//
// Verus DoomKnight
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Ravenous / Valiance
// └─ Cape: Vainglory
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Wizard
// ├─ Weapon: Ravenous / Valiance / Arcana
// └─ Cape: Vainglory
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region Safe Comp

/// <summary>
/// Safe Composition - Balanced approach for consistent performance
/// </summary>
// Arcana Invoker
// ├─ Class: Lucky
// ├─ Helm: Healer
// ├─ Weapon: Elysium
// └─ Cape: Absolution
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Wizard
// ├─ Weapon: Ravenous / Valiance / Arcana
// └─ Cape: Vainglory
//
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region F2P Fastest

/// <summary>
/// F2P Fastest Composition - Cost-optimized for free-to-play players
/// </summary>
// Arcana Invoker
// ├─ Class: Lucky
// ├─ Helm: Healer
// ├─ Weapon: Elysium
// └─ Cape: Absolution
//
// Verus DoomKnight
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Ravenous / Valiance
// └─ Cape: Vainglory
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Wizard
// ├─ Weapon: Ravenous / Valiance / Arcana
// └─ Cape: Vainglory
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Lucky Aweblast / Valiance
// └─ Cape: Absolution

#endregion

#region Solo Options

/// <summary>
/// Solo Options - Individual class configurations for solo play
/// </summary>
// Arcana Invoker
// ├─ Class: Lucky
// ├─ Helm: Healer
// ├─ Weapon: Elysium
// └─ Cape: Absolution
//
// Dragon of Time
// ├─ Class: Healer
// ├─ Helm: Healer
// ├─ Weapon: Elysium
// └─ Cape: Absolution
//
// Void Highlord
// ├─ Class: Lucky
// ├─ Helm: Forge / Anima
// ├─ Weapon: Valiance / Ravenous
// └─ Cape: Vainglory
//
// Chaos Avenger
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// └─ Cape: Vainglory

#endregion
#endregion

public class UltraEzrajal
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

    public string OptionsStorage = "UltraEzrajal2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<EzrajalComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: CSS / VDK / LR / LOO\n"
                + "Safe: AI / LR / AP / LOO\n"
                + "F2PFastest: AI / VDK / LR / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            EzrajalComp.Unselected
        ),
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
        Bot.UltraBossHelper.EnableCounterAttack();
        C.AddDrop("Ezrajal Insignia");
        Prep();
        Fight();
        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
        Bot.StopSync();
    }

    void Prep()
    {
        // Sync-equip classes if a comp is selected
        EzrajalComp comp = Bot.Config!.Get<EzrajalComp>("DoEquipClasses");
        if (comp != EzrajalComp.Unselected)
        {
            string[] classes = comp switch
            {
                EzrajalComp.Fast => new[] { C.CheckInventory("Chrono ShadowSlayer") ? "Chrono ShadowSlayer" : "Chrono ShadowHunter", "Verus DoomKnight", "Legion Revenant", "Lord of Order" },
                EzrajalComp.Safe => new[] { "Arcana Invoker", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                EzrajalComp.F2PFastest => new[] { "Arcana Invoker", "Verus DoomKnight", "Legion Revenant", "Lord of Order" },
                _ => throw new InvalidOperationException($"Unhandled EzrajalComp value: {comp}")
            };

            Ultra.EquipClassSync(classes, 4, "ezrajal_class.sync");
        }

        if (Bot.Config!.Get<bool>("DoEnh"))
        {
            Adv.GearStore(false, true);
            DoEnhs();
        }
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
    }

    void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // ---------------------------
        // MAP SETUP
        // ---------------------------
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_ezrajal.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);
         
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        C.EnsureAccept(8152);

        // ---------------------------
        // MAIN COMBAT LOOP
        // ---------------------------
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

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Ezrajal Defeated", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                C.EnsureComplete(8152);
                Bot.UltraBossHelper.DisableCounterAttack();
                break;
            }

            // ---------------------------
            // COUNTER ATTACK HANDLER
            // ---------------------------
            if (
                Bot.Player.HasTarget
                && Bot.Target?.Auras?.Any(a => a != null && a?.Name == "Counter Attack") == true
            )
            {
                Bot.Combat.CancelAutoAttack();

                Bot.Sleep(6300);
            }
            else
            {
                Bot.Combat.Attack(boss);
            }

            Bot.Sleep(500); // slightly lower, smoother attacks
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Verus DoomKnight
            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lord of Order
            case "Lord of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // ArchPaladin
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Dragon of Time
            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Void Highlord
            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
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

    public enum EzrajalComp
    {
        Unselected,
        Fast,
        Safe,
        F2PFastest,
    }
}
