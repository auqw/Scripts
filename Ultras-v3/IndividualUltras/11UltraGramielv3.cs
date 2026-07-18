/*
name: UltraGramiel_v3
description: Ultra Gramiel v3 — two-phase fight (crystals then Gramiel) with chat-driven crystal taunt warnings and pulse-driven taunt rotation.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraCustomClassSync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/GetScrolls.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraAsync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraDeath.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/PrerequisitesChecker.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class UltraGramiel_v3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots2 C => CoreBots2.Instance;
    private static CoreEngine2 Engine => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;
    private static UltraDeath Death => _Death ??= new UltraDeath();
    private static UltraDeath _Death;

    // Crystal taunter roles — 2 groups (T1 and T2) across 2 crystals (left and right)
    private const string LeftCrystalT1 = "StoneCrusher";
    private const string RightCrystalT1 = "ArchPaladin";
    private const string LeftCrystalT2 = "Lord Of Order";
    private const string RightCrystalT2 = "ArchFiend";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { LeftCrystalT1 },
        new[] { RightCrystalT1 },
        new[] { LeftCrystalT2 },
        new[] { RightCrystalT2 }
    };

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private System.Threading.ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 3;
    private DateTime fightStartTime = DateTime.MinValue;
    private DateTime _gramielFightStart = DateTime.MinValue;
    private int _gramielTaunterIndex = 0;
    private bool _gramielTauntLaunched = false;
    private int crystalMapId = 2;
    private bool isT1Taunter = true;
    private bool isTaunter = false;

    // Chat-driven crystal state (Speaker-style: message → fire async)
    private int tauntCounter = 0;
    private DateTime lastCycleTime = DateTime.MinValue;
    private volatile bool _attackCrystal;



    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);

        if (!new PrerequisitesChecker().PrerequisiteSyncGate(4))
            return;

        try
        {
            while (_deathRetries.Value < MaxDeathRetries)
            {
                Engine.Boot();
                _tauntCts = new();
                _wipeCts = new();
                _retreatComplete.Reset();
                Bot.Events.ScriptStopping -= StopTauntEvent;
                Bot.Events.ScriptStopping += StopTauntEvent;

                // Start background wipe monitor (also handles individual death signaling)
                UltraDeath.StartWipeMonitor(
                    C, 4, _wipeCts, _retreatComplete,
                    () => UltraDeath.PerformRetreat(C, 4, MaxDeathRetries, _deathRetries, "UltraGramielRetreat.sync")
                );

                Bot.Events.ExtensionPacketReceived += GramielMessageListener;
                try
                {
                    Prep();
                    Fight();
                }
                finally
                {
                    Bot.Events.ExtensionPacketReceived -= GramielMessageListener;
                }
            }
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            _wipeCts.Cancel();
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopTauntEvent(Exception? e)
    {
        _tauntCts.Cancel();
        return true;
    }

    private bool IsTaunter()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        return className == LeftCrystalT1 || className == RightCrystalT1 || className == LeftCrystalT2 || className == RightCrystalT2;
    }

    private void ApplyTaunterClasses()
    {
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;

        if (className.Equals(LeftCrystalT1, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isTaunter = true;
            isT1Taunter = true;
            _gramielTaunterIndex = 0;
            C.Logger($"[UltraGramiel-v3] Assigned to taunt LEFT crystal (mapId=2) - T1 (Gramiel taunter index 0)");
        }
        else if (className.Equals(LeftCrystalT2, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isTaunter = true;
            isT1Taunter = false;
            _gramielTaunterIndex = 1;
            C.Logger($"[UltraGramiel-v3] Assigned to taunt LEFT crystal (mapId=2) - T2 (Gramiel taunter index 1)");
        }
        else if (className.Equals(RightCrystalT1, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isTaunter = true;
            isT1Taunter = true;
            _gramielTaunterIndex = 2;
            C.Logger($"[UltraGramiel-v3] Assigned to taunt RIGHT crystal (mapId=3) - T1 (Gramiel taunter index 2)");
        }
        else if (className.Equals(RightCrystalT2, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isTaunter = true;
            isT1Taunter = false;
            _gramielTaunterIndex = 3;
            C.Logger($"[UltraGramiel-v3] Assigned to taunt RIGHT crystal (mapId=3) - T2 (Gramiel taunter index 3)");
        }
        else
        {
            crystalMapId = 2;
            isTaunter = false;
            isT1Taunter = true;
            _gramielTaunterIndex = 0;
            C.Logger($"[UltraGramiel-v3] Class '{className}' not a taunter, defaulting to LEFT crystal DPS.");
        }
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[UltraGramiel-v3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "ultra_gramiel_class-v3.sync", allowDuplicates);
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);
        ApplyTaunterClasses();

        tauntCounter = 0;
        lastCycleTime = DateTime.MinValue;
        fightStartTime = DateTime.MinValue;
        _gramielFightStart = DateTime.MinValue;
        _gramielTauntLaunched = false;

        Enh.ApplyGramiel();

        C.Logger($"[UltraGramiel-v3] Class: {Bot.Player.CurrentClass?.Name ?? "None"} | IsTaunter: {IsTaunter()}");
    }

    private void Fight()
    {
        const string map = "ultragramiel";
        const string boss = "Gramiel the Graceful";
        const string bossDefeatedTemp = "Gramiel the Graceful Vanquished";

        const string waitSyncFile = "ultra_gramiel.sync";
        const string gramielFightTimeSyncFile = "UltraGramielFightTime.sync";
        const string completionSyncFile = "UltraGramielCompletion.sync";
        const string retreatSyncFile = "UltraGramielRetreat.sync";
        int armySize = 4;

        const int questId = 10301;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(gramielFightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        Pots.EnsureRecommendedPotions(skipThird: skipThird, context: "Gramiel");
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(skipThird: skipThird, ensureStock: false, context: "Gramiel");

        if (skipThird)
        {
            C.Logger("[UltraGramiel-v3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
        }

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all 4 entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        fightStartTime = DateTime.UtcNow;

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor
                // Wait for respawn and keep fighting
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(500);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Gramiel the Graceful defeated. Finishing quest.");
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                _deathRetries.Value = MaxDeathRetries;
                break;
            }

            Pots.ActivateEquippedPotion();

            // Detect phase: crystals alive → crystal phase, else Gramiel phase
            bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
                .Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

            if (anyCrystalAlive)
            {
                // === CRYSTAL PHASE (chat-driven) ===
                // Pure event-driven: GramielMessageListener fires _ = CrystalTauntAsync() on message.
                // This loop handles targeting.

                int targetCrystalMapId = GetAliveCrystalTarget(crystalMapId);

                if (targetCrystalMapId == 0)
                {
                    Bot.Combat.Attack(1);
                    Bot.Sleep(500);
                    continue;
                }

                // HP readings — needed for both lockout override and normal balancing
                int otherCrystalMapId = crystalMapId == 2 ? 3 : 2;
                int myHP = Bot.Monsters.CurrentAvailableMonsters
                    .Where(x => x != null && x.Alive && x.MapID == crystalMapId)
                    .Select(x => x.HP)
                    .FirstOrDefault();
                int otherHP = Bot.Monsters.CurrentAvailableMonsters
                    .Where(x => x != null && x.Alive && x.MapID == otherCrystalMapId)
                    .Select(x => x.HP)
                    .FirstOrDefault();

                // HP balancing: keep both crystals within 30 HP of each other.
                // When one dies too far ahead, the remaining one goes Unstable and
                // explodes after 5s, instant party wipe.
                // Strategy: if either crystal is low (< 50), everyone attacks the higher-HP one
                // to converge HP. Otherwise, use the 30-HP deadband.
                bool lowHP = (myHP > 0 && myHP < 50) || (otherHP > 0 && otherHP < 50);
                if (myHP > 0 && otherHP > 0)
                {
                    if (lowHP && otherHP > myHP)
                    {
                        // Getting low — converge on the higher-HP crystal
                        targetCrystalMapId = otherCrystalMapId;
                    }
                    else if (!lowHP && otherHP > myHP + 30)
                    {
                        // Normal deadband: switch if other is >30 ahead
                        targetCrystalMapId = otherCrystalMapId;
                    }
                }

                // When _attackCrystal is active (7s after a crystal taunt), stay on assigned crystal
                // — EXCEPT during low-HP convergence, which already set targetCrystalMapId above.
                if (_attackCrystal)
                {
                    Bot.Combat.Attack(targetCrystalMapId);
                    Bot.Sleep(500);
                    continue;
                }

                Bot.Combat.Attack(targetCrystalMapId);
                Bot.Sleep(500);
                continue;
            }

            // === GRAMIEL PHASE (all crystals dead) ===
            Bot.Combat.Attack(boss);

            if (!_gramielTauntLaunched)
            {
                _gramielTauntLaunched = true;
                C.Logger("[UltraGramiel-v3] All crystals dead. Launching Gramiel pulse taunt rotation.");

                string gramielFightTimeSyncPath = Ultra.ResolveSyncPath(gramielFightTimeSyncFile);
                if (_gramielTaunterIndex == 0)
                {
                    C.Logger("[UltraGramiel-v3] Gramiel Taunter (Primary) — setting fight start time.");
                    _gramielFightStart = UltraAsync.SetFightTime(C, gramielFightTimeSyncPath);
                    UltraAsync.StartTauntLoop(Bot, C, Engine, _gramielFightStart, 0, 4, cancellationToken: _tauntCts.Token);
                }
                else
                {
                    C.Logger($"[UltraGramiel-v3] Gramiel Taunter (Index {_gramielTaunterIndex}) — reading fight start time.");
                    _gramielFightStart = UltraAsync.GetFightTime(Ultra, C, gramielFightTimeSyncPath);
                    UltraAsync.StartTauntLoop(Bot, C, Engine, _gramielFightStart, _gramielTaunterIndex, 4, cancellationToken: _tauntCts.Token);
                }

                C.Logger($"[UltraGramiel-v3] Gramiel fight started — {Bot.Player.CurrentClass?.Name} taunting on pulse {_gramielTaunterIndex} of 4.");
            }

            Bot.Combat.Attack(1);

            Bot.Sleep(500);
        }

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }

    private int GetAliveCrystalTarget(int preferredCrystalMapId)
    {
        int otherCrystalMapId = preferredCrystalMapId == 2 ? 3 : 2;

        bool preferredAlive = Bot.Monsters.CurrentAvailableMonsters
            .Any(x => x != null && x.Alive && x.MapID == preferredCrystalMapId);
        if (preferredAlive)
            return preferredCrystalMapId;

        bool otherAlive = Bot.Monsters.CurrentAvailableMonsters
            .Any(x => x != null && x.Alive && x.MapID == otherCrystalMapId);
        return otherAlive ? otherCrystalMapId : 0;
    }

    private async Task CrystalTauntAsync(int targetMapId)
    {
        C.Logger($"[UltraGramiel-v3] CrystalTauntAsync started — 1s buffer, then 40 presses on mapId {targetMapId}.");

        await Task.Delay(1000);

        for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
        {
            if (!Bot.Player.Alive)
                break;

            // Re-acquire target if lost, dead, or targeting the wrong crystal.
            if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || Bot.Player.Target.MapID != targetMapId)
                Bot.Combat.Attack(targetMapId);

            Engine.Cast(5);
            await Task.Delay(25);
        }
    }

    private async Task AttackCrystalAsync(int targetMapId)
    {
        C.Logger($"[UltraGramiel-v3] AttackCrystalAsync started — keeping target on mapId {targetMapId} for 7s.");
        await Task.Delay(7000);
        _attackCrystal = false;
        C.Logger("[UltraGramiel-v3] AttackCrystalAsync done — returning to normal targeting.");
    }

    private void GramielMessageListener(dynamic packet)
    {
        try
        {
            string type = packet["params"].type;
            if (type is not "json")
                return;

            if (!Bot.Player.Alive)
                return;

            dynamic data = packet["params"].dataObj;
            string cmd = data.cmd.ToString();

            if (cmd != "ct")
                return;

            if (data.anims is null)
                return;

            foreach (dynamic anim in data.anims)
            {
                if (anim is null || anim.msg is null)
                    continue;

                string message = (string)anim.msg;
                if (!message.Contains("The Grace Crystal prepares a defense shattering attack!", StringComparison.OrdinalIgnoreCase))
                    continue;

                TimeSpan sinceLastCycle = DateTime.UtcNow - lastCycleTime;
                if (sinceLastCycle.TotalSeconds < 10)
                    continue;

                lastCycleTime = DateTime.UtcNow;
                tauntCounter++;

                bool currentlyT1Turn = tauntCounter % 2 == 1;
                bool shouldTauntNow = (isT1Taunter && currentlyT1Turn) || (!isT1Taunter && !currentlyT1Turn);

                C.Logger($"[UltraGramiel-v3] Crystal warning #{tauntCounter}, turn={(currentlyT1Turn ? "T1" : "T2")}, shouldTauntNow={shouldTauntNow}");

                if (shouldTauntNow)
                {
                    int targetCrystal = GetAliveCrystalTarget(crystalMapId);
                    if (targetCrystal != 0)
                    {
                        _attackCrystal = true;
                        C.Logger($"[UltraGramiel-v3] Firing CrystalTauntAsync + AttackCrystalAsync on mapId {targetCrystal}.");
                        _ = CrystalTauntAsync(targetCrystal);
                        _ = AttackCrystalAsync(targetCrystal);
                    }
                }
            }
        }
        catch { }
    }
}
