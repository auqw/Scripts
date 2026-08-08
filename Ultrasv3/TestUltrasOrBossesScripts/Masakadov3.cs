/*
name: Masakadov3
description: Challenge Boss Template v3 — single-taunter fight flow with synced class equip. Copy and adapt for specific bosses.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCounterAttack.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.IO;
using System.Threading;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Masakadov3
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
    private static string _fbsMuteFile = "";

    bool usePotions;
    private CancellationTokenSource _tauntCts = new();
    private string _role = "";
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Masakadov3";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("TauntClass1", "Taunt Class 1", "First taunter class. Leave empty for no taunter.", "ArchPaladin"),
        new Option<string>("TauntClass2", "Taunt Class 2", "Second taunter class. Leave empty for single taunter.", "Lord of Order"),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "King's Echo"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }
        Engine.Boot();

        UltraCounterAttack.Enable();

        try
        {
            _tauntCts?.Cancel();
            _tauntCts = new();
            Bot.Events.ScriptStopping -= StopTauntEvent;
            Bot.Events.ScriptStopping += StopTauntEvent;

            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            UltraCounterAttack.Disable();
            _tauntCts.Cancel();
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "masakadov3_class.sync");
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        usePotions = Bot.Config!.Get<bool>("UsePotions");

        string? className = Bot.Player.CurrentClass?.Name;
        string? tc1 = Bot.Config!.Get<string>("TauntClass1");
        string? tc2 = Bot.Config!.Get<string>("TauntClass2");
        bool hasTwoTaunters = !string.IsNullOrWhiteSpace(tc1) && !string.IsNullOrWhiteSpace(tc2);

        if (hasTwoTaunters)
        {
            if (className == tc1) _role = "Taunter1";
            else if (className == tc2) _role = "Taunter2";
            else _role = "Dps";
        }
        else
        {
            if (className == tc1) _role = "Taunter";
            else _role = "Dps";
        }
        C.Logger($"[Masakadov3] Role: {_role} ({className})");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");

        Bot.Sleep(2500);
    }

    void DoEnhs() => Enh.Apply();

    private bool IsTaunter()
    {
        return _role is "Taunter" or "Taunter1" or "Taunter2";
    }

    private bool StopTauntEvent(Exception? e)
    {
        _tauntCts.Cancel();
        return true;
    }

    private void Fight()
    {
        const string map = "victormatsuri";
        const string boss = "Masakado";
        const string bossDefeatedTemp = "Agehachou Crest";

        const string waitSyncFile = "masakadov3_ready.sync";
        const string fightTimeSyncFile = "masakadov3_fighttime.sync";
        const string completionSyncFile = "masakadov3_completion.sync";
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));

        
        const int questId = 10295;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(fightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: skipThird);

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird, ensureStock: false);

        if (skipThird)
        {
            C.Logger("[Masakadov3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
        }

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        string fightTimeSyncPath = Ultra.ResolveSyncPath(fightTimeSyncFile);
        bool hasTwoTaunters = !string.IsNullOrWhiteSpace(Bot.Config!.Get<string>("TauntClass2"));

        if (hasTwoTaunters)
        {
            if (_role == "Taunter1")
            {
                C.Logger("[Masakadov3] Taunter1 (Primary) — setting fight start time.");
                DateTime fightStart = UltraAsync.SetFightTime(C, fightTimeSyncPath);
                UltraAsync.StartTauntLoop(Bot, C, Engine, fightStart, 0, 2, cancellationToken: _tauntCts.Token);
            }
            else if (_role == "Taunter2")
            {
                C.Logger("[Masakadov3] Taunter2 (Secondary) — reading fight start time.");
                DateTime fightStart = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
                UltraAsync.StartTauntLoop(Bot, C, Engine, fightStart, 1, 2, cancellationToken: _tauntCts.Token);
            }
        }
        else if (_role == "Taunter")
        {
            C.Logger("[Masakadov3] Taunter detected — starting taunt loop.");
            UltraAsync.StartTauntLoop(Bot, C, Engine, DateTime.UtcNow, 0, 1, cancellationToken: _tauntCts.Token);
        }

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
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Boss defeated. Finishing quest.");
                Bot.Events.ScriptStopping -= StopTauntEvent;
                UltraCounterAttack.Disable();
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            Bot.Combat.Attack(boss);

            if (usePotions)
                Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
