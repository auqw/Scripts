/*
name: Aria's Greenhouse Story
description: This will finish the Aria's and Kylokos' Greenhouse quests.
tags: story, quest, aria, greenhouse, nature,water,fire,energy,ariagreenhouse,starfest,star festival,kylokos
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class AriaGreenhouse
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        Nature();
        Water();
        Fire();
        Energy();
        Kylokos();
    }

    public void Nature()
    {
        if (Core.isCompletedBefore(10188))
            return;

        Story.PreLoad(this);

        // 10179 | Drosanthemum
        Story.MapItemQuest(10179, "faerie", new[] { 14392, 14393 });


        // 10180 | String of Turtles
        if (!Story.QuestProgression(10180))
        {
            Core.HuntMonsterQuest(10180,
                ("pines", "Red Shell Turtle", ClassType.Farm));
        }


        // 10181 | Aconite
        Core.AddDrop(Core.QuestRewards(10181));
        Story.MapItemQuest(10181, "cloister", 14394);
        Story.KillQuest(10181, "cloister", "Karasu");


        // 10182 | Blood Red Carnations
        Story.MapItemQuest(10182, "darkoviaforest", 14395);


        // 10183 | Cottongrass
        if (!Story.QuestProgression(10183))
        {
            Core.HuntMonsterQuest(10183,
                ("northlands", "Snow Golem", ClassType.Farm));
        }


        // 10184 | Hebeloma Aminophilum
        Farm.Experience(60);
        Story.MapItemQuest(10184, "deleuzetundra", new[] { 14396, 14397 });
        Story.KillQuest(10184, "deleuzetundra", "Empty Creature");


        // 10185 | Annual Mercury
        if (!Story.QuestProgression(10185))
        {
            Core.HuntMonsterQuest(10185,
                ("yokaitreasure", "Quicksilver", ClassType.Farm));
        }


        // 10186 | Gorshka Maika
        if (!Story.QuestProgression(10186))
        {
            Core.HuntMonsterQuest(10186,
                ("hakuwar", "Zakhvatchik", ClassType.Solo));
        }


        // 10187 | Alchemilla
        if (!Story.QuestProgression(10187))
        {
            Core.HuntMonsterQuest(10187,
                ("yokairealm", "Mikoto Kukol'nyy", ClassType.Solo));
        }


        // 10188 | Arianne Vivaldi
        Story.MapItemQuest(10188, "vivaldicavern", 14398);
    }

    public void Water()
    {
        if (Core.isCompletedBefore(10203))
            return;

        Story.PreLoad(this);

        Core.AddDrop(Core.QuestRewards(10193, 10194, 10195, 10196, 10197));

        // 10189 | People and Scarecrows
        if (!Story.QuestProgression(10189))
        {
            Core.HuntMonsterQuest(10189,
                ("farm", "Scarecrow", ClassType.Farm));
        }


        // 10190 | Dolphins and Sharks
        if (!Story.QuestProgression(10190))
        {
            Core.HuntMonsterQuest(10190,
                ("pirates", "Fishman Soldier", ClassType.Farm));
        }


        // 10191 | Dragons and Lizards
        if (!Story.QuestProgression(10191))
        {
            Core.HuntMonsterQuest(10191,
                ("natatorium", "Merdraconian", ClassType.Farm));
        }


        // 10192 | Water and Oil
        Farm.Experience(60);
        Story.MapItemQuest(10192, "feverfew", 14399);
        Story.KillQuest(10192, "feverfew", "Coral Creeper");

        // 10193 | Black Silk and White Fleece
        if (!Story.QuestProgression(10193))
        {
            Core.HuntMonsterQuest(10193,
                ("blackseakeep", "Blacksea Pirate Mage", ClassType.Farm));
        }


        // 10194 | Sunlight and Moonbeams
        if (!Story.QuestProgression(10194))
        {
            Core.HuntMonsterQuest(10194,
                ("sunlightzone", "Blighted Water", ClassType.Farm));
        }


        // 10195 | Midnight and Noon
        if (!Story.QuestProgression(10195))
        {
            Core.HuntMonsterQuest(10195,
                ("midnightzone", "Venerated Wraith", ClassType.Farm));
        }


        // 10196 | Lie and Truth
        if (!Story.QuestProgression(10196))
        {
            Core.HuntMonsterQuest(10196,
                ("abyssalzone", "Blighted Water", ClassType.Farm));
        }


        // 10197 | Odette and Odile
        if (!Story.QuestProgression(10197))
        {
            Core.Logger("Boss can't be soloed, so we skip it. Use army to kill it.");
            return;
        }


        // 10198 | Hyonix's Treasure Chest
        Story.ChainQuest(10198);

        // 10203 | Sinclair and Demien
        Story.MapItemQuest(10203, "sinclaircove", 14400);


    }

    public void Fire()
    {
        if (Core.isCompletedBefore(10213))
            return;

        Story.PreLoad(this);

        // 10204 | Grassroots Opinion
        if (!Story.QuestProgression(10204))
        {
            Core.HuntMonsterQuest(10204,
                ("greendragon", "Greenguard Dragon", ClassType.Solo));
        }


        // 10205 | Flame War
        if (!Story.QuestProgression(10205))
        {
            Core.HuntMonsterQuest(10205,
                ("bludrut2", "Fire Elemental", ClassType.Farm));
        }


        // 10206 | Ashes Reignited
        if (!Story.QuestProgression(10206))
        {
            Core.HuntMonsterQuest(10206,
                ("lair", "Water Draconian", ClassType.Farm));
        }


        // 10207 | Feldspar Silica
        Story.MapItemQuest(10207, "mafic", 14402);
        Story.KillQuest(10207, "mafic", "Living Fire");


        // 10208 | Convective Conversation
        if (!Story.QuestProgression(10208))
        {
            Core.HuntMonsterQuest(10208,
                ("embersea", "Living Lava", ClassType.Farm));
        }


        // 10209 | Spicy Chicken
        if (!Story.QuestProgression(10209))
        {
            Core.HuntMonsterQuest(10209,
                ("lavarun", "Phedra", ClassType.Solo));
        }


        // 10210 | Akriloth's Inferno
        if (!Story.QuestProgression(10210))
        {
            Core.HuntMonsterQuest(10210,
                ("firewar", "Uriax", ClassType.Solo));
        }


        // 10211 | Scattered Embers
        Story.MapItemQuest(10211, "fireplanewar", 14403, 11);
        Story.KillQuest(10211, "fireplanewar", "Shadefire Onslaught");


        // 10212 | Tempered Restraint
        if (!Story.QuestProgression(10212))
        {
            Core.HuntMonsterQuest(10212,
                ("wartraining", "Fire Champion", ClassType.Solo));
        }


        // 10213 | Extinct Volcano
        Story.MapItemQuest(10213, "kingeldfell", 14404);
    }

    public void Energy()
    {
        if (Core.isCompletedBefore(10223))
            return;

        Story.PreLoad(this);


        // 10214 | Dragonlord Dreams
        if (!Story.QuestProgression(10214))
        {
            Core.HuntMonsterQuest(10214,
                ("dwarfhold", "Chaos Drow", ClassType.Farm));
        }


        // 10215 | Background Noise
        if (!Story.QuestProgression(10215))
        {
            Core.HuntMonsterQuest(10215,
                ("uppercity", "Drow Assassin", ClassType.Farm));
        }


        // 10216 | Manticore Prince
        if (!Story.QuestProgression(10216))
        {
            Core.HuntMonsterQuest(10216,
                ("venomvaults", "Chaonslaught Warrior", ClassType.Farm));
        }


        // 10217 | Solid Clouds
        Story.MapItemQuest(10217, "stormtemple", 14405, 8);
        Story.KillQuest(10217, "stormtemple", "Chaonslaught Warrior");


        // 10218 | Electric Rivalry
        if (!Story.QuestProgression(10218))
        {
            Core.HuntMonsterQuest(10218,
                ("thunderfang", "Tonitru", ClassType.Solo));
        }


        // 10219 | A Dragon's Dignity
        if (!Story.QuestProgression(10219))
        {
            Core.HuntMonsterQuest(10219,
                ("pride", "Valsarian", ClassType.Solo));
        }


        // 10220 | A Show Horse
        if (!Story.QuestProgression(10220))
        {
            Core.HuntMonsterQuest(10220,
                ("balemorale", "Skye Warrior", ClassType.Farm));
        }


        // 10221 | Elemental Executioners
        if (!Story.QuestProgression(10221))
        {
            Core.HuntMonsterQuest(10221,
                ("loughshine", "Energy Elemental", ClassType.Farm));
        }


        // 10222 | Pride's Grave
        if (!Story.QuestProgression(10222))
        {
            Core.HuntMonsterQuest(10222,
                ("naoisegrave", "Volgritian", ClassType.Solo));
        }


        // 10223 | King of the Tempest
        Story.MapItemQuest(10223, "terminagrove", 14406);


    }

    public void Kylokos()
    {
        if (Core.isCompletedBefore(10318))
            return;

        Story.PreLoad(this);


        // 10309 | Epsilon Tau
        if (!Story.QuestProgression(10309))
        {
            Core.HuntMonsterQuest(10309,
                ("starfield", "Nova Seed", ClassType.Farm));
        }


        // 10310 | Fixed Earth
        if (!Story.QuestProgression(10310))
        {
            Core.HuntMonsterQuest(10310,
                ("hedgemaze", "Minotaur", ClassType.Farm));
        }


        // 10311 | Fable Fall
        if (!Story.QuestProgression(10311))
        {
            Core.HuntMonsterQuest(10311,
                ("fableforest", "Fire Elemental", ClassType.Farm));
        }


        // 10312 | Delta Andromedae
        if (!Story.QuestProgression(10312))
        {
            Core.HuntMonsterQuest(10312,
                ("ashray", "Kitefin Shark Bait", ClassType.Farm));
        }


        // 10313 | Beta Arietis
        if (!Story.QuestProgression(10313))
        {
            Core.HuntMonsterQuest(10313,
                ("kolyaban", "Tentacled Darkblood", ClassType.Farm));
        }


        // 10314 | 35 Arietis
        if (!Story.QuestProgression(10314))
        {
            Core.HuntMonsterQuest(10314,
                ("comet", "Vaderix", ClassType.Farm));
        }


        // 10315 | Fractured Pleiades
        if (!Story.QuestProgression(10315))
        {
            Core.HuntMonsterQuest(10315,
                ("atlaspromenade", "Atlas Light Magus", ClassType.Solo));
        }


        // 10316 | Lambda Ori
        if (!Story.QuestProgression(10316))
        {
            Core.HuntMonsterQuest(10316,
                ("victormatsuri", "Kitsune Himawari", ClassType.Solo));
        }


        // 10317 | Delta Ori
        if (!Story.QuestProgression(10317))
        {
            Core.HuntMonsterQuest(10317,
                ("hakuwar", "Long Kukol'nyy", ClassType.Solo));
        }


        // 10318 | Western Moon Mansions
        if (!Story.QuestProgression(10318))
        {
            Core.HuntMonsterQuest(10318,
                ("yokaiportal", "Kitsune Spirits", ClassType.Farm));
        }


    }
}
