/*
name: Core Army FarmerJoe
description: Shared Army coordination and combat mechanics for FarmerJoe.
tags: core, army, farmerjoe
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.Options;

#nullable enable

public class CoreArmyFJ
{

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced? _Adv;

    private const string ArmyProtocolVersion = "1";
    private const int ArmyPollDelay = 500;
    private const int ArmyFileRetryDelay = 100;
    private static readonly string[] ArmyAliases =
    {
        "playerOne",
        "playerTwo",
        "playerThree",
        "playerFour",
        "playerFive",
        "playerSix",
        "playerSeven",
    };

    private string[] armyPlayers = Array.Empty<string>();
    private string armyUsername = string.Empty;
    private string armySyncPath = string.Empty;
    private string armyLaunchToken = string.Empty;
    private string armySessionId = string.Empty;
    private int armyPlayerIndex = -1;
    private long nextArmyStepId = 1;
    private long lastArmyStepId;
    private bool armyInitialized;
    private bool armySessionStarted;
    private bool armySessionFailureLogged;
    private bool armyStopLogged;
    private bool armyTransportFailed;
    private bool armyTransportFailureLogged;

    public Option<string> player1 = new(
        "player1",
        "Player 1",
        "Required. Player 1 is the sync leader.",
        string.Empty
    );

    public Option<string> player2 = new(
        "player2",
        "Player 2",
        "Required. Players 1-4 are required; players 5-7 are optional.",
        string.Empty
    );

    public Option<string> player3 = new(
        "player3",
        "Player 3",
        "Required. Players 1-4 are required; players 5-7 are optional.",
        string.Empty
    );

    public Option<string> player4 = new(
        "player4",
        "Player 4",
        "Required. Players 1-4 are required; players 5-7 are optional.",
        string.Empty
    );

    public Option<string> player5 = new(
        "player5",
        "Player 5",
        "Optional; maximum group size is 7.",
        string.Empty
    );

    public Option<string> player6 = new(
        "player6",
        "Player 6",
        "Optional; maximum group size is 7.",
        string.Empty
    );

    public Option<string> player7 = new(
        "player7",
        "Player 7",
        "Optional; maximum group size is 7.",
        string.Empty
    );

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    public bool ValidateFunctionBasedSkillsDisabled()
    {
        IPluginContainer? plugin = Ioc.Default
            .GetRequiredService<IPluginManager>()
            .GetContainer("Function Based Skills");

        if (plugin == null)
            return true;

        bool enabled;
        try
        {
            enabled = plugin.OptionContainer.Get<bool>("Enabled");
        }
        catch
        {
            enabled = true;
        }

        if (!enabled)
            return true;

        Core.Logger(
            "Function Based Skills is detected. Disable it before running this script.\nAll LW scripts use their own custom skill engine, and this plugin will interfere with it.",
            "ValidateFunctionBasedSkillsDisabled",
            messageBox: true,
            stopBot: true
        );
        return false;
    }

    public bool ValidatePrivateRoomNumber(int roomNumber)
    {
        if (roomNumber >= 1001 && roomNumber <= 99999)
            return true;

        Core.Logger(
            $"Private room number {roomNumber} is invalid. Use 1001 through 99999.",
            "ValidatePrivateRoomNumber",
            messageBox: true,
            stopBot: true
        );
        return false;
    }


    public bool StartArmySync(
        string syncFileName,
        int playerCount,
        string? optionCategory = null
    )
    {
        if (!ValidateFunctionBasedSkillsDisabled())
            return false;

        ResetArmyState();

        if (
            string.IsNullOrWhiteSpace(syncFileName)
            || !string.Equals(
                Path.GetFileName(syncFileName),
                syncFileName,
                StringComparison.Ordinal
            )
            || !string.Equals(
                Path.GetExtension(syncFileName),
                ".sync",
                StringComparison.OrdinalIgnoreCase
            )
        )
            return ArmyInitializationFailure("Sync file must be a filename ending in .sync.");

        if (playerCount < 4 || playerCount > 7)
            return ArmyInitializationFailure("Army player count must be from 4 through 7.");

        if (Bot.Config == null)
            return ArmyInitializationFailure("Army player configuration is unavailable.");

        string[] players = new string[playerCount];
        for (int index = 0; index < playerCount; index++)
        {
            string optionName = $"player{index + 1}";
            players[index] = NormalizeArmyUsername(
                string.IsNullOrEmpty(optionCategory)
                    ? Bot.Config.Get<string>(optionName)
                    : Bot.Config.Get<string>(optionCategory, optionName)
            );
        }

        if (players.Any(string.IsNullOrEmpty))
            return ArmyInitializationFailure("Every requested Army player name is required.");

        if (players.Distinct(StringComparer.OrdinalIgnoreCase).Count() != players.Length)
            return ArmyInitializationFailure("Army player names must be unique.");

        string username = NormalizeArmyUsername(Core.Username());
        int playerIndex = Array.FindIndex(
            players,
            player => string.Equals(player, username, StringComparison.OrdinalIgnoreCase)
        );
        if (playerIndex < 0)
            return ArmyInitializationFailure("Current account is not in the Army roster.");

        armyPlayers = players;
        armyUsername = username;
        armyPlayerIndex = playerIndex;
        armyLaunchToken = Guid.NewGuid().ToString("N");
        armySessionId = armyPlayerIndex == 0 ? Guid.NewGuid().ToString("N") : string.Empty;
        armySyncPath = Path.Combine(ClientFileSources.SkuaOptionsDIR, syncFileName);
        armyInitialized = true;

        bool started = armyPlayerIndex == 0
            ? StartLeaderArmySession()
            : JoinFollowerArmySession();
        armySessionStarted = started;
        return started;
    }

    private bool ValidateArmyRequirements(int? minimumLevel = null, string ultraName = "")
    {
        InventoryItem[]? houseItems = null;
        bool requestedHouseInventory = false;
        try
        {
            Bot.Wait.ForTrue(
                () =>
                {
                    if (!Bot.Player.Loaded)
                        return false;

                    houseItems = Bot.Flash.GetGameObject<InventoryItem[]>(
                        "world.myAvatar.houseitems"
                    );
                    if (houseItems != null)
                        return true;

                    if (!requestedHouseInventory)
                    {
                        requestedHouseInventory = true;
                        Bot.Flash.CallGameFunction("world.loadHouseInventory");
                    }

                    return false;
                },
                40
            );
        }
        catch
        {
            houseItems = null;
        }

        if (Bot.ShouldExit)
            return false;

        if (minimumLevel.HasValue)
        {
            if (
                Bot.Player.Level < minimumLevel.Value
                && !SendArmySignal("ULTRA_LEVEL_INVALID")
            )
                return false;

            bool hasRequiredDamageBoost = Bot.Inventory.Items.Any(item =>
                item.Equipped
                && (
                    !Core.NoneEnhancableFilter(item)
                    || item.Category == ItemCategory.Armor
                )
                && Core.GetBoostFloat(item, "dmgAll") >= 1.30f
            );

            if (
                !hasRequiredDamageBoost
                && !SendArmySignal("ULTRA_WEAPON_INVALID")
            )
                return false;
        }

        string? failureSignal = houseItems == null
            ? "HOUSE_REQUIREMENT_UNAVAILABLE"
            : houseItems.Any(item =>
                item != null && item.Category == ItemCategory.House && item.Equipped
            )
                ? null
                : "HOUSE_REQUIREMENT_INVALID";

        if (failureSignal != null && !SendArmySignal(failureSignal))
            return false;

        if (!SyncArmy(minimumLevel.HasValue ? "ULTRA_ACCESS_CHECK" : "HOUSE_REQUIREMENTS"))
            return false;

        if (minimumLevel.HasValue)
        {
            List<string> playersBelowLevel = new();
            List<string> playersMissingDamageBoost = new();
            for (int playerNumber = 1; playerNumber <= armyPlayers.Length; playerNumber++)
            {
                if (HasArmySignal("ULTRA_LEVEL_INVALID", playerNumber))
                    playersBelowLevel.Add($"Player {playerNumber}");

                if (HasArmySignal("ULTRA_WEAPON_INVALID", playerNumber))
                    playersMissingDamageBoost.Add($"Player {playerNumber}");
            }

            if (playersBelowLevel.Count > 0)
            {
                Core.Logger(
                    $"{ultraName} requires every player to be at least level {minimumLevel}. Players below the requirement: {string.Join(", ", playersBelowLevel)}.",
                    "ValidateUltraAccess",
                    messageBox: true,
                    stopBot: true
                );
                return false;
            }

            if (playersMissingDamageBoost.Count > 0)
            {
                Core.Logger(
                    $"{ultraName} requires every player to equip a weapon or armor with at least a 30% damage boost to all before starting. Players missing the equipped boost: {string.Join(", ", playersMissingDamageBoost)}. Equip a qualifying weapon or armor and restart the script.",
                    "ValidateUltraAccess",
                    messageBox: true,
                    stopBot: true
                );
                return false;
            }
        }

        string[] lines = ReadArmyLines();
        if (Bot.ShouldExit || armyTransportFailed || !CanContinueArmySync(lines))
            return false;

        string[][] reports = lines.Select(line => line.Split('|')).Where(parts =>
            parts.Length == 5
            && parts[0] == "SIGNAL"
            && parts[1] == ArmyProtocolVersion
            && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
        ).ToArray();

        List<string> playersMissingHouse = new();
        List<string> playersUnavailable = new();
        for (int index = 0; index < armyPlayers.Length; index++)
        {
            foreach (string[] report in reports)
            {
                if (!string.Equals(
                    NormalizeArmyUsername(report[4]),
                    armyPlayers[index],
                    StringComparison.OrdinalIgnoreCase
                ))
                    continue;

                if (report[3] == "HOUSE_REQUIREMENT_INVALID")
                {
                    playersMissingHouse.Add($"Player {index + 1}");
                    break;
                }

                if (report[3] == "HOUSE_REQUIREMENT_UNAVAILABLE")
                {
                    playersUnavailable.Add($"Player {index + 1}");
                    break;
                }
            }
        }

        List<string> failures = new();
        if (playersMissingHouse.Count > 0)
            failures.Add(
                $"Every player must own and equip a house before starting. Players missing an equipped house: {string.Join(", ", playersMissingHouse)}. Buy or equip a house and restart the script."
            );
        if (playersUnavailable.Count > 0)
            failures.Add(
                $"House requirement could not be verified for: {string.Join(", ", playersUnavailable)}. Wait for the account to finish loading and restart the script."
            );

        if (failures.Count == 0)
            return !Bot.ShouldExit;

        Core.Logger(
            string.Join(" ", failures),
            "ValidateEquippedHouseRequirement",
            messageBox: true,
            stopBot: true
        );
        return false;
    }

    public bool SyncArmy(string step)
    {
        if (!RequireArmySession())
            return false;

        if (!CanContinueArmySync(ReadArmyLines()))
            return false;

        string stepName = NormalizeArmyRecordName(step);
        if (string.IsNullOrEmpty(stepName))
        {
            Core.Logger("Army sync step name is invalid.", "CoreArmyFJ");
            return false;
        }

        return armyPlayerIndex == 0
            ? RunLeaderArmySync(stepName)
            : RunFollowerArmySync(stepName);
    }

    // Work and help callbacks should perform one bounded action so each account can recheck progress.
    public bool RunCooperativeObjective(
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action help,
        int maxWorkAttempts = 0,
        int maxPlayersPerSubgroup = 0,
        Action? prepareForCombat = null
    ) =>
        RunCooperativeObjective(
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
    )
    {
        if (!RequireArmySession())
            return false;

        string name = NormalizeArmyRecordName(objectiveName);
        if (
            string.IsNullOrEmpty(name)
            || isComplete == null
            || doWork == null
            || help == null
        )
        {
            Core.Logger("Cooperative Army objective configuration is invalid.", "CoreArmyFJ");
            return false;
        }

        if (!SyncArmy($"OBJECTIVE_{name}_START"))
            return false;

        if (prepareForCombat != null)
        {
            if (!isComplete())
                prepareForCombat();

            if (!SyncArmy($"OBJECTIVE_{name}_READY"))
                return false;
        }

        long objectiveId = lastArmyStepId;
        string doneSignal = $"SIG_OBJ_{objectiveId}_{name}_DONE";
        string failSignal = $"SIG_OBJ_{objectiveId}_{name}_FAILED";
        bool reportedDone = false;
        int workAttempts = 0;

        unreachableHelperTargets.Clear();
        attemptedBypassReloadCells.Clear();
        failedGotoCounts.Clear();
        loggedHelperSleepMode = false;

        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            if (!CanContinueArmySync(ReadArmyLines()))
                return false;

            // Abort the whole Army if any account gave up on this objective.
            for (int playerNumber = 1; playerNumber <= armyPlayers.Length; playerNumber++)
            {
                if (HasArmySignal(failSignal, playerNumber))
                {
                    Core.Logger(
                        $"Army objective {name}: player {playerNumber} ({armyPlayers[playerNumber - 1]}) failed. Stopping.",
                        "CoreArmyFJ"
                    );
                    return false;
                }
            }

            bool localComplete = reportedDone || isComplete();
            if (localComplete && !reportedDone)
            {
                if (!SendArmySignal(doneSignal))
                    return false;

                reportedDone = true;
                Core.Logger($"Army objective {name}: local account is done; helping others.", "CoreArmyFJ");
            }

            List<int> pendingPlayers = new();
            List<int> helperPlayers = new();
            for (int playerNumber = 1; playerNumber <= armyPlayers.Length; playerNumber++)
            {
                if (HasArmySignal(doneSignal, playerNumber))
                    helperPlayers.Add(playerNumber);
                else
                    pendingPlayers.Add(playerNumber);
            }

            if (pendingPlayers.Count == 0)
                return SyncArmy($"OBJECTIVE_{name}_DONE");

            if (localComplete)
            {
                // Spread helpers across pending players (optionally scoped to fixed-size room subgroups).
                int localPlayerNumber = armyPlayerIndex + 1;
                bool useSubgroups = maxPlayersPerSubgroup > 0;
                int localSubgroup = useSubgroups
                    ? (localPlayerNumber - 1) / maxPlayersPerSubgroup
                    : 0;

                List<int> scopedPending = useSubgroups
                    ? pendingPlayers
                        .Where(p => (p - 1) / maxPlayersPerSubgroup == localSubgroup)
                        .ToList()
                    : pendingPlayers;

                if (scopedPending.Count == 0)
                {
                    actAsSupportHelper = false;
                    help(0, string.Empty);
                }
                else
                {
                    List<int> accessiblePending = scopedPending
                        .Where(p => !unreachableHelperTargets.Contains(NormalizeArmyUsername(armyPlayers[p - 1])))
                        .ToList();

                    if (accessiblePending.Count == 0)
                    {
                        actAsSupportHelper = false;
                        if (!loggedHelperSleepMode)
                        {
                            Core.Logger(
                                $"Army objective {name}: remaining teammate(s) are in higher-level or inaccessible rooms; waiting in sleep mode.",
                                "CoreArmyFJ"
                            );
                            loggedHelperSleepMode = true;
                        }
                        Bot.Sleep(1000);
                    }
                    else
                    {
                        List<int> scopedHelpers = useSubgroups
                            ? helperPlayers
                                .Where(p => (p - 1) / maxPlayersPerSubgroup == localSubgroup)
                                .ToList()
                            : helperPlayers;

                        int helperIndex = Math.Max(0, scopedHelpers.IndexOf(localPlayerNumber));
                        int targetPlayerNumber = accessiblePending[helperIndex % accessiblePending.Count];
                        string targetPlayerName = armyPlayers[targetPlayerNumber - 1];

                        // Option 1 (First-Helper-To-Support):
                        // While all accounts are working on an objective (e.g., ranking a class to 10),
                        // everyone equips the required class. As soon as the 1st account finishes and enters
                        // Helper mode (helperIndex == 0), it equips a Support class (Oracle / StoneCrusher /
                        // ArchPaladin / Lord of Order) to heal the remaining accounts. Once Player 2 (or the
                        // subgroup's dedicated Support when split) finishes, that account takes over the
                        // Support role and the temporary support helper returns to its own SoloClass!
                        int supportPlayerNumber = useSubgroups
                            ? localSubgroup * maxPlayersPerSubgroup + 2
                            : 2;
                        actAsSupportHelper =
                            localPlayerNumber == supportPlayerNumber
                            || (!scopedHelpers.Contains(supportPlayerNumber) && helperIndex == 0);

                        help(targetPlayerNumber, targetPlayerName);
                    }
                }
            }
            else
            {
                actAsSupportHelper = false;
                if (maxWorkAttempts > 0 && workAttempts >= maxWorkAttempts)
                {
                    Core.Logger(
                        $"Army objective {name}: gave up after {workAttempts} attempts. Notifying the Army.",
                        "CoreArmyFJ"
                    );
                    SendArmySignal(failSignal);
                    return false;
                }

                workAttempts++;
                doWork();
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return false;
    }

    private static readonly string[] SupportHelperPriority =
    {
        "Lord of Order",
        "ArchPaladin",
        "StoneCrusher",
        "Oracle",
        "Evolved Shaman",
        "Healer (Rare)",
        "Healer",
    };

    private bool HasClassInInventory(string? className) =>
        !string.IsNullOrWhiteSpace(className)
        && Bot.Inventory.Items.Any(item =>
            item != null
            && item.Category == ItemCategory.Class
            && string.Equals(item.Name?.Trim(), className.Trim(), StringComparison.OrdinalIgnoreCase)
        );

    private bool actAsSupportHelper;
    private string lastSkillRotationClass = string.Empty;
    private string lastSpecialEquippedMapRoom = string.Empty;
    private readonly HashSet<string> unreachableHelperTargets = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> attemptedBypassReloadCells = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> failedGotoCounts = new(StringComparer.OrdinalIgnoreCase);
    private bool loggedHelperSleepMode;

    private static readonly HashSet<string> LevelBracketedMaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "icestormarena",
        "icestormunder",
        "battlegrounda",
        "battlegroundb",
        "battlegroundc",
        "battlegroundd",
        "battlegrounde",
        "icewing",
        "voidxyfrag",
        "voidnerfkitten",
    };

    private static bool IsLevelBracketedMap(string? mapName)
    {
        string? normalized = NormalizeMapKey(mapName);
        return !string.IsNullOrEmpty(normalized) && LevelBracketedMaps.Contains(normalized);
    }

    private static int GetRequiredLevelForMapCell(string? mapName, string? cell)
    {
        string? normalizedMap = NormalizeMapKey(mapName);
        string normalizedCell = (cell ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalizedMap))
            return 0;

        if (normalizedMap == "icestormarena")
        {
            return normalizedCell switch
            {
                "r4" => 1,
                "r5" => 5,
                "r6" => 10,
                "r7" => 20,
                "r10" => 25,
                "r11" => 30,
                "r14" => 35,
                "r16" => 50,
                "r17" => 61,
                "r2" => 61,
                "r3" or "r3a" or "r3b" or "r3c" or "r18" or "r19" or "r20" => 75,
                _ => 0,
            };
        }

        if (normalizedMap == "icestormunder")
            return normalizedCell is "enter" or "spawn" or "" ? 0 : 75;

        if (normalizedMap == "battlegrounda")
            return 1;
        if (normalizedMap == "battlegroundb")
            return normalizedCell is "enter" or "spawn" or "" ? 0 : 16;
        if (normalizedMap == "battlegroundc")
            return normalizedCell is "enter" or "spawn" or "" ? 0 : 31;
        if (normalizedMap == "battlegroundd")
            return normalizedCell is "enter" or "spawn" or "" ? 0 : 46;
        if (normalizedMap == "battlegrounde")
            return normalizedCell is "enter" or "spawn" or "" ? 0 : 61;

        if (normalizedMap == "icewing")
            return 75;

        if (normalizedMap is "voidxyfrag" or "voidnerfkitten")
            return 80;

        return 0;
    }

    private static readonly HashSet<string> SoloOnlyMaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "queeniona",
        "baconcatb",
        "caroling",
        "chaosbattle",
        "chaoslord",
        "chaosrealm",
        "darkthronehub",
        "drakathfight",
        "dragonfire",
        "dragonkoi",
        "falcontower",
        "finalbattle",
        "finalshowdown",
        "herotournament",
        "infernalarena",
        "malgor",
        "nothing",
        "ravenscar",
        "superslayin",
        "titandrakath",
        "tlapd",
        "trickortreat",
        "vordredboss",
        "treetitanbattle",
    };

    private static readonly Dictionary<string, (int Slot, int Value)[]> MapQuestSlotBypasses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["marsh2"] = new[] { (58, 8) },
            ["nightmare"] = new[] { (192, 9) },
            ["temple"] = new[] { (49, 25) },
            ["wanders"] = new[] { (176, 6) },
            ["kitsune"] = new[] { (25, 22) },
            ["dragonkoiz"] = new[] { (25, 22) },
            ["elemental"] = new[] { (32, 35) },
            ["twilightedge"] = new[] { (156, 23) },
            ["desoloth"] = new[] { (56, 35) },
            ["xancave"] = new[] { (53, 35) },
            ["shadowgrove"] = new[] { (315, 7) },
            ["stalagbite"] = new[] { (22, 35) },
            ["maloth"] = new[] { (246, 23) },
            ["originul"] = new[] { (387, 16) },
            ["fiendshard"] = new[] { (387, 16) },
            ["mummies"] = new[] { (97, 15) },
            ["doomvault"] = new[] { (126, 18) },
            ["doomvaultb"] = new[] { (127, 26), (126, 18) },
            ["pyramid"] = new[] { (36, 28) },
            ["djinn"] = new[] { (36, 28) },
            ["ultradrakath"] = new[] { (182, 5) },
            ["backroom"] = new[] { (402, 12) },
            ["chaoscave"] = new[] { (26, 22) },
            ["lycanwar"] = new[] { (26, 22) },
            ["timespace"] = new[] { (100, 14) },
            ["transformation"] = new[] { (405, 12) },
            ["ebilcorphq"] = new[] { (431, 9) },
            ["necrodungeon"] = new[] { (77, 18) },
            ["oddities"] = new[] { (456, 13) },
            ["championdrakath"] = new[] { (182, 7) },
            ["glacera"] = new[] { (225, 21) },
            ["ultratyndarius"] = new[] { (412, 22) },
            ["towerofdoom"] = new[] { (159, 10) },
            ["towerofdoom2"] = new[] { (159, 10) },
            ["towerofdoom3"] = new[] { (159, 10) },
            ["towerofdoom4"] = new[] { (159, 10) },
            ["towerofdoom5"] = new[] { (159, 10) },
            ["towerofdoom6"] = new[] { (159, 10) },
            ["towerofdoom7"] = new[] { (159, 10) },
            ["towerofdoom8"] = new[] { (159, 10) },
            ["towerofdoom9"] = new[] { (159, 10) },
            ["towerofdoom10"] = new[] { (159, 10) },
            ["wolfwing"] = new[] { (26, 23) },
            ["darkoviaforest"] = new[] { (26, 23) },
            ["lycan"] = new[] { (26, 23) },
            ["safiria"] = new[] { (26, 23) },
            ["manacradle"] = new[] { (488, 20) },
            ["shadowattack"] = new[] { (175, 18) },
            ["dreadhaven"] = new[] { (175, 20) },
            ["confrontation"] = new[] { (175, 20) },
            ["celestialarenab"] = new[] { (249, 20) },
            ["celestialarenac"] = new[] { (249, 20) },
            ["celestialarenad"] = new[] { (249, 20) },
            ["moonyardb"] = new[] { (28, 35) },
        };

    private static readonly Dictionary<string, int> MapQuestIdBypasses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["stormtemple"] = 2814,
            ["onslaughttower"] = 2621,
            ["ledgermayne"] = 847,
        };

    private static string? NormalizeMapKey(string? mapName)
    {
        if (string.IsNullOrWhiteSpace(mapName))
            return null;

        int dashIndex = mapName.IndexOf('-');
        string cleanName = dashIndex >= 0 ? mapName.Substring(0, dashIndex) : mapName;
        return cleanName.Trim().ToLowerInvariant();
    }

    private static bool IsSoloOnlyMap(string? mapName)
    {
        string? normalized = NormalizeMapKey(mapName);
        return !string.IsNullOrEmpty(normalized) && SoloOnlyMaps.Contains(normalized);
    }

    private void ApplyQuestSlotBypass(int slot, int value)
    {
        try
        {
            if (Bot.Flash.CallGameFunction<int>("world.getQuestValue", slot) < value)
                Bot.Quests.UpdateQuest(value, slot);
        }
        catch
        {
            // Ignore transient Flash read errors while map transitions settle.
        }
    }

    public void EnsureMapAccessBypass(string? mapName)
    {
        string? normalized = NormalizeMapKey(mapName);
        if (string.IsNullOrEmpty(normalized))
            return;

        if (string.Equals(normalized, "doomvaultb", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Core.SetAchievement(18);
            }
            catch
            {
                // Best effort; quest-slot bypasses below still proceed.
            }
        }

        if (MapQuestIdBypasses.TryGetValue(normalized, out int questId))
        {
            try
            {
                if (!Bot.Quests.HasBeenCompleted(questId))
                    Bot.Quests.UpdateQuest(questId);
            }
            catch
            {
                // Best effort when quest data has not been fetched yet.
            }
        }

        if (MapQuestSlotBypasses.TryGetValue(normalized, out var slots))
        {
            foreach (var (slot, value) in slots)
                ApplyQuestSlotBypass(slot, value);
        }

        EnsureSpecialMapItemEquipped(normalized);
    }

    private void EnsureSpecialMapItemEquipped(string? normalizedMap)
    {
        if (string.IsNullOrEmpty(normalizedMap) || Bot.Map == null || !Bot.Map.Loaded)
            return;

        string? currentLoadedMap = NormalizeMapKey(Bot.Map.Name);
        if (!string.Equals(currentLoadedMap, normalizedMap, StringComparison.OrdinalIgnoreCase))
            return;

        string roomKey = $"{normalizedMap}:{Bot.Map.RoomID}";
        if (string.Equals(lastSpecialEquippedMapRoom, roomKey, StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            if (string.Equals(normalizedMap, "rivensylth", StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.GetMapItem(3935);
                Core.Sleep(300);
                Core.SendPackets($"%xt%zm%equipItem%{Bot.Map.RoomID}%32057%");
                Core.Sleep(300);
                lastSpecialEquippedMapRoom = roomKey;
            }
            else if (
                string.Equals(normalizedMap, "onslaughttower", StringComparison.OrdinalIgnoreCase)
                && !Core.isCompletedBefore(2627)
            )
            {
                Bot.Map.GetMapItem(68);
                Core.Sleep(300);
                Core.SendPackets($"%xt%zm%equipItem%{Bot.Map.RoomID}%2096%");
                Core.Sleep(300);
                lastSpecialEquippedMapRoom = roomKey;
            }
        }
        catch
        {
            // Ignore transient packet errors while map is still initializing.
        }
    }

    private bool JumpToPlayerInSameMap(
        string normalizedTarget,
        string targetCell,
        string targetPad,
        string? targetMap
    )
    {
        string? currentMap = NormalizeMapKey(Bot.Map?.Name ?? targetMap);
        if (string.Equals(currentMap, "whitemap", StringComparison.OrdinalIgnoreCase))
            return false;

        EnsureMapAccessBypass(currentMap);

        int requiredLevel = GetRequiredLevelForMapCell(currentMap, targetCell);
        if (requiredLevel > 0 && Bot.Player.Level < requiredLevel)
        {
            if (unreachableHelperTargets.Add(normalizedTarget))
            {
                Core.Logger(
                    $"Map '{currentMap}' cell '{targetCell}' requires Lv {requiredLevel} (local Lv {Bot.Player.Level}); putting helper in sleep mode for '{normalizedTarget}'.",
                    "CoordinateFollowAndCombat"
                );
            }
            return false;
        }

        if (Bot.Player.Cell == targetCell)
            return true;

        Bot.Map?.Jump(targetCell, targetPad);
        Bot.Wait.ForCellChange(targetCell);
        if (Bot.Player.Cell == targetCell)
            return true;

        // Retry once after cancelling target/combat in case a transient combat lock blocked the cell jump.
        if (Bot.Player.HasTarget)
            Bot.Combat.CancelTarget();
        Bot.Sleep(300);
        if (Bot.Map?.TryGetPlayer(normalizedTarget, out var refreshedTarget) == true && refreshedTarget != null)
        {
            targetCell = refreshedTarget.Cell;
            targetPad = refreshedTarget.Pad;
            requiredLevel = GetRequiredLevelForMapCell(currentMap, targetCell);
            if (requiredLevel > 0 && Bot.Player.Level < requiredLevel)
            {
                if (unreachableHelperTargets.Add(normalizedTarget))
                {
                    Core.Logger(
                        $"Map '{currentMap}' cell '{targetCell}' requires Lv {requiredLevel} (local Lv {Bot.Player.Level}); putting helper in sleep mode for '{normalizedTarget}'.",
                        "CoordinateFollowAndCombat"
                    );
                }
                return false;
            }
            if (Bot.Player.Cell == targetCell)
                return true;
        }
        Bot.Map?.Jump(targetCell, targetPad);
        Bot.Wait.ForCellChange(targetCell);
        if (Bot.Player.Cell == targetCell)
            return true;

        // Level-bracketed XP/CP grind maps (e.g. /icestormarena, /icestormunder, /battlegrounde):
        // Never bounce through /whitemap here (that can strand helpers in /whitemap during SaveState
        // or room transitions). Only mark unreachable when the local level is genuinely below the room cap.
        if (IsLevelBracketedMap(currentMap))
        {
            if (requiredLevel > 0 && Bot.Player.Level < requiredLevel)
            {
                if (unreachableHelperTargets.Add(normalizedTarget))
                {
                    Core.Logger(
                        $"Map '{currentMap}' cell '{targetCell}' requires Lv {requiredLevel} (local Lv {Bot.Player.Level}); putting helper in sleep mode for '{normalizedTarget}'.",
                        "CoordinateFollowAndCombat"
                    );
                }
            }
            else
            {
                Bot.Sleep(500);
            }
            return false;
        }

        // If the client loaded a gated map SWF before quest-tree/achievement bypasses were
        // applied (e.g., /wolfwing Dracowerepyre gate or /doomvault cutscenes), reload via
        // /whitemap with bypasses active at most once per target+cell before falling back to sleep mode.
        string reloadKey = $"{normalizedTarget}:{currentMap}:{targetCell}";
        if (!string.IsNullOrEmpty(currentMap) && !IsSoloOnlyMap(currentMap) && attemptedBypassReloadCells.Add(reloadKey))
        {
            Core.Logger(
                $"Map '{currentMap}' blocked cell jump to '{targetCell}'; reloading via /whitemap with access bypass.",
                "CoordinateFollowAndCombat"
            );
            Core.Join("whitemap", ignoreCheck: true);
            EnsureMapAccessBypass(currentMap);
            Bot.Player.Goto(normalizedTarget);
            Bot.Wait.ForTrue(() => Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget), 20);
            if (!Bot.Map.PlayerExists(normalizedTarget) || string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
            {
                Core.Join(currentMap);
                Bot.Wait.ForTrue(() => Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget), 20);
            }
            EnsureMapAccessBypass(currentMap);
            if (Bot.Map.TryGetPlayer(normalizedTarget, out var playerAfterReload) && playerAfterReload != null)
            {
                if (playerAfterReload.Cell != Bot.Player.Cell)
                {
                    Bot.Map.Jump(playerAfterReload.Cell, playerAfterReload.Pad);
                    Bot.Wait.ForCellChange(playerAfterReload.Cell);
                }
                if (Bot.Player.Cell == playerAfterReload.Cell)
                    return true;
            }
        }

        if (Bot.Player.Cell != targetCell && unreachableHelperTargets.Add(normalizedTarget))
        {
            Core.Logger(
                $"Map '{currentMap}' cell '{targetCell}' remains inaccessible after bypass check; putting helper in sleep mode for '{normalizedTarget}'.",
                "CoordinateFollowAndCombat"
            );
        }

        return Bot.Player.Cell == targetCell;
    }

    private volatile bool _helperSeaVoiceChargeDetected;
    private bool _helperSeaVoiceListenerAttached;

    private void HelperSeaVoiceFlashListener(string name, object[] args)
    {
        try
        {
            if (name != "pext" || args.Length == 0 || _helperSeaVoiceChargeDetected)
                return;

            string rawPacket = args[0]?.ToString() ?? string.Empty;
            if (rawPacket.IndexOf("You will not resist!", StringComparison.OrdinalIgnoreCase) < 0)
                return;

            _helperSeaVoiceChargeDetected = true;
            Bot.Combat.StopAttacking = true;
            try
            {
                Bot.Skills.Stop();
            }
            catch
            {
            }
            Core.Logger("Helper detected Voice in the Sea charge; pausing skills for Vigil.", "CopyHelpedActions");
        }
        catch
        {
        }
    }

    private void EnsureSeaVoiceHelperProtection(bool shouldAttach)
    {
        bool inSeaVoice = string.Equals(Bot.Map?.Name, "seavoice", StringComparison.OrdinalIgnoreCase);
        if (shouldAttach && inSeaVoice)
        {
            if (!_helperSeaVoiceListenerAttached)
            {
                _helperSeaVoiceChargeDetected = false;
                Bot.Flash.FlashCall += HelperSeaVoiceFlashListener;
                _helperSeaVoiceListenerAttached = true;
            }

            if (!Bot.Inventory.IsEquipped("Vigil"))
            {
                if (!Core.CheckInventory("Vigil", 1))
                    Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
                if (Core.CheckInventory("Vigil", 1) && !Bot.Inventory.IsEquipped("Vigil"))
                    Core.Equip("Vigil");
            }
        }
        else if (_helperSeaVoiceListenerAttached)
        {
            Bot.Flash.FlashCall -= HelperSeaVoiceFlashListener;
            _helperSeaVoiceListenerAttached = false;
            _helperSeaVoiceChargeDetected = false;
            Bot.Combat.StopAttacking = false;
        }
    }

    private void HandleCopyHelpedActions(Skua.Core.Models.Players.PlayerInfo? targetPlayer)
    {
        if (targetPlayer != null)
        {
            float dx = Math.Abs(Bot.Player.X - targetPlayer.X);
            float dy = Math.Abs(Bot.Player.Y - targetPlayer.Y);
            if (dx > 25f || dy > 25f)
            {
                Bot.Player.WalkTo((int)targetPlayer.X, (int)targetPlayer.Y);
            }
        }

        bool inSeaVoice = string.Equals(Bot.Map?.Name, "seavoice", StringComparison.OrdinalIgnoreCase);
        EnsureSeaVoiceHelperProtection(inSeaVoice);

        if (inSeaVoice && _helperSeaVoiceChargeDetected)
        {
            Bot.Combat.StopAttacking = true;
            try
            {
                Bot.Skills.Stop();
            }
            catch
            {
            }
            while (
                !Bot.ShouldExit
                && Bot.Player.Alive
                && Bot.Player.HasTarget
                && !Bot.Self.HasActiveAura("Vigil")
                && string.Equals(Bot.Map?.Name, "seavoice", StringComparison.OrdinalIgnoreCase)
            )
            {
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
                Bot.Sleep(50);
            }
            _helperSeaVoiceChargeDetected = false;
            Bot.Combat.StopAttacking = false;
        }
    }

    public void CoordinateFollowAndCombat(
        string targetPlayerName,
        string monster = "*",
        bool useSkillRotations = true,
        bool waitForFirstHit = true,
        ClassType compositionType = ClassType.Solo,
        string? targetMap = null,
        bool copyHelpedActions = false
    )
    {
        if (string.IsNullOrWhiteSpace(targetPlayerName) || Bot.ShouldExit)
            return;

        if (IsSoloOnlyMap(targetMap))
            return;

        string normalizedTarget = NormalizeArmyUsername(targetPlayerName);
        if (string.Equals(normalizedTarget, armyUsername, StringComparison.OrdinalIgnoreCase))
            return;

        if (unreachableHelperTargets.Contains(normalizedTarget))
        {
            Bot.Sleep(1000);
            return;
        }

        bool isDoomKittenFight =
            string.Equals(NormalizeMapKey(targetMap ?? Bot.Map?.Name), "doomkitten", StringComparison.OrdinalIgnoreCase)
            || string.Equals(monster, "DoomKitten", StringComparison.OrdinalIgnoreCase);

        bool inSameCell = false;
        if (
            !string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase)
            && Bot.Map.TryGetPlayer(normalizedTarget, out var targetPlayer)
            && targetPlayer != null
        )
        {
            inSameCell = JumpToPlayerInSameMap(
                normalizedTarget,
                targetPlayer.Cell,
                targetPlayer.Pad,
                Bot.Map?.Name ?? targetMap
            );
        }
        else
        {
            EnsureMapAccessBypass(targetMap);
            Bot.Player.Goto(normalizedTarget);
            Bot.Wait.ForTrue(() => Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget), 20);
            if (
                (!Bot.Map.PlayerExists(normalizedTarget) || string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
                && !string.IsNullOrEmpty(targetMap)
                && !IsSoloOnlyMap(targetMap)
            )
            {
                Core.Join(targetMap);
                Bot.Wait.ForTrue(() => Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget), 20);

                // In XP/CP level-bracketed farms (where /goto is often blocked on /battlegrounde or private rooms),
                // a higher-level helper may need to step down (or up to icestormunder if Lv 75+) to find the teammate.
                if (!Bot.Map.PlayerExists(normalizedTarget) && IsLevelBracketedMap(targetMap))
                {
                    string normalizedTargetMap = NormalizeMapKey(targetMap) ?? string.Empty;
                    string[] fallbackMaps = normalizedTargetMap switch
                    {
                        "icestormunder" => new[] { "battlegrounde", "icestormarena" },
                        "battlegrounde" => new[] { "icestormarena" },
                        "icestormarena" when Bot.Player.Level >= 75 => new[] { "icestormunder", "battlegrounde" },
                        "icestormarena" when Bot.Player.Level >= 61 => new[] { "battlegrounde" },
                        _ => Array.Empty<string>(),
                    };

                    foreach (string fallbackMap in fallbackMaps)
                    {
                        if (Bot.ShouldExit || Bot.Map.PlayerExists(normalizedTarget))
                            break;
                        Core.Join(fallbackMap);
                        Bot.Wait.ForTrue(() => Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget), 20);
                    }
                }
            }
            string? loadedMap = Bot.Map?.Name ?? targetMap;
            EnsureMapAccessBypass(loadedMap);
            if (string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
            {
                // Never strand a helper in /whitemap (e.g. if Goto chased a teammate's 1s SaveState).
                Bot.Sleep(500);
                return;
            }
            if (Bot.Map.TryGetPlayer(normalizedTarget, out var playerAfterGoto) && playerAfterGoto != null)
            {
                failedGotoCounts.Remove(normalizedTarget);
                inSameCell = JumpToPlayerInSameMap(
                    normalizedTarget,
                    playerAfterGoto.Cell,
                    playerAfterGoto.Pad,
                    loadedMap
                );
            }
            else
            {
                int fails = failedGotoCounts.TryGetValue(normalizedTarget, out int prev) ? prev + 1 : 1;
                failedGotoCounts[normalizedTarget] = fails;

                int mapEntryRequiredLevel = GetRequiredLevelForMapCell(targetMap, "r2");
                bool isWholeMapLevelLocked =
                    !string.Equals(NormalizeMapKey(targetMap), "icestormarena", StringComparison.OrdinalIgnoreCase)
                    && IsLevelBracketedMap(targetMap)
                    && mapEntryRequiredLevel > 0
                    && Bot.Player.Level < mapEntryRequiredLevel;

                // Only sleep early when the local player is actually below the target map's level gate.
                // Higher-level helpers following a lower-level teammate into /icestormarena (e.g. Lv 65 -> Lv 61)
                // must wait for the teammate to finish equipping/joining instead of sleeping after 2 quick Gotos.
                int failThreshold = isWholeMapLevelLocked ? 2 : 12;
                if (fails >= failThreshold && unreachableHelperTargets.Add(normalizedTarget))
                {
                    Core.Logger(
                        $"Could not reach teammate '{normalizedTarget}' after {fails} Goto attempts (different level-locked map or private instance); putting helper in sleep mode for '{normalizedTarget}'.",
                        "CoordinateFollowAndCombat"
                    );
                }
                else
                {
                    Bot.Sleep(1000);
                }
            }
        }

        if (!inSameCell)
        {
            if (!copyHelpedActions)
                EnsureSeaVoiceHelperProtection(false);
            return;
        }

        // Only equip/swap helper role class when there are actually live monsters in the target's cell.
        if (Bot.Monsters.CurrentMonsters.Any(m => m.Alive))
        {
            if (isDoomKittenFight)
            {
                if (HasClassInInventory("DragonSoul Shinobi"))
                    SwitchClassOutsideCombatRoom("DragonSoul Shinobi");
                else if (HasClassInInventory("Blaze Binder"))
                    SwitchClassOutsideCombatRoom("Blaze Binder");
                else if (HasClassInInventory("StoneCrusher"))
                    SwitchClassOutsideCombatRoom("StoneCrusher");
                else
                    EnsureHelperRoleClassEquipped(compositionType);
            }
            else
            {
                EnsureHelperRoleClassEquipped(compositionType);
            }
        }

        if (copyHelpedActions)
        {
            Bot.Map.TryGetPlayer(normalizedTarget, out var syncedTargetPlayer);
            HandleCopyHelpedActions(syncedTargetPlayer);
        }
        else
        {
            EnsureSeaVoiceHelperProtection(false);
        }

        if (useSkillRotations && !_helperSeaVoiceChargeDetected)
        {
            EnsureSkillRotationActive();
        }

        if (!Bot.Combat.StopAttacking)
        {
            string currentClassName = Bot.Player.CurrentClass?.Name ?? string.Empty;
            bool isSupportHelper = IsSupportClassName(currentClassName);

            var aliveInCell = Bot.Monsters.CurrentMonsters.Where(m => m.Alive).ToList();
            string mapName = Bot.Map.Name?.ToLowerInvariant() ?? string.Empty;

            // Boss shield mechanics:
            // - escherion: MapID 3 is invulnerable while Staff of Inversion (MapID 2) is alive (State == 2)
            // - stalagbite: Vath (MapID 7) is shielded while Stalagbite (MapID 8) is in State 1 or 2
            var escherion = mapName == "escherion" ? aliveInCell.FirstOrDefault(m => m.MapID == 3) : null;
            var staff = mapName == "escherion" ? aliveInCell.FirstOrDefault(m => m.MapID == 2) : null;
            var vath = mapName == "stalagbite" ? aliveInCell.FirstOrDefault(m => m.MapID == 7) : null;
            var stalagbite = mapName == "stalagbite" ? aliveInCell.FirstOrDefault(m => m.MapID == 8) : null;

            Skua.Core.Models.Monsters.Monster? targetMonster = null;
            if (escherion != null && escherion.State == 2 && staff != null)
            {
                targetMonster = !waitForFirstHit || staff.HP < staff.MaxHP ? staff : null;
            }
            else if (stalagbite != null && stalagbite.State is 1 or 2)
            {
                targetMonster = !waitForFirstHit || stalagbite.HP < stalagbite.MaxHP ? stalagbite : null;
            }
            else
            {
                var eligibleMonsters = aliveInCell.Where(m =>
                    string.IsNullOrEmpty(monster) || monster == "*"
                    || m.Name.Equals(monster, StringComparison.OrdinalIgnoreCase)
                    || m.MapID.ToString() == monster
                );

                // Wait for helped player to land first attack (HP dropped below MaxHP)
                targetMonster = waitForFirstHit
                    ? eligibleMonsters.FirstOrDefault(m => m.MaxHP > 1 ? m.HP < m.MaxHP : true)
                    : eligibleMonsters.FirstOrDefault();
            }

            if (targetMonster != null)
            {
                Bot.Combat.Attack(targetMonster);
            }
            else
            {
                if (Bot.Player.HasTarget)
                {
                    Bot.Combat.CancelTarget();
                }
            }
        }
    }

    private string? ResolveCompositionHelperClass(ClassType compositionType)
    {
        if (compositionType == ClassType.Farm)
        {
            if (
                !string.IsNullOrWhiteSpace(Core.FarmClass)
                && !CoreBots.IsGenericClass(Core.FarmClass)
                && HasClassInInventory(Core.FarmClass)
            )
            {
                return Core.FarmClass;
            }
        }

        if (
            !string.IsNullOrWhiteSpace(Core.SoloClass)
            && !CoreBots.IsGenericClass(Core.SoloClass)
            && HasClassInInventory(Core.SoloClass)
        )
        {
            return Core.SoloClass;
        }

        return null;
    }

    private void EnsureHelperRoleClassEquipped(ClassType compositionType)
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        string? baseRoleClass = ResolveCompositionHelperClass(compositionType);

        // If this account's designated role is already a support class
        // (e.g., P2=StoneCrusher, P3=ArchPaladin, P4=Lord of Order), keep it equipped.
        // During Farm objectives, finished helpers stay on their Farm composition class
        // (AoE / dedicated Farm support) instead of pinging to Solo/Support and back.
        bool roleIsSupport =
            !string.IsNullOrEmpty(baseRoleClass) && IsSupportClassName(baseRoleClass);

        string? expectedClass =
            compositionType != ClassType.Farm && actAsSupportHelper && !roleIsSupport
                ? SupportHelperPriority.FirstOrDefault(HasClassInInventory) ?? baseRoleClass
                : baseRoleClass;

        if (
            string.IsNullOrWhiteSpace(expectedClass)
            || CoreBots.IsGenericClass(expectedClass)
            || string.Equals(currentClass, expectedClass, StringComparison.OrdinalIgnoreCase)
        )
        {
            return;
        }

        SwitchClassOutsideCombatRoom(expectedClass);
    }

    private void SwitchClassOutsideCombatRoom(string targetClass)
    {
        if (
            string.IsNullOrWhiteSpace(targetClass)
            || CoreBots.IsGenericClass(targetClass)
            || Bot.ShouldExit
        )
            return;

        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (string.Equals(currentClass, targetClass, StringComparison.OrdinalIgnoreCase))
            return;

        if (!HasClassInInventory(targetClass))
        {
            Core.Logger(
                $"Skipping class switch to '{targetClass}' because it is not in inventory.",
                "CoreArmyFJ"
            );
            return;
        }

        // 1. Stop skills, cancel target, and disable aggro immediately so we stop generating combat actions.
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

        // 2. Prefer jumping to the safe "Enter" cell on the current map instead of forcing /whitemap.
        if (Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0)
        {
            Core.JumpWait();
            Bot.Wait.ForCombatExit(20);
        }

        // Only fall back to /whitemap if the current cell still has monsters or combat did not exit.
        string currentMap = Bot.Map?.Name ?? string.Empty;
        if (
            !string.Equals(currentMap, "whitemap", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(currentMap, "battleon", StringComparison.OrdinalIgnoreCase)
            && (Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0)
        )
        {
            Core.Join("whitemap");
            Bot.Wait.ForMapLoad("whitemap");
            Core.JumpWait();
            Bot.Wait.ForCombatExit(20);
        }

        // 3. Ensure the class is enhanced before equipping (an unenhanced class has 0 stats and fails RankUp/Combat).
        // Pass ForceEnh: true so CoreAdvanced enhances only the unenhanced class and returns before touching equipped Forge/Awe gear.
        if (
            Bot.Inventory.TryGetItem(targetClass, out InventoryItem? classItem)
            && classItem != null
            && classItem.EnhancementLevel <= 0
        )
        {
            Adv.SmartEnhance(targetClass, ForceEnh: true);
        }

        // 4. Equip the class and start its Advanced Skills rotation.
        Core.Equip(targetClass);
        Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(targetClass), 20);

        StartClassSkills(targetClass);
    }

    private static bool IsSupportClassName(string? className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return false;

        return className.Equals("Oracle", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Healer", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Healer (Rare)", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Evolved Shaman", StringComparison.OrdinalIgnoreCase)
            || className.Equals("StoneCrusher", StringComparison.OrdinalIgnoreCase)
            || className.Equals("ArchPaladin", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Lord of Order", StringComparison.OrdinalIgnoreCase);
    }

    public void StartClassSkills(string? className = null)
    {
        string resolvedClass = string.IsNullOrWhiteSpace(className)
            ? (Bot.Player.CurrentClass?.Name ?? "generic")
            : className.Trim();

        bool inDoomKitten = string.Equals(NormalizeMapKey(Bot.Map?.Name), "doomkitten", StringComparison.OrdinalIgnoreCase);
        bool isDoomKittenShinobi =
            inDoomKitten
            && (
                string.Equals(resolvedClass, "DragonSoul Shinobi", StringComparison.OrdinalIgnoreCase)
                || string.Equals(resolvedClass, "Shadow Dragon Shinobi", StringComparison.OrdinalIgnoreCase)
            );
        bool isDoomKittenBlaze =
            inDoomKitten
            && (
                string.Equals(resolvedClass, "Blaze Binder", StringComparison.OrdinalIgnoreCase)
                || string.Equals(resolvedClass, "FireLord Summoner", StringComparison.OrdinalIgnoreCase)
            );
        bool isDoomKittenSC =
            inDoomKitten
            && string.Equals(resolvedClass, "StoneCrusher", StringComparison.OrdinalIgnoreCase);

        if (isDoomKittenShinobi)
        {
            Bot.Skills.StartAdvanced("4 | 1 | 3M<30 | 2H<30");
            lastSkillRotationClass = $"{resolvedClass}:doomkitten";
            return;
        }
        if (isDoomKittenBlaze)
        {
            Bot.Skills.StartAdvanced("3 | 4 | 2");
            lastSkillRotationClass = $"{resolvedClass}:doomkitten";
            return;
        }
        if (isDoomKittenSC)
        {
            Bot.Skills.StartAdvanced("3 | 1 | 4");
            lastSkillRotationClass = $"{resolvedClass}:doomkitten";
            return;
        }

        // Bard and Arachnomancer default to WaitForCooldown in AdvancedSkills.json, which can stall
        // on a skill slot (leaving the account on auto-attacks only). Force UseIfAvailable rotations.
        if (string.Equals(resolvedClass, "Bard", StringComparison.OrdinalIgnoreCase))
        {
            Bot.Skills.StartAdvanced("1 | 4 | 2 | 3");
            lastSkillRotationClass = resolvedClass;
            return;
        }
        if (string.Equals(resolvedClass, "Arachnomancer", StringComparison.OrdinalIgnoreCase))
        {
            Bot.Skills.StartAdvanced("4 | 1 | 3 | 2");
            lastSkillRotationClass = resolvedClass;
            return;
        }

        Bot.Skills.StartAdvanced(resolvedClass, false, ClassUseMode.Base);
        lastSkillRotationClass = resolvedClass;
    }

    public void EnsureSkillRotationActive()
    {
        try
        {
            string currentClassName = Bot.Player.CurrentClass?.Name ?? "generic";
            bool inDoomKitten = string.Equals(NormalizeMapKey(Bot.Map?.Name), "doomkitten", StringComparison.OrdinalIgnoreCase);
            bool isDoomKittenOverride =
                inDoomKitten
                && (
                    string.Equals(currentClassName, "DragonSoul Shinobi", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentClassName, "Shadow Dragon Shinobi", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentClassName, "Blaze Binder", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentClassName, "FireLord Summoner", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentClassName, "StoneCrusher", StringComparison.OrdinalIgnoreCase)
                );

            string rotationKey = isDoomKittenOverride
                ? $"{currentClassName}:doomkitten"
                : currentClassName;

            if (
                !Bot.Skills.TimerRunning
                || !string.Equals(lastSkillRotationClass, rotationKey, StringComparison.OrdinalIgnoreCase)
            )
            {
                StartClassSkills(currentClassName);
            }
        }
        catch
        {
            // Ignore if skill timer is not ready
        }
    }

    public bool SendArmySignal(string signal)
    {
        if (!RequireArmySession())
            return false;

        string[] lines = ReadArmyLines();
        if (!CanContinueArmySync(lines))
            return false;

        string signalName = NormalizeArmyRecordName(signal);
        if (string.IsNullOrEmpty(signalName))
        {
            Core.Logger("Army signal name is invalid.", "CoreArmyFJ");
            return false;
        }

        return AppendArmyRecord(
            new[]
            {
                "SIGNAL",
                ArmyProtocolVersion,
                armySessionId,
                signalName,
                armyUsername,
            }
        );
    }

    public bool HasArmySignal(string signal, int senderPlayerNumber)
    {
        if (!RequireArmySession())
            return false;

        string signalName = NormalizeArmyRecordName(signal);
        if (string.IsNullOrEmpty(signalName))
        {
            Core.Logger("Army signal name is invalid.", "CoreArmyFJ");
            return false;
        }

        if (senderPlayerNumber < 1 || senderPlayerNumber > armyPlayers.Length)
        {
            Core.Logger("Army signal sender player number is invalid.", "CoreArmyFJ");
            return false;
        }

        string[] lines = ReadArmyLines();
        if (!CanContinueArmySync(lines))
            return false;

        string expectedSender = armyPlayers[senderPlayerNumber - 1];
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length == 5
                && parts[0] == "SIGNAL"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
                && IsArmyRecordName(parts[3])
                && string.Equals(parts[3], signalName, StringComparison.Ordinal)
                && string.Equals(
                    NormalizeArmyUsername(parts[4]),
                    expectedSender,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return true;
        }

        return false;
    }


    public int ArmyPlayerNumber =>
        armyInitialized && armyPlayerIndex >= 0 ? armyPlayerIndex + 1 : 0;

    public bool IsArmyPlayer(int playerNumber) =>
        armyInitialized
        && playerNumber >= 1
        && playerNumber <= armyPlayers.Length
        && armyPlayerIndex == playerNumber - 1;

    public bool StopArmySync(string reason = "")
    {
        if (!RequireArmySession())
            return false;

        if (!CanContinueArmySync(ReadArmyLines()))
            return false;

        if (armyPlayerIndex != 0)
        {
            Core.Logger("Only playerOne may stop Army sync.", "CoreArmyFJ");
            return false;
        }

        if (!IsSafeArmyField(reason))
        {
            Core.Logger("Army stop reason contains invalid data.", "CoreArmyFJ");
            return false;
        }

        return AppendArmyRecord(
            new[] { "STOP", ArmyProtocolVersion, armySessionId, reason }
        );
    }

    private bool StartLeaderArmySession()
    {
        if (!ClearArmyFile())
            return false;

        if (
            !AppendArmyRecord(new[] { "RESET", ArmyProtocolVersion, armySessionId })
            || !WriteArmyReady(armySessionId)
        )
            return false;

        Dictionary<string, string>? launchTokens = WaitForArmyReady(armySessionId);
        if (launchTokens == null)
            return false;

        List<string> start = new() { "START", ArmyProtocolVersion, armySessionId };
        start.AddRange(armyPlayers.Select(player => launchTokens[player]));
        return AppendArmyRecord(start);
    }

    private bool JoinFollowerArmySession()
    {
        Core.Logger("Waiting for playerOne to start Army sync.", "CoreArmyFJ");
        string readySession = string.Empty;

        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            string[] lines = ReadArmyLines();
            string? sessionId = FindLatestArmyReset(lines);
            if (sessionId == null)
            {
                Bot.Sleep(ArmyPollDelay);
                continue;
            }

            if (!string.Equals(readySession, sessionId, StringComparison.Ordinal))
            {
                if (!WriteArmyReady(sessionId))
                    return false;

                readySession = sessionId;
            }

            if (HasMatchingArmyStart(lines, sessionId))
            {
                if (HasArmyStop(lines, sessionId))
                    return false;

                armySessionId = sessionId;
                return true;
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return false;
    }

    private bool WriteArmyReady(string sessionId) =>
        AppendArmyRecord(
            new[] { "READY", ArmyProtocolVersion, sessionId, armyUsername, armyLaunchToken }
        );

    private Dictionary<string, string>? WaitForArmyReady(string sessionId)
    {
        string previousMissing = string.Empty;
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            Dictionary<string, string> tokens = new(StringComparer.OrdinalIgnoreCase);
            foreach (string line in ReadArmyLines())
            {
                string[] parts = line.Split('|');
                if (
                    parts.Length != 5
                    || parts[0] != "READY"
                    || parts[1] != ArmyProtocolVersion
                    || !string.Equals(parts[2], sessionId, StringComparison.Ordinal)
                )
                    continue;

                string username = NormalizeArmyUsername(parts[3]);
                if (
                    armyPlayers.Contains(username, StringComparer.OrdinalIgnoreCase)
                    && Guid.TryParseExact(parts[4], "N", out _)
                )
                    tokens[username] = parts[4];
            }

            if (armyPlayers.All(tokens.ContainsKey))
                return tokens;

            string missing = LogArmyNames(
                armyPlayers.Where(player => !tokens.ContainsKey(player))
            );
            if (!string.Equals(missing, previousMissing, StringComparison.Ordinal))
            {
                Core.Logger($"Waiting for Army players: {missing}.", "CoreArmyFJ");
                previousMissing = missing;
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return null;
    }

    private bool HasMatchingArmyStart(string[] lines, string sessionId)
    {
        for (int index = lines.Length - 1; index >= 0; index--)
        {
            string[] parts = lines[index].Split('|');
            if (
                parts.Length != 3 + armyPlayers.Length
                || parts[0] != "START"
                || parts[1] != ArmyProtocolVersion
                || !string.Equals(parts[2], sessionId, StringComparison.Ordinal)
            )
                continue;

            string[] tokens = parts.Skip(3).ToArray();
            return tokens.All(token => Guid.TryParseExact(token, "N", out _))
                && string.Equals(
                    tokens[armyPlayerIndex],
                    armyLaunchToken,
                    StringComparison.Ordinal
                );
        }

        return false;
    }

    private bool RunLeaderArmySync(string stepName)
    {
        long stepId = nextArmyStepId;
        if (
            !AppendArmyRecord(
                new[]
                {
                    "STEP",
                    ArmyProtocolVersion,
                    armySessionId,
                    stepId.ToString(),
                    stepName,
                }
            )
        )
            return false;

        nextArmyStepId++;
        if (!WriteArmyArrived(stepId))
            return false;

        string previousMissing = string.Empty;
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            string[] lines = ReadArmyLines();
            if (!CanContinueArmySync(lines))
                return false;

            HashSet<string> arrived = GetArmyArrivals(lines, stepId);
            if (armyPlayers.All(arrived.Contains))
            {
                if (
                    !AppendArmyRecord(
                        new[]
                        {
                            "CONTINUE",
                            ArmyProtocolVersion,
                            armySessionId,
                            stepId.ToString(),
                        }
                    )
                )
                    return false;

                lastArmyStepId = stepId;
                return true;
            }

            string missing = LogArmyNames(
                armyPlayers.Where(player => !arrived.Contains(player))
            );
            if (!string.Equals(missing, previousMissing, StringComparison.Ordinal))
            {
                Core.Logger($"Waiting for Army players: {missing}.", "CoreArmyFJ");
                previousMissing = missing;
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return false;
    }

    private bool RunFollowerArmySync(string expectedStepName)
    {
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            string[] lines = ReadArmyLines();
            if (!CanContinueArmySync(lines))
                return false;

            if (!TryFindNextArmyStep(lines, out long stepId, out string stepName))
            {
                Bot.Sleep(ArmyPollDelay);
                continue;
            }

            if (!string.Equals(stepName, expectedStepName, StringComparison.Ordinal))
            {
                Core.Logger("Army sync step does not match playerOne.", "CoreArmyFJ");
                return false;
            }

            if (!WriteArmyArrived(stepId))
                return false;

            while (!Bot.ShouldExit && !armyTransportFailed)
            {
                string[] continueLines = ReadArmyLines();
                if (!CanContinueArmySync(continueLines))
                    return false;

                if (HasArmyContinue(continueLines, stepId))
                {
                    lastArmyStepId = stepId;
                    return true;
                }

                Bot.Sleep(ArmyPollDelay);
            }
        }

        return false;
    }

    private bool WriteArmyArrived(long stepId) =>
        AppendArmyRecord(
            new[]
            {
                "ARRIVED",
                ArmyProtocolVersion,
                armySessionId,
                stepId.ToString(),
                armyUsername,
            }
        );

    private HashSet<string> GetArmyArrivals(string[] lines, long stepId)
    {
        HashSet<string> arrived = new(StringComparer.OrdinalIgnoreCase);
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length != 5
                || parts[0] != "ARRIVED"
                || parts[1] != ArmyProtocolVersion
                || !string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
                || !long.TryParse(parts[3], out long recordStepId)
                || recordStepId != stepId
            )
                continue;

            string username = NormalizeArmyUsername(parts[4]);
            if (armyPlayers.Contains(username, StringComparer.OrdinalIgnoreCase))
                arrived.Add(username);
        }

        return arrived;
    }

    private bool TryFindNextArmyStep(
        string[] lines,
        out long nextStepId,
        out string nextStepName
    )
    {
        nextStepId = long.MaxValue;
        nextStepName = string.Empty;

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length != 5
                || parts[0] != "STEP"
                || parts[1] != ArmyProtocolVersion
                || !string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
                || !long.TryParse(parts[3], out long stepId)
                || stepId <= lastArmyStepId
                || stepId >= nextStepId
                || !IsArmyRecordName(parts[4])
            )
                continue;

            nextStepId = stepId;
            nextStepName = parts[4];
        }

        if (nextStepId != long.MaxValue)
            return true;

        nextStepId = 0;
        return false;
    }

    private bool HasArmyContinue(string[] lines, long stepId)
    {
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length == 4
                && parts[0] == "CONTINUE"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
                && long.TryParse(parts[3], out long recordStepId)
                && recordStepId == stepId
            )
                return true;
        }

        return false;
    }

    private bool CanContinueArmySync(string[] lines)
    {
        string? latestSessionId = FindLatestArmyReset(lines);
        if (
            latestSessionId != null
            && !string.Equals(latestSessionId, armySessionId, StringComparison.Ordinal)
        )
        {
            if (!armySessionFailureLogged)
            {
                Core.Logger(
                    "Army sync session was replaced.",
                    "CoreArmyFJ",
                    messageBox: true,
                    stopBot: true
                );
                armySessionFailureLogged = true;
            }

            return false;
        }

        if (HasArmyStop(lines, armySessionId))
        {
            if (!armyStopLogged)
            {
                Core.Logger("Army sync was stopped by playerOne.", "CoreArmyFJ");
                armyStopLogged = true;
            }

            return false;
        }

        return true;
    }

    private static string? FindLatestArmyReset(string[] lines)
    {
        for (int index = lines.Length - 1; index >= 0; index--)
        {
            string[] parts = lines[index].Split('|');
            if (
                parts.Length == 3
                && parts[0] == "RESET"
                && parts[1] == ArmyProtocolVersion
                && Guid.TryParseExact(parts[2], "N", out _)
            )
                return parts[2];
        }

        return null;
    }

    private static bool HasArmyStop(string[] lines, string sessionId)
    {
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length == 4
                && parts[0] == "STOP"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], sessionId, StringComparison.Ordinal)
                && IsSafeArmyField(parts[3])
            )
                return true;
        }

        return false;
    }

    private bool AppendArmyRecord(IReadOnlyList<string> fields)
    {
        if (fields.Count == 0 || fields.Any(field => !IsSafeArmyField(field)))
        {
            Core.Logger("Army sync record contains invalid data.", "CoreArmyFJ");
            return false;
        }

        string line = string.Join("|", fields);
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            try
            {
                using FileStream stream = new(
                    armySyncPath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read
                );
                using StreamWriter writer = new(stream, new UTF8Encoding(false));
                writer.WriteLine(line);
                LogArmyRecord(fields);
                return true;
            }
            catch (IOException)
            {
                Bot.Sleep(ArmyFileRetryDelay);
            }
            catch (Exception ex)
            {
                SetArmyTransportFailure(ex);
            }
        }

        return false;
    }

    private string[] ReadArmyLines()
    {
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            try
            {
                return File.Exists(armySyncPath)
                    ? File.ReadAllLines(armySyncPath)
                    : Array.Empty<string>();
            }
            catch (IOException)
            {
                Bot.Sleep(ArmyFileRetryDelay);
            }
            catch (Exception ex)
            {
                SetArmyTransportFailure(ex);
            }
        }

        return Array.Empty<string>();
    }

    private bool ClearArmyFile()
    {
        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            try
            {
                using FileStream stream = new(
                    armySyncPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read
                );
                return true;
            }
            catch (IOException)
            {
                Bot.Sleep(ArmyFileRetryDelay);
            }
            catch (Exception ex)
            {
                SetArmyTransportFailure(ex);
            }
        }

        return false;
    }

    private void LogArmyRecord(IReadOnlyList<string> fields)
    {
        switch (fields[0])
        {
            case "RESET":
                Core.Logger("Army sync reset.", "CoreArmyFJ");
                break;
            case "START":
                Core.Logger("Army sync started.", "CoreArmyFJ");
                break;
            case "STEP" when fields.Count > 4:
                Core.Logger($"Army sync step {fields[4]}.", "CoreArmyFJ");
                break;
            case "CONTINUE":
                Core.Logger("Army sync continuing.", "CoreArmyFJ");
                break;
            case "STOP":
                Core.Logger("Army sync stop sent.", "CoreArmyFJ");
                break;
        }
    }

    private void SetArmyTransportFailure(Exception exception)
    {
        armyTransportFailed = true;
        Bot.Log($"CoreArmyFJ Army sync file access failed: {exception}");
        if (!armyTransportFailureLogged)
        {
            Core.Logger(
                "Army sync file access failed.",
                "CoreArmyFJ",
                messageBox: true,
                stopBot: true
            );
            armyTransportFailureLogged = true;
        }
    }

    private bool RequireArmySession()
    {
        if (!armyInitialized)
        {
            Core.Logger(
                "Army sync has not been initialized.",
                "CoreArmyFJ",
                messageBox: true,
                stopBot: true
            );
            return false;
        }

        if (armySessionStarted && !string.IsNullOrEmpty(armySessionId))
            return true;

        Core.Logger(
            "Army sync session has not started.",
            "CoreArmyFJ",
            messageBox: true,
            stopBot: true
        );
        return false;
    }

    private bool ArmyInitializationFailure(string message)
    {
        Core.Logger(
            message,
            "CoreArmyFJ",
            messageBox: true,
            stopBot: true
        );
        return false;
    }

    private string LogArmyNames(IEnumerable<string> usernames) =>
        string.Join(", ", usernames.Select(LogArmyName));

    private string LogArmyName(string username)
    {
        string normalized = NormalizeArmyUsername(username);
        int index = Array.FindIndex(
            armyPlayers,
            player => string.Equals(player, normalized, StringComparison.OrdinalIgnoreCase)
        );
        return index >= 0 && index < ArmyAliases.Length ? ArmyAliases[index] : "unknownPlayer";
    }

    private void ResetArmyState()
    {
        armyPlayers = Array.Empty<string>();
        armyUsername = string.Empty;
        armySyncPath = string.Empty;
        armyLaunchToken = string.Empty;
        armySessionId = string.Empty;
        armyPlayerIndex = -1;
        nextArmyStepId = 1;
        lastArmyStepId = 0;
        armyInitialized = false;
        armySessionStarted = false;
        armySessionFailureLogged = false;
        armyStopLogged = false;
        armyTransportFailed = false;
        armyTransportFailureLogged = false;
    }

    private static string NormalizeArmyUsername(string? username) =>
        username?.Trim().ToLowerInvariant() ?? string.Empty;

    private static string NormalizeArmyRecordName(string? name)
    {
        string normalized = name?.Trim().ToUpperInvariant() ?? string.Empty;
        return IsArmyRecordName(normalized) ? normalized : string.Empty;
    }

    private static bool IsArmyRecordName(string name) =>
        name.Length > 0
        && name.All(character =>
            character is >= 'A' and <= 'Z'
            || character is >= '0' and <= '9'
            || character == '_'
        );

    private static bool IsSafeArmyField(string? value) =>
        value != null
        && !value.Contains('|')
        && !value.Contains('\r')
        && !value.Contains('\n')
        && !value.Contains('\0');

    public bool AllPlayersSatisfy(string objectiveName, Func<bool> localPredicate)
    {
        if (!armySessionStarted || armyPlayers.Length == 0 || string.IsNullOrWhiteSpace(armySyncPath))
            return localPredicate();

        string safeName = NormalizeArmyRecordName(objectiveName);
        if (string.IsNullOrEmpty(safeName))
            safeName = "FLAG";

        string flagPrefix = $"{armySyncPath}.{safeName}.{armySessionId}";
        string myFlagPath = $"{flagPrefix}.p{armyPlayerIndex + 1}";

        try
        {
            File.WriteAllText(myFlagPath, localPredicate() ? "1" : "0");
        }
        catch { }

        if (!SyncArmy($"{safeName}_FLAG_READY"))
            return false;

        bool allTrue = true;
        for (int i = 1; i <= armyPlayers.Length; i++)
        {
            string pPath = $"{flagPrefix}.p{i}";
            try
            {
                string val = File.Exists(pPath) ? File.ReadAllText(pPath).Trim() : "0";
                if (val != "1")
                {
                    allTrue = false;
                    break;
                }
            }
            catch
            {
                allTrue = false;
                break;
            }
        }

        SyncArmy($"{safeName}_FLAG_DONE");

        try
        {
            if (File.Exists(myFlagPath))
                File.Delete(myFlagPath);
        }
        catch { }

        return allTrue;
    }

    /// <summary>
    /// Cancels combat targets and moves to a safe cell so quest turn-ins are not rejected for being in combat.
    /// </summary>
    public void ExitCombatForSafeTurnIn()
    {
        if (Bot.Player.HasTarget)
            Bot.Combat.CancelTarget();

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        if (Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0)
        {
            Core.JumpWait();
            Bot.Wait.ForCombatExit(20);
        }
    }

    /// <summary>
    /// Exits combat before completing the given quests via <see cref="CoreBots.EnsureComplete(int[])"/>.
    /// </summary>
    public void SafeEnsureComplete(params int[] questIds)
    {
        if (questIds == null || questIds.Length == 0)
            return;

        ExitCombatForSafeTurnIn();
        Core.EnsureComplete(questIds);
    }

    /// <summary>
    /// Turns in any completable quests in <paramref name="questIds"/> after exiting combat,
    /// avoiding in-combat rejections and temp-item desync loops.
    /// </summary>
    public bool CompleteCompletableQuestsOutOfCombat(int[] questIds, bool reaccept = true)
    {
        if (questIds == null || questIds.Length == 0)
            return false;

        int[] completable = questIds
            .Where(id => id > 0 && Bot.Quests.IsInProgress(id) && Bot.Quests.CanComplete(id))
            .Distinct()
            .ToArray();

        if (completable.Length == 0)
            return false;

        ExitCombatForSafeTurnIn();

        foreach (int questId in completable)
        {
            if (Bot.ShouldExit)
                break;

            Core.EnsureComplete(questId);
            Bot.Sleep(Core.ActionDelay);

            if (Bot.Quests.IsInProgress(questId))
            {
                Core.Logger(
                    $"SafeTurnIn: Quest {questId} remained in progress after out-of-combat turn-in; resetting quest state."
                );
                Bot.Flash.CallGameFunction("world.abandonQuest", questId);
                Bot.Sleep(Core.ActionDelay);
                Core.EnsureLoad(questId);
            }
        }

        if (reaccept && !Bot.ShouldExit)
        {
            int[] toAccept = questIds
                .Where(id => id > 0 && !Bot.Quests.IsInProgress(id))
                .Distinct()
                .ToArray();
            if (toAccept.Length > 0)
                Core.EnsureAcceptmultiple(toAccept);
        }

        return true;
    }

    /// <summary>
    /// Farms monsters for repeatable quests without background turn-in spam,
    /// exiting combat cleanly whenever any registered quest becomes completable.
    /// </summary>
    public void RunSafeQuestFarm(
        string map,
        string cell,
        string pad,
        string monster,
        int[] questIds,
        Func<bool> stopCondition
    )
    {
        if (questIds == null || questIds.Length == 0)
            return;

        Core.EnsureAcceptmultiple(questIds);

        while (!Bot.ShouldExit && !stopCondition())
        {
            if (Core.CheckSaveState())
                Core.ExecuteSaveState();

            if (CompleteCompletableQuestsOutOfCombat(questIds, reaccept: true) && stopCondition())
                break;

            int[] missingAccept = questIds
                .Where(id => id > 0 && !Bot.Quests.IsInProgress(id))
                .Distinct()
                .ToArray();
            if (missingAccept.Length > 0)
                Core.EnsureAcceptmultiple(missingAccept);

            Core.KillMonster(map, cell, pad, monster, log: false);
        }

        CompleteCompletableQuestsOutOfCombat(questIds, reaccept: false);
    }
}

