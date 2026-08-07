/*
name: Army Farm Void Auras
description: Solo-farms Void Auras using custom NSoD-style methods so army sync can be added later without modifying CoreNSOD.
tags: void aura, nsoD, farm, solo, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/ArmyDependencies/ArmyGeneral.cs

using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyFarmVoidAuras
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreEnginev3 Engine => _Engine ??= new CoreEnginev3();
    private static CoreEnginev3 _Engine;
    private static CoreFarms Farm => _Farm ??= new CoreFarms();
    private static CoreFarms _Farm;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;

    public CoreUltrav3 Ultra = new();
    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyFarmVoidAuras";

    public List<IOption> Options = new()
    {
        new Option<int>("Quantity", "Void Aura Quantity", "Number of Void Auras to farm.", 7600),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself). Set to 1 for solo.", 4),
        new Option<bool>("UseArmySync", "Enable Army Sync", "Use army sync to coordinate Void Aura and essence progress.", true),
        new Option<bool>("EnableClassSync", "Enable Class Sync", "Auto-equip classes from the army class presets before farming starts.", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Apply UltraEnhancements for the equipped class before farming.", true),
        new Option<string>("Class1", "Class 1", "Preset class 1 to auto-equip before the fight. Use format: ClassName,Username.", "Chaos Avenger"),
        new Option<string>("Class2", "Class 2", "Preset class 2 to auto-equip before the fight. Use format: ClassName,Username.", "Chaos Avenger"),
        new Option<string>("Class3", "Class 3", "Preset class 3 to auto-equip before the fight. Use format: ClassName,Username.", "Chaos Avenger"),
        new Option<string>("Class4", "Class 4", "Preset class 4 to auto-equip before the fight. Use format: ClassName,Username.", "Chaos Avenger"),
        CoreBots.Instance.SkipOptions,
    };

    private readonly (string map, int? monsterID, string? monsterName, string essence)[] EssenceStages =
    {
        ("timespace", null, "Astral Ephemerite", "Astral Ephemerite Essence"),
        ("citadel", 21, "Belrot the Fiend", "Belrot the Fiend Essence"),
        ("greenguardwest", 22, "Black Knight", "Black Knight Essence"),
        ("mudluk", 18, "Tiger Leech", "Tiger Leech Essence"),
        ("aqlesson", 17, "Carnax", "Carnax Essence"),
        ("necrocavern", 5, "Chaos Vordred", "Chaos Vordred Essence"),
        ("hachiko", 10, "Dai Tengu", "Dai Tengu Essence"),
        ("timevoid", 12, "Unending Avatar", "Unending Avatar Essence"),
        ("dragonchallenge", 4, "Void Dragon", "Void Dragon Essence"),
        ("maul", 17, "Creature Creation", "Creature Creation Essence"),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        int quant = Bot.Config!.Get<int>("Quantity");
        int armySize = Bot.Config.Get<int>("ArmySize");
        bool useArmySync = Bot.Config.Get<bool>("UseArmySync");
        bool enableClassSync = Bot.Config.Get<bool>("EnableClassSync");

        if (useArmySync && armySize > 1)
            SetupArmy(armySize, enableClassSync);

        if (Bot.Config.Get<bool>("DoEnh"))
            DoEnhs();

        VoidAuras(quant, useArmySync);

        Core.SetOptions(false);
    }

    public void VoidAuras(int quant = 7600, bool useArmySync = false)
    {
        if (!useArmySync && Core.CheckInventory("Void Aura", quant))
            return;

        Core.AddDrop("Void Aura");
        Core.FarmingLogger("Void Aura", quant);
        RetrieveVoidAuras(quant, useArmySync);
    }

    public void RetrieveVoidAuras(int quant = 7600, bool useArmySync = false)
    {
        if (!useArmySync && Core.CheckInventory("Void Aura", quant))
            return;

        int essenceQuant = 100;
        string auraSyncFile = "ArmyFarmVoidAuras_VoidAura.sync";

        Farm.EvilREP();
        Core.AddDrop(EssenceStages.Select(s => s.essence).ToArray());
        if (!Core.CheckInventory("Necromancer", toInv: false))
            Bot.Drops.Add("Creature Shard");

        string stageSyncFile = "ArmyFarmVoidAuras_stage.sync";
        int armySize = Bot.Config!.Get<int>("ArmySize");

        if (useArmySync)
        {
            ArmyGeneral.PrepareSyncFiles(Ultra, auraSyncFile, EssenceStages.Select(s => s.essence), "ArmyFarmVoidAuras_Essence");
            ArmyGeneral.PrepareStageSync(Ultra, stageSyncFile);
        }

        while (!Bot.ShouldExit)
        {
            if (Core.CheckInventory("Void Aura", quant))
            {
                if (!useArmySync || Ultra.CheckArmyProgressBool(() => Core.CheckInventory("Void Aura", quant), auraSyncFile))
                    break;

                Core.Logger("Army members still farming. Helping until all are done.");
            }

            Core.EnsureAccept(4432);

            int ownStage = GetOwnStage(essenceQuant);
            int stageIndex = useArmySync
                ? ArmyGeneral.PublishAndGetArmyStage(Ultra, Bot, ownStage, stageSyncFile, armySize)
                : ownStage;

            if (useArmySync)
                Core.Logger($"Own stage {ownStage}, army stage {stageIndex}");

            if (stageIndex >= EssenceStages.Length)
            {
                int turnIns = GetMaxVoidAuraTurnIns();
                Core.Logger($"Turning in quest 4432 {turnIns} time(s) based on essence stacks.");
                if (turnIns > 0)
                {
                    Core.EnsureCompleteMulti(4432, turnIns);
                    Bot.Wait.ForPickup("Void Aura");
                }

                if (useArmySync)
                    Ultra.CheckArmyProgress("Void Aura", quant, false, auraSyncFile);

                Core.FarmingLogger("Void Aura", quant);
                continue;
            }

            var stage = EssenceStages[stageIndex];
            string syncFile = ArmyGeneral.GetSyncFileForItem("ArmyFarmVoidAuras_Essence", stage.essence);
            bool stageComplete = Core.CheckInventory(stage.essence, essenceQuant);
            bool isHelper = useArmySync && stageComplete;

            if (isHelper)
                Core.Logger($"Army helper active on {stage.essence}: {Bot.Inventory.GetQuantity(stage.essence)}/{essenceQuant}");
            else
                Core.Logger($"Army stage {stageIndex + 1}/{EssenceStages.Length}: {stage.essence}");

            while (!Bot.ShouldExit && !Ultra.CheckArmyProgressBool(() => Core.CheckInventory(stage.essence, essenceQuant), syncFile))
            {
                Core.FarmingLogger(stage.essence, essenceQuant);

                if (!string.IsNullOrEmpty(stage.monsterName))
                {
                    Core.Join(stage.map);
                    Bot.Wait.ForMapLoad(stage.map);
                    Engine.ChooseBestCell(stage.monsterName!);

                    if (isHelper)
                    {
                        Bot.Combat.Attack(stage.monsterName!);
                        Bot.Sleep(300);
                    }
                    else
                    {
                        Core.HuntMonster(
                            stage.map,
                            stage.monsterName!,
                            stage.essence,
                            essenceQuant,
                            false
                        );
                    }
                }
                else if (stage.monsterID.HasValue)
                {
                    if (isHelper)
                    {
                        Bot.Combat.Attack(stage.monsterID.Value);
                        Bot.Sleep(300);
                    }
                    else
                    {
                        Core.HuntMonsterMapID(
                            stage.map,
                            stage.monsterID.Value,
                            stage.essence,
                            essenceQuant,
                            false,
                            false,
                            false
                        );
                    }
                }
                else
                {
                    break;
                }

                Bot.Sleep(100);
            }
        }
    }

    private int GetOwnStage(int quant)
    {
        for (int i = 0; i < EssenceStages.Length; i++)
        {
            if (!Core.CheckInventory(EssenceStages[i].essence, quant))
                return i;
        }

        return EssenceStages.Length;
    }

    private int GetMaxVoidAuraTurnIns()
    {
        int minEssenceStacks = EssenceStages.Min(stage => Bot.Inventory.GetQuantity(stage.essence));
        return minEssenceStacks / 20;
    }

    private void SetupArmy(int armySize, bool enableClassSync)
    {
        string readySyncFile = "ArmyFarmVoidAuras.ready.sync";
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(readySyncFile));
        Bot.Sleep(2500);
        Core.Logger($"Waiting for army ready ({armySize - 1} other players)...");
        Ultra.WaitForArmy(armySize - 1, readySyncFile);

        if (enableClassSync)
        {
            Core.Logger("Starting army class sync...");
            ArmyGeneral.PrepareClassSync(Ultra, Bot, armySize, "ArmyFarmVoidAuras.class.sync");
        }
    }

    private void DoEnhs()
    {
        Core.Logger("Applying UltraEnhancements...");
        Enh.Apply();
    }

    private void HuntMonsterBatch(
        int quant,
        bool isTemp,
        bool publicRoom,
        bool log,
        params (string map, int monster, string essence)[] monsters
    )
    {
        Core.AddDrop(monsters.Select(x => x.essence).ToArray());
        Core.EquipClass(ClassType.Solo);
        foreach (var monster in monsters.Where(x =>
            x.essence != null && x.monster > 0 && !Core.CheckInventory(x.essence, quant)))
        {
            Core.HuntMonsterMapID(
                monster.map,
                monster.monster,
                monster.essence,
                quant,
                isTemp,
                log,
                publicRoom
            );
        }
    }
}
