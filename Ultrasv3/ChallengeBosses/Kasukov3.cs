/*
name: Kasukov3
description: Kasuko challenge boss v3 — 2 taunters taunt Kasuko and attack Whirlpool. Army sync with taunt loop via UltraAsync.
tags: kasuko, challenge boss
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Kasukov3
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
    private static string _fbsMuteFile = "";

    // ── Boss & Map Data ──────────────────────────────────────────
    // Monsters on lavarockshore (2 total):
    // "Whirlpool" [MapID: 1, HP: 1,000,000] — add / priority target for taunters
    // "Kasuko"    [MapID: 2, HP: 5,000,000] — main boss
    private const int WhirlpoolMapID = 1;
    private const int KasukoMapID = 2;

    // ── Taunter Role Names ──────────────────────────────────────
    private const string Taunter1AttackWhirlpool1 = "Taunter1AttackWhirlpool1";
    private const string Taunter2AttackWhirlpool2 = "Taunter2AttackWhirlpool2";

    bool usePotions;
    private CancellationTokenSource _tauntCts = new();
    private DateTime fightStartTime = DateTime.MinValue;
    private string _role = "";

    public bool DontPreconfigure = true;
    public string OptionsStorage = "Kasukov3";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("Taunter1Class", "Taunter 1 Class (Primary)", "Class name for Taunter1 — taunts Kasuko and attacks Whirlpool.", "Verus DoomKnight"),
        new Option<string>("Taunter2Class", "Taunter 2 Class (Secondary)", "Class name for Taunter2 — taunts Kasuko and attacks Whirlpool.", "Lord of Order"),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Verus DoomKnight"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "King's Echo"),
        new Option<string>("Class5", "Class 5", "Preset class 5 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class6", "Class 6", "Preset class 6 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class7", "Class 7", "Preset class 7 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
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
        _tauntCts = new();
        Bot.Events.ScriptStopping -= StopTauntEvent;
        Bot.Events.ScriptStopping += StopTauntEvent;

        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
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
        return className == Bot.Config!.Get<string>("Taunter1Class")
            || className == Bot.Config!.Get<string>("Taunter2Class");
    }

    private int MyTaunterIndex()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Bot.Config!.Get<string>("Taunter1Class")) return 0;
        if (className == Bot.Config!.Get<string>("Taunter2Class")) return 1;
        return -1;
    }

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "kasukov3_class-v3.sync");
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        usePotions = Bot.Config!.Get<bool>("UsePotions");

        // Determine role based on equipped class
        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Bot.Config!.Get<string>("Taunter1Class")) _role = Taunter1AttackWhirlpool1;
        else if (className == Bot.Config!.Get<string>("Taunter2Class")) _role = Taunter2AttackWhirlpool2;
        C.Logger($"[Kasukov3] Role: {_role} ({className})");

        if (Bot.Config!.Get<bool>("DoEnh"))
            Enh.Apply();

        bool skipThird = IsTaunter();
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
        {
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: skipThird);
            Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird, ensureStock: false);
        }

        Bot.Sleep(2500);
    }

    private void Fight()
    {
        const string map = "lavarockshore";
        const string boss = "Kasuko";
        const string bossDefeatedTemp = "Molten Heart";

        const string waitSyncFile = "kasukov3.sync";
        const string completionSyncFile = "Kasukov3Completion.sync";
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));

        const int questId = 9254;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: skipThird);
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird, ensureStock: false);

        if (skipThird)
        {
            C.Logger("[Kasukov3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
        }

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        // ── Taunt loop setup ────────────────────────────────────
        string fightTimeSyncPath = Ultra.ResolveSyncPath("Kasukov3FightTime.sync");
        if (_role == Taunter1AttackWhirlpool1)
        {
            C.Logger("[Kasukov3] Taunter1 (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 2, cancellationToken: _tauntCts.Token);
        }
        else if (_role == Taunter2AttackWhirlpool2)
        {
            C.Logger("[Kasukov3] Taunter2 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, 2, cancellationToken: _tauntCts.Token);
        }

        Bot.Sleep(2000);

        // Pre-seed completion sync file so all entries exist before the loop starts.
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
                C.Logger("Kasuko defeated. Finishing quest.");
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // ── Dynamic targeting ─────────────────────────────────
            // Taunters: attack Whirlpool normally, switch to Kasuko during
            //           taunt pulse window (~1s before to ~3s after) to land the taunt.
            //           Pattern from UltraNulgathv3 Taunter3AttackBlade.
            // Non-taunters: Whirlpool (MapID 1) → Kasuko (MapID 2)
            if (IsTaunter())
            {
                double elapsed = (DateTime.UtcNow - fightStartTime).TotalSeconds;
                int pulseInterval = 5;
                int cycleLength = 2 * pulseInterval; // 10s cycle for 2 taunters
                double timeInCycle = elapsed % cycleLength;
                int myFireTime = MyTaunterIndex() * pulseInterval;
                double startWindow = (myFireTime - 1 + cycleLength) % cycleLength;
                double endWindow = (myFireTime + 3) % cycleLength;

                bool inTauntWindow;
                if (startWindow < endWindow)
                    inTauntWindow = timeInCycle >= startWindow && timeInCycle <= endWindow;
                else
                    inTauntWindow = timeInCycle >= startWindow || timeInCycle <= endWindow;

                if (inTauntWindow)
                {
                    // During taunt window — target Kasuko to land the taunt
                    if (Bot.Player.Target?.Name != boss)
                        Bot.Combat.Attack(boss);
                }
                else
                {
                    // Outside taunt window — attack Whirlpool if alive, else Kasuko
                    if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == WhirlpoolMapID && x.HP > 0))
                    {
                        if (Bot.Player.Target?.MapID != WhirlpoolMapID)
                            Bot.Combat.Attack(WhirlpoolMapID);
                    }
                    else if (Bot.Player.Target?.Name != boss)
                    {
                        Bot.Combat.Attack(boss);
                    }
                }
            }
            else
            {
                // Non-taunter — Whirlpool first, then Kasuko
                if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == WhirlpoolMapID && x.HP > 0))
                {
                    if (Bot.Player.Target?.MapID != WhirlpoolMapID)
                        Bot.Combat.Attack(WhirlpoolMapID);
                }
                else if (Bot.Player.Target?.Name != boss)
                {
                    Bot.Combat.Attack(boss);
                }
            }

            if (usePotions)
                Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
