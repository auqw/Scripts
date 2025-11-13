/*
name: Infernal Dianoia
description: This will finish the Azalith's quests in /infernaldianoia.
tags: story, quest, queen of monster, celestial realm, infernaldianoia,aranx,azalith, extra,qom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalParadise.cs
using Skua.Core.Interfaces;

public class InfernalDianoia
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static InfernalParadise IP
    {
        get => _IP ??= new InfernalParadise();
        set => _IP = value;
    }
    private static InfernalParadise _IP;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10094))
            return;

        IP.Storyline();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Infernal Pegasus", // UseableMonsters[0],
            "Bright Dominion", // UseableMonsters[1],
            "Fallen Arthelyn", // UseableMonsters[2],
            "Eudae", // UseableMonsters[3],
            "Avatar of Life", // UseableMonsters[4],
            "Avatar of Spirits", // UseableMonsters[5],
            "Avatar of Time", // UseableMonsters[6],
            "Aranx, Nightstar", // UseableMonsters[7]
        };
        #endregion Useable Monsters

        // 10085 | Fallen Feathers
        if (!Story.QuestProgression(10085))
        {
            Core.HuntMonsterQuest(10085, ("infernaldianoia", UseableMonsters[0], ClassType.Farm));
        }

        // 10086 | Seven Heavens
        if (!Story.QuestProgression(10086))
        {
            Core.HuntMonsterQuest(10086, ("infernaldianoia", UseableMonsters[1], ClassType.Farm));
        }

        // 10087 | Omniscient Judgement
        Story.MapItemQuest(10087, "infernaldianoia", 14174, 7);

        // 10088 | Rapturous Ascent
        if (!Story.QuestProgression(10088))
        {
            Core.HuntMonsterQuest(
                10088,
                ("infernaldianoia", UseableMonsters[0], ClassType.Farm),
                ("infernaldianoia", UseableMonsters[1], ClassType.Farm)
            );
        }

        // 10089 | Broken Stoicism
        if (!Story.QuestProgression(10089))
        {
            Core.HuntMonsterQuest(10089, ("infernaldianoia", UseableMonsters[2], ClassType.Solo));
        }

        // 10090 | Bitter Blessing
        if (!Story.QuestProgression(10090))
        {
            Core.HuntMonsterQuest(10090, ("infernaldianoia", UseableMonsters[3], ClassType.Solo));
        }

        // 10091 | Life and Spirits
        if (!Story.QuestProgression(10091))
        {
            Core.HuntMonsterQuest(
                10091,
                ("infernaldianoia", UseableMonsters[4], ClassType.Solo),
                ("infernaldianoia", UseableMonsters[5], ClassType.Solo)
            );
        }

        // 10092 | On Your Time
        if (!Story.QuestProgression(10092))
        {
            Core.HuntMonsterQuest(10092, ("infernaldianoia", UseableMonsters[6], ClassType.Solo));
        }

        // 10093 | Rapid Metanoia
        if (!Story.QuestProgression(10093))
        {
            Core.HuntMonsterQuest(10093, ("infernaldianoia", UseableMonsters[3], ClassType.Solo));
        }

        // 10094 | Filial Impiety
        if (!Story.QuestProgression(10094))
        {
            Core.HuntMonsterQuest(10094, ("infernaldianoia", UseableMonsters[7], ClassType.Solo));
        }
    }
}
