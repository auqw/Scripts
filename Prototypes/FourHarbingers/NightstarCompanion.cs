/*
name: Nightstar Companion
description: Farms the Four Harbingers prerequisites and merges the Nightstar Companion.
tags: nightstar companion, four harbingers, fourharbingers, merge, scrolls, vouchers
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/0DoAllFourHarbingers.cs
//cs_include Scripts/Prototypes/FourHarbingers/Halosis.cs
//cs_include Scripts/Prototypes/FourHarbingers/Bello.cs
//cs_include Scripts/Prototypes/FourHarbingers/Fame.cs
//cs_include Scripts/Prototypes/FourHarbingers/Mors.cs
//cs_include Scripts/Prototypes/FourHarbingers/AnethyxosAbsolution.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NightstarCompanion
{
    private const string Map = "fourharbingers";
    private const int TroveShop = 2762;

    private readonly IScriptInterface Bot = IScriptInterface.Instance;
    private readonly CoreBots Core = CoreBots.Instance;
    private readonly CoreAdvanced Adv = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "FourHarbingers_NightstarCompanion";

    public List<IOption> Options = new()
    {
        new Option<bool>("UsePotions", "Use Potions", "Use the optimized potions during each fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the optimized enhancements before each fight.", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (Core.CheckInventory("Nightstar Companion"))
        {
            Core.Logger("Nightstar Companion already obtained.");
            return;
        }

        bool fourHarbingersComplete =
            Core.isCompletedBefore(10850)
            && Core.isCompletedBefore(10851)
            && Core.isCompletedBefore(10852)
            && Core.isCompletedBefore(10853)
            && Core.isCompletedBefore(10854);

        if (!fourHarbingersComplete)
        {
            DoAllFourHarbingers story = new();
            story.ScriptMain(bot);

            if (!Core.isCompletedBefore(10850)
                || !Core.isCompletedBefore(10851)
                || !Core.isCompletedBefore(10852)
                || !Core.isCompletedBefore(10853)
                || !Core.isCompletedBefore(10854))
            {
                Core.Logger("The Four Harbingers storyline is incomplete; stopping before farming Trove materials.", messageBox: true);
                return;
            }
        }

        Core.BankingBlackList.AddRange(
            new[]
            {
                "Gold Voucher 100k",
                "Gold Voucher 500k",
                "Scroll of the Heretic",
                "Scroll of the Wanderer",
                "Scroll of the Innocent",
                "Scroll of the Benevolent",
                "Scroll of the Preacher",
                "Celestial Dragon of Time",
                "Harbinger of Conquest",
                "Harbinger of Death",
                "Harbinger of Famine",
                "Harbinger of War",
                "Infernal Dragon of Time",
            }
        );

        Adv.MergeItemisinShopExceptions.AddRange(
            new[]
            {
                "Scroll of the Heretic",
                "Scroll of the Wanderer",
                "Scroll of the Innocent",
                "Scroll of the Benevolent",
                "Scroll of the Preacher",
            }
        );

        foreach (string item in new[]
        {
            "Celestial Dragon of Time",
            "Harbinger of Conquest",
            "Harbinger of Death",
            "Harbinger of Famine",
            "Harbinger of War",
            "Infernal Dragon of Time",
            "Nightstar Companion",
        })
        {
            if (Bot.ShouldExit)
                return;

            if (Core.CheckInventory(item))
                continue;

            Core.Logger($"Attempting merge for {item}...");

            Adv.StartBuyAllMerge(
                Map,
                TroveShop,
                FindIngredient,
                buyOnlyThis: item,
                buyMode: mergeOptionsEnum.all
            );

            // Retry once if merge failed
            if (!Core.CheckInventory(item))
            {
                Core.Logger($"{item} not obtained on first attempt, retrying...");
                Adv.StartBuyAllMerge(
                    Map,
                    TroveShop,
                    FindIngredient,
                    buyOnlyThis: item,
                    buyMode: mergeOptionsEnum.all
                );
            }

            if (!Core.CheckInventory(item))
            {
                Core.Logger($"{item} could not be obtained. Continuing to next item.");
                continue;
            }
        }

        if (Core.CheckInventory("Nightstar Companion"))
            Core.Logger("Nightstar Companion complete.");
        else
            Core.Logger("Nightstar Companion was not obtained; restart the script to continue from the remaining requirements.");
    }

    private void FindIngredient()
    {
        ItemBase requirement = Adv.externalItem;
        int requiredQuantity = Adv.externalQuant;

        if (requirement == null || string.IsNullOrEmpty(requirement.Name))
            return;

        int currentQuantity = Bot.Inventory.GetQuantity(requirement.Name);
        int needed = requiredQuantity - currentQuantity;

        if (needed <= 0)
            return;

        switch (requirement.Name)
        {
            case "Scroll of the Heretic":
                FarmScroll(requirement.Name, requiredQuantity, () =>
                {
                    AnethyxosAbsolution absolution = new() { DoAllMode = true, FarmQuantity = requiredQuantity };
                    absolution.ScriptMain(Bot);
                });
                break;

            case "Scroll of the Wanderer":
                FarmScroll(requirement.Name, requiredQuantity, () =>
                {
                    Halosis halosis = new() { DoAllMode = true, FarmQuantity = requiredQuantity };
                    halosis.ScriptMain(Bot);
                });
                break;

            case "Scroll of the Innocent":
                FarmScroll(requirement.Name, requiredQuantity, () =>
                {
                    Mors mors = new() { DoAllMode = true, FarmQuantity = requiredQuantity };
                    mors.ScriptMain(Bot);
                });
                break;

            case "Scroll of the Benevolent":
                FarmScroll(requirement.Name, requiredQuantity, () =>
                {
                    Fame fame = new() { DoAllMode = true, FarmQuantity = requiredQuantity };
                    fame.ScriptMain(Bot);
                });
                break;

            case "Scroll of the Preacher":
                FarmScroll(requirement.Name, requiredQuantity, () =>
                {
                    Bello bello = new() { DoAllMode = true, FarmQuantity = requiredQuantity };
                    bello.ScriptMain(Bot);
                });
                break;

            default:
                Core.Logger($"No farm registered for {requirement.Name}. Skipping.", messageBox: true);
                break;
        }
    }

    private void FarmScroll(string itemName, int quantity, Action runBoss)
    {
        Core.FarmingLogger(itemName, quantity);

        int failCount = 0;
        int previousQuantity = Bot.Inventory.GetQuantity(itemName);

        while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quantity))
        {
            runBoss();

            // Wait up to 20 seconds for quest turn-in or delayed pickup
            Bot.Wait.ForTrue(() =>
                Bot.Inventory.GetQuantity(itemName) > previousQuantity,
                20
            );

            int currentQuantity = Bot.Inventory.GetQuantity(itemName);

            if (currentQuantity > previousQuantity)
            {
                Core.Logger($"{itemName}: Progress made ({currentQuantity}/{quantity}).");
                previousQuantity = currentQuantity;
                failCount = 0;
                continue;
            }

            failCount++;

            if (failCount >= 5)
            {
                Core.Logger($"No progress after 5 attempts for {itemName}. Stopping this scroll.");
                break;
            }

            Core.Logger($"{itemName}: No progress, retrying (attempt {failCount}/5).");
        }
    }
}
