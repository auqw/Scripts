/*
name: Army Prismatas
description: Farms gold using the Prismatas in /archmage and selling the elemental bindings
tags: Prismatas, elemental binding, gold, farm
*/

//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyPristmasv3
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    private static CoreBots _sCore;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEnginev3 Core = new();
    public CoreUltrav3 Ultra = new();
    private static CoreBots sCore
    {
        get => _sCore ??= new CoreBots();
        set => _sCore = value;
    }

    private static UltraEnhancements Enh
    {
        get => _Enh ??= new UltraEnhancements();
        set => _Enh = value;
    }
    private static UltraEnhancements _Enh;

    public string OptionsStorage = "ArmyPristmas-v2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<int>(
            "ArmySize",
            "Army Size",
            "How many players are in your army (including yourself).",
            4
        ),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Verus DoomKnight"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "King's Echo"),
        new Option<string>("Class5", "Class 5", "Preset class 5 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class6", "Class 6", "Preset class 6 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class7", "Class 7", "Preset class 7 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),

        new Option<bool>(
            "DoEnh",
            "Do Enhancements",
            "Auto-enhance gear properly for the fight before killing Prismatas.",
            true
        ),

        new Option<bool>(
            "SellEvery100",
            "Sell Every 100",
            "Enable to sell Elemental Binding every 100. Disable to keep them.",
            true
        ),
        new Option<bool>(
            "StopAtMaxGold",
            "Stop at Max Gold",
            "Enable to stop when reaching 100M gold. Disable to continue farming.",
            true
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        C.BankingBlackList.AddRange(new[] { "Elemental Binding" });
        C.SetOptions(true);
        C.Logger("Elemental Bindings will be sold every 100\nClass presets will be applied if configured.");
        Core.Boot();
        try
        {
            Prep();
            KillPrismatas();
        }
        finally
        {
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    private void Prep()
    {
        C.Logger("Preparing Army Prismatas...");
        EquipPresetClasses();

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();
    }

    void KillPrismatas()
    {
        const string map = "archmage";
        const string boss = "Prismata";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // Wait for the army in a safe map before entering the boss area.
        Core.Join("whitemap");
        Bot.Wait.ForMapLoad("whitemap");

        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        if (armySize > 1)
            Ultra.WaitForArmy(armySize - 1, "ArmyPrismatas.sync", 3000, 500, 10000);
        var (bestCell, _) = Core.ChooseBestCell(boss);

        Core.Join(map);
        C.AddDrop("Elemental Binding");
        C.Jump("r2", "Left");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);

        bool sellEvery100 = Bot.Config!.Get<bool>("SellEvery100");
        bool stopAtMaxGold = Bot.Config!.Get<bool>("StopAtMaxGold");
        while (!Bot.ShouldExit)
        {
            if (stopAtMaxGold && Ultra.CheckArmyProgressBool(() => Bot.Player.Gold >= 100000000, syncPath))
            {
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != bestCell)
            {
                Bot.Sleep(200);
                Bot.Map.Jump(bestCell);
                continue;
            }

            if (Bot.Player.Cell != "r2")
            {
                Bot.Map.Jump("r2", "Left");
                Bot.Wait.ForCellChange("r2");
            }

            Bot.Combat.Attack(GetPrismataTargetMapID());

            if (!C.GoldMaxed && sellEvery100 && C.CheckInventory("Elemental Binding", 100))
            {
                C.SellItem("Elemental Binding", all: true);
            }

            Bot.Sleep(500);
        }
    }

    private void EquipPresetClasses()
    {
        var entries = new[]
        {
            Bot.Config!.Get<string>("Class1"),
            Bot.Config.Get<string>("Class2"),
            Bot.Config.Get<string>("Class3"),
            Bot.Config.Get<string>("Class4"),
            Bot.Config.Get<string>("Class5"),
            Bot.Config.Get<string>("Class6"),
            Bot.Config.Get<string>("Class7"),
        }
        .Select(ParseClassEntry)
        .Where(e => !string.IsNullOrEmpty(e.ClassName))
        .ToList();

        if (entries.Count == 0)
            return;

        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        string[] classNames = entries.Select(e => e.ClassName).ToArray();

        bool allowDuplicates = classNames.Length < armySize
            || classNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() < classNames.Length;

        string[][] classSlots;
        if (entries.Count >= armySize)
        {
            classSlots = entries
                .Take(armySize)
                .Select(e => new[] { e.ClassName })
                .ToArray();
        }
        else
        {
            classSlots = Enumerable.Range(0, armySize)
                .Select(_ => classNames)
                .ToArray();
        }

        var preferredAssignments = entries
            .Where(e => !string.IsNullOrEmpty(e.Username))
            .GroupBy(e => e.Username, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ClassName, StringComparer.OrdinalIgnoreCase);

        Ultra.EquipClassSync(
            classSlots,
            armySize,
            "army_prismatas_class-v2.sync",
            allowDuplicates,
            preferredAssignments.Count > 0 ? preferredAssignments : null
        );
    }

    private (string ClassName, string Username) ParseClassEntry(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return (string.Empty, string.Empty);

        var parts = raw.Split(',');
        string className = parts[0].Trim();
        string username = parts.Length > 1 ? parts[1].Trim() : string.Empty;
        return (className, username);
    }

    private int GetPrismataTargetMapID()
    {
        var currentTarget = Bot.Player.Target;
        if (currentTarget != null && currentTarget.Alive && currentTarget.HP > 0)
            return currentTarget.MapID;

        var nextTarget = Bot.Monsters.CurrentAvailableMonsters
            .Where(m => m != null && m.Alive && m.HP > 0)
            .OrderBy(m => m.MapID)
            .FirstOrDefault();

        return nextTarget?.MapID ?? 1;
    }

    private void DoEnhs()
    {
        Enh.Apply();
    }
}
