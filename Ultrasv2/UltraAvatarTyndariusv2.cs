/*
name: UltraAvatarTyndariusv2
description: Ultra Avatar Tyndarius helper with taunter rotation and orb priority.
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
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class UltraAvatarTyndariusv2
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
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

    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    // Ball taunter roles
    bool isBall1Taunter;
    bool isBall2Killer;
    bool isTynTaunter;

    // Tyndarius timer-based taunt rotation
    private DateTime tynFightStartTime = DateTime.MinValue;
    private double tynTauntOffset = 0;
    private const double TynTauntInterval = 12.0; // 2 taunters × 6s slots
    private const double TynTauntWindow = 6.0;
    private DateTime tynLastTaunt = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius-v2";

    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>(
            "Ball1Taunter",
            "Ball 1 Taunter",
            "Class name that taunts Ball 1 (left orb).",
            "StoneCrusher"
        ),
        new Option<string>(
            "Ball2Killer",
            "Ball 2 Killer",
            "Class name that kills Ball 2 (right orb).",
            "King's Echo"
        ),
        new Option<string>(
            "TynTaunter1",
            "Tyndarius Taunter 1",
            "Class name of Tyndarius Taunter 1 (fires at 0s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),
        new Option<string>(
            "TynTaunter2",
            "Tyndarius Taunter 2",
            "Class name of Tyndarius Taunter 2 (fires at 5s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "Lord of Order"
        ),

        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. \nUse format: ClassName,Username. \nOnly type ClassName if you want it to be random.", "King's Echo"),

        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string s) => (s ?? "").Trim().ToLower();
    private bool HasAssignedClass(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        NormalizeString(Bot.Player.CurrentClass?.Name) == NormalizeString(name);

    public void ScriptMain(IScriptInterface bot)
    {
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

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "tyndarius_class-v2.sync");
    }

    void Prep()
    {
        EquipPresetClasses();

        string ball1  = (Bot.Config!.Get<string>("Ball1Taunter") ?? "").Trim();
        string ball2  = (Bot.Config!.Get<string>("Ball2Killer") ?? "").Trim();
        string tyn1   = (Bot.Config!.Get<string>("TynTaunter1") ?? "").Trim();
        string tyn2   = (Bot.Config!.Get<string>("TynTaunter2") ?? "").Trim();

        isBall1Taunter = HasAssignedClass(ball1);
        isBall2Killer  = HasAssignedClass(ball2);
        isTynTaunter   = HasAssignedClass(tyn1) || HasAssignedClass(tyn2);

        string cn = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (cn.Equals(tyn1, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 0;
        else if (cn.Equals(tyn2, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 5;
        else
            tynTauntOffset = 0;

        C.Logger($"Tyndarius — Ball1Taunter: {isBall1Taunter} | Ball2Killer: {isBall2Killer} | TynTaunter: {isTynTaunter} (offset {tynTauntOffset}s)");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        bool usePotions = Bot.Config!.Get<bool>("UsePotions");
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: isBall1Taunter || isTynTaunter);

        EquipPresetClasses();

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: isBall1Taunter || isTynTaunter, ensureStock: false);

        if (isBall1Taunter || isTynTaunter)
            Ultra.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map  = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("UltraAvatarTyndariusWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("UltraAvatarTyndariusWipeAlive.sync");
        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);
        Bot.Sleep(2500);

        C.AddDrop("Avatar Tyndarius Insignia");
        C.EnsureAccept(8245);
        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_tyndarius.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

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
                    tynFightStartTime = DateTime.MinValue;
                    tynLastTaunt = DateTime.MinValue;
                    armyWipeDetected = false;
                    C.Logger("Army wipe recovered — resetting Tyndarius timer and resuming fight.");
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
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Avatar Tyndarius Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8245);
                Core.DisableSkills();
                Ultra.JoinHouse();
                break;
            }

            if (Bot.Map.Name != map)
                Core.Join(map);

            if (Bot.Player.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
            }

            bool ball1Alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 1);
            bool ball2Alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 3);
            bool bothDead   = !ball1Alive && !ball2Alive;

            // ── Ball1 taunter / Ball2 killer roles ────────────────────────────────
            if (isBall1Taunter)
            {
                if (ball1Alive)
                {
                    AttackTarget(1);
                    for (int i = 0; i < 20 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        AttackTarget(1);
                        Core.Cast(5);
                        Bot.Sleep(25);
                    }
                }
                else if (ball2Alive)
                {
                    AttackTarget(3);
                    Bot.Sleep(250);
                }
                else
                {
                    AttackTarget(2);
                    Bot.Sleep(250);
                }
            }
            else if (isBall2Killer)
            {
                if (ball2Alive)
                {
                    AttackTarget(3);
                }
                else if (ball1Alive)
                {
                    AttackTarget(1);
                }
                else
                {
                    AttackTarget(2);
                    Bot.Sleep(250);
                }
            }

            // ── Tyndarius phase (timer-based, all non-ball roles + ball roles when balls dead) ──
            if (bothDead || (!isBall1Taunter && !isBall2Killer))
            {
                if (tynFightStartTime == DateTime.MinValue)
                {
                    tynFightStartTime = DateTime.Now;
                    C.Logger("Balls down — Tyndarius timer started.");
                }

                AttackTarget(2);

                if (isTynTaunter)
                {
                    TimeSpan elapsed = DateTime.Now - tynFightStartTime;
                    double t = elapsed.TotalSeconds;
                    double cycle = (t - tynTauntOffset) % TynTauntInterval;
                    bool inWindow = cycle >= 0 && cycle < TynTauntWindow;
                    bool cooled = (DateTime.Now - tynLastTaunt).TotalSeconds >= TynTauntInterval - 1;

                    if (inWindow && cooled)
                    {
                        tynLastTaunt = DateTime.Now;
                        C.Logger($"Tyndarius taunt ({t:F1}s, offset {tynTauntOffset}s)");

                        for (int i = 0; i < 60 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive) break;
                            AttackTarget(2);
                            Core.Cast(5);
                            Bot.Sleep(50);
                        }

                        C.Logger("Tyndarius taunt done.");
                        Bot.Sleep(100);
                    }
                    else
                    {
                        // Outside the taunt window, stay on Tyndarius and keep pressure on boss.
                        AttackTarget(2);
                        Bot.Sleep(250);
                    }
                }
                else
                {
                    Bot.Sleep(250);
                }
            }

            Pots.ActivateEquippedPotion();
            Bot.Sleep(100);
        }
    }

    void DoEnh() => Enh.Apply();

    private void AttackTarget(int mapID)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || Bot.Player.Target.MapID != mapID)
            Bot.Combat.Attack(mapID);
    }

    private void AttackTarget(string name)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || !Bot.Player.Target.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            Bot.Combat.Attack(name);
    }
}



