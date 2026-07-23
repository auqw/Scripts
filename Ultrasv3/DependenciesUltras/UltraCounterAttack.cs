/*
name: UltraCounterAttack
description: Detects the "Counter Attack" aura (4s) via ExtensionPacketReceived.
             Stops attacking immediately and resumes via a 4s timer when the aura expires.
             No messenger dependency — purely aura-based.
tags: ultra, counter, attack
*/

//cs_include Scripts/CoreBots.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class UltraCounterAttack
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;

    private static bool _enabled = false;
    private static bool _counterActive = false;
    private static string? _savedTarget = null;
    private static CancellationTokenSource? _timerCts = null;

    /// <summary>
    /// Enables Counter Attack handling. Listens for the "Counter Attack" aura on
    /// every server packet. On aura appeared → StopAttacking + 4s timer.
    /// On timer expiry → resume attacking. The aura fade is NOT used for resume
    /// because CancelTarget (triggered by StopAttacking) wipes the target, making
    /// consecutive aura checks unreliable. The 4s timer is the source of truth.
    /// </summary>
    public static void Enable()
    {
        if (_enabled)
            return;

        _enabled = true;
        _counterActive = false;
        _savedTarget = null;
        _timerCts = null;

        Bot.Events.ExtensionPacketReceived += OnExtensionPacket;

        C.Logger("[UltraCounterAttack] Enabled.");
    }

    /// <summary>
    /// Disables Counter Attack handling, unsubscribes listeners, and cleans up state.
    /// </summary>
    public static void Disable()
    {
        if (!_enabled)
            return;

        _enabled = false;
        _counterActive = false;
        _savedTarget = null;

        _timerCts?.Cancel();
        _timerCts = null;

        Bot.Events.ExtensionPacketReceived -= OnExtensionPacket;
        Bot.Combat.StopAttacking = false;

        C.Logger("[UltraCounterAttack] Disabled.");
    }

    /// <summary>
    /// Fires on every server packet. Detects the "Counter Attack" aura (4s reflect).
    /// On first sight of the aura → stop attacking + 4s timer.
    /// When the aura is not visible (target wiped by CancelTarget) → just reset flag
    /// so the next Counter Attack is detected. The 4s timer handles the resume.
    /// </summary>
    private static void OnExtensionPacket(dynamic packet)
    {
        try
        {
            bool auraNow = Bot.Player?.Alive == true
                && Bot.Player.HasTarget
                && Bot.Target?.Auras?.Any(a => a != null && a.Name == "Counter Attack") == true;

            if (auraNow && !_counterActive)
                StartCounterAttackTimer("aura");
            else if (!auraNow && _counterActive)
                // Target likely wiped by CancelTarget — just reset the flag so the
                // next Counter Attack is detected. The 4s timer handles the resume.
                _counterActive = false;
        }
        catch { }
    }

    /// <summary>
    /// Sets StopAttacking = true, saves the target name, and fires a background
    /// Task that resumes attacking after 4 seconds. No messenger interaction.
    /// </summary>
    private static void StartCounterAttackTimer(string source)
    {
        _counterActive = true;
        _savedTarget = Bot.Player.Target?.Name;
        Bot.Combat.StopAttacking = true;
        C.Logger($"[UltraCounterAttack] Counter Attack aura detected ({source}) — stopping attacks for 4s.");

        _timerCts?.Cancel();
        _timerCts = new();
        var token = _timerCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(4000, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (token.IsCancellationRequested)
                return;

            _timerCts = null;
            Bot.Combat.StopAttacking = false;
            if (!string.IsNullOrWhiteSpace(_savedTarget))
                Bot.Combat.Attack(_savedTarget);
            _savedTarget = null;
            C.Logger("[UltraCounterAttack] Counter Attack window expired — resuming attacks.");
        });
    }

    /// <summary>
    /// Whether Counter Attack handling is currently enabled.
    /// </summary>
    public static bool IsEnabled => _enabled;

    /// <summary>
    /// Whether a Counter Attack is currently active (attacks are being stopped).
    /// </summary>
    public static bool IsActive => _counterActive;
}
