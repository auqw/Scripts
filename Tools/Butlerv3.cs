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
    public string OptionsStorage = "Butler";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "playerName",
            "Player Name",
            "Insert the name of the player to follow Capitals and punctuation are required.",
            ""
        ),
        CoreBots.Instance.SkipOptions,
        new Option<string>(
            "lockedMapsList",
            "Custom Locked Maps",
            "Fill in the Maps that the bot will check (in order), if the player is not in the current map, split with a , (comma)."
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
    string RN;
    List<string> lockedMapList;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        BasicAFButler();
        Core.SetOptions(false);
    }

    public void BasicAFButler()
    {
        Bot.Events.ExtensionPacketReceived += LockedZoneListener;

        LockedZoneWarning = false;
        string? lockedMapsRaw = Bot.Config?.Get<string>("lockedMapsList") ?? string.Empty;
        lockedMapList = lockedMapsRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(m => m.Trim())
            .ToList();
        playerName = Bot.Config!.Get<string>("playerName");
        classType = Bot.Config!.Get<ClassType>("classType");
        RN = Bot.Config!.Get<string>("RoomNumber") ?? Core.PrivateRoomNumber.ToString();

        if (string.IsNullOrEmpty(playerName))
        {
            Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
            return;
        }

        if (classType != ClassType.None)
            Core.EquipClass(classType);

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (
                Bot.Player.Alive
                && Bot.Map.PlayerNames != null
                && Bot.Map.PlayerNames.Contains(playerName)
            )
            {
                if (
                    Bot.Map.TryGetPlayer(playerName, out PlayerInfo? targetPlayer)
                    && targetPlayer != null
                )
                {
                    if (targetPlayer.Cell != Bot.Player.Cell)
                    {
                        Bot.Player.Goto(playerName);
                        Bot.Sleep(1000);
                    }
                    else
                    {
                        if (!Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.HP > 0))
                            Bot.Sleep(500);
                        else
                        {
                            Bot.Combat.Attack("*");
                        }
                        Bot.Sleep(500);
                    }
                }
            }

            if (Bot.Map.PlayerNames == null || !Bot.Map.PlayerNames.Contains(playerName))
            {
                if (LockedZoneWarning)
                {
                    Core.JumpWait();
                    LockedZoneWarning = false;
                    foreach (string map in lockedMapList)
                    {
                        if (Bot.ShouldExit)
                        {
                            Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
                            return;
                        }

                        Core.Join($"{map}-{RN}");
                        Bot.Wait.ForMapLoad(map);

                        if (Bot.Map.PlayerNames!.Any(x => x != null && x == playerName))
                        {
                            Bot.Player.Goto(playerName);
                            Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
                            break;
                        }
                    }
                }

                Core.JumpWait();
                Bot.Sleep(1000);
                Bot.Player.Goto(playerName);
                Bot.Sleep(1000);
            }
        }

        Bot.Events.ExtensionPacketReceived -= LockedZoneListener;
        Core.JumpWait();
    }

    bool PlayerInMap => Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Contains(playerName!);

    void LockedZoneListener(dynamic packet)
    {
        string? type = packet["params"].type;

        if (type is "str")
        {
            dynamic data = packet["params"].dataObj;
            string cmd = data[0];

            if (cmd == null || data == null)
                return;

            if (cmd is "warning")
            {
                string lockerZonePacket = Convert.ToString(packet);
                if (
                    lockerZonePacket.Contains("a Locked zone.")
                    || lockerZonePacket.Contains("is not available.")
                )
                    LockedZoneWarning = true;
            }
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
