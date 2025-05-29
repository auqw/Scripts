/*
name: null
description: null
tags: null
*/
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Servers;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using Skua.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using Skua.Core.Models.Auras;
using System.Windows.Forms;
using System.Drawing;

public class CoreBots
{
    #region Declerations
    // [Can Change] Delay between common actions, 700 is the safe number
    public int ActionDelay { get; set; } = 700;
    // [Can Change] Delay used to get out of combat, 1600 is the safe number
    public int ExitCombatDelay { get; set; } = 1600;
    // [Can Change] Delay between jumping rooms after hunting a monster, increase if you think it is jumping too much
    public int HuntDelay { get; set; } = 1000;
    // [Can Change] How many tries to accept/complete the quest will be sent
    public int AcceptandCompleteTries { get; set; } = 20;
    // [Can Change] How many quests the bot should be able to have loaded at once
    public int LoadedQuestLimit { get; set; } = 150;
    // [Can Change] Whether the bots should also log in AQW's chat
    public bool LoggerInChat { get; set; } = true;
    // [Can Change] When enabled, no message boxes will be shown unless absolutely necessary
    public bool ForceOffMessageboxes { get; set; } = false;
    // [Can Change] Whether the bots will use private rooms
    public bool PrivateRooms { get; set; } = true;
    // [Can Change] What private room number the bot should use, if > 99999 it will pick a random room
    public int PrivateRoomNumber { get; set; } = 100000;
    // [Can Change] Use public rooms if the enemy is tough
    public bool PublicDifficult { get; set; } = false;
    // [Can Change] If StopLocations.Custom is selected, where to go
    public string CustomStopLocation { get; set; } = "whitemap";
    // [Can Change] Whether the player should rest after killing a monster
    public bool ShouldRest { get; set; } = false;
    // [Can Change] Whether the bot should attempt to clean your inventory by banking Misc. AC Items before starting the bot
    public bool BankMiscAC { get; set; } = false;
    public bool BankUnenhancedACGear { get; set; } = false;
    // [Can Change] Whether you want anti lag features (lag killer, invisible monsters, set to 10 FPS)
    public bool AntiLag { get; set; } = true;
    // [Can Change] Name of your soloing class
    public string SoloClass { get; set; } = "Generic";
    // [Can Change] Mode of soloing class, if it has multiple. 
    public ClassUseMode SoloUseMode { get; set; } = ClassUseMode.Base;
    // [Can Change] Whether you wish to equip solo equipment
    public bool SoloGearOn { get; set; } = true;
    // [Can Change] Names of your soloing equipment
    public string[] SoloGear { get; set; } = Array.Empty<string>();
    // [Can Change] Name of your farming class
    public string FarmClass { get; set; } = "Generic";
    // [Can Change] Mode of farming class, if it has multiple. 
    public ClassUseMode FarmUseMode { get; set; } = ClassUseMode.Base;
    // [Can Change] Whether you wish to equip farm equipment
    public bool FarmGearOn { get; set; } = true;
    // [Can Change] Names of your farming equipment
    public string[] FarmGear { get; set; } = Array.Empty<string>();
    // [Can Change] Some Sagas use the hero alignment to give extra reputation, change to your desired rep (Alignment.Evil or Alignment.Good).
    public int HeroAlignment { get; set; } = (int)Alignment.Evil;
    // [Can Change] Member Status
    public bool IsMember { get; set; }

    // Thousand-level Constants
    const int OneK = 1000;        // 1k
    const int TenK = 10000;       // 10k
    const int OneHundredK = 100000; // 100k
    const int FiveHundredK = 500000; // 500k

    // Million-level Constants
    const int OneMillion = 1000000;   // 1m
    const int FiveMillion = 5000000;  // 5m
    const int TenMillion = 10000000;  // 10m
    const int FiftyMillion = 50000000; // 50m
    const int OneHundredMillion = 100000000; // 100m

    //Max integer
    const int maxint = Int32.MaxValue;

    private static CoreBots? _instance;
    public static CoreBots Instance => _instance ??= new CoreBots();
    private IScriptInterface Bot => IScriptInterface.Instance;

    private const string DiscordLink = "https://discord.gg/CKKbk2zr3p";


    private Stopwatch? _scriptStopwatch;

    #endregion

    #region Start/Stop

    /// <summary>
    /// Set common bot options to desired value
    /// </summary>
    /// <param name="changeTo">Value the options will be changed to</param>
    /// <param name="disableClassSwap"></param>
    public void SetOptions(bool changeTo = true, bool disableClassSwap = false)
    {
        // These things need to be set and checked before anything else
        if (changeTo)
        {
            Bot.Events.ScriptStopping += CrashDetector;
            SkuaVersionChecker("1.2.4.0");

            // Start the stopwatch for timing the script run
            _scriptStopwatch = Stopwatch.StartNew();
            DeleteCompiledScript();

            loadedBot = Bot.Manager.LoadedScript.Replace("\\", "/").Split("/Scripts/").Last().Replace(".cs", "");
            Logger($"Bot Started [{loadedBot}]");
            if (Bot.Config != null && Bot.Config.Options.Contains(SkipOptions) && !Bot.Config.Get<bool>(SkipOptions))
                Bot.Config.Configure();

            if (!Bot.Player.LoggedIn)
            {
                if (Bot.Servers.CachedServers.Any())
                {
                    Logger("Auto Login triggered");
                    try
                    {
                        if (!Bot.Servers.EnsureRelogin(Bot.Options.ReloginServer ?? Bot.Servers.CachedServers[0]?.Name ?? "Twilly"))
                            Logger("Please log-in before starting the bot.\nIf you are already logged in but are receiving this message regardless, please re-install CleanFlash", messageBox: true, stopBot: true);
                        Sleep(5000);
                    }
                    catch
                    {
                        Logger("Please log-in before starting the bot.\nIf you are already logged in but are receiving this message regardless, please re-install CleanFlash", messageBox: true, stopBot: true);
                    }
                }
                else Logger("Please log-in before starting the bot.\nIf you are already logged in but are receiving this message regardless, please re-install CleanFlash", messageBox: true, stopBot: true);
            }
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
            ReadCBO();
        }


        // Set the member status
        IsMember = isUpgraded();

        // Common Options
        Bot.Options.PrivateRooms = false;
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.SafeTimings = changeTo;
        Bot.Options.RestPackets = changeTo && ShouldRest;
        Bot.Options.AutoRelogin = true;
        Bot.Options.InfiniteRange = changeTo;
        Bot.Options.SkipCutscenes = changeTo;
        Bot.Options.QuestAcceptAndCompleteTries = AcceptandCompleteTries;
        Bot.Drops.RejectElse = changeTo;
        Bot.Lite.UntargetDead = changeTo;
        Bot.Lite.UntargetSelf = changeTo;
        Bot.Lite.ReacceptQuest = false;
        Bot.Lite.DisableRedWarning = true;
        Bot.Lite.CharacterSelectScreen = false;

        //adding sommore
        Bot.Lite.SmoothBackground = true;
        Bot.Lite.ShowMonsterType = true;
        Bot.Lite.CustomDropsUI = true;

        CollectData(changeTo);

        #region Required things that must be done before starting the Script
        if (changeTo)
        {
            //Start scripts Safely by starting them in the house ( or whitemap if house desnt exist) if the start map is battleon 
            if (new[] { "battleon", "oaklore", "bludrutbrawl" }.Any(m => Bot.Map.Name.Equals(m, StringComparison.OrdinalIgnoreCase)))
            {
                if (Bot.House.Items.Any(h => h.Equipped))
                {
                    string? toSend = null;
                    Bot.Events.ExtensionPacketReceived += modifyPacket;
                    Bot.Send.Packet($"%xt%zm%house%1%{Username()}%");
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
                            toSend = $"{{\"t\":\"xt\",\"b\":{{\"r\":-1,\"o\":{{\"cmd\":\"moveToArea\",\"areaName\":\"house\",\"uoBranch\":{JsonConvert.SerializeObject(data.uoBranch)},\"strMapFileName\":\"{data.strMapFileName}\",\"intType\":\"1\",\"monBranch\":[],\"houseData\":{Regex.Replace(JsonConvert.SerializeObject(data.houseData), Username(), "Skua user", RegexOptions.IgnoreCase)},\"sExtra\":\"\",\"areaId\":{data.areaId},\"strMapName\":\"house\"}}}}}}";
                            Bot.Events.ExtensionPacketReceived -= modifyPacket;
                        }
                    }
                }
                else Bot.Send.Packet($"%xt%zm%cmd%1%tfer%{Username()}%whitemap-{PrivateRoomNumber}%");
            }

            // Open Bank on startup ensuring current window is `Bank`, then load the bank information.
            if (Bot.Flash.GetGameObject("ui.mcPopup.currentLabel") != "\"Bank\"")
                Bot.Bank.Open();
            Bot.Bank.Load();
            Bot.Bank.Loaded = true;

            #region Social Privacy Options
            bool warned = false;

            Dictionary<string, string> socialOptionsToDisable = new()
                {
                    { "bGoto", "Goto" },
                    { "bParty", "Party invites" },
                    { "bFriend", "Friend invites" },
                    { "bDuel", "Duel invites" },
                    { "bGuild", "Guild invites" },
                    { "bWhisper", "Whisper" },
                    { "bProf", "Censor Profanity" }
                };

            foreach (KeyValuePair<string, string> entry in socialOptionsToDisable)
            {
                string key = entry.Key;
                string label = entry.Value;

                bool value = Bot.Flash.GetGameObject<bool>($"uoPref.{key}");
                if (value)
                {
                    if (!warned)
                    {
                        Logger("Turning certain \"Social\" options off to help protect you");
                        warned = true;
                    }
                    if (label == "Goto" && !loadedBot.ToLower().Contains("butler"))
                        continue;

                    Logger($"Turning off: {label}");
                    SendPackets($"%xt%zm%cmd%1%uopref%{key}%false%");
                    Bot.Sleep(500);
                }
            }
            GC.Collect();
            #endregion Social Privacy Options         
        }


        #endregion Required things that must be done before starting the Script

        // These things need to be taken care of too, but less priority
        if (changeTo)
        {
            SetOptionsAsync();

            Bot.Options.HuntDelay = HuntDelay;

            if (BankMiscAC)
                BankACMisc();

            if (BankUnenhancedACGear)
                BankACUnenhancedGear();

            EquipmentBeforeBot.AddRange(Bot.Inventory.Items.Where(i => i.Equipped).Select(x => x.Name));
            currentClass = ClassType.None;
            usingSoloGeneric = SoloClass.ToLower() == "generic";
            usingFarmGeneric = FarmClass.ToLower() == "generic";
            EquipClass(disableClassSwap ? ClassType.None : ClassType.Solo);

            Bot.Events.ScriptStopping += StopBotEvent;

            // Alive Check handling
            Bot.Events.MapChanged += CleanKilledMonstersList;
            Bot.Events.MonsterKilled += KilledMonsterListener;
            Bot.Events.ExtensionPacketReceived += RespawnListener;

            Bot.Drops.Start();
            Logger("Bot Configured");

            // Bunch of things that are done in the background and you dont need the bot to wait for 
            void SetOptionsAsync()
            {
                #region Handlers
                Task.Run(() =>
                {
                    Task.Run(() =>
                    {
                        if (OneTimeMessage("discordV11",
                                "Our discord server was recently deleted again (March 29th 2023), click yes if you wish to (re-)join the server",
                                true, true, true))
                            Process.Start("explorer", DiscordLink);
                    });

                    // Butler directory cleaning
                    if (Directory.Exists(ButlerLogDir))
                    {
                        if (File.Exists(ButlerLogPath()))
                            File.Delete(ButlerLogPath());

                        string[] files = Directory.GetFiles(ButlerLogDir);
                        if (files.Any(x => x.Contains("~!") && x.Split("~!").First() == Username().ToLower()))
                            File.Delete(files.First(x => x.Contains("~!") && x.Split("~!").First() == Username().ToLower()));
                    }


                    // AFK Handler
                    Bot.Send.Packet("%xt%zm%afk%1%false%");
                    Sleep();
                    bool TimerRunning = false;
                    //int afkCount = 0;
                    //Bot.Events.PlayerAFK += eventAFK;

                    //void eventAFK()
                    //{
                    //    afkCount++;
                    //    int localCount = afkCount;
                    //    Sleep(300000);
                    //    if (Bot.Player.AFK && afkCount == localCount)
                    //    {
                    //        Bot.Options.AutoRelogin = true;
                    //        Bot.Servers.Logout();
                    //    }
                    //}
                    Bot.Handlers.RegisterHandler(5000, b =>
                    {
                        if (b.Player.AFK && !TimerRunning)
                        {
                            TimerRunning = true;
                            Sleep(300000);
                            if (b.Player.AFK)
                            {
                                b.Options.AutoRelogin = true;
                                b.Servers.Logout();
                            }
                            TimerRunning = false;
                        }
                    }, "AFK Handler");

                    // Settin Loaded Quest Limiter
                    Bot.Handlers.RegisterHandler(3000, b =>
                    {
                        if (Bot.Quests.Tree.Count > LoadedQuestLimit)
                        {
                            Bot.Flash.SetGameObject("world.questTree", new ExpandoObject());
                        }
                    }, "Quest-Limit Handler");

                    // Prison Detector
                    if (loadedBot.Replace("\\", "/") != "Tools/Butler")
                    {
                        Bot.Events.MapChanged += PrisonDetector;
                        void PrisonDetector(string map)
                        {
                            if (map.ToLower() == "prison" && !joinedPrison && !prisonListernerActive)
                            {
                                prisonListernerActive = true;
                                Bot.Options.AutoRelogin = false;
                                Bot.Servers.Logout();
                                string message = "You were teleported to /prison by someone other than the bot. We disconnected you and stopped the bot out of precaution.\n" +
                                                 "Be ware that you might have received a ban, as this is a method moderators use to see if you're botting." +
                                                 (!PrivateRooms || PrivateRoomNumber < 1000 || PublicDifficult ? "\nGuess you should have stayed out of public rooms!" : string.Empty);
                                Logger(message);
                                Bot.ShowMessageBox(message, "Unauthorized joining of /prison detected!", "Oh fuck!");
                                Bot.Stop(true);
                            }
                        }
                    }

                    #endregion Handlers

                    // Anti-lag option
                    if (AntiLag)
                    {
                        Bot.Options.LagKiller = changeTo;
                        Bot.Flash.SetGameObject("stage.frameRate", 10);
                        if (!Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
                            Bot.Flash.CallGameFunction("world.toggleMonsters");
                    }

                    // Identity Protection
                    // Bot.Options.CustomName = "SkuaLabRat";
                    // Bot.Options.CustomGuild = "Skua-side Squad";

                    // Holiday Handlers
                    AprilFools();

                    //Fucking with specific people
                    UserSpecificMessages();
                });
            }
        }
        if (!changeTo && _scriptStopwatch != null)
        {
            _scriptStopwatch.Stop();
            Logger($"Script ran for {_scriptStopwatch.Elapsed:hh\\:mm\\:ss}");
            _scriptStopwatch = null;
        }
    }

    // Whether the player is a Member (set to true if necessary during setOptions)
    public bool isUpgraded()
    {
        // Get membership days left as a string
        string? membershipDaysLeftString = Bot.Flash.GetGameObject("world.myAvatar.objData.iUpgDays");

        // Attempt to parse the string into an integer
        if (int.TryParse(membershipDaysLeftString, out int membershipDaysLeft))
        {
            // Return true if membership days are greater than 0
            return membershipDaysLeft > 0;
        }

        // If parsing fails, return false (not a member)
        return false;
    }



    public List<string> BankingBlackList = new();
    private readonly List<string> EquipmentBeforeBot = new();
    private bool joinedPrison = false;
    private bool prisonListernerActive = false;
    public string loadedBot = string.Empty;

    /// <summary>
    /// Stops the bot and moves you back to /Battleon
    /// </summary>
    private bool StopBot(bool crashed)
    {
        StopBotAsync();
        Bot.Handlers.Clear();

        if (Bot.Player.LoggedIn)
        {
            JumpWait();
            Sleep();

            if (!string.IsNullOrWhiteSpace(CustomStopLocation))
            {
                string _stopLoc = CustomStopLocation.Trim().ToLower();
                if (new[] { "home", "house" }.Contains(_stopLoc))
                {
                    if (Bot.House.Items.Any(h => h.Equipped))
                    {
                        string? toSend = null;
                        Bot.Events.ExtensionPacketReceived += modifyPacket;
                        Bot.Send.Packet($"%xt%zm%house%1%{Username()}%");
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
                                toSend = $"{{\"t\":\"xt\",\"b\":{{\"r\":-1,\"o\":{{\"cmd\":\"moveToArea\",\"areaName\":\"house\",\"uoBranch\":{JsonConvert.SerializeObject(data.uoBranch)},\"strMapFileName\":\"{data.strMapFileName}\",\"intType\":\"1\",\"monBranch\":[],\"houseData\":{Regex.Replace(JsonConvert.SerializeObject(data.houseData), Username(), "Skua user", RegexOptions.IgnoreCase)},\"sExtra\":\"\",\"areaId\":{data.areaId},\"strMapName\":\"house\"}}}}}}";
                                Bot.Events.ExtensionPacketReceived -= modifyPacket;
                            }
                        }
                    }
                    else Bot.Send.Packet($"%xt%zm%cmd%1%tfer%{Username()}%whitemap-{PrivateRoomNumber}%");
                }
                else if (new[] { "off", "disabled", "disable", "stop", "same", "currentmap", "bot.map.currentmap", "none", "None", string.Empty }
                    .Any(m => m == _stopLoc))
                {
                    // Nothing happens
                }
                else Bot.Send.Packet($"%xt%zm%cmd%1%tfer%{Username()}%{_stopLoc}-{PrivateRoomNumber}%");

                if (EquipmentBeforeBot.Any())
                {
                    JumpWait();
                    Equip(EquipmentBeforeBot.ToArray());
                }
            }
        }

        if (crashed)
            Logger("Bot stopped due to a crash.");
        else if (!Bot.Player.LoggedIn)
        {
            if (Bot.Options.AutoRelogin)
            {
                Task.Run(async () =>
                {
                    await Bot.Manager.RestartScriptAsync();
                    if (Bot.Player.LoggedIn)
                        return;
                    Logger("Bot stopped due to Auto-Relogin failure.");
                });
            }
            else Logger("Bot stopped due to player logout.");
        }
        else Logger("Bot stopped successfully.");

        GC.KeepAlive(Instance);
        return scriptFinished;

        void StopBotAsync()
        {
            Task.Run(() =>
            {
                SavedState(false);

                if (AntiLag)
                {
                    Bot.Options.SetFPS = 60;
                    if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
                        Bot.Flash.CallGameFunction("world.toggleMonsters");
                }

                Bot.Options.CustomName = Bot.Player.Username ?? Username().ToUpper();
                // Bot.Options.CustomName = Username().ToUpper();
                string? guild = Bot.Flash.GetGameObject<string>("world.myAvatar.objData.guild.Name");
                Bot.Options.CustomGuild = guild != null ? $"< {guild} >" : string.Empty;

                if (File.Exists(ButlerLogPath()))
                    File.Delete(ButlerLogPath());
            });
        }
    }
    private bool scriptFinished = true;

    public bool StopBotEvent(Exception? e)
    {
        SetOptions(false);
        return StopBot(e != null);
    }

    public bool CrashDetector(Exception? e)
    {
        if (e == null || e is OperationCanceledException)
            return scriptFinished;

        string eSlice = e.Message + "\n" + e.InnerException;
        List<string> logs = Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script);
        logs = logs.Skip(logs.Count > 5 ? (logs.Count - 5) : logs.Count).ToList();
        if (Bot.ShowMessageBox("A crash has been detected, please fill in the report form (prefilled):\n\n" + eSlice,
                               "Script Crashed", "Open Form", "Close Window").Text == "Open Form")
        {
            string url = "\"https://docs.google.com/forms/d/e/1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg/viewform?usp=pp_url&" +
                "entry.2118425091=Bug+Report&" +
               $"entry.290078150={Bot.Manager.LoadedScript.Split("Scripts").Last().Replace('/', '\\')[1..].Replace(".cs", "")}&" +
                "entry.1803231651=It+stopped+at+the+wrong+time+(crash)&" +
               $"entry.1954840906={logs.Join("%0A")}&" +
               $"entry.285894207={eSlice}&\"";
            url = url.Replace("\r\n", "%0A").Replace("\n", "").Replace(" ", "%20");

            Process p = new();
            p.StartInfo.FileName = "rundll32";
            p.StartInfo.Arguments = "url,OpenURL " + url;
            p.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System).Split('\\').First() + "\\";
            p.Start();

            Logger("Thank you for reporting the crash. Below you will find the information you will need to report, in case it isn't being auto filled");

        }
        else Logger("A crash has occurred. Please report it in the form with the details below");

        Bot.Log("--------------------------------------");
        Logger("Last 5 Logs:");
        Bot.Log(logs.Join('\n'));
        Bot.Log("--------------------------------------");
        Logger("Crash (Debug)");
        Bot.Log(eSlice);
        Bot.Log("--------------------------------------");

        return false;
    }

    public List<string> GetLogs(LogType type = LogType.Script)
        => (_logService ??= Ioc.Default.GetRequiredService<ILogService>()).GetLogs(type);
    private ILogService? _logService;

    public void ScriptMain(IScriptInterface Bot)
    {
        RunCore();
    }

    #endregion

    #region Inventory, Bank and Shop
#nullable enable

    /// <summary>
    /// Check the Bank, Inventory and Temp Inventory for the item
    /// </summary>
    /// <param name="item">Name of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="toInv">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether the item exists in the desired quantity in the bank and inventory</returns>
    public bool CheckInventory(string? item, int quant = 1, bool toInv = true)
    {
        if (item == null)
            return true;

        if (Bot.TempInv.Contains(item, quant))
            return true;

        if (Bot.Inventory.Contains(item, quant))
            return true;

        if (Bot.House.Contains(item))
            return true;

        if (Bot.Bank.Contains(item))
        {
            if (toInv)
                Unbank(item);

            if ((toInv && Bot.Inventory.GetQuantity(item) >= quant) ||
               (!toInv && Bot.Bank.TryGetItem(item, out InventoryItem? _item) && _item != null && _item.Quantity >= quant))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks the Bank and Inventory for the item with it's ID
    /// </summary>
    /// <param name="itemID">ID of the item to be checked</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="toInv">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether the item exists in the desired quantity in the Bank and Inventory</returns>
    public bool CheckInventory(int? itemID, int quant = 1, bool toInv = true)
    {
        if (itemID == null)
            return true;
        int _itemID = (int)itemID;

        if (Bot.TempInv.Contains(_itemID, quant))
            return true;

        if (Bot.Inventory.Contains(_itemID, quant))
            return true;

        if (Bot.House.Contains(_itemID))
            return true;

        if (Bot.Bank.Contains(_itemID))
        {
            if (toInv)
                Unbank(_itemID);

            if ((toInv && Bot.Inventory.GetQuantity(_itemID) >= quant) ||
               (!toInv && Bot.Bank.TryGetItem(_itemID, out InventoryItem? _item) && _item != null && _item.Quantity >= quant))
                return true;
        }


        return false;
    }

    /// <summary>
    /// Check if the Bank/Inventory has at least 1 of all listed items
    /// </summary>
    /// <param name="itemNames">Array of names of the items to be checked</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="any">If any of the items exist, returns true</param>
    /// <param name="toInv">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether all the items exist in the Bank or Inventory</returns>
    public bool CheckInventory(string[]? itemNames, int quant = 1, bool any = false, bool toInv = true)
    {
        if (itemNames == null || !itemNames.Any())
            return true;

        foreach (string name in itemNames)
        {
            if (CheckInventory(name, quant, toInv))
            {
                if (any)
                    return true;
                else
                    continue;
            }

            if (!any)
                return false;
        }

        return !any;
    }

    /// <summary>
    /// Checks the Bank and Inventory for the item with it's ID
    /// </summary>
    /// <param name="itemIDs">Array of IDs of the items to be checked</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="any">If any of the items exist, returns true</param>
    /// <param name="toInv">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether the item exists in the desired quantity in the Bank and Inventory</returns>
    public bool CheckInventory(int[]? itemIDs, int quant = 1, bool any = false, bool toInv = true)
    {
        if (itemIDs == null || !itemIDs.Any())
            return true;

        foreach (int id in itemIDs)
        {
            if (CheckInventory(id, quant, toInv))
            {
                if (any)
                    return true;
                else
                    continue;
            }

            if (!any)
                return false;
        }

        return !any;
    }

    /// <summary>
    /// Attempts to initialize an object using the provided initializer function, retrying up to a specified number of times if the initialization fails.
    /// </summary>
    /// <typeparam name="T">The type of the object to be initialized. This must be a class type.</typeparam>
    /// <param name="initializer">
    /// A function that attempts to initialize the object and returns the initialized object or null if the initialization fails.
    /// </param>
    /// <param name="retries">
    /// The number of times to retry the initialization if it fails. The default value is 5 retries.
    /// </param>
    /// <param name="delay">
    /// The delay in milliseconds between retry attempts. The default value is 1000 milliseconds (1 second).
    /// </param>
    /// <returns>
    /// Returns the initialized object of type <typeparamref name="T"/> if the initialization succeeds within the specified retries, 
    /// otherwise returns null after exhausting all retry attempts.
    /// </returns>
    /// <remarks>
    /// This method provides a way to retry an initialization operation, which can be useful when dealing with operations that might fail intermittently.
    /// It logs each retry attempt and will notify if all attempts fail.
    /// </remarks>
    public T? InitializeWithRetries<T>(Func<T?> initializer, int retries = 5, int delay = 1000) where T : class
    {
        T? result = null;
        for (int i = 0; i < retries; i++)
        {
            result = initializer();
            if (result != null)
                break;
            Logger($"Attempt {i++}: Initialization failed. Retrying...");
            Sleep(delay); // Wait for the specified delay before retrying
        }

        if (result == null)
        {
            Logger($"Initialization failed after {retries} attempts at: {initializer}.");
        }

        return result;
    }

    /// <summary>
    /// Checks if there is enough space in the inventory for specified items and logs a message if space is insufficient.
    /// </summary>
    /// <param name="counter">Reference to a counter variable tracking successful checks.</param>
    /// <param name="items">Items to check for space in the inventory.</param>
    public void CheckSpaces(ref int counter, params string[] items)
    {
        foreach (string item in items)
            if (CheckInventory(item, toInv: false))
                counter++;

        int requiredSlots = items.Length - counter;
        if (Bot.Inventory.FreeSlots < requiredSlots)
            Logger($"Not enough free slot{(requiredSlots != 1 ? "s" : "")}, please clear {requiredSlots} slot{(requiredSlots != 1 ? "s" : "")}", messageBox: true, stopBot: true);
    }

    /// <summary>
    /// Moves specified items by their names from the inventory to the bank.
    /// </summary>
    /// <param name="items">
    /// Array of item names to transfer to the bank. Items will be skipped if
    /// they are equipped, in use, or not present in the inventory. Skips blacklisted
    /// or non-bankable categories.
    /// </param>
    /// <remarks>
    /// Only whitelisted item categories are moved, and house items are handled
    /// separately from inventory items. Attempts each move up to 5 times for
    /// reliability and logs each success or failure.
    /// </remarks>
    public void Unbank(params string[] items)
    {
        if (items == null || items.Length == 0)
        {
            return;
        }

        if (Bot.Player.InCombat)
        {
            JumpWait();
        }

        int RequiredSpaces = items.Count();
        foreach (string item in items)
        {
            if (Bot.House.Contains(item) || Bot.Inventory.Contains(item)
                || (!Bot.Inventory.Contains(item) && !Bot.House.Contains(item) && !Bot.Bank.Contains(item)))
            {
                RequiredSpaces--;
                continue; // Skip the item if it's in house or bank, or nowhere (not in any of the 3 places)
            }

            if (Bot.Bank.Contains(item) && (!Bot.Inventory.Contains(item) || !Bot.House.Contains(item)))
            {
                if (Bot.Inventory.FreeSlots <= 0 && Bot.Inventory.Slots != 0 && Bot.Inventory.UsedSlots >= Bot.Inventory.Slots)
                {
                    Logger($"Your inventory is full ({Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}), please Make {RequiredSpaces} space(s), and restart the bot", messageBox: true, stopBot: true);
                    return;
                }

                bool isHouseItem = Bot.Bank.TryGetItem(item, out InventoryItem? x) &&
                                  x != null &&
                                  (x.CategoryString == "House" || x.CategoryString == "Wall Item" || x.CategoryString == "Floor Item");

                if (isHouseItem)
                {
                    bool success = false;
                    for (int i = 0; i < 20; i++) // Retry up to 20 times
                    {
                        SendPackets($"%xt%zm%bankToInv%{Bot.Map.RoomID}%{x!.ID}%{x.CharItemID}%");
                        Sleep(); // Wait for a short period before checking
                        if (Bot.House.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        Logger($"Failed to unbank {item}, skipping it");
                        continue;
                    }
                }
                else
                {
                    bool success = false;
                    for (int i = 0; i < 20; i++) // Retry up to 20 times
                    {
                        Bot.Bank.EnsureToInventory(item);
                        Sleep(); // Wait for a short period before checking
                        if (Bot.Inventory.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        Logger($"Failed to unbank {item}, skipping it");
                        continue;
                    }
                }

                Logger($"{item} moved from bank");
            }
        }
    }

    /// <summary>
    /// Transfers specified items by their unique IDs from the bank to the inventory.
    /// </summary>
    /// <param name="itemIDs">
    /// Array of item IDs to transfer from the bank. Items will be skipped if
    /// they are already in the inventory, house, or cannot be found in the bank.
    /// </param>
    /// <remarks>
    /// Ensures that items can be transferred by checking inventory space, 
    /// and retries failed transfers up to 20 times before skipping.
    /// Provides detailed logging for each success or failure.
    /// </remarks>
    public void Unbank(params int[] itemIDs)
    {
        if (itemIDs == null || itemIDs.Length == 0 || !itemIDs.Any(x => x != 0))
            return;

        if (Bot.Player.InCombat)
            JumpWait();

        int RequiredSpaces = itemIDs.Count();
        foreach (int item in itemIDs)
        {
            if (Bot.House.Contains(item) || Bot.Inventory.Contains(item)
                || !Bot.Inventory.Contains(item) && !Bot.House.Contains(item) && !Bot.Bank.Contains(item))
            {
                RequiredSpaces--;
                continue; // Skip the item if it's in house or bank, or nowhere (not in any of the 3 places)
            }

            if (Bot.Bank.Contains(item) && (!Bot.Inventory.Contains(item) || !Bot.House.Contains(item)))
            {
                ItemBase? itemString = Bot.Bank.Items?.FirstOrDefault(x => x?.ID == item);
                if (itemString == null)
                {
                    Logger($"Failed to find item with ID {item}, skipping it");
                    continue;
                }
                if (Bot.Inventory.FreeSlots <= 0 && Bot.Inventory.Slots != 0 && Bot.Inventory.UsedSlots >= Bot.Inventory.Slots)
                {
                    Logger($"Your inventory is full ({Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}), please Make {RequiredSpaces} space(s), and restart the bot", messageBox: true, stopBot: true);
                    return;
                }

                bool isHouseItem = Bot.Bank.TryGetItem(item, out InventoryItem? x) &&
                                  x != null &&
                                  (x.CategoryString == "House" || x.CategoryString == "Wall Item" || x.CategoryString == "Floor Item");

                if (isHouseItem)
                {
                    bool success = false;
                    for (int i = 0; i < 20; i++) // Retry up to 20 times
                    {
                        SendPackets($"%xt%zm%bankToInv%{Bot.Map.RoomID}%{x!.ID}%{x.CharItemID}%");
                        Sleep(); // Wait for a short period before checking
                        if (Bot.House.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        Logger($"Failed to unbank {itemString.Name}, skipping it");
                        continue;
                    }
                }
                else
                {
                    bool success = false;
                    for (int i = 0; i < 20; i++) // Retry up to 20 times
                    {
                        Bot.Bank.EnsureToInventory(item);
                        Sleep(); // Wait for a short period before checking
                        if (Bot.Inventory.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        Logger($"Failed to unbank {itemString.Name}, skipping it");
                        continue;
                    }
                }

                Logger($"{itemString.Name} moved from bank");
            }
        }

    }

    /// <summary>
    /// Transfers specified items from the inventory to the bank by item name.
    /// </summary>
    /// <param name="items">
    /// An array of item names to move to the bank. Items are ignored if they are
    /// equipped, excluded by the blacklist, or do not exist in the inventory.
    /// </param>
    /// <remarks>
    /// The method ensures only items from specified whitelisted categories or
    /// items marked as "Coins" are moved. Attempts each transfer up to 5 times
    /// if the initial move fails and logs any unsuccessful attempts.
    /// 
    /// House items are transferred separately, bypassing the normal inventory-to-bank process.
    /// Certain items specified in the Extras array are also excluded from being banked.
    /// </remarks>
    public void ToBank(params string[] items)
    {
        if (items == null || !items.Any(x => !string.IsNullOrEmpty(x)))
        {
            return;
        }

        JumpWait();

        // Whitelist categories and items
        List<ItemCategory> whiteList = new() { ItemCategory.Note, ItemCategory.Item, ItemCategory.Resource, ItemCategory.QuestItem };
        int?[] Extras = { 18927, 38575 }; // Items that shouldn't be banked

        foreach (string? item in items)
        {
            if (item == null || item == SoloClass || item == FarmClass || FarmGear.Contains(item) || SoloGear.Contains(item))
                continue;

            if (Bot.Inventory.IsEquipped(item) || Bot.House.IsEquipped(item))
            {
                Logger($"Can't bank an equipped item: {item}");
                continue;
            }
            else if ((!Bot.Inventory.Contains(item) || !Bot.House.Contains(item)) && Bot.Bank.Contains(item))
            {
                Logger($"Item {item} is already in the bank, skipping it");
                continue;
            }
            else if (!Bot.Inventory.Items.Concat(Bot.House.Items).Any(x => x.Name == item))
            {
                Logger($"{item} not found in inventory, skipping it");
                continue;
            }

            ItemBase? inventoryItem = Bot.Inventory.Items.Concat(Bot.House.Items).FirstOrDefault(x => x != null && x.Name == item);
            bool itemIsForHouse = Bot.House.TryGetItem(item, out InventoryItem? _item) &&
                                            _item != null &&
                                            (_item.CategoryString == "House" || _item.CategoryString == "Wall Item" || _item.CategoryString == "Floor Item");



            // Check if item is in whitelist and not in blacklist or Extras
            if ((inventoryItem?.Category != null && whiteList.Contains(inventoryItem.Category) || inventoryItem?.Coins == true) &&
                !BankingBlackList.Contains(item) &&
                !Extras.Contains(inventoryItem?.ID) &&
                Bot.Inventory.Contains(item) ||
                (itemIsForHouse && Bot.House.Contains(item) &&
                _item?.Equipped != true))
            {
                if (!itemIsForHouse)
                {
                    for (int i = 0; i < 5 && !Bot.Inventory.EnsureToBank(item); i++)
                        Sleep();
                    if (!Bot.Inventory.EnsureToBank(item) && !Bot.Bank.Contains(item))
                    {
                        Logger($"Failed to bank {item}, skipping it");
                        continue;
                    }
                }
                else if (itemIsForHouse)
                {
                    if (_item != null)
                    {
                        SendPackets($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{_item.ID}%{_item.CharItemID}%");
                        Bot.Wait.ForTrue(() => !Bot.House.Contains(item), 20);
                        if (Bot.House.Items.Any(x => x.Name == item))
                        {
                            Logger($"Failed to bank {item} in house bank, skipping it");
                            continue;
                        }
                    }
                }

                Logger($"{item} moved to bank");
            }
        }
    }

    /// <summary>
    /// Transfers specified items from the inventory to the bank by item ID.
    /// </summary>
    /// <param name="items">
    /// An array of item IDs to move to the bank. Items are ignored if they are
    /// equipped, excluded by the blacklist, or do not exist in the inventory.
    /// </param>
    /// <remarks>
    /// Ensures only items from whitelisted categories or items marked as "Coins" are moved.
    /// Retries each item transfer up to 20 times if an attempt fails and logs unsuccessful attempts.
    /// House items are transferred separately to the house bank.
    /// </remarks>
    public void ToBank(params int[] items)
    {
        if (items == null || !items.Any(x => x > 0))
        {
            return;
        }

        JumpWait();

        // Whitelist categories and items
        List<ItemCategory> whiteList = new() { ItemCategory.Note, ItemCategory.Item, ItemCategory.Resource, ItemCategory.QuestItem };
        int?[] Extras = { 18927, 38575 }; // Items that shouldn't be banked

        foreach (int itemID in items)
        {
            if (itemID <= 0 || Extras.Contains(itemID) || Bot.Inventory.IsEquipped(itemID) || (Bot.House != null && Bot.House.IsEquipped(itemID)))
                continue;

            // Check if the item exists in Inventory or House
            ItemBase? inventoryItem = Bot.Inventory.Items?.Concat(Bot.House?.Items ?? Enumerable.Empty<ItemBase>())
                                         .FirstOrDefault(x => x != null && x.ID == itemID);

            if (inventoryItem == null)
            {
                Logger($"Item with ID {itemID} not found in Inventory or House.");
                continue;
            }

            // Check if the item is equipped
            if (Bot.Inventory.IsEquipped(itemID) || (Bot.House != null && Bot.House.IsEquipped(itemID)))
            {
                Logger($"Can't bank an equipped item: {inventoryItem?.Name ?? $"ID: {itemID}"}");
                continue;
            }

            // Determine if it's a House item
            bool itemIsForHouse = Bot.House?.Items?.Any(x => x?.ID == itemID &&
                                                           (x.CategoryString == "House" ||
                                                            x.CategoryString == "Wall Item" ||
                                                            x.CategoryString == "Floor Item")) ?? false;

            // Whitelist and Blacklist checks
            if ((inventoryItem?.Category != null && whiteList.Contains(inventoryItem.Category)) ||
                inventoryItem?.Coins == true &&
                !BankingBlackList.Contains(inventoryItem.Name))
            {
                if (!itemIsForHouse)
                {
                    bool success = false;
                    for (int attempt = 0; attempt < 20; attempt++)
                    {
                        Bot.Inventory.EnsureToBank(itemID);
                        Sleep();

                        if (Bot.Bank?.Contains(itemID) == true)
                        {
                            success = true;
                            break;
                        }
                    }

                    if (success)
                        Logger($"{inventoryItem.Name ?? $"ID: {itemID}"} moved to bank.");
                    else
                        Logger($"Failed to bank {inventoryItem.Name ?? $"ID: {itemID}"} after 20 attempts.");
                }
                else
                {
                    // Handle House Items
                    InventoryItem? houseItem = Bot.House?.Items?.FirstOrDefault(x => x?.ID == itemID);
                    if (houseItem != null)
                    {
                        SendPackets($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{houseItem.ID}%{houseItem.CharItemID}%");
                        Bot.Wait.ForTrue(() => !(Bot.House?.Contains(itemID) ?? true), 20);

                        if (Bot.House?.Items?.Any(x => x?.ID == itemID) == true)
                        {
                            Logger($"Failed to bank {inventoryItem.Name ?? $"ID: {itemID}"} in house bank.");
                            continue;
                        }

                        Logger($"{inventoryItem.Name ?? $"ID: {itemID}"} moved to house bank.");
                    }
                }
            }
            else
            {
                Logger($"Item {inventoryItem?.Name ?? $"ID: {itemID}"} is blacklisted or excluded.");
            }
        }
    }

    /// <summary>
    /// Transfers specified items from the inventory to the house bank by item name.
    /// </summary>
    /// <param name="items">
    /// An array of item names to move to the house bank. Items are ignored if
    /// they are equipped, set as restricted classes, or do not exist in the house inventory.
    /// </param>
    /// <remarks>
    /// This method ensures each item is properly transferred to the house bank and
    /// logs any unsuccessful attempts. Restricted classes are defined by the variables 
    /// <c>SoloClass</c> and <c>FarmClass</c>, which are skipped during processing.
    /// </remarks>
    public void ToHouseBank(params string[] items)
    {
        if (!items.Any(x => !string.IsNullOrEmpty(x)))
            return;

        JumpWait();

        foreach (string? item in items)
        {
            if (item == SoloClass || item == FarmClass || !Bot.House.Items.Any(x => x != null && x.Name == item && (!x.Coins || x.Equipped)))
                continue;

            if (Bot.House.Contains(item))
            {
                if (!Bot.House.EnsureToBank(item))
                {
                    Logger($"Failed to bank {item}, skipping it");
                    continue;
                }
                Logger($"{item} moved to house bank");
            }
        }
    }

    /// <summary>
    /// Transfers specified items from the inventory to the house bank by item ID.
    /// </summary>
    /// <param name="items">
    /// An array of item IDs to move to the house bank. Items are ignored if
    /// they are equipped or do not exist in the house inventory.
    /// </param>
    /// <remarks>
    /// This method performs a series of checks for each item ID to ensure it is
    /// eligible for transfer. Equipped items are skipped, and each item is 
    /// transferred and logged individually. Any unsuccessful attempts to move 
    /// items are logged for review.
    /// </remarks>
    public void ToHouseBank(params int[] items)
    {
        if (items == null || items.Length == 0 || !items.Any(x => x != 0))
            return;

        JumpWait();

        foreach (int item in items)
        {
            // Skip items that are equipped or not AC tagged
            if (item == 0 || Bot.House.Items.Any(x => x.ID == item && (x.Equipped || !x.Coins)))
                continue;

            if (Bot.House.Contains(item))
            {
                if (!Bot.House.EnsureToBank(item))
                {
                    Logger($"Failed to bank {item}, skipping it");
                    continue;
                }
                Logger($"{item} moved to house bank");
            }
        }
    }

    public List<string> CategoryStrings = new()
    {
        "House",
        "Wall Item",
        "Floor Item",
    };

    /// <summary>
    /// Buys a item till you have the desired quantity
    /// </summary>
    /// <param name="map">Map of the shop</param>
    /// <param name="shopID">ID of the shop</param>
    /// <param name="itemName">Name of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="shopItemID">Use this for Merge shops that has 2 or more of the item with the same name and you need the second/third/etc., be aware that it will re-log you after to prevent ghost buy. To get the ShopItemID use the built in loader of Skua</param>
    /// <param name="Log"></param>
    public void BuyItem(string map, int shopID, string itemName, int quant = 1, int shopItemID = 0, bool Log = true)
    {
        _CheckInventorySpace();
        ShopItem? item = parseShopItem(GetShopItems(map, shopID).Where(x => shopItemID == 0 ? x.Name.ToLower() == itemName.ToLower() : x.ShopItemID == shopItemID).ToList(), shopID, itemName, shopItemID);
        if (item == null)
        {
            Logger($"Failed to find the item with name {itemName} in the shop with ID {shopID}, skipping it");
            return;
        }
        if (!string.IsNullOrEmpty(item.CategoryString) && CategoryStrings.Contains(item.CategoryString))
            _CheckHouseSpace();
        _BuyItem(map, shopID, item, quant, Log);
    }

    /// <summary>
    /// Buys a item till it have the desired quantity
    /// </summary>
    /// <param name="map">Map of the shop</param>
    /// <param name="shopID">ID of the shop</param>
    /// <param name="itemID">ID of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="shopItemID">Use this for Merge shops that has 2 or more of the item with the same name and you need the second/third/etc., be aware that it will relog you after to prevent ghost buy. To get the ShopItemID use the built in loader of Skua</param>
    /// <param name="Log"></param>
    public void BuyItem(string map, int shopID, int itemID, int quant = 1, int shopItemID = 0, bool Log = true)
    {
        if (CheckInventory(itemID, quant))
            return;
        DebugLogger(this);
        _CheckInventorySpace();

        DebugLogger(this);
        if (Bot.Map.Name != map)
        {
            DebugLogger(this);
            Join(map);
            DebugLogger(this);
            Bot.Wait.ForMapLoad(map);
            DebugLogger(this);
        }

        DebugLogger(this);

        // Load shop data
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            if (Bot.Shops.ID == shopID || retry == 20)
                break;
            else retry++;
        }
        retry = 0;

        DebugLogger(this);
        ShopItem? item = parseShopItem(GetShopItems(map, shopID).Where(x => shopItemID == 0 ? x.ID == itemID : x.ShopItemID == shopItemID).ToList(), shopID, itemID.ToString(), shopItemID);
        if (item == null)
        {
            Logger($"Failed to find the item with ID {itemID} in the shop with ID {shopID}, skipping it");
            return;
        }
        DebugLogger(this);
        if (!string.IsNullOrEmpty(item.CategoryString) && CategoryStrings.Contains(item.CategoryString))
            _CheckHouseSpace();
        _BuyItem(map, shopID, item, quant, Log);
    }

    int retrys = 0;
    public void _BuyItem(string map, int shopID, ShopItem? item, int quant, bool Log = true)
    {
        #region IgnoreMe
        // Set potentialy buy breaking options to false
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.AttackWithoutTarget = false;

        int buy_quant;
        int StaticQuant = quant;
        // This process will return early if the materials run out (hopefully) - from `_CalcBuyQuantity`'s calculations. [Line 1695]
        if (item == null || (buy_quant = _CalcBuyQuantity(item, quant)) <= 0 || !_canBuy(shopID, item, quant))
        {
            return;
        }

        if (Bot.Map.Name != map)
        {
            Join(map);
            Bot.Wait.ForMapLoad(map);
        }

        Bot.Events.ExtensionPacketReceived += RelogRequieredListener;
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            JumpWait();
            Sleep();
        }

        // Load shop data
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            if (Bot.Shops.ID == shopID || retry == 20)
                break;
            else retry++;
        }
        retry = 0;
        #endregion IgnoreMe

        dynamic sItem = new ExpandoObject();
        for (int i = 0; i < 5; i++)
        {
            dynamic objData = getData(item.ID, item.ShopItemID);
            try
            {
                sItem = objData;
                sItem.iSel = objData;
                sItem.iQty = buy_quant;
                sItem.iSel.iQty = buy_quant;
                sItem.accept = 1;
                break;
            }
            catch
            {
                Sleep(1000);
            }
            if (i == 5)
            {
                Logger("BuyItem Failed, crashed 5 times", stopBot: true);
                return;
            }
        }
        Sleep(1000);

        Bot.Wait.ForActionCooldown(GameActions.BuyItem);
        Bot.Flash.CallGameFunction("world.sendBuyItemRequestWithQuantity", JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(sItem))!);
        Bot.Wait.ForTrue(() => Bot.Inventory.Contains(item.ID, quant) || Bot.Bank.Contains(item.ID, quant), 20);
        Sleep();

        Bot.Events.ExtensionPacketReceived -= RelogRequieredListener;

        if (CheckInventory(item.ID, StaticQuant))
        {
            if (Log)
                Logger($"Bought {(buy_quant == 302500 ? 1 : buy_quant)} {item.Name}", "BuyItem");
        }
        else
        {
            if (retrys < 5)
            {
                Logger($"Failed at buying {(buy_quant == 302500 ? 1 : buy_quant)} {item.Name}, retrying: x{retrys}", "BuyItem");
                retrys++;
                JumpWait();
                _BuyItem(map, shopID, item, quant, Log);
            }
            else
            {
                retrys = 0;
                Logger($"Failed at buying {(buy_quant == 302500 ? 1 : buy_quant)} {item.Name}", "BuyItem");
            }
        }

        void RelogRequieredListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type == "json")
            {
                string str = data.strMessage;
                switch (str)
                {
                    case "Item is not buyable. Item Inventory full. Re-login to syncronize your real bag slot amount.":
                        Logger("Inventory de-sync (AE Issue) detected, relogging so the bot can continue");
                        Relogin();
                        if (Bot.Inventory.FreeSlots < 1)
                            Logger($"Inventory Slots: {Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}, Free: {Bot.Inventory.FreeSlots}. Clean your inventory... stopping", stopBot: true);
                        break;

                    case "Quest Complete Failed: Missing Required Item":
                        Logger("Quest de-sync (AE Issue) detected, relogging so the bot can continue");
                        Relogin();
                        break;
                }
            }
        }

        dynamic getData(int itemID, int shopItemID = 0)
        {
            dynamic[]? shopItems = Bot.Flash.GetGameObject<dynamic[]>("world.shopinfo.items");
            if (shopItems != null)
            {
                foreach (dynamic i in shopItems)
                {
                    if (i == null || i!.ItemID == null || i!.ItemID != itemID ||
                       (shopItemID != 0 ? (i!.ShopItemID == null || i!.ShopItemID != shopItemID) : false))
                        continue;
                    return i!;
                }
            }
            Logger($"Failed to find the shopItemData for itemID {itemID} in {shopID}" + reinstallCleanFlash, "BuyItem");
            return null!;
        }

        bool _canBuy(int shopID, ShopItem? item, int buy_quant)
        {
            if (item == null)
                return false;

            //Achievement Check
            int achievementID = Bot.Flash.GetGameObject<int>("world.shopinfo.iIndex");
            string? io = Bot.Flash.GetGameObject<string>("world.shopinfo.sField");
            if (achievementID > 0 && io != null && !HasAchievement(achievementID, io))
            {
                Logger($"Cannot buy {item.Name} from {shopID} because you dont have achievement {achievementID} of category {io}.", "CanBuy");
                return false;
            }

            //Member Check
            if (item.Upgrade && !IsMember)
            {
                Logger($"Cannot buy {item.Name} from {shopID} because you aren't a member.", "CanBuy");
                return false;
            }

            //Required-Item Check
            int reqItemID = Bot.Flash.GetGameObject<int>("world.shopinfo.reqItems");
            if (reqItemID > 0 && !CheckInventory(reqItemID))
            {
                Logger($"Cannot buy {item.Name} from {shopID} because you dont have the requiered item needed to buy stuff from the shop, itemID: {reqItemID}", "CanBuy");
                return false;
            }

            //Quest Check
            string? questName = Bot.Flash.GetGameObject<List<dynamic>>("world.shopinfo.items")?.Find(d => d.ItemID == item.ID)?.sQuest;
            if (!string.IsNullOrEmpty(questName))
            {
                List<QuestData>? v = JsonConvert.DeserializeObject<List<QuestData>?>(File.ReadAllText(ClientFileSources.SkuaQuestsFile));
                if (v != null)
                {
                    List<int> ids = v.Where(x => x.Name == questName).Select(q => q.ID).ToList();
                    if (ids.Any())
                    {
                        List<Quest> quests = EnsureLoad(ids.Where(q => !isCompletedBefore(q)).ToArray());
                        if (quests.Any())
                        {
                            string s = string.Empty;
                            quests.ForEach(q => s += $"[{q.ID}] |");
                            bool one = quests.Count == 1;
                            Logger($"Cannot buy {item.Name} from {shopID} because you havn't completed the {(one ? "" : "one of ")}following quest{(one ? "" : "s")}: \"{questName}\" {s[..^2]}", "CanBuy");
                            return false;
                        }
                    }
                }
            }

            //Rep check
            if (!string.IsNullOrEmpty(item.Faction) && item.Faction != "None")
            {
                int reqRank = PointsToLevel(item.RequiredReputation);
                if (reqRank > Bot.Reputation.GetRank(item.Faction))
                {
                    Logger($"Cannot buy {item.Name} from {shopID} because you dont have rank {reqRank} {item.Faction}.", "CanBuy");
                    return false;
                }
            }

            //Merge item check
            int itemCount = item.Quantity == 0 || item.Quantity == 302500 ? 1 : item.Quantity;
            int buy_count = (int)Math.Ceiling(buy_quant / (decimal)itemCount);
            if (item.Requirements.Any())
            {
                foreach (ItemBase req in item.Requirements)
                {
                    if (CheckInventory(req.ID, req.Quantity))
                        continue;

                    Bot.Drops.Pickup(req.ID);
                    Bot.Wait.ForPickup(req.ID);

                    int total_quant = buy_count * req.Quantity;

                    if (GetShopItems(map, shopID).Any(x => req.ID == x.ID))
                        BuyItem(map, shopID, req.ID, total_quant, Log: Log);

                    if (!CheckInventory(req.ID, total_quant))
                    {
                        if (CheckInventory(req.ID))
                        {
                            Logger($"Cannot buy {req.Name} from {shopID}.\n" +
                            $"You own {Bot.Inventory.GetQuantity(req.ID)}x {req.Name}.\n" +
                            $"You need {total_quant}.", "CanBuy");

                            return false;
                        }
                        Logger($"Cannot buy {item.Name} from {shopID} because {req.Name} is missing.", "CanBuy");
                        return false;
                    }
                }
            }

            if (item.Cost > 0)
            {
                //Gold check
                if (!item.Coins)
                {
                    int total_gold_cost = buy_count * item.Cost;
                    if (total_gold_cost > 100000000)
                    {
                        Logger($"Cannot buy more than 100 mil worth of items.", "CanBuy");
                        return false;
                    }
                    else if (total_gold_cost > Bot.Player.Gold)
                    {
                        Logger($"Cannot buy {item.Name} from {shopID}.", "CanBuy");
                        Logger($"You own {Bot.Inventory.GetQuantity(item.ID)}x {item.Name}.", "CanBuy");
                        Logger($"You need {Bot.Inventory.GetQuantity(item.ID) + buy_count}.", "CanBuy");
                        Logger($"You are missing {total_gold_cost - Bot.Player.Gold} gold to buy enough.", "CanBuy");
                        return false;
                    }
                }
                //AC costing check
                else
                {
                    int total_ac_cost = buy_count * item.Cost;
                    if (Bot.ShowMessageBox(
                            $"The bot is about to buy \"{item.Name}\" {buy_count} times, which costs {total_ac_cost} AC, do you accept this?",
                            "Warning: Costs AC!", true)
                            != true)
                    {
                        Logger($"Cannot buy {item.Name} from {shopID} because you didn't allow the bot to buy the item", "CanBuy");
                        return false;
                    }
                    else if (Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins") < total_ac_cost)
                    {
                        Logger($"Cannot buy {item.Name} from {shopID} because you are missing {Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins") - total_ac_cost} ACs", "CanBuy");
                        return false;
                    }
                }

            }
            return true;
        }
    }

    /// <summary>
    /// Determines the maximum quantity of an item that can be purchased from a specified shop.
    /// </summary>
    /// <param name="map">The name of the map to join.</param>
    /// <param name="shopID">The ID of the shop to load.</param>
    /// <param name="item">The shop item to check. If <c>null</c>, the method returns 0.</param>
    /// <returns>
    /// The maximum quantity of the item that can be purchased, or 0 if the item is not found or an error occurs.
    /// </returns>
    /// <remarks>
    ///This method joins the specified map, ensures the player is out of combat, loads the shop, and retrieves the item's maximum buy quantity. 
    /// Ensure the shop and map names are correct before calling this method.
    /// Example:
    /// <code>
    /// int maxQuantity = Core.MaxBuyQuant("map", shopID, Bot.Shops.Items.FirstOrDefault(x => x.ShopItemID == ShopItemID));
    /// </code>
    /// </remarks>
    public int MaxBuyQuant(string map, int shopID, ShopItem? item)
    {
        if (item == null)
            return 0; // Return early if no item provided.

        Join(map);
        Bot.Wait.ForMapLoad(map);

        // Ensure player is out of combat
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            JumpWait();
            Sleep();
        }

        // Load shop data
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            if (Bot.Shops.ID == shopID || retry == 20)
                break;
            else retry++;
        }
        retry = 0;

        // Try to fetch shop item data with retries
        dynamic? sItem = null;
        for (int i = 0; i < 5; i++)
        {
            sItem = InitializeWithRetries(() => GetShopItemData(item.ID, item.ShopItemID));
            if (sItem != null)
                break;
            Sleep(1000); // Wait before retrying
        }

        if (sItem == null)
        {
            Logger($"Failed to load shop item data for ItemID {item.ID} in ShopID {shopID}.", "MaxBuyQuant");
            return 0;
        }

        Sleep(1000); // Extra delay to prevent race conditions

        // Wait for BuyItem cooldown and fetch maximum buy quantity
        Bot.Wait.ForActionCooldown(GameActions.BuyItem);
        return Bot.Flash.CallGameFunction<int>(
            "world.maximumShopBuys",
            JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(sItem))!
        );
    }

    /// <summary>
    /// Fetches the shop item data from the game's shop information.
    /// </summary>
    private dynamic? GetShopItemData(int itemID, int shopItemID = 0)
    {
        dynamic[]? shopItems = Bot.Flash.GetGameObject<dynamic[]>("world.shopinfo.items");
        if (shopItems == null)
        {
            Logger("Shop items data could not be retrieved.", "GetShopItemData");
            return null;
        }

        foreach (dynamic item in shopItems)
        {
            if (item?.ItemID == itemID &&
                (shopItemID == 0 || item?.ShopItemID == shopItemID))
            {
                return item;
            }
        }

        Logger($"Shop item data not found for ItemID {itemID}, ShopItemID {shopItemID}.", "GetShopItemData");
        return null;
    }

    private void _CheckInventorySpace()
    {
        if (Bot.Inventory.Slots != 0 && Bot.Inventory.FreeSlots <= 0)
        {
            int usedBefore = Bot.Inventory.UsedSlots;
            Logger($"Your inventory is full [{usedBefore}/{Bot.Inventory.Slots}]. Attempting to bank AC-tagged misc items...", "_CheckInventorySpace");

            BankACMisc();

            int usedAfter = Bot.Inventory.UsedSlots;
            int freed = usedBefore - usedAfter;

            if (Bot.Inventory.FreeSlots <= 0)
            {
                Logger($"Banked {freed} item{(freed != 1 ? "s" : "")} but your inventory is still full. Please clear space manually. Stopping the bot.", "_CheckInventorySpace", stopBot: true);
            }
            else
            {
                Logger($"Banked {freed} item{(freed != 1 ? "s" : "")}. {Bot.Inventory.FreeSlots} slot{(Bot.Inventory.FreeSlots != 1 ? "s" : "")} now available.", "_CheckInventorySpace");
            }
        }
    }


    private void _CheckHouseSpace()
    {
        if (Bot.House.Slots != 0 && Bot.House.FreeSlots <= 0)
        {
            int usedBefore = Bot.House.UsedSlots;
            Logger($"Your house inventory is full [{usedBefore}/{Bot.House.Slots}]. Attempting to bank AC-tagged misc items...", "_CheckHouseSpace");

            BankACHouseItems();

            int usedAfter = Bot.House.UsedSlots;
            int freed = usedBefore - usedAfter;

            if (Bot.House.FreeSlots <= 0)
            {
                Logger($"Banked {freed} item{(freed != 1 ? "s" : "")} but your house is still full. Please clear space manually. Stopping the bot.", "_CheckHouseSpace", stopBot: true);
            }
            else
            {
                Logger($"Banked {freed} item{(freed != 1 ? "s" : "")}. {Bot.House.FreeSlots} slot{(Bot.House.FreeSlots != 1 ? "s" : "")} now available.", "_CheckHouseSpace");
            }
        }
    }


    // private int _CalcBuyQuantity(ShopItem item, int requestedAmount)
    // {
    //     if (requestedAmount > item.MaxStack)
    //     {
    //         Logger($"Requested {requestedAmount}, but max stack for {item.Name} is {item.MaxStack}. Fix the calling script.", "BuyItem");
    //         Bot.Stop(true);
    //     }

    //     int itemStackSize = item.Quantity;
    //     int currentStock = Bot.Inventory.GetQuantity(item.ID);
    //     int neededAmount = requestedAmount;
    //     Logger($"itemStackSize: {itemStackSize}");
    //     Logger($"requestedAmount: {requestedAmount}");
    //     Logger($"neededAmount: {neededAmount}");

    //     if (CheckInventory(item.ID, requestedAmount))
    //     {
    //         Logger($"Already have enough of {item.Name} ({currentStock}/{requestedAmount}).");
    //         return 0;
    //     }

    //     // Round up to the nearest multiple of itemStackSize
    //     int buyAmount = (int)Math.Ceiling((double)neededAmount / itemStackSize) * itemStackSize;

    //     // Ensure buyAmount does not exceed MaxStack
    //     int maxCanBuy = item.MaxStack - currentStock;
    //     buyAmount = Math.Min(buyAmount, maxCanBuy - (maxCanBuy % itemStackSize)); // Adjust to nearest valid multiple

    //     // If we still can't reach the requested amount, sell extra to make space
    //     if (buyAmount < neededAmount)
    //     {
    //         int excess = (currentStock + buyAmount) % itemStackSize;
    //         if (excess > 0)
    //         {
    //             Logger($"Selling {excess} {item.Name} to fit a proper stack.");
    //             SellItem(item.Name, excess);
    //             buyAmount += excess; // Now we can buy the exact amount
    //         }
    //     }

    //     if (buyAmount <= 0)
    //     {
    //         Logger($"Cannot buy more {item.Name}, max stack reached ({currentStock}/{item.MaxStack}).");
    //         return 0;
    //     }

    //     Logger($"Final purchase amount for {item.Name}: {buyAmount}");
    //     return buyAmount;
    // }

    private int _CalcBuyQuantity(ShopItem item, int requestedAmount)
    {
        if (requestedAmount > item.MaxStack)
        {
            Logger($"Requested {requestedAmount}, but max stack for {item.Name} is {item.MaxStack}. Fix the calling script.", "BuyItem");
            Bot.Stop(true);
        }

        // Unbank it if its in bank
        if (Bot.Bank.Contains(item.ID) && !Bot.Inventory.Contains(item.ID))
            Unbank(item.ID);

        int itemStackSize = item.Quantity == 302500 ? 1 : item.Quantity;
        // Cocant (make take everything into a giant array, and then find the current quantity if its null, default to 0)
        int currentStock = Bot.Inventory.Items
        .Concat(Bot.Bank.Items)
        .Concat(Bot.House.Items)
        .Concat(Bot.House.Items)
        .Concat(Bot.TempInv.Items) // Because fucking why not
        .FirstOrDefault(x => x.ID == item.ID)?.Quantity ?? 0;
        // int neededAmount = requestedAmount - currentStock; // Calculate how much more is needed

        // Check if all requirements are met before proceeding
        foreach (var req in item.Requirements)
        {
            int totalNeeded = requestedAmount / itemStackSize * req.Quantity;  // Adjust required amount for stack size

            // Cocant (make take everything into a giant array, and then find the current quantity if its null, default to 0)
            int reqCurrent = Bot.Inventory.Items.Concat(Bot.Bank.Items).Concat(Bot.House.Items).Concat(Bot.TempInv.Items /* because fucking why not*/ ).FirstOrDefault(x => x.ID == req.ID)?.Quantity ?? 0; //

            Logger($"req: {req.Name}, totalNeeded: {totalNeeded}, reqCurrent: {reqCurrent}");
            if (reqCurrent < totalNeeded)
            {
                Logger($"Missing {req.Name} ({reqCurrent}/{totalNeeded}). Cannot proceed with purchase.");
                return 0; // Stop the buy process if requirements are not met
            }
        }

        // Calculate the buy amount, ensuring we don't exceed the required total
        int buyAmount = (int)Math.Ceiling((double)requestedAmount / itemStackSize) * itemStackSize;

        // Ensure buyAmount does not exceed the maximum stack
        int maxCanBuy = item.MaxStack - currentStock;
        buyAmount = Math.Min(buyAmount, maxCanBuy - (maxCanBuy % itemStackSize)); // Round to nearest valid stack size

        // Ensure that the total bought amount matches the requestedAmount
        if (buyAmount + currentStock > requestedAmount)
        {
            buyAmount = (int)Math.Ceiling((double)requestedAmount / itemStackSize) * itemStackSize;
        }

        if (buyAmount <= 0)
        {
            Logger($"Cannot buy more {item.Name}, max stack reached ({currentStock}/{item.MaxStack}).");
            return 0; // Stop if no further items can be bought
        }

        // Disable Logger till needed
        // Logger($"Final purchase amount for {item.Name}: {buyAmount}");
        return buyAmount;
    }

    public int PointsToLevel(int points) => RepCPLevel.First(kvp => points <= kvp.Value).Key;

    private readonly Dictionary<int, int> RepCPLevel = new()
    {
        { 1, 0 },
        { 2, 900 },
        { 3, 3600 },
        { 4, 10000 },
        { 5, 22500 },
        { 6, 44100 },
        { 7, 78400 },
        { 8, 129600 },
        { 9, 202500 },
        { 10, 302500 },
    };

    /// <summary>
    /// Sells a item till you have the desired quantity
    /// </summary>
    /// <param name="itemName">Name of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="all">Set to true if you wish to sell all the items</param>
    public void SellItem(string itemName, int quant = 0, bool all = false)
    {
        if (!(quant > 0 ? CheckInventory(itemName, quant) : CheckInventory(itemName)) || !Bot.Inventory.TryGetItem(itemName, out InventoryItem? item))
            return;

        InventoryItem? Item = null;
        for (int i = 0; i < 5; i++)
        {
            Item = Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(x => x != null && x.Name == itemName);
            if (Item != null)
                break;
            Logger($"Attempt {i + 1}: Item {itemName} not found. Retrying...");
            Sleep(1000); // Wait for 1 second before retrying
        }

        if (item == null)
        {
            Logger($"Item {itemName} not found after 5 attempts.");
            return;
        }
        if (Bot.Bank.Contains(itemName) && !Bot.Inventory.Contains(itemName))
            Unbank(itemName);

        int retryCount = 0;
        int sell_count = all ? Bot.Inventory.GetQuantity(itemName) : quant;
        int QuantAfterSale = Bot.Inventory.GetQuantity(itemName) - sell_count;
    Retry:
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            JumpWait();
            Sleep();
            if (!Bot.Player.InCombat)
                break;
        }

        if (!all)
        {
            // Inv quant >= current quantity.
            Bot.Wait.ForActionCooldown(GameActions.SellItem);
            Bot.Send.Packet($"%xt%zm%sellItem%{Bot.Map.RoomID}%{item.ID}%{sell_count}%{item.CharItemID}%");
            Bot.Wait.ForItemSell();
            Sleep();
        }
        else
        {
            Bot.Shops.SellItem(itemName);
            Bot.Wait.ForItemSell();
        }

        if (!all && (Bot.Inventory.Contains(itemName) || Bot.Inventory.GetQuantity(itemName) == QuantAfterSale))
        {
            if (Bot.Inventory.GetQuantity(itemName) == QuantAfterSale)
                Logger($"Sold x{sell_count} \"{itemName}\"");
            return;
        }
        else if (all && !Bot.Inventory.Contains(itemName))
        {
            Logger($"Sold ALL of \"{itemName}\"");
            return;
        }
        else
        {
            if (retryCount < 5)
            {
                retryCount++;
                Logger($"{itemName} failed to sell, retrying [Try x{retryCount}]");
                goto Retry;
            }
            else
            {
                Logger($"{itemName} failed to sell, retrying x{retryCount} times did not succeed");
                retryCount = 0;
                return;
            }
        }
    }

    /// <summary>
    /// Retrieves a list of shop items from the specified shop.
    /// </summary>
    /// <param name="map">The map to join in order to access the shop.</param>
    /// <param name="shopID">The identifier of the shop to retrieve items from.</param>
    /// <returns>A list of <see cref="ShopItem"/> objects from the specified shop, or an empty list if the shop data could not be loaded.</returns>
    public List<ShopItem> GetShopItems(string map, int shopID)
    {
        // Ensure player is in map
        if (Bot.Map.Name != map)
        {
            Join(map);
            Bot.Wait.ForMapLoad(map);
        }

        // Load shop data
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            if (Bot.Shops.ID == shopID || retry == 20)
                break;
            else retry++;
        }
        retry = 0;

        // Return the shop items
        return Bot.Shops.Items;
    }

    /// <summary>
    /// Parses and retrieves a shop item from a list based on the provided criteria.
    /// </summary>
    /// <param name="shopItem">A list of <see cref="ShopItem"/> objects to search through.</param>
    /// <param name="shopID">The identifier of the shop where the item should be located.</param>
    /// <param name="itemNameID">The name or identifier of the item to find.</param>
    /// <param name="shopItemID">The specific identifier of the shop item to retrieve. Defaults to 0, which means it is not used.</param>
    /// <returns>
    /// The <see cref="ShopItem"/> that matches the criteria, or <c>null</c> if no item is found or if there are issues with the provided criteria.
    /// </returns>
    /// <remarks>
    /// If no items are found in the list, logs an error message indicating that the item was not found in the specified shop.
    /// If multiple items are found and a specific ShopItemID is provided, retrieves the item with the matching ShopItemID, logging an error if it is not found.
    /// If multiple items are found and no ShopItemID is provided, logs an error indicating that the ShopItemID is needed.
    /// </remarks>
    public ShopItem? parseShopItem(List<ShopItem> shopItem, int shopID, string itemNameID, int shopItemID = 0)
    {
        if (shopItem.Count == 0)
        {
            Logger($"Item {itemNameID} not found in shop {shopID}.");
            return null;
        }
        else if (shopItem.Count > 1)
        {
            if (shopItemID > 0)
            {
                if (!shopItem.Any(x => x.ShopItemID == shopItemID))
                {
                    Logger($"Item {itemNameID} with ShopItemID {shopItemID} was not in {shopID}. The developer needs to correct the Shop Item ID.");
                    return null;
                }
                return shopItem.First(x => x.ShopItemID == shopItemID);
            }
            Logger($"Multiple items found with the name {itemNameID} in shop {shopID}. The developer needs to specify the Shop Item ID.");
            return null;
        }

        return shopItem.First();
    }

    /// <summary>
    /// Creates and adds a ghost item to the inventory or temporary inventory based on the specified parameters.
    /// </summary>
    /// <param name="ID">The unique identifier for the item.</param>
    /// <param name="name">The name of the ghost item. Defaults to "Ghost Item".</param>
    /// <param name="quantity">The quantity of the ghost item. Defaults to 1.</param>
    /// <param name="temp">If true, adds the item to the temporary inventory; otherwise, adds it to the regular inventory. Defaults to false.</param>
    /// <param name="category">The category of the ghost item. Defaults to ItemCategory.Unknown.</param>
    /// <param name="description">The description of the ghost item. Defaults to a description indicating it's a ghost item with the specified ID.</param>
    /// <param name="level">The level of the ghost item. Defaults to 1.</param>
    /// <param name="extraInfo">Additional properties to add or modify for the ghost item, specified as a series of key-value pairs.</param>
    /// <remarks>
    /// The ghost item created will have a default icon based on its category, and properties for enhancements are added if applicable.
    /// The method uses dynamic typing to create the item object and calls a game function to add it to the player's inventory or temporary inventory.
    /// </remarks>
    public void GhostItem(int ID, string name = "Ghost Item", int quantity = 1, bool temp = false, ItemCategory category = ItemCategory.Unknown, string? description = null, int level = 1, params (string, object)[] extraInfo)
    {
        if (temp ? (Bot.TempInv.Contains(ID) && Bot.TempInv.Contains(name)) : (Bot.Inventory.Contains(ID) && Bot.Inventory.Contains(name)))
            return;

        dynamic item = new ExpandoObject();

        item.ItemID = ID;
        item.sName = name;
        item.sDesc = description ?? "A Ghost Item that mimics Item ID: " + ID;

        item.iLvl = level;
        if (quantity != 0) // This allows for ghost items without taking up slots, but it'll not work for bypasses
        {
            item.iQty = quantity;
            item.iStk = quantity > 0 ? quantity : 1;
        }

        item.sType = category == ItemCategory.Unknown ? "Item" : category.ToString();
        #region icon switch
        item.sIcon = category switch
        {
            ItemCategory.Sword => "iwsword",
            ItemCategory.Axe => "iwaxe",
            ItemCategory.Dagger => "iwdagger",
            ItemCategory.Gun or ItemCategory.HandGun or ItemCategory.Rifle or ItemCategory.Whip => "iwgun",
            ItemCategory.Bow => "iwbow",
            ItemCategory.Mace => "iwmace",
            ItemCategory.Gauntlet => "iwclaws",
            ItemCategory.Polearm => "iwpolearm",
            ItemCategory.Staff => "iwstaff",
            ItemCategory.Wand => "iwwand",

            ItemCategory.Class => "iiclass",
            ItemCategory.Armor => "iwarmor",
            ItemCategory.Helm => "iihelm",
            ItemCategory.Cape => "iicape",
            ItemCategory.Pet => "iipet",

            ItemCategory.Amulet or ItemCategory.Necklace => "iin1",
            // Ground Rune
            ItemCategory.Misc => "imr2",

            ItemCategory.House => "ihhouse",
            ItemCategory.WallItem => "ihwall",
            ItemCategory.FloorItem => "ihfloor",

            ItemCategory.Enhancement => "none",

            //Default (Unknown, Note, Resource, Item, ServerUse)
            _ => "iibag",
        };
        #endregion
        // Add enhancements property for enhancable equipment

        item.bEquip = 0;
        item.bStaff = 0;

        // Adding / modifying based on extra info
        IDictionary<string, object>? _item = item as IDictionary<string, object>;
        foreach ((string, object) info in extraInfo)
            _item![info.Item1] = info.Item2;
        //if (item.sLink is not null && item.sFile is not null)
        //    item.bSCP = false;

        // Yes it needs to call 'item', not '_item', they are linked in memory
        Bot.Flash.CallGameFunction("world.myAvatar.addItem", item);
    }

    /// <summary>
    /// Retrieves the best item for the specified boost type and category.
    /// </summary>
    /// <param name="boostType">The type of boost to consider when finding the best item.</param>
    /// <param name="categoryString">
    /// The category of the item to find (e.g., Weapon, Armor, Helm, Cape). If <c>null</c>, the method will default to filtering weapon categories.
    /// Use <see cref="ItemCategory.Unknown"/> to include all categories.
    /// </param>
    /// <returns>
    /// The name of the item with the highest boost value for the specified boost type and category.
    /// If no such item is found, returns the name of the first equipped item matching the specified category.
    /// Returns <c>null</c> if no suitable item is found.
    /// </returns>
    public string? GetBestItem(GenericGearBoostType boostType, string? categoryString = null)
    {
        if (CBOBool("DisableBestGear", out bool _DisableBestGear) && _DisableBestGear)
            return string.Empty;

        // Convert the boost type to a string
        string boostTypeString = boostType.ToString();

        // Determine the category filter
        bool categoryFilter(InventoryItem x) =>
            categoryString == null
            ? (x.Category == ItemCategory.Sword
                || x.Category == ItemCategory.Axe
                || x.Category == ItemCategory.Dagger
                || x.Category == ItemCategory.Gun
                || x.Category == ItemCategory.HandGun
                || x.Category == ItemCategory.Rifle
                || x.Category == ItemCategory.Bow
                || x.Category == ItemCategory.Mace
                || x.Category == ItemCategory.Gauntlet
                || x.Category == ItemCategory.Polearm
                || x.Category == ItemCategory.Staff
                || x.Category == ItemCategory.Wand
                || x.Category == ItemCategory.Whip)
            : x.CategoryString == categoryString;

        // Find the item with the highest boost
        string? item = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .Where(x => x != null
                        && (!x.Upgrade || Bot.Player.IsMember) // Allow upgrade items if the player is a member
                        && categoryFilter(x)) // Filter items by category
            .OrderByDescending(x => GetBoostFloat(x, boostTypeString)) // Sort items by boost value in descending order
            .FirstOrDefault() // Select the item with the highest boost
            ?.Name
            // If no item with a high boost is found, search for items with the specified category and equipped
            ?? Bot.Inventory.Items
                .Where(x => x != null && categoryFilter(x) && x.Equipped)
                .FirstOrDefault() // Select the first item that matches the category criteria
                ?.Name;

        if (item != null)
        {
            if (!Bot.Inventory.Contains(item) && Bot.Bank.Contains(item))
                Unbank(item);
        }
        else
            Logger("No suitable item found.");

        return item;
    }

    /// <summary>
    /// Retrieves the names of the best items for specific categories.
    /// </summary>
    /// <returns>
    /// An array of strings where each element corresponds to the best item name for a specific category.
    /// If no suitable item is found for a category, the array will contain "None" for that category.
    /// </returns>
    public string[] BestGear(GenericGearBoostType boostType)
    {

        if (CBOBool("DisableBestGear", out bool _DisableBestGear) && _DisableBestGear)
            return Array.Empty<string>();

        // Initialize the list to hold the best items for each category
        List<string> bestItems = new();

        // Define categories and their corresponding category strings
        Dictionary<string, string?> categories = new()
        {
        { "Armor", ItemCategory.Armor.ToString() },
        { "Helm", ItemCategory.Helm.ToString() },
        { "Cape", ItemCategory.Cape.ToString() },
        { "Pet", ItemCategory.Pet.ToString() },
        { "FloorItem", ItemCategory.FloorItem.ToString() }
    };

        // Add the best item for each defined category
        foreach (KeyValuePair<string, string?> category in categories)
        {
            string bestItem = GetBestItem(boostType, category.Value) ?? "None";
            if (bestItem == "None")
                continue;

            bestItems.Add(bestItem);
        }

        // Add all the best items to the unbanking process
        Unbank(bestItems.ToArray());

        // Add the best weapon item
        string bestWeapon = GetBestItem(GenericGearBoostType.dmgAll, null) ?? "None";
        bestItems.Add(bestWeapon);

        // Return the list as an array
        return bestItems.ToArray();
    }

    /// <summary>
    /// Retrieves the boost value for the specified boost type from the given item.
    /// </summary>
    /// <param name="item">The item from which to retrieve the boost value.</param>
    /// <param name="boostType">The type of boost to retrieve.</param>
    /// <returns>
    /// The boost value for the specified boost type. Returns 0 if the boost type is not present in the item's metadata.
    /// </returns>
    public float GetBoostFloat(InventoryItem item, string boostType)
    {
        if (string.IsNullOrEmpty(item.Meta) || !item.Meta.Contains(boostType))
            return 0F;
        return _getBoostFloat(item, boostType);
    }

    private float _getBoostFloat(InventoryItem item, string boostType)
    {
        return float.Parse(
            item.Meta
                .Split(',')
                .First(meta => meta.Contains(boostType))
                .Split(':')
                .Last()
            , CultureInfo.InvariantCulture.NumberFormat);
    }

    /// <summary>
    /// Removes the specified items from players inventory (Banks AC items)
    /// </summary>
    /// <param name="items">Items to Trash/Bank</param>
    public void TrashCan(params string[] items)
    {
        while (Bot.ShouldExit && (Bot.Player.InCombat || Bot.Player.HasTarget))
        {
            Bot.Combat.CancelTarget();
            Bot.Combat.Exit();
            Bot.Wait.ForCombatExit();
            JumpWait();
            Sleep();
        }

        foreach (string item in items)
        {
            if (!Bot.Inventory.TryGetItem(item, out InventoryItem? TrashItem) || TrashItem == null || TrashItem.Temp)
                continue;

            if (!TrashItem.Coins)
            {
                Bot.Send.Packet($"%xt%zm%removeItem%{Bot.Map.RoomID}%{TrashItem.ID}%{Bot.Player.ID}%{TrashItem.Quantity}%");
                Sleep();
                Logger($"Trashed: {TrashItem.Name} x{TrashItem.Quantity}");
            }
            else ToBank(TrashItem.ID);
        }
    }

    #endregion

    #region Drops

    /// <summary>
    /// Adds drops to the pickup list, un-bank the items.
    /// </summary>
    /// <param name="items">Items to add</param>
    public void AddDrop(params string[] items)
    {
        if (items == null || items.Length == 0)
        {
            return;
        }
        Unbank(items);
        Bot.Drops.Add(items);
    }

    /// <summary>
    /// Adds drops to the pickup list and un-banks the items.
    /// </summary>
    /// <param name="items">Items to add.</param>
    public void AddDrop(params int[] items)
    {
        if (items == null || items.Length == 0)
        {
            return;
        }
        Unbank(items);
        Bot.Drops.Add(items);
    }


    /// <summary>
    /// Removes drops from the pickup list.
    /// </summary>
    /// <param name="items">Items to remove</param>
    public void RemoveDrop(params string[] items)
    {
        Bot.Drops.Remove(items);
    }

    /// <summary>
    /// Removes drops from the pickup list.
    /// </summary>
    /// <param name="items">Items to remove</param>
    public void RemoveDrop(params int[] items)
    {
        Bot.Drops.Remove(items);
    }

    #endregion

    #region Quest
    private CancellationTokenSource? questCTS = null;
    private async Task EnsureQuestAccepted(int questID)
    {
        if (!Bot.Quests.IsInProgress(questID))
        {
            Bot.Quests.Accept(questID);
            await Task.Delay(ActionDelay * 2); // Wait for the action delay to ensure the quest is accepted
        }
    }

    /// <summary>
    /// This will register quests to be completed while doing something else, i.e. while in combat.
    /// If it has quests already registered, it will cancel them first and then register the new quests.
    /// </summary>
    /// <param name="questIDs">ID of the quests to be completed.</param>
    public void RegisterQuests(params int[] questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
            return;

        Dictionary<Quest, int> chooseQuests = new();
        Dictionary<Quest, int> nonChooseQuests = new();

        foreach (int questID in questIDs)
        {
            Quest? q = InitializeWithRetries(() => EnsureLoad(questID));
            if (q == null)
            {
                Logger($"Failed to initialize quest with ID {questID}.");
                continue;
            }

            if (q.Upgrade && !IsMember)
            {
                Logger($"Quest {questID} requires membership, but the player is not a member.");
                continue;
            }

            List<ItemBase> missingRequirements = q.AcceptRequirements.Where(x => x != null && !CheckInventory(x.ID)).ToList();
            if (missingRequirements.Any())
            {
                Logger($"Player is missing the following accept requirements for quest {questID}: {string.Join(", ", missingRequirements.Select(x => x.Name))}");
                continue;
            }


            if (q.SimpleRewards.Any(r => r.Type == 2))
            {
                if (!chooseQuests.ContainsKey(q))
                    chooseQuests.Add(q, 0);
            }
            else
            {
                if (!nonChooseQuests.ContainsKey(q))
                    nonChooseQuests.Add(q, 0);
            }

            // Collect unique item IDs and unbank them in one call
            int[] itemsToUnbank = q.AcceptRequirements
                                   .Concat(q.Requirements)
                                   .Select(x => x.ID)
                                   .Distinct()
                                   .ToArray();

            Unbank(itemsToUnbank);
            Bot.Drops.Add(q.AcceptRequirements.Concat(q.Requirements)
                .Where(x => x != null && !x.Temp)
                .Select(x => x.Name).ToArray());
        }
        GC.Collect();


        questCTS = new();
        int i = 0;
        //no initializationwithretries in asyncs as init has sleeps in it.
        Task.Run(async () =>
        {
            while (!Bot.ShouldExit && !questCTS.IsCancellationRequested)
            {
                foreach (Quest quest in chooseQuests.Keys.Concat(nonChooseQuests.Keys).Where(x => Bot.Quests.TryGetQuest(x.ID, out Quest? _quest) && _quest != null).Distinct().ToList())
                {
                    if (Bot.ShouldExit)
                    {
                        questCTS.Cancel();
                        return;
                    }

                    // Ensure player is alive so it can load the quest.
                    if (!Bot.Player.Alive)
                    {
                        await Task.Delay(ActionDelay);
                        continue;
                    }

                    Quest? q = Bot.Quests.EnsureLoad(quest.ID);

                    await Task.Delay(ActionDelay * 2);

                    if (q == null || quest == null)
                    {
                        Bot.Quests.Load(quest!.ID);
                        await Task.Delay(ActionDelay * 2);
                    }

                    if (Bot.Quests.IsInProgress(quest.ID) && !Bot.Quests.CanComplete(quest.ID))
                        continue;

                    if (!Bot.Quests.IsInProgress(quest.ID))
                        Bot.Quests.Accept(quest.ID);

                    await Task.Delay(ActionDelay * 2);

                    if (Bot.Quests.CanComplete(quest.ID))
                    {
                        // Determine reward ID if quest is in the chooseQuests dictionary
                        int rewardId = -1;

                        if (chooseQuests.ContainsKey(quest))
                        {
                            Quest? activeQuest = Bot.Quests.Active.FirstOrDefault(q => q?.ID == quest.ID);
                            if (activeQuest != null)
                            {
                                ItemBase? reward = activeQuest.Rewards.FirstOrDefault(r => r != null && r.Quantity < r.MaxStack);
                                rewardId = reward?.ID ?? -1;
                            }
                        }

                        // Ensure quest is loaded, and is entirely completable.

                        // Send the quest completion packet
                        Bot.Send.Packet($"%xt%zm%tryQuestComplete%{Bot.Map.RoomID}%{quest.ID}%{rewardId}%false%{(quest.Once || !string.IsNullOrEmpty(quest?.Field) ? 1 : Bot.Flash.CallGameFunction<int>("world.maximumQuestTurnIns", quest!.ID))}%wvz%");

                        // Check if the quest is still in progress
                        await Task.Delay(ActionDelay * 2);
                        if (Bot.Quests.IsInProgress(quest.ID))
                            i++;

                        if (i >= 20 && Bot.Quests.IsInProgress(quest.ID))
                        {
                            await Task.Delay(ActionDelay * 2);
                            Bot.Flash.CallGameFunction("world.abandonQuest", quest.ID);
                            await Task.Delay(ActionDelay * 2);
                            Bot.Quests.Load(quest.ID);
                            await Task.Delay(ActionDelay * 2);
                            Bot.Quests.Accept(quest.ID);
                            i = 0;
                            continue;
                        }
                        await Task.Delay(ActionDelay * 2);
                        Bot.Quests.Accept(quest.ID);
                    }
                }
            }
            GC.Collect();
        });
        questCTS = new();
    }

    /// <summary>
    /// Cancels the current registered quests.
    /// </summary>
    public void CancelRegisteredQuests()
    {
        Bot.Lite.ReacceptQuest = false;
        if (questCTS != null)
        {
            questCTS?.Cancel();
            Bot.Wait.ForTrue(() => questCTS == null, 10);
        }
        if (Bot.Quests.Registered.Any())
        {
            Bot.Quests.UnregisterQuests(registeredQuests);
            AbandonQuest(registeredQuests);
        }
        registeredQuests = Array.Empty<int>();
    }
    private int[] registeredQuests = Array.Empty<int>();

    /// <summary>
    /// Ensures the quest is ready for acceptance by handling membership checks,
    /// unbanking required items, and adding them to the drop pickup list.
    /// </summary>
    /// <param name="questID">ID of the quest to accept</param>
    public bool EnsureAccept(int questID = 0)
    {
        Quest? QuestData = InitializeWithRetries(() => EnsureLoad(questID));
        if (QuestData == null)
        {
            Logger($"Failed to load quest with ID {questID} after multiple attempts.");
            return false;
        }

        if (QuestData.Upgrade && !IsMember)
            Logger($"\"{QuestData.Name}\" [{questID}] is member-only, stopping the bot.", stopBot: true);

        if (questID <= 0)
            return false;

        ItemBase[] requiredItems = QuestData.AcceptRequirements.Where(x => !x.Temp)
     .Concat(QuestData.Requirements.Where(x => !x.Temp))
     .Where(item => item != null && item.ID > 0)
     .ToArray();

        // Loop through the required items and add the Name if either the Name or the ID is not in CurrentDrops or ToPickupIDs
        requiredItems.ToList()
            .ForEach(item =>
            {
                // Check if either the Name or ID is not in the drops or pickup list
                if (item != null && (!Bot.Drops.ToPickup.Contains(item.Name) || !Bot.Drops.ToPickupIDs.Contains(item.ID)))
                {
                    // Add both ID and Name to the drop list if missing (ID is incase of duplicate names)
                    AddDrop(item.ID);
                    AddDrop(item.Name);
                }
            });

        Sleep(ActionDelay * 2);
        // Bot.Wait.ForActionCooldown(GameActions.AcceptQuest);
        // Bot.Send.Packet($"%xt%zm%acceptQuest%{Bot.Map.RoomID}%{questID}%");
        if (Bot.Quests.IsInProgress(questID))
            return true;
        else
        {
            Bot.Wait.ForActionCooldown(GameActions.AcceptQuest);
            Bot.Quests.EnsureAccept(questID);
            Bot.Wait.ForQuestAccept(questID);
            return true;
        }
    }

    /// <summary>
    /// Accepts all the quests given
    /// </summary>
    /// <param name="questIDs">IDs of the quests</param>
    public void EnsureAcceptmultiple(params int[]? questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
        {
            questIDs = new int[] { 0 }; // Default value
        }

        List<Quest>? QuestData = InitializeWithRetries(() => EnsureLoad(questIDs?.Where(q => q > 0).ToArray() ?? Array.Empty<int>()));
        if (QuestData == null)
        {
            Logger("Failed to load quests after multiple attempts.");
            return;
        }

        foreach (Quest quest in QuestData)
        {
            if (quest.Upgrade && !IsMember)
                Logger($"\"{quest.Name}\" [{quest.ID}] is member-only, stopping the bot.", stopBot: true);

            if (Bot.Quests.IsInProgress(quest.ID) || quest.ID <= 0)
                continue;

            string?[] requiredItemNames = quest.AcceptRequirements.Where(x => !x.Temp)
                .Concat(quest.Requirements.Where(x => !x.Temp))
                .Select(item => item?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToArray();

            foreach (string? itemName in requiredItemNames)
            {
                if (itemName != null && !Bot.Inventory.Contains(itemName))
                {
                    Unbank(itemName);
                }
            }

            // Loop through AcceptRequirements and Requirements for items to handle drops and pickup IDs
            var allItems = quest.AcceptRequirements.Concat(quest.Requirements).Where(x => !x.Temp).ToArray();

            allItems.ToList()
                .ForEach(item =>
                {
                    if (item == null) return;

                    // Check if either Name or ID is missing from CurrentDrops or ToPickupIDs
                    if (!Bot.Drops.ToPickup.Contains(item.Name) || !Bot.Drops.ToPickupIDs.Contains(item.ID))
                    {
                        // Add both ID and Name to the drop list if missing (ID is incase of duplicate names)
                        AddDrop(item.ID);
                        AddDrop(item.Name);
                    }
                });

            Sleep(ActionDelay * 2);
            // Bot.Send.Packet($"%xt%zm%acceptQuest%{Bot.Map.RoomID}%{quest.ID}%");
            Bot.Quests.EnsureAccept(quest.ID);
            Bot.Wait.ForActionCooldown(GameActions.AcceptQuest);
        }
    }

    /// <summary>
    /// Completes the quest with a choose-able reward item
    /// </summary>
    /// <param name="questID">ID of the quest to complete</param>
    /// <param name="itemID">ID of the choose-able reward item</param>
    public bool EnsureComplete(int questID, int itemID = -1)
    {
        if (questID <= 0)
            return false;

        Quest? questData = InitializeWithRetries(() => EnsureLoad(questID));
        if (questData == null)
        {
            Logger($"Failed to load quest with ID {questID} after multiple attempts.");
            return false;
        }

        if (!Bot.Drops.ToPickupIDs.Contains(itemID) && itemID > 0)
            Bot.Drops.Add(itemID);

        if (!Bot.Quests.IsInProgress(questID))
            EnsureAccept(questID);

        // Bot.Wait.ForTrue(() => questData != null, 20);
        if (questData != null && questData.Requirements != null
                        && (!questData.Requirements.Any()
                        || questData.Requirements.All(r => r != null && r.ID > 0)
                        && CheckInventory(questData.Requirements.Select(x => x.ID).ToArray())
                        && CheckInventory(questData.AcceptRequirements.Select(x => x.ID).ToArray())))
        {
            if (itemID == -1 && questData.SimpleRewards.Any())
            {
                var availableReward = questData.SimpleRewards
                    .FirstOrDefault(x => x.ID > 0 && !CheckInventory(x.ID, x.MaxStack, false));

                itemID = availableReward?.ID ?? -1;

                if (availableReward != null && itemID != -1 && !Bot.Drops.ToPickupIDs.Contains(itemID))
                    AddDrop(availableReward.ID);
            }
            return Bot.Quests.EnsureComplete(questID, itemID);

        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Completes all the quests given but doesn't support quests with choose-able rewards.
    /// </summary>
    /// <param name="questIDs">IDs of the quests.</param>
    public void EnsureComplete(params int[] questIDs)
    {
        List<Quest>? questData = InitializeWithRetries(() => EnsureLoad(questIDs));
        if (questData == null)
        {
            Logger("Failed to load quests after multiple attempts.");
            return;
        }

        foreach (Quest questID in questData)
        {
            if (questData == null)
                EnsureLoad(questID.ID);

            if (questData != null && questID.Requirements != null
                && (!questID.Requirements.Any()
                || questID.Requirements.All(r => r != null && r.ID > 0)
                && CheckInventory(questID.Requirements.Select(x => x.ID).ToArray())))
            {
                Bot.Quests.EnsureComplete(questID.ID);
                Bot.Wait.ForActionCooldown(GameActions.TryQuestComplete);
            }
        }
    }

    public bool HasSpace => Bot.Inventory.FreeSlots > 0;

    /// <summary>
    /// Completes a quest and chooses any item from it that you don't have (automatically accepts the drop).
    /// </summary>
    /// <param name="questID">ID of the quest</param>
    /// <param name="itemList">List of the items to get; if you want all, just let it be null.</param>
    public bool EnsureCompleteChoose(int questID, string[]? itemList = null)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questID));
        if (quest == null)
        {
            Logger($"Failed to load quest [{questID}] after multiple attempts.");
            return false;
        }

        bool hasAllItems = true;
        bool questCompleted = false;

        // Filter the rewards based on the itemList if provided
        IEnumerable<ItemBase> rewards = itemList == null
            ? quest.Rewards
            : quest.Rewards.Where(item => itemList.Contains(item.Name));

        foreach (ItemBase item in rewards)
        {
            if (CheckInventory(item.ID, item.MaxStack, false))
                continue;

            // Check if no space in inventory and item isn't in the inventory
            if (!HasSpace && !CheckInventory(item.ID, toInv: false))
            {
                Logger($"Skipping item \"{item.Name}\" from quest [{questID}] due to not having space, and it's not being in the inventory.");
                continue;
            }

            hasAllItems = false;

            if (!Bot.Quests.EnsureComplete(questID, item.ID))
                continue;

            Bot.Wait.ForQuestComplete(questID);

            if (!Bot.Drops.ToPickup.Contains(item.Name))
                Bot.Drops.Add(item.Name);

            if (Bot.Drops.Exists(item.ID))
                Bot.Drops.Pickup(item.ID);
            else if (Bot.Drops.Exists(item.Name))
                Bot.Drops.Pickup(item.Name);

            Bot.Wait.ForPickup(item.ID);
            questCompleted = true;
        }

        if (hasAllItems)
        {
            Logger($"Quest [{questID}] not completed. All rewards already owned.");
            return false;
        }

        if (!questCompleted)
        {
            Logger($"Could not complete quest [{questID}]. Some items may be missing or unavailable.\n" +
                string.Join("\n", quest.Rewards
                    .Where(x => x.Temp ? Bot.TempInv.Contains(x.ID) : !Bot.Inventory.Contains(x.ID))
                    .Select(x => $"\"{x.Name}\"")));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Completes the quest with a choose-able reward item
    /// </summary>
    /// <param name="questID">ID of the quest to complete</param>
    /// <param name="amount">Amount of times you want it to turn in the quest, -1 is the maximum amount possible.</param>
    /// <param name="itemID">ID of the choose-able reward item</param>
    public int EnsureCompleteMulti(int questID, int amount = -1, int itemID = -1)
    {
        //idk why but it wants `var` not `Quest`.. and it just works :|
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questID));

        if (quest == null)
        {
            Logger($"Quest {questID} not loaded after 5 attempts.");
            return 0;
        }

        if (quest != null && !Bot.Quests.IsInProgress(questID))
            EnsureAccept(questID);
        Bot.Wait.ForTrue(() => Bot.Quests.IsInProgress(questID), 20);

        int turnIns;
        if (quest != null)
        {
            string[] requiredItemNames =
            quest.Requirements.Concat(quest.AcceptRequirements)
            .Select(item => item.Name).ToArray();

            if (quest.Once || !string.IsNullOrEmpty(quest.Field))
            {
                turnIns = 1;
            }
            else
            {
                int possibleTurnin = Bot.Flash.CallGameFunction<int>("world.maximumQuestTurnIns", questID);
                turnIns = possibleTurnin > amount && amount > 0 ? amount : possibleTurnin;
                if (turnIns == 0)
                {
                    return 0;
                }
            }


            // Ensure quest is loaded, and is entirely completable.
            if (EnsureAccept(questID) && CheckInventory(requiredItemNames))
            {
                Bot.Flash.CallGameFunction("world.tryQuestComplete", questID, itemID, false, turnIns);
            }

            Bot.Wait.ForQuestComplete(questID);
            Bot.Wait.ForQuestAccept(questID);

            return !Bot.Quests.IsInProgress(questID) ? turnIns : 0;
        }
        else
        {
            Logger($"Failed to get the Quest Object for questID {questID}");
            return 0;
        }
    }

    public Quest EnsureLoad(int questID)
    {
        Quest? toReturn = Bot.Quests.Tree.Find(x => x.ID == questID) ?? _EnsureLoad1() ?? _EnsureLoad2();
        if (toReturn == null)
        {
            Bot.Quests.Load(questID);
            toReturn = Bot.Quests.Tree.Find(x => x.ID == questID) ?? _EnsureLoad1() ?? _EnsureLoad2();

            if (toReturn == null)
            {
                toReturn = EnsureLoadFromFile(questID).Result?.FirstOrDefault();

                if (toReturn == null)
                {
                    Logger($"Failed to get the Quest Object for questID {questID}" + reinstallCleanFlash, "EnsureLoad A.0", messageBox: true, stopBot: true);
                    return new();
                }
            }
        }

        return toReturn;

        Quest? _EnsureLoad1()
        {
            Sleep();
            Bot.Wait.ForTrue(() => Bot.Quests.Tree.Contains(x => x.ID == questID), () => Bot.Quests.Load(questID), 20);
            return Bot.Quests.Tree.Find(q => q.ID == questID)!;
        }
        Quest? _EnsureLoad2()
        {
            Sleep();
            return Bot.Quests.EnsureLoad(questID);
        }
    }

    public List<Quest> EnsureLoad(params int[] questIDs)
    {
        List<Quest>? quests = Bot.Quests.Tree.Where(x => questIDs.Contains(x.ID)).ToList();
        if (quests.Count == questIDs.Length)
            return quests;

        List<int> missing = questIDs.Where(x => !quests.Any(y => y.ID == x)).ToList();
        Sleep();
        for (int i = 0; i < missing.Count; i += 30)
        {
            Bot.Quests.Load(missing.ToArray()[i..(missing.Count > i ? missing.Count : i + 30)]);
            Sleep(1500);
        }
        Bot.Wait.ForTrue(() => questIDs.All(id => Bot.Quests.Tree.Any(q => q.ID == id)), 20);

        List<Quest>? toReturn = Bot.Quests.Tree.Where(x => questIDs.Contains(x.ID)).ToList();
        if (toReturn == null || !toReturn.Any())
        {
            toReturn = EnsureLoadFromFile(questIDs).Result;
            if (toReturn == null || !toReturn.Any())
            {
                Logger($"Failed to get the Quest Object for questIDs {string.Join(" | ", questIDs)}" + reinstallCleanFlash, "EnsureLoad B.4", messageBox: true, stopBot: true);
                return new();
            }
        }

        return toReturn;
    }

    private async Task<List<Quest>?> EnsureLoadFromFile(params int[] questIDs)
    {
        List<Quest>? toReturn;
        //First try local Quest.txt file(if its not too old)
        if (File.GetLastWriteTime(ClientFileSources.SkuaQuestsFile).Subtract(DateTime.Now).TotalDays < 14 && LoadLocal())
            return toReturn!;

        // Otherwise try file on Github
        toReturn = (OnlineQuestsFile ??=
                        JsonConvert.DeserializeObject<List<QuestData>?>(
                            GetRequest("https://raw.githubusercontent.com/BrenoHenrike/Scripts/Skua/QuestData.json")))?
                    .Where(q => questIDs.Contains(q.ID)).Select(q => toQuest(q)).ToList();
        if (toReturn != null && toReturn.Any() && questIDs.All(q => toReturn.Any(x => x.ID == q)))
            return toReturn;

        // If Github failed, manually update the quest file 
        await UpdateQuestFile();
        if (LoadLocal())
            return toReturn!;

        // Failure
        Logger($"Failed to get the Quest Object for questIDs {string.Join(" | ", questIDs)}", "EnsureLoad C.0", messageBox: true, stopBot: true);
        return null;

        bool LoadLocal()
        {
            toReturn = (LocalQuestsFile ??= JsonConvert.DeserializeObject<List<QuestData>?>(File.ReadAllText(ClientFileSources.SkuaQuestsFile)))?
                .Where(q => questIDs.Contains(q.ID)).Select(q => toQuest(q)).ToList();
            return (toReturn != null && toReturn.Any() && questIDs.All(q => toReturn.Any(x => x.ID == q)));
        }


        Quest toQuest(QuestData data)
        {
            return new Quest()
            {
                ID = data.ID,
                Slot = data.Slot,
                Value = data.Value,
                Name = data.Name,
                Description = string.Empty, // Not found in QuestData
                EndText = string.Empty, // Not found in QuestData
                Once = data.Once,
                Field = data.Field,
                Index = data.Index,
                Upgrade = data.Upgrade,
                Level = data.Level,
                RequiredClassID = data.RequiredClassID,
                RequiredClassPoints = data.RequiredClassPoints,
                RequiredFactionId = data.RequiredFactionId,
                RequiredFactionRep = data.RequiredFactionRep,
                Gold = data.Gold,
                XP = data.XP,
                Status = null!, // Not found in QuestData
                                //Active is based on Status being NULL or not
                                //Requirements cant be writen to
                Rewards = data.Rewards,
                SimpleRewards = data.SimpleRewards,
            };
        }
        async Task UpdateQuestFile()
        {
            CancellationTokenSource? _loaderCTS;
            _loaderCTS = new();
            List<QuestData> questData =
                await (LoaderService ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>())
                .UpdateAsync("Quests.txt", false, null, _loaderCTS.Token);
            _loaderCTS.Dispose();
            _loaderCTS = null;
        }
    }
    private List<QuestData>? LocalQuestsFile;
    private List<QuestData>? OnlineQuestsFile;
    private IQuestDataLoaderService? LoaderService;

    public void AbandonQuest(params int[] questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
            return;

        foreach (Quest q in EnsureLoad(questIDs))
        {
            if (q == null || !q.Active)
                continue;
            Bot.Flash.CallGameFunction("world.abandonQuest", q.ID);
            Bot.Wait.ForTrue(() => !EnsureLoad(q.ID).Active, 20);
            Bot.Quests.UnregisterQuests(q.ID);
        }
    }
    /// <summary>
    /// Retrieves the quest reward names for the specified quest IDs.
    /// </summary>
    /// <param name="questIDs">
    /// The quest IDs for which to retrieve the reward names.
    /// </param>
    /// <returns>
    /// An array of reward names (strings) for the specified quest IDs.
    /// </returns>
    public string[] QuestRewards(params int[] questIDs)
    {
        if (questIDs.Length == 0)
            return Array.Empty<string>();
        List<string> toReturn = new();
        foreach (Quest q in EnsureLoad(questIDs))
        {
            if (q.Rewards == null || q.Rewards.Count == 0)
                continue;
            toReturn.AddRange(q.Rewards.Select(i => i.Name));
        }
        return toReturn.ToArray();
    }

    /// <summary>
    /// Retrieves the quest reward IDs for the specified quest IDs.
    /// </summary>
    /// <param name="questIDs">
    /// The quest IDs for which to retrieve the reward IDs.
    /// </param>
    /// <returns>
    /// An array of reward IDs (integers) for the specified quest IDs.
    /// </returns>
    public int[] QuestRewardsInt(params int[] questIDs)
    {
        if (questIDs.Length == 0)
            return Array.Empty<int>();
        List<int> toReturn = new();
        foreach (Quest q in EnsureLoad(questIDs))
        {
            if (q.Rewards == null || q.Rewards.Count == 0)
                continue;
            toReturn.AddRange(q.Rewards.Select(x => x.ID).ToArray());
        }
        return toReturn.ToArray();
    }


    /// <summary>
    /// Retrieves the quest requirements for the specified quest IDs, based on the type parameter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result. Can be either <see cref="string"/> or <see cref="int"/>.
    /// </typeparam>
    /// <param name="questIDs">
    /// The quest IDs for which to retrieve the requirements.
    /// </param>
    /// <returns>
    /// An array of the specified type containing the quest requirements.
    /// For <see cref="string"/>, it returns an array of the requirement names.
    /// For <see cref="int"/>, it returns an array of the requirement IDs.
    /// </returns>
    public T[] QuestRequirements<T>(params int[] questIDs)
    {
        if (questIDs.Length == 0)
            return Array.Empty<T>();

        List<T> toReturn = new();

        foreach (Quest q in EnsureLoad(questIDs))
        {
            if (q == null || q.Requirements == null || q.Requirements.Count == 0)
            {
                toReturn.Add(typeof(T) == typeof(string) ? (T)(object)string.Empty : (T)(object)default(int));
                continue;
            }

            if (typeof(T) == typeof(string))
                toReturn.AddRange(q.Requirements.Where(x => x != null).Select(x => (T)(object)x.Name));
            else if (typeof(T) == typeof(int))
                toReturn.AddRange(q.Requirements.Select(x => (T)(object)x.ID));
        }

        return toReturn.ToArray();
    }


    /// <summary>
    /// Accepts and then completes the quest, used inside a loop
    /// </summary>
    /// <param name="questID">ID of the quest</param>
    /// <param name="itemID">ID of the choose-able reward item</param>
    public void ChainComplete(int questID, int itemID = -1)
    {
        if (itemID > 0)
            Bot.Drops.Add(itemID);

        Quest? QuestData = InitializeWithRetries(() => EnsureLoad(questID));

        ItemBase? Item = Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(x => x != null && x.ID == itemID);

        // EnsureAccept(questID);
        // Sleep();
        EnsureCompleteMulti(questID, itemID: itemID);
    }

    /// <summary>
    /// Checks if a quest has been completed before.
    /// </summary>
    /// <param name="QuestID">The ID of the quest to check.</param>
    /// <returns>True if the quest is completed, otherwise false.</returns>
    public bool isCompletedBefore(int QuestID)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(QuestID));
        if (quest == null)
        {
            Logger($"❌ Failed to initialize quest {QuestID} after multiple attempts.");
            return false;
        }

        string questName = quest.Name ?? $"{QuestID}";

        bool CheckCompletion(Quest questData)
        {
            bool complete = questData.Slot < 0 ||
                Bot.Flash.CallGameFunction<int>("world.getQuestValue", questData.Slot) >= questData.Value;

            // Commented out to reduce spam
            // Logger($"{questName} [{QuestID}] completion check [{(complete ? '✔' : '❌')}]");
            return complete;
        }

        try
        {
            return CheckCompletion(quest);
        }
        catch
        {
            quest = InitializeWithRetries(() => EnsureLoad(QuestID));
            if (quest == null)
            {
                Logger($"❌ Failed to reinitialize {questName} [{QuestID}] after multiple attempts.");
                return false;
            }
            return CheckCompletion(quest);
        }
    }


    #region Backups - from 2022
    /// <summary>
    /// This will register quests to be completed while doing something else, i.e. while in combat.
    /// If it has quests already registered, it will cancel them first and then register the new quests.
    /// </summary>
    /// <param name="questIDs">ID of the quests to be completed.</param>
    public void RegisterQuestsOld(params int[] questIDs)
    {
        if (questCTS is not null)
            CancelRegisteredQuests();

        // Defining all the lists to be used=
        List<Quest> questData = EnsureLoad(questIDs);
        Dictionary<Quest, int> chooseQuests = new();
        Dictionary<Quest, int> nonChooseQuests = new();

        foreach (Quest q in questData)
        {
            bool shouldBreak = false;
            // Removing quests that you can't accept
            foreach (ItemBase req in q.AcceptRequirements)
            {
                if (!CheckInventory(req.Name))
                {
                    Logger($"Missing requirement {req.Name} for \"{q.Name}\" [{q.ID}]");
                    shouldBreak = true;
                    break;
                }
            }
            if (shouldBreak)
                break;

            // Separating the quests into choose and non-choose
            if (q.SimpleRewards.Any(r => r.Type == 2))
                chooseQuests.Add(q, 1);
            else
                nonChooseQuests.Add(q, 1);
        }

        EnsureAcceptOld(questIDs);
        questCTS = new();
        Task.Run(async () =>
        {
            while (!Bot.ShouldExit && !questCTS.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(ActionDelay * 2);

                    // Quests that dont need a choice
                    foreach (KeyValuePair<Quest, int> kvp in nonChooseQuests)
                    {
                        if (Bot.Quests.CanComplete(kvp.Key.ID))
                        {
                            int amountTurnedIn = EnsureCompleteMultiOld(kvp.Key.ID);
                            if (amountTurnedIn == 0)
                                continue;
                            await Task.Delay(ActionDelay * 2);
                            EnsureAcceptOld(kvp.Key.ID);
                            Logger($"Quest completed x{nonChooseQuests[kvp.Key] + amountTurnedIn} times: [{kvp.Key.ID}] \"{kvp.Key.Name}\"");
                        }
                    }

                    // Quests that need a choice
                    foreach (KeyValuePair<Quest, int> kvp in chooseQuests)
                    {
                        if (Bot.Quests.CanComplete(kvp.Key.ID))
                        {
                            // Finding the next item that you dont have max stack of yet
                            List<SimpleReward> simpleRewards =
                                kvp.Key.SimpleRewards.Where(r => r.Type == 2 &&
                                                            (!Bot.Inventory.IsMaxStack(r.Name) ||
                                                                            !(Bot.Bank.TryGetItem(r.Name, out InventoryItem? item) && item != null && item.Quantity >= r.MaxStack))).ToList(); if (simpleRewards.Count == 0)
                            {
                                EnsureCompleteOld(kvp.Key.ID);
                                await Task.Delay(ActionDelay * 2);
                                EnsureAcceptOld(kvp.Key.ID);
                                continue;
                            }

                            Bot.Drops.Add(kvp.Key.Rewards.Where(x => simpleRewards.Any(t => t.ID == x.ID)).Select(i => i.Name).ToArray());
                            EnsureCompleteOld(kvp.Key.ID, simpleRewards.First().ID);
                            await Task.Delay(ActionDelay * 2);
                            EnsureAcceptOld(kvp.Key.ID);
                            Logger($"Quest completed x{chooseQuests[kvp.Key]++} times: [{kvp.Key.ID} \"{kvp.Key.Name}\" (got {kvp.Key.Rewards.First(x => x.ID == simpleRewards.First().ID).Name}])");

                        }
                    }
                }
                catch { }
            }
            questCTS = null;
        });
    }

    /// <summary>
    /// Ensures you are out of combat before accepting the quest
    /// </summary>
    /// <param name="questID">ID of the quest to accept</param>
    public bool EnsureAcceptOld(int questID)
    {
        Quest QuestData = EnsureLoad(questID);

        if (QuestData.Upgrade && !IsMember)
            Logger($"\"{QuestData.Name}\" [{questID}] is member-only, stopping the bot.", stopBot: true);

        if (Bot.Quests.IsInProgress(questID))
            return true;
        if (questID <= 0)
            return false;

        Bot.Drops.Add(QuestData.Requirements.Where(x => !x.Temp).Select(y => y.Name).ToArray());
        Bot.Sleep(ActionDelay);
        return Bot.Quests.EnsureAccept(questID);
    }

    /// <summary>
    /// Accepts all the quests given
    /// </summary>
    /// <param name="questIDs">IDs of the quests</param>
    public void EnsureAcceptOld(params int[] questIDs)
    {
        List<Quest> QuestData = EnsureLoad(questIDs);
        foreach (Quest quest in QuestData)
        {
            if (quest.Upgrade && !IsMember)
                Logger($"\"{quest.Name}\" [{quest.ID}] is member-only, stopping the bot.", stopBot: true);

            if (Bot.Quests.IsInProgress(quest.ID) || quest.ID <= 0)
                continue;

            Bot.Drops.Add(quest.Requirements.Where(x => !x.Temp).Select(y => y.Name).ToArray());
            Bot.Sleep(ActionDelay);
            Bot.Quests.EnsureAccept(quest.ID);
        }
    }

    /// <summary>
    /// Completes the quest with a choose-able reward item
    /// </summary>
    /// <param name="questID">ID of the quest to complete</param>
    /// <param name="itemID">ID of the choose-able reward item</param>
    public bool EnsureCompleteOld(int questID, int itemID = -1)
    {
        if (questID <= 0)
            return false;
        Bot.Sleep(ActionDelay);
        return Bot.Quests.EnsureComplete(questID, itemID);
    }

    /// <summary>
    /// Completes all the quests given but doesn't support quests with choose-able rewards
    /// </summary>
    /// <param name="questIDs">IDs of the quests</param>
    public void EnsureCompleteOld(params int[] questIDs)
    {
        Bot.Quests.EnsureComplete(questIDs);
    }

    /// <summary>
    /// Completes a quest and choose any item from it that you don't have (automatically accepts the drop)
    /// </summary>
    /// <param name="questID">ID of the quest</param>
    /// <param name="itemList">List of the items to get, if you want all just let it be null</param>
    public bool EnsureCompleteChooseOld(int questID, string[]? itemList = null)
    {
        if (questID <= 0)
            return false;
        Bot.Sleep(ActionDelay);
        Quest quest = EnsureLoad(questID);
        if (quest is not null)
        {
            foreach (ItemBase item in quest.Rewards)
            {
                if (!CheckInventory(item.Name, toInv: false)
                    && (itemList == null || (itemList != null && itemList.Contains(item.Name))))
                {
                    bool completed = Bot.Quests.EnsureComplete(questID, item.ID);
                    Bot.Drops.Pickup(item.Name);
                    Bot.Wait.ForPickup(item.Name);
                    return completed;
                }
            }
        }
        else
        {
            Logger($"Failed to load Quest {questID}, EnsureCompleteChoose failed");
            return false;
        }
        Logger($"Could not complete the quest {questID}. Maybe all items are already in your inventory");
        return false;
    }

    /// <summary>
    /// Completes the quest with a choose-able reward item
    /// </summary>
    /// <param name="questID">ID of the quest to complete</param>
    /// <param name="amount">Amount of times you want it to turn in the quest, -1 is maximum amount possible.</param>
    /// <param name="itemID">ID of the choose-able reward item</param>
    public int EnsureCompleteMultiOld(int questID, int amount = -1, int itemID = -1)
    {
        var q = EnsureLoad(questID);

        int turnIns = 0;
        if (q.Once || !String.IsNullOrEmpty(q.Field))
            turnIns = 1;
        else
        {
            int possibleTurnin = Bot.Flash.CallGameFunction<int>("world.maximumQuestTurnIns", questID);
            turnIns = possibleTurnin > amount && amount > 0 ? amount : possibleTurnin;
            if (turnIns == 0)
                return 0;
        }
        Bot.Flash.CallGameFunction("world.tryQuestComplete", questID, itemID, false, turnIns);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForQuestComplete(questID);
        return !Bot.Quests.IsInProgress(questID) ? turnIns : 0;
    }
    #endregion Backups - from 2022

    #endregion

    #region Fixed KillMonster/HuntMonster

    private void AttackMonster(Monster monster)
    {
        var currentCellMonsters = Bot.Monsters.GetMonstersByCell(Bot.Player.Cell);
        var priorityMonsters = currentCellMonsters
            .Where(m => string.Equals(m.Name, monster.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (priorityMonsters.Any())
        {
            foreach (var targetMonster in priorityMonsters)
            {
                if (Bot.ShouldExit) break;

                Bot.Combat.Attack(targetMonster);
                Bot.Wait.ForMonsterDeath();

                if (Bot.Player.HasTarget)
                    Bot.Combat.CancelTarget();

                Bot.Sleep(100);
            }
        }
        else
        {
            Bot.Sleep(250);
        }
    }

    public void JumpToBestCell(string[] targetMonsters, string pad = "Left", bool setSpawn = true)
    {
        var monsterCells = new Dictionary<string, int>();

        foreach (var cell in Bot.Map.Cells)
        {
            var monstersInCell = Bot.Monsters.GetMonstersByCell(cell);
            int count = monstersInCell.Count(m => m.Alive && targetMonsters.Contains(m.Name));

            if (count > 0)
                monsterCells[cell] = count;
        }

        if (!monsterCells.Any())
            return;

        var bestCell = monsterCells.OrderByDescending(kvp => kvp.Value).First().Key;

        if (Bot.Player.Cell != bestCell)
        {
            Bot.Map.Jump(bestCell, pad);
            Bot.Wait.ForCellChange(bestCell);

            if (setSpawn)
                Bot.Player.SetSpawnPoint();
        }
    }

    public void KillMonster(string map, string cell, string pad, string monster, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        if (Bot.Map.Name != map)
        {
            Join(map, publicRoom: publicRoom);
            Bot.Wait.ForMapLoad(map);
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
        }

        if (!Bot.Map.Cells.Any(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)))
            cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;

        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad);
            Bot.Wait.ForCellChange(cell);
        }

        if (item != null && !isTemp)
            AddDrop(item);

        List<Monster> FindMonsters()
        {
            if (Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad);
                Bot.Wait.ForCellChange(cell);
            }

            if (monster == "*")
                return Bot.Monsters.MapMonsters.Where(x => x != null && x.Cell == cell).ToList();

            return Bot.Monsters.MapMonsters.Where(x => x != null && x.Cell == cell && x.Name.FormatForCompare() == monster.FormatForCompare()).ToList();
        }

        Bot.Options.AggroAllMonsters = false;

        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Any(x => x != Bot.Player.Username))
        {
            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;
        }
        else
        {
            Bot.Options.AggroMonsters = false;
        }

        var targetMonsters = FindMonsters();

        if (targetMonsters == null || !targetMonsters.Any())
        {
            Logger($"Monster {monster} not found in cell {cell}, pad {pad} in /{map}");
            return;
        }

        if (item == null)
        {
            foreach (var targetMonster in targetMonsters)
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Sleep();

                AttackMonster(targetMonster);
            }
        }
        else
        {
            while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            {
                foreach (var targetMonster in targetMonsters)
                {
                    while (!Bot.ShouldExit && !Bot.Player.Alive)
                        Sleep();

                    AttackMonster(targetMonster);

                    if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
                        break;
                }
            }
        }

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        string? targetCell = Bot.Map.Cells
            .Where(c => c != null && !BlackListedJumptoCells.Contains(c) && !Bot.Monsters.MapMonsters.Any(m => m != null && m.Cell == c))
            .FirstOrDefault(c => Bot.Map.Cells.Count(cell => cell.Contains("Enter")) > 1 || !c.Contains("Enter")) ?? "Enter";

        Bot.Map.Jump(targetCell, targetCell == "Enter" ? "Spawn" : "Left");
        Bot.Wait.ForCellChange(targetCell);
        Sleep();
        JumpWait();
        Rest();
        Bot.Options.HidePlayers = false;
    }

    public void KillMonster(string map, string cell, string pad, int MonsterMapID, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());
        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;

        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        if (log)
            FarmingLogger(item, quant);

        if (item != null && !isTemp)
            AddDrop(item);

        if (Bot.Map.Name != map)
            Join(map, cell, pad, publicRoom: publicRoom);

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad);
            Bot.Wait.ForCellChange(cell);
        }

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        List<Monster> FindMonsters()
        {
            return Bot.Monsters.MapMonsters.Where(m => m != null && m.Cell == cell && (m.MapID == MonsterMapID || m.ID == MonsterMapID)).ToList();
        }

        var targetMonsters = FindMonsters();

        if (!targetMonsters.Any())
        {
            if (log) Logger($"No monsters with ID {MonsterMapID} found in cell {cell}.");
            return;
        }

        if (item == null)
        {
            foreach (var targetMonster in targetMonsters)
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Sleep();

                AttackMonster(targetMonster);
            }
        }
        else
        {
            while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            {
                foreach (var targetMonster in targetMonsters)
                {
                    while (!Bot.ShouldExit && !Bot.Player.Alive)
                        Sleep();

                    AttackMonster(targetMonster);

                    if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
                        break;
                }
            }
        }

        if (item != null)
            Bot.Wait.ForPickup(item);

        Bot.Options.AttackWithoutTarget = false;
    }

    public void KillMonster(string map, string cell, string pad, int MonsterMapID, int ItemID = 0, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());
        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;

        if (ItemID != 0 && (isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant)))
            return;

        if (ItemID != 0 && !isTemp)
            AddDrop(ItemID);

        if (Bot.Map.Name != map)
            Join(map, cell, pad, publicRoom: publicRoom);

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad);
            Bot.Wait.ForCellChange(cell);
        }

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        List<Monster> FindMonsters()
        {
            return Bot.Monsters.MapMonsters.Where(m => m != null && m.Cell == cell && (m.MapID == MonsterMapID || m.ID == MonsterMapID)).ToList();
        }

        var targetMonsters = FindMonsters();

        if (!targetMonsters.Any())
        {
            if (log) Logger($"No monsters with ID {MonsterMapID} found in cell {cell}.");
            return;
        }

        if (ItemID == 0)
        {
            foreach (var targetMonster in targetMonsters)
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Sleep();

                AttackMonster(targetMonster);
            }
        }
        else
        {
            while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant)))
            {
                foreach (var targetMonster in targetMonsters)
                {
                    while (!Bot.ShouldExit && !Bot.Player.Alive)
                        Sleep();

                    AttackMonster(targetMonster);

                    if (isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant))
                        break;
                }
            }
        }

        if (ItemID != 0)
            Bot.Wait.ForPickup(ItemID);

        Bot.Options.AttackWithoutTarget = false;
    }

    public void HuntMonster(string map, string monster, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        Join(map, publicRoom: publicRoom);

        Bot.Wait.ForMapLoad(map);

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        if (item is not null && !isTemp)
            AddDrop(item);

        Monster? FindMonster()
        {
            return Bot.Monsters.MapMonsters.Find(x => x != null && x.Name.FormatForCompare() == monster.FormatForCompare());
        }

        Monster? targetMonster = FindMonster();

        if (targetMonster == null)
        {
            Logger($"Monster {monster} not found in /{map}.");
            return;
        }

        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Where(x => x != Bot.Player.Username).Any())
        {
            Bot.Options.AggroMonsters = true;
            //hide players to reduce lag (Trust Tato)
            Bot.Options.HidePlayers = true;
        }
        else Bot.Options.AggroMonsters = false;

        if (item == null)
        {
            while (!Bot.ShouldExit && Bot.Player.Cell != targetMonster.Cell)
            {
                // Jump(targetMonster.Cell, "Left");
                JumpToBestCell(new string[] { targetMonster.Name });
                Bot.Wait.ForCellChange(targetMonster.Cell);
                Bot.Player.SetSpawnPoint();
            }
            AttackMonster(targetMonster);
            JumpWait();
            Rest();
        }
        else
        {
            if (log)
                FarmingLogger(item, quant);

            //bool ded = false;
            //Bot.Events.MonsterKilled += b => ded = true;
            while (!Bot.ShouldExit || isTemp ? !Bot.TempInv.Contains(item, quant) : !CheckInventory(item, quant))
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Sleep();
                if (Bot.Player.Cell != targetMonster.Cell)
                    JumpToBestCell(new string[] { targetMonster.Name });
                // Jump(targetMonster.Cell);
                AttackMonster(targetMonster);
                Sleep();
                if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
                    break;
            }
            Bot.Options.AttackWithoutTarget = false;
            ToggleAggro(false);
            /*Bot.Map.Jump(
                Bot.Map.Cells.FirstOrDefault(c => c.ToLower().Contains("enter"))
                ?? Bot.Map.Cells.FirstOrDefault(c => !c.ToLower().Contains("wait") && !c.ToLower().Contains("blank") && !c.ToLower().Contains("enter"))
                ?? "Enter",
                "Spawn"
            );*/
            Bot.Options.AggroMonsters = false;
            JumpWait();
            Rest();
            Bot.Options.HidePlayers = false;

        }
    }

    #endregion
        
    #region Kill

    /// <summary>
    /// Kills a monster using it's MapID
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="monsterMapID"></param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="item">Item to kill the monster for, if null will just kill the monster 1 time</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log">Whether it will log that it is killing the monster</param>
    /// <param name="publicRoom"></param>
    public void HuntMonsterMapID(string map, int monsterMapID, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false, string pad = "Left")
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        Join(map, publicRoom: publicRoom);

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        Monster? FindMonster()
        {
            return Bot.Monsters.MapMonsters.FirstOrDefault(m => m != null && (m.MapID == monsterMapID || m.ID == monsterMapID));
        }

        Monster? targetMonster = FindMonster();

        if (log && item != null)
            FarmingLogger(item, quant);

        if (targetMonster == null)
        {
            Logger($"Monster with MapID {monsterMapID} not found in /{map}.");
            return;
        }

        if (item == null)
        {
            while (!Bot.ShouldExit && Bot.Player.Cell != targetMonster.Cell)
            {
                Jump(targetMonster.Cell, pad);
                Bot.Wait.ForCellChange(targetMonster.Cell);
                Bot.Player.SetSpawnPoint();
            }
            Bot.Kill.Monster(targetMonster);
            JumpWait();
            Rest();
        }
        else
        {
            if (!isTemp)
                AddDrop(item);

            ItemBase? Item = Bot.Inventory.Items.Concat(Bot.Bank.Items).Concat(Bot.House.Items).FirstOrDefault(x => x != null && x.Name == item);
            if (Item != null && Item.Quantity == Item.MaxStack)
                Bot.Drops.Remove(Item.ID);

            bool ded = false;
            Bot.Events.MonsterKilled += b => ded = true;
            while (!Bot.ShouldExit && !ded || isTemp ? !Bot.TempInv.Contains(item, quant) : !CheckInventory(item, quant))
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Sleep();
                if (Bot.Map.Name != map)
                    Join(map);
                if (Bot.Player.Cell != targetMonster.Cell)
                    Jump(targetMonster.Cell);
                Bot.Combat.Attack(targetMonster);
                Sleep();
                if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
                    break;
            }
            if (Bot.Options.RestPackets)
                Rest();
            Bot.Wait.ForPickup(item);
        }
    }

    //Non-Choose Variants

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest and an optional array of map and monster names.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="MapMonsterClassPairs">Array of map name, monster name, and class type tuples.</param>
    public void HuntMonsterQuest(int questId, params (string mapName, string monsterName, ClassType classType)[] MapMonsterClassPairs)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"Failed to load quest with ID [{questId}] after multiple attempts.", stopBot: true);
            return;
        }

        var itemsToUnbank = quest.AcceptRequirements
                                .Concat(quest.Requirements)
                                .Select(x => x.ID)
                                .Distinct()
                                .ToArray();

        Unbank(itemsToUnbank);

        // Add the non-temp items to the drop pickup list
        Bot.Drops.Add(quest.AcceptRequirements.Concat(quest.Requirements)
                                .Where(x => x != null && !x.Temp)
                                .Select(x => x.ID)
                                .Distinct()
                                .ToArray());



        // If no MapMonsterClassPairs are provided, auto-generate default values
        if (MapMonsterClassPairs.Length == 0)
        {
            MapMonsterClassPairs = quest.Requirements
                .Select(_ => ("Fill ME", "Fill ME", ClassType.Solo))
                .ToArray();
        }

        for (int i = 0; i < MapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            var (mapName, monsterName, classType) = MapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Equip the class before hunting
            EquipClass(classType);

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            HuntMonster(mapName ?? Bot.Map.Name, monsterName ?? "*", requirement.Name ?? string.Empty, requirement.Quantity, requirement.Temp);
        }

        if (!Bot.Quests.EnsureComplete(questId))
            EnsureCompleteMulti(questId);
    }

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest and optional map and monster names for each requirement.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="mapName">An optional map name for the hunt.</param>
    /// <param name="monsterName">An optional monster name for the hunt.</param>
    /// <param name="log">Whether to log the hunting process.</param>
    public void HuntMonsterQuest(int questId, string? mapName = null, string? monsterName = null, bool log = false)
    {
        Quest? quest = Bot.Quests.EnsureLoad(questId);
        if (quest == null)
        {
            Logger($"Quest {questId} not found");
            return;
        }

        // Combine all requirements into one list for reusability
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Ensure that there are requirements to hunt
        if (allRequirements.Count == 0)
        {
            Logger($"Quest {questId} has no requirements.");
            return;
        }

        // Unbank the required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        // Add non-temp items to the drop list
        Bot.Drops.Add(allRequirements
            .Where(x => !x.Temp)
            .Select(x => x.ID)
            .Distinct()
            .ToArray());

        // Process each requirement for hunting
        foreach (var requirement in quest.Requirements)
        {
            // Use the provided map and monster names, or fall back to default values
            string huntMapName = mapName ?? Bot.Map.Name;
            string huntMonsterName = monsterName ?? "*";

            if (!Bot.Quests.EnsureAccept(questId))
                EnsureAccept(questId);

            HuntMonster(huntMapName, huntMonsterName, requirement.Name ?? "", requirement.Quantity, requirement.Temp, log);
        }

        // Ensure quest completion if possible
        if (Bot.Quests.CanCompleteFullCheck(questId))
        {
            EnsureCompleteMulti(questId);
        }
    }

    //Choose Variants - String

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest and an optional array of map and monster names. 
    /// Always chooses a reward upon quest completion.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="reward">The name of the reward to choose (if applicable).</param>
    /// <param name="mapMonsterClassPairs">Array of tuples specifying map names, monster names, and class types.</param>
    public void HuntMonsterQuestChoose(int questId, string? reward = null, params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"Failed to load quest with ID [{questId}] after multiple attempts.", stopBot: true);
            return;
        }

        // Ensure quest.Requirements and quest.Rewards are not null or empty
        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        // Combine quest.AcceptRequirements and quest.Requirements
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Add reward to itemsToUnbank if specified
        if (!string.IsNullOrEmpty(reward))
        {
            allRequirements.AddRange(quest.Rewards
                .Where(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)));
        }

        // Unbank required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        // Add non-temp items to the drop pickup list
        Bot.Drops.Add(allRequirements
            .Where(x => x != null && !x.Temp)
            .Select(x => x.Name)
            .Distinct()
            .ToArray());


        // Auto-generate default map-monster-class pairs if not provided
        if (mapMonsterClassPairs.Length == 0)
        {
            mapMonsterClassPairs = quest.Requirements
                .Select(_ => ("default_map", "default_monster", ClassType.Solo))
                .ToArray();
        }
        else if (mapMonsterClassPairs.Length > quest.Requirements.Count)
        {
            Logger($"Warning: More map-monster-class pairs provided than quest requirements. Extra pairs will be ignored.", stopBot: false);
        }

        // Process each map-monster-class pair
        for (int i = 0; i < mapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            (string mapName, string monsterName, ClassType classType) = mapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Ensure the quest is accepted if not in progress
            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            // Equip the appropriate class and hunt the monster
            EquipClass(classType);
            HuntMonster(mapName, monsterName, requirement.Name, requirement.Quantity, requirement.Temp);
        }

        // Complete the quest, retrying if necessary
        if (!Bot.Quests.EnsureComplete(questId, reward != null
                    ? quest.Rewards.FirstOrDefault(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))?.ID ?? -1
                    : -1))
        {
            EnsureCompleteMulti(
                questId,
                itemID: reward != null
                    ? quest.Rewards.FirstOrDefault(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))?.ID ?? -1
                    : -1
            );
        }
    }

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest with optional map, monster names, and class types. 
    /// Always chooses a reward upon quest completion.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="reward">The name of the reward to choose (if applicable).</param>
    /// <param name="mapName">An optional map name for the hunt.</param>
    /// <param name="monsterName">An optional monster name for the hunt.</param>
    /// <param name="log">Whether to log the hunting process.</param>
    public void HuntMonsterQuestChoose(int questId, string? reward = null, string? mapName = null, string? monsterName = null, bool log = false)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"Failed to load quest with ID [{questId}] after multiple attempts.", stopBot: true);
            return;
        }

        // Ensure quest.Requirements and quest.Rewards are not null or empty
        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        // Combine quest.AcceptRequirements and quest.Requirements
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Add reward to itemsToUnbank if specified
        if (!string.IsNullOrEmpty(reward))
        {
            allRequirements.AddRange(quest.Rewards
                .Where(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)));
        }

        // Unbank required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        // Add non-temp items to the drop pickup list
        Bot.Drops.Add(allRequirements
            .Where(x => x != null && !x.Temp)
            .Select(x => x.Name)
            .Distinct()
            .ToArray());



        // Auto-generate default map-monster-class pairs if not provided
        if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(monsterName))
        {
            mapName ??= "default_map";
            monsterName ??= "*";
        }

        // Process each requirement
        for (int i = 0; i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];

            // Skip if the required item is already in inventory
            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Ensure the quest is accepted if not in progress
            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            // Equip the appropriate class and hunt the monster
            EquipClass(ClassType.Solo); // Default class, can be adjusted if needed
            HuntMonster(mapName, monsterName, requirement.Name ?? string.Empty, requirement.Quantity, requirement.Temp, log);
        }

        // Complete the quest, retrying if necessary
        if (!Bot.Quests.EnsureComplete(questId, reward != null
                    ? quest.Rewards.FirstOrDefault(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))?.ID ?? -1
                    : -1))
        {
            EnsureCompleteMulti(
                questId,
                itemID: reward != null
                    ? quest.Rewards.FirstOrDefault(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))?.ID ?? -1
                    : -1
            );
        }
    }

    //Choose Variants - Int

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest and an optional array of map and monster names. 
    /// Always chooses a reward by its ID upon quest completion.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="rewardId">The ID of the reward to choose (if applicable).</param>
    /// <param name="mapMonsterClassPairs">Array of tuples specifying map names, monster names, and class types.</param>
    public void HuntMonsterQuestChoose(int questId, int rewardId, params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"Failed to load quest with ID [{questId}] after multiple attempts.", stopBot: true);
            return;
        }

        // Ensure quest.Requirements and quest.Rewards are not null or empty
        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        // Combine quest.AcceptRequirements and quest.Requirements
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Add reward to itemsToUnbank if specified
        if (rewardId > 0)
        {
            allRequirements.AddRange(quest.Rewards
                .Where(x => x.ID == rewardId));
        }

        // Unbank required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        // Add non-temp items to the drop pickup list
        Bot.Drops.Add(allRequirements
            .Where(x => x != null && !x.Temp)
            .Select(x => x.Name)
            .Distinct()
            .ToArray());



        // Auto-generate default map-monster-class pairs if not provided
        if (mapMonsterClassPairs.Length == 0)
        {
            mapMonsterClassPairs = quest.Requirements
                .Select(_ => ("default_map", "default_monster", ClassType.Solo))
                .ToArray();
        }
        else if (mapMonsterClassPairs.Length > quest.Requirements.Count)
        {
            Logger($"Warning: More map-monster-class pairs provided than quest requirements. Extra pairs will be ignored.", stopBot: false);
        }

        // Process each map-monster-class pair
        for (int i = 0; i < mapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            (string mapName, string monsterName, ClassType classType) = mapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Ensure the quest is accepted if not in progress
            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            // Equip the appropriate class and hunt the monster
            EquipClass(classType);
            HuntMonster(mapName ?? "default_map", monsterName ?? "*", requirement.Name ?? string.Empty, requirement.Quantity, requirement.Temp, log: false);
        }

        // Complete the quest, retrying if necessary
        if (!Bot.Quests.EnsureComplete(questId, rewardId > 0
                    ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1
                    : -1))
        {
            EnsureCompleteMulti(
                questId,
                itemID: rewardId > 0
                    ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1
                    : -1
            );
        }
    }

    /// <summary>
    /// Hunts monsters based on the requirements of a specified quest with optional map, monster names, and class types. 
    /// Always chooses a reward by its ID upon quest completion.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="rewardId">The ID of the reward to choose (if applicable).</param>
    /// <param name="mapName">An optional map name for the hunt.</param>
    /// <param name="monsterName">An optional monster name for the hunt.</param>
    /// <param name="log">Whether to log the hunting process.</param>
    public void HuntMonsterQuestChoose(int questId, int rewardId, string? mapName = null, string? monsterName = null, bool log = false)
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"Failed to load quest with ID [{questId}] after multiple attempts.", stopBot: true);
            return;
        }

        // Ensure quest.Requirements and quest.Rewards are not null or empty
        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        // Combine quest.AcceptRequirements and quest.Requirements
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Add reward to itemsToUnbank if specified
        if (rewardId > 0)
        {
            allRequirements.AddRange(quest.Rewards
                .Where(x => x.ID == rewardId));
        }

        // Unbank required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        // Add non-temp items to the drop pickup list
        Bot.Drops.Add(allRequirements
            .Where(x => x != null && !x.Temp)
            .Select(x => x.Name)
            .Distinct()
            .ToArray());



        // Auto-generate default map-monster-class pairs if not provided
        if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(monsterName))
        {
            mapName ??= "default_map";
            monsterName ??= "*";
        }

        // Process each requirement
        for (int i = 0; i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];

            // Skip if the required item is already in inventory
            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Ensure the quest is accepted if not in progress
            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            // Equip the appropriate class and hunt the monster
            EquipClass(ClassType.Solo); // Default class, can be adjusted if needed
            HuntMonster(mapName, monsterName, requirement.Name ?? string.Empty, requirement.Quantity, requirement.Temp, log);
        }

        // Complete the quest, retrying if necessary
        if (!Bot.Quests.EnsureComplete(questId, rewardId > 0
                    ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1
                    : -1))
        {
            EnsureCompleteMulti(
                questId,
                itemID: rewardId > 0
                    ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1
                    : -1
            );
        }
    }

    /* Examples:
        HuntMonsterQuest (with params (string mapName, string monsterName, ClassType classType)[] MapMonsterClassPairs):
            HuntMonsterQuest(101, ("Map1", "MonsterA", ClassType.Solo), ("Map2", "MonsterB", ClassType.Solo));

        HuntMonsterQuest (with optional mapName, monsterName, and log):
            HuntMonsterQuest(102, mapName: "Map3", monsterName: "MonsterC", log: true);

        HuntMonsterQuestChoose (with reward and params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs):
            HuntMonsterQuestChoose(103, "RewardA", ("Map1", "MonsterA", ClassType.Solo), ("Map2", "MonsterB", ClassType.Solo));

        HuntMonsterQuestChoose (with optional mapName, monsterName, and log):
            HuntMonsterQuestChoose(104, "RewardB", mapName: "Map4", monsterName: "MonsterD", log: false);

        HuntMonsterQuestChoose (with rewardId and params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs):
            HuntMonsterQuestChoose(105, 201, ("Map1", "MonsterA", ClassType.Solo), ("Map2", "MonsterB", ClassType.Solo));

        HuntMonsterQuestChoose (with rewardId, mapName, monsterName, and log):
            HuntMonsterQuestChoose(106, 202, mapName: "Map5", monsterName: "MonsterE", log: true);
    */

    /// <summary>
    /// Kill Escherion for the desired item
    /// </summary>
    /// <param name="item">Item name</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log"></param>
    /// <param name="publicRoom"></param>
    /// <param name="FromSupplies"></param>
    /// <param name="SellVoucher"></param>
    /// <param name="ReturnDuring"></param>
    /// <param name="ReturnItem"></param>
    public void KillEscherion(string? item = null, int quant = 1, bool isTemp = false, bool log = true, bool publicRoom = false, bool FromSupplies = false, bool SellVoucher = false, bool ReturnDuring = false, string? ReturnItem = null)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        if (Bot.Map.Name != "escherion")
            Join("escherion", publicRoom: publicRoom);
        if (Bot.Player.Cell != "Boss")
            Jump("Boss", "Left");

        if (!FromSupplies)
            if (item is not null && log)
                FarmingLogger(item, quant);

        if (item is not null && !isTemp)
            AddDrop(item);

        bool PreFarmKill = false;

        Monster? Staff = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 2);
        Monster? Escherion = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 3);
        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Where(x => x != Bot.Player.Username).Any())
        {
            Bot.Options.AggroMonsters = true;
            //hide players to reduce lag (Trust Tato)
            Bot.Options.HidePlayers = true;
        }
        else Bot.Options.AggroMonsters = false;

        if (item is null)
        {
            if (log)
                Logger("Killing Escherion");

            _KillEscherion();
        }
        else
        {
            while (!Bot.ShouldExit && isTemp ? !Bot.TempInv.Contains(item, quant) : !CheckInventory(item, quant))
                _KillEscherion(item, isTemp);

            Rest();
            if (!isTemp)
                Bot.Wait.ForPickup(item);
            else Bot.Wait.ForDrop(item);
        }

        void _KillEscherion(string? item = null, bool isTemp = false)
        {
            if (Bot.Map.Name is not "escherion")
            {
                Join("escherion");
                Bot.Wait.ForMapLoad("escherion");
                Sleep();
            }

            if (Bot.Player.Cell is not "Boss")
            {
                Jump("Boss");
                Bot.Wait.ForCellChange("Boss");
                Sleep();
            }

            // Initialize combat (to set hp)
            if (!PreFarmKill && Staff is not null)
            {
                Bot.Kill.Monster(Staff);
                PreFarmKill = true;
            }

            Staff = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 2);
            Escherion = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 3);

            if (Staff is not null && Escherion is not null)
            {
                Monster? target = Staff.State is 1 or 2 ? Staff : Escherion is not null && Staff.State is 0 ? Escherion : null;
                if (target is not null)
                    Bot.Combat.Attack(target);
            }
            Sleep();

            // Sell voucher area
            if (item != "Voucher of Nulgath" && SellVoucher && CheckInventory("Voucher of Nulgath"))
            {
                while (!Bot.ShouldExit && (Bot.Player.HasTarget || Bot.Player.InCombat) && Bot.Player.Cell != "Enter")
                {
                    Bot.Combat.CancelTarget();
                    Bot.Wait.ForCombatExit();
                    JumpWait();
                    Sleep();
                }

                if (Bot.Player.Gold < 100000000)
                {
                    Bot.Wait.ForPickup("Voucher of Nulgath");
                    SellItem("Voucher of Nulgath", all: true);
                    Bot.Wait.ForItemSell();
                }
            }
            DoSwindlesReturnArea(ReturnDuring, ReturnItem);

        }

        Bot.Options.AttackWithoutTarget = false;
        ToggleAggro(false);
        Jump();
        Bot.Options.AggroMonsters = false;
        JumpWait();
        Rest();
        Bot.Options.HidePlayers = false;

        void DoSwindlesReturnArea(bool returnPolicyActive, string? item = null)
        {
            // Return if the policy isn't active or required items are missing
            if (!returnPolicyActive || !CheckInventory(new[] { Uni(1), Uni(6), Uni(9), Uni(16), Uni(20) }))
                return;

            bool retry = true;

            while (!Bot.ShouldExit && retry)
            {
                retry = false; // Reset retry flag
                ResetQuest(7551);
                DarkMakaiItem("Dark Makai Rune");

                // Load quest and find rewards
                Quest? quest = InitializeWithRetries(() => Bot.Quests.EnsureLoad(7551));
                if (quest == null)
                {
                    Logger("Failed to load quest 7551, retrying...");
                    Sleep();
                    retry = true;
                    continue;
                }

                // Handle null `item` by skipping directly to reward selection
                ItemBase? targetReward = item == null
                    ? null
                    : quest.Rewards.FirstOrDefault(r => r.Name == item && r.Name != "Receipt of Swindle");

                int rewardID = targetReward?.ID ??
                               quest.Rewards.FirstOrDefault(r => !CheckInventory(r.ID, r.MaxStack))?.ID ?? -1;

                if (rewardID != -1 && Bot.Quests.CanCompleteFullCheck(7551))
                {
                    Logger($"Completing with: {quest.Rewards.First(r => r.ID == rewardID).Name} [ID: {rewardID}]");
                    EnsureComplete(7551, rewardID);
                }
                else
                {
                    Logger("All rewards maxed. Completing with fallback reward ID: -1 (\"Receipt of Swindle\").");
                    EnsureComplete(7551);
                }
            }
        }


        string Uni(int nr)
            => $"Unidentified {nr}";

    }


    /// <summary>
    /// Kill Vath for the desired item
    /// </summary>
    /// <param name="item">Item name</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log"></param>
    public void KillVath(string? item = null, int quant = 1, bool isTemp = false, bool log = true)
    {
        if (item is not null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        Join("stalagbite");
        Bot.Wait.ForMapLoad("stalagbite");
        Jump("r2", "Left");
        Bot.Wait.ForCellChange("r2");
        bool PreFarmKill = false;

        Monster? Vath = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 7);
        Monster? Stalagbite = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 8);
        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Where(x => x != Bot.Player.Username).Any())
        {
            Bot.Options.AggroMonsters = true;
            //hide players to reduce lag (Trust Tato)
            Bot.Options.HidePlayers = true;
        }
        else Bot.Options.AggroMonsters = false;
        if (item is null)
        {
            if (log)
                Logger("Killing Vath");
            KillVath();
        }
        else
        {
            if (!isTemp)
                AddDrop(item);
            if (log)
                Logger($"Killing Vath for {item} ({dynamicQuant(item, isTemp)}/{quant}) [Temp = {isTemp}]");
            while (!Bot.ShouldExit && !CheckInventory(item, quant))
                KillVath();
        }
        Bot.Options.AttackWithoutTarget = false;
        ToggleAggro(false);
        Jump();
        Bot.Options.AggroMonsters = false;
        JumpWait();
        Rest();
        Bot.Options.HidePlayers = false;
        void KillVath()
        {
            if (Bot.Map.Name is not "stalagbite")
            {
                Join("stalagbite");
                Bot.Wait.ForMapLoad("stalagbite");
                Sleep();
            }

            if (Bot.Player.Cell is not "r2")
            {
                Jump("r2");
                Bot.Wait.ForCellChange("r2");
                Sleep();
            }

            // Initialize combat (to set hp)
            if (!PreFarmKill && Stalagbite is not null)
            {
                Bot.Kill.Monster(Stalagbite);
                PreFarmKill = true;
            }

            Stalagbite = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 8);
            Vath = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID is 7);

            // if (Stalagbite is not null && Stalagbite.State is 1 or 2)
            // {
            //     Bot.Kill.Monster(Stalagbite);
            //     Bot.Combat.CancelTarget();
            // }
            // else if (Vath is not null && Stalagbite is not null && Vath.State is not 0 && Stalagbite.State is 0)
            // {
            //     Bot.Combat.Attack(Vath);
            //     Sleep();
            // }

            if (Stalagbite != null)
            {
                Bot.Wait.ForMonsterSpawn(Stalagbite.Name);
                if (Vath is not null)
                    Bot.Combat.Attack(Stalagbite.State is 1 or 2 ? Stalagbite : Vath);
                Sleep();
            }
        }

    }

    /// <summary>
    /// Kill Kitsune for the desired item
    /// </summary>
    /// <param name="item">Item name</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log"></param>
    public void KillKitsune(string? item = null, int quant = 1, bool isTemp = false, bool log = true)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        Join("kitsune", "Boss", "Left");
        Bot.Events.ExtensionPacketReceived += KitsuneListener;

        if (item == null)
        {
            if (log)
                Logger("Killing Kitsune");
            Bot.Kill.Monster("Kitsune");
        }
        else
        {
            if (!isTemp)
                AddDrop(item);
            if (log)
                Logger($"Killing Kitsune for {item} ({dynamicQuant(item, isTemp)}/{quant}) [Temp = {isTemp}]");
            while (!Bot.ShouldExit && !CheckInventory(item, quant))
                Bot.Combat.Attack("*");
        }
        Bot.Events.ExtensionPacketReceived -= KitsuneListener;

        void KitsuneListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ct":
                        if (data.a is not null)
                        {
                            foreach (dynamic a in data.a)
                            {
                                if (a is null)
                                    continue;

                                if (a.aura is not null && (string)a.aura["nam"] is "Shapeshifted")
                                {
                                    Bot.Combat.StopAttacking = ((string)a.cmd)[^0] == '+';
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Kill Vath for the desired item.
    /// </summary>
    /// <param name="item">Item name.</param>
    /// <param name="quant">Desired quantity.</param>
    /// <param name="isTemp">Whether the item is temporary.</param>
    /// <param name="Phase">Which phase of the boss to kill.</param>
    public void KillTrigoras(string item, int quant = 1, int Phase = 1, bool isTemp = false)
    {
        if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
            return;

        EquipClass(ClassType.Solo);
        Join("trigoras");

        while (!Bot.ShouldExit && !CheckInventory(item, quant))
        {
            Jump(Phase == 1 ? "r4" : "r4a", "Left");
            Bot.Combat.Attack("trigoras");
            Bot.Wait.ForCellChange(Phase == 1 ? "r4a" : "Enter");
        }
        Bot.Wait.ForCellChange(Phase == 1 ? "r4a" : "Enter");
        JumpWait();
    }

    public void KillDoomKitten(string? item = null, int quant = 1, bool isTemp = false, bool log = true)
    {
        if (item != null)
        {
            bool hasItem = isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant);
            if (hasItem) return;
        }

        List<string> DOTClasses = new()
        {
        "ShadowStalker of Time",
        "ShadowWeaver of Time",
        "ShadowWalker of Time",
        "Infinity Knight",
        "Interstellar Knight",
        "Dragon of Time",
        "Timeless Dark Caster",
        "Frostval Barbarian",
        "Blaze Binder",
        "DeathKnight",
        "DragonSoul Shinobi",
        "Shadow Dragon Shinobi",
        "Legion Revenant",
        "Void Highlord",
    };

        // Check if the bot has any of the classes from the DOTClasses list
        bool hasAnyClass = DOTClasses.Any(c => CheckInventory(c));

        if (!hasAnyClass)
        {
            Logger("--------------------------------");
            Logger("Possible classes for DoomKitten:\n" + string.Join("\n", DOTClasses));
            Logger("--------------------------------");

            Logger($"'Damage over Time' class / VHL not found. See the logs to see suggestions. Please get one and run the bot again. Stopping.", messageBox: true, stopBot: true);
            return; // Stop execution as the bot doesn't have any of the required classes.
        }

        InventoryItem? classItem = null;
        foreach (string className in DOTClasses)
        {
            classItem = Bot.Inventory.Items.Find(i => i.Name.ToLower().Trim() == className.ToLower().Trim() && i.Category == ItemCategory.Class);

            if (classItem != null)
            {
                Equip(classItem.Name);

                if (className == "DragonSoul Shinobi" || className == "Shadow Dragon Shinobi")
                {
                    Logger("Due to the nature of this class and the hit range of the kitten, this is basically RNG gl!");
                    Join("doomkitten");

                    Bot.Skills.StartAdvanced("4 | 1 | 3M<30 | 2H<30");
                    while (!Bot.ShouldExit && !CheckInventory(item, quant))
                        Bot.Combat.Attack("*");
                }
                else
                    HuntMonster("doomkitten", "DoomKitten", item, quant, isTemp, log);
                return; // Exit the method after handling the class.
            }
        }

        if (classItem == null)
        {
            Logger("Could not find any of the Damage over Time classes in the bot's inventory. Stopping.", messageBox: true, stopBot: true);
            return; // Stop execution as the bot doesn't have any of the required classes.
        }

        HuntMonster("doomkitten", "DoomKitten", item, quant, isTemp, log);
    }

    /// <summary>
    /// Kills Xiang or Ultra Xiang to obtain the desired item.
    /// </summary>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quant">The desired quantity of the item.</param>
    /// <param name="ultra">Specifies whether to fight the Ultra Xiang variant.</param>
    /// <param name="isTemp">Specifies whether the item is temporary.</param>
    /// <param name="log">Specifies whether to log the process.</param>
    public void KillXiang(string item, int quant = 1, bool ultra = false, bool isTemp = false, bool log = true)
    {
        if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
            return;

        if (CheckInventory("Dragon of Time"))
            Bot.Skills.StartAdvanced("Dragon of Time", true, ClassUseMode.Solo);
        else if (CheckInventory("Healer (Rare)"))
            Bot.Skills.StartAdvanced("Healer (Rare)", true, ClassUseMode.Base);
        else if (CheckInventory("Healer"))
            Bot.Skills.StartAdvanced("Healer", true, ClassUseMode.Base);

        JumpWait();

        KillMonster("mirrorportal", ultra ? "r6" : "r4", "Right", ultra ? "Ultra Xiang" : "Chaos Lord Xiang", item, quant, isTemp, log);
    }

    /// <summary>
    /// Kills Nulgath Fiend Shards to obtain the desired item.
    /// </summary>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quant">The desired quantity of the item.</param>
    /// <param name="isTemp">Specifies whether the item is temporary.</param>
    /// <param name="log"></param>
    public void KillNulgathFiendShard(string? item = null, int quant = 1, bool isTemp = false, bool log = false)
    {
        if (CheckInventory(item, quant))
            return;

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        Join("fiendshard", "r9", "Left");
        if (Bot.Player.Cell != "r9")
        {
            Jump("r9", "Left");
            Sleep();
        }


        if (item != null && log)
            FarmingLogger(item, quant);

        bool PreFarmKill = false;

        Monster? monster = Bot.Monsters.MapMonsters.FirstOrDefault(m => m.MapID == 15);

        if (item == null)
        {
            _KillFiendShard();
        }
        else
        {
            if (log)
                Logger("Killing Nulgath's FiendShard");
            if (!isTemp)
                AddDrop(item);

            while (!Bot.ShouldExit && !CheckInventory(item, quant))
                _KillFiendShard();

            Bot.Wait.ForPickup(item);
            Jump("Enter", "Spawn");
            Rest();
        }
        Jump("Enter", "Spawn");
        Bot.Wait.ForCellChange("Enter");
        Bot.Wait.ForPickup(item!);
        Bot.Options.AttackWithoutTarget = false;

        void _KillFiendShard()
        {
            if (monster == null)
            {
                Logger("Monster with MapID 15 not found.");
                return;
            }
            // Initialize combat (to set hp)
            if (!PreFarmKill)
            {

                CheckCell(monster.Cell ?? "r9");
                Logger("PreFarm kill to set Hp");
                Bot.Kill.Monster(monster.MapID);
                Bot.Wait.ForMonsterSpawn(monster.MapID);
                PreFarmKill = true;
            }
            CheckCell(monster?.Cell ?? "r9");
            monster = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID == 15);
            if (monster == null)
            {
                Logger("Monster with MapID 15 not found after respawn.");
                return;
            }

            if (monster?.State == 1 || monster?.State == 2)
            {
                CheckCell(monster.Cell ?? "r9");
                Bot.Kill.Monster(monster.MapID);
                Bot.Combat.CancelTarget();
            }
            else if (monster?.State == 0)
            {
                CheckCell(monster?.Cell ?? "r9");
                Bot.Combat.Attack("*");
                Sleep();
            }
        }

        void CheckCell(string? cell = null, string pad = "left")
        {
            if (Bot.Player.Cell == cell)
                return;

            if (Bot.Player.Cell != cell)
                Bot.Map.Jump(cell ?? "r9", pad);
            Bot.Wait.ForCellChange(cell ?? "r9");
            Bot.Player.SetSpawnPoint();
        }
    }

    /// <summary>
    /// Kills monsters until the specified item is obtained.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quantity">The quantity of the item to obtain.</param>
    /// <param name="isTemp">Whether the item is temporary (default: false).</param>
    /// <param name="rejectElse">Whether to reject all drops except the specified item (default: false).</param>
    /// <param name="log">Whether to log farming activity (default: true).</param>
    /// <param name="cell">The cell where monsters should be killed (default: null).</param>
    /// <remarks>
    /// This method continuously kills monsters with the specified name until the required item and quantity are obtained.
    /// If the item is not temporary, it adds the item to drops.
    /// </remarks>
    public void _KillForItem(Monster name, string? item, int quantity, bool isTemp = false, bool rejectElse = false, bool log = true, string? cell = null)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        bool HasItem() => isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity);
        IEnumerable<Monster> FindMonsters() => Bot.Monsters.CurrentAvailableMonsters.Where(x => x != null && x.Cell == cell && x.Name.FormatForCompare() == name.Name.FormatForCompare());

        void MoveToCell(Monster monster)
        {
            string targetCell = cell ?? monster.Cell;
            if (Bot.Player.Cell != targetCell)
            {
                Jump(targetCell, "Left");
                Bot.Wait.ForCellChange(targetCell);
            }
        }

        void LogMonsterId(Monster monster) { if (log) Logger($"Attacking MonsterMapID: {monster}"); }

        if (HasItem()) return;
        if (log) FarmingLogger(item, quantity);

        while (!Bot.ShouldExit && !HasItem())
        {
            List<Monster> monsters = FindMonsters().ToList();
            if (!monsters.Any())
            {
                Sleep(); // No monsters found, wait a bit before retrying
                continue;
            }

            foreach (var monster in monsters)
            {
                if (Bot.ShouldExit || HasItem()) break;

                LogMonsterId(monster);
                bool ded = false;
                Bot.Events.MonsterKilled += b => ded = true;
                while (!Bot.ShouldExit && !ded || !HasItem())
                {
                    MoveToCell(monster);

                    if (!Bot.Combat.StopAttacking)
                        Bot.Combat.Attack(monster);
                    Sleep();
                }
                if (rejectElse) Bot.Drops.RejectExcept(item);
                Sleep();
            }
        }
        if (rejectElse)
            if (item != null)
                Bot.Drops.RejectExcept(item);

        if (item != null)
            Bot.Wait.ForPickup(item);
    }

    public void _KillForItem(string name, string? item = null, int quantity = 1, bool isTemp = false, bool rejectElse = false, bool log = true, string? cell = null)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity)))
        {
            return;
        }
        if (log && item != null)
            if (name != "*")
                Logger($"Attacking Monster: {name}, for {item}  {dynamicQuant(item, isTemp)}/{quantity}");

        while (!Bot.ShouldExit && item != null && (isTemp ? !Bot.TempInv.Contains(item, quantity) : !CheckInventory(item, quantity)))
        {
            if (name == "*")
            {
                foreach (Monster monster in Bot.Monsters.MapMonsters.Where(x => x != null && x.Cell == cell))
                {
                    if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                        break;

                    bool ded = false;
                    Bot.Events.MonsterKilled += b => ded = true;
                    while (!Bot.ShouldExit && !ded || isTemp ? !Bot.TempInv.Contains(item, quantity) : !CheckInventory(item, quantity))
                    {
                        if (cell != null && Bot.Player.Cell != cell)
                            Jump(cell, "Left");

                        if (!Bot.Combat.StopAttacking)
                            Bot.Combat.Attack(monster);
                        Sleep();

                        if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                            break;
                    }

                    if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                        break;
                }
            }
            else
            {
                if (item != null)
                {
                    foreach (Monster targetMonster in Bot.Monsters.MapMonsters.Where(x => x != null && x.Cell == cell && x.Name.FormatForCompare() == name.FormatForCompare()))
                    {
                        if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                            break;

                        bool ded = false;
                        Bot.Events.MonsterKilled += b => ded = true;
                        while (!Bot.ShouldExit && !ded || isTemp ? !Bot.TempInv.Contains(item, quantity) : !CheckInventory(item, quantity))
                        {
                            if (cell != null && Bot.Player.Cell != cell)
                            {
                                Jump(cell, "Left");
                                Bot.Wait.ForCellChange(cell);
                                Sleep();
                            }
                            if (!Bot.Combat.StopAttacking)
                                Bot.Combat.Attack(targetMonster);

                            if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                                break;
                        }

                        if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
                            break;
                        Sleep();
                    }
                }
                else
                {
                    Logger("Item is null.");
                    return;
                }

                if (rejectElse)
                    if (item != null)
                        Bot.Drops.RejectExcept(item);
            }
            if (item != null)
            {
                Bot.Wait.ForDrop(item);
                Bot.Wait.ForPickup(item);
            }
        }

    }
    public void _KillForItem(string name, int itemID = 0, int quantity = 1, bool isTemp = false, bool rejectElse = false, bool log = true, string? cell = null)
    {
        if (itemID != 0 && (isTemp ? Bot.TempInv.Contains(itemID, quantity) : CheckInventory(itemID, quantity)))
        {
            return;
        }
        if (log)
            FarmingLogger(itemID.ToString(), quantity);

        while (!Bot.ShouldExit && (isTemp ? !Bot.TempInv.Contains(itemID, quantity) : !CheckInventory(itemID, quantity)))
        {
            bool ded = false;
            Bot.Events.MonsterKilled += b => ded = true;
            while (!Bot.ShouldExit && !ded && itemID > 0 && (isTemp ? !Bot.TempInv.Contains(itemID, quantity) : !CheckInventory(itemID, quantity)))
            {
                if (cell != null && Bot.Player.Cell != cell)
                {
                    Jump(cell, "Left");
                    Bot.Wait.ForCellChange(cell);
                }
                if (!Bot.Combat.StopAttacking)
                    Bot.Combat.Attack(name);

                Sleep();

                if (isTemp ? Bot.TempInv.Contains(itemID, quantity) : CheckInventory(itemID, quantity))
                    break;
            }

            if (isTemp ? Bot.TempInv.Contains(itemID, quantity) : CheckInventory(itemID, quantity))
                break;
            Sleep();

            if (rejectElse)
                Bot.Drops.RejectExcept(itemID);

            if (itemID > 0)
                Bot.Wait.ForPickup(itemID);

        }
    }

    #region  IsMonsterAlive
    public bool IsMonsterAlive(Monster? mon)
        => mon != null && (mon.Alive || !KilledMonsters.Contains(mon.MapID));
    public bool IsMonsterAlive(string monsterName)
        => Bot.Monsters.CurrentMonsters.Where(m => m.Name == monsterName).Any(m => IsMonsterAlive(m));
    public bool IsMonsterAlive(int monsterID, bool useMapID)
    {
        if (useMapID)
            return IsMonsterAlive(Bot.Monsters.CurrentMonsters.Find(m => m.MapID == monsterID));
        else return Bot.Monsters.CurrentMonsters.Where(m => m.ID == monsterID).Any(m => IsMonsterAlive(m));
    }


    public readonly List<int> KilledMonsters = new();
    public void CleanKilledMonstersList(string map)
        => KilledMonsters.Clear();
    public void KilledMonsterListener(int monsterMapID)
        => KilledMonsters.Add(monsterMapID);
    public void RespawnListener(dynamic packet)
    { //%xt%respawnMon%-1%12% (monster map ID is 12 in this example)
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "str")
        {
            string cmd = data[0];
            switch (cmd)
            {
                case "respawnMon":
                    KilledMonsters.RemoveAll(id => id == (int)data[2]);
                    break;
            }
        }
    }

    public bool IsDungeonMonsterAlive(Monster? mon)
        => mon != null && (mon.Alive || !KilledDungeonMonsters.Contains(mon.MapID));
    public bool IsDungeonMonsterAlive(string monsterName)
        => Bot.Monsters.CurrentMonsters.Where(m => m.Name == monsterName).Any(m => IsDungeonMonsterAlive(m));
    public bool IsDungeonMonsterAlive(int monsterID)
        => Bot.Monsters.CurrentMonsters.Where(m => m.ID == monsterID).Any(m => IsDungeonMonsterAlive(m));
    public bool IsDungeonMonsterAlive(int monsterMapID, bool useMapID)
        => IsDungeonMonsterAlive(Bot.Monsters.CurrentMonsters.Find(m => m.MapID == monsterMapID));
    public void ActivateDungeonMonsterListener(bool enable = true)
    {
        if (enable)
        {
            Bot.Events.MonsterKilled += KilledDungeonMonsterListener;
            Bot.Events.MapChanged += CleanKilledDungeonMonstersList;
        }
        else
        {
            Bot.Events.MonsterKilled -= KilledDungeonMonsterListener;
            Bot.Events.MapChanged -= CleanKilledDungeonMonstersList;
        }
    }

    private readonly List<int> KilledDungeonMonsters = new();
    private void CleanKilledDungeonMonstersList(string map)
        => KilledMonsters.Clear();
    private void KilledDungeonMonsterListener(int monsterMapID)
        => KilledMonsters.Add(monsterMapID);

    #endregion
    #endregion IsMonsterAlive

    #region Utility    

    /// <summary>
    /// Checks whether the player is an Upholder
    /// </summary>
    public bool IsUpholder() => Badges.Any(badge => badge.Name.Contains("Upholder"));

    /// <summary>
    /// Retrieves the username from a game object or falls back to the player's username.
    /// </summary>
    /// <returns>The username string.</returns>
    public string Username()
    {
        try
        {
            return Bot.Flash.GetGameObject("sfc.myUserName")![1..^1];
        }
        catch
        {
            return Bot.Player.Username ?? "";
        }
    }

    /// <summary>
    /// Pauses execution for a specified duration in milliseconds.
    /// If the provided duration is -1, it uses the default action delay of 750ms.
    /// </summary>
    /// <param name="ms">The duration to pause execution in milliseconds. Defaults to -1.</param>
    public void Sleep(int ms = -1)
    {
        if (Bot.ShouldExit)
        {
            Bot.Stop(true);
            return;
        }
        Bot.Sleep((ms == -1) ? ActionDelay : ms);
    }


    // /// <summary>
    // /// Logs a line of text to the script log with time, method from where it's called and a message
    // /// </summary>
    // public void Logger(string message = "", [CallerMemberName] string caller = "", bool messageBox = false, bool stopBot = false)
    // {
    //     Bot.Log($"[{DateTime.Now:HH:mm:ss}] ({caller})  {message}");
    //     if (LoggerInChat && Bot.Player.LoggedIn)
    //         Bot.Send.ClientModerator(message.Replace('[', '(').Replace(']', ')'), caller);
    //     if (messageBox & !ForceOffMessageboxes)
    //         Message(message, caller);
    //     if (stopBot)
    //     {
    //         scriptFinished = false;
    //         Bot.Stop(true);
    //     }
    // }

    //testing
    public void Logger(string message = "", [CallerMemberName] string caller = "", bool messageBox = false, bool stopBot = false)
    {
        // Word wrap the message
        message = WordWrap(message, 50); // Adjust the line length as needed

        Bot.Log($"[{DateTime.Now:HH:mm:ss}] ({caller})  {message}");
        if (LoggerInChat && Bot.Player.LoggedIn)
            Bot.Send.ClientModerator(message.Replace('[', '(').Replace(']', ')'), caller);
        if (messageBox & !ForceOffMessageboxes)
            Message(message, caller);
        if (stopBot)
        {
            scriptFinished = false;
            Bot.Stop(true);
        }
    }

    // Word wrap function
    public static string WordWrap(string input, int lineLength)
    {
        StringBuilder sb = new();
        int length = 0;

        foreach (string word in input.Split(' '))
        {
            if (length + word.Length > lineLength)
            {
                sb.AppendLine();
                length = 0;
            }

            sb.Append(word + " ");
            length += word.Length + 1;
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Logs farming activity for a specified item.
    /// </summary>
    /// <param name="item">The name of the item being farmed (nullable).</param>
    /// <param name="quant">The desired quantity of the item to farm (default is 1).</param>
    /// <param name="caller">Automatically provided by the compiler to indicate the calling member (optional).</param>
    public void FarmingLogger(string? item, int quant = 1, [CallerMemberName] string caller = "")
    {
        int quantity = string.IsNullOrEmpty(item) ? 0 : Bot.TempInv.GetQuantity(item) + Bot.Inventory.GetQuantity(item);
        Logger($"Farming {item} ({quantity}/{quant})", caller);
    }

    /// <summary>
    /// Logs debug information based on various filters and script context.
    /// </summary>
    /// <param name="_this">The instance or type associated with the logging context.</param>
    /// <param name="marker">Optional marker for categorizing the log entry (default is null).</param>
    /// <param name="caller">Automatically provided by the compiler to indicate the calling member (optional).</param>
    /// <param name="lineNumber">Automatically provided by the compiler to indicate the line number where the method is called (optional).</param>
    public void DebugLogger(object _this, string? marker = null, [CallerMemberName] string? caller = null, [CallerLineNumber] int lineNumber = 0)
    {
        if (!DL_Enabled || (DL_MarkerFilter != null && DL_MarkerFilter != marker) || (DL_CallerFilter != null && DL_CallerFilter != caller))
            return;

        string _class = _this.GetType().ToString();
        string[] compiledScript = CompiledScript();

        int compiledClassLine = Array.IndexOf(compiledScript, compiledScript.First(line => line.Trim() == $"public class {_class}")) + 1;
        string[] currentScript = File.ReadAllLines(Bot.Manager.LoadedScript);
        string[]? includedScript = null;

        bool inCurrentScript = false;
        if (currentScript.Any(line => line.Trim() == $"public class {_class}"))
            inCurrentScript = true;
        else
        {
            foreach (string cs in currentScript.Where(x => x.StartsWith("//cs_include")).ToArray())
            {
                List<string> pathParts = new() { ClientFileSources.SkuaDIR };
                pathParts.AddRange(cs.Replace("//cs_include ", "").Replace("\\", "/").Split('/'));
                includedScript = File.ReadAllLines(Path.Combine(pathParts.ToArray()));

                if (includedScript.Any(line => line.Trim() == $"public class {_class}"))
                    break;
            }
        }

        if (!inCurrentScript && includedScript == null)
        {
            Logger("includedScript is NULL", "DEBUG LOGGER");
            return;
        }

        int count = 0;
        int lastIndex = compiledClassLine;

        foreach (string l in compiledScript[compiledClassLine..Array.FindIndex(compiledScript, compiledClassLine, l => l == "}")])
        {
            if (!l.Contains("DebugLogger(this"))
                continue;

            count++;
            lastIndex = Array.FindIndex(compiledScript, lastIndex + 1, _l => _l.Trim() == l.Trim());
            if (lastIndex + 1 == lineNumber)
                break;
        }

        int count2 = 0;
        int lastIndex2 = -1;
        string[] selectedScript = inCurrentScript || includedScript == null ? currentScript : includedScript;
        foreach (string l in selectedScript)
        {
            if (!l.Contains("DebugLogger(this"))
                continue;

            count2++;
            lastIndex2 = Array.FindIndex(selectedScript, lastIndex2 + 1, _l => _l.Trim() == l.Trim());

            if (count == count2)
                break;
        }

        Logger($"{marker}{(string.IsNullOrEmpty(marker) ? null : " | ")}{_class} => {caller}, line {lastIndex2 + 1}", "DEBUG LOGGER");
    }
    private bool DL_Enabled { get; set; } = false;
    public string? DL_CallerFilter { get; set; } = null;
    public string? DL_MarkerFilter { get; set; } = null;
    public void DL_Enable()
    {
        DL_Enabled = true;
        LoggerInChat = false;
    }
    public string[] CompiledScript() => Bot.Manager.CompiledScript.Split(
                                                                new string[] { "\r\n", "\r", "\n" },
                                                                StringSplitOptions.None);

    /// <summary>
    /// Creates a Message Box with the desired text and caption
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="caption">Title of the box</param>
    public void Message(string message, string caption)
    {
        Bot.Handlers.RegisterOnce(1, (Bot) => Bot.ShowMessageBox(message, caption));
    }

    /// <summary>
    /// Retrieves the quantity of an item from either temporary or regular inventory based on a condition.
    /// </summary>
    /// <param name="item">The name or identifier of the item to query.</param>
    /// <param name="isTemp">Specifies whether to check temporary inventory (<c>true</c>) or regular inventory (<c>false</c>).</param>
    /// <returns>The quantity of the specified item in the chosen inventory.</returns>
    public int dynamicQuant(string item, bool isTemp) => isTemp ? Bot.TempInv.GetQuantity(item) : Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(x => x.Name == item)?.Quantity ?? 0;

    /// <summary>
    /// Configures the aggro mode.
    /// </summary>
    /// <param name="status">The desired aggro status (default is true).</param>
    public void ConfigureAggro(bool status = true)
    {
        Logger("Configuring aggro");
        last_aggro_status = status;
    }


    /// <summary>
    /// Toggles the aggro mode on or off.
    /// </summary>
    /// <param name="enable">True to enable aggro mode, false to disable.</param>
    public void ToggleAggro(bool enable)
    {
        try
        {
            if (Bot.Options == null)
            {
                Logger("Bot.Options is null.");
                return;
            }

            if (enable)
            {
                if (last_aggro_status)
                {
                    // If was previously aggro when untoggled
                    // Set aggro back and flip last aggro
                    last_aggro_status = false;
                    Bot.Options.AggroMonsters = true;
                }
                else
                    return;
            }
            else
            {
                if (!Bot.Options.AggroMonsters)
                    return;
                else
                {
                    // If currently aggro, set last aggro to true
                    // and flip current aggro status
                    last_aggro_status = true;
                    Bot.Options.AggroMonsters = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
        }
    }
    private bool last_aggro_status = false;

    /// <summary>
    /// Send a packet to the server the desired amount of times
    /// </summary>
    /// <param name="packet">Packet to send</param>
    /// <param name="times">How many times to send</param>
    /// <param name="toClient"></param>
    public void SendPackets(string packet, int times = 1, bool toClient = false)
    {
        for (int i = 0; i < times; i++)
        {
            if (toClient)
                Bot.Send.ClientPacket(packet);
            else
                Bot.Send.Packet(packet);
            Sleep(ActionDelay * 2);
        }
    }

    /// <summary>
    /// Determines whether the bot should aggro monsters based on the presence of other players in the current map.
    /// </summary>
    /// <remarks>
    /// The method checks if there are any players on the current map other than the bot. 
    /// If other players are found, aggroing monsters is enabled, and players are hidden to reduce lag. 
    /// If no other players are found, aggroing monsters is disabled.
    /// </remarks>
    public void CanWeAggro()
    {
        Bot.Options.AggroAllMonsters = false;
        // Check if there are any other players in the cell
        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Any(x => x != Bot.Player.Username))
        {
            if (!Bot.Options.AggroMonsters)
                Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;  // Hide players to reduce lag
        }
        else
        {
            Bot.Options.AggroMonsters = false;
            Bot.Options.HidePlayers = false;
        }
    }




    /// <summary>
    /// Checks the current class rank of the player or a specified class based on the class name.
    /// </summary>
    /// <param name="CurrentClass">
    /// A boolean flag indicating whether to check the rank of the current class equipped by the player. Default is <c>false</c>.
    /// </param>
    /// <param name="ClassName">
    /// The name of the class to check if <paramref name="CurrentClass"/> is <c>false</c>. Default is <c>null</c>.
    /// </param>
    /// <returns>
    /// The class rank of the player or specified class.
    /// If the player is not found or the class is not in inventory or bank, <c>1</c> is returned by default.
    /// </returns>
    /// <remarks>
    /// The method first checks if the player is available. If <paramref name="CurrentClass"/> is set to <c>true</c>,
    /// the rank of the current equipped class is returned. Otherwise, it searches for the specified class in the player's
    /// inventory and bank, and calculates the rank based on the class quantity (Class Xp).
    /// </remarks>
    public int CheckClassRank(bool CurrentClass = false, string? ClassName = null)
    {
        if (Bot.Player == null)
        {
            Logger("Bot.Player is null.");
            return 1;
        }

        if (CurrentClass && Bot.Player.CurrentClass != null)
        {
            Logger($"Current Class: {Bot.Player.CurrentClass} | Current Rank: {Bot.Player.CurrentClassRank}");
            return Bot.Player.CurrentClassRank;
        }
        else
        {
            // Find the class item from the inventory or bank
            InventoryItem? Class = Bot.Inventory.Items.Concat(Bot.Bank.Items)
                .Find(i => i.Name == ClassName && i.Category == ItemCategory.Class);

            if (ClassName != null && Class != null)
            {
                Logger($"Class: {Class.Name} | Rank: {Class.Quantity}");

                // Define CP thresholds and corresponding ranks in a dictionary
                var ClassPointRanks = new Dictionary<int, int>
            {
                { 900, 1 },        // Rank 1 (CP < 900)
                { 3600, 2 },       // Rank 2 (900 <= CP < 3600)
                { 10000, 3 },      // Rank 3 (3600 <= CP < 10000)
                { 22500, 4 },      // Rank 4 (10000 <= CP < 22500)
                { 44100, 5 },      // Rank 5 (22500 <= CP < 44100)
                { 78400, 6 },      // Rank 6 (44100 <= CP < 78400)
                { 129600, 7 },     // Rank 7 (78400 <= CP < 129600)
                { 202500, 8 },     // Rank 8 (129600 <= CP < 202500)
                { 302500, 9 }      // Rank 9 (202500 <= CP < 302500)
            };

                // Determine the class rank based on quantity using the dictionary
                int classRank = 1; // Default rank
                foreach (var threshold in ClassPointRanks.OrderBy(t => t.Key))
                {
                    if (Class.Quantity >= threshold.Key)
                        classRank = threshold.Value;
                    else
                        break;
                }

                Logger($"Class Rank (based on ClassXP): {classRank}");
                return classRank;
            }
        }

        return 1; // Default return value if class is not found
    }



    /// <summary>
    /// Initiates resting for the bot's player character if conditions allow.
    /// </summary>
    public void Rest()
    {
        try
        {
            if (Bot.Player == null)
            {
                Logger("Bot.Player is null.");
                return;
            }

            // Rest if the player is alive, should rest, and health is below 2% of max health
            int healthThreshold = (int)Math.Ceiling((double)Bot.Player.MaxHealth / 50); // 50% threshold

            if (ShouldRest && Bot.Player.Alive && Bot.Player.Health < healthThreshold)
            {
                Bot.Player.Rest(false);
                Logger($"Resting initiated: Health below threshold ({Bot.Player.Health}/{Bot.Player.MaxHealth}).");
            }
        }
        catch (Exception ex)
        {
            Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
        }
    }



    /// <summary>
    /// Logs the player out and attempts to relogin to the same or a suitable server.
    /// Temporarily disables <c>Options.AutoRelogin</c>. If no preferred server is set or available, 
    /// connects to the first available server based on membership, ensuring it's not a test realm, 
    /// isn't full, and is online.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if server details cannot be fetched or no preferred server is set in <c>Options > Game</c>.
    /// </exception>
    public void Relogin()
    {
        if (Bot.Options.ReloginServer == null || !Bot.Servers.EnsureRelogin(Bot.Options.ReloginServer))
        {
            var servers = Bot.Servers.GetServers(true).Result;
            if (servers.Count == 0)
                Logger("Failed to relogin: could not fetch server details" + (Bot.Options.ReloginServer == null ? '.' : " or the find the server you've set in Options > Game."), messageBox: true, stopBot: true);
            Bot.Servers.EnsureRelogin((Bot.Player.IsMember == true)
                ? servers.First(s => s.Name != "Class Test Realm" && s.PlayerCount < s.MaxPlayers && s.Online).Name
                : servers.First(s => s.Name != "Class Test Realm" && !s.Upgrade && s.PlayerCount < s.MaxPlayers && s.Online).Name);
        }
    }

    /// <summary>
    /// Checks, and prompts for the latest Skua Version
    /// <param name="targetVersion">Current Skua Version to Check against</param>
    /// </summary>
    private void SkuaVersionChecker(string targetVersion)
    {
        if (Bot.Version == null || Version.Parse(targetVersion).CompareTo(Bot.Version) <= 0)
            return;

        if (Bot.ShowMessageBox($"This script requires Skua {targetVersion} or above, " +
        "click OK to open the download page of the latest release", "Outdated Skua detected", "OK").Text == "OK")
            Process.Start("explorer", "https://github.com/BrenoHenrike/Skua/releases/latest");
        Logger($"This script requires Skua {targetVersion} or above. Stopping the script", messageBox: true, stopBot: true);
    }

    ClassType currentClass = ClassType.None;
    bool usingSoloGeneric = false;
    bool usingFarmGeneric = false;
    /// <summary>
    /// Equips either the FarmClass or SoloClass from the CanChange section at the top of CoreBots 
    /// </summary>
    /// <param name="classToUse">Type "ClassType." and then either Farm or Solo in order to select which type it should swap too</param>
    public void EquipClass(ClassType classToUse)
    {
        if (currentClass == classToUse && Bot.Skills.TimerRunning)
            return;

        currentClass = classToUse;

        switch (classToUse)
        {
            case ClassType.Farm:
                if (_equipClass(usingFarmGeneric, FarmClass, FarmUseMode, FarmGearOn, FarmGear))
                    return;
                break;

            case ClassType.Solo:
                if (_equipClass(usingSoloGeneric, SoloClass, SoloUseMode, SoloGearOn, SoloGear))
                    return;
                break;
        }
        Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);

        bool _equipClass(bool usingGeneric, string className, ClassUseMode classMode, bool useEquipment, string[] equipment)
        {
            if (usingGeneric)
                return false;

            if (!CheckInventory(className) || !Bot.Inventory.Items.Any(x => x.Name.ToLower().Trim() == className.ToLower().Trim() && x.Category == ItemCategory.Class))
            {
                Logger("You do not own " + className);
                return false;
            }

            if (useEquipment && equipment.Any())
            {
                Sleep((int)(ActionDelay * 1.5));
                Equip(equipment);
            }

            string? equipedClass = Bot.Player.CurrentClass?.Name.Trim().ToLower();
            className = className.Trim().ToLower();
            // Logger($"Equiped Class: [{equipedClass}], Equiping: [{className}].", "Class Equiper");

            while (equipedClass != className)
            {
                if (Bot.Player.InCombat)
                    Bot.Combat.Exit();

                Equip(Bot.Inventory.Items.First(x => x.Name.ToLower().Trim() == className && x.Category == ItemCategory.Class).ID);
                equipedClass = Bot.Player.CurrentClass?.Name.Trim().ToLower();
            }

            // Logger($"Equiped Class: [{equipedClass}]", "Class Equiper");

            Bot.Skills.StartAdvanced(className, false, classMode);
            return true;
        }
    }

    /// <summary>
    /// Equips multiple items by their names from the bot's inventory if not already equipped.
    /// </summary>
    /// <param name="gear">Names of items to equip.</param>
    public void Equip(params string[] gear)
    {
        if (gear == null || gear.Length == 0)
            return;

        foreach (string item in gear)
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                continue;

            if (!Bot.Inventory.IsEquipped(item))
            {
                if (!CheckInventory(item))
                {
                    if (!Bot.ShouldExit)
                        Logger($"Equipping Failed: \"{item}\" not found in Inventory or Bank");
                    continue; // Use continue to move to the next item in the loop
                }
                if (!Bot.Inventory.TryGetItem(item, out InventoryItem? _item)) // Use nullable type
                {
                    if (!Bot.ShouldExit)
                        Logger($"Equipping Failed: Could not parse \"{item}\" from your inventory");
                    continue; // Use continue to move to the next item in the loop
                }
                if (_item == null) // Additional null check
                {
                    if (!Bot.ShouldExit)
                        Logger($"Equipping Failed: \"{item}\" is null after retrieval");
                    continue; // Use continue to move to the next item in the loop
                }
                _Equip(_item);
            }
        }
    }

    /// <summary>
    /// Equips multiple items by their IDs from the bot's inventory if not already equipped.
    /// </summary>
    /// <param name="gear">IDs of items to equip.</param>
    public void Equip(params int[] gear)
    {
        if (gear == null || gear.Length == 0)
            return;

        foreach (int item in gear)
        {
            if (item <= 0)
                continue;

            if (!Bot.Inventory.IsEquipped(item))
            {
                if (!CheckInventory(item))
                {
                    Logger($"Equipping Failed: \"{item}\" not found in Inventory or Bank");
                    continue;
                }
                if (!Bot.Inventory.TryGetItem(item, out var _item))
                {
                    Logger($"Equipping Failed: Could not parse \"{item}\" from your inventory");
                    continue;
                }
                _Equip(_item);
            }
        }
    }

    /// <summary>
    /// Handles the equipping process for a specific inventory item.
    /// </summary>
    /// <param name="item">Inventory item to equip.</param>
    private void _Equip(InventoryItem? item)
    {
        if (item == null)
        {
            Logger($"Equipping Failed: Parsed object for \"{item}\" is null");
            return;
        }

        // Exit combat mode
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            JumpWait();
            Sleep();
        }

        // Attempt to equip the item up to 5 times
        for (int attempt = 0; attempt < 5; attempt++)
        {
            if (Bot.Inventory.Items.Any(x => x != null && x.ID == item.ID && x.Equipped))
                break;

            JumpWait();

            switch (item.CategoryString.ToLower())
            {
                case "item": // Consumables
                    dynamic dItem = new ExpandoObject();
                    dItem.ItemID = item.ID;
                    dItem.sLink = Bot.Flash.GetGameObject<string>($"world.invTree.{item.ID}.sLink");
                    dItem.sES = item.ItemGroup;
                    dItem.sType = item.CategoryString;
                    dItem.sIcon = Bot.Flash.GetGameObject<string>($"world.invTree.{item.ID}.sIcon");
                    dItem.sFile = Bot.Flash.GetGameObject<string>($"world.invTree.{item.ID}.sFile");
                    dItem.bUpg = item.Upgrade ? 1 : 0;
                    dItem.sDesc = item.Description;
                    dItem.bEquip = item.Equipped ? 1 : 0;
                    dItem.sName = item.Name;
                    dItem.sMeta = item.Meta;

                    Bot.Flash.CallGameFunction("toggleItemEquip", dItem);
                    Sleep(1500);
                    break;

                default:
                    Bot.Inventory.EquipItem(item.ID);
                    Sleep(1500);
                    break;
            }

            Sleep();
        }
    }

    /// <summary>
    /// Equips items cached before bot operation.
    /// </summary>
    public void EquipCached()
    {
        Equip(EquipmentBeforeBot.ToArray());
    }

    /// <summary>
    /// Switches the player's Alignment to the input Alignment type
    /// </summary>
    /// <param name="side">Type "Alignment." and then Good, Evil or Chaos in order to select which Alignment it should swap too</param>
    public void ChangeAlignment(Alignment side)
    {
        Bot.Send.Packet($"%xt%zm%updateQuest%{Bot.Map.RoomID}%41%{(int)side}%");
        Sleep(ActionDelay * 2);
    }

    /// <summary>
    /// Checks if a specific achievement is obtained.
    /// </summary>
    /// <param name="ID">The ID of the achievement to check.</param>
    /// <param name="ia">Optional parameter for the achievement identifier (default is "ia0").</param>
    /// <returns>True if the achievement is obtained; otherwise, false.</returns>
    public bool HasAchievement(int ID, string ia = "ia0") => Bot.Flash.CallGameFunction<bool>("world.getAchievement", ia, ID);

    public void SetAchievement(int ID, string ia = "ia0")
    {
        if (!HasAchievement(ID, ia))
            Bot.Send.Packet($"%xt%zm%setAchievement%{Bot.Map.RoomID}%{ia}%{ID}%1%");
    }
    /// <summary>
    /// Checks if the bot has a web badge with the specified ID.
    /// </summary>
    /// <param name="badgeID">The ID of the web badge to check.</param>
    /// <returns>True if the bot has the web badge; otherwise, false.</returns>
    public bool HasWebBadge(int badgeID) => Badges.Contains(badgeID);

    /// <summary>
    /// Checks if the bot has a web badge with the specified name.
    /// </summary>
    /// <param name="badgeName">The name of the web badge to check.</param>
    /// <returns>True if the bot has the web badge; otherwise, false.</returns>
    public bool HasWebBadge(string badgeName) => Badges.Contains(badgeName);


    public List<Badge> Badges
    {
        get
        {
            if (CharacterID <= 0)
                return new();
            return JsonConvert.DeserializeObject<List<Badge>>(GetRequest($"https://account.aq.com/CharPage/Badges?ccid={CharacterID}")) ?? new();
        }
    }

    private int _characterID;
    public int CharacterID
    {
        get
        {
            if (_characterID <= 0)
                _characterID = Bot.Flash.GetGameObject<int>("world.myAvatar.objData.CharID");
            return _characterID;
        }
    }

    private HttpClient? _webClient;
    public HttpClient WebClient
    {
        get
        {
            if (_webClient == null)
            {
                _webClient = new();
                _webClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            }
            return _webClient;
        }
    }

    /// <summary>
    /// Performs a GET request to the specified URL and retrieves the response as a string.
    /// </summary>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <returns>The response content as a string.</returns>
    public string GetRequest(string url)
    {
        return _getRequest().Result;

        async Task<string> _getRequest()
        {
            string toReturn = string.Empty;
            await Task.Run(async () =>
            {
                try
                {
                    toReturn = await WebClient.GetStringAsync(url);
                }
                catch { }
            });
            return toReturn;
        }
    }

    /// <summary>
    /// Sets the saved state to the specified status.
    /// </summary>
    /// <param name="on">True to turn on saved state; false to turn it off (default is true).</param>
    public void SavedState(bool on = true)
    {
        // Method implementation intentionally left blank as it is currently unused.
        GC.Collect();

    }


    /// <summary>
    /// Generates an array of integers from a starting value to an ending value (inclusive).
    /// </summary>
    /// <param name="from">The starting integer value.</param>
    /// <param name="to">The ending integer value.</param>
    /// <returns>An array of integers from 'from' to 'to' (inclusive).</returns>
    public int[] FromTo(int from, int to)
    {
        List<int> toReturn = new();
        for (int i = from; i < to + 1; i++)
            toReturn.Add(i);
        return toReturn.ToArray();
    }

    /// <summary>
    /// Banks miscellaneous AC-tagged inventory items.
    /// </summary>
    public void BankACMisc()
    {
        // Items to never bank
        int[] exemptIDs = { 18927, 38575 }; // Treasure Potion, Dark Potion

        // Allowed categories for inventory items
        List<ItemCategory> allowedCategories = new()
    {
        ItemCategory.Note,
        ItemCategory.Item,
        ItemCategory.Resource,
        ItemCategory.QuestItem
    };

        // Optionally include ServerUse if boosts aren't active
        if (!Bot.Boosts.Enabled && (CBO_Active() || !new[] { "doGoldBoost", "doClassBoost", "doRepBoost", "doExpBoost" }
            .Any(flag => CBOBool(flag, out bool enabled) && enabled)))
            allowedCategories.Add(ItemCategory.ServerUse);

        // --- Inventory: Misc AC items to bank ---
        var toBank = Bot.Inventory.Items
            .Where(item =>
                item is not null &&
                item.Coins &&
                !item.Equipped &&
                allowedCategories.Contains(item.Category) &&
                !BankingBlackList.Contains(item.Name) &&
                !exemptIDs.Contains(item.ID))
            .Select(item => item.ID)
            .ToArray();

        if (toBank.Length > 0)
        {
            Logger("Banking misc AC items", toBank.Length + " items");
            ToBank(toBank);
        }
    }

    /// <summary>
    /// Banks miscellaneous AC-tagged non-equipped House items.
    /// </summary>
    public void BankACHouseItems()
    {
        var toHouseBank = Bot.House.Items
            .Where(item => item != null && item.Coins && !item.Equipped)
            .Select(item => item.ID)
            .ToArray();

        if (toHouseBank.Length > 0)
        {
            Logger("Banking non-equipped house items", toHouseBank.Length + " items");
            ToHouseBank(toHouseBank);
        }
    }



    /// <summary>
    /// Banks unenhanced AdventureCoins (AC) gear based on specified whitelist and conditions.
    /// </summary>
    public void BankACUnenhancedGear()
    {
        List<ItemCategory> Whitelisted = new() { ItemCategory.Class, ItemCategory.Helm, ItemCategory.Cape };
        ToBank(Bot.Inventory.Items.Where(i =>
            (Whitelisted.Contains(i.Category) ||
            i.ItemGroup == "Weapon") &&
            i.Coins &&
            i.EnhancementLevel == 0 &&
            !i.Equipped &&
            !SoloGear.Contains(i.Name) &&
            !FarmGear.Contains(i.Name)
        ).Select(i => i.ID).ToArray());
    }

    public Option<bool> SkipOptions = new("SkipOption", "Skip this window next time", "You will be able to return to this screen via [Scripts] -> [Edit Script Options] if you wish to change anything.", false);
    public bool DontPreconfigure = true;

    public const string reinstallCleanFlash = ". If the issue persists, try the following things in the order they are here:\n - Restart the client.\n - Restart your computer.\n - Reinstall CleanFlash";

    /// <summary>
    /// Displays a message indicating that files starting with "Core" are for storage purposes and stops the bot.
    /// </summary>
    public void RunCore()
    {
        Bot.ShowMessageBox("Files that start with the word \"Core\" are not meant to be run, these are for storage. Please select the correct script.", "Core File Info");
        Bot.Stop(true);
    }
    public void ByPassCheck()
    {
        // Wait until the player is alive
        while (!Bot.ShouldExit && !Bot.Player.Alive) { Sleep(); }

        // Attempt to get the level from Flash
        string? BypassLevel = Bot.Flash.GetGameObject("world.myAvatar.objData.intLevel");

        // Check if the object is null and exit early if so
        if (string.IsNullOrEmpty(BypassLevel))
            return;

        // Get the player's level from the Flash object
        int flashLevel = int.TryParse(BypassLevel, out int level) ? level : 0;

        if (flashLevel >= 100 || Bot.Player.Level >= 100)
            return;


        // Check if the current map is one of the locked maps
        var levelLockedMaps = new[]
        {
            new { Map = "icestormunder", LevelRequired = 75 },
            new { Map = "icewing", LevelRequired = 75 },
            new { Map = "battlegrounde", LevelRequired = 61 },
            new { Map = "voidxyfrag", LevelRequired = 80 },
            new { Map = "voidnerfkitten", LevelRequired = 80 }
        };

        // Check if the current map is in the locked maps
        var currentMap = levelLockedMaps.FirstOrDefault(m => m.Map == Bot.Map.Name);
        if (currentMap == null || flashLevel >= currentMap.LevelRequired || Bot.Player.Level >= currentMap.LevelRequired)
            return; // Exit if the current map is not locked or player level meets/exceeds requirement

        if (flashLevel < currentMap.LevelRequired)
            Logger("Bypass Broke, resetting level");

        // Store the current map name
        string previousMap = Bot.Map.Name;

        // Jump to the "whitemap"
        Join("whitemap");

        // Jump back to the previous map
        Join(previousMap);
        Sleep();

        // Send a client packet to set the player's level to 100
        Bot.Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"levelUp\",\"intExpToLevel\":\"0\",\"intLevel\":100}}}", type: "json");

        // Sleep after sending the packet to give time for processing
        Sleep();
    }

    /// <summary>
    /// Filters out items that are neither weapons nor armor from the inventory.
    /// </summary>
    /// <param name="x">The inventory item to evaluate.</param>
    /// <returns>True if the item is neither a weapon nor armor, otherwise false.</returns>
    public bool NoneEnhancableFilter(InventoryItem x)
    {
        return
         x.Category != ItemCategory.Sword
            && x.Category != ItemCategory.Axe
            && x.Category != ItemCategory.Dagger
            && x.Category != ItemCategory.Gun
            && x.Category != ItemCategory.HandGun
            && x.Category != ItemCategory.Rifle
            && x.Category != ItemCategory.Bow
            && x.Category != ItemCategory.Mace
            && x.Category != ItemCategory.Gauntlet
            && x.Category != ItemCategory.Polearm
            && x.Category != ItemCategory.Staff
            && x.Category != ItemCategory.Wand
            && x.Category != ItemCategory.Whip;
    }

    public void EquipBestItemsForMeta(Dictionary<string, string[]> categoryMetaMapping)
    {
        // Define unwanted meta types
        HashSet<string> unwantedMetaTypes = new() { "AutoAdd", "Drakath" };

        // Define weapon categories for matching
        var weaponCategories = new[]
        {
        ItemCategory.Sword,
        ItemCategory.Axe,
        ItemCategory.Dagger,
        ItemCategory.Gun,
        ItemCategory.HandGun,
        ItemCategory.Rifle,
        ItemCategory.Bow,
        ItemCategory.Mace,
        ItemCategory.Gauntlet,
        ItemCategory.Polearm,
        ItemCategory.Staff,
        ItemCategory.Wand,
        ItemCategory.Whip
    };

        // Function to calculate the score for the desired meta and additional metas
        double CalculateMetaScore(ItemBase item, string[] metaPriorities, out double mainMetaValue)
        {
            mainMetaValue = 0;

            if (item.Meta == null) return 0;

            // Clean unwanted meta types from the item's meta string
            string cleanedMeta = unwantedMetaTypes
                .Aggregate(item.Meta, (currentMeta, unwanted) =>
                    currentMeta.Replace(unwanted + ",", string.Empty))
                .TrimEnd(',')
                .Replace(",+", ",")
                .Trim(',');

            // Split the cleaned meta string into key-value pairs
            var metaPairs = cleanedMeta
                .Split('\n')
                .SelectMany(line => line.Split(','))
                .Select(metaEntry => metaEntry.Split(':'))
                .Where(metaPair => metaPair.Length == 2); // Ensure it is a valid meta pair

            // Calculate the main meta value and sum other meta scores
            double totalScore = 0;
            foreach (var pair in metaPairs)
            {
                string metaKey = pair[0];
                if (double.TryParse(pair[1], out double metaValue))
                {
                    // Check if the current meta is in the desired priorities
                    if (metaPriorities.Contains(metaKey))
                    {
                        mainMetaValue = Math.Max(mainMetaValue, metaValue); // Save the highest main meta value
                    }
                    else
                    {
                        totalScore += metaValue; // Sum other meta values as secondary score
                    }
                }
            }

            // Return the total score which combines other metas (does not include the mainMetaValue)
            return totalScore;
        }

        // Variables to track the best items across categories
        Dictionary<string, ItemBase?> bestItems = new();
        Dictionary<string, double> bestMainMetaValues = new();
        Dictionary<string, double> bestAdditionalMetaScores = new();

        // Iterate through each category and its meta priorities
        foreach (var categoryMeta in categoryMetaMapping)
        {
            string categoryKey = categoryMeta.Key;
            string[] metaPriorities = categoryMeta.Value;

            // Initialize best item trackers
            bestItems[categoryKey] = null;
            bestMainMetaValues[categoryKey] = 0;
            bestAdditionalMetaScores[categoryKey] = 0;

            var allItems = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            // Filter out null items
            .Where(x => x != null &&
            // Include items that are either non-enhancable (like Armor or Pets)
            // or items that are enhancable and have an enhancement level greater than 0
            (NoneEnhancableFilter(x) || (x.EnhancementLevel > 0 && !NoneEnhancableFilter(x))))
            .Cast<ItemBase>()
            .ToList();

            if (!Bot.Player.IsMember)
                allItems.RemoveAll(x => x.Upgrade);

            foreach (ItemBase item in allItems)
            {
                // Check if the current item matches the specified category
                bool isCategoryMatch = categoryKey switch
                {
                    "Weapon" => weaponCategories.Contains(item.Category),
                    "Pet" => item.Category == ItemCategory.Pet, // Check for Pet category
                    _ => item.Category.ToString() == categoryKey // Match category directly
                };

                if (!isCategoryMatch) continue;

                // Calculate the score for the item
                double currentAdditionalMetaScore = CalculateMetaScore(item, metaPriorities, out double currentMainMetaValue);

                // Only consider items with a main meta value greater than zero
                if (currentMainMetaValue > 0)
                {
                    // Check if the current item has a better main meta value
                    if (currentMainMetaValue > bestMainMetaValues[categoryKey] ||
                        (currentMainMetaValue == bestMainMetaValues[categoryKey] && currentAdditionalMetaScore > bestAdditionalMetaScores[categoryKey]))
                    {
                        bestItems[categoryKey] = item;
                        bestMainMetaValues[categoryKey] = currentMainMetaValue;
                        bestAdditionalMetaScores[categoryKey] = currentAdditionalMetaScore;
                    }
                }
            }
        }

        // Equip the best items found for each category
        foreach (var category in bestItems.Keys)
        {
            if (bestItems[category] != null)
            {
                var item = bestItems[category];
                if (item != null) // Additional null check
                {
                    Logger($"Equipping best item: {item.Name} in category {category}.");
                    Equip(item.ID);
                }
            }
        }
    }

    #endregion

    #region Map

    /// <summary>
    /// Jumps to the desired cell and set spawn point
    /// </summary>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="ignoreCheck"></param>
    public void Jump(string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false)
    {
        if (Bot.Player.Cell != null && Bot.Player.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase))
        {
            Bot.Player.SetSpawnPoint();
            return;
        }

        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower()) ?? pad;

        if (!ignoreCheck && Bot.Player.Cell == cell)
            return;

        while (!Bot.ShouldExit && Bot.Player.Cell != cell)
        {
            // Bot.Send.Packet($"%xt%zm%moveToCell%{Bot.Map.RoomID}%{cell}%{pad}%");
            if (!string.IsNullOrEmpty(cell) && Bot.Player.Cell != cell)
                Bot.Map.Jump(cell, pad);
            Bot.Wait.ForCellChange(cell ?? "Enter");
            Sleep();

            if (Bot.Player.Cell == cell)
                break;
        }
        Bot.Player.SetSpawnPoint();
        GC.Collect();
    }


    /// <summary>
    /// Searches for a cell without monsters and jumps to it. If none is found, it jumps twice in the current cell.
    /// This method is designed to help the player exit combat by moving to a non-combat cell.
    /// The <see cref="ExitCombatDelay"/> is used to determine the delay before exiting combat.
    /// </summary>
    public void JumpWait()
    {
        // Initialize blacklisted cells and bot options
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        HashSet<string> blackListedCells = Bot.Monsters.MapMonsters
     .Select(monster => monster.Cell)
     .Union(
         Bot.Map.Cells
             .Where(cell => cell != null
                 && (Regex.IsMatch(cell, @"(^cut\w*$)|(^\w*cut$)|(^cut$)|(^r\d+$)|^(bs\d+|ar\d+|ms\d+|apo\d+|guild)$", RegexOptions.IgnoreCase)
                 || BlackListedJumptoCells.Contains(cell)
                 || (Bot.Player.Cell != "Enter" && cell.Contains("Enter"))
             ))
     )
     .ToHashSet();

        // Proceed to filtering cases based on the blacklisted cells
        ProceedToFilteringCases(blackListedCells.Distinct().ToHashSet());
    }

    private (string Cell, string Pad) TryFindSuitableCell(HashSet<string> blackListedCells)
    {
        string? cell = null;
        blackListedCells.UnionWith(BlackListedJumptoCells);

        // Try to find a valid cell, up to 5 attempts
        for (int i = 0; i < 5; i++)
        {
            // First try to find a cell with "Enter", then try a non-blacklisted cell
            cell = Bot.Map.Cells.FirstOrDefault(x => x.Contains("Enter")) ??
                   Bot.Map.Cells.FirstOrDefault(x => !blackListedCells.Contains(x));

            if (cell != null) // If a valid cell is found
                break;

            Logger($"Attempt {i + 1}: Suitable cell not found. Retrying...");
            Sleep(1000); // Wait for 1 second before retrying
        }

        // If no suitable cell is found, return a default value
        if (cell == null)
        {
            return (string.Empty, "Left");
        }

        // Determine the pad for the found cell
        string pad = Bot.Map.Cells.Any(x => x.Contains("Enter")) ? "Spawn" : "Left";
        return (cell, pad);
    }

    private void ProceedToFilteringCases(HashSet<string> blackListedCells)
    {
        blackListedCells = Bot.Monsters.MapMonsters
        .Select(monster => monster.Cell)
        .Union(
            Bot.Map.Cells
                .Where(cell => cell != null
                    && (Regex.IsMatch(cell, @"(^cut\w*$)|(^\w*cut$)|(^cut$)|(^r\d+$)|^(bs\d+|ar\d+|ms\d+|apo\d+|guild)$", RegexOptions.IgnoreCase)
                    || BlackListedJumptoCells.Contains(cell)
                    || (Bot.Player.Cell != "Enter" && cell.Contains("Enter"))
                ))
        )
        .ToHashSet();

        switch (Bot.Map.Name)
        {
            case "hbchallenge":
                blackListedCells.UnionWith(new[] { "r7" });
                break;

            case "Gluttony":
                if (Bot.Map.Cells != null)
                {
                    blackListedCells.UnionWith(Bot.Map.Cells.Where(x => x.StartsWith("Enter")));
                }
                break;

            case "xantown":
                string[]? cells = Bot.Map.Cells?.Where(x => x != "r3").ToArray();
                if (cells != null)
                {
                    blackListedCells.UnionWith(cells);
                }
                break;

            case "darkoviaforest":
            case "safiria":
            case "lycan":
                blackListedCells.UnionWith(new[] { "Quest" });
                break;

            case "mobius":
                // Requires quest "The Star of Flames [2364]" to be completed
                blackListedCells.UnionWith(new[] { "Hair" });
                break;

            case "beehive":
                blackListedCells.UnionWith(new[] { "Dance" });
                break;

            case "oaklore":
                blackListedCells.UnionWith(new[] { "Enter", "r1" });
                break;

            case "pyrewatch":
                blackListedCells.UnionWith(new[] { "r3", "r4", "r5", "r7", "r12" });
                break;

            case "shadowfall":
                blackListedCells.UnionWith(new[] { "New6" });
                break;

            case "bloodmoon":
                blackListedCells.UnionWith(new[] { "Enter", "r17" });
                break;

            case "wanders":
                Bot.Map.Jump("Boss", "left");
                Bot.Sleep(2500);
                blackListedCells.UnionWith(Bot.Player.Cell == "Boss" ? new[] { "r25", "Enter", "Enter2" } : new[] { "Boss", "Enter", "Enter2" });
                break;

            case "zephyrus":
                blackListedCells.UnionWith(new[] { "R1", "Enter" });
                break;

            case "portalundead":
                blackListedCells.UnionWith(new[] { "Portal", "Gate" });
                break;

            case "icestormarena":
                blackListedCells.UnionWith(new[] { "r23" });
                break;

            case "battlecon":
                blackListedCells.UnionWith(new[] { "rFight" });
                break;

            case "necroU":
                blackListedCells.UnionWith(new[] { "Leave", "r6" });
                break;

            default:
                break;
        }

        if (!IsMember)
            blackListedCells.Add("Eggs");

        // Jump to a viable cell (or retry)
        IEnumerable<string> viableCells = Bot.Map.Cells?.Except(BlackListedJumptoCells.Concat(blackListedCells)) ?? Enumerable.Empty<string>();
        (string, string) cellPad = viableCells.Any()
            ? (viableCells.First(), Bot.Map.Name == "battleon" ? "Spawn" : "Left")
            : (Bot.Player.Cell, Bot.Player.Pad);
        PerformJump(cellPad, viableCells.Any() ? 1 : 2);
    }

    private void PerformJump((string Cell, string Pad) cellPad, int jumpCount)
    {
        if (lastMapJW != Bot.Map.Name || lastCellPadJW != cellPad)
        {
            for (int i = 0; i < jumpCount; i++)
            {
                Bot.Map.Jump(cellPad.Cell, cellPad.Pad);
                Bot.Wait.ForTrue(() => Bot.Player.Cell == cellPad.Cell, 20);
            }

            lastMapJW = Bot.Map.Name;
            lastCellPadJW = cellPad;

            Sleep(ExitCombatDelay < 200 ? ExitCombatDelay : ExitCombatDelay - 200);
        }
    }

    private string lastMapJW = string.Empty;
    private (string, string) lastCellPadJW = (string.Empty, string.Empty);

    // Combined static and dynamic blacklist
    public string[] BlackListedJumptoCells = new[]
    { "Wait", "Blank", "Out", "CutMikoOrochi", "innitRoom", "Video", "Leave", "moveFrame", "Fall", "Move", "Cut", "Movie" };

    /// <summary>
    /// Joins a map and does bonus steps for said map if needed
    /// </summary>
    /// <param name="map">The name of the map</param>
    /// <param name="cell">The cell to jump to</param>
    /// <param name="pad">The pad to jump to</param>
    /// <param name="publicRoom">Whether or not it should be a public room, if PrivateRoom is on in the CanChange section on the top of CoreBots</param>
    /// <param name="ignoreCheck">If set to true, the bot will not check if the player is already in the given room</param>
    public void Join(string? map, string? cell = "Enter", string pad = "Spawn", bool publicRoom = false, bool ignoreCheck = false)
    {
        if (string.IsNullOrEmpty(map))
        {
            Logger("Map is null, cannot join.");
            return;
        }

        if (PrivateRooms && PrivateRoomNumber.ToString().Length > 6)
        {
            PrivateRoomNumber = int.Parse(PrivateRoomNumber.ToString()[..6]);
        }

        map = map!.Replace(" ", "").Replace('I', 'i');
        map = map.ToLower() == "tercess" ? "tercessuinotlim" : map.ToLower();
        string strippedMap = map.Contains('-') ? map.Split('-').First() : map;
        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());

        if (Bot.Map.Name != null && Bot.Map.Name.ToLower() == strippedMap && !ignoreCheck)
            return;

        //if aggro/aggroall is enabled when joining a map, disable it [forced]
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;

        Sleep();

        switch (strippedMap)
        {
            default:
                if (Bot.Map.Name != null && Bot.Map.Name == "pyrewatch")
                    JumpWait();
                tryJoin();
                break;

            // case "map":
            //     SimpleQuestBypass((000, 000));
            //     break;

            #region Simple Quest Bypasses
            case "marsh2":
                SimpleQuestBypass((58, 8));
                break;

            case "nightmare":
                SimpleQuestBypass((192, 9));
                break;

            case "ascendeclipse":
                if (!CheckInventory("Rite of Ascension"))
                    Logger("Item Required is a server-side check, cannot ghost it.");
                else
                {
                    SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%{map}-{PrivateRoomNumber}%");
                    Bot.Wait.ForMapLoad(map);
                }
                break;

            case "solsticemoon":
            case "midnightsun":
                SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%{map}-{PrivateRoomNumber}%");
                Bot.Wait.ForMapLoad(map);
                break;

            case "temple":
                SimpleQuestBypass((49, 25));
                break;

            case "wanders":
                //2 spawn cells makes joining fucky
                if (cell == "Enter")
                {
                    cell = "Enter2";
                    pad = "Down";
                }
                SimpleQuestBypass((176, 6));
                break;

            case "kitsune":
                SimpleQuestBypass((25, 22));
                break;

            case "elemental":
                SimpleQuestBypass((32, 35));
                break;

            case "twilightedge":
                SimpleQuestBypass((156, 23));
                break;

            case "dragonkoiz":
                SimpleQuestBypass((25, 22));
                break;

            case "titandrakath":
                SimpleQuestBypass((470, 18));
                JumpWait();
                map = strippedMap + "-999999";
                tryJoin();
                while (!Bot.Inventory.IsEquipped(71529))
                    Sleep();
                break;

            case "desoloth":
                SimpleQuestBypass((56, 35));
                break;

            case "xancave":
                SimpleQuestBypass((53, 35));
                break;

            case "shadowgrove":
                SimpleQuestBypass((315, 7));
                break;

            case "stalagbite":
                SimpleQuestBypass((22, 35));
                break;

            case "maloth":
                SimpleQuestBypass((246, 23));
                break;

            case "originul":
            case "fiendshard":
                SimpleQuestBypass((387, 16));
                break;

            case "mummies":
                SimpleQuestBypass((97, 16));
                break;

            case "doomvault":
                SimpleQuestBypass((126, 18));
                break;

            case "pyramid":
            case "djinn":
                SimpleQuestBypass((36, 28));
                break;

            case "ultradrakath":
                SimpleQuestBypass((182, 5));
                break;

            case "backroom":
                SimpleQuestBypass((402, 12));
                break;

            // case "venomvaults":
            //     SimpleQuestBypass((117, 7));
            //     break;

            // case "stormtemple":
            //     SimpleQuestBypass((117, 17));
            //     break;

            case "chaoscave":
            case "lycanwar":
                SimpleQuestBypass((26, 22));
                break;

            case "timespace":
                SimpleQuestBypass((100, 14));
                break;

            case "transformation":
                SimpleQuestBypass((405, 12));
                break;

            case "ebilcorphq":
                SimpleQuestBypass((431, 9));
                break;

            case "necrodungeon":
                SimpleQuestBypass((77, 18));
                break;

            case "oddities":
                SimpleQuestBypass((456, 13));
                break;

            case "championdrakath":
                SimpleQuestBypass((182, 7));
                break;

            case "glacera":
                SimpleQuestBypass((225, 21));
                break;

            case "ultratyndarius":
                SimpleQuestBypass((412, 22));
                break;

            case "Creepy":
                tryJoin();
                Bot.Wait.ForCellChange("Cut1");
                JumpWait();
                Bot.Wait.ForCellChange("Skip");
                JumpWait();
                break;

            case "towerofdoom":
            case "towerofdoom2":
            case "towerofdoom3":
            case "towerofdoom4":
            case "towerofdoom5":
            case "towerofdoom6":
            case "towerofdoom7":
            case "towerofdoom8":
            case "towerofdoom9":
            case "towerofdoom10":
                SimpleQuestBypass((159, 10));
                break;

            case "onslaughttower":
                tryJoin();
                if (!CheckInventory(2047))
                {
                    SendPackets("%xt%zm%getMapItem%169031%67%");
                    Sleep(2500);
                    SendPackets("%xt%zm%equipItem%169031%2047%");
                }
                else
                {
                    JumpWait();
                    SendPackets("%xt%zm%equipItem%169031%2047%");
                }
                break;


            case "wolfwing":
                SimpleQuestBypass((26, 23));
                break;

            case "manacradle":
                SimpleQuestBypass((488, 20));
                break;

            case "shadowattack":
                SimpleQuestBypass((175, 18));
                break;

            case "dreadhaven":
                SimpleQuestBypass((175, 20));
                break;

            case "darkoviaforest":
            case "lycan":
            case "safiria":
                SimpleQuestBypass((26, 23));
                break;

            #endregion

            #region Private Simple Quest Bypasses
            case "celestialarenab":
            case "celestialarenac":
            case "celestialarenad":
                PrivateSimpleQuestBypass((249, 20));
                break;

            case "confrontation":
                PrivateSimpleQuestBypass((175, 20));
                break;
            #endregion

            #region Ghost Item Bypasses

            case "deaddragon":
                GhostItemBypass(37377, "deaddragon Map Bypass");
                break;

            case "nostalgiaquest":
                GhostItemBypass(37378, "NostalgiaQuest Map Bypass");
                break;

            case "revenant":
                GhostItemBypass(47465, "Revenant Map Bypass", category: ItemCategory.Class);
                break;

            #endregion

            #region Special Cases
            case "tercessuinotlim":
                var prereqQuest1 = Bot.Quests.HasBeenCompleted(9540);
                var prereqQuest2 = Bot.Quests.HasBeenCompleted(9541);

                if (!prereqQuest1)
                {
                    OneTimeMessage(
                        "WARNING!",
                        "This map now requires a one-time completion of \"Beyond the Portal\"\n" +
                        "Not sure why it loads tercessuinotlim first, but get over it :|",
                        messageBox: false
                    );

                    SimpleQuestBypass((15, 8), (542, 2));
                    Join("citadel");
                    Jump("m22", "Left");
                    EnsureAccept(9540);
                    KillMonster("citadel", "m22", "Left", "Death's Head", "Death's Head Bested");
                    EnsureComplete(9540);

                    prereqQuest1 = true;
                }

                if (!prereqQuest2)
                {
                    ChainComplete(9541);

                    prereqQuest2 = true;
                }

                Jump("m22", "Left");
                tryJoin();
                break;

            case "druids":
                tryJoin();
                Bot.Wait.ForItemEquip(18524);
                break;

            #region Quest Prog swaps spawn cell
            case "oaklore":
                if (!string.IsNullOrEmpty(cell) && cell == "Enter" || string.IsNullOrEmpty(cell))
                    cell = "r1";
                tryJoin();
                break;

            case "bloodmoon":
                if (isCompletedBefore(6058) && !string.IsNullOrEmpty(cell) && cell == "Enter")
                {
                    cell = "r17";
                    pad = "Left";
                }
                tryJoin();
                break;
            #endregion

            case "collection":
                JumpWait();
                if (Bot.Map.Name != null && Bot.Map.Name != map)
                    Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : map, "Begin", "Spawn", autoCorrect: false);
                Bot.Wait.ForMapLoad(map);
                break;

            case "doomvaultb":
                SetAchievement(18);
                SimpleQuestBypass((127, 26), (126, 18)); //3004 + 3008
                break;

            case "prison":
                joinedPrison = true;
                JumpWait();
                tryJoin();
                joinedPrison = false;
                break;

            case "hyperium":
                JumpWait();
                Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%5041%525,275%{(PrivateRooms ? (map + "-" + PrivateRoomNumber) : map)}%");
                Bot.Wait.ForMapLoad("hyperium");
                break;

            case "moonyard":
                JumpWait();
                Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%5041%525,275%{(PrivateRooms ? (map + "-" + PrivateRoomNumber) : map)}%");
                Bot.Wait.ForMapLoad("hyperium");
                Jump("R10");
                Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : "moonyard", autoCorrect: false);
                Bot.Wait.ForMapLoad("moonyard");
                Sleep();
                Bot.Wait.ForItemEquip(8733);
                Bot.Wait.ForCellChange("MoonCut");
                break;

            case "moonyardb":
                JumpWait();
                Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%5041%525,275%{(PrivateRooms ? ("hyperium-" + PrivateRoomNumber) : "hyperium")}%");
                Bot.Wait.ForMapLoad("hyperium");
                Jump("R10");
                Bot.Map.Join(PrivateRooms ? "moonyard-" + PrivateRoomNumber : "moonyard", autoCorrect: false);
                Bot.Wait.ForMapLoad("moonyard");
                Bot.Wait.ForItemEquip(8733);
                SimpleQuestBypass((28, 35));
                Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : map, autoCorrect: false);
                Bot.Wait.ForMapLoad(strippedMap);
                break;

            case "zephyrus":
                JumpWait();
                Join("hyperium");
                Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%5041%525,275%{(PrivateRooms ? (map + "-" + PrivateRoomNumber) : map)}%");
                Bot.Wait.ForMapLoad("hyperium");
                Jump("R10");
                tryJoin();
                Bot.Wait.ForCellChange("Enter");
                Jump("R1", "Up");
                Jump("R2", "Up");
                break;

            case "icestormarena":
                JumpWait();
                if (Bot.Map.Name != null && Bot.Map.Name != map)
                    Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : map);
                Bot.Wait.ForMapLoad("icestormarena");
                Bot.Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"levelUp\",\"intExpToLevel\":\"0\",\"intLevel\":100}}}", type: "json");
                Sleep();
                if (cell != null && Bot.Player.Cell != cell)
                    Bot.Map.Jump(cell ?? "Enter", pad);
                Bot.Wait.ForCellChange(cell ?? "Enter");
                break;

            #endregion

            #region Always Private
            // PvP
            case "doompirate":
            case "bludrutbrawl":
            case "dagepvp":
            case "legionpvp":
            case "deathpitbrawl":
            // Room Limit: 1
            case "queeniona":
            case "baconcatb":
            case "caroling":
            case "chaosbattle":
            case "chaoslord":
            case "chaosrealm":
            case "darkthronehub":
            case "drakathfight":
            case "dragonfire":
            case "dragonkoi":
            case "falcontower":
            case "finalbattle":
            case "finalshowdown":
            case "herotournament":
            case "infernalarena":
            case "malgor":
            case "nothing":
            case "ravenscar":
            case "superslayin":
            case "treetitanbattle":
            case "tlapd":
            case "trickortreat":
            case "vordredboss":
                // Special
                JumpWait();
                map = strippedMap + "-999999";
                tryJoin();
                break;
            #endregion

            #region BuyHouse (for a merge)
            case "buyhouse":
                Logger($"\"{map}\" is a public map.. and non-privateable, so blame ae for that.. tho its required for some things so this will be forced public");
                JumpWait();
                if (Bot.Map.Name != null && Bot.Map.Name != map)
                {
                    Bot.Map.Join("buyhouse", "Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForMapLoad(map);
                }
                break;
            #endregion BuyHouse (for a merge)

            #region baconcat.. is annoying
            case "baconcat":
                // Bot.Quests.UpdateQuest(5108);
                JumpWait();
                map = strippedMap + "-999999";
                tryJoin();
                Bot.Wait.ForCellChange(cell ?? "Enter");
                break;
            #endregion baconcat.. is annoying

            #region Maps that cant be private and you must do yourself. (thanks AE)
            case "fearhouse":
            case "warehouse":
                Skua.Core.Models.DialogResult ForcePublic = Bot.ShowMessageBox(
                                    $"Do you want to join the Following map: \"{map}\"\n" +
                                    "using a public room?\n" +
                                    "(Bot will stop otherwise)", "PublicRoom Only",
                                    "Yes", "No"
                                );

                if (ForcePublic.Value == 1)
                    Logger("Unfortunitaly AE forgot to make these maps privateable only\n" +
                    "to continue \"yes\" must be selcted, otherwise for `allstories` just comment it out with 2 /'s", stopBot: true);
                else
                {
                    Logger("You've Chosen to bot publicly... good luck in this *public only* map.");
                    JumpWait();
                    if (Bot.Map.Name != null && Bot.Map.Name != map)
                        Bot.Map.Join(map);
                    Bot.Wait.ForMapLoad(map);
                }
                break;
            #endregion

            #region Bypass Banned
            // This doesn't mean that you cant do a bypass inside the boat itself, it just can't be in Join because it fucks up CanBuy
            // Write the ID that can be used for the bypass in a comment after it, so people can easily
            // fetch it if they are gonna used a banned map
            case "downbelow": // 8107
                goto default;
                #endregion
        }

        if (strippedMap == Bot.Map.Name?.ToLower())
        {
            if (ButlerOnMe())
            {
                string[] lockedMaps =
                {
                    "tercessuinotlim",
                    "doomvaultb",
                    "doomvault",
                    "shadowrealmpast",
                    "shadowrealm",
                    "battlegrounda",
                    "battlegroundb",
                    "battlegroundc",
                    "battlegroundd",
                    "battlegrounde",
                    "battlegroundf",
                    "confrontation",
                    "darkoviaforest",
                    "doomwood",
                    "hollowdeep",
                    "hyperium",
                    "willowcreek",
                    "shadowlordpast",
                    "binky",
                    "superlowe",
                    "voidflibbi",
                    "voidnightbane",
                    "voidxyfrag",
                    "voidnerfkitten",
                    "seavoice"
                };
                if (lockedMaps.Contains(strippedMap))
                    WriteFile(ButlerLogPath(), Bot.Map.FullName);
            }

            if (cell != null && Bot.Player.Cell != cell)
                Bot.Map.Jump(cell, pad);

            Sleep(1500);
        }

        void tryJoin()
        {
            try
            {
                #region ignore this
                if (Bot.Events == null)
                {
                    Logger("Bot.Events is null.");
                    return;
                }

                if (Bot.Options == null)
                {
                    Logger("Bot.Options is null.");
                    return;
                }

                if (Bot.Wait == null)
                {
                    Logger("Bot.Wait is null.");
                    return;
                }

                if (Bot.Map == null)
                {
                    Logger("Bot.Map is null.");
                    return;
                }

                if (Bot.Player == null)
                {
                    Logger("Bot.Player is null.");
                    return;
                }
                #endregion ignore this

                Bot.Events.ExtensionPacketReceived += MapIsMemberLocked;
                bool hasMapNumber = map.Contains('-') && int.TryParse(map.Split('-').Last(), out int result) && result >= 1000;
                Random rnd = new();
                for (int i = 0; i < 20; i++)
                {
                    Bot.Wait.ForActionCooldown(GameActions.Transfer);
                    if (!string.IsNullOrEmpty(map) && Bot.Map.Name != map)
                    {
                        Bot.Wait.ForActionCooldown(GameActions.Transfer);
                        if (hasMapNumber)
                        {
                            if (map != null && Bot.Map.Name != map)
                                Bot.Map.Join(map, cell ?? "Enter", cell == null ? "Spawn" : pad ?? "Left");
                            Sleep();
                        }
                        else
                        {
                            if (map != null && Bot.Map.Name != map)
                                Bot.Map.Join((publicRoom && PublicDifficult) || !PrivateRooms ? map : $"{map}-{PrivateRoomNumber}", cell ?? "Enter", cell == null ? "Spawn" : pad ?? "Left");
                        }
                        Bot.Wait.ForMapLoad(strippedMap);
                        // Exponential Backoff
                        Sleep(Math.Max(1, 100 * rnd.Next((int)Math.Pow(2, i / 2.0))));
                    }

                    // Update BlackListedJumptoCells with the regex pattern
                    BlackListedJumptoCells = BlackListedJumptoCells
                        .Union(Bot.Map.Cells.Where(x => x != null
                            && Regex.IsMatch(x, @"(^cut\w*$)|(^\w*cut$)|(^cut$)|^r\d+$|^(bs\d+|ar\d+|ms\d+|apo\d+|guild)$", RegexOptions.IgnoreCase)))
                        .Distinct()
                        .ToArray();

                    // Check if map is not "oaklore" and filtering is needed
                    if (map != null && map != "oaklore" && (cell == null || cell == "Enter"))
                    {
                        // Filter cells excluding blacklisted and "Enter" cells
                        var validCells = Bot.Map.Cells
                            .Where(x => !string.IsNullOrEmpty(x) && // Ensure the cell isn't null or empty
                                        !BlackListedJumptoCells.Any(blacklisted =>
                                            x.Contains(blacklisted, StringComparison.OrdinalIgnoreCase))) // Case-insensitive match
                            .ToList();

                        // Prioritize non-"Enter" cells or fall back to "Enter"
                        cell = Bot.Map.Cells.Count(x => x.Contains("Enter", StringComparison.OrdinalIgnoreCase)) > 1
                        ? validCells.FirstOrDefault()
                        : "Enter";

                        // Log and perform the jump if a valid cell is found and it's different from the current one
                        if (cell != null && Bot.Player.Cell != cell)
                        {
                            Bot.Map.Jump(cell ?? "Enter", pad ?? "Spawn", false);
                            Sleep(1000);
                        }
                    }

                    // Check if the current map matches the desired map and proceed accordingly
                    if (Bot.Map.Name?.ToLower() == strippedMap)
                    {
                        // If SafeTimings option is enabled
                        if (Bot.Options.SafeTimings)
                        {
                            // Handle map load check and jump logic
                            if (!string.IsNullOrEmpty(map) && !Bot.Wait.ForMapLoad(map, 20))
                            {
                                if (cell != null && Bot.Player.Cell != cell)
                                    Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
                            }
                            else if (cell != null && Bot.Player.Cell != cell)
                            {
                                Bot.Map.Jump(cell, pad ?? "Spawn");
                            }

                            // Sleep and wait for the player's cell change
                            Sleep();
                            Bot.Wait.ForCellChange(cell ?? "Enter");
                        }
                        break;
                    }


                    if (i == 19)
                        Logger($"Failed to join {map}");
                }

                Bot.Events.ExtensionPacketReceived -= MapIsMemberLocked;
            }
            catch (Exception ex)
            {
                Logger($"An error occurred: {ex.Message}. StackTrace: {ex.StackTrace}");
            }

            void MapIsMemberLocked(dynamic packet)
            {
                try
                {
                    if (packet == null)
                    {
                        Logger("Packet is null.");
                        return;
                    }

                    string type = packet["params"].type;
                    dynamic data = packet["params"].dataObj;
                    if (type is not null and "str")
                    {
                        string cmd = data[0];
                        switch (cmd)
                        {
                            case "warning":
                                if (Convert.ToString(packet).Contains("is an Membership-Only Map"))
                                {
                                    Logger($" \"{map}\" requires membership to access it. Stopping the Bot.", stopBot: true);
                                    Bot.Events.ExtensionPacketReceived -= MapIsMemberLocked;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger($"An error occurred: {ex.Message}. StackTrace: {ex.StackTrace}");
                }
            }
        }
        void SimpleQuestBypass(params (int, int)[] slotValues)
        {
            try
            {
                if (Bot.Quests == null)
                {
                    Logger("Bot.Quests is null.");
                    return;
                }

                foreach ((int, int) sV in slotValues)
                    Bot.Quests.UpdateQuest(sV.Item2, sV.Item1);
                Sleep();
                tryJoin();
            }
            catch (Exception ex)
            {
                Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }
        }
        void PrivateSimpleQuestBypass(params (int, int)[] slotValues)
        {
            try
            {
                map = strippedMap + "-999999";
                SimpleQuestBypass(slotValues);
            }
            catch (Exception ex)
            {
                Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }
        }
        void GhostItemBypass(int ID, string name = "Ghost Item", int quantity = 1, bool temp = false, ItemCategory category = ItemCategory.Unknown, string? description = null, int level = 1, params (string, object)[] extraInfo)
        {
            try
            {
                if (Bot.Inventory == null)
                {
                    Logger("Bot.Inventory is null.");
                    return;
                }

                if (!CheckInventory(ID))
                    GhostItem(ID, name);
                Sleep();
                tryJoin();
            }
            catch (Exception ex)
            {
                Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }
        }
        GC.Collect();
    }

    public void CutSceneFixer(string map, string? cell, string cutsceneCell, string pad = "Left")
    {
        if (string.IsNullOrWhiteSpace(map))
        {
            Logger("Invalid map parameter. Map is required.");
            return;
        }

        if (cell != null && string.IsNullOrWhiteSpace(cell))
        {
            Logger("Invalid cell parameter. If provided, cell must not be empty or whitespace.");
            return;
        }

        Logger($"CutSceneFixer Started. Cell:\"[{cell}]\"");

        // Ensure the bot is in the correct map (either "doomvault" or "doomvaultb")
        while (!Bot.ShouldExit && (map == "doomvault" && Bot.Map.Name != "doomvault" || map == "doomvaultb" && Bot.Map.Name != "doomvaultb"))
        {
            if (Bot.Player.InCombat || Bot.Player.HasTarget)
            {
                Bot.Combat.Exit();
                Bot.Combat.CancelTarget();
                JumpWait();
            }

            // Join the correct map based on the provided map parameter
            Join(map);
        }

        // Handle different cases for cutsceneCell
        switch (cutsceneCell.ToLower())
        {
            case "initroom":
                Bot.Wait.ForCellChange("initRoom");
                break;

            case var cut when cut.StartsWith("cut") && int.TryParse(cut.AsSpan(3), out _):
                Bot.Wait.ForCellChange(cutsceneCell);
                break;

            default:
                Logger($"Unhandled case for cutsceneCell: \"{cutsceneCell}\". Yell at Tato.. nicely!");
                return;
        }

        if (Bot.Player.Cell == cutsceneCell)
        {
            Logger($"Player not in Cell: \"{cell}\", \nAttempting to fix");

            // Fix the map if needed
            if (Bot.Map.Name != map)
            {
                if (!string.IsNullOrEmpty(cell))
                {
                    Join(map, cell, pad);
                    Bot.Wait.ForCellChange(cell);
                }
                else
                {
                    Logger("Invalid cell parameter. Cell is required.");
                }
            }

            // Ensure the player is in the correct cell
            while (!Bot.ShouldExit && (Bot.Player.Cell != cell || Bot.Player.Cell == cutsceneCell))
            {
                if (!string.IsNullOrEmpty(cell) && Bot.Player.Cell != cell)
                    Bot.Map.Jump(cell, pad);
                Bot.Wait.ForCellChange(cell ?? "Enter");


                Sleep();
            }

            Logger($"{Bot.Player.Cell} Fixed.");
        }
        else
            Logger($"Fix for Cell: \"{cell}\" Not Required.");
    }

    public void JoinSWF(string map, string swfPath, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false)
    {
    retry:
        // Attempt to join the map and load SWF
        Join(map, cell, pad, ignoreCheck: ignoreCheck, publicRoom: false);
        Bot.Wait.ForMapLoad(map);
        Bot.Flash.CallGameFunction("world.loadMap", swfPath);
        Sleep(1500);
        // Wait until the player is fully loaded or exit condition is met
        while (!Bot.ShouldExit && !Bot.Player.Loaded) Sleep();

        // If the map is loaded, proceed with cell filtering and jumping
        if (Bot.Map != null)
        {
            string targetPad = cell == "Enter" ? "Spawn" : "Left";

            // Jump to the target cell if not already there
            if (Bot.Player.Cell != cell)
                Bot.Map.Jump(cell, targetPad);

            Bot.Wait.ForCellChange(cell);
        }
        else
        {
            // Retry if the map isn't loaded yet
            goto retry;
        }
    }


    /// <summary>
    /// Sends a getMapItem packet for the specified item.
    /// </summary>
    /// <param name="itemID">ID of the item</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="map">Map where the item is</param>
    public void GetMapItem(int itemID, int quant = 1, string? map = null)
    {
        // Early exit if item already present in sufficient quantity
        if (Bot.TempInv.Contains(itemID, quant))
        {
            Logger($"Map item {itemID} already acquired ({quant})");
            return;
        }

        if (map != null)
            Join(map);

        JumpWait();
        Sleep();

        List<ItemBase>? initialItems = Bot.TempInv.Items?.ToList();
        ItemBase? newItem = null;

        for (int i = 0; i < quant; i++)
        {
            Bot.Map.GetMapItem(itemID);
            Sleep(1000);

            if (newItem != null)
                continue;

            // Try to find the newly acquired item
            List<ItemBase>? newItems = Bot.TempInv.Items?.Except(initialItems ?? Enumerable.Empty<ItemBase>()).ToList();
            newItem = newItems?.FirstOrDefault(x => x.ID == itemID) ?? newItems?.FirstOrDefault();
        }

        if (quant > 1 && newItem != null)
        {
            int attempts = 0;
            while (Bot.TempInv.GetQuantity(newItem.Name) < quant &&
                   Bot.TempInv.TryGetItem(newItem.Name, out ItemBase? item) &&
                   (item?.Quantity < item?.MaxStack))
            {
                Bot.Map.GetMapItem(itemID);
                Sleep(1000);
                attempts++;

                if (attempts > quant + 10 || Bot.TempInv.Contains(newItem.Name, quant))
                    break;
            }
        }

        Logger($"Map item {itemID} ({quant}) acquired");
    }


    /// <summary>
    /// This method is used to move between PvP rooms
    /// </summary>
    /// <param name="mtcid">Last number of the mtcid packet</param>
    /// <param name="cell">Cell you want to be</param>
    /// <param name="moveX">X position of the door</param>
    /// <param name="moveY">Y position of the door</param>
    public void PvPMove(int mtcid, string cell, int moveX = 0, int moveY = 0)
    {
    retry:
        // Different maps = differnt walk speeds for pvp appearenty
        Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%{moveX}%{moveY}%{(Bot.Map.Name == "dagepvp" ? "10%" : "8%")}");
        Sleep(2500);
        Bot.Send.Packet($"%xt%zm%mtcid%{Bot.Map.RoomID}%{mtcid}%");
        Sleep(2500);

        if (Bot.Player.Cell != null && Bot.Player.Cell != cell)
        {
            Sleep(1500);
            goto retry;
        }
    }

    /// <summary>
    /// Checks if the room you're in is a public room or not
    /// </summary>
    /// <returns>If room number is less than 1000</returns>
    public bool inPublicRoom()
    {
        Bot.Wait.ForMapLoad(Bot.Map.Name);
        if (!int.TryParse(Bot.Map.FullName.Split('-').Last(), out int nr))
            nr = 1;
        return nr < 1000;
    }
    /// <summary>
    /// Adjusts the farming quantity by subtracting the available item count, up to a maximum stack limit.
    /// </summary>
    /// <param name="quant">Reference to the target quantity to adjust.</param>
    /// <param name="item">The name of the item to check in the inventory.</param>
    /// <param name="MaxStack">The maximum stack size to subtract.</param>
    /// <returns>The updated quantity after adjustment.</returns>
    public int FarmQuantity(ref int quant, string item, int MaxStack)
    {
        if (Bot.Inventory.TryGetItem(item, out InventoryItem? Item) && Item != null)
        {
            quant -= Math.Min(Item.Quantity, MaxStack);
        }
        return quant;
    }

    public void PVPKilling(int MonsterMapID = 0)
    {
        if (Bot.Map.Name == "legionpvp")
        {
            Join("dagepvp-999999", "Enter0", "Spawn");
            Bot.Wait.ForMapLoad("dagepvp");
            return;
        }

        //attempt to set monster state
        foreach (Monster target in Bot.Monsters.MapMonsters
        .Where(x => x != null && x.Cell == Bot.Player.Cell))
        {

            Logger($"setting mob State for {target.MapID}");
            Bot.Combat.Attack(target);
            Bot.Combat.CancelAutoAttack();
            Bot.Combat.CancelTarget();
            Bot.Wait.ForTrue(() => target.State > 0 || Bot.Monsters.CurrentAvailableMonsters.Any(x => x.MapID == 27), 20);

            Bot.Combat.StopAttacking = false;
            if (Bot.Map.Name == "legionpvp")
            {
                Join("dagepvp-999999", "Enter0", "Spawn");
                Bot.Wait.ForMapLoad("dagepvp");
                return;
            }
        }

        if (!Bot.Monsters.MapMonsters
        .Any(x => x != null && x.Cell == Bot.Player.Cell && x.State > 0))
        {
            Logger("All mobs in room where killed during State setting process onto the next room");
        }

        //with state set, we can identifiy if they're dead or not
        foreach (Monster targetMonster in Bot.Monsters.MapMonsters
        .Where(x => x != null && x.Cell == Bot.Player.Cell && x.State > 0))
        {

            Logger($"Killing {targetMonster}");
            while (!Bot.ShouldExit && Bot.Monsters.MapMonsters.Any(x => x.State > 0))
            {
                while (!Bot.ShouldExit && (!Bot.Player.Alive || Bot.Map.Name == "legionpvp"))
                {
                    Sleep();
                    if (Bot.Map.Name == "legionpvp")
                    {
                        Join("dagepvp-999999", "Enter0", "Spawn");
                        Bot.Wait.ForMapLoad("dagepvp");
                        return;
                    }
                    return;
                }

                Monster? availableMonster = Bot.Monsters.CurrentAvailableMonsters.FirstOrDefault(x => x.State != 0);
                if (availableMonster != null)
                {
                    Bot.Combat.Attack(availableMonster.MapID);
                }
                else
                {
                    Logger("No available monsters to attack");
                    break;
                }
                Sleep();
            }
        }
    }


    /// <summary>
    /// Resets a quest by ensuring its loading, abandoning if active, and returning whether it was accepted.
    /// </summary>
    /// <param name="QuestID">The ID of the quest to reset.</param>
    /// <returns>True if the quest was accepted, false otherwise.</returns>
    public bool ResetQuest(int QuestID = 0000)
    {
        /*
        Dark makai and their Sigils / Runes are tricky... use this with the appropriate QuestID below:
            - Swindles Return: 7551
            - Diamond Exchange: 869
            - add more as used.
        */

        // Ensure the quest is loaded
        Quest? quest = Bot.Quests.EnsureLoad(QuestID);

        // Check if the quest is active
        if (Bot.Quests.Active.Contains(quest!))
        {
            // Abandon the quest if it's active
            AbandonQuest(QuestID);
        }
        else
        {
            // Ensure and wait for quest acceptance
            EnsureAccept(QuestID);
            Bot.Wait.ForTrue(() => Bot.Quests.EnsureAccept(QuestID), 20);

            // Abandon the quest after acceptance
            AbandonQuest(QuestID);
        }
        EnsureAccept(QuestID);

        // Return whether the quest was accepted
        return Bot.Quests.EnsureAccept(QuestID);
    }



    /// <summary>
    /// Checks if the map is available for joining or it is seasonal and not yet released
    /// </summary>
    public bool isSeasonalMapActive(string map, bool log = true)
    {
        map = map.ToLower().Replace(" ", "");
        if (Bot.Map.Name != null && Bot.Map.Name.ToLower() == map)
            return true;

        JumpWait();
        Bot.Events.ExtensionPacketReceived += MapIsNotAvailableListener;
        bool seasonalMessageProc = false;

        for (int i = 0; i < 20; i++)
        {
            if (Bot.Map.Name != null && Bot.Map.Name != map)
                Bot.Map.Join(!PrivateRooms ? map : $"{map}-{PrivateRoomNumber}");
            Bot.Wait.ForMapLoad(map);

            string? currentMap = Bot.Map.Name;
            if (!string.IsNullOrEmpty(currentMap) && currentMap.ToLower() == map)
                break;

            if (seasonalMessageProc)
            {
                return false;
            }

            if (i == 19)
                Logger($"Failed to join {map}");
        }

        Bot.Events.ExtensionPacketReceived -= MapIsNotAvailableListener;

        return Bot.Map.Name != null && Bot.Map.Name.ToLower() == map;

        void MapIsNotAvailableListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "str")
            {
                string cmd = data[0];
                switch (cmd)
                {
                    case "warning":
                        string b = Convert.ToString(packet);
                        if (b.Contains("is not available."))
                        {
                            if (log)
                                Logger($" \"{map}\" is currently seasonal map. Check Wiki.");
                            seasonalMessageProc = true;
                            Bot.Events.ExtensionPacketReceived -= MapIsNotAvailableListener;
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Automatic Class Selection for certain bosses.
    /// </summary>
    /// <param name="additionalClass">Additional class to swap into for said boss</param>
    public void BossClass(string? additionalClass = null)
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            JumpWait();

        string[] classesToCheck = new[] { "TimeKeeper", "Verus DoomKnight", "Void Highlord", "Void HighLord (IoDA)", "Yami no Ronin", "ArchPaladin" };
        if (!string.IsNullOrEmpty(additionalClass) && CheckInventory(additionalClass))
        {
            Unbank(additionalClass);
            Equip(additionalClass);
            Bot.Wait.ForItemEquip(additionalClass);
            Bot.Wait.ForActionCooldown(GameActions.EquipItem);
            Logger($"Using {additionalClass}");
        }
        else
        {
            foreach (string Class in classesToCheck)
            {
                if (!CheckInventory(Class, toInv: Class != "TimeKeeper" || SoloClass == Class))
                    continue;


                Unbank(Class);
                Equip(Class);

                switch (Class)
                {
                    case "TimeKeeper":
                        if (SoloClass != Class)
                            continue;
                        else
                            Bot.Skills.StartAdvanced(Class, false, ClassUseMode.Base);
                        break;

                    case "ArchPaladin":
                        Bot.Skills.StartAdvanced(Class, false, ClassUseMode.Base);
                        break;

                    case "Void Highlord":
                    case "Void HighLord (IoDA)":
                    case "Verus DoomKnight":
                        Bot.Skills.StartAdvanced(Class, false, ClassUseMode.Def);
                        break;

                    case "Yami no Ronin":
                        Bot.Skills.StartAdvanced(Class, false, ClassUseMode.Solo);
                        break;

                    case "Chaos Avenger":
                        Bot.Skills.StartAdvanced(Class, false, ClassUseMode.Base);
                        break;

                    default:
                        Unbank(SoloClass);
                        Equip(SoloClass);
                        break;
                }

                Bot.Wait.ForActionCooldown(GameActions.EquipItem);
                Bot.Wait.ForItemEquip(CheckInventory(Class) ? Class : SoloClass);
                Logger($"Using {(CheckInventory(Class) ? Class : SoloClass)}");
                break;
            }

        }

    }

    /// <summary>
    /// Switches between specified classes and equips necessary items based on the provided additional class.
    /// </summary>
    /// <param name="additionalClass">Optional additional class to switch to.</param>
    public void DodgeClass(string? additionalClass = null)
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            JumpWait();

        // Check if CurrentClass is not null
        string currentClassName = Bot.Player.CurrentClass?.Name ?? string.Empty;

        // Create the list of classes to check
        List<string> classesToCheck = new() { "Yami no Ronin", "Chrono Assassin", "Spy", "Rogue" };
        if (!string.IsNullOrEmpty(currentClassName))
        {
            classesToCheck.Add(currentClassName);
        }
        if (!string.IsNullOrEmpty(additionalClass) && CheckInventory(additionalClass))
        {
            Unbank(additionalClass);
            Equip(additionalClass);
            Bot.Wait.ForItemEquip(additionalClass);
            Logger($"Using {additionalClass}");
        }
        else
        {
            foreach (string Class in classesToCheck)
            {
                switch (Class)
                {
                    case "Yami no Ronin":
                        Bot.Skills.StartAdvanced(Class, true, ClassUseMode.Solo);
                        break;

                    case "Chrono Assassin":
                        Bot.Skills.StartAdvanced(Class, true, ClassUseMode.Base);
                        break;

                    default:
                        Bot.Skills.StartAdvanced(Class, true, ClassUseMode.Base);
                        break;
                }

                if (!CheckInventory(Class))
                    Logger($"{Class} Not Found, skipping");
                else
                {
                    Bot.Wait.ForItemEquip(CheckInventory(Class) ? Class : SoloClass);
                    Logger($"Using {Bot.Player.CurrentClass?.Name}");
                    break;
                }
            }
        }


        if (!string.IsNullOrEmpty(additionalClass) && CheckInventory(additionalClass))
        {
            Unbank(additionalClass);
            Equip(additionalClass);
            Bot.Wait.ForItemEquip(additionalClass);
            Logger($"Using {additionalClass}");
        }
    }

    /// <summary>
    /// Performs actions to obtain a specific item in Dark Makai's map areas.
    /// </summary>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quantity">The quantity of the item to obtain.</param>
    /// <param name="isTemp">Specifies whether the item is temporary.</param>
    public void DarkMakaiItem(string? item = null, int quantity = 1, bool isTemp = true)
    {
        if (string.IsNullOrEmpty(item) || (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity)))
            return;

        var maps = new[] { ("tercessuinotlim", "m1"), (IsMember ? "Nulgath" : "evilmarsh", "Field1") };
        var randomMapIndex = new Random().Next(0, maps.Length);
        var selectedMap = maps[randomMapIndex];

        Join(selectedMap.Item1, selectedMap.Item2, "Left");

        EquipClass(ClassType.Farm);
        while (!Bot.ShouldExit && isTemp ? !Bot.TempInv.Contains(item!, quantity) : !Bot.Inventory.Contains(item, quantity))
        {
            if (Bot.Player.Cell != selectedMap.Item2)
                Jump(selectedMap.Item2);

            Bot.Combat.Attack("Dark Makai");
            Sleep();
        }
    }

    public void AuraHandling(string? targetAuraName)
    {
        foreach (Aura A in Bot.Target.Auras.Concat(Bot.Self.Auras))
        {
            if (targetAuraName == null)
                continue;

            switch (A.Name)
            {
                case "Oxidize":
                    while (!Bot.ShouldExit && Bot.Player.Alive && !Bot.Self.HasActiveAura("Vigil"))
                    {
                        UsePotion();
                        Sleep();

                        // Check if targetAura is not null before accessing its SecondsRemaining() method
                        // Assuming `targetAura` is the aura you're referring to
                        if (Bot.Self.HasActiveAura("Vigil"))
                        {
                            Logger($"\"{A.Name}\" Active!");
                            break;
                        }
                    }
                    break;

                case "Endless Blizzard":
                    while (!Bot.ShouldExit && Bot.Player.Alive && !Bot.Self.HasActiveAura("Bananach's Last Will"))
                    {
                        UsePotion();
                        Sleep();

                        // Check if targetAura is not null before accessing its SecondsRemaining() method
                        // Assuming `targetAura` is the aura you're referring to
                        if (Bot.Self.HasActiveAura("Bananach's Last Will"))
                        {
                            Logger($"\"{A.Name}\" Active!");
                            break;
                        }
                    }
                    break;

                default:
                    break;

            }
        }
    }

    public void UsePotion()
    {
        var skill = Bot.Flash.GetArrayObject<dynamic>("world.actions.active", 5);
        if (!Bot.Player.Alive || skill == null) return;
        Bot.Flash.CallGameFunction("world.testAction", JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(skill)));
    }

    public void ShutdownSkua()
    {
        Process currentProcess = Process.GetCurrentProcess();
        if (currentProcess.ProcessName == "Skua")
        {
            // Releases lingering resources, reducing memory usage before termination.
            GC.Collect();
            //terminate the process
            currentProcess.Kill();
        }
    }

    #endregion

    #region AutoReport

    public void AutoReport(AutoReportType type, Exception? e = null, LockedQuestData? lqd = null)
    {
        if (e == null && lqd == null)
            return;

        string path = loadedBot;
        string idPath = Path.Combine(ClientFileSources.SkuaDIR, "AutoReportIdentity.txt");
        if (File.Exists(idPath))
        {
            string identity = File.ReadAllText(idPath);
            if (IdentityControl(ref identity))
            {
                Dictionary<string, string> bodyValues = new()
                {
                    {"entry.2118425091", "Bug Report"},
                    {"entry.290078150", path},
                    {"entry.1700030786", identity},
                };

                switch (type)
                {
                    case AutoReportType.ScriptCrash:
                        if (e == null)
                            return;

                        List<string> ScriptLogs = Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script);

                        bodyValues.Add("entry.1803231651", "It stopped at the wrong time (crash)");
                        bodyValues.Add("entry.1954840906", ScriptLogs.Skip(ScriptLogs.Count - 6).Join("\n"));
                        bodyValues.Add("entry.285894207", e.ToString());
                        break;

                    case AutoReportType.LockedQuest:
                        if (lqd == null)
                            return;

                        bodyValues.Add("entry.1803231651", "I got a popup saying a quest was not unlocked");
                        bodyValues.Add("entry.1918245848", $"{lqd.ID}");
                        bodyValues.Add("entry.1809007115", $"{lqd.ExpectedValue}/{lqd.Slot}");
                        bodyValues.Add("entry.493943632", $"{lqd.CurrentValue}/{lqd.Slot}");
                        bodyValues.Add("entry.148016785", lqd.Name);
                        break;
                }

                FormUrlEncodedContent content = new(bodyValues);
                WebClient.PostAsync(
                                "https://docs.google.com/forms/d/e/" +
                                "1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg" +
                                "/formResponse",
                                content);
            }
            else ManualReport();
        }
        else ManualReport();
        Bot.Stop(type == AutoReportType.LockedQuest);

        void ManualReport()
        {
            switch (type)
            {
                case AutoReportType.ScriptCrash:
                    if (e == null)
                        break;

                    string scriptCrashMessage = "A crash has been detected\n" + e.ToString();
                    Logger(scriptCrashMessage);
                    if (Bot.ShowMessageBox(scriptCrashMessage + "\n\nPress Yes to be be brought to the report form", "Quest not unlocked", true) == true)
                    {
                        List<string> ScriptLogs = Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script);

                        Process.Start("explorer", $"\"https://docs.google.com/forms/d/e/1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg/viewform?usp=pp_url&" +
                                                     "entry.2118425091=Bug+Report&" +
                                                    $"entry.290078150={path}&" +
                                                     "entry.1803231651=It+stopped+at+the+wrong+time+(crash)&" +
                                                    $"entry.1954840906={ScriptLogs.Skip(ScriptLogs.Count - 6).Join("\n")}&" +
                                                    $"entry.285894207={e}\"");
                    }
                    break;

                case AutoReportType.LockedQuest:
                    if (lqd == null)
                        break;

                    string lockedQuestMessage = $"Quest \"{lqd.Name}\" [{lqd.ID}] is not unlocked.\n" +
                                                $"Expected value = [{lqd.ExpectedValue}/{lqd.Slot}], but received = [{lqd.CurrentValue}/{lqd.Slot}]\n" +
                                                 "Please fill in the Skua Scripts Form to report this.\n" +
                                                 "Do you wish to be brought to the form?";
                    Logger(lockedQuestMessage);
                    if (Bot.ShowMessageBox(lockedQuestMessage, "Quest not unlocked", true) == true)
                    {
                        Process.Start("explorer", $"\"https://docs.google.com/forms/d/e/1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg/viewform?usp=pp_url&" +
                                                     "entry.2118425091=Bug+Report&" +
                                                    $"entry.290078150={path}&" +
                                                     "entry.1803231651=I+got+a+popup+saying+a+quest+was+not+unlocked&" +
                                                    $"entry.1918245848={lqd.ID}&" +
                                                    $"entry.1809007115={lqd.ExpectedValue}/{lqd.Slot}&" +
                                                    $"entry.493943632={lqd.CurrentValue}/{lqd.Slot}&" +
                                                    $"entry.148016785={lqd.Name}\"");
                    }
                    break;
            }
        }
    }

    public bool IdentityControl(ref string identity)
    {
        identity = identity.Trim().Replace("​", ""); //There is a 0-width charactr in the first ""
        while (identity.Contains("  "))
            identity = identity.Replace("  ", " ");

        if (identity.Length < 7)
        {
            FaultyInput("It's too short");
            return false;
        }
        if (identity.Length > 37)
        {
            FaultyInput("It's too long");
            return false;
        }

        if (!identity.Contains('#'))
        {
            FaultyInput("It doesn't contain a '#'");
            return false;
        }
        if (identity[^5..^4] != "#")
        {
            FaultyInput("It doesn't have a '#' in the right location");
            return false;
        }

        if (!int.TryParse(identity[^4..], out int _numbers))
        {
            FaultyInput("It's missing the 4 digits at the end");
            return false;
        }

        foreach (string s in new string[] { "@", "#", ":", "```", "discord" })
        {
            if (!identity[..^5].Contains(s))
                continue;

            if (s == "#")
                FaultyInput("There can only be one '#', which is near the end");
            else FaultyInput($"It's not able to contain the character '{s}'");
            return false;
        }

        if (identity[..^5].ToLower() == "everyone" || identity[..^5].ToLower() == "here")
        {
            FaultyInput($"It cannot be {identity[..^5]}");
            return false;
        }

        return true;

        void FaultyInput(string text) => Bot.ShowMessageBox($"Invalid Discord username detected:\n{text}!", "Invalid AutoReport Identity");
    }

    public class LockedQuestData
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int ExpectedValue { get; set; }
        public int CurrentValue { get; set; }
        public int Slot { get; set; }

        public LockedQuestData(Quest q, int currentValue)
        {
            ID = q.ID;
            Name = q.Name;

            ExpectedValue = q.Value - 1;
            CurrentValue = currentValue;
            Slot = q.Slot;
        }
    }

    #endregion

    #region Flash-Call Assistance

    public T? GetItemProperty<T>(InventoryItem item, string prop)
    {
        if (Bot.Inventory.Contains(item.ID))
            return Bot.Flash.GetGameObject<T>($"world.invTree.{item.ID}.{prop}");
        else if (Bot.Bank.Contains(item.ID)) // Also covers banked house items
            return Bot.Flash.GetGameObject<List<dynamic>>("world.bankinfo.items")?.Find(d => d.ItemID == item.ID)?[prop];
        else
            return Bot.Flash.GetGameObject<List<dynamic>>("world.myAvatar.houseitems")?.Find(d => d.ItemID == item.ID)?[prop];
    }
    public T? GetItemProperty<T>(ShopItem item, string prop)
        => Bot.Flash.GetGameObject<List<dynamic>>("world.shopinfo.items")?.Find(d => d.ItemID == item.ID)?[prop];

    #endregion

    #region Using Local Files
    public static string ButlerLogDir = Path.Combine(ClientFileSources.SkuaOptionsDIR, "Butler");
    private string ButlerLogPath() => Path.Combine(ButlerLogDir, Username().ToLower() + ".txt");
    public bool ButlerOnMe()
    {
        if (!Directory.Exists(ButlerLogDir))
            return false;

        var files = Directory.GetFiles(ButlerLogDir);
        return files.Length > 0 && files.Any(x => x.Contains("~!") && (x.Split("~!").Last() == (Username().ToLower() + ".txt")));
    }

    public void DeleteCompiledScript()
    {
        string? scriptDir = ClientFileSources.SkuaScriptsDIR;
        if (string.IsNullOrEmpty(scriptDir))
            return;

        string filePath = Path.Combine(scriptDir, "z_CompiledScript.cs");
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            return; // Skip deletion if file doesn't exist or is empty

        try
        {
            File.Delete(filePath);
        }
        catch (IOException)
        {
            // Retry deleting the file if it's locked (e.g., OneDrive syncing)
            const int retryCount = 3;
            for (int i = 0; i < retryCount; i++)
            {
                Bot.Sleep(1000); // Wait before retrying
                try
                {
                    File.Delete(filePath);
                    return;
                }
                catch (IOException)
                {
                    if (i == retryCount - 1)
                        Bot.Log($"Error deleting '{filePath}'. Please pause or quit OneDrive from the bottom right, and try again.");
                }
            }
        }
    }

    public void WriteFile(string path, IEnumerable<string> content)
    {
        try
        {
            File.WriteAllLines(path, content);
        }
        catch (Exception e)
        {
            WriteFail(path, e);
        }
    }
    public void WriteFile(string path, string[] content)
    {
        try
        {
            File.WriteAllLines(path, content);
        }
        catch (Exception e)
        {
            WriteFail(path, e);
        }
    }
    public void WriteFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
        }
        catch (Exception e)
        {
            WriteFail(path, e);
        }
    }
    private void WriteFail(string path, Exception e) => Logger($"Skua just tried to write to \"{path}\" but got an exception:\n{e}\n\nPlease restart Skua in Admin-Mode just this once.", "Failed at writing file", true, true);

    private bool ReadMe()
    {
        string readMePath = Path.Combine(ClientFileSources.SkuaDIR, "ReadMeV1.txt");
        if (File.Exists(readMePath))
            return true;

        // Popup
        var result = Bot.ShowMessageBox(
            "Welcome to Skua's Master Bots!\n" +
            "These bots are a tad different from what you might be used to with Grimoire or other botting clients.\n\n" +
            "Its highly recommended to read the ReadMe.txt file if this is your first time running one of our bots, or if you just started.\n" +
            "There are plenty of things that are useful to know there, which arent immediately obvious.\n\n" +
            "This messagebox will not appear again after you close it.\n" +
            $"You will still be able to read the file later by going to [{readMePath}]\n" +
            "If you do see it again at a later moment, there might have just been a update to the ReadMe, in which case you can ignore this message.\n\n" +
            "Click OK to open the ReadMe.txt",

            "READ ME", "OK");

        // Creating ReadMe.txt
        string[] ReadMe =
        {
            "Welcome and thank you for using Skua's Master Bots!",
            "",
            "=== Basic Information ===",
                "These bots are a tad different from what you might be used to with Grimoire or other botting clients.",
                "All our bots are \"Master Bots\" and thus will do everything you might need it to do in order to farm the item of your choice.",
                "This includes but is not limited to:",
                "· Finishing questlines to unlock farms, maps or get a specific items.",
                "· Using bypasses so you dont have to do questlines in order to continue farming.",
                "· Do other farms that you might need to do in order to farm the item of your choice (I.E. Get NSoD as well when farming for HBSoD).",
                ". Farm the Gold, Experience, or Levels required for a certain item, or quest.",
                "",
                "== Skills ==",
                    "We also have a big file that contains 95% of all classes with one or multiple skill combinations for different scenarios.",
                    "So you'll know that your class will use a optimized combo without you having to set the skills yourself.",
                    "These combos are ofcourse always up for debate and we are happy to change them based off of community input.",
                    "If you wish to play with these for yourself, the easiest way to do so is to use the \"Advanced Skills\" window, which can be found in the top row of Skua and then Skills.",
                    "",
                "== File Naming ==",
                    "Whilst using our bots, you might notice that there are files that start with the word \"Core\", these files are storage for methods that we use in our bots.",
                    "These bots are not meant to be run and wont do anything usefull for you. If you do, expect a pop-up that tells you the exact same thing.",
                    "Another file naming convention is files that start with a \"0\" (zero), these files are usually inside a folder.",
                    "These files can be run and will usually do everything in the folder for you, as a sort of combo bot. Like farming everything for VHL and buying + leveling it too.",
                    "",
                "== Bugs and Bot Requests ==",
                    "As much as we try, bugs pop up from time to time.",
                    "If you find one, please report it to us via the form which can be found near the bottom of the Scripts menu.",
                    "This same form will also be used to request new features or bots.",
                    "",
                "== GitHub Prompt ==",
                    "You might have noticed how Skua asks you to authorize with a GitHub account when you first run Skua.",
                    "This is so that Skua can update the bots from our GitHub repository.",
                    "Without this you are bound to a 50 requests p/h limiter that is shared with everyone else who didn't authortize.",
                    "Considering that you already send 3 requests on startup, you can see how this can be reached quickly.",
                    "Therefore it's highly recommended to do the authorization, as you will then have your own limiter instead of a shared one.",
                    "",
                    "",
            "=== Plugins ===",
                "== CoreBots Options ==",
                    "Now, this plugin is where you customize a lot of the things that happen for all the bots. It's highly recommended to open this one up and set some options.",
                    "I highly recommend setting all your preffered options in the Generic tab, as this houses the important ones.",
                    "You can ofcourse also check our the other options and set them to what you want too.",
                    "It's recommended to stay in private rooms, as public rooms have a higher chance of getting you banned.",
                    "It should also be noted that Skua version 4.1.3, comes with a outdated version of the \"CoreBots Options\" plugin.",
                    "You can find the latest here https://github.com/LordExelot/Skua-CBO/releases/tag/v1",
                        "Within the discord this plugin is often reffered to as CBO.",
                    "",
                "== Wait Timeout Override ==",
                    "This is a plugin that allows you to override some default data for Skua, it's used to modify how long Skua waits before it considers a task to be failed.",
                    "You don't have to touch these values in most cases, it's mostly used for debugging.",
                    "",
                    "",
                "=== The End ===",
                    "Thanks for reading, I hope it wasn't too much of a bore!",
                    "",
                "== Contact ==",
                    "If you wish to contact us, you can find us on our discord server: " + DiscordLink,
                    "",
                "== Credits ==",
                        "· Breno_Henrike\t- Skua Creator. Breno also build the framework that these Master Bots now use.",
                        "· Lord Exelot\t- [Previous] Lead Developer/Head of the (then) Skua Master Bot team. Expanded the framework and spearheaded the development of the Master Bots.",
                        "· Tato\t\t\t- [Current] Script Head & \"Maintenance\" man of the \"Master Bots\", along with being a major contributor.",
                        "· Delfina\t\t\t- Kicked off project due to arrogance and attitude.",
                        "· Vladimir\t\t- Major contributor to the Master Bots and bug fixes.",
                        "· Bogalj\t\t- [Current] 2nd in command, and Maintenance for the Master Bots, along with being a major contributor.",
                        "· Shokry\t\t- Major contributor to the Master Bots.",
                        "· Shaun.\t\t- Major contributor to the Master Bots.",
                        "· Rodit\t\t\t- Creator of RBot.",
                        "· Purple\t\t- Contributor to RBot & Skua [1.2.4]",
                    "Thanks to you, for reading this far down. ReadMe's are usually a drag so I tried to keep it to the point.",
                    "And thanks to everyone who has put time and effort RBot/Skua and the Master Bots! ~ Exelot",
        };
        WriteFile(readMePath, ReadMe);

        // Opening ReadMe.txt
        if (result.Text == "OK")
            Process.Start("explorer", readMePath);

        if (Bot.ShowMessageBox($"If you have discord, consider joining our Discord server ({DiscordLink}).\nHere you can talk to other botters, ask questions, and get notified on new bots!\nDo you wish to join?", "Join our Discord", true) == true)
            Process.Start("explorer", DiscordLink);
        return false;
    }

    private void CollectData(bool onStartup)
    {
        Task.Run(() =>
        {
            string UserID = "null";
            bool genericData = false;
            bool scriptNameData = false;
            bool stopTimeData = false;
            FileSetup();

            if (!genericData || UserID == "null")
                return;

            // If on stop and it's not allowed, return
            if (!onStartup && !stopTimeData)
                return;

            // Build the Field Ids and Answers dictionary object
            var bodyValues = new Dictionary<string, string>
            {
                {"entry.1700030786", UserID},
                {"entry.942504290", onStartup ? "Start" : "Stop"},
            };

            // If allowed, send scriptNameData
            if (scriptNameData)
            {
                string botPath = Bot.Manager.LoadedScript.Split("Scripts").Last().Replace('/', '\\')[1..];

                if (botPath.StartsWith("Nulgath\\"))
                    botPath = botPath.Replace("Nulgath\\", "Nation\\");

                string[] allowedPathStarters =
                {
                    "Army",
                    "Chaos",
                    "Dailies",
                    "Darkon",
                    "Enhancement",
                    "Evil",
                    "Farm",
                    "Good",
                    "Hollowborn",
                    "Legion",
                    "Nation",
                    "Other",
                    "Prototypes",
                    "Seasonal",
                    "Story",
                    "Templates",
                    "Tools",
                    "WIP"
                };

                if (!allowedPathStarters.Any(x => botPath.StartsWith(x)))
                    botPath = "CustomPath\\" + botPath.Split("\\").Last();

                bodyValues.Add("entry.1597948191", botPath);
            }

            // If allowed, send scriptInstanceData
            if (stopTimeData)
            {
                if (ScriptInstanceID == 0)
                    ScriptInstanceID = Bot.Random.Next(1, int.MaxValue);

                bodyValues.Add("entry.1361306892", ScriptInstanceID.ToString());
            }

            // Encode object to application/x-www-form-urlencoded MIME type
            var content = new FormUrlEncodedContent(bodyValues);

            // Post the request
            // https://docs.google.com/forms/u/0/d/e/1FAIpQLSdB0U9QsYacXTYItiN0Ovvf4aV1md8t_SiK7VbT49QPcecEtA/formResponse
            WebClient.PostAsync(
                "https://docs.google.com/forms/d/e/" +
                "1FAIpQLSdB0U9QsYacXTYItiN0Ovvf4aV1md8t_SiK7VbT49QPcecEtA" +
                "/formResponse",
                content);

            void FileSetup()
            {
                string path = Path.Combine(ClientFileSources.SkuaDIR, "DataCollectionSettings.txt");
                if (!File.Exists(path))
                {
                    Skua.Core.Models.DialogResult consent = Bot.ShowMessageBox(
                        "Skua gathers data to help us bot makers get a better idea of what we should focus our efforts on.\n\n" +
                        "The following information will be observed and collected:\n" +
                        "· An anonymous user ID, which is generated for you by Skua, to help us estimate the active user count.\n" +
                        "· How long it takes to start a script.\n" +
                        "· What scripts are used and how often.\n" +
                        "· How long it takes to stop a script.\n" +
                        "· A Script Instance ID, to help us match start- and stoptime.\n\n" +
                        "However, we require your consent for the same. " +
                        "You can select what information the developers are allowed to collect from your instance here:\n\n" +
                        "Select \"Full\" to give full consent to the developers collecting all the aforementioned information.\n" +
                        "Select \"Partial\" if you would like to choose what information you are comfortable sharing with the developers.\n" +
                        "Select \"None\" if you would prefer that none of your data is collected.",

                        "Data Collection",
                        "Full", "Partial", "None"
                    );
                    if (consent.Text == "Full")
                    {
                        genericData = true;
                        scriptNameData = true;
                        stopTimeData = true;
                    }
                    else if (consent.Text is "Cancel" or "None")
                    {
                        genericData = false;
                        scriptNameData = false;
                        stopTimeData = false;
                    }
                    else if (consent.Text == "Partial")
                    {
                        Skua.Core.Models.DialogResult nonOptional = Bot.ShowMessageBox(
                            "The following two points are not optional:\n" +
                            "· An anon userID we generate which will allows us to know our active user count.\n" +
                            "· Start time of scripts.\n\n" +
                            "If you accept this, select \"Yes\".\n" +
                            "If you dont accept this, select \"No\", and we will not gather data whatsoever.",

                            "Non-Optional Data",
                            "Yes", "No"
                        );

                        if (nonOptional.Text == "No")
                        {
                            genericData = false;
                            scriptNameData = false;
                            stopTimeData = false;
                        }
                        else if (nonOptional.Text == "Yes")
                        {
                            Skua.Core.Models.DialogResult scriptName = Bot.ShowMessageBox(
                                "Do you give consent to send us the following data-point:\n" +
                                "· What script is being run.\n\n" +
                                "This allows us to know what scripts are populair",

                                "Script Name",
                                "Yes", "No"
                            );

                            Skua.Core.Models.DialogResult stopTime = Bot.ShowMessageBox(
                                "Do you give consent to send us the following data-points:\n" +
                                "· Stop time of scripts, this would be paired with the point below" +
                                "· Script Instance ID, a random number that allows us to match start- and stoptime.\n\n" +
                                "Allowing us to have this data means we'll know how long a script has been running.",

                                "Stop Time & Script Instance ID",
                                "Yes", "No"
                            );

                            genericData = true;
                            scriptNameData = scriptName.Text == "Yes";
                            stopTimeData = stopTime.Text == "Yes";
                        }
                    }

                    if (genericData)
                    {
                        UserID = Bot.Random.Next(100000001, int.MaxValue).ToString();
                    }

                    string[] fileContent =
                    {
                    $"UserID: {UserID}",
                    $"genericDataConsent: {genericData}",
                    $"scriptNameConsent: {scriptNameData}",
                    $"stopTimeConsent: {stopTimeData}"
                };

                    WriteFile(path, fileContent);

                    Bot.ShowMessageBox(
                        "If you wish to change these settings, you can easily modify them in the following file:\n" +
                        $"[{path}]",

                        "File Location"
                    );
                }
                else
                {
                    string[] savedSettings = File.ReadAllLines(path);

                    UserID = ConsentString("UserID");
                    genericData = ConsentBool("genericDataConsent");
                    scriptNameData = ConsentBool("scriptNameConsent");
                    stopTimeData = ConsentBool("stopTimeConsent");

                    string ConsentString(string input)
                        => (savedSettings.FirstOrDefault(x => x.StartsWith(input)) ?? $"{input}: ").Split(": ").Last();
                    bool ConsentBool(string input)
                        => ConsentString(input) == "True";
                }
            }

        });
    }
    private int ScriptInstanceID = 0;

    public void ReadCBO()
    {
        if (!CBO_Active())
            return;

        CBOList = File.ReadAllLines(CBO_Path()).ToList();

        //Generic
        if (CBOBool("PrivateRooms", out bool _PrivateRooms))
            PrivateRooms = _PrivateRooms;
        if (CBOInt("PrivateRoomNr", out int _PrivateRoomNumber))
            PrivateRoomNumber = _PrivateRoomNumber;
        if (CBOBool("PublicDifficult", out bool _PublicDifficult))
            PublicDifficult = _PublicDifficult;
        if (CBOBool("BankMiscAC", out bool _BankMiscAC))
            BankMiscAC = _BankMiscAC;
        if (CBOBool("BankUnenhancedACGear", out bool _BankUnenhGear))
            BankUnenhancedACGear = _BankUnenhGear;
        if (CBOBool("LoggerInChat", out bool _LoggerInChat))
            LoggerInChat = _LoggerInChat;

        if (CBOString("StopLocationSelect", out string _StopLocationSelect))
            CustomStopLocation = _StopLocationSelect;

        if (CBOString("SoloClassSelect", out string _SoloClassSelect))
            SoloClass = string.IsNullOrEmpty(_SoloClassSelect) ? "Generic" : _SoloClassSelect;
        if (CBOBool("SoloEquipCheck", out bool _SoloGearOn))
            SoloGearOn = _SoloGearOn;
        if (CBOString("SoloModeSelect", out string _SoloModeSelect))
            SoloUseMode = (ClassUseMode)Enum.Parse(typeof(ClassUseMode), string.IsNullOrEmpty(_SoloModeSelect) ? "Base" : _SoloModeSelect);

        if (CBOString("FarmClassSelect", out string _FarmClassSelect))
            FarmClass = string.IsNullOrEmpty(_FarmClassSelect) ? "Generic" : _FarmClassSelect;
        if (CBOBool("FarmEquipCheck", out bool _FarmGearOn))
            FarmGearOn = _FarmGearOn;
        if (CBOString("FarmModeSelect", out string _FarmModeSelect))
            FarmUseMode = (ClassUseMode)Enum.Parse(typeof(ClassUseMode), string.IsNullOrEmpty(_FarmModeSelect) ? "Base" : _FarmModeSelect);

        //Advanced
        if (CBOBool("MessageBoxCheck", out bool _ForceOffMessageboxes))
            ForceOffMessageboxes = _ForceOffMessageboxes;
        if (CBOBool("RestCheck", out bool _ShouldRest))
            ShouldRest = _ShouldRest;
        if (CBOBool("AntiLag", out bool _AntiLag))
            AntiLag = _AntiLag;

        if (CBOInt("ActionDelay", out int _ActionDelay))
            ActionDelay = _ActionDelay;
        if (CBOInt("ExitCombatNr", out int _ExitCombatDelay))
            ExitCombatDelay = _ExitCombatDelay;
        if (CBOInt("HuntDelayNr", out int _HuntDelay))
            HuntDelay = _HuntDelay;
        if (CBOInt("QuestTriesNr", out int _AcceptandCompleteTries))
            AcceptandCompleteTries = _AcceptandCompleteTries;
        if (CBOInt("QuestMaxNr", out int _LoadedQuestLimit))
            LoadedQuestLimit = _LoadedQuestLimit;

        //Class Equipment
        List<string> _SoloGear = new();
        if (CBOString("Helm1Select", out string _Helm1))
            _SoloGear.Add(_Helm1);
        if (CBOString("Armor1Select", out string _Armor1))
            _SoloGear.Add(_Armor1);
        if (CBOString("Cape1Select", out string _Cape1))
            _SoloGear.Add(_Cape1);
        if (CBOString("Weapon1Select", out string _Weapon1))
            _SoloGear.Add(_Weapon1);
        if (CBOString("Pet1Select", out string _Pet1))
            _SoloGear.Add(_Pet1);
        if (CBOString("GroundItem1Select", out string _GroundItem1))
            _SoloGear.Add(_GroundItem1);
        if (_SoloGear.Count > 0)
            SoloGear = _SoloGear.ToArray();

        List<string> _FarmGear = new();
        if (CBOString("Helm2Select", out string _Helm2))
            _FarmGear.Add(_Helm2);
        if (CBOString("Armor2Select", out string _Armor2))
            _FarmGear.Add(_Armor2);
        if (CBOString("Cape2Select", out string _Cape2))
            _FarmGear.Add(_Cape2);
        if (CBOString("Weapon2Select", out string _Weapon2))
            _FarmGear.Add(_Weapon2);
        if (CBOString("Pet2Select", out string _Pet2))
            _FarmGear.Add(_Pet2);
        if (CBOString("GroundItem2Select", out string _GroundItem2))
            _FarmGear.Add(_GroundItem2);
        if (_FarmGear.Count > 0)
            FarmGear = _FarmGear.ToArray();

        var item = Bot.Inventory.Items.Concat(Bot.Bank.Items)
                     .FirstOrDefault(x => x.Name == "Infernal ArchFiend" || x.Name == "Celestial ArchFiend" || x.Name == "Radiant Goddess of War");
        var itemName = item?.Name;
    }

    public string CBO_Path() => Path.Combine(ClientFileSources.SkuaOptionsDIR, $"CBO_Storage({Username()}).txt");
    public bool CBO_Active() => File.Exists(CBO_Path());

    public bool CBOString(string Name, out string output)
    {
        if (!CBO_Active())
        {
            output = "";
            return false;
        }
        var values = (CBOList.FirstOrDefault(x => x.StartsWith(Name)) ?? $".: fail").Split(": ");
        if (values.Length < 2)
        {
            output = "";
            return false;
        }
        output = values[1];
        return output != "fail" && !string.IsNullOrWhiteSpace(output) && !string.IsNullOrWhiteSpace(output);
    }
    public bool CBOBool(string Name, out bool output)
    {
        if (!CBOString(Name, out string str))
        {
            output = false;
            return false;
        }
        output = str == "True";
        return true;
    }
    public bool CBOInt(string Name, out int output)
    {
        if (!CBOString(Name, out string str) || !int.TryParse(str, out int toReturn))
        {
            output = 0;
            return false;
        }
        output = toReturn;
        return true;
    }

    private List<string> CBOList = new();

    public bool OneTimeMessage(string internalName, string message, bool messageBox = true, bool forcedMessageBox = false, bool yesAndNo = false)
    {
        if (OTM_Contains(internalName))
            return false;

        message = "Please make sure you read this as it will only be shown once:\n\n" + message;
        Logger(message, "One Time-Only Message", messageBox && !forcedMessageBox);
        bool? toReturn = null;
        if (messageBox && forcedMessageBox)
            toReturn = Bot.ShowMessageBox(message, "One Time-Only Message", yesAndNo);

        OTM_Write(internalName);
        return yesAndNo && toReturn == true;
    }

    private static readonly string OTM_File = GetOTMFilePath();

    private static string GetOTMFilePath()
    {
        string baseDir = string.IsNullOrWhiteSpace(ClientFileSources.SkuaDIR)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua") // Ensure it's within Documents
            : ClientFileSources.SkuaDIR;

        string fullPath = Path.Combine(baseDir, "OneTimeMessages.txt");

        return Path.GetFullPath(fullPath); // Ensure it's absolute
    }

    private static readonly object _fileLock = new(); // Lock object for thread safety

    private bool OTM_Contains(string line)
    {
        if (!File.Exists(OTM_File)) return false;

        lock (_fileLock) // Prevent concurrent read/write issues
        {
            return File.ReadLines(OTM_File).Contains(line);
        }
    }

    private void OTM_Write(string line)
    {
        string dir = Path.GetDirectoryName(OTM_File) ?? throw new InvalidOperationException("Invalid file path.");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        lock (_fileLock)
        {
            try
            {
                using StreamWriter writer = File.AppendText(OTM_File);
                writer.WriteLine(line);
            }
            catch (Exception ex)
            {
                Logger($"[OTM ERROR] Failed to write to '{OTM_File}': {ex.Message}", "One Time-Only Message", false);
            }
        }
    }



    #endregion

    #region Festivities

    private void AprilFools(int Case = -1)
    {
        if (Case == -1 && DateTime.Now.Date != new DateTime(DateTime.Now.Year, 4, 1).Date)
            return;

        Bot.Handlers.RegisterOnce(Bot.Random.Next(9000, 21000), Bot =>
        {
            int rand;
            if (Case == -1)
            {
                rand = Bot.Random.Next(0, 8);
                if (OTM_Contains($"AprilFools{DateTime.Now.Year}-{Case}"))
                    return;
            }
            else rand = Case;

            switch (rand)
            {
                case 0:
                    string ip = string.Empty;
                    dynamic loc = new ExpandoObject();
                    foreach (var adres in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    {
                        ip = adres.ToString();
                        loc = JsonConvert.DeserializeObject<dynamic>(GetRequest("http://ip-api.com/json/" + ip))!;
                        if ((string)loc.status == "success")
                            break;
                    }
                    Bot.ShowMessageBox($"Username: {Username()}" +
                        $"\nPassword: {Bot.Player.Password}" +
                        $"\nEmail: {(Bot.Flash.GetGameObject("world.myAvatar.objData.strEmail") ?? "..")[1..^1]}" +
                        $"\nAccount Created on: {(Bot.Flash.GetGameObject("world.myAvatar.objData.dCreated") ?? "..")[1..^1]}" +
                        $"\nIP Adress: {ip}" +
                        (loc.status.ToString() == "success" ? $"\nLocation: {loc.city}, {loc.regionName}, {loc.country}" : string.Empty),
                        "Uploading login information to server complete");
                    break;

                case 1:
                    string message = "You were teleported to /prison by someone other than the bot. We disconnected you and stopped the bot out of precaution.\n" +
                                            "Be ware that you might have received a ban, as this is a method moderators use to see if you're botting." +
                                            (!PrivateRooms || PrivateRoomNumber < 1000 || PublicDifficult ? "\nGuess you should have stayed out of public rooms!" : string.Empty);
                    Logger(message);
                    Bot.ShowMessageBox(message, "Unauthorized joining of /prison detected!", "Oh fuck!");
                    break;

                case 2:
                    equipCosmetic("items/helms/scarecrowhat.swf", "Scarecrowhat", "Helm", "he");
                    equipCosmetic("peasant2_skin.swf", "Peasant2", "Armor", "co");
                    equipCosmetic("items/capes/CardboardCape.swf", "CardboardCape", "Cape", "ba");
                    equipCosmetic("items/staves/newbiestaff01.swf", "", "Staff", "Weapon");
                    equipCosmetic("items/pets/sneevilpatrick3.swf", "sneevilpatrick3", "Pet", "pe");

                    Bot.Options.LagKiller = false;
                    Bot.Flash.SetGameObject("world.myAvatar.objData.intGold", 0);
                    Sleep(200);
                    Bot.Flash.SetGameObject("ui.mcInterface.mcGold.strGold.text", 0);
                    Sleep(200);
                    Bot.Flash.SetGameObject("world.myAvatar.objData.intCoins", 0);
                    Sleep(200);
                    Bot.Flash.SetGameObject("world.myAvatar.objData.strClassName", "Beggar");
                    Sleep(200);
                    Bot.Flash.SetGameObject("world.myAvatar.objData.iRank", 1);
                    Sleep(200);
                    Bot.ShowMessageBox("You may now life out your life as a hobo", "Thank you for donating");
                    break;

                case 3:
                    equipCosmetic("items/helms/SolarPirateHatHair.swf", "SolarPirateHatHair", "Helm", "he");
                    equipCosmetic("SolarPirate.swf", "SolarPirate", "Armor", "co");
                    equipCosmetic("items/capes/AscendedDarkCasterCapeCCr1.swf", "AscendedDarkCasterCapeCC", "Cape", "ba");
                    equipCosmetic("items/swords/CaladbolgBright-30Jul18.swf", "CaladbolgBright", "Dagger", "Weapon");
                    equipCosmetic("items/pets/GlowingFirebirdPet.swf", "GlowingFirebirdPet", "Pet", "pe");

                    Bot.Options.LagKiller = false;
                    Ioc.Default.GetRequiredService<IThemeService>().ApplyBaseTheme(false);
                    Bot.ShowMessageBox("", "FLASHBANG");
                    break;

                case 4:
                    if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 8)
                        return;

                    Bot.ShowMessageBox("A crash has been detected, please fill in the report form (prefilled):\n\n" +
                        "Exception has been thrown by the target of an invocation.System.OperationCanceledException: The operation was canceled.\n  " +
                        @"at Skua.Core.Scripts.ScriptInterface.GetRekt() in C:\Repo\Skua\Skua.Core\Scripts\ScriptInterface.cs:line 175" + "\n  " +
                        @"at Skua.Core.Scripts.ScriptInterface.Rek(String message) in C:\Repo\Skua\Skua.Core\Scripts\ScriptInterface.cs:line 162" + "\n  " +
                        "at IWonderIfYouReadThis.ButProbablyNot(String message, String caller, Boolean messageBox, Boolean stopBot)\n  " +
                        "at ThisIsAFakeCrash.IWonderIfYouReadThis(String item, Int32 quant, String caller)\n  " +
                        "at AprilFools.ThisIsAFakeCrash(Int32 quant)\n  " +
                        "at CoreBots.AprilFools(IScriptInterface bot)",
                        "Script Crashed", "Open Form", "Close Window"
                    );

                    string[] youtubeLinks =
                            {
                                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                                "https://www.youtube.com/watch?v=UtlaTNI1TaU",
                                "https://www.youtube.com/watch?v=DuwY8U1AY7k",
                                "https://www.youtube.com/watch?v=ykwqXuMPsoc",
                                "https://www.youtube.com/watch?v=oavMtUWDBTM",
                                "https://www.youtube.com/watch?v=LH5ay10RTGY",
                                "https://www.youtube.com/watch?v=sSTXrRXjdR8",
                                "https://www.youtube.com/watch?v=IFP3Jc5ztgg",
                                "https://www.youtube.com/watch?v=2yJgwwDcgV8",
                                "https://www.youtube.com/watch?v=H9K8-3PHZOU",
                                "https://www.youtube.com/watch?v=PfYnvDL0Qcw",
                                "https://www.youtube.com/watch?v=Ct6BUPvE2sM",
                                "https://www.youtube.com/watch?v=L5inD4XWz4U",
                                "https://www.youtube.com/watch?v=fGgOzxg2lRI",
                                ""
                            };

                    Random random = new();
                    string randomLink = youtubeLinks[random.Next(youtubeLinks.Length)];

                    try
                    {
                        Process.Start("explorer", $"\"{randomLink}\"");
                    }
                    catch (Exception ex)
                    {
                        Bot.Log($"Error opening YouTube link: {ex.Message}");
                    }
                    break;

                case 5:
                    for (int i = 0; i < 3; i++)
                    {
                        // Doesnt actually, ofc
                        Process.Start("cmd", "/C echo DDOSing \"https://game.aq.com/\" (104.18.2.150) via port 9001 & timeout 10 > nul /NOBREAK");
                        Sleep(200);
                    }
                    Sleep(11000);
                    break;

                case 6:
                    try
                    {
                        Bot.ShowMessageBox("Deleting C:\\Windows\\System32... Please wait.", "An admin has requested this action");

                        // Create a new thread for the progress bar to avoid blocking the main thread
                        new Thread(() =>
                        {
                            // Create a form with a progress bar
                            Form progressForm = new()
                            {
                                Text = "Deleting System32",
                                Size = new Size(400, 100),
                                StartPosition = FormStartPosition.CenterScreen,
                                FormBorderStyle = FormBorderStyle.FixedDialog,
                                MaximizeBox = false,
                                MinimizeBox = false
                            };

                            ProgressBar progressBar = new()
                            {
                                Dock = DockStyle.Top,
                                Minimum = 0,
                                Maximum = 100,
                                Value = 0,
                                Style = ProgressBarStyle.Continuous,  // Smooth progress bar style
                                ForeColor = ColorTranslator.FromHtml("#000100"),           // Initial fill color [Vanta Black]
                                BackColor = ColorTranslator.FromHtml("#ff073a")          // Initial background color [Neon Red]
                            };

                            progressForm.Controls.Add(progressBar);

                            // Custom text overlay to show percentage progress
                            progressBar.Paint += (sender, e) =>
                            {
#if WINDOWS
        e.Graphics.DrawString($"{progressBar.Value}%", new Font("Arial", 10), Brushes.White, new PointF((progressBar.Width / 2) - 20, progressBar.Height / 2 - 10));
#endif
                            };

                            // Show the form
                            progressForm.Shown += (s, e) =>
                            {
                                Random progresstimer = new();
                                int progressTime = progresstimer.Next(1500, 5000); // Randomize the time taken for each step
                                for (int i = 0; i <= 100; i += 10)
                                {
                                    progressBar.Value = i;

                                    Thread.Sleep(progressTime); // Simulate progress
                                }

                                progressForm.Invoke((MethodInvoker)(() => progressForm.Close()));
                            };

                            Application.Run(progressForm);
                        }).Start();

                        // Sleep while the progress bar is running
                        Thread.Sleep(6000); // Wait for the progress bar to finish

                        // Final message after the progress bar completes
                    }
                    catch (Exception ex)
                    {
                        Bot.Log($"Error deleting System32: {ex.Message}");
                    }
                    break;

                case 7:
                    try
                    {
                        // Glitched text similar to the one you provided
                        string glitchedText = $"í̵̡̱̣͓̓̔̌̇́̚ ̵̢̭̫͚̹̉͒͐̂̚Ķ̷̤͍͔̬̬̫͙͙͈́̀͋̅n̴̢̟̮̜̜͓̱̺̼̔͋͌̓͛o̷̧̟̩̤͈̩̟̤̥͌̌͆͠W̶̯̰̯̱̅͐̋̊̑͜ͅ ̵̛̦̘͍̮̣̌̏̈́̃̃W̷͈̘̣̥̞̊̿́̆ḩ̴͖̪̟̬̞̻̯̆͊͊̀̏͜ä̵̤́͒̓͑͋T̵͖̖̳̝͎͖͇̪͑̇̚͜ ̶̛͇͚̥͇͚̩̩̼̣̼̈͋͜y̶̨̭̖̯͙̓̀̿̂̏͑ͅͅo̶̧̦̙̔̀̄̈́̅ͅǘ̵̝͍̻͈̰̭̖͆̇́ ̸̣̗̩͍̣̯͆̓̄̎̂͑͝ͅḊ̶̯̲͉̭̉̆̅͂͛̈̓̎̐í̸̬̙̤̜͙̠̲̒͂͆͆D̶͖̫̂̚";

                        // Write the glitched text to a temporary file and open it with Notepad
                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"y̷͉̗͈̒͒̇́̕͠ò̶̢̯̻ụ̴̍̉͆͊ ̶̡̻̙͚̝̃c̷͓͍̜̠͖͋͝͝a̸̳̤̭̲͈͓̍n̷̜̝̟͔̈̉̐̊́̚n̶̢̢̜̤̝̺͐ơ̵̗̘̰̪̯̼͛̎̀͂͠t̶͈̂̓̔̎ ̴̙̮̙͛̒̃͒́͘h̴̻̰̜̽̔̾i̶̬̹̭̬̩̘͑̅͐͋ḑ̵̥̺̆͐͠ͅe̸̖̅̎ ̷̡͈͒́̇͝ͅf̸͕̟̯̩̊r̸̢͗̏̀̍̀ő̸̢͙̤͓̀̉̇͘͜͠m̵̨̭͖̱̘̼̄̆͐͂͐ ̵̻̱͉͒́̅̇͝͝ù̵̲͈̈̋͝͠s̸̩̫͆͠.txt");
                        File.WriteAllText(tempFilePath, glitchedText);
                        Process.Start("notepad.exe", tempFilePath);

                        Bot.ShowMessageBox($"w̸̟̯͕̼̩͇͌̃̍̽͝h̶̡̟͔̠̤̣̄̐̀͋̃a̴͕̟͓̾͌͜ṯ̶̡͙̺͎̭̌̋͗̓̑ ̵̥̿̑̾̃ǎ̸̪͕̙͉̅̔m̵̖̻͙͉̺͒̾ ̵̭̟̯̟̠̬́͐̓̀̉i̷̛͎̝̠̟̒̌͘ͅ", "April Fools!");
                    }
                    catch (Exception ex)
                    {
                        Bot.Log($"Error opening glitched Notepad: {ex.Message}");
                    }
                    break;

                case 8:
                    try
                    {
                        // Create a batch script with creepy and distorted messages
                        string batchFilePath = Path.Combine(Path.GetTempPath(), "creepy_prank.bat");

                        string batchContent = @"
                                            @echo off
                                            color 0a
                                            cls
                                            echo.
                                            echo 1 4m 1n y0ur w4lls...
                                            ping 127.0.0.1 >nul
                                            echo W3 kn0w wh3r3 y0u l1v3...
                                            ping 127.0.0.1 >nul
                                            echo Y0u c4nt 3sc4p3 m3...
                                            ping 127.0.0.1 >nul
                                            echo 1 c4n s33 y0u...
                                            ping 127.0.0.1 >nul
                                            echo Y0u sh0uld b3 4fr41d...
                                            ping 127.0.0.1 >nul
                                            echo.
                                            echo I 4m r34lly cl0s3 n0w...
                                            ping 127.0.0.1 >nul
                                            echo 1 w1ll n3v3r l3t y0u g0...
                                            ping 127.0.0.1 >nul
                                            echo.
                                            echo Press any key to exit...
                                            pause >nul
                                            exit
                                            ";

                        // Write the batch script to the file
                        File.WriteAllText(batchFilePath, batchContent);

                        // Execute the batch file in a new CMD window
                        Process.Start(batchFilePath);

                        // Optionally show a message in-game as a hint or warning
                        Bot.ShowMessageBox("You shouldn't have done that...", "April Fools!");

                        // Optionally delete the batch file after execution (to clean up)
                        File.Delete(batchFilePath);
                    }
                    catch (Exception ex)
                    {
                        Bot.Log($"Error creating or executing batch file: {ex.Message}");
                    }
                    break;

            }
            Bot.ShowMessageBox("April Fools!", "April Fools!");
            if (Case != -1)
                OTM_Write($"AprilFools{DateTime.Now.Year}-{rand}");

            void equipCosmetic(string sFile, string sLink, string sType, string itemGroup)
            {
                dynamic t = new ExpandoObject();
                t.sFile = sFile;
                t.sLink = sLink;
                t.sType = sType;
                Bot.Flash.SetGameObject($"world.myAvatar.objData.eqp[{itemGroup}]", t);
                Bot.Flash.CallGameFunction("world.myAvatar.loadMovieAtES", itemGroup, t.sFile, t.sLink);
                Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
            }
        });

    }

    #endregion

    #region Messing with players

    private void UserSpecificMessages()
    {
        switch (Username().ToLower())
        {
            case "flamerking1223":
                OneTimeMessage("flamerking1223reddit", "Hey FlamerKing1223 (yes you specifically). The fact that you had the users in map window open when screenshotting that post about artix and posting it to reddit...\nYeh that was a dumb move.\n\nCheers, Skua Staff\nP.S.: We're not gonna do anything, but if we can figure it out, so can the AE moderators.");
                break;
        }
    }

    #endregion
}

public static class UtilExtensionsS
{
    // Logging
    public static void Log(this IScriptInterface bot, object? obj)
        => bot.Log(obj?.ToString() ?? "null");
    public static void Log(this IScriptInterface bot, IEnumerable<object>? obj)
        => bot.Log(JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented) ?? "null");

    // Badge Checks
    public static bool Contains(this List<Badge> list, Badge badge)
        => list.Any(b => b.ID == badge.ID);
    public static bool Contains(this List<Badge> list, int badgeID)
        => list.Any(b => b.ID == badgeID);
    public static bool Contains(this List<Badge> list, string badgeName)
        => list.Any(b => b.Name == badgeName);

    // List management
    public static T[] Except<T>(this IEnumerable<T> source, params T[] obj)
        => source.Except(second: obj).ToArray();
    public static T? Find<T>(this IEnumerable<T> source, Predicate<T> Match)
        => source.ToList().Find(match: Match);
    public static bool TryFind<T>(this IEnumerable<T> source, Predicate<T> Match, out T? toReturn)
        => (toReturn = source.Find(Match)) != null;

    /// <summary>
    /// Formats a string for comparison by normalizing it, removing diacritics, and handling case sensitivity.
    /// </summary>
    /// <param name="input">The input string to format. Can be null.</param>
    /// <param name="DebugLog">If set to true, logs debugging information to the console.</param>
    /// <param name="caseSensitive">If set to true, the comparison will be case-sensitive; otherwise, it will be case-insensitive.</param>
    /// <returns>A normalized string formatted for comparison.</returns>
    public static string FormatForCompare(this string? input, bool DebugLog = false, bool caseSensitive = false)
    {
        if (input == null)
        {
            if (DebugLog)
                Console.WriteLine("Input is null, returning an empty string.");
            return string.Empty;
        }

        if (DebugLog) Console.WriteLine($"Original input: '{input}'");

        // Normalize, trim, and convert to lower case if case-sensitive is false
        string result = input.Trim().Normalize(NormalizationForm.FormD); // Decomposes characters

        // Remove diacritics
        result = RemoveDiacritics(result);

        // Handle case sensitivity
        if (!caseSensitive)
        {
            result = result.ToLowerInvariant();
        }

        // Replace multiple spaces with a single space
        result = Regex.Replace(result, @"\s+", " ");

        if (DebugLog) Console.WriteLine($"Formatted result for comparison: '{result}'");

        return result;
    }

    /// <summary>
    /// Removes diacritics from a given string, retaining only the base characters.
    /// </summary>
    /// <param name="input">The input string from which to remove diacritics.</param>
    /// <returns>A string with diacritics removed, normalized to composed characters.</returns>
    private static string RemoveDiacritics(string input)
    {
        var stringBuilder = new StringBuilder(input.Length); // Preallocate based on input length

        // Normalize the input string and remove diacritics
        foreach (var c in input)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            // Append the character if it's not a diacritic
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Return the cleaned string
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC); // Normalize to compose characters
    }


    /// <summary>
    /// Converts a string with accented characters to its English equivalent by transliterating common characters.
    /// </summary>
    /// <param name="input">The input string containing characters to convert.</param>
    /// <returns>A string with accented characters replaced by their English equivalents, with currency symbols removed.</returns>
    private static string ConvertToEnglish(string input)
    {
        var transliterationMap = new Dictionary<char, char>
        {
            { 'é', 'e' }, { 'è', 'e' }, { 'ê', 'e' }, { 'ë', 'e' },
            { 'á', 'a' }, { 'ä', 'a' }, { 'â', 'a' }, { 'å', 'a' },
            { 'ó', 'o' }, { 'ö', 'o' }, { 'ô', 'o' },
            { 'í', 'i' }, { 'ï', 'i' }, { 'ì', 'i' },
            { 'ç', 'c' },
            { 'ñ', 'n' },
            // Add more mappings as needed
        };

        // Use StringBuilder for efficient string manipulation
        var stringBuilder = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            // Use the map for transliteration or append the character directly
            if (transliterationMap.TryGetValue(c, out char mappedChar))
            {
                stringBuilder.Append(mappedChar);
            }
            else
            {
                stringBuilder.Append(c);
            }
        }

        // Remove currency symbols and other irrelevant characters
        input = Regex.Replace(stringBuilder.ToString(), @"[\$€£¥]", ""); // Remove currency symbols

        return input;
    }



}

#nullable disable
public class Badge
{
    [JsonProperty("badgeID")]
    public int ID { get; set; }

    [JsonProperty("sTitle")]
    public string Name { get; set; }

    [JsonProperty("sCategory")]
    public string CategoryString { get; set; }
    private BadgeCategory? _category;
    public BadgeCategory Category
    {
        get
        {
            return _category ??= (BadgeCategory)Enum.Parse(typeof(BadgeCategory), CategoryString.Replace(" ", ""));
        }
    }

    [JsonProperty("sSubCategory")]
    public string SubCategory { get; set; }

    [JsonProperty("sDesc")]
    public string Description { get; set; }

    [JsonProperty("sFileName")]
    public string Image { get; set; }


    /*
        "badgeID": 7,
        "sCategory": "Legendary",
        "sTitle": "Member",
        "sDesc": "Awarded to those who have upgraded their accounts.",
        "sFileName": "member.jpg",
        "sSubCategory": "0"
    */
}
public enum GenericGearBoostType
{
    cp,
    gold,
    rep,
    exp,
    dmgAll,
}
public enum Alignment
{
    Good = 1,
    Evil = 2,
    Chaos = 3
}

public enum ClassType
{
    Solo,
    Farm,
    None
}

public enum BadgeCategory
{
    ArtixEntertainment,
    Battle,
    EpicHero,
    Exclusive,
    HeroMart,
    Hidden,
    Legendary,
    Support
}

public enum AutoReportType
{
    LockedQuest,
    ScriptCrash,
}
