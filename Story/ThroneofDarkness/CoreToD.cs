/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class CoreToD
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    bool doAll = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void CompleteToD(bool SkipDPRepStorys = false)
    {
        int[] questIDs = { 4992, 5050, 5108, 5120, 5154, 5165, 5187, 5212, 5313, 5332, 5434, 5451 };
        if (Core.IsMember)
            questIDs = questIDs.Concat(new[] { 5010, 5022, 5083 }).ToArray();
        Core.EnsureLoad(questIDs);
        if (questIDs.All(x => Core.isCompletedBefore(x)))
            return;

        Story.PreLoad(this);
        doAll = true;

        //Vaden - The DeathKnight
        CastleofBones();
        BoneTowerAll();

        //Xeven - The Time Assassin
        ParadoxPortal();
        DeepSpace();

        //Ziri - The Daimon
        BaconCatFortress();
        LaserSharkInvasion();

        //Pax - The Warlord
        if (!SkipDPRepStorys)
        {
            DeathPit();
            DeathPitPVP();
        }

        //Sekt - The Eternal
        ShiftingPyramid();
        FourthDimensionalPyramid();
        Yasaris();

        //Scarletta - The Sorceress
        ShatterGlassMaze();
        TowerofMirrors();

        //??? - The Stranger
        AntiqueShop();
        MysteriousDungeon();
    }

    #region Vaden - The DeathKnight

    public void CastleofBones()
    {
        if (Core.isCompletedBefore(4992))
            return;

        if (doAll)
            Core.Logger("Vaden - The DeathKnight: Castle of Bones");
        Story.PreLoad(this);

        // Enter the Castle of Bone
        Story.KillQuest(4968, "bonecastle", "Undead Guard");

        // Slay Vaden's Undead Guards
        Story.KillQuest(4969, "bonecastle", "Undead Guard");

        // Destroyer in the Foyer
        Story.KillQuest(4970, "bonecastle", "Undead Knight");

        // Help! I'm a Fallen Lord and I Can't Get Up!
        if (Bot.Player.Cell != "r4")
            Core.Jump("r4", "Left"); //cutscene issues...?
        Story.KillQuest(4971, "bonecastle", "Fallen Deathknight");

        // Gem-nastics
        if (!Story.QuestProgression(4972))
        {
            Core.EnsureAccept(4972);
            Core.GetMapItem(4342, 3, "bonecastle");
            Core.HuntMonster("bonecastle", "Fallen Deathknight", "Necrogem", 3);
            Core.EnsureComplete(4972);
        }

        // Bone Appetit
        Story.KillQuest(4973, "bonecastle", "Undead Waiter");

        // The Walking Bread
        if (!Story.QuestProgression(4974))
        {
            Core.EnsureAccept(4974);
            Core.GetMapItem(4343, 2, "bonecastle");
            Core.GetMapItem(4344, 2, "bonecastle");
            Core.GetMapItem(4345, 3, "bonecastle");
            Core.HuntMonster("bonecastle", "Undead Knight", "Knight's Fork", 1);
            Core.EnsureComplete(4974);
        }

        // Ghoul-ash
        Story.KillQuest(4975, "bonecastle", "Ghoul");

        // The Butcher
        Story.KillQuest(4976, "bonecastle", "The Butcher");

        // Lean Mean Undead Slaying Machine
        Story.KillQuest(4977, "bonecastle", "Skeletal Warrior");

        // Moar Loot
        if (!Story.QuestProgression(4978))
        {
            Core.EnsureAccept(4978);
            Core.GetMapItems(new[] { (4346, 1), (4347, 1), (4348, 1) }, "bonecastle");
            Core.HuntMonster("bonecastle", "Skeletal Warrior", "Warrior's Screwdriver", 1);
            Core.EnsureComplete(4978);
        }

        // Putting Your Hands All over Everything
        if (!Story.QuestProgression(4979))
        {
            Core.EnsureAccept(4979);
            Core.GetMapItems(new[] { (4349, 1), (4350, 1), (4351, 1) }, "bonecastle");
            Core.HuntMonster("bonecastle", "Skeletal Warrior", "Warrior's Dusting Rag", 1);
            Core.EnsureComplete(4979);
        }

        // Paladin Rock
        if (!Story.QuestProgression(4980))
        {
            Core.EnsureAccept(4980);
            Core.GetMapItems(new[] { (4354, 1), (4355, 1) }, "bonecastle");
            Core.HuntMonster("bonecastle", "Grateful Undead", "Song Request Ticket", 5);
            Core.HuntMonster("bonecastle", "That 70's Zombie", "Sweet Dancing Shoes", 2);
            Core.EnsureComplete(4980);
        }

        // Do You Find This Humerus?
        if (!Story.QuestProgression(4981))
        {
            Core.EnsureAccept(4981);
            Core.KillMonster("bonecastle", "r8", "Left", "Skeletal Warrior", "Undead Humerus Bones", 5);
            Core.EnsureComplete(4981);
        }

        // Vaden Says
        if (!Story.QuestProgression(4982))
        {
            Core.EnsureAccept(4982);
            Core.KillMonster("bonecastle", "Enter", "Spawn", "Undead Guard", "Yellow, Green");
            Core.KillMonster("bonecastle", "r3", "Bottom", "Undead Knight", "Red, Red");
            Core.KillMonster("bonecastle", "r8", "Left", "Skeletal Warrior", "Blue, Green, Red");
            Core.EnsureComplete(4982);
        }

        // The Dead King's Bedroom
        Story.MapItemQuest(4983, "bonecastle", 4352);

        // Game of Porcelain Thrones
        Story.MapItemQuest(4984, "bonecastle", 4353, 4);

        // Teenage Mutant Sewer Turtles
        Story.KillQuest(4985, "bonecastle", "Turtle");

        // Adolescent Inhuman Samurai Reptiles
        if (!Story.QuestProgression(4986))
        {
            Core.EnsureAccept(4986);
            Core.HuntMonster("bonecastle", "Turtle", "Dali's Bo", 1);
            Core.HuntMonster("bonecastle", "Turtle", "Basquiat's Sai", 2);
            Core.HuntMonster("bonecastle", "Turtle", "Warhol's Nunchaku", 1);
            Core.HuntMonster("bonecastle", "Turtle", "Picasso's Katanas", 2);
            Core.EnsureComplete(4986);
        }

        // Snuggles!
        Story.KillQuest(4987, "bonecastle", "Snuggles, Torturer");

        // Game of Bones
        if (!Story.QuestProgression(4988))
        {
            Core.EnsureAccept(4988);
            Core.HuntMonster("bonecastle", "Jon Bones", "Jon Bones' Cape", 1);
            Core.HuntMonster("bonecastle", "Oberon Marrowtell", "Oberon Marrowtell's Armor", 1);
            Core.HuntMonster("bonecastle", "Baskerville", "Baskerville's Helmet", 1);
            Core.HuntMonster("bonecastle", "Knight of Lichens", "Knight of Lichens' Lance", 1);
            Core.EnsureComplete(4988);
        }

        // Rot Tin Tin!
        Story.KillQuest(4989, "bonecastle", "Rot Tin Tin");

        // Gold Digger
        if (!Story.QuestProgression(4990))
        {
            Core.EnsureAccept(4990);
            Core.HuntMonster("bonecastle", "Undead Golden Knight", "Vaden's Gold", 50);
            Core.HuntMonster("bonecastle", "Undead Golden Knight", "Gold Skull", 10);
            Core.HuntMonster("bonecastle", "Undead Golden Knight", "Crystal Skull", 1);
            Core.EnsureComplete(4990);
        }

        // Gotta Hand It To Ya
        if (!Story.QuestProgression(4991))
        {
            Core.EnsureAccept(4991);
            Core.HuntMonster("bonecastle", "Undead Knight", "Knight Defeated", 5);
            Core.HuntMonster("bonecastle", "Skeletal Warrior", "Warrior Defeated", 5);
            Core.EnsureComplete(4991);
        }

        // Vaden's Defeat
        Story.KillQuest(4992, "bonecastle", "Vaden");
    }

    public void BoneTowerAll()
    {
        if (!Core.IsMember || Core.isCompletedBefore(5022))
            return;

        if (doAll)
            Core.Logger("Vaden - The DeathKnight: Bone Tower (Silver)");
        Story.PreLoad(this);
        CastleofBones();

        // Eye Sp-eye
        Story.KillQuest(4996, "towersilver", "Flying Spyball");

        // Stone Cold
        Story.KillQuest(4997, "towersilver", "Fallen Emperor Statue");

        // Slay 'Em All
        if (!Story.QuestProgression(4999))
        {
            Core.EnsureAccept(4999);
            Core.HuntMonster("towersilver", "Undead Guard", "Undead Guard Slain", 3);
            Core.HuntMonster("towersilver", "Undead Knight", "Undead Knight Slain", 3);
            Core.EnsureComplete(4999);
        }

        // Slay 'Em All Again
        if (!Story.QuestProgression(4998))
        {
            Core.EnsureAccept(4998);
            Core.HuntMonster("towersilver", "Fallen Deathknight", "Fallen Deathknight Slain", 3);
            Core.HuntMonster("towersilver", "Undead Warrior", "Undead Warrior Slain", 3);
            Core.EnsureComplete(4998);
        }

        // Farming For Loot
        if (!Story.QuestProgression(5000))
        {
            Core.EnsureAccept(5000);
            Core.HuntMonster("towersilver", "Flying Spyball", "Sp-Eye Camera");
            Core.HuntMonster("towersilver", "Fallen Deathknight", "Deathknight's Axe");
            Core.HuntMonster("towersilver", "Undead Warrior", "Warrior's Shield");
            Core.HuntMonster("towersilver", "Undead Knight", "Knight's Helm");
            Core.HuntMonster("towersilver", "Undead Guard", "Guard's Skull");
            Core.EnsureComplete(5000);
        }

        // Or... Not.
        Story.MapItemQuest(5001, "towersilver", new[] { 4368, 4369, 4370, 4371, 4372 });

        // Mirror, Mirror
        Story.MapItemQuest(5002, "towersilver", 4373, 3);

        // Bloody Scary
        Story.KillQuest(5003, "towersilver", "Bloody Scary");

        // Just Nasty
        Story.KillQuest(5004, "towersilver", "Bone Creeper");

        // Scavenger Hunt
        Story.KillQuest(5005, "towersilver", "Ghoul");

        // Ghoul Booty
        Story.MapItemQuest(5006, "towersilver", 4374, 5);

        // Flester's Guards
        Story.KillQuest(5007, "towersilver", "Undead Golden Knight");

        // Flester the Silver
        Story.KillQuest(5008, "towersilver", "Flester The Silver");

        // Get my Stuff
        if (!Story.QuestProgression(5009))
        {
            Core.EnsureAccept(5009);
            Core.HuntMonster("towersilver", "Fallen Deathknight", "Chef Ramskull's Apron");
            Core.HuntMonster("towersilver", "Undead Knight", "Chef Ramskull's Hat");
            Core.HuntMonster("towersilver", "Undead Warrior", "Chef Ramskull's Cookbook");
            Core.HuntMonster("towersilver", "Ghoul", "Chef Ramskull's Spatula");
            Core.HuntMonster("towersilver", "Undead Guard", "Chef Ramskull's Skillet");
            Core.EnsureComplete(5009);
        }

        // In the Mix
        Story.KillQuest(5010, "towersilver", "Bloody Scary");

        // They Know We're Coming
        Story.KillQuest(5011, "towergold", "Grim Souldier");

        // Elitist Jerks
        Story.KillQuest(5012, "towergold", "Undead Golden Knight");

        // Loot, Hero!
        Story.MapItemQuest(5013, "towergold", 4375, 5);
        Story.KillQuest(5013, "towergold", "Skullspider");

        // Those Aren't Big Birds, Sweetheart
        Story.KillQuest(5014, "towergold", "Vampire Bat");

        // The Nope Room
        Story.KillQuest(5015, "towergold", "Webbed Ghoul");

        // Arach-NO-phobia
        Story.KillQuest(5016, "towergold", "Bone Widow");

        // Library Infestation
        Story.KillQuest(5017, "towergold", "Book Maggot");

        // Creepy!
        Story.MapItemQuest(5018, "towergold", 4376, 6);
        Story.KillQuest(5018, "towergold", "Bone Creeper");

        // It's Not Like They Need Them Anymore
        Story.MapItemQuest(5019, "towergold", 4377, 3);

        // Advanced Self Defense
        if (!Story.QuestProgression(5020))
        {
            Core.EnsureAccept(5020);
            Core.HuntMonster("towergold", "Undead Knight", "Undead Knight Noogied", 3);
            Core.HuntMonster("towergold", "Undead Guard", "Undead Guard Swirlied", 3);
            Core.EnsureComplete(5020);
        }

        // Take A Mallet To 'Em
        Story.KillQuest(5021, "towergold", "Fallen Emperor Statue");

        // Yurrod the Gold
        Story.KillQuest(5022, "towergold", "Yurrod the Gold");
    }

    #endregion

    #region Xeven  - The Time Assassin

    public void ParadoxPortal()
    {
        if (Core.isCompletedBefore(5050))
            return;

        if (doAll)
            Core.Logger("Xeven - The Time Assassin: Paradox Portal");
        Story.PreLoad(this);

        // Through the Portal!
        Story.KillQuest(5034, "portalmaze", "Time Wraith");

        // Red Alert
        Story.KillQuest(5035, "portalmaze", "Heavy Lab Guard");

        // Bugs... They Give Me Hives
        Story.KillQuest(5036, "portalmaze", "Hatchling");

        // Through the Delta V Portal!
        Story.KillQuest(5037, "portalmaze", "Time Wraith");

        // Hwoounga? Unf!
        Story.KillQuest(5038, "portalmaze", "Jurassic Monkey");

        // They're Firin' Their LAZORS
        Story.KillQuest(5039, "portalmaze", "Lazor Dino");

        // Through the Jurassic Portal!
        Story.KillQuest(5040, "portalmaze", "Time Wraith");

        // Where's Twilly?!?
        Story.MapItemQuest(5041, "portalmaze", new[] { 4408, 4409, 4410 });

        // Through the Yulgar Portal!
        Story.KillQuest(5042, "portalmaze", "Time Wraith");

        // Ode to the Walkers
        Story.KillQuest(5043, "portalmaze", "Bucket Zombie");

        // Sonnet of the Undead
        if (!Story.QuestProgression(5044))
        {
            Core.EnsureAccept(5044);
            Core.HuntMonster("portalmaze", "Bucket Zombie", "Rain Bucket", 1);
            Core.HuntMonster("portalmaze", "Dancing Zombie", "Coral's Sunglasses", 1);
            Core.HuntMonster("portalmaze", "Tunneling Zombie", "Grick's Pickaxe", 1);
            Core.EnsureComplete(5044);
        }

        // Through the Zombie Portal!
        Story.KillQuest(5045, "portalmaze", "Time Wraith");

        // Your Lips Are Sealed
        Story.KillQuest(5046, "portalmaze", "Pactagonal Knight");

        // Through the Swordhaven Portal!
        Story.KillQuest(5047, "portalmaze", "Time Wraith");

        // Escape from the ChronoLords
        Story.KillQuest(5048, "portalmaze", "ChronoLord");

        // Vorefax
        Story.KillQuest(5049, "portalmaze", "Vorefax ");

        // The Death of Time
        Story.KillQuest(5050, "portalmaze", "Mors Temporis");
    }

    public void DeepSpace()
    {
        if (!Core.IsMember || Core.isCompletedBefore(5083))
            return;

        if (doAll)
            Core.Logger("Xeven - The Time Assassin: Paradox Portal");
        Story.PreLoad(this);

        ParadoxPortal();

        // The Western Portal
        Story.MapItemQuest(5068, "tachyon", 4446);
        Story.KillQuest(5068, "tachyon", new[] { "Time Wraith", "Timestream Rider" });

        // This Thing Needs A Bigger Battery
        Story.KillQuest(5069, "tachyon", "Spacetime Anomaly");

        // Deserted
        Story.MapItemQuest(5072, "tachyon", 4447, 4);
        Story.KillQuest(5072, "tachyon", new[] { "Sandshark", "Bupers Camel" });

        // Getting Sand In Your Boots
        Story.MapItemQuest(5073, "tachyon", 4448);

        // The Eastern Portal
        Story.MapItemQuest(5074, "tachyon", 4449);
        Story.KillQuest(5074, "tachyon", new[] { "Time Wraith", "Timestream Rider" });

        // Time for a Tune-Up
        Story.KillQuest(5075, "tachyon", "Medusoid");

        // Time Void Archaeology
        Story.MapItemQuest(5076, "tachyon", 4450, 3);
        Story.MapItemQuest(5076, "tachyon", 4451, 3);
        Story.KillQuest(5076, "tachyon", new[] { "Jungle Tog", "Jungle Fury" });

        // Going With the Flow
        Story.MapItemQuest(5077, "tachyon", 4452);

        // The Central Portal
        Story.MapItemQuest(5078, "tachyon", 4453);
        Story.KillQuest(5078, "tachyon", new[] { "Time Wraith", "Timestream Rider" });

        // Is There Tech Support For This Thing?
        Story.KillQuest(5079, "tachyon", "Void Serpent");

        // Shovelling Snow
        Story.MapItemQuest(5080, "tachyon", 4454, 5);
        Story.KillQuest(5080, "tachyon", new[] { "Ice Wolf", "Polar Elemental" });

        // Winter Winds
        Story.MapItemQuest(5081, "tachyon", 4455);

        // Opening the Megaportal
        Story.MapItemQuest(5082, "tachyon", 4456);

        // Svelgr the Devourer
        Story.KillQuest(5083, "tachyon", "Svelgr the Devourer");
    }

    #endregion

    #region Ziri - The Daimon

    public void BaconCatFortress()
    {
        if (Core.isCompletedBefore(5108))
            return;

        if (doAll)
            Core.Logger("Ziri - The Daimon: BaconCat Fortress");
        Story.PreLoad(this);

        // How Rude!
        Story.MapItemQuest(5087, "baconcat", 4466, 7);

        // Bar Fight!
        Story.KillQuest(5088, "baconcat", "Yulgar Regular");

        // Number Two
        Story.MapItemQuest(5089, "baconcat", 4467, 1);
        Story.KillQuest(5089, "baconcat", "Yulgar Regular");

        // Forget the Mess
        Story.KillQuest(5090, "baconcat", "Slime");

        // The Chosen One!
        Story.MapItemQuest(5091, "baconcat", 4473, 1);

        // BACON PIZZA!
        Story.KillQuest(5092, "baconcat", new[] { "Baconcatzard", "Pizzacatzard" });

        // No More Clowns!
        if (!Story.QuestProgression(5093))
        {
            Core.EnsureAccept(5093);
            Core.HuntMonsterMapID("baconcat", 26, "Scary Face Paint!", 10);
            Core.HuntMonsterMapID("baconcat", 26, "Honking Clown Nose", 3);
            Core.HuntMonsterMapID("baconcat", 26, "Rainbow Wig", 3);
            Core.EnsureComplete(5093);
        }

        // Life's a Beach
        Story.MapItemQuest(5094, "baconcat", 4468, 9);

        // Not all Sand is Cat Litter
        Story.KillQuest(5095, "baconcat", new[] { "Fart Elemental", "Litter Elemental" });

        // King Strong
        Story.KillQuest(5096, "baconcat", new[] { "King Strong", "Box", "King Strong" });

        // Snack Man!
        Story.MapItemQuest(5097, "baconcat", 4469, 4);

        // Ghost Busting
        if (!Story.QuestProgression(5098))
        {
            Core.EnsureAccept(5098);
            Core.Join("baconcat", "r11a", "Left");
            Core.Logger("Cutscene may have appeared... `Hunt` wil resume soon, please be patient.");
            Bot.Wait.ForCellChange("Cut1");
            Core.Sleep(2500);
            Core.HuntMonsterMapID("baconcat", 41, "Oopy Defeated");
            Core.HuntMonsterMapID("baconcat", 40, "Bloopy Defeated");
            Core.HuntMonsterMapID("baconcat", 42, "Hoopy Defeated");
            Core.HuntMonsterMapID("baconcat", 43, "Frood Defeated");
            Core.EnsureComplete(5098);
        }

        // Super Ziri Brothers
        if (!Story.QuestProgression(5099))
        {
            Core.EnsureAccept(5099);
            Core.KillMonster("baconcat", "r12", "Left", "Red Shell Turtle", "Turtle Shells", 5);
            Core.KillMonster("baconcat", "r12", "Left", "Snapper Shrub", "Power Flower", 3);
            Core.EnsureComplete(5099);
        }

        // It's-A-Me
        Story.KillQuest(5100, "baconcat", "Horcio");

        // Me So Corny!
        Story.KillQuest(5109, "baconcat", "Corn Minion");

        // Me Nom You Long Time
        Story.KillQuest(5110, "baconcat", "Non-GMO Brutalcorn");

        // Make a Wish
        Story.MapItemQuest(5101, "baconcat", 4470, 1);

        // Smell This!
        Story.KillQuest(5102, "baconcat", "Scent Trail");

        // Trial of being SMALL
        Story.KillQuest(5103, "baconcat", new[] { "Buttermancer", "Potato Knight" });

        // King of the Unbread
        Story.KillQuest(5104, "baconcat", "King of the Unbread");

        // Evil Undead!
        Story.KillQuest(5105, "baconcat", "Chainsaw Actor");

        // Pala-dinner
        Story.KillQuest(5106, "baconcat", "Paladin Actor");

        // Kitty Boo Boo, Overlord of the Catverse
        Story.KillQuest(5107, "baconcat", "Kitty Boo Boo");

        // Stop Hitting Yourself!
        if (!Story.QuestProgression(5108))
        {
            Core.EnsureAccept(5108);
            Core.KillMonster("baconcatyou", "Enter", "Spawn", "*", "Defeated YOURSELF!");
            Core.EnsureComplete(5108);
        }
    }

    public void LaserSharkInvasion()
    {
        if (Core.isCompletedBefore(5120))
            return;

        if (doAll)
            Core.Logger("Ziri - The Daimon: Laser Shark Invasion");
        Story.PreLoad(this);

        // Cloud Sharks!
        Story.KillQuest(5111, "baconcatlair", "Cloud Shark");

        // Get Those Waffle Cones Ready
        Story.KillQuest(5112, "baconcatlair", "Ice Cream Shark");

        //Grody
        if (!Story.QuestProgression(5113))
        {
            Core.EnsureAccept(5113);
            Core.GetMapItem(4474, 6, "baconcatlair");
            Core.HuntMonster("baconcatlair", "Ice Cream Shark", "Big Cherry", 4);
            Core.EnsureComplete(5113);
        }

        // We're Gonna Need A Bigger Eraser
        if (!Story.QuestProgression(5114))
        {
            Core.EnsureAccept(5114);
            Core.BuyItem("librarium", 651, "Really Big Pencil");
            Core.EnsureComplete(5114);
        }

        // Second Draft
        if (!Story.QuestProgression(5115))
        {
            Core.EnsureAccept(5115);
            Core.GetMapItem(4475, 4, "baconcatlair");
            Core.HuntMonster("baconcatlair", "Sketchy Shark", "Shark Erased", 6);
            Core.EnsureComplete(5115);
        }

        // Game on!
        if (!Story.QuestProgression(5116))
        {
            Core.EnsureAccept(5116);
            Core.KillMonster("baconcat", "r12", "Left", "*", "Cheat Codes", 4);
            Core.EnsureComplete(5116);
        }

        // Game Sharks
        if (!Story.QuestProgression(5117))
        {
            Core.EnsureAccept(5117);
            Core.GetMapItem(4476, 4, "baconcatlair");
            Core.HuntMonster("baconcatlair", "8-Bit Shark", "Snot Kicked", 6);
            Core.EnsureComplete(5117);
        }

        // Save the Kittarians
        if (!Story.QuestProgression(5118))
        {
            Core.EnsureAccept(5118);
            Core.HuntMonster("baconcatlair", "Cat Clothed Shark", "Kittarian Clothes", 6);
            Core.HuntMonsterMapID("baconcatlair", 14, "Kittarian Spoon", 4);
            Core.HuntMonsterMapID("baconcatlair", 13, "Kittarian Fork", 4);
            Core.EnsureComplete(5118);
        }

        // Bacon Cat Force Needs YOU!
        if (!Story.QuestProgression(5119))
        {
            Core.EnsureAccept(5119);
            Core.HuntMonster("baconcatlair", "Cloud Shark", "Cloud Shark Farts", 3);
            Core.HuntMonster("baconcatlair", "Ice Cream Shark", "Shark Sprinkles", 3);
            Core.HuntMonster("baconcatlair", "Sketchy Shark", "College-Ruled Paper", 3);
            Core.HuntMonster("baconcatlair", "8-Bit Shark", "Great White DLC", 3);
            Core.HuntMonster("baconcatlair", "Cat Clothed Shark", "Kittarian Costumes", 3);
            Core.EnsureComplete(5119);
        }

        // Ziri Is Also Tough
        Story.KillQuest(5120, "baconcatlair", "Cloud Shark");

        if (Core.IsMember)
        {
            //Robo Sharks! 5121
            Story.KillQuest(5121, "baconcatlair", "Robo Shark");

            //Beat the Bushes 5122
            Story.MapItemQuest(5122, "baconcatlair", 4477, 4);

            //Sharks Should Not Walk 5123
            if (!Story.QuestProgression(5123))
            {
                Core.EnsureAccept(5123);
                Core.HuntMonster("baconcatlair", "Robo Shark", "Walking Shark 2 Destroyed", 4);
                Core.HuntMonster("baconcatlair", "Robo Shark", "Walking Shark 1 Destroyed", 4);
                Core.EnsureComplete(5123);
            }

            //Grab those Quarters 5124
            Story.KillQuest(5124, "baconcatlair", "Robo Shark");

            //Shark Invaders! 5125
            if (!Story.QuestProgression(5125))
            {
                Core.EnsureAccept(5125);
                Core.HuntMonster("baconcatlair", "Shark Invader", "Hi Score", 100);
                Core.HuntMonster("baconcatlair", "Shark Invader", "BONUS!");
                Core.EnsureComplete(5125);
            }

            //Sharks like Honey? 5126
            Story.MapItemQuest(5126, "baconcatlair", 4480, 6);
            Story.KillQuest(5126, "baconcatlair", "Robo Shark");

            //At Least They Like to Party 5127
            Story.MapItemQuest(5127, "baconcatlair", 4479, 3);
            Story.KillQuest(5127, "baconcatlair", new[] { "Party Shark", "Party Shark" });

            //Remora Brigade 5128
            Story.KillQuest(5128, "baconcatlair", "Laser Remora");

            //Into the Whirlpool 5129
            Story.MapItemQuest(5129, "baconcatlair", 4478);

            //Giant, Huge Cyborg Shark! 5130
            Story.KillQuest(5130, "baconcatlair", "Cyborg Laser Shark");

            //Wheel of Bacon 5131
            if (!Story.QuestProgression(5131))
            {
                Core.EnsureAccept(5131);
                Core.HuntMonster("baconcatlair", "Robo Shark", "Wheel of Bacon Token", 5, isTemp: false);
                Core.EnsureComplete(5131);
            }
        }
    }

    #endregion

    #region Pax - The Warlord

    public void DeathPit()
    {
        if (Core.isCompletedBefore(5154))
            return;

        if (doAll)
            Core.Logger("Pax - The Warlord: Death Pit");
        Story.PreLoad(this);

        // Mingle
        Story.MapItemQuest(
            5133,
            "DeathPit",
            new[] { 4484, 4485, 4486, 4487, 4488, 4489, 4490, 4491 }
        );

        // Those Dummies
        Story.KillQuest(5134, "DeathPit", "Training Dummy");

        // Round 1: You vs Omar the Meek!
        Story.KillQuest(5135, "DeathPit", "Omar the Meek");

        // Round 2: You vs a Bunch of Sneevils!
        Story.KillQuest(5136, "DeathPit", "Sneevil");

        // Battle: You vs Hattori!
        Story.KillQuest(5137, "DeathPit", "Hattori");

        // Battle: You vs the Slime Horde!
        if (!Story.QuestProgression(5138))
        {
            Core.EnsureAccept(5138);
            Core.HuntMonster("deathpit", "Giant Green Slime", "Giant Slime Defeated", 1);
            Core.HuntMonster("deathpit", "Slime", "Slime Defeated", 4);
            Core.EnsureComplete(5138);
        }

        // Battle: You vs the Sludge Lord!
        Story.KillQuest(5139, "DeathPit", "Sludgelord");

        // Battle: You vs Some Salamanders!
        Story.KillQuest(5140, "DeathPit", "Salamander");

        // Battle: You vs a Trobble!
        Story.KillQuest(5141, "DeathPit", "Trobble");

        // Trouble Battle
        Story.KillQuest(5142, "DeathPit", "Trobble");

        // Horc Battle
        Story.KillQuest(5143, "DeathPit", "Horc Gladiator");

        // Battle: You vs the Brawlers!
        Story.KillQuest(5144, "DeathPit", "Drakel Brawler");

        // Battle: You vs the Gladiators!
        Story.KillQuest(5145, "DeathPit", "Drakel Gladiator");

        // Battle: You vs the Battlemasters!
        Story.KillQuest(5146, "DeathPit", "Drakel Battlemaster");

        // Battle: You vs General Gall!
        Story.KillQuest(5147, "DeathPit", "General Gall");

        // Velm's Minions
        Story.KillQuest(5148, "DeathPit", "Drakel Brawler");

        // eneral Velm
        Story.KillQuest(5149, "DeathPit", "General Velm");

        // Chud's Minions
        Story.KillQuest(5150, "DeathPit", "Drakel Battlemaster");

        // General Chud
        Story.KillQuest(5151, "DeathPit", "General Chud");

        // Hun'Gar's Minions
        Story.KillQuest(5152, "DeathPit", "Drakel Battlemaster");

        // Hun'Gar Defeated
        Story.KillQuest(5153, "DeathPit", "General Hun'Gar");

        // Pax Defeated
        Story.KillQuest(5154, "DeathPit", "Warlord Pax");
    }

    public void DeathPitPVP()
    {
        if (Core.isCompletedBefore(5165))
            return;

        if (doAll)
            Core.Logger("Pax - The Warlord: Death Pit");
        Story.PreLoad(this);

        DeathPit();

        // Do You Even Brawl
        if (!Story.QuestProgression(5155))
        {
            Core.EnsureAccept(5155);
            Farm.DeathPitToken(quant: 1, ForStory: true);
            Core.EnsureComplete(5155);
        }
        // Flex For Hun'Gar
        if (!Story.QuestProgression(5156))
        {
            Core.EnsureAccept(5156);
            Farm.DeathPitToken(quant: 15, ForStory: true);
            Core.EnsureComplete(5156);
        }

        // Pummel For Hun'Gar
        if (!Story.QuestProgression(5157))
        {
            Core.EnsureAccept(5157);
            Farm.DeathPitToken("Brawler Token", 3, true, true);
            Farm.DeathPitToken("Restorer Token", 4, true, true);
            Core.EnsureComplete(5157);
        }

        // Destroy For Hun'Gar
        if (!Story.QuestProgression(5165))
        {
            Core.EnsureAccept(5165);
            Farm.DeathPitToken("Death Pit Victory (Captain Defeat)", 1, true, true);
            Core.EnsureComplete(5165);
        }
    }

    #endregion

    #region Sekt - The Eternal

    public void ShiftingPyramid()
    {
        if (Core.isCompletedBefore(5187))
            return;

        if (doAll)
            Core.Logger("Sekt - The Eternal: Shifting Pyramid");
        Story.PreLoad(this);

        // Hunt for the Infinity Codex
        if (!Story.QuestProgression(5166))
        {
            Core.EnsureAccept(5166);
            Core.KillMonster("whitehole", "r2", "Left", "Vortex Mage", "Infinity Text Page", 8);
            Core.EnsureComplete(5166);
        }

        // Sacrific and Survival
        if (!Story.QuestProgression(5167))
        {
            Core.EnsureAccept(5167);
            Core.HuntMonster("whitehole", "Vortex Naga", "Naga Slain", 3);
            Core.HuntMonster("whitehole", "Vortex Hawk", "Hawk Slain", 3);
            Core.EnsureComplete(5167);
        }

        // The Cartouche of Isis
        if (!Story.QuestProgression(5168))
        {
            Core.EnsureAccept(5168);
            Core.HuntMonster("whitehole", "Gate Goblin", "Balm Cartouche");
            Core.EnsureComplete(5168);
        }

        // The Cartouche of Ma'at
        if (!Story.QuestProgression(5169))
        {
            Core.EnsureAccept(5169);
            Core.HuntMonster("whitehole", "Vortex Walker", "Scale Cartouche Fragment", 5);
            Core.EnsureComplete(5169);
        }

        // Bound to Do Good
        if (!Story.QuestProgression(5170))
        {
            Core.EnsureAccept(5170);
            Core.HuntMonster("whitehole", "Dimensional Crystal", "Quartz", 3);
            Core.HuntMonster("whitehole", "Gate Goblin", "Lime", 2);
            Core.HuntMonster("whitehole", "Vortex Matter", "Natron");
            Core.EnsureComplete(5170);
        }

        // Honor the Goddess Isis
        Story.MapItemQuest(5171, "whitehole", 4539);

        // Stick to the Task
        if (!Story.QuestProgression(5172))
        {
            Core.EnsureAccept(5172);
            Core.KillMonster("whitehole", "r3", "Left", "Dimensional Crystal", "Quartz", 4);
            Core.KillMonster("whitehole", "r6", "Left", "Gate Goblin", "Lime", 1);
            Core.KillMonster("whitehole", "r10", "Left", "Vortex Matter", "Ash", 3);
            Core.EnsureComplete(5172);
        }

        // Honor the Goddess Ma'at
        Story.MapItemQuest(5173, "whitehole", 4540, 4);
        Story.MapItemQuest(5173, "whitehole", 4542);

        // Duty is light as a feather
        if (!Story.QuestProgression(5174))
        {
            Core.EnsureAccept(5174);
            Core.HuntMonster("whitehole", "Vortex Hawk", "Feather of Ma'at");
            Core.EnsureComplete(5174);
        }

        // Judgement... or Justice?
        if (!Story.QuestProgression(5175))
        {
            Core.EnsureAccept(5175);
            Core.HuntMonster("whitehole", "Hand of Ma'at", "Judgement Passed");
            Core.EnsureComplete(5175);
        }

        // The Cartouche of Thoth
        if (!Story.QuestProgression(5176))
        {
            Core.EnsureAccept(5176);
            Core.HuntMonster("whitehole", "Vortex Mage", "Scroll Cartouche");
            Core.EnsureComplete(5176);
        }

        // Make Your Mark
        Story.MapItemQuest(5177, "whitehole", 4541, 4);

        // The Cartouche of Kebechet
        if (!Story.QuestProgression(5178))
        {
            Core.EnsureAccept(5178);
            Core.HuntMonster("whitehole", "Vortex Naga", "Cartouche Fragment", 6);
            Core.EnsureComplete(5178);
        }

        // Honor the Goddess Kebechet
        Story.MapItemQuest(5179, "whitehole", 4543);

        // Destroy to Purify
        if (!Story.QuestProgression(5180))
        {
            Core.EnsureAccept(5180);
            Core.HuntMonster("whitehole", "Vortex Crystal", "Natron", 5);
            Core.EnsureComplete(5180);
        }

        // Guardian of the Vortex
        if (!Story.QuestProgression(5181))
        {
            Core.EnsureAccept(5181);
            Core.HuntMonster("whitehole", "Vortex Guardian", "Guardian Slain");
            Core.EnsureComplete(5181);
        }

        // Stick with it
        if (!Story.QuestProgression(5182))
        {
            Core.EnsureAccept(5182);
            Core.HuntMonster("whitehole", "Dimensional Crystal", "Quartz", 2);
            Core.HuntMonster("whitehole", "Gate Goblin", "Lime", 4);
            Core.HuntMonster("whitehole", "Vortex Matter", "Ash", 1);
            Core.EnsureComplete(5182);
        }

        // Honor the God Thoth
        Story.MapItemQuest(5183, "whitehole", 4544);

        // The Brightest Cartouches
        if (!Story.QuestProgression(5184))
        {
            Core.EnsureAccept(5184);
            Core.HuntMonster("whitehole", "Dimensional Crystal", "Sun Cartouche");
            Core.HuntMonster("whitehole", "Dimensional Crystal", "Sky Cartouche");
            Core.HuntMonster("whitehole", "Vortex Matter", "Star Cartouche");
            Core.HuntMonster("whitehole", "Vortex Crystal", "Moon Cartouche");
            Core.EnsureComplete(5184);
        }

        // Honor the Astral Deities
        Story.MapItemQuest(5185, "whitehole", 4545, 4);

        // Serpent of the Stars
        if (!Story.QuestProgression(5186))
        {
            Core.EnsureAccept(5186);
            Core.HuntMonster("whitehole", "Mehensi Serpent", "Mehen Slain");
            Core.EnsureComplete(5186);
        }

        // The Infinity Shield
        Story.MapItemQuest(5187, "whitehole", 4546);
    }

    public void FourthDimensionalPyramid()
    {
        if (Core.isCompletedBefore(5212))
            return;

        if (doAll)
            Core.Logger("Sekt - The Eternal: Fourth Dimensional Pyramid");
        Story.PreLoad(this);

        // Eye for an Eye of the Old Gods
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5189, "fourdpyramid", "Sekt");

        // Hounded by History
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(5190, "fourdpyramid", "Negastri Hound");

        // Stand and De-Lever
        Story.MapItemQuest(5191, "fourdpyramid", 4556, 1);

        // Yo Mummy
        Core.EquipClass(ClassType.Solo);
        if (!Story.QuestProgression(5192))
        {
            Core.EnsureAccept(5192);
            Core.GetMapItem(4557, 1, "fourdpyramid");
            Core.HuntMonster("fourdpyramid", "Sekt's Mummy", "Mummy Grease", 1);
            Core.EnsureComplete(5192);
        }

        // Find the Secret Brick
        Story.MapItemQuest(5193, "fourdpyramid", 4558, 1);

        // Gauze in 60 Seconds
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(5194, "fourdpyramid", "Nega Mummy");

        // De-Lever-ence
        Story.MapItemQuest(5195, "fourdpyramid", 4559, 1);

        // A Jarring Solution
        if (!Story.QuestProgression(5196))
        {
            Core.EnsureAccept(5196);
            Core.GetMapItem(4560, 1, "fourdpyramid");
            Core.HuntMonster("fourdpyramid", "Nega Mummy", "Canopic Jar", 1);
            Core.HuntMonster("fourdpyramid", "Guardian of Anubyx", "Tesseract Energy", 5);
            Core.EnsureComplete(5196);
        }

        // Ra of Light
        Story.MapItemQuest(5197, "fourdpyramid", 4561, 1);

        // Sphynxes are Riddled With Gems
        if (!Story.QuestProgression(5198))
        {
            Core.EnsureAccept(5198);
            Core.HuntMonsterMapID("fourdpyramid", 19, "White Gem", 2);
            Core.HuntMonsterMapID("fourdpyramid", 20, "Black Gem", 2);
            Core.EnsureComplete(5198);
        }

        // 4th Dimensional Teleport
        Story.MapItemQuest(5199, "fourdpyramid", 4562, 4);
        Story.MapItemQuest(5199, "fourdpyramid", 4564, 1);

        // Mummies vs Daddies
        Story.KillQuest(5200, "fourdpyramid", "Nega Mummy");

        // The Tesseract
        Story.KillQuest(5201, "fourdpyramid", "Guardian of Anubyx");

        // Another Mystery To Solve
        Story.MapItemQuest(5202, "fourdpyramid", 4565, 1);

        // Whose Skeleton Is This?
        Story.MapItemQuest(5203, "fourdpyramid", 4566, 1);

        // Fourth Scrap of the Propechy
        Story.MapItemQuest(5204, "fourdpyramid", 4567, 1);

        // Fighting in 4D
        if (!Story.QuestProgression(5205))
        {
            Core.EnsureAccept(5205);
            Core.GetMapItem(4568, 1, "fourdpyramid");
            Core.HuntMonster("fourdpyramid", "Tesseract Sprite", "Tesseract Sprites", 8);
            Core.EnsureComplete(5205);
        }

        // Lever-age
        Story.MapItemQuest(5206, "fourdpyramid", 4569, 1);

        // 4D Goblins?
        if (!Story.QuestProgression(5207))
        {
            Core.EnsureAccept(5207);
            Core.GetMapItem(4570, 1, "fourdpyramid");
            Core.HuntMonster("fourdpyramid", "Tesseract Goblin", "Goblin Tools", 4);
            Core.EnsureComplete(5207);
        }

        // Stone Sphynx Gems
        if (!Story.QuestProgression(5208))
        {
            Core.EnsureAccept(5208);
            Core.HuntMonsterMapID("fourdpyramid", 19, "White Gem", 1);
            Core.HuntMonsterMapID("fourdpyramid", 20, "Black Gem", 3);
            Core.EnsureComplete(5208);
        }

        // Beam Me Up Scotty;
        Story.MapItemQuest(5209, "fourdpyramid", 4571, 4);
        Story.MapItemQuest(5209, "fourdpyramid", 4572, 1);

        // Sekt... Again
        Story.MapItemQuest(5210, "fourdpyramid", 4573, 1);

        // The Black Plague
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5211, "fourdpyramid", "Black Plague");

        // The Hero's Doom
        Story.MapItemQuest(5212, "fourdpyramid", 4574, 1);
    }

    public void Yasaris()
    {
        if (Core.isCompletedBefore(5239))
            return;

        Story.PreLoad(this);

        //Serepthys, Please 5213
        Story.MapItemQuest(5213, "yasaris", 4590);

        //The Hall of the Falcon 5214
        Story.MapItemQuest(5214, "yasaris", 4576);

        //Secrets and Whispers 5215
        Story.KillQuest(5215, "yasaris", "Vortex Hawk");

        //The Venom is the Key 5216
        Story.KillQuest(5216, "yasaris", "Sacred Serpent");

        //The Venom Is Your Wine 5217
        Story.MapItemQuest(5217, "yasaris", 4577);

        //Another Interpretation 5218
        Story.MapItemQuest(5218, "yasaris", 4578);

        //Well, Go Get the Sac, Then! 5219
        if (!Bot.Quests.IsUnlocked(5220) || !Core.CheckInventory("Preserved Venom Sac of Mehensi"))
        {
            Core.AddDrop("Preserved Venom", "Preserved Venom Sac of Mehensi");
            Core.EnsureAccept(5219);
            Core.GetMapItem(4579, 1, "yasaris");
            Bot.Wait.ForPickup("Preserved Venom");
            Core.EnsureComplete(5219);
            Bot.Wait.ForPickup("Preserved Venom Sac of Mehensi");
        }

        //An Offering to the Falcon Guardian 5240
        Story.ChainQuest(5240);

        //The Hall of the Jackal 5220
        Story.MapItemQuest(5220, "yasaris", 4591);

        //Cross the Chasm 5221
        if (!Story.QuestProgression(5221))
        {
            Core.EnsureAccept(5221);
            Core.HuntMonster("yasaris", "Avatar of Anubyx", "Baboon, Jackal", 1);
            Core.HuntMonster("yasaris", "Inverted Avatar", "Falcon, Human", 1);
            Core.EnsureComplete(5221);
        }

        //Enter the Code 5222
        Story.MapItemQuest(5222, "yasaris", 4580);

        //Look for the Offering 5223
        Story.MapItemQuest(5223, "yasaris", 4581);

        //Needs More Energy 5224
        if (!Story.QuestProgression(5224))
        {
            Core.EnsureAccept(5224);
            Core.HuntMonster("yasaris", "Avatar of Anubyx", "Energy Vessel", 1);
            Core.HuntMonster("yasaris", "Negastri Hound", "Tesseract Energy", 5);
            Core.EnsureComplete(5224);
        }

        //Charge the Spear 5225
        Story.MapItemQuest(5225, "yasaris", 4582);

        //Take the Offering 5226
        if (!Core.CheckInventory("Charged Spear of Anubyx"))
        {
            Core.AddDrop("Get Charged Spear of Anubyx", "Charged Spear of Anubyx");
            Core.EnsureAccept(5226);
            Core.GetMapItem(4583, 1, "yasaris");
            Core.EnsureComplete(5226);
            Bot.Wait.ForPickup("Get Charged Spear of Anubyx");
            Bot.Wait.ForPickup("Charged Spear of Anubyx");
        }

        //An Offering to the Jackal Guardian 5241
        Story.ChainQuest(5241);

        //The Hall of the Baboon 5227
        Story.MapItemQuest(5227, "yasaris", 4592);

        //Phase In 5228
        Story.KillQuest(5228, "yasaris", "Dimensional Crystal");

        //What Will the Crystals do? 5229
        Story.MapItemQuest(5229, "yasaris", 4584);

        //Charge those Platforms 5230
        if (!Story.QuestProgression(5230))
        {
            Core.EnsureAccept(5230);
            Core.GetMapItem(4585, 1, "yasaris");
            Core.HuntMonster("yasaris", "Tesseract Sprite", "Tesseract Energy", 8);
            Core.EnsureComplete(5230);
        }

        //Explore the Secret Room 5231
        Story.MapItemQuest(5231, "yasaris", 4593);

        //Get More Crystals 5232
        Story.KillQuest(5232, "yasaris", "Dimensional Crystal");

        //Charge the Pedestal 5233
        if (!Bot.Quests.IsUnlocked(5234) || !Core.CheckInventory("Crystal of Hatoth"))
        {
            Core.AddDrop("Crystal of Hatoth");
            Core.EnsureAccept(5233);
            Core.GetMapItem(4595, 1, "yasaris");
            Core.GetMapItem(4586, 1, "yasaris");
            Core.EnsureComplete(5233);
            Bot.Wait.ForPickup("Crystal of Hatoth");
        }

        //An Offering to the Baboon Guardian 5242
        Story.ChainQuest(5242);

        //The Hall of Humanity 5234
        Story.MapItemQuest(5234, "yasaris", 4594);

        //Activate the Levers 5235
        if (!Story.QuestProgression(5235))
        {
            Core.EnsureAccept(5235);
            Core.GetMapItem(4587, 4, "yasaris");
            Core.HuntMonster("yasaris", "Avatar of Serepthys", "Gem of Serepthys", 4);
            Core.EnsureComplete(5235);
        }

        //Pull the Levers 5236
        Story.MapItemQuest(5236, "yasaris", 4588, 4);

        //Reveal the Paths 5237
        Story.KillQuest(5237, "yasaris", "Spirit of Ptahmun");

        //Find the Spirit Knife 5238
        Story.MapItemQuest(5238, "yasaris", 4589);

        if (!Bot.Quests.IsUnlocked(5239) || !Core.CheckInventory("Heart of Ptahmun"))
        {
            Core.AddDrop("Heart of Ptahmun");
            Core.EnsureAccept(5238);
            Core.GetMapItem(4589, 1, "yasaris");
            Core.EnsureComplete(5238);
            Bot.Wait.ForPickup("Heart of Ptahmun");
        }

        //An Offering to the Human Guardian 5243
        Story.ChainQuest(5243);

        //Battle Serepthys 5239
        Story.KillQuest(5239, "yasaris", "Serepthys");
    }
    #endregion

    #region Scarletta - The Sorceress

    public void ShatterGlassMaze()
    {
        if (Core.isCompletedBefore(5313))
            return;

        if (doAll)
            Core.Logger("Scarletta - The Sorceress: ShatterGlass Maze");
        Story.PreLoad(this);

        if (!Story.QuestProgression(5298))
        {
            Core.EnsureAccept(5298);
            Core.GetMapItem(4678, 1, "hedgemaze");

            Core.HuntMonster("hedgemaze", "Knight's Reflection", "Knight's Reflection Dispersed", 1);
            Core.EnsureComplete(5298);
        }

        Core.EquipClass(ClassType.Solo);
        if (!Story.QuestProgression(5299))
        {
            Core.EnsureAccept(5299);
            Core.HuntMonster("hedgemaze", "Mirrored Shard", "Mirror Shard Slain", 4);
            Core.HuntMonsterMapID("hedgemaze", 48, "Hedge Goblin Slain", 4);
            Core.HuntMonsterMapID("hedgemaze", 20, "Minotaur Slain", 3);
            Story.MapItemQuest(5299, "hedgemaze", 4679);
        }

        Story.KillQuest(5300, "hedgemaze", "Knight's Reflection");

        Story.MapItemQuest(5301, "hedgemaze", 4680);

        Story.KillQuest(5302, "hedgemaze", "Hedge Goblin");

        Story.MapItemQuest(5303, "hedgemaze", 4681, 12);

        Story.KillQuest(5304, "hedgemaze", "Mirrored Shard");

        Story.MapItemQuest(5305, "hedgemaze", 4682);

        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5306, "hedgemaze", "Minotaur Prime");

        Story.MapItemQuest(5307, "hedgemaze", 4683);

        Story.MapItemQuest(5308, "hedgemaze", 4684);

        Story.MapItemQuest(5309, "hedgemaze", 4685, 5);

        if (!Story.QuestProgression(5310))
        {
            Core.EnsureAccept(5310);
            Core.KillMonster("hedgemaze", "r21", "Right", "*", "Maze Monster Killed", 8);
            Core.EnsureComplete(5310);
        }

        Story.MapItemQuest(5311, "hedgemaze", 4686);

        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5312, "hedgemaze", "Shattered Knight");

        Story.KillQuest(5313, "hedgemaze", "Resurrected Minotaur");
    }

    public void TowerofMirrors()
    {
        if (Core.isCompletedBefore(5332))
            return;

        Core.Logger($"{Core.SoloClass}, {Core.FarmClass}");

        ShatterGlassMaze();

        if (doAll)
            Core.Logger("Scarletta - The Sorceress: Tower of Mirrors");
        Story.PreLoad(this);

        // Drink Me
        if (!Story.QuestProgression(5314))
        {
            Core.EnsureAccept(5314);
            Core.HuntMonster("towerofmirrors", "Glassgoyle", "Glass Powder", 8);
            Core.HuntMonster("towerofmirrors", "Glass Serpent", "Reflective Venom", 4);
            Core.EnsureComplete(5314);
        }

        // The Key To Success
        if (!Story.QuestProgression(5315))
        {
            Core.EnsureAccept(5315);
            Core.GetMapItems(new[] { (4691, 1), (4692, 1) }, "towerofmirrors");
            Core.HuntMonster("towerofmirrors", "Silver Elemental", "Mirror Key", 1);
            Core.EnsureComplete(5315);
        }

        // Phanatics
        if (!Story.QuestProgression(5316))
        {
            Core.EnsureAccept(5316);
            Core.HuntMonster("towerofmirrors", "Phans", "Mosh Past the Phans", 8);
            Core.HuntMonster("towerofmirrors", "Phans", "Backstage Pass", 1);
            Core.EnsureComplete(5316);
        }

        // But I Have A Backstage Pass!
        if (!Story.QuestProgression(5317))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5317);
            Core.HuntMonsterMapID("towerofmirrors", 36, "Lathannos the Reversed Defeated");
            Core.EnsureComplete(5317);
        }

        // True Love
        if (!Story.QuestProgression(5318))
        {
            Core.EnsureAccept(5318);
            Core.GetMapItems(new[] { (4687, 1), (4693, 1) }, "towerofmirrors");
            Core.HuntMonster("towerofmirrors", "Silver Elemental", "Andres' Mirror Key", 1);
            Core.EnsureComplete(5318);
        }

        // Turn to the Left
        if (!Story.QuestProgression(5319))
        {
            Core.EnsureAccept(5319);
            Core.HuntMonster("towerofmirrors", "Runway Wraith", "Runway Wraith Defeated", 6);
            Core.HuntMonster("towerofmirrors", "Runway Wraith", "Avant-Garde Footwear", 1);
            Core.HuntMonster("towerofmirrors", "Runway Wraith", "Retro Wig", 1);
            Core.HuntMonster("towerofmirrors", "Runway Wraith", "Someone's Wearable Art Project", 1);
            Core.EnsureComplete(5319);
        }

        // Now Turn to the Right
        if (!Story.QuestProgression(5320))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5320);
            Core.HuntMonsterMapID("towerofmirrors", 40, "Lukrisio the Buffed Defeated");
            Core.EnsureComplete(5320);
        }

        // Or Maybe THIS Is True Love
        if (!Story.QuestProgression(5321))
        {
            Core.EnsureAccept(5321);
            Core.GetMapItems(new[] { (4688, 1), (4694, 1) }, "towerofmirrors");
            Core.HuntMonster("towerofmirrors", "Silver Elemental", "Gisele's Mirror Key", 1);
            Core.EnsureComplete(5321);
        }

        // Those Harpies!
        if (!Story.QuestProgression(5322))
        {
            Core.EnsureAccept(5322);
            Core.HuntMonster("towerofmirrors", "Pageant Mom", "Pageant Mom Defeated", 8);
            Core.HuntMonster("towerofmirrors", "Pageant Mom", "Pageant Sash", 1);
            Core.EnsureComplete(5322);
        }

        // Drink your Go-Go Juice
        if (!Story.QuestProgression(5323))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5323);
            Core.HuntMonsterMapID("towerofmirrors", 45, "Mendeskar the Smudged Defeated");
            Core.EnsureComplete(5323);
        }

        // Oh Sure, Why Not
        if (!Story.QuestProgression(5324))
        {
            Core.EnsureAccept(5324);
            Core.GetMapItems(new[] { (4689, 1), (4695, 1) }, "towerofmirrors");
            Core.HuntMonster("towerofmirrors", "Silver Elemental", "Emilia's Mirror Key", 1);
            Core.EnsureComplete(5324);
        }

        // Behind the Scenes
        if (!Story.QuestProgression(5325))
        {
            Core.EnsureAccept(5325);
            Core.HuntMonster("towerofmirrors", "Stage Tech", "Stage Tech Defeated", 9);
            Core.HuntMonster("towerofmirrors", "Stage Tech", "Rope Ladder");
            Core.EnsureComplete(5325);
        }

        // In the Spotlight
        if (!Story.QuestProgression(5326))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5326);
            Core.HuntMonsterMapID("towerofmirrors", 49, "Atticus the Warped Defeated");
            Core.EnsureComplete(5326);
        }

        // Oh, I Give Up
        if (!Story.QuestProgression(5327))
        {
            Core.EnsureAccept(5327);
            Core.GetMapItems(new[] { (4690, 1), (4696, 1) }, "towerofmirrors");
            Core.HuntMonster("towerofmirrors", "Silver Elemental", "Dora's Mirror Key", 1);
            Core.EnsureComplete(5327);
        }

        // We Gotta Wendi-GO
        if (!Story.QuestProgression(5328))
        {
            Core.EnsureAccept(5328);
            Core.HuntMonster("towerofmirrors", "Sasquatch", "Sasquatches Defeated", 10);
            Core.HuntMonster("towerofmirrors", "Sasquatch", "Tracking Tag", 5);
            Core.EnsureComplete(5328);
        }

        // Kick His- Wait, Did We Already Use That Pun?
        if (!Story.QuestProgression(5329))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5329);
            Core.HuntMonsterMapID("towerofmirrors", 52, "Leofire the Shattered Defeated");
            Core.EnsureComplete(5329);
        }

        // Fiend Zoned
        Story.KillQuest(5330, "towerofmirrors", "Fervent Suitor");

        // Find Scarletta
        Story.MapItemQuest(5331, "towerofmirrors", 4697);

        Bot.Wait.ForCellChange("Cut4");
        while (!Bot.ShouldExit && Bot.Player.Cell != "Enter")
        {
            Core.Jump();
            Core.Sleep();
        }

        // Defeat ... Wait. What?
        if (!Story.QuestProgression(5332))
        {
            Core.AddDrop("Blood Sorceress");
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(5332);
            Core.HuntMonsterMapID("towerofmirrors", 32, "Defeat the Groglurk");
            Core.EnsureComplete(5332);
        }
    }

    #endregion

    #region ??? - The Stranger

    public void AntiqueShop()
    {
        if (Core.isCompletedBefore(5434))
            return;

        if (doAll)
            Core.Logger("??? - The Stranger: Antique Shop");
        Story.PreLoad(this);

        //cursed artifact shop - 5428
        Story.MapItemQuest(5428, "cursedshop", 4803);

        //lamps, painting and chairs, oh my!
        Story.KillQuest(5429, "cursedshop", "Antique Chair");

        //the (un)Dresser
        Story.KillQuest(5430, "cursedshop", "UnDresser");

        //ghost stories
        Story.KillQuest(5431, "cursedshop", "Writing Desk");

        //you can't tell time
        Story.KillQuest(5432, "cursedshop", "Grandfather Clock");

        //Dr. Darkwood's Robe
        Story.MapItemQuest(5433, "cursedshop", new[] { 4804, 4805 });

        //defeat the arcane sentinel
        if (!Story.QuestProgression(5434))
        {
            Core.EnsureAccept(5434);
            Core.GetMapItem(4806, 1, "cursedshop");
            Core.HuntMonster("cursedshop", "Arcane Sentinel", "Darkwood's Robe", 1);
            Core.EnsureComplete(5434);
        }
    }

    public void MysteriousDungeon()
    {
        if (Core.isCompletedBefore(5451))
            return;

        AntiqueShop();

        if (doAll)
            Core.Logger("??? - The Stranger: Mysterious Dungeon");
        Story.PreLoad(this);

        //cursed artifact shop - 5428
        Story.MapItemQuest(5428, "cursedshop", 4803);

        //lamps, painting and chairs, oh my!
        Story.KillQuest(5429, "cursedshop", "Antique Chair");

        //the (un)Dresser
        Story.KillQuest(5430, "cursedshop", "UnDresser");

        //ghost stories
        Story.KillQuest(5431, "cursedshop", "Writing Desk");

        //you can't tell time
        Story.KillQuest(5432, "cursedshop", "Grandfather Clock");

        //Dr. Darkwood's Robe
        Story.MapItemQuest(5433, "cursedshop", new[] { 4804, 4805 });

        //defeat the arcane sentinel
        if (!Story.QuestProgression(5434))
        {
            Core.EnsureAccept(5434);
            Core.GetMapItem(4806, 1, "cursedshop");
            Core.HuntMonster("cursedshop", "Arcane Sentinel", "Darkwood's Robe", 1);
            Core.EnsureComplete(5434);
        }

        //Get Out Of Jail Free?
        Story.MapItemQuest(5438, "MysteriousDungeon", 4818);

        //Sorry, Skudly
        Story.KillQuest(5439, "MysteriousDungeon", "Skudly");

        //A Friend In Need
        Story.MapItemQuest(5440, "MysteriousDungeon", 4808);

        //A Cryptic Messaage
        Story.MapItemQuest(5441, "MysteriousDungeon", 4809);

        //seeking answers
        Story.MapItemQuest(
            5442,
            "MysteriousDungeon",
            new[] { 4810, 4811, 4812, 4813, 4814, 4815, 4816 }
        );

        //Curses!
        Story.MapItemQuest(5443, "MysteriousDungeon", 4817);

        //Skudly, Staaaahp!
        Story.KillQuest(5444, "MysteriousDungeon", "Skudly");

        // Not So Mysterious After All
        Story.KillQuest(5445, "MysteriousDungeon", "Mysterious Stranger");

        // Shut Up And Listen, Vaden!
        Story.KillQuest(5446, "MysteriousDungeon", "Vaden", AutoCompleteQuest: false);

        // Shut Up And Listen, Xeight!
        Story.KillQuest(5447, "MysteriousDungeon", "Xeight", AutoCompleteQuest: false);

        // Shut Up And Listen, Ziri
        Story.KillQuest(5448, "MysteriousDungeon", "Ziri", AutoCompleteQuest: false);

        // Shut Up And Listen, Pax!
        Story.KillQuest(5449, "MysteriousDungeon", "Pax", AutoCompleteQuest: false);

        // Shut Up And Listen, Sekt!
        Story.KillQuest(5450, "MysteriousDungeon", "Sekt", AutoCompleteQuest: false);

        // Shut Up And Listen, Groglurk!
        Story.KillQuest(5451, "MysteriousDungeon", "Scarletta", AutoCompleteQuest: false);
    }

    #endregion
}
