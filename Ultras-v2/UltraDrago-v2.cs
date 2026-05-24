/*
name: UltraDrago
description: Ultra King Drago helper with class-based taunters and UltraPotions support.
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
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDrago
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

    public CoreEngine Core = new();

    public CoreUltra Ultra = new();

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

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("Ultra Drago loaded.");

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

        C.Join("whitemap");

        Astravia.AstraviaJudgement();

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

        C.Logger($"Taunter [{(isPrimaryTaunter || isSecondaryTaunter ? "Yes" : "No")}] | Drago taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");

        Pots.UseRecommendedPotions(potionQuant, skipThird: isPrimaryTaunter || isSecondaryTaunter);

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

        Ultra.ClearSyncFile(syncPath);

        Bot.Sleep(2500);

        if (!Bot.Quests.IsUnlocked(8397))
            Bot.Quests.UpdateQuest(8395);

        C.AddDrop("King Drago Insignia");

        Core.Join(map);

        C.EnsureAccept(8397);

        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_drago.sync");

        Core.ChooseBestCell(boss);

        Bot.Player.SetSpawnPoint();

        Core.EnableSkills();

        // Sync fight start time
        fightStartTime = DateTime.Now;
        C.Logger($"Fight start time synced: {fightStartTime:HH:mm:ss.fff}");

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
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

                            for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                            {
                                if (!Bot.Player.Alive) break;
                                if (!Bot.Player.HasTarget)
                                    Bot.Combat.Attack(leftSummon);
                                Bot.Skills.UseSkill(5);
                                Bot.Sleep(25);
                            }

                            C.Logger("Taunt done — 60 presses complete.");
                            Bot.Sleep(300);
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

            Bot.Sleep(250);
        }
    }

    void DoEnhs() => Enh.Apply();
}