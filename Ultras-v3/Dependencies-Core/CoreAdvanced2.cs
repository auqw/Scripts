/*
name: null
description: null
tags: null
*/
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
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

public class CoreAdvanced2
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 Core => CoreBots2.Instance;
    private static CoreFarms2 Farm
    {
        get => _Farm ??= new CoreFarms2();
        set => _Farm = value;
    }
    private static CoreFarms2 _Farm;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    #region Shop

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

    public void StartBuyAllMerge(string map, int shopID, Action findIngredients, string? buyOnlyThis = null, string[]? itemBlackList = null, mergeOptionsEnum? buyMode = null, string Group = "First", int ShopItemID = 0, bool Log = true)
    {
        #region Setup and Initialization
        if (
            buyOnlyThis == null
            && buyMode == null
            && Bot.Config != null
            && !Bot.Config.Get<bool>(CoreBots2.Instance.SkipOptions)
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

    public List<ItemCategory> miscCatagories = new()
    {
        ItemCategory.Note,
        ItemCategory.Item,
        ItemCategory.QuestItem,
        ItemCategory.ServerUse,
    };
    public ItemBase externalItem = new();
    public int externalQuant = 0;
    public bool matsOnly = false;
    public List<string> MaxStackOneItems = new();
    public List<string> AltFarmItems = new();

    /// <summary>
    /// The list of ScriptOptions for any merge script.
    /// </summary>
    public List<IOption> MergeOptions = new()
    {
        CoreBots2.Instance.SkipOptions,
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
    };

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

        Bot.Hunt.ForItem(monster, item, quant, isTemp);

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


    private readonly List<string> ReEquippedItems = new();
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
        try
        {
            new CoreEnhancements().EnhanceEquipped(type, cSpecial, hSpecial, wSpecial);
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

        try
        {
            new CoreEnhancements().EnhanceItem(item, type, cSpecial, hSpecial, wSpecial, logging);
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

        try
        {
            new CoreEnhancements().EnhanceItem(items, type, cSpecial, hSpecial, wSpecial);
        }
        catch (Exception e)
        {
            AdvCrash(e);
        }
    }

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
                                ) ?? Array.Empty<string>()
                        )
                        + (st.FrameCount > maxFrames ? "\n…" : "");

            string inner = "";
            if (ex.InnerException != null)
            {
                string[] innerLines =
                    ex.InnerException.StackTrace?.Split('\n') ?? Array.Empty<string>();
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

    public readonly ItemCategory[] WeaponCatagories =
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
    };

    // All u* unlock checks are now consolidated in CoreEnhancements.
    // These thin wrappers preserve the public API for external scripts (Adv.uXxx()).
    private static readonly CoreEnhancements _enhancements = new();
    public bool uAwe() => _enhancements.uAwe();
    public bool uForgeWeapon() => _enhancements.uForgeWeapon();
    public bool uLacerate() => _enhancements.uLacerate();
    public bool uSmite() => _enhancements.uSmite();
    public bool uValiance() => _enhancements.uValiance();
    public bool uArcanasConcerto() => _enhancements.uArcanasConcerto();
    public bool uAbsolution() => _enhancements.uAbsolution();
    public bool uVainglory() => _enhancements.uVainglory();
    public bool uAvarice() => _enhancements.uAvarice();
    public bool uForgeCape() => _enhancements.uForgeCape();
    public bool uElysium() => _enhancements.uElysium();
    public bool uAcheron() => _enhancements.uAcheron();
    public bool uPenitence() => _enhancements.uPenitence();
    public bool uLament() => _enhancements.uLament();
    public bool uVim() => _enhancements.uVim();
    public bool uExamen() => _enhancements.uExamen();
    public bool uForgeHelm() => _enhancements.uForgeHelm();
    public bool uPneuma() => _enhancements.uPneuma();
    public bool uAnima() => _enhancements.uAnima();
    public bool uDauntless() => _enhancements.uDauntless();
    public bool uPraxis() => _enhancements.uPraxis();
    public bool uRavenous() => _enhancements.uRavenous();
    public bool uHearty() => _enhancements.uHearty();

    #endregion

    #region SmartEnhance

    /// <summary>
    /// Automatically finds the best Enhancement for the given class and enhances all equipped gear with it too
    /// </summary>
    /// <param name="className">Name of the class you wish to enhance</param>
    /// <param name="ForceEnh">For classes that are recieved unenhanced</param>
    public void SmartEnhance(string? className, bool ForceEnh = false)
    {
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

        className = SelectedClass.Name.ToLower();

        // If the class isn't enhanced yet, enhance it first
        if (SelectedClass.EnhancementLevel <= 0)
        {
            new CoreEnhancements().EnhanceItem(SelectedClass.Name, EnhancementType.Lucky);
            if (ForceEnh)
                return;
        }

        Core.Equip(SelectedClass.Name ?? className);
        Bot.Wait.ForTrue(() => Bot.Player.CurrentClass?.Name == className, 40);

        // Use CoreEnhancements.Apply for the full enhancement with presets
        new CoreEnhancements().Apply(SelectedClass.Name ?? className);
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

public enum mergeOptionsEnum
{
    all = 0,
    acOnly = 1,
    mergeMats = 2,
    select = 3,
};
