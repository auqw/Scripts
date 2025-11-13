/*
name: Obsidian Rock
description: Aggromon for obsidian rock, up to 8 ppl.
tags: obsidian rock, legion, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class Generated_ArmyObsidianRock
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

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
        sArmy.player7,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Core.Logger(
            "~\"All\"~ Army Scripts have been disabled by the author.",
            "**READ ME!!**",
            stopBot: true
        );
        // ArmyObsidianRock();
        Core.SetOptions(false);
    }

    public void ArmyObsidianRock() =>
        Army.RunGeneratedAggroMon(map, monNames, questIDs, classtype, drops);

    private static readonly List<int> questIDs = new() { 2742 };
    private static readonly List<string> monNames = new()
    {
        "Living Fire",
        "Sulfur Imp",
        "Firestorm Hatchling",
    };
    private static readonly List<string> drops = new() { "Obsidian Rock" };
    private string map = "firestorm";
    private ClassType classtype = ClassType.Farm;
}
