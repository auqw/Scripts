/*
name: WarMonger
description: Does the Quests from the Trainers in /Trainers
tags: trainers, quests, story, warmonger, warrior, mage, healer, rogue, warlord
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class WarMonger
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

        Trainers();

        Core.SetOptions(false);
    }

    public void Trainers()
    {
        if (Core.isCompletedBefore(10177))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Zardman Grunt", // UseableMonsters[0]
            "Zardman Hunter", // UseableMonsters[1]
            "Trobble", // UseableMonsters[2]
            "LeatherWing", // UseableMonsters[3]
            "Dwakel Blaster", // UseableMonsters[4]
            "Inquisitor Guard", // UseableMonsters[5]
            "Inquisitor Captain", // UseableMonsters[6]
            "Frogzard", // UseableMonsters[7]
            "Slime", // UseableMonsters[8]
            "Spider", // UseableMonsters[9]
            "Wolf", // UseableMonsters[10]
            "Chaorrupted Bear", // UseableMonsters[11]
            "Chaorrupted Wolf", // UseableMonsters[12]
            "Rock Elemental", // UseableMonsters[13]
            "River Fishman", // UseableMonsters[14]
            "Undead Berserker", // UseableMonsters[15]
            "Thrax Ironhide", // UseableMonsters[16]
            "Ogug Stoneaxe", // UseableMonsters[17]
            "Horc Warrior", // UseableMonsters[18]
            "Skeletal Warrior", // UseableMonsters[19]
            "Castle Wraith", // UseableMonsters[20]
            "Bone Terror", // UseableMonsters[21]
            "Eldritch Stalker", // UseableMonsters[22]
            "Terrarsite", // UseableMonsters[23]
            "Shadefire Onslaught", // UseableMonsters[24]
            "Ashray Fisherman", // UseableMonsters[25]
            "Kitefin Shark Bait", // UseableMonsters[26]
            "Lightguard Paladin", // UseableMonsters[27]
            "Noble's Knight", // UseableMonsters[28]
            "Garde Wraith", // UseableMonsters[29]
            "Undead Garde ", // UseableMonsters[30]
            "Warlord Harley", // UseableMonsters[31]
        };
        #endregion Useable Monsters


        #region Mage

        // 10156 | Perfect Practice
        if (!Story.QuestProgression(10156))
        {
            Core.HuntMonsterQuest(
                10156,
                ("forest", UseableMonsters[0], ClassType.Farm),
                ("forest", UseableMonsters[1], ClassType.Farm)
            );
        }

        // 10157 | Grateful Guru
        if (!Story.QuestProgression(10157))
        {
            Core.HuntMonsterQuest(
                10157,
                ("guru", UseableMonsters[2], ClassType.Farm),
                ("guru", UseableMonsters[3], ClassType.Farm)
            );
        }

        // 10158 | Magitech
        if (!Story.QuestProgression(10158))
        {
            Core.HuntMonsterQuest(10158, ("crashsite", UseableMonsters[4], ClassType.Solo));
        }

        // 10159 | Trial by Fire
        if (!Story.QuestProgression(10159))
        {
            Core.HuntMonsterQuest(
                10159,
                ("citadel", UseableMonsters[5], ClassType.Solo),
                ("citadel", UseableMonsters[6], ClassType.Solo)
            );
        }
        #endregion Mage


        #region Healer

        // 10160 | Pinch Your Nose
        if (!Story.QuestProgression(10160))
        {
            Core.HuntMonsterQuest(
                10160,
                ("greenguardwest", UseableMonsters[7], ClassType.Farm),
                ("greenguardwest", UseableMonsters[8], ClassType.Farm)
            );
        }

        // 10161 | Small Doses
        if (!Story.QuestProgression(10161))
        {
            Core.HuntMonsterQuest(
                10161,
                ("greenguardeast", UseableMonsters[9], ClassType.Solo),
                ("greenguardeast", UseableMonsters[10], ClassType.Solo)
            );
        }

        // 10162 | Chaotic Seedbed
        if (!Story.QuestProgression(10162))
        {
            Core.HuntMonsterQuest(
                10162,
                ("forestchaos", UseableMonsters[11], ClassType.Solo),
                ("forestchaos", UseableMonsters[12], ClassType.Farm)
            );
        }

        // 10163 | Dark's Embrace, Sacred Space
        if (!Story.QuestProgression(10163))
        {
            Core.HuntMonsterQuest(10163, ("bludrut", UseableMonsters[13], ClassType.Solo));
        }

        #endregion Healer

        #region Warrior
        // 10164 | Beginner's Hurdle
        if (!Story.QuestProgression(10164))
        {
            Core.HuntMonsterQuest(10164, ("river", UseableMonsters[14], ClassType.Farm));
        }

        // 10165 | Berserker Break
        if (!Story.QuestProgression(10165))
        {
            Core.HuntMonsterQuest(10165, ("marsh2", UseableMonsters[15], ClassType.Farm));
        }

        // 10166 | Path of the Warrior
        if (!Story.QuestProgression(10166))
        {
            Core.HuntMonsterQuest(10166, ("marsh2", UseableMonsters[16], ClassType.Solo));
        }

        // 10167 | Stone Will
        if (!Story.QuestProgression(10167))
        {
            Core.HuntMonsterQuest(10167, ("greenguardwest", UseableMonsters[17], ClassType.Solo));
        }
        #endregion Warrior

        #region Rogue

        // 10168 | Serious Stealth
        // Use iscomplete before due to 10168 being skipped for w/e reason with questprog.
        if (!Story.QuestProgression(10168))
        {
            Core.HuntMonsterQuest(10168, ("orctown", UseableMonsters[18], ClassType.Farm));
        }

        // 10169 | Shady Snooping
        if (!Story.QuestProgression(10169))
        {
            Core.HuntMonsterQuest(10169, ("battleundera", UseableMonsters[19], ClassType.Farm));
        }

        // 10170 | Stomping Grounds
        if (!Story.QuestProgression(10170))
        {
            Core.HuntMonsterQuest(10170, ("Castle", UseableMonsters[20], ClassType.Farm));
        }

        // 10171 | Swift Shadow
        if (!Story.QuestProgression(10171))
        {
            Core.HuntMonsterQuest(10171, ("battleundera", UseableMonsters[21], ClassType.Solo));
        }

        #endregion Rogue

        #region WarLord
        // 10172 | Chaos Reigned
        if (!Story.QuestProgression(10172))
        {
            Core.HuntMonsterQuest(
                10172,
                ("DeepForest", UseableMonsters[22], ClassType.Farm),
                ("DeepForest", UseableMonsters[23], ClassType.Farm)
            );
        }

        // 10173 | Fireproofing
        if (!Story.QuestProgression(10173))
        {
            Core.HuntMonsterQuest(10173, ("FirePlaneWar", UseableMonsters[24], ClassType.Farm));
        }

        // 10174 | Fishy-men Business
        if (!Story.QuestProgression(10174))
        {
            Core.HuntMonsterQuest(
                10174,
                ("Ashray", UseableMonsters[25], ClassType.Farm),
                ("Ashray", UseableMonsters[26], ClassType.Farm)
            );
        }

        // 10175 | Baleful Morals
        if (!Story.QuestProgression(10175))
        {
            Core.HuntMonsterQuest(
                10175,
                ("Balemorale", UseableMonsters[27], ClassType.Farm),
                ("Balemorale", UseableMonsters[28], ClassType.Farm)
            );
        }

        // 10176 | Nainhdeil
        if (!Story.QuestProgression(10176))
        {
            Core.HuntMonsterQuest(
                10176,
                ("LiaTaraHill", UseableMonsters[29], ClassType.Farm),
                ("LiaTaraHill", UseableMonsters[30], ClassType.Farm)
            );
        }

        // Farming Quest - very hard to solo without army.
        // // 10177 | Oxford Degree in Beatdowns
        // if (!Story.QuestProgression(10177))
        // {
        //     Core.HuntMonsterQuest(10177,
        //         ("trainers", UseableMonsters[31], ClassType.Solo));
        // }
        #endregion WarLord
    }
}
