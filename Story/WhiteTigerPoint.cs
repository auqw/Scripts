/*
name: WhiteTigerPoint
description: Does the quests in /whitetigerpoint.
tags: whitetigerpoint, storyline, story, white tiger point, white tiger point storyline
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class whitetigerpoint
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoStory();

        Core.SetOptions(false);
    }

    public void DoStory()
    {
        if (Core.isCompletedBefore(10329))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Astral Spirit", // UseableMonsters[0],
            "Byakko Cub", // UseableMonsters[1],
            "Rigel Stray", // UseableMonsters[2],
            "Lunar Haze", // UseableMonsters[3],
            "Byakko", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10320 | Stellar Dynamics
        if (!Story.QuestProgression(10320))
        {
            Core.HuntMonsterQuest(10320, ("whitetigerpoint", UseableMonsters[0], ClassType.Farm));
        }

        // 10321 | Hokey Horoscope
        if (!Story.QuestProgression(10321))
        {
            Story.MapItemQuest(10321, "whitetigerpoint", 14656);
            Story.MapItemQuest(10321, "whitetigerpoint", 14657, 2);
        }

        // 10322 | Tokaki
        if (!Story.QuestProgression(10322))
        {
            Core.HuntMonsterQuest(10322, ("whitetigerpoint", UseableMonsters[1], ClassType.Farm));
        }

        // 10323 | Ekie
        if (!Story.QuestProgression(10323))
        {
            Story.MapItemQuest(10323, "whitetigerpoint", 14658);
            Story.MapItemQuest(10323, "whitetigerpoint", 14659, 2);
        }

        // 10324 | Subaru
        if (!Story.QuestProgression(10324))
        {
            Core.HuntMonsterQuest(
                10324,
                ("whitetigerpoint", UseableMonsters[1], ClassType.Farm),
                ("whitetigerpoint", UseableMonsters[0], ClassType.Farm)
            );
        }

        // 10325 | Kagasuki
        if (!Story.QuestProgression(10325))
        {
            Core.HuntMonsterQuest(10325, ("whitetigerpoint", UseableMonsters[2], ClassType.Farm));
        }

        // 10326 | Amefuri
        if (!Story.QuestProgression(10326))
        {
            Core.HuntMonsterQuest(10326, ("whitetigerpoint", UseableMonsters[3], ClassType.Farm));
        }

        // 10327 | Tatara
        if (!Story.QuestProgression(10327))
        {
            Story.MapItemQuest(10327, "whitetigerpoint", 14660);
        }

        // 10328 | Toroki
        if (!Story.QuestProgression(10328))
        {
            Core.HuntMonsterQuest(
                10328,
                ("whitetigerpoint", UseableMonsters[3], ClassType.Farm),
                ("whitetigerpoint", UseableMonsters[2], ClassType.Farm)
            );
        }

        // 10329 | Komokuten
        if (!Story.QuestProgression(10329))
        {
            Core.HuntMonsterQuest(10329, ("whitetigerpoint", UseableMonsters[4], ClassType.Solo));
        }
    }
}
