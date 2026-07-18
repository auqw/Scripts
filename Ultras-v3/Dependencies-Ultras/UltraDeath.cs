/*
name: UltraDeath
description: Army death sync — detects and propagates death events across army members.
tags: ultra, death, sync, army
*/

//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class UltraDeath
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 C => CoreBots2.Instance;
    private CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private CoreUltra2 _Ultra;

    private const string DeathSyncFile = "ultra_death.sync";

    /// <summary>
    /// Signals that this player has died. Writes the death timestamp to the sync file.
    /// </summary>
    public void SignalDeath()
    {
        string path = Ultra.ResolveSyncPath(DeathSyncFile);
        string key = $"{Bot.Player.Username}|death";
        Ultra.UpdateEntry(path, key, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
    }

    /// <summary>
    /// Returns true if any army member has died recently (within the stale threshold).
    /// </summary>
    public bool HasDeathOccurred()
    {
        string path = Ultra.ResolveSyncPath(DeathSyncFile);
        string[] lines = Ultra.ReadLines(path);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 30;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 2)
                continue;

            if (!long.TryParse(parts[1], out long ts))
                continue;

            if (now - ts <= staleThreshold)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Counts how many unique army members have died recently (within the stale threshold).
    /// </summary>
    public int CountDeadPlayers(int staleThresholdSec = 30)
    {
        string path = Ultra.ResolveSyncPath(DeathSyncFile);
        string[] lines = Ultra.ReadLines(path);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var deadPlayers = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 2)
                continue;

            string? username = parts[0]?.Trim();
            if (string.IsNullOrEmpty(username))
                continue;

            // Extract just the username from "username|death" format
            string[] keyParts = username.Split('|');
            if (keyParts.Length < 1 || string.IsNullOrEmpty(keyParts[0]))
                continue;

            if (!long.TryParse(parts[1], out long ts))
                continue;

            if (now - ts <= staleThresholdSec)
                deadPlayers.Add(keyParts[0]);
        }

        return deadPlayers.Count;
    }

    /// <summary>
    /// Returns true when at least <paramref name="requiredDead"/> unique army members
    /// have died within the stale threshold. Use this for army-wipe detection.
    /// </summary>
    public bool IsArmyWiped(int requiredDead, int staleThresholdSec = 15)
    {
        return CountDeadPlayers(staleThresholdSec) >= requiredDead;
    }

    /// <summary>
    /// Clears all death entries from the sync file.
    /// </summary>
    public void ClearDeaths()
    {
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(DeathSyncFile));
    }

    /// <summary>
    /// Async loop that monitors this player's death and signals the army.
    /// Call with a CancellationToken to stop it.
    /// </summary>
    public async Task MonitorDeathAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    SignalDeath();
                    C.Logger("[UltraDeath] Death detected, signaling army.");
                }

                await Task.Delay(500, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Waits (async, polling) until any player in the army has died, up to <paramref name="timeoutSecs"/>.
    /// Returns true if a death was detected, false on timeout.
    /// </summary>
    public async Task<bool> WaitForDeathAsync(int timeoutSecs = 60, CancellationToken token = default)
    {
        var start = DateTime.UtcNow;

        while (!token.IsCancellationRequested)
        {
            if (HasDeathOccurred())
                return true;

            if ((DateTime.UtcNow - start).TotalSeconds >= timeoutSecs)
                return false;

            try
            {
                await Task.Delay(500, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Spawns a background thread that:
    /// 1. Monitors this player's death and signals it via <see cref="SignalDeath"/>
    /// 2. Checks if the full army is wiped
    /// 3. On wipe: cancels <paramref name="wipeCts"/> and runs <paramref name="onWipe"/>
    /// Signals <paramref name="retreatComplete"/> when done.
    /// </summary>
    public static void StartWipeMonitor(
        CoreBots2 C,
        int armySize,
        CancellationTokenSource wipeCts,
        ManualResetEvent retreatComplete,
        Action? onWipe = null
    )
    {
        var t = new Thread(() => WipeMonitorLoop(C, armySize, wipeCts, retreatComplete, onWipe));
        t.IsBackground = true;
        t.Start();
    }

    private static void WipeMonitorLoop(
        CoreBots2 C,
        int armySize,
        CancellationTokenSource wipeCts,
        ManualResetEvent retreatComplete,
        Action? onWipe
    )
    {
        var death = new UltraDeath();
        var bot = IScriptInterface.Instance;

        while (!wipeCts.IsCancellationRequested)
        {
            try
            {
                // Signal this player's death if dead
                if (!bot.Player.Alive)
                {
                    death.SignalDeath();
                    C.Logger("[UltraDeath] Death detected, signaling army.");
                }

                // Check for army wipe
                if (death.IsArmyWiped(armySize))
                {
                    C.Logger("[UltraDeath] Army wipe detected — running retreat.");
                    wipeCts.Cancel();

                    onWipe?.Invoke();

                    return;
                }

                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                C.Logger($"[UltraDeath] WipeMonitor error: {ex.Message}");
            }
            finally
            {
                if (wipeCts.IsCancellationRequested)
                    retreatComplete.Set();
            }
        }

        retreatComplete.Set();
    }

    /// <summary>
    /// Lightweight mutable wrapper for an integer, so it can be safely captured in lambdas
    /// and modified across threads.
    /// </summary>
    public class RetryCounter
    {
        public int Value;
    }

    /// <summary>
    /// Performs the retreat sequence from a background thread.
    /// Call this as the <c>onWipe</c> callback from <see cref="StartWipeMonitor"/>.
    /// </summary>
    public static void PerformRetreat(
        CoreBots2 C,
        int armySize,
        int maxRetries,
        RetryCounter retryCount,
        string retreatSyncFile
    )
    {
        int currentRetries = Interlocked.Increment(ref retryCount.Value);
        C.Logger($"[UltraDeath] Retreating. Retry {currentRetries}/{maxRetries}.");

        new CoreUltra2().PersistentJoinHouse();

        if (currentRetries >= maxRetries)
        {
            C.Logger($"{maxRetries} Retries, Stopping the scripts.", messageBox: true, stopBot: true);
            return;
        }

        new UltraDeath().ClearDeaths();
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, retreatSyncFile, useSkill: false);
        C.Logger("[UltraDeath] All retreated. Restarting fight.");
    }
}
