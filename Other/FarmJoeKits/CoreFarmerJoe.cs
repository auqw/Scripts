/*
name: null
description: null
tags: null
*/
#region includes
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Dailies/0AllDailies.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Dailies/Cryomancer.cs
//cs_include Scripts/Enhancement/InventoryEnhancer.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/GearOfAwe/CapeOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowrealmMerge.cs
//cs_include Scripts/Dailies/MineCrafting.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Other/Classes/DragonShinobi.cs
//cs_include Scripts/Other/Classes/REP-based/EternalInversionist.cs
//cs_include Scripts/Other/Classes/REP-based/GlacialBerserker.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Other/Classes/ScarletSorceress.cs
//cs_include Scripts/Other/Classes/BloodSorceress.cs
//cs_include Scripts/Other/Classes/FrostSpiritReaver.cs
//cs_include Scripts/Other/FreeBoosts/FreeBoostsQuest(10mns)[Rng].cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/DualChainSawKatanas.cs
//cs_include Scripts/Other/Weapons/EnchantedVictoryBladeWeapons.cs
//cs_include Scripts/Seasonal/Frostvale/NorthlandsMonk.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/Tutorial.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Tools/BankAllItems.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Other/Classes/REP-based/MasterRanger.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Other/MergeShops/SynderesMerge.cs
//cs_include Scripts/Other/Classes/REP-based/DarkbloodStormKing.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Story/AgeofRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/YulgarsUndineMerge.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnFortressMerge.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Other/Classes/Daily-Classes/BlazeBinder.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/ExaltedApotheosisPreReqs.cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Other/Classes/Dragonslayer.cs
#endregion includes


using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class CoreFarmerJoe
{
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

    public string OptionsStorage = "FarmerJoePet";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "OutFit",
            "Get a Pre-Made Outfit, Curtious of the Community",
            "We are farmers, bum ba dum bum bum bum bum",
            false
        ),
        new Option<bool>("EquipOutfit", "Equip outfit at the end?", "Yay or Nay", false),
        new Option<bool>("SellStarterClasses", "SellStarterClasses", "Yay or Nay", false),
        new Option<bool>(
            "GetBoosts",
            "GetBoosts",
            "Do the \"Free Boosts\" quests to get some 10minute boosts (the droprate is < 1% so itll take awhile)",
            false
        ),
        new Option<bool>(
            "EquipBoostingGear",
            "EquipBoostingGear",
            "use a predetermined set of gear based on what you own, and certain paramiters set, otherwise use whats set for your solo/farm class gear in CBO",
            true
        ),
        new Option<PetChoice>(
            "PetChoice",
            "Choose Your Pet",
            "Extra stuff to choose, if you have any suggestions -form in disc, and put it under request. or dm Tato(the retarded one on disc)",
            PetChoice.None
        ),
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

    /// <summary>
    /// Orchestrates the entire progression process from level 1 to endgame, including:
    /// - **Leveling:** Progresses through levels 1 to 100 with specific milestones and enhancements.
    /// - **Item Acquisition:** Handles the acquisition of necessary items and classes at each stage.
    /// - **Outfit Setup:** Configures the character's outfit and equipment.
    /// - **Pet Management:** Acquires and equips specified pets.
    /// </summary>
    public void DoAll()
    {
        Adv.GearStore();
        Level1to30();
        Level30to75();
        Level75to100();
        EndGame();
        Outfit();
        Pets(PetChoice.HotMama);
        Pets(PetChoice.Akriloth);
        Adv.GearStore(true);
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

    #region Leve30 to 75
    /// <summary>
    /// Progresses the character from level 30 to 75 by acquiring essential items and enhancing classes. This includes:
    /// - **Item Acquisition:** Obtaining items like "Awethur's Accoutrements," "Master Ranger," "Burning Blade," "Scarlet Sorceress," "Blaze Binder," "DragonSoul Shinobi," "ArchPaladin," and "ArchFiend DeathLord."
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
            "Joe will handle each level bracket (0-30, 30-50, 50-55, 55-60, 60-65, 65-75, 75-100) doing the leveling first, then aquireing classes and items after so its prepared and hopefully strong enough to handle the next level bracket."
        );
        Core.Logger(
            "We'll occasionaly get rep/class/gold boosts throguh out the script to help speed things up a bit."
        );
        foreach (int Level in Core.FromTo(0, 75))
        {
            if (Bot.Config!.Get<bool>("GetBoosts"))
            { // Always ensure we have 10 of each boost type
                Farm.GetBoost("REP", 10, true);
                Farm.GetBoost("XP", Bot.Player.Level >= 100 ? 0 : 10, true);
                Boosts.GetBoostsSelect(Bot.Player.Gold >= 100000000 ? 0 : 10, 10, 0);
            }

            // Handle special cases and leveling
            switch (Level)
            {
                case 30:
                    if (
                        Bot.Player.Level >= 30
                        && Core.CheckInventory(
                            new[]
                            {
                                "Archfiend",
                                "Blaze Binder",
                                "Scarlet Sorceress",
                                "Master Ranger",
                            },
                            any: true,
                            toInv: false
                        )
                        && Core.CheckInventory(
                            new[] { "ArchPaladin", "Dragonslayer General", "Dragonslayer" },
                            any: true,
                            toInv: false
                        )
                        && Core.CheckInventory(
                            new[]
                            {
                                "Burning Blade of Abezeth",
                                "Burning Blade",
                                "Awethur's Accoutrements",
                            },
                            any: true,
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }

                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger(
                        "Level 30: Getting Master Ranger, Awethur's Accoutrements, & Dragonslayer"
                    );
                    Farm.Experience(Level);
                    HandleLevel30();
                    break;

                case 50:
                    if (
                        Bot.Player.Level >= 50
                        && Core.CheckInventory(
                            new[] { "Archfiend", "Blaze Binder", "Scarlet Sorceress" },
                            any: true,
                            toInv: false
                        )
                        && Core.CheckInventory(
                            new[] { "ArchPaladin", "Dragonslayer General" },
                            any: true,
                            toInv: false
                        )
                        && Core.CheckInventory(
                            new[] { "Burning Blade of Abezeth", "Burning Blade" },
                            any: true,
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }
                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger(
                        "Level 50: Getting Scarlet Sorceress, Dragonslayer General, & Burning Blade"
                    );
                    Farm.Experience(Level);
                    HandleLevel50();
                    break;

                case 55:
                    if (
                        Bot.Player.Level >= 55
                        && Core.CheckInventory(
                            new[]
                            {
                                "Archfiend",
                                !Bot.Quests.IsDailyComplete(2209)
                                && !Core.CheckInventory("Blaze Binder")
                                    ? "Blaze Binder"
                                    : "Scarlet Sorceress",
                            },
                            any: true,
                            toInv: false
                        )
                        && Core.CheckInventory(
                            !Bot.Quests.IsDailyComplete(3966) && !Core.CheckInventory("Cryomancer")
                                ? "Cryomancer"
                                : "Dragonslayer General",
                            toInv: false
                        )
                        && Core.CheckInventory(
                            new[] { "Burning Blade of Abezeth", "Burning Blade" },
                            any: true,
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }
                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger("Level 55: Getting Blaze Binder & Cryomancer");
                    Farm.Experience(Level);
                    HandleLevel55();
                    break;

                case 60:
                    if (
                        Bot.Player.Level >= 60
                        && Core.CheckInventory(
                            new[] { "ArchPaladin", "DragonSoul Shinobi" },
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }
                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger("Level 60: Getting DragonSoul Shinobi for DoomKittem(ArchPaladin)");
                    Farm.Experience(Level);
                    HandleLevel60();
                    break;

                case 65:
                    if (
                        Bot.Player.Level >= 65
                        && Core.CheckInventory(
                            new[] { "ArchPaladin", "Glacial Berserker" },
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }
                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger("Level 65: Getting Glacial Berserker &  ArchPaladin");
                    Farm.Experience(Level);
                    HandleLevel65();
                    break;

                case 75:
                    if (
                        Bot.Player.Level >= 75
                        && Core.CheckInventory(
                            new[] { "ArchPaladin", "ArchFiend", "Archfiend DeathLord" },
                            toInv: false
                        )
                    )
                    {
                        continue;
                    }
                    // Set Solo & FarmClass, and ensure class is ranked up each 5th level
                    SetClass();
                    SetClass();
                    Core.Logger("Level 75: Getting ArchFiend DeathLord (for +30dmgAll), ArchFiend");
                    Farm.Experience();
                    HandleLevel75();
                    break;

                default:
                    if (Bot.Player.Level >= 75)
                    {
                        continue;
                    }
                    Farm.Experience(Level);
                    break;
            }
        }
    }

    private void HandleLevel30()
    {
        // Master Ranger or farm alternative
        if (
            !Core.CheckInventory(
                new[] { "Archfiend", "Blaze Binder", "Scarlet Sorceress", "Master Ranger" },
                any: true,
                toInv: false
            )
        )
        {
            if (Core.CheckInventory("Venom Head"))
                Core.SellItem("Venom Head");

            Core.Logger("Level 30: Acquiring Master Ranger");
            SetClass();
            MR.GetMR();
        }

        // Dragonslayer or higher tier
        if (
            !Core.CheckInventory(
                new[] { "ArchPaladin", "Dragonslayer General", "Dragonslayer" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 30: Acquiring Dragonslayer");
            SetClass();
            DSlayer.GetDragonslayer();
        }

        // Blade of Awe progression
        if (
            !Core.CheckInventory(
                new[] { "Burning Blade of Abezeth", "Burning Blade", "Awethur's Accoutrements" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 30: Farming Blade of Awe Rep for enhancements & sword");
            SetClass();
            Farm.BladeofAweREP();
            Adv.BuyItem("museum", 631, "Awethur's Accoutrements");
        }
    }

    private void HandleLevel50()
    {
        // Scarlet Sorceress or farm alternative
        if (
            !Core.CheckInventory(
                new[] { "Archfiend", "Blaze Binder", "Scarlet Sorceress" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 50: Acquiring Scarlet Sorceress");
            SetClass();
            SS.GetSSorc();
        }

        // Dragonslayer General or ArchPaladin
        if (
            !Core.CheckInventory(
                new[] { "ArchPaladin", "Dragonslayer General" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 50: Acquiring Dragonslayer General");
            SetClass();
            DSG.GetDSGeneral();
        }

        // Burning Blade progression
        if (
            !Core.CheckInventory(
                new[] { "Burning Blade of Abezeth", "Burning Blade" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 50: Acquiring Burning Blade");
            SetClass();
            BB.GetBurningBlade();
        }
    }

    private void HandleLevel55()
    {
        // Blaze Binder
        if (!Core.CheckInventory("Blaze Binder", toInv: false))
        {
            Core.Logger("Level 55: Acquiring Blaze Binder");
            SetClass();
            Bb.GetClass();
        }

        // Cryomancer
        if (!Core.CheckInventory("Cryomancer", toInv: false))
        {
            Core.Logger("Level 55: Acquiring Cryomancer");
            SetClass();
            Cryo.DoCryomancer();
        }
    }

    private void HandleLevel60()
    {
        // DragonSoul Shinobi or ArchPaladin
        if (
            !Core.CheckInventory(
                new[] { "ArchPaladin", "DragonSoul Shinobi" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 60: Acquiring DragonSoul Shinobi");
            SetClass();
            DS.GetDSS();
        }
    }

    private void HandleLevel65()
    {
        // Glacial Berserker or ArchPaladin
        if (
            !Core.CheckInventory(
                new[] { "ArchPaladin", "Glacial Berserker" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Level 65: Acquiring Glacial Berserker");
            SetClass();
            GB.GetGB();
        }

        // ArchPaladin
        if (!Core.CheckInventory("ArchPaladin", toInv: false))
        {
            Core.Logger("Level 65: Acquiring ArchPaladin");
            SetClass();
            AP.GetAP();
        }
    }

    private void HandleLevel75()
    {
        // Archfiend DeathLord with +30 dmgAll boost
        if (
            !Adv.HasMinimalBoost(GenericGearBoost.dmgAll, 30)
            || !Core.CheckInventory("Archfiend DeathLord", toInv: false)
        )
        {
            Core.Logger("Level 75: Acquiring Archfiend DeathLord for +30 dmgAll");
            SetClass();
            AFDeath.GetArm(true, ArchfiendDeathLord.RewardChoice.Archfiend_DeathLord);
        }

        // Archfiend
        if (!Core.CheckInventory("Archfiend", toInv: false))
        {
            Core.Logger("Level 75: Acquiring Archfiend");
            SetClass();
            AF.GetArchfiend();
        }
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
        // Phase 1: Prepare healer class for 13 LoC (or rank Dragon of Time)
        Core.Logger("Phase 1: Class Preparation - Healer/Dragon of Time for 13 LoC");
        if (Core.CheckInventory("Dragon of Time", toInv: false))
        {
            if (ClassDragonOfTime?.Quantity < 302500)
            {
                Core.Logger("Dragon of Time found but not rank 10 - ranking up");
                Adv.RankUpClass("Dragon of Time");
            }
        }
        else if (!Core.CheckInventory(new[] { "Healer", "Healer (Rare)" }, any: true))
        {
            Core.Logger("No healing class found - acquiring Healer");
            Adv.BuyItem("classhalla", 176, "Healer");
            Adv.RankUpClass(Core.CheckInventory("Healer (Rare)") ? "Healer (Rare)" : "Healer");
        }

        // Phase 2: Chaos quests and enhancements
        Core.Logger("Phase 2: Chaos Shenanigans - CoA & 13 LoC");
        COA.GetCoA();
        Core.Equip("Cape of Awe");
        SetClass();
        LOC.Complete13LOC();

        // Phase 3: Solo classes and weapons
        Core.Logger("Phase 3: Solo Classes & Weapon - LoO Daily");
        LOO.GetLoO();
        Core.ToBank(Core.EnsureLoad(7156).Rewards.Select(i => i.Name).ToArray());

        // Acquire additional farming classes
        Core.Logger("Phase 3: Acquiring Additional Classes (FSR, Northlands Monk, Shaman)");
        SC.GetSC();

        if (!Core.CheckInventory("Frost Spirit Reaver", toInv: false))
        {
            Core.Logger("Acquiring Frost Spirit Reaver");
            SetClass();
            FSR.GetFSR();
        }

        if (!Core.CheckInventory("Northlands Monk", toInv: false))
        {
            Core.Logger("Acquiring Northlands Monk");
            SetClass();
            NM.GetNlMonk();
        }

        if (!Core.CheckInventory("Shaman", toInv: false))
        {
            Core.Logger("Acquiring Shaman");
            SetClass();
            Shaman.GetShaman();
        }

        // Unlock enhancements
        Core.Logger("Phase 3: Unlocking Helmet Enhancements (Forge, Vim, Examen, Anima, Pneuma)");
        UnlockForgeEnhancements.ForgeHelmEnhancement();
        UnlockForgeEnhancements.Vim();
        UnlockForgeEnhancements.Examen();
        UnlockForgeEnhancements.Anima();
        UnlockForgeEnhancements.Pneuma();

        Core.Logger("Phase 3: Unlocking Weapon Enhancements (Forge, Lacerate, Smite, Praxis)");
        UnlockForgeEnhancements.ForgeWeaponEnhancement();
        UnlockForgeEnhancements.Lacerate();
        UnlockForgeEnhancements.Smite();
        UnlockForgeEnhancements.Praxis();

        // Phase 4: Level and acquire endgame items
        Core.Logger("Phase 4: Leveling to 80");
        Farm.Experience(80);

        Core.Logger("Phase 4: Celestial Arena QuestLine for Blinding Bright of Awe (BBoA)");
        SetClass();
        CAQ.DoAll();
        BBOA.GetBBoA();

        Core.Logger(
            "Phase 4: Unlocking Cape Enhancements (Forge, Absolution, Vainglory, Avarice, Penitence, Lament)"
        );
        UnlockForgeEnhancements.ForgeCapeEnhancement();
        UnlockForgeEnhancements.Absolution();
        UnlockForgeEnhancements.Vainglory();
        UnlockForgeEnhancements.Avarice();
        UnlockForgeEnhancements.Lament();

        Core.Logger("Phase 4: Acquiring Yin & Yang Roentgenium (YnR)");
        YNR.GetYnR();

        Core.Logger("Phase 4: Acquiring Dragon of Time");
        SetClass();
        DoT.GetDoT();

        Core.Logger("Phase 4: Final Leveling to 100");
        SetClass();
        Farm.Experience();

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
        UnlockForgeEnhancements.Elysium();
        UnlockForgeEnhancements.ArcanasConcerto();
        UnlockForgeEnhancements.Ravenous();
        UnlockForgeEnhancements.DauntLess();

        // Apotheosis:
        Core.Logger("P5: Apotheosis prereqs");
        ExaltedApotheosisPreReqs.PreReqs();

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
            Core.HuntMonster(
                "gravestrike",
                "Ultra Akriloth",
                "Akriloth Pet",
                isTemp: false,
                log: false
            );
            Bot.Wait.ForPickup("Akriloth Pet");
            Core.Equip("Akriloth Pet");
        }
    }

    /// <summary>
    /// Acquires and equips a shirt and hat for the character.
    /// - **NO BOTS Armor:** Purchases and merges the "NO BOTS Armor."
    /// - **Scarecrow Hat:** Buys the "Scarecrow Hat" from Yulgar's shop.
    /// </summary>
    public void ShirtAndHat()
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
        Core.HuntMonster(
            "undergroundlabb",
            "Rabid Server Hamster",
            "The Server is Down",
            isTemp: false,
            log: false
        );
        Bot.Wait.ForPickup("The Server is Down");
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
        Core.Logger(
            "Doing `Tutorial Badges` Required for fresh accounts to leave oaklore (this may take a moment to start.. we dont know why.)\n"
                + "(by bot i mean.. obviously u can do this manualy)"
        );

        Tutorial.Badges();

        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            InventoryItem? equippedItem = Bot.Inventory.Items.Find(i =>
                i.Equipped && i.Category == category
            );
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
                    ItemBase? DefaultWep = Bot.Inventory.Items.Find(x =>
                        x.Name.StartsWith("Default")
                    );
                    if (DefaultWep != null && Core.CheckInventory(DefaultWep.ID))
                    {
                        Core.BuyItem("classhalla", 299, "Battle Oracle Battlestaff");
                        Core.Equip("Battle Oracle Battlestaff");
                        Bot.Shops.SellItem(DefaultWep.ID);
                        Bot.Wait.ForTrue(() => !Bot.Inventory.Contains(DefaultWep.ID), 20);
                    }
                    break;

                default:
                    break;
            }
        }

        if (
            Core.CheckInventory(
                new[] { "Assassin", "Ninja Warrior", "Ninja" },
                any: true,
                toInv: false
            )
            && ClassNinja?.Quantity >= 302500
            && Core.CheckInventory(new[] { "Mage (Rare)", "Mage" }, any: true, toInv: false)
            && ClassMage?.Quantity >= 302500
            && Bot.Player.Level >= 30
        )
        {
            Core.Logger("Acc is lvl 30+, skipping beginner items.");
            return;
        }

        Core.Logger("Starting out acc:\n" + "\tGoals: Ninja class & Mage Class");

        Farm.Experience(10);
        Core.Logger("Getting Started: Beginner Levels/Equipment");
        if (
            !Core.CheckInventory(new[] { "Rogue (Rare)", "Rogue" }, any: true, toInv: false)
            && ClassRogue?.Quantity < 302500
        )
        {
            Core.Logger(
                "Getting rogue.. so we can get ninja (thers a 10k hp \"boss\" to kill during the \"Hit Job\" quest.)"
            );
            Adv.BuyItem("classhalla", 172, "Rogue");
        }

        if (
            !Core.CheckInventory(
                new[] { "Assassin", "Ninja Warrior", "Ninja" },
                any: true,
                toInv: false
            )
        )
        {
            Core.Logger("Getting starter Dodge class (Ninja)");
            SetClass();
            Adv.RankUpClass("Rogue");
            //ninja requires a few quets.. its ok tho
            Mazumi.MazumiQuests();
            Core.BuyItem("classhalla", 178, "Ninja");
        }

        SetClass();
        Adv.RankUpClass("Ninja");

        if (!Core.CheckInventory(new[] { "Mage (Rare)", "Mage" }, any: true, toInv: false))
        {
            Core.Logger("Getting Starter Farm class (Mage)");
            Adv.BuyItem("classhalla", 174, 15653, shopItemID: 9845);
        }
        SetClass();
    }
    #endregion Extra:

    #region BTS
    public void SetClass()
    {
        // Class lists for solo and farm activities
        string[] soloClasses = new[]
        {
            "Void Highlord",
            "Legion Revenant",
            "Dragon of Time",
            "ArchPaladin",
            "Dragonslayer General",
            "Glacial Berserker",
            "Dragonslayer",
            "DragonSoul Shinobi",
            "Assassin",
            "Ninja Warrior",
            "Ninja",
            "Rogue (Rare)",
            "Rogue",
            "Healer (Rare)",
            "Healer",
        };

        string[] farmClasses = new[]
        {
            "Legion Revenant",
            "Blaze Binder",
            "Archfiend",
            "Scarlet Sorceress",
            "Master Ranger",
            "Mage (Rare)",
            "Mage",
        };

        // Combine inventory and bank items once for reuse
        List<InventoryItem> available = Bot.Inventory.Items.Concat(Bot.Bank.Items).ToList();
        bool equipBoosting = Bot.Config!.Get<bool>("EquipBoostingGear");

        // Determine which classes to use based on availability
        string newSolo =
            soloClasses.FirstOrDefault(c =>
                available.Any(i => i.Name == c && i.Category == ItemCategory.Class)
            ) ?? Core.SoloClass;
        string newFarm =
            farmClasses.FirstOrDefault(c =>
                available.Any(i => i.Name == c && i.Category == ItemCategory.Class)
            ) ?? Core.FarmClass;

        // Update class settings
        Core.SoloClass = newSolo;
        Core.FarmClass = newFarm;

        // If CBO classes are not set to Generic, we're done
        if (Core.SoloClass != "Generic" && Core.FarmClass != "Generic")
            return;

        // Find and rank up solo class if needed
        var soloItem = available.FirstOrDefault(i =>
            i.Name == newSolo && i.Category == ItemCategory.Class
        );
        if (soloItem != null && soloItem.Quantity < 302500)
        {
            Core.Unbank(soloItem.ID);
            Adv.RankUpClass(newSolo);
        }

        // Find and rank up farm class if needed
        var farmItem = available.FirstOrDefault(i =>
            i.Name == newFarm && i.Category == ItemCategory.Class
        );
        if (farmItem != null && farmItem.Quantity < 302500)
        {
            Core.Unbank(farmItem.ID);
            Adv.RankUpClass(newFarm);
        }

        // Equip boosting gear if enabled
        if (equipBoosting)
        {
            var dmgPriority = new[]
            {
                "dmgAll",
                "gold",
                "cp",
                "rep",
                "Undead",
                "Chaos",
                "Elemental",
                "Dragonkin",
                "Human",
            };
            var armorPriority = Core.CheckInventory("Polly Roger")
                ? new[] { "gold", "cp", "rep" }
                : dmgPriority;

            var metaPriorities = new Dictionary<string, string[]>
            {
                { "Cape", dmgPriority },
                { "Helm", dmgPriority },
                { "Armor", armorPriority },
                { "Weapon", new[] { "dmgAll", "gold", "cp", "rep" } },
                { "Pet", dmgPriority },
            };

            Core.EquipBestItemsForMeta(metaPriorities);
        }

        Core.Logger($"Setting SoloClass to: {newSolo}.\nSetting FarmClass to: {newFarm}.");
    }

    /// <summary>
    /// Enhances the first item from the given list of items in the player's inventory, if found.
    /// </summary>
    /// <remarks>
    /// This method checks the player's inventory for the specified items and enhances the first
    /// item found using the "Adv.SmartEnhance" method.
    /// </remarks>
    public void DmgOverTimeEnh()
    {
        string[] itemsToCheck = new[]
        {
            "ShadowStalker of Time",
            "ShadowWeaver of Time",
            "ShadowWalker of Time",
            "Infinity Knight",
            "Interstellar Knight",
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
    #endregion BTS


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
