/*
name: LichwarGoldXP
description: Levels/farms gold using aggroing on cell: "r11" of /LichWar. - all accounts *Must* be members
tags: army, gold, exp, member, upgrade
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

public class ArmyLichWarMem
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
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public string OptionsStorage = "ArmyLeveling2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<FarmType>("Gold or EXP", "Gold or EXP", "Are we Maxing Gold, EXP, or Both", FarmType.Both),
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
        C.SetOptions(disableClassSwap: true);

        Leveling();

        C.SetOptions(false);
    }

    void Leveling()
    {
        if (sArmy.Players().Length <= 0)
        {
            C.Logger(
                "Players empty, please add players to the options ( scripts botton > edit scripts option > insert account names exactly as is)"
            );
            return;
        }

        FarmType WTFweDOING = Bot.Config!.Get<FarmType>("Gold or EXP");

        // concise assignment using a switch expression
        bool WeDoingThis = WTFweDOING switch
        {
            FarmType.Both => Bot.Player.Gold >= 100_000_000 && Bot.Player.Level >= 100,
            FarmType.Gold => Bot.Player.Gold >= 100_000_000,
            FarmType.Exp => Bot.Player.Level >= 100,
            _ => false // safe fallback
        };


        const string map = "lichwar";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        C.RegisterQuests(10282, 10283);
        Core.Join(map);
        C.Jump("r6", "Right");
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "ArmyLeveling.sync");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => WeDoingThis, syncPath))
            {
                Bot.Options.AggroMonsters = false;
                C.JumpWait();
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
            Bot.Sleep(200);
        }
    }


    enum FarmType
    {
        Gold = 1,
        Exp = 2,
        Both = 3
    }
}
