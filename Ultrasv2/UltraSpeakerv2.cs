/*
name: UltraSpeakerv2
description: Ultra First Speaker helper with zoning, taunt timing, and custom rotation.
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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Options;

public class UltraSpeakerv2
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
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    string? className = null;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraSpeaker-v2";

    // Timer-based taunt rotation
    private DateTime fightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double tauntIntervalSeconds = 12.0;
    private const double TauntWindowSeconds = 4.0;
    private DateTime lastTauntTime = DateTime.MinValue;

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>(
            "Taunter1",
            "Taunter 1 Class",
            "Class name of Taunter 1 (fires at 0s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "Lord of Order"
        ),
        new Option<string>(
            "Taunter2",
            "Taunter 2 Class",
            "Class name of Taunter 2 (fires at 4s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "StoneCrusher"
        ),
        new Option<string>(
            "Taunter3",
            "Taunter 3 Class",
            "Class name of Taunter 3 (fires at 8s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Verus DoomKnight"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),CoreBots.Instance.SkipOptions,
    };


    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions();
        C.Logger("This script uses the `corner spam taunt method.. and works ^_^");
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

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "speaker_class-v2.sync");
    }

    void Prep()
    {
        EquipPresetClasses();
        className = Bot.Player.CurrentClass?.Name?.ToLower();

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        string t1 = (Bot.Config!.Get<string>("Taunter1") ?? string.Empty).Trim();
        string t2 = (Bot.Config!.Get<string>("Taunter2") ?? string.Empty).Trim();
        string t3 = (Bot.Config!.Get<string>("Taunter3") ?? string.Empty).Trim();
        string cn = className ?? string.Empty;
        bool isTaunter = cn.Equals(t1, StringComparison.OrdinalIgnoreCase)
            || cn.Equals(t2, StringComparison.OrdinalIgnoreCase)
            || cn.Equals(t3, StringComparison.OrdinalIgnoreCase);

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: isTaunter);

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: isTaunter, ensureStock: false);

        // Assign taunt offset based on which slot this client fills
        if (cn.Equals(t1, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (cn.Equals(t2, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 4;
        else if (cn.Equals(t3, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 8;
        else
            tauntOffsetSeconds = 0; // fallback — still participates

        C.Logger($"Taunter [{(isTaunter ? "Yes" : "No")}] | Speaker taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        if (isTaunter)
            Ultra.GetScrollOfEnrage();
    }


    void Fight()
    {
        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Log("Ultra Quest isn't unlocked, we'll fakeunlock it so you can atleast get the quest drop");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        Bot.Options.DisableCollisions = true;
        C.EnsureAccept(9173);
        C.AddDrop("The First Speaker Silenced");
        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Quests.UpdateQuest(9125);
        Core.Join("ultraspeaker");
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_speaker.sync");
        Core.ChooseBestCell("The First Speaker");
        Core.EnableSkills();

        string wipeDeadSyncPath = Ultra.ResolveSyncPath("UltraSpeakerWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("UltraSpeakerWipeAlive.sync");
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);

        // Sync fight start time across all clients
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

            // Dead → wait for respawn
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                lastTauntTime = DateTime.MinValue;
                continue;
            }

            // Check if all players are done
            bool allComplete = Ultra.CheckArmyProgressBool(
                () => Bot.Inventory.Contains("The First Speaker Silenced", 1),
                syncPath
            );

            if (allComplete)
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsDailyComplete(9173))
                {
                    C.Logger("Weekly already complete, try again Friday morning");
                    if (Bot.Config!.Get<bool>("DoEnh"))
                        return;
                }
                else C.EnsureComplete(9173);
                Ultra.JoinHouse();
                break;
            }

            // If we're petrified, wait
            if (Bot.Self.Auras.Any(a => a.Name == "Stasis"))
            {
                Core.DisableSkills();
                Bot.Wait.ForTrue(() => !Bot.Self.Auras.Any(a => a.Name == "Stasis"), 20);
                Core.EnableSkills();
                continue;
            }

            // Position management in Boss cell
            if (Bot.Player?.Cell == "Boss")
            {
                int minX = 0, maxX = 100;
                int minY = 485, maxY = 500;

                bool isInBox =
                    Bot.Player.Position.X >= minX &&
                    Bot.Player.Position.X <= maxX &&
                    Bot.Player.Position.Y >= minY &&
                    Bot.Player.Position.Y <= maxY;

                if (!isInBox)
                {
                    Random rand = new();
                    int randomX = rand.Next(minX, maxX + 1);
                    int randomY = rand.Next(minY, maxY + 1);
                    Bot.Player.WalkTo(randomX, randomY);
                    Bot.Sleep(500);
                }
            }

            // Combat logic - only attack if monster exists
            if (Bot.Monsters.CurrentMonsters?.Any(m => m != null && m.Name == "The First Speaker" && m.Alive) == true)
            {
                Bot.Combat.Attack("The First Speaker");

                Pots.ActivateEquippedPotion();

                // Timer-based taunt rotation — only for taunters
                string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
                string t1 = (Bot.Config!.Get<string>("Taunter1") ?? string.Empty).Trim();
                string t2 = (Bot.Config!.Get<string>("Taunter2") ?? string.Empty).Trim();
                string t3 = (Bot.Config!.Get<string>("Taunter3") ?? string.Empty).Trim();
                bool isTaunter = cn.Equals(t1, StringComparison.OrdinalIgnoreCase)
                    || cn.Equals(t2, StringComparison.OrdinalIgnoreCase)
                    || cn.Equals(t3, StringComparison.OrdinalIgnoreCase);

                if (isTaunter && Bot.Player?.HasTarget == true)
                {
                    TimeSpan timeSinceFightStart = DateTime.Now - fightStartTime;
                    double currentTime = timeSinceFightStart.TotalSeconds;
                    double timeInCycle = (currentTime - tauntOffsetSeconds) % tauntIntervalSeconds;
                    bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
                    bool cooldownExpired = (DateTime.Now - lastTauntTime).TotalSeconds >= tauntIntervalSeconds - 1;

                    if (inTauntWindow && cooldownExpired)
                    {
                        lastTauntTime = DateTime.Now;
                        Bot.Combat.Attack("The First Speaker");
                        C.Logger($"Taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s)");
                        C.Logger($"{className} executing taunt!");

                        for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive)
                                break;
                            if (!Bot.Player.HasTarget)
                                Bot.Combat.Attack("The First Speaker");
                            Core.Cast(5);
                            Bot.Sleep(50);
                        }

                        C.Logger("Taunt done — 60 presses complete.");
                        Bot.Sleep(100);
                    }
                }
            }
            Bot.Sleep(100);
        }
    }

    void DoEnh() => Enh.ApplyNoVainglory();

}



