/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using System.Diagnostics;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Options;
using Skua.Core.Models.Servers;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CoreArmyLite
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    List<string> cellToAggro = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    #region Army Logging
    private static ArmyLogging armyLogging { get => _armyLogging ??= new ArmyLogging(); set => _armyLogging = value; }
    private static ArmyLogging _armyLogging;

    public void setLogName(string name)
    {
        armyLogging.setLogName(name);
    }

    public void registerMessage(string message, bool delPrevMsg = true)
    {
        armyLogging.registerMessage(message);
        if (delPrevMsg)
        {
            string? player1 = Bot.Config!.Get<string>("player1");
            if (!string.IsNullOrEmpty(player1) && player1.ToLower() == Core.Username().ToLower())
            {
                Core.Logger("Clearing log");
                armyLogging.ClearLogFile();
            }
        }
    }

    public void ClearLogFile()
    {
        string? player1 = Bot.Config!.Get<string>("player1");
        if (!string.IsNullOrEmpty(player1) && player1.ToLower() == Core.Username().ToLower())
        {
            Core.Logger("Clearing log");
            armyLogging.ClearLogFile();
        }
    }

    public bool isEmpty()
    {
        return armyLogging.isEmpty();
    }

    public bool isAlreadyInLog(string[] playersList)
    {
        return armyLogging.isAlreadyInLog(playersList);
    }

    public bool sendDone(int tryCount = 1)
    {
        int attempts = 0;
        while (attempts < tryCount)
        {
            try
            {
                if (!armyLogging.isAlreadyInLog(Players()))
                {
                    armyLogging.WriteLog(
                        $"{Core.Username().ToLower()}:done:{armyLogging.message}"
                    );
                    return true;
                }
                attempts++;
            }
            catch
            {
                attempts++;
            }
            Core.Sleep();
        }
        return false;
    }

    public bool isDone(int tryCount = 1, string[]? players = null)
    {
        int attempts = 0;
        while (attempts < tryCount)
        {
            try
            {
                string[] playerArray = players ?? Players();
                if (armyLogging.isAlreadyInLog(playerArray))
                    return true;
            }
            catch { }
            attempts++;
            Core.Sleep();
        }
        return false;
    }
    #endregion Army Logging

    #region Aggro Mon
#nullable enable

    public int AggroMonPacketDelay { get; set; } = 500;

    /// <summary>
    /// Starts the AggroMon. Jumps to the specified map and starts sending the AggroPacket.
    /// </summary>
    public void AggroIfAnyPlayers()
    {
        if (aggroCTS is not null)
            AggroMonStop();

        string[] players = Players();
        int partySize = players.Length;
        List<int> AggroMonMapIDs = this._AggroMonMIDs;
        foreach (string player in players)
        {
            if (player.ToLower() != Core.Username().ToLower())
            {
                Bot.Map.TryGetPlayer(player, out PlayerInfo? playerObject);
                if (playerObject != null)
                {
                    AddMapIDs(GetMapIDs(Bot.Monsters.GetMonstersByCell(playerObject.Cell)));
                }
            }
        }


        aggroCTS?.Cancel();
        aggroCTS?.Dispose();
        aggroCTS = new();
        Task.Run(async () =>
               {
                   while (!Bot.ShouldExit && !aggroCTS.IsCancellationRequested)
                   {
                       try
                       {
                           if (AggroMonMapIDs.Count > 0)
                               Bot.Send.Packet(AggroMonPacket(AggroMonMapIDs.ToArray()));
                           await Task.Delay(AggroMonPacketDelay);
                       }
                       catch { }
                   }
                   aggroCTS = null;
               });
        List<int> GetMapIDs(List<Monster> monsterData) => monsterData.Select(m => m.MapID).ToList();
        void AddMapIDs(List<int> MMIDs)
        {
            foreach (int ID in MMIDs)
                if (!AggroMonMapIDs.Contains(ID))
                    AggroMonMapIDs.Add(ID);
        }
    }

    public void AggroMonStart(string? map = null, string? _cell = null, string? _pad = null)
    {
        if (aggroCTS is not null)
            AggroMonStop();

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = getRoomNr();

        Bot.Map.Join($"{map}-{Core.PrivateRoomNumber}", "Enter", "Spawn", autoCorrect: false);
        if (map != null)
            Bot.Wait.ForMapLoad(map);

        Bot.Map.Jump(_cell ?? "Enter", _pad ?? "Spawn", autoCorrect: false);
        Bot.Wait.ForCellChange(_cell ?? "Enter");

        // Retrieve player configurations
        for (int i = 1; i <= 10; i++)
            Bot.Config!.Get<string>($"player{i}");

        Core.Logger($"Party Size: {PartySize()}");

        if (map != null)
            WaitForPartyCell(_cell ?? Bot.Player.Cell, _pad ?? Bot.Player.Pad, PartySize());

        // Use HashSet to prevent duplicate MapIDs
        HashSet<int> AggroMonMapIDs = new();

        void PopulateAggroMonMapIDs()
        {
            foreach (string cell in _AggroMonCells)
                foreach (int id in Bot.Monsters.GetMonstersByCell(cell).Select(m => m.MapID))
                    AggroMonMapIDs.Add(id);

            foreach (string name in _AggroMonNames)
                foreach (int id in Bot.Monsters.MapMonsters.Where(m => m.Name == name).Select(m => m.MapID))
                    AggroMonMapIDs.Add(id);

            foreach (int id in _AggroMonIDs)
                foreach (int mapId in Bot.Monsters.MapMonsters.Where(m => m.ID == id || m.MapID == id).Select(m => m.MapID))
                    AggroMonMapIDs.Add(mapId);

            foreach (int mapID in _AggroMonMIDs)
                AggroMonMapIDs.Add(mapID);
        }
        PopulateAggroMonMapIDs();

        // Start aggro task
        aggroCTS?.Cancel();
        aggroCTS?.Dispose();
        aggroCTS = new();

        Task.Run(async () =>
        {
            while (!Bot.ShouldExit && !aggroCTS.IsCancellationRequested)
            {
                try
                {
                    if (Bot.Player.Alive && Bot.Player.Cell == _cell)
                        Bot.Send.Packet(AggroMonPacket(AggroMonMapIDs.ToArray()));
                    await Task.Delay(AggroMonPacketDelay);
                }
                catch { }
            }

            aggroCTS?.Cancel();
            aggroCTS?.Dispose();
            aggroCTS = null;
        });
    }
    private CancellationTokenSource? aggroCTS = null;

    /// <summary>
    /// Stops/Pauses the Aggro Mon Task. Clear will clear the stored settings like AggroMonClear so you can set a new one.
    /// </summary>
    public void AggroMonStop(bool clear = false)
    {
    Retry:
        Bot.Options.AttackWithoutTarget = false;
        aggroCTS?.Cancel();
        aggroCTS?.Dispose();
        aggroCTS = null;
        if (clear)
            AggroMonClear();
        Bot.Wait.ForTrue(() => aggroCTS == null, 30);
        Core.Jump(Bot.Player.Cell, Bot.Player.Pad);
        if (aggroCTS != null)
            goto Retry;
    }

    /// <summary>
    /// Set the AggroMon using Cells. Aggros everything in the Cell.
    /// </summary>
    public void AggroMonCells(params string[] cells)
        => _AggroMonCells = cells.ToList();
    /// <summary>
    /// Set the AggroMon using Monster Names. Aggros everything with the specified name.
    /// </summary>
    public void AggroMonNames(params string[] names)
        => _AggroMonNames = names.ToList();
    /// <summary>
    /// Set the AggroMon using Monster IDs. Aggros everything using the specified ID.
    /// </summary>    
    public void AggroMonIDs(params int[] monsterIDs)
        => _AggroMonIDs = monsterIDs.ToList();
    /// <summary>
    /// Set the AggroMon using Monster Map IDs. Aggros everything using the specified Map ID.
    /// </summary>
    public void AggroMonMIDs(params int[] monsterMapIDs)
        => _AggroMonMIDs = monsterMapIDs.ToList();
    private List<string> _AggroMonCells
    {
        get => __AggroMonCells ??= new List<string>();
        set => __AggroMonCells = value;
    }
    private List<string> __AggroMonCells;

    private List<string> _AggroMonNames
    {
        get => __AggroMonNames ??= new List<string>();
        set => __AggroMonNames = value;
    }
    private List<string> __AggroMonNames;

    private List<int> _AggroMonIDs
    {
        get => __AggroMonIDs ??= new List<int>();
        set => __AggroMonIDs = value;
    }
    private List<int> __AggroMonIDs;

    private List<int> _AggroMonMIDs
    {
        get => __AggroMonMIDs ??= new List<int>();
        set => __AggroMonMIDs = value;
    }
    private List<int> __AggroMonMIDs;


    /// <summary>
    /// Clears the stored Monster Cells/Names/IDs so you can set another AggroMon.
    /// </summary>
    public void AggroMonClear()
    {
        _AggroMonCells.Clear();
        _AggroMonNames.Clear();
        _AggroMonIDs.Clear();
        _AggroMonMIDs.Clear();
        _SmartAggroMonCells.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    public string AggroMonPacket(params int[] MonsterMapIDs)
        => $"%xt%zm%aggroMon%{Bot.Map.RoomID}%{string.Join('%', MonsterMapIDs)}%";

    public void SmartAggroMonStart(string map, params string?[] monsters)
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = getRoomNr();
        Bot.Map.Join($"{map}-{Core.PrivateRoomNumber}", autoCorrect: false);
        Bot.Wait.ForMapLoad(map);

        //Devining variables
        var _monsters = Bot.Monsters.MapMonsters
            .Where(m => !string.IsNullOrEmpty(m.Name) && monsters.Any(name => name == m.Name))
            .ToList(); var cellComparison = new Dictionary<string, int>();

        //Prioritizing monsters of which fewer excist
        foreach (Monster m in _monsters)
            if (!cellComparison.ContainsKey(m.Cell))
                cellComparison.Add(m.Cell, _monsters.Count(t => t.Name == m.Name));
        var SortedDict = cellComparison.OrderBy(kvp => kvp.Value).ToDictionary(pair => pair.Key, pair => pair.Value).Keys.ToArray();
        cellComparison = null;

        //Special option on DivideOnCells, which will have it store all cells that it divides people to
        _getCellsForSmartAggroMon = true;
        DivideOnCells(SortedDict);
        _getCellsForSmartAggroMon = false;

        AggroMonCells(_SmartAggroMonCells.ToArray());
        //leaving this here incase... somethings broke?
        // AggroMonCells(Core.ButlerOnMe() ? new[] { Bot.Player.Cell } : _SmartAggroMonCells.ToArray());
        AggroMonStart(map);
    }
    private bool _getCellsForSmartAggroMon = false;
    private List<string> _SmartAggroMonCells
    {
        get => __SmartAggroMonCells ??= new List<string>();
        set => __SmartAggroMonCells = value;
    }
    private List<string> __SmartAggroMonCells;


    public void RunGeneratedAggroMon(string map, List<string> monNames, List<int> questIDs, ClassType classtype, List<string>? drops = null)
    {
        if (classtype != ClassType.None)
            Core.EquipClass(classtype);

        if (questIDs.Count > 0)
            Core.RegisterQuests(questIDs.ToArray());

        if (drops == null || drops.Count == 0 || drops.All(x => string.IsNullOrEmpty(x)))
            Bot.Drops.Stop();
        else Core.AddDrop(drops.ToArray());

        SmartAggroMonStart(map, monNames.ToArray());

        while (!Bot.ShouldExit)
        {
            monNames.ForEach(monName =>
            {
                Bot.Combat.Attack(monName);
                Core.Sleep(500);
            });
        }
        AggroMonStop(true);

        if (questIDs.Count > 0)
            Core.CancelRegisteredQuests();
    }

    #region Script Options

    public Option<string> player1 = new("player1", "Account #1", "Name of one of your accounts.", "");
    public Option<string> player2 = new("player2", "Account #2", "Name of one of your accounts.", "");
    public Option<string> player3 = new("player3", "Account #3", "Name of one of your accounts.", "");
    public Option<string> player4 = new("player4", "Account #4", "Name of one of your accounts.", "");
    public Option<string> player5 = new("player5", "Account #5", "Name of one of your accounts.", "");
    public Option<string> player6 = new("player6", "Account #6", "Name of one of your accounts.", "");
    public Option<string> player7 = new("player7", "Account #7", "Name of one of your accounts.", "");
    public Option<string> player8 = new("player8", "Account #8", "Name of one of your accounts.", "");
    public Option<string> player9 = new("player9", "Account #9", "Name of one of your accounts.", "");
    public Option<string> player10 = new("player10", "Account #10", "Name of one of your accounts.", "");
    public Option<string> player11 = new("player11", "Account #11", "Name of one of your accounts.", "");
    public Option<string> player12 = new("player12", "Account #12", "Name of one of your accounts.", "");
    public Option<string> player13 = new("player13", "Account #13", "Name of one of your accounts.", "");
    public Option<string> player14 = new("player14", "Account #14", "Name of one of your accounts.", "");
    public Option<string> player15 = new("player15", "Account #15", "Name of one of your accounts.", "");
    public Option<string> player16 = new("player16", "Account #16", "Name of one of your accounts.", "");
    public Option<string> player17 = new("player17", "Account #17", "Name of one of your accounts.", "");
    public Option<string> player18 = new("player18", "Account #18", "Name of one of your accounts.", "");
    public Option<string> player19 = new("player19", "Account #19", "Name of one of your accounts.", "");
    public Option<string> player20 = new("player20", "Account #20", "Name of one of your accounts.", "");


    public Option<int> packetDelay = new(
        "PacketDelay", "Delay for Packet Spam", "Sets the delay for the Packet Spam\n" +
        "Increase if spamming too much - Decrease if missing kills\n" +
        "Recommended setting: 500 or 1000)", 500
    );

    #endregion

    #endregion
    #region Party Management

    public void PartyInvite(string Name)
        => Bot.Send.Packet($"%xt%zm%gp%1%pi%{Name}%");

    private void PartyAccept(int partyID)
        => Bot.Send.Packet($"%xt%zm%gp%1%pa%{partyID}%");

    public void PartyKick(string Name)
        => Bot.Send.Packet($"%xt%zm%gp%1%pk%{Name}%");

    public void PartyLeave()
        => Bot.Send.Packet($"%xt%zm%gp%1%pl%");

    public void PartySummon(string Name)
        => Bot.Send.Packet($"%xt%zm%gp%1%ps%{Name}%");

    public void PartySummonAccept()
        => Bot.Send.Packet("%xt%zm%gp%1%psa%");

    public void PartyPromote(string Name)
        => Bot.Send.Packet($"%xt%zm%gp%1%pp%{Name}%");

    public void PartyOn()
        => Bot.Send.Packet("%xt%zm%cmd%1%partyon%");

    public string[]? PartyMemberArray()
    {
        string[]? members = Bot.Flash.GetGameObject<string[]>("world.partyMembers");
        return members?.Concat(new[] { Core.Username().ToLower() }).ToArray();
    }

    public string? getPartyLeader()
        => Bot.Flash.GetGameObject<string>("world.partyOwner");


    public bool isPartyLeader()
        => Core.Username().ToLower() == (getPartyLeader() ?? string.Empty).ToLower();

    private int getPartyID()
        => Bot.Flash.GetGameObject<int>("world.partyID");

    public void PartyManagement(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type == "json")
        {
            string cmd = data.cmd;
            switch (cmd)
            {
                //When being invited for a party, accept
                case "pi":
                    //string sender = data.owner;
                    int partyID = data.pid;
                    //if (sender.ToLower() == PartyLeader)
                    //{
                    PartyAccept(partyID);
                    Core.Logger($"Joined the party");
                    Core.Sleep();
                    Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
                    //}
                    break;
                //When being summoned by someone, accept
                case "ps":
                    PartySummonAccept();
                    Core.Logger($"Accepted the summon");
                    break;
                //When someone leaves the party (stopped their bot), stop the bot
                case "pr":
                    string prUNM = data.unm;
                    if (!Bot.ShouldExit && stopping)
                    {
                        stopping = true;
                        Core.Logger($"A member has left the party, stopping the bot");
                        Bot.Stop(true);
                    }
                    break;
            }
        }
    }
    private bool stopping = false;

    #endregion
    #region Utility

    private bool monsterAvail(string[] monsterList)
    {
        for (int i = 0; i < monsterList.Length; i++)
        {
            if (IsMonsterAlive(monsterList[i]))
            {
                return true;
            }
        }
        return false;
    }

    public void doPriorityAttack(string[] monsterList)
    {
        for (int i = 0; i < monsterList.Length; i++)
        {
            if (IsMonsterAlive(monsterList[i]))
            {
                if (Int32.TryParse(monsterList[i], out int x))
                {
                    Bot.Combat.Attack(x);
                    return;
                }
            }
        }
    }

    public bool IsMonsterAlive(string monster)
    {
        try
        {
            string? jsonData = Bot.Flash.Call("availableMonsters");
            if (string.IsNullOrEmpty(jsonData))
            {
                // Core.Logger("Failed to retrieve available monsters data.");
                return false;
            }
            JArray monsters = JArray.Parse(jsonData);

            if (monsters.Count == 0)
            {
                return false;
            }

            if (monster == "*")
            {
                foreach (var mon in monsters)
                {
                    var intState = mon["intState"]?.ToString();
                    if (string.IsNullOrEmpty(intState) || intState == "1" || intState == "2")
                    {
                        return true;
                    }
                }
                return false;
            }

            bool isByMapID = monster.StartsWith("id-");
            string identifier = isByMapID ? monster.Substring(3) : monster;

            foreach (var mon in monsters)
            {
                bool match = isByMapID
                    ? mon["MonMapID"]?.ToString() == identifier
                    : mon["strMonName"]?.ToString() == identifier;

                if (match)
                {
                    var intState = mon["intState"]?.ToString();
                    if (string.IsNullOrEmpty(intState) || intState == "1" || intState == "2")
                    {
                        return true;
                    }
                    else if (intState == "0")
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch { return true; }

    }

    public void StartFarm(string item, int quant)
    {
        bool needSendDone = true;
        int countCheck = 0;
        while (!Bot.ShouldExit)
        {
            if (Core.CheckInventory(item, quant) && needSendDone)
            {
                if (sendDone())
                    needSendDone = false;
            }
            if (!needSendDone && isDone() && countCheck == 10)
            {
                break;
            }
            countCheck++;
            if (countCheck > 10)
                countCheck = 0;

            // killing monster
            if (IsMonsterAlive("*"))
            {
                Bot.Combat.Attack("*");
            }

            Bot.Sleep(100);
        }
    }

    public void StartFarmRep(string faction, int rank)
    {
        bool needSendDone = true;
        int countCheck = 0;
        while (!Bot.ShouldExit)
        {
            if (Farm.FactionRank(faction) >= rank && needSendDone)
            {
                if (sendDone())
                    needSendDone = false;
            }
            if (!needSendDone && isDone() && countCheck == 10)
            {
                break;
            }
            countCheck++;
            if (countCheck > 10)
                countCheck = 0;

            if (IsMonsterAlive("*"))
            {
                Bot.Combat.Attack("*");
            }

            Bot.Sleep(100);
        }
    }

    public void StartFarmGold(int quant)
    {
        bool needSendDone = true;
        int countCheck = 0;
        while (!Bot.ShouldExit)
        {
            if (Bot.Player.Gold >= quant && needSendDone)
            {
                if (sendDone())
                    needSendDone = false;
            }
            if (!needSendDone && isDone() && countCheck == 10)
            {
                break;
            }
            countCheck++;
            if (countCheck > 10)
                countCheck = 0;

            if (IsMonsterAlive("*"))
            {
                Bot.Combat.Attack("*");
            }

            Bot.Sleep(100);
        }
    }

    /// <summary>
    /// Generates a unique 5-digit room number based on the machine name,
    /// username, and the current date and time (with a 5-hour offset). The 
    /// output is deterministic for a given machine and user, ensuring no 
    /// leading zeros in the returned integer.
    /// </summary>
    /// <returns>A unique 5-digit integer room number that does not start 
    /// with a zero.</returns>
    public int getRoomNr()
    {
        // Combine machine name, username, and fixed date for uniqueness
        string uniqueIdentifier = $"{Environment.MachineName}_{Environment.UserName}_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}";

        // Hash the unique identifier
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueIdentifier));

        // Generate combinedDigits from the hash while incorporating randomness
        StringBuilder combinedDigits = new();
        for (int i = 0; i < hash.Length; i++)
        {
            // Mix in the index to add variability
            int digit = (hash[i] + i) % 10; // Ensures it's always a digit (0-9)
            combinedDigits.Append(digit);
        }

        // Ensure we have at least 5 digits
        if (combinedDigits.Length < 5)
        {
            combinedDigits.Append('0', 5 - combinedDigits.Length); // Pad with zeros if needed
        }

        // Take the first 5 characters
        string roomNumberStr = combinedDigits.ToString().Substring(0, 5);

        // Ensure the first digit isn't '0'
        if (roomNumberStr[0] == '0')
        {
            roomNumberStr = string.Concat("1", roomNumberStr.AsSpan(1)); // Replace with '1'
        }

        // Return the integer
        return int.Parse(roomNumberStr);
    }

    /// <summary>
    /// Spreads players around the input cells, if no cells are set - will spread to any cell that has a monster in it. 
    /// If player count is more than cell count, will add players to the cells listed in order. Example: c1: P1 + P4, c2: P2, c3: P3
    /// </summary>
    public void DivideOnCells(params string[] cells)
    {
        // Parsing all the player names from an unspecified amount of player name options
        string[] _players = Players();

        // If no paramaters are given, select all cells that have monsters in them
        if (cells == null || cells.Length == 0)
        {
            List<Monster> monsters = Bot.Monsters.MapMonsters;
            if (monsters == null || monsters.Count == 0)
                return;

            List<string> _cells = new();
            foreach (string cell in monsters.Select(m => m.Cell))
                if (!_cells.Contains(cell))
                    _cells.Add(cell);
            cells = _cells.OrderBy(x => x).ToArray();
        }

        //Dividing the players amongst the cells
        int cellCount = 0;
        string username = Core.Username().ToLower();
        foreach (string p in _players)
        {
            string cell = cells[cellCount];
            if (_getCellsForSmartAggroMon && !_SmartAggroMonCells.Contains(cell))
                _SmartAggroMonCells.Add(cell);

            if (username == p)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
                Bot.Player.SetSpawnPoint();
            }
            cellCount = cellCount == cells.Length - 1 ? 0 : cellCount + 1;
        }
    }

    public void DivideOnCellsPriority(string[] cells, string priorityCell, bool setAggro = false, bool log = false, bool equipClass = false)
    {
        // Parsing all the player names from an unspecified amount of player name options
        string[] _players = Players();
        if (log) Core.Logger($"divide on cells: {string.Join(",", cells)}. priority cell: {priorityCell}");

        if (setAggro)
        {
            cellToAggro.Clear();
            int playerCount = _players.Length;
            int _cellCount = cells.Length;
            int aggroCell = playerCount > _cellCount ? _cellCount : playerCount;
            for (int i = 0; i < aggroCell; i++)
            {
                cellToAggro.Add(cells[i]);
            }
            AggroMonCells(cellToAggro.ToArray());
        }

        if (string.IsNullOrEmpty(priorityCell))
        {
            int cellCount = 0;
            string username = Core.Username().ToLower();
            foreach (string p in _players)
            {
                string cell = cells[cellCount];
                if (username == p)
                {
                    Core.Logger($"i am on cell: {cell}");
                    Core.Jump(cell, "Left");
                }
                cellCount = cellCount == cells.Length - 1 ? 0 : cellCount + 1;
            }
        }
        else
        {
            int playersCount = _players.Length;
            string username = Core.Username().ToLower(); // Username of the player running this method
            for (int i = 0; i < playersCount; i++) // Iterate through each player
            {
                string p = _players[i];
                string cell = (cells.Length > 0 && i < cells.Length) ? cells[i] : priorityCell; // Get the cell for the current player

                if (username == p)
                {
                    Core.Logger($"i am on cell: {cell}");
                    if (equipClass)
                    {
                        if (cell.Equals(priorityCell))
                        {
                            Core.EquipClass(ClassType.Solo);
                        }
                        else
                        {
                            Core.EquipClass(ClassType.Farm);
                        }
                    }
                    Core.Jump(cell, "Left");
                }
            }
        }
    }

    public void waitForSignal(string message, string[]? players = null, bool delPrevMsg = false)
    {
        registerMessage(message, delPrevMsg);
        while (!Bot.ShouldExit)
        {
            sendDone(10);
            if (isAlreadyInLog(new string[] { Core.Username().ToLower() }))
                break;
            Bot.Sleep(500);
        }
        while (!Bot.ShouldExit)
        {
            if (players == null ? isDone(10) : isDone(10, players)) break;
            Bot.Sleep(500);
        }
    }

    /// <summary>
    /// Waits for the party members to join the specified cell in the game.
    /// If no cell is specified, it checks the current cell for the required 
    /// player count. Logs the final list of players and monitors until all 
    /// expected players are present, a timeout occurs, or the bot exits.
    /// </summary>
    /// <param name="cell">The cell to jump to. If null, the current cell is used.</param>
    /// <param name="pad">The direction to pad when jumping; defaults to "Left".</param>
    /// <param name="playerCount">The expected number of players; defaults to the party size.</param>
    /// <param name="timeoutSeconds">Optional timeout in seconds; defaults to 60s.</param>
    public void WaitForPartyCell(string? cell = null, string? pad = null, int? playerCount = null, int timeoutSeconds = 60)
    {
        // Jump to the specified cell if provided
        if (!string.IsNullOrEmpty(cell))
        {
            Bot.Map.Jump(cell, pad ?? "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        // Normalize expected player names
        var expectedPlayers = Players()
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim().ToLower())
            .ToList();

        int requiredCount = playerCount ?? PartySize();

        Core.Logger($"Waiting for players: {string.Join(", ", expectedPlayers)}");

        DateTime startTime = DateTime.UtcNow;

        while (!Bot.ShouldExit && Bot.Map.PlayerNames != null)
        {
            // Timeout check
            if ((DateTime.UtcNow - startTime).TotalSeconds > timeoutSeconds)
            {
                Core.Logger("Timeout reached. Lobby may be bugged or some players are missing.");
                break;
            }

            // Normalize current map player names
            var currentPlayers = Bot.Map.PlayerNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLower())
                .ToHashSet();

            // Determine missing players
            var missing = expectedPlayers
                .Where(p => !currentPlayers.Contains(p))
                .ToList();

            if (!missing.Any() && currentPlayers.Count >= requiredCount)
            {
                Core.Logger("All players found!");
                break;
            }

            if (missing.Any())
                Core.Logger($"Missing: {string.Join(", ", missing)}");

            Core.Sleep(1000);
        }
    }


    public string[] Players()
    {
        List<string> players = new();
        int index = 1;

        while (!Bot.ShouldExit && index <= 20) // Stop when index exceeds 20
        {
            if (Bot.Config == null)
            {
                Core.Logger("Configuration is null.");
                break;
            }

            // Retrieve player names from the configuration
            string? playerName = Bot.Config.Get<string>($"player{index}");

            // If the player name is null or empty, continue to the next index
            if (string.IsNullOrEmpty(playerName))
            {
                index++; // Skip to the next player index
                continue; // Continue to the next loop iteration
            }

            // Add the player's name to the list and log it
            players.Add(playerName.ToLower().Trim());

            index++; // Increment the index after processing
        }

        return players.ToArray();
    }
    public int PartySize() => Players() == null ? 0 : Players().Where(x => !string.IsNullOrEmpty(x)).Count();

    // public void waitForParty(string map, string? item = null, int playerMax = -1)
    // {
    //     Bot.Events.PlayerAFK += PlayerAFK;
    //     string[] players = Players();
    //     int partySize = players.Length;
    //     List<string> playersWhoHaveBeenHere = new() { Core.Username() };
    //     int playerCount = 1;

    //     int logCount = 0;
    //     int butlerTimer = 0;
    //     bool hasWaited = false;

    //     Core.Join(map);
    //     int dynamicPartySize = playerMax == -1 ? partySize : playerMax;

    //     while (playerCount < dynamicPartySize)
    //     {
    //         if (Bot.Map.PlayerNames != null)
    //             foreach (var name in Bot.Map.PlayerNames)
    //                 if (!playersWhoHaveBeenHere.Contains(name) && players.Select(x => x.ToLower().Trim()).Contains(name.ToLower()))
    //                     playersWhoHaveBeenHere.Add(name);
    //         playerCount = playersWhoHaveBeenHere.Count;

    //         logCount++;
    //         if (logCount == 15)
    //         {
    //             Core.Logger($"Waiting for the party{(item == null ? string.Empty : (" to farm " + item))} [{playerCount}/{dynamicPartySize}]");
    //             hasWaited = true;
    //             logCount = 0;
    //         }
    //         Core.Sleep(1000);

    //         if (playersWhoHaveBeenHere.Count == (dynamicPartySize - 1))
    //             butlerTimer++;
    //         if (butlerTimer >= 30)
    //         {
    //             b_breakOnMap = Bot.Map.Name;
    //             string toFollow = players.First(p => !playersWhoHaveBeenHere.Any(n => n.ToLower() == p.ToLower().Trim()));
    //             Core.Logger($"Missing {toFollow}, initiating Butler.cs");
    //             Core.Logger("Butler active until in map /" + b_breakOnMap);
    //             Butler(toFollow, roomNr: getRoomNr());
    //             Core.Logger($"{toFollow} has joined {b_breakOnMap}. continuing");
    //             Bot.Events.PlayerAFK -= PlayerAFK;
    //             break;
    //         }
    //     }
    //     if (hasWaited)
    //         Core.Logger($"Party complete [{partySize}/{partySize}]");
    //     Core.Sleep(3500); //To make sure everyone attack at the same time, to avoid deaths

    //     void PlayerAFK()
    //     {
    //         Core.Logger("Anti-AFK engaged");
    //         Core.Sleep(1500);
    //         Bot.Send.Packet("%xt%zm%afk%1%false%");
    //     }
    // }

    /// <summary>
    /// Waits until all expected party members are present in the specified map.
    /// Tracks players, logs progress, and uses Butler.cs to fetch missing players
    /// if they fail to join within the given wait time. Also engages anti-AFK.
    /// </summary>
    /// <param name="map">The map name to join and wait in.</param>
    /// <param name="item">Optional: Item name for contextual logging (e.g., farming target).</param>
    /// <param name="playerMax">
    /// The expected number of players in the party. 
    /// If -1 (default), uses <see cref="PartySize"/>.
    /// </param>
    public void WaitForParty(string map, string? item = null, int playerMax = -1)
    {
        Bot.Events.PlayerAFK += PlayerAFK;

        string[] allPlayers = Players();
        int targetPartySize = playerMax > 0 ? playerMax : PartySize();

        HashSet<string> playersHere = new(StringComparer.OrdinalIgnoreCase)
    {
        Core.Username()
    };

        Core.Join(map, "Enter", "Spawn");
        Bot.Wait.ForMapLoad(map);

        int logCount = 0;
        int butlerTimer = 0;
        bool hasWaited = false;

        while (playersHere.Count < targetPartySize && !Bot.ShouldExit)
        {
            TrackPlayers(allPlayers, playersHere);

            if (++logCount >= 15)
            {
                Core.Logger($"Waiting for the party{(item == null ? string.Empty : $" to farm {item}")} [{playersHere.Count}/{targetPartySize}]");
                hasWaited = true;
                logCount = 0;
            }

            Core.Sleep(1000);

            if (playersHere.Count < targetPartySize && ++butlerTimer >= 30)
            {
                var missingPlayers = allPlayers.Where(p => !playersHere.Contains(p));
                foreach (string missing in missingPlayers)
                    HandleMissingPlayer(missing, playersHere, map);

                // Reset timers
                butlerTimer = 0;
                logCount = 0;
            }
        }

        if (hasWaited)
            Core.Logger($"Party complete [{targetPartySize}/{targetPartySize}]");

        Core.Sleep(3500);

        // --- Helpers ---
        void TrackPlayers(string[] expected, HashSet<string> seen)
        {
            if (Bot.Map.PlayerNames == null)
                return;

            foreach (string name in Bot.Map.PlayerNames)
            {
                if (expected.Contains(name, StringComparer.OrdinalIgnoreCase))
                    seen.Add(name);
            }
        }

        void HandleMissingPlayer(string missing, HashSet<string> seen, string currentMap)
        {
            b_breakOnMap = Bot.Map.Name;

            Core.Logger($"Missing {missing}, initiating Butler.cs");
            Core.Logger($"Butler active until in map /{currentMap}");

            Butler(missing, roomNr: getRoomNr());

            Core.Logger($"{missing} has rejoined {b_breakOnMap}. Continuing...");
            seen.Add(missing);
        }

        void PlayerAFK()
        {
            Core.Logger("Anti-AFK engaged");
            Core.Sleep(1500);
            Bot.Send.Packet("%xt%zm%afk%1%false%");
        }
    }


    public bool SellToSync(string? item, int quant)
    {
        if (Core.CheckInventory(item, quant) || item == null)
            return true;
        if (SellToSyncOn)
            Core.SellItem(item, all: true);
        return false;
    }
    public bool SellToSyncOn = false;
    #endregion
    #region OneClient

    public bool doForAll(bool randomServers = false)
    {
        if (Bot.ShouldExit || _doForAllIndex >= (doForAllAccountDetails ??= readManager()).Length)
            return false;

        Bot.Options.AutoRelogin = false;

        string name = doForAllAccountDetails[_doForAllIndex].Item1;
        string pass = doForAllAccountDetails[_doForAllIndex++].Item2;

        Server[] ServerList = Bot.Servers.CachedServers
            .Where(x => !BlacklistedServers.Contains(x.Name.ToLower()) && (Core.IsMember || !x.Upgrade) && x.Online)
            .ToArray();

        if (Core.Username() != name)
        {
            if (Bot.Player.LoggedIn)
            {
                Bot.Servers.Logout();
                while (Bot.Player.LoggedIn)
                    Core.Sleep();
            }

            Bot.Servers.Login(name, pass);
            Core.Sleep(3000);

            var availableServers = randomServers
        ? Bot.Servers.ServerList
            .Where(x => !BlacklistedServers.Contains(x.Name.ToLower()) && !x.Upgrade && x.Online)
            .ToList() // Convert to List<Server> to match CachedServers
        : Bot.Servers.CachedServers;


            if (availableServers.Count > 0)
            {
                var targetServer = randomServers
                    ? availableServers[Bot.Random.Next(0, Math.Min(availableServers.Count, 5))]
                    : availableServers.First(x => x.Name == Bot.Options.ReloginServer);

                Bot.Servers.Connect(targetServer);
            }
        }

        Bot.Wait.ForMapLoad("battleon");
        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 100);

        while (!Bot.ShouldExit && !Bot.Player.Loaded)
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);

        Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");
        Bot.Wait.ForMapLoad("house");
        Core.Sleep();

        if (Bot.Flash.GetGameObject("ui.mcPopup.currentLabel") != "\"Bank\"")
            Bot.Bank.Open();

        Bot.Bank.Load();
        Core.ReadCBO();
        Core.IsMember = Bot.Player.IsMember;


        return true;

        // Improved readManager method
        (string, string)[] readManager()
        {
            string dirPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Skua.Manager");

            if (!Directory.Exists(dirPath))
            {
                Core.Logger($"No folders found at {dirPath}. Add accounts using The `Skua Manager` app, Opening now." +
                "(if the manager does not appear, in the bottom right of your screen click the `^`, and find the `Skua Manager` icon," +
                "Right click the icon, and click \"Show Manager\")", "AccountManager", true, true);
                Process.Start(Path.Combine(AppContext.BaseDirectory, "Skua.Manager.exe"));
                return Array.Empty<(string, string)>();
            }

            string[] dirs = Directory.GetDirectories(dirPath, Bot.Version.ToString(), SearchOption.AllDirectories);

            if (dirs.Length == 0)
            {
                dirs = Directory.GetDirectories(dirPath, "1.2.3.0", SearchOption.AllDirectories);
                if (dirs.Length == 0)
                    dirs = Directory.GetDirectories(dirPath, "1.2.2.1", SearchOption.AllDirectories);
            }

            if (dirs.Length != 1)
            {
                if (dirs.Length == 0)
                {
                    Core.Logger("Found no Folders for `Skua.Manager`, add accounts using The `Skua Manager` app. (Opening now)", "AccountManager", true, true);
                    Process.Start(Path.Combine(AppContext.BaseDirectory, "Skua.Manager.exe"));
                }
                if (dirs.Length > 1)
                {
                    Core.Logger("Found multiple Folders for `Skua.Manager`," +
                    "Please delete the ones you don't want to use (if you're not sure, find the folder with the 1.2.4 version inside, you can remove the rest.)" +
                    "Opening Appdata folder now.", "AccountManager", true, true);
                    Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Skua.Manager"));
                }
                return Array.Empty<(string, string)>();
            }

            var xml = new XmlDocument();
            string configPath = Path.Combine(dirs[0], "user.config");

            try
            {
                xml.Load(configPath);
            }
            catch (Exception ex)
            {
                Core.Logger($"Failed to load user.config: {ex.Message}", "AccountManager", true, true);
                return Array.Empty<(string, string)>();
            }

            List<(string, string)> toReturn = new();
            try
            {
                dynamic[] dyn = JsonConvert.DeserializeObject<dynamic[]>(
                    JsonConvert.DeserializeObject<dynamic>(
                        JsonConvert.SerializeXmlNode(xml))!
                    .configuration
                    .userSettings
                    ["Skua.Manager.Properties.Settings"]
                    .setting.ToString()
                );

                foreach (var d in dyn)
                {
                    if (d["@name"] == "ManagedAccounts" && d["value"].ArrayOfString["string"] != null)
                    {
                        string[] accs = JsonConvert.DeserializeObject<string[]>(d["value"].ArrayOfString["string"].ToString());
                        foreach (string acc in accs)
                        {
                            string[] info = acc.Split("{=}");
                            if (info.Length == 3)
                                toReturn.Add((info[1], info[2]));
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Logger($"Failed to parse accounts: {ex.Message}", "AccountManager", true, true);
            }

            if (toReturn.Count <= 0)
            {
                Core.Logger("No accounts found. Add accounts using The `Skua Manager` app, Opening now." +
                "(if the manager does not appear, in the bottom right of your screen click the `^`, and find the `Skua Manager` icon," +
                "Right click the icon, and click \"Show Manager\")", "AccountManager", true);
                Process.Start(Path.Combine(AppContext.BaseDirectory, "Skua.Manager.exe"));
                Bot.Stop(true);
            }

            return toReturn.ToArray();
        }
    }

    private int _doForAllIndex = 0;
    public (string, string)[]? doForAllAccountDetails;
    private readonly string[] BlacklistedServers =
    {
        "artix",
        "sir ver",
        "yorumi",
        "gravelyn",
        "galanoth",
        "class test realm",
        $"{null}"
    };
    #endregion

    #region Butler
    public void Butler(string playerName, bool LockedMaps = true, string? LockedMapsList = null, ClassType classType = ClassType.Farm, bool CopyWalk = false, int roomNr = 1, bool rejectDrops = true, string? attackPriority = null, int hibernateTimer = 0)
    {
        Bot.Events.PlayerAFK += PlayerAFK;

        #region Initialization and Setup
        if (string.IsNullOrEmpty(playerName) || playerName == "Insert Name")
        {
            Core.Logger("No name was inserted, stopping the bot.", messageBox: true, stopBot: true);
        }

        if (roomNr <= 0)
        {
            roomNr = Core.PrivateRoomNumber;
        }

        playerName = playerName.Trim().ToLower();
        b_playerName = playerName;

        // Setup parameters
        b_doLockedMaps = LockedMaps;
        b_doCopyWalk = CopyWalk;
        b_hibernationTimer = hibernateTimer;
        b_shouldHibernate = b_hibernationTimer > 0;

        if (!string.IsNullOrEmpty(attackPriority))
            _attackPriority.AddRange(attackPriority.Split(',', StringSplitOptions.TrimEntries));

        if (!string.IsNullOrEmpty(LockedMapsList))
            _LockedMapsList.AddRange(LockedMapsList.Split(',', StringSplitOptions.TrimEntries));

        // Create directory for communication with player
        if (!Directory.Exists(CoreBots.ButlerLogDir))
            Directory.CreateDirectory(CoreBots.ButlerLogDir);
        File.Create(commFile());

        // Setting up private room if needed
        if (roomNr != 999999 && roomNr >= 1000)
        {
            Core.PrivateRooms = true;
            Core.PrivateRoomNumber = roomNr;
        }

        int[] bypasses = {
        598, 3004, 3008, 3484, 3799, 4616, 8107, 9126, 5915, 9814, 7522
    };
        Bot.Quests.Load(bypasses);
        foreach (int questId in bypasses)
            Bot.Quests.UpdateQuest(questId);
        Core.SetAchievement(18); // doomvaultb
        if (Bot.Player.Level < 100)
            Bot.Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"levelUp\",\"intExpToLevel\":\"0\",\"intLevel\":100}}}", type: "json"); // level bypass
        #endregion

        int retryLimit = 5; // Set retry limit
        int retryCount = 0; // Track number of retries
        PlayerInfo? playerObject = new(); // Initialize player object

        while (!Bot.ShouldExit)
        {
            #region Ignore me
            // Try to /goto the player with retry logic
            while (!tryGoto(playerName, out playerObject, roomNr) && playerObject != null && retryCount < retryLimit)
            {
                // Increment retry count and log the attempt
                retryCount++;
                Core.Logger($"Attempt {retryCount} to find {playerName} failed. Retrying...");

                // Handle combat disengagement and fallback
                // ExitCombat();

                // Join fallback map if /goto fails
                if (Bot.House.Items.Any(h => h.Equipped))
                {
                    string? toSend = null;
                    Bot.Events.ExtensionPacketReceived += modifyPacket;
                    Bot.Send.Packet($"%xt%zm%house%1%{Core.Username()}%");
                    Bot.Wait.ForMapLoad("house");
                    Task.Run(() =>
                    {
                        Bot.Wait.ForMapLoad("house");
                        if (Bot.Wait.ForTrue(() => toSend != null, 20))
                            Bot.Send.ClientPacket(toSend!, "json");
                        Bot.Events.ExtensionPacketReceived -= modifyPacket;
                        for (int i = 0; i < 7; i++)
                            Bot.Send.ClientServer(" ", "");
                    });

                    void modifyPacket(dynamic packet)
                    {
                        string type = packet["params"].type;
                        dynamic data = packet["params"].dataObj;
                        if ((type is not null and "json") && (data.houseData is not null))
                        {
                            toSend = $"{{\"t\":\"xt\",\"b\":{{\"r\":-1,\"o\":{{\"cmd\":\"moveToArea\",\"areaName\":\"house\",\"uoBranch\":{JsonConvert.SerializeObject(data.uoBranch)},\"strMapFileName\":\"{data.strMapFileName}\",\"intType\":\"1\",\"monBranch\":[],\"houseData\":{Regex.Replace(JsonConvert.SerializeObject(data.houseData), Core.Username(), "Skua user", RegexOptions.IgnoreCase)},\"sExtra\":\"\",\"areaId\":{data.areaId},\"strMapName\":\"house\"}}}}}}";
                            Bot.Events.ExtensionPacketReceived -= modifyPacket;
                        }
                    }
                }
                else Bot.Send.Packet($"%xt%zm%cmd%1%tfer%{Core.Username()}%whitemap-{Core.PrivateRoomNumber}%");

                Core.Logger($"Could not find {playerName}. Ensure they are on the same server.", "tryGoto");

                if (b_shouldHibernate)
                    Core.Logger($"Bot will hibernate and retry every {hibernateTimer} seconds.", "tryGoto");

                #region Hybernation
                int elapsedMinutes = 0;

                // Enter hibernation retry loop
                while (!Bot.ShouldExit)
                {
                    if (b_shouldHibernate)
                    {
                        // Sleep for the hibernate period
                        for (int t = 0; t < hibernateTimer; t++)
                        {
                            Core.Sleep(1000);
                            if (Bot.ShouldExit)
                                break;
                        }
                    }

                    // Retry /goto after hibernation
                    if (tryGoto(playerName, out playerObject, roomNr) && playerObject != null)
                    {
                        break;
                    }

                    // Increment elapsed minutes and log every 5 minutes
                    elapsedMinutes += hibernateTimer;
                    if (elapsedMinutes >= 300)
                    {
                        Core.Logger($"Bot has been hibernating for {elapsedMinutes / 60} minutes.");
                        elapsedMinutes = 0;
                    }
                }

                // After several failed retries, consider adding a dynamic fallback strategy
                if (retryCount >= retryLimit)
                {
                    Core.Logger($"Unable to locate {playerName} after {retryLimit} attempts, performing alternative action.");
                    // e.g., join a specific map or wait before retrying
                    Core.Join("whitemap");
                }
                if (tryGoto(playerName, out playerObject, roomNr) && playerObject != null)
                {
                    Core.Logger($"{playerName} found!");
                    break;
                }
                #endregion Hybernation
            }

            if (retryCount >= retryLimit)
            {
                break; // Exit Butler method after retry limit reached
            }

            // Check for break on specific map
            if (b_breakOnMap != null && b_breakOnMap == Bot.Map.Name)
            {
                b_breakOnMap = null;
                break;
            }
            #endregion Ignore me

            #region Combat Area
            // Check if there are monsters in the same cell
            while (!Bot.ShouldExit)
            {
                // Exit early if {playerName} is not in the map
                if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Count > 0 && !Bot.Map.PlayerNames.Contains(playerName))
                {
                    Core.Logger($"{playerName} is not in the map");
                    break;
                }

                #region ignore this
                // Handle combat based on attack priority or attack all
                while (!Bot.ShouldExit && !Bot.Player.Alive) { Core.Sleep(); }

                if (!string.IsNullOrEmpty(attackPriority))
                {
                    List<string> Mons = new();
                    if (!string.IsNullOrEmpty(attackPriority))
                    {
                        var attackPriorityItems = attackPriority
                            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        Mons.AddRange(attackPriorityItems);
                    }
                    if (!Bot.Combat.StopAttacking)
                        PriorityAttack(Mons);
                }
                #endregion ignore this
                else
                {
                    if (!Bot.Combat.StopAttacking)
                    {
                        Bot.Combat.Attack("*"); // Attack any monster if no priority exists
                        Core.Sleep(); // Pause to avoid busy waiting
                    }
                }

                // Ensure player is in the same cell
                if (Bot.Map.TryGetPlayer(playerName, out playerObject) && playerObject != null && playerObject?.Cell != Bot.Player.Cell)
                {
                    Bot.Map.Jump(playerObject?.Cell ?? Bot.Player.Cell, playerObject?.Pad ?? Bot.Player.Pad, false);
                    Bot.Wait.ForCellChange(playerObject?.Cell ?? Bot.Player.Cell);
                    Core.Sleep();
                    Bot.Player.SetSpawnPoint();
                }

            }
            #endregion Combat Area
            //itll get here if the player isnt in the map
        }

        ButlerStop();
    }


    public void PlayerAFK()
    {
        Core.Logger("Anti-AFK engaged");
        Core.Sleep(1500);
        Bot.Send.Packet("%xt%zm%afk%1%false%");
    }
    public string? b_playerName = null;
    public bool b_doLockedMaps = true;
    public bool b_doCopyWalk = false;
    public int b_hibernationTimer = 0;
    public bool b_shouldHibernate = true;
    public List<string> _attackPriority
    {
        get => __attackPriority ??= new List<string>();
        set => __attackPriority = value;
    }
    public List<string> __attackPriority;

    public List<string> _LockedMapsList
    {
        get => __LockedMapsList ??= new List<string>();
        set => __LockedMapsList = value;
    }
    public List<string> __LockedMapsList;


    public bool tryGoto(string userName, out PlayerInfo? playerObject, int roomNumber)
    {
        playerObject = new();

        if (string.IsNullOrWhiteSpace(userName))
        {
            Core.Logger("Invalid username.");
            return false;
        }

        int retry = 0;

        // Handle locked zones by subscribing to the event if necessary
        if (b_doLockedMaps)
            Bot.Events.ExtensionPacketReceived += LockedZoneListener;

        while (!Bot.ShouldExit)
        {
            if (Bot.Map.TryGetPlayer(userName, out playerObject) && playerObject != null && playerObject.Cell == Bot.Player.Cell)
            {
                return true;
            }
            Core.JumpWait();
            Core.Sleep();
            Bot.Player.Goto(userName);
            Core.Sleep();
            playerObject = Bot.Map.TryGetPlayer(userName, out playerObject) && playerObject != null ? playerObject : null;
            if (Bot.Map.TryGetPlayer(userName, out playerObject) && playerObject != null)
            {
                if (playerObject != null && playerObject?.Cell == Bot.Player.Cell)
                {
                    return true;
                }

                string targetCell = playerObject?.Cell ?? Bot.Player.Cell;
                string targetPad = playerObject?.Pad ?? Bot.Player.Pad;

                Bot.Map.Jump(targetCell, targetPad, false);
                Bot.Wait.ForCellChange(targetCell);
                Core.Sleep();
                playerObject = Bot.Map.TryGetPlayer(userName, out playerObject) && playerObject != null ? playerObject : null;

                if (playerObject != null && playerObject?.Cell == Bot.Player.Cell)
                {
                    return true;
                }
            }


            if (playerObject != null && playerObject.Cell == Bot.Player.Cell)
            {
                return true;
            }
            // Check for locked zone warning and handle it accordingly
            if (LockedZoneWarning)
            {
                break;
            }

            if (++retry >= 5)
                break;
            Core.Sleep(1000);

        }

        // Handle locked zone scenario
        if (b_doLockedMaps && LockedZoneWarning && !insideLockedMaps)
        {
            LockedZoneWarning = false;
            LockedMaps(userName, false, roomNumber); // Handle the locked zone logic
            Core.ToggleAggro(true);
            Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
            return true;
        }

        // Unsubscribe from event once done
        Bot.Events.ExtensionPacketReceived -= LockedZoneListener;

        return false;
    }

    private void ExitCombat()
    {
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        string? targetCell = Bot.Map.Cells
            .Where(c => !Core.BlackListedJumptoCells.Contains(c) &&
                        !Bot.Monsters.MapMonsters.Any(m => m != null && m.Cell == c))
            .FirstOrDefault(c => Bot.Map.Cells.Count(cell => cell.Contains("Enter")) > 1 || !c.Contains("Enter"))
            ?? "Enter";

        Bot.Map.Jump(targetCell, targetCell == "Enter" ? "Spawn" : "Left");
        Bot.Wait.ForCellChange(targetCell);
        Core.Sleep();
        Core.JumpWait();
    }
    public bool LockedZoneWarning = false;
    public bool MapUnavailable = false;
    public bool insideLockedMaps = false;

    public void LockedZoneListener(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;

        if (type is not null and "str")
        {
            string cmd = data[0];
            switch (cmd)
            {
                case "warning":
                    string LockerZonePacket = Convert.ToString(packet);
                    if (LockerZonePacket.Contains("a Locked zone.") || LockerZonePacket.Contains("is not available."))
                        LockedZoneWarning = true;
                    break;
            }
        }
    }

    public void LockedMaps(string Pname, bool Cancel = false, int RoomNumber = 100000, List<string>? LockedMapList = null)
    {
        LockedMapList ??= new();
        LockedMapList.AddRange(_LockedMapsList.ToArray());
        PlayerInfo? foundPlayer = null;
        GC.Collect();
        if (Cancel || Bot.Map.PlayerExists(Pname))
        {
            Core.Logger($"Already at {Pname} or Cancel is true, stopping LockedMaps.", "LockedMaps");
            return;
        }

        // Flatten any comma-separated entries
        LockedMapList = LockedMapList
            .SelectMany(entry => entry.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToList();

        _LockedMapsList.AddRange(LockedMapList.Where(map => !_LockedMapsList.Contains(map)));

        if (_LockedMapsList.Count > 0)
            Core.Logger($"Custom locked maps added: {string.Join(", ", LockedMapList)}");


        if (Pname != null)
            b_playerName = Pname;

        string playerLogPath = Path.Combine(CoreBots.ButlerLogDir, b_playerName + ".txt");
        if (File.Exists(playerLogPath))
        {
            string? targetMap = File.ReadLines(playerLogPath).FirstOrDefault();
            if (targetMap != null)
            {
                if (Bot.Map.Name != targetMap)
                {
                    Core.JumpWait();
                    Bot.Map.Join($"{targetMap}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(targetMap);
                }
                Core.MeasureExecutionTime(() =>
              {
                  if (b_playerName != null && Bot.Map.TryGetPlayer(b_playerName, out PlayerInfo? _PO) && _PO != null)
                  {
                      foundPlayer = _PO;
                      return;
                  }
              });

                if (foundPlayer != null)
                {
                    Core.Logger($"Found {b_playerName} in /{targetMap}");
                    return;
                }
            }
        }

        string[] NonMemMaps = { "tercessuinotlim", "doomvaultb", "doomvault", "shadowrealmpast", "battlegrounda", "battlegroundb", "battlegroundc", "battlegroundd", "battlegrounde", "battlegroundf", "doomwood", "shadowrealm", "confrontation", "darkoviaforest", "ledgermayne", "hollowdeep", "hyperium", "willowcreek", "voidflibbi", "voidnightbane", "championdrakath", "ultraezrajal", "ultrawarden", "ultraengineer", "ultradage", "ultratyndarius", "ultranulgath", "ultradrago", "ultradarkon", "ultraspeaker" };
        string[] MemMaps = { "shadowlordpast", "binky", "superlowe" };
        string[] EventMaps = Array.Empty<string>();

        // Map and quests to use to see the boss without doing the story
        (string Map, int QuestUpdate)[] UpdateQuest = new[]
        {
        ("ultradrakath", 3879),
        ("bocklincastle", 102520),
        ("bocklingrove", 10239)
        };

        var levelLockedMaps = new[]
        {
        new { Map = "icestormunder", LevelRequired = 75 },
        new { Map = "icewing", LevelRequired = 75 },
        new { Map = "battlegrounde", LevelRequired = 61 },
        new { Map = "voidxyfrag", LevelRequired = 80 },
        new { Map = "voidnerfkitten", LevelRequired = 80 }
    };

        int maptry = 1;
        int mapCount = LockedMapList.Count == 0
        // If count = 0, add all resticted maps, else do the custom maps only.
            ? (Core.IsMember ? NonMemMaps.Length + MemMaps.Length + EventMaps.Length + levelLockedMaps.Length
                             : NonMemMaps.Length + EventMaps.Length + levelLockedMaps.Length)
            : LockedMapList.Count;

        if (LockedMapList.Count == 0)
        {
            foreach (string map in EventMaps)
            {
                if (Bot.ShouldExit)
                    return;

                if (!Core.isSeasonalMapActive(map)) continue;

                Core.Logger($"[{(_LockedMapsList.Count > 9 ? $"{maptry:D2}/{mapCount:D2}" : $"{maptry}/{mapCount}")}] Searching /{map}", "LockedZoneHandler => LockedMapList");

                if (Bot.Map.Name != map)
                {
                    Bot.Map.Join($"{map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(map);
                }

                Core.Sleep();

                if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                {
                    Core.Logger($"Found {b_playerName} in /{map}");
                    return;
                }
                else continue;
            }

            foreach (var mapInfo in levelLockedMaps)
            {
                if (Bot.ShouldExit)
                    return;

                if (Bot.Player.Level < mapInfo.LevelRequired)
                {
                    Core.Logger($"Level too low for /{mapInfo.Map} (required: {mapInfo.LevelRequired}, current: {Bot.Player.Level})");
                    continue;
                }

                Core.Logger($"[{maptry++:D2}/{mapCount:D2}] Searching /{mapInfo.Map}", "LockedZoneHandler =>  !LockedMapList");

                if (Bot.Map.Name != mapInfo.Map)
                {
                    Bot.Map.Join($"{mapInfo.Map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(mapInfo.Map);
                }

                Core.Sleep();

                if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                {
                    Core.Logger($"Found {b_playerName} in /{mapInfo.Map}");
                    return;
                }
                else continue;
            }

            foreach (string map in NonMemMaps)
            {
                if (Bot.ShouldExit)
                    return;

                Core.Logger($"[{(_LockedMapsList.Count > 9 ? $"{maptry:D2}/{mapCount:D2}" : $"{maptry}/{mapCount}")}] Searching /{map}", "LockedZoneHandler => LockedMapList");

                if (Bot.Map.Name != map)
                {
                    Bot.Map.Join($"{map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(map);
                }

                Core.Sleep();

                if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                {
                    Core.Logger($"Found {b_playerName} in /{map}");
                    return;
                }
                else continue;
            }

            if (Core.IsMember)
            {
                foreach (string map in MemMaps)
                {
                    if (Bot.ShouldExit)
                        return;

                    Core.Logger($"[{(_LockedMapsList.Count > 9 ? $"{maptry:D2}/{mapCount:D2}" : $"{maptry}/{mapCount}")}] Searching /{map}", "LockedZoneHandler => LockedMapList");

                    if (Bot.Map.Name != map)
                    {
                        Bot.Map.Join($"{map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                        Bot.Wait.ForMapLoad(map);
                    }

                    Core.Sleep();

                    if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                    {
                        Core.Logger($"Found {b_playerName} in /{map}");
                        return;
                    }
                    else continue;
                }
            }
        }
        else
        {
            foreach ((string Map, int QuestUpdate) in UpdateQuest)
            {
                if (Bot.ShouldExit)
                    return;

                if (Bot.Map.Name != Map)
                {
                    Bot.Map.Join($"{Map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(Map);
                }

                Core.Sleep();

                if (QuestUpdate > 0)
                    Bot.Quests.UpdateQuest(QuestUpdate);

                if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                {
                    Core.Logger($"Found {b_playerName} in /{Map}");
                    return;
                }
                maptry++;
            }

            foreach (string map in _LockedMapsList)
            {
                if (Bot.ShouldExit || LockedMapList.Count <= 0)
                    return;

                Core.Logger($"[{(_LockedMapsList.Count > 9 ? $"{maptry:D2}/{mapCount:D2}" : $"{maptry}/{mapCount}")}] Searching /{map}", "LockedZoneHandler => Custom LockedMapList");

                if (Bot.Map.Name != map)
                {
                    Bot.Map.Join($"{map}-{RoomNumber}", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(map);
                }

                Core.Sleep();

                if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
                {
                    Core.Logger($"Found {b_playerName} in /{map}");
                    return;
                }
                else continue;
            }
        }

        if (!(b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName)))
        {
            Core.Join($"whitemap-{RoomNumber}");
            Core.Logger($"Could not find {b_playerName} in any of the maps.", "LockedZoneHandler");
        }

        if (!b_shouldHibernate)
            return;

        Core.Logger($"Bot will now hibernate and try to /goto {b_playerName} every {b_hibernationTimer} seconds", "LockedZoneHandler");

        int min = 0;
        while (!Bot.ShouldExit)
        {
            for (int t = 0; t < b_hibernationTimer; t++)
            {
                Core.Sleep(1000);
                if (Bot.ShouldExit)
                    return;
            }

            if (b_playerName != null && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(b_playerName))
            {
                Core.Logger($"{b_playerName} found!");
                break;
            }

            min += b_hibernationTimer;
            if (min >= 300)
            {
                Core.Logger($"Bot has been hibernating for {min / 60} minutes.");
                min = 0;
            }
        }
    }

    public void PriorityAttack(List<string>? mons = null)
    {
        if (_attackPriority != null)
        {
            mons ??= new List<string>();
            mons.AddRange(_attackPriority);
        }

        if (mons?.Count > 0)
        {
            foreach (string mon in mons)
            {
                Monster? priorityMonster = Bot.Monsters.CurrentMonsters
                    .FirstOrDefault(m => m.Name?.FormatForCompare() == mon.FormatForCompare() && m.Cell == Bot.Player.Cell) ?? null;

                if (priorityMonster != null)
                    Bot.Kill.Monster(priorityMonster.MapID);
                else Bot.Kill.Monster("*");
            }
        }
        else Bot.Combat.Attack("*");

    }

    private async void MapNumberParses(string map)
    {
        // Wait untill the full name I.E. "battleon-12345" is set
        if (string.IsNullOrEmpty(Bot.Map.FullName))
        {
            for (int a = 0; a < 10; a++)
            {
                if (!string.IsNullOrEmpty(Bot.Map.FullName))
                    break;
                await Task.Delay(Core.ActionDelay);
                if (a == 9)
                    return;
            }
        }

        if (!int.TryParse(Bot.Map.FullName.Split('-').Last(), out int mapNr) || map == b_prevRoom || !Bot.Map.PlayerExists(b_playerName!))
            return;

        // If the number is the same number as on the previous map
        if (b_allocRoomNr == mapNr)
        {
            // If the set private room number wasn't correct
            if (Core.PrivateRoomNumber != mapNr)
            {
                Core.Logger("Static room number detected. PrivateRoomNumber is now " + mapNr);
                Core.PrivateRoomNumber = mapNr;
            }
            Core.PrivateRooms = mapNr >= 1000;
            Bot.Events.MapChanged -= MapNumberParses;
            return;
        }

        b_prevRoom = map;
        b_allocRoomNr = mapNr;
    }
    private int b_allocRoomNr = 0;
    private string? b_prevRoom = null;

    private void CopyWalkListener(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "str")
        {
            string cmd = data[0];
            switch (cmd)
            {
                //movement in the same cell || From server: %xt%uotls%-1%{playerName}%sp:8,tx:181,ty:358,strFrame:Bigger%
                //movement to another cell || From server: %xt%uotls%-1%{playerName}%mvts:-1,px:500,py:375,strPad:Left,bResting:false,mvtd:0,tx:0,ty:0,strFrame:Bigger%
                case "uotls":
                    string WalkPacket = Convert.ToString(packet);
                    if (!WalkPacket.Contains(b_playerName!))
                        break;

                    foreach (string str in WalkPacket.Split(','))
                    {
                        string spl = "";
                        if (str.Contains(':'))
                            spl = str.Split(':')[1];

                        switch (str.Split(':')[0])
                        {
                            // Setting X cordinate
                            case "tx":
                                moveX = int.Parse(spl);
                                break;

                            // Setting Y cordinate
                            case "ty":
                                moveY = int.Parse(spl);
                                break;

                            // Setting speed
                            case "sp":
                                moveSpeed = int.Parse(spl);
                                break;
                        }
                    }

                    if (moveX != 0 || moveY != 0)
                        Bot.Flash.Call("walkTo", moveX, moveY, moveSpeed);
                    break;
            }
        }
    }
    private int moveX = 0;
    private int moveY = 0;
    private int moveSpeed = 0;

    private bool ScriptStopping(Exception? e)
    {
        ButlerStop();
        return true;
    }

    private void ButlerStop()
    {
        // Removing listeners
        Bot.Events.MapChanged -= MapNumberParses;
        Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
        Bot.Events.ExtensionPacketReceived -= CopyWalkListener;

        // Delete communication files
        if (File.Exists(commFile()))
            File.Delete(commFile());
        Bot.Events.PlayerAFK -= PlayerAFK;
    }

    private string commFile() => Path.Combine(CoreBots.ButlerLogDir, $"{Core.Username().ToLower()}~!{b_playerName}.txt");
    public string? b_breakOnMap = null;
    #endregion
}

public class ArmyLogging
{
    private static readonly object lockObject = new();
    private string logFilePath = string.Empty;
    public string message = string.Empty;

    // public ArmyLogging(string fileName = "ArmyLog.txt")
    // {
    //     logFilePath = Path.Combine(ClientFileSources.SkuaOptionsDIR, fileName);
    //     ClearLogFile();
    // }
    public void setLogName(string fileName)
    {
        logFilePath = Path.Combine(ClientFileSources.SkuaOptionsDIR, fileName + ".log");
        //ClearLogFile();
    }

    public void registerMessage(string msg)
    {
        message = msg;
    }

    public bool isEmpty()
    {
        if (new FileInfo(logFilePath).Length == 0)
        {
            return true;
        }
        using FileStream stream = File.OpenRead(logFilePath);
        return stream.Length == 0;
    }

    public bool isAlreadyInLog(string[] playersList)
    {
        try
        {
            List<string> lines = ReadLog();
            string joinedString = string.Join(Environment.NewLine, lines);
            string[] players = playersList;
            foreach (string pl in players)
            {
                if (!joinedString.Contains($"{pl.ToLower()}:done:{message}"))
                {
                    return false;
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void WriteLog(string logMessage)
    {
        lock (lockObject)
        {
            using StreamWriter w = File.AppendText(logFilePath);
            w.WriteLine($"{logMessage}");
        }
    }

    public List<string> ReadLog()
    {
        List<string> lines = new();
        lock (lockObject)
        {
            using StreamReader r = File.OpenText(logFilePath);
            string? line;
            while ((line = r.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }
        return lines;
    }

    public void ClearLogFile()
    {
        lock (lockObject)
        {
            using FileStream fs = File.Open(logFilePath, FileMode.Create, FileAccess.Write);
            // File is truncated and cleared
        }
    }
}

// Top-level static class for DateTime extension methods
public static class DateTimeExtensions
{
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
}