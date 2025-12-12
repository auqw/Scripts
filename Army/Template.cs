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
using Skua.Core.Models.Quests;
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
        Ultra.ArmyHandler(
            map: "map",
            QuestIDs: new int[] { 1, 2, 3 },
            WaitForArmysyncPath: "SyncFileName",
            AggroCell: "Cell",
            checkType: CoreUltra.CheckType.Item,
            Itemname: "QuestItem",
            quant: 1,
            isTemp: true,
            UseBool: false,
            PlayerCount: sArmy.Players().Length,
            QuestReward: "QuestReward"
        );
    }

    enum CheckType
    {
        Bool = 1,
        Item = 2
    }
}
