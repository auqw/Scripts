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

    // Recovery: a restarted account (relogin / manual restart) asks for a REJOIN, the leader
    // re-creates the session and every account restarts its run from the top (skip gates make
    // that fast). A FAIL record stops the whole Army with the reason instead of hanging forever.
    private bool armyRestartRequested;
    private bool armyFailureLogged;
    private string armyIgnoredSessionId = string.Empty;
    private System.Threading.Timer? armyHeartbeatTimer;
    private const int ArmyHeartbeatIntervalMs = 10000;
    private const int ArmyHeartbeatStaleSeconds = 60;
    private const int ArmyStaleWarningIntervalSeconds = 120;
    private readonly Dictionary<string, DateTime> armyStaleWarnedAt = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>True when the current run ended because the Army session must be re-created.</summary>
    public bool RestartRequested => armyRestartRequested;

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

        // After a restart request, never rejoin the session we just left.
        string ignoredSession = armyRestartRequested ? armySessionId : string.Empty;
        StopArmyHeartbeat();
        ResetArmyState();
        armyIgnoredSessionId = ignoredSession;

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

        StartArmyHeartbeat();
        bool started = armyPlayerIndex == 0
            ? StartLeaderArmySession()
            : JoinFollowerArmySession();
        armySessionStarted = started;
        return started;
    }

    /// <summary>Stops background work owned by the Army session. Call from the script's finally block.</summary>
    public void EndArmySession() => StopArmyHeartbeat();

    /// <summary>
    /// Tells every account that this one cannot continue, so nobody waits forever at a barrier.
    /// Ignored when the session is already stopping, failed, or being restarted.
    /// </summary>
    public void ReportArmyFailure(string reason)
    {
        if (!armySessionStarted || string.IsNullOrEmpty(armySessionId) || armyRestartRequested || armyTransportFailed)
            return;

        string[] lines = ReadArmyLines();
        if (FindLatestArmyReset(lines) != armySessionId || HasArmyStop(lines, armySessionId) || FindArmyFailure(lines) != null)
            return;

        string safeReason = new string(
            (reason ?? string.Empty).Select(c => c == '|' || c == '\r' || c == '\n' || c == '\0' ? ' ' : c).ToArray()
        ).Trim();
        if (safeReason.Length > 200)
            safeReason = safeReason[..200];

        AppendArmyRecord(new[] { "FAIL", ArmyProtocolVersion, armySessionId, armyUsername, safeReason });
    }

    private string HeartbeatPath(string username) => $"{armySyncPath}.HB.{username}";

    private void StartArmyHeartbeat()
    {
        StopArmyHeartbeat();
        string path = HeartbeatPath(armyUsername);
        void Beat()
        {
            try
            {
                File.WriteAllText(path, DateTime.UtcNow.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            catch { }
        }

        Beat();
        armyHeartbeatTimer = new System.Threading.Timer(
            _ =>
            {
                if (Bot.ShouldExit)
                {
                    StopArmyHeartbeat();
                    return;
                }
                Beat();
            },
            null,
            ArmyHeartbeatIntervalMs,
            ArmyHeartbeatIntervalMs
        );
    }

    private void StopArmyHeartbeat()
    {
        try
        {
            armyHeartbeatTimer?.Dispose();
        }
        catch { }
        armyHeartbeatTimer = null;
    }

    private double? GetHeartbeatAgeSeconds(string username)
    {
        try
        {
            string path = HeartbeatPath(username);
            if (!File.Exists(path))
                return null;
            if (!long.TryParse(File.ReadAllText(path).Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long ticks))
                return null;
            return (DateTime.UtcNow - new DateTime(ticks, DateTimeKind.Utc)).TotalSeconds;
        }
        catch
        {
            return null;
        }
    }

    // Called while waiting at a barrier: warns (rate-limited) about teammates whose bot stopped beating.
    private void WarnAboutStaleArmyPlayers(IEnumerable<string> waitingFor)
    {
        foreach (string player in waitingFor)
        {
            double? age = GetHeartbeatAgeSeconds(player);
            if (age == null || age < ArmyHeartbeatStaleSeconds)
                continue;

            if (
                armyStaleWarnedAt.TryGetValue(player, out DateTime last)
                && (DateTime.UtcNow - last).TotalSeconds < ArmyStaleWarningIntervalSeconds
            )
                continue;

            armyStaleWarnedAt[player] = DateTime.UtcNow;
            Core.Logger(
                $"{LogArmyName(player)} has not responded for {(int)age}s (bot stopped or disconnected?). "
                    + "Restart the bot on that account; the Army will resync automatically.",
                "CoreArmyFJ"
            );
        }
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
        Action? prepareForCombat = null,
        Func<int>? resolveSubgroupCap = null,
        string? targetMap = null
    ) =>
        RunCooperativeObjective(
            objectiveName,
            isComplete,
            doWork,
            (targetPlayerNumber, targetPlayerName) => help(),
            maxWorkAttempts,
            maxPlayersPerSubgroup,
            prepareForCombat,
            resolveSubgroupCap,
            targetMap
        );

    // targetMap (optional) is the map doWork farms on; StallGuard uses it to re-enter that map
    // when the account stalls (frozen client) or idles in a /house after a manual unstick.
    public bool RunCooperativeObjective(
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action<int, string> help,
        int maxWorkAttempts = 0,
        int maxPlayersPerSubgroup = 0,
        Action? prepareForCombat = null,
        Func<int>? resolveSubgroupCap = null,
        string? targetMap = null
    )
    {
        int[]? outerRoomLayout = activeRoomLayout;
        string? outerObjectiveMap = currentObjectiveMap;
        if (!string.IsNullOrWhiteSpace(targetMap) && targetMap != "*")
            currentObjectiveMap = NormalizeMapKey(targetMap);
        try
        {
            return RunCooperativeObjectiveInner(
                objectiveName,
                isComplete,
                doWork,
                help,
                maxWorkAttempts,
                maxPlayersPerSubgroup,
                prepareForCombat,
                resolveSubgroupCap,
                targetMap
            );
        }
        finally
        {
            // Nested objectives restore the layout of the objective that called them.
            activeRoomLayout = outerRoomLayout;
            currentObjectiveMap = outerObjectiveMap;
            loggedRoomWaitMode = false;
        }
    }

    private bool RunCooperativeObjectiveInner(
        string objectiveName,
        Func<bool> isComplete,
        Action doWork,
        Action<int, string> help,
        int maxWorkAttempts,
        int maxPlayersPerSubgroup,
        Action? prepareForCombat,
        Func<int>? resolveSubgroupCap,
        string? targetMap
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

        // Fast path: every step id is leader-sequential, so the START barrier's id is known in
        // advance. Accounts already done publish their DONE signal *before* arriving at START;
        // after START continues every account sees the same set of signals, so "all done" is a
        // consistent decision and the objective costs a single barrier instead of three.
        // The fast decision only looks at PRE signals, which are written exclusively before START,
        // so a teammate finishing right after START can never make accounts decide differently.
        long predictedObjectiveId = lastArmyStepId + 1;
        string preSignal = $"SIG_OBJ_{predictedObjectiveId}_{name}_PRE";
        bool doneBeforeStart = isComplete();
        if (doneBeforeStart && !SendArmySignal(preSignal))
            return false;

        if (!SyncArmy($"OBJECTIVE_{name}_START"))
            return false;

        long objectiveId = lastArmyStepId;
        string doneSignal = $"SIG_OBJ_{objectiveId}_{name}_DONE";
        string failSignal = $"SIG_OBJ_{objectiveId}_{name}_FAILED";
        bool reportedDone = false;
        int workAttempts = 0;

        if (objectiveId == predictedObjectiveId)
        {
            string[] startLines = ReadArmyLines();
            if (!CanContinueArmySync(startLines))
                return false;

            bool allDoneAtStart = true;
            for (int playerNumber = 1; playerNumber <= armyPlayers.Length && allDoneAtStart; playerNumber++)
                allDoneAtStart = HasArmySignalIn(startLines, preSignal, playerNumber);

            if (allDoneAtStart)
                return true;
        }

        if (isComplete())
        {
            if (!SendArmySignal(doneSignal))
                return false;
            reportedDone = true;
            Core.Logger($"Army objective {name}: already complete on this account before combat prep.", "CoreArmyFJ");
        }

        if (!SyncArmy($"OBJECTIVE_{name}_PREP_CHECK"))
            return false;

        string[] prepLines = ReadArmyLines();
        if (!CanContinueArmySync(prepLines))
            return false;

        List<int> prePending = new();
        for (int playerNumber = 1; playerNumber <= armyPlayers.Length; playerNumber++)
        {
            if (!HasArmySignalIn(prepLines, doneSignal, playerNumber) && !HasArmySignalIn(prepLines, failSignal, playerNumber))
                prePending.Add(playerNumber);
        }

        if (prePending.Count == 0)
            return SyncArmy($"OBJECTIVE_{name}_DONE");

        int effectiveSubgroupCap = maxPlayersPerSubgroup;
        if (resolveSubgroupCap != null)
        {
            if (armyPlayerIndex == 0)
                PublishMapSubgroupCap(name, resolveSubgroupCap());

            if (!SyncArmy($"OBJECTIVE_{name}_MAPCAP"))
                return false;

            int detectedCap = ReadMapSubgroupCap(name);
            if (detectedCap > 0)
                effectiveSubgroupCap = detectedCap;
        }

        // Room layout: pending accounts are spread first, finished accounts fill the free seats.
        // Every account computes the same initial layout from the PREP_CHECK snapshot (no one works
        // between PREP_CHECK and READY), later changes come from the layout file.
        bool useSubgroups = effectiveSubgroupCap > 0 && effectiveSubgroupCap < armyPlayers.Length;
        if (useSubgroups)
        {
            bool[] prePendingMask = new bool[armyPlayers.Length];
            foreach (int p in prePending)
                prePendingMask[p - 1] = true;
            activeRoomLayout = ComputeRoomLayout(prePendingMask, effectiveSubgroupCap, null);
            Core.Logger($"Army objective {name}: rooms {DescribeRoomLayout(activeRoomLayout)}.", "CoreArmyFJ");
        }
        else
        {
            activeRoomLayout = null;
        }

        if (prepareForCombat != null)
        {
            int localRoom = GetLayoutRoom(armyPlayerIndex + 1);
            bool roomHasPending = activeRoomLayout == null
                || (localRoom >= 0 && prePending.Any(p => activeRoomLayout[p - 1] == localRoom));

            if (roomHasPending)
                prepareForCombat();
        }

        if (!SyncArmy($"OBJECTIVE_{name}_READY"))
            return false;

        unreachableHelperTargets.Clear();
        unreachableHelperMarkedAt.Clear();
        attemptedBypassReloadCells.Clear();
        failedGotoCounts.Clear();
        failedSameMapJumpCounts.Clear();
        loggedHelperSleepMode = false;

        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            // One read of the sync file per iteration (it was read ~15 times before).
            string[] loopLines = ReadArmyLines();
            if (!CanContinueArmySync(loopLines))
                return false;

            RecordCurrentMapCap();
            LearnMapCapFromRoomFullReports();

            // Abort the whole Army if any account gave up on this objective.
            for (int playerNumber = 1; playerNumber <= armyPlayers.Length; playerNumber++)
            {
                if (HasArmySignalIn(loopLines, failSignal, playerNumber))
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
                bool playerDone = HasArmySignalIn(loopLines, doneSignal, playerNumber)
                    || (playerNumber == armyPlayerIndex + 1 && reportedDone);
                if (playerDone)
                    helperPlayers.Add(playerNumber);
                else
                    pendingPlayers.Add(playerNumber);
            }

            if (pendingPlayers.Count == 0)
                return SyncArmy($"OBJECTIVE_{name}_DONE");

            if (useSubgroups)
                RefreshRoomLayout(name, objectiveId, effectiveSubgroupCap, pendingPlayers, helperPlayers);

            if (localComplete)
            {
                // Spread helpers across the pending players of this account's room (layout-scoped).
                int localPlayerNumber = armyPlayerIndex + 1;
                int localRoom = GetLayoutRoom(localPlayerNumber);

                List<int> scopedPending = useSubgroups
                    ? pendingPlayers.Where(p => GetLayoutRoom(p) == localRoom).ToList()
                    : pendingPlayers;

                if (useSubgroups && localRoom < 0)
                {
                    // Every room with a pending teammate is full: wait outside the map.
                    actAsSupportHelper = false;
                    help(0, string.Empty);
                    WaitOutsideFullRooms(name);
                }
                else if (scopedPending.Count == 0)
                {
                    loggedRoomWaitMode = false;
                    actAsSupportHelper = false;
                    help(0, string.Empty);
                }
                else
                {
                    loggedRoomWaitMode = false;
                    ExpireUnreachableHelperTargets(name);
                    List<int> accessiblePending = scopedPending
                        .Where(p => !unreachableHelperTargets.Contains(NormalizeArmyUsername(armyPlayers[p - 1])))
                        .ToList();

                    if (accessiblePending.Count == 0)
                    {
                        actAsSupportHelper = false;
                        if (!loggedHelperSleepMode)
                        {
                            Core.Logger(
                                $"Army objective {name}: remaining teammate(s) are in higher-level, inaccessible or full rooms; waiting in sleep mode.",
                                "CoreArmyFJ"
                            );
                            loggedHelperSleepMode = true;
                        }
                        Bot.Sleep(1000);
                    }
                    else
                    {
                        List<int> scopedHelpers = useSubgroups
                            ? helperPlayers.Where(p => GetLayoutRoom(p) == localRoom).ToList()
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
                        int supportPlayerNumber = useSubgroups ? GetLayoutRoomSupportPlayer(localRoom) : 2;
                        actAsSupportHelper =
                            localPlayerNumber == supportPlayerNumber
                            || (!scopedHelpers.Contains(supportPlayerNumber) && helperIndex == 0);

                        int helpedPlayerNumber = targetPlayerNumber;
                        lastHelpTargetFinished = false;
                        lastHelpTargetCheckTick = unchecked(Environment.TickCount - 1000);
                        helpTargetFinished = () =>
                            HasArmySignal(doneSignal, helpedPlayerNumber)
                            || HasArmySignal(failSignal, helpedPlayerNumber);
                        IsHelpingTeammate = true;
                        try
                        {
                            help(targetPlayerNumber, targetPlayerName);
                        }
                        finally
                        {
                            IsHelpingTeammate = false;
                            helpTargetFinished = null;
                        }
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
                RunWorkWithStallGuard(name, doWork, targetMap);
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return false;
    }

    // StallGuard. doWork usually ends in official farm loops (Story.KillQuest,
    // HuntMonster, ...) that have no timeout: one account looping silently held the whole
    // Army for 45 min (quest 5330). While doWork runs, a background monitor watches a
    // progress signature (inventory, temp items, gold/XP/level, rep, monster HP in the
    // cell). With no progress at all for WorkStallMinutes it logs the account state and
    // nudges it (temp items and accepted quests survive a map change). With a known objective
    // map every nudge is a full re-entry (/whitemap, then the objective map): a frozen client
    // only recovers with a real map change. Without one, odd nudges jump to Enter (the farm
    // loops jump back to their target cell) and even nudges rejoin the current map.
    // From the 3rd nudge a "still stuck" warning is logged.
    // A real fight always changes monster HP, so bosses are never nudged.
    // Fast path: the usual manual unstick is "/join house". The farm loops join their map only
    // once, so the account then idled in the house forever; now, after OffMapRecoverySeconds
    // in a /house without progress, it is sent back to the objective map.
    public int WorkStallMinutes { get; set; } = 5;
    public int OffMapRecoverySeconds { get; set; } = 20;

    // Lonely overflow: a "room is full" answer moved this account into an overflow room that no
    // teammate shares (e.g. everyone was still working, so no helper follows it there). After
    // LonelyOverflowSeconds the account retries its planned room once per work attempt; if that
    // room really is full the room-full listener sends it to the overflow room again.
    public int LonelyOverflowSeconds { get; set; } = 15;

    /// <summary>Planned room before the overflow move (0 = not in overflow). Set by the script.</summary>
    public Func<int>? PlannedRoomBeforeOverflow { get; set; }

    /// <summary>Puts the planned room back into the private-room settings. Set by the script.</summary>
    public Action? RestorePlannedRoom { get; set; }

    /// <summary>Other Army accounts in the current map instance (-1 when it cannot be read).</summary>
    public int CountArmyTeammatesInMap()
    {
        try
        {
            IEnumerable<string>? names = Bot.Map.PlayerNames;
            if (names == null)
                return -1;

            HashSet<string> here = new(names.Select(NormalizeArmyUsername));
            int count = 0;
            for (int i = 0; i < armyPlayers.Length; i++)
            {
                if (i != armyPlayerIndex && here.Contains(NormalizeArmyUsername(armyPlayers[i])))
                    count++;
            }
            return count;
        }
        catch
        {
            return -1;
        }
    }

    private bool IsInLonelyOverflowRoom(string? currentMap, string objectiveMap)
    {
        int plannedRoom = PlannedRoomBeforeOverflow?.Invoke() ?? 0;
        if (plannedRoom <= 0 || !string.Equals(currentMap, objectiveMap, StringComparison.OrdinalIgnoreCase))
            return false;

        int room = GetCurrentRoomNumber();
        if (room <= 0 || room == plannedRoom)
            return false;

        return CountArmyTeammatesInMap() == 0;
    }

    private void RunWorkWithStallGuard(string objectiveName, Action doWork, string? targetMap = null)
    {
        using CancellationTokenSource stallGuardCts = new();
        Thread stallGuard = new(() => WatchWorkForStalls(objectiveName, targetMap, stallGuardCts.Token))
        {
            IsBackground = true,
            Name = $"ArmyStallGuard-{objectiveName}",
        };
        stallGuard.Start();
        try
        {
            doWork();
        }
        finally
        {
            stallGuardCts.Cancel();
            stallGuard.Join(3000);
        }
    }

    private void WatchWorkForStalls(string objectiveName, string? targetMap, CancellationToken token)
    {
        string? objectiveMap = NormalizeMapKey(targetMap);
        string lastSignature = ReadWorkProgressSignature();
        DateTime lastProgress = DateTime.UtcNow;
        DateTime? strandedSince = null;
        DateTime? lonelyOverflowSince = null;
        bool lonelyOverflowRetried = false;
        DateTime? maplessSince = null;
        int maplessRelogs = 0;
        int nudges = 0;

        while (!token.IsCancellationRequested && !Bot.ShouldExit)
        {
            if (token.WaitHandle.WaitOne(5000))
                return;

            string signature = ReadWorkProgressSignature();
            bool progressed = signature != lastSignature;
            if (progressed)
            {
                lastSignature = signature;
                lastProgress = DateTime.UtcNow;
                nudges = 0;
            }

            string? currentMap = ReadCurrentMapKey();

            // Working accounts never reach the objective loop, so they learn caps here.
            LearnMapCapFromRoomFullReports();

            // Fast path: stranded in a /house without progress (needs a known objective map;
            // otherwise the regular nudges below handle it).
            if (objectiveMap != null && IsHouseMap(currentMap))
            {
                if (progressed || strandedSince == null)
                    strandedSince = DateTime.UtcNow;
                else if (DateTime.UtcNow - strandedSince.Value >= TimeSpan.FromSeconds(Math.Max(15, OffMapRecoverySeconds)))
                {
                    if (token.IsCancellationRequested || Bot.ShouldExit)
                        return;

                    strandedSince = null;
                    lastProgress = DateTime.UtcNow;
                    Core.Logger(
                        $"Army objective {objectiveName}: in '/{currentMap}' for {OffMapRecoverySeconds}s without progress "
                            + $"(manual unstick?); returning to '/{objectiveMap}'. State: {DescribeWorkState()}",
                        "StallGuard"
                    );
                    TryStallRecovery(objectiveName, () => RejoinObjectiveMap(objectiveMap, fullReentry: false));
                }
                continue;
            }
            strandedSince = null;

            // Fast path: logged in but no map loaded (map= empty, every /join "Failed to join",
            // even /whitemap). Only a relog clears it; Relogin parks in /house and the house
            // fast path above then returns to the objective map.
            if (!progressed && IsClientMapless())
            {
                maplessSince ??= DateTime.UtcNow;
                if (
                    DateTime.UtcNow - maplessSince.Value >= TimeSpan.FromSeconds(Math.Max(30, MaplessRelogSeconds))
                    && maplessRelogs < Math.Max(1, MaxMaplessRelogs)
                )
                {
                    if (token.IsCancellationRequested || Bot.ShouldExit)
                        return;

                    maplessRelogs++;
                    maplessSince = null;
                    lastProgress = DateTime.UtcNow;
                    Core.Logger(
                        $"Army objective {objectiveName}: no map loaded for {MaplessRelogSeconds}s (relog #{maplessRelogs}). State: {DescribeWorkState()}",
                        "StallGuard"
                    );
                    TryStallRecovery(objectiveName, () => Core.Relogin("client stuck without a map"));
                    continue;
                }
            }
            else
            {
                maplessSince = null;
            }

            if (objectiveMap != null && !lonelyOverflowRetried && IsInLonelyOverflowRoom(currentMap, objectiveMap))
            {
                lonelyOverflowSince ??= DateTime.UtcNow;
                if (DateTime.UtcNow - lonelyOverflowSince.Value >= TimeSpan.FromSeconds(Math.Max(5, LonelyOverflowSeconds)))
                {
                    if (token.IsCancellationRequested || Bot.ShouldExit)
                        return;

                    lonelyOverflowRetried = true;
                    lonelyOverflowSince = null;
                    lastProgress = DateTime.UtcNow;
                    int plannedRoom = PlannedRoomBeforeOverflow?.Invoke() ?? 0;
                    Core.Logger(
                        $"Army objective {objectiveName}: alone in overflow room {GetCurrentRoomNumber()} of '/{objectiveMap}' for {LonelyOverflowSeconds}s; "
                            + $"retrying planned room {plannedRoom}.",
                        "StallGuard"
                    );
                    TryStallRecovery(
                        objectiveName,
                        () =>
                        {
                            RestorePlannedRoom?.Invoke();
                            RejoinObjectiveMap(objectiveMap, fullReentry: false);
                        }
                    );
                    continue;
                }
            }
            else
            {
                lonelyOverflowSince = null;
            }

            if (progressed)
                continue;

            if (DateTime.UtcNow - lastProgress < TimeSpan.FromMinutes(Math.Max(1, WorkStallMinutes)))
                continue;

            if (token.IsCancellationRequested || Bot.ShouldExit)
                return;

            nudges++;
            lastProgress = DateTime.UtcNow;

            string action;
            Action recovery;
            if (objectiveMap != null)
            {
                // A frozen client only wakes up with a real map change (same fix as a manual
                // /join house), so with a known objective map every nudge is a full re-entry.
                action = $"full re-entry into '/{objectiveMap}'";
                recovery = () => RejoinObjectiveMap(objectiveMap, fullReentry: true);
            }
            else if (nudges % 2 == 0)
            {
                string? rejoinMap = IsHouseMap(currentMap) ? null : Bot.Map?.Name;
                action = rejoinMap != null ? $"rejoining '/{NormalizeMapKey(rejoinMap)}'" : "no map to rejoin";
                recovery = () =>
                {
                    if (rejoinMap != null)
                        RejoinObjectiveMap(rejoinMap, fullReentry: false);
                };
            }
            else
            {
                action = "jumping to Enter";
                recovery = () => Bot.Map.Jump("Enter", "Spawn");
            }

            Core.Logger(
                $"Army objective {objectiveName}: no progress for {WorkStallMinutes} min (nudge #{nudges}, {action}). State: {DescribeWorkState()}",
                "StallGuard"
            );
            if (nudges >= 3)
            {
                Core.Logger(
                    $"Army objective {objectiveName}: still stuck after {nudges} nudges (~{nudges * WorkStallMinutes} min); the rest of the Army is waiting on this account.",
                    "StallGuard"
                );
            }

            TryStallRecovery(objectiveName, recovery);
        }
    }

    private void TryStallRecovery(string objectiveName, Action recovery)
    {
        try
        {
            recovery();
        }
        catch (Exception ex)
        {
            Core.Logger($"Army objective {objectiveName}: stall recovery failed: {ex.Message}", "StallGuard");
        }
    }

    // Joins the objective map in the account's current (private) room. A full re-entry passes
    // through /whitemap first so a half-loaded map or a stuck transition is fully reset.
    private void RejoinObjectiveMap(string map, bool fullReentry)
    {
        LeaveCombatBeforeMapChange();
        if (fullReentry)
        {
            Core.Join("whitemap", ignoreCheck: true);
            Bot.Wait.ForMapLoad("whitemap");
        }

        EnsureMapAccessBypass(NormalizeMapKey(map));
        Core.Join(map, ignoreCheck: true);
    }

    private string? ReadCurrentMapKey()
    {
        try
        {
            return NormalizeMapKey(Bot.Map?.Name);
        }
        catch
        {
            return null;
        }
    }

    // P6 on /overworld (2026-11-26): map= empty for 13+ min, every join failed, the Army waited.
    public int MaplessRelogSeconds { get; set; } = 90;
    public int MaxMaplessRelogs { get; set; } = 3;

    private bool IsClientMapless()
    {
        try
        {
            return Bot.Player.LoggedIn && (string.IsNullOrEmpty(Bot.Map?.Name) || !Bot.Map.Loaded);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsHouseMap(string? normalizedMap) =>
        !string.IsNullOrEmpty(normalizedMap) && normalizedMap.StartsWith("house", StringComparison.OrdinalIgnoreCase);

    private string ReadWorkProgressSignature()
    {
        try
        {
            StringBuilder sb = new();
            sb.Append(Bot.Player.Level).Append('|').Append(Bot.Player.XP).Append('|').Append(Bot.Player.Gold).Append('|');
            sb.Append(Bot.Inventory.Items?.Sum(i => (long)i.Quantity) ?? 0).Append('|');
            sb.Append(Bot.Inventory.Items?.Count ?? 0).Append('|');
            sb.Append(Bot.TempInv.Items?.Sum(i => (long)i.Quantity) ?? 0).Append('|');
            sb.Append(Bot.Reputation.FactionList?.Sum(f => (long)f.Rep) ?? 0).Append('|');
            foreach (var monster in Bot.Monsters.CurrentMonsters ?? Enumerable.Empty<Skua.Core.Models.Monsters.Monster>())
                sb.Append(monster.MapID).Append(':').Append(monster.HP).Append(',');
            return sb.ToString();
        }
        catch
        {
            // Game state is read from a background thread; a failed read must never look like a stall.
            return Guid.NewGuid().ToString();
        }
    }

    private string DescribeWorkState()
    {
        try
        {
            var target = Bot.Player.Target;
            var cellMonsters = Bot.Monsters.CurrentMonsters ?? Enumerable.Empty<Skua.Core.Models.Monsters.Monster>();
            string monsters = string.Join(
                ", ",
                cellMonsters.GroupBy(m => m.Name).Select(g => $"{g.Key} x{g.Count()} ({g.Count(m => m.HP > 0)} alive)")
            );
            return $"map={Bot.Map?.FullName ?? Bot.Map?.Name}, cell={Bot.Player.Cell}/{Bot.Player.Pad}, "
                + $"alive={Bot.Player.Alive}, hp={Bot.Player.Health}, "
                + $"target={(Bot.Player.HasTarget ? $"{target?.Name} hp={target?.HP}" : "none")}, "
                + $"cellMonsters=[{(string.IsNullOrEmpty(monsters) ? "none" : monsters)}]";
        }
        catch (Exception ex)
        {
            return $"unavailable ({ex.Message})";
        }
    }

    private static readonly string[] SupportHelperPriority =
    {
        "Lord of Order",
        "ArchPaladin",
        "Paladin Chronomancer",
        "StoneCrusher",
        "Infinity Titan",
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

    // True while this (finished) account runs the help callback (Goto/follow a teammate).
    // A "room is full" warning then means the teammate's room is full, not ours.
    public volatile bool IsHelpingTeammate;

    // Set by RunCooperativeObjective while a help callback runs, so long Goto/Join waits in
    // CoordinateFollowAndCombat can bail out as soon as the helped teammate finishes.
    private Func<bool>? helpTargetFinished;
    private int lastHelpTargetCheckTick;
    private bool lastHelpTargetFinished;

    private bool HelpTargetFinished()
    {
        if (helpTargetFinished == null)
            return false;

        int now = Environment.TickCount;
        if (unchecked(now - lastHelpTargetCheckTick) < 1000)
            return lastHelpTargetFinished;

        lastHelpTargetCheckTick = now;
        try
        {
            lastHelpTargetFinished = helpTargetFinished();
        }
        catch
        {
            lastHelpTargetFinished = false;
        }
        return lastHelpTargetFinished;
    }

    // Core.Join retries 20x with a growing random back-off and never looks at the helped
    // teammate: when the map refused the join ("Failed to join hydrachallenge") a helper sat
    // there ~3 min while every other account waited at the next barrier (each BS8736 round,
    // 2026-11-23). Helpers make one bounded attempt instead and give up as soon as the
    // teammate finishes; the help loop simply retries on its next pass.
    public int HelperJoinTimeoutSeconds { get; set; } = 15;

    private bool JoinMapForHelp(string map)
    {
        string? key = NormalizeMapKey(map?.Replace(" ", ""));
        if (string.IsNullOrEmpty(key) || Bot.ShouldExit)
            return false;

        string requested = map!.Replace(" ", "").ToLowerInvariant();
        string joinTarget = requested.Contains('-') || !Core.PrivateRooms
            ? requested
            : $"{requested}-{Core.PrivateRoomNumber}";

        // Same map name is not enough: a teammate in another private room of the same map
        // (e.g. an overflow room) is only reached by joining that exact room.
        int dashIndex = joinTarget.IndexOf('-');
        int expectedRoom = dashIndex >= 0
            && int.TryParse(joinTarget.Substring(dashIndex + 1), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int parsedRoom)
            ? parsedRoom
            : 0;

        bool OnMap()
        {
            if (!string.Equals(NormalizeMapKey(Bot.Map?.Name), key, StringComparison.OrdinalIgnoreCase))
                return false;
            if (expectedRoom <= 0)
                return true;
            int currentRoom = GetCurrentRoomNumber();
            // Unknown room (area name unreadable): keep the old name-only behaviour.
            return currentRoom <= 0 || currentRoom == expectedRoom;
        }

        if (OnMap())
            return true;
        if (HelpTargetFinished())
            return false;

        EnsureMapAccessBypass(key);
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;

        LeaveCombatBeforeMapChange();
        lastHelperJoinHitFullRoom = false;
        int fullBefore = Volatile.Read(ref roomFullWarnings);
        lastHelperJoinHitFullRoom = TravelHitFullRoom(() =>
        {
            Bot.Map.Join(joinTarget, "Enter", "Spawn", autoCorrect: !Core.PrivateRooms);
            Bot.Wait.ForTrue(
                () => OnMap() || HelpTargetFinished() || Bot.ShouldExit || RoomFullSince(fullBefore),
                Math.Max(5, HelperJoinTimeoutSeconds)
            );
        });
        if (OnMap())
        {
            lastHelperJoinHitFullRoom = false;
            return true;
        }

        if (!HelpTargetFinished() && !Bot.ShouldExit)
        {
            Core.Logger(
                lastHelperJoinHitFullRoom
                    ? $"Helper could not join '/{joinTarget}': room is full (now on '/{DescribeCurrentRoom()}')."
                    : $"Helper could not join '/{joinTarget}' within {Math.Max(5, HelperJoinTimeoutSeconds)}s (now on '/{DescribeCurrentRoom()}'); retrying on the next pass.",
                "CoordinateFollowAndCombat"
            );
        }
        return false;
    }

    // Set by JoinMapForHelp: the last helper join was refused because the room is full.
    private bool lastHelperJoinHitFullRoom;

    /// <summary>Room number of the loaded instance ("map-1234" -> 1234), 0 when unknown.</summary>
    public int GetCurrentRoomNumber()
    {
        try
        {
            string? areaName = Bot.Flash.GetGameObject<string>("world.strAreaName");
            if (string.IsNullOrWhiteSpace(areaName))
                return 0;

            int dash = areaName.LastIndexOf('-');
            return dash >= 0
                && int.TryParse(areaName.Substring(dash + 1).Trim().Trim('"'), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int room)
                ? room
                : 0;
        }
        catch
        {
            return 0;
        }
    }

    private string lastSkillRotationClass = string.Empty;
    private string lastSpecialEquippedMapRoom = string.Empty;
    private readonly HashSet<string> unreachableHelperTargets = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> attemptedBypassReloadCells = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> failedGotoCounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> failedSameMapJumpCounts = new(StringComparer.OrdinalIgnoreCase);
    private bool loggedHelperSleepMode;

    // "Unreachable" is not forever: a teammate that was in a /house (manual unstick), a frozen
    // client or another instance can come back to the objective map, and helpers sleeping until
    // the objective ends left it alone (quest 8736 Klunk, 2026-11-23). Each mark expires after this many seconds, then
    // the helper tries to reach that teammate again from scratch.
    public int UnreachableHelperRetrySeconds { get; set; } = 120;
    private readonly Dictionary<string, DateTime> unreachableHelperMarkedAt = new(StringComparer.OrdinalIgnoreCase);

    private void ExpireUnreachableHelperTargets(string objectiveName)
    {
        if (unreachableHelperTargets.Count == 0)
        {
            unreachableHelperMarkedAt.Clear();
            return;
        }

        DateTime now = DateTime.UtcNow;
        foreach (string target in unreachableHelperTargets)
        {
            if (!unreachableHelperMarkedAt.ContainsKey(target))
                unreachableHelperMarkedAt[target] = now;
        }

        List<string> expired = unreachableHelperMarkedAt
            .Where(kv => now - kv.Value >= TimeSpan.FromSeconds(Math.Max(30, UnreachableHelperRetrySeconds)))
            .Select(kv => kv.Key)
            .ToList();
        if (expired.Count == 0)
            return;

        foreach (string target in expired)
        {
            unreachableHelperTargets.Remove(target);
            unreachableHelperMarkedAt.Remove(target);
            failedGotoCounts.Remove(target);
            failedSameMapJumpCounts.Remove(target);
        }

        loggedHelperSleepMode = false;
        Core.Logger(
            $"Army objective {objectiveName}: retrying unreachable teammate(s) {string.Join(", ", expired.Select(t => $"'{t}'"))}.",
            "CoreArmyFJ"
        );
    }

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
            RefreshRealQuestSlot(slot);
            if (Bot.Flash.CallGameFunction<int>("world.getQuestValue", slot) < value)
                Bot.Quests.UpdateQuest(value, slot);
        }
        catch
        {
            // Ignore transient Flash read errors while map transitions settle.
        }
    }

    // ---------------------------------------------------------------------------------------
    // Real quest-slot values. Map bypasses (ours and CoreBots.Join's SimpleQuestBypass) and the
    // explicit Bot.Quests.UpdateQuest calls in some story steps raise a story slot CLIENT-side
    // only, until a relog (e.g. /timespace sets slot 100 = 14 = "2518 done"). isCompletedBefore
    // then reports unfinished story quests as done: 2518 was skipped and the server refused 2519;
    // 565-567 were skipped and 597 was refused ("Missing Quest Progress").
    //
    // A client value is only trusted when no bypass could have produced it: below the slot's
    // lowest bypass value, above its highest one, or right after a fresh login. Everything else
    // comes from the per-account record (ArmyRealQuestSlots.<user>.txt), which is kept across
    // script restarts and raised on real turn-ins (QuestTurnedIn). An account that starts with an
    // unexplained value in the bypass range and no record relogs once to read the server values.
    // ---------------------------------------------------------------------------------------
    private readonly Dictionary<int, int> realQuestSlots = new();
    private string realQuestSlotsLogin = string.Empty;
    private bool realQuestSlotsLoaded;
    private bool questTurnInHooked;
    private Dictionary<int, (int Min, int Max)>? questSlotBypassBounds;

    // Client-side fakes outside the map table: explicit Bot.Quests.UpdateQuest(questId) calls in
    // ArmyFarmerJoeDoAll (354 dwarfprison access, 567/597 Wolfwing, 847 Ledgermayne, 4614 sandport,
    // 2814 Lionfang raindrops, 4361 treetitanbattle), in MapQuestIdBypasses, and in CoreBots.Join
    // (2621 onslaughttower, 4361, 8776). UpdateQuest SETS the slot, so these can also LOWER real
    // progress: rejoining /onslaughttower reset slot 106 from 11 to 10 and locked 2623 on P1-P3
    // (2026-11-26). RestoreRealQuestSlots puts the real value back before each story step.
    // Keep in sync when adding such a call.
    private static readonly (int Slot, int Value)[] ExplicitQuestSlotFakes =
    {
        (22, 31),
        (26, 22),
        (26, 23),
        (32, 35),
        (97, 15),
        (106, 10),
        (117, 17),
        (196, 14),
        (470, 18),
    };

    private Dictionary<int, (int Min, int Max)> QuestSlotBypassBounds =>
        questSlotBypassBounds ??= MapQuestSlotBypasses.Values
            .SelectMany(v => v)
            .Concat(ExplicitQuestSlotFakes)
            .GroupBy(v => v.Slot)
            .ToDictionary(g => g.Key, g => (g.Min(v => v.Value), g.Max(v => v.Value)));

    /// <summary>True when no map bypass can have produced this client value (so it is the server's).</summary>
    private bool IsCertainlyRealSlotValue(int slot, int client) =>
        !QuestSlotBypassBounds.TryGetValue(slot, out var bounds) || client < bounds.Min || client > bounds.Max;

    private string RealQuestSlotsPath
    {
        get
        {
            string user = new string((Core.Username() ?? "unknown").ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
            return Path.Combine(ClientFileSources.SkuaOptionsDIR, $"ArmyRealQuestSlots.{(user.Length > 0 ? user : "unknown")}.txt");
        }
    }

    // Per-connection player id: a new value means a new login (client slots = server values).
    private string CurrentLoginMarker()
    {
        try
        {
            int id = Bot.Player.ID;
            return id > 0 ? id.ToString(System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private int ReadClientQuestSlot(int slot)
    {
        try
        {
            return Bot.Flash.CallGameFunction<int>("world.getQuestValue", slot);
        }
        catch
        {
            return -1;
        }
    }

    private void EnsureRealQuestSlotsLoaded()
    {
        if (realQuestSlotsLoaded)
            return;

        realQuestSlotsLoaded = true;
        try
        {
            if (!File.Exists(RealQuestSlotsPath))
                return;

            foreach (string line in File.ReadAllLines(RealQuestSlotsPath))
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                if (parts[0].Trim() == "login")
                {
                    realQuestSlotsLogin = parts[1].Trim();
                    continue;
                }

                if (
                    int.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int slot)
                    && int.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int value)
                    && slot > 0
                    && value >= 0
                )
                    realQuestSlots[slot] = value;
            }
        }
        catch { }
    }

    private void SaveRealQuestSlots()
    {
        try
        {
            File.WriteAllLines(
                RealQuestSlotsPath,
                new[] { $"login={realQuestSlotsLogin}" }
                    .Concat(realQuestSlots.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))
            );
        }
        catch { }
    }

    // Caller holds the lock. Returns true when the record changed.
    private bool SetRealQuestSlot(int slot, int value)
    {
        if (realQuestSlots.TryGetValue(slot, out int known) && known == value)
            return false;

        realQuestSlots[slot] = value;
        return true;
    }

    /// <summary>Updates the record from the client when the client value is certainly the server's.</summary>
    private void RefreshRealQuestSlot(int slot)
    {
        int client = ReadClientQuestSlot(slot);
        if (client < 0 || !IsCertainlyRealSlotValue(slot, client))
            return;

        lock (realQuestSlots)
        {
            EnsureRealQuestSlotsLoaded();
            if (SetRealQuestSlot(slot, client))
                SaveRealQuestSlots();
        }
    }

    // Records every bypass slot from the client. With trustClient (fresh login) every value is
    // taken as is; otherwise only certain values. Returns the ambiguous slots without a record.
    private List<string> RecordBypassSlotsFromClient(bool trustClient)
    {
        List<string> ambiguous = new();
        lock (realQuestSlots)
        {
            EnsureRealQuestSlotsLoaded();
            bool changed = false;
            foreach (int slot in QuestSlotBypassBounds.Keys)
            {
                int client = ReadClientQuestSlot(slot);
                if (client < 0)
                    continue;

                if (trustClient || IsCertainlyRealSlotValue(slot, client))
                    changed |= SetRealQuestSlot(slot, client);
                else if (!realQuestSlots.ContainsKey(slot))
                    ambiguous.Add($"{slot}={client}");
            }

            string login = CurrentLoginMarker();
            if (trustClient && login.Length > 0 && login != realQuestSlotsLogin)
            {
                realQuestSlotsLogin = login;
                changed = true;
            }

            if (changed)
                SaveRealQuestSlots();
        }
        return ambiguous;
    }

    /// <summary>
    /// Startup (before the Army session): records the real value of every bypass slot. A new
    /// login is trusted as is; an unexplained bypass-range value without a record costs one relog.
    /// </summary>
    public void InitRealQuestSlotTracking()
    {
        if (!questTurnInHooked)
        {
            try
            {
                Bot.Events.QuestTurnedIn += OnQuestTurnedInTrackRealSlot;
                Bot.Events.MapChanged += OnMapChangedRaiseLoweredSlots;
                questTurnInHooked = true;
            }
            catch { }
        }

        bool newLogin;
        lock (realQuestSlots)
        {
            EnsureRealQuestSlotsLoaded();
            string login = CurrentLoginMarker();
            newLogin = login.Length > 0 && login != realQuestSlotsLogin;
        }

        List<string> ambiguous = RecordBypassSlotsFromClient(trustClient: newLogin);
        if (ambiguous.Count == 0)
            return;

        Core.Logger(
            $"Story slots {string.Join(", ", ambiguous)} may be faked by a map bypass from an earlier run (no real value on record); "
                + "relogging once to read the server values.",
            "CoreArmyFJ"
        );
        try
        {
            Core.Relogin("reading real story progress");
        }
        catch (Exception ex)
        {
            Core.Logger($"Relog for story progress failed: {ex.Message}", "CoreArmyFJ");
        }

        if (!Bot.Player.LoggedIn)
        {
            Core.Logger("Still logged out after the relog; story checks keep using the client values.", "CoreArmyFJ");
            return;
        }

        RecordBypassSlotsFromClient(trustClient: true);
    }

    public void StopRealQuestSlotTracking()
    {
        if (!questTurnInHooked)
            return;

        try
        {
            Bot.Events.QuestTurnedIn -= OnQuestTurnedInTrackRealSlot;
            Bot.Events.MapChanged -= OnMapChangedRaiseLoweredSlots;
        }
        catch { }
        questTurnInHooked = false;
    }

    // CoreBots.Join("onslaughttower") runs UpdateQuest(2621) before every join, which SETS slot 106
    // to 10 even after 2622 is done. Raising back to the real value is always safe (a map bypass
    // only needs a value at least this high), so it is done right after every map change.
    private void OnMapChangedRaiseLoweredSlots(string map)
    {
        try
        {
            List<(int Slot, int Value)> records;
            lock (realQuestSlots)
            {
                EnsureRealQuestSlotsLoaded();
                records = realQuestSlots.Select(kv => (kv.Key, kv.Value)).ToList();
            }

            foreach (var (slot, value) in records)
            {
                int client = ReadClientQuestSlot(slot);
                if (client < 0 || client >= value)
                    continue;

                Bot.Quests.UpdateQuest(value, slot);
                Core.Logger(
                    $"Quest slot {slot}: joining /{map} lowered the client to {client}; raised back to the real value {value}.",
                    "CoreArmyFJ"
                );
            }
        }
        catch { }
    }

    private void OnQuestTurnedInTrackRealSlot(int questId)
    {
        try
        {
            Quest? quest = Bot.Quests.Tree?.FirstOrDefault(q => q.ID == questId);
            if (quest == null || quest.Slot <= 0)
                return;

            lock (realQuestSlots)
            {
                EnsureRealQuestSlotsLoaded();
                if (!realQuestSlots.TryGetValue(quest.Slot, out int known) || quest.Value <= known)
                    return;

                realQuestSlots[quest.Slot] = quest.Value;
                SaveRealQuestSlots();
            }
        }
        catch { }
    }

    /// <summary>
    /// Story completion against the real slot value when the client value may be faked.
    /// Null = the client value can be trusted (use isCompletedBefore).
    /// </summary>
    public bool? IsQuestDoneByRealSlot(int questId)
    {
        try
        {
            Quest? quest = Bot.Quests.Tree?.FirstOrDefault(q => q.ID == questId);
            if (quest == null || quest.Slot <= 0 || !QuestSlotBypassBounds.ContainsKey(quest.Slot))
                return null;

            int client = ReadClientQuestSlot(quest.Slot);
            if (client < 0)
                return null;

            if (IsCertainlyRealSlotValue(quest.Slot, client))
            {
                RefreshRealQuestSlot(quest.Slot);
                return null;
            }

            lock (realQuestSlots)
            {
                EnsureRealQuestSlotsLoaded();
                return realQuestSlots.TryGetValue(quest.Slot, out int real) ? real >= quest.Value : null;
            }
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Puts the real value back into the client before story work, so official helpers
    /// (Story.KillQuest, QuestProgression...) do not skip quests a bypass faked as done.
    /// Joining a bypass map raises it again; a map already joined keeps working.
    /// </summary>
    public void RestoreRealQuestSlots()
    {
        List<(int Slot, int Value)> records;
        lock (realQuestSlots)
        {
            EnsureRealQuestSlotsLoaded();
            records = realQuestSlots.Select(kv => (kv.Key, kv.Value)).ToList();
        }

        foreach (var (slot, value) in records)
        {
            int client = ReadClientQuestSlot(slot);
            if (client < 0 || client == value)
                continue;

            if (IsCertainlyRealSlotValue(slot, client))
            {
                RefreshRealQuestSlot(slot);
                continue;
            }

            try
            {
                Bot.Quests.UpdateQuest(value, slot);
                Core.Logger(
                    $"Quest slot {slot}: client showed {client} (map bypass), restored real value {value}.",
                    "CoreArmyFJ"
                );
            }
            catch { }
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
                // The table says the cell is open, but the client keeps refusing the jump (e.g. the
                // teammate itself is stuck in a transition cell like moveFrame). Give up after a while
                // instead of retrying forever; the helper then serves another reachable teammate.
                int fails = failedSameMapJumpCounts.TryGetValue(normalizedTarget, out int prev) ? prev + 1 : 1;
                failedSameMapJumpCounts[normalizedTarget] = fails;
                if (fails >= 12 && unreachableHelperTargets.Add(normalizedTarget))
                {
                    Core.Logger(
                        $"Map '{currentMap}' cell '{targetCell}' refused {fails} jumps (local Lv {Bot.Player.Level}, table Lv {requiredLevel}); putting helper in sleep mode for '{normalizedTarget}'.",
                        "CoordinateFollowAndCombat"
                    );
                }
                else
                {
                    Bot.Sleep(500);
                }
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
            TryJoinWhitemapQuick();
            EnsureMapAccessBypass(currentMap);
            LeaveCombatBeforeMapChange();
            Bot.Player.Goto(normalizedTarget);
            Bot.Wait.ForTrue(() => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished(), 20);
            if (HelpTargetFinished())
                return false;
            if (!Bot.Map.PlayerExists(normalizedTarget) || string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
            {
                JoinMapForHelp(currentMap!);
                Bot.Wait.ForTrue(() => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished(), 20);
                if (HelpTargetFinished())
                    return false;
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

            // Teammate bumped into an overflow room: join that exact room. /goto across private
            // rooms kept failing (12x -> sleep mode) and the map-name fallback below never moved
            // a helper that was already on the same map, just in the planned room.
            int targetOverflowRoom = !string.IsNullOrEmpty(targetMap) && Core.PrivateRooms
                ? ReadOverflowRoom(FindArmyPlayerNumber(normalizedTarget))
                : 0;
            if (targetOverflowRoom > 0)
            {
                string overflowTarget = $"{NormalizeMapKey(targetMap!.Replace(" ", ""))}-{targetOverflowRoom}";
                if (!Bot.Map.PlayerExists(normalizedTarget))
                {
                    if (!JoinMapForHelp(overflowTarget) && lastHelperJoinHitFullRoom)
                    {
                        ReportRoomFull(targetOverflowRoom);
                        MarkTeammateRoomFull(normalizedTarget, $"/{overflowTarget}");
                        return;
                    }
                    Bot.Wait.ForTrue(
                        () => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished(),
                        20
                    );
                }
            }
            else
            {
                LeaveCombatBeforeMapChange();
                int fullBefore = Volatile.Read(ref roomFullWarnings);
                bool gotoHitFullRoom = TravelHitFullRoom(() =>
                {
                    Bot.Player.Goto(normalizedTarget);
                    Bot.Wait.ForTrue(
                        () => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished() || RoomFullSince(fullBefore),
                        20
                    );
                });
                if (gotoHitFullRoom && !Bot.Map.PlayerExists(normalizedTarget) && !HelpTargetFinished())
                {
                    MarkTeammateRoomFull(normalizedTarget, "/goto refused");
                    return;
                }
            }
            if (HelpTargetFinished())
                return;
            if (
                targetOverflowRoom <= 0
                && (!Bot.Map.PlayerExists(normalizedTarget) || string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
                && !string.IsNullOrEmpty(targetMap)
                && !IsSoloOnlyMap(targetMap)
            )
            {
                if (!JoinMapForHelp(targetMap!) && lastHelperJoinHitFullRoom)
                {
                    ReportRoomFull(Core.PrivateRoomNumber);
                    MarkTeammateRoomFull(normalizedTarget, $"/{targetMap}");
                    return;
                }
                Bot.Wait.ForTrue(
                    () => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished(),
                    20
                );
                if (HelpTargetFinished())
                    return;

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
                        if (Bot.ShouldExit || Bot.Map.PlayerExists(normalizedTarget) || HelpTargetFinished())
                            break;
                        JoinMapForHelp(fallbackMap);
                        Bot.Wait.ForTrue(
                            () => (Bot.Map.Loaded && Bot.Map.PlayerExists(normalizedTarget)) || HelpTargetFinished(),
                            20
                        );
                    }
                }
            }
            // Teammate finished while we were travelling: don't count it as a failed Goto.
            if (HelpTargetFinished())
                return;
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

        if (inSameCell)
            failedSameMapJumpCounts.Remove(normalizedTarget);

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

        // 2. Leave combat on the current map first (JumpWait, then the map's known safe cell).
        if (InCombatCell())
            TryLeaveCombatInPlace(20);

        // Only fall back to /whitemap (bounded) if the current cell still has monsters or combat did not exit.
        if (!IsUtilityMap(Bot.Map?.Name) && InCombatCell() && TryJoinWhitemapQuick())
        {
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
            || className.Equals("Lord of Order", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Infinity Titan", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Paladin Chronomancer", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Bard", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Troubador of Love", StringComparison.OrdinalIgnoreCase)
            || className.Equals("Shaman", StringComparison.OrdinalIgnoreCase);
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


    // Same as HasArmySignal but over lines already read (avoids re-reading the sync file per player).
    private bool HasArmySignalIn(string[] lines, string signal, int senderPlayerNumber)
    {
        string signalName = NormalizeArmyRecordName(signal);
        if (string.IsNullOrEmpty(signalName) || senderPlayerNumber < 1 || senderPlayerNumber > armyPlayers.Length)
            return false;

        string expectedSender = armyPlayers[senderPlayerNumber - 1];
        foreach (string line in lines)
        {
            if (!line.StartsWith("SIGNAL|", StringComparison.Ordinal))
                continue;

            string[] parts = line.Split('|');
            if (
                parts.Length == 5
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
                && string.Equals(parts[3], signalName, StringComparison.Ordinal)
                && string.Equals(NormalizeArmyUsername(parts[4]), expectedSender, StringComparison.OrdinalIgnoreCase)
            )
                return true;
        }

        return false;
    }

    // ---------------------------------------------------------------------------------------
    // Room subgroups. A map cap smaller than the Army splits players into the fewest rooms
    // possible with balanced sizes (7 @ cap 6 -> 4+3, 7 @ cap 3 -> 3+2+2) instead of leaving
    // the last player alone (old (p-1)/cap gave 6+1 and 3+3+1). Groups stay contiguous so
    // P1-P4 (the core roles) stay together whenever the cap allows.
    // ---------------------------------------------------------------------------------------
    public static int GetSubgroupIndex(int playerNumber, int playerCount, int cap)
    {
        if (cap <= 0 || playerCount <= 0 || playerCount <= cap)
            return 0;

        int groups = (playerCount + cap - 1) / cap;
        int baseSize = playerCount / groups;
        int extra = playerCount % groups;
        int start = 1;
        for (int group = 0; group < groups; group++)
        {
            int size = baseSize + (group < extra ? 1 : 0);
            if (playerNumber < start + size)
                return group;
            start += size;
        }

        return groups - 1;
    }

    public static int GetSubgroupFirstPlayer(int subgroup, int playerCount, int cap)
    {
        if (cap <= 0 || playerCount <= cap || subgroup <= 0)
            return 1;

        int groups = (playerCount + cap - 1) / cap;
        int baseSize = playerCount / groups;
        int extra = playerCount % groups;
        int start = 1;
        for (int group = 0; group < subgroup && group < groups; group++)
            start += baseSize + (group < extra ? 1 : 0);
        return start;
    }

    // ---------------------------------------------------------------------------------------
    // Dynamic room layout (per cooperative objective). Rooms follow who still has work to do:
    // - pending accounts are spread first (with everyone pending this equals the static split);
    // - finished accounts fill the free seats, every room getting a helper before any gets two;
    // - when a room has nobody pending left, its accounts move to rooms with free seats;
    // - accounts that fit nowhere wait in /whitemap (or /house) until a seat opens.
    // Pending accounts never change room mid-objective: their farm call can run for a long time.
    // Room index i always maps to RoomNumber + i, so room numbers stay predictable.
    // ---------------------------------------------------------------------------------------
    public const int RoomLayoutWaiting = -1;

    // Room per player (index = player - 1) for the running objective; null = single shared room.
    private int[]? activeRoomLayout;
    private bool loggedRoomWaitMode;

    public bool HasActiveRoomLayout => activeRoomLayout != null;

    /// <summary>Room of a player in the active layout: 0 without a layout, -1 when waiting outside.</summary>
    public int GetLayoutRoom(int playerNumber)
    {
        int[]? layout = activeRoomLayout;
        if (layout == null || playerNumber < 1 || playerNumber > layout.Length)
            return 0;
        return layout[playerNumber - 1];
    }

    /// <summary>Players (ascending) sharing the given room in the active layout.</summary>
    public List<int> GetLayoutRoomMembers(int room)
    {
        int[]? layout = activeRoomLayout;
        List<int> members = new();
        if (layout == null)
        {
            for (int p = 1; p <= armyPlayers.Length; p++)
                members.Add(p);
            return members;
        }

        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == room)
                members.Add(i + 1);
        }
        return members;
    }

    // Seat 1 of the room is the support seat (same rule as GetRoomSeat in the main script).
    private int GetLayoutRoomSupportPlayer(int room)
    {
        List<int> members = GetLayoutRoomMembers(room);
        if (members.Count > 1)
            return members[1];
        return members.Count == 1 ? members[0] : 2;
    }

    public static int[] ComputeRoomLayout(bool[] pending, int cap, int[]? previous)
    {
        int n = pending.Length;
        int[] layout = new int[n];
        List<int> pendingIdx = Enumerable.Range(0, n).Where(i => pending[i]).ToList();
        if (cap <= 0 || n <= cap || pendingIdx.Count == 0)
            return layout;

        for (int i = 0; i < n; i++)
            layout[i] = RoomLayoutWaiting;

        bool usePrevious = previous != null && previous.Length == n;
        Dictionary<int, int> members = new();

        if (!usePrevious)
        {
            // Fewest rooms that hold everyone, never more rooms than pending accounts.
            int groups = Math.Min((n + cap - 1) / cap, pendingIdx.Count);
            int baseSize = pendingIdx.Count / groups;
            int extra = pendingIdx.Count % groups;
            int k = 0;
            for (int room = 0; room < groups; room++)
            {
                int size = baseSize + (room < extra ? 1 : 0);
                members[room] = 0;
                for (int s = 0; s < size; s++, k++)
                {
                    layout[pendingIdx[k]] = room;
                    members[room]++;
                }
            }
        }
        else
        {
            // Pending accounts keep their room.
            foreach (int i in pendingIdx)
            {
                int room = previous![i];
                if (room < 0 || (members.TryGetValue(room, out int count) && count >= cap))
                    continue;
                layout[i] = room;
                members[room] = members.TryGetValue(room, out int c) ? c + 1 : 1;
            }

            // Safety: a pending account without a valid room takes a free seat or a new room.
            foreach (int i in pendingIdx.Where(i => layout[i] == RoomLayoutWaiting).ToList())
            {
                int room = members.Where(m => m.Value < cap).OrderBy(m => m.Value).ThenBy(m => m.Key).Select(m => m.Key).DefaultIfEmpty(-1).First();
                if (room < 0)
                {
                    room = 0;
                    while (members.ContainsKey(room))
                        room++;
                }
                layout[i] = room;
                members[room] = members.TryGetValue(room, out int c) ? c + 1 : 1;
            }
        }

        // Finished accounts fill free seats: room with fewest helpers first; prefer the helper
        // already in that room, then one without a room, then the lowest player number.
        HashSet<int> activeRooms = new(members.Keys);
        Dictionary<int, int> helpersIn = activeRooms.ToDictionary(r => r, r => 0);
        List<int> freeHelpers = Enumerable.Range(0, n).Where(i => !pending[i]).ToList();
        while (freeHelpers.Count > 0)
        {
            List<int> open = activeRooms.Where(r => members[r] < cap).ToList();
            if (open.Count == 0)
                break;

            int room = open.OrderBy(r => helpersIn[r]).ThenBy(r => members[r]).ThenBy(r => r).First();
            int pick = -1;
            if (usePrevious)
            {
                int[] prev = previous!;
                foreach (int i in freeHelpers)
                {
                    if (prev[i] == room)
                    {
                        pick = i;
                        break;
                    }
                }
                if (pick < 0)
                {
                    foreach (int i in freeHelpers)
                    {
                        if (!activeRooms.Contains(prev[i]))
                        {
                            pick = i;
                            break;
                        }
                    }
                }
            }
            if (pick < 0)
                pick = freeHelpers[0];

            layout[pick] = room;
            members[room]++;
            helpersIn[room]++;
            freeHelpers.Remove(pick);
        }

        return layout;
    }

    public static string DescribeRoomLayout(int[] layout)
    {
        List<string> parts = new();
        foreach (int room in layout.Where(r => r >= 0).Distinct().OrderBy(r => r))
        {
            IEnumerable<string> players = Enumerable.Range(0, layout.Length).Where(i => layout[i] == room).Select(i => $"P{i + 1}");
            parts.Add($"room+{room}: {string.Join(",", players)}");
        }

        List<string> waiting = Enumerable.Range(0, layout.Length).Where(i => layout[i] < 0).Select(i => $"P{i + 1}").ToList();
        if (waiting.Count > 0)
            parts.Add($"waiting: {string.Join(",", waiting)}");
        return string.Join(" | ", parts);
    }

    private string RoomLayoutPath(string objectiveName)
    {
        string safeName = NormalizeArmyRecordName(objectiveName);
        if (string.IsNullOrEmpty(safeName))
            safeName = "OBJ";
        return $"{armySyncPath}.LAYOUT.{safeName}.{armySessionId}";
    }

    private int[]? ReadRoomLayout(string objectiveName, long objectiveId)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return null;

        try
        {
            string path = RoomLayoutPath(objectiveName);
            if (!File.Exists(path))
                return null;

            string[] parts = File.ReadAllText(path).Trim().Split('|');
            if (
                parts.Length != 2
                || !long.TryParse(parts[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long id)
                || id != objectiveId
            )
                return null;

            string[] rooms = parts[1].Split(',');
            if (rooms.Length != armyPlayers.Length)
                return null;

            int[] layout = new int[rooms.Length];
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!int.TryParse(rooms[i], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out layout[i]) || layout[i] < RoomLayoutWaiting)
                    return null;
            }
            return layout;
        }
        catch
        {
            return null;
        }
    }

    private void WriteRoomLayout(string objectiveName, long objectiveId, int[] layout)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return;

        try
        {
            string path = RoomLayoutPath(objectiveName);
            string temp = $"{path}.{armyPlayerIndex + 1}.tmp";
            string content = $"{objectiveId.ToString(System.Globalization.CultureInfo.InvariantCulture)}|{string.Join(",", layout)}";
            File.WriteAllText(temp, content);
            if (File.Exists(path))
                File.Delete(path);
            File.Move(temp, path);
        }
        catch { }
    }

    // Called once per objective loop. The coordinator is the lowest-numbered finished account:
    // accounts still working may sit inside a long farm call and cannot react in time.
    private void RefreshRoomLayout(string objectiveName, long objectiveId, int cap, List<int> pendingPlayers, List<int> helperPlayers)
    {
        int[]? fromFile = ReadRoomLayout(objectiveName, objectiveId);
        if (fromFile != null && (activeRoomLayout == null || !fromFile.SequenceEqual(activeRoomLayout)))
        {
            activeRoomLayout = fromFile;
            Core.Logger($"Army objective {objectiveName}: rooms updated -> {DescribeRoomLayout(fromFile)}.", "CoreArmyFJ");
        }

        if (activeRoomLayout == null || helperPlayers.Count == 0 || helperPlayers.Min() != armyPlayerIndex + 1)
            return;

        bool[] mask = new bool[armyPlayers.Length];
        foreach (int p in pendingPlayers)
            mask[p - 1] = true;

        int[] next = ComputeRoomLayout(mask, cap, activeRoomLayout);
        if (next.SequenceEqual(activeRoomLayout))
            return;

        activeRoomLayout = next;
        WriteRoomLayout(objectiveName, objectiveId, next);
        Core.Logger($"Army objective {objectiveName}: rooms rebalanced -> {DescribeRoomLayout(next)}.", "CoreArmyFJ");
    }

    // No room with a pending teammate has a free seat: stop fighting and wait in /whitemap (or /house).
    private void WaitOutsideFullRooms(string objectiveName)
    {
        if (!loggedRoomWaitMode)
        {
            Core.Logger(
                $"Army objective {objectiveName}: every room with a pending teammate is full; waiting in /whitemap until a seat opens.",
                "CoreArmyFJ"
            );
            loggedRoomWaitMode = true;
        }

        string currentMap = Bot.Map?.Name ?? string.Empty;
        if (
            !string.Equals(currentMap, "whitemap", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(currentMap, "house", StringComparison.OrdinalIgnoreCase)
        )
        {
            try
            {
                Bot.Skills.Stop();
            }
            catch { }

            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();

            Bot.Options.AttackWithoutTarget = false;
            Bot.Options.AggroAllMonsters = false;
            Bot.Options.AggroMonsters = false;
            TryLeaveCombatInPlace(10);
            TryJoinWhitemapQuick();
        }

        Bot.Sleep(1000);
    }

    // ---------------------------------------------------------------------------------------
    // Leaving a combat room. In /xantown the Enter cell has Fire Imps/Lava Golems, every other
    // cell is r* (JumpWait blacklists them) and Core.Join("whitemap") kept failing while the
    // account was being hit: 20 retries = ~5 min per call, ~36 min lost in one Xan chain
    // (2026-11-25). Leave combat in place first (known safe cell), then try /whitemap a bounded
    // number of times.
    // ---------------------------------------------------------------------------------------
    private static readonly Dictionary<string, (string Cell, string Pad)> KnownSafeCells = new(StringComparer.OrdinalIgnoreCase)
    {
        // Same cell the Xan chain already parks in after each step.
        ["xantown"] = ("r12", "Left"),
    };

    private static bool IsUtilityMap(string? map) =>
        string.Equals(map, "whitemap", StringComparison.OrdinalIgnoreCase)
        || string.Equals(map, "battleon", StringComparison.OrdinalIgnoreCase);

    private bool InCombatCell() => Bot.Player.InCombat || Bot.Monsters.CurrentMonsters.Count > 0;

    /// <summary>
    /// The map's known safe cell, otherwise JumpWait. True when the account ends out of combat.
    /// </summary>
    public bool TryLeaveCombatInPlace(int waitSeconds = 10)
    {
        if (!InCombatCell() || IsUtilityMap(Bot.Map?.Name))
            return true;

        string? map = NormalizeMapKey(Bot.Map?.Name);
        if (map != null && KnownSafeCells.TryGetValue(map, out var safe))
        {
            if (!string.Equals(Bot.Player.Cell, safe.Cell, StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.Jump(safe.Cell, safe.Pad, false);
                Bot.Wait.ForCellChange(safe.Cell);
            }
            Bot.Wait.ForCombatExit(waitSeconds);
            if (!InCombatCell())
                return true;
        }

        Core.JumpWait();
        Bot.Wait.ForCombatExit(waitSeconds);
        return !Bot.Player.InCombat;
    }

    // AQW refuses /join and /goto while the player is in combat (confirmed 2026-11-25): every
    // map change must first move to a safe cell. Cheap no-op when already out of combat.
    private void LeaveCombatBeforeMapChange()
    {
        // A dead player cannot change map (P5 died on /overworld Enter and both /whitemap
        // tries failed): wait for the respawn first.
        if (Bot.Player.LoggedIn && !Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 40);

        if (!Bot.Player.InCombat)
            return;

        try
        {
            Bot.Skills.Stop();
        }
        catch { }
        if (Bot.Player.HasTarget)
            Bot.Combat.CancelTarget();
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        TryLeaveCombatInPlace(10);
    }

    public int WhitemapJoinAttempts { get; set; } = 2;
    public int WhitemapJoinRetryCooldownSeconds { get; set; } = 60;
    private int lastWhitemapJoinFailTick;
    private bool whitemapJoinFailedRecently;

    /// <summary>
    /// /whitemap in at most WhitemapJoinAttempts tries (~10s each) instead of Core.Join's 20.
    /// After a failure, further calls return false at once for WhitemapJoinRetryCooldownSeconds
    /// so wait loops do not spam joins. Callers carry on in the current map when it fails.
    /// </summary>
    public bool TryJoinWhitemapQuick()
    {
        if (string.Equals(Bot.Map?.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
            return true;

        if (
            whitemapJoinFailedRecently
            && unchecked(Environment.TickCount - lastWhitemapJoinFailTick) < Math.Max(10, WhitemapJoinRetryCooldownSeconds) * 1000
        )
            return false;

        LeaveCombatBeforeMapChange();

        string target = Core.PrivateRooms ? $"whitemap-{Core.PrivateRoomNumber}" : "whitemap";
        for (int i = 0; i < Math.Max(1, WhitemapJoinAttempts) && !Bot.ShouldExit; i++)
        {
            Bot.Map.Join(target, "Enter", "Spawn", autoCorrect: !Core.PrivateRooms);
            if (Bot.Wait.ForMapLoad("whitemap", 10))
            {
                whitemapJoinFailedRecently = false;
                return true;
            }
            Bot.Sleep(1000);
        }

        whitemapJoinFailedRecently = true;
        lastWhitemapJoinFailTick = Environment.TickCount;
        Core.Logger(
            $"Could not reach '/{target}' after {Math.Max(1, WhitemapJoinAttempts)} tries; staying on '/{DescribeCurrentRoom()}'. State: {DescribeWorkState()}",
            "CoreArmyFJ"
        );
        return false;
    }

    /// <summary>"map-room" of the loaded instance (just the map when the room is unknown).</summary>
    private string DescribeCurrentRoom()
    {
        string map = Bot.Map?.Name ?? "?";
        int room = GetCurrentRoomNumber();
        return room > 0 ? $"{map}-{room}" : map;
    }

    // ---------------------------------------------------------------------------------------
    // Full private rooms. The server answers a join/goto into a full room with a "warning"
    // packet containing "is full". Helpers used to ignore it and retried the same full room
    // every ~8s for 15+ min (thirdspell, castleundead, doomwood... 2026-11-25).
    // ---------------------------------------------------------------------------------------

    /// <summary>Text of a "room is full" warning packet, or null for any other packet.</summary>
    public static string? GetRoomFullWarningText(object? packetObj)
    {
        try
        {
            dynamic? packet = packetObj;
            if (packet == null)
                return null;

            dynamic? paramsObj = packet["params"];
            if (paramsObj == null || paramsObj.type != "str")
                return null;

            dynamic? dataObj = paramsObj.dataObj;
            if (dataObj == null)
                return null;

            string? cmd = dataObj[0];
            if (cmd != "warning")
                return null;

            // Only the warning's own fields ("warning", "-1", message). The full packet also carries
            // the socket data (ip, ports 5592/8080...), which looked like room numbers.
            string text = Convert.ToString(dataObj) ?? string.Empty;
            if (
                !text.Contains("is full", StringComparison.OrdinalIgnoreCase)
                || text.Contains("inventory", StringComparison.OrdinalIgnoreCase)
                || text.Contains("bank", StringComparison.OrdinalIgnoreCase)
                || text.Contains("house", StringComparison.OrdinalIgnoreCase)
                || text.Contains("bag", StringComparison.OrdinalIgnoreCase)
            )
                return null;

            return text;
        }
        catch
        {
            return null;
        }
    }

    private int roomFullWarnings;

    private void CountRoomFullWarning(dynamic packet)
    {
        if (GetRoomFullWarningText((object?)packet) != null)
            Interlocked.Increment(ref roomFullWarnings);
    }

    /// <summary>Runs a join/goto and reports whether the server answered "room is full" meanwhile.</summary>
    private bool TravelHitFullRoom(Action travel)
    {
        int before = Volatile.Read(ref roomFullWarnings);
        Bot.Events.ExtensionPacketReceived += CountRoomFullWarning;
        try
        {
            travel();
        }
        finally
        {
            try
            {
                Bot.Events.ExtensionPacketReceived -= CountRoomFullWarning;
            }
            catch { }
        }
        return Volatile.Read(ref roomFullWarnings) != before;
    }

    private bool RoomFullSince(int before) => Volatile.Read(ref roomFullWarnings) != before;

    // The teammate's room is at its player cap: it already has as much help as the map allows.
    // Park this teammate like an unreachable one (re-tried after UnreachableHelperRetrySeconds).
    private void MarkTeammateRoomFull(string normalizedTarget, string where)
    {
        failedGotoCounts.Remove(normalizedTarget);
        if (unreachableHelperTargets.Add(normalizedTarget))
        {
            Core.Logger(
                $"Teammate '{normalizedTarget}' is in a full room ({where}); not retrying for {Math.Max(30, UnreachableHelperRetrySeconds)}s.",
                "CoordinateFollowAndCombat"
            );
        }
    }

    // ---------------------------------------------------------------------------------------
    // Persistent map player caps (world.areaData.intMaxPlayers), learned whenever any Army
    // account stands in a map and shared by every account/session through one small file.
    // Lets playerOne split rooms without probing, including maps it should not probe.
    // ---------------------------------------------------------------------------------------
    private Dictionary<string, int>? knownMapCaps;
    private string lastRecordedCapMap = string.Empty;

    private static string MapCapsPath => Path.Combine(ClientFileSources.SkuaOptionsDIR, "ArmyMapCaps.txt");

    private Dictionary<string, int> LoadKnownMapCaps()
    {
        Dictionary<string, int> caps = new(StringComparer.OrdinalIgnoreCase);
        try
        {
            if (File.Exists(MapCapsPath))
            {
                foreach (string line in File.ReadAllLines(MapCapsPath))
                {
                    string[] parts = line.Split('=');
                    if (
                        parts.Length == 2
                        && int.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int max)
                        && max > 0
                    )
                        caps[parts[0].Trim()] = max;
                }
            }
        }
        catch { }

        return caps;
    }

    /// <summary>Known intMaxPlayers for a map (0 when never seen).</summary>
    public int GetKnownMapMaxPlayers(string? mapName)
    {
        string? key = NormalizeMapKey(mapName);
        if (string.IsNullOrEmpty(key))
            return 0;

        knownMapCaps ??= LoadKnownMapCaps();
        if (knownMapCaps.TryGetValue(key, out int max))
            return max;

        // Another account may have learned it since we loaded.
        knownMapCaps = LoadKnownMapCaps();
        return knownMapCaps.TryGetValue(key, out max) ? max : 0;
    }

    public void RecordMapMaxPlayers(string? mapName, int maxPlayers)
    {
        string? key = NormalizeMapKey(mapName);
        if (string.IsNullOrEmpty(key) || maxPlayers <= 0 || key == "whitemap")
            return;

        knownMapCaps ??= LoadKnownMapCaps();
        if (knownMapCaps.TryGetValue(key, out int existing) && existing == maxPlayers)
            return;

        knownMapCaps[key] = maxPlayers;
        try
        {
            File.AppendAllText(MapCapsPath, $"{key}={maxPlayers}{Environment.NewLine}");
        }
        catch { }
    }

    /// <summary>Records the cap of the map this account is standing in (cheap; only when the map changed).</summary>
    public void RecordCurrentMapCap()
    {
        try
        {
            string? map = NormalizeMapKey(Bot.Map?.Name);
            if (string.IsNullOrEmpty(map) || string.Equals(map, lastRecordedCapMap, StringComparison.OrdinalIgnoreCase) || !Bot.Map.Loaded)
                return;

            int max = DetectCurrentMapMaxPlayers();
            if (max <= 0)
                return;

            RecordMapMaxPlayers(map, max);
            lastRecordedCapMap = map;
        }
        catch { }
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

        DeleteOldArmySideFiles();

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
        DateTime lastLeaderCheck = DateTime.UtcNow;

        while (!Bot.ShouldExit && !armyTransportFailed)
        {
            string[] lines = ReadArmyLines();
            string? sessionId = FindLatestArmyReset(lines);
            if (
                sessionId == null
                || string.Equals(sessionId, armyIgnoredSessionId, StringComparison.Ordinal)
            )
            {
                Bot.Sleep(ArmyPollDelay);
                continue;
            }

            if (HasAnyArmyStart(lines, sessionId))
            {
                if (HasMatchingArmyStart(lines, sessionId))
                {
                    if (HasArmyStop(lines, sessionId))
                        return false;

                    armySessionId = sessionId;
                    return true;
                }

                // This session started without us (our bot was restarted mid-run). If it is still
                // live, ask playerOne to re-create it; then wait for the next RESET.
                if (!HasArmyStop(lines, sessionId))
                {
                    Core.Logger("Army is already running without this account; asking playerOne to restart the session.", "CoreArmyFJ");
                    AppendArmyRecord(new[] { "REJOIN", ArmyProtocolVersion, sessionId, armyUsername });
                }

                armyIgnoredSessionId = sessionId;
                continue;
            }

            if (!string.Equals(readySession, sessionId, StringComparison.Ordinal))
            {
                if (!WriteArmyReady(sessionId))
                    return false;

                readySession = sessionId;
            }

            if ((DateTime.UtcNow - lastLeaderCheck).TotalSeconds >= 15 && armyPlayers.Length > 0)
            {
                lastLeaderCheck = DateTime.UtcNow;
                WarnAboutStaleArmyPlayers(new[] { armyPlayers[0] });
            }

            Bot.Sleep(ArmyPollDelay);
        }

        return false;
    }

    private bool HasAnyArmyStart(string[] lines, string sessionId)
    {
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length >= 3
                && parts[0] == "START"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], sessionId, StringComparison.Ordinal)
            )
                return true;
        }

        return false;
    }

    // MAPCAP / UNSAFESPAWN / AllPlayersSatisfy flag files are per session; drop the old ones
    // (heartbeat files are kept, live teammates keep rewriting them).
    private void DeleteOldArmySideFiles()
    {
        try
        {
            string? dir = Path.GetDirectoryName(armySyncPath);
            string prefix = Path.GetFileName(armySyncPath) + ".";
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return;

            foreach (string file in Directory.GetFiles(dir, prefix + "*"))
            {
                string name = Path.GetFileName(file);
                if (
                    !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    || name.Contains(".HB.", StringComparison.Ordinal)
                )
                    continue;
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
        catch { }
    }

    private bool WriteArmyReady(string sessionId) =>
        AppendArmyRecord(
            new[] { "READY", ArmyProtocolVersion, sessionId, armyUsername, armyLaunchToken }
        );

    private Dictionary<string, string>? WaitForArmyReady(string sessionId)
    {
        string previousMissing = string.Empty;
        DateTime lastStaleCheck = DateTime.UtcNow;
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

            if ((DateTime.UtcNow - lastStaleCheck).TotalSeconds >= 15)
            {
                lastStaleCheck = DateTime.UtcNow;
                WarnAboutStaleArmyPlayers(armyPlayers.Where(player => !tokens.ContainsKey(player)));
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
        DateTime lastStaleCheck = DateTime.UtcNow;
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

            if ((DateTime.UtcNow - lastStaleCheck).TotalSeconds >= 15)
            {
                lastStaleCheck = DateTime.UtcNow;
                WarnAboutStaleArmyPlayers(armyPlayers.Where(player => !arrived.Contains(player)));
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
                string mismatch =
                    $"sync step mismatch: playerOne is at '{stepName}' but this account reached '{expectedStepName}'";
                Core.Logger($"Army {mismatch}. Stopping the Army so nobody waits forever.", "CoreArmyFJ");
                ReportArmyFailure(mismatch);
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
        if (armyRestartRequested)
            return false;

        if (
            latestSessionId != null
            && !string.Equals(latestSessionId, armySessionId, StringComparison.Ordinal)
        )
        {
            // playerOne re-created the session (it restarted, or a teammate asked to rejoin).
            if (!armySessionFailureLogged)
            {
                Core.Logger("Army session was re-created by playerOne; restarting this run to rejoin.", "CoreArmyFJ");
                armySessionFailureLogged = true;
            }

            armyRestartRequested = true;
            return false;
        }

        (string User, string Reason)? failure = FindArmyFailure(lines);
        if (failure != null)
        {
            if (!armyFailureLogged)
            {
                Core.Logger(
                    $"Army stopped: {LogArmyName(failure.Value.User)} ({failure.Value.User}) failed: {failure.Value.Reason}",
                    "CoreArmyFJ",
                    messageBox: true
                );
                armyFailureLogged = true;
            }

            return false;
        }

        string? rejoining = FindArmyRejoin(lines);
        if (rejoining != null)
        {
            Core.Logger(
                $"{LogArmyName(rejoining)} restarted its bot and asked to rejoin; restarting the Army session.",
                "CoreArmyFJ"
            );
            armyRestartRequested = true;
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

    private (string User, string Reason)? FindArmyFailure(string[] lines)
    {
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length == 5
                && parts[0] == "FAIL"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
            )
                return (NormalizeArmyUsername(parts[3]), parts[4]);
        }

        return null;
    }

    private string? FindArmyRejoin(string[] lines)
    {
        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (
                parts.Length == 4
                && parts[0] == "REJOIN"
                && parts[1] == ArmyProtocolVersion
                && string.Equals(parts[2], armySessionId, StringComparison.Ordinal)
            )
            {
                string user = NormalizeArmyUsername(parts[3]);
                if (
                    !string.Equals(user, armyUsername, StringComparison.OrdinalIgnoreCase)
                    && armyPlayers.Contains(user, StringComparer.OrdinalIgnoreCase)
                )
                    return user;
            }
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
            case "FAIL" when fields.Count > 4:
                Core.Logger($"Army failure reported to all accounts: {fields[4]}", "CoreArmyFJ");
                break;
            case "REJOIN":
                Core.Logger("Army rejoin requested.", "CoreArmyFJ");
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
        armyRestartRequested = false;
        armyFailureLogged = false;
        armyIgnoredSessionId = string.Empty;
        armyStaleWarnedAt.Clear();
        lastRecordedCapMap = string.Empty;
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

    public int ArmyPlayerCount => armyPlayers.Length;

    public int DetectCurrentMapMaxPlayers(string? expectedMap = null)
    {
        try
        {
            if (
                !string.IsNullOrWhiteSpace(expectedMap)
                && !string.Equals(Bot.Map?.Name, expectedMap, StringComparison.OrdinalIgnoreCase)
            )
                return 0;

            int maxPlayers = Bot.Flash.GetGameObject<int>("world.areaData.intMaxPlayers");
            return maxPlayers > 0 ? maxPlayers : 0;
        }
        catch
        {
            return 0;
        }
    }

    public void PublishMapSubgroupCap(string objectiveName, int cap)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return;

        string safeName = NormalizeArmyRecordName(objectiveName);
        if (string.IsNullOrEmpty(safeName))
            safeName = "MAP";

        string capPath = $"{armySyncPath}.MAPCAP.{safeName}.{armySessionId}";
        // Raw cap (0 = unknown); a cap >= Army size means one shared room.
        int normalizedCap = cap > 0 ? cap : 0;
        try
        {
            File.WriteAllText(capPath, normalizedCap.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        catch { }
    }

    public int ReadMapSubgroupCap(string objectiveName)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return 0;

        string safeName = NormalizeArmyRecordName(objectiveName);
        if (string.IsNullOrEmpty(safeName))
            safeName = "MAP";

        string capPath = $"{armySyncPath}.MAPCAP.{safeName}.{armySessionId}";
        try
        {
            if (!File.Exists(capPath))
                return 0;

            string raw = File.ReadAllText(capPath).Trim();
            if (int.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int cap))
                return cap > 0 ? cap : 0;
        }
        catch { }

        return 0;
    }

    public void PublishMapUnsafeSpawn(string mapName)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return;

        string safeName = NormalizeArmyRecordName(mapName);
        if (string.IsNullOrEmpty(safeName))
            return;

        string path = $"{armySyncPath}.UNSAFESPAWN.{safeName}.{armySessionId}";
        try
        {
            File.WriteAllText(path, "1");
        }
        catch { }
    }

    // Overflow room of each account. A pending account bumped out of a full room by the server
    // lands in an overflow room only it knows; /goto across private rooms is unreliable, so helpers
    // read this side file and join "{map}-{room}" directly. 0 / missing = planned room.
    private string? OverflowRoomPath(int playerNumber) =>
        !armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath) || playerNumber < 1
            ? null
            : $"{armySyncPath}.OVERFLOW.P{playerNumber}.{armySessionId}";

    // ---------------------------------------------------------------------------------------
    // Learned room caps. world.areaData.intMaxPlayers reads 0 on every map, so the cap is
    // learned from the server instead: an account whose join is answered "destination room is
    // full" reports objective map + room; an account inside that room records its player count
    // (teammates + outsiders) as the map cap in ArmyMapCaps.txt. Later objectives on that map
    // (and later runs) then split the Army into rooms up front.
    // ---------------------------------------------------------------------------------------
    private string? currentObjectiveMap;
    private readonly HashSet<string> processedRoomFullReports = new(StringComparer.OrdinalIgnoreCase);
    private const int RoomFullReportFreshSeconds = 30;

    private string? RoomFullReportPath(string map, int room) =>
        !armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath) || room <= 0
            ? null
            : $"{armySyncPath}.ROOMFULL.{map}.{room}.{armySessionId}";

    /// <summary>This account's join into <paramref name="room"/> of the current objective map was rejected as full.</summary>
    public void ReportRoomFull(int room)
    {
        string? map = currentObjectiveMap;
        if (string.IsNullOrEmpty(map))
            return;

        string? path = RoomFullReportPath(map, room);
        if (path == null)
            return;

        try
        {
            File.WriteAllText(path, DateTime.UtcNow.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        catch { }
    }

    /// <summary>Inside a room a teammate was just rejected from: record its player count as the map cap.</summary>
    public void LearnMapCapFromRoomFullReports()
    {
        try
        {
            string? map = ReadCurrentMapKey();
            int room = GetCurrentRoomNumber();
            if (string.IsNullOrEmpty(map) || room <= 0 || !Bot.Map.Loaded)
                return;

            // A learned cap is only a lower bound: someone may have left before it was read.
            // More players seen in one room than the cap -> raise it.
            int knownCap = GetKnownMapMaxPlayers(map);
            int seenHere = Bot.Map.PlayerNames?.Count() ?? 0;
            if (knownCap > 0 && seenHere > knownCap)
            {
                RecordMapMaxPlayers(map, seenHere);
                Core.Logger($"Raised room cap for '/{map}': {seenHere} players seen in room {room} (was {knownCap}).", "CoreArmyFJ");
            }

            string? path = RoomFullReportPath(map, room);
            if (path == null || !File.Exists(path))
                return;

            string raw = File.ReadAllText(path).Trim();
            if (!long.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long ticks))
                return;

            // Called from the objective loop and the StallGuard thread.
            lock (processedRoomFullReports)
            {
                if (!processedRoomFullReports.Add($"{path}|{ticks}"))
                    return;
            }

            if (DateTime.UtcNow - new DateTime(ticks, DateTimeKind.Utc) > TimeSpan.FromSeconds(RoomFullReportFreshSeconds))
                return;

            int playersHere = Bot.Map.PlayerNames?.Count() ?? 0;
            if (playersHere < 2)
                return;

            int known = GetKnownMapMaxPlayers(map);
            if (known > 0 && known <= playersHere)
                return;

            RecordMapMaxPlayers(map, playersHere);
            Core.Logger(
                $"Learned room cap for '/{map}': {playersHere} players/room (a teammate was rejected from room {room}; previous cap {(known > 0 ? known.ToString() : "unknown")}).",
                "CoreArmyFJ"
            );
        }
        catch { }
    }

    public void PublishOverflowRoom(int room)
    {
        string? path = OverflowRoomPath(armyPlayerIndex + 1);
        if (path == null)
            return;

        try
        {
            if (room > 0)
                File.WriteAllText(path, room.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (File.Exists(path))
                File.Delete(path);
        }
        catch { }
    }

    public int ReadOverflowRoom(int playerNumber)
    {
        string? path = OverflowRoomPath(playerNumber);
        if (path == null)
            return 0;

        try
        {
            if (!File.Exists(path))
                return 0;

            string raw = File.ReadAllText(path).Trim();
            return int.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int room) && room > 0
                ? room
                : 0;
        }
        catch
        {
            return 0;
        }
    }

    public int FindArmyPlayerNumber(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return 0;

        string normalized = NormalizeArmyUsername(username);
        for (int i = 0; i < armyPlayers.Length; i++)
        {
            if (string.Equals(NormalizeArmyUsername(armyPlayers[i]), normalized, StringComparison.OrdinalIgnoreCase))
                return i + 1;
        }
        return 0;
    }

    public bool IsMapUnsafeSpawn(string mapName)
    {
        if (!armySessionStarted || string.IsNullOrWhiteSpace(armySyncPath))
            return false;

        string safeName = NormalizeArmyRecordName(mapName);
        if (string.IsNullOrEmpty(safeName))
            return false;

        string path = $"{armySyncPath}.UNSAFESPAWN.{safeName}.{armySessionId}";
        try
        {
            return File.Exists(path);
        }
        catch
        {
            return false;
        }
    }
}

