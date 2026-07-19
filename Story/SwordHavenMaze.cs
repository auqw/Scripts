/*
name: Sword Haven Maze
description: This will finish the ShadowVoid Story.
tags: story, quest, Sword Haven Maze, SwordHavenMaze
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;

public class SwordHavenMaze
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

        SwordHavenMazeQuests();

        Core.SetOptions(false);
    }

    public void SwordHavenMazeQuests()
    {
        if (Core.isCompletedBefore(10809))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Noble Ghost", // UseableMonsters[0],
            "Feral Boar", // UseableMonsters[1],
            "Undead Prisoner", // UseableMonsters[2],
            "Extinction Artist", // UseableMonsters[3],
            "Aficionado Cosima ", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10800 | Luxury Rubbish
        if (!Story.QuestProgression(10800))
        {
            Story.MapItemQuest(10800, "swordhavenmaze", 15945);
        }


        // 10801 | Castaigne Cup
        if (!Story.QuestProgression(10801))
        {
            Story.MapItemQuest(10801, "swordhavenmaze", 15946);
        }


        // 10802 | Dethrix Dollars
        if (!Story.QuestProgression(10802))
        {
            Story.MapItemQuest(10802, "swordhavenmaze", 15947);
        }


        // 10803 | Nothing but Numbers
        if (!Story.QuestProgression(10803))
        {
            Story.MapItemQuest(10803, "swordhavenmaze", 15948);
        }


        // 10804 | Call for Retrial
        if (!Story.QuestProgression(10804))
        {
            Story.MapItemQuest(10804, "swordhavenmaze", 15949);
        }

        // 10806 | With Love
        if (!Story.QuestProgression(10806))
        {
            Story.MapItemQuest(10806, "swordhavenmaze", 15950);
        }


        // 10807 | Reeking Rinds
        if (!Story.QuestProgression(10807))
        {
            Story.MapItemQuest(10807, "swordhavenmaze", 15951);
        }


        // 10808 | Pig-ment
        if (!Story.QuestProgression(10808))
        {
            Core.HuntMonsterQuest(10808,
                ("swordhavenmaze", UseableMonsters[3], ClassType.Solo));
        }


        // 10809 | Passing Passepied
        if (!Story.QuestProgression(10809))
        {
            Core.HuntMonsterQuest(10809,
                ("swordhavenmaze", UseableMonsters[4], ClassType.Solo));
        }
    }


}
