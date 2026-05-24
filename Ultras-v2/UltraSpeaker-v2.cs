/*
name: UltraSpeaker
description: Ultra First Speaker helper with zoning, taunt timing, and custom rotation.
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
using System.Dynamic;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Options;

public class UltraSpeaker
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
    string? className = null;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraSpeaker-v2";

    // Timer-based taunt rotation
    private DateTime fightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 16.0; // 4 taunters × 4s slots
    private const double TauntWindowSeconds = 4.0;
    private DateTime lastTauntTime = DateTime.MinValue;

    public List<IOption> Options = new()
    {
        new Option<string>(
            "Taunter1",
            "Taunter 1 Class",
            "Class name of Taunter 1 (fires at 0s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),
        new Option<string>(
            "Taunter2",
            "Taunter 2 Class",
            "Class name of Taunter 2 (fires at 4s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "Lord Of Order"
        ),
        new Option<string>(
            "Taunter3",
            "Taunter 3 Class",
            "Class name of Taunter 3 (fires at 8s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "StoneCrusher"
        ),
        new Option<string>(
            "Taunter4",
            "Taunter 4 Class",
            "Class name of Taunter 4 (fires at 12s).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "Void Highlord"
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        CoreBots.Instance.SkipOptions,
    };


    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        C.SetOptions();
        C.Logger("This script uses the `corner spam taunt method.. and works ^_^");
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.Boot();
        Prep();
        Fight();
        C.SetOptions(false);
        Bot.StopSync();
    }

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        // Assign taunt offset based on which slot this client fills
        string t1 = Bot.Config!.Get<string>("Taunter1").Trim();
        string t2 = Bot.Config!.Get<string>("Taunter2").Trim();
        string t3 = Bot.Config!.Get<string>("Taunter3").Trim();
        string t4 = Bot.Config!.Get<string>("Taunter4").Trim();

        string cn = className ?? string.Empty;
        if (cn.Equals(t1, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (cn.Equals(t2, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 4;
        else if (cn.Equals(t3, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 8;
        else if (cn.Equals(t4, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 12;
        else
            tauntOffsetSeconds = 0; // fallback — still participates

        bool isTaunter = cn.Equals(t1, StringComparison.OrdinalIgnoreCase) || cn.Equals(t2, StringComparison.OrdinalIgnoreCase) || cn.Equals(t3, StringComparison.OrdinalIgnoreCase) || cn.Equals(t4, StringComparison.OrdinalIgnoreCase);
        C.Logger($"Taunter [{(isTaunter ? "Yes" : "No")}] | Speaker taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        Pots.UseRecommendedPotions(potionQuant, skipThird: isTaunter);

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

        // Sync fight start time across all clients
        fightStartTime = DateTime.Now;
        C.Logger($"Fight start time synced: {fightStartTime:HH:mm:ss.fff}");

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
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
            if (Bot.Monsters.CurrentMonsters.Any(m => m.Name == "The First Speaker" && m.Alive))
            {
                Bot.Combat.Attack("The First Speaker");

                Pots.ActivateEquippedPotion();

                // Timer-based taunt rotation — only for taunters
                string cn = Bot.Player.CurrentClass?.Name ?? string.Empty;
                string t1 = Bot.Config!.Get<string>("Taunter1").Trim();
                string t2 = Bot.Config!.Get<string>("Taunter2").Trim();
                string t3 = Bot.Config!.Get<string>("Taunter3").Trim();
                string t4 = Bot.Config!.Get<string>("Taunter4").Trim();
                bool isTaunter = cn.Equals(t1, StringComparison.OrdinalIgnoreCase) || cn.Equals(t2, StringComparison.OrdinalIgnoreCase) || cn.Equals(t3, StringComparison.OrdinalIgnoreCase) || cn.Equals(t4, StringComparison.OrdinalIgnoreCase);

                if (isTaunter && Bot.Player.HasTarget)
                {
                    TimeSpan timeSinceFightStart = DateTime.Now - fightStartTime;
                    double currentTime = timeSinceFightStart.TotalSeconds;
                    double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;
                    bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
                    bool cooldownExpired = (DateTime.Now - lastTauntTime).TotalSeconds >= TauntIntervalSeconds - 1;

                    if (inTauntWindow && cooldownExpired)
                    {
                        lastTauntTime = DateTime.Now;
                        Bot.Combat.Attack("The First Speaker");
                        C.Logger($"Taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s)");
                        C.Logger($"{className} executing taunt!");

                        for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                        {
                            if (!Bot.Player.Alive)
                                break;
                            if (!Bot.Player.HasTarget)
                                Bot.Combat.Attack("The First Speaker");
                            Bot.Skills.UseSkill(5);
                            Bot.Sleep(25);
                        }

                        C.Logger("Taunt done — 60 presses complete.");
                        Bot.Sleep(300);
                    }
                }
            }
            Bot.Sleep(500);
        }
    }

    void DoEnh() => Enh.ApplySpeaker();

}
