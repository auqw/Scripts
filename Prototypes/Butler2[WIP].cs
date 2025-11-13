/*
name: Butler
description: This will follow a player and copy their actions and do attack actions.
tags: butler, follow, player, copy, actions, attack, maidr, auto, goto
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Players;
using Skua.Core.Options;

public class Butler2
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public static CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army => new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "Butler";
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<string>(
            "playerName",
            "Player Name",
            "Insert the name of the player to follow",
            ""
        ),
        new Option<ClassType>(
            "classType",
            "Class Type",
            "This uses the farm or solo class set in [Options] > [CoreBots]",
            ClassType.Farm
        ),
        new Option<bool>(
            "rejectDrops",
            "Reject Drops",
            "Do you wish for the Butler to reject all drops? If false, your drop screen will fill up.",
            true
        ),
        new Option<string>(
            "attackPriority",
            "Attack Priority",
            "Fill in the monsters that the bot should prioritize (in order), seperated with a , (comma).",
            ""
        ),
        new Option<string>(
            "Quests",
            "Quests",
            "This will Register the Quests to be ran asynchronously",
            ""
        ),
        new Option<string>(
            "Drops",
            "Drops",
            "Insert the name of the Drops to be picked up, seperated by a , (comma).",
            ""
        ),
        new Option<bool>(
            "lockedMaps",
            "Locked Zone Handling",
            "When the followed account goes in to a locked map, this function allows the Butler to follow that account.",
            true
        ),
        new Option<string>(
            "CustomLockedMapList",
            "Custom Locked Maps",
            "Fill in the Maps that the bot will check (in order), if the player is not in the current map, split with a , (comma).",
            ""
        ),
        new Option<bool>(
            "copyWalk",
            "Copy Walk",
            "Set to true if you want to move to the same position of the player you follow.",
            false
        ),
        new Option<string>(
            "roomNumber",
            "Room Number",
            "Insert the room number which will be used when looking through Locked Zones.",
            "999999"
        ),
        new Option<string>(
            "hibernationTimer",
            "Hibernate Timer",
            "How many seconds should the bot wait before trying to /goto again?\nIf set to 0, it will not hibernate at all.",
            "60"
        ),
        new Option<bool>(
            "unlockAllMaps",
            "Unlock All Maps",
            "Grants access to all maps, even if your account hasn't completed the required quests.",
            false
        ),
    };

    CancellationTokenSource? ButlerTokenSource;
    CancellationToken _cancellationToken;

    int gotoTry = 0;
    const int maxTry = 20;
    bool LockedZone = false;
    bool needJump = false;
    string? cellJump = null;
    string? padJump = null;
    string? followedPlayerCell = null;
    string? playerToFollow = null;
    bool isGoto = false;
    bool initializationDone = false;
    int RoomNumber = 0;
    public bool b_shouldHibernate = true;
    List<string> CustomMaps = new();
    List<string> attackPriorityItems = new();

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);
        DoButler(Bot.Config!.Get<string>("playerName"), cancellationToken: _cancellationToken);

        Core.SetOptions(false, disableClassSwap: true);
    }

    public void DoButler(
        string? playerName,
        bool log = false,
        CancellationToken cancellationToken = default
    )
    {
        // Core.DL_Enable();
        // Initialize the CancellationTokenSource and get the Token
        ButlerTokenSource = new CancellationTokenSource();
        _cancellationToken = ButlerTokenSource.Token;

        #region Setup area
        Core.OneTimeMessage(
            "Butler2 [WIP]",
            "this butler is more stable, but atm only has the follow and attack feature",
            forcedMessageBox: true
        );
        if (string.IsNullOrEmpty(playerName))
        {
            Core.Logger("You need to set a player name.");
            return;
        }
        else
            playerToFollow = playerName;

        // Room Number
        if (!int.TryParse(Bot.Config?.Get<string>("roomNumber"), out int roomNr))
        {
            Core.Logger(
                "Please set a valid room number in the configuration.",
                "Invalid Room Number",
                messageBox: true
            );
            Bot.Config?.Configure(); // Opens the config UI for user input
            Bot.Stop(false); // Stops the bot but allows cleanup
        }
        else
        {
            RoomNumber = roomNr;
            Core.Logger($"Room Number: {RoomNumber}");
        }

        if (Bot.Config!.Get<bool>("rejectDrops"))
        {
            Bot.Options.RejectAllDrops = true;
        }

        // Load Config Data
        string drops = Bot.Config!.Get<string>("Drops") ?? "";
        string attackPriority = Bot.Config!.Get<string>("attackPriority") ?? "";
        string quests = Bot.Config!.Get<string>("Quests") ?? "";
        string CustomLockedMapList = Bot.Config!.Get<string>("CustomLockedMapList") ?? "";

        // Process Config Data
        Core.AddDrop(
            drops
                .Split(',', StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray()
        );

        // Process attackPriority and add to Army._attackPriority
        if (!string.IsNullOrEmpty(attackPriority))
        {
            attackPriorityItems = attackPriority
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            Army._attackPriority.AddRange(attackPriorityItems);
            Core.Logger($"Attack Prio. added: {string.Join(", ", attackPriorityItems)}");
        }

        if (!string.IsNullOrEmpty(CustomLockedMapList))
        {
            CustomMaps = CustomLockedMapList
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            Core.Logger($"Custom locked maps added: {string.Join(", ", CustomMaps)}");
        }

        // Process quests and register them
        if (!string.IsNullOrEmpty(quests))
        {
            var questItems = quests
                .Split(',', StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(int.Parse)
                .ToArray();
            Core.RegisterQuests(questItems);
        }

        // EQUIP CLASS & START BUTLER MODE
        Core.EquipClass(Bot.Config!.Get<ClassType>("classType"));
        Bot.Events.PlayerAFK += PlayerAFK;
        Bot.Events.ExtensionPacketReceived += MapHandler;

        if (Bot.Config!.Get<bool>("unlockAllMaps"))
        {
            UnlockAllMaps();
        }

        StartButler(playerName, roomNr, cancellationToken: cancellationToken);

        // --- Call GoToPlayer at script start if not in same map as player ---
        if (
            !string.IsNullOrEmpty(playerName)
            && Bot.Map.PlayerNames != null
            && !Bot.Map.PlayerNames.Contains(playerName)
        )
        {
            isGoto = true;
            Task.Run(() => GoToPlayer(playerName, _cancellationToken), cancellationToken);
        }
        Core.Logger($"Butler started for player {playerName}");
        Core.Sleep(5000);

        #endregion Setup area

        int skillIndex = 0;
        int[] skillList = { 1, 2, 3, 4 };

        while (
            !Bot.ShouldExit
            && !ButlerTokenSource.IsCancellationRequested
            && (!isGoto || !LockedZone)
        )
        {
            #region ignore this

            if (!Bot.Player.LoggedIn)
            {
                Core.Logger("You are not logged in.");
                return;
            }

            #endregion ignore this
            Bot.Wait.ForMapLoad(Bot.Map.Name);

            if (!Bot.Map.TryGetPlayer(playerName, out PlayerInfo? player) || isGoto)
                Task.Run(() => GoToPlayer(playerName, _cancellationToken), cancellationToken);

            if (isGoto || LockedZone)
            {
                LockedZone = false;
                Bot.Wait.ForTrue(() => player != null, 20);
            }

            if (attackPriorityItems.Count > 0 && !Bot.Combat.StopAttacking)
                Army.PriorityAttack(attackPriorityItems);
            else if (!Bot.Combat.StopAttacking)
                Bot.Combat.Attack("*");

            // Use skills in rotation
            Bot.Skills.UseSkill(skillList[skillIndex]);
            skillIndex = (skillIndex + 1) % skillList.Length;

            Core.Sleep();
        }
        StopButler(cancellationToken);
    }

    public void StartButler(
        string? playerName,
        int roomnr,
        int hibernateTimer = 1,
        CancellationToken cancellationToken = default
    )
    {
        ButlerTokenSource = new CancellationTokenSource();
        _cancellationToken = ButlerTokenSource.Token;

        Task.Run(
            async () =>
            {
                while (!Bot.ShouldExit && !ButlerTokenSource.IsCancellationRequested)
                {
                    if (string.IsNullOrWhiteSpace(playerName))
                    {
                        Core.Logger("You need to set a player name.");
                        StopButler();
                        return;
                    }

                    if (Bot.ShouldExit || ButlerTokenSource.IsCancellationRequested)
                    {
                        Core.Logger("Bot is exiting, canceling Butler task...");
                        ButlerTokenSource?.Cancel();
                        ButlerTokenSource?.Dispose();
                        ButlerTokenSource = new CancellationTokenSource();
                        StopButler();
                        return;
                    }

                    if (!Bot.Player.LoggedIn)
                    {
                        Core.Logger("You are not logged in.");
                        StopButler();
                        return;
                    }

                    await Task.Delay(Core.ActionDelay * 2);

                    if (
                        needJump
                        || (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(playerName))
                    )
                    {
                        if (!initializationDone)
                        {
                            if (
                                !Bot.Map.TryGetPlayer(playerName, out PlayerInfo? player)
                                || player?.Cell == null
                                || player.Pad == null
                            )
                                continue;

                            if (!string.IsNullOrEmpty(player.Cell))
                                cellJump ??= player.Cell;
                            if (!string.IsNullOrEmpty(player.Pad))
                                padJump ??= player.Pad;

                            if (cellJump != null && padJump != null)
                                initializationDone = true;
                        }

                        if (cellJump != null && padJump != null)
                        {
                            if (Bot.Player.Cell != cellJump)
                            {
                                Bot.Map.Jump(cellJump, padJump, false);
                                Bot.Wait.ForCellChange(cellJump);
                            }
                        }
                        else
                        {
                            Core.Logger("Jumping to player failed, cell or pad is null.");
                            continue;
                        }

                        needJump = false;
                        if (initializationDone)
                            continue;
                    }

                    if (
                        Bot.Map.PlayerNames != null
                            && !Bot.Map.PlayerNames.Contains(playerName)
                            && !Bot.ShouldExit
                        || !ButlerTokenSource.IsCancellationRequested
                    )
                    {
                        if (!LockedZone && isGoto)
                        {
                            if (Bot.Player.Cell != "Enter" && cellJump != null)
                            {
                                Bot.Map.Jump("Enter", "Spawn", autoCorrect: false);
                                Bot.Wait.ForCellChange(cellJump);
                            }
                            GoToPlayer(playerName, _cancellationToken);
                        }

                        if (LockedZone)
                        {
                            LockedZone = false;
                            isGoto = false;
                            Army.LockedMaps(
                                playerName,
                                ButlerTokenSource.IsCancellationRequested,
                                RoomNumber,
                                CustomMaps
                            );
                        }
                    }

                    if (
                        followedPlayerCell != null
                        && Bot.Player.Cell == followedPlayerCell
                        && isGoto
                    )
                    {
                        Core.Logger("Successfully moved to player, resetting gotoTry.");
                        gotoTry = 0;
                        isGoto = false;
                    }

                    if (gotoTry > 5 && b_shouldHibernate)
                    {
                        if (gotoTry >= maxTry)
                        {
                            Core.Logger(
                                $"Max retries reached ({maxTry}). Forcing \"Hibernate\" mode."
                            );
                            b_shouldHibernate = true;
                        }
                        else
                        {
                            Core.Logger(
                                (gotoTry >= 1 ? "Continuing" : "Entering") + " hibernation mode..."
                            );
                        }

                        isGoto = false;

                        if (Hibernate(playerName))
                            gotoTry = 0;
                    }

                    await Task.Delay(Core.ActionDelay * 2);
                }
            },
            _cancellationToken
        );
    }

    public void GoToPlayer(string name, CancellationToken cancellationToken = default)
    {
        ButlerTokenSource = new CancellationTokenSource();
        _cancellationToken = ButlerTokenSource.Token;

        if (!isGoto)
            return;

        while (
            !Bot.ShouldExit
            && !Bot.Player.Alive
            && ButlerTokenSource?.IsCancellationRequested == false
        )
            Task.Delay(500, cancellationToken);

        if (ButlerTokenSource?.IsCancellationRequested != false)
        {
            Core.Logger("Token canceled.");
            StopButler(cancellationToken);
        }

        if (Bot.Map.PlayerExists(name))
        {
            LockedZone = false;
            isGoto = false;
            b_shouldHibernate = false;
            gotoTry = 0;
        }

        Bot.Send.Packet($"%xt%zm%cmd%1%goto%{name}%");
        Task.Delay(LockedZone ? 2500 : 1500, cancellationToken);
        return;
    }

    bool Hibernate(string? playername, CancellationToken cancellationToken = default)
    {
        ButlerTokenSource = new();
        _cancellationToken = ButlerTokenSource.Token;

        if (string.IsNullOrWhiteSpace(playername) || Bot.ShouldExit || !Bot.Player.LoggedIn)
        {
            Core.Logger("You need to set a player name.");
            StopButler(cancellationToken);
            Bot.Stop(false);
            return false;
        }

        DateTime startTime = DateTime.Now;
        int lastLoggedMinute = -1;

        while (
            !Bot.ShouldExit
            && !ButlerTokenSource.IsCancellationRequested
            && Bot.Map.PlayerNames != null
            && !Bot.Map.PlayerNames.Contains(playername)
        )
        {
            if (LockedZone)
            {
                b_shouldHibernate = false;
                break;
            }

            if (!b_shouldHibernate)
                continue;

            for (int i = 0; i < 5; i++)
            {
                Core.Sleep(1000);
                if (playerToFollow != null)
                    GoToPlayer(playerToFollow, _cancellationToken);
                else
                {
                    Core.Logger("No player to follow.");
                }

                if (
                    Bot.ShouldExit
                    || ButlerTokenSource.IsCancellationRequested
                    || Bot.Map.PlayerNames.Contains(playername)
                )
                {
                    b_shouldHibernate = false;
                    break;
                }

                int minutesElapsed = (int)(DateTime.Now - startTime).TotalMinutes;
                if (minutesElapsed > 0 && minutesElapsed != lastLoggedMinute)
                {
                    Core.Logger(
                        $"Bot has been hibernating for {minutesElapsed} minute{(minutesElapsed == 1 ? "" : "s")}."
                    );
                    lastLoggedMinute = minutesElapsed;
                }
            }
        }

        return true;
    }

    public void StopButler(CancellationToken cancellationToken = new())
    {
        if (ButlerTokenSource != null)
        {
            ButlerTokenSource.Cancel();
            ButlerTokenSource.Dispose();
            ButlerTokenSource = new CancellationTokenSource();
        }

        _cancellationToken = default;

        Bot.Events.ExtensionPacketReceived -= MapHandler;
        Bot.Events.PlayerAFK -= PlayerAFK;

        GC.Collect();
        Core.Logger("Butler task has been canceled.");
    }

    public void MapHandler(dynamic packet)
    {
        ButlerTokenSource = new();
        _cancellationToken = ButlerTokenSource.Token;

        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        string? playerName = Bot.Config!.Get<string>("playerName");
        if (string.IsNullOrEmpty(playerName))
            return;
        // Handle str-type packets with data containing movement information
        if (type == "str" && data.Count >= 4)
        {
            // --- Handle exitArea first ---
            if (data[0]?.ToString() == "exitArea")
            {
                string? leftPlayer = data[3]?.ToString();
                if (
                    !string.IsNullOrEmpty(playerToFollow)
                    && !string.IsNullOrEmpty(leftPlayer)
                    && leftPlayer.Equals(playerToFollow, StringComparison.OrdinalIgnoreCase)
                )
                {
                    followedPlayerCell = null;
                    Core.Logger($"Detected {playerToFollow} left the map. Attempting to follow...");
                    isGoto = true; // Only set here, not in the main loop
                }
                return;
            }

            // Check if the packet matches the followed player
            if (
                data[0]?.ToString() == "uotls"
                && data[2]?.ToString().ToLower() == playerName.ToLower()
            )
            {
                isGoto = true; // Only set here, not in the main loop
                string? movementData = data[3]?.ToString();
                if (string.IsNullOrEmpty(movementData))
                    return;
                followedPlayerCell = null;
                string[] movementParts = movementData.Split(',', StringSplitOptions.TrimEntries);
                string? cell = null,
                    pad = null;
                int x = 0,
                    y = 0,
                    sp = 0;
                bool xSuccess = false,
                    ySuccess = false,
                    spSuccess = false;

                foreach (string part in movementParts)
                {
                    string[] keyValue = part.Split(':');
                    if (keyValue.Length != 2)
                        continue;

                    string key = keyValue[0],
                        value = keyValue[1];
                    switch (key)
                    {
                        case "strFrame":
                            cell = value;
                            break;
                        case "strPad":
                            pad = value;
                            break;
                        case "tx":
                            xSuccess = int.TryParse(value, out x);
                            break;
                        case "ty":
                            ySuccess = int.TryParse(value, out y);
                            break;
                        case "sp":
                            spSuccess = int.TryParse(value, out sp);
                            break;
                    }
                }

                // If both cell and pad are present, it's a jump
                if (!string.IsNullOrEmpty(cell) && !string.IsNullOrEmpty(pad))
                {
                    followedPlayerCell = cell;
                    needJump = true;
                    cellJump = cell;
                    padJump = pad;
                    // V for debug purp. v
                    // Core.Logger($"Need to Jump to Cell: {cellJump}, Pad: {padJump}");
                    if (!string.IsNullOrEmpty(cell))
                    {
                        if (Bot.Player.Cell != cell || Bot.Player.Pad != pad)
                        {
                            Bot.Map.Jump(cellJump, padJump, false);
                            Bot.Wait.ForCellChange(cellJump);
                        }
                        followedPlayerCell = null;
                    }
                }
                // If only tx/ty are present, it's a walk
                else if (xSuccess && ySuccess)
                {
                    Bot.Flash.Call("walkTo", x, y, sp);
                    Core.Logger($"Walking to X: {x}, Y: {y}, Speed: {sp}");
                    isGoto = false;
                    followedPlayerCell = null;
                }
            }
        }
        // Handle warning message
        else if (data[0]?.ToString() == "warning")
        {
            string? warningMessage = data[2]?.ToString();

            if (!string.IsNullOrEmpty(warningMessage))
            {
                if (warningMessage.Contains("Locked zone"))
                {
                    Core.Logger(
                        $"{playerToFollow} is in a Locked map (cannot goto player). Scanning Locked maps for player"
                    );
                    b_shouldHibernate = false;
                    LockedZone = true;
                }
                else if (warningMessage.Contains("Please slow down. Last action was too soon!"))
                {
                    // do nothing
                }
                else
                {
                    LockedZone = false;
                    b_shouldHibernate = true;
                }
            }
        }
        // Handle json-type packets with moveToArea command
        else if (type == "json" && data.cmd.ToString() == "moveToArea" && data.uoBranch != null)
        {
            foreach (dynamic uoBranch in data.uoBranch)
            {
                string uoName = Convert.ToString(uoBranch.uoName);
                if (
                    !string.IsNullOrEmpty(playerToFollow)
                    && uoName?.ToLower() == playerToFollow.ToLower()
                )
                {
                    followedPlayerCell = uoBranch.strFrame;
                    Core.Logger($"Updated followedPlayerCell: {followedPlayerCell}");
                }
            }
        }
    }

    public void PlayerAFK()
    {
        Core.Logger("Anti-AFK engaged");
        Core.Sleep(1500);
        Bot.Send.Packet("%xt%zm%afk%1%false%");
    }

    public void UnlockAllMaps()
    {
        ButlerTokenSource = new();
        _cancellationToken = ButlerTokenSource.Token;

        Stopwatch stopwatchTotal = Stopwatch.StartNew();

        // Use configured paths directly
        string questDataPath = ClientFileSources.SkuaQuestsFile;

        dynamic[] v = JsonConvert.DeserializeObject<dynamic[]>(File.ReadAllText(questDataPath))!;

        Stopwatch stopwatchExtraction = Stopwatch.StartNew();
        Dictionary<int, (int Slot, int Value)> questData = ExtractQuestDataFromScripts();
        stopwatchExtraction.Stop();

        Core.Logger(
            $"[Profiling] Quest data extraction took {stopwatchExtraction.ElapsedMilliseconds} ms"
        );

        // Get last quests per slot (final quest value)
        var lastQuestsPerSlot = questData
            .Where(q => q.Value.Slot >= 0 && q.Value.Value >= 0)
            .GroupBy(q => q.Value.Slot)
            .Select(g => g.OrderByDescending(q => q.Value.Value).First())
            .ToList();

        Core.Logger(
            $"[Butler] Unlocking {lastQuestsPerSlot.Count} maps by updating their final quests per slot in batches..."
        );

        const int batchSize = 30;
        for (int i = 0; i < lastQuestsPerSlot.Count; i += batchSize)
        {
            var batch = lastQuestsPerSlot.Skip(i).Take(batchSize);

            Stopwatch stopwatchBatch = Stopwatch.StartNew();

            foreach ((int id, (int slot, int value)) in batch)
                if (!Core.isCompletedBefore(id))
                {
                    Bot.Quests.UpdateQuest(id);
                    Bot.Sleep(Core.ActionDelay + Bot.Random.Next(100, 300));
                }

            stopwatchBatch.Stop();
            Core.Logger(
                $"[Profiling] Batch {(i / batchSize) + 1} processing took {FormatElapsedTime(stopwatchBatch.Elapsed)}"
            );
        }

        GC.Collect();
        stopwatchTotal.Stop();
        Core.Logger(
            $"[Profiling] UnlockAllMaps total time: {FormatElapsedTime(stopwatchTotal.Elapsed)}"
        );
    }

    public Dictionary<int, (int Slot, int Value)> ExtractQuestDataFromScripts()
    {
        ButlerTokenSource = new();
        _cancellationToken = ButlerTokenSource.Token;

        string scriptsRoot = ClientFileSources.SkuaScriptsDIR;
        string questDataPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");

        var questIDs = new HashSet<int>();
        Regex regex = new(@"Core\.isCompletedBefore\s*\(\s*(\d+)\s*\)");

        foreach (string subfolder in new[] { "Seasonal", "Story" })
        {
            string folderPath = Path.Combine(scriptsRoot, subfolder);
            if (!Directory.Exists(folderPath))
            {
                Core.Logger($"[Butler] Folder does not exist: {folderPath}");
                continue;
            }

            foreach (
                string file in Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
            )
            {
                foreach (string line in File.ReadLines(file))
                {
                    Match match = regex.Match(line);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int questID))
                        questIDs.Add(questID);
                }
            }
        }

        if (!File.Exists(questDataPath))
        {
            Core.Logger($"[Butler] QuestData.json not found at {questDataPath}, creating...");
            File.WriteAllText(questDataPath, "[]");
        }

        string json = File.ReadAllText(questDataPath);
        JArray questArray = JArray.Parse(json);

        ConcurrentDictionary<int, (int Slot, int Value)> result = new();

        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Parallel.ForEach(
            questArray.Cast<JObject>(),
            options,
            quest =>
            {
                int id = quest["ID"]?.Value<int>() ?? -1;
                if (!questIDs.Contains(id))
                    return;

                int slot = quest["Slot"]?.Value<int>() ?? -1;
                int value = quest["Value"]?.Value<int>() ?? -1;

                if (id > 0 && slot >= 0 && value >= 0)
                    result.TryAdd(id, (slot, value));
            }
        );

        return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private static string FormatElapsedTime(TimeSpan ts) =>
        $"{(int)ts.TotalHours:D2}hrs {ts.Minutes:D2}mns {ts.Seconds:D2}sec {ts.Milliseconds:D3}ms";
}
