/*
name: BankAllItems
description: null
tags: null
*/
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BankAllItems
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots2 Core => CoreBots2.Instance;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "BankAllBlackList";
    public List<IOption> Options = new()
    {
        new Option<bool>("Inventory", "InventoryACBank", "Bank all Ac Inventory Items", true),
        new Option<bool>("House", "HouseACBank", "Bank all Ac House Items", true),
        new Option<bool>("BanknonAc", "BanknonAc", "Bank non-AC items", false),
        new Option<string>("BlackList","BlackList Items","Fill in the items teh bot *shouldn't* bank, split with a , (comma).",""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        BankAll(
            Bot.Config!.Get<bool>("Inventory"),
            Bot.Config!.Get<bool>("House"),
            Bot.Config!.Get<bool>("BanknonAc"),
            Bot.Config!.Get<string>("BlackList") ?? string.Empty
        );

        Core.SetOptions(false);
    }

    public void BankAll(bool inventory, bool house, bool bankNonAc, string blackList)
    {
        bool bankFullLogged = false;

        HashSet<string> blackListedItems = BuildBlackList(blackList);

        Core.Logger(
            $"[{blackListedItems.Count(name => Bot.Inventory.Contains(name))}x Bag Spaces used] BlackList: {string.Join(", ", blackListedItems)}"
        );

        if (inventory)
            ProcessItems(Bot.Inventory.Items, false);

        if (house)
            ProcessItems(Bot.House.Items, true);

        void ProcessItems(IEnumerable<InventoryItem> items, bool isHouse)
        {
            bool movedAny = false;

            foreach (InventoryItem item in items)
            {
                if (blackListedItems.Contains(item.Name)
                    || item.Equipped
                    || item.Wearing
                    || (!bankNonAc && !item.Coins))
                    continue;

                if (!item.Coins && Bot.Bank.FreeSlots == 0)
                {
                    if (!bankFullLogged)
                    {
                        Core.Logger($"{Core.Username()}'s Bank is full");
                        bankFullLogged = true;
                    }
                    continue;
                }

                BankItem(item, isHouse, bankNonAc);
                movedAny = true;
            }

            Core.Logger($"{(isHouse ? "House" : "Inventory")} Items: {(movedAny ? "✅" : "Nothing to bank")}");
        }
    }

    private void BankItem(InventoryItem item, bool isHouse, bool bankNonAc)
    {
        if (item.Coins && bankNonAc)
        {
            if (isHouse)
                Bot.House.EnsureToBank(item.ID);
            else
                Bot.Inventory.EnsureToBank(item.ID);

            Bot.Wait.ForPickup(item.Name);
            return;
        }

        if (isHouse)
            Core.ToHouseBank(item.ID);
        else
            Core.ToBank(item.ID);
    }

    private HashSet<string> BuildBlackList(string blackList)
    {
        HashSet<string> set = new(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(blackList))
            foreach (string item in blackList.Split(','))
                set.Add(item.Trim());

        foreach (string name in new[]
        {
        Core.SoloClass,
        Core.FarmClass,
        Core.DodgeClass,
        Core.BossClass,
        "Treasure Potion"
    })
            set.Add(name);

        set.UnionWith(Core.FarmGear);
        set.UnionWith(Core.SoloGear);
        set.UnionWith(Core.DodgeGear);
        set.UnionWith(Core.BossGear);
        set.UnionWith(Core.BankingBlackList);

        return set;
    }

}
