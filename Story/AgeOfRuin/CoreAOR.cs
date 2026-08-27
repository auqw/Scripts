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
        Story.MapItemQuest(9213, "terminatemple", new[] { 11625, 11626, 11627 });
        Core.Logger("resetting map data ( mobs can bug out and become unattackable)");
        Core.Join("whitemap-100000");
        Core.Join("terminatemple");
        Story.KillQuest(9213, "terminatemple", "Termina Defender");

        // Loaded Resume (9214)
        Story.MapItemQuest(9214, "terminatemple", new[] { 11628, 11629, 11630 });
        Story.KillQuest(9214, "terminatemple", "Clandestine Guard");

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
        Story.MapItemQuest(9226, "ashray", new[] { 11663, 11664 });
        Story.KillQuest(9226, "ashray", "Ashray Fisherman");

        // Slimy Scavenger (9227)
        Story.KillQuest(9227, "ashray", "Ghostly Eel");

        // Troubled Waters (9228)
        Story.MapItemQuest(9228, "ashray", 11665);
        Story.KillQuest(9228, "ashray", "Stagnant Water");

        // Washed Ink (9229)
        Story.MapItemQuest(9229, "ashray", 11666);
        Story.KillQuest(9229, "ashray", "Ashray Fisherman");

        // Fishy Hospitality (9230)
        Story.KillQuest(9230, "ashray", "Kitefin Shark Bait");

        // Doctoring Papers (9231)
        Story.MapItemQuest(9231, "ashray", 11667);
        Story.KillQuest(9231, "ashray", "Ghostly Eel");

        // Psychic Pollution (9232)
        Story.MapItemQuest(9232, "ashray", 11668);
        Story.KillQuest(9232, "ashray", "Stagnant Water");

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
        Story.MapItemQuest(9244, "sunlightzone", new[] { 11705, 11706 });
        Story.KillQuest(9244, "sunlightzone", "Blighted Water");

        // Tech Illiterate (9245)
        Story.MapItemQuest(9245, "sunlightzone", 11707, 3);
        Story.KillQuest(9245, "sunlightzone", "Spectral Jellyfish");

        // Plugging Leaks (9246)
        Story.KillQuest(9246, "sunlightzone", new[] { "Spectral Jellyfish", "Blighted Water" });

        // Shared History (9247)
        Story.MapItemQuest(9247, "sunlightzone", new[] { 11708, 11709, 11710 });

        // Flat Scares (9248)
        Story.MapItemQuest(9248, "sunlightzone", 11711);
        Story.KillQuest(9248, "sunlightzone", "Astravian Illusion");

        // Fishy Bully (9249)
        Story.MapItemQuest(9249, "sunlightzone", 11712, 5);
        Story.KillQuest(9249, "sunlightzone", "Infernal Illusion");

        // Faint Howls (9250)
        Story.MapItemQuest(9250, "sunlightzone", 11713);
        Story.KillQuest(9250, "sunlightzone", "Seraphic Illusion");

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
        Story.KillQuest(9261, "twilightzone", new[] { "Whale Louse", "Polymelia Lamprey" });

        // Morning Stretches (9262)
        Story.MapItemQuest(9262, "twilightzone", new[] { 11751, 11752 });

        // Natural Empathy (9263)
        Story.KillQuest(9263, "twilightzone", "Decay Spirit");

        // Comfort Blanket of Snow (9264)
        Story.KillQuest(9264, "twilightzone", "Ice Guardian");

        // Whale Watching (9265)
        Story.MapItemQuest(9265, "twilightzone", new[] { 11753, 11754, 11755 });

        // Exhausted Spirits (9266)
        Story.KillQuest(9266, "twilightzone", new[] { "Decay Spirit", "Ice Guardian" });

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
        Story.KillQuest(9272, "sunlightzone", new[] { "Infernal Illusion", "Astravian Illusion" });

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
        Story.MapItemQuest(9293, "midnightzone", 11845);
        Story.KillQuest(9293, "midnightzone", "Polymelia Lamprey");

        // Graveyard Shift (9294)
        Story.KillQuest(9294, "midnightzone", new[] { "Vowed ShadowSlayer", "Vowed ShadowSlayer" });

        // Educational Execution (9295)
        Story.MapItemQuest(9295, "midnightzone", 11846);
        Story.KillQuest(9295, "midnightzone", "Undead Prisoner");

        // Vows For Ignorance (9296)
        Story.MapItemQuest(9296, "midnightzone", 11847);
        Story.KillQuest(9296, "midnightzone", new[] { "Undead Prisoner", "Vowed ShadowSlayer" });

        // Protein Shake (9297)
        Story.MapItemQuest(9297, "midnightzone", 11848, 3);
        Story.KillQuest(9297, "midnightzone", "Shadow Viscera");

        // Duty Beyond Death (9298)
        Story.KillQuest(9298, "midnightzone", "Venerated Wraith");

        // Designated Taunters (9299)
        Story.KillQuest(9299, "midnightzone", new[] { "Shadow Viscera", "Venerated Wraith" });

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
        Story.MapItemQuest(9306, "abyssalzone", 11914);
        Story.KillQuest(9306, "abyssalzone", "Kitefin Shark Bait");

        // Suckered Blockade (9307)
        Story.MapItemQuest(9307, "abyssalzone", 11893, 6);
        Story.MapItemQuest(9307, "abyssalzone", 11894);

        // Digestive Fluids (9308)
        Story.KillQuest(9308, "abyssalzone", "Blighted Water");

        // Sodden Secrets (9309)
        Story.MapItemQuest(9309, "abyssalzone", 11895, 3);
        Story.KillQuest(9309, "abyssalzone", "Shadow Viscera");

        // The Hidden Corpse (9310)
        Story.KillQuest(9310, "abyssalzone", new[] { "Shadow Viscera", "Blighted Water" });

        // Octo-Flake Fish Feed (9311)
        Story.MapItemQuest(9311, "abyssalzone", 11896);
        Story.KillQuest(9311, "abyssalzone", "Foam Scavenger");

        // Completely Surrounded (9312)
        Story.MapItemQuest(9312, "abyssalzone", 11897, 6);
        Story.MapItemQuest(9312, "abyssalzone", 11898);

        // Sea Salt Soap (9313)
        Story.KillQuest(9313, "abyssalzone", "Necro Adipocere");

        // In the Grip of Justice (9314)
        Story.MapItemQuest(9314, "abyssalzone", 11899);
        Story.KillQuest(9314, "abyssalzone", new[] { "Necro Adipocere", "Foam Scavenger" });

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
        Story.MapItemQuest(9329, "trenchobserve", 11975);
        Story.KillQuest(9329, "trenchobserve", "Venerated Wraith");

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
        Story.MapItemQuest(9333, "trenchobserve", 11978, 4);
        Story.KillQuest(9333, "trenchobserve", "Seabase Turret");

        // Here Lies Shadow (9334)
        Story.MapItemQuest(9334, "trenchobserve", new[] { 11979, 11981 });
        Story.MapItemQuest(9334, "trenchobserve", 11980, 2);

        // Nature's White Noise (9335)
        Story.KillQuest(9335, "trenchobserve", "Sea Spirit");

        // Dreams Seep into Reality (9336)
        Story.KillQuest(9336, "trenchobserve", "Necro Adipocere");

        // Hadal Havoc (9337)
        Story.MapItemQuest(9337, "trenchobserve", 11982);
        Story.KillQuest(9337, "trenchobserve", new[] { "Necro Adipocere", "Sea Spirit" });

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

        string[] possibleSoloClasses = new[]
        {
        "Chrono ShadowHunter",
        "Chrono ShadowSlayer",
        "Chaos Avenger",
        "Verus DoomKnight",
        "Hollowborn Vindicator",
        "Lich",
        "ArchPaladin",
        "Lord of Order",
        "StoneCrusher",
        "Dragon of Time",
        "Unundead Goat",
    };

        // Build owned class lookup (inventory + bank)
        HashSet<string> ownedClasses =
            Bot.Inventory?.Items?.Select(i => i.Name)
                .Concat(Bot.Bank?.Items?.Select(i => i.Name) ?? Enumerable.Empty<string>())
                .ToHashSet()
            ?? new();

        // Priority chain:
        // 1) BossClass
        // 2) SoloClass
        // 3) First owned meta solo class
        // 4) Currently equipped class
        string? selectedClass =
      !string.IsNullOrWhiteSpace(Core.BossClass) ? Core.BossClass :
      !string.IsNullOrWhiteSpace(Core.SoloClass) ? Core.SoloClass :
      possibleSoloClasses.FirstOrDefault(ownedClasses.Contains) ??
      Bot.Player?.CurrentClass?.Name ??
      Bot.Player?.CurrentClass?.Name;

        if (selectedClass == null)
        {
            Core.Logger("No suitable class found for soloing \"Voice of the Sea\". Pleaseensure you own one of the recommended classes.");
            return;
        }
        Core.Logger($"Soloing \"Voice of the Sea\" with {selectedClass}");

        Adv.GearStore(EnhAfter: true);
        Adv.SmartEnhance(selectedClass);

        KillThing(
            map: "seavoice",
            mobMapID: 1,
            itemUsed: 78994,
            Class: selectedClass,
            item: "Voice in the Sea Defeated",
            quant: 1,
            isTemp: true
        );

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
        Story.MapItemQuest(9723, "balemorale", new[] { 13179, 13180 });
        Story.KillQuest(9723, "balemorale", "Chaos Spider");

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
        Story.MapItemQuest(9726, "balemorale", 13183);
        Story.KillQuest(9726, "balemorale", new[] { "Chaos Spider", "Chaos Crystal" });

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
        Story.MapItemQuest(9735, "castleeblana", 13204);
        Story.KillQuest(
            9735,
            "castleeblana",
            new[] { "Skye Warrior", "Skye Executor", "Skye Executor" }
        );

        // Harbinger's Tears 9736
        Story.KillQuest(9736, "castleeblana", "Bananach Raven");

        // Spectre of Hunger 9737
        Story.MapItemQuest(9737, "castleeblana", 13205);
        Story.KillQuest(9737, "castleeblana", "Fear Gorta");

        // InnJustice 9738
        Story.MapItemQuest(9738, "castleeblana", 13206);
        Story.KillQuest(9738, "castleeblana", new[] { "Bananach Raven", "Fear Gorta" });

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
        Story.MapItemQuest(9755, "loughshine", 13273);
        Story.KillQuest(9755, "loughshine", "Skye Cailleach");

        // Maigh Eo (9756)
        Story.KillQuest(9756, "loughshine", "Scorched Elder Yew");

        // Resentment Harbour (9757)
        Story.MapItemQuest(9757, "loughshine", 13274, 5);
        Story.MapItemQuest(9757, "loughshine", 13275);

        // Wisened Yew (9758)
        Story.KillQuest(9758, "loughshine", new[] { "Skye Cailleach", "Scorched Elder Yew" });

        // Dragonflare (9759)
        Story.MapItemQuest(9759, "loughshine", 13276);

        // Shock Scatter (9760)
        Story.KillQuest(9760, "loughshine", "Skye Executor");

        // Grace of Three (9761)
        Story.MapItemQuest(9761, "loughshine", 13277);
        Story.KillQuest(9761, "loughshine", "Energy Elemental");

        // Iubhair (9762)
        Story.MapItemQuest(9762, "loughshine", 13278, 5);
        Story.MapItemQuest(9762, "loughshine", 13279);

        // Parting the Clouds (9763)
        Story.MapItemQuest(9763, "loughshine", 13280);
        Story.KillQuest(9763, "loughshine", new[] { "Skye Executor", "Energy Elemental" });

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
        Story.MapItemQuest(9768, "naoisegrave", 13296);
        Story.KillQuest(9768, "naoisegrave", "Bananach Raven");

        // Energetic Feud (9769)
        Story.MapItemQuest(9769, "naoisegrave", 13297);
        Story.KillQuest(9769, "naoisegrave", "Energy Elemental");

        // Glacial Seal (9774)
        Story.MapItemQuest(9774, "naoisegrave", new[] { 13298, 13299 });

        // The Cold Wind Guides (9770)
        Story.MapItemQuest(9770, "naoisegrave", 13300);
        Story.KillQuest(9770, "naoisegrave", new[] { "Energy Elemental", "Bananach Raven" });

        // Cold Callers (9771)
        Story.KillQuest(9771, "naoisegrave", "Ice Guardian");

        // Destinies Lost (9772)
        Story.KillQuest(9772, "naoisegrave", "Bone Dragonling");

        // Of The Sorrows (9773)
        Core.EquipClass(ClassType.Solo);
        Story.MapItemQuest(9773, "naoisegrave", 13301);
        Story.KillQuest(9773, "naoisegrave", "Warden Iseul");

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
        Story.KillQuest(9807, "liatarahill", new[] { "Undead Garde", "Garde Wraith" });

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
        Story.MapItemQuest(9810, "liatarahill", 13368);
        Story.KillQuest(9810, "liatarahill", new[] { "Undead Garde", "Garde Wraith" });

        // Spring Melt (9811)
        Story.MapItemQuest(9811, "liatarahill", 13366);
        Story.KillQuest(9811, "liatarahill", "Ice Guardian");

        // Connla (9812)
        Story.KillQuest(9812, "liatarahill", "Skye Cailleach");

        // Dumha na nGiall (9813)
        Story.MapItemQuest(9813, "liatarahill", 13367);
        Story.KillQuest(9813, "liatarahill", new[] { "Ice Guardian", "Skye Cailleach" });

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
        Story.MapItemQuest(9819, "castlegaheris", 13378);
        Story.KillQuest(9819, "castlegaheris", "Energy Elemental");

        // Lost and Numb (9820)
        Story.KillQuest(9820, "castlegaheris", "Ice Guardian");

        // Childhood Memories (9821)
        Story.MapItemQuest(9821, "castlegaheris", new[] { 13379, 13380, 13381 });

        // Strangled Sobs (9822)
        Story.KillQuest(9822, "castlegaheris", new[] { "Energy Elemental", "Ice Guardian" });

        // Distant Care (9823)
        Story.KillQuest(9823, "castlegaheris", "Dark Cage");

        // Minus K (9824)
        Story.KillQuest(9824, "castlegaheris", "Glacial Crystal");

        // Someone Else's Blood and Tears (9825)
        Story.KillQuest(9825, "castlegaheris", "Elemental Hybrid");

        // Honor Thy Family (9826)
        Story.MapItemQuest(9826, "castlegaheris", new[] { 13382, 13383 });

        // Life's Latest Lows (9827)
        Story.KillQuest(9827, "castlegaheris", new[] { "Glacial Crystal", "Elemental Hybrid" });

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
            Core.HuntMonsterQuest(
                10350,
                ("thelimacity", UseableMonsters[2], ClassType.Farm),
                ("thelimacity", UseableMonsters[1], ClassType.Farm)
            );
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
            Core.HuntMonsterQuest(
                10355,
                ("thelimacity", UseableMonsters[3], ClassType.Solo),
                ("thelimacity", UseableMonsters[4], ClassType.Farm)
            );
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
        Story.MapItemQuest(10359, "mountmaleno", 14805);
        Story.KillQuest(10359, "mountmaleno", "Soul Raven");

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
            Core.HuntMonsterQuest(
                10362,
                ("mountmaleno", UseableMonsters[0], ClassType.Farm),
                ("mountmaleno", UseableMonsters[1], ClassType.Farm)
            );
        }

        // 10363 | Journey Through the Dark Night
        Story.MapItemQuest(10363, "mountmaleno", new[] { 14808, 14809 });

        // 10364 | Lunar Caustic
        Story.MapItemQuest(10364, "mountmaleno", 14810);
        Story.KillQuest(10364, "mountmaleno", UseableMonsters[2]);

        // 10365 | Like Water and Lithium
        Story.MapItemQuest(10365, "mountmaleno", 14811);
        Story.KillQuest(10365, "mountmaleno", UseableMonsters[3]);

        // 10366 | The Shadows Lengthen in…
        Story.MapItemQuest(10366, "mountmaleno", 14812);

        // 10367 | Blithe Hospitality
        if (!Story.QuestProgression(10367))
        {
            Core.HuntMonsterQuest(
                10367,
                ("mountmaleno", UseableMonsters[3], ClassType.Farm),
                ("mountmaleno", UseableMonsters[2], ClassType.Solo)
            );
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
        Story.MapItemQuest(10415, "forgealbedo", 14962);
        Story.KillQuest(10415, "forgealbedo", UseableMonsters[1]);

        // 10416 | The Leap that Never Landed
        Story.MapItemQuest(10416, "forgealbedo", 14963, 4);
        Story.KillQuest(10416, "forgealbedo", UseableMonsters[2]);

        // 10417 | Insistent Existence
        Story.MapItemQuest(10417, "forgealbedo", 14964, 4);
        Story.KillQuest(10417, "forgealbedo", UseableMonsters[3]);

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
        Story.MapItemQuest(10420, "forgealbedo", 14965, 6);
        Story.KillQuest(10420, "forgealbedo", UseableMonsters[1]);

        // 10421 | Lost Admin Privileges
        if (!Story.QuestProgression(10421))
        {
            Core.HuntMonsterQuest(
                10421,
                ("forgealbedo", UseableMonsters[1], ClassType.Farm),
                ("forgealbedo", UseableMonsters[5], ClassType.Farm)
            );
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
                Core.HuntMonsterQuest(10735,
                    ("forgecitrinitas", UseableMonsters[5], ClassType.Solo),
                    ("forgecitrinitas", UseableMonsters[3], ClassType.Farm),
                    ("forgecitrinitas", UseableMonsters[4], ClassType.Farm));
            }
        }

        else
        {
            // 10736 | Silica Spring (Legend)
            if (!Story.QuestProgression(10736))
            {
                Core.HuntMonsterQuest(10736,
                    ("forgecitrinitas", UseableMonsters[5], ClassType.Solo),
                    ("forgecitrinitas", UseableMonsters[3], ClassType.Farm),
                    ("forgecitrinitas", UseableMonsters[4], ClassType.Farm));
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

    public void KillThing(string map, int mobMapID, int itemUsed, string Class, string item, int quant = 1, bool isTemp = false)
    {
        string? classFromPlayer = Bot.Player.CurrentClass?.Name;
        string? classNameToUse = Class ?? classFromPlayer;

        if (string.IsNullOrWhiteSpace(classNameToUse))
        {
            Core.Logger("KillThing aborted: no class specified and player has no current class.");
            return;
        }

        Core.Equip(classNameToUse);

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);

        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
        Core.Equip(itemUsed);

        Core.Logger($"{itemUsed} [Vigil] Equipped? {Bot.Inventory?.IsEquipped("Vigil")}");

        if (Bot.Player.Cell != "r2")
        {
            Core.Jump("r2", "Left");
            Bot.Wait.ForCellChange("r2");
        }

        Bot.Player.SetSpawnPoint();
        Bot.Skills.Stop();

        int skillIndex = 0;
        int[] skillList = { 1, 2, 3, 4 };

        try
        {
            while (
                !Bot.ShouldExit
                && (isTemp ? !Bot.TempInv.Contains(item, quant) : !Core.CheckInventory(item, quant))
            )
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    skillIndex = 0;
                    continue;
                }

                if (Bot.Player.Cell != "r2")
                {
                    Core.Jump("r2", "Left");
                    Bot.Wait.ForCellChange("r2");
                }

                // Use Vigil to cleanse Oxidize.
                if (Bot.Self.HasActiveAura("Oxidize") && !Bot.Self.HasActiveAura("Vigil"))
                {
                    if (Bot.Skills.CanUseSkill(5))
                        Bot.Skills.UseSkill(5);

                    Bot.Sleep(500);
                    continue;
                }

                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack("*");

                if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
                    continue;

                if (Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    if (Bot.Skills.UseSkill(skillList[skillIndex]))
                        skillIndex = (skillIndex + 1) % skillList.Length;
                }
                Bot.Sleep(200);
            }
        }
        finally
        {
            Bot.Skills.Resume();
            Bot.Combat.StopAttacking = false;
        }
        Core.Logger($"KillThing completed for {item} ({quant}).");
    }

}
