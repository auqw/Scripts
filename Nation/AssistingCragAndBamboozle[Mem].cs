/*
name: AssistingCragAndBamboozle[Mem]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Nation/CoreNation.cs
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class AssistingCragAndBamboozle
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    static CoreNation _Nation;

    readonly string[] ACaBItems =
    {
        "Sword of Nulgath",
        "Gem of Nulgath",
        "Tainted Gem",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Totem of Nulgath",
        "Blood Gem of the Archfiend",
        "Unidentified 19",
        "Elders' Blood",
        "Voucher of Nulgath",
        "Voucher of Nulgath (non-mem)",
        "Archfiend's Favor",
        "Essence of Nulgath",
        "Nulgath Larvae",
        "Gem of Domniation",
        "Fiend Seal",
    };

    public string OptionsStorage = "AssistingCragAndBamboozle";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<Rewards>(
            "PickReward",
            "Pick your reward",
            "This quest is a daily.. unless you have multiple sparrows blood then itll keep running.",
            Rewards.Get_whats_not_maxed
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                Nation.CragName,
                "Gem of Nulgath",
                "Tainted Gem",
                "Dark Crystal Shard",
                "Diamond of Nulgath",
                "Totem of Nulgath",
                "Blood Gem of the Archfiend",
            }
        );

        Core.SetOptions();

        AssistingCandB(Bot.Config!.Get<Rewards>("PickReward"));

        Core.SetOptions(false);
    }

    public void AssistingCandB(Rewards reward = Rewards.Get_whats_not_maxed)
    {
        if (!Core.IsMember ||
            !Core.CheckInventory(Nation.CragName) ||
            (!Core.CheckInventory("Sparrow's Blood") &&
             !Daily.CheckDailyv2(803, true, true, "Sparrow's Blood")))
            return;

        Quest? quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(5817));
        if (quest?.Rewards == null)
        {
            Core.Logger("Quest 5817 failed to load.");
            return;
        }

        // Build target pool once
        List<ItemBase> targets = quest.Rewards
            .Where(r => reward == Rewards.Get_whats_not_maxed
                ? !Core.CheckInventory(r.ID, r.MaxStack)
                : r.ID == (int)reward && !Core.CheckInventory(r.ID, r.MaxStack))
            .ToList();

        if (targets.Count == 0)
        {
            Core.Logger("Everything already maxed. No work needed.");
            return;
        }

        Core.AddDrop(
            "Nulgath Larvae", "Sparrow's Blood", "Sword of Nulgath", "Gem of Nulgath",
            "Tainted Gem", "Dark Crystal Shard", "Diamond of Nulgath", "Totem of Nulgath",
            "Blood Gem of the Archfiend", "Unidentified 19", "Elders' Blood",
            "Voucher of Nulgath", "Voucher of Nulgath (non-mem)"
        );

        Core.Logger($"AssistingCandB Optimized → Targets: {targets.Count}");

        while (!Bot.ShouldExit && targets.Count > 0)
        {
            Core.EnsureAccept(5817);

            if (!Core.CheckInventory("Tendurrr The Assistant"))
                Core.KillMonster("tercessuinotlim", "m2", "Left", "*", "Tendurrr The Assistant", isTemp: false);

            Daily.SparrowsBlood(1);
            Bot.Wait.ForPickup(5584);

            if (!Core.CheckInventory("Sparrow's Blood"))
            {
                Core.Logger("Missing Sparrow's Blood. Stopping.");
                return;
            }

            Nation.EssenceofNulgath(20);
            Nation.ApprovalAndFavor(100, 100);
            Nation.NationRound4Medal();

            Core.EnsureAccept(4748);
            Core.HuntMonster("shadowblast", "Legion Fenrir", "Fiend Seal", 10, isTemp: false);

            // ---- SMART BATCH EXECUTION ----
            List<ItemBase> completedThisCycle = new();

            foreach (ItemBase target in targets)
            {
                if (Core.CheckInventory(target.ID, target.MaxStack))
                {
                    completedThisCycle.Add(target);
                    continue;
                }

                Core.EnsureComplete(5817, target.ID);

                if (Core.CheckInventory(target.ID, target.MaxStack))
                    completedThisCycle.Add(target);

                if (!Core.CheckInventory("Sparrow's Blood"))
                {
                    Core.Logger($"Stopped early after completing {target.Name}");
                    return;
                }
            }

            // prune finished items in one go (fast + clean)
            targets.RemoveAll(t => completedThisCycle.Any(c => c.ID == t.ID));
        }

        Core.Logger("AssistingCandB Optimized → COMPLETE.");
    }
    public enum Rewards
    {
        Blood_Gem_of_the_Archfiend = 22332,
        Gem_of_Nulgath = 6136,
        Elders_Blood = 5586,
        Totem_of_Nulgath = 5357,
        Voucher_of_Nulgath_non_mem = 4862,
        Voucher_of_Nulgath = 4861,
        Diamond_of_Nulgath = 4771,
        Dark_Crystal_Shard = 4770,
        Tainted_Gem = 4769,
        Unidentified_19 = 4752,
        Sword_of_Nulgath = 4670,
        Get_whats_not_maxed = -1,
    }
}
