/*
name: Ascend Eclipse Test
description: Tests the Ascension of the Eclipse dungeon in a loop. Handles dual final bosses (Ascended Solstice + Ascended Midnight) with HP-balanced targeting, alternating convergence taunts, and Daybreak/Nightfall cross-cancel mechanic.
tags: greatblade, entwined, eclipse, ascend, ascendeclipse, army, taunt, test
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Models.Quests;
using System;
using System.IO;
using System.Linq;

public class AscendEclipseTest
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
    public string OptionsStorage = "AscendEclipse_Test";
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
            "oracleClassTauntReset",
            "Oracle Class Taunt Reset",
            "ON: swaps to Oracle then back before taunt fights to clear the class taunt bug. (DOESNT USE POTIONS WHEN ON) \n" +
            "OFF: skips the Oracle swap and only ensures Scroll of Enrage is equipped.",
            true
        ),
        new Option<bool>(
            "usePotions",
            "Use Potions",
            "DOESNT WORK WHEN Oracle Class Taunt Reset IS ON. (this is by design) \n" +
            "ON: equips and uses configured class potion sets before starting the dungeon loop. (TONICS and ELIXIRS ONLY) \n" +
            "OFF: skips tonic/elixir usage.",
            false)
    };

    // Fixed 4-class lineup
    const string player1Class = "Legion Revenant"; // moon/Midnight side
    const string player2Class = "StoneCrusher";    // sun/Solstice side
    const string player3Class = "ArchPaladin";     // sun/Solstice side
    const string player4Class = "Lord of Order";   // moon/Midnight side

    readonly int[] lrSkillList = new[] { 3, 4, 2, 1 };
    readonly int[] scSkillList = new[] { 3, 2, 4, 1 };
    readonly int[] apSkillList = new[] { 2, 3, 1, 4 };
    readonly int[] looSkillList = new[] { 2, 3, 1, 4 };

    // Final boss names
    const string AscendedSolstice = "Ascended Solstice"; // sun — Solar Charge  → "The Sun Converges"
    const string AscendedMidnight = "Ascended Midnight"; // moon — Lunar Charge → "The Moon Converges"

    // Usable-slot taunt item
    const string TauntItem = "Scroll of Enrage";

    bool autoEnhance;
    bool autoGetEnrage;
    bool usePotions;
    bool oracleClassTauntReset;
    int runCount;
    bool syncFilesClearedOnStartup;

    /// <summary>
    /// Optional orchestration hook: when > 0, the main loop exits once the account holds
    /// at least this many Ecliptic Offering. Leave 0 for the normal "loop forever" behavior.
    /// </summary>
    public int TargetEclipticOfferingCount;
    const string EclipticOffering = "Ecliptic Offering";

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
            Core.Logger("Add 4 account names in the script options before starting.");

            if (!SkipSetOptions)
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
        oracleClassTauntReset = Bot.Config.Get<bool>("oracleClassTauntReset");
        usePotions = Bot.Config.Get<bool>("usePotions");

        SetupPartyFromPlayer1();

        EquipArmyClasses();

        if (autoEnhance)
            ApplyEnhancements();

        if (usePotions && !oracleClassTauntReset)
            UseClassPotions();
        else if (usePotions && oracleClassTauntReset)
            Core.Logger("[Potions] Skipping tonic/elixir usage because Oracle Class Taunt Reset is ON.");

        PrepareTauntRole();

        TryAcceptDailyQuest(9305, "Frozen Cycle");

        try
        {
            if (TargetEclipticOfferingCount > 0)
                Core.AddDrop(EclipticOffering);

            while (!Bot.ShouldExit)
            {
                if (TargetEclipticOfferingCount > 0 &&
                    Core.CheckInventory(EclipticOffering, TargetEclipticOfferingCount))
                {
                    Core.Logger($"[AscendEclipse] Reached target of {TargetEclipticOfferingCount} {EclipticOffering}; stopping.");
                    break;
                }

                RunAscendEclipse();
            }
        }
        finally
        {
            if (!SkipSetOptions)
                Core.SetOptions(false);
        }
    }

    // ── Dungeon run ───────────────────────────────────────────────────────────

    void RunAscendEclipse()
    {
        int run = ++runCount;
        string runCheckpoint = $"AscendEclipse_{run}";

        ResetReusableSyncFiles();

        ResetDungeonInstance("ascendeclipse", runCheckpoint);

        // Enter room — focus Fallen Star, swap to Blessless Deer while Solar Flare is active
        ArmyKillStarWithDeerCleanse("ascendeclipse", "Enter", "Left", $"{runCheckpoint}_01");

        // r1 — everyone rotates taunt on Suffocated Light
        ArmyKillMonsterWithRotatingAuraTaunt("ascendeclipse", "r1", "Left", "Suffocated Light", "Gathering Light", $"{runCheckpoint}_03", "Imprisoned Fairy");

        // r2 — SC focuses/taunts Sunset Knight, everyone else attacks Moon Haze with LR taunting it
        ArmyKillRoom2SplitTaunt("ascendeclipse", "r2", "Left", $"{runCheckpoint}_05");

        // r3 — dual final boss fight (Ascended Solstice + Ascended Midnight simultaneously)
        ArmyKillDualBoss("ascendeclipse", "r3", "Left", $"{runCheckpoint}_07");

        // Daily quest completion
        TryCompleteDailyQuest(9305, "Ecliptic Offering");
    }

    string GetConfiguredClassForCurrentAccount()
    {
        string username = Core.Username();

        if (username.Equals(Bot.Config!.Get<string>("player1"), StringComparison.OrdinalIgnoreCase)) return player1Class;
        if (username.Equals(Bot.Config.Get<string>("player2"), StringComparison.OrdinalIgnoreCase)) return player2Class;
        if (username.Equals(Bot.Config.Get<string>("player3"), StringComparison.OrdinalIgnoreCase)) return player3Class;
        if (username.Equals(Bot.Config.Get<string>("player4"), StringComparison.OrdinalIgnoreCase)) return player4Class;

        return Bot.Player.CurrentClass?.Name ?? string.Empty;
    }


    void ResetClassStateWithOracle(string originalClass)
    {
        const string resetClass = "Oracle";

        Bot.Skills.Pause();
        try
        {
            if (Core.CheckInventory(resetClass))
            {
                EquipClassByName(resetClass);
                Bot.Sleep(1000);
            }
            else
            {
                Core.Logger("[Taunt Check] Oracle is missing, so class-state reset is limited to re-equipping the original class.");
            }

            if (!string.IsNullOrWhiteSpace(originalClass))
            {
                EquipClassByName(originalClass);
                Bot.Sleep(1000);
            }
        }
        finally
        {
            Bot.Skills.Resume();
        }
    }

    void OracleResetBeforeRoomFight(string checkpoint)
    {
        string assignedClass = GetConfiguredClassForCurrentAccount();
        string logPrefix = $"[Taunt Setup {checkpoint}]";

        SyncArmy($"{checkpoint}_oracle_reset_ready.sync");

        if (oracleClassTauntReset)
        {
            Core.Logger($"[Oracle Reset] Resetting class state before {checkpoint}: Oracle -> {assignedClass}.");
            ResetClassStateWithOracle(assignedClass);
            Core.Logger($"[Oracle Reset] Class state reset complete before {checkpoint}.");
        }
        else
        {
            Core.Logger($"[Oracle Reset] Skipped before {checkpoint}; option is OFF.");
        }

        EnsureTauntItemEquipped(logPrefix);

        SyncArmy($"{checkpoint}_oracle_reset_done.sync");
        Core.Logger($"[Oracle Reset] Class state reset complete before {checkpoint}.");
    }

    void EnsureTauntItemEquipped(string logPrefix)
    {
        if (!Core.CheckInventory(TauntItem))
        {
            Core.Logger($"{logPrefix} {TauntItem} is missing; boss charges will NOT be redirected.");
            return;
        }

        if (!Bot.Inventory.IsEquipped(TauntItem))
        {
            Bot.Inventory.EquipUsableItem(TauntItem);
            Bot.Sleep(750);
        }

        if (Bot.Inventory.IsEquipped(TauntItem))
            Core.Logger($"{logPrefix} {TauntItem} equipped.");
        else
            Core.Logger($"{logPrefix} Tried to equip {TauntItem}, but it is not showing as equipped.");
    }

    // ── Army helpers ──────────────────────────────────────────────────────────

    void LogR3Debug(string cell, ref long lastStatusLogAt, ref long lastAuraLogAt, ref string lastTarget)
    {
        long now = Environment.TickCount64;

        if (now - lastStatusLogAt <= 3000)
            return;

        var solstice = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedSolstice, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        var midnight = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedMidnight, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        string solsticeText = solstice == null
            ? "dead/missing"
            : $"{solstice.HP}/{solstice.MaxHP} ({GetHpPercent(solstice.HP, solstice.MaxHP):0.0}%)";

        string midnightText = midnight == null
            ? "dead/missing"
            : $"{midnight.HP}/{midnight.MaxHP} ({GetHpPercent(midnight.HP, midnight.MaxHP):0.0}%)";

        Core.Logger(
            $"[r3 Status] HP: Solstice={solsticeText}, Midnight={midnightText}; " +
            $"Me: HP={Bot.Player.Health}, Alive={Bot.Player.Alive}, Cell={Bot.Player.Cell}, " +
            $"HasTarget={Bot.Player.HasTarget}"
        );

        lastStatusLogAt = now;
    }

    float GetHpPercent(int hp, int maxHp)
    {
        if (maxHp <= 0)
            return 0f;

        return hp * 100f / maxHp;
    }

    void ArmyKillStarWithDeerCleanse(string map, string cell, string pad, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");

        long noTargetSince = 0;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                if (!string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
                {
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                }

                Bot.Player.SetSpawnPoint();
                Bot.Sleep(500);
                continue;
            }

            if (!string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.Jump(cell, pad, autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
                Bot.Player.SetSpawnPoint();
            }

            bool isDeerFocusPlayer = IsConfiguredAccount(Bot.Config!.Get<string>("player4") ?? "");
            bool hasSolarFlare = Bot.Self.HasActiveAura("Solar Flare");

            string target = PickEnterRoomTarget(isDeerFocusPlayer, hasSolarFlare, cell);

            Bot.Combat.Attack(target);
            UseClassSkills();
            Bot.Sleep(400);

            bool fallenStarAlive = MonsterAvailable("Fallen Star", cell);
            bool blesslessDeerAlive = MonsterAvailable("Blessless Deer", cell);

            if (!fallenStarAlive && !blesslessDeerAlive)
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

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillRoom2SplitTaunt(string map, string cell, string pad, string checkpoint)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");
        OracleResetBeforeRoomFight(checkpoint);

        KillRoom2SplitTaunt(cell, pad);

        SyncArmy($"{checkpoint}_done.sync");
    }

    string PickEnterRoomTarget(bool isDeerFocusPlayer, bool hasSolarFlare, string cell)
    {
        bool fallenStarAlive = MonsterAvailable("Fallen Star", cell);
        bool blesslessDeerAlive = MonsterAvailable("Blessless Deer", cell);

        if (!fallenStarAlive && !blesslessDeerAlive)
            return "";

        if (isDeerFocusPlayer && blesslessDeerAlive)
            return "Blessless Deer";

        if (hasSolarFlare && blesslessDeerAlive)
            return "Blessless Deer";

        if (fallenStarAlive)
            return "Fallen Star";

        return "Blessless Deer";
    }

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
        OracleResetBeforeRoomFight(checkpoint);

        KillFocusedMonsterWithLrApAuraTaunt(monster, auraName, cell, pad, nextMonster);

        SyncArmy($"{checkpoint}_done.sync");
    }

    void ArmyKillWithDelayedTaunt(string map, string cell, string pad, string monster, string checkpoint, bool sunSide, string enrageMessage, int tauntOffsetSeconds = 6)
    {
        JoinAndFocus(map, cell, pad);
        SyncArmy($"{checkpoint}_ready.sync");
        bool isTaunter = sunSide ? IsSunTaunter() : IsMoonTaunter();
        string startingTaunterConfig = sunSide ? "player1" : "player3";
        BossFight(monster, cell, enrageMessage, isTaunter, tauntOffsetSeconds, startingTaunterConfig);
        SyncArmy($"{checkpoint}_done.sync");
    }

    /// <summary>
    /// Fights both Ascended Solstice and Ascended Midnight simultaneously.
    /// - Sun taunters (player2+3 / SC+AP) redirect "The Sun Converges" onto Solstice.
    /// - Moon taunters (player1+4 / LR+LoO) redirect "The Moon Converges" onto Midnight.
    /// - SC + AP focus Ascended Solstice.
    /// - LR + LoO focus Ascended Midnight.
    /// - Hollowed Eclipse (both convergences active = -5000% defenses) is avoided because
    ///   the 10s charge cooldown naturally spaces the two convergences apart.
    /// </summary>
    void ArmyKillDualBoss(string map, string cell, string pad, string checkpoint)
    {
        int resetCount = 0;

        while (!Bot.ShouldExit)
        {
            string attemptCheckpoint = resetCount == 0 ? checkpoint : $"{checkpoint}_retry_{resetCount}";
            JoinAndFocus(map, cell, pad);
            SyncArmy($"{attemptCheckpoint}_ready.sync");
            OracleResetBeforeRoomFight(attemptCheckpoint);

            if (DualBossFight(cell, pad, attemptCheckpoint))
                break;

            resetCount++;
            Core.Logger($"[r3 Reset] Player death detected; resetting dual boss fight. Attempt #{resetCount}.");
            ResetR3Fight(map, "r3a", "Left", cell, pad, $"{attemptCheckpoint}_soft_reset");
        }

        SyncArmy($"{checkpoint}_done.sync");
    }

    // ── Dual boss fight ───────────────────────────────────────────────────────

    bool DualBossFight(string cell, string pad, string checkpoint)
    {
        // r3 uses local convergence-cycle ownership instead of "last taunter" memory.
        // Sun cycle odd/even: player2(SC) / player3(AP)
        // Moon cycle odd/even: player1(LR) / player4(LoO)
        long lastR3StatusLogAt = 0;
        long lastR3AuraLogAt = 0;
        string lastR3Target = "";

        int sunCycle = 0;
        int moonCycle = 0;

        bool sunNeedsEnrage = false;
        bool moonNeedsEnrage = false;
        bool sunHandledThisCycle = true;
        bool moonHandledThisCycle = true;

        long lastSunTriggerAt = 0;
        long lastMoonTriggerAt = 0;

        int lastSunWaitingLogCycle = 0;
        int lastMoonWaitingLogCycle = 0;

        bool fightSpawnSet = false;
        long noTargetSince = 0;
        long solsticeFocusUntil = Environment.TickCount64 + 0;
        string deathResetPath = SyncGroupPath($"{checkpoint}_death_reset.signal");
        bool splitPhaseAnnounced = false;

        Core.Logger("[r3 Focus] No all-Solstice opener; using split target with HP balance guard.");

        Bot.Flash.FlashCall += Listener;
        try
        {
            while (!Bot.ShouldExit)
            {
                if (DeathResetRequested())
                {
                    Core.Logger("[r3 Reset] Another player died; resetting dual boss fight.");
                    return false;
                }

                if (!Bot.Player.Alive)
                {
                    RequestDeathReset();
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    return false;
                }

                ReturnToFightCell();

                int mySlot = GetMyPlayerSlot();

                LogR3Debug(cell, ref lastR3StatusLogAt, ref lastR3AuraLogAt, ref lastR3Target);

                bool solsticeAliveNow = MonsterAvailable(AscendedSolstice, cell);
                bool midnightAliveNow = MonsterAvailable(AscendedMidnight, cell);

                if (!solsticeAliveNow && !midnightAliveNow)
                {
                    noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;

                    if (Environment.TickCount64 - noTargetSince > 500)
                        return true;
                }
                else
                {
                    noTargetSince = 0;
                }

                // Clear stale convergence flags before processing taunts.
                // This prevents a late packet from forcing Scroll attempts after a boss is already dead.
                if (!solsticeAliveNow)
                {
                    sunNeedsEnrage = false;
                    sunHandledThisCycle = true;
                }

                if (!midnightAliveNow)
                {
                    moonNeedsEnrage = false;
                    moonHandledThisCycle = true;
                }

                // Keep opposite-side packet flags from polluting each account's logs.
                if (!IsSunTaunter())
                {
                    sunNeedsEnrage = false;
                    sunHandledThisCycle = true;
                }

                if (!IsMoonTaunter())
                {
                    moonNeedsEnrage = false;
                    moonHandledThisCycle = true;
                }

                // ── Sun convergence taunt (player2+3 / SC+AP on Ascended Solstice) ──────
                if (IsSunTaunter() && sunNeedsEnrage && !sunHandledThisCycle)
                {
                    int assignedSunSlot = sunCycle % 2 == 1 ? 2 : 3;

                    if (mySlot == assignedSunSlot)
                    {
                        Core.Logger($"[Sun Taunt] Sun cycle #{sunCycle}; my slot player{mySlot} is assigned.");
                        bool taunted = TryR3Taunt("[Sun Taunt]", AscendedSolstice, cell);

                        if (taunted)
                        {
                            sunNeedsEnrage = false;
                            sunHandledThisCycle = true;
                        }
                    }
                    else
                    {
                        if (lastSunWaitingLogCycle != sunCycle)
                        {
                            Core.Logger($"[Sun Taunt] Sun cycle #{sunCycle}; assigned to player{assignedSunSlot}, my slot is player{mySlot}. Continuing fight.");
                            lastSunWaitingLogCycle = sunCycle;
                        }

                        sunNeedsEnrage = false;
                        sunHandledThisCycle = true;
                    }
                }

                // ── Moon convergence taunt (player1+4 / LR+LoO on Ascended Midnight) ─────
                if (IsMoonTaunter() && moonNeedsEnrage && !moonHandledThisCycle)
                {
                    int assignedMoonSlot = moonCycle % 2 == 1 ? 1 : 4;

                    if (mySlot == assignedMoonSlot)
                    {
                        Core.Logger($"[Moon Taunt] Moon cycle #{moonCycle}; my slot player{mySlot} is assigned.");
                        bool taunted = TryR3Taunt("[Moon Taunt]", AscendedMidnight, cell);

                        if (taunted)
                        {
                            moonNeedsEnrage = false;
                            moonHandledThisCycle = true;
                        }
                    }
                    else
                    {
                        if (lastMoonWaitingLogCycle != moonCycle)
                        {
                            Core.Logger($"[Moon Taunt] Moon cycle #{moonCycle}; assigned to player{assignedMoonSlot}, my slot is player{mySlot}. Continuing fight.");
                            lastMoonWaitingLogCycle = moonCycle;
                        }

                        moonNeedsEnrage = false;
                        moonHandledThisCycle = true;
                    }
                }

                // ── r3 focus targets ───────────────────────────────────────────────
                // No opener by default.
                // Normal split:
                // SC + AP focus Ascended Solstice.
                // LR + LoO focus Ascended Midnight.
                // Balance guard in GetR3FocusTarget() temporarily shifts damage to Midnight
                // if Solstice gets too far ahead/lower.
                if (!splitPhaseAnnounced && Environment.TickCount64 >= solsticeFocusUntil)
                {
                    Core.Logger("[r3 Focus] Split target with HP balance guard active.");
                    splitPhaseAnnounced = true;
                }

                string target = Environment.TickCount64 < solsticeFocusUntil && MonsterAvailable(AscendedSolstice, cell)
                    ? AscendedSolstice
                    : GetR3FocusTarget(cell);

                if (!string.IsNullOrEmpty(target))
                {
                    Bot.Combat.Attack(target);
                    UseClassSkills();
                }

                Bot.Sleep(300);

                bool solsticeAlive = MonsterAvailable(AscendedSolstice, cell);
                bool midnightAlive = MonsterAvailable(AscendedMidnight, cell);

                if (!solsticeAlive && !midnightAlive)
                {
                    noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;
                    if (Environment.TickCount64 - noTargetSince > 1800)
                        return true;
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

        return false;

        void RequestDeathReset()
        {
            Core.Logger("[r3 Death Reset] I died; requesting r3 reset.");

            WriteSyncLines(
                deathResetPath,
                ReadSyncLines(deathResetPath)
                    .Where(l => !l.StartsWith($"{checkpoint}:", StringComparison.OrdinalIgnoreCase))
                    .Append($"{checkpoint}:{Core.Username().ToLower()}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}")
                    .ToArray()
            );
        }

        bool DeathResetRequested()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return ReadSyncLines(deathResetPath)
                .Select(l => l.Split(':'))
                .Any(p =>
                    p.Length >= 3
                    && p[0].Equals(checkpoint, StringComparison.OrdinalIgnoreCase)
                    && long.TryParse(p[2], out long ts)
                    && now - ts <= 120);
        }

        void ReturnToFightCell()
        {
            if (string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
            {
                if (!fightSpawnSet)
                {
                    Bot.Player.SetSpawnPoint();
                    fightSpawnSet = true;
                }

                return;
            }

            Core.Logger($"[Recover] Returning to {cell} after respawn/desync.");
            Bot.Map.Jump(cell, pad, autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
            Bot.Player.SetSpawnPoint();
            fightSpawnSet = true;
            Bot.Sleep(500);
        }

        void Listener(string name, object[] args)
        {
            try
            {
                if (name != "pext" && name != "packetFromServer")
                    return;

                // Prefilter on the raw packet text so the expensive dynamic JSON parse
                // only runs for packets that can actually contain a convergence message.
                if (args.Length == 0 || args[0] is not string raw ||
                    !raw.Contains("Converges", StringComparison.OrdinalIgnoreCase))
                    return;

                dynamic? data = null;

                if (name == "pext")
                {
                    var packet = JsonConvert.DeserializeObject<dynamic>(raw)!;
                    if (packet?["params"]?["type"]?.ToString() == "json")
                        data = packet["params"]["dataObj"];
                }
                else if (name == "packetFromServer")
                {
                    var packet = JsonConvert.DeserializeObject<dynamic>(raw)!;
                    data = packet?["b"]?["o"];
                }

                if (data == null || data!["cmd"]?.ToString() != "ct")
                    return;

                bool sunTriggered = false;
                bool moonTriggered = false;

                if (data!["anims"] != null)
                {
                    foreach (var a in data!.anims)
                    {
                        if (a?.msg == null)
                            continue;

                        string msg = (string)a.msg;

                        if (msg.IndexOf("The Sun Converges", StringComparison.OrdinalIgnoreCase) >= 0)
                            sunTriggered = true;

                        if (msg.IndexOf("The Moon Converges", StringComparison.OrdinalIgnoreCase) >= 0)
                            moonTriggered = true;
                    }
                }

                if ((!sunTriggered || !moonTriggered) && data["a"] != null)
                {
                    foreach (var a in data.a)
                    {
                        if (a == null || a!["cmd"]?.ToString() != "aura+" || a!["auras"] == null)
                            continue;

                        foreach (var aura in a!["auras"])
                        {
                            if (aura?.msgOn == null || !(bool)aura!.isNew)
                                continue;

                            string msg = (string)aura!.msgOn;

                            if (msg.IndexOf("The Sun Converges", StringComparison.OrdinalIgnoreCase) >= 0)
                                sunTriggered = true;

                            if (msg.IndexOf("The Moon Converges", StringComparison.OrdinalIgnoreCase) >= 0)
                                moonTriggered = true;
                        }
                    }
                }

                long now = Environment.TickCount64;

                if (sunTriggered && now - lastSunTriggerAt > 1500)
                {
                    lastSunTriggerAt = now;
                    sunCycle++;
                    sunNeedsEnrage = true;
                    sunHandledThisCycle = false;

                    int assignedSunSlot = sunCycle % 2 == 1 ? 2 : 3;
                    Core.Logger($"[Sun Taunt] 'The Sun Converges' detected. Sun cycle #{sunCycle}; assigned to player{assignedSunSlot}.");
                }

                if (moonTriggered && now - lastMoonTriggerAt > 1500)
                {
                    lastMoonTriggerAt = now;
                    moonCycle++;
                    moonNeedsEnrage = true;
                    moonHandledThisCycle = false;

                    int assignedMoonSlot = moonCycle % 2 == 1 ? 1 : 4;
                    Core.Logger($"[Moon Taunt] 'The Moon Converges' detected. Moon cycle #{moonCycle}; assigned to player{assignedMoonSlot}.");
                }
            }
            catch
            {
                // Ignore malformed combat packets.
            }
        }
    }

    bool TryR3Taunt(string logPrefix, string bossName, string cell)
    {
        if (!MonsterAvailable(bossName, cell))
        {
            Core.Logger($"{logPrefix} {bossName} is not available; cannot taunt.");
            return false;
        }

        Core.Logger($"{logPrefix} Targeting {bossName} for Scroll of Enrage.");
        Core.Logger($"{logPrefix} Force-using Scroll of Enrage on {bossName}; skipping CanUseSkill(5) gate.");

        Bot.Skills.Pause();

        try
        {
            for (int attempt = 1; attempt <= 10 && !Bot.ShouldExit; attempt++)
            {
                if (!MonsterAvailable(bossName, cell))
                {
                    Core.Logger($"{logPrefix} {bossName} died/disappeared during taunt attempts; treating taunt as handled.");
                    return true;
                }

                Core.Logger($"{logPrefix} Scroll attempt {attempt}/10.");

                // Retarget every attempt so a stale target or desync cannot make all retries hit the wrong target.
                Bot.Combat.Attack(bossName);
                Bot.Sleep(100);
                Bot.Combat.CancelAutoAttack();

                Core.UsePotion();

                long confirmEnd = Environment.TickCount64 + 500;

                while (!Bot.ShouldExit && Environment.TickCount64 < confirmEnd)
                {
                    Bot.Sleep(100);

                    if (!MonsterAvailable(bossName, cell))
                    {
                        Core.Logger($"{logPrefix} {bossName} died/disappeared while confirming Enrage; treating taunt as handled.");
                        return true;
                    }

                    if (Bot.Player.HasTarget &&
                        Bot.Target != null &&
                        Bot.Target.Auras.Any(x =>
                            (x.Name.Equals("Focus", StringComparison.OrdinalIgnoreCase) ||
                             x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase)) &&
                            x.RemainingTime > 4))
                    {
                        Core.Logger($"{logPrefix} Enrage confirmed on {bossName}.");
                        return true;
                    }
                }

                Bot.Sleep(150);
            }

            Core.Logger($"{logPrefix} Scroll force-use attempts failed; Enrage was not confirmed on {bossName}.");
            return false;
        }
        finally
        {
            Bot.Skills.Resume();
        }
    }

    void ResetR3Fight(string map, string safeCell, string safePad, string bossCell, string bossPad, string checkpoint)
    {
        Bot.Skills.Pause();

        try
        {
            Core.Logger($"[r3 Death Reset] Stopping combat and moving to {safeCell}.");

            Bot.Combat.CancelAutoAttack();

            if (!Bot.Player.Alive)
            {
                Core.Logger("[r3 Death Reset] I am dead; waiting to respawn before moving to safe cell.");
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 30);
                Bot.Sleep(1000);
            }

            if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
                JoinShrineDungeon(map, safeCell, safePad);

            if (!string.Equals(Bot.Player.Cell, safeCell, StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.Jump(safeCell, safePad, autoCorrect: false);
                Bot.Wait.ForCellChange(safeCell);
            }

            Bot.Player.SetSpawnPoint();

            Core.Logger($"[r3 Death Reset] Reached {safeCell}; waiting for all accounts.");
            SyncArmy($"{checkpoint}_all_in_safe_cell.sync");

            if (!Bot.Player.Alive)
            {
                Core.Logger("[r3 Death Reset] I died during retreat; waiting to respawn again.");
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 30);
                Bot.Sleep(1000);

                if (!string.Equals(Bot.Player.Cell, safeCell, StringComparison.OrdinalIgnoreCase))
                {
                    Bot.Map.Jump(safeCell, safePad, autoCorrect: false);
                    Bot.Wait.ForCellChange(safeCell);
                }

                Bot.Player.SetSpawnPoint();
            }

            SyncArmy($"{checkpoint}_all_alive_safe.sync");

            Core.Logger($"[r3 Death Reset] Everyone synced in {safeCell}; returning to {bossCell}.");

            Bot.Map.Jump(bossCell, bossPad, autoCorrect: false);
            Bot.Wait.ForCellChange(bossCell);
            Bot.Player.SetSpawnPoint();

            Bot.Sleep(1000);

            SyncArmy($"{checkpoint}_all_returned_to_r3.sync");

            Core.Logger("[r3 Death Reset] r3 retry ready.");
        }
        finally
        {
            Bot.Skills.Resume();
        }
    }

    /// <summary>
    /// Returns the r3 focus target with a balance guard.
    /// Normal split: SC + AP focus Ascended Solstice, LR + LoO focus Ascended Midnight.
    /// If Solstice is getting too far lower than Midnight, SC temporarily helps Midnight.
    /// If Solstice is dangerously far lower, everyone temporarily helps Midnight catch down.
    /// </summary>
    string GetR3FocusTarget(string cell)
    {
        var solstice = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedSolstice, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        var midnight = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedMidnight, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        if (solstice == null && midnight == null)
            return "";

        if (solstice == null)
            return AscendedMidnight;

        if (midnight == null)
            return AscendedSolstice;

        float solsticePct = GetHpPercent(solstice.HP, solstice.MaxHP);
        float midnightPct = GetHpPercent(midnight.HP, midnight.MaxHP);
        float gap = midnightPct - solsticePct;
        int mySlot = GetMyPlayerSlot();

        // Emergency correction:
        // Solstice is much lower than Midnight, so pause Solstice damage entirely.
        if (gap >= 7f)
            return AscendedMidnight;

        // Mild correction:
        // SC swaps to Midnight, AP stays on Solstice. LR + LoO continue Midnight.
        if (gap >= 4f)
        {
            if (mySlot == 2) // SC
                return AscendedMidnight;

            if (mySlot == 3) // AP
                return AscendedSolstice;

            if (mySlot == 1 || mySlot == 4) // LR + LoO
                return AscendedMidnight;
        }

        // Normal split:
        // SC + AP focus Solstice. LR + LoO focus Midnight.
        if (mySlot == 2 || mySlot == 3)
            return AscendedSolstice;

        if (mySlot == 1 || mySlot == 4)
            return AscendedMidnight;

        return solsticePct >= midnightPct ? AscendedSolstice : AscendedMidnight;
    }
    string PickHigherHpBoss(string cell)
    {
        var solstice = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedSolstice, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        var midnight = Bot.Monsters.MapMonsters.FirstOrDefault(m =>
            m != null
            && string.Equals(m.Name, AscendedMidnight, StringComparison.OrdinalIgnoreCase)
            && string.Equals(m.Cell, cell, StringComparison.OrdinalIgnoreCase)
            && m.Alive);

        if (solstice == null && midnight == null) return "";
        if (solstice == null) return AscendedMidnight;
        if (midnight == null) return AscendedSolstice;

        float solsticePct = solstice.MaxHP > 0 ? (float)solstice.HP / solstice.MaxHP : 0f;
        float midnightPct = midnight.MaxHP > 0 ? (float)midnight.HP / midnight.MaxHP : 0f;

        return solsticePct >= midnightPct ? AscendedSolstice : AscendedMidnight;
    }

    // ── Single-target boss fight (r2 taunt bosses) ────────────────────────────

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
                if (!Bot.Player.Alive) { Bot.Wait.ForTrue(() => Bot.Player.Alive, 20); ReturnToFightCell(cell); Bot.Sleep(500); continue; }

                ReturnToFightCell(cell);

                if (isTaunter && needsEnrage && !usedEnrage)
                {
                    if (!usedLastEnrage && Bot.Player.HasTarget &&
                        (tauntOffsetSeconds <= 0 || DateTimeOffset.Now > tauntTime))
                    {
                        Core.Logger($"[Taunt] '{enrageMessage}' — my turn, applying Scroll of Enrage...");
                        Bot.Skills.Pause();
                        while (!Bot.ShouldExit && Bot.Player.HasTarget && needsEnrage && !usedEnrage)
                        {
                            Bot.Combat.CancelAutoAttack();
                            Core.UsePotion();
                            Bot.Sleep(200);
                            if (Bot.Player.HasTarget &&
                                (Bot.Target.Auras.Any(x => x.Name.Equals("Focus", StringComparison.OrdinalIgnoreCase) && x.RemainingTime > 4) ||
                                 Bot.Target.Auras.Any(x => x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase) && x.RemainingTime > 4)))
                            {
                                usedEnrage = true; needsEnrage = false; usedLastEnrage = true;
                                Core.Logger("[Taunt] Enrage confirmed!");
                            }
                            else { Bot.Sleep(200); }
                        }
                        Bot.Skills.Resume();
                    }
                    else if (usedLastEnrage)
                    {
                        Core.Logger($"[Taunt] '{enrageMessage}' — other taunter's turn.");
                        usedEnrage = true; needsEnrage = false; usedLastEnrage = false;
                    }
                }

                if (!needsEnrage || usedEnrage || !Bot.Player.HasTarget)
                {
                    Bot.Combat.Attack(monster);
                    UseClassSkills();
                }

                Bot.Sleep(300);

                if (Bot.Player.HasTarget) { noTargetSince = 0; }
                else if (!MonsterAvailable(monster, cell))
                {
                    noTargetSince = noTargetSince == 0 ? Environment.TickCount64 : noTargetSince;
                    if (Environment.TickCount64 - noTargetSince > 1800) break;
                }
                else { noTargetSince = 0; }
            }
        }
        finally { Bot.Flash.FlashCall -= Listener; }

        void ReturnToFightCell(string targetCell)
        {
            if (string.Equals(Bot.Player.Cell, targetCell, StringComparison.OrdinalIgnoreCase))
            {
                if (!fightSpawnSet) { Bot.Player.SetSpawnPoint(); fightSpawnSet = true; }
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
                    foreach (var a in data!.a)
                        if (a != null && a!["cmd"]?.ToString() == "aura+" && a!["auras"] != null)
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

    // ── Infrastructure ────────────────────────────────────────────────────────
    void SetupPartyFromPlayer1()
    {
        string player1 = Bot.Config!.Get<string>("player1") ?? "";
        string player2 = Bot.Config.Get<string>("player2") ?? "";
        string player3 = Bot.Config.Get<string>("player3") ?? "";
        string player4 = Bot.Config.Get<string>("player4") ?? "";

        if (string.IsNullOrWhiteSpace(player1))
        {
            Core.Logger("[Party] No player1 configured; skipping party setup.");
            return;
        }

        Bot.Flash.FlashCall += PartyInviteListener;

        try
        {
            SyncArmy("party_setup_ready.sync");

            EnsurePartyInvites();

            if (IsConfiguredAccount(player1))
            {
                Core.Logger("[Party] I am player1; inviting the other accounts.");

                //PartyOn();

                PartyInvite(player2);
                Bot.Sleep(750);

                PartyInvite(player3);
                Bot.Sleep(750);

                PartyInvite(player4);
                Bot.Sleep(750);
            }
            else
            {
                Core.Logger("[Party] Waiting for player1 party invite.");
            }

            Bot.Sleep(5000);
            SyncArmy("party_setup_done.sync");
        }
        finally
        {
            Bot.Flash.FlashCall -= PartyInviteListener;
        }
    }
    bool EnsurePartyInvites()
    {
        bool partyEnabled = Bot.Flash.GetGameObject<bool>("uoPref.bParty");
        if (!partyEnabled)
        {
            Core.Logger("Party invites enabled");
            Bot.Send.Packet("%xt%zm%cmd%1%uopref%bParty%true%");
            Bot.Sleep(500);
            return true;
        }
        Core.Logger("Party invites already enabled");
        return true;
    }

    void PartyInvite(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (name.Equals(Core.Username(), StringComparison.OrdinalIgnoreCase))
            return;

        Core.Logger("[Party] Sending invite to configured account.");
        Bot.Send.Packet($"%xt%zm%gp%1%pi%{name}%");
    }

    void PartyAccept(int partyID)
    {
        Bot.Send.Packet($"%xt%zm%gp%1%pa%{partyID}%");
    }

    void PartyOn()
    {
        Bot.Send.Packet("%xt%zm%cmd%1%partyon%");
    }

    void PartyInviteListener(string name, object[] args)
    {
        try
        {
            if (name != "pext")
                return;

            dynamic packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;

            if (packet?["params"]?["type"]?.ToString() != "json")
                return;

            dynamic data = packet["params"]["dataObj"];

            if (data?["cmd"]?.ToString() != "pi")
                return;

            int partyID = data.pid;

            PartyAccept(partyID);
            Core.Logger("[Party] Accepted player1's party invite.");

            Bot.Sleep(1000);
            Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
        }
        catch
        {
            // Ignore malformed or unrelated packets.
        }
    }

    string SyncGroupPath(string syncFile)
    {
        string group = GetSyncGroup(syncFile);
        return Ultra.ResolveSyncPath($"AscendEclipse_{Core.PrivateRoomNumber}_{group}.sync");
    }

    string GetSyncGroup(string syncFile)
    {
        string name = syncFile.ToLower();

        if (name.Contains("party"))
            return "setup";

        if (name.Contains("reset") || name.Contains("whitemap") || name.Contains("rejoined") || name.Contains("restock"))
            return "reset";

        return "run";
    }


    void ResetReusableSyncFiles()
    {
        // Sync files are cleared at startup only; stale lines from earlier runs are
        // pruned by SyncArmy, so per-run clearing is unnecessary.
        if (syncFilesClearedOnStartup)
            return;

        Core.Logger("[Sync] Moving to /whitemap before clearing startup sync files.");
        Core.Join("whitemap", "Enter", "Spawn");
        WaitForMapName("whitemap", 8000);
        Bot.Sleep(1000);

        bool isPlayer1 = IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "");

        if (!isPlayer1)
        {
            Core.Logger("[Sync] Waiting briefly while player1 clears startup sync files.");
            Bot.Sleep(1500);
            syncFilesClearedOnStartup = true;
            Core.Logger("[Sync] Done waiting for player1 startup sync clear.");
            return;
        }

        Core.Logger("[Sync] Player1 is clearing startup sync files for AscendEclipse.");

        ResetSyncFile(SyncGroupPath("AscendEclipse_reset_ready.sync"));
        ResetSyncFile(SyncGroupPath("AscendEclipse_run_ready.sync"));

        syncFilesClearedOnStartup = true;

        Core.Logger("[Sync] Startup sync files cleared.");
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
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string[] lines = ReadSyncLines(path);

            lines = lines
                .Where(l => !l.StartsWith($"{checkpoint}:{username}:", StringComparison.OrdinalIgnoreCase))
                .Where(l => !IsStaleSyncLine(l, now))
                .ToArray();

            WriteSyncLines(
                path,
                lines
                    .Append($"{checkpoint}:{username}:ready:{now}")
                    .ToArray()
            );

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

    /// <summary>
    /// True when a sync line's trailing unix timestamp is older than 10 minutes.
    /// Handles both ready lines (checkpoint:user:ready:ts) and death-reset signals
    /// (checkpoint:user:ts); lines without a parseable timestamp are kept.
    /// </summary>
    bool IsStaleSyncLine(string line, long now)
    {
        const long maxAgeSeconds = 600;

        int lastColon = line.LastIndexOf(':');

        if (lastColon < 0 || lastColon == line.Length - 1)
            return false;

        if (!long.TryParse(line[(lastColon + 1)..], out long ts))
            return false;

        return now - ts > maxAgeSeconds;
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

    void UseStrictSkillRotation(int[] skillList, ref int skillIndex)
    {
        if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
        {
            int skill = skillList[skillIndex];

            if (Bot.Skills.CanUseSkill(skill))
            {
                Bot.Skills.UseSkill(skill);
                skillIndex = (skillIndex + 1) % skillList.Length;
            }
        }
    }

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

    void KillRoom2SplitTaunt(string cell, string pad)
    {
        long noTargetSince = 0;

        bool lrMoonlightGazeArmed = false;
        bool lrMoonlightGazeTaunted = false;
        long lrTauntAt = 0;

        bool scSunsWarmthArmed = false;
        bool scSunsWarmthTaunted = false;
        long scTauntAt = 0;

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

            bool sunsetKnightAlive = MonsterAvailable("Sunset Knight", cell);
            bool moonHazeAlive = MonsterAvailable("Moon Haze", cell);

            string target;

            if (isStoneCrusher)
            {
                target = sunsetKnightAlive ? "Sunset Knight" : "Moon Haze";
            }
            else
            {
                target = moonHazeAlive ? "Moon Haze" : "Sunset Knight";
            }

            if (!string.IsNullOrWhiteSpace(target))
                Bot.Combat.Attack(target);

            if (isLegionRevenant && target == "Moon Haze" && moonHazeAlive)
            {
                bool moonlightGazeActive = Bot.Self.HasActiveAura("Moonlight Gaze");

                if (moonlightGazeActive && !lrMoonlightGazeArmed)
                {
                    lrMoonlightGazeArmed = true;
                    lrMoonlightGazeTaunted = false;
                    lrTauntAt = Environment.TickCount64 + 6000;
                    Core.Logger("[r2 LR Taunt] Moonlight Gaze detected on Moon Haze. Taunting in 6 seconds.");
                }

                if (lrMoonlightGazeArmed && !lrMoonlightGazeTaunted && Environment.TickCount64 >= lrTauntAt)
                {
                    lrMoonlightGazeTaunted = TauntCurrentTarget("[r2 LR Taunt]", "Moon Haze");
                }
                else
                {
                    UseClassSkills();
                    Bot.Sleep(400);
                }

                if (!moonlightGazeActive && lrMoonlightGazeTaunted)
                {
                    lrMoonlightGazeArmed = false;
                    lrMoonlightGazeTaunted = false;
                    lrTauntAt = 0;
                }
            }
            else if (isStoneCrusher && target == "Sunset Knight" && sunsetKnightAlive)
            {
                bool sunsWarmthActive = Bot.Self.HasActiveAura("Sun's Warmth");

                if (sunsWarmthActive && !scSunsWarmthArmed)
                {
                    scSunsWarmthArmed = true;
                    scSunsWarmthTaunted = false;
                    scTauntAt = Environment.TickCount64 + 6000;
                    Core.Logger("[r2 SC Taunt] Sun's Warmth detected on Sunset Knight. Taunting in 6 seconds.");
                }

                if (scSunsWarmthArmed && !scSunsWarmthTaunted && Environment.TickCount64 >= scTauntAt)
                {
                    scSunsWarmthTaunted = TauntCurrentTarget("[r2 SC Taunt]", "Sunset Knight");
                }
                else
                {
                    UseClassSkills();
                    Bot.Sleep(400);
                }

                if (!sunsWarmthActive && scSunsWarmthTaunted)
                {
                    scSunsWarmthArmed = false;
                    scSunsWarmthTaunted = false;
                    scTauntAt = 0;
                }
            }
            else
            {
                UseClassSkills();
                Bot.Sleep(400);
            }

            if (!sunsetKnightAlive && !moonHazeAlive)
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
                Core.Logger($"{logPrefix} Scroll attempt {attempt}/{maxAttempts}.");

                Core.UsePotion();

                long confirmEnd = Environment.TickCount64 + 500;

                while (!Bot.ShouldExit && Environment.TickCount64 < confirmEnd)
                {
                    Bot.Sleep(100);

                    if (Bot.Player.HasTarget &&
                        Bot.Target != null &&
                        Bot.Target.Auras.Any(x =>
                            (x.Name.Equals("Focus", StringComparison.OrdinalIgnoreCase) ||
                             x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase)) &&
                            x.RemainingTime > 4))
                    {
                        Core.Logger($"{logPrefix} Enrage confirmed on {expectedTarget}.");
                        return true;
                    }
                }

                Bot.Sleep(150);
            }

            Core.Logger($"{logPrefix} Scroll force-use attempts failed; Enrage was not confirmed on {expectedTarget}.");
            return false;
        }
        finally
        {
            Bot.Skills.Resume();
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
                    Core.Logger($"[r1 Taunt] {auraName} detected on {monster}. Cycle #{auraCycle}; {role}'s turn to taunt.");
                    _ = TauntCurrentTarget("[r1 Taunt]", monster);
                }
                else
                {
                    if (isLegionRevenant || isArchPaladin)
                    {
                        string waitingFor = lrTurn ? "LR" : "AP";
                        Core.Logger($"[r1 Taunt] {auraName} detected on {monster}. Cycle #{auraCycle}; waiting for {waitingFor}.");
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

    int GetMyPlayerSlot()
    {
        string username = Core.Username();

        if (username.Equals(Bot.Config!.Get<string>("player1"), StringComparison.OrdinalIgnoreCase)) return 1;
        if (username.Equals(Bot.Config.Get<string>("player2"), StringComparison.OrdinalIgnoreCase)) return 2;
        if (username.Equals(Bot.Config.Get<string>("player3"), StringComparison.OrdinalIgnoreCase)) return 3;
        if (username.Equals(Bot.Config.Get<string>("player4"), StringComparison.OrdinalIgnoreCase)) return 4;

        return 0;
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

    void ResetDungeonInstance(string nextMap, string checkpoint)
    {
        SyncArmy($"{checkpoint}_reset_ready.sync");
        Core.Logger("Resetting dungeon...");
        Core.Join("whitemap", "Enter", "Spawn");
        WaitForMapName("whitemap", 8000);
        Bot.Sleep(1000);
        SyncArmy($"{checkpoint}_all_in_whitemap.sync");
        SellHallowedRemainsIfMax();
        RefreshPotionsIfAurasMissing();
        RestockEnrageIfLow($"Before {checkpoint}", minimumCount: 80);
        EnsureTauntItemEquipped("[Restock]");

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

    void RestockEnrageIfLow(string context, int minimumCount = 80)
    {
        if (!autoGetEnrage)
            return;

        int count = GetInventoryQuantity(TauntItem);
        Core.Logger($"[Taunt] {context}: {count} {TauntItem} remaining.");

        if (count >= minimumCount)
            return;

        Core.Logger($"[Taunt] {TauntItem} is below {minimumCount}. Restocking before continuing.");

        Ultra.GetScrollOfEnrage();

        int newCount = GetInventoryQuantity(TauntItem);
        Core.Logger($"[Taunt] Restock complete: {newCount} {TauntItem} remaining.");

        EnsureTauntItemEquipped("[Taunt Restock]");
    }

    void JoinAndFocus(string map, string cell, string pad)
    {
        JoinShrineDungeon(map, cell, pad);

        if (!string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
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
        IsConfiguredAccount(Bot.Config!.Get<string>("player2") ?? "") ||
        IsConfiguredAccount(Bot.Config!.Get<string>("player3") ?? "");

    bool IsMoonTaunter() =>
        IsConfiguredAccount(Bot.Config!.Get<string>("player1") ?? "") ||
        IsConfiguredAccount(Bot.Config!.Get<string>("player4") ?? "");

    bool IsConfiguredAccount(string account) =>
        !string.IsNullOrWhiteSpace(account)
        && string.Equals(Core.Username(), account, StringComparison.OrdinalIgnoreCase);

    // ── Setup ─────────────────────────────────────────────────────────────────

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

            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
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

            case "stonecrusher":
            case "infinity titan":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            default:
                Core.Logger($"[Enhance] No profile for '{className}' — skipping.");
                break;
        }
    }

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

    void RefreshPotionsIfAurasMissing()
    {
        if (!usePotions)
            return;

        if (oracleClassTauntReset)
        {
            Core.Logger("[Potions] Skipping potion refresh because Oracle Class Taunt Reset is ON.");
            return;
        }

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

    void PrepareTauntRole()
    {
        if (autoGetEnrage)
        {
            Core.Logger($"Auto-crafting {TauntItem}...");
            Ultra.GetScrollOfEnrage();
        }

        if (!Core.CheckInventory(TauntItem))
        {
            Core.Logger($"No {TauntItem} found — boss charges will NOT be redirected.");
            return;
        }

        if (!Bot.Inventory.IsEquipped(TauntItem))
            Bot.Inventory.EquipUsableItem(TauntItem);

        Core.Logger($"{TauntItem} is equipped.");
    }
}

//lonewolf was here ( ͡° ͜ʖ ͡°)