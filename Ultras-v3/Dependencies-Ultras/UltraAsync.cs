/*
name: UltraAsync
description: Pulse-driven taunt loop for Ultra boss scripts.
tags: ultra, async, taunt, pulse
*/

//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPulse.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class UltraAsync
{
    public static DateTime SetFightTime(CoreBots2 C, string syncPath)
    {
        var now = DateTime.UtcNow;
        File.WriteAllText(syncPath, now.Ticks.ToString());
        C.Logger($"[FightTime] Primary set fight start: {now:HH:mm:ss.fff} UTC");
        return now;
    }

    public static DateTime GetFightTime(CoreUltra2 ultra, CoreBots2 C, string syncPath, int timeoutSecs = 60)
    {
        C.Logger($"[FightTime] Waiting for Primary to set fight start (timeout: {timeoutSecs}s)...");
        var start = DateTime.UtcNow;
        while ((DateTime.UtcNow - start).TotalSeconds < timeoutSecs)
        {
            string[] lines = ultra.ReadLines(syncPath);
            if (lines.Length > 0 && long.TryParse(lines[0], out long ticks) && ticks > 0)
            {
                var time = new DateTime(ticks, DateTimeKind.Utc);
                double ageSecs = (DateTime.UtcNow - time).TotalSeconds;
                if (ageSecs > 10)
                {
                    C.Logger($"[FightTime] Skipping stale timestamp ({ageSecs:F0}s old) — waiting for fresh write...");
                    Thread.Sleep(500);
                    continue;
                }
                C.Logger($"[FightTime] Read fight start: {time:HH:mm:ss.fff} UTC");
                return time;
            }
            Thread.Sleep(500);
        }
        C.Logger("[FightTime] TIMEOUT — Primary never set fight time. Falling back to UtcNow.");
        return DateTime.UtcNow;
    }

    public static void StartTauntLoop(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterIndex, int taunterCount = 2, Func<bool>? shouldSkipTaunt = null, int pulseIntervalSec = 5, CancellationToken cancellationToken = default)
    {
        var t = new Thread(() => TauntPulseLoop(bot, C, engine, fightStart, taunterIndex, taunterCount, shouldSkipTaunt, pulseIntervalSec, cancellationToken));
        t.IsBackground = true;
        t.Start();
    }

    private static void TauntPulseLoop(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterIndex, int taunterCount, Func<bool>? shouldSkipTaunt = null, int pulseIntervalSec = 5, CancellationToken cancellationToken = default)
    {
        int lastPulse = -1;
        string label = GetTaunterLabel(taunterIndex);
        int firstPulseSec = taunterIndex * pulseIntervalSec;
        int intervalSec = taunterCount * pulseIntervalSec;
        C.Logger($"[{label}] Pulse taunt loop started. Fires every {intervalSec}s (first at t={firstPulseSec}s, interval={pulseIntervalSec}s).{(shouldSkipTaunt != null ? " Skip condition enabled." : "")}");

        while (!bot.ShouldExit && !cancellationToken.IsCancellationRequested)
        {
            if (!bot.Player.Alive)
            {
                Thread.Sleep(500);
                continue;
            }

            int currentPulse = UltraPulse.CurrentPulse(fightStart, pulseIntervalSec);

            if (currentPulse != lastPulse)
            {
                lastPulse = currentPulse;

                if (currentPulse % taunterCount == taunterIndex)
                {
                    // Check skip condition before taunting
                    if (shouldSkipTaunt != null && shouldSkipTaunt())
                    {
                        double elapsed = (DateTime.UtcNow - fightStart).TotalSeconds;
                        C.Logger($"[{label}] Pulse {currentPulse} at t={elapsed:F1}s — SKIPPED (skip condition met).");
                        continue;
                    }

                    double elapsed2 = (DateTime.UtcNow - fightStart).TotalSeconds;
                    C.Logger($"[{label}] Pulse {currentPulse} — taunting at t={elapsed2:F1}s.");
                    TauntPresses(bot, C, engine, cancellationToken);
                }
            }

            Thread.Sleep(250);
        }
    }

    private static string GetTaunterLabel(int index)
    {
        return index switch
        {
            0 => "Primary",
            1 => "Secondary",
            2 => "Tertiary",
            3 => "Quaternary",
            _ => $"Taunter-{index}"
        };
    }

    public static void TauntPresses(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, CancellationToken cancellationToken = default)
    {
        var start = DateTime.UtcNow;
        C.Logger("[Taunt] Starting 60 presses...");
        int pressed = 0;
        for (int i = 0; i < 60 && !bot.ShouldExit && !cancellationToken.IsCancellationRequested; i++)
        {
            if (!bot.Player.Alive)
            {
                C.Logger("[Taunt] Died during taunt — aborting.");
                return;
            }
            if (cancellationToken.IsCancellationRequested)
            {
                C.Logger("[Taunt] Cancellation requested — aborting presses.");
                return;
            }
            engine.Cast(5);
            pressed++;

            double nextPressMs = (DateTime.UtcNow - start).TotalMilliseconds + 50;
            while ((DateTime.UtcNow - start).TotalMilliseconds < nextPressMs && !cancellationToken.IsCancellationRequested)
                Thread.SpinWait(10);
        }
        double took = (DateTime.UtcNow - start).TotalSeconds;
        C.Logger($"[Taunt] Done — {pressed} presses in {took:F2}s.");
    }

    public static async Task PrimaryTauntLoopAsync(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterCount = 2, int pulseIntervalSec = 5)
        => await Task.Run(() => TauntPulseLoop(bot, C, engine, fightStart, 0, taunterCount, null, pulseIntervalSec));

    public static async Task SecondaryTauntLoopAsync(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterCount = 2, int pulseIntervalSec = 5)
        => await Task.Run(() => TauntPulseLoop(bot, C, engine, fightStart, 1, taunterCount, null, pulseIntervalSec));

    public static async Task TertiaryTauntLoopAsync(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterCount = 3, int pulseIntervalSec = 5)
        => await Task.Run(() => TauntPulseLoop(bot, C, engine, fightStart, 2, taunterCount, null, pulseIntervalSec));

    public static async Task QuaternaryTauntLoopAsync(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine, DateTime fightStart, int taunterCount = 4, int pulseIntervalSec = 5)
        => await Task.Run(() => TauntPulseLoop(bot, C, engine, fightStart, 3, taunterCount, null, pulseIntervalSec));

    public static async Task TauntAsync(IScriptInterface bot, CoreBots2 C, CoreEngine2 engine)
        => await Task.Run(() => TauntPresses(bot, C, engine));
}
