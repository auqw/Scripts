/*
name: Army Spirit Orbs
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

public class ArmySpiritOrbs
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

    public string OptionsStorage = "Army_Gold";
    public bool DontPreconfigure = true;
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
        C.BankingBlackList.AddRange(new[] { "Spirit Orb", "Bone Dust" });
        C.SetOptions(disableClassSwap: true);

        GetSO();

        C.SetOptions(false);
    }

    void GetSO(int quant = 10000)
    {
        if (sArmy.Players().Length <= 0)
        {
            C.Logger(
                "Players empty, please add players to the options ( scripts botton > edit scripts option > insert account names exactly as is)"
            );
            return;
        }
        const string map = "necroproject";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        Bot.Quests.UpdateQuest(9901);
        C.AddDrop("Spirit Orb", "Bone Dust");
        C.RegisterQuests(2083);
        Core.Join(map);
        C.Jump("r5", "Left");
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "ArmySpiritOrbs.sync");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Spirit Orb", 65000, false, syncPath))
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
    }
}
