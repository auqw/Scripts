/*
name: Remnants of a Champion Merge
description: This bot will farm the items belonging to the selected mode for the Remnants of a Champion Merge [2711] in /flameusurper
tags: remnants, of, a, champion, merge, flameusurper, flame, maleno, albedo, citrinitas, rubedo, usurper, manifestations, magnum, opus, wings, divine, quintessence, eye, extinction, house, intro, scion, destruction, horns, morph, companion, liberta, aeterna
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs 
//cs_include Scripts/Other/MergeShops/CarcossaCanteenMerge.cs
//cs_include Scripts/Other/MergeShops/ForgeMalenoMerge.cs
//cs_include Scripts/Other/MergeShops/FortLumaForgeMerge.cs
//cs_include Scripts/Other/MergeShops/WarwickForestMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class RemnantsofachampionMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;
    private static WarwickForestMerge WFM
    {
        get => _WFM ??= new WarwickForestMerge();
        set => _WFM = value;
    }
    private static WarwickForestMerge _WFM;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Great Flame of Yew", "Cinders of a Champion", "Blade of Rubedo" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.DoAll();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("flameusurper", 2711, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
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

                case "Great Flame of Yew":
                case "Cinders of a Champion":
                    Core.Logger("As of the time of making this, the bot *Cannot* farm this item. farm it yourself manaully then rerun this.");
                    break;

                case "Blade of Rubedo":
                    WFM.BuyAllMerge(req.Name);
                    break;

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("100873", "Flame of Maleno", "Mode: [select] only\nShould the bot buy \"Flame of Maleno\" ?", false),
        new Option<bool>("100877", "Flame of Albedo", "Mode: [select] only\nShould the bot buy \"Flame of Albedo\" ?", false),
        new Option<bool>("100881", "Flame of Citrinitas", "Mode: [select] only\nShould the bot buy \"Flame of Citrinitas\" ?", false),
        new Option<bool>("100887", "Flame of Rubedo", "Mode: [select] only\nShould the bot buy \"Flame of Rubedo\" ?", false),
        new Option<bool>("100945", "Flame Usurper Manifestations", "Mode: [select] only\nShould the bot buy \"Flame Usurper Manifestations\" ?", false),
        new Option<bool>("100867", "Magnum Opus Wings", "Mode: [select] only\nShould the bot buy \"Magnum Opus Wings\" ?", false),
        new Option<bool>("100868", "Divine Quintessence Wings", "Mode: [select] only\nShould the bot buy \"Divine Quintessence Wings\" ?", false),
        new Option<bool>("100869", "Eye of Extinction", "Mode: [select] only\nShould the bot buy \"Eye of Extinction\" ?", false),
        new Option<bool>("100948", "Flame of Maleno House Intro", "Mode: [select] only\nShould the bot buy \"Flame of Maleno House Intro\" ?", false),
        new Option<bool>("100872", "Flame Scion Gauntlets", "Mode: [select] only\nShould the bot buy \"Flame Scion Gauntlets\" ?", false),
        new Option<bool>("100863", "Scion of Destruction Helm", "Mode: [select] only\nShould the bot buy \"Scion of Destruction Helm\" ?", false),
        new Option<bool>("100864", "Scion of Destruction Mask", "Mode: [select] only\nShould the bot buy \"Scion of Destruction Mask\" ?", false),
        new Option<bool>("100865", "Scion of Destruction Horns", "Mode: [select] only\nShould the bot buy \"Scion of Destruction Horns\" ?", false),
        new Option<bool>("100866", "Scion of Destruction Morph", "Mode: [select] only\nShould the bot buy \"Scion of Destruction Morph\" ?", false),
        new Option<bool>("100947", "Flame Usurper Companion Blades", "Mode: [select] only\nShould the bot buy \"Flame Usurper Companion Blades\" ?", false),
        new Option<bool>("100946", "Flame Usurper Blades", "Mode: [select] only\nShould the bot buy \"Flame Usurper Blades\" ?", false),
        new Option<bool>("100870", "Liberta Aeterna", "Mode: [select] only\nShould the bot buy \"Liberta Aeterna\" ?", false),
   };
}
