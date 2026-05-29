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
        new Option<string>("player1", "Legion Revenant", "AQW account name for the Legion Revenant slot.", ""),
        new Option<string>("player2", "StoneCrusher", "AQW account name for the StoneCrusher slot.", ""),
        new Option<string>("player3", "ArchPaladin", "AQW account name for the ArchPaladin slot.", ""),
        new Option<string>("player4", "Lord Of Order", "AQW account name for the Lord Of Order slot.", ""),
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
        new Option<bool>(
            "autoEnhance",
            "Auto-Apply Enhancements",
            "ON: each account automatically applies the correct enhancements for its fixed class.\n" +
            "Slot 1 (LR): Wizard/Pneuma/Ravenous/Vainglory  Slot 2 (SC): Fighter/Anima/Ravenous/Absolution\n" +
            "Slot 3 (AP): Lucky/Forge/Ravenous/Penitence     Slot 4 (LoO): Lucky/Forge/ArcanasConcerto/Penitence",
            true
        ),
        new Option<bool>(
            "autoGetEnrage",
            "Auto-Craft Scroll of Enrage",
            "ON: all accounts auto-farm Mystic Parchment, buy Zealous Ink, and craft Scroll of Enrage before farming. Requires SpellCrafting rank 5.\n" +
            "OFF: scroll requirement is skipped.",
            true
        ),
    };

    // ── Item names ────────────────────────────────────────────────────────────
    const string SliverSunlight    = "Sliver of Sunlight";
    const string SliverMoonlight   = "Sliver of Moonlight";
    const string Solarbrand        = "Solarbrand";
    const string Lunarbrand        = "Lunarbrand";
    const string BladeBurningSun   = "Blade of the Burning Sun";
    const string BladeGlowingMoon  = "Blade of the Glowing Moon";
    const string GreatMidnightSun  = "Greatblade of the Midnight Sun";
    const string GreatSolsticeMoon = "Greatblade of the Solstice Moon";
    // EO phase items — uncomment when ascendeclipse is ready
    // const string EclipticOffering  = "Ecliptic Offering"; // confirm: "Hallowed Remains"?
    // const string Umbrabrand        = "Umbrabrand";
    // const string BladeBoundEclipse = "Blade of the Bound Eclipse";
    // const string GreatEntwinedEcl  = "Greatblade of the Entwined Eclipse";

    // ── Shop ─────────────────────────────────────────────────────────────────
    const string MergeMap  = "templeshrine";
    const int    MergeShop = 2303;

    // Shop item IDs (prevents name-lookup ambiguity)
    const int ID_Solarbrand        = 78465;
    const int ID_Lunarbrand        = 78460;
    const int ID_BladeBurningSun   = 78466;
    const int ID_BladeGlowingMoon  = 78461;
    const int ID_GreatMidnightSun  = 78467;
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
    const int SunlightNeeded  = 215;
    const int MoonlightNeeded = 215;

    bool autoEnhance;
    bool autoGetEnrage;

    // Fixed 4-class lineup matching the working reference script
    const string player1Class = "Legion Revenant";  // sun taunter  (player1+2 alternate)
    const string player2Class = "StoneCrusher";      // sun taunter  (player1+2 alternate)
    const string player3Class = "ArchPaladin";       // moon taunter (player3+4 alternate)
    const string player4Class = "Lord Of Order";     // moon taunter (player3+4 alternate)

    readonly int[] lrSkillList = new[] { 3, 4, 2, 1 };
    readonly int[] scSkillList = new[] { 3, 2, 4, 1 };
    readonly int[] apSkillList = new[] { 2, 3, 1, 4 };
    readonly int[] looSkillList = new[] { 2, 3, 1, 4 };

    int midnightRunCount;
    int solsticeRunCount;
    int syncCount;

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
        if (Core.PrivateRoomNumber < 1000 || Core.PrivateRoomNumber > 99999)
            Core.PrivateRoomNumber = sArmy.getRoomNr();
        Core.Logger($"Army mode enabled: {sArmy.Players().Length} accounts, private room #{Core.PrivateRoomNumber}.");

        autoEnhance = Bot.Config.Get<bool>("autoEnhance");
        autoGetEnrage = Bot.Config.Get<bool>("autoGetEnrage");

        EquipArmyClasses();

        if (autoEnhance)
            ApplyEnhancements();

        PrepareTauntRole();

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
        FarmSunlightTo(5);   MergeToHave(Solarbrand,       ID_Solarbrand,       1);
        FarmMoonlightTo(5);  MergeToHave(Lunarbrand,       ID_Lunarbrand,       1);
        FarmSunlightTo(50);  MergeToHave(BladeBurningSun,  ID_BladeBurningSun,  1);
        FarmMoonlightTo(50); MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 1);

        // Iteration 2 — second set
        FarmSunlightTo(5);   MergeToHave(Solarbrand,       ID_Solarbrand,       2);
        FarmMoonlightTo(5);  MergeToHave(Lunarbrand,       ID_Lunarbrand,       2);
        FarmSunlightTo(50);  MergeToHave(BladeBurningSun,  ID_BladeBurningSun,  2);
        FarmMoonlightTo(50); MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 2);

        // Third Solar + Lunar (held for Umbrabrand in the EO phase)
        FarmSunlightTo(5);  MergeToHave(Solarbrand,  ID_Solarbrand,  3);
        FarmMoonlightTo(5); MergeToHave(Lunarbrand,   ID_Lunarbrand,  3);

        // Greatblades — one per side
        FarmSunlightTo(100);  MergeToHave(GreatMidnightSun,  ID_GreatMidnightSun,  1);
        FarmMoonlightTo(100); MergeToHave(GreatSolsticeMoon, ID_GreatSolsticeMoon, 1);

        Core.Logger("Basic blade chain complete! Remaining: 1× Solarbrand, 1× Lunarbrand, 1× BBS, 1× BGM — held for the Ecliptic Offering phase.");
    }

    // ── Merge helpers ─────────────────────────────────────────────────────────

    void MergeBasicChain()
    {
        Core.Logger("Starting basic blade merge chain...");
        // 3× Solarbrand — 2 consumed by BBS, 1 held for Umbrabrand
        MergeToHave(Solarbrand,       ID_Solarbrand,       3);
        // 3× Lunarbrand — 2 consumed by BGM, 1 held for Umbrabrand
        MergeToHave(Lunarbrand,       ID_Lunarbrand,       3);
        // 2× BBS — 1 consumed by GreatMidnightSun, 1 held for BoBE
        MergeToHave(BladeBurningSun,  ID_BladeBurningSun,  2);
        // 2× BGM — 1 consumed by GreatSolsticeMoon, 1 held for BoBE
        MergeToHave(BladeGlowingMoon, ID_BladeGlowingMoon, 2);
        MergeToHave(GreatMidnightSun,  ID_GreatMidnightSun,  1);
        MergeToHave(GreatSolsticeMoon, ID_GreatSolsticeMoon, 1);
        Core.Logger("Basic blade chain complete! Remaining: 1× Solarbrand, 1× Lunarbrand, 1× BBS, 1× BGM — held for the Ecliptic Offering phase.");
    }

    /// <summary>
    /// Merges until we own at least <paramref name="targetTotal"/> of the item.
    /// Accounts for what's already in inventory — won't over-merge.
    /// </summary>
    void MergeToHave(string itemName, int shopItemID, int targetTotal)
    {
        int have  = Bot.Inventory.GetQuantity(itemName);
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

    void SyncArmy(string syncFile)
    {
        int partySize = sArmy.Players().Length;
        if (partySize <= 1)
            return;

        string path = Ultra.ResolveSyncPath($"Greatblade_{Core.PrivateRoomNumber}_{++syncCount}_{syncFile}");
        string username = Core.Username().ToLower();

        while (!Bot.ShouldExit)
        {
            string[] lines = ReadSyncLines(path);
            lines = lines.Where(line => !line.StartsWith($"{username}:", StringComparison.OrdinalIgnoreCase)).ToArray();
            WriteSyncLines(path, lines.Append($"{username}:ready:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}").ToArray());

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int ready = ReadSyncLines(path)
                .Select(line => line.Split(':'))
                .Where(parts => parts.Length >= 3 && long.TryParse(parts[2], out long ts) && now - ts <= 120)
                .Select(parts => parts[0])
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
        string path = Ultra.ResolveSyncPath($"Greatblade_{Core.PrivateRoomNumber}_{syncFile}");
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
        KillFocusedMonster(monster, cell, pad);
        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillWithDelayedTaunt(string map, string cell, string pad, string monster, string checkpoint, bool sunSide, string enrageMessage, int tauntOffsetSeconds = 6)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        // Sun side (midnightsun): player1+2 alternate — player1 "starts" so player2 fires first
        // Moon side (solsticemoon): player3+4 alternate — player3 "starts" so player4 fires first
        bool isTaunter = sunSide ? IsSunTaunter() : IsMoonTaunter();
        string startingTaunterConfig = sunSide ? "player1" : "player3";

        BossFight(monster, cell, enrageMessage, isTaunter, tauntOffsetSeconds, startingTaunterConfig);
        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillShrineBoss(string map, string cell, string pad, string monster, string checkpoint, bool moonBoss)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        // Sun boss: player1+2 alternate — player1 fires first, player2 is "starting" (defers first)
        // Moon boss: player3+4 alternate — player3 fires first, player4 is "starting" (defers first)
        bool isTaunter = moonBoss ? IsMoonTaunter() : IsSunTaunter();
        string enrageMessage         = moonBoss ? "The Moon Converges" : "The Sun Converges";
        string startingTaunterConfig = moonBoss ? "player4" : "player2";

        BossFight(monster, cell, enrageMessage, isTaunter, 0, startingTaunterConfig);

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

        JoinShrineDungeon(nextMap, "Enter", "Left", force: true);
        Bot.Sleep(1000);

        SyncArmy($"{checkpoint}_all_rejoined_dungeon.sync");
    }

    void JoinAndFocus(string map, string cell, string pad)
    {
        if (map == "midnightsun" || map == "solsticemoon")
            JoinShrineDungeon(map, cell, pad);
        else
            Core.Join(map, cell, pad);

        if ((map == "midnightsun" || map == "solsticemoon") && Bot.Map.PlayerNames.Count() < sArmy.Players().Length)
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

    void KillFocusedMonster(string monster, string cell, string pad)
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

            Bot.Combat.Attack(monster);
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
        bool needsEnrage    = false;
        bool usedEnrage     = false;
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

                if (isTaunter && needsEnrage && !usedEnrage)
                {
                    if (!usedLastEnrage && Bot.Player.HasTarget &&
                        (tauntOffsetSeconds <= 0 || DateTimeOffset.Now > tauntTime))
                    {
                        // My turn — apply the scroll and verify it landed.
                        Core.Logger($"[Taunt] '{enrageMessage}' — my turn, applying Scroll of Enrage...");
                        Bot.Skills.Pause();
                        try
                        {
                            while (!Bot.ShouldExit && Bot.Player.HasTarget && needsEnrage && !usedEnrage)
                            {
                                Bot.Combat.CancelAutoAttack();
                                Core.UsePotion();
                                Bot.Sleep(200);

                                if (Bot.Player.HasTarget &&
                                    (Bot.Target.Auras.Any(x => x.Name.Equals("Focus",    StringComparison.OrdinalIgnoreCase) && x.RemainingTime > 4) ||
                                     Bot.Target.Auras.Any(x => x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase) && x.RemainingTime > 4)))
                                {
                                    usedEnrage  = true;
                                    needsEnrage = false;
                                    usedLastEnrage = true; // other taunter goes next convergence
                                    Core.Logger("[Taunt] Enrage confirmed!");
                                }
                                else
                                {
                                    Bot.Sleep(200);
                                }
                            }
                        }
                        finally
                        {
                            Bot.Skills.Resume();
                        }
                    }
                    else if (usedLastEnrage)
                    {
                        // Other taunter's turn this convergence — stand by.
                        Core.Logger($"[Taunt] '{enrageMessage}' — other taunter's turn.");
                        usedEnrage     = true;
                        needsEnrage    = false;
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

                if (data == null || data["cmd"]?.ToString() != "ct") return;

                bool triggered = false;

                if (data["anims"] != null)
                    foreach (var a in data.anims)
                        if (a?.msg != null && ((string)a.msg).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0)
                        { triggered = true; break; }

                if (!triggered && data["a"] != null)
                    foreach (var a in data.a)
                        if (a != null && a["cmd"]?.ToString() == "aura+" && a["auras"] != null)
                            foreach (var aura in a["auras"])
                                if (aura?.msgOn != null &&
                                    ((string)aura.msgOn).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                    (bool)aura.isNew)
                                { triggered = true; break; }

                if (!triggered) return;

                needsEnrage = true;
                usedEnrage  = false;
                if (tauntOffsetSeconds > 0)
                    tauntTime = DateTimeOffset.Now.AddSeconds(tauntOffsetSeconds);

                Core.Logger($"[Taunt] '{enrageMessage}' — {(tauntOffsetSeconds > 0 ? $"enraging in {tauntOffsetSeconds}s" : "enraging now")}.");
            }
            catch { }
        }
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

        if (!Core.CheckInventory(className))
        {
            Core.Logger($"{Core.Username()} is assigned to use {className}, but that class is not in inventory.", stopBot: true);
            return;
        }

        if (!IsClassEquipped(className))
        {
            Core.Equip(className);
            Bot.Wait.ForItemEquip(className);
            Bot.Sleep(1000);
        }

        Core.Logger($"{Core.Username()} equipped {className}; using custom skill rotation.");
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
            Core.Logger($"{username} was not matched to a player slot — keeping current class.");
    }

    void ApplyEnhancements()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";
        Core.Logger($"{Core.Username()} applying enhancements for {className}...");

        switch (className.ToLower())
        {
            case "legion revenant":
                Adv.EnhanceEquipped(
                    type:     EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "stonecrusher":
                Adv.EnhanceEquipped(
                    type:     EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type:     EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "lord of order":
                Adv.EnhanceEquipped(
                    type:     EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            default:
                Core.Logger($"[Enhance] No enhancement profile for '{className}' — skipping.");
                break;
        }
    }

    void PrepareTauntRole()
    {
        // All 4 players equip Scroll of Enrage:
        // player1+2 alternate on sun fights, player3+4 alternate on moon fights.
        if (autoGetEnrage)
        {
            Core.Logger($"{Core.Username()} auto-crafting Scroll of Enrage...");
            Ultra.GetScrollOfEnrage();
        }

        if (!Core.CheckInventory("Scroll of Enrage"))
        {
            Core.Logger($"{Core.Username()} has no Scroll of Enrage — boss charges will NOT be redirected. " +
                        $"Enable 'Auto-Craft Scroll of Enrage' or craft one manually (SpellCrafting rank 5 required).");
            return;
        }

        if (!Bot.Inventory.IsEquipped("Scroll of Enrage"))
            Bot.Inventory.EquipUsableItem("Scroll of Enrage");

        Core.Logger($"{Core.Username()} has Scroll of Enrage equipped.");
    }


    // ── Dungeon clears ────────────────────────────────────────────────────────

    void RunMidnightSun()
    {
        int run = ++midnightRunCount;
        ResetDungeonInstance("midnightsun", $"GreatbladeMidnight_{run}");
        ArmyKillMonster("midnightsun", "Enter", "Left", "Shining Star",  $"GreatbladeMidnight_{run}_01");
        ArmyKillMonster("midnightsun", "Enter", "Left", "Dying Light",   $"GreatbladeMidnight_{run}_02");
        ArmyKillMonster("midnightsun", "r1",    "Left", "Shining Star",  $"GreatbladeMidnight_{run}_03");
        // Dawn Knight: taunt immediately on "The Light Gathers"; the old 6s delay was too late and caused wipes.
        ArmyKillWithDelayedTaunt("midnightsun", "r1", "Left", "Dawn Knight", $"GreatbladeMidnight_{run}_04", sunSide: true, enrageMessage: "The Light Gathers", tauntOffsetSeconds: 0);
        ArmyKillMonster("midnightsun", "r2",    "Left", "Dying Light",   $"GreatbladeMidnight_{run}_05");
        // Dawn Knight: taunt immediately on "The Light Gathers"; the old 6s delay was too late and caused wipes.
        ArmyKillWithDelayedTaunt("midnightsun", "r2", "Left", "Dawn Knight", $"GreatbladeMidnight_{run}_06", sunSide: true, enrageMessage: "The Light Gathers", tauntOffsetSeconds: 0);
        // Shrine boss: Hollow Solstice — LR taunts on "The Sun Converges" (no offset)
        ArmyKillShrineBoss("midnightsun", "r3", "Left", "Hollow Solstice",  $"GreatbladeMidnight_{run}_07", moonBoss: false);
    }

    void RunSolsticeMoon()
    {
        int run = ++solsticeRunCount;
        ResetDungeonInstance("solsticemoon", $"GreatbladeSolstice_{run}");
        ArmyKillMonster("solsticemoon", "Enter", "Left", "Faithless Deer",  $"GreatbladeSolstice_{run}_01");
        ArmyKillMonster("solsticemoon", "Enter", "Left", "Shackled Fairy", $"GreatbladeSolstice_{run}_02");
        ArmyKillMonster("solsticemoon", "r1",    "Left", "Faithless Deer",  $"GreatbladeSolstice_{run}_03");
        ArmyKillWithDelayedTaunt("solsticemoon", "r1", "Left", "Lunar Haze", $"GreatbladeSolstice_{run}_04", sunSide: false, enrageMessage: "You gaze into the moon");
        ArmyKillMonster("solsticemoon", "r2",    "Left", "Shackled Fairy", $"GreatbladeSolstice_{run}_05");
        ArmyKillWithDelayedTaunt("solsticemoon", "r2", "Left", "Lunar Haze", $"GreatbladeSolstice_{run}_06", sunSide: false, enrageMessage: "You gaze into the moon");
        ArmyKillShrineBoss("solsticemoon", "r3", "Left", "Hollow Midnight", $"GreatbladeSolstice_{run}_07", moonBoss: true);
    }
}
