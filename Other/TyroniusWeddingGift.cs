/*
name: Tyronius Weddin gGift
description: Farms "All Drops" From Quest: "Flames of Love".
tags: Flames of Love, drops
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class TyroniusWeddingGift
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
        "Wedding Staff of the ShadowFire",
        "ShadowFire Wedding Guest"
    };

    public void Snowball()
    {
        if (Core.CheckInventory(Loot))
            return;

        Core.AddDrop(Loot);
        Core.EquipClass(ClassType.Solo);
        // Flames of Love
        Core.RegisterQuests(10787);
        while (!Bot.ShouldExit && (!Core.CheckInventory(Loot)))
        {
            Core.HuntMonster("fireplanewar", "Shadefire Onslaught", "Flame of Passion", 8);
            Core.HuntMonster("skytower", "Sunstone", "Sunlit Warmth", 8);
            Bot.Wait.ForQuestComplete(10787);
        }
        Core.CancelRegisteredQuests();
    }
}
