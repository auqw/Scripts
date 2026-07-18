/*
name: AstralEmpyreanv3
description: Two-taunter strategy for Astral Empyrean with aura-based taunting and army synchronization.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
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
using Skua.Core.Options;

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

    bool usePotions;
    private CancellationTokenSource _tauntCts = new();
    private DateTime fightStartTime = DateTime.MinValue;
    private string _role = "";
    private int _deathRetries = 0;
    private const int MaxDeathRetries = 3;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "AstralEmpyreanv3Army";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("Taunter1Class", "Taunter 1 Class (Primary)", "Class name for Taunter 1 (sets fight start time).", "Verus DoomKnight"),
        new Option<string>("Taunter2Class", "Taunter 2 Class (Secondary)", "Class name for Taunter 2.", "Lord Of Order"),
        new Option<string>("Taunter3Class", "Taunter 3 Class (Tertiary)", "Class name for Taunter 3.", "ArchPaladin"),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord Of Order"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Verus DoomKnight"),
        new Option<string>("Class5", "Class 5", "Preset class 5 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class6", "Class 6", "Preset class 6 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class7", "Class 7", "Preset class 7 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),

        CoreBots2.Instance.SkipOptions,
    };

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
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "astralempyreanv3_class-v3.sync");
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
            || className == Bot.Config!.Get<string>("Taunter2Class")
            || className == Bot.Config!.Get<string>("Taunter3Class");
    }

    private int MyTaunterIndex()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Bot.Config!.Get<string>("Taunter1Class")) return 0;
        if (className == Bot.Config!.Get<string>("Taunter2Class")) return 1;
        if (className == Bot.Config!.Get<string>("Taunter3Class")) return 2;
        return -1;
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        usePotions = Bot.Config!.Get<bool>("UsePotions");

        string? className = Bot.Player.CurrentClass?.Name;
        if (className == Bot.Config!.Get<string>("Taunter1Class")) _role = "Taunter1";
        else if (className == Bot.Config!.Get<string>("Taunter2Class")) _role = "Taunter2";
        else if (className == Bot.Config!.Get<string>("Taunter3Class")) _role = "Taunter3";
        C.Logger($"[AstralEmpyreanv3] Role: {_role} ({className})");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
        {
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: IsTaunter(), context: "AstralEmpyrean");
            Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), ensureStock: false, context: "AstralEmpyrean");
        }

        Bot.Quests.UpdateQuest(9802);
        Bot.Sleep(2500);
    }

    void DoEnhs() => Enh.ApplyAstralEmpyrean();

    private void Fight()
    {
        const string map = "astralshrine";
        const string boss = "Astral Empyrean";
        const string bossDefeatedTemp = "Astral's Supernova";

        const string waitSyncFile = "AstralEmpyreanv3.sync";
        const string completionSyncFile = "AstralEmpyreanv3Completion.sync";
        const string retreatSyncFile = "AstralEmpyreanv3Retreat.sync";
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));

        
        const int questId = 9803;

        C.AddDrop("Star of the Empyrean");
        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: skipThird, context: "AstralEmpyrean");
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: skipThird, ensureStock: false, context: "AstralEmpyrean");

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
            // Check if the entire army is dead — alive players retreat only on full wipe
            if (Bot.Player.Alive && Death.IsArmyWiped(armySize))
            {
                _deathRetries++;
                C.Logger($"[AstralEmpyreanv3] Army wipe detected. Retreating. Retry {_deathRetries}/{MaxDeathRetries}.");

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
                C.Logger($"[AstralEmpyreanv3] Death detected. Waiting for army wipe. Retry {_deathRetries}/{MaxDeathRetries}.");

                Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();

                Death.SignalDeath();
                Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));
                Ultra.ClearSyncFile(Ultra.ResolveSyncPath(retreatSyncFile));

                // Wait for the rest of the army to die (full wipe) or timeout
                var wipeWaitStart = DateTime.UtcNow;
                const int wipeTimeoutSec = 30;
                while (!Bot.ShouldExit)
                {
                    if (Death.IsArmyWiped(armySize))
                    {
                        C.Logger($"[AstralEmpyreanv3] Army wipe confirmed after {(DateTime.UtcNow - wipeWaitStart).TotalSeconds:F0}s.");
                        break;
                    }

                    if ((DateTime.UtcNow - wipeWaitStart).TotalSeconds >= wipeTimeoutSec)
                    {
                        C.Logger($"[AstralEmpyreanv3] Wipe wait timeout ({wipeTimeoutSec}s) — retreating anyway.", "Warning");
                        break;
                    }

                    Bot.Sleep(500);
                }

                if (Bot.ShouldExit)
                    break;

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
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // Default — attack the boss
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            if (usePotions)
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
