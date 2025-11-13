/*
name: Army Pet Tamer Rep
description: Farms reputation for the Pet Tamer faction
tags: army pet tamer reputation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/PockeymogsStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs

using Skua.Core.Interfaces;

public class Generated_ArmyPetTamerRep
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;
    private static CoreArmyRep CAR
    {
        get => _CAR ??= new CoreArmyRep();
        set => _CAR = value;
    }
    private static CoreArmyRep _CAR;
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
    private static PockeyMogsStory lite
    {
        get => _lite ??= new PockeyMogsStory();
        set => _lite = value;
    }
    private static PockeyMogsStory _lite;

    public string OptionsStorage = "CustomAggroMon";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        lite.PockeyMogs();
        static void Setup() => CAR.ArmyRavenlossRep();
        Setup();
        Core.SetOptions(false);
    }
}
