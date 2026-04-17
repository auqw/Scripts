/*
name: Crownsreach FXIII Merge
description: This bot will farm the items belonging to the selected mode for the Crownsreach FXIII Merge [1354] in /crownsreachfxiii
tags: crownsreach, fxiii, merge, crownsreachfxiii, dandy, gilded, tentacle, wings, fascinator, dapper, cane
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CrownsreachFXIIIMerge
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
        Core.BankingBlackList.AddRange(new[] { "Amethyst Gem" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.IsMember)
        {
            Core.Logger("Membership required for map, cannot do the merge");
            return;
        }

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("crownsreachfxiii", 1354, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Amethyst Gem":
                    Core.EquipClass(ClassType.Farm);
                    while (Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster("crownsreachfxiii", "r4", "Left", "*");
                    Bot.Wait.ForPickup(req.ID);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("38191", "Crownsreach Dandy", "Mode: [select] only\nShould the bot buy \"Crownsreach Dandy\" ?", false),
        new Option<bool>("38194", "Gilded Tentacle Wings", "Mode: [select] only\nShould the bot buy \"Gilded Tentacle Wings\" ?", false),
        new Option<bool>("38192", "Crownsreach Fascinator", "Mode: [select] only\nShould the bot buy \"Crownsreach Fascinator\" ?", false),
        new Option<bool>("38193", "Dapper Dandy Hair", "Mode: [select] only\nShould the bot buy \"Dapper Dandy Hair\" ?", false),
        new Option<bool>("38195", "Dapper Gilded Cane", "Mode: [select] only\nShould the bot buy \"Dapper Gilded Cane\" ?", false),
   };
}
