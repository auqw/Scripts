/*
name: All Classes
description: This script will get all of the classes that are currently farmable.
tags: all classes, class, farm, complete, all
*/

#region  includes
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Dailies/Cryomancer.cs
//cs_include Scripts/Other/Classes/Daily-Classes/BlazeBinder.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/RavenlossSaga.cs
//cs_include Scripts/Other/Classes/REP-based/Arachnomancer.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Other/Classes/REP-based/Bard.cs
//cs_include Scripts/Other/Classes/REP-based/ChaosSlayer.cs
//cs_include Scripts/Other/Classes/REP-based/DarkbloodStormKing.cs
//cs_include Scripts/Other/Classes/REP-based/DeathKnight.cs
//cs_include Scripts/Other/Classes/REP-based/ElementalDracomancer.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Other/Classes/REP-based/EternalInversionist.cs
//cs_include Scripts/Other/Classes/REP-based/EvolvedShaman.cs
//cs_include Scripts/Other/Classes/REP-based/GlacialBerserker.cs
//cs_include Scripts/Other/Classes/REP-based/HorcEvader.cs
//cs_include Scripts/Other/Classes/REP-based/ImperialChunin.cs
//cs_include Scripts/Other/Classes/REP-based/Lycan.cs
//cs_include Scripts/Other/Classes/REP-based/MasterRanger.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Other/Classes/REP-based/RoyalBattleMage.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Other/Classes/REP-based/ThiefOfHours.cs
//cs_include Scripts/Other/Classes/REP-based/TrollSpellsmith.cs
//cs_include Scripts/Other/Classes/REP-based/BeastMaster[Mem].cs
//cs_include Scripts/Other/Classes/REP-based/UndeadSlayer[Mem].cs
//cs_include Scripts/Evil/DoomKnight[Mem].cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Other/Classes/LegionDoomKnight[Mem].cs
//cs_include Scripts/Other/Classes/Curio-Classes/LegendaryElementalWarrior[mem].cs
//cs_include Scripts/Other/Classes/Members-CLasses/Acolyte[Mem].cs
//cs_include Scripts/Other/Classes/Members-CLasses/AlphaOmega[Mem].cs
//cs_include Scripts/Story/Safiria[Member].cs
//cs_include Scripts/Other/MergeShops/BloodAncientMerge.cs
//cs_include Scripts/Other/Classes/Members-CLasses/BloodAncient[Mem].cs
//cs_include Scripts/Other/Classes/Members-CLasses/BloodTitan[Mem].cs
//cs_include Scripts/Other/MergeShops/BloodTitanMerge[Mem].cs
//cs_include Scripts/Other/Classes/Members-CLasses/ChronoAssassin[Mem].cs
//cs_include Scripts/Other/MergeShops/DeathPitArenaRepMerge.cs
//cs_include Scripts/Other/Classes/Members-CLasses/DrakelWarlord[Mem].cs
//cs_include Scripts/Other/Classes/Members-CLasses/Renegade[Mem].cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowDragonShinobiMerge.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowDragonShinobi.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Seasonal/Frostvale/FrostvalBarbarian.cs
//cs_include Scripts/Seasonal/Frostvale/NorthlandsMonk.cs
//cs_include Scripts/Seasonal/LuckyDay/LuckyDayShamrockFairMerge.cs
//cs_include Scripts/Seasonal/LuckyDay/EvolvedLeprechaun.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/CoreDageBirthday.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/DarkBirthdayTokenMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/ExaltedHarbinger.cs
//cs_include Scripts/Seasonal/MayThe4th/DarkLord.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonStory.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonMerge.cs
//cs_include Scripts/Seasonal/Mogloween/PumpkinLord(Class).cs
//cs_include Scripts/Seasonal/Mogloween/VampireLord(Class).cs
//cs_include Scripts/Legion/LegionMaterials/SoulSand.cs
//cs_include Scripts/Legion/Various/LegionBonfire.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/LegionSwordMasterAssassin.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/BlazeBeardStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/BlazeBeardMerge.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/AlphaPirate.cs
//cs_include Scripts/Story/Legion/AtlasFalls.cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Legion/MergeShops/AtlasFallsGearMerge.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/PirateClass.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs

//cs_include Scripts/Evil/VordredsArmor.cs
//cs_include Scripts/Other/Concerts/BattleConcert2023.cs
//cs_include Scripts/Other/Concerts/NeoMetalNecro.cs
//cs_include Scripts/Other/Concerts/DoomMetalNecro.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise3.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise4.cs
//cs_include Scripts/Legion/MergeShops/UndeadLegionMerge.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Legion/Various/ExaltedSoulCleaver.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Other/Classes/BloodSorceress.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Other/Classes/DragonShinobi.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Other/Classes/Dragonslayer.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/Enforcer.cs
//cs_include Scripts/Other/Classes/FrostSpiritReaver.cs
//cs_include Scripts/Other/Classes/GrimNecromancer[600kAC].cs
//cs_include Scripts/Other/Classes/HighSeasCommander[10y].cs
//cs_include Scripts/Other/Classes/LightMage.cs
//cs_include Scripts/Other/Classes/MechaJouster.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/Classes/ProtoSartorium.cs
//cs_include Scripts/Other/Classes/Rustbucket.cs
//cs_include Scripts/Other/Classes/ScarletSorceress.cs
//cs_include Scripts/Other/Classes/SkyChargedGrenadier[9yMem].cs
//cs_include Scripts/Other/Classes/Curio-Classes/AbyssalAngelsShadow.cs
//cs_include Scripts/Chaos/ChaosAvengerPreReqs.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Other/Classes/ArchMage/CoreArchMage.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Other/MergeShops/TerminaTempleMerge.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DoomPirateStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/DoomPirateHaulMerge.cs
//cs_include Scripts/Other/Classes/VerusDoomKnight.cs
//cs_include Scripts/Other/Weapons/AvatarOfDeathsScythe.cs
//cs_include Scripts/Other/Weapons/GuardianOfSpiritsBlade.cs
//cs_include Scripts/Other/Weapons/LanceOfTime.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
//cs_include Scripts/Other/Classes/LightCaster.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Other/MergeShops/GooseMerge.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Other/MergeShops/BrightForestMerge.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalArena.cs
//cs_include Scripts/Other/MergeShops/DoomLegacyMerge.cs
//cs_include Scripts/Other/MergeShops/CelestialChallengerMerge.cs
//cs_include Scripts/Other/MergeShops/SpoilsofLightMerge.cs
//cs_include Scripts/Seasonal/NewYear/ArchiveofTimeMerge.cs
//cs_include Scripts/Other/MergeShops/CrocriverMerge.cs
//cs_include Scripts/Other/MergeShops/SuperSlayinMerge.cs
//cs_include Scripts/Story/DreamPalace.cs
//cs_include Scripts/Other/MergeShops/DreampalaceMerge.cs
//cs_include Scripts/Other/MergeShops/BonecastleMerge.cs
//cs_include Scripts/Other/MergeShops/BonecastleTowerMerge.cs
//cs_include Scripts/Other/MergeShops/CelestialRealmMerge.cs
//cs_include Scripts/Other/MergeShops/3LittleWolvesHousesMerge.cs
//cs_include Scripts/Other/Various/Potions.cs
//cs_include Scripts/Story/CruxShip.cs
//cs_include Scripts/Seasonal/Mogloween/MoonlightKhopeshMerge.cs
//cs_include Scripts/Other/MergeShops/ThirdspellMerge.cs
//cs_include Scripts/Seasonal/Friday13th/MergeShops/ShadowMerge.cs
//cs_include Scripts/Darkon/MergeShops/ArcanaInvokerResourceMerge.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowofDoom/CoreShadowofDoom.cs
//cs_include Scripts/Story/FableForest.cs
//cs_include Scripts/Other/Classes/ArcanaInvoker[Non-Insignia].cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Other/MergeShops/FelixsGildedGearMerge.cs
//cs_include Scripts/Other/MergeShops/LoughshineLootMerge.cs
//cs_include Scripts/Other/MergeShops/LiaTaraHillLootMerge.cs
//cs_include Scripts/Other/MergeShops/ColdThunderMerge.cs
//cs_include Scripts/Other/MergeShops/LothianTreasuryMerge.cs
//cs_include Scripts/Other/Classes/SovereignOfStorms.cs
//cs_include Scripts/Other/Classes/Sentinel.cs
//cs_include Scripts/Other/Classes/MartialArtist.cs
//cs_include Scripts/Other/Classes/NoClassClasses/NoHollowbornClass.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Hollowborn/HollowbornVindicator(NonInsignia).cs
//cs_include Scripts/Hollowborn/Materials/VindicatorBadge.cs
//cs_include Scripts/Hollowborn/Materials/DeathsPower.cs
//cs_include Scripts/Hollowborn/Materials/GraceOrb.cs
//cs_include Scripts/Hollowborn/Materials/GramielsEmblem.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorCrest.cs
//cs_include Scripts/Seasonal/Friday13th/MergeShops/ColossalWaresMerge.cs
//cs_include Scripts/Farm/REP/GrimskullTrollingRep.cs
//cs_include Scripts/Prototypes/MoreSkullsBoss.cs
//cs_include Scripts/Other/Classes/Lich.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/DoomVault.cs
//cs_include Scripts/Story/DoomVaultB.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Prototypes/Grimgaol.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Other/MergeShops/InfernalArenaMerge.cs
//cs_include Scripts/Other/Classes/DeathKnightLord[mem].cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/Classes/KingsEcho.cs
#endregion includes

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AllClasses
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    #region Dailies
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;
    private static BlazeBinder BB { get => _BB ??= new BlazeBinder(); set => _BB = value; }
    private static BlazeBinder _BB;
    private static Cryomancer Cryo { get => _Cryo ??= new Cryomancer(); set => _Cryo = value; }
    private static Cryomancer _Cryo;
    private static LordOfOrder LOO { get => _LOO ??= new LordOfOrder(); set => _LOO = value; }
    private static LordOfOrder _LOO;
    #endregion Dailies

    #region Rep
    private static Arachnomancer Arach { get => _Arach ??= new Arachnomancer(); set => _Arach = value; }
    private static Arachnomancer _Arach;
    private static ChaosSlayer CS { get => _CS ??= new ChaosSlayer(); set => _CS = value; }
    private static ChaosSlayer _CS;
    private static DarkbloodStormKing DBSK { get => _DBSK ??= new DarkbloodStormKing(); set => _DBSK = value; }
    private static DarkbloodStormKing _DBSK;
    private static ElementalDracomancer ED { get => _ED ??= new ElementalDracomancer(); set => _ED = value; }
    private static ElementalDracomancer _ED;
    private static EternalInversionist EI { get => _EI ??= new EternalInversionist(); set => _EI = value; }
    private static EternalInversionist _EI;
    private static EvolvedShaman ES { get => _ES ??= new EvolvedShaman(); set => _ES = value; }
    private static EvolvedShaman _ES;
    private static GlacialBerserker GB { get => _GB ??= new GlacialBerserker(); set => _GB = value; }
    private static GlacialBerserker _GB;
    private static HorcEvader HE { get => _HE ??= new HorcEvader(); set => _HE = value; }
    private static HorcEvader _HE;
    private static ImperialChunin IC { get => _IC ??= new ImperialChunin(); set => _IC = value; }
    private static ImperialChunin _IC;
    private static Lycan Lycan { get => _Lycan ??= new Lycan(); set => _Lycan = value; }
    private static Lycan _Lycan;
    private static MasterRanger MR { get => _MR ??= new MasterRanger(); set => _MR = value; }
    private static MasterRanger _MR;
    private static Paladin Pal { get => _Pal ??= new Paladin(); set => _Pal = value; }
    private static Paladin _Pal;
    private static RoyalBattleMage RBM { get => _RBM ??= new RoyalBattleMage(); set => _RBM = value; }
    private static RoyalBattleMage _RBM;
    private static Shaman Shaman { get => _Shaman ??= new Shaman(); set => _Shaman = value; }
    private static Shaman _Shaman;
    private static StoneCrusher SC { get => _SC ??= new StoneCrusher(); set => _SC = value; }
    private static StoneCrusher _SC;
    private static ThiefOfHours TOH { get => _TOH ??= new ThiefOfHours(); set => _TOH = value; }
    private static ThiefOfHours _TOH;
    private static TrollSpellsmith TS { get => _TS ??= new TrollSpellsmith(); set => _TS = value; }
    private static TrollSpellsmith _TS;
    #endregion Rep

    #region Member
    private static AlphaOmega AO { get => _AO ??= new AlphaOmega(); set => _AO = value; }
    private static AlphaOmega _AO;
    private static DeathKnightLord DKL { get => _DKL ??= new DeathKnightLord(); set => _DKL = value; }
    private static DeathKnightLord _DKL;
    private static Acolyte Acolyte { get => _Acolyte ??= new Acolyte(); set => _Acolyte = value; }
    private static Acolyte _Acolyte;
    private static Bard Bard { get => _Bard ??= new Bard(); set => _Bard = value; }
    private static Bard _Bard;
    private static BeastMaster BM { get => _BM ??= new BeastMaster(); set => _BM = value; }
    private static BeastMaster _BM;
    private static BloodAncient BA { get => _BA ??= new BloodAncient(); set => _BA = value; }
    private static BloodAncient _BA;
    private static BloodTitan BT { get => _BT ??= new BloodTitan(); set => _BT = value; }
    private static BloodTitan _BT;
    private static ChronoAssassin CA { get => _CA ??= new ChronoAssassin(); set => _CA = value; }
    private static ChronoAssassin _CA;
    private static DeathKnight DK { get => _DK ??= new DeathKnight(); set => _DK = value; }
    private static DeathKnight _DK;
    private static DoomKnight DoomK { get => _DoomK ??= new DoomKnight(); set => _DoomK = value; }
    private static DoomKnight _DoomK;
    private static DrakelWarlord DW { get => _DW ??= new DrakelWarlord(); set => _DW = value; }
    private static DrakelWarlord _DW;
    private static LegionDoomKnight LDK { get => _LDK ??= new LegionDoomKnight(); set => _LDK = value; }
    private static LegionDoomKnight _LDK;
    private static LegendaryElementalWarrior LEW { get => _LEW ??= new LegendaryElementalWarrior(); set => _LEW = value; }
    private static LegendaryElementalWarrior _LEW;
    private static Renegade Ren { get => _Ren ??= new Renegade(); set => _Ren = value; }
    private static Renegade _Ren;
    private static UndeadSlayer US { get => _US ??= new UndeadSlayer(); set => _US = value; }
    private static UndeadSlayer _US;
    #endregion Member

    #region Seasonal
    private static AlphaPirate APir { get => _APir ??= new AlphaPirate(); set => _APir = value; }
    private static AlphaPirate _APir;
    private static DarkLord DL { get => _DL ??= new DarkLord(); set => _DL = value; }
    private static DarkLord _DL;
    private static EvolvedLeprechaun EL { get => _EL ??= new EvolvedLeprechaun(); set => _EL = value; }
    private static EvolvedLeprechaun _EL;
    private static ExaltedHarbinger EH { get => _EH ??= new ExaltedHarbinger(); set => _EH = value; }
    private static ExaltedHarbinger _EH;
    private static FrostvalBarbarian FB { get => _FB ??= new FrostvalBarbarian(); set => _FB = value; }
    private static FrostvalBarbarian _FB;
    private static LegionSwordMasterAssassin LSMA { get => _LSMA ??= new LegionSwordMasterAssassin(); set => _LSMA = value; }
    private static LegionSwordMasterAssassin _LSMA;
    private static NorthlandsMonk NM { get => _NM ??= new NorthlandsMonk(); set => _NM = value; }
    private static NorthlandsMonk _NM;
    private static PirateClass Pirate { get => _Pirate ??= new PirateClass(); set => _Pirate = value; }
    private static PirateClass _Pirate;
    private static ShadowDragonShinobi SDS { get => _SDS ??= new ShadowDragonShinobi(); set => _SDS = value; }
    private static ShadowDragonShinobi _SDS;
    private static PumpkinLord PL { get => _PL ??= new PumpkinLord(); set => _PL = value; }
    private static PumpkinLord _PL;
    private static VampireLord VL { get => _VL ??= new VampireLord(); set => _VL = value; }
    private static VampireLord _VL;
    private static NoHollowbornClass NHBC { get => _NHBC ??= new NoHollowbornClass(); set => _NHBC = value; }
    private static NoHollowbornClass _NHBC;
    #endregion Seasonal

    #region Various
    private static AbyssalAngelsShadow AAS { get => _AAS ??= new AbyssalAngelsShadow(); set => _AAS = value; }
    private static AbyssalAngelsShadow _AAS;
    private static ArchFiend AF { get => _AF ??= new ArchFiend(); set => _AF = value; }
    private static ArchFiend _AF;
    private static BloodSorceress BS { get => _BS ??= new BloodSorceress(); set => _BS = value; }
    private static BloodSorceress _BS;
    private static DoomMetalNecro DMN { get => _DMN ??= new DoomMetalNecro(); set => _DMN = value; }
    private static DoomMetalNecro _DMN;
    private static Dragonslayer DS { get => _DS ??= new Dragonslayer(); set => _DS = value; }
    private static Dragonslayer _DS;
    private static DragonslayerGeneral DSG { get => _DSG ??= new DragonslayerGeneral(); set => _DSG = value; }
    private static DragonslayerGeneral _DSG;
    private static DragonShinobi DSS { get => _DSS ??= new DragonShinobi(); set => _DSS = value; }
    private static DragonShinobi _DSS;
    private static Enforcer Enf { get => _Enf ??= new Enforcer(); set => _Enf = value; }
    private static Enforcer _Enf;
    private static ExaltedSoulCleaver ESC { get => _ESC ??= new ExaltedSoulCleaver(); set => _ESC = value; }
    private static ExaltedSoulCleaver _ESC;
    private static FrostSpiritReaver FSR { get => _FSR ??= new FrostSpiritReaver(); set => _FSR = value; }
    private static FrostSpiritReaver _FSR;
    private static GrimNecromancer GN { get => _GN ??= new GrimNecromancer(); set => _GN = value; }
    private static GrimNecromancer _GN;
    private static HighSeasCommander HSC { get => _HSC ??= new HighSeasCommander(); set => _HSC = value; }
    private static HighSeasCommander _HSC;
    private static InfiniteLegionDC ILDC { get => _ILDC ??= new InfiniteLegionDC(); set => _ILDC = value; }
    private static InfiniteLegionDC _ILDC;
    private static LightMage LM { get => _LM ??= new LightMage(); set => _LM = value; }
    private static LightMage _LM;
    private static MechaJouster MJ { get => _MJ ??= new MechaJouster(); set => _MJ = value; }
    private static MechaJouster _MJ;
    private static MartialArtist MA { get => _MA ??= new MartialArtist(); set => _MA = value; }
    private static MartialArtist _MA;
    private static Necromancer Necro { get => _Necro ??= new Necromancer(); set => _Necro = value; }
    private static Necromancer _Necro;
    private static NeoMetalNecro NMN { get => _NMN ??= new NeoMetalNecro(); set => _NMN = value; }
    private static NeoMetalNecro _NMN;
    private static ProtoSartorium PS { get => _PS ??= new ProtoSartorium(); set => _PS = value; }
    private static ProtoSartorium _PS;
    private static Rustbucket RB { get => _RB ??= new Rustbucket(); set => _RB = value; }
    private static Rustbucket _RB;
    private static ScarletSorceress SS { get => _SS ??= new ScarletSorceress(); set => _SS = value; }
    private static ScarletSorceress _SS;
    private static SkyChargedGrenadier SCG { get => _SCG ??= new SkyChargedGrenadier(); set => _SCG = value; }
    private static SkyChargedGrenadier _SCG;
    private static SwordMaster SM { get => _SM ??= new SwordMaster(); set => _SM = value; }
    private static SwordMaster _SM;
    private static Sentinel Sentinel { get => _Sentinel ??= new Sentinel(); set => _Sentinel = value; }
    private static Sentinel _Sentinel;
    #endregion Various

    #region End game
    private static ArcanaInvoker AI { get => _AI ??= new ArcanaInvoker(); set => _AI = value; }
    private static ArcanaInvoker _AI;
    private static CoreArchMage AM { get => _AM ??= new CoreArchMage(); set => _AM = value; }
    private static CoreArchMage _AM;
    private static ArchPaladin AP { get => _AP ??= new ArchPaladin(); set => _AP = value; }
    private static ArchPaladin _AP;
    private static ChaosAvengerClass CAV { get => _CAV ??= new ChaosAvengerClass(); set => _CAV = value; }
    private static ChaosAvengerClass _CAV;
    private static DragonOfTime DOT { get => _DOT ??= new DragonOfTime(); set => _DOT = value; }
    private static DragonOfTime _DOT;
    private static HBVNonInsig HBV { get => _HBV ??= new HBVNonInsig(); set => _HBV = value; }
    private static HBVNonInsig _HBV;
    private static KingsEcho KE { get => _KE ??= new KingsEcho(); set => _KE = value; }
    private static KingsEcho _KE;
    private static Lich lich { get => _lich ??= new Lich(); set => _lich = value; }
    private static Lich _lich;
    private static LightCaster LC { get => _LC ??= new LightCaster(); set => _LC = value; }
    private static LightCaster _LC;
    private static CoreLR LR { get => _LR ??= new CoreLR(); set => _LR = value; }
    private static CoreLR _LR;
    private static SovereignOfStorms SOS { get => _SOS ??= new SovereignOfStorms(); set => _SOS = value; }
    private static SovereignOfStorms _SOS;
    private static VerusDoomKnightClass VDK { get => _VDK ??= new VerusDoomKnightClass(); set => _VDK = value; }
    private static VerusDoomKnightClass _VDK;
    private static CoreVHL VHL { get => _VHL ??= new CoreVHL(); set => _VHL = value; }
    private static CoreVHL _VHL;
    private static CoreYnR YNR { get => _YNR ??= new CoreYnR(); set => _YNR = value; }
    private static CoreYnR _YNR;
    #endregion End game


    public string OptionsStorage = "GetAllClasses";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("RankALL", "Rankup All Classes", "wether to Rankup the class to 10 after acquiring it", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        int GoldBoostID = Bot.Boosts.GetBoostID(BoostType.Gold, true);
        int ClassBoostID = Bot.Boosts.GetBoostID(BoostType.Class, true);
        int ExperienceBoostID = Bot.Boosts.GetBoostID(BoostType.Experience, true);
        int ReputationBoostID = Bot.Boosts.GetBoostID(BoostType.Reputation, true);

        Core.BankingBlackList.AddRange(
            Bot.Inventory.Items
                .Where(x => x.ID == GoldBoostID || x.ID == ClassBoostID || x.ID == ExperienceBoostID || x.ID == ReputationBoostID)
                .Cast<ItemBase>()
                .Select(item => item.Name)
        );


        Core.SetOptions();

        GetAllClasses();

        Core.SetOptions(false);
    }

    public void GetAllClasses()
    {
        bool rankUpClass = Bot.Config!.Get<bool>("RankALL");

        //some of these are required for forge enhancements
        MoreClassesToGet(rankUpClass);

        // then we start the rest.
        DailyClasses(rankUpClass);
        RepClasses(rankUpClass);
        MemClasses(rankUpClass);
        SeasonalClasses(rankUpClass);
        VariousClasses(rankUpClass);
        EndGameClasses(rankUpClass);
        ACorToHardtoGetClasses(rankUpClass);

    }

    public void MoreClassesToGet(bool rankUpClass)
    {
        Core.Logger("=== Buying `beginner` classes start (will help with forge enhancements later)===");
        Adv.GearStore();
        Core.BuyItem("trainers", 170, "Warrior");
        Core.BuyItem("trainers", 174, "Mage");
        Core.BuyItem("trainers", 176, "Healer");
        Core.BuyItem("trainers", 172, "Rogue");
        Core.BuyItem("classhalla", 178, "Ninja");
        Core.BuyItem(Bot.Map.Name, 299, "Barber");
        Core.BuyItem(Bot.Map.Name, 299, "Oracle");
        Core.BuyItem(Bot.Map.Name, 222, "Battle Warrior");
        Core.BuyItem(Bot.Map.Name, 222, "Battle Healer");
        Core.BuyItem(Bot.Map.Name, 222, "No Class");
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Warrior", "Mage", "Healer", "Rogue", "Ninja", "Barber", "Oracle", "Battle Warrior", "Battle Healer", "No Class" });
        Core.Logger("=== `beginner` classes - Bought! ===");
    }

    public void DailyClasses(bool rankUpClass)
    {
        Core.Logger("=== Doing Daily Classes ===");

        Adv.GearStore();
        CheckAndExecute("Blaze Binder", () => BB.GetClass(rankUpClass));
        CheckAndExecute("The Collector", Daily.CollectorClass);
        CheckAndExecute("Cryomancer", () => Cryo.DoCryomancer(rankUpClass));
        CheckAndExecute("Lord of Order", () => LOO.GetLoO(rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Blaze Binder", "The Collector", "Cryomancer", "Death KnightLord", "Lord of Order" });

        Core.Logger("=== Daily Classes - Completed! ===");
    }

    public void RepClasses(bool rankUpClass)
    {
        Core.Logger("=== Doing Reputation Classes ===");

        Adv.GearStore();
        CheckAndExecute("Arachnomancer", () => Arach.GetArach(rankUpClass));
        CheckAndExecute("Darkblood StormKing", () => DBSK.GetDSK(rankUpClass));
        CheckAndExecute("Elemental Dracomancer", () => ED.GetED(rankUpClass));
        CheckAndExecute("Eternal Inversionist", () => EI.GetEI(rankUpClass));
        CheckAndExecute("Evolved Shaman", () => ES.GetES(rankUpClass));
        CheckAndExecute("Glacial Berserker", () => GB.GetGB(rankUpClass));
        CheckAndExecute("Horc Evader", () => HE.GetHE(rankUpClass));
        CheckAndExecute("Imperial Chunin", () => IC.GetIC(rankUpClass));
        CheckAndExecute("Lycan", () => Lycan.GetLycan(rankUpClass));
        CheckAndExecute("Master Ranger", () => MR.GetMR(rankUpClass));
        CheckAndExecute("Paladin", () => Pal.GetPaladin(rankUpClass));
        CheckAndExecute("Royal BattleMage", () => RBM.GetRBM(rankUpClass));
        CheckAndExecute("Shaman", () => Shaman.GetShaman(rankUpClass));
        CheckAndExecute("StoneCrusher", () => SC.GetSC(rankUpClass));
        CheckAndExecute("Thief of Hours", () => TOH.GetToH(rankUpClass));
        CheckAndExecute("Troll Spellsmith", () => TS.GetTS(rankUpClass));

        // Chaos Slayer variants
        CheckAndExecute("Chaos Slayer Mystic", () => CS.GetCS(CSvariant.Mystic, rankUpClass));
        CheckAndExecute("Chaos Slayer Berserker", () => CS.GetCS(CSvariant.Berserker, rankUpClass));
        CheckAndExecute("Chaos Slayer Cleric", () => CS.GetCS(CSvariant.Cleric, rankUpClass));
        CheckAndExecute("Chaos Slayer Thief", () => CS.GetCS(CSvariant.Thief, rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Arachnomancer", "Darkblood StormKing", "Elemental Dracomancer", "Eternal Inversionist", "Evolved Shaman", "Glacial Berserker", "Horc Evader", "Imperial Chunin", "Lycan", "Master Ranger", "Paladin", "Royal BattleMage", "Shaman", "StoneCrusher", "Thief of Hours", "Troll Spellsmith", "Chaos Slayer Mystic", "Chaos Slayer Berserker", "Chaos Slayer Cleric", "Chaos Slayer Thief" });


        Core.Logger("=== Reputation Classes - Completed! ===");
    }

    private void MemClasses(bool rankUpClass)
    {
        if (!Core.IsMember)
            return;

        Core.Logger("=== Doing Member Classes ===");

        Adv.GearStore();
        CheckAndExecute("Alpha Omega", () => AO.GetAlphaOmega(rankUpClass));
        CheckAndExecute("Acolyte", () => Acolyte.GetAcolyte(rankUpClass));
        CheckAndExecute("Bard", () => Bard.GetBard(rankUpClass));
        CheckAndExecute("BeastMaster", () => BM.GetBM(rankUpClass));
        CheckAndExecute("Blood Ancient", () => BA.GetBAnc(rankUpClass));
        CheckAndExecute("Blood Titan", () => BT.Getclass(rankUpClass));
        CheckAndExecute("Chrono Assassin", () => CA.GetChronoAss(rankUpClass));
        CheckAndExecute("DeathKnight", () => DK.GetDK(rankUpClass));
        CheckAndExecute("Death KnightLord", () => DKL.DKL());
        CheckAndExecute("DoomKnight", () => DoomK.GetDoomKnight(rankUpClass));
        CheckAndExecute("Drakel Warlord", () => DW.GetClass(rankUpClass));
        CheckAndExecute("Legion DoomKnight", () => LDK.GetLDK(rankUpClass));
        CheckAndExecute("Legendary Elemental Warrior", () => LEW.GetLEW(rankUpClass));
        CheckAndExecute("Renegade", () => Ren.Getclass(rankUpClass));
        CheckAndExecute("UndeadSlayer", () => US.GetUS(rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Alpha Omega", "Acolyte", "Bard", "BeastMaster", "Blood Ancient", "Blood Titan", "Chrono Assassin", "DeathKnight", "DoomKnight", "Drakel Warlord", "Legion DoomKnight", "Legendary Elemental Warrior", "Renegade", "UndeadSlayer" });

        Core.Logger("=== Member Classes - Completed! ===");
    }

    private void SeasonalClasses(bool rankUpClass)
    {
        Core.Logger("=== Doing Seasonal Classes ===");

        Adv.GearStore();
        CheckAndExecute("Alpha Pirate", () => APir.GetAlphaPirate(rankUpClass));
        CheckAndExecute("Dark Lord", () => DL.GetDL(rankUpClass));
        CheckAndExecute("Evolved Leprechaun", () => EL.GetClass(rankUpClass));
        CheckAndExecute("Exalted Harbinger", () => EH.GetEH(rankUpClass));
        CheckAndExecute("Frostval Barbarian", () => FB.GetFB(rankUpClass));
        CheckAndExecute("Legion SwordMaster Assassin", () => LSMA.GetClass(rankUpClass));
        CheckAndExecute("Northlands Monk", () => NM.GetNlMonk(rankUpClass));
        CheckAndExecute("Pirate", () => Pirate.GetPirate(rankUpClass));
        CheckAndExecute("Shadow Dragon Shinobi", () => SDS.GetClass(rankUpClass));
        CheckAndExecute("Pumpkin Lord", () => PL.GetClass(rankUpClass));
        CheckAndExecute("Vampire Lord", () => VL.GetClass(rankUpClass));
        // CheckAndExecute("No Hollowborn Class", () => NHBC.GetNHBC(rankUpClass));

        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Alpha Pirate", "Dark Lord", "Evolved Leprechaun", "Exalted Harbinger", "Frostval Barbarian", "Legion SwordMaster Assassin", "Northlands Monk", "Pirate", "Shadow Dragon Shinobi", "Pumpkin Lord", "Vampire Lord" });

        Core.Logger("=== Seasonal Classes - Completed! ===");
    }

    private void VariousClasses(bool rankUpClass)
    {
        Core.Logger("=== Doing Various Classes ===");

        Adv.GearStore();
        CheckAndExecute("Abyssal Angel Shadow", () => AAS.GetAbyssal(rankUpClass));
        CheckAndExecute("Archfiend", () => AF.GetArchfiend(rankUpClass));
        CheckAndExecute("Blood Sorceress", () => BS.GetBSorc(rankUpClass));
        CheckAndExecute("Doom Metal Necro", () => DMN.GetClass(rankUpClass));
        CheckAndExecute("Dragonslayer", () => DS.GetDragonslayer(rankUpClass));
        CheckAndExecute("Dragonslayer General", () => DSG.GetDSGeneral(rankUpClass));
        CheckAndExecute("DragonSoul Shinobi", () => DSS.GetDSS(rankUpClass));
        CheckAndExecute("Enforcer", () => Enf.GetClass(rankUpClass));
        CheckAndExecute("Frost SpititReaver", () => FSR.GetFSR(rankUpClass));
        CheckAndExecute("HighSeas Commander", () => HSC.GetHSC(rankUpClass));
        CheckAndExecute("Infinite Legion Dark Caster", () => ILDC.GetILDC(rankUpClass));
        CheckAndExecute("Martial Artist", () => MA.GetMartialArtist(rankUpClass));
        CheckAndExecute("MechaJouster", () => MJ.GetMJ(rankUpClass));
        CheckAndExecute("Necromancer", () => Necro.GetNecromancer(rankUpClass));
        CheckAndExecute("Neo Metal Necro", () => NMN.GetClass(rankUpClass));
        CheckAndExecute("ProtoSartorium", () => PS.GetPS(rankUpClass));
        CheckAndExecute("Rustbucket", () => RB.GetRustbucket(rankUpClass));
        CheckAndExecute("Scarlet Sorceress", () => SS.GetSSorc(rankUpClass));
        CheckAndExecute("SwordMaster", () => SM.GetSwordMaster(rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Abyssal Angel Shadow", "Archfiend", "Blood Sorceress", "Doom Metal Necro", "Dragonslayer", "Dragonslayer General", "DragonSoul Shinobi", "Enforcer", "Frost SpititReaver", "HighSeas Commander", "Infinite Legion Dark Caster", "MechaJouster", "Necromancer", "Neo Metal Necro", "ProtoSartorium", "Rustbucket", "Scarlet Sorceress", "SwordMaster" });

        Core.Logger("=== Various Classes - Completed! ===");
    }

    private void EndGameClasses(bool rankUpClass)
    {
        Core.Logger("=== Doing End Game Classes ===");

        Adv.GearStore();
        CheckAndExecute("ArchPaladin", () => AP.GetAP(rankUpClass));
        CheckAndExecute("Dragon of Time", () => DOT.GetDoT(rankUpClass, doExtra: false));
        CheckAndExecute("Void Highlord", () => VHL.GetVHL(rankUpClass));
        CheckAndExecute("Yami no Ronin", () => YNR.GetYnR(rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "ArchPaladin", "Dragon of Time", "Void Highlord", "Yami no Ronin" });

        Core.Logger("=== End Game Classes - Completed! ===");
    }

    private void ACorToHardtoGetClasses(bool rankUpClass)
    {
        Core.Logger("=== AC / Special Requirement / Army Classes ===");

        Adv.GearStore();
        // Why do you own these classes?
        CheckAndExecute("Grim Necromancer", () => GN.GetGN(rankUpClass)); // 600k ac purchased

        // Classes that require a certain time played:
        CheckAndExecute("SkyCharged Grenadier", () => SCG.GetSCG(rankUpClass)); // 9 years membership
        CheckAndExecute("Sentinel", () => Sentinel.GetSentinel(rankUpClass)); // 16 years played

        // Classes that Cost ACs / AC badges:
        CheckAndExecute("LightCaster", () => LC.GetLC(rankUpClass)); // LC gets LM at the same time
        CheckAndExecute("Legion Revenant", () => LR.GetLR(rankUpClass));
        CheckAndExecute("Exalted Soul Cleaver", () => ESC.GetClass(rankUpClass));

        // Classes that require an army or are just to damn hard to solo,
        // these scripts will more then likely just return when they cant farm an item:
        // CheckAndExecute("Chaos Avenger", () => CAV.GetClass(rankUpClass));
        // CheckAndExecute("Archmage", () => AM.GetAM(rankUpClass));
        // CheckAndExecute("Verus DoomKnight", () => VDK.GetClass(rankUpClass));

        // Classes that take to long to farm for a bank class:
        // CheckAndExecute("Arcana Invoker", () => AI.GetAI(rankUpClass));
        CheckAndExecute("Hollowborn Vindicator", () => HBV.GetClass(rankUpClass)); // Non Insignia
        CheckAndExecute("King's Echo", () => KE.GetKE(rankUpClass));
        // CheckAndExecute("Lich", () => lich.Example(rankUpClass));
        CheckAndExecute("ShadowScythe General", Daily.ShadowScytheClass);
        // CheckAndExecute("Sovereign of Storms", () => SOS.GetSOS(rankUpClass));
        Adv.GearStore(true, true);
        Core.ToBank(new[] { "Grim Necromancer", "SkyCharged Grenadier", "Sentinel", "LightCaster", "Legion Revenant", "Exalted Soul Cleaver", "Chaos Avenger", "Archmage", "Verus DoomKnight", "Arcana Invoker", "Hollowborn Vindicator", "Lich", "ShadowScythe General", "Sovereign of Storms" });

        Core.Logger("=== AC / Special Requirement / Army Classes - Completed! ===");
    }


    bool IsitRank10(ItemBase item) => item != null && item.Quantity == 302500;

    void CheckAndExecute(string className, Action action)
    {
        // Find the item in both inventory and bank.
        ItemBase? Class = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(x => x.Name.ToLower() == className.ToLower());

        bool ItemOwned = Bot.Inventory.Items.Concat(Bot.Bank.Items).Contains(Class);

        // Check if the item is found and if it meets the quantity requirement.
        if (!ItemOwned || (Class != null && !IsitRank10(Class)))
        {
            action();
        }
    }


}
