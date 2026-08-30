/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs 
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;

public class CoreSunSet
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void DoAll()
    {
        TempleofDoom();
    }
    private static CoreOasis COA
    {
        get => _COA ??= new CoreOasis();
        set => _COA = value;
    }
    private static CoreOasis _COA;


    public void TempleofDoom()
    {
        COA.DoAll();
        if (Core.isCompletedBefore(10845))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Aegis of Order", // UseableMonsters[0]
            "Doom Leech", // UseableMonsters[1]
            "Emptiness", // UseableMonsters[2]
            "Sentinel of Order", // UseableMonsters[3]
            "Tainted Paladin", // UseableMonsters[4]
        ];

        // 10836 | Within a Raw Wound
        if (!Story.QuestProgression(10836))
        {
            Story.MapItemQuest(10836, "templeofdoom", [16097, 16101], AutoCompleteQuest: false);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("templeofdoom", UseableMonsters[2], "Erased Text", 12);
            Core.EnsureComplete(10836);
        }

        // 10837 | Trust is the Cure
        if (!Story.QuestProgression(10837))
        {
            Story.MapItemQuest(10837, "templeofdoom", [16102, 16114], AutoCompleteQuest: false);
            Core.HuntMonsterQuest(10837, ("templeofdoom", UseableMonsters[1], ClassType.Farm)); // Drained Leech x15
        }

        // 10838 | Inferiority Builds Character
        if (!Story.QuestProgression(10838))
        {
            Story.MapItemQuest(10838, "templeofdoom", [16115, 16116, 16117], AutoCompleteQuest: false);
            Core.HuntMonsterQuest(10838, ("templeofdoom", UseableMonsters[4], ClassType.Farm)); // Corrupted Gold x6
        }

        // 10839 | Mission Briefing
        if (!Story.QuestProgression(10839)) Story.MapItemQuest(10839, "templeofdoom", [16118, 16119]);

        // 10840 | The Light's Reward
        if (!Story.QuestProgression(10840))
        {
            Story.MapItemQuest(10840, "templeofdoom", 16120, AutoCompleteQuest: false);
            Core.HuntMonsterQuest(10840, ("templeofdoom", UseableMonsters[4], ClassType.Farm)); // Erased Paladin x15
        }

        // 10841 | Wordless Cipher
        if (!Story.QuestProgression(10841)) Story.MapItemQuest(10841, "templeofdoom", 16121);

        // 10842 | Attention! Salute!
        if (!Story.QuestProgression(10842))
        {
            Story.MapItemQuest(10842, "templeofdoom", [16122, 16123, 16124], AutoCompleteQuest: false);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("templeofdoom", UseableMonsters[2], "Deletion Denied", 18);
            Core.EnsureComplete(10842);
        }

        // 10843 | All Roads Lead To…
        if (!Story.QuestProgression(10843)) Story.MapItemQuest(10843, "templeofdoom", 16125, 3);

        // 10844 | Law and Order
        if (!Story.QuestProgression(10844))
        {
            Core.HuntMonsterQuest(10844, ("templeofdoom", UseableMonsters[0], ClassType.Solo)); // Aegis' Stone Veil x6
        }

        // 10845 | There's an Order to These Things
        if (!Story.QuestProgression(10845))
        {
            Core.HuntMonsterQuest(10845, ("templeofdoom", UseableMonsters[3], ClassType.Solo)); // Sentinel of Order Defeated x1
        }
    }

}
