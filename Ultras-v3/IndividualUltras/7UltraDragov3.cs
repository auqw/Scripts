/*
name: UltraDrago_v3
description: Ultra King Drago v3 — Taunter1AttackRightSummon1 + Taunter2AttackRightSummon2 + DPSAttackRightSummon1 + DPSAttackRightSummon2.
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
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/PrerequisitesChecker.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class UltraDrago_v3
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

    private const string Taunter1AttackRightSummon1 = "Lord Of Order";
    private const string Taunter2AttackRightSummon2 = "Verus DoomKnight";
    private const string DPSAttackRightSummon1 = "StoneCrusher";
    private const string DPSAttackRightSummon2 = "King's Echo";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Taunter1AttackRightSummon1 },
        new[] { Taunter2AttackRightSummon2 },
        new[] { DPSAttackRightSummon1 },
        new[] { DPSAttackRightSummon2 }
    };

    private CancellationTokenSource _tauntCts = new();
    private DateTime fightStartTime = DateTime.MinValue;
    private string _role = "";

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);
        Engine.Boot();
        _tauntCts = new();
        Bot.Events.ScriptStopping -= StopTauntEvent;
        Bot.Events.ScriptStopping += StopTauntEvent;

        if (!new PrerequisitesChecker().PrerequisiteSyncGate(4))
            return;

        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
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
        return className == Taunter1AttackRightSummon1 || className == Taunter2AttackRightSummon2;
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[UltraDrago-v3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "ultra_drago_class-v3.sync", allowDuplicates);
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Taunter1AttackRightSummon1) _role = "Taunter1AttackRightSummon1";
        else if (className == Taunter2AttackRightSummon2) _role = "Taunter2AttackRightSummon2";
        else if (className == DPSAttackRightSummon1) _role = "DPSAttackRightSummon1";
        else _role = "DPSAttackRightSummon2";

        Enh.Apply();

        C.Logger($"[UltraDrago-v3] Role: {_role} ({className})");
    }

    private void Fight()
    {
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string bossDefeatedTemp = "Drago Dethroned";
        const string leftSummon = "Executioner Dene";
        const string rightSummon = "Bowmaster Algie";

        const string waitSyncFile = "ultra_drago.sync";
        const string fightTimeSyncFile = "UltraDragoFightTime.sync";
        const string completionSyncFile = "UltraDragoCompletion.sync";
        int armySize = 4;

        const int questId = 8397;
        

        if (!Bot.Quests.IsUnlocked(questId))
            Bot.Quests.UpdateQuest(8395);

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
            C.Logger("[UltraDrago-v3] Taunter detected, equipping Scroll of Enrage.");
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
        if (_role == "Taunter1AttackRightSummon1")
        {
            C.Logger("[UltraDrago-v3] Taunter1AttackRightSummon1 (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Taunter2AttackRightSummon2")
        {
            C.Logger("[UltraDrago-v3] Taunter2AttackRightSummon2 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, cancellationToken: _tauntCts.Token);
        }

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("King Drago defeated. Finishing quest.");
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            if (IsTaunter())
            {
                // Taunters — attack rightSummon normally;
                // 1s before taunt pulse, switch to leftSummon to taunt it (like Taunter3AttackBlade)
                double elapsed = (DateTime.UtcNow - fightStartTime).TotalSeconds;
                double timeInCycle = elapsed % 10;

                bool targetLeftSummon;
                if (_role == "Taunter1AttackRightSummon1")
                    // index 0: taunts at t=0,10,20... pre-switch at 9s, stay until 3s
                    targetLeftSummon = timeInCycle >= 9 || timeInCycle <= 3;
                else
                    // index 1: taunts at t=5,15,25... pre-switch at 4s, stay until 8s
                    targetLeftSummon = timeInCycle >= 4 && timeInCycle <= 8;

                if (targetLeftSummon)
                {
                    // Target leftSummon for taunt
                    if (Ultra.MonsterAlive(leftSummon))
                    {
                        if (Bot.Player.Target?.Name != leftSummon)
                            Bot.Combat.Attack(leftSummon);
                    }
                    else if (Ultra.MonsterAlive(rightSummon))
                    {
                        if (Bot.Player.Target?.Name != rightSummon)
                            Bot.Combat.Attack(rightSummon);
                    }
                    else
                    {
                        if (Bot.Player.Target?.Name != boss)
                            Bot.Combat.Attack(boss);
                    }
                }
                else
                {
                    // Attack rightSummon primarily
                    if (Ultra.MonsterAlive(rightSummon))
                    {
                        if (Bot.Player.Target?.Name != rightSummon)
                            Bot.Combat.Attack(rightSummon);
                    }
                    else if (Ultra.MonsterAlive(leftSummon))
                    {
                        if (Bot.Player.Target?.Name != leftSummon)
                            Bot.Combat.Attack(leftSummon);
                    }
                    else
                    {
                        if (Bot.Player.Target?.Name != boss)
                            Bot.Combat.Attack(boss);
                    }
                }
            }
            else
            {
                if (Ultra.MonsterAlive(rightSummon))
                {
                    if (Bot.Player.Target?.Name != rightSummon)
                        Bot.Combat.Attack(rightSummon);
                }
                else if (Ultra.MonsterAlive(leftSummon))
                {
                    if (Bot.Player.Target?.Name != leftSummon)
                        Bot.Combat.Attack(leftSummon);
                }
                else
                {
                    if (Bot.Player.Target?.Name != boss)
                        Bot.Combat.Attack(boss);
                }
            }
            Pots.ActivateEquippedPotion();

            Bot.Sleep(500);
        }
    }

}
