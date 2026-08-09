/*
name: UltraNulgathv2
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
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
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UltraNulgathv2
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
    public string OptionsStorage = "UltraNulgath-v2";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("a", "Taunter 1 ClassName", "Class name of Taunter 1 (fires at 0s).\nName must be exact including punctuation, spelling, and capitalisation.", "Lord of Order"),
        new Option<string>("b", "Taunter 2 ClassName", "Class name of Taunter 2 (fires at 5s).\nName must be exact including punctuation, spelling, and capitalisation.", "StoneCrusher"),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "King's Echo"),

        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),CoreBots.Instance.SkipOptions,
    };

    string a, b;

    // Timer-based taunt rotation
    private DateTime fightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 10.0; // 2 taunters × 5s slots
    private const double TauntWindowSeconds = 5.0;
    private DateTime lastTauntTime = DateTime.MinValue;
    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "Ultra Nulgath",
            "Deaths more then likely will happen, Suggested class and thier enhs are in the script at the top"
        );

        a = Bot.Config!.Get<string>("a") ?? "";
        b = Bot.Config!.Get<string>("b") ?? "";

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

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "nulgath_class-v2.sync");
    }

    // Overfiend Blade = 1
    // Nulgath = 2
    void Fight()
    {

        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("UltraNulgathWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("UltraNulgathWipeAlive.sync");

        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");
        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_nulgath.sync");

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

            // 1. Death Reset Logic
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                lastTauntTime = DateTime.MinValue;
                continue;
            }

            // 2. Victory Check
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Nulgath the Archfiend Defeated?", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                Core.DisableSkills();
                Bot.Sleep(200);
                C.Jump("Enter", "Spawn");
                C.Join("whitemap");
                if (!Bot.Quests.IsDailyComplete(8692))
                    C.EnsureComplete(8692);
                Ultra.JoinHouse();
                break;
            }

            // 3. Dynamic Targeting
            if (IsTaunter())
            {
                if (Bot.Player.Target?.MapID != 2)
                    Bot.Combat.Attack(2);
            }
            else
            {
                // DPS focus Blade (1) until dead, then switch to Nulgath (2)
                if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
                {
                    if (Bot.Player.Target?.MapID != 1)
                        Bot.Combat.Attack(1);
                }
                else
                {
                    if (Bot.Player.Target?.MapID != 2)
                        Bot.Combat.Attack(2);
                }
            }

            // 4. Timer-based taunt rotation
            if (IsTaunter() && Bot.Player.HasTarget)
            {
                TimeSpan timeSinceFightStart = DateTime.Now - fightStartTime;
                double currentTime = timeSinceFightStart.TotalSeconds;
                double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;
                bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
                bool cooldownExpired = (DateTime.Now - lastTauntTime).TotalSeconds >= TauntIntervalSeconds - 1;

                if (inTauntWindow && cooldownExpired)
                {
                    lastTauntTime = DateTime.Now;
                    Bot.Combat.Attack(2);
                    C.Logger($"Taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s) — taunting!");

                    for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        Core.Cast(5);
                        Bot.Sleep(50);
                    }

                    C.Logger("Taunt done — 60 presses complete.");
                    Bot.Sleep(100);
                }
            }

            Pots.ActivateEquippedPotion();
            Bot.Sleep(100);
        }
    }


    void Prep()
    {
        EquipPresetClasses();

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: IsTaunter());

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), ensureStock: false);

        // Assign taunt offset after enhancements so the class is final
        string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (!string.IsNullOrEmpty(a) && cn.Equals(a, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (!string.IsNullOrEmpty(b) && cn.Equals(b, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 5;
        else
            tauntOffsetSeconds = 0; // non-taunter, offset irrelevant

        C.Logger($"Taunter [{(IsTaunter() ? "Yes" : "No")}] | Nulgath taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        if (IsTaunter())
        {
            Ultra.GetScrollOfEnrage();
            Core.EquipEnrage();
        }
    }
    void DoEnhs() => Enh.Apply();
}



