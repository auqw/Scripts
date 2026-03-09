/*
name: UltraEngineer
description: Ultra Engineer helper prioritizing drones with army synchronization and consumables.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Fast = Lich / LR / AP / LOO, Safe = LR / SC / AP / LOO, F2PFast = AI / LR / AP / LOO.
- Default composition is set to Safe when selected.
- Recommended fixed comp taunter classes are slot 1 = Lich and slot 2 = Legion Revenant.
- No explicit script-level taunt-role logic is used; scripts rely on combat/party behavior.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;
#region Comps
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
// Verus DoomKnight
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
    EngineerComp ActiveComp = EngineerComp.Safe;

    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<EngineerComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: Lich / LR / AP / LOO\n"
                + "Safe: LR / SC / AP / LOO\n"
                + "F2PFast: AI / LR / AP / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            EngineerComp.Safe
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
        ActiveComp = Bot.Config == null ? EngineerComp.Safe : Bot.Config.Get<EngineerComp>("Main", "DoEquipClasses");

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
        EngineerComp comp = EngineerComp.Safe,
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

    void Prep()
    {
        C.Logger($"UltraEngineer prep: {ActiveComp}");
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnhs();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
        C.Logger("Potions/consumables prepared.");
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

    void ApplyCompAndEquip(EngineerComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == EngineerComp.Unselected)
            return;

        string[] classes = comp switch
        {
            EngineerComp.Fast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Lich" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            EngineerComp.Safe => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Legion Revenant" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "StoneCrusher" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            EngineerComp.F2PFast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Arcana Invoker" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled EngineerComp value: {comp}")
        };

        Ultra.EquipClassSync(classes, 4, "engineer_class.sync");
        C.Logger(
            $"Engineer classes selected => [1]={classes[0]}, [2]={classes[1]}, [3]={classes[2]}, [4]={classes[3]}"
        );
    }

    void Fight()
    {
        const string map = "ultraengineer";
        const string boss = "Ultra Engineer";
        const string priority1 = "Defense Drone";
        const string priority2 = "Attack Drone";
        C.Logger("UltraEngineer fight start.");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8154);
        C.AddDrop("Engineer Insignia");
        Core.Join(map);
        if (Bot.Map.Name != map)
            Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_engineer.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Bot.Map.Name != map)
                Core.Join(map);

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Engineer Defeated", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                C.EnsureComplete(8154);
                break;
            }
            if (UseLifeSteal && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
            Ultra.KillWithPriority(boss, 3, priority1, 2, priority2, 1);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

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

            // ArchPaladin
            case "ArchPaladin":
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

            // Verus DoomKnight
            case "Verus DoomKnight":
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

            default:
                break;
        }
    }

    public enum EngineerComp
    {
        Unselected,
        Fast,
        Safe,
        F2PFast,
    }
}
