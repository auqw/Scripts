/*
name: Secret Map Quest
description: if you own the (rare) beta berserker class, this will do the quest [5516] for the rewards.
tags: beta berserker armor, dark berserker, beta berserker, secret map, rare, pseudo-rare
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using System.Linq;

public class SecretMapQuest
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private const int CandleShopId = 1317;
    private const string CandleName = "Golden 8th Birthday Candle";

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoQuest();

        Core.SetOptions(false);
    }

    public void DoQuest()
    {
        InventoryItem? BetaBerserker = Bot.Inventory.Items.Find(i =>
            i.Name.ToLower().Trim() == "Beta Berserker"
            && i.Category == ItemCategory.Class
        );

        if (BetaBerserker == null || Core.CheckInventory(Core.QuestRewards(5516)))
            return;

        // Check/Buy Candle before proceeding
        if (!EnsureGolden8thBirthdayCandle())
            return;

        Core.Unbank("Beta Berserker");
        Core.AddDrop(Core.QuestRewards(5516));

        while (!Core.CheckInventory(Core.QuestRewards(5516)))
        {
            while (BetaBerserker != null && BetaBerserker.Quantity < 1)
                Core.KillMonster("battleontown", "Enter", "Spawn", "*", log: false);

            Core.EnsureAccept(5516);
            Core.HuntMonster("nostalgiaquest", "Boss Zardman", "Secret Map");
            Core.EnsureComplete(5516);
        }
    }

    private bool EnsureGolden8thBirthdayCandle()
    {

        if (Core.CheckInventory(CandleName))
            return true;

        Bot.Shops.Load(CandleShopId);

        bool inShop = Bot.Shops.Items != null &&
                      Bot.Shops.Items.Any(i => i != null &&
                          i.Name.Equals(CandleName, System.StringComparison.OrdinalIgnoreCase));

        if (!inShop)
        {
            Core.Logger("You account needs to be at least 8 years old to complete this quest.");
            return false;
        }

        Bot.Shops.BuyItem(CandleName, CandleShopId);
        Bot.Wait.ForPickup(CandleName);

        if (!Core.CheckInventory(CandleName))
        {
            Core.Logger("You account needs to be at least 8 years old to complete this quest.");
            return false;
        }

        return true;
    }
}
