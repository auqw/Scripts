/*
name: Kolrv3
description: 2 man Kolr Scripts, uses KE LoO. Don't change the classes, KE LoO has a specific skillset for this boss.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Kolrv3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static string _fbsMuteFile = "";

    bool usePotions;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Kolrv3";
    public string[] MultiOptions = { "SelectDrops" };
    public List<IOption> SelectDrops = new()
    {
        new Option<bool>("GreatFlameOfYew", "Great Flame of Yew", "Farm Great Flame of Yew from Kolr.", false),
        new Option<int>("GreatFlameOfYewQuant", "Great Flame of Yew Quantity", "How many Great Flame of Yew to farm per player.", 1),
    };
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 2),
        new Option<bool>("DoDailyOnly", "Do Daily Only", "True: do the daily quest only. False: farm selected drops instead.", true),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "King's Echo"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }
        Engine.Boot();

        try
        {
            bool dailyOnly = DoDailyOnly;

            if (dailyOnly)
            {
                // Daily mode: single run, complete quest, done
                Prep();
                Fight();
            }
            else
            {
                // Drop-farm mode: cycle kills until drops collected
                if (Bot.Config!.Get<bool>("SelectDrops", "GreatFlameOfYew"))
                    C.AddDrop("Great Flame of Yew");

                while (!Bot.ShouldExit && !HasSelectedDrops())
                {
                    int current = Bot.Inventory.GetQuantity("Great Flame of Yew");
                    int target = Bot.Config!.Get<int>("SelectDrops", "GreatFlameOfYewQuant");
                    C.Logger($"Great Flame of Yew: {current}/{target}");

                    Prep();
                    Fight(); // kills boss once, syncs army, joins house

                    // Re-enable skills for next cycle (Fight disables them on boss kill)
                    Engine.EnableSkills();
                    Bot.Sleep(1000);
                }

                if (!Bot.ShouldExit)
                    C.Logger("All selected drops collected. Farm complete.");
            }
        }
        finally
        {
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private void EquipPresetClasses()
    {
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "kolrv3_class-v3.sync");
    }

    private bool DoDailyOnly => Bot.Config!.Get<bool>("DoDailyOnly");

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        usePotions = Bot.Config!.Get<bool>("UsePotions");

        if (Bot.Config!.Get<bool>("DoEnh"))
            Enh.ApplyKolr();

        // Register drops for SelectDrops if not doing daily-only
        if (!DoDailyOnly)
        {
            if (Bot.Config!.Get<bool>("SelectDrops", "GreatFlameOfYew"))
                C.AddDrop("Great Flame of Yew");
        }

        Bot.Sleep(2500);
    }

    /// <summary>
    /// Returns true when all enabled SelectDrops targets are met.
    /// </summary>
    private bool HasSelectedDrops()
    {
        if (Bot.Config!.Get<bool>("SelectDrops", "GreatFlameOfYew"))
        {
            int quant = Bot.Config!.Get<int>("SelectDrops", "GreatFlameOfYewQuant");
            if (!C.CheckInventory("Great Flame of Yew", quant))
                return false;
        }
        return true;
    }

    private void Fight()
    {
        const string map = "flameusurper";
        const string boss = "Kolr, Usurper of Flames";
        const string bossDefeatedTemp = "Choronzonite";

        const string waitSyncFile = "kolrv3.sync";
        const string completionSyncFile = "Kolrv3Completion.sync";
        const string killSyncFile = "Kolrv3Kill.sync";
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));

        
        const int questId = 10715;

        bool dailyOnly = DoDailyOnly;

        if (dailyOnly)
        {
            if (!UltraGeneral.IsQuestGreen(Bot, questId))
                UltraGeneral.EnsureAcceptOnce(Bot, questId);
        }
        else
        {
            // Drop mode: fresh kill sync file each cycle
            Ultra.ClearSyncFile(Ultra.ResolveSyncPath(killSyncFile));
        }

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: false, context: "Kolr");

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: false, context: "Kolr", ensureStock: false);

        C.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCellOneMonster(boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        bool bossWasEngaged = false;

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (dailyOnly)
            {
                // Daily mode: check for boss temp item, complete quest
                if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
                {
                    C.Logger("Boss defeated. Finishing quest.");
                    Engine.DisableSkills();
                    Engine.Join(map);
                    Ultra.PersistentJoinHouse();
                    UltraGeneral.CompleteQuest(Bot, questId);
                    Bot.Sleep(3000);
                    break;
                }
            }
            else
            {
                // Drop-farm mode: detect boss death by target state
                // Mark engaged once we've attacked the boss
                if (Bot.Player.HasTarget && Bot.Player.Target?.Name == boss)
                    bossWasEngaged = true;

                // Boss died: target lost after having been engaged
                if (bossWasEngaged && (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0))
                {
                    // Signal this player's kill to the sync file
                    string key = $"{Bot.Player.Username}|kill";
                    Ultra.UpdateEntry(Ultra.ResolveSyncPath(killSyncFile), key, "1");

                    // Check if all army members have signaled the kill
                    string[] lines = Ultra.ReadLines(Ultra.ResolveSyncPath(killSyncFile));
                    int killCount = lines.Count(l => l.Contains(":1:"));
                    C.Logger($"Boss kill signal: {killCount}/{armySize}");

                    if (killCount >= armySize)
                    {
                        C.Logger("All players confirmed boss kill. Retreating to house.");
                        Engine.DisableSkills();
                        Engine.Join(map);
                        Ultra.PersistentJoinHouse();
                        Bot.Sleep(3000);
                        break;
                    }
                }
            }

            // Attack the boss if not on cooldown
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            if (usePotions)
                Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
