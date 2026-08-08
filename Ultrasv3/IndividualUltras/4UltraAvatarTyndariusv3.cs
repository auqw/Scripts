/*
name: UltraAvatarTyndariusv3
description: Ultra Avatar Tyndarius v3 — Ball1TaunterAttackBall2 + Ball2TaunterAttackBall2 + Ball2Attacker1 + Ball2Attacker2 with pulse-driven targeting.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraDeath.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.Linq;
using Skua.Core.Interfaces;

public class UltraAvatarTyndariusv3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;
    private static UltraDeath Death => _Death ??= new UltraDeath();
    private static UltraDeath _Death;

    private const string Ball1TaunterAttackBall2 = "Verus DoomKnight";
    private const string Ball2TaunterAttackBall2 = "Lord of Order";
    private const string Ball2Attacker1 = "StoneCrusher";
    private const string Ball2Attacker2 = "King's Echo";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Ball1TaunterAttackBall2 },
        new[] { Ball2TaunterAttackBall2 },
        new[] { Ball2Attacker1 },
        new[] { Ball2Attacker2 }
    };

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private System.Threading.ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 3;
    private DateTime fightStartTime = DateTime.MinValue;
    private string _role = "";

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);

        try
        {
            while (_deathRetries.Value < MaxDeathRetries)
            {
                Engine.Boot();
                _tauntCts?.Cancel();
                _tauntCts = new();
                _wipeCts = new();
                _retreatComplete.Reset();
                Bot.Events.ScriptStopping -= StopTauntEvent;
                Bot.Events.ScriptStopping += StopTauntEvent;

                // Start background wipe monitor (also handles individual death signaling)
                UltraDeath.StartWipeMonitor(
                    C, 4, _wipeCts, _retreatComplete,
                    () => UltraDeath.PerformRetreat(C, 4, MaxDeathRetries, _deathRetries, "UltraAvatarTyndariusRetreat.sync")
                );

                Prep();
                Fight();
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



    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[UltraAvatarTyndarius-v3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "ultra_tyndarius_class-v3.sync", allowDuplicates);
    }

    private bool IsTaunter() => _role == "Ball1TaunterAttackBall2" || _role == "Ball2TaunterAttackBall2";

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Ball1TaunterAttackBall2) _role = "Ball1TaunterAttackBall2";
        else if (className == Ball2TaunterAttackBall2) _role = "Ball2TaunterAttackBall2";
        else if (className == Ball2Attacker1) _role = "Ball2Attacker1";
        else _role = "Ball2Attacker2";

        Enh.ApplyTyndarius();

        C.Logger($"[UltraAvatarTyndarius-v3] Role: {_role} ({className})");
    }

    private void Fight()
    {
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";
        const string bossDefeatedTemp = "Ultra Avatar Tyndarius Defeated";

        const string waitSyncFile = "ultra_tyndarius.sync";
        const string fightTimeSyncFile = "UltraAvatarTyndariusFightTime.sync";
        const string completionSyncFile = "UltraAvatarTyndariusCompletion.sync";
        int armySize = 4;

        const int questId = 8245;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(fightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        Pots.EnsureRecommendedPotions(skipThird: skipThird);
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(skipThird: skipThird, ensureStock: false);

        if (skipThird)
        {
            C.Logger("[UltraAvatarTyndarius-v3] Taunter detected, equipping Scroll of Enrage.");
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

        string fightTimeSyncPath = Ultra.ResolveSyncPath(fightTimeSyncFile);

        // Set or retrieve fight start time, then launch the appropriate taunter loop
        if (_role == "Ball1TaunterAttackBall2")
        {
            C.Logger("[UltraAvatarTyndarius-v3] Ball1TaunterAttackBall2 (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 2, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Ball2TaunterAttackBall2")
        {
            C.Logger("[UltraAvatarTyndarius-v3] Ball2TaunterAttackBall2 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, 2, cancellationToken: _tauntCts.Token);
        }

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor
                // Wait for respawn and keep fighting
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Ultra Avatar Tyndarius defeated. Finishing quest.");
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

            // All roles: ball2 (MapID 3) → ball1 (MapID 1) → Tyndarius (MapID 2)
            if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == 3 && x.HP > 0))
            {
                if (Bot.Player.Target?.MapID != 3)
                    Bot.Combat.Attack(3);
            }
            else if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
            {
                if (Bot.Player.Target?.MapID != 1)
                    Bot.Combat.Attack(1);
            }
            else if (Bot.Player.Target?.MapID != 2)
            {
                Bot.Combat.Attack(2);
            }

            Pots.ActivateEquippedPotion();

            Bot.Sleep(500);
        }

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }

}
