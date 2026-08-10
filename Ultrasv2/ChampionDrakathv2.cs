/*
name: ChampionDrakathv2
description: Champion Drakath helper with threshold-based taunt timing and army sync.
tags: Ultra
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
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Options;

public class ChampionDrakathv2
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


    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath-v2";
    string a, b, c, d;
    int previousHP = 0;
    private static readonly int[] roundThresholds = { 18000000, 16000000, 14000000, 12000000, 10000000, 8000000, 6000000, 4000000 };

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("a", "Taunter Class (Primary)", "", "ArchPaladin"),
        new Option<string>("b", "Taunter Class (Secondary)", "", "Lord of Order"),
        new Option<string>("c", "Taunter Class (Tertiary)", "", "StoneCrusher"),
        new Option<string>("d", "Taunter Class (Quaternary)", "", ""),
        new Option<bool>("SoloTaunt", "Solo Taunt", "Only primary taunter", false),
        new Option<HowManyTaunts>("HowManyTaunts", "How many taunters", "", HowManyTaunts.Three),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "King's Echo"),

        new Option<bool>("DoEnh", "Do Enhancements", "", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        new Option<int>("ThresholdBuffer", "Threshold Buffer (HP)", "How far above each HP threshold to start taunting. Increase if your team nukes fast and misses thresholds. Default: 250000", 250000),CoreBots.Instance.SkipOptions,
    };
    bool SoloTaunt;

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config!.Get<string>("a") ?? string.Empty).Trim();
        b = (Bot.Config.Get<string>("b") ?? string.Empty).Trim();
        c = (Bot.Config.Get<string>("c") ?? string.Empty).Trim();
        d = (Bot.Config.Get<string>("d") ?? string.Empty).Trim();
        SoloTaunt = Bot.Config.Get<bool>("SoloTaunt");

        if ((SoloTaunt && string.IsNullOrEmpty(a))
            || (!SoloTaunt && string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)))
        {
            Core.Log("Setup", "Primary taunter required.");
            Bot.StopSync();
            return;
        }

        if (SoloTaunt)
        {
            b = string.Empty;
            c = string.Empty;
            d = string.Empty;
        }

        C.SetOptions();
        Core.Boot();
        try
        {
            Prep();
            Fight();
            C.JumpWait();
        }
        finally
        {
            Core.DisableSkills();
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    bool IsTaunter()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(currentClass))
            return false;

        int taunterCount = (int)Bot.Config!.Get<HowManyTaunts>("HowManyTaunts");

        if (taunterCount >= 1 && !string.IsNullOrEmpty(a) && currentClass.Equals(a, StringComparison.OrdinalIgnoreCase))
            return true;
        if (taunterCount >= 2 && !string.IsNullOrEmpty(b) && currentClass.Equals(b, StringComparison.OrdinalIgnoreCase))
            return true;
        if (taunterCount >= 3 && !string.IsNullOrEmpty(c) && currentClass.Equals(c, StringComparison.OrdinalIgnoreCase))
            return true;
        if (taunterCount >= 4 && !string.IsNullOrEmpty(d) && currentClass.Equals(d, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    // Returns 0-based index of this client's taunter slot, or -1 if not a taunter
    int MyTaunterIndex()
    {
        string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        int taunterCount = (int)Bot.Config!.Get<HowManyTaunts>("HowManyTaunts");

        if (taunterCount >= 1 && !string.IsNullOrEmpty(a) && currentClass.Equals(a, StringComparison.OrdinalIgnoreCase))
            return 0;
        if (taunterCount >= 2 && !string.IsNullOrEmpty(b) && currentClass.Equals(b, StringComparison.OrdinalIgnoreCase))
            return 1;
        if (taunterCount >= 3 && !string.IsNullOrEmpty(c) && currentClass.Equals(c, StringComparison.OrdinalIgnoreCase))
            return 2;
        if (taunterCount >= 4 && !string.IsNullOrEmpty(d) && currentClass.Equals(d, StringComparison.OrdinalIgnoreCase))
            return 3;

        return -1;
    }

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "champion_drakath_class-v2.sync");
    }

    void Prep()
    {
        EquipPresetClasses();

        if (Bot.Config!.Get<bool>("DoEnh"))
            Enh.Apply();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: IsTaunter());

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), ensureStock: false);

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map = "championdrakath";
        const string boss = "Champion Drakath";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("ChampionDrakathWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("ChampionDrakathWipeAlive.sync");
        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8300);
        C.AddDrop("Champion Drakath Insignia");

        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "champion_drakath.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);
         
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        bool[] tauntFired = new bool[8]; // 18M-4M in 2M chunks
        previousHP = 0; // Reset at fight start
        int myIndex = MyTaunterIndex();
        int taunterCount = (int)Bot.Config!.Get<HowManyTaunts>("HowManyTaunts");
        int buffer = Bot.Config!.Get<int>("ThresholdBuffer");
        bool armyWipeDetected = false;

        // Build thresholds dynamically: flat round numbers + user-defined buffer
        int[] thresholds = roundThresholds.Select(t => t + buffer).ToArray();
        C.Logger($"Thresholds: [{string.Join(", ", thresholds.Select(t => $"{t:n0}"))}] (buffer: {buffer:n0})");

        while (!Bot.ShouldExit)
        {
            bool allDead = UltraGeneral.IsWholeArmyDead(Ultra, Bot, wipeDeadSyncPath);
            if (allDead)
            {
                if (!armyWipeDetected)
                    C.Logger("Army wipe detected — all clients dead.");
                armyWipeDetected = true;
            }

            if (armyWipeDetected)
            {
                bool allAlive = UltraGeneral.IsWholeArmyAlive(Ultra, Bot, wipeAliveSyncPath);
                if (allAlive)
                {
                    C.Logger("Army wipe recovered — all clients alive again.");
                    Ultra.ClearSyncFile(wipeDeadSyncPath);
                    Ultra.ClearSyncFile(wipeAliveSyncPath);
                    armyWipeDetected = false;
                    previousHP = 0;
                    for (int j = 0; j < tauntFired.Length; j++)
                        tauntFired[j] = false;
                    C.Logger("Army wipe recovered — resuming fight.");
                    continue;
                }

                Bot.Combat.CancelTarget();
                C.Logger("Army wipe active — waiting for everyone to respawn before fighting.");
                Bot.Sleep(250);
                continue;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Champion Drakath Defeated"), syncPath))
            {
                Bot.Sleep(2500);
                C.Jump("Enter", "Spawn");
                if (!Bot.Quests.IsDailyComplete(8300))
                    C.EnsureComplete(8300);
                else Bot.Log("Daily already Complete");
                Ultra.JoinHouse();
                break;
            }

            Bot.Combat.Attack("*");

            Pots.ActivateEquippedPotion();

            Bot.Sleep(100);

            // Only execute taunt logic if this account is a taunter
            if (myIndex >= 0
                && Bot.Player.HasTarget
                && Bot.Player.Target?.HP > 0)
            {
                // Detect HP reset (boss respawned/wiped)
                if (Bot.Player.Target?.HP > previousHP + 1000000)
                {
                    C.Logger("Boss HP reset detected - clearing taunt flags");
                    for (int j = 0; j < tauntFired.Length; j++)
                        tauntFired[j] = false;
                }

                previousHP = Bot.Player.Target?.HP ?? 0;

                // Check thresholds (18M down to 4M)
                // Each threshold is assigned to a taunter by index: threshold % taunterCount == myIndex
                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (tauntFired[i])
                        continue;

                    if (Bot.Player.Target?.HP > thresholds[i])
                        continue;

                    // Only this taunter's assigned thresholds
                    if (i % taunterCount != myIndex)
                    {
                        // Not my threshold — mark it fired so we don't keep checking
                        tauntFired[i] = true;
                        continue;
                    }

                    C.Logger($"{roundThresholds[i] / 1000000}M threshold — my turn (slot {myIndex}), taunting!");

                    Bot.Combat.Attack(boss);

                    for (int p = 0; p < 60 && !Bot.ShouldExit; p++)
                    {
                        if (!Bot.Player.Alive)
                        {
                            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                            // Reset from this threshold onwards on death
                            for (int j = i; j < tauntFired.Length; j++)
                                tauntFired[j] = false;
                            goto NextLoop;
                        }
                        Core.Cast(5);
                        Bot.Sleep(50);
                    }

                    tauntFired[i] = true;
                    Bot.Sleep(100);
                    break;
                }

                // After 2M → always taunt (all taunters)
                if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 2100000)
                {
                    C.Logger($"HP < 2M — continuous taunt (slot {myIndex})");
                    Bot.Combat.Attack(boss);

                    for (int p = 0; p < 60 && !Bot.ShouldExit; p++)
                    {
                        if (!Bot.Player.Alive) break;
                        Core.Cast(5);
                        Bot.Sleep(50);
                    }

                    Bot.Sleep(100);
                }
            }

            NextLoop:;
        }

        C.JumpWait();
    }


    enum HowManyTaunts
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
}



