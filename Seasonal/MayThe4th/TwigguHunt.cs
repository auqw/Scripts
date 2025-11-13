/*
name: GL-24K's Quests
description: This script will complete GL-24K's Quests in /twigguhunt.
tags: gl24k,gl-24k,twigguhunt,twiggu hunt,may the 4th,seasonal, story, quests, may4th
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonStory.cs
using Skua.Core.Interfaces;

public class TwigguHunt
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static MurderMoon MM
    {
        get => _MM ??= new MurderMoon();
        set => _MM = value;
    }
    private static MurderMoon _MM;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9702) || !Core.isSeasonalMapActive("twigguhunt"))
            return;

        MM.MurderMoonStory();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Scout It Out (9698)
        Story.MapItemQuest(9698, "twigguhunt", 13131, 5);

        // No Bot Unbeaten (9699)
        Story.KillQuest(9699, "twigguhunt", "Scout Droid");

        // If Memory Serves... (9700)
        Story.KillQuest(9700, "twigguhunt", "Scout Droid");

        // Target Located (9701)
        Story.KillQuest(9701, "twigguhunt", "Bodyguard Droid");

        // Bring Them In (9702)
        if (!Story.QuestProgression(9702))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9702, "twigguhunt", "Twiggu");
            Core.EquipClass(ClassType.Farm);
        }
    }
}
