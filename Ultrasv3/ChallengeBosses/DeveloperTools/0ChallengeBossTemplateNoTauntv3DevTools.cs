/*
name: ChallengeBossTemplateNoTaunt
description: Challenge Boss Template v3 — no-taunt fight flow with synced class equip. Copy and adapt for specific bosses.
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

public class ChallengeBossTemplateNoTaunt
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
    public string OptionsStorage = "ChallengeBossTemplateNoTaunt";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "Lord of Order"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", "King's Echo"),
        new Option<string>("Class5", "Class 5", "Preset class 5 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class6", "Class 6", "Preset class 6 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
        new Option<string>("Class7", "Class 7", "Preset class 7 to auto-equip before the fight.\nUse format: ClassName,Username.\nOnly type ClassName if you want it to be random.", ""),
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
            Prep();
            Fight();
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
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "challengeboss_template_class-v3.sync");
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        usePotions = Bot.Config!.Get<bool>("UsePotions");

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
        {
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: false);
            Pots.UseRecommendedPotions(potionQuant, skipThird: false, ensureStock: false);
        }

        Bot.Sleep(2500);
    }

    void DoEnhs() => Enh.Apply();

    private void Fight()
    {
        const string map = "changemap";
        const string boss = "Change Boss Name";
        const string bossDefeatedTemp = "Change Boss Defeated Temp";

        const string waitSyncFile = "challengeboss_template.sync";
        const string completionSyncFile = "ChallengeBossCompletion.sync";
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));

        
        const int questId = 0;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        if (usePotions)
            Pots.EnsureRecommendedPotions(potionQuant, skipThird: false);

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        if (usePotions)
            Pots.UseRecommendedPotions(potionQuant, skipThird: false, ensureStock: false);

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all 4 entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

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

            // Default — attack the boss
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            if (usePotions)
                Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
