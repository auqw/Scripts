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
        Story.MapItemQuest(10836, "templeofdoom", [16097, 16101]);

        // 10837 | Trust is the Cure
        Core.EquipClass(ClassType.Farm);
        Story.MapItemQuest(10837, "templeofdoom", [16102, 16114]);
        Story.KillQuest(10837, "templeofdoom", UseableMonsters[1]); // Drained Leech x15

        // 10838 | Inferiority Builds Character
        Story.MapItemQuest(10838, "templeofdoom", [16115, 16116, 16117]);
        Story.KillQuest(10838, "templeofdoom", UseableMonsters[4]); // Corrupted Gold x6

        // 10839 | Mission Briefing
        Story.MapItemQuest(10839, "templeofdoom", [16118, 16119]);

        // 10840 | The Light's Reward
        Story.MapItemQuest(10840, "templeofdoom", 16120);
        Story.KillQuest(10840, "templeofdoom", UseableMonsters[4]); // Erased Paladin x15

        // 10841 | Wordless Cipher
        Story.MapItemQuest(10841, "templeofdoom", 16121);

        // 10842 | Attention! Salute!
        Story.MapItemQuest(10842, "templeofdoom", [16122, 16123, 16124]);
        Story.KillQuest(10842, "templeofdoom", UseableMonsters[2]); // Deletion Denied x18

        // 10843 | All Roads Lead To…
        Story.MapItemQuest(10843, "templeofdoom", 16125, 3);

        // 10844 | Law and Order
        Story.KillQuest(10844, "templeofdoom", UseableMonsters[0]); // Aegis' Stone Veil x6

        // 10845 | There's an Order to These Things
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10845, "templeofdoom", UseableMonsters[3]); // Sentinel of Order Defeated x1
    }

}