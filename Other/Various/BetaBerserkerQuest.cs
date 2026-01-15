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
    private const string DarkBerserkerName = "Dark Berserker";

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoQuest();

        Core.SetOptions(false);
    }

    public void DoQuest()
    {
        // 🚫 Stop immediately if Dark Berserker is already owned
        if (OwnsDarkBerserker())
        {
            Core.Logger("You already own Dark Berserker.");
            return;
        }

        InventoryItem? BetaBerserker = Bot.Inventory.Items.Find(i =>
            i.Name.Equals("Beta Berserker", System.StringComparison.OrdinalIgnoreCase)
            && i.Category == ItemCategory.Class
        );

        if (BetaBerserker == null || Core.CheckInventory(Core.QuestRewards(5516)))
            return;

        // Ensure Candle before proceeding
        if (!EnsureGolden8thBirthdayCandle())
            return;

        Core.Unbank("Beta Berserker");
        Core.AddDrop(Core.QuestRewards(5516));

        while (!Core.CheckInventory(Core.QuestRewards(5516)))
        {
            if (OwnsDarkBerserker())
            {
                Core.Logger("You already own Dark Berserker.");
                Bot.Stop(true);
                return;
            }

            while (BetaBerserker != null && BetaBerserker.Quantity < 1)
                Core.KillMonster("battleontown", "Enter", "Spawn", "*", log: false);

            Core.EnsureAccept(5516);
            Core.HuntMonster("nostalgiaquest", "Boss Zardman", "Secret Map");
            Core.EnsureComplete(5516);
        }
    }

    private bool OwnsDarkBerserker()
    {
        bool inInventory = Bot.Inventory.Items
            .Any(i => i.Name.Equals(DarkBerserkerName, System.StringComparison.OrdinalIgnoreCase));

        bool inBank = Bot.Bank.Items
            .Any(i => i.Name.Equals(DarkBerserkerName, System.StringComparison.OrdinalIgnoreCase));

        return inInventory || inBank;
    }

    private bool EnsureGolden8thBirthdayCandle()
    {
        // Try unbank first (safe even if not banked)
        Core.Unbank(CandleName);

        if (Core.CheckInventory(CandleName))
            return true;

        Bot.Shops.Load(CandleShopId);

        bool inShop = Bot.Shops.Items != null &&
                      Bot.Shops.Items.Any(i =>
                          i.Name.Equals(CandleName, System.StringComparison.OrdinalIgnoreCase));

        if (!inShop)
        {
            Core.Logger("Your account needs to be at least 8 years old to complete this quest.");
            return false;
        }

        Bot.Shops.BuyItem(CandleName, CandleShopId);
        Bot.Wait.ForPickup(CandleName);

        if (!Core.CheckInventory(CandleName))
        {
            Core.Logger("Your account needs to be at least 8 years old to complete this quest.");
            return false;
        }

        return true;
    }
}
