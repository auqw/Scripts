/*
name: Tunnel Of Love Story
description: This will complete the Tunnel of Love story in /tunneloflove.
tags: story, quest, seasonal, hero, heart,valentines,tunneloflove,tunnel of love,big daddy,hero's heart day
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class TunnelOfLove
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

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10071) || !Core.isSeasonalMapActive("tunneloflove"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Rosey Moth", // UseableMonsters[0],
            "Love Knight", // UseableMonsters[1],
            "Galanoth", // UseableMonsters[2],
            "Mourner", // UseableMonsters[3],
            "Oubliette", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10062 | Live Decor
        if (!Story.QuestProgression(10062))
        {
            Core.HuntMonsterQuest(10062, ("tunneloflove", UseableMonsters[0], ClassType.Farm));
        }

        // 10063 | A Past Storm
        Story.MapItemQuest(10063, "tunneloflove", new[] { 14138, 14139 });

        // 10064 | Employee Sniping
        if (!Story.QuestProgression(10064))
        {
            Core.HuntMonsterQuest(10064, ("tunneloflove", UseableMonsters[1], ClassType.Farm));
        }

        // 10065 | Auntie Celeste
        Story.MapItemQuest(10065, "tunneloflove", 14140);

        // 10066 | A Cold Reception
        if (!Story.QuestProgression(10066))
        {
            Core.HuntMonsterQuest(10066, ("tunneloflove", UseableMonsters[2], ClassType.Solo));
        }

        // 10067 | Lethal Lethe
        Story.MapItemQuest(10067, "tunneloflove", 14141);

        // 10068 | Missing Orpheus
        if (!Story.QuestProgression(10068))
        {
            Core.HuntMonsterQuest(10068, ("tunneloflove", UseableMonsters[3], ClassType.Farm));
        }

        // 10069 | Mentor and Student
        Story.MapItemQuest(10069, "tunneloflove", new[] { 14142, 14143 });

        // 10070 | Rosy Hues
        if (!Story.QuestProgression(10070))
        {
            Core.HuntMonsterQuest(
                10070,
                ("tunneloflove", UseableMonsters[0], ClassType.Farm),
                ("tunneloflove", UseableMonsters[1], ClassType.Farm)
            );
        }

        // 10071 | Oubliable
        if (!Story.QuestProgression(10071))
        {
            Core.HuntMonsterQuest(10071, ("tunneloflove", UseableMonsters[4], ClassType.Solo));
        }
    }
}
