/*
name: UltraDailies
description: Runs Ultra Ezrajal → Warden → Engineer → Avatar Tyndarius in sequence. Enhances once at start, restocks potions between each boss.
tags: ultra, daily, ezrajal, warden, engineer, tyndarius
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraPotions.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraEnhancements.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDailies
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private static UltraPotions Pots
    {
        get => _Pots ??= new UltraPotions();
        set => _Pots = value;
    }
    private static UltraPotions _Pots;

    private static UltraEnhancements Enh
    {
        get => _Enh ??= new UltraEnhancements();
        set => _Enh = value;
    }
    private static UltraEnhancements _Enh;

    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    // Warden taunt state
    private DateTime wardenFightStartTime = DateTime.MinValue;
    private double wardenTauntOffset = 0;
    private const double WardenTauntInterval = 12.0;
    private const double WardenTauntWindow = 6.0;
    private DateTime wardenLastTaunt = DateTime.MinValue;

    // Tyndarius role flags
    private bool isBall1Taunter = false;
    private bool isBall2Killer = false;

    // Tyndarius timer-based taunt state
    private DateTime tynFightStartTime = DateTime.MinValue;
    private double tynTauntOffset = 0;
    private const double TynTauntInterval = 12.0;
    private const double TynTauntWindow = 6.0;
    private DateTime tynLastTaunt = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDailies";

    public List<IOption> Options = new()
    {
        // ── Warden ───────────────────────────────────────────────────────────
        new Option<string>(
            "WardenTaunter1",
            "Warden — Primary Taunter",
            "Class name of Warden Taunter 1 (fires at 0s).\nLeave blank if not applicable.",
            ""
        ),
        new Option<string>(
            "WardenTaunter2",
            "Warden — Secondary Taunter",
            "Class name of Warden Taunter 2 (fires at 6s).\nLeave blank if not applicable.",
            ""
        ),

        // ── Tyndarius ─────────────────────────────────────────────────────────
        new Option<string>(
            "TynBall1Taunter",
            "Tyndarius — Ball 1 Taunter",
            "Class name that taunts Ball 1.",
            ""
        ),
        new Option<string>(
            "TynBall2Killer",
            "Tyndarius — Ball 2 Killer",
            "Class name that kills Ball 2.",
            ""
        ),
        new Option<string>(
            "TynTaunter1",
            "Tyndarius — Taunter 1",
            "Class name of Tyndarius Taunter 1 (fires at 0s).",
            ""
        ),
        new Option<string>(
            "TynTaunter2",
            "Tyndarius — Taunter 2",
            "Class name of Tyndarius Taunter 2 (fires at 6s).",
            ""
        ),

        // ── General ───────────────────────────────────────────────────────────
        new Option<bool>("DoEnh", "Do Enhancements", "Enhance gear once at the start.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string s) => (s ?? "").Trim().ToLower();
    private bool HasClass(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        NormalizeString(Bot.Player.CurrentClass?.Name) == NormalizeString(name);

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions))
            Bot.Config.Configure();

        C.SetOptions();
        Core.Boot();

        // Enhance once at the very start
        if (Bot.Config!.Get<bool>("DoEnh"))
            Enh.Apply();

        // Wait for player data to be fully loaded before resolving roles
        Bot.Wait.ForTrue(() => !string.IsNullOrWhiteSpace(Bot.Player.CurrentClass?.Name), 20);

        // Resolve roles from options (done once, class doesn't change)
        string wt1  = (Bot.Config!.Get<string>("WardenTaunter1") ?? "").Trim();
        string wt2  = (Bot.Config!.Get<string>("WardenTaunter2") ?? "").Trim();
        string tb1  = (Bot.Config!.Get<string>("TynBall1Taunter") ?? "").Trim();
        string tb2  = (Bot.Config!.Get<string>("TynBall2Killer") ?? "").Trim();
        string tyn1 = (Bot.Config!.Get<string>("TynTaunter1") ?? "").Trim();
        string tyn2 = (Bot.Config!.Get<string>("TynTaunter2") ?? "").Trim();

        string cn = Bot.Player.CurrentClass?.Name ?? string.Empty;

        // Warden offset
        if (!string.IsNullOrEmpty(wt1) && cn.Equals(wt1, StringComparison.OrdinalIgnoreCase))
            wardenTauntOffset = 0;
        else if (!string.IsNullOrEmpty(wt2) && cn.Equals(wt2, StringComparison.OrdinalIgnoreCase))
            wardenTauntOffset = 6;

        // Tyndarius ball roles
        isBall1Taunter = HasClass(tb1);
        isBall2Killer  = HasClass(tb2);

        // Tyndarius taunt offset
        if (!string.IsNullOrEmpty(tyn1) && cn.Equals(tyn1, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 0;
        else if (!string.IsNullOrEmpty(tyn2) && cn.Equals(tyn2, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 6;

        bool isWardenTaunter = HasClass(wt1) || HasClass(wt2);
        bool isTynTaunter    = HasClass(tyn1) || HasClass(tyn2);

        C.Logger($"Current class: '{cn}'");
        C.Logger($"Roles — WardenTaunt: {isWardenTaunter} (offset {wardenTauntOffset}s) | Ball1: {isBall1Taunter} | Ball2Kill: {isBall2Killer} | TynTaunt: {isTynTaunter} (offset {tynTauntOffset}s)");

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");

        string progressPath = Ultra.ResolveSyncPath("daily_progress.sync");

        // ── Boss loop ─────────────────────────────────────────────────────────
        while (!Bot.ShouldExit)
        {
            // Clear progress file at the start of each full run
            Ultra.ClearSyncFile(progressPath);
            Bot.Sleep(1000);

            // Boss order: 0=Ezrajal, 1=Warden, 2=Engineer, 3=Tyndarius
            // Each client writes which bosses it has completed.
            // Everyone joins the lowest boss that not all 4 have completed.

            RunBossSequence(potionQuant, isWardenTaunter, isTynTaunter, progressPath);

            if (Bot.ShouldExit) break;

            C.Logger("All 4 dailies complete for all accounts — heading to house.");
            Ultra.JoinHouse();
            break;
        }

        C.SetOptions(false);
        Bot.StopSync();
    }

    // ── Boss sequence with army sync ──────────────────────────────────────────
    void RunBossSequence(int potionQuant, bool isWardenTaunter, bool isTynTaunter, string progressPath)
    {
        // Boss indices: 0=Ezrajal, 1=Warden, 2=Engineer, 3=Tyndarius
        string me = Bot.Player.Username ?? "Unknown";
        bool[] myDone = new bool[4]
        {
            Bot.Quests.IsDailyComplete(8152), // Ezrajal
            Bot.Quests.IsDailyComplete(8153), // Warden
            Bot.Quests.IsDailyComplete(8154), // Engineer
            Bot.Quests.IsDailyComplete(8245), // Tyndarius
        };
        C.Logger($"Daily status — Ez: {myDone[0]} | Warden: {myDone[1]} | Eng: {myDone[2]} | Tyn: {myDone[3]}");

        while (!Bot.ShouldExit)
        {
            // Write my current progress
            WriteProgress(progressPath, me, myDone);

            // Find the lowest boss index not yet completed by all 4 accounts
            int target = GetTargetBoss(progressPath, myDone);

            if (target == -1)
            {
                C.Logger("All bosses complete for all accounts.");
                return;
            }

            C.Logger($"Army target boss: {BossName(target)} (index {target})");

            // Go to whitemap between bosses, restock potions
            Core.Join("whitemap");
            Bot.Wait.ForMapLoad("whitemap");
            Bot.Sleep(1500);

            bool skipThird = target == 1 ? isWardenTaunter : target == 3 ? isTynTaunter : false;
            Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird);

            // Fight the target boss
            switch (target)
            {
                case 0: FightEzrajal(potionQuant);              break;
                case 1: FightWarden(isWardenTaunter, potionQuant); break;
                case 2: FightEngineer(potionQuant);             break;
                case 3: FightTyndarius(potionQuant);            break;
            }

            if (Bot.ShouldExit) return;

            // Mark this boss as done for me
            myDone[target] = true;
            WriteProgress(progressPath, me, myDone);
        }
    }

    string BossName(int index) => index switch
    {
        0 => "Ezrajal",
        1 => "Warden",
        2 => "Engineer",
        3 => "Tyndarius",
        _ => "Unknown"
    };

    void WriteProgress(string path, string username, bool[] done)
    {
        // Format: username:0101  (1=done, 0=not done, one char per boss)
        string payload = string.Concat(done.Select(d => d ? "1" : "0"));
        Ultra.UpdateEntry(path, username, payload);
    }

    int GetTargetBoss(string path, bool[] myDone)
    {
        string[] lines = Ultra.ReadLines(path);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int stale = 600;

        // For each boss index, check if all active accounts have completed it
        for (int boss = 0; boss < 4; boss++)
        {
            // If I haven't done it, this is a candidate
            if (!myDone[boss])
                return boss;

            // I've done it — check if everyone else has too
            bool allDone = true;
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                // Format: username:payload:timestamp
                if (parts.Length < 3) continue;
                if (!long.TryParse(parts[parts.Length - 1], out long ts)) continue;
                if (now - ts > stale) continue;

                string payload = parts[1];
                if (payload.Length <= boss) { allDone = false; break; }
                if (payload[boss] != '1') { allDone = false; break; }
            }

            if (!allDone)
                return boss; // someone else hasn't done this boss yet — go help
        }

        return -1; // all bosses done by everyone
    }

    // ── Between bosses ────────────────────────────────────────────────────────
    void BetweenBosses(int potionQuant, bool skipThird)
    {
        Core.Join("whitemap");
        Bot.Wait.ForMapLoad("whitemap");
        Bot.Sleep(1500);
        Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird);
    }

    // ── Ezrajal ───────────────────────────────────────────────────────────────
    void FightEzrajal(int potionQuant)
    {
        const string map  = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

        Pots.UseRecommendedPotions(potionQuant);

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        Bot.UltraBossHelper.EnableCounterAttack();
        if (!Bot.Quests.IsDailyComplete(8152))
            C.EnsureAccept(8152);
        C.AddDrop("Ezrajal Insignia");
        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_ezrajal.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Ezrajal Defeated", 1), syncPath))
            {
                C.Logger("Ezrajal — all players done.");
                C.EnsureComplete(8152);
                Bot.UltraBossHelper.DisableCounterAttack();
                Core.DisableSkills();
                Core.Join("whitemap");
                break;
            }

            if (Bot.Player.HasTarget
                && Bot.Target?.Auras?.Any(a => a != null && a.Name == "Counter Attack") == true)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Sleep(6300);
            }
            else
            {
                Bot.Combat.Attack(boss);
            }

            Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }

    // ── Warden ────────────────────────────────────────────────────────────────
    void FightWarden(bool isWardenTaunter, int potionQuant)
    {
        const string map  = "ultrawarden";
        const string boss = "Ultra Warden";

        if (isWardenTaunter)
            Ultra.GetScrollOfEnrage();

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        if (!Bot.Quests.IsDailyComplete(8153))
            C.EnsureAccept(8153);
        C.AddDrop("Warden Insignia");
        Core.Join(map);
        int armySizeW = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySizeW - 1, "ultra_warden.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        wardenFightStartTime = DateTime.Now;
        C.Logger($"Warden fight start: {wardenFightStartTime:HH:mm:ss.fff}");

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Warden Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("Warden — all players done.");
                C.EnsureComplete(8153);
                Core.DisableSkills();
                Core.Join("whitemap");
                break;
            }

            Bot.Combat.Attack("*");

            if (isWardenTaunter && Bot.Player.HasTarget)
            {
                TimeSpan elapsed = DateTime.Now - wardenFightStartTime;
                double t = elapsed.TotalSeconds;
                double cycle = (t - wardenTauntOffset) % WardenTauntInterval;
                bool inWindow = cycle >= 0 && cycle < WardenTauntWindow;
                bool cooled = (DateTime.Now - wardenLastTaunt).TotalSeconds >= WardenTauntInterval - 1;

                if (inWindow && cooled)
                {
                    wardenLastTaunt = DateTime.Now;
                    Bot.Combat.Attack(boss);
                    C.Logger($"Warden taunt ({t:F1}s, offset {wardenTauntOffset}s)");

                    for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        if (!Bot.Player.HasTarget) Bot.Combat.Attack(boss);
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(25);
                    }

                    C.Logger("Warden taunt done.");
                    Bot.Sleep(300);
                }
            }

            Pots.ActivateEquippedPotion();
            Bot.Sleep(250);
        }
    }

    // ── Engineer ──────────────────────────────────────────────────────────────
    void FightEngineer(int potionQuant)
    {
        const string map       = "ultraengineer";
        const string boss      = "Ultra Engineer";
        const string priority1 = "Defense Drone";
        const string priority2 = "Attack Drone";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        if (!Bot.Quests.IsDailyComplete(8154))
            C.EnsureAccept(8154);
        C.AddDrop("Engineer Insignia");
        Core.Join(map);
        int armySizeE = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySizeE - 1, "ultra_engineer.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Engineer Defeated", 1), syncPath))
            {
                C.Logger("Engineer — all players done.");
                C.EnsureComplete(8154);
                Core.DisableSkills();
                Core.Join("whitemap");
                break;
            }

            Ultra.KillWithPriority(boss, 3, priority1, 2, priority2, 1);
            Pots.ActivateEquippedPotion();
        }
    }

    // ── Avatar Tyndarius ──────────────────────────────────────────────────────
    void FightTyndarius(int potionQuant)
    {
        const string map  = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        string tyn1 = (Bot.Config!.Get<string>("TynTaunter1") ?? "").Trim();
        string tyn2 = (Bot.Config!.Get<string>("TynTaunter2") ?? "").Trim();
        string cn   = Bot.Player.CurrentClass?.Name ?? string.Empty;
        bool isTynTaunter = cn.Equals(tyn1, StringComparison.OrdinalIgnoreCase) || cn.Equals(tyn2, StringComparison.OrdinalIgnoreCase);

        if (isBall1Taunter || isTynTaunter)
            Ultra.GetScrollOfEnrage();

        // Reset Tyndarius fight timer for this run
        tynFightStartTime = DateTime.MinValue;

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.AddDrop("Avatar Tyndarius Insignia");
        if (!Bot.Quests.IsDailyComplete(8245))
            C.EnsureAccept(8245);
        Core.Join(map);
        int armySizeT = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySizeT - 1, "ultra_tyndarius.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Avatar Tyndarius Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("Tyndarius — all players done.");
                C.EnsureComplete(8245);
                Core.DisableSkills();
                Core.Join("whitemap");
                break;
            }

            if (Bot.Map.Name != map)
                Core.Join(map);

            if (Bot.Player.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
            }

            bool ball1Alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 1);
            bool ball2Alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 3);
            bool bothDead   = !ball1Alive && !ball2Alive;

            // ── Ball phase ────────────────────────────────────────────────────
            if (isBall1Taunter)
            {
                if (!bothDead)
                {
                    if (ball1Alive)
                    {
                        Bot.Combat.Attack(1);
                        for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive) break;
                            if (!Bot.Player.HasTarget) Bot.Combat.Attack(1);
                            Bot.Skills.UseSkill(5);
                            Bot.Sleep(25);
                        }
                    }
                    else if (ball2Alive)
                    {
                        Bot.Combat.Attack(3);
                        Bot.Sleep(500);
                    }
                    Pots.ActivateEquippedPotion();
                    if (Bot.ShouldExit) C.Jump("Enter", "Spawn");
                    continue;
                }
            }
            else if (isBall2Killer)
            {
                if (!bothDead)
                {
                    if (ball2Alive)
                    {
                        Bot.Combat.Attack(3);
                        for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive) break;
                            if (!Bot.Player.HasTarget) Bot.Combat.Attack(3);
                            Bot.Skills.UseSkill(5);
                            Bot.Sleep(25);
                        }
                    }
                    else if (ball1Alive)
                    {
                        Bot.Combat.Attack(1);
                        Bot.Sleep(500);
                    }
                    Pots.ActivateEquippedPotion();
                    if (Bot.ShouldExit) C.Jump("Enter", "Spawn");
                    continue;
                }
            }

            // ── Tyndarius phase (balls dead) ──────────────────────────────────
            if (tynFightStartTime == DateTime.MinValue)
            {
                tynFightStartTime = DateTime.Now;
                C.Logger("Balls down — Tyndarius timer started.");
            }

            Bot.Combat.Attack(2);

            if (isTynTaunter && Bot.Player.HasTarget)
            {
                TimeSpan elapsed = DateTime.Now - tynFightStartTime;
                double t = elapsed.TotalSeconds;
                double cycle = (t - tynTauntOffset) % TynTauntInterval;
                bool inWindow = cycle >= 0 && cycle < TynTauntWindow;
                bool cooled = (DateTime.Now - tynLastTaunt).TotalSeconds >= TynTauntInterval - 1;

                if (inWindow && cooled)
                {
                    tynLastTaunt = DateTime.Now;
                    C.Logger($"Tyndarius taunt ({t:F1}s, offset {tynTauntOffset}s)");

                    for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        if (!Bot.Player.HasTarget) Bot.Combat.Attack(2);
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(25);
                    }

                    C.Logger("Tyndarius taunt done.");
                    Bot.Sleep(300);
                }
                else if (ball2Alive)
                {
                    // Outside taunt window — help kill Ball 2
                    Bot.Combat.Attack(3);
                    Bot.Sleep(500);
                }
                else if (ball1Alive)
                {
                    // Ball 2 dead — help kill Ball 1
                    Bot.Combat.Attack(1);
                    Bot.Sleep(500);
                }
                else
                {
                    Bot.Sleep(500);
                }
            }
            else
            {
                Bot.Sleep(500);
            }

            Pots.ActivateEquippedPotion();

            if (Bot.ShouldExit)
                C.Jump("Enter", "Spawn");
        }
    }
}
