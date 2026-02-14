/*
name: heartsdaygrave
description: This will complete the heartsdaygrave quest.
tags: story, quest, seasonal, heartsdaygrave, heart
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class heartsdaygrave
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

        DoStory();

        Core.SetOptions(false);
    }

    public void DoStory()
    {
        if (Core.isCompletedBefore(10601))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Heartbroken Zombie", // UseableMonsters[0],
	"Hearty Gift", // UseableMonsters[1],
	"Dark Love Knight", // UseableMonsters[2],
	"Vaioh's Warriors", // UseableMonsters[3],
	"Illusionist Zio", // UseableMonsters[4],
	"Warlord Vaioh", // UseableMonsters[5]
};
        #endregion Useable Monsters

        // 10592 | Heartless Monsters
        if (!Story.QuestProgression(10592))
        {
            Core.HuntMonsterQuest(10592,
                ("heartsdaygrave", UseableMonsters[0], ClassType.Farm));
        }


        // 10593 | Heart Throb
        if (!Story.QuestProgression(10593))
        {
            Core.HuntMonsterQuest(10593,
                ("heartsdaygrave", UseableMonsters[1], ClassType.Farm));
        }


        // 10594 | Love in Every Form
        if (!Story.QuestProgression(10594))
        {
            Core.HuntMonsterQuest(10594,
                ("heartsdaygrave", UseableMonsters[2], ClassType.Farm));
        }


        // 10595 | Love's Call
        if (!Story.QuestProgression(10595))
        {
            Core.HuntMonsterQuest(10595,
                ("heartsdaygrave", UseableMonsters[3], ClassType.Farm));
        }


        // 10596 | Eat Your Heart Out
        if (!Story.QuestProgression(10596))
        {
            Core.HuntMonsterQuest(10596,
                ("heartsdaygrave", UseableMonsters[0], ClassType.Farm),
                ("heartsdaygrave", UseableMonsters[1], ClassType.Farm));
        }


        // 10597 | Undead Wing-Liches (and Wing-Cutie)
        if (!Story.QuestProgression(10597))
        {
            Story.MapItemQuest(10597, "heartsdaygrave", 15490);
            Story.MapItemQuest(10597, "heartsdaygrave", 15491);
        }


        // 10598 | Deranged Duty
        if (!Story.QuestProgression(10598))
        {
            Core.HuntMonsterQuest(10598,
                ("heartsdaygrave", UseableMonsters[2], ClassType.Farm),
                ("heartsdaygrave", UseableMonsters[3], ClassType.Farm));
        }


        // 10599 | The Dead and the Restless
        if (!Story.QuestProgression(10599))
        {
            Story.MapItemQuest(10599, "heartsdaygrave", 15492, 7);
        }


        // 10600 | Work Obligations
        if (!Story.QuestProgression(10600))
        {
            Core.HuntMonsterQuest(10600,
                ("heartsdaygrave", UseableMonsters[4], ClassType.Boss));
        }


        // 10601 | Does Your Heart Quiver?
        if (!Story.QuestProgression(10601))
        {
            Core.HuntMonsterQuest(10601,
                ("heartsdaygrave", UseableMonsters[5], ClassType.Boss));
        }


    }
}
