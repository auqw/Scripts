/*
name: ManaHarvest Merge
description: This bot will farm the items belonging to the selected mode for the ManaHarvest Merge [2503] in /manaharvest
tags: manaharvest, merge, manaharvest, shadowslayer, leathers, leather, cap, ritual, headstone, shadowslayerss, quiver, immortal, shadow, breaker, breakers, shadowslayers, heirloom, rifle, bow
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using System.Globalization;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ManaHarvestMerge
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

    private static CoreHarvestDay CHD
    {
        get => _CHD ??= new CoreHarvestDay();
        set => _CHD = value;
    }
    private static CoreHarvestDay _CHD;

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
            new[] { "Emergency Rations", "Glossly Chestnut Waves", "Glossy Chestnut Locks" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CHD.ManaHarvest();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("manaharvest", 2503, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Emergency Rations":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            9975,
                            ("manaharvest", CHD.UMManaHarvest[1], ClassType.Farm),
                            ("manaharvest", CHD.UMManaHarvest[7], ClassType.Solo),
                            ("manaharvest", CHD.UMManaHarvest[4], ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Glossly Chestnut Waves":
                case "Glossy Chestnut Locks":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "manaharvest",
                        CHD.UMManaHarvest[7],
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
            "88554",
            "ShadowSlayer Leathers",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayer Leathers\" ?",
            false
        ),
        new Option<bool>(
            "88557",
            "ShadowSlayer Leather Hat",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayer Leather Hat\" ?",
            false
        ),
        new Option<bool>(
            "88558",
            "ShadowSlayer Leather Cap",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayer Leather Cap\" ?",
            false
        ),
        new Option<bool>(
            "88559",
            "Ritual Headstone",
            "Mode: [select] only\nShould the bot buy \"Ritual Headstone\" ?",
            false
        ),
        new Option<bool>(
            "88560",
            "ShadowSlayers's Leather Quiver",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayers's Leather Quiver\" ?",
            false
        ),
        new Option<bool>(
            "88561",
            "Immortal Shadow Breaker",
            "Mode: [select] only\nShould the bot buy \"Immortal Shadow Breaker\" ?",
            false
        ),
        new Option<bool>(
            "88562",
            "Immortal Shadow Breakers",
            "Mode: [select] only\nShould the bot buy \"Immortal Shadow Breakers\" ?",
            false
        ),
        new Option<bool>(
            "88563",
            "ShadowSlayer's Heirloom Rifle",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayer's Heirloom Rifle\" ?",
            false
        ),
        new Option<bool>(
            "88564",
            "ShadowSlayer's Heirloom Bow",
            "Mode: [select] only\nShould the bot buy \"ShadowSlayer's Heirloom Bow\" ?",
            false
        ),
    };
}
