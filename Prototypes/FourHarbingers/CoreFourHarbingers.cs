/*
name: CoreFourHarbingers
description: null
tags: null
*/

//cs_include Scripts/CoreBots.cs
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;

public class CoreFourHarbingers
{
    public static readonly string[] AllMergeItems =
    {
        "Celestial Dragon of Time",
        "Infernal Dragon of Time",
        "Harbinger of War",
        "Harbinger of Famine",
        "Harbinger of Death",
        "Harbinger of Conquest",
        "Bow of Halosis",
        "Halosis' Divine Wheel",
        "Wings of Mors",
        "Bello's Armaments",
        "Bello's Braziers",
        "Bello's Sanctified Braziers",
        "Hunting Bow of Halosis",
        "Poise of Halosis",
        "Poise of Fames",
        "Poise of Mors",
        "Poise of Bello",
        "Infernal Dragon of Time Hood",
        "Celestial Dragon of Time Hood",
        "Bello's Brazier",
        "Fames' Barren Scale",
        "Bello's Sanctified Brazier",
        "Nightstar Companion",
        "Scythe of Mors",
    };

    public enum FarmMode
    {
        Once,
        Until_Stopped,
        Quest_Completions,
        Item_Quantity,
    }

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;

    private const string GoldVoucher100k = "Gold Voucher 100k";
    private const string GoldVoucher500k = "Gold Voucher 500k";

    private readonly Dictionary<string, MergeRecipe> MergeRecipes = CreateMergeRecipes();

    public bool UsePotionsEnabled()
    {
        return Bot.Config!.Get<bool>("UsePotions");
    }

    public bool DoEnhancementsEnabled()
    {
        return Bot.Config!.Get<bool>("DoEnhancements");
    }

    public FarmMode GetFarmMode(bool doAllMode, FarmMode farmModeOverride)
    {
        if (doAllMode)
            return farmModeOverride;

        if (Bot.Config!.Get<int>("FarmQuantity") == 0)
            return FarmMode.Until_Stopped;

        return FarmMode.Quest_Completions;
    }

    public int GetFarmQuantity(bool doAllMode, int farmQuantityOverride)
    {
        if (doAllMode)
            return farmQuantityOverride;

        return Bot.Config!.Get<int>("FarmQuantity");
    }

    public string GetFarmItem(bool doAllMode, string farmItemOverride)
    {
        if (doAllMode)
            return farmItemOverride;

        return "";
    }

    public void AddDrops(params string[] drops)
    {
        Bot.Drops.Add(drops);
    }

    public bool EquipClass(string className)
    {
        if (!Core.CheckInventory(className, toInv: false))
        {
            Core.Logger($"WARNING: {className} is required for this setup.", messageBox: true);
            return false;
        }

        if (!Bot.Inventory.Contains(className))
        {
            if (Bot.Inventory.FreeSlots <= 0)
            {
                Core.Logger($"WARNING: {className} is in the bank, but no free inventory slot is available.", messageBox: true);
                return false;
            }

            Bot.Bank.EnsureToInventory(className);
            Bot.Wait.ForTrue(() => Bot.Inventory.Contains(className), 20);
            if (!Bot.Inventory.Contains(className))
            {
                Core.Logger($"WARNING: {className} could not be moved from the bank.", messageBox: true);
                return false;
            }
        }

        Core.Equip(className);
        Bot.Wait.ForItemEquip(className);
        if (!Bot.Inventory.IsEquipped(className))
        {
            Core.Logger($"WARNING: {className} could not be equipped.", messageBox: true);
            return false;
        }

        return true;
    }

    public bool EquipClass(string className, string fallbackClass, bool allowFallback, out string resolvedClass)
    {
        resolvedClass = className;
        if (!Core.CheckInventory(className, toInv: false))
        {
            if (!allowFallback || !Core.CheckInventory(fallbackClass, toInv: false))
            {
                Core.Logger($"WARNING: {className} is required for this setup.", messageBox: true);
                return false;
            }

            Core.Logger(
                $"WARNING: {className} was selected, but you do not own it. Falling back to {fallbackClass}.",
                messageBox: true
            );
            resolvedClass = fallbackClass;
        }

        return EquipClass(resolvedClass);
    }

    public bool EnsureBossRoom(string cell, string pad)
    {
        if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
        {
            Core.Join("fourharbingers-100000", cell, pad);
            return false;
        }

        if (!Bot.Player.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase))
        {
            Core.Jump(cell, pad);
            return false;
        }

        return true;
    }

    public void ReturnToSafeRoom()
    {
        if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
            Core.Join("fourharbingers-100000", "Enter", "Spawn");
        else if (!Bot.Player.Cell.Equals("Enter", StringComparison.OrdinalIgnoreCase))
            Core.Jump("Enter", "Spawn");

        Bot.Wait.ForCombatExit();
    }

    public void StopSkills()
    {
        Bot.Skills.Resume();
        Bot.Skills.Stop();
        Bot.Wait.ForTrue(() => !Bot.Skills.TimerRunning, 20);
    }

    public bool RunQuests(int questID, Func<bool> fightBoss, Action restockPotions,
        FarmMode farmMode, int farmQuantity, string farmItem)
    {
        if (farmQuantity < 1)
            farmQuantity = 1;

        if (farmMode == FarmMode.Item_Quantity && string.IsNullOrWhiteSpace(farmItem))
        {
            Core.Logger("WARNING: Farm Mode is Item Quantity, but Farm Item is empty.", messageBox: true);
            return false;
        }

        int completions = 0;
        while (!Bot.ShouldExit && !FarmTargetReached(farmMode, farmQuantity, farmItem, completions))
        {
            if (!fightBoss())
                return false;

            Bot.Wait.ForQuestComplete(questID);
            completions++;

            if (!FarmTargetReached(farmMode, farmQuantity, farmItem, completions))
                restockPotions();
        }

        return FarmTargetReached(farmMode, farmQuantity, farmItem, completions);
    }

    private bool FarmTargetReached(FarmMode farmMode, int farmQuantity, string farmItem, int completions)
    {
        if (farmMode == FarmMode.Once)
            return completions >= 1;

        if (farmMode == FarmMode.Quest_Completions)
            return completions >= farmQuantity;

        if (farmMode == FarmMode.Until_Stopped)
            return false;

        return Core.CheckInventory(farmItem, farmQuantity, toInv: false);
    }

    public void GetPotion(string itemName, string voucherName, int voucherQuantity, int voucherCost,
        int targetQuantity, int requiredAlchemyRank = 0, string requiredFaction = "", int requiredFactionRank = 0)
    {
        try
        {
            if (Bot.Inventory.GetQuantity(itemName) > 1)
                return;

            if (Bot.Bank.Contains(itemName))
            {
                if (!Bot.Inventory.Contains(itemName) && Bot.Inventory.FreeSlots <= 0)
                {
                    WarnPotion(itemName, "no free inventory slot is available");
                    return;
                }

                Bot.Bank.EnsureToInventory(itemName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);
                if (Bot.Inventory.GetQuantity(itemName) > 1)
                    return;
            }

            if (requiredAlchemyRank > 0 && !Bot.Reputation.HasRank("Alchemy", requiredAlchemyRank))
            {
                WarnPotion(itemName, $"Alchemy rank {requiredAlchemyRank} is required");
                return;
            }

            if (!string.IsNullOrWhiteSpace(requiredFaction)
                && !Bot.Reputation.HasRank(requiredFaction, requiredFactionRank))
            {
                WarnPotion(itemName, $"{requiredFaction} rank {requiredFactionRank} is required");
                return;
            }

            int requiredSlots = 0;
            if (!Bot.Inventory.Contains(itemName))
                requiredSlots++;
            if (!Bot.Inventory.Contains(voucherName))
                requiredSlots++;
            if (Bot.Inventory.FreeSlots < requiredSlots)
            {
                WarnPotion(itemName, $"{requiredSlots} free inventory slots are required");
                return;
            }

            if (Bot.Bank.Contains(voucherName))
            {
                Bot.Bank.EnsureToInventory(voucherName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(voucherName), 20);
            }

            int missingVouchers = voucherQuantity - Bot.Inventory.GetQuantity(voucherName);
            if (missingVouchers < 0)
                missingVouchers = 0;

            int requiredGold = missingVouchers * voucherCost;
            if (Bot.Player.Gold < requiredGold)
            {
                WarnPotion(itemName, $"{requiredGold} gold is required");
                return;
            }

            Core.Join("alchemyacademy");
            Bot.Shops.Load(2036);
            if (!Bot.Shops.IsLoaded || Bot.Shops.ID != 2036)
            {
                WarnPotion(itemName, "the potion shop could not be loaded");
                return;
            }

            if (missingVouchers > 0)
            {
                Core.BuyItem("alchemyacademy", 2036, voucherName, voucherQuantity);
                Bot.Wait.ForTrue(() => Bot.Inventory.GetQuantity(voucherName) >= voucherQuantity, 20);
            }

            if (Bot.Inventory.GetQuantity(voucherName) < voucherQuantity)
            {
                WarnPotion(itemName, "the required vouchers could not be purchased");
                return;
            }

            Core.BuyItem("alchemyacademy", 2036, itemName, targetQuantity);
            Bot.Wait.ForTrue(() => Bot.Inventory.GetQuantity(itemName) >= targetQuantity, 20);
            if (Bot.Inventory.GetQuantity(itemName) < targetQuantity)
                WarnPotion(itemName, "it could not be purchased");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion preparation failed for {itemName}: {ex}");
            WarnPotion(itemName, "preparation failed");
        }
    }

    public void UsePotion(string itemName, string auraName)
    {
        try
        {
            if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
            {
                Core.Logger("WARNING: Potions will only be used inside fourharbingers.");
                return;
            }

            if (!Bot.Inventory.Contains(itemName))
            {
                WarnPotion(itemName, "it is not in the inventory");
                return;
            }

            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Wait.ForItemEquip(itemName);
            Bot.Sleep(2000);
            if (!Bot.Inventory.IsEquipped(itemName))
            {
                WarnPotion(itemName, "it could not be equipped");
                return;
            }

            if (Bot.Self.HasActiveAura(auraName))
                return;

            int quantityBefore = Bot.Inventory.GetQuantity(itemName);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(2000);
            Bot.Wait.ForTrue(() => Bot.Self.HasActiveAura(auraName)
                || Bot.Inventory.GetQuantity(itemName) < quantityBefore, 20);
            if (!Bot.Self.HasActiveAura(auraName)
                && Bot.Inventory.GetQuantity(itemName) >= quantityBefore)
                WarnPotion(itemName, "its effect could not be verified");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion use failed for {itemName}: {ex}");
            WarnPotion(itemName, "use failed");
        }
    }

    public void RestockPotions(string[] potionNames, Action getPotions, Action usePotions)
    {
        if (potionNames.All(item => Bot.Inventory.GetQuantity(item) > 1))
            return;

        ReturnToSafeRoom();
        getPotions();
        Core.Join("fourharbingers-100000", "Enter", "Spawn");
        usePotions();
    }

    public bool CompleteMergeItems(IEnumerable<string> selectedItems, Func<string, int, bool> farmScroll)
    {
        List<string> targets = selectedItems
            .Where(item => MergeRecipes.ContainsKey(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (targets.Count == 0)
        {
            Core.Logger("No Four Harbingers merge items were selected.");
            return true;
        }

        Dictionary<string, int> itemDemand = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> purchaseCounts = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> materialRequirements = new(StringComparer.OrdinalIgnoreCase);
        foreach (string target in targets)
            AddMergeDemand(target, 1, itemDemand, purchaseCounts, materialRequirements);

        foreach (KeyValuePair<string, int> material in materialRequirements)
        {
            if (material.Key.Equals(GoldVoucher100k, StringComparison.OrdinalIgnoreCase)
                || material.Key.Equals(GoldVoucher500k, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!farmScroll(material.Key, material.Value))
            {
                Core.Logger($"WARNING: Failed to farm {material.Key} ({OwnedQuantity(material.Key)}/{material.Value}).", messageBox: true);
                return false;
            }
        }

        ReturnToSafeRoom();

        int voucher100k = GetRequirement(materialRequirements, GoldVoucher100k);
        int voucher500k = GetRequirement(materialRequirements, GoldVoucher500k);
        if (!BuyGoldVouchers(voucher100k, voucher500k))
            return false;

        foreach (string material in materialRequirements.Keys)
            MoveToInventory(material);

        foreach (string target in targets.OrderByDescending(GetRecipeDepth))
        {
            if (!AcquireMergeItem(target, false))
                return false;
        }

        Core.Logger($"Four Harbingers merge complete: {string.Join(", ", targets)}.");
        return true;
    }

    private void AddMergeDemand(string itemName, int quantity, Dictionary<string, int> itemDemand,
        Dictionary<string, int> purchaseCounts, Dictionary<string, int> materialRequirements)
    {
        if (!MergeRecipes.TryGetValue(itemName, out MergeRecipe? recipe))
        {
            if (!materialRequirements.ContainsKey(itemName))
                materialRequirements[itemName] = 0;

            materialRequirements[itemName] += quantity;
            return;
        }

        int previousDemand = GetRequirement(itemDemand, itemName);
        int previousPurchases = GetRequirement(purchaseCounts, itemName);
        int totalDemand = previousDemand + quantity;
        int requiredPurchases = Math.Max(0, totalDemand - OwnedQuantity(itemName));
        int addedPurchases = requiredPurchases - previousPurchases;
        itemDemand[itemName] = totalDemand;
        if (addedPurchases <= 0)
            return;

        purchaseCounts[itemName] = requiredPurchases;
        foreach (KeyValuePair<string, int> requirement in recipe.Requirements)
            AddMergeDemand(requirement.Key, requirement.Value * addedPurchases, itemDemand, purchaseCounts, materialRequirements);
    }

    private bool BuyGoldVouchers(int voucher100k, int voucher500k)
    {
        int missing100k = Math.Max(0, voucher100k - OwnedQuantity(GoldVoucher100k));
        int missing500k = Math.Max(0, voucher500k - OwnedQuantity(GoldVoucher500k));
        int requiredGold = (missing100k * 100000) + (missing500k * 500000);
        if (Bot.Player.Gold < requiredGold)
        {
            Core.Logger($"WARNING: The selected Four Harbingers merge items require {requiredGold:N0} Gold for missing vouchers.", messageBox: true);
            return false;
        }

        MoveToInventory(GoldVoucher100k);
        MoveToInventory(GoldVoucher500k);

        if (voucher100k > 0 && Bot.Inventory.GetQuantity(GoldVoucher100k) < voucher100k)
            Core.BuyItem("fourharbingers", 2762, GoldVoucher100k, voucher100k);

        if (voucher500k > 0 && Bot.Inventory.GetQuantity(GoldVoucher500k) < voucher500k)
            Core.BuyItem("fourharbingers", 2762, GoldVoucher500k, voucher500k);

        if (Bot.Inventory.GetQuantity(GoldVoucher100k) < voucher100k
            || Bot.Inventory.GetQuantity(GoldVoucher500k) < voucher500k)
        {
            Core.Logger("WARNING: The required Four Harbingers Gold Vouchers could not be purchased.", messageBox: true);
            return false;
        }

        return true;
    }

    private bool AcquireMergeItem(string itemName, bool isRequirement)
    {
        if (Core.CheckInventory(itemName, toInv: false))
        {
            if (isRequirement)
                MoveToInventory(itemName);

            return true;
        }

        if (!MergeRecipes.TryGetValue(itemName, out MergeRecipe? recipe))
            return false;

        foreach (string requirement in recipe.Requirements.Keys)
        {
            if (MergeRecipes.ContainsKey(requirement) && !AcquireMergeItem(requirement, true))
                return false;
        }

        Core.BuyItem("fourharbingers", 2762, itemName);
        if (!Core.CheckInventory(itemName, toInv: false))
        {
            Core.Logger($"WARNING: {itemName} could not be merged.", messageBox: true);
            return false;
        }

        return true;
    }

    private void MoveToInventory(string itemName)
    {
        if (!Bot.Inventory.Contains(itemName) && Bot.Bank.Contains(itemName))
        {
            Bot.Bank.EnsureToInventory(itemName);
            Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);
        }
    }

    private int OwnedQuantity(string itemName)
    {
        return Bot.Inventory.Items
            .Concat(Bot.Bank.Items)
            .Where(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            .Sum(item => item.Quantity);
    }

    private int GetRecipeDepth(string itemName)
    {
        if (!MergeRecipes.TryGetValue(itemName, out MergeRecipe? recipe))
            return 0;

        int depth = 1;
        foreach (string requirement in recipe.Requirements.Keys)
            depth = Math.Max(depth, 1 + GetRecipeDepth(requirement));

        return depth;
    }

    private int GetRequirement(Dictionary<string, int> requirements, string itemName)
    {
        if (requirements.TryGetValue(itemName, out int quantity))
            return quantity;

        return 0;
    }

    private static Dictionary<string, MergeRecipe> CreateMergeRecipes()
    {
        Dictionary<string, MergeRecipe> recipes = new(StringComparer.OrdinalIgnoreCase);

        AddRecipe(recipes, "Celestial Dragon of Time", (GoldVoucher100k, 10), ("Scroll of the Heretic", 10));
        AddRecipe(recipes, "Infernal Dragon of Time", (GoldVoucher100k, 10), ("Scroll of the Heretic", 10));
        AddRecipe(recipes, "Harbinger of War", (GoldVoucher100k, 10), ("Scroll of the Preacher", 20));
        AddRecipe(recipes, "Harbinger of Famine", (GoldVoucher100k, 10), ("Scroll of the Benevolent", 20));
        AddRecipe(recipes, "Harbinger of Death", (GoldVoucher100k, 10), ("Scroll of the Innocent", 20));
        AddRecipe(recipes, "Harbinger of Conquest", (GoldVoucher100k, 10), ("Scroll of the Wanderer", 20));
        AddRecipe(recipes, "Bow of Halosis", (GoldVoucher100k, 4), ("Scroll of the Wanderer", 4));
        AddRecipe(recipes, "Halosis' Divine Wheel", (GoldVoucher100k, 2), ("Scroll of the Wanderer", 2));
        AddRecipe(recipes, "Wings of Mors", (GoldVoucher100k, 2), ("Scroll of the Innocent", 2));
        AddRecipe(recipes, "Bello's Armaments", (GoldVoucher100k, 2), ("Scroll of the Preacher", 2));
        AddRecipe(recipes, "Bello's Braziers", ("Bello's Brazier", 1), (GoldVoucher100k, 2), ("Scroll of the Preacher", 2));
        AddRecipe(recipes, "Bello's Sanctified Braziers", ("Bello's Sanctified Brazier", 1), (GoldVoucher100k, 2), ("Scroll of the Preacher", 2));
        AddRecipe(recipes, "Hunting Bow of Halosis", ("Bow of Halosis", 1), (GoldVoucher100k, 2), ("Scroll of the Wanderer", 2));
        AddRecipe(recipes, "Poise of Halosis", (GoldVoucher100k, 2), ("Scroll of the Wanderer", 2));
        AddRecipe(recipes, "Poise of Fames", (GoldVoucher100k, 2), ("Scroll of the Benevolent", 2));
        AddRecipe(recipes, "Poise of Mors", (GoldVoucher100k, 2), ("Scroll of the Innocent", 2));
        AddRecipe(recipes, "Poise of Bello", (GoldVoucher100k, 2), ("Scroll of the Preacher", 2));
        AddRecipe(recipes, "Infernal Dragon of Time Hood", (GoldVoucher100k, 2), ("Scroll of the Heretic", 2));
        AddRecipe(recipes, "Celestial Dragon of Time Hood", (GoldVoucher100k, 2), ("Scroll of the Heretic", 2));
        AddRecipe(recipes, "Bello's Brazier", (GoldVoucher100k, 4), ("Scroll of the Preacher", 4));
        AddRecipe(recipes, "Fames' Barren Scale", (GoldVoucher100k, 4), ("Scroll of the Benevolent", 4));
        AddRecipe(recipes, "Bello's Sanctified Brazier", (GoldVoucher100k, 4), ("Scroll of the Preacher", 4));
        AddRecipe(recipes, "Nightstar Companion",
            ("Celestial Dragon of Time", 1),
            ("Infernal Dragon of Time", 1),
            ("Harbinger of Conquest", 1),
            ("Harbinger of Death", 1),
            ("Harbinger of Famine", 1),
            ("Harbinger of War", 1),
            (GoldVoucher500k, 30));
        AddRecipe(recipes, "Scythe of Mors", (GoldVoucher100k, 4), ("Scroll of the Innocent", 4));

        return recipes;
    }

    private static void AddRecipe(Dictionary<string, MergeRecipe> recipes, string itemName,
        params (string ItemName, int Quantity)[] requirements)
    {
        recipes[itemName] = new MergeRecipe(requirements);
    }

    private class MergeRecipe
    {
        public Dictionary<string, int> Requirements { get; }

        public MergeRecipe(IEnumerable<(string ItemName, int Quantity)> requirements)
        {
            Requirements = requirements.ToDictionary(
                requirement => requirement.ItemName,
                requirement => requirement.Quantity,
                StringComparer.OrdinalIgnoreCase
            );
        }
    }

    public void RefreshPotion(string auraName)
    {
        if (Bot.Player.InCombat && !Bot.Self.HasActiveAura(auraName) && Bot.Skills.CanUseSkill(5))
            Bot.Skills.UseSkill(5);
    }

    public bool HasMonsterAura(int mapID, string auraName)
    {
        try
        {
            List<Aura>? auras = JsonConvert.DeserializeObject<List<Aura>>(Bot.Target.GetMonsterAura(mapID));
            if (auras == null)
                return false;

            return auras.Any(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    public void WarnEnhancementFallback(string message)
    {
        Core.Logger($"WARNING: {message} The script may fail.", messageBox: true);
    }

    private void WarnPotion(string itemName, string reason)
    {
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");
    }
}
