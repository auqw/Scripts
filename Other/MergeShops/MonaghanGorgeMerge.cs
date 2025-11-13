/*
name: MonaghanGorge Merge
description: This bot will farm the items belonging to the selected mode for the MonaghanGorge Merge [2499] in /monaghangorge
tags: monaghangorge, merge, monaghangorge, enchanted, tricksters, apprentice, omen, trickster, morph, dark, cards, assistant, barb, barbs, trick, card, allure, grinning, stage
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class MonaghanGorgeMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Apocalyptic Nihil Coin",
                "Trickster's Apprentice",
                "Dark Omen Trickster Morph",
                "Dark Omen Trickster Hair",
                "Dark Omen Trickster Visage",
                "Dark Omen Trickster Locks",
                "Dark Omen Cards",
                "Dark Omen Card Familiar",
                "Trickster's Dark Omen",
                "Dark Omen Barb",
                "Dark Omen Barbs",
                "Dark Omen Trick Card",
                "Trickster's Hidden Grin",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("monaghangorge", 2499, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                case "Apocalyptic Nihil Coin":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9929, "monaghangorge", "Trickster Duartaine");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Trickster's Apprentice":
                case "Dark Omen Trickster Morph":
                case "Dark Omen Trickster Hair":
                case "Dark Omen Trickster Visage":
                case "Dark Omen Trickster Locks":
                case "Dark Omen Cards":
                case "Dark Omen Card Familiar":
                case "Trickster's Dark Omen":
                case "Dark Omen Barb":
                case "Dark Omen Barbs":
                case "Dark Omen Trick Card":
                case "Trickster's Hidden Grin":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "monaghangorge",
                        "Trickster Duartaine",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "89421",
            "Enchanted Trickster's Apprentice",
            "Mode: [select] only\nShould the bot buy \"Enchanted Trickster's Apprentice\" ?",
            false
        ),
        new Option<bool>(
            "88740",
            "Enchanted Omen Trickster Morph",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trickster Morph\" ?",
            false
        ),
        new Option<bool>(
            "88741",
            "Enchanted Omen Trickster Hair",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trickster Hair\" ?",
            false
        ),
        new Option<bool>(
            "88742",
            "Enchanted Omen Trickster Visage",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trickster Visage\" ?",
            false
        ),
        new Option<bool>(
            "88743",
            "Enchanted Omen Trickster Locks",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trickster Locks\" ?",
            false
        ),
        new Option<bool>(
            "88744",
            "Enchanted Dark Omen Cards",
            "Mode: [select] only\nShould the bot buy \"Enchanted Dark Omen Cards\" ?",
            false
        ),
        new Option<bool>(
            "88745",
            "Dark Omen Trickster's Assistant",
            "Mode: [select] only\nShould the bot buy \"Dark Omen Trickster's Assistant\" ?",
            false
        ),
        new Option<bool>(
            "88748",
            "Enchanted Dark Omen Barb",
            "Mode: [select] only\nShould the bot buy \"Enchanted Dark Omen Barb\" ?",
            false
        ),
        new Option<bool>(
            "88749",
            "Enchanted Dark Omen Barbs",
            "Mode: [select] only\nShould the bot buy \"Enchanted Dark Omen Barbs\" ?",
            false
        ),
        new Option<bool>(
            "88750",
            "Enchanted Omen Trick Card",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trick Card\" ?",
            false
        ),
        new Option<bool>(
            "88751",
            "Enchanted Omen Trick Cards",
            "Mode: [select] only\nShould the bot buy \"Enchanted Omen Trick Cards\" ?",
            false
        ),
        new Option<bool>(
            "89423",
            "Dark Omen Allure",
            "Mode: [select] only\nShould the bot buy \"Dark Omen Allure\" ?",
            false
        ),
        new Option<bool>(
            "88747",
            "Trickster's Grinning Stage",
            "Mode: [select] only\nShould the bot buy \"Trickster's Grinning Stage\" ?",
            false
        ),
    };
}
