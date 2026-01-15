/*
name: Secret Map Quest
description: if you own the (rare) beta berserker class, this will do the quest [5516] for the rewards.
tags: beta berserker armor, dark berserker, beta berserker, secret map, rare, pseudo-rare
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

public class SecretMapQuest
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private const int CandleShopId = 1317;
    private const string CandleName = "Golden 8th Birthday Candle";
    private const string DarkBerserkerName = "Dark Berserker";

    private readonly Dictionary<string, int> Months = new()
    {
        { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 },
        { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 },
        { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 }
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoQuest();

        Core.SetOptions(false);
    }

    public void DoQuest()
    {
        if (OwnsDarkBerserker())
        {
            Core.Logger("You already own Dark Berserker.");
            return;
        }

        InventoryItem? BetaBerserker = Bot.Inventory.Items.Find(i =>
            i.Name.Equals("Beta Berserker", StringComparison.OrdinalIgnoreCase) &&
            i.Category == ItemCategory.Class
        );

        if (BetaBerserker == null || Core.CheckInventory(Core.QuestRewards(5516)))
            return;

        if (!EnsureGolden8thBirthdayCandle())
        {
            Bot.Stop(true);
            return;
        }

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

            while (BetaBerserker.Quantity < 1)
                Core.KillMonster("battleontown", "Enter", "Spawn", "*", log: false);

            Core.EnsureAccept(5516);
            Core.HuntMonster("nostalgiaquest", "Boss Zardman", "Secret Map");
            Core.EnsureComplete(5516);
        }
    }

    private bool OwnsDarkBerserker()
    {
        return Bot.Inventory.Items.Any(i => i.Name.Equals(DarkBerserkerName, StringComparison.OrdinalIgnoreCase)) ||
               Bot.Bank.Items.Any(i => i.Name.Equals(DarkBerserkerName, StringComparison.OrdinalIgnoreCase));
    }

    private bool EnsureGolden8thBirthdayCandle()
    {
        Core.Unbank(CandleName);
        if (Core.CheckInventory(CandleName))
            return true;

        Core.Logger("Checking if Your Acc is 8 Years Old");

        if (!IsAccountAtLeast8YearsOld())
        {
            Core.Logger("your acc isn't old enough.");
            return false;
        }

        Core.BuyItem(Bot.Map.Name, CandleShopId, CandleName);
        Bot.Wait.ForPickup(CandleName);

        return Core.CheckInventory(CandleName);
    }

    private bool IsAccountAtLeast8YearsOld()
    {
        try
        {
            string raw = Bot.Flash.GetGameObject("world.myAvatar.objData.dCreated")!;
            string[] parts = raw[1..^1].Split(' ');
            string[] time = parts[3].Split(':');

            DateTime creationDate = new DateTime(
                int.Parse(parts[5]),
                Months[parts[1]],
                int.Parse(parts[2]),
                int.Parse(time[0]),
                int.Parse(time[1]),
                int.Parse(time[2]),
                DateTimeKind.Unspecified
            );

            return creationDate.AddYears(8) <= DateTime.Now;
        }
        catch
        {
            return false;
        }
    }
}
