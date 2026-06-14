/*
name: Nova Genesis Hangar Merge
description: This bot will farm the items belonging to the selected mode for the Nova Genesis Hangar Merge [2732] in /carcossacabins
tags: nova, genesis, hangar, merge, carcossacabins, wolfblade, suit, runehawk, mystraven, swainson, mk, iii, wings, amplified, hawk, ci, orion, visor, awakened, ii, cc, imparatus
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NovaGenesisHangarMerge
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
        Core.BankingBlackList.AddRange(new[] { "EP Cell", "Runehawk Nova Genesis Visor", "Wolfblade Nova Genesis Helm", "Mystraven Nova Genesis Visor" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("carcossacabins", 2732, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "EP Cell":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop("Wolfblade Nova Genesis Helm", "Mystraven Nova Genesis Visor", "Runehawk Nova Genesis Visor");
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10760);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("carcossacabins", "Clementine", "Clementine's Blood Sample");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("carcossacabins", "Evolved Lifeform", "Lifeform's Fingers Ganglion", 9);
                        Core.HuntMonster("carcossacabins", "Doom Leech", "Leech's Ganglion", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Wolfblade Nova Genesis Helm":
                case "Mystraven Nova Genesis Visor":
                case "Runehawk Nova Genesis Visor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    // Add all because they are ac tagged and we may need them or can bank them
                    Core.AddDrop("Wolfblade Nova Genesis Helm", "Mystraven Nova Genesis Visor", "Runehawk Nova Genesis Visor");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("carcossacabins", "Clementine", req.Name, isTemp: false);
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
        new Option<bool>("101556", "Wolfblade Nova Genesis Suit", "Mode: [select] only\nShould the bot buy \"Wolfblade Nova Genesis Suit\" ?", false),
        new Option<bool>("101564", "Runehawk Nova Genesis Suit", "Mode: [select] only\nShould the bot buy \"Runehawk Nova Genesis Suit\" ?", false),
        new Option<bool>("101573", "Mystraven Nova Genesis Suit", "Mode: [select] only\nShould the bot buy \"Mystraven Nova Genesis Suit\" ?", false),
        new Option<bool>("101572", "Swainson MK III", "Mode: [select] only\nShould the bot buy \"Swainson MK III\" ?", false),
        new Option<bool>("101577", "Mystraven Nova Genesis Wings", "Mode: [select] only\nShould the bot buy \"Mystraven Nova Genesis Wings\" ?", false),
        new Option<bool>("101568", "Amplified Nova Hawk Wings", "Mode: [select] only\nShould the bot buy \"Amplified Nova Hawk Wings\" ?", false),
        new Option<bool>("101559", "Wolfblade Nova Genesis Wings", "Mode: [select] only\nShould the bot buy \"Wolfblade Nova Genesis Wings\" ?", false),
        new Option<bool>("101563", "Dual C.I. Orion", "Mode: [select] only\nShould the bot buy \"Dual C.I. Orion\" ?", false),
        new Option<bool>("101565", "Runehawk Nova Genesis Helm", "Mode: [select] only\nShould the bot buy \"Runehawk Nova Genesis Helm\" ?", false),
        new Option<bool>("101558", "Wolfblade Nova Genesis Visor", "Mode: [select] only\nShould the bot buy \"Wolfblade Nova Genesis Visor\" ?", false),
        new Option<bool>("101574", "Mystraven Nova Genesis Helm", "Mode: [select] only\nShould the bot buy \"Mystraven Nova Genesis Helm\" ?", false),
        new Option<bool>("101576", "Awakened Nova Genesis Helm", "Mode: [select] only\nShould the bot buy \"Awakened Nova Genesis Helm\" ?", false),
        new Option<bool>("101571", "Swainson MK II", "Mode: [select] only\nShould the bot buy \"Swainson MK II\" ?", false),
        new Option<bool>("101580", "C.C. Imparatus MK II", "Mode: [select] only\nShould the bot buy \"C.C. Imparatus MK II\" ?", false),
        new Option<bool>("101581", "C.C. Imparatus MK III", "Mode: [select] only\nShould the bot buy \"C.C. Imparatus MK III\" ?", false),
        new Option<bool>("101562", "C.I. Orion", "Mode: [select] only\nShould the bot buy \"C.I. Orion\" ?", false),
   };
}
