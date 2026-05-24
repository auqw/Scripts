/*
name: UltraAvatarTyndarius
description: Ultra Avatar Tyndarius helper with taunter rotation and orb priority.
tags: Ultra
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraPotions.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraEnhancements.cs
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

public class UltraAvatarTyndarius
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

    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    // Ball taunter roles
    bool isBall1Taunter;
    bool isBall2Killer;

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
        new Option<string>(
            "Ball1Taunter",
            "Ball 1 Taunter",
            "Class name that taunts Ball 1 (left orb).",
            "Legion Revenant"
        ),
        new Option<string>(
            "Ball2Killer",
            "Ball 2 Killer",
            "Class name that kills Ball 2 (right orb).",
            "Chaos Avenger"
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
            "Class name of Tyndarius Taunter 2 (fires at 6s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "Lord Of Order"
        ),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string s) => (s ?? "").Trim().ToLower();
    private bool HasAssignedClass(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        NormalizeString(Bot.Player.CurrentClass?.Name) == NormalizeString(name);

    public void ScriptMain(IScriptInterface bot)
    {
        C.Join("whitemap");

        if (Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions))
            Bot.Config.Configure();

        C.SetOptions();
        Core.Boot();
        Prep();
        Fight();
        C.SetOptions(false);
        Bot.StopSync();
    }

    void Prep()
    {
        string ball1  = (Bot.Config!.Get<string>("Ball1Taunter") ?? "").Trim();
        string ball2  = (Bot.Config!.Get<string>("Ball2Killer") ?? "").Trim();
        string tyn1   = (Bot.Config!.Get<string>("TynTaunter1") ?? "").Trim();
        string tyn2   = (Bot.Config!.Get<string>("TynTaunter2") ?? "").Trim();

        isBall1Taunter = HasAssignedClass(ball1);
        isBall2Killer  = HasAssignedClass(ball2);

        bool isTynTaunter = HasAssignedClass(tyn1) || HasAssignedClass(tyn2);

        string cn = Bot.Player.CurrentClass?.Name ?? string.Empty;
        if (cn.Equals(tyn1, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 0;
        else if (cn.Equals(tyn2, StringComparison.OrdinalIgnoreCase))
            tynTauntOffset = 6;

        C.Logger($"Tyndarius — Ball1: {isBall1Taunter} | Ball2Kill: {isBall2Killer} | TynTaunt: {isTynTaunter} (offset {tynTauntOffset}s)");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        Pots.UseRecommendedPotions(potionQuant, skipThird: isBall1Taunter || isTynTaunter);

        if (isBall1Taunter || isTynTaunter)
            Ultra.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map  = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        string tyn1 = (Bot.Config!.Get<string>("TynTaunter1") ?? "").Trim();
        string tyn2 = (Bot.Config!.Get<string>("TynTaunter2") ?? "").Trim();
        string cn   = Bot.Player.CurrentClass?.Name ?? string.Empty;
        bool isTynTaunter = cn.Equals(tyn1, StringComparison.OrdinalIgnoreCase) || cn.Equals(tyn2, StringComparison.OrdinalIgnoreCase);

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.AddDrop("Avatar Tyndarius Insignia");
        C.EnsureAccept(8245);
        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_tyndarius.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
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

            // ── Ball phase ────────────────────────────────────────────────────
            if (isBall1Taunter)
            {
                if (bothDead)
                {
                    // Balls dead — fall through to Tyndarius taunt below
                }
                else if (ball1Alive)
                {
                    Bot.Combat.Attack(1);
                    for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        if (!Bot.Player.HasTarget) Bot.Combat.Attack(1);
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(25);
                    }
                }
                else if (ball2Alive)
                {
                    Bot.Combat.Attack(3);
                    Bot.Sleep(500);
                }
            }
            else if (isBall2Killer)
            {
                if (ball2Alive)
                {
                    Bot.Combat.Attack(3);
                    for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        if (!Bot.Player.HasTarget) Bot.Combat.Attack(3);
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(25);
                    }
                }
                else if (ball1Alive)
                {
                    Bot.Combat.Attack(1);
                    Bot.Sleep(500);
                }
                // if both dead fall through to Tyndarius below
            }

            // ── Tyndarius phase (timer-based, all non-ball roles + ball roles when balls dead) ──
            if (bothDead || (!isBall1Taunter && !isBall2Killer))
            {
                if (tynFightStartTime == DateTime.MinValue)
                {
                    tynFightStartTime = DateTime.Now;
                    C.Logger("Balls down — Tyndarius timer started.");
                }

                Bot.Combat.Attack(2);

                if (isTynTaunter && Bot.Player.HasTarget)
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

                        for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive) break;
                            if (!Bot.Player.HasTarget) Bot.Combat.Attack(2);
                            Bot.Skills.UseSkill(5);
                            Bot.Sleep(25);
                        }

                        C.Logger("Tyndarius taunt done.");
                        Bot.Sleep(300);
                    }
                    else if (ball2Alive)
                    {
                        // Outside taunt window — help kill Ball 2
                        Bot.Combat.Attack(3);
                        Bot.Sleep(500);
                    }
                    else if (ball1Alive)
                    {
                        // Ball 2 dead — help kill Ball 1
                        Bot.Combat.Attack(1);
                        Bot.Sleep(500);
                    }
                    else
                    {
                        Bot.Sleep(500);
                    }
                }
                else
                {
                    Bot.Sleep(500);
                }
            }

            Pots.ActivateEquippedPotion();

            if (Bot.ShouldExit)
                C.Jump("Enter", "Spawn");
        }
    }

    void DoEnh() => Enh.Apply();
}
