/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.Options;
using Skua.Core.Utils;

public class CoreAdvanced
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced? _instance;
    public static CoreAdvanced Instance => _instance ??= new CoreAdvanced();

    /// <summary>
    /// Auto-registers the OnEquipClass hook so EquipClass auto-enhances when CoreAdvanced is included.
    /// </summary>
    static CoreAdvanced()
    {
        CoreBots.OnEquipClass += (c) =>
        {
            try { Instance.SmartEnhance(c); }
            catch (Exception ex) { IScriptInterface.Instance.Log($"[Enhancement] SmartEnhance failed for {c}: {ex.Message}"); }
        };
    }

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    #region Shop

    /// <summary>
    /// Buys an item by name from a shop, handling requirements such as XP, Rep, Gold, and merge items.
    /// Safely respects stack sizes, inventory, and optional merge index.
    /// </summary>
    public void BuyItem(string map, int shopID, string itemName, int quant = 1, int shopItemID = 0, int index = 0, bool Log = true)
    {
        if (Core.CheckInventory(itemName, quant))
            return;

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);
        Core.JumpWait();

        // Get full shop list; do not pre-filter
        List<ShopItem> shopItems = Core.GetShopItems(map, shopID);

        ShopItem? item = Core.parseShopItem(shopItems, shopID, itemName, shopItemID);
        if (item == null)
            return;

        int shopQuant = item.Quantity > 0 ? item.Quantity : 1;

        _BuyItem(map, shopID, item, quant, shopQuant, shopItemID, index, Log);
    }

    /// <summary>
    /// Buys an item by ID from a shop, handling requirements such as XP, Rep, Gold, and merge items.
    /// Safely respects stack sizes, inventory, and optional merge index.
    /// </summary>
    public void BuyItem(string map, int shopID, int itemID, int quant = 1, int shopQuant = 1, int shopItemID = 0, int index = 0, bool Log = true)
    {
        if (Core.CheckInventory(itemID, quant))
            return;

        // Inventory space check
        if (Bot.Inventory.FreeSlots <= 0 && !Bot.Inventory.Contains(itemID))
        {
            if (Log) Core.Logger("❌ Inventory full, cannot buy items.");
            return;
        }

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);
        Core.JumpWait();

        // Wait for combat to end
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
        {
            Core.JumpWait();
            Bot.Wait.ForCombatExit();
        }

        // Get full shop list
        List<ShopItem> shopItems = Core.GetShopItems(map, shopID);
        ShopItem? item = Core.parseShopItem(shopItems, shopID, itemID, shopItemID);
        if (item == null)
        {
            if (Log) Core.Logger($"❌ Item {itemID} not found in shop {shopID} on {map}");
            return;
        }

        // House space check if item is a house-storable category
        if (!string.IsNullOrEmpty(item.CategoryString) && Core.CategoryStrings.Contains(item.CategoryString))
        {
            // Check for house item space
            if (Bot.House.FreeSlots <= 0 && !Bot.House.Contains(itemID))
                Core.BankACHouseItems();

            //Recheck after banking a house items that arent equiped
            if (Bot.House.FreeSlots <= 0 && !Bot.House.Contains(itemID))
            {
                if (Log)
                    Core.Logger("❌ House full, cannot store this item.");
                return;
            }
        }

        int effectiveShopQuant = item.Quantity > 0 ? item.Quantity : shopQuant;

        _BuyItem(map, shopID, item, quant, effectiveShopQuant, shopItemID, index, Log);
    }

    private void _BuyItem(string map, int shopID, ShopItem item, int quant = 1, int shopQuant = 1, int shopItemID = 1, int index = 0, bool Log = true)
    {
        // Quantity per purchase from shop
        int itemStack = item.Quantity > 0 ? item.Quantity : 1;

        // Handle requirements first
        if (item.Requirements != null)
        {
            foreach (ItemBase req in item.Requirements)
            {
                int stacksNeeded = (int)Math.Ceiling((double)quant / itemStack);
                int totalNeeded = stacksNeeded * req.Quantity;

                if (Core.CheckInventory(req.ID, totalNeeded))
                    continue;

                // Special farm cases
                if (req.Name.Contains("Gold Voucher"))
                {
                    Farm.Voucher(req.Name, totalNeeded);
                    continue;
                }

                if (req.Name == "Dragon Runestone")
                {
                    Farm.DragonRunestone(totalNeeded);
                    continue;
                }

                // Try to buy from shop if available
                while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, totalNeeded))
                {
                    if (Bot.Map.Name != map)
                        Core.Join(map);

                    ShopItem? shopReqItem = Core.GetShopItems(map, shopID)
                        .FirstOrDefault(x => x.ID == req.ID);

                    if (shopReqItem != null)
                        BuyItem(map, shopID, req.ID, totalNeeded, shopReqItem.ShopItemID, Log: Log);
                    else
                    {
                        Core.Logger(
                            $"Missing requirement: {req.Name} [{req.ID}] in shop {shopID} on map {map}. " +
                            $"It may be a drop, daily, or special item."
                        );
                        return;
                    }
                }
            }
        }

        // Ensure requirements are satisfied before main purchase
        GetItemReq(item, quant);

        // Rejoin map & load shop safely
        if (Bot.Map.Name != map)
            Core.Join(map);

        List<ShopItem> shopItems = Core.GetShopItems(map, shopID)
            .Where(x =>
                x.ID == item.ID &&
                !(x.Coins && x.Cost > 0) &&
                (item.Requirements?.All(r => Core.CheckInventory(r.ID, r.Quantity)) ?? true)
            )
            .ToList();

        ShopItem? mainItem = shopItems.Count > index ? shopItems[index] : shopItems.FirstOrDefault();

        if (mainItem == null)
        {
            Core.Logger($"❌ Failed to find {item.Name} in shop {shopID} on map {map}");
            return;
        }

        // Calculate buy amount respecting stack size and max stack
        int currentStock = Bot.Inventory.Items
            .Concat(Bot.Bank.Items)
            .Concat(Bot.House.Items)
            .Concat(Bot.TempInv.Items)
            .Where(x => x.ID == mainItem.ID)
            .Sum(x => x.Quantity);

        int buyAmount = Core._CalcBuyQuantity(mainItem, quant);

        if (buyAmount <= 0)
        {
            Core.Logger($"Cannot buy {mainItem.Name}, max stack reached ({currentStock}/{mainItem.MaxStack})");
            return;
        }

        Core.BuyItem(map, shopID, mainItem.ID, buyAmount, mainItem.ShopItemID != 1 ? mainItem.ShopItemID : shopItemID, index: index, Log: Log);

        Core.Sleep();

        // Verify purchase
        if (!Core.CheckInventory(mainItem.ID, quant))
        {
            Core.Logger($"❌ Failed to buy {mainItem.Name} ({quant}x)");

            foreach (var req in mainItem.Requirements.Where(r => r != null && !Core.CheckInventory(r.ID, r.Quantity)))
                Core.Logger($"⚠️ Missing requirement: {req.Name} x{req.Quantity}");
        }
    }

    /// <summary>
    /// Ensures that all necessary requirements (Experience, Reputation, Gold, and specific items)
    /// are met in order to purchase an item. This includes verifying player level, farming or purchasing
    /// required reputation, acquiring specific items such as Gold Vouchers and Dragon Runestones,
    /// and ensuring enough gold is available for the transaction.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ShopItem"/> object that contains all the details about the item,
    /// including its requirements like reputation, level, gold cost, and additional items needed.
    /// </param>
    /// <param name="quant">
    /// The quantity of the item needed for purchase. The default value is 1, but can be adjusted
    /// to handle cases where multiple units of the item are required.
    /// </param>
    public void GetItemReq(ShopItem item, int quant = 1)
    {
        if (item?.Requirements == null)
        {
            Core.Logger("Invalid item or missing requirements.");
            return;
        }

        // Ensure required reputation for faction-based items
        if (
            !string.IsNullOrEmpty(item.Faction)
            && item.Faction != "None"
            && item.RequiredReputation > 0
            && Farm.FactionRank(item.Faction) < item.RequiredReputation
        )
        {
            Core.Logger($"Farming reputation for {item.Faction} (Required: {item.RequiredReputation})");
            runRep(item.Faction, Core.PointsToLevel(item.RequiredReputation));
        }

        // Level up if the item requires a higher player level
        if (item.Level > Bot.Player.Level)
        {
            Core.Logger($"Farming experience to reach level {item.Level}");
            Farm.Experience(Math.Min(item.Level, 100));
        }

        // Farm gold if the item costs gold and isn't a premium currency purchase
        if (!item.Coins && item.Cost > 0)
        {
            int GoldtoFarm = Math.Min(item.Cost * quant, 100000000); // 100m gold cap
            Farm.Gold(GoldtoFarm);
        }

        // Handle Gold Vouchers (multiple types possible)
        if (item.Requirements.Any(x => x != null && x.Name.StartsWith("Gold Voucher")))
        {
            foreach (
                ItemBase req in item.Requirements.Where(x =>
                    x != null && x.Name.StartsWith("Gold Voucher")
                )
            )
            {
                Farm.Voucher(req.Name, req.Quantity);
            }
        }

        // Handle Dragon Runestone farming if required
        if (
            item.Requirements != null
            && item.Requirements.Any(x => x != null && x.Name.StartsWith("Dragon Runestone"))
        )
        {
            ItemBase? runestoneReq = item.Requirements.FirstOrDefault(x =>
                x != null && x.Name == "Dragon Runestone"
            );
            if (runestoneReq != null)
                Farm.DragonRunestone(runestoneReq.Quantity);
        }

        // Warn if a temp item is missing
        if (item.Requirements != null)
            foreach (
                ItemBase req in item.Requirements.Where(x =>
                    x != null && x.Temp && x.Quantity > Bot.TempInv.GetQuantity(x.ID)
                )
            )
                Core.Logger(
                    $"Temp item: {req.Name}, quant needed: {req.Quantity}... did the bot not farm them?"
                );
    }

    private void runRep(string faction, int rank)
    {
        faction = faction.Replace(" ", "");
        Type farmClass = Farm.GetType();
        MethodInfo? theMethod = farmClass.GetMethod(faction + "REP");
        if (theMethod == null)
        {
            Core.Logger(
                "Failed to find "
                    + faction
                    + "REP. Make sure you have the correct name and capitalization."
            );
            return;
        }
        try
        {
            switch (faction.ToLower())
            {
                case "alchemy":
                case "blacksmith":
                    theMethod.Invoke(Farm, new object[] { rank, true });
                    break;
                case "bladeofawe":
                    theMethod.Invoke(Farm, new object[] { rank, false });
                    break;
                default:
                    theMethod.Invoke(Farm, new object[] { rank });
                    break;
            }
        }
        catch
        {
            Core.Logger(
                $"Faction {faction} has invalid paramaters, please report",
                messageBox: true,
                stopBot: true
            );
        }
    }

    #region old StartBuyAllMerge, revert if broke
    /// <summary>
    /// Buys merge items from a shop based on specified options. Filters ShopItems to ensure uniqueness by ID and ShopItemID,
    /// selecting items based on Upgrade requirements and excluding those ending with "insignia".
    /// </summary>
    /// <param name="map">The map from which the shop is loaded.</param>
    /// <param name="shopID">The shop ID to load shop data.</param>
    /// <param name="findIngredients">Action determining where to retrieve items.</param>
    /// <param name="buyOnlyThis">Optional. Limits purchases to a specific item.</param>
    /// <param name="itemBlackList">Optional. List of excluded items.</param>
    /// <param name="buyMode">Optional. Specifies buying mode.</param>
    /// <param name="Group">Optional. Specifies group selection method.</param>
    /// <param name="ShopItemID">Optional. Specifies ShopItem ID.</param>
    /// <param name="Log">Optional. Enables logging.</param>
    // We'll use this later
    // public void StartBuyAllMerge(
    //     string map,
    //     int shopID,
    //     Action findIngredients,
    //     string? buyOnlyThis = null,
    //     string[]? itemBlackList = null,
    //     mergeOptionsEnum? buyMode = null,
    //     string Group = "First",
    //     int ShopItemID = 0,
    //     bool Log = true
    // )
    // {
    //     #region Setup and Initialization
    //     if (
    //         buyOnlyThis == null
    //         && buyMode == null
    //         && Bot.Config != null
    //         && !Bot.Config.Get<bool>(CoreBots.Instance.SkipOptions)
    //     )
    //         Bot.Config!.Configure();

    //     int mode = 0;
    //     if (buyOnlyThis != null)
    //         mode = (int)mergeOptionsEnum.all;
    //     else if (buyMode != null)
    //         mode = (int)buyMode;
    //     else if (
    //         Bot.Config != null
    //         && Bot.Config.MultipleOptions.Any(o =>
    //             o.Value.Any(x => x.Category == "Generic" && x.Name == "mode")
    //         )
    //     )
    //         mode = (int)Bot.Config.Get<mergeOptionsEnum>("Generic", "mode");
    //     else
    //         Core.Logger(
    //             "Invalid setup detected for StartBuyAllMerge. Please report",
    //             messageBox: true,
    //             stopBot: true
    //         );

    //     matsOnly = mode == 2;

    //     // HashSet for tracking unique item IDs to prevent redundant operations
    //     HashSet<int> uniqueItemIds = new(
    //         new[]
    //         {
    //             Bot.Bank.Items.Select(item => item.ID),
    //             Bot.TempInv.Items.Select(item => item.ID),
    //             Bot.House.Items.Select(item => item.ID),
    //             Bot.Inventory.Items.Select(item => item.ID),
    //         }.SelectMany(id => id)
    //     );

    //     // Filter shop items based on various conditions
    //     List<ShopItem> shopItems = Core.GetShopItems(map, shopID)
    //         .GroupBy(item => new
    //         {
    //             item.Name,
    //             item.ID,
    //             item.ShopItemID,
    //         })
    //         .Select(group =>
    //         {
    //             IOrderedEnumerable<ShopItem> orderedGroup = group.OrderBy(item =>
    //                 item.ShopItemID != group.First().ShopItemID
    //             );
    //             return Group == "First" ? orderedGroup.First() : orderedGroup.Last();
    //         })
    //         .Where(x => !x.Name.ToLower().EndsWith("insignia"))
    //         .Where(x => !uniqueItemIds.Contains(x.ID))
    //         .ToList();

    //     uniqueItemIds = new HashSet<int>(); // Reset for re-use

    //     List<ShopItem> items = new();
    //     bool memSkipped = false;

    //     // Process shop items based on various conditions
    //     foreach (ShopItem item in shopItems)
    //     {
    //         if (
    //             miscCatagories.Contains(item.Category)
    //             || (!string.IsNullOrEmpty(buyOnlyThis) && buyOnlyThis != item.Name)
    //             || (
    //                 itemBlackList != null
    //                 && itemBlackList.Any(x => x.ToLower() == item.Name.ToLower())
    //             )
    //         )
    //             continue;

    //         if (
    //             Core.IsMember
    //             || !item.Upgrade
    //             || item.Requirements.Any(x =>
    //                 x != null && Bot.Shops.Items.Any(x => x != null && x.Upgrade && !Core.IsMember)
    //             )
    //         )
    //         {
    //             if (mode == 3)
    //             {
    //                 if (Bot.Config!.Get<bool>("Select", $"{item.ID}"))
    //                     items.Add(item);
    //             }
    //             else if (mode != 1)
    //                 items.Add(item);
    //             else if (item.Coins)
    //                 items.Add(item);
    //         }
    //         else if (mode == 3 && Bot.Config!.Get<bool>("Select", $"{item.ID}"))
    //         {
    //             Core.Logger($"\"{item.Name}\" will be skipped, as you aren't a member.");
    //             memSkipped = true;
    //         }
    //     }

    //     if (items.Count == 0)
    //     {
    //         Core.Logger(
    //             $"Found {items.Count} items to purchase from shop [{shopID}] on map [{map}]."
    //         );
    //         HandleNoItemsFound(mode, memSkipped);
    //         return;
    //     }
    //     #endregion

    //     int t = 0;
    //     // Why did we need the `for ( int i = 0; i < 2; i++)`?

    //     foreach (ShopItem item in items)
    //     {
    //         if (Core.CheckInventory(item.ID, toInv: false))
    //             continue;

    //         if (item.Upgrade && !Core.IsMember)
    //         {
    //             Core.Logger($"Skipping {item.Name} [{item.ID}] as it is member-only.");
    //             continue;
    //         }

    //         if (!matsOnly)
    //         {
    //             Core.Logger($"Farming to buy {item.Name} (#{t++}/{items.Count})");
    //         }

    //         foreach (ItemBase req in item.Requirements)
    //         {
    //             EnsureShopLoaded(map, shopID);
    //             HandleItemRequirements(req, req.Quantity, findIngredients);
    //         }

    //         if (item.Requirements.All(x => x != null && Core.CheckInventory(x.ID, x.Quantity)))
    //         {
    //             if (!matsOnly)
    //                 Core.Logger($"Buying {item.Name} (#{t++}/{items.Count})");

    //             // Attempt to purchase the required quantity of the shop item
    //             BuyItem(map, shopID, item.ID, shopItemID: item.ShopItemID, Log: Log);
    //             Bot.Wait.ForPickup(item.ID);
    //             if (Core.CheckInventory(item.ID, item.Quantity))
    //             {
    //                 continue;
    //             }
    //             else
    //             {
    //                 IEnumerable<string> missing = item
    //                     .Requirements.Where(x => !Core.CheckInventory(x.ID, x.Quantity))
    //                     .Select(x => $"\"{x.Name} x{x.Quantity}\"");

    //                 Core.Logger(
    //                     $"Failed to meet requirements for {item.Name} [{item.ID}] due to missing: {string.Join(", ", missing)}."
    //                 );
    //                 continue;
    //             }
    //         }
    //     }

    //     // Helper methods
    //     bool EnsureShopLoaded(string? map, int shopID)
    //     {
    //         if (map == null)
    //         {
    //             Core.Logger("Map is null, unable to load shop.");
    //             return false;
    //         }
    //         Core.Join(map);
    //         Bot.Wait.ForMapLoad(map);
    //         while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
    //         {
    //             Bot.Shops.Load(shopID);
    //             Bot.Wait.ForActionCooldown(GameActions.LoadShop);
    //             Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
    //             Core.Sleep(1000);
    //             if (Bot.Shops.ID == shopID)
    //                 return true;
    //         }
    //         return true;
    //     }

    //     void HandleItemRequirements(ItemBase? Req, int ReqQuant, Action findIngredients)
    //     {
    //         if (Req == null)
    //         {
    //             Core.Logger("Requirement item is null, cannot process.");
    //             return;
    //         }
    //         if (Core.CheckInventory(Req.ID, ReqQuant))
    //             return;

    //         EnsureShopLoaded(map, shopID);
    //         ShopItem? wasinshop = Bot.Shops.Items.FirstOrDefault(x => x.ID == Req.ID);

    //         if (wasinshop != null)
    //         {
    //             Core.Logger($"Item: \"{Req.Name}  [{Req.ID}\"] is in the shop!");
    //             while (!Bot.ShouldExit && !Core.CheckInventory(Req.ID, ReqQuant))
    //             {
    //                 // for requirements that are in the shop, but are just buyable with gold. (excludes ac buyable items)
    //                 if (
    //                     wasinshop.Requirements.Count == 0
    //                     && ((wasinshop.Coins && wasinshop.Cost <= 0) || !wasinshop.Coins)
    //                 ) //|| wasinshop.Name.Contains("Gold Voucher") || wasinshop.Name.Contains("Dragon Runestone"))
    //                 {
    //                     // Otherwise buy the item directly
    //                     BuyItem(
    //                         map,
    //                         shopID,
    //                         Req.ID,
    //                         ReqQuant,
    //                         shopItemID: wasinshop.ShopItemID,
    //                         Log: Log
    //                     );
    //                     Bot.Wait.ForPickup(Req.ID);
    //                 }
    //                 else
    //                 {
    //                     // Items not in the shop, so we have to get it externally
    //                     if (wasinshop.Name.Contains("Dragon Runestone"))
    //                     {
    //                         Farm.DragonRunestone(ReqQuant);
    //                         continue;
    //                     }
    //                     if (wasinshop.Name.Contains("Gold Voucher"))
    //                     {
    //                         Farm.Voucher(wasinshop.Name, ReqQuant);
    //                         continue;
    //                     }
    //                     IngredientWasintheShop(wasinshop, ReqQuant);
    //                 }

    //                 if (Core.CheckInventory(Req.ID, ReqQuant))
    //                     break;
    //                 else
    //                 {
    //                     Core.Logger(
    //                         $"Failed to meet requirements for \"{Req.Name}\" [{Req.ID}] x{ReqQuant}, Retrying the farm (items may have been used)."
    //                     );
    //                 }
    //             }
    //         }
    //         else if (
    //             Req?.Name?.Contains("Gold Voucher") == true
    //             || Req?.Name?.Contains("Dragon Runestone") == true
    //         )
    //         {
    //             // Handle special cases for Gold Vouchers and Dragon Runestones
    //             if (Req.Name?.Contains("Gold Voucher") == true)
    //             {
    //                 Farm.Voucher(Req.Name, ReqQuant);
    //                 return;
    //             }

    //             if (Req.Name?.Contains("Dragon Runestone") == true)
    //             {
    //                 Farm.DragonRunestone(ReqQuant);
    //                 return;
    //             }
    //         }
    //         else if (wasinshop == null)
    //         {
    //             // Items not in the shop, so we have to get it externally
    //             if (Req != null)
    //             {
    //                 externalItem = Req;
    //                 externalQuant = Req.Quantity;
    //                 Core.AddDrop(externalItem.ID);
    //                 Core.Logger(
    //                     $"{externalItem.Name} [{externalItem.ID}] is an external item (not from this shop), attempting to farm it from The ingredient list."
    //                 );

    //                 findIngredients();
    //                 Bot.Wait.ForPickup(externalItem.ID);
    //             }
    //             else
    //             {
    //                 Core.Logger("Cannot process null requirement item.");
    //                 return;
    //             }
    //         }
    //         Bot.Wait.ForPickup(Req!.ID);
    //     }

    //     void IngredientWasintheShop(ShopItem item, int craftingQ)
    //     {
    //         // Ensure we are checking for items in the shop and inventory properly
    //         if (item == null)
    //         {
    //             Core.Logger($"Item not found in the shop.");
    //             return;
    //         }

    //         // If item is already in inventory, no need to continue
    //         if (Core.CheckInventory(item.ID, item.Quantity))
    //             return;

    //         // Ensure shop is loaded before proceeding
    //         EnsureShopLoaded(map, shopID);
    //         foreach (ItemBase req in item.Requirements)
    //         {
    //             if (Core.CheckInventory(req.ID, req.Quantity))
    //                 continue;

    //             EnsureShopLoaded(map, shopID);
    //             int ReqQuant = req.Quantity * craftingQ;
    //             ShopItem? wasinshop = Bot.Shops.Items.FirstOrDefault(x => x.ID == req.ID);

    //             if (wasinshop != null && !MergeItemisinShopExceptions.Contains(req.Name))
    //             {
    //                 Core.Logger($"Item: \"{wasinshop.Name}  [{wasinshop.ID}\"] is in the shop.");
    //                 ReqQuant = Math.Min(ReqQuant, wasinshop.MaxStack);
    //                 // for requirements that are in the shop, but are just buyable with gold. (excludes ac buyable items)
    //                 if (wasinshop.Requirements.Count <= 0 && wasinshop.Cost <= 0)
    //                 {
    //                     BuyItem(
    //                         map,
    //                         shopID,
    //                         wasinshop.ID,
    //                         ReqQuant,
    //                         shopItemID: wasinshop.ShopItemID,
    //                         Log: Log
    //                     );
    //                     Bot.Wait.ForPickup(wasinshop.ID);
    //                 }
    //                 else
    //                 {
    //                     // Items not in the shop, so we have to get it externally
    //                     if (req.Name.Contains("Dragon Runestone"))
    //                     {
    //                         Farm.DragonRunestone(ReqQuant);
    //                         continue;
    //                     }
    //                     if (req.Name.Contains("Gold Voucher"))
    //                     {
    //                         Farm.Voucher(req.Name, ReqQuant);
    //                         continue;
    //                     }
    //                     // Core.Logger($"Requirements: {string.Join(", ", wasinshop.Requirements)}");
    //                     IngredientWasintheShop(wasinshop, ReqQuant);
    //                 }
    //                 continue;
    //             }
    //             else if (wasinshop == null || MergeItemisinShopExceptions.Contains(req.Name))
    //             {
    //                 // Items not in the shop, so we have to get it externally
    //                 if (req.Name.Contains("Dragon Runestone"))
    //                 {
    //                     Farm.DragonRunestone(ReqQuant);
    //                     continue;
    //                 }
    //                 if (req.Name.StartsWith("Gold Voucher"))
    //                 {
    //                     Farm.Voucher(req.Name, ReqQuant);
    //                     continue;
    //                 }
    //                 externalItem = req;
    //                 externalQuant = ReqQuant;
    //                 Core.AddDrop(externalItem.ID);
    //                 Core.Logger(
    //                     $"{externalItem.Name} [{externalItem.ID}] is an external item (not a shop item), attempting to farm it from The ingredient list."
    //                 );
    //                 findIngredients();
    //             }
    //         }

    //         EnsureShopLoaded(map, shopID);
    //         if (item.Requirements.All(x => x != null && Core.CheckInventory(x.ID, x.Quantity)))
    //         {
    //             // If all requirements are met, attempt to buy the item
    //             if (!matsOnly)
    //                 Core.Logger($"Buying {item.Name} [{item.ID}] from the shop.");

    //             // Attempt to purchase the Requirement of Main / Sub-Main item
    //             BuyItem(map, shopID, item.ID, craftingQ, shopItemID: item.ShopItemID, Log: Log);
    //             Bot.Wait.ForPickup(item.ID);
    //         }
    //         else
    //             // If the purchase was unsuccessful, log the failure
    //             Core.Logger($"Failed to meet requirements for {item.Name} [{item.ID}].");
    //     }

    //     void HandleNoItemsFound(int mode, bool memSkipped)
    //     {
    //         if (buyOnlyThis != null)
    //             return;

    //         switch (mode)
    //         {
    //             case 0:
    //             case 2:
    //                 Core.Logger("The bot fetched 0 items to farm. Something must have gone wrong.");
    //                 break;
    //             case 1:
    //                 if (shopItems.All(x => !x.Coins))
    //                     Core.Logger(
    //                         "The bot fetched 0 items to farm. This is because none of the items in this shop are AC tagged."
    //                     );
    //                 else
    //                     Core.Logger(
    //                         "The bot fetched 0 items to farm. Something must have gone wrong."
    //                     );
    //                 break;
    //             case 3:
    //                 if (memSkipped)
    //                     Core.Logger(
    //                         "The bot fetched 0 items to farm. This is because you aren't a member."
    //                     );
    //                 else
    //                     Core.Logger(
    //                         "The bot fetched 0 items to farm. Something must have gone wrong."
    //                     );
    //                 break;
    //         }
    //     }
    // }
    #endregion

    /// <summary>
    /// Buys merge items from a shop based on specified options. Filters ShopItems to ensure uniqueness by ID and ShopItemID,
    /// selecting items based on Upgrade requirements and excluding those ending with "insignia".
    /// </summary>
    /// <param name="map">The map from which the shop is loaded.</param>
    /// <param name="shopID">The shop ID to load shop data.</param>
    /// <param name="findIngredients">Action determining where to retrieve items.</param>
    /// <param name="buyOnlyThis">Optional. Limits purchases to a specific item.</param>
    /// <param name="itemBlackList">Optional. List of excluded items.</param>
    /// <param name="buyMode">Optional. Specifies buying mode.</param>
    /// <param name="Group">Optional. Specifies group selection method.</param>
    /// <param name="ShopItemID">Optional. Specifies ShopItem ID.</param>
    /// <param name="Log">Optional. Enables logging.</param>
    public void StartBuyAllMerge(string map, int shopID, Action findIngredients, string? buyOnlyThis = null, string[]? itemBlackList = null, mergeOptionsEnum? buyMode = null, string Group = "First", int ShopItemID = 0, bool Log = true)
    {
        #region Setup and Initialization
        if (
            buyOnlyThis == null
            && buyMode == null
            && Bot.Config != null
            && !Bot.Config.Get<bool>(CoreBots.Instance.SkipOptions)
        )
            Bot.Config!.Configure();

        int mode = 0;
        if (buyOnlyThis != null)
            mode = (int)mergeOptionsEnum.all;
        else if (buyMode != null)
            mode = (int)buyMode;
        else if (
            Bot.Config != null
            && Bot.Config.MultipleOptions.Any(o =>
                o.Value.Any(x => x.Category == "Generic" && x.Name == "mode")
            )
        )
            mode = (int)Bot.Config.Get<mergeOptionsEnum>("Generic", "mode");
        else
            Core.Logger(
                "Invalid setup detected for StartBuyAllMerge. Please report",
                messageBox: true,
                stopBot: true
            );

        matsOnly = mode == 2;

        // HashSet for tracking unique item IDs to prevent redundant operations
        HashSet<int> uniqueItemIds = new(
            new[]
            {
                Bot.Bank.Items.Select(item => item.ID),
                Bot.TempInv.Items.Select(item => item.ID),
                Bot.House.Items.Select(item => item.ID),
                Bot.Inventory.Items.Select(item => item.ID),
            }.SelectMany(id => id)
        );

        // Filter shop items based on various conditions
        List<ShopItem> shopItems = Core.GetShopItems(map, shopID)
            .GroupBy(item => new
            {
                item.Name,
                item.ID,
                item.ShopItemID,
            })
            .Select(group =>
            {
                IOrderedEnumerable<ShopItem> orderedGroup = group.OrderBy(item =>
                    item.ShopItemID != group.First().ShopItemID
                );
                return Group == "First" ? orderedGroup.First() : orderedGroup.Last();
            })
            .Where(x => !x.Name.ToLower().EndsWith("insignia"))
            .Where(x => !uniqueItemIds.Contains(x.ID))
            .ToList();

        uniqueItemIds = new HashSet<int>(); // Reset for re-use

        List<ShopItem> items = new();
        bool memSkipped = false;

        // Process shop items based on various conditions
        foreach (ShopItem item in shopItems)
        {
            if (
                miscCatagories.Contains(item.Category)
                || (!string.IsNullOrEmpty(buyOnlyThis) && buyOnlyThis != item.Name)
                || (
                    itemBlackList != null
                    && itemBlackList.Any(x => x.ToLower() == item.Name.ToLower())
                )
            )
                continue;

            if (
                Core.IsMember
                || !item.Upgrade
                || item.Requirements.Any(x =>
                    x != null && Bot.Shops.Items.Any(x => x != null && x.Upgrade && !Core.IsMember)
                )
            )
            {
                if (mode == 3)
                {
                    if (Bot.Config!.Get<bool>("Select", $"{item.ID}"))
                        items.Add(item);
                }
                else if (mode != 1)
                    items.Add(item);
                else if (item.Coins)
                    items.Add(item);
            }
            else if (mode == 3 && Bot.Config!.Get<bool>("Select", $"{item.ID}"))
            {
                Core.Logger($"\"{item.Name}\" will be skipped, as you aren't a member.");
                memSkipped = true;
            }
        }

        if (items.Count <= 0)
        {
            HandleNoItemsFound(mode, memSkipped);
            return;
        }
        #endregion



        Dictionary<int, int> acquiredItems = new();
        int t = 0;

        foreach (ShopItem item in items)
        {
            if (Core.CheckInventory(item.ID, toInv: false))
            {
                Core.Logger($"{item.Name} Owned x{Bot.Inventory.GetQuantity(item.ID)}/{1}");
                continue;
            }

            if (item.Upgrade && !Core.IsMember)
            {
                Core.Logger($"Skipping {item.Name} [{item.ID}] as it is member-only.");
                continue;
            }

            if (!matsOnly)
            {
                Core.Logger($"Farming to buy {item.Name} (#{t++}/{items.Count})");
            }

            // Process all requirements for this item
            ProcessItemWithDependencies(item, 1, map, shopID, findIngredients, acquiredItems: acquiredItems);

            // After dependencies are handled, check if we can buy the main item
            EnsureShopLoaded(map, shopID);
            if (item.Requirements.All(x => x != null && Core.CheckInventory(x.ID, x.Quantity)))
            {
                if (!matsOnly)
                    Core.Logger($"Buying {item.Name} (#{t}/{items.Count})");

                BuyItem(map, shopID, item.ID, shopItemID: item.ShopItemID, Log: Log);
                Bot.Wait.ForPickup(item.ID);

                if (!Core.CheckInventory(item.ID, item.Quantity))
                {
                    IEnumerable<string> missing = item
                        .Requirements.Where(x => x != null && !Core.CheckInventory(x.ID, x.Quantity))
                        .Select(x => $"\"{x.Name} x{x.Quantity}\"");

                    Core.Logger(
                        $"Failed to meet requirements for {item.Name} [{item.ID}] due to missing: {string.Join(", ", missing)}."
                    );
                }
            }
        }

        #region Helper Methods

        bool EnsureShopLoaded(string? map, int shopID)
        {
            if (map == null)
            {
                Core.Logger("Map is null, unable to load shop.");
                return false;
            }
            Core.Join(map);
            Bot.Wait.ForMapLoad(map);
            while (!Bot.ShouldExit && Bot.Shops.ID != shopID)
            {
                Bot.Shops.Load(shopID);
                Bot.Wait.ForActionCooldown(GameActions.LoadShop);
                Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == shopID, 20);
                Core.Sleep(1000);
                if (Bot.Shops.ID == shopID)
                    return true;
            }
            return true;
        }

        void ProcessItemWithDependencies(ItemBase item, int quantity, string map, int shopID, Action findIngredients, int depth = 0, Dictionary<int, int>? acquiredItems = null)
        {
            acquiredItems ??= []; // Initialize if null
            const int MAX_DEPTH = 15;

            if (depth > MAX_DEPTH)
            {
                Core.Logger($"Max recursion depth reached for {item.Name}");
                return;
            }

            if (item == null)
            {
                Core.Logger("Item is null, cannot process.");
                return;
            }

            // If already have enough, skip
            if (Core.CheckInventory(item.ID, quantity))
            {
                return;
            }

            EnsureShopLoaded(map, shopID);

            // Look up the item in the shop to get its requirements
            ShopItem? shopItem = Bot.Shops.Items.FirstOrDefault(x => x.ID == item.ID);
            if (shopItem == null)
            {
                Core.Logger("ShopItem Returned null.. some how", "ProcessItemWithDependencies");
                return;
            }

            // Process all requirements first
            foreach (ItemBase req in shopItem.Requirements)
            {
                if (req == null)
                {
                    continue;
                }

                int requiredQty = req.Quantity * quantity;
                Core.FarmingLogger(req.Name, requiredQty);
                // Check if already have it
                if (Core.CheckInventory(req.ID, requiredQty))
                {
                    Core.Logger(
                        $"{req.Name} Owned x{Bot.Inventory.GetQuantity(req.ID)}/{requiredQty}"
                    );
                    continue;
                }
                EnsureShopLoaded(map, shopID);
                ShopItem? reqInShop = Bot.Shops.Items.FirstOrDefault(x => x.ID == req.ID);

                // if (reqInShop != null && reqInShop.Requirements.Any(x => !Bot.Shops.Items.Contains(x)))
                //     MergeItemisinShopExceptions.Add(reqInShop.Name);

                // Check exceptions first - these should always be farmed/merged, never bought
                if (req != null && MergeItemisinShopExceptions.Contains(req.Name!))
                {
                    Core.Logger(
                        $"{req.Name} [{req.ID}] is in merge exceptions, using merge script."
                    );
                    externalItem = req;
                    externalQuant = requiredQty;
                    Core.AddDrop(externalItem.ID);
                    findIngredients();
                    Bot.Wait.ForPickup(req.ID);
                    acquiredItems[req.ID] = Bot.Inventory.GetQuantity(req.ID);
                    continue;
                }

                if (req!.Name?.Contains("Dragon Runestone") == true)
                {
                    Core.Logger($"Farming Dragon Runestone x{requiredQty}");
                    Farm.DragonRunestone(requiredQty);
                }
                if (req.Name?.Contains("Gold Voucher") == true)
                {
                    Core.Logger($"Farming Gold Voucher x{requiredQty}");
                    Farm.Voucher(req.Name, requiredQty);
                }
                if (reqInShop != null)
                {
                    Core.Logger(
                        $"Requirement \"{reqInShop.Name}\" [{reqInShop.ID}] is in shop.",
                        "ProcessItemWithDependencies"
                    );
                    requiredQty = Math.Min(requiredQty, reqInShop.MaxStack);

                    // If requirement has no dependencies, buy it directly
                    if (
                        reqInShop.Requirements.Count == 0
                        && ((reqInShop.Coins && reqInShop.Cost <= 0) || !reqInShop.Coins)
                    )
                    {
                        if (reqInShop.Upgrade && !Bot.Player.IsMember)
                        {
                            Core.Logger(
                                $"{reqInShop.Name} is MEMBERS ONLY, you are not.... we can't buy it >.>"
                            );
                            return;
                        }
                        else
                        {
                            {
                                BuyItem(
                                    map,
                                    shopID,
                                    reqInShop.ID,
                                    requiredQty,
                                    shopItemID: reqInShop.ShopItemID
                                );
                            }
                        }
                        Bot.Wait.ForPickup(reqInShop.ID);
                    }
                    else if (reqInShop.Requirements.Count > 0)
                    {
                        // Recurse for nested requirements
                        ProcessItemWithDependencies(
                            reqInShop,
                            requiredQty,
                            map,
                            shopID,
                            findIngredients,
                            depth + 1,
                            acquiredItems
                        );
                        Bot.Wait.ForPickup(req!.ID);
                    }
                }
                else
                {
                    // Item not in current shop - must be farmed/crafted externally
                    externalItem = req;
                    externalQuant = requiredQty;
                    Core.AddDrop(externalItem.ID);
                    Core.Logger(
                        $"{externalItem.Name} [{externalItem.ID}] not in shop, using merge script."
                    );
                    findIngredients();
                    Bot.Wait.ForPickup(req.ID);
                }

                // Verify we got it
                if (!Core.CheckInventory(req!.ID, requiredQty))
                {
                    Core.Logger($"Warning: Failed to acquire {req.Name} x{requiredQty}.");
                }
            }

            EnsureShopLoaded(map, shopID);
            if (shopItem.Requirements.All(x => Core.CheckInventory(x.ID, x.Quantity)))
            {
                BuyItem(map, shopID, shopItem.ID, quantity, shopItemID: shopItem.ShopItemID);
            }
        }

        void HandleNoItemsFound(int mode, bool memSkipped)
        {
            if (buyOnlyThis != null)
                return;

            switch (mode)
            {
                case 0:
                case 2:
                    Core.Logger("The bot fetched 0 items to farm. Something must have gone wrong.");
                    break;
                case 1:
                    if (shopItems.All(x => !x.Coins))
                        Core.Logger(
                            "The bot fetched 0 items to farm. This is because none of the items in this shop are AC tagged."
                        );
                    else
                        Core.Logger(
                            "The bot fetched 0 items to farm. Something must have gone wrong."
                        );
                    break;
                case 3:
                    if (memSkipped)
                        Core.Logger(
                            "The bot fetched 0 items to farm. This is because you aren't a member."
                        );
                    else
                        Core.Logger(
                            "The bot fetched 0 items to farm. Something must have gone wrong."
                        );
                    break;
            }
        }

        #endregion
    }

    // If an item is in the shop, and it loops without going to the findingredients, add it here. with a comment above it saying what merge its for.
    public List<string> MergeItemisinShopExceptions = new()
    {
        /* No need to add to this. Do so in the merge script itself above the `Adv.StartBuyAllMerge` line
            Example:

            Adv.MergeItemisinShopExceptions.AddRange(
                        new[]
                        {
                            "Commmon Mogugu",
                            "Super Rare Mogugu",
                            "Super Super Rare Mogugu",
                            "Super Super Super Rare Mogugu",
                        }
                    );
    */
    };

    public List<ItemCategory> miscCatagories =
    [
        ItemCategory.Note,
        ItemCategory.Item,
        ItemCategory.QuestItem,
        ItemCategory.ServerUse,
    ];
    public ItemBase externalItem = new();
    public int externalQuant = 0;
    public bool matsOnly = false;
    public List<string> MaxStackOneItems = [];
    public List<string> AltFarmItems = [];

    /// <summary>
    /// The list of ScriptOptions for any merge script.
    /// </summary>
    public List<IOption> MergeOptions =
    [
        CoreBots.Instance.SkipOptions,
        new Option<mergeOptionsEnum>(
            "mode",
            "Select the mode to use",
            "Regardless of the mode you pick, the bot wont (attempt to) buy Legend-only items if you're not a Legend.\n"
                + "Select the Mode Explanation item to get more information",
            mergeOptionsEnum.all
        ),
        new Option<string>(
            " ",
            "Mode Explanation [all]",
            "Mode [all]: \t\tYou get all the items from shop, even if non-AC ones if any.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [acOnly]",
            "Mode [acOnly]: \tYou get all the AC tagged items from the shop.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [mergeMats]",
            "Mode [mergeMats]: \tYou dont buy any items but instead get the materials to buy them yourself, this way you can choose.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [select]",
            "Mode [select]: \tYou are able to select what items you get and which ones you dont in the Select Category below.",
            "click here"
        ),
    ];

    /// <summary>
    /// The name of ScriptOptions for any merge script.
    /// </summary>
    public string OptionsStorage = "MergeOptionStorage";
    #endregion

    #region Kill
#nullable enable

    /// <summary>
    /// Joins a map, jumps to a specified cell and pad, sets the spawn point, and kills the specified monster using the best available race gear.
    /// </summary>
    /// <param name="map">The map to join.</param>
    /// <param name="cell">The cell to jump to.</param>
    /// <param name="pad">The pad to jump to.</param>
    /// <param name="monster">The name of the monster to kill.</param>
    /// <param name="item">The item to kill the monster for. If null or empty, will just kill the monster once.</param>
    /// <param name="quant">The desired quantity of the item to collect.</param>
    /// <param name="isTemp">Whether the item is temporary.</param>
    /// <param name="log">Whether to log the killing of the monster.</param>
    /// <param name="publicRoom">Whether the action should take place in a public room.</param>
    public void BoostKillMonster(
        string map,
        string cell,
        string pad,
        string monster,
        string item = "",
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = false
    )
    {
        if (item != "" && Core.CheckInventory(item, quant))
            return;

        Core.Join(map, cell, pad, publicRoom: publicRoom);

        // _RaceGear(monster);
        Core.KillMonster(map, cell, pad, monster, item, quant, isTemp, log, publicRoom);

        GearStore(true);
    }

    /// <summary>
    /// Kills a monster using it's ID, with the specified monsters the best available race gear
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="monsterID">ID of the monster</param>
    /// <param name="item">Item to kill the monster for, if null will just kill the monster 1 time</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log">Whether it will log that it is killing the monster</param>
    /// <param name="publicRoom"></param>
    public void BoostKillMonster(
        string map,
        string cell,
        string pad,
        int monsterID,
        string item = "",
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = false
    )
    {
        if (item != "" && Core.CheckInventory(item, quant))
            return;

        Core.Join(map, cell, pad, publicRoom: publicRoom);

        // _RaceGear(monsterID);

        Core.KillMonster(map, cell, pad, monsterID, item, quant, isTemp, log, publicRoom);

        GearStore(true);
    }

    /// <summary>
    /// Joins a map, hunts for the monster, and kills the specified monster using the best available race gear.
    /// </summary>
    /// <param name="map">The map to join.</param>
    /// <param name="monster">The name of the monster to hunt and kill.</param>
    /// <param name="item">The item to hunt the monster for. If null, it will just hunt and kill the monster once.</param>
    /// <param name="quant">The desired quantity of the item to collect.</param>
    /// <param name="isTemp">Whether the item is temporary.</param>
    /// <param name="log">Whether to log the hunting and killing of the monster.</param>
    /// <param name="publicRoom">Whether the action should take place in a public room.</param>
    public void BoostHuntMonster(
        string map,
        string monster,
        string? item = null,
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = false
    )
    {
        if (item != null && Core.CheckInventory(item, quant))
            return;

        Core.Join(map, publicRoom: publicRoom);

        // _RaceGear(monster);

        Core.HuntMonster(map, monster, item, quant, isTemp, log, publicRoom);

        GearStore(true);
    }

    /// <summary>
    /// Joins a map, jumps to a specified cell and pad, sets the spawn point, and kills the specified monster using the best available race gear. Additionally, it listens for counter-attacks.
    /// </summary>
    /// <param name="map">The map to join.</param>
    /// <param name="cell">The cell to jump to.</param>
    /// <param name="pad">The pad to jump to.</param>
    /// <param name="monster">The name of the monster to kill.</param>
    /// <param name="item">The item to kill the monster for. If null, it will just kill the monster once.</param>
    /// <param name="quant">The desired quantity of the item to collect.</param>
    /// <param name="isTemp">Whether the item is temporary.</param>
    /// <param name="log">Whether to log the killing of the monster.</param>
    /// <param name="publicRoom">Whether the action should take place in a public room.</param>
    /// <param name="forAuto">Whether the method is used for an automated process.</param>
    public void KillUltra(
        string map,
        string cell,
        string pad,
        string monster,
        string? item = null,
        int quant = 1,
        bool isTemp = true,
        bool log = true,
        bool publicRoom = true,
        bool forAuto = false
    )
    {
        if (item != null && Core.CheckInventory(item, quant))
            return;
        if (item != null && !isTemp)
            Core.AddDrop(item);

        Core.Join(map, cell, pad, publicRoom: publicRoom);
        // if (!forAuto)
        //     _RaceGear(monster);
        Core.Jump(cell, pad);

        if (log)
            Core.Logger($"Killing Ultra-Boss {monster} for {item} ({quant}) [Temp = {isTemp}]");

        Bot.Hunt.ForItem(monster, item!, quant, isTemp);

        if (!forAuto)
            GearStore(true);
    }

    #region WIP/Proof of Concept Methods(W.I.P)
    /// <summary>
    /// Kills a monster while monitoring for a specific aura.
    /// </summary>
    public void KillWithAura(
        string map,
        string cell,
        string pad,
        string monster,
        string[] auraNames,
        Dictionary<string, Action>? auraReactions = null,
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = true,
        int ItemToUse = 0,
        int SafeItem = 0,
        CancellationToken cancellationToken = default
    )
    {
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : Core.CheckInventory(item, quant))
        )
            return;

        DateTime lastAuraTrigger = DateTime.MinValue;
        TimeSpan auraCooldown = TimeSpan.FromSeconds(0);
        monster = monster.Trim().FormatForCompare();

        Bot.Events.ExtensionPacketReceived += AuraListener;

        #region Setup Item Equip (optional)
        if (ItemToUse > 0)
        {
            int fallbackPotion = 1749;
            int equipSafe = SafeItem > 0 ? SafeItem : fallbackPotion;

            if (!Core.CheckInventory(equipSafe))
                BuyItem("embersea", 1100, fallbackPotion, 10, 1, 17966);

            EquipRetry(equipSafe);
            Core.Equip(ItemToUse);
        }
        #endregion

        if (item == null)
        {
            if (log)
                Core.Logger($"Killing {monster}");
            Bot.Kill.Monster(monster);
        }
        else
        {
            if (!isTemp)
                Core.AddDrop(item);
            if (log)
                Core.FarmingLogger(item, quant);

            while (
                !Bot.ShouldExit
                && !Core.CheckInventory(item, quant)
                && !cancellationToken.IsCancellationRequested
            )
            {
                while (
                    !Bot.ShouldExit
                    && !Bot.Player.Alive
                    && !cancellationToken.IsCancellationRequested
                ) { }

                if (Bot.Map.Name != map)
                    Core.Join(map, cell, pad);
                if (Bot.Player.Cell != cell)
                    Core.Jump(cell, pad);

                Bot.Combat.Attack(monster);
                Bot.Sleep(500);

                if (
                    isTemp
                        ? Bot.TempInv.Contains(item, quant)
                        : (Bot.Inventory.Contains(item, quant) || Bot.Bank.Contains(item, quant))
                )
                    break;
            }
        }

        Bot.Events.ExtensionPacketReceived -= AuraListener;

        void AuraListener(dynamic packet)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if ((string?)packet["params"]?.type != "json")
                return;

            dynamic? data = packet["params"]?.dataObj;
            if (data?.cmd?.ToString() != "ct" || data?.a is null)
                return;

            if (data == null)
                return;

            foreach (dynamic a in data.a)
            {
                string? auraName = a?.aura?["nam"]?.ToString();
                if (string.IsNullOrEmpty(auraName) || !auraNames.Contains(auraName))
                    continue;

                // Throttle cooldown
                if (DateTime.Now - lastAuraTrigger < auraCooldown)
                    continue;

                lastAuraTrigger = DateTime.Now;

                if (
                    auraReactions != null
                    && auraReactions.TryGetValue(auraName, out Action? reaction)
                )
                {
                    Bot.Log($"Invoking reaction for aura: {auraName}");
                    try
                    {
                        reaction?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Core.Logger($"Exception during aura reaction '{auraName}': {ex}");
                    }
                }
                else
                {
                    // fallback switch logic if no reaction found
                    switch (auraName)
                    {
                        case "Shapeshifted":
                            Bot.Log($"Detected aura (switch fallback): {auraName}");
                            break;

                        default:
                            Core.Logger($"Unhandled aura (switch fallback): {auraName}");
                            break;
                    }
                }

                break; // react to only one aura per packet
            }
        }

        void EquipRetry(int id)
        {
            Core.Equip(id);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(id), 20);
            Bot.Sleep(2000);
            Core.Equip(id); // Flash refresh workaround
            Bot.Sleep(2000);
        }
    }

    /* Example:
     Adv.KillWithAura(
            map: "kitsune",
            cell: "Boss",
            pad: "Left",
            monster: "kitsune",
            auraNames: new[] { "Shapeshifted" },
            auraReactions: new Dictionary<string, Action>
            {
                ["Shapeshifted"] = () => Core.Logger("Aura: Shapeshifted", "Example"),
            },
            item: "Fox Tail",
            quant: 3,
            log: true,
            ItemToUse: 0,
            SafeItem: 0,
            cancellationToken: CancellationToken.None
        );
        */

    #endregion WIP/Proof of Concept Methods(W.I.P)

    #endregion

    #region Gear

    // /// <summary>
    // /// Ranks up your class
    // /// </summary>
    // /// <param name="className"></param>
    // /// <param name="gearRestore"></param>
    // /// <param name="itemid"></param>
    // public void RankUpClass(string className, bool gearRestore = true, int itemid = 0)
    // {
    //     if (className == "(Current)")
    //     {
    //         if (Bot.Player.CurrentClass == null || string.IsNullOrEmpty(Bot.Player.CurrentClass.Name))
    //         {
    //             Core.Logger("CurrentClass is null or has no name. Cannot rank up.");
    //             return;
    //         }
    //         className = Bot.Player.CurrentClass.Name;
    //     }

    //     // Determine search condition based on itemid
    //     bool classMatch(InventoryItem i) =>
    //         (
    //             itemid > 0
    //                 ? i.ID == itemid
    //                 : i.Name.Equals(className, StringComparison.OrdinalIgnoreCase)
    //         )
    //         && i.Category == ItemCategory.Class;

    //     // Find the class item in inventory and bank
    //     ItemBase? itemInv = Bot
    //         .Inventory.Items.Concat(Bot.Bank.Items)
    //         .FirstOrDefault(c => c != null && classMatch(c));

    //     if (itemInv == null)
    //     {
    //         Core.Logger(
    //             $"Can't level up {(itemid > 0 ? $"item ID {itemid}" : $"\"{className}\"")} because you don't own it."
    //         );
    //         return;
    //     }

    //     // Unbank the item if it's in the bank but not in the inventory
    //     if (Bot.Bank.Contains(itemInv.ID) && !Bot.Inventory.Contains(itemInv.ID))
    //     {
    //         Core.Unbank(itemInv.ID);
    //         Core.Sleep();

    //         // Recheck the item in inventory after unbanking
    //         itemInv = Bot.Inventory.Items.FirstOrDefault(c => c != null && classMatch(c));
    //         if (itemInv == null)
    //         {
    //             Core.Logger(
    //                 $"Failed to unbank {(itemid > 0 ? $"item ID {itemid}" : $"\"{className}\"")}."
    //             );
    //             return;
    //         }
    //     }

    //     // Check if the class is already Rank 10 or unavailable due to membership requirement
    //     if (itemInv.Upgrade && !Bot.Player.IsMember)
    //     {
    //         Core.Logger($"\"{itemInv.Name}\" requires membership to rank up.");
    //         return;
    //     }

    //     // Check if the class is already Rank 10
    //     if (itemInv.Quantity >= 302500)
    //     {
    //         Core.Logger($"\"{itemInv.Name}\" is already Rank 10");
    //         return;
    //     }

    //     // Check if the class cannot be leveled past Rank 1
    //     if (
    //         itemInv.Name.Equals("Hobo Highlord")
    //         || itemInv.Name.Equals("No Class")
    //         || itemInv.Name.Equals("Obsidian No Class")
    //     )
    //     {
    //         Core.Logger($"\"{itemInv.Name}\" cannot be leveled past Rank 1");
    //         return;
    //     }

    //     // Optionally restore gear
    //     if (gearRestore)
    //         GearStore();

    //     Core.JumpWait();

    //     // Attempt to enhance the class if applicable
    //     SmartEnhance(className);

    //     // Find the class item in inventory after enhancement
    //     InventoryItem? classItem = Bot.Inventory.Items.FirstOrDefault(c => c != null && classMatch(c));
    //     if (classItem == null)
    //     {
    //         Core.Logger(
    //             $"Class item {(itemid > 0 ? $"ID {itemid}" : $"\"{className}\"")} not found in inventory."
    //         );
    //         return;
    //     }

    //     // Ensure the class item is enhanced before leveling up
    //     if (classItem.EnhancementLevel <= 0)
    //     {
    //         Core.Logger(
    //             $"Can't level up \"{classItem.Name}\" because it's not enhanced, and AutoEnhance is turned off"
    //         );
    //         return;
    //     }

    //     // Equip the class if it's not already equipped
    //     if (!Bot.Inventory.IsEquipped(classItem.ID))
    //     {
    //         Core.Equip(classItem.ID);
    //         Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(classItem.ID), 20);
    //     }

    //     // Activate the class boost
    //     Farm.ToggleBoost(BoostType.Class);
    //     Farm.IcestormArena(Bot.Player.Level, true);
    //     Core.Jump("Enter");
    //     Bot.Options.AggroMonsters = false;

    //     // Recheck the class item after jumping
    //     classItem = Bot.Inventory.Items.FirstOrDefault(c => c != null && classMatch(c));
    //     if (classItem == null)
    //     {
    //         Core.Logger(
    //             $"Class item {(itemid > 0 ? $"ID {itemid}" : $"\"{className}\"")} not found in inventory."
    //         );
    //         return;
    //     }

    //     // Check if the class reached Rank 10
    //     if (classItem.Quantity >= 302500)
    //         Core.Logger($"\"{classItem.Name}\" is now Rank 10");
    //     else
    //         Core.Logger($"\"{classItem.Name}\" is somehow... not rank 10??");

    //     // Deactivate the class boost
    //     Farm.ToggleBoost(BoostType.Class, false);

    //     // Optionally restore gear
    //     if (gearRestore)
    //         GearStore(true);
    // }

    // New public API
    public void RankUpClass(string className, bool gearRestore = true)
        => RankUpClassInternal(className, null, gearRestore);

    public void RankUpClass(int itemId, bool gearRestore = true)
        => RankUpClassInternal(null, itemId, gearRestore);

    // Exact old signature, with default for gearRestore
    public void RankUpClass(string className, int itemid, bool gearRestore = true)
    {
        if (itemid > 0)
            RankUpClass(itemid, gearRestore);   // call int overload
        else
            RankUpClass(className, gearRestore); // call string overload
    }



    private void RankUpClassInternal(string? className, int? itemId, bool gearRestore)
    {
        if (className == "(Current)")
        {
            string? currentName = Bot.Player.CurrentClass?.Name;
            if (string.IsNullOrEmpty(currentName))
            {
                Core.Logger("CurrentClass is null or has no name. Cannot rank up.");
                return;
            }
            className = currentName;
        }

        bool classMatch(InventoryItem i) =>
            i.Category == ItemCategory.Class
            && (itemId.HasValue
                ? i.ID == itemId.Value
                : i.Name.Equals(className!, StringComparison.OrdinalIgnoreCase));

        ItemBase? itemInv = Bot.Inventory.Items
            .Concat(Bot.Bank.Items)
            .FirstOrDefault(i => i != null && classMatch(i));

        if (itemInv == null)
        {
            Core.Logger(
                itemId.HasValue
                    ? $"Can't level up item ID {itemId.Value} because you don't own it."
                    : $"Can't level up \"{className}\" because you don't own it."
            );
            return;
        }

        if (Bot.Bank.Contains(itemInv.ID) && !Bot.Inventory.Contains(itemInv.ID))
        {
            Core.Unbank(itemInv.ID);
            Core.Sleep();

            itemInv = Bot.Inventory.Items.FirstOrDefault(i => i != null && classMatch(i));
            if (itemInv == null)
            {
                Core.Logger("Failed to unbank class item.");
                return;
            }
        }

        if (itemInv.Upgrade && !Bot.Player.IsMember)
        {
            Core.Logger($"\"{itemInv.Name}\" requires membership to rank up.");
            return;
        }

        if (itemInv.Quantity >= 302500)
        {
            Core.Logger($"\"{itemInv.Name}\" is already Rank 10");
            return;
        }

        if (itemInv.Name is "Hobo Highlord" or "No Class" or "Obsidian No Class")
        {
            Core.Logger($"\"{itemInv.Name}\" cannot be leveled past Rank 1");
            return;
        }

        if (gearRestore)
            GearStore();

        Core.JumpWait();

        SmartEnhance(itemInv.Name);

        InventoryItem? classItem = Bot.Inventory.Items
            .FirstOrDefault(i => i != null && classMatch(i));

        if (classItem == null)
        {
            Core.Logger("Class item not found in inventory after enhance.");
            return;
        }

        if (classItem.EnhancementLevel <= 0)
        {
            Core.Logger($"Can't level up \"{classItem.Name}\" because it's not enhanced.");
            return;
        }

        if (!Bot.Inventory.IsEquipped(classItem.ID))
        {
            Core.Equip(classItem.ID);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(classItem.ID), 20);
        }

        Farm.ToggleBoost(BoostType.Class);
        Farm.IcestormArena(Bot.Player.Level, true);
        Core.Jump("Enter");
        Bot.Options.AggroMonsters = false;

        classItem = Bot.Inventory.Items.FirstOrDefault(i => i != null && classMatch(i));
        if (classItem == null)
        {
            Core.Logger("Class item missing after arena.");
            return;
        }

        Core.Logger(
            classItem.Quantity >= 302500
                ? $"\"{classItem.Name}\" is now Rank 10"
                : $"\"{classItem.Name}\" is somehow... not rank 10??"
        );

        Farm.ToggleBoost(BoostType.Class, false);

        if (gearRestore)
            GearStore(true);
    }


    /// <summary>
    /// Stores the gear a player has so that it can later restore these
    /// </summary>
    /// <param name="Restore">Set true to restore previously stored gear</param>
    /// <param name="EnhAfter">Reapply enhancements after restore</param>
    public void GearStore(bool Restore = false, bool EnhAfter = false)
    {
        if (!Restore)
        {
            ReEquippedItems.Clear();

            InventoryItem[] equippedItems =
                Bot.Inventory.Items.Where(i => i.Equipped).ToArray();

            foreach (InventoryItem item in equippedItems)
                ReEquippedItems.Add(item.Name);

            ReEnhanceAfter = CurrentClassEnh();
            ReWEnhanceAfter = CurrentWeaponSpecial();

            ReCEnhanceAfter = equippedItems.Any(i => i.Category == ItemCategory.Cape)
                ? CurrentCapeSpecial()
                : CapeSpecial.None;

            ReHEnhanceAfter = equippedItems.Any(i => i.Category == ItemCategory.Helm)
                ? CurrentHelmSpecial()
                : HelmSpecial.None;

            // ---- Store summary ----
            Core.Logger("GearStore: Saved current equipment state");
            Core.Logger($" - Items: {string.Join(", ", ReEquippedItems)}");
            Core.Logger($" - Class Enh: {ReEnhanceAfter}");

            if (ReCEnhanceAfter != CapeSpecial.None)
                Core.Logger($" - Cape Special: {ReCEnhanceAfter}");

            if (ReHEnhanceAfter != HelmSpecial.None)
                Core.Logger($" - Helm Special: {ReHEnhanceAfter}");

            if (ReWEnhanceAfter != WeaponSpecial.None)
                Core.Logger($" - Weapon Special: {ReWEnhanceAfter}");
        }
        else if (ReEquippedItems.Count > 0)
        {
            // ---- Restore summary ----
            Core.Logger("GearStore: Restoring saved equipment state");
            Core.Logger($" - Items: {string.Join(", ", ReEquippedItems)}");

            if (EnhAfter)
            {
                ReEnhanceAfter = CurrentClassEnh();
                ReCEnhanceAfter = CurrentCapeSpecial();
                ReHEnhanceAfter = CurrentHelmSpecial();
                ReWEnhanceAfter = CurrentWeaponSpecial();

                Core.Logger(
                    $" - Enhancements → Class: {ReEnhanceAfter}" +
                    $"{(ReCEnhanceAfter != CapeSpecial.None ? $", Cape: {ReCEnhanceAfter}" : "")}" +
                    $"{(ReHEnhanceAfter != HelmSpecial.None ? $", Helm: {ReHEnhanceAfter}" : "")}" +
                    $"{(ReWEnhanceAfter != WeaponSpecial.None ? $", Weapon: {ReWEnhanceAfter}" : "")}",
                    messageBox: false
                );
            }


            Core.JumpWait();
            Core.Equip(ReEquippedItems.ToArray());

            if (EnhAfter)
                EnhanceEquipped(
                    ReEnhanceAfter,
                    ReCEnhanceAfter,
                    ReHEnhanceAfter,
                    ReWEnhanceAfter
                );
        }
    }


    private readonly List<string> ReEquippedItems = [];
    private EnhancementType ReEnhanceAfter = EnhancementType.Lucky;
    private CapeSpecial ReCEnhanceAfter = CapeSpecial.None;
    private HelmSpecial ReHEnhanceAfter = HelmSpecial.None;
    private WeaponSpecial ReWEnhanceAfter = WeaponSpecial.None;

    /// <summary>
    /// Find out if an item is a weapon or not
    /// </summary>
    /// <param name="Item">The ItemBase object of the item</param>
    /// <returns>Returns if its a weapon or not</returns>
    public bool isWeapon(ItemBase Item) => Item.ItemGroup == "Weapon";

    /// <summary>
    /// Will do GearStore() and then figure out the race of the monster paramater and equip bestGear on it
    /// </summary>
    /// <param name="Monster">The Monster object of the monster</param>
    public void _RaceGear(string Monster)
    {
        // if (!Bot.Monsters.MapMonsters.Any(x => x.Name.ToLower() == Monster.ToLower()))
        // {
        //     Core.Logger("Could not find any monster with the name " + Monster);
        //     return;
        // }
        // GearStore();
        // string Map = Bot.Map.LastMap;
        // string MonsterRace = "";
        // if (Monster != "*")
        //     MonsterRace =
        //         Bot.Monsters.MapMonsters.First(x => x.Name.ToLower() == Monster.ToLower())?.Race
        //         ?? "";
        // else
        // {
        //     if (Bot.Monsters.CurrentMonsters.Count == 0)
        //     {
        //         Core.Logger(
        //             $"No monsters are present in cell \"{Bot.Player.Cell}\" in /{Bot.Map.Name}"
        //         );
        //         return;
        //     }
        //     MonsterRace = Bot.Monsters.CurrentMonsters.First().Race ?? "";
        // }

        // if (MonsterRace == null || MonsterRace == "")
        //     return;

        // string[] _BestGear = BestGear((RacialGearBoost)Enum.Parse(typeof(RacialGearBoost), MonsterRace), false);
        // if (_BestGear.Length == 0)
        //     return;
        // EnhanceItem(_BestGear, CurrentClassEnh(), CurrentCapeSpecial(), CurrentHelmSpecial(), CurrentWeaponSpecial());
        // Core.Equip(_BestGear);
        Core.Logger("BestGear Disabled");

        //EnhanceEquipped(CurrentClassEnh(), CurrentCapeSpecial(), CurrentHelmSpecial(), CurrentWeaponSpecial());
        // Core.Join(Map);
    }

    /// <summary>
    /// Will do GearStore() and then figure out the race of the monster paramater and equip bestGear on it
    /// </summary>
    /// <param name="MonsterID">The MonsterID of the monster</param>
    public void _RaceGear(int MonsterID)
    {
        // GearStore();
        // string Map = Bot.Map.LastMap;
        // string MonsterRace = Bot.Monsters.MapMonsters.First(x => x.ID == MonsterID).Race;

        // if (MonsterRace == null || MonsterRace == "")
        //     return;

        // string[] _BestGear = BestGear((RacialGearBoost)Enum.Parse(typeof(RacialGearBoost), MonsterRace), false);
        // if (_BestGear.Length == 0)
        //     return;
        // EnhanceItem(_BestGear, CurrentClassEnh(), CurrentCapeSpecial(), CurrentHelmSpecial(), CurrentWeaponSpecial());
        // Core.Equip(_BestGear);

        Core.Logger("BestGear Disabled");
        //EnhanceEquipped(CurrentClassEnh(), CurrentCapeSpecial(), CurrentHelmSpecial(), CurrentWeaponSpecial());
        // Core.Join(Map);
    }

    public bool HasMinimalBoost(GenericGearBoost boostType, int percentage) =>
        Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .Any(x =>
                Core.GetBoostFloat(x, boostType.ToString()) >= ((percentage / (float)100) + 1)
            );

    public bool HasMinimalBoost(RacialGearBoost boostType, int percentage) =>
        Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .Any(x =>
                Core.GetBoostFloat(x, boostType.ToString()) >= ((percentage / (float)100) + 1)
            );

    #endregion

    #region Enhancement

    /// <summary>
    /// Enhances your currently equipped gear
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cSpecial"></param>
    /// <param name="hSpecial"></param>
    /// <param name="wSpecial"></param>
    public void EnhanceEquipped(
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None
    )
    {
        if (Core.CBOBool("DisableAutoEnhance", out bool _disableAutoEnhance) && _disableAutoEnhance)
            return;

        List<InventoryItem> EquippedItems = Bot.Inventory.Items.FindAll(i =>
            i.Equipped == true && EnhanceableCatagories.Contains(i.Category)
        );
        try
        {
            AutoEnhance(EquippedItems, type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            AdvCrash(e);
        }
    }

    /// <summary>
    /// Enhances a selected item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type"></param>
    /// <param name="cSpecial"></param>
    /// <param name="hSpecial"></param>
    /// <param name="wSpecial"></param>
    /// <param name="logging"></param>
    public void EnhanceItem(
        string item,
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None,
        bool logging = false
    )
    {
        if (
            string.IsNullOrEmpty(item)
            || (
                Core.CBOBool("DisableAutoEnhance", out bool _disableAutoEnhance)
                && _disableAutoEnhance
            )
        )
            return;

        if (!Core.CheckInventory(item))
        {
            Core.Logger($"Enhancement Failed: Could not find \"{item}\"");
            return;
        }

        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            if (Bot.Player.HasTarget)
                Bot.Combat.CancelTarget();
            Core.JumpWait();
            Core.Sleep();
        }

        InventoryItem? SelectedItem = Bot.Inventory.Items.Find(i =>
            i.Name.ToLower().Trim() == item.ToLower().Trim()
            && EnhanceableCatagories.Contains(i.Category)
        );
        ;
        if (SelectedItem == null)
        {
            if (Bot.Inventory.Items.Any(i => i.Name.ToLower().Trim() == item.ToLower().Trim()))
                Core.Logger($"Enhancement Failed: {item} cannot be enhanced");
            return;
        }

        try
        {
            AutoEnhance([SelectedItem], type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            AdvCrash(e);
        }
    }

    /// <summary>
    /// Enhances multiple selected items
    /// </summary>
    /// <param name="items"></param>
    /// <param name="type"></param>
    /// <param name="cSpecial"></param>
    /// <param name="hSpecial"></param>
    /// <param name="wSpecial"></param>
    public void EnhanceItem(
        string[] items,
        EnhancementType type,
        CapeSpecial cSpecial = CapeSpecial.None,
        HelmSpecial hSpecial = HelmSpecial.None,
        WeaponSpecial wSpecial = WeaponSpecial.None
    )
    {
        if (
            items.Length == 0
            || (
                Core.CBOBool("DisableAutoEnhance", out bool _disableAutoEnhance)
                && _disableAutoEnhance
            )
        )
            return;

        // If any of the items in the items array cant be found, return
        List<string>? notFound = [];
        foreach (string item in items)
            if (!Core.CheckInventory(item))
                notFound.Add(item);

        if (notFound.Count > 0)
        {
            if (notFound.Count == 1)
                Core.Logger($"Enhancement Failed: Could not find {notFound.First()}");
            else
                Core.Logger(
                    $"Enhancement Failed: Could not find the following items: {string.Join(", ", notFound)}"
                );
            return;
        }
        notFound = null;

        // Find all the items in the items array
        List<InventoryItem> SelectedItems = Bot.Inventory.Items.FindAll(i =>
            items.Contains(i.Name) && EnhanceableCatagories.Contains(i.Category)
        );

        // If any of the items in the items array cant be enhanced, return
        if (SelectedItems.Count != items.Length)
        {
            List<string> unEnhanceable = [];

            foreach (string item in items)
                if (
                    !Bot.Inventory.Items.Any(i =>
                        i.Name == item && EnhanceableCatagories.Contains(i.Category)
                    )
                )
                    unEnhanceable.Add(item);

            if (unEnhanceable.Count == 1)
                Core.Logger(
                    $"Enhancement Failed: Unenhanceable item found, {unEnhanceable.First()}"
                );
            else
                Core.Logger(
                    $"Enhancement Failed: The following items are unenhanceable, {string.Join(", ", unEnhanceable)}"
                );

            return;
        }

        try
        {
            AutoEnhance(SelectedItems, type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            AdvCrash(e);
        }
    }

    private static bool IsEnhancedWithBaseForge(InventoryItem item) =>
        item.EnhancementPatternID == 0 && item.EnhancementLevel > 0;

    // private void AdvCrash(Exception e, [CallerMemberName] string? caller = null)
    // {
    //     if (e == null || (Bot.ShouldExit && e is OperationCanceledException))
    //         return;
    //     List<string> logs = Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script);
    //     logs = logs.Skip(logs.Count > 5 ? (logs.Count - 5) : logs.Count).ToList();
    //     Bot.Handlers.RegisterOnce(1, Bot => Bot.ShowMessageBox($"{caller} has crashed. Please fill in the Skua Bug Report/Request for under the topic: Crashed\n" +
    //             $"Due to special handling for this type of crash, your script will continue without using {caller} in this instance.\n\n" +
    //             "---------------------------------------------------" +
    //             "Last 5 logs:\n\t" +
    //             logs.Join("\n\t") +
    //             "\n\n" +
    //             "---------------------------------------------------" +
    //             "Crash Log:\n\t" +
    //             e.Message + "\n" + e.InnerException,
    //         caller + " crashed"));
    // }

    private void AdvCrash(Exception e, [CallerMemberName] string? caller = null)
    {
        if (e == null || (Bot.ShouldExit && e is OperationCanceledException))
            return;

        // Determine severity
        string GetSeverity(Exception ex)
        {
            return ex is NullReferenceException or InvalidOperationException ? "❗ Major"
                : ex is ArgumentException or FormatException ? "⚠️ Minor"
                : "🔥 Critical";
        }

        string severity = GetSeverity(e);

        // Grab last 5 logs, truncate lines
        List<string> logs = Ioc
            .Default.GetRequiredService<ILogService>()
            .GetLogs(LogType.Script)
            .Skip(
                Math.Max(
                    0,
                    Ioc.Default.GetRequiredService<ILogService>().GetLogs(LogType.Script).Count - 5
                )
            )
            .Select(
                (l, i) => $"{i + 1}. {(l.Length > 80 ? string.Concat(l.AsSpan(0, 77), "…") : l)}"
            )
            .ToList();

        // Helper: compact exception info
        string GetExceptionDetails(
            Exception ex,
            int maxFrames = 5,
            int maxInnerLines = 5,
            int maxFrameLength = 80
        )
        {
            StackTrace st = new(ex, true);
            StackFrame? frame = st.GetFrames()?.FirstOrDefault(f => f.GetFileLineNumber() > 0);

            string location =
                frame != null
                    ? $"{frame.GetFileName()?.Split('\\').LastOrDefault()} @ line {frame.GetFileLineNumber()} in {frame.GetMethod()?.Name}"
                    : "No line info. Top stack frames:\n"
                        + string.Join(
                            "\n",
                            st.GetFrames()
                                ?.Take(maxFrames)
                                .Select(f =>
                                    f.ToString().Length > maxFrameLength
                                        ? string.Concat(f.ToString().AsSpan(0, maxFrameLength), "…")
                                        : f.ToString()
                                ) ?? []
                        )
                        + (st.FrameCount > maxFrames ? "\n…" : "");

            string inner = "";
            if (ex.InnerException != null)
            {
                string[] innerLines =
                    ex.InnerException.StackTrace?.Split('\n') ?? [];
                inner =
                    $"\n🔹 **Inner Exception** 🔹\nMessage: {ex.InnerException.Message}\n"
                    + string.Join(
                        "\n",
                        innerLines
                            .Take(maxInnerLines)
                            .Select(l =>
                                l.Length > maxFrameLength
                                    ? string.Concat(l.AsSpan(0, maxFrameLength), "…")
                                    : l
                            )
                    )
                    + (innerLines.Length > maxInnerLines ? "\n…" : "");
            }

            return $"🔥 **Outer Exception** 🔥\nSeverity: {severity}\nMessage: {ex.Message}\n📍 Location: {location}{inner}";
        }

        string crashDetails = GetExceptionDetails(e);

        // Build one-line summary
        string oneLineSummary =
            $"📌 {caller} Crash | {severity} | Location: {crashDetails.Split('\n')[3]}";

        // Build ultimate MessageBox
        string message =
            "══════════════════════════════════════════\n"
            + $"🛑 **{caller} Crash Report** 🛑\n"
            + "══════════════════════════════════════════\n\n"
            + $"{oneLineSummary}\n\n"
            + $"⚠️ Script will continue without `{caller}`.\n"
            + $"📸 Take a screenshot and post it to Discord.\n\n"
            + "────────────── 📜 Last 5 Logs ──────────────\n"
            + string.Join("\n", logs)
            + "\n"
            + "────────────── 💻 Crash Details ────────────\n"
            + crashDetails
            + "\n"
            + "══════════════════════════════════════════";

        Bot.Handlers.RegisterOnce(1, Bot => Bot.ShowMessageBox(message, $"{caller} crashed"));
    }


    /// <summary>
    /// Determines what Enhancement Type the player has on their currently equipped class
    /// </summary>
    /// <returns>Returns the equipped Enhancement Type</returns>
    public EnhancementType CurrentClassEnh()
    {
        int patternId = Bot.Player.CurrentClass?.EnhancementPatternID ?? 9;

        if (patternId == 1 || patternId == 23)
            patternId = 9;

        return Enum.IsDefined(typeof(EnhancementType), patternId)
            ? (EnhancementType)patternId
            : EnhancementType.Lucky;
    }

    /// <summary>
    /// Determines what Cape Special the player has on their currently equipped cape
    /// </summary>
    /// <returns>Returns the equipped Cape Special</returns>
    public CapeSpecial CurrentCapeSpecial()
    {
        InventoryItem? EquippedCape = Bot.Inventory.Items.Find(i =>
            i.Equipped && i.Category == ItemCategory.Cape
        );
        if (EquippedCape == null)
            return CapeSpecial.None;
        int patternId = EquippedCape.EnhancementPatternID;
        if (Enum.IsDefined(typeof(EnhancementType), patternId))
            return CapeSpecial.None;
        return (CapeSpecial)patternId;
    }

    /// <summary>
    /// Determines what Helm Special the player has on their currently equipped helm
    /// </summary>
    /// <returns>Returns the equipped Helm Special</returns>
    public HelmSpecial CurrentHelmSpecial()
    {
        InventoryItem? EquippedHelm = Bot.Inventory.Items.Find(i =>
            i.Equipped && i.Category == ItemCategory.Helm
        );
        if (EquippedHelm == null)
            return HelmSpecial.None;
        int patternId = EquippedHelm.EnhancementPatternID;

        if (Enum.IsDefined(typeof(EnhancementType), patternId))
            return HelmSpecial.None;
        return (HelmSpecial)patternId;
    }

    /// <summary>
    /// Determines what Weapon Special the player has on their currently equipped weapon
    /// </summary>
    /// <returns>Returns the equipped Weapon Special</returns>
    public WeaponSpecial CurrentWeaponSpecial()
    {
        InventoryItem? EquippedWeapon = Bot.Inventory.Items.Find(i =>
            i.Equipped && WeaponCatagories.Contains(i.Category)
        );
        if (EquippedWeapon == null)
            return WeaponSpecial.None;
        int patternId = EquippedWeapon.EnhancementPatternID;

        if (Enum.IsDefined(typeof(EnhancementType), patternId))
            return WeaponSpecial.None;
        return (WeaponSpecial)patternId;
    }

    private static readonly ItemCategory[] EnhanceableCatagories =
    {
        ItemCategory.Sword,
        ItemCategory.Axe,
        ItemCategory.Dagger,
        ItemCategory.Gun,
        ItemCategory.HandGun,
        ItemCategory.Rifle,
        ItemCategory.Bow,
        ItemCategory.Mace,
        ItemCategory.Gauntlet,
        ItemCategory.Polearm,
        ItemCategory.Staff,
        ItemCategory.Wand,
        ItemCategory.Whip,
        ItemCategory.Class,
        ItemCategory.Helm,
        ItemCategory.Cape,
    };

    public readonly ItemCategory[] WeaponCatagories = EnhanceableCatagories[..12];

    private bool AlreadyHasRequestedEnhancement(
        InventoryItem item,
        EnhancementType type,
        CapeSpecial cSpecial,
        HelmSpecial hSpecial,
        WeaponSpecial wSpecial,
        bool logging = false
    )
    {
        if (item == null || item.EnhancementLevel <= 0 || item.EnhancementLevel != Bot.Player.Level)
            return false;

        if (item.Category == ItemCategory.Cape)
        {
            if (cSpecial != CapeSpecial.None)
                return (int)cSpecial == item.EnhancementPatternID;
            return (int)type == item.EnhancementPatternID;
        }

        if (item.Category == ItemCategory.Helm)
        {
            if (hSpecial != HelmSpecial.None)
                return (int)hSpecial == item.EnhancementPatternID;
            return (int)type == item.EnhancementPatternID;
        }

        if (item.ItemGroup == "Weapon")
        {
            if (wSpecial == WeaponSpecial.None)
                return (int)type == item.EnhancementPatternID;
            return WeaponAlreadyHasRequestedEnhancement(item, type, wSpecial);
        }

        if (item.Category == ItemCategory.Class)
            return (int)type == item.EnhancementPatternID;

        return false;
    }

    private bool WeaponAlreadyHasRequestedEnhancement(
        InventoryItem item,
        EnhancementType type,
        WeaponSpecial wSpecial
    )
    {
        if (item == null || item.ItemGroup != "Weapon" || item.EnhancementLevel <= 0)
            return false;

        int currentPattern = item.EnhancementPatternID;
        int currentProc = getProcID(item);

        if (wSpecial == WeaponSpecial.None)
            return currentPattern == (int)type;

        if ((int)wSpecial > 0 && (int)wSpecial <= 6)
            return currentPattern == (int)type && currentProc == (int)wSpecial;

        if (wSpecial == WeaponSpecial.Forge)
            return currentProc == 0 && (currentPattern == (int)type || currentPattern == 10);

        return currentProc == (int)wSpecial;
    }

    private void AutoEnhance(
        List<InventoryItem> ItemList,
        EnhancementType type,
        CapeSpecial cSpecial,
        HelmSpecial hSpecial,
        WeaponSpecial wSpecial,
        bool logging = false
    )
    {
        // In case the 'CurrentEnhancement()' failed and returned 0
        if (type == 0)
            return;

        // Empty check
        if (ItemList.Count == 0)
        {
            Core.Logger("Enhancement Failed:\t\"ItemList\" is empty");
            return;
        }

        // Defining cape
        InventoryItem? cape = null;
        if (cSpecial != CapeSpecial.None && ItemList.Any(i => i.Category == ItemCategory.Cape))
        {
            cape = ItemList.Find(i => i.Category == ItemCategory.Cape);

            // Removing cape from the list because it needs to be enhanced seperately
            if (cape != null)
                ItemList.Remove(cape);
        }

        // Defining helm
        InventoryItem? helm = null;
        if (hSpecial != HelmSpecial.None && ItemList.Any(i => i.Category == ItemCategory.Helm))
        {
            helm = ItemList.Find(i => i.Category == ItemCategory.Helm);

            // Removing helm from the list because it needs to be enhanced seperately
            if (helm != null)
                ItemList.Remove(helm);
        }

        // Defining weapon
        InventoryItem? weapon = null;
        // If Awe-Enhancements aren't unlocked, enhance them with normal enhancements
        if (
            wSpecial != WeaponSpecial.None
            && ItemList.Any(i => i.ItemGroup == "Weapon")
            && (uAwe() || wSpecial == WeaponSpecial.Forge || (int)wSpecial > 6)
        )
        {
            weapon = ItemList.Find(i => i.ItemGroup == "Weapon");

            // Removing weapon from the list because it needs to be enhanced seperately
            if (weapon != null)
                ItemList.Remove(weapon);
        }

        int skipCounter = 0;

        // Setting the shop ID for the enhancement type
        if (ItemList.Count > 0)
        {
            int shopID = 0;

            switch (type)
            {
                case EnhancementType.Fighter:
                    shopID = Bot.Player.Level >= 50 ? 768 : 141;
                    break;
                case EnhancementType.Thief:
                    shopID = Bot.Player.Level >= 50 ? 767 : 142;
                    break;
                case EnhancementType.Hybrid:
                    shopID = Bot.Player.Level >= 50 ? 766 : 143;
                    break;
                case EnhancementType.Wizard:
                    shopID = Bot.Player.Level >= 50 ? 765 : 144;
                    break;
                case EnhancementType.Healer:
                    shopID = Bot.Player.Level >= 50 ? 762 : 145;
                    break;
                case EnhancementType.SpellBreaker:
                    shopID = Bot.Player.Level >= 50 ? 764 : 146;
                    break;
                case EnhancementType.Lucky:
                    shopID = Bot.Player.Level >= 50 ? 763 : 147;
                    break;
                default:
                    Core.Logger(
                        $"Enhancement Failed:\tInvalid EnhancementType given, received {(int)type} | {type}"
                    );
                    return;
            }

            // Enhancing the remaining items
            foreach (InventoryItem item in ItemList)
            {
                if (AlreadyHasRequestedEnhancement(item, type, cSpecial, hSpecial, wSpecial, logging))
                {
                    skipCounter++;
                    continue;
                }
                _AutoEnhance(item, shopID, Bot.Map?.Name, logging);
                Core.Sleep();
            }
        }

        // Enhancing the cape with the cape special
        if (cape != null)
        {
            bool canEnhance = true;

            switch (cSpecial)
            {
                case CapeSpecial.Forge:
                    if (!uForgeCape())
                    {
                        Core.Logger(
                            "Enhancement Failed:\tYou did not unlock the Forge (Cape) Enhancement yet"
                        );
                        canEnhance = false;
                    }
                    break;
                case CapeSpecial.Absolution:
                    if (!uAbsolution())
                        Fail();
                    break;
                case CapeSpecial.Avarice:
                    if (!uAvarice())
                        Fail();
                    break;
                case CapeSpecial.Vainglory:
                    if (!uVainglory())
                        Fail();
                    break;
                case CapeSpecial.Penitence:
                    if (!uPenitence())
                        Fail();
                    break;
                case CapeSpecial.Lament:
                    if (!uLament())
                        Fail();
                    break;
                default:
                    Core.Logger(
                        $"Enhancement Failed:\tInvalid \"CapeSpecial\" given, received {(int)cSpecial} | {cSpecial}"
                    );
                    return;

                    void Fail()
                    {
                        Core.Logger(
                            $"Enhancement Failed:\tYou did not unlock the {cSpecial} Enhancement yet"
                        );
                        canEnhance = false;
                    }
            }

            if (AlreadyHasRequestedEnhancement(cape, type, cSpecial, hSpecial, wSpecial, logging))
            {
                skipCounter++;
            }
            else if (canEnhance)
                _AutoEnhance(cape, 2143, ((int)cSpecial > 0) ? "forge" : null, logging);
            else
                skipCounter++;
        }

        // Enhancing the helm with the helm special
        if (helm != null)
        {
            bool canEnhance = true;

            switch (hSpecial)
            {
                case HelmSpecial.Vim:
                    if (!uVim())
                        Fail();
                    break;
                case HelmSpecial.Examen:
                    if (!uExamen())
                        Fail();
                    break;
                case HelmSpecial.Forge:
                    if (!uForgeHelm())
                        Fail();
                    break;
                case HelmSpecial.Anima:
                    if (!uAnima())
                        Fail();
                    break;
                case HelmSpecial.Pneuma:
                    if (!uPneuma())
                        Fail();
                    break;
                case HelmSpecial.Hearty:
                    if (!uHearty())
                        Fail();
                    break;
                default:
                    Core.Logger(
                        $"Enhancement Failed:\tInvalid \"HelmSpecial\" given, received {(int)hSpecial} | {hSpecial}"
                    );
                    return;

                    void Fail()
                    {
                        Core.Logger(
                            $"Enhancement Failed:\tYou did not unlock the {hSpecial} Enhancement yet"
                        );
                        canEnhance = false;
                    }
            }

            if (AlreadyHasRequestedEnhancement(helm, type, cSpecial, hSpecial, wSpecial, logging))
            {
                skipCounter++;
            }
            else if (canEnhance)
                _AutoEnhance(helm, 2164, ((int)hSpecial > 0) ? "forge" : null);
            else
                skipCounter++;
        }

        // Enhancing the weapon with the weapon special
        if (weapon != null)
        {
            int shopID = 0;
            bool canEnhance = true;

            if ((int)wSpecial > 0 && (int)wSpecial <= 6)
            {
                switch (type)
                {
                    case EnhancementType.Fighter:
                        shopID = 635;
                        break;
                    case EnhancementType.Thief:
                        shopID = 637;
                        break;
                    case EnhancementType.Hybrid:
                        shopID = 633;
                        break;
                    case EnhancementType.Wizard:
                    case EnhancementType.SpellBreaker:
                        shopID = 636;
                        break;
                    case EnhancementType.Healer:
                        shopID = 638;
                        break;
                    case EnhancementType.Lucky:
                        shopID = 639;
                        break;
                    default:
                        Core.Logger(
                            $"Enhancement Failed:\tInvalid \"EnhancementType\" given, received {(int)wSpecial} | {wSpecial}"
                        );
                        return;
                }
            }
            else
            {
                switch (wSpecial)
                {
                    case WeaponSpecial.Forge:
                        if (!uForgeWeapon())
                        {
                            Core.Logger(
                                "Enhancement Failed:\tYou did not unlock the Forge (Weapon) Enhancement yet"
                            );
                            canEnhance = false;
                        }
                        break;
                    case WeaponSpecial.Lacerate:
                        if (!uLacerate())
                            Fail();
                        break;
                    case WeaponSpecial.Smite:
                        if (!uSmite())
                            Fail();
                        break;
                    case WeaponSpecial.Valiance:
                        if (!uValiance())
                            Fail();
                        break;
                    case WeaponSpecial.Arcanas_Concerto:
                        if (!uArcanasConcerto())
                        {
                            Core.Logger(
                                "Enhancement Failed:\tYou did not unlock the Arcana's Concerto Enhancement yet"
                            );
                            canEnhance = false;
                        }
                        break;
                    case WeaponSpecial.Elysium:
                        if (!uElysium())
                            Fail();
                        break;
                    case WeaponSpecial.Acheron:
                        if (!uAcheron())
                            Fail();
                        break;
                    case WeaponSpecial.Praxis:
                        if (!uPraxis())
                            Fail();
                        break;
                    case WeaponSpecial.Dauntless:
                        if (!uDauntless())
                            Fail();
                        break;
                    case WeaponSpecial.Ravenous:
                        if (!uRavenous())
                            Fail();
                        break;

                    default:
                        Core.Logger(
                            $"Enhancement Failed:\tInvalid \"WeaponSpecial\" given, received {(int)wSpecial} | {wSpecial}"
                        );
                        return;

                        void Fail()
                        {
                            Core.Logger(
                                $"Enhancement Failed:\tYou did not unlock the {wSpecial} Enhancement yet"
                            );
                            canEnhance = false;
                        }
                }

                shopID = 2142;
            }

            if (AlreadyHasRequestedEnhancement(weapon, type, cSpecial, hSpecial, wSpecial, logging))
            {
                skipCounter++;
            }
            else if (canEnhance)
                _AutoEnhance(weapon, shopID, (wSpecial == WeaponSpecial.Forge || (int)wSpecial > 6) ? "forge" : null, logging);
            else
                skipCounter++;
        }

        if (skipCounter > 0)
            Core.Logger(
                $"Enhancement Skipped:\t{skipCounter} item{(skipCounter > 1 ? 's' : null)}"
            );

        void _AutoEnhance(InventoryItem item, int shopID, string? map = null, bool logging = false)
        {
            bool specialOnCape = item.Category == ItemCategory.Cape && cSpecial != CapeSpecial.None;
            bool specialOnHelm = item.Category == ItemCategory.Helm && hSpecial != HelmSpecial.None;
            bool specialOnWeapon = item.ItemGroup == "Weapon" && wSpecial.ToString() != "None";
            string mapName = map ?? Bot.Map?.Name ?? "whitemap";
            List<ShopItem> shopItems = Core.GetShopItems(mapName, shopID);

            // Shopdata complete check
            if (!shopItems.Any(x => x.Category == ItemCategory.Enhancement) || shopItems.Count == 0)
            {
                Core.Logger(
                    $"Enhancement Failed for {item.Name}[{item.ID}], (EnhancementLevel: {item.EnhancementLevel}, map: {mapName}, shopID: {shopID}):\n"
                        + $"Couldn't find enhancements in shop {shopID}"
                );
                return;
            }

            // Checking if the item is already optimally enhanced
            if (Bot.Player.Level == item.EnhancementLevel)
            {
                if (specialOnCape)
                {
                    if ((int)cSpecial == item.EnhancementPatternID)
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if (specialOnHelm)
                {
                    if ((int)hSpecial == item.EnhancementPatternID)
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if (specialOnWeapon)
                {
                    if (
                        ((int)wSpecial > 0 && (int)wSpecial <= 6 ? (int)type : 10) == item.EnhancementPatternID
                        && (int)wSpecial == getProcID(item)
                    )
                    {
                        skipCounter++;
                        return;
                    }
                }
                else if ((int)type == item.EnhancementPatternID)
                {
                    skipCounter++;
                    return;
                }
            }

            // Logging
            if (logging)
            {
                if (specialOnCape)
                    Core.Logger(
                        $"Searching Enhancement:\tForge/{cSpecial.ToString().Replace("_", " ")} - \"{item.Name}\""
                    );
                else if (specialOnWeapon)
                    Core.Logger(
                        $"Searching Enhancement:\t{((int)wSpecial > 0 && (int)wSpecial <= 6 ? type : "Forge")}/{wSpecial.ToString().Replace("_", " ")} - \"{item.Name}\""
                    );
                else
                    Core.Logger($"Searching Enhancement:\t{type} - \"{item.Name}\"");
            }

            List<ShopItem> availableEnh = [];

            // Filters
            foreach (ShopItem enh in shopItems)
            {
                // Remove enhancments that you dont have access to
                if ((!Bot.Player.IsMember && enh.Upgrade) || (enh.Level > Bot.Player.Level))
                {
                    continue;
                }

                string enhName = enh.Name.Replace(" ", "").Replace("\'", "").ToLower();

                // Cape if cSpecial
                if (
                    specialOnCape
                    && enhName.Contains(cSpecial.ToString().Replace("_", "").ToLower())
                )
                    availableEnh.Add(enh);
                // Weapon if wSpecial
                else if (
                    specialOnWeapon
                    && enhName.Contains(wSpecial.ToString().Replace("_", "").ToLower())
                )
                    availableEnh.Add(enh);
                //Helm if hSpecial
                else if (
                    specialOnHelm
                    && enhName.Contains(hSpecial.ToString().Replace("_", "").ToLower())
                )
                    availableEnh.Add(enh);
                // Class
                else if (item.Category == ItemCategory.Class && enhName.Contains("armor"))
                    availableEnh.Add(enh);
                // Helm
                else if (item.Category == ItemCategory.Helm && enhName.Contains("helm"))
                    availableEnh.Add(enh);
                // Cape if not cSpecial
                else if (item.Category == ItemCategory.Cape && enhName.Contains("cape"))
                    availableEnh.Add(enh);
                // Weapon2 if not wSpecial
                else if (item.ItemGroup == "Weapon" && enhName.Contains("weapon"))
                    availableEnh.Add(enh);
            }

            // Empty check
            ShopItem? bestEnhancement = null;
            if (availableEnh.Count == 0)
            {
                if (logging)
                    Core.Logger($"Enhancement Failed:\t\"availableEnh\" is empty");
                return;
            }
            else if (availableEnh.Count == 1)
                bestEnhancement = availableEnh.First();
            else
            {
                // Sorting by level (descending)
                List<ShopItem> sortedList = availableEnh
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Upgrade ? 1 : 0)
                    .ToList();
                bestEnhancement = sortedList[0];
            }

            // Null check
            if (bestEnhancement == null)
            {
                if (logging)
                    Core.Logger(
                        $"Enhancement Failed:\tCould not find the best enhancement for \"{item.Name}\""
                    );
                return;
            }

            // Compare with current enhancement
            if (
                bestEnhancement.ID == getEnhID(item)
                && item.EnhancementLevel > 0
                && bestEnhancement.Level == item.EnhancementLevel
            )
            {
                if (logging)
                    Core.Logger(
                        $"Enhancement Canceled:\tBest enhancement is already applied for \"{item.Name}\""
                    );
                return;
            }

            // Enhancing the item
            int roomId = Bot.Map?.RoomID ?? 1;

            Bot.Send.Packet(
                $"%xt%zm%enhanceItemShop%{roomId}%{item.ID}%{bestEnhancement.ID}%{shopID}%"
            );

            // Final logging
            if (specialOnCape)
            {
                if (logging)
                    Core.Logger(
                        $"Enhancement Applied:\tForge/{cSpecial.ToString().Replace("_", " ")} - \"{item.Name}\" (Lvl {bestEnhancement.Level})"
                    );
            }
            else if (specialOnWeapon)
            {
                if (logging)
                    Core.Logger(
                        $"Enhancement Applied:\t{((int)wSpecial > 0 && (int)wSpecial <= 6 ? type : "Forge")}/{wSpecial.ToString().Replace("_", " ")} - \"{item.Name}\" (Lvl {bestEnhancement.Level})"
                    );
            }
            else
            {
                if (logging)
                    Core.Logger(
                        $"Enhancement Applied:\t{type} - \"{item.Name}\" (Lvl {bestEnhancement.Level})"
                    );
            }
            Core.Sleep();
        }
    }

    private int getProcID(InventoryItem? item) =>
        item == null ? 0 : Core.GetItemProperty<int>(item, "ProcID");

    private int getEnhID(InventoryItem? item) =>
        item == null ? 0 : Core.GetItemProperty<int>(item, "iEnh");

    public bool uAwe() => Core.isCompletedBefore(2937);

    public bool uForgeWeapon() => Core.isCompletedBefore(8738);

    public bool uLacerate() => Core.isCompletedBefore(8739);

    public bool uSmite() => Core.isCompletedBefore(8740);

    public bool uValiance() => Core.isCompletedBefore(8741);

    public bool uArcanasConcerto() => Core.isCompletedBefore(8742);

    public bool uAbsolution() => Core.isCompletedBefore(8743);

    public bool uVainglory() => Core.isCompletedBefore(8744);

    public bool uAvarice() => Core.isCompletedBefore(8745);

    public bool uForgeCape() => Core.isCompletedBefore(8758);

    public bool uElysium() => Core.isCompletedBefore(8821);

    public bool uAcheron() => Core.isCompletedBefore(8820);

    public bool uPenitence() => Core.isCompletedBefore(8822);

    public bool uLament() => Core.isCompletedBefore(8823);

    public bool uVim() => Core.isCompletedBefore(8824);

    public bool uExamen() => Core.isCompletedBefore(8825);

    public bool uForgeHelm() => Core.isCompletedBefore(8828);

    public bool uPneuma() => Core.isCompletedBefore(8827);

    public bool uAnima() => Core.isCompletedBefore(8826);

    public bool uDauntless() => Core.isCompletedBefore(9172);

    public bool uPraxis() => Core.isCompletedBefore(9171);

    public bool uRavenous() => Core.isCompletedBefore(9560);

    public bool uHearty()
    {
        return Core.isCompletedBefore(9466) && Farm.FactionRank("Grimskull Trolling") >= 7;
    }

    #endregion

    #region SmartEnhance

    /// <summary>
    /// Automatically finds the best Enhancement for the given class and enhances all equipped gear with it too
    /// </summary>
    /// <param name="className">Name of the class you wish to enhance</param>
    /// <param name="ForceEnh">For classes that are recieved unenhanced</param>
    public void SmartEnhance(string? className, bool ForceEnh = false)
    {
        // Get CBO bool for DisableAutoEnhance and return if we're not forcing an enh for unenhed (lvl = 0) classes
        bool EnhDisabled = false;
        if (Core.CBOBool("DisableAutoEnhance", out bool _AutoEnhance))
            EnhDisabled = _AutoEnhance;

        if (string.IsNullOrEmpty(className))
        {
            Core.Logger($"{className} is null");
            return;
        }

        if (EnhDisabled && !ForceEnh)
        {
            Core.Logger("AutoEnh turned off in CBO, class/items will *not* be enhanced");
            return;
        }

        if (!Core.CheckInventory(className))
        {
            Core.Logger($"SmartEnhance Failed: Class {className} was not found in inventory");
            return;
        }

        if (Bot.Player.InCombat)
            Core.JumpWait();

        // Error correction
        className = className.ToLower().Trim();
        InventoryItem? SelectedClass = Bot.Inventory.Items.Find(i =>
            i.Name.ToLower().Trim() == className.ToLower().Trim()
            && i.Category == ItemCategory.Class
        );
        if (SelectedClass == null)
        {
            Core.Logger($"SmartEnhance Failed: Class {className} was not found in inventory");
            return;
        }

        // Creating variables that can be assigned to
        className = SelectedClass.Name.ToLower();
        EnhancementType? type = null;
        CapeSpecial cSpecial = CapeSpecial.None;
        HelmSpecial hSpecial = HelmSpecial.None;
        WeaponSpecial wSpecial = WeaponSpecial.None;

        // If the item doesnt exist in the forge enh lib, or the player doesn't have the Forge enh unlocked, use Awe enh instead
        if (!ForgeEnhancementLibrary())
            AweEnhancementLibrary();

        // Can't be too careful
        if (type == null)
        {
            Core.Logger($"SmartEnhance Failed: 'type' for {className} is NULL");
            return;
        }

        // If the class isn't enhanced yet, enhance it with the enhancement type
        if (SelectedClass.EnhancementLevel <= 0)
        {
            EnhanceItem(SelectedClass.Name ?? className, (EnhancementType)type);
            // Used only for unenhanced classes usualy gotten from quests (ex: blood/scarlet sorccerer)
            if (ForceEnh)
                return;
        }
        Core.Equip(SelectedClass.Name ?? className);
        Bot.Wait.ForTrue(() => Bot.Player.CurrentClass?.Name == className, 40);
        EnhanceEquipped((EnhancementType)type, cSpecial, hSpecial, wSpecial);

        bool ForgeEnhancementLibrary()
        {
            switch (className.ToLower())
            {
                #region Lucky Region

                #region Lucky - Vim - vainglory - lacerate
                case "horc evader":
                    if (!uLacerate() || !uVim() || !uVainglory())
                        goto default;

                    type = EnhancementType.Thief;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Lacerate;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion

                #region Luck - Awe_Blast | Arcanas_Concerto - ForgeHelm - Penitence
                case "lord of order":
                    if (!uAwe() || !uForgeHelm() || !uPenitence())
                        goto default;

                    type = EnhancementType.Lucky;
                    wSpecial = uArcanasConcerto()
                        ? WeaponSpecial.Arcanas_Concerto
                        : WeaponSpecial.Awe_Blast;
                    hSpecial = HelmSpecial.Forge;
                    cSpecial = CapeSpecial.Penitence;
                    break;
                #endregion

                #region Ravenous
                case "PlaceHolder":
                    if (!uRavenous())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Ravenous;
                    break;
                #endregion Ravenous


                #region Lucky - Dauntless - Vim - Lament
                case "great thief":
                    if (!uDauntless() || !uVim() || !uLament())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = WeaponSpecial.Dauntless;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion Lucky - Dauntless - Vim - Penitence

                #region Lucky - Lacerate - Vim - Lament
                case "timekeeper":
                case "timekiller":
                    if (!uLacerate() || !uVim() || !uLament())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = WeaponSpecial.Lacerate;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion Lucky - Lacerate - Vim - Lament

                #region Lucky - Forge - Spiral Carve
                case "corrupted chronomancer":
                case "underworld chronomancer":
                case "eternal chronomancer":
                case "immortal chronomancer":
                case "dark metal necro":
                    if (!uForgeCape())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Spiral_Carve;
                    break;
                #endregion

                #region Lucky - Forge - Awe Blast
                case "glacial berserker":
                    if (!Core.isCompletedBefore(8758))
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Awe_Blast;
                    break;
                #endregion

                #region Lucky - Forge - Mana Vamp
                case "legendary elemental warrior":
                case "mythic elemental warrior":
                case "ultra elemental warrior":
                    if (!uForgeCape())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Mana_Vamp;
                    break;
                #endregion

                #region Lucky - Forge - Smite
                case "Draconic Chronomancer":
                    if (!uSmite() || !uForgeCape())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Smite;
                    break;
                #endregion

                #region Lucky - Forge - Elysium
                case "ultra omniknight":
                case "dark ultra omninight":
                    if (!uElysium() || !uForgeCape())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Elysium;
                    break;
                #endregion

                #region Lucky - Vainglory - Valiance - Anima
                case "archfiend":
                case "eternal inversionist":
                case "dragonlord":
                    if (!uVainglory() || !uValiance() || !uAnima())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;

                    break;
                #endregion

                #region Lucky - Vainglory - Valiance - Vim
                case "continuum chronomancer":
                case "quantum chronomancer":
                    if (
                        !uPenitence()
                        || (!uDauntless() || !uValiance())
                        || !uRavenous()
                        || !uValiance()
                        || !uAnima()
                    )
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial =
                        uRavenous() ? WeaponSpecial.Ravenous
                        : uDauntless() ? WeaponSpecial.Dauntless
                        : WeaponSpecial.Valiance;

                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Vainglory - Dauntless - Anima
                case "chaos avenger":
                    if (!uAnima() || !uVainglory())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Lacerate - Forge - Lament

                case "doom metal necro":
                case "neo metal necro":
                    if (!uLacerate() || !uForgeHelm() || !uLament())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = uLament() ? CapeSpecial.Lament : CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Lacerate;
                    hSpecial = HelmSpecial.Forge;
                    break;
                #endregion Lucky - lacerate - forge

                #region Lucky - Vainglory - Dauntless|Valiance|Smite - Vim
                case "martial artist":
                case "master martial artist":
                    if ((!uDauntless() && !uValiance() && !uSmite()) || !uVainglory() || !uVim())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial =
                        uDauntless() ? WeaponSpecial.Dauntless
                        : uValiance() ? WeaponSpecial.Valiance
                        : WeaponSpecial.Smite; // else do smite, if no smite > do Awe
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion

                #region Lucky - Vainglory - Praxis - Vim
                case "yami no ronin":
                    if (!uPraxis() || !uVainglory() || !uVim())
                        goto default;
                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Praxis;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion

                #region Lucky - Vainglory - Valiance - Anima
                case "nechronomancer":
                case "necrotic chronomancer":
                    if (!uVainglory() || !uArcanasConcerto() || !uAnima())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Vainglory - Elysium - Vim
                case "shadowwalker of time":
                case "shadowstalker of time":
                case "shadowweaver of time":
                    if (!uVainglory() || !uElysium() || !uVim())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion

                #region Lucky - Vainglory - Valiance - None
                case "legion doomknight":
                    if (!uVainglory() || !uValiance())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = CurrentHelmSpecial();
                    break;
                #endregion

                #region Lucky - Vainglory - Elysium - Pneuma
                case "antique hunter":
                case "artifact hunter":
                    if (!uVainglory() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Lucky - Lament - Elysium - Pneuma
                case "abyssal angel":
                case "abyssal angel's shadow":
                    if (!uLament() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Lucky - Dauntless | Ravenous - Anima | ForgeHelm - Vainglory
                case "verus doomknight":
                    if (!uRavenous() || !uForgeHelm() || !uVainglory())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Ravenous;
                    hSpecial = uAnima() ? HelmSpecial.Anima : HelmSpecial.Forge;
                    break;
                #endregion

                #region Lucky - Vainglory - Dauntless/Valiance - Anima
                case "debris highlord":
                case "void highlord":
                case "void highlord (ioda)":
                    if (!uAnima() || !uValiance() || !uVainglory())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = !uDauntless()
                        ? (
                            uRavenous()
                                ? WeaponSpecial.Ravenous
                                : (uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Forge)
                        )
                        : WeaponSpecial.Dauntless;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion


                #region Lucky - Avarice - Dauntless - Anima
                case "flame dragon warrior":
                    if (!uAvarice() || !uDauntless() || !uAnima())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Avarice;
                    wSpecial = WeaponSpecial.Dauntless;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Avarice - Elysium - Anima
                case "chaos slayer":
                case "chaos slayer berserker":
                case "chaos slayer cleric":
                case "chaos slayer mystic":
                case "chaos slayer thief":
                    if (!uAvarice() || !uElysium() || !uAnima())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Avarice;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Penitence - Ravenous | Praxis | Lacerate - Forge | None
                case "archpaladin":
                    if (!uLacerate() || !uForgeHelm() || !uPenitence())
                        goto default;

                    type = EnhancementType.Lucky;
                    wSpecial = uValiance()
                        ? WeaponSpecial.Valiance
                        : (uPraxis() ? WeaponSpecial.Praxis : WeaponSpecial.Lacerate);
                    hSpecial = uForgeHelm() ? HelmSpecial.Forge : HelmSpecial.None;
                    cSpecial = CapeSpecial.Penitence;
                    break;
                #endregion

                #region Fighter - Ravenous | Valiance - Anima - Absolution
                case "stonecrusher":
                    if (!uValiance() || !uAnima() || !uAbsolution())
                        goto default;

                    type = EnhancementType.Fighter;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;
                    cSpecial = CapeSpecial.Absolution;
                    break;
                #endregion

                #endregion

                #region Wizard Region

                #region Wizard -  Valiance|Praxis - Pneuna - Vainglory|Lament
                case "lightcaster":
                    if (!uValiance() || !uPneuma() || !uVainglory())
                    {
                        if (!uLament() || !uPraxis())
                            goto default;
                    }
                    type = EnhancementType.Wizard;
                    cSpecial = !uVainglory() ? CapeSpecial.Lament : CapeSpecial.Vainglory;
                    wSpecial = !uValiance() ? WeaponSpecial.Praxis : WeaponSpecial.Valiance;
                    hSpecial = !uPneuma() ? CurrentHelmSpecial() : HelmSpecial.Pneuma;
                    break;
                #endregion

                case "archivist of time":
                    #region Wizard -  Valiance - Pneuna - Vainglory
                    if (!uValiance() || !uPneuma() || !uVainglory())
                    {
                        goto default;
                    }
                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Pneuma;
                    break;

                    #endregion

                #region Wizard - Forge - Awe Blast
                case "infinity knight":
                    if (!uForgeCape())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Forge;
                    wSpecial = WeaponSpecial.Awe_Blast;
                    hSpecial = CurrentHelmSpecial();
                    break;
                #endregion

                #region Wizard - Vainglory - Valiance - Pneuma
                case "archmage":
                case "darklord":
                case "arcana invoker":
                    if (!uVainglory() || !uValiance() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Penitence - Acheron - Pneuma
                case "master of moglins":
                case "dark master of moglins":
                    if (!uPenitence() || !uAcheron() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Penitence;
                    wSpecial = WeaponSpecial.Acheron;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Vainglory - Ravenous | Valiance - Pneuma | Wizard
                case "legion revenant":
                case "legion revenant (ioda)":
                    if (!uVainglory() || !uValiance() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Avarice - Elysium - Pneuma
                case "vampire lord":
                case "enchanted vampire lord":
                case "royal vampire lord":
                case "darkside":
                case "dark lord":
                    if (!uAvarice() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Avarice;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region  Wizard - Vainglory - Elysium - Pneuma
                case "shaman":
                    if (!uVainglory() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Avarice - Acheron - Pneuma
                case "blaze binder":
                    if (!uAvarice() || !uAcheron() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Avarice;
                    wSpecial = WeaponSpecial.Acheron;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Lament - Elysium - Pneuma
                case "royal battlemage":
                    if (!uLament() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Wizard - Lament - Valiance - Pneuma
                case "scarlet sorceress":
                    if (!uLament() || !uValiance() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion


                #region Wizard - Vainglory / Forge - Daunt / Ravenous / Forge - Pneuma / Forge
                case "sovereign of storms":
                    if (!uVainglory() || !uDauntless() || !uRavenous() || !uPneuma())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = uVainglory() ? CapeSpecial.Vainglory : CapeSpecial.Forge;
                    wSpecial = uDauntless()
                        ? WeaponSpecial.Dauntless
                        : (uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Forge);
                    hSpecial = uPneuma() ? HelmSpecial.Pneuma : HelmSpecial.Forge;
                    break;
                #endregion


                #region Wizard - Ravenous - Lament - Examen
                case "lich":
                    if (!(uRavenous() && uLament() && uExamen()))
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = WeaponSpecial.Ravenous;
                    hSpecial = HelmSpecial.Examen;
                    break;
                #endregion
                #endregion

                #region Healer Region

                #region Healer - Avarice - Elysium - Pneuma
                case "dragon of time":
                    if (!uAvarice() || !uElysium() || !uPneuma())
                        goto default;

                    type = EnhancementType.Healer;
                    cSpecial = CapeSpecial.Avarice;
                    wSpecial = WeaponSpecial.Elysium;
                    hSpecial = HelmSpecial.Pneuma;
                    break;

                #endregion

                #region  Healer - None - Valiance - Nine
                case "obsidian paladin chronomancer":
                case "paladin chronomancer":
                    if (!uValiance())
                        goto default;

                    type = EnhancementType.Healer;
                    cSpecial = CapeSpecial.None;
                    wSpecial = WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.None;
                    break;

                #endregion

                #region Fighter - Ravenous | Valiance - Anima - Absolution
                case "frostval barbarian":
                    if (!uAbsolution() || !uValiance() || !uAnima())
                        goto default;
                    type = EnhancementType.Fighter;
                    cSpecial = CapeSpecial.Absolution;
                    wSpecial = uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Penitence | Absolution - Elysium | Valiance - Vim
                case "arachnomancer":
                    if (!uAbsolution() || !uAbsolution() || !uVim())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = uPenitence() ? CapeSpecial.Penitence : CapeSpecial.Absolution;
                    wSpecial = uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Vim;
                    break;
                #endregion

                #region Healer - Valiance - Current - Current

                #endregion

                #region Wizard - Elysium/Ravenous - Examen - Lament
                case "phantom chronomancer":
                case "phantasm chronomancer":

                    if (uElysium() || uRavenous())
                        goto default;
                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Lament;
                    wSpecial = uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Ravenous;
                    hSpecial = HelmSpecial.Examen;
                    break;

                #endregion

                case "scion of flames":
                    if ((!uVainglory() || !uLament())
                    && (!uPneuma() || !uForgeHelm())
                    && (!uRavenous() || !uValiance()))
                        goto default;
                    type = EnhancementType.Wizard;
                    cSpecial = uVainglory() ? CapeSpecial.Vainglory : CapeSpecial.Lament;
                    wSpecial = uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance;
                    hSpecial = uPneuma() ? HelmSpecial.Pneuma : HelmSpecial.Forge;
                    break;

                #region Healer - Current - Valiance/Awe - Current
                case "healer":
                case "healer (rare)":
                    type = EnhancementType.Healer;
                    cSpecial = CurrentCapeSpecial();
                    wSpecial = uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Awe_Blast;
                    hSpecial = CurrentHelmSpecial();
                    break;
                #endregion

                #region Luck - Vim - Lam - Rav
                case "chrono shadowslayer":
                case "chrono shadowhunter":
                    type = EnhancementType.Lucky;
                    cSpecial = uLament()
                        ? CapeSpecial.Lament
                        : (uForgeCape() ? CapeSpecial.Forge : CurrentCapeSpecial());
                    wSpecial = uRavenous()
                        ? WeaponSpecial.Ravenous
                        : (
                            uArcanasConcerto()
                                ? WeaponSpecial.Arcanas_Concerto
                                : (uForgeWeapon() ? WeaponSpecial.Forge : WeaponSpecial.Awe_Blast)
                        );
                    hSpecial = uVim()
                        ? HelmSpecial.Vim
                        : (uForgeHelm() ? HelmSpecial.Forge : CurrentHelmSpecial());
                    break;
                #endregion

                #region Lucky - Vainglory - Valiance / Dauntless - Anima
                case "glacial warlord":
                case "glaceran warlord":
                case "dark glaceran warlord":
                case "savage glaceran warlord":
                    if (!uVainglory() || !uValiance() || !uAnima())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Lucky - Ravenous/Valiance - Examen - Lament/Vainglory
                case "king's echo":
                    if (!uValiance() || !uExamen() || !uVainglory())
                        goto default;

                    type = EnhancementType.Healer;
                    cSpecial = uLament() ? CapeSpecial.Lament : CapeSpecial.Vainglory;
                    wSpecial = uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp;
                    hSpecial = HelmSpecial.Examen;
                    break;
                #endregion

                #region Luck - Val/Smite/Mana - Anima - Vg
                case "dragonslayer general":
                    type = EnhancementType.Lucky;

                    cSpecial =
                        uVainglory() ? CapeSpecial.Vainglory
                        : uForgeCape() ? CapeSpecial.Forge
                        : CurrentCapeSpecial();

                    wSpecial =
                        uValiance() ? WeaponSpecial.Valiance
                        : uSmite() ? WeaponSpecial.Smite
                        : WeaponSpecial.Mana_Vamp;

                    hSpecial =
                        uAnima() ? HelmSpecial.Anima
                        : uForgeHelm() ? HelmSpecial.Forge
                        : CurrentHelmSpecial();

                    break;
                #endregion

                #region Luck - Dauntless | Ravenous - Anima - Vainglory
                case "chrono chaorruptor":
                    if (!uRavenous() || !uAnima() || !uVainglory())
                        goto default;

                    type = EnhancementType.Lucky;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Ravenous;
                    hSpecial = HelmSpecial.Anima;
                    break;
                #endregion

                #region Wizard - Ravenous - Pneuma - Vainglory
                case "chrono dataknight":
                case "chrono dragonknight":
                    if (!uRavenous() || !uPneuma() || !uVainglory())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Vainglory;
                    wSpecial = WeaponSpecial.Ravenous;
                    hSpecial = HelmSpecial.Pneuma;
                    break;
                #endregion

                #region Luck - Ravenous | Valiance - ForgeHelm | Luck - Absolution
                case "legendary hero":
                    if (!uValiance() || !uForgeHelm() || !uAbsolution())
                        goto default;

                    type = EnhancementType.Wizard;
                    cSpecial = CapeSpecial.Absolution;
                    wSpecial = uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance;
                    hSpecial = HelmSpecial.Forge;
                    break;
                #endregion

                #endregion

                #region Unassigned Region

                // This list serves as an overview of what classes dont have a Forge Enhancement yet, when adding a setup for it, remove it from here
                case "acolyte":
                case "alpha doommega":
                case "alpha omega":
                case "alpha pirate":
                case "arcane dark caster":
                case "assassin":
                case "barber":
                case "bard":
                case "battlemage of love":
                case "battlemage":
                case "beast warrior":
                case "beastmaster":
                case "berserker":
                case "beta berserker":
                case "blademaster assassin":
                case "blademaster":
                case "blood ancient":
                case "blood sorceress":
                case "blood titan":
                case "frostblood titan":
                case "cardclasher":
                case "chaos avenger member preview":
                case "chaos champion prime":
                case "chaos shaper":
                case "chrono assassin":
                case "chrono commandant":
                case "chronocommander":
                case "chronocorrupter":
                case "chronomancer prime":
                case "chronomancer":
                case "chunin":
                case "classic alpha pirate":
                case "classic barber":
                case "classic defender":
                case "classic doomknight":
                case "classic dragonlord":
                case "classic exalted soul cleaver":
                case "classic guardian":
                case "classic paladin":
                case "classic pirate":
                case "classic soul cleaver":
                case "clawsuit":
                case "cryomancer mini pet coming soon":
                case "cryomancer":
                case "daimon":
                case "dark battlemage":
                case "dark caster":
                case "dark chaos berserker":
                case "dark cryomancer":
                case "dark harbinger":
                case "dark legendary hero":
                case "darkblood stormking":
                case "deathknight lord":
                case "deathknight":
                case "defender":
                case "doomknight overlord":
                case "doomknight":
                case "dragon knight":
                case "dragon shinobi":
                case "dragonslayer":
                case "dragonsoul shinobi":
                case "drakel warlord":
                case "elemental dracomancer":
                case "empyrean chronomancer":
                case "enforcer":
                case "evolved clawsuit":
                case "evolved dark caster":
                case "evolved leprechaun":
                case "evolved pumpkin lord":
                case "evolved shaman":
                case "exalted harbinger":
                case "exalted soul cleaver":
                case "firelord summoner":
                case "frost spiritreaver":
                case "glacial berserker test":
                case "grim necromancer":
                case "grunge rocker":
                case "guardian":
                case "heavy metal necro":
                case "heavy metal rockstar":
                case "heroic naval commander":
                case "highseas commander":
                case "hobo highlord":
                case "immortal dark caster":
                case "imperial chunin":
                case "infinite dark caster":
                case "infinite legion dark caster":
                case "infinity titan":
                case "legendary naval commander":
                case "legion blademaster assassin":
                case "legion doomknight tester":
                case "legion evolved dark caster":
                case "legion paladin":
                case "legion revenant member test":
                case "legion swordmaster assassin":
                case "leprechaun":
                case "lightcaster test":
                case "lightmage":
                case "love caster":
                case "lycan":
                case "master ranger":
                case "mechajouster":
                case "mindbreaker":
                case "mystical dark caster":
                case "naval commander":
                case "necromancer":
                case "ninja warrior":
                case "no class":
                case "northlands monk":
                case "not a mod":
                case "nu metal necro":
                case "obsidian no class":
                case "oracle":
                case "overworld chronomancer":
                case "paladin highlord":
                case "paladin":
                case "paladinslayer":
                case "pink romancer":
                case "pinkomancer":
                case "pirate":
                case "prismatic clawsuit":
                case "protosartorium":
                case "psionic mindbreaker":
                case "pumpkin lord":
                case "pyromancer":
                case "ranger":
                case "renegade":
                case "rustbucket":
                case "sakura cryomancer":
                case "sentinel":
                case "shadow dragon shinobi":
                case "shadow ripper":
                case "shadow rocker":
                case "shadowflame dragonlord":
                case "shadowscythe general":
                case "silver paladin":
                case "skycharged grenadier":
                case "skyguard grenadier":
                case "sorcerer":
                case "soul cleaver":
                case "star captain":
                case "starlord":
                case "swordmaster assassin":
                case "swordmaster":
                case "the collector":
                case "thief of hours":
                case "timeless chronomancer":
                case "timeless dark caster":
                case "troubador of love":
                case "unchained rocker":
                case "unchained rockstar":
                case "undead leperchaun":
                case "undeadslayer":
                case "unlucky leperchaun":
                case "vampire":
                case "vindicator of they":
                case "void highlord tester":
                case "warlord":
                case "warrior":
                case "classic warrior":
                case "warrior (rare)":
                case "warriorscythe general":
                case "witch":
                default: // If the correct enhancement arent unlocked, or the class in question isnt in the Forge Enhancement Lib, use Awe Enhancements Lib
                    type = EnhancementType.Lucky;
                    return false;

                #endregion
            }
            return true;

            // // Always place this check as the last one in a 'if' + '||' stack.
            // // See EXAMPLE_CLASS as an example.
            // bool uDauntlessExtra()
            // {
            //     // Check if Dauntless is unlocked, and set it as wSpecial if true.
            //     if (uDauntless())
            //     {
            //         wSpecial = WeaponSpecial.Dauntless;
            //         return true;
            //     }
            //     // If Dauntless is not unlocked, try Valiance and it's extras
            //     // If neither Valiance nor its bonusses are unlocked, this will return false so that it can be used with the 'goto default' lines
            //     else return uValianceExtra();
            // }

            // // Always place this check as the last one in a 'if' + '||' stack.
            // // See ArchPaladin as an example.
            // bool uValianceExtra()
            // {
            //     // Check if Valiance is unlocked, and set it as wSpecial if true.
            //     if (uValiance())
            //         wSpecial = WeaponSpecial.Valiance;
            //     // Otherwise, check if Praxis is unlocked, and set it as wSpecial if true.
            //     else if (uPraxis())
            //         wSpecial = WeaponSpecial.Praxis;
            //     // If neither Valiance and Praxis are not unlocked, return false so that it can be used in conjunction with the 'goto default' lines.
            //     else return false;

            //     // This will only occur if Valiance or Praxis is unlocked.
            //     return true;
            // }
        }

        void AweEnhancementLibrary()
        {
            //tolower incase we accidentaly use capitals.. it breaks
            switch (className)
            {
                #region Lucky Region

                #region Lucky - Spiral Carve
                case "abyssal angel":
                case "abyssal angel's shadow":
                case "artifact hunter":
                case "assassin":
                case "archmage":
                case "beastmaster":
                case "berserker":
                case "beta berserker":
                case "blademaster assassin":
                case "blademaster":
                case "blood titan":
                case "frostblood titan":
                case "cardclasher":
                case "chaos avenger member preview":
                case "chaos champion prime":
                case "chaos slayer":
                case "chaos slayer berserker":
                case "chaos slayer cleric":
                case "chaos slayer mystic":
                case "chaos slayer thief":
                case "chrono chaorruptor":
                case "chrono commandant":
                case "chronocommander":
                case "chronocorrupter":
                case "chunin":
                case "classic alpha pirate":
                case "classic barber":
                case "classic doomknight":
                case "classic exalted soul cleaver":
                case "classic guardian":
                case "classic paladin":
                case "classic pirate":
                case "classic soul cleaver":
                case "continuum chronomancer":
                case "corrupted chronomancer":
                case "dark chaos berserker":
                case "dark harbinger":
                case "doomknight":
                case "empyrean chronomancer":
                case "eternal chronomancer":
                case "evolved clawsuit":
                case "evolved dark caster":
                case "evolved leprechaun":
                case "exalted harbinger":
                case "exalted soul cleaver":
                case "glaceran warlord":
                case "dark glaceran warlord":
                case "savage glaceran warlord":
                case "glacial warlord":
                case "great thief":
                case "hollowborn vindicator member preview":
                case "immortal chronomancer":
                case "imperial chunin":
                case "infinite dark caster":
                case "infinite legion dark caster":
                case "infinity titan":
                case "legion blademaster assassin":
                case "legion evolved dark caster":
                case "legion swordmaster assassin":
                case "leprechaun":
                case "lycan":
                case "master ranger":
                case "mechajouster":
                case "necromancer":
                case "ninja warrior":
                case "not a mod":
                case "overworld chronomancer":
                case "pinkomancer":
                case "prismatic clawsuit":
                case "quantum chronomancer":
                case "ranger":
                case "renegade":
                case "rogue":
                case "classic rogue":
                case "rogue (rare)":
                case "scarlet sorceress":
                case "shadowscythe general":
                case "skycharged grenadier":
                case "skyguard grenadier":
                case "sovereign of storms":
                case "soul cleaver":
                case "starlord":
                case "swordmaster assassin":
                case "swordmaster":
                case "timekeeper":
                case "timekiller":
                case "timeless chronomancer":
                case "undead leperchaun":
                case "undeadslayer":
                case "underworld chronomancer":
                case "unlucky leperchaun":
                case "debris highlord":
                case "void highlord":
                case "void highlord (ioda)":
                case "verus doomknight":
                    type = EnhancementType.Lucky;
                    wSpecial = WeaponSpecial.Spiral_Carve;
                    break;
                #endregion

                #region Lucky - Mana Vamp
                case "alpha doommega":
                case "alpha omega":
                case "alpha pirate":
                case "beast warrior":
                case "blood ancient":
                case "chaos avenger":
                case "chaos shaper":
                case "classic defender":
                case "clawsuit":
                case "cryomancer mini pet coming soon":
                case "dark legendary hero":
                case "dragonsoul shinobi":
                case "ultra omniknight":
                case "dark ultra omninight":
                case "doomknight overlord":
                case "dragonslayer general":
                case "drakel warlord":
                case "glacial berserker test":
                case "heroic naval commander":
                case "legendary elemental warrior":
                case "mythic elemental warrior":
                case "legendary naval commander":
                case "legion revenant member test":
                case "naval commander":
                case "paladin high lord":
                case "paladin":
                case "paladinslayer":
                case "pirate":
                case "pumpkin lord":
                case "shadowflame dragonlord":
                case "shadowstalker of time":
                case "shadowwalker of time":
                case "shadowweaver of time":
                case "silver paladin":
                case "thief of hours":
                case "ultra elemental warrior":
                case "void highlord tester":
                case "warlord":
                case "warrior":
                case "warrior (rare)":
                case "warriorscythe general":
                case "yami no ronin":
                case "arachnomancer":
                    type = EnhancementType.Lucky;
                    wSpecial = WeaponSpecial.Mana_Vamp;
                    break;
                #endregion

                #region Lucky - Awe Blast
                case "archpaladin":
                case "bard":
                case "chrono assassin":
                case "chronomancer":
                case "chronomancer prime":
                case "Phantom Chronomancer":
                case "dark metal necro":
                case "deathknight lord":
                case "dragon shinobi":
                case "dragonlord":
                case "evolved pumpkin lord":
                case "glacial berserker":
                case "grunge rocker":
                case "guardian":
                case "heavy metal necro":
                case "heavy metal rockstar":
                case "hollowborn vindicator":
                case "Hollowborn Vindicator Member Preview":
                case "hobo highlord":
                case "lord of order":
                case "legendary hero":
                case "nechronomancer":
                case "phantom chronomancer":
                case "phantasm chronomancer":
                case "necrotic chronomancer":
                case "Draconic Chronomancer":
                case "no class":
                case "nu metal necro":
                case "obsidian no class":
                case "protosartorium":
                case "shadow dragon shinobi":
                case "shadow ripper":
                case "shadow rocker":
                case "star captain":
                case "troubador of love":
                case "unchained rocker":
                case "unchained rockstar":
                case "undead goat":
                case "unundead goat":
                case "doom metal necro":
                case "neo metal necro":
                case "martial artist":
                case "master martial artist":
                case "antique hunter":
                case "archivist of time":
                    type = EnhancementType.Lucky;
                    wSpecial = WeaponSpecial.Awe_Blast;
                    break;
                #endregion

                #region Lucky - Health Vamp
                case "eternal inversionist":
                case "archfiend":
                case "barber":
                case "classic dragonlord":
                case "dragonslayer":
                case "enforcer":
                case "flame dragon warrior":
                case "rustbucket":
                case "sentinel":
                case "vampire":
                case "vampire lord":
                case "enchanted vampire lord":
                case "royal vampire lord":
                case "chrono shadowhunter":
                    type = EnhancementType.Lucky;
                    wSpecial = WeaponSpecial.Health_Vamp;
                    break;
                #endregion

                #endregion

                #region  Theif Region

                #region  Theif - Mana Vamp
                case "ninja":
                case "classic ninja":
                case "ninja (rare)":
                case "horc evader":
                    type = EnhancementType.Thief;
                    wSpecial = WeaponSpecial.Mana_Vamp;
                    break;
                #endregion

                #endregion

                #region Wizard Region

                #region Wizard - Awe Blast
                case "acolyte":
                case "arcane dark caster":
                case "battlemage":
                case "battlemage of love":
                case "blaze binder":
                case "blood sorceress":
                case "dark battlemage":
                case "dragon knight":
                case "firelord summoner":
                case "grim necromancer":
                case "highseas commander":
                case "infinity knight":
                case "interstellar knight":
                case "master of moglins":
                case "dark master of moglins":
                case "lich":
                case "mystical dark caster":
                case "northlands monk":
                case "royal battlemage":
                case "timeless dark caster":
                case "witch":
                case "stonecrusher":
                case "scion of flames":
                    type = EnhancementType.Wizard;
                    wSpecial = WeaponSpecial.Awe_Blast;
                    break;
                #endregion

                #region Wizard - Spiral Carve
                case "chrono dataknight":
                case "chrono dragonknight":
                case "cryomancer":
                case "dark caster":
                case "dark cryomancer":
                case "darkblood stormking":
                case "darkside":
                case "defender":
                case "frost spiritreaver":
                case "immortal dark caster":
                case "legion paladin":
                case "legion revenant":
                case "legion revenant (ioda)":
                case "lightcaster":
                case "pink romancer":
                case "psionic mindbreaker":
                case "pyromancer":
                case "sakura cryomancer":
                case "troll spellsmith":
                case "classic legion doomknight":
                case "legion doomknight":
                case "legion doomknight tester":
                case "arcana invoker":
                case "king\u0027s echo":
                    type = EnhancementType.Wizard;
                    wSpecial = WeaponSpecial.Spiral_Carve;
                    break;
                #endregion

                #region Wizard - Health Vamp
                case "daimon":
                case "dark lord":
                case "evolved shaman":
                case "lightmage":
                case "mindbreaker":
                case "vindicator of they":
                case "elemental dracomancer":
                case "lightcaster test":
                case "love caster":
                case "mage":
                case "classic mage":
                case "mage (rare)":
                case "sorcerer":
                case "the collector":
                    type = EnhancementType.Wizard;
                    wSpecial = WeaponSpecial.Health_Vamp;
                    break;
                #endregion

                #region Wizard - Mana Vamp
                case "oracle":
                case "shaman":
                    type = EnhancementType.Wizard;
                    wSpecial = WeaponSpecial.Mana_Vamp;
                    break;
                #endregion

                #endregion

                #region Fighter Region

                #region Fighter - Awe Blast
                case "deathknight":
                case "frostval barbarian":
                    type = EnhancementType.Fighter;
                    wSpecial = WeaponSpecial.Awe_Blast;
                    break;
                #endregion

                #endregion

                #region Healer Region

                #region Healer - Health Vamp
                case "dragon of time":
                    type = EnhancementType.Healer;
                    wSpecial = WeaponSpecial.Health_Vamp;
                    break;
                #endregion

                #region Healer - Mana Vamp
                case "obsidian paladin chronomancer":
                case "paladin chronomancer":
                    type = EnhancementType.Healer;
                    wSpecial = WeaponSpecial.Mana_Vamp;
                    break;
                #endregion

                #endregion

                default:
                    Core.Logger(
                        $"SmartEnhance Failed: \"{className}\" is not found in the Smart Enhance Library, please report to @tato2",
                        messageBox: true
                    );
                    return;
            }
        }
    }

    #endregion
}

public enum Auras
{
    Shapeshifted,
    stuff2,
}

public enum GenericGearBoost
{
    cp,
    gold,
    rep,
    exp,
    dmgAll,
}

public enum RacialGearBoost
{
    None,
    Chaos,
    Dragonkin,
    Drakath,
    Elemental,
    Human,
    Orc,
    Undead,
}

public enum EnhancementType // Enhancement Pattern ID
{
    Fighter = 2,
    Thief = 3,
    Hybrid = 5,
    Wizard = 6,
    Healer = 7,
    SpellBreaker = 8,
    Lucky = 9,
}

public enum CapeSpecial // Enhancement Pattern ID
{
    None = 0,
    Forge = 10,
    Absolution = 11,
    Avarice = 12,
    Vainglory = 24,
    Penitence = 29,
    Lament = 30,
}

public enum WeaponSpecial // Proc ID
{
    None = 0,
    Forge = 1,
    Spiral_Carve = 2,
    Awe_Blast = 3,
    Health_Vamp = 4,
    Mana_Vamp = 5,
    Powerword_Die = 6,
    Lacerate = 7,
    Smite = 8,
    Valiance = 9,
    Arcanas_Concerto = 10,
    Acheron = 11,
    Elysium = 12,
    Praxis = 13,
    Dauntless = 14,
    Ravenous = 15,
}

public enum HelmSpecial //Enhancement Pattern ID
{
    None = 0,
    Forge = 10,
    Vim = 25,
    Examen = 26,
    Anima = 28,
    Pneuma = 27,
    Hearty = 32,
}

// Backwards-compatibility helpers: some numeric IDs for WeaponSpecial and HelmSpecial
// have been renumbered. To avoid breaking persisted/external data that stores
// the raw numeric values, use these mapping helpers when reading/writing IDs.
public static class SpecialIdMapper
{
    // Map legacy numeric IDs to current WeaponSpecial values
    public static WeaponSpecial WeaponFromId(int id)
    {
        // Legacy -> Current mappings (handle both old and new numeric IDs)
        switch (id)
        {
            case 0: return WeaponSpecial.Forge; // legacy/current
            case 1: return WeaponSpecial.None; // legacy/current
            case 2: return WeaponSpecial.Spiral_Carve;
            case 3: return WeaponSpecial.Awe_Blast;
            case 4: return WeaponSpecial.Health_Vamp;
            case 5: return WeaponSpecial.Mana_Vamp;
            case 6: return WeaponSpecial.Powerword_Die;
            case 7: return WeaponSpecial.Lacerate;
            case 8: return WeaponSpecial.Smite;
            case 9: return WeaponSpecial.Valiance;
            case 10: return WeaponSpecial.Arcanas_Concerto;
            case 11: return WeaponSpecial.Acheron;
            case 12: return WeaponSpecial.Elysium;
            case 13: return WeaponSpecial.Praxis;
            case 14: return WeaponSpecial.Dauntless;
            case 15: return WeaponSpecial.Ravenous;
            // Unknown/changed IDs: default to None
            default: return WeaponSpecial.None;
        }
    }

    // Map legacy numeric IDs to current HelmSpecial values
    public static HelmSpecial HelmFromId(int id)
    {
        switch (id)
        {
            case 0: return HelmSpecial.None;
            case 10: return HelmSpecial.Forge;
            case 25: return HelmSpecial.Vim;
            case 26: return HelmSpecial.Examen;
            case 27: return HelmSpecial.Pneuma;
            case 28: return HelmSpecial.Anima;
            case 32: return HelmSpecial.Hearty;
            default: return HelmSpecial.None;
        }
    }
}

public enum mergeOptionsEnum
{
    all = 0,
    acOnly = 1,
    mergeMats = 2,
    select = 3,
};
