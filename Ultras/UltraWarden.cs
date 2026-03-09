/*
name: UltraWarden
description: Ultra Warden helper with HP-band taunt trigger and army synchronization.
tags: Ultra

Fight notes:
- Composition is Recommended = [slot 1-4]: LR / AP / LOO / VDK.
- Default composition is set to Recommended/Safe behavior.
- Taunter positions are fixed by comp slots:
  - Slot 2 and slot 4 are treated as taunter slots.
- Default taunters are AP (slot2) and VDK (slot4) unless overridden via class overrides.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWarden
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

    string overrideA,
        overrideB,
        overrideC,
        overrideD;

    string tauntSlot2 = "ArchPaladin";
    string tauntSlot4 = "Verus DoomKnight";
    bool UseLifeSteal;
    bool EquipBestGear;
    bool RestoreGear;
    WardenComp ActiveComp = WardenComp.Recommended;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraWarden";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<WardenComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Recommended: LR / AP / LOO / VDK\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            WardenComp.Recommended
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Non-taunters equip/restock/use Scroll of Life Steal.", true),
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
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");
        ActiveComp = Bot.Config == null ? WardenComp.Recommended : Bot.Config.Get<WardenComp>("Main", "DoEquipClasses");

        Adv.GearStore();

        try
        {
            Run(ActiveComp, EquipBestGear, UseLifeSteal, overrideA, overrideB, overrideC, overrideD);
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
        WardenComp comp = WardenComp.Recommended,
        bool equipBestGear = true,
        bool useLifeSteal = true,
        string? classAOverride = null,
        string? classBOverride = null,
        string? classCOverride = null,
        string? classDOverride = null
    )
    {
        ActiveComp = comp;
        EquipBestGear = equipBestGear;
        UseLifeSteal = useLifeSteal;
        overrideA = classAOverride?.Trim() ?? string.Empty;
        overrideB = classBOverride?.Trim() ?? string.Empty;
        overrideC = classCOverride?.Trim() ?? string.Empty;
        overrideD = classDOverride?.Trim() ?? string.Empty;

        Core.Boot();
        Prep();
        Fight();
    }

    bool IsTaunter() => Core.HasClassEquipped(tauntSlot2) || Core.HasClassEquipped(tauntSlot4);

    void Prep()
    {
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
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

    void ApplyCompAndEquip(WardenComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == WardenComp.Unselected)
            return;

        string[] classes = comp switch
        {
            WardenComp.Recommended => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Legion Revenant" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Lord Of Order" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Verus DoomKnight" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled WardenComp value: {comp}")
        };

        tauntSlot2 = classes[1];
        tauntSlot4 = classes[3];

        Ultra.EquipClassSync(classes, 4, "warden_class.sync");
    }

    void Fight()
    {
        const string map = "ultrawarden";
        const string boss = "Ultra Warden";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8153);
        C.AddDrop("Warden Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_warden.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

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

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Warden Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8153);
                break;
            }

            if (Core.HasClassEquipped(tauntSlot2) || Core.HasClassEquipped(tauntSlot4))
                Ultra.UltraWardenTaunter();

            Bot.Combat.Attack("*");
            Bot.Sleep(250);
        }
    }

    public enum WardenComp
    {
        Unselected,
        Recommended,
    }
}
