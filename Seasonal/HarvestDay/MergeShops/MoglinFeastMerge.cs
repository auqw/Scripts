/*
name: Moglin Feast Merge
description: This bot will farm the items belonging to the selected mode for the Moglin Feast Merge [2637] in /moglinfeast
tags: moglin, feast, merge, moglinfeast, fall, forest, traveler, morph, cap, fruit, assortment, fae, ruler, king, queen, assassin, scarf
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class MoglinFeastMerge
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
    private static CoreHarvestDay HarvestDay
    {
        get => _HarvestDay ??= new CoreHarvestDay();
        set => _HarvestDay = value;
    }
    private static CoreHarvestDay _HarvestDay;

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
                "Golden Leaves",
                "Persimmon Branch",
                "Fall Fruit Basket",
                "Fae Dust",
                "Fall Fae Assassin Morph",
                "Fall Fae Assassin Visage",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        HarvestDay.MoglinFeast();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("moglinfeast", 2637, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Items not setup

                case "Golden Leaves":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            10498,
                            ("moglinfeast", "Hay Fever Sylph", ClassType.Farm),
                            ("moglinfeast", "Redcap Mush", ClassType.Farm),
                            ("moglinfeast", "Fall Fae Queen", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Persimmon Branch":
                case "Fall Fruit Basket":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "moglinfeast",
                        "Pumpkin Mimic",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Fae Dust":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(
                        "moglinfeast",
                        "r7",
                        "Left",
                        "*",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Fall Fae Assassin Morph":
                case "Fall Fae Assassin Visage":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "moglinfeast",
                        "Fall Fae Queen",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94699",
            "Fall Forest Traveler",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler\" ?",
            false
        ),
        new Option<bool>(
            "94700",
            "Fall Forest Traveler Hair",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Hair\" ?",
            false
        ),
        new Option<bool>(
            "94701",
            "Fall Forest Traveler Locks",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Locks\" ?",
            false
        ),
        new Option<bool>(
            "94702",
            "Fall Forest Traveler Morph",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Morph\" ?",
            false
        ),
        new Option<bool>(
            "94703",
            "Fall Forest Traveler Visage",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Visage\" ?",
            false
        ),
        new Option<bool>(
            "94704",
            "Fall Forest Traveler Hat",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Hat\" ?",
            false
        ),
        new Option<bool>(
            "94705",
            "Fall Forest Traveler Cap",
            "Mode: [select] only\nShould the bot buy \"Fall Forest Traveler Cap\" ?",
            false
        ),
        new Option<bool>(
            "94711",
            "Fall Fruit Assortment",
            "Mode: [select] only\nShould the bot buy \"Fall Fruit Assortment\" ?",
            false
        ),
        new Option<bool>(
            "96967",
            "Fall Fae Ruler",
            "Mode: [select] only\nShould the bot buy \"Fall Fae Ruler\" ?",
            false
        ),
        new Option<bool>(
            "96968",
            "Fall Fae King",
            "Mode: [select] only\nShould the bot buy \"Fall Fae King\" ?",
            false
        ),
        new Option<bool>(
            "96969",
            "Fall Fae Queen",
            "Mode: [select] only\nShould the bot buy \"Fall Fae Queen\" ?",
            false
        ),
        new Option<bool>(
            "96972",
            "Fall Fae Assassin",
            "Mode: [select] only\nShould the bot buy \"Fall Fae Assassin\" ?",
            false
        ),
        new Option<bool>(
            "96975",
            "Fall Fae Assassin Mask",
            "Mode: [select] only\nShould the bot buy \"Fall Fae Assassin Mask\" ?",
            false
        ),
        new Option<bool>(
            "96976",
            "Fall Fae Assassin Scarf",
            "Mode: [select] only\nShould the bot buy \"Fall Fae Assassin Scarf\" ?",
            false
        ),
    };
}
