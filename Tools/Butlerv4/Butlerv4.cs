/*
name: Butlerv4 (TCP)
description: Follows a leader via Goto every ~1s. Connects via TCP for leader location data.
tags: butler, follow, goto, tcp
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/.Debug/Butlerv4FromScratch/DownloadDll.cs

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Butlerv4
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
        new Option<bool>("AutoEnhance", "Auto Enhance", "Automatically enhance equipped class on startup.", true),
        new Option<bool>("UseGoto", "Use Goto", "Use Goto to follow instead of direct Join+Jump.", true),
        CoreBots.Instance.SkipOptions,
    };

    string playerName = string.Empty;
    private volatile bool _gotoPending;
    private DateTime _lastGotoTime = DateTime.MinValue;
    private const int GotoMinIntervalMs = 500;

    // ── TCP state ────────────────────────────────────────────────────
    private TcpClient? _tcp;
    private NetworkStream? _stream;
    private System.IO.StreamReader? _streamReader;
    private string _tcpMap = "";
    private string _tcpRoom = "";
    private string _tcpCell = "";
    private string _tcpPad = "";
    private bool _tcpInCombat = false;
    private bool _tcpHasTarget = false;
    private bool _lockedZone = false;
    private bool _roomFull = false;
    private bool _pvpZone = false;
    private bool _gotoIgnored = false;
    private bool _differentServer = false;
    private bool _isParked = false;
    private bool _houseJoined = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        DownloadDLL.Download();

        Bot.Events.ExtensionPacketReceived += ChatListener;

        string myUsername = Bot.Player.Username ?? "";

        for (int i = 1; i <= 4; i++)
        {
            string butlerList = Bot.Config!.Get<string>($"Leader{i}Butlers") ?? "";
            var butlers = butlerList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (butlers.Any(b => string.Equals(b, myUsername, StringComparison.OrdinalIgnoreCase)))
            {
                playerName = Bot.Config!.Get<string>($"Leader{i}Name") ?? "";
                break;
            }
        }

        if (string.IsNullOrEmpty(playerName))
        {
            Core.Logger($"This account is not assigned to any leader's butler list.", messageBox: true, stopBot: true);
            return;
        }

        ConnectToLeader();

        // Auto-enhance equipped class if enabled
        if (Bot.Config!.Get<bool>("AutoEnhance"))
        {
            string currentClass = Bot.Player.CurrentClass?.Name ?? "";
            if (!string.IsNullOrEmpty(currentClass) && Core.CheckInventory(currentClass))
            {
                Core.Logger($"[Butler] Auto-enhancing class: {currentClass}");
                Adv.SmartEnhance(currentClass);
            }
        }

        DontAttack();
        FollowLeader();

        Bot.Events.ExtensionPacketReceived -= ChatListener;
    }

    private void FollowLeader()
    {
        while (!Bot.ShouldExit)
        {
            while (!Bot.ShouldExit && Bot.Player?.Alive != true)
                Core.Sleep(250);

            PollTcpData();

            if (_isParked)
            {
                _lockedZone = false;
                _roomFull = false;
                _pvpZone = false;
                _gotoIgnored = false;
                _differentServer = false;
                Core.Sleep(5000);
                _isParked = false;
                continue;
            }

            // Leader offline check — if TCP isn't connected, park
            if (_tcp == null || !_tcp.Connected)
            {
                EnterSafeState("Leader is offline or unreachable, parking");
                continue;
            }

            if (_differentServer)
            {
                EnterSafeState("Leader could not be found. Either in a different server or logged off, parking");
                continue;
            }

            if (_lockedZone)
            {
                Core.Logger($"[Butler] Locked zone — tcpMap=[{_tcpMap}] tcpRoom=[{_tcpRoom}]");
                Core.Join($"{_tcpMap}-{_tcpRoom}");
                Core.Jump(_tcpCell, _tcpPad);
                _lockedZone = false;
                continue;
            }

            if (_roomFull)
            {
                EnterSafeState("Room is full, parking");
                continue;
            }

            if (_pvpZone)
            {
                Core.Logger($"[Butler] PvP zone — tcpMap=[{_tcpMap}] tcpRoom=[{_tcpRoom}]");
                Core.Join($"{_tcpMap}-{_tcpRoom}");
                Core.Jump(_tcpCell, _tcpPad);
                _pvpZone = false;
                continue;
            }

            if (_gotoIgnored)
            {
                if (Bot.Config!.Get<bool>("UseGoto"))
                {
                    Core.Logger("Please disable incognito mode in CBO for your leader or turn on its Goto", messageBox: true, stopBot: true);
                    return;
                }
                _gotoIgnored = false;
                continue;
            }

            if (int.TryParse(_tcpRoom, out int roomNum) && roomNum < 1000)
            {
                EnterSafeState($"Leader in room {roomNum}. Please choose a room number higher than 1000, parking");
                continue;
            }

            TryGotoLeader();

            if (_tcpInCombat || _tcpHasTarget)
                Bot.Combat.Attack("*");
            else if ((Bot.Player.InCombat || Bot.Player.HasTarget) && !IsLeaderInSameCell())
                QuickDeaggro();

            Core.Sleep(500);
        }
    }

    private void TryGotoLeader()
    {
        if (_gotoPending)
            return;

        var now = DateTime.UtcNow;
        if ((now - _lastGotoTime).TotalMilliseconds < GotoMinIntervalMs)
            return;

        // If we have TCP data and everything matches, skip the Goto
        if (!string.IsNullOrEmpty(_tcpCell) &&
            string.Equals(_tcpMap, Bot.Map?.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(_tcpCell, Bot.Player?.Cell, StringComparison.OrdinalIgnoreCase))
            return;

        // Otherwise map/cell/pad differs → Goto to leader
        _lastGotoTime = now;
        _gotoPending = true;

        _ = Task.Run(() =>
        {
            try
            {
                if (Bot.Config!.Get<bool>("UseGoto"))
                    Bot.Player?.Goto(playerName);
                else
                {
                    string map = _tcpMap;
                    string room = _tcpRoom;
                    string cell = _tcpCell;
                    string pad = _tcpPad;
                    if (!string.IsNullOrEmpty(map) && !string.IsNullOrEmpty(room))
                    {
                        Core.Join($"{map}-{room}");
                        Core.Jump(cell, pad);
                    }
                }
            }
            catch { }
            _gotoPending = false;
        });
    }

    // ================================================================
    //  TCP
    // ================================================================

    private void ConnectToLeader()
    {
        int port = LeaderButlerSyncv2.LeaderButlerSyncPlugin.ReadLeaderPort(playerName);
        if (port < 0)
            return;

        try
        {
            _tcp = new TcpClient();
            _tcp.Connect("127.0.0.1", port);
            _tcp.NoDelay = true;
            _stream = _tcp.GetStream();
            _streamReader = new System.IO.StreamReader(_stream, Encoding.UTF8);

            string handshake = $"HELLO|{Bot.Player.Username}|{playerName}\n";
            byte[] hb = Encoding.UTF8.GetBytes(handshake);
            _stream.Write(hb, 0, hb.Length);
            _stream.Flush();

            _streamReader.ReadLine(); // welcome
        }
        catch
        {
            Disconnect();
        }
    }

    private void PollTcpData()
    {
        if (_tcp == null || !_tcp.Connected)
        {
            ConnectToLeader();
            return;
        }

        try
        {
            while (_stream!.DataAvailable)
            {
                string? line = _streamReader!.ReadLine();
                if (line == null)
                {
                    Disconnect();
                    return;
                }
                var parts = line.Split('|');
                if (parts.Length >= 9)
                {
                    _tcpMap = parts[0] ?? "";
                    _tcpRoom = parts[1] ?? "";
                    _tcpCell = parts[2] ?? "";
                    _tcpPad = parts[3] ?? "";
                    _tcpInCombat = parts[6] == "1";
                    _tcpHasTarget = parts[7] == "1";
                }
            }
        }
        catch
        {
            Disconnect();
        }
    }

    private void Disconnect()
    {
        _streamReader?.Close();
        _stream?.Close();
        _tcp?.Close();
        _streamReader = null;
        _stream = null;
        _tcp = null;
    }

    // ================================================================
    //  COMBAT
    // ================================================================

    private void DontAttack()
    {
        Bot.Combat.CancelTarget();
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
    }

    private bool IsButlerAlive()
    {
        return Bot.Player?.Alive == true;
    }

    private bool IsLeaderInSameCell()
    {
        return !string.IsNullOrEmpty(_tcpCell) &&
               string.Equals(_tcpMap, Bot.Map?.Name, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(_tcpCell, Bot.Player?.Cell, StringComparison.OrdinalIgnoreCase);
    }

    private void QuickDeaggro()
    {
        if (!IsButlerAlive())
            return;

        if (Bot.Player.InCombat || (Bot.Player.HasTarget && (Bot.Player.Target?.HP ?? 0) > 0))
        {
            DontAttack();
            Bot.Map.Jump(Bot.Player.Cell ?? "Enter", Bot.Player.Pad ?? "Left");
        }
    }

    void EnterSafeState(string reason)
    {
        if (_isParked)
            return;

        Core.Logger(reason);

        if (!_houseJoined)
        {
            _houseJoined = true;
            if (Bot.House.Items.Any(h => h.Equipped))
            {
                Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");
                Bot.Wait.ForMapLoad("house");
            }
            else Core.Join("yulgar-100000");
        }

        _isParked = true;
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
                if (!string.IsNullOrEmpty(text) && text.Contains("ignoring goto"))
                    _gotoIgnored = true;
            }
            else if (cmd == "warning")
            {
                string? chat = Convert.ToString(packet);
                if (!string.IsNullOrEmpty(chat))
                {
                    if (chat.Contains("Locked zone") || chat.Contains("not available"))
                        _lockedZone = true;
                    if (chat.Contains("full"))
                        _roomFull = true;
                    if (chat.Contains("PvP zone"))
                        _pvpZone = true;
                    if (chat.Contains("ignoring goto"))
                        _gotoIgnored = true;
                    if (chat.Contains("could not be found"))
                        _differentServer = true;
                }
            }
        }
        catch { }
    }
}
