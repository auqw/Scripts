/*
name: Army Leveling
description: Levles using aggroing on cell: "r11" of shadowbattleon.
tags: null
*/

//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyLeveling
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
        new Option<bool> ("Solo", "Solo Farm", "Use just 1 account"),
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

        Prereqs();
        Leveling();

        C.SetOptions(false);
    }

    void Leveling()
    {
        if (!Bot.Config!.Get<bool>("Solo"))
        {
            if (sArmy.Players().Length <= 0)
            {
                C.Logger(
                    "Players empty, please add players to the options ( scripts botton > edit scripts option > insert account names exactly as is)"
                );
                return;
            }
        }
        const string map = "shadowbattleon";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.Logger("Players in Current Army: " +
          (Bot.Config.Get<bool>("Solo") ? "Yourself" : $"{sArmy.Players().Length}"));
        C.RegisterQuests(9421, 9422, 9426);
        Core.Join(map);
        C.Jump("r11", "Left");
        if (!Bot.Config.Get<bool>("Solo"))
        {
            if (sArmy.Players().Length > 1)
                Ultra.WaitForArmy(sArmy.Players().Length - 1, "ArmyLeveling.sync");
        }
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {

            if ((Bot.Config.Get<bool>("Solo") && Bot.Player.Level >= 100)
                || Ultra.CheckArmyProgressBool(() => Bot.Player.Level >= 100, syncPath))
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
            Bot.Sleep(500);
        }
    }


    private void Prereqs()
    {
        if (C.isCompletedBefore(9425))
            return;

        Story.PreLoad(this);
        C.Logger("Doing \"shadowbattleon\" prerequisite quests");

        // Mega Shadow Hunt Medal 9422
        Story.KillQuest(9422, "shadowbattleon", "Doomed Beast");

        // Early Autopsy 9423
        Story.KillQuest(9423, "shadowbattleon", "Doomed Beast");

        // Given Life and Purpose 9424
        Story.KillQuest(9424, "shadowbattleon", "Possessed Armor");

        // Adult Hatchling 9425
        Story.KillQuest(9425, "shadowbattleon", "Ouro Spawn");

        C.Logger("Quests are done, Leaving map and rejoining to join the armies RoomNumber.");
        C.Join("whitemap-999999");
    }
}
