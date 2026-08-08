/*
name: UltraGramielv2
description: Ultra Gramiel helper with class sync support.
tags: ultra, gramiel, Ultra Gramiel
*/

//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraGramielv2
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    public bool DontPreconfigure = true;

    public string OptionsStorage = "UltraGramiel-v2";

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

    // Gramiel crystal role tracking
    private int crystalMapId = 2;
    private bool isT1Taunter = false;
    private bool isTaunter = false;
    private DateTime fightStartTime = DateTime.MinValue;
    private DateTime gramielFightStartTime = DateTime.MinValue;
    private DateTime lastCrystalTauntTime = DateTime.MinValue;
    private DateTime lastGramielTauntTime = DateTime.MinValue;
    private const double CrystalTauntCycleSeconds = 28.0; // Full crystal cycle: T1 (14s) + T2 (14s)
    private const double CrystalTauntWindowSeconds = 14.0; // Each crystal taunter group gets 14 seconds
    private const double GramielTauntIntervalSeconds = 12.0; // Full cycle: 4 taunters × 3s slots
    private const double GramielTauntWindowSeconds = 3.0; // Each taunter gets 3 seconds
    private double crystalTauntOffsetSeconds = 0; // 0/14 seconds for crystal phase
    private double gramielTauntOffsetSeconds = 0; // 0/3/6/9 seconds for Gramiel phase

    private bool IsTaunter() => isTaunter;

    private int tauntCounter = 0;
    private DateTime lastTauntWarningTime = DateTime.MinValue;
    private volatile bool shouldExecuteTaunt = false;
    private readonly System.Threading.ManualResetEventSlim _tauntSignal = new(false);
    private bool timerPrimedByChat = false;
    private DateTime nextWarningTime = DateTime.MinValue;
    private const double ChatWarningIntervalSeconds = 14.0;

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),

        new Option<string>(
            "LeftCrystalT1",
            "Left Crystal — T1 Taunter",
            "Class name for the Left Crystal T1 taunter.",
            "StoneCrusher"
        ),

        new Option<string>(
            "RightCrystalT1",
            "Right Crystal — T1 Taunter",
            "Class name for the Right Crystal T1 taunter.",
            "Lord of Order"
        ),

        new Option<string>(
            "LeftCrystalT2",
            "Left Crystal — T2 Taunter",
            "Class name for the Left Crystal T2 taunter.",
            "ArchPaladin"
        ),

        new Option<string>(
            "RightCrystalT2",
            "Right Crystal — T2 Taunter",
            "Class name for the Right Crystal T2 taunter.",
            "King's Echo"
        ),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "King's Echo"),

        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance gear before the fight.", true),

        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),

        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("Ultra Gramiel loaded.");

        C.SetOptions();
        Core.Boot();
        try
        {
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
        finally
        {
            Core.DisableSkills();
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    private void ApplyTaunterClasses()
    {
        string leftT1Class = Bot.Config!.Get<string>("LeftCrystalT1").Trim();
        string rightT1Class = Bot.Config!.Get<string>("RightCrystalT1").Trim();
        string leftT2Class = Bot.Config!.Get<string>("LeftCrystalT2").Trim();
        string rightT2Class = Bot.Config!.Get<string>("RightCrystalT2").Trim();

        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;

        if (className.Equals(leftT1Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isTaunter = true;
            isT1Taunter = true;
            crystalTauntOffsetSeconds = 0;
            gramielTauntOffsetSeconds = 0;
            C.Logger($"Assigned to taunt LEFT crystal (mapId=2) - T1 (crystal offset: 0s, Gramiel offset: 0s)");
        }
        else if (className.Equals(leftT2Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isTaunter = true;
            isT1Taunter = false;
            crystalTauntOffsetSeconds = 14;
            gramielTauntOffsetSeconds = 6;
            C.Logger($"Assigned to taunt LEFT crystal (mapId=2) - T2 (crystal offset: 14s, Gramiel offset: 6s)");
        }
        else if (className.Equals(rightT1Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isTaunter = true;
            isT1Taunter = true;
            crystalTauntOffsetSeconds = 0;
            gramielTauntOffsetSeconds = 3;
            C.Logger($"Assigned to taunt RIGHT crystal (mapId=3) - T1 (crystal offset: 0s, Gramiel offset: 3s)");
        }
        else if (className.Equals(rightT2Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isTaunter = true;
            isT1Taunter = false;
            crystalTauntOffsetSeconds = 14;
            gramielTauntOffsetSeconds = 9;
            C.Logger($"Assigned to taunt RIGHT crystal (mapId=3) - T2 (crystal offset: 14s, Gramiel offset: 9s)");
        }
        else
        {
            crystalMapId = 2;
            isTaunter = false;
            isT1Taunter = true;
            crystalTauntOffsetSeconds = 0;
            gramielTauntOffsetSeconds = 0;
            C.Logger($"Class '{className}' not found in config, defaulting to LEFT crystal (mapId=2) - T1 (crystal offset: 0s, Gramiel offset: 0s)");
        }
    }

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "gramiel_class-v2.sync");
    }

    private void Prep()
    {
        EquipPresetClasses();
        ApplyTaunterClasses();

        timerPrimedByChat = false;
        tauntCounter = 0;
        nextWarningTime = DateTime.MinValue;
        lastTauntWarningTime = DateTime.MinValue;
        shouldExecuteTaunt = false;
        fightStartTime = DateTime.MinValue;

        C.Logger($"Current class: {Bot.Player.CurrentClass?.Name ?? "None"} | IsTaunter: {IsTaunter()}");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: IsTaunter());

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), ensureStock: false);

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();

        Bot.Sleep(2500);
    }

    private void Fight()
    {
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        const string map = "ultragramiel";
        const int gramielMapId = 1;

        C.AddDrop("Gramiel the Graceful Vanquished");
        C.EnsureAccept(10301);

        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_gramiel.sync");
        Core.ChooseBestCell("*");
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        fightStartTime = DateTime.Now;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(250);
                continue;
            }

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Gramiel the Graceful Vanquished", 1), syncPath))
            {
                Core.DisableSkills();
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(10301))
                    C.EnsureComplete(10301);
                Ultra.JoinHouse();
                break;
            }

            // Check if any crystal is alive
            bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
                .Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

            if (anyCrystalAlive)
            {
                int targetCrystalMapId = GetAliveCrystalTarget(crystalMapId);

                if (timerPrimedByChat && !shouldExecuteTaunt && DateTime.Now >= nextWarningTime)
                {
                    tauntCounter++;
                    bool currentlyT1Turn = tauntCounter % 2 == 1;
                    C.Logger($"[GramielWarning] {DateTime.Now:HH:mm:ss.fff} timer warning, turn={(currentlyT1Turn ? "T1" : "T2")}, counter={tauntCounter}");
                    nextWarningTime = DateTime.Now.AddSeconds(ChatWarningIntervalSeconds);

                    bool shouldTauntNow = (isT1Taunter && currentlyT1Turn) || (!isT1Taunter && !currentlyT1Turn);
                    if (shouldTauntNow)
                    {
                        shouldExecuteTaunt = true;
                        _tauntSignal.Set();
                    }
                }

                if (shouldExecuteTaunt)
                {
                    shouldExecuteTaunt = false;
                    _tauntSignal.Reset();
                    ExecuteCrystalTaunt(targetCrystalMapId);
                    continue;
                }

                if (targetCrystalMapId == 0)
                {
                    C.Logger("Alive crystals exist but no valid target resolved; attacking Gramiel as fallback.");
                    Bot.Combat.Attack(gramielMapId);
                    Bot.Sleep(250);
                    continue;
                }

                if (targetCrystalMapId != crystalMapId)
                {
                    C.Logger($"Primary crystal {crystalMapId} is down; switching to crystal {targetCrystalMapId}.");
                }
                else
                {
                    int otherCrystalMapId = crystalMapId == 2 ? 3 : 2;
                    int myHP = Bot.Monsters.CurrentAvailableMonsters
                        .Where(x => x != null && x.Alive && x.MapID == crystalMapId)
                        .Select(x => x.HP)
                        .FirstOrDefault();
                    int otherHP = Bot.Monsters.CurrentAvailableMonsters
                        .Where(x => x != null && x.Alive && x.MapID == otherCrystalMapId)
                        .Select(x => x.HP)
                        .FirstOrDefault();

                    if (otherHP > 0 && myHP > 0 && myHP < otherHP - 30)
                    {
                        C.Logger($"Retargeting from crystal {crystalMapId} to crystal {otherCrystalMapId} for HP balance.");
                        targetCrystalMapId = otherCrystalMapId;
                    }
                }

                Bot.Combat.Attack(targetCrystalMapId);

                if (timerPrimedByChat)
                {
                    double elapsedSeconds = (DateTime.Now - fightStartTime).TotalSeconds;
                    double crystalTimeInCycle = (elapsedSeconds - crystalTauntOffsetSeconds) % CrystalTauntCycleSeconds;
                    if (crystalTimeInCycle < 0)
                        crystalTimeInCycle += CrystalTauntCycleSeconds;

                    bool crystalInTauntWindow = crystalTimeInCycle >= 0 && crystalTimeInCycle < CrystalTauntWindowSeconds;
                    bool crystalCooldownExpired = (DateTime.Now - lastCrystalTauntTime).TotalSeconds >= CrystalTauntCycleSeconds - 1;

                    if (crystalInTauntWindow && crystalCooldownExpired && Bot.Player.HasTarget)
                    {
                        lastCrystalTauntTime = DateTime.Now;
                        C.Logger($"Crystal taunt window ({(isT1Taunter ? "T1" : "T2")}) reached (offset {crystalTauntOffsetSeconds}s, current={elapsedSeconds:F1}s).");
                        for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive)
                                break;
                            if (!Bot.Player.HasTarget)
                                Bot.Combat.Attack(targetCrystalMapId);
                            Core.Cast(5);
                            Bot.Sleep(50);
                        }
                        C.Logger("Crystal taunt executed.");
                    }
                }

                Bot.Sleep(250);
                continue;
            }

            if (gramielFightStartTime == DateTime.MinValue)
            {
                gramielFightStartTime = DateTime.Now;
                C.Logger("All crystals dead. Beginning Gramiel taunt rotation.");
            }

            Bot.Combat.Attack(gramielMapId);

            TimeSpan timeSinceFightStart = DateTime.Now - gramielFightStartTime;
            double currentTime = timeSinceFightStart.TotalSeconds;
            double gramielTimeInCycle = (currentTime - gramielTauntOffsetSeconds) % GramielTauntIntervalSeconds;
            if (gramielTimeInCycle < 0)
                gramielTimeInCycle += GramielTauntIntervalSeconds;

            bool gramielInTauntWindow = gramielTimeInCycle >= 0 && gramielTimeInCycle < GramielTauntWindowSeconds;
            bool gramielCooldownExpired = (DateTime.Now - lastGramielTauntTime).TotalSeconds >= GramielTauntIntervalSeconds - 1;

            if (gramielInTauntWindow && gramielCooldownExpired && Bot.Player.HasTarget)
            {
                lastGramielTauntTime = DateTime.Now;
                C.Logger($"Gramiel taunt window reached (offset {gramielTauntOffsetSeconds}s, current={currentTime:F1}s).");

                for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                {
                    if (!Bot.Player.Alive)
                        break;
                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(gramielMapId);
                    Core.Cast(5);
                    Bot.Sleep(50);
                }

                C.Logger("Gramiel taunt executed.");
            }

            Bot.Sleep(100);
        }
    }

    void DoEnhs() => Enh.ApplyGramiel();

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

    private void ExecuteCrystalTaunt(int targetCrystalMapId)
    {
        if (targetCrystalMapId == 0)
        {
            C.Logger("No alive crystal available for taunt. Skipping this taunt.");
            return;
        }

        if (targetCrystalMapId != crystalMapId)
            C.Logger($"Primary crystal {crystalMapId} is down, switching to crystal {targetCrystalMapId}.");

        Bot.Combat.Attack(targetCrystalMapId);

        int acquireAttempts = 0;
        while (!Bot.Player.HasTarget && acquireAttempts < 5 && !Bot.ShouldExit)
        {
            Bot.Combat.Attack(targetCrystalMapId);
            acquireAttempts++;
        }

        if (!Bot.Player.HasTarget)
        {
            C.Logger("Failed to acquire crystal target for taunt. Skipping this taunt.");
            return;
        }

        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        C.Logger($"{className} executing crystal taunt!");

        int attempts = 0;
        while (!Bot.ShouldExit && attempts < 40)
        {
            if (!Bot.Player.Alive)
                break;

            if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || Bot.Player.Target.MapID != targetCrystalMapId)
            {
                int newTargetCrystalId = GetAliveCrystalTarget(crystalMapId);
                if (newTargetCrystalId == 0)
                {
                    C.Logger("All crystals are dead during taunt. Aborting crystal taunt.");
                    break;
                }

                if (newTargetCrystalId != targetCrystalMapId)
                {
                    C.Logger($"Crystal target died mid-taunt. Switching from crystal {targetCrystalMapId} to crystal {newTargetCrystalId}.");
                    targetCrystalMapId = newTargetCrystalId;
                }

                Bot.Combat.Attack(targetCrystalMapId);
                Bot.Sleep(25);
                attempts++;
                continue;
            }

            Core.Cast(5);
            Bot.Sleep(25);
            attempts++;
        }
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
                if (message.Contains("The Grace Crystal prepares a defense shattering attack!", StringComparison.OrdinalIgnoreCase))
                {
                    TimeSpan timeSinceLastWarning = DateTime.Now - lastTauntWarningTime;
                    if (timeSinceLastWarning.TotalMilliseconds < 3000)
                        return;

                    lastTauntWarningTime = DateTime.Now;

                    if (!timerPrimedByChat)
                    {
                        timerPrimedByChat = true;
                        fightStartTime = DateTime.Now;
                        tauntCounter++;
                        bool currentlyT1Turn = tauntCounter % 2 == 1;
                        bool shouldTauntNow = (isT1Taunter && currentlyT1Turn) || (!isT1Taunter && !currentlyT1Turn);
                        nextWarningTime = DateTime.Now.AddSeconds(ChatWarningIntervalSeconds);

                        C.Logger($"[GramielWarning] first warning received; timer primed, next warning expected at {nextWarningTime:HH:mm:ss.fff}");
                        C.Logger($"[GramielWarning] first warning count={tauntCounter}, turn={(currentlyT1Turn ? "T1" : "T2")}, shouldTauntNow={shouldTauntNow}");

                        if (shouldTauntNow)
                        {
                            shouldExecuteTaunt = true;
                            _tauntSignal.Set();
                        }
                    }
                }
            }
        }
        catch { }
    }

}

