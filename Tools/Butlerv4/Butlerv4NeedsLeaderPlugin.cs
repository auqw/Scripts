/*
name: Butler version 4
description: Follows a player using Goto; sync-file fallback for locked zones; transition-safe goto; reduced blinking; combat-aware syncing; server-aware sleeping.
tags: butler, follow, sync, locked-zone, quickdeaggro, lowlog
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Butlerv4NeedsLeaderPlugin
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "Butler1";

    public List<IOption> Options = new()
    {
        new Option<string>("Leader1Name", "Leader 1 Name", "Name of leader 1.", ""),
        new Option<string>("Leader1Butlers", "Butlers For Leader 1", "Comma-separated butler account names. Example: acc1,acc2,acc3", ""),
        new Option<string>("Leader2Name", "Leader 2 Name", "Name of leader 2.", ""),
        new Option<string>("Leader2Butlers", "Butlers For Leader 2", "Comma-separated butler account names. Example: acc1,acc2,acc3", ""),
        new Option<string>("Leader3Name", "Leader 3 Name", "Name of leader 3.", ""),
        new Option<string>("Leader3Butlers", "Butlers For Leader 3", "Comma-separated butler account names. Example: acc1,acc2,acc3", ""),
        new Option<string>("Leader4Name", "Leader 4 Name", "Name of leader 4.", ""),
        new Option<string>("Leader4Butlers", "Butlers For Leader 4", "Comma-separated butler account names. Example: acc1,acc2,acc3", ""),
        new Option<bool>("AutoEnhance", "Enable Auto-Enhance", "Automatically enhance your class when starting.", true),
        CoreBots.Instance.SkipOptions,
    };

    volatile bool LockedZoneWarning;
    volatile bool GotoIsOff;
    volatile bool RoomFull;
    volatile bool IsParked;

    string playerName = string.Empty;
    string syncFilePath = "";

    private int noSyncFileCounter = 0;
    private const int noSyncFileThreshold = 5;
    private volatile bool _gotoPending;
    private DateTime _lastGotoTime = DateTime.MinValue;
    private const int GotoMinIntervalMs = 500;
    private const int LeaderIdleTimeoutMinutes = 3;
    private DateTime _lastLeaderAttackTime = DateTime.UtcNow;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        Core.LoggerInChat = false;

        Bot.Events.ExtensionPacketReceived += ChatListener;
        Execute();
        Bot.Events.ExtensionPacketReceived -= ChatListener;

        Core.SetOptions(false);
    }

    public void Execute()
    {
        Core.Join("whitemap-100000");

        string myUsername = Bot.Player.Username ?? "";
        int leaderIndex = 0;

        for (int i = 1; i <= 4; i++)
        {
            string butlerList = Bot.Config!.Get<string>($"Leader{i}Butlers") ?? "";
            var butlers = butlerList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (butlers.Any(b => string.Equals(b, myUsername, StringComparison.OrdinalIgnoreCase)))
            {
                leaderIndex = i;
                playerName = Bot.Config!.Get<string>($"Leader{i}Name") ?? "";
                break;
            }
        }

        if (string.IsNullOrEmpty(playerName))
        {
            Core.Logger($"This account ({myUsername}) is not assigned to any leader's butler list.", messageBox: true, stopBot: true);
            return;
        }

        Core.Logger($"Following Leader {leaderIndex}: {playerName}");

        if (playerName == Bot.Player.Username)
        {
            Core.Logger("Cannot follow yourself.", messageBox: true, stopBot: true);
            return;
        }

        TryAutoEnhance();

        string charLocDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua",
            "character_locations"
        );
        Directory.CreateDirectory(charLocDir);

        syncFilePath = Path.Combine(charLocDir, $"{playerName}_location.sync");

        long lastTicks = 0;
        int staleCounter = 0;
        const int staleThreshold = 60;

        while (!Bot.ShouldExit)
        {
            while (!Bot.ShouldExit && Bot.Player?.Alive != true) Core.Sleep(250);

            if (!TryReadSyncData(out var data))
                continue;

            // Stale tick detection
            if (data.Ticks == lastTicks)
            {
                staleCounter++;
                if (staleCounter >= staleThreshold)
                {
                    EnterSafeState("Leader offline (stale ticks), parking");
                    staleCounter = 0;
                }
                Core.Sleep(250);
                continue;
            }
            staleCounter = 0;
            lastTicks = data.Ticks;

            if (!data.LeaderLoggedIn) { EnterSafeState("Leader logged out, parking"); continue; }

            // Leader idle detection: reset timer when leader attacks, park after 3 minutes idle
            if (data.LeaderInCombat || data.LeaderHasTarget)
                _lastLeaderAttackTime = DateTime.UtcNow;
            else if ((DateTime.UtcNow - _lastLeaderAttackTime).TotalMinutes >= LeaderIdleTimeoutMinutes)
            {
                Core.Logger("Leader hasn't attacked, parking");
                EnterSafeState($"Leader hasn't attacked in {LeaderIdleTimeoutMinutes} minutes.");
                continue;
            }

            if (!IsSameServer(data.LeaderServer))
            {
                EnterSafeState("Leader changed server, parking");
                continue;
            }

            string currentMap = Bot.Map.Name?.ToLower() ?? "";
            string currentCell = Bot.Player?.Cell ?? "Enter";

            bool mapMismatch = !string.Equals(data.Map, currentMap, StringComparison.OrdinalIgnoreCase);
            bool cellMismatch = !string.Equals(currentCell, data.Cell, StringComparison.OrdinalIgnoreCase);

            if (mapMismatch || cellMismatch)
            {
                HandleMapOrCellMismatch(data);
                continue;
            }

            if (GotoIsOff)
            {
                Core.Logger($"{playerName} goto disabled. Stopping.");
                break;
            }

            if (RoomFull)
            {
                TryHandleRoomFull();
                continue;
            }

            if (!mapMismatch)
                HandlePadMismatch(data);

            UpdateCombatState(data);
            Core.Sleep(250);
        }

        Bot.Events.ExtensionPacketReceived -= ChatListener;
        QuickDeaggro();
    }

    private bool IsButlerAlive()
    {
        return Bot.Player?.Alive == true;
    }

    private void AntiAutoAggro(string oldCell, string oldPad)
    {
        if (!IsButlerAlive())
            return;

        Core.Sleep(50);

        if (Bot.Player.Cell == oldCell && Bot.Player.Pad == oldPad &&
            (Bot.Player.InCombat || Bot.Player.HasTarget))
        {
            Core.JumpWait();
        }
    }

    void QuickDeaggro()
    {
        if (!IsButlerAlive())
            return;

        if (Bot.Player.InCombat || (Bot.Player.HasTarget && (Bot.Player.Target?.HP ?? 0) > 0))
        {
            DontAttack();
            Bot.Map.Jump(Bot.Player.Cell ?? "Enter", Bot.Player.Pad ?? "Left");
        }
    }

    void DontAttack()
    {
        Bot.Combat.CancelTarget();
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
    }

    void EnterSafeState(string reason)
    {
        if (!IsParked)
        {
            Core.Logger(reason);

            Core.Join("whitemap-100000");
            Bot.Wait.ForMapLoad("whitemap");
            JoinHouse();

            IsParked = true;
        }
        else
        {
            Core.Logger("Safe parked. Sleeping 10 seconds.");
        }

        Core.Sleep(10000);
    }

    void JoinHouse()
    {
        try
        {
            if (Bot.House.Items.Any(h => h.Equipped))
            {
                string? toSend = null;

                void modifyPacket(dynamic packet)
                {
                    try
                    {
                        string pkt = Convert.ToString(packet) ?? "";
                        if (pkt.Contains("%xt%zm%house%")) toSend = pkt;
                    }
                    catch (Exception ex)
                    {
                        Core.Logger($"[Butler] JoinHouse modifyPacket: {ex.Message}");
                    }
                }

                Bot.Events.ExtensionPacketReceived += modifyPacket;
                Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");
                Bot.Wait.ForMapLoad("house");

                if (Bot.Wait.ForTrue(() => toSend != null, 20))
                    Bot.Send.ClientPacket(toSend!, "json");

                Bot.Events.ExtensionPacketReceived -= modifyPacket;

                for (int i = 0; i < 7; i++) Bot.Send.ClientServer(" ", "");
            }
            else Core.Join("yulgar-100000");
        }
        catch (Exception ex)
        {
            Core.Logger($"[Butler] JoinHouse failed: {ex.Message}");
            Core.Join("yulgar-100000");
        }
    }

    private static string? ReadSyncFileSafe(string path, int retries = 5, int delayMs = 50)
    {
        if (!File.Exists(path)) return null;

        for (int i = 0; i < retries; i++)
        {
            try
            {
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader sr = new(fs);
                string? text = sr.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(text) && text.Split('|').Length >= 10)
                    return text.Trim();
            }
            catch (Exception ex)
            {
                Core.Logger($"[Butler] ReadSyncFileSafe retry {i + 1}/{retries}: {ex.Message}");
            }

            Core.Sleep(delayMs);
        }

        return null;
    }

    private void AsyncGoto(string playerName)
    {
        if (_gotoPending)
            return;

        var now = DateTime.UtcNow;
        if ((now - _lastGotoTime).TotalMilliseconds < GotoMinIntervalMs)
            return;

        _lastGotoTime = now;
        _gotoPending = true;

        string oldCell = Bot.Player.Cell ?? "Enter";
        string oldPad = Bot.Player.Pad ?? "Left";

        _ = Task.Run(() =>
        {
            try
            {
                Bot.Player.Goto(playerName);
            }
            catch { }

            // Break aggro if Goto didn't actually move us
            AntiAutoAggro(oldCell, oldPad);

            _gotoPending = false;
        });
    }

    private struct SyncData
    {
        public string Map;
        public string Room;
        public string Cell;
        public string Pad;
        public bool LeaderLoggedIn;
        public string LeaderServer;
        public long Ticks;
        public bool LeaderInCombat;
        public bool LeaderHasTarget;
        public bool LeaderAlive;
    }

    private void TryAutoEnhance()
    {
        bool autoEnhanceToggle = Bot.Config!.Get<bool>("AutoEnhance");
        Core.CBOBool("DisableAutoEnhance", out bool disableAutoEnhance);
        if (autoEnhanceToggle && !disableAutoEnhance)
        {
            string currentClass = Bot.Player.CurrentClass?.Name ?? "";
            if (!string.IsNullOrEmpty(currentClass))
            {
                Core.Logger($"Enhancing class: {currentClass}");
                Adv.SmartEnhance(currentClass);
            }
        }
    }

    private bool TryReadSyncData(out SyncData data)
    {
        data = default;
        string? content = ReadSyncFileSafe(syncFilePath);
        if (content == null)
        {
            noSyncFileCounter++;
            if (noSyncFileCounter >= noSyncFileThreshold)
            {
                EnterSafeState("Sync file missing, parking");
                noSyncFileCounter = 0;
            }
            Core.Sleep(500);
            return false;
        }
        noSyncFileCounter = 0;

        var parts = content.Split('|');
        data = new SyncData
        {
            Map = parts.ElementAtOrDefault(0)?.ToLower() ?? "unknown",
            Room = parts.ElementAtOrDefault(1) ?? "0",
            Cell = parts.ElementAtOrDefault(2) ?? "Enter",
            Pad = parts.ElementAtOrDefault(3) ?? "Left",
            LeaderLoggedIn = parts.ElementAtOrDefault(4) == "1",
            LeaderServer = parts.ElementAtOrDefault(5) ?? "",
            Ticks = long.TryParse(parts.ElementAtOrDefault(6), out var t) ? t : 0,
            LeaderInCombat = parts.ElementAtOrDefault(7) == "1",
            LeaderHasTarget = parts.ElementAtOrDefault(8) == "1",
            LeaderAlive = parts.ElementAtOrDefault(9) == "1",
        };
        return true;
    }

    private bool IsSameServer(string leaderServer)
    {
        string myServer = Bot.Player.ServerIP ?? "";
        return string.Equals(leaderServer, myServer, StringComparison.OrdinalIgnoreCase);
    }

    private void HandleMapOrCellMismatch(SyncData data)
    {
        QuickDeaggro();

        if (LockedZoneWarning)
        {
            Core.Logger("Locked zone - joining via sync file.");
            Bot.Events.ExtensionPacketReceived -= ChatListener;
            try
            {
                Core.Join("whitemap-100000");

                string? syncContent = ReadSyncFileSafe(syncFilePath);
                if (syncContent != null)
                {
                    var syncParts = syncContent.Split('|');
                    string lockedMap = syncParts.ElementAtOrDefault(0)?.ToLower() ?? "unknown";
                    string lockedRoom = syncParts.ElementAtOrDefault(1) ?? "0";

                    Core.Join($"{lockedMap}-{lockedRoom}");
                    Bot.Wait.ForMapLoad(lockedMap);

                    AsyncGoto(playerName);
                    DontAttack();
                    Core.Sleep(50);
                }

                LockedZoneWarning = false;
            }
            finally
            {
                Bot.Events.ExtensionPacketReceived += ChatListener;
            }
        }
        else
        {
            string? finalCheck = ReadSyncFileSafe(syncFilePath);
            if (finalCheck != null)
            {
                var finalParts = finalCheck.Split('|');
                string finalRoom = finalParts.ElementAtOrDefault(1) ?? "0";

                if (!int.TryParse(finalRoom, out int finalRoomNumber) ||
                    finalRoomNumber == -1 || finalRoomNumber < 10000)
                {
                    EnterSafeState($"Non-private room detected, parking");
                    return;
                }
            }

            if (IsParked)
            {
                Core.Logger("Leader valid again. Rejoining.");
                Core.Join("whitemap-100000");
                Bot.Wait.ForMapLoad("whitemap");
                IsParked = false;
            }

            AsyncGoto(playerName);
            DontAttack();
        }
    }

    private void TryHandleRoomFull()
    {
        string? syncContent = ReadSyncFileSafe(syncFilePath);
        if (syncContent == null)
        {
            RoomFull = false;
            return;
        }

        var parts = syncContent.Split('|');
        string roomMap = parts.ElementAtOrDefault(0)?.ToLower() ?? "";
        string roomNum = parts.ElementAtOrDefault(1) ?? "0";

        Core.Logger($"[Butler] Room full — trying to join {roomMap}-{roomNum} directly.");
        Core.Join($"{roomMap}-{roomNum}");
        Bot.Wait.ForMapLoad(roomMap);

        if (Bot.Map.PlayerExists(playerName))
        {
            Core.Logger("[Butler] Joined leader's room.");
            RoomFull = false;
        }
        else
        {
            Core.Logger("[Butler] Could not join leader's room — retrying later.");
            RoomFull = false;
        }
    }

    private void HandlePadMismatch(SyncData data)
    {
        string myPad = Bot.Player?.Pad ?? "Left";
        if (string.Equals(myPad, data.Pad, StringComparison.OrdinalIgnoreCase))
            return;

        // Same cell, just jump to the correct pad — no Goto needed
        Core.Logger($"[Butler] Jumping to pad: {data.Pad}");
        Bot.Map.Jump(data.Cell, data.Pad);
        DontAttack();
        Core.Sleep(50);
    }

    private void UpdateCombatState(SyncData data)
    {
        bool shouldAttack = data.LeaderAlive && (data.LeaderHasTarget || data.LeaderInCombat);
        if (shouldAttack) Bot.Combat.Attack("*");
        else if (Bot.Player.InCombat || Bot.Player.HasTarget) QuickDeaggro();
    }

    void ChatListener(dynamic packet)
    {
        try
        {
            if (packet == null) return;

            var paramsObj = packet["params"];
            if (paramsObj == null || paramsObj.type != "str") return;

            dynamic? dataObj = paramsObj.dataObj;
            if (dataObj == null) return;

            string? cmd = dataObj[0];
            if (string.IsNullOrEmpty(cmd)) return;

            if (cmd == "server")
            {
                string? text = dataObj[2]?.ToString();
                if (!string.IsNullOrEmpty(text) && text.Contains("ignoring goto")) GotoIsOff = true;
            }
            else if (cmd == "warning")
            {
                string? chat = Convert.ToString(packet);
                if (!string.IsNullOrEmpty(chat))
                {
                    if (chat.Contains("Locked zone") || chat.Contains("not available")) LockedZoneWarning = true;
                    if (chat.Contains("full")) RoomFull = true;
                    if (chat.Contains("ignoring goto")) GotoIsOff = true;
                }
            }
        }
        catch (Exception ex)
        {
            Core.Logger($"[Butler] ChatListener: {ex.Message}");
        }
    }
}
