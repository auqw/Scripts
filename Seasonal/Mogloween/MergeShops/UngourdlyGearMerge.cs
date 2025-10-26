/*
name: Ungourdly Gear Merge
description: This bot will farm the items belonging to the selected mode for the Ungourdly Gear Merge [2629] in /eldritchbattletown
tags: ungourdly, gear, merge, eldritchbattletown, gold, voucher, k, great, calabaza, pumpkin, kings, revenge, fever, kabocha, king, naginata, wrath
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UngourdlyGearMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreMogloween CoreMogloween { get => _CoreMogloween ??= new CoreMogloween(); set => _CoreMogloween = value; }
    private static CoreMogloween _CoreMogloween;



    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Acromegalia Seed" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CoreMogloween.Eldritchbattletown();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("eldritchbattletown", 2629, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Items not setup

                case "Acromegalia Seed":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10457 : 10454); 
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("eldritchbattletown", "Kathool Cultist", "Tier 2 Kathool Member Card", 6, isTemp: false);
                        Core.HuntMonster("eldritchbattletown", "Dzeza Cultist", "Tier 2 Dzeza Member Card", 6, isTemp: false);
                        Core.HuntMonster("eldritchbattletown", "Harvest Acromegalia", "Gourd Twinning", isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("57304", "Gold Voucher 25k", "Mode: [select] only\nShould the bot buy \"Gold Voucher 25k\" ?", false),
        new Option<bool>("89573", "Great Calabaza Blade", "Mode: [select] only\nShould the bot buy \"Great Calabaza Blade\" ?", false),
        new Option<bool>("89574", "Great Calabaza Blades", "Mode: [select] only\nShould the bot buy \"Great Calabaza Blades\" ?", false),
        new Option<bool>("89575", "Pumpkin King's Revenge", "Mode: [select] only\nShould the bot buy \"Pumpkin King's Revenge\" ?", false),
        new Option<bool>("89576", "Dual Pumpkin King's Revenge", "Mode: [select] only\nShould the bot buy \"Dual Pumpkin King's Revenge\" ?", false),
        new Option<bool>("89577", "Pumpkin Fever Staff", "Mode: [select] only\nShould the bot buy \"Pumpkin Fever Staff\" ?", false),
        new Option<bool>("89578", "Kabocha King Naginata", "Mode: [select] only\nShould the bot buy \"Kabocha King Naginata\" ?", false),
        new Option<bool>("89579", "Pumpkin Fever Axe", "Mode: [select] only\nShould the bot buy \"Pumpkin Fever Axe\" ?", false),
        new Option<bool>("89580", "Pumpkin Fever Axes", "Mode: [select] only\nShould the bot buy \"Pumpkin Fever Axes\" ?", false),
        new Option<bool>("89581", "Pumpkin King's Wrath", "Mode: [select] only\nShould the bot buy \"Pumpkin King's Wrath\" ?", false),
        new Option<bool>("89582", "Dual Pumpkin King's Wrath", "Mode: [select] only\nShould the bot buy \"Dual Pumpkin King's Wrath\" ?", false),
   };
}
