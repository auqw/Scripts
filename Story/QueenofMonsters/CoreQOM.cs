/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class CoreQOM
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void CompleteEverything()
    {
        aTheftofLight();
        DoomwoodPaladinsTrial();
        DarkoviaDarkDiaspora();
        ShadowfallDarknessRising();
        SwordhavenTheNewWorld();
        TheDestroyer();
        TheReshaper();
        TheBook();
        TheQueensSecrets();
    }

    public void aTheftofLight()
    {
        //Progress Check
        if (Core.isCompletedBefore(5387))
            return;

        //Preload Quests
        Story.PreLoad(this);

        Core.AddDrop(31052);

        //Summon Help
        if (!Story.QuestProgression(4495))
        {
            Core.EnsureAccept(4495);
            Core.GetMapItem(3698, 1, "celestialrealm");
            Core.HuntMonster("celestialrealm", "Celestial Bird of Paradise", "Golden Grain", 1);
            Core.HuntMonster("celestialrealm", "Fallen Knight", "Bright Flower Seed", 10);
            Core.EnsureComplete(4495);
        }

        //Power Up!
        if (!Story.QuestProgression(4496))
            Core.BuyItem("embersea", 1100, "Basic Guard Potion", 10);
        Story.KillQuest(
            4496,
            "celestialrealm",
            new[] { "Celestial Bird of Paradise", "Fallen Knight" }
        );

        //The Final Spell Fragment
        Story.MapItemQuest(4497, "celestialrealm", 3696);

        //Find the Map
        if (!Story.QuestProgression(4498))
        {
            Core.AddDrop("Dwakel Decoder");
            Core.GetMapItem(106, 1, "crashsite");
            Core.EnsureAccept(4498);
            Core.HuntMonster("celestialrealm", "Infernal Knight", "Map of Lore", 1);
            Core.EnsureComplete(4498);
        }

        //Reveal the Portal!
        Story.MapItemQuest(4499, "celestialrealm", 3693);

        //Investigate the Ruins
        if (!Story.QuestProgression(4500))
        {
            Core.EnsureAccept(4500);
            Core.GetMapItem(3694, 3, "lostruins");
            Core.HuntMonster("lostruins", "Underworld Hound", "Underworld Hound Defeated", 5);
            Core.EnsureComplete(4500);
        }

        //Take out the Knights
        Story.KillQuest(4501, "lostruins", "Fallen Knight");

        //Find the Clues
        if (!Story.QuestProgression(4502))
        {
            Core.EnsureAccept(4502);
            Core.HuntMonster("lostruins", "Underworld Hound", "Clue 1");
            Core.HuntMonster("lostruins", "Infernal Imp", "Clue 2");
            Core.HuntMonster("lostruins", "Fallen Knight", "Clue 3");
            Core.KillMonster("lostruins", "r5", "Left", "Infernal Knight", "Clue 4");
            Core.EnsureComplete(4502);
        }

        //Recover the Cage Key!
        if (!Story.QuestProgression(4503))
        {
            Core.EnsureAccept(4503);
            Farm.FishingREP(2);
            if (!Core.CheckInventory("Holy Oil"))
                Core.BuyItem("fishing", 356, "Holy Oil");
            Core.HuntMonsterMapID("lostruins", 8, "Cage Key");
            Core.EnsureComplete(4503);
            Bot.Wait.ForPickup(31052);
        }

        //Protect Them
        if (!Story.QuestProgression(4504))
        {
            Core.EnsureAccept(4504);
            if (!Core.CheckInventory("Potent Guard Potion", 10))
                Core.BuyItem("embersea", 1100, "Potent Guard Potion", 10);
            Core.HuntMonster("lostruins", "Fallen Knight", "Infernal Enemy Defeated", 15);
            Core.EnsureComplete(4504);
        }

        //Break the Spell
        if (!Story.QuestProgression(4505))
        {
            Core.EnsureAccept(4505);
            Core.HuntMonster("lostruins", "Fallen Knight", "Brimstone-Stained Gauntlet", 5);
            Core.HuntMonster("lostruins", "Underworld Hound", "Onyx Spike", 3);
            Core.GetMapItem(3697, 5, "lostruins");
            Core.EnsureComplete(4505);
        }

        //Open the Cage
        if (!Core.CheckInventory(31052))
        {
            Core.EnsureAccept(4503);
            Farm.FishingREP(2);
            if (!Core.CheckInventory("Holy Oil"))
                Core.BuyItem("fishing", 356, "Holy Oil");
            Core.HuntMonsterMapID("lostruins", 8, "Cage Key");
            Core.EnsureComplete(4503);
            Bot.Wait.ForPickup(31052);
        }
        Story.MapItemQuest(4506, "lostruins", 3695);

        //Defeat the Infernal Warlord
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(4507, "lostruins", "Infernal Warlord");

        //Celestial Realm at War
        Story.KillQuest(4509, "lostruinswar", "Infernal Imp");

        //Defeat the Diabolical Warlord!
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(4508, "lostruinswar", "Diabolical Warlord");

        //Infernal Destruction
        if (!Story.QuestProgression(5374))
        {
            Core.EnsureAccept(5374);
            Core.HuntMonster("infernalspire", "Fallen Knight", "Fallen Knight Slain", 8);
            Core.HuntMonster("infernalspire", "Underworld Hound", "Underworld Hound Slain", 8);
            Core.EnsureComplete(5374);
        }

        //Gone Without A Trace
        if (!Story.QuestProgression(5375))
        {
            Core.EnsureAccept(5375);
            Core.GetMapItem(4729, 1, "infernalspire");
            Core.GetMapItem(4730, 1, "infernalspire");
            Core.KillMonster("infernalspire", "r2", "Left", "Fallen Knight", "Celestial Silk", 1);
            Core.HuntMonster("infernalspire", "Underworld Hound", "Torn Feathers", 1);
            Core.EnsureComplete(5375);
        }

        //Helzekiel
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5376, "infernalspire", "Helzekiel");

        //Get the Keys
        if (!Story.QuestProgression(5377))
        {
            Core.EnsureAccept(5377);
            Core.KillMonster("infernalspire", "r7", "Left", "Dungeon Fiend", "Infernal Key", 6);
            Core.KillMonster("infernalspire", "r7", "Left", "Dungeon Fiend", "Dungeon Fiend Slain", 6);
            Core.KillMonster("infernalspire", "r7", "Left", "Infernal Hound", "Infernal Hound Slain", 6);
            Core.EnsureComplete(5377);
        }

        //Free the Captives
        Story.MapItemQuest(5378, "infernalspire", 4731, 6);

        //Energy Needed
        if (!Story.QuestProgression(5379))
        {
            Core.EnsureAccept(5379);
            Core.GetMapItem(4732, 6, "infernalspire");
            Core.HuntMonster("infernalspire", "Dungeon Fiend", "Celestial Energy", 6);
            Core.EnsureComplete(5379);
        }

        //Interrogate the Jailer
        Story.KillQuest(5380, "infernalspire", "Garvodeus");

        //Get the Code
        if (!Story.QuestProgression(5381))
        {
            Core.EnsureAccept(5381);
            Core.KillMonster("infernalspire", "r13", "Left", "Fallen Knight", "Override Code");
            Core.KillMonster("infernalspire", "r13", "Left", "Fallen Knight", "Fallen Knight Slain", 6);
            Core.HuntMonster("infernalspire", "Infernal Imp", "Infernal Imp Slain", 6);
            Core.EnsureComplete(5381);
        }

        //Enter the Code
        Story.MapItemQuest(5382, "infernalspire", 4733);

        //Smash it Up
        Story.MapItemQuest(5383, "infernalspire", 4734, 4);

        //Take out the Overseer
        Story.KillQuest(5384, "infernalspire", "Azkorath");

        //Clear the Invaders
        if (!Story.QuestProgression(5385))
        {
            Core.EnsureAccept(5385);
            Core.HuntMonster("infernalspire", "Infernal Knight", "Infernal Knight Slain", 8);
            Core.HuntMonster("infernalspire", "Grievous Fiend", "Grievous Fiend Slain", 8);
            Core.EnsureComplete(5385);
        }

        //Find the Weapon
        Story.MapItemQuest(5386, "infernalspire", 4735);

        //What is THAT?
        Story.KillQuest(5387, "infernalspire", "Malxas");
    }

    public void DoomwoodPaladinsTrial()
    {
        //Progress Check
        if (Core.isCompletedBefore(5416))
            return;

        //Preload Quests
        Story.PreLoad(this);

        //A New Recruit
        if (!Story.QuestProgression(5404))
        {
            Core.EnsureAccept(5404);
            if (!Core.CheckInventory("Blessed Coffee of the Lightguard"))
            {
                Core.AddDrop("Blessed Coffee of the Lightguard");
                Core.EnsureAccept(5405);
                Core.HuntMonster("sandsea", "Oasis Monkey", "Pally Luwak Beans");
                Core.EnsureComplete(5405);
            }
            Core.EnsureComplete(5404);
        }

        //SLAY some UNDEAD
        Story.KillQuest(5406, "doompally", "Doomwood Ectomancer");

        //SLAY some MORE UNDEAD!!
        Story.KillQuest(5407, "doompally", "Doomwood Soldier");

        //Slay BIGGER!!
        Story.KillQuest(5408, "doompally", "Doomwood Ectomancer");

        //Babe in the Woods
        Story.MapItemQuest(5409, "doompally", 4758);

        //The Dark Thicket
        if (!Story.QuestProgression(5410))
        {
            Core.EnsureAccept(5410);
            Core.GetMapItem(4759, 5, "doompally");
            Core.HuntMonster("doompally", "Doomwood Bonemuncher", "Thicket Monsters Slain", 8);
            Core.EnsureComplete(5410);
        }

        //Emily
        Story.MapItemQuest(5411, "doompally", 4761);
        Story.MapItemQuest(5411, "doompally", 4760, 6);

        //Sage Advice
        Story.KillQuest(5412, "doompally", "Doomwood Treeant");

        //Abra-Cadaver
        Story.KillQuest(5413, "doompally", "Doomwood Ectomancer");

        //A (Skele)TON of skulls
        Story.KillQuest(5414, "doompally", "Doomwood Bonemuncher");

        //Summoning the Subjugator
        Story.MapItemQuest(5415, "doompally", 4762);

        //Subjugator Wraithbone
        Story.KillQuest(5416, "doompally", "Skeletal Subjugator");
    }

    public void DarkoviaDarkDiaspora()
    {
        //Progress Check
        if (Core.isCompletedBefore(5503))
            return;

        //Preload Quests
        Story.PreLoad(this);

        //Hounds and Infernals and Imp, Oh my!
        if (!Story.QuestProgression(5487))
        {
            Core.EnsureAccept(5487);
            Core.HuntMonster("darkoviainvasion", "Underworld Hound", "Underworld Hound Slain", 5);
            Core.HuntMonster("darkoviainvasion", "Infernal Imp", "Infernal Imp Slain", 5);
            Core.EnsureComplete(5487);
        }

        //Like Imps in A Pod
        Story.MapItemQuest(5488, "DarkoviaInvasion", 4905, 6);

        //A Grievous Threat
        Story.KillQuest(5489, "DarkoviaInvasion", "Grievous Fiend");

        //Undead Investigation
        Story.MapItemQuest(5490, "SafiriaInvasion", 4904);

        //What We Need Is A Big Can Of Raid
        if (!Story.QuestProgression(5491))
        {
            Core.EnsureAccept(5491);
            Core.HuntMonster("safiriainvasion", "Fallen Knight", "Fallen Knights Slain", 5);
            Core.HuntMonster("safiriainvasion", "Infernal Knight", "Infernal Knights Slain", 5);
            Core.EnsureComplete(5491);
        }

        //Trapped!
        Story.MapItemQuest(5492, "SafiriaInvasion", 4895, 6);
        Story.MapItemQuest(5492, "SafiriaInvasion", 4896);

        //Won't Someone Think Of The Minions?
        Story.MapItemQuest(5493, "SafiriaInvasion", 4897, 9);

        //Here, Doggy Doggy
        if (!Story.QuestProgression(5494))
        {
            Core.EnsureAccept(5494);
            Core.GetMapItems(new[] { (4898, 1), (4899, 1) }, "SafiriaInvasion");
            Core.HuntMonster("safiriainvasion", "Blood Maggot", "Maggot Blood", 6);
            Core.EnsureComplete(5494);
        }

        //Ma'alech
        Story.KillQuest(5495, "SafiriaInvasion", "Ma'alech");

        //I'm Not Lycan This Situation
        Story.MapItemQuest(5496, "LycanInvasion", 4900);

        //The Best Way To Slay An Infernal
        if (!Story.QuestProgression(5497))
        {
            Core.EnsureAccept(5497);
            Core.HuntMonster("lycaninvasion", "Infernal Knight", "Celestial Spear", 6);
            Core.HuntMonster("lycaninvasion", "Fallen Knight", "Celestial Lightblade", 6);
            Core.EnsureComplete(5497);
        }

        //A Dire Situation
        Story.KillQuest(5498, "LycanInvasion", "Dire Wolf");

        //I'd Lycan To Go Now
        Story.MapItemQuest(5499, "LycanInvasion", 4901);
        Story.MapItemQuest(5499, "LycanInvasion", 4903, 6);

        //Lord Balax'el
        Story.KillQuest(5500, "LycanInvasion", "Lord Balax'el");

        //Follow Lady Solani
        if (!Story.QuestProgression(5501))
        {
            Core.EnsureAccept(5501);
            Core.GetMapItem(4902, 1, "SafiriaInvasion");
            Core.HuntMonster("safiriainvasion", "Fallen Knight", "Infernals Slain", 5);
            Core.EnsureComplete(5501);
        }

        //Revenant Slayer
        if (!Story.QuestProgression(5502))
        {
            Core.EnsureAccept(5502);
            Core.HuntMonster("safiriainvasion", "Shadow Imp", "Shadow Imp Slain", 8);
            Core.HuntMonster("safiriainvasion", "Revenant", "Revenant Slain", 8);
            Core.EnsureComplete(5502);
        }

        //Noddharath
        Story.KillQuest(5503, "SafiriaInvasion", "Noddharath");
    }

    public void ShadowfallDarknessRising()
    {
        //Progress Check
        if (Core.isCompletedBefore(5557))
            return;

        DarkoviaDarkDiaspora();

        //Preload Quests
        Story.PreLoad(this);

        //Commander Tibias
        Story.MapItemQuest(5543, "shadowfallinvasion", 5024);

        //Clear the Walls
        if (!Story.QuestProgression(5544))
        {
            Core.EnsureAccept(5544);
            Core.HuntMonster("shadowfallinvasion", "Infernal Imp", "Infernal Imp Slain", 6);
            Core.HuntMonster("shadowfallinvasion", "Infernal Knight", "Infernal Knight Slain", 6);
            Core.EnsureComplete(5544);
        }

        //Load the Ballistas
        Story.MapItemQuest(5545, "shadowfallinvasion", 5025, 4);
        Story.MapItemQuest(5545, "shadowfallinvasion", 5026, 4);

        //Arm the Archers
        Story.KillQuest(5546, "shadowfallinvasion", "Infernal Imp");

        //Reinforce the Walls
        Story.MapItemQuest(5547, "shadowfallinvasion", 5027, 5);

        //Work Done!
        Story.MapItemQuest(5548, "shadowfallinvasion", 5028);

        //Clear the Tower
        Story.KillQuest(5549, "shadowfallinvasion", "Bone Creeper");

        //Find the Passage
        Story.MapItemQuest(5550, "shadowfallinvasion", 5029);

        //Go Through That Door
        if (!Story.QuestProgression(5551))
        {
            Core.EnsureAccept(5551);
            Core.HuntMonster("shadowfallinvasion", "Bone Guardian", "Bone Guardian Slain");
            Core.HuntMonster("shadowfallinvasion", "Bone Guardian", "Soul Fragment Retrievement");
            Core.EnsureComplete(5551);
        }

        //Infernal Attack
        if (!Story.QuestProgression(5552))
        {
            Core.EnsureAccept(5552);
            Core.HuntMonster("shadowfallinvasion", "Nethermage", "Nethermage Slain", 4);
            Core.HuntMonster("shadowfallinvasion", "Diabolical Scryer", "Diabolical Scryer Slain", 4);
            Core.HuntMonster("shadowfallinvasion", "Fallen Knight", "Fallen Knight Slain", 4);
            Core.EnsureComplete(5552);
        }

        //Here's A Hammer, Get To Work
        Story.MapItemQuest(5553, "shadowfallinvasion", 5030, 9);

        //The Next Step
        Story.MapItemQuest(5554, "shadowfallinvasion", 5031);

        //Soul Fuel
        if (!Story.QuestProgression(5555))
        {
            Core.EnsureAccept(5555);
            Core.GetMapItem(5032, 1, "shadowfallinvasion");
            Core.HuntMonster("shadowfallinvasion", "Diabolical Scryer", "Infernal Soul", 12);
            Core.EnsureComplete(5555);
        }

        //Time to Fly
        Story.MapItemQuest(5556, "shadowfallinvasion", 5033);

        //YOU again!
        Story.KillQuest(5557, "shadowfallinvasion", "Lord Balax'el");
    }

    public void SwordhavenTheNewWorld()
    {
        //Progress Check
        if (Core.isCompletedBefore(5586))
            return;

        //Preload Quests
        Story.PreLoad(this);

        //Stand for Swordhaven
        if (!Story.QuestProgression(5575))
        {
            Core.EnsureAccept(5575);
            Core.KillMonster("safiriainvasion", "r4", "Left", "Fallen Knight", "Fallen Knight's Armor", 5);
            Core.KillMonster("safiriainvasion", "r4", "Left", "Infernal Knight", "Infernal Knight's Armor", 5);
            Core.EnsureComplete(5575);
        }

        //Use Their Energy Against Them
        Story.KillQuest(5576, "shadowfallinvasion", "Nethermage");

        //Scry this!
        Story.KillQuest(5577, "shadowfallinvasion", "Diabolical Scryer");

        //Recalibration
        if (!Story.QuestProgression(5578))
        {
            Core.EnsureAccept(5578);
            Core.KillMonster("DoomPally", "r3", "Right", "*", "Doomwood Invaders Fought", 4);
            Core.KillMonster("DarkoviaInvasion", "Enter", "Spawn", "*", "Darkovia Invaders Fought", 4);
            Core.KillMonster("shadowfallinvasion", "r4", "Left", "*", "Shadowfall Invaders Fought", 4);
            Core.EnsureComplete(5578);
        }

        //Arm the Armored!
        Story.MapItemQuest(5579, "castleinvasion", 5055, 5);
        Story.MapItemQuest(5579, "castleinvasion", 5056);

        //Save the Citizens
        Story.MapItemQuest(5580, "castleinvasion", 5058, 5);

        //Destroy the Infernals
        if (!Story.QuestProgression(5581))
        {
            Core.EnsureAccept(5581);
            Core.HuntMonster("castleinvasion", "Infernal Knight", "Infernal Knights Slain", 3);
            Core.HuntMonster("castleinvasion", "Fallen Knight", "Fallen Knights Slain", 3);
            Core.HuntMonster("castleinvasion", "Nethermage", "Nethermages Slain", 3);
            Core.EnsureComplete(5581);
        }

        //What is THAT??
        if (!Story.QuestProgression(5582))
        {
            Core.EnsureAccept(5582);
            // Drop is 100%, and will get stuck  otherwise as its auto-turnin
            Core.KillMonster("castleinvasion", "r2a", "Left", "*");
            Bot.Wait.ForPickup("Giant Worm of Teeth Defeated");
            Core.EnsureComplete(5582);
        }

        //Arm the Civilians
        Story.MapItemQuest(5583, "castleinvasion", 5057, 4);

        //Get Those Beasts
        if (!Story.QuestProgression(5584))
        {
            Core.EnsureAccept(5584);
            Core.HuntMonster("castleinvasion", "Infernal Imp", "Infernal Imps Slain", 4);
            Core.HuntMonster("castleinvasion", "Underworld Hound", "Underworld Hounds Slain", 4);
            Core.EnsureComplete(5584);
        }

        Core.Logger("Relaoding the map for the worm");
        Bot.Map.Reload();
        Bot.Sleep(2500);
        Bot.Wait.ForMapLoad("castleinvasion");

        //It's Baaaaack!
        if (!Story.QuestProgression(5585))
        {
            Core.EnsureAccept(5585);
            // Drop is 100%, and will get stuck  otherwise as its auto-turnin
            Core.KillMonster("castleinvasion", "r6a", "Left", "*");
            Bot.Wait.ForPickup("Giant Worm of Teeth Defeated... Again!");
            Core.EnsureComplete(5585);
        }

        //Him Again???
        Story.KillQuest(5586, "castleinvasion", "Lord Balax'el");
    }

    public void TheDestroyer()
    {
        if (Core.isCompletedBefore(5847))
            return;

        //Preload Quests
        Story.PreLoad(this);

        //What Happened to Baldric?
        Story.MapItemQuest(5791, "therift", 5228);

        //Quests Here Bug Out So This is Needed.
        Core.AcceptandCompleteTries = 3;

        //Get a Monitor
        Story.MapItemQuest(5803, "charredpath", 5248);

        //Follow the Pattern.. He Ne Ar Kr
        if (!Story.QuestProgression(5804))
        {
            Core.EnsureAccept(5804);
            Core.GetMapItem(5249, 1, "crashsite");
            Core.HuntMonster("crashsite", "Dwakel Warrior", "Purified Xenon", 6);
            Core.EnsureComplete(5804);
        }

        //The PTM is Ready!
        if (!Story.QuestProgression(5805))
        {
            Core.EnsureAccept(5805);
            Core.GetMapItem(5256, 1, "charredpath");
            Core.KillMonster("charredpath", "r3", "Left", "Noxious Fumes", "Noxious Fumes Eradicated", 5);
            Core.KillMonster("charredpath", "r3", "Left", "Toxic Bile", "Toxic Bile Eradicated", 5);
            Core.EnsureComplete(5805);
        }

        //Save the Creatures
        if (!Story.QuestProgression(5806))
        {
            Core.EnsureAccept(5806);
            Core.GetMapItem(5250, 6, "charredpath");
            Core.HuntMonster("charredpath", "Ragewing", "Monster Stilled", 7);
            Core.EnsureComplete(5806);
        }

        //Clear the Treeants
        if (!Story.QuestProgression(5807))
        {
            Core.EnsureAccept(5807);
            Core.GetMapItem(5251, 3, "charredpath");
            Core.HuntMonster("charredpath", "Toxic Treeant", "Treeant Felled", 6);
            Core.EnsureComplete(5807);
        }

        //Excise the Infection
        Story.KillQuest(5808, "charredpath", "Infected Hare");

        //Root out the Plague
        if (!Story.QuestProgression(5809))
        {
            Core.EnsureAccept(5809);
            Core.GetMapItem(5252, 6, "charredpath");
            Core.HuntMonster("charredpath", "Plague Spreader", "Plague Spreaders Slain", 7);
            Core.EnsureComplete(5809);
        }

        //I Said Yes, Yes, Yes
        Story.MapItemQuest(5810, "charredpath", 5255);

        //Rally the Mages
        if (!Story.QuestProgression(5811))
        {
            Core.EnsureAccept(5811);
            Core.GetMapItem(5253, 4, "therift");
            Core.HuntMonster("therift", "Mana Chest", "Mana Bomb", 8);
            Core.EnsureComplete(5811);
        }

        //Wisteria Hysteria
        Story.KillQuest(5812, "charredpath", "Toxic Wisteria");

        //Confront Extriki
        Story.MapItemQuest(5813, "charredpath", 5254);

        //Remove the Bile
        if (!Story.QuestProgression(5819))
        {
            Core.EnsureAccept(5819);
            Core.HuntMonster("charredpath", "Noxious Fumes", "Toxin Destroyed", 10);
            Core.EnsureComplete(5819);
        }

        //Get the Plague (Crystal)
        Story.KillQuest(5820, "charredpath", "Plague Spreader");

        //Grab the Growth
        Story.KillQuest(5821, "charredpath", "Infected Hare");

        //Treeants for Wood
        Story.KillQuest(5822, "farm", "Treeant");

        //Sand sounds better than Litter...
        if (!Story.QuestProgression(5823))
        {
            Core.EnsureAccept(5823);
            while (!Bot.ShouldExit && !Core.CheckInventory(39194, 8))
                Core.KillMonster("baconcat", "r8", "Left", "Litter Elemental", log: false);
            Core.EnsureComplete(5823);
        }

        //Destroy the Zognax
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5824, "charredpath", "Zognax");

        //Bandages Needed
        Story.KillQuest(5830, "charredpath", "Ravenous Parasite");

        //Shinies!
        if (!Story.QuestProgression(5831))
        {
            Core.EnsureAccept(5831);
            Core.KillMonster("skytower", "r3", "Bottom", "Sunstone", "Sunstone", 3);
            Core.KillMonster("skytower", "r3", "Bottom", "Moonstone", "Moonstone", 3);
            Core.KillMonster("skytower", "r3", "Bottom", "Star Sapphire", "Star Sapphire", 3);
            Core.EnsureComplete(5831);
        }

        //Plushies
        Story.KillQuest(5832, "sewerpink", "Cutie Grumbley");

        //Sleepies
        if (!Story.QuestProgression(5833))
        {
            Core.EnsureAccept(5833);

            if (!Core.CheckInventory("Black Metal Cold Brew"))
            {
                Core.AddDrop("Black Metal Cold Brew");
                Core.EnsureAccept(5834);
                Core.HuntMonster("therift", "Mana Chest", "Liquid Mana", 4);
                Core.HuntMonster("therift", "Ravenous Parasite", "Parasite \"Spice\"", 4);
                Core.EnsureComplete(5834);
            }

            Core.EnsureComplete(5833);
        }

        //What's Next?
        Story.MapItemQuest(5835, "charredpath", 5270);

        //Make a Bed
        Story.KillQuest(5836, "charredpath", "Pustulisk");

        //Talk to Ravinos
        Story.MapItemQuest(5837, "underglade", 5271);

        //Into the Underglade
        if (!Story.QuestProgression(5838))
        {
            Core.EnsureAccept(5838);
            Core.HuntMonster("underglade", "Tree Nymph", "Tree Nymph", 5);
            Core.HuntMonster("underglade", "Forest Spirit", "Forest Spirit", 5);
            Core.EnsureComplete(5838);
        }

        //Clear the Spores
        Story.KillQuest(5839, "underglade", "Slime Spore");

        //Cleanse the Walls
        if (!Story.QuestProgression(5840))
        {
            Core.EnsureAccept(5840);
            Core.GetMapItem(5272, 8, "underglade");
            Core.HuntMonster("underglade", "Forest Spirit", "Essence of Nature", 8);
            Core.EnsureComplete(5840);
        }

        //Expose the Entrance
        Story.KillQuest(5841, "underglade", "Blackened Earth");

        //Gather the Glows
        Story.KillQuest(5842, "underglade", "Luminous Fungus");

        //More Corruption Revealed
        if (!Story.QuestProgression(5843))
        {
            Core.EnsureAccept(5843);
            Core.GetMapItem(5273, 6, "underglade");
            Core.HuntMonster("underglade", "Forest Spirit", "Elemental Earth", 6);
            Core.EnsureComplete(5843);
        }

        //The Goblin Threat
        Story.KillQuest(5844, "underglade", "Twisted Goblin");

        //Get an Offering
        Story.KillQuest(5845, "underglade", "Gemstone Elemental");

        //The Heart of the Glade
        Story.KillQuest(5846, "underglade", "Lunamoss");

        //Defeat Extriki
        Story.KillQuest(5847, "extriki", "Extriki");
    }

    public void TheReshaper(bool TerraneMerge = false)
    {
        //Progress Check
        if (Core.isCompletedBefore(5877))
            return;

        //Preload Quests
        Story.PreLoad(this);

        //Capture the Misshapen
        if (!Story.QuestProgression(5849))
        {
            Core.EnsureAccept(5849);
            Core.HuntMonster("pilgrimage", "SpiderWing", "Spiderwing Captured", 4);
            Core.HuntMonster("pilgrimage", "Urstrix", "Urstrix Captured", 4);
            Core.EnsureComplete(5849);
        }

        //Defeat the Parasites
        Story.KillQuest(5850, "Pilgrimage", "Ravenous Parasite");

        //The Source
        Story.KillQuest(5851, "Pilgrimage", "Extrikiti");

        //Find Lucky!
        if (!Story.QuestProgression(5852))
        {
            Core.EnsureAccept(5852);
            Core.GetMapItem(5289, 1, "Pilgrimage");
            Core.GetMapItem(5288, 3, "Pilgrimage");
            Core.HuntMonster("pilgrimage", "Extrikiti", "Tuft of Fur", 3);
            Core.HuntMonster("pilgrimage", "Extrikiti", "Lucky's Collar", 1);
            Core.EnsureComplete(5852);
        }

        //Gather a Scouting Party
        if (!Story.QuestProgression(5853))
        {
            Core.EnsureAccept(5853);
            Core.GetMapItem(5290, 6, "pilgrimage");
            Core.GetMapItem(5291, map: "pilgrimage");
            Core.EnsureComplete(5853);
        }

        //Defeat the Parasites
        if (!Story.QuestProgression(5854))
        {
            Core.EnsureAccept(5854);
            Core.GetMapItem(5292, 1, "Pilgrimage");
            Core.HuntMonster("pilgrimage", "Ravenous Parasite", "Monsters Slain", 10);
            Core.EnsureComplete(5854);
        }

        //Bad Dog
        Story.KillQuest(5855, "Pilgrimage", "Lucky");

        //Connect to the Earth
        if (!Story.QuestProgression(6276))
        {
            Core.EnsureAccept(6276);
            Core.GetMapItem(5769, 5, "guardiantree");
            Core.HuntMonster("guardiantree", "Blossoming Treeant", "Treeant Cuttings", 5);
            Core.EnsureComplete(6276);
        }

        //Help the Hedgies
        Story.MapItemQuest(6277, "guardiantree", 5776, 5);
        Story.MapItemQuest(6277, "guardiantree", 5770, 5);

        //Cleanse the Corrupted Zards
        Story.KillQuest(6278, "guardiantree", "Corrupted Zard");

        //Plant the Seed
        if (!Story.QuestProgression(6279))
        {
            Core.EnsureAccept(6279);
            Core.KillMonster("guardiantree", "r2a", "Bottom", "Seed Spitter", "Perfect Seed");
            Core.GetMapItem(5771, map: "guardiantree");
            Core.EnsureComplete(6279);
        }

        //Reach the Top
        Story.MapItemQuest(6280, "guardiantree", 5772);

        //Cointain the Pollen
        Story.KillQuest(6281, "guardiantree", "Blossoming Treeant");

        //Pass Through the Pollen
        if (!Story.QuestProgression(6282))
        {
            Core.EnsureAccept(6282);
            Core.GetMapItem(5773, 1, "guardiantree");
            Core.HuntMonster("guardiantree", "Pollen Cloud", "Pollen Clouds Destroyed", 5);
            Core.EnsureComplete(6282);
        }

        //Reinvigorate the Sprout
        if (!Story.QuestProgression(6283))
        {
            Core.EnsureAccept(6283);
            Core.KillMonster("guardiantree", "r8", "Left", "Seed Spitter", "Life Energy", 8);
            Core.EnsureComplete(6283);
        }

        //Up We Go!
        Story.MapItemQuest(6284, "guardiantree", 5774);

        //Grow a Bridge
        if (!Story.QuestProgression(6285))
        {
            Core.EnsureAccept(6285);
            Core.GetMapItem(5775, 2, "guardiantree");
            Core.HuntMonster("guardiantree", "Myconid", "Fungal Spores", 6);
            Core.EnsureComplete(6285);
        }

        //Take Down Terrane
        if (!Story.QuestProgression(6286))
        {
            Core.EnsureAccept(6286);
            Core.KillMonster("guardiantree", "r12", "Left", "Terrane");
            Core.EnsureComplete(6286);
        }
        if (TerraneMerge)
            return;

        //Explore the Cavern
        if (!Story.QuestProgression(5856))
        {
            Core.EnsureAccept(5856);
            Core.GetMapItem(5293, 1, "TwistedCavern");
            Core.HuntMonster("twistedcavern", "Extrikiti", "Extrikitis Slain", 8);
            Core.EnsureComplete(5856);
        }

        //Capture the Insects
        Story.KillQuest(5857, "TwistedCavern", "Infesting Swarm");

        //Face the Consequences
        if (!Story.QuestProgression(5858))
        {
            Core.EnsureAccept(5858);
            Core.HuntMonster("TwistedCavern", "Fungal Lord", "Fungal Lord Slain", 6);
            Core.HuntMonster("TwistedCavern", "Seed Stalker", "Seed Stalker Slain", 6);
            Core.HuntMonster("TwistedCavern", "Seed Stalker", "Scrap of Brown Cloth");
            Core.EnsureComplete(5858);
        }

        //Stalk This
        Story.KillQuest(5859, "TwistedCavern", "Seed Stalker");

        //Free the Flies
        if (!Story.QuestProgression(5860))
        {
            Core.EnsureAccept(5860);
            Core.GetMapItem(5294, 1, "TwistedCavern");
            Core.HuntMonster("twistedcavern", "SpiderWing", "Spiderwings Slain", 8);
            Core.EnsureComplete(5860);
        }

        //Follow the Swarm
        Story.MapItemQuest(5861, "TwistedCavern", 5295, 6);

        //Leading to the Wall
        if (!Story.QuestProgression(5862))
        {
            Core.EnsureAccept(5862);
            Core.HuntMonster("TwistedCavern", "Urstrix", "Urstrix Slain", 6);
            Core.HuntMonster("TwistedCavern", "Fungal Lord", "Fungal Lord Slain", 4);
            Core.EnsureComplete(5862);
        }

        //Get through the Wall
        Story.KillQuest(5863, "TwistedCavern", "Wall of Vines");

        //Defeat the Golem
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5864, "TwistedCavern", "Lore Golem");

        //Time for a Dose
        if (!Story.QuestProgression(5866))
        {
            Core.EnsureAccept(5866);
            Core.HuntMonster("brokenwoods", "Urstrix", "Urstrix Dosed", 3);
            Core.HuntMonster("brokenwoods", "SpiderWing", "Spiderwing Dosed", 3);
            Core.EnsureComplete(5866);
        }

        //Comparing Samples
        if (!Story.QuestProgression(5867))
        {
            Core.EnsureAccept(5867);
            Core.HuntMonster("brightoak", "Hootbear", "Hootbear Feathers", 2);
            Core.HuntMonster("BrokenWoods", "Urstrix", "Urstrix Feathers", 2);
            Core.HuntMonster("pines", "LeatherWing", "LeatherWing Claws", 2);
            Core.HuntMonster("BrokenWoods", "SpiderWing", "SpiderWing Claws", 2);
            Core.EnsureComplete(5867);
        }

        //Examine the Foliage
        if (!Story.QuestProgression(5868))
        {
            Core.EnsureAccept(5868);
            Core.GetMapItem(5296, 4, "BrokenWoods");
            Core.GetMapItem(5297, 4, "BrokenWoods");
            Core.HuntMonster("brokenwoods", "Fungal Lord", "Fungal Lord Slime", 4);
            Core.EnsureComplete(5868);
        }

        //Investigate the Goo
        Story.KillQuest(5869, "BrokenWoods", "Extrikiti");

        //Second Dose
        if (!Story.QuestProgression(5870))
        {
            Core.EnsureAccept(5870);
            Core.HuntMonster("brokenwoods", "Urstrix", "Urstrix Dosed", 3);
            Core.HuntMonster("brokenwoods", "SpiderWing", "Spiderwing Dosed", 3);
            Core.EnsureComplete(5870);
        }

        //Still Not Working
        if (!Story.QuestProgression(5871))
        {
            Core.EnsureAccept(5871);
            Core.HuntMonster("charredpath", "Ravenous Parasite", "Captured Parasite", 3);
            Core.HuntMonster("charredpath", "Plague Spreader", "Captured Plague Spreader", 3);
            Core.EnsureComplete(5871);
        }

        //Get the Final Ingredients
        if (!Story.QuestProgression(5872))
        {
            Core.EnsureAccept(5872);
            Core.BuyItem("embersea", 1100, 1749, 25);
            Core.HuntMonster("Arcangrove", "Seed Spitter", "Uncorrupted Spitter Seeds", 3);
            Core.HuntMonster("poisonforest", "Treeant", "Treeant Berries", 4);
            Core.EnsureComplete(5872);
        }

        //Smash the Hive
        Story.KillQuest(5873, "brokenwoods", "Hive");

        //Acolyte's Medallions
        if (!Story.QuestProgression(5874))
        {
            Core.EnsureAccept(5874);
            Core.HuntMonster("Kolyaban", "Poisonous Darkblood", "Acolyte's Medallion", 4);
            Core.EnsureComplete(5874);
        }

        //Let's Hope This Works
        Story.KillQuest(5876, "Kolyaban", "Twisted Aria");

        //Defeat Kolyaban
        Story.KillQuest(5877, "Kolyaban", "Kolyaban");
    }

    public void TheBook()
    {
        if (Core.isCompletedBefore(8080))
            return;

        Story.PreLoad(this);

        //8048 | Time to Study
        if (!Story.QuestProgression(8048))
        {
            Core.EnsureAccept(8048);
            Core.HuntMonster("forestreach", "Monstrous Imp", "Monstrous Imp Captured", 8);
            Core.HuntMonster("forestreach", "Eldritch Parasite", "Eldritch Parasite Captured", 8);
            Core.EnsureComplete(8048);
        }

        //8049 | Calm Them Down
        Story.KillQuest(8049, "forestreach", "Chaos Spitter");

        //8050 | Get the Reagents
        if (!Story.QuestProgression(8050))
        {
            Core.EnsureAccept(8050);
            Core.GetMapItem(8362, 5, "forestreach");
            Core.HuntMonster("forestreach", "Chaos Sp-eye", "Chaos Ichor", 6);
            Core.EnsureComplete(8050);
        }

        //8051 | Samples for Comparison
        if (!Story.QuestProgression(8051))
        {
            Core.EnsureAccept(8051);
            Core.GetMapItem(8363, 5, "forestreach");
            Core.HuntMonster("forestreach", "Chaos Spitter", "Chaos Spitter Cutting", 7);
            Core.EnsureComplete(8051);
        }

        //8052 | More Testing Required
        if (!Story.QuestProgression(8052))
        {
            Core.EnsureAccept(8052);
            Core.HuntMonster("forestreach", "EldritchWing", "EldritchWing Captured", 8);
            Core.HuntMonster("forestreach", "Chaos Sp-eye", "Chaos Sp-Eye Captured", 8);
            Core.EnsureComplete(8052);
        }

        //8053 | Tidy Up
        if (!Story.QuestProgression(8053))
        {
            Core.EnsureAccept(8053);
            Core.HuntMonster("forestreach", "Eldritch Parasite", "Eldritch Parasite Slain", 8);
            Core.HuntMonster("forestreach", "Monstrous Imp", "Monstrous Imp Slain", 8);
            Core.EnsureComplete(8053);
        }

        //8054 | We Need Neutralizer
        if (!Story.QuestProgression(8054))
        {
            Core.EnsureAccept(8054);
            Core.HuntMonster("forestreach", "Chaos Spitter", "Acidic Pollen", 6);
            Core.HuntMonster("forestreach", "Chaos Sp-eye", "Eye Gunk", 4);
            Core.EnsureComplete(8054);
        }

        //8055 | Neutralize 'Em
        Story.MapItemQuest(8055, "forestreach", 8364, 5);

        //8056 | Vermin Clearing
        Story.KillQuest(8056, "backroom", "Chaos Rat");

        //8057 | Find the Book
        Story.MapItemQuest(8057, "backroom", 8365);

        //8058 | Get the Key
        Story.KillQuest(8058, "backroom", "Chaos Rat");

        //8059 | Get the Book
        Story.MapItemQuest(8059, "backroom", 8366);

        //8060 | It's a Book Wyrm!
        Story.KillQuest(8060, "backroom", "Book Wyrm");

        //8067 | The Eye of the Beholder
        Story.KillQuest(8067, "deepforest", "Creeping Gaze");

        //8068 | The Double - Edged Sword
        Story.KillQuest(8068, "deepforest", "Eldritch Stalker");

        //8069 | So Ichor - y
        Story.KillQuest(8069, "deepforest", "Terrarsite");

        //8070 | Reveal the Words
        Story.MapItemQuest(8070, "deepforest", 8415);

        //8071 | Shining Time
        Story.KillQuest(8071, "deepforest", "Deep Truffle");

        //8072 | Light the Way
        Story.MapItemQuest(8072, "deepforest", 8416, 8);

        //8073 | Chaos Gathers
        Story.KillQuest(8073, "deepforest", "Creeping Gaze");

        //8074 | Hold your Breath
        Story.KillQuest(8074, "deepforest", "Terrarsite");

        //8075 | Disperse the Mist
        Story.MapItemQuest(8075, "deepforest", 8418, 8);

        //8076 | Monsters in the Mist
        Story.KillQuest(8076, "deepforest", "Deep Truffle");

        //8077 | Truffle Hunting
        Story.KillQuest(8077, "deepforest", "Deep Truffle");

        //8078 | Bribe 'Em All
        Story.KillQuest(8078, "deepforest", "Cthulhoid");

        //8079 | Heading Back Home
        Story.MapItemQuest(8079, "deepforest", 8419, 4);
        Story.MapItemQuest(8079, "deepforest", 8420, 1);

        //8080 | The Aberration
        Story.KillQuest(8080, "deepforest", "Aberrant Horror");
    }

    public void TheQueensSecrets(bool DeepTunnelMerge = false)
    {
        if (Core.isCompletedBefore(8107))
            return;

        Story.PreLoad(this);

        //8083 | Crystal Tears
        Story.KillQuest(8083, "transformation", "Monstrite");

        //8084 | That Which Grows
        Story.KillQuest(8084, "transformation", "Chaos Spitter");

        //8085 | That Which Flies
        Story.KillQuest(8085, "transformation", "Tentastrike");

        //8086 | That Which Flames
        Story.KillQuest(8086, "transformation", "Tentaflame");

        //8087 | A Caustic Oil
        Story.KillQuest(8087, "transformation", "Chaos Spitter");

        //8088 | Tools of the Trade
        if (!Story.QuestProgression(8088))
        {
            Core.EnsureAccept(8088);
            Core.HuntMonster("transformation", "Monstrite", "Mortar Stone");
            Core.HuntMonster("transformation", "Monstrite", "Pestle Stone");
            Core.EnsureComplete(8088);
        }

        //8089 | Burn it!
        Story.MapItemQuest(8089, "transformation", 8435);
        Story.MapItemQuest(8089, "transformation", 8436);
        Story.MapItemQuest(8089, "transformation", 8437);

        //8090 | Find the Tributes
        Story.KillQuest(8090, "transformation", "Deep Tunneler");

        //8091 | Compare the Crystal
        Story.KillQuest(8091, "transformation", "Monstrite");

        //8092 | Acid and Scratch Test
        if (!Story.QuestProgression(8092))
        {
            Core.EnsureAccept(8092);
            Core.HuntMonster("transformation", "Tentastrike", "Acid Venom", 5);
            Core.HuntMonster("transformation", "Deep Tunneler", "Sharpest Fang", 1);
            Core.EnsureComplete(8092);
        }

        //8093 | The Largest Monstrite
        Story.KillQuest(8093, "transformation", "Eldritch Abomination");

        //8094 | Tame the Queen
        Story.KillQuest(8094, "transformation", "Queen of Monsters");
        if (DeepTunnelMerge)
            return;

        //8096 | Worms of Earth
        Story.KillQuest(8096, "downbelow", "Earthwyrm");

        //8097 | Find the Trail
        Story.KillQuest(8097, "downbelow", "Rumbling Rubble");

        //8098 | Open the Way
        if (!Story.QuestProgression(8098))
        {
            Core.EnsureAccept(8098);
            Core.GetMapItem(8491, 1, "downbelow");
            Core.HuntMonster("downbelow", "Monstrous Flame", "Monstrous Flame", 5);
            Core.EnsureComplete(8098);
        }

        //8099 | Follow the Trail
        if (!Story.QuestProgression(8099))
        {
            Core.EnsureAccept(8099);
            Core.GetMapItem(8492, 1, "downbelow");
            Core.HuntMonster("downbelow", "Earthwyrm", "Monster Slain", 8);
            Core.EnsureComplete(8099);
        }

        //8100 | Tentarachnid Horde
        Story.KillQuest(8100, "downbelow", "Tentarachnid");

        //8101 | The Shadows
        Story.KillQuest(8101, "downbelow", "Creeping Shadow");

        //8102 | Keep up the Search
        if (!Story.QuestProgression(8102))
        {
            Core.EnsureAccept(8102);
            Core.GetMapItem(8493, 1, "downbelow");
            Core.HuntMonster("downbelow", "Creeping Shadow", "Monster Slain", 6);
            Core.EnsureComplete(8102);
        }

        //8103 | Defeat the Guardian
        Story.KillQuest(8103, "downbelow", "Guardian Golem");

        //8104 | Wall of Rage
        if (!Story.QuestProgression(8104))
        {
            Core.EnsureAccept(8104);
            Core.GetMapItem(8494, 1, "downbelow");
            Core.HuntMonster("downbelow", "Living Rage", "Hidden Hope", 5);
            Core.EnsureComplete(8104);
        }

        //8105 | Through the Fire
        if (!Story.QuestProgression(8105))
        {
            Core.EnsureAccept(8105);
            Core.GetMapItem(8495, 1, "downbelow");
            Core.HuntMonster("downbelow", "Living Rage", "Monsters Slain", 9);
            Core.EnsureComplete(8105);
        }

        //8106 | Heart of the Power
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(8106, "downbelow", "Anka");

        //8107 | Undying Rage
        if (!Story.QuestProgression(8107))
        {
            Core.EnsureAccept(8107);
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("downbelow", "Enter", "Spawn", "Earthwyrm", "Anka's Followers Slain", 1000, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("downbelow", "Anka", "Soul of Vengeance", 25, false);
            Core.EnsureComplete(8107);
        }
    }
}
