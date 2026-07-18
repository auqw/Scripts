/*
name: Flibbitiestgibbet
description: Ultra Flibbitiestgibbet helper for army farming voidflibbi.
tags: Ultra, Flibbitiestgibbet, voidFlibbitiestgibbet
*/

//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreStory2.cs
using System;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Flibbitiestgibbet
{
    private static CoreAdvanced2 Adv
    {
        get => _Adv ??= new CoreAdvanced2();
        set => _Adv = value;
    }
    private static CoreAdvanced2 _Adv;

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

    private CoreBots2 C => CoreBots2.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine2 Core = new();
    public CoreUltra2 Ultra = new();

    bool usePotions;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Flibbitiestgibbet-v3";
    public List<IOption> Options = new()
    {
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", "ArchPaladin"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", "StoneCrusher"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", "Lord Of Order"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", "King's Echo"),
        new Option<string>("Class5", "Class 5", "Preset class 5 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", ""),
        new Option<string>("Class6", "Class 6", "Preset class 6 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", ""),
        new Option<string>("Class7", "Class 7", "Preset class 7 to auto-equip before the fight.\nUse format: ClassName,Username. Only type ClassName if you want it to be random.", ""),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("UsePotions", "Use Potions", "Enable buying and consuming recommended potions.", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        CoreBots2.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(disableCoreSkills: true);
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
        UltraGeneral.EquipPresetClasses(Ultra, Bot, "flibbi_class-v3.sync");
    }

    void Prep()
    {
        EquipPresetClasses();

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

    private bool HasBossTarget(string boss) =>
        Bot.Player.HasTarget &&
        string.Equals(Bot.Player.Target?.Name, boss, StringComparison.OrdinalIgnoreCase);

    void Fight()
    {
        const string map = "voidflibbi";
        const string boss = "Flibbitiestgibbet";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        string wipeDeadSyncPath = Ultra.ResolveSyncPath("FlibbitiestgibbetWipeDead.sync");
        string wipeAliveSyncPath = Ultra.ResolveSyncPath("FlibbitiestgibbetWipeAlive.sync");
        Ultra.ClearSyncFile(syncPath);
        Ultra.ClearSyncFile(wipeDeadSyncPath);
        Ultra.ClearSyncFile(wipeAliveSyncPath);
        Bot.Sleep(2500);

        C.EnsureAcceptmultiple(8653, 9418, 7713);
        C.AddDrop("Flibbitiestgibbet's ??? Essence", "Insatiable Hunger", "Chest Plate", "Starlit Journal Page 3 Scraps");

        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "Flibbitiestgibbet.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        C.Logger("Fight start synced.");

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
                    armyWipeDetected = false;
                    C.Logger("Army wipe recovered — resuming fight.");
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

            if (Ultra.CheckArmyProgressBool(() => InventoryHasItems(), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);
                Ultra.JoinHouse();
                break;
            }

            if (!HasBossTarget(boss))
                Bot.Combat.Attack(boss);

            if (usePotions)
                Pots.ActivateEquippedPotion();

            Bot.Sleep(100);
        }
    }

    bool InventoryHasItems()
    {
        if (selectedItem == ItemToFarm.All)
        {
            return Enum.GetValues<ItemToFarm>()
                .Where(i => i != ItemToFarm.All)
                .All(i => Bot.Inventory.Contains((int)i, GetQuantity(i)));
        }

        return Bot.Inventory.Contains((int)selectedItem, GetQuantity(selectedItem));
    }

    int GetQuantity(ItemToFarm item) => item == ItemToFarm.Void_Essentia ? 1 : Bot.Config!.Get<int>("Quantity");
    private ItemToFarm selectedItem => Bot.Config!.Get<ItemToFarm>("Item");

    void DoEnhs() => Enh.Apply();

    public enum ItemToFarm
    {
        All = 0,
        Flibbitiestgibbets_Essence = 73861,
        Flibbitigiblets = 70054,
        Void_Essentia = 73355,
        Chest_Plate = 40066
    }
}
