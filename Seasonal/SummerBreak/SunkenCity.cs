/*
name: SunkenCity
description: Does the quests from /SunkenCity
tags: sunkencity, sunken city, story, weekly
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class SunkenCity
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

        SagaName();

        Core.SetOptions(false);
    }

    public void SagaName()
    {
        if (Core.isCompletedBefore(10276) || !Core.isSeasonalMapActive("sunkencity"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Necro Adipocere", // UseableMonsters[0],
            "Merdrathoolian", // UseableMonsters[1],
            "Nereid Princess", // UseableMonsters[2]
        };
        #endregion Useable Monsters

        // 10274 | Amensalism
        if (!Story.QuestProgression(10274))
        {
            Core.HuntMonsterQuest(10274, ("sunkencity", UseableMonsters[0], ClassType.Farm));
        }

        // 10275 | Mutualism
        if (!Story.QuestProgression(10275))
        {
            Core.HuntMonsterQuest(10275, ("sunkencity", UseableMonsters[1], ClassType.Farm));
        }

        // 10276 | Symbiogenesis
        if (!Story.QuestProgression(10276))
        {
            Core.HuntMonsterQuest(10276, ("sunkencity", UseableMonsters[2], ClassType.Solo));
        }
    }
}
