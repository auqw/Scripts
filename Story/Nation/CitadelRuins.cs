/*
name: Citadel Ruins
description: This will finish the Citadel Ruins quest.
tags: story, quest, nation, citadel-ruins
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class CitadelRuins
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public string[] rewards =
    {
        "Purified Claymore of Destiny",
        "Good Iron Wing 1",
        "Spinal Tap of Retribution",
        "Purified Claw of Nulgath",
        "Mage's Gratitude",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        Farm.Experience(30);
        Farm.GoodREP();
        MurrysQuests();
        PolishsQuestsTercessuinotlim();
        PolishsQuestsCitadelRuins();
        CutieMakaisQuest();
        CasparillasQuests();
    }

    public void CasparillasQuests()
    {
        if (Core.isCompletedBefore(6682))
            return;

        Story.PreLoad(this);

        // Easy AND Breezy!
        Story.KillQuest(6669, "forest", "Boss Zardman");

        // Disguises and a Ruse
        Story.KillQuest(6671, "bludrut2", "Shadow Creeper");

        // Icky and Smelly
        Story.KillQuest(6672, "marsh2", "Thrax Ironhide");

        // Dress for the Occasion!
        Story.KillQuest(6673, "sleuthhound", "Harmoire");

        // Can't Play Games without A Lunch Break
        Story.KillQuest(6674, "noobshire", "Kittarian Mouse Eater");

        // She Needs a Hero
        if (!Story.QuestProgression(6675))
        {
            Bot.Quests.UpdateQuest(3008);
            Core.EnsureAccept(6675);
            Core.HuntMonsterMapID("doomvault", 21, "Your Princess is Not in This Castle");
            Core.EnsureComplete(6675);
        }

        // Sun and Fun
        Story.KillQuest(6676, "beachparty", "Sun Flare");

        // Heebie Jeebies
        Story.KillQuest(6677, "marsh", "Dreadspider");

        // A New Pet
        Story.KillQuest(6678, "iceplane", "Enfield");

        // Spoiled Dragon
        Story.KillQuest(6679, "lair", "Red Dragon");

        // The Unpredictable Element
        Story.KillQuest(6680, "ledgermayne", "Ledgermayne");

        // There, But Not There
        Story.KillQuest(6681, "palace", "Invisible");

        // Staying Humble
        Core.AddDrop("Elite Void Sword Pet");
        if (!Story.QuestProgression(6682))
        {
            Core.EnsureAccept(6682);
            Core.KillArchFiendDragonLord("Twisted Armor Piece", 3);
            Core.EnsureComplete(6682);
        }
    }

    public void CutieMakaisQuest()
    {
        if (Core.isCompletedBefore(4325))
            return;

        Story.PreLoad(this);

        // Drearia On Demand
        if (!Story.QuestProgression(4312))
        {
            Core.EnsureAccept(4312);
            Core.GetMapItem(3485, 1, "drearia");
            Core.HuntMonster("drearia", "Dark Makai", "Dark Makai Notes", 1);
            Core.HuntMonster("drearia", "Evil Elemental", "Evil Elemental Notes", 1);
            Core.HuntMonster("drearia", "Green Rat", "Green Rat Notes", 1);
            Core.EnsureComplete(4312);
        }

        // Plant a Little Seed and Nature Grows
        Story.KillQuest(4313, "drearia", "Dark Makai");

        // A Key Discovery
        if (!Story.QuestProgression(4314))
        {
            Core.EnsureAccept(4314);
            Core.GetMapItem(3466, 1, "drearia");
            Core.HuntMonster("drearia", "Green Rat", "Front Door Key", 1);
            Core.EnsureComplete(4314);
        }

        // Creepy House... Yay!
        if (!Story.QuestProgression(4315))
        {
            Core.EnsureAccept(4315);
            Core.GetMapItem(3467, 1, "drearia");
            Core.HuntMonster("drearia", "Green Rat", "Defeated Varmint", 3);
            Core.EnsureComplete(4315);
        }

        // Sparkling Books
        Story.MapItemQuest(4316, "drearia", 3468);

        // A Paladin in Peril
        Story.MapItemQuest(4317, "swordhavenpink", 3469);

        // Pink Stinks!
        if (!Story.QuestProgression(4318))
        {
            Core.EnsureAccept(4318);
            Core.GetMapItem(3486, 5, "swordhavenpink");
            Core.HuntMonster("swordhavenpink", "Pink Slime", "Squished Pink Slime", 8);
            Core.EnsureComplete(4318);
        }

        // Rats, RATS!
        Story.KillQuest(4319, "sewerpink", "Pink Rat");

        // AdventureQuest Worm
        Story.KillQuest(4320, "sewerpink", "Cutie Grumbley");

        // UnBEARable Sight
        if (!Story.QuestProgression(4321))
        {
            Core.EnsureAccept(4321);
            Core.GetMapItem(3470, 1, "pinewoodpink");
            Core.HuntMonster("pinewoodpink", "Pink Grizzly", "Pink Teddy Bear", 5);
            Core.EnsureComplete(4321);
        }

        // Too Much Pink in Pinewood!
        if (!Story.QuestProgression(4322))
        {
            Core.EnsureAccept(4322);
            Core.GetMapItem(3471, 5, "pinewoodpink");
            Core.HuntMonster("pinewoodpink", "Pink Shell Turtle", "Bows", 5);
            Core.EnsureComplete(4322);
        }

        // Kill Sparkletooth
        Story.KillQuest(4323, "pinewoodpink", "Sparkletooth");

        // The Citadorable Plot
        Story.MapItemQuest(4324, "Citadel", 3472);

        // Fuzzy Run Minigame
        Story.KillQuest(4325, "pinewoodpink", "Pink Grizzly");
    }

    public void MurrysQuests()
    {
        if (Core.isCompletedBefore(182))
            return;

        Story.PreLoad(this);

        // "Citadel"
        Story.KillQuest(144, "Citadel", "Inquisitor Guard");
        Story.KillQuest(145, "Citadel", "Crusader");
        Story.KillQuest(146, "Citadel", "Inquisitor Captain");
        Story.KillQuest(147, "Citadel", "Burning Witch");
        Story.KillQuest(148, "Citadel", "Inquisitor Guard");
        Story.KillQuest(149, "Citadel", "Inquisitor Guard", false);
        Story.KillQuest(181, "Citadel", "Belrot The Fiend");
        if (!Story.QuestProgression(182)) // 151 dcs you when u try to load it, and can be skipped.. somehow idk itd break otherwise.
        {
            Core.EnsureAccept(182);
            Core.HuntMonster("Citadel", "Grand Inquisitor", "Ring of Evil Intent");
            Core.EnsureComplete(182);
        }
    }

    public void PolishsQuestsTercessuinotlim()
    {
        if (Core.isCompletedBefore(668))
            return;

        Story.PreLoad(this);

        Core.AddDrop(
            "Unidentified 9",
            "Unidentified 28",
            "Dark Crystal Shard",
            "Claw of Nulgath",
            "Relic of Chaos"
        );

        // "Tercessuinotlim"

        //iron wing helm enchant=
        Nation.TheAssistant("Unidentified 9");
        Story.KillQuest(560, "underworld", "Undead Bruiser", GetReward: false);

        //cleansing of spinal tap=
        Nation.Supplies("Dark Crystal Shard", 5);
        Nation.TheAssistant("Unidentified 28");
        Story.KillQuest(585, "Tercessuinotlim", "Legion Fenrir");

        //purified claw
        if (!Core.CheckInventory("Purified Claw of Nulgath"))
        {
            Core.AddDrop("Purified Claw of Nulgath");
            Core.EnsureAccept(668);
            Nation.FarmTaintedGem(7);
            Nation.Supplies("Claw of Nulgath");

            Core.ResetQuest(7551);
            Core.DarkMakaiItem("Dark Makai Sigil");
            Core.EnsureComplete(668);
        }
    }

    public void PolishsQuestsCitadelRuins()
    {
        if (Core.isCompletedBefore(6182))
            return;

        Story.PreLoad(this);

        Core.AddDrop("Mage's Gratitude");
        // "citadelruins";

        //get ready to amplify
        if (!Story.QuestProgression(6172))
        {
            Core.EnsureAccept(6172);
            Core.GetMapItem(5592, 1, "citadelruins");
            Core.HuntMonster("citadelruins", "Mana Sprites", "Siphoned Mana", 8);
            Core.EnsureComplete(6172);
        }

        //break the seal
        Story.MapItemQuest(6173, "citadelruins", 5602);

        //clear out the squatters
        if (!Story.QuestProgression(6174))
        {
            Core.EnsureAccept(6174);
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitor Hobos Defeated", 8);
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitors' Manifesto", 5);
            Core.EnsureComplete(6174);
        }

        //grab a clue
        if (!Story.QuestProgression(6175))
        {
            Core.EnsureAccept(6175);
            Core.GetMapItem(5593, 5, "citadelruins");
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Clue Found", 1);
            Core.EnsureComplete(6175);
        }
        Core.Sleep(2500);

        //unlock the door
        if (!Story.QuestProgression(6176))
        {
            Core.EnsureAccept(6176);
            Core.HuntMonster("citadelruins", "Inquisitor Heavy", "Heavies Beat Down", 2);
            Core.HuntMonster("citadelruins", "Inquisitor Heavy", "Citadel Key");
            Story.MapItemQuest(6176, "citadelruins", 5603);
        }

        //search the labs
        if (!Story.QuestProgression(6177))
        {
            Core.EnsureAccept(6177);
            Core.GetMapItems(new[] { (5594, 1), (5595, 1), (5596, 1) }, "citadelruins");
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitors Defeated", 6);
            Core.EnsureComplete(6177);
        }

        //break another seal
        if (!Story.QuestProgression(6178))
        {
            Core.EnsureAccept(6178);
            Core.GetMapItem(5597, 1, "citadelruins");
            Core.HuntMonster("citadelruins", "Mana Sprites", "Siphoned Mana", 8);
            Core.EnsureComplete(6178);
        }

        //inspect time
        Story.MapItemQuest(6179, "citadelruins", 5598);

        //find the parts
        if (!Story.QuestProgression(6180))
        {
            Core.EnsureAccept(6180);
            Core.GetMapItems(new[] { (5599, 1), (5600, 1), (5601, 1) }, "citadelruins");
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Missing Parts", 6);
            Core.EnsureComplete(6180);
        }

        //defeat the grand inquisitor
        Story.KillQuest(6181, "citadelruins", "Grand Inquisitor Murry");

        //defeat enn'tropy
        Story.KillQuest(6182, "citadelruins", "Enn'tröpy");
        Core.ToBank(rewards);
    }
}
