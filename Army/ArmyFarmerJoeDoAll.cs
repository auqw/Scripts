/*
name: Army FarmerJoe DoAll
description: Synchronized 4 to 7 account FarmerJoe progression from Level 1 to Level 100 Endgame.
tags: army, farmerjoe, progression, doall, dailies, DoT, AF, YnR, VDK, VHL, Forge Enhancements, AM, AI, AWESCENDED, HOLLOWBORN REAPER'S SCYTHE
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyFJ.cs
//cs_include Scripts/Story/Tutorial.cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Other/FreeBoosts/FreeBoostsQuest(10mns)[Rng].cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Other/Classes/VerusDoomKnight.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Other/Classes/ArchMage/CoreArchMage.cs
//cs_include Scripts/Other/Classes/ArcanaInvoker[Non-Insignia].cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowRealmMerge.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Other/Classes/KathoolAdept.cs
//cs_include Scripts/Other/Classes/ScionofFlames.cs
//cs_include Scripts/Tools/BankAllItems.cs
//cs_include Scripts/Other/Weapons/ChaorrupterUnlocked.cs
//cs_include Scripts/Other/MergeShops/RemnantsofachampionMerge.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmyFarmerJoeDoAll
{
    private static readonly CoreArmyFJ Army = new();
    private static readonly CoreAdvanced Adv = new();
    private static readonly CoreFarms Farm = new();
    private static readonly CoreStory Story = new();
    private static readonly CoreDailies Daily = new();
    private static readonly Tutorial Tutorial = new();
    private static readonly Mazumi Mazumi = new();
    private static readonly Lair Lair = new();
    private static readonly FreeBoosts Boosts = new();
    private static readonly BurningBladeOfAbezeth BBoA = new();
    private static readonly DragonOfTime DoT = new();
    private static readonly ArchFiend AF = new();
    private static readonly CoreYnR YnR = new();
    private static readonly VerusDoomKnightClass VDK = new();
    private static readonly UnlockForgeEnhancements ForgeUnlock = new();
    private static readonly CoreVHL VHLCore = new();
    private static readonly CoreArchMage AMCore = new();
    private static readonly ArcanaInvoker AICore = new();
    private static readonly ShadowrealmMerge SRM = new();
    private static readonly Awescended AwescendedCore = new();
    private static readonly CelestialPast CelestialPastCore = new();
    private static readonly KathoolAdept KACore = new();
    private static readonly ScionofFlames SOFCore = new();
    private static readonly BankAllItems BankAllCore = new();
    private static readonly ChaorrupterUnlocked ChaorrupterCore = new();
    private static readonly RemnantsofachampionMerge ROACCore = new();

    private const int Rank10ClassPoints = 302500;

    public enum PetChoice
    {
        None,
        HotMama,
        Akriloth,
    }

    public enum BoostGearPolicy
    {
        UpgradeIfBetter,
        KeepEquippedGear,
    }

    private static IScriptInterface Bot => IScriptInterface.Instance;

    public string OptionsStorage = "ArmyFarmerJoeDoAll";
    public bool DontPreconfigure = true;
    private const string SyncFileName = "ArmyFarmerJoeDoAll.sync";
    public List<IOption> Options = new()
    {
        Army.player1,
        Army.player2,
        Army.player3,
        Army.player4,
        Army.player5,
        Army.player6,
        Army.player7,
        new Option<string>(
            "RoomNumber",
            "Army Private Room Number",
            "Required. Shared private room number (1001-99999) so all Army accounts join the exact same map instance. Do not leave as 00000.",
            "00000"
        ),
        new Option<bool>(
            "OutFit",
            "Get a Pre-Made Outfit, Curtious of the Community",
            "We are farmers, bum ba dum bum bum bum bum",
            false
        ),
        new Option<bool>(
            "EquipOutfit",
            "Equip our the FarmerJoe outfit at the end?",
            "Yay or Nay",
            false
        ),
        new Option<bool>(
            "SellStarterClasses",
            "SellStarterClasses",
            "Yay or Nay",
            false
        ),
        new Option<bool>(
            "GetBoosts",
            "GetBoosts",
            "Do the \"Free Boosts\" quests to get some 10minute boosts (the droprate is < 1% so itll take awhile)",
            false
        ),
        new Option<PetChoice>(
            "PetChoice",
            "Choose Your Pet",
            "Extra stuff to choose",
            PetChoice.None
        ),
        new Option<BoostGearPolicy>(
            "BoostGearPolicy",
            "Boost Gear Policy",
            "UpgradeIfBetter: equips higher-boost Weapon/Armor/Cape/Helm (never downgrades superior equipped items, respects AQW Show/Wearing, treats EXP weight as 0 at Lv 100).\n"
                + "KeepEquippedGear: keeps each player's already-equipped Weapon/Armor/Cape/Helm untouched during Army progression.",
            BoostGearPolicy.UpgradeIfBetter
        ),
        new Option<string>(
            "KeepEquippedGearPlayers",
            "Keep Equipped Gear Players",
            "Comma-separated Army player numbers (e.g. \"1\") or exact usernames that should always keep their currently equipped Weapon/Armor/Cape/Helm (e.g. a full endgame main).\n"
                + "Per-account Skua CBO 'DisableBestGear' is also respected automatically.",
            string.Empty
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        CoreBots core = CoreBots.Instance;
        core.SetOptions();
        try
        {
            if (!StartArmySync(SyncFileName))
                return;

            if (!RunStartupBankAll())
                return;

            if (!RunStartupUnlockedDailies())
                return;

            if (!RunLevel1To30())
                return;

            if (!RunBracket30())
                return;

            if (!RunBracket50())
                return;

            if (!RunBracket55())
                return;

            if (!RunBracket60())
                return;

            if (!RunBracket65())
                return;

            if (!RunBracket70())
                return;

            if (!RunStep3A())
                return;

            if (!RunStep3B())
                return;

            if (!RunStep3C())
                return;

            if (!RunStep4A())
                return;

            if (!RunSellStarterClasses())
                return;

            if (!RunFarmerJoeOutfit())
                return;

            if (IsPlayer(1))
            {
                if (StopArmySync("STEP4A_COMPLETE"))
                    core.Logger("Army FarmerJoe: player 1 published STEP4A_COMPLETE.");
                return;
            }

            WaitForArmyStop();
        }
        finally
        {
            core.SetOptions(false);
        }
    }

    public bool StartArmySync(string syncFileName)
    {
        if (Bot.Config == null)
        {
            CoreBots.Instance.Logger(
                "Army player configuration is unavailable.",
                "ArmyFarmerJoeDoAll"
            );
            return false;
        }

        int playerCount = 0;
        bool foundEmptySlot = false;
        for (int playerNumber = 1; playerNumber <= 7; playerNumber++)
        {
            string playerName = Bot.Config.Get<string>($"player{playerNumber}");
            if (string.IsNullOrWhiteSpace(playerName))
            {
                foundEmptySlot = true;
                continue;
            }

            if (foundEmptySlot)
            {
                CoreBots.Instance.Logger(
                    "Army player slots must be filled consecutively starting at Player 1.",
                    "ArmyFarmerJoeDoAll"
                );
                return false;
            }

            playerCount++;
        }

        if (playerCount < 4)
        {
            CoreBots.Instance.Logger(
                "Army FarmerJoe requires at least 4 players. Players 5-7 are optional.",
                "ArmyFarmerJoeDoAll"
            );
            return false;
        }

        if (!ApplyArmyRoomSettings())
            return false;

        return Army.StartArmySync(syncFileName, playerCount);
    }

    private bool RunStartupBankAll()
    {
        CoreBots.Instance.Logger("Startup: Running BankAllItems (AC inventory & house items)...");
        BankAllCore.BankAll(inventory: true, house: true, bankNonAc: false, blackList: string.Empty);
        EnsureBestStaticBoostGearEquipped();
        SetClass();
        return Army.SyncArmy("STARTUP_BANK_ALL_DONE");
    }

    private bool RunEldersBloodDaily(string objectivePrefix = "BRACKET30")
    {
        CoreBots core = CoreBots.Instance;
        bool eldersBloodAvailable =
            Bot.Player.Level >= 30
            && !core.CheckInventory("Elders' Blood", 20, toInv: false)
            && CanRunPeriodicQuest(802);

        return RunCooperativeObjective(
            $"{objectivePrefix}_ELDERS_BLOOD",
            () =>
                !eldersBloodAvailable
                || Bot.Quests.IsDailyComplete(802)
                || core.CheckInventory("Elders' Blood", 20, toInv: false),
            () =>
            {
                if (
                    eldersBloodAvailable
                    && !Bot.Quests.IsDailyComplete(802)
                    && !core.CheckInventory("Elders' Blood", 20, toInv: false)
                )
                {
                    UnbankIfOwned("Elders' Blood");
                    SetClass(ClassType.Farm);
                    Daily.EldersBlood();
                }
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(
                    targetPlayerName,
                    "Gorillaphant",
                    compositionType: ClassType.Farm
                );
            },
            maxWorkAttempts: 2
        );
    }

    private bool _startupBracket30DailiesDone;
    private bool _startupBracket50DailiesDone;
    private bool _startupBracket55DailiesDone;
    private bool _startupEncroachingShadowsDone;
    private bool _startupDagesScrollDone;

    private bool RunStartupUnlockedDailies()
    {
        CoreBots core = CoreBots.Instance;
        core.Logger("Startup: Checking and running unlocked Dailies/Weeklies before progression...");

        if (!Army.SyncArmy("STARTUP_DAILIES_START"))
            return false;

        if (!RunEldersBloodDaily("STARTUP"))
            return false;

        if (!RunBracket30ApprovedDailiesAndWeeklies())
            return false;

        _startupBracket30DailiesDone = Army.AllPlayersSatisfy(
            "STARTUP_DAILIES_LV30_READY",
            () => Bot.Player.Level >= 30
        );

        if (
            Army.AllPlayersSatisfy(
                "STARTUP_DAILIES_LV50_READY",
                () => Bot.Player.Level >= 50 && core.isCompletedBefore(2084, log: false)
            )
        )
        {
            if (!RunBracket50ApprovedDailies())
                return false;
            _startupBracket50DailiesDone = true;
        }

        if (
            Army.AllPlayersSatisfy(
                "STARTUP_DAILIES_LV55_BK_READY",
                () => Bot.Player.Level >= 55 && core.isCompletedBefore(2084, log: false)
            )
        )
        {
            if (!RunBracket55ApprovedDailies())
                return false;
            _startupBracket55DailiesDone = true;
        }

        bool cryoDailyDone = false;
        if (
            !RunCooperativeObjective(
                "STARTUP_CRYO_DAILY",
                () =>
                    cryoDailyDone
                    || Bot.Player.Level < 55
                    || !core.isCompletedBefore(3937, log: false)
                    || core.CheckInventory("Cryomancer", toInv: false)
                    || core.CheckInventory("Glacera Ice Token", 84, toInv: false)
                    || Bot.Quests.IsDailyComplete(3966),
                () =>
                {
                    if (
                        cryoDailyDone
                        || Bot.Player.Level < 55
                        || !core.isCompletedBefore(3937, log: false)
                        || core.CheckInventory("Cryomancer", toInv: false)
                        || core.CheckInventory("Glacera Ice Token", 84, toInv: false)
                        || Bot.Quests.IsDailyComplete(3966)
                    )
                        return;

                    UnbankIfOwned("Glacera Ice Token");
                    SetClass(ClassType.Solo);
                    Daily.Cryomancer();
                    BankIfPresent("Glacera Ice Token");
                    cryoDailyDone = true;
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                },
                maxWorkAttempts: 2
            )
        )
            return false;

        if (
            Army.AllPlayersSatisfy(
                "STARTUP_DAILIES_LOO_READY",
                () => Bot.Player.Level >= 65 && core.isCompletedBefore(2084, log: false)
            )
            && !RunLordOfOrderProgression("STARTUP")
        )
            return false;

        if (
            Army.AllPlayersSatisfy("STARTUP_DAILIES_LV75_READY", () => Bot.Player.Level >= 75)
        )
        {
            if (!RunTheEncroachingShadowsDaily())
                return false;
            _startupEncroachingShadowsDone = true;
        }

        if (
            Army.AllPlayersSatisfy(
                "STARTUP_DAILIES_DAGE_READY",
                () => Bot.Player.Level >= 75 && core.isCompletedBefore(2084, log: false)
            )
        )
        {
            if (!RunDagesScrollFragmentDaily())
                return false;
            _startupDagesScrollDone = true;
        }

        return Army.SyncArmy("STARTUP_DAILIES_END");
    }

    private int _activeSubgroupMaxPlayers;

    private int GetLocalPlayerNumber()
    {
        if (Army.ArmyPlayerNumber > 0)
            return Army.ArmyPlayerNumber;

        for (int playerNumber = 1; playerNumber <= 7; playerNumber++)
        {
            if (IsPlayer(playerNumber))
                return playerNumber;
        }

        return 1;
    }

    private int GetSubgroupRoomIndex(int maxPlayersPerRoom)
    {
        if (maxPlayersPerRoom <= 0)
            return 0;

        return (GetLocalPlayerNumber() - 1) / maxPlayersPerRoom;
    }

    private bool ApplyRoomSettingsWithOffset(int roomOffset)
    {
        const int MinRoom = 1001;
        const int MaxRoom = 99999;

        string? rawRoom = Bot.Config?.Get<string>("RoomNumber")?.Trim();
        int roomNumber = int.TryParse(rawRoom, out int parsedRoom) ? parsedRoom : 0;

        if (!Army.ValidatePrivateRoomNumber(roomNumber))
            return false;

        int span = MaxRoom - MinRoom + 1;
        int normalizedOffset = ((roomOffset % span) + span) % span;
        int effectiveRoom = MinRoom + (((roomNumber - MinRoom) + normalizedOffset) % span);

        CoreBots core = CoreBots.Instance;
        core.PrivateRooms = true;
        core.PrivateRoomNumber = effectiveRoom;
        core.PublicDifficult = false;
        return true;
    }

    private bool ApplyArmyRoomSettings()
    {
        _activeSubgroupMaxPlayers = 0;
        return ApplyRoomSettingsWithOffset(0);
    }

    private bool ApplySubgroupRoomSettings(int maxPlayersPerRoom)
    {
        _activeSubgroupMaxPlayers = Math.Max(0, maxPlayersPerRoom);
        return ApplyCurrentArmyRoomSettings();
    }

    private bool ApplyCurrentArmyRoomSettings() =>
        ApplyRoomSettingsWithOffset(GetSubgroupRoomIndex(_activeSubgroupMaxPlayers));

    public bool RunLevel1To30()
    {
        if (!Army.SyncArmy("STEP1_START"))
            return false;

        ApplyArmyRoomSettings();
        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP1_BEGINNER",
                () =>
                    Bot.Player.Level >= 25
                    && HasClassInInventory("Oracle")
                    && HasRank10Class("Oracle")
                    && HasClassInInventory("Ninja")
                    && HasRank10Class("Ninja")
                    && HasClassInInventory("Mage")
                    && HasRank10Class("Mage")
            )
        )
        {
            CoreBots.Instance.Logger("Army FarmerJoe: Step 1 (Level 1-30) already completed on all accounts — skipping.");
            return Army.SyncArmy("STEP1_COMPLETE");
        }

        CoreBots.Instance.Logger("Army FarmerJoe: starting level 1-30 on all accounts.");
        if (!RunBeginnerItems())
            return false;

        if (IScriptInterface.Instance.ShouldExit)
            return false;

        CoreBots.Instance.Logger(
            "Class points may be desynced at Rank 9, if you are stuck at rank 9, please stop the bot & relog if this happens"
        );
        return Army.SyncArmy("STEP1_COMPLETE");
    }

    private bool RunBeginnerItems()
    {
        if (
            !RunCooperativeObjective(
                "BEGINNER_INITIAL_CHECK",
                () => true,
                () => { },
                (targetPlayerNumber, targetPlayerName) => { }
            )
        )
            return false;

        CoreBots.Instance.Logger(
            "Running Step 1 (Level 1-30) with strict per-step verification on all accounts."
        );

        // 1. Tutorial badges
        bool tutorialComplete = false;
        if (
            !RunCooperativeObjective(
                "BEGINNER_TUTORIAL",
                () => tutorialComplete,
                () =>
                {
                    Tutorial.Badges();
                    tutorialComplete = !Bot.ShouldExit;
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    // Tutorial badges are non-combat; helpers just wait.
                    Bot.Sleep(1000);
                }
            )
        )
            return false;

        // 2. Basic equipment
        bool basicEquipmentComplete = false;
        Action equipBasicItems = () =>
        {
            LeaveCombatRoomForEquipmentChange();
            foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
            {
                InventoryItem? equippedItem = Bot.Inventory.Items.Find(item =>
                    item.Equipped && item.Category == category
                );

                switch (category)
                {
                    case ItemCategory.Helm:
                        if (equippedItem == null && !ShouldPreserveEquippedGear())
                        {
                            CoreBots.Instance.BuyItem("classhalla", 299, "Battle Oracle Hood");
                            Bot.Wait.ForItemBuy();
                            CoreBots.Instance.Equip("Battle Oracle Hood");
                        }
                        break;

                    case ItemCategory.Cape:
                        if (equippedItem == null && !ShouldPreserveEquippedGear())
                        {
                            CoreBots.Instance.BuyItem("classhalla", 299, "Battle Oracle Wings");
                            Bot.Wait.ForItemBuy();
                            CoreBots.Instance.Equip("Battle Oracle Wings");
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
                        InventoryItem? equippedWeapon = Bot.Inventory.Items.Find(item =>
                            item.Equipped && string.Equals(item.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase)
                        );
                        ItemBase? defaultWeapon = Bot.Inventory.Items.Find(item =>
                            item.Name.StartsWith("Default")
                        );

                        if (
                            !ShouldPreserveEquippedGear()
                            && (equippedWeapon == null || equippedWeapon.Name.StartsWith("Default"))
                            && defaultWeapon != null
                            && CoreBots.Instance.CheckInventory(defaultWeapon.ID)
                        )
                        {
                            CoreBots.Instance.BuyItem(
                                "classhalla",
                                299,
                                "Battle Oracle Battlestaff"
                            );
                            Bot.Wait.ForItemBuy();
                            CoreBots.Instance.Equip("Battle Oracle Battlestaff");
                            Bot.Shops.SellItem(defaultWeapon.ID);
                            Bot.Wait.ForTrue(
                                () => !Bot.Inventory.Contains(defaultWeapon.ID),
                                20
                            );
                        }
                        break;
                }
            }

            EnsureBestStaticBoostGearEquipped();
            basicEquipmentComplete = !Bot.ShouldExit;
        };

        if (
            !RunCooperativeObjective(
                "BEGINNER_BASIC_EQUIPMENT",
                () => basicEquipmentComplete,
                equipBasicItems,
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 2b. Oracle (free 0 AC Healer/Support class in /classhalla shop 299 — bought early so Player 2 can heal from Level 1!)
        bool HasOracle() => HasClassInInventory("Oracle");
        bool HasOracleRank10() => HasOracle() && HasRank10Class("Oracle");

        if (
            !RunCooperativeObjective(
                "BEGINNER_ORACLE_BUY",
                HasOracle,
                () =>
                {
                    if (HasOracle())
                        return;

                    CoreBots.Instance.Logger("Getting Starter Support/Healer class (Oracle - 0 AC)");
                    LeaveCombatRoomForEquipmentChange();
                    CoreBots.Instance.BuyItem("classhalla", 299, "Oracle");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        SetClass();

        // 3. Level 10
        CoreBots.Instance.Logger("Starting out acc:\n\tGoals: Oracle, Ninja & Mage Classes");
        if (
            !RunCooperativeObjective(
                "BEGINNER_LEVEL_10",
                () => Bot.Player.Level >= 10,
                () =>
                {
                    if (Bot.Player.Level >= 10)
                        return;

                    SetClass(ClassType.Farm);
                    Farm.Experience(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Bone Berserker",
                        compositionType: ClassType.Farm
                    );
                }
            )
        )
            return false;

        // 3b. Rank Oracle to 10 right after Level 10 so our Healer is full power for Rogue/Ninja/Mazumi
        if (
            !RunCooperativeObjective(
                "BEGINNER_ORACLE_RANK_10",
                HasOracleRank10,
                () =>
                {
                    if (HasOracleRank10())
                        return;

                    RankUpClassForArmy("Oracle");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        SetClass();

        // 4. Rogue
        CoreBots.Instance.Logger("Getting Started: Beginner Levels/Equipment");
        bool HasRogue() => HasAnyClassInInventory("Rogue (Rare)", "Rogue");

        if (
            !RunCooperativeObjective(
                "BEGINNER_ROGUE",
                HasRogue,
                () =>
                {
                    if (HasRogue())
                        return;

                    CoreBots.Instance.Logger(
                        "Getting rogue.. so we can get ninja (there's a 10k hp \"boss\" to kill during the \"Hit Job\" quest.)"
                    );
                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("classhalla", 172, "Rogue");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 5. Ninja (split into granular cooperative barriers)
        bool HasNinja() => HasAnyClassInInventory("Assassin", "Ninja Warrior", "Ninja");

        bool HasRank10Rogue() =>
            HasRank10Class("Rogue") || HasRank10Class("Rogue (Rare)");

        // 5a. Rank up Rogue to 10
        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_RANK_ROGUE",
                () => HasNinja() || HasRank10Rogue(),
                () =>
                {
                    if (HasNinja() || HasRank10Rogue())
                        return;

                    CoreBots.Instance.Logger("Getting starter Dodge class (Ninja) - Ranking Rogue");
                    string rogueClass = HasClassInInventory("Rogue (Rare)")
                        ? "Rogue (Rare)"
                        : "Rogue";
                    RankUpClassForArmy(rogueClass);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        // 5b. Level to 25 + SetClass
        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_LEVEL_25",
                () => Bot.Player.Level >= 25,
                () =>
                {
                    if (Bot.Player.Level >= 25)
                        return;

                    SetClass(ClassType.Farm);
                    Farm.Experience(25);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        compositionType: ClassType.Farm
                    );
                }
            )
        )
            return false;

        // 5c. Mazumi quests (coordinated quest by quest and monster by monster)
        if (!RunCoordinatedMazumiQuests(HasNinja))
            return false;

        // 5d. Buy Ninja
        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_BUY",
                HasNinja,
                () =>
                {
                    if (HasNinja())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    CoreBots.Instance.BuyItem("classhalla", 178, "Ninja");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 5e. Rank Ninja to 10 (all accounts rank up Ninja; 1st to finish becomes Support Helper)
        bool HasRank10Ninja() =>
            HasRank10Class("Ninja") || HasRank10Class("Assassin") || HasRank10Class("Ninja Warrior");

        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_RANK_10",
                HasRank10Ninja,
                () =>
                {
                    if (HasRank10Ninja())
                        return;

                    string ninjaClass =
                        HasClassInInventory("Assassin") ? "Assassin"
                        : HasClassInInventory("Ninja Warrior") ? "Ninja Warrior"
                        : "Ninja";
                    RankUpClassForArmy(ninjaClass);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        SetClass();

        // 6. Mage
        bool HasMage() => HasAnyClassInInventory("Mage (Rare)", "Mage");

        bool HasRank10Mage() =>
            HasRank10Class("Mage") || HasRank10Class("Mage (Rare)");

        if (
            !RunCooperativeObjective(
                "BEGINNER_MAGE",
                HasMage,
                () =>
                {
                    if (HasMage())
                        return;

                    CoreBots.Instance.Logger("Getting Starter Farm class (Mage)");
                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("classhalla", 174, 15653, shopItemID: 9845);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6b. Rank Mage to 10 (all accounts rank up Mage; 1st to finish becomes Support Helper)
        if (
            !RunCooperativeObjective(
                "BEGINNER_MAGE_RANK_10",
                HasRank10Mage,
                () =>
                {
                    if (HasRank10Mage())
                        return;

                    string mageClass = HasClassInInventory("Mage (Rare)")
                        ? "Mage (Rare)"
                        : "Mage";
                    RankUpClassForArmy(mageClass);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        // 8. Final SetClass (resolves party role Solo/Support/Debuff + Farm + Dodge and ranks to 10)
        bool finalClassSetComplete = false;
        if (
            !RunCooperativeObjective(
                "BEGINNER_FINAL_SET_CLASS",
                () => finalClassSetComplete,
                () =>
                {
                    SetClass(rankUpIfNeeded: true);
                    finalClassSetComplete = !Bot.ShouldExit;
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    private bool RunCoordinatedMazumiQuests(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;

        // 5c-1. Quest 90: Ninja Grudge (Fishman Soldier in /pirates)
        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_MAZUMI_90",
                () => skipCondition() || core.isCompletedBefore(92, log: false) || core.isCompletedBefore(90, log: false),
                () =>
                {
                    if (skipCondition() || core.isCompletedBefore(92, log: false) || core.isCompletedBefore(90, log: false))
                        return;

                    SetClass();
                    Story.KillQuest(90, "pirates", "Fishman Soldier");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Fishman Soldier");
                }
            )
        )
            return false;

        // 5c-2. Quest 91: Without a Trace (5 targets in /greenguardwest)
        bool IsQuest91Done() =>
            skipCondition() || core.isCompletedBefore(92, log: false) || core.isCompletedBefore(91, log: false);

        (string Suffix, string Monster, string Item, int Quant, string? Cell, string? Pad)[] q91Targets =
        {
            ("KITTARIAN", "Kittarian", "Kittarian's Wallet", 2, null, null),
            ("FISHMAN", "River Fishman", "River Fishman's Wallet", 2, null, null),
            ("SLIME", "Slime", "Slime-Soaked Wallet", 2, null, null),
            ("FROGZARD", "Frogzard", "Frogzard's Lint Hoard", 2, "West3", "Up"),
            ("BOAR", "Big Bad Boar", "Big Bad Boar's Wallet", 1, "West12", "Up"),
        };

        foreach (var target in q91Targets)
        {
            if (
                !RunCooperativeObjective(
                    $"BEGINNER_NINJA_MAZUMI_91_{target.Suffix}",
                    () => IsQuest91Done() || core.CheckInventory(target.Item, target.Quant),
                    () =>
                    {
                        if (IsQuest91Done() || core.CheckInventory(target.Item, target.Quant))
                            return;

                        SetClass();
                        core.EnsureAccept(91);
                        if (target.Cell != null && target.Pad != null)
                            core.KillMonster("greenguardwest", target.Cell, target.Pad, target.Monster, target.Item, target.Quant);
                        else
                            core.HuntMonster("greenguardwest", target.Monster, target.Item, target.Quant);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(targetPlayerName, target.Monster);
                    }
                )
            )
                return false;
        }

        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_MAZUMI_91_COMPLETE",
                IsQuest91Done,
                () =>
                {
                    if (IsQuest91Done())
                        return;

                    core.EnsureComplete(91);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 5c-3. Quest 92: Hit Job (2 10k HP bosses in /greenguardwest)
        bool IsQuest92Done() => skipCondition() || core.isCompletedBefore(92, log: false);

        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_MAZUMI_92_BREKEN",
                () => IsQuest92Done() || core.CheckInventory("Breken the Vile's Sword", 1),
                () =>
                {
                    if (IsQuest92Done() || core.CheckInventory("Breken the Vile's Sword", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(92);
                    core.HuntMonster("greenguardwest", "Breken the Vile", "Breken the Vile's Sword", 1);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Breken the Vile");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_MAZUMI_92_OGUG",
                () => IsQuest92Done() || core.CheckInventory("Ogug Stoneaxe's Helmet", 1),
                () =>
                {
                    if (IsQuest92Done() || core.CheckInventory("Ogug Stoneaxe's Helmet", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(92);
                    core.HuntMonster("greenguardwest", "Ogug Stoneaxe", "Ogug Stoneaxe's Helmet", 1);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Ogug Stoneaxe");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BEGINNER_NINJA_MAZUMI_92_COMPLETE",
                IsQuest92Done,
                () =>
                {
                    if (IsQuest92Done())
                        return;

                    core.EnsureComplete(92);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    #region Step 2A: Bracket 30
    public bool RunBracket30()
    {
        if (!Army.SyncArmy("STEP2A_START"))
            return false;

        ApplyArmyRoomSettings();
        CoreBots core = CoreBots.Instance;
        // 2. GetBoosts (only if enabled in options; runs before skip so GetBoosts=true doesn't block Bracket 30 skip)
        if (
            !Army.AllPlayersSatisfy(
                "SKIP_BRACKET30_GET_BOOSTS",
                () => Bot.Config?.Get<bool>("GetBoosts") != true
            )
        )
        {
            bool getBoostsEnabled = Bot.Config?.Get<bool>("GetBoosts") == true;
            bool boostsDone = false;
            if (
                !RunCooperativeObjective(
                    "BRACKET30_GET_BOOSTS",
                    () => !getBoostsEnabled || boostsDone,
                    () =>
                    {
                        if (!getBoostsEnabled)
                        {
                            boostsDone = true;
                            return;
                        }

                        SetClass(ClassType.Farm);
                        Farm.GetBoost("REP", 10, true);
                        Farm.GetBoost("XP", Bot.Player.Level >= 100 ? 0 : 10, true);
                        Boosts.GetBoostsSelect(10, 10, 0);
                        boostsDone = !Bot.ShouldExit;
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            compositionType: ClassType.Farm
                        );
                    }
                )
            )
                return false;
        }

        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2A_BRACKET30",
                () =>
                    Bot.Player.Level >= 30
                    && HasClassInInventory("Master Ranger")
                    && HasRank10Class("Master Ranger")
                    && HasClassInInventory("Darkblood StormKing")
                    && HasRank10Class("Darkblood StormKing")
                    && (
                        (HasClassInInventory("Imperial Chunin") && HasRank10Class("Imperial Chunin"))
                        || (HasClassInInventory("Chunin") && HasRank10Class("Chunin"))
                    )
                    && HasClassInInventory("Shaman")
                    && HasRank10Class("Shaman")
                    && HasClassInInventory("Evolved Shaman")
                    && HasRank10Class("Evolved Shaman")
                    && HasClassInInventory("Bard")
                    && HasRank10Class("Bard")
                    && Farm.FactionRank("Blade of Awe") >= 10
                    && core.CheckInventory("Awethur's Accoutrements", toInv: false)
                    && core.isCompletedBefore(8828, log: false)
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2A (Bracket 30) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2A_COMPLETE");
        }

        core.Logger("Army FarmerJoe: starting Step 2A (Bracket 30) on all accounts.");

        // 0. Initial SetClass setup
        bool initialSetupDone = false;
        if (
            !RunCooperativeObjective(
                "BRACKET30_INITIAL_SETUP",
                () => initialSetupDone,
                () =>
                {
                    SetClass();
                    EnsureBestStaticBoostGearEquipped();
                    initialSetupDone = !Bot.ShouldExit;
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        if (!_startupBracket30DailiesDone)
        {
            // 1. Elders' Blood Daily (if Level >= 30 and daily available)
            if (!RunEldersBloodDaily("BRACKET30"))
                return false;

            // 1b. Approved Bracket 30 Weeklies & Early Daily (Design Notes 1213, Wheel of Doom 3076/3075, Odd's 'n' Ends 6187)
            if (!RunBracket30ApprovedDailiesAndWeeklies())
                return false;
        }

        // 3. Level 30 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET30_LEVEL_30", 30))
            return false;

        // 4. Master Ranger (Sandsea Rep 10 -> Buy & Rank 10)
        bool HasMasterRanger() => HasClassInInventory("Master Ranger");
        bool HasMasterRangerRank10() => HasMasterRanger() && HasRank10Class("Master Ranger");

        // 4a. Sandsea Rep 10
        if (
            !RunCooperativeObjective(
                "BRACKET30_MR_SANDSEA_REP",
                () => HasMasterRanger() || Farm.FactionRank("Sandsea") >= 10,
                () =>
                {
                    if (HasMasterRanger() || Farm.FactionRank("Sandsea") >= 10)
                        return;

                    if (core.CheckInventory("Venom Head"))
                        core.SellItem("Venom Head");

                    core.Logger("Level 30: Farming Sandsea Rep 10 for Master Ranger");
                    SetClass(ClassType.Farm);
                    Farm.SandseaREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 4b. Buy Master Ranger
        if (
            !RunCooperativeObjective(
                "BRACKET30_MR_BUY",
                HasMasterRanger,
                () =>
                {
                    if (HasMasterRanger())
                        return;

                    if (core.CheckInventory("Venom Head"))
                        core.SellItem("Venom Head");

                    core.Logger("Level 30: Buying Master Ranger");
                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("sandsea", 242, 7260, shopItemID: 5682);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 4c. Rank Master Ranger to 10 & SetClass
        if (
            !RunCooperativeObjective(
                "BRACKET30_MR_RANK_10",
                HasMasterRangerRank10,
                () =>
                {
                    if (HasMasterRangerRank10())
                        return;

                    RankUpClassForArmy("Master Ranger");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 5. Dragonslayer (Galanoth 4 quests coordinated 1 by 1 -> Buy -> Rank 10)
        bool HasDragonslayer() => HasClassInInventory("Dragonslayer");
        bool HasDragonslayerRank10() => HasDragonslayer() && HasRank10Class("Dragonslayer");

        (int QuestId, string Monster)[] galanothQuests =
        {
            (165, "Wyvern"),
            (166, "Bronze Draconian"),
            (167, "Dark Draconian"),
            (168, "Red Dragon"),
        };

        // 5a. Galanoth quests 165, 166, 167, 168 coordinated quest by quest
        foreach (var gQuest in galanothQuests)
        {
            if (
                !RunCooperativeObjective(
                    $"BRACKET30_DS_QUEST_{gQuest.QuestId}",
                    () =>
                        HasDragonslayer()
                        || core.isCompletedBefore(169, log: false)
                        || core.isCompletedBefore(gQuest.QuestId, log: false),
                    () =>
                    {
                        if (
                            HasDragonslayer()
                            || core.isCompletedBefore(169, log: false)
                            || core.isCompletedBefore(gQuest.QuestId, log: false)
                        )
                            return;

                        SetClass();
                        Story.KillQuest(gQuest.QuestId, "lair", gQuest.Monster);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(targetPlayerName, gQuest.Monster);
                    }
                )
            )
                return false;
        }

        // 5b. Buy Dragonslayer
        if (
            !RunCooperativeObjective(
                "BRACKET30_DS_BUY",
                HasDragonslayer,
                () =>
                {
                    if (HasDragonslayer())
                        return;

                    core.Logger("Level 30: Buying Dragonslayer");
                    LeaveCombatRoomForEquipmentChange();
                    core.BuyItem("lair", 38, "Dragonslayer");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 5c. Rank Dragonslayer to 10 & SetClass
        if (
            !RunCooperativeObjective(
                "BRACKET30_DS_RANK_10",
                HasDragonslayerRank10,
                () =>
                {
                    if (HasDragonslayerRank10())
                        return;

                    RankUpClassForArmy("Dragonslayer");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 5d. Imperial Chunin (Yokai Rep 10 in /dragonkoiz — room limit is 3 players, so split the
        // Army into <=3-player sub-rooms for farming Pockey Chew and buying from Shop 95, then
        // restore the shared private room before ranking Imperial Chunin in /icestormarena).
        const int DragonkoiMaxPlayers = 3;
        bool HasImperialChunin() => HasClassInInventory("Imperial Chunin");
        bool HasImperialChuninRank10() =>
            HasImperialChunin() && HasRank10Class("Imperial Chunin");
        void LeaveDragonkoizIfPresent()
        {
            if (string.Equals(Bot.Map?.Name, "dragonkoiz", StringComparison.OrdinalIgnoreCase))
                LeaveCombatRoomForEquipmentChange();
        }

        LeaveDragonkoizIfPresent();
        if (!ApplySubgroupRoomSettings(DragonkoiMaxPlayers))
            return false;

        core.Logger(
            $"Level 30: /dragonkoiz capped at {DragonkoiMaxPlayers} players; Player {GetLocalPlayerNumber()} (subgroup {GetSubgroupRoomIndex(DragonkoiMaxPlayers) + 1}) using private room {core.PrivateRoomNumber}."
        );

        try
        {
            if (
                !RunCooperativeObjective(
                    "BRACKET30_CHUNIN_YOKAI_REP",
                    () => HasImperialChunin() || Farm.FactionRank("Yokai") >= 10,
                    () =>
                    {
                        if (HasImperialChunin() || Farm.FactionRank("Yokai") >= 10)
                            return;

                        core.Logger("Level 30: Farming Yokai Rep 10 for Imperial Chunin");
                        SetClass(ClassType.Farm);
                        Farm.YokaiREP(10);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        if (
                            targetPlayerNumber <= 0
                            || string.IsNullOrWhiteSpace(targetPlayerName)
                        )
                        {
                            LeaveDragonkoizIfPresent();
                            Bot.Sleep(1000);
                            return;
                        }

                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Pockey Chew",
                            compositionType: ClassType.Farm
                        );
                    },
                    maxPlayersPerSubgroup: DragonkoiMaxPlayers
                )
            )
                return false;

            if (
                !RunCooperativeObjective(
                    "BRACKET30_CHUNIN_BUY",
                    HasImperialChunin,
                    () =>
                    {
                        if (HasImperialChunin())
                            return;

                        core.Logger("Level 30: Buying Imperial Chunin");
                        LeaveCombatRoomForEquipmentChange();
                        Adv.BuyItem("dragonkoiz", 95, "Imperial Chunin");
                        Bot.Wait.ForItemBuy();
                        LeaveDragonkoizIfPresent();
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        LeaveDragonkoizIfPresent();
                        Bot.Sleep(1000);
                    },
                    maxWorkAttempts: 3,
                    maxPlayersPerSubgroup: DragonkoiMaxPlayers
                )
            )
                return false;

            LeaveDragonkoizIfPresent();
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        core.Logger(
            $"Level 30: Restored shared private room {core.PrivateRoomNumber} after /dragonkoiz."
        );

        if (
            !RunCooperativeObjective(
                "BRACKET30_CHUNIN_RANK_10",
                HasImperialChuninRank10,
                () =>
                {
                    if (HasImperialChuninRank10())
                        return;

                    RankUpClassForArmy("Imperial Chunin");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 5e. Shaman & Evolved Shaman (Arcangrove Rep 10 in /arcangrove — No boss, Gorillaphant & Seed Spitter ~2k HP, unlocks 2 unique classes for Player 4 & Player 5 from the same Rep!)
        bool HasShaman() => HasClassInInventory("Shaman");
        bool HasShamanRank10() => HasShaman() && HasRank10Class("Shaman");
        bool HasEvolvedShaman() => HasClassInInventory("Evolved Shaman");
        bool HasEvolvedShamanRank10() =>
            HasEvolvedShaman() && HasRank10Class("Evolved Shaman");

        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET30_SHAMAN_ARCANGROVE_REP",
                () =>
                    (HasShaman() && HasEvolvedShaman())
                    || Farm.FactionRank("Arcangrove") >= 10,
                () =>
                {
                    if (
                        (HasShaman() && HasEvolvedShaman())
                        || Farm.FactionRank("Arcangrove") >= 10
                    )
                        return;

                    core.Logger("Level 30: Farming Arcangrove Rep 10 for Shaman & Evolved Shaman");
                    SetClass(ClassType.Farm);
                    Farm.ArcangroveREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET30_SHAMAN_BUY",
                HasShaman,
                () =>
                {
                    if (HasShaman())
                        return;

                    core.Logger("Level 30: Buying Shaman");
                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("arcangrove", 214, 5765, shopItemID: 4799);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET30_SHAMAN_RANK_10",
                HasShamanRank10,
                () =>
                {
                    if (HasShamanRank10())
                        return;

                    RankUpClassForArmy("Shaman");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        if (
            !RunCooperativeObjective(
                "BRACKET30_EVOLVED_SHAMAN_BUY",
                HasEvolvedShaman,
                () =>
                {
                    if (HasEvolvedShaman())
                        return;

                    core.Logger("Level 30: Buying Evolved Shaman (same Arcangrove Rep 10)");
                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("arcangrove", 214, "Evolved Shaman", shopItemID: 6396);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET30_EVOLVED_SHAMAN_RANK_10",
                HasEvolvedShamanRank10,
                () =>
                {
                    if (HasEvolvedShamanRank10())
                        return;

                    RankUpClassForArmy("Evolved Shaman");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 5f. Bard (Non-Member Support — Kimberly Story + Mythsong Rep 4 -> Buy in /mythsong shop 186 & Rank 10)
        bool HasBard() => HasClassInInventory("Bard");
        bool HasBardRank10() => HasBard() && HasRank10Class("Bard");
        bool IsBardMythsongDone() => HasBard() || Farm.FactionRank("Mythsong") >= 4;

        if (!RunCoordinatedKimberlyStory(() => IsBardMythsongDone() || core.isCompletedBefore(710, log: false), "BRACKET30_BARD_KIMBERLY"))
            return false;

        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET30_BARD_MYTHSONG_REP4",
                IsBardMythsongDone,
                () =>
                {
                    if (IsBardMythsongDone())
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Farm);
                    Farm.MythsongREP(4);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET30_BARD_BUY",
                HasBard,
                () =>
                {
                    if (HasBard())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("mythsong", 186, "Bard");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET30_BARD_RANK_10",
                HasBardRank10,
                () =>
                {
                    if (HasBardRank10())
                        return;

                    RankUpClassForArmy("Bard");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 6. Blade of Awe Progression (5 UnlockBoA quests coordinated 1 by 1 -> BoA Rep 10 -> Awethur's Accoutrements)
        core.Logger("Level 30: Farming Blade of Awe Rep for enhancements & sword");

        // 6a-1. Quest 2933: Find the Stonewrit! (Sketchy Dragon in /j6)
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_2933_STONEWRIT",
                () => core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2933, log: false),
                () =>
                {
                    if (core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2933, log: false))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(2933);
                    core.HuntMonster("j6", "Sketchy Dragon", "Stonewrit Found!", isTemp: false);
                    core.EnsureComplete(2933);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Sketchy Dragon");
                }
            )
        )
            return false;

        // 6a-2. Quest 2934: Find the Handle! (Fire Elemental in /gilead)
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_2934_HANDLE",
                () => core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2934, log: false),
                () =>
                {
                    if (core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2934, log: false))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(2934);
                    core.HuntMonster("gilead", "Fire Elemental", "Handle Found!", isTemp: false);
                    core.EnsureComplete(2934);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Fire Elemental");
                }
            )
        )
            return false;

        // 6a-3. Quest 2935: Find the Hilt! (Skeletal Viking in /castleundead)
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_2935_HILT",
                () => core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2935, log: false),
                () =>
                {
                    if (core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2935, log: false))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(2935);
                    core.HuntMonster("castleundead", "Skeletal Viking", "Hilt Found!", isTemp: false);
                    core.EnsureComplete(2935);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Skeletal Viking", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 6a-4. Quest 2936: Find the Blade! (Hydra Head in /hydra)
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_2936_BLADE",
                () => core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2936, log: false),
                () =>
                {
                    if (core.isCompletedBefore(2937, log: false) || core.isCompletedBefore(2936, log: false))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(2936);
                    core.HuntMonster("hydra", "Hydra Head", "Blade Found!", isTemp: false);
                    core.EnsureComplete(2936);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Hydra Head", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 6a-5. Quest 2937: Find the Runes! (Escherion in /escherion)
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_2937_RUNES",
                () => core.isCompletedBefore(2937, log: false),
                () =>
                {
                    if (core.isCompletedBefore(2937, log: false))
                        return;

                    SetClass(ClassType.Solo);
                    core.ResetQuest(2937);
                    core.EnsureAccept(2937);
                    core.KillEscherion("Runes Found!");
                    core.EnsureComplete(2937);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, targetMap: "escherion");
                }
            )
        )
            return false;

        // 6b. Blade of Awe Rep 10 & Blade of Awe purchase
        if (
            !RunCooperativeObjective(
                "BRACKET30_BOA_REP_10",
                () => Farm.FactionRank("Blade of Awe") >= 10 && core.CheckInventory(17585, toInv: false),
                () =>
                {
                    SetClass(ClassType.Farm);
                    Farm.BladeofAweREP(10, farmBoA: true);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Skeletal Viking", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 6c. Buy Awethur's Accoutrements
        if (
            !RunCooperativeObjective(
                "BRACKET30_AWETHUR_BUY",
                () => core.CheckInventory("Awethur's Accoutrements", toInv: false),
                () =>
                {
                    if (core.CheckInventory("Awethur's Accoutrements", toInv: false))
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("museum", 631, "Awethur's Accoutrements");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6d. Equip best available static boost gear (Awethur's Accoutrements / Blade of Awe unless superior gear is owned)
        if (!SyncBoostGearForArmy("BRACKET30_BOOST_GEAR_EQUIP"))
            return false;

        // 7. Unlock Forge Helm Enhancement (Quest 8828 coordinated boss by boss)
        bool IsForgeHelmDone() => core.isCompletedBefore(8828, log: false);

        // 7a. Blacksmithing Rep 4
        if (!RunBlacksmithingRank4Step("BRACKET30_FORGE_BLACKSMITH_REP_4", IsForgeHelmDone))
            return false;

        // 7b-1. Quest 8828 Boss 1: Escherion (1st Lord Of Chaos Staff)
        if (
            !RunCooperativeObjective(
                "BRACKET30_FORGE_HELM_ESCHERION",
                () => IsForgeHelmDone() || core.CheckInventory("1st Lord Of Chaos Staff", 1),
                () =>
                {
                    if (IsForgeHelmDone() || core.CheckInventory("1st Lord Of Chaos Staff", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(8828);
                    core.KillEscherion("1st Lord Of Chaos Staff");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, targetMap: "escherion");
                }
            )
        )
            return false;

        // 7b-2. Quest 8828 Boss 2: Vath (Chaos Dragonlord Axe)
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET30_FORGE_HELM_VATH",
                () => IsForgeHelmDone() || core.CheckInventory("Chaos Dragonlord Axe", 1),
                () =>
                {
                    if (IsForgeHelmDone() || core.CheckInventory("Chaos Dragonlord Axe", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(8828);
                    core.KillVath("Chaos Dragonlord Axe");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, targetMap: "stalagbite");
                }
            )
        )
            return false;

        // 7b-3. Quest 8828 Boss 3: Kitsune (Hanzamune Dragon Koi Blade)
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET30_FORGE_HELM_KITSUNE",
                () => IsForgeHelmDone() || core.CheckInventory("Hanzamune Dragon Koi Blade", 1),
                () =>
                {
                    if (IsForgeHelmDone() || core.CheckInventory("Hanzamune Dragon Koi Blade", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(8828);
                    core.KillKitsune("Hanzamune Dragon Koi Blade");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, targetMap: "kitsune");
                }
            )
        )
            return false;

        // 7b-4. Quest 8828 Boss 4: Wolfwing (Wrath of the Werepyre)
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET30_FORGE_HELM_WOLFWING",
                () => IsForgeHelmDone() || core.CheckInventory("Wrath of the Werepyre", 1),
                () =>
                {
                    if (IsForgeHelmDone() || core.CheckInventory("Wrath of the Werepyre", 1))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(8828);
                    core.HuntMonster("wolfwing", "Wolfwing", "Wrath of the Werepyre", isTemp: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Wolfwing", targetMap: "wolfwing");
                }
            )
        )
            return false;

        // 7b-5. Complete Quest 8828
        if (
            !RunCooperativeObjective(
                "BRACKET30_FORGE_HELM_COMPLETE",
                IsForgeHelmDone,
                () =>
                {
                    if (IsForgeHelmDone())
                        return;

                    core.EnsureComplete(8828);
                    core.Logger("Enhancement Unlocked: Forge (Helm)");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        return Army.SyncArmy("STEP2A_COMPLETE");
    }
    #endregion Step 2A: Bracket 30

    #region Step 2B: Bracket 50
    public bool RunBracket50()
    {
        if (!Army.SyncArmy("STEP2B_START"))
            return false;

        ApplyArmyRoomSettings();
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2B_BRACKET50",
                () =>
                    Bot.Player.Level >= 50
                    && HasClassInInventory("Scarlet Sorceress")
                    && HasRank10Class("Scarlet Sorceress")
                    && HasClassInInventory("Dragonslayer General")
                    && HasRank10Class("Dragonslayer General")
                    && HasOwnedBoostWeaponAtLeast(0.15)
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2B (Bracket 50) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2B_COMPLETE");
        }

        core.Logger("Army FarmerJoe: starting Step 2B (Bracket 50) on all accounts.");
        EnsureBestStaticBoostGearEquipped();

        // 1. Level 50 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET50_LEVEL_50", 50))
            return false;

        // 2. Scarlet Sorceress (ShatterGlassMaze -> TowerOfMirrors -> Blood Sorceress drop -> 1 CP + unequip -> Quest 6236 -> Rank 10)
        bool HasScarletSorceress() => HasClassInInventory("Scarlet Sorceress");
        bool HasScarletSorceressRank10() =>
            HasScarletSorceress() && HasRank10Class("Scarlet Sorceress");

        // 2a. Coordinated ShatterGlassMaze (/hedgemaze, Quests 5298-5313)
        if (!RunCoordinatedShatterGlassMaze(HasScarletSorceress))
            return false;

        // 2b. Coordinated TowerOfMirrors (/towerofmirrors, Quests 5314-5332)
        if (!RunCoordinatedTowerOfMirrors(HasScarletSorceress))
            return false;

        // 2c. Drop Blood Sorceress (ItemID 36298) from Scarletta in /towerofmirrors
        if (
            !RunCooperativeObjective(
                "BRACKET50_SS_BLOOD_DROP",
                () => HasScarletSorceress() || HasClassInInventory(36298),
                () =>
                {
                    if (HasScarletSorceress() || HasClassInInventory(36298))
                        return;

                    core.Logger("Level 50: Farming Blood Sorceress drop from Scarletta");
                    core.AddDrop(36298, 42992);
                    SetClass(ClassType.Solo);
                    core.KillMonster(
                        "towerofmirrors",
                        "r16",
                        "Top",
                        "Scarletta",
                        "Blood Sorceress",
                        isTemp: false
                    );
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Scarletta");
                }
            )
        )
            return false;

        // 2d. Upgrade Blood Sorceress (36298) -> Scarlet Sorceress (42992) via Quest 6236
        // Wiki / Server requirements:
        // 1) Blood Sorceress drops at EnhancementLevel == 0 (unenhanced) and iQty == 0 on the server,
        //    even though the Flash client sets bloodSorc.Quantity == 1 in memory upon pickup.
        //    Therefore, checking Quantity < 1 is a trap; we must check Quantity <= 1 (or fallback to Rank 10 if 1 CP turn-in is rejected).
        // 2) Blood Sorceress MUST be unequipped when turning in Quest 6236 (the quest consumes it).
        // 2d. Rank Blood Sorceress to Rank 10 when Scarlet Sorceress is not yet owned (Quest 6236 requires Rank 10 Blood Sorceress)
        bool HasBloodSorceressReadyFor6236() =>
            HasScarletSorceress() || (HasClassInInventory(36298) && HasRank10Class("Blood Sorceress"));

        if (
            !RunCooperativeObjective(
                "BRACKET50_SS_BLOOD_RANK_10",
                HasBloodSorceressReadyFor6236,
                () =>
                {
                    if (HasBloodSorceressReadyFor6236() || !HasClassInInventory(36298))
                        return;

                    RankUpClassForArmy("Blood Sorceress");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;

        // 2e. Upgrade Blood Sorceress (ID 36298) -> Scarlet Sorceress (ID 42992) via Quest 6236 (Drop the Blood Sorceress)
        // Note: Quest 6236 consumes Blood Sorceress (36298) on turn-in, so Blood Sorceress MUST be unequipped first.
        if (
            !RunCooperativeObjective(
                "BRACKET50_SS_UPGRADE_6236",
                HasScarletSorceress,
                () =>
                {
                    if (HasScarletSorceress())
                        return;

                    if (!HasClassInInventory(36298))
                        return;

                    core.AddDrop(42992);

                    if (!HasRank10Class("Blood Sorceress"))
                        RankUpClassForArmy("Blood Sorceress");

                    // Ensure Blood Sorceress is UNEQUIPPED before turning in Quest 6236
                    if (
                        Bot.Inventory.IsEquipped("Blood Sorceress")
                        || string.Equals(
                            Bot.Player.CurrentClass?.Name,
                            "Blood Sorceress",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        SetClass();
                        if (
                            Bot.Inventory.IsEquipped("Blood Sorceress")
                            || string.Equals(
                                Bot.Player.CurrentClass?.Name,
                                "Blood Sorceress",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            string fallbackClass =
                                new[] { "Master Ranger", "Mage (Rare)", "Mage", "Oracle", "Rogue", "Healer", "Warrior" }
                                    .FirstOrDefault(HasClassInInventory)
                                ?? string.Empty;
                            if (!string.IsNullOrEmpty(fallbackClass))
                            {
                                LeaveCombatRoomForEquipmentChange();
                                core.Equip(fallbackClass);
                                Bot.Wait.ForTrue(() => !Bot.Inventory.IsEquipped("Blood Sorceress"), 20);
                            }
                        }
                    }

                    core.Logger("Level 50: Turning in Quest 6236 for Scarlet Sorceress");
                    core.EnsureAccept(6236);
                    core.EnsureComplete(6236);
                    Bot.Wait.ForPickup(42992);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                },
                maxWorkAttempts: 4
            )
        )
            return false;

        // 2f. Rank Scarlet Sorceress to 10 & SetClass
        if (
            !RunCooperativeObjective(
                "BRACKET50_SS_RANK_10",
                HasScarletSorceressRank10,
                () =>
                {
                    if (HasScarletSorceressRank10())
                        return;

                    RankUpClassForArmy("Scarlet Sorceress");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 3. Dragonslayer General (30k Gold + 75 Enchanted Scale via Quest 5294 + 100 Dragon Claw in /dragontown -> Buy -> Rank 10)
        bool HasDSG() => HasClassInInventory(35996);
        bool HasDSGRank10() => HasDSG() && HasRank10Class("Dragonslayer General");

        // 3a. 30,000 Gold for Dragonslayer General
        if (
            !RunCooperativeObjective(
                "BRACKET50_DSG_GOLD",
                () => HasDSG() || Bot.Player.Gold >= 30000,
                () =>
                {
                    if (HasDSG() || Bot.Player.Gold >= 30000)
                        return;

                    SetClass(ClassType.Farm);
                    Farm.Gold(30000);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 3b. Farm 75 Enchanted Scale (Quest 5294) & 100 Dragon Claw from Tempest Dracolich in /dragontown
        if (
            !RunCooperativeObjective(
                "BRACKET50_DSG_MATS",
                () =>
                    HasDSG()
                    || (
                        core.CheckInventory("Enchanted Scale", 75)
                        && core.CheckInventory("Dragon Claw", 100)
                    ),
                () =>
                {
                    if (
                        HasDSG()
                        || (
                            core.CheckInventory("Enchanted Scale", 75)
                            && core.CheckInventory("Dragon Claw", 100)
                        )
                    )
                        return;

                    // Quest 5294 requires Dragonslayer Rank 10 in inventory
                    core.Logger(
                        "Level 50: Farming Enchanted Scale (75) & Dragon Claw (100) for Dragonslayer General"
                    );
                    core.AddDrop("Enchanted Scale", "Dragon Claw");
                    SetClass(ClassType.Farm);
                    try
                    {
                        core.RegisterQuests(5294);
                        while (
                            !Bot.ShouldExit
                            && (
                                !core.CheckInventory("Enchanted Scale", 75)
                                || !core.CheckInventory("Dragon Claw", 100)
                            )
                        )
                        {
                            core.KillMonster(
                                "dragontown",
                                "r4",
                                "Right",
                                "Tempest Dracolich",
                                log: false
                            );
                        }
                    }
                    finally
                    {
                        core.CancelRegisteredQuests();
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Tempest Dracolich", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 3c. Buy Dragonslayer General
        if (
            !RunCooperativeObjective(
                "BRACKET50_DSG_BUY",
                HasDSG,
                () =>
                {
                    if (HasDSG())
                        return;

                    core.Logger("Level 50: Buying Dragonslayer General");
                    LeaveCombatRoomForEquipmentChange();
                    core.BuyItem("dragontown", 1286, 35996, shopItemID: 3937);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3d. Rank Dragonslayer General to 10 & SetClass
        if (
            !RunCooperativeObjective(
                "BRACKET50_DSG_RANK_10",
                HasDSGRank10,
                () =>
                {
                    if (HasDSGRank10())
                        return;

                    RankUpClassForArmy("Dragonslayer General");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName);
                }
            )
        )
            return false;
        SetClass();

        // 4. Burning Blade (+15% All Damage weapon from Diabolical Warlord in /lostruinswar)
        bool HasBurningBladeOrBetter() => HasOwnedBoostWeaponAtLeast(0.15);

        if (
            !RunCooperativeObjective(
                "BRACKET50_BURNING_BLADE",
                HasBurningBladeOrBetter,
                () =>
                {
                    if (HasBurningBladeOrBetter())
                        return;

                    core.Logger("Level 50: Farming Burning Blade (+15% All Damage) from Diabolical Warlord");
                    core.AddDrop("Burning Blade");
                    PrepareCombatLoadoutForArmy(ClassType.Solo, "lostruinswar");
                    core.KillMonster(
                        "lostruinswar",
                        "r7",
                        "Left",
                        "Diabolical Warlord",
                        "Burning Blade",
                        isTemp: false
                    );
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Diabolical Warlord");
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo, "lostruinswar")
            )
        )
            return false;

        if (!SyncBoostGearForArmy("BRACKET50_BURNING_BLADE_EQUIP"))
            return false;

        // 8. Approved Bracket 50 Dailies (Unlock "A Loyal Follower" 2084 -> Mine Crafting 2091 -> ShadowScythe General 3828/3827)
        if (!_startupBracket50DailiesDone && !RunBracket50ApprovedDailies())
            return false;

        return Army.SyncArmy("STEP2B_COMPLETE");
    }

    private bool RunCoordinatedShatterGlassMaze(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_HEDGEMAZE_STORY",
                () =>
                    skipCondition()
                    || core.isCompletedBefore(5332, log: false)
                    || core.isCompletedBefore(5313, log: false)
            )
        )
            return true;

        core.EnsureLoad(Enumerable.Range(5298, 16).ToArray());

        bool IsHedgeQuestDone(int questId) =>
            skipCondition()
            || core.isCompletedBefore(5332, log: false)
            || core.isCompletedBefore(5313, log: false)
            || core.isCompletedBefore(questId, log: false);

        // Quest 5298: Find the Hedge Maze
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5298",
                () => IsHedgeQuestDone(5298),
                () =>
                {
                    if (IsHedgeQuestDone(5298))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5298);
                    core.GetMapItem(4678, 1, "hedgemaze");
                    core.HuntMonster(
                        "hedgemaze",
                        "Knight's Reflection",
                        "Knight's Reflection Dispersed",
                        1
                    );
                    core.EnsureComplete(5298);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Knight's Reflection", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5299: Clear the Hedges
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5299",
                () => IsHedgeQuestDone(5299),
                () =>
                {
                    if (IsHedgeQuestDone(5299))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5299);
                    core.HuntMonster("hedgemaze", "Mirrored Shard", "Mirror Shard Slain", 4);
                    core.HuntMonsterMapID("hedgemaze", 48, "Hedge Goblin Slain", 4);
                    core.HuntMonsterMapID("hedgemaze", 20, "Minotaur Slain", 3);
                    Story.MapItemQuest(5299, "hedgemaze", 4679);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5300: Un-Mirror the Knights
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5300",
                () => IsHedgeQuestDone(5300),
                () =>
                {
                    if (IsHedgeQuestDone(5300))
                        return;

                    SetClass(ClassType.Farm);
                    Story.KillQuest(5300, "hedgemaze", "Knight's Reflection");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Knight's Reflection", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5301: Find the Goblin Camp
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5301",
                () => IsHedgeQuestDone(5301),
                () =>
                {
                    if (IsHedgeQuestDone(5301))
                        return;

                    Story.MapItemQuest(5301, "hedgemaze", 4680);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5302:Wait, Those Aren't Redcaps!
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5302",
                () => IsHedgeQuestDone(5302),
                () =>
                {
                    if (IsHedgeQuestDone(5302))
                        return;

                    SetClass(ClassType.Farm);
                    Story.KillQuest(5302, "hedgemaze", "Hedge Goblin");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Hedge Goblin", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5303: Pick the Glass Flowers
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5303",
                () => IsHedgeQuestDone(5303),
                () =>
                {
                    if (IsHedgeQuestDone(5303))
                        return;

                    Story.MapItemQuest(5303, "hedgemaze", 4681, 12);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5304: Polish the Mirror Shards
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5304",
                () => IsHedgeQuestDone(5304),
                () =>
                {
                    if (IsHedgeQuestDone(5304))
                        return;

                    SetClass(ClassType.Farm);
                    Story.KillQuest(5304, "hedgemaze", "Mirrored Shard");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Mirrored Shard", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5305: Hone the Guerilla Tactic
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5305",
                () => IsHedgeQuestDone(5305),
                () =>
                {
                    if (IsHedgeQuestDone(5305))
                        return;

                    Story.MapItemQuest(5305, "hedgemaze", 4682);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5306: Slay the Minotaur Prime (Boss)
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5306_MINOTAUR_PRIME",
                () => IsHedgeQuestDone(5306),
                () =>
                {
                    if (IsHedgeQuestDone(5306))
                        return;

                    SetClass(ClassType.Solo);
                    Story.KillQuest(5306, "hedgemaze", "Minotaur Prime");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Minotaur Prime");
                }
            )
        )
            return false;

        // Quests 5307, 5308, 5309: MapItems 4683, 4684, 4685 x5
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5307_5309",
                () => IsHedgeQuestDone(5309),
                () =>
                {
                    if (IsHedgeQuestDone(5309))
                        return;

                    if (!IsHedgeQuestDone(5307))
                        Story.MapItemQuest(5307, "hedgemaze", 4683);
                    if (!IsHedgeQuestDone(5308))
                        Story.MapItemQuest(5308, "hedgemaze", 4684);
                    if (!IsHedgeQuestDone(5309))
                        Story.MapItemQuest(5309, "hedgemaze", 4685, 5);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5310: Defend the Maze
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5310",
                () => IsHedgeQuestDone(5310),
                () =>
                {
                    if (IsHedgeQuestDone(5310))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5310);
                    core.KillMonster("hedgemaze", "r21", "Right", "*", "Maze Monster Killed", 8);
                    core.EnsureComplete(5310);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5311: Find the Center
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5311",
                () => IsHedgeQuestDone(5311),
                () =>
                {
                    if (IsHedgeQuestDone(5311))
                        return;

                    Story.MapItemQuest(5311, "hedgemaze", 4686);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5312: Shattered Knight (54.3k HP Boss)
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5312_SHATTERED_KNIGHT",
                () => IsHedgeQuestDone(5312),
                () =>
                {
                    if (IsHedgeQuestDone(5312))
                        return;

                    SetClass(ClassType.Solo);
                    Story.KillQuest(5312, "hedgemaze", "Shattered Knight");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shattered Knight");
                }
            )
        )
            return false;

        // Quest 5313: Resurrected Minotaur (79.1k HP Boss)
        if (
            !RunCooperativeObjective(
                "BRACKET50_HEDGEMAZE_5313_RESURRECTED_MINOTAUR",
                () => IsHedgeQuestDone(5313),
                () =>
                {
                    if (IsHedgeQuestDone(5313))
                        return;

                    SetClass(ClassType.Solo);
                    Story.KillQuest(5313, "hedgemaze", "Resurrected Minotaur");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Resurrected Minotaur");
                }
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    private bool RunCoordinatedTowerOfMirrors(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_TOWER_OF_MIRRORS_STORY",
                () => skipCondition() || core.isCompletedBefore(5332, log: false)
            )
        )
            return true;

        core.EnsureLoad(Enumerable.Range(5314, 19).ToArray());

        bool IsTowerQuestDone(int questId) =>
            skipCondition()
            || core.isCompletedBefore(5332, log: false)
            || core.isCompletedBefore(questId, log: false);

        // Quest 5314: Drink Me
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5314_DRINK_ME",
                () => IsTowerQuestDone(5314),
                () =>
                {
                    if (IsTowerQuestDone(5314))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5314);
                    core.HuntMonster("towerofmirrors", "Glassgoyle", "Glass Powder", 8);
                    core.HuntMonster("towerofmirrors", "Glass Serpent", "Reflective Venom", 4);
                    core.EnsureComplete(5314);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5315: The Key To Success
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5315_KEY_SUCCESS",
                () => IsTowerQuestDone(5315),
                () =>
                {
                    if (IsTowerQuestDone(5315))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5315);
                    core.GetMapItems(new[] { (4691, 1), (4692, 1) }, "towerofmirrors");
                    core.HuntMonster("towerofmirrors", "Silver Elemental", "Mirror Key", 1);
                    core.EnsureComplete(5315);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Silver Elemental", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5316: Phanatics
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5316_PHANATICS",
                () => IsTowerQuestDone(5316),
                () =>
                {
                    if (IsTowerQuestDone(5316))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5316);
                    core.HuntMonster("towerofmirrors", "Phans", "Mosh Past the Phans", 8);
                    core.HuntMonster("towerofmirrors", "Phans", "Backstage Pass", 1);
                    core.EnsureComplete(5316);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Phans", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5317: But I Have A Backstage Pass! (Boss: Lathannos the Reversed, MapID 36)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5317_LATHANNOS",
                () => IsTowerQuestDone(5317),
                () =>
                {
                    if (IsTowerQuestDone(5317))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5317);
                    core.HuntMonsterMapID(
                        "towerofmirrors",
                        36,
                        "Lathannos the Reversed Defeated"
                    );
                    core.EnsureComplete(5317);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "36");
                }
            )
        )
            return false;

        // Quest 5318: True Love
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5318_TRUE_LOVE",
                () => IsTowerQuestDone(5318),
                () =>
                {
                    if (IsTowerQuestDone(5318))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5318);
                    core.GetMapItems(new[] { (4687, 1), (4693, 1) }, "towerofmirrors");
                    core.HuntMonster("towerofmirrors", "Silver Elemental", "Andres' Mirror Key", 1);
                    core.EnsureComplete(5318);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Silver Elemental", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5319: Turn to the Left
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5319_TURN_LEFT",
                () => IsTowerQuestDone(5319),
                () =>
                {
                    if (IsTowerQuestDone(5319))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5319);
                    core.HuntMonster("towerofmirrors", "Runway Wraith", "Runway Wraith Defeated", 6);
                    core.HuntMonster("towerofmirrors", "Runway Wraith", "Avant-Garde Footwear", 1);
                    core.HuntMonster("towerofmirrors", "Runway Wraith", "Retro Wig", 1);
                    core.HuntMonster(
                        "towerofmirrors",
                        "Runway Wraith",
                        "Someone's Wearable Art Project",
                        1
                    );
                    core.EnsureComplete(5319);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Runway Wraith", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5320: Now Turn to the Right (Boss: Lukrisio the Buffed, MapID 40)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5320_LUKRISIO",
                () => IsTowerQuestDone(5320),
                () =>
                {
                    if (IsTowerQuestDone(5320))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5320);
                    core.HuntMonsterMapID("towerofmirrors", 40, "Lukrisio the Buffed Defeated");
                    core.EnsureComplete(5320);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "40");
                }
            )
        )
            return false;

        // Quest 5321: Or Maybe THIS Is True Love
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5321_MAYBE_TRUE_LOVE",
                () => IsTowerQuestDone(5321),
                () =>
                {
                    if (IsTowerQuestDone(5321))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5321);
                    core.GetMapItems(new[] { (4688, 1), (4694, 1) }, "towerofmirrors");
                    core.HuntMonster(
                        "towerofmirrors",
                        "Silver Elemental",
                        "Gisele's Mirror Key",
                        1
                    );
                    core.EnsureComplete(5321);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Silver Elemental", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5322: Those Harpies!
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5322_HARPIES",
                () => IsTowerQuestDone(5322),
                () =>
                {
                    if (IsTowerQuestDone(5322))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5322);
                    core.HuntMonster("towerofmirrors", "Pageant Mom", "Pageant Mom Defeated", 8);
                    core.HuntMonster("towerofmirrors", "Pageant Mom", "Pageant Sash", 1);
                    core.EnsureComplete(5322);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Pageant Mom", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5323: Drink your Go-Go Juice (Boss: Mendeskar the Smudged, MapID 45)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5323_MENDESKAR",
                () => IsTowerQuestDone(5323),
                () =>
                {
                    if (IsTowerQuestDone(5323))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5323);
                    core.HuntMonsterMapID("towerofmirrors", 45, "Mendeskar the Smudged Defeated");
                    core.EnsureComplete(5323);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "45");
                }
            )
        )
            return false;

        // Quest 5324: Oh Sure, Why Not
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5324_SURE_WHY_NOT",
                () => IsTowerQuestDone(5324),
                () =>
                {
                    if (IsTowerQuestDone(5324))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5324);
                    core.GetMapItems(new[] { (4689, 1), (4695, 1) }, "towerofmirrors");
                    core.HuntMonster(
                        "towerofmirrors",
                        "Silver Elemental",
                        "Emilia's Mirror Key",
                        1
                    );
                    core.EnsureComplete(5324);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Silver Elemental", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5325: Behind the Scenes
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5325_BEHIND_SCENES",
                () => IsTowerQuestDone(5325),
                () =>
                {
                    if (IsTowerQuestDone(5325))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5325);
                    core.HuntMonster("towerofmirrors", "Stage Tech", "Stage Tech Defeated", 9);
                    core.HuntMonster("towerofmirrors", "Stage Tech", "Rope Ladder");
                    core.EnsureComplete(5325);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Stage Tech", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5326: In the Spotlight (Boss: Atticus the Warped, MapID 49)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5326_ATTICUS",
                () => IsTowerQuestDone(5326),
                () =>
                {
                    if (IsTowerQuestDone(5326))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5326);
                    core.HuntMonsterMapID("towerofmirrors", 49, "Atticus the Warped Defeated");
                    core.EnsureComplete(5326);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "49");
                }
            )
        )
            return false;

        // Quest 5327: Oh, I Give Up
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5327_I_GIVE_UP",
                () => IsTowerQuestDone(5327),
                () =>
                {
                    if (IsTowerQuestDone(5327))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5327);
                    core.GetMapItems(new[] { (4690, 1), (4696, 1) }, "towerofmirrors");
                    core.HuntMonster("towerofmirrors", "Silver Elemental", "Dora's Mirror Key", 1);
                    core.EnsureComplete(5327);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Silver Elemental", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5328: We Gotta Wendi-GO
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5328_WENDIGO",
                () => IsTowerQuestDone(5328),
                () =>
                {
                    if (IsTowerQuestDone(5328))
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5328);
                    core.HuntMonster("towerofmirrors", "Sasquatch", "Sasquatches Defeated", 10);
                    core.HuntMonster("towerofmirrors", "Sasquatch", "Tracking Tag", 5);
                    core.EnsureComplete(5328);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Sasquatch", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5329: Kick His- Wait, Did We Already Use That Pun? (Boss: Leofire the Shattered, MapID 52)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5329_LEOFIRE",
                () => IsTowerQuestDone(5329),
                () =>
                {
                    if (IsTowerQuestDone(5329))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5329);
                    core.HuntMonsterMapID("towerofmirrors", 52, "Leofire the Shattered Defeated");
                    core.EnsureComplete(5329);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "52");
                }
            )
        )
            return false;

        // Quest 5330: Fiend Zoned
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5330_FIEND_ZONED",
                () => IsTowerQuestDone(5330),
                () =>
                {
                    if (IsTowerQuestDone(5330))
                        return;

                    SetClass(ClassType.Farm);
                    Story.KillQuest(5330, "towerofmirrors", "Fervent Suitor");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Fervent Suitor", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Quest 5331: Find Scarletta (Cutscene Cut4 -> Jump back to Enter)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5331_FIND_SCARLETTA",
                () => IsTowerQuestDone(5331),
                () =>
                {
                    if (IsTowerQuestDone(5331))
                        return;

                    Story.MapItemQuest(5331, "towerofmirrors", 4697);
                    Bot.Wait.ForCellChange("Cut4");
                    while (!Bot.ShouldExit && Bot.Player.Cell != "Enter")
                    {
                        core.Jump();
                        core.Sleep();
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Quest 5332: Defeat ... Wait. What? (Final Boss: Scarletta / Groglurk, MapID 32)
        if (
            !RunCooperativeObjective(
                "BRACKET50_TOWER_5332_GROGLURK",
                () => IsTowerQuestDone(5332),
                () =>
                {
                    if (IsTowerQuestDone(5332))
                        return;

                    core.AddDrop("Blood Sorceress");
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(5332);
                    core.HuntMonsterMapID("towerofmirrors", 32, "Defeat the Groglurk");
                    core.EnsureComplete(5332);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "32");
                }
            )
        )
            return false;

        return !Bot.ShouldExit;
    }
    #endregion Step 2B: Bracket 50

    #region Step 2C: Brackets 55, 60, and 65
    /// <summary>
    /// Step 2C (Part 1 - Bracket 55):
    /// 1. Level 55 + SmartEnhance
    /// 2. Blaze Binder (Pyromancer -> 1 CP -> DoomWood REP 10 -> Embersea REP 10 -> Blaze Binder Rank 10)
    /// 3. Cryomancer (Coordinated /frozentower 3907-3937 -> Daily 3966/3965 -> Buy & Rank 10 if 84x tokens reached)
    /// 4. Arachnomancer (Coordinated /twilightedge 3428 + /weaverwar 3429-3439 -> Ravenloss REP 10 -> Buy & Rank 10)
    /// 5. Cape of Awe (+25% CP/EXP/Gold/Rep from Binky in /doomvault)
    /// 6. Approved Bracket 55 Dailies (Seal of Light 3826 & Seal of Darkness 3825)
    /// </summary>
    public bool RunBracket55()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        // Entry barrier for Bracket 55
        if (!Army.SyncArmy("STEP2C_55_START"))
            return false;

        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2C_55_BRACKET55",
                () =>
                    Bot.Player.Level >= 55
                    && HasClassInInventory("Blaze Binder")
                    && HasRank10Class("Blaze Binder")
                    && HasClassInInventory("Cryomancer")
                    && HasRank10Class("Cryomancer")
                    && HasClassInInventory("Arachnomancer")
                    && HasRank10Class("Arachnomancer")
                    && core.CheckInventory("Cape of Awe", toInv: false)
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2C (Bracket 55) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2C_55_COMPLETE") && !Bot.ShouldExit;
        }

        SetClass(rankUpIfNeeded: true);
        EnsureBestStaticBoostGearEquipped();

        // 1. Level 55 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET55_LEVEL_55", 55))
            return false;

        // 2. Blaze Binder (Pyromancer -> 1 CP -> DoomWood REP 10 -> Embersea REP 10 -> Blaze Binder Rank 10)
        bool HasBB() => HasClassInInventory("Blaze Binder");
        bool HasBBRank10() => HasBB() && HasRank10Class("Blaze Binder");
        bool HasPyro() => HasBB() || HasAnyClassInInventory(12811, 12812) || HasClassInInventory("Pyromancer");
        // Note: When a class is freshly bought/dropped in the current session, the Flash client sets item.Quantity = 1 in memory
        // even though it has 0 Class Points (iQty = 0) on the server. Killing 1 monster awards >= 50 CP, so Quantity > 1 guarantees >= 1 real CP synced on the server.
        bool HasPyro1CP() =>
            HasBB()
            || Bot.Inventory.Items.Any(item =>
                item != null
                && (
                    item.ID == 12811
                    || item.ID == 12812
                    || string.Equals(item.Name?.Trim(), "Pyromancer", StringComparison.OrdinalIgnoreCase)
                )
                && item.Category == ItemCategory.Class
                && item.Quantity > 1
            );

        // 2a. Farm 84x Shurpu Blaze Token from Shurpu Ring Guardian in /xancave
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET55_BB_PYRO_TOKENS",
                () => HasPyro() || core.CheckInventory("Shurpu Blaze Token", 84),
                () =>
                {
                    if (HasPyro() || core.CheckInventory("Shurpu Blaze Token", 84))
                        return;

                    core.AddDrop("Shurpu Blaze Token");
                    PrepareCombatLoadoutForArmy(ClassType.Solo, "xancave");
                    if (!JoinMapVerified("xancave"))
                    {
                        core.Logger("Map '/xancave' is currently unavailable for 'BRACKET55_BB_PYRO_TOKENS'.");
                        return;
                    }
                    core.HuntMonster(
                        "xancave",
                        "Shurpu Ring Guardian",
                        "Shurpu Blaze Token",
                        84,
                        isTemp: false
                    );
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shurpu Ring Guardian", targetMap: "xancave");
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo, "xancave")
            )
        )
            return false;

        // 2b. Buy Pyromancer (ItemID 12812) in /xancave Shop 447 and sell leftover tokens
        if (
            !RunCooperativeObjective(
                "BRACKET55_BB_PYRO_BUY",
                HasPyro,
                () =>
                {
                    if (HasPyro())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    core.BuyItem("xancave", 447, 12812, shopItemID: 1278);
                    Bot.Wait.ForItemBuy();
                    if (core.CheckInventory("Shurpu Blaze Token"))
                        core.SellItem("Shurpu Blaze Token", all: true);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 2c. Ensure Pyromancer has >= 1 Class Point (1 CP) so Firestorm Merge Shop (1140) accepts it
        if (
            !RunCooperativeObjective(
                "BRACKET55_BB_PYRO_1CP",
                HasPyro1CP,
                () =>
                {
                    if (HasPyro1CP())
                        return;

                    if (!Bot.Inventory.IsEquipped("Pyromancer"))
                    {
                        LeaveCombatRoomForEquipmentChange();
                        EnhanceClassForArmyRespectingGearPolicy("Pyromancer", allowFullGearEnhance: false);
                        core.Equip("Pyromancer");
                        Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped("Pyromancer"), 20);
                    }
                    Bot.Skills.StartAdvanced("Pyromancer", false, ClassUseMode.Base);
                    core.KillMonster("noobshire", "Enter", "Spawn", "*", log: false);
                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 2d. DoomWood REP Rank 10 in /shadowfallwar (Garden1, Left, Quests 1151, 1152, 1153)
        if (
            !RunCooperativeObjective(
                "BRACKET55_BB_DOOMWOOD_REP",
                () => HasBB() || Farm.FactionRank("DoomWood") >= 10,
                () =>
                {
                    if (HasBB() || Farm.FactionRank("DoomWood") >= 10)
                        return;

                    EnsureBestStaticBoostGearEquipped(BoostObjective.Rep);
                    SetClass(ClassType.Farm);
                    Farm.DoomWoodREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 2e. Embersea REP Rank 10 in /fireforge (Quest 4228, Blazebinder MapID 4)
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET55_BB_EMBERSEA_REP",
                () => HasBB() || Farm.FactionRank("Embersea") >= 10,
                () =>
                {
                    if (HasBB() || Farm.FactionRank("Embersea") >= 10)
                        return;

                    EnsureBestStaticBoostGearEquipped(BoostObjective.Rep);
                    SetClass(ClassType.Farm);
                    Farm.EmberseaREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 2f. Buy Darkness Sigil, Flame Sigil, and Blaze Binder in /fireforge
        if (
            !RunCooperativeObjective(
                "BRACKET55_BB_BUY",
                HasBB,
                () =>
                {
                    if (HasBB())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    if (Bot.Inventory.IsEquipped("Pyromancer"))
                    {
                        SetClass(ClassType.Solo);
                    }

                    Adv.BuyItem("fireforge", 1142, "Darkness Sigil");
                    Adv.BuyItem("fireforge", 1142, "Flame Sigil");
                    Adv.BuyItem("fireforge", 1140, "Blaze Binder");
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 2g. Rank Blaze Binder to Rank 10 and update SetClass()
        if (
            !RunCooperativeObjective(
                "BRACKET55_BB_RANK_10",
                HasBBRank10,
                () =>
                {
                    if (!HasBB())
                        return;

                    if (!HasBBRank10())
                        RankUpClassForArmy("Blaze Binder");

                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 3. Cryomancer (Coordinated /frozentower 3907-3937 -> Daily 3966/3965 -> Buy & Rank 10 if acquired)
        bool HasCryoAnywhere() => core.CheckInventory("Cryomancer", toInv: false);
        bool HasCryo() => HasClassInInventory("Cryomancer");
        bool HasCryoRank10() => HasCryo() && HasRank10Class("Cryomancer");
        bool cryoDailyAvailable =
            !HasCryoAnywhere()
            && (
                CanRunPeriodicQuest(3966)
                || (core.IsMember && CanRunPeriodicQuest(3965))
                || core.CheckInventory("Glacera Ice Token", 84, toInv: false)
            );
        bool IsCryoDailyDone() =>
            !cryoDailyAvailable
            || HasCryoAnywhere()
            || (
                Bot.Quests.IsDailyComplete(3966)
                && (!core.IsMember || Bot.Quests.IsDailyComplete(3965))
                && !core.CheckInventory("Glacera Ice Token", 84, toInv: false)
            );

        ApplySubgroupRoomSettings(4);
        try
        {
            if (!RunCoordinatedFrozenTower(HasCryoAnywhere))
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET55_CRYO_DAILY",
                IsCryoDailyDone,
                () =>
                {
                    if (IsCryoDailyDone())
                        return;

                    UnbankIfOwned("Glacera Ice Token");
                    if (!HasCryoAnywhere() && core.CheckInventory("Glacera Ice Token", 84))
                    {
                        LeaveCombatRoomForEquipmentChange();
                        core.BuyItem("frozenruins", 1056, 27525, shopItemID: 2603, index: 1);
                        Bot.Wait.ForItemBuy();
                    }

                    if (!HasCryoAnywhere())
                    {
                        SetClass(ClassType.Farm);
                        Daily.Cryomancer();

                        if (!HasCryoAnywhere() && core.CheckInventory("Glacera Ice Token", 84, toInv: false))
                        {
                            UnbankIfOwned("Glacera Ice Token");
                            LeaveCombatRoomForEquipmentChange();
                            core.BuyItem("frozenruins", 1056, 27525, shopItemID: 2603, index: 1);
                            Bot.Wait.ForItemBuy();
                        }
                    }

                    BankIfPresent("Glacera Ice Token");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Invader", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET55_CRYO_RANK_10",
                () => !HasCryo() || HasCryoRank10(),
                () =>
                {
                    if (!HasCryo())
                        return;

                    if (!HasCryoRank10())
                        RankUpClassForArmy("Cryomancer");

                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 4. Arachnomancer (Coordinated /twilightedge 3428 + /weaverwar 3429-3439 -> Ravenloss REP 10 -> Buy & Rank 10)
        bool HasArach() => HasClassInInventory("Arachnomancer");
        bool HasArachRank10() => HasArach() && HasRank10Class("Arachnomancer");
        bool IsWeaverWarDone() =>
            HasArach() || Farm.FactionRank("Ravenloss") >= 10 || core.isCompletedBefore(3439, log: false);

        if (!RunCoordinatedWeaverWar(IsWeaverWarDone))
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET55_ARACH_RAVENLOSS_REP",
                () => HasArach() || Farm.FactionRank("Ravenloss") >= 10,
                () =>
                {
                    if (HasArach() || Farm.FactionRank("Ravenloss") >= 10)
                        return;

                    SetClass(ClassType.Farm);
                    Farm.RavenlossREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "ChaosWeaver Mage", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET55_ARACH_BUY",
                HasArach,
                () =>
                {
                    if (HasArach())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    core.BuyItem("ravenloss", 850, "Arachnomancer", shopItemID: 14837);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET55_ARACH_RANK_10",
                HasArachRank10,
                () =>
                {
                    if (!HasArach())
                        return;

                    if (!HasArachRank10())
                        RankUpClassForArmy("Arachnomancer");

                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 5. Cape of Awe (+25% CP/EXP/Gold/Rep from Binky in /doomvault r5)
        if (!RunBracket55CapeOfAwe())
            return false;

        // 6. Approved Bracket 55 Dailies (Seal of Light 3826 & Seal of Darkness 3825)
        if (!_startupBracket55DailiesDone && !RunBracket55ApprovedDailies())
            return false;

        SetClass(rankUpIfNeeded: true);
        return Army.SyncArmy("STEP2C_55_COMPLETE") && !Bot.ShouldExit;
    }

    /// <summary>
    /// Step 2C (Part 2 - Bracket 60):
    /// 1. Level 60 + SmartEnhance
    /// 2. DragonSoul Shinobi (Coordinated Yokai Story: /shogunwar, /shinringrove, /greenshell, /heiwavalley, /shadowfortress -> 300x Dragon Shinobi Token -> Buy & Rank 10)
    /// 3. Unlock Forge Weapon Enhancement: Lacerate (Quest 8739)
    /// </summary>
    public bool RunBracket60()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        // Entry barrier for Bracket 60
        if (!Army.SyncArmy("STEP2C_60_START"))
            return false;

        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2C_60_BRACKET60",
                () =>
                    Bot.Player.Level >= 60
                    && (HasClassInInventory(59476) || HasClassInInventory("DragonSoul Shinobi"))
                    && HasRank10Class("DragonSoul Shinobi")
                    && core.isCompletedBefore(8739, log: false)
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2C (Bracket 60) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2C_60_COMPLETE") && !Bot.ShouldExit;
        }

        SetClass(rankUpIfNeeded: true);
        EnsureBestStaticBoostGearEquipped();

        // 1. Level 60 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET60_LEVEL_60", 60))
            return false;

        // 2. DragonSoul Shinobi (ItemID 59476)
        bool HasDSS() =>
            HasClassInInventory(59476) || HasClassInInventory("DragonSoul Shinobi");
        bool HasDSSRank10() => HasRank10Class("DragonSoul Shinobi");

        if (!RunCoordinatedYokaiStory(HasDSS))
            return false;

        // 2a. Farm 300x Dragon Shinobi Token (ItemID 20561) via Quest 7924 from 1st Head of Orochi in /shadowfortress
        if (
            !RunCooperativeObjective(
                "BRACKET60_DSS_TOKENS",
                () => HasDSS() || core.CheckInventory(20561, 300),
                () =>
                {
                    if (HasDSS() || core.CheckInventory(20561, 300))
                        return;

                    core.AddDrop(20561);
                    SetClass(ClassType.Solo);
                    core.FarmingLogger("Dragon Shinobi Token", 300);
                    try
                    {
                        core.RegisterQuests(7924);
                        while (!Bot.ShouldExit && !core.CheckInventory(20561, 300))
                            core.HuntMonster("shadowfortress", "1st Head of Orochi", log: false);
                    }
                    finally
                    {
                        core.CancelRegisteredQuests();
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "1st Head of Orochi");
                }
            )
        )
            return false;

        // 2b. Buy DragonSoul Shinobi (ItemID 59476) in /shadowfortress Shop 1968
        if (
            !RunCooperativeObjective(
                "BRACKET60_DSS_BUY",
                HasDSS,
                () =>
                {
                    if (HasDSS())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("shadowfortress", 1968, 59476, shopItemID: 8078);
                    Bot.Wait.ForItemBuy();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 2c. Rank DragonSoul Shinobi to Rank 10
        if (
            !RunCooperativeObjective(
                "BRACKET60_DSS_RANK_10",
                HasDSSRank10,
                () =>
                {
                    if (!HasDSS())
                        return;

                    if (!HasDSSRank10())
                        RankUpClassForArmy("DragonSoul Shinobi");

                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 3. Unlock Forge Weapon Enhancement: Lacerate (Quest 8739)
        bool IsLacerateDone() => core.isCompletedBefore(8739, log: false);
        bool HasBlacksmithRank5() => IsLacerateDone() || Farm.FactionRank("Blacksmithing") >= 5;

        // 3a. Ensure Prerequisite Quest 92 (Mazumi) is completed
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_PREREQ_92",
                () => IsLacerateDone() || Bot.Quests.IsUnlocked(8739) || core.isCompletedBefore(92, log: false),
                () =>
                {
                    if (IsLacerateDone() || Bot.Quests.IsUnlocked(8739) || core.isCompletedBefore(92, log: false))
                        return;

                    SetClass(ClassType.Solo);
                    if (!Bot.Quests.IsUnlocked(92))
                    {
                        if (!Bot.Quests.IsUnlocked(91))
                        {
                            core.EnsureAccept(90);
                            core.HuntMonster("pirates", "Shark Bait", "Pirate Medallion", 12);
                            core.EnsureComplete(90);
                        }
                        core.EnsureAccept(91);
                        core.KillMonster("greenguardwest", "West1", "Left", "Kittarian", "Kittarian's Wallet", 2);
                        core.KillMonster("greenguardwest", "West9", "Left", "River Fishman", "River Fishman's Wallet", 2);
                        core.KillMonster("greenguardwest", "West10", "Left", "Slime", "Slime-Soaked Wallet", 2);
                        core.KillMonster("greenguardwest", "West3", "Left", "Frogzard", "Frogzard's Lint Hoard", 2);
                        core.KillMonster("greenguardwest", "West12", "Up", "Big Bad Boar", "Big Bad Boar's Wallet");
                        core.EnsureComplete(91);
                    }
                    if (!core.isCompletedBefore(92, log: false))
                    {
                        core.EnsureAccept(92);
                        core.HuntMonster("greenguardwest", "Breken the Vile", "Breken the Vile's Sword", 1);
                        core.HuntMonster("greenguardwest", "Ogug Stoneaxe", "Ogug Stoneaxe's Helmet", 1);
                        core.EnsureComplete(92);
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 3b. Blacksmithing Rank 5 (Traditional Rank 4 via Quest 2777 + 6 shared rounds of Quest 8736)
        if (!RunBlacksmithingRank4Step("BRACKET60_LACERATE_BLACKSMITH_REP_4", HasBlacksmithRank5))
            return false;

        // Coordinate Quest 8736 (Intensive Metallic Mastery: 2,500 Rep per turn-in, 12,500 Rep from Rank 4 to Rank 5)
        // Split across its 3 maps per round so the entire army fights Hydra Head 25, Creature Creation, and Dread Klunk together!
        for (int round = 1; round <= 6; round++)
        {
            int currentRound = round;
            bool roundCompleted = false;

            if (
                !RunCooperativeObjective(
                    $"BRACKET60_LACERATE_BS8736_R{currentRound}_HYDRA",
                    () => HasBlacksmithRank5() || core.CheckInventory("Hydra Scale Piece", 75),
                    () =>
                    {
                        if (HasBlacksmithRank5() || core.CheckInventory("Hydra Scale Piece", 75))
                            return;

                        core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                        core.EnsureAccept(8736);
                        SetClass(ClassType.Solo);
                        core.HuntMonster(
                            "hydrachallenge",
                            "Hydra Head 25",
                            "Hydra Scale Piece",
                            75,
                            isTemp: false,
                            log: false
                        );
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Hydra Head 25",
                            targetMap: "hydrachallenge"
                        );
                    }
                )
            )
                return false;

            if (
                !RunSubgroupCooperativeObjective(
                    4,
                    $"BRACKET60_LACERATE_BS8736_R{currentRound}_MAUL",
                    () => HasBlacksmithRank5() || core.CheckInventory("Creature Shard", 1),
                    () =>
                    {
                        if (HasBlacksmithRank5() || core.CheckInventory("Creature Shard", 1))
                            return;

                        core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                        core.EnsureAccept(8736);
                        SetClass(ClassType.Solo);
                        core.HuntMonster(
                            "maul",
                            "Creature Creation",
                            "Creature Shard",
                            1,
                            isTemp: false,
                            log: false
                        );
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Creature Creation",
                            targetMap: "maul"
                        );
                    }
                )
            )
                return false;

            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        $"BRACKET60_LACERATE_BS8736_R{currentRound}_KLUNK",
                        () => HasBlacksmithRank5() || roundCompleted,
                        () =>
                        {
                            if (HasBlacksmithRank5() || roundCompleted)
                                return;

                            ApplyCurrentArmyRoomSettings();
                            core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                            core.EnsureAccept(8736);
                            SetClass(ClassType.Solo);
                            core.HuntMonster(
                                "towerofdoom",
                                "Dread Klunk",
                                "Monster Trophy",
                                15,
                                isTemp: false,
                                log: false
                            );
                            LeaveCombatRoomForEquipmentChange();
                            core.EnsureComplete(8736);
                            roundCompleted = true;
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(
                                targetPlayerName,
                                "Dread Klunk",
                                targetMap: "towerofdoom"
                            );
                        },
                        prepareForCombat: () => ApplyCurrentArmyRoomSettings()
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }

        // 3c. Coordinate the 6 required items for Quest 8739 (Lacerate)
        // Item 1: Undead Plague Spear (Big Jack Sprat in /graveyard)
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_SPEAR",
                () => IsLacerateDone() || core.CheckInventory("Undead Plague Spear"),
                () =>
                {
                    if (IsLacerateDone() || core.CheckInventory("Undead Plague Spear"))
                        return;

                    core.AddDrop("Undead Plague Spear");
                    core.EnsureAccept(8739);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("graveyard", "Big Jack Sprat", "Undead Plague Spear", isTemp: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Big Jack Sprat");
                }
            )
        )
            return false;

        // Item 2: Kuro's Wrath (Kuro in /river)
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_KURO",
                () => IsLacerateDone() || core.CheckInventory("Kuro's Wrath"),
                () =>
                {
                    if (IsLacerateDone() || core.CheckInventory("Kuro's Wrath"))
                        return;

                    core.AddDrop("Kuro's Wrath");
                    core.EnsureAccept(8739);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("river", "Kuro", "Kuro's Wrath", isTemp: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Kuro");
                }
            )
        )
            return false;

        // Item 3: Massive Horc Cleaver (Quest 279 -> Horc Master in /warhorc)
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_CLEAVER",
                () => IsLacerateDone() || core.CheckInventory("Massive Horc Cleaver"),
                () =>
                {
                    if (IsLacerateDone() || core.CheckInventory("Massive Horc Cleaver"))
                        return;

                    core.AddDrop("Massive Horc Cleaver");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(279);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("warhorc", "Horc Master", "Boss Prize");
                    core.EnsureComplete(279);
                    Bot.Wait.ForPickup("Massive Horc Cleaver");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Horc Master");
                }
            )
        )
            return false;

        // Item 4: Sword in the Stone (Quest 316 -> 7x MapItem 54 + Spider in /greenguardeast)
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_SWORD_STONE",
                () => IsLacerateDone() || core.CheckInventory("Sword in the Stone"),
                () =>
                {
                    if (IsLacerateDone() || core.CheckInventory("Sword in the Stone"))
                        return;

                    core.AddDrop("Sword in the Stone");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(316);
                    SetClass(ClassType.Farm);
                    core.GetMapItem(54, 7, "greenguardeast");
                    core.HuntMonster("greenguardeast", "Spider", "Tiny Sword");
                    core.EnsureComplete(316);
                    Bot.Wait.ForPickup("Sword in the Stone");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Spider", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Item 5: Forest Axe (Quest 301 -> 4x MapItem 55 + Mosquito in /farm)
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_FOREST_AXE",
                () => IsLacerateDone() || core.CheckInventory("Forest Axe"),
                () =>
                {
                    if (IsLacerateDone() || core.CheckInventory("Forest Axe"))
                        return;

                    core.AddDrop("Forest Axe");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(301);
                    SetClass(ClassType.Farm);
                    core.GetMapItem(55, 4, "farm");
                    core.HuntMonster("farm", "Mosquito", "Mosquito Juice");
                    core.EnsureComplete(301);
                    Bot.Wait.ForPickup("Forest Axe");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Mosquito", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Item 6: Black Knight Orb (Quest 318 across 4 bosses)
        bool HasBKO() => IsLacerateDone() || core.CheckInventory("Black Knight Orb");

        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET60_LACERATE_BKO_WELL",
                () => HasBKO() || Bot.TempInv.Contains("Black Knight Leg Piece"),
                () =>
                {
                    if (HasBKO() || Bot.TempInv.Contains("Black Knight Leg Piece"))
                        return;

                    core.AddDrop("Black Knight Orb");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(318);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("well", "Gell Oh No", "Black Knight Leg Piece");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Gell Oh No");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_BKO_DRAGON",
                () => HasBKO() || Bot.TempInv.Contains("Black Knight Chest Piece"),
                () =>
                {
                    if (HasBKO() || Bot.TempInv.Contains("Black Knight Chest Piece"))
                        return;

                    core.AddDrop("Black Knight Orb");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(318);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("greendragon", "Greenguard Dragon", "Black Knight Chest Piece");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Greenguard Dragon");
                }
            )
        )
            return false;

        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET60_LACERATE_BKO_DEATHGAZER",
                () => HasBKO() || Bot.TempInv.Contains("Black Knight Shoulder Piece"),
                () =>
                {
                    if (HasBKO() || Bot.TempInv.Contains("Black Knight Shoulder Piece"))
                        return;

                    core.AddDrop("Black Knight Orb");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(318);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("deathgazer", "Deathgazer", "Black Knight Shoulder Piece");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Deathgazer");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_BKO_TRUNK",
                () => HasBKO() || Bot.TempInv.Contains("Black Knight Arm Piece"),
                () =>
                {
                    if (HasBKO() || Bot.TempInv.Contains("Black Knight Arm Piece"))
                        return;

                    core.AddDrop("Black Knight Orb");
                    core.EnsureAccept(8739);
                    core.EnsureAccept(318);
                    SetClass(ClassType.Solo);
                    core.HuntMonster("trunk", "GreenGuard Basilisk", "Black Knight Arm Piece");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "GreenGuard Basilisk");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_BKO_COMPLETE",
                HasBKO,
                () =>
                {
                    if (HasBKO())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    core.AddDrop("Black Knight Orb");
                    core.EnsureComplete(318);
                    Bot.Wait.ForPickup("Black Knight Orb");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3d. Turn in Quest 8739 to unlock Lacerate
        if (
            !RunCooperativeObjective(
                "BRACKET60_LACERATE_COMPLETE",
                IsLacerateDone,
                () =>
                {
                    if (IsLacerateDone())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureAccept(8739);
                    core.EnsureComplete(8739);

                    if (IsLacerateDone() && !ShouldPreserveEquippedGear() && !IsDisableAutoEnhanceEnabled())
                    {
                        string? currentClass = Bot.Player.CurrentClass?.Name;
                        if (!string.IsNullOrWhiteSpace(currentClass))
                        {
                            ApplyAndRecordSmartEnhance(currentClass);
                            Army.StartClassSkills(currentClass);
                        }
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        SetClass(rankUpIfNeeded: true);
        return Army.SyncArmy("STEP2C_60_COMPLETE") && !Bot.ShouldExit;
    }

    /// <summary>
    /// Step 2C (Part 3 - Bracket 65):
    /// 1. Level 65 + SmartEnhance
    /// 2. Glacial Berserker (Coordinated Glacera Story up to Quest 5601 -> Glacera REP 10 -> Buy & Rank 10 if Shop 1339 is active)
    /// </summary>
    public bool RunBracket65()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        // Entry barrier for Bracket 65
        if (!Army.SyncArmy("STEP2C_65_START"))
            return false;

        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2C_65_BRACKET65",
                () =>
                    Bot.Player.Level >= 65
                    && (HasAnyClassInInventory(38084, 38049) || HasClassInInventory("Glacial Berserker"))
                    && HasRank10Class("Glacial Berserker")
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2C (Bracket 65) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2C_65_COMPLETE") && !Bot.ShouldExit;
        }

        SetClass(rankUpIfNeeded: true);
        EnsureBestStaticBoostGearEquipped();

        // 1. Level 65 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET65_LEVEL_65", 65))
            return false;

        // 2. Glacial Berserker (ItemID 38084 or 38049)
        bool HasGB() =>
            HasAnyClassInInventory(38084, 38049) || HasClassInInventory("Glacial Berserker");
        bool HasGBRank10() => HasRank10Class("Glacial Berserker");
        bool IsGlaceraRepDone() => HasGB() || Farm.FactionRank("Glacera") >= 10;
        bool IsGlaceraStoryDone() => IsGlaceraRepDone() || core.isCompletedBefore(5601, log: false);

        ApplySubgroupRoomSettings(4);
        try
        {
            if (!RunCoordinatedFrozenTower(IsGlaceraStoryDone))
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        if (!RunCoordinatedGlaceraForGB(IsGlaceraStoryDone))
            return false;

        // 2a. Glacera REP Rank 10 in /icewindwar (cell r5, Left, Quests 5597, 5598, 5599, 5600)
        if (
            !RunSubgroupCooperativeObjective(
                4,
                "BRACKET65_GB_GLACERA_REP",
                IsGlaceraRepDone,
                () =>
                {
                    if (IsGlaceraRepDone())
                        return;

                    SetClass(ClassType.Farm);
                    Farm.GlaceraREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // 2b. Buy Glacial Berserker in /icewindpass Shop 1339 (Seasonal Frostval shop; skips cleanly if out-of-season)
        bool gbBuyAttempted = false;
        if (
            !RunCooperativeObjective(
                "BRACKET65_GB_BUY",
                () => HasGB() || gbBuyAttempted,
                () =>
                {
                    if (HasGB() || gbBuyAttempted)
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    Adv.BuyItem("icewindpass", 1339, "Glacial Berserker", shopItemID: 22266);
                    Bot.Wait.ForItemBuy();
                    gbBuyAttempted = true;
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // 2c. Rank Glacial Berserker to Rank 10 if acquired
        if (
            !RunCooperativeObjective(
                "BRACKET65_GB_RANK_10",
                () => !HasGB() || HasGBRank10(),
                () =>
                {
                    if (!HasGB())
                        return;

                    if (!HasGBRank10())
                        RankUpClassForArmy("Glacial Berserker");

                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        // 3. Approved Bracket 65 Dailies (Embrace Your Chaos 3596 & Lord of Order 7156-7165)
        if (!RunBracket65ApprovedDailies())
            return false;

        SetClass(rankUpIfNeeded: true);
        return Army.SyncArmy("STEP2C_COMPLETE") && !Bot.ShouldExit;
    }

    #region Step 2D: Bracket 70
    /// <summary>
    /// Step 2D (Bracket 70):
    /// 1. Level 70 + SmartEnhance + Best Boost Gear
    /// 2. StoneCrusher (Arcangrove 10 + Kimberly Story & Mythsong 10 + BrightOak Story & Brightoak 10 -> Buy & Rank 10)
    /// 3. Forge Helm Enhancements (Blacksmithing Rank 7 -> Vim [8824], Examen [8825], Anima [8826], Pneuma [8827])
    /// 4. ArchPaladin (Good Rank 10 -> Quests 5463-5470 incl. XansLair & SpellCrafting Rank 8 -> Buy & Rank 10)
    /// </summary>
    public bool RunBracket70()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        if (!Army.SyncArmy("STEP2D_70_START"))
            return false;

        if (
            Army.AllPlayersSatisfy(
                "SKIP_STEP2D_70_BRACKET70",
                () =>
                    Bot.Player.Level >= 70
                    && HasClassInInventory("StoneCrusher")
                    && HasRank10Class("StoneCrusher")
                    && core.isCompletedBefore(8824, log: false)
                    && core.isCompletedBefore(8825, log: false)
                    && core.isCompletedBefore(8826, log: false)
                    && core.isCompletedBefore(8827, log: false)
                    && HasClassInInventory("ArchPaladin")
                    && HasRank10Class("ArchPaladin")
            )
        )
        {
            core.Logger("Army FarmerJoe: Step 2D (Bracket 70) already completed on all accounts — skipping.");
            EnsureBestStaticBoostGearEquipped();
            return Army.SyncArmy("STEP2D_COMPLETE") && !Bot.ShouldExit;
        }

        SetClass(rankUpIfNeeded: true);
        EnsureBestStaticBoostGearEquipped();

        // 1. Level 70 + post-level-up SmartEnhance
        if (!RunBracketLevelAndEnhance("BRACKET70_LEVEL_70", 70))
            return false;

        // 2. StoneCrusher (done BEFORE ArchPaladin so /gaiazor is unlocked and Support roles get StoneCrusher for AP bosses)
        if (!RunBracket70StoneCrusher())
            return false;

        // 3. Forge Helm Enhancements (Vim, Examen, Anima, Pneuma)
        if (!RunBracket70ForgeHelms())
            return false;

        // 4. ArchPaladin (F2P chain 5463-5470 -> Buy & Rank 10)
        if (!RunBracket70ArchPaladin())
            return false;

        SetClass(rankUpIfNeeded: true);
        return Army.SyncArmy("STEP2D_COMPLETE") && !Bot.ShouldExit;
    }

    private bool RunCoordinatedGoldStep(
        string objectiveName,
        int goldAmount,
        Func<bool> alreadySatisfied
    )
    {
        bool IsGoldDone() => alreadySatisfied() || Bot.Player.Gold >= goldAmount;

        return RunCooperativeObjective(
            objectiveName,
            IsGoldDone,
            () =>
            {
                if (IsGoldDone())
                    return;

                SetClass(ClassType.Farm);
                Farm.Gold(goldAmount);
                _syncedClassType = ClassType.None;
                SetClass();
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(
                    targetPlayerName,
                    compositionType: ClassType.Farm
                );
            }
        );
    }

    private void JoinRivensylthAndTransform(CoreBots core)
    {
        core.Join("rivensylth");
        Bot.Map.GetMapItem(3935);
        core.Sleep(300);
        core.SendPackets($"%xt%zm%equipItem%{Bot.Map.RoomID}%32057%");
        core.Sleep(300);
    }

    private bool RunBracket70StoneCrusher()
    {
        CoreBots core = CoreBots.Instance;
        UnbankIfOwned("StoneCrusher", "Shaman Armor", "Earth's Song Token");

        bool HasSC() => HasAnyClassInInventory(33394) || HasClassInInventory("StoneCrusher");
        bool HasSCRank10() => HasRank10Class("StoneCrusher");

        // 2a. Verify Arcangrove Rank 10 (already completed in Bracket 30, fallback safety)
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_ARCANGROVE_REP",
                () => HasSC() || Farm.FactionRank("Arcangrove") >= 10,
                () =>
                {
                    if (HasSC() || Farm.FactionRank("Arcangrove") >= 10)
                        return;

                    SetClass(ClassType.Farm);
                    Farm.ArcangroveREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "*",
                        compositionType: ClassType.Farm,
                        targetMap: "arcangrove"
                    );
                }
            )
        )
            return false;

        // 2b. Kimberly Storyline (/stairway, /beehive, /orchestra, /palooza)
        bool IsMythsongDone() => HasSC() || Farm.FactionRank("Mythsong") >= 10;
        bool IsKimberlyDone() => IsMythsongDone() || core.isCompletedBefore(710, log: false);

        if (!RunCoordinatedKimberlyStory(IsKimberlyDone))
            return false;

        // 2c. Mythsong Rank 10 (Quest 4829 Sugar, Sugar in /beehive killing Stinger)
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_MYTHSONG_REP",
                IsMythsongDone,
                () =>
                {
                    if (IsMythsongDone())
                        return;

                    SetClass(ClassType.Farm);
                    Farm.MythsongREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Stinger",
                        compositionType: ClassType.Farm,
                        targetMap: "beehive"
                    );
                }
            )
        )
            return false;

        // 2d. BrightOak Storyline (/rivensylth, /brightoak, /elfhame, /darkheart, /gaiazor up to 4810 so /gaiazor is unlocked for both SC and AP)
        bool IsBrightOakStoryDone() => core.isCompletedBefore(4810, log: false);
        if (!RunCoordinatedBrightOakStory(IsBrightOakStoryDone))
            return false;

        // 2e. Brightoak Rank 10 via Quest 4667 (Unlocking the Guardian's Mouth, MapItem 3984 in /elfhame - pure non-combat!)
        // Avoids CoreFarms.BrightoakREP bug at L2347 (where 'while (Bot.ShouldExit && ...)' skips getting MapItem 3984).
        bool IsBrightoakRepDone() => HasSC() || Farm.FactionRank("Brightoak") >= 10;
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_BRIGHTOAK_REP",
                IsBrightoakRepDone,
                () =>
                {
                    if (IsBrightoakRepDone())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    core.Join("elfhame");
                    while (!Bot.ShouldExit && Farm.FactionRank("Brightoak") < 10)
                    {
                        core.EnsureAccept(4667);
                        core.GetMapItem(3984, 1, "elfhame");
                        core.EnsureComplete(4667);
                        Bot.Wait.ForQuestComplete(4667);
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                }
            )
        )
            return false;

        // 2f. Ensure 150,000 Gold for Shaman Armor (50k) + Earth's Song Token (50k) + Merge overhead
        if (!RunCoordinatedGoldStep("BRACKET70_SC_GOLD", 150000, HasSC))
            return false;

        // 2g. Buy StoneCrusher (33394, shopItemID: 4222) in /gaiazor Shop 1210
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BUY",
                HasSC,
                HasSC,
                () =>
                {
                    UnbankIfOwned("StoneCrusher", "Shaman Armor", "Earth's Song Token");
                    if (HasSC())
                        return;

                    Adv.BuyItem("gaiazor", 1210, 33394, shopItemID: 4222);
                    Bot.Wait.ForItemBuy();
                }
            )
        )
            return false;

        // 2h. Rank StoneCrusher to Rank 10 & refresh party composition
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_RANK_10",
                HasSCRank10,
                () =>
                {
                    if (!HasSCRank10())
                        RankUpClassForArmy("StoneCrusher");

                    _syncedClassType = ClassType.None;
                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: true);
        return !Bot.ShouldExit;
    }

    private bool RunCoordinatedKimberlyStory(
        Func<bool> skipCondition,
        string objectivePrefix = "BRACKET70_SC_KIMBERLY"
    )
    {
        CoordinatedStoryStep[] steps =
        {
            new(
                648,
                "stairway",
                "*",
                c =>
                {
                    c.EnsureAccept(648);
                    c.HuntMonster("stairway", "Grateful Undead", "Grateful Undead Shirt", 4);
                    c.HuntMonster("stairway", "Rock Lobster", "Rock Lobster Tail", 4);
                    c.EnsureComplete(648);
                }
            ),
            new(649, "stairway", "Rock Lobster", c => Story.KillQuest(649, "stairway", "Rock Lobster")),
            new(650, "stairway", "Grateful Undead", c => Story.KillQuest(650, "stairway", "Grateful Undead")),
            new(
                651,
                "stairway",
                "*",
                c =>
                {
                    c.EnsureAccept(651);
                    c.HuntMonster("stairway", "Elwood Bruise", "Defeated Elwood", 1);
                    c.HuntMonster("stairway", "Jake Bruise", "Defeated Jake", 1);
                    c.EnsureComplete(651);
                },
                ClassType.Solo
            ),
            new(658, "beehive", "Stinger", c => Story.KillQuest(658, "beehive", "Stinger")),
            new(659, "beehive", "Killer Queen Bee", c => Story.KillQuest(659, "beehive", "Killer Queen Bee"), ClassType.Solo),
            new(
                660,
                "beehive",
                "*",
                c =>
                {
                    c.EnsureAccept(660);
                    c.KillMonster("beehive", "r6", "Right", "*", "No Shoes!");
                    c.EnsureComplete(660);
                }
            ),
            new(
                661,
                "beehive",
                "*",
                c =>
                {
                    c.Join("beehive");
                    Story.ChainQuest(661);
                },
                isCombat: false
            ),
            new(
                675,
                "orchestra",
                "Mozard",
                c =>
                {
                    c.EnsureAccept(675);
                    c.KillMonster("orchestra", "R7", "Down", "Mozard", "Fire Flea", 15);
                    c.EnsureComplete(675);
                }
            ),
            new(
                676,
                "orchestra",
                "Pachelbel's Cannon ",
                c =>
                {
                    c.EnsureAccept(676);
                    c.KillMonster("orchestra", "R4", "Down", "Pachelbel's Cannon ", "Cannon Powder");
                    c.EnsureComplete(676);
                }
            ),
            new(677, "orchestra", "Mozard", c => Story.KillQuest(677, "orchestra", "Mozard")),
            new(678, "orchestra", "Faust", c => Story.KillQuest(678, "orchestra", "Faust"), ClassType.Solo),
            new(
                4827,
                "stairway",
                "*",
                c =>
                {
                    c.EnsureAccept(4827);
                    c.HuntMonster("stairway", "Rock Lobster", "Lobster Rocked", 5);
                    c.HuntMonster("stairway", "Grateful Undead", "Ungrateful Undead", 5);
                    c.EnsureComplete(4827);
                }
            ),
            new(707, "palooza", "*", c => Story.ChainQuest(707), isCombat: false),
            new(
                709,
                "palooza",
                "Pony Gary Yellow",
                c =>
                {
                    c.EnsureAccept(709);
                    c.HuntMonster("palooza", "Pony Gary Yellow", "Pony Gary Yellow Defeated");
                    c.EnsureComplete(709);
                },
                ClassType.Solo
            ),
            new(
                710,
                "palooza",
                "Kimberly",
                c =>
                {
                    c.EnsureAccept(710);
                    c.HuntMonster("palooza", "Kimberly", "Kimberly Defeated");
                    c.EnsureComplete(710);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain(
            objectivePrefix,
            steps,
            isEnabled: () => !skipCondition()
        );
    }

    private bool RunNaturePotionStep(string objectiveName, int quant, Func<bool> skipCheck)
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => skipCheck() || core.CheckInventory("Restoration of Nature Potion", quant);

        return RunCooperativeObjective(
            objectiveName,
            IsDone,
            () =>
            {
                if (IsDone())
                    return;

                core.AddDrop("Restoration of Nature Potion");
                SetClass(ClassType.Farm);
                while (!Bot.ShouldExit && !core.CheckInventory("Restoration of Nature Potion", quant))
                {
                    core.EnsureAccept(4660);
                    core.BuyItem("sandsea", 245, "Water of Life");
                    core.KillMonster("brightoak", "r2", "Left", "Bright Treeant", "Bright Ore", 3);
                    core.KillMonster("brightoak", "r2", "Left", "Wolfwood", "Herbal Remedy", 4);
                    core.EnsureComplete(4660);
                    Bot.Wait.ForPickup("Restoration of Nature Potion");
                }
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(
                    targetPlayerName,
                    "*",
                    compositionType: ClassType.Farm,
                    targetMap: "brightoak"
                );
            }
        );
    }

    private bool RunCoordinatedBrightOakStory(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        core.AddDrop(
            "Restoration of Nature Potion",
            "Paddylump's Elixir",
            "Ravinos Token I",
            "Ravinos Token II",
            "Ravinos Token III",
            "Ravinos Token IV",
            "Ravinos Token V",
            "Lapis Token I",
            "Lapis Token II",
            "Lapis Token III",
            "Sparrow's Blood"
        );

        // Part 1: Ælfred in /rivensylth (4637-4644)
        bool IsAelfredDone() => skipCondition() || core.isCompletedBefore(4644, log: false);
        CoordinatedStoryStep[] aelfredSteps =
        {
            new(
                4637,
                "rivensylth",
                "*",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.MapItemQuest(4637, "rivensylth", 3944);
                },
                isCombat: false
            ),
            new(
                4638,
                "rivensylth",
                "Cave Creeper",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.KillQuest(4638, "rivensylth", "Cave Creeper");
                },
                skipMapVerify: true
            ),
            new(
                4639,
                "rivensylth",
                "*",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.MapItemQuest(4639, "rivensylth", 3945, 4);
                },
                isCombat: false
            ),
            new(
                4640,
                "rivensylth",
                "Mushroom",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.KillQuest(4640, "rivensylth", "Mushroom");
                },
                skipMapVerify: true
            ),
            new(
                4641,
                "pines",
                "Pine Grizzly",
                c =>
                {
                    c.EnsureAccept(4641);
                    JoinRivensylthAndTransform(c);
                    c.GetMapItem(3948, 4, "rivensylth");
                    c.HuntMonster("pines", "Pine Grizzly", "Straw", 3);
                    c.EnsureComplete(4641);
                },
                skipMapVerify: true
            ),
            new(
                4642,
                "rivensylth",
                "Cave Creeper",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.KillQuest(4642, "rivensylth", "Cave Creeper");
                },
                skipMapVerify: true
            ),
            new(
                4643,
                "rivensylth",
                "*",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.MapItemQuest(4643, "rivensylth", 3946);
                },
                isCombat: false
            ),
            new(
                4644,
                "rivensylth",
                "Avada",
                c =>
                {
                    JoinRivensylthAndTransform(c);
                    Story.KillQuest(4644, "rivensylth", "Avada");
                },
                ClassType.Solo,
                skipMapVerify: true
            ),
        };

        if (!RunCoordinatedStoryChain("BRACKET70_SC_BO_AELFRED", aelfredSteps, isEnabled: () => !IsAelfredDone()))
            return false;

        // Part 2: AvenGreywhorl in /brightoak & /elfhame (4466, 4659-4668)
        bool IsAvenDone() => skipCondition() || core.isCompletedBefore(4668, log: false);

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_AVEN1",
                new CoordinatedStoryStep[]
                {
                    new(4466, "brightoak", "Bright Treeant", c => Story.KillQuest(4466, "brightoak", "Bright Treeant")),
                    new(
                        4659,
                        "elfhame",
                        "Ruin Stalker",
                        c =>
                        {
                            c.EnsureAccept(4659);
                            c.GetMapItem(3983, 1, "elfhame");
                            c.HuntMonster("elfhame", "Ruin Stalker", "Ruin Stalker Defeated", 6);
                            c.EnsureComplete(4659);
                        }
                    ),
                },
                isEnabled: () => !IsAvenDone()
            )
        )
            return false;

        // 4660 + NaturePotion(1) -> 4661
        bool Is4661Done() => IsAvenDone() || core.isCompletedBefore(4661, log: false);
        if (!RunNaturePotionStep("BRACKET70_SC_BO_POTION_1", 1, Is4661Done))
            return false;

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_4661",
                new CoordinatedStoryStep[]
                {
                    new(4661, "elfhame", "Blighted Deer ", c => Story.KillQuest(4661, "elfhame", "Blighted Deer ")),
                },
                isEnabled: () => !Is4661Done()
            )
        )
            return false;

        // 4662: requires Paddylump's Elixir (Quest 820 in /cloister) + Ruin Dweller x12 in /elfhame
        bool Is4662Done() => IsAvenDone() || core.isCompletedBefore(4662, log: false);
        bool HasElixir() => Is4662Done() || core.CheckInventory("Paddylump's Elixir");

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_820_ACORNENT",
                820,
                "cloister",
                "Acornent",
                "Wiggly Worm",
                15,
                isTemp: true,
                role: ClassType.Farm,
                beforeHunt: c => c.AddDrop("Paddylump's Elixir"),
                isEnabled: () => !HasElixir()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_820_KARASU",
                820,
                "cloister",
                "Karasu",
                "MegaMite",
                10,
                isTemp: true,
                role: ClassType.Farm,
                beforeHunt: c => c.AddDrop("Paddylump's Elixir"),
                isEnabled: () => !HasElixir()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_820_COMPLETE",
                HasElixir,
                HasElixir,
                () =>
                {
                    core.AddDrop("Paddylump's Elixir");
                    core.EnsureAccept(820);
                    core.EnsureComplete(820);
                    Bot.Wait.ForPickup("Paddylump's Elixir");
                }
            )
        )
            return false;

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_4662",
                new CoordinatedStoryStep[]
                {
                    new(
                        4662,
                        "elfhame",
                        "Ruin Dweller",
                        c =>
                        {
                            c.EnsureAccept(4662);
                            c.HuntMonster("elfhame", "Ruin Dweller", "Ruin Dweller Defeated", 12);
                            c.EnsureComplete(4662);
                        }
                    ),
                },
                isEnabled: () => !Is4662Done()
            )
        )
            return false;

        // 4663 (needs NaturePotion(2)) + 4664
        bool Is4664Done() => IsAvenDone() || core.isCompletedBefore(4664, log: false);
        bool Is4663Done() => Is4664Done() || core.isCompletedBefore(4663, log: false);
        if (!RunNaturePotionStep("BRACKET70_SC_BO_POTION_2", 2, Is4663Done))
            return false;

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_4663_4664",
                new CoordinatedStoryStep[]
                {
                    new(
                        4663,
                        "elfhame",
                        "Wolfrider",
                        c =>
                        {
                            c.EnsureAccept(4663);
                            c.HuntMonster("elfhame", "Wolfrider", "Wolfrider Maimed", 4);
                            c.EnsureComplete(4663);
                        }
                    ),
                    new(4664, "elfhame", "Ratawampus", c => Story.KillQuest(4664, "elfhame", "Ratawampus")),
                },
                isEnabled: () => !Is4664Done()
            )
        )
            return false;

        // 4665 (needs NaturePotion(3))
        bool Is4665Done() => IsAvenDone() || core.isCompletedBefore(4665, log: false);
        if (!RunNaturePotionStep("BRACKET70_SC_BO_POTION_3", 3, Is4665Done))
            return false;

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_4665",
                new CoordinatedStoryStep[]
                {
                    new(
                        4665,
                        "elfhame",
                        "*",
                        c =>
                        {
                            c.EnsureAccept(4665);
                            c.HuntMonster("elfhame", "Ratawampus", "Ratawampus Cleared", 2);
                            c.HuntMonster("elfhame", "Ruin Dweller", "Ruin Dweller Cleared", 3);
                            c.EnsureComplete(4665);
                        }
                    ),
                },
                isEnabled: () => !Is4665Done()
            )
        )
            return false;

        // 4666 (needs NaturePotion(4)), 4667, 4668
        bool Is4666Done() => IsAvenDone() || core.isCompletedBefore(4666, log: false);
        if (!RunNaturePotionStep("BRACKET70_SC_BO_POTION_4", 4, Is4666Done))
            return false;

        if (
            !RunCoordinatedStoryChain(
                "BRACKET70_SC_BO_4666_4668",
                new CoordinatedStoryStep[]
                {
                    new(
                        4666,
                        "elfhame",
                        "Ruin Stalker",
                        c =>
                        {
                            c.EnsureAccept(4666);
                            c.KillMonster("elfhame", "r2", "Left", "Ruin Stalker", "Ruin Stalker Contained", 6);
                            c.EnsureComplete(4666);
                        }
                    ),
                    new(4667, "elfhame", "*", c => Story.MapItemQuest(4667, "elfhame", 3984), isCombat: false),
                    new(4668, "elfhame", "Guardian Spirit", c => Story.KillQuest(4668, "elfhame", "Guardian Spirit"), ClassType.Solo),
                },
                isEnabled: () => !IsAvenDone()
            )
        )
            return false;

        // Part 3: FlixSpiderwhisp, LapisPart1, and RavinosBrightgladePart1 (up to 4700 Defeat the Gaiazor)
        bool IsGaiazor4700Done() => skipCondition() || core.isCompletedBefore(4700, log: false);
        CoordinatedStoryStep[] darkheartSteps =
        {
            new(4469, "brightoak", "Hootbear", c => Story.KillQuest(4469, "brightoak", "Hootbear")),
            new(
                4470,
                "brightoak",
                "Brightpool Guardian",
                c =>
                {
                    c.EnsureAccept(4470);
                    c.GetMapItem(3667, 5, "brightoak");
                    c.HuntMonster("brightoak", "Brightpool Guardian", "Brightpool Subdued", 9);
                    c.EnsureComplete(4470);
                }
            ),
            new(
                4467,
                "brightoak",
                "Grove Spore",
                c =>
                {
                    c.EnsureAccept(4467);
                    c.GetMapItem(3666, 10, "brightoak");
                    c.HuntMonster("brightoak", "Grove Spore", "Herbal Spore", 10);
                    c.EnsureComplete(4467);
                }
            ),
            new(4468, "brightoak", "Twisted Goblin", c => Story.KillQuest(4468, "brightoak", "Twisted Goblin")),
            new(
                4463,
                "brightoak",
                "*",
                c =>
                {
                    c.EnsureAccept(4463);
                    c.KillMonster("brightoak", "r2", "Left", "Wolfwood", "Corrupted Fang", 2);
                    c.KillMonster("brightoak", "r3", "Left", "Hootbear", "Shriveled Claw", 4);
                    c.KillMonster("brightoak", "r6", "Left", "Tainted Earth", "Muddy Vial", 2);
                    c.EnsureComplete(4463);
                }
            ),
            new(4692, "darkheart", "*", c => Story.MapItemQuest(4692, "darkheart", 4052), isCombat: false),
            new(4693, "darkheart", "Mutated Leech", c => Story.KillQuest(4693, "darkheart", "Mutated Leech")),
            new(4694, "darkheart", "Mutated Leech", c => Story.KillQuest(4694, "darkheart", "Mutated Leech")),
            new(
                4695,
                "darkheart",
                "Wisterrora",
                c =>
                {
                    c.EnsureAccept(4695);
                    c.GetMapItem(4053, 6, "darkheart");
                    c.HuntMonster("darkheart", "Wisterrora", "Wisterrora Defeated", 5);
                    c.EnsureComplete(4695);
                }
            ),
            new(
                4696,
                "darkheart",
                "Toxic Grove Spider",
                c =>
                {
                    c.EnsureAccept(4696);
                    c.GetMapItem(4054, 6, "darkheart");
                    c.HuntMonster("darkheart", "Toxic Grove Spider", "Toxic Grove Spider Defeated", 7);
                    c.EnsureComplete(4696);
                }
            ),
            new(
                4697,
                "brightoak",
                "Brightpool Guardian",
                c =>
                {
                    c.EnsureAccept(4697);
                    c.GetMapItem(4055, 7, "darkheart");
                    c.HuntMonster("brightoak", "Brightpool Guardian", "Brightpool Core Retrieved", 7);
                    c.EnsureComplete(4697);
                },
                skipMapVerify: true
            ),
            new(
                4698,
                "darkheart",
                "*",
                c =>
                {
                    c.EnsureAccept(4698);
                    c.KillMonster("darkheart", "Enter", "Spawn", "Tainted Earth", "Tainted Earth Removed", 8);
                    c.KillMonster("darkheart", "r2", "Left", "Toxic Grove Spider", "Toxic Grove Spider Dispatched", 5);
                    c.KillMonster("darkheart", "Enter", "Spawn", "Mutated Leech", "Mutated Leech Slain", 6);
                    c.EnsureComplete(4698);
                }
            ),
            new(4699, "darkheart", "*", c => Story.MapItemQuest(4699, "darkheart", 4056), isCombat: false),
            new(4700, "darkheart", "Gaiazor", c => Story.KillQuest(4700, "darkheart", "Gaiazor"), ClassType.Solo),
        };

        if (!RunCoordinatedStoryChain("BRACKET70_SC_BO_DARKHEART", darkheartSteps, isEnabled: () => !IsGaiazor4700Done()))
            return false;

        // Part 4: Gaiazor Pastoral Quests (4799-4810) so Nevanna & Gaiazor in /gaiazor are unlocked for ArchPaladin 5470
        bool IsRavinos2Done() => skipCondition() || core.isCompletedBefore(4804, log: false);

        if (
            !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_4799",
                () => IsRavinos2Done() || core.CheckInventory("Ravinos Token I") || core.isCompletedBefore(4799, log: false),
                () => IsRavinos2Done() || core.CheckInventory("Ravinos Token I") || core.isCompletedBefore(4799, log: false),
                () =>
                {
                    core.AddDrop("Ravinos Token I");
                    core.EnsureAccept(4799);
                    core.GetMapItem(4204, 6, "gaiazor");
                    core.EnsureComplete(4799);
                    Bot.Wait.ForPickup("Ravinos Token I");
                }
            )
        )
            return false;

        bool HasRavinosII() => IsRavinos2Done() || core.CheckInventory("Ravinos Token II") || core.isCompletedBefore(4800, log: false);
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_BO_4800",
                HasRavinosII,
                () =>
                {
                    if (HasRavinosII())
                        return;

                    core.AddDrop("Ravinos Token II");
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(4800);
                    core.HuntMonster("gaiazor", "Wolfwood", "Wolfwood Slain", 4);
                    core.HuntMonster("gaiazor", "Wisterrora", "Wisterrora Slain", 4);
                    core.HuntMonster("gaiazor", "Tree Golem", "Tree Golem Slain", 4);
                    core.EnsureComplete(4800);
                    Bot.Wait.ForPickup("Ravinos Token II");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm, targetMap: "gaiazor");
                }
            )
        )
            return false;

        bool HasRavinosIII() => IsRavinos2Done() || core.CheckInventory("Ravinos Token III") || core.isCompletedBefore(4801, log: false);
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_BO_4801",
                HasRavinosIII,
                () =>
                {
                    if (HasRavinosIII())
                        return;

                    core.AddDrop("Ravinos Token III");
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(4801);
                    core.HuntMonster("gaiazor", "Tree Golem", "Lapis' Runestones");
                    core.HuntMonster("gaiazor", "Wolfwood", "Flix's Fertilizer");
                    core.HuntMonster("gaiazor", "Wisterrora", "Zephyr's Toolkit");
                    core.EnsureComplete(4801);
                    Bot.Wait.ForPickup("Ravinos Token III");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm, targetMap: "gaiazor");
                }
            )
        )
            return false;

        bool HasRavinosIV() => IsRavinos2Done() || core.CheckInventory("Ravinos Token IV") || core.isCompletedBefore(4802, log: false);
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_BO_4802",
                HasRavinosIV,
                () =>
                {
                    if (HasRavinosIV())
                        return;

                    core.AddDrop("Ravinos Token IV");
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(4802);
                    core.HuntMonster("gaiazor", "Tree Golem", "Tree Golem Roots", 5);
                    core.HuntMonster("gaiazor", "Wisterrora", "Wisterrora Thorns", 5);
                    core.GetMapItem(4205, 5, "gaiazor");
                    core.EnsureComplete(4802);
                    Bot.Wait.ForPickup("Ravinos Token IV");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm, targetMap: "gaiazor");
                }
            )
        )
            return false;

        bool HasRavinosV() => IsRavinos2Done() || core.CheckInventory("Ravinos Token V") || core.isCompletedBefore(4803, log: false);
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_4803_DARKHEART",
                4803,
                "darkheart",
                "Toxic Grove Spider",
                "Grove Spider Silk",
                6,
                isTemp: true,
                role: ClassType.Farm,
                beforeHunt: c => c.AddDrop("Ravinos Token V"),
                isEnabled: () => !HasRavinosV()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_4803_BLOODTUSK",
                4803,
                "bloodtusk",
                "Trollola Plant",
                "Trollola Nectar",
                5,
                isTemp: true,
                role: ClassType.Farm,
                beforeHunt: c => c.AddDrop("Ravinos Token V"),
                isEnabled: () => !HasRavinosV()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_4803_FIRESTORM",
                4803,
                "firestorm",
                "Sulfur Imp",
                "Searbush",
                2,
                isTemp: true,
                role: ClassType.Farm,
                beforeHunt: c => c.AddDrop("Ravinos Token V"),
                isEnabled: () => !HasRavinosV()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_4803_COMPLETE",
                HasRavinosV,
                HasRavinosV,
                () =>
                {
                    core.AddDrop("Ravinos Token V");
                    core.EnsureAccept(4803);
                    core.EnsureComplete(4803);
                    Bot.Wait.ForPickup("Ravinos Token V");
                }
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_4804",
                IsRavinos2Done,
                IsRavinos2Done,
                () => Story.MapItemQuest(4804, "gaiazor", 4206)
            )
        )
            return false;

        // LapisPart2 (4805-4808) + RavinosBrightgladePart3 (4809-4810)
        bool IsLapis2Done() => skipCondition() || core.isCompletedBefore(4808, log: false);
        bool HasLapisI() => IsLapis2Done() || core.CheckInventory("Lapis Token I") || core.isCompletedBefore(4805, log: false);
        if (
            !RunCooperativeObjective(
                "BRACKET70_SC_BO_4805",
                HasLapisI,
                () =>
                {
                    if (HasLapisI())
                        return;

                    core.AddDrop("Lapis Token I");
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(4805);
                    core.HuntMonster("gaiazor", "Wolfwood", "Wolfwood Twigs", 7);
                    core.HuntMonster("gaiazor", "Wisterrora", "Drop of Wisterrora Ichor");
                    core.EnsureComplete(4805);
                    Bot.Wait.ForPickup("Lapis Token I");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm, targetMap: "gaiazor");
                }
            )
        )
            return false;

        // 4806: Outside Assistance requires Sparrow's Blood (Quest 803 in /arcangrove)
        bool HasLapisII() => IsLapis2Done() || core.CheckInventory("Lapis Token II") || core.isCompletedBefore(4806, log: false);
        bool HasSparrowsBlood() => HasLapisII() || core.CheckInventory("Sparrow's Blood");

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_803_LILY",
                803,
                "arcangrove",
                "*",
                "Blood Lily",
                30,
                isTemp: true,
                role: ClassType.Farm,
                cell: "LeftBack",
                pad: "Left",
                beforeHunt: c => c.AddDrop("Sparrow's Blood", "Lapis Token II"),
                isEnabled: () => !HasSparrowsBlood()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_803_SNAPDRAKE",
                803,
                "arcangrove",
                "*",
                "Snapdrake",
                17,
                isTemp: true,
                role: ClassType.Farm,
                cell: "RightBack",
                pad: "Left",
                beforeHunt: c => c.AddDrop("Sparrow's Blood", "Lapis Token II"),
                isEnabled: () => !HasSparrowsBlood()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_SC_BO_803_DIRT",
                803,
                "arcangrove",
                "*",
                "DOOM Dirt",
                12,
                isTemp: true,
                role: ClassType.Farm,
                cell: "Back",
                pad: "Left",
                beforeHunt: c => c.AddDrop("Sparrow's Blood", "Lapis Token II"),
                isEnabled: () => !HasSparrowsBlood()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_4806_COMPLETE",
                HasLapisII,
                HasLapisII,
                () =>
                {
                    core.AddDrop("Sparrow's Blood", "Lapis Token II");
                    if (!core.CheckInventory("Sparrow's Blood"))
                    {
                        core.EnsureAccept(803);
                        core.EnsureComplete(803);
                        Bot.Wait.ForPickup("Sparrow's Blood");
                    }
                    core.EnsureAccept(4806);
                    core.EnsureComplete(4806);
                    Bot.Wait.ForPickup("Lapis Token II");
                }
            )
        )
            return false;

        bool HasLapisIII() => IsLapis2Done() || core.CheckInventory("Lapis Token III") || core.isCompletedBefore(4807, log: false);
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET70_SC_BO_4807_4808",
                IsLapis2Done,
                IsLapis2Done,
                () =>
                {
                    core.AddDrop("Lapis Token III");
                    core.EnsureAccept(4808);
                    if (!HasLapisIII())
                    {
                        core.EnsureAccept(4807);
                        core.GetMapItem(4207, 1, "gaiazor");
                        core.GetMapItem(4208, 1, "gaiazor");
                        core.GetMapItem(4209, 1, "gaiazor");
                        core.EnsureComplete(4807);
                        Bot.Wait.ForPickup("Lapis Token III");
                    }
                    Story.MapItemQuest(4808, "gaiazor", 4210);
                }
            )
        )
            return false;

        CoordinatedStoryStep[] finalGaiazorSteps =
        {
            new(4809, "gaiazor", "Nevanna", c => Story.KillQuest(4809, "gaiazor", "Nevanna"), ClassType.Solo),
            new(4810, "gaiazor", "Gaiazor", c => Story.KillQuest(4810, "gaiazor", "Gaiazor"), ClassType.Solo),
        };

        return RunCoordinatedStoryChain(
            "BRACKET70_SC_BO_GAIAZOR_BOSS",
            finalGaiazorSteps,
            isEnabled: () => !skipCondition()
        );
    }

    private bool RunCoordinatedBlacksmithingRankStep(
        string objectivePrefix,
        int targetRank,
        int maxRounds,
        Func<bool> alreadySatisfied
    )
    {
        CoreBots core = CoreBots.Instance;
        bool HasTargetRank() => alreadySatisfied() || Farm.FactionRank("Blacksmithing") >= targetRank;

        if (!RunBlacksmithingRank4Step($"{objectivePrefix}_RANK4", HasTargetRank))
            return false;

        for (int round = 1; round <= maxRounds; round++)
        {
            int currentRound = round;
            bool roundCompleted = false;

            if (
                !RunCooperativeObjective(
                    $"{objectivePrefix}_BS8736_R{currentRound}_HYDRA",
                    () => HasTargetRank() || core.CheckInventory("Hydra Scale Piece", 75),
                    () =>
                    {
                        if (HasTargetRank() || core.CheckInventory("Hydra Scale Piece", 75))
                            return;

                        core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                        core.EnsureAccept(8736);
                        SetClass(ClassType.Solo);
                        core.HuntMonster(
                            "hydrachallenge",
                            "Hydra Head 25",
                            "Hydra Scale Piece",
                            75,
                            isTemp: false,
                            log: false
                        );
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Hydra Head 25",
                            targetMap: "hydrachallenge"
                        );
                    }
                )
            )
                return false;

            if (
                !RunCooperativeObjective(
                    $"{objectivePrefix}_BS8736_R{currentRound}_MAUL",
                    () => HasTargetRank() || core.CheckInventory("Creature Shard", 1),
                    () =>
                    {
                        if (HasTargetRank() || core.CheckInventory("Creature Shard", 1))
                            return;

                        core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                        core.EnsureAccept(8736);
                        SetClass(ClassType.Solo);
                        core.HuntMonster(
                            "maul",
                            "Creature Creation",
                            "Creature Shard",
                            1,
                            isTemp: false,
                            log: false
                        );
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Creature Creation",
                            targetMap: "maul"
                        );
                    }
                )
            )
                return false;

            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        $"{objectivePrefix}_BS8736_R{currentRound}_KLUNK",
                        () => HasTargetRank() || roundCompleted,
                        () =>
                        {
                            if (HasTargetRank() || roundCompleted)
                                return;

                            ApplyCurrentArmyRoomSettings();
                            core.AddDrop("Hydra Scale Piece", "Creature Shard", "Monster Trophy");
                            core.EnsureAccept(8736);
                            SetClass(ClassType.Solo);
                            core.HuntMonster(
                                "towerofdoom",
                                "Dread Klunk",
                                "Monster Trophy",
                                15,
                                isTemp: false,
                                log: false
                            );
                            LeaveCombatRoomForEquipmentChange();
                            core.EnsureComplete(8736);
                            roundCompleted = true;
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(
                                targetPlayerName,
                                "Dread Klunk",
                                targetMap: "towerofdoom"
                            );
                        },
                        prepareForCombat: () => ApplyCurrentArmyRoomSettings()
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }

        return !Bot.ShouldExit;
    }

    private bool RunStarterClassRank10ForForgeStep(
        string objectivePrefix,
        string className,
        int classhallaShopId,
        Func<bool> isForgeQuestDone
    )
    {
        CoreBots core = CoreBots.Instance;
        UnbankIfOwned(className);

        bool HasStarterClass() => isForgeQuestDone() || HasClassInInventory(className);
        bool HasStarterRank10() => isForgeQuestDone() || HasRank10Class(className);

        if (
            !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_BUY",
                HasStarterClass,
                HasStarterClass,
                () =>
                {
                    UnbankIfOwned(className);
                    if (HasStarterClass())
                        return;

                    core.BuyItem("classhalla", classhallaShopId, className);
                    Bot.Wait.ForItemBuy();
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_RANK10",
                HasStarterRank10,
                () =>
                {
                    if (!HasStarterRank10())
                        RankUpClassForArmy(className);

                    _syncedClassType = ClassType.None;
                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*");
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass();
        return !Bot.ShouldExit;
    }

    private bool RunSingleForgeHelmUnlockStep(
        string objectivePrefix,
        int questId,
        string starterClassName,
        int classhallaShopId,
        int etherealEssenceQuant
    )
    {
        CoreBots core = CoreBots.Instance;
        bool IsQuestDone() => core.isCompletedBefore(questId, log: false);

        if (
            !RunStarterClassRank10ForForgeStep(
                $"{objectivePrefix}_CLASS",
                starterClassName,
                classhallaShopId,
                IsQuestDone
            )
        )
            return false;

        ApplySubgroupRoomSettings(4);
        try
        {
            if (
                !RunCoordinatedHuntStep(
                    $"{objectivePrefix}_ESSENCE",
                    questId,
                    "towerofdoom10",
                    "*",
                    "Ethereal Essence",
                    etherealEssenceQuant,
                    isTemp: false,
                    role: ClassType.Farm,
                    cell: "r8",
                    pad: "Left",
                    softRetryLimit: 0,
                    beforeHunt: _ =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        UnbankIfOwned(starterClassName, "Ethereal Essence");
                    },
                    isEnabled: () => !IsQuestDone()
                )
            )
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        return RunCoordinatedTurnInStep(
            $"{objectivePrefix}_COMPLETE",
            IsQuestDone,
            IsQuestDone,
            () =>
            {
                UnbankIfOwned(starterClassName, "Ethereal Essence");
                core.EnsureAccept(questId);
                core.EnsureComplete(questId);
                core.Logger($"Enhancement Unlocked: {objectivePrefix} [{questId}]");
            }
        );
    }

    private bool RunBracket70ForgeHelms()
    {
        CoreBots core = CoreBots.Instance;
        bool AllHelmsDone() =>
            core.isCompletedBefore(8824, log: false)
            && core.isCompletedBefore(8825, log: false)
            && core.isCompletedBefore(8826, log: false)
            && core.isCompletedBefore(8827, log: false);

        // 3a. Blacksmithing Rank 7 (up to 20 shared rounds of Quest 8736 from Rank 5 to Rank 7)
        if (
            !RunCoordinatedBlacksmithingRankStep(
                "BRACKET70_FORGE_HELM_BS7",
                7,
                22,
                AllHelmsDone
            )
        )
            return false;

        // 3b. Vim [8824] (Rogue Rank 10 + 250x Ethereal Essence)
        if (!RunSingleForgeHelmUnlockStep("BRACKET70_FORGE_VIM", 8824, "Rogue", 172, 250))
            return false;

        // 3c. Examen [8825] (Healer Rank 10 + 250x Ethereal Essence)
        if (!RunSingleForgeHelmUnlockStep("BRACKET70_FORGE_EXAMEN", 8825, "Healer", 176, 250))
            return false;

        // 3d. Anima [8826] (Warrior Rank 10 + 650x Ethereal Essence)
        if (!RunSingleForgeHelmUnlockStep("BRACKET70_FORGE_ANIMA", 8826, "Warrior", 170, 650))
            return false;

        // 3e. Pneuma [8827] (Mage Rank 10 + 650x Ethereal Essence)
        if (!RunSingleForgeHelmUnlockStep("BRACKET70_FORGE_PNEUMA", 8827, "Mage", 174, 650))
            return false;

        // 3f. Re-run SmartEnhance so all accounts apply their unlocked Forge Helm enhancements
        return RunCoordinatedTurnInStep(
            "BRACKET70_FORGE_HELM_ENHANCE",
            () => false,
            () => true,
            () =>
            {
                if (!ShouldPreserveEquippedGear() && !IsDisableAutoEnhanceEnabled())
                {
                    string? currentClass = Bot.Player.CurrentClass?.Name;
                    if (!string.IsNullOrWhiteSpace(currentClass))
                    {
                        ApplyAndRecordSmartEnhance(currentClass);
                        Army.StartClassSkills(currentClass);
                    }
                }
            }
        );
    }

    private void KillDoomKittenForArmy(string itemName, int quant = 1, bool isTemp = true)
    {
        CoreBots core = CoreBots.Instance;
        if (isTemp ? Bot.TempInv.Contains(itemName, quant) : core.CheckInventory(itemName, quant))
            return;

        if (!isTemp)
            core.AddDrop(itemName);

        LeaveCombatRoomForEquipmentChange();
        UnbankIfOwned("DragonSoul Shinobi", "Shadow Dragon Shinobi", "Blaze Binder", "StoneCrusher");

        string chosenClass = "StoneCrusher";
        string skillRotation = "3 | 1 | 4";

        if (HasClassInInventory("DragonSoul Shinobi"))
        {
            chosenClass = "DragonSoul Shinobi";
            skillRotation = "4 | 1 | 3M<30 | 2H<30";
        }
        else if (HasClassInInventory("Blaze Binder"))
        {
            chosenClass = "Blaze Binder";
            skillRotation = "3 | 4 | 2";
        }

        if (HasClassInInventory(chosenClass))
        {
            EnsureCombatEnhancementReady(chosenClass, allowFullGearEnhance: true);

            core.Equip(chosenClass);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(chosenClass), 20);
        }

        core.Join("doomkitten");
        Bot.Skills.StartAdvanced(skillRotation);

        while (
            !Bot.ShouldExit
            && !(isTemp ? Bot.TempInv.Contains(itemName, quant) : core.CheckInventory(itemName, quant))
        )
        {
            if (!Bot.Player?.HasTarget ?? true || Bot.Player?.Target?.HP > 0)
                Bot.Combat.Attack("*");

            Bot.Sleep(500);
        }

        _syncedClassType = ClassType.None;
    }

    private bool RunCoordinatedXansLairStory(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;

        // 1. Volcano (1242-1261)
        bool IsVolcanoDone() => skipCondition() || core.isCompletedBefore(1261, log: false);
        CoordinatedStoryStep[] volcanoSteps =
        {
            new(1242, "volcano", "Lava Golem", c => Story.KillQuest(1242, "volcano", "Lava Golem")),
            new(
                1243,
                "volcano",
                "Fire Imp",
                c =>
                {
                    c.EnsureAccept(1243);
                    c.GetMapItem(526, 3, "volcano");
                    c.HuntMonster("volcano", "Fire Imp", "Broiled Clue", 7);
                    c.EnsureComplete(1243);
                }
            ),
            new(1244, "volcano", "Fire Imp", c => Story.KillQuest(1244, "volcano", "Fire Imp")),
            new(
                1245,
                "volcano",
                "Lava Golem",
                c =>
                {
                    c.EnsureAccept(1245);
                    c.GetMapItem(527, 8, "volcano");
                    c.HuntMonster("volcano", "Lava Golem", "Blazing Clue", 10);
                    c.EnsureComplete(1245);
                }
            ),
            new(1246, "volcano", "Fire Imp", c => Story.KillQuest(1246, "volcano", "Fire Imp")),
            new(1247, "volcano", "Fire Imp", c => Story.KillQuest(1247, "volcano", "Fire Imp")),
            new(1248, "volcano", "*", c => Story.MapItemQuest(1248, "volcano", 528, 12), isCombat: false),
            new(1249, "volcano", "Flamethrower Dwakel", c => Story.KillQuest(1249, "volcano", "Flamethrower Dwakel")),
            new(
                1250,
                "volcano",
                "Fire Imp",
                c =>
                {
                    c.EnsureAccept(1250);
                    c.GetMapItem(529, 8, "volcano");
                    c.HuntMonster("volcano", "Fire Imp", "Torched Clue", 10);
                    c.EnsureComplete(1250);
                }
            ),
            new(1251, "volcano", "Flamethrower Dwakel", c => Story.KillQuest(1251, "volcano", "Flamethrower Dwakel")),
            new(
                1252,
                "volcano",
                "Fire Imp",
                c =>
                {
                    c.EnsureAccept(1252);
                    c.GetMapItem(530, 10, "volcano");
                    c.HuntMonster("volcano", "Fire Imp", "Melted Clue", 10);
                    c.EnsureComplete(1252);
                }
            ),
            new(1253, "volcano", "Flame Elemental", c => Story.KillQuest(1253, "volcano", "Flame Elemental")),
            new(1254, "volcano", "*", c => Story.MapItemQuest(1254, "volcano", 531, 12), isCombat: false),
            new(1255, "volcano", "Fire Imp", c => Story.KillQuest(1255, "volcano", "Fire Imp")),
            new(1256, "volcano", "Fire Imp", c => Story.KillQuest(1256, "volcano", "Fire Imp")),
            new(1257, "volcano", "*", c => Story.MapItemQuest(1257, "volcano", 532, 10), isCombat: false),
            new(1258, "volcano", "Flamethrower Dwakel", c => Story.KillQuest(1258, "volcano", "Flamethrower Dwakel")),
            new(
                1259,
                "volcano",
                "Fire Imp",
                c =>
                {
                    c.EnsureAccept(1259);
                    c.HuntMonster("volcano", "Fire Imp", "Singed Clue", 15);
                    c.EnsureComplete(1259);
                }
            ),
            new(
                1260,
                "volcano",
                "Fire Imp",
                c =>
                {
                    c.EnsureAccept(1260);
                    c.GetMapItem(533, 6, "volcano");
                    c.HuntMonster("volcano", "Fire Imp", "Smoking Clue", 13);
                    c.EnsureComplete(1260);
                }
            ),
            new(1261, "volcano", "Magman", c => Story.KillQuest(1261, "volcano", "Magman"), ClassType.Solo),
        };

        if (!RunCoordinatedStoryChain("BRACKET70_AP_XAN_VOLCANO", volcanoSteps, isEnabled: () => !IsVolcanoDone()))
            return false;

        // 2. Xantown (1733-1740) - always jump to safe cell r12, Left after completing in /xantown
        bool IsXantownDone() => skipCondition() || core.isCompletedBefore(1740, log: false);
        CoordinatedStoryStep[] xantownSteps =
        {
            new(1733, "xantown", "*", c => Story.MapItemQuest(1733, "xantown", 922), isCombat: false),
            new(1734, "xantown", "Fire Imp", c => Story.KillQuest(1734, "xantown", "Fire Imp")),
            new(1735, "xantown", "Fire Imp", c => Story.KillQuest(1735, "xantown", "Fire Imp")),
            new(1736, "xantown", "Fire Imp", c => Story.KillQuest(1736, "xantown", "Fire Imp")),
            new(1737, "xantown", "*", c => Story.MapItemQuest(1737, "xantown", 923, 4), isCombat: false),
            new(1738, "xantown", "*", c => Story.MapItemQuest(1738, "xantown", 924), isCombat: false),
            new(
                1739,
                "xantown",
                "*",
                c =>
                {
                    Story.MapItemQuest(1739, "xantown", 925);
                    if (Bot.Map.Name == "xantown")
                        c.Jump("r12", "Left");
                },
                isCombat: false
            ),
            new(
                1740,
                "xantown",
                "Xan",
                c =>
                {
                    Story.KillQuest(1740, "xantown", "Xan");
                    if (Bot.Map.Name == "xantown")
                        c.Jump("r12", "Left");
                },
                ClassType.Solo
            ),
        };

        if (!RunCoordinatedStoryChain("BRACKET70_AP_XAN_TOWN", xantownSteps, isEnabled: () => !IsXantownDone()))
            return false;

        // 3. Xancave (2151-2157)
        bool IsXancaveDone() => skipCondition() || core.isCompletedBefore(2157, log: false);
        CoordinatedStoryStep[] xancaveSteps =
        {
            new(2151, "xancave", "*", c => Story.MapItemQuest(2151, "xancave", 1220), isCombat: false),
            new(2152, "xancave", "Lava Goblin", c => Story.KillQuest(2152, "xancave", "Lava Goblin")),
            new(2153, "xancave", "*", c => Story.MapItemQuest(2153, "xancave", 1221), isCombat: false),
            new(2154, "xancave", "Lava Goblin", c => Story.KillQuest(2154, "xancave", "Lava Goblin")),
            new(
                2155,
                "xancave",
                "Lava Goblin",
                c =>
                {
                    c.EnsureAccept(2155);
                    c.GetMapItem(1223, 8, "xancave");
                    c.HuntMonster("xancave", "Lava Goblin", "Fire Salts", 10);
                    c.HuntMonster("xancave", "Lava Goblin", "Fire Wire", 10);
                    c.EnsureComplete(2155);
                }
            ),
            new(2156, "xancave", "Shurpu Ring Guardian", c => Story.KillQuest(2156, "xancave", "Shurpu Ring Guardian"), ClassType.Solo),
            new(2157, "xancave", "*", c => Story.MapItemQuest(2157, "xancave", 1222), isCombat: false),
        };

        return RunCoordinatedStoryChain("BRACKET70_AP_XAN_CAVE", xancaveSteps, isEnabled: () => !IsXancaveDone());
    }

    private bool RunCoordinatedSpellCraftingRank8AndSlumber(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        UnbankIfOwned("Scroll of Ethereal Slumber", "Archmage Ink", "Mystic Quills", "Mystic Parchment", "Mystic Shards");

        bool HasSlumber() => skipCondition() || core.CheckInventory("Scroll of Ethereal Slumber");
        bool HasRank8() => HasSlumber() || core.CheckInventory("Archmage Ink") || Farm.FactionRank("SpellCrafting") >= 8;
        bool HasRank4() => HasRank8() || Farm.FactionRank("SpellCrafting") >= 4;
        bool HasRank1() => HasRank4() || Farm.FactionRank("SpellCrafting") >= 1;

        // 1. Unlock SpellCrafting (Quest 2260) if Rank 0
        if (
            !RunCooperativeObjective(
                "BRACKET70_AP_SC_UNLOCK_2260",
                HasRank1,
                () =>
                {
                    if (HasRank1())
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAccept(2260);
                    core.GetMapItem(1920, 1, "dragonrune");
                    core.HuntMonster("castleundead", "Skeletal Warrior", "Arcane Parchment", 13, log: false);
                    core.EnsureComplete(2260);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Skeletal Warrior",
                        compositionType: ClassType.Farm,
                        targetMap: "castleundead"
                    );
                }
            )
        )
            return false;

        // 2. Rank 1 -> Rank 4 via Mystic Quills (Slugfit in /mobius) + Ember Ink (2299)
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_SC_QUILLS_R4",
                0,
                "mobius",
                "Slugfit",
                "Mystic Quills",
                10,
                isTemp: false,
                role: ClassType.Farm,
                acceptQuest: false,
                isEnabled: () => !HasRank4()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_SC_TURNIN_R4",
                HasRank4,
                HasRank4,
                () =>
                {
                    core.BuyItem("dragonrune", 549, "Ember Ink", 50);
                    while (!Bot.ShouldExit && core.CheckInventory("Ember Ink") && Farm.FactionRank("SpellCrafting") < 4)
                        core.ChainComplete(2299);
                    core.SellItem("Ember Ink", all: true);
                }
            )
        )
            return false;

        // 3. Rank 4 -> Rank 8 via Mystic Parchment (Skull Warrior in /underworld) + Hallow Ink (2322)
        if (
            !RunCooperativeObjective(
                "BRACKET70_AP_SC_RANK8",
                HasRank8,
                () =>
                {
                    if (HasRank8())
                        return;

                    core.AddDrop("Mystic Parchment", "Mystic Shards");
                    while (!Bot.ShouldExit && Farm.FactionRank("SpellCrafting") < 8)
                    {
                        SetClass(ClassType.Farm);
                        core.HuntMonster("underworld", "Skull Warrior", "Mystic Parchment", 10, isTemp: false);
                        LeaveCombatRoomForEquipmentChange();
                        core.BuyItem("dragonrune", 549, "Hallow Ink", 50);
                        while (!Bot.ShouldExit && core.CheckInventory("Hallow Ink") && Farm.FactionRank("SpellCrafting") < 8)
                            core.ChainComplete(2322);
                    }
                    core.SellItem("Hallow Ink", all: true);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Skull Warrior",
                        compositionType: ClassType.Farm,
                        targetMap: "underworld"
                    );
                }
            )
        )
            return false;

        // 4. Farm 2x Mystic Shards from Skull Warrior in /underworld -> Buy Archmage Ink -> Complete 2347 for Scroll of Ethereal Slumber
        bool HasArchmageInkOrShards() =>
            HasSlumber() || core.CheckInventory("Archmage Ink") || core.CheckInventory("Mystic Shards", 2);

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_SC_SHARDS",
                0,
                "underworld",
                "Skull Warrior",
                "Mystic Shards",
                2,
                isTemp: false,
                role: ClassType.Farm,
                acceptQuest: false,
                isEnabled: () => !HasArchmageInkOrShards()
            )
        )
            return false;

        return RunCoordinatedTurnInStep(
            "BRACKET70_AP_SC_SLUMBER_2347",
            HasSlumber,
            HasSlumber,
            () =>
            {
                core.AddDrop("Scroll of Ethereal Slumber");
                if (!core.CheckInventory("Archmage Ink"))
                {
                    if (core.CheckInventory("Mystic Shards", 2))
                        core.BuyItem("dragonrune", 549, "Archmage Ink", 1);

                    if (!core.CheckInventory("Archmage Ink"))
                    {
                        if (!core.CheckInventory("Arcane Quill"))
                        {
                            if (Bot.Player.Gold < 100000)
                                Farm.Gold(100000);
                            core.BuyItem("spellcraft", 693, "Gold Voucher 100k");
                            core.BuyItem("spellcraft", 693, 17391, 1, shopItemID: 7685);
                        }
                        core.BuyItem("spellcraft", 622, "Archmage Ink", 1);
                    }
                }
                core.ChainComplete(2347);
                Bot.Wait.ForPickup("Scroll of Ethereal Slumber");
                core.SellItem("Archmage Ink", all: true);
                core.SellItem("Mystic Shards", all: true);
            }
        );
    }

    private bool RunBracket70ArchPaladin()
    {
        CoreBots core = CoreBots.Instance;
        UnbankIfOwned("ArchPaladin", "Stone Paladin Armor", "Exalted Paladin Seal");
        core.EnsureLoad(5463, 5464, 5465, 5466, 5467, 5468, 5469, 5470);

        bool HasAP() => HasAnyClassInInventory(36920) || HasClassInInventory("ArchPaladin");
        bool HasAPRank10() => HasRank10Class("ArchPaladin");

        // 4a. Good Rank 10
        bool IsGoodRank4() => HasAP() || Farm.FactionRank("Good") >= 4;
        bool IsGoodRank10() => HasAP() || Farm.FactionRank("Good") >= 10;

        if (
            !RunCooperativeObjective(
                "BRACKET70_AP_GOOD_REP_4",
                IsGoodRank4,
                () =>
                {
                    if (IsGoodRank4())
                        return;

                    core.ChangeAlignment(Alignment.Good);
                    SetClass(ClassType.Farm);
                    Farm.GoodREP(4);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "*",
                        compositionType: ClassType.Farm,
                        targetMap: "swordhavenbridge"
                    );
                }
            )
            || !RunCooperativeObjective(
                "BRACKET70_AP_GOOD_REP_10",
                IsGoodRank10,
                () =>
                {
                    if (IsGoodRank10())
                        return;

                    core.ChangeAlignment(Alignment.Good);
                    SetClass(ClassType.Farm);
                    Farm.GoodREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "*",
                        compositionType: ClassType.Farm,
                        targetMap: "castleundead"
                    );
                }
            )
        )
            return false;

        // 4b. Quest 5463: A Strong Base (Stone Paladin Armor in /temple Shop 288 + Exalted Paladin Seal in /darkthronehub Shop 1308 for 500k Gold)
        bool Is5463Done() => HasAP() || core.isCompletedBefore(5463, log: false) || Bot.Quests.IsUnlocked(5467);
        if (
            !RunCoordinatedGoldStep("BRACKET70_AP_5463_GOLD", 550000, Is5463Done)
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5463_COMPLETE",
                Is5463Done,
                Is5463Done,
                () =>
                {
                    Story.BuyQuest(5463, "temple", 288, "Stone Paladin Armor");
                    if (!Is5463Done())
                    {
                        if (Bot.Player.Gold < 500000)
                            Farm.Gold(500000);
                        Story.BuyQuest(5463, "darkthronehub", 1308, "Exalted Paladin Seal");
                    }
                }
            )
        )
            return false;

        // 4c. Quest 5464: Proof of Valor
        bool Is5464Done() => HasAP() || core.isCompletedBefore(5464, log: false) || Bot.Quests.IsUnlocked(5467);
        if (!EnsureLoyalFollowerUnlocked("BRACKET70_AP"))
            return false;

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5464_UNDEAD_ENERGY",
                5464,
                "battleunderb",
                "*",
                "Undead Energy",
                1000,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                beforeHunt: _ => UnbankIfOwned("Undead Energy"),
                isEnabled: () => !Is5464Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5464_BINKY",
                5464,
                "doomvault",
                "Binky",
                "Binky's Uni-horn",
                1,
                isTemp: false,
                role: ClassType.Solo,
                cell: "r5",
                pad: "Left",
                softRetryLimit: 0,
                beforeHunt: _ => UnbankIfOwned("Binky's Uni-horn"),
                isEnabled: () => !Is5464Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5464_MOYA",
                5464,
                "banished",
                "Desterrat Moya",
                "Desterrat Moya Tentacle",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5464Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5464_DREADHAVEN",
                5464,
                "dreadfight",
                "*",
                "Dreadhaven Helm",
                1,
                isTemp: true,
                role: ClassType.Solo,
                cell: "Enter",
                pad: "Spawn",
                softRetryLimit: 0,
                isEnabled: () => !Is5464Done()
            )
        )
            return false;

        // DoomKitten Claw (anti-crit fight using DragonSoul Shinobi / Blaze Binder / StoneCrusher DoT rotation "3 | 1 | 4", room cap 6 -> subgroup 4)
        bool HasDoomKittenClaw() => Is5464Done() || Bot.TempInv.Contains("DoomKitten Claw");
        ApplySubgroupRoomSettings(4);
        try
        {
            if (
                !RunCooperativeObjective(
                    "BRACKET70_AP_5464_DOOMKITTEN",
                    HasDoomKittenClaw,
                    () =>
                    {
                        if (HasDoomKittenClaw())
                            return;

                        ApplyCurrentArmyRoomSettings();
                        core.EnsureAccept(5464);
                        KillDoomKittenForArmy("DoomKitten Claw", 1, isTemp: true);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "DoomKitten",
                            compositionType: ClassType.Solo,
                            targetMap: "doomkitten"
                        );
                    },
                    maxPlayersPerSubgroup: 4,
                    prepareForCombat: () => ApplyCurrentArmyRoomSettings()
                )
            )
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        _syncedClassType = ClassType.None;
        SetClass();

        // Vordred's Skull (/vordredboss is Room Limit: 1 in SoloOnlyMaps; helpers wait cleanly while active account solos Vordred)
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5464_VORDRED",
                5464,
                "vordredboss",
                "Vordred",
                "Vordred's Skull",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                beforeHunt: _ => UnbankIfOwned("Vordred's Skull"),
                isEnabled: () => !Is5464Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5464_COMPLETE",
                Is5464Done,
                Is5464Done,
                () =>
                {
                    core.EnsureAccept(5464);
                    core.EnsureComplete(5464);
                }
            )
        )
            return false;

        // 4d. Quest 5465: Mastering the Arcane
        bool Is5465Done() => HasAP() || core.isCompletedBefore(5465, log: false) || Bot.Quests.IsUnlocked(5467);
        bool HasPyromancerArtifact() => Is5465Done() || core.CheckInventory("Pyromancer Artifact");

        if (
            !RunCooperativeObjective(
                "BRACKET70_AP_5465_XAN",
                HasPyromancerArtifact,
                () =>
                {
                    if (HasPyromancerArtifact())
                        return;

                    core.AddDrop("Pyromancer Artifact");
                    core.EnsureAccept(5465);
                    SetClass(ClassType.Solo);
                    core.KillMonster("xantown", "r8", "Left", "Xan", "Pyromancer Artifact", isTemp: false);
                    if (Bot.Map.Name == "xantown")
                        core.Jump("r12", "Left");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Xan", targetMap: "xantown");
                }
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_DRACOLICH",
                5465,
                "dragonheart",
                "Proto-Air Dracolich",
                "Zephyrus Manifesto",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_KAROK",
                5465,
                "northstar",
                "Karok The Fallen",
                "Karok's Glaceran Gem",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_KIBBLE",
                5465,
                "thirdspell",
                "Mana Phoenix",
                "Nightmare Kibble",
                200,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_THUNDERFANG",
                5465,
                "thunderfang",
                "Lightning Ball",
                "Condensed Energy",
                1,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_DOWNWARD",
                5465,
                "downward",
                "Crystal Mana Construct",
                "Crystallized Mana Catalyst",
                1,
                isTemp: false,
                role: ClassType.Solo,
                cell: "r11",
                pad: "Right",
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5465_FARM",
                5465,
                "farm",
                "Treeant",
                "Just the Perfect Stick",
                1,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5465Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5465_COMPLETE",
                Is5465Done,
                Is5465Done,
                () =>
                {
                    core.EnsureAccept(5465);
                    core.EnsureComplete(5465);
                }
            )
        )
            return false;

        // 4e. Quest 5466: For Those Who Have Fallen
        bool Is5466Done() => HasAP() || core.isCompletedBefore(5466, log: false) || Bot.Quests.IsUnlocked(5467);
        bool HasHolyHandGrenade() => Is5466Done() || core.CheckInventory("Holy Hand Grenade");

        if (
            !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5466_GRENADE",
                HasHolyHandGrenade,
                HasHolyHandGrenade,
                () =>
                {
                    core.EnsureAccept(5466);
                    core.BuyItem("castle", 88, "Holy Hand Grenade");
                }
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5466_MANOR",
                5466,
                "manor",
                "Bird of Paradise",
                "Feather of Paradise",
                20,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5466Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5466_DOOMWOOD",
                5466,
                "doomwood",
                "*",
                "Shoelace of a Fallen Paladin",
                77,
                isTemp: false,
                role: ClassType.Farm,
                cell: "r6",
                pad: "Right",
                softRetryLimit: 0,
                isEnabled: () => !Is5466Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5466_FOTIA",
                5466,
                "fotia",
                "Amia the Cult Leader",
                "Eternity Flame",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5466Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5466_COMPLETE",
                Is5466Done,
                Is5466Done,
                () =>
                {
                    core.EnsureAccept(5466);
                    core.EnsureComplete(5466);
                }
            )
        )
            return false;

        // 4f. Quest 5467: Commandment
        bool Is5467Done() => HasAP() || core.isCompletedBefore(5467, log: false);
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5467_BRIGHTFALL",
                5467,
                "brightfall",
                "Painadin Overlord",
                "Skill Observed",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5467Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5467_CITADEL",
                5467,
                "citadel",
                "Grand Inquisitor",
                "Spirit of Vindication",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5467Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5467_ALLIANCE",
                5467,
                "alliance",
                "Good Lieutenant",
                "Radiant Blade Enhancement",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5467Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5467_COMPLETE",
                Is5467Done,
                Is5467Done,
                () =>
                {
                    core.EnsureAccept(5467);
                    core.EnsureComplete(5467);
                }
            )
        )
            return false;

        // 4g. Quest 5468: Hymn of Light
        bool Is5468Done() => HasAP() || core.isCompletedBefore(5468, log: false);
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5468_LIONFANG",
                5468,
                "poisonforest",
                "Xavier Lionfang",
                "Divine Elixir",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5468Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5468_ULTRAALTEON",
                5468,
                "ultraalteon",
                "Ultra Chaos Alteon",
                "Prayer of Salvation",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5468Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5468_NEWFINALE",
                5468,
                "newfinale",
                "Alliance Healer",
                "Acolyte's Braille",
                1,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5468Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5468_SKYTOWER",
                5468,
                "skytower",
                "Dove",
                "Innocence",
                25,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5468Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5468_COMPLETE",
                Is5468Done,
                Is5468Done,
                () =>
                {
                    core.EnsureAccept(5468);
                    core.EnsureComplete(5468);
                }
            )
        )
            return false;

        // 4h. Quest 5469: Righteous Seal (XansLair + Shurpu Ring Guardian + SpellCrafting Rank 8 Scroll of Ethereal Slumber + Golden Caster + Pettivox)
        bool Is5469Done() => HasAP() || core.isCompletedBefore(5469, log: false);
        bool IsXansLairSkip() => Is5469Done() || core.CheckInventory("Fists of Fire");

        if (!RunCoordinatedXansLairStory(IsXansLairSkip))
            return false;

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5469_SHURPU",
                5469,
                "xancave",
                "Shurpu Ring Guardian",
                "Fists of Fire",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                beforeHunt: c => c.AddDrop("Scroll of Ethereal Slumber", "Fists of Fire", "Holy Magic Attunement", "Ring of Mana Transposition"),
                isEnabled: () => !Is5469Done()
            )
        )
            return false;

        if (!RunCoordinatedSpellCraftingRank8AndSlumber(() => Is5469Done()))
            return false;

        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5469_ONSLAUGHT",
                5469,
                "onslaughttower",
                "Golden Caster",
                "Holy Magic Attunement",
                1,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5469Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5469_PALACE",
                5469,
                "palace",
                "Pettivox",
                "Ring of Mana Transposition",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5469Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5469_COMPLETE",
                Is5469Done,
                Is5469Done,
                () =>
                {
                    core.EnsureAccept(5469);
                    core.EnsureComplete(5469);
                    core.SellItem("Scroll of Ethereal Slumber", all: true);
                }
            )
        )
            return false;

        // 4i. Quest 5470: Sacred Magic: Eden
        bool Is5470Done() => HasAP() || core.isCompletedBefore(5470, log: false);
        if (
            !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_SERAPH",
                5470,
                "seraph",
                "Adventus",
                "Sacred Tome",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_MARSH",
                5470,
                "marsh",
                "Marsh Tree",
                "Paladaffodil",
                25,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_WANDERS",
                5470,
                "wanders",
                "Lotus Spider",
                "Spirit Lotus",
                25,
                isTemp: false,
                role: ClassType.Farm,
                cell: "r5",
                pad: "Left",
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_BLOODTUSK",
                5470,
                "bloodtusk",
                "Trollola Plant",
                "Radiant Magnolia",
                25,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_GAIAZOR_WISTERRORA",
                5470,
                "gaiazor",
                "Wisterrora",
                "Cyanoblossom",
                25,
                isTemp: false,
                role: ClassType.Farm,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_GAIAZOR_NEVANNA",
                5470,
                "gaiazor",
                "Nevanna",
                "BrightOak Forest Sapling",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedHuntStep(
                "BRACKET70_AP_5470_INFERNALSPIRE",
                5470,
                "infernalspire",
                "Malxas",
                "Forbidden Demon Seal",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !Is5470Done()
            )
            || !RunCoordinatedTurnInStep(
                "BRACKET70_AP_5470_COMPLETE",
                Is5470Done,
                Is5470Done,
                () =>
                {
                    core.EnsureAccept(5470);
                    core.EnsureComplete(5470);
                }
            )
        )
            return false;

        // 4j. Buy ArchPaladin (36920, shopItemID: 21833) in /darkthronehub Shop 1303 (Room Limit: 1 -> non-combat barrier)
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET70_AP_BUY",
                HasAP,
                HasAP,
                () =>
                {
                    UnbankIfOwned("ArchPaladin");
                    if (HasAP())
                        return;

                    core.BuyItem("darkthronehub", 1303, 36920, shopItemID: 21833);
                    Bot.Wait.ForItemBuy();
                }
            )
        )
            return false;

        // 4k. Rank ArchPaladin to Rank 10 & refresh party composition
        if (
            !RunCooperativeObjective(
                "BRACKET70_AP_RANK_10",
                HasAPRank10,
                () =>
                {
                    if (!HasAPRank10())
                        RankUpClassForArmy("ArchPaladin");

                    _syncedClassType = ClassType.None;
                    SetClass();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "icestormarena");
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: true);
        return !Bot.ShouldExit;
    }
    #endregion Step 2D: Bracket 70

    #region Step 3A: Level 75 Progression (13 LoC, LoO Daily, Additional Classes & Forge Weapons)
    /// <summary>
    /// Step 3A: Level 75 Progression.
    /// Follows the agreed 3-phase flow:
    /// - Level 75 + The Encroaching Shadows daily (Quest 8653)
    /// - Phase 1: Healer / Dragon of Time Rank 10 preparation (for Chaos Lord Xiang)
    /// - Phase 2: Cape of Awe check + Complete 13 Lords of Chaos (Escherion through Chaos Lord Drakath)
    /// - Phase 3: Lord of Order Daily + Additional Classes (Shaman, Northlands Monk, Frost SpiritReaver) + Forge Weapons (Forge & Praxis)
    /// </summary>
    public bool RunStep3A()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();
        core.Logger(
            "Step 3A: Starting Level 75 Progression (13 Lords of Chaos, LoO Daily, Additional Classes & Forge Weapons)..."
        );

        if (!Army.SyncArmy("STEP3A_75_START"))
            return false;

        // 0. Level 75 + Enhance + Best Boost Gear + The Encroaching Shadows Daily (8653)
        if (!RunBracketLevelAndEnhance("STEP3A_LEVEL_75", 75))
            return false;

        EnsureBestStaticBoostGearEquipped();

        if (!RunTheEncroachingShadowsDaily())
            return false;

        // Phase 1: Prepare Healer (or Dragon of Time) at Rank 10 for Chaos Lord Xiang (13 LoC & LoO Day 9)
        if (!RunStep3AHealerPrep())
            return false;

        // Phase 2: Ensure Cape of Awe + Chaorrupter Unlocked (+50% Chaos / +120% Drakath) + Complete 13 Lords of Chaos
        if (!RunBracket55CapeOfAwe("STEP3A"))
            return false;

        if (!RunStep3AChaorrupterUnlocked())
            return false;

        if (!RunCoordinated13LoC())
            return false;

        // Phase 3A: Lord of Order Daily Progression
        if (!RunLordOfOrderProgression("STEP3A"))
            return false;

        BankQuestExtras(7156, "Lord of Order");

        // Phase 3B: Additional Level 75 Classes (Shaman, Northlands Monk, Frost SpiritReaver)
        if (!RunStep3AAdditionalClasses())
            return false;

        // Phase 3C: Forge Weapon Enhancements (Forge Weapon 8738 & Praxis 9171)
        if (!RunStep3AForgeWeapons())
            return false;

        EnsureBestStaticBoostGearEquipped();
        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);

        if (!Army.SyncArmy("STEP3A_75_END"))
            return false;

        core.Logger("Step 3A: Level 75 Progression completed.");
        return !Bot.ShouldExit;
    }

    private bool RunStep3AHealerPrep()
    {
        CoreBots core = CoreBots.Instance;

        bool NeedsXiangOrExamenHealer() =>
            !core.isCompletedBefore(3189, log: false)
            || !core.isCompletedBefore(7164, log: false)
            || !core.isCompletedBefore(8825, log: false);

        bool HasHealerOrDoT() =>
            !NeedsXiangOrExamenHealer()
            || HasAnyClassInInventory("Dragon of Time", "Healer (Rare)", "Healer");

        bool HasRank10HealerOrDoT() =>
            !NeedsXiangOrExamenHealer()
            || HasRank10Class("Dragon of Time")
            || HasRank10Class("Healer (Rare)")
            || HasRank10Class("Healer");

        if (
            !RunCoordinatedTurnInStep(
                "STEP3A_HEALER_BUY",
                HasHealerOrDoT,
                HasHealerOrDoT,
                () =>
                {
                    if (HasHealerOrDoT())
                        return;
                    Adv.BuyItem("classhalla", 176, "Healer");
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "STEP3A_HEALER_RANK_10",
                HasRank10HealerOrDoT,
                () =>
                {
                    if (HasRank10HealerOrDoT())
                        return;

                    string classToRank =
                        HasClassInInventory("Dragon of Time") ? "Dragon of Time"
                        : HasClassInInventory("Healer (Rare)") ? "Healer (Rare)"
                        : "Healer";

                    core.Unbank(classToRank);
                    RankUpClassForArmy(classToRank);
                    EnhanceClassForArmyRespectingGearPolicy(classToRank, allowFullGearEnhance: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "*",
                        targetMap: "icestormarena"
                    );
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);
        return !Bot.ShouldExit;
    }

    private bool RunCoordinated13LoC()
    {
        CoreBots core = CoreBots.Instance;

        if (!Run13LoCPrologue())
            return false;
        if (!Run13LoCEscherion())
            return false;
        if (!Run13LoCVath())
            return false;
        if (!Run13LoCKitsune())
            return false;
        if (!Run13LoCWolfwing())
            return false;
        if (!RunCoordinatedKimberlyStory(() => core.isCompletedBefore(710, log: false), "STEP3A_LOC_KIMBERLY"))
            return false;
        if (!Run13LoCLedgermayne())
            return false;
        if (!Run13LoCTibicenas())
            return false;
        if (!Run13LoCKhasaanda())
            return false;
        if (!Run13LoCIadoa())
            return false;
        if (!Run13LoCLionfang())
            return false;
        if (!Run13LoCXiang())
            return false;
        if (!Run13LoCAlteon())
            return false;
        if (!Run13LoCHero())
            return false;

        return !Bot.ShouldExit;
    }

    private bool Run13LoCPrologue()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(6219, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(183, "battleundera", "Skeletal Fire Mage", () => Story.KillQuest(183, "battleundera", "Skeletal Fire Mage")),
            new(
                176,
                "swordhavenundead",
                "Skeletal Soldier",
                () =>
                {
                    core.EnsureAccept(176);
                    core.HuntMonster("swordhavenundead", "Skeletal Soldier", "Slain Skeletal Soldier", 5, log: false);
                    core.EnsureComplete(176);
                }
            ),
            new(177, "swordhavenundead", "Skeletal Ice Mage", () => Story.KillQuest(177, "swordhavenundead", "Skeletal Ice Mage")),
            new(178, "swordhavenundead", "Undead Giant", () => Story.KillQuest(178, "swordhavenundead", "Undead Giant"), ClassType.Solo),
            new(179, "castleundead", "*", () => Story.MapItemQuest(179, "castleundead", 38, 5)),
            new(180, "castleundead", "Skeletal Viking", () => Story.KillQuest(180, "castleundead", "Skeletal Viking")),
            new(
                195,
                "",
                "*",
                () =>
                {
                    if (!core.isCompletedBefore(195, log: false))
                    {
                        core.Join("castleundead", "King2", "Center");
                        Bot.Sleep(1500);
                        core.Join("shadowfall");
                        core.SendPackets("%xt%zm%updateQuest%73922%43%1%");
                        Bot.Sleep(1500);
                        core.SendPackets($"%xt%zm%updateQuest%188220%41%{(core.HeroAlignment > 1 ? 1 : core.HeroAlignment)}%");
                        Bot.Sleep(1500);
                    }
                    Story.ChainQuest(195);
                }
            ),
            new(196, "chaoscrypt", "Chaorrupted Armor", () => Story.KillQuest(196, "chaoscrypt", "Chaorrupted Armor"), ClassType.Solo),
            new(
                6216,
                "prison",
                "Piggy Drake",
                () =>
                {
                    core.EnsureAccept(6216);
                    core.GetMapItem(39, 5, "prison");
                    core.BuyItem("prison", 1559, 42993);
                    core.EnsureComplete(6216);
                }
            ),
            new(6217, "chaoscrypt", "*", () => Story.MapItemQuest(6217, "chaoscrypt", 5662)),
            new(6218, "chaoscrypt", "Chaorrupted Knight", () => Story.KillQuest(6218, "chaoscrypt", "Chaorrupted Knight")),
            new(
                6219,
                "forestchaos",
                "*",
                () => Story.KillQuest(6219, "forestchaos", new[] { "Chaorrupted Bear", "Chaorrupted Wolf" }),
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_PROLOGUE", steps, IsDone);
    }

    private bool Run13LoCEscherion()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(272, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(245, "mobius", "Chaos Sp-Eye", () => Story.KillQuest(245, "mobius", "Chaos Sp-Eye")),
            new(246, "mobius", "*", () => Story.MapItemQuest(246, "mobius", 42, 5)),
            new(247, "mobius", "Fire Imp", () => Story.KillQuest(247, "mobius", "Fire Imp")),
            new(260, "mobius", "*", () => Story.MapItemQuest(260, "mobius", 44)),
            new(248, "mobius", "Cyclops Raider", () => Story.KillQuest(248, "mobius", "Cyclops Raider")),
            new(249, "mobius", "Slugfit", () => Story.KillQuest(249, "mobius", "Slugfit")),
            new(250, "faerie", "Chainsaw Sneevil", () => Story.KillQuest(250, "faerie", "Chainsaw Sneevil")),
            new(251, "faerie", "*", () => Story.MapItemQuest(251, "faerie", 43, 7)),
            new(252, "faerie", "Chainsaw Sneevil", () => Story.KillQuest(252, "faerie", "Chainsaw Sneevil")),
            new(255, "faerie", "Cyclops Warlord", () => Story.KillQuest(255, "faerie", "Cyclops Warlord"), ClassType.Solo),
            new(256, "faerie", "Aracara", () => Story.KillQuest(256, "faerie", "Aracara"), ClassType.Solo),
            new(257, "cornelis", "Gargoyle", () => Story.KillQuest(257, "cornelis", "Gargoyle")),
            new(261, "cornelis", "*", () => Story.MapItemQuest(261, "cornelis", 45)),
            new(258, "cornelis", "Gargoyle", () => Story.KillQuest(258, "cornelis", "Gargoyle")),
            new(262, "cornelis", "*", () => Story.MapItemQuest(262, "cornelis", 46)),
            new(259, "cornelis", "Stone Golem", () => Story.KillQuest(259, "cornelis", "Stone Golem")),
            new(263, "cornelis", "*", () => Story.MapItemQuest(263, "cornelis", 47)),
            new(266, "mobius", "*", () => Story.MapItemQuest(266, "mobius", 48)),
            new(267, "mobius", "*", () => Story.MapItemQuest(267, "mobius", 49)),
            new(264, "mobius", "Cyclops Raider", () => Story.KillQuest(264, "mobius", "Cyclops Raider")),
            new(
                265,
                "faerie",
                "Chainsaw Sneevil",
                () =>
                {
                    core.EnsureAccept(265);
                    core.HuntMonster("faerie", "Chainsaw Sneevil", "Cardboard Box", 4, isTemp: false, log: false);
                    core.EnsureComplete(265);
                }
            ),
            new(268, "relativity", "Cyclops Raider", () => Story.KillQuest(268, "relativity", "Cyclops Raider")),
            new(269, "relativity", "Fire Imp", () => Story.KillQuest(269, "relativity", "Fire Imp")),
            new(270, "relativity", "Head Gargoyle", () => Story.KillQuest(270, "relativity", "Head Gargoyle")),
            new(271, "hydra", "*", () => Story.MapItemQuest(271, "hydra", new[] { 50, 51, 52 })),
            new(
                272,
                "escherion",
                "*",
                () =>
                {
                    core.EnsureAccept(272);
                    core.KillEscherion("Defeated Escherion", isTemp: true);
                    core.EnsureComplete(272);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_ESCHERION", steps, IsDone);
    }

    private bool Run13LoCVath()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(363, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(319, "tavern", "*", () => Story.MapItemQuest(319, "tavern", 56, 7)),
            new(320, "pines", "Pine Grizzly", () => Story.KillQuest(320, "pines", "Pine Grizzly")),
            new(321, "pines", "Red Shell Turtle", () => Story.KillQuest(321, "pines", "Red Shell Turtle")),
            new(322, "pines", "Twistedtooth", () => Story.KillQuest(322, "pines", "Twistedtooth"), ClassType.Solo),
            new(324, "pines", "Red Shell Turtle", () => Story.KillQuest(324, "pines", "Red Shell Turtle")),
            new(325, "pines", "Pine Grizzly", () => Story.KillQuest(325, "pines", "Pine Grizzly")),
            new(326, "pines", "LeatherWing", () => Story.KillQuest(326, "pines", "LeatherWing")),
            new(
                323,
                "pines",
                "Pine Troll",
                () =>
                {
                    core.EnsureAccept(327);
                    core.HuntMonster("pines", "Pine Troll", "Pine Air Freshener", log: false);
                    core.EnsureComplete(327);
                    if (core.CheckInventory("Snowbeard's Gold"))
                        core.ChainComplete(323);
                }
            ),
            new(344, "dwarfhold", "*", () => Story.MapItemQuest(344, "dwarfhold", 60)),
            new(331, "mountainpath", "Ore Balboa", () => Story.KillQuest(331, "mountainpath", "Ore Balboa")),
            new(332, "mountainpath", "Vultragon", () => Story.KillQuest(332, "mountainpath", "Vultragon")),
            new(333, "dwarfhold", "Chaos Drow", () => Story.KillQuest(333, "dwarfhold", "Chaos Drow")),
            new(334, "dwarfhold", "Glow Worm", () => Story.KillQuest(334, "dwarfhold", "Glow Worm")),
            new(335, "dwarfhold", "Albino Bat", () => Story.KillQuest(335, "dwarfhold", "Albino Bat")),
            new(336, "dwarfhold", "Chaotic Draconian", () => Story.KillQuest(336, "dwarfhold", "Chaotic Draconian")),
            new(337, "dwarfhold", "*", () => Story.MapItemQuest(337, "dwarfhold", 59, 7)),
            new(338, "dwarfhold", "Chaos Drow", () => Story.KillQuest(338, "dwarfhold", "Chaos Drow")),
            new(339, "dwarfhold", "Chaotic Draconian", () => Story.KillQuest(339, "dwarfhold", "Chaotic Draconian")),
            new(
                341,
                "dwarfhold",
                "*",
                () =>
                {
                    if (!core.CheckInventory("Upper City Key"))
                    {
                        core.AddDrop("Key Mold", "Block of Talc", "Upper City Key");
                        if (!core.CheckInventory("Block of Talc") && !core.CheckInventory("Key Mold"))
                        {
                            core.EnsureAccept(340);
                            core.HuntMonster("dwarfhold", "Albino Bat", "warm liquid", log: false);
                            core.GetMapItem(58, 1, "dwarfhold");
                            core.EnsureComplete(340);
                            Bot.Wait.ForPickup("Block of Talc");
                        }
                        if (!core.CheckInventory("Key Mold"))
                        {
                            core.ChainComplete(343);
                            Bot.Wait.ForPickup("Key Mold");
                        }
                        core.EnsureAccept(341);
                        core.HuntMonster("dwarfhold", "Amadeus", "Iron Ingot", log: false);
                        core.EnsureComplete(341);
                        Bot.Wait.ForPickup("Upper City Key");
                    }
                },
                ClassType.Solo
            ),
            new(346, "uppercity", "*", () => Story.MapItemQuest(346, "uppercity", 61)),
            new(347, "uppercity", "Drow Assassin", () => Story.KillQuest(347, "uppercity", "Drow Assassin")),
            new(348, "uppercity", "Chaotic Draconian", () => Story.KillQuest(348, "uppercity", "Chaotic Draconian")),
            new(349, "uppercity", "Chaos Egg", () => Story.KillQuest(349, "uppercity", "Chaos Egg")),
            new(350, "uppercity", "Terradactyl", () => Story.KillQuest(350, "uppercity", "Terradactyl")),
            new(351, "uppercity", "Rhino Beetle", () => Story.KillQuest(351, "uppercity", "Rhino Beetle")),
            new(352, "uppercity", "Cave Lizard", () => Story.KillQuest(352, "uppercity", "Cave Lizard")),
            new(
                354,
                "dwarfprison",
                "*",
                () =>
                {
                    core.AddDrop("Thermite");
                    core.EnsureAccept(353);
                    core.HuntMonster("dwarfprison", "Balboa", "Iron Oxide", 4, log: false);
                    core.HuntMonster("dwarfprison", "Chaos Drow", "Aluminum Powder", log: false);
                    core.HuntMonster("dwarfprison", "Albino Bat", "Magnesium", 3, log: false);
                    core.EnsureComplete(353);
                    Bot.Wait.ForPickup("Thermite");

                    core.EnsureAccept(354);
                    core.Join("vath");
                    core.Jump("CutCap", "Left");
                    Bot.Quests.UpdateQuest(354);
                    core.Join("dwarfprison", "Enter", "Right");
                    core.EnsureComplete(354);
                }
            ),
            new(355, "dwarfprison", "Warden Elfis", () => Story.KillQuest(355, "dwarfprison", "Warden Elfis"), ClassType.Solo),
            new(
                357,
                "dwarfprison",
                "*",
                () =>
                {
                    core.AddDrop("Tee-En-Tee");
                    if (!core.CheckInventory("Tee-En-Tee"))
                    {
                        core.EnsureAccept(356);
                        core.HuntMonster("dwarfprison", "Albino Bat", "concentrated nitric acid", 3, log: false);
                        core.HuntMonster("dwarfprison", "Chaos Drow", "concentrated sulfuric acid", 3, log: false);
                        core.HuntMonster("dwarfprison", "Balboa", "methylbenzene", 2, log: false);
                        core.EnsureComplete(356);
                        Bot.Wait.ForPickup("Tee-En-Tee");
                    }
                    Story.ChainQuest(357);
                }
            ),
            new(362, "roc", "*", () => Story.MapItemQuest(362, "roc", 62)),
            new(363, "stalagbite", "*", () => Story.MapItemQuest(363, "stalagbite", 63)),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_VATH", steps, IsDone);
    }

    private bool Run13LoCKitsune()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(488, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(
                380,
                "yokaiboat",
                "Kappa Ninja",
                () =>
                {
                    Story.MapItemQuest(380, "yokaiboat", 64);
                    Story.KillQuest(380, "yokaiboat", "Kappa Ninja");
                }
            ),
            new(
                381,
                "yokaiboat",
                "Kappa Ninja",
                () =>
                {
                    core.EnsureAccept(381);
                    core.KillMonster("yokaiboat", "r4", "Spawn", "*", "Sail Permit", log: false);
                    core.EnsureComplete(381);
                }
            ),
            new(
                382,
                "dragonkoi",
                "Ryoku",
                () =>
                {
                    core.EnsureAccept(382);
                    core.Join("dragonkoi");
                    core.HuntMonster("dragonkoi", "Ryoku", "Absolute Defeat", log: false);
                    core.EnsureComplete(382);
                },
                ClassType.Solo
            ),
            new(
                402,
                "hachiko",
                "*",
                () => Story.KillQuest(402, "hachiko", new[] { "Samurai Nopperabo", "Ninja Nopperabo" })
            ),
            new(
                403,
                "hachiko",
                "Samurai Nopperabo",
                () =>
                {
                    core.EnsureAccept(403);
                    core.KillMonster("hachiko", "Ox", "Center", "Samurai Nopperabo", "Note from DT", log: false);
                    core.EnsureComplete(403);
                }
            ),
            new(
                405,
                "hachiko",
                "*",
                () =>
                {
                    core.EnsureAccept(405);
                    core.KillMonster("hachiko", "Tiger", "Left", "Samurai Nopperabo", "Scroll Case Key", log: false);
                    core.KillMonster("hachiko", "Snake", "Left", "Ninja Nopperabo", "Scrap I", log: false);
                    core.KillMonster("hachiko", "Horse", "Left", "Samurai Nopperabo", "Scrap II", log: false);
                    core.KillMonster("hachiko", "Pig", "Left", "Ninja Nopperabo", "Scrap III", log: false);
                    core.EnsureComplete(405);
                }
            ),
            new(406, "hachiko", "Dai Tengu", () => Story.KillQuest(406, "hachiko", "Dai Tengu"), ClassType.Solo),
            new(466, "bamboo", "*", () => Story.MapItemQuest(466, "bamboo", 90)),
            new(
                467,
                "bamboo",
                "Tanuki",
                () => Story.KillQuest(467, "bamboo", new[] { "Tanuki", "Tanuki" })
            ),
            new(468, "bamboo", "SoulTaker", () => Story.KillQuest(468, "bamboo", "SoulTaker"), ClassType.Solo),
            new(469, "junkyard", "*", () => Story.MapItemQuest(469, "junkyard", 91)),
            new(
                470,
                "junkyard",
                "*",
                () =>
                {
                    core.EnsureAccept(470);
                    core.KillMonster("junkyard", "r2", "Right", 1, "Wild Plain's Blade", log: false);
                    core.KillMonster("junkyard", "r3", "Top", 4, "Cold Steel Hilt", log: false);
                    core.KillMonster("junkyard", "r3", "Top", 3, "Soul Gem", log: false);
                    core.KillMonster("junkyard", "r4", "Left", 6, "Iron Tsuba", log: false);
                    core.KillMonster("junkyard", "r2", "Right", 2, "Wooden Saya", log: false);
                    core.EnsureComplete(470);
                }
            ),
            new(471, "junkyard", "Onibaba", () => Story.KillQuest(471, "junkyard", "Onibaba"), ClassType.Solo),
            new(
                473,
                "yokairiver",
                "Funa-yurei",
                () =>
                {
                    core.EnsureAccept(473);
                    core.HuntMonster("yokairiver", "Funa-yurei", "Roka's Geta", log: false);
                    core.HuntMonster("yokairiver", "Funa-yurei", "Roka's Kabuto", log: false);
                    core.HuntMonster("yokairiver", "Funa-yurei", "Roka's Sashimono", log: false);
                    core.HuntMonster("yokairiver", "Funa-yurei", "Roka'sened Wakizashi", log: false);
                    core.EnsureComplete(473);
                }
            ),
            new(
                474,
                "yokairiver",
                "Kappa Ninja",
                () =>
                {
                    Story.KillQuest(474, "yokairiver", "Kappa Ninja");
                    Story.MapItemQuest(474, "yokairiver", 92);
                }
            ),
            new(476, "yokairiver", "Nure Onna", () => Story.KillQuest(476, "yokairiver", "Nure Onna"), ClassType.Solo),
            new(477, "yokaigrave", "Skello Kitty", () => Story.KillQuest(477, "yokaigrave", "Skello Kitty")),
            new(
                478,
                "yokaigrave",
                "*",
                () => Story.KillQuest(478, "yokaigrave", new[] { "Ninja Nopperabo", "Samurai Nopperabo" })
            ),
            new(479, "yokaigrave", "Neko Mata", () => Story.KillQuest(479, "yokaigrave", "Neko Mata"), ClassType.Solo),
            new(481, "odokuro", "O-dokuro", () => Story.KillQuest(481, "odokuro", "O-dokuro"), ClassType.Solo),
            new(484, "yokaiwar", "O-Dokuro's Head", () => Story.KillQuest(484, "yokaiwar", "O-Dokuro's Head"), ClassType.Solo),
            new(
                488,
                "kitsune",
                "Kitsune",
                () =>
                {
                    core.EnsureAccept(488);
                    core.KillKitsune("Defeated Kitsune", isTemp: true);
                    core.EnsureComplete(488);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_KITSUNE", steps, IsDone);
    }

    private bool Run13LoCWolfwing()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(597, log: false) || core.isCompletedBefore(598, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(494, "darkoviagrave", "*", () => Story.MapItemQuest(494, "darkoviagrave", 97)),
            new(495, "darkoviagrave", "Skeletal Fire Mage", () => Story.KillQuest(495, "darkoviagrave", "Skeletal Fire Mage")),
            new(496, "darkoviagrave", "Rattlebones", () => Story.KillQuest(496, "darkoviagrave", "Rattlebones")),
            new(497, "darkoviagrave", "Albino Bat", () => Story.KillQuest(497, "darkoviagrave", "Albino Bat")),
            new(498, "darkoviagrave", "Blightfang", () => Story.KillQuest(498, "darkoviagrave", "Blightfang"), ClassType.Solo),
            new(308, "greenguardeast", "Wolf", () => Story.KillQuest(308, "greenguardeast", "Wolf")),
            new(309, "greenguardwest", "Slime", () => Story.KillQuest(309, "greenguardwest", "Slime")),
            new(310, "greenguardwest", "Frogzard", () => Story.KillQuest(310, "greenguardwest", "Frogzard")),
            new(311, "greenguardeast", "Spider", () => Story.KillQuest(311, "greenguardeast", "Spider")),
            new(
                514,
                "greenguardwest",
                "Big Bad Boar",
                () =>
                {
                    if (!core.CheckInventory("Red's Big Wolf Slaying Axe"))
                    {
                        core.AddDrop("Red's Big Wolf Slaying Axe");
                        Story.KillQuest(515, "greenguardwest", "Big Bad Boar");
                        Bot.Wait.ForPickup("Red's Big Wolf Slaying Axe");
                    }
                    Story.ChainQuest(514);
                }
            ),
            new(516, "darkoviaforest", "Dire Wolf", () => Story.KillQuest(516, "darkoviaforest", "Dire Wolf")),
            new(
                517,
                "darkoviaforest",
                "Blood Maggot",
                () => Story.KillQuest(517, "darkoviaforest", new[] { "Blood Maggot", "Blood Maggot", "Blood Maggot" })
            ),
            new(
                518,
                "darkoviaforest",
                "Lich of the Stone",
                () => Story.KillQuest(518, "darkoviaforest", "Lich of the Stone"),
                ClassType.Solo
            ),
            new(519, "safiria", "Blood Maggot", () => Story.KillQuest(519, "safiria", "Blood Maggot")),
            new(520, "safiria", "Albino Bat", () => Story.KillQuest(520, "safiria", "Albino Bat")),
            new(521, "safiria", "Chaos Lycan", () => Story.KillQuest(521, "safiria", "Chaos Lycan")),
            new(522, "safiria", "Twisted Paw", () => Story.KillQuest(522, "safiria", "Twisted Paw"), ClassType.Solo),
            new(534, "lycan", "Dire Wolf", () => Story.KillQuest(534, "lycan", "Dire Wolf")),
            new(535, "lycan", "*", () => Story.KillQuest(535, "lycan", new[] { "Lycan Knight", "Lycan" })),
            new(536, "lycan", "Chaos Vampire Knight", () => Story.KillQuest(536, "lycan", "Chaos Vampire Knight")),
            new(537, "lycan", "Sanguine", () => Story.KillQuest(537, "lycan", "Sanguine"), ClassType.Solo),
            new(552, "", "*", () => Story.ChainQuest(552)),
            new(
                564,
                "lycanwar",
                "Edvard",
                () =>
                {
                    core.Join("lycanwar");
                    core.Jump("Boss", "Left");
                    core.KillMonster("lycanwar", "Boss", "Left", "Edvard", log: false);
                    Bot.Sleep(5000);
                    Story.MapItemQuest(564, "chaoscave", 107);
                },
                ClassType.Solo
            ),
            new(565, "chaoscave", "Werepyre", () => Story.KillQuest(565, "chaoscave", "Werepyre")),
            new(
                566,
                "chaoscave",
                "Werepyre",
                () =>
                {
                    core.EnsureAccept(566);
                    core.KillMonster("chaoscave", "r3", "Left", "Werepyre", "Secret Words", log: false);
                    core.EnsureComplete(566);
                }
            ),
            new(
                567,
                "chaoscave",
                "DracoWerePyre",
                () =>
                {
                    core.EnsureAccept(567);
                    Bot.Quests.UpdateQuest(567);
                    core.KillMonster("chaoscave", "r5", "Left", "DracoWerePyre", "DracoWerePyre Defeated", log: false);
                    core.EnsureComplete(567);
                    Bot.Quests.UpdateQuest(597);
                },
                ClassType.Solo
            ),
            new(
                597,
                "wolfwing",
                "Wolfwing",
                () =>
                {
                    if (core.HeroAlignment != 1)
                        core.ChangeAlignment(Alignment.Good);
                    Story.ChainQuest(597);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_WOLFWING", steps, IsDone);
    }

    private bool Run13LoCLedgermayne()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(847, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(805, "arcangrove", "*", () => Story.MapItemQuest(805, "arcangrove", 139)),
            new(806, "cloister", "*", () => Story.MapItemQuest(806, "cloister", 142)),
            new(807, "cloister", "*", () => Story.MapItemQuest(807, "cloister", 140)),
            new(808, "cloister", "Acornent", () => Story.KillQuest(808, "cloister", "Acornent")),
            new(809, "cloister", "Karasu", () => Story.KillQuest(809, "cloister", "Karasu")),
            new(
                810,
                "cloister",
                "*",
                () =>
                {
                    if (core.CheckInventory(43280))
                        core.SellItem(43280, all: true);
                    core.EnsureAccept(810);
                    Adv.BuyItem("embersea", 1100, 5572, shopItemID: 17967);
                    Story.MapItemQuest(810, "cloister", 141, 3);
                }
            ),
            new(811, "cloister", "Wendigo", () => Story.KillQuest(811, "cloister", "Wendigo"), ClassType.Solo),
            new(813, "mudluk", "*", () => Story.MapItemQuest(813, "mudluk", 143)),
            new(814, "mudluk", "Swamp Lurker", () => Story.KillQuest(814, "mudluk", "Swamp Lurker")),
            new(815, "mudluk", "Swamp Lurker", () => Story.KillQuest(815, "mudluk", "Swamp Lurker")),
            new(816, "arcangrove", "Gorillaphant", () => Story.KillQuest(816, "arcangrove", "Gorillaphant")),
            new(817, "mudluk", "Swamp Frogdrake", () => Story.KillQuest(817, "mudluk", "Swamp Frogdrake")),
            new(818, "mudluk", "Tiger Leech", () => Story.KillQuest(818, "mudluk", "Tiger Leech"), ClassType.Solo),
            new(825, "natatorium", "*", () => Story.MapItemQuest(825, "natatorium", 144)),
            new(826, "natatorium", "*", () => Story.MapItemQuest(826, "natatorium", 145, 12)),
            new(827, "natatorium", "Anglerfish", () => Story.KillQuest(827, "natatorium", "Anglerfish")),
            new(828, "natatorium", "Merdraconian", () => Story.KillQuest(828, "natatorium", "Merdraconian")),
            new(
                829,
                "cloister",
                "*",
                () =>
                {
                    core.EnsureAccept(829);
                    core.HuntMonster("arcangrove", "Seed Spitter", "Snapdrake", 5, log: false);
                    core.HuntMonster("cloister", "Acornent", "Green Finger", 4, log: false);
                    core.HuntMonster("cloister", "Karasu", "Karasu's Feather", 3, log: false);
                    core.HuntMonster("cloister", "Wendigo", "Wendigo's Tooth", log: false);
                    core.HuntMonster("mudluk", "Swamp Frogdrake", "Fresh Water", 6, log: false);
                    core.EnsureComplete(829);
                },
                ClassType.Solo
            ),
            new(830, "natatorium", "Nessie", () => Story.KillQuest(830, "natatorium", "Nessie"), ClassType.Solo),
            new(831, "gilead", "*", () => Story.MapItemQuest(831, "gilead", 146)),
            new(
                832,
                "gilead",
                "Earth Elemental",
                () =>
                {
                    core.EnsureAccept(832);
                    core.HuntMonster("gilead", "Earth Elemental", "Clod of Earth", 5, log: false);
                    core.HuntMonster("arcangrove", "Seed Spitter", "Living Earth", 4, log: false);
                    core.EnsureComplete(832);
                }
            ),
            new(
                833,
                "gilead",
                "Water Elemental",
                () =>
                {
                    core.EnsureAccept(833);
                    core.HuntMonster("gilead", "Water Elemental", "Drop of Water", 5, log: false);
                    core.HuntMonster("natatorium", "Merdraconian", "Living Water", 6, log: false);
                    core.EnsureComplete(833);
                }
            ),
            new(
                834,
                "gilead",
                "Wind Elemental",
                () =>
                {
                    core.EnsureAccept(834);
                    core.HuntMonster("gilead", "Wind Elemental", "Breeze of Air", 5, log: false);
                    core.HuntMonster("cloister", "Karasu", "Living Air", 8, log: false);
                    core.EnsureComplete(834);
                }
            ),
            new(
                835,
                "gilead",
                "Fire Elemental",
                () =>
                {
                    core.EnsureAccept(835);
                    core.HuntMonster("gilead", "Fire Elemental", "Spark of Fire", 5, log: false);
                    core.HuntMonster("mudluk", "Swamp Frogdrake", "Living Fire", 5, log: false);
                    core.EnsureComplete(835);
                }
            ),
            new(836, "gilead", "Mana Elemental", () => Story.KillQuest(836, "gilead", "Mana Elemental"), ClassType.Solo),
            new(838, "mafic", "*", () => Story.MapItemQuest(838, "mafic", 147)),
            new(839, "mafic", "Volcanic Maggot", () => Story.KillQuest(839, "mafic", "Volcanic Maggot")),
            new(840, "mafic", "Scoria Serpent", () => Story.KillQuest(840, "mafic", "Scoria Serpent")),
            new(841, "mafic", "Living Fire", () => Story.KillQuest(841, "mafic", "Living Fire")),
            new(842, "mafic", "Mafic Dragon", () => Story.KillQuest(842, "mafic", "Mafic Dragon"), ClassType.Solo),
            new(843, "elemental", "Mana Imp", () => Story.KillQuest(843, "elemental", "Mana Imp")),
            new(844, "elemental", "Mana Falcon", () => Story.KillQuest(844, "elemental", "Mana Falcon")),
            new(
                845,
                "elemental",
                "*",
                () =>
                {
                    core.EnsureAccept(845);
                    core.HuntMonster("mafic", "Mafic Dragon", "Ruby Dragonstone", log: false);
                    core.HuntMonster("cloister", "Wendigo", "Quartz Beaststone", log: false);
                    core.HuntMonster("mudluk", "Tiger Leech", "Beryl Leechstone", log: false);
                    core.HuntMonster("natatorium", "Nessie", "Sapphire Serpentstone", log: false);
                    core.HuntMonster("gilead", "Mana Elemental", "Tourmaline Monolith", log: false);
                    core.EnsureComplete(845);
                },
                ClassType.Solo
            ),
            new(
                846,
                "elemental",
                "Mana Golem",
                () =>
                {
                    Bot.Quests.UpdateQuest(847);
                    Story.KillQuest(846, "elemental", "Mana Golem");
                },
                ClassType.Solo
            ),
            new(
                847,
                "ledgermayne",
                "Ledgermayne",
                () =>
                {
                    core.EnsureAccept(847);
                    Bot.Events.ExtensionPacketReceived -= LedgermayneZoneListener;
                    Bot.Events.ExtensionPacketReceived += LedgermayneZoneListener;
                    try
                    {
                        core.HuntMonster("ledgermayne", "Ledgermayne", "Ledgermayne Defeated", log: false);
                    }
                    finally
                    {
                        Bot.Events.ExtensionPacketReceived -= LedgermayneZoneListener;
                    }
                    core.EnsureComplete(847);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_LEDGERMAYNE", steps, IsDone);
    }

    private async void LedgermayneZoneListener(dynamic packet)
    {
        if (
            Bot.ShouldExit
            || Bot.Player?.Alive != true
            || !string.Equals(Bot.Map?.Name, "ledgermayne", StringComparison.OrdinalIgnoreCase)
            || packet?["params"]?.type?.ToString() != "json"
        )
            return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (zoneSet is null)
            return;

        int x;
        int y;

        if (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase))
            (x, y) = (292, 355);
        else if (zoneSet.Equals("B", StringComparison.OrdinalIgnoreCase))
            (x, y) = (750, 350);
        else if (zoneSet.Length == 0)
            (x, y) = (592, 250);
        else
            return;

        await Task.Run(() => Bot.Player.WalkTo(x, y, 9));
    }

    private bool Run13LoCTibicenas()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(1005, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(930, "sandport", "*", () => Story.MapItemQuest(930, "sandport", 251)),
            new(931, "sandport", "Sandshark", () => Story.KillQuest(931, "sandport", "Sandshark")),
            new(932, "sandport", "Tomb Robber", () => Story.KillQuest(932, "sandport", "Tomb Robber")),
            new(933, "sandport", "Tomb Robber", () => Story.KillQuest(933, "sandport", "Tomb Robber")),
            new(
                934,
                "sandport",
                "*",
                () =>
                {
                    core.EnsureAccept(934);
                    core.KillMonster("sandport", "r6", "Left", "Sell-Sword Leader", "Leader's Note", log: false);
                    core.KillMonster("sandport", "r5", "Left", "Horc Sell-Sword", "Horc Sell-Sword Defeated", 3, log: false);
                    core.EnsureComplete(934);
                    Bot.Quests.UpdateQuest(4614);
                }
            ),
            new(967, "pyramid", "Golden Scarab", () => Story.KillQuest(967, "pyramid", "Golden Scarab")),
            new(968, "pyramid", "Anubis Deathguard", () => Story.KillQuest(968, "pyramid", "Anubis Deathguard")),
            new(969, "pyramid", "Mummy", () => Story.KillQuest(969, "pyramid", "Mummy")),
            new(970, "pyramid", "Golden Scarab", () => Story.KillQuest(970, "pyramid", "Golden Scarab")),
            new(971, "pyramid", "*", () => Story.MapItemQuest(971, "pyramid", 304)),
            new(
                972,
                "wanders",
                "Kalestri Worshiper",
                () =>
                {
                    core.EnsureAccept(972);
                    core.KillMonster("wanders", "r2", "Right", "Kalestri Worshiper", "Kalestri Worshipper Slain", 8, log: false);
                    core.EnsureComplete(972);
                }
            ),
            new(
                973,
                "wanders",
                "Kalestri Hound",
                () =>
                {
                    core.EnsureAccept(973);
                    core.KillMonster("wanders", "r3", "Right", "Kalestri Hound", "Star Ruby of Shifting", 10, log: false);
                    core.EnsureComplete(973);
                }
            ),
            new(
                974,
                "wanders",
                "Kalestri Hound",
                () =>
                {
                    core.EnsureAccept(974);
                    core.KillMonster("wanders", "r3", "Right", "Kalestri Hound", "Kalestri Hound Slain", 10, log: false);
                    core.EnsureComplete(974);
                }
            ),
            new(
                975,
                "wanders",
                "Lotus Spider",
                () =>
                {
                    core.EnsureAccept(975);
                    core.KillMonster("wanders", "r5", "Right", "Lotus Spider", "Lotus Spider Slain", 10, log: false);
                    core.EnsureComplete(975);
                }
            ),
            new(976, "wanders", "*", () => Story.MapItemQuest(976, "wanders", 306)),
            new(
                977,
                "wanders",
                "Lotus Spider",
                () =>
                {
                    core.EnsureAccept(977);
                    core.KillMonster("wanders", "r5", "Right", "Lotus Spider", "Venom Sac", log: false);
                    core.EnsureComplete(977);
                }
            ),
            new(
                978,
                "wanders",
                "Sek-Duat",
                () =>
                {
                    core.EnsureAccept(978);
                    core.KillMonster("wanders", "Boss", "Left", "Sek-Duat", "Sek-Duat Freed", log: false);
                    core.EnsureComplete(978);
                },
                ClassType.Solo
            ),
            new(995, "sandcastle", "*", () => Story.MapItemQuest(995, "sandcastle", 361)),
            new(996, "sandcastle", "War Hyena", () => Story.KillQuest(996, "sandcastle", "War Hyena")),
            new(997, "sandcastle", "War Mummy", () => Story.KillQuest(997, "sandcastle", "War Mummy")),
            new(998, "sandcastle", "War Hyena", () => Story.KillQuest(998, "sandcastle", "War Hyena")),
            new(999, "sandcastle", "Chaos Sphinx", () => Story.KillQuest(999, "sandcastle", "Chaos Sphinx"), ClassType.Solo),
            new(1000, "djinn", "Lamia", () => Story.KillQuest(1000, "djinn", "Lamia")),
            new(1001, "sandsea", "Desert Vase", () => Story.KillQuest(1001, "sandsea", "Desert Vase")),
            new(1002, "sandsea", "Bupers Camel", () => Story.KillQuest(1002, "sandsea", "Bupers Camel")),
            new(1003, "djinn", "Harpy", () => Story.KillQuest(1003, "djinn", "Harpy")),
            new(1004, "djinn", "*", () => Story.MapItemQuest(1004, "djinn", 370, 5)),
            new(1005, "djinn", "Tibicenas", () => Story.KillQuest(1005, "djinn", "Tibicenas"), ClassType.Solo),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_TIBICENAS", steps, IsDone);
    }

    private bool Run13LoCKhasaanda()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(1473, log: false) || core.isCompletedBefore(1468, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(
                1232,
                "crossroads",
                "*",
                () =>
                {
                    Story.MapItemQuest(1232, "bloodtusk", 523);
                    Story.KillQuest(1232, "bloodtusk", "Trollola Plant");
                    Story.KillQuest(1232, "crossroads", "Chinchilizard");
                }
            ),
            new(
                1233,
                "crossroads",
                "*",
                () => Story.KillQuest(1233, "crossroads", new[] { "Lemurphant", "Koalion" })
            ),
            new(
                1234,
                "crossroads",
                "*",
                () =>
                {
                    core.EnsureAccept(1234);
                    core.KillMonster("bloodtusk", "r8", "Right", "Crystal-Rock", "Quartze", 3, log: false);
                    core.HuntMonster("crossroads", "Lemurphant", "Lemurphant Tusks", 5, log: false);
                    core.HuntMonster("crossroads", "Chinchilizard", "Chinchilizard Scales", 5, log: false);
                    Story.MapItemQuest(1234, "crossroads", 525);
                }
            ),
            new(
                1235,
                "crossroads",
                "*",
                () =>
                {
                    Story.MapItemQuest(1235, "crossroads", 521, 10);
                    Story.KillQuest(1235, "bloodtusk", "Trollola Plant");
                    Story.KillQuest(1235, "crossroads", new[] { "Lemurphant", "Koalion" });
                }
            ),
            new(1236, "", "*", () => Story.ChainQuest(1236)),
            new(
                1237,
                "crossroads",
                "*",
                () =>
                {
                    Story.MapItemQuest(1237, "bloodtusk", 524, 5);
                    Story.MapItemQuest(1237, "crossroads", 522, 5);
                    Story.KillQuest(1237, "bloodtusk", "Rock");
                    Story.KillQuest(1237, "crossroads", new[] { "Koalion", "Lemurphant" });
                }
            ),
            new(1241, "", "*", () => Story.ChainQuest(1241)),
            new(1273, "", "*", () => Story.ChainQuest(1273)),
            new(1280, "ravinetemple", "*", () => Story.MapItemQuest(1280, "ravinetemple", 553)),
            new(
                1281,
                "ravinetemple",
                "*",
                () =>
                {
                    Story.MapItemQuest(1281, "ravinetemple", 554, 5);
                    Story.MapItemQuest(1281, "ravinetemple", new[] { 555, 556 }, 10);
                }
            ),
            new(1282, "ravinetemple", "Temple Guardian", () => Story.KillQuest(1282, "ravinetemple", "Temple Guardian")),
            new(
                1283,
                "ravinetemple",
                "Temple Guardian",
                () =>
                {
                    Story.MapItemQuest(1283, "ravinetemple", 557, 10);
                    Story.KillQuest(1283, "ravinetemple", "Temple Guardian");
                }
            ),
            new(1284, "ravinetemple", "Temple Guardian", () => Story.KillQuest(1284, "ravinetemple", "Temple Guardian")),
            new(1375, "alliance", "*", () => Story.MapItemQuest(1375, "alliance", new[] { 679, 680 })),
            new(
                1376,
                "alliance",
                "*",
                () => Story.KillQuest(1376, "alliance", new[] { "Good Soldier", "Evil Soldier" })
            ),
            new(1377, "alliance", "*", () => Story.MapItemQuest(1377, "alliance", 675, 10)),
            new(1378, "alliance", "*", () => Story.MapItemQuest(1378, "alliance", 676)),
            new(1379, "alliance", "Chaorrupted Evil Soldier", () => Story.KillQuest(1379, "alliance", "Chaorrupted Evil Soldier")),
            new(
                1380,
                "alliance",
                "*",
                () => Story.KillQuest(1380, "alliance", new[] { "General Cynari", "General Tibias" }),
                ClassType.Solo
            ),
            new(1424, "ancienttemple", "Chaotic Vulture", () => Story.KillQuest(1424, "ancienttemple", "Chaotic Vulture")),
            new(
                1425,
                "ancienttemple",
                "Chaotic Vulture",
                () =>
                {
                    Story.MapItemQuest(1425, "ancienttemple", 706, 7);
                    Story.KillQuest(1425, "ancienttemple", "Chaotic Vulture");
                }
            ),
            new(1426, "ancienttemple", "Chaos Troll Spirit", () => Story.KillQuest(1426, "ancienttemple", "Chaos Troll Spirit")),
            new(1427, "ancienttemple", "Serpentress", () => Story.KillQuest(1427, "ancienttemple", "Serpentress"), ClassType.Solo),
            new(1428, "ancienttemple", "*", () => Story.MapItemQuest(1428, "ancienttemple", 707)),
            new(1456, "orecavern", "*", () => Story.MapItemQuest(1456, "orecavern", 717)),
            new(
                1457,
                "orecavern",
                "Crashroom",
                () =>
                {
                    Story.MapItemQuest(1457, "orecavern", 719, 5);
                    Story.KillQuest(1457, "orecavern", "Crashroom");
                }
            ),
            new(1458, "orecavern", "*", () => Story.MapItemQuest(1458, "orecavern", 718, 5)),
            new(1459, "orecavern", "Chaorrupted Evil Soldier", () => Story.KillQuest(1459, "orecavern", "Chaorrupted Evil Soldier")),
            new(1460, "orecavern", "Naga Baas", () => Story.KillQuest(1460, "orecavern", "Naga Baas"), ClassType.Solo),
            new(1469, "dreamnexus", "*", () => Story.MapItemQuest(1469, "dreamnexus", new[] { 734, 735, 736, 737 })),
            new(
                1470,
                "dreamnexus",
                "*",
                () => Story.KillQuest(1470, "dreamnexus", new[] { "Dark Wyvern", "Aether Serpent" })
            ),
            new(
                1471,
                "dreamnexus",
                "*",
                () =>
                {
                    Story.MapItemQuest(1471, "dreamnexus", 738, 10);
                    Story.MapItemQuest(1471, "dreamnexus", 739, 11);
                }
            ),
            new(1472, "dreamnexus", "Solar Phoenix", () => Story.KillQuest(1472, "dreamnexus", "Solar Phoenix")),
            new(
                1473,
                "dreamnexus",
                "Khasaanda",
                () =>
                {
                    core.EnsureAccept(1473);
                    core.KillMonster("dreamnexus", "r17a", "Up", "Khasaanda", "Khasaanda Defeated", log: false);
                    core.EnsureComplete(1473);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_KHASAANDA", steps, IsDone);
    }

    private bool Run13LoCIadoa()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(2519, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(2239, "thespan", "*", () => Story.MapItemQuest(2239, "thespan", new[] { 1358, 1359, 1360, 1361, 1362, 1363 })),
            new(2240, "timelibrary", "*", () => Story.KillQuest(2240, "timelibrary", new[] { "Sneak", "Tog", "Shadowscythe" })),
            new(
                2241,
                "timelibrary",
                "*",
                () =>
                {
                    Story.MapItemQuest(2241, "timelibrary", 1365, 3);
                    Story.KillQuest(2241, "timelibrary", new[] { "Moglin Ghost", "Undead Knight" });
                }
            ),
            new(
                2242,
                "timelibrary",
                "Tog",
                () =>
                {
                    Story.MapItemQuest(2242, "timelibrary", 1366, 2);
                    Story.KillQuest(2242, "timelibrary", "Tog");
                }
            ),
            new(
                2243,
                "timelibrary",
                "*",
                () =>
                {
                    Story.MapItemQuest(2243, "timelibrary", 1367);
                    Story.KillQuest(2243, "timelibrary", new[] { "Shadowscythe", "Training Globe" });
                }
            ),
            new(
                2244,
                "timelibrary",
                "*",
                () =>
                {
                    Story.MapItemQuest(2244, "timelibrary", 1368);
                    Story.KillQuest(2244, "timelibrary", new[] { "Queen's Recruit", "Queen's Knight" });
                }
            ),
            new(2253, "timevoid", "Ephemerite", () => Story.KillQuest(2253, "timevoid", "Ephemerite")),
            new(
                2254,
                "timevoid",
                "Time-Travel Fairy",
                () =>
                {
                    Story.MapItemQuest(2254, "timevoid", 1438, 16);
                    Story.KillQuest(2254, "timevoid", "Time-Travel Fairy");
                }
            ),
            new(
                2255,
                "timevoid",
                "*",
                () =>
                {
                    Story.MapItemQuest(2255, "timevoid", 1439, 12);
                    Story.KillQuest(2255, "timevoid", new[] { "Ephemerite", "Time-Travel Fairy" });
                }
            ),
            new(
                2256,
                "timevoid",
                "*",
                () => Story.KillQuest(2256, "timevoid", new[] { "Time-Travel Fairy", "Void Phoenix" })
            ),
            new(2257, "timevoid", "*", () => Story.MapItemQuest(2257, "timevoid", new[] { 1440, 1441, 1442, 1443 })),
            new(
                2258,
                "timevoid",
                "Unending Avatar",
                () => Story.KillQuest(2258, "timevoid", "Unending Avatar", AutoCompleteQuest: false),
                ClassType.Solo
            ),
            new(2376, "aqlesson", "*", () => Story.MapItemQuest(2376, "aqlesson", 1467)),
            new(2377, "aqlesson", "Ninja", () => Story.KillQuest(2377, "aqlesson", "Ninja")),
            new(
                2378,
                "aqlesson",
                "*",
                () =>
                {
                    Story.MapItemQuest(2378, "aqlesson", 1468, 8);
                    Story.MapItemQuest(2378, "aqlesson", 1469);
                }
            ),
            new(
                2379,
                "aqlesson",
                "*",
                () =>
                {
                    Story.MapItemQuest(2379, "aqlesson", new[] { 1470, 1471 }, 3);
                    Story.KillQuest(2379, "aqlesson", new[] { "Eternite Ore", "Water Elemental" });
                }
            ),
            new(
                2380,
                "aqlesson",
                "*",
                () =>
                {
                    Story.MapItemQuest(2380, "aqlesson", new[] { 1473, 1472 }, 3);
                    Story.KillQuest(2380, "aqlesson", new[] { "Ice Elemental", "Fire Elemental" });
                }
            ),
            new(2381, "aqlesson", "Void Dragon", () => Story.KillQuest(2381, "aqlesson", "Void Dragon")),
            new(2382, "aqlesson", "Firezard", () => Story.KillQuest(2382, "aqlesson", "Firezard")),
            new(2383, "aqlesson", "Ice Elemental", () => Story.KillQuest(2383, "aqlesson", "Ice Elemental")),
            new(2384, "thespan", "*", () => Story.MapItemQuest(2384, "thespan", 1474)),
            new(2385, "aqlesson", "Akriloth", () => Story.KillQuest(2385, "aqlesson", "Akriloth"), ClassType.Solo),
            new(2386, "aqlesson", "Carnax", () => Story.KillQuest(2386, "aqlesson", "Carnax"), ClassType.Solo),
            new(2470, "dflesson", "*", () => Story.MapItemQuest(2470, "dflesson", 1549, 8)),
            new(2471, "dflesson", "Fire Elemental", () => Story.KillQuest(2471, "dflesson", "Fire Elemental")),
            new(
                2472,
                "dflesson",
                "*",
                () => Story.KillQuest(2472, "dflesson", new[] { "Lava Slime", "Fire Elemental" })
            ),
            new(
                2473,
                "dflesson",
                "*",
                () => Story.KillQuest(2473, "dflesson", new[] { "Tog", "Agitated Orb" })
            ),
            new(2474, "dflesson", "Vultragon", () => Story.KillQuest(2474, "dflesson", "Vultragon")),
            new(2475, "dflesson", "Chaos Sp-Eye", () => Story.KillQuest(2475, "dflesson", "Chaos Sp-Eye")),
            new(2476, "dflesson", "Chaorrupted Evil Soldier", () => Story.KillQuest(2476, "dflesson", "Chaorrupted Evil Soldier")),
            new(
                2477,
                "dflesson",
                "*",
                () => Story.KillQuest(2477, "dflesson", new[] { "Lava Golem", "Fire Elemental" })
            ),
            new(
                2478,
                "dflesson",
                "*",
                () => Story.KillQuest(2478, "dflesson", new[] { "Chaotic Horcboar", "Chaotic Chicken" })
            ),
            new(
                2479,
                "dflesson",
                "Fluffy the Dracolich",
                () => Story.KillQuest(2479, "dflesson", "Fluffy the Dracolich"),
                ClassType.Solo
            ),
            new(2504, "mqlesson", "*", () => Story.MapItemQuest(2504, "mqlesson", 1580, GetReward: false)),
            new(2505, "mqlesson", "Asteroid", () => Story.KillQuest(2505, "mqlesson", "Asteroid")),
            new(2506, "mqlesson", "*", () => Story.MapItemQuest(2506, "mqlesson", 1581, 5)),
            new(2507, "mqlesson", "MystRaven Student", () => Story.KillQuest(2507, "mqlesson", "MystRaven Student")),
            new(2508, "mqlesson", "Training Globe", () => Story.KillQuest(2508, "mqlesson", "Training Globe")),
            new(2509, "mqlesson", "MystRaven Student", () => Story.KillQuest(2509, "mqlesson", "MystRaven Student")),
            new(2510, "mqlesson", "Chaos Shadowscythe", () => Story.KillQuest(2510, "mqlesson", "Chaos Shadowscythe")),
            new(2511, "mqlesson", "Chaos Shadowscythe", () => Story.KillQuest(2511, "mqlesson", "Chaos Shadowscythe")),
            new(2512, "mqlesson", "Dragonoid", () => Story.KillQuest(2512, "mqlesson", "Dragonoid"), ClassType.Solo),
            new(2513, "deepchaos", "Chaotic Merdrac", () => Story.KillQuest(2513, "deepchaos", "Chaotic Merdrac")),
            new(2515, "deepchaos", "Chaos Angler", () => Story.KillQuest(2515, "deepchaos", "Chaos Angler")),
            new(2516, "deepchaos", "*", () => Story.MapItemQuest(2516, "deepchaos", 1582, 3)),
            new(2517, "deepchaos", "Kathool", () => Story.KillQuest(2517, "deepchaos", "Kathool"), ClassType.Solo),
            new(2518, "timespace", "Astral Ephemerite", () => Story.KillQuest(2518, "timespace", "Astral Ephemerite")),
            new(
                2519,
                "timespace",
                "Chaos Lord Iadoa",
                () =>
                {
                    if (!core.CheckInventory("Dragonoid of Hours"))
                        core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false, log: false);
                    Story.KillQuest(2519, "timespace", "Chaos Lord Iadoa");
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_IADOA", steps, IsDone);
    }

    private bool Run13LoCLionfang()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(2814, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(2612, "blackhorn", "Restless Undead", () => Story.KillQuest(2612, "blackhorn", "Restless Undead")),
            new(2613, "blackhorn", "*", () => Story.MapItemQuest(2613, "blackhorn", 1615, 10)),
            new(2614, "blackhorn", "Tomb Spider", () => Story.KillQuest(2614, "blackhorn", "Tomb Spider")),
            new(
                2615,
                "blackhorn",
                "*",
                () =>
                {
                    Story.MapItemQuest(2615, "blackhorn", 1616);
                    Story.KillQuest(2615, "blackhorn", new[] { "Restless Undead", "Tomb Spider" });
                }
            ),
            new(2616, "blackhorn", "*", () => Story.MapItemQuest(2616, "blackhorn", 1617)),
            new(2617, "blackhorn", "Bonefeeder Spider", () => Story.KillQuest(2617, "blackhorn", "Bonefeeder Spider"), ClassType.Solo),
            new(2618, "blackhorn", "*", () => Story.MapItemQuest(2618, "blackhorn", 1618)),
            new(2619, "blackhorn", "Tomb Spider", () => Story.KillQuest(2619, "blackhorn", "Tomb Spider")),
            new(2620, "blackhorn", "Restless Undead", () => Story.KillQuest(2620, "blackhorn", "Restless Undead")),
            new(2621, "blackhorn", "*", () => Story.MapItemQuest(2621, "blackhorn", 1619)),
            new(2622, "onslaughttower", "*", () => Story.MapItemQuest(2622, "onslaughttower", new[] { 1620, 1621, 1622, 1623 })),
            new(2623, "onslaughttower", "Golden Caster", () => Story.KillQuest(2623, "onslaughttower", "Golden Caster")),
            new(2624, "onslaughttower", "Golden Caster", () => Story.KillQuest(2624, "onslaughttower", "Golden Caster")),
            new(2625, "onslaughttower", "*", () => Story.MapItemQuest(2625, "onslaughttower", 1624, 8)),
            new(2626, "onslaughttower", "*", () => Story.MapItemQuest(2626, "onslaughttower", 1625)),
            new(2627, "onslaughttower", "*", () => Story.MapItemQuest(2627, "onslaughttower", 1626, 4)),
            new(2628, "onslaughttower", "Golden Caster", () => Story.KillQuest(2628, "onslaughttower", "Golden Caster")),
            new(2629, "onslaughttower", "*", () => Story.MapItemQuest(2629, "onslaughttower", 1627)),
            new(
                2630,
                "onslaughttower",
                "Maximillian Lionfang",
                () => Story.KillQuest(2630, "onslaughttower", "Maximillian Lionfang"),
                ClassType.Solo
            ),
            new(2666, "falguard", "Chaonslaught Caster", () => Story.KillQuest(2666, "falguard", "Chaonslaught Caster")),
            new(2667, "falguard", "*", () => Story.MapItemQuest(2667, "falguard", 1628, 6)),
            new(
                2668,
                "falguard",
                "*",
                () => Story.KillQuest(2668, "falguard", new[] { "Chaonslaught Warrior", "Chaonslaught Cavalry" })
            ),
            new(2669, "falguard", "*", () => Story.MapItemQuest(2669, "falguard", 1629)),
            new(2670, "falguard", "Chaonslaught Warrior", () => Story.KillQuest(2670, "falguard", "Chaonslaught Warrior")),
            new(2671, "falguard", "*", () => Story.MapItemQuest(2671, "falguard", 1630)),
            new(2672, "falguard", "*", () => Story.MapItemQuest(2672, "falguard", 1631)),
            new(2673, "falguard", "Chaonslaught Caster", () => Story.KillQuest(2673, "falguard", "Chaonslaught Caster")),
            new(2674, "falguard", "*", () => Story.MapItemQuest(2674, "falguard", 1632)),
            new(2675, "falguard", "Primarch", () => Story.KillQuest(2675, "falguard", "Primarch"), ClassType.Solo),
            new(2720, "deathpits", "Rotting Darkblood", () => Story.KillQuest(2720, "deathpits", "Rotting Darkblood")),
            new(2721, "deathpits", "*", () => Story.MapItemQuest(2721, "deathpits", 1691, 5)),
            new(2722, "deathpits", "Rotting Darkblood", () => Story.KillQuest(2722, "deathpits", "Rotting Darkblood")),
            new(2723, "deathpits", "Rotting Darkblood", () => Story.KillQuest(2723, "deathpits", "Rotting Darkblood")),
            new(2724, "deathpits", "Ghastly Darkblood", () => Story.KillQuest(2724, "deathpits", "Ghastly Darkblood")),
            new(2725, "deathpits", "Ghastly Darkblood", () => Story.KillQuest(2725, "deathpits", "Ghastly Darkblood")),
            new(2726, "deathpits", "*", () => Story.MapItemQuest(2726, "deathpits", 1692, 6)),
            new(2727, "deathpits", "Rotting Darkblood", () => Story.KillQuest(2727, "deathpits", "Rotting Darkblood")),
            new(2728, "deathpits", "Ghastly Darkblood", () => Story.KillQuest(2728, "deathpits", "Ghastly Darkblood")),
            new(2729, "deathpits", "Sundered Darkblood", () => Story.KillQuest(2729, "deathpits", "Sundered Darkblood")),
            new(2730, "deathpits", "*", () => Story.MapItemQuest(2730, "deathpits", 1693, 9)),
            new(
                2731,
                "deathpits",
                "*",
                () => Story.KillQuest(2731, "deathpits", new[] { "Sundered Darkblood", "Rotting Darkblood", "Ghastly Darkblood" })
            ),
            new(2732, "deathpits", "*", () => Story.MapItemQuest(2732, "deathpits", 1694, 12)),
            new(
                2740,
                "deathpits",
                "Wrathful Vestis",
                () =>
                {
                    Story.MapItemQuest(2740, "deathpits", 1695);
                    Story.KillQuest(2740, "deathpits", "Wrathful Vestis");
                },
                ClassType.Solo
            ),
            new(2792, "venomvaults", "*", () => Story.MapItemQuest(2792, "venomvaults", 1724)),
            new(2793, "venomvaults", "Chaonslaught Warrior", () => Story.KillQuest(2793, "venomvaults", "Chaonslaught Warrior")),
            new(2794, "venomvaults", "*", () => Story.MapItemQuest(2794, "venomvaults", 1725)),
            new(2796, "venomvaults", "*", () => Story.MapItemQuest(2796, "venomvaults", 1726)),
            new(2797, "venomvaults", "Chaonslaught Caster", () => Story.KillQuest(2797, "venomvaults", "Chaonslaught Caster")),
            new(2798, "venomvaults", "*", () => Story.MapItemQuest(2798, "venomvaults", 1727, 5)),
            new(2799, "venomvaults", "Chaonslaught Caster", () => Story.KillQuest(2799, "venomvaults", "Chaonslaught Caster")),
            new(2800, "venomvaults", "Chaonslaught Warrior", () => Story.KillQuest(2800, "venomvaults", "Chaonslaught Warrior")),
            new(2801, "venomvaults", "Chaonslaught Caster", () => Story.KillQuest(2801, "venomvaults", "Chaonslaught Caster")),
            new(2802, "venomvaults", "*", () => Story.MapItemQuest(2802, "venomvaults", 1728, 3)),
            new(2803, "venomvaults", "Chaonslaught Caster", () => Story.KillQuest(2803, "venomvaults", "Chaonslaught Caster")),
            new(2804, "venomvaults", "Manticore", () => Story.KillQuest(2804, "venomvaults", "Manticore"), ClassType.Solo),
            new(2805, "stormtemple", "*", () => Story.MapItemQuest(2805, "stormtemple", 1729, 4)),
            new(
                2806,
                "stormtemple",
                "*",
                () =>
                {
                    core.EnsureAccept(2806);
                    core.KillMonster("stormtemple", "r1", "Left", "*", "Lightning Boots", log: false);
                    core.EnsureComplete(2806);
                }
            ),
            new(2807, "stormtemple", "Chaonslaught Caster", () => Story.KillQuest(2807, "stormtemple", "Chaonslaught Caster")),
            new(2808, "stormtemple", "Chaonslaught Caster", () => Story.KillQuest(2808, "stormtemple", "Chaonslaught Caster")),
            new(2809, "stormtemple", "*", () => Story.MapItemQuest(2809, "stormtemple", 1730, 3)),
            new(2810, "stormtemple", "Chaonslaught Caster", () => Story.KillQuest(2810, "stormtemple", "Chaonslaught Caster")),
            new(2811, "stormtemple", "*", () => Story.MapItemQuest(2811, "stormtemple", 1731)),
            new(2812, "stormtemple", "Chaonslaught Cavalry", () => Story.KillQuest(2812, "stormtemple", "Chaonslaught Cavalry")),
            new(2813, "stormtemple", "*", () => Story.MapItemQuest(2813, "stormtemple", 1732)),
            new(
                2814,
                "stormtemple",
                "Chaos Lord Lionfang",
                () => Story.KillQuest(2814, "stormtemple", "Chaos Lord Lionfang"),
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_LIONFANG", steps, IsDone);
    }

    private bool Run13LoCXiang()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(3189, log: false);

        var preBossSteps = new CoordinatedStoryStep[]
        {
            new(2909, "battleoff", "*", () => Story.MapItemQuest(2909, "battleoff", 1779)),
            new(2910, "battleoff", "*", () => Story.MapItemQuest(2910, "battleoff", 1780, 8)),
            new(2911, "battleoff", "Evil Moglin", () => Story.KillQuest(2911, "battleoff", "Evil Moglin")),
            new(2912, "battleoff", "Evil Moglin", () => Story.KillQuest(2912, "battleoff", "Evil Moglin")),
            new(2913, "brightfall", "Undead Minion", () => Story.KillQuest(2913, "brightfall", "Undead Minion")),
            new(2914, "brightfall", "Undead Mage", () => Story.KillQuest(2914, "brightfall", "Undead Mage")),
            new(2915, "brightfall", "*", () => Story.MapItemQuest(2915, "brightfall", 1781, 6)),
            new(2916, "brightfall", "*", () => Story.MapItemQuest(2916, "brightfall", 1782, 8)),
            new(2917, "brightfall", "Painadin Overlord", () => Story.KillQuest(2917, "brightfall", "Painadin Overlord"), ClassType.Solo),
            new(
                2918,
                "",
                "*",
                () =>
                {
                    core.Join("overworld");
                    Story.ChainQuest(2918);
                }
            ),
            new(2919, "overworld", "Undead Minion", () => Story.KillQuest(2919, "overworld", "Undead Minion")),
            new(2920, "overworld", "Undead Minion", () => Story.KillQuest(2920, "overworld", "Undead Minion")),
            new(2921, "overworld", "Undead Minion", () => Story.KillQuest(2921, "overworld", "Undead Minion")),
            new(2922, "overworld", "Undead Mage", () => Story.KillQuest(2922, "overworld", "Undead Mage")),
            new(2923, "overworld", "*", () => Story.MapItemQuest(2923, "overworld", 1800, 6)),
            new(2924, "overworld", "Undead Bruiser", () => Story.KillQuest(2924, "overworld", "Undead Bruiser")),
            new(2925, "overworld", "Undead Bruiser", () => Story.KillQuest(2925, "overworld", "Undead Bruiser")),
            new(2926, "overworld", "Undead Minion", () => Story.KillQuest(2926, "overworld", "Undead Minion")),
            new(2927, "overworld", "*", () => Story.MapItemQuest(2927, "overworld", 1801, 8)),
            new(2928, "overworld", "Undead Minion", () => Story.KillQuest(2928, "overworld", "Undead Minion")),
            new(2929, "overworld", "Undead Minion", () => Story.KillQuest(2929, "overworld", "Undead Minion")),
            new(2930, "overworld", "Undead Minion", () => Story.KillQuest(2930, "overworld", "Undead Minion")),
            new(2931, "overworld", "*", () => Story.MapItemQuest(2931, "overworld", 1802, 10)),
            new(2932, "overworld", "Undead Artix", () => Story.KillQuest(2932, "overworld", "Undead Artix"), ClassType.Solo),
            new(3166, "reddeath", "Fire Leech", () => Story.KillQuest(3166, "reddeath", "Fire Leech")),
            new(
                3167,
                "reddeath",
                "RedDeath Moglin",
                () =>
                {
                    Story.MapItemQuest(3167, "reddeath", new[] { 2178, 2179 });
                    Story.KillQuest(3167, "reddeath", "RedDeath Moglin");
                }
            ),
            new(3168, "reddeath", "*", () => Story.MapItemQuest(3168, "reddeath", 2180)),
            new(3169, "reddeath", "Fire Leech", () => Story.KillQuest(3169, "reddeath", "Fire Leech")),
            new(3170, "reddeath", "Grim Widow", () => Story.KillQuest(3170, "reddeath", "Grim Widow")),
            new(3171, "reddeath", "Swamp Wraith", () => Story.KillQuest(3171, "reddeath", "Swamp Wraith")),
            new(3172, "reddeath", "Swamp Wraith", () => Story.KillQuest(3172, "reddeath", "Swamp Wraith")),
            new(3183, "battleontown", "*", () => Story.MapItemQuest(3183, "battleontown", 2203)),
            new(
                3184,
                "earthstorm",
                "Shard Spinner",
                () =>
                {
                    if (!core.CheckInventory("Shriekward Potion"))
                    {
                        core.EnsureAccept(3184);
                        core.HuntMonster("earthstorm", "Shard Spinner", "Perfect Prism", log: false);
                        core.EnsureComplete(3184);
                    }
                }
            ),
            new(
                3185,
                "bloodtuskwar",
                "Chaotic Horcboar",
                () =>
                {
                    if (!core.CheckInventory("Shriekward Potion"))
                    {
                        core.EnsureAccept(3185);
                        core.HuntMonster("bloodtuskwar", "Chaotic Horcboar", "Unchaorrupted Sample", log: false);
                        core.EnsureComplete(3185);
                    }
                }
            ),
            new(
                3186,
                "bloodtuskwar",
                "Chaos Tigriff",
                () =>
                {
                    if (!core.CheckInventory("Shriekward Potion"))
                    {
                        core.EnsureAccept(3186);
                        core.HuntMonster("bloodtuskwar", "Chaos Tigriff", "Harpy Feather", log: false);
                        core.EnsureComplete(3186);
                    }
                }
            ),
            new(
                3187,
                "mirrorportal",
                "*",
                () =>
                {
                    if (!core.CheckInventory("Shriekward Potion"))
                    {
                        core.EnsureAccept(3187);
                        core.Join("mirrorportal");
                        core.EnsureComplete(3187);
                        Bot.Wait.ForPickup("Shriekward Potion");
                        core.SellItem("Shriekward Potion", all: true);
                    }
                }
            ),
            new(
                3188,
                "mirrorportal",
                "Chaos Harpy",
                () =>
                {
                    core.EnsureAccept(3188);
                    core.BuyItem("mirrorportal", 774, "Shriekward Potion", 99);
                    core.Equip("Shriekward Potion");
                    core.KillMonster("mirrorportal", "r3", "Left", "Chaos Harpy", "Chaos Harpy Defeated", log: false);
                    core.EnsureComplete(3188);
                    core.JumpWait();
                    if (Bot.Inventory.IsEquipped(20771))
                    {
                        core.SendPackets($"%xt%zm%unequipItem%{Bot.Map.RoomID}%{20771}%");
                        core.Sleep();
                    }
                    core.SellItem("Shriekward Potion", all: true);
                },
                ClassType.Solo
            ),
        };

        if (!RunCoordinatedStoryChain("STEP3A_LOC_XIANG", preBossSteps, IsDone))
            return false;

        // 3189: Chaos Lord Xiang (equip Healer / Dragon of Time across the whole army)
        if (
            !RunCooperativeObjective(
                "STEP3A_LOC_XIANG_3189",
                IsDone,
                () =>
                {
                    if (IsDone())
                        return;

                    Adv.GearStore();
                    core.EnsureAccept(3189);
                    core.KillXiang("Chaos Lord Xiang Defeated", isTemp: true);
                    core.EnsureComplete(3189);
                    Adv.GearStore(true);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    string healerClass =
                        HasClassInInventory("Dragon of Time") ? "Dragon of Time"
                        : HasClassInInventory("Healer (Rare)") ? "Healer (Rare)"
                        : "Healer";
                    if (HasClassInInventory(healerClass) && Bot.Player.CurrentClass?.Name != healerClass)
                        core.Equip(healerClass);

                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Chaos Lord Xiang",
                        targetMap: "mirrorportal"
                    );
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);
        return !Bot.ShouldExit;
    }

    private bool Run13LoCAlteon()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(3160, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(3077, "archives", "Chaos Bandit", () => Story.KillQuest(3077, "archives", "Chaos Bandit")),
            new(3078, "archives", "Camouflaged Sp-eye", () => Story.KillQuest(3078, "archives", "Camouflaged Sp-eye")),
            new(
                3079,
                "archives",
                "*",
                () => Story.KillQuest(3079, "archives", new[] { "Chaos Bandit", "Camouflaged Sp-eye" })
            ),
            new(
                3080,
                "archives",
                "Chaos Bandit",
                () =>
                {
                    Story.MapItemQuest(3080, "archives", 1937);
                    Story.KillQuest(3080, "archives", "Chaos Bandit");
                }
            ),
            new(3081, "archives", "Chaos Rat", () => Story.KillQuest(3081, "archives", "Chaos Rat")),
            new(3082, "archives", "Chaos Spider", () => Story.KillQuest(3082, "archives", "Chaos Spider")),
            new(3083, "archives", "*", () => Story.KillQuest(3083, "archives", new[] { "Chaos Rat", "Chaos Spider" })),
            new(3084, "archives", "Sludgelord", () => Story.KillQuest(3084, "archives", "Sludgelord"), ClassType.Solo),
            new(3094, "armory", "Chaorrupted Prisoner", () => Story.KillQuest(3094, "armory", "Chaorrupted Prisoner")),
            new(
                3095,
                "armory",
                "Chaorrupted Prisoner",
                () =>
                {
                    Story.MapItemQuest(3095, "armory", 1956, 4);
                    Story.KillQuest(3095, "armory", "Chaorrupted Prisoner");
                }
            ),
            new(3096, "armory", "Chaos Drifter", () => Story.KillQuest(3096, "armory", "Chaos Drifter")),
            new(3089, "armory", "Chaorrupted Prisoner", () => Story.KillQuest(3089, "armory", "Chaorrupted Prisoner")),
            new(3090, "armory", "Chaos Mage", () => Story.KillQuest(3090, "armory", "Chaos Mage")),
            new(3091, "armory", "Chaos Mage", () => Story.KillQuest(3091, "armory", "Chaos Mage")),
            new(3092, "armory", "*", () => Story.MapItemQuest(3092, "armory", 1957)),
            new(3093, "armory", "Chaos General", () => Story.KillQuest(3093, "armory", "Chaos General"), ClassType.Solo),
            new(3120, "ceremony", "Chaos Invader", () => Story.KillQuest(3120, "ceremony", "Chaos Invader")),
            new(
                3121,
                "swordhaven",
                "*",
                () =>
                {
                    Story.MapItemQuest(3121, "yulgar", new[] { 2108, 2109, 2110 });
                    Story.MapItemQuest(3121, "archives", 2111);
                    Story.MapItemQuest(3121, "swordhaven", new[] { 2112, 2113, 2114, 2115 });
                }
            ),
            new(3122, "swordhaven", "*", () => Story.MapItemQuest(3122, "swordhaven", 2116, 8)),
            new(3123, "mafic", "Living Fire", () => Story.KillQuest(3123, "mafic", "Living Fire")),
            new(3124, "ceremony", "Chaos Invader", () => Story.KillQuest(3124, "ceremony", "Chaos Invader")),
            new(3125, "ceremony", "*", () => Story.MapItemQuest(3125, "ceremony", 2118, 6)),
            new(
                3126,
                "ceremony",
                "Chaos Invader",
                () =>
                {
                    Story.MapItemQuest(3126, "ceremony", 2119);
                    Story.KillQuest(3126, "ceremony", "Chaos Invader");
                }
            ),
            new(3127, "ceremony", "Chaos Justicar", () => Story.KillQuest(3127, "ceremony", "Chaos Justicar"), ClassType.Solo),
            new(3133, "chaosaltar", "*", () => Story.MapItemQuest(3133, "chaosaltar", 2127, 12)),
            new(3134, "chaosaltar", "Princess Thrall", () => Story.KillQuest(3134, "chaosaltar", "Princess Thrall"), ClassType.Solo),
            new(3158, "castleroof", "Chaos Dragon", () => Story.KillQuest(3158, "castleroof", "Chaos Dragon"), ClassType.Solo),
            new(3159, "swordhavenfalls", "*", () => Story.MapItemQuest(3159, "swordhavenfalls", 2158)),
            new(
                3160,
                "swordhavenfalls",
                "Chaos Lord Alteon",
                () => Story.KillQuest(3160, "swordhavenfalls", "Chaos Lord Alteon"),
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_ALTEON", steps, IsDone);
    }

    private bool Run13LoCHero()
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() => core.isCompletedBefore(3799, log: false) && core.isCompletedBefore(3881, log: false);

        var steps = new CoordinatedStoryStep[]
        {
            new(3578, "", "*", () => Story.ChainQuest(3578)),
            new(3579, "", "*", () => Story.ChainQuest(3579)),
            new(3580, "", "*", () => Story.ChainQuest(3580)),
            new(3581, "", "*", () => Story.ChainQuest(3581)),
            new(3582, "", "*", () => Story.ChainQuest(3582)),
            new(3583, "", "*", () => Story.ChainQuest(3583)),
            new(3584, "", "*", () => Story.ChainQuest(3584)),
            new(3585, "", "*", () => Story.ChainQuest(3585)),
            new(3586, "", "*", () => Story.ChainQuest(3586)),
            new(3587, "", "*", () => Story.ChainQuest(3587)),
            new(3588, "", "*", () => Story.ChainQuest(3588)),
            new(3589, "", "*", () => Story.ChainQuest(3589)),
            new(3590, "", "*", () => Story.ChainQuest(3590)),
            new(3591, "", "*", () => Story.ChainQuest(3591)),
            new(3764, "", "*", () => Story.ChainQuest(3764)),
            new(3765, "mountdoomskull", "*", () => Story.MapItemQuest(3765, "mountdoomskull", 2726)),
            new(3766, "", "*", () => Story.ChainQuest(3766)),
            new(3779, "", "*", () => Story.ChainQuest(3779)),
            new(3781, "newfinale", "Alliance Virago", () => Story.KillQuest(3781, "newfinale", "Alliance Virago")),
            new(3788, "newfinale", "Chaos Challenger", () => Story.KillQuest(3788, "newfinale", "Chaos Challenger"), ClassType.Solo),
            new(3783, "newfinale", "Alliance Virago", () => Story.KillQuest(3783, "newfinale", "Alliance Virago")),
            new(
                3789,
                "newfinale",
                "Chaorrupted Lycan Hunter",
                () =>
                {
                    Story.KillQuest(3789, "newfinale", "Chaorrupted Lycan Hunter");
                    core.EnsureComplete(3789);
                },
                ClassType.Solo
            ),
            new(3785, "newfinale", "Alliance Virago", () => Story.KillQuest(3785, "newfinale", "Alliance Virago")),
            new(3790, "newfinale", "Memory of Vampires", () => Story.KillQuest(3790, "newfinale", "Memory of Vampires"), ClassType.Solo),
            new(3787, "newfinale", "Alliance Virago", () => Story.KillQuest(3787, "newfinale", "Alliance Virago")),
            new(3608, "", "*", () => Story.ChainQuest(3608)),
            new(3609, "", "*", () => Story.ChainQuest(3609)),
            new(3610, "", "*", () => Story.ChainQuest(3610)),
            new(3611, "", "*", () => Story.ChainQuest(3611)),
            new(3612, "", "*", () => Story.ChainQuest(3612)),
            new(3613, "", "*", () => Story.ChainQuest(3613)),
            new(3614, "", "*", () => Story.ChainQuest(3614)),
            new(3615, "", "*", () => Story.ChainQuest(3615)),
            new(3616, "", "*", () => Story.ChainQuest(3616)),
            new(3617, "", "*", () => Story.ChainQuest(3617)),
            new(3618, "", "*", () => Story.ChainQuest(3618)),
            new(3619, "", "*", () => Story.ChainQuest(3619)),
            new(3791, "", "*", () => Story.ChainQuest(3791)),
            new(3792, "", "*", () => Story.ChainQuest(3792)),
            new(
                3794,
                "newfinale",
                "Alliance Soldier",
                () =>
                {
                    Story.KillQuest(3794, "newfinale", "Alliance Soldier");
                    core.Join("newfinale", "NPC2", "Left");
                    core.SendPackets("%xt%zm%setAchievement%15075%ia1%20%1%");
                }
            ),
            new(3795, "drakathfight", "*", () => Story.MapItemQuest(3795, "drakathfight", 2894)),
            new(3620, "shadowrise", "Darkness Elemental", () => Story.KillQuest(3620, "shadowrise", "Darkness Elemental")),
            new(3796, "shadowrise", "*", () => Story.MapItemQuest(3796, "shadowrise", 2895)),
            new(3797, "", "*", () => Story.ChainQuest(3797)),
            new(
                3798,
                "shadowattack",
                "*",
                () =>
                {
                    if (!core.CheckInventory("Hunt for Death"))
                    {
                        core.AddDrop("Hunt for Death");
                        Story.MapItemQuest(3798, "shadowattack", 2896);
                        Bot.Wait.ForPickup("Hunt for Death");
                    }
                }
            ),
            new(
                3799,
                "shadowattack",
                "Death",
                () =>
                {
                    if (!core.CheckInventory("You Beat Death?!"))
                    {
                        core.AddDrop("You Beat Death?!");
                        Story.KillQuest(3799, "shadowattack", "Death");
                        Bot.Wait.ForPickup("You Beat Death?!");
                    }
                },
                ClassType.Solo
            ),
            new(
                3875,
                "confrontation",
                "*",
                () =>
                {
                    core.EnsureAccept(3875);
                    core.Join("confrontation");
                    Bot.Wait.ForMapLoad("confrontation");
                    core.EnsureComplete(3875);
                },
                ClassType.Solo
            ),
            new(
                3876,
                "finalbattle",
                "*",
                () =>
                {
                    core.EnsureAccept(3876);
                    core.KillMonster("finalbattle", "r1", "Left", 1, 26875, log: false);
                    core.EnsureComplete(3876);
                },
                ClassType.Solo
            ),
            new(
                3877,
                "finalbattle",
                "*",
                () =>
                {
                    core.EnsureAccept(3877);
                    core.KillMonster("finalbattle", "r4", "Left", 14, 26876, log: false);
                    core.EnsureComplete(3877);
                },
                ClassType.Solo
            ),
            new(
                3878,
                "finalbattle",
                "*",
                () =>
                {
                    core.EnsureAccept(3878);
                    core.KillMonster("finalbattle", "r9", "Left", 23, 26877, log: false);
                    core.EnsureComplete(3878);
                },
                ClassType.Solo
            ),
            new(
                3879,
                "chaosrealm",
                "Alteon",
                () =>
                {
                    core.EnsureAccept(3879);
                    core.Join("chaosrealm");
                    core.KillMonster("chaosrealm", "r24", "Left", "Alteon", "Chaos Lord Alteon Defeated", log: false);
                    core.EnsureComplete(3879);
                },
                ClassType.Solo
            ),
            new(
                3880,
                "chaoslord",
                "*",
                () =>
                {
                    core.EnsureAccept(3880);
                    core.Join("chaoslord");
                    core.KillMonster("chaoslord", "r2", "Left", "*", "13th Lord of Chaos Defeated", log: false);
                    core.EnsureComplete(3880);
                },
                ClassType.Solo
            ),
            new(
                3881,
                "finalshowdown",
                "*",
                () =>
                {
                    core.EnsureAccept(3881);
                    core.Join("finalshowdown");
                    if (Bot.Player.Cell != "r2")
                        core.Jump("r2", "Left");
                    while (!Bot.ShouldExit && !Bot.TempInv.Contains(26881))
                    {
                        Bot.Combat.Attack("*");
                        core.Sleep();
                    }
                    core.JumpWait();
                    core.EnsureComplete(3881);
                },
                ClassType.Solo
            ),
        };

        return RunCoordinatedStoryChain("STEP3A_LOC_HERO", steps, IsDone);
    }

    private bool RunStep3AAdditionalClasses()
    {
        CoreBots core = CoreBots.Instance;

        // 1. Shaman (Arcangrove Rank 10 + Shop 214, ItemID 5765, shopItemID 4799)
        bool HasShaman() => HasClassInInventory("Shaman");
        bool HasRank10Shaman() => HasRank10Class("Shaman");

        if (
            !RunCooperativeObjective(
                "STEP3A_SHAMAN_ARCANGROVE_REP_10",
                () => HasShaman() || Farm.FactionRank("Arcangrove") >= 10,
                () =>
                {
                    if (HasShaman() || Farm.FactionRank("Arcangrove") >= 10)
                        return;
                    SetClass(ClassType.Farm);
                    Farm.ArcangroveREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "arcangrove");
                }
            )
        )
            return false;

        if (
            !RunCoordinatedTurnInStep(
                "STEP3A_SHAMAN_BUY",
                HasShaman,
                HasShaman,
                () =>
                {
                    if (HasShaman())
                        return;
                    Adv.BuyItem("arcangrove", 214, 5765, shopItemID: 4799);
                }
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "STEP3A_SHAMAN_RANK_10",
                HasRank10Shaman,
                () =>
                {
                    if (HasRank10Shaman())
                        return;
                    core.Unbank("Shaman");
                    RankUpClassForArmy("Shaman");
                    EnhanceClassForArmyRespectingGearPolicy("Shaman", allowFullGearEnhance: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "*",
                        targetMap: "icestormarena"
                    );
                }
            )
        )
            return false;

        // 2. Northlands Monk (Seasonal /frozensoul drop from Frozensoul Queen)
        bool HasNorthlandsMonk() => HasClassInInventory("Northlands Monk");
        bool HasRank10NorthlandsMonk() => HasRank10Class("Northlands Monk");
        bool canFarmNorthlandsMonk = HasNorthlandsMonk() || core.isSeasonalMapActive("frozensoul");

        if (canFarmNorthlandsMonk)
        {
            if (
                !RunCoordinatedHuntStep(
                    "STEP3A_NORTHLANDS_MONK_GET",
                    0,
                    "frozensoul",
                    "Frozensoul Queen",
                    "Northlands Monk",
                    1,
                    isTemp: false,
                    role: ClassType.Solo,
                    softRetryLimit: 0,
                    isEnabled: () => !HasNorthlandsMonk()
                )
            )
                return false;

            if (
                !RunCooperativeObjective(
                    "STEP3A_NORTHLANDS_MONK_RANK_10",
                    () => !HasNorthlandsMonk() || HasRank10NorthlandsMonk(),
                    () =>
                    {
                        if (!HasNorthlandsMonk() || HasRank10NorthlandsMonk())
                            return;
                        core.Unbank("Northlands Monk");
                        RankUpClassForArmy("Northlands Monk");
                        EnhanceClassForArmyRespectingGearPolicy("Northlands Monk", allowFullGearEnhance: false);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "*",
                            targetMap: "icestormarena"
                        );
                    }
                )
            )
                return false;
        }
        else
        {
            core.Logger("Step 3A: Skipping Northlands Monk (/frozensoul is currently out of season).");
        }

        // 3. Frost SpiritReaver (requires seasonal Frost Sigil + Kyanos merge items if not already owned)
        bool HasFSR() => HasClassInInventory("Frost SpiritReaver");
        bool HasRank10FSR() => HasRank10Class("Frost SpiritReaver");
        bool HasFSRMergeMaterials() =>
            core.CheckInventory("Frost Sigil", toInv: false)
            && core.CheckInventory("Envoy of Kyanos", toInv: false)
            && core.CheckInventory("Favored of Kyanos", toInv: false)
            && core.CheckInventory("Warrior of Kyanos", toInv: false)
            && core.CheckInventory("Ice Diamond", 29, toInv: false);
        bool canFarmFSR = HasFSR() || HasFSRMergeMaterials();

        if (canFarmFSR)
        {
            if (
                !RunCoordinatedTurnInStep(
                    "STEP3A_FSR_BUY",
                    HasFSR,
                    HasFSR,
                    () =>
                    {
                        if (HasFSR())
                            return;
                        core.Unbank(
                            "Frost Sigil",
                            "Envoy of Kyanos",
                            "Favored of Kyanos",
                            "Warrior of Kyanos",
                            "Ice Diamond"
                        );
                        Adv.BuyItem("icedungeon", 1948, "Frost SpiritReaver");
                    }
                )
            )
                return false;

            if (
                !RunCooperativeObjective(
                    "STEP3A_FSR_RANK_10",
                    () => !HasFSR() || HasRank10FSR(),
                    () =>
                    {
                        if (!HasFSR() || HasRank10FSR())
                            return;
                        core.Unbank("Frost SpiritReaver");
                        RankUpClassForArmy("Frost SpiritReaver");
                        EnhanceClassForArmyRespectingGearPolicy(
                            "Frost SpiritReaver",
                            allowFullGearEnhance: false
                        );
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "*",
                            targetMap: "icestormarena"
                        );
                    }
                )
            )
                return false;
        }
        else
        {
            core.Logger(
                "Step 3A: Skipping Frost SpiritReaver (missing seasonal Frost Sigil / Kyanos merge materials)."
            );
        }

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);
        return !Bot.ShouldExit;
    }

    private bool RunStep3AForgeWeapons()
    {
        CoreBots core = CoreBots.Instance;

        // 1. Forge Weapon Enhancement (Quest 8738 - Blacksmithing Rank 4, Level 30)
        bool IsForgeWeaponDone() => core.isCompletedBefore(8738, log: false);

        if (!IsForgeWeaponDone())
        {
            if (!RunBlacksmithingRank4Step("STEP3A_FORGE_WEAPON_BS_RANK_4", IsForgeWeaponDone))
                return false;
        }

        if (
            !RunCooperativeObjective(
                "STEP3A_FORGE_WEAPON_ESCHERION",
                () => IsForgeWeaponDone() || core.CheckInventory("1st Lord Of Chaos Helm"),
                () =>
                {
                    if (IsForgeWeaponDone() || core.CheckInventory("1st Lord Of Chaos Helm"))
                        return;
                    core.EnsureAccept(8738);
                    SetClass(ClassType.Solo);
                    core.KillEscherion("1st Lord Of Chaos Helm");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "escherion");
                }
            )
            || !RunSubgroupCooperativeObjective(
                4,
                "STEP3A_FORGE_WEAPON_VATH",
                () => IsForgeWeaponDone() || core.CheckInventory("Chaos Dragonlord Helm"),
                () =>
                {
                    if (IsForgeWeaponDone() || core.CheckInventory("Chaos Dragonlord Helm"))
                        return;
                    core.EnsureAccept(8738);
                    SetClass(ClassType.Solo);
                    core.KillVath("Chaos Dragonlord Helm");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "stalagbite");
                }
            )
            || !RunSubgroupCooperativeObjective(
                4,
                "STEP3A_FORGE_WEAPON_KITSUNE",
                () => IsForgeWeaponDone() || core.CheckInventory("Chaos Shogun Helmet"),
                () =>
                {
                    if (IsForgeWeaponDone() || core.CheckInventory("Chaos Shogun Helmet"))
                        return;
                    core.EnsureAccept(8738);
                    SetClass(ClassType.Solo);
                    core.KillKitsune("Chaos Shogun Helmet");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "kitsune");
                }
            )
            || !RunCoordinatedHuntStep(
                "STEP3A_FORGE_WEAPON_WOLFWING",
                8738,
                "wolfwing",
                "Wolfwing",
                "Wolfwing Mask",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !IsForgeWeaponDone()
            )
            || !RunCoordinatedTurnInStep(
                "STEP3A_FORGE_WEAPON_COMPLETE",
                IsForgeWeaponDone,
                IsForgeWeaponDone,
                () =>
                {
                    core.EnsureAccept(8738);
                    core.EnsureComplete(8738);
                }
            )
        )
            return false;

        // 2. Praxis (Quest 9171 - Another One Bites the Dust, Blacksmithing Rank 6)
        bool IsPraxisDone() => core.isCompletedBefore(9171, log: false);

        if (!IsPraxisDone())
        {
            if (!RunCoordinatedBlacksmithingRankStep("STEP3A_PRAXIS_BS_RANK_6", 6, 12, IsPraxisDone))
                return false;
        }

        if (
            !RunCooperativeObjective(
                "STEP3A_PRAXIS_CHRONOSPAN_REP_10",
                () =>
                    IsPraxisDone()
                    || core.CheckInventory("Thief of Hours Armor")
                    || Farm.FactionRank("ChronoSpan") >= 10,
                () =>
                {
                    if (
                        IsPraxisDone()
                        || core.CheckInventory("Thief of Hours Armor")
                        || Farm.FactionRank("ChronoSpan") >= 10
                    )
                        return;
                    SetClass(ClassType.Farm);
                    Farm.ChronoSpanREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "thespan");
                }
            )
            || !RunCooperativeObjective(
                "STEP3A_PRAXIS_YOKAI_REP_10",
                () =>
                    IsPraxisDone()
                    || core.CheckInventory("Imperial Chunin Clone")
                    || Farm.FactionRank("Yokai") >= 10,
                () =>
                {
                    if (
                        IsPraxisDone()
                        || core.CheckInventory("Imperial Chunin Clone")
                        || Farm.FactionRank("Yokai") >= 10
                    )
                        return;
                    SetClass(ClassType.Farm);
                    Farm.YokaiREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "dragonkoiz");
                }
            )
        )
            return false;

        bool HasPraxisShopArmors() =>
            IsPraxisDone()
            || (
                core.CheckInventory("Thief of Hours Armor")
                && core.CheckInventory("Hashashin Armor")
                && core.CheckInventory("Imperial Chunin Clone")
            );

        if (
            !RunCoordinatedGoldStep(
                "STEP3A_PRAXIS_GOLD",
                250000,
                () => HasPraxisShopArmors() && core.CheckInventory(59465)
            )
            || !RunCoordinatedTurnInStep(
                "STEP3A_PRAXIS_BUY_ARMORS",
                HasPraxisShopArmors,
                HasPraxisShopArmors,
                () =>
                {
                    core.EnsureAccept(9171);
                    if (!core.CheckInventory("Thief of Hours Armor"))
                        Adv.BuyItem("thespan", 439, "Thief of Hours Armor");
                    if (!core.CheckInventory("Hashashin Armor"))
                        Adv.BuyItem("yulgar", 69, "Hashashin Armor");
                    if (!core.CheckInventory("Imperial Chunin Clone"))
                        Adv.BuyItem("dragonkoiz", 95, "Imperial Chunin Clone");
                }
            )
            || !RunCoordinatedHuntStep(
                "STEP3A_PRAXIS_DRAGON_ROGUE",
                9171,
                "ectocave",
                "Ektorax",
                "Dragon Rogue",
                1,
                isTemp: false,
                role: ClassType.Solo,
                softRetryLimit: 0,
                isEnabled: () => !IsPraxisDone()
            )
        )
            return false;

        // /darkally story (Quests 7419-7428) required for Quest 7409 ("The Path to Power" -> Yami)
        bool IsDarkAllyDone() =>
            IsPraxisDone()
            || core.CheckInventory("Yami", 3)
            || core.isCompletedBefore(7428, log: false);

        var darkAllySteps = new CoordinatedStoryStep[]
        {
            new(
                7419,
                "darkally",
                "Shadow",
                () =>
                {
                    Story.MapItemQuest(7419, "darkally", 7179, 6);
                    core.KillMonster("darkally", "r2", "Left", "Shadow", "Shadow Destroyed", 10, log: false);
                    core.EnsureComplete(7419);
                }
            ),
            new(
                7420,
                "darkally",
                "Underworld Golem",
                () =>
                {
                    core.EnsureAccept(7420);
                    core.HuntMonster("darkally", "Underworld Golem", "Underworld Granite", 8, log: false);
                    core.HuntMonster("darkally", "Underworld Golem", "Underworld Diamond", 8, log: false);
                    core.EnsureComplete(7420);
                }
            ),
            new(
                7421,
                "darkally",
                "*",
                () =>
                {
                    Story.MapItemQuest(7421, "darkally", 7180, 1);
                    Story.MapItemQuest(7421, "darkally", 7181, 8);
                }
            ),
            new(
                7422,
                "darkally",
                "Dark Makai",
                () =>
                {
                    core.EnsureAccept(7422);
                    core.Join("darkally", "r2", "Left");
                    while (!Bot.ShouldExit && !core.CheckInventory(53855, 10))
                        core.KillMonster("darkally", "r2", "Left", "Dark Makai", log: false);
                    core.EnsureComplete(7422);
                }
            ),
            new(
                7423,
                "darkally",
                "*",
                () =>
                {
                    core.EnsureAccept(7423);
                    core.KillMonster("darkally", "r2", "Left", 4452, "Shredded Shadow", 9, log: false);
                    core.EnsureComplete(7423);
                }
            ),
            new(
                7424,
                "darkally",
                "Legion Defector",
                () =>
                {
                    core.EnsureAccept(7424);
                    core.HuntMonster("darkally", "Legion Defector", "Defectors Slain", 10, log: false);
                    core.HuntMonster("darkally", "Legion Defector", "Legion Soul", 5, log: false);
                    core.EnsureComplete(7424);
                }
            ),
            new(
                7425,
                "darkally",
                "Frozen Pyromancer",
                () => Story.KillQuest(7425, "darkally", "Frozen Pyromancer"),
                ClassType.Solo
            ),
            new(
                7426,
                "darkally",
                "Underworld Golem",
                () => Story.KillQuest(7426, "darkally", "Underworld Golem")
            ),
            new(7427, "darkally", "*", () => Story.MapItemQuest(7427, "darkally", 7182, 1)),
            new(
                7428,
                "darkally",
                "Underfiend",
                () => Story.KillQuest(7428, "darkally", "Underfiend"),
                ClassType.Solo
            ),
        };

        if (!RunCoordinatedStoryChain("STEP3A_PRAXIS_DARKALLY", darkAllySteps, IsDarkAllyDone))
            return false;

        if (
            !RunCooperativeObjective(
                "STEP3A_PRAXIS_YAMI",
                () => IsPraxisDone() || core.CheckInventory("Yami", 3),
                () =>
                {
                    if (IsPraxisDone() || core.CheckInventory("Yami", 3))
                        return;
                    core.EnsureAccept(9171);
                    core.AddDrop("Yami");
                    SetClass(ClassType.Farm);
                    core.RegisterQuests(7409);
                    while (!Bot.ShouldExit && !core.CheckInventory("Yami", 3))
                        core.KillMonster("darkally", "r2", "Left", "*", log: false);
                    core.CancelRegisteredQuests();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", targetMap: "darkally");
                }
            )
        )
            return false;

        ApplySubgroupRoomSettings(4);
        try
        {
            if (
                !RunCooperativeObjective(
                    "STEP3A_PRAXIS_DS_TOKENS",
                    () =>
                        IsPraxisDone()
                        || core.CheckInventory(59465)
                        || core.CheckInventory("Dragon Shinobi Token", 100),
                    () =>
                    {
                        if (
                            IsPraxisDone()
                            || core.CheckInventory(59465)
                            || core.CheckInventory("Dragon Shinobi Token", 100)
                        )
                            return;
                        ApplyCurrentArmyRoomSettings();
                        core.EnsureAccept(9171);
                        if (!core.isCompletedBefore(7924, log: false))
                        {
                            SetClass(ClassType.Solo);
                            core.EnsureAccept(7924);
                            core.HuntMonster(
                                "shadowfortress",
                                "1st Head of Orochi",
                                "Perfectly Preserved Dragon Tooth",
                                log: false
                            );
                            core.EnsureComplete(7924);
                        }
                        core.AddDrop("Dragon Shinobi Token");
                        SetClass(ClassType.Solo);
                        core.RegisterQuests(7924);
                        while (!Bot.ShouldExit && !core.CheckInventory("Dragon Shinobi Token", 100))
                            core.HuntMonster(
                                "shadowfortress",
                                "1st Head of Orochi",
                                "Perfectly Preserved Dragon Tooth",
                                log: false
                            );
                        core.CancelRegisteredQuests();
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "1st Head of Orochi",
                            targetMap: "shadowfortress"
                        );
                    },
                    maxPlayersPerSubgroup: 4,
                    prepareForCombat: () => ApplyCurrentArmyRoomSettings()
                )
            )
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        if (
            !RunCoordinatedTurnInStep(
                "STEP3A_PRAXIS_COMPLETE",
                IsPraxisDone,
                IsPraxisDone,
                () =>
                {
                    core.EnsureAccept(9171);
                    if (!core.CheckInventory(59465))
                        Adv.BuyItem("shadowfortress", 1968, 59465, shopItemID: 6869);
                    core.EnsureComplete(9171);
                }
            )
            || !RunSubgroupCooperativeObjective(
                4,
                "STEP3A_FORGE_SMITE",
                () => core.isCompletedBefore(8740, log: false),
                () =>
                {
                    if (core.isCompletedBefore(8740, log: false))
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Solo);
                    ForgeUnlock.Smite();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
            )
        )
            return false;

        ApplySubgroupRoomSettings(4);
        try
        {
            if (
                !RunCooperativeObjective(
                    "STEP3A_FORGE_CAPE",
                    () => core.isCompletedBefore(8758, log: false),
                    () =>
                    {
                        if (core.isCompletedBefore(8758, log: false))
                            return;

                        ApplyCurrentArmyRoomSettings();
                        PrepareCombatLoadoutForArmy(ClassType.Solo);
                        ForgeUnlock.ForgeCapeEnhancement();
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                    },
                    maxPlayersPerSubgroup: 4,
                    prepareForCombat: () =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        PrepareCombatLoadoutForArmy(ClassType.Solo);
                    }
                )
            )
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        if (
            !RunCoordinatedTurnInStep(
                "STEP3A_FORGE_ENHANCE_EQUIPPED",
                () => false,
                () => true,
                () =>
                {
                    if (!ShouldPreserveEquippedGear() && !IsDisableAutoEnhanceEnabled())
                    {
                        string? currentClass = Bot.Player.CurrentClass?.Name;
                        if (!string.IsNullOrWhiteSpace(currentClass))
                        {
                            ApplyAndRecordSmartEnhance(currentClass);
                            Army.StartClassSkills(currentClass);
                        }
                    }
                }
            )
        )
            return false;

        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);
        return !Bot.ShouldExit;
    }
    #endregion Step 3A: Level 75 Progression (13 LoC, LoO Daily, Additional Classes & Forge Weapons)

    #region Step 3B: Level 80 Progression (BBoA, Dragon of Time, ArchFiend, Yami no Ronin, Dage's Scroll Fragment Daily)
    /// <summary>
    /// Step 3B: Level 80 Progression.
    /// - Level 80 + Enhance + Best Boost Gear
    /// - Dailies: The Encroaching Shadows (8653), LoO (7156-7165), Dage's Scroll Fragment (3596)
    /// - Burning Blade of Abezeth (+30% All Damage, solo-only /celestialarenad)
    /// - Dragon of Time (subgroup 1-4 / 5-7 for /towerofdoom*, bypass via EnsureMapAccessBypass)
    /// - ArchFiend (50x Bone Dust pre-check + subgroup 1-4 / 5-7 for /tercessuinotlim)
    /// - Yami no Ronin (synchronized Fail to the King [793] Legion vs Non-Legion route)
    /// </summary>
    public bool RunStep3B()
    {
        CoreBots core = CoreBots.Instance;
        core.Logger(
            "Step 3B: Starting Level 80 Progression (BBoA, Dragon of Time, ArchFiend, Yami no Ronin & Dage's Scroll Fragment)..."
        );

        if (!Army.SyncArmy("STEP3B_80_START"))
            return false;

        if (!RunBracketLevelAndEnhance("STEP3B_LEVEL_80", 80))
            return false;

        EnsureBestStaticBoostGearEquipped();

        if (!RunTheEncroachingShadowsDaily())
            return false;

        if (!RunLordOfOrderProgression("STEP3B"))
            return false;

        BankQuestExtras(7156, "Lord of Order");

        if (!RunDagesScrollFragmentDaily())
            return false;

        if (!RunStep3BBurningBladeOfAbezeth())
            return false;

        if (!RunStep3BDragonOfTime())
            return false;

        if (!RunStep3BArchFiend())
            return false;

        if (!RunStep3BYamiNoRonin())
            return false;

        EnsureBestStaticBoostGearEquipped();
        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);

        if (!Army.SyncArmy("STEP3B_80_END"))
            return false;

        core.Logger("Step 3B: Level 80 Progression completed.");
        return !Bot.ShouldExit;
    }

    private bool RunStep3BBurningBladeOfAbezeth()
    {
        CoreBots core = CoreBots.Instance;
        bool HasBBoAOrSkip() =>
            ShouldPreserveEquippedGear()
            || core.CheckInventory("Burning Blade Of Abezeth", 1, toInv: false);

        // /celestialarena* and /celestialarenad are strict 1-player solo maps; helpers cannot join.
        if (
            !RunCooperativeObjective(
                "STEP3B_BBOA",
                HasBBoAOrSkip,
                () =>
                {
                    if (HasBBoAOrSkip())
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Solo);
                    BBoA.GetBBoA();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
            )
        )
            return false;

        EnsureBestStaticBoostGearEquipped();
        return !Bot.ShouldExit;
    }

    private bool RunStep3BDragonOfTime()
    {
        CoreBots core = CoreBots.Instance;
        bool HasDoT() => core.CheckInventory("Dragon of Time", 1, toInv: false);

        // /towerofdoom* has a 6-player room cap: split into Group 0 (Players 1-4) and Group 1 (Players 5-7).
        if (!HasDoT())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP3B_DOT_CLASS",
                        HasDoT,
                        () =>
                        {
                            if (HasDoT())
                                return;

                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            DoT.GetDoT(rankUpClass: false, doExtra: false);
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP3B_DOT_CLASS_SKIP"))
            return false;

        RankUpClassForArmy("Dragon of Time");
        return !Bot.ShouldExit;
    }

    private bool RunStep3BArchFiend()
    {
        CoreBots core = CoreBots.Instance;
        bool HasArchFiend() => core.CheckInventory("ArchFiend", 1, toInv: false);

        // All accounts (including helpers) need 50 Bone Dust in inventory to enter /tercessuinotlim.
        if (
            !RunCoordinatedCombatStep(
                "STEP3B_AF_BONE_DUST",
                () => HasArchFiend() || core.CheckInventory("Bone Dust", 50),
                () => HasArchFiend() || core.CheckInventory("Bone Dust", 50),
                "*",
                ClassType.Farm,
                () =>
                {
                    if (HasArchFiend() || core.CheckInventory("Bone Dust", 50))
                        return;
                    core.HuntMonster("battleunderb", "Skeleton Warrior", "Bone Dust", 50, isTemp: false);
                },
                targetMap: "battleunderb"
            )
        )
            return false;

        // /tercessuinotlim has a 6-player room cap: split into Group 0 (Players 1-4) and Group 1 (Players 5-7).
        if (!HasArchFiend())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP3B_AF_CLASS",
                        HasArchFiend,
                        () =>
                        {
                            if (HasArchFiend())
                                return;

                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            AF.GetArchfiend(rankUp: false);
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP3B_AF_CLASS_SKIP"))
            return false;

        RankUpClassForArmy("ArchFiend");
        return !Bot.ShouldExit;
    }

    private bool RunStep3BYamiNoRonin()
    {
        CoreBots core = CoreBots.Instance;
        bool HasYnR() => core.CheckInventory("Yami no Ronin", 1, toInv: false);

        bool allLegion = Army.AllPlayersSatisfy(
            "YNR_FAIL_TO_THE_KING_793",
            () => core.isCompletedBefore(793, log: false)
        );
        core.Logger(
            $"Step 3B: Yami no Ronin route selected -> {(allLegion ? "Legion" : "Non-Legion")} (Fail to the King [793] check across Army)."
        );

        if (
            !RunCooperativeObjective(
                "STEP3B_YNR_BLADEMASTER_SCROLL",
                () => HasYnR() || core.CheckInventory("Blademaster Sword Scroll", 1, toInv: false),
                () =>
                {
                    if (HasYnR() || core.CheckInventory("Blademaster Sword Scroll", 1, toInv: false))
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Solo);
                    YnR.BlademasterSwordScroll(nonLegion: !allLegion);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
            )
            || !RunCooperativeObjective(
                "STEP3B_YNR_YOKAI_SCROLL",
                () => HasYnR() || core.CheckInventory("Yokai Sword Scroll", 1, toInv: false),
                () =>
                {
                    if (HasYnR() || core.CheckInventory("Yokai Sword Scroll", 1, toInv: false))
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Farm);
                    YnR.YokaiSwordScroll();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Farm)
            )
            || !RunCoordinatedTurnInStep(
                "STEP3B_YNR_TURNIN",
                HasYnR,
                HasYnR,
                () =>
                {
                    if (HasYnR())
                        return;
                    core.AddDrop("Yami no Ronin");
                    core.EnsureAccept(7408);
                    core.Unbank("Blademaster Sword Scroll", "Yokai Sword Scroll");
                    core.EnsureComplete(7408);
                    Bot.Wait.ForPickup("Yami no Ronin");
                }
            )
        )
            return false;

        RankUpClassForArmy("Yami no Ronin");
        return !Bot.ShouldExit;
    }
    #endregion Step 3B: Level 80 Progression

    #region Step 3C: Level 90 Progression (Verus DoomKnight & Forge Capes)
    /// <summary>
    /// Step 3C: Level 90 Progression.
    /// - Level 90 + Enhance + Best Boost Gear
    /// - Dailies: The Encroaching Shadows (8653), LoO (7156-7165), Dage's Scroll Fragment (3596)
    /// - Verus DoomKnight (Rank 10 Evil + coordinated /terminatemple questline + Rank 10)
    /// - Forge Capes (Blacksmithing Rank 9 + Good Rank 10 + Absolution -> Vainglory -> Avarice -> Penitence -> Lament)
    /// </summary>
    public bool RunStep3C()
    {
        CoreBots core = CoreBots.Instance;
        core.Logger("Step 3C: Starting Level 90 Progression (Verus DoomKnight & Forge Capes)...");

        if (!Army.SyncArmy("STEP3C_90_START"))
            return false;

        if (!RunBracketLevelAndEnhance("STEP3C_LEVEL_90", 90))
            return false;

        EnsureBestStaticBoostGearEquipped();

        if (!RunTheEncroachingShadowsDaily())
            return false;

        if (!RunLordOfOrderProgression("STEP3C"))
            return false;

        BankQuestExtras(7156, "Lord of Order");

        if (!RunDagesScrollFragmentDaily())
            return false;

        if (!RunStep3CVerusDoomKnight())
            return false;

        if (!RunStep3CForgeCapes())
            return false;

        if (!RunStep3CScionOfFlames())
            return false;

        if (!RunStep3CLibertaAeterna())
            return false;

        if (!RunStep3CKathoolAdept())
            return false;

        EnsureBestStaticBoostGearEquipped();
        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);

        if (!Army.SyncArmy("STEP3C_90_END"))
            return false;

        core.Logger("Step 3C: Level 90 Progression completed.");
        return !Bot.ShouldExit;
    }

    private bool RunStep3CVerusDoomKnight()
    {
        CoreBots core = CoreBots.Instance;
        bool HasVDK() => core.CheckInventory("Verus DoomKnight", 1, toInv: false);

        if (
            !RunCooperativeObjective(
                "STEP3C_VDK_EVIL_REP_10",
                () => HasVDK() || Farm.FactionRank("Evil") >= 10,
                () =>
                {
                    if (HasVDK() || Farm.FactionRank("Evil") >= 10)
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Farm);
                    Farm.EvilREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Farm)
            )
            || !RunCooperativeObjective(
                "STEP3C_VDK_CLASS",
                HasVDK,
                () =>
                {
                    if (HasVDK())
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Solo);
                    VDK.GetClass(rankup: false);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
            )
        )
            return false;

        RankUpClassForArmy("Verus DoomKnight");
        return !Bot.ShouldExit;
    }

    private bool RunStep3CForgeCapes()
    {
        CoreBots core = CoreBots.Instance;
        bool IsAllCapesDone() => core.isCompletedBefore(8823, log: false);

        if (
            !RunCoordinatedBlacksmithingRankStep(
                "STEP3C_FORGE_CAPE_BS_RANK9",
                9,
                45,
                () => IsAllCapesDone() || core.isCompletedBefore(8743, log: false)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "STEP3C_FORGE_CAPE_GOOD_REP10",
                () => IsAllCapesDone() || core.isCompletedBefore(8743, log: false) || Farm.FactionRank("Good") >= 10,
                () =>
                {
                    if (IsAllCapesDone() || core.isCompletedBefore(8743, log: false) || Farm.FactionRank("Good") >= 10)
                        return;

                    EnsureBestStaticBoostGearEquipped(BoostObjective.Rep);
                    PrepareCombatLoadoutForArmy(ClassType.Farm);
                    Farm.GoodREP(10);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Farm)
            )
        )
            return false;

        var capeSteps = new (string Objective, int QuestId, Action UnlockAction)[]
        {
            ("STEP3C_FORGE_CAPE_BASE", 8758, () => ForgeUnlock.ForgeCapeEnhancement()),
            ("STEP3C_FORGE_CAPE_ABSOLUTION", 8743, () => ForgeUnlock.Absolution()),
            ("STEP3C_FORGE_CAPE_VAINGLORY", 8744, () => ForgeUnlock.Vainglory()),
            ("STEP3C_FORGE_CAPE_AVARICE", 8745, () => ForgeUnlock.Avarice()),
            ("STEP3C_FORGE_CAPE_PENITENCE", 8822, () => ForgeUnlock.Penitence()),
            ("STEP3C_FORGE_CAPE_LAMENT", 8823, () => ForgeUnlock.Lament()),
        };

        ApplySubgroupRoomSettings(4);
        try
        {
            foreach ((string objective, int questId, Action unlockAction) in capeSteps)
            {
                bool IsCapeDone() => core.isCompletedBefore(questId, log: false);
                if (
                    !RunCooperativeObjective(
                        objective,
                        IsCapeDone,
                        () =>
                        {
                            if (IsCapeDone())
                                return;

                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            unlockAction();
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        maxPlayersPerSubgroup: 4,
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        // Restore best boost Armor after /sloth (HazMat Suit used in Lament) and reset enhancement cache
        // so the next combat loadout re-applies the newly unlocked Forge Cape enhancements.
        LeaveCombatRoomForEquipmentChange();
        EnsureBestStaticBoostGearEquipped();
        _lastSmartEnhancedClass = null;
        return !Bot.ShouldExit;
    }

    private bool RunStep3CScionOfFlames()
    {
        CoreBots core = CoreBots.Instance;
        bool HasSOF() => core.CheckInventory("Scion of Flames", 1, toInv: false);

        if (!HasSOF())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP3C_SCION_OF_FLAMES",
                        HasSOF,
                        () =>
                        {
                            if (HasSOF())
                                return;

                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                            SOFCore.GetScionofFlames(rankUpClass: false);
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(
                                targetPlayerName,
                                "*",
                                compositionType: ClassType.Boss,
                                copyHelpedActions: true
                            );
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP3C_SCION_OF_FLAMES_SKIP"))
            return false;

        if (HasSOF())
            RankUpClassForArmy("Scion of Flames");

        return !Bot.ShouldExit;
    }

    private bool RunStep3AChaorrupterUnlocked()
    {
        CoreBots core = CoreBots.Instance;
        bool HasChaorrupter() => core.CheckInventory("Chaorrupter Unlocked", 1, toInv: false);

        return RunCooperativeObjective(
            "STEP3A_CHAORRUPTER_UNLOCKED",
            HasChaorrupter,
            () =>
            {
                if (HasChaorrupter())
                    return;

                PrepareCombatLoadoutForArmy(ClassType.Farm);
                ChaorrupterCore.GetYourSword();
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm);
            },
            prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Farm)
        );
    }

    private bool RunStep3CLibertaAeterna()
    {
        CoreBots core = CoreBots.Instance;
        bool HasLiberta() => core.CheckInventory("Liberta Aeterna", 1, toInv: false);

        return RunCooperativeObjective(
            "STEP3C_LIBERTA_AETERNA",
            HasLiberta,
            () =>
            {
                if (HasLiberta())
                    return;

                PrepareCombatLoadoutForArmy(ClassType.Boss);
                ROACCore.BuyAllMerge("Liberta Aeterna");
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Boss);
            },
            prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Boss)
        );
    }

    private bool RunStep3CKathoolAdept()
    {
        CoreBots core = CoreBots.Instance;
        bool HasKA() => core.CheckInventory("Kathool Adept", 1, toInv: false);

        if (!HasKA())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP3C_KATHOOL_ADEPT",
                        HasKA,
                        () =>
                        {
                            if (HasKA())
                                return;

                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                            KACore.GetClass(rankUpClass: false);
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(
                                targetPlayerName,
                                "*",
                                compositionType: ClassType.Boss,
                                copyHelpedActions: true
                            );
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP3C_KATHOOL_ADEPT_SKIP"))
            return false;

        if (HasKA())
            RankUpClassForArmy("Kathool Adept");

        return !Bot.ShouldExit;
    }
    #endregion Step 3C: Level 90 Progression

    #region Step 4A: Level 100 Endgame Progression (VHL, AM, AI, Valiance, Ravenous, Infernal ArchFiend, Hollowborn Reaper's Scythe & Awescended)
    /// <summary>
    /// Step 4A: Level 100 Endgame Progression (no weekly Ultra Bosses).
    /// - Level 100 + Enhance + Best Boost Gear
    /// - Dailies: The Encroaching Shadows (8653), LoO (7156-7165), Dage's Scroll Fragment (3596)
    /// - Endgame Classes: Void Highlord, ArchMage, Arcana Invoker (Non-Insignia)
    /// - Forge Weapons: Hero's Valiance (8741) & Ravenous (9560)
    /// - Endgame Gear: Infernal ArchFiend (+40% All Races), Hollowborn Reaper's Scythe (+40% All Damage)
    /// - Final Questline: Awe-scention (Awescended)
    /// </summary>
    public bool RunStep4A()
    {
        CoreBots core = CoreBots.Instance;
        core.Logger(
            "Step 4A: Starting Level 100 Endgame Progression (VHL, AM, AI, Valiance, Ravenous, Infernal ArchFiend, Hollowborn Reaper's Scythe & Awescended)..."
        );

        if (!Army.SyncArmy("STEP4A_100_START"))
            return false;

        if (!RunBracketLevelAndEnhance("STEP4A_LEVEL_100", 100))
            return false;

        EnsureBestStaticBoostGearEquipped();

        if (!RunTheEncroachingShadowsDaily())
            return false;

        if (!RunLordOfOrderProgression("STEP4A"))
            return false;

        BankQuestExtras(7156, "Lord of Order");

        if (!RunDagesScrollFragmentDaily())
            return false;

        if (!RunStep4AVoidHighlord())
            return false;

        if (!RunStep4AArchMage())
            return false;

        if (!RunStep4AArcanaInvoker())
            return false;

        if (!RunStep4AValiance())
            return false;

        if (!RunStep4ARavenousAndInfernalArchFiend())
            return false;

        if (!RunStep4AHollowbornReapersScythe())
            return false;

        if (!RunStep4AAwescended())
            return false;

        if (!RunStep4AElysium())
            return false;

        EnsureBestStaticBoostGearEquipped();
        _lastSmartEnhancedClass = null;
        _syncedClassType = ClassType.None;
        SetClass(rankUpIfNeeded: false);

        if (!Army.SyncArmy("STEP4A_100_END"))
            return false;

        core.Logger("Step 4A: Level 100 Endgame Progression completed.");
        return !Bot.ShouldExit;
    }

    private bool RunStep4AClassUnlock(
        string objectivePrefix,
        string className,
        Action unlockWithoutRankUp
    )
    {
        CoreBots core = CoreBots.Instance;
        bool HasClass() => core.CheckInventory(className, 1, toInv: false);

        if (
            !RunCooperativeObjective(
                objectivePrefix,
                HasClass,
                () =>
                {
                    if (HasClass())
                        return;

                    PrepareCombatLoadoutForArmy(ClassType.Solo);
                    unlockWithoutRankUp();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                },
                prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
            )
        )
            return false;

        if (HasClass())
            RankUpClassForArmy(className);

        return !Bot.ShouldExit;
    }

    private bool RunStep4AVoidHighlord() =>
        RunStep4AClassUnlock("STEP4A_VHL", "Void Highlord", () => VHLCore.GetVHL(rankUpClass: false));

    private bool RunStep4AArchMage() =>
        RunStep4AClassUnlock("STEP4A_AM", "ArchMage", () => AMCore.GetAM(rankUpClass: false));

    private bool RunStep4AArcanaInvoker() =>
        RunStep4AClassUnlock("STEP4A_AI", "Arcana Invoker", () => AICore.GetAI(rankUpClass: false));

    private bool RunStep4AValiance()
    {
        CoreBots core = CoreBots.Instance;
        bool IsValianceDone() => core.isCompletedBefore(8741, log: false);

        if (
            !RunCoordinatedBlacksmithingRankStep(
                "STEP4A_VALIANCE_BS_RANK10",
                10,
                55,
                IsValianceDone
            )
        )
            return false;

        if (!IsValianceDone())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP4A_VALIANCE_UNLOCK",
                        IsValianceDone,
                        () =>
                        {
                            if (IsValianceDone())
                                return;

                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            ForgeUnlock.HerosValiance();
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP4A_VALIANCE_SKIP"))
            return false;

        _lastSmartEnhancedClass = null;
        return !Bot.ShouldExit;
    }

    private bool RunStep4ARavenousAndInfernalArchFiend()
    {
        CoreBots core = CoreBots.Instance;
        bool IsRavenousDone() => core.isCompletedBefore(9560, log: false);
        bool HasInfernalArchFiend() => core.CheckInventory("Infernal ArchFiend", 1, toInv: false);

        // Ensure 50 Bone Dust in case Prime Fiend Shard / Nulgath steps enter /tercessuinotlim.
        if (
            !RunCoordinatedCombatStep(
                "STEP4A_RAVENOUS_BONE_DUST",
                () => IsRavenousDone() || core.CheckInventory("Bone Dust", 50),
                () => IsRavenousDone() || core.CheckInventory("Bone Dust", 50),
                "*",
                ClassType.Farm,
                () =>
                {
                    if (IsRavenousDone() || core.CheckInventory("Bone Dust", 50))
                        return;
                    core.HuntMonster("battleunderb", "Skeleton Warrior", "Bone Dust", 50, isTemp: false);
                },
                targetMap: "battleunderb"
            )
        )
            return false;

        if (!IsRavenousDone())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP4A_RAVENOUS_UNLOCK",
                        IsRavenousDone,
                        () =>
                        {
                            if (IsRavenousDone())
                                return;

                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            ForgeUnlock.Ravenous();
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP4A_RAVENOUS_SKIP"))
            return false;

        if (
            !RunCoordinatedTurnInStep(
                "STEP4A_BUY_INFERNAL_ARCHFIEND",
                () => !IsRavenousDone() || HasInfernalArchFiend(),
                () => !IsRavenousDone() || HasInfernalArchFiend(),
                () =>
                {
                    if (!IsRavenousDone() || HasInfernalArchFiend())
                        return;

                    core.AddDrop("Infernal ArchFiend");
                    Adv.BuyItem("fiendshard", 2411, "Infernal ArchFiend");
                    Bot.Wait.ForPickup("Infernal ArchFiend");
                }
            )
        )
            return false;

        _lastSmartEnhancedClass = null;
        return !Bot.ShouldExit;
    }

    private bool RunStep4AHollowbornReapersScythe()
    {
        CoreBots core = CoreBots.Instance;
        bool HasScythe() => core.CheckInventory("Hollowborn Reaper's Scythe", 1, toInv: false);

        return RunCooperativeObjective(
            "STEP4A_HOLLOWBORN_SCYTHE",
            HasScythe,
            () =>
            {
                if (HasScythe())
                    return;

                PrepareCombatLoadoutForArmy(ClassType.Solo);
                SRM.BuyAllMerge("Hollowborn Reaper's Scythe");
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
            },
            prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
        );
    }

    private bool RunStep4AAwescended()
    {
        CoreBots core = CoreBots.Instance;
        bool IsAwescendedDone() =>
            core.isCompletedBefore(8042, log: false) && core.CheckInventory("Awescended", 1, toInv: false);

        return RunSubgroupCooperativeObjective(
            4,
            "STEP4A_AWESCENDED",
            IsAwescendedDone,
            () =>
            {
                if (IsAwescendedDone())
                    return;

                PrepareCombatLoadoutForArmy(ClassType.Solo);
                AwescendedCore.GetAwe();
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
            },
            prepareForCombat: () => PrepareCombatLoadoutForArmy(ClassType.Solo)
        );
    }

    private bool RunStep4AElysium()
    {
        CoreBots core = CoreBots.Instance;
        bool IsElysiumDone() => core.isCompletedBefore(8821, log: false);

        if (
            !RunCoordinatedBlacksmithingRankStep(
                "STEP4A_ELYSIUM_BS_RANK10",
                10,
                55,
                IsElysiumDone
            )
        )
            return false;

        if (!IsElysiumDone())
        {
            ApplySubgroupRoomSettings(4);
            try
            {
                if (
                    !RunCooperativeObjective(
                        "STEP4A_ELYSIUM_CELESTIAL_PAST",
                        () => IsElysiumDone() || core.isCompletedBefore(7681, log: false),
                        () =>
                        {
                            if (IsElysiumDone() || core.isCompletedBefore(7681, log: false))
                                return;

                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            CelestialPastCore.CompleteCeletialPast();
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;

                if (
                    !RunCooperativeObjective(
                        "STEP4A_ELYSIUM_AZALITH",
                        () => IsElysiumDone() || core.CheckInventory("The Divine Will"),
                        () =>
                        {
                            if (IsElysiumDone() || core.CheckInventory("The Divine Will"))
                                return;

                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                            core.HuntMonster("celestialpast", "Azalith", "The Divine Will", 1, isTemp: false);
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(
                                targetPlayerName,
                                "Azalith",
                                targetMap: "celestialpast",
                                compositionType: ClassType.Boss
                            );
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Boss);
                        }
                    )
                )
                    return false;

                if (
                    !RunCooperativeObjective(
                        "STEP4A_ELYSIUM_UNLOCK",
                        IsElysiumDone,
                        () =>
                        {
                            if (IsElysiumDone())
                                return;

                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                            ForgeUnlock.Elysium();
                        },
                        (targetPlayerNumber, targetPlayerName) =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Solo);
                        },
                        prepareForCombat: () =>
                        {
                            ApplyCurrentArmyRoomSettings();
                            PrepareCombatLoadoutForArmy(ClassType.Solo);
                        }
                    )
                )
                    return false;
            }
            finally
            {
                ApplyArmyRoomSettings();
            }
        }
        else if (!Army.SyncArmy("STEP4A_ELYSIUM_SKIP"))
            return false;

        return !Bot.ShouldExit;
    }
    #endregion Step 4A: Level 100 Endgame Progression

    /// <summary>
    /// Coordinates /frozentower Quests 3907-3937 (Slot 185, Value 1..31) for Cryomancer and Glacera Story.
    /// </summary>
    private bool RunCoordinatedFrozenTower(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_FROZEN_TOWER_STORY",
                () => skipCondition() || core.isCompletedBefore(3937, log: false)
            )
        )
            return true;

        core.EnsureLoad(Enumerable.Range(3907, 31).ToArray());

        bool IsFTQuestDone(int questId) =>
            skipCondition() || core.isCompletedBefore(3937, log: false) || core.isCompletedBefore(questId, log: false);

        // 3907: Seek the Tower
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3907",
                () => IsFTQuestDone(3907),
                () =>
                {
                    if (IsFTQuestDone(3907))
                        return;
                    Story.MapItemQuest(3907, "frozentower", 3022);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3908: A n-Ice Beginning
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3908",
                () => IsFTQuestDone(3908),
                () =>
                {
                    if (IsFTQuestDone(3908))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3908, "frozentower", "Polar Elemental");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3909: Search for Syrrus
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3909",
                () => IsFTQuestDone(3909),
                () =>
                {
                    if (IsFTQuestDone(3909))
                        return;
                    Story.MapItemQuest(3909, "frozentower", 3019);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3910: Building the Base
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3910",
                () => IsFTQuestDone(3910),
                () =>
                {
                    if (IsFTQuestDone(3910))
                        return;
                    Story.MapItemQuest(3910, "frozentower", 3004, 13);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3911: Refugee Roundup
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3911",
                () => IsFTQuestDone(3911),
                () =>
                {
                    if (IsFTQuestDone(3911))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3911);
                    core.HuntMonster("frozentower", "Frostwyrm", "Refugee Saved", 7);
                    core.EnsureComplete(3911);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frostwyrm", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3912: Retrieve the Water Starstone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3912",
                () => IsFTQuestDone(3912),
                () =>
                {
                    if (IsFTQuestDone(3912))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(3912, "frozentower", "FrostDeep Dweller");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "FrostDeep Dweller")
            )
        )
            return false;

        // 3913: Magical Attraction
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3913",
                () => IsFTQuestDone(3913),
                () =>
                {
                    if (IsFTQuestDone(3913))
                        return;
                    Story.MapItemQuest(3913, "frozentower", 3005, 13);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3914: Frozen Blood
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3914",
                () => IsFTQuestDone(3914),
                () =>
                {
                    if (IsFTQuestDone(3914))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3914, "frozentower", "Twisted Ice");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Twisted Ice", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3915: Retrieve the Fire Starstone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3915",
                () => IsFTQuestDone(3915),
                () =>
                {
                    if (IsFTQuestDone(3915))
                        return;
                    Story.MapItemQuest(3915, "frozentower", 3006);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3916: Defend the Tower!
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3916",
                () => IsFTQuestDone(3916),
                () =>
                {
                    if (IsFTQuestDone(3916))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3916, "frozentower", "Polar Elemental");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3917: Refugee Rescue Run
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3917",
                () => IsFTQuestDone(3917),
                () =>
                {
                    if (IsFTQuestDone(3917))
                        return;
                    Story.MapItemQuest(3917, "frozentower", 3007, 6);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3918: Retrieve the Earth Starstone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3918",
                () => IsFTQuestDone(3918),
                () =>
                {
                    if (IsFTQuestDone(3918))
                        return;
                    Story.MapItemQuest(3918, "frozentower", 3013);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3919: Polar Penetration and Progress
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3919",
                () => IsFTQuestDone(3919),
                () =>
                {
                    if (IsFTQuestDone(3919))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3919);
                    core.GetMapItem(3008, 6, "frozentower");
                    core.HuntMonster("frozentower", "Polar Elemental", "Ice Sample", 6);
                    core.EnsureComplete(3919);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3920: Save the Astronomer Apprentice
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3920",
                () => IsFTQuestDone(3920),
                () =>
                {
                    if (IsFTQuestDone(3920))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3920);
                    core.GetMapItem(3020, 1, "frozentower");
                    core.HuntMonster("frozentower", "Polar Elemental", "Polar Elemental Guard Defeated", 4);
                    core.HuntMonster("frozentower", "Ice Wolf", "Ice Wolf Guard Defeated", 5);
                    core.EnsureComplete(3920);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3921: Glacial Elixir
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3921",
                () => IsFTQuestDone(3921),
                () =>
                {
                    if (IsFTQuestDone(3921))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(3921);
                    core.GetMapItem(3017, 6, "frozentower");
                    core.HuntMonster("frozentower", "FrostDeep Dweller", "FrostDeep Venom", 1);
                    core.EnsureComplete(3921);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "FrostDeep Dweller")
            )
        )
            return false;

        // 3922: Retrieve the Energy Starstone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3922",
                () => IsFTQuestDone(3922),
                () =>
                {
                    if (IsFTQuestDone(3922))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3922, "frozentower", "Polar Elemental");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3923: Marking the Future
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3923",
                () => IsFTQuestDone(3923),
                () =>
                {
                    if (IsFTQuestDone(3923))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3923, "frozentower", "Frostwyrm");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frostwyrm", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3924: Glacial Shift
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3924",
                () => IsFTQuestDone(3924),
                () =>
                {
                    if (IsFTQuestDone(3924))
                        return;
                    Story.MapItemQuest(3924, "frozentower", 3009, 6);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3925: Divination Draft
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3925",
                () => IsFTQuestDone(3925),
                () =>
                {
                    if (IsFTQuestDone(3925))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3925);
                    core.GetMapItem(3012, 4, "frozentower");
                    core.GetMapItem(3011, 4, "frozentower");
                    core.HuntMonster("frozentower", "Frostwyrm", "Frozen Tail", 5);
                    core.HuntMonster("frozentower", "Arctic Eel", "Frozen Scale", 5);
                    core.EnsureComplete(3925);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3926: Retrieve the Light StarStone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3926",
                () => IsFTQuestDone(3926),
                () =>
                {
                    if (IsFTQuestDone(3926))
                        return;
                    Story.MapItemQuest(3926, "frozentower", 3021);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3927: The Future is Bright
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3927",
                () => IsFTQuestDone(3927),
                () =>
                {
                    if (IsFTQuestDone(3927))
                        return;
                    Story.MapItemQuest(3927, "frozentower", 3014, 7);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3928: Bled Bone Dry
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3928",
                () => IsFTQuestDone(3928),
                () =>
                {
                    if (IsFTQuestDone(3928))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3928, "frozentower", "Arctic Eel");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Arctic Eel", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3929: Chill of Fear
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3929",
                () => IsFTQuestDone(3929),
                () =>
                {
                    if (IsFTQuestDone(3929))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3929, "frozentower", "Polar Elemental");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3930: Retrieve the Darkness Starstone
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3930",
                () => IsFTQuestDone(3930),
                () =>
                {
                    if (IsFTQuestDone(3930))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3930, "frozentower", "Twisted Ice");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Twisted Ice", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3931: Web of Fear
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3931",
                () => IsFTQuestDone(3931),
                () =>
                {
                    if (IsFTQuestDone(3931))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3931, "frozentower", "Frostwyrm");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frostwyrm", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3932: Frozen in Time
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3932",
                () => IsFTQuestDone(3932),
                () =>
                {
                    if (IsFTQuestDone(3932))
                        return;
                    Story.MapItemQuest(3932, "frozentower", 3016, 13);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3933: Heart of the Matter
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3933",
                () => IsFTQuestDone(3933),
                () =>
                {
                    if (IsFTQuestDone(3933))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3933, "frozentower", "Ice Wolf");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Ice Wolf", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3934: Retrieve the Wind Starstone (Boss: Rotten Ice, 29,280 HP)
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3934",
                () => IsFTQuestDone(3934),
                () =>
                {
                    if (IsFTQuestDone(3934))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(3934, "frozentower", "Rotten Ice");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Rotten Ice")
            )
        )
            return false;

        // 3935: Create the Gate
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3935",
                () => IsFTQuestDone(3935),
                () =>
                {
                    if (IsFTQuestDone(3935))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3935);
                    core.GetMapItem(3018, 13, "frozentower");
                    core.HuntMonster("frozentower", "Ice Wolf", "Blood of the Ice Wolf", 1);
                    core.EnsureComplete(3935);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Ice Wolf", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3936: Drive Back the Invaders
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3936",
                () => IsFTQuestDone(3936),
                () =>
                {
                    if (IsFTQuestDone(3936))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3936);
                    core.HuntMonster("frozentower", "Frost Invader", "FrostSpawn Invader defeated", 10);
                    core.EnsureComplete(3936);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Invader", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3937: Defeat the FrostSpawn Invaders
        if (
            !RunCooperativeObjective(
                "BRACKET55_FT_3937",
                () => IsFTQuestDone(3937),
                () =>
                {
                    if (IsFTQuestDone(3937))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3937);
                    core.HuntMonster("frozentower", "Frost Fangbeast", "Fangbeasts defeated", 15);
                    core.EnsureComplete(3937);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Fangbeast", compositionType: ClassType.Farm)
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    /// <summary>
    /// Coordinates /twilightedge Quest 3428 and /weaverwar Quests 3429-3439 to unlock Quest 3445 (Ravenloss REP).
    /// </summary>
    private bool RunCoordinatedWeaverWar(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;

        bool IsWWQuestDone(int questId) =>
            skipCondition() || core.isCompletedBefore(3439, log: false) || core.isCompletedBefore(questId, log: false);

        // 3428: Clear Twilight's Edge
        if (
            !RunCooperativeObjective(
                "BRACKET55_WW_3428",
                () => IsWWQuestDone(3428),
                () =>
                {
                    if (IsWWQuestDone(3428))
                        return;
                    Story.MapItemQuest(3428, "twilightedge", 2577);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        (int QuestId, int MapItemId, string Cell, string Monster, string TempItem, int Quant)[] weaverQuests =
        {
            (3429, 2578, "s1", "Weaver Queen's Hound", "Weaver Hounds Slain", 6),
            (3430, 2579, "f1", "ChaosWeaver Mage", "Spider Slain", 6),
            (3431, 2580, "d1", "Dread Arysa", "Arysa Slain", 6),
            (3432, 2581, "do1", "Weaver Queen's Hound", "Weaver Hound Slain", 6),
            (3433, 2582, "fr1", "ChaosWeaver Mage", "Weaver Slain", 6),
            (3434, 2583, "n1", "Evolved Dreadspider", "Spider Slain", 6),
            (3435, 2584, "b1", "Dread Arysa", "Arysa Slain", 6),
            (3436, 2585, "g1", "ChaosWeaver Mage", "Weaver Slain", 6),
            (3437, 2586, "sl1", "ChaosWeaver Mage", "Weaver Slain", 6),
            (3438, 2587, "ba1", "Evolved Dreadspider", "Spider Slain", 6),
        };

        foreach (var wq in weaverQuests)
        {
            if (
                !RunCooperativeObjective(
                    $"BRACKET55_WW_{wq.QuestId}",
                    () => IsWWQuestDone(wq.QuestId),
                    () =>
                    {
                        if (IsWWQuestDone(wq.QuestId))
                            return;

                        SetClass(ClassType.Farm);
                        core.EnsureAccept(wq.QuestId);
                        core.GetMapItem(wq.MapItemId, 1, "weaverwar");
                        core.KillMonster("weaverwar", wq.Cell, "Spawn", wq.Monster, wq.TempItem, wq.Quant);
                        core.EnsureComplete(wq.QuestId);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                        Army.CoordinateFollowAndCombat(targetPlayerName, wq.Monster, compositionType: ClassType.Farm)
                )
            )
                return false;
        }

        // 3439: Save the SoulWeaver (Boss: ChaosWeaver Warrior, 132,840 HP in /twilightedge)
        if (
            !RunCooperativeObjective(
                "BRACKET55_WW_3439_BOSS",
                () => IsWWQuestDone(3439),
                () =>
                {
                    if (IsWWQuestDone(3439))
                        return;

                    SetClass(ClassType.Solo);
                    Story.KillQuest(3439, "twilightedge", "ChaosWeaver Warrior");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "ChaosWeaver Warrior")
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    /// <summary>
    /// Coordinates the Yokai Storyline (/shogunwar, /shinringrove, /greenshell, /heiwavalley, /shadowfortress)
    /// up to Quest 6494 (Defeat Jaaku!) to unlock Quest 7924 for DragonSoul Shinobi.
    /// Skips optional side-quests 6460, 6461, and 6473, and fixes the Quest 6494 completion bug.
    /// </summary>
    private bool RunCoordinatedYokaiStory(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_YOKAI_STORY",
                () => skipCondition() || core.isCompletedBefore(6494, log: false)
            )
        )
            return true;

        core.EnsureLoad(
            Enumerable.Range(6450, 10)
                .Concat(Enumerable.Range(6460, 13))
                .Concat(Enumerable.Range(6488, 7))
                .ToArray()
        );

        bool IsShogunDone(int questId) =>
            skipCondition()
            || core.isCompletedBefore(6494, log: false)
            || core.isCompletedBefore(6459, log: false)
            || core.isCompletedBefore(questId, log: false);

        bool IsShinrinDone(int questId) =>
            skipCondition()
            || core.isCompletedBefore(6494, log: false)
            || core.isCompletedBefore(6472, log: false)
            || core.isCompletedBefore(questId, log: false);

        bool IsShadowFortressDone(int questId) =>
            skipCondition() || core.isCompletedBefore(6494, log: false) || core.isCompletedBefore(questId, log: false);

        // --- Part 1: ShogunWar (Quests 6450-6459) ---
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6450",
                () => IsShogunDone(6450),
                () =>
                {
                    if (IsShogunDone(6450))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6450);
                    core.HuntMonster("shogunwar", "Shadow Samurai", "Shadow Medallions", 5);
                    core.EnsureComplete(6450);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6451",
                () => IsShogunDone(6451),
                () =>
                {
                    if (IsShogunDone(6451))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6451);
                    core.HuntMonsterMapID("shogunwar", 8, "Kijimuna Pollen", 4);
                    core.HuntMonsterMapID("shogunwar", 7, "Reishi Spores", 4);
                    core.EnsureComplete(6451);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6452",
                () => IsShogunDone(6452),
                () =>
                {
                    if (IsShogunDone(6452))
                        return;
                    Story.MapItemQuest(6452, "shogunwar", 5956, 5);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6453",
                () => IsShogunDone(6453),
                () =>
                {
                    if (IsShogunDone(6453))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6453, "shogunwar", "Shadow Samurai");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6454",
                () => IsShogunDone(6454),
                () =>
                {
                    if (IsShogunDone(6454))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6454, "shogunwar", "Bamboo Treeant");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Bamboo Treeant", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6455",
                () => IsShogunDone(6455),
                () =>
                {
                    if (IsShogunDone(6455))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6455, "shogunwar", "Shadow Samurai");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6456",
                () => IsShogunDone(6456),
                () =>
                {
                    if (IsShogunDone(6456))
                        return;
                    Story.MapItemQuest(6456, "shogunwar", new[] { 5957, 5958, 5959, 5960 });
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6457",
                () => IsShogunDone(6457),
                () =>
                {
                    if (IsShogunDone(6457))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6457, "shogunwar", "Reishi");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Reishi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6458",
                () => IsShogunDone(6458),
                () =>
                {
                    if (IsShogunDone(6458))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6458, "shogunwar", "Bamboo Treeant");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Bamboo Treeant", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6459: Defeat the Beast (Boss: Orochi, MapID 25)
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6459_OROCHI",
                () => IsShogunDone(6459),
                () =>
                {
                    if (IsShogunDone(6459))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6459);
                    core.HuntMonsterMapID("shogunwar", 25, "Orochi Defeated");
                    core.EnsureComplete(6459);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "25")
            )
        )
            return false;

        // --- Part 2: ShinrinGrove & GreenShell (Quests 6462-6472) ---
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6462",
                () => IsShinrinDone(6462),
                () =>
                {
                    if (IsShinrinDone(6462))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6462);
                    core.HuntMonster("shinringrove", "Kame", "Kame Defeated");
                    core.EnsureComplete(6462);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Kame")
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6463",
                () => IsShinrinDone(6463),
                () =>
                {
                    if (IsShinrinDone(6463))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6463);
                    core.HuntMonster("shinringrove", "Reishi", "Reishi Caps", 6);
                    core.HuntMonster("shinringrove", "Tsurubebi", "Tsurubebi Flame", 3);
                    core.EnsureComplete(6463);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6464: Summon Otoroshi (Boss: Otoroshi, 84,920 HP in /shinringrove r4a, Left)
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6464_OTOROSHI",
                () => IsShinrinDone(6464),
                () =>
                {
                    if (IsShinrinDone(6464))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6464);
                    core.GetMapItem(5962, 1, "shinringrove");
                    core.Sleep();
                    core.KillMonster("shinringrove", "r4a", "Left", "Otoroshi", "Otoroshi Defeated");
                    core.EnsureComplete(6464);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Otoroshi")
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6465",
                () => IsShinrinDone(6465),
                () =>
                {
                    if (IsShinrinDone(6465))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6465);
                    core.GetMapItem(5964, 5, "greenshell");
                    core.HuntMonster("greenshell", "Tsurubebi", "Tsurubebi Extinguished", 5);
                    core.EnsureComplete(6465);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Tsurubebi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6466",
                () => IsShinrinDone(6466),
                () =>
                {
                    if (IsShinrinDone(6466))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6466, "greenshell", "Tsukumogami");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Tsukumogami", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6467",
                () => IsShinrinDone(6467),
                () =>
                {
                    if (IsShinrinDone(6467))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6467, "shinringrove", "Kodama");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Kodama", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6468",
                () => IsShinrinDone(6468),
                () =>
                {
                    if (IsShinrinDone(6468))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6468, "shinringrove", "Moglinberry Bush");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Moglinberry Bush", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6469",
                () => IsShinrinDone(6469),
                () =>
                {
                    if (IsShinrinDone(6469))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6469);
                    core.GetMapItem(5965, 8, "greenshell");
                    core.HuntMonster("shinringrove", "Reishi", "Luminous Spores", 6);
                    core.EnsureComplete(6469);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Reishi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6470",
                () => IsShinrinDone(6470),
                () =>
                {
                    if (IsShinrinDone(6470))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6470);
                    core.GetMapItem(5966, 1, "greenshell");
                    core.HuntMonster("greenshell", "Merdraconian", "Merdraconians Slain", 3);
                    core.EnsureComplete(6470);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Merdraconian", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6471",
                () => IsShinrinDone(6471),
                () =>
                {
                    if (IsShinrinDone(6471))
                        return;
                    Story.MapItemQuest(6471, "greenshell", 5967);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6472: Battle for the Shinrin Do (Boss: Nagami, 129,050 HP in /greenshell)
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6472_NAGAMI",
                () => IsShinrinDone(6472),
                () =>
                {
                    if (IsShinrinDone(6472))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6472);
                    core.HuntMonster("greenshell", "Nagami", "Nagami Defeated");
                    core.EnsureComplete(6472);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Nagami")
            )
        )
            return false;

        // --- Part 3: HeiwaValley & ShadowFortress (Quests 6474-6483 & 6485-6494) ---
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6474",
                () => IsShadowFortressDone(6474),
                () =>
                {
                    if (IsShadowFortressDone(6474))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6474, "heiwavalley", "Abumi Guchi");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Abumi Guchi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6475",
                () => IsShadowFortressDone(6475),
                () =>
                {
                    if (IsShadowFortressDone(6475))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6475);
                    core.HuntMonster("heiwavalley", "Shadow Samurai", "Pilfered Armor", 5);
                    core.HuntMonster("heiwavalley", "Shadow Samurai", "Stolen Sword", 5);
                    core.EnsureComplete(6475);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6476",
                () => IsShadowFortressDone(6476),
                () =>
                {
                    if (IsShadowFortressDone(6476))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6476);
                    core.GetMapItem(5968, 6, "heiwavalley");
                    core.HuntMonster("heiwavalley", "Kosenjobi", "Kosenjobi Flame", 6);
                    core.EnsureComplete(6476);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Kosenjobi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6477",
                () => IsShadowFortressDone(6477),
                () =>
                {
                    if (IsShadowFortressDone(6477))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6477, "heiwavalley", "Goryo");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Goryo", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6478",
                () => IsShadowFortressDone(6478),
                () =>
                {
                    if (IsShadowFortressDone(6478))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6478);
                    core.HuntMonster("heiwavalley", "Kosenjobi", "Spectral Heat", 5);
                    Story.BuyQuest(6478, "heiwavalley", 1608, "Wasabi");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Kosenjobi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6479",
                () => IsShadowFortressDone(6479),
                () =>
                {
                    if (IsShadowFortressDone(6479))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6479, "heiwavalley", "Abumi Guchi");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Abumi Guchi", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6480",
                () => IsShadowFortressDone(6480),
                () =>
                {
                    if (IsShadowFortressDone(6480))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6480, "heiwavalley", "Inugami");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Inugami", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6481",
                () => IsShadowFortressDone(6481),
                () =>
                {
                    if (IsShadowFortressDone(6481))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6481);
                    core.HuntMonster("heiwavalley", "Inugami", "Spirit Eyes", 4);
                    Story.MapItemQuest(6481, "heiwavalley", 5970, 8);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Inugami", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6482",
                () => IsShadowFortressDone(6482),
                () =>
                {
                    if (IsShadowFortressDone(6482))
                        return;
                    Story.MapItemQuest(6482, "heiwavalley", 5971);
                    Story.MapItemQuest(6482, "heiwavalley", 5972, 8);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6483: Defeat the Onryo (Boss: Onryo, MapID 15 in /heiwavalley)
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6483_ONRYO",
                () => IsShadowFortressDone(6483),
                () =>
                {
                    if (IsShadowFortressDone(6483))
                        return;
                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6483);
                    core.HuntMonsterMapID("heiwavalley", 15, "Onryo Slain");
                    core.EnsureComplete(6483);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "15")
            )
        )
            return false;

        // 6485: Calm the Spirits
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6485",
                () => IsShadowFortressDone(6485),
                () =>
                {
                    if (IsShadowFortressDone(6485))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6485, "shadowfortress", "Restless Spirit");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Restless Spirit", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6486: Disarm the Samurai
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6486",
                () => IsShadowFortressDone(6486),
                () =>
                {
                    if (IsShadowFortressDone(6486))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6486, "shadowfortress", "Shadow Samurai");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6487: Find the Throne Room
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6487",
                () => IsShadowFortressDone(6487),
                () =>
                {
                    if (IsShadowFortressDone(6487))
                        return;
                    Story.MapItemQuest(6487, "shadowfortress", 5973);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6488: Shadow Essences
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6488",
                () => IsShadowFortressDone(6488),
                () =>
                {
                    if (IsShadowFortressDone(6488))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6488);
                    core.GetMapItem(5974, 1, "shadowfortress");
                    core.HuntMonster("shadowfortress", "Living Shadow", "Shadow Essence", 7);
                    core.EnsureComplete(6488);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Living Shadow", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6489: New Recruits
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6489",
                () => IsShadowFortressDone(6489),
                () =>
                {
                    if (IsShadowFortressDone(6489))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(6489, "shadowfortress", "Shadow Samurai");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Shadow Samurai", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6490: Shadow Heads
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6490",
                () => IsShadowFortressDone(6490),
                () =>
                {
                    if (IsShadowFortressDone(6490))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(6490);
                    core.GetMapItem(5975, 7, "shadowfortress");
                    core.HuntMonster("shadowfortress", "Creeping Shadow", "Shadows Slain", 7);
                    core.EnsureComplete(6490);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Creeping Shadow", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 6491: A Head has Woken (Boss: 7th Head of Orochi)
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6491",
                () => IsShadowFortressDone(6491),
                () =>
                {
                    if (IsShadowFortressDone(6491))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(6491, "shadowfortress", "7th Head of Orochi");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "7th Head of Orochi")
            )
        )
            return false;

        // 6492: Get to the Roof
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6492",
                () => IsShadowFortressDone(6492),
                () =>
                {
                    if (IsShadowFortressDone(6492))
                        return;
                    Story.MapItemQuest(6492, "shadowfortress", new[] { 5976, 5979 });
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6493: Find Jaaku! (Coordinate each of the 6 Heads of Orochi + MapItem 5977)
        (string Label, string Monster, string TempDrop)[] orochiHeads =
        {
            ("6TH", "6th Head of Orochi", "6th Head Defeated"),
            ("5TH", "5th Head of Orochi", "5th Head Defeated"),
            ("4TH", "4th Head of Orochi", "4th Head Defeated"),
            ("3RD", "3rd Head of Orochi", "3rd Head Defeated"),
            ("2ND", "2nd Head of Orochi", "2nd Head Defeated"),
            ("1ST", "1st Head of Orochi", "1st Head Defeated"),
        };

        foreach (var head in orochiHeads)
        {
            if (
                !RunCooperativeObjective(
                    $"BRACKET60_YOKAI_6493_HEAD_{head.Label}",
                    () => IsShadowFortressDone(6493) || Bot.TempInv.Contains(head.TempDrop),
                    () =>
                    {
                        if (IsShadowFortressDone(6493) || Bot.TempInv.Contains(head.TempDrop))
                            return;

                        SetClass(ClassType.Solo);
                        core.EnsureAccept(6493);
                        core.HuntMonster("shadowfortress", head.Monster, head.TempDrop);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                        Army.CoordinateFollowAndCombat(targetPlayerName, head.Monster)
                )
            )
                return false;
        }

        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6493_COMPLETE",
                () => IsShadowFortressDone(6493),
                () =>
                {
                    if (IsShadowFortressDone(6493))
                        return;

                    core.EnsureAccept(6493);
                    Story.MapItemQuest(6493, "shadowfortress", 5977);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 6494: Defeat Jaaku! (Boss: Jaaku, MapID 31, 175,880 HP — fixes EnsureComplete(6494))
        if (
            !RunCooperativeObjective(
                "BRACKET60_YOKAI_6494_JAAKU",
                () => IsShadowFortressDone(6494),
                () =>
                {
                    if (IsShadowFortressDone(6494))
                        return;

                    SetClass(ClassType.Solo);
                    core.EnsureAccept(6494);
                    core.HuntMonsterMapID("shadowfortress", 31, "Jaaku Defeated");
                    core.EnsureComplete(6494);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "31")
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    /// <summary>
    /// Coordinates /frozenruins, /glacera, /icewindpass, and /icewindwar Quests (3941-3954 & 5587-5601)
    /// to unlock Glacera REP farming (Quests 5597-5600) for Glacial Berserker.
    /// Skips Northstar (3958-3971), optional quests 5595-5596, and IceDungeon (7832-7841).
    /// </summary>
    private bool RunCoordinatedGlaceraForGB(Func<bool> skipCondition)
    {
        CoreBots core = CoreBots.Instance;
        if (
            Army.AllPlayersSatisfy(
                "SKIP_GLACERA_STORY",
                () => skipCondition() || core.isCompletedBefore(5601, log: false)
            )
        )
            return true;

        core.EnsureLoad(
            new[]
            {
                3941, 3942, 3943, 3944, 3945, 3946, 3947, 3948, 3949, 3950, 3951, 3952, 3953, 3954,
                5587, 5588, 5589, 5590, 5591, 5592, 5593, 5594, 5595, 5596, 5597, 5598, 5599, 5600, 5601,
            }
        );

        bool IsRuinsQuestDone(int questId) =>
            skipCondition() || core.isCompletedBefore(questId, log: false);

        bool IsIceWindQuestDone(int questId) =>
            skipCondition() || core.isCompletedBefore(questId, log: false);

        // --- Part 1: FrozenRuins & Glacera (Quests 3941-3954) ---
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3941",
                () => IsRuinsQuestDone(3941),
                () =>
                {
                    if (IsRuinsQuestDone(3941))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3941, "frozenruins", "Frost Fangbeast");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Fangbeast", compositionType: ClassType.Farm)
            )
        )
            return false;

        for (int qId = 3942; qId <= 3944; qId++)
        {
            int currentQuest = qId;
            if (
                !RunCooperativeObjective(
                    $"BRACKET65_GLACERA_{currentQuest}",
                    () => IsRuinsQuestDone(currentQuest),
                    () =>
                    {
                        if (IsRuinsQuestDone(currentQuest))
                            return;
                        SetClass(ClassType.Farm);
                        Story.KillQuest(currentQuest, "frozenruins", "Frost Reaper");
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                        Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Reaper", compositionType: ClassType.Farm)
                )
            )
                return false;
        }

        // 3945: Form the Lock
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3945",
                () => IsRuinsQuestDone(3945),
                () =>
                {
                    if (IsRuinsQuestDone(3945))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3945);
                    core.GetMapItem(3050, 10, "frozenruins");
                    core.HuntMonster("frozenruins", "Frozen Moglinster", "Frostbreath Weed", 5);
                    core.EnsureComplete(3945);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frozen Moglinster", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3946: Glacera
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3946",
                () => IsRuinsQuestDone(3946),
                () =>
                {
                    if (IsRuinsQuestDone(3946))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3946);
                    if (!core.CheckInventory(27357))
                        core.HuntMonster("frozenruins", "Frost Reaper", "Mercury");
                    core.EnsureComplete(3946);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Reaper", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3947: A Frosty Welcome
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3947",
                () => IsRuinsQuestDone(3947),
                () =>
                {
                    if (IsRuinsQuestDone(3947))
                        return;
                    core.EnsureAccept(3947);
                    core.Join("glacera");
                    Bot.Wait.ForMapLoad("glacera");
                    core.GetMapItem(3048, 1, "glacera");
                    core.EnsureComplete(3947);
                    core.Jump("Enter", "Spawn");
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3948: Key to the Fortress
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3948",
                () => IsRuinsQuestDone(3948),
                () =>
                {
                    if (IsRuinsQuestDone(3948))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3948, "glacera", "Frost Invader");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Frost Invader",
                        compositionType: ClassType.Farm,
                        targetMap: "glacera"
                    )
            )
        )
            return false;

        // 3949: Breaking Boulders
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3949",
                () => IsRuinsQuestDone(3949),
                () =>
                {
                    if (IsRuinsQuestDone(3949))
                        return;
                    Story.MapItemQuest(3949, "glacera", 3049, 6);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3950: The Scythe of Vengeance
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3950",
                () => IsRuinsQuestDone(3950),
                () =>
                {
                    if (IsRuinsQuestDone(3950))
                        return;
                    Story.MapItemQuest(3950, "glacera", 3047);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // 3951: Rescue the Refugees
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3951",
                () => IsRuinsQuestDone(3951),
                () =>
                {
                    if (IsRuinsQuestDone(3951))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(3951);
                    core.HuntMonster("frozenruins", "Frost Invader", "Ice Beast Destroyed", 10);
                    core.EnsureComplete(3951);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Invader", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3952: Defeat the Fangbeasts
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3952",
                () => IsRuinsQuestDone(3952),
                () =>
                {
                    if (IsRuinsQuestDone(3952))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3952, "frozenruins", "Frost Fangbeast");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Fangbeast", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3953: Destroy the Frost Reapers
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3953",
                () => IsRuinsQuestDone(3953),
                () =>
                {
                    if (IsRuinsQuestDone(3953))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(3953, "frozenruins", "Frost Reaper");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Reaper", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 3954: FrostSpawn General Takedown (Boss: Frost General, 65,620 HP)
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_3954_BOSS",
                () => IsRuinsQuestDone(3954),
                () =>
                {
                    if (IsRuinsQuestDone(3954))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(3954, "frozenruins", "Frost General");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost General")
            )
        )
            return false;

        // --- Part 2: IceWindPass & IceWindWar (Quests 5587-5594, 5597-5601; room cap 6 -> subgroup 4) ---
        ApplySubgroupRoomSettings(4);
        try
        {
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5587",
                () => IsIceWindQuestDone(5587),
                () =>
                {
                    if (IsIceWindQuestDone(5587))
                        return;
                    Story.MapItemQuest(5587, "icewindpass", 5074, 5);
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5588",
                () => IsIceWindQuestDone(5588),
                () =>
                {
                    if (IsIceWindQuestDone(5588))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(5588, "icewindpass", "Glacial Elemental");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Glacial Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5589",
                () => IsIceWindQuestDone(5589),
                () =>
                {
                    if (IsIceWindQuestDone(5589))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5589);
                    core.GetMapItem(5075, 5, "icewindpass");
                    core.HuntMonster("icewindpass", "Glacial Elemental", "Iced Mana", 9);
                    core.EnsureComplete(5589);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Glacial Elemental", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5590",
                () => IsIceWindQuestDone(5590),
                () =>
                {
                    if (IsIceWindQuestDone(5590))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(5590, "icewindpass", "Polar Golem");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Polar Golem", compositionType: ClassType.Farm)
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5591",
                () => IsIceWindQuestDone(5591),
                () =>
                {
                    if (IsIceWindQuestDone(5591))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(5591, "icewindpass", "Frost Invader");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Invader", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 5592: Blast the Frostspawn Symbiote (Boss: Frostspawn Symbiote)
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5592_SYMBIOTE",
                () => IsIceWindQuestDone(5592),
                () =>
                {
                    if (IsIceWindQuestDone(5592))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(5592, "icewindpass", "Frostspawn Symbiote");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frostspawn Symbiote")
            )
        )
            return false;

        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5593",
                () => IsIceWindQuestDone(5593),
                () =>
                {
                    if (IsIceWindQuestDone(5593))
                        return;
                    SetClass(ClassType.Farm);
                    Story.KillQuest(5593, "icewindpass", "Frost Invader");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frost Invader", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 5594: Take it Down (Boss: Frostspawn Horror, 153,590 HP)
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5594_HORROR",
                () => IsIceWindQuestDone(5594),
                () =>
                {
                    if (IsIceWindQuestDone(5594))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(5594, "icewindpass", "Frostspawn Horror");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Frostspawn Horror")
            )
        )
            return false;

        // 5595: Keep the Frostspawn Away!
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5595",
                () => IsIceWindQuestDone(5595),
                () =>
                {
                    if (IsIceWindQuestDone(5595))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5595);
                    core.HuntMonster("icewindpass", "Frost Invader", "Frost Invader Slain", 5);
                    core.HuntMonster("icewindpass", "Frostspawn Troll", "Frostspawn Troll Slain", 5);
                    core.EnsureComplete(5595);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 5596: Take a Break from Fighting
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5596",
                () => IsIceWindQuestDone(5596),
                () =>
                {
                    if (IsIceWindQuestDone(5596))
                        return;
                    SetClass(ClassType.Farm);
                    core.EnsureAccept(5596);
                    core.HuntMonster("icewindpass", "Polar Golem", "Living Snow", 3);
                    core.HuntMonster("icewindpass", "Glacial Elemental", "Elemental Icicles", 3);
                    core.EnsureComplete(5596);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 5597, 5598, 5599, 5600: IceWindWar Medals (Slot 225, Value 9)
        bool warMedalsDone = false;
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5597_5600_WAR",
                () => IsIceWindQuestDone(5597) || warMedalsDone,
                () =>
                {
                    if (IsIceWindQuestDone(5597) || warMedalsDone)
                        return;

                    SetClass(ClassType.Farm);
                    core.EnsureAcceptmultiple(new[] { 5597, 5598, 5599, 5600 });
                    core.HuntMonster("icewindwar", "Glaceran Defender", "FrostSpawn Medal", 10);
                    core.HuntMonster("icewindwar", "Glaceran Defender", "Mega Frostspawn Medal", 5);
                    core.HuntMonster("icewindwar", "Frost Invader", "World Ender Medal", 10);
                    core.HuntMonster("icewindwar", "Frost Invader", "Mega World Ender Medal", 5);
                    core.EnsureComplete(new[] { 5597, 5598, 5599, 5600 });
                    warMedalsDone = true;
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "*", compositionType: ClassType.Farm)
            )
        )
            return false;

        // 5601: What is THAT? (Boss: Soricomorpha in /icewindwar, Slot 225, Value 10)
        if (
            !RunCooperativeObjective(
                "BRACKET65_GLACERA_5601_SORICOMORPHA",
                () => IsIceWindQuestDone(5601),
                () =>
                {
                    if (IsIceWindQuestDone(5601))
                        return;
                    SetClass(ClassType.Solo);
                    Story.KillQuest(5601, "icewindwar", "Soricomorpha");
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Soricomorpha")
            )
        )
            return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        return !Bot.ShouldExit;
    }
    #endregion Step 2C: Brackets 55, 60, and 65

    #region SetClass (Party Role Composition & CBO Resolution — 100% Free-to-Play Classes Only)
    // Required Players 1-4 form the Endgame Meta Core (Main DPS + StoneCrusher + ArchPaladin + Lord of Order).
    // Optional Players 5-7 add non-overlapping debuffs, amplifiers, dodge DPS, and backup support.

    // Player 1 (Required): Main Solo DPS (never overlaps with Players 2-7 at the same milestone)
    private static readonly string[] Player1SoloClasses =
    {
        "King's Echo",
        "Arcana Invoker",
        "Chaos Avenger",
        "Void Highlord",
        "Verus DoomKnight",
        "Dragon of Time",
        "Frost SpiritReaver",
        "Cryomancer",
        "Dragonslayer General",
        "Glacial Berserker",
        "Dragonslayer",
        "DragonSoul Shinobi",
        "Ninja",
        "Rogue",
    };

    // Player 2 (Required): Primary Support / Haste & HoT Buffer -> Endgame preference: StoneCrusher
    private static readonly string[] Player2SupportClasses =
    {
        "StoneCrusher",
        "Bard",
        "Oracle",
        "Healer (Rare)",
        "Healer",
    };

    // Player 3 (Required): Hit-Chance / Damage Debuffer & Tank -> Endgame preference: ArchPaladin
    private static readonly string[] Player3DebuffClasses =
    {
        "ArchPaladin",
        "Imperial Chunin",
        "Rogue (Rare)",
        "Rogue",
        "Ninja",
        "Oracle",
    };

    // Player 4 (Required): Damage Amplifier & Stat Buffer -> Endgame preference: Lord of Order
    private static readonly string[] Player4AmpClasses =
    {
        "Lord of Order",
        "Arachnomancer",
        "Shaman",
        "Mage (Rare)",
        "Mage",
        "Oracle",
    };

    // Player 5 (Optional): Defense Shred / Decay Debuffer & Sub-DPS (Evolved Shaman -> ShadowScythe General -> Glacial Berserker -> Kathool Adept -> Verus DoomKnight)
    private static readonly string[] Player5FlexClasses =
    {
        "Verus DoomKnight",
        "Kathool Adept",
        "Glacial Berserker",
        "ShadowScythe General",
        "Evolved Shaman",
        "Master Ranger",
        "Oracle",
        "Healer (Rare)",
        "Healer",
    };

    // Player 6 (Optional): Crit / Damage Amplifier & Flex DPS (Master Ranger -> Scarlet Sorceress -> Northlands Monk -> Archfiend -> Scion of Flames)
    private static readonly string[] Player6FlexClasses =
    {
        "Scion of Flames",
        "Archfiend",
        "Northlands Monk",
        "Scarlet Sorceress",
        "Master Ranger",
        "Oracle",
        "Healer (Rare)",
        "Healer",
    };

    // Player 7 (Optional): Stealth / Dodge DPS & Execute (Ninja -> ShadowScythe General -> DragonSoul Shinobi -> Yami No Ronin)
    private static readonly string[] Player7FlexClasses =
    {
        "Yami No Ronin",
        "DragonSoul Shinobi",
        "ShadowScythe General",
        "Ninja",
        "Rogue",
        "Oracle",
        "Healer (Rare)",
        "Healer",
    };

    // Secondary Support pool for Player 5 during Farm mode in larger armies (5-7 players)
    private static readonly string[] Player5FarmSupportClasses =
    {
        "Lord of Order",
        "StoneCrusher",
        "ArchPaladin",
        "Bard",
        "Oracle",
        "Evolved Shaman",
        "Healer (Rare)",
        "Healer",
    };

    private static readonly string[] FarmClasses =
    {
        "ArchMage",
        "Kathool Adept",
        "Arcana Invoker",
        "Dragon of Time",
        "Archfiend",
        "Blaze Binder",
        "ShadowScythe General",
        "Scarlet Sorceress",
        "Pyromancer",
        "Evolved Shaman",
        "Shaman",
        "Imperial Chunin",
        "Master Ranger",
        "Mage",
    };

    private string[] GetRoleBossPool()
    {
        if (IsPlayer(2))
            return Player2SupportClasses;
        if (IsPlayer(3))
            return Player3DebuffClasses;
        if (IsPlayer(4))
            return Player4AmpClasses;
        if (IsPlayer(5))
            return Player5FlexClasses;
        if (IsPlayer(6))
            return Player6FlexClasses;
        if (IsPlayer(7))
            return Player7FlexClasses;

        return Player1SoloClasses;
    }

    private string[] GetRoleFarmPool()
    {
        // Farm Composition: AoE Farm classes combined with Support/Healer classes
        // - Player 2 stays on Primary Healer/Support (4-7 player armies)
        // - Player 5 adds Secondary Buffer/Support in larger armies (5-7 player armies)
        if (IsPlayer(2))
            return Player2SupportClasses;
        if (IsPlayer(5))
            return Player5FarmSupportClasses;

        return FarmClasses;
    }

    private string GetPartyRoleLabel()
    {
        if (IsPlayer(2))
            return "P2-Support(SC)";
        if (IsPlayer(3))
            return "P3-Debuff(AP)";
        if (IsPlayer(4))
            return "P4-BuffAmp(LoO)";
        if (IsPlayer(5))
            return "P5-FlexDebuff";
        if (IsPlayer(6))
            return "P6-FlexAmpDPS";
        if (IsPlayer(7))
            return "P7-FlexDodgeDPS";
        return "P1-MainDPS";
    }

    private string? _lastSolo;
    private string? _lastFarm;
    private int _setClassBusy;
    private bool _cboInitialized;
    private ClassType _syncedClassType = ClassType.None;

    public void SetClass(bool rankUpIfNeeded = false) =>
        SetClass(ClassType.Solo, rankUpIfNeeded);

    public void SetClass(ClassType targetType, bool rankUpIfNeeded = false)
    {
        if (System.Threading.Interlocked.Exchange(ref _setClassBusy, 1) == 1)
        {
            CoreBots.Instance.Logger("SetClass is already running, skipping the overlapping call.");
            return;
        }

        try
        {
            SetClassInner(targetType, rankUpIfNeeded);
        }
        finally
        {
            System.Threading.Volatile.Write(ref _setClassBusy, 0);
        }
    }

    private string ResolveDesiredClass(CoreBots core, ClassType targetType, out ClassType effectiveType)
    {
        if (
            targetType == ClassType.Farm
            && !CoreBots.IsGenericClass(core.FarmClass)
            && !string.IsNullOrWhiteSpace(core.FarmClass)
            && HasClassInInventory(core.FarmClass)
        )
        {
            effectiveType = ClassType.Farm;
            return core.FarmClass;
        }

        effectiveType = targetType == ClassType.Farm
            && !string.Equals(core.FarmClass, core.SoloClass, StringComparison.OrdinalIgnoreCase)
            ? ClassType.Solo
            : targetType;
        return core.SoloClass;
    }

    private void SetClassInner(ClassType targetType, bool rankUpIfNeeded)
    {
        CoreBots core = CoreBots.Instance;
        if (!_cboInitialized || rankUpIfNeeded)
        {
            core.ReadCBO();
            _cboInitialized = true;
        }
        ApplyCurrentArmyRoomSettings();

        // Always use the standard Base mode from Skua's Advanced Skills Set
        core.SoloUseMode = ClassUseMode.Base;
        core.FarmUseMode = ClassUseMode.Base;
        core.DodgeUseMode = ClassUseMode.Base;
        core.BossUseMode = ClassUseMode.Base;

        string[] bossPool = GetRoleBossPool();
        string[] farmPool = GetRoleFarmPool();

        // Two unified compositions:
        // 1. Boss Composition (used for SoloClass, DodgeClass, and BossClass)
        // 2. Farm Composition (AoE farmers + dedicated Supports on P2 and P5)
        string resolvedSolo = ResolveClass(bossPool);
        string resolvedFarm = ResolveClass(farmPool);

        core.SoloClass = resolvedSolo;
        core.DodgeClass = resolvedSolo;
        core.BossClass = resolvedSolo;
        core.FarmClass = resolvedFarm;

        if (
            !string.Equals(_lastSolo, resolvedSolo, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(_lastFarm, resolvedFarm, StringComparison.OrdinalIgnoreCase)
        )
        {
            _syncedClassType = ClassType.None;
        }

        bool Settled(string? className) =>
            CoreBots.IsGenericClass(className)
            || (!string.IsNullOrWhiteSpace(className) && HasRank10Class(className));

        if (
            rankUpIfNeeded
            && (
                !string.Equals(_lastSolo, resolvedSolo, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(_lastFarm, resolvedFarm, StringComparison.OrdinalIgnoreCase)
                || !Settled(resolvedSolo)
                || !Settled(resolvedFarm)
            )
        )
        {
            RankIfNeeded(resolvedSolo, "BossComposition");
            RankIfNeeded(resolvedFarm, "FarmComposition");
            Bot.Sleep(500);
            _lastSolo = resolvedSolo;
            _lastFarm = resolvedFarm;
        }
        else if (string.IsNullOrEmpty(_lastSolo) && string.IsNullOrEmpty(_lastFarm))
        {
            _lastSolo = resolvedSolo;
            _lastFarm = resolvedFarm;
        }

        string desiredClass = ResolveDesiredClass(core, targetType, out ClassType effectiveType);
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        bool alreadyWearingDesired =
            !CoreBots.IsGenericClass(desiredClass)
            && string.Equals(currentClass, desiredClass, StringComparison.OrdinalIgnoreCase);

        if (alreadyWearingDesired)
        {
            Army.EnsureSkillRotationActive();
            _syncedClassType = effectiveType;
            return;
        }

        if (CoreBots.IsGenericClass(desiredClass))
        {
            if (_syncedClassType != effectiveType)
            {
                LeaveCombatRoomForEquipmentChange();
                core.EquipClass(effectiveType);
                Army.StartClassSkills(Bot.Player.CurrentClass?.Name);
                _syncedClassType = effectiveType;
            }
            else
            {
                Army.EnsureSkillRotationActive();
            }
            return;
        }

        if (HasClassInInventory(desiredClass))
        {
            LeaveCombatRoomForEquipmentChange();
            EnsureCombatEnhancementReady(desiredClass, allowFullGearEnhance: false);
            core.EquipClass(effectiveType);
            Army.StartClassSkills(Bot.Player.CurrentClass?.Name ?? desiredClass);
            _syncedClassType = effectiveType;
        }
        else if (!Bot.Skills.TimerRunning && !string.IsNullOrWhiteSpace(currentClass))
        {
            Army.StartClassSkills(currentClass);
        }
    }

    private bool IsDisableAutoEnhanceEnabled() =>
        CoreBots.Instance.CBOBool("DisableAutoEnhance", out bool disabled) && disabled;

    private bool HasEquippedItemBelowCurrentCap()
    {
        if (ShouldPreserveEquippedGear() || IsDisableAutoEnhanceEnabled() || Bot.Inventory?.Items == null)
            return false;

        int cap = Math.Min(Bot.Player.Level, 100);
        return Bot.Inventory.Items.Any(x =>
            x != null
            && x.Equipped
            && (
                x.Category == ItemCategory.Class
                || string.Equals(x.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase)
                || x.Category == ItemCategory.Helm
                || x.Category == ItemCategory.Cape
            )
            && x.EnhancementLevel < cap
        );
    }

    private readonly HashSet<string> _enhancedAtCapCache = new(StringComparer.OrdinalIgnoreCase);
    private string? _lastSmartEnhancedClass;
    private string? _lastSmartEnhancedGearSignature;

    private string GetEquippedCombatGearSignature()
    {
        if (Bot.Inventory?.Items == null)
            return string.Empty;

        string weapon =
            Bot.Inventory.Items.FirstOrDefault(x =>
                x != null && x.Equipped && string.Equals(x.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase)
            )?.Name
            ?? string.Empty;
        string helm =
            Bot.Inventory.Items.FirstOrDefault(x => x != null && x.Equipped && x.Category == ItemCategory.Helm)?.Name
            ?? string.Empty;
        string cape =
            Bot.Inventory.Items.FirstOrDefault(x => x != null && x.Equipped && x.Category == ItemCategory.Cape)?.Name
            ?? string.Empty;

        return $"{weapon}|{helm}|{cape}";
    }

    private bool NeedsOnDemandEnhancement(string className, int minLevelGap = 10)
    {
        if (Bot.Inventory?.Items == null)
            return false;

        string normalizedClass = className.Trim();
        int cap = Math.Min(Bot.Player.Level, 100);

        InventoryItem? classItem = Bot.Inventory.Items.FirstOrDefault(i =>
            i != null
            && i.Category == ItemCategory.Class
            && i.Name?.Trim().Equals(normalizedClass, StringComparison.OrdinalIgnoreCase) == true
        );
        if (classItem != null && classItem.EnhancementLevel <= 0)
            return true;

        bool hasUnenhancedEquipped = Bot.Inventory.Items.Any(x =>
            x != null
            && x.Equipped
            && (
                string.Equals(x.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase)
                || x.Category == ItemCategory.Helm
                || x.Category == ItemCategory.Cape
            )
            && x.EnhancementLevel <= 0
        );
        if (hasUnenhancedEquipped)
            return true;

        // Every class has its own predetermined enhancement kit (Lucky/Wizard/Healer/Forge/Awe).
        // Whenever the combat class or equipped gear changes, re-apply SmartEnhance for that class.
        if (!string.Equals(_lastSmartEnhancedClass, normalizedClass, StringComparison.OrdinalIgnoreCase))
            return true;

        string currentGearSignature = GetEquippedCombatGearSignature();
        if (!string.Equals(_lastSmartEnhancedGearSignature, currentGearSignature, StringComparison.OrdinalIgnoreCase))
            return true;

        if (_enhancedAtCapCache.Contains($"{normalizedClass}:{cap}"))
            return false;

        if (classItem != null && cap - classItem.EnhancementLevel >= minLevelGap)
            return true;

        return Bot.Inventory.Items.Any(x =>
            x != null
            && x.Equipped
            && (
                string.Equals(x.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase)
                || x.Category == ItemCategory.Helm
                || x.Category == ItemCategory.Cape
            )
            && cap - x.EnhancementLevel >= minLevelGap
        );
    }

    private void EnhanceClassForArmyRespectingGearPolicy(string className, bool allowFullGearEnhance) =>
        EnsureCombatEnhancementReady(className, allowFullGearEnhance);

    private void ApplyAndRecordSmartEnhance(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return;

        string normalizedClass = className.Trim();
        int cap = Math.Min(Bot.Player.Level, 100);
        Adv.SmartEnhance(normalizedClass);
        _lastSmartEnhancedClass = normalizedClass;
        _lastSmartEnhancedGearSignature = GetEquippedCombatGearSignature();
        _enhancedAtCapCache.Add($"{normalizedClass}:{cap}");
    }

    private void EnsureCombatEnhancementReady(string className, bool allowFullGearEnhance, int minLevelGap = 10)
    {
        if (string.IsNullOrWhiteSpace(className))
            return;

        InventoryItem? classItem = Bot.Inventory?.Items.FirstOrDefault(i =>
            i != null
            && i.Category == ItemCategory.Class
            && i.Name?.Trim().Equals(className.Trim(), StringComparison.OrdinalIgnoreCase) == true
        );

        // Unenhanced Lv 0 classes (e.g. freshly dropped Blood Sorceress / Scarlet Sorceress) cannot be equipped in AQW
        // until enhanced at least once. Enhance only the class item when DisableAutoEnhance or PreserveEquippedGear is active.
        if (classItem != null && classItem.EnhancementLevel <= 0)
        {
            LeaveCombatRoomForEquipmentChange();
            Adv.EnhanceItem(
                classItem.Name ?? className,
                EnhancementType.Lucky,
                CapeSpecial.None,
                HelmSpecial.None,
                WeaponSpecial.None,
                logging: false,
                bypassDisableAutoEnhance: true
            );
            if (!allowFullGearEnhance || ShouldPreserveEquippedGear() || IsDisableAutoEnhanceEnabled())
                return;
        }

        if (!allowFullGearEnhance || ShouldPreserveEquippedGear() || IsDisableAutoEnhanceEnabled())
            return;

        if (!NeedsOnDemandEnhancement(className, minLevelGap))
            return;

        LeaveCombatRoomForEquipmentChange();
        ApplyAndRecordSmartEnhance(className);
    }

    private void PrepareCombatLoadoutForArmy(ClassType role, string? targetMap = null)
    {
        string? targetRace = role == ClassType.Boss ? DetectCurrentCellMonsterRace() : null;
        EnsureBestStaticBoostGearEquipped(role == ClassType.Boss ? BoostObjective.Combat : BoostObjective.Balanced, targetRace);
        SetClass(role);
        string? activeClass = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(activeClass))
        {
            EnsureCombatEnhancementReady(activeClass, allowFullGearEnhance: true);
            Army.EnsureSkillRotationActive();
        }

        if (!string.IsNullOrWhiteSpace(targetMap))
            Army.EnsureMapAccessBypass(targetMap);
    }

    private bool RunBracketLevelAndEnhance(string objectiveName, int targetLevel)
    {
        string targetExpMap = targetLevel switch
        {
            <= 10 => "oaklore",
            <= 20 => "swordhavenundead",
            <= 60 => "icestormarena",
            < 75 => "battlegrounde",
            _ => "icestormunder",
        };
        int subgroupCap = GetSubgroupCapForMap(targetExpMap);

        // On-demand enhancement policy: only level up here; enhancements are applied lazily
        // when the next rank-up or combat Ready Check actually requires the equipped gear.
        return RunSubgroupCooperativeObjective(
            subgroupCap,
            objectiveName,
            () => Bot.Player.Level >= targetLevel,
            () =>
            {
                if (Bot.Player.Level < targetLevel)
                {
                    EnsureBestStaticBoostGearEquipped(BoostObjective.Exp);
                    SetClass(ClassType.Farm);
                    Farm.Experience(targetLevel);
                    EnsureBestStaticBoostGearEquipped(BoostObjective.Balanced);
                }
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(
                    targetPlayerName,
                    "*",
                    compositionType: ClassType.Farm,
                    targetMap: targetExpMap
                );
            }
        );
    }

    private void RankUpClassForArmy(string className)
    {
        if (string.IsNullOrWhiteSpace(className) || Bot.ShouldExit)
            return;

        if (!HasClassInInventory(className))
        {
            CoreBots.Instance.Logger($"WARNING: Cannot rank up class '{className}' because it is not in inventory.");
            return;
        }

        bool preserveGear = ShouldPreserveEquippedGear();
        if (!preserveGear && !HasRank10Class(className))
            EnsureBestStaticBoostGearEquipped(BoostObjective.ClassPoints);

        // If the class is not yet equipped, step out to /whitemap first so SmartEnhance and Equip
        // cannot be blocked by monster aggro, kill-credit, or party buffs in the room.
        if (!string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase))
        {
            LeaveCombatRoomForEquipmentChange();
            EnhanceClassForArmyRespectingGearPolicy(className, allowFullGearEnhance: !preserveGear);
            CoreBots.Instance.Equip(className);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(className), 20);
        }

        _syncedClassType = ClassType.None;

        // Start Skua's Advanced Skills Set rotation for the class being ranked up!
        // (Adv.RankUpClass only calls Core.Equip, which does not start Bot.Skills.StartAdvanced!)
        Army.StartClassSkills(className);

        if (preserveGear)
        {
            // Adv.RankUpClass calls SmartEnhance(className) internally without ForceEnh, which would overwrite
            // equipped endgame Forge enhancements on Weapon/Helm/Cape. Farm /icestormarena CP directly instead.
            if (!HasRank10Class(className))
            {
                Farm.ToggleBoost(BoostType.Class);
                Farm.IcestormArena(Bot.Player.Level, true);
                CoreBots.Instance.Jump("Enter");
                Bot.Options.AggroMonsters = false;
                Farm.ToggleBoost(BoostType.Class, false);
            }
            return;
        }

        // Pass gearRestore: false so that when this account hits Rank 10 in /icestormarena while other
        // accounts are still fighting, it doesn't try to restore/re-enhance old gear inside the active combat room!
        Adv.RankUpClass(className, gearRestore: false);
    }

    private bool RunBlacksmithingRank4Step(string objectiveName, Func<bool> alreadySatisfied)
    {
        bool IsDone() => alreadySatisfied() || Farm.FactionRank("Blacksmithing") >= 4;

        return RunCooperativeObjective(
            objectiveName,
            IsDone,
            () =>
            {
                if (IsDone())
                    return;

                SetClass(ClassType.Farm);
                FarmBlacksmithingRank4();
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Army.CoordinateFollowAndCombat(targetPlayerName, compositionType: ClassType.Farm);
            }
        );
    }

    private void FarmBlacksmithingRank4()
    {
        if (Bot.ShouldExit || Farm.FactionRank("Blacksmithing") >= 4)
            return;

        CoreBots core = CoreBots.Instance;
        core.Logger("Farming Blacksmithing Rank 4 via Quest 2777 in /greenguardwest...");
        int turnIns = 0;
        try
        {
            while (!Bot.ShouldExit && Farm.FactionRank("Blacksmithing") < 4)
            {
                if (!Bot.Quests.IsInProgress(2777))
                    core.EnsureAccept(2777);

                core.KillMonster("greenguardwest", "West4", "Right", "Slime", "Slimy Lost Sock", 5, isTemp: true, log: false);

                if (Bot.ShouldExit)
                    break;

                bool completed = core.EnsureComplete(2777);
                if (!completed)
                {
                    core.JumpWait();
                    Bot.Sleep(500);
                    completed = core.EnsureComplete(2777);
                }

                if (completed)
                {
                    Bot.Wait.ForTrue(
                        () => !Bot.Quests.IsInProgress(2777) && !Bot.TempInv.Contains("Slimy Lost Sock", 5),
                        15
                    );
                    turnIns++;
                    if (turnIns == 1 || turnIns % 5 == 0 || Farm.FactionRank("Blacksmithing") >= 4)
                    {
                        core.Logger(
                            $"Blacksmithing Rep 4 progress: {Farm.TotalFactionRep("Blacksmithing")}/10000 "
                                + $"(Rank {Farm.FactionRank("Blacksmithing")}, turn-in #{turnIns})."
                        );
                    }
                }
            }
        }
        finally
        {
            Farm.ToggleBoost(BoostType.Reputation, false);
            if (Bot.Quests.IsInProgress(2777))
                core.AbandonQuest(2777);
            LeaveCombatRoomForEquipmentChange();
        }
    }

    private void LeaveCombatRoomForEquipmentChange()
    {
        try
        {
            Bot.Skills.Stop();
        }
        catch
        {
            // Ignore if skill timer is not active
        }

        if (Bot.Player.HasTarget)
            Bot.Combat.CancelTarget();

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        // 1. Prefer jumping to the safe "Enter" cell on the current map instead of forcing /whitemap.
        if (Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0)
        {
            CoreBots.Instance.JumpWait();
            Bot.Wait.ForCombatExit(20);
        }

        // 2. Only fall back to /whitemap if the current cell still has monsters or combat did not exit.
        string currentMap = Bot.Map?.Name ?? string.Empty;
        if (
            !string.Equals(currentMap, "whitemap", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(currentMap, "battleon", StringComparison.OrdinalIgnoreCase)
            && (Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0)
        )
        {
            CoreBots.Instance.Join("whitemap");
            Bot.Wait.ForMapLoad("whitemap");
            CoreBots.Instance.JumpWait();
            Bot.Wait.ForCombatExit(20);
        }
    }

    private readonly HashSet<int> _unequippableBoostItemIds = new();

    private readonly struct BoostProfile
    {
        public double DmgAll { get; init; }
        public double Chaos { get; init; }
        public double Dragonkin { get; init; }
        public double Elemental { get; init; }
        public double Human { get; init; }
        public double Undead { get; init; }
        public double Cp { get; init; }
        public double Exp { get; init; }
        public double Gold { get; init; }
        public double Rep { get; init; }

        public bool HasAnyBoost =>
            DmgAll > 0
            || Chaos > 0
            || Dragonkin > 0
            || Elemental > 0
            || Human > 0
            || Undead > 0
            || Cp > 0
            || Exp > 0
            || Gold > 0
            || Rep > 0;
    }

    private static double ParseBoostFraction(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return 0;

        string cleaned = rawValue.Trim();
        bool hasPercent = cleaned.EndsWith("%", StringComparison.Ordinal);
        if (hasPercent)
            cleaned = cleaned.Substring(0, cleaned.Length - 1).Trim();
        if (cleaned.StartsWith("+", StringComparison.Ordinal))
            cleaned = cleaned.Substring(1).Trim();

        if (!double.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            return 0;
        if (!double.IsFinite(value) || value <= 0)
            return 0;

        double bonus = hasPercent || value >= 5.0
            ? value / 100.0
            : value >= 1.0
                ? value - 1.0
                : value;

        return Math.Max(0, bonus);
    }

    private static BoostProfile ParseBoostProfile(ItemBase? item)
    {
        string? meta = item?.Meta;
        if (string.IsNullOrWhiteSpace(meta))
            return default;

        double dmgAll = 0;
        double chaos = 0;
        double dragonkin = 0;
        double elemental = 0;
        double human = 0;
        double undead = 0;
        double cp = 0;
        double exp = 0;
        double gold = 0;
        double rep = 0;

        string[] entries = meta.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string entry in entries)
        {
            int sepIndex = entry.IndexOf(':');
            if (sepIndex < 0)
                sepIndex = entry.IndexOf('=');
            if (sepIndex <= 0 || sepIndex >= entry.Length - 1)
                continue;

            string key = entry.Substring(0, sepIndex).Trim().ToLowerInvariant();
            double fraction = ParseBoostFraction(entry.Substring(sepIndex + 1));
            if (fraction <= 0)
                continue;

            switch (key)
            {
                case "dmgall":
                case "alldmg":
                case "all":
                    dmgAll = Math.Max(dmgAll, fraction);
                    break;
                case "chaos":
                    chaos = Math.Max(chaos, fraction);
                    break;
                case "dragonkin":
                case "dragon":
                    dragonkin = Math.Max(dragonkin, fraction);
                    break;
                case "elemental":
                    elemental = Math.Max(elemental, fraction);
                    break;
                case "human":
                    human = Math.Max(human, fraction);
                    break;
                case "undead":
                    undead = Math.Max(undead, fraction);
                    break;
                case "cp":
                case "classpoints":
                    cp = Math.Max(cp, fraction);
                    break;
                case "exp":
                case "xp":
                    exp = Math.Max(exp, fraction);
                    break;
                case "gold":
                    gold = Math.Max(gold, fraction);
                    break;
                case "rep":
                case "reputation":
                    rep = Math.Max(rep, fraction);
                    break;
            }
        }

        return new BoostProfile
        {
            DmgAll = dmgAll,
            Chaos = chaos,
            Dragonkin = dragonkin,
            Elemental = elemental,
            Human = human,
            Undead = undead,
            Cp = cp,
            Exp = exp,
            Gold = gold,
            Rep = rep,
        };
    }

    private static bool Dominates(in BoostProfile a, in BoostProfile b)
    {
        const double eps = 1e-6;
        return a.DmgAll >= b.DmgAll - eps
            && a.Chaos >= b.Chaos - eps
            && a.Dragonkin >= b.Dragonkin - eps
            && a.Elemental >= b.Elemental - eps
            && a.Human >= b.Human - eps
            && a.Undead >= b.Undead - eps
            && a.Cp >= b.Cp - eps
            && a.Exp >= b.Exp - eps
            && a.Gold >= b.Gold - eps
            && a.Rep >= b.Rep - eps;
    }

    private static double Max4(double v1, double v2, double v3, double v4) =>
        Math.Max(Math.Max(v1, v2), Math.Max(v3, v4));

    private enum BoostObjective
    {
        Balanced,
        Combat,
        Exp,
        Gold,
        Rep,
        ClassPoints,
    }

    private BoostObjective _activeBoostObjective = BoostObjective.Balanced;

    private static string? NormalizeMonsterRace(string? race)
    {
        if (string.IsNullOrWhiteSpace(race))
            return null;

        string trimmed = race.Trim();
        if (trimmed.Equals("None", StringComparison.OrdinalIgnoreCase))
            return null;
        if (trimmed.Contains("Chaos", StringComparison.OrdinalIgnoreCase))
            return "Chaos";
        if (trimmed.Contains("Dragon", StringComparison.OrdinalIgnoreCase))
            return "Dragonkin";
        if (trimmed.Contains("Elemental", StringComparison.OrdinalIgnoreCase))
            return "Elemental";
        if (trimmed.Contains("Human", StringComparison.OrdinalIgnoreCase))
            return "Human";
        if (trimmed.Contains("Undead", StringComparison.OrdinalIgnoreCase))
            return "Undead";

        return null;
    }

    private string? DetectCurrentCellMonsterRace()
    {
        try
        {
            string? targetRace = NormalizeMonsterRace(
                Bot.Flash.GetGameObject("world.myAvatar.target.objData.sRace")
            );
            if (!string.IsNullOrEmpty(targetRace))
                return targetRace;

            if (Bot.Monsters?.CurrentMonsters != null)
            {
                foreach (var monster in Bot.Monsters.CurrentMonsters)
                {
                    if (monster == null || !monster.Alive || monster.MapID <= 0)
                        continue;

                    string? race = NormalizeMonsterRace(
                        Bot.Flash.GetGameObject($"world.monsters.{monster.MapID}.objData.sRace")
                    );
                    if (!string.IsNullOrEmpty(race))
                        return race;
                }
            }
        }
        catch
        {
            // Ignore transient Flash lookup errors when not in combat or map is transitioning.
        }

        return null;
    }

    private static double ScoreStaticLoadout(
        in BoostProfile w,
        in BoostProfile a,
        in BoostProfile c,
        in BoostProfile h,
        int playerLevel,
        BoostObjective objective = BoostObjective.Balanced,
        string? targetRace = null
    )
    {
        double dmgAll = Max4(w.DmgAll, a.DmgAll, c.DmgAll, h.DmgAll);
        double chaos = Max4(w.Chaos, a.Chaos, c.Chaos, h.Chaos);
        double dragonkin = Max4(w.Dragonkin, a.Dragonkin, c.Dragonkin, h.Dragonkin);
        double elemental = Max4(w.Elemental, a.Elemental, c.Elemental, h.Elemental);
        double human = Max4(w.Human, a.Human, c.Human, h.Human);
        double undead = Max4(w.Undead, a.Undead, c.Undead, h.Undead);
        double cp = Max4(w.Cp, a.Cp, c.Cp, h.Cp);
        double exp = Max4(w.Exp, a.Exp, c.Exp, h.Exp);
        double gold = Max4(w.Gold, a.Gold, c.Gold, h.Gold);
        double rep = Max4(w.Rep, a.Rep, c.Rep, h.Rep);

        double combatFactor;
        if (!string.IsNullOrEmpty(targetRace))
        {
            double targetedRaceBoost = targetRace switch
            {
                "Chaos" => chaos,
                "Dragonkin" => dragonkin,
                "Elemental" => elemental,
                "Human" => human,
                "Undead" => undead,
                _ => 0.0,
            };
            combatFactor = (1.0 + dmgAll) * (1.0 + targetedRaceBoost) - 1.0;
        }
        else
        {
            double dmgFactorChaos = (1.0 + dmgAll) * (1.0 + chaos) - 1.0;
            double dmgFactorDragon = (1.0 + dmgAll) * (1.0 + dragonkin) - 1.0;
            double dmgFactorElem = (1.0 + dmgAll) * (1.0 + elemental) - 1.0;
            double dmgFactorHuman = (1.0 + dmgAll) * (1.0 + human) - 1.0;
            double dmgFactorUndead = (1.0 + dmgAll) * (1.0 + undead) - 1.0;

            combatFactor =
                0.25 * dmgAll
                + 0.15 * (dmgFactorChaos + dmgFactorDragon + dmgFactorElem + dmgFactorHuman + dmgFactorUndead);
        }

        double effectiveExp = playerLevel >= 100 ? 0.0 : exp;

        return objective switch
        {
            BoostObjective.Combat =>
                combatFactor * 1500.0 + cp * 60.0 + effectiveExp * 60.0 + rep * 50.0 + gold * 40.0,
            BoostObjective.ClassPoints =>
                cp * 1500.0 + combatFactor * 450.0 + effectiveExp * 80.0 + rep * 60.0 + gold * 50.0,
            BoostObjective.Exp =>
                effectiveExp * 1500.0 + combatFactor * 450.0 + cp * 80.0 + rep * 60.0 + gold * 50.0,
            BoostObjective.Rep =>
                rep * 1500.0 + combatFactor * 450.0 + cp * 60.0 + effectiveExp * 60.0 + gold * 50.0,
            BoostObjective.Gold =>
                gold * 1500.0 + combatFactor * 450.0 + cp * 60.0 + effectiveExp * 60.0 + rep * 50.0,
            _ =>
                combatFactor * 1000.0 + cp * 120.0 + effectiveExp * 120.0 + rep * 100.0 + gold * 80.0,
        };
    }

    private sealed class BoostCandidate
    {
        public BoostCandidate(InventoryItem? item, BoostProfile profile, bool isBankItem)
        {
            Item = item;
            Profile = profile;
            IsBankItem = isBankItem;
        }

        public InventoryItem? Item { get; }
        public BoostProfile Profile { get; }
        public bool IsBankItem { get; }
        public bool IsEquipped => Item?.Equipped == true;
    }

    private bool ShouldPreserveEquippedGear()
    {
        CoreBots core = CoreBots.Instance;
        if (core.CBOBool("DisableBestGear", out bool disableBestGear) && disableBestGear)
            return true;

        if (Bot.Config == null)
            return false;

        BoostGearPolicy policy = Bot.Config.Get<BoostGearPolicy>("BoostGearPolicy");
        if (policy == BoostGearPolicy.KeepEquippedGear)
            return true;

        string? keepPlayers = Bot.Config.Get<string>("KeepEquippedGearPlayers");
        if (string.IsNullOrWhiteSpace(keepPlayers))
            return false;

        int localPlayerNumber = GetLocalPlayerNumber();
        string localUsername = Bot.Player?.Username?.Trim() ?? string.Empty;
        string[] tokens = keepPlayers.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string rawToken in tokens)
        {
            string token = rawToken.Trim();
            if (string.IsNullOrEmpty(token))
                continue;

            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int playerNumber) && playerNumber == localPlayerNumber)
                return true;

            if (!string.IsNullOrEmpty(localUsername) && string.Equals(token, localUsername, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private List<InventoryItem> EnumerateOwnedInventoryAndBankItems()
    {
        var byId = new Dictionary<int, InventoryItem>();

        if (Bot.Inventory?.Items != null)
        {
            foreach (InventoryItem item in Bot.Inventory.Items)
            {
                if (item != null && item.ID > 0 && !byId.ContainsKey(item.ID))
                    byId[item.ID] = item;
            }
        }

        if (Bot.Bank?.Items != null)
        {
            foreach (InventoryItem item in Bot.Bank.Items)
            {
                if (item != null && item.ID > 0 && !byId.ContainsKey(item.ID))
                    byId[item.ID] = item;
            }
        }

        return byId.Values.ToList();
    }

    private static bool MatchesBoostSlot(InventoryItem item, string slotKey) =>
        slotKey switch
        {
            "Weapon" => string.Equals(item.ItemGroup, "Weapon", StringComparison.OrdinalIgnoreCase),
            "Armor" => item.Category == ItemCategory.Armor,
            "Cape" => item.Category == ItemCategory.Cape,
            "Helm" => item.Category == ItemCategory.Helm,
            _ => false,
        };

    private List<BoostCandidate> GetPrunedSlotCandidates(
        List<InventoryItem> ownedItems,
        string slotKey,
        int playerLevel,
        bool isMember,
        string? targetRace,
        out BoostCandidate currentCandidate
    )
    {
        InventoryItem? equippedItem = null;
        var rawBoostCandidates = new List<BoostCandidate>();

        foreach (InventoryItem item in ownedItems)
        {
            if (item == null || item.Temp || !MatchesBoostSlot(item, slotKey))
                continue;

            if (item.Equipped)
                equippedItem = item;

            if (!item.Equipped)
            {
                if (_unequippableBoostItemIds.Contains(item.ID))
                    continue;
                if (item.Upgrade && !isMember)
                    continue;
            }

            BoostProfile profile = ParseBoostProfile(item);
            if (!item.Equipped && !profile.HasAnyBoost)
                continue;

            bool isBankOnly = !Bot.Inventory!.Contains(item.ID);
            rawBoostCandidates.Add(new BoostCandidate(item, profile, isBankOnly));
        }

        currentCandidate =
            rawBoostCandidates.FirstOrDefault(c => c.IsEquipped)
            ?? new BoostCandidate(equippedItem, ParseBoostProfile(equippedItem), false);

        var pruned = new List<BoostCandidate> { currentCandidate };
        BoostProfile zero = default;

        // Sort higher-value single-slot items first so dominance filtering converges immediately.
        foreach (
            BoostCandidate candidate in rawBoostCandidates
                .Where(c => !c.IsEquipped)
                .OrderByDescending(c => ScoreStaticLoadout(c.Profile, zero, zero, zero, playerLevel, _activeBoostObjective, targetRace))
                .ThenBy(c => c.IsBankItem ? 1 : 0)
                .ThenBy(c => c.Item?.ID ?? 0)
        )
        {
            bool dominated = pruned.Any(existing =>
                Dominates(existing.Profile, candidate.Profile)
                && (
                    !Dominates(candidate.Profile, existing.Profile)
                    || existing.IsEquipped
                    || !existing.IsBankItem && candidate.IsBankItem
                )
            );

            if (!dominated)
            {
                pruned.Add(candidate);
                if (pruned.Count >= 9)
                    break;
            }
        }

        return pruned;
    }

    private List<InventoryItem> ResolveBestStaticBoostSwaps(string? targetRace = null)
    {
        var swaps = new List<InventoryItem>();
        if (ShouldPreserveEquippedGear() || Bot.Inventory == null)
            return swaps;

        int playerLevel = Bot.Player.Level;
        bool isMember = CoreBots.Instance.IsMember;
        List<InventoryItem> owned = EnumerateOwnedInventoryAndBankItems();

        List<BoostCandidate> weapons = GetPrunedSlotCandidates(owned, "Weapon", playerLevel, isMember, targetRace, out BoostCandidate curW);
        List<BoostCandidate> armors = GetPrunedSlotCandidates(owned, "Armor", playerLevel, isMember, targetRace, out BoostCandidate curA);
        List<BoostCandidate> capes = GetPrunedSlotCandidates(owned, "Cape", playerLevel, isMember, targetRace, out BoostCandidate curC);
        List<BoostCandidate> helms = GetPrunedSlotCandidates(owned, "Helm", playerLevel, isMember, targetRace, out BoostCandidate curH);

        double currentScore = ScoreStaticLoadout(curW.Profile, curA.Profile, curC.Profile, curH.Profile, playerLevel, _activeBoostObjective, targetRace);
        double bestScore = currentScore;
        int bestEquippedCount =
            (curW.IsEquipped ? 1 : 0)
            + (curA.IsEquipped ? 1 : 0)
            + (curC.IsEquipped ? 1 : 0)
            + (curH.IsEquipped ? 1 : 0);

        BoostCandidate bestW = curW;
        BoostCandidate bestA = curA;
        BoostCandidate bestC = curC;
        BoostCandidate bestH = curH;

        foreach (BoostCandidate w in weapons)
        foreach (BoostCandidate a in armors)
        foreach (BoostCandidate c in capes)
        foreach (BoostCandidate h in helms)
        {
            double score = ScoreStaticLoadout(w.Profile, a.Profile, c.Profile, h.Profile, playerLevel, _activeBoostObjective, targetRace);
            int equippedCount =
                (w.IsEquipped ? 1 : 0)
                + (a.IsEquipped ? 1 : 0)
                + (c.IsEquipped ? 1 : 0)
                + (h.IsEquipped ? 1 : 0);

            if (
                score > bestScore + 1e-4
                || (Math.Abs(score - bestScore) <= 1e-4 && equippedCount > bestEquippedCount)
            )
            {
                bestScore = score;
                bestEquippedCount = equippedCount;
                bestW = w;
                bestA = a;
                bestC = c;
                bestH = h;
            }
        }

        double minImprovement = string.IsNullOrEmpty(targetRace) ? 1e-4 : 15.0;
        if (bestScore <= currentScore + minImprovement)
            return swaps;

        if (bestW.Item != null && !bestW.IsEquipped)
            swaps.Add(bestW.Item);
        if (bestA.Item != null && !bestA.IsEquipped)
            swaps.Add(bestA.Item);
        if (bestC.Item != null && !bestC.IsEquipped)
            swaps.Add(bestC.Item);
        if (bestH.Item != null && !bestH.IsEquipped)
            swaps.Add(bestH.Item);

        return swaps;
    }

    private bool HasPendingBoostGearUpgrade() => ResolveBestStaticBoostSwaps().Count > 0;

    private bool HasOwnedBoostWeaponAtLeast(double minDmgAll)
    {
        bool isMember = CoreBots.Instance.IsMember;
        foreach (InventoryItem item in EnumerateOwnedInventoryAndBankItems())
        {
            if (item == null || item.Temp || !MatchesBoostSlot(item, "Weapon"))
                continue;
            if ((item.Upgrade && !isMember) || _unequippableBoostItemIds.Contains(item.ID))
                continue;

            BoostProfile profile = ParseBoostProfile(item);
            if (profile.DmgAll >= minDmgAll - 1e-6)
                return true;

            string trimmedName = item.Name?.Trim() ?? string.Empty;
            if (
                minDmgAll <= 0.15 + 1e-6
                && (
                    string.Equals(trimmedName, "Burning Blade", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(trimmedName, "Burning Blade of Abezeth", StringComparison.OrdinalIgnoreCase)
                )
            )
                return true;
            if (
                minDmgAll <= 0.30 + 1e-6
                && string.Equals(trimmedName, "Burning Blade of Abezeth", StringComparison.OrdinalIgnoreCase)
            )
                return true;
        }

        return false;
    }

    private bool HasOwnedCapeOfAweOrBetter()
    {
        CoreBots core = CoreBots.Instance;
        if (core.CheckInventory("Cape of Awe", toInv: false))
            return true;

        bool isMember = core.IsMember;
        int playerLevel = Bot.Player.Level;
        bool isMaxLevel = playerLevel >= 100;

        foreach (InventoryItem item in EnumerateOwnedInventoryAndBankItems())
        {
            if (item == null || item.Temp || !MatchesBoostSlot(item, "Cape"))
                continue;
            if ((item.Upgrade && !isMember) || _unequippableBoostItemIds.Contains(item.ID))
                continue;

            BoostProfile profile = ParseBoostProfile(item);
            bool coversResources =
                profile.Cp >= 0.25 - 1e-6
                && profile.Gold >= 0.25 - 1e-6
                && profile.Rep >= 0.25 - 1e-6
                && (isMaxLevel || profile.Exp >= 0.25 - 1e-6);

            if (coversResources)
                return true;
        }

        return false;
    }

    private void EnsureBestStaticBoostGearEquipped(BoostObjective? objective = null, string? targetRace = null)
    {
        if (objective.HasValue)
            _activeBoostObjective = objective.Value;

        if (Bot.ShouldExit || ShouldPreserveEquippedGear())
            return;

        if (string.IsNullOrEmpty(targetRace) && _activeBoostObjective == BoostObjective.Combat)
            targetRace = DetectCurrentCellMonsterRace();

        List<InventoryItem> swaps = ResolveBestStaticBoostSwaps(targetRace);
        if (swaps.Count == 0)
            return;

        CoreBots core = CoreBots.Instance;
        LeaveCombatRoomForEquipmentChange();

        int[] bankIds = swaps
            .Where(item => !Bot.Inventory!.Contains(item.ID))
            .Select(item => item.ID)
            .Distinct()
            .ToArray();
        if (bankIds.Length > 0)
            core.Unbank(bankIds);

        bool needEnhance = false;
        foreach (InventoryItem target in swaps)
        {
            if (Bot.ShouldExit)
                break;

            if (!Bot.Inventory!.Contains(target.ID))
            {
                core.Logger($"BoostGear: could not unbank '{target.Name}' ({target.ID}); skipping.");
                _unequippableBoostItemIds.Add(target.ID);
                continue;
            }

            if (!Bot.Inventory.IsEquipped(target.ID))
            {
                core.Logger($"BoostGear: equipping '{target.Name}' ({target.Category}, Meta='{target.Meta}').");
                core.Equip(target.ID);
                Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(target.ID), 20);
            }

            if (!Bot.Inventory.IsEquipped(target.ID))
            {
                core.Logger($"BoostGear: '{target.Name}' ({target.ID}) failed to equip; marking skipped.");
                _unequippableBoostItemIds.Add(target.ID);
                continue;
            }

            if (target.Category != ItemCategory.Armor)
            {
                InventoryItem? equippedNow = Bot.Inventory.Items.FirstOrDefault(x => x != null && x.ID == target.ID);
                int cap = Math.Min(Bot.Player.Level, 100);
                if (
                    equippedNow != null
                    && (equippedNow.EnhancementLevel <= 0 || cap - equippedNow.EnhancementLevel >= 10)
                )
                    needEnhance = true;
            }
        }

        if (needEnhance && !Bot.ShouldExit)
        {
            string? currentClass = Bot.Player.CurrentClass?.Name;
            if (!string.IsNullOrWhiteSpace(currentClass))
                EnsureCombatEnhancementReady(currentClass, allowFullGearEnhance: true);
            if (!string.IsNullOrWhiteSpace(currentClass))
                Army.EnsureSkillRotationActive();
        }
    }

    private bool SyncBoostGearForArmy(string objectiveName)
    {
        return RunCooperativeObjective(
            objectiveName,
            () => ShouldPreserveEquippedGear() || !HasPendingBoostGearUpgrade(),
            () =>
            {
                if (ShouldPreserveEquippedGear())
                    return;

                EnsureBestStaticBoostGearEquipped();
            },
            (targetPlayerNumber, targetPlayerName) =>
            {
                Bot.Sleep(1000);
            },
            maxWorkAttempts: 2
        );
    }

    private bool IsAweGuardian()
    {
        try
        {
            return Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intAQ") > 0;
        }
        catch
        {
            return false;
        }
    }

    private bool RunBracket55CapeOfAwe(string objectivePrefix = "BRACKET55")
    {
        CoreBots core = CoreBots.Instance;

        bool isGuardian = IsAweGuardian();
        int questId = isGuardian ? 4179 : core.IsMember ? 4178 : 4180;
        int passId = isGuardian ? 29403 : core.IsMember ? 29402 : 29404;
        int passShopItemId = isGuardian ? 18569 : core.IsMember ? 18580 : 18579;
        string passName = isGuardian
            ? "Guardian Awe Pass"
            : core.IsMember
                ? "Legendary Awe Pass"
                : "Armor of Awe Pass";
        string passMap = isGuardian ? "tower" : "museum";
        int passShopId = isGuardian ? 53 : 1130;
        int minRep = isGuardian ? 5 : 10;

        bool HasCapeMaterialOrDone() =>
            HasOwnedCapeOfAweOrBetter()
            || core.CheckInventory("Cape of Awe", toInv: false)
            || core.CheckInventory("Cape Relic", toInv: false)
            || core.CheckInventory("Cape Fragment", toInv: false)
            || core.CheckInventory("Cape Shard", toInv: false);

        // Ensure Blade of Awe prerequisite rank if an account entered Bracket 55 without it.
        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_COA_REP",
                () =>
                    HasCapeMaterialOrDone()
                    || (
                        Farm.FactionRank("Blade of Awe") >= minRep
                        && (isGuardian || core.CheckInventory(17585, toInv: false))
                    ),
                () =>
                {
                    bool needBladeOfAwe = !isGuardian && !core.CheckInventory(17585, toInv: false);
                    if (
                        HasCapeMaterialOrDone()
                        || (Farm.FactionRank("Blade of Awe") >= minRep && !needBladeOfAwe)
                    )
                        return;

                    SetClass(ClassType.Farm);
                    Farm.BladeofAweREP(minRep, farmBoA: needBladeOfAwe);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Army.CoordinateFollowAndCombat(targetPlayerName, "Skeletal Viking", compositionType: ClassType.Farm);
                }
            )
        )
            return false;

        // Prep: acquire Awe Pass & accept quest on all pending accounts before entering /doomvault r5.
        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_COA_PREP",
                () =>
                    HasCapeMaterialOrDone()
                    || (Bot.Quests.IsInProgress(questId) && core.CheckInventory(passId, toInv: true)),
                () =>
                {
                    UnbankIfOwned(passName, "Cape Shard", "Cape Fragment", "Cape Relic", "Cape of Awe");
                    if (HasCapeMaterialOrDone())
                        return;

                    if (!core.CheckInventory(passId, toInv: true))
                    {
                        LeaveCombatRoomForEquipmentChange();
                        Adv.BuyItem(passMap, passShopId, passId, shopItemID: passShopItemId);
                        Bot.Wait.ForItemBuy();
                    }

                    core.AddDrop("Cape Shard", "Cape Fragment", "Cape Relic", "Cape of Awe");
                    core.EnsureAccept(questId);
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        // Coordinated Binky fight (/doomvault r5 Left -> Cape Shard, room cap 5 -> subgroup 4).
        ApplySubgroupRoomSettings(4);
        try
        {
            if (
                !RunCooperativeObjective(
                    $"{objectivePrefix}_COA_BINKY",
                    HasCapeMaterialOrDone,
                    () =>
                    {
                        UnbankIfOwned(passName, "Cape Shard", "Cape Fragment", "Cape Relic", "Cape of Awe");
                        if (HasCapeMaterialOrDone())
                            return;

                        ApplyCurrentArmyRoomSettings();
                        core.Logger("Level 55: Farming Cape Shard from Binky in /doomvault (Cape of Awe)");
                        core.AddDrop("Cape Shard", "Cape Fragment");
                        core.EnsureAccept(questId);
                        SetClass(ClassType.Solo);
                        core.KillMonster("doomvault", "r5", "Left", "Binky", "Cape Shard", isTemp: false);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                    {
                        ApplyCurrentArmyRoomSettings();
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            "Binky",
                            compositionType: ClassType.Solo,
                            targetMap: "doomvault"
                        );
                    },
                    maxPlayersPerSubgroup: 4,
                    prepareForCombat: () => ApplyCurrentArmyRoomSettings()
                )
            )
                return false;
        }
        finally
        {
            ApplyArmyRoomSettings();
        }

        // Turn in quest -> buy Cape Relic -> buy Cape of Awe -> bank pass.
        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_COA_BUY",
                HasOwnedCapeOfAweOrBetter,
                () =>
                {
                    if (HasOwnedCapeOfAweOrBetter())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    UnbankIfOwned(passName, "Cape Shard", "Cape Fragment", "Cape Relic", "Cape of Awe");
                    core.AddDrop("Cape Fragment", "Cape Relic", "Cape of Awe");

                    if (!core.CheckInventory("Cape of Awe", toInv: false))
                    {
                        if (!core.CheckInventory("Cape Relic", toInv: false))
                        {
                            if (!core.CheckInventory("Cape Fragment", toInv: false) && core.CheckInventory("Cape Shard", toInv: false))
                            {
                                core.EnsureComplete(questId);
                                Bot.Wait.ForPickup("Cape Fragment");
                            }

                            if (core.CheckInventory("Cape Fragment", toInv: false))
                            {
                                Adv.BuyItem("museum", 1129, "Cape Relic");
                                Bot.Wait.ForItemBuy();
                            }
                        }

                        if (core.CheckInventory("Cape Relic", toInv: false))
                        {
                            Adv.BuyItem("museum", 1129, "Cape of Awe");
                            Bot.Wait.ForItemBuy();
                        }
                    }

                    BankIfPresent("Legendary Awe Pass", "Guardian Awe Pass", "Armor of Awe Pass");
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    Bot.Sleep(1000);
                },
                maxWorkAttempts: 3
            )
        )
            return false;

        return SyncBoostGearForArmy($"{objectivePrefix}_COA_EQUIP");
    }

    private string ResolveClass(string[] pool)
    {
        // Prefer the highest-priority class in the pool that is already Rank 10 in Inventory,
        // falling back to the highest-priority owned class in Inventory.
        string? bestRank10 = pool.FirstOrDefault(className =>
            HasClassInInventory(className) && HasRank10Class(className)
        );
        if (!string.IsNullOrEmpty(bestRank10))
            return bestRank10;

        return pool.FirstOrDefault(HasClassInInventory) ?? "Generic";
    }

    private void RankIfNeeded(string? className, string label)
    {
        CoreBots core = CoreBots.Instance;
        if (string.IsNullOrEmpty(className) || CoreBots.IsGenericClass(className))
            return;

        if (!HasClassInInventory(className))
        {
            core.Logger($"WARNING: {label} class '{className}' not in inventory!");
            return;
        }

        if (HasRank10Class(className))
            return;

        core.Logger($"{label} resolved -> {className}");
        RankUpClassForArmy(className);
    }
    #endregion SetClass

    private bool HasClassInInventory(string? className) =>
        !string.IsNullOrWhiteSpace(className)
        && Bot.Inventory?.Items.Any(item =>
            item.Category == ItemCategory.Class
            && item.Name?.Trim().Equals(className.Trim(), StringComparison.OrdinalIgnoreCase) == true
        ) == true;

    private bool HasClassInInventory(int itemId) =>
        itemId > 0
        && Bot.Inventory?.Items.Any(item =>
            item.Category == ItemCategory.Class && item.ID == itemId
        ) == true;

    private bool HasAnyClassInInventory(params string[] classNames) =>
        classNames.Any(HasClassInInventory);

    private bool HasAnyClassInInventory(params int[] itemIds) =>
        itemIds.Any(HasClassInInventory);

    private bool HasRank10Class(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return false;

        InventoryItem? item = Bot.Inventory?.Items.FirstOrDefault(item =>
            item.Category == ItemCategory.Class
            && item.Name?.Trim().Equals(className.Trim(), StringComparison.OrdinalIgnoreCase) == true
        );

        return item != null && item.Quantity >= Rank10ClassPoints;
    }

    #region Approved Dailies, Weeklies & Future NSoD Daily
    private sealed class CoordinatedStoryStep
    {
        public CoordinatedStoryStep(
            int questId,
            string map,
            string helpMonster,
            Action<CoreBots> work,
            ClassType role = ClassType.Farm,
            bool isCombat = true,
            bool skipMapVerify = false
        )
        {
            QuestId = questId;
            Map = map;
            HelpMonster = helpMonster;
            Work = work;
            Role = role;
            IsCombat = isCombat;
            SkipMapVerify = skipMapVerify;
        }

        public CoordinatedStoryStep(
            int questId,
            string map,
            string helpMonster,
            Action work,
            ClassType role = ClassType.Farm,
            bool isCombat = true,
            bool skipMapVerify = false
        )
            : this(questId, map, helpMonster, _ => work(), role, isCombat, skipMapVerify) { }

        public int QuestId { get; }
        public string Map { get; }
        public string HelpMonster { get; }
        public Action<CoreBots> Work { get; }
        public ClassType Role { get; }
        public bool IsCombat { get; }
        public bool SkipMapVerify { get; }
    }

    private Quest? TryLoadQuest(int questId)
    {
        CoreBots core = CoreBots.Instance;
        return core.InitializeWithRetries(() => core.EnsureLoad(questId));
    }

    private bool CanRunPeriodicQuest(int questId, bool requireAvailable = false)
    {
        if (TryLoadQuest(questId) == null)
            return false;

        if (Bot.Quests.IsDailyComplete(questId))
            return false;

        return !requireAvailable || Bot.Quests.IsAvailable(questId);
    }

    private int GetInventoryAndBankQuantity(int itemId) =>
        (Bot.Inventory?.Items ?? Enumerable.Empty<InventoryItem>())
            .Concat(Bot.Bank?.Items ?? Enumerable.Empty<InventoryItem>())
            .Where(item => item != null && item.ID == itemId)
            .Sum(item => item.Quantity);

    private void BankIfPresent(params string[] itemNames)
    {
        if (itemNames == null || itemNames.Length == 0 || Bot.Inventory?.Items == null)
            return;

        string[] owned = itemNames
            .Where(name =>
                !string.IsNullOrWhiteSpace(name)
                && Bot.Inventory.Items.Any(item =>
                    item?.Name != null
                    && string.Equals(item.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)
                )
            )
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (owned.Length == 0)
            return;

        LeaveCombatRoomForEquipmentChange();
        CoreBots.Instance.ToBank(owned);
    }

    private void BankQuestExtras(int questId, params string[] excludeNames)
    {
        string[] rewards = CoreBots.Instance.QuestRewards(questId);
        if (rewards == null || rewards.Length == 0)
            return;

        if (excludeNames != null && excludeNames.Length > 0)
        {
            rewards = rewards
                .Where(r => !excludeNames.Any(ex => string.Equals(r, ex, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        BankIfPresent(rewards);
    }

    private void UnbankIfOwned(params string[] itemNames)
    {
        if (itemNames == null || itemNames.Length == 0 || Bot.Bank?.Items == null)
            return;

        string[] inBank = itemNames
            .Where(name =>
                !string.IsNullOrWhiteSpace(name)
                && Bot.Bank.Items.Any(item =>
                    item?.Name != null
                    && string.Equals(item.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)
                )
            )
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (inBank.Length > 0)
            CoreBots.Instance.Unbank(inBank);
    }

    private void WaitForDailyRewardPickup(int questId, string rewardName)
    {
        if (Bot.Quests.IsDailyComplete(questId))
            Bot.Wait.ForPickup(rewardName);
    }

    private bool JoinMapVerified(string map, string? cell = null, string? pad = null)
    {
        CoreBots core = CoreBots.Instance;
        Army.EnsureMapAccessBypass(map);
        if (!string.IsNullOrEmpty(cell) && !string.IsNullOrEmpty(pad))
            core.Join(map, cell, pad);
        else
            core.Join(map);

        if (!string.Equals(Bot.Map?.Name, map, StringComparison.OrdinalIgnoreCase))
            return false;

        Army.EnsureMapAccessBypass(map);
        return true;
    }

    private bool RunCoordinatedTurnInStep(
        string objectiveName,
        Func<bool> initialSkipCheck,
        Func<bool> liveDoneCheck,
        Action work
    )
    {
        bool done = initialSkipCheck();
        return RunCooperativeObjective(
            objectiveName,
            () => done || liveDoneCheck(),
            () =>
            {
                if (done || liveDoneCheck())
                {
                    done = true;
                    return;
                }

                LeaveCombatRoomForEquipmentChange();
                work();
                done = true;
            },
            (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
            maxWorkAttempts: 2
        );
    }

    private static int GetSubgroupCapForMap(string? mapName)
    {
        if (string.IsNullOrWhiteSpace(mapName))
            return 0;

        string normalized = mapName.Trim().ToLowerInvariant();
        if (normalized == "dragonkoiz")
            return 3;

        return normalized switch
        {
            "aqlesson" => 4,
            "arcangrove" => 4,
            "banished" => 4,
            "battlegrounda" => 4,
            "battlegroundb" => 4,
            "battlegroundc" => 4,
            "battlegroundd" => 4,
            "battlegrounde" => 4,
            "beehive" => 4,
            "bloodtitan" => 4,
            "bonecastlec" => 4,
            "bosschallenge" => 4,
            "boxes" => 4,
            "brightfall" => 4,
            "callofkathool" => 4,
            "deathgazer" => 4,
            "deepchaos" => 4,
            "djinn" => 4,
            "doomkitten" => 4,
            "doomvault" => 4,
            "doomvaultb" => 4,
            "ectocave" => 4,
            "fireforge" => 4,
            "frozentower" => 4,
            "frozenruins" => 4,
            "gaiazor" => 4,
            "gilead" => 4,
            "gravestrike" => 4,
            "hedgemaze" => 4,
            "hydrachallenge" => 4,
            "icestormunder" => 4,
            "icewindpass" => 4,
            "icewindwar" => 4,
            "infernalspire" => 4,
            "kitsune" => 4,
            "ledgermayne" => 4,
            "lostruinswar" => 4,
            "maul" => 4,
            "mobius" => 4,
            "mqlesson" => 4,
            "mythsong" => 4,
            "oaklore" => 4,
            "odokuro" => 4,
            "sepulchurebattle" => 4,
            "shadowfortress" => 4,
            "stalagbite" => 4,
            "swordhavenundead" => 4,
            "tercessuinotlim" => 4,
            "thirdspell" => 4,
            "towerofdoom" => 4,
            "towerofdoom10" => 4,
            "towerofmirrors" => 4,
            "ultraalteon" => 4,
            "undergroundlabb" => 4,
            "underworld" => 4,
            "warhorc" => 4,
            "well" => 4,
            "wolfwing" => 4,
            "xancave" => 4,
            "yokaiwar" => 4,
            _ => 0,
        };
    }

    private bool RunCoordinatedCombatStep(
        string objectiveName,
        Func<bool> initialSkipCheck,
        Func<bool> liveDoneCheck,
        string helpMonster,
        ClassType role,
        Action work,
        int softRetryLimit = 2,
        string? targetMap = null
    )
    {
        int attempts = 0;
        bool done = initialSkipCheck();
        int previousSubgroupCap = _activeSubgroupMaxPlayers;
        int mapSubgroupCap = previousSubgroupCap > 0 ? 0 : GetSubgroupCapForMap(targetMap);

        if (mapSubgroupCap > 0)
            ApplySubgroupRoomSettings(mapSubgroupCap);

        try
        {
            return RunCooperativeObjective(
                objectiveName,
                () => done || liveDoneCheck(),
                () =>
                {
                    if (done || liveDoneCheck())
                    {
                        done = true;
                        return;
                    }

                    if (mapSubgroupCap > 0)
                        ApplyCurrentArmyRoomSettings();

                    attempts++;
                    PrepareCombatLoadoutForArmy(role, targetMap);
                    work();

                    if (liveDoneCheck())
                    {
                        done = true;
                        return;
                    }

                    if (softRetryLimit > 0 && attempts >= softRetryLimit)
                    {
                        CoreBots.Instance.Logger(
                            $"Daily objective '{objectiveName}' could not finish after {attempts} attempts; skipping cleanly so the Army can continue."
                        );
                        done = true;
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    if (mapSubgroupCap > 0)
                        ApplyCurrentArmyRoomSettings();

                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        helpMonster,
                        compositionType: role,
                        targetMap: targetMap
                    );
                },
                maxWorkAttempts: softRetryLimit > 0 ? softRetryLimit + 1 : 0,
                maxPlayersPerSubgroup: mapSubgroupCap,
                prepareForCombat: () =>
                {
                    if (mapSubgroupCap > 0)
                        ApplyCurrentArmyRoomSettings();
                    PrepareCombatLoadoutForArmy(role, targetMap);
                }
            );
        }
        finally
        {
            if (mapSubgroupCap > 0)
                ApplyArmyRoomSettings();
        }
    }

    private bool RunSubgroupCooperativeObjective(
        int subgroupCap,
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action<int, string> help,
        int maxWorkAttempts = 0,
        Action? prepareForCombat = null
    )
    {
        int previousSubgroupCap = _activeSubgroupMaxPlayers;
        int effectiveCap = previousSubgroupCap > 0 ? 0 : subgroupCap;
        if (effectiveCap > 0)
            ApplySubgroupRoomSettings(effectiveCap);

        try
        {
            return RunCooperativeObjective(
                objectiveName,
                isComplete,
                () =>
                {
                    if (effectiveCap > 0)
                        ApplyCurrentArmyRoomSettings();
                    doWork();
                },
                (targetPlayerNumber, targetPlayerName) =>
                {
                    if (effectiveCap > 0)
                        ApplyCurrentArmyRoomSettings();
                    help(targetPlayerNumber, targetPlayerName);
                },
                maxWorkAttempts: maxWorkAttempts,
                maxPlayersPerSubgroup: effectiveCap,
                prepareForCombat: prepareForCombat == null
                    ? null
                    : () =>
                    {
                        if (effectiveCap > 0)
                            ApplyCurrentArmyRoomSettings();
                        prepareForCombat();
                    }
            );
        }
        finally
        {
            if (effectiveCap > 0)
                ApplyArmyRoomSettings();
        }
    }

    private bool RunCoordinatedHuntStep(
        string objectiveName,
        int questId,
        string map,
        string monster,
        string itemName,
        int quant = 1,
        bool isTemp = false,
        ClassType role = ClassType.Solo,
        string? cell = null,
        string? pad = null,
        bool acceptQuest = true,
        bool checkBankAlso = true,
        int softRetryLimit = 2,
        Action<CoreBots>? beforeHunt = null,
        Func<bool>? isEnabled = null
    )
    {
        CoreBots core = CoreBots.Instance;

        bool IsStepDisabled() => isEnabled != null && !isEnabled();

        bool HasTargetItem() =>
            isTemp
                ? Bot.TempInv.Contains(itemName, quant)
                : core.CheckInventory(itemName, quant, toInv: !checkBankAlso);

        return RunCoordinatedCombatStep(
            objectiveName,
            () => IsStepDisabled() || HasTargetItem(),
            () => IsStepDisabled() || HasTargetItem(),
            monster,
            role,
            () =>
            {
                if (IsStepDisabled())
                    return;

                beforeHunt?.Invoke(core);
                if (!isTemp)
                    core.AddDrop(itemName);
                if (acceptQuest && questId > 0)
                    core.EnsureAccept(questId);

                if (!JoinMapVerified(map, cell, pad))
                {
                    core.Logger($"Map '/{map}' is currently unavailable for '{objectiveName}'.");
                    return;
                }

                if (!string.IsNullOrEmpty(cell) && !string.IsNullOrEmpty(pad))
                    core.KillMonster(map, cell, pad, monster, itemName, quant, isTemp);
                else
                    core.HuntMonster(map, monster, itemName, quant, isTemp);
            },
            softRetryLimit: softRetryLimit,
            targetMap: map
        );
    }

    private bool RunCoordinatedStoryChain(
        string objectivePrefix,
        CoordinatedStoryStep[] steps,
        Func<bool>? isEnabled = null,
        params string[] dropsToKeep
    )
    {
        CoreBots core = CoreBots.Instance;
        bool IsDisabled() => isEnabled != null && !isEnabled();

        if (!IsDisabled())
        {
            int[] questIdsToPreload = steps
                .Select(s => s.QuestId)
                .Where(id => id > 0)
                .Distinct()
                .ToArray();
            if (questIdsToPreload.Length > 0)
                core.EnsureLoad(questIdsToPreload);

            if (dropsToKeep != null && dropsToKeep.Length > 0)
                core.AddDrop(dropsToKeep);
        }

        foreach (CoordinatedStoryStep step in steps)
        {
            if (Bot.ShouldExit)
                return false;

            string objName = $"{objectivePrefix}_{step.QuestId}";
            int stepSubgroupCap = GetSubgroupCapForMap(step.Map);
            if (!step.IsCombat)
            {
                if (
                    !RunSubgroupCooperativeObjective(
                        stepSubgroupCap,
                        objName,
                        () => IsDisabled() || core.isCompletedBefore(step.QuestId, log: false),
                        () =>
                        {
                            if (IsDisabled() || core.isCompletedBefore(step.QuestId, log: false))
                                return;

                            LeaveCombatRoomForEquipmentChange();
                            step.Work(core);
                        },
                        (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                        maxWorkAttempts: 3
                    )
                )
                    return false;

                continue;
            }

            if (
                !RunSubgroupCooperativeObjective(
                    stepSubgroupCap,
                    objName,
                    () => IsDisabled() || core.isCompletedBefore(step.QuestId, log: false),
                    () =>
                    {
                        if (IsDisabled() || core.isCompletedBefore(step.QuestId, log: false))
                            return;

                        SetClass(step.Role);
                        if (!step.SkipMapVerify && !string.IsNullOrEmpty(step.Map))
                            core.Join(step.Map);
                        step.Work(core);
                    },
                    (targetPlayerNumber, targetPlayerName) =>
                        Army.CoordinateFollowAndCombat(
                            targetPlayerName,
                            step.HelpMonster,
                            compositionType: step.Role,
                            targetMap: step.Map
                        )
                )
            )
                return false;
        }

        return !Bot.ShouldExit;
    }

    private bool RunBracket30ApprovedDailiesAndWeeklies()
    {
        CoreBots core = CoreBots.Instance;

        // 1b-1. Weekly: Read the Design Notes! [1213] (+150 Loremaster Rep)
        bool skipDesignNotes =
            Farm.FactionRank("Loremaster") >= 10
            || !CanRunPeriodicQuest(1213);
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET30_DESIGN_NOTES_WEEKLY",
                () => skipDesignNotes,
                () => Bot.Quests.IsDailyComplete(1213) || Farm.FactionRank("Loremaster") >= 10,
                () =>
                {
                    core.Logger("Weekly: Read the Design Notes! [1213]");
                    core.ChainComplete(1213);
                }
            )
        )
            return false;

        // 1b-2. Weekly: Wheel of Doom [3076 F2P / 3075 Mem]
        bool HasGearForSpin() => core.CheckInventory("Gear of Doom", 3, toInv: false);
        bool f2pWheelReady = HasGearForSpin() && CanRunPeriodicQuest(3076);
        bool memWheelReady = core.IsMember && CanRunPeriodicQuest(3075);
        bool skipWheel = !f2pWheelReady && !memWheelReady;
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET30_WHEEL_OF_DOOM",
                () => skipWheel,
                () =>
                    (!HasGearForSpin() || Bot.Quests.IsDailyComplete(3076))
                    && (!core.IsMember || Bot.Quests.IsDailyComplete(3075)),
                () =>
                {
                    core.Logger("Weekly: Wheel of Doom [3076/3075]");
                    UnbankIfOwned("Gear of Doom");
                    core.AddDrop("Treasure Potion");
                    Daily.WheelofDoom(log: true);
                    BankIfPresent("Gear of Doom", "Treasure Potion");
                }
            )
        )
            return false;

        // 1b-3. Daily: Odd's 'n' Ends [6187] (Crypto Token, cap 300)
        bool cryptoCapped = core.CheckInventory("Crypto Token", 300, toInv: false);
        bool oddsAvailable = !cryptoCapped && CanRunPeriodicQuest(6187);
        if (
            !RunCoordinatedCombatStep(
                "BRACKET30_ODDS_N_ENDS_DAILY",
                () => !oddsAvailable,
                () => Bot.Quests.IsDailyComplete(6187) || core.CheckInventory("Crypto Token", 300, toInv: false),
                "Sneevil",
                ClassType.Farm,
                () =>
                {
                    core.Logger("Daily: Odd's 'n' Ends [6187] (/boxes Sneevil -> Crypto Token)");
                    UnbankIfOwned("Crypto Token");
                    core.AddDrop("Crypto Token");
                    core.EnsureAccept(6187);
                    core.KillMonster("boxes", "Closet", "Center", "Sneevil", "Metal Ore", 1, isTemp: true);
                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureComplete(6187);
                    WaitForDailyRewardPickup(6187, "Crypto Token");
                    BankIfPresent("Crypto Token");
                },
                targetMap: "boxes"
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    private bool EnsureLoyalFollowerUnlocked(string objectivePrefix)
    {
        CoreBots core = CoreBots.Instance;
        TryLoadQuest(2084);

        bool IsUnlocked() => core.isCompletedBefore(2084, log: false);

        // Step 1: Ensure 16,000+ Gold before /doomwood Shop 276 so helpers assist if Gold farming runs
        bool IsGoldReadyForSetup() =>
            IsUnlocked()
            || core.isCompletedBefore(2067, log: false)
            || core.CheckInventory("Bonegrinder Medal")
            || Bot.Player.Gold >= 16000;

        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_LOYAL_FOLLOWER_GOLD",
                IsGoldReadyForSetup,
                () =>
                {
                    if (IsGoldReadyForSetup())
                        return;

                    SetClass(ClassType.Farm);
                    Farm.Gold(20000);
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        compositionType: ClassType.Farm
                    )
            )
        )
            return false;

        // Step 2: 2066 (Handle) & 2067 (Bonegrinder Medal, 15,000 Gold) in /doomwood Shop 276
        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_LOYAL_FOLLOWER_SETUP",
                () => IsUnlocked() || (core.isCompletedBefore(2066, log: false) && core.isCompletedBefore(2067, log: false)),
                () =>
                {
                    if (IsUnlocked() || (core.isCompletedBefore(2066, log: false) && core.isCompletedBefore(2067, log: false)))
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    Story.BuyQuest(2066, "doomwood", 276, "Blinding Light of Destiny Handle");
                    Story.BuyQuest(2067, "doomwood", 276, "Bonegrinder Medal");
                },
                (targetPlayerNumber, targetPlayerName) => Bot.Sleep(1000),
                maxWorkAttempts: 3
            )
        )
            return false;

        // Step 2: 2082, 2083 & 100 Spirit Orbs in /battleunderb (Skeleton Warrior)
        bool AreSpiritOrbsReady() =>
            IsUnlocked()
            || (
                core.isCompletedBefore(2082, log: false)
                && core.isCompletedBefore(2083, log: false)
                && core.CheckInventory("Spirit Orb", 100, toInv: false)
            );

        if (
            !RunCooperativeObjective(
                $"{objectivePrefix}_LOYAL_FOLLOWER_SPIRIT_ORBS",
                AreSpiritOrbsReady,
                () =>
                {
                    if (AreSpiritOrbsReady())
                        return;

                    core.Logger("Unlocking Mine Crafting: 2082, 2083 & 100 Spirit Orbs in /battleunderb");
                    UnbankIfOwned("Spirit Orb", "Undead Essence", "Bone Dust");
                    core.AddDrop("Spirit Orb", "Undead Energy", "Undead Essence", "Bone Dust");
                    SetClass(ClassType.Farm);

                    if (!core.isCompletedBefore(2082, log: false))
                    {
                        core.EnsureAccept(2082);
                        core.HuntMonster("battleunderb", "Skeleton Warrior", "Undead Essence", 25, isTemp: false);
                        core.EnsureComplete(2082);
                    }

                    if (!core.isCompletedBefore(2083, log: false))
                    {
                        core.EnsureAccept(2083);
                        core.HuntMonster("battleunderb", "Skeleton Warrior", "Bone Dust", 40, isTemp: false);
                        core.EnsureComplete(2083);
                    }

                    if (!core.CheckInventory("Spirit Orb", 100))
                    {
                        try
                        {
                            core.RegisterQuests(2082, 2083);
                            while (!Bot.ShouldExit && !core.CheckInventory("Spirit Orb", 100))
                                core.KillMonster("battleunderb", "Enter", "Spawn", "Skeleton Warrior", log: false);
                        }
                        finally
                        {
                            core.CancelRegisteredQuests();
                        }
                    }
                },
                (targetPlayerNumber, targetPlayerName) =>
                    Army.CoordinateFollowAndCombat(
                        targetPlayerName,
                        "Skeleton Warrior",
                        compositionType: ClassType.Farm
                    )
            )
        )
            return false;

        // Step 3: 2084 ("A Loyal Follower" — Ephemerite in /timevoid)
        return RunCooperativeObjective(
            $"{objectivePrefix}_LOYAL_FOLLOWER_COMPLETE",
            IsUnlocked,
            () =>
            {
                if (IsUnlocked())
                    return;

                SetClass(ClassType.Solo);
                core.EnsureAccept(2084);
                core.HuntMonster("timevoid", "Ephemerite", "Celestial Compass");
                LeaveCombatRoomForEquipmentChange();
                core.EnsureComplete(2084);
                BankIfPresent("Spirit Orb", "Loyal Spirit Orb", "Undead Energy", "Bone Dust", "Undead Essence");
            },
            (targetPlayerNumber, targetPlayerName) =>
                Army.CoordinateFollowAndCombat(targetPlayerName, "Ephemerite")
        );
    }

    private (string Name, int ItemId) PickMineCraftingMetal(CoreBots core)
    {
        (string Name, int ItemId, int TargetQuant)[] priority =
        {
            ("Barium", (int)MineCraftingMetalsEnum.Barium, 1),
            ("Copper", (int)MineCraftingMetalsEnum.Copper, 1),
            ("Silver", (int)MineCraftingMetalsEnum.Silver, 1),
            ("Barium", (int)MineCraftingMetalsEnum.Barium, 4),
            ("Aluminum", (int)MineCraftingMetalsEnum.Aluminum, 1),
            ("Gold", (int)MineCraftingMetalsEnum.Gold, 1),
            ("Iron", (int)MineCraftingMetalsEnum.Iron, 1),
            ("Platinum", (int)MineCraftingMetalsEnum.Platinum, 1),
        };

        foreach ((string name, int itemId, int targetQuant) in priority)
        {
            if (!core.CheckInventory(itemId, targetQuant, toInv: false))
                return (name, itemId);
        }

        if (Farm.FactionRank("DoomWood") < 10)
        {
            return priority
                .DistinctBy(m => m.ItemId)
                .Select(m => (m.Name, m.ItemId, Owned: GetInventoryAndBankQuantity(m.ItemId)))
                .Where(m => m.Owned < 999)
                .OrderBy(m => m.Owned)
                .Select(m => (m.Name, m.ItemId))
                .FirstOrDefault();
        }

        return (string.Empty, 0);
    }

    private bool RunBracket50ApprovedDailies()
    {
        CoreBots core = CoreBots.Instance;

        // 8a. Unlock "A Loyal Follower" [2084] (prereq for Mine Crafting, Bright Knight Seals & Embrace Your Chaos)
        if (!EnsureLoyalFollowerUnlocked("BRACKET50"))
            return false;

        // 8b. Daily: Mine Crafting [2091] (Metals for BLoD/NSoD + 2,000 DoomWood Rep)
        (string targetMetalName, int targetMetalId) = PickMineCraftingMetal(core);
        bool mineCraftingAvailable =
            core.isCompletedBefore(2084, log: false)
            && targetMetalId > 0
            && CanRunPeriodicQuest(2091);

        if (
            !RunCoordinatedCombatStep(
                "BRACKET50_MINE_CRAFTING_DAILY",
                () => !mineCraftingAvailable,
                () => Bot.Quests.IsDailyComplete(2091),
                "Balboa",
                ClassType.Farm,
                () =>
                {
                    core.Logger($"Daily: Mine Crafting [2091] -> Target Metal: {targetMetalName} (+2,000 DoomWood Rep)");
                    UnbankIfOwned("Axe of the Prospector");
                    core.AddDrop("Axe of the Prospector", targetMetalName);
                    core.EnsureAccept(2091);
                    if (!core.CheckInventory("Axe of the Prospector"))
                        core.HuntMonster("stalagbite", "Balboa", "Axe of the Prospector", 1, isTemp: false);
                    core.HuntMonster("stalagbite", "Balboa", "Raw Ore", 30, isTemp: true);
                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureComplete(2091, targetMetalId);
                    WaitForDailyRewardPickup(2091, targetMetalName);
                    BankIfPresent(targetMetalName, "Axe of the Prospector");
                },
                targetMap: "stalagbite"
            )
        )
            return false;

        // 8c. Daily: ShadowScythe General [3828 F2P / 3827 Mem] (Lvl 50+, fixed F2P handling)
        bool HasSSG() => core.CheckInventory("ShadowScythe General", toInv: false);
        bool ssgF2PReady = Bot.Player.Level >= 50 && !HasSSG() && CanRunPeriodicQuest(3828);
        bool ssgMemReady = Bot.Player.Level >= 50 && !HasSSG() && core.IsMember && CanRunPeriodicQuest(3827);
        bool canBuySSGNow = !HasSSG() && core.CheckInventory("Shadow Shield", 100, toInv: false);

        if (
            !RunCoordinatedCombatStep(
                "BRACKET50_SHADOWSCYTHE_DAILY",
                () => HasSSG() || (!ssgF2PReady && !ssgMemReady && !canBuySSGNow),
                () =>
                    HasSSG()
                    || (
                        Bot.Quests.IsDailyComplete(3828)
                        && (!core.IsMember || Bot.Quests.IsDailyComplete(3827))
                        && !core.CheckInventory("Shadow Shield", 100, toInv: false)
                    ),
                "Citadel Crusader",
                ClassType.Farm,
                () =>
                {
                    core.Logger("Daily: ShadowScythe General [3828/3827]");
                    UnbankIfOwned("Shadow Shield");
                    core.AddDrop("Shadow Shield");

                    if (ssgF2PReady && !Bot.Quests.IsDailyComplete(3828))
                    {
                        core.EnsureAccept(3828);
                        core.HuntMonster("lightguardwar", "Citadel Crusader", "Broken Blade", 1, isTemp: true);
                        LeaveCombatRoomForEquipmentChange();
                        core.EnsureComplete(3828);
                        WaitForDailyRewardPickup(3828, "Shadow Shield");
                    }

                    if (ssgMemReady && !Bot.Quests.IsDailyComplete(3827))
                    {
                        core.EnsureAccept(3827);
                        core.HuntMonster("lightguardwar", "Citadel Crusader", "Broken Blade", 1, isTemp: true);
                        LeaveCombatRoomForEquipmentChange();
                        core.EnsureComplete(3827);
                        WaitForDailyRewardPickup(3827, "Shadow Shield");
                    }

                    if (!HasSSG() && core.CheckInventory("Shadow Shield", 100))
                    {
                        LeaveCombatRoomForEquipmentChange();
                        core.BuyItem("shadowfall", 1644, "ShadowScythe General");
                        Bot.Wait.ForItemBuy();
                    }

                    BankIfPresent("Shadow Shield");
                }
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    private bool RunBracket55ApprovedDailies()
    {
        CoreBots core = CoreBots.Instance;

        if (!EnsureLoyalFollowerUnlocked("BRACKET55"))
            return false;

        bool HasBrightKnight() => core.CheckInventory("Bright Knight", toInv: false);
        bool unlocked = core.isCompletedBefore(2084, log: false);

        // 5a. Daily: Seal of Light [3826] (ULTRA Alteon)
        bool sealLightReady =
            unlocked
            && !HasBrightKnight()
            && !core.CheckInventory("Seal of Light", 50, toInv: false)
            && CanRunPeriodicQuest(3826);

        if (
            !RunCoordinatedCombatStep(
                "BRACKET55_SEAL_OF_LIGHT_DAILY",
                () => !sealLightReady,
                () => Bot.Quests.IsDailyComplete(3826) || HasBrightKnight(),
                "ULTRA Alteon",
                ClassType.Solo,
                () =>
                {
                    core.Logger("Daily: Seal of Light [3826] (/alteonbattle ULTRA Alteon)");
                    UnbankIfOwned("Seal of Light");
                    core.AddDrop("Seal of Light");
                    core.EnsureAccept(3826);
                    core.HuntMonster("alteonbattle", "ULTRA Alteon", "Alteon Defeated", 1, isTemp: true);
                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureComplete(3826);
                    WaitForDailyRewardPickup(3826, "Seal of Light");
                },
                targetMap: "alteonbattle"
            )
        )
            return false;

        // 5b. Daily: Seal of Darkness [3825] (ULTRA Sepulchure)
        bool sealDarknessReady =
            unlocked
            && !HasBrightKnight()
            && !core.CheckInventory("Seal of Darkness", 50, toInv: false)
            && CanRunPeriodicQuest(3825);

        if (
            !RunCoordinatedCombatStep(
                "BRACKET55_SEAL_OF_DARKNESS_DAILY",
                () => !sealDarknessReady,
                () => Bot.Quests.IsDailyComplete(3825) || HasBrightKnight(),
                "ULTRA Sepulchure",
                ClassType.Solo,
                () =>
                {
                    core.Logger("Daily: Seal of Darkness [3825] (/sepulchurebattle ULTRA Sepulchure)");
                    UnbankIfOwned("Seal of Darkness");
                    core.AddDrop("Seal of Darkness");
                    core.EnsureAccept(3825);
                    core.HuntMonster("sepulchurebattle", "ULTRA Sepulchure", "Sepulchure Defeated", 1, isTemp: true);
                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureComplete(3825);
                    WaitForDailyRewardPickup(3825, "Seal of Darkness");
                },
                targetMap: "sepulchurebattle"
            )
        )
            return false;

        // 5c. Turn in Seals for Bright Knight if 50+50 seals & 1,000,000 Gold are available, else bank Seals
        bool CanBuyBrightKnightNow() =>
            !HasBrightKnight()
            && core.CheckInventory("Seal of Light", 50, toInv: false)
            && core.CheckInventory("Seal of Darkness", 50, toInv: false)
            && Bot.Player.Gold >= 1000000;

        bool hasLooseSealsInInv =
            Bot.Inventory?.Items.Any(i =>
                i?.Name != null
                && (
                    string.Equals(i.Name.Trim(), "Seal of Light", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(i.Name.Trim(), "Seal of Darkness", StringComparison.OrdinalIgnoreCase)
                )
            ) == true;

        bool buyHandled = !CanBuyBrightKnightNow() && !hasLooseSealsInInv;
        if (
            !RunCoordinatedTurnInStep(
                "BRACKET55_BRIGHT_KNIGHT_CLEANUP",
                () => buyHandled,
                () => buyHandled,
                () =>
                {
                    if (CanBuyBrightKnightNow())
                    {
                        UnbankIfOwned("Seal of Light", "Seal of Darkness");
                        core.BuyItem("alteonbattle", 574, "Bright Knight");
                        Bot.Wait.ForItemBuy();
                    }

                    BankIfPresent("Bright Knight", "Seal of Light", "Seal of Darkness");
                    buyHandled = true;
                }
            )
        )
            return false;

        return !Bot.ShouldExit;
    }

    private bool RunBracket65ApprovedDailies()
    {
        CoreBots core = CoreBots.Instance;

        if (!EnsureLoyalFollowerUnlocked("BRACKET65"))
            return false;

        // 3a. Daily: Embrace Your Chaos [3596] (/mountdoomskull b1 -> +5,000 Chaos Rep + Dage's Scroll Fragment)
        bool embraceReady =
            core.isCompletedBefore(2084, log: false)
            && !core.CheckInventory("Dage's Scroll Fragment", 13, toInv: false)
            && CanRunPeriodicQuest(3596);
        if (embraceReady && !RunDagesScrollFragmentDaily())
            return false;

        // 3b. Daily: Lord of Order Progression Chain [7156-7165] (Level 50+, Player 4 Endgame Support Class)
        return RunLordOfOrderProgression();
    }

    private int GetNextAvailableLoOQuest(CoreBots core)
    {
        for (int questId = 7156; questId <= 7165; questId++)
        {
            if (!core.isCompletedBefore(questId))
                return questId;
        }
        return 0;
    }

    private bool RunLoODay1Stories(
        CoreBots core,
        Func<bool> activeCondition,
        string objectivePrefix = "BRACKET65"
    )
    {
        if (
            !RunCoordinatedStoryChain(
                $"{objectivePrefix}_LOO_CITADEL_182",
                new[]
                {
                    new CoordinatedStoryStep(
                        182,
                        "citadel",
                        "Grand Inquisitor",
                        c =>
                        {
                            c.EnsureAccept(182);
                            c.HuntMonster("Citadel", "Grand Inquisitor", "Ring of Evil Intent");
                            c.EnsureComplete(182);
                        },
                        ClassType.Solo
                    ),
                },
                activeCondition
            )
        )
            return false;

        // Polish's Quests (/citadelruins 6172-6182)
        if (
            !RunCoordinatedStoryChain(
                $"{objectivePrefix}_LOO_CITADELRUINS",
                new[]
                {
                    new CoordinatedStoryStep(
                        6172,
                        "citadelruins",
                        "Mana Sprites",
                        c =>
                        {
                            c.EnsureAccept(6172);
                            c.GetMapItem(5592, 1, "citadelruins");
                            c.HuntMonster("citadelruins", "Mana Sprites", "Siphoned Mana", 8);
                            c.EnsureComplete(6172);
                        }
                    ),
                    new CoordinatedStoryStep(6173, "citadelruins", "*", c => Story.MapItemQuest(6173, "citadelruins", 5602), isCombat: false),
                    new CoordinatedStoryStep(
                        6174,
                        "citadelruins",
                        "Inquisitor Hobo",
                        c =>
                        {
                            c.EnsureAccept(6174);
                            c.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitor Hobos Defeated", 8);
                            c.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitors' Manifesto", 5);
                            c.EnsureComplete(6174);
                        }
                    ),
                    new CoordinatedStoryStep(
                        6175,
                        "citadelruins",
                        "Inquisitor Hobo",
                        c =>
                        {
                            c.EnsureAccept(6175);
                            c.GetMapItem(5593, 5, "citadelruins");
                            c.HuntMonster("citadelruins", "Inquisitor Hobo", "Clue Found", 1);
                            c.EnsureComplete(6175);
                            c.Sleep(2500);
                        }
                    ),
                    new CoordinatedStoryStep(
                        6176,
                        "citadelruins",
                        "Inquisitor Heavy",
                        c =>
                        {
                            c.EnsureAccept(6176);
                            c.HuntMonster("citadelruins", "Inquisitor Heavy", "Heavies Beat Down", 2);
                            c.HuntMonster("citadelruins", "Inquisitor Heavy", "Citadel Key");
                            Story.MapItemQuest(6176, "citadelruins", 5603);
                        }
                    ),
                    new CoordinatedStoryStep(
                        6177,
                        "citadelruins",
                        "Inquisitor Hobo",
                        c =>
                        {
                            c.EnsureAccept(6177);
                            c.GetMapItems(new[] { (5594, 1), (5595, 1), (5596, 1) }, "citadelruins");
                            c.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitors Defeated", 6);
                            c.EnsureComplete(6177);
                        }
                    ),
                    new CoordinatedStoryStep(
                        6178,
                        "citadelruins",
                        "Mana Sprites",
                        c =>
                        {
                            c.EnsureAccept(6178);
                            c.GetMapItem(5597, 1, "citadelruins");
                            c.HuntMonster("citadelruins", "Mana Sprites", "Siphoned Mana", 8);
                            c.EnsureComplete(6178);
                        }
                    ),
                    new CoordinatedStoryStep(6179, "citadelruins", "*", c => Story.MapItemQuest(6179, "citadelruins", 5598), isCombat: false),
                    new CoordinatedStoryStep(
                        6180,
                        "citadelruins",
                        "Inquisitor Hobo",
                        c =>
                        {
                            c.EnsureAccept(6180);
                            c.GetMapItems(new[] { (5599, 1), (5600, 1), (5601, 1) }, "citadelruins");
                            c.HuntMonster("citadelruins", "Inquisitor Hobo", "Missing Parts", 6);
                            c.EnsureComplete(6180);
                        }
                    ),
                    new CoordinatedStoryStep(6181, "citadelruins", "Grand Inquisitor Murry", c => Story.KillQuest(6181, "citadelruins", "Grand Inquisitor Murry"), ClassType.Solo),
                    new CoordinatedStoryStep(6182, "citadelruins", "Enn'tröpy", c => Story.KillQuest(6182, "citadelruins", "Enn'tröpy"), ClassType.Solo),
                },
                activeCondition,
                "Mage's Gratitude"
            )
        )
            return false;

        // Ravenscar Storyline (/ravenscar 2901-2907)
        return RunCoordinatedStoryChain(
            $"{objectivePrefix}_LOO_RAVENSCAR",
            new[]
            {
                new CoordinatedStoryStep(2901, "ravenscar", "*", c => Story.MapItemQuest(2901, "ravenscar", 1767), isCombat: false),
                new CoordinatedStoryStep(
                    2902,
                    "ravenscar",
                    "Restless Undead",
                    c =>
                    {
                        c.EnsureAccept(2902);
                        c.HuntMonster("ravenscar", "Restless Undead", "Crumpled Book Page", 1);
                        c.HuntMonster("ravenscar", "Restless Undead", "Undead Slain", 6);
                        c.EnsureComplete(2902);
                    }
                ),
                new CoordinatedStoryStep(2903, "ravenscar", "*", c => Story.MapItemQuest(2903, "ravenscar", 1768), isCombat: false),
                new CoordinatedStoryStep(2904, "ravenscar", "Restless Undead", c => Story.KillQuest(2904, "ravenscar", "Restless Undead")),
                new CoordinatedStoryStep(2905, "ravenscar", "Undead Soldier", c => Story.KillQuest(2905, "ravenscar", "Undead Soldier")),
                new CoordinatedStoryStep(2906, "ravenscar", "Shadowman", c => Story.KillQuest(2906, "ravenscar", "Shadowman"), ClassType.Solo),
                new CoordinatedStoryStep(2907, "ravenscar", "*", c => Story.MapItemQuest(2907, "ravenscar", 1769), isCombat: false),
            },
            activeCondition
        );
    }

    private bool RunLoODay5DFOStories(
        Func<bool> activeCondition,
        string objectivePrefix = "BRACKET65"
    )
    {
        return RunCoordinatedStoryChain(
            $"{objectivePrefix}_LOO_DFO",
            new[]
            {
                new CoordinatedStoryStep(6294, "firewar", "Fire Dragon", c => Story.KillQuest(6294, "firewar", "Fire Dragon")),
                new CoordinatedStoryStep(6295, "firewar", "Fire Dragon", c => Story.KillQuest(6295, "firewar", "Fire Dragon")),
                new CoordinatedStoryStep(
                    6296,
                    "firewar",
                    "Inferno Dragon",
                    c =>
                    {
                        c.EnsureAccept(6296);
                        c.KillMonster("firewar", "r8", "Right", "Inferno Dragon", "Pyritium Shards", 5);
                        c.EnsureComplete(6296);
                    }
                ),
                new CoordinatedStoryStep(
                    6297,
                    "firewar",
                    "Inferno Dragon",
                    c =>
                    {
                        c.EnsureAccept(6297);
                        c.KillMonster("firewar", "r8", "Right", "Inferno Dragon", "Perfect Pyritium");
                        c.EnsureComplete(6297);
                    }
                ),
                new CoordinatedStoryStep(6298, "firewar", "Uriax", c => Story.KillQuest(6298, "firewar", "Uriax"), ClassType.Solo),
                new CoordinatedStoryStep(
                    6299,
                    "firewar",
                    "*",
                    c =>
                    {
                        c.EnsureAccept(6299);
                        c.Join("firewar", "r10", "Left");
                        if (Bot.Player.Cell != "r10")
                            c.Jump("r10", "Left");
                        while (!Bot.ShouldExit && !Bot.TempInv.Contains(43572))
                        {
                            Bot.Combat.Attack("*");
                            Bot.Sleep(500);
                        }
                        c.EnsureComplete(6299);
                        c.JumpWait();
                    },
                    ClassType.Solo
                ),
                new CoordinatedStoryStep(
                    6301,
                    "northmountain",
                    "Ice Elemental",
                    c =>
                    {
                        c.EnsureAccept(6301);
                        c.HuntMonster("northmountain", "Ice Elemental", "Monsters Defeated", 8);
                        c.GetMapItem(5812, 1, "northmountain");
                        c.EnsureComplete(6301);
                    }
                ),
                new CoordinatedStoryStep(6302, "northmountain", "Ice Elemental", c => Story.KillQuest(6302, "northmountain", "Ice Elemental")),
                new CoordinatedStoryStep(6303, "northmountain", "Ursice Savage", c => Story.KillQuest(6303, "northmountain", "Ursice Savage")),
                new CoordinatedStoryStep(6304, "northmountain", "Ice Spitter", c => Story.KillQuest(6304, "northmountain", "Ice Spitter")),
                new CoordinatedStoryStep(6305, "northmountain", "Ice Elemental", c => Story.KillQuest(6305, "northmountain", "Ice Elemental")),
                new CoordinatedStoryStep(
                    6306,
                    "northmountain",
                    "*",
                    c =>
                    {
                        Story.MapItemQuest(6306, "northmountain", 5813, 4);
                        Story.MapItemQuest(6306, "northmountain", 5814);
                    },
                    isCombat: false
                ),
                new CoordinatedStoryStep(6307, "northmountain", "Izotz", c => Story.KillQuest(6307, "northmountain", "Izotz"), ClassType.Solo),
                new CoordinatedStoryStep(6308, "charredplains", "Fire Dragon", c => Story.KillQuest(6308, "charredplains", "Fire Dragon")),
                new CoordinatedStoryStep(6309, "charredplains", "*", c => Story.MapItemQuest(6309, "charredplains", 5815), isCombat: false),
                new CoordinatedStoryStep(6310, "charredplains", "Akriloth", c => Story.KillQuest(6310, "charredplains", "Akriloth"), ClassType.Solo),
                new CoordinatedStoryStep(6311, "northmountain", "Izotz", c => Story.KillQuest(6311, "northmountain", "Izotz"), ClassType.Solo),
                new CoordinatedStoryStep(6312, "drakonnan", "*", c => Story.MapItemQuest(6312, "drakonnan", 5827, 5), isCombat: false),
                new CoordinatedStoryStep(6313, "drakonnan", "Fire Dragon", c => Story.KillQuest(6313, "drakonnan", "Fire Dragon")),
                new CoordinatedStoryStep(6314, "drakonnan", "Living Lava", c => Story.KillQuest(6314, "drakonnan", "Living Lava")),
                new CoordinatedStoryStep(6315, "drakonnan", "Fire Dragon", c => Story.KillQuest(6315, "drakonnan", "Fire Dragon")),
                new CoordinatedStoryStep(6316, "northmountain", "Ice Elemental", c => Story.KillQuest(6316, "northmountain", "Ice Elemental")),
                new CoordinatedStoryStep(6317, "northmountain", "*", c => Story.MapItemQuest(6317, "northmountain", 5826, 6), isCombat: false),
                new CoordinatedStoryStep(6318, "drakonnan", "Fire Elemental", c => Story.KillQuest(6318, "drakonnan", "Fire Elemental")),
                new CoordinatedStoryStep(6319, "drakonnan", "Living Fire", c => Story.KillQuest(6319, "drakonnan", "Living Fire")),
            },
            activeCondition,
            "Ice Katana"
        );
    }

    private bool CompleteLoODayIfReady(
        CoreBots core,
        int questId,
        int nextLoOQuest,
        string[] requiredItems,
        int chooseItemId = -1,
        string objectivePrefix = "BRACKET65"
    )
    {
        bool completeAttempted = false;
        return RunCoordinatedTurnInStep(
            $"{objectivePrefix}_LOO_{questId}_COMPLETE",
            () => completeAttempted || nextLoOQuest != questId || core.isCompletedBefore(questId, log: false) || !Bot.Quests.IsAvailable(questId),
            () => completeAttempted || nextLoOQuest != questId || core.isCompletedBefore(questId, log: false),
            () =>
            {
                completeAttempted = true;
                UnbankIfOwned(requiredItems);
                if (!requiredItems.All(item => core.CheckInventory(item)))
                {
                    core.Logger($"LoO [{questId}] missing one or more required items; will retry on a future daily run.");
                    return;
                }

                LeaveCombatRoomForEquipmentChange();
                core.EnsureAccept(questId);
                core.AddDrop(core.QuestRewards(questId));
                if (chooseItemId > 0)
                {
                    Bot.Drops.Add(chooseItemId);
                    core.EnsureComplete(questId, chooseItemId);
                    Bot.Wait.ForPickup(chooseItemId);
                    BankQuestExtras(questId, "Lord of Order");
                }
                else
                {
                    core.EnsureComplete(questId);
                    BankQuestExtras(questId);
                }
            }
        );
    }

    private bool RunLoOQuestMultiHunt(
        CoreBots core,
        int questId,
        int nextLoOQuest,
        (string Map, string Monster, string Item, int Quant, ClassType Role, string? Cell, string? Pad, Action<CoreBots>? BeforeHunt)[] targets,
        int softRetryLimit = 2,
        string objectivePrefix = "BRACKET65"
    )
    {
        bool IsActiveOnThisQuest() => nextLoOQuest == questId && !core.isCompletedBefore(questId, log: false);

        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if (
                !RunCoordinatedHuntStep(
                    $"{objectivePrefix}_LOO_{questId}_STEP_{i + 1}",
                    questId,
                    t.Map,
                    t.Monster,
                    t.Item,
                    t.Quant,
                    isTemp: false,
                    role: t.Role,
                    cell: t.Cell,
                    pad: t.Pad,
                    acceptQuest: true,
                    checkBankAlso: true,
                    softRetryLimit: softRetryLimit,
                    beforeHunt: t.BeforeHunt,
                    isEnabled: IsActiveOnThisQuest
                )
            )
            {
                return false;
            }
        }

        return CompleteLoODayIfReady(
            core,
            questId,
            nextLoOQuest,
            targets.Select(t => t.Item).ToArray(),
            objectivePrefix: objectivePrefix
        );
    }

    private bool RunLordOfOrderProgression(string objectivePrefix = "BRACKET65")
    {
        CoreBots core = CoreBots.Instance;
        bool HasLoOClass() => HasClassInInventory(50741) || HasClassInInventory("Lord of Order");

        bool eligible = Bot.Player.Level >= 50 && !HasLoOClass();
        int nextQuestId = eligible ? GetNextAvailableLoOQuest(core) : 0;
        if (nextQuestId > 0)
            core.Logger($"Daily: Lord of Order [{nextQuestId}/7165] active for this account.");

        if (
            Army.AllPlayersSatisfy(
                $"{objectivePrefix}_LOO_ALL_DONE",
                () => nextQuestId == 0 && (!HasLoOClass() || HasRank10Class("Lord of Order"))
            )
        )
            return true;

        // Always keep barriers aligned across all accounts, skipping work instantly when nextQuestId != day.
        // Day 1 [7156]: Heart of Servitude
        if (nextQuestId == 7156)
            core.AddDrop(core.QuestRewards(7156));

        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7156_SKIP", () => nextQuestId != 7156)
            && (
                !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7156_GOLD",
                () => nextQuestId != 7156 || core.CheckInventory("Dreadrock Donation Receipt", 1, toInv: false) || Bot.Player.Gold >= 1000000,
                () => nextQuestId != 7156 || core.CheckInventory("Dreadrock Donation Receipt", 1, toInv: false) || Bot.Player.Gold >= 1000000,
                "*",
                ClassType.Farm,
                () =>
                {
                    if (Bot.Player.Gold < 1000000)
                    {
                        Farm.Gold(1000000);
                        _syncedClassType = ClassType.None;
                        SetClass();
                    }
                }
            )
            || !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_LOO_7156_SHOPS",
                () => nextQuestId != 7156 || core.CheckInventory("Dreadrock Donation Receipt", 1, toInv: false),
                () => nextQuestId != 7156 || core.CheckInventory("Dreadrock Donation Receipt", 1, toInv: false),
                () =>
                {
                    if (!core.CheckInventory("Dreadrock Donation Receipt", 1, toInv: false) && Bot.Player.Gold >= 1000000)
                        core.BuyItem("dreadrock", 1221, "Dreadrock Donation Receipt");
                }
            )
            || !RunLoODay1Stories(core, () => nextQuestId == 7156, objectivePrefix)
            || !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7156_MAGES_GRATITUDE",
                () => nextQuestId != 7156 || core.CheckInventory("Mage's Gratitude", 1, toInv: false),
                () => nextQuestId != 7156 || core.CheckInventory("Mage's Gratitude", 1, toInv: false),
                "Enn'tröpy",
                ClassType.Solo,
                () =>
                {
                    UnbankIfOwned("Mage's Gratitude");
                    if (!core.CheckInventory("Mage's Gratitude"))
                    {
                        core.AddDrop("Mage's Gratitude");
                        core.EnsureAccept(6182);
                        core.HuntMonster("citadelruins", "Enn'tröpy", "Enn'tröpy Defeated", isTemp: true);
                        LeaveCombatRoomForEquipmentChange();
                        core.EnsureComplete(6182);
                        Bot.Wait.ForPickup("Mage's Gratitude");
                    }
                },
                targetMap: "citadelruins"
            )
            || !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_LOO_7156_RAVENSCAR_TRUTH",
                () => nextQuestId != 7156 || core.CheckInventory("Ravenscar's Truth", 1, toInv: false),
                () => nextQuestId != 7156 || core.CheckInventory("Ravenscar's Truth", 1, toInv: false),
                () =>
                {
                    UnbankIfOwned("Ravenscar's Truth");
                    if (!core.CheckInventory("Ravenscar's Truth"))
                        core.BuyItem("ravenscar", 614, "Ravenscar's Truth");
                }
            )
            || !RunCoordinatedHuntStep(
                $"{objectivePrefix}_LOO_7156_WATCHTOWER",
                7156,
                "watchtower",
                "Chaorrupted Knight",
                "Pristine Blades of Order",
                1,
                role: ClassType.Solo,
                acceptQuest: true,
                isEnabled: () => nextQuestId == 7156
            )
            || !RunCoordinatedHuntStep(
                $"{objectivePrefix}_LOO_7156_DEADMOOR",
                7156,
                "deadmoor",
                "Banshee Mallora",
                "Deadmoor Spirits Helped",
                1,
                role: ClassType.Solo,
                acceptQuest: true,
                isEnabled: () => nextQuestId == 7156
            )
            || !CompleteLoODayIfReady(
                core,
                7156,
                nextQuestId,
                new[]
                {
                    "Pristine Blades of Order",
                    "Dreadrock Donation Receipt",
                    "Deadmoor Spirits Helped",
                    "Mage's Gratitude",
                    "Ravenscar's Truth",
                },
                objectivePrefix: objectivePrefix
            )
            )
        )
            return false;

        // Day 2 [7157]: Spirit of Justice
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7157_SKIP", () => nextQuestId != 7157)
            && !RunLoOQuestMultiHunt(
                core,
                7157,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("dwarfprison", "Warden Elfis", "Warden Elfis Detained", 1, ClassType.Solo, null, null, null),
                    ("prison", "Piggy Drake", "Piggy Drake Punished", 1, ClassType.Solo, null, null, null),
                    ("mysteriousdungeon", "Mysterious Stranger", "Mysterious Stranger Foiled", 1, ClassType.Solo, null, null, null),
                    ("dreammaster", "Calico Cobby", "Calico Cobby Crushed", 1, ClassType.Solo, null, null, null),
                },
                objectivePrefix: objectivePrefix
            )
        )
            return false;

        // Day 3 [7158]: Purification of Chaos
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7158_SKIP", () => nextQuestId != 7158)
            && (
                !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7158_GOLD",
                () => nextQuestId != 7158 || core.CheckInventory("Chaoroot", 15, toInv: false) || Bot.Player.Gold >= 500000,
                () => nextQuestId != 7158 || core.CheckInventory("Chaoroot", 15, toInv: false) || Bot.Player.Gold >= 500000,
                "*",
                ClassType.Farm,
                () =>
                {
                    if (Bot.Player.Gold < 500000)
                    {
                        Farm.Gold(500000);
                        _syncedClassType = ClassType.None;
                        SetClass();
                    }
                }
            )
            || !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_LOO_7158_CHAOROOT",
                () => nextQuestId != 7158 || core.CheckInventory("Chaoroot", 15, toInv: false),
                () => nextQuestId != 7158 || core.CheckInventory("Chaoroot", 15, toInv: false),
                () =>
                {
                    UnbankIfOwned("Chaoroot");
                    if (!core.CheckInventory("Chaoroot", 15) && Bot.Player.Gold >= 500000)
                        Adv.BuyItem("tercessuinotlim", 1951, "Chaoroot", 15);
                }
            )
            || !RunLoOQuestMultiHunt(
                core,
                7158,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("chaosboss", "Ultra Chaos Warlord", "Chaotic War Essence", 15, ClassType.Solo, null, null, null),
                    ("shadowgates", "Chaorruption", "Chaorrupting Particles", 15, ClassType.Solo, null, null, null),
                    ("stormtemple", "Chaos Lord Lionfang", "Purified Raindrop", 45, ClassType.Solo, null, null, _ => Bot.Quests.UpdateQuest(2814)),
                },
                objectivePrefix: objectivePrefix
            )
            )
        )
            return false;

        // Day 4 [7159]: Steadfast Will
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7159_SKIP", () => nextQuestId != 7159)
            && !RunLoOQuestMultiHunt(
                core,
                7159,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("gaiazor", "Gaiazor", "Gaiazor's Cornerstone", 1, ClassType.Solo, null, null, null),
                    ("treetitanbattle", "Dakka the Dire Dragon", "Dakka's Crystal", 1, ClassType.Solo, null, null, _ => { Bot.Quests.UpdateQuest(4361); Bot.Sleep(1500); }),
                    ("andre", "Giant Necklace", "Andre's Necklace Fragment", 1, ClassType.Solo, null, null, null),
                    ("desolich", "Desolich", "Desolich's Skull", 1, ClassType.Solo, null, null, _ => LeaveCombatRoomForEquipmentChange()),
                },
                objectivePrefix: objectivePrefix
            )
        )
            return false;

        // Day 5 [7160]: Strike of Order
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7160_SKIP", () => nextQuestId != 7160)
            && (
                !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7160_KITSUNE",
                () => nextQuestId != 7160 || core.CheckInventory("Hanzamune Dragon Koi Blade", 1, toInv: false),
                () => nextQuestId != 7160 || core.CheckInventory("Hanzamune Dragon Koi Blade", 1, toInv: false),
                "*",
                ClassType.Solo,
                () =>
                {
                    core.AddDrop("Hanzamune Dragon Koi Blade");
                    core.EnsureAccept(7160);
                    core.KillKitsune("Hanzamune Dragon Koi Blade");
                },
                targetMap: "kitsune"
            )
            || !RunCoordinatedHuntStep($"{objectivePrefix}_LOO_7160_LEDGERMAYNE", 7160, "ledgermayne", "Ledgermayne", "The Supreme Arcane Staff", 1, role: ClassType.Solo, isEnabled: () => nextQuestId == 7160)
            || !RunCoordinatedHuntStep($"{objectivePrefix}_LOO_7160_DRAGONOID", 7160, "mqlesson", "Dragonoid", "Dragonoid of Hours", 1, role: ClassType.Solo, isEnabled: () => nextQuestId == 7160)
            || !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_LOO_7160_SAFIRIA",
                () => nextQuestId != 7160 || core.CheckInventory("Safiria's Spirit Orb", 1, toInv: false),
                () => nextQuestId != 7160 || core.CheckInventory("Safiria's Spirit Orb", 1, toInv: false),
                () =>
                {
                    UnbankIfOwned("Safiria's Spirit Orb");
                    if (!core.CheckInventory("Safiria's Spirit Orb"))
                    {
                        core.AddDrop("Safiria's Spirit Orb");
                        core.EnsureAccept(7160);
                        core.GetMapItem(5470, 1, "maxius");
                        Bot.Wait.ForPickup("Safiria's Spirit Orb");
                    }
                }
            )
            || !RunLoODay5DFOStories(() => nextQuestId == 7160, objectivePrefix)
            || !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7160_ICE_KATANA",
                () => nextQuestId != 7160 || core.CheckInventory("Ice Katana", 1, toInv: false),
                () => nextQuestId != 7160 || core.CheckInventory("Ice Katana", 1, toInv: false),
                "Living Fire",
                ClassType.Farm,
                () =>
                {
                    UnbankIfOwned("Ice Katana");
                    if (!core.CheckInventory("Ice Katana"))
                    {
                        core.AddDrop("Ice Katana");
                        core.EnsureAccept(6319);
                        core.HuntMonster("drakonnan", "Living Fire", "Inferno Heart");
                        LeaveCombatRoomForEquipmentChange();
                        core.EnsureComplete(6319);
                        Bot.Wait.ForPickup("Ice Katana");
                    }
                },
                targetMap: "drakonnan"
            )
            || !CompleteLoODayIfReady(
                core,
                7160,
                nextQuestId,
                new[]
                {
                    "Hanzamune Dragon Koi Blade",
                    "The Supreme Arcane Staff",
                    "Dragonoid of Hours",
                    "Safiria's Spirit Orb",
                    "Ice Katana",
                },
                objectivePrefix: objectivePrefix
            )
            )
        )
            return false;

        // Day 6 [7161]: Harmony
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7161_SKIP", () => nextQuestId != 7161)
            && !RunLoOQuestMultiHunt(
                core,
                7161,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("elemental", "Tree of Destiny", "Unity of Life", 1, ClassType.Solo, null, null, null),
                    ("orchestra", "Faust", "Harmony of Solace", 1, ClassType.Solo, null, null, null),
                    ("cathedral", "Pactagonal Knight", "Teamwork Observed", 100, ClassType.Farm, null, null, null),
                    ("goose", "Queen's ArchSage", "Scroll of Enchantment", 1, ClassType.Solo, null, null, null),
                },
                objectivePrefix: objectivePrefix
            )
        )
            return false;

        // Day 7 [7162]: Ordinance
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7162_SKIP", () => nextQuestId != 7162)
            && !RunLoOQuestMultiHunt(
                core,
                7162,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("newfinale", "Alliance Healer", "Acolyte's Braille", 1, ClassType.Farm, null, null, null),
                    ("wardwarf", "Drow Assassin", "Suppressed Drows", 50, ClassType.Farm, null, null, null),
                    ("warundead", "Skeletal Fire Mage", "Suppressed Undead", 50, ClassType.Farm, null, null, null),
                    ("warhorc", "Horc Warrior", "Suppressed Horcs", 50, ClassType.Farm, null, null, null),
                    ("weaverwar", "Weaver Queen's Hound", "Suppressed Weavers", 50, ClassType.Farm, null, null, null),
                    ("extriki", "Extriki", "Strength of Resilience", 1, ClassType.Solo, null, null, null),
                },
                objectivePrefix: objectivePrefix
            )
        )
            return false;

        // Day 8 [7163]: Axiom
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7163_SKIP", () => nextQuestId != 7163)
            && !RunLoOQuestMultiHunt(
                core,
                7163,
                nextQuestId,
                new (string, string, string, int, ClassType, string?, string?, Action<CoreBots>?)[]
                {
                    ("elfhame", "Guardian Spirit", "Law of Nature", 1, ClassType.Solo, null, null, null),
                    ("deepchaos", "Kathool", "Law of Time", 1, ClassType.Solo, null, null, null),
                    ("necrocavern", "ShadowStone Support", "Law of Gravity", 1, ClassType.Solo, null, null, null),
                    ("blackholesun", "Reflecteract", "Law of Relativity", 1, ClassType.Solo, null, null, null),
                    ("thunderfang", "Tonitru", "Law of Conservation of Energy", 1, ClassType.Solo, null, null, null),
                    ("lair", "Red Dragon", "Law of Low Drop Rates", 100, ClassType.Solo, null, null, null),
                },
                objectivePrefix: objectivePrefix
            )
        )
            return false;

        // Day 9 [7164]: Blessing of Order
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7164_SKIP", () => nextQuestId != 7164)
            && (
                !RunCoordinatedHuntStep(
                $"{objectivePrefix}_LOO_7164_RAXGORE",
                7164,
                "doomvaultb",
                "Undead Raxgore",
                "Weapon Imprint",
                15,
                role: ClassType.Solo,
                cell: "r26",
                pad: "Left",
                isEnabled: () => nextQuestId == 7164
            )
            || !RunCoordinatedTurnInStep(
                $"{objectivePrefix}_LOO_7164_LURE",
                () => nextQuestId != 7164 || core.CheckInventory("Lure of Order", 1, toInv: false),
                () => nextQuestId != 7164 || core.CheckInventory("Lure of Order", 1, toInv: false),
                () =>
                {
                    UnbankIfOwned("Lure of Order");
                    if (!core.CheckInventory("Lure of Order"))
                    {
                        Farm.FishingREP(7);
                        LeaveCombatRoomForEquipmentChange();
                        core.BuyItem("greenguardwest", 363, "Lure of Order");
                    }
                }
            )
            || !RunCoordinatedCombatStep(
                $"{objectivePrefix}_LOO_7164_XIANG",
                () => nextQuestId != 7164 || core.CheckInventory("Quixotic Mana Essence", 10, toInv: false),
                () => nextQuestId != 7164 || core.CheckInventory("Quixotic Mana Essence", 10, toInv: false),
                "*",
                ClassType.Solo,
                () =>
                {
                    UnbankIfOwned("Quixotic Mana Essence");
                    core.AddDrop("Quixotic Mana Essence");
                    core.EnsureAccept(7164);
                    bool enhAfterRestore = !ShouldPreserveEquippedGear();
                    Adv.GearStore(EnhAfter: enhAfterRestore);
                    core.KillXiang("Quixotic Mana Essence", 10, true);
                    LeaveCombatRoomForEquipmentChange();
                    Adv.GearStore(true, EnhAfter: enhAfterRestore);
                    _syncedClassType = ClassType.None;
                    SetClass();
                },
                targetMap: "mirrorportal"
            )
            || !RunCoordinatedHuntStep($"{objectivePrefix}_LOO_7164_SEREPTHYS", 7164, "yasaris", "Serepthys", "Inversion Infusion", 5, role: ClassType.Solo, isEnabled: () => nextQuestId == 7164)
            || !CompleteLoODayIfReady(
                core,
                7164,
                nextQuestId,
                new[] { "Weapon Imprint", "Lure of Order", "Quixotic Mana Essence", "Inversion Infusion" },
                objectivePrefix: objectivePrefix
            )
            )
        )
            return false;

        // Day 10 [7165]: The Final Challenge -> Unlock Lord of Order + Rank 10 for Player 4
        if (
            !Army.AllPlayersSatisfy($"{objectivePrefix}_LOO_DAY_7165_SKIP", () => nextQuestId != 7165)
            && (
                !RunCoordinatedHuntStep(
                $"{objectivePrefix}_LOO_7165_DRAKATH",
                7165,
                "ultradrakath",
                "Champion of Chaos",
                "Champion of Chaos Confronted",
                1,
                role: ClassType.Solo,
                isEnabled: () => nextQuestId == 7165
            )
            || !CompleteLoODayIfReady(
                core,
                7165,
                nextQuestId,
                new[] { "Champion of Chaos Confronted" },
                chooseItemId: 50741,
                objectivePrefix: objectivePrefix
            )
            )
        )
            return false;

        return RunCooperativeObjective(
            $"{objectivePrefix}_LOO_RANK_10",
            () => !HasLoOClass() || HasRank10Class("Lord of Order"),
            () =>
            {
                if (!HasLoOClass())
                    return;

                if (!HasRank10Class("Lord of Order"))
                    RankUpClassForArmy("Lord of Order");

                SetClass();
            },
            (targetPlayerNumber, targetPlayerName) =>
                Army.CoordinateFollowAndCombat(targetPlayerName, "*")
        );
    }

    /// <summary>
    /// Future Bracket (Level 75+): Daily "The Encroaching Shadows" [8653] (Void Aura x50 + (Necro) Scroll of Dark Arts).
    /// Intentionally NOT invoked during Level 1-65 brackets; ready for future Level 75+ integration.
    /// </summary>
    public bool RunTheEncroachingShadowsDaily()
    {
        CoreBots core = CoreBots.Instance;
        bool voidAuraCapped = core.CheckInventory("Void Aura", 7500, toInv: false);
        bool dailyAvailable = Bot.Player.Level >= 75 && !voidAuraCapped && CanRunPeriodicQuest(8653);

        if (
            !RunCoordinatedHuntStep(
                "FUTURE75_ENCROACHING_SHADOWS_ICEWING",
                8653,
                "icewing",
                "Warlord Icewing",
                "Glacial Pinion",
                1,
                role: ClassType.Solo,
                acceptQuest: true,
                checkBankAlso: true,
                beforeHunt: c =>
                {
                    if (!dailyAvailable)
                        return;
                    c.AddDrop("Void Aura", "(Necro) Scroll of Dark Arts", "Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets");
                },
                isEnabled: () => dailyAvailable
            )
            || !RunCoordinatedHuntStep(
                "FUTURE75_ENCROACHING_SHADOWS_HYDRA90",
                8653,
                "hydrachallenge",
                "Hydra Head 90",
                "Hydra Eyeball",
                3,
                role: ClassType.Solo,
                cell: "h90",
                pad: "Left",
                isEnabled: () => dailyAvailable
            )
            || !RunCoordinatedHuntStep(
                "FUTURE75_ENCROACHING_SHADOWS_FLIBBI",
                8653,
                "voidflibbi",
                "Flibbitiestgibbet",
                "Flibbitigiblets",
                1,
                role: ClassType.Solo,
                isEnabled: () => dailyAvailable
            )
        )
            return false;

        bool turnInDone = false;
        return RunCoordinatedTurnInStep(
            "FUTURE75_ENCROACHING_SHADOWS_COMPLETE",
            () => !dailyAvailable || turnInDone,
            () => !dailyAvailable || Bot.Quests.IsDailyComplete(8653) || turnInDone,
            () =>
            {
                turnInDone = true;
                UnbankIfOwned("Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets");
                if (
                    !core.CheckInventory("Glacial Pinion", 1)
                    || !core.CheckInventory("Hydra Eyeball", 3)
                    || !core.CheckInventory("Flibbitigiblets", 1)
                )
                    return;

                core.AddDrop("Void Aura", "(Necro) Scroll of Dark Arts");
                core.EnsureAccept(8653);
                core.EnsureComplete(8653);
                WaitForDailyRewardPickup(8653, "Void Aura");
                BankIfPresent("Void Aura", "(Necro) Scroll of Dark Arts");
            }
        );
    }

    /// <summary>
    /// Level 80+ Daily: "Dage's Scroll Fragment" [3596] (prerequisite for Drakath the Eternal / Hero's Valiance in Stage 4A).
    /// </summary>
    public bool RunDagesScrollFragmentDaily()
    {
        CoreBots core = CoreBots.Instance;
        bool hasEnoughFragments = core.CheckInventory("Dage's Scroll Fragment", 13, toInv: false);
        bool dailyAvailable = Bot.Player.Level >= 75 && !hasEnoughFragments && CanRunPeriodicQuest(3596);
        bool dailyDone = !dailyAvailable;

        return RunCoordinatedCombatStep(
            "STEP3B_DAGES_SCROLL_FRAGMENT",
            () => dailyDone,
            () => dailyDone,
            "*",
            ClassType.Farm,
            () =>
            {
                if (dailyDone)
                    return;
                Daily.DagesScrollFragment(ToBank: true);
                dailyDone = true;
            },
            targetMap: "mountdoomskull"
        );
    }
    #endregion Approved Dailies, Weeklies & Future NSoD Daily

    #region Farmer Joe Optional Cosmetics & Cleanup
    private static readonly string[] StarterClassesToSell =
    {
        "Warrior",
        "Mage",
        "Rogue",
        "Healer",
    };

    private string ResolveSelectedOutfitPetName()
    {
        PetChoice choice = Bot.Config?.Get<PetChoice>("PetChoice") ?? PetChoice.None;
        return choice switch
        {
            PetChoice.HotMama => "Hot Mama",
            PetChoice.Akriloth => "Akriloth Pet",
            _ => string.Empty,
        };
    }

    private List<string> GetSellableStarterClassNames()
    {
        string currentClass = Bot.Player?.CurrentClass?.Name?.Trim() ?? string.Empty;
        string activeBossClass = ResolveClass(GetRoleBossPool());
        string activeFarmClass = ResolveClass(GetRoleFarmPool());

        var ownedByName = new Dictionary<string, InventoryItem>(StringComparer.OrdinalIgnoreCase);
        foreach (InventoryItem item in EnumerateOwnedInventoryAndBankItems())
        {
            if (
                item.Category == ItemCategory.Class
                && !string.IsNullOrWhiteSpace(item.Name)
                && !ownedByName.ContainsKey(item.Name.Trim())
            )
            {
                ownedByName[item.Name.Trim()] = item;
            }
        }

        CoreBots core = CoreBots.Instance;
        bool keepHealerFallback =
            !core.isCompletedBefore(8825, log: false)
            || !core.isCompletedBefore(3189, log: false)
            || !core.isCompletedBefore(7164, log: false)
            || !core.isCompletedBefore(3525, log: false)
            || (
                !ownedByName.ContainsKey("Lord of Order")
                && !ownedByName.ContainsKey("ArchPaladin")
                && !ownedByName.ContainsKey("StoneCrusher")
                && !ownedByName.ContainsKey("Oracle")
            );

        var result = new List<string>();
        foreach (string starterClass in StarterClassesToSell)
        {
            if (!ownedByName.TryGetValue(starterClass, out InventoryItem? item))
                continue;

            // Keep AC/0-AC classes, currently equipped class, and any active role or support fallback class.
            if (item.Coins || item.Equipped)
                continue;

            if (
                (string.Equals(starterClass, "Rogue", StringComparison.OrdinalIgnoreCase) && !core.isCompletedBefore(8824, log: false))
                || (string.Equals(starterClass, "Warrior", StringComparison.OrdinalIgnoreCase) && !core.isCompletedBefore(8826, log: false))
                || (string.Equals(starterClass, "Mage", StringComparison.OrdinalIgnoreCase) && !core.isCompletedBefore(8827, log: false))
            )
            {
                continue;
            }

            if (
                string.Equals(starterClass, currentClass, StringComparison.OrdinalIgnoreCase)
                || string.Equals(starterClass, activeBossClass, StringComparison.OrdinalIgnoreCase)
                || string.Equals(starterClass, activeFarmClass, StringComparison.OrdinalIgnoreCase)
                || (
                    keepHealerFallback
                    && string.Equals(starterClass, "Healer", StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                continue;
            }

            bool inInventory =
                Bot.Inventory?.Items?.Any(inv =>
                    inv != null
                    && inv.Category == ItemCategory.Class
                    && string.Equals(inv.Name?.Trim(), starterClass, StringComparison.OrdinalIgnoreCase)
                ) == true;
            if (!inInventory && CoreBots.Instance.InventoryBagFreeSlots <= 0)
                continue;

            result.Add(item.Name!.Trim());
        }

        return result;
    }

    /// <summary>
    /// Sells superseded non-AC starter classes when <c>SellStarterClasses</c> is enabled,
    /// preserving equipped, AC-tagged, rare, and active Army role classes.
    /// </summary>
    public bool RunSellStarterClasses()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        bool sellEnabled = Bot.Config?.Get<bool>("SellStarterClasses") ?? false;
        bool sellAttempted = false;

        return RunCoordinatedTurnInStep(
            "FARMERJOE_SELL_STARTER_CLASSES",
            () => !sellEnabled || GetSellableStarterClassNames().Count == 0,
            () => !sellEnabled || sellAttempted || GetSellableStarterClassNames().Count == 0,
            () =>
            {
                sellAttempted = true;
                if (!sellEnabled)
                    return;

                List<string> toSell = GetSellableStarterClassNames();
                if (toSell.Count == 0)
                    return;

                LeaveCombatRoomForEquipmentChange();
                foreach (string className in toSell)
                {
                    if (Bot.ShouldExit)
                        return;

                    UnbankIfOwned(className);
                    if (core.CheckInventory(className, toInv: true))
                    {
                        core.Logger($"SellStarterClasses: selling superseded starter class \"{className}\".");
                        core.SellItem(className);
                    }
                }
            }
        );
    }

    private bool AcquireOutfitPetStep(
        string objectiveName,
        string expectedPetName,
        string activePetName,
        string map,
        string monster
    )
    {
        CoreBots core = CoreBots.Instance;
        bool IsDone() =>
            !string.Equals(activePetName, expectedPetName, StringComparison.OrdinalIgnoreCase)
            || core.CheckInventory(expectedPetName, toInv: false);

        return RunCoordinatedCombatStep(
            objectiveName,
            IsDone,
            IsDone,
            monster,
            ClassType.Solo,
            () =>
            {
                if (IsDone())
                    return;

                core.Logger($"FarmerJoe Pet: hunting {monster} in /{map} for \"{expectedPetName}\".");
                core.AddDrop(expectedPetName);
                core.HuntMonster(map, monster, expectedPetName, isTemp: false, log: false);
                Bot.Wait.ForPickup(expectedPetName);
            },
            targetMap: map
        );
    }

    /// <summary>
    /// Handles the optional <c>OutFit</c>, <c>EquipOutfit</c>, and <c>PetChoice</c> objectives
    /// at the end of progression while respecting gear preservation policies.
    /// </summary>
    public bool RunFarmerJoeOutfit()
    {
        CoreBots core = CoreBots.Instance;
        ApplyArmyRoomSettings();

        bool getOutfit = Bot.Config?.Get<bool>("OutFit") ?? false;
        bool equipOutfit = Bot.Config?.Get<bool>("EquipOutfit") ?? false;
        string petName = ResolveSelectedOutfitPetName();
        bool hasPetSelection = !string.IsNullOrEmpty(petName);

        bool NeedSynderesSouvenir() =>
            getOutfit
            && !core.CheckInventory("NO BOTS Armor", toInv: false)
            && !core.CheckInventory("Synderes' Souvenir", toInv: false);

        if (
            !RunCoordinatedCombatStep(
                "FARMERJOE_OUTFIT_SYNDERES_SOUVENIR",
                () => !NeedSynderesSouvenir(),
                () => !NeedSynderesSouvenir(),
                "Evil Elemental",
                ClassType.Farm,
                () =>
                {
                    if (!NeedSynderesSouvenir())
                        return;

                    core.Logger("FarmerJoe Outfit: farming \"Synderes' Souvenir\" [4247] in /enemyforest.");
                    core.AddDrop("Synderes' Souvenir");
                    core.EnsureAccept(4247);
                    core.HuntMonster("enemyforest", "Evil Elemental", "Forest Denizen Slain", 5, isTemp: true);
                    LeaveCombatRoomForEquipmentChange();
                    core.EnsureComplete(4247);
                    Bot.Wait.ForPickup("Synderes' Souvenir");
                },
                targetMap: "enemyforest"
            )
        )
            return false;

        bool buyAttempted = false;
        bool NeedShirtOrHat() =>
            getOutfit
            && (
                !core.CheckInventory("NO BOTS Armor", toInv: false)
                || !core.CheckInventory("Scarecrow Hat", toInv: false)
            );

        if (
            !RunCoordinatedTurnInStep(
                "FARMERJOE_OUTFIT_SHIRT_AND_HAT",
                () => !NeedShirtOrHat(),
                () => buyAttempted || !NeedShirtOrHat(),
                () =>
                {
                    buyAttempted = true;
                    if (!NeedShirtOrHat())
                        return;

                    LeaveCombatRoomForEquipmentChange();
                    if (!core.CheckInventory("NO BOTS Armor", toInv: false))
                    {
                        UnbankIfOwned("Synderes' Souvenir");
                        if (core.CheckInventory("Synderes' Souvenir", toInv: true) && Bot.Player.Gold >= 1000)
                        {
                            core.Logger("FarmerJoe Outfit: buying \"NO BOTS Armor\" in /enemyforest (Shop 332).");
                            Adv.BuyItem("enemyforest", 332, "NO BOTS Armor");
                            Bot.Wait.ForItemBuy();
                        }
                        else
                        {
                            core.Logger("FarmerJoe Outfit: skipping \"NO BOTS Armor\" purchase (missing souvenir or 1,000 Gold).");
                        }
                    }

                    if (!core.CheckInventory("Scarecrow Hat", toInv: false))
                    {
                        if (Bot.Player.Gold >= 1000)
                        {
                            core.Logger("FarmerJoe Outfit: buying \"Scarecrow Hat\" in /yulgar (Shop 16).");
                            Adv.BuyItem("yulgar", 16, "Scarecrow Hat");
                            Bot.Wait.ForItemBuy();
                        }
                        else
                        {
                            core.Logger("FarmerJoe Outfit: skipping \"Scarecrow Hat\" purchase (requires 1,000 Gold).");
                        }
                    }
                }
            )
        )
            return false;

        bool NeedServerIsDown() => getOutfit && !core.CheckInventory("The Server is Down", toInv: false);

        if (
            !RunCoordinatedCombatStep(
                "FARMERJOE_OUTFIT_SERVERS_ARE_DOWN",
                () => !NeedServerIsDown(),
                () => !NeedServerIsDown(),
                "Rabid Server Hamster",
                ClassType.Solo,
                () =>
                {
                    if (!NeedServerIsDown())
                        return;

                    core.Logger("FarmerJoe Outfit: hunting Rabid Server Hamster in /undergroundlabb for \"The Server is Down\".");
                    core.AddDrop("The Server is Down");
                    core.HuntMonster("undergroundlabb", "Rabid Server Hamster", "The Server is Down", isTemp: false, log: false);
                    Bot.Wait.ForPickup("The Server is Down");
                },
                targetMap: "undergroundlabb"
            )
        )
            return false;

        if (!AcquireOutfitPetStep("FARMERJOE_PET_HOT_MAMA", "Hot Mama", petName, "battleundere", "Hot Mama"))
            return false;

        if (!AcquireOutfitPetStep("FARMERJOE_PET_AKRILOTH", "Akriloth Pet", petName, "gravestrike", "Ultra Akriloth"))
            return false;

        bool equipAttempted = false;
        bool wantAnyEquip = equipOutfit || hasPetSelection;

        return RunCoordinatedTurnInStep(
            "FARMERJOE_EQUIP_OUTFIT",
            () => !wantAnyEquip,
            () => !wantAnyEquip || equipAttempted,
            () =>
            {
                equipAttempted = true;
                if (!wantAnyEquip)
                    return;

                if (ShouldPreserveEquippedGear())
                {
                    core.Logger(
                        "FarmerJoe Outfit: preserving pre-equipped gear/pet on this account per BoostGearPolicy / KeepEquippedGearPlayers / DisableBestGear."
                    );
                    return;
                }

                var itemsToEquip = new List<string>();
                if (equipOutfit)
                {
                    string[] outfitCandidates =
                    {
                        "NO BOTS Armor",
                        "Scarecrow Hat",
                        "The Server is Down",
                        "Hollowborn Reaper's Scythe",
                    };
                    foreach (string itemName in outfitCandidates)
                    {
                        if (core.CheckInventory(itemName, toInv: false))
                            itemsToEquip.Add(itemName);
                    }
                }

                if (hasPetSelection && core.CheckInventory(petName, toInv: false))
                    itemsToEquip.Add(petName);

                if (itemsToEquip.Count == 0)
                    return;

                LeaveCombatRoomForEquipmentChange();
                UnbankIfOwned(itemsToEquip.ToArray());
                string[] invItems = itemsToEquip
                    .Where(itemName => core.CheckInventory(itemName, toInv: true))
                    .ToArray();
                if (invItems.Length > 0)
                {
                    core.Logger($"FarmerJoe Outfit: equipping {string.Join(", ", invItems)}.");
                    core.Equip(invItems);
                }

                if (equipOutfit && !IsDisableAutoEnhanceEnabled())
                {
                    string currentClass = Bot.Player?.CurrentClass?.Name ?? string.Empty;
                    EnhanceClassForArmyRespectingGearPolicy(currentClass, allowFullGearEnhance: true);
                }
            }
        );
    }
    #endregion Farmer Joe Optional Cosmetics & Cleanup

    public bool RunCooperativeObjective(
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action help,
        int maxWorkAttempts = 0,
        int maxPlayersPerSubgroup = 0,
        Action? prepareForCombat = null
    ) => Army.RunCooperativeObjective(
        objectiveName,
        isComplete,
        doWork,
        (targetPlayerNumber, targetPlayerName) => help(),
        maxWorkAttempts,
        maxPlayersPerSubgroup,
        prepareForCombat
    );

    public bool RunCooperativeObjective(
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action<int, string> help,
        int maxWorkAttempts = 0,
        int maxPlayersPerSubgroup = 0,
        Action? prepareForCombat = null
    ) => Army.RunCooperativeObjective(
        objectiveName,
        isComplete,
        doWork,
        help,
        maxWorkAttempts,
        maxPlayersPerSubgroup,
        prepareForCombat
    );

    public bool IsPlayer(int playerNumber) => Army.IsArmyPlayer(playerNumber);

    public bool StopArmySync(string reason) => Army.StopArmySync(reason);

    public bool WaitForArmyStop() => Army.SyncArmy("STOP_CHECK");
}
