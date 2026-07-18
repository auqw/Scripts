/*
name: AstralEmpyreanv3 (No Option)
description: Three-taunter strategy for Astral Empyrean with aura-based taunting and army synchronization. No config needed.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraCustomClassSync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraAsync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/GetScrolls.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraDeath.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class AstralEmpyreanv3
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

    private const string Taunter1 = "Verus DoomKnight";
    private const string Taunter2 = "Lord Of Order";
    private const string Taunter3 = "ArchPaladin";
    private const string Dps1 = "StoneCrusher";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Taunter1 },
        new[] { Taunter2 },
        new[] { Taunter3 },
        new[] { Dps1 }
    };

    private CancellationTokenSource _tauntCts = new();
    private DateTime fightStartTime = DateTime.MinValue;
    private string _role = "";
    private int _deathRetries = 0;
    private const int MaxDeathRetries = 3;

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);

        if (!Bot.Quests.IsUnlocked(9803))
        {
            C.Logger("Quest not unlocked: Asterism's Toll, we'll continue anyway");
            Bot.Quests.UpdateQuest(9803);
        }

        try
        {
            while (_deathRetries < MaxDeathRetries)
            {
                Engine.Boot();
                _tauntCts = new();
                Bot.Events.ScriptStopping -= StopTauntEvent;
                Bot.Events.ScriptStopping += StopTauntEvent;

                Prep();
                Fight();
            }
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[AstralEmpyreanv3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "astralempyreanv3_class-v3.sync", allowDuplicates);
    }

    private bool StopTauntEvent(Exception? e)
    {
        _tauntCts.Cancel();
        return true;
    }

    private bool IsTaunter() => _role == "Taunter1" || _role == "Taunter2" || _role == "Taunter3";

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Taunter1) _role = "Taunter1";
        else if (className == Taunter2) _role = "Taunter2";
        else if (className == Taunter3) _role = "Taunter3";
        else _role = "Dps1";
        C.Logger($"[AstralEmpyreanv3] Role: {_role} ({className})");

        Enh.ApplyAstralEmpyrean();

        Bot.Quests.UpdateQuest(9802);
        Bot.Sleep(2500);
    }

    private void Fight()
    {
        const string map = "astralshrine";
        const string boss = "Astral Empyrean";
        const string bossDefeatedTemp = "Astral's Supernova";

        const string waitSyncFile = "AstralEmpyreanv3.sync";
        const string completionSyncFile = "AstralEmpyreanv3Completion.sync";
        const string retreatSyncFile = "AstralEmpyreanv3Retreat.sync";
        int armySize = 4;

        const int questId = 9803;

        C.AddDrop("Star of the Empyrean");
        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        Pots.EnsureRecommendedPotions(30, skipThird: skipThird, context: "AstralEmpyrean");
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(30, skipThird: skipThird, ensureStock: false, context: "AstralEmpyrean");

        if (skipThird)
        {
            C.Logger("[AstralEmpyreanv3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
        }

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        string fightTimeSyncPath = Ultra.ResolveSyncPath("AstralEmpyreanFightTime.sync");
        if (_role == "Taunter1")
        {
            C.Logger("[AstralEmpyreanv3] Taunter1 (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 3, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Taunter2")
        {
            C.Logger("[AstralEmpyreanv3] Taunter2 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, 3, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Taunter3")
        {
            C.Logger("[AstralEmpyreanv3] Taunter3 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 2, 3, cancellationToken: _tauntCts.Token);
        }

        Bot.Events.ExtensionPacketReceived += AstralZoneListener;
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all 4 entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        while (!Bot.ShouldExit)
        {
            // Check if any alive army member detected a death
            if (Bot.Player.Alive && Death.HasDeathOccurred())
            {
                _deathRetries++;
                C.Logger($"[AstralEmpyreanv3] Army death detected. Retreating. Retry {_deathRetries}/{MaxDeathRetries}.");

                Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();

                Engine.Join(map);
                Ultra.PersistentJoinHouse();

                if (_deathRetries >= MaxDeathRetries)
                {
                    C.Logger($"{MaxDeathRetries} Retries, Stopping the scripts.", messageBox: true, stopBot: true);
                    break;
                }

                Death.ClearDeaths();
                UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, retreatSyncFile, useSkill: false);
                C.Logger("[AstralEmpyreanv3] All retreated. Restarting fight.");
                return;
            }

            if (!Bot.Player.Alive)
            {
                _deathRetries++;
                C.Logger($"[AstralEmpyreanv3] Death detected. Retry {_deathRetries}/{MaxDeathRetries}.");

                Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();

                Death.SignalDeath();
                Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));
                Ultra.ClearSyncFile(Ultra.ResolveSyncPath(retreatSyncFile));

                Engine.Join(map);
                Ultra.PersistentJoinHouse();

                if (_deathRetries >= MaxDeathRetries)
                {
                    C.Logger($"{MaxDeathRetries} Retries, Stopping the scripts.", messageBox: true, stopBot: true);
                    break;
                }

                Ultra.ClearSyncFile(Ultra.ResolveSyncPath("ultra_death.sync"));
                Death.ClearDeaths();
                UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, retreatSyncFile, useSkill: false);
                C.Logger("[AstralEmpyreanv3] All retreated. Restarting fight.");
                return;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Boss defeated. Finishing quest.");
                Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                _deathRetries = MaxDeathRetries;
                break;
            }

            // Default — attack the boss
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }

    public async void AstralZoneListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;
        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return;

        int x = 0, y = 0;

        switch (zoneSet.ToUpper())
        {
            case "B": // Red on bottom - GO UP — center of safe box
                x = 240;
                y = 200;
                break;
            case "A": // Red on top - GO DOWN — center of safe box
                x = 600;
                y = 429;
                break;
            default:
                return;
        }

        _ = Task.Run(() => Bot.Player.WalkTo(x, y));
    }
}
