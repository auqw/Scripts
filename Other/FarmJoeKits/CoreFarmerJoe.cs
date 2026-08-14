/*
name: null
description: null
tags: null
*/
#region includes
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Dailies/0AllDailies.cs
//cs_include Scripts/Dailies/Cryomancer.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Enhancement/InventoryEnhancer.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Good/GearOfAwe/CapeOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnFortressMerge.cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowrealmMerge.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Nation/AFDL/EnoughDOOMforanArchfiend.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Other/Classes/BloodSorceress.cs
//cs_include Scripts/Other/Classes/Daily-Classes/BlazeBinder.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Other/Classes/DragonShinobi.cs
//cs_include Scripts/Other/Classes/Dragonslayer.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/REP-based/DarkbloodStormKing.cs
//cs_include Scripts/Other/Classes/REP-based/EternalInversionist.cs
//cs_include Scripts/Other/Classes/REP-based/GlacialBerserker.cs
//cs_include Scripts/Other/Classes/REP-based/MasterRanger.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Other/Classes/ScarletSorceress.cs
//cs_include Scripts/Other/Classes/FrostSpiritReaver.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/FreeBoosts/FreeBoostsQuest(10mns)[Rng].cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
//cs_include Scripts/Other/MergeShops/SynderesMerge.cs
//cs_include Scripts/Other/MergeShops/YulgarsUndineMerge.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Other/Weapons/DualChainSawKatanas.cs
//cs_include Scripts/Other/Weapons/EnchantedVictoryBladeWeapons.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/ExaltedApotheosisPreReqs.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Seasonal/Frostvale/NorthlandsMonk.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/AgeofRuin/CoreAOR.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/Tutorial.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Tools/BankAllItems.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Other/Classes/KingsEcho.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
#endregion includes


using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class CoreFarmerJoe
{
    #region dependicies
    //other
    public static IScriptInterface Bot => IScriptInterface.Instance;
    private static FreeBoosts Boosts
    {
        get => _Boosts ??= new FreeBoosts();
        set => _Boosts = value;
    }
    private static FreeBoosts _Boosts;
    private static FarmAllDailies FAD
    {
        get => _FAD ??= new FarmAllDailies();
        set => _FAD = value;
    }
    private static FarmAllDailies _FAD;
    private static InventoryEnhancer InvEn
    {
        get => _InvEn ??= new InventoryEnhancer();
        set => _InvEn = value;
    }
    private static InventoryEnhancer _InvEn;
    private static SynderesMerge SM
    {
        get => _SM ??= new SynderesMerge();
        set => _SM = value;
    }
    private static SynderesMerge _SM;
    private static ArchfiendDeathLord AFDeath
    {
        get => _AFDeath ??= new ArchfiendDeathLord();
        set => _AFDeath = value;
    }
    private static ArchfiendDeathLord _AFDeath;
    private static UnlockForgeEnhancements UnlockForgeEnhancements
    {
        get => _UnlockForgeEnhancements ??= new UnlockForgeEnhancements();
        set => _UnlockForgeEnhancements = value;
    }
    private static UnlockForgeEnhancements _UnlockForgeEnhancements;
    private static ExaltedApotheosisPreReqs ExaltedApotheosisPreReqs
    {
        get => _ExaltedApotheosisPreReqs ??= new ExaltedApotheosisPreReqs();
        set => _ExaltedApotheosisPreReqs = value;
    }
    private static ExaltedApotheosisPreReqs _ExaltedApotheosisPreReqs;

    private static EnoughDOOMforanArchfiend EnoughDOOMforanArchfiend
    {
        get => _EnoughDOOMforanArchfiend ??= new EnoughDOOMforanArchfiend();
        set => _EnoughDOOMforanArchfiend = value;
    }
    private static EnoughDOOMforanArchfiend _EnoughDOOMforanArchfiend;


    //Cores
    public static CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CapeOfAwe COA
    {
        get => _COA ??= new CapeOfAwe();
        set => _COA = value;
    }
    private static CapeOfAwe _COA;
    public static Core13LoC LOC => new();
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreVHL VHL
    {
        get => _VHL ??= new CoreVHL();
        set => _VHL = value;
    }
    private static CoreVHL _VHL;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreYnR YNR
    {
        get => _YNR ??= new CoreYnR();
        set => _YNR = value;
    }
    private static CoreYnR _YNR;

    //Classes
    private static MasterRanger MR
    {
        get => _MR ??= new MasterRanger();
        set => _MR = value;
    }
    private static MasterRanger _MR;
    private static Shaman Shaman
    {
        get => _Shaman ??= new Shaman();
        set => _Shaman = value;
    }
    private static Shaman _Shaman;
    private static GlacialBerserker GB
    {
        get => _GB ??= new GlacialBerserker();
        set => _GB = value;
    }
    private static GlacialBerserker _GB;
    private static StoneCrusher SC
    {
        get => _SC ??= new StoneCrusher();
        set => _SC = value;
    }
    private static StoneCrusher _SC;
    private static DragonShinobi DS
    {
        get => _DS ??= new DragonShinobi();
        set => _DS = value;
    }
    private static DragonShinobi _DS;
    private static ArchPaladin AP
    {
        get => _AP ??= new ArchPaladin();
        set => _AP = value;
    }
    private static ArchPaladin _AP;
    private static Dragonslayer DSlayer
    {
        get => _DSlayer ??= new Dragonslayer();
        set => _DSlayer = value;
    }
    private static Dragonslayer _DSlayer;
    private static DragonslayerGeneral DSG
    {
        get => _DSG ??= new DragonslayerGeneral();
        set => _DSG = value;
    }
    private static DragonslayerGeneral _DSG;
    private static LordOfOrder LOO
    {
        get => _LOO ??= new LordOfOrder();
        set => _LOO = value;
    }
    private static LordOfOrder _LOO;
    private static ScarletSorceress SS
    {
        get => _SS ??= new ScarletSorceress();
        set => _SS = value;
    }
    private static ScarletSorceress _SS;
    private static EternalInversionist EI
    {
        get => _EI ??= new EternalInversionist();
        set => _EI = value;
    }
    private static EternalInversionist _EI;
    private static DarkbloodStormKing DBSK
    {
        get => _DBSK ??= new DarkbloodStormKing();
        set => _DBSK = value;
    }
    private static DarkbloodStormKing _DBSK;
    private static DragonOfTime DoT
    {
        get => _DoT ??= new DragonOfTime();
        set => _DoT = value;
    }
    private static DragonOfTime _DoT;
    private static BloodSorceress BS
    {
        get => _BS ??= new BloodSorceress();
        set => _BS = value;
    }
    private static BloodSorceress _BS;
    private static BlazeBinder Bb
    {
        get => _Bb ??= new BlazeBinder();
        set => _Bb = value;
    }
    private static BlazeBinder _Bb;
    private static ArchFiend AF
    {
        get => _AF ??= new ArchFiend();
        set => _AF = value;
    }
    private static ArchFiend _AF;
    private static Cryomancer Cryo
    {
        get => _Cryo ??= new Cryomancer();
        set => _Cryo = value;
    }
    private static Cryomancer _Cryo;
    private static FrostSpiritReaver FSR
    {
        get => _FSR ??= new FrostSpiritReaver();
        set => _FSR = value;
    }
    private static FrostSpiritReaver _FSR;
    private static NorthlandsMonk NM
    {
        get => _NM ??= new NorthlandsMonk();
        set => _NM = value;
    }
    private static NorthlandsMonk _NM;

    //Weapons
    private static DualChainSawKatanas DCSK
    {
        get => _DCSK ??= new DualChainSawKatanas();
        set => _DCSK = value;
    }
    private static DualChainSawKatanas _DCSK;
    private static BurningBlade BB
    {
        get => _BB ??= new BurningBlade();
        set => _BB = value;
    }
    private static BurningBlade _BB;
    private static BurningBladeOfAbezeth BBOA
    {
        get => _BBOA ??= new BurningBladeOfAbezeth();
        set => _BBOA = value;
    }
    private static BurningBladeOfAbezeth _BBOA;
    private static EnchantedVictoryBladeWeapons EVBW
    {
        get => _EVBW ??= new EnchantedVictoryBladeWeapons();
        set => _EVBW = value;
    }
    private static EnchantedVictoryBladeWeapons _EVBW;
    private static ShadowrealmMerge SRM
    {
        get => _SRM ??= new ShadowrealmMerge();
        set => _SRM = value;
    }
    private static ShadowrealmMerge _SRM;

    //Story
    private static Tutorial Tutorial
    {
        get => _Tutorial ??= new Tutorial();
        set => _Tutorial = value;
    }
    private static Tutorial _Tutorial;
    private static CelestialArenaQuests CAQ
    {
        get => _CAQ ??= new CelestialArenaQuests();
        set => _CAQ = value;
    }
    private static CelestialArenaQuests _CAQ;
    private static GlaceraStory GS
    {
        get => _GS ??= new GlaceraStory();
        set => _GS = value;
    }
    private static GlaceraStory _GS;
    private static Mazumi Mazumi
    {
        get => _Mazumi ??= new Mazumi();
        set => _Mazumi = value;
    }
    private static Mazumi _Mazumi;
    private static KingsEcho KE
    {
        get => _KE ??= new KingsEcho();
        set => _KE = value;
    }
    private static KingsEcho _KE;
    #endregion

    private const int RANK_10_CLASS_POINTS = 302500;
    public string OptionsStorage = "FarmerJoePet";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>( "OutFit", "Get a Pre-Made Outfit, Curtious of the Community", "We are farmers, bum ba dum bum bum bum bum", false ),
        new Option<bool>("EquipOutfit", "Equip our the FarmerJoe outfit at the end?", "Yay or Nay", false),
        new Option<bool>("SellStarterClasses", "SellStarterClasses", "Yay or Nay", false),
        new Option<bool>( "GetBoosts", "GetBoosts", "Do the \"Free Boosts\" quests to get some 10minute boosts (the droprate is < 1% so itll take awhile)", false ),
        // new Option<bool>( "EquipBoostingGear", "EquipBoostingGear", "use a predetermined set of gear based on what you own, and certain paramiters set, otherwise use whats set for your solo/farm class gear in CBO", true ),
        new Option<PetChoice>( "PetChoice", "Choose Your Pet", "Extra stuff to choose, if you have any suggestions -form in disc, and put it under request. or dm Tato(the retarded one on disc)", PetChoice.None ),
        CoreBots.Instance.SkipOptions,
    };

    public static void ScriptMain(IScriptInterface bot) => Core.RunCore();

    #region InvClasses

    private readonly Dictionary<string, InventoryItem?> _classCache = new(
        StringComparer.OrdinalIgnoreCase
    );

    private InventoryItem? GetClass(string className)
    {
        if (_classCache.TryGetValue(className, out InventoryItem? cached))
            return cached;

        InventoryItem? item = Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(i =>
                i.Name?.Trim().Equals(className, StringComparison.OrdinalIgnoreCase) == true
                && i.Category == ItemCategory.Class
            );

        _classCache[className] = item; // cache result
        return item;
    }

    // Strongly-typed properties
    private InventoryItem? ClassNinja => GetClass("Ninja");
    private InventoryItem? ClassRogue => GetClass("Rogue");
    private InventoryItem? ClassMage => GetClass("Mage");
    private InventoryItem? ClassMasterRanger => GetClass("Master Ranger");
    private InventoryItem? ClassScarletSorceress => GetClass("Scarlet Sorceress");
    private InventoryItem? ClassBlazeBinder => GetClass("Blaze Binder");
    private InventoryItem? ClassDragonSoulShinobi => GetClass("DragonSoul Shinobi");
    private InventoryItem? ClassArchPaladin => GetClass("ArchPaladin");
    private InventoryItem? ClassArchFiend => GetClass("ArchFiend");
    private InventoryItem? ClassGlacialBerserker => GetClass("Glacial Berserker");
    private InventoryItem? ClassCryomancer => GetClass("Cryomancer");
    private InventoryItem? ClassDragonOfTime => GetClass("Dragon of Time");
    private InventoryItem? ClassDragonslayer => GetClass("Dragonslayer");
    private InventoryItem? ClassDragonslayerGeneral => GetClass("Dragonslayer General");

    #endregion InvClasses

    // Class lists for solo and farm activities
    string[] soloClasses = new[]
    {
        "King's Echo",
        "Chaos Avenger",
        "Void Highlord",
        "Void Highlord (IoDA)",
        "Legion Revenant",
        "Dragon of Time",
        "ArchPaladin",
        "Lord of Order",
        "Cryomancer",
        "Dragonslayer General",
        "Glacial Berserker",
        "Dragonslayer",
        "DragonSoul Shinobi",
        "Assassin",
        "Ninja Warrior",
        "Classic Ninja",
        "Ninja",
        "Ninja (Rare)",
        "Rogue (Rare)",
        "Rogue",
        "Healer (Rare)",
        "Healer",
    };

    string[] farmClasses = new[]
    {
        "Legion Revenant",
        "ArchMage",
        "Dragon of Time",
        "Archfiend",
        "Blaze Binder",
        "Scarlet Sorceress",
        "Pyromancer",
        "Master Ranger",
        "Mage (Rare)",
        "Mage",
    };

    string[] dodgeClasses = new[]
    {
            "Great Thief",
            "Yami No Ronin",
            "Chrono Assassin",
            "Horc Evader",
            "Assassin",
            "Ninja Warrior",
            "Classic Ninja",
            "Ninja",
            "Ninja (Rare)",
            "Rogue (Rare)",
            "Rogue"
    };

    /// <summary>
    /// Orchestrates the entire progression process from level 1 to endgame, including:
    /// - **Leveling:** Progresses through levels 1 to 100 with specific milestones and enhancements.
    /// - **Item Acquisition:** Handles the acquisition of necessary items and classes at each stage.
    /// - **Outfit Setup:** Configures the character's outfit and equipment.
    /// - **Pet Management:** Acquires and equips specified pets.
    /// </summary>
    public void DoAll()
    {
        Adv.GearStore(EnhAfter: true);
        Level1to30();
        Level30to75();
        Level75to100();
        EndGame();
        Outfit();
        Pets(PetChoice.HotMama);
        Pets(PetChoice.Akriloth);
        Adv.GearStore(true, EnhAfter: true);
    }

    /// <summary>
    /// Guides the character from level 1 to 30, focusing on acquiring essential beginner items and class enhancements.
    /// - **Beginner Items:** Ensures the character is equipped with necessary beginner gear and classes.
    /// - **Desync Warning:** Provides a safety note about potential desynchronization issues with class points at Rank 9, advising a relog if necessary.
    /// </summary>
    public void Level1to30()
    {
        // Beginner Items
        BeginnerItems();

        //safety incase it desyncs.. the relog fuction isnt exactly perfect
        Core.Logger(
            "Class points may be desynced at Rank 9, if you are stuck at rank 9, please stop the bot & relog if this happens"
        );
    }

    bool HasEnhancedThisBracket = false;

    #region Leve30 to 75
    /// <summary>
    /// Progresses the character from level 30 to 75 by acquiring essential items and enhancing classes. This includes:
    /// - **Item Acquisition:** Obtaining items like "Awethur's Accoutrements," "Master Ranger," "Burning Blade", "Scarlet Sorceress," "Blaze Binder," "DragonSoul Shinobi," "ArchPaladin," and "ArchFiend DeathLord."
    /// - **Class Enhancements:** Ensuring classes are ranked up and equipped as needed.
    /// - **Experience Farming:** Efficiently farming experience for each level milestone (30, 50, 55, 60, 65, 70, and 75).
    /// </summary>
    public void Level30to75()
    {
        //Preset Solo & FarmClass (required if additional Classes were pre-aquired before the script or your restarting it and CBO wasnt saved.)
        SetClass();
        // Check for Elders' Blood daily if player is level 30 or higher
        if (Bot.Player.Level >= 30 && Daily.CheckDailyv2(802, true, true, "Elders' Blood"))
            Daily.EldersBlood();

        Core.Logger(
            "Joe will handle each level bracket (0-30, 30-50, 50-55, 55-60, 60-65, 65-75, 75-100)\n doing the leveling first, then acquiring classes and items after\n so its prepared... and hopefully strong enough to handle the next level bracket."
        );

        if (Bot.Config!.Get<bool>("GetBoosts"))
        {
            Core.OneTimeMessage(
                "GetBoosts Message",
                "We'll occasionaly get rep/class/gold boosts throughout the script to help speed things up a bit (the quest's droprate is low, but fast kills - you can disable this in scripts button > edit options ( when script is stopped).)"
            );
            // Always ensure we have 10 of each boost type
            Farm.GetBoost("REP", 10, true);
            Farm.GetBoost("XP", Bot.Player.Level >= 100 ? 0 : 10, true);
            Boosts.GetBoostsSelect(10, 10, 0);
        }

        var levelHandlers = new Dictionary<int, Action>
        {
            { 30, HandleLevel30 },
            { 50, HandleLevel50 },
            { 55, HandleLevel55 },
            { 60, HandleLevel60 },
            { 65, HandleLevel65 },
            { 70, HandleLevel70 },
            { 75, HandleLevel75 },
            { 80, Handlelevel80 },
        };

        foreach (int level in new[] { 30, 50, 55, 60, 65, 70, 75, 80 })
        {
            if (Bot.Player.Level > 75)
                HasEnhancedThisBracket = true;

            // // Smart enhance at start of each bracket
            if (
                !HasEnhancedThisBracket
                || Bot.Inventory.Items.Any(x => x != null && x.EnhancementLevel < Bot.Player.Level)
            )
            {
                Adv.SmartEnhance(Bot.Player.CurrentClass?.Name ?? string.Empty);
                HasEnhancedThisBracket = true;
            }

            // Level up to target
            Farm.Experience(level);

            // Execute level-specific handler if it exists
            if (levelHandlers.TryGetValue(level, out Action? handler))
            {
                Bot.Log($"Level Handler: {level} bracket");
                handler();
                BankAllUnusedClasses();
            }
            // Reset enhancement flag for next bracket
            HasEnhancedThisBracket = false;
        }
    }

    private static bool AnyRank10(string[] names)
    {
        foreach (string name in names)
        {
            if (
                Bot.Inventory.TryGetItem(name, out InventoryItem? item)
                && item?.Category == ItemCategory.Class
                && item.Quantity >= RANK_10_CLASS_POINTS
            )
            {
                return true;
            }
        }
        return false;
    }

    private void HandleLevel30()
    {
        if (
            !Core.CheckInventory("Master Ranger", toInv: false)
            || !AnyRank10(new[] { "Master Ranger" })
        )
        {
            if (Core.CheckInventory("Venom Head"))
                Core.SellItem("Venom Head");

            Core.Logger("Level 30: Acquiring Master Ranger");
            SetClass();
            MR.GetMR();
        }

        if (
            !Core.CheckInventory("Dragonslayer", toInv: false)
            || !AnyRank10(new[] { "Dragonslayer" })
        )
        {
            Core.Logger("Level 30: Acquiring Dragonslayer");
            SetClass();
            DSlayer.GetDragonslayer();
        }

        // --- BLADE OF AWE PROGRESSION ---
        string[] aweItems = new[]
        {
            "Burning Blade of Abezeth",
            "Burning Blade",
            "Awethur's Accoutrements",
        };

        Core.Logger("Level 30: Farming Blade of Awe Rep for enhancements & sword");
        SetClass();
        Farm.BladeofAweREP();
        Adv.BuyItem("museum", 631, "Awethur's Accoutrements");
        UnlockForgeEnhancements.ForgeHelmEnhancement();
    }

    private void HandleLevel50()
    {
        // --- SCARLET SORCERESS / FARM ALTERNATIVES ---
        if (
            !Core.CheckInventory("Scarlet Sorceress", toInv: false)
            || !AnyRank10(new[] { "Scarlet Sorceress" })
        )
        {
            Core.Logger("Level 50: Acquiring Scarlet Sorceress");
            SetClass();
            SS.GetSSorc();
        }

        if (
            !Core.CheckInventory("Dragonslayer General", toInv: false)
            || !AnyRank10(new[] { "Dragonslayer General" })
        )
        {
            Core.Logger("Level 50: Acquiring Dragonslayer General");
            SetClass();
            DSG.GetDSGeneral();
        }

        Core.Logger("Level 50: Acquiring Burning Blade");
        SetClass();
        BB.GetBurningBlade();
    }

    private void HandleLevel55()
    {
        // Blaze Binder
        if (
            !Core.CheckInventory("Blaze Binder", toInv: false)
            || !AnyRank10(new[] { "Blaze Binder" })
        )
        {
            Core.Logger("Level 55: Acquiring Blaze Binder");
            SetClass();
            Bb.GetClass();
        }

        // Cryomancer
        if (!Core.CheckInventory("Cryomancer", toInv: false) || !AnyRank10(new[] { "Cryomancer" }))
        {
            Core.Logger("Level 55: Acquiring Cryomancer");
            SetClass();
            Cryo.DoCryomancer();
        }
    }

    private void HandleLevel60()
    {
        if (
            !Core.CheckInventory("DragonSoul Shinobi", toInv: false)
            || !AnyRank10(new[] { "DragonSoul Shinobi" })
        )
        {
            Core.Logger("Level 60: Acquiring DragonSoul Shinobi");
            SetClass();
            DS.GetDSS();
        }

        UnlockForgeEnhancements.Lacerate();
    }

    private void HandleLevel65()
    {
        if (
            !Core.CheckInventory("Glacial Berserker", toInv: false)
            || !AnyRank10(new[] { "Glacial Berserker" })
        )
        {
            Core.Logger("Level 65: Acquiring Glacial Berserker");
            SetClass();
            GB.GetGB();
        }
    }

    private void HandleLevel70()
    {
        // Horc Evader
        if (
            !Core.CheckInventory("Horc Evader", toInv: false) || !AnyRank10(new[] { "Horc Evader" })
        )
        {


            Core.Logger("Geting Horc Evader so we can attemp to kill Ultra Chaos Alteon during the story to get AP");
            Adv.BuyItem("bloodtusk", 308, "Horc Evader");
            SetClass();
        }

        Core.Logger("unlocking the easy lvl 70 forge enhs");
        UnlockForgeEnhancements.Smite();
        UnlockForgeEnhancements.Vim();
        UnlockForgeEnhancements.Pneuma();
        UnlockForgeEnhancements.Examen();
        UnlockForgeEnhancements.Anima();

        // ArchPaladin
        if (
            !Core.CheckInventory("ArchPaladin", toInv: false) || !AnyRank10(new[] { "ArchPaladin" })
        )
        {
            Core.Logger("Level 70: Acquiring ArchPaladin");
            SetClass();
            AP.GetAP();
        }
    }

    private void HandleLevel75()
    {
        // Archfiend DoomLord (+30 dmgAll)
        if (
            (
                !Core.CheckInventory("Archfiend DoomLord", toInv: false)
                || !AnyRank10(new[] { "Archfiend" })
            ) || !Adv.HasMinimalBoost(GenericGearBoost.dmgAll, 30)
        )
        {
            Core.Logger("Level 75: Acquiring Archfiend DoomLord for 35% more damage to any monster, and gives 25% more class points, XP, gold, and rep when equipped");
            SetClass();
            EnoughDOOMforanArchfiend.AFDL();
        }

        // Archfiend
        string[] af = new[] { "Archfiend" };
        if (!Core.CheckInventory("Archfiend", toInv: false) || !AnyRank10(new[] { "Archfiend" }))
        {
            Core.Logger("Level 75: Acquiring Archfiend");
            SetClass();
            AF.GetArchfiend();
        }
    }

    private void Handlelevel80()
    {
        if (Core.CheckInventory(new[] { "Void Highlord", "Void Highlord (IoDA)" }, any: true))
        {
            Core.Logger("Dragon of Time found but not rank 10 - ranking up");
            SetClass();
            return;
        }
        Core.Logger("Bracket level [80]; Attemping to aquire VHL (Elders' bloods are a daily, this will take a total of 17 days ( minus your current roents/elders' quantity))");
        VHL.GetVHL();
    }

    #endregion Leve 30-75

    /// <summary>
    /// Advances the character from level 75 to 100.
    ///
    /// Phase 1: Prepare healer class (Dragon of Time or Healer) for 13 LoC
    /// Phase 2: Acquire Enchanted Cape of Awe and complete 13 LoC
    /// Phase 3: Get Lord of Order daily, acquire additional farming classes (FSR, Northlands Monk, Shaman),
    ///          and unlock helmet + weapon enhancements
    /// Phase 4: Level to 80, complete Celestial Arena QuestLine for BBoA, unlock cape enhancements,
    ///          acquire YnR and Dragon of Time, level to 100, and obtain Hollowborn Reaper's Scythe
    /// </summary>
    public void Level75to100()
    {
        // --- Phase 1: Healer / Dragon of Time ---
        Core.Logger("Phase 1: Class Preparation - Healer/Dragon of Time for 13 LoC");

        if (Core.CheckInventory("Dragon of Time", toInv: false))
        {
            if (ClassDragonOfTime?.Quantity < RANK_10_CLASS_POINTS)
            {
                Core.Logger("Dragon of Time found but not rank 10 - ranking up");
                SetClass();
            }
        }
        else
        {
            string healer = Core.CheckInventory("Healer (Rare)") ? "Healer (Rare)" : "Healer";
            if (!Core.CheckInventory(new[] { "Healer", "Healer (Rare)" }, any: true))
            {
                Core.Logger("No healing class found! Acquiring the `Healer` class to do `Xang` later during the 13LoC story quests ( as healing classes basicly NUKE her....).");
                Adv.BuyItem("classhalla", 176, "Healer");
                Adv.RankUpClass(healer);
            }
        }

        // --- Phase 2: Chaos quests & enhancements ---
        Core.Logger("Phase 2: Chaos Shenanigans - CoA & 13 LoC");
        COA.GetCoA();
        Core.Equip("Cape of Awe");
        SetClass();
        LOC.Complete13LOC();

        // --- Phase 3: Solo Classes & Weapons ---
        Core.Logger("Phase 3: Solo Classes & Weapon - LoO Daily");
        LOO.GetLoO();
        SetClass();

        // Bank any LoO rewards
        Core.ToBank(Core.EnsureLoad(7156).Rewards.Select(i => i.Name).ToArray());

        string[] additionalClasses =
        {
            "Frost Spirit Reaver",
            "Northlands Monk",
            "Shaman"
        };

        Action[] getters =
        {
            () => FSR.GetFSR(),
            () => NM.GetNlMonk(),
            () => Shaman.GetShaman()
        };

        for (int i = 0; i < additionalClasses.Length; i++)
        {
            string className = additionalClasses[i];

            bool hasClass = Core.CheckInventory(className, toInv: false);

            // If player doesn't own the class, try to get it ONCE
            if (!hasClass)
            {
                Core.Logger($"{className} not owned — attempting acquisition.");
                SetClass();
                getters[i].Invoke();

                // Recheck after attempt. If still missing, skip (seasonal / unavailable)
                if (!Core.CheckInventory(className, toInv: false))
                {
                    Core.Logger($"{className} unavailable — skipping.");
                    continue;
                }
            }

            // At this point the class exists → now ensure rank 10
            if (!AnyRank10(new[] { className }))
            {
                Core.Logger($"Ranking {className} to Rank 10.");
                SetClass();
                getters[i].Invoke();
            }
        }


        // Unlock Helmet Enhancements
        Core.Logger("Phase 3: Unlocking Helmet Enhancements");

        // Unlock Weapon Enhancements
        Core.Logger("Phase 3: Unlocking Weapon Enhancements");
        UnlockForgeEnhancements.ForgeWeaponEnhancement();
        UnlockForgeEnhancements.Praxis();

        // --- Phase 4: Leveling & Endgame ---
        Core.Logger("Phase 4: Leveling to 80");
        Farm.Experience(80);

        Core.Logger("Phase 4: Celestial Arena QuestLine for Blinding Bright of Awe (BBoA)");
        SetClass();
        CAQ.DoAll();
        BBOA.GetBBoA();

        // Unlock Cape Enhancements
        Core.Logger("Phase 4: Unlocking Cape Enhancements");
        UnlockForgeEnhancements.ForgeCapeEnhancement();
        UnlockForgeEnhancements.Absolution();
        UnlockForgeEnhancements.Vainglory();
        UnlockForgeEnhancements.Avarice();
        UnlockForgeEnhancements.Lament();

        // Acquire Yin & Yang Roentgenium
        Core.Logger("Phase 4: Acquiring Yami no Ronin (YnR)");
        YNR.GetYnR();

        // Acquire Dragon of Time (ensure class is set)
        Core.Logger("Phase 4: Acquiring Dragon of Time");
        SetClass();
        DoT.GetDoT();

        // Final leveling to 100
        Core.Logger("Phase 4: Final Leveling to 100");
        SetClass();
        Farm.Experience();

        // Get King's Echo
        SetClass();
        KE.GetKE();

        // Acquire Hollowborn Reaper's Scythe
        Core.Logger("Phase 4: Acquiring Hollowborn Reaper's Scythe");
        SRM.BuyAllMerge("Hollowborn Reaper's Scythe");

        Core.Logger("Level 75 to 100 progression complete!");
    }

    /// <summary>
    /// Completes endgame preparations and tasks, including:
    /// - **Outfit Setup:** Configuring the character's outfit if specified.
    /// - **Class Setup:** Ensuring the appropriate class is set.
    /// - **Item Preparation:** Pre-farming and enhancing key items like "Hero's Valiance," "Elysium," "Arcana's Concerto," "Ravenous," and "DauntLess."
    /// - **Apotheosis:** Completing prerequisites for the Exalted Apotheosis.
    /// </summary>
    public void EndGame()
    {
        #region Ending & Extras

        if (Bot.Config!.Get<bool>("OutFit"))
            Outfit();

        SetClass();

        #region  Prefarm some non-Skua-able items:

        //Prep for remaning Enh:
        Core.Logger(
            "P5: Preparing for Remaining Enhancements: Heros Valiance, Elysium, Arcanas Concerto, Ravenous, DauntLess"
        );
        UnlockForgeEnhancements.HerosValiance();
        if (Core.CheckInventory("The Divine Will"))
            UnlockForgeEnhancements.Elysium();
        // UnlockForgeEnhancements.ArcanasConcerto();
        // UnlockForgeEnhancements.DauntLess();
        if (Core.CheckInventory("Void Highlord") && Core.CheckInventory("Roentgenium of Nulgath", 10))
            UnlockForgeEnhancements.Ravenous();

        // Apotheosis:
        Core.Logger("P5: Apotheosis prereqs");
        ExaltedApotheosisPreReqs.PreReqs();

        // Max nulgath materials

        Core.Logger("P6: Max nation materials via Supplies");
        Nation.Supplies();

        #endregion Prefarm some non-Skua-able items:

        #endregion Ending & Extras
    }

    /// <summary>
    /// Manages the setup of a character's outfit by handling class configuration, item acquisition, and equipment. This includes:
    /// - **Class Setup:** Configuring the character's class.
    /// - **Basic Equipment:** Equipping items like shirts, hats, and enhancing the current class.
    /// - **Additional Setup:** Handling pets and equipping the complete outfit if specified in the configuration.
    /// </summary>
    public void Outfit()
    {
        SetClass();

        // Easy Difficulty Stuff
        ShirtAndHat();
        ServersAreDown();
        Adv.SmartEnhance(Bot.Player.CurrentClass?.Name ?? string.Empty);

        // Extra Stuff
        Pets();

        if (Bot.Config!.Get<bool>("EquipOutfit"))
        {
            Core.Equip(
                new[]
                {
                    "NO BOTS Armor",
                    "Scarecrow Hat",
                    "The Server is Down",
                    "Hollowborn Reaper's Scythe",
                }
            );
            Core.Equip(Bot.Config.Get<PetChoice>("PetChoice").ToString());
        }

        Core.Logger("We are farmers, bum ba dum bum bum bum bum");
    }

    #region other stuff

    #region Extra:
    /// <summary>
    /// Manages the acquisition and equipping of pets based on the user's configuration or specified choice.
    /// - **Config-Based Acquisition:** Checks the configuration for the selected pet and acquires it if not already owned.
    /// - **Specific Pets:**
    ///   - **Hot Mama:** Acquires and equips the "Hot Mama" pet if selected and not already in inventory.
    ///   - **Akriloth Pet:** Acquires and equips the "Akriloth Pet" if selected and not already in inventory.
    /// </summary>
    /// <param name="petChoice">Selected pet choice</param>
    public void Pets(PetChoice petChoice = PetChoice.None)
    {
        var configPetChoice = Bot.Config!.Get<PetChoice>("Pets");

        if (configPetChoice == PetChoice.None)
            return;

        if (configPetChoice == PetChoice.HotMama && !Core.CheckInventory("Hot Mama"))
        {
            SetClass();
            Core.HuntMonster("battleundere", "Hot Mama", "Hot Mama", isTemp: false, log: false);
            Bot.Wait.ForPickup("Hot Mama");
            Core.Equip("Hot Mama");
        }

        if (configPetChoice == PetChoice.Akriloth && !Core.CheckInventory("Akriloth Pet"))
        {
            SetClass();
            Core.HuntMonster("gravestrike", "Ultra Akriloth", "Akriloth Pet", isTemp: false, log: false); Bot.Wait.ForPickup("Akriloth Pet");
            Core.Equip("Akriloth Pet");
        }
    }

    /// <summary>
    /// Acquires and equips a shirt and hat for the character.
    /// - **NO BOTS Armor:** Purchases and merges the "NO BOTS Armor."
    /// - **Scarecrow Hat:** Buys the "Scarecrow Hat" from Yulgar's shop.
    /// </summary>
    public static void ShirtAndHat()
    {
        Core.FarmingLogger("NO BOTS Armor", 1);
        SM.BuyAllMerge(buyOnlyThis: "NO BOTS Armor");
        Core.FarmingLogger("Scarecrow Hat", 1);
        Adv.BuyItem("yulgar", 16, "Scarecrow Hat");
    }

    /// <summary>
    /// Hunts a specific monster to obtain and equip an item related to server downtime.
    /// - **Item Acquisition:** Hunts the "Rabid Server Hamster" to get the "The Server is Down" item if not already in inventory.
    /// - **Equipping:** Equips the obtained item once acquired.
    /// </summary>
    public void ServersAreDown()
    {
        if (Core.CheckInventory("The Server is Down"))
            return;

        Core.FarmingLogger("The Server is Down", 1);
        SetClass();
        Core.HuntMonster("undergroundlabb", "Rabid Server Hamster", "The Server is Down", isTemp: false, log: false); Bot.Wait.ForPickup("The Server is Down");
        Core.Equip("The Server is Down");
    }

    /// <summary>
    /// Sets up a character with essential beginner items and classes. This includes:
    /// - **Equipping Basic Gear:**
    ///   - **Helm and Cape:** Equips the "Battle Oracle Hood" and "Battle Oracle Wings" if not already equipped.
    ///   - **Default Weapons:** Replaces any default weapon with the "Battle Oracle Battlestaff" and sells the default weapon.
    /// - **Class Preparation:**
    ///   - **Skipping Setup:** If the character is level 30 or higher and has the required classes, skips further setup.
    ///   - **Initial Setup:** Acquires initial badges, level up to 10, and equips a "Rogue" or "Ninja" class if not already owned.
    ///   - **Final Setup:** Obtains and ranks up the "Ninja" class, and acquires a "Mage" class for farming if not already owned.
    /// </summary>
    void BeginnerItems()
    {
        // Combined skip logic: skip if EITHER high-tier classes exist OR both beginner classes are rank 10+
        bool hasHighTier =
            Core.CheckInventory(soloClasses.Except("Assassin", "Ninja Warrior", "Ninja"), any: true, toInv: false)
            && Core.CheckInventory(farmClasses.Except("Mage", "Mage (rare)"), any: true, toInv: false);

        bool hasBothRank10Beginners =
            Core.CheckInventory(new[] { "Assassin", "Ninja Warrior", "Ninja" }, any: true, toInv: false)
            && (ClassNinja?.Quantity ?? 0) >= RANK_10_CLASS_POINTS
            && Core.CheckInventory(new[] { "Mage (Rare)", "Mage" }, any: true, toInv: false)
            && (ClassMage?.Quantity ?? 0) >= RANK_10_CLASS_POINTS
            && Bot.Player.Level >= 30;

        if (hasHighTier || hasBothRank10Beginners)
        {
            Core.Logger("Acc is lvl 30+, skipping beginner items.");
            return;
        }

        Core.Logger("As the Acc is still \"new\", we're going todo the tutorial badges");

        Tutorial.Badges();

        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            InventoryItem? equippedItem = Bot.Inventory.Items.Find(i =>
                i.Equipped && i.Category == category);

            switch (category)
            {
                case ItemCategory.Helm:
                    if (equippedItem == null)
                    {
                        Core.BuyItem("classhalla", 299, "Battle Oracle Hood");
                        Core.Equip("Battle Oracle Hood");
                    }
                    break;

                case ItemCategory.Cape:
                    if (equippedItem == null)
                    {
                        Core.BuyItem("classhalla", 299, "Battle Oracle Wings");
                        Core.Equip("Battle Oracle Wings");
                    }
                    break;

                case ItemCategory.Staff:
                case ItemCategory.Axe:
                case ItemCategory.Bow:
                case ItemCategory.HandGun:
                case ItemCategory.Gauntlet:
                case ItemCategory.Dagger:
                case ItemCategory.Rifle:
                case ItemCategory.Sword:
                case ItemCategory.Whip:
                case ItemCategory.Wand:
                case ItemCategory.Mace:
                case ItemCategory.Polearm:
                    ItemBase? defaultWep = Bot.Inventory.Items.Find(x =>
                        x.Name.StartsWith("Default"));

                    if (defaultWep != null && Core.CheckInventory(defaultWep.ID))
                    {
                        Core.BuyItem("classhalla", 299, "Battle Oracle Battlestaff");
                        Core.Equip("Battle Oracle Battlestaff");
                        Bot.Shops.SellItem(defaultWep.ID);
                        Bot.Wait.ForTrue(() => !Bot.Inventory.Contains(defaultWep.ID), 20);
                    }
                    break;
            }
        }

        Core.Logger("Starting out acc:\n\tGoals: Ninja class & Mage Class");

        Farm.Experience(10);

        Core.Logger("Getting Started: Beginner Levels/Equipment");

        // Buy Rogue if we don't already own it.
        if (!Core.CheckInventory(new[] { "Rogue (Rare)", "Rogue" }, any: true, toInv: false))
        {
            Core.Logger(
                "Getting rogue.. so we can get ninja (there's a 10k hp \"boss\" to kill during the \"Hit Job\" quest.)");

            Adv.BuyItem("classhalla", 172, "Rogue");
        }

        if (!Core.CheckInventory(new[] { "Assassin", "Ninja Warrior", "Ninja" }, any: true, toInv: false))
        {
            Core.Logger("Getting starter Dodge class (Ninja)");

            Adv.RankUpClass("Rogue");
            Farm.Experience(25);
            SetClass();

            // Ninja requires a few quests.
            Mazumi.MazumiQuests();

            Core.BuyItem("classhalla", 178, "Ninja");
        }

        SetClass();

        if (!Core.CheckInventory(new[] { "Mage (Rare)", "Mage" }, any: true, toInv: false))
        {
            Core.Logger("Getting Starter Farm class (Mage)");
            Adv.BuyItem("classhalla", 174, 15653, shopItemID: 9845);
        }

        SetClass();
    }
    #endregion Extra:


    #region BTS
    private int _lastCheckedLevel = 0;
    private string? _lastSolo;
    private string? _lastFarm;
    private string? _lastDodge;

    public void SetClass()
    {
        Core.ReadCBO();
        _classCache.Clear();

        // Skip if nothing meaningful changed
        if (ShouldSkipSetClass())
            return;

        // Resolve classes
        Core.SoloClass = ResolveClass(Core.SoloClass, soloClasses, out _) ?? Core.SoloClass;
        Core.FarmClass = ResolveClass(Core.FarmClass, farmClasses, out _) ?? Core.FarmClass;
        Core.DodgeClass = ResolveClass(Core.DodgeClass, dodgeClasses, out _) ?? Core.DodgeClass;

        Core.ReadCBO();

        (string? Class, string Label)[] rankTargets =
        [
            (Core.SoloClass, "SoloClass"),
            (Core.FarmClass, "FarmClass"),
            (Core.DodgeClass, "DodgeClass")
        ];

        foreach ((string? className, string label) in rankTargets)
            RankIfNeeded(className, label);


        Bot.Sleep(500);

        Core.EquipClass(ClassType.Solo);

        _lastCheckedLevel = Bot.Player?.Level ?? 0;
        _lastSolo = Core.SoloClass;
        _lastFarm = Core.FarmClass;
        _lastDodge = Core.DodgeClass;
    }

    private bool ShouldSkipSetClass()
    {
        int currentLevel = Bot.Player?.Level ?? 0;

        bool levelIncreased = currentLevel > _lastCheckedLevel;

        bool classChanged =
            !string.Equals(_lastSolo, Core.SoloClass, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(_lastFarm, Core.FarmClass, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(_lastDodge, Core.DodgeClass, StringComparison.OrdinalIgnoreCase);

        return !levelIncreased && !classChanged;
    }

    private static string? ResolveClass(string? current, string[] pool, out string? found)
    {
        found = null;
        bool invalid =
            string.IsNullOrEmpty(current)
            || current.Equals("Generic", StringComparison.OrdinalIgnoreCase)
            || !pool.Any(x => x.Equals(current, StringComparison.OrdinalIgnoreCase))
            || !Core.CheckInventory(current);

        if (!invalid)
            return current;

        found = pool.FirstOrDefault(x => Core.CheckInventory(x));
        return found ?? current;
    }

    private static void BankAllUnusedClasses()
    {
        // More explicit null handling instead of relying on spread operator
        var keepList = new List<string>();
        if (!string.IsNullOrWhiteSpace(Core.SoloClass))
            keepList.Add(Core.SoloClass!);
        if (!string.IsNullOrWhiteSpace(Core.FarmClass))
            keepList.Add(Core.FarmClass!);
        if (!string.IsNullOrWhiteSpace(Core.DodgeClass))
            keepList.Add(Core.DodgeClass!);

        string[] keep = [.. keepList.Distinct(StringComparer.OrdinalIgnoreCase)];

        // Get ALL classes from inventory (actual items)
        string[] toBank = Bot.Inventory.Items
            .Where(x => x != null
                && x.Category == ItemCategory.Class
                && !keep.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
            .Select(x => x.Name)
            .ToArray();

        if (toBank.Length > 0)
        {
            Core.Logger($"Banking unused classes: {string.Join(", ", toBank)}");
            Core.ToBank(toBank);
        }
        else
        {
            Core.Logger("No unused classes to bank");
        }
    }

    private static void RankIfNeeded(string? className, string label)
    {
        if (string.IsNullOrEmpty(className))
            return;

        if (!Core.CheckInventory(className))
        {
            Core.Logger($"WARNING: {label} class '{className}' not in inventory!");
            return;
        }

        if (Core.CheckClassRank(false, className) >= 10)
            return;

        Core.Logger($"{label} resolved -> {className}");
        Adv.RankUpClass(className);
    }
    #endregion BTS

    /// <summary>
    /// Enhances the first item from the given list of items in the player's inventory, if found.
    /// </summary>
    /// <remarks>
    /// This method checks the player's inventory for the specified items and enhances the first
    /// item found using the "Adv.SmartEnhance" method.
    /// </remarks>
    public static void DmgOverTimeEnh()
    {
        string[] itemsToCheck = new[]
        {
            "ShadowStalker of Time",
            "ShadowWeaver of Time",
            "ShadowWalker of Time",
            "Infinity Knight",
            "Interstellar Knight",
            "Void Highlord (IoDA)",
            "Void Highlord",
            "Dragon of Time",
            "Timeless Dark Caster",
            "Frostval Barbarian",
            "Blaze Binder",
            "DeathKnight",
            "DragonSoul Shinobi",
            "Shadow Dragon Shinobi",
            "Legion Revenant",
        };

        foreach (string item in itemsToCheck)
        {
            if (Core.CheckInventory(new[] { item }, any: true))
            {
                Adv.SmartEnhance(item);
                break; // Stops the loop once the item is found and enhanced.
            }
        }
    }


    #region Explanation:
    /*________________________________________________________________________________________Explanation_____________________________________________________________________________________________
       🖕 The SetClass() method checks whether the swapToSoloClass and swapToFarmClass flags are both set to true. If so, it logs an error message and returns.
       🖕 The method initializes newSoloClass and newFarmClass variables with the current values of SoloClass and FarmClass, respectively.
       🖕 If both SoloClass and FarmClass are already set and not "Generic", the method logs a message indicating that the CBO classes are already set and exits.
       🖕 If SoloClass is not already set, the method attempts to find a valid class from the soloClassesToCheck list using the CheckAndSetClass() method.
       🖕 If a valid class is found in the soloClassesToCheck list, it is set as newSoloClass, and the method logs a message indicating the class was found.
       🖕 If SoloClass is already set, the method logs a message indicating the predetermined SoloClass is being used.
       🖕 If FarmClass is not already set, the method attempts to find a valid class from the farmClassesToCheck list using the CheckAndSetClass() method.
       🖕 If a valid class is found in the farmClassesToCheck list, it is set as newFarmClass, and the method logs a message indicating the class was found.
       🖕 If FarmClass is already set, the method logs a message indicating the predetermined FarmClass is being used.
       🖕 If the swapToSoloClass flag is true, the method equips the class represented by newSoloClass using Core.EquipClass().
       🖕 If the swapToFarmClass flag is true, the method equips the class represented by newFarmClass using Core.EquipClass().
       🖕 SoloClass and FarmClass are updated with the values of newSoloClass and newFarmClass, respectively.
       🖕 The method logs messages indicating the updated SoloClass and FarmClass values.

       The CheckAndSetClass() method checks if the provided classToCheck is "Generic" or exists in the classesToCheck array.
       If so, it calls the FindValidClass() method to find a valid class from the classesToCheck list and possibly ranks it up.
       If classToCheck is not "Generic" and not found in classesToCheck, the method logs a message indicating the predetermined class is being used.

       The FindValidClass() method iterates through classesToCheck to find a valid class in the player's inventory.
       If a valid class is found, it logs whether the class is being ranked up or not and returns the class name.
       If no valid class is found, it logs a message that no valid class was found and returns "Generic".

       This approach allows the SetClass() method to independently set SoloClass and FarmClass, choose valid classes based on predefined lists,
       and optionally rank up classes, providing flexibility for class management.
       ℭ𝔬𝔲𝔯𝔱𝔢𝔰𝔶 𝔬𝔣 ℭ𝔥𝔞𝔱𝔊ℙ𝔗
       ________________________________________________________________________________________Explanation_____________________________________________________________________________________________*/

    #endregion Explanation:


    public enum PetChoice
    {
        None,
        HotMama,
        Akriloth,
    };
    #endregion other stuff
}
