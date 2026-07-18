/*
name: null
description: null
tags: null
version: 1.4.0.5
*/
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Auras;
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
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

public class CoreBots2
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
    public string SoloClass { get; set; } = string.Empty;

    // [Can Change] Mode of soloing class, if it has multiple.
    public ClassUseMode SoloUseMode { get; set; } = ClassUseMode.Base;

    // [Can Change] Whether you wish to equip solo equipment
    public bool SoloGearOn { get; set; } = true;

    // [Can Change] Names of your soloing equipment
    public string[] SoloGear { get; set; } = [];

    // [Can Change] Name of your farming class
    public string FarmClass { get; set; } = string.Empty;

    // [Can Change] Mode of farming class, if it has multiple.
    public ClassUseMode FarmUseMode { get; set; } = ClassUseMode.Base;

    // [Can Change] Whether you wish to equip farm equipment
    public bool FarmGearOn { get; set; } = true;

    // [Can Change] Names of your farming equipment
    public string[] FarmGear { get; set; } = [];

    // [Can Change] Name of your dodge class
    public string DodgeClass { get; set; } = string.Empty;

    // [Can Change] Mode of dodge class, if it has multiple.
    public ClassUseMode DodgeUseMode { get; set; } = ClassUseMode.Base;

    // [Can Change] Whether you wish to equip dodge equipment
    public bool DodgeGearOn { get; set; } = true;

    // [Can Change] Names of your dodge equipment
    public string[] DodgeGear { get; set; } = [];

    // [Can Change] Name of your bossing class
    public string BossClass { get; set; } = string.Empty;

    // [Can Change] Mode of boss class, if it has multiple.
    public ClassUseMode BossUseMode { get; set; } = ClassUseMode.Base;

    // [Can Change] Whether you wish to equip bossing equipment
    public bool BossGearOn { get; set; } = true;

    // [Can Change] Names of your bossing equipment
    public string[] BossGear { get; set; } = [];

    // [Can Change] Some Sagas use the hero alignment to give extra reputation, change to your desired rep (Alignment.Evil or Alignment.Good).
    public int HeroAlignment { get; set; } = (int)Alignment.Evil;

    // [Can Change] Member Status
    public bool IsMember { get; set; }
    public bool AutoEnhance { get; set; } = true;
    public bool BestGear { get; set; } = false;

    private static CoreBots2? _instance;
    public static CoreBots2 Instance => _instance ??= new CoreBots2();
    private IScriptInterface Bot => IScriptInterface.Instance;

    private const string DiscordLink = "https://discord.gg/CKKbk2zr3p";

    private readonly CoreSkills _coreSkills = new();
    private Stopwatch? _scriptStopwatch;

    #endregion Declerations

    #region Start/Stop

    private static readonly System.Net.Http.HttpClient _client = CreateClient();

    private static System.Net.Http.HttpClient CreateClient()
    {
        System.Net.Http.HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd("SkuaVersionChecker");
        return client;
    }

    public static void RunSync(Func<Task> asyncAction)
    {
        asyncAction().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Set common bot options to desired value
    /// </summary>
    /// <param name="changeTo">Value the options will be changed to</param>
    /// <param name="disableClassSwap"></param>
    /// <param name="disableCoreSkills">If true, skip starting legacy CoreSkills.</param>
    public void SetOptions(bool changeTo = true, bool disableClassSwap = false, bool disableCoreSkills = false)
    {
        EnforceInvariantCulture();

        RunSync(() => SkuaVersionCheckerAsync());

        // Clear Handlers incase script starts a 2nd time somehow whilst the script is running 
        Bot.Handlers.Clear();

        if (changeTo)
        {
            // Prevent duplicate subscriptions if script restarts internally
            Bot.Events.ScriptStopping -= CrashDetector;
            Bot.Events.ScriptStopping += CrashDetector;

            _scriptStopwatch = Stopwatch.StartNew();

            loadedBot = Bot.Manager.LoadedScript?
                .Replace("\\", "/")
                .Split("/Scripts/")
                .Last()
                .Replace(".cs", "") ?? "Unknown";

            Logger($"Bot Started [{loadedBot}]");

            if (
                Bot.Config != null
                && Bot.Config.Options != null
                && Bot.Config.Options.Contains(SkipOptions)
                && !Bot.Config.Get<bool>(SkipOptions)
            )
                Bot.Config.Configure();

            const int maxLoginAttempts = 3;
            int loginAttempt = 0;

            // Pre-calc fallback server once
            string server =
                Bot.Options.ReloginServer ??
                Bot.Servers.CachedServers?
                    .FirstOrDefault(s =>
                        s.Name != "Class Test Realm" &&
                        s.Online &&
                        s.PlayerCount < s.MaxPlayers)?.Name
                ?? "Twilly";

            while (!Bot.Player.LoggedIn && !Bot.ShouldExit)
            {
                if (Bot.Servers.CachedServers == null || !Bot.Servers.CachedServers.Any())
                {
                    Logger("Server list not ready, waiting...");
                    Sleep(3000);
                    continue;
                }

                loginAttempt++;
                Logger($"Auto Login attempt {loginAttempt}/{maxLoginAttempts} → {server}");

                try
                {
                    bool relogSuccess = Bot.Servers.EnsureRelogin(server);
                    if (relogSuccess)
                        break;

                    Logger("Relogin failed, retrying...");
                }
                catch (Exception ex)
                {
                    Logger($"Relogin exception: {ex.Message}");
                }

                Sleep(5000);
            }

            if (!Bot.Player.LoggedIn)
            {
                if (!Bot.ShouldExit)
                {
                    Logger(
                        "Please log-in before starting the bot.\nIf already logged in but still seeing this, reinstall CleanFlash.",
                        messageBox: true,
                        stopBot: true
                    );
                }
                return;
            }

        }

        // Anti-Counter atk Measures for mobs
        Bot.UltraBossHelper.EnableCounterAttack();

        /* Test Server IPs:
            Twilly ServerTest  
            Artix ServerTest 
                - Server IP - sockpuppet.aq.com
       */
        if (changeTo && Bot.Player.LoggedIn && !String.IsNullOrEmpty(Bot.Player.ServerIP))
        {
            if (Bot.Player.ServerIP == "sockpuppet.aq.com")
            {
                Logger("You're current on a test server!! This will cause issues with scripts\n" + "please logout and login to a non-test server manually\n" + "*or* set your manager to a non-Test server", stopBot: true );
            }
        }

        int loadAttempts = 0;
        while (!Bot.Player.Loaded && !Bot.ShouldExit)
        {
            if (loadAttempts >= 30)
            {
                Logger("Player stuck in spirit state after 30s, logging out to trigger auto-relogin.");
                Bot.Options.AutoRelogin = true;
                Bot.Servers.Logout();
                loadAttempts = 0;
            }
            Bot.Sleep(1000);
            loadAttempts++;
        }

        ReadCBO();
        #region Social Privacy Options

        bool isStarting = changeTo;
        CBOBool("IncognitoMode", out bool IncognitoModeOn);
        if (!IncognitoModeOn)
        {
            if (isStarting == true)
            {
                Logger("Incognito Mode in CBO is off. Skipping privacy settings.");
            }
        }
        else
        {
            bool disabling = isStarting;
            bool warned = false;

            foreach (
                (string key, string label) in new Dictionary<string, string>
                {
                    { "bGoto", "Goto" },
                    { "bParty", "Party invites" },
                    { "bFriend", "Friend invites" },
                    { "bDuel", "Duel invites" },
                    { "bGuild", "Guild invites" },
                    { "bWhisper", "Whisper" },
                }
            )
            {
                if (label == "Goto" && !loadedBot.ToLower().Contains("butler"))
                    continue;

                bool current = Bot.Flash.GetGameObject<bool>($"uoPref.{key}");
                if (disabling ? current : !current)
                {
                    if (disabling && !warned)
                    {
                        Logger(
                            "[SetOptions] Turning certain \"Social\" options off to help protect you"
                        );
                        warned = true;
                    }

                    Logger($"[SetOptions] {(disabling ? "Turning off" : "Re-enabling")}: {label}");
                    SendPackets($"%xt%zm%cmd%1%uopref%{key}%{(!disabling).ToString().ToLower()}%");
                    Bot.Sleep(500);
                }
            }

            if (disabling)
                GC.Collect();
        }

        #endregion Social Privacy Options

        // Set the member status
        IsMember = isUpgraded();

        // Common Options
        Bot.Options.RejectAllDrops = false;
        Bot.Options.PrivateRooms = false;
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.QuestAcceptAndCompleteTries = AcceptandCompleteTries;
        Bot.Options.AutoRelogin = true;
        Bot.Options.SafeTimings = changeTo;
        Bot.Options.RestPackets = changeTo && ShouldRest;
        Bot.Options.InfiniteRange = changeTo;
        Bot.Options.SkipCutscenes = changeTo;

        // Lite Options
        Bot.Lite.ReacceptQuest = false;
        Bot.Lite.DisableRedWarning = true;
        Bot.Lite.CharacterSelectScreen = false;
        Bot.Lite.UntargetDead = true;
        Bot.Lite.UntargetSelf = true;
        Bot.Lite.SmoothBackground = true;
        Bot.Lite.ShowMonsterType = true;
        Bot.Lite.CustomDropsUI = true;
        Bot.Lite.AurasUI = true;
        Bot.Lite.QuantityWarnings = false;
        Bot.Lite.VisualSkillCooldowns = true;
        Bot.Lite.ChatUI = true;
        Bot.Lite.QuestLogTurnIns = true;
        Bot.Lite.DisableSoundFx = true;

        // Drop Options
        Bot.Drops.RejectElse = changeTo;
        Bot.Drops.StopAsync();
        Bot.Drops.Clear();
        Bot.Drops.Start();

        CollectData(changeTo);

        #region Required things that must be done before starting the Script

        if (changeTo)
        {
            //Start scripts Safely by starting them in the house ( or whitemap if house desnt exist) if the start map is battleon
            if (
                new[] { "battleon", "oaklore", "bludrutbrawl" }.Any(m =>
                    Bot.Map.Name.Equals(m, StringComparison.OrdinalIgnoreCase)
                )
            )
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
                            toSend =
                                $"{{\"t\":\"xt\",\"b\":{{\"r\":-1,\"o\":{{\"cmd\":\"moveToArea\",\"areaName\":\"house\",\"uoBranch\":{JsonConvert.SerializeObject(data.uoBranch)},\"strMapFileName\":\"{data.strMapFileName}\",\"intType\":\"1\",\"monBranch\":[],\"houseData\":{Regex.Replace(JsonConvert.SerializeObject(data.houseData), Username(), "Skua user", RegexOptions.IgnoreCase)},\"sExtra\":\"\",\"areaId\":{data.areaId},\"strMapName\":\"house\"}}}}}}";
                            Bot.Events.ExtensionPacketReceived -= modifyPacket;
                        }
                    }
                }
                else
                    Bot.Send.Packet(
                        $"%xt%zm%cmd%1%tfer%{Username()}%whitemap-{PrivateRoomNumber}%"
                    );
            }
            // Open Bank on startup ensuring current window is `Bank`, then load the bank information.
            if (Bot.Flash.GetGameObject("ui.mcPopup.currentLabel") != "\"Bank\"")
                Bot.Bank.Open();
            Bot.Bank.Load();
            Bot.Bank.Loaded = true;

            AutoAddTags();
            DateTime now = DateTime.Now;
            if (now >= new DateTime(now.Year, 12, 25) && now < new DateTime(now.Year, 12, 26, 12, 0, 0))
                OneTimeMessage("Xmax2025", "Merry Christmas - Skua Team");


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

            EquipmentBeforeBot.AddRange(
                Bot.Inventory.Items.Where(i => i.Equipped).Select(x => x.Name)
            );

            var currentClassName = Bot.Player.CurrentClass?.Name ?? "generic";

            usingSoloGeneric = SoloClass.ToLower() == "generic";
            usingFarmGeneric = FarmClass.ToLower() == "generic";
            usingDodgeGeneric = DodgeClass.ToLower() == "generic";
            usingBossGeneric = BossClass.ToLower() == "generic";

            if (!disableCoreSkills)
                EnableCoreSkills();

            // Alive Check handling
            Bot.Events.MapChanged += CleanKilledMonstersList;
            Bot.Events.MonsterKilled += KilledMonsterListener;
            Bot.Events.ExtensionPacketReceived += RespawnListener;

            Logger("Bot Configured");

            // Bunch of things that are done in the background and you dont need the bot to wait for
            void SetOptionsAsync()
            {
                #region Handlers

                Task.Run(() =>
                {
                    Task.Run(() =>
                    {
                        if (
                            OneTimeMessage(
                                "discordV11",
                                "Our discord server was recently deleted again (March 29th 2023), click yes if you wish to (re-)join the server",
                                true,
                                true,
                                true
                            )
                        )
                            Process.Start("explorer", DiscordLink);
                    });

                    // Butler directory cleaning
                    if (Directory.Exists(ButlerLogDir))
                    {
                        if (File.Exists(ButlerLogPath()))
                            File.Delete(ButlerLogPath());

                        string[] files = Directory.GetFiles(ButlerLogDir);
                        if (
                            files.Any(x =>
                                x.Contains("~!") && x.Split("~!").First() == Username().ToLower()
                            )
                        )
                            File.Delete(
                                files.First(x =>
                                    x.Contains("~!")
                                    && x.Split("~!").First() == Username().ToLower()
                                )
                            );
                    }

                    // AFK Handler
                    Bot.Send.Packet("%xt%zm%afk%1%false%");
                    Sleep();
                    bool TimerRunning = false;
                    Bot.Handlers.RegisterHandler(
                        5000,
                        b =>
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
                        },
                        "AFK Handler"
                    );

                    // Settin Loaded Quest Limiter
                    Bot.Handlers.RegisterHandler(
                        3000,
                        b =>
                        {
                            if (Bot.Quests.Tree.Count > LoadedQuestLimit)
                            {
                                Bot.Flash.SetGameObject("world.questTree", new ExpandoObject());
                            }
                        },
                        "Quest-Limit Handler"
                    );

                    // Prison Detector
                    if (loadedBot.Replace("\\", "/") != "Tools/Butler")
                    {
                        Bot.Events.MapChanged += PrisonDetector;
                        void PrisonDetector(string map)
                        {
                            if (
                                map.ToLower() == "prison"
                                && !joinedPrison
                                && !prisonListernerActive
                            )
                            {
                                prisonListernerActive = true;
                                Bot.Options.AutoRelogin = false;
                                Bot.Servers.Logout();
                                string message =
                                    "You were teleported to /prison by someone other than the bot. We disconnected you and stopped the bot out of precaution.\n"
                                    + "Be ware that you might have received a ban, as this is a method moderators use to see if you're botting."
                                    + (
                                        !PrivateRooms || PrivateRoomNumber < 1000 || PublicDifficult
                                            ? "\nGuess you should have stayed out of public rooms!"
                                            : string.Empty
                                    );
                                Logger(message);
                                Bot.ShowMessageBox(
                                    message,
                                    "Unauthorized joining of /prison detected!",
                                    "Oh fuck!"
                                );
                                Bot.Events.MapChanged -= PrisonDetector;
                                Bot.StopSync(true);
                            }
                            Bot.Events.MapChanged -= PrisonDetector;
                        }
                    }

                #endregion Handlers

                    // Anti-lag option
                    if (AntiLag)
                    {
                        Bot.Options.LagKiller = changeTo;

                        // Some maps are codded horrible and the animations can cause lag or freezes, so we'll turn all the animations off
                        Bot.Lite.FreezeMonsterPosition = changeTo;
                        Bot.Lite.DisableMonsterAnimation = changeTo;
                        Bot.Lite.DisableDamageStrobe = changeTo;
                        Bot.Lite.DisableSelfAnimation = changeTo;
                        Bot.Lite.DisableWeaponAnimation = changeTo;
                        Bot.Lite.DisableSkillAnimation = changeTo;
                        Bot.Lite.DisableAuraAnimations = changeTo;

                        Bot.Flash.SetGameObject("stage.frameRate", 10);
                        if (!Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
                            Bot.Flash.CallGameFunction("world.toggleMonsters");
                    }

                    // Identity Protection
                    // Bot.Options.CustomName = "SkuaLabRat";
                    // Bot.Options.CustomGuild = "Skua-cide Squad";

                    // Holiday Handlers
                    AprilFools();

                    //Fucking with specific people
                    UserSpecificMessages();
                });
            }
        }

        if (!changeTo && _scriptStopwatch != null)
        {
            StopCoreSkills();
            Bot.Drops.Clear();
            _scriptStopwatch.Stop();
            Logger($"Script ran for {_scriptStopwatch.Elapsed:hh\\:mm\\:ss}");
            _scriptStopwatch = null;
        }
    }

    // Whether the player is a Member (set to true if necessary during setOptions)
    public bool isUpgraded()
    {
        // Get membership days left as a string
        string? membershipDaysLeftString = Bot.Flash.GetGameObject(
            "world.myAvatar.objData.iUpgDays"
        );

        // Attempt to parse the string into an integer
        if (int.TryParse(membershipDaysLeftString, out int membershipDaysLeft))
        {
            // Return true if membership days are greater than 0
            return membershipDaysLeft > 0;
        }

        // If parsing fails, return false (not a member)
        return false;
    }

    public List<string> BankingBlackList
    {
        get => _BankingBlackList ??= [];
        set => _BankingBlackList = value;
    }
    public List<string>? _BankingBlackList;

    private readonly List<string> EquipmentBeforeBot = [];
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
                                toSend =
                                    $"{{\"t\":\"xt\",\"b\":{{\"r\":-1,\"o\":{{\"cmd\":\"moveToArea\",\"areaName\":\"house\",\"uoBranch\":{JsonConvert.SerializeObject(data.uoBranch)},\"strMapFileName\":\"{data.strMapFileName}\",\"intType\":\"1\",\"monBranch\":[],\"houseData\":{Regex.Replace(JsonConvert.SerializeObject(data.houseData), Username(), "Skua user", RegexOptions.IgnoreCase)},\"sExtra\":\"\",\"areaId\":{data.areaId},\"strMapName\":\"house\"}}}}}}";
                                Bot.Events.ExtensionPacketReceived -= modifyPacket;
                            }
                        }
                    }
                    else
                        Bot.Send.Packet(
                            $"%xt%zm%cmd%1%tfer%{Username()}%whitemap-{PrivateRoomNumber}%"
                        );
                }
                else if (
                    new[]
                    {
                        "off",
                        "disabled",
                        "disable",
                        "stop",
                        "same",
                        "currentmap",
                        "bot.map.currentmap",
                        "none",
                        "None",
                        string.Empty,
                    }.Any(m => m == _stopLoc)
                )
                {
                    // Nothing happens
                }
                else
                    Bot.Send.Packet(
                        $"%xt%zm%cmd%1%tfer%{Username()}%{_stopLoc}-{PrivateRoomNumber}%"
                    );

                if (EquipmentBeforeBot.Any())
                {
                    string[] PVPMaps = new[]
                    {
                        "bludrutbrawl",
                        "darkoviapvp",
                        "dagepvp",
                        "deathpitbrawl",
                        "frostbrawl",
                        "chaosbrawl",
                        "doomarenaa",
                        "doomarenab",
                        "doomarenac",
                        "doomarenad",
                    };

                    if (PVPMaps.Contains(Bot.Map.Name))
                        Join("whitemap");
                    else
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
                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        await Bot.Manager.RestartScriptAsync();
                        if (Bot.Player.LoggedIn)
                            return;
                        Logger("Bot stopped due to Auto-Relogin failure.");
                    }
                    catch (OperationCanceledException)
                    {
                        Logger("Auto-relogin timed out after 30 seconds.");
                    }
                });
            }
            else
                Logger("Bot stopped due to player logout.");
        }
        else
            Logger("Bot stopped successfully.");

        GC.KeepAlive(_instance);
        return scriptFinished;

        void StopBotAsync()
        {
            Task.Run(() =>
            {
                SavedState(false);

                Bot.Events.MapChanged -= CleanKilledMonstersList;
                Bot.Events.MonsterKilled -= KilledMonsterListener;
                Bot.Events.ExtensionPacketReceived -= RespawnListener;
                if (AntiLag)
                {
                    Bot.Options.SetFPS = 60;
                    if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
                        Bot.Flash.CallGameFunction("world.toggleMonsters");
                }

                Bot.Options.CustomName = Bot.Player.Username ?? Username().ToUpper();
                // Bot.Options.CustomName = Username().ToUpper();
                string? guild = Bot.Flash.GetGameObject<string>(
                    "world.myAvatar.objData.guild.Name"
                );
                Bot.Options.CustomGuild = guild != null ? $"< {guild} >" : string.Empty;

                if (File.Exists(ButlerLogPath()))
                    File.Delete(ButlerLogPath());
            });
        }
    }

    private bool scriptFinished = true;

    private void StopCoreSkills()
    {
        Bot.Events.ScriptStopping -= StopBotEvent;
        _coreSkills.DisableSkills();
        Bot.Skills.Stop();
    }

    private void EnableCoreSkills()
    {
        Bot.Skills.Stop();
        _coreSkills.EnableSkills();
        Bot.Events.ScriptStopping -= StopBotEvent;
        Bot.Events.ScriptStopping += StopBotEvent;
    }

    public bool StopBotEvent(Exception? e)
    {
        StopCoreSkills();
        Bot.Events.ScriptStopping -= StopBotEvent;
        SetOptions(false);
        return StopBot(e != null);
    }

    public bool CrashDetector(Exception? e)
    {
        if (e == null || e is OperationCanceledException)
            return scriptFinished;

        string eSlice = e.Message + "\n" + e.InnerException;
        List<string> logs = Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script);
        logs = [.. logs.Skip(logs.Count > 5 ? (logs.Count - 5) : logs.Count)];
        if (
            Bot.ShowMessageBox(
                "A crash has been detected, please fill in the report form (prefilled):\n\n"
                    + eSlice,
                "Script Crashed",
                "Open Form",
                "Close Window"
            ).Text == "Open Form"
        )
        {
            string url =
                "\"https://docs.google.com/forms/d/e/1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg/viewform?usp=pp_url&"
                + "entry.2118425091=Bug+Report&"
                + $"entry.290078150={Bot.Manager.LoadedScript.Split("Scripts").Last().Replace('/', '\\')[1..].Replace(".cs", "")}&"
                + "entry.1803231651=It+stopped+at+the+wrong+time+(crash)&"
                + $"entry.1954840906={logs.Join("%0A")}&"
                + $"entry.285894207={eSlice}&\"";
            url = url.Replace("\r\n", "%0A").Replace("\n", "").Replace(" ", "%20");

            Process p = new();
            p.StartInfo.FileName = "rundll32";
            p.StartInfo.Arguments = "url,OpenURL " + url;
            p.StartInfo.WorkingDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.System).Split('\\').First()
                + "\\";
            p.Start();

            Logger(
                "Thank you for reporting the crash. Below you will find the information you will need to report, in case it isn't being auto filled"
            );
        }
        else
            Logger("A crash has occurred. Please report it in the form with the details below");

        Bot.Log("--------------------------------------");
        Logger("Last 5 Logs:");
        Bot.Log(logs.Join('\n'));
        Bot.Log("--------------------------------------");
        Logger("Crash (Debug)");
        Bot.Log(eSlice);
        Bot.Log("--------------------------------------");
        Bot.Events.ScriptStopping -= CrashDetector;

        return false;
    }

    public List<string> GetLogs(LogType type = LogType.Script) =>
        (_logService ??= Ioc.Default.GetRequiredService<ILogService>()).GetLogs(type);

    private ILogService? _logService;

    public void ScriptMain(IScriptInterface Bot)
    {
        RunCore();
    }

    #endregion Start/Stop

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

            if (
                (toInv && Bot.Inventory.GetQuantity(item) >= quant)
                || (
                    !toInv
                    && Bot.Bank.TryGetItem(item, out InventoryItem? _item)
                    && _item != null
                    && _item.Quantity >= quant
                )
            )
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

            if (
                (toInv && Bot.Inventory.GetQuantity(_itemID) >= quant)
                || (
                    !toInv
                    && Bot.Bank.TryGetItem(_itemID, out InventoryItem? _item)
                    && _item != null
                    && _item.Quantity >= quant
                )
            )
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
    public bool CheckInventory(
        string[]? itemNames,
        int quant = 1,
        bool any = false,
        bool toInv = true
    )
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
    public T? InitializeWithRetries<T>(Func<T> initializer, int retries = 5, int delay = 1000)
    {
        T? result = default;

        for (int i = 0; i < retries; i++)
        {
            try
            {
                result = initializer();
                if (!EqualityComparer<T>.Default.Equals(result, default))
                    return result;
            }
            catch (Exception ex)
            {
                Logger($"Attempt {i + 1}/{retries} threw exception: {ex.Message}");
            }

            Logger($"Attempt {i + 1}/{retries}: Initialization failed. Retrying...");
            Sleep(delay);
        }

        Logger($"Initialization failed after {retries} attempts at: {initializer.Method.Name}.");
        return default;
    }

    /// <summary>
    /// Checks if there is enough space in the inventory for the specified items
    /// and logs a message if space is insufficient. Attempts to bank misc AC items
    /// to free up required slots.
    /// </summary>
    /// <param name="counter">Reference to a counter tracking how many items are already in inventory.</param>
    /// <param name="items">Array of item names to check space for.</param>
    public void CheckSpaces(ref int counter, params string[] items)
    {
        foreach (string item in items)
            if (CheckInventory(item, toInv: false))
                counter++;

        int requiredSlots = items.Length - counter;

        // Attempt to bank misc AC items to free up space if needed
        if (requiredSlots > 1 && Bot.Inventory.FreeSlots < requiredSlots)
            BankACMisc(requiredSlots);

        // Re-check free slots and alert if still insufficient
        if (Bot.Inventory.FreeSlots < requiredSlots)
        {
            string plural = requiredSlots != 1 ? "s" : "";
            Logger(
                $"Not enough free slot{plural}, please clear {requiredSlots} slot{plural}",
                messageBox: true,
                stopBot: true
            );
        }
    }

    /// <summary>
    /// Moves specified items by their names from the bank to inventory or house,
    /// skipping items that are equipped, in use, or not present in any location.
    /// Only applicable for whitelisted categories; retries each move up to 20 times.
    /// Logs success or failure for each item.
    /// </summary>
    /// <param name="items">
    /// Array of item names to transfer from the bank. Items will be skipped if
    /// they are already in inventory, house, or not found anywhere.
    /// </param>
    public void Unbank(params string[] items)
    {
        if (items == null || items.Length == 0)
            return;

        if (Bot.Player.InCombat)
            JumpWait();

        int requiredSpaces = items.Length;

        foreach (string item in items)
        {
            bool inHouse = Bot.House.Contains(item);
            bool inInventory = Bot.Inventory.Contains(item);
            bool inBank = Bot.Bank.Contains(item);
            if (inHouse || inInventory || (!inHouse && !inInventory && !inBank))
            {
                requiredSpaces--;
                continue;
            }

            if (inBank && (!inInventory || !inHouse))
            {
                if (
                    Bot.Inventory.FreeSlots <= 0
                    && Bot.Inventory.Slots != 0
                    && Bot.Inventory.UsedSlots >= Bot.Inventory.Slots
                )
                {
                    Logger($"⚠️ Your inventory is full ({Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}. Attempting to make room by banking Misc Ac items");

                    // Try *All* banking metods to try and make space.
                    BankACMisc();
                    BankACUnenhancedGear();
                    BankACHouseItems();
                }

                //Retry after banking misc stuff
                if (
                    Bot.Inventory.FreeSlots <= 0
                    && Bot.Inventory.Slots != 0
                    && Bot.Inventory.UsedSlots >= Bot.Inventory.Slots
                )
                {
                    if (Bot.Inventory.FreeSlots <= 0)
                        Logger($"⚠️ Your inventory is full ({Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}) — please make {requiredSpaces} space(s) and restart the bot.",
                            messageBox: true,
                            stopBot: true
                        );
                    else return;
                }

                bool isHouseItem =
                    Bot.Bank.TryGetItem(item, out InventoryItem? bankItem)
                    && bankItem != null
                    && (
                        bankItem.CategoryString == "House"
                        || bankItem.CategoryString == "Wall Item"
                        || bankItem.CategoryString == "Floor Item"
                    );

                bool success = false;

                if (isHouseItem)
                {
                    if (bankItem == null)
                    {
                        Logger($"❌ Failed to get bank item for '{item}', skipping.");
                        continue;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        SendPackets(
                            $"%xt%zm%bankToInv%{Bot.Map.RoomID}%{bankItem.ID}%{bankItem.CharItemID}%"
                        );
                        Sleep();
                        if (Bot.House.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Bot.Bank.EnsureToInventory(item);
                        Sleep();
                        if (Bot.Inventory.Contains(item))
                        {
                            success = true;
                            break;
                        }
                    }
                }

                if (!success)
                {
                    Logger($"🚫 Failed to unbank {item}, skipping it.");
                    continue;
                }

                Logger($"✅ {item} successfully moved from bank! 🏦➡️🎒");
            }
        }
    }

    /// <summary>
    /// Transfers specified items by their unique IDs from the bank to inventory.
    /// Skips items already in inventory, house, or not found in bank.
    /// Retries each transfer up to 20 times, logging success or failure.
    /// </summary>
    /// <param name="itemIDs">Array of item IDs to transfer from bank.</param>
    public void Unbank(params int[] itemIDs)
    {
        if (itemIDs == null || itemIDs.Length == 0 || !itemIDs.Any(id => id != 0))
            return;

        if (Bot.Player.InCombat)
            JumpWait();

        int requiredSpaces = itemIDs.Length;

        foreach (int itemID in itemIDs)
        {
            bool inHouse = Bot.House.Contains(itemID);
            bool inInventory = Bot.Inventory.Contains(itemID);
            bool inBank = Bot.Bank.Contains(itemID);

            if (inHouse || inInventory || (!inHouse && !inInventory && !inBank))
            {
                requiredSpaces--;
                continue;
            }

            if (inBank && (!inInventory || !inHouse))
            {
                ItemBase? bankItem = Bot.Bank.Items?.FirstOrDefault(x => x?.ID == itemID);
                if (bankItem == null)
                {
                    Logger($"❌ Failed to find item with ID {itemID}, skipping it.");
                    continue;
                }

                if (
                    Bot.Inventory.FreeSlots <= 0
                    && Bot.Inventory.Slots != 0
                    && Bot.Inventory.UsedSlots >= Bot.Inventory.Slots
                )
                {
                    Logger(
                        $"⚠️ Your inventory is full ({Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}) — please make {requiredSpaces} space(s) and restart the bot.",
                        messageBox: true,
                        stopBot: true
                    );
                    return;
                }

                bool isHouseItem =
                    Bot.Bank.TryGetItem(itemID, out InventoryItem? invItem)
                    && invItem != null
                    && (
                        invItem.CategoryString == "House"
                        || invItem.CategoryString == "Wall Item"
                        || invItem.CategoryString == "Floor Item"
                    );

                bool success = false;
                if (isHouseItem)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        SendPackets(
                            $"%xt%zm%bankToInv%{Bot.Map.RoomID}%{invItem!.ID}%{invItem.CharItemID}%"
                        );
                        Sleep();
                        if (Bot.House.Contains(itemID))
                        {
                            success = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Bot.Bank.EnsureToInventory(itemID);
                        Sleep();
                        if (Bot.Inventory.Contains(itemID))
                        {
                            success = true;
                            break;
                        }
                    }
                }

                if (!success)
                {
                    Logger($"🚫 Failed to unbank {bankItem.Name}, skipping it.");
                    continue;
                }

                Logger($"✅ {bankItem.Name} successfully moved from bank! 🏦➡️🎒");
            }
        }
    }
    private static readonly HashSet<int> Extras = [18927, 38575];

    /// <summary>
    /// Transfers specified items by name from inventory/house to bank.
    /// Skips equipped, blacklisted, or nonexistent items.
    /// Retries up to 5 times on failures. Handles house items separately.
    /// </summary>
    /// <param name="items">Item names to move to bank.</param>
    public void ToBank(params string[] items)
    {
        if (items == null || !items.Any(name => !string.IsNullOrEmpty(name)))
            return;

        List<ItemCategory> whiteList =
    [
        ItemCategory.Note,
        ItemCategory.Item,
        ItemCategory.Resource,
        ItemCategory.QuestItem,
    ];

        foreach (string? item in items)
        {
            if (string.IsNullOrEmpty(item)
                || item == FarmClass
                || item == SoloClass
                || item == DodgeClass
                || item == BossClass
                || FarmGear.Contains(item)
                || SoloGear.Contains(item)
                || DodgeGear.Contains(item)
                || BossGear.Contains(item))
                continue;

            if (Bot.Inventory.IsEquipped(item) || (Bot.House?.IsEquipped(item) ?? false))
            {
                Logger($"⚔️ Can't bank equipped item: {item}");
                continue;
            }

            if (Bot.Bank.Contains(item))
            {
                Logger($"ℹ️ {item} is already in bank, skipping.");
                continue;
            }

            ItemBase? inventoryItem = Bot
                .Inventory.Items.Concat(Bot.House?.Items ?? Enumerable.Empty<InventoryItem>())
                .FirstOrDefault(x => x?.Name == item);

            if (inventoryItem == null)
            {
                DebugLogger($"❌ {item} not found in inventory, skipping.", "ToBank Debug");
                continue;
            }

            // Coin (AC) items have infinite bank space, only block non-coin items when bank is full
            if (!inventoryItem.Coins && Bot.Bank.FreeSlots <= 0)
            {
                Logger($"🏦 Bank is full, skipping {item}.");
                continue;
            }

            if (
                (!whiteList.Contains(inventoryItem.Category) && !inventoryItem.Coins)
                || BankingBlackList.Contains(item)
                || Extras.Contains(inventoryItem.ID)
            )
            {
                Logger($"⛔ {item} is blacklisted or excluded.");
                continue;
            }

            InventoryItem? houseItem = null;
            bool itemIsForHouse =
                Bot.House != null
                && Bot.House.TryGetItem(item, out houseItem)
                && houseItem != null
                && (
                    houseItem.CategoryString == "House"
                    || houseItem.CategoryString == "Wall Item"
                    || houseItem.CategoryString == "Floor Item"
                );

            if (!itemIsForHouse)
            {
                JumpWait();
                bool success = false;
                for (int i = 0; i < 5; i++)
                {
                    if (Bot.Inventory.EnsureToBank(item))
                    {
                        success = true;
                        break;
                    }
                    Sleep();
                }
                if (!success && !Bot.Bank.Contains(item))
                {
                    Logger($"🚫 Failed to bank {item} after 5 attempts, skipping.");
                    continue;
                }
            }
            else
            {
                if (houseItem == null)
                    continue;

                if ((Bot.House?.FreeSlots ?? 0) <= 0)
                {
                    Logger($"🏠 House storage full, skipping {item}.");
                    continue;
                }

                JumpWait();
                SendPackets($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{houseItem.ID}%{houseItem.CharItemID}%");
                Bot.Wait.ForTrue(() => !(Bot.House?.Contains(item) ?? false), 20);

                if (Bot.House?.Items?.Any(x => x?.Name == item) ?? false)
                {
                    Logger($"🚫 Failed to bank {item} from house storage, skipping.");
                    continue;
                }
            }

            Logger($"💰 {item} moved to bank successfully! 🎒➡️🏦");
        }
    }
    /// <summary>
    /// Transfers specified items by ID from inventory/house to bank.
    /// Skips equipped, blacklisted, or nonexistent items.
    /// Retries up to 20 times on failures. Handles house items separately.
    /// </summary>
    /// <param name="items">Item IDs to move to bank.</param>
    public void ToBank(params int[] items)
    {
        if (items == null || !items.Any(id => id > 0))
            return;

        List<ItemCategory> whiteList =
    [
        ItemCategory.Note,
        ItemCategory.Item,
        ItemCategory.Resource,
        ItemCategory.QuestItem,
    ];

        foreach (int itemID in items)
        {
            if (itemID <= 0
                || Extras.Contains(itemID)
                || Bot.Inventory.IsEquipped(itemID)
                || (Bot.House != null && Bot.House.IsEquipped(itemID)))
                continue;

            ItemBase? inventoryItem = Bot
                .Inventory.Items.Concat(Bot.House?.Items ?? Enumerable.Empty<ItemBase>())
                .FirstOrDefault(x => x?.ID == itemID);

            if (inventoryItem == null)
                continue;

            // Coin (AC) items have infinite bank space, only block non-coin items when bank is full
            if (!inventoryItem.Coins && (Bot.Bank?.FreeSlots ?? 0) <= 0)
            {
                Logger($"🏦 Bank is full, skipping {inventoryItem.Name ?? $"ID: {itemID}"}.");
                continue;
            }

            if (
                !whiteList.Contains(inventoryItem.Category)
                && !inventoryItem.Coins
                || BankingBlackList.Contains(inventoryItem.Name)
            )
            {
                Logger($"⛔ {inventoryItem.Name ?? $"ID: {itemID}"} is blacklisted or excluded.");
                continue;
            }

            bool itemIsForHouse =
                Bot.House?.Items?.Any(x =>
                    x?.ID == itemID
                    && (
                        x.CategoryString == "House"
                        || x.CategoryString == "Wall Item"
                        || x.CategoryString == "Floor Item"
                    )
                ) ?? false;

            if (!itemIsForHouse)
            {
                JumpWait();
                bool success = false;
                for (int attempt = 0; attempt < 20; attempt++)
                {
                    Bot.Inventory.EnsureToBank(itemID);
                    if (Bot.Bank?.Contains(itemID) == true)
                    {
                        success = true;
                        break;
                    }
                    if (attempt == 10)
                        Logger($"⏳ Still trying to bank {inventoryItem.Name ?? $"ID: {itemID}"}, attempt {attempt + 1}/20...");
                    Sleep();
                }
                if (success)
                    Logger($"💰 {inventoryItem.Name ?? $"ID: {itemID}"} moved to bank! 🎒➡️🏦");
                else
                    Logger($"🚫 Failed to bank {inventoryItem.Name ?? $"ID: {itemID}"} after 20 attempts.");
            }
            else
            {
                InventoryItem? houseItem = Bot.House?.Items?.FirstOrDefault(x => x?.ID == itemID);
                if (houseItem == null)
                    continue;

                if ((Bot.House?.FreeSlots ?? 0) <= 0)
                {
                    Logger($"🏠 House storage full, skipping {inventoryItem.Name ?? $"ID: {itemID}"}.");
                    continue;
                }

                JumpWait();
                SendPackets($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{houseItem.ID}%{houseItem.CharItemID}%");
                Bot.Wait.ForTrue(() => !(Bot.House?.Contains(itemID) ?? true), 20);

                if (Bot.House?.Items?.Any(x => x?.ID == itemID) == true)
                    Logger($"🚫 Failed to bank {inventoryItem.Name ?? $"ID: {itemID}"} from house storage.");
                else
                    Logger($"🏠💰 {inventoryItem.Name ?? $"ID: {itemID}"} moved to bank from house storage! 🎒➡️🏦");
            }
        }
    }

    /// <summary>
    /// Transfers specified items by name from house inventory to house bank.
    /// Skips equipped items, restricted classes, or nonexistent items.
    /// </summary>
    /// <param name="items">Item names to move to house bank.</param>
    public void ToHouseBank(params string[] items)
    {
        if (items == null || !items.Any(name => !string.IsNullOrEmpty(name)))
            return;

        JumpWait();

        foreach (string? item in items)
        {
            if ((Bot.House?.FreeSlots ?? 0) <= 0 || string.IsNullOrEmpty(item) || item == SoloClass || item == FarmClass)
                continue;

            bool itemExists = Bot.House?.Items?.Any(x =>
                x?.Name == item && (!x.Coins || !x.Equipped)
            ) ?? false;
            if (!itemExists)
                continue;

            if (Bot.House?.Contains(item) ?? false)
            {
                if (!(Bot.House?.EnsureToBank(item) ?? false))
                {
                    Logger($"🚫 Failed to bank {item}, skipping it.");
                    continue;
                }

                Logger($"🏠💰 {item} moved to house bank successfully!");
            }
        }
    }

    /// <summary>
    /// Transfers specified items by ID from house inventory to house bank.
    /// Skips equipped items or nonexistent items.
    /// </summary>
    /// <param name="items">Item IDs to move to house bank.</param>
    public void ToHouseBank(params int[] items)
    {
        if (items == null || !items.Any(id => id > 0))
            return;

        JumpWait();

        foreach (int itemID in items)
        {
            if (itemID == 0 || (Bot.House?.FreeSlots ?? 0) <= 0)
                continue;

            bool itemExists = Bot.House?.Items?.Any(x => x?.ID == itemID && (!x.Equipped && x.Coins)) ?? false;
            if (!itemExists)
                continue;

            if (Bot.House?.Contains(itemID) ?? false)
            {
                if (!(Bot.House?.EnsureToBank(itemID) ?? false))
                {
                    Logger($"🚫 Failed to bank {itemID}, skipping it.");
                    continue;
                }

                Logger($"🏠💰 Item ID {itemID} moved to house bank successfully!");
            }
        }
    }

    public List<string> CategoryStrings = ["House", "Wall Item", "Floor Item"];

    /// <summary>
    /// Buys a item till you have the desired quantity
    /// </summary>
    /// <param name="map">Map of the shop</param>
    /// <param name="shopID">ID of the shop</param>
    /// <param name="itemName">Name of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="shopItemID">Use this for Merge shops that has 2 or more of the item with the same name and you need the second/third/etc., be aware that it will re-log you after to prevent ghost buy. To get the ShopItemID use the built in loader of Skua</param>
    /// <param name="Log"></param>
    // public void BuyItem(string map, int shopID, string itemName, int quant = 1, int shopItemID = 0, bool Log = true)
    // {
    //     _CheckInventorySpace();

    //     ShopItem? item = parseShopItem(
    //         GetShopItems(map, shopID),
    //         shopID,
    //         itemName,
    //         shopItemID
    //     );

    //     if (item == null)
    //     {
    //         Logger(
    //             $"Failed to find the item '{itemName}' in the shop with ID {shopID}, skipping it."
    //         );
    //         return;
    //     }

    //     if (!string.IsNullOrEmpty(item.CategoryString)
    //         && CategoryStrings.Contains(item.CategoryString))
    //     {
    //         _CheckHouseSpace();
    //     }

    //     _BuyItem(map, shopID, item, quant, Log);
    // }
    public void BuyItem(string map, int shopID, string itemName, int quant = 1, int shopItemID = 0, int index = 0, bool Log = true)
    {
        _CheckInventorySpace();
        ShopItem? item = parseShopItem(
            GetShopItems(map, shopID),
            shopID,
            itemName,
            shopItemID
        );
        if (item == null)
        {
            Logger(
                $"Failed to find the item '{itemName}' in the shop with ID {shopID}, skipping it."
            );
            return;
        }
        if (!string.IsNullOrEmpty(item.CategoryString)
            && CategoryStrings.Contains(item.CategoryString))
        {
            _CheckHouseSpace();
        }
        _BuyItem(map, shopID, item, quant, index, Log);
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
    public void BuyItem(string map, int shopID, int itemID, int quant = 1, int shopItemID = 0, int index = 0, bool Log = true)
    {
        if (CheckInventory(itemID, quant))
            return;

        _CheckInventorySpace();

        ShopItem? item = parseShopItem(
            GetShopItems(map, shopID),
            shopID,
            itemID,
            shopItemID
        );

        if (item == null)
        {
            Logger(
                $"Failed to find the item with ID {itemID} in the shop with ID {shopID}, skipping it"
            );
            return;
        }

        if (!string.IsNullOrEmpty(item.CategoryString)
            && CategoryStrings.Contains(item.CategoryString))
        {
            _CheckHouseSpace();
        }

        _BuyItem(map, shopID, item, quant, index, Log);
    }

    int retrys = 0;

    public void _BuyItem(string map, int shopID, ShopItem? item, int quant, int index = 0, bool Log = true)
    {
        #region IgnoreMe

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.AttackWithoutTarget = false;

        int buy_quant;
        int StaticQuant = quant;
        if (
            item == null
            || (buy_quant = _CalcBuyQuantity(item, quant)) <= 0
            || !_canBuy(shopID, item, quant)
        )
            return;

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

        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            if (Bot.Shops.ID == shopID || retry == 20)
                break;
            else
                retry++;
        }
        retry = 0;

        #endregion IgnoreMe

        // Resolve the correct ShopItem by index when multiple matches exist
        List<ShopItem> matches = Bot.Shops.Items
            .Where(x => x.ID == item.ID)
            .ToList();
        ShopItem? resolvedItem = matches.Count > index ? matches[index] : matches.FirstOrDefault();
        if (resolvedItem == null)
        {
            Logger($"Failed to find {item.Name} at index {index} in shop {shopID}.", "BuyItem");
            return;
        }
        item = resolvedItem;

        dynamic sItem = new ExpandoObject();
        bool succeeded = false;
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
                succeeded = true;
                break;
            }
            catch
            {
                Sleep(1000);
            }
        }

        if (!succeeded)
        {
            Logger("BuyItem Failed, crashed 5 times", stopBot: true);
            return;
        }
        Sleep(1000);

        Bot.Wait.ForActionCooldown(GameActions.BuyItem);
        Bot.Flash.CallGameFunction(
            "world.sendBuyItemRequestWithQuantity",
            JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(sItem))!
        );
        Bot.Wait.ForTrue(
            () => Bot.Inventory.Contains(item.ID, quant) || Bot.Bank.Contains(item.ID, quant),
            20
        );
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
                Logger(
                    $"Failed at buying {(buy_quant == 302500 ? 1 : buy_quant)} {item.Name}, retrying: x{retrys}",
                    "BuyItem"
                );
                retrys++;
                JumpWait();
                _BuyItem(map, shopID, item, quant, index, Log);
            }
            else
            {
                retrys = 0;
                Logger(
                    $"Failed at buying {(buy_quant == 302500 ? 1 : buy_quant)} {item.Name}",
                    "BuyItem"
                );
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
                        Relogin(
                            "Inventory de-sync (AE Issue) detected, relogging so the bot can continue"
                        );
                        if (Bot.Inventory.FreeSlots < 1)
                            Logger(
                                $"Inventory Slots: {Bot.Inventory.UsedSlots}/{Bot.Inventory.Slots}, Free: {Bot.Inventory.FreeSlots}. Clean your inventory... stopping",
                                stopBot: true
                            );
                        break;

                    case "Quest Complete Failed: Missing Required Item":
                        Relogin(
                            "Quest de-sync (AE Issue) detected, relogging so the bot can continue"
                        );
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
                    if (
                        i == null
                        || i!.ItemID == null
                        || i!.ItemID != itemID
                        || (
                            shopItemID != 0
                                ? (i!.ShopItemID == null || i!.ShopItemID != shopItemID)
                                : false
                        )
                    )
                        continue;
                    return i!;
                }
            }
            Logger(
                $"Failed to find the shopItemData for itemID {itemID} in {shopID}"
                    + reinstallCleanFlash,
                "BuyItem"
            );
            return null!;
        }

        bool _canBuy(int shopID, ShopItem? item, int buy_quant)
        {
            if (item == null)
                return false;

            if (!HasSpace && !CheckInventory(item.ID, toInv: false))
            {
                // Attempt to bank something
                BankACMisc(1);
                // Recheck for space
                if (!HasSpace)
                    return false;
            }

            //Achievement Check
            int achievementID = Bot.Flash.GetGameObject<int>("world.shopinfo.iIndex");
            string? io = Bot.Flash.GetGameObject<string>("world.shopinfo.sField");
            if (achievementID > 0 && io != null && !HasAchievement(achievementID, io))
            {
                Logger(
                    $"Cannot buy {item.Name} from {shopID} because you dont have achievement {achievementID} of category {io}.",
                    "CanBuy"
                );
                return false;
            }

            //Member Check
            if (item.Upgrade && !Bot.Player.IsMember)
            {
                Logger(
                    $"Cannot buy {item.Name} from {shopID} because you aren't a member.",
                    "CanBuy"
                );
                return false;
            }

            //Required-Item Check
            int reqItemID = Bot.Flash.GetGameObject<int>("world.shopinfo.reqItems");
            if (reqItemID > 0 && !CheckInventory(reqItemID))
            {
                Logger(
                    $"Cannot buy {item.Name} from {shopID} because you dont have the requiered item needed to buy stuff from the shop, itemID: {reqItemID}",
                    "CanBuy"
                );
                return false;
            }

            // Quest Check
            string? questName = Bot
                .Flash.GetGameObject<List<dynamic>>("world.shopinfo.items")
                ?.Find(d => d.ItemID == item.ID)
                ?.sQuest;
            if (!string.IsNullOrEmpty(questName))
            {
                List<QuestData>? v = JsonConvert.DeserializeObject<List<QuestData>?>(
                    File.ReadAllText(ClientFileSources.SkuaQuestsFile)
                );
                if (v != null)
                {
                    List<int> ids = v.Where(x => x.Name == questName).Select(q => q.ID).ToList();
                    List<int> incompleteIDs = [.. ids.Where(q => !isCompletedBefore(q))];
                    if (incompleteIDs.Any())
                    {
                        List<Quest>? quests = InitializeWithRetries(() =>
                            EnsureLoad(incompleteIDs.ToArray())
                        );
                        if (quests != null && quests.Any())
                        {
                            string questList = string.Join(" | ", quests.Select(q => $"[{q.ID}]"));
                            bool one = quests.Count == 1;
                            Logger(
                                $"Cannot buy {item.Name} from {shopID} because you haven't completed {(one ? "" : "one of ")}the following quest{(one ? "" : "s")}: \"{questName}\" {questList}",
                                "CanBuy"
                            );
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
                    Logger(
                        $"Cannot buy {item.Name} from {shopID} because you dont have rank {reqRank} {item.Faction}.",
                        "CanBuy"
                    );
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
                            Logger(
                                $"Cannot buy {req.Name} from {shopID}.\n"
                                    + $"You own {Bot.Inventory.GetQuantity(req.ID)}x {req.Name}.\n"
                                    + $"You need {total_quant}.",
                                "CanBuy"
                            );

                            return false;
                        }
                        Logger(
                            $"Cannot buy {item.Name} from {shopID} because {req.Name} is missing.",
                            "CanBuy"
                        );
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
                        Logger(
                            $"You own {Bot.Inventory.GetQuantity(item.ID)}x {item.Name}.",
                            "CanBuy"
                        );
                        Logger(
                            $"You need {Bot.Inventory.GetQuantity(item.ID) + buy_count}.",
                            "CanBuy"
                        );
                        Logger(
                            $"You are missing {total_gold_cost - Bot.Player.Gold} gold to buy enough.",
                            "CanBuy"
                        );
                        return false;
                    }
                }
                //AC costing check
                else
                {
                    int total_ac_cost = buy_count * item.Cost;
                    if (
                        Bot.ShowMessageBox(
                            $"The bot is about to buy \"{item.Name}\" {buy_count} times, which costs {total_ac_cost} AC, do you accept this?",
                            "Warning: Costs AC!",
                            true
                        ) != true
                    )
                    {
                        Logger(
                            $"Cannot buy {item.Name} from {shopID} because you didn't allow the bot to buy the item",
                            "CanBuy"
                        );
                        return false;
                    }
                    else if (
                        Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins")
                        < total_ac_cost
                    )
                    {
                        Logger(
                            $"Cannot buy {item.Name} from {shopID} because you are missing {Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins") - total_ac_cost} ACs",
                            "CanBuy"
                        );
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
    /// <param name="item">The shop item to check. If <c>null</c>, returns 0.</param>
    /// <returns>
    /// The maximum quantity of the item that can be purchased, or 0 if the item is not found or an error occurs.
    /// </returns>
    /// <remarks>
    /// Joins the map, ensures the player is out of combat, loads the shop, and retrieves the item's maximum buy quantity.
    /// </remarks>
    public int MaxBuyQuant(string map, int shopID, ShopItem? item)
    {
        if (item == null)
            return 0;

        Join(map);
        Bot.Wait.ForMapLoad(map);

        // Wait until out of combat, cancelling any targets
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            JumpWait();
            Sleep();
        }

        // Load the specified shop with retries
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != shopID && retry < 20)
        {
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
            Sleep(1000);
            retry++;
        }
        if (Bot.Shops.ID != shopID)
        {
            Logger($"Failed to load shop {shopID} after {retry} attempts.", "MaxBuyQuant");
            return 0;
        }

        // Attempt to fetch shop item data with retries
        dynamic? sItem = null;
        for (int i = 0; i < 5; i++)
        {
            sItem = InitializeWithRetries(() => GetShopItemData(item.ID, item.ShopItemID));
            if (sItem != null)
                break;
            Sleep(1000);
        }

        if (sItem == null)
        {
            Logger(
                $"Failed to load shop item data for ItemID {item.ID} in ShopID {shopID}.",
                "MaxBuyQuant"
            );
            return 0;
        }

        Sleep(1000); // Prevent potential race conditions

        Bot.Wait.ForActionCooldown(GameActions.BuyItem);

        // Deserialize and call Flash function to get max buy quantity
        return Bot.Flash.CallGameFunction<int>(
            "world.maximumShopBuys",
            JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(sItem))!
        );
    }

    /// <summary>
    /// Fetches the shop item data from the game's shop information.
    /// </summary>
    /// <param name="itemID">The item ID to search for.</param>
    /// <param name="shopItemID">Optional shop item ID to match (default 0 means ignore).</param>
    /// <returns>The dynamic shop item data if found; otherwise, null.</returns>
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
            if (item?.ItemID == itemID && (shopItemID == 0 || item?.ShopItemID == shopItemID))
                return item;
        }

        Logger(
            $"Shop item data not found for ItemID {itemID}, ShopItemID {shopItemID}.",
            "GetShopItemData"
        );
        return null;
    }

    private void _CheckInventorySpace()
    {
        if (Bot.Inventory.Slots != 0 && Bot.Inventory.FreeSlots <= 0)
        {
            int usedBefore = Bot.Inventory.UsedSlots;
            Logger(
                $"Your inventory is full [{usedBefore}/{Bot.Inventory.Slots}]. Attempting to bank AC-tagged misc items...",
                "_CheckInventorySpace"
            );

            BankACMisc();

            int usedAfter = Bot.Inventory.UsedSlots;
            int freed = usedBefore - usedAfter;

            if (Bot.Inventory.FreeSlots <= 0)
            {
                Logger(
                    $"Banked {freed} item{(freed != 1 ? "s" : "")} but your inventory is still full. Please clear space manually. Stopping the bot.",
                    "_CheckInventorySpace",
                    stopBot: true
                );
            }
            else
            {
                Logger(
                    $"Banked {freed} item{(freed != 1 ? "s" : "")}. {Bot.Inventory.FreeSlots} slot{(Bot.Inventory.FreeSlots != 1 ? "s" : "")} now available.",
                    "_CheckInventorySpace"
                );
            }
        }
    }

    private void _CheckHouseSpace()
    {
        if (Bot.House.Slots != 0 && Bot.House.FreeSlots <= 0)
        {
            int usedBefore = Bot.House.UsedSlots;
            Logger(
                $"Your house inventory is full [{usedBefore}/{Bot.House.Slots}]. Attempting to bank AC-tagged misc items...",
                "_CheckHouseSpace"
            );

            BankACHouseItems();

            int usedAfter = Bot.House.UsedSlots;
            int freed = usedBefore - usedAfter;

            if (Bot.House.FreeSlots <= 0)
            {
                Logger(
                    $"Banked {freed} item{(freed != 1 ? "s" : "")} but your house is still full. Please clear space manually. Stopping the bot.",
                    "_CheckHouseSpace",
                    stopBot: true
                );
            }
            else
            {
                Logger(
                    $"Banked {freed} item{(freed != 1 ? "s" : "")}. {Bot.House.FreeSlots} slot{(Bot.House.FreeSlots != 1 ? "s" : "")} now available.",
                    "_CheckHouseSpace"
                );
            }
        }
    }

    public int _CalcBuyQuantity(ShopItem item, int requestedAmount)
    {
        if (requestedAmount > item.MaxStack)
        {
            Logger(
                $"Requested {requestedAmount}, but max stack for {item.Name} is {item.MaxStack}. Fix the calling script.",
                "BuyItem"
            );
            Bot.StopSync(true);
        }

        if (Bot.Bank.Contains(item.ID) && !Bot.Inventory.Contains(item.ID))
            Unbank(item.ID);

        int itemStackSize = item.Quantity > 0 ? item.Quantity : 1;

        int currentStock =
            Bot.Inventory.Items.Concat(Bot.Bank.Items)
                .Concat(Bot.House.Items)
                .Concat(Bot.TempInv.Items)
                .Where(x => x.ID == item.ID)
                .Sum(x => x.Quantity);

        int stacksNeeded =
            (int)Math.Ceiling((double)requestedAmount / itemStackSize);

        foreach (var req in item.Requirements)
        {
            int totalNeeded = stacksNeeded * req.Quantity;

            int reqCurrent =
                Bot.Inventory.Items.Concat(Bot.Bank.Items)
                    .Concat(Bot.House.Items)
                    .Concat(Bot.TempInv.Items)
                    .Where(x => x.ID == req.ID)
                    .Sum(x => x.Quantity);

            if (reqCurrent < totalNeeded)
            {
                Logger(
                    $"Missing {req.Name} ({reqCurrent}/{totalNeeded}). Cannot proceed with purchase."
                );
                return 0;
            }
        }

        int buyAmount = stacksNeeded * itemStackSize;

        int maxCanBuy = item.MaxStack - currentStock;
        buyAmount = Math.Min(buyAmount, maxCanBuy - (maxCanBuy % itemStackSize));

        if (buyAmount <= 0)
        {
            Logger($"Cannot buy more {item.Name}, max stack reached ({currentStock}/{item.MaxStack}).");
            return 0;
        }

        return buyAmount;
    }

    private static readonly (int Rank, int RepRequired)[] RepThresholds =
    [
        (1, 0),
        (2, 900),
        (3, 3600),
        (4, 10000),
        (5, 22500),
        (6, 44100),
        (7, 78400),
        (8, 129600),
        (9, 202500),
        (10, 302500)
    ];

    public int PointsToLevel(int points)
    {
        foreach ((int rank, int req) in RepThresholds)
            if (points <= req)
                return rank;

        return 10; // max rank if somehow above cap
    }

    /// <summary>
    /// Sells a item till you have the desired quantity
    /// </summary>
    /// <param name="itemName">Name of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="all">Set to true if you wish to sell all the items</param>
    public void SellItem(string itemName, int quant = 1, bool all = false)
    {
        if (
            !(quant > 0 ? CheckInventory(itemName, quant) : CheckInventory(itemName))
            || !Bot.Inventory.TryGetItem(itemName, out InventoryItem? item)
        )
            return;

        InventoryItem? Item = null;
        for (int i = 0; i < 5; i++)
        {
            Item = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x => x != null && x.Name == itemName);
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

        JumpWait();

        // Inv quant >= current quantity.
        Bot.Wait.ForActionCooldown(GameActions.SellItem);
        Bot.Send.Packet(
            $"%xt%zm%sellItem%{Bot.Map.RoomID}%{item.ID}%{sell_count}%{item.CharItemID}%"
        );
        Bot.Wait.ForItemSell();
        Sleep();

        if (
            !all
            && (
                Bot.Inventory.Contains(itemName)
                || Bot.Inventory.GetQuantity(itemName) == QuantAfterSale
            )
        )
        {
            if (Bot.Inventory.GetQuantity(itemName) == QuantAfterSale)
                Logger($"Sold x{sell_count} \"{itemName}\"");
            {
                return;
            }
        }
        else if (all && !CheckInventory(itemName))
        {
            Logger($"Sold all of \"{itemName}\"");
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
    /// Sells an item by ID till you have the desired quantity
    /// </summary>
    /// <param name="itemID">ID of the item</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="all">Set to true if you wish to sell all the items</param>
    public void SellItem(int itemID, int quant = 1, bool all = false)
    {
        InventoryItem? item = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(x => x != null && x.ID == itemID);

        if (item == null)
        {
            Logger($"Item with ID {itemID} not found.");
            return;
        }

        string itemName = item.Name;

        if (
            !(quant > 0 ? CheckInventory(itemName, quant) : CheckInventory(itemName))
        )
            return;

        InventoryItem? Item = null;
        for (int i = 0; i < 5; i++)
        {
            Item = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x => x != null && x.ID == itemID);
            if (Item != null)
                break;
            Logger($"Attempt {i + 1}: Item ID {itemID} not found. Retrying...");
            Sleep(1000);
        }

        if (Item == null)
        {
            Logger($"Item ID {itemID} not found after 5 attempts.");
            return;
        }

        if (Bot.Bank.Contains(itemName) && !Bot.Inventory.Contains(itemName))
            Unbank(itemName);

        int retryCount = 0;
        int sell_count = all ? Bot.Inventory.GetQuantity(itemName) : quant;
        int QuantAfterSale = Bot.Inventory.GetQuantity(itemName) - sell_count;

    Retry:
        JumpWait();
        Bot.Wait.ForActionCooldown(GameActions.SellItem);
        Bot.Send.Packet(
            $"%xt%zm%sellItem%{Bot.Map.RoomID}%{itemID}%{sell_count}%{Item.CharItemID}%"
        );
        Bot.Wait.ForItemSell();
        Sleep();

        if (
            !all
            && (
                Bot.Inventory.Contains(itemName)
                || Bot.Inventory.GetQuantity(itemName) == QuantAfterSale
            )
        )
        {
            if (Bot.Inventory.GetQuantity(itemName) == QuantAfterSale)
                Logger($"Sold x{sell_count} \"{itemName}\" (ID: {itemID})");
            return;
        }
        else if (all && !CheckInventory(itemName))
        {
            Logger($"Sold all of \"{itemName}\" (ID: {itemID})");
            return;
        }
        else
        {
            if (retryCount < 5)
            {
                retryCount++;
                Logger($"Item ID {itemID} failed to sell, retrying [Try x{retryCount}]");
                goto Retry;
            }
            else
            {
                Logger($"Item ID {itemID} failed to sell, retrying x{retryCount} times did not succeed");
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
    // public List<ShopItem> GetShopItems(string map, int shopID)
    // {
    //     // Ensure player is in map
    //     if (!Bot.Map.Name.Equals(map, StringComparison.OrdinalIgnoreCase))
    //     {
    //         Join(map);
    //         Bot.Wait.ForMapLoad(map);
    //     }

    //     int retry = 0;
    //     while (!Bot.ShouldExit && retry++ < 20)
    //     {
    //         if (Bot.Shops.IsLoaded && Bot.Shops.ID == shopID)
    //             break;

    //         Bot.Shops.Load(shopID);
    //         Bot.Wait.ForActionCooldown(GameActions.LoadShop);
    //         Bot.Wait.ForTrue(
    //             () => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID,
    //             20
    //         );

    //         Sleep(1000);
    //     }

    //     if (!Bot.Shops.IsLoaded || Bot.Shops.ID != shopID)
    //     {
    //         Logger($"Failed to load shop {shopID} in map {map}.");
    //         return new();
    //     }

    //     return Bot.Shops.Items;
    // }
    public List<ShopItem> GetShopItems(string map, int shopID)
    {
        if (!Bot.Map.Name.Equals(map, StringComparison.OrdinalIgnoreCase))
        {
            Join(map);
            Bot.Wait.ForMapLoad(map);
        }
        int retry = 0;
        while (!Bot.ShouldExit && retry++ < 20)
        {
            if (Bot.Shops.IsLoaded && Bot.Shops.ID == shopID)
                break;
            Bot.Shops.Load(shopID);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(
                () => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID,
                20
            );
            Sleep(1000);
        }
        if (!Bot.Shops.IsLoaded || Bot.Shops.ID != shopID)
        {
            Logger($"Failed to load shop {shopID} in map {map}.");
            return [];
        }

        // Wait for the popup label to confirm the shop UI is fully ready
        Bot.Wait.ForTrue(() =>
        {
            string label = Bot.Flash.GetGameObject("ui.mcPopup.currentLabel");
            string flashID = Bot.Flash.GetGameObject("world.shopinfo.ShopID");
            return (label == "Shop" || label == "MergeShop" || label == "HouseShop")
                && flashID == shopID.ToString();
        }, 20);

        Bot.Log($"Shop loaded: \"{Bot.Flash.GetGameObject("world.shopinfo.sName")}\" ({shopID})");
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
    public ShopItem? parseShopItem(List<ShopItem> shopItems, int shopID, string itemNameID, int shopItemID = 0)
    {
        if (shopItems.Count == 0)
        {
            Logger($"Shop {shopID} has no items loaded.");
            return null;
        }

        if (shopItemID > 0)
        {
            ShopItem? byId = shopItems.FirstOrDefault(x => x.ShopItemID == shopItemID);
            if (byId != null)
                return byId;
            else
                Logger($"Item with ShopItemID {shopItemID} not found in shop {shopID}. Using \"{itemNameID}\" string as backup method");

        }

        List<ShopItem> matches = shopItems
            .Where(x => x.Name.Equals(itemNameID, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            Logger($"Item '{itemNameID}' not found in shop {shopID}.");
            return null;
        }

        if (matches.Count > 1)
        {
            Logger(
                $"Multiple items found with the name '{itemNameID}' in shop {shopID}. Specify ShopItemID."
            );
            return null;
        }

        return matches[0];
    }


    public ShopItem? parseShopItem(List<ShopItem> shopItems, int shopID, int itemID, int shopItemID = 0)
    {
        if (shopItems?.Count == 0)
        {
            Logger($"Shop {shopID} has no items loaded.");
            return null;
        }

        // 1️⃣ Try ShopItemID first (merge shops / duplicates)
        if (shopItemID > 0)
        {
            ShopItem? bySID = shopItems.FirstOrDefault(x => x.ShopItemID == shopItemID);
            if (bySID != null)
                return bySID;
            else
                Logger($"ShopItemID {shopItemID} not found in shop {shopID}. Falling back to ItemID...");
        }

        // 2️⃣ Fallback to ItemID
        ShopItem? byItemID = shopItems.FirstOrDefault(x => x.ID == itemID);
        if (byItemID != null)
            return byItemID;

        Logger($"Item ID {itemID} not found in shop {shopID}.");
        return null;
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
    public void GhostItem(int ID,
        string name = "Ghost Item",
        int quantity = 1,
        bool temp = false,
        ItemCategory category = ItemCategory.Unknown,
        string? description = null,
        int level = 1,
        params (string, object)[] extraInfo
    )
    {
        if (
            temp
                ? (Bot.TempInv.Contains(ID) && Bot.TempInv.Contains(name))
                : (Bot.Inventory.Contains(ID) && Bot.Inventory.Contains(name))
        )
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
            ItemCategory.Gun or ItemCategory.HandGun or ItemCategory.Rifle or ItemCategory.Whip =>
                "iwgun",
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

        #endregion icon switch

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
                ? (
                    x.Category == ItemCategory.Sword
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
                    || x.Category == ItemCategory.Whip
                )
                : x.CategoryString == categoryString;

        // Find the item with the highest boost
        string? item =
            Bot.Inventory.Items.Concat(Bot.Bank.Items)
                .Where(x =>
                    x != null
                    && (!x.Upgrade || Bot.Player.IsMember) // Allow upgrade items if the player is a member
                    && categoryFilter(x)
                ) // Filter items by category
                .OrderByDescending(x => GetBoostFloat(x, boostTypeString)) // Sort items by boost value in descending order
                .FirstOrDefault() // Select the item with the highest boost
                ?.Name
            // If no item with a high boost is found, search for items with the specified category and equipped
            ?? Bot.Inventory.Items.Where(x => x != null && categoryFilter(x) && x.Equipped)
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
            item.Meta.Split(',').First(meta => meta.Contains(boostType)).Split(':').Last(),
            CultureInfo.InvariantCulture.NumberFormat
        );
    }

    /// <summary>
    /// Removes the specified items from players inventory (Banks AC items)
    /// </summary>
    /// <param name="items">Items to Trash/Bank</param>
    public void TrashCan(params string[] items)
    {
        while (!Bot.ShouldExit && (Bot.Player.InCombat || Bot.Player.HasTarget))
        {
            Bot.Combat.CancelTarget();
            Bot.Combat.Exit();
            Bot.Wait.ForCombatExit();
            JumpWait();
            Sleep();
        }

        foreach (string item in items)
        {
            if (
                !Bot.Inventory.TryGetItem(item, out InventoryItem? TrashItem)
                || TrashItem == null
                || TrashItem.Temp
            )
                continue;

            if (!TrashItem.Coins)
            {
                Bot.Send.Packet(
                    $"%xt%zm%removeItem%{Bot.Map.RoomID}%{TrashItem.ID}%{Bot.Player.ID}%{TrashItem.Quantity}%"
                );
                Sleep();
                Logger($"Trashed: {TrashItem.Name} x{TrashItem.Quantity}");
            }
            else
                ToBank(TrashItem.ID);
        }
    }

    #endregion Inventory, Bank and Shop

    #region Drops

    /// <summary>
    /// Adds drops to the pickup list, un-bank the items.
    /// </summary>
    /// <param name="items">Items to add</param>
    public void AddDrop(params string[] items)
    {
        if (items == null || items.Length == 0)
            return;

        Unbank(items);
        foreach (string item in items)
            Bot.Drops.Add(item);
    }


    /// <summary>
    /// Adds drops to the pickup list and un-banks the items.
    /// </summary>
    /// <param name="items">Items to add.</param>
    public void AddDrop(params int[] items)
    {
        if (items == null || items.Length == 0)
            return;

        Unbank(items);
        Bot.Drops.Add(items);
    }


    /// <summary>
    /// Removes drops from the pickup list.
    /// </summary>
    /// <param name="items">Items to remove</param>
    public void RemoveDrop(params string[] items)
    {
        if (items == null || items.Length == 0)
            return;

        string[] filteredItems = items
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct()
            .Where(item => Bot.Drops.ToPickup.Contains(item))
            .ToArray();

        if (filteredItems.Length == 0)
            return;

        Bot.Drops.Remove(filteredItems);
    }


    /// <summary>
    /// Removes drops from the pickup list.
    /// </summary>
    /// <param name="items">Items to remove</param>
    public void RemoveDrop(params int[] items)
    {
        if (items == null || items.Length == 0)
            return;

        int[] filteredItems = items
            .Distinct()
            .Where(id => Bot.Drops.ToPickupIDs.Contains(id))
            .ToArray();

        if (filteredItems.Length == 0)
            return;

        Bot.Drops.Remove(filteredItems);
    }


    #endregion Drops

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

        Dictionary<Quest, int> chooseQuests = [];
        Dictionary<Quest, int> nonChooseQuests = [];

        foreach (int questID in questIDs.Distinct())
        {
            Quest? q = InitializeWithRetries(() => EnsureLoad(questID));
            if (q == null)
            {
                Logger($"Failed to initialize quest with ID {questID}.");
                continue;
            }

            if (q.Upgrade && !Bot.Player.IsMember)
            {
                Logger($"Quest {questID} requires membership, but the player is not a member.");
                continue;
            }

            List<ItemBase> missingRequirements = q
                .AcceptRequirements.Where(x => x != null && !CheckInventory(x.ID))
                .ToList();
            if (missingRequirements.Any())
            {
                Logger(
                    $"Player is missing the following accept requirements for quest {questID}: {string.Join(", ", missingRequirements.Select(x => x.Name))}"
                );
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

            ItemBase[] requiredItems = q
           .AcceptRequirements.Where(x => !x.Temp)
           .Concat(q.Requirements.Where(x => !x.Temp))
           .Where(item => item != null && item.ID > 0)
           .ToArray();

            // Loop through the required items and add the Name if either the Name or the ID is not in CurrentDrops or ToPickupIDs
            requiredItems
                .ToList()
                .ForEach(item =>
                {
                    // Check if either the Name or ID is not in the drops or pickup list
                    if (
                        item != null
                        && (
                            !Bot.Drops.ToPickup.Contains(item.Name)
                            || !Bot.Drops.ToPickupIDs.Contains(item.ID)
                        )
                    )
                    {

                        // Add both ID and Name to the drop list if missing (ID is incase of duplicate names)
                        AddDrop(item.Name);
                        AddDrop(item.ID);
                    }
                });

            // Collect unique item IDs and unbank them in one call
            int[] itemsToUnbank = q
                .AcceptRequirements.Concat(q.Requirements)
                .Select(x => x.ID)
                .Distinct()
                .ToArray();

            Unbank(itemsToUnbank);
        }
        GC.Collect();

        questCTS = new();
        int i = 0;
        //no initializationwithretries in asyncs as init has sleeps in it.
        Task.Run(async () =>
        {
            while (!Bot.ShouldExit && !questCTS.IsCancellationRequested)
            {
                foreach (
                    Quest? quest in chooseQuests
                        .Keys.Concat(nonChooseQuests.Keys)
                        .Where(x =>
                            Bot.Quests.TryGetQuest(x.ID, out Quest? _quest) && _quest != null
                        )
                        .Distinct()
                        .ToList()
                )
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
                        Bot.Quests.EnsureLoad(quest!.ID);
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
                            Quest? activeQuest = Bot.Quests.Active.FirstOrDefault(q =>
                                q?.ID == quest.ID
                            );
                            if (activeQuest != null)
                            {
                                ItemBase? reward = activeQuest.Rewards.FirstOrDefault(r =>
                                    r != null && r.Quantity < r.MaxStack
                                );
                                rewardId = reward?.ID ?? -1;
                            }
                        }

                        // Ensure quest is loaded, and is entirely completable.

                        // Send the quest completion packet
                        Bot.Send.Packet(
                            $"%xt%zm%tryQuestComplete%{Bot.Map.RoomID}%{quest.ID}%{rewardId}%false%{(quest.Once || !string.IsNullOrEmpty(quest?.Field) ? 1 : Bot.Flash.CallGameFunction<int>("world.maximumQuestTurnIns", quest!.ID))}%wvz%"
                        );

                        // Check if the quest is still in progress
                        await Task.Delay(ActionDelay * 2);
                        if (Bot.Quests.IsInProgress(quest!.ID))
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

    // #region Will Require 1.3.1.1 and still requires work

    // /// <summary>
    // /// This will register quests to be completed while doing something else, i.e. while in combat.
    // /// If it has quests already registered, it will cancel them first and then register the new quests.
    // /// </summary>
    // /// <param name="quests">Tuples of (questID, rewardID) to be completed.</param>
    // public void RegisterQuests(params (int questId, int rewardId)[] quests)
    // {
    //     if (quests == null || quests.Length == 0)
    //         return;

    //     Dictionary<Quest, int> questRewards = new();

    //     foreach ((int questID, int rewardID) in quests)
    //     {
    //         Quest? q = InitializeWithRetries(() => EnsureLoad(questID));
    //         if (q == null)
    //         {
    //             Logger($"Failed to initialize quest with ID {questID}.");
    //             continue;
    //         }

    //         if (q.Upgrade && !Bot.Player.IsMember)
    //         {
    //             Logger($"Quest {questID} requires membership, but the player is not a member.");
    //             continue;
    //         }

    //         List<ItemBase> missingRequirements = q
    //             .AcceptRequirements.Where(x => x != null && !CheckInventory(x.ID))
    //             .ToList();
    //         if (missingRequirements.Any())
    //         {
    //             Logger(
    //                 $"Player is missing the following accept requirements for quest {questID}: {string.Join(", ", missingRequirements.Select(x => x.Name))}"
    //             );
    //             continue;
    //         }

    //         if (!questRewards.ContainsKey(q))
    //             questRewards.Add(q, rewardID == 0 ? -1 : rewardID);

    //         // Collect unique item IDs and unbank them in one call
    //         int[] itemsToUnbank = q
    //             .AcceptRequirements.Concat(q.Requirements)
    //             .Select(x => x.ID)
    //             .Distinct()
    //             .ToArray();

    //         Unbank(itemsToUnbank);
    //     }
    //     GC.Collect();

    //     var quesToRegister = questRewards
    //         .Where(kvp => !Bot.Quests.Registered.Contains(kvp.Key.ID))
    //         .ToList();

    //     foreach ((Quest Q, int rewardID) in quesToRegister)
    //     {
    //         Bot.Quests.RegisterQuests((Q.ID, rewardID));
    //     }
    // }

    // /// <summary>
    // /// Overload for registering quests with default reward ID (-1).
    // /// </summary>
    // /// <param name="questIDs">IDs of the quests to be completed.</param>
    // public void RegisterQuests(params int[] questIDs)
    // {
    //     if (questIDs == null || questIDs.Length == 0)
    //         return;

    //     var questTuples = questIDs.Select(id => (id, -1)).ToArray();
    //     RegisterQuests(questTuples);
    // }

    // #endregion

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

        if (QuestData.Upgrade && !Bot.Player.IsMember)
            Logger(
                $"\"{QuestData.Name}\" [{questID}] is member-only, stopping the bot.",
                stopBot: true
            );

        if (questID <= 0)
            return false;

        ItemBase[] requiredItems = QuestData
            .AcceptRequirements.Where(x => !x.Temp)
            .Concat(QuestData.Requirements.Where(x => !x.Temp))
            .Where(item => item != null && item.ID > 0)
            .ToArray();

        // Loop through the required items and add the Name if either the Name or the ID is not in CurrentDrops or ToPickupIDs
        requiredItems
            .ToList()
            .ForEach(item =>
            {
                // Check if either the Name or ID is not in the drops or pickup list
                if (
                    item != null
                    && (
                        !Bot.Drops.ToPickup.Contains(item.Name)
                        || !Bot.Drops.ToPickupIDs.Contains(item.ID)
                    )
                )
                {

                    // Add both ID and Name to the drop list if missing (ID is incase of duplicate names)
                    AddDrop(item.Name);
                    AddDrop(item.ID);
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
            questIDs = new int[] { 0 };

        List<Quest>? QuestData = InitializeWithRetries(() =>
            EnsureLoad(questIDs?.Where(q => q > 0).ToArray() ?? Array.Empty<int>())
        );

        if (QuestData == null)
        {
            Logger("Failed to load quests after multiple attempts.");
            return;
        }

        foreach (Quest quest in QuestData)
        {
            if (quest.Upgrade && !Bot.Player.IsMember)
                Logger(
                    $"\"{quest.Name}\" [{quest.ID}] is member-only, stopping the bot.",
                    stopBot: true
                );

            if (Bot.Quests.IsInProgress(quest.ID) || quest.ID <= 0)
                continue;

            ItemBase[] items = quest
                .AcceptRequirements.Concat(quest.Requirements)
                .Where(x => x != null && !x.Temp && x.ID > 0)
                .ToArray();

            foreach (ItemBase item in items)
            {
                if (!Bot.Drops.ToPickup.Contains(item.Name) || !Bot.Drops.ToPickupIDs.Contains(item.ID))
                {
                    AddDrop(item.Name);
                    AddDrop(item.ID);
                }

                if (!Bot.Inventory.Contains(item.ID))
                    Unbank(item.ID);
            }

            Sleep(ActionDelay * 2);
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
        if (
            questData != null
            && questData.Requirements != null
            && (
                !questData.Requirements.Any()
                || questData.Requirements.All(r => r != null && r.ID > 0)
                    && CheckInventory(questData.Requirements.Select(x => x.ID).ToArray())
                    && CheckInventory(questData.AcceptRequirements.Select(x => x.ID).ToArray())
            )
        )
        {
            if (itemID == -1 && questData.SimpleRewards.Any())
            {
                var availableReward = questData.SimpleRewards.FirstOrDefault(x =>
                    x.ID > 0 && !CheckInventory(x.ID, x.MaxStack, false)
                );

                itemID = availableReward?.ID ?? -1;

                if (
                    availableReward != null
                    && itemID != -1
                    && !Bot.Drops.ToPickupIDs.Contains(itemID)
                )
                    AddDrop(availableReward.ID);
                return Bot.Quests.EnsureComplete(questID, itemID);
            }
            else
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

            if (questID.Requirements != null
                && (!questID.Requirements.Any()
                    || questID.Requirements.All(r => r != null && r.ID > 0)
                        && CheckInventory(questID.Requirements.Select(x => x.ID).ToArray())
                )
            )
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

        if (itemList?.Length > 0)
            Bot.Drops.Add(itemList);

        // Check turnin requirements
        if (!quest.Requirements.All(req => req != null && CheckInventory(req.ID, req.Quantity)))
        {
            string missing = string.Join(
                ", ",
                quest
                    .Requirements.Where(req => req != null && !CheckInventory(req.ID, req.Quantity))
                    .Select(req => $"\"{req.Name}\"")
            );

            if (quest.Requirements.Any(x => Bot.Bank.Contains(x.ID)))
                Logger(
                    "Missing turnin requirements, and something is in the bank. Unbank it then restart."
                );
            else
                Logger($"Missing {missing}");

            return false;
        }

        bool hasAllRewardItems = true;
        bool questCompleted = false;

        // Filter rewards by provided list, unless null
        IEnumerable<ItemBase> rewards =
            itemList == null
                ? quest.Rewards
                : quest.Rewards.Where(item => itemList.Contains(item.Name));

        // -------------------------
        // FIX: If no reward matches, still complete the quest normally.
        // -------------------------
        if (!rewards.Any())
        {
            if (!Bot.Quests.EnsureComplete(questID))
            {
                Logger($"Failed to complete quest [{questID}] (no rewards matched).");
                return false;
            }

            Bot.Wait.ForQuestComplete(questID);
            return true;
        }

        // Handle actual reward selection
        foreach (ItemBase item in rewards)
        {
            if (CheckInventory(item.ID, item.MaxStack, false))
                continue;

            // Ensure inventory space
            if (!HasSpace && !CheckInventory(item.ID, toInv: false))
            {
                BankACMisc(1);
                if (!HasSpace)
                {
                    Logger($"Skipping \"{item.Name}\" from quest [{questID}] due to no space.");
                    continue;
                }
            }

            hasAllRewardItems = false;

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

        if (hasAllRewardItems)
        {
            Logger($"Quest [{questID}] not completed. All reward items already owned.");
            return false;
        }

        if (!questCompleted)
        {
            Logger(
                $"Could not complete quest [{questID}]. Some items may be missing or unavailable.\n"
                    + string.Join(
                        "\n",
                        quest
                            .Rewards.Where(x =>
                                x.Temp ? Bot.TempInv.Contains(x.ID) : !Bot.Inventory.Contains(x.ID)
                            )
                            .Select(x => $"\"{x.Name}\"")
                    )
            );
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
        if (itemID > 0)
            Bot.Drops.Add(itemID);

        if (quest != null && !Bot.Quests.IsInProgress(questID))
            EnsureAccept(questID);
        Bot.Wait.ForTrue(() => Bot.Quests.IsInProgress(questID), 20);

        int turnIns;
        if (quest != null)
        {
            string[] requiredItemNames = quest
                .Requirements.Concat(quest.AcceptRequirements)
                .Select(item => item.Name)
                .ToArray();

            if (quest.Once || !string.IsNullOrEmpty(quest.Field))
            {
                turnIns = 1;
            }
            else
            {
                int possibleTurnin = Bot.Flash.CallGameFunction<int>(
                    "world.maximumQuestTurnIns",
                    questID
                );
                turnIns = possibleTurnin > amount && amount > 0 ? amount : possibleTurnin;
                if (turnIns == 0)
                {
                    return 0;
                }
            }

            // Ensure quest is loaded, and is entirely completable.
            if (EnsureAccept(questID) && CheckInventory(requiredItemNames))
            {
                Bot.Flash.CallGameFunction(
                    "world.tryQuestComplete",
                    questID,
                    itemID,
                    false,
                    turnIns
                );
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
        if (questID <= 0)
        {
            Logger($"❌ Invalid QuestID: {questID}", "EnsureLoad", messageBox: false);
            return new Quest(); // safe empty quest
        }

        Quest? quest = Bot.Quests.Tree.FirstOrDefault(x => x.ID == questID)
                       ?? _TryLoadFromBot()
                       ?? EnsureLoadFromFile(questID).Result?.FirstOrDefault();

        if (quest == null)
        {
            Logger(
                $"[Warning] Failed to get the Quest Object for questID {questID}. The quest will be skipped.",
                "EnsureLoad A.0",
                messageBox: false
            );
            return new Quest();
        }

        return quest;

        Quest? _TryLoadFromBot()
        {
            Sleep();
            Bot.Wait.ForTrue(
                () => Bot.Quests.Tree.Any(q => q.ID == questID),
                () => Bot.Quests.Load(questID),
                20
            );
            return Bot.Quests.Tree.FirstOrDefault(q => q.ID == questID);
        }
    }

    public List<Quest> EnsureLoad(params int[] questIDs)
    {
        if (questIDs.Length == 0)
            return [];

        List<Quest> quests = Bot.Quests.Tree.Where(x => questIDs.Contains(x.ID)).ToList();
        if (quests.Count == questIDs.Length)
            return quests;

        List<int> missing = questIDs.Where(id => !quests.Any(q => q.ID == id)).ToList();

        Sleep();
        const int batchSize = 30;
        for (int i = 0; i < missing.Count; i += batchSize)
        {
            int take = Math.Min(batchSize, missing.Count - i);
            int[] batch = [.. missing.GetRange(i, take)];
            Bot.Quests.Load(batch);
            Sleep(1500);
        }

        Bot.Wait.ForTrue(() => questIDs.All(id => Bot.Quests.Tree.Any(q => q.ID == id)), 20);

        List<Quest> toReturn = Bot.Quests.Tree.Where(x => questIDs.Contains(x.ID)).ToList();

        if (toReturn.Count < questIDs.Length)
        {
            List<Quest>? fileQuests = EnsureLoadFromFile([.. missing]).Result;
            if (fileQuests != null)
                toReturn.AddRange(fileQuests.Where(q => !toReturn.Any(x => x.ID == q.ID)));

            if (toReturn.Count < questIDs.Length)
            {
                Logger(
                    $"[Warning] Failed to get the Quest Object for questIDs {string.Join(" | ", questIDs)}. These quests will be skipped.",
                    "EnsureLoad B.4",
                    messageBox: false
                );
                return [];
            }
        }

        return toReturn;
    }

    private static readonly HttpClient GitHubClient = new();
    private async Task<List<Quest>?> EnsureLoadFromFile(params int[] questIDs)
    {
        if (questIDs.Length == 0)
        {
            Logger("No QuestIDs provided to EnsureLoadFromFile", "EnsureLoad", messageBox: false);
            return null;
        }

        List<Quest>? toReturn = null;

        // Try local quest file if it's recent
        if (File.Exists(ClientFileSources.SkuaQuestsFile) &&
            DateTime.Now.Subtract(File.GetLastWriteTime(ClientFileSources.SkuaQuestsFile)).TotalDays < 14 &&
            LoadLocal())
            return toReturn;

        // Try GitHub as fallback
        try
        {
            List<Quest>? githubQuests = await LoadFromGithubWithTimeout();
            if (githubQuests != null && githubQuests.Count > 0)
                return githubQuests;
        }
        catch (Exception ex)
        {
            Logger($"Failed to load from GitHub: {ex.Message}", "EnsureLoad", messageBox: false);
        }

        // If GitHub fails, try local again even if outdated
        if (File.Exists(ClientFileSources.SkuaQuestsFile) && LoadLocal())
        {
            Logger("Using outdated local quest file as fallback", "EnsureLoad", messageBox: false);
            return toReturn;
        }

        // Attempt to update quest file manually
        try
        {
            await UpdateQuestFile();
            if (LoadLocal())
                return toReturn;
        }
        catch (Exception ex)
        {
            Logger($"Failed to update quest file: {ex.Message}", "EnsureLoad", messageBox: false);
        }

        // Last resort: try local one more time
        if (File.Exists(ClientFileSources.SkuaQuestsFile) && LoadLocal())
            return toReturn;

        // Complete failure
        Logger(
            $"[Warning] Failed to get the Quest Object for questIDs {string.Join(" | ", questIDs)}. GitHub is unreachable and no local quest data is available.",
            "EnsureLoad C.0",
            messageBox: false
        );
        return null;

        // Local loader
        bool LoadLocal()
        {
            try
            {
                LocalQuestsFile ??= JsonConvert.DeserializeObject<List<QuestData>?>(
                    File.ReadAllText(ClientFileSources.SkuaQuestsFile)
                );

                if (LocalQuestsFile == null || LocalQuestsFile.Count == 0)
                {
                    LocalQuestsFile = null;
                    return false;
                }

                Dictionary<int, QuestData> questMap = LocalQuestsFile
                    .GroupBy(q => q.ID)
                    .Select(g => g.First())
                    .ToDictionary(q => q.ID);

                List<Quest> result = [];
                foreach (int questID in questIDs.Distinct())
                {
                    if (!questMap.TryGetValue(questID, out QuestData? data))
                    {
                        Logger($"Quest ID {questID} missing from local quest file", "LoadLocal", messageBox: false);
                        continue; // skip missing quests
                    }
                    result.Add(ToQuest(data));
                }

                if (result.Count == 0)
                    return false;

                toReturn = result;
                return true;
            }
            catch (Exception ex)
            {
                Logger($"Failed to load local quest file: {ex.Message}", "LoadLocal", messageBox: false);
                return false;
            }
        }

        // GitHub loader
        async Task<List<Quest>?> LoadFromGithubWithTimeout()
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
            HttpClient client = GitHubClient;

            try
            {
                string response = await client.GetStringAsync(
                    "https://raw.githubusercontent.com/auqw/Scripts/Skua/QuestData.json",
                    cts.Token
                );

                OnlineQuestsFile = JsonConvert.DeserializeObject<List<QuestData>?>(response);
                if (OnlineQuestsFile == null || OnlineQuestsFile.Count == 0)
                    return null;

                Dictionary<int, QuestData> questMap = OnlineQuestsFile
                    .GroupBy(q => q.ID)
                    .Select(g => g.First())
                    .ToDictionary(q => q.ID);

                List<Quest> result = [];
                foreach (int questID in questIDs.Distinct())
                {
                    if (!questMap.TryGetValue(questID, out QuestData? data))
                        continue; // skip missing quests

                    result.Add(ToQuest(data));
                }

                return result.Count > 0 ? result : null;
            }
            catch (TaskCanceledException)
            {
                Logger("GitHub request timed out after 30 seconds", "EnsureLoad", messageBox: false);
                return null;
            }
            catch (Exception ex)
            {
                Logger($"GitHub load failed: {ex.Message}", "EnsureLoad", messageBox: false);
                return null;
            }
        }

        Quest ToQuest(QuestData data)
        {
            return new Quest()
            {
                ID = data.ID,
                Slot = data.Slot,
                Value = data.Value,
                Name = data.Name ?? $"Quest {data.ID}",
                Description = string.Empty,
                EndText = string.Empty,
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
                Status = null!,
                Rewards = data.Rewards ?? new(),
                SimpleRewards = data.SimpleRewards ?? new(),
            };
        }

        async Task UpdateQuestFile()
        {
            using CancellationTokenSource cts = new(TimeSpan.FromMinutes(2));
            try
            {
                List<QuestData> questData = await (LoaderService ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>())
                    .UpdateAsync("QuestData.json", false, null, cts.Token);

                LocalQuestsFile = questData;
            }
            finally
            {
                cts.Dispose();
            }
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
    /// <param name="questIDs">The quest IDs for which to retrieve the reward names.</param>
    /// <returns>An array of reward names (strings) for the specified quest IDs.</returns>
    public string[] QuestRewards(params int[] questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
            return Array.Empty<string>();

        List<string> toReturn = [];

        if (questIDs.Length <= 15)
        {
            InitializeWithRetries(() => EnsureLoad(questIDs));
            toReturn.AddRange(
                Bot?.Quests?.Tree?.Where(q => questIDs.Contains(q.ID))
                    .SelectMany(q =>
                        q?.Rewards?.Where(r => r != null).Select(r => r!.Name)
                        ?? Enumerable.Empty<string>()
                    ) ?? Enumerable.Empty<string>()
            );
        }
        else
        {
            List<Quest>? quests = InitializeWithRetries(() => EnsureLoad(questIDs));
            if (quests == null)
            {
                Logger(
                    $"Failed to load quests with IDs: {string.Join(", ", questIDs)}",
                    "QuestRewards"
                );
                return Array.Empty<string>();
            }

            toReturn.AddRange(
                quests
                    .Where(q => q != null && q.Rewards?.Count > 0)
                    .SelectMany(q => q!.Rewards!.Where(r => r != null).Select(r => r!.Name))
            );
        }

        return [.. toReturn];
    }

    /// <summary>
    /// Retrieves the quest reward IDs for the specified quest IDs.
    /// </summary>
    /// <param name="questIDs">The quest IDs for which to retrieve the reward IDs.</param>
    /// <returns>An array of reward IDs (integers) for the specified quest IDs.</returns>
    public int[] QuestRewardsInt(params int[] questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
            return Array.Empty<int>();

        List<int> toReturn = [];

        foreach (Quest? q in EnsureLoad(questIDs) ?? Enumerable.Empty<Quest>())
        {
            if (q?.Rewards == null || q.Rewards.Count == 0)
                continue;

            toReturn.AddRange(q.Rewards.Where(r => r != null).Select(r => r!.ID));
        }

        return [.. toReturn];
    }

    /// <summary>
    /// Retrieves the quest requirements for the specified quest IDs, based on the type parameter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result. Can be either <see cref="string"/> or <see cref="int"/>.
    /// </typeparam>
    /// <param name="questIDs">The quest IDs for which to retrieve the requirements.</param>
    /// <returns>
    /// An array of the specified type containing the quest requirements.
    /// For <see cref="string"/>, it returns an array of the requirement names.
    /// For <see cref="int"/>, it returns an array of the requirement IDs.
    /// </returns>
    public T[] QuestRequirements<T>(params int[] questIDs)
    {
        if (questIDs == null || questIDs.Length == 0)
            return Array.Empty<T>();

        List<T> toReturn = [];

        foreach (Quest? q in EnsureLoad(questIDs) ?? Enumerable.Empty<Quest>())
        {
            if (q?.Requirements == null || q.Requirements.Count == 0)
                continue;

            if (typeof(T) == typeof(string))
            {
                toReturn.AddRange(
                    q.Requirements.Where(r => r != null).Select(r => (T)(object)r!.Name)
                );
            }
            else if (typeof(T) == typeof(int))
            {
                toReturn.AddRange(
                    q.Requirements.Where(r => r != null).Select(r => (T)(object)r!.ID)
                );
            }
        }

        return [.. toReturn];
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

        ItemBase? Item = Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(x => x != null && x.ID == itemID);

        // EnsureAccept(questID);
        // Sleep();
        EnsureCompleteMulti(questID, itemID: itemID);
    }

    /// <summary>
    /// Checks if a quest has been completed before.
    /// </summary>
    /// <param name="QuestID">The ID of the quest to check.</param>
    /// <returns>True if the quest is completed, otherwise false.</returns>
    public bool isCompletedBefore(int QuestID, bool log = true)
    {
        if (QuestID <= 0)
            return false;

        Quest? quest = InitializeWithRetries(() => EnsureLoad(QuestID));
        if (quest == null)
        {
            Logger($"❌ Failed to initialize quest {QuestID} after multiple attempts.");
            return false;
        }

        string questName = quest.Name ?? $"{QuestID}";

        bool CheckCompletion(Quest? QuestData)
        {
            if (QuestData == null)
            {
                Logger($"❌ Quest data for {questName} [{QuestID}] is null.");
                return false;
            }
            bool complete =
                QuestData.Slot < 0
                || Bot.Flash.CallGameFunction<int>("world.getQuestValue", QuestData.Slot)
                    >= QuestData.Value;

            // Commented out to reduce spam
            if (log)
                Logger($"{questName} [{QuestID}] completion check [{(complete ? '✔' : '❌')}]");
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
                Logger(
                    $"❌ Failed to reinitialize {questName} [{QuestID}] after multiple attempts."
                );
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
        List<Quest>? questData = InitializeWithRetries(() => EnsureLoad(questIDs));
        if (questData == null || !questData.Any())
        {
            Logger("No quests found to register.");
            return;
        }
        Dictionary<Quest, int> chooseQuests = [];
        Dictionary<Quest, int> nonChooseQuests = [];

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
                            Logger(
                                $"Quest completed x{nonChooseQuests[kvp.Key] + amountTurnedIn} times: [{kvp.Key.ID}] \"{kvp.Key.Name}\""
                            );
                        }
                    }

                    // Quests that need a choice
                    foreach (KeyValuePair<Quest, int> kvp in chooseQuests)
                    {
                        if (Bot.Quests.CanComplete(kvp.Key.ID))
                        {
                            // Finding the next item that you dont have max stack of yet
                            List<SimpleReward> simpleRewards = kvp
                                .Key.SimpleRewards.Where(r =>
                                    r.Type == 2
                                    && (
                                        !Bot.Inventory.IsMaxStack(r.Name)
                                        || !(
                                            Bot.Bank.TryGetItem(r.Name, out InventoryItem? item)
                                            && item != null
                                            && item.Quantity >= r.MaxStack
                                        )
                                    )
                                )
                                .ToList();
                            if (simpleRewards.Count == 0)
                            {
                                EnsureCompleteOld(kvp.Key.ID);
                                await Task.Delay(ActionDelay * 2);
                                EnsureAcceptOld(kvp.Key.ID);
                                continue;
                            }

                            Bot.Drops.Add(
                                kvp.Key.Rewards.Where(x => simpleRewards.Any(t => t.ID == x.ID))
                                    .Select(i => i.Name)
                                    .ToArray()
                            );
                            EnsureCompleteOld(kvp.Key.ID, simpleRewards.First().ID);
                            await Task.Delay(ActionDelay * 2);
                            EnsureAcceptOld(kvp.Key.ID);
                            Logger(
                                $"Quest completed x{chooseQuests[kvp.Key]++} times: [{kvp.Key.ID} \"{kvp.Key.Name}\" (got {kvp.Key.Rewards.First(x => x.ID == simpleRewards.First().ID).Name}])"
                            );
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
        Quest? QuestData = InitializeWithRetries(() => EnsureLoad(questID));
        if (QuestData == null)
        {
            Logger($"Failed to load quest with ID {questID} after multiple attempts.");
            return false;
        }

        if (QuestData.Upgrade && !Bot.Player.IsMember)
            Logger(
                $"\"{QuestData.Name}\" [{questID}] is member-only, stopping the bot.",
                stopBot: true
            );

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
        List<Quest>? QuestData = InitializeWithRetries(() => EnsureLoad(questIDs));

        if (QuestData == null || !QuestData.Any())
        {
            Logger("No quests found to accept.");
            return;
        }
        foreach (Quest quest in QuestData)
        {
            if (quest.Upgrade && !Bot.Player.IsMember)
                Logger(
                    $"\"{quest.Name}\" [{quest.ID}] is member-only, stopping the bot.",
                    stopBot: true
                );

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
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questID));
        if (quest is not null)
        {
            foreach (ItemBase item in quest.Rewards)
            {
                if (
                    !CheckInventory(item.Name, toInv: false)
                    && (itemList == null || (itemList != null && itemList.Contains(item.Name)))
                )
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
        Logger(
            $"Could not complete the quest {questID}. Maybe all items are already in your inventory"
        );
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
        Quest? q = InitializeWithRetries(() => EnsureLoad(questID));
        if (q == null)
        {
            Logger($"Failed to load quest with ID {questID} after multiple attempts.");
            return 0;
        }

        int turnIns = 0;
        if (q.Once || !String.IsNullOrEmpty(q.Field))
            turnIns = 1;
        else
        {
            int possibleTurnin = Bot.Flash.CallGameFunction<int>(
                "world.maximumQuestTurnIns",
                questID
            );
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

    #endregion Quest

    #region Kill

    /// <summary>
    /// Joins a map, jump and set the spawn point and kills the specified monster
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="monster">Name of the monster to kill</param>
    /// <param name="item">Item to kill the monster for, if null will just kill the monster 1 time</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log">Whether it will log that it is killing the monster</param>
    /// <param name="publicRoom"></param>
    public void KillMonster(string map, string cell, string pad, string monster, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        if (Bot.Map.Name != map)
        {
            Join(map, publicRoom: publicRoom); // 🗺️
            Bot.Wait.ForMapLoad(map); // ⏳
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10); // 🔄
        }

        if (!Bot.Map.Cells.Any(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)))
            cell =
                Bot.Map.Cells.FirstOrDefault(c =>
                    c.Equals(cell, StringComparison.OrdinalIgnoreCase)
                )
                ?? cell; // 🔀

        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower()); // 📝

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad, autoCorrect: false); // ➡️
            Bot.Wait.ForCellChange(cell); // ⏳
        }

        Bot.Player.SetSpawnPoint(); // 📍

        if (item != null && !isTemp)
            AddDrop(item); // 💎

        Bot.Options.AggroAllMonsters = false; // ⚔️

        Monster? targetMonster = FindMonster(map: map, monster: monster); // 🕵️

        if (targetMonster == null)
        {
            Logger($"⚠️ Monster {monster} not found in cell {cell}, pad {pad} in /{map}");
            return;
        }

        if (item == null)
        {
            while (!Bot.ShouldExit)
            {
                if (!Bot.Player.Alive)
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20); // 💀➡️💖

                if (cell != null && Bot.Player.Cell != cell)
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false); // ➡️
                    Bot.Wait.ForCellChange(cell); // ⏳
                }
                if (
                    !Bot.Player.HasTarget
                    || Bot.Player.Target != null || Bot.Player?.Target?.HP > 0
                )
                    Bot.Combat.Attack(monster); // ⚔️

                Sleep(500); // 💤
                            // Check if player doenst have a target after the attacking
                            // if not then its dead and we can move on
                if (!Bot.Player!.HasTarget)
                    return;
            }
        }
        else
        {
            if (monster == "*")
                _KillForItem("*", item, quant, isTemp, log: log, cell: cell); // 🐺💎
            else
                _KillForItem(monster, item, quant, isTemp, log: log, cell: cell); // 🎯💎
        }

        Bot.Options.AttackWithoutTarget = false; // ⚔️❌
        Bot.Options.AggroAllMonsters = false; // ❌
        Bot.Options.AggroMonsters = false; // ❌

        string? targetCell =
            Bot.Map.Cells.Where(c =>
                    c != null
                    && !BlackListedJumptoCells.Contains(c)
                    && !Bot.Monsters.MapMonsters.Any(monster =>
                        monster != null && monster.Cell == c
                    )
                )
                .FirstOrDefault(c =>
                    c != null
                    && (
                        Bot.Map.Cells.Count(cell => cell.Contains("Enter")) > 1
                        || !c.Contains("Enter")
                    )
                )
            ?? "Enter";

        Bot.Map.Jump(targetCell, targetCell == "Enter" ? "Spawn" : "Left", autoCorrect: false); // ➡️
        Bot.Wait.ForCellChange(targetCell); // ⏳
        Sleep(); // 💤
        JumpWait(); // 🏃‍♂️
        Rest(); // 🛌
        Bot.Options.HidePlayers = false; // 👀
    }

    public void EquipBestClassForTargets(IEnumerable<Monster>? targets)
    {
        if (targets == null)
            return;

        bool hasHighHpTarget = false;
        bool hasLowHpTarget = false;

        // Scan targets for HP thresholds
        foreach (Monster? monster in targets)
        {
            if (monster == null)
                continue;

            int hp = monster.MaxHP;

            if (hp > 10000)
            {
                hasHighHpTarget = true;
                break; // Solo takes priority
            }

            if (hp < 10000)
                hasLowHpTarget = true;
        }

        // Solo priority
        if (hasHighHpTarget)
        {
            EquipClass(ClassType.Solo);
            return;
        }

        if (!hasLowHpTarget)
            return; // Nothing to farm

        // Count monsters by (Name + Cell) for farm decision
        Dictionary<(string Name, string Cell), int> monsterCounts = [];

        foreach (Monster monster in Bot.Monsters.MapMonsters)
        {
            if (monster == null || monster.HP <= 0)
                continue;

            string? name = monster.Name;
            string? cell = monster.Cell;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(cell))
                continue;

            var key = (name, cell);
            monsterCounts.TryGetValue(key, out int count);
            count++;
            monsterCounts[key] = count;

            if (count >= 2)
            {
                EquipClass(ClassType.Farm);
                return;
            }

        }
    }

    // Overload for a single Monster
    public void EquipBestClassForTargets(Monster? target)
    {
        if (target == null)
            return;

        EquipBestClassForTargets(new[] { target });
    }

    /// <summary>
    /// Kills a monster using it's ID
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="MonsterMapID">MapID of the monster</param>
    /// <param name="item">Item to kill the monster for, if null will just kill the monster 1 time</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log">Whether it will log that it is killing the monster</param>
    /// <param name="publicRoom"></param>
    public void KillMonster(
        string map,
        string cell,
        string pad,
        int MonsterMapID,
        string? item = null,
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = false
    )
    {
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());
        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;
        // Already has item? Exit early
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        if (log && item != null)
            FarmingLogger(item, quant);

        if (item != null && !isTemp)
            AddDrop(item);

        // Ensure player is in the map and cell
        if (Bot.Map.Name != map)
            Join(map, cell, pad, publicRoom: publicRoom);

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad, autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        // Disable aggro while positioning
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        // Get all target monsters by ID
        List<Monster> targetMonsters = FindMonstersList(map, "*", MonsterMapID);

        if (targetMonsters.Count == 0)
        {
            if (log)
                Logger($"⚠️ No monsters with ID {MonsterMapID} found in cell {cell}.");
            return;
        }



        // Main kill loop
        while (!Bot.ShouldExit && (item == null || !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))))
        {
            // Respawn check
            while (!Bot.ShouldExit && !Bot.Player.Alive)
                Sleep(500);

            // Ensure correct position
            if (Bot.Map.Name != map)
                Join(map, cell, pad, publicRoom: publicRoom);

            if (Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            // Attack monsters
            foreach (Monster monster in targetMonsters.ToArray())
            {
                if (monster == null)
                    continue;

                if (!Bot.Player.Alive)
                    continue;

                Bot.Combat.Attack(monster);
                Sleep(500);

                // Stop if we collected enough
                if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
                    break;
            }
        }

        // Pickup item if needed
        if (item != null)
            Bot.Wait.ForPickup(item);

        Rest(); // Recover if necessary

        // Reset options
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = true;
        Bot.Options.AggroMonsters = true;
    }

    /// <summary>
    /// Joins a map, jumps to the given cell/pad, and kills the monster with the specified MonsterMapID.
    /// Optionally farms for an item until the desired quantity is reached.
    /// </summary>
    public void KillMonster(string map, string cell, string pad, int MonsterMapID, int ItemID = 0, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false)
    {
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower());
        cell = Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase)) ?? cell;

        // Exit if item already collected
        if (ItemID != 0 && (isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant)))
            return;

        if (log && ItemID != 0)
            FarmingLogger(Bot.Inventory.GetItem(ItemID)?.Name ?? ItemID.ToString(), quant);

        if (ItemID != 0 && !isTemp)
            AddDrop(ItemID);

        // Join map and cell if needed
        if (Bot.Map.Name != map)
            Join(map, cell, pad, publicRoom: publicRoom);

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad, autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        // Store original combat settings
        bool prevAggroAll = Bot.Options.AggroAllMonsters;
        bool prevAggro = Bot.Options.AggroMonsters;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        // Get all monsters with the MapID
        List<Monster> targetMonsters = Bot.Monsters.MapMonsters
            .Where(m => m != null && m.MapID == MonsterMapID && m.Alive)
            .ToList();

        if (targetMonsters.Count == 0)
        {
            Logger($"⚠️ No monsters with MapID {MonsterMapID} found in /{map} ({cell}, {pad})");
            // Restore settings
            Bot.Options.AggroAllMonsters = prevAggroAll;
            Bot.Options.AggroMonsters = prevAggro;
            return;
        }



        // Kill until item obtained (or once if no item)
        while (!Bot.ShouldExit && (ItemID == 0 || !(isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant))))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (Bot.Map.Name != map)
                Join(map, cell, pad, publicRoom: publicRoom);

            if (Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            // Refresh alive targets
            targetMonsters = targetMonsters.Where(m => m.Alive).ToList();
            if (targetMonsters.Count == 0)
            {
                Logger($"⚠️ No alive monsters with MapID {MonsterMapID} left in /{map}");
                break;
            }

            foreach (Monster monster in targetMonsters.ToArray())
            {
                if (!Bot.Player.Alive)
                    break;

                Bot.Combat.Attack(monster.MapID);
                Sleep(500);

                if (ItemID != 0 && (isTemp ? Bot.TempInv.Contains(ItemID, quant) : CheckInventory(ItemID, quant)))
                    break;
            }
        }

        if (ItemID != 0)
            Bot.Wait.ForPickup(ItemID);

        Rest();

        // Restore original combat settings
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = prevAggroAll;
        Bot.Options.AggroMonsters = prevAggro;
    }

    public void KillBossWithSecondary(
        string map, string cell, string pad,
        int primaryMapID,
        int secondaryMapID,
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = true)
    {
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        if (item != null && log)
            FarmingLogger($"⚔️ {item}", quant);

        if (item != null && !isTemp)
            AddDrop(item);

        _Kill(primaryMapID, secondaryMapID, item, quant, isTemp);

        void _Kill(int primaryID, int secondaryID, string? drop, int quantity, bool temp)
        {
            Bot.Options.AggroMonsters = true;

            bool done = false;
            while (!Bot.ShouldExit && !done)
            {
                if (!(Bot.Player?.Alive ?? false))
                    Bot.Wait.ForTrue(() => Bot.Player?.Alive ?? false, 20);

                if (Bot.Map.Name != map)
                    Join(map, cell, pad);

                if (Bot.Player!.Cell != cell)
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                }

                Monster? primary = Bot.Monsters.MapMonsters
                    .FirstOrDefault(m => m != null && m.MapID == primaryID);

                Monster? secondary = Bot.Monsters.MapMonsters
                    .FirstOrDefault(m => m != null && m.MapID == secondaryID);

                if (secondary != null && secondary.Alive)
                {
                    Bot.Combat.Attack(secondaryID);
                }
                else if (primary != null && primary.Alive)
                {
                    Bot.Combat.Attack(primaryID);
                }

                Sleep();

                if (drop == null && (primary == null || !primary.Alive))
                {
                    if (log)
                        Logger("💀 Boss killed once");
                    done = true;
                }
                else if (
                    drop != null &&
                    (temp ? Bot.TempInv.Contains(drop, quantity) : CheckInventory(drop, quantity))
                )
                {
                    done = true;
                }
            }

            if (!temp && drop != null)
                Bot.Wait.ForPickup(drop);
        }
    }


    /// <summary>
    /// Joins a map and hunts for the monster.
    /// </summary>
    /// <param name="map">Map to join.</param>
    /// <param name="monster">Name of the monster to kill.</param>
    /// <param name="item">Item to hunt the monster for. If null, will just hunt and kill the monster once.</param>
    /// <param name="quant">Desired quantity of the item.</param>
    /// <param name="isTemp">Whether the item is temporary.</param>
    /// <param name="log">Whether to log the hunt process.</param>
    /// <param name="publicRoom">Whether to use a public room.</param>
    public void HuntMonster(
        string map,
        string monster,
        string? item = null,
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = false
    )
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        // Join the specified map
        if (Bot.Map.Name != map)
        {
            Join(map, publicRoom: publicRoom);
            Bot.Wait.ForMapLoad(map);
        }

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        if (item != null && !isTemp)
            AddDrop(item);

        Monster? targetMonster = FindMonster(map, monster);
        if (targetMonster == null)
        {
            Logger($"⚠️ Monster \"{monster}\" not found in /{map}.");
            return;
        }

        if (Bot.Map.PlayerNames?.Any(x => x != Bot.Player.Username) == true)
        {
            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true; // Trust Tato — reduces lag
        }
        else
            Bot.Options.AggroMonsters = false;


        if (item == null)
        {
            while (!Bot.ShouldExit)
            {
                if (Bot.Player != null && !Bot.Player.Alive)
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                if (Bot.Player?.Cell != targetMonster?.Cell)
                {
                    Jump(targetMonster?.Cell ?? "Enter");
                    Bot.Wait.ForCellChange(targetMonster?.Cell ?? "Enter");
                    Bot.Player!.SetSpawnPoint();
                }
                if (!Bot.Player!.HasTarget && targetMonster != null && targetMonster.HP > 0)
                    Bot.Combat.Attack(targetMonster.Name);

                Bot.Sleep(500);

                if (
                    !Bot.Player.HasTarget
                    || (Bot.Player.Target != null && Bot.Player?.Target?.HP <= 0)
                )
                {
                    Bot.Options.AttackWithoutTarget = false;
                    Bot.Options.AggroMonsters = false;
                    Bot.Options.HidePlayers = false;
                    return;
                }
            }
            JumpWait();
            Rest();
        }
        else
        {
            if (log)
                Logger($"Killing {monster} for item: \"{item}\" " + $"{Bot.Inventory.GetQuantity(item)}/{quant}", "HuntMonster");
            while (
                !Bot.ShouldExit
                && !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
            )
            {
                if (!Bot.Player.Alive)
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                targetMonster = FindMonster(map, monster);
                if (targetMonster == null)
                    continue;

                if (Bot.Player.Cell != null && Bot.Player.Cell != targetMonster?.Cell)
                {
                    string cellToJump = targetMonster?.Cell ?? "Enter";
                    Jump(cellToJump, "Left");
                    Bot.Wait.ForCellChange(cellToJump);
                }

                if (!Bot.Player.HasTarget && targetMonster != null && targetMonster.HP > 0)
                    Bot.Combat.Attack(targetMonster.Name);

                Bot.Sleep(500);

                if (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
                    break;
            }

            Bot.Options.AttackWithoutTarget = false;
            Bot.Options.AggroMonsters = false;
            Bot.Options.HidePlayers = false;
            ToggleAggro(false);

            // Attempt to jump back to an 'Enter' or usable cell
            Bot.Map.Jump(
                Bot.Map.Cells.FirstOrDefault(c => c.ToLower().Contains("enter"))
                ?? Bot.Map.Cells.FirstOrDefault(c =>
                        !c.ToLower().Contains("wait")
                        && !c.ToLower().Contains("blank")
                        && !c.ToLower().Contains("enter")
                    )
                    ?? "Enter",
                "Spawn"
            );

            JumpWait();
            Rest();
        }
    }

    /// <summary>
    /// Hunts a monster for multiple items with independent temp checks.
    /// </summary>
    public void HuntMonsterMulti(
        string map,
        string monster,
        (string item, int quant, bool isTemp)[] items,
        bool log = true,
        bool publicRoom = false
    )
    {
        if (items == null || items.Length == 0)
            return;

        // Exit early if everything is already obtained
        if (items.All(x => x.isTemp
                ? Bot.TempInv.Contains(x.item, x.quant)
                : CheckInventory(x.item, x.quant)))
            return;

        if (Bot.Map.Name != map)
        {
            Join(map, publicRoom: publicRoom);
            Bot.Wait.ForMapLoad(map);
        }

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        // Register non-temp drops
        foreach ((string item, _, bool isTemp) in items)
            if (!isTemp)
                AddDrop(item);

        Monster? targetMonster = FindMonster(map, monster);
        if (targetMonster == null)
        {
            Logger($"⚠️ Monster \"{monster}\" not found in /{map}.");
            return;
        }

        if (Bot.Map.PlayerNames?.Any(x => x != Bot.Player.Username) == true)
        {
            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;
        }
        else
            Bot.Options.AggroMonsters = false;

        if (log)
        {
            foreach ((string item, int quant, bool isTemp) in items)
            {
                int current = isTemp
                    ? Bot.TempInv.GetQuantity(item)
                    : Bot.Inventory.GetQuantity(item);

                Logger($"Farming \"{item}\" {current}/{quant}", "HuntMonsterMulti");
            }
        }

        while (!Bot.ShouldExit &&
            !items.All(x => x.isTemp
                ? Bot.TempInv.Contains(x.item, x.quant)
                : CheckInventory(x.item, x.quant)))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            targetMonster = FindMonster(map, monster);
            if (targetMonster == null)
                continue;

            if (Bot.Player.Cell != targetMonster.Cell)
            {
                Jump(targetMonster.Cell, "Left");
                Bot.Wait.ForCellChange(targetMonster.Cell);
            }

            if (!Bot.Player.HasTarget && targetMonster.HP > 0)
                Bot.Combat.Attack(targetMonster.Name);

            Bot.Sleep(500);
        }

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.HidePlayers = false;
        ToggleAggro(false);

        Bot.Map.Jump(
            Bot.Map.Cells.FirstOrDefault(c => c.ToLower().Contains("enter"))
            ?? Bot.Map.Cells.FirstOrDefault(c =>
                    !c.ToLower().Contains("wait")
                    && !c.ToLower().Contains("blank")
                    && !c.ToLower().Contains("enter"))
            ?? "Enter",
            "Spawn"
        );

        JumpWait();
        Rest();
    }


    /// <summary>
    /// Kills a monster using its MapID
    /// </summary>
    public void HuntMonsterMapID(string map, int monsterMapID, string? item = null, int quant = 1, bool isTemp = true, bool log = true, bool publicRoom = false, string pad = "Left")
    {
        // Join map if needed
        if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
        {
            Join(map, publicRoom: publicRoom);
            Bot.Wait.ForMapLoad(map);
        }

        if (log && item != null)
            FarmingLogger($"💎 {item}", quant);

        if (!isTemp)
            AddDrop(item!);

        // Find target monster(s) alive
        List<Monster> targets = Bot.Monsters.MapMonsters
            .Where(m => m != null && m.MapID == monsterMapID && m.Alive)
            .ToList();

        if (!targets.Any())
            return;

        if (Bot.Map.PlayerNames?.Any(x => x != Bot.Player.Username) == true)
        {
            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;
        }
        else
        {
            Bot.Options.AggroMonsters = false;
        }

        // Main attack loop
        while (!Bot.ShouldExit && (item == null || !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
            {
                Join(map, publicRoom: publicRoom);
                Bot.Wait.ForMapLoad(map);
            }

            // Refresh alive targets
            targets = targets.Where(t => t.Alive).ToList();
            if (!targets.Any())
                break;

            Monster target = targets.First();

            if (!string.Equals(Bot.Player.Cell, target.Cell, StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.Jump(target.Cell, pad);
                Bot.Wait.ForCellChange(target.Cell);
            }

            if (!Bot.Player.HasTarget || Bot.Player.Target?.MapID != monsterMapID)
                Bot.Combat.Attack(target.MapID);

            Sleep(200);

            if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
            || (item == null && Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0))
                break;
        }

        if (item != null)
        {
            Bot.Wait.ForDrop(item);
            Bot.Wait.ForPickup(item);
        }

        // Reset combat options safely
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.HidePlayers = false;

        // Jump to safe cell after hunt
        string safeCell = Bot.Map.Cells
            .FirstOrDefault(c => !BlackListedJumptoCells.Contains(c)
                                 && !Bot.Monsters.MapMonsters.Any(m => m?.Cell == c)
                                 && (Bot.Map.Cells.Count(cell => cell.Contains("Enter")) > 1 || !c.Contains("Enter")))
            ?? "Enter";

        Bot.Map.Jump(safeCell, safeCell == "Enter" ? "Spawn" : "Left");
        Bot.Wait.ForCellChange(safeCell);
        Sleep();
        JumpWait();
        Rest();
    }

    /// <summary>
    /// Old-compatible signature: forwards to the new overload with log = true so existing calls keep working.
    /// </summary>
    public void HuntMonsterQuest(
        int questId,
        params (string mapName, string monsterName, ClassType classType)[] MapMonsterClassPairs
    ) => HuntMonsterQuest(questId, log: true, MapMonsterClassPairs);

    /// <summary>
    /// Loads the quest, unbanks required items, adds non-temp drops, iterates requirements (using provided map/monster/class tuples) to call HuntMonster() for each, then attempts to complete the quest.
    /// </summary>
    /// <param name="questId">The ID of the quest to load requirements from.</param>
    /// <param name="log">Whether to log each hunt action (forwarded to HuntMonster)</param>
    /// <param name="MapMonsterClassPairs">Array of map name, monster name, and class type tuples.</param>
    public void HuntMonsterQuest(
        int questId,
        bool log = true,
        params (string mapName, string monsterName, ClassType classType)[] MapMonsterClassPairs
    )
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger(
                $"❌ Failed to load quest with ID [{questId}] after multiple attempts.",
                stopBot: true
            );
            return;
        }

        var itemsToUnbank = quest
            .AcceptRequirements.Concat(quest.Requirements)
            .Select(x => x.ID)
            .Distinct()
            .ToArray();

        Unbank(itemsToUnbank);

        // Add the non-temp items to the drop pickup list
        Bot.Drops.Add(
            quest
                .AcceptRequirements.Concat(quest.Requirements)
                .Where(x => x != null && !x.Temp)
                .Select(x => x.ID)
                .Distinct()
                .ToArray()
        );

        // If no MapMonsterClassPairs are provided, auto-generate default values
        if (MapMonsterClassPairs.Length == 0)
        {
            MapMonsterClassPairs = [.. quest
                .Requirements.Select(_ => ("Fill ME", "Fill ME", ClassType.Solo))];
        }

        for (int i = 0; i < MapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            var (mapName, monsterName, classType) = MapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            // Equip the class before hunting
            EquipClass((mapName, monsterName, classType).classType);

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            HuntMonster(
                mapName ?? Bot.Map.Name,
                monsterName ?? "*",
                requirement.Name ?? string.Empty,
                requirement.Quantity,
                requirement.Temp,
                log
            );
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
    public void HuntMonsterQuest(
        int questId,
        string? mapName = null,
        string? monsterName = null
    )
    {
        Quest? quest = InitializeWithRetries(() => Bot.Quests.EnsureLoad(questId));
        if (quest == null)
        {
            Logger($"⚠️ Quest {questId} not found"); // ⚠️
            return;
        }

        // Combine all requirements into one list for reusability
        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        // Ensure that there are requirements to hunt
        if (allRequirements.Count == 0)
        {
            Logger($"⚠️ Quest {questId} has no requirements."); // ⚠️
            return;
        }

        // Unbank the required items
        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank); // 🏦➡️👜

        // Add non-temp items to the drop list
        Bot.Drops.Add(allRequirements.Where(x => !x.Temp).Select(x => x.ID).Distinct().ToArray()); // 📦🛒

        // Process each requirement for hunting
        foreach (var requirement in quest.Requirements)
        {
            // Use the provided map and monster names, or fall back to default values
            string huntMapName = mapName ?? Bot.Map.Name;
            string huntMonsterName = monsterName ?? "*";

            if (!Bot.Quests.EnsureAccept(questId))
                EnsureAccept(questId); // 📝✅

            HuntMonster(
                huntMapName,
                huntMonsterName,
                requirement.Name ?? "",
                requirement.Quantity,
                requirement.Temp
            ); // ⚔️🐲💎
        }

        // Ensure quest completion if possible
        if (Bot.Quests.CanCompleteFullCheck(questId))
        {
            EnsureCompleteMulti(questId); // 🏁📜
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
    public void HuntMonsterQuestChoose(
        int questId,
        string? reward = null,
        params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs
    )
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger(
                $"⚠️💀 Failed to load quest with ID [{questId}] after multiple attempts.",
                stopBot: true
            );
            return;
        }

        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"⚠️👻 Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"⚠️🎁 Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        if (!string.IsNullOrEmpty(reward))
            allRequirements.AddRange(
                quest.Rewards.Where(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))
            );

        var itemsToUnbank = allRequirements.Select(x => x.ID).Distinct().ToArray();
        Unbank(itemsToUnbank);

        Bot.Drops.Add(
            allRequirements
                .Where(x => x != null && !x.Temp)
                .Select(x => x.Name)
                .Distinct()
                .ToArray()
        );

        if (mapMonsterClassPairs.Length == 0)
            mapMonsterClassPairs = [.. quest
                .Requirements.Select(_ => ("default_map", "default_monster", ClassType.Solo))];
        else if (mapMonsterClassPairs.Length > quest.Requirements.Count)
            Logger(
                $"⚠️🛑 More map-monster-class pairs provided than quest requirements. Extra pairs will be ignored.",
                stopBot: false
            );

        for (int i = 0; i < mapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            (string mapName, string monsterName, ClassType classType) = mapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            EquipClass(classType);
            HuntMonster(
                mapName,
                monsterName,
                requirement.Name,
                requirement.Quantity,
                requirement.Temp
            );
        }

        if (
            !Bot.Quests.EnsureComplete(
                questId,
                reward != null
                    ? quest
                        .Rewards.FirstOrDefault(x =>
                            x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)
                        )
                        ?.ID
                    ?? -1
                    : -1
            )
        )
        {
            Logger($"🛡️✅ Retry completing quest {questId} with reward {reward ?? "none"}");
            EnsureCompleteMulti(
                questId,
                itemID: reward != null
                    ? quest
                        .Rewards.FirstOrDefault(x =>
                            x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)
                        )
                        ?.ID
                    ?? -1
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
    public void HuntMonsterQuestChoose(
        int questId,
        string? reward = null,
        string? mapName = null,
        string? monsterName = null,
        bool log = false
    )
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger(
                $"⚠️💀 Failed to load quest with ID [{questId}] after multiple attempts.",
                stopBot: true
            );
            return;
        }

        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"⚠️👻 Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"⚠️🎁 Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        if (!string.IsNullOrEmpty(reward))
            allRequirements.AddRange(
                quest.Rewards.Where(x => x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase))
            );

        Unbank(allRequirements.Select(x => x.ID).Distinct().ToArray());
        Bot.Drops.Add(allRequirements.Where(x => !x.Temp).Select(x => x.Name).Distinct().ToArray());

        mapName ??= "default_map";
        monsterName ??= "*";

        foreach (var requirement in quest.Requirements)
        {
            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            EquipClass(ClassType.Solo);
            HuntMonster(
                mapName,
                monsterName,
                requirement.Name ?? "",
                requirement.Quantity,
                requirement.Temp,
                log
            );
        }

        if (
            !Bot.Quests.EnsureComplete(
                questId,
                reward != null
                    ? quest
                        .Rewards.FirstOrDefault(x =>
                            x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)
                        )
                        ?.ID
                    ?? -1
                    : -1
            )
        )
        {
            Logger($"🛡️✅ Retrying completion of quest {questId} with reward {reward ?? "none"}");
            EnsureCompleteMulti(
                questId,
                itemID: reward != null
                    ? quest
                        .Rewards.FirstOrDefault(x =>
                            x.Name.Equals(reward, StringComparison.OrdinalIgnoreCase)
                        )
                        ?.ID
                    ?? -1
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
    public void HuntMonsterQuestChoose(
        int questId,
        int rewardId,
        params (string mapName, string monsterName, ClassType classType)[] mapMonsterClassPairs
    )
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger(
                $"⚠️💀 Failed to load quest with ID [{questId}] after multiple attempts.",
                stopBot: true
            );
            return;
        }

        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"⚠️👻 Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"⚠️🎁 Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();

        if (rewardId > 0)
            allRequirements.AddRange(quest.Rewards.Where(x => x.ID == rewardId));

        Unbank(allRequirements.Select(x => x.ID).Distinct().ToArray());
        Bot.Drops.Add(allRequirements.Where(x => !x.Temp).Select(x => x.Name).Distinct().ToArray());

        if (mapMonsterClassPairs.Length == 0)
            mapMonsterClassPairs = [.. quest
                .Requirements.Select(_ => ("default_map", "default_monster", ClassType.Solo))];
        else if (mapMonsterClassPairs.Length > quest.Requirements.Count)
            Logger(
                $"⚠️🛑 More map-monster-class pairs provided than quest requirements. Extra pairs will be ignored.",
                stopBot: false
            );

        for (int i = 0; i < mapMonsterClassPairs.Length && i < quest.Requirements.Count; i++)
        {
            ItemBase requirement = quest.Requirements[i];
            (string mapName, string monsterName, ClassType classType) = mapMonsterClassPairs[i];

            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            EquipClass(classType);
            HuntMonster(
                mapName ?? "default_map",
                monsterName ?? "*",
                requirement.Name ?? "",
                requirement.Quantity,
                requirement.Temp
            );
        }

        if (
            !Bot.Quests.EnsureComplete(
                questId,
                rewardId > 0 ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1 : -1
            )
        )
        {
            Logger($"🛡️✅ Retrying completion of quest {questId} with reward ID {rewardId}");
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
    public void HuntMonsterQuestChoose(
        int questId,
        int rewardId,
        string? mapName = null,
        string? monsterName = null,
        bool log = false
    )
    {
        Quest? quest = InitializeWithRetries(() => EnsureLoad(questId));
        if (quest == null)
        {
            Logger(
                $"⚠️💀 Failed to load quest with ID [{questId}] after multiple attempts.",
                stopBot: true
            );
            return;
        }

        if (quest.Requirements == null || quest.Requirements.Count == 0)
        {
            Logger($"⚠️👻 Quest with ID [{questId}] has no requirements.", stopBot: true);
            return;
        }

        if (quest.Rewards == null || quest.Rewards.Count == 0)
        {
            Logger($"⚠️🎁 Quest with ID [{questId}] has no rewards.", stopBot: true);
            return;
        }

        var allRequirements = quest.AcceptRequirements.Concat(quest.Requirements).ToList();
        if (rewardId > 0)
            allRequirements.AddRange(quest.Rewards.Where(x => x.ID == rewardId));

        Unbank(allRequirements.Select(x => x.ID).Distinct().ToArray());
        Bot.Drops.Add(allRequirements.Where(x => !x.Temp).Select(x => x.Name).Distinct().ToArray());

        mapName ??= "default_map";
        monsterName ??= "*";

        foreach (ItemBase requirement in quest.Requirements)
        {
            if (CheckInventory(requirement.ID, requirement.Quantity))
                continue;

            if (!Bot.Quests.IsInProgress(questId))
                EnsureAccept(questId);

            EquipClass(ClassType.Solo);
            HuntMonster(
                mapName,
                monsterName,
                requirement.Name ?? string.Empty,
                requirement.Quantity,
                requirement.Temp,
                log
            );
        }

        if (
            !Bot.Quests.EnsureComplete(
                questId,
                rewardId > 0 ? quest.Rewards.FirstOrDefault(x => x.ID == rewardId)?.ID ?? -1 : -1
            )
        )
        {
            Logger($"🛡️✅ Retrying completion of quest {questId} with reward ID {rewardId}");
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

    int MobFindRetry = 0;

    /// <summary>
    /// Finds monsters on the map, optionally filtering by MapID or monster name.<br/>
    /// Supports wildcards ("*") to return all monsters.<br/>
    /// <para>Usage examples:</para>
    /// 1️⃣ Hunt by MapID + monster name:<br/>
    ///    Monster? TargetMonster = FindMonsters("chronogem", "Gem Forgemaster", 144);<br/>
    /// 2️⃣ Hunt by MapID only → returns all monsters in that MapID:<br/>
    ///    Monster? TargetMonster = FindMonsters("chronogem", monsterMapID: 144);<br/>
    /// 3️⃣ Hunt by name only (MapID ignored):<br/>
    ///    Monster? TargetMonster = FindMonsters("chronogem", "Gem Forgemaster");<br/>
    /// 4️⃣ Hunt all monsters in map (MapID optional):<br/>
    ///    Monster? TargetMonster = FindMonsters("chronogem", "*");
    /// </summary>
    public List<Monster> FindMonstersList(string map = "", string monster = "*", int monsterMapID = -1)
    {
        if (!Bot.Player.Loaded)
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);

        if (Bot.Map.Name != map)
            Join(map);

        IEnumerable<Monster> candidates = Bot.Monsters.MapMonsters
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Name));

        // 1️⃣ MapID is authoritative
        if (monsterMapID >= 0)
        {
            List<Monster> byMapId = candidates
                .Where(x => x.MapID == monsterMapID)
                .ToList();

            if (byMapId.Count > 0)
                return byMapId;

            // 2️⃣ Fallback: some monsters use ID instead of MapID
            return candidates
                .Where(x => x.ID == monsterMapID)
                .ToList();
        }

        // "*" → everything valid
        if (string.IsNullOrWhiteSpace(monster) || monster == "*")
            return [.. candidates];

        string target = monster.FormatForCompare();

        // 🔁 If we already auto-corrected this typo before, reuse it silently
        if (_monsterNameCache.TryGetValue(target, out string? corrected))
            target = corrected;


        // 3️⃣ Exact name match ONLY
        List<Monster> matches = candidates
            .Where(x => x.Name.FormatForCompare() == target)
            .ToList();

        if (matches.Count > 0)
            return matches;

        // 4️⃣ Fuzzy fallback (typo protection)
        string targetClean = target;

        Monster? closestMatch = candidates
            .Select(x => new
            {
                Monster = x,
                Distance = LevenshteinDistance(
                    x.Name.FormatForCompare(),
                    targetClean)
            })
            .OrderBy(x => x.Distance)
            .FirstOrDefault()?.Monster;

        if (closestMatch != null)
        {
            int distance = LevenshteinDistance(
                closestMatch.Name.FormatForCompare(),
                targetClean);

            if (distance <= AllowedTypoDistance(targetClean))
            {
                string correctedName = closestMatch.Name.FormatForCompare();
                // cache the correction so we never log this again
                _monsterNameCache[targetClean] = correctedName;

                Logger(
                    $"⚠️ Monster \"{monster}\" not found in /{map}. "
                  + $"Auto-correcting to closest match: \"{closestMatch.Name}\" (distance {distance})"
                );

                return [closestMatch];
            }
        }

        // 5️⃣ True failure log
        string[] visible = candidates
            .Select(x => $"\"{x.Name}\" [{(x.Alive ? "Alive" : "Dead")}]")
            .Distinct()
            .ToArray();

        Logger(
            $"❌ Monster \"{monster}\" not found in /{map}. "
          + $"Visible monsters: {string.Join(", ", visible)}"
        );
        MobFindRetry++;

        if (MobFindRetry < 5)
            return [];

        Bot.Log(
            $"Failed to Find the mob {monster} within 5 retrys"
            + (monsterMapID > 0 ? $"[MID: {monsterMapID}]" : "")
            + $"Do either of the following:\n" +
            "\t1. Relog, and restart the script\n" +
            "\t2. Try a different server."
            + "Script will now stop.");
        Bot.StopAsync();

        return [];
    }

    // monster name typo cache  (requestedName -> correctedName)
    private readonly Dictionary<string, string> _monsterNameCache = [];

    public Monster? FindMonster(string map, string monster, int monsterMapID = -1)
    {
        List<Monster> matches = FindMonstersList(map, monster, monsterMapID);

        return matches.FirstOrDefault(x => x.Alive)
            ?? matches.FirstOrDefault();
    }

    private static int LevenshteinDistance(string a, string b)
    {
        int[,] matrix = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= b.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost
                );
            }

        return matrix[a.Length, b.Length];
    }

    private static int AllowedTypoDistance(string name)
        => name.Length switch
        {
            <= 6 => 1,
            <= 12 => 2,
            _ => 3
        };





    /// <summary>
    /// Kill Escherion for the desired item
    /// </summary>
    /// <param name="item">Item name</param>
    /// <param name="quant">Desired quantity</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log"></param>
    /// <param name="publicRoom"></param>
    /// <param name="FromSupplies"></param>
    public void KillEscherion(
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = true,
        bool publicRoom = false,
        bool FromSupplies = false
    )
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        if (!FromSupplies && item != null && log)
            FarmingLogger($"⚔️ {item}", quant);

        if (item != null && !isTemp)
            AddDrop(item);

        if (item == null)
        {
            if (log)
                Logger("💀 Killing Escherion");

            _KillEscherion();
        }
        else
        {
            _KillEscherion(item, quant, isTemp);
            Rest();
        }

        void _KillEscherion(string? item = null, int quant = 1, bool isTemp = false)
        {
            Bot.Options.AggroMonsters = true;

            if (
                item != null
                && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
            )
                return;

            CheckMap();

            bool done = false;
            while (!Bot.ShouldExit && !done)
            {
                if (!(Bot.Player?.Alive ?? false))
                    Bot.Wait.ForTrue(() => Bot.Player?.Alive ?? false, 20);

                CheckMap();

                // MonsterMapIDs:
                // 2 = Staff
                // 3 = Escherion
                if (!Bot.Player!.HasTarget)
                    Bot.Combat.Attack(3);
                else if (
                    Bot.Player?.Target?.MapID == 3
                    && Bot.Player?.Target?.State == 2
                    && Bot.Monsters.MapMonsters.FirstOrDefault(x =>
                        x != null && x.MapID == 2
                    )?.Alive == true
                )
                    // Escherion is invulnerable → attack Staff of Inversion
                    Bot.Combat.Attack(2);
                else if (Bot.Player?.Target?.MapID == 2 && Bot.Player?.Target?.HP > 0)
                    // Staff of Inversion still alive → attack it
                    Bot.Combat.Attack(2);
                else
                    // Otherwise, attack Escherion
                    Bot.Combat.Attack(3);
                Sleep();

                if (item == null && !Bot.Player!.HasTarget)
                {
                    if (log)
                        Logger("💀 No item selected, killed Escherion once");
                    done = true;
                    break;
                }
                else if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
                {
                    done = true;
                    break;
                }
            }

            if (!isTemp && item != null)
                Bot.Wait.ForPickup(item);
        }
        void CheckMap()
        {
            if (
                Bot.Map.Name == "escherion"
                && Bot.Player?.Cell != "Cut1"
                && Bot.Player?.Cell == "Boss"
            )
                return;

            if (Bot.Map.Name != "escherion")
            {
                Join("escherion", "Boss", "Left");
            }
            if (Bot.Player?.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
                Bot.Player?.SetSpawnPoint();
            }
            if (Bot.Player?.Cell == "Cut1")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
                Bot.Player.SetSpawnPoint();
            }
        }
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
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        Join("stalagbite");
        Bot.Wait.ForMapLoad("stalagbite");
        Jump("r2", "Left");
        Bot.Wait.ForCellChange("r2");

        if (item == null)
        {
            if (log)
                Logger("⚡ Killing Vath");
            KillVathInternal();
        }
        else
        {
            if (!isTemp)
                AddDrop(item);
            if (log)
                Logger(
                    $"⚡ Killing Vath for {item} ({dynamicQuant(item, isTemp)}/{quant}) [Temp = {isTemp}]"
                );
            while (!Bot.ShouldExit && !CheckInventory(item, quant))
                KillVathInternal();
        }

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.HidePlayers = false;
        JumpWait();

        void KillVathInternal()
        {
            if (Bot.Map.Name != "stalagbite")
            {
                Join("stalagbite");
                Bot.Wait.ForMapLoad("stalagbite");
                Sleep();
            }

            if (Bot.Player.Cell != "r2")
            {
                Jump("r2");
                Bot.Wait.ForCellChange("r2");
                Sleep();
            }

            Monster? vath = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID == 7);
            Monster? stalagbite = Bot.Monsters.MapMonsters.FirstOrDefault(x => x.MapID == 8);

            if (stalagbite != null)
            {
                Bot.Wait.ForMonsterSpawn(stalagbite.Name);
                if (vath != null)
                    Bot.Combat.Attack(stalagbite.State is 1 or 2 ? stalagbite : vath);
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
    public void KillKitsune(
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = true
    )
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        if (Bot.Map.Name != "kitsune")
        {
            Join("kitsune");
            Bot.Wait.ForMapLoad("kitsune");
        }

        if (Bot.Player.Cell != "Boss")
        {
            Bot.Map.Jump("Boss", "Left");
            Bot.Wait.ForCellChange("Boss");
        }

        Bot.Events.ExtensionPacketReceived += KitsuneListener;

        if (item == null)
        {
            if (log)
                Logger("🌀 Killing Kitsune");

            #region Map & Cell insurance

            if (Bot.Map.Name != "kitsune")
            {
                Join("kitsune", "Boss", "Left");
                Bot.Wait.ForMapLoad("kitsune");
            }

            if (Bot.Player.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left");
                Bot.Wait.ForCellChange("Boss");
            }

            #endregion Map & Cell insurance

            Bot.Kill.Monster("Kitsune");
        }
        else
        {
            if (!isTemp)
                AddDrop(item);
            if (log)
                Logger(
                    $"🌀 Killing Kitsune for {item} ({dynamicQuant(item, isTemp)}/{quant}) [Temp = {isTemp}]"
                );
            while (!Bot.ShouldExit && !CheckInventory(item, quant))
            {
                #region Map & Cell insurance

                if (Bot.Map.Name != "kitsune")
                {
                    Join("kitsune", "Boss", "Left");
                    Bot.Wait.ForMapLoad("kitsune");
                }

                if (Bot.Player!.Cell != "Boss")
                {
                    Bot.Map.Jump("Boss", "Left");
                    Bot.Wait.ForCellChange("Boss");
                }

                #endregion Map & Cell insurance

                if (
                    !Bot.Player.HasTarget
                    || Bot.Player.Target == null || Bot.Player?.Target?.HP > 0
                )
                    Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
        }

        Bot.Events.ExtensionPacketReceived -= KitsuneListener;

        void KitsuneListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type == "json")
            {
                string cmd = data.cmd.ToString();
                if (cmd == "ct" && data.a != null)
                {
                    foreach (dynamic a in data.a)
                    {
                        if (a?.aura?["nam"]?.ToString() == "Shapeshifted")
                            Bot.Combat.StopAttacking = ((string)a.cmd)[^0] == '+';
                    }
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
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        EquipClass(ClassType.Solo);
        Join("trigoras");

        if (Phase == 1)
            Logger("⚔️ Killing Trigoras Phase 1");
        else
            Logger("⚔️ Killing Trigoras Phase 2");

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
        // Early exit if item already owned
        if (item != null && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            return;

        string[] dotClasses =
        {
        // "ShadowStalker of Time",
        // "ShadowWeaver of Time",
        // "ShadowWalker of Time",
        // "Infinity Knight",
        // "Interstellar Knight",
        "Dragon of Time"
        // "Timeless Dark Caster",
        // "Frostval Barbarian",
        // "Blaze Binder",
        // "DeathKnight",
        // "DragonSoul Shinobi",
        // "Shadow Dragon Shinobi",
        // "Legion Revenant",
        // "Void Highlord"
    };

        // Find first owned DoT class in inventory
        InventoryItem? ownedDotClass = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(i =>
                i.Category == ItemCategory.Class &&
                dotClasses.Contains(i.Name, StringComparer.OrdinalIgnoreCase));

        // If we own one → equip it, otherwise keep whatever class is currently equipped
        if (ownedDotClass != null)
        {
            Equip(ownedDotClass.Name);
            new CoreEnhancements().Apply(ownedDotClass.Name);
        }
        else
        {
            Logger("⚠️ No recommended DoT class found — using currently equipped class.");
            Logger("--------------------------------");
            Logger($"Suggested Classes:\n{string.Join("\n", dotClasses)}");
            Logger("--------------------------------");
            new CoreEnhancements().ApplyCurrent();
        }

        bool usingShinobi =
            ownedDotClass?.Name is "DragonSoul Shinobi" or "Shadow Dragon Shinobi";

        Join("doomkitten");

        if (usingShinobi)
        {
            Logger("🎯 RNG GL due to class + kitten hit range.");
            Bot.Skills.StartAdvanced("4 | 1 | 3M<30 | 2H<30");

            if (item == null)
            {
                Logger("🎯 No item selected, killing DoomKitten once.");
                Bot.Kill.Monster("*");
                Bot.Sleep(500);
                return;
            }

            while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant)))
            {
                if (!Bot.Player?.HasTarget ?? true || Bot.Player?.Target?.HP > 0)
                    Bot.Combat.Attack("*");

                Bot.Sleep(500);
            }

            return;
        }

        // Normal farming path
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
    public void KillXiang(
        string item,
        int quant = 1,
        bool ultra = false,
        bool isTemp = false,
        bool log = true
    )
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : CheckInventory(item, quant))
        )
            return;

        if (CheckInventory("Dragon of Time"))
            Bot.Skills.StartAdvanced("Dragon of Time", true, ClassUseMode.Solo);
        else if (CheckInventory("Healer (Rare)"))
            Bot.Skills.StartAdvanced("Healer (Rare)", true, ClassUseMode.Base);
        else if (CheckInventory("Healer"))
            Bot.Skills.StartAdvanced("Healer", true, ClassUseMode.Base);

        JumpWait();

        if (log)
        {
            int current = item is null ? 0 : dynamicQuant(item, isTemp);
            Logger(
                $"🔥 Killing {(ultra ? "Ultra Xiang" : "Chaos Lord Xiang")} for {item ?? "(none)"} ({current}/{quant}) [Temp = {isTemp}]"
            );
        }
        KillMonster(
            "mirrorportal",
            ultra ? "r6" : "r4",
            "Right",
            ultra ? "Ultra Xiang" : "Chaos Lord Xiang",
            item,
            quant,
            isTemp,
            log
        );
    }

    /// <summary>
    /// Kills Nulgath Fiend Shards to obtain the desired item.
    /// </summary>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quant">The desired quantity of the item.</param>
    /// <param name="isTemp">Specifies whether the item is temporary.</param>
    /// <param name="log"></param>
    public void KillNulgathFiendShard(
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = false
    )
    {
        Logger("💨 Void method: KillNulgathFiendShard is no longer in use.");
        return;
    }


    /// <summary>
    /// Kills a Monster object until the specified item is obtained.
    /// </summary>
    public void _KillForItem(
        Monster name,
        string? item,
        int quantity,
        bool isTemp = false,
        bool rejectElse = false,
        bool log = true,
        string? cell = null
    )
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        string trimmedName = name.Name.Trim().FormatForCompare();

        // Attempt to find the monster in the current cell using the new helper
        Monster? target = FindMonster(Bot.Map.Name, trimmedName);
        if (target == null)
        {
            Logger($"⚠️ Monster \"{name.Name}\" not found in current cell.");
            return;
        }

        if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
            return;

        if (log)
            FarmingLogger(item, quantity);

        while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity)))
        {
            if (Bot.Player.Cell != target.Cell)
            {
                Bot.Map.Jump(target.Cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(target.Cell);
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(target.MapID);

            Sleep(500);

            if (rejectElse)
                Bot.Drops.RejectExcept(item);
        }

        Bot.Wait.ForPickup(item);
    }

    public void _KillForItem(
        string name,
        int itemID,
        int quantity = 1,
        bool isTemp = false,
        bool rejectElse = false,
        bool log = true,
        string? cell = null
    )
    {
        if (itemID == 0)
            throw new ArgumentNullException(nameof(itemID));

        string trimmedName = name.Trim().FormatForCompare();

        if (isTemp ? Bot.TempInv.Contains(itemID, quantity) : CheckInventory(itemID, quantity))
            return;

        if (log)
            FarmingLogger(itemID.ToString(), quantity);

        while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(itemID, quantity) : CheckInventory(itemID, quantity)))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (cell != null && Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            CanWeAggro();

            // Re-find the monster every loop
            Monster? target = FindMonster(Bot.Map.Name, trimmedName);

            if (target != null && target.HP > 0)
            {
                // Only attack if no target or current target is dead
                if (!Bot.Player.HasTarget || Bot.Player.Target != target)
                    Bot.Combat.Attack(target.Name);
            }
            else
            {
                Sleep(500); // Monster not found or dead, wait and retry
                continue;
            }

            if (rejectElse)
                Bot.Drops.RejectExcept(itemID);

            Sleep(250); // small loop delay
        }

        Bot.Wait.ForPickup(itemID);
    }

    public void _KillForItem(
        string name,
        string? item = null,
        int quantity = 1,
        bool isTemp = false,
        bool rejectElse = false,
        bool log = true,
        string? cell = null
    )
    {
        if (item == null)
            return;

        string trimmedName = name.Trim().FormatForCompare();

        if (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
            return;

        if (log)
            FarmingLogger(item, quantity);

        while (!Bot.ShouldExit && !(isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity)))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (cell != null && Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            CanWeAggro();

            // Re-find monster every loop for reliability
            Monster? target =
                // Wild card ? > null \ else name trimmed
                name == "*" ? null
                : FindMonster(Bot.Map.Name, trimmedName);

            if (name == "*" && !Bot.Player.HasTarget)
            {
                Bot.Combat.Attack("*");
            }

            /* Only attack if we 
            1. Dont have a target
            2. Target isnt {target}
            3. Target.HP <= 0
            */
            else if (target != null
                    && (!Bot.Player.HasTarget || Bot.Player.Target != target || target.HP <= 0))
            {
                Bot.Combat.Attack(target.Name);
            }

            Sleep(250); // Small delay to prevent spam
            continue; // Monster not found yet, loop again
        }

        Bot.Wait.ForPickup(item);
    }

    #region IsMonsterAlive

    public bool IsMonsterAlive(Monster? mon) =>
        mon != null && ((mon.HP > 0 && mon.State != 0) || !KilledMonsters.Contains(mon.MapID));

    public bool IsMonsterAlive(string monsterName) =>
        Bot.Monsters.CurrentMonsters.Where(m => m.Name == monsterName).Any(IsMonsterAlive);

    public bool IsMonsterAlive(int monsterID, bool useMapID)
    {
        if (useMapID)
            return IsMonsterAlive(Bot.Monsters.CurrentMonsters.Find(m => m.MapID == monsterID));
        else
            return Bot.Monsters.CurrentMonsters.Where(m => m.ID == monsterID).Any(IsMonsterAlive);
    }

    public readonly List<int> KilledMonsters = [];

    public void CleanKilledMonstersList(string map) => KilledMonsters.Clear();

    public void KilledMonsterListener(int monsterMapID) => KilledMonsters.Add(monsterMapID);

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

    public enum TagOperation
    {
        AddTag,
        AddTags,
        RemoveTag,
        RemoveTags,
        ClearTags
    }

    public void Tags(TagOperation operation, string[]? tags = null)
    {
        List<ManagedAccount> accounts = Bot.Accounts.GetAllAccounts();
        ManagedAccount? acc = accounts.FirstOrDefault(x => x.Username == Bot.Player.Username);
        if (acc == null) return;

        if (tags == null || tags.Length == 0)
            return;

        switch (operation)
        {
            case TagOperation.AddTag:
                if (!acc.Tags.Contains(tags[0]))
                    Bot.Accounts.AddTag(tags[0]);
                break;
            case TagOperation.AddTags:
                foreach (string tag in tags)
                {
                    if (!acc.Tags.Contains(tag))
                        Bot.Accounts.AddTag(tag);
                }
                break;
            case TagOperation.RemoveTag:
                Bot.Accounts.RemoveTag(tags[0]);
                break;
            case TagOperation.RemoveTags:
                foreach (string tag in tags)
                    Bot.Accounts.RemoveTags(tags);
                break;
            case TagOperation.ClearTags:
                Bot.Accounts.ClearTags();
                break;
        }
    }

    List<string> TagsToAdd = [];

    public void AutoAddTags()
    {
        TagsToAdd.Clear();
        FilterTags(Bot.Player.Username, EndGameTags);
        foreach (var kvp in EndGameTags)
        {
            bool hasInventoryItem = CheckInventory(kvp.Key, toInv: false);
            bool tagAlreadyExists = Bot.Accounts.HasTag(kvp.Value);
            if (hasInventoryItem && !tagAlreadyExists)
                TagsToAdd.Add(kvp.Value);
        }
        if (TagsToAdd.Count > 0)
        {
            Bot.Accounts.AddTags(Bot.Player.Username, TagsToAdd.ToArray());
            string tagsToAddString = string.Join(", ", TagsToAdd);
            Logger(tagsToAddString);
        }
    }

    List<string> TagsToRemove = [];

    public void FilterTags(string username, Dictionary<string, string> tags)
    {
        TagsToRemove.Clear();
        List<string> baseTags = [];
        foreach (var kvp in tags)
        {
            baseTags.Add(kvp.Value);
        }

        foreach (string tag in Bot.Accounts.GetTags(username))
        {
            if (!baseTags.Contains(tag))
                TagsToRemove.Add(tag);
        }

        if (TagsToRemove.Count > 0)
        {
            Bot.Accounts.RemoveTags(username, TagsToRemove.ToArray());
            string tagsToRemoveString = string.Join(", ", TagsToRemove);
            Logger($"Removed: {tagsToRemoveString}");
        }
    }

    public Dictionary<string, string> EndGameTags = new()
    {
        // Classes
        { "Legion Revenant", "LR" },
        { "Void Highlord", "VHL" },
        { "Verus DoomKnight", "VDK" },
        { "Lich", "Lich" },
        { "Hollowborn Vindicator", "HBV" },

        { "Dragon of Time", "DoT" },
        { "Chaos Avenger", "CAV" },
        { "ArchMage", "AM" },
        { "LightCaster", "LC" },
        { "ArchPaladin", "AP" },
        { "Lord Of Order", "LOO" },
        { "StoneCrusher", "SC" },
        { "Yami no Ronin", "YnR" },
        { "Frostval Barbarian", "FB" },

        // Armor
        { "Sepulchure's DoomKnight Armor", "SDKA" },
        { "Radiant Goddess of War", "RGoW" },
        { "Fire Champion's Armor", "FCA" },
        { "Hollowborn DoomKnight", "HBDK" },

        // Weapons
        { "Necrotic Sword of Doom", "NSoD" },
        { "Necrotic Blade of Doom", "NBoD" },

    };



    public bool IsDungeonMonsterAlive(Monster? mon) =>
        mon != null && (mon.Alive || !KilledDungeonMonsters.Contains(mon.MapID));

    public bool IsDungeonMonsterAlive(string monsterName) =>
        Bot
            .Monsters.CurrentMonsters.Where(m => m.Name == monsterName)
            .Any(m => IsDungeonMonsterAlive(m));

    public bool IsDungeonMonsterAlive(int monsterID) =>
        Bot
            .Monsters.CurrentMonsters.Where(m => m.ID == monsterID)
            .Any(m => IsDungeonMonsterAlive(m));

    public bool IsDungeonMonsterAlive(int monsterMapID, bool useMapID) =>
        IsDungeonMonsterAlive(Bot.Monsters.CurrentMonsters.Find(m => m.MapID == monsterMapID));

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

    private readonly List<int> KilledDungeonMonsters = [];

    private void CleanKilledDungeonMonstersList(string map) => KilledMonsters.Clear();

    private void KilledDungeonMonsterListener(int monsterMapID) => KilledMonsters.Add(monsterMapID);

    #endregion IsMonsterAlive

    #endregion Kill

    #region Utility

    /// <summary>
    /// Checks whether the player is an Upholder
    /// </summary>
    public bool IsUpholder() => Badges.Any(badge => badge.Name.Contains("Upholder"));
    public bool GoldMaxed => Bot.Player.Gold >= 100_000_000;

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
    /// If the provided duration is -1, 0, or negative, it uses the default action delay.
    /// </summary>
    /// <param name="ms">The duration to pause execution in milliseconds. Defaults to -1.</param>
    public void Sleep(int ms = -1)
    {
        if (Bot.ShouldExit)
        {
            Bot.StopSync(true);
            return;
        }

        // use ActionDelay if ms is -1, 0, or negative
        int delay = (ms <= 0) ? ActionDelay : ms;

        Thread.Sleep(delay);
    }

    public void Logger(string message = "", [CallerMemberName] string caller = "", bool messageBox = false, bool stopBot = false)
    {
        string rawMessage = message ?? string.Empty;

        // =========================
        // Console / file log (FULL)
        // =========================
        string logMessage = $"[{DateTime.Now:HH:mm:ss}] ({caller}) {rawMessage}";
        Bot.Log(logMessage);

        // =========================
        // AQW CHAT SAFE PIPELINE
        // =========================
        if (LoggerInChat && Bot.Player.LoggedIn)
        {
            string chatMessage = SanitizeAQWChat(rawMessage);

            const int maxLen = 180;
            if (chatMessage.Length > maxLen)
                chatMessage = chatMessage[..maxLen];

            Bot.Send.ClientModerator(chatMessage, caller);
        }

        // =========================
        // MESSAGE BOX (RAW SAFE)
        // =========================
        if (messageBox && !ForceOffMessageboxes)
            Message(rawMessage, caller);

        // =========================
        // STOP BOT FLAG
        // =========================
        if (stopBot)
        {
            scriptFinished = false;
            Bot.StopSync(true);
        }
    }

    private static string SanitizeAQWChat(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Strip non-ASCII first
        input = Regex.Replace(input, @"[^\x20-\x7E\t\n]", "");

        // Remove specific Flash-breaking characters
        input = input
            .Replace("\x00", "")  // null
            .Replace("\x1B", "")  // escape
            .Replace("\x7F", "")  // DEL
            .Replace("\r", "");   // carriage return

        // Normalize whitespace
        input = Regex.Replace(input, @"\s+", " ");

        return input.Trim();
    }


    // Word wrap function
    public static string WordWrap(string? input, int lineLength)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        StringBuilder sb = new();
        int length = 0;
        bool inSentencePause = false;

        foreach (string word in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (
                word.EndsWith('.')
                || word.EndsWith('!')
                || word.EndsWith('?')
                || word.EndsWith(',')
                || word.EndsWith(';')
                || word.EndsWith(':')
            )
                inSentencePause = true;

            // Handle long words by hard breaking them - use StringInfo to respect emoji/surrogate pairs
            if (word.Length > lineLength)
            {
                if (length > 0)
                {
                    sb.AppendLine();
                    length = 0;
                }

                var si = new StringInfo(word);
                for (int i = 0; i < si.LengthInTextElements; i += lineLength)
                {
                    int takeCount = Math.Min(lineLength, si.LengthInTextElements - i);
                    sb.AppendLine(si.SubstringByTextElements(i, takeCount));
                }
                inSentencePause = false;
                continue;
            }

            if (length + word.Length + 1 > lineLength)
            {
                sb.AppendLine();
                length = 0;
            }
            sb.Append(word).Append(' ');
            length += word.Length + 1;

            if (inSentencePause && length > lineLength / 1.3)
            {
                sb.AppendLine();
                length = 0;
                inSentencePause = false;
            }
        }
        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Logs farming activity for a specified item.
    /// </summary>
    /// <param name="item">The name of the item being farmed (nullable).</param>
    /// <param name="quant">The desired quantity of the item to farm (default is 1).</param>
    /// <param name="caller">Automatically provided by the compiler to indicate the calling member (optional).</param>
    public void FarmingLogger(string? item, int quant = 1, [CallerMemberName] string caller = "")
    {
        int quantity = string.IsNullOrEmpty(item)
            ? 0
            : Bot.TempInv.GetQuantity(item) + Bot.Inventory.GetQuantity(item);

        Logger($"Farming {item} ({quantity}/{quant})", caller);
    }

    /// <summary>
    /// Logs debug information based on various filters and script context.
    /// </summary>
    /// <param name="_this">The instance or type associated with the logging context.</param>
    /// <param name="marker">Optional marker for categorizing the log entry (default is null).</param>
    /// <param name="caller">Automatically provided by the compiler to indicate the calling member (optional).</param>
    /// <param name="lineNumber">Automatically provided by the compiler to indicate the line number where the method is called (optional).</param>
    public void DebugLogger(
        object _this,
        string? marker = null,
        [CallerMemberName] string? caller = null,
        [CallerLineNumber] int lineNumber = 0
    )
    {
        if (
            !DL_Enabled
            || (DL_MarkerFilter != null && DL_MarkerFilter != marker)
            || (DL_CallerFilter != null && DL_CallerFilter != caller)
        )
            return;

        string _class = _this.GetType().ToString();
        string[] compiledScript = CompiledScript();

        int compiledClassLine =
            Array.IndexOf(
                compiledScript,
                compiledScript.First(line => line.Trim() == $"public class {_class}")
            ) + 1;
        string[] currentScript = File.ReadAllLines(Bot.Manager.LoadedScript);
        string[]? includedScript = null;

        bool inCurrentScript = false;
        if (currentScript.Any(line => line.Trim() == $"public class {_class}"))
            inCurrentScript = true;
        else
        {
            foreach (string cs in currentScript.Where(x => x.StartsWith("//cs_include")).ToArray())
            {
                List<string> pathParts = [ClientFileSources.SkuaDIR, .. cs.Replace("//cs_include ", "").Replace("\\", "/").Split('/')];
                includedScript = File.ReadAllLines(Path.Combine([.. pathParts]));

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

        foreach (
            string l in compiledScript[
                compiledClassLine..Array.FindIndex(compiledScript, compiledClassLine, l => l == "}")
            ]
        )
        {
            if (!l.Contains("DebugLogger(this)"))
                continue;

            count++;
            lastIndex = Array.FindIndex(compiledScript, lastIndex + 1, _l => _l.Trim() == l.Trim());
            if (lastIndex + 1 == lineNumber)
                break;
        }

        int count2 = 0;
        int lastIndex2 = -1;
        string[] selectedScript =
            inCurrentScript || includedScript == null ? currentScript : includedScript;
        foreach (string l in selectedScript)
        {
            if (!l.Contains("DebugLogger(this)"))
                continue;

            count2++;
            lastIndex2 = Array.FindIndex(
                selectedScript,
                lastIndex2 + 1,
                _l => _l.Trim() == l.Trim()
            );

            if (count == count2)
                break;
        }

        Logger(
            $"{marker}{(string.IsNullOrEmpty(marker) ? null : " | ")}{_class} => {caller}, line {lastIndex2 + 1}",
            "DEBUG LOGGER"
        );
    }

    public bool DL_Enabled { get; set; } = false;
    public string? DL_CallerFilter { get; set; } = null;
    public string? DL_MarkerFilter { get; set; } = null;

    public void DL_Enable()
    {
        DL_Enabled = true;
        LoggerInChat = false;
    }

    public string[] CompiledScript() =>
        Bot.Manager.CompiledScript.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );

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
    public int dynamicQuant(string item, bool isTemp) =>
        isTemp
            ? Bot.TempInv.GetQuantity(item)
            : Bot.Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x => x.Name == item)
                ?.Quantity
            ?? 0;

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

    public bool AggroMonsters()
    {
        if (
            Bot.Map.PlayerNames != null
            && Bot.Map.PlayerNames.Where(x => x != Bot.Player.Username).Any()
        )
        {
            return true;
        }
        return false;
    }

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
        if (Bot.Options.AggroAllMonsters == true)
            Bot.Options.AggroAllMonsters = false;

        // Check if there are any other players in the cell
        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames!.Any(x => x != Bot.Player.Username))
        {
            if (Bot.Options.AggroMonsters == false)
                Bot.Options.AggroMonsters = true;

            if (Bot.Options.HidePlayers == false)
                Bot.Options.HidePlayers = true; // Hide players to reduce lag
        }
        else
        {
            if (Bot.Options.AggroMonsters == true)
                Bot.Options.AggroMonsters = false;
            if (Bot.Options.HidePlayers == true)
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
            DebugLogger(
                this,
                $"Current Class: {Bot.Player.CurrentClass} | Current Rank: {Bot.Player.CurrentClassRank}"
            );
            return Bot.Player.CurrentClassRank;
        }
        else
        {
            // Find the class item from the inventory or bank
            InventoryItem? Class = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .Find(i => i.Name == ClassName && i.Category == ItemCategory.Class);

            if (ClassName != null && Class != null)
            {
                Logger($"Class: {Class.Name} | Rank: {Class.Quantity}");

                // Define CP thresholds and corresponding ranks in a dictionary
                var ClassPointRanks = new Dictionary<int, int>
                {
                    { 900, 1 }, // Rank 1 (CP < 900)
                    { 3600, 2 }, // Rank 2 (900 <= CP < 3600)
                    { 10000, 3 }, // Rank 3 (3600 <= CP < 10000)
                    { 22500, 4 }, // Rank 4 (10000 <= CP < 22500)
                    { 44100, 5 }, // Rank 5 (22500 <= CP < 44100)
                    { 78400, 6 }, // Rank 6 (44100 <= CP < 78400)
                    { 129600, 7 }, // Rank 7 (78400 <= CP < 129600)
                    { 202500, 8 }, // Rank 8 (129600 <= CP < 202500)
                    { 302500, 9 }, // Rank 9 (202500 <= CP < 302500)
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

                DebugLogger(this, $"Class Rank (based on ClassXP): {classRank}");
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
                Logger(
                    $"Resting initiated: Health below threshold ({Bot.Player.Health}/{Bot.Player.MaxHealth})."
                );
            }
        }
        catch (Exception ex)
        {
            Logger($"An error occurred: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void Relogin(string reason = "")
    {
        Bot.Log($"⚡ Relogin Triggered{(string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}")}");

        // Save original options
        bool origAutoRelog = Bot.Options.AutoRelogin;
        bool origAutoRelogAny = Bot.Options.AutoReloginAny;
        bool origRetryRelogin = Bot.Options.RetryRelogin;
        bool origSafeRelogin = Bot.Options.SafeRelogin;

        try
        {
            // Set login info if available
            string? username = Bot.Player?.Username;
            string? password = Bot.Player?.Password;
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                Bot.Servers.SetLoginInfo(username, password);
            else
                Bot.Log("ℹ️ No cached credentials found; continuing without SetLoginInfo.");

            // Reset relogin options
            Bot.Options.AutoRelogin = false;
            Bot.Options.AutoReloginAny = false;
            Bot.Options.RetryRelogin = false;
            Bot.Options.SafeRelogin = false;

            // Try preferred server first (LastIP server or Twilly)
            string preferredServer = Bot.Options.ReloginServer ?? Bot.Servers.LastName ?? "Twilly";
            Bot.Log($"🎯 Attempting relogin to: {preferredServer} 🌐");
            CancellationTokenSource cts = new();
            Bot.Wait.ForTrue(() => Bot.Servers.EnsureRelogin(cts.Token).Result, 20);
            if (Bot.Wait.ForTrue(() => (Bot.Player?.Loaded ?? false), 20))
            {
                SendPlayerToHouse(preferredServer, reason);
                return;
            }

            // Fallback servers
            string[] fallbackServers = { "Twilly", "Twig", "Safiria" };
            foreach (string server in fallbackServers.Where(s => s != preferredServer))
            {
                Bot.Log($"⚡ Trying fallback server: {server} 🔄");
                if (Bot.Servers.EnsureRelogin(server))
                {
                    if (Bot.Wait.ForTrue(() => Bot.Player?.Loaded ?? false, 20))
                    {
                        Bot.Log($"✅ Fallback relogin to {server} successful! 🎉");
                        SendPlayerToHouse(server, reason);
                        return;
                    }
                }
                Bot.Sleep(1000);
            }

            Bot.Log("❌ Relogin failed after all attempts. 🛑");
            Bot.StopSync();
        }
        finally
        {
            // Restore original options
            Bot.Options.AutoRelogin = origAutoRelog;
            Bot.Options.AutoReloginAny = origAutoRelogAny;
            Bot.Options.RetryRelogin = origRetryRelogin;
            Bot.Options.SafeRelogin = origSafeRelogin;
        }
        return;

        void SendPlayerToHouse(string serverName, string reason = "")
        {
            Bot.Log(
                $"🏠 Relogin successful! Sending to house from {serverName}{(string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}")} ✨"
            );
            Bot.Send.Packet($"%xt%zm%house%1%{Username()}%");

            if (!Bot.Wait.ForMapLoad("house", 20))
            {
                Bot.Log("⚠️ Failed to load house map, falling back to Battleon. 🏰");
                Join("battleon-100000");
            }
        }
    }

    // /// <summary>
    // /// Checks, and prompts for the latest Skua Version
    // /// <param name="targetVersion">Current Skua Version to Check against</param>
    // /// </summary>
    // private void SkuaVersionChecker(string targetVersion = "1.4.2.0")
    // {
    //     if (Bot.Version == null || Bot.Version.ToString() == "1.3.3.2" || Version.Parse(targetVersion).CompareTo(Bot.Version) <= 0)
    //         return;

    //     if (
    //         Bot.ShowMessageBox(
    //             $"This script requires Skua {targetVersion} or above, "
    //                 + "click OK to open the download page of the latest release, or update yourself via the `Skua Manager > \"Update\" tab`",
    //             "Outdated Skua detected",
    //             "OK"
    //         ).Text == "OK"
    //     )
    //         Process.Start("explorer", "https://github.com/auqw/Skua/releases/latest");
    //     Logger(
    //         $"This script requires Skua {targetVersion} or above. Stopping the script",
    //         messageBox: true,
    //         stopBot: true
    //     );
    // }
    // 

    private const int MaxRetries = 3;

    private async Task SkuaVersionCheckerAsync()
    {
        if (Bot.Version == null || Bot.Version.ToString() == "1.3.3.2")
            return;

        bool isPt = System.Globalization.CultureInfo.CurrentUICulture
            .TwoLetterISOLanguageName == "pt";

        if (!OperatingSystem.IsWindowsVersionAtLeast(10))
        {
            Version os = Environment.OSVersion.Version;
            string winVer = $"{os.Major}.{os.Minor}.{os.Build}";

            string msg = isPt
                ? $"O Skua requer Windows 10 ou superior. Você está atualmente na versão do Windows: {winVer}"
                : $"Skua requires Windows 10 or higher. You are currently on Windows version: {winVer}";

            Logger(msg);
            Bot.ShowMessageBox(msg,
                isPt ? "Windows não suportado" : "Unsupported Windows",
                "OK");

            Logger(msg, messageBox: true, stopBot: true);
            return;
        }

        try
        {
            string atom = await GetAtomFeedAsync();

            string latestTag = ExtractLatestVersionFromAtom(atom);

            if (!Version.TryParse(latestTag, out Version? latestVersion))
            {
                Logger("Failed to parse release version.");
                return;
            }

            if (latestVersion <= Bot.Version)
                return;

            bool is64 = Environment.Is64BitOperatingSystem;
            string arch = is64 ? "x64" : "x86";
            string fileName = $"Skua-{latestTag}-Release-{arch}.msi";
            string installerPath = Path.Combine(Path.GetTempPath(), fileName);
            string downloadUrl =
                $"https://github.com/auqw/Skua/releases/download/{latestTag}/{fileName}";

            string prompt = isPt
                ? $"Nova versão detectada ({latestTag}).\n\nBaixar agora?"
                : $"New version detected ({latestTag}).\n\nDownload now?";

            if (Bot.ShowMessageBox(
                    prompt,
                    isPt ? "Atualização disponível" : "Update Available",
                    "OK",
                    isPt ? "Cancelar" : "Cancel").Text != "OK")
            {
                Logger("Update declined.");
                return;
            }

            Logger(isPt ? "Baixando instalador..." : "Downloading installer...");

            byte[] installerBytes = await ExecuteWithRetryAsync(
                () => _client.GetByteArrayAsync(downloadUrl));

            await File.WriteAllBytesAsync(installerPath, installerBytes);

            // Compute SHA256 for verification/logging
            string fileHash = ComputeSHA256(installerPath);

            // Launch installer
            Process? installerProc = Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec",
                Arguments = $"/i \"{installerPath}\"",
                UseShellExecute = true
            });

            // Safe cleanup: wait a few seconds, then delete only if installer has started
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000); // short initial wait
                try
                {
                    if (installerProc != null && !installerProc.HasExited && File.Exists(installerPath))
                    {
                        // Wait a bit more to avoid deleting too early
                        await Task.Delay(10000);
                    }

                    if (File.Exists(installerPath))
                        File.Delete(installerPath);
                }
                catch { }
            });


            Logger("Update required. Stopping script.",
                messageBox: true,
                stopBot: true);
        }
        catch (Exception ex)
        {
            Logger($"Update check failed: {ex.Message}");
        }
    }

    // Dynamic User-Agent per request
    private async Task<string> GetAtomFeedAsync()
    {
        using System.Net.Http.HttpRequestMessage request = new(
            HttpMethod.Get,
            "https://github.com/auqw/Skua/releases.atom");

        string versionString = Bot.Version?.ToString() ?? "unknown";
        request.Headers.UserAgent.ParseAdd($"SkuaVersionChecker/{versionString}");

        using System.Net.Http.HttpResponseMessage response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    // Retry helper with exponential backoff
    private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch when (attempt < MaxRetries)
            {
                int delay = (int)Math.Pow(2, attempt) * 1000;
                await Task.Delay(delay);
            }
        }

        throw new Exception("Max retry attempts reached.");
    }

    // SHA256 helper
    private string ComputeSHA256(string filePath)
    {
        using System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
        using FileStream fs = File.OpenRead(filePath);
        byte[] hash = sha.ComputeHash(fs);
        // Uppercase continuous
        return string.Concat(hash.Select(b => b.ToString("X2")));

    }

    // Atom parser (no changes)
    private string ExtractLatestVersionFromAtom(string atomXml)
    {
        int entryIndex = atomXml.IndexOf("<entry>", StringComparison.OrdinalIgnoreCase);
        if (entryIndex == -1) return string.Empty;

        int titleStart = atomXml.IndexOf("<title>", entryIndex, StringComparison.OrdinalIgnoreCase);
        if (titleStart == -1) return string.Empty;
        titleStart += 7;

        int titleEnd = atomXml.IndexOf("</title>", titleStart, StringComparison.OrdinalIgnoreCase);
        if (titleEnd == -1) return string.Empty;

        string title = atomXml.Substring(titleStart, titleEnd - titleStart).Trim();

        if (title.StartsWith("Skua ", StringComparison.OrdinalIgnoreCase)) title = title[5..];
        else if (title.StartsWith("Release ", StringComparison.OrdinalIgnoreCase)) title = title[8..];
        else if (title.StartsWith("v", StringComparison.OrdinalIgnoreCase)) title = title[1..];

        return title.Trim();
    }

    ClassType currentClass = ClassType.None;
    bool usingSoloGeneric = false;
    bool usingFarmGeneric = false;
    bool usingDodgeGeneric = false;
    bool usingBossGeneric = false;

    /// <summary>
    /// Equips either the FarmClass or SoloClass (or Dodge/Boss) from CoreBots
    /// </summary>
    /// <param name="classToUse">Type "ClassType." and then either Farm, Solo, Dodge, or Boss</param>
    public void EquipClass(ClassType classToUse)
    {
        // Already equipped? Skip if timer is running
        if (currentClass == classToUse && Bot.Skills.TimerRunning)
            return;

        currentClass = classToUse;

        switch (classToUse)
        {
            case ClassType.Farm:
                if (FarmClass != "Generic" && FarmClass != "(Current)"
                    && !string.IsNullOrEmpty(FarmClass)
                    && CheckInventory(FarmClass))
                    if (_equipClass(usingFarmGeneric, FarmClass, FarmUseMode, FarmGearOn, FarmGear))
                        return;
                break;

            case ClassType.Solo:
                if (SoloClass != "Generic" && SoloClass != "(Current)"
                    && !string.IsNullOrEmpty(SoloClass)
                    && CheckInventory(SoloClass))
                    if (_equipClass(usingSoloGeneric, SoloClass, SoloUseMode, SoloGearOn, SoloGear))
                        return;
                break;

            case ClassType.Dodge:
                if (DodgeClass != "Generic" && DodgeClass != "(Current)"
                    && !string.IsNullOrEmpty(DodgeClass)
                    && CheckInventory(DodgeClass))
                    if (_equipClass(usingDodgeGeneric, DodgeClass, DodgeUseMode, DodgeGearOn, DodgeGear))
                        return;
                break;

            case ClassType.Boss:
                if (BossClass != "Generic" && BossClass != "(Current)"
                    && !string.IsNullOrEmpty(BossClass)
                    && CheckInventory(BossClass))
                    if (_equipClass(usingBossGeneric, BossClass, BossUseMode, BossGearOn, BossGear))
                        return;
                break;
        }

        // Fallback: start skills for current class if equip didn't run
        EnableCoreSkills();

        // --- Local helper ---
        bool _equipClass(
            bool usingGeneric,
            string className,
            ClassUseMode classMode,
            bool useEquipment,
            string[] equipment
        )
        {
            if (usingGeneric)
            {
                Bot.Log("Using generic");
                return false;
            }

            if (!CheckInventory(className))
            {
                Logger("You do not own " + className);
                return false;
            }

            // Get class ID once
            InventoryItem? classItem = Bot.Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x =>
                    x.Name.Equals(className, StringComparison.OrdinalIgnoreCase)
                    && x.Category == ItemCategory.Class);

            if (classItem == null)
            {
                Logger($"Class \"{className}\" found but not categorized as Class item");
                return false;
            }

            if (useEquipment && equipment.Any())
            {
                Sleep((int)(ActionDelay * 1.5));
                Equip(equipment);
            }


            string classNameLower = className.Trim().ToLower();
            string? equippedClass = Bot.Player.CurrentClass?.Name.Trim().ToLower();
            int classID = classItem.ID;

            // Equip loop
            while (!Bot.ShouldExit && equippedClass != classNameLower)
            {
                JumpWait();
                Equip(className);
                Bot.Wait.ForItemEquip(className);
                Sleep(100); // small buffer to let server sync
                equippedClass = Bot.Player.CurrentClass?.Name.Trim().ToLower();
            }

            // Enhance equipped gear for the newly equipped class
            new CoreEnhancements().ApplyCurrent();

            // Start skills for the actual equipped class
            EnableCoreSkills();
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
                    if (Bot.Inventory.IsEquipped(item.ID))
                        Logger($"Equipped item: {item.Name} (ID: {item.ID})");
                    break;

                default:
                    Bot.Inventory.EquipItem(item.ID);
                    Sleep(1500);
                    if (Bot.Inventory.IsEquipped(item.ID))
                        Logger($"Equipped item: {item.Name} (ID: {item.ID})");
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
    public bool HasAchievement(int ID, string ia = "ia0") =>
        Bot.Flash.CallGameFunction<bool>("world.getAchievement", ia, ID);

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
                return [];
            return JsonConvert.DeserializeObject<List<Badge>>(
                    GetRequest($"https://account.aq.com/CharPage/Badges?ccid={CharacterID}")
                ) ?? [];
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

    private Random _randomInterval = new Random();
    private bool _savedStateEnabled = false;
    private string _previousMap = "";
    private DateTime _nextSaveStateTime = DateTime.MinValue;

    /// <summary>
    /// Sets the saved state to the specified status.
    /// Enables/disables automatic save state jumps to the player's house at random intervals (30 mins - 1 hour).
    /// </summary>
    /// <param name="on">True to turn on saved state; false to turn it off (default is true).</param>
    /// <param name="previousMap">The map to return to after the save state (default is empty).</param>
    public void SavedState(bool on = true, string previousMap = "")
    {
        if (string.IsNullOrEmpty(previousMap) || on is true && _savedStateEnabled)
            return;

        _savedStateEnabled = on;
        Logger($"SaveState [{_savedStateEnabled}] " +
        (_savedStateEnabled ? "We'll randomly goto /whitemap to save your progress every 30mns to an hour" : ""), "SaveStateHandler");

        if (on)
        {
            // Store the previous map for returning later
            _previousMap = previousMap;

            // Initialize next save state time
            _nextSaveStateTime = DateTime.Now.AddMilliseconds(GetRandomInterval());
        }
        else
        {
            _savedStateEnabled = false;
        }
    }

    /// <summary>
    /// Generates a random interval between 30 minutes and 1 hour (in milliseconds).
    /// </summary>
    private int GetRandomInterval()
    {
        // 30 minutes = 1,800,000 ms
        // 1 hour = 3,600,000 ms
        int minMs = 30 * 60 * 1000;      // 30 minutes
        int maxMs = 60 * 60 * 1000;      // 1 hour

        return _randomInterval.Next(minMs, maxMs + 1);
    }

    /// <summary>
    /// Checks if it's time to execute a save state during farming.
    /// Call this periodically within your farm method.
    /// </summary>
    public bool CheckSaveState()
    {
        if (!_savedStateEnabled)
            return false;

        // Check if enough time has passed
        if (DateTime.Now >= _nextSaveStateTime)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Executes the save state action: calls JumpWait and navigates to the player's house.
    /// </summary>
    public void ExecuteSaveState()
    {
        if (string.IsNullOrEmpty(_previousMap) || !_savedStateEnabled)
            return;

        try
        {
            // Call the jump wait function
            Jump("Enter", "Spawn", false);

            // Navigate to whitemap
            Join("whitemap-100000");

            // Return to previous map if stored
            if (!string.IsNullOrEmpty(_previousMap))
            {
                Join(_previousMap);
            }

            // Reset the timer with a new random interval
            _nextSaveStateTime = DateTime.Now.AddMilliseconds(GetRandomInterval());
        }
        catch (Exception ex)
        {
            Logger($"SaveState error: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates an array of integers from a starting value to an ending value (inclusive).
    /// </summary>
    /// <param name="from">The starting integer value.</param>
    /// <param name="to">The ending integer value.</param>
    /// <returns>An array of integers from 'from' to 'to' (inclusive).</returns>
    public int[] FromTo(int from, int to)
    {
        List<int> toReturn = [];
        for (int i = from; i < to + 1; i++)
            toReturn.Add(i);
        return [.. toReturn];
    }

    /// <summary>
    /// Banks miscellaneous AC-tagged inventory items from specific allowed categories,
    /// excluding equipped items, blacklisted names, and explicitly exempt item IDs.
    /// Also includes <see cref="ItemCategory.ServerUse"/> if no boosts are active,
    /// and optional CBO flags (e.g., doGoldBoost, doRepBoost) are disabled.
    /// </summary>
    /// <param name="RequiredSpaces">
    /// Optional limit on how many items to bank; if set to 0, all matching items are banked.
    /// </param>
    public void BankACMisc(int RequiredSpaces = 0)
    {
        // Items to never bank (e.g., important consumables)
        int[] exemptIDs = { 18927, 38575 }; // Treasure Potion, Dark Potion
        // Allowed inventory categories
        List<ItemCategory> allowedCategories =
        [
            ItemCategory.Note,
            ItemCategory.Item,
            ItemCategory.Resource,
            ItemCategory.QuestItem
        ];

        // Include ServerUse if boosts are not active
        if (
            !Bot.Boosts.Enabled
            && (
                CBO_Active()
                || !new[] { "doGoldBoost", "doClassBoost", "doRepBoost", "doExpBoost" }.Any(flag =>
                    CBOBool(flag, out bool enabled) && enabled
                )
            )
        )
        {
            allowedCategories.Add(ItemCategory.ServerUse);
        }

        // Filter AC-tagged misc items to bank
        var toBankItems = Bot
            .Inventory.Items.Where(item =>
                item != null
                // if item is AC tagged
                && item.Coins
                // Check if the item is equipped or worn (maingear override.. transmog sorta thing)
                && !item.Equipped
                // Check if the item is worn (maingear override.. transmog sorta thing) - FIXED: Added null check
                && (item.Wearing == false)
                // If meta (boost type) is null ( doesnt exist) or empty ("")
                && string.IsNullOrEmpty(item.Meta)
                // If the enhancement is "Adventurer" or if the enhancement level is 0
                && (item?.EnhancementPatternID == 1 || item?.EnhancementLevel == 0)
                // If allowedCategories (above) contains the item's category ( `ItemCategory .Helm` for example)
                && allowedCategories.Contains(item.Category)
                // If the banking blacklist (set at the top of a script before the first `setoptions()` ...sometimes) contains the item's name
                && !BankingBlackList.Contains(item.Name)
                // If the exemptIDs list above contains the items ItemID
                && !exemptIDs.Contains(item.ID)
                // If Farm/Solo/Boss/Dodge Gear ( set in `options > corebots`) contains the item
                && !FarmGear.Contains(item.Name)
                && item.Name != FarmClass
                && !SoloGear.Contains(item.Name)
                && item.Name != SoloClass
                && !DodgeGear.Contains(item.Name)
                && item.Name != DodgeClass
                && !BossGear.Contains(item.Name)
                && item.Name != BossClass
            )
            .ToArray();

        if (toBankItems.Length == 0)
            return;

        // Add debug logging to verify what's being banked
        if (DL_Enabled)
        {
            foreach (var item in toBankItems)
            {
                DebugLogger(
                    $"DEBUG - Banking: {item.Name} | Equipped: {item.Equipped} | Wearing: {item.Wearing} | ID: {item.ID}"
                );
            }
        }

        var selected =
            RequiredSpaces > 0 ? toBankItems.Take(RequiredSpaces).ToArray() : toBankItems;
        string names = string.Join(", ", selected.Select(item => $"\"{item.Name}\""));
        Logger($"Banking misc AC items [{selected.Length} items]: {names}");
        ToBank(selected.Select(item => item.ID).ToArray());
    }

    /// <summary>
    /// Banks miscellaneous AC-tagged non-equipped House items.
    /// </summary>
    public void BankACHouseItems()
    {
        var toHouseBank = Bot
            .House.Items.Where(item => item != null && item.Coins && !item.Equipped)
            .Select(item => item.ID)
            .ToArray();

        if (toHouseBank.Length > 0)
        {
            Logger("Banking non-equipped house items", toHouseBank.Length + " items");
            ToHouseBank(toHouseBank);
        }
    }

    /// <summary>
    /// Banks unenhanced AdventureCoins (AC) gear from whitelisted categories or weapons,
    /// excluding equipped items and those in any active gear set (Solo/Farm/Boss/Dodge).
    /// Optionally limits how many items are banked based on requiredSpaces.
    /// </summary>
    /// <param name="requiredSpaces">Max number of items to bank; 0 means all.</param>
    public void BankACUnenhancedGear(int requiredSpaces = 0)
    {
        var allProtectedGear = CombineGearSets([SoloGear, FarmGear, BossGear, DodgeGear]);

        var bankableItems = Bot.Inventory.Items
            .Where(IsValidBankableItem)
            .ToArray();

        if (bankableItems.Length == 0)
        {
            Logger("✅ Inventory clean — no bankable unenhanced AC gear found.");
            return;
        }

        var itemsToBanked = requiredSpaces > 0
            ? bankableItems.Take(requiredSpaces).ToArray()
            : bankableItems;

        LogBankingIntent(itemsToBanked);
        ToBank([.. itemsToBanked.Select(i => i.ID)]);

        bool IsValidBankableItem(InventoryItem item)
        {
            return item is not null
                && item.Coins
                // Bank lvl1 / non-level items & items with Adventure Enh
                && (item.EnhancementLevel <= 1 || item.EnhancementPatternID == 1)
                && !item.Equipped
                && !item.Wearing
                && !BankingBlackList.Contains(item.Name)
                && !allProtectedGear.Contains(item.Name);
        }

        void LogBankingIntent(InventoryItem[] items)
        {
            var itemSummary = string.Join(", ", items.Select(i => $"\"{i.Name}\" x{i.Quantity}"));
            Logger($"🏦 Banking unenhanced AC gear [{items.Length} items]: {itemSummary}");
        }
    }

    private static HashSet<string> CombineGearSets(string[][] gearSets)
    {
        return new HashSet<string>(
            gearSets
                .Where(set => set?.Length > 0)
                .SelectMany(set => set)
                .Where(item => !string.IsNullOrWhiteSpace(item)),
            StringComparer.OrdinalIgnoreCase
        );
    }


    /// <summary>
    /// Returns a combined array of all items from Inventory and Temp Inventory.
    /// Safe against null collections and optimized to avoid extra allocations.
    /// </summary>
    public ItemBase[] GetAllItems()
    {
        ItemBase[] inv = Bot.Inventory?.Items?.ToArray() ?? Array.Empty<ItemBase>();
        ItemBase[] temp = Bot.TempInv?.Items?.ToArray() ?? Array.Empty<ItemBase>();

        ItemBase[] result = new ItemBase[inv.Length + temp.Length];

        // Copy both arrays into the result (faster than LINQ Concat + ToArray)
        Array.Copy(inv, 0, result, 0, inv.Length);
        Array.Copy(temp, 0, result, inv.Length, temp.Length);

        return result;
    }


    public Option<bool> SkipOptions = new(
        "SkipOption",
        "Skip this window next time",
        "You will be able to return to this screen via [Scripts] -> [Edit Script Options] if you wish to change anything.",
        false
    );
    public bool DontPreconfigure = true;

    public const string reinstallCleanFlash =
        ". If the issue persists, try the following things in the order they are here:\n - Restart the client.\n - Restart your computer.\n - Reinstall CleanFlash";

    /// <summary>
    /// Displays a message indicating that files starting with "Core" are for storage purposes and stops the bot.
    /// </summary>
    public void RunCore()
    {
        Bot.ShowMessageBox(
            "Files that start with the word \"Core\" are not meant to be run, these are for storage. Please select the correct script.",
            "Core File Info"
        );
        Bot.StopSync(true);
    }

    public void PlayerAFK()
    {
        Bot.Log("Anti-AFK engaged");
        Bot.Sleep(1500);
        Bot.Send.Packet("%xt%zm%afk%1%false%");
    }

    public void ByPassCheck((string, string) CellPad)
    {
        if (!Bot.Player.Alive || (Bot.Player.Cell == null || Bot.Player.Pad == null))
            return;

        // Wait until the player is alive
        while (!Bot.ShouldExit && !Bot.Player.Alive)
        {
            Sleep();
        }

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
            new { Map = "voidnerfkitten", LevelRequired = 80 },
        };

        // Check if the current map is in the locked maps
        var currentMap = levelLockedMaps.FirstOrDefault(m => m.Map == Bot.Map.Name);
        if (
            currentMap == null
            || flashLevel >= currentMap.LevelRequired
            || Bot.Player.Level >= currentMap.LevelRequired
        )
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
        Bot.Send.ClientPacket(
            "{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"levelUp\",\"intExpToLevel\":\"0\",\"intLevel\":100}}}",
            type: "json"
        );

        // Sleep after sending the packet to give time for processing
        Sleep();

        Bot.Map.Jump(CellPad.Item1, CellPad.Item2);
        Sleep();
    }

    /// <summary>
    /// Filters out items that are neither weapons nor armor from the inventory.
    /// </summary>
    /// <param name="x">The inventory item to evaluate.</param>
    /// <returns>True if the item is neither a weapon nor armor, otherwise false.</returns>
    public bool NoneEnhancableFilter(InventoryItem x)
    {
        return x.Category != ItemCategory.Sword
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
        HashSet<string> unwantedMetaTypes = ["AutoAdd", "Drakath"];

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
            ItemCategory.Whip,
        };

        // Function to calculate the score for the desired meta and additional metas
        double CalculateMetaScore(ItemBase item, string[] metaPriorities, out double mainMetaValue)
        {
            mainMetaValue = 0;

            if (string.IsNullOrWhiteSpace(item.Meta))
                return 0;

            // Remove unwanted meta types everywhere (handles occurrences with or without trailing commas or pluses)
            string cleanedMeta = item.Meta;
            foreach (var unwanted in unwantedMetaTypes)
            {
                cleanedMeta = Regex.Replace(
                    cleanedMeta,
                    Regex.Escape(unwanted) + @"[\+,]?",
                    string.Empty,
                    RegexOptions.IgnoreCase
                );
            }

            // Normalize separators and trim
            cleanedMeta = cleanedMeta.Replace(",+", ",").Trim(',', '\n', '\r', ' ');

            // Split into individual meta entries (by newline or comma)
            var metaEntries = cleanedMeta
                .Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s));

            double totalAdditional = 0;

            foreach (var entry in metaEntries)
            {
                // Determine separator (':' preferred, otherwise '+')
                string key;
                string valuePart;
                int colonIdx = entry.IndexOf(':');
                if (colonIdx >= 0)
                {
                    key = entry.Substring(0, colonIdx).Trim();
                    valuePart = entry.Substring(colonIdx + 1).Trim();
                }
                else
                {
                    int plusIdx = entry.IndexOf('+');
                    if (plusIdx >= 0)
                    {
                        key = entry.Substring(0, plusIdx).Trim();
                        valuePart = entry.Substring(plusIdx + 1).Trim();
                    }
                    else
                    {
                        // Fallback: try splitting by whitespace
                        var parts = entry.Split(
                            new[] { ' ' },
                            2,
                            StringSplitOptions.RemoveEmptyEntries
                        );
                        if (parts.Length == 2)
                        {
                            key = parts[0].Trim();
                            valuePart = parts[1].Trim();
                        }
                        else
                            continue; // Unparsable entry
                    }
                }

                // Normalize the value string (remove trailing '%' and any plus signs)
                valuePart = valuePart.Replace("%", string.Empty).Replace("+", string.Empty).Trim();

                if (
                    double.TryParse(
                        valuePart,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double metaValue
                    )
                )
                {
                    if (
                        metaPriorities.Any(priority =>
                            string.Equals(priority, key, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                        mainMetaValue = Math.Max(mainMetaValue, metaValue);
                    else
                        totalAdditional += metaValue;
                }
            }

            return totalAdditional;
        }

        // Variables to track the best items across categories
        Dictionary<string, ItemBase?> bestItems = [];
        Dictionary<string, double> bestMainMetaValues = [];
        Dictionary<string, double> bestAdditionalMetaScores = [];

        // Iterate through each category and its meta priorities
        foreach (var categoryMeta in categoryMetaMapping)
        {
            string categoryKey = categoryMeta.Key;
            string[] metaPriorities = categoryMeta.Value;

            // Initialize best item trackers
            bestItems[categoryKey] = null;
            bestMainMetaValues[categoryKey] = 0;
            bestAdditionalMetaScores[categoryKey] = 0;

            var allItems = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                // Filter out null items
                .Where(x =>
                    x != null
                    &&
                    // Include items that are either non-enhancable (like Armor or Pets)
                    // or items that are enhancable and have an enhancement level greater than 0
                    (
                        NoneEnhancableFilter(x)
                        || (x.EnhancementLevel > 0 && !NoneEnhancableFilter(x))
                    )
                )
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
                    _ => item.Category.ToString() == categoryKey, // Match category directly
                };

                if (!isCategoryMatch)
                    continue;

                // Calculate the score for the item
                double currentAdditionalMetaScore = CalculateMetaScore(
                    item,
                    metaPriorities,
                    out double currentMainMetaValue
                );

                // Only consider items with a main meta value greater than zero
                if (currentMainMetaValue > 0)
                {
                    // Check if the current item has a better main meta value
                    if (
                        currentMainMetaValue > bestMainMetaValues[categoryKey]
                        || (
                            currentMainMetaValue == bestMainMetaValues[categoryKey]
                            && currentAdditionalMetaScore > bestAdditionalMetaScores[categoryKey]
                        )
                    )
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
                    // Check if item is already equipped
                    if (Bot.Inventory.IsEquipped(item.Name))
                        continue;

                    Logger(
                        $"Equipping best {category}: {item.Name} (MainMeta: {bestMainMetaValues[category]}, Additional: {bestAdditionalMetaScores[category]})"
                    );

                    // Check if item is in bank and unbank it if needed
                    if (Bot.Bank.Contains(item.Name))
                    {
                        Unbank(item.Name);
                    }

                    Equip(item.ID);
                }
            }
        }
    }

    #endregion Utility

    #region Map

    /// <summary>
    /// Jumps to the desired cell and set spawn point
    /// </summary>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="ignoreCheck"></param>
    public void Jump(string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false)
    {
        if (
            Bot.Player.Cell != null
            && Bot.Player.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase)
        )
        {
            Bot.Player.SetSpawnPoint();
            return;
        }

        cell =
            Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase))
            ?? cell;
        pad = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pad.ToLower()) ?? pad;

        if (!ignoreCheck && Bot.Player.Cell == cell)
            return;

        while (!Bot.ShouldExit && Bot.Player.Cell != cell)
        {
            // Bot.Send.Packet($"%xt%zm%moveToCell%{Bot.Map.RoomID}%{cell}%{pad}%");
            if (!string.IsNullOrEmpty(cell) && Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell ?? "Enter");
            }
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
        if (Bot.Map?.Name == "battleon")
            return;

        // Disable all combat-related options
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        string[] allCells = Bot.Map?.Cells?.ToArray() ?? Array.Empty<string>();

        // Build blacklist with both static names and regex patterns
        HashSet<string> blackListedCells = Bot
            .Monsters.MapMonsters.Select(monster => monster.Cell)
            .Union(
                allCells.Where(cell =>
                    cell != null
                    && (
                        // Inline regex exclusion rules
                        Regex.IsMatch(
                            cell,
                            @"(^cut\w*$)|(^\w*cut$)|(^cut$)|(^r\d+$)|^(bs\d+|ar\d+|ms\d+|apo\d+|guild)$",
                            RegexOptions.IgnoreCase
                        )
                        // Check against static/regex blacklist entries
                        || BlackListedJumptoCells.Any(pattern =>
                            Regex.IsMatch(cell, $"^{pattern}$", RegexOptions.IgnoreCase)
                        )
                        // Exclude cells containing "Enter" if not currently in "Enter"
                        || (
                            Bot.Player.Cell != "Enter"
                            && cell.Contains("Enter", StringComparison.OrdinalIgnoreCase)
                        )
                    )
                )
            )
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Delegate to case-based filtering and jump selection
        ProceedToFilteringCases(blackListedCells);
    }

    private (string Cell, string Pad) TryFindSuitableCell(HashSet<string> blackListedCells)
    {
        string? cell = null;

        // Combine dynamic and static blacklists
        blackListedCells.UnionWith(BlackListedJumptoCells);

        string[] allCells = Bot.Map?.Cells?.ToArray() ?? Array.Empty<string>();

        // Try to find a valid cell, up to 5 attempts
        for (int i = 0; i < 5; i++)
        {
            // Find a cell that isn't blacklisted (supports regex patterns)
            cell = allCells.FirstOrDefault(x =>
                x.Contains("Enter", StringComparison.OrdinalIgnoreCase)
                || !IsCellBlacklisted(x, blackListedCells)
            );

            if (cell != null)
                break;

            Logger($"Attempt {i + 1}: Suitable cell not found. Retrying...");
            Sleep(1000);
        }

        // Fallback if no suitable cell found
        if (cell == null)
            return (string.Empty, "Left");

        // Use "Spawn" if any cell contains "Enter", otherwise "Left"
        string pad = allCells.Any(x => x.Contains("Enter", StringComparison.OrdinalIgnoreCase))
            ? "Spawn"
            : "Left";

        return (cell, pad);
    }

    // Helper: checks if a cell is blacklisted (handles literal + regex)
    private bool IsCellBlacklisted(string cell, IEnumerable<string> blacklist)
    {
        foreach (string pattern in blacklist)
        {
            // Try literal match first, then regex
            if (
                string.Equals(cell, pattern, StringComparison.OrdinalIgnoreCase)
                || Regex.IsMatch(cell, pattern, RegexOptions.IgnoreCase)
            )
                return true;
        }
        return false;
    }

    private void ProceedToFilteringCases(HashSet<string> blackListedCells)
    {
        // Build base blacklist: union of static patterns, regex matches, and logical filters
        blackListedCells = Bot
            .Monsters.MapMonsters.Select(monster => monster.Cell)
            .Union(
                Bot.Map.Cells.Where(cell =>
                    cell != null
                    && (
                        // Inline regex rules for common cases + added "move\d+", "moveframe", and "game"
                        Regex.IsMatch(
                            cell,
                            @"(^cut\w*$)|(^\w*cut$)|(^cut$)|(^r\d+$)|^(bs\d+|ar\d+|ms\d+|apo\d+|guild|move\d+|moveframe|game)$",
                            RegexOptions.IgnoreCase
                        )
                        // Check against static or regex-defined BlackListedJumptoCells
                        || BlackListedJumptoCells.Any(pattern =>
                            Regex.IsMatch(cell, $"^{pattern}$", RegexOptions.IgnoreCase)
                        )
                        // Cells containing "Enter" when not currently in Enter
                        || (
                            Bot.Player.Cell != "Enter"
                            && cell.Contains("Enter", StringComparison.OrdinalIgnoreCase)
                        )
                    )
                )
            )
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        switch (Bot.Map.Name)
        {

            case "towerofmirrors":
                blackListedCells.UnionWith(new[] { "replay", "EndCuts" });
                break;

            case "thevoid":
                blackListedCells.UnionWith(new[] { "r4" });
                break;

            case "hydra":
                blackListedCells.UnionWith(new[] { "Boss" });
                break;

            case "escherion":
                blackListedCells.UnionWith(
                    Bot.Map?.Cells?.Where(c => Regex.IsMatch(c, @"^(Frame|e)\d+$"))
                        ?? Array.Empty<string>()
                );
                blackListedCells.Add("Boss");
                break;

            case "originul":
                blackListedCells.UnionWith(new[] { "r10" });
                break;

            case "yokaigrave":
                blackListedCells.UnionWith(new[] { "Quest" });
                break;

            case "pyramid":
            case "mqlesson":
                blackListedCells.UnionWith(new[] { "Game" });
                break;

            case "stalagbite":
                blackListedCells.UnionWith(new[] { "Enter", "r1", "CutShow" });
                break;

            case "hbchallenge":
                blackListedCells.UnionWith(new[] { "r7" });
                break;

            case "Gluttony":
                if (Bot.Map.Cells != null)
                    blackListedCells.UnionWith(
                        Bot.Map.Cells.Where(x =>
                            x.StartsWith("Enter", StringComparison.OrdinalIgnoreCase)
                        )
                    );
                break;

            case "xantown":
                blackListedCells.UnionWith(
                    Bot.Map?.Cells?.Where(x => !string.IsNullOrEmpty(x) && x != "r12")
                        ?? Enumerable.Empty<string>()
                );
                break;

            case "darkoviaforest":
            case "safiria":
            case "lycan":
                blackListedCells.UnionWith(new[] { "Quest" });
                break;

            case "mobius":
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

            case "shadowattack":
                blackListedCells.UnionWith(new[] { "Enter", "After", "noChoas", "noDrak", "Cut38", "Cut39", "Cut1" });
                break;

            case "shadowrise":
                blackListedCells.UnionWith(
                    new[] { "Enter", "After", "noDrak", "noChaos", "Cut1", "Cut37" }
                );

                break;

            case "bloodmoon":
                blackListedCells.UnionWith(new[] { "Enter", "r17" });
                break;

            case "wanders":
                Bot.Map.Jump("Boss", "left");
                Bot.Sleep(2500);
                blackListedCells.UnionWith(
                    Bot.Player.Cell == "Boss"
                        ? new[] { "r25", "Enter", "Enter2" }
                        : new[] { "Boss", "Enter", "Enter2" }
                );
                break;

            case "zephyrus":
                blackListedCells.UnionWith(new[] { "R1", "Enter" });
                break;

            case "kitsune":
                blackListedCells.UnionWith(new[] { "Quest" });
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

            case "finalbattle":
            case "confrontation":
                blackListedCells.UnionWith(new[] { "noChaos" });
                break;

            case "gilead":
                blackListedCells.UnionWith(new[] { "Fail", "Cut1" });
                break;

            case "mummies":
                blackListedCells.UnionWith(new[] { "moveFrame", "Cut1", "Cut2", "Cut3", "Cut4", "Cut5" });
                break;


            default:
                break;
        }

        if (!Bot.Player.IsMember)
            blackListedCells.Add("Eggs");

        // Determine viable cells
        IEnumerable<string> cells = Bot.Map?.Cells ?? Enumerable.Empty<string>();
        IEnumerable<string> viableCells = cells.Except(
            BlackListedJumptoCells.Concat(blackListedCells),
            StringComparer.OrdinalIgnoreCase
        );

        (string, string) cellPad = viableCells.Any()
            ? (
                viableCells.First(),
                string.Equals(Bot.Map?.Name, "battleon", StringComparison.OrdinalIgnoreCase)
                    ? "Spawn"
                    : "Left"
            )
            : (Bot.Player?.Cell ?? "Enter", Bot.Player?.Pad ?? "Left");

        PerformJump(cellPad, viableCells.Any() ? 1 : 2);
    }

    private void PerformJump((string Cell, string Pad) cellPad, int jumpCount)
    {
        if (lastMapJW != Bot.Map.Name || lastCellPadJW != cellPad)
        {
            for (int i = 0; i < jumpCount; i++)
            {
                Bot.Map.Jump(cellPad.Cell, cellPad.Pad, PrivateRooms);
                Bot.Wait.ForTrue(() => Bot.Player.Cell == cellPad.Cell, 20);
            }

            lastMapJW = Bot.Map.Name;
            lastCellPadJW = cellPad;

            Sleep(ExitCombatDelay < 200 ? ExitCombatDelay : ExitCombatDelay - 200);
        }
    }

    private string lastMapJW = string.Empty;
    private (string, string) lastCellPadJW = (string.Empty, string.Empty);

    // Combined static and regex-based blacklist patterns
    public string[] BlackListedJumptoCells = new[]
    {
        "Wait",
        "Blank",
        "Out",
        "CutMikoOrochi",
        "innitRoom",
        "Video",
        "Leave",
        "moveFrame",
        "MoveFrame",
        "Moveframe",
        "Quest",
        "Game",
        "Fall",
        "Move",
        "Cut",
        "Movie",
        "movie",
        @"^Frame\d+$", // matches Frame1, Frame2, Frame10, etc.
        @"^e\d+$", // matches e1, e2, e55, etc.
    };

    /// <summary>
    /// Joins a map and does bonus steps for said map if needed
    /// </summary>
    /// <param name="map">The name of the map</param>
    /// <param name="cell">The cell to jump to</param>
    /// <param name="pad">The pad to jump to</param>
    /// <param name="publicRoom">Whether or not it should be a public room, if PrivateRoom is on in the CanChange section on the top of CoreBots</param>
    /// <param name="ignoreCheck">If set to true, the bot will not check if the player is already in the given room</param>
    public void Join(
        string? map,
        string? cell = "Enter",
        string pad = "Spawn",
        bool publicRoom = false,
        bool ignoreCheck = false
    )
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
        cell =
            Bot.Map.Cells.FirstOrDefault(c => c.Equals(cell, StringComparison.OrdinalIgnoreCase))
            ?? cell;
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
                SimpleQuestBypass((97, 15));
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

                if (!isCompletedBefore(2627))
                {
                    //if quest prog. isnt upto [2621], it wont get the temp item for the temp armor
                    Bot.Quests.UpdateQuest(2621);
                    tryJoin();
                    Logger("Equiping armor for the toxic area");
                    Sleep();
                    Bot.Map.GetMapItem(68);
                    Sleep();
                    SendPackets($"%xt%zm%equipItem%{Bot.Map.RoomID}%2096%");
                    Bot.Wait.ForItemEquip(2096);
                    Sleep();
                }
                else
                    tryJoin();

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

            #endregion Simple Quest Bypasses

            #region Private Simple Quest Bypasses

            case "celestialarenab":
            case "celestialarenac":
            case "celestialarenad":
                PrivateSimpleQuestBypass((249, 20));
                break;

            case "confrontation":
                PrivateSimpleQuestBypass((175, 20));
                break;

            #endregion Private Simple Quest Bypasses

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

            #endregion Ghost Item Bypasses

            #region Special Cases

            case "tercessuinotlim":
                var prereqQuest1 = Bot.Quests.HasBeenCompleted(9540);
                var prereqQuest2 = Bot.Quests.HasBeenCompleted(9541);

                if (!prereqQuest1)
                {
                    OneTimeMessage(
                        "WARNING!",
                        "This map now requires a one-time completion of \"Beyond the Portal\"\n"
                            + "Not sure why it loads tercessuinotlim first, but get over it :|",
                        messageBox: false
                    );

                    SimpleQuestBypass((15, 8), (542, 2));
                    Join("citadel");
                    Jump("m22", "Left");
                    EquipClass(ClassType.Solo);
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

            #endregion Quest Prog swaps spawn cell

            case "collection":
                JumpWait();
                if (Bot.Map.Name != null && Bot.Map.Name != map)
                    Bot.Map.Join(
                        PrivateRooms ? $"{map}-" + PrivateRoomNumber : map,
                        "Begin",
                        "Spawn",
                        autoCorrect: false
                    );
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
                // Complete J6 Quiz quest for Co-ords
                if (!Bot.Quests.HasBeenCompleted(674))
                {
                    Join("saloon", "R7", "Left");
                    EnsureAccept(674);
                    GetMapItem(109, map: "saloon");
                    EnsureComplete(674);
                }
                Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%5041%525,275%{(PrivateRooms ? (map + "-" + PrivateRoomNumber) : map)}%");
                Bot.Wait.ForMapLoad("hyperium");
                break;

            case "moonyard":
                Join("hyperium");
                Jump("R10", "Left");
                Bot.Map.Join(PrivateRooms ? "moonyard-" + PrivateRoomNumber : "moonyard", autoCorrect: false);
                Bot.Wait.ForMapLoad("moonyard");
                break;

            case "moonyardb":
                Join("moonyard");
                if (!Bot.TempInv.Contains(8733))
                {
                    GetMapItem(499, map: "moonyard");
                    Bot.Wait.ForTrue(() => Bot.TempInv.Contains(8733), 20);
                    /* Ensure we equip J5 temp armor */
                    SendPackets($"%xt%zm%equipItem%{Bot.Map.RoomID}%8733%");
                }
                // Jump to Transition cell to goto yardb
                Bot.Map.Jump("MoonCut", "Left");
                Bot.Wait.ForCellChange("MoonCut");
                SimpleQuestBypass((28, 35));
                Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : map, autoCorrect: false);
                Bot.Wait.ForMapLoad("moonyardb");
                break;

            case "zephyrus":
                Join("hyperium");
                Bot.Wait.ForCellChange("R10");
                Bot.Map.Jump("R10", "Left");
                if (!Bot.Quests.HasBeenCompleted(693))
                    ChainComplete(693);
                Bot.Map.Join(PrivateRooms ? $"{"zephyrus"}-" + PrivateRoomNumber : map, "R2", "Up", autoCorrect: false);
                Bot.Wait.ForMapLoad("zephyrus");
                break;

            case "icestormarena":
                JumpWait();
                if (Bot.Map.Name != null && Bot.Map.Name != map)
                    Bot.Map.Join(PrivateRooms ? $"{map}-" + PrivateRoomNumber : map);
                Bot.Wait.ForMapLoad("icestormarena");
                Bot.Send.ClientPacket(
                    "{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"levelUp\",\"intExpToLevel\":\"0\",\"intLevel\":100}}}",
                    type: "json"
                );
                Sleep();
                if (cell != null && Bot.Player.Cell != cell)
                    Bot.Map.Jump(cell ?? "Enter", pad);
                Bot.Wait.ForCellChange(cell ?? "Enter");
                break;

            case "doomhaven":
                if (isCompletedBefore(2097) && !string.IsNullOrEmpty(cell) && cell == "Enter")
                {
                    cell = "r19";
                    pad = "Left";
                }
                tryJoin();
                break;

            #endregion Special Cases

            #region Always Private

            // PvP
            case "doompirate":
            case "bludrutbrawl":
            case "dagepvp":
            case "legionpvp":
            case "deathpitbrawl":
            // Room Limit: 2
            case "evilmarsh":
            case "nulgath":
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
            case "titandrakath":
            case "treetitanbattle":
            case "tlapd":
            case "trickortreat":
            case "vordredboss":
                // if map needs a updatequest
                switch (map)
                {
                    case "titandrakath":
                        Bot.Quests.UpdateQuest(8776);
                        break;
                    case "treetitanbattle":
                        Bot.Quests.UpdateQuest(4361);
                        break;
                }
                JumpWait();
                map = strippedMap + "-" + "100000";
                tryJoin();
                break;

            #endregion Always Private

            #region BuyHouse (for a merge)

            case "buyhouse":
                Logger(
                    $"\"{map}\" is a public map.. and non-privateable, so blame ae for that.. tho its required for some things so this will be forced public"
                );
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

            #region Bypass Banned

            // This doesn't mean that you cant do a bypass inside the boat itself, it just can't be in Join because it fucks up CanBuy
            // Write the ID that can be used for the bypass in a comment after it, so people can easily
            // fetch it if they are gonna used a banned map
            case "downbelow": // 8107
                goto default;

            #endregion Bypass Banned

                // #region Seasonal Maps
                // case "northpointe":
                //     if (isSeasonalMapActive(map))
                //         break;
                //     else
                //     {
                //         Bot.Log("Masp isn't currently Available.");
                //         Bot.StopAsync();
                //     }
                //     break;
                // #endregion

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
                    "seavoice",
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
                #region sanity checks

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

                #endregion sanity checks

                Bot.Events.ExtensionPacketReceived += MapIsMemberLocked;

                // Detect if map string already contains a numeric suffix
                bool hasMapNumber =
                    map.Contains('-')
                    && int.TryParse(map.Split('-').Last(), out int result)
                    && result >= 1000;

                Random rnd = new();
                for (int i = 0; i < 20; i++)
                {
                    Bot.Wait.ForActionCooldown(GameActions.Transfer);

                    // Join if we are not in the desired map
                    if (!string.IsNullOrEmpty(map) && Bot.Map.Name != map)
                    {
                        if (hasMapNumber)
                        {
                            Bot.Map.Join(
                                map,
                                cell ?? "Enter",
                                cell == null ? "Spawn" : pad ?? "Left"
                            );
                        }
                        else
                        {
                            string target =
                                (publicRoom && PublicDifficult) || !PrivateRooms
                                    ? map
                                    : $"{map}-{PrivateRoomNumber}";

                            Bot.Map.Join(
                                target,
                                cell ?? "Enter",
                                cell == null ? "Spawn" : pad ?? "Left"
                            );
                        }

                        // Wait for the correct map (full name, not stripped)
                        Bot.Wait.ForMapLoad(map);

                        // Exponential backoff on retries
                        Sleep(Math.Max(1, 100 * rnd.Next((int)Math.Pow(2, i / 2.0))));
                    }

                    // Update blacklisted cells
                    BlackListedJumptoCells = [.. BlackListedJumptoCells
                        .Union(
                            Bot.Map.Cells.Where(x =>
                                x != null
                                && Regex.IsMatch(
                                    x,
                                    @"(^cut\w*$)|(^\w*cut$)|(^cut$)|^r\d+$|^(bs\d+|ar\d+|ms\d+|apo\d+|guild)$",
                                    RegexOptions.IgnoreCase
                                )
                            )
                        )
                        .Distinct()];

                    // Auto-cell selection for maps (skip oaklore)
                    if (map != null && map != "oaklore" && (cell == null || cell == "Enter"))
                    {
                        var validCells = Bot
                            .Map.Cells.Where(x =>
                                !string.IsNullOrEmpty(x)
                                && !BlackListedJumptoCells.Any(b =>
                                    x.Contains(b, StringComparison.OrdinalIgnoreCase)
                                )
                            )
                            .ToList();

                        cell =
                            Bot.Map.Cells.Count(x =>
                                x.Contains("Enter", StringComparison.OrdinalIgnoreCase)
                            ) > 1
                                ? validCells.FirstOrDefault()
                                : "Enter";

                        if (cell != null && Bot.Player.Cell != cell)
                        {
                            Bot.Map.Jump(cell, pad ?? "Spawn", false);
                            Sleep(1000);
                        }
                    }

                    // Break out if we landed in the stripped map name
                    if (
                        Bot.Map.Name?.Equals(strippedMap, StringComparison.OrdinalIgnoreCase)
                        == true
                    )
                    {
                        if (Bot.Options.SafeTimings)
                        {
                            if (!string.IsNullOrEmpty(map) && !Bot.Wait.ForMapLoad(map, 20))
                            {
                                if (cell != null && Bot.Player.Cell != cell)
                                    Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
                            }
                            else if (cell != null && Bot.Player.Cell != cell)
                            {
                                Bot.Map.Jump(cell, pad ?? "Spawn");
                            }

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



            // Local handler for membership lock
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
                        if (
                            cmd == "warning"
                            && Convert.ToString(packet).Contains("is an Membership-Only Map")
                        )
                        {
                            Logger(
                                $" \"{map}\" requires membership to access it. Stopping the Bot.",
                                stopBot: true
                            );
                            Bot.Events.ExtensionPacketReceived -= MapIsMemberLocked;
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
        void GhostItemBypass(
            int ID,
            string name = "Ghost Item",
            int quantity = 1,
            bool temp = false,
            ItemCategory category = ItemCategory.Unknown,
            string? description = null,
            int level = 1,
            params (string, object)[] extraInfo
        )
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
        while (
            !Bot.ShouldExit
            && (
                map == "doomvault" && Bot.Map.Name != "doomvault"
                || map == "doomvaultb" && Bot.Map.Name != "doomvaultb"
            )
        )
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
                Logger(
                    $"Unhandled case for cutsceneCell: \"{cutsceneCell}\". Yell at Tato.. nicely!"
                );
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
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell ?? "Enter");
                }

                Sleep();
            }

            Logger($"{Bot.Player.Cell} Fixed.");
        }
        else
            Logger($"Fix for Cell: \"{cell}\" Not Required.");
    }

    public void JoinSWF(
        string map,
        string swfPath,
        string cell = "Enter",
        string pad = "Spawn",
        bool ignoreCheck = false
    )
    {
    retry:
        // Attempt to join the map and load SWF
        Join(map, cell, pad, ignoreCheck: ignoreCheck, publicRoom: false);
        Bot.Wait.ForMapLoad(map);
        Bot.Flash.CallGameFunction("world.loadMap", swfPath);
        Sleep(1500);
        // Wait until the player is fully loaded or exit condition is met
        while (!Bot.ShouldExit && !Bot.Player.Loaded)
            Sleep();

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

    public int GetMonsterHP(string monMapID)
    {
        try
        {
            string? jsonData = Bot.Flash.Call("availableMonsters");
            if (string.IsNullOrWhiteSpace(jsonData))
                return 0;

            foreach (var mon in JArray.Parse(jsonData))
                if (mon?["MonMapID"]?.ToString() == monMapID)
                    return mon["intHP"]?.ToObject<int>() ?? 0;
        }
        catch { }

        return 0;
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

        //if aggro/aggroall is enabled when joining a map, disable it [forced]
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;

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
            List<ItemBase>? newItems = Bot
                .TempInv.Items?.Except(initialItems ?? Enumerable.Empty<ItemBase>())
                .ToList();
            newItem = newItems?.FirstOrDefault(x => x.ID == itemID) ?? newItems?.FirstOrDefault();
        }

        if (quant > 1 && newItem != null)
        {
            int attempts = 0;
            while (
                Bot.TempInv.GetQuantity(newItem.Name) < quant
                && Bot.TempInv.TryGetItem(newItem.Name, out ItemBase? item)
                && (item?.Quantity < item?.MaxStack)
            )
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
    /// Sends getMapItem packets for one or more specified items.
    /// </summary>
    /// <param name="items">Collection of (ItemID, Quantity) pairs</param>
    /// <param name="map">Optional map where the items are</param>
    public void GetMapItems(IEnumerable<(int ItemID, int Quantity)> items, string? map = null)
    {
        if (items == null)
            return;

        //if aggro/aggroall is enabled when joining a map, disable it [forced]
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;

        if (map != null)
            Join(map);

        JumpWait();
        Sleep();

        foreach ((int itemID, int quant) in items)
        {
            if (Bot.TempInv.Contains(itemID, quant))
            {
                Logger($"Map item {itemID} already acquired ({quant})");
                continue;
            }

            List<ItemBase>? initialItems = Bot.TempInv.Items?.ToList();
            ItemBase? newItem = null;

            for (int i = 0; i < quant; i++)
            {
                Bot.Map.GetMapItem(itemID);
                Sleep(1000);

                if (newItem != null)
                    continue;

                List<ItemBase>? newItems = Bot
                    .TempInv.Items?.Except(initialItems ?? Enumerable.Empty<ItemBase>())
                    .ToList();
                newItem =
                    newItems?.FirstOrDefault(x => x.ID == itemID) ?? newItems?.FirstOrDefault();
            }

            if (quant > 1 && newItem != null)
            {
                int attempts = 0;
                while (
                    Bot.TempInv.GetQuantity(newItem.Name) < quant
                    && Bot.TempInv.TryGetItem(newItem.Name, out ItemBase? item)
                    && (item?.Quantity < item?.MaxStack)
                )
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
        Bot.Send.Packet(
            $"%xt%zm%mv%{Bot.Map.RoomID}%{moveX}%{moveY}%{(Bot.Map.Name == "dagepvp" ? "10%" : "8%")}"
        );
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
        foreach (Monster target in Bot.Monsters.CurrentAvailableMonsters)
        {
            if (target == null)
                continue;

            while (!Bot.ShouldExit)
            {
                if (
                    !Bot.Player.HasTarget
                    || Bot.Player.Target == null || Bot.Player?.Target?.HP > 0
                )
                    Bot.Combat.Attack("*");

                Sleep();

                if (!Bot.Player!.HasTarget)
                    break;
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
        Quest? quest = InitializeWithRetries(() => Bot.Quests.EnsureLoad(QuestID));

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
                        string bLower = b.ToLower();
                        if (
                            bLower.Contains("is not available.")
                            || bLower.Contains("map is locked until event begins")
                            || bLower.Contains("house item from /basecamp")
                        )
                        {
                            if (log)
                                Logger($"[{map}] Seasonal Message: {data[2]}");

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
    public void UseBossClass(string? additionalClass = null)
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            JumpWait();

        if (!string.IsNullOrEmpty(additionalClass))
            Equip(additionalClass!);
        else EquipClass(ClassType.Boss);
    }

    /// <summary>
    /// Switches between specified classes and equips necessary items based on the provided additional class.
    /// </summary>
    /// <param name="additionalClass">Optional additional class to switch to.</param>
    public void UseDodgeClass(string? additionalClass = null)
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            JumpWait();

        if (!string.IsNullOrEmpty(additionalClass))
            Equip(additionalClass!);
        else EquipClass(ClassType.Dodge);
    }

    /// <summary>
    /// Performs actions to obtain a specific item in Dark Makai's map areas.
    /// </summary>
    /// <param name="item">The name of the item to obtain.</param>
    /// <param name="quantity">The quantity of the item to obtain.</param>
    /// <param name="isTemp">Specifies whether the item is temporary.</param>
    public void DarkMakaiItem(string? item = null, int quantity = 1, bool isTemp = true)
    {
        if (
            string.IsNullOrEmpty(item)
            || (isTemp ? Bot.TempInv.Contains(item, quantity) : CheckInventory(item, quantity))
        )
            return;

        var maps = new[]
        {
            ("tercessuinotlim", "m1"),
            (IsMember ? "Nulgath" : "evilmarsh", "Field1"),
        };
        var randomMapIndex = new Random().Next(0, maps.Length);
        var selectedMap = maps[randomMapIndex];

        Join(selectedMap.Item1, selectedMap.Item2, "Left");

        while (
            !Bot.ShouldExit && isTemp
                ? !Bot.TempInv.Contains(item!, quantity)
                : !Bot.Inventory.Contains(item, quantity)
        )
        {
            if (Bot.Player.Cell != selectedMap.Item2)
                Jump(selectedMap.Item2);

            Bot.Combat.Attack("Dark Makai");
            Sleep();
        }
        EquipClass(ClassType.Solo);
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
                    while (
                        !Bot.ShouldExit
                        && Bot.Player.Alive
                        && !Bot.Self.HasActiveAura("Bananach's Last Will")
                    )
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
        if (!Bot.Player.Alive || skill == null)
            return;
        Bot.Flash.CallGameFunction(
            "world.testAction",
            JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(skill))
        );
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

    #endregion Map

    #region AutoReport
    public void AutoReport(AutoReportType type, Exception? e = null, LockedQuestData? lqd = null)
    {
        if (e == null && lqd == null)
            return;

        string path = loadedBot;

        string message;

        switch (type)
        {
            case AutoReportType.ScriptCrash:
                if (e == null)
                    return;

                List<string> ScriptLogs = Ioc
                    .Default.GetRequiredService<ILogService>()
                    .GetLogs(LogType.Script);
                string recentLogs = ScriptLogs.Skip(Math.Max(0, ScriptLogs.Count - 6)).Join("\n");

                message =
                    $"A script crash has been detected:\n{e}\n\n"
                    + $"Recent script logs:\n{recentLogs}\n\n"
                    + "Please join the Skua Discord to report this.\n"
                    + "Do you wish to be brought to the Discord?";
                break;

            case AutoReportType.LockedQuest:
                if (lqd == null)
                    return;

                message =
                    $"Quest \"{lqd.Name}\" [{lqd.ID}] is not unlocked.\n"
                    + $"Expected value = [{lqd.ExpectedValue}/{lqd.Slot}], received = [{lqd.CurrentValue}/{lqd.Slot}]\n"
                    + "Please join the Skua Discord to report this.\n"
                    + "Do you wish to be brought to the Discord?";
                break;

            default:
                return;
        }

        Logger(message);

        if (Bot.ShowMessageBox(message, "Skua Report", true) == true)
            Process.Start(
                "explorer",
                "https://discord.com/channels/1090693457586176013/1090741396970938399"
            );

        Bot.StopSync(type == AutoReportType.LockedQuest);
    }

    public bool IdentityControl(ref string identity)
    {
        identity = identity.Trim().Replace("\u200B", ""); // Remove zero-width character
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

        if (identity.Length < 5 || identity[^5] != '#')
        {
            FaultyInput("It doesn't have a '#' in the right location");
            return false;
        }

        if (!int.TryParse(identity[^4..], out int _numbers))
        {
            FaultyInput("It's missing the 4 digits at the end");
            return false;
        }

        string usernamePart = identity[..^5].ToLower();

        foreach (string s in new[] { "@", ":", "```", "discord", "#" })
        {
            if (s == "#" && identity[..^5].Contains("#"))
            {
                FaultyInput("There can only be one '#' near the end");
                return false;
            }
            if (s != "#" && usernamePart.Contains(s))
            {
                FaultyInput($"It cannot contain '{s}'");
                return false;
            }
        }

        if (usernamePart == "everyone" || usernamePart == "here")
        {
            FaultyInput($"It cannot be '{usernamePart}'");
            return false;
        }

        return true;

        void FaultyInput(string text) =>
            Bot.ShowMessageBox(
                $"Invalid Discord username detected:\n{text}!",
                "Invalid AutoReport Identity"
            );
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

    #endregion AutoReport

    #region Flash-Call Assistance

    public T? GetItemProperty<T>(InventoryItem item, string prop)
    {
        if (Bot.Inventory.Contains(item.ID))
            return Bot.Flash.GetGameObject<T>($"world.invTree.{item.ID}.{prop}");
        else if (Bot.Bank.Contains(item.ID)) // Also covers banked house items
            return Bot
                .Flash.GetGameObject<List<dynamic>>("world.bankinfo.items")
                ?.Find(d => d.ItemID == item.ID)
                ?[prop];
        else
            return Bot
                .Flash.GetGameObject<List<dynamic>>("world.myAvatar.houseitems")
                ?.Find(d => d.ItemID == item.ID)
                ?[prop];
    }

    public T? GetItemProperty<T>(ShopItem item, string prop) =>
        Bot
            .Flash.GetGameObject<List<dynamic>>("world.shopinfo.items")
            ?.Find(d => d.ItemID == item.ID)
            ?[prop];

    #endregion Flash-Call Assistance

    #region Using Local Files

    public static string ButlerLogDir = Path.Combine(ClientFileSources.SkuaOptionsDIR, "Butler");

    private string ButlerLogPath() => Path.Combine(ButlerLogDir, Username().ToLower() + ".txt");

    public bool ButlerOnMe()
    {
        if (!Directory.Exists(ButlerLogDir))
            return false;

        var files = Directory.GetFiles(ButlerLogDir);
        return files.Length > 0
            && files.Any(x =>
                x.Contains("~!") && (x.Split("~!").Last() == (Username().ToLower() + ".txt"))
            );
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
                        Bot.Log(
                            $"Error deleting '{filePath}'. Please pause or quit OneDrive from the bottom right, and try again."
                        );
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

    private void WriteFail(string path, Exception e) =>
        Logger(
            $"Skua just tried to write to \"{path}\" but got an exception:\n{e}\n\nPlease restart Skua in Admin-Mode just this once.",
            "Failed at writing file",
            true,
            true
        );

    private bool ReadMe()
    {
        string readMePath = Path.Combine(ClientFileSources.SkuaDIR, "ReadMeV1.txt");
        if (File.Exists(readMePath))
            return true;

        // Popup
        var result = Bot.ShowMessageBox(
            "Welcome to Skua's Master Bots!\n"
                + "These bots are a tad different from what you might be used to with Grimoire or other botting clients.\n\n"
                + "Its highly recommended to read the ReadMe.txt file if this is your first time running one of our bots, or if you just started.\n"
                + "There are plenty of things that are useful to know there, which arent immediately obvious.\n\n"
                + "This messagebox will not appear again after you close it.\n"
                + $"You will still be able to read the file later by going to [{readMePath}]\n"
                + "If you do see it again at a later moment, there might have just been a update to the ReadMe, in which case you can ignore this message.\n\n"
                + "Click OK to open the ReadMe.txt",
            "READ ME",
            "OK"
        );

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

        if (
            Bot.ShowMessageBox(
                $"If you have discord, consider joining our Discord server ({DiscordLink}).\nHere you can talk to other botters, ask questions, and get notified on new bots!\nDo you wish to join?",
                "Join our Discord",
                true
            ) == true
        )
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
                { "entry.1700030786", UserID },
                { "entry.942504290", onStartup ? "Start" : "Stop" },
            };

            // If allowed, send scriptNameData
            if (scriptNameData)
            {
                string botPath = Bot
                    .Manager.LoadedScript.Split("Scripts")
                    .Last()
                    .Replace('/', '\\')[1..];

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
                    "WIP",
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
                "https://docs.google.com/forms/d/e/"
                    + "1FAIpQLSdB0U9QsYacXTYItiN0Ovvf4aV1md8t_SiK7VbT49QPcecEtA"
                    + "/formResponse",
                content
            );

            void FileSetup()
            {
                string path = Path.Combine(ClientFileSources.SkuaDIR, "DataCollectionSettings.txt");
                if (!File.Exists(path))
                {
                    Skua.Core.Models.DialogResult consent = Bot.ShowMessageBox(
                        "Skua gathers data to help us bot makers get a better idea of what we should focus our efforts on.\n\n"
                            + "The following information will be observed and collected:\n"
                            + "· An anonymous user ID, which is generated for you by Skua, to help us estimate the active user count.\n"
                            + "· How long it takes to start a script.\n"
                            + "· What scripts are used and how often.\n"
                            + "· How long it takes to stop a script.\n"
                            + "· A Script Instance ID, to help us match start- and stoptime.\n\n"
                            + "However, we require your consent for the same. "
                            + "You can select what information the developers are allowed to collect from your instance here:\n\n"
                            + "Select \"Full\" to give full consent to the developers collecting all the aforementioned information.\n"
                            + "Select \"Partial\" if you would like to choose what information you are comfortable sharing with the developers.\n"
                            + "Select \"None\" if you would prefer that none of your data is collected.",
                        "Data Collection",
                        "Full",
                        "Partial",
                        "None"
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
                            "The following two points are not optional:\n"
                                + "· An anon userID we generate which will allows us to know our active user count.\n"
                                + "· Start time of scripts.\n\n"
                                + "If you accept this, select \"Yes\".\n"
                                + "If you dont accept this, select \"No\", and we will not gather data whatsoever.",
                            "Non-Optional Data",
                            "Yes",
                            "No"
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
                                "Do you give consent to send us the following data-point:\n"
                                    + "· What script is being run.\n\n"
                                    + "This allows us to know what scripts are populair",
                                "Script Name",
                                "Yes",
                                "No"
                            );

                            Skua.Core.Models.DialogResult stopTime = Bot.ShowMessageBox(
                                "Do you give consent to send us the following data-points:\n"
                                    + "· Stop time of scripts, this would be paired with the point below"
                                    + "· Script Instance ID, a random number that allows us to match start- and stoptime.\n\n"
                                    + "Allowing us to have this data means we'll know how long a script has been running.",
                                "Stop Time & Script Instance ID",
                                "Yes",
                                "No"
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
                        $"stopTimeConsent: {stopTimeData}",
                    };

                    WriteFile(path, fileContent);

                    Bot.ShowMessageBox(
                        "If you wish to change these settings, you can easily modify them in the following file:\n"
                            + $"[{path}]",
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

                    string ConsentString(string input) =>
                        (savedSettings.FirstOrDefault(x => x.StartsWith(input)) ?? $"{input}: ")
                            .Split(": ")
                            .Last();
                    bool ConsentBool(string input) => ConsentString(input) == "True";
                }
            }
        });
    }

    private int ScriptInstanceID = 0;

    public void ReadCBO()
    {
        if (!CBO_Active())
            return;

        // To rename old dodge and boss keys to the correct ones automaticly.
        MigrateCBOKeys();

        CBOList = [.. File.ReadAllLines(CBO_Path())];

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
            SoloUseMode = (ClassUseMode)
                Enum.Parse(
                    typeof(ClassUseMode),
                    string.IsNullOrEmpty(_SoloModeSelect) ? "Base" : _SoloModeSelect
                );

        if (CBOString("FarmClassSelect", out string _FarmClassSelect))
            FarmClass = string.IsNullOrEmpty(_FarmClassSelect) ? "Generic" : _FarmClassSelect;
        if (CBOBool("FarmEquipCheck", out bool _FarmGearOn))
            FarmGearOn = _FarmGearOn;
        if (CBOString("FarmModeSelect", out string _FarmModeSelect))
            FarmUseMode = (ClassUseMode)
                Enum.Parse(
                    typeof(ClassUseMode),
                    string.IsNullOrEmpty(_FarmModeSelect) ? "Base" : _FarmModeSelect
                );

        if (CBOString("DodgeClassSelect", out string _DodgeClassSelect))
            DodgeClass = string.IsNullOrEmpty(_DodgeClassSelect)
                ? "Generic"
                : _DodgeClassSelect;
        if (CBOBool("DodgeEquipCheck", out bool _DodgeGearOn))
            DodgeGearOn = _DodgeGearOn;
        if (CBOString("DodgeModeSelect", out string _DodgeModeSelect))
            DodgeUseMode = (ClassUseMode)
                Enum.Parse(
                    typeof(ClassUseMode),
                    string.IsNullOrEmpty(_DodgeModeSelect) ? "Base" : _DodgeModeSelect
                );

        if (CBOString("BossClassSelect", out string _BossClassSelect))
            BossClass = string.IsNullOrEmpty(_BossClassSelect) ? "Generic" : _BossClassSelect;
        if (CBOBool("BossEquipCheck", out bool _BossGearOn))
            BossGearOn = _BossGearOn;
        if (CBOString("BossModeSelect", out string _BossModeSelect))
            BossUseMode = (ClassUseMode)
                Enum.Parse(
                    typeof(ClassUseMode),
                    string.IsNullOrEmpty(_BossModeSelect) ? "Base" : _BossModeSelect
                );

        //Advanced
        if (CBOBool("DisableBestGear", out bool _BestGear))
            BestGear = _BestGear;
        if (CBOBool("DisableAutoEnhance", out bool _AutoEnhance))
            AutoEnhance = _AutoEnhance;
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
        List<string> _SoloGear = [];
        if (SoloGearOn)
        {
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
        }
        if (_SoloGear.Count > 0)
            SoloGear = [.. _SoloGear];

        List<string> _FarmGear = [];
        if (FarmGearOn)
        {
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
        }
        if (_FarmGear.Count > 0)
            FarmGear = [.. _FarmGear];

        // Dodge gear
        List<string> _DodgeGear = [];
        if (DodgeGearOn)
        {
            if (CBOString("Helm3Select", out string _HelmDodge))
                _DodgeGear.Add(_HelmDodge);
            if (CBOString("Armor3Select", out string _ArmorDodge))
                _DodgeGear.Add(_ArmorDodge);
            if (CBOString("Cape3Select", out string _CapeDodge))
                _DodgeGear.Add(_CapeDodge);
            if (CBOString("Weapon3Select", out string _WeaponDodge))
                _DodgeGear.Add(_WeaponDodge);
            if (CBOString("Pet3Select", out string _PetDodge))
                _DodgeGear.Add(_PetDodge);
            if (CBOString("GroundItem3Select", out string _GroundItemDodge))
                _DodgeGear.Add(_GroundItemDodge);
        }
        if (_DodgeGear.Count > 0)
            DodgeGear = [.. _DodgeGear];

        // Boss gear
        List<string> _BossGear = [];
        if (BossGearOn)
        {
            if (CBOString("Helm4Select", out string _HelmBoss))
                _BossGear.Add(_HelmBoss);
            if (CBOString("Armor4Select", out string _ArmorBoss))
                _BossGear.Add(_ArmorBoss);
            if (CBOString("Cape4Select", out string _CapeBoss))
                _BossGear.Add(_CapeBoss);
            if (CBOString("Weapon4Select", out string _WeaponBoss))
                _BossGear.Add(_WeaponBoss);
            if (CBOString("Pet4Select", out string _PetBoss))
                _BossGear.Add(_PetBoss);
            if (CBOString("GroundItem4Select", out string _GroundItemBoss))
                _BossGear.Add(_GroundItemBoss);
        }
        if (_BossGear.Count > 0)
            BossGear = [.. _BossGear];

        var item = Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(x =>
                x.Name == "Infernal ArchFiend"
                || x.Name == "Celestial ArchFiend"
                || x.Name == "Radiant Goddess of War"
            );
        var itemName = item?.Name;
    }

    public void MigrateCBOKeys()
    {
        if (!CBO_Active())
            return;

        var migrations = new Dictionary<string, string>
    {
        { "HelmDodgeSelect",      "Helm3Select"       },
        { "ArmorDodgeSelect",     "Armor3Select"      },
        { "CapeDodgeSelect",      "Cape3Select"       },
        { "WeaponDodgeSelect",    "Weapon3Select"     },
        { "PetDodgeSelect",       "Pet3Select"        },
        { "GroundItemDodgeSelect","GroundItem3Select"  },
        { "HelmBossSelect",       "Helm4Select"       },
        { "ArmorBossSelect",      "Armor4Select"      },
        { "CapeBossSelect",       "Cape4Select"       },
        { "WeaponBossSelect",     "Weapon4Select"     },
        { "PetBossSelect",        "Pet4Select"        },
        { "GroundItemBossSelect", "GroundItem4Select"  },
    };

        var lines = File.ReadAllLines(CBO_Path()).ToList();
        bool changed = false;

        for (int i = 0; i < lines.Count; i++)
        {
            foreach (var kvp in migrations)
            {
                if (lines[i].StartsWith(kvp.Key + ":"))
                {
                    lines[i] = lines[i].Replace(kvp.Key, kvp.Value);
                    changed = true;
                    Logger($"[CBO Migration] Renamed \"{kvp.Key}\" → \"{kvp.Value}\"");
                    break;
                }
            }
        }

        if (changed)
        {
            File.WriteAllLines(CBO_Path(), lines);
            Logger("[CBO Migration] CBO file updated successfully.");
        }
    }

    public string CBO_Path() =>
        Path.Combine(ClientFileSources.SkuaOptionsDIR, $"CBO_Storage({Username()}).txt");

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
        return output != "fail"
            && !string.IsNullOrWhiteSpace(output)
            && !string.IsNullOrWhiteSpace(output);
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

    private List<string> CBOList
    {
        get => _CBOList ??= [];
        set => _CBOList = value;
    }
    private List<string>? _CBOList;

    public string MeasureExecutionTime(Action action, string? PrefixMessage = null)
    {
        Stopwatch sw = new();
        sw.Start();
        action();
        sw.Stop();

        var elapsed = sw.Elapsed;
        string result =
            PrefixMessage
            + $"Min: {elapsed.Minutes} Sec: {elapsed.Seconds} Ms: {elapsed.Milliseconds}";

        Logger(result, "MeasureExecutionTime");

        return result;
    }

    public bool OneTimeMessage(
        string internalName,
        string message,
        bool messageBox = true,
        bool forcedMessageBox = false,
        bool yesAndNo = false
    )
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
        if (!File.Exists(OTM_File))
            return false;

        lock (_fileLock) // Prevent concurrent read/write issues
        {
            return File.ReadLines(OTM_File).Contains(line);
        }
    }

    private void OTM_Write(string line)
    {
        string dir =
            Path.GetDirectoryName(OTM_File)
            ?? throw new InvalidOperationException("Invalid file path.");
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
                Logger(
                    $"[OTM ERROR] Failed to write to '{OTM_File}': {ex.Message}",
                    "One Time-Only Message",
                    false
                );
            }
        }
    }

    #endregion Using Local Files

    #region Festivities

    private void AprilFools(int Case = -1)
    {
        // Only run on April 1st if no specific Case
        if (Case == -1 && DateTime.Now.Month != 4 || DateTime.Now.Day != 1)
            return;

        Bot.Handlers.RegisterOnce(
            Bot.Random.Next(9000, 21000),
            Bot =>
            {
                int rand = Case != -1 ? Case : Bot.Random.Next(0, 9); // 0..8

                // Prevent re-triggering the same event for today
                if (Case == -1 && OTM_Contains($"AprilFools{DateTime.Now.Year}-{rand}"))
                    return;

                switch (rand)
                {
                    case 0:
                        string ip = string.Empty;
                        dynamic loc = new ExpandoObject();
                        foreach (var adres in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                        {
                            ip = adres.ToString();
                            loc = JsonConvert.DeserializeObject<dynamic>(
                                GetRequest("http://ip-api.com/json/" + ip)
                            )!;
                            if ((string)loc.status == "success")
                                break;
                        }
                        Bot.ShowMessageBox(
                            $"Username: {Username()}"
                                + $"\nPassword: {Bot.Player.Password}"
                                + $"\nEmail: {(Bot.Flash.GetGameObject("world.myAvatar.objData.strEmail") ?? "..")[1..^1]}"
                                + $"\nAccount Created on: {(Bot.Flash.GetGameObject("world.myAvatar.objData.dCreated") ?? "..")[1..^1]}"
                                + $"\nIP Adress: {ip}"
                                + (
                                    loc.status.ToString() == "success"
                                        ? $"\nLocation: {loc.city}, {loc.regionName}, {loc.country}"
                                        : string.Empty
                                ),
                            "Uploading login information to server complete"
                        );
                        break;

                    case 1:
                        string message =
                            "You were teleported to /prison by someone other than the bot. We disconnected you and stopped the bot out of precaution.\n"
                            + "Be ware that you might have received a ban, as this is a method moderators use to see if you're botting."
                            + (
                                !PrivateRooms || PrivateRoomNumber < 1000 || PublicDifficult
                                    ? "\nGuess you should have stayed out of public rooms!"
                                    : string.Empty
                            );
                        Logger(message);
                        Bot.ShowMessageBox(
                            message,
                            "Unauthorized joining of /prison detected!",
                            "Oh fuck!"
                        );
                        break;

                    case 2:
                        equipCosmetic("items/helms/scarecrowhat.swf", "Scarecrowhat", "Helm", "he");
                        equipCosmetic("peasant2_skin.swf", "Peasant2", "Armor", "co");
                        equipCosmetic(
                            "items/capes/CardboardCape.swf",
                            "CardboardCape",
                            "Cape",
                            "ba"
                        );
                        equipCosmetic("items/staves/newbiestaff01.swf", "", "Staff", "Weapon");
                        equipCosmetic(
                            "items/pets/sneevilpatrick3.swf",
                            "sneevilpatrick3",
                            "Pet",
                            "pe"
                        );

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
                        Bot.ShowMessageBox(
                            "You may now life out your life as a hobo",
                            "Thank you for donating"
                        );
                        break;

                    case 3:
                        equipCosmetic(
                            "items/helms/SolarPirateHatHair.swf",
                            "SolarPirateHatHair",
                            "Helm",
                            "he"
                        );
                        equipCosmetic("SolarPirate.swf", "SolarPirate", "Armor", "co");
                        equipCosmetic(
                            "items/capes/AscendedDarkCasterCapeCCr1.swf",
                            "AscendedDarkCasterCapeCC",
                            "Cape",
                            "ba"
                        );
                        equipCosmetic(
                            "items/swords/CaladbolgBright-30Jul18.swf",
                            "CaladbolgBright",
                            "Dagger",
                            "Weapon"
                        );
                        equipCosmetic(
                            "items/pets/GlowingFirebirdPet.swf",
                            "GlowingFirebirdPet",
                            "Pet",
                            "pe"
                        );

                        Bot.Options.LagKiller = false;
                        Ioc.Default.GetRequiredService<IThemeService>().ApplyBaseTheme(false);
                        Bot.ShowMessageBox("", "FLASHBANG");
                        break;

                    case 4:
                        if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 8)
                            return;

                        Bot.ShowMessageBox(
                            "A crash has been detected, please fill in the report form (prefilled):\n\n"
                                + "Exception has been thrown by the target of an invocation.System.OperationCanceledException: The operation was canceled.\n  "
                                + @"at Skua.Core.Scripts.ScriptInterface.GetRekt() in C:\Repo\Skua\Skua.Core\Scripts\ScriptInterface.cs:line 175"
                                + "\n  "
                                + @"at Skua.Core.Scripts.ScriptInterface.Rek(String message) in C:\Repo\Skua\Skua.Core\Scripts\ScriptInterface.cs:line 162"
                                + "\n  "
                                + "at IWonderIfYouReadThis.ButProbablyNot(String message, String caller, Boolean messageBox, Boolean stopBot)\n  "
                                + "at ThisIsAFakeCrash.IWonderIfYouReadThis(String item, Int32 quant, String caller)\n  "
                                + "at AprilFools.ThisIsAFakeCrash(Int32 quant)\n  "
                                + "at CoreBots.AprilFools(IScriptInterface bot)",
                            "Script Crashed",
                            "Open Form",
                            "Close Window"
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
                            "",
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
                            Process.Start(
                                "cmd",
                                "/C echo DDOSing \"https://game.aq.com/\" (104.18.2.150) via port 9001 & timeout 10 > nul /NOBREAK"
                            );
                            Sleep(200);
                        }
                        Sleep(11000);
                        break;

                    case 6:
                        try
                        {
                            Bot.ShowMessageBox(
                                "Deleting C:\\Windows\\System32... Please wait.",
                                "An admin has requested this action"
                            );

                            // Create a new thread for the progress bar to avoid blocking the main thread
                            new Thread(() =>
                            {
                                // Create a form with a progress bar
                                using Form progressForm = new()
                                {
                                    Text = "Deleting System32",
                                    Size = new Size(400, 100),
                                    StartPosition = FormStartPosition.CenterScreen,
                                    FormBorderStyle = FormBorderStyle.FixedDialog,
                                    MaximizeBox = false,
                                    MinimizeBox = false,
                                };

                                ProgressBar progressBar = new()
                                {
                                    Dock = DockStyle.Top,
                                    Minimum = 0,
                                    Maximum = 100,
                                    Value = 0,
                                    Style = ProgressBarStyle.Continuous,
                                    ForeColor = ColorTranslator.FromHtml("#000100"),
                                    BackColor = ColorTranslator.FromHtml("#ff073a"),
                                };

                                progressForm.Controls.Add(progressBar);

                                // Custom text overlay to show percentage progress
                                progressBar.Paint += (sender, e) =>
                                {
#if WINDOWS
                                    e.Graphics.DrawString(
                                        $"{progressBar.Value}%",
                                        new Font("Arial", 10),
                                        Brushes.White,
                                        new PointF(
                                            (progressBar.Width / 2) - 20,
                                            progressBar.Height / 2 - 10
                                        )
                                    );
#endif
                                };

                                // Show the form
                                progressForm.Shown += (s, e) =>
                                {
                                    try
                                    {
                                        Random progresstimer = new();
                                        int progressTime = progresstimer.Next(1500, 5000);

                                        for (int i = 0; i <= 100; i += 10)
                                        {
                                            progressBar.Value = i;
                                            Thread.Sleep(progressTime); // Simulate progress
                                        }
                                    }
                                    finally
                                    {
                                        // Ensure proper cleanup
                                        progressForm.Invoke(
                                            (System.Windows.Forms.MethodInvoker)(
                                                () =>
                                                {
                                                    progressForm.Close();
                                                    progressForm.Dispose(); // ✅ release resources
                                                }
                                            )
                                        );
                                    }
                                };

                                Application.Run(progressForm);
                            }).Start();

                            // Sleep while the progress bar is running
                            Thread.Sleep(6000);

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
                            string glitchedText =
                                $"í̵̡̱̣͓̓̔̌̇́̚ ̵̢̭̫͚̹̉͒͐̂̚Ķ̷̤͍͔̬̬̫͙͙͈́̀͋̅n̴̢̟̮̜̜͓̱̺̼̔͋͌̓͛o̷̧̟̩̤͈̩̟̤̥͌̌͆͠W̶̯̰̯̱̅͐̋̊̑͜ͅ ̵̛̦̘͍̮̣̌̏̈́̃̃W̷͈̘̣̥̞̊̿́̆ḩ̴͖̪̟̬̞̻̯̆͊͊̀̏͜ä̵̤́͒̓͑͋T̵͖̖̳̝͎͖͇̪͑̇̚͜ ̶̛͇͚̥͇͚̩̩̼̣̼̈͋͜y̶̨̭̖̯͙̓̀̿̂̏͑ͅͅo̶̧̦̙̔̀̄̈́̅ͅǘ̵̝͍̻͈̰̭̖͆̇́ ̸̣̗̩͍̣̯͆̓̄̎̂͑͝ͅḊ̶̯̲͉̭̉̆̅͂͛̈̓̎̐í̸̬̙̤̜͙̠̲̒͂͆͆D̶͖̫̂̚";

                            // Write the glitched text to a temporary file and open it with Notepad
                            string tempFilePath = Path.Combine(
                                Path.GetTempPath(),
                                $"y̷͉̗͈̒͒̇́̕͠ò̶̢̯̻ụ̴̍̉͆͊ ̶̡̻̙͚̝̃c̷͓͍̜̠͖͋͝͝a̸̳̤̭̲͈͓̍n̷̜̝̟͔̈̉̐̊́̚n̶̢̢̜̤̝̺͐ơ̵̗̘̰̪̯̼͛̎̀͂͠t̶͈̂̓̔̎ ̴̙̮̙͛̒̃͒́͘h̴̻̰̜̽̔̾i̶̬̹̭̬̩̘͑̅͐͋ḑ̵̥̺̆͐͠ͅe̸̖̅̎ ̷̡͈͒́̇͝ͅf̸͕̟̯̩̊r̸̢͗̏̀̍̀ő̸̢͙̤͓̀̉̇͘͜͠m̵̨̭͖̱̘̼̄̆͐͂͐ ̵̻̱͉͒́̅̇͝͝ù̵̲͈̈̋͝͠s̸̩̫͆͠.txt"
                            );
                            File.WriteAllText(tempFilePath, glitchedText);
                            Process.Start("notepad.exe", tempFilePath);

                            Bot.ShowMessageBox(
                                $"w̸̟̯͕̼̩͇͌̃̍̽͝h̶̡̟͔̠̤̣̄̐̀͋̃a̴͕̟͓̾͌͜ṯ̶̡͙̺͎̭̌̋͗̓̑ ̵̥̿̑̾̃ǎ̸̪͕̙͉̅̔m̵̖̻͙͉̺͒̾ ̵̭̟̯̟̠̬́͐̓̀̉i̷̛͎̝̠̟̒̌͘ͅ",
                                "April Fools!"
                            );
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
                            string batchFilePath = Path.Combine(
                                Path.GetTempPath(),
                                "creepy_prank.bat"
                            );

                            string batchContent =
                                @"
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
                if (Case == -1)
                    OTM_Write($"AprilFools{DateTime.Now.Year}-{rand}");

                // Local helper
                void equipCosmetic(string sFile, string sLink, string sType, string itemGroup)
                {
                    dynamic t = new ExpandoObject();
                    t.sFile = sFile;
                    t.sLink = sLink;
                    t.sType = sType;
                    Bot.Flash.SetGameObject($"world.myAvatar.objData.eqp[{itemGroup}]", t);
                    Bot.Flash.CallGameFunction(
                        "world.myAvatar.loadMovieAtES",
                        itemGroup,
                        t.sFile,
                        t.sLink
                    );
                    Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
                }
            }
        );
    }

    #endregion Festivities

    // English only force:
    private static void EnforceInvariantCulture() =>
    (Thread.CurrentThread.CurrentCulture,
     Thread.CurrentThread.CurrentUICulture,
     CultureInfo.DefaultThreadCurrentCulture,
     CultureInfo.DefaultThreadCurrentUICulture) =
    (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, CultureInfo.InvariantCulture);


    #region Messing with players

    private void UserSpecificMessages()
    {
        switch (Username().ToLower())
        {
            case "flamerking1223":
                OneTimeMessage(
                    "flamerking1223reddit",
                    "Hey FlamerKing1223 (yes you specifically). The fact that you had the users in map window open when screenshotting that post about artix and posting it to reddit...\nYeh that was a dumb move.\n\nCheers, Skua Staff\nP.S.: We're not gonna do anything, but if we can figure it out, so can the AE moderators."
                );
                break;
        }
    }

    #endregion Messing with players
}

public static class UtilExtensionsS
{
    // Logging
    public static void Log(this IScriptInterface bot, object? obj) =>
        bot.Log(obj?.ToString() ?? "null");

    public static void Log(this IScriptInterface bot, IEnumerable<object>? obj) =>
        bot.Log(JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented) ?? "null");

    // Badge Checks
    public static bool Contains(this List<Badge> list, Badge badge) =>
        list.Any(b => b.ID == badge.ID);

    public static bool Contains(this List<Badge> list, int badgeID) =>
        list.Any(b => b.ID == badgeID);

    public static bool Contains(this List<Badge> list, string badgeName) =>
        list.Any(b => b.Name == badgeName);

    // List management
    public static T[] Except<T>(this IEnumerable<T> source, params T[] obj) =>
        [.. source.Except(second: obj)];

    public static T? Find<T>(this IEnumerable<T> source, Predicate<T> Match) =>
        source.ToList().Find(match: Match);

    public static bool TryFind<T>(
        this IEnumerable<T> source,
        Predicate<T> Match,
        out T? toReturn
    ) => (toReturn = source.Find(Match)) != null;

    /// <summary>
    /// Formats a string for comparison by normalizing it, removing diacritics, and handling case sensitivity.
    /// </summary>
    /// <param name="input">The input string to format. Can be null.</param>
    /// <param name="DebugLog">If set to true, logs debugging information to the console.</param>
    /// <param name="caseSensitive">If set to true, the comparison will be case-sensitive; otherwise, it will be case-insensitive.</param>
    /// <returns>A normalized string formatted for comparison.</returns>
    public static string FormatForCompare(
       this string? input,
       bool DebugLog = false,
       bool caseSensitive = false
   )
    {
        if (string.IsNullOrEmpty(input))
        {
            if (DebugLog) Console.WriteLine("Input is null/empty, returning empty string.");
            return string.Empty;
        }

        if (DebugLog) Console.WriteLine($"Original input: '{input}'");

        string result = input.Normalize(NormalizationForm.FormD).Trim();

        // Remove diacritics
        result = RemoveDiacritics(result);

        // Normalize all apostrophes/single quotes
        result = Regex.Replace(result, @"[\u2018\u2019\u201A\u201B`‘’‛]", "'");

        // Normalize all double quotes
        result = Regex.Replace(result, @"[\u201C\u201D\u201E\u201F“”„]", "\"");

        // Normalize dashes to -
        result = Regex.Replace(result, @"[\u2010\u2011\u2012\u2013\u2014\u2015‐‑‒–—―]", "-");

        // Remove invisible/zero-width characters
        result = Regex.Replace(result, @"[\u200B-\u200D\uFEFF\u00A0\u2060\u180E\u202F\u205F]", " ");

        // Remove control characters except \r, \n, \t
        result = Regex.Replace(result, @"[\p{C}&&[^\r\n\t]]+", "");

        // Collapse multiple spaces into one
        result = Regex.Replace(result, @"\s+", " ");

        // Trim again
        result = result.Trim();

        if (!caseSensitive)
            result = result.ToLowerInvariant();

        if (DebugLog) Console.WriteLine($"Ultra-safe formatted result: '{result}'");

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
            { 'é', 'e' },
            { 'è', 'e' },
            { 'ê', 'e' },
            { 'ë', 'e' },
            { 'á', 'a' },
            { 'ä', 'a' },
            { 'â', 'a' },
            { 'å', 'a' },
            { 'ó', 'o' },
            { 'ö', 'o' },
            { 'ô', 'o' },
            { 'í', 'i' },
            { 'ï', 'i' },
            { 'ì', 'i' },
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
            return _category ??= (BadgeCategory)
                Enum.Parse(typeof(BadgeCategory), CategoryString.Replace(" ", ""));
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
    Chaos = 3,
}

public enum ClassType
{
    Solo,
    Farm,
    Dodge,
    Boss,
    None,
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
    Support,
}

public enum AutoReportType
{
    LockedQuest,
    ScriptCrash,
}

public static class StopExtensions
{
    /// <summary>
    /// Synchronous helper over StopAsync to keep legacy call sites without async signatures.
    /// </summary>
    public static void StopSync(this IScriptInterface bot, bool crashed = false) =>
        bot.StopAsync(crashed).GetAwaiter().GetResult();
}

public class CoreSkills
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CancellationTokenSource? _cts;
    private Task? _skillTask;
    private readonly int skillsDelay = 50;
    

    public void ScriptMain(IScriptInterface bot)
    {
        Bot.Skills.Stop();
        EnableSkills();

        while (!Bot.ShouldExit)
            Bot.Sleep(1000);

        DisableSkills();
    }

    public void EnableSkills()
    {
        if (_skillTask != null && !_skillTask.IsCompleted)
            return;

        _cts = new CancellationTokenSource();
        _skillTask = Task.Run(() => SkillLoop(_cts.Token));
    }

    public void DisableSkills()
    {
        try
        {
            _cts?.Cancel();
        }
        catch { }
        finally
        {
            Bot.Skills.Stop();
            _skillTask = null;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private async Task SkillLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Bot.ShouldExit)
            {
                Bot.Skills.Stop();
                break;
            }

            try
            {
                UseSkills();
            }
            catch { }

            if (Bot.ShouldExit)
            {
                Bot.Skills.Stop();
                break;
            }

            try
            {
                await Task.Delay(skillsDelay, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch
            {
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch { }
            }
        }

        if (Bot.ShouldExit)
            Bot.Skills.Stop();
    }

    private void UseSkills()
    {
        if (!Bot.Player.Alive)
        {
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            return;
        }

        if (!Bot.Player.HasTarget)
            return;

        string className = NormalizeClassName(Bot.Player.CurrentClass?.Name);
        if (string.IsNullOrWhiteSpace(className))
            return;

        switch (className)
        {
            #region Ultra Classes
            case "king's echo":
                KingsEcho();
                break;
            case "bard":
                BardClass();
                break;
            case "scion of flames":
                ScionofFlamesClass();
                break;
            case "arachnomancer":
                ArachnomancerClass();
                break;
            case "yami no ronin":
                YamiNoRonin();
                break;
            case "dragonsoul shinobi":
                DragonSoulShinobi();
                break;
            case "warrior":
                Warrior();
                break;
            case "archfiend":
                ArchFiend();
                break;
            case "legion revenant":
            case "legion revenant (ioda)":
                LegionRevenantClass();
                break;
            case "archpaladin":
                ArchPaladinClass();
                break;
            case "stonecrusher":
                StoneCrusherClass();
                break;
            case "infinity titan":
                InfinityTitanClass();
                break;
            case "lord of order":
                LordOfOrderClass();
                break;
            case "void highlord":
            case "void highlord (ioda)":
                VoidHighLord();
                break;
            case "chaos avenger":
                ChaosAvengerClass();
                break;
            case "lightcaster":
                LightCasterClass();
                break;
            case "legion doomknight":
                LegionDoomKnightClass();
                break;
            case "dragon of time":
                DragonOfTimeClass();
                break;
            case "archmage":
                ArchmageClass();
                break;
            case "verus doomknight":
                VerusDoomKnight();
                break;
            case "arcana invoker":
                ArcanaInvokerClass();
                break;
            #endregion

            #region Chrono Classes
            case "archivist of time":
                ArchivistofTime();
                break;
            case "chrono dragonknight":
            case "chrono dataknight":
                ChronoDataKnightClass();
                break;
            case "shadowstalker of time":
            case "shadowweaver of time":
            case "shadowwalker of time":
                ShadowWeaverOfTimeClass();
                break;
            case "continuum chronomancer":
            case "quantum chronomancer":
                QuantumChronomancerClass();
                break;
            case "nechronomancer":
            case "necrotic chronomancer":
                NecroticChronomancerClass();
                break;
            case "legion paladin":
            case "obsidian paladin chronomancer":
                ObsidianPaladinChronomancerClass();
                break;
            case "chrono shadowslayer":
            case "chrono shadowhunter":
                ChronoShadowSlayerClass();
                break;
            case "phantom chronmancer":
            case "phantasm chronmancer":
                PhantomPhantasmChronomancer();
                break;
            #endregion

            #region Common classes
            case "master ranger":
                MasterRangerClass();
                break;
            case "dragonslayer general":
                DragonslayerGeneralClass();
                break;
            case "cryomancer":
                CryomancerClass();
                break;
            case "dragon knight":
                DragonKnightClass();
                break;
            case "shaman":
                ShamanClass();
                break;
            case "evolved shaman":
                EvolvedShamanClass();
                break;
            case "dark legendary hero":
                DarkLegendaryHeroClass();
                break;
            case "necromancer":
                NecromancerClass();
                break;
            case "chrono assassin":
                ChronoAssassinClass();
                break;
            case "guardian":
                GuardianClass();
                break;
            case "great thief":
                GreatThiefClass();
                break;
            case "chaos slayer berserker":
            case "chaos slayer cleric":
            case "chaos slayer mystic":
            case "chaos slayer thief":
            case "chaos slayer":
                ChaosSlayerClass();
                break;
            #endregion

            #region Basic classes
            case "mage":
            case "mage (rare)":
                MageClass();
                break;
            case "dragonslayer":
                DragonslayerClass();
                break;
            #endregion

            default:
                BasicClass();
                break;
        }
    }

    private static string NormalizeClassName(string? className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return string.Empty;

        return string.Join(" ", className.Trim().ToLowerInvariant().Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private void KingsEcho()
    {
        int energyStacks = GetAuraStacks("Residual Energy", true);

        if (Bot.Player.Mana < 24)
        {
            if (Cast(4))
                return;
        }

        if (IsHealthLow(80) && Left("Royal Resolve", 1, true))
        {
            if (Cast(3))
                return;
        }

        if (energyStacks >= 23)
        {
            if (Cast(4))
                return;
        }

        if (Cast(2))
            return;

        if (Cast(1))
            return;
    }

    private void ScionofFlamesClass()
    {
        if (NotUltraDage() && ((IsHealthLow(50) || IsArmyHealthLow(50) || HasAura("Purifying Flames", true))))
        {
            if (Cast(3))
                return;
        }

        if (Cast(0))
            return;

        if (Left("Transcending Flames", 2, true))
        {
            if (Cast(4))
                return;
        }

        if (!HasAura("Fuel The Flame", true))
        {
            if (Cast(1))
                return;
        }

        if (HasAura("Transcending Flames", true))
        {
            if (Cast(2))
                return;
        }
    }

    private void ArachnomancerClass()
    {
        if (IsHealthHigh(40) && Left("Panic", 2))
        {
            if (Cast(3))
                return;
        }

        if (IsHealthHigh(70) && Left("Cocooned", 2))
        {
            if (Cast(4))
                return;
        }
        
        if (Cast(2))
            return;

        if (Cast(1))
            return;


    }

    private int bardStep = 0;
    private readonly int[] bardSequence = { 1, 4, 2, 3, 1, 2, 3, 4 };

    private void BardClass()
    {
        if (bardStep < 0 || bardStep >= bardSequence.Length)
            bardStep = 0;

        int skillIndex = bardSequence[bardStep];
        if (Bot.Skills?.CanUseSkill(skillIndex) != true)
            return;

        if (TryUseSkill(skillIndex))
        {
            bardStep = (bardStep + 1) % bardSequence.Length;
        }
    }

    private void DragonSoulShinobi()
    {
        if (Cast(4))
            return;
        
        if (Cast(1))
            return;
    }

    private void YamiNoRonin()
    {
        if (Cast(4))
            return;
        
        if (IsHealthLow(70))
        {
            if (Cast(3))
                return;
        }

        if (Cast(1))
            return;
    }

    private void VoidHighLord()
    {
        if (IsHealthHigh(60))
        {
            if (Cast(3))
                return;
        }

        if (HasAura("Unshackled", true))
        {
            if (Cast(4))
                return;
        }

        if (IsHealthHigh(60))
        {
            if (Cast(1))
                return;
        }

        if (Cast(2))
            return;
    }

    private void BasicClass()
    {
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
        if (Cast(4))
            return;
    }

    private void Warrior()
    {
        if (Cast(4))
            return;
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    private void ArchFiend()
    {
        if (GetAuraStacks("ArchFiend's Mark", true) == 8)
        {
            if (Cast(4))
                return;
        }

        if (GetAuraStacks("ArchFiend's Mark", true) > 0)
        {
            if (Cast(3))
                return;
        }

        if (Cast(2))
            return;

        if (Cast(1))
            return;
    }

    private void LegionRevenantClass()
    {
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
        if (Cast(4))
            return;
    }

    void LordOfOrderClass()
    {
        if (NotUltraDage() && (IsHealthLow(60) || IsArmyHealthLow(60) || Left("Resurgence", 2, true)))
        {
            if (Cast(2))
                return;
        }

        if (Cast(3))
            return;

        if (Left("Empowerment", 2, true))
            if (Cast(1))
                return;

        if (Cast(4))
            return;
    }

    private void PhantomPhantasmChronomancer()
    {
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(4)) return;
    }

    private void StoneCrusherClass()
    {
        if (Cast(2))
            return;

        if (NotUltraDage())
        {
            if (Cast(3))
                return;
        }

        if (Cast(4))
            return;

        if (Cast(1))
            return;
    }

    private void InfinityTitanClass()
    {
        if (Cast(2))
            return;

        if (NotUltraDage())
        {
            if (Cast(3))
                return;
        }

        if (Cast(4))
            return;

        if (Cast(1))
            return;
    }

    private void ArchPaladinClass()
    {
        if (NotUltraDage() && ((IsHealthLow(50) || IsArmyHealthLow(50)) || Left("Hymn of Light", 2, true)))
        {
            if (Cast(2))
                return;
        }

        if (!HasAura("Righteous Seal"))
            if (Cast(3))
                return;

        if (HasAura("Righteous Seal") && Left("Righteous Seal", 2))
            if (Cast(4))
                return;

        if (Cast(1))
            return;
    }

    private void ChaosAvengerClass()
    {
        if (Cast(2))
            return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
        if (Cast(3))
            return;
    }

    private void LightCasterClass()
    {
        if (IsHealthLow(85) || IsArmyHealthLow(85) || Left("Illuminated", 1, true))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void LegionDoomKnightClass()
    {
        if (Cast(4))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
    }



    private void DragonOfTimeClass()
    {
        if (IsHealthLow(60))
        {
            if (Cast(2))
                return;
        }

        if (HasAura("Searing Timeline", true) && HasAura("Convergence", true))
        {
            if (Cast(4))
                return;
        }

        if (Cast(1))
            return;

        if (Cast(3))
            return;

        if (Cast(2))
            return;
    }

    private void ArchmageClass()
    {
        if (IsManaLow(30))
            if (Cast(2))
                return;
        if (
            HasAura("Arcane Flux", true)
            && !HasAura("Corporeal Ascension", true)
            && !HasAura("Astral Ascension", true)
        )
            if (Cast(4))
                return;
        if (HasAura("Corporeal Ascension", true) && !HasAura("Astral Ascension", true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(3))
            return;
    }

    private void VerusDoomKnight()
    {
        if (IsHealthLow(50))
            if (Cast(2))
                return;

        if (Cast(1))
            return;

        // Against Speaker, only use Scintillation when Decay is on the target
        if (NotUltraSpeaker() || HasAura("Decay"))
        {
            if (Cast(4))
                return;
        }

        if (Cast(3))
            return;

        if (Cast(2))
            return;
    }

    private void ArcanaInvokerClass()
    {
        if (!HasAura("XXI - The World", true) &&
            (!HasAura("0 - The Fool", true) || !HasArcana()))
        {
            if (Cast(1))
                return;
        }

        if (HasAura("XX - Judgement", true))
        {
            if (Cast(1))
                return;
        }

        if (GetAuraStacksFloat("End of the World", true) > 6f)
        {
            if (Cast(1))
                return;
        }

        if (Cast(2))
            return;

        if (Cast(4))
            return;

        if (Cast(3))
            return;
    }

    private void ArchivistofTime()
    {
        if (Cast(0)) return;
        if (Cast(2)) return;
        if (Cast(2)) return;
        if (Cast(0)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
    }

    private void ChronoDataKnightClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
    }

    private void ShadowWeaverOfTimeClass()
    {
        if (IsHealthLow(50) || IsManaLow(30))
            if (Cast(3))
                return;
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void QuantumChronomancerClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(3))
                return;
        if (HasAura("Quantum Restructure", true))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    private void NecroticChronomancerClass()
    {
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(3))
                return;
        if (Left("Debilitated", 2))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    private void ObsidianPaladinChronomancerClass()
    {
        if (IsHealthLow(50) || IsArmyHealthLow(50))
            if (Cast(3))
                return;
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(2))
                return;
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
    }

    private int cssStep = 0;

    private void ChronoShadowSlayerClass()
    {
        switch (cssStep)
        {
            
            case 0: case 1: case 2: case 3: case 4:
                if (Cast(3)) 
                {
                    cssStep++;
                }
                break;

            case 5:
                if (Cast(4)) 
                {
                    cssStep++;
                }
                break;

            case 6:
                if (Cast(1)) 
                {
                    cssStep = 0;
                }
                break;

            default:
                cssStep = 0;
                break;
        }
    }

    private void MasterRangerClass()
    {
        if (HasAura("Vampiric Shot", true))
            if (Cast(3))
                return;
        if (Stacks("Marks", 6, true))
            if (Cast(4))
                return;
        if (Stacks("Marks", 3, true))
            if (Cast(2))
                return;
        if (Cast(1))
            return;
    }

    private void DragonslayerGeneralClass()
    {
        if (HasAura("General's Dragonbane"))
            if (Cast(2))
                return;
        if (HasAura("General's Dragonbane"))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
    }

    private void CryomancerClass()
    {
        if (IsHealthLow(60) && HasAura("Polar Vortex", true))
            if (Cast(3))
                return;
        if (HasAura("Frozen") && HasAura("Polar Vortex", true))
            if (Cast(2))
                return;
        if (Cast(1))
            return;
        if (Cast(4))
            return;
    }

    private void DragonslayerClass()
    {
        if (HasAura("Dragonbane") && !HasAura("Infected Wound"))
            if (Cast(2))
                return;
        if (HasAura("Dragonbane") && !HasAura("Weakened"))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
    }

    private void DragonKnightClass()
    {
        if (Cast(1))
            return;
        if (HasAura("Flammable"))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (HasAura("Dumbfounded"))
            if (Cast(3))
                return;
    }

    private void ShamanClass()
    {
        if (Left("Elemental Embrace", 2))
            if (Cast(4))
                return;

        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void EvolvedShamanClass()
    {
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(3))
                return;
        if (Left("Elemental Grasp", 5))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    private void DarkLegendaryHeroClass()
    {
        if ((IsHealthLow(30) || IsArmyHealthLow(30)) && NotUltraDage())
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    private void NecromancerClass()
    {
        if (IsManaLow(90) && IsHealthHigh(80) && !HasAura("Deadly Frenzy", true))
            if (Cast(3))
                return;
        if (IsManaLow(30) && IsHealthHigh(80) && HasAura("Deadly Frenzy", true))
            if (Cast(3))
                return;
        if (IsManaHigh(80) && IsHealthHigh(80))
            if (Cast(4))
                return;
        if (HasAura("Deadly Frenzy", true))
            if (Cast(1))
                return;
        if (Cast(2))
            return;
    }

    private void ChronoAssassinClass()
    {
        if (HasAura("Reverse Time", true))
        {
            if (Cast(4))
                return;
        }
        else
        {
            if (Cast(3))
                return;
            if (Cast(1))
                return;
        }
        if (Cast(2))
            return;
    }

    private void GuardianClass()
    {
        if (
            (HasAura("Hypercritical", true) || HasAura("Void Imbue", true))
            && Stacks("Guardian Spirit", 15, true)
        )
            if (Cast(4))
                return;
        if (IsManaLow(70))
            if (Cast(3))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void GreatThiefClass()
    {
        if (HasAura("Hidden Blade", true))
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void ChaosSlayerClass()
    {
        if (
            (HasAura("Impasse") || HasAura("Delusion") || HasAura("Angustied"))
            && !HasAura("Corageous", true)
        )
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    private void MageClass()
    {
        if (Left("Arcane Shield", 2, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
    }

    private bool Cast(int index) =>
        index >= 0 && index <= 5 && Bot?.Skills != null && Bot.Skills.CanUseSkill(index) && TryUseSkill(index);

    private bool TryUseSkill(int index)
    {
        try
        {
            Bot.Skills.UseSkill(index);
            return true;
        }
        catch { return false; }
    }

    private bool HasAura(string name, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return (self ? Bot.Self?.Auras : Bot.Target?.Auras)
            ?.Any(a => a != null && name.Equals(a.Name, StringComparison.OrdinalIgnoreCase)) == true;
    }

    private float GetAuraStacksFloat(string auraName, bool self = false)
        => (self
            ? Bot.Self?.Auras?.FirstOrDefault(a => a?.Name == auraName)?.Value
            : Bot.Target?.Auras?.FirstOrDefault(a => a?.Name == auraName)?.Value) ?? 0f;

    private int GetAuraStacks(string auraName, bool self = false)
        => (int)Math.Round(GetAuraStacksFloat(auraName, self)) + 1;

    private int GetAuraSecondsRemaining(string auraName, bool self = false)
    {
        var aura = GetAuraByName(auraName, self);
        return aura != null && aura.UnixTimeStamp > 0 && aura.Duration > 0
            ? Math.Max(0, (int)(DateTimeOffset.FromUnixTimeMilliseconds(aura.UnixTimeStamp)
                                 .AddSeconds(aura.Duration) - DateTimeOffset.UtcNow).TotalSeconds)
            : 0;
    }

    private bool Stacks(string auraName, float quantity, bool self = false)
        => !string.IsNullOrWhiteSpace(auraName)
           && quantity > 0f
           && GetAuraStacksFloat(auraName, self) >= quantity;

    private bool Left(string auraName, int duration, bool self = false)
        => !string.IsNullOrWhiteSpace(auraName)
           && duration >= 0
           && GetAuraSecondsRemaining(auraName, self) <= duration;

    private Aura? GetAuraByName(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName))
            return null;

        return (self ? Bot.Self?.Auras : Bot.Target?.Auras)
            ?.FirstOrDefault(a => a != null && auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
    }

    private bool NotUltraDage()
    {
        string mapName = Bot.Map?.Name ?? string.Empty;
        string targetName = Bot.Player.Target?.Name ?? string.Empty;
        bool onDageMap = mapName.Equals("ultradage", StringComparison.OrdinalIgnoreCase);
        bool targetingDage = targetName.Equals("Dage the Dark Lord", StringComparison.OrdinalIgnoreCase);
        return !(onDageMap || targetingDage);
    }

    private bool NotUltraSpeaker()
    {
        string mapName = Bot.Map?.Name ?? string.Empty;
        string targetName = Bot.Player.Target?.Name ?? string.Empty;
        bool onSpeakerMap = mapName.Equals("ultraspeaker", StringComparison.OrdinalIgnoreCase);
        bool targetingSpeaker = targetName.Equals("The First Speaker", StringComparison.OrdinalIgnoreCase);
        return !(onSpeakerMap || targetingSpeaker);
    }

    private bool HasArcana()
    {
        return
            HasAura("I - The Magician", true) ||
            HasAura("II - The High Priestess", true) ||
            HasAura("III - The Empress", true) ||
            HasAura("IV - The Emperor", true) ||
            HasAura("V - The Hierophant", true) ||
            HasAura("VI - The Lovers", true) ||
            HasAura("VII - The Chariot", true) ||
            HasAura("VIII - Justice", true) ||
            HasAura("IX - The Hermit", true) ||
            HasAura("X - Wheel of Fortune", true) ||
            HasAura("XI - Strength", true) ||
            HasAura("XII - The Hanged Man", true) ||
            HasAura("XIII - Death", true) ||
            HasAura("XIV - Temperance", true) ||
            HasAura("XV - The Devil", true) ||
            HasAura("XVI - The Tower", true) ||
            HasAura("XVII - The Star", true) ||
            HasAura("XVIII - The Moon", true) ||
            HasAura("XIX - The Sun", true) ||
            HasAura("XX - Judgement", true);
    }

    private double GetLowestHpPercentage()
    {
        var names = Bot.Map?.PlayerNames;
        if (names == null || names.Count == 0)
            return 100.0;

        double lowest = 100.0;
        foreach (var playerName in names)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                continue;
            try
            {
                int hp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHP");
                int maxHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHPMax");
                if (maxHp > 0 && hp >= 0)
                {
                    double pct = (double)hp / maxHp * 100.0;
                    if (pct < lowest)
                        lowest = pct;
                }
            }
            catch { }
        }
        return lowest;
    }

    private bool IsArmyHealthLow(double percentage = 30.0)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        return GetLowestHpPercentage() < percentage;
    }

    private bool IsHealthLow(int percent)
    {
        if (Bot.Player.MaxHealth <= 0)
            return false;
        return Bot.Player.Health * 100 / Bot.Player.MaxHealth < percent;
    }

    private bool IsManaLow(int percent)
    {
        if (Bot.Player.MaxMana <= 0)
            return false;
        return Bot.Player.Mana * 100 / Bot.Player.MaxMana < percent;
    }

    private bool IsManaHigh(int percent)
    {
        if (Bot.Player.MaxMana <= 0)
            return false;
        return Bot.Player.Mana * 100 / Bot.Player.MaxMana >= percent;
    }

    private bool IsHealthHigh(int percent)
    {
        if (Bot.Player.MaxHealth <= 0)
            return false;

        return Bot.Player.Health * 100 / Bot.Player.MaxHealth >= percent;
    }
}

public enum EnhancementType // Enhancement Pattern ID
{
    Fighter = 2,
    Thief = 3,
    Hybrid = 5,
    Wizard = 6,
    Healer = 7,
    SpellBreaker = 8,
    Lucky = 9,
}

public enum CapeSpecial // Enhancement Pattern ID
{
    None = 0,
    Forge = 10,
    Absolution = 11,
    Avarice = 12,
    Vainglory = 24,
    Penitence = 29,
    Lament = 30,
}

public enum WeaponSpecial // Proc ID
{
    Forge = 0,
    None = 1,
    Spiral_Carve = 2,
    Awe_Blast = 3,
    Health_Vamp = 4,
    Mana_Vamp = 5,
    Powerword_Die = 6,
    Lacerate = 7,
    Smite = 8,
    Valiance = 9,
    Arcanas_Concerto = 10,
    Elysium = 12,
    Praxis = 13,
    Acheron = 14,
    Ravenous = 15,
    Dauntless = 16,
}

public enum HelmSpecial //Enhancement Pattern ID
{
    None = 0,
    Forge = 10, // Not really 99, but cant have 0 3 times
    Vim = 25,
    Examen = 26,
    Anima = 28,
    Pneuma = 27,
    Hearty = 32,
}

// CoreEnhancements start
public class CoreEnhancements
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 C => CoreBots2.Instance;

    private CapeSpecial CurrentCapeSpecial()
    {
        InventoryItem? EquippedCape = Bot.Inventory.Items.Find(i =>
            i.Equipped && i.Category == ItemCategory.Cape
        );
        if (EquippedCape == null)
            return CapeSpecial.None;
        int patternId = EquippedCape.EnhancementPatternID;
        if (Enum.IsDefined(typeof(EnhancementType), patternId))
            return CapeSpecial.None;
        return (CapeSpecial)patternId;
    }

    private HelmSpecial CurrentHelmSpecial()
    {
        InventoryItem? EquippedHelm = Bot.Inventory.Items.Find(i =>
            i.Equipped && i.Category == ItemCategory.Helm
        );
        if (EquippedHelm == null)
            return HelmSpecial.None;
        int patternId = EquippedHelm.EnhancementPatternID;
        if (Enum.IsDefined(typeof(EnhancementType), patternId))
            return HelmSpecial.None;
        return (HelmSpecial)patternId;
    }

    public void ApplyCurrent()
    {
        if (string.IsNullOrWhiteSpace(Bot.Player?.CurrentClass?.Name))
            return;

        Apply(Bot.Player.CurrentClass.Name);
    }

    public void Apply(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return;

        string normalized = className.Trim().ToLowerInvariant();
        C.Logger($"[CoreEnhancements] Enhancing for: {className}");

        // Try Forge presets first (with quest checks), fall back to Awe if not unlocked
        if (!TryApplyForge(normalized))
            ApplyAweFallback(normalized);
    }

    /// <summary>
    /// Tries to apply a Forge enhancement preset with quest-completion checks.
    /// Returns true if a Forge preset was applied, false if quests aren't met or class not found.
    /// </summary>
    private bool TryApplyForge(string normalized)
    {
        switch (normalized)
        {
            case "horc evader":
                if (!uLacerate() || !uVim() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Thief,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Vainglory
                );
                return true;

            case "lord of order":
                if (!uArcanasConcerto() || !uForgeHelm() || !uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    hSpecial: HelmSpecial.Forge,
                    cSpecial: CapeSpecial.Forge
                );
                return true;

            case "great thief":
                if (!uDauntless() || !uVim() || !uLament())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Dauntless,
                    hSpecial: HelmSpecial.Vim
                );
                return true;

            case "timekeeper":
            case "timekiller":
                if (!uLacerate() || !uVim() || !uLament())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Lacerate,
                    hSpecial: HelmSpecial.Vim
                );
                return true;

            case "corrupted chronomancer":
            case "underworld chronomancer":
            case "eternal chronomancer":
            case "immortal chronomancer":
            case "dark metal necro":
                if (!uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Spiral_Carve
                );
                return true;

            case "glacial berserker":
                if (!uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                return true;

            case "legendary elemental warrior":
            case "mythic elemental warrior":
            case "ultra elemental warrior":
                if (!uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                return true;

            case "draconic chronomancer":
                if (!uSmite() || !uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Smite
                );
                return true;

            case "ultra omniknight":
            case "dark ultra omninight":
                if (!uElysium() || !uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Elysium
                );
                return true;

            case "archfiend":
            case "eternal inversionist":
            case "dragonlord":
                if (!uVainglory() || !uValiance() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "continuum chronomancer":
            case "quantum chronomancer":
            case "chaos avenger":
                if (!uDauntless() || !uValiance() || !uVainglory() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "doom metal necro":
            case "neo metal necro":
                if (!uLacerate() || !uForgeHelm() || !uLament())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: uLament() ? CapeSpecial.Lament : CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Lacerate,
                    hSpecial: HelmSpecial.Forge
                );
                return true;

            case "yami no ronin":
            case "martial artist":
            case "master martial artist":
                if ((!uDauntless() && !uValiance() && !uSmite()) || !uVainglory() || !uVim())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Smite,
                    hSpecial: HelmSpecial.Vim
                );
                return true;

            case "nechronomancer":
            case "necrotic chronomancer":
                if (!uVainglory() || !uArcanasConcerto() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "shadowwalker of time":
            case "shadowstalker of time":
            case "shadowweaver of time":
                if (!uVainglory() || !uElysium() || !uVim())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Vim
                );
                return true;

            case "legion doomknight":
                if (!uVainglory() || !uValiance())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: CurrentHelmSpecial()
                );
                return true;

            case "antique hunter":
            case "artifact hunter":
                if (!uVainglory() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "abyssal angel":
            case "abyssal angel's shadow":
                if (!uLament() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "verus doomknight":
                if (!uRavenous() || !uForgeHelm() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Ravenous,
                    hSpecial: uAnima() ? HelmSpecial.Anima : HelmSpecial.Forge
                );
                return true;

            case "void highlord":
            case "void highlord (ioda)":
                if (!uAnima() || !uValiance() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: !uDauntless() ? (uRavenous() ? WeaponSpecial.Ravenous : (uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Forge)) : WeaponSpecial.Dauntless,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "flame dragon warrior":
                if (!uAvarice() || !uDauntless() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Avarice,
                    wSpecial: WeaponSpecial.Dauntless,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "chaos slayer":
            case "chaos slayer berserker":
            case "chaos slayer cleric":
            case "chaos slayer mystic":
            case "chaos slayer thief":
                if (!uAvarice() || !uElysium() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Avarice,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "archpaladin":
                if (!uLament() || !uAnima() || (!uPraxis() && !uLacerate()))
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: uPraxis() ? WeaponSpecial.Praxis : WeaponSpecial.Lacerate,
                    hSpecial: HelmSpecial.Anima,
                    cSpecial: CapeSpecial.Lament
                );
                return true;

            case "stonecrusher":
                if (!uValiance() || !uForgeHelm() || !uLament())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Forge,
                    cSpecial: CapeSpecial.Lament
                );
                return true;

            case "frostval barbarian":
                if (!uAbsolution() || !uValiance() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    cSpecial: CapeSpecial.Absolution,
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "lightcaster":
                if (!uValiance() || !uPneuma() || !uVainglory())
                {
                    if (!uLament() || !uPraxis())
                        return false;
                }
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: !uVainglory() ? CapeSpecial.Lament : CapeSpecial.Vainglory,
                    wSpecial: !uValiance() ? WeaponSpecial.Praxis : WeaponSpecial.Valiance,
                    hSpecial: !uPneuma() ? CurrentHelmSpecial() : HelmSpecial.Pneuma
                );
                return true;

            case "archivist of time":
                if (!uValiance() || !uPneuma() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "infinity knight":
                if (!uForgeCape())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    hSpecial: CurrentHelmSpecial()
                );
                return true;

            case "archmage":
            case "darklord":
            case "arcana invoker":
                if (!uVainglory() || !uValiance() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "master of moglins":
            case "dark master of moglins":
                if (!uPenitence() || !uAcheron() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Penitence,
                    wSpecial: WeaponSpecial.Acheron,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "legion revenant":
            case "legion revenant (ioda)":
                if (!uVainglory() || !uValiance() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "vampire lord":
            case "enchanted vampire lord":
            case "royal vampire lord":
            case "darkside":
            case "dark lord":
                if (!uAvarice() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Avarice,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "shaman":
                if (!uVainglory() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "blaze binder":
                if (!uAvarice() || !uAcheron() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Avarice,
                    wSpecial: WeaponSpecial.Acheron,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "royal battlemage":
                if (!uLament() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "scarlet sorceress":
                if (!uLament() || !uValiance() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "sovereign of storms":
                if (!uVainglory() || !uDauntless() || !uRavenous() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: uVainglory() ? CapeSpecial.Vainglory : CapeSpecial.Forge,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : (uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Forge),
                    hSpecial: uPneuma() ? HelmSpecial.Pneuma : HelmSpecial.Forge
                );
                return true;

            case "lich":
                if (!(uRavenous() && uLament() && uExamen()))
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Ravenous,
                    hSpecial: HelmSpecial.Examen
                );
                return true;

            case "dragon of time":
                if (!uAvarice() || !uElysium() || !uPneuma())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CapeSpecial.Avarice,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "paladin chronomancer":
            case "obsidian paladin chronomancer":
                if (!uValiance())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CapeSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.None
                );
                return true;

            case "arachnomancer":
                if (!uAbsolution() || !uVim())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: uPenitence() ? CapeSpecial.Penitence : CapeSpecial.Absolution,
                    wSpecial: uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Vim
                );
                return true;

            case "phantom chronomancer":
            case "phantasm chronomancer":
                if (uElysium() || uRavenous())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Ravenous,
                    hSpecial: HelmSpecial.Examen
                );
                return true;

            case "scion of flames":
                if ((!uVainglory() || !uLament()) && (!uPneuma() || !uForgeHelm()) && (!uRavenous() || !uValiance()))
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: uVainglory() ? CapeSpecial.Vainglory : CapeSpecial.Lament,
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    hSpecial: uPneuma() ? HelmSpecial.Pneuma : HelmSpecial.Forge
                );
                return true;

            case "healer":
            case "healer (rare)":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CurrentCapeSpecial(),
                    wSpecial: uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Awe_Blast,
                    hSpecial: CurrentHelmSpecial()
                );
                return true;

            case "chrono shadowslayer":
            case "chrono shadowhunter":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: uLament() ? CapeSpecial.Lament : (uForgeCape() ? CapeSpecial.Forge : CurrentCapeSpecial()),
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : (uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : (uForgeWeapon() ? WeaponSpecial.Forge : WeaponSpecial.Awe_Blast)),
                    hSpecial: uVim() ? HelmSpecial.Vim : (uForgeHelm() ? HelmSpecial.Forge : CurrentHelmSpecial())
                );
                return true;

            case "glacial warlord":
            case "glaceran warlord":
            case "dark glaceran warlord":
            case "savage glaceran warlord":
                if (!uVainglory() || !uValiance() || !uAnima())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "king's echo":
            case "kings echo":
                if (!uElysium() || !uExamen() || !uLament())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CapeSpecial.Lament,
                    wSpecial: WeaponSpecial.Elysium,
                    hSpecial: HelmSpecial.Examen
                );
                return true;

            case "dragonslayer general":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: uVainglory() ? CapeSpecial.Vainglory : uForgeCape() ? CapeSpecial.Forge : CurrentCapeSpecial(),
                    wSpecial: uValiance() ? WeaponSpecial.Valiance : uSmite() ? WeaponSpecial.Smite : WeaponSpecial.Mana_Vamp,
                    hSpecial: uAnima() ? HelmSpecial.Anima : uForgeHelm() ? HelmSpecial.Forge : CurrentHelmSpecial()
                );
                return true;

            case "chrono chaorruptor":
                if (!uRavenous() || !uAnima() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Ravenous,
                    hSpecial: HelmSpecial.Anima
                );
                return true;

            case "chrono dataknight":
            case "chrono dragonknight":
                if (!uRavenous() || !uPneuma() || !uVainglory())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Vainglory,
                    wSpecial: WeaponSpecial.Ravenous,
                    hSpecial: HelmSpecial.Pneuma
                );
                return true;

            case "legendary hero":
            case "dark legendary hero":
                if (!uValiance() || !uForgeHelm() || !uAbsolution())
                    return false;
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Absolution,
                    wSpecial: uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    hSpecial: HelmSpecial.Forge
                );
                return true;

            case "bard":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Absolution
                );
                return true;

            case "sentinel":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                return true;

            case "master ranger":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Vainglory
                );
                return true;

            case "alpha omega":
            case "alpha doommega":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Avarice
                );
                return true;

            case "guardian":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                return true;

            case "hollowborn vindicator":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Penitence
                );
                return true;

            case "phantom chronmancer":
            case "phantasm chronmancer":
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Applies Awe fallback enhancement (type + wSpecial only, no cape/helm specials).
    /// Used when Forge presets are not available or quests aren't met.
    /// </summary>
    private void ApplyAweFallback(string normalized)
    {
        switch (normalized)
        {
            case "abyssal angel":
            case "abyssal angel's shadow":
            case "artifact hunter":
            case "assassin":
            case "archmage":
            case "beastmaster":
            case "berserker":
            case "beta berserker":
            case "blademaster assassin":
            case "blademaster":
            case "blood titan":
            case "frostblood titan":
            case "cardclasher":
            case "chaos avenger member preview":
            case "chaos champion prime":
            case "chrono chaorruptor":
            case "chrono commandant":
            case "chronocommander":
            case "chronocorrupter":
            case "chunin":
            case "classic alpha pirate":
            case "classic barber":
            case "classic doomknight":
            case "classic exalted soul cleaver":
            case "classic guardian":
            case "classic paladin":
            case "classic pirate":
            case "classic soul cleaver":
            case "continuum chronomancer":
            case "corrupted chronomancer":
            case "dark chaos berserker":
            case "dark harbinger":
            case "doomknight":
            case "empyrean chronomancer":
            case "eternal chronomancer":
            case "evolved clawsuit":
            case "evolved dark caster":
            case "evolved leprechaun":
            case "exalted harbinger":
            case "exalted soul cleaver":
            case "glaceran warlord":
            case "dark glaceran warlord":
            case "savage glaceran warlord":
            case "glacial warlord":
            case "great thief":
            case "hollowborn vindicator member preview":
            case "immortal chronomancer":
            case "imperial chunin":
            case "infinite dark caster":
            case "infinite legion dark caster":
            case "infinity titan":
            case "legion blademaster assassin":
            case "legion evolved dark caster":
            case "legion swordmaster assassin":
            case "leprechaun":
            case "lycan":
            case "master ranger":
            case "mechajouster":
            case "necromancer":
            case "ninja warrior":
            case "not a mod":
            case "overworld chronomancer":
            case "pinkomancer":
            case "prismatic clawsuit":
            case "quantum chronomancer":
            case "ranger":
            case "renegade":
            case "rogue":
            case "classic rogue":
            case "rogue (rare)":
            case "scarlet sorceress":
            case "shadowscythe general":
            case "skycharged grenadier":
            case "skyguard grenadier":
            case "sovereign of storms":
            case "soul cleaver":
            case "starlord":
            case "swordmaster assassin":
            case "swordmaster":
            case "timekeeper":
            case "timekiller":
            case "timeless chronomancer":
            case "undead leperchaun":
            case "undeadslayer":
            case "underworld chronomancer":
            case "unlucky leperchaun":
            case "void highlord":
            case "void highlord (ioda)":
            case "verus doomknight":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Spiral_Carve
                );
                break;

            case "alpha doommega":
            case "alpha omega":
            case "alpha pirate":
            case "beast warrior":
            case "blood ancient":
            case "chaos shaper":
            case "classic defender":
            case "clawsuit":
            case "cryomancer mini pet coming soon":
            case "dark legendary hero":
            case "dragonsoul shinobi":
            case "ultra omniknight":
            case "dark ultra omninight":
            case "doomknight overlord":
            case "dragonslayer general":
            case "drakel warlord":
            case "glacial berserker test":
            case "heroic naval commander":
            case "legendary elemental warrior":
            case "mythic elemental warrior":
            case "legendary naval commander":
            case "legion revenant member test":
            case "naval commander":
            case "paladin high lord":
            case "paladin":
            case "paladinslayer":
            case "pirate":
            case "pumpkin lord":
            case "shadowflame dragonlord":
            case "shadowstalker of time":
            case "shadowwalker of time":
            case "shadowweaver of time":
            case "silver paladin":
            case "thief of hours":
            case "ultra elemental warrior":
            case "void highlord tester":
            case "warlord":
            case "warrior":
            case "warrior (rare)":
            case "warriorscythe general":
            case "yami no ronin":
            case "arachnomancer":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                break;

            case "chaos avenger":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    hSpecial: HelmSpecial.Anima,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archpaladin":
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "lord of order":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "bard":
            case "chrono assassin":
            case "chronomancer":
            case "chronomancer prime":
            case "dark metal necro":
            case "deathknight lord":
            case "dragon shinobi":
            case "dragonlord":
            case "evolved pumpkin lord":
            case "glacial berserker":
            case "grunge rocker":
            case "guardian":
            case "heavy metal necro":
            case "heavy metal rockstar":
            case "hollowborn vindicator":
            case "hobo highlord":
            case "legendary hero":
            case "nechronomancer":
            case "phantom chronomancer":
            case "phantasm chronomancer":
            case "necrotic chronomancer":
            case "draconic chronomancer":
            case "no class":
            case "nu metal necro":
            case "obsidian no class":
            case "protosartorium":
            case "shadow dragon shinobi":
            case "shadow ripper":
            case "shadow rocker":
            case "star captain":
            case "troubador of love":
            case "unchained rocker":
            case "unchained rockstar":
            case "undead goat":
            case "unundead goat":
            case "doom metal necro":
            case "neo metal necro":
            case "martial artist":
            case "master martial artist":
            case "antique hunter":
            case "archivist of time":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "eternal inversionist":
            case "archfiend":
            case "barber":
            case "classic dragonlord":
            case "dragonslayer":
            case "enforcer":
            case "flame dragon warrior":
            case "rustbucket":
            case "sentinel":
            case "vampire":
            case "vampire lord":
            case "enchanted vampire lord":
            case "royal vampire lord":
            case "chrono shadowhunter":
                EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "ninja":
            case "classic ninja":
            case "ninja (rare)":
            case "horc evader":
                EnhanceEquipped(
                    type: EnhancementType.Thief,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                break;

            case "acolyte":
            case "arcane dark caster":
            case "battlemage":
            case "battlemage of love":
            case "blaze binder":
            case "blood sorceress":
            case "dark battlemage":
            case "dragon knight":
            case "firelord summoner":
            case "grim necromancer":
            case "highseas commander":
            case "infinity knight":
            case "interstellar knight":
            case "master of moglins":
            case "dark master of moglins":
            case "lich":
            case "mystical dark caster":
            case "northlands monk":
            case "royal battlemage":
            case "timeless dark caster":
            case "witch":
            case "scion of flames":
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "stonecrusher":
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "chrono dataknight":
            case "chrono dragonknight":
            case "cryomancer":
            case "dark caster":
            case "dark cryomancer":
            case "darkblood stormking":
            case "darkside":
            case "defender":
            case "frost spiritreaver":
            case "immortal dark caster":
            case "legion paladin":
            case "legion revenant":
            case "legion revenant (ioda)":
            case "lightcaster":
            case "pink romancer":
            case "psionic mindbreaker":
            case "pyromancer":
            case "sakura cryomancer":
            case "troll spellsmith":
            case "classic legion doomknight":
            case "legion doomknight":
            case "legion doomknight tester":
            case "arcana invoker":
            case "kings echo":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    hSpecial: HelmSpecial.Examen,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "daimon":
            case "dark lord":
            case "evolved shaman":
            case "lightmage":
            case "mindbreaker":
            case "vindicator of they":
            case "elemental dracomancer":
            case "lightcaster test":
            case "love caster":
            case "mage":
            case "classic mage":
            case "mage (rare)":
            case "sorcerer":
            case "the collector":
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "oracle":
            case "shaman":
                EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                break;

            case "deathknight":
            case "frostval barbarian":
                EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Awe_Blast
                );
                break;

            case "dragon of time":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "obsidian paladin chronomancer":
            case "paladin chronomancer":
                EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                break;

            default:
                C.Logger($"[CoreEnhancements] No enhancement preset available for: {normalized}");
                break;
        }
    }

    public bool HasPreset(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return false;

        return className.Trim().ToLowerInvariant() switch
        {
            "archpaladin" => true,
            "stonecrusher" => true,
            "lord of order" => true,
            "dragon of time" => true,
            "void highlord" => true,
            "king's echo" => true,
            "kings echo" => true,
            _ => false,
        };
    }

    public void EnhanceEquipped(
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None
    )
    {
        List<InventoryItem> EquippedItems = Bot.Inventory.Items.FindAll(i =>
            i.Equipped == true && EnhanceableCatagories.Contains(i.Category)
        );
        try
        {
            AutoEnhance(EquippedItems, type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            C.Logger($"[CoreEnhancements] Enhancement crash: {e.Message}");
        }
    }

    public void EnhanceItem(
        string item,
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None,
        bool logging = false
    )
    {
        if (string.IsNullOrEmpty(item))
            return;

        if (C.CBOBool("DisableAutoEnhance", out bool _disableAutoEnhance) && _disableAutoEnhance)
            return;

        if (!C.CheckInventory(item))
        {
            C.Logger($"[CoreEnhancements] Enhancement Failed: Could not find \"{item}\"");
            return;
        }

        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            C.JumpWait();
            C.Sleep();
        }

        InventoryItem? SelectedItem = Bot.Inventory.Items.Find(i =>
            i.Name.Equals(item, StringComparison.OrdinalIgnoreCase)
            && EnhanceableCatagories.Contains(i.Category)
        );

        if (SelectedItem == null)
        {
            if (Bot.Inventory.Items.Any(i => i.Name.Equals(item, StringComparison.OrdinalIgnoreCase)))
                C.Logger($"[CoreEnhancements] Enhancement Failed: {item} cannot be enhanced");
            return;
        }

        try
        {
            AutoEnhance(new() { SelectedItem }, type, cSpecial, hSpecial, wSpecial, logging);
        }
        catch (Exception e)
        {
            C.Logger($"[CoreEnhancements] Enhancement crash: {e.Message}");
        }
    }

    public void EnhanceItem(
        string[] items,
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None
    )
    {
        if (items.Length == 0)
            return;

        if (C.CBOBool("DisableAutoEnhance", out bool _disableAutoEnhance) && _disableAutoEnhance)
            return;

        List<string> notFound = new();
        foreach (string item in items)
            if (!C.CheckInventory(item))
                notFound.Add(item);

        if (notFound.Count > 0)
        {
            if (notFound.Count == 1)
                C.Logger($"[CoreEnhancements] Enhancement Failed: Could not find {notFound.First()}");
            else
                C.Logger($"[CoreEnhancements] Enhancement Failed: Could not find the following items: {string.Join(", ", notFound)}");
            return;
        }

        List<InventoryItem> SelectedItems = Bot.Inventory.Items.FindAll(i =>
            items.Contains(i.Name) && EnhanceableCatagories.Contains(i.Category)
        );

        if (SelectedItems.Count != items.Length)
        {
            List<string> unEnhanceable = new();

            foreach (string item in items)
                if (!Bot.Inventory.Items.Any(i =>
                    i.Name == item && EnhanceableCatagories.Contains(i.Category)
                ))
                    unEnhanceable.Add(item);

            if (unEnhanceable.Count == 1)
                C.Logger($"[CoreEnhancements] Enhancement Failed: Unenhanceable item found, {unEnhanceable.First()}");
            else
                C.Logger($"[CoreEnhancements] Enhancement Failed: The following items are unenhanceable, {string.Join(", ", unEnhanceable)}");

            return;
        }

        try
        {
            AutoEnhance(SelectedItems, type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            C.Logger($"[CoreEnhancements] Enhancement crash: {e.Message}");
        }
    }

    private static readonly ItemCategory[] EnhanceableCatagories =
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
        ItemCategory.Whip,
        ItemCategory.Class,
        ItemCategory.Helm,
        ItemCategory.Cape,
    };

    private readonly ItemCategory[] WeaponCatagories = EnhanceableCatagories[..12];

    private void AutoEnhance(
        List<InventoryItem> ItemList,
        EnhancementType type,
        CapeSpecial cSpecial,
        HelmSpecial hSpecial,
        WeaponSpecial wSpecial,
        bool logging = false
    )
    {
        if (type == 0)
            return;

        if (ItemList.Count == 0)
        {
            C.Logger("[CoreEnhancements] Enhancement Failed:\t\"ItemList\" is empty");
            return;
        }

        InventoryItem? cape = null;
        if (cSpecial != CapeSpecial.None && ItemList.Any(i => i.Category == ItemCategory.Cape))
        {
            cape = ItemList.Find(i => i.Category == ItemCategory.Cape);
            if (cape != null)
                ItemList.Remove(cape);
        }

        InventoryItem? helm = null;
        if (hSpecial != HelmSpecial.None && ItemList.Any(i => i.Category == ItemCategory.Helm))
        {
            helm = ItemList.Find(i => i.Category == ItemCategory.Helm);
            if (helm != null)
                ItemList.Remove(helm);
        }

        InventoryItem? weapon = null;
        if (
            wSpecial != WeaponSpecial.None
            && ItemList.Any(i => i.ItemGroup == "Weapon")
            && (uAwe() || (int)wSpecial > 6)
        )
        {
            weapon = ItemList.Find(i => i.ItemGroup == "Weapon");
            if (weapon != null)
                ItemList.Remove(weapon);
        }

        bool weaponAlreadyEnhanced = weapon == null || AlreadyHasRequestedEnhancement(weapon, type, cSpecial, hSpecial, wSpecial, logging);

        if (ItemList.All(i => AlreadyHasRequestedEnhancement(i, type, cSpecial, hSpecial, wSpecial, logging))
            && (cape == null || AlreadyHasRequestedEnhancement(cape, type, cSpecial, hSpecial, wSpecial, logging))
            && (helm == null || AlreadyHasRequestedEnhancement(helm, type, cSpecial, hSpecial, wSpecial, logging))
            && weaponAlreadyEnhanced)
        {
            C.Logger("[CoreEnhancements] All equipped items already have requested enhancements. Skipping AutoEnhance.");
            return;
        }

        int skipCounter = 0;

        if (ItemList.Count > 0)
        {
            int shopID = type switch
            {
                EnhancementType.Fighter => Bot.Player.Level >= 50 ? 768 : 141,
                EnhancementType.Thief => Bot.Player.Level >= 50 ? 767 : 142,
                EnhancementType.Hybrid => Bot.Player.Level >= 50 ? 766 : 143,
                EnhancementType.Wizard => Bot.Player.Level >= 50 ? 765 : 144,
                EnhancementType.Healer => Bot.Player.Level >= 50 ? 762 : 145,
                EnhancementType.SpellBreaker => Bot.Player.Level >= 50 ? 764 : 146,
                EnhancementType.Lucky => Bot.Player.Level >= 50 ? 763 : 147,
                _ => 0,
            };

            if (shopID == 0)
            {
                C.Logger($"[CoreEnhancements] Enhancement Failed:\tInvalid EnhancementType given, received {(int)type} | {type}");
                return;
            }

            foreach (InventoryItem item in ItemList)
            {
                if (AlreadyHasRequestedEnhancement(item, type, cSpecial, hSpecial, wSpecial, logging))
                {
                    if (logging)
                        C.Logger($"[CoreEnhancements] Skipping already-enhanced item: \"{item.Name}\"");
                    skipCounter++;
                    continue;
                }

                _AutoEnhance(item, shopID, Bot.Map?.Name, logging);
                C.Sleep();
            }
        }

        if (cape != null)
        {
            bool canEnhance = true;

            switch (cSpecial)
            {
                case CapeSpecial.Forge:
                    if (!uForgeCape())
                    {
                        C.Logger("[CoreEnhancements] Enhancement Failed:\tYou did not unlock the Forge (Cape) Enhancement yet");
                        canEnhance = false;
                    }
                    break;
                case CapeSpecial.Absolution:
                    if (!uAbsolution())
                        Fail();
                    break;
                case CapeSpecial.Avarice:
                    if (!uAvarice())
                        Fail();
                    break;
                case CapeSpecial.Vainglory:
                    if (!uVainglory())
                        Fail();
                    break;
                case CapeSpecial.Penitence:
                    if (!uPenitence())
                        Fail();
                    break;
                case CapeSpecial.Lament:
                    if (!uLament())
                        Fail();
                    break;
                default:
                    C.Logger($"[CoreEnhancements] Enhancement Failed:\tInvalid \"CapeSpecial\" given, received {(int)cSpecial} | {cSpecial}");
                    return;

                    void Fail()
                    {
                        C.Logger($"[CoreEnhancements] Enhancement Failed:\tYou did not unlock the {cSpecial} Enhancement yet");
                        canEnhance = false;
                    }
            }

            if (canEnhance)
            {
                if (AlreadyHasRequestedEnhancement(cape, type, cSpecial, hSpecial, wSpecial, logging))
                {
                    if (logging)
                        C.Logger($"[CoreEnhancements] Skipping already-enhanced item: \"{cape.Name}\"");
                    skipCounter++;
                }
                else
                {
                    _AutoEnhance(cape, 2143, ((int)cSpecial > 0) ? "forge" : null, logging);
                }
            }
            else
                skipCounter++;
        }

        if (helm != null)
        {
            bool canEnhance = true;

            switch (hSpecial)
            {
                case HelmSpecial.Vim:
                    if (!uVim())
                        Fail();
                    break;
                case HelmSpecial.Examen:
                    if (!uExamen())
                        Fail();
                    break;
                case HelmSpecial.Forge:
                    if (!uForgeHelm())
                        Fail();
                    break;
                case HelmSpecial.Anima:
                    if (!uAnima())
                        Fail();
                    break;
                case HelmSpecial.Pneuma:
                    if (!uPneuma())
                        Fail();
                    break;
                case HelmSpecial.Hearty:
                    if (!uHearty())
                        Fail();
                    break;
                default:
                    C.Logger($"[CoreEnhancements] Enhancement Failed:\tInvalid \"HelmSpecial\" given, received {(int)hSpecial} | {hSpecial}");
                    return;

                    void Fail()
                    {
                        C.Logger($"[CoreEnhancements] Enhancement Failed:\tYou did not unlock the {hSpecial} Enhancement yet");
                        canEnhance = false;
                    }
            }

            if (canEnhance)
            {
                if (AlreadyHasRequestedEnhancement(helm, type, cSpecial, hSpecial, wSpecial, logging))
                {
                    if (logging)
                        C.Logger($"[CoreEnhancements] Skipping already-enhanced item: \"{helm.Name}\"");
                    skipCounter++;
                }
                else
                {
                    _AutoEnhance(helm, 2164, ((int)hSpecial > 0) ? "forge" : null);
                }
            }
            else
                skipCounter++;
        }

        if (weapon != null)
        {
            int shopID = 0;
            bool canEnhance = true;

            if ((int)wSpecial <= 6)
            {
                shopID = type switch
                {
                    EnhancementType.Fighter => 635,
                    EnhancementType.Thief => 637,
                    EnhancementType.Hybrid => 633,
                    EnhancementType.Wizard => 636,
                    EnhancementType.SpellBreaker => 636,
                    EnhancementType.Healer => 638,
                    EnhancementType.Lucky => 639,
                    _ => 0,
                };
            }
            else
            {
                switch (wSpecial)
                {
                    case WeaponSpecial.Forge:
                        if (!uForgeWeapon())
                        {
                            C.Logger("[CoreEnhancements] Enhancement Failed:\tYou did not unlock the Forge (Weapon) Enhancement yet");
                            canEnhance = false;
                        }
                        break;
                    case WeaponSpecial.Lacerate:
                        if (!uLacerate())
                            Fail();
                        break;
                    case WeaponSpecial.Smite:
                        if (!uSmite())
                            Fail();
                        break;
                    case WeaponSpecial.Valiance:
                        if (!uValiance())
                            Fail();
                        break;
                    case WeaponSpecial.Arcanas_Concerto:
                        if (!uArcanasConcerto())
                        {
                            C.Logger("[CoreEnhancements] Enhancement Failed:\tYou did not unlock the Arcana's Concerto Enhancement yet");
                            canEnhance = false;
                        }
                        break;
                    case WeaponSpecial.Elysium:
                        if (!uElysium())
                            Fail();
                        break;
                    case WeaponSpecial.Acheron:
                        if (!uAcheron())
                            Fail();
                        break;
                    case WeaponSpecial.Praxis:
                        if (!uPraxis())
                            Fail();
                        break;
                    case WeaponSpecial.Dauntless:
                        if (!uDauntless())
                            Fail();
                        break;
                    case WeaponSpecial.Ravenous:
                        if (!uRavenous())
                            Fail();
                        break;
                    default:
                        C.Logger($"[CoreEnhancements] Enhancement Failed:\tInvalid \"WeaponSpecial\" given, received {(int)wSpecial} | {wSpecial}");
                        return;

                        void Fail()
                        {
                            C.Logger($"[CoreEnhancements] Enhancement Failed:\tYou did not unlock the {wSpecial} Enhancement yet");
                            canEnhance = false;
                        }
                }

                shopID = 2142;
            }

            if (canEnhance)
            {
                if (AlreadyHasRequestedEnhancement(weapon, type, cSpecial, hSpecial, wSpecial, logging))
                {
                    if (logging)
                        C.Logger($"[CoreEnhancements] Skipping already-enhanced item: \"{weapon.Name}\"");
                    skipCounter++;
                }
                else
                {
                    _AutoEnhance(weapon, shopID, ((int)wSpecial > 6) ? "forge" : null, logging);
                }
            }
            else
                skipCounter++;
        }

        if (skipCounter > 0)
            C.Logger($"[CoreEnhancements] Enhancement Skipped:\t{skipCounter} item{(skipCounter > 1 ? 's' : null)}");

        void _AutoEnhance(InventoryItem item, int shopID, string? map = null, bool logging = false)
        {
            bool specialOnCape = item.Category == ItemCategory.Cape && cSpecial != CapeSpecial.None;
            bool specialOnHelm = item.Category == ItemCategory.Helm && hSpecial != HelmSpecial.None;
            bool specialOnWeapon = item.ItemGroup == "Weapon" && wSpecial.ToString() != "None";
            string mapName = map ?? Bot.Map?.Name ?? "whitemap";

            if (AlreadyHasRequestedEnhancement(item, type, cSpecial, hSpecial, wSpecial, logging))
                return;

            List<ShopItem> shopItems = C.GetShopItems(mapName, shopID);

            if (!shopItems.Any(x => x.Category == ItemCategory.Enhancement) || shopItems.Count == 0)
            {
                C.Logger($"[CoreEnhancements] Enhancement Failed for {item.Name}[{item.ID}], (EnhancementLevel: {item.EnhancementLevel}, map: {mapName}, shopID: {shopID}): Couldn't find enhancements in shop {shopID}");
                return;
            }

            if (logging && specialOnWeapon)
            {
                C.Logger($"[CoreEnhancements] Weapon enhancement search: {item.Name} Pattern={item.EnhancementPatternID} Proc={getProcID(item)} Requested={wSpecial}({(int)wSpecial}) shopID={shopID}");
            }

            if (Bot.Player.Level == item.EnhancementLevel)
            {
                if (specialOnCape)
                {
                    if ((int)cSpecial == item.EnhancementPatternID)
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if (specialOnHelm)
                {
                    if ((int)hSpecial == item.EnhancementPatternID)
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if (specialOnWeapon)
                {
                    bool procMatches = (int)wSpecial == getProcID(item)
                        || ((int)wSpecial == 99 && getProcID(item) == 0);

                    bool weaponAlreadyApplied;
                    if (wSpecial == WeaponSpecial.Forge)
                    {
                        weaponAlreadyApplied = procMatches
                            && (item.EnhancementPatternID == (int)type || item.EnhancementPatternID == 10);
                    }
                    else if ((int)wSpecial <= 6)
                    {
                        // Awe enhancements: both type AND proc must match
                        weaponAlreadyApplied = procMatches
                            && item.EnhancementPatternID == (int)type;
                    }
                    else
                    {
                        // Forge enhancements: only proc matters (overrides base type)
                        weaponAlreadyApplied = procMatches;
                    }

                    if (weaponAlreadyApplied)
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if ((int)type == item.EnhancementPatternID)
                {
                    skipCounter++;
                    return;
                }
            }

            if (logging)
            {
                if (specialOnCape)
                    C.Logger($"[CoreEnhancements] Searching Enhancement:\tForge/{cSpecial.ToString().Replace("_", " ")} - \"{item.Name}\"");
                else if (specialOnWeapon)
                    C.Logger($"[CoreEnhancements] Searching Enhancement:\t{((int)wSpecial <= 6 ? type : "Forge")}/{wSpecial.ToString().Replace("_", " ")} - \"{item.Name}\"");
                else
                    C.Logger($"[CoreEnhancements] Searching Enhancement:\t{type} - \"{item.Name}\"");
            }

            List<ShopItem> availableEnh = new();
            foreach (ShopItem enh in shopItems)
            {
                if ((!Bot.Player.IsMember && enh.Upgrade) || (enh.Level > Bot.Player.Level))
                    continue;

                string enhName = enh.Name.Replace(" ", "").Replace("'", "").ToLower();

                if (specialOnCape && enhName.Contains(cSpecial.ToString().Replace("_", "").ToLower()))
                    availableEnh.Add(enh);
                else if (specialOnWeapon && enhName.Contains(wSpecial.ToString().Replace("_", "").ToLower()))
                    availableEnh.Add(enh);
                else if (specialOnHelm && enhName.Contains(hSpecial.ToString().Replace("_", "").ToLower()))
                    availableEnh.Add(enh);
                else if (item.Category == ItemCategory.Class && enhName.Contains("armor"))
                    availableEnh.Add(enh);
                else if (item.Category == ItemCategory.Helm && enhName.Contains("helm"))
                    availableEnh.Add(enh);
                else if (item.Category == ItemCategory.Cape && enhName.Contains("cape"))
                    availableEnh.Add(enh);
                else if (item.ItemGroup == "Weapon" && enhName.Contains("weapon"))
                    availableEnh.Add(enh);
            }

            ShopItem? bestEnhancement = null;
            if (availableEnh.Count == 0)
            {
                if (logging)
                    C.Logger("[CoreEnhancements] Enhancement Failed:\t\"availableEnh\" is empty");
                return;
            }
            else if (availableEnh.Count == 1)
                bestEnhancement = availableEnh.First();
            else
            {
                List<ShopItem> sortedList = availableEnh
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Upgrade ? 1 : 0)
                    .ToList();
                bestEnhancement = sortedList[0];
            }

            if (bestEnhancement == null)
            {
                if (logging)
                    C.Logger($"[CoreEnhancements] Enhancement Failed:\tCould not find the best enhancement for \"{item.Name}\"");
                return;
            }

            if (bestEnhancement.ID == getEnhID(item)
                && item.EnhancementLevel > 0
                && bestEnhancement.Level == item.EnhancementLevel)
            {
                if (logging)
                    C.Logger($"[CoreEnhancements] Enhancement Canceled:\tBest enhancement is already applied for \"{item.Name}\"");
                return;
            }

            int roomId = Bot.Map?.RoomID ?? 1;
            Bot.Send.Packet($"%xt%zm%enhanceItemShop%{roomId}%{item.ID}%{bestEnhancement.ID}%{shopID}%");

            if (logging)
            {
                if (specialOnCape)
                    C.Logger($"[CoreEnhancements] Enhancement Applied:\tForge/{cSpecial.ToString().Replace("_", " ")} - \"{item.Name}\" (Lvl {bestEnhancement.Level})");
                else if (specialOnWeapon)
                    C.Logger($"[CoreEnhancements] Enhancement Applied:\t{((int)wSpecial <= 6 ? type : "Forge")}/{wSpecial.ToString().Replace("_", " ")} - \"{item.Name}\" (Lvl {bestEnhancement.Level})");
                else
                    C.Logger($"[CoreEnhancements] Enhancement Applied:\t{type} - \"{item.Name}\" (Lvl {bestEnhancement.Level})");
            }
            C.Sleep();
        }
    }

    private bool AlreadyHasRequestedEnhancement(
        InventoryItem item,
        EnhancementType type,
        CapeSpecial cSpecial,
        HelmSpecial hSpecial,
        WeaponSpecial wSpecial,
        bool logging = false
    )
    {
        if (item == null || item.EnhancementLevel <= 0 || item.EnhancementLevel != Bot.Player.Level)
            return false;

        if (item.Category == ItemCategory.Cape)
        {
            if (cSpecial != CapeSpecial.None)
                return (int)cSpecial == item.EnhancementPatternID;

            return (int)type == item.EnhancementPatternID;
        }

        if (item.Category == ItemCategory.Helm)
        {
            if (hSpecial != HelmSpecial.None)
                return (int)hSpecial == item.EnhancementPatternID;

            return (int)type == item.EnhancementPatternID;
        }

        if (item.ItemGroup == "Weapon")
        {
            if (wSpecial == WeaponSpecial.None)
                return (int)type == item.EnhancementPatternID;

            return WeaponAlreadyHasRequestedEnhancement(item, type, wSpecial);
        }

        if (item.Category == ItemCategory.Class)
            return (int)type == item.EnhancementPatternID;

        return false;
    }

    private int getProcID(InventoryItem? item) =>
        item == null ? 0 : C.GetItemProperty<int>(item, "ProcID");

    private int getEnhID(InventoryItem? item) =>
        item == null ? 0 : C.GetItemProperty<int>(item, "iEnh");

    private bool WeaponAlreadyHasRequestedEnhancement(
        InventoryItem item,
        EnhancementType type,
        WeaponSpecial wSpecial
    )
    {
        if (item == null || item.ItemGroup != "Weapon" || item.EnhancementLevel <= 0)
            return false;

        int currentPattern = item.EnhancementPatternID;
        int currentProc = getProcID(item);

        if (wSpecial == WeaponSpecial.None)
            return currentPattern == (int)type;

        if ((int)wSpecial <= 6)
            return currentPattern == (int)type && currentProc == (int)wSpecial;

        if (wSpecial == WeaponSpecial.Forge)
            return currentProc == 0 && (currentPattern == (int)type || currentPattern == 10);

        return currentProc == (int)wSpecial;
    }

    public bool uAwe() => C.isCompletedBefore(2937);
    public bool uForgeWeapon() => C.isCompletedBefore(8738);
    public bool uLacerate() => C.isCompletedBefore(8739);
    public bool uSmite() => C.isCompletedBefore(8740);
    public bool uValiance() => C.isCompletedBefore(8741);
    public bool uArcanasConcerto() => C.isCompletedBefore(8742);
    public bool uAbsolution() => C.isCompletedBefore(8743);
    public bool uVainglory() => C.isCompletedBefore(8744);
    public bool uAvarice() => C.isCompletedBefore(8745);
    public bool uForgeCape() => C.isCompletedBefore(8758);
    public bool uElysium() => C.isCompletedBefore(8821);
    public bool uAcheron() => C.isCompletedBefore(8820);
    public bool uPenitence() => C.isCompletedBefore(8822);
    public bool uLament() => C.isCompletedBefore(8823);
    public bool uVim() => C.isCompletedBefore(8824);
    public bool uExamen() => C.isCompletedBefore(8825);
    public bool uForgeHelm() => C.isCompletedBefore(8828);
    public bool uPneuma() => C.isCompletedBefore(8827);
    public bool uAnima() => C.isCompletedBefore(8826);
    public bool uDauntless() => C.isCompletedBefore(9172);
    public bool uPraxis() => C.isCompletedBefore(9171);
    public bool uRavenous() => C.isCompletedBefore(9560);
    public bool uHearty()
    {
        return C.isCompletedBefore(9466) && FactionRank("Grimskull Trolling") >= 7;
    }

    private int FactionRank(string faction)
    {
        if (string.IsNullOrWhiteSpace(faction))
            return 0;

        return Bot.Reputation.GetRank(faction);
    }
}