/*
name: UltraDarkon
description: Ultra Darkon with class-based taunters and UltraPotions support.
tags: ultra, darkon, taunt, Ultra Darkon, ultra darkon
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
using Skua.Core.Options;

public class UltraDarkon
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

    public CoreEngine Core = new();

    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;

    public string OptionsStorage = "UltraDarkon-v2";

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
        new Option<string>(
            "PrimaryTaunter",
            "Primary Taunter Class",
            "Class name of the primary taunter (fires at 0s).",
            "StoneCrusher"
        ),

        new Option<string>(
            "SecondaryTaunter",
            "Secondary Taunter Class",
            "Class name of the secondary taunter (fires at 6s).",
            "Lord Of Order"
        ),

        new Option<bool>(
            "DoEnh",
            "Do Enhancements",
            "Auto-Enhance Gear properly for the fight",
            true
        ),

        new Option<int>(
            "PotionQuantity",
            "Potion Quantity",
            "How many potions to keep stocked.",
            10
        ),

        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),

        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string input) =>
        (input ?? "").Trim().ToLower();

    bool HasAssignedClass(string assignedClass) =>
        NormalizeString(Bot.Player.CurrentClass?.Name)
        == NormalizeString(assignedClass);

    bool IsTaunter() => isPrimaryTaunter || isSecondaryTaunter;

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("Ultra Darkon loaded.");

        C.SetOptions();

        Core.Boot();

        Prep();

        Fight();

        C.SetOptions(false);

        Bot.StopSync();
    }

    void Prep()
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        string primaryTaunter =
            Bot.Config!.Get<string>("PrimaryTaunter");

        string secondaryTaunter =
            Bot.Config!.Get<string>("SecondaryTaunter");

        isPrimaryTaunter = HasAssignedClass(primaryTaunter);

        isSecondaryTaunter = HasAssignedClass(secondaryTaunter);

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        // Assign taunt offset after enhancements so the class is final
        string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (!string.IsNullOrEmpty(primaryTaunter) && cn.Equals(primaryTaunter, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (!string.IsNullOrEmpty(secondaryTaunter) && cn.Equals(secondaryTaunter, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 6;
        else
            tauntOffsetSeconds = 0; // non-taunter, offset irrelevant

        C.Logger($"Taunter [{(IsTaunter() ? "Yes" : "No")}] | Darkon taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), context: "Darkon");

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();

        Bot.Sleep(2500);
    }

    void Fight()
    {
        const string boss = "Darkon the Conductor";

        if (!C.isCompletedBefore(8733))
        {
            C.Logger("Quest 8733 (\"The World\") not completed.");
            Bot.Quests.UpdateQuest(8733);
        }

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");

        Ultra.ClearSyncFile(syncPath);

        Bot.Sleep(2500);

        C.EnsureAccept(8746);

        C.AddDrop("Darkon Insignia");

        Core.Join("ultradarkon");
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "Ultra_Darkon.sync");

        Core.ChooseBestCell(boss);

        Core.EnableSkills();

        // Sync fight start time
        fightStartTime = DateTime.Now;
        C.Logger($"Fight start time synced: {fightStartTime:HH:mm:ss.fff}");

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Darkon the Conductor Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8746);
                Bot.Wait.ForPickup("Darkon Insignia");
                Ultra.JoinHouse();
                break;
            }

            Bot.Combat.Attack(boss);

            // Timer-based taunt rotation — only for taunters
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

                    for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                    {
                        if (!Bot.Player.Alive) break;
                        if (!Bot.Player.HasTarget)
                            Bot.Combat.Attack(boss);
                        Bot.Skills.UseSkill(5);
                        Bot.Sleep(25);
                    }

                    C.Logger("Taunt done — 60 presses complete.");
                    Bot.Sleep(300);
                }
            }

            Pots.ActivateEquippedPotion();

            Bot.Sleep(250);
        }
    }

    void DoEnhs() => Enh.Apply();
}