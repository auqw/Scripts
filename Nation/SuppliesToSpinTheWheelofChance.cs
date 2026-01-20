/*
name: SuppliesToSpinTheWheelofChance
description: Do "Supplies to Spin the Wheel" [*or* swindles bilk quests if u have it avaible.]
tags: swindles return policy, supplies to spin the wheel, swindles bilk, the assistant, nulgath, nation, supplies, ULTRA Alteon, escherion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class SuppliesToSpinTheWheelofChance
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public string OptionsStorage = "SuppliesOptions";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<SwindlesReturnItem>(
            "SwindlesReturnItem",
            "SwindlesReturnItem",
            "pick the reward for the \"Swindles Return\" Quest",
            SwindlesReturnItem.All
        ),
        new Option<SuppliesReward>(
            "SuppliesReward",
            "SuppliesReward",
            "pick the reward for the \"Supplies to spin the wheel\" Quest",
            SuppliesReward.All
        ),
        new Option<bool>(
            "AssistantDuring",
            "Do: \"The Assistant\" during?",
            "Do the quest: [The Assistant], (requires alota gold, that you will get from the vouchers of nulgath (mem)) during this.",
            false
        ),
        new Option<bool>(
            "UltraAlteon",
            "Kill \"UltraAlteon\"",
            "Instead of \"Escherion\" or bamboozle, do \"ULTRA Alteon\"?",
            false
        ),
        // add option for Voucher Item: Totem of Nulgath During
        new Option<bool>(
            "VoucherItemQuestDuring",
            "Do `Voucher Item: Totem of Nulgath` During?",
            "Do Voucher Item: Totem of Nulgath During?",
            false
        ),
        new Option<bool>("KeepVoucher", "Keep Voucher?", "Keep Voucher? (false = gold)", false),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.SuppliesRewards.Concat(Nation.SwindlesReturnRewards));
        Core.SetOptions();

        DoSupplies();

        Core.SetOptions(false);
    }

    public void DoSupplies()
    {
        // Get and normalize config options
        string? swindlesReturnItem = GetNormalizedConfigItem<SwindlesReturnItem>("SwindlesReturnItem");
        string? suppliesItem = GetNormalizedConfigItem<SuppliesReward>("SuppliesReward");

        // Load required quests
        Quest supplies = LoadQuestWithRetry(2857, "Supplies");
        Quest swindlesReturn = LoadQuestWithRetry(7551, "Swindle's Return");

        // Build combined rewards list
        List<ItemBase> combinedRewards = BuildCombinedRewardsList(supplies, swindlesReturn, suppliesItem);

        // Log configuration
        LogSuppliesConfiguration(combinedRewards, suppliesItem, swindlesReturnItem);

        // Process each reward
        ProcessRewards(combinedRewards, supplies, swindlesReturn, ref suppliesItem, ref swindlesReturnItem);
    }

    private string? GetNormalizedConfigItem<T>(string configKey) where T : Enum
    {
        string? item = Bot.Config!.Get<T>(configKey).ToString()?.Replace('_', ' ');
        return item == "All" ? null : item;
    }

    private Quest LoadQuestWithRetry(int questId, string questName)
    {
        while (true)
        {
            Quest? quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(questId));
            if (quest != null)
                return quest;

            Core.Logger($"Failed to load quest {questId} ({questName}). Retrying...");
            Core.Sleep();
        }
    }

    private List<ItemBase> BuildCombinedRewardsList(Quest supplies, Quest swindlesReturn, string? suppliesItem)
    {
        List<ItemBase> combinedRewards = new();

        // Add unique Supplies rewards
        combinedRewards.AddRange(
            supplies.Rewards
                .Where(r => r != null && Nation.SuppliesRewards.Contains(r.Name))
                .DistinctBy(r => r.ID)
        );

        // Add unique Swindle's Return rewards
        combinedRewards.AddRange(
            swindlesReturn.Rewards
                .Where(r => r != null && Nation.SwindlesReturnRewards.Contains(r.Name))
                .DistinctBy(r => r.ID)
        );

        // Filter to specific item if chosen
        if (suppliesItem != null)
        {
            ItemBase? chosen = supplies.Rewards?.FirstOrDefault(r => r.Name == suppliesItem);
            if (chosen != null)
                return new List<ItemBase> { chosen };
        }

        return combinedRewards.DistinctBy(r => r.ID).ToList();
    }

    private void LogSuppliesConfiguration(List<ItemBase> rewards, string? suppliesItem, string? swindlesReturnItem)
    {
        string rewardNames = string.Join("\", \"", rewards.Select(r => r.Name));
        Core.Logger(
            $"Rewards Selected: \"{rewardNames}\"\n\n" +
            $"Maxing Supplies? {(suppliesItem == null ? "Yes" : "No")}\n" +
            $"Maxing Swindles? {(swindlesReturnItem == null ? "Yes" : "No")}\n",
            "STStW Config"
        );
    }

    private void ProcessRewards(
     List<ItemBase> combinedRewards,
     Quest supplies,
     Quest swindlesReturn,
     ref string? suppliesItem,
     ref string? swindlesReturnItem)
    {
        foreach (ItemBase item in combinedRewards)
        {
            if (Core.CheckInventory(item.ID, item.MaxStack))
            {
                Core.Logger($"Skipping {item.Name} - already at max stack ({item.MaxStack})");
                continue;
            }

            Core.FarmingLogger(item.Name, item.MaxStack);

            // Determine items dynamically if set to "All"
            swindlesReturnItem ??= GetNextNonMaxedReward(swindlesReturn, Nation.SwindlesReturnRewards);
            suppliesItem ??= GetNextNonMaxedReward(supplies, Nation.SuppliesRewards);

            if (swindlesReturnItem == null)
                Core.Logger("All Swindle's Return items are maxed - Return Policy will be disabled");

            if (suppliesItem == null && !Nation.SuppliesRewards.Contains(item.Name))
            {
                Core.Logger($"No valid Supplies item found for {item.Name} - skipping");
                continue;
            }
            // ✅ FIX: Only use item.Name as fallback if it's actually valid for that quest
            string currentSuppliesItem = suppliesItem ??
                (Nation.SuppliesRewards.Contains(item.Name) ? item.Name : suppliesItem);

            string? currentSwindlesItem = swindlesReturnItem ??
                (Nation.SwindlesReturnRewards.Contains(item.Name) ? item.Name : null);

            Core.Logger($"Target - Supplies: {currentSuppliesItem ?? "None"}, Swindle's Return: {currentSwindlesItem ?? "None"}");

            // Get max stacks only for valid items
            int suppliesMaxStack = currentSuppliesItem != null
                ? GetRewardMaxStack(supplies, currentSuppliesItem)
                : 0;

            int swindlesMaxStack = currentSwindlesItem != null
                ? GetRewardMaxStack(swindlesReturn, currentSwindlesItem)
                : 0;

            Core.Logger($"Max Stacks - Supplies: {suppliesMaxStack}, Swindle's Return: {swindlesMaxStack}");

            bool returnPolicyActive = Core.CBOBool("Nation_ReturnPolicyDuringSupplies", out bool returnSupplies)
                && returnSupplies
                && currentSwindlesItem != null; // ✅ Only activate if we have a valid Swindle's item

            Nation.Supplies(
                currentSuppliesItem,
                suppliesMaxStack,
                Bot.Config!.Get<bool>("UltraAlteon"),
                Bot.Config!.Get<bool>("KeepVoucher"),
                Bot.Config!.Get<bool>("AssistantDuring"),
                currentSwindlesItem, // Can be null now
                returnPolicyActive,
                Bot.Config.Get<bool>("VoucherItemQuestDuring")
            );
        }
    }
    private string? GetNextNonMaxedReward(Quest quest, string[] validRewards)
    {
        if (quest?.Rewards == null)
        {
            Core.Logger($"Quest rewards are null for quest {quest?.ID ?? 0}");
            return null;
        }

        var allItems = Bot.Inventory.Items.Concat(Bot.Bank.Items);

        return quest.Rewards
            .Where(r => r != null &&
                        validRewards.Contains(r.Name) &&
                        !allItems.Any(i => i.ID == r.ID && i.Quantity >= r.MaxStack))
            .Select(r => r.Name)
            .FirstOrDefault();
    }

    private int GetRewardMaxStack(Quest quest, string itemName)
    {
        ItemBase? reward = quest.Rewards?.FirstOrDefault(x => x != null && x.Name == itemName);
        int maxStack = reward?.MaxStack ?? 0;

        if (maxStack == 0)
            Core.Logger($"Warning: Could not find max stack for '{itemName}' in quest {quest.ID}");

        return maxStack;
    }

    public enum SwindlesReturnItem
    {
        All,
        Tainted_Gem,
        Dark_Crystal_Shard,
        Diamond_of_Nulgath,
        Gem_of_Nulgath,
        Blood_Gem_of_the_Archfiend,
        Receipt_of_Swindle,
    }

    public enum SuppliesReward
    {
        All,
        Tainted_Gem,
        Dark_Crystal_Shard,
        Diamond_of_Nulgath,
        Voucher_of_Nulgath,
        Voucher_of_Nulgath_NonMem,
        Gem_of_Nulgath,
        Unidentified_10,
        Essence_of_Nulgath,
    }

}
