/*
name: Witch Queen's Atelier Merge
description: This script will farm the items required to get all the items from the Witch Queen's Atelier Merge Shop.
tags: witch, queen, atelier, merge, birgittaspire, talia, galilera, starlight, chaser, visionary, astrolabe
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;
using System.Collections.Generic;

public class WitchQueensAtelierMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
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
        Core.BankingBlackList.AddRange(new[] { "Gold Voucher 100k", "Witch Princess' Hat", "Starlight Chaser's Morph", "Starlight Chaser's Visage", "Crystalized Darkovian Tear", "Witch Princess' Astrolabe", "Galilera" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("birgittaspire", 2751, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Witch Princess' Hat":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(10824);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster("birgittaspire", "r2", "Left", "Witch Queen Talia", req.Name, quant, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Crystalized Darkovian Tear":
                case "Witch Princess' Astrolabe":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster("birgittaspire", "r2", "Left", "Witch Queen Talia", req.Name, quant, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("Galilera", "Galilera", "Mode: [select] only\nShould the bot buy \"Galilera\" ?", false),
        new Option<bool>("Dual Galilera", "Dual Galilera", "Mode: [select] only\nShould the bot buy \"Dual Galilera\" ?", false),
        new Option<bool>("Noble Cavendish Witch", "Noble Cavendish Witch", "Mode: [select] only\nShould the bot buy \"Noble Cavendish Witch\" ?", false),
        new Option<bool>("Starlight Chaser's Hat", "Starlight Chaser's Hat", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Hat\" ?", false),
        new Option<bool>("Starlight Chaser's Morph", "Starlight Chaser's Morph", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Morph\" ?", false),
        new Option<bool>("Starlight Chaser's Visage", "Starlight Chaser's Visage", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Visage\" ?", false),
        new Option<bool>("Starlight Witch Hat", "Starlight Witch Hat", "Mode: [select] only\nShould the bot buy \"Starlight Witch Hat\" ?", false),
        new Option<bool>("Visionary Astrolabe", "Visionary Astrolabe", "Mode: [select] only\nShould the bot buy \"Visionary Astrolabe\" ?", false),
    };
}
