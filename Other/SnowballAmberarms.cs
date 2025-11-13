/*
name: Snowball Amberarms Drops
description: Farms "All Drops" From Quest: "Snowball Thinks You're the Best".
tags: snowball thinks you're the best, drops
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class SnowballAmberarms
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Snowball();

        Core.SetOptions(false);
    }

    public string[] Loot =
    {
        "TundraWyrm Hunter",
        "WyrmHunter's Cowl",
        "WyrmHunter's Claymore",
        "WyrmHunter Back Claymore",
    };

    public void Snowball()
    {
        if (Core.CheckInventory(Loot) || (!Core.CheckInventory("Snowball Amberarms")))
            return;

        Core.AddDrop(Loot);
        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(3508);
        while (!Bot.ShouldExit && (!Core.CheckInventory(Loot)))
        {
            Core.HuntMonster("bosschallenge", "Mutated Void Dragon", "Dread Talon", isTemp: false);
            Core.HuntMonster("towerofdoom6", "Dread Terror", "Dread Tooth", 20, false);
        }
    }
}
