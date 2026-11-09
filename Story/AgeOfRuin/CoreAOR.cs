/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;

public class CoreAOR
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;
    private bool _voiceChargeDetected;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void DoAll()
    {
        TerminaTemple(true, true);
        AshrayVillage(false, false);
        SunlightZone(false, false);
        TwilightZone(false, false);
        YulgarAria(false, false);
        MidnightZone(false, false);
        AbyssalZone(false, false);
        DeepWater(false, false);
        SeaVoice();
        Balemorale(false, false);
        Castleeblana(false, false);
        Loughshine(false, false);
        NaoiseGrave(false, false);
        LiaTaraHill(false, false);
        CastleGaheris(false, false);
        ColdThunder();
        ThelimaCity(false, false);
        MountMaleno(false, false);
        SanctuaryAiwass(false, false);
        ForgeAlbedo(false, false);
        FortLuma(false, false);
        WarWickForest(false, false);
        MagnumOpus(false, false);
        RubedoPeak(false, false);
        ForgeCitrinitas(false, false);

    }

    public void TerminaTemple(bool seaVoice = false, bool coldThunder = false)
    {
        if (
            Core.isCompletedBefore(
                coldThunder ? 9851
                : seaVoice ? 9351
                : 10438
            )
        )
            return;

        SoW.ManaCradle();
        Story.PreLoad(this);

        // Familiar Faces (9213)
        if (!Story.QuestProgression(9213))
        {
            Core.EnsureAccept(9213);
            Core.GetMapItems(new[] { (11625, 1), (11626, 1), (11627, 1) }, "terminatemple");
            Core.Logger("resetting map data ( mobs can bug out and become unattackable)");
            Core.Join("whitemap-100000");
            Core.Join("terminatemple");
            Core.HuntMonster("terminatemple", "Termina Defender", "Defender Trained", 7);
            Core.EnsureComplete(9213);
        }

        // Loaded Resume (9214)
        if (!Story.QuestProgression(9214))
        {
            Core.EnsureAccept(9214);
            Core.GetMapItems(new[] { (11628, 1), (11629, 1), (11630, 1) }, "terminatemple");
            Core.HuntMonster("terminatemple", "Clandestine Guard", "Guard Fought", 7);
            Core.EnsureComplete(9214);
        }

        if (!seaVoice && !coldThunder)
            return;

        // If seaVoice or coldThunder & quest isnt unlocked
        if (seaVoice || coldThunder && !Core.isCompletedBefore(9348))
            SeaVoice();

        // Mopping Up (9351)
        Story.MapItemQuest(9351, "terminatemple", new[] { 12050, 12051 });

        if (seaVoice && !coldThunder)
            return;

        if (coldThunder)
            ColdThunder();

        // Tell-Tale Heart (9851)
        Story.MapItemQuest(9851, "terminatemple", new[] { 13541, 13542 });
    }

    public void AshrayVillage(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9234))
            return;

        TerminaTemple(seaVoice, coldThunder);

        // Big ol' Eyes (9225)
        Story.KillQuest(9225, "ashray", "Kitefin Shark Bait");

        // Angry Angler (9226)
        if (!Story.QuestProgression(9226))
        {
            Core.EnsureAccept(9226);
            Core.GetMapItems(new[] { (11663, 1), (11664, 1) }, "ashray");
            Core.HuntMonster("ashray", "Ashray Fisherman", "Fisherman's Trident", 4);
            Core.EnsureComplete(9226);
        }

        // Slimy Scavenger (9227)
        Story.KillQuest(9227, "ashray", "Ghostly Eel");

        // Troubled Waters (9228)
        if (!Story.QuestProgression(9228))
        {
            Core.EnsureAccept(9228);
            Core.GetMapItem(11665, 1, "ashray");
            Core.HuntMonster("ashray", "Stagnant Water", "Elemental Cleared", 8);
            Core.EnsureComplete(9228);
        }

        // Washed Ink (9229)
        if (!Story.QuestProgression(9229))
        {
            Core.EnsureAccept(9229);
            Core.GetMapItem(11666, 1, "ashray");
            Core.HuntMonster("ashray", "Ashray Fisherman", "Fisherman Repelled", 8);
            Core.EnsureComplete(9229);
        }

        // Fishy Hospitality (9230)
        Story.KillQuest(9230, "ashray", "Kitefin Shark Bait");

        // Doctoring Papers (9231)
        if (!Story.QuestProgression(9231))
        {
            Core.EnsureAccept(9231);
            Core.GetMapItem(11667, 1, "ashray");
            Core.HuntMonster("ashray", "Ghostly Eel", "Excorsized Eel", 8);
            Core.EnsureComplete(9231);
        }

        // Psychic Pollution (9232)
        if (!Story.QuestProgression(9232))
        {
            Core.EnsureAccept(9232);
            Core.GetMapItem(11668, 1, "ashray");
            Core.HuntMonster("ashray", "Stagnant Water", "Water Samples", 3);
            Core.EnsureComplete(9232);
        }

        // Duck Dive (9233)
        Story.MapItemQuest(9233, "ashray", new[] { 11669, 11670 });

        // Faces in the Foam (9234)
        Story.KillQuest(9234, "ashray", "Seafoam Elemental");
    }

    public void SunlightZone(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9251))
            return;

        AshrayVillage(seaVoice, coldThunder);

        // Detergent Shortage (9242)
        Story.KillQuest(9242, "sunlightzone", "Blighted Water");

        // Ghost in the Machine (9243)
        Story.KillQuest(9243, "sunlightzone", "Spectral Jellyfish");

        // Efficient Division (9244)
        if (!Story.QuestProgression(9244))
        {
            Core.EnsureAccept(9244);
            Core.GetMapItems(new[] { (11705, 1), (11706, 1) }, "sunlightzone");
            Core.HuntMonster("sunlightzone", "Blighted Water", "Mopped Water", 6);
            Core.EnsureComplete(9244);
        }

        // Tech Illiterate (9245)
        if (!Story.QuestProgression(9245))
        {
            Core.EnsureAccept(9245);
            Core.GetMapItem(11707, 3, "sunlightzone");
            Core.HuntMonster("sunlightzone", "Spectral Jellyfish", "Jelly Filling", 6);
            Core.EnsureComplete(9245);
        }

        // Plugging Leaks (9246)
        if (!Story.QuestProgression(9246))
        {
            Core.EnsureAccept(9246);
            Core.HuntMonster("sunlightzone", "Spectral Jellyfish", "Stinging Tentacles", 6);
            Core.HuntMonster("sunlightzone", "Blighted Water", "Grainy Water", 6);
            Core.EnsureComplete(9246);
        }

        // Shared History (9247)
        Story.MapItemQuest(9247, "sunlightzone", new[] { 11708, 11709, 11710 });

        // Flat Scares (9248)
        if (!Story.QuestProgression(9248))
        {
            Core.EnsureAccept(9248);
            Core.GetMapItem(11711, 1, "sunlightzone");
            Core.HuntMonster("sunlightzone", "Astravian Illusion", "Astravian Dispelled", 10);
            Core.EnsureComplete(9248);
        }

        // Fishy Bully (9249)
        if (!Story.QuestProgression(9249))
        {
            Core.EnsureAccept(9249);
            Core.GetMapItem(11712, 5, "sunlightzone");
            Core.HuntMonster("sunlightzone", "Infernal Illusion", "Infernal Dispelled", 10);
            Core.EnsureComplete(9249);
        }

        // Faint Howls (9250)
        if (!Story.QuestProgression(9250))
        {
            Core.EnsureAccept(9250);
            Core.GetMapItem(11713, 1, "sunlightzone");
            Core.HuntMonster("sunlightzone", "Seraphic Illusion", "Seraph Dispelled", 10);
            Core.EnsureComplete(9250);
        }

        // Down the Digestive Tract (9251)
        Story.KillQuest(9251, "sunlightzone", "Marine Snow");
    }

    public void TwilightZone(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9268))
            return;

        SunlightZone(seaVoice, coldThunder);

        // Marshmallows With Bite (9258)
        Story.KillQuest(9258, "twilightzone", "Whale Louse");

        // Meaty Cold Spaghetti (9259)
        Story.KillQuest(9259, "twilightzone", "Polymelia Lamprey");

        // Songs in the Seams (9260)
        Story.MapItemQuest(9260, "twilightzone", 11749);
        Story.MapItemQuest(9260, "twilightzone", 11750, 4);

        // Parched Throats (9261)
        if (!Story.QuestProgression(9261))
        {
            Core.EnsureAccept(9261);
            Core.HuntMonster("twilightzone", "Whale Louse", "Louse Antennae", 6);
            Core.HuntMonster("twilightzone", "Polymelia Lamprey", "Lamprey Tooth", 6);
            Core.EnsureComplete(9261);
        }

        // Morning Stretches (9262)
        Story.MapItemQuest(9262, "twilightzone", new[] { 11751, 11752 });

        // Natural Empathy (9263)
        Story.KillQuest(9263, "twilightzone", "Decay Spirit");

        // Comfort Blanket of Snow (9264)
        Story.KillQuest(9264, "twilightzone", "Ice Guardian");

        // Whale Watching (9265)
        Story.MapItemQuest(9265, "twilightzone", new[] { 11753, 11754, 11755 });

        // Exhausted Spirits (9266)
        if (!Story.QuestProgression(9266))
        {
            Core.EnsureAccept(9266);
            Core.HuntMonster("twilightzone", "Decay Spirit", "Spirit Restrained", 7);
            Core.HuntMonster("twilightzone", "Ice Guardian", "Guardian Restrained", 7);
            Core.EnsureComplete(9266);
        }

        // Singing to Whales (9267)
        if (!Story.QuestProgression(9267))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9267);
            Core.HuntMonster("twilightzone", "Leviathan", "Leviathan Fought");
            Core.EnsureComplete(9267);
        }

        // The Sea's Commitment (9268)
        Story.MapItemQuest(9268, "twilightzone", 11756);
    }

    public void YulgarAria(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9274))
            return;

        TwilightZone(seaVoice, coldThunder);

        // Octotree (9270)
        Story.KillQuest(9270, "twilightzone", "Polymelia Lamprey");

        // Thirsty Roots (9271)
        Story.KillQuest(9271, "sunlightzone", "Blighted Water");

        // Dollar Store Mogloween Costume (9272)
        if (!Story.QuestProgression(9272))
        {
            Core.EnsureAccept(9272);
            Core.HuntMonster("sunlightzone", "Infernal Illusion", "Infernal Dispersed", 4);
            Core.HuntMonster("sunlightzone", "Astravian Illusion", "Astravian Dispersed", 4);
            Core.EnsureComplete(9272);
        }

        // Sea Snow Angels (9273)
        Story.KillQuest(9273, "sunlightzone", "Marine Snow");

        // Ten Klicks (9274)
        if (!Story.QuestProgression(9274))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9274);
            Core.HuntMonster("twilightzone", "Leviathan", "Leviathan's Tendril", 3);
            Core.EnsureComplete(9274);
        }
    }

    public void MidnightZone(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9301))
            return;

        TwilightZone(seaVoice, coldThunder);

        // Motivation Malady (9292)
        Story.MapItemQuest(9292, "midnightzone", new[] { 11842, 11843, 11844 });

        // Radical Renovation (9293)
        if (!Story.QuestProgression(9293))
        {
            Core.EnsureAccept(9293);
            Core.GetMapItem(11845, 1, "midnightzone");
            Core.HuntMonster("midnightzone", "Polymelia Lamprey", "Lamprey Limb", 8);
            Core.EnsureComplete(9293);
        }

        // Graveyard Shift (9294)
        if (!Story.QuestProgression(9294))
        {
            Core.EnsureAccept(9294);
            Core.HuntMonster("midnightzone", "Vowed Shadowslayer", "Slayer Subdued", 8);
            Core.HuntMonster("midnightzone", "Vowed Shadowslayer", "Key Card", 1);
            Core.EnsureComplete(9294);
        }

        // Educational Execution (9295)
        if (!Story.QuestProgression(9295))
        {
            Core.EnsureAccept(9295);
            Core.GetMapItem(11846, 1, "midnightzone");
            Core.HuntMonster("midnightzone", "Undead Prisoner", "Prisoner Subdued", 10);
            Core.EnsureComplete(9295);
        }

        // Vows For Ignorance (9296)
        if (!Story.QuestProgression(9296))
        {
            Core.EnsureAccept(9296);
            Core.GetMapItem(11847, 1, "midnightzone");
            Core.HuntMonster("midnightzone", "Undead Prisoner", "Bone Sample", 6);
            Core.HuntMonster("midnightzone", "Vowed Shadowslayer", "Slayer's Written Vow", 6);
            Core.EnsureComplete(9296);
        }

        // Protein Shake (9297)
        if (!Story.QuestProgression(9297))
        {
            Core.EnsureAccept(9297);
            Core.GetMapItem(11848, 3, "midnightzone");
            Core.HuntMonster("midnightzone", "Shadow Viscera", "Viscera Sample", 6);
            Core.EnsureComplete(9297);
        }

        // Duty Beyond Death (9298)
        Story.KillQuest(9298, "midnightzone", "Venerated Wraith");

        // Designated Taunters (9299)
        if (!Story.QuestProgression(9299))
        {
            Core.EnsureAccept(9299);
            Core.HuntMonster("midnightzone", "Shadow Viscera", "Shadow Captured", 7);
            Core.HuntMonster("midnightzone", "Venerated Wraith", "Wraith Appeased", 7);
            Core.EnsureComplete(9299);
        }

        // Beloved Simulacrum (9230)
        Story.MapItemQuest(9300, "midnightzone", 11849, 4);
        Story.MapItemQuest(9300, "midnightzone", 11850);

        // Roko's Royal Basilisk (9301)
        if (!Story.QuestProgression(9301))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9301);
            Core.HuntMonster("midnightzone", "Sparagmos", "Sparagmos A.I. Defeated");
            Core.EnsureComplete(9301);
        }
    }

    public void AbyssalZone(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9315))
            return;

        MidnightZone(seaVoice, coldThunder);

        // Shark Kiting (9306)
        if (!Story.QuestProgression(9306))
        {
            Core.EnsureAccept(9306);
            Core.GetMapItem(11914, 1, "abyssalzone");
            Core.HuntMonster("abyssalzone", "Kitefin Shark Bait", "Shark Fin", 8);
            Core.EnsureComplete(9306);
        }

        // Suckered Blockade (9307)
        Story.MapItemQuest(9307, "abyssalzone", 11893, 6);
        Story.MapItemQuest(9307, "abyssalzone", 11894);

        // Digestive Fluids (9308)
        Story.KillQuest(9308, "abyssalzone", "Blighted Water");

        // Sodden Secrets (9309)
        if (!Story.QuestProgression(9309))
        {
            Core.EnsureAccept(9309);
            Core.GetMapItem(11895, 3, "abyssalzone");
            Core.HuntMonster("abyssalzone", "Shadow Viscera", "Shadow Sample", 7);
            Core.EnsureComplete(9309);
        }

        // The Hidden Corpse (9310)
        if (!Story.QuestProgression(9310))
        {
            Core.EnsureAccept(9310);
            Core.HuntMonster("abyssalzone", "Shadow Viscera", "Shadow Organs", 6);
            Core.HuntMonster("abyssalzone", "Blighted Water", "Decayed Elemental Gem", 6);
            Core.EnsureComplete(9310);
        }

        // Octo-Flake Fish Feed (9311)
        if (!Story.QuestProgression(9311))
        {
            Core.EnsureAccept(9311);
            Core.GetMapItem(11896, 1, "abyssalzone");
            Core.HuntMonster("abyssalzone", "Foam Scavenger", "Foamy Residue", 10);
            Core.EnsureComplete(9311);
        }

        // Completely Surrounded (9312)
        Story.MapItemQuest(9312, "abyssalzone", 11897, 6);
        Story.MapItemQuest(9312, "abyssalzone", 11898);

        // Sea Salt Soap (9313)
        Story.KillQuest(9313, "abyssalzone", "Necro Adipocere");

        // In the Grip of Justice (9314)
        if (!Story.QuestProgression(9314))
        {
            Core.EnsureAccept(9314);
            Core.GetMapItem(11899, 1, "abyssalzone");
            Core.HuntMonster("abyssalzone", "Necro Adipocere", "Necro Wax", 3);
            Core.HuntMonster("abyssalzone", "Foam Scavenger", "Scavenger Tentacle", 7);
            Core.EnsureComplete(9314);
        }

        // Together as One (9315)
        if (!Story.QuestProgression(9315))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9315);
            Core.HuntMonster("abyssalzone", "The Ashray", "The Ashray Vanquished");
            Core.EnsureComplete(9315);
        }
    }

    public void DeepWater(bool panopticonMerge = false, bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9338))
            return;

        AbyssalZone(seaVoice, coldThunder);

        // Unsung Heroes (9329)
        if (!Story.QuestProgression(9329))
        {
            Core.EnsureAccept(9329);
            Core.GetMapItem(11975, 1, "trenchobserve");
            Core.HuntMonster("trenchobserve", "Venerated Wraith", "Wraith I.D Card", 8);
            Core.EnsureComplete(9329);
        }

        // Watertight Guarantee (9330)
        if (!Story.QuestProgression(9330))
        {
            Core.EnsureAccept(9330);
            Core.GetMapItem(11976, map: "trenchobserve");
            Core.HuntMonster("trenchobserve", "Seabase Turret", "Turret Screws", 8);
            Core.EnsureComplete(9330);
        }

        // Core Electrolytes (9331)
        Story.MapItemQuest(9331, "trenchobserve", 11977, 4);

        // Guardian Spirits (9332)
        Story.KillQuest(9332, "trenchobserve", "Venerated Wraith");
        if (panopticonMerge)
            return;

        // Enemy in Need (9333)
        if (!Story.QuestProgression(9333))
        {
            Core.EnsureAccept(9333);
            Core.GetMapItem(11978, 4, "trenchobserve");
            Core.HuntMonster("trenchobserve", "Seabase Turret", "Turret Disarmed", 6);
            Core.EnsureComplete(9333);
        }

        // Here Lies Shadow (9334)
        Story.MapItemQuest(9334, "trenchobserve", new[] { 11979, 11981 });
        Story.MapItemQuest(9334, "trenchobserve", 11980, 2);

        // Nature's White Noise (9335)
        Story.KillQuest(9335, "trenchobserve", "Sea Spirit");

        // Dreams Seep into Reality (9336)
        Story.KillQuest(9336, "trenchobserve", "Necro Adipocere");

        // Hadal Havoc (9337)
        if (!Story.QuestProgression(9337))
        {
            Core.EnsureAccept(9337);
            Core.GetMapItem(11982, 1, "trenchobserve");
            Core.HuntMonster("trenchobserve", "Necro Adipocere", "Waxen Limb", 3);
            Core.HuntMonster("trenchobserve", "Sea Spirit", "Spirit Stinger", 7);
            Core.EnsureComplete(9337);
        }

        // See You on the Other Side (9338)
        if (!Story.QuestProgression(9338))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9338);
            Core.HuntMonster("trenchobserve", "Lady Noelle", "Lady Noelle Defeated");
            Core.EnsureComplete(9338);
        }
    }

    public void SeaVoice()
    {
        if (Core.isCompletedBefore(9348))
            return;

        SoW.ManaCradle();
        DeepWater(panopticonMerge: false, seaVoice: false, coldThunder: false);
        Story.PreLoad(this);
        Core.EnsureAccept(9348);

        FightVoiceInTheSea("Voice in the Sea Defeated", isTemp: true);

        Adv.GearStore(true, EnhAfter: true);
        Core.EnsureComplete(9348);
        Core.SellItem("Vigil", all: true);
    }
    public void Balemorale(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9729))
            return;

        TerminaTemple(seaVoice, coldThunder);

        Core.EquipClass(ClassType.Farm);

        // Estrangement (9719)
        Story.MapItemQuest(9719, "balemorale", 12933);

        // Searchlights (9720)
        Story.KillQuest(9720, "balemorale", "Lightguard Paladin");

        // Queensmen (9721)
        Story.KillQuest(9721, "balemorale", "Noble's Knight");

        // Cellar Secrets (9722)
        Story.MapItemQuest(9722, "balemorale", new[] { 13177, 13178 });

        // Chaotic Roots (9723)
        if (!Story.QuestProgression(9723))
        {
            Core.EnsureAccept(9723);
            Core.GetMapItems(new[] { (13179, 1), (13180, 1) }, "balemorale");
            Core.HuntMonster("balemorale", "Chaos Spider", "Spider Legs", 128);
            Core.EnsureComplete(9723);
        }

        // Eroding Era (9724)
        if (!Story.QuestProgression(9724))
        {
            Core.EnsureAccept(9724);
            Core.KillMonster("balemorale", "r13", "Left", "*", "Chaotic Shard", 50);
            Core.EnsureComplete(9724);
        }

        // Old Wolf (9725)
        Story.MapItemQuest(9725, "balemorale", 13181, 5);
        Story.MapItemQuest(9725, "balemorale", 13182);

        // Abandoned Cradle (9726)
        if (!Story.QuestProgression(9726))
        {
            Core.EnsureAccept(9726);
            Core.GetMapItem(13183, 1, "balemorale");
            Core.HuntMonster("balemorale", "Chaos Spider", "Chaotic Venom", 9);
            Core.HuntMonster("balemorale", "Chaos Crystal", "Violet Core", 9);
            Core.EnsureComplete(9726);
        }

        // Shockwaves (9727)
        Story.KillQuest(9727, "balemorale", "Skye Warrior");

        // Sleight of Hand (9728)
        Story.MapItemQuest(9728, "balemorale", 13184, 7);
        Story.MapItemQuest(9728, "balemorale", 13185);

        // Double Fianchetto (9729)
        if (!Story.QuestProgression(9729))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9729);
            Core.HuntMonster("balemorale", "Queen Victoria", "Queen Victoria Defeated");
            Core.EnsureComplete(9729);
            Core.EquipClass(ClassType.Farm);
        }
    }

    public void Castleeblana(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9741))
            return;

        Balemorale(seaVoice, coldThunder);

        Core.EquipClass(ClassType.Farm);

        // Skye's Raindrops 9732
        Story.KillQuest(9732, "castleeblana", "Skye Warrior");

        // Doctor's Orders 9733
        Story.MapItemQuest(9733, "castleeblana", 13202, 5);
        Story.MapItemQuest(9733, "castleeblana", 13203);

        // Shockwave's Ripples 9734
        Story.KillQuest(9734, "castleeblana", "Skye Executor");

        // Caretaker's Shadow 9735
        if (!Story.QuestProgression(9735))
        {
            Core.EnsureAccept(9735);
            Core.GetMapItem(13204, 1, "castleeblana");
            Core.HuntMonster("castleeblana", "Skye Warrior", "Warrior's Shockstone", 6);
            Core.HuntMonster("castleeblana", "Skye Executor", "Executor's Shockstone", 9);
            Core.EnsureComplete(9735);
        }

        // Harbinger's Tears 9736
        Story.KillQuest(9736, "castleeblana", "Bananach Raven");

        // Spectre of Hunger 9737
        if (!Story.QuestProgression(9737))
        {
            Core.EnsureAccept(9737);
            Core.GetMapItem(13205, 1, "castleeblana");
            Core.HuntMonster("castleeblana", "Fear Gorta", "Gorta Rag", 10);
            Core.EnsureComplete(9737);
        }

        // InnJustice 9738
        if (!Story.QuestProgression(9738))
        {
            Core.EnsureAccept(9738);
            Story.MapItemQuest(9738, "castleeblana", 13206);
            Core.HuntMonster("castleeblana", "Fear Gorta", "Hunger Leaves", 9);
            Core.HuntMonster("castleeblana", "Bananach Raven", "Electrified Feather", 9);
            Core.EnsureComplete(9738);
        }
        // Find Shelter in... 9739
        Story.MapItemQuest(9739, "castleeblana", new[] { 13207, 13208 });

        // Heavy Handed 9740
        Story.KillQuest(9740, "castleeblana", "Skye Warrior");

        // Miserable Monsoon 9741
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9741, "castleeblana", "Warden Indradeep");
    }

    public void Loughshine(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9764))
            return;

        Castleeblana(seaVoice, coldThunder);

        // Wicker Magic (9755)
        if (!Story.QuestProgression(9755))
        {
            Core.EnsureAccept(9755);
            Core.GetMapItem(13273, 1, "loughshine");
            Core.HuntMonster("loughshine", "Skye Cailleach", "Cailleach's Shockstone", 5);
            Core.EnsureComplete(9755);
        }

        // Maigh Eo (9756)
        Story.KillQuest(9756, "loughshine", "Scorched Elder Yew");

        // Resentment Harbour (9757)
        Story.MapItemQuest(9757, "loughshine", 13274, 5);
        Story.MapItemQuest(9757, "loughshine", 13275);

        // Wisened Yew (9758)
        if (!Story.QuestProgression(9758))
        {
            Core.EnsureAccept(9758);
            Core.HuntMonster("loughshine", "Skye Cailleach", "Cailleach's Twine", 3);
            Core.HuntMonster("loughshine", "Scorched Elder Yew", "Yew Aril", 3);
            Core.EnsureComplete(9758);
        }

        // Dragonflare (9759)
        Story.MapItemQuest(9759, "loughshine", 13276);

        // Shock Scatter (9760)
        Story.KillQuest(9760, "loughshine", "Skye Executor");

        // Grace of Three (9761)
        if (!Story.QuestProgression(9761))
        {
            Core.EnsureAccept(9761);
            Core.GetMapItem(13277, 1, "loughshine");
            Core.HuntMonster("loughshine", "Energy Elemental", "Electric Plasma", 12);
            Core.EnsureComplete(9761);
        }

        // Iubhair (9762)
        Story.MapItemQuest(9762, "loughshine", 13278, 5);
        Story.MapItemQuest(9762, "loughshine", 13279);

        // Parting the Clouds (9763)
        if (!Story.QuestProgression(9763))
        {
            Core.EnsureAccept(9763);
            Core.GetMapItem(13280, 1, "loughshine");
            Core.HuntMonster("loughshine", "Skye Executor", "Executor's Initials", 8);
            Core.HuntMonster("loughshine", "Energy Elemental", "Energy Core", 8);
            Core.EnsureComplete(9763);
        }

        // Arrester (9764)
        if (!Story.QuestProgression(9764))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9764, "loughshine", "Warden Iseul");
            Core.EquipClass(ClassType.Farm);
        }
    }

    public void NaoiseGrave(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9777))
            return;

        Loughshine(seaVoice, coldThunder);

        // Till When? (9768)
        if (!Story.QuestProgression(9768))
        {
            Core.EnsureAccept(9768);
            Core.GetMapItem(13296, 1, "naoisegrave");
            Core.HuntMonster("naoisegrave", "Bananach Raven", "Corvus Feather", 10);
            Core.EnsureComplete(9768);
        }

        // Energetic Feud (9769)
        if (!Story.QuestProgression(9769))
        {
            Core.EnsureAccept(9769);
            Core.GetMapItem(13297, 1, "naoisegrave");
            Core.HuntMonster("naoisegrave", "Energy Elemental", "Overdriven Electron", 10);
            Core.EnsureComplete(9769);
        }

        // Glacial Seal (9774)
        Story.MapItemQuest(9774, "naoisegrave", new[] { 13298, 13299 });

        // The Cold Wind Guides (9770)
        if (!Story.QuestProgression(9770))
        {
            Core.EnsureAccept(9770);
            Core.GetMapItem(13300, 1, "naoisegrave");
            Core.HuntMonster("naoisegrave", "Energy Elemental", "Frozen Lightning", 6);
            Core.HuntMonster("naoisegrave", "Bananach Raven", "Warm Raven Tear", 10);
            Core.EnsureComplete(9770);
        }

        // Cold Callers (9771)
        Story.KillQuest(9771, "naoisegrave", "Ice Guardian");

        // Destinies Lost (9772)
        Story.KillQuest(9772, "naoisegrave", "Bone Dragonling");

        // Of The Sorrows (9773)
        Core.EquipClass(ClassType.Solo);
        if (!Story.QuestProgression(9773))
        {
            Core.EnsureAccept(9773);
            Core.GetMapItem(13301, 1, "naoisegrave");
            Core.HuntMonster("naoisegrave", "Warden Iseul ", "Iseul's Mask", 1);
            Core.EnsureComplete(9773);
        }

        // Cold-Blooded Torture (9775)
        Story.KillQuest(9775, "naoisegrave", "Ice Guardian");

        // Echoing Cries (9776)
        Story.KillQuest(9776, "naoisegrave", "Bone Dragonling");

        // Dragon's Seal (9777)
        if (!Story.QuestProgression(9777))
        {
            Story.KillQuest(9777, "naoisegrave", "Volgritian");
        }
    }

    public void LiaTaraHill(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9814))
            return;

        NaoiseGrave(seaVoice, coldThunder);

        // At Ease (9805)
        Story.KillQuest(9805, "liatarahill", "Undead Garde");

        // Greenguard Daisy (9806)
        Story.KillQuest(9806, "liatarahill", "Garde Wraith");

        // True Nature (9807)
        if (!Story.QuestProgression(9807))
        {
            Core.EnsureAccept(9807);
            Core.HuntMonster("liatarahill", "Undead Garde ", "Garde's Sprouts", 6);
            Core.HuntMonster("liatarahill", "Garde Wraith", "Wraith's Breath", 12);
            Core.EnsureComplete(9807);
        }

        // Deafly Pride (9808)
        Story.MapItemQuest(9808, "liatarahill", new[] { 13364, 13365 });

        // King of Briars and Smoke (9809)
        if (!Story.QuestProgression(9809))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9809, "liatarahill", "King Duncan");
            Core.EquipClass(ClassType.Farm);
        }

        // Last Snowfall (9810)
        if (!Story.QuestProgression(9810))
        {
            Core.EnsureAccept(9810);
            Core.GetMapItem(13368, 1, "liatarahill");
            Core.HuntMonster("liatarahill", "Undead Garde ", "Garde's Bones", 50);
            Core.HuntMonster("liatarahill", "Garde Wraith", "Wraith's Soul", 3);
            Core.EnsureComplete(9810);
        }

        // Spring Melt (9811)
        if (!Story.QuestProgression(9811))
        {
            Core.EnsureAccept(9811);
            Core.GetMapItem(13366, 1, "liatarahill");
            Core.HuntMonster("liatarahill", "Ice Guardian", "Icy Fractal", 3);
            Core.EnsureComplete(9811);
        }

        // Connla (9812)
        Story.KillQuest(9812, "liatarahill", "Skye Cailleach");

        // Dumha na nGiall (9813)
        if (!Story.QuestProgression(9813))
        {
            Core.EnsureAccept(9813);
            Core.GetMapItem(13367, 1, "liatarahill");
            Core.HuntMonster("liatarahill", "Ice Guardian", "Frozen Blessing", 12);
            Core.HuntMonster("liatarahill", "Skye Cailleach", "Cailleach's Mask", 9);
            Core.EnsureComplete(9813);
        }

        // Changeling (9814)
        if (!Story.QuestProgression(9814))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9814, "liatarahill", "Warden Illaria");
            Core.EquipClass(ClassType.Farm);
        }
    }

    public void CastleGaheris(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(9828))
            return;

        LiaTaraHill(seaVoice, coldThunder);

        // Needs of the Few (9819)
        if (!Story.QuestProgression(9819))
        {
            Core.EnsureAccept(9819);
            Core.GetMapItem(13378, 1, "castlegaheris");
            Core.HuntMonster("castlegaheris", "Energy Elemental ", "Electronic Gem", 18);
            Core.EnsureComplete(9819);
        }

        // Lost and Numb (9820)
        Story.KillQuest(9820, "castlegaheris", "Ice Guardian");

        // Childhood Memories (9821)
        Story.MapItemQuest(9821, "castlegaheris", new[] { 13379, 13380, 13381 });

        // Strangled Sobs (9822)
        if (!Story.QuestProgression(9822))
        {
            Core.EnsureAccept(9822);
            Core.HuntMonster("castlegaheris", "Energy Elemental ", "Shock Enhancer", 9);
            Core.HuntMonster("castlegaheris", "Ice Guardian", "Frozen Eye", 9);
            Core.EnsureComplete(9822);
        }

        // Distant Care (9823)
        Story.KillQuest(9823, "castlegaheris", "Dark Cage");

        // Minus K (9824)
        Story.KillQuest(9824, "castlegaheris", "Glacial Crystal");

        // Someone Else's Blood and Tears (9825)
        Story.KillQuest(9825, "castlegaheris", "Elemental Hybrid");

        // Honor Thy Family (9826)
        Story.MapItemQuest(9826, "castlegaheris", new[] { 13382, 13383 });

        // Life's Latest Lows (9827)
        if (!Story.QuestProgression(9827))
        {
            Core.EnsureAccept(9827);
            Core.HuntMonster("castlegaheris", "Glacial Crystal", "Glaceran Ice", 32);
            Core.HuntMonster("castlegaheris", "Elemental Hybrid", "Hybrid Remnants", 32);
            Core.EnsureComplete(9827);
        }

        // Vow of Misery (9828)
        if (!Story.QuestProgression(9828))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9828, "castlegaheris", "Thundersnow Storm");
            Core.EquipClass(ClassType.Farm);
        }
    }

    public void ColdThunder()
    {
        if (Core.isCompletedBefore(9851))
            return;

        CastleGaheris(false, false);

        // 9832 | A Friend's Faith
        Story.MapItemQuest(9832, "coldthunder", 13403);

        // 9833 | The Storm Queen
        if (!Story.QuestProgression(9833))
        {
            Core.Logger($"Doing 9833 | The Storm Queen");
            Core.EnsureAccept(9833);
            ColdThunderBoss("Cold Thunder Defeated");
            Core.EnsureComplete(9833);
        }
    }

    public void ThelimaCity(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10356))
            return;

        TerminaTemple(seaVoice, coldThunder);

        Story.PreLoad(this);
        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Noelle Knight", // UseableMonsters[0],
            "Drow Soldier", // UseableMonsters[1],
            "Dwarven Aegis", // UseableMonsters[2],
            "Maleno Elemental", // UseableMonsters[3],
            "Maleno Match", // UseableMonsters[4],
            "Flame of Maleno", // UseableMonsters[5],
            "Eternal Flames", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10347 | Sister Statues
        Story.MapItemQuest(10347, "thelimacity", new[] { 14770, 14771 });

        // 10348 | Starstruck Chivalry
        if (!Story.QuestProgression(10348))
            Core.HuntMonsterQuest(10348, "thelimacity", UseableMonsters[0]);

        // 10349 | Black Dahlia
        Story.MapItemQuest(10349, "thelimacity", new[] { 14772, 14773 });

        // 10350 | Talls and Stouts
        if (!Story.QuestProgression(10350))
        {
            Core.EnsureAccept(10350);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("thelimacity", UseableMonsters[1], "Black Glass Pendant", 9);
            Core.HuntMonster("thelimacity", UseableMonsters[2], "Golden Ram Horn", 9);
            Core.EnsureComplete(10350);
        }

        // 10351 | A Lost Matchstick
        if (!Story.QuestProgression(10351))
        {
            Core.EnsureAccept(10351);
            Story.MapItemQuest(10351, "thelimacity", 14774);
            Core.KillMonster("thelimacity", "r9", "Left", UseableMonsters[4], "Ashen Key");
            Core.EnsureComplete(10351);
        }

        // 10352 | Flash Fire
        Story.MapItemQuest(10352, "thelimacity", 14775, 4);

        // 10353 | Unio Naturalis
        if (!Story.QuestProgression(10353))
        {
            Core.HuntMonsterQuest(10353, ("thelimacity", UseableMonsters[3], ClassType.Solo));
        }

        // 10354 | A Friend's Distant Laugh
        if (!Story.QuestProgression(10354))
        {
            Core.HuntMonsterQuest(10354, ("thelimacity", UseableMonsters[4], ClassType.Farm));
        }

        // 10355 | Putrefaction
        if (!Story.QuestProgression(10355))
        {
            Core.EnsureAccept(10355);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("thelimacity", UseableMonsters[3], "Black Flame", 3);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("thelimacity", UseableMonsters[4], "Burnt Hair", 12);
            Core.EnsureComplete(10355);
        }

        // 10356 | Enatiodromia
        if (!Story.QuestProgression(10356))
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonsterQuest(10356, ("thelimacity", UseableMonsters[5], ClassType.Solo));
        }
    }

    public void MountMaleno(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10368))
            return;

        ThelimaCity(seaVoice, true);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Soul Raven", // UseableMonsters[0],
            "Maleno Match", // UseableMonsters[1],
            "Maleno Elemental", // UseableMonsters[2],
            "Draconian Bandit", // UseableMonsters[3],
            "Idalion", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10359 | A-Luring Song
        if (!Story.QuestProgression(10359))
        {
            Core.EnsureAccept(10359);
            Core.GetMapItem(14805, 1, "mountmaleno");
            Core.HuntMonster("mountmaleno", "Soul Raven", "Soul Song", 15);
            Core.EnsureComplete(10359);
        }

        // 10360 | Splendid Ashes, Pompous Grave
        if (!Story.QuestProgression(10360))
        {
            Core.HuntMonsterQuest(10360, ("mountmaleno", UseableMonsters[1], ClassType.Farm));
        }

        // 10361 | Echolocation
        Story.MapItemQuest(10361, new[] { (14806, 1, "mountmaleno"), (14807, 5, "mountmaleno") });

        // 10362 | Chymerical Ash
        if (!Story.QuestProgression(10362))
        {
            Core.EnsureAccept(10362);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("mountmaleno", "Soul Raven", "Soul Feather", 6);
            Core.HuntMonster("mountmaleno", "Maleno Match", "Burnt Letters", 9);
            Core.EnsureComplete(10362);
        }

        // 10363 | Journey Through the Dark Night
        Story.MapItemQuest(10363, "mountmaleno", new[] { 14808, 14809 });

        // 10364 | Lunar Caustic
        if (!Story.QuestProgression(10364))
        {
            Core.EnsureAccept(10364);
            Core.GetMapItem(14810, 1, "mountmaleno");
            Core.HuntMonster("mountmaleno", "Maleno Elemental", "Maleno Steel", 6);
            Core.EnsureComplete(10364);
        }

        // 10365 | Like Water and Lithium
        if (!Story.QuestProgression(10365))
        {
            Core.EnsureAccept(10365);
            Core.GetMapItem(14811, 1, "mountmaleno");
            Core.HuntMonster("mountmaleno", "Draconian Bandit", "Putrefied Scale", 15);
            Core.EnsureComplete(10365);
        }

        // 10366 | The Shadows Lengthen in…
        Story.MapItemQuest(10366, "mountmaleno", 14812);

        // 10367 | Blithe Hospitality
        if (!Story.QuestProgression(10367))
        {
            Core.EnsureAccept(10367);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("mountmaleno", "Draconian Bandit", "Fused Hoard Coins", 9);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("mountmaleno", "Maleno Elemental", "Black Lead", 3);
            Core.EnsureComplete(10367);
        }

        // 10368 | Sal Volatile
        if (!Story.QuestProgression(10368))
        {
            Core.HuntMonsterQuest(10368, ("mountmaleno", UseableMonsters[4], ClassType.Solo));
        }
    }

    public void SanctuaryAiwass(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10384))
            return;

        MountMaleno(seaVoice, coldThunder);

        // 10375 - Raw Dove x12 (Dropped by Dove)
        Story.KillQuest(10375, "sanctuaryaiwass", "Dove");

        // 10376 - Albedo Bracelet x4 (Dropped by Albedo Elemental)
        Story.KillQuest(10376, "sanctuaryaiwass", "Albedo Elemental");

        // 10377 - Pristine Feather x18 (Dropped by Albedo Match)
        Story.KillQuest(10377, "sanctuaryaiwass", "Albedo Match");

        // 10378 - Leonal's Dream x15 (Dropped by Leonal)
        //        Map items: Svala (14865), Snowbeard (14866)
        if (!Story.QuestProgression(10378))
        {
            Core.EnsureAccept(10378);
            Core.GetMapItem(14865, map: "sanctuaryaiwass"); // Svala
            Core.GetMapItem(14866, map: "sanctuaryaiwass"); // Snowbeard
            Core.HuntMonster("sanctuaryaiwass", "Leonal", "Leonal's Dream", 15);
            Core.EnsureComplete(10378);
        }

        // 10379 - Salt Sigil x1 (Dropped by Crystal Prima)
        //        Map item: Geopetal (14867)
        if (!Story.QuestProgression(10379))
        {
            Core.EnsureAccept(10379);
            Core.GetMapItem(14867, map: "sanctuaryaiwass"); // Geopetal
            Core.HuntMonster("sanctuaryaiwass", "Crystal Prima", "Salt Sigil", 1);
            Core.EnsureComplete(10379);
        }

        // 10380 - Albedo Materia x1 (Dropped by Albedo Elemental)
        //        Map item: Isiri (14868)
        if (!Story.QuestProgression(10380))
        {
            Core.EnsureAccept(10380);
            Core.HuntMonster("sanctuaryaiwass", "Albedo Elemental", "Albedo Materia", 1);
            Core.GetMapItem(14868, map: "sanctuaryaiwass"); // Isiri
            Core.EnsureComplete(10380);
        }

        // 10381 - Match's Scar x5 (Dropped by Albedo Match)
        Story.KillQuest(10381, "sanctuaryaiwass", "Albedo Match");

        // 10382 - Mercury Sigil x1 (Dropped by Mercury Prima)
        //        Sulfur Sigil x1 (Dropped by Sulfur Prima)
        if (!Story.QuestProgression(10382))
        {
            Core.EnsureAccept(10382);
            Core.HuntMonster("sanctuaryaiwass", "Mercury Prima", "Mercury Sigil", 1);
            Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Sulfur Sigil", 1);
            Core.EnsureComplete(10382);
        }

        // 10383 - Offer Flowers x8, Read Strange Scroll x1
        //        Map items: Blue arrows (14869) x8, Strange Scroll (14870)
        if (!Story.QuestProgression(10383))
        {
            Core.EnsureAccept(10383);
            for (int i = 0; i < 8; i++)
                Core.GetMapItem(14869, map: "sanctuaryaiwass"); // Click blue arrows 8 times
            Core.GetMapItem(14870, map: "sanctuaryaiwass"); // Read Strange Scroll
            Core.EnsureComplete(10383);
        }

        // 10384 - Commune with Aiwass x1 (Dropped by Anima Animus Aiwass)
        Story.KillQuest(10384, "sanctuaryaiwass", "Anima Animus Aiwass");
    }

    public void ForgeAlbedo(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10422))
            return;

        SanctuaryAiwass(seaVoice, coldThunder);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Albedo Elemental", // UseableMonsters[0],
            "Defensive Turret", // UseableMonsters[1],
            "Runehawk Husk", // UseableMonsters[2],
            "Collapsed Husk", // UseableMonsters[3],
            "Albedo Match", // UseableMonsters[4],
            "Defense Droid", // UseableMonsters[5],
            "Flame of Albedo", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10413 | Discerning Threats
        if (!Story.QuestProgression(10413))
        {
            Core.HuntMonsterQuest(10413, ("forgealbedo", UseableMonsters[0], ClassType.Farm));
        }

        // 10414 | Model Students
        Story.MapItemQuest(10414, "forgealbedo", new[] { 14959, 14960, 14961 });

        // 10415 | System Delusion
        if (!Story.QuestProgression(10415))
        {
            Core.EnsureAccept(10415);
            Core.GetMapItem(14962, 1, "forgealbedo");
            Core.HuntMonster("forgealbedo", "Defensive Turret", "Turret Logic Module", 18);
            Core.EnsureComplete(10415);
        }

        // 10416 | The Leap that Never Landed
        if (!Story.QuestProgression(10416))
        {
            Core.EnsureAccept(10416);
            Core.GetMapItem(14963, 4, "forgealbedo");
            Core.HuntMonster("forgealbedo", "Runehawk Husk", "Runehawk Student ID", 12);
            Core.EnsureComplete(10416);
        }

        // 10417 | Insistent Existence
        if (!Story.QuestProgression(10417))
        {
            Core.EnsureAccept(10417);
            Core.GetMapItem(14964, 4, "forgealbedo");
            Core.HuntMonster("forgealbedo", "Collapsed Husk", "Husk's Stained ID", 3);
            Core.EnsureComplete(10417);
        }

        // 10418 | Failed Reflection
        if (!Story.QuestProgression(10418))
        {
            Core.HuntMonsterQuest(10418, ("forgealbedo", UseableMonsters[4], ClassType.Farm));
        }

        // 10419 | Technically Aware
        if (!Story.QuestProgression(10419))
        {
            Core.HuntMonsterQuest(10419, ("forgealbedo", UseableMonsters[5], ClassType.Solo));
        }

        // 10420 | Mental Barrier
        if (!Story.QuestProgression(10420))
        {
            Core.EnsureAccept(10420);
            Core.GetMapItem(14965, 6, "forgealbedo");
            Core.HuntMonster("forgealbedo", "Defensive Turret", "Blastproof Shell", 12);
            Core.EnsureComplete(10420);
        }

        // 10421 | Lost Admin Privileges
        if (!Story.QuestProgression(10421))
        {
            Core.EnsureAccept(10421);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("forgealbedo", "Defensive Turret", "Active Fuel Cell", 6);
            Core.HuntMonster("forgealbedo", "Defense Droid", "Rapid Cooling Fluid", 9);
            Core.EnsureComplete(10421);
        }

        // 10422 | Conscious Unconsciousness
        if (!Story.QuestProgression(10422))
        {
            Core.HuntMonsterQuest(10422, ("forgealbedo", UseableMonsters[6], ClassType.Solo));
        }
    }

    public void FortLuma(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10618))
            return;

        ForgeAlbedo(seaVoice, coldThunder);
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Citrinitas Elemental", // UseableMonsters[0],
            "Luma Lifeform", // UseableMonsters[1],
            "Lightguard Elite", // UseableMonsters[2],
            "Rigaud &amp; Cassie", // UseableMonsters[3],
            // use monstermapid for this'n [10]
            "Citrinitas Match", // UseableMonsters[4],
            "Luma Dragon Twins", // UseableMonsters[5],
            "Flame of Citrinitas", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10609 | Xanthosis
        if (!Story.QuestProgression(10609))
        {
            Story.MapItemQuest(10609, "fortluma", 15542);
            Core.HuntMonsterQuest(10609,
                ("fortluma", UseableMonsters[0], ClassType.Solo));
        }


        // 10610 | Golden Lead
        if (!Story.QuestProgression(10610))
        {
            Story.MapItemQuest(10610, "fortluma", 15543);
            Core.HuntMonsterQuest(10610,
                ("fortluma", UseableMonsters[1], ClassType.Farm));
        }


        // 10611 | Luminary Vision
        if (!Story.QuestProgression(10611))
        {
            Story.MapItemQuest(10611, "fortluma", new[] { 15544, 15545 });
        }


        // 10612 | Premature Sunrise
        if (!Story.QuestProgression(10612))
        {
            Core.HuntMonsterQuest(10612,
                ("fortluma", UseableMonsters[2], ClassType.Farm));
        }


        // 10613 | Blinded by the Light
        if (!Story.QuestProgression(10613))
        {
            Core.EnsureAccept(10613);
            Core.HuntMonsterMapID("fortluma", 10, "Astero's Aurum");
            Core.EnsureComplete(10613);
        }


        // 10614 | Wayward Wisewoman
        if (!Story.QuestProgression(10614))
        {
            Core.EnsureAccept(10614);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("fortluma", UseableMonsters[2], "Elite Promotion Notice", 6);
            Core.HuntMonster("fortluma", UseableMonsters[0], "Cloudy Citrine", 2);
            Story.MapItemQuest(10614, "fortluma", 15546);
            Bot.Wait.ForQuestComplete(10614);
        }

        Bot.Sleep(2500);
        if (Bot.Player.Cell.Contains("Cut"))
            Core.JumpWait();

        // 10615 | Incognizance
        if (!Story.QuestProgression(10615))
        {
            Core.EnsureAccept(10615);
            Core.HuntMonster("fortluma", UseableMonsters[4], "Calcified Eyes", 18);
            Story.MapItemQuest(10615, "fortluma", 15547);
        }


        // 10616 | Flowers of Luma
        if (!Story.QuestProgression(10616))
        {
            Core.HuntMonsterQuest(10616,
                ("fortluma", UseableMonsters[5], ClassType.Solo));
        }


        // 10617 | Quid Pro Quo
        if (!Story.QuestProgression(10617))
        {
            Core.EnsureAccept(10617);
            Core.KillMonster("fortluma", "r10", "Left", "*", "Dark Azoth", 6);
            Story.MapItemQuest(10617, "fortluma", 15548);
        }


        // 10618 |Drowned King in Yellow
        if (!Story.QuestProgression(10618))
        {
            Bot.Log("**Boss Gear Suggestion**\nSuggested Class and Enh for this boss if your stuck, swap your Solo class to: Dragon of time(or any decent HoT class)\n--With these enhancements--\n" + "Class: Healer\nHelm: Healer\nWeapon: Elysium/Healer\nCape: Absolution/Healer");
            Core.HuntMonsterQuest(10618,
                ("fortluma", UseableMonsters[6], ClassType.Solo));
            Bot.Wait.ForQuestComplete(10618);
            Bot.Sleep(2500);
            Core.JumpWait();

        }

    }

    public void WarWickForest(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10687))
            return;
        FortLuma(seaVoice, coldThunder);

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Black Phoenix", // UseableMonsters[0],
            "Rubedo Elemental", // UseableMonsters[1],
            "Rubedo Match", // UseableMonsters[2],
            "Charred Remnant", // UseableMonsters[3],
            "Idea of a Champion", // UseableMonsters[4],
            "Flame of Rubedo", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10678 | A Phoenix's Repeated Demise
        if (!Story.QuestProgression(10678))
        {
            Story.MapItemQuest(10678, "warwickforest", 15656);
            Core.HuntMonsterQuest(10678,
                ("warwickforest", UseableMonsters[0], ClassType.Solo));
        }

        // 10679 | Reopened Scar
        if (!Story.QuestProgression(10679))
        {
            Story.MapItemQuest(10679, "warwickforest", 15657);
            Core.HuntMonsterQuest(10679,
                ("warwickforest", UseableMonsters[1], ClassType.Solo));
        }

        // 10680 | Pyroclast Breath
        if (!Story.QuestProgression(10680))
        {
            Core.HuntMonsterQuest(10680,
                ("warwickforest", UseableMonsters[3], ClassType.Solo));
        }

        // 10681 | Smoke Signals
        if (!Story.QuestProgression(10681))
        {
            Story.MapItemQuest(10681, "warwickforest", 15658);
            Core.HuntMonsterQuest(10681,
                ("warwickforest", UseableMonsters[1], ClassType.Solo));
        }

        // 10682 | Reflection in Oil
        if (!Story.QuestProgression(10682))
        {
            Core.EnsureAccept(10682);
            Story.MapItemQuest(10682, "warwickforest", 15659);
            Core.KillMonster("warwickforest", "r8", "Left", "*", "Smoldering Ashes", 6);
            Core.EnsureComplete(10682);
        }

        // 10683 | Iosis of the Soul
        if (!Story.QuestProgression(10683))
        {
            Story.MapItemQuest(10683, "warwickforest", 15660);
            Core.HuntMonsterQuest(10683,
                ("warwickforest", UseableMonsters[2], ClassType.Solo));
        }

        // 10684 | The Scorpion and the Fox
        if (!Story.QuestProgression(10684))
        {
            Core.EnsureAccept(10684);
            Story.MapItemQuest(10684, "warwickforest", 15661);
            Core.KillMonster("warwickforest", "r2", "Left", "*", "Philosopher's Eye", 15);
            Core.EnsureComplete(10684);

        }

        // 10685 | Dalitong Keli
        if (!Story.QuestProgression(10685))
        {
            Story.MapItemQuest(10685, "warwickforest", 15662, 5);
        }

        // 10686 | The Ideal Champion
        if (!Story.QuestProgression(10686))
        {
            Core.HuntMonsterQuest(10686,
                ("warwickforest", UseableMonsters[4], ClassType.Solo));
        }

        // 10687 | Kolr
        if (!Story.QuestProgression(10687))
        {
            Core.HuntMonsterQuest(10687,
                ("warwickforest", UseableMonsters[5], ClassType.Solo));
        }

    }

    public void MagnumOpus(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10705))
            return;

        WarWickForest(true, true);
        Story.PreLoad(this);

        // 10703 | The Black Mage
        if (!Story.QuestProgression(10703))
        {
            Core.EnsureAccept(10703);
            Core.GetMapItem(15703, 1, "magnumopus");
            Core.EnsureComplete(10703);
        }

        // 10704 | Hic Iacet Revelatio
        if (!Story.QuestProgression(10704))
        {
            Core.EnsureAccept(10704);
            if (!string.IsNullOrEmpty(Core.BossClass))
                Core.EquipClass(ClassType.Boss);
            else
            {
                Bot.Log("BossClass is empty/unselected in CBO, we'll use the Solo class or if thats empty, the currently equipped class.");
                Core.EquipClass(ClassType.Solo);
            }
            Bot.Events.ExtensionPacketReceived += FlameoftheBeyond;

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Flame of the Beyond Defeated"))
            {
                if (Bot.Map.Name != "magnumopus")
                    Core.Join("magnumopus");

                if (Bot.Player.Cell != "r2")
                    Core.Jump("r2", "bottom");

                Bot.Combat.Attack("*");

                Bot.Sleep(200);
                if (Bot.TempInv.Contains("Flame of the Beyond Defeated"))
                    break;
            }

            Bot.Events.ExtensionPacketReceived -= FlameoftheBeyond;
            Core.EnsureComplete(10704);
        }


        async void FlameoftheBeyond(dynamic packet)
        {
            if (packet?["params"]?.type?.ToString() != "json")
                return;
            if (!Bot.Player.Alive)
                return;
            dynamic data = packet["params"].dataObj;
            if (data?.cmd?.ToString() != "event")
                return;
            string? zoneSet = data?.args?.zoneSet?.ToString();
            if (string.IsNullOrEmpty(zoneSet))
                return;

            float px = Bot.Player.X;
            float py = Bot.Player.Y;

            // Zone A active, walk right
            if (string.Equals(zoneSet, "A", StringComparison.OrdinalIgnoreCase))
            {
                /* box of : 
                x: 550, y: 339
                x: 678, y: 465
                */
                if (px >= 550 && px <= 678 && py >= 339 && py <= 465)
                    return;

                int randX = Random.Shared.Next(550, 678);
                int randY = Random.Shared.Next(339, 465);
                _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
                return;
            }

            // Zone B active, walk left
            if (string.Equals(zoneSet, "B", StringComparison.OrdinalIgnoreCase))
            {
                /* box of : 
                x: 426, y: 359
                x: 211, y: 445
                */
                if (px >= 211 && px <= 426 && py >= 359 && py <= 445)
                    return;

                int randX = Random.Shared.Next(211, 426);
                int randY = Random.Shared.Next(359, 445);
                _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
                return;
            }
        }

    }

    public void RubedoPeak(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10714))
            return;

        MagnumOpus(seaVoice, coldThunder);

        Story.PreLoad(this);

        // 10714 | Wave Goodbye
        Story.KillQuest(10714, "rubedopeak", "Rubedo Match");
    }

    public void ForgeCitrinitas(bool seaVoice = false, bool coldThunder = false)
    {
        if (Core.isCompletedBefore(10736))
            return;

        RubedoPeak(seaVoice, coldThunder);
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Citrinitas Elemental", // UseableMonsters[0],
            "Defensive Turret", // UseableMonsters[1],
            "Citrinitas Match", // UseableMonsters[2],
            "Luma Lifeform", // UseableMonsters[3],
            "Defense Droid", // UseableMonsters[4],
            "Clematis", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10725 | The Tarnished Ideal
        if (!Story.QuestProgression(10725))
        {
            Core.HuntMonsterQuest(10725,
                ("forgecitrinitas", UseableMonsters[0], ClassType.Farm));
        }


        // 10726 | Pointed Suggestions
        if (!Story.QuestProgression(10726))
        {
            Story.MapItemQuest(10726, "forgecitrinitas", new[] { 15820, 15821 });
        }


        // 10727 | Bite the Bullet
        if (!Story.QuestProgression(10727))
        {
            Core.HuntMonsterQuest(10727,
                ("forgecitrinitas", UseableMonsters[1], ClassType.Farm));
        }


        // 10728 | Pain and Ecstasy
        if (!Story.QuestProgression(10728))
        {
            Core.HuntMonsterQuest(10728,
                ("forgecitrinitas", UseableMonsters[3], ClassType.Farm));
        }


        // 10729 | Reincubation
        if (!Story.QuestProgression(10729))
        {
            Story.MapItemQuest(10729, "forgecitrinitas", 15822);
        }


        // 10730 | Server Reboot
        if (!Story.QuestProgression(10730))
        {
            Core.HuntMonsterQuest(10730,
                ("forgecitrinitas", UseableMonsters[4], ClassType.Farm));
        }


        // 10731 | Of Pyrite and Nickel
        if (!Story.QuestProgression(10731))
        {
            Core.HuntMonsterQuest(10731,
                ("forgecitrinitas", "Citrinitas Match", ClassType.Farm));
        }


        // 10732 | Skin Deep
        if (!Story.QuestProgression(10732))
        {
            Core.HuntMonsterQuest(10732,
                ("forgecitrinitas", UseableMonsters[3], ClassType.Farm));
        }


        // 10733 | Break Room
        if (!Story.QuestProgression(10733))
        {
            Story.MapItemQuest(10733, "forgecitrinitas", new[] { 15823, 15824, 15825 });
        }


        // 10734 | Clematis' Aspirations
        if (!Story.QuestProgression(10734))
        {
            Core.HuntMonsterQuest(10734,
                ("forgecitrinitas", UseableMonsters[5], ClassType.Solo));
        }


        if (Core.IsMember)
        {
            // 10735 | Silica Spring
            if (!Story.QuestProgression(10735))
            {
                Core.EnsureAccept(10735);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("forgecitrinitas", "Clematis", "Clematis' Blood Sample");
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("forgecitrinitas", "Luma Lifeform", "Lifeform's Hope", 12);
                Core.HuntMonster("forgecitrinitas", "Defense Droid", "Droid's Battery Acid", 9);
                Core.EnsureComplete(10735);
            }
        }

        else
        {
            // 10736 | Silica Spring (Legend)
            if (!Story.QuestProgression(10736))
            {
                Core.EnsureAccept(10736);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("forgecitrinitas", "Clematis", "Clematis' Blood Sample");
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("forgecitrinitas", "Luma Lifeform", "Lifeform's Hope", 12);
                Core.HuntMonster("forgecitrinitas", "Defense Droid", "Droid's Battery Acid", 9);
                Core.EnsureComplete(10736);
            }
        }


    }

    // Mostly for `Skye's Lightning` for the Merge
    public void ColdThunderBoss(string? item = null, int quant = 1, bool isTemp = true)
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : Core.CheckInventory(item, quant))
        )
        {
            Bot.Events.ExtensionPacketReceived -= Listener;
            Core.JumpWait();
            return;
        }

        Core.EquipClass(ClassType.Solo);

        // Reset potion flags
        bool needsPotion = false;
        bool potionApplied = false;

        Bot.Events.ExtensionPacketReceived += Listener;

        if (!isTemp && item != null)
            Core.AddDrop(item);

        //get, equip, and use safe pot (potions buggy af ae.. fix this)
        Core.BuyItem("mirrorportal", 774, "Shriekward Potion", 99);
        Core.Equip("Shriekward Potion");
        Bot.Wait.ForItemEquip("Shriekward Potion");
        Bot.Wait.ForActionCooldown(GameActions.EquipItem);
        Core.Sleep();
        Core.UsePotion();

        //get & equip useable pot
        Core.BuyItem("coldthunder", 2467, "Bananach's Last Will", 1000);
        Core.Equip("Bananach's Last Will");
        Bot.Wait.ForItemEquip("Bananach's Last Will");
        Bot.Wait.ForActionCooldown(GameActions.EquipItem);
        Core.Sleep();

        Core.Logger(
            "About to attack Cold Thunder boss. It's very unstable; recommended to do it with more accounts."
        );

        while (
            !Bot.ShouldExit
            && item != null
            && (isTemp ? !Bot.TempInv.Contains(item, quant) : !Core.CheckInventory(item, quant))
        )
        {
            if (Bot.Map.Name != "coldthunder")
                Core.Join("coldthunder");

            if (Bot.Player.Cell != "r3")
                Core.Jump("r3");

            // Check if potion is needed and handle it
            if (needsPotion && !potionApplied)
            {
                Core.Logger(
                    "Detected 'The skies rumble. Prepare yourself!' - applying Bananach's Last Will potion..."
                );

                // Keep trying to use potion until it's successfully applied
                while (!Bot.ShouldExit && needsPotion && !potionApplied)
                {
                    Bot.Combat.CancelAutoAttack();
                    Bot.Combat.CancelTarget();
                    Core.UsePotion();
                    Core.Sleep(200); // Small delay to allow potion to apply

                    // Check if Bananach's Last Will potion was applied
                    // You might need to adjust the aura name based on what this potion actually provides
                    if (Bot.Self.Auras.Any(a => a != null && a.Name == "Bananach's Last Will"))
                    {
                        potionApplied = true;
                        needsPotion = false;
                        Core.Logger("Bananach's Last Will potion successfully applied!");
                    }
                    else
                    {
                        Core.Sleep(100); // Brief pause before retrying
                    }
                }
            }

            // Only attack if no potion is needed or potion has been applied
            if (!needsPotion || potionApplied)
            {
                Bot.Combat.Attack("*");
            }

            if (
                item != null
                && (isTemp ? Bot.TempInv.Contains(item, quant) : Core.CheckInventory(item, quant))
            )
            {
                Core.JumpWait();
                break;
            }

            Core.Sleep();
        }
        Bot.Events.ExtensionPacketReceived -= Listener;

        // Reset potion flags
        needsPotion = false;
        potionApplied = false;

        Core.JumpWait();
        Adv.GearStore(true, EnhAfter: true);

        void Listener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ct":
                        if (data.anims is not null)
                        {
                            foreach (var a in data.anims)
                            {
                                if (a is null)
                                    continue;

                                if (
                                    a.msg is not null
                                    && (string)a.msg is "The skies rumble. Prepare yourself!"
                                )
                                {
                                    needsPotion = true;
                                    potionApplied = false;
                                    Core.Logger(
                                        "Event detected: The skies rumble. Prepare yourself! - Potion needed."
                                    );
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    public void FightVoiceInTheSea(string item, int quant = 1, bool isTemp = false)
    {
        if (!isTemp)
            Core.Unbank(item);
        if (Finished())
            return;
        if (!isTemp)
            Core.AddDrop(item);

        // Vainglory compounds the healing reduction from Voice in the Sea.
        var cape = Bot.Inventory.Items.Find(i => i.Equipped
            && i.Category == ItemCategory.Cape
            && i.EnhancementPatternID == (int)CapeSpecial.Vainglory);
        if (cape != null)
            Adv.EnhanceItem(
                cape.Name,
                EnhancementType.Lucky,
                CapeSpecial.None,
                HelmSpecial.None,
                WeaponSpecial.None,
                logging: false,
                bypassDisableAutoEnhance: true
            );

        Core.Join("seavoice");
        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
        Core.Equip("Vigil");
        if (!Bot.Inventory.IsEquipped("Vigil"))
        {
            Core.Logger("Vigil could not be equipped.");
            return;
        }

        _voiceChargeDetected = false;
        Bot.Flash.FlashCall += SeaVoiceFlashListener;
        try
        {
            Core.Jump("r2", "Left");
            Bot.Player.SetSpawnPoint();
            while (!Bot.ShouldExit && !Finished())
            {
                if (!Bot.Player.Alive)
                {
                    _voiceChargeDetected = false;
                    Bot.Skills.Resume();
                    Bot.Sleep(500);
                    continue;
                }
                if (Bot.Player.Cell != "r2")
                    Core.Jump("r2", "Left");

                if (_voiceChargeDetected)
                {
                    while (!Bot.ShouldExit && Bot.Player.Alive && Bot.Player.HasTarget && !Bot.Self.HasActiveAura("Vigil"))
                    {
                        if (Bot.Skills.CanUseSkill(5))
                            Bot.Skills.UseSkill(5);
                        Bot.Sleep(50);
                    }
                    _voiceChargeDetected = false;
                    Bot.Skills.Resume();
                    if (!Bot.ShouldExit && Bot.Self.HasActiveAura("Vigil"))
                        Core.Logger("Vigil active. Class skills resumed.");
                    continue;
                }

                var target = Bot.Player.Target;
                if (target == null || target.MapID != 1 || target.HP <= 0)
                    Bot.Combat.Attack(1);
                Bot.Sleep(500);
            }
        }
        finally
        {
            Bot.Flash.FlashCall -= SeaVoiceFlashListener;
            _voiceChargeDetected = false;
            Bot.Skills.Resume();
            Core.JumpWait();
        }

        bool Finished()
        {
            if (isTemp)
                return Bot.TempInv.Contains(item, quant);
            return Bot.Inventory.Contains(item, quant);
        }
    }

    private void SeaVoiceFlashListener(string name, object[] args)
    {
        try
        {
            if (name != "pext" || args.Length == 0 || _voiceChargeDetected)
                return;
            string rawPacket = args[0]?.ToString() ?? string.Empty;
            if (rawPacket.IndexOf("You will not resist!", StringComparison.OrdinalIgnoreCase) < 0)
                return;
            Bot.Skills.Pause();
            _voiceChargeDetected = true;
            Core.Logger("Charge detected. Class skills paused for Vigil.");
        }
        catch
        {
        }
    }
}
