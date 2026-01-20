/*
name: tercesarchive
description: This will finish the tercesarchive quest.
tags: story, quest, tercesarchive, nation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
using Skua.Core.Interfaces;

public class TercesArchive
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static OblivionTundra OblivionTundra
    {
        get => _OblivionTundra ??= new OblivionTundra();
        set => _OblivionTundra = value;
    }
    private static OblivionTundra _OblivionTundra;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(10557))
            return;

        OblivionTundra.Storyline();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Oblivion Magus", // UseableMonsters[0],
            "Fiendish Devourer", // UseableMonsters[1],
            "Nightmare Fiend", // UseableMonsters[2],
            "Wasted Clone", // UseableMonsters[3],
            "Fiend of Voracity", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10549 | Oblivious Oblivion
        if (!Story.QuestProgression(10549))
        {
            Core.HuntMonsterQuest(10549,
                ("tercesarchive", UseableMonsters[0], ClassType.Farm));
        }


        // 10550 | Tongue and Cheek
        if (!Story.QuestProgression(10550))
        {
            Core.HuntMonsterQuest(10550,
                ("tercesarchive", UseableMonsters[1], ClassType.Farm));
        }


        // 10551 | O.T.
        if (!Story.QuestProgression(10551))
        {
            Core.HuntMonsterQuest(10551,
                ("tercesarchive", UseableMonsters[1], ClassType.Farm),
                ("tercesarchive", UseableMonsters[0], ClassType.Farm));
        }


        // 10552 | The Fiend is in the Details
        if (!Story.QuestProgression(10552))
        {
            Story.MapItemQuest(10552, "tercesarchive", 15393);
            Story.MapItemQuest(10552, "tercesarchive", 15394);
        }


        // 10553 | Soulectomy
        if (!Story.QuestProgression(10553))
        {
            Core.HuntMonsterQuest(10553,
                ("tercesarchive", UseableMonsters[3], ClassType.Farm));
        }


        // 10554 | A Nation Night Terror
        if (!Story.QuestProgression(10554))
        {
            Core.HuntMonsterQuest(10554,
                ("tercesarchive", UseableMonsters[2], ClassType.Farm));
        }


        // 10555 | Point of No Return
        if (!Story.QuestProgression(10555))
        {
            Core.HuntMonsterQuest(10555,
                ("tercesarchive", UseableMonsters[3], ClassType.Farm),
                ("tercesarchive", UseableMonsters[2], ClassType.Farm));
        }


        // 10556 | All Appetite
        if (!Story.QuestProgression(10556))
        {
            Core.HuntMonsterQuest(10556,
                ("tercesarchive", UseableMonsters[4], ClassType.Solo));
        }

        // 10557 | Adimonde's Invitation
        if (!Story.QuestProgression(10557))
        {
            Story.MapItemQuest(10557, "tercesarchive", 15395);
        }
    }



}