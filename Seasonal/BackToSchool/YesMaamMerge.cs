/*
name: Yes Ma'am Merge
description: This bot will farm the items belonging to the selected mode for the Yes Ma'am / 1st Grade Merge [2749] in /extracredit
tags: back-to-school, yes maam, 1st grade, merge, extracredit, beat breaker, beatbox, whyphone, selfie, primo, pro, boss, based, ace, cosmo buns
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/BackToSchool/YesMaam.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class YesMaamMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static YesMaam Chain { get => _Chain ??= new YesMaam(); set => _Chain = value; }
    private static YesMaam _Chain;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[]
        {
            "1st Grade School Supplies", "Golden Apple", "Beatbox Boombox", "Golden WHYphones",
            "Old School Selfie Device", "Ace Beat Breaker Morph", "Beat Breaker Beanie Visage",
            "Beat Breaker Cap Morph", "Beat Breaker Beanie Morph", "Beat Breaker Cap Visage",
            "Cool Beat Breaker Visage", "Golden WHYphone",
        });
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("extracredit"))
            return;

        // The shop only unlocks once the 'Yes Ma'am!' test chain is done
        if (!Core.isCompletedBefore(10823))
        {
            Core.Logger("'Yes Ma'am!' test chain not complete, doing it first.");
            Chain.DoChain();
        }

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("extracredit", 2749, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
                #endregion

                // All of these drop from Mind-Controlled Porkon (perma inv items)
                case "1st Grade School Supplies":
                case "Beatbox Boombox":
                case "Golden WHYphone":
                case "Golden WHYphones":
                case "Old School Selfie Device":
                case "Cool Beat Breaker Visage":
                case "Beat Breaker Beanie Morph":
                case "Beat Breaker Beanie Visage":
                case "Beat Breaker Cap Morph":
                case "Beat Breaker Cap Visage":
                case "Ace Beat Breaker Morph":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster("extracredit", "r6", "Left", "Mind-Controlled Porkon", req.Name, quant, isTemp: false);
                    break;

                case "Golden Apple":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8793);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("extracredit", "Dogear", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Gold Voucher 25k":
                    Farm.Voucher(req.Name, quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("101208", "Beat Breaker", "Mode: [select] only\nShould the bot buy \"Beat Breaker\" ?", false),
        new Option<bool>("101230", "Primo Beat Breaker", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker\" ?", false),
        new Option<bool>("101223", "Beatbox Boomboxes", "Mode: [select] only\nShould the bot buy \"Beatbox Boomboxes\" ?", false),
        new Option<bool>("101227", "Golden WHYphone Ultras", "Mode: [select] only\nShould the bot buy \"Golden WHYphone Ultras\" ?", false),
        new Option<bool>("101229", "Old School Selfie Devices", "Mode: [select] only\nShould the bot buy \"Old School Selfie Devices\" ?", false),
        new Option<bool>("101209", "Pro Beat Breaker Morph", "Mode: [select] only\nShould the bot buy \"Pro Beat Breaker Morph\" ?", false),
        new Option<bool>("101210", "Pro Beat Breaker Visage", "Mode: [select] only\nShould the bot buy \"Pro Beat Breaker Visage\" ?", false),
        new Option<bool>("101211", "Boss Beat Breaker Morph", "Mode: [select] only\nShould the bot buy \"Boss Beat Breaker Morph\" ?", false),
        new Option<bool>("101213", "Based Beat Breaker Morph", "Mode: [select] only\nShould the bot buy \"Based Beat Breaker Morph\" ?", false),
        new Option<bool>("101214", "Boss Beat Breaker Visage", "Mode: [select] only\nShould the bot buy \"Boss Beat Breaker Visage\" ?", false),
        new Option<bool>("101218", "Ace Beat Breaker Visage", "Mode: [select] only\nShould the bot buy \"Ace Beat Breaker Visage\" ?", false),
        new Option<bool>("101235", "Top Beat Breaker Shades", "Mode: [select] only\nShould the bot buy \"Top Beat Breaker Shades\" ?", false),
        new Option<bool>("101236", "Cosmo Buns Shades", "Mode: [select] only\nShould the bot buy \"Cosmo Buns Shades\" ?", false),
        new Option<bool>("101237", "Primo Beat Breaker Visage", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Visage\" ?", false),
        new Option<bool>("101238", "Primo Beat Breaker Morph", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Morph\" ?", false),
        new Option<bool>("101239", "Primo Beat Breaker Beanie", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Beanie\" ?", false),
        new Option<bool>("101240", "Primo Beat Breaker Beanie Shades", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Beanie Shades\" ?", false),
        new Option<bool>("101241", "Primo Beat Breaker Beanie Cap", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Beanie Cap\" ?", false),
        new Option<bool>("101242", "Top Beat Breaker Beanie", "Mode: [select] only\nShould the bot buy \"Top Beat Breaker Beanie\" ?", false),
        new Option<bool>("101243", "Primo Beat Breaker Cap Visage", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Cap Visage\" ?", false),
        new Option<bool>("101244", "Primo Beat Breaker Cap", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Cap\" ?", false),
        new Option<bool>("101245", "Primo Beat Breaker Shades", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Shades\" ?", false),
        new Option<bool>("101246", "Primo Beat Breaker Cap Morph", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Cap Morph\" ?", false),
        new Option<bool>("101247", "Primo Beat Breaker Hat Visage", "Mode: [select] only\nShould the bot buy \"Primo Beat Breaker Hat Visage\" ?", false),
        new Option<bool>("101226", "Golden WHYphone Ultra", "Mode: [select] only\nShould the bot buy \"Golden WHYphone Ultra\" ?", false),
    };
}
