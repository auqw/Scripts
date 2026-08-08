/*
name: JoePrepsForUltras
description: This script will farm all the required items for Ultra Bosses
tags: joe, ultra, boss, preparation, farm
*/

#region includes
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Dailies/0AllDailies.cs
//cs_include Scripts/Dailies/Cryomancer.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Dailies/MineCrafting.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Enhancement/InventoryEnhancer.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/AriaGreenhouse.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Nation/AFDL/EnoughDOOMforanArchfiend.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Good/GearOfAwe/CapeOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnFortressMerge.cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowrealmMerge.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Other/Classes/BloodSorceress.cs
//cs_include Scripts/Other/Classes/Daily-Classes/BlazeBinder.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Other/Classes/DragonShinobi.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/Classes/REP-based/DarkbloodStormKing.cs
//cs_include Scripts/Other/Classes/REP-based/EternalInversionist.cs
//cs_include Scripts/Other/Classes/REP-based/GlacialBerserker.cs
//cs_include Scripts/Other/Classes/REP-based/MasterRanger.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Other/Classes/ScarletSorceress.cs
//cs_include Scripts/Other/FreeBoosts/FreeBoostsQuest(10mns)[Rng].cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
//cs_include Scripts/Other/MergeShops/SynderesMerge.cs
//cs_include Scripts/Other/MergeShops/YulgarsUndineMerge.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/Various/Potions.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Other/Weapons/DualChainSawKatanas.cs
//cs_include Scripts/Other/Weapons/EnchantedVictoryBladeWeapons.cs
//cs_include Scripts/Other/Weapons/ExaltedApotheosisPreReqs.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Seasonal/Friday13th/Story/CoreFriday13th.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/Story/0AllStories.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
//cs_include Scripts/Story/FireIsland/CoreFireIsland.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs
//cs_include Scripts/Story/Legion/AtlasFalls.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/DageTheEvilIsland/CoreDageTheEvilIsland.cs
//cs_include Scripts/Story/Legion/DageChallengeStory.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/Ravenscar.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/WorldSoul.cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/DeadlyDungeon[Mem].cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/KillerCatacombs[Mem].cs
//cs_include Scripts/Story/LordsofChaos/ChoasFinaleBonus[Mem]/PyramidofPain[Mem].cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Story/MemetsRealm/CoreMemet.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/FiendPast.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/ShadowBlastArena.cs
//cs_include Scripts/Story/Nation/tercesarchive.cs
//cs_include Scripts/Story/Nation/Tercessuinotlim.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
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
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Tools/BankAllItems.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/Adam1a1Quests.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/DjinnGuard.cs
//cs_include Scripts/Story/AranxQuests.cs
//cs_include Scripts/Story/Arcangrove.cs
//cs_include Scripts/Story/AriaPet[MEM].cs
//cs_include Scripts/Story/Artixpointe.cs
//cs_include Scripts/Story/ArtixWedding.cs
//cs_include Scripts/Story/Asylum.cs
//cs_include Scripts/Story/Banished.cs
//cs_include Scripts/Story/BeleensDream.cs
//cs_include Scripts/Story/BloodMoon.cs
//cs_include Scripts/Story/Bludrut.cs
//cs_include Scripts/Story/BoneBreak.cs
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
//cs_include Scripts/Story/DoomVault.cs
//cs_include Scripts/Story/DoomVaultB.cs
//cs_include Scripts/Story/Downward.cs
//cs_include Scripts/Story/DracoCon.cs
//cs_include Scripts/Story/DragonRoad[Upholder].cs
//cs_include Scripts/Story/DreamPalace.cs
//cs_include Scripts/Story/Shinkansen.cs
//cs_include Scripts/Story/LaeWedding.cs
//cs_include Scripts/Story/Lightguard[MEM].cs
//cs_include Scripts/Story/LightoviaCave.cs
//cs_include Scripts/Story/LostVilla.cs
//cs_include Scripts/Story/Manor.cs
//cs_include Scripts/Story/Marsh2[MEM].cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Story/Mobius.cs
//cs_include Scripts/Story/ShadowVoid.cs
//cs_include Scripts/Story/NecroProject.cs
//cs_include Scripts/Story/Noobshire.cs
//cs_include Scripts/Story/Nukemichi[mem].cs
//cs_include Scripts/Story/NytheraSaga.cs
//cs_include Scripts/Story/Pirates[Member].cs
//cs_include Scripts/Story/PoisonForest.cs
//cs_include Scripts/Story/QueenReign.cs
//cs_include Scripts/Story/QuibbleHunt.cs
//cs_include Scripts/Story/Dwarfhold.cs
//cs_include Scripts/Story/DwarvesVsGiants.cs
//cs_include Scripts/Story/Eden.cs
//cs_include Scripts/Story/EtherstormWastes.cs
//cs_include Scripts/Story/ExaltiaTower.cs
//cs_include Scripts/Story/Extinction.cs
//cs_include Scripts/Story/FableForest.cs
//cs_include Scripts/Story/FrozenNorthlands.cs
//cs_include Scripts/Story/HuntersMoon.cs
//cs_include Scripts/Story/GameHaven.cs
//cs_include Scripts/Story/GiantTaleStory.cs
//cs_include Scripts/Story/Guru.cs
//cs_include Scripts/Story/IcePlane.cs
//cs_include Scripts/Story/RavenlossSaga.cs
//cs_include Scripts/Story/River.cs
//cs_include Scripts/Story/Safiria[Member].cs
//cs_include Scripts/Story/ShadowGates.cs
//cs_include Scripts/Story/ShadowSlayerK.cs
//cs_include Scripts/Story/ShadowVault.cs
//cs_include Scripts/Story/Shattersword.cs
//cs_include Scripts/Story/SuperDeath.cs
//cs_include Scripts/Story/Tower[mem].cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/TitanAttack.cs
//cs_include Scripts/Story/Tournament.cs
//cs_include Scripts/Story/MustyCave.cs
//cs_include Scripts/Story/Tutorial.cs
//cs_include Scripts/Story/ShipWreck.cs
//cs_include Scripts/Story/SkyGuardSaga.cs
//cs_include Scripts/Story/SpirePast.cs
//cs_include Scripts/Story/Ubear.cs
//cs_include Scripts/Story/VasalkarLairWar.cs
//cs_include Scripts/Story/UnderGroundLab.cs
//cs_include Scripts/Story/WatchTower.cs

//cs_include Scripts/Story/Oasis/CoreOasis.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/WillowCreek.cs
//cs_include Scripts/Story/BaseCamp.cs
//cs_include Scripts/Other/MergeShops/TerminaTempleMerge.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DoomPirateStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/DoomPirateHaulMerge.cs
//cs_include Scripts/Other/Classes/VerusDoomKnight.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
//cs_include Scripts/Other/Classes/KingsEcho.cs

#endregion includes

using System;
using System.Collections.Generic;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class JoePrepsForUltras
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
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
    private static UnlockForgeEnhancements UnlockForgeEnhancements
    {
        get => _UnlockForgeEnhancements ??= new UnlockForgeEnhancements();
        set => _UnlockForgeEnhancements = value;
    }
    private static UnlockForgeEnhancements _UnlockForgeEnhancements;
    private static ArchPaladin AP
    {
        get => _AP ??= new ArchPaladin();
        set => _AP = value;
    }
    private static ArchPaladin _AP;
    private static StoneCrusher SC
    {
        get => _SC ??= new StoneCrusher();
        set => _SC = value;
    }
    private static StoneCrusher _SC;
    private static LordOfOrder LOO
    {
        get => _LOO ??= new LordOfOrder();
        set => _LOO = value;
    }
    private static LordOfOrder _LOO;
    private static PotionBuyer PotionBuyer
    {
        get => _PotionBuyer ??= new PotionBuyer();
        set => _PotionBuyer = value;
    }
    private static PotionBuyer _PotionBuyer;
    private static BuyScrolls Scroll
    {
        get => _Scroll ??= new BuyScrolls();
        set => _Scroll = value;
    }
    private static BuyScrolls _Scroll;
    private static AllStories AllStories
    {
        get => _AllStories ??= new AllStories();
        set => _AllStories = value;
    }
    private static AllStories _AllStories;
    private static CoreNSOD NSOD
    {
        get => _NSOD ??= new CoreNSOD();
        set => _NSOD = value;
    }
    private static CoreNSOD _NSOD;
    private static CoreLR LR
    {
        get => _LR ??= new CoreLR();
        set => _LR = value;
    }
    private static CoreLR _LR;
    private static CoreYnR YnR
    {
        get => _YnR ??= new CoreYnR();
        set => _YnR = value;
    }
    private static CoreYnR _YnR;
    private static VerusDoomKnightClass VDK
    {
        get => _VDK ??= new VerusDoomKnightClass();
        set => _VDK = value;
    }
    private static VerusDoomKnightClass _VDK;

    public enum PlayerNumber
    {
        Player1,
        Player2,
        Player3,
        Player4,
    }

    public string OptionsStorage = "JoePrepsForUltras";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("GetClasses", "Get Classes", "Enable to Skip aquiring classes for each player -- and still do the rest of the prep ( faster for those that already own the classes)", false),
        new Option<PlayerNumber>(
            "Player",
            "Player Number",
            "Which Player number are you in Insert's \"ALL ULTRAS\" bot (for grim) setup?\n"
                + "Player1: Tank/Support (Legion Revenant, ArchPaladin, StoneCrusher, Chaos Avenger)\n"
                + "Player2: DPS/Chrono (Quantum Chronomancer, Legion Revenant, Chaos Avenger, StoneCrusher)\n"
                + "Player3: Support (Lord of Order, Legion Revenant)\n"
                + "Player4: DPS/Support (ArchPaladin, LightCaster, Verus DoomKnight)",
            PlayerNumber.Player1
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        PlayerSetup();

        Core.SetOptions(false);
    }

    public void PlayerSetup()
    {
        if (!Bot.Config!.Get<bool>("GetClasses"))
        {
            Bot.Log("Class Aquisition disabled, skipping this part.");
            OtherPrep();
            return;
        }

        PlayerNumber playerNumber = Bot.Config!.Get<PlayerNumber>("Player");
        Core.Logger($"Setting up for {playerNumber}");

        switch (playerNumber)
        {
            case PlayerNumber.Player1:
                SetupPlayerOne();
                break;
            case PlayerNumber.Player2:
                SetupPlayerTwo();
                break;
            case PlayerNumber.Player3:
                SetupPlayerThree();
                break;
            case PlayerNumber.Player4:
                SetupPlayerFour();
                break;
            default:
                Core.Logger(
                    "Invalid player number selected. Please choose Player1, Player2, Player3, or Player4."
                );
                break;
        }

        // Common preparations for all players
        OtherPrep();
    }

    private void SetupPlayerOne()
    {
        Core.Logger("Setting up Player 1 (Tank/Support)");

        // Legion Revenant
        Core.Logger("Getting Legion Revenant...");
        LR.GetLR(true);

        // ArchPaladin
        Core.Logger("Getting ArchPaladin...");
        AP.GetAP(true);

        // StoneCrusher
        Core.Logger("Getting StoneCrusher...");
        SC.GetSC(true);

        // ChaosAvenger (Ultra requirement)
        if (!Core.CheckInventory("Chaos Avenger"))
            Core.Logger(
                "Note: Chaos Avenger must be obtained manually as it requires ultras, and Skua does not support this"
            );
        else
            Core.Logger("Chaos Avenger found!");
    }

    private void SetupPlayerTwo()
    {
        Core.Logger("Setting up Player 2 (DPS/Chrono)");

        // Quantum Chronomancer
        if (!Core.CheckInventory("Quantum Chronomancer"))
            Core.Logger(
                "Note: Quantum Chronomancer must be bought for 6k ACs in /heromart or use Arachnomancer instead"
            );
        else
            Core.Logger("Quantum Chronomancer found!");

        // Legion Revenant
        Core.Logger("Getting Legion Revenant...");
        LR.GetLR(true);

        // ChaosAvenger
        if (!Core.CheckInventory("Chaos Avenger"))
            Core.Logger(
                "Note: Chaos Avenger must be obtained manually as it requires ultras, and Skua does not support this"
            );
        else
            Core.Logger("Chaos Avenger found!");

        // StoneCrusher
        Core.Logger("Getting StoneCrusher...");
        SC.GetSC(true);
    }

    private void SetupPlayerThree()
    {
        Core.Logger("Setting up Player 3 (Support)");

        // Lord of Order
        Core.Logger("Getting Lord of Order...");
        LOO.GetLoO(true);

        // Legion Revenant
        Core.Logger("Getting Legion Revenant...");
        LR.GetLR(true);
    }

    private void SetupPlayerFour()
    {
        Core.Logger("Setting up Player 4 (DPS/Support)");

        // ArchPaladin
        Core.Logger("Getting ArchPaladin...");
        AP.GetAP(true);

        // LightCaster
        if (!Core.CheckInventory("LightCaster"))
            Core.Logger(
                "Note: LightCaster requires 1000 ACs to purchase. Pre-farming materials..."
            );
        else
            Core.Logger("LightCaster found!");

        // Verus DoomKnight
        if (!Core.CheckInventory("Verus DoomKnight"))
        {
            Core.Logger("Getting Verus DoomKnight...");
            VDK.GetClass();
        }
        else
            Core.Logger("Verus DoomKnight found!");
    }

    public void OtherPrep()
    {
        Core.Logger("Performing general preparations for all players...");

        // Level and gold requirements
        Farm.Experience();
        Farm.Gold();
        Farm.BladeofAweREP();

        // Check and unlock required enhancements
        CheckAndUnlockEnhancements();

        // Buy potions and scrolls
        Core.Logger("Getting potions and scrolls...");
        PotionBuyer.INeedYourStrongestPotions(
            new[] { "Potent Malevolence Elixir" },
            new[] { true },
            10,
            true,
            true
        );
        Scroll.BuyScroll(Scrolls.Enrage, 1000);

        if (!Core.CheckInventory("Scroll of Life Steal", 99))
            Adv.BuyItem(
                "terminatemple",
                2328,
                "Scroll of Life Steal",
                99 - Bot.Inventory.GetQuantity("Scroll of Life Steal")
            );

        // Story completion
        Core.Logger("Completing *ALL* storylines...");
        AllStories.CompleteAll();

        // Endgame weapon - Necrotic Sword of Doom (51% damage to all monsters)
        if (!Core.CheckInventory("Necrotic Sword of Doom"))
        {
            Core.Logger("Starting Necrotic Sword of Doom farm (51% damage to all monsters)");
            NSOD.GetNSOD();

            if (Core.CheckInventory("Necrotic Sword of Doom"))
                Core.Logger("Successfully obtained Necrotic Sword of Doom!");
            else
                Core.Logger(
                    "NSOD farm incomplete - consider continuing manually or using a dedicated script"
                );
        }
        else
            Core.Logger("Necrotic Sword of Doom already obtained!");
    }

    /// <summary>
    /// Check and unlock enhancements required for the player's role
    /// </summary>
    private void CheckAndUnlockEnhancements()
    {
        // Player-specific enhancements
        Dictionary<string, string[]> enhancementsByPlayer = new()
        {
            {
                "Player1",
                new[]
                {
                    "ArcanasConcerto",
                    "Ravenous",
                    "Lacerate",
                    "Dauntless",
                    "Vainglory",
                    "Avarice",
                    "Absolution",
                    "Lament",
                    "ForgeHelm",
                }
            },
            {
                "Player2",
                new[]
                {
                    "Valiance",
                    "ArcanasConcerto",
                    "Dauntless",
                    "Lament",
                    "Vainglory",
                    "Penitence",
                    "Absolution",
                }
            },
            {
                "Player3",
                new[]
                {
                    "Health Vamp/AweBlast",
                    "Dauntless",
                    "Penitence",
                    "Avarice",
                    "Absolution",
                    "ForgeHelm",
                }
            },
            {
                "Player4",
                new[]
                {
                    "Ravenous",
                    "Elysium",
                    "ArcanasConcerto",
                    "Dauntless",
                    "Valiance",
                    "Penitence",
                    "Lament",
                    "ForgeHelm",
                }
            },
        };

        // Enhancement to Quest ID mapping
        Dictionary<string, int> enhancementToQuestID = new()
        {
            { "Health Vamp/AweBlast", 2937 },
            { "ForgeWeapon", 8738 },
            { "Lacerate", 8739 },
            { "Valiance", 8741 },
            { "ArcanasConcerto", 8742 },
            { "Absolution", 8743 },
            { "Vainglory", 8744 },
            { "Avarice", 8745 },
            { "Elysium", 8821 },
            { "Penitence", 8822 },
            { "Lament", 8823 },
            { "ForgeHelm", 8828 },
            { "Dauntless", 9172 },
            { "Ravenous", 9560 },
        };

        // Enhancement to unlock method mapping
        Dictionary<string, Action> enhancementActions = new()
        {
            { "Health Vamp/AweBlast", () => Farm.BladeofAweREP() },
            { "Lacerate", () => UnlockForgeEnhancements.Lacerate() },
            { "Valiance", () => UnlockForgeEnhancements.HerosValiance() },
            { "ArcanasConcerto", () => UnlockForgeEnhancements.ArcanasConcerto() },
            { "Absolution", () => UnlockForgeEnhancements.Absolution() },
            { "Vainglory", () => UnlockForgeEnhancements.Vainglory() },
            { "Avarice", () => UnlockForgeEnhancements.Avarice() },
            { "Elysium", () => UnlockForgeEnhancements.Elysium() },
            { "Penitence", () => UnlockForgeEnhancements.Penitence() },
            { "Lament", () => UnlockForgeEnhancements.Lament() },
            { "ForgeHelm", () => UnlockForgeEnhancements.ForgeHelmEnhancement() },
            { "Dauntless", () => UnlockForgeEnhancements.DauntLess() },
            { "Ravenous", () => UnlockForgeEnhancements.Ravenous() },
        };

        // Get current player
        PlayerNumber playerNumber = Bot.Config!.Get<PlayerNumber>("Player");
        string playerKey = Enum.GetName(typeof(PlayerNumber), playerNumber) ?? "Player1";

        Core.Logger($"Checking enhancements for {playerKey}:");

        // Get player-specific enhancements
        string[] playerEnhancements = enhancementsByPlayer[playerKey];
        List<string> missingEnhancements = new();

        // Check status and collect missing enhancements
        foreach (string enhancement in playerEnhancements)
        {
            if (enhancementToQuestID.TryGetValue(enhancement, out int questID))
            {
                bool isCompleted = Core.isCompletedBefore(questID);
                string completionStatus = isCompleted ? "✅" : "❌";
                Core.Logger($"{enhancement} - {completionStatus}");

                if (!isCompleted)
                    missingEnhancements.Add(enhancement);
            }
            else
                Core.Logger($"Enhancement {enhancement} not found in reference list");
        }

        // Unlock missing enhancements
        if (missingEnhancements.Count > 0)
        {
            Core.Logger("Unlocking missing enhancements...");
            foreach (var enhancement in missingEnhancements)
            {
                if (enhancementActions.TryGetValue(enhancement, out var action))
                {
                    Core.Logger($"Unlocking: {enhancement}");
                    action.Invoke();
                }
                else
                    Core.Logger($"Unhandled enhancement: {enhancement}");
            }
        }
        else
            Core.Logger("No missing enhancements for your player role!");
    }
}
