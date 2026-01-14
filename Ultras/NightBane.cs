/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class NightBane
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "NightBane";
    public List<IOption> Options = new()
    {
        new Option<PlayerCount>("PlayerCount", "How many accounts", "Number of accounts (between 4-7) we'll be using", PlayerCount.Four),
        new Option<ItemToFarm>("Item", "Item to Farm", "Which item to farm (choose 'All' for all items)", ItemToFarm.Insatiable_Hunger),
        new Option<int>("Quantity", "Quantity", "Number of items to check/farm", 1),
        CoreBots.Instance.SkipOptions,
    };

    // Get selected item and quantity
    int quantityToFarm => Bot.Config!.Get<int>("Quantity");
    ItemToFarm selectedItem => Bot.Config!.Get<ItemToFarm>("Item"); // Keep this as enum
    int Players => (int)Bot.Config!.Get<PlayerCount>("PlayerCount");
    int GetQuantity(ItemToFarm item) => item == ItemToFarm.Starlit_Journal_Page_3 ? 10 : quantityToFarm;

    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        Core.Boot();
        Fight();
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "voidNightBane";
        const string boss = "NightBane";

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // 'Wrong Turn at Voidbuquerque' && 'Doom Spikes'
        C.EnsureAcceptmultiple(9418, 7713);
        C.AddDrop("Nightbane's ??? Essence", "Insatiable Hunger", "Chest Plate", "Starlit Journal Page 3 Scraps");

        Core.Join(map);
        Ultra.WaitForArmy(Players, "NightBane.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
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
                Adv.GearStore(true, true);
                break;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);

            Bot.Sleep(500);
        }
    }

    bool InventoryHasItems()
    {
        if (selectedItem == ItemToFarm.All)
        {
            // Check all items except "All"
            return Enum.GetValues<ItemToFarm>()
                .Where(i => i != ItemToFarm.All)
                .All(i => Bot.Inventory.Contains((int)i, GetQuantity(i)));
        }
        else
        {
            // Check single item
            return Bot.Inventory.Contains((int)selectedItem, GetQuantity(selectedItem));
        }
    }
}

enum PlayerCount
{
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7
}

public enum ItemToFarm
{
    All = 0,                  // Special value for "all items"
    Nightbanes_Essence = 73862,
    Insatiable_Hunger = 73361,
    Chest_Plate = 40066,
    Starlit_Journal_Page_3 = 56682
}
