/*
name: Etherstorm Wastes Story
description: This will finish the Etherstorm Wastes Story.
tags: story, quest, etherstorm-wastes
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class EtherStormWastes
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

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        DragonPlane();
        FireStorm();
        Water();
        Wind();
        Fire();
        Desoloth();
        FireStorm();
        AirStorm();
        WaterStorm();
        EarthStorm();
        Dragonhame();
        DragonHeart();
    }

    public void DragonPlane()
    {
        if (Core.isCompletedBefore(1395))
            return;

        Story.PreLoad(this);

        //Explore The DragonPlane 1384
        Story.MapItemQuest(1384, "dragonplane", new[] { 682, 683, 684, 685, 686 });

        //Lots Of Fiber 1385
        if (!Story.QuestProgression(1385))
        {
            Core.EnsureAccept(1385);
            Core.HuntMonster("dragonplane", "Fire Elemental", "Fire Fiber", 3);
            Core.HuntMonster("dragonplane", "Earth Elemental", "Earth Fiber", 3);
            Core.HuntMonster("dragonplane", "Water Elemental", "Water Fiber", 3);
            Core.HuntMonster("dragonplane", "Wind Elemental", "Air Fiber", 3);
            Core.EnsureComplete(1385);
        }

        //Carrying The Torches 1386
        Story.MapItemQuest(1386, "dragonplane", 687, 6);

        //Meet me in the Earth Realm 1387
        Story.MapItemQuest(1387, "dragonplane", 688);

        //Breaking Chains 1388
        if (!Story.QuestProgression(1388))
        {
            Core.EnsureAccept(1388);
            Core.HuntMonster("dragonplane", "Earth Elemental", "Earth Elemental Destroyed", 5);
            Core.HuntMonster("dragonplane", "Living Earth", "Living Earth Destroyed", 5);
            Core.EnsureComplete(1388);
        }

        //Soil Sprites 1389
        Story.MapItemQuest(1389, "dragonplane", 689, 5);

        //Cripple The Earth Prime 1390
        Story.KillQuest(1390, "dragonplane", "Living Earth");

        //Ward Of The Stone 1391
        Story.MapItemQuest(1391, "dragonplane", 696, 5);

        //Moganth's Dracoscintilla 1392
        Story.KillQuest(1392, "dragonplane", "Moganth");

        //Meet me in the DragonPlane 1393
        Story.MapItemQuest(1393, "dragonplane", 688);

        if (Core.IsMember)
        {
            //Getting Your Feet Wet 1394
            Story.MapItemQuest(1394, "dragonplane", 687, 6);

            //Meet me in the Water Realm 1395
            Story.MapItemQuest(1395, "water", 688);
        }
    }

    public void Water()
    {
        if (!Core.IsMember)
            return;

        DragonPlane();
        if (Core.isCompletedBefore(1403))
            return;

        Story.PreLoad(this);
        //Weaken The Bonds 1396
        if (!Story.QuestProgression(1396))
        {
            Core.EnsureAccept(1396);
            Core.HuntMonster("water", "Water Elemental", "Water Elemental Destroyed", 5);
            Core.HuntMonster("water", "Living Water", "Living Water Destroyed", 5);
            Core.EnsureComplete(1396);
        }

        //Udaroth Is Ready 1397
        Story.MapItemQuest(1397, "water", 690, 6);

        //A Loophole 1398
        Story.KillQuest(1398, "water", "Living Water");

        //Flipping The Script 1399
        Story.MapItemQuest(1399, "water", 691, 6);

        //Udaroth, The Water Prime 1400
        Story.KillQuest(1400, "water", "Udaroth");

        //Meet me in the DragonPlane 1401
        Story.MapItemQuest(1401, "dragonplane", 688);

        //Up In The Air 1402
        Story.MapItemQuest(1402, "dragonplane", 687, 6);

        //Meet me in the Air Realm 1403
        Story.MapItemQuest(1403, "wind", 688);
    }

    public void Wind()
    {
        if (!Core.IsMember)
            return;

        Water();
        if (Core.isCompletedBefore(1411))
            return;

        Story.PreLoad(this);

        //A Weak Wind 1404
        if (!Story.QuestProgression(1404))
        {
            Core.EnsureAccept(1404);
            Core.HuntMonster("wind", "Wind Elemental", "Wind Elemental Destroyed", 8);
            Core.HuntMonster("wind", "Living Air", "Living Air Destroyed", 8);
            Core.EnsureComplete(1404);
        }

        //An Ounce Of Prevention 1405
        Story.MapItemQuest(1405, "wind", 695, 5);

        //This Is A Breeze 1406
        Story.KillQuest(1406, "wind", "Living Air");

        //Chime Time 1407
        Story.MapItemQuest(1407, "wind", 692, 6);

        //Cellot, The Air Prime 1408
        Story.KillQuest(1408, "wind", "Cellot");

        //Meet me in the DragonPlane 1409
        Story.MapItemQuest(1409, "dragonplane", 688);

        //Hot Hot Heat 1410
        Story.MapItemQuest(1410, "dragonplane", 687, 6);

        //Meet me in the Fire Plane 1411
        Story.MapItemQuest(1411, "fire", 688);
    }

    public void Fire()
    {
        if (!Core.IsMember)
            return;

        Wind();
        if (Core.isCompletedBefore(1417))
            return;

        Story.PreLoad(this);

        //Putting Out Fires 1412
        if (!Story.QuestProgression(1412))
        {
            Core.EnsureAccept(1412);
            Core.HuntMonster("fire", "Fire Elemental", "Fire Elemental Destroyed", 10);
            Core.HuntMonster("fire", "Living Fire", "Living Fire Destroyed", 10);
            Core.EnsureComplete(1412);
        }

        //At A Loss 1413
        Story.MapItemQuest(1413, "fire", 693, 6);

        //Backburner 1414
        Story.KillQuest(1414, "fire", "Living Fire");

        //Open Fire 1415
        Story.MapItemQuest(1415, "fire", 694, 8);

        //Zellare, The Fire Prime 1416
        Story.KillQuest(1416, "fire", "Zellare");

        //Meet me in the DragonPlane 1417
        Story.MapItemQuest(1417, "dragonplane", 688);
    }

    public void Desoloth()
    {
        if (!Core.IsMember)
            return;

        Fire();
        if (Core.isCompletedBefore(1418))
            return;

        //    [[Desoloth Freed Character Page Badge]]
        //Open the DragonGate 1418
        if (!Story.QuestProgression(1418))
        {
            Core.EnsureAccept(1418);
            Core.HuntMonster("desoloth", "Desoloth", "Desoloth Freed!");
            Core.EnsureComplete(1418);
        }
    }

    public void FireStorm()
    {
        if (Core.isCompletedBefore(1547))
            return;

        Story.PreLoad(this);

        //Something Smells Rotten in EtherStorm 1532
        if (!Story.QuestProgression(1532))
        {
            Core.EnsureAccept(1532);
            Core.HuntMonster("firestorm", "Sulfur Imp", "Sulfuric Core", 10);
            Core.HuntMonster("firestorm", "Sulfur Imp", "Enchanted Lava Bucket", 1);
            Core.EnsureComplete(1532);
        }

        //Play With Fire 1533
        Story.KillQuest(1533, "firestorm", "Living Fire");

        //Set Me On Fire 1542
        Story.MapItemQuest(1542, "firestorm", 784, 13);

        //Walk Through Fire 1543
        Story.MapItemQuest(1543, "firestorm", 785);

        //Everything's On Fire 1545
        Story.MapItemQuest(1545, "firestorm", 786, 20);

        //Baby's On Fire 1546
        Story.KillQuest(1546, "firestorm", "FireStorm Hatchling");

        //Fire In My Heart 1547
        Story.KillQuest(1547, "firestorm", "Ssikari");
    }

    public void AirStorm()
    {
        FireStorm();
        if (Core.isCompletedBefore(1577))
            return;

        Story.PreLoad(this);

        //A Fresh Breath of Life 1571
        Story.KillQuest(1571, "airstorm", "Living Air");

        //Where Air You? 1572
        Story.MapItemQuest(1572, "airstorm", 827);

        //Invading Your (Sacred) Space 1573
        Story.MapItemQuest(1573, "airstorm", 823, 10);

        //There's Magic Every-Air 1574
        if (!Story.QuestProgression(1574))
        {
            Core.EnsureAccept(1574);
            Core.HuntMonster("airstorm", "KingCrystal", "Trapped Air Spirit", 1);
            Core.HuntMonster("airstorm", "Air Crystal", "Airy Aether Released", 12);
            Core.EnsureComplete(1574);
        }

        //Dragon's Fire Blossom to be Found 1575
        if (!Story.QuestProgression(1575))
        {
            Core.EnsureAccept(1575);
            Core.GetMapItem(824, 12, "firestorm");
            Core.HuntMonster("firestorm", "Sulfur Imp", "Caustic Pollen", 8);
            Core.EnsureComplete(1575);
        }

        //The Space Between 1576
        Story.KillQuest(1576, "airstorm", "Energy Tornado");

        //The Burning Question 1577
        Story.MapItemQuest(1577, "firestorm", 825);
    }

    public void WaterStorm()
    {
        AirStorm();
        if (Core.isCompletedBefore(1622))
            return;

        Story.PreLoad(this);

        //So, something washed up on the beach... 1616
        Story.MapItemQuest(1616, "waterstorm", 840);
        Story.MapItemQuest(1616, "waterstorm", 841);

        //Dilution Solution 1617
        Story.KillQuest(1617, "waterstorm", "Marsh Lurker");

        //A Frogdrake's Love Story 1618
        Story.MapItemQuest(1618, "waterstorm", 842, 13);

        //A Little Bit of Bubbly Never Hurt 1619
        if (!Story.QuestProgression(1619))
        {
            Core.EnsureAccept(1619);
            Core.GetMapItem(843, 10, "waterstorm");
            Core.HuntMonster("waterstorm", "Living Water", "Aqua Vitae", 15);
            Core.HuntMonster("waterstorm", "Living Water", "Water Bubble Burst", 10);
            Core.EnsureComplete(1619);
        }

        //Flight of the Fishwings 1620
        if (!Story.QuestProgression(1620))
        {
            Core.EnsureAccept(1620);
            Core.HuntMonster("waterstorm", "Fishwing", "Vial of Fishwing Venom", 1);
            Core.HuntMonster("waterstorm", "Fishwing", "Fish Wings", 12);
            Core.EnsureComplete(1620);
        }

        //Hunt for the Greater Good 1621
        if (!Story.QuestProgression(1621))
        {
            Core.EnsureAccept(1621);
            Core.GetMapItem(844, 3, "waterstorm");
            Core.HuntMonster("waterstorm", "Fishman Soldier", "Shining Fishman Trident", 1);
            Core.HuntMonster("waterstorm", "Marsh Lurker", "Marsh Tribal Totem", 1);
            Core.HuntMonster("waterstorm", "Frogdrake", "Powdered Drake's Horn", 1);
            Core.EnsureComplete(1621);
        }

        //Kill the Deep Dweller 1622
        Story.KillQuest(1622, "waterstorm", "Deep Dweller");
    }

    public void EarthStorm()
    {
        WaterStorm();
        if (Core.isCompletedBefore(1639))
            return;

        Story.PreLoad(this);

        //Call from Deep Within 1633
        if (!Story.QuestProgression(1633))
        {
            Core.EnsureAccept(1633);
            Core.HuntMonster("earthstorm", "Wind Elemental", "Still Wind", 5);
            Core.HuntMonster("earthstorm", "Water Elemental", "Toxic Water", 5);
            Core.HuntMonster("earthstorm", "Earth Elemental", "Shaken Stones", 5);
            Core.HuntMonster("earthstorm", "Fire Elemental", "Snuffed Flame", 5);
            Core.EnsureComplete(1633);
        }

        //Chrysalis of Flames 1634
        if (!Story.QuestProgression(1634))
        {
            Core.EnsureAccept(1634);
            Core.GetMapItem(860, 3, "earthstorm");
            Core.HuntMonster("earthstorm", "Crystallized Living Fire", "Shattered Heat", 5);
            Core.HuntMonster("earthstorm", "Sapphire Golem", "Aqua Libertas", 5);
            Core.EnsureComplete(1634);
        }

        //Glittering Secrets of Old 1635
        if (!Story.QuestProgression(1635))
        {
            Core.EnsureAccept(1635);
            Core.GetMapItem(862, 8, "earthstorm");
            Core.HuntMonster("earthstorm", "Crystalized Jellyfish", "Water Shell Matrix Decoder", 6);
            Core.HuntMonster("earthstorm", "Diamond Golem", "Diamond CrystalCutter", 6);
            Core.EnsureComplete(1635);
        }

        //Aria of Great Significance
        if (!Story.QuestProgression(1636))
        {
            Core.EnsureAccept(1636);
            Core.GetMapItem(861, 8, "earthstorm");
            Core.GetMapItem(863, 1, "earthstorm");
            Core.HuntMonster("earthstorm", "Shard Spinner", "G Tone", 2);
            Core.HuntMonster("earthstorm", "Shard Spinner", "B Tone", 2);
            Core.HuntMonster("earthstorm", "Shard Spinner", "D Tone", 2);
            Core.HuntMonster("earthstorm", "Shard Spinner", "E Tone", 2);
            Core.HuntMonster("earthstorm", "Emerald Golem", "Greenglass Bells", 8);
            Core.EnsureComplete(1636);
        }

        //Rekindling a Pomegranate 1637
        Story.KillQuest(1637, "earthstorm", "Ruby Golem");

        //Legendary Crystal Skulls 1638
        Story.KillQuest(1638, "earthstorm", "Amethite");

        //Return of the Crystal Beauty 1639
        Story.KillQuest(1639, "earthstorm", "Arradia");
    }

    public void Dragonhame()
    {
        if (Core.isCompletedBefore(3522))
            return;

        Story.PreLoad(this);

        //Talk to Hs'Sakar in Firestorm 3497
        Story.MapItemQuest(3497, "firestorm", 2646);

        //Bring Amulet to Drakor 3498
        Story.KillQuest(3498, "firestorm", "Sulfur Imp");

        //Kill Us Some Zombies 3499
        Story.KillQuest(3499, "shadowfall", "Skeletal Knight");

        //Put 'Em Through the Wringer 3501
        Story.KillQuest(3501, "fotia", "Femme Cult Worshiper");

        //The Core of the Problem 3502
        if (!Story.QuestProgression(3502))
        {
            Core.EnsureAccept(3502);
            Core.HuntMonster("dragonplane", "Earth Elemental", "Core of Earth", 3);
            Core.HuntMonster("dragonplane", "Fire Elemental", "Core of Fire", 3);
            Core.HuntMonster("dragonplane", "Water Elemental", "Core of Water", 3);
            Core.HuntMonster("dragonplane", "Wind Elemental", "Core of Wind", 3);
            Core.EnsureComplete(3502);
        }

        //The Scroll of Salubris 3503
        Story.KillQuest(3503, "Natatorium", "Merdraconian");

        //Must Be All Those Limbs 3504
        if (!Story.QuestProgression(3504))
        {
            Core.EnsureAccept(3504);
            Core.KillMonster("wanders", "r5", "Left", "Lotus Spider", "Lotus Petal", 5);
            Core.KillMonster("wanders", "r2", "Down", "Kalestri Worshiper", "Talisman of Renewal", 5);
            Core.EnsureComplete(3504);
        }

        //What We Need Is More Mages 3505
        Story.MapItemQuest(3505, "dragonrune", new[] { 2649, 2650, 2651, 2652, 2653 });

        //Protect the Eggs 3506
        Story.MapItemQuest(3506, "dragonhame", 2647, 8);
        Story.MapItemQuest(3506, "dragonhame", 2648);

        //Very, Very Sad 3507
        Story.KillQuest(3507, "dragonhame", "Infected Dragon");

        //Ward the Hospital 3509
        Story.KillQuest(3509, "dragonhame", "Infected Dragon");

        //Recover samples in Lair 3511
        if (!Story.QuestProgression(3511))
        {
            Core.EnsureAccept(3511);
            Core.HuntMonster("lair", "Dark Draconian", "Superior Dragon Blood", 3);
            Core.HuntMonster("lair", "Venom Draconian", "Draconian Venom", 3);
            Core.HuntMonster("lair", "Water Draconian", "Draconian Tears", 3);
            Core.HuntMonster("lair", "Wyvern", "Wyvern Scales", 3);
            Core.EnsureComplete(3511);
        }

        //Darkness to Calm Dragons 3512
        Story.KillQuest(3512, "lair", "Dark Draconian");

        //Quest for the Red Dragon’s Scroll 3513
        Story.KillQuest(3513, "lair", "Red Dragon");

        //Help Artix in Swordhavenundead 3514
        if (!Story.QuestProgression(3514))
        {
            Core.EnsureAccept(3514);
            Core.KillMonster("swordhavenundead", "Enter", "Spawn", "*", "Undead Slain", 13);
            Core.EnsureComplete(3514);
        }

        //Kill Dracolich in Temple 3515
        Story.KillQuest(3515, "temple", "Dracolich");

        //Research in Gilead 3516
        if (!Story.QuestProgression(3516))
        {
            Core.EnsureAccept(3516);
            Core.HuntMonster("dragonplane", "Wind Elemental", "Pure Air Essence", 3);
            Core.HuntMonster("dragonplane", "Earth Elemental", "Pure Earth Essence", 3);
            Core.HuntMonster("dragonplane", "Fire Elemental", "Pure Fire Essence", 3);
            Core.HuntMonster("dragonplane", "Water Elemental", "Pure Water Essence", 3);
            Core.EnsureComplete(3516);
        }

        //Fire and Ice in Battleundera 3517
        if (!Story.QuestProgression(3517))
        {
            Core.EnsureAccept(3517);
            Core.HuntMonster("battleundera", "Skeletal Fire Mage", "Undead Fire Marrow", 6);
            Core.HuntMonster("battleundera", "Skeletal Ice Mage", "Undead Ice Marrow", 6);
            Core.EnsureComplete(3517);
        }

        //Unlife's a Lich 3518
        Story.KillQuest(3518, "battleundera", "Lich");

        //Dragonhame Infirmary Invasion 3519
        Story.KillQuest(3519, "dragonheart", "Zombie Dragon");

        //Elemental Dracolich Destruction 3520
        Story.KillQuest(3520, "infirmary", "Proto-Earth Dracolich");

        //Dragonplane in Peril 3522
        Story.ChainQuest(3522);
    }

    public void DragonHeart()
    {
        if (Core.isCompletedBefore(3535))
            return;

        Story.PreLoad(this);

        //Ward the Heart 3523
        Story.MapItemQuest(3523, "dragonheart", 2660, 5);

        //Infection Rejection 3524
        Story.KillQuest(3524, "dragonheart", "Infected Dragonling");

        //Purify the Plane 3525
        Story.MapItemQuest(3525, "dragonheart", 2661);

        //Zombie Dragon Destruction 3526
        Story.KillQuest(3526, "dragonheart", "Zombie Dragon");

        //Water Realm Purification 3527
        if (!Story.QuestProgression(3527))
        {
            Core.EnsureAccept(3527);
            Core.GetMapItem(2662, 1, "dragonheart");
            Core.HuntMonster("dragonheart", "Proto-Water Dracolich", "Water Link Weakened", 5);
            Core.EnsureComplete(3527);
        }

        //Water Cleanses Fire 3528
        Story.MapItemQuest(3528, "dragonheart", 2663, 7);

        //Fire Realm Purification 3529
        if (!Story.QuestProgression(3529))
        {
            Core.EnsureAccept(3529);
            Core.GetMapItem(2664, 1, "dragonheart");
            Core.HuntMonster("dragonheart", "Proto-Fire Dracolich", "Fire Link Weakened", 6);
            Core.EnsureComplete(3529);
        }

        //Fire Cleanses Earth 3530
        Story.MapItemQuest(3530, "dragonheart", 2665, 7);

        //Earth Realm Purification 3531
        if (!Story.QuestProgression(3531))
        {
            Core.EnsureAccept(3531);
            Core.GetMapItem(2666, 1, "dragonheart");
            Core.HuntMonster("dragonheart", "Proto-Earth Dracolich", "Earth Link Weakened", 7);
            Core.EnsureComplete(3531);
        }

        //Earth Cleanses Air 3532
        Story.MapItemQuest(3532, "dragonheart", 2667, 7);

        //Air Realm Purification 3533
        if (!Story.QuestProgression(3533))
        {
            Core.EnsureAccept(3533);
            Core.GetMapItem(2668, 1, "dragonheart");
            Core.HuntMonster("dragonheart", "Proto-Air Dracolich", "Air Link Weakened", 8);
            Core.EnsureComplete(3533);
        }

        //Dracolich Disconnection 3534
        if (!Story.QuestProgression(3534))
        {
            Core.EnsureAccept(3534);
            Core.HuntMonster("dragonheart", "Deluge Dracolich", "Water Link Severed", 1);
            Core.HuntMonster("dragonheart", "Inferno Dracolich", "Fire Link Severed", 1);
            Core.HuntMonster("dragonheart", "Granite Dracolich", "Earth Link Severed", 1);
            Core.HuntMonster("dragonheart", "Tempest Dracolich", "Air Link Severed", 1);
            Core.EnsureComplete(3534);
        }

        //Heart-searing Pain 3535
        Story.KillQuest(3535, "dragonheart", "Avatar of Desolich");
    }
}
