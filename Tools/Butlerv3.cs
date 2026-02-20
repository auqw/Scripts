/*
name: Butler version 3.0
description: This will follow a player and copy their actions and do attack actions.
tags: butler, follow, player, copy, actions, attack, maidr, auto, goto, version 3
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using System.IO;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Options;

// Butler version: 3.0
public class Butler3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army => new();
    public bool DontPreconfigure = true;
    public string OptionsStorage = "ButlerV3";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "playerName",
            "Player Name",
            "Insert the name of the player to follow Capitals and punctuation are required.",
            "PlayerNameGoesHere"
        ),
        CoreBots.Instance.SkipOptions,
        new Option<string>(
            "lockedMapsList",
            "Custom Locked Maps",
            "Fill in the Maps that the bot will check (in order), if the player is not in the current map, split with a , (comma).",
            "Locked,maps,seperated,by,a,comma"
        ),
        new Option<string>(
            "RoomNumber",
            "RoomNumberForLockedMaps",
            "Room number to use when LockedMaps is triggered, if empty itll use your CoreBots PrivateRoom#",
            ""
        ),
        new Option<ClassType>(
            "classType",
            "Class Type",
            "This uses the farm or solo class set in [Options] > [CoreBots]",
            ClassType.Farm
        ),
    };

    bool LockedZoneWarning;
    string? playerName;
    ClassType classType;
    string? RN;
    List<string?> lockedMapList;

    // String? TargetCell;
    bool RoomFull;
    bool GotoisOff;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        BasicAFButler();
        Core.SetOptions(false);
    }

    public void BasicAFButler()
    {
        #region Setup
        Core.Logger("Joining whitemap, as starting on certain maps.. just breaks things? Not sure why :| ");
        Core.Join("whitemap-100000");

        // Subscribe once here, never touch it inside the loop
        Bot.Events.ExtensionPacketReceived += ChatListener;
        LockedZoneWarning = false;

        var lockedMapsConfig = Bot.Config!.Get<string>("lockedMapsList");
        lockedMapList = lockedMapsConfig?.Split(',').Select(m => m?.Trim()).ToList() ?? new List<string?>();

        playerName = Bot.Config!.Get<string>("playerName");
        classType = Bot.Config!.Get<ClassType>("classType");

        RN = !string.IsNullOrEmpty(Bot.Config!.Get<string>("RoomNumber"))
            ? Bot.Config!.Get<string>("RoomNumber")
            : Core.PrivateRoomNumber.ToString();

        if (string.IsNullOrEmpty(playerName))
        {
            Bot.Events.ExtensionPacketReceived -= ChatListener;
            Core.Logger("PlayerName is empty", "Empty PlayerName", true);
            return;
        }

        if (classType != ClassType.None)
            Core.EquipClass(classType);
        #endregion

        if (playerName == Bot.Player.Username)
            Core.Logger("THE FUCK ARE YOU FOLLOWING YOURSELF FOR RETARD?", "Retard alert", messageBox: true);

        while (!Bot.ShouldExit)
        {
            try
            {
                if (!Bot.Map.PlayerExists(playerName))
                {
                    // First attempt to goto before checking flags
                    Bot.Player!.Goto(playerName);
                    Bot.Sleep(1000);
                    Core.Logger($"{playerName} isn't on the current map, following!");

                    // Player is ignoring goto — stop everything
                    if (GotoisOff)
                    {
                        Core.Logger($"{playerName} is ignoring goto requests, Stopping script!");
                        Core.JumpWait();
                        Core.Join("whitemap-100000");
                        break; // exits while loop, hits cleanup below
                    }

                    // Tried to follow into a locked zone
                    if (LockedZoneWarning)
                    {
                        LockedZoneWarning = false;
                        Core.JumpWait();
                        Core.Join("whitemap-100000");

                        if (lockedMapList.Count > 0)
                        {
                            // Try each locked map to find the player
                            Core.Logger("LockedMaps handler Initiated.", "LockedMapList.Count > 0");
                            foreach (string? map in lockedMapList.Where(m => m != null))
                            {
                                if (Bot.ShouldExit)
                                    break;

                                Core.Join($"{map}-{RN}");
                                Bot.Wait.ForMapLoad(map!);

                                if (Bot.Map.PlayerExists(playerName))
                                {
                                    Bot.Player?.Goto(playerName);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // No locked maps configured — incrementally wait and retry goto
                            Core.Logger("LockedMap list is Empty, we'll Sleep incrementally", "lockedMapList <= 0");
                            Core.Join("whitemap-100000");

                            Random random = new();
                            int sleepTimer = 500;
                            const int maxSleep = 5000;
                            const int increment = 1000;

                            while (!Bot.ShouldExit && !Bot.Map.PlayerExists(playerName))
                            {
                                Core.Logger($"Sleeping for: {sleepTimer}");
                                Bot.Sleep(sleepTimer);
                                Bot.Player!.Goto(playerName);

                                if (sleepTimer < maxSleep)
                                    sleepTimer = Math.Min(sleepTimer + random.Next(increment), maxSleep);
                            }
                        }

                        continue; // re-evaluate from top of outer loop
                    }

                    // Room is full — wait incrementally and retry
                    if (RoomFull)
                    {
                        Random random = new();
                        int sleepTimer = 1000;
                        const int maxSleep = 5000;
                        const int increment = 1000;

                        while (!Bot.ShouldExit && !Bot.Map.PlayerExists(playerName!))
                        {
                            Bot.Sleep(sleepTimer);

                            if (sleepTimer < maxSleep)
                                sleepTimer = Math.Min(sleepTimer + random.Next(increment), maxSleep);

                            Bot.Player!.Goto(playerName!);

                            if (sleepTimer >= maxSleep)
                                Bot.Log("Room is still full, waiting for access or until we can goto the player.");
                        }

                        // Room freed up
                        RoomFull = false;
                        continue;
                    }

                    // No flags set — player just isn't on map yet, jump and retry
                    Core.JumpWait();
                    continue;
                }

                // Player is on the same map — follow and attack
                while (!Bot.ShouldExit)
                {
                    if (!Bot.Player!.Alive)
                        Bot.Wait.ForTrue(() => Bot.Player?.Alive ?? false, 20);

                    // Player left the map, break to outer loop to follow
                    if (!Bot.Map.PlayerExists(playerName))
                        break;

                    Bot.Player.Goto(playerName);
                    Bot.Sleep(500);
                    Bot.Combat.Attack("*");
                }
            }
            catch (Exception ex)
            {
                Core.Logger($"Error in main loop: {ex.Message}");
            }
        }

        // Always clean up the listener on exit, regardless of how we got here
        Bot.Events.ExtensionPacketReceived -= ChatListener;
        Core.JumpWait();
    }

    // void ChatListener(dynamic packet)
    // {
    //     try
    //     {
    //         if (packet == null)
    //             return;

    //         var paramsObj = packet["params"];
    //         if (paramsObj == null)
    //             return;

    //         string? type = paramsObj.type;
    //         if (type != "str")
    //             return;

    //         dynamic? dataObj = paramsObj.dataObj;
    //         if (dataObj == null)
    //             return;

    //         string? cmd = dataObj[0];
    //         if (string.IsNullOrEmpty(cmd))
    //             return;

    //         // ------------------------------
    //         // SERVER MESSAGES (%xt%server ...)
    //         // ------------------------------
    //         if (cmd == "server")
    //         {
    //             string? text = dataObj[2]?.ToString();
    //             if (string.IsNullOrWhiteSpace(text))
    //                 return;

    //             // Detect "is ignoring goto requests" on server channel
    //             if (text.Contains("is ignoring goto requests", StringComparison.OrdinalIgnoreCase))
    //             {
    //                 GotoisOff = true;
    //                 return;
    //             }
    //         }

    //         // ------------------------------
    //         // WARNING MESSAGES (existing)
    //         // ------------------------------
    //         if (cmd == "warning")
    //         {
    //             string chatPacket = Convert.ToString(packet) ?? string.Empty;

    //             if (
    //                 chatPacket.Contains("a Locked zone.", StringComparison.OrdinalIgnoreCase)
    //                 || chatPacket.Contains("is not available.", StringComparison.OrdinalIgnoreCase)
    //             )
    //                 LockedZoneWarning = true;

    //             if (chatPacket.Contains("is full", StringComparison.OrdinalIgnoreCase))
    //             {
    //                 Core.DebugLogger(
    //                     this,
    //                     $"Room is full, we'll wait incrementally, whilst trying to goto {playerName}"
    //                 );
    //                 RoomFull = true;
    //             }

    //             if (chatPacket.Contains("ignoring goto", StringComparison.OrdinalIgnoreCase))
    //             {
    //                 Core.DebugLogger(
    //                     this,
    //                     $"{playerName} is ignoring goto requests, Stopping script!"
    //                 );

    //                 Bot.StopSync(true);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Core.Logger( $"Error in ChatListener: {ex.Message}");
    //     }
    // }
    void ChatListener(dynamic packet)
    {
        try
        {
            if (packet == null)
                return;

            var paramsObj = packet["params"];
            if (paramsObj == null)
                return;

            string? type = paramsObj.type;
            if (type != "str")
                return;

            dynamic? dataObj = paramsObj.dataObj;
            if (dataObj == null)
                return;

            string? cmd = dataObj[0];
            if (string.IsNullOrEmpty(cmd))
                return;

            // SERVER MESSAGES (%xt%server ...)
            if (cmd == "server")
            {
                string? text = dataObj[2]?.ToString();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                if (text.Contains("is ignoring goto requests", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"{playerName} is ignoring goto requests (Disable `incognito` mode in CBO on the account your following), Stopping script!");
                    GotoisOff = true;
                }
            }
            // WARNING MESSAGES
            else if (cmd == "warning")
            {
                // Read the actual warning text instead of stringifying the whole packet
                string? warningText = dataObj[1]?.ToString();
                if (string.IsNullOrWhiteSpace(warningText))
                    return;

                if (warningText.Contains("a Locked zone.", StringComparison.OrdinalIgnoreCase)
                    || warningText.Contains("is not available.", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger("Zone is either Locked or unavailable. (maybe your missing an item, or the area requires membership?)");
                    LockedZoneWarning = true;
                }
                else if (warningText.Contains("is full", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"Room is full, we'll wait incrementally, whilst trying to goto {playerName}");
                    RoomFull = true;
                }
                else if (warningText.Contains("ignoring goto", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"{playerName} is ignoring goto requests (Disable `incognito` mode in CBO on the account your following), Stopping script!");
                    GotoisOff = true;
                }
            }
        }
        catch (Exception ex)
        {
            Core.Logger($"Error in ChatListener: {ex.Message}");
        }
    }
}

#region MyChild
//
//                                  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒░░
//                              ▓▓▓▓████████████████▓▓▓▓▒▒
//                         ▓▓▓▓████░░░░░░░░░░░░░░░░██████▓▓
//                      ▓▓████░░░░░░░░░░░░░░░░░░░░░░░░░░████
//                   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//                ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//              ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//             ▓▓██░░░░░░▓▓██░░  ░░░░░░░░░░░░░░░░░░░░▓▓██░░  ░░██
//           ▓▓██░░░░░░░░██████░░░░░░░░░░░░░░░░░░░░░░██████░░░░░░██
//          ▓▓██░░░░░░░░██████▓▓░░░░░░██░░░░██░░░░░░██████▓▓░░░░██
//         ▓▓██▒▒░░░░░░░░▓▓████▓▓░░░░░░████████░░░░░░▓▓████▓▓░░░░░░██
//       ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░██░░░░██░░░░░░░░░░░░░░░░░░░░██
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//       ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ░░▓▓▒▒░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░father░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░i hunger░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//   ░░▓▓▓▓░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██░░
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//      ▓▓████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//        ▓▓▓▓████████░░░░░░░░░░░░░░░░░░░░░░░░████████░░
//        ░░░░▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░

#endregion MyChild
