/*
name: UltraEzrajal
description: Ultra Ezrajal helper handling Counter Attack windows with army sync.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Fast = CSS / VDK / LR / LOO, Safe = AI / LR / AP / LOO, F2PFastest = AI / VDK / LR / LOO.
- Default composition is set to Safe when selected.
- No dedicated taunter role is configured in this script.
- Taunt/taunter setup is intentionally not used here; combat is handled via counter-attack windows.
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
// Lord Of Order
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
// Lord Of Order
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
// Lord Of Order
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
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public string OptionsStorage = "UltraEzrajal2";
    public bool DontPreconfigure = true;

    string a,
        b,
        c,
        d,
        overrideA,
        overrideB,
        overrideC,
        overrideD;
    bool UseLifeSteal;
    bool EquipBestGear;
    bool DoEnhancements;
    bool RestoreGear;
    private EzrajalComp ActiveComp = EzrajalComp.Safe;
    private bool wasCounterAttackDetected;
    private int counterAttackCycles;

    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<EzrajalComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: CSS / VDK / LR / LOO\n"
                + "Safe: AI / LR / AP / LOO\n"
                + "F2PFastest: AI / VDK / LR / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            EzrajalComp.Safe
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Equip/restock/use Scroll of Life Steal.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Secondary Class Override", "Blank = use selected comp default for slot 2.", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4.", ""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");
        ActiveComp = Bot.Config == null ? EzrajalComp.Safe : Bot.Config.Get<EzrajalComp>("Main", "DoEquipClasses");
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
        EzrajalComp comp = EzrajalComp.Safe,
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
        Bot.UltraBossHelper.EnableCounterAttack();
        C.AddDrop("Ezrajal Insignia");
        Prep();
        Fight();
    }

    void Prep()
    {
        C.Logger($"[UltraEzrajal] Prep: {ActiveComp}");
        // Sync-equip classes if a comp is selected
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
        {
            DoEnhs();
        }

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
    }

    void EquipBestDmgGear()
    {
        C.EquipBestItemsForMeta(
            new Dictionary<string, string[]>
            {
                { "Weapon", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Armor", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Helm", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Cape", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Pet", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
            }
        );
    }

    void ApplyCompAndEquip(EzrajalComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == EzrajalComp.Unselected)
            return;

        string[] classes = comp switch
        {
            EzrajalComp.Fast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Chrono ShadowSlayer" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Verus DoomKnight" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Legion Revenant" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            EzrajalComp.Safe => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Arcana Invoker" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            EzrajalComp.F2PFastest => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Arcana Invoker" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Verus DoomKnight" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Legion Revenant" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled EzrajalComp value: {comp}")
        };

        Ultra.EquipClassSync(classes, 4, "ezrajal_class.sync");
    }

    void Fight()
    {
        C.Logger("[UltraEzrajal] Fight start.");
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
        Core.ChooseBestCell(boss);
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

            if (UseLifeSteal && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

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
            bool hasCounterAttackAura = Core.HasAura("Counter Attack");
            bool counterAttackStateChanged = hasCounterAttackAura != wasCounterAttackDetected;
            if (counterAttackStateChanged)
            {
                wasCounterAttackDetected = hasCounterAttackAura;
                if (hasCounterAttackAura)
                {
                    counterAttackCycles++;
                    C.Logger(
                        $"[UltraEzrajal] Counter Attack aura detected; entering evade cycle #{counterAttackCycles}."
                    );
                }
                else
                {
                    C.Logger("[UltraEzrajal] Counter Attack aura no longer active; resuming normal attacks.");
                }
            }

            if (hasCounterAttackAura)
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

            // Lord Of Order
            case "Lord Of Order":
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
