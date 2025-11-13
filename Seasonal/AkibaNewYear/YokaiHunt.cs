/*
name: Yokai Hunt
description: Completes the quests in yokaihunt.
tags: yokai, seasonal, akiba-new-year, yokaihunt, story, baoyu lin, ai no miko, yue huang
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class YokaiHunt
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

        DoAll();
        Core.SetOptions(false);
    }

    public void DoAll()
    {
        AiNoMiko();
        YueHuang();
        BaoyuLin();
        AiNoMiko2();
    }

    public void AiNoMiko()
    {
        if (Core.isCompletedBefore(7941) || !Core.isSeasonalMapActive("yokaihunt"))
            return;

        Story.PreLoad(this);

        //Nope-perabo (7936)
        Story.KillQuest(7936, "yokaihunt", "Ox Nopperabo");

        //Intel Processor (7937)
        Story.KillQuest(7937, "yokaihunt", "Golden Ox Guard");

        //Scout Out (7938)
        Story.MapItemQuest(7938, "yokaihunt", 8133);
        Story.KillQuest(7938, "yokaihunt", "Golden Ox Guard");

        //Re-info-source-ments (7939)
        Story.KillQuest(7939, "yokaihunt", "Golden Ox Guard");

        //Yokai Ox Spirit (7940)
        Story.KillQuest(7940, "yokaihunt", "Ox Yokai Spirit");

        //Etokoun Captured (7941)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7941, "yokaihunt", "Etokoun");
    }

    public void YueHuang()
    {
        if (Core.isCompletedBefore(9098) || !Core.isSeasonalMapActive("yokaihunt"))
            return;

        AiNoMiko();

        Story.PreLoad(this);

        //Vitamoon (9092)
        Story.KillQuest(9092, "natatorium", new[] { "Anglerfish", "Merdraconian" });

        //Fill'er Up (9093)
        if (!Story.QuestProgression(9093))
        {
            Core.EnsureAccept(9093);
            Core.KillMonster(
                "wanders",
                "r5",
                "Left",
                "Lotus Spider",
                "Lotus Seeds",
                10,
                log: false
            );
            Core.HuntMonster("battlefowl", "ChickenCow", "Chickencow Egg", 3, log: false);
            Core.EnsureComplete(9093);
        }

        //Mirror Flowers (9094)
        Story.KillQuest(9094, "guardiantree", new[] { "Blossoming Treeant", "Seed Spitter" });

        //Moon's Reflection (9095)
        Story.KillQuest(9095, "beachparty", new[] { "Water Elemental", "Boiling Elemental" });

        //Shifting Faces (9096)
        Story.KillQuest(9096, "safiria", new[] { "Albino Bat", "Chaos Lycan" });

        //Eto... Bleh (9097)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9097, "yokaihunt", "Etokoun");

        //Elixir of the Moon (9098)
        Story.KillQuest(9098, "yokaihunt", "Elixir Etokoun");
    }

    public void BaoyuLin()
    {
        if (Core.isCompletedBefore(9575) || !Core.isSeasonalMapActive("yokaihunt"))
            return;

        YueHuang();

        Story.PreLoad(this);

        // Art of Resilience (9571)
        Story.KillQuest(9571, "zhu", "Plum Treeant");

        // Lucky Red (9572)
        Story.KillQuest(9572, "shipwreck", new[] { "Gilded Merdraconian", "Lobthulhu" });

        // Palm Soot (9573)
        Story.KillQuest(9573, "burningbeach", "Lava Guardian");

        // Hong of the West (9574)
        Story.KillQuest(9574, "ashfallcamp", "Smoldur");

        // Baihong Guan Ri (9575)
        Story.KillQuest(9575, "yokaihunt", "Mutou Hong");
    }

    public void AiNoMiko2()
    {
        if (Core.isCompletedBefore(10060) || !Core.isSeasonalMapActive("yokaihunt"))
            return;

        BaoyuLin();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Ox Nopperabo", // UseableMonsters[0],
            "Golden Ox Guard", // UseableMonsters[1],
            "Ox Yokai Spirit", // UseableMonsters[2],
            "Etokoun", // UseableMonsters[3],
            "Elixir Etokoun", // UseableMonsters[4],
            "Mutou Hong", // UseableMonsters[5],
            "Zhenzhu Shé", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10056 | Xionghuang Remedy
        if (!Story.QuestProgression(10056))
        {
            Core.HuntMonsterQuest(
                10056,
                ("yokairiver", "Kappa Ninja", ClassType.Farm),
                ("yokairiver", "Funa-yurei", ClassType.Farm)
            );
        }

        // 10057 | Calabash Elixir
        if (!Story.QuestProgression(10057))
        {
            Core.HuntMonsterQuest(10057, ("yokaihunt", UseableMonsters[4], ClassType.Solo));
        }

        // 10058 | Duàn Qiáo
        if (!Story.QuestProgression(10058))
        {
            Core.HuntMonsterQuest(10058, ("hakuvillage", "Mountain Oni", ClassType.Farm));
        }

        // 10059 | Sangharama's Blessing
        if (!Story.QuestProgression(10059))
        {
            Core.HuntMonsterQuest(10059, ("shogunwar", "Shadow Samurai", ClassType.Farm));
        }

        // 10060 | Lady Suzhen
        if (!Story.QuestProgression(10060))
        {
            Core.HuntMonsterQuest(10060, ("yokaihunt", UseableMonsters[6], ClassType.Solo));
        }
    }
}
