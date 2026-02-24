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
            "InsertPlayerNamehere"
        ),
        CoreBots.Instance.SkipOptions,
        new Option<string>(
            "lockedMapsList",
            "Custom Locked Maps",
            "Fill in the Maps that the bot will check (in order), if the player is not in the current map, split with a , (comma) ( example: Locked,maps,seperated,by,a,comma).",
            ""
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
    bool GotoIsOff;
    string? playerName;
    ClassType classType;
    string? RN;
    List<string?> lockedMapList;

    // String? TargetCell;
    bool RoomFull;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        BasicAFButler();
        Bot.Events.ExtensionPacketReceived -= ChatListener;
        Core.SetOptions(false);
    }

    public void BasicAFButler()
    {
        #region Ignore
        // Core.DL_Enable();
        Core.Logger(
            "Joining whitemap, as starting on certain maps.. just breaks things? Not sure why :| "
        );
        Core.Join("whitemap-100000");
        Bot.Events.ExtensionPacketReceived += ChatListener;
        LockedZoneWarning = false;
        // Null-safe config reads
        var lockedMapsConfig = Bot.Config!.Get<string>("lockedMapsList");
        lockedMapList =
            lockedMapsConfig?.Split(',').Select(m => m?.Trim()).ToList() ?? new List<string?>();

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
        {
            Core.Logger(
                "THE FUCK ARE YOU FOLLOWING YOURSELF FOR RETARD?",
                "Retard alert",
                messageBox: true
            );
        }

        while (!Bot.ShouldExit)
        {
            try
            {
                while (!Bot.ShouldExit)
                {
                    if (!Bot.Player!.Alive)
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                    Bot.Player!.Goto(playerName);

                    #region Server Warnings
                    if (GotoIsOff)
                    {
                        LockedZoneWarning = false;
                        GotoIsOff = false;
                        break;
                    }
                    if (LockedZoneWarning)
                    {
                        Bot.Events.ExtensionPacketReceived -= ChatListener;
                        Core.JumpWait();
                        Core.Join("whitemap-100000");
                        LockedZoneWarning = false;

                        if (lockedMapList.Count > 0)
                        {
                            Core.Logger("LockedMaps handler Initiated.", "LockedMapList.Count > 0");
                            foreach (string? map in lockedMapList.Where(m => m != null))
                            {
                                if (Bot.ShouldExit)
                                {
                                    Bot.Events.ExtensionPacketReceived -= ChatListener;
                                    return;
                                }

                                Core.Join($"{map}-{RN}");
                                Bot.Wait.ForMapLoad(map!);

                                if (Bot.Map.PlayerExists(playerName))
                                {
                                    Bot.Events.ExtensionPacketReceived += ChatListener;
                                    Bot.Player?.Goto(playerName);
                                    break;
                                }
                            }
                            Bot.Events.ExtensionPacketReceived += ChatListener;
                            continue;
                        }
                        else if (lockedMapList.Count <= 0)
                        {
                            Core.Logger(
                                "LockedMap list is Empty, we'll Sleep incrimentaly",
                                "lockedMapList <= 0"
                            );
                            Core.Join("whitemap-100000");
                            Random random = new();
                            int sleepTimer = 500; // Start at 500ms
                            const int maxSleep = 10000; // 10 seconds max
                            const int increment = 1000; // 500ms random increment

                            while (!Bot.ShouldExit && !Bot.Map.PlayerExists(playerName))
                            {
                                Core.Logger($"Sleeping for: {sleepTimer}");
                                Bot.Sleep(sleepTimer);

                                Bot.Player!.Goto(playerName);
                                // Increment sleep time if under 5 seconds
                                if (sleepTimer < maxSleep)
                                {
                                    sleepTimer += random.Next(increment); // Add 0-500ms randomly
                                    sleepTimer = Math.Min(sleepTimer, maxSleep); // Cap at 5 seconds
                                }

                                if (Bot.Map.PlayerExists(playerName))
                                {
                                    Bot.Log($"{playerName} Found!");
                                    Bot.Events.ExtensionPacketReceived += ChatListener;
                                    break;
                                }
                            }
                            Bot.Events.ExtensionPacketReceived += ChatListener;
                            continue;
                        }
                    }
                    if (RoomFull)
                    {
                        Bot.Events.ExtensionPacketReceived -= ChatListener;
                        Random random = new();
                        int sleepTimer = 1000; // Start at 500ms
                        const int maxSleep = 5000; // 10 seconds max
                        const int increment = 1000; // 500ms random increment

                        while (!Bot.ShouldExit && !Bot.Map.PlayerExists(playerName!))
                        {
                            Bot.Sleep(sleepTimer);

                            // Increment sleep time if under 5 seconds
                            if (sleepTimer < maxSleep)
                            {
                                sleepTimer += random.Next(increment); // Add 0-500ms randomly
                                sleepTimer = Math.Min(sleepTimer, maxSleep); // Cap at 5 seconds
                            }
                            Bot.Player!.Goto(playerName!);
                            if (Bot.Map.PlayerExists(playerName!))
                            {
                                Bot.Events.ExtensionPacketReceived += ChatListener;
                                RoomFull = false;
                                break;
                            }
                            if (RoomFull && sleepTimer >= maxSleep)
                                Bot.Log(
                                    "Room is still full, we'll continue waiting for the map to be accessable, or till we can goto the player."
                                );
                        }
                        continue;
                    }
                    #endregion

                    if (!Bot.Map.PlayerExists(playerName))
                    {
                        Core.Logger($"{playerName} isn't on the current map, following!");
                        Core.JumpWait();
                        continue;
                    }

                    Bot.Sleep(500);
                    Bot.Combat.Attack("*");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Bot.Log($"An error occurred: {ex.Message}");
            }
        }

        Bot.Events.ExtensionPacketReceived -= ChatListener;
        Core.JumpWait();
    }

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

            // ------------------------------
            // SERVER MESSAGES (%xt%server ...)
            // ------------------------------
            if (cmd == "server")
            {
                string? text = dataObj[2]?.ToString();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                // Detect "is ignoring goto requests" on server channel
                if (text.Contains("is ignoring goto requests", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"{playerName} is ignoring goto requests (disable `incognito mode` in the CBO for that account), Stopping script!", "Goto is follow for the person you're following");
                    GotoIsOff = true;

                    return;
                }
            }

            // ------------------------------
            // WARNING MESSAGES (existing)
            // ------------------------------
            if (cmd == "warning")
            {
                string chatPacket = Convert.ToString(packet) ?? string.Empty;

                if (
                    chatPacket.Contains("a Locked zone.", StringComparison.OrdinalIgnoreCase)
                    || chatPacket.Contains("is not available.", StringComparison.OrdinalIgnoreCase)
                )
                    LockedZoneWarning = true;

                if (chatPacket.Contains("is full", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"Room is full, we'll wait incrementally, whilst trying to goto {playerName}");
                    RoomFull = true;
                }

                if (chatPacket.Contains("ignoring goto", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Logger($"{playerName} is ignoring goto requests (disable `incognito mode` in the CBO for that account), Stopping script!", "Goto is follow for the person you're following");
                    GotoIsOff = true;
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
