/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;

public class CoreSoW
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

    public string[] MainyuDrops = { "Mainyu Rune", "Mainyu Tail", "Mainyu Wings" };

    public string[] MalgorDrops =
    {
        "Shadow DragonMaster",
        "Shadow DragonMaster's Helm",
        "Shadow DragonMaster's Tail + Blades",
        "Shadow Dragonscale Blade",
        "Shadowflame Devastation",
        "ShadowFlame Dragon Blade",
        "Shadowflame Shroud",
        "Shadowflame Vanguard Armet",
        "Shadowflame Vanguard Armet Locks",
        "Shadowflame Vanguard Spear",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void CompleteCoreSoW()
    {
        ShadowWar();
        ShadowlordKeep();
        Timestream();
        GraniteCove();
        BlackseaKeep();
        Junkhoard();
        Junkheap();
        Shadowgrove();
        AozoraHills();
        GhostNexus();
        Shadowsong();
        DarkAlly();
        DarkAlliance();
        InnerShadows();
        Tyndarius();
        RuinedCrown();
        Timekeep();
        TimestreamWar();
        DeadLines();
        ShadowFlame();
        ManaCradle();
    }

    public void ShadowWar()
    {
        if (Core.isCompletedBefore(6852))
            return;

        Story.PreLoad(this);

        // Shadow Medals: Defend the Village! 6846
        Story.KillQuest(6846, "shadowwar", "Shadowflame Slasher", GetReward: false);

        //Seed Spitter Oil 6847
        Story.KillQuest(6847, "shadowwar", "Seed Spitter", GetReward: false);

        //Mega Shadow Medals 6848
        if (!Story.QuestProgression(6848))
        {
            Core.EnsureAccept(6848);
            Core.HuntMonster("shadowwar", "Shadowflame Slasher", "Mega Shadow Medal", 3);
            Core.EnsureComplete(6848);
        }
        //Shadow Samples 6849
        if (!Story.QuestProgression(6849))
        {
            Core.EnsureAccept(6849);
            Core.HuntMonster("shadowwar", "Umbral Goo", "Shadow Samples", 5);
            Core.EnsureComplete(6849);
        }

        //Shadow Medals: Fight them Back! 6850
        if (!Story.QuestProgression(6850))
        {
            Core.EnsureAccept(6850);
            Core.KillMonster("shadowwar", "r2", "Left", "*", "Shadow Medal", 5);
            Core.EnsureComplete(6850);
        }

        //Interrogation 6851 //had multiple reports made it a questprog.
        if (!Story.QuestProgression(6851))
        {
            Core.EnsureAccept(6851);
            Core.HuntMonster("shadowwar", "Shadowflame Scout", "Useful Clue", 3);
            Core.EnsureComplete(6851);
            Bot.Wait.ForQuestComplete(6851); //insurance... because y[e]
        }

        //Defeat Malgor! 6852 //quest prog, due to getting stuck on a cutscene cell.
        if (!Story.QuestProgression(6852))
        {
            Core.JumpWait();
            Bot.Options.AttackWithoutTarget = true;
            Core.EnsureAccept(6852);
            Adv.GearStore(EnhAfter: true);
            Core.UseBossClass();
            Core.HuntMonster("malgor", "Malgor", "Defeat Malgor", log: false);
            Adv.GearStore(true, EnhAfter: true);
            Bot.Options.AttackWithoutTarget = false;
            Core.EnsureComplete(6852);
            Bot.Wait.ForMapLoad("shadowwar"); //game will force you bac to shadowar-1 ._. so you're welcome.
            Core.Join("whitemap-100000");
        }
    }

    public void ShadowlordKeep()
    {
        if (Core.isCompletedBefore(6864))
            return;

        Story.PreLoad(this);

        //Defeat the Minions (6856)
        if (!Story.QuestProgression(6856))
        {
            Core.EnsureAccept(6856);
            Core.GetMapItem(6390, 1, "shadowlordkeep");
            Core.HuntMonster("shadowlordkeep", "Shadow Gunner", "Shadow Minion Defeated", 10);
            Core.EnsureComplete(6856);
        }

        //Gather the Gunpowder (6857)
        Story.KillQuest(6857, "shadowlordkeep", "Shadow Gunner");

        //Build a Bomb (6858)
        if (!Story.QuestProgression(6858))
        {
            Core.EnsureAccept(6858);
            while (
                !Bot.ShouldExit && (!Core.CheckInventory(48347) || !Core.CheckInventory(48348, 8))
            ) //Torch && Scrap of Cloth
                Core.HuntMonster("shadowlordkeep", "Shadow Mage", log: false);
            Core.EnsureComplete(6858);
        }

        //Light 'em Up! (6859)
        Story.MapItemQuest(6859, "shadowlordkeep", 6391);

        //Fight the Shadows (6860)
        Story.KillQuest(6860, "shadowlordkeep", "Shadow Imp");

        //Magical Potion (6861)
        Story.KillQuest(6861, "shadowlordkeep", "Shadow Mage");

        //Overload the Energy Field (6862)
        if (!Story.QuestProgression(6862))
        {
            Core.EnsureAccept(6862);
            Core.GetMapItem(6392, 1, "shadowlordkeep");
            Core.HuntMonster("shadowlordkeep", "Stray Energy", "Stray Energy", 8);
            Core.EnsureComplete(6862);
        }

        //Defeat the Portal Guards (6863)
        Story.KillQuest(6863, "shadowlordkeep", "Portal Guard");

        //Go Through the Portal (6864)
        Story.MapItemQuest(6864, "timestream", 6393);
    }

    public void Timestream()
    {
        if (Core.isCompletedBefore(6867))
            return;

        ShadowlordKeep();

        Story.PreLoad(this);

        //Fuel for the Cell (6865)
        Story.KillQuest(6865, "timestream", "Spacetime Energy");

        //Open the Rift (6866)
        Story.MapItemQuest(6866, "timestream", 6394);

        //Defeat the ShadowKnight (6867)
        Story.KillQuest(6867, "timestream", "ShadowKnight Gar");
    }

    public void GraniteCove()
    {
        if (Core.isCompletedBefore(6883))
            return;

        Story.PreLoad(this);

        //Get A Disguise (6873)
        Story.KillQuest(6873, "granitecove", "Blacksea Pirate");

        //Gear Up! (6874)
        Story.MapItemQuest(6874, "granitecove", 6433);

        //Gather Information (6875)
        Story.MapItemQuest(6875, "granitecove", new[] { 6434, 6435, 6436 });

        //Find the Missing Pages (6876)
        Story.KillQuest(6876, "granitecove", "Jungle Treeant");

        //Collect the Beans (6877)
        Story.KillQuest(6877, "granitecove", "Island Monkey");

        //Smooth the Way (6878)
        Story.KillQuest(6878, "granitecove", "Jungle Treeant");

        //Sweeten the Pot (6879)
        Story.KillQuest(6879, "granitecove", "Blacksea Pirate");

        //Wake the Statues (6880)
        Story.MapItemQuest(6880, "granitecove", 6438, 10);

        //Clear the Tattersail Pirates (6881)
        Story.KillQuest(6881, "granitecove", "Tattersail Pirate");

        //Kill Bill (6882)
        Story.KillQuest(6882, "granitecove", "Blacktooth Bill");

        //Defeat Teja! (6883)
        Story.KillQuest(6883, "granitecove", "Pirate Queen Teja");
    }

    public void BlackseaKeep()
    {
        if (Core.isCompletedBefore(6896))
            return;

        Story.PreLoad(this);

        //Get the Keys (6886)
        Story.KillQuest(6886, "blackseakeep", "Blacksea Scallywag");

        //Open the Chests (6887)
        if (!Story.QuestProgression(6887))
        {
            Core.EnsureAccept(6887);
            Core.GetMapItem(6446, 6, "blackseakeep");
            Core.HuntMonster("blackseakeep", "Pure Darkness", "Shadow Slain", 6);
            Core.EnsureComplete(6887);
        }

        //Interrogation Time (6888)
        Story.KillQuest(6888, "blackseakeep", "Blacksea Privateer");

        //Get the Picks (6889)
        Story.KillQuest(6889, "blackseakeep", "Blacksea Scallywag");

        //Break In! (6890)
        Story.MapItemQuest(6890, "blackseakeep", 6448);

        //Kick that Booty! (6891)
        Story.KillQuest(6891, "blackseakeep", "Blacksea Privateer");

        //We Need a Spell (6892)
        if (!Story.QuestProgression(6892))
        {
            Core.EnsureAccept(6892);
            Core.HuntMonster("blackseakeep", "BlackSea Pirate Mage", "Spell Reagents", 8);
            Core.HuntMonster("blackseakeep", "BlackSea Pirate Mage", "Compliant Mage", 1);
            Core.EnsureComplete(6892);
        }

        //Light up the Shadows (6893)
        Story.MapItemQuest(6893, "blackseakeep", 6449, 7);

        //Get the Pitch (6894)
        Story.KillQuest(6894, "blackseakeep", "Blacksea Privateer");

        //Light It Up (6895)
        Story.MapItemQuest(6895, "blackseakeep", 6450, 8);

        //Get My Eye! (6896)
        Story.KillQuest(6896, "blackseakeep", "First Mate Bloodbone");
    }

    public void Junkhoard()
    {
        if (Core.isCompletedBefore(6960))
            return;

        Story.PreLoad(this);

        //"Salute" the Magpie (6949)
        Story.KillQuest(6949, "junkhoard", "Magpie");

        //Gather the Parts (6950)
        Story.KillQuest(6950, "junkhoard", "Junk Golem");

        //Engine Needs a Spark (6951)
        Story.KillQuest(6951, "junkhoard", "Portal Manifestation");

        //Metal for Welding (6952)
        Story.KillQuest(6952, "junkhoard", "Junk Golem");

        //Feel the Heat (6953)
        Story.KillQuest(6953, "junkhoard", "Portal Manifestation");

        //Find the Forge (6954)
        Story.MapItemQuest(6954, "junkhoard", 6497);

        //Get the Coal (6955)
        Story.KillQuest(6955, "junkhoard", "Magpie");

        //Defeat the Coal Elemental (6956)
        Story.KillQuest(6956, "junkhoard", "Coal Elemental");

        //Filthy Thieving Magpies (6957)
        Story.KillQuest(6957, "junkhoard", "Magpie");

        //Track the Tracker (6958)
        Story.KillQuest(6958, "junkhoard", "Junk Golem");

        //Follow the Chakram (6959)
        Story.MapItemQuest(6959, "junkhoard", 6498);

        //Get it BACK! (6960)
        Story.KillQuest(6960, "junkhoard", "Magpie Junk Heap");
    }

    public void Junkheap()
    {
        if (Core.isCompletedBefore(6974))
            return;

        Junkhoard();

        Story.PreLoad(this);

        //Whispering Walls (6961)
        Story.MapItemQuest(6961, "junkheap", 6500, 5);

        //So Many Magpies (6962)
        Story.KillQuest(6962, "junkheap", "Magpie");

        //Charge the Portals (6963)
        if (!Story.QuestProgression(6963))
        {
            Core.EnsureAccept(6963);
            Core.GetMapItem(6501, 1, "junkheap");
            Core.HuntMonster("junkheap", "Tiny Manifestation ", "Tiny Energy", 8);
            Core.EnsureComplete(6963);
        }

        //Through the Rift (6964)
        Story.MapItemQuest(6964, "junkheap", 6502);

        //Free Niknak (6965)
        if (!Story.QuestProgression(6965))
        {
            Core.EnsureAccept(6965);
            Core.GetMapItem(6503, 1, "junkheap");
            Core.HuntMonster("junkheap", "Shadowflame Scout", "Niknak's Key", 1);
            Core.EnsureComplete(6965);
        }

        //Afraid of Shadows (6966)
        Story.KillQuest(6966, "junkheap", "Shadow Imp");

        //Wood for Splints (6967)
        Story.KillQuest(6967, "junkheap", "Dark Treeant");

        //First Aid (6968)
        if (!Story.QuestProgression(6968))
        {
            Core.EnsureAccept(6968);
            Core.HuntMonster("junkheap", "Shadow Imp", "Salve Weed", 2);
            Core.HuntMonster("junkheap", "Shadow Imp", "Healing Potion", 3);
            Core.EnsureComplete(6968);
        }

        //Delivery Time (6969)
        Story.MapItemQuest(6969, "junkheap", 6504);

        //Need Food (6970)
        if (!Story.QuestProgression(6970))
        {
            Core.EnsureAccept(6970);
            Core.HuntMonster("junkheap", "Dark Treeant", "Questionable Fruit", 5);
            Core.HuntMonster("junkheap", "Shadowflame Scout", "Canteen", 5);
            Core.EnsureComplete(6970);
        }

        //A New Coat (6971)
        Story.KillQuest(6971, "junkheap", "Shadow Imp");

        //Bruise the Bruiser (6972)
        Story.KillQuest(6972, "junkheap", "Shadowflame Bruiser");

        //Shadowy Yulgar (6973)
        Story.KillQuest(6973, "junkheap", "Shadowflame Yulgar");

        //Take the Portal (6974)
        Story.MapItemQuest(6974, "junkhoard", 6505);
    }

    public void Shadowgrove()
    {
        if (Core.isCompletedBefore(6999))
            return;

        Story.PreLoad(this);

        //Dragonlord Medals: Defend Arcangrove! (6993)
        Story.KillQuest(6993, "shadowgrove", "Shadow Dragonlord");

        //Dragonscale Powder (6994)
        Story.KillQuest(6994, "shadowgrove", "Shadow Wyvern");

        //Mega Dragonlord Medals (6995)
        Story.KillQuest(6995, "shadowgrove", "Shadow Dragonlord");

        //Dragon Oil (6996)
        Story.KillQuest(6996, "shadowgrove", "Shadow Wyvern");

        //Head of a Dragon (6997)
        Story.KillQuest(6997, "shadowgrove", "Mutant Shadow Dragon");

        //Mutant Dragon Oil (6998) - buggy mob
        if (!Story.QuestProgression(6998))
        {
            Core.EnsureAccept(6998);
            Core.KillMonster("shadowgrove", "r9", "Left", "Mutant Shadow Dragon", "Mutant Dragon Oil", 3);
            Core.EnsureComplete(6998);
        }

        //Titan Shadow Dragonlord (6999)
        Story.KillQuest(6999, "shadowgrove", "Titan Shadow Dragonlord");
    }

    public void AozoraHills()
    {
        if (Core.isCompletedBefore(7062))
            return;

        Story.PreLoad(this);

        //Gather Offerings (7050)
        if (!Story.QuestProgression(7050))
        {
            Core.EnsureAccept(7050);
            Core.GetMapItem(6643, 5, "aozorahills");
            Core.GetMapItem(6644, 5, "aozorahills");
            Core.HuntMonster("aozorahills", "Reishi", "Fresh Truffles", 5);
            Core.EnsureComplete(7050);
        }

        //Deliver the Gift (7051)
        Story.MapItemQuest(7051, "aozorahills", 6645);

        //Pest Control (7052)
        Story.KillQuest(7052, "aozorahills", "Aozora Kijimuna");

        //Prove Your Worth (7053)
        Story.KillQuest(7053, "aozorahills", "Osanshouo");

        //Light for the Lantern (7054)
        Story.KillQuest(7054, "aozorahills", "Kosenjobi");

        //Attune to Fuyurei (7055)
        if (!Story.QuestProgression(7055))
        {
            Core.EnsureAccept(7055);
            Core.GetMapItem(6646, 1, "aozorahills");
            Core.HuntMonster("aozorahills", "Fuyurei", "Fuyurei Ki", 8);
            Core.EnsureComplete(7055);
        }

        //Follow the Path (7056)
        Story.MapItemQuest(7056, "aozorahills", 6647, 3);

        //Attune to Kijimuna (7057)
        if (!Story.QuestProgression(7057))
        {
            Core.EnsureAccept(7057);
            Core.GetMapItem(6648, 1, "aozorahills");
            Core.HuntMonster("aozorahills", "Aozora Kijimuna", "Kijimuna Ki", 8);
            Core.EnsureComplete(7057);
        }

        //Path of the Kijimuna (7058)
        Story.MapItemQuest(7058, "aozorahills", 6649, 3);

        //Attune to Reishi (7059)
        if (!Story.QuestProgression(7059))
        {
            Core.EnsureAccept(7059);
            Core.GetMapItem(6650, 1, "aozorahills");
            Core.HuntMonster("aozorahills", "Reishi", "Reishi Ki", 8);
            Core.EnsureComplete(7059);
        }

        //Path to Reishi (7060)
        Story.MapItemQuest(7060, "aozorahills", 6651, 3);

        //Black Flames (7061)
        Story.KillQuest(7061, "aozorahills", "Kosenjobi");

        //Defeat Grandmother Hasu (7062)
        Story.KillQuest(7062, "aozorahills", "Ghostly Hasu");
    }

    public void GhostNexus()
    {
        if (Core.isCompletedBefore(7118))
            return;

        AozoraHills();

        Story.PreLoad(this);

        //What's That I Hear? (7106)
        Story.MapItemQuest(7106, "ghostnexus", 6700);

        //Goat Gone Wild (7107)
        Story.KillQuest(7107, "ghostnexus", "Chaos Goat");

        //Creepy Eyes (7108)
        Story.KillQuest(7108, "ghostnexus", "Chaos Sp-eye");

        //Find My Parents (7109)
        Story.MapItemQuest(7109, "ghostnexus", 6701);

        //More Food (7110)
        if (!Story.QuestProgression(7110))
        {
            Core.EnsureAccept(7110);
            Core.HuntMonster("ghostnexus", "Chaos Goat", "Goat Milk", 5);
            Core.HuntMonster("ghostnexus", "Chaos Wolf", "Wolf Meat", 5);
            Core.EnsureComplete(7110);
        }

        //Clear the Way (7111)
        Story.KillQuest(7111, "ghostnexus", "Gnarltooth");

        //Drive Them Back (7112)
        Story.KillQuest(7112, "ghostnexus", "Infernal Knight");

        //Calm the Yokai (7113)
        Story.KillQuest(7113, "ghostnexus", "Abumi Guchi");

        //Get the Supplies (7114)
        if (!Story.QuestProgression(7114))
        {
            Core.EnsureAccept(7114);
            Core.HuntMonster("ghostnexus", "Infernal Knight", "Spare Weapon", 8);
            Core.HuntMonster("ghostnexus", "Abumi Guchi", "Crate of Food", 8);
            Core.EnsureComplete(7114);
        }

        //Defeat the Nether Warlock (7115)
        Story.KillQuest(7115, "ghostnexus", "Nether Warlock");

        //So... Sad... (7116)
        Story.KillQuest(7116, "ghostnexus", "Manifestation of Grief");

        //Release the Rage (7117)
        Story.KillQuest(7117, "ghostnexus", "Manifestation of Rage");

        //Anguish is the Worst (7118)
        Story.KillQuest(7118, "ghostnexus", "Mahou's Anguish");
    }

    public void Shadowsong()
    {
        if (Core.isCompletedBefore(7348))
            return;

        Story.PreLoad(this);

        //Shadow Medal (7340)
        if (!Bot.Quests.IsUnlocked(7341))
        {
            Core.EnsureAccept(7340);
            while (!Bot.ShouldExit && !Core.CheckInventory(53063, 5))
                Core.HuntMonster("shadowsong", "Shadowflame Troll", log: false);
            Core.EnsureComplete(7340);
        }

        //Song of Fate (7341)
        Story.KillQuest(7341, "shadowsong", "Tune-A-Fish");

        //Broken Strings (7342)
        Story.KillQuest(7342, "shadowsong", "Beatle");

        //Shattered Crystal (7343)
        Story.KillQuest(7343, "shadowsong", "Shattered Crystal");

        //Mega Shadow Medals (7345)
        Core.EnsureAccept(7344);
        Core.HuntMonster("shadowsong", "Shadowflame Ur-Troll", "Mega Shadow Medal", 3);
        Core.EnsureComplete(7344);

        //New Music (7345)
        Story.KillQuest(7345, "shadowsong", "Mozard");

        //Listen to the Drums (7346)
        Story.KillQuest(7346, "shadowsong", "Pachelbel's Cannon");

        //Strength in Music (7347)
        Story.KillQuest(7347, "shadowsong", "Mozard");

        //Stop the Music (7348)
        Story.KillQuest(7348, "shadowsong", "Oh'Garr");
    }

    public void DarkAlly()
    {
        if (Core.isCompletedBefore(7428))
            return;

        Story.PreLoad(this);

        //Shadows Encroach
        if (!Story.QuestProgression(7419))
        {
            Story.MapItemQuest(7419, "darkally", 7179, 6);
            Core.KillMonster("darkally", "r2", "Left", "Shadow", "Shadow Destroyed", 10);
            Core.EnsureComplete(7419);
        }

        if (!Story.QuestProgression(7420))
        {
            Core.EnsureAccept(7420);
            Core.HuntMonster("darkally", "Underworld Golem", "Underworld Granite", 8);
            Core.HuntMonster("darkally", "Underworld Golem", "Underworld Diamond", 8);
            Core.EnsureComplete(7420);
        }

        Story.MapItemQuest(7421, "darkally", 7180, 1);
        Story.MapItemQuest(7421, "darkally", 7181, 8);

        if (!Story.QuestProgression(7422))
        {
            Core.EnsureAccept(7422);
            Core.Join("Darkally", "r2", "Left");
            while (!Bot.ShouldExit && !Core.CheckInventory(53855, 10))
                Core.KillMonster("Darkally", "r2", "Left", "Dark Makai");
            Core.EnsureComplete(7422);
        }

        if (!Story.QuestProgression(7423))
        {
            Core.EnsureAccept(7423);
            Core.KillMonster("Darkally", "r2", "Left", 4452, "Shredded Shadow", 9);
            Core.EnsureComplete(7423);
        }

        if (!Story.QuestProgression(7424))
        {
            Core.EnsureAccept(7424);
            Core.HuntMonster("Darkally", "Legion Defector", "Defectors Slain", 10);
            Core.HuntMonster("Darkally", "Legion Defector", "Legion Soul", 5);
            Core.EnsureComplete(7424);
        }

        Story.KillQuest(7425, "darkally", "Frozen Pyromancer");

        Story.KillQuest(7426, "darkally", "Underworld Golem");

        Story.MapItemQuest(7427, "darkally", 7182, 1);

        Story.KillQuest(7428, "darkally", "Underfiend");
    }

    public void DarkAlliance()
    {
        if (Core.isCompletedBefore(7460))
            return;

        DarkAlly();
        Story.PreLoad(this);

        //Clear the Shadows --DAGE--
        if (!Story.QuestProgression(7446))
        {
            Core.EnsureAccept(7446);
            Core.GetMapItem(7224, 8, "darkalliance");
            Core.HuntMonster("darkalliance", "Shadow", "Shadow Slain", 8);
            Core.EnsureComplete(7446);
        }
        //Destroy the Swords
        if (!Story.QuestProgression(7447))
        {
            Core.EnsureAccept(7447);
            Core.GetMapItem(7225, 1, "darkalliance");
            Core.HuntMonster("darkalliance", "Shadowblade", "Shadow Sword Slain", 7);
            Core.EnsureComplete(7447);
        }
        //Fuel for the Forge
        Story.KillQuest(7448, "darkalliance", "Shadow Void");
        //Sever the Connection
        Story.KillQuest(7449, "darkalliance", "Shadow Makai");
        //Armor Against the Shadow
        Story.KillQuest(7450, "darkalliance", "Shadow Void");
        //Aid from the Pyromancer
        Story.MapItemQuest(7451, "darkalliance", 7226, 8);
        //Find the Underflame --Frozen Pyromancer--
        Story.MapItemQuest(7452, "darkalliance", 7227, 1);
        //Gather some Underlava
        Story.KillQuest(7453, "darkalliance", "Underlava");
        //Gather Sulfur
        Story.KillQuest(7454, "darkalliance", "Underworld Imp");
        //Coal for Flame
        Story.MapItemQuest(7455, "darkalliance", 7228, 6);
        //Defeat the Underflame Guardian
        Story.KillQuest(7456, "darkalliance", "Underflame Guardian");
        //Take the Flame
        Story.MapItemQuest(7457, "darkalliance", 7229, 1);
        //Remove the Shadows --dage--
        Story.KillQuest(7458, "darkalliance", "Shadow");
        //Burn the Shadows
        Story.MapItemQuest(7459, "darkalliance", 7230, 1);
        //Defeat Shadow Nulgath
        Story.KillQuest(7460, "darkalliance", "ShadowFlame Nulgath");
    }

    public void InnerShadows()
    {
        if (Core.isCompletedBefore(7472))
            return;

        DarkAlliance();
        Story.PreLoad(this);

        //Gather the Shadows (7461)
        Story.KillQuest(7461, "innershadows", "Infected Minion");

        //Dusty Boost (7462)
        if (!Story.QuestProgression(7462))
        {
            Core.EnsureAccept(7462);
            Core.GetMapItem(7271, 1, "innershadows");
            Core.HuntMonster("innershadows", "Infected Minion", "Dry Bones", 8);
            Core.EnsureComplete(7462);
        }

        //Defeat my Minions (7463)
        Story.KillQuest(7463, "innershadows", "Infected Minion");

        //Pass the Portal (7464)
        Story.MapItemQuest(7464, "innershadows", 7272);

        //We Need to See (7465)
        Story.MapItemQuest(7465, "innershadows", 7273, 8);

        //Stop the Crows (7466)
        Story.KillQuest(7466, "innershadows", "Shadowcrow");

        //Convert the Shadowscythe (7467)
        Story.KillQuest(7467, "innershadows", "Shadow ShadowScythe");

        //Feed the Doomblade (7468)
        Story.KillQuest(7468, "innershadows", "Infected Minion");

        //Speak to the Ghosts (7469)
        Story.MapItemQuest(7469, "innershadows", 7274, 5);

        //Gather Spell Reagents (7470)
        if (!Story.QuestProgression(7470))
        {
            Core.EnsureAccept(7470);
            Core.HuntMonster("innershadows", "ShadowSpitter", "Blackened Herbs", 7);
            Core.HuntMonster("innershadows", "Shadowcrow", "Shadowy Feathers", 7);
            Core.EnsureComplete(7470);
        }

        //Follow the Trail (7471)
        Story.MapItemQuest(7471, "innershadows", 7275, 5);

        //Defeat Krahen (7472)
        Story.KillQuest(7472, "innershadows", "Krahen");
    }

    public void Tyndarius(bool WarTrainingMerge = false)
    {
        if (Core.isCompletedBefore(8243))
            return;

        InnerShadows();

        //War Medals
        Story.KillQuest(8125, "fireplanewar", "Shadowflame Soldier");

        // Mega War Medals
        Story.KillQuest(8126, "fireplanewar", "Shadowflame Soldier");

        // Shadowflame Un - slaught
        Story.KillQuest(8127, "fireplanewar", "Shadefire Onslaught");

        // Soldier On
        Story.KillQuest(8128, "fireplanewar", "Shadowflame Soldier");

        // Fanning the Flames
        if (!Story.QuestProgression(8129))
        {
            Core.EnsureAccept(8129);
            Core.GetMapItem(8523, 5, "fireplanewar");
            Core.HuntMonster("fireplanewar", "Shadowflame Soldier", "Battles Turned", 5);
            Core.EnsureComplete(8129);
        }

        // Trailblazer
        Story.KillQuest(8130, "fireplanewar", "Shadefire Onslaught");

        // ShadowClaw
        Story.KillQuest(8131, "fireplanewar", "ShadowClaw");

        // In Their Element
        Story.KillQuest(8132, "fireplanewar", "Shadefire Elemental");

        // Cure the Fire
        if (!Story.QuestProgression(8133))
        {
            Core.EnsureAccept(8133);
            Core.GetMapItem(8524, 5, "fireplanewar");
            Core.HuntMonster("fireplanewar", "Living Shadowflame", "Shadowflame Removed", 5);
            Core.EnsureComplete(8133);
        }

        // Human Torch
        Story.KillQuest(8134, "fireplanewar", "Living Shadowflame");

        // ShadowFlame Phedra
        Story.KillQuest(8135, "fireplanewar", "ShadowFlame Phedra");

        // Gather Fuel
        Story.KillQuest(8136, "shadowfireplane", "Living Shadowflame");

        // Attune to Play
        Story.MapItemQuest(8137, "shadowfireplane", 8544, 5);

        // Sparks Will Fly
        Story.KillQuest(8138, "shadowfireplane", "Onslaught Knight");

        // Awaken Lady Fiamme
        Story.MapItemQuest(8139, "shadowfireplane", 8542);
        Core.Sleep(5000);

        // Destroy the Barrier
        if (!Story.QuestProgression(8140))
        {
            Core.Join("shadowfireplane", "r6", "Left"); // for incase u start here
            Core.EnsureAccept(8140);
            Core.GetMapItem(8543);
            Core.KillMonster("shadowfireplane", "r6", "Left", "Shadow Wing", "Shadow Flamewing Defeated", 2);
            Core.KillMonster("shadowfireplane", "r6", "Left", "Shadowfire Summoner", "Shadowfire Summoner Defeated", 1);
            Core.EnsureComplete(8140);
        }

        // Blaze a Path
        if (!Story.QuestProgression(8141))
        {
            Core.EnsureAccept(8141);
            Core.HuntMonster("shadowfireplane", "Shadowfire Corporal", "Shadowfire Corporal Defeated", 5);
            Core.HuntMonster("shadowfireplane", "Onslaught Knight", "Onslaught Knight Defeated", 5);
            Core.EnsureComplete(8141);
        }

        // Into the Tiger's Den
        Story.KillQuest(8142, "shadowfireplane", "Shadowfire Tiger");

        // One Final Push
        Story.KillQuest(8143, "shadowfireplane", "Shadowfire Corporal");

        // Defeat Elius
        Story.KillQuest(8144, "shadowfireplane", "Elius");

        // Elementary Defense
        Story.KillQuest(8179, "fireinvasion", "Onslaught Knight");

        // Tiger Burning Bright
        if (!Story.QuestProgression(8180))
        {
            Core.EnsureAccept(8180);
            Core.GetMapItem(8728, 3, "fireinvasion");
            Core.HuntMonster("fireinvasion", "Shadowfire Tiger", "Tiger Slain", 10);
            Core.EnsureComplete(8180);
        }

        // Crush the Cavalry
        Story.KillQuest(8181, "fireinvasion", "Shadefire Cavalry");

        // Capture the Corporals
        Story.KillQuest(8182, "fireinvasion", "Shadowfire Corporal");

        // Light up the Night
        if (!Story.QuestProgression(8183))
        {
            Core.EnsureAccept(8183);
            Core.GetMapItem(8729, 6, "fireinvasion");
            Core.HuntMonster("fireinvasion", "Shadefire Elemental", "Shadefire Elemental Slain", 7);
            Core.EnsureComplete(8183);
        }

        // Major Malfunction
        Story.KillQuest(8184, "fireinvasion", "Shadefire Major");

        // Darkness in Swordhaven
        if (!Story.QuestProgression(8185))
        {
            Core.EnsureAccept(8185);
            Core.KillMonster("fireinvasion", "r8", "Top", "Shadefire Elemental", "Elemental Slain", 7);
            Core.KillMonster("fireinvasion", "r7", "Top", "Shadowfire Tiger", "Tiger Slain", 7);
            Core.EnsureComplete(8185);
        }

        // Extinguish the Flames
        Story.KillQuest(8186, "fireinvasion", "Living Shadowflame");

        // ShadeFire Colonel Clash
        Story.KillQuest(8187, "fireinvasion", "Shadefire Colonel");

        // Defense of Embersea
        Story.KillQuest(8188, "fireinvasion", "Living Shadowflame");

        // A Chilling Conflict
        Story.KillQuest(8189, "fireinvasion", "Onslaught Knight");

        // General Dismay
        Story.KillQuest(8190, "fireinvasion", "Shadefire General");

        //A Fallen Friend
        Story.KillQuest(8191, "fireinvasion", "Shadowflame Kyron");

        // Fire Fighting
        if (!Story.QuestProgression(8192))
        {
            Core.EnsureAccept(8192);
            Core.HuntMonster("fireinvasion", "Living Shadowflame", "ShadowFlame Tag", 15);
            Core.HuntMonster("fireinvasion", "Shadefire Cavalry", "Corrupted Badge", 3);
            Core.EnsureComplete(8192);
        }

        // Shadefires of War
        Story.KillQuest(8193, "wartraining", "Simulated Shadefire");

        // Extinguish the Shadowflame
        Story.MapItemQuest(8194, "wartraining", 8746, 4);

        // A Major Pain in the Neck
        Story.KillQuest(8195, "wartraining", "Simulated Major");

        // The Envoy of Fire
        Story.KillQuest(8196, "wartraining", "Simulated Elius");

        // Putting Out Small Fires
        Story.KillQuest(8197, "wartraining", "Simulated Fire");

        // Element - ary
        Story.KillQuest(8198, "wartraining", "Simulated Elemental");

        // A Dragonslayer's Past
        Story.KillQuest(8199, "wartraining", "Simulated Fire");

        // The Champion of Fire
        Story.KillQuest(8200, "wartraining", "Fire Champion");

        // March of the Warfury
        Story.KillQuest(8201, "wartraining", "Warfury Soldier");

        // Elite It to Beat It
        Story.KillQuest(8202, "wartraining", "Warfury Elite");

        //The Goddess of War
        Story.KillQuest(8203, "wartraining", "Varga");
        if (WarTrainingMerge)
            return;

        // Warfury Training
        Story.KillQuest(8204, "wartraining", "Warfury Soldier");

        //Colonel Vanguard 8233
        Story.KillQuest(8233, "fireavatar", "Shadefire Colonel");

        //In a Major Way 8234
        Story.KillQuest(8234, "fireavatar", "Shadefire Major");

        //Elemental Backup 8235
        Story.MapItemQuest(8235, "fireavatar", 8859, 4);

        //The Cavalry's Here 8236
        Story.KillQuest(8236, "fireavatar", "Shadefire Cavalry");

        //Path of Flame 8237
        Story.KillQuest(8237, "fireavatar", "Shadefire Colonel");

        //The Envoy of Fire 8238
        Story.KillQuest(8238, "fireavatar", "Elius");

        //Living in Shadowflame 8239
        Story.KillQuest(8239, "fireavatar", "Living Shadowflame");

        //Elemental of Surprise 8240
        Story.KillQuest(8240, "fireavatar", "Shadefire Elemental");

        //Well That’s New 8241
        if (!Story.QuestProgression(8241))
        {
            Core.EnsureAccept(8241);
            Core.KillMonster("fireavatar", "r8", "Left", "Shadow Lava", "Shadow Lava Defeated", 8);
            Core.EnsureComplete(8241);
        }

        //Thermal Energy 8242
        if (!Story.QuestProgression(8242))
        {
            Core.EnsureAccept(8242);
            Core.KillMonster("fireavatar", "r7", "Left", "Living Shadowflame", "Power Restored", 10);
            Core.EnsureComplete(8242);
        }

        //Avatar of Fire 8243
        if (!Story.QuestProgression(8243))
        {
            Core.EnsureAccept(8243);
            Core.HuntMonster("fireavatar", "Fire Orb", "Fire Orb Defeated", 2);
            Core.HuntMonster("fireavatar", "Avatar Tyndarius", "Avatar Tyndarius Defeated", 1);
            Core.EnsureComplete(8243);
        }
    }

    public void RuinedCrown()
    {
        if (Core.isCompletedBefore(8787))
            return;

        // 8778 Mental Damage Sponge
        Story.MapItemQuest(8778, "ruinedcrown", new[] { 10380, 10382, 10383 });

        // 8779 Scraping the Barrel
        if (!Story.QuestProgression(8779))
        {
            Core.EnsureAccept(8779);
            Core.HuntMonster("ruinedcrown", "Mana-Burdened Knight", "Busted Helmet", 7);
            Core.HuntMonster("ruinedcrown", "Mana-Burdened Minion", "Brittle Bone", 7);
            Core.EnsureComplete(8779);
        }

        // 8780 Fractals
        Story.MapItemQuest(8780, "ruinedcrown", 10384, 6);

        // 8781 Blind Retaliation
        Story.KillQuest(8781, "ruinedcrown", "Mana-Burdened Mage");

        // 8782 Deafening Silence
        if (!Story.QuestProgression(8782))
        {
            Core.EnsureAccept(8782);
            Core.HuntMonster("ruinedcrown", "Mana-Burdened Knight", "Swordhaven Crest", 7);
            Core.HuntMonster("ruinedcrown", "Mana-Burdened Minion", "Shadowscythe Crest", 7);
            Core.EnsureComplete(8782);
        }

        // 8784 Stilled Mind (Yes 8784 before 8783)
        Story.MapItemQuest(8784, "ruinedcrown", 10385, 6);

        // 8783 Volatile Nature
        Story.KillQuest(8783, "ruinedcrown", "Frenzied Mana");

        // 8785 Heartache
        Story.KillQuest(8785, "ruinedcrown", "Mana-Burdened Mage");

        // 8786 Clouded Vision
        if (!Story.QuestProgression(8786))
        {
            Core.EnsureAccept(8786);
            Core.GetMapItem(10386, 1, "ruinedcrown");
            Core.HuntMonster("ruinedcrown", "Frenzied Mana", "Frienzied Mana Cleared", 6);
            Core.EnsureComplete(8786);
        }

        // 8787 Guilt Complex
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(8787, "ruinedcrown", "Calamitous Warlic");

        // 8788 Em-pathetic Connection (Merge Shop Quest)
        Core.EnsureAccept(8788);

        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster("ruinedcrown", "Frenzied Mana", "Mana Residue", 8);
        Core.HuntMonster($"ruinedcrown", "Mana-Burdened Mage", "Mage's Blood Sample", 8);

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster($"ruinedcrown", "Calamitous Warlic", "Warlic's Favor");
        Core.EnsureComplete(8788);
    }

    public void Timekeep()
    {
        if (Core.isCompletedBefore(8813))
            return;

        RuinedCrown();

        // 8803|Mood Pendulum
        Story.MapItemQuest(8803, "Timekeep", new[] { 10455, 10456, 10457 });

        // 8804|Bug Issue
        Story.KillQuest(8804, "Timekeep", "Decaying Locust");

        // 8805|Advanced Age
        Story.KillQuest(8805, "Timekeep", "Distorted Imp");

        // 8806|One Singularity
        Story.MapItemQuest(8806, "Timekeep", 10458, 6);

        // 8807|Canaries on Edge
        Story.KillQuest(8807, "Timekeep", "Mana-Burdened Mage");

        // 8808|Caution! Wet Floor
        Story.MapItemQuest(8808, "Timekeep", new[] { 10459, 10460 });

        // 8809|Reflection in the Puddle
        Story.KillQuest(8809, "Timekeep", "Mumbler");

        // 8810|Meniscus Point
        if (!Story.QuestProgression(8810))
        {
            Core.EnsureAccept(8810);
            Core.HuntMonster("timekeep", "Distorted Imp", "Imp Bones", 7);
            Core.HuntMonster("timekeep", "Mana-Burdened Mage", "Mage Quelled", 7);
            Core.EnsureComplete(8810);
        }

        // 8811|Distractions
        if (!Story.QuestProgression(8811))
        {
            Core.EnsureAccept(8811);
            Core.HuntMonster("timekeep", "Mumbler", "Mumbler Crushed", 7);
            Core.HuntMonster("timekeep", "Decaying Locust", "Locust Smashed", 7);
            Core.EnsureComplete(8811);
        }

        // 8812|Dog Bites Back
        Story.KillQuest(8812, "Timekeep", "Mal-formed Gar");

        // 8813|Janitorial Duties
        if (!Story.QuestProgression(8813))
        {
            Core.EnsureAccept(8813);
            Core.HuntMonster("timekeep", "Mal-formed Gar", "Gar's Resignation Letter", 1);
            Core.HuntMonster("timekeep", "Mumbler", "Mumbler Drool", 8);
            Core.HuntMonster("timekeep", "Decaying Locust", "Locust Wings", 8);
            Core.EnsureComplete(8813);
        }
    }

    public void TimestreamWar()
    {
        if (Core.isCompletedBefore(8819))
            return;

        Timekeep();

        // 8814|Timestream Medals (dont need to do the mega metals)
        if (!Bot.Quests.IsUnlocked(8816))
        {
            Core.EnsureAccept(8814);
            Core.HuntMonster("Streamwar", "Decaying Locust", "Timestream Medal", 5);
            Core.EnsureComplete(8814);
        }

        // 8816|Teary Components
        Story.KillQuest(8816, "Streamwar", "Mumbler");

        // 8817|Proportional Retaliation
        Story.KillQuest(8817, "Streamwar", "Decaying Locust");

        //Growing Pains (8818)
        Story.KillQuest(8818, "streamwar", "False Wyvern");

        //Middle Child (8819)
        Story.KillQuest(8819, "streamwar", "Second Speaker");
    }

    public void DeadLines()
    {
        if (Core.isCompletedBefore(8868))
            return;

        TimestreamWar();

        Story.PreLoad(this);

        //Falling Apart 8859
        Story.MapItemQuest(8859, "DeadLines", 10601, 6);

        //Baby Steps 8860
        if (!Story.QuestProgression(8860))
        {
            Core.EnsureAccept(8860);
            Core.GetMapItem(10602, 1, "DeadLines");
            Core.HuntMonster("deadlines", "Frenzied Mana", "Mana Dispelled", 6);
            Core.EnsureComplete(8860);
        }

        //Eternal Flame 8861
        if (!Story.QuestProgression(8861))
        {
            Core.EnsureAccept(8861);
            Core.GetMapItems(new[] { (10603, 1), (10604, 1) }, "DeadLines");
            Core.HuntMonster("deadlines", "Shadowfall Warrior", "Shadowfall Helm", 7);
            Core.EnsureComplete(8861);
        }

        //The Wolf Cries 8862
        if (!Story.QuestProgression(8862))
        {
            Core.EnsureAccept(8862);
            Core.GetMapItems(new[] { (10605, 1), (10606, 1) }, "DeadLines");
            Core.HuntMonster("deadlines", "Swordhaven Knight", "Knight's Crest", 7);
            Core.EnsureComplete(8862);
        }

        //Breaking Down 8863
        if (!Story.QuestProgression(8863))
        {
            Core.EnsureAccept(8863);
            Core.GetMapItems(new[] { (10607, 1), (10608, 1) }, "DeadLines");
            Core.HuntMonster("deadlines", "Shadowfall Warrior", "Shadowfall Bones", 7);
            Core.EnsureComplete(8863);
        }

        //Growth Through Hardship 8864
        if (!Story.QuestProgression(8864))
        {
            Core.EnsureAccept(8864);
            Core.GetMapItems(new[] { (10609, 1), (10610, 1) }, "DeadLines");
            Core.HuntMonster("deadlines", "Swordhaven Knight", "Knight's Armaments", 7);
            Core.EnsureComplete(8864);
        }

        //Arteries 8865
        if (!Story.QuestProgression(8865))
        {
            Core.EnsureAccept(8865);
            Core.GetMapItems(new[] { (10611, 1), (10612, 1) }, "DeadLines");
            Core.HuntMonster("deadlines", "Frenzied Mana", "Mana Dissipated", 6);
            Core.EnsureComplete(8865);
        }

        //Paranoiac 8866
        if (!Story.QuestProgression(8866))
        {
            Core.EnsureAccept(8866);
            Core.GetMapItem(10613, 1, "DeadLines");
            Core.HuntMonster("deadlines", "Chaos Mage", "Mage Hoods", 6);
            Core.EnsureComplete(8866);
        }

        //A Hard Swerve 8867
        if (!Story.QuestProgression(8867))
        {
            Core.EnsureAccept(8867);
            Core.GetMapItem(10614, 1, "DeadLines");
            Core.HuntMonster("deadlines", "Frenzied Mana", "Mana Cleared", 6);
            Core.EnsureComplete(8867);
        }

        //Nigh Invincible 8868
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(8868, "DeadLines", "Eternal Dragon");
    }

    public void ShadowFlame()
    {
        if (Core.isCompletedBefore(8965))
            return;

        DeadLines();

        Story.PreLoad(this);

        // Lies in Peace 8956
        Core.EquipClass(ClassType.Solo);
        Story.MapItemQuest(8956, "worldscore", 10877);
        Story.KillQuest(8956, "worldscore", "False Wyvern");

        // There Was an Attempt 8957
        if (!Story.QuestProgression(8957))
        {
            Core.EnsureAccept(8957);
            Core.GetMapItem(10878, 5, "worldscore");
            Core.HuntMonster("worldscore", "Elemental Attempt", "Pulsating Mana Residue", 12);
            Core.EnsureComplete(8957);
        }

        // Scared Straight 8958
        if (!Story.QuestProgression(8958))
        {
            Core.EnsureAccept(8958);
            Core.GetMapItem(10879, 1, "worldscore");
            Core.HuntMonster("worldscore", "Infernal Mockery", "Waxen Horns", 12);
            Core.EnsureComplete(8958);
        }

        // Making Waves 8959
        if (!Story.QuestProgression(8959))
        {
            Core.EnsureAccept(8959);
            Core.GetMapItem(10880, 4, "worldscore");
            Core.HuntMonster("worldscore", "Infernal Mockery", "Infernal Shoved", 10);
            Core.EnsureComplete(8959);
        }

        // In Circles 8960
        if (!Story.QuestProgression(8960))
        {
            Core.EnsureAccept(8960);
            Core.GetMapItem(10881, 1, "worldscore");
            Core.HuntMonster("worldscore", "Elemental Attempt", "Vanquished Elemental", 14);
            Core.EnsureComplete(8960);
        }

        // Nameless Remnants 8961
        Story.MapItemQuest(8961, "worldscore", 10882, 4);

        // Dreams Unborn 8962
        Story.KillQuest(8962, "worldscore", "Crystalized Mana");

        // Innocent Crafts 8963
        if (!Story.QuestProgression(8963))
        {
            Core.EnsureAccept(8963);
            Core.GetMapItem(10883, 1, "worldscore");
            Core.HuntMonster("worldscore", "Infernal Mockery", "Rubbery Wings", 14);
            Core.EnsureComplete(8963);
        }

        // Cold Bedrock 8964
        if (!Story.QuestProgression(8964))
        {
            Core.EnsureAccept(8964);
            Core.GetMapItem(10884, 1, "worldscore");
            Core.HuntMonster("worldscore", "Elemental Attempt", "Sterile Mana", 16);
            Core.EnsureComplete(8964);
        }

        // Rippling Heartbeat 8965
        Story.KillQuest(8965, "worldscore", "Mask of Tranquility");
    }

    public void ManaCradle()
    {
        if (Core.isCompletedBefore(9126))
            return;

        ShadowFlame();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Solo);

        // Stunted Growth 9116
        Story.KillQuest(9116, "manacradle", "Elemental Attempt");

        // Tossing and Turning 9117
        Story.KillQuest(9117, "manacradle", "Crystalized Mana");

        // Dark Intruder 9118
        Story.MapItemQuest(9118, "manacradle", 11268, 3);
        Story.MapItemQuest(9118, "manacradle", 11269, 3);

        // Doggy Paddle 9119
        if (!Story.QuestProgression(9119))
        {
            Core.EnsureAccept(9119);
            Core.HuntMonster("manacradle", "Crystalized Mana", "Crystal Shard", 8);
            Core.HuntMonster("manacradle", "Elemental Attempt", "Elemental Drops", 8);
            Core.EnsureComplete(9119);
        }

        // Amethyst Memories 9120
        Story.KillQuest(9120, "manacradle", "Dark Tainted Mana");

        // Crawling Darkness 9121
        Story.KillQuest(9121, "manacradle", "Darkness Elemental");

        // Raised Veins 9122
        Story.MapItemQuest(9122, "manacradle", 11270, 5);

        // In The Eye 9123
        if (!Story.QuestProgression(9123))
        {
            Core.EnsureAccept(9123);
            Core.GetMapItem(11271, 1, "manacradle");
            Core.HuntMonster("manacradle", "Darkness Elemental", "Dark Shard", 10);
            Core.HuntMonster("manacradle", "Dark Tainted Mana", "Dark Residue", 10);
            Core.EnsureComplete(9123);
        }


        // If Dodge Class was found, kill Malgor & The Mainyu with it, else use what you have on.
        Core.EquipClass(ClassType.Dodge);

        // Cognitive Dissonance 9124
        Story.KillQuest(9124, "manacradle", "Malgor");

        // Your Hero 9125
        Story.KillQuest(9125, "manacradle", "The Mainyu");

        Core.ToBank(MainyuDrops.Concat(MalgorDrops).ToArray());
        #region GroupBoss Quest
        // These 2 arent needed.
        // Once Upon Another Time 9126
        // Build Malgor's Armor Set 9127
        #endregion
        Adv.GearStore(true, EnhAfter: true);
    }
}
