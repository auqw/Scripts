/*
name: Northlands Monk (Class)
description: This will kill the FrozenSoul Queen that drops the Northlands Monk (Class).
tags: northlands-monk, seasonal, class, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class NorthlandsMonk
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetNlMonk();

        Core.SetOptions(false);
    }

    public void GetNlMonk(bool rankUpClass = true)
    {
        if (!Core.isSeasonalMapActive("frozensoul"))
            return;

        if (Core.CheckInventory(52413))
        {
            Adv.RankUpClass("Northlands Monk", true);
            return;
        }

        Core.AddDrop(52413);

        Core.EquipClass(ClassType.Solo);
        while (!Bot.ShouldExit && !Core.CheckInventory("Northlands Monk"))
            Core.KillMonster(
                "frozensoul",
                "r4",
                "Left",
                "Frozensoul Queen",
                "Northlands Monk",
                isTemp: false
            );

        if (rankUpClass)
            Adv.RankUpClass("Northlands Monk", true);
    }
}
