/*
name: UltraDage
description: Two-taunter strategy for Ultra Dage with timer-based taunting and army synchronization.
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

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UltraDage
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

    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

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
    private const double TauntIntervalSeconds = 12.0; // 2 taunters × 6s slots
    private const double TauntWindowSeconds = 6.0;
    private DateTime lastTauntTime = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage-v2";

    public List<IOption> Options = new()
    {
        new Option<string>(
            "a",
            "Primary Taunter Class",
            "Class name of the primary taunter (fires at 0s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),

        new Option<string>(
            "b",
            "Secondary Taunter Class",
            "Class name of the secondary taunter (fires at 6s).\nName must be exact including punctuation, spelling, and capitalisation.",
            "Lord Of Order"
        ),

        new Option<string>(
            "primaryDecayer",
            "Primary Decayer Class",
            "Class name of the primary decayer.",
            "Legion Revenant"
        ),

        new Option<string>(
            "secondaryDecayer",
            "Secondary Decayer Class",
            "Class name of the secondary decayer.",
            "Void Highlord"
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

    public void ScriptMain(IScriptInterface bot)
    {
        C.Join("whitemap");

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();
        primaryDecayer = (Bot.Config.Get<string>("primaryDecayer") ?? "").Trim();
        secondaryDecayer = (Bot.Config.Get<string>("secondaryDecayer") ?? "").Trim();

        C.SetOptions();
        Core.Boot();

        Prep();

        Bot.Events.ExtensionPacketReceived += UltraDageListener;

        Fight();

        Bot.Events.ExtensionPacketReceived -= UltraDageListener;

        C.SetOptions(false);
        Bot.StopSync();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);
    bool IsPrimaryDecayer() => Core.HasClassEquipped(primaryDecayer);
    bool IsSecondaryDecayer() => Core.HasClassEquipped(secondaryDecayer);

    void Prep()
    {
        if (!C.isCompletedBefore(793))
            C.Logger("Player is not part of the Legion — quest turn-in may not be available, but the kill will still proceed.");

        Bot.Quests.UpdateQuest(793);

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();

        // Assign taunt offset after enhancements so the class is final
        string cn = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (!string.IsNullOrEmpty(a) && cn.Equals(a, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 0;
        else if (!string.IsNullOrEmpty(b) && cn.Equals(b, StringComparison.OrdinalIgnoreCase))
            tauntOffsetSeconds = 6;
        else
            tauntOffsetSeconds = 0; // non-taunter, offset irrelevant

        C.Logger($"Taunter [{(IsTaunter() ? "Yes" : "No")}] | Dage taunt offset: {tauntOffsetSeconds}s (class: {cn})");

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        Pots.UseRecommendedPotions(potionQuant, skipThird: IsTaunter(), context: "Dage");

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

        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.AddDrop("Dage the Evil Insignia");
        C.EnsureAccept(8547);

        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_dage.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        // Sync fight start time
        fightStartTime = DateTime.Now;
        C.Logger($"Fight start time synced: {fightStartTime:HH:mm:ss.fff}");

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
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

            C.Logger($"[DEBUG] Map: {Bot.Map.Name} | Target: {Bot.Player.Target?.Name ?? "none"}");

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

            // Decayers
            if (IsPrimaryDecayer() && Core.HasAura("Legionnaire"))
            {
                if (Bot.Skills.CanUseSkill(5))
                {
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(200);
                }
            }

            if (IsSecondaryDecayer() && Core.HasAura("Legionnaire") && Core.Left("Legionnaire", 8))
            {
                if (Bot.Skills.CanUseSkill(5))
                {
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(200);
                }
            }

            Bot.Sleep(250);
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
