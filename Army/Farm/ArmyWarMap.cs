/*
name: Armywarmap
description: Levels the players acc using an arnmy on /CarcossaCourt with 3 selectable areas
tags: war, army, leveling, CarcossaCourt
*/

//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmyWarMap
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

    public string OptionsStorage = "ArmyWar";
    public bool DontPreconfigure = true;
    public List<IOption> Options =
    [
        new Option<bool>("Solo", "Solo Farm", "Use just 1 account"),
        new Option<bool>("Endless", "Endless Farm", "Disregard the Level check, and run till *YOU* stop it."),
        new Option<ClassType>("ClassType", "ClassType to use", "What Classtype to use (high hp classes work well, as you'll get hit around 1k~ without buffs)"),
        new Option<LevelType>(
            "LevelingArea",
            "Leveling Area",
            "Area to do the leveling in:\n" +
            "LowtoMidLevel = 'Enter' cell with 3 mobs (~10k HP each)\n" +
            "HighLevel = 'r5' cell with 3 mobs (~25k HP each)\n" +
            "Boss = Dryden (smaller boss for now)"
        ),

        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.player7,

        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    ];

    public void ScriptMain(IScriptInterface Bot)
    {
        C.SetOptions(disableClassSwap: true);

        Leveling();

        C.SetOptions(false);
    }

    public void Leveling()
    {
        bool Solo = Bot.Config!.Get<bool>("Solo");
        bool Endless = Bot.Config!.Get<bool>("Endless");
        LevelType levelType = Bot.Config.Get<LevelType>("LevelingArea");
        ClassType classType = Bot.Config!.Get<ClassType>("ClassType");
        if (!Solo)
        {
            if (sArmy.Players().Length <= 0)
            {
                C.Logger("Players empty, please add players to the options (scripts button > edit scripts option > insert account names exactly as is)");
                return;
            }
        }
        Quest? quest = C.InitializeWithRetries(() => Bot.Quests.EnsureLoad(10778));
        if (quest == null)
        {
            C.Logger("Failed to load quest 10778. Please check your connection or the server status.");
            return;
        }
        bool QuestNerfed = quest.Gold < 2500 || quest.XP < 6000;
        if (QuestNerfed)
            if (Bot.ShowMessageBox("\"CarcossaCourt's war quests have been nerfed, continue?", "War Nerfed", true) == false)
                return;

        string map = "carcossacourt";

        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        var (cell, pad) = GetCellPad(levelType);

        Bot.Sleep(2500);

        C.Logger("Players in Current Army: " + (Solo ? "Yourself" : $"{sArmy.Players().Length}"));

        Dictionary<LevelType, int[]> questMap = new()
        {
            { LevelType.LowtoMidLevel, new[] { 10778, 10779 } },
            { LevelType.HighLevel,     new[] { 10778, 10779, 10780, 10781 } },
            { LevelType.SmallBoss,     new[] { 10778, 10779, 10784 } },
            { LevelType.BigBoss,     new[] { 10778, 10779, 10785 } }
        };
        C.EquipClass(classType);
        C.AddDrop("A Revelation");
        C.RegisterQuests(questMap[levelType]);
        Core.Join(map);
        C.Jump(cell, pad);

        if (!Solo)
        {
            if (sArmy.Players().Length > 1)
                Ultra.WaitForArmy(sArmy.Players().Length - 1, "ArmyWarMap.sync");
        }

        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);

        Bot.Options.AggroMonsters = true;

        C.SavedState(true, map);
        while (!Bot.ShouldExit)
        {

            if (C.CheckSaveState())
                C.ExecuteSaveState();


            if (!Endless && ((Solo && Bot.Player != null && Bot.Player.Level >= 100)
             || Ultra.CheckArmyProgressBool(() => Bot.Player != null && Bot.Player.Level >= 100, syncPath)))
            {
                Bot.Options.AggroMonsters = false;
                C.JumpWait();
                C.Logger("All players finished farm.");
                break;
            }

            if (Bot.Player != null && !Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player?.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, false);
                Bot.Wait.ForCellChange(cell);
            }

            Bot.Combat.Attack("*");
            Bot.Sleep(500);
        }
    }

    public static (string cell, string pad) GetCellPad(LevelType type) =>
        type switch
        {
            LevelType.LowtoMidLevel => ("Enter", "Spawn"),
            LevelType.HighLevel => ("r5", "Left"),
            LevelType.SmallBoss => ("r8", "Left"),
            LevelType.BigBoss => ("r9", "Center"),
            _ => ("Enter", "Spawn")
        };

    public enum LevelType
    {
        LowtoMidLevel,
        HighLevel,
        SmallBoss,
        BigBoss
    }
}
