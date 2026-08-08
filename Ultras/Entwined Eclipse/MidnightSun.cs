/*
name: Midnight Sun (Sun Side Test)
description: Farms Sliver of Sunlight by running /midnightsun in a loop. 4-account army script with alternating Scroll of Enrage taunt. Slot 1 = Legion Revenant, Slot 2 = StoneCrusher, Slot 3 = ArchPaladin, Slot 4 = Lord of Order.
tags: greatblade, entwined, eclipse, midnight, sun, farm, test, army
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

public class MidnightSunTest
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
    public string OptionsStorage = "MidnightSunTest";
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

    // Fixed 4-class lineup
    const string player1Class = "Legion Revenant"; // Dying Light + Hollow Solstice taunter
    const string player2Class = "StoneCrusher";    // Dawn Knight + Hollow Solstice taunter
    const string player3Class = "ArchPaladin";     // Dying Light taunter
    const string player4Class = "Lord of Order";   // Support

    readonly int[] lrSkillList = new[] { 3, 4, 2, 1 };
    readonly int[] scSkillList = new[] { 3, 2, 4, 1 };
    readonly int[] apSkillList = new[] { 2, 3, 1, 4 };
    readonly int[] looSkillList = new[] { 2, 3, 1, 4 };

    const string SliverSunlight = "Sliver of Sunlight";

    bool autoEnhance;
    bool autoGetEnrage;
    bool usePotions;
    int runCount;
    bool syncFilesClearedOnStartup;

    /// <summary>
    /// Optional orchestration hook: when > 0, the main loop exits once the account holds
    /// at least this many Sliver of Sunlight. Leave 0 for the normal "loop forever" behavior.
    /// </summary>
    public int TargetSliverCount;

    /// <summary>
    /// Orchestration hook: when true, skip Core.SetOptions(true/false). Used when this
    /// script is invoked from a parent orchestrator that runs its own SetOptions.
    /// </summary>
    public bool SkipSetOptions;

    // Route:
    // Enter: Dying Light -> Shining Star
    // r1: Dawn Knight -> Shining Star
    // r2: SC handles Dawn Knight while LR/AP/LoO handle Dying Light
    // r3: Hollow Solstice, LR/SC alternate on The Sun Converges

    public void ScriptMain(IScriptInterface bot)
    {
        if (!SkipSetOptions)
            Core.SetOptions(disableClassSwap: true);
        if (sArmy.Players().Length < 4)
        {
            Core.Logger("Add 4 account names in the script options before starting.");
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

        autoEnhance = Bot.Config!.Get<bool>("autoEnhance");
        autoGetEnrage = true;
        usePotions = Bot.Config.Get<bool>("usePotions");

        EquipArmyClasses();

        if (autoEnhance)
            ApplyEnhancements();

        if (usePotions)
            UseClassPotions();

        PrepareTauntRole();

        TryAcceptDailyQuest(9304, "Dawn Breaks");

        Core.AddDrop(SliverSunlight);

        try
        {
            while (!Bot.ShouldExit)
            {
                if (TargetSliverCount > 0 && Core.CheckInventory(SliverSunlight, TargetSliverCount))
                {
                    Core.Logger($"[MidnightSun] Reached target of {TargetSliverCount} {SliverSunlight}; stopping.");
                    break;
                }
                RunMidnightSun();
            }
        }
        finally
        {
            if (!SkipSetOptions)
                Core.SetOptions(false);
        }
    }

    // ── Dungeon run ───────────────────────────────────────────────────────────

    void RunMidnightSun()
    {
        int run = ++runCount;
        string runCheckpoint = $"MidnightSun_{run}";

        ResetReusableSyncFiles();

        ResetDungeonInstance("midnightsun", runCheckpoint);

        // Enter: Dying Light first, then Shining Star after Dying Light dies.
        ArmyKillMonsterWithRotatingAuraTaunt("midnightsun", "Enter", "Left", "Dying Light", "Gathering Light", $"{runCheckpoint}_01", "Shining Star");

        // r1: Dawn Knight first, then Shining Star after Dawn Knight dies.
        ArmyKillMonsterWithSelfAuraDelayedTaunt("midnightsun", "r1", "Left", "Dawn Knight", "Sun's Warmth", $"{runCheckpoint}_03", "Shining Star");

        // r2: SC focuses/taunts Dawn Knight, everyone else focuses Dying Light.
        // Once either mob dies, all accounts swap to whichever mob is still alive.
        ArmyKillRoom2SplitFocusTaunt("midnightsun", "r2", "Left", $"{runCheckpoint}_05");

        // Shrine boss: Hollow Solstice — player1+2 alternate on "The Sun Converges"
        ArmyKillShrineBoss("midnightsun", "r3", "Left", "Hollow Solstice", $"{runCheckpoint}_06");

        TryCompleteDailyQuest(9304, "Sliver of Sunlight");
    }

    // ── Army helpers ──────────────────────────────────────────────────────────

    void ArmyKillMonster(string map, string cell, string pad, string monster, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");
        KillFocusedMonster(monster, cell, pad);
        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillMonsterWithRotatingAuraTaunt(string map, string cell, string pad, string monster, string auraName, string checkpoint, string? nextMonster = null)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        KillFocusedMonsterWithLrApAuraTaunt(monster, auraName, cell, pad, nextMonster);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillMonsterWithSelfAuraDelayedTaunt(string map, string cell, string pad, string monster, string auraName, string checkpoint, string? nextMonster = null, int tauntDelayMs = 6000)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        KillFocusedMonsterWithSunSelfAuraDelayedTaunt(monster, auraName, cell, pad, nextMonster, tauntDelayMs);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillRoom2SplitFocusTaunt(string map, string cell, string pad, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        KillRoom2SplitFocusTaunt(cell, pad);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillWithDelayedTaunt(string map, string cell, string pad, string monster, string checkpoint, string enrageMessage, int tauntOffsetSeconds = 6)
    {
        JoinAndFocus(map, cell, pad);

        bool isTaunter = IsSunTaunter();
        SyncArmy($"{checkpoint}_ready.sync");

        // player1+2 alternate — player1 "starts" so player2 fires first
        BossFight(monster, cell, enrageMessage, isTaunter, tauntOffsetSeconds, startingTaunterConfig: "player1");

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillShrineBoss(string map, string cell, string pad, string monster, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);

        bool isTaunter = IsSunTaunter();
        SyncArmy($"{checkpoint}_ready.sync");

        // player1+2 alternate — player2 is "starting" so player1 fires first
        BossFight(monster, cell, "The Sun Converges", isTaunter, 0, startingTaunterConfig: "player2");

        Core.Logger($"[r3] {monster} fight complete. Waiting for army done sync.");

        SyncArmy($"{checkpoint}_done.sync");
    }

    string SyncGroupPath(string syncFile)
    {
        string group = GetSyncGroup(syncFile);
        return Ultra.ResolveSyncPath($"MidnightSun_{Core.PrivateRoomNumber}_{group}.sync");
    }

    string GetSyncGroup(string syncFile)
    {
        string name = syncFile.ToLower();

        if (name.Contains("party"))
            return "setup";

        if (name.Contains("reset") || name.Contains("whitemap") || name.Contains("rejoined"))
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
            Core.Logger("[Sync] Player1 is clearing startup sync files for MidnightSun.");

            ResetSyncFile(SyncGroupPath("MidnightSun_reset_ready.sync"));
            ResetSyncFile(SyncGroupPath("MidnightSun_run_ready.sync"));

            syncFilesClearedOnStartup = true;

            Core.Logger("[Sync] Startup sync files cleared.");
            Bot.Sleep(1500);
            return;
        }

        Core.Logger("[Sync] Player1 is resetting reusable reset sync file for the new MidnightSun run.");

        ResetSyncFile(SyncGroupPath("MidnightSun_reset_ready.sync"));

        Core.Logger("[Sync] Reusable reset sync file reset complete.");
        Bot.Sleep(1500);
    }

    void ResetSyncFile(string path)
    {
        try
        {
            string? dir = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

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
                .Where(l => !l.StartsWith($"{checkpoint}:{username}:", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            WriteSyncLines(
                path,
                lines
                    .Append($"{checkpoint}:{username}:ready:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}")
                    .ToArray()
            );

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            int ready = ReadSyncLines(path)
                .Select(l => l.Split(':'))
                .Where(p =>
                    p.Length >= 4
                    && p[0].Equals(checkpoint, StringComparison.OrdinalIgnoreCase)
                    && long.TryParse(p[3], out long ts)
                    && now - ts <= 120)
                .Select(p => p[1])
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
                ? File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
                : Array.Empty<string>();
        }
        catch { Bot.Sleep(100); return Array.Empty<string>(); }
    }

    void WriteSyncLines(string path, string[] lines)
    {
        try
        {
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(path, lines);
        }
        catch { Bot.Sleep(100); }
    }

    // ── Combat helpers ────────────────────────────────────────────────────────

    void KillFocusedMonster(string monster, string cell, string pad)
    {
        long noTargetSince = 0;
        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                if (Bot.Player.Cell != cell) { Bot.Map.Jump(cell, pad, autoCorrect: false); Bot.Wait.ForCellChange(cell); }
                Bot.Player.SetSpawnPoint();
                Bot.Sleep(500);
                continue;
            }
            if (Bot.Player.Cell != cell) { Bot.Map.Jump(cell, pad, autoCorrect: false); Bot.Wait.ForCellChange(cell); }

            Bot.Combat.Attack(monster);
            UseClassSkills();
            Bot.Sleep(400);

            if (Bot.Player.HasTarget) { noTargetSince = 0; continue; }
            if (!MonsterAvailable(monster, cell))
            {
                noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;
                if (Environment.TickCount64 - noTargetSince > 1800) break;
            }
            else { noTargetSince = 0; Bot.Sleep(300); }
        }
    }

    void KillFocusedMonsterWithLrApAuraTaunt(string monster, string auraName, string cell, string pad, string? nextMonster = null)
    {
        long noTargetSince = 0;
        bool auraWasActive = false;
        int auraCycle = 0;

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

            bool mainAlive = MonsterAvailable(monster, cell);
            bool nextAlive = !string.IsNullOrWhiteSpace(nextMonster)
                && MonsterAvailable(nextMonster, cell);

            if (!mainAlive && !nextAlive)
            {
                noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;

                if (Environment.TickCount64 - noTargetSince > 1800)
                    break;

                Bot.Sleep(300);
                continue;
            }

            noTargetSince = 0;

            string target = mainAlive ? monster : nextMonster!;
            Bot.Combat.Attack(target);

            bool fightingMainMonster = target.Equals(monster, StringComparison.OrdinalIgnoreCase);

            bool auraActive = fightingMainMonster
                && Bot.Player.HasTarget
                && Bot.Target != null
                && Bot.Target.Auras.Any(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));

            if (fightingMainMonster && auraActive && !auraWasActive)
            {
                auraWasActive = true;
                auraCycle++;

                bool lrTurn = auraCycle % 2 == 1;
                bool apTurn = auraCycle % 2 == 0;
                bool isLegionRevenant = string.Equals(Bot.Player.CurrentClass?.Name, "Legion Revenant", StringComparison.OrdinalIgnoreCase);
                bool isArchPaladin = string.Equals(Bot.Player.CurrentClass?.Name, "ArchPaladin", StringComparison.OrdinalIgnoreCase);

                if ((lrTurn && isLegionRevenant) || (apTurn && isArchPaladin))
                {
                    string role = isLegionRevenant ? "LR" : "AP";
                    Core.Logger($"[Dying Light Taunt] {auraName} detected on {monster}. Cycle #{auraCycle}; {role}'s turn to taunt.");
                    _ = TauntCurrentTarget("[Dying Light Taunt]", monster);
                }
                else
                {
                    if (isLegionRevenant || isArchPaladin)
                    {
                        string waitingFor = lrTurn ? "LR" : "AP";
                        Core.Logger($"[Dying Light Taunt] {auraName} detected on {monster}. Cycle #{auraCycle}; waiting for {waitingFor}.");
                    }

                    UseClassSkills();
                    Bot.Sleep(400);
                }
            }
            else
            {
                UseClassSkills();
                Bot.Sleep(400);
            }

            if (!auraActive)
                auraWasActive = false;
        }
    }

    void KillFocusedMonsterWithSunSelfAuraDelayedTaunt(string monster, string auraName, string cell, string pad, string? nextMonster = null, int tauntDelayMs = 6000)
    {
        long noTargetSince = 0;
        bool auraWasActive = false;
        int auraCycle = 0;
        bool tauntArmed = false;
        bool tauntAttempted = false;
        long tauntAt = 0;

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

            bool mainAlive = MonsterAvailable(monster, cell);
            bool nextAlive = !string.IsNullOrWhiteSpace(nextMonster)
                && MonsterAvailable(nextMonster, cell);

            if (!mainAlive && !nextAlive)
            {
                noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;

                if (Environment.TickCount64 - noTargetSince > 1800)
                    break;

                Bot.Sleep(300);
                continue;
            }

            noTargetSince = 0;

            string target = mainAlive ? monster : nextMonster!;
            Bot.Combat.Attack(target);

            bool fightingMainMonster = target.Equals(monster, StringComparison.OrdinalIgnoreCase);
            bool auraActive = fightingMainMonster && Bot.Self.HasActiveAura(auraName);

            if (auraActive && !auraWasActive)
            {
                auraWasActive = true;
                auraCycle++;

                // Match the old Dawn Knight ordering: player2 taunts first, then player1, then repeats.
                string assignedConfig = auraCycle % 2 == 1 ? "player2" : "player1";
                string assignedLabel = auraCycle % 2 == 1 ? "SC" : "LR";
                bool isAssignedTaunter = IsConfiguredAccount(Bot.Config!.Get<string>(assignedConfig) ?? "");

                if (isAssignedTaunter)
                {
                    tauntArmed = true;
                    tauntAttempted = false;
                    tauntAt = Environment.TickCount64 + tauntDelayMs;
                    Core.Logger($"[Dawn Knight Taunt] {auraName} detected. Cycle #{auraCycle}; {assignedLabel}'s turn. Taunting in {tauntDelayMs / 1000.0:0.#} seconds.");
                }
                else if (IsSunTaunter())
                {
                    Core.Logger($"[Dawn Knight Taunt] {auraName} detected. Cycle #{auraCycle}; waiting for {assignedLabel}.");
                }
            }

            if (tauntArmed && !tauntAttempted && Environment.TickCount64 >= tauntAt)
            {
                tauntAttempted = TauntCurrentTarget("[Dawn Knight Taunt]", monster);
            }
            else
            {
                UseClassSkills();
                Bot.Sleep(400);
            }

            if (!auraActive)
            {
                auraWasActive = false;

                if (tauntAttempted)
                {
                    tauntArmed = false;
                    tauntAttempted = false;
                    tauntAt = 0;
                }
            }
        }
    }


    void KillRoom2SplitFocusTaunt(string cell, string pad)
    {
        const string dyingLight = "Dying Light";
        const string dawnKnight = "Dawn Knight";
        const string dyingAura = "Gathering Light";
        const string dawnAura = "Sun's Warmth";
        const int dawnTauntDelayMs = 6000;

        long noTargetSince = 0;

        bool dyingAuraWasActive = false;
        int dyingAuraCycle = 0;

        bool dawnAuraWasActive = false;
        bool dawnTauntArmed = false;
        bool dawnTauntAttempted = false;
        long dawnTauntAt = 0;

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

            bool isStoneCrusher = string.Equals(Bot.Player.CurrentClass?.Name, "StoneCrusher", StringComparison.OrdinalIgnoreCase) || string.Equals(Bot.Player.CurrentClass?.Name, "Infinity Titan", StringComparison.OrdinalIgnoreCase);

            bool isLegionRevenant = string.Equals(Bot.Player.CurrentClass?.Name, "Legion Revenant", StringComparison.OrdinalIgnoreCase);
            bool isArchPaladin = string.Equals(Bot.Player.CurrentClass?.Name, "ArchPaladin", StringComparison.OrdinalIgnoreCase);

            bool dawnKnightAlive = MonsterAvailable(dawnKnight, cell);
            bool dyingLightAlive = MonsterAvailable(dyingLight, cell);

            string target = "";

            if (isStoneCrusher)
                target = dawnKnightAlive ? dawnKnight : dyingLightAlive ? dyingLight : "";
            else
                target = dyingLightAlive ? dyingLight : dawnKnightAlive ? dawnKnight : "";

            if (!string.IsNullOrWhiteSpace(target))
                Bot.Combat.Attack(target);

            bool handledTauntWindow = false;

            if (target == dyingLight && dyingLightAlive)
            {
                bool auraActive = Bot.Player.HasTarget
                    && Bot.Target != null
                    && Bot.Target.Auras.Any(a => a.Name.Equals(dyingAura, StringComparison.OrdinalIgnoreCase));

                if (auraActive && !dyingAuraWasActive)
                {
                    dyingAuraWasActive = true;
                    dyingAuraCycle++;

                    bool lrTurn = dyingAuraCycle % 2 == 1;
                    bool apTurn = dyingAuraCycle % 2 == 0;

                    if ((lrTurn && isLegionRevenant) || (apTurn && isArchPaladin))
                    {
                        string role = isLegionRevenant ? "LR" : "AP";
                        Core.Logger($"[r2 Dying Light Taunt] {dyingAura} detected. Cycle #{dyingAuraCycle}; {role}'s turn to taunt.");
                        _ = TauntCurrentTarget("[r2 Dying Light Taunt]", dyingLight);
                        handledTauntWindow = true;
                    }
                    else if (isLegionRevenant || isArchPaladin)
                    {
                        string waitingFor = lrTurn ? "LR" : "AP";
                        Core.Logger($"[r2 Dying Light Taunt] {dyingAura} detected. Cycle #{dyingAuraCycle}; waiting for {waitingFor}.");
                    }
                }

                if (!auraActive)
                    dyingAuraWasActive = false;
            }
            else
            {
                dyingAuraWasActive = false;
            }

            if (isStoneCrusher && target == dawnKnight && dawnKnightAlive)
            {
                bool auraActive = Bot.Self.HasActiveAura(dawnAura);

                if (auraActive && !dawnAuraWasActive)
                {
                    dawnAuraWasActive = true;
                    dawnTauntArmed = true;
                    dawnTauntAttempted = false;
                    dawnTauntAt = Environment.TickCount64 + dawnTauntDelayMs;
                    Core.Logger($"[r2 SC Dawn Taunt] {dawnAura} detected. Taunting in {dawnTauntDelayMs / 1000.0:0.#} seconds.");
                }

                if (dawnTauntArmed && !dawnTauntAttempted && Environment.TickCount64 >= dawnTauntAt)
                {
                    dawnTauntAttempted = TauntCurrentTarget("[r2 SC Dawn Taunt]", dawnKnight);
                    handledTauntWindow = true;
                }

                if (!auraActive)
                {
                    dawnAuraWasActive = false;
                    dawnTauntArmed = false;
                    dawnTauntAttempted = false;
                    dawnTauntAt = 0;
                }
            }
            else
            {
                dawnAuraWasActive = false;
                dawnTauntArmed = false;
                dawnTauntAttempted = false;
                dawnTauntAt = 0;
            }

            if (!handledTauntWindow)
            {
                UseClassSkills();
                Bot.Sleep(400);
            }

            if (!dawnKnightAlive && !dyingLightAlive)
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

    void BossFight(string monster, string cell, string enrageMessage, bool isTaunter, int tauntOffsetSeconds = 0, string startingTaunterConfig = "player2")
    {
        bool needsEnrage = false;
        bool usedEnrage = false;
        bool usedLastEnrage = isTaunter &&
            Bot.Player.Username.Equals(Bot.Config!.Get<string>(startingTaunterConfig) ?? "", StringComparison.OrdinalIgnoreCase);
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
                        Core.Logger($"[Taunt] '{enrageMessage}' — my turn, applying Scroll of Enrage...");

                        if (TauntCurrentTarget(enrageMessage, maxAttempts: 10))
                        {
                            usedEnrage = true;
                            needsEnrage = false;
                            usedLastEnrage = true;
                        }
                    }
                    else if (usedLastEnrage)
                    {
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
                        usedLastEnrage = false;
                    }
                }

                if (!needsEnrage || usedEnrage || !Bot.Player.HasTarget)
                    Bot.Combat.Attack(monster);

                UseClassSkills();
                Bot.Sleep(300);

                if (Bot.Player.HasTarget &&
                    Bot.Player.Target != null &&
                    string.Equals(Bot.Player.Target.Name, monster, StringComparison.OrdinalIgnoreCase) &&
                    Bot.Player.Target.HP <= 0)
                {
                    noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;

                    if (Environment.TickCount64 - noTargetSince > 1800)
                        break;
                }
                else if (Bot.Player.HasTarget)
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
        finally { Bot.Flash.FlashCall -= Listener; }

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

                if (data == null || data?["cmd"]?.ToString() != "ct") return;

                bool triggered = false;
                if (data?["anims"] != null)
                    foreach (var a in data.anims)
                        if (a?.msg != null && ((string)a!.msg).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0)
                        { triggered = true; break; }

                if (!triggered && data?["a"] != null)
                    foreach (var a in data!.a)
                        if (a != null && a!["cmd"]?.ToString() == "aura+" && a?["auras"] != null)
                            foreach (var aura in a!["auras"])
                                if (aura?.msgOn != null &&
                                    ((string)aura!.msgOn).IndexOf(enrageMessage, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                    (bool)aura!.isNew)
                                { triggered = true; break; }

                if (!triggered) return;

                needsEnrage = true; usedEnrage = false;
                if (tauntOffsetSeconds > 0) tauntTime = DateTimeOffset.Now.AddSeconds(tauntOffsetSeconds);
                Core.Logger($"[Taunt] '{enrageMessage}' — {(tauntOffsetSeconds > 0 ? $"enraging in {tauntOffsetSeconds}s" : "enraging now")}.");
            }
            catch { }
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

    bool MonsterAvailable(string monster, string cell)
    {
        try
        {
            return Bot.Monsters.MapMonsters.Any(m =>
                m != null
                && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
                && (monster == "*" || string.Equals(m.Name, monster, StringComparison.OrdinalIgnoreCase))
                && m.Alive);
        }
        catch { return true; }
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

    void ResetDungeonInstance(string nextMap, string checkpoint)
    {
        SyncArmy($"{checkpoint}_reset_ready.sync");
        Core.Logger($"Resetting dungeon...");
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

    void RestockEnrageIfLow(string context, int minimumCount = 80)
    {
        bool needsEnrage =
            IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player2") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player3") ?? "") ||
            IsConfiguredAccount(Bot.Config!.Get<string>("player4") ?? "");

        if (!needsEnrage)
            return;

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
        JoinShrineDungeon(map, cell, pad);

        if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Count() < sArmy.Players().Length)
        {
            Core.Logger($"Only {Bot.Map.PlayerNames.Count()}/{sArmy.Players().Length} players in /{map}-{Core.PrivateRoomNumber}; retrying.");
            JoinShrineDungeon(map, cell, pad, force: true);
            Bot.Sleep(1000);
        }

        if (Bot.Player.Cell != cell) { Bot.Map.Jump(cell, pad, autoCorrect: false); Bot.Wait.ForCellChange(cell); }
        Bot.Player.SetSpawnPoint();
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
    }

    void JoinShrineDungeon(string map, string cell, string pad, bool force = false)
    {
        string target = $"{map}-{Core.PrivateRoomNumber}";
        for (int attempt = 1; attempt <= 5 && !Bot.ShouldExit && (force || Bot.Map.Name != map); attempt++)
        {
            force = false;
            Core.Logger($"Joining /{target} ({attempt}/5)...");
            Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%{target}%");
            WaitForMapName(map, 8000);
            if (Bot.Map.Name == map) break;
            Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");
            WaitForMapName(map, 8000);
            Bot.Sleep(500);
        }
        if (Bot.Map.Name != map) { Core.Logger($"Could not join /{target}. Retrying next loop."); return; }
        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 10);
    }

    void WaitForMapName(string map, int timeoutMs)
    {
        long end = Environment.TickCount64 + timeoutMs;
        while (!Bot.ShouldExit && Bot.Map.Name != map && Environment.TickCount64 < end)
            Bot.Sleep(250);
    }

    // ── Taunter checks ────────────────────────────────────────────────────────

    bool IsSunTaunter() =>
        IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "") ||
        IsConfiguredAccount(Bot.Config!.Get<string>("player2") ?? "");

    bool IsConfiguredAccount(string account) =>
        !string.IsNullOrWhiteSpace(account)
        && string.Equals(Core.Username(), account, StringComparison.OrdinalIgnoreCase);

    // ── Setup ─────────────────────────────────────────────────────────────────

    void EquipArmyClasses()
    {
        string username = Core.Username();
        string? p1 = Bot.Config!.Get<string>("player1");
        string? p2 = Bot.Config!.Get<string>("player2");
        string? p3 = Bot.Config!.Get<string>("player3");
        string? p4 = Bot.Config!.Get<string>("player4");

        if (username.Equals(p1, StringComparison.OrdinalIgnoreCase)) EquipClassByName(player1Class);
        else if (username.Equals(p2, StringComparison.OrdinalIgnoreCase)) EquipClassByName(player2Class);
        else if (username.Equals(p3, StringComparison.OrdinalIgnoreCase)) EquipClassByName(player3Class);
        else if (username.Equals(p4, StringComparison.OrdinalIgnoreCase)) EquipClassByName(player4Class);
        else Core.Logger("This account was not matched to a player slot — keeping current class.");
    }

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
                Core.Logger($"Missing required class: {className}.", stopBot: true);
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

    void UseClassPotions()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";

        Core.Logger($"Using potions for {className}...");

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
                Core.Logger($"[Potions] No potion profile for '{className}' — skipping.");
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
    void PrepareTauntRole()
    {
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
}

//lonewolf was here ( ͡° ͜ʖ ͡°)