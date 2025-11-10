/*
name: SuppliesToSpinTheWheelofChance
description: Do "Supplies to Spin the Wheel" [*or* swindles bilk quests if u have it avaible.]
tags: swindles return policy, supplies to spin the wheel, swindles bilk, the assistant, nulgath, nation, supplies, ultra alteon, escherion
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
    private static CoreNation Nation { get => _Nation ??= new CoreNation(); set => _Nation = value; }
    private static CoreNation _Nation;

    public string OptionsStorage = "SuppliesOptions";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<SwindlesReturnItem>("SwindlesReturnItem", "SwindlesReturnItem", "pick the reward for the \"Swindles Return\" Quest", SwindlesReturnItem.All),
        new Option<SuppliesReward>("SuppliesReward", "SuppliesReward", "pick the reward for the \"Supplies to spin the wheel\" Quest", SuppliesReward.All),
        new Option<bool>("AssistantDuring", "Do: \"The Assistant\" during?", "Do the quest: [The Assistant], (requires alota gold, that you will get from the vouchers of nulgath (mem)) during this.", false),
        new Option<bool>("UltraAlteon", "Kill \"UltraAlteon\"", "Instead of \"Escherion\" or bamboozle, do \"Ultra Alteon\"?", false),
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
    {// Set Config Options & Convert Enum into string for display
        string? SwindlesReturnItem = Bot.Config!.Get<SwindlesReturnItem>("SwindlesReturnItem")
            .ToString()?.Replace('_', ' ');
        string? SuppliesItem = Bot.Config!.Get<SuppliesReward>("SuppliesReward")
            .ToString()?.Replace('_', ' ');

        // Normalize "All" into null to mean "max everything"
        SuppliesItem = SuppliesItem == "All" ? null : SuppliesItem;
        SwindlesReturnItem = SwindlesReturnItem == "All" ? null : SwindlesReturnItem;

    Retry2857:
        // Load quest 2857 (Supplies)
        Quest? Supplies = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(2857));
        if (Supplies == null)
        {
            Core.Logger("Failed to load quest 2857.");
            Core.Sleep();
            goto Retry2857;
        }

    Retry7551:
        // Load quest 7551 (Swindle’s Return)
        Quest? SwindlesReturn = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(7551));
        if (SwindlesReturn == null)
        {
            Core.Logger("Failed to load quest 7551.");
            Core.Sleep();
            goto Retry7551;
        }

        // --- Build combined rewards list ---
        List<ItemBase> combinedRewards = new();

        // Add unique Supplies rewards
        combinedRewards.AddRange(Supplies.Rewards
            .Where(r => r != null && Nation.SuppliesRewards.Contains(r.Name))
            .DistinctBy(r => r.ID));

        // Add unique SwindlesReturn rewards
        combinedRewards.AddRange(SwindlesReturn.Rewards
            .Where(r => r != null && Nation.SwindlesReturnRewards.Contains(r.Name))
            .DistinctBy(r => r.ID));

        // If a specific Supplies reward was chosen, filter down to it
        if (SuppliesItem != null)
        {
            var chosen = Supplies.Rewards?.FirstOrDefault(r => r.Name == SuppliesItem);
            if (chosen != null)
                combinedRewards = new List<ItemBase> { chosen };
        }
        else
        {
            // Otherwise, ensure no duplicates remain
            combinedRewards = combinedRewards.DistinctBy(r => r.ID).ToList();
        }

        // Log what we’re working on
        Core.Logger(
            $"Rewards Selected: \"{string.Join("\", \"", combinedRewards.Select(r => r.Name))}\"\n\n" +
            $"Maxing Supplies? {(SuppliesItem == null ? "Yes" : "No")}\n" +
            $"Maxing Swindles? {(SwindlesReturnItem == null ? "Yes" : "No")}\n",
            "STStW Config");

        // Process rewards
        foreach (ItemBase item in combinedRewards)
        {
            // Skip if the item is already in the inventory and we have the max stack
            if (Core.CheckInventory(item.ID, item.MaxStack))
                continue;

            Core.FarmingLogger(item.Name, item.MaxStack);

            // Determine the SwindlesReturnItem if it's null
            if (SwindlesReturn == null || SwindlesReturn.Rewards == null)
            {
                Core.Logger("SwindlesReturn or SwindlesReturn.Rewards is null");
                return;
            }

            SwindlesReturnItem ??= SwindlesReturn.Rewards
                .Where(r => r != null && !Bot.Inventory.Items.Concat(Bot.Bank.Items).Any(i => i.ID == r.ID) && r.Quantity < r.MaxStack && Nation.SwindlesReturnRewards.Contains(r.Name))
                .Select(r => r.Name)
                .FirstOrDefault();

            Core.Logger($"SwindlesReturnItem: {SwindlesReturnItem}");

            // Determine the SuppliesItem if it's null
            if (Supplies == null || Supplies.Rewards == null)
            {
                Core.Logger("Supplies or Supplies.Rewards is null");
                return;
            }

            SuppliesItem ??= Supplies.Rewards
                .Where(r => r != null && !Bot.Inventory.Items.Concat(Bot.Bank.Items).Any(i => i.ID == r.ID) && r.Quantity < r.MaxStack && Nation.SuppliesRewards.Contains(r.Name))
                .Select(r => r.Name)
                .FirstOrDefault();

            Core.Logger($"SuppliesItem: {SuppliesItem}");

            // Determine the max stack values directly without null checks
            ItemBase? suppliesReward = SuppliesItem == null
                ? Supplies.Rewards.FirstOrDefault(x => x != null && x.Name == item.Name)
                : Supplies.Rewards.FirstOrDefault(x => x != null && x.Name == SuppliesItem);

            int suppliesMaxStack = suppliesReward != null ? suppliesReward.MaxStack : 0;
            Core.Logger($"suppliesMaxStack: {suppliesMaxStack}");

            ItemBase? swindlesReward = SwindlesReturnItem == null
                ? SwindlesReturn.Rewards.FirstOrDefault(x => x != null && x.Name == item.Name)
                : SwindlesReturn.Rewards.FirstOrDefault(x => x != null && x.Name == SwindlesReturnItem);

            int swindlesMaxStack = swindlesReward != null ? swindlesReward.MaxStack : 0;
            Core.Logger($"swindlesMaxStack: {swindlesMaxStack}");

            // Call the Nation.Supplies method with the correct parameters
            Nation.Supplies(
                SuppliesItem ?? item.Name, // Coalesce SuppliesItem to item.Name if it's null
                suppliesMaxStack, // Use determined max stack for Supplies
                Bot.Config!.Get<bool>("UltraAlteon"),
                Bot.Config!.Get<bool>("KeepVoucher"),
                Bot.Config!.Get<bool>("AssistantDuring"),
                SwindlesReturnItem ?? item.Name, // Use SwindlesReturnItem if set, otherwise use item.Name
                Core.CBOBool("Nation_ReturnPolicyDuringSupplies", out bool _returnSupplies) && _returnSupplies
                );
        }
    }

    public enum SwindlesReturnItem
    {
        All,
        Tainted_Gem,
        Dark_Crystal_Shard,
        Diamond_of_Nulgath,
        Gem_of_Nulgath,
        Blood_Gem_of_the_Archfiend,
        Receipt_of_Swindle
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
