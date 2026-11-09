/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;

public class CoreMogloween
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.RunCore();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        if (!Core.isSeasonalMapActive("mogloween"))
            return;
        Mogloween();
        CandyShop();
        CandyCorn();
        Pie();
        Asylum();
        PoeHub();
        MystcroftForest();
        Chromafection();
        TwigsArcade();
        Masquerade();
        That();
        NecroCarnival();
        TrickTown();
        CursedCastle();
        Eldritchbattletown();
        EldritchWorld();
        ElodeaTemple();
		BirgittaMogloween();
		ShadowSlayerWar();
		Walpurgisnacht();
    }

    public void Mogloween()
    {
        if (!Core.isSeasonalMapActive("mogloween"))
            return;
        if (Core.isCompletedBefore(395))
            return;

        Story.PreLoad(this);
        //Candy Craze 95
        Story.KillQuest(95, "mogloween", "Pumpkinhead Fred");

        //Chocolate Goodness 96
        Story.KillQuest(96, "mogloween", "Pumpkinhead Fred");

        //Chilling Costume 93
        Story.KillQuest(93, "mogloween", "Ghostly Sheet");

        //Candy Basket 94
        Story.KillQuest(94, "mogloween", "Pumpkinhead Fred");

        //Lollipop Potion 97
        Story.KillQuest(97, "mogloween", "Jack-O-Doom");

        //Mystery Candy 98
        if (!Story.QuestProgression(98))
        {
            Core.EnsureAccept(98);
            Core.KillMonster("mogloween", "Pit1", "Right", "Blister", "Mystery Candy", 3);
            Core.EnsureComplete(98);
        }

        //Can't have enough 99
        if (!Story.QuestProgression(99))
        {
            Core.EnsureAccept(99);
            Core.KillMonster("mogloween", "Pit1", "Right", "Blister", "Mystery Candy", 5);
            Core.EnsureComplete(99);
        }

        //Squash the King 391
        Story.KillQuest(391, "mogloween", "Great Pumpkin King");

        //A Bone To Pick 392
        Story.KillQuest(392, "mogloweengrave", "Graveyard Soldier");

        //Were are the Wolves 393
        Story.KillQuest(393, "mogloweengrave", "Graveyard Werewolf");

        //Killer, Chiller, Thriller Here Tonight 394
        Story.KillQuest(394, "mogloweengrave", "Thriller");

        //Candy Shop Cutscene 395
        Story.MapItemQuest(395, "candyshop", 69);
    }

    public void CandyShop()
    {
        if (!Core.isSeasonalMapActive("candyshop"))
            return;
        Mogloween();
        if (Core.isCompletedBefore(401))
            return;

        Story.PreLoad(this);

        //Clear A Path 396
        Story.KillQuest(396, "candyshop", "Dark Moglinster");

        //Kanthalite-D 397
        Story.KillQuest(397, "candyshop", "Sugarrush Ghoul");

        //Myx It Up 398
        Story.KillQuest(398, "candyshop", "Super Moglinster");

        //Candy Conundrum 399
        Story.KillQuest(399, "candyshop", "Dark Moglinster");

        //Boos Clues 400
        if (!Story.QuestProgression(400))
        {
            Core.EnsureAccept(400);
            Core.HuntMonster("voltabolt", "Moglinsterbot", "Fluoride Element");
            Core.HuntMonster("voltabolt", "Moglinsterbot", "Stainless Steel Veneers");
            Core.HuntMonster("voltabolt", "Nightmare Dentist Chair", "Robotic Drill Bit");
            Core.HuntMonster("voltabolt", "Nightmare Dentist Chair", "Fill-up's Screwdriver");
            Core.EnsureComplete(400);
        }

        //What’s Up, Doc? 401
        Story.KillQuest(401, "voltabolt", "Dental Driller");
    }

    public void CandyCorn()
    {
        if (!Core.isSeasonalMapActive("candycorn"))
            return;
        CandyShop();
        if (Core.isCompletedBefore(878))
            return;

        Story.PreLoad(this);

        //Investigate the Farmhouse 871
        Story.MapItemQuest(871, "candycorn", 193);

        //Children of Chaos 872
        Story.KillQuest(872, "candycorn", "Candy Kid");

        //Where is that Barn Key? 873
        Story.KillQuest(873, "candycorn", "Candy Kid");

        //Investigate the Barn 874
        Story.MapItemQuest(874, "candycorn", 194);

        //Malik-EYE is so Grounded 875
        Story.KillQuest(875, "candycorn", "Malik-EYE");

        //Clearing the Candy Corn Field  876
        Story.KillQuest(876, "candycorn", "Field Guardian");

        //So Sick of EYE-Sac 877
        Story.KillQuest(877, "candycorn", "EYE-Sac");

        //She Who Walks Behind The Stalks 878
        Story.KillQuest(878, "candycorn", "Stalkwalker");
    }

    public void Pie()
    {
        if (!Core.isSeasonalMapActive("pie"))
            return;
        CandyShop();
        if (Core.isCompletedBefore(1363))
            return;

        Story.PreLoad(this);

        //Take a Bite Out of Crime 1355
        Story.KillQuest(1355, "pie", "Gourdo");

        //Skullberry Picking 1356
        Story.MapItemQuest(1356, "pie", 654, 10);

        //Can I Axe You a Question 1357
        Story.KillQuest(1357, "pie", "Gourdo");

        //Don't Forge-t 1358
        Story.MapItemQuest(1358, "pie", 655);

        //A Monstrous Appetite 1359
        if (!Story.QuestProgression(1359))
        {
            Core.EnsureAccept(1359);
            Core.HuntMonster("pie", "Gourdo", "Pie Defended", 13);
            Core.HuntMonster("pie", "Gourdo", "Friend Avenged", 2);
            Core.EnsureComplete(1359);
        }

        //Undead End in Sight 1360
        Story.MapItemQuest(1360, "pie", 656);

        //Missing Servant 1361
        Story.MapItemQuest(1361, "pie", 658);

        //Remember Your Quest 1362
        Story.KillQuest(1362, "pie", "Myst Imp");

        //Head to WillowCreek! 1363
        Story.MapItemQuest(1363, "willowcreek", 657);
    }

    public void Asylum()
    {
        if (Core.isCompletedBefore(2466))
            return;

        Story.PreLoad(this);

        //Reflect Upon the Situation 2454
        Story.KillQuest(2454, "mirrormaze", "Insane Ghoul");

        //Escape the Insanity! 2455
        Story.MapItemQuest(2455, "mirrormaze", 1522);

        //Smoke and Mirrors 2456
        Story.MapItemQuest(2456, "mirrormaze", 1523, 6);

        //Shot in the Dark 2457
        Story.KillQuest(2457, "mirrormaze", "Insane Ghoul");

        //Cyser - Oh Where Art Thou? 2458
        Story.MapItemQuest(2458, "mirrormaze", 1524);

        // [[Catacombs]]

        //Tricks Trail Trackin 2459
        Story.MapItemQuest(2459, "catacombs", new[] { 1525, 1526, 1527, 1528, 1529, 1530 });

        //Cultivate Answers 2460
        Story.KillQuest(2460, "catacombs", "Scorpion Cultist");

        //One Found Doll 2461
        Story.MapItemQuest(2461, "catacombs", 1531);

        //The Key to Saving Kimberly 2462
        Story.KillQuest(2462, "catacombs", "Scorpion Cultist");

        //Right Tool for the Job 2463
        Story.MapItemQuest(2463, "catacombs", 1532, 10);

        //Pick the Lock 2464
        Story.MapItemQuest(2464, "catacombs", 1533);

        //Destroy DeSawed 2465
        if (!Story.QuestProgression(2465))
        {
            Core.EnsureAccept(2465);
            Core.KillMonster("catacombs", "Frame6", "Left", "Dr. De'Sawed", "Disarmed De'Sawed");
            Core.EnsureComplete(2465);
        }

        //Demolish DeSawed FOR REAL 2466
        if (!Story.QuestProgression(2466))
        {
            Core.EnsureAccept(2466);
            Core.KillMonster("catacombs", "Boss2", "Left", "Dr. De'Sawed", "De'Sawed Defeated");
            Core.EnsureComplete(2466);
        }
    }

    public void PoeHub()
    {
        if (!Core.isSeasonalMapActive("poehub"))
            return;
        if (Core.isCompletedBefore(4571))
            return;

        Story.PreLoad(this);

        //Story Time! 3833
        Story.KillQuest(3833, "poehub", "Bookcase");

        //Medicine Man 3834
        if (!Story.QuestProgression(3834))
        {
            Core.EnsureAccept(3834);
            Core.HuntMonster("crusher", "Gothic Chest", "Mortar");
            Core.HuntMonster("crusher", "Gothic Chest", "Pestle");
            Core.HuntMonster("crusher", "Gothic Chest", "Syringe");
            Core.HuntMonster("crusher", "Gothic Chest", "Gourd Medicine Vial");
            Core.EnsureComplete(3834);
        }

        //Her Herbalism 3835
        if (!Story.QuestProgression(3835))
        {
            Core.EnsureAccept(3835);
            Core.GetMapItem(2941, 2, "crusher");
            Core.GetMapItem(2942, 3, "crusher");
            Core.GetMapItem(2943, 2, "crusher");
            Core.HuntMonster("crusher", "Bookcase", "Herbology Book", 1);
            Core.EnsureComplete(3835);
        }

        //Extra-Sensory Siblings 3836
        Story.KillQuest(3836, "crusher", "Apparition");

        //Dr's House(of Crusher)Call 3837
        Story.MapItemQuest(3837, "crusher", 2944);

        //Spook and the Book 3838
        Story.KillQuest(3838, "crusher", "Bookcase");

        //Sampling Sips 3839
        Story.KillQuest(3839, "cask", "Moglinberry Vines");

        //Glow with the Flambeaux 3840
        Story.KillQuest(3840, "cask", "Fire Performer");

        //Nitre Fighter 3841
        if (!Story.QuestProgression(3841))
        {
            Core.EnsureAccept(3841);
            Core.GetMapItem(2945, 1, "cask");
            Core.HuntMonster("cask", "Nitre Golem", "Nitre Crystal", 1);
            Core.EnsureComplete(3841);
        }

        //Spider Trouble 3842
        Story.KillQuest(3842, "cask", "Cellar Spider");

        //Coat of Harms 3843
        Story.MapItemQuest(3843, "cask", 2952);

        //Break through the Wall 3844
        Story.MapItemQuest(3844, "cask", 2946);

        //Search the Bones 3845
        Story.MapItemQuest(3845, "cask", 2953);

        //Defeat the Bone Terror 4570
        Story.KillQuest(4570, "cask", "Bone Terror");

        //Defeat Macabre Sepulchure 4571
        Story.KillQuest(4571, "wyvern", "Macabre Sepulchure");
    }

    public void MystcroftForest()
    {
        if (!Core.isSeasonalMapActive("mystcroftforest"))
            return;
        if (Core.isCompletedBefore(5427))
            return;

        Story.PreLoad(this);

        //Tome of The Otherworld 5418
        Story.KillQuest(5418, "mystcroftforest", "Eerie Apparition");

        //Monster Data 5419
        Story.KillQuest(5419, "mystcroftforest", "Eerie Apparition");

        //The Best Costumes Need the Best Fabric 5420
        Story.KillQuest(5420, "mystcroftforest", "Eerie Apparition");

        //Accessorize! 5421
        if (!Story.QuestProgression(5421))
        {
            Core.EnsureAccept(5421);
            Core.GetMapItem(4801, 8, "mystcroftforest");
            Core.HuntMonster("mystcroftforest", "Grim Goblin", "Gobbled String", 1);
            Core.EnsureComplete(5421);
        }

        //Make those Munchies 5422
        if (!Story.QuestProgression(5422))
        {
            Core.EnsureAccept(5422);
            Core.GetMapItem(4802, 8, "mystcroftforest");
            Core.HuntMonster("mystcroftforest", "Eerie Apparition", "Truffle Puree", 2);
            Core.EnsureComplete(5422);
        }

        //Every Party Needs Drinks 5423
        Story.KillQuest(5423, "mystcroftforest", "Grim Goblin");

        //Beautiful Creatures and their Uses 5424
        Story.KillQuest(5424, "timevoid", "Void Phoenix");

        //Ooh, Shiny! 5425
        if (!Story.QuestProgression(5425))
        {
            Core.EnsureAccept(5425);
            Core.HuntMonster("skytower", "Moonstone", "Moonstone Crystal", 10);
            Core.HuntMonster("skytower", "Sunstone", "Sunstone Crystal", 10);
            Core.HuntMonster("skytower", "Star Sapphire", "Sapphire Crystal", 10);
            Core.EnsureComplete(5425);
        }

        //Time to get Dressed Up! 5426
        if (!Story.QuestProgression(5426))
        {
            Core.EnsureAccept(5426);
            Core.GetMapItem(4800, 1, "mystcroftforest");
            Core.HuntMonster("mystcroftforest", "Grim Goblin", "Gilded Bookmark", 1);
            Core.EnsureComplete(5426);
        }

        //EEEEK 5427
        Story.KillQuest(5427, "mystcroftforest", "Barghest");
    }

    public void Chromafection()
    {
        if (!Core.isSeasonalMapActive("chromafection"))
            return;
        if (Core.isCompletedBefore(6537))
            return;

        Story.PreLoad(this);

        //Wacky Winka’s Candy Shop 6531
        Story.MapItemQuest(6531, "chromafection", 6004);

        //Nougat-ta Save the Parents 6532
        Story.KillQuest(6532, "chromafection", "Colorless Drone");

        //Chroma Kids 6533
        Story.MapItemQuest(6533, "chromafection", 6006, 10);

        //Caramel Clues 6534
        Story.KillQuest(6534, "chromafection", "Free Samples");

        //Sweet, Sweet Proof 6535
        Story.MapItemQuest(6535, "chromafection", 6007, 15);

        //Whacky Snacky 6536
        Story.MapItemQuest(6536, "chromafection", 6005);

        //Chroma CRASH 6537
        Story.KillQuest(6537, "chromafection", "Chromafection");

        //Color Farming 6538
        Story.KillQuest(6538, "chromafection", "Chromafection");
    }

    public void TwigsArcade()
    {
        if (!Core.isSeasonalMapActive("twigsarcade"))
            return;
        if (Core.isCompletedBefore(6579))
            return;

        Story.PreLoad(this);

        //Stop the Hurting 6568
        if (!Story.QuestProgression(6568))
        {
            Core.EnsureAccept(6568);
            Core.HuntMonster("twigsarcade", "SCOTTY SNEEVIL", "Mascots \"Stopped\"", 8);
            Core.EnsureComplete(6568);
        }

        //Gather the Goods 6569
        if (!Story.QuestProgression(6569))
        {
            // Hinge
            if (!Story.QuestProgression(6569))
            {
                Core.EnsureAccept(6569);
                Core.GetMapItem(6069, 3, "twigsarcade");

                // Fully Charged Battery
                Core.GetMapItem(6070, 3, "twigsarcade");

                // Wires && Small Screw
                Core.HuntMonster("twigsarcade", "SCOTTY SNEEVIL", "Wires", 5);
                Core.HuntMonster("twigsarcade", "SCOTTY SNEEVIL", "Small Screw", 16);
                Core.EnsureComplete(6569);
            }
        }

        //Spooky Casings 6570
        Story.KillQuest(6570, "twigsarcade", "Scotty Sneevil");

        //Do a Goo-d Job 6571
        Story.KillQuest(6571, "twigsarcade", "Ectoplasm");

        //Plop Plop Fizz Fizz 6572
        Story.MapItemQuest(6572, "twigsarcade", 6071);

        //Test the Ghost box 6573
        if (!Story.QuestProgression(6573))
        {
            Core.EnsureAccept(6573);
            Core.HuntMonster("twigsarcade", "SCOTTY SNEEVIL", "Ghost Caught", 5);
            Core.EnsureComplete(6573);
        }

        //Get Wid of CWEEPY TWIG 6574
        Story.KillQuest(6574, "twigsarcade", "Cweepy Twig");

        //Cranky Spirits 6575
        Story.KillQuest(6575, "twigsarcade", "Spirit Residue");

        //It's Too S-warm 6576
        Story.KillQuest(6576, "twigsarcade", "Swarmer");

        //Get into the Panic Room 6577
        Story.KillQuest(6577, "twigsarcade", "Cweepy Twilly");

        //Smash the Wall 6578
        Story.MapItemQuest(6578, "twigsarcade", 6072);

        //Scary Baby 6579
        Story.KillQuest(6579, "twigsarcade", "Baby");
    }

    public void Masquerade()
    {
        if (!Core.isSeasonalMapActive("masquerade"))
            return;
        if (Core.isCompletedBefore(7154))
            return;

        Story.PreLoad(this);

        //Mmm... Cake 7139
        Story.KillQuest(7139, "masquerade", "Harried Waiter");

        //Catch the Sprites 7140
        Story.KillQuest(7140, "masquerade", "Garden Sprite");

        //Spicy 7141
        Story.KillQuest(7141, "masquerade", "Kitchen Brownie");

        //Things are Getting Hairy 7142
        Story.MapItemQuest(7142, "masquerade", new[] { 6779, 6780, 6781, 6782, 6783 });

        //Let 'em Loose 7143
        Story.MapItemQuest(7143, "masquerade", 6784);

        //Gather Spores 7144
        Story.KillQuest(7144, "masquerade", "Glowshroom");

        //Getting Dusty 7145
        Story.KillQuest(7145, "masquerade", "Garden Sprite");

        //Gather Moss 7146
        Story.KillQuest(7146, "masquerade", "Fae Treeant");

        //Feed Bramblebite 7147
        Story.MapItemQuest(7147, "masquerade", 6785);

        //Feed Bramblebite 7148
        if (!Story.QuestProgression(7148))
        {
            Core.EnsureAccept(7148);
            Core.GetMapItem(6786, 6, "masquerade");
            Core.HuntMonster("masquerade", "Glass Spitter", "Gemseeds", 10);
            Core.EnsureComplete(7148);
        }

        //So Glamourous 7149
        Story.KillQuest(7149, "masquerade", "Garden Sprite");

        //Likin' the Lichen 7150
        Story.KillQuest(7150, "masquerade", "Fae Treeant");

        //It's Wine Time 7151
        Story.KillQuest(7151, "masquerade", "Wandering Guest");

        //Nap Time for Barb 7152
        if (!Story.QuestProgression(7152))
        {
            Core.EnsureAccept(7152);
            Core.GetMapItem(6789, 1, "masquerade");
            Core.HuntMonster("masquerade", "Scriptkeeper", "Put Barb to Sleep", 1);
            Core.EnsureComplete(7152);
        }

        //Sneak Out 7153
        Story.MapItemQuest(7153, "masquerade", 6787);

        //Break Out 7154
        Story.KillQuest(7154, "masquerade", "Aermhar");
    }

    public void That()
    {
        if (!Core.isSeasonalMapActive("that"))
            return;
        if (Core.isCompletedBefore(7179))
            return;

        Story.PreLoad(this);

        //Yeti Witual 7167
        if (!Story.QuestProgression(7167))
        {
            Core.EnsureAccept(7167);
            Core.GetMapItem(6790, 8, "that");
            Core.HuntMonster("that", "Candy Goblin", "Witch Twuffles", 8);
            Core.EnsureComplete(7167);
        }

        //Get the Fur 7168
        Story.KillQuest(7168, "that", "Moglinster");

        //Vapor Needed 7169
        Story.KillQuest(7169, "that", "Mystcroft Ghost");

        //Trap the Flame 7170
        Story.KillQuest(7170, "that", "Will o' The Wisp");

        //Down the Well 7171
        Story.MapItemQuest(7171, "that", 6791);

        //Cweepie-cwawlie Time 7172
        if (!Story.QuestProgression(7172))
        {
            Core.EnsureAccept(7172);
            Core.HuntMonster("that", "Well Spider", "Minion Slain", 10);
            Core.EnsureComplete(7172);
        }

        //Gwoop Destruction 7173
        Story.KillQuest(7173, "that", "Congealed Fear");

        //Seawch for Bubble 7174
        Story.MapItemQuest(7174, "that", 6792, 6);
        Story.MapItemQuest(7174, "that", 6793);

        //Free Bubble! 7175
        if (!Story.QuestProgression(7175))
        {
            Core.EnsureAccept(7175);
            Core.GetMapItem(6794, 1, "that");
            Core.HuntMonster("that", "Congealed Fear", "Skeleton Key", 1);
            Core.EnsureComplete(7175);
        }

        //Echo... echo... echo 7176
        Story.KillQuest(7176, "that", "Echo of That");

        //Gather Hope 7177
        Story.KillQuest(7177, "that", "Shattered Hope");

        //Bweak the Blacklights 7178
        Story.KillQuest(7178, "that", "Blacklights");

        //Get Wid of THAT 7179
        Story.KillQuest(7179, "that", "That");
    }

    public void NecroCarnival()
    {
        if (!Core.isSeasonalMapActive("necrocarnival"))
            return;
        if (Core.isCompletedBefore(8374))
            return;

        Story.PreLoad(this);

        //Dolls' Hide and Seek 8363
        if (!Story.QuestProgression(8363))
        {
            Core.EnsureAccept(8363);
            Core.GetMapItem(9249, 9, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Gummy Tapeworm", "Cartilage Gathered", 9);
            Core.EnsureComplete(8363);
        }

        //Arts and Crafts 8364
        if (!Story.QuestProgression(8364))
        {
            Core.EnsureAccept(8364);
            Core.GetMapItem(9250, 1, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Skeleclown", "Clowns Disciplined", 10);
            Core.EnsureComplete(8364);
        }

        //Lemonade, Chewy Ice 8365
        if (!Story.QuestProgression(8365))
        {
            Core.EnsureAccept(8365);
            Core.GetMapItem(9251, 2, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Mooch Treeant", "Red Sap Collected", 9);
            Core.HuntMonster("necrocarnival", "Gummy Tapeworm", "Tape Worms Fried", 9);
            Core.EnsureComplete(8365);
        }

        //Screams and Tag 8366
        if (!Story.QuestProgression(8366))
        {
            Core.EnsureAccept(8366);
            Core.GetMapItem(9252, 1, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Skeleclown", "Clown Fired", 10);
            Core.EnsureComplete(8366);
        }

        //Playful Teething 8367
        if (!Story.QuestProgression(8367))
        {
            Core.EnsureAccept(8367);
            Core.GetMapItem(9253, 4, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Mooch Treeant", "Pet Food", 9);
            Core.EnsureComplete(8367);
        }

        //Days Pass 8368
        if (!Story.QuestProgression(8368))
        {
            Core.EnsureAccept(8368);
            Core.GetMapItem(9254, 1, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Cotton Tick", "Cotton Tick Fluff Collected", 10);
            Core.EnsureComplete(8368);
        }

        //Pinned Bugs 8369
        if (!Story.QuestProgression(8369))
        {
            Core.EnsureAccept(8369);
            Core.GetMapItem(9255, 1, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Gummy Tapeworm", "Gum Sludge", 10);
            Core.EnsureComplete(8369);
        }

        //Sweet Glue 8370
        Story.KillQuest(8370, "necrocarnival", "Mooch Treeant");

        //Witch Soup 8371
        if (!Story.QuestProgression(8371))
        {
            Core.EnsureAccept(8371);
            Story.MapItemQuest(8371, "necrocarnival", 9256);
            Core.HuntMonster("necrocarnival", "Mooch Treeant", "Sticky Bark", 10);
            Core.HuntMonster("necrocarnival", "Gummy Tapeworm", "Gum Eye", 10);
            Core.HuntMonster("necrocarnival", "Skeleclown", "Finger Tips", 10);
            Core.EnsureComplete(8371);
        }

        //Written Promise 8372
        if (!Story.QuestProgression(8372))
        {
            Core.EnsureAccept(8372);
            Core.GetMapItem(9257, 7, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Cotton Tick", "Red Tick Juice", 10);
            Core.EnsureComplete(8372);
        }

        //All Fall Down 8373
        if (!Story.QuestProgression(8373))
        {
            Core.EnsureAccept(8373);
            Core.GetMapItem(9258, 1, "necrocarnival");
            Core.HuntMonster("necrocarnival", "Skeleclown", "Skeleclowns Reaped", 10);
            Core.EnsureComplete(8373);
        }

        //Lullaby 8374
        Story.KillQuest(8374, "necrocarnival", "Deva");
    }

    public void TrickTown()
    {
        if (!Core.isSeasonalMapActive("tricktown"))
            return;
        if (Core.isCompletedBefore(8935))
            return;

        Story.PreLoad(this);

        // Punchergeist 8926
        if (!Story.QuestProgression(8926))
        {
            Core.EnsureAccept(8926);
            Core.GetMapItem(10813, 1, "tricktown");
            Core.HuntMonster("tricktown", "Playful Ghost", "Specter Sweets", 8);
            Core.EnsureComplete(8926);
        }

        // Party Town 8927
        if (!Story.QuestProgression(8927))
        {
            Core.EnsureAccept(8927);
            Core.GetMapItem(10804, 3, "tricktown");
            Core.HuntMonster("tricktown", "Decay Spirit", "Decay Jelly", 8);
            Core.EnsureComplete(8927);
        }

        // Sweet Mucus 8928
        if (!Story.QuestProgression(8928))
        {
            Core.EnsureAccept(8928);
            Core.GetMapItem(10805, 5, "tricktown");
            Core.HuntMonster("tricktown", "Playful Ghost", "Ghosts Told Off", 8);
            Core.EnsureComplete(8928);
        }

        // Door to Door 8929
        if (!Story.QuestProgression(8929))
        {
            Core.EnsureAccept(8929);
            while (!Bot.ShouldExit && !Core.CheckInventory(73465, 300))
            {
                Core.AddDrop("Treats");
                Core.Join("tricktown");
                Core.KillMonster("trickortreat", "Enter", "Spawn", "Trick or Treater");
                Bot.Wait.ForPickup("Treats");
            }
            Core.EnsureComplete(8929);
        }

        // Crusted Compost 8930
        if (!Story.QuestProgression(8930))
        {
            Core.EnsureAccept(8930);
            Core.GetMapItem(10806, 3, "tricktown");
            Core.HuntMonster("tricktown", "Rotting Pumpkin", "Pumpkin Seeds", 10);
            Core.EnsureComplete(8930);
        }

        // A Spot of Sludge 8931
        if (!Story.QuestProgression(8931))
        {
            Core.EnsureAccept(8931);
            Core.GetMapItem(10807, 4, "tricktown");
            Core.HuntMonster("tricktown", "Decay Spirit", "Decay Spirit Caught", 10);
            Core.EnsureComplete(8931);
        }

        // Messy Unrest 8932
        if (!Story.QuestProgression(8932))
        {
            Core.EnsureAccept(8932);
            Core.HuntMonster("tricktown", "Decay Spirit", "Decay Spirit Calmed", 6);
            Core.HuntMonster("tricktown", "Playful Ghost", "Ghosts Silenced", 6);
            Core.EnsureComplete(8932);
        }

        // Plain Monster 8933
        if (!Story.QuestProgression(8933))
        {
            Core.EnsureAccept(8933);
            Core.GetMapItem(10808, 1, "tricktown");
            Core.HuntMonster("tricktown", "Rotting Mound", "Rotting Mound Caught", 1);
            Core.EnsureComplete(8933);
        }

        // Study Group 8934
        if (!Story.QuestProgression(8934))
        {
            Core.EnsureAccept(8934);
            Core.GetMapItem(10809, 5, "tricktown");
            Core.HuntMonster("tricktown", "Rotting Mound", "Skin Patches", 8);
            Core.EnsureComplete(8934);
        }

        // The Fruits of Labor 8935
        Story.KillQuest(8935, "tricktown", "Madam Ester");
    }

    public void CursedCastle()
    {
        if (!Core.isSeasonalMapActive("cursedcastle"))
            return;
        if (Core.isCompletedBefore(9454))
            return;

        Core.EquipClass(ClassType.Farm);

        // Ghoul Fungi 9445
        if (!Story.QuestProgression(9445))
        {
            Core.EnsureAccept(9445);
            Core.GetMapItem(12272, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Luminous Fungus", "Harvested Fungus", 8);
            Core.EnsureComplete(9445);
        }

        // Grotesque Guardian 9446
        if (!Story.QuestProgression(9446))
        {
            Core.EnsureAccept(9446);
            Core.GetMapItem(12273, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Noble Gargoyle", "Living Rubble", 8);
            Core.EnsureComplete(9446);
        }

        // Dragon Instinct 9447
        Story.MapItemQuest(9447, "cursedcastle", new[] { 12274, 12275 });

        // The Uninvited 9448
        if (!Story.QuestProgression(9448))
        {
            Core.EnsureAccept(9448);
            Core.GetMapItem(12276, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Noble Ghost", "Ghostly Velvet", 4);
            Core.EnsureComplete(9448);
        }

        // Room Service 9449
        Story.MapItemQuest(9449, "cursedcastle", new[] { 12277, 12278, 12279 });

        // Newborn's Teething 9450
        if (!Story.QuestProgression(9450))
        {
            Core.EnsureAccept(9450);
            Core.GetMapItem(12280, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Bone Dragonling", "Dragonling Rib", 20);
            Core.EnsureComplete(9450);
        }

        Core.EquipClass(ClassType.Solo);

        // Sadistic Solicitations 9451
        if (!Story.QuestProgression(9451))
        {
            Core.EnsureAccept(9451);
            Core.GetMapItem(12281, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Noble Ghost", "Shredded Invitation", 3);
            Core.EnsureComplete(9451);
        }

        // Stomping on Eggshells 9452
        if (!Story.QuestProgression(9452))
        {
            Core.EnsureAccept(9452);
            Core.GetMapItem(12282, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Dragonlord Coledale", "Dragonlord Defeated", 1);
            Core.EnsureComplete(9452);
        }

        // Stone Clad Clan 9453
        if (!Story.QuestProgression(9453))
        {
            Core.EnsureAccept(9453);
            Core.GetMapItem(12283, 1, "cursedcastle");
            Core.HuntMonster("cursedcastle", "Noble Gargoyle", "Cursed Stones", 4);
            Core.EnsureComplete(9453);
        }

        // Restless Waltz 9454
        Story.KillQuest(9454, "cursedcastle", "Unborn Brood");
    }

    public void Eldritchbattletown()
    {
        if (Core.isCompletedBefore(10453) || !Core.isSeasonalMapActive("eldritchbattletown"))
            return;

        Story.PreLoad(this);
        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Living Pumpkin", // UseableMonsters[0],
            "Dzeza Sapling", // UseableMonsters[1],
            "Nudibranch", // UseableMonsters[2],
            "Kathool Kultist", // UseableMonsters[3],
            "Dzeza Cultist", // UseableMonsters[4],
            "Harvest Acromegalia", // UseableMonsters[5]
        };
        #endregion Useable Monsters


        Core.EquipClass(ClassType.Farm);

        // 10444 | Pumpkin Guts
        if (!Story.QuestProgression(10444))
        {
            if (!Story.QuestProgression(10444))
            {
                Core.EnsureAccept(10444);
                Core.GetMapItem(15040, 1, "eldritchbattletown");
                Core.HuntMonster("eldritchbattletown", "Living Pumpkin", "Pumpkin Seeds", 12);
                Core.EnsureComplete(10444);
            }
        }

        // 10445 | Growth Spurt
        if (!Story.QuestProgression(10445))
        {
            if (!Story.QuestProgression(10445))
            {
                Core.EnsureAccept(10445);
                Core.GetMapItem(15041, 1, "eldritchbattletown");
                Core.HuntMonster("eldritchbattletown", "Dzeza Sapling", "Toothy Branch", 12);
                Core.EnsureComplete(10445);
            }
        }

        // 10446 | Mini-Thurians
        if (!Story.QuestProgression(10446))
        {
            if (!Story.QuestProgression(10446))
            {
                Core.EnsureAccept(10446);
                Core.GetMapItem(15042, 1, "eldritchbattletown");
                Core.HuntMonster("eldritchbattletown", "Nudibranch", "Nudibranch Glands", 3);
                Core.EnsureComplete(10446);
            }
        }

        // 10447 | Paper Love Bombs
        if (!Story.QuestProgression(10447))
        {
            Story.MapItemQuest(10447, "eldritchbattletown", 15047, 6);
        }

        // 10448 | Abomination vs Monstrosity
        if (!Story.QuestProgression(10448))
        {
            Story.KillQuest(
                10448,
                "eldritchbattletown",
                new[] { UseableMonsters[2], UseableMonsters[1] }
            );
        }

        // 10449 | The Mom Friend
        if (!Story.QuestProgression(10449))
        {
            if (!Story.QuestProgression(10449))
            {
                Core.EnsureAccept(10449);
                Core.GetMapItem(15043, 1, "eldritchbattletown");
                Core.HuntMonster("eldritchbattletown", "Kathool Kultist", "Kultist Parasites", 6);
                Core.EnsureComplete(10449);
            }
        }

        Core.EquipClass(ClassType.Solo);
        // 10450 | Excited Excrescence
        if (!Story.QuestProgression(10450))
        {
            Story.KillQuest(10450, "eldritchbattletown", UseableMonsters[4]);
        }

        // 10451 | Love It or Condemn It
        if (!Story.QuestProgression(10451))
        {
            Story.MapItemQuest(10451, "eldritchbattletown", new[] { 15044, 15045, 15046 });
        }

        // 10452 | Recruitment Initiative
        if (!Story.QuestProgression(10452))
        {
            Story.KillQuest(
                10452,
                "eldritchbattletown",
                new[] { UseableMonsters[3], UseableMonsters[4] }
            );
        }

        Core.EquipClass(ClassType.Solo);

        // 10453 | Galeux D'Eyes
        if (!Story.QuestProgression(10453))
        {
            Story.KillQuest(10453, "eldritchbattletown", UseableMonsters[5]);
        }
    }

    public void EldritchWorld()
    {
        if (Core.isCompletedBefore(10467) || !Core.isSeasonalMapActive("eldritchworld"))
            return;

        Eldritchbattletown();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Nudibranch", // UseableMonsters[0],
            "Infested Fisherman", // UseableMonsters[1],
            "Kathool Cultist", // UseableMonsters[2],
            "Dzeza Cultist", // UseableMonsters[3],
            "Dzeza Sapling", // UseableMonsters[4],
            "Infested Mummy", // UseableMonsters[5],
            "Mass of Hair", // UseableMonsters[6],
            "Mass of Teeth", // UseableMonsters[7],
            "Dzeza the Boundless", // UseableMonsters[8]
        };
        #endregion Useable Monsters

        // 10465 | World of the Eldritch
        if (!Story.QuestProgression(10465))
        {
            void EnsureItemFromQuest(
                int sourceQuestId,
                int monsterIndex,
                string itemName,
                int qty = 1
            )
            {
                if (Core.CheckInventory(itemName, qty))
                    return;

                Core.AddDrop(itemName);
                Core.HuntMonsterQuest(
                    sourceQuestId,
                    ("eldritchworld", UseableMonsters[monsterIndex], ClassType.Farm)
                );
            }

            Core.EnsureAccept(10465);

            EnsureItemFromQuest(10458, 0, "Nudibranch Roe");
            EnsureItemFromQuest(10459, 1, "Barnacle Rash");
            EnsureItemFromQuest(10460, 2, "Shrimp Noodle Pack");
            EnsureItemFromQuest(10461, 3, "Dzeza Welcome Pack");
            EnsureItemFromQuest(10462, 4, "Dzeza Coconuts");
            EnsureItemFromQuest(10463, 5, "Desiccated Bulbs");
            EnsureItemFromQuest(10464, 7, "Bucket of Molars");
            EnsureItemFromQuest(10468, 6, "Head of Hair");

            Core.EnsureComplete(10465);
        }

        // 10466 | Limitless Lengths
        if (!Story.QuestProgression(10466))
        {
            Core.EnsureAccept(10466);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("eldritchworld", "Mass of Teeth", "Brittle Enamel", 6);
            Core.HuntMonster("eldritchworld", "Mass of Hair", "Moldy Strands", 6);
            Core.EnsureComplete(10466);
        }

        // 10467 | Merely Existing
        if (!Story.QuestProgression(10467))
        {
            Core.HuntMonsterQuest(10467, ("eldritchworld", UseableMonsters[8], ClassType.Solo));
        }
    }

    public void ElodeaTemple()
    {
        if (Core.isCompletedBefore(10478) || !Core.isSeasonalMapActive("elodeatemple"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Nudibranch", // UseableMonsters[0],
            "Kathool Cultist", // UseableMonsters[1],
            "Merdrathoolian", // UseableMonsters[2],
            "Riptide Adept", // UseableMonsters[3],
            "Sarasa Maiden", // UseableMonsters[4],
            "Necroadipocere", // UseableMonsters[5],
            "Child of the Depths", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10469 | Escargot Escorts
        if (!Story.QuestProgression(10469))
        {
            Core.HuntMonsterQuest(10469, ("elodeatemple", UseableMonsters[0], ClassType.Farm));
        }

        // 10470 | Plus 2 Minus 2
        if (!Story.QuestProgression(10470))
        {
            Core.EnsureAccept(10470);
            Core.GetMapItem(15090, 1, "elodeatemple");
            Core.HuntMonster("elodeatemple", "Kathool Cultist", "Kultist's Invitation", 2);
            Core.EnsureComplete(10470);
        }

        // 10471 | Bottomfeeder Fruits
        if (!Story.QuestProgression(10471))
        {
            Core.EnsureAccept(10471);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("elodeatemple", "Nudibranch", "Seaslug Entrails", 6);
            Core.HuntMonster("elodeatemple", "Kathool Cultist", "Kultist Entrails", 4);
            Core.EnsureComplete(10471);
        }

        // 10472 | Dirge of the Depths
        if (!Story.QuestProgression(10472))
        {
            Core.HuntMonsterQuest(10472, ("elodeatemple", UseableMonsters[2], ClassType.Farm));
        }

        // 10473 | Borrowed Voices
        if (!Story.QuestProgression(10473))
        {
            Core.EnsureAccept(10473);
            Core.GetMapItem(15091, 1, "elodeatemple");
            Core.HuntMonster("elodeatemple", "Riptide Adept", "Riptide Song", 12);
            Core.EnsureComplete(10473);
        }

        // 10474 | Sea Symphony
        if (!Story.QuestProgression(10474))
        {
            Core.EnsureAccept(10474);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("elodeatemple", "Riptide Adept", "Adept's Aria", 8);
            Core.HuntMonster("elodeatemple", "Merdrathoolian", "Merdrathoolian's Oratio", 6);
            Core.EnsureComplete(10474);
        }

        // 10475 | If You Hate the Robe, You'll Hate its Priest
        Story.MapItemQuest(10475, new[] { (15092, 1, "elodeatemple"), (15093, 8, "elodeatemple") });

        // 10476 | Comets on the Cliff
        if (!Story.QuestProgression(10476))
        {
            Core.HuntMonsterQuest(10476, ("elodeatemple", UseableMonsters[4], ClassType.Farm));
        }

        // 10477 | Pipe Clog
        if (!Story.QuestProgression(10477))
        {
            Core.HuntMonsterQuest(10477, ("elodeatemple", UseableMonsters[5], ClassType.Farm));
        }

        // 10478 | The Victor! The Survivor!
        if (!Story.QuestProgression(10478))
        {
            Core.HuntMonsterQuest(10478, ("elodeatemple", UseableMonsters[6], ClassType.Solo));
        }
    }
	
	public void BirgittaMogloween()
    {
        if (Core.isCompletedBefore(10905)|| !Core.isSeasonalMapActive("birgittamogloween"))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Alpha Grigor", // UseableMonsters[0]
            "Champion Ensis", // UseableMonsters[1]
            "Lycan Guest", // UseableMonsters[2]
            "Nazarene Artist", // UseableMonsters[3]
            "ShadowSlayer", // UseableMonsters[4]
            "Vampiric Guest", // UseableMonsters[5]
        ];

        // 10896 | Silver of the Crossroads
        Story.KillQuest(10896, "birgittamogloween", UseableMonsters[4]); // Silver Bullets x36

        // 10897 | Lilies of the Valley
        Story.MapItemQuest(10897, "birgittamogloween", [16281, 16282]);

        // 10898 | Blue Blooms
        Story.KillQuest(10898, "birgittamogloween", UseableMonsters[3]); // Teal Resin x15

        // 10899 | Wise Beyond Years
        Story.MapItemQuest(10899, "birgittamogloween", [16283, 16284]);
		Story.MapItemQuest(10899, "birgittamogloween", 16285, 5);

        // 10900 | Dog Breath
        Story.KillQuest(10900, "birgittamogloween", UseableMonsters[2]); // Pickled Garlic x8

        // 10901 | Love Knows No Bounds
        Story.MapItemQuest(10901, "birgittamogloween", [16286, 16287]);

        // 10902 | Poison Perfume
        Story.KillQuest(10902, "birgittamogloween", UseableMonsters[5]); // Wolfsbane Perfume x2

        // 10903 | Naiver Vogel
        Story.MapItemQuest(10903, "birgittamogloween", 16288);
        Story.KillQuest(10903, "birgittamogloween", UseableMonsters[3]); // Blue Cork Grease x5

        // 10904 | Fighting Like Bats and Dogs
        Core.EnsureAccept(10904);
        Core.HuntMonster("birgittamogloween", UseableMonsters[2], "Darkovian Fur", 9);
        Core.HuntMonster("birgittamogloween", UseableMonsters[5], "Darkovian Silk", 9);
        Core.EnsureComplete(10904);

        // 10905 | The Opening Act
        Core.EnsureAccept(10905);
        Core.HuntMonster("birgittamogloween", UseableMonsters[1], "Champion Ensis Defeated", 1);
        Core.HuntMonster("birgittamogloween", UseableMonsters[0], "Alpha Grigor Defeated", 1);
        Core.EnsureComplete(10905);
    }
	   
	public void ShadowSlayerWar()
    {
        if (Core.isCompletedBefore(10916)||!Core.isSeasonalMapActive("shadowslayerwar"))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Awakened Hunter", // UseableMonsters[0]
            "Darkovia Hunter", // UseableMonsters[1]
            "Forest Cultist", // UseableMonsters[2]
            "ShadowSlayer Z", // UseableMonsters[3]
            "Sublime Hunter", // UseableMonsters[4]
            "Vampiric Ancestor ", // UseableMonsters[5]
            "Werepyre Ancestor ", // UseableMonsters[6]
        ];

        // 10908 | Wulpurga Medal
        Story.KillQuest(10908, "shadowslayerwar", UseableMonsters[5]); // Wulpurga Medal x5

        // 10909 | Mega Wulpurga Medal
        Story.KillQuest(10909, "shadowslayerwar", UseableMonsters[5]); // Mega Walpurga Medal x3

        // 10910 | Ghillie Suit
        Story.KillQuest(10910, "shadowslayerwar", UseableMonsters[1]); // Darkovian Hunter's Camo x6

        // 10911 | Silver Blood
        Story.KillQuest(10911, "shadowslayerwar", UseableMonsters[4]); // Sublime Silver x6

        // 10912 | Primal Rage
        Story.KillQuest(10912, "shadowslayerwar", UseableMonsters[5]); // Ancestral Vision x6

        // 10913 | Chimerical Presence
        Story.KillQuest(10913, "shadowslayerwar", UseableMonsters[6]); // Werepyre's Moonstone x1

        // 10914 | Sunlit Invocation
        Story.KillQuest(10914, "shadowslayerwar", UseableMonsters[2]); // Cultist's Sun Wheel x6

        // 10915 | The Most Dangerous Game
        Story.KillQuest(10915, "shadowslayerwar", UseableMonsters[0]); // Greater Hunter's Hexagram x6

        // 10916 | Daughter of the Sun
        Story.KillQuest(10916, "shadowslayerwar", UseableMonsters[3]); // ShadowSlayer Z Defeated x1
    }
	
	public void Walpurgisnacht()
    {
        if (Core.isCompletedBefore(10930)||!Core.isSeasonalMapActive("walpurgisnacht"))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Awakened Hunter", // UseableMonsters[0]
            "Beastly Hunger", // UseableMonsters[1]
            "Bloodlust", // UseableMonsters[2]
            "Druid Revenant", // UseableMonsters[3]
            "Forest Cultist", // UseableMonsters[4]
            "Moon's Firstborn", // UseableMonsters[5]
            "ShadowSlayer G ", // UseableMonsters[6]
            "The Gentleman", // UseableMonsters[7]
        ];

        // 10921 | Vervain and Vaughn
        Story.MapItemQuest(10921, "walpurgisnacht", 16340);
        Story.KillQuest(10921, "walpurgisnacht", UseableMonsters[3]); // Druid's Moon Sigil x4

        // 10922 | Marigold and Angelica
        Story.MapItemQuest(10922, "walpurgisnacht", 16341);
        Story.KillQuest(10922, "walpurgisnacht", UseableMonsters[4]); // Cultist's Angelica x6

        // 10923 | Nialla's Mourning Cypress
        if (!Story.QuestProgression(10923))
        {
            Core.EnsureAccept(10923);
            Core.GetMapItem(16342, 1, "walpurgisnacht");
            Core.HuntMonster("walpurgisnacht", UseableMonsters[4], "Cultist's Eye", 6);
            Core.HuntMonster("walpurgisnacht", UseableMonsters[3], "Druid's Eye", 6);
            Core.EnsureComplete(10923);
        }

        // 10924 | Song's Sheep Sorrel
        Story.MapItemQuest(10924, "walpurgisnacht", 16343);
        Story.KillQuest(10924, "walpurgisnacht", UseableMonsters[0]); // Hunter's Septogram x12

        // 10925 | Auld Man's Bell
        Story.KillQuest(10925, "walpurgisnacht", UseableMonsters[6]); // G's Crossroads Bullet x1

        // 10926 | Muscadier for Focus
        Story.MapItemQuest(10926, "walpurgisnacht", 16344, 8);

        // 10927 | Blood of the Moon
        Story.KillQuest(10927, "walpurgisnacht", UseableMonsters[2]); // Moonchild's Blood x6

        // 10928 | Bones of the Forest
        Story.KillQuest(10928, "walpurgisnacht", UseableMonsters[1]); // Moonchild's Marrow x6

        // 10929 | Empty Nest
        Story.KillQuest(10929, "walpurgisnacht", UseableMonsters[5]); // Moonchild's Memory x1

        // 10930 | Gentleman of the Crossroads
        Story.KillQuest(10930, "walpurgisnacht", UseableMonsters[7]); // Father Forest Defeated x1
    }
}
