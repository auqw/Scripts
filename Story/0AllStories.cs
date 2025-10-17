/*
name: Complete All Stories
description: This will complete all stories.
tags: story, quest, complete, all
*/

#region includes
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_Include Scripts/Farm/BuyScrolls.cs

//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs

//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs

//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs

//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs

//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

//cs_include Scripts/Story/FireIsland/CoreFireIsland.cs
//cs_include Scripts/Seasonal/Friday13th/Story/CoreFriday13th.cs

//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs

//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs

//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/Legion/DageTheEvilIsland/CoreDageTheEvilIsland.cs
//cs_include Scripts/Story/Legion/AtlasFalls.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/DageChallengeStory.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/Ravenscar.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/WorldSoul.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs

//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/DeadlyDungeon[Mem].cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/KillerCatacombs[Mem].cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/PyramidofPain[Mem].cs

//cs_include Scripts/Story/Lynaria/CoreLynaria.cs

//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/FiendPast.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/ShadowBlastArena.cs
//cs_include Scripts/Story/Nation/Tercessuinotlim.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs

//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/GoldenArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalDianoia.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalParadise.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/LivingDungeon.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/OrbHunt.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/QueenBattle.cs

//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs

//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs

//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs

//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs

//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs

//cs_include Scripts/Story/Adam1a1Quests.cs
//cs_include Scripts/Story/AranxQuests.cs
//cs_include Scripts/Story/Arcangrove.cs
//cs_include Scripts/Story/AriaGreenhouse.cs
//cs_include Scripts/Story/AriaPet[MEM].cs
//cs_include Scripts/Story/Artixpointe.cs
//cs_include Scripts/Story/ArtixWedding.cs
//cs_include Scripts/Story/Asylum.cs

//cs_include Scripts/Story/Banished.cs
//cs_include Scripts/Story/BaseCamp.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/BeleensDream.cs
//cs_include Scripts/Story/BloodMoon.cs
//cs_include Scripts/Story/Bludrut.cs
//cs_include Scripts/Story/BoneBreak.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/BrightCrystalStory.cs

//cs_include Scripts/Story/CastleOfGlass.cs
//cs_include Scripts/Story/CastleTunnels.cs
//cs_include Scripts/Story/Concert[MEM].cs
//cs_include Scripts/Story/Cornelis[mem].cs
//cs_include Scripts/Story/CrashSite.cs
//cs_include Scripts/Story/Cleric.cs
//cs_include Scripts/Story/CruxShip.cs

//cs_include Scripts/Story/DarkCarnax.cs
//cs_include Scripts/Story/DarkDungeon.cs
//cs_include Scripts/Story/DeathsRealm.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/DjinnGuard.cs
//cs_include Scripts/Story/DoomVault.cs
//cs_include Scripts/Story/DoomVaultB.cs
//cs_include Scripts/Story/Downward.cs
//cs_include Scripts/Story/DracoCon.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/DragonRoad[Upholder].cs
//cs_include Scripts/Story/DreamPalace.cs
//cs_include Scripts/Story/Dwarfhold.cs
//cs_include Scripts/Story/DwarvesVsGiants.cs

//cs_include Scripts/Story/Eden.cs
//cs_include Scripts/Story/EtherstormWastes.cs
//cs_include Scripts/Story/ExaltiaTower.cs
//cs_include Scripts/Story/Extinction.cs

//cs_include Scripts/Story/FableForest.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Story/FrozenNorthlands.cs

//cs_include Scripts/Story/HuntersMoon.cs

//cs_include Scripts/Story/GameHaven.cs
//cs_include Scripts/Story/GiantTaleStory.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/Guru.cs

//cs_include Scripts/Story/IcePlane.cs

//cs_include Scripts/Story/J6Saga.cs

//cs_include Scripts/Story/LaeWedding.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Lightguard[MEM].cs
//cs_include Scripts/Story/LightoviaCave.cs
//cs_include Scripts/Story/LostVilla.cs

//cs_include Scripts/Story/Manor.cs
//cs_include Scripts/Story/Marsh2[MEM].cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Story/Mobius.cs
//cs_include Scripts/Story/MustyCave.cs

//cs_include Scripts/Story/NecroProject.cs
//cs_include Scripts/Story/Noobshire.cs
//cs_include Scripts/Story/Nukemichi[mem].cs
//cs_include Scripts/Story/NytheraSaga.cs

//cs_include Scripts/Story/Pirates[Member].cs
//cs_include Scripts/Story/PoisonForest.cs

//cs_include Scripts/Story/QueenReign.cs
//cs_include Scripts/Story/QuibbleHunt.cs

//cs_include Scripts/Story/RavenlossSaga.cs
//cs_include Scripts/Story/River.cs

//cs_include Scripts/Story/Safiria[Member].cs
//cs_include Scripts/Story/ShadowGates.cs
//cs_include Scripts/Story/ShadowSlayerK.cs
//cs_include Scripts/Story/ShadowVault.cs
//cs_include Scripts/Story/ShadowVoid.cs
//cs_include Scripts/Story/Shattersword.cs
//cs_include Scripts/Story/Shinkansen.cs
//cs_include Scripts/Story/ShipWreck.cs
//cs_include Scripts/Story/SkyGuardSaga.cs
//cs_include Scripts/Story/SpirePast.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/SuperDeath.cs


//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/TitanAttack.cs
//cs_include Scripts/Story/Tournament.cs
//cs_include Scripts/Story/Tower[mem].cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/Tutorial.cs


//cs_include Scripts/Story/Ubear.cs
//cs_include Scripts/Story/UnderGroundLab.cs

//cs_include Scripts/Story/VasalkarLairWar.cs

//cs_include Scripts/Story/WatchTower.cs
//cs_include Scripts/Story/WhiteTigerPoint.cs
//cs_include Scripts/Story/WillowCreek.cs

//cs_include Scripts/Story/XansLair.cs

//cs_include Scripts/Story/Yokai.cs

//cs_include Scripts/Story/MemetsRealm/CoreMemet.cs
#endregion
using Skua.Core.Interfaces;

public class AllStories
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }    private static CoreAdvanced _Adv;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;

    #region Folder based
    // 7 Deadly Dragons
    private static Core7DD DD { get => _DD ??= new Core7DD(); set => _DD = value; }    private static Core7DD _DD;
    private static HatchTheEgg Egg { get => _Egg ??= new HatchTheEgg(); set => _Egg = value; }    private static HatchTheEgg _Egg;
    private static GetSDD SDD { get => _SDD ??= new GetSDD(); set => _SDD = value; }    private static GetSDD _SDD;

    // Age Of Ruin
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }    private static CoreAOR _AOR;

    // Doomwood (all parts)
    private static CoreDoomwood DW { get => _DW ??= new CoreDoomwood(); set => _DW = value; }    private static CoreDoomwood _DW;

    // Dragons Of Yokai
    private static CoreDOY DOY { get => _DOY ??= new CoreDOY(); set => _DOY = value; }    private static CoreDOY _DOY;

    // Elergy of Madness - Darkon
    private static CoreAstravia CoreAstravia { get => _CoreAstravia ??= new CoreAstravia(); set => _CoreAstravia = value; }    private static CoreAstravia _CoreAstravia;

    //Fire Island
    private static CoreFireIsland FI { get => _FI ??= new CoreFireIsland(); set => _FI = value; }    private static CoreFireIsland _FI;

    //Hollowborn
    private static CoreHollowbornStory HB { get => _HB ??= new CoreHollowbornStory(); set => _HB = value; }    private static CoreHollowbornStory _HB;

    //Friday 13th
    private static CoreFriday13th CoreFriday13th { get => _CoreFriday13th ??= new CoreFriday13th(); set => _CoreFriday13th = value; }    private static CoreFriday13th _CoreFriday13th;

    // Isle Of Fotia
    private static CoreIsleOfFotia CoreIsleOfFotia { get => _CoreIsleOfFotia ??= new CoreIsleOfFotia(); set => _CoreIsleOfFotia = value; }    private static CoreIsleOfFotia _CoreIsleOfFotia;

    // Legion
    private static CoreDageTheEvilIsland DageIsland { get => _DageIsland ??= new CoreDageTheEvilIsland(); set => _DageIsland = value; }    private static CoreDageTheEvilIsland _DageIsland;
    private static AtlasFalls AtlasFalls { get => _AtlasFalls ??= new AtlasFalls(); set => _AtlasFalls = value; }    private static AtlasFalls _AtlasFalls;
    private static AtlasKingdom AtlasKingdom { get => _AtlasKingdom ??= new AtlasKingdom(); set => _AtlasKingdom = value; }    private static AtlasKingdom _AtlasKingdom;
    private static AtlasPromenade AtlasPromenade { get => _AtlasPromenade ??= new AtlasPromenade(); set => _AtlasPromenade = value; }    private static AtlasPromenade _AtlasPromenade;
    private static DageChallengeStory DageChallengeStory { get => _DageChallengeStory ??= new DageChallengeStory(); set => _DageChallengeStory = value; }    private static DageChallengeStory _DageChallengeStory;
    private static DarkWarLegionandNation DarkWar { get => _DarkWar ??= new DarkWarLegionandNation(); set => _DarkWar = value; }    private static DarkWarLegionandNation _DarkWar;
    private static Ravenscar Ravenscar { get => _Ravenscar ??= new Ravenscar(); set => _Ravenscar = value; }    private static Ravenscar _Ravenscar;
    private static SeraphicWar_Story SeraphicWar_Story { get => _SeraphicWar_Story ??= new SeraphicWar_Story(); set => _SeraphicWar_Story = value; }    private static SeraphicWar_Story _SeraphicWar_Story;
    private static SevenCircles SevenCircles { get => _SevenCircles ??= new SevenCircles(); set => _SevenCircles = value; }    private static SevenCircles _SevenCircles;
    private static WorldSoul WorldSoul { get => _WorldSoul ??= new WorldSoul(); set => _WorldSoul = value; }    private static WorldSoul _WorldSoul;

    // 13 Lord of Chaos
    private static Core13LoC LOC { get => _LOC ??= new Core13LoC(); set => _LOC = value; }    private static Core13LoC _LOC;
    private static DeadlyDungeon DeadlyDungeon { get => _DeadlyDungeon ??= new DeadlyDungeon(); set => _DeadlyDungeon = value; }    private static DeadlyDungeon _DeadlyDungeon;
    private static KillerCatacombs KillerCatacombs { get => _KillerCatacombs ??= new KillerCatacombs(); set => _KillerCatacombs = value; }    private static KillerCatacombs _KillerCatacombs;
    private static PyramidofPain PyramidofPain { get => _PyramidofPain ??= new PyramidofPain(); set => _PyramidofPain = value; }    private static PyramidofPain _PyramidofPain;

    // Lynaria
    private static CoreLynaria Lynaria { get => _Lynaria ??= new CoreLynaria(); set => _Lynaria = value; }    private static CoreLynaria _Lynaria;

    // Nation
    private static Bamboozle Bamboozle { get => _Bamboozle ??= new Bamboozle(); set => _Bamboozle = value; }    private static Bamboozle _Bamboozle;
    private static CitadelRuins CitadelRuins { get => _CitadelRuins ??= new CitadelRuins(); set => _CitadelRuins = value; }    private static CitadelRuins _CitadelRuins;
    private static DeleuzeTundraStory DeleuzeTundra { get => _DeleuzeTundra ??= new DeleuzeTundraStory(); set => _DeleuzeTundra = value; }    private static DeleuzeTundraStory _DeleuzeTundra;
    private static FiendPast FiendPast { get => _FiendPast ??= new FiendPast(); set => _FiendPast = value; }    private static FiendPast _FiendPast;
    private static Fiendshard_Story Fiendshard_Story { get => _Fiendshard_Story ??= new Fiendshard_Story(); set => _Fiendshard_Story = value; }    private static Fiendshard_Story _Fiendshard_Story;
    private static OblivionTundra OblivionTundra { get => _OblivionTundra ??= new OblivionTundra(); set => _OblivionTundra = value; }    private static OblivionTundra _OblivionTundra;
    private static Originul_Story Originul_Story { get => _Originul_Story ??= new Originul_Story(); set => _Originul_Story = value; }    private static Originul_Story _Originul_Story;
    private static ShadowBlastArena ShadowBlastArena { get => _ShadowBlastArena ??= new ShadowBlastArena(); set => _ShadowBlastArena = value; }    private static ShadowBlastArena _ShadowBlastArena;
    private static Tercessuinotlim Tercessuinotlim { get => _Tercessuinotlim ??= new Tercessuinotlim(); set => _Tercessuinotlim = value; }    private static Tercessuinotlim _Tercessuinotlim;
    private static VoidRefuge VoidRefuge { get => _VoidRefuge ??= new VoidRefuge(); set => _VoidRefuge = value; }    private static VoidRefuge _VoidRefuge;
    private static VoidChasm VoidChasm { get => _VoidChasm ??= new VoidChasm(); set => _VoidChasm = value; }    private static VoidChasm _VoidChasm;

    // Queen of Monsters
    private static CoreQOM QOM { get => _QOM ??= new CoreQOM(); set => _QOM = value; }    private static CoreQOM _QOM;
    private static BrightOak BrightOak { get => _BrightOak ??= new BrightOak(); set => _BrightOak = value; }    private static BrightOak _BrightOak;
    private static CelestialArenaQuests CelestialArena { get => _CelestialArena ??= new CelestialArenaQuests(); set => _CelestialArena = value; }    private static CelestialArenaQuests _CelestialArena;
    private static CelestialPast CelestialPast { get => _CelestialPast ??= new CelestialPast(); set => _CelestialPast = value; }    private static CelestialPast _CelestialPast;
    private static GoldenArena GoldenArena { get => _GoldenArena ??= new GoldenArena(); set => _GoldenArena = value; }    private static GoldenArena _GoldenArena;
    private static InfernalDianoia InfernalDianoia { get => _InfernalDianoia ??= new InfernalDianoia(); set => _InfernalDianoia = value; }    private static InfernalDianoia _InfernalDianoia;
    private static InfernalParadise InfernalParadise { get => _InfernalParadise ??= new InfernalParadise(); set => _InfernalParadise = value; }    private static InfernalParadise _InfernalParadise;
    private static LivingDungeon LivingDungeon { get => _LivingDungeon ??= new LivingDungeon(); set => _LivingDungeon = value; }    private static LivingDungeon _LivingDungeon;
    private static OrbHunt OrbHunt { get => _OrbHunt ??= new OrbHunt(); set => _OrbHunt = value; }    private static OrbHunt _OrbHunt;
    private static QueenBattle QueenBattle { get => _QueenBattle ??= new QueenBattle(); set => _QueenBattle = value; }    private static QueenBattle _QueenBattle;

    // Sepulchure Saga
    private static CoreSepulchure CoreSS { get => _CoreSS ??= new CoreSepulchure(); set => _CoreSS = value; }    private static CoreSepulchure _CoreSS;

    // Shadows of Chaos
    private static CoreSoC CoreSoC { get => _CoreSoC ??= new CoreSoC(); set => _CoreSoC = value; }    private static CoreSoC _CoreSoC;

    // Shadow of War
    private static CoreSoW SOW { get => _SOW ??= new CoreSoW(); set => _SOW = value; }    private static CoreSoW _SOW;

    //Summer 2015 AdventureMap
    private static CoreSummer CoreSummer { get => _CoreSummer ??= new CoreSummer(); set => _CoreSummer = value; }    private static CoreSummer _CoreSummer;

    // Throne of Darkness
    private static CoreToD TOD { get => _TOD ??= new CoreToD(); set => _TOD = value; }    private static CoreToD _TOD;

    //MemetsRealm
    private static MemetsRealm MemetsRealm { get => _MemetsRealm ??= new MemetsRealm(); set => _MemetsRealm = value; }    private static MemetsRealm _MemetsRealm;

    #endregion

    #region Standalone (sorted alphabetically)
    private static Adam1a1Quest Adam1A1Quest { get => _Adam1A1Quest ??= new Adam1a1Quest(); set => _Adam1A1Quest = value; }    private static Adam1a1Quest _Adam1A1Quest;
    private static AranxQuests AranxQuests { get => _AranxQuests ??= new AranxQuests(); set => _AranxQuests = value; }    private static AranxQuests _AranxQuests;
    private static Arcangrove Arcangrove { get => _Arcangrove ??= new Arcangrove(); set => _Arcangrove = value; }    private static Arcangrove _Arcangrove;
    private static AriaGreenhouse AriaGreenhouse { get => _AriaGreenhouse ??= new AriaGreenhouse(); set => _AriaGreenhouse = value; }    private static AriaGreenhouse _AriaGreenhouse;
    private static AriaPet AriaPet { get => _AriaPet ??= new AriaPet(); set => _AriaPet = value; }    private static AriaPet _AriaPet;
    private static Artixpointe Artixpointe { get => _Artixpointe ??= new Artixpointe(); set => _Artixpointe = value; }    private static Artixpointe _Artixpointe;
    private static ArtixWedding ArtixWedding { get => _ArtixWedding ??= new ArtixWedding(); set => _ArtixWedding = value; }    private static ArtixWedding _ArtixWedding;
    private static Asylum Asylum { get => _Asylum ??= new Asylum(); set => _Asylum = value; }    private static Asylum _Asylum;

    private static Banished Banished { get => _Banished ??= new Banished(); set => _Banished = value; }    private static Banished _Banished;
    private static BaseCamp BaseCamp { get => _BaseCamp ??= new BaseCamp(); set => _BaseCamp = value; }    private static BaseCamp _BaseCamp;
    private static BattleUnder BattleUnder { get => _BattleUnder ??= new BattleUnder(); set => _BattleUnder = value; }    private static BattleUnder _BattleUnder;
    private static BeleensDream BeleensDream { get => _BeleensDream ??= new BeleensDream(); set => _BeleensDream = value; }    private static BeleensDream _BeleensDream;
    private static BloodMoon BloodMoon { get => _BloodMoon ??= new BloodMoon(); set => _BloodMoon = value; }    private static BloodMoon _BloodMoon;
    private static Bludrut Bludrut { get => _Bludrut ??= new Bludrut(); set => _Bludrut = value; }    private static Bludrut _Bludrut;
    private static BoneBreak BoneBreak { get => _BoneBreak ??= new BoneBreak(); set => _BoneBreak = value; }    private static BoneBreak _BoneBreak;
    private static Borgars Borgars { get => _Borgars ??= new Borgars(); set => _Borgars = value; }    private static Borgars _Borgars;
    private static BrightCrystalStory BrightCrystal { get => _BrightCrystal ??= new BrightCrystalStory(); set => _BrightCrystal = value; }    private static BrightCrystalStory _BrightCrystal;

    private static CastleOfGlass CastleOfGlass { get => _CastleOfGlass ??= new CastleOfGlass(); set => _CastleOfGlass = value; }    private static CastleOfGlass _CastleOfGlass;
    private static CastleTunnels CastleTunnels { get => _CastleTunnels ??= new CastleTunnels(); set => _CastleTunnels = value; }    private static CastleTunnels _CastleTunnels;
    private static Cleric Cleric { get => _Cleric ??= new Cleric(); set => _Cleric = value; }    private static Cleric _Cleric;
    private static Concert Concert { get => _Concert ??= new Concert(); set => _Concert = value; }    private static Concert _Concert;
    private static Cornelis Cornelis { get => _Cornelis ??= new Cornelis(); set => _Cornelis = value; }    private static Cornelis _Cornelis;
    private static CrashSite CrashSite { get => _CrashSite ??= new CrashSite(); set => _CrashSite = value; }    private static CrashSite _CrashSite;
    private static CruxShip CruxShip { get => _CruxShip ??= new CruxShip(); set => _CruxShip = value; }    private static CruxShip _CruxShip;

    private static DarkCarnaxStory DarkCarnax { get => _DarkCarnax ??= new DarkCarnaxStory(); set => _DarkCarnax = value; }    private static DarkCarnaxStory _DarkCarnax;
    private static DarkDungeon DarkDungeon { get => _DarkDungeon ??= new DarkDungeon(); set => _DarkDungeon = value; }    private static DarkDungeon _DarkDungeon;
    private static DeathsRealm DeathsRealm { get => _DeathsRealm ??= new DeathsRealm(); set => _DeathsRealm = value; }    private static DeathsRealm _DeathsRealm;
    private static DjinnGateStory DjinnGateStory { get => _DjinnGateStory ??= new DjinnGateStory(); set => _DjinnGateStory = value; }    private static DjinnGateStory _DjinnGateStory;
    private static DjinnGuard DjinnGuard { get => _DjinnGuard ??= new DjinnGuard(); set => _DjinnGuard = value; }    private static DjinnGuard _DjinnGuard;
    private static DoomVaultA DoomVaultA { get => _DoomVaultA ??= new DoomVaultA(); set => _DoomVaultA = value; }    private static DoomVaultA _DoomVaultA;
    private static DoomVaultB DoomVaultB { get => _DoomVaultB ??= new DoomVaultB(); set => _DoomVaultB = value; }    private static DoomVaultB _DoomVaultB;
    private static Downward Downward { get => _Downward ??= new Downward(); set => _Downward = value; }    private static Downward _Downward;
    private static DracoCon DracoCon { get => _DracoCon ??= new DracoCon(); set => _DracoCon = value; }    private static DracoCon _DracoCon;
    private static DragonFableOrigins DragonFableOrigins { get => _DragonFableOrigins ??= new DragonFableOrigins(); set => _DragonFableOrigins = value; }    private static DragonFableOrigins _DragonFableOrigins;
    private static DragonRoad DragonRoad { get => _DragonRoad ??= new DragonRoad(); set => _DragonRoad = value; }    private static DragonRoad _DragonRoad;
    private static DreamPalace DreamPalace { get => _DreamPalace ??= new DreamPalace(); set => _DreamPalace = value; }    private static DreamPalace _DreamPalace;
    private static Dwarfhold Dwarfhold { get => _Dwarfhold ??= new Dwarfhold(); set => _Dwarfhold = value; }    private static Dwarfhold _Dwarfhold;
    private static DwarvesVsGiants DwarvesVsGiants { get => _DwarvesVsGiants ??= new DwarvesVsGiants(); set => _DwarvesVsGiants = value; }    private static DwarvesVsGiants _DwarvesVsGiants;

    private static Eden Eden { get => _Eden ??= new Eden(); set => _Eden = value; }    private static Eden _Eden;
    private static EtherStormWastes EtherStormWastes { get => _EtherStormWastes ??= new EtherStormWastes(); set => _EtherStormWastes = value; }    private static EtherStormWastes _EtherStormWastes;
    private static ExaltiaTower ExaltiaTower { get => _ExaltiaTower ??= new ExaltiaTower(); set => _ExaltiaTower = value; }    private static ExaltiaTower _ExaltiaTower;
    private static Extinction Extinction { get => _Extinction ??= new Extinction(); set => _Extinction = value; }    private static Extinction _Extinction;

    private static FableForest FableForest { get => _FableForest ??= new FableForest(); set => _FableForest = value; }    private static FableForest _FableForest;
    private static Friendship Friendship { get => _Friendship ??= new Friendship(); set => _Friendship = value; }    private static Friendship _Friendship;
    private static FrozenNorthlands FrozenNorthlands { get => _FrozenNorthlands ??= new FrozenNorthlands(); set => _FrozenNorthlands = value; }    private static FrozenNorthlands _FrozenNorthlands;

    private static Gamehaven Gamehaven { get => _Gamehaven ??= new Gamehaven(); set => _Gamehaven = value; }    private static Gamehaven _Gamehaven;
    private static GiantTaleStory GiantTaleStory { get => _GiantTaleStory ??= new GiantTaleStory(); set => _GiantTaleStory = value; }    private static GiantTaleStory _GiantTaleStory;
    private static GlaceraStory GlaceraStory { get => _GlaceraStory ??= new GlaceraStory(); set => _GlaceraStory = value; }    private static GlaceraStory _GlaceraStory;
    private static Guru Guru { get => _Guru ??= new Guru(); set => _Guru = value; }    private static Guru _Guru;

    private static HuntersMoon HuntersMoon { get => _HuntersMoon ??= new HuntersMoon(); set => _HuntersMoon = value; }    private static HuntersMoon _HuntersMoon;

    private static IcePlane IcePlane { get => _IcePlane ??= new IcePlane(); set => _IcePlane = value; }    private static IcePlane _IcePlane;

    private static LaeWedding LaeWedding { get => _LaeWedding ??= new LaeWedding(); set => _LaeWedding = value; }    private static LaeWedding _LaeWedding;
    private static Lair Lair { get => _Lair ??= new Lair(); set => _Lair = value; }    private static Lair _Lair;
    private static Lightguard Lightguard { get => _Lightguard ??= new Lightguard(); set => _Lightguard = value; }    private static Lightguard _Lightguard;
    private static LightoviaCave LightoviaCave { get => _LightoviaCave ??= new LightoviaCave(); set => _LightoviaCave = value; }    private static LightoviaCave _LightoviaCave;
    private static LostVilla LostVilla { get => _LostVilla ??= new LostVilla(); set => _LostVilla = value; }    private static LostVilla _LostVilla;

    private static Manor Manor { get => _Manor ??= new Manor(); set => _Manor = value; }    private static Manor _Manor;
    private static Marsh2 Marsh2 { get => _Marsh2 ??= new Marsh2(); set => _Marsh2 = value; }    private static Marsh2 _Marsh2;
    private static Mazumi Mazumi { get => _Mazumi ??= new Mazumi(); set => _Mazumi = value; }    private static Mazumi _Mazumi;
    private static Mobius Mobius { get => _Mobius ??= new Mobius(); set => _Mobius = value; }    private static Mobius _Mobius;
    private static MustyCave MustyCave { get => _MustyCave ??= new MustyCave(); set => _MustyCave = value; }    private static MustyCave _MustyCave;

    private static NecroProject NecroProject { get => _NecroProject ??= new NecroProject(); set => _NecroProject = value; }    private static NecroProject _NecroProject;
    private static Noobshire Noobshire { get => _Noobshire ??= new Noobshire(); set => _Noobshire = value; }    private static Noobshire _Noobshire;
    private static Nukemichi Nukemichi { get => _Nukemichi ??= new Nukemichi(); set => _Nukemichi = value; }    private static Nukemichi _Nukemichi;
    private static NytheraSaga NytheraSaga { get => _NytheraSaga ??= new NytheraSaga(); set => _NytheraSaga = value; }    private static NytheraSaga _NytheraSaga;

    private static J6Saga J6Saga { get => _J6Saga ??= new J6Saga(); set => _J6Saga = value; }    private static J6Saga _J6Saga;

    private static Pirates Pirates { get => _Pirates ??= new Pirates(); set => _Pirates = value; }    private static Pirates _Pirates;
    private static PoisonForest PoisonForest { get => _PoisonForest ??= new PoisonForest(); set => _PoisonForest = value; }    private static PoisonForest _PoisonForest;

    private static QueenReign QueenReign { get => _QueenReign ??= new QueenReign(); set => _QueenReign = value; }    private static QueenReign _QueenReign;
    private static QuibbleHunt QuibbleHunt { get => _QuibbleHunt ??= new QuibbleHunt(); set => _QuibbleHunt = value; }    private static QuibbleHunt _QuibbleHunt;

    private static RavenlossSaga RavenlossSaga { get => _RavenlossSaga ??= new RavenlossSaga(); set => _RavenlossSaga = value; }    private static RavenlossSaga _RavenlossSaga;
    private static River River { get => _River ??= new River(); set => _River = value; }    private static River _River;

    private static Safiria Safiria { get => _Safiria ??= new Safiria(); set => _Safiria = value; }    private static Safiria _Safiria;
    private static ShadowGates ShadowGates { get => _ShadowGates ??= new ShadowGates(); set => _ShadowGates = value; }    private static ShadowGates _ShadowGates;
    private static ShadowSlayerK ShadowSlayerK { get => _ShadowSlayerK ??= new ShadowSlayerK(); set => _ShadowSlayerK = value; }    private static ShadowSlayerK _ShadowSlayerK;
    private static ShadowVault ShadowVault { get => _ShadowVault ??= new ShadowVault(); set => _ShadowVault = value; }    private static ShadowVault _ShadowVault;
    private static ShadowVoid ShadowVoid { get => _ShadowVoid ??= new ShadowVoid(); set => _ShadowVoid = value; }    private static ShadowVoid _ShadowVoid;
    private static Shattersword Shattersword { get => _Shattersword ??= new Shattersword(); set => _Shattersword = value; }    private static Shattersword _Shattersword;
    private static Shinkansen Shinkansen { get => _Shinkansen ??= new Shinkansen(); set => _Shinkansen = value; }    private static Shinkansen _Shinkansen;
    private static ShipWreck ShipWreck { get => _ShipWreck ??= new ShipWreck(); set => _ShipWreck = value; }    private static ShipWreck _ShipWreck;
    private static SkyGuardSaga SkyGuardSaga { get => _SkyGuardSaga ??= new SkyGuardSaga(); set => _SkyGuardSaga = value; }    private static SkyGuardSaga _SkyGuardSaga;
    private static SpirePast SpirePast { get => _SpirePast ??= new SpirePast(); set => _SpirePast = value; }    private static SpirePast _SpirePast;
    private static StarSinc StarSinc { get => _StarSinc ??= new StarSinc(); set => _StarSinc = value; }    private static StarSinc _StarSinc;
    private static SuperDeath SuperDeath { get => _SuperDeath ??= new SuperDeath(); set => _SuperDeath = value; }    private static SuperDeath _SuperDeath;

    private static ThirdSpell ThirdSpell { get => _ThirdSpell ??= new ThirdSpell(); set => _ThirdSpell = value; }    private static ThirdSpell _ThirdSpell;
    private static TitanAttackStory TitanAttackStory { get => _TitanAttackStory ??= new TitanAttackStory(); set => _TitanAttackStory = value; }    private static TitanAttackStory _TitanAttackStory;
    private static Tournament Tournament { get => _Tournament ??= new Tournament(); set => _Tournament = value; }    private static Tournament _Tournament;
    private static Tower Tower { get => _Tower ??= new Tower(); set => _Tower = value; }    private static Tower _Tower;
    private static TowerOfDoom TowerOfDoom { get => _TowerOfDoom ??= new TowerOfDoom(); set => _TowerOfDoom = value; }    private static TowerOfDoom _TowerOfDoom;
    private static Tutorial Tutorial { get => _Tutorial ??= new Tutorial(); set => _Tutorial = value; }    private static Tutorial _Tutorial;

    private static Ubear Ubear { get => _Ubear ??= new Ubear(); set => _Ubear = value; }    private static Ubear _Ubear;
    private static UnderGroundLab UnderGroundLab { get => _UnderGroundLab ??= new UnderGroundLab(); set => _UnderGroundLab = value; }    private static UnderGroundLab _UnderGroundLab;

    public LairWar LairWar
    {
        get => _LairWar ??= new LairWar();
        set => _LairWar = value;
    }
    public LairWar _LairWar; //VasalkarLairWar.cs

    private static WatchTower WatchTower { get => _WatchTower ??= new WatchTower(); set => _WatchTower = value; }    private static WatchTower _WatchTower;
    private static whitetigerpoint WhiteTigerPoint { get => _WhiteTigerPoint ??= new whitetigerpoint(); set => _WhiteTigerPoint = value; }    private static whitetigerpoint _WhiteTigerPoint;
    private static WillowCreek WillowCreek { get => _WillowCreek ??= new WillowCreek(); set => _WillowCreek = value; }    private static WillowCreek _WillowCreek;

    private static XansLair Xans { get => _Xans ??= new XansLair(); set => _Xans = value; }    private static XansLair _Xans;

    private static YokaiQuests Yokai { get => _Yokai ??= new YokaiQuests(); set => _Yokai = value; }    private static YokaiQuests _Yokai;

    #endregion

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CompleteAll();

        Core.SetOptions(false);
    }

    public void CompleteAll()
    {
        Tutorial.Badges();
        Core.Logger($"Story: Tutorial - Complete");

        #region In folders
        #region 13LOC
        Adv.GearStore();
        LOC.Complete13LOC(true);
        Adv.GearStore(true);
        Core.Logger($"Saga: The 13 Lords of Chaos - Complete");
        if (Core.IsMember)
        {
            DeadlyDungeon.DeadlyDungeonQuest_Line();
            Core.Logger($"Story: Deadly Dungeon - Complete");

            KillerCatacombs.KillerCatacombs_Line();
            Core.Logger($"Story: Killer Catacombs - Complete");

            PyramidofPain.PyramidofPain_Line();
            Core.Logger($"Story: Pyramid of Pain - Complete");
        }
        Core.Logger($"Saga: The 13 Lords of Chaos (Extra) - Complete");
        #endregion

        #region 7DD
        DD.Complete7DD();
        Core.Logger($"Saga: 7 Deadly Dragons - Complete");

        Egg.Hatch();
        Core.Logger($"Saga: 7 Deadly Dragons (Extra) - Complete");
        #endregion

        #region AOR
        AOR.DoAll();
        Core.Logger($"Saga: Age Of Ruin - Complete");
        #endregion

        #region Doomwood
        DW.CompleteDoomwood();
        Core.Logger($"Saga: Doomwood - Complete");
        #endregion

        #region DOY
        DOY.DoAll();
        Core.Logger($"Saga: Dragons Of Yokai - Complete");
        #endregion

        #region Elergy
        CoreAstravia.CompleteCoreAstravia();
        Core.Logger($"Saga: Elergy of Madness - Complete");
        #endregion

        #region CoreFireIsland
        FI.CompleteFireIsland();
        Core.Logger($"Saga: Fireisland Maps - Complete");
        #endregion

        #region Hollowborn
        HB.DoAll();
        Core.Logger($"Saga: Hollowborn - Complete");
        #endregion

        #region CoreFriday13th
        CoreFriday13th.CompleteFriday13th();
        Core.Logger($"Saga: Friday 13th - Complete");
        #endregion

        #region IsleOfFotia
        CoreIsleOfFotia.CompleteALL();
        Core.Logger($"Saga: Isle of Fotia - Complete");
        #endregion

        #region Legion
        #region CoreDageTheEvilIsland
        DageIsland.CompleteDageTheEvilIslandStory();
        Core.Logger($"Saga: DageTheEvil island Maps - Complete");
        #endregion

        AtlasFalls.Storyline();
        Core.Logger($"Story: Atlas Falls - Complete");

        AtlasKingdom.Storyline();
        Core.Logger($"Story: Atlas Kingdom - Complete");

        AtlasPromenade.Storyline();
        Core.Logger($"Story: Atlas Promenade - Complete");

        DageChallengeStory.DageChallengeQuests();
        Core.Logger($"Story: Dage Challenge - Complete");

        DarkWar.DarkWarLegion();
        DarkWar.DarkWarNation();
        Core.Logger($"Story: Dark War - Complete");

        Ravenscar.Storyline();
        Core.Logger($"Story: Ravenscar - Complete");

        SeraphicWar_Story.SeraphicWar_Questline();
        Core.Logger($"Story: Seraphic War - Complete");

        SevenCircles.CirclesWar();
        Core.Logger($"Story: Seven Circles - Complete");

        WorldSoul.WorldSoulQuests();
        Core.Logger($"Story: World Soul - Complete");
        #endregion

        #region Lynaria
        Lynaria.DoAll();
        Core.Logger($"Story: Lynaria - Complete");
        #endregion

        #region Nation
        Bamboozle.BamboozleQuest();
        Core.Logger($"Story: Bamboozle - Complete");

        CitadelRuins.DoAll();
        Core.Logger($"Story: Citadel Ruins - Complete");

        DeleuzeTundra.DeleuzeTundra();
        Core.Logger($"Story: Deleuze Tundra - Complete");

        Fiendshard_Story.Fiendshard_QuestlineP1();
        Core.Logger($"Story: Fiendshard - Complete");

        FiendPast.DoAll();
        Core.Logger($"Story: Fiend Past - Complete");

        OblivionTundra.Storyline();
        Core.Logger($"Story: Oblivion Tundra - Complete");

        Originul_Story.Originul_Questline();
        Core.Logger($"Story: Originul - Complete");

        ShadowBlastArena.Doall();
        Core.Logger($"Story: ShadowBlast Arena - Complete");

        Tercessuinotlim.JadziaQuests();
        Core.Logger($"Story: Tercessuinotlim - Complete");

        VoidRefuge.Storyline();
        Core.Logger($"Story: Void Refuge - Complete");

        VoidChasm.Storyline();
        Core.Logger($"Story: Void Chasm - Complete");
        #endregion

        #region QoM
        QOM.CompleteEverything();
        Core.Logger($"Saga: Queen of Monsters - Complete");

        BrightOak.doall();
        Core.Logger($"Story: BrightOak - Complete");

        CelestialArena.Arena1to10();
        CelestialArena.Arena11to20();
        CelestialArena.Arena21to29();
        Core.Logger($"Story: Celestial Arena - Complete");

        CelestialPast.CompleteCeletialPast();
        Core.Logger($"Story: Celestial Past - Complete");

        GoldenArena.StoryLine();
        Core.Logger($"Story: GoldenArena - Complete");

        InfernalDianoia.Storyline();
        Core.Logger($"Story: Infernal Dianoia - Complete");

        InfernalParadise.Storyline();
        Core.Logger($"Story: Infernal Paradise - Complete");

        LivingDungeon.LivingDungeonStory();
        Core.Logger($"Story: LivingDungeon - Complete");

        OrbHunt.OrbHuntSaga();
        Core.Logger($"Story: Orb Hunt - Complete");

        QueenBattle.StoryLine();
        Core.Logger($"Story: QueenBattle - Complete");
        #endregion

        #region Sepulchure Saga
        CoreSS.CompleteSS();
        Core.Logger($"Saga: Sepulchure - Complete");
        #endregion

        #region Shadows Of Chaos
        CoreSoC.CompleteCoreSoC();
        Core.Logger($"Saga: Shadows of Chaos - Complete");
        #endregion

        #region Shadow Of War
        SOW.CompleteCoreSoW();
        Core.Logger($"Saga: Shadow of War [Part1&2] - Complete");
        #endregion

        #region Summer 2015 AdventureMap
        CoreSummer.DoAll();
        Core.Logger($"Saga: Summer 2015 AdventureMap - Complete");
        #endregion

        #region ToD
        // Skip rep quests
        TOD.CompleteToD(false);
        Core.Logger($"Saga: Throne of Darkness - Complete");
        #endregion

        #endregion

        #region Standalone
        Adam1A1Quest.Storyline();
        Core.Logger($"Story: Adam1a1 - Complete");


        AranxQuests.StoryLine();
        Core.Logger($"Story: Aranx - Complete");

        Arcangrove.GravelynandVictoria();
        Core.Logger($"Story: Arcangrove - Complete");

        AriaGreenhouse.DoAll();
        Core.Logger($"Story: Aria Greenhouse - Complete");

        if (Core.IsMember)
        {
            AriaPet.StoryLine();
            Core.Logger($"Story: Aria Pet - Complete");
        }

        Artixpointe.OmniArtifact();
        Core.Logger($"Story: Artixpointe - Complete");

        ArtixWedding.ArtixWeddingComplete();
        Core.Logger($"Story: ArtixWedding - Complete");

        Asylum.StoryLine();
        Core.Logger($"Story: Asylum - Complete");


        Banished.doall();
        Core.Logger($"Story: Banished - Complete");

        BaseCamp.StoryLine();
        Core.Logger($"Story: Base Camp - Complete");

        BattleUnder.BattleUnderAll();
        Core.Logger($"Story: BattleUnder - Complete");

        BeleensDream.StoryLine();
        Core.Logger($"Story: BeleensDream - Complete");

        BloodMoon.BloodMoonSaga();
        Core.Logger($"Story: Blood Moon - Complete");

        Bludrut.StoryLine();
        Core.Logger($"Story: Bludrut - Complete");

        if (!Core.HasAchievement(30, "ip6") || Core.isCompletedBefore(5981) || !Core.CheckInventory(27222) || !Core.IsMember)
        {
            BoneBreak.StoryLine();
            Core.Logger($"Story: BoneBreak - Complete");
        }

        Borgars.StoryLine();
        Core.Logger($"Story: Borgars - Complete");

        BrightCrystal.CrystalBrightQuests();
        Core.Logger($"Story: Bright Crystal - Complete");


        CastleOfGlass.StoryLine();
        Core.Logger($"Story: CastleOfGlass - Complete");

        CastleTunnels.StoryLine();
        Core.Logger($"Story: CastleTunnels - Complete");

        CelestialPast.CompleteCeletialPast();
        Core.Logger($"Story: CelestialPast - Complete");

        Cleric.StoryLine();
        Core.Logger($"Story: Cleric - Complete");

        if (Core.IsMember)
        {
            Concert.DoAll();
            Core.Logger($"Story: Concert Event - Complete");
        }

        Cornelis.StoryLine();
        Core.Logger($"Story: Hodan Quests - Complete");

        CrashSite.StoryLine();
        Core.Logger($"Story: CrashSite - Complete");

        CruxShip.StoryLine();
        Core.Logger($"Story: CruxShip - Complete");


        // DarkCarnax.Storyline();
        // Core.Logger($"Story: Nightmare Carnax - Complete");

        DarkDungeon.Storyline();
        Core.Logger($"Story: Dark Dungeon - Complete");


        if (Core.IsMember)
        {
            CoreFriday13th.Deadfly();
            Core.Logger($"Story: DeadFly - Complete");
        }

        DeathsRealm.StoryLine();
        Core.Logger($"Story: Death's Realm - Complete");

        DjinnGateStory.DjinnGate();
        Core.Logger($"Story: Djinn Gate - Complete");

        DjinnGuard.CompleteDjinnGuard();
        Core.Logger($"Story: Djinn Guard - Complete");

        DoomVaultA.StoryLine();
        Core.Logger($"Story: Doom Vault B - Complete");
        DoomVaultB.StoryLine();

        Core.Logger($"Story: Doom Vault A - Complete");

        Downward.StoryLine();
        Core.Logger($"Story: Downward - Complete");

        DracoCon.StoryLine();
        Core.Logger($"Story: Draco Con - Complete");

        DragonFableOrigins.DragonFableOriginsAll();
        Core.Logger($"Saga: Dragon Fable Origins - Complete");

        if (Core.HasAchievement(22, "ip9") || Core.HasAchievement(15, "ip11") || Core.HasAchievement(8, "ip14") || !Core.HasAchievement(10, "ip16") || !Core.HasAchievement(12, "ip17") || !Core.HasAchievement(18, "ip18") || !Core.HasAchievement(2, "ip20"))
        {
            DragonRoad.StoryLine();
            Core.Logger($"Story: DragonRoad - Complete");
        }

        DreamPalace.StoryLine();
        Core.Logger($"Story: Dream Palace - Complete");

        Dwarfhold.DoAll();
        Core.Logger($"Story: Dwarfhold - Complete");

        DwarvesVsGiants.StoryLine();
        Core.Logger($"Story: Dwarves Vs Giants - Complete");


        Eden.StoryLine();
        Core.Logger($"Story: Eden - Complete");

        EtherStormWastes.DoAll();
        Core.Logger($"Saga: Ether Storm Wastes - Complete");

        ExaltiaTower.StoryLine();
        Core.Logger($"Story: Exaltia Tower - Complete");

        Extinction.StoryLine();
        Core.Logger($"Story: Extinction - Complete");

        FableForest.StoryLine();
        Core.Logger($"Story: FableForest - Complete");

        Friendship.CompleteStory();
        Core.Logger($"Story: Friendship - Complete");

        FrozenNorthlands.Storyline();
        Core.Logger($"Story: Frozen Northlands - Complete");




        HuntersMoon.StoryLine();
        Core.Logger($"Story: Hunter's Moon - Complete");

        if (Core.PrivateRooms == true)
        {
            Gamehaven.Storyline();
            Core.Logger($"Story: Game Haven - Complete");
        }

        GiantTaleStory.DoAll();
        Core.Logger($"Story: Giant Tale - Complete");

        GlaceraStory.DoAll();
        Core.Logger($"Story: Glacera - Complete");

        Guru.StoryLine();
        Core.Logger($"Story: Guru - Complete");


        IcePlane.StoryLine();
        Core.Logger($"Story: Ice Plane - Complete");


        J6Saga.J6();
        Core.Logger($"Sage: J6 - Complete");


        LaeWedding.Storyline();
        Core.Logger($"Story: Lae's Wedding - Complete");

        Lair.DoAll();
        Core.Logger($"Story: Lair - Complete");

        if (Core.IsMember)
        {
            Lightguard.StoryLine();
            Core.Logger($"Story: Lightguard Michem Quests - Complete");
        }

        LightoviaCave.LightoviaCaveQuests();
        Core.Logger($"Story: LightoviaCave - Complete");

        LostVilla.Storyline();
        Core.Logger($"Story: Lost Villa - Complete");


        Manor.StoryLine();
        Core.Logger($"Story: Manor - Complete");

        Mazumi.MazumiQuests();
        Core.Logger($"Story: Mazumi Quests - Complete");

        Mobius.DoAll();
        Core.Logger($"Story: Mobius Quests - Complete");

        if (Core.IsMember)
        {
            Marsh2.StoryLine();
            Core.Logger($"Story: Marsh 2 - Complete");
        }

        MustyCave.Storyline();
        Core.Logger($"Story: MustyCave - Complete");


        NecroProject.StoryLine();
        Core.Logger($"Story: Necro Project - Complete");

        Noobshire.doAll();
        Core.Logger($"Story: Noobshire - Complete");

        Nukemichi.NukemichiQuests();
        Core.Logger($"Saga: Nukemichi Quests - Complete");

        NytheraSaga.DoAll();
        Core.Logger($"Saga: Nythera - Complete");


        if (Core.IsMember)
        {
            Pirates.StoryLine();
            Core.Logger($"Story: Pirates - Complete");
        }

        PoisonForest.StoryLine();
        Core.Logger($"Story: PoisonForest - Complete");


        QueenReign.DoAll();
        Core.Logger($"Story: QueenReign - Complete");

        QuibbleHunt.StoryLine();
        Core.Logger($"Story: QuibbleHunt - Complete");


        RavenlossSaga.DoAll();
        Core.Logger($"Saga: RavenLoss - Complete");

        River.StoryLine();
        Core.Logger($"Saga: River - Complete");


        if (Core.IsMember)
        {
            Safiria.StoryLine();
            Core.Logger($"Story: Safiria - Complete");
        }

        ShadowGates.StoryLine();
        Core.Logger($"Story: ShadowGates - Complete");

        ShadowSlayerK.Storyline();
        Core.Logger($"Story: Shadow Slayer K - Complete");

        ShadowVault.StoryLine();
        Core.Logger($"Story: ShadowVault - Complete");

        ShadowVoid.ShadowVoidQuests();
        Core.Logger($"Story: ShadowVoid - Complete");

        Shattersword.StoryLine();
        Core.Logger($"Story: Shattersword - Complete");

        Shinkansen.Storyline();
        Core.Logger($"Story: Shinkansen - Complete");

        ShipWreck.StoryLine();
        Core.Logger($"Story: ShipWreck - Complete");

        SkyGuardSaga.DoAll();
        Core.Logger($"Saga: SkyGuard Saga - Complete");

        SpirePast.Storyline();
        Core.Logger($"Story: Spire Past - Complete");

        StarSinc.StarSincQuests();
        Core.Logger($"Story: Star Sinc - Complete");

        SuperDeath.StoryLine();
        Core.Logger($"Story: SuperDeath - Complete");



        ThirdSpell.StoryLine();
        Core.Logger($"Story: Third Spell - Complete");

        TitanAttackStory.DoAll();
        Core.Logger($"Story: Titan Attack - Complete");

        Tournament.StoryLine();
        Core.Logger($"Story: Tournament - Complete");

        Tower.StoryLine();
        Core.Logger($"Story: Tower - Complete");

        TowerOfDoom.TowerProgress();
        Core.Logger($"Story: Tower Of Doom - Complete");


        Ubear.StoryLine();
        Core.Logger($"Story: Ubear - Complete?");

        UnderGroundLab.partofundergroundlabb();
        Core.Logger($"Story: Underground Lab - Complete?");


        LairWar.doAll();
        Core.Logger($"Story: Vasalkar Lair War - Complete");


        WatchTower.StoryLine();
        Core.Logger($"Story: WatchTower - Complete");

        WhiteTigerPoint.DoStory();
        Core.Logger($"Story: White Tiger Point - Complete");

        WillowCreek.StoryLine();
        Core.Logger($"Story: Willow Creek - Complete");


        Xans.DoAll();
        Core.Logger($"Story: Xans Lair - Complete");


        Yokai.Quests();
        Core.Logger($"Story: Yokai - Complete");

        #endregion
    }
}
