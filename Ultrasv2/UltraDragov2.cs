/*
name: UltraDragov2
description: Ultra King Drago helper with class-based taunters and UltraPotions support.
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
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;
public class UltraDragov2
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

    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;

    public IScriptInterface Bot => IScriptInterface.Instance;

    private static CoreEnginev3 Core => CoreEnginev3.Instance;

    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    public bool DontPreconfigure = true;

    public string OptionsStorage = "UltraDrago-v2";

    bool isPrimaryTaunter;
    bool isSecondaryTaunter;

    // Timer-based taunt rotation
    private DateTime fightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 12.0; // 2 taunters × 6s slots
    private const double TauntWindowSeconds = 6.0;
    private DateTime lastTauntTime = DateTime.MinValue;

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),

        new Option<string>(
            "PrimaryTaunter",
            "Primary Taunter Class",
            "Class name of the primary taunter.",
            "ArchPaladin"
        ),

        new Option<string>(
            "SecondaryTaunter",
            "Secondary Taunter Class",
            "Class name of the secondary taunter.",
            "StoneCrusher"
        ),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "King's Echo"),

        new Option<bool>(
            "DoEnh",
            "Do Enhancements",
            "Auto-Enhance Gear properly for the fight",
            true
        ),

        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),

        new Option<int>(
            "PotionQuantity",
            "Potion Quantity",
            "How many potions to keep stocked.",
            10
        ),

        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string? input) =>
        (input ?? "").Trim().ToLower();

    bool HasAssignedClass(string? assignedClass) =>
        NormalizeString(Bot.Player.CurrentClass?.Name)
        == NormalizeString(assignedClass);

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "drago_class-v2.sync");
    }

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("Ultra Drago loaded.");

        C.SetOptions();

        Core.Boot();
        try
        {
            Prep();

            Fight();
        }
        finally
        {
            Core.DisableSkills();
            C.SetOptions(false);

            Bot.StopSync();
        }
    }

    void Prep()
    {
        C.Join("whitemap");

        EquipPresetClasses();

        Astravia.AstraviaJudgement();

        string primaryTaunter =
            (Bot.Config!.Get<string>("PrimaryTaunter") ?? string.Empty).Trim();

        string secondaryTaunter =
            (Bot.Config!.Get<string>("SecondaryTaunter") ?? string.Empty).Trim();

        isPrimaryTaunter = HasAssignedClass(primaryTaunter);

        isSecondaryTaunter = HasAssignedClass(secondaryTaunter);

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: isPrimaryTaunter || isSecondaryTaunter);

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: isPrimaryTaunter || isSecondaryTaunter, ensureStock: false);

        // Assign taunt offset after enhancements so the class is final
        string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (!string.IsNullOrEmpty(primaryTaunter) && cn.Equals(primaryTaunter, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (!string.IsNullOrEmpty(secondaryTaunter) && cn.Equals(secondaryTaunter, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 6;
        else
            tauntOffsetSeconds = 0; // non-taunter, offset irrelevant

        C.Logger($"Taunter [{(isPrimaryTaunter || isSecondaryTaunter ? "Yes" : "No")}] | Drago taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        if (isPrimaryTaunter || isSecondaryTaunter)
            Ultra.GetScrollOfEnrage();

        Bot.Sleep(2500);
    }

    void Fight()
    {
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string leftSummon = "Executioner Dene";
        const string rightSummon = "Bowmaster Algie";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("UltraDragoWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("UltraDragoWipeAlive.sync");

        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);

        Bot.Sleep(2500);

        if (!Bot.Quests.IsUnlocked(8397))
            Bot.Quests.UpdateQuest(8395);

        C.AddDrop("King Drago Insignia");

        Core.Join(map);

        C.EnsureAccept(8397);

        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_drago.sync");

        var (bestCell, _) = Core.ChooseBestCell(boss);


        Bot.Player.SetSpawnPoint();

        Core.EnableSkills();

        // Sync fight start time
        fightStartTime = DateTime.Now;
        C.Logger($"Fight start time synced: {fightStartTime:HH:mm:ss.fff}");

        bool armyWipeDetected = false;

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
                    Bot.Combat.CancelTarget();
                    fightStartTime = DateTime.MinValue;
                    lastTauntTime = DateTime.MinValue;
                    armyWipeDetected = false;
                    C.Logger("Army wipe recovered — resetting taunt timing and resuming fight.");
                    continue;
                }

                Bot.Combat.CancelTarget();
                C.Logger("Army wipe active — waiting for everyone to respawn before fighting.");
                Bot.Sleep(250);
                continue;
            }

            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                lastTauntTime = DateTime.MinValue;
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Drago Dethroned", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsUnlocked(8397))
                    C.EnsureComplete(8397);
                Bot.Wait.ForPickup("King Drago Insignia");
                Ultra.JoinHouse();
                break;
            }

            Pots.ActivateEquippedPotion();

            if (isPrimaryTaunter || isSecondaryTaunter)
            {
                // Taunters: Executioner Dene (left) → Bowmaster Algie (right) → Boss
                // Taunt is on leftSummon (Executioner Dene)
                if (Ultra.MonsterAlive(leftSummon))
                {
                    if (Bot.Player.Target?.Name != leftSummon)
                        Bot.Combat.Attack(leftSummon);

                    // Timer-based taunt rotation — only when summon is alive and we have a target
                    if (Bot.Player.HasTarget)
                    {
                        TimeSpan timeSinceFightStart = DateTime.Now - fightStartTime;
                        double currentTime = timeSinceFightStart.TotalSeconds;
                        double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;
                        bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
                        bool cooldownExpired = (DateTime.Now - lastTauntTime).TotalSeconds >= TauntIntervalSeconds - 1;

                        if (inTauntWindow && cooldownExpired)
                        {
                            lastTauntTime = DateTime.Now;
                            Bot.Combat.Attack(leftSummon);
                            C.Logger($"Taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s) — taunting {leftSummon}!");

                            for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                            {
                                if (!Bot.Player.Alive) break;
                                if (!Bot.Player.HasTarget)
                                    Bot.Combat.Attack(leftSummon);
                                Core.Cast(5);
                                Bot.Sleep(50);
                            }

                            C.Logger("Taunt done — 60 presses complete.");
                            Bot.Sleep(100);
                        }
                    }
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
                // Non-taunters: Bowmaster Algie (right) → Executioner Dene (left) → Boss
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

            Bot.Sleep(100);
        }
    }

    void DoEnhs() => Enh.Apply();
}


