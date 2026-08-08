/*
name: Solstice Moon Test
description: Runs only the Solstice Moon / Shrine - Left portion for testing Lunar Haze and Hollow Midnight taunt timing.
tags: solstice, moon, hollow midnight, lunar haze, army, taunt, test
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Models.Quests;
using System;
using System.IO;
using System.Linq;

public class SolsticeMoonTest
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    public CoreEngine Engine = new();
    public CoreUltra Ultra = new();
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "SolsticeMoon_Test";
    public List<IOption> Options = new()
    {
        new Option<string>("player1", "LR Account", "Name of the account that will use Legion Revenant.", ""),
        new Option<string>("player2", "SC Account", "Name of the account that will use StoneCrusher.", ""),
        new Option<string>("player3", "AP Account", "Name of the account that will use ArchPaladin.", ""),
        new Option<string>("player4", "LoO Account", "Name of the account that will use Lord of Order.", ""),

        new Option<int>(
            "privateRoomNumber",
            "Private Room Number (IMPORTANT)",
            "Private room number used by all 4 accounts. Set the same number on every account!",
            69420
        ),

        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,

        new Option<bool>(
            "autoEnhance",
            "Auto-Apply Enhancements",
            "ON: each account automatically applies the correct enhancements for its fixed class.\n" +
            "Slot 1 (LR): Wizard/Wizard Helm/Elysium/Penitence\n" +
            "Slot 2 (SC): Fighter/Fighter Helm/Valiance/Absolution\n" +
            "Slot 3 (AP): Lucky/Lucky Helm/Valiance/Penitence\n" +
            "Slot 4 (LoO): Lucky/Lucky Helm/Valiance/Penitence",
            true
        ),
        new Option<bool>(
            "usePotions",
            "Use Potions",
            "ON: equips and uses configured class potion sets before starting the dungeon loop. (TONICS and ELIXIRS ONLY)\n" +
            "OFF: skips potion usage.",
            false
        ),
    };

    // ── Item names ────────────────────────────────────────────────────────────
    const string SliverSunlight = "Sliver of Sunlight";
    const string SliverMoonlight = "Sliver of Moonlight";
    const string Solarbrand = "Solarbrand";
    const string Lunarbrand = "Lunarbrand";
    const string BladeBurningSun = "Blade of the Burning Sun";
    const string BladeGlowingMoon = "Blade of the Glowing Moon";
    const string GreatMidnightSun = "Greatblade of the Midnight Sun";
    const string GreatSolsticeMoon = "Greatblade of the Solstice Moon";
    // EO phase items — uncomment when ascendeclipse is ready
    // const string EclipticOffering  = "Ecliptic Offering"; // confirm: "Hallowed Remains"?
    // const string Umbrabrand        = "Umbrabrand";
    // const string BladeBoundEclipse = "Blade of the Bound Eclipse";
    // const string GreatEntwinedEcl  = "Greatblade of the Entwined Eclipse";

    // ── Shop ─────────────────────────────────────────────────────────────────
    const string MergeMap = "templeshrine";
    const int MergeShop = 2303;

    // Shop item IDs (prevents name-lookup ambiguity)
    const int ID_Solarbrand = 78465;
    const int ID_Lunarbrand = 78460;
    const int ID_BladeBurningSun = 78466;
    const int ID_BladeGlowingMoon = 78461;
    const int ID_GreatMidnightSun = 78467;
    const int ID_GreatSolsticeMoon = 78462;
    // const int ID_Umbrabrand        = 78455;
    // const int ID_BladeBoundEclipse = 78456;
    // const int ID_GreatEntwinedEcl  = 78457;
    // const int ID_RiteOfAscension   = 78809;

    // ── Totals for basic blade chain ─────────────────────────────────────────
    // Solarbrand ×3  (5  Sun ea)  →  15 Sun
    // BBS        ×2  (50 Sun ea)  → 100 Sun
    // GreatMidSun    (100 Sun)    → 100 Sun  ───  215 Sun total
    //
    // Lunarbrand ×3  (5  Moon ea) →  15 Moon
    // BGM        ×2  (50 Moon ea) → 100 Moon
    // GreatSolMoon   (100 Moon)   → 100 Moon ─── 215 Moon total
    const int SunlightNeeded = 215;
    const int MoonlightNeeded = 215;

    bool autoEnhance;
    bool autoGetEnrage;
    bool checkUsableSlotBeforeBossRooms;
    bool usePotions;

    // Fixed 4-class lineup matching the working reference script
    const string player1Class = "Legion Revenant";  // Lunar Haze taunter
    const string player2Class = "StoneCrusher";     // DPS/support
    const string player3Class = "ArchPaladin";      // Hollow Midnight taunter
    const string player4Class = "Lord of Order";    // Hollow Midnight taunter

    readonly int[] lrSkillList = new[] { 3, 4, 2, 1 };
    readonly int[] scSkillList = new[] { 3, 2, 4, 1 };
    readonly int[] apSkillList = new[] { 2, 3, 1, 4 };
    readonly int[] looSkillList = new[] { 2, 3, 1, 4 };

    int midnightRunCount;
    int solsticeRunCount;
    bool syncFilesClearedOnStartup;

    /// <summary>
    /// Optional orchestration hook: when > 0, the main loop exits once the account holds
    /// at least this many Sliver of Moonlight. Leave 0 for the normal "loop forever" behavior.
    /// </summary>
    public int TargetSliverCount;

    /// <summary>
    /// Orchestration hook: when true, skip Core.SetOptions(true/false). Used when this
    /// script is invoked from a parent orchestrator that runs its own SetOptions.
    /// </summary>
    public bool SkipSetOptions;

    public void ScriptMain(IScriptInterface bot)
    {
        if (!SkipSetOptions)
            Core.SetOptions(disableClassSwap: true);
        if (sArmy.Players().Length < 4)
        {
            Core.Logger("Add 4 account names in the script options before starting the army farm.");
            Core.SetOptions(false);
            return;
        }

        Core.PrivateRooms = true;
        int configuredRoom = Bot.Config!.Get<int>("privateRoomNumber");
        if (configuredRoom >= 1000 && configuredRoom <= 99999)
        {
            Core.PrivateRoomNumber = configuredRoom;
        }
        else
        {
            Core.Logger($"Invalid private room number '{configuredRoom}'. Generating a fallback room number.");
            Core.PrivateRoomNumber = sArmy.getRoomNr();
        }

        Core.Logger($"Army mode enabled: {sArmy.Players().Length} accounts, private room #{Core.PrivateRoomNumber}.");


        autoEnhance = Bot.Config.Get<bool>("autoEnhance");
        autoGetEnrage = true;
        checkUsableSlotBeforeBossRooms = true;
        usePotions = Bot.Config.Get<bool>("usePotions");

        EquipArmyClasses();

        if (autoEnhance)
            ApplyEnhancements();

        if (usePotions)
            UseClassPotions();

        PrepareTauntRole();

        TryAcceptDailyQuest(9303, "Night Falls");

        Core.AddDrop(SliverMoonlight);

        try
        {
            while (!Bot.ShouldExit)
            {
                if (TargetSliverCount > 0 && Core.CheckInventory(SliverMoonlight, TargetSliverCount))
                {
                    Core.Logger($"[SolsticeMoon] Reached target of {TargetSliverCount} {SliverMoonlight}; stopping.");
                    break;
                }
                RunSolsticeMoon();
            }
        }
        finally
        {
            if (!SkipSetOptions)
                Core.SetOptions(false);
        }
    }

    // ── Mode 1: Farm all materials first, then merge everything ───────────────

    void GrindFarmFirst()
    {
        Core.Logger("Mode: Farm Max Stack First");
        FarmSunlightTo(SunlightNeeded);
        FarmMoonlightTo(MoonlightNeeded);
        MergeBasicChain();
    }

    // ── Mode 2: Farm just enough for each weapon, merge as you go ─────────────

    void GrindIncremental()
    {
        Core.Logger("Mode: Merge As Available");

        // Iteration 1 — first Solarbrand, Lunarbrand, BBS, BGM
        FarmSunlightTo(5); MergeToHave(Solarbrand, ID_Solarbrand, 1);
        FarmMoonlightTo(5); MergeToHave(Lunarbrand, ID_Lunarbrand, 1);
        FarmSunlightTo(50); MergeToHave(BladeBurningSun, ID_BladeBurningSun, 1);
        FarmMoonlightTo(50); MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 1);

        // Iteration 2 — second set
        FarmSunlightTo(5); MergeToHave(Solarbrand, ID_Solarbrand, 2);
        FarmMoonlightTo(5); MergeToHave(Lunarbrand, ID_Lunarbrand, 2);
        FarmSunlightTo(50); MergeToHave(BladeBurningSun, ID_BladeBurningSun, 2);
        FarmMoonlightTo(50); MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 2);

        // Third Solar + Lunar (held for Umbrabrand in the EO phase)
        FarmSunlightTo(5); MergeToHave(Solarbrand, ID_Solarbrand, 3);
        FarmMoonlightTo(5); MergeToHave(Lunarbrand, ID_Lunarbrand, 3);

        // Greatblades — one per side
        FarmSunlightTo(100); MergeToHave(GreatMidnightSun, ID_GreatMidnightSun, 1);
        FarmMoonlightTo(100); MergeToHave(GreatSolsticeMoon, ID_GreatSolsticeMoon, 1);

        Core.Logger("Basic blade chain complete! Remaining: 1× Solarbrand, 1× Lunarbrand, 1× BBS, 1× BGM — held for the Ecliptic Offering phase.");
    }

    // ── Merge helpers ─────────────────────────────────────────────────────────

    void MergeBasicChain()
    {
        Core.Logger("Starting basic blade merge chain...");
        // 3× Solarbrand — 2 consumed by BBS, 1 held for Umbrabrand
        MergeToHave(Solarbrand, ID_Solarbrand, 3);
        // 3× Lunarbrand — 2 consumed by BGM, 1 held for Umbrabrand
        MergeToHave(Lunarbrand, ID_Lunarbrand, 3);
        // 2× BBS — 1 consumed by GreatMidnightSun, 1 held for BoBE
        MergeToHave(BladeBurningSun, ID_BladeBurningSun, 2);
        // 2× BGM — 1 consumed by GreatSolsticeMoon, 1 held for BoBE
        MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 2);
        MergeToHave(GreatMidnightSun, ID_GreatMidnightSun, 1);
        MergeToHave(GreatSolsticeMoon, ID_GreatSolsticeMoon, 1);
        Core.Logger("Basic blade chain complete! Remaining: 1× Solarbrand, 1× Lunarbrand, 1× BBS, 1× BGM — held for the Ecliptic Offering phase.");
    }

    /// <summary>
    /// Merges until we own at least <paramref name="targetTotal"/> of the item.
    /// Accounts for what's already in inventory — won't over-merge.
    /// </summary>
    void MergeToHave(string itemName, int shopItemID, int targetTotal)
    {
        int have = Bot.Inventory.GetQuantity(itemName);
        int toBuy = targetTotal - have;
        if (toBuy <= 0) return;

        Core.Logger($"Merging {toBuy}× {itemName} ({have} already owned)...");
        Core.BuyItem(MergeMap, MergeShop, itemName, toBuy, shopItemID);
    }

    // ── Farming ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Farms Sliver of Sunlight until we own at least <paramref name="target"/>.
    /// In army mode, every listed account must reach the target before moving on.
    /// </summary>
    void FarmSunlightTo(int target)
    {
        if (ArmyHasItem(SliverSunlight, target, $"GreatbladeSunlightProgress_{target}.sync"))
            return;
        Core.AddDrop(SliverSunlight);
        Core.Logger($"Farming Sliver of Sunlight (need {target}, have {Bot.Inventory.GetQuantity(SliverSunlight)})...");
        while (!Bot.ShouldExit && !ArmyHasItem(SliverSunlight, target, $"GreatbladeSunlightProgress_{target}.sync"))
            RunMidnightSun();
    }

    /// <summary>
    /// Farms Sliver of Moonlight until we own at least <paramref name="target"/>.
    /// In army mode, every listed account must reach the target before moving on.
    /// </summary>
    void FarmMoonlightTo(int target)
    {
        if (ArmyHasItem(SliverMoonlight, target, $"GreatbladeMoonlightProgress_{target}.sync"))
            return;
        Core.AddDrop(SliverMoonlight);
        Core.Logger($"Farming Sliver of Moonlight (need {target}, have {Bot.Inventory.GetQuantity(SliverMoonlight)})...");
        while (!Bot.ShouldExit && !ArmyHasItem(SliverMoonlight, target, $"GreatbladeMoonlightProgress_{target}.sync"))
            RunSolsticeMoon();
    }

    string SyncGroupPath(string syncFile)
    {
        string group = GetSyncGroup(syncFile);
        return Ultra.ResolveSyncPath($"SolsticeMoon_{Core.PrivateRoomNumber}_{group}.sync");
    }

    string ItemProgressSyncPath(string syncFile) =>
        Ultra.ResolveSyncPath($"SolsticeMoon_{Core.PrivateRoomNumber}_{syncFile}");

    string GetSyncGroup(string syncFile)
    {
        string name = syncFile.ToLower();

        if (name.Contains("party") || name.Contains("setup"))
            return "setup";

        if (name.Contains("reset") || name.Contains("whitemap") || name.Contains("rejoined") || name.Contains("restock"))
            return "reset";

        return "run";
    }

    void ResetReusableSyncFiles()
    {

        Core.Logger("[Sync] Moving to /whitemap before clearing reusable sync files.");
        Core.Join("whitemap", "Enter", "Spawn");
        WaitForMapName("whitemap", 8000);
        Bot.Sleep(1000);

        bool isPlayer1 = IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "");

        if (!isPlayer1)
        {
            if (!syncFilesClearedOnStartup)
                Core.Logger("[Sync] Waiting briefly while player1 clears startup sync files.");
            else
                Core.Logger("[Sync] Waiting briefly while player1 resets reusable reset sync file.");

            Bot.Sleep(1500);

            if (!syncFilesClearedOnStartup)
            {
                syncFilesClearedOnStartup = true;
                Core.Logger("[Sync] Done waiting for player1 startup sync clear.");
            }
            else
            {
                Core.Logger("[Sync] Done waiting for player1 sync reset.");
            }

            return;
        }

        if (!syncFilesClearedOnStartup)
        {
            Core.Logger("[Sync] Player1 is clearing startup sync files for SolsticeMoon.");

            ResetSyncFile(SyncGroupPath("SolsticeMoon_reset_ready.sync"));
            ResetSyncFile(SyncGroupPath("SolsticeMoon_run_ready.sync"));

            syncFilesClearedOnStartup = true;

            Core.Logger("[Sync] Startup sync files cleared.");
            Bot.Sleep(1500);
            return;
        }

        Core.Logger("[Sync] Player1 is resetting reusable reset sync file for the new SolsticeMoon run.");

        ResetSyncFile(SyncGroupPath("SolsticeMoon_reset_ready.sync"));

        Core.Logger("[Sync] Reusable reset sync file reset complete.");
        Bot.Sleep(1500);
    }

    void ResetSyncFile(string path)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, string.Empty);
        }
        catch
        {
            Bot.Sleep(100);
        }
    }

    void SyncArmy(string syncFile)
    {
        int partySize = sArmy.Players().Length;
        if (partySize <= 1)
            return;

        string path = SyncGroupPath(syncFile);
        string username = Core.Username().ToLower();
        string checkpoint = syncFile.Replace(":", "_");

        while (!Bot.ShouldExit)
        {
            string[] lines = ReadSyncLines(path);

            lines = lines
                .Where(line => !line.StartsWith($"{checkpoint}:{username}:", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            WriteSyncLines(
                path,
                lines
                    .Append($"{checkpoint}:{username}:ready:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}")
                    .ToArray()
            );

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            int ready = ReadSyncLines(path)
                .Select(line => line.Split(':'))
                .Where(parts =>
                    parts.Length >= 4 &&
                    parts[0].Equals(checkpoint, StringComparison.OrdinalIgnoreCase) &&
                    long.TryParse(parts[3], out long ts) &&
                    now - ts <= 120)
                .Select(parts => parts[1])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            if (ready >= partySize)
                return;

            Bot.Sleep(250);
        }
    }

    string[] ReadSyncLines(string path)
    {
        try
        {
            return File.Exists(path)
                ? File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray()
                : Array.Empty<string>();
        }
        catch
        {
            Bot.Sleep(100);
            return Array.Empty<string>();
        }
    }

    void WriteSyncLines(string path, string[] lines)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllLines(path, lines);
        }
        catch
        {
            Bot.Sleep(100);
        }
    }

    bool ArmyHasItem(string itemName, int target, string syncFile) =>
        ArmyProgress(itemName, target, syncFile);

    bool ArmyProgress(string itemName, int target, string syncFile)
    {
        int partySize = sArmy.Players().Length;
        string path = ItemProgressSyncPath(syncFile);
        string username = Core.Username().ToLower();
        int quantity = Bot.Inventory.GetQuantity(itemName);

        string[] lines = ReadSyncLines(path);
        lines = lines.Where(line => !line.StartsWith($"{username}:", StringComparison.OrdinalIgnoreCase)).ToArray();
        WriteSyncLines(path, lines.Append($"{username}:{quantity}:{target}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}").ToArray());

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int complete = ReadSyncLines(path)
            .Select(line => line.Split(':'))
            .Where(parts => parts.Length >= 4 && long.TryParse(parts[3], out long ts) && now - ts <= 120)
            .GroupBy(parts => parts[0].ToLower())
            .Select(group => group.Last())
            .Count(parts => int.TryParse(parts[1], out int current) && current >= target);

        return complete >= partySize;
    }

    void ArmyKillMonster(string map, string cell, string pad, string monster, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");
        KillFocusedMonster(map, monster, cell, pad);
        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillWithDelayedTaunt(string map, string cell, string pad, string monster, string checkpoint, bool sunSide, string enrageMessage, int tauntOffsetSeconds = 6)
    {
        JoinAndFocus(map, cell, pad);

        // Sun side (midnightsun): player1+2 alternate — player1 "starts" so player2 fires first.
        // Moon side (solsticemoon): player3+4 alternate — player3 "starts" so player4 fires first.
        bool isTaunter = sunSide ? IsSunTaunter() : IsMoonTaunter();
        string startingTaunterConfig = sunSide ? "player1" : "player3";

        if (isTaunter)
            CheckUsableSlotBeforeBossRoom($"{monster} / {enrageMessage}");

        SyncArmy($"{checkpoint}_ready.sync");

        BossFight(monster, cell, enrageMessage, isTaunter, tauntOffsetSeconds, startingTaunterConfig);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillLunarHazeWithLrTaunt(string map, string cell, string pad, string secondaryMob, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        KillLunarHazeWithLrOnlyTaunt(cell, pad, secondaryMob);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void KillLunarHazeWithLrOnlyTaunt(string cell, string pad, string secondaryMob)
    {
        const string lunarHaze = "Lunar Haze";
        const string lunarAura = "Moonlight Gaze";
        const int tauntDelayMs = 6000;

        long noTargetSince = 0;

        bool lrMoonlightGazeArmed = false;
        bool lrMoonlightGazeTaunted = false;
        long lrTauntAt = 0;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                if (Bot.Player.Cell != cell)
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                }

                Bot.Player.SetSpawnPoint();
                Bot.Sleep(500);
                continue;
            }

            if (Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
                Bot.Player.SetSpawnPoint();
            }

            bool isLegionRevenant = string.Equals(
                Bot.Player.CurrentClass?.Name,
                "Legion Revenant",
                StringComparison.OrdinalIgnoreCase
            );

            bool lunarHazeAlive = MonsterAvailable(lunarHaze, cell);
            bool secondaryAlive = MonsterAvailable(secondaryMob, cell);

            string target = lunarHazeAlive
                ? lunarHaze
                : secondaryAlive
                    ? secondaryMob
                    : string.Empty;

            if (!string.IsNullOrWhiteSpace(target))
                Bot.Combat.Attack(target);

            bool handledTauntWindow = false;

            if (isLegionRevenant && target == lunarHaze && lunarHazeAlive)
            {
                bool moonlightGazeActive = Bot.Self.HasActiveAura(lunarAura);

                if (moonlightGazeActive && !lrMoonlightGazeArmed)
                {
                    lrMoonlightGazeArmed = true;
                    lrMoonlightGazeTaunted = false;
                    lrTauntAt = Environment.TickCount64 + tauntDelayMs;
                    Core.Logger($"[Lunar Haze LR Taunt] {lunarAura} detected on {lunarHaze}. Taunting in {tauntDelayMs / 1000.0:0.#} seconds.");
                }

                if (lrMoonlightGazeArmed && !lrMoonlightGazeTaunted && Environment.TickCount64 >= lrTauntAt)
                {
                    lrMoonlightGazeTaunted = TauntCurrentTarget("[Lunar Haze LR Taunt]", lunarHaze);
                    handledTauntWindow = true;
                }

                if (!moonlightGazeActive && lrMoonlightGazeTaunted)
                {
                    lrMoonlightGazeArmed = false;
                    lrMoonlightGazeTaunted = false;
                    lrTauntAt = 0;
                }
            }
            else
            {
                lrMoonlightGazeArmed = false;
                lrMoonlightGazeTaunted = false;
                lrTauntAt = 0;
            }

            if (!handledTauntWindow)
            {
                UseClassSkills();
                Bot.Sleep(400);
            }

            if (!lunarHazeAlive && !secondaryAlive)
            {
                noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;

                if (Environment.TickCount64 - noTargetSince > 1800)
                    break;
            }
            else
            {
                noTargetSince = 0;
            }
        }
    }

    void ArmyKillShrineBoss(string map, string cell, string pad, string monster, string checkpoint, bool moonBoss)
    {
        JoinAndFocus(map, cell, pad);

        // Sun boss: player1+2 alternate — player1 fires first, player2 is "starting" (defers first).
        // Moon boss: player3+4 alternate — player3 fires first, player4 is "starting" (defers first).
        bool isTaunter = moonBoss ? IsMoonTaunter() : IsSunTaunter();
        string enrageMessage = moonBoss ? "The Moon Converges" : "The Sun Converges";
        string startingTaunterConfig = moonBoss ? "player4" : "player2";

        if (isTaunter)
            CheckUsableSlotBeforeBossRoom($"{monster} / {enrageMessage}");

        SyncArmy($"{checkpoint}_ready.sync");

        BossFight(monster, cell, enrageMessage, isTaunter, 0, startingTaunterConfig);

        if (string.Equals(cell, "r3", StringComparison.OrdinalIgnoreCase))
            Core.Logger($"[r3] {monster} fight complete. Waiting for army done sync.");

        SyncArmy($"{checkpoint}_done.sync");
    }


    void ResetDungeonInstance(string nextMap, string checkpoint)
    {
        SyncArmy($"{checkpoint}_reset_ready.sync");

        string whiteMapTarget = Core.PrivateRooms
            ? $"whitemap-{Core.PrivateRoomNumber}"
            : "whitemap";

        Core.Logger($"Resetting cleared dungeon by joining /{whiteMapTarget}...");
        Core.Join("whitemap", "Enter", "Spawn");
        WaitForMapName("whitemap", 8000);
        Bot.Sleep(1000);

        SyncArmy($"{checkpoint}_all_in_whitemap.sync");

        SellHallowedRemainsIfMax();
        RefreshPotionsIfAurasMissing();
        RestockEnrageIfLow($"Before {checkpoint}", minimumCount: 80);
        EnsureEnrageEquipped("Restock");

        SyncArmy($"{checkpoint}_restock_done.sync");

        JoinShrineDungeon(nextMap, "Enter", "Left", force: true);
        Bot.Sleep(1000);

        SyncArmy($"{checkpoint}_all_rejoined_dungeon.sync");
    }

    int GetInventoryQuantity(string itemName)
    {
        return Bot.Inventory.Items
            .FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            ?.Quantity ?? 0;
    }

    void SellHallowedRemainsIfMax()
    {
        const string itemName = "Hallowed Remains";
        const int maxStack = 500;

        int quantity = Bot.Inventory.GetQuantity(itemName);

        if (quantity < maxStack)
            return;

        Core.Logger($"[Inventory] {itemName} is at max stack ({quantity}/{maxStack}). Selling all.");

        Core.SellItem(itemName, all: true);

        Bot.Sleep(1000);
    }

    void TryAcceptDailyQuest(int questId, string questName)
    {
        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questId));

        if (quest == null)
        {
            Core.Logger($"[Daily] Failed to load {questName} [{questId}]. Skipping.");
            return;
        }

        if (Bot.Quests.IsDailyComplete(questId))
        {
            Core.Logger($"[Daily] {quest.Name} [{quest.ID}] is not available right now.");
            return;
        }

        Core.Logger($"[Daily] Accepting {quest.Name} [{quest.ID}].");
        Core.EnsureAccept(questId);
        Core.AddDrop(quest.Rewards.Select(x => x.Name).ToArray());
        Core.AddDrop(quest.Requirements.Select(x => x.Name).ToArray());
    }

    void TryCompleteDailyQuest(int questId, string rewardName)
    {
        if (!Bot.Quests.IsInProgress(questId))
            return;

        Core.Logger($"[Daily] Attempting to complete daily quest {questId}.");

        Core.EnsureComplete(questId);
        Bot.Wait.ForPickup(rewardName);

        Core.Logger($"[Daily] Completed daily quest {questId}.");
    }

    void RestockEnrageIfLow(string context, int minimumCount = 80)
    {
        bool needsEnrage =
            IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player2") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player3") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player4") ?? "");

        if (!needsEnrage)
            return;

        if (!string.Equals(Bot.Map.Name, "whitemap", StringComparison.OrdinalIgnoreCase))
        {
            Core.Logger($"[Taunt] {context}: skipping Scroll of Enrage restock because this account is not in /whitemap.");
            return;
        }

        int count = GetInventoryQuantity("Scroll of Enrage");
        Core.Logger($"[Taunt] {context}: {count} Scroll of Enrage remaining.");

        if (count >= minimumCount)
            return;

        Core.Logger($"[Taunt] Scroll of Enrage is below {minimumCount}. Restocking before continuing.");

        Ultra.GetScrollOfEnrage();

        int newCount = GetInventoryQuantity("Scroll of Enrage");
        Core.Logger($"[Taunt] Restock complete: {newCount} Scroll of Enrage remaining.");

        EnsureEnrageEquipped("restock");
    }

    void JoinAndFocus(string map, string cell, string pad)
    {
        if (map == "midnightsun" || map == "solsticemoon")
            JoinShrineDungeon(map, cell, pad);
        else
            Core.Join(map, cell, pad);

        if ((map == "midnightsun" || map == "solsticemoon") && Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Count() < sArmy.Players().Length)
        {
            Core.Logger($"Only {Bot.Map.PlayerNames.Count()}/{sArmy.Players().Length} players visible in /{map}-{Core.PrivateRoomNumber}; retrying dungeon join.");
            JoinShrineDungeon(map, cell, pad, force: true);
            Bot.Sleep(1000);
        }

        if (Bot.Player.Cell != cell)
        {
            Bot.Map.Jump(cell, pad, autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        Bot.Player.SetSpawnPoint();
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
    }

    void JoinShrineDungeon(string map, string cell, string pad, bool force = false)
    {
        string target = Core.PrivateRooms ? $"{map}-{Core.PrivateRoomNumber}" : map;

        for (int attempt = 1; attempt <= 5 && !Bot.ShouldExit && (force || Bot.Map.Name != map); attempt++)
        {
            force = false;
            Core.Logger($"Joining /{target} ({attempt}/5)...");
            Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%{target}%");
            WaitForMapName(map, 8000);

            if (Bot.Map.Name == map)
                break;

            Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");
            WaitForMapName(map, 8000);
            Bot.Sleep(500);
        }

        if (Bot.Map.Name != map)
        {
            Core.Logger($"Could not join /{target}. Retrying next loop.");
            return;
        }

        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
    }

    void WaitForMapName(string map, int timeoutMs)
    {
        long end = Environment.TickCount64 + timeoutMs;
        while (!Bot.ShouldExit && Bot.Map.Name != map && Environment.TickCount64 < end)
            Bot.Sleep(250);
    }

    void KillFocusedMonster(string map, string monster, string cell, string pad)
    {
        long noTargetSince = 0;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                if (Bot.Player.Cell != cell)
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                }
                Bot.Player.SetSpawnPoint();
                Bot.Sleep(500);
                continue;
            }

            if (Bot.Player.Cell != cell)
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            AttackPriorityTarget(map, cell, monster);
            UseClassSkills();
            Bot.Sleep(400);

            if (Bot.Player.HasTarget)
            {
                noTargetSince = 0;
                continue;
            }

            if (!MonsterAvailable(monster, cell))
            {
                noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;
                if (Environment.TickCount64 - noTargetSince > 1800)
                    break;
            }
            else
            {
                noTargetSince = 0;
                Bot.Sleep(300);
            }
        }
    }

    bool TargetBossDefeated(string monster)
    {
        try
        {
            return Bot.Player.HasTarget &&
                Bot.Player.Target != null &&
                string.Equals(Bot.Player.Target.Name, monster, StringComparison.OrdinalIgnoreCase) &&
                Bot.Player.Target.HP <= 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Fights a monster with optional alternating Scroll of Enrage taunting.
    /// Listens via Bot.Flash.FlashCall (same event as UltraDrakath),
    /// fires Core.UsePotion() and retries until the boss has Focus/Reckless aura.
    /// Both taunters (LR and LoO) participate and alternate each convergence.
    /// startingTaunterConfig: the player config key whose account goes SECOND
    /// (i.e. starts with usedLastEnrage=true so the other taunter fires first).
    /// </summary>
    void BossFight(string monster, string cell, string enrageMessage, bool isTaunter, int tauntOffsetSeconds = 0, string startingTaunterConfig = "player2")
    {
        bool needsEnrage = false;
        bool usedEnrage = false;
        // The "starting" player defers first — so the other taunter fires on convergence 1,
        // then they swap each time, matching the PR's alternating pattern.
        bool usedLastEnrage = isTaunter &&
            Bot.Player.Username.Equals(
                Bot.Config!.Get<string>(startingTaunterConfig) ?? "",
                StringComparison.OrdinalIgnoreCase);
        DateTimeOffset tauntTime = DateTimeOffset.MinValue;
        long noTargetSince = 0;
        bool fightSpawnSet = false;

        Bot.Flash.FlashCall += Listener;
        try
        {
            while (!Bot.ShouldExit)
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    ReturnToFightCell(cell);
                    Bot.Sleep(500);
                    continue;
                }

                ReturnToFightCell(cell);

                if (TargetBossDefeated(monster))
                    break;

                if (isTaunter && needsEnrage && !usedEnrage)
                {
                    if (!usedLastEnrage && Bot.Player.HasTarget &&
                        (tauntOffsetSeconds <= 0 || DateTimeOffset.Now > tauntTime))
                    {
                        // My turn — apply the scroll and verify it landed.
                        Core.Logger($"[Taunt] '{enrageMessage}' — my turn, applying Scroll of Enrage...");

                        if (TauntCurrentTarget(enrageMessage, maxAttempts: 10))
                        {
                            usedEnrage = true;
                            needsEnrage = false;
                            usedLastEnrage = true; // other taunter goes next convergence
                        }
                    }
                    else if (usedLastEnrage)
                    {
                        // Other taunter's turn this convergence — stand by long enough to observe Focus/Reckless.
                        Core.Logger($"[Taunt] '{enrageMessage}' — other taunter's turn.");

                        long waitUntil = Environment.TickCount64 + 2500;
                        while (!Bot.ShouldExit &&
                               Bot.Player.HasTarget &&
                               !TargetHasEnrageAura() &&
                               Environment.TickCount64 < waitUntil)
                        {
                            Bot.Sleep(100);
                        }

                        if (TargetHasEnrageAura())
                            Core.Logger("[Taunt] Other taunter's Enrage confirmed.");
                        else
                            Core.Logger("[Taunt] Other taunter turn complete; rotating.");

                        usedEnrage = true;
                        needsEnrage = false;
                        usedLastEnrage = false; // I go next convergence
                    }
                }

                if (!needsEnrage || usedEnrage || !Bot.Player.HasTarget)
                    Bot.Combat.Attack(monster);

                UseClassSkills();
                Bot.Sleep(300);

                if (Bot.Player.HasTarget)
                {
                    noTargetSince = 0;
                }
                else if (!MonsterAvailable(monster, cell))
                {
                    noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;
                    if (Environment.TickCount64 - noTargetSince > 1800)
                        break;
                }
                else
                {
                    noTargetSince = 0;
                }
            }
        }
        finally
        {
            Bot.Flash.FlashCall -= Listener;
        }

        void ReturnToFightCell(string targetCell)
        {
            if (string.IsNullOrWhiteSpace(targetCell))
                return;

            if (string.Equals(Bot.Player.Cell, targetCell, StringComparison.OrdinalIgnoreCase))
            {
                if (!fightSpawnSet)
                {
                    Bot.Player.SetSpawnPoint();
                    fightSpawnSet = true;
                }
                return;
            }

            Core.Logger($"[Recover] Returning to {targetCell} after respawn/desync.");
            Bot.Map.Jump(targetCell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(targetCell);
            Bot.Player.SetSpawnPoint();
            fightSpawnSet = true;
            Bot.Sleep(500);
        }

        void Listener(string name, object[] args)
        {
            try
            {
                dynamic? data = null;
                if (name == "pext")
                {
                    var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
                    if (packet?["params"]?["type"]?.ToString() == "json")
                        data = packet["params"]["dataObj"];
                }
                else if (name == "packetFromServer")
                {
                    var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
                    data = packet?["b"]?["o"];
                }

                if (data == null || data!["cmd"]?.ToString() != "ct") return;

                bool triggered = false;

                if (data!["anims"] != null)
                    foreach (var a in data!.anims)
                        if (a?.msg != null && ((string)a!.msg).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0)
                        { triggered = true; break; }

                if (!triggered && data!["a"] != null)
                    foreach (var a in data.a)
                        if (a != null && a!["cmd"]?.ToString() == "aura+" && a!["auras"] != null)
                            foreach (var aura in a!["auras"])
                                if (aura?.msgOn != null &&
                                    ((string)aura!.msgOn).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                    (bool)aura!.isNew)
                                { triggered = true; break; }

                if (!triggered) return;

                needsEnrage = true;
                usedEnrage = false;
                if (tauntOffsetSeconds > 0)
                    tauntTime = DateTimeOffset.Now.AddSeconds(tauntOffsetSeconds);

                Core.Logger($"[Taunt] '{enrageMessage}' — {(tauntOffsetSeconds > 0 ? $"enraging in {tauntOffsetSeconds}s" : "enraging now")}.");
            }
            catch { }
        }
    }

    void AttackPriorityTarget(string map, string cell, string fallbackMonster)
    {
        foreach (string target in GetRoomTargetPriority(map, cell, fallbackMonster))
        {
            if (target == "*" || MonsterAvailable(target, cell))
            {
                Bot.Combat.Attack(target);
                return;
            }
        }

        Bot.Combat.Attack(fallbackMonster);
    }

    string[] GetRoomTargetPriority(string map, string cell, string fallbackMonster)
    {
        if (string.Equals(map, "solsticemoon", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(cell, "Enter", StringComparison.OrdinalIgnoreCase))
        {
            return new[] { "Shackled Fairy", "Faithless Deer", fallbackMonster };
        }

        return new[] { fallbackMonster };
    }

    bool MonsterAvailable(string monster, string cell)
    {
        try
        {
            return Bot.Monsters.MapMonsters.Any(m =>
                m != null
                && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
                && (monster == "*" || string.Equals(m.Name, monster, StringComparison.OrdinalIgnoreCase))
                && m.Alive
            );
        }
        catch
        {
            return true;
        }
    }

    void UseClassSkills()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";

        switch (className.ToLower())
        {
            case "legion revenant":
                UseFirstAvailableSkill(lrSkillList);
                break;

            case "stonecrusher":
            case "infinity titan":
                UseFirstAvailableSkill(scSkillList);
                break;

            case "archpaladin":
                UseFirstAvailableSkill(apSkillList);
                break;

            case "lord of order":
                UseFirstAvailableSkill(looSkillList);
                break;
        }
    }

    void UseFirstAvailableSkill(int[] skillPriority)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
            return;

        foreach (int skill in skillPriority)
        {
            if (Bot.Skills.CanUseSkill(skill))
            {
                Bot.Skills.UseSkill(skill);
                return;
            }
        }
    }

    // Sun side: player1 (LR) + player2 (SC) alternate — midnightsun dungeon
    bool IsSunTaunter() =>
        IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "") ||
        IsConfiguredAccount(Bot.Config!.Get<string>("player2") ?? "");

    // Moon side: player3 (AP) + player4 (LoO) alternate — solsticemoon dungeon
    bool IsMoonTaunter() =>
        IsConfiguredAccount(Bot.Config!.Get<string>("player3") ?? "") ||
        IsConfiguredAccount(Bot.Config!.Get<string>("player4") ?? "");

    bool IsConfiguredAccount(string account) =>
        !string.IsNullOrWhiteSpace(account)
        && string.Equals(Core.Username(), account, StringComparison.OrdinalIgnoreCase);

    void EquipClassByName(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return;

        string classToEquip = className;

        if (!Core.CheckInventory(classToEquip))
        {
            if (className.Equals("StoneCrusher", StringComparison.OrdinalIgnoreCase) &&
                Core.CheckInventory("Infinity Titan"))
            {
                Core.Logger("This account is assigned to use StoneCrusher, but StoneCrusher is missing. Using Infinity Titan instead.");
                classToEquip = "Infinity Titan";
            }
            else
            {
                Core.Logger($"This account is assigned to use {className}, but that class is not in inventory.", stopBot: true);
                return;
            }
        }

        if (!IsClassEquipped(classToEquip))
        {
            Core.Equip(classToEquip);
            Bot.Wait.ForItemEquip(classToEquip);
            Bot.Sleep(1000);
        }

        Core.Logger($"Equipped {classToEquip}; using custom skill rotation.");
    }

    bool IsClassEquipped(string className) =>
        !string.IsNullOrWhiteSpace(className)
        && string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase);

    void EquipArmyClasses()
    {
        string username = Core.Username();
        string? p1 = Bot.Config!.Get<string>("player1");
        string? p2 = Bot.Config!.Get<string>("player2");
        string? p3 = Bot.Config!.Get<string>("player3");
        string? p4 = Bot.Config!.Get<string>("player4");

        if (username.Equals(p1, StringComparison.OrdinalIgnoreCase))
            EquipClassByName(player1Class);
        else if (username.Equals(p2, StringComparison.OrdinalIgnoreCase))
            EquipClassByName(player2Class);
        else if (username.Equals(p3, StringComparison.OrdinalIgnoreCase))
            EquipClassByName(player3Class);
        else if (username.Equals(p4, StringComparison.OrdinalIgnoreCase))
            EquipClassByName(player4Class);
        else
            Core.Logger("This account was not matched to a player slot — keeping current class.");
    }

    void ApplyEnhancements()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";
        Core.Logger($"Applying enhancements for {className}...");

        switch (className.ToLower())
        {
            case "legion revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.None,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "stonecrusher":
            case "infinity titan":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            default:
                Core.Logger($"[Enhance] No profile for '{className}' — skipping.");
                break;
        }
    }


    bool TargetHasEnrageAura()
    {
        return Bot.Player.HasTarget &&
            (Bot.Target.Auras.Any(x =>
                x.Name.Equals("Focus", StringComparison.OrdinalIgnoreCase) &&
                x.RemainingTime > 4) ||
             Bot.Target.Auras.Any(x =>
                x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase) &&
                x.RemainingTime > 4));
    }

    bool EnsureEnrageEquipped(string context)
    {
        if (!Core.CheckInventory("Scroll of Enrage"))
        {
            Core.Logger($"[Taunt] {context}: missing Scroll of Enrage.");
            return false;
        }

        if (!Bot.Inventory.IsEquipped("Scroll of Enrage"))
        {
            Core.Logger($"[Taunt] {context}: Scroll of Enrage was not equipped; re-equipping usable slot.");
            Bot.Inventory.EquipUsableItem("Scroll of Enrage");
            Bot.Sleep(600);
        }

        return Bot.Inventory.IsEquipped("Scroll of Enrage");
    }

    void CheckUsableSlotBeforeBossRoom(string context)
    {
        if (!checkUsableSlotBeforeBossRooms)
            return;

        if (!EnsureEnrageEquipped(context))
            return;

        if (!Bot.Skills.CanUseSkill(5))
        {
            Core.Logger($"[Taunt] {context}: usable slot is not ready yet; waiting briefly before boss room.");
            Bot.Wait.ForTrue(() => Bot.Skills.CanUseSkill(5), 10);
        }
    }

    bool TauntCurrentTarget(string logPrefix, string expectedTarget, int maxAttempts = 10)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
        {
            Bot.Sleep(300);
            return false;
        }

        Core.Logger($"{logPrefix} Targeting {expectedTarget} for Scroll of Enrage.");
        Core.Logger($"{logPrefix} Force-using Scroll of Enrage on {expectedTarget}; skipping CanUseSkill(5) gate.");

        Bot.Skills.Pause();

        try
        {
            Bot.Combat.CancelAutoAttack();

            for (int attempt = 1; attempt <= maxAttempts && !Bot.ShouldExit; attempt++)
            {
                if (!Bot.Player.HasTarget)
                    return false;

                EnsureEnrageEquipped(expectedTarget);

                Core.Logger($"{logPrefix} Scroll attempt {attempt}/{maxAttempts}.");

                Core.UsePotion();

                long confirmEnd = Environment.TickCount64 + 1000;

                while (!Bot.ShouldExit && Environment.TickCount64 < confirmEnd)
                {
                    Bot.Sleep(100);

                    if (TargetHasEnrageAura())
                    {
                        Core.Logger($"{logPrefix} Enrage confirmed on {expectedTarget}.");
                        return true;
                    }
                }

                Bot.Sleep(500);
            }

            Core.Logger($"{logPrefix} Scroll force-use attempts failed; Enrage was not confirmed on {expectedTarget}.");
            return false;
        }
        finally
        {
            Bot.Skills.Resume();
        }
    }

    bool TauntCurrentTarget(string enrageMessage, int maxAttempts = 10)
    {
        Bot.Skills.Pause();

        try
        {
            for (int attempt = 1; attempt <= maxAttempts && !Bot.ShouldExit; attempt++)
            {
                if (!Bot.Player.HasTarget)
                    return false;

                if (TargetHasEnrageAura())
                    return true;

                EnsureEnrageEquipped(enrageMessage);

                if (!Bot.Skills.CanUseSkill(5))
                    Bot.Wait.ForTrue(() => Bot.Skills.CanUseSkill(5), 3);

                Core.Logger($"[Taunt] '{enrageMessage}' — Scroll of Enrage attempt {attempt}/{maxAttempts}.");

                Bot.Combat.CancelAutoAttack();
                Core.UsePotion();
                Bot.Sleep(350);

                if (TargetHasEnrageAura())
                {
                    Core.Logger("[Taunt] Enrage confirmed!");
                    return true;
                }
            }

            Core.Logger($"[Taunt] WARNING: '{enrageMessage}' was not confirmed after {maxAttempts} attempts.");
            return false;
        }
        finally
        {
            Bot.Skills.Resume();
        }
    }

    void PrepareTauntRole()
    {
        // All 4 players equip Scroll of Enrage:
        // player1+2 alternate on sun fights, player3+4 alternate on moon fights.
        if (autoGetEnrage)
        {
            Core.Logger("Auto-crafting Scroll of Enrage...");
            Ultra.GetScrollOfEnrage();
        }

        if (!EnsureEnrageEquipped("setup"))
        {
            Core.Logger("No Scroll of Enrage found — boss charges will NOT be redirected.");
            return;
        }

        Core.Logger("Scroll of Enrage is equipped.");
    }


    // ── Dungeon clears ────────────────────────────────────────────────────────

    void RunMidnightSun()
    {
        ResetReusableSyncFiles();
        int run = ++midnightRunCount;
        ResetDungeonInstance("midnightsun", $"GreatbladeMidnight_{run}");
        ArmyKillMonster("midnightsun", "Enter", "Left", "Shining Star", $"GreatbladeMidnight_{run}_01");
        ArmyKillMonster("midnightsun", "Enter", "Left", "Dying Light", $"GreatbladeMidnight_{run}_02");
        ArmyKillMonster("midnightsun", "r1", "Left", "Shining Star", $"GreatbladeMidnight_{run}_03");
        // Dawn Knight at r1: player1+2 alternate on "The Light Gathers" (6 s offset)
        ArmyKillWithDelayedTaunt("midnightsun", "r1", "Left", "Dawn Knight", $"GreatbladeMidnight_{run}_04", sunSide: true, enrageMessage: "The Light Gathers");
        ArmyKillMonster("midnightsun", "r2", "Left", "Dying Light", $"GreatbladeMidnight_{run}_05");
        // Dawn Knight at r2: player1+2 alternate on "The Light Gathers" (6 s offset)
        ArmyKillWithDelayedTaunt("midnightsun", "r2", "Left", "Dawn Knight", $"GreatbladeMidnight_{run}_06", sunSide: true, enrageMessage: "The Light Gathers");
        // Shrine boss: Hollow Solstice — LR taunts on "The Sun Converges" (no offset)
        ArmyKillShrineBoss("midnightsun", "r3", "Left", "Hollow Solstice", $"GreatbladeMidnight_{run}_07", moonBoss: false);
    }

    void RunSolsticeMoon()
    {
        int run = ++solsticeRunCount;
        string runCheckpoint = $"GreatbladeSolstice_{run}";

        ResetReusableSyncFiles();

        ResetDungeonInstance("solsticemoon", runCheckpoint);

        ArmyKillMonster("solsticemoon", "Enter", "Left", "*", $"GreatbladeSolstice_{run}_01");

        // r1: everyone focuses Lunar Haze first, only LR taunts Moonlight Gaze.
        // After Lunar Haze dies, everyone swaps to Faithless Deer before moving on.
        ArmyKillLunarHazeWithLrTaunt(
            "solsticemoon",
            "r1",
            "Left",
            "Faithless Deer",
            $"GreatbladeSolstice_{run}_02"
        );

        // r2: everyone focuses Lunar Haze first, only LR taunts Moonlight Gaze.
        // After Lunar Haze dies, everyone swaps to Shackled Fairy before moving on.
        ArmyKillLunarHazeWithLrTaunt(
            "solsticemoon",
            "r2",
            "Left",
            "Shackled Fairy",
            $"GreatbladeSolstice_{run}_03"
        );

        // Shrine boss: Hollow Midnight — player3+4 alternate on "The Moon Converges".
        ArmyKillShrineBoss(
            "solsticemoon",
            "r3",
            "Left",
            "Hollow Midnight",
            $"GreatbladeSolstice_{run}_04",
            moonBoss: true
        );

        TryCompleteDailyQuest(9303, "Sliver of Moonlight");
    }

    void MoveSolstice(string checkpoint)
    {
        switch (Bot.Player.Cell)
        {
            case "Enter":
                Bot.Map.Jump("r1", "Left", autoCorrect: false);
                break;

            case "r1":
                Bot.Map.Jump("r2", "Left", autoCorrect: false);
                break;

            case "r2":
                Bot.Map.Jump("r3", "Left", autoCorrect: false);
                break;

            case "r3":
            case "r3a":
                RestartSolstice(checkpoint);
                break;
        }
    }

    void UseClassPotions()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";

        Core.Logger($"Using tonic/elixir for {className}...");

        switch (className.ToLower())
        {
            case "legion revenant":
                UsePotionSet("Sage Tonic", "Potent Malevolence Elixir");
                break;

            case "stonecrusher":
            case "infinity titan":
                UsePotionSet("Might Tonic", "Potent Battle Elixir");
                break;

            case "archpaladin":
                UsePotionSet("Fate Tonic", "Potent Battle Elixir");
                break;

            case "lord of order":
                UsePotionSet("Fate Tonic", "Potent Destruction Elixir");
                break;

            default:
                Core.Logger($"[Potions] No tonic/elixir profile for '{className}' — skipping.");
                break;
        }
    }

    void UsePotionSet(string tonic, string elixir)
    {
        UsePotionItem(tonic);
        UsePotionItem(elixir);
    }

    void UsePotionItem(string itemName)
    {
        if (!Core.CheckInventory(itemName))
            BuyAlchemyPotion(itemName);

        if (!Core.CheckInventory(itemName))
        {
            Core.Logger($"[Potions] Failed to get {itemName}; skipping.");
            return;
        }

        if (!Bot.Inventory.IsEquipped(itemName))
        {
            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Sleep(500);
        }

        Core.UsePotion();
        Bot.Sleep(1000);
    }
    void RefreshPotionsIfAurasMissing()
    {
        if (!usePotions)
            return;

        bool hasTonic =
            Bot.Self.HasActiveAura("Sage") ||
            Bot.Self.HasActiveAura("Might") ||
            Bot.Self.HasActiveAura("Fate");

        bool hasElixir =
            Bot.Self.HasActiveAura("Potent Malevolence Elixir") ||
            Bot.Self.HasActiveAura("Potent Battle Elixir") ||
            Bot.Self.HasActiveAura("Potent Destruction Elixir");

        if (hasTonic && hasElixir)
        {
            Core.Logger("[Potions] Potion auras still active. Skipping potion refresh.");
            return;
        }

        Core.Logger("[Potions] Missing potion aura. Refreshing class potions.");
        UseClassPotions();
    }

    void BuyAlchemyPotion(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName) || Core.CheckInventory(itemName))
        {
            if (!string.IsNullOrWhiteSpace(itemName))
                Core.Logger($"[Potions] Have: {itemName}");
            return;
        }

        const int shopId = 2036;
        const string map = "alchemyacademy";
        const string voucher = "Gold Voucher 500k";

        void NeedVoucher(int wanted)
        {
            int missing = Math.Max(0, wanted - Bot.Inventory.GetQuantity(voucher));

            if (missing > 0)
            {
                Core.Logger($"[Potions] Buying {missing}x {voucher}.");
                Engine.BuyItem(voucher, shopId, map, missing);
                Bot.Sleep(500);
            }
        }

        void BuyPotion(int count)
        {
            Core.Logger($"[Potions] Buying {count}x {itemName}.");
            Engine.BuyItem(itemName, shopId, map, count, calculateRemaining: false);
            Bot.Sleep(500);
        }

        switch (itemName)
        {
            case "Might Tonic":
            case "Sage Tonic":
            case "Fate Tonic":
                if (!Engine.Faction("Alchemy", 8))
                {
                    Core.Logger("[Potions] Alchemy rank 8 required for tonic.");
                    return;
                }

                NeedVoucher(2);
                BuyPotion(10);
                break;

            case "Potent Malevolence Elixir":
            case "Potent Battle Elixir":
            case "Potent Destruction Elixir":
                NeedVoucher(4);
                BuyPotion(8);
                break;

            default:
                Core.Logger($"[Potions] Unknown potion: {itemName}");
                return;
        }
    }

    void RestartSolstice(string checkpoint)
    {
        SyncArmy($"{checkpoint}_restart_ready.sync");
        string target = Core.PrivateRooms ? $"solsticemoon-{Core.PrivateRoomNumber}" : "solsticemoon";
        Core.Logger($"Restarting /{target} with EclipseAscent packet 24946...");
        Bot.Send.Packet($"%xt%zm%dungeonQueue%24946%{target}%");
        Bot.Wait.ForMapLoad("solsticemoon");
        Bot.Wait.ForTrue(() => string.Equals(Bot.Player.Cell, "Enter", StringComparison.OrdinalIgnoreCase), 20);
        SyncArmy($"{checkpoint}_restart_done.sync");
    }
}

//lonewolf was here ( ͡° ͜ʖ ͡°)