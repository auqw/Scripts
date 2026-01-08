/*
name: Army Pristmas
description: Farms gold using the Pristmas in /archmage and selling the elemental bindings
tags: Pristmas, elemental binding, gold, farm
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyPristmas
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
    private static CoreBots sCore
    {
        get => _sCore ??= new CoreBots();
        set => _sCore = value;
    }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }

    private static CoreArmyLite _sArmy;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

    public string OptionsStorage = "ArmyPristmas";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
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
        C.SetOptions(disableClassSwap: true);
        C.Logger("Elemental Bindings will be sold every 100\n The script will use what ever class you have on.");
        Core.Boot();
        KillPrismatas();

        C.SetOptions(false);
    }

    void KillPrismatas()
    {
        const string map = "archmage";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
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

            if (Bot.Player.Cell != "r2")
            {
                Bot.Map.Jump("r2", "Left");
                Bot.Wait.ForCellChange("r2");
            }

            Bot.Combat.Attack(
                Bot.Monsters.CurrentAvailableMonsters
                    .FirstOrDefault(m => m.MapID == 1)?.HP > 0 ? 1 : 2
            );

            if (sellEvery100 && C.CheckInventory("Elemental Binding", 100))
            {
                C.SellItem("Elemental Binding", all: true);
            }

            Bot.Sleep(500);
        }
    }
}
