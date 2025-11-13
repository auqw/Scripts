/*
name: Nation Loyalty Rewarded
description: Does the Nation Loyalty Rewarded Quest to max the quest rewards.
tags: nation loyalty rewarded, nulgath, nation, dark crystal shard, diamond of nulgath, diamond badge of nulgath
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreAdvanced.cs

using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class NationLoyaltyRewarded
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        var quest = Core.EnsureLoad(4749);
        var questRewards = quest?.Rewards ?? new();
        var acceptRequirements = quest?.AcceptRequirements ?? new();

        Core.BankingBlackList.AddRange(
            (
                questRewards?.Select(item => item?.ToString() ?? string.Empty)
                ?? Enumerable.Empty<string>()
            ).Concat(
                acceptRequirements?.Select(item => item?.ToString() ?? string.Empty)
                    ?? Enumerable.Empty<string>()
            )
        );

        Core.SetOptions();
        FarmQuest(
            questRewards
                ?.Select(x => x?.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!) // tell compiler these are non-null
                .ToArray()
        );

        Core.SetOptions(false);
    }

    public void FarmQuest(string[]? farmItems = null, int quantity = 0)
    {
        var quest = Core.EnsureLoad(4749);
        var rewards = quest?.Rewards;

        if (farmItems == null || farmItems.Length == 0)
        {
            // Process all items
            if (rewards != null && rewards.Any())
            {
                foreach (var item in rewards.Where(x => x != null))
                {
                    int targetQty =
                        quantity == 0
                            ? rewards.Find(x => x?.Name == item?.Name)?.MaxStack ?? 0
                            : quantity;

                    if (!string.IsNullOrWhiteSpace(item?.Name))
                        NLR(new[] { item.Name }, targetQty);
                }
            }
            else
            {
                Core.Logger("No rewards found for quest 4749. Skipping FarmQuest.");
            }
        }
        else
        {
            // Add rewards to drop list
            Core.AddDrop(farmItems.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

            // Required items to start quest
            Nation.NationRound4Medal();
            Nation.FarmUni13(1);

            // Process each specified item
            foreach (string? farmItem in farmItems)
            {
                if (string.IsNullOrWhiteSpace(farmItem))
                    continue;

                ItemBase? reward = Core.InitializeWithRetries(() =>
                    rewards?.FirstOrDefault(x => x?.Name == farmItem)
                );
                if (reward == null)
                {
                    Core.Logger($"Reward '{farmItem}' not found in quest 4749.");
                    continue;
                }

                if (Bot.Inventory.IsMaxStack(reward.ID))
                    continue;

                int targetQty = quantity == 0 ? reward.MaxStack : quantity;
                NLR(new[] { farmItem }, targetQty);
            }
        }
    }

    public void NLR(string[]? items = null, int quantity = 1)
    {
        if (
            items == null
            || items.Length == 0
            || items.All(item =>
                string.IsNullOrWhiteSpace(item) || Core.CheckInventory(item, quantity)
            )
        )
        {
            Core.Logger(
                items == null || items.Length == 0
                    ? "Items parameter is null or empty, skipping NLR."
                    : $"{string.Join(", ", items.Where(x => !string.IsNullOrWhiteSpace(x)))} x{quantity} already in inventory. Skipping..."
            );
            return;
        }

        foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x)))
            Core.FarmingLogger(item!, quantity);

        Core.EquipClass(ClassType.Solo);

        // Nation Loyalty Rewarded quest (ID: 4749)
        while (
            !Bot.ShouldExit
            && items.Any(item =>
                !string.IsNullOrWhiteSpace(item) && !Core.CheckInventory(item, quantity)
            )
        )
        {
            Core.EnsureAccept(4749);

            Core.KillMonster("dflesson", "r12", "Right", 29, 33257, isTemp: true, publicRoom: true);
            Core.HuntMonster("aqlesson", "Carnax", "Carnax Eye", log: false);
            Core.HuntMonster("deepchaos", "Kathool", "Kathool Tentacle", log: false);
            Core.HuntMonster("lair", "Red Dragon", "Red Dragon's Fang", log: false);
            Core.HuntMonster("bloodtitan", "Blood Titan", "Blood Titan's Blade", log: false);

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster(
                "tercessuinotlim",
                "m2",
                "Left",
                "*",
                "Defeated Makai",
                25,
                log: false
            );

            foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x)))
                Bot.Wait.ForPickup(item!);

            Core.EnsureComplete(4749);
        }
    }
}
