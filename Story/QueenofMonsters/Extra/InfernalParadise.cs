/*
name: Infernal Paradise
description: This will finish the Aranx' quests in /infernalparadise.
tags: story, quest, queen of monster, celestial realm, infernalparadise,aranx, extra,qom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
using Skua.Core.Interfaces;

public class InfernalParadise
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CelestialPast CP
    {
        get => _CP ??= new CelestialPast();
        set => _CP = value;
    }
    private static CelestialPast _CP;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10081))
            return;

        CP.CompleteCeletialPast();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Infernal Mage", // UseableMonsters[0],
            "Infernal Malxas", // UseableMonsters[1],
            "Infernal Knight", // UseableMonsters[2],
            "Akh-a", // UseableMonsters[3],
            "Maah-na", // UseableMonsters[4],
            "Azalith", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10073 | Revoked Devotion
        if (!Story.QuestProgression(10073))
        {
            Core.HuntMonsterQuest(10073, ("infernalparadise", UseableMonsters[0], ClassType.Farm));
        }

        // 10074 | Reverence is Fear
        Story.MapItemQuest(10074, "infernalparadise", 14158, 5);

        // 10075 | Incidental Fall
        if (!Story.QuestProgression(10075))
        {
            Core.HuntMonsterQuest(10075, ("infernalparadise", UseableMonsters[1], ClassType.Solo));
        }

        // 10076 | Knight-Erratic
        if (!Story.QuestProgression(10076))
        {
            Core.HuntMonsterQuest(10076, ("infernalparadise", UseableMonsters[2], ClassType.Farm));
        }

        // 10077 | Terrified Terrors
        Story.MapItemQuest(10077, "infernalparadise", 14159, 5);

        // 10078 | Eudaimonia
        if (!Story.QuestProgression(10078))
        {
            Core.HuntMonsterQuest(
                10078,
                ("infernalparadise", UseableMonsters[3], ClassType.Solo),
                ("infernalparadise", UseableMonsters[4], ClassType.Solo)
            );
        }

        // 10079 | Giver and Redeemer
        Story.MapItemQuest(10079, "infernalparadise", 14160, 6);

        // 10080 | Losing the Cause
        if (!Story.QuestProgression(10080))
        {
            Core.HuntMonsterQuest(10080, ("infernalparadise", UseableMonsters[0], ClassType.Farm));
        }

        // 10083 | Building Doubts
        if (!Story.QuestProgression(10083))
        {
            Core.HuntMonsterQuest(10083, ("infernalparadise", UseableMonsters[2], ClassType.Farm));
        }

        // 10081 | Azalith the Disavowed
        if (!Story.QuestProgression(10081))
        {
            Core.EquipClass(ClassType.Dodge);
            Core.HuntMonsterQuest(10081, ("infernalparadise", UseableMonsters[5], ClassType.Solo));
        }
    }
}
