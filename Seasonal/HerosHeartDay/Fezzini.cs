/*
name: Fezzini Story
description: This will complete the Fezzini story quest.
tags: story, quest, seasonal, fezzini, hero, heart
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class FezziniStory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FezziniScript();

        Core.SetOptions(false);
    }

    public void FezziniScript()
    {
        if (!Core.isSeasonalMapActive("fezzini"))
            return;
        if (Core.isCompletedBefore(7389))
            return;

        Story.PreLoad(this);

        //The Dancing Dead
        if (!Story.QuestProgression(7377))
        {
            Core.EnsureAccept(7377);
            Core.KillMonster("fezzini", "Enter", "Spawn", "Zombie Dancer", "Zombie Defeated", 10);
            Core.EnsureComplete(7377);
        }

        //Rats n' Goo
        Story.KillQuest(7378, "fezzini", new[] { "Street Rat", "Zombie Goo" });

        //Get a Clue
        if (!Story.QuestProgression(7379))
        {
            Core.EnsureAccept(7379);
            Core.KillMonster("fezzini", "Enter", "Spawn", "Zombie Dancer", "A Clue?", 10);
            Core.EnsureComplete(7379);
        }

        //Find Lim
        Story.MapItemQuest(7380, "fezzini", 7100);

        //Bottle Time
        if (!Story.QuestProgression(7381))
        {
            Core.EnsureAccept(7381);
            Core.KillMonster("fezzini", "Enter", "Spawn", "Zombie Dancer", "Little Bottle", 6);
            Core.EnsureComplete(7381);
        }

        //Get Some Fur
        Story.KillQuest(7382, "fezzini", new[] { "Zombie Goo", "Street Rat" });

        //Go Tell Beleen
        Story.MapItemQuest(7383, "fezzini", 7101);

        //Ask Around
        Story.MapItemQuest(7384, "fezzini", new[] { 7102, 7103, 7104, 7105, 7106 });

        //Zombie Invasion
        Story.KillQuest(7385, "fezzini", "Zombie Dancer");

        //Castle Zombies
        Story.KillQuest(7386, "fezzini", "Hostile Minion");

        //Warn the King and Queen
        Story.MapItemQuest(7387, "fezzini", 7107);

        //Monstrous Guards!
        Story.KillQuest(7388, "fezzini", "Monstrous Guard");

        //It's Salvaza!
        Story.KillQuest(7389, "fezzini", "Salvaza");
    }
}
