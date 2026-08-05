/*
name: Army Farm Void Auras v2
description: Army-farms Void Auras with shared private room support and helper mode so finished accounts keep attacking to speed up the rest of the army.
tags: void aura, voidaura, va, nsoD, farm, army, sync
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Army/ArmyDependencies/ArmyGeneral.cs
//cs_include Scripts/Army/CoreArmyLite.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyFarmVoidAurasv2
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreEnginev3 Engine => _Engine ??= new CoreEnginev3();
    private static CoreEnginev3 _Engine;
    private static CoreFarms Farm => _Farm ??= new CoreFarms();
    private static CoreFarms _Farm;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static CoreArmyLite Army => _Army ??= new CoreArmyLite();
    private static CoreArmyLite _Army;

    public CoreUltrav3 Ultra = new();
    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyFarmVoidAurasv2";

    public List<IOption> Options = new()
    {
        new Option<int>("Quantity", "Void Aura Quantity", "Number of Void Auras to farm.", 7600),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself). Set to 1 for solo.", 4),
        new Option<int>(
            "privateRoomNumber",
            "Private Room Number",
            "Private room number used by all army accounts. Set the same number on every account (1000-99999).",
            12345
        ),
        new Option<bool>("UseArmySync", "Enable Army Sync", "Use army sync to coordinate Void Aura and essence progress.", true),
        new Option<bool>(
            "HelpOthersWhenDone",
            "Help Others When Done",
            "If this account already has the current essence or Void Auras, keep attacking to help other army members finish faster.",
            true
        ),
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

        if (useArmySync && armySize > 1)
        {
            SetupPrivateRoom();
            SetupArmy(armySize, Bot.Config.Get<bool>("EnableClassSync"));
        }
        else if (useArmySync)
        {
            Core.Logger("Army sync is enabled but Army Size is 1. Set Army Size to your real army count on every account.");
        }

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
        bool helpOthers = Bot.Config!.Get<bool>("HelpOthersWhenDone");
        string auraSyncFile = "ArmyFarmVoidAurasv2_VoidAura.sync";

        Farm.EvilREP();
        Core.AddDrop(EssenceStages.Select(s => s.essence).ToArray());
        if (!Core.CheckInventory("Necromancer", toInv: false))
            Bot.Drops.Add("Creature Shard");

        string stageSyncFile = "ArmyFarmVoidAurasv2_stage.sync";
        int armySize = Bot.Config.Get<int>("ArmySize");

        while (!Bot.ShouldExit)
        {
            if (Core.CheckInventory("Void Aura", quant))
            {
                if (!useArmySync || !helpOthers || Ultra.CheckArmyProgressBool(() => Core.CheckInventory("Void Aura", quant), auraSyncFile))
                    break;

                Core.Logger("Army members still farming Void Auras. Staying in the fight to help others.");
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
            string syncFile = ArmyGeneral.GetSyncFileForItem("ArmyFarmVoidAurasv2_Essence", stage.essence);
            bool stageSkillsEnabled = false;

            while (!Bot.ShouldExit)
            {
                RefreshStageSync(stageSyncFile, GetOwnStage(essenceQuant));

                if (useArmySync)
                {
                    int armyStageNow = ArmyGeneral.PublishAndGetArmyStage(Ultra, Bot, GetOwnStage(essenceQuant), stageSyncFile, armySize);
                    if (armyStageNow != stageIndex)
                    {
                        Core.Logger($"Army stage changed {stageIndex} -> {armyStageNow}. Re-syncing.");
                        break;
                    }
                }

                if (Ultra.CheckArmyProgressBool(() => Core.CheckInventory(stage.essence, essenceQuant), syncFile))
                    break;

                bool stageComplete = Core.CheckInventory(stage.essence, essenceQuant);
                bool isHelper = useArmySync && helpOthers && stageComplete;

                if (isHelper)
                    Core.Logger($"Army helper on {stage.essence}: {Bot.Inventory.GetQuantity(stage.essence)}/{essenceQuant}");
                else
                    Core.Logger($"Army stage {stageIndex + 1}/{EssenceStages.Length}: {stage.essence}");

                Core.FarmingLogger(stage.essence, essenceQuant);
                RunStageCombat(stage, isHelper, useArmySync, ref stageSkillsEnabled);
                Bot.Sleep(isHelper ? 500 : 100);
            }

            if (stageSkillsEnabled)
            {
                Engine.DisableSkills();
                stageSkillsEnabled = false;
            }

            ResetCombatOptions();
        }
    }

    private void RunStageCombat(
        (string map, int? monsterID, string? monsterName, string essence) stage,
        bool isHelper,
        bool useArmySync,
        ref bool stageSkillsEnabled
    )
    {
        EnsureStageSkills(ref stageSkillsEnabled);

        if (isHelper)
        {
            JoinStageMap(stage.map);

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                return;
            }

            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;

            if (Bot.Player.Cell == null || Bot.Player.Cell.Equals("Enter", StringComparison.OrdinalIgnoreCase))
                PrepareHelperPosition(stage);
            else
                EnsureHelperOnTargetCell(stage);

            HelperAttack(stage);
            return;
        }

        FarmStage(stage, 100, useArmySync);
    }

    private void EnsureStageSkills(ref bool stageSkillsEnabled)
    {
        if (stageSkillsEnabled || !Bot.Config!.Get<bool>("DoEnh"))
            return;

        Engine.EnableSkills();
        stageSkillsEnabled = true;
    }

    private void RefreshStageSync(string stageSyncFile, int ownStage)
    {
        string? username = Bot.Player.Username;
        if (string.IsNullOrWhiteSpace(username))
            return;

        string key = username.Replace(":", "-");
        Ultra.UpdateEntry(Ultra.ResolveSyncPath(stageSyncFile), key, ownStage.ToString());
    }

    private void FarmStage(
        (string map, int? monsterID, string? monsterName, string essence) stage,
        int essenceQuant,
        bool useArmySync = false
    )
    {
        if (!useArmySync)
        {
            JoinStageMap(stage.map);

            if (!string.IsNullOrEmpty(stage.monsterName))
            {
                Engine.ChooseBestCell(stage.monsterName!);
                Core.HuntMonster(stage.map, stage.monsterName!, stage.essence, essenceQuant, false);
                return;
            }

            if (stage.monsterID.HasValue)
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

            return;
        }

        if (Core.CheckInventory(stage.essence, essenceQuant))
            return;

        JoinStageMap(stage.map);

        if (!Bot.Player.Alive)
        {
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            return;
        }

        if (Bot.Map.PlayerNames?.Any(x => x != Bot.Player.Username) == true)
        {
            Bot.Options.AggroMonsters = true;
            Bot.Options.HidePlayers = true;
        }

        if (!string.IsNullOrEmpty(stage.monsterName))
        {
            Engine.ChooseBestCell(stage.monsterName!);
            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(stage.monsterName!);
            return;
        }

        if (!stage.monsterID.HasValue)
            return;

        var target = Bot.Monsters.MapMonsters
            .FirstOrDefault(m => m != null && m.MapID == stage.monsterID.Value && m.Alive);

        if (target != null && !string.Equals(Bot.Player.Cell, target.Cell, StringComparison.OrdinalIgnoreCase))
        {
            Bot.Map.Jump(target.Cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(target.Cell);
        }

        if (!Bot.Player.HasTarget || Bot.Player.Target?.MapID != stage.monsterID.Value)
            Bot.Combat.Attack(stage.monsterID.Value);
    }

    private void PrepareHelperPosition((string map, int? monsterID, string? monsterName, string essence) stage)
    {
        EnsureHelperOnTargetCell(stage);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(500);
    }

    private void EnsureHelperOnTargetCell((string map, int? monsterID, string? monsterName, string essence) stage)
    {
        if (!string.IsNullOrEmpty(stage.monsterName))
        {
            Engine.ChooseBestCell(stage.monsterName!);
            return;
        }

        if (!stage.monsterID.HasValue)
            return;

        var target = Bot.Monsters.MapMonsters
            .FirstOrDefault(m => m != null && m.MapID == stage.monsterID.Value && m.Alive);

        if (target == null || string.IsNullOrWhiteSpace(target.Cell))
            return;

        if (string.Equals(Bot.Player.Cell, target.Cell, StringComparison.OrdinalIgnoreCase))
            return;

        Bot.Map.Jump(target.Cell, "Left", autoCorrect: false);
        Bot.Wait.ForCellChange(target.Cell);
    }

    private void HelperAttack((string map, int? monsterID, string? monsterName, string essence) stage)
    {
        if (!string.IsNullOrEmpty(stage.monsterName))
            Bot.Combat.Attack(stage.monsterName!);
        else if (stage.monsterID.HasValue)
            Bot.Combat.Attack(stage.monsterID.Value);
        else
            Bot.Combat.Attack("*");
    }

    private void JoinStageMap(string map)
    {
        if (string.Equals(Bot.Map.Name, map, System.StringComparison.OrdinalIgnoreCase))
            return;

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);
    }

    private void ResetCombatOptions()
    {
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.HidePlayers = false;
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

    private void SetupPrivateRoom()
    {
        Core.PrivateRooms = true;

        int configuredRoom = Bot.Config!.Get<int>("privateRoomNumber");
        if (configuredRoom >= 1000 && configuredRoom <= 99999)
        {
            Core.PrivateRoomNumber = configuredRoom;
        }
        else
        {
            Core.Logger($"Invalid private room number '{configuredRoom}'. Generating a fallback room number.");
            Core.PrivateRoomNumber = Army.getRoomNr();
        }

        Core.Logger($"Army private room set to #{Core.PrivateRoomNumber}. Use this same number on every account.");
    }

    private void SetupArmy(int armySize, bool enableClassSync)
    {
        string readySyncFile = "ArmyFarmVoidAurasv2.ready.sync";
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(readySyncFile));
        Bot.Sleep(2500);
        Core.Logger($"Waiting for army ready ({armySize - 1} other players) in room #{Core.PrivateRoomNumber}...");
        Ultra.WaitForArmy(armySize - 1, readySyncFile);

        ArmyGeneral.PrepareSyncFiles(
            Ultra,
            "ArmyFarmVoidAurasv2_VoidAura.sync",
            EssenceStages.Select(s => s.essence),
            "ArmyFarmVoidAurasv2_Essence"
        );
        ArmyGeneral.PrepareStageSync(Ultra, "ArmyFarmVoidAurasv2_stage.sync");

        if (enableClassSync)
        {
            Core.Logger("Starting army class sync...");
            ArmyGeneral.PrepareClassSync(Ultra, Bot, armySize, "ArmyFarmVoidAurasv2.class.sync");
        }
    }

    private void DoEnhs()
    {
        Core.Logger("Applying UltraEnhancements...");
        Enh.Apply();
    }
}
