/*
name: UltraDagev2
description: Two-taunter strategy for Ultra Dage with timer-based taunting and army synchronization.
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

using System;
using System.Linq;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UltraDagev2
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

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

    string a, b, primaryDecayer, secondaryDecayer;

    // Timer-based taunt rotation
    private DateTime fightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 10.0; // 2 taunters × 5s slots
    private const double TauntWindowSeconds = 5.0;
    private DateTime lastTauntTime = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage-v2";

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>(
            "a",
            "Primary Taunter Class",
            "Class name of the primary taunter (fires at 0s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),

        new Option<string>(
            "b",
            "Secondary Taunter Class",
            "Class name of the secondary taunter (fires at 5s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "StoneCrusher"
        ),

        new Option<string>(
            "primaryDecayer",
            "Primary Decayer Class",
            "Class name of the primary decayer.",
            ""
        ),

        new Option<string>(
            "secondaryDecayer",
            "Secondary Decayer Class",
            "Class name of the secondary decayer.",
            ""
        ),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Verus DoomKnight"),
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

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();
        primaryDecayer = (Bot.Config.Get<string>("primaryDecayer") ?? "").Trim();
        secondaryDecayer = (Bot.Config.Get<string>("secondaryDecayer") ?? "").Trim();

        C.SetOptions();
        Core.Boot();
        try
        {
            Prep();

            Bot.Events.ExtensionPacketReceived += UltraDageListener;
            try
            {
                Fight();
            }
            finally
            {
                Bot.Events.ExtensionPacketReceived -= UltraDageListener;
            }
        }
        finally
        {
            Core.DisableSkills();
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);
    bool IsPrimaryDecayer() => Core.HasClassEquipped(primaryDecayer);
    bool IsSecondaryDecayer() => Core.HasClassEquipped(secondaryDecayer);

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "dage_class-v2.sync");
    }

    void Prep()
    {
        if (!C.isCompletedBefore(793))
            C.Logger("Player is not part of the Legion — quest turn-in may not be available, but the kill will still proceed.");

        Bot.Quests.UpdateQuest(793);

        EquipPresetClasses();

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: IsTaunter(), context: "Dage");

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), context: "Dage", ensureStock: false);

        // Assign taunt offset after enhancements so the class is final
        string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (!string.IsNullOrEmpty(a) && cn.Equals(a, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (!string.IsNullOrEmpty(b) && cn.Equals(b, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 5;
        else
            tauntOffsetSeconds = 0; // non-taunter, offset irrelevant

        C.Logger($"Taunter [{(IsTaunter() ? "Yes" : "No")}] | Dage taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();

        if (IsPrimaryDecayer() || IsSecondaryDecayer())
            Ultra.GetScrollOfDecay();
    }

    void Fight()
    {
        const string map = "ultradage";
        const string boss = "Dage the Dark Lord";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("UltraDageWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("UltraDageWipeAlive.sync");

        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);
        Bot.Sleep(2500);

        C.AddDrop("Dage the Evil Insignia");
        C.EnsureAccept(8547);

        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_dage.sync");
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

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                lastTauntTime = DateTime.MinValue;
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Dage the Dark Lord Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                if (!Bot.Quests.IsDailyComplete(8547))
                    C.EnsureComplete(8547);
                Ultra.JoinHouse();
                break;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);

            Pots.ActivateEquippedPotion();

            // Timer-based taunt rotation
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
                    Bot.Combat.Attack(boss);
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

            // Decayers
            if (IsPrimaryDecayer() && Core.HasAura("Legionnaire"))
            {
                for (int i = 0; i < 20 && !Bot.ShouldExit; i++)
                {
                    if (!Bot.Player.Alive) break;
                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(boss);
                    Core.Cast(5);
                    Bot.Sleep(25);
                }
            }

            if (IsSecondaryDecayer() && Core.HasAura("Legionnaire") && Core.Left("Legionnaire", 8))
            {
                for (int i = 0; i < 20 && !Bot.ShouldExit; i++)
                {
                    if (!Bot.Player.Alive) break;
                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(boss);
                    Core.Cast(5);
                    Bot.Sleep(25);
                }
            }

            Bot.Sleep(100);
        }
    }

    public async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;

        if (!Bot.Player.Alive)
            return;

        dynamic data = packet["params"].dataObj;

        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();

        if (!string.IsNullOrEmpty(zoneSet))
        {
            int targetX =
                zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase)
                    ? 122
                    : zoneSet.Equals("B", StringComparison.OrdinalIgnoreCase)
                        ? 856
                        : 0;

            if (targetX != 0)
            {
                _ = Task.Run(() =>
                {
                    Bot.Player.WalkTo(targetX, 420);

                    Bot.Wait.ForTrue(
                        () => Math.Abs(Bot.Player.X - targetX) < 40,
                        10
                    );

                    Bot.Sleep(2000);
                });

                return;
            }
        }

        if (string.IsNullOrEmpty(zoneSet))
        {
            _ = Task.Run(() =>
            {
                Bot.Sleep(5000);

                int middleX = 500;

                Bot.Player.WalkTo(middleX, 420);

                Bot.Wait.ForTrue(
                    () => Math.Abs(Bot.Player.X - middleX) < 40,
                    10
                );
            });

            return;
        }
    }

    void DoEnh() => Enh.ApplyDage();
}



