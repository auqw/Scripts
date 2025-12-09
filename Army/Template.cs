/*
name: null
description: null
tags: null
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

public class ArmyTemplate
{
    #region  IgnoreME
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
    #endregion

    public string OptionsStorage = "Fill me in";
    public bool DontPreconfigure = true;

    // Add / remove players below, to get to how ever many is the map's cap... or leave it alone, doesn't matter.
    public List<IOption> Options = new()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.player7,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        C.BankingBlackList.AddRange(new[] { "add", "any", "drops", "needed" });
        C.SetOptions(disableClassSwap: true);

        Examples();

        C.SetOptions(false);
    }



    void Examples()
    {
        ArmyHandler(
            map: "mapname",
            QuestIDs: new int[] { },
            WaitForArmysyncPath: "phase1_items",
            AggroCell: "Enter",
            checkType: CheckType.Item,
            Itemname: "ItemName",
            quant: 9999,
            UseBool: false
        );

        ArmyHandler(
            map: "Shadowbattleon",
            QuestIDs: new int[] {9421, 9422, 9426 },
            WaitForArmysyncPath: "ArmySBO",
            AggroCell: "r11",
            checkType: CheckType.Bool,
            condition: () => Bot.Player.Level >= 100,
            UseBool: true
        );
    }


    // You can either make a secondary void or add all of the below to the first one under the first item.
    private void ArmyHandler(string map, int[] QuestIDs, string WaitForArmysyncPath, string AggroCell, CheckType checkType, string? Itemname = null, int quant = 0, bool UseBool = false, Func<bool>? condition = null)
    {
        // Sync file used to keep track of what accs are done.
        string syncPath = Ultra.ResolveSyncPath(WaitForArmysyncPath);
        Ultra.ClearSyncFile(WaitForArmysyncPath);

        // Log Players in current army.
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");


        // Uncomment below, and add any questids that are used for this portion
        // C.RegisterQuests(1,2,3);

        Core.Join(map);

        C.Jump(AggroCell, "Left");

        // Don't Touch vv
        if (sArmy.Players().Length > 1)
            // Dont make this the same as the syncPath
            Ultra.WaitForArmy(sArmy.Players().Length - 1, WaitForArmysyncPath);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        // Pick a variant below ( multiple can be used as long as the sync files are different.)

        if (UseBool && condition != null)
        {
            // Bool variant
            while (!Bot.ShouldExit)
            {
                // Replace the `Bot.Player.Level >= 100` below with the bool
                // you want want all accs to have true, leave the rest of this alone.
                if (Ultra.CheckArmyProgressBool(condition, syncPath))
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

                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
            return;
        }

        if (Itemname != null)
        {
            //Int variant
            while (!Bot.ShouldExit)
            {
                // Replace `Itemname` with the wanted item
                // Replace the 500 with the quantity you desire
                // Replace `false` if the item is a temp item with `true` or leave as `false` for non-temp items.
                if (Ultra.CheckArmyProgress(Itemname, quant, false, syncPath))
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

                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
            return;
        }
    }


    enum CheckType
    {
        Bool = 1,
        Item = 2
    }
}
