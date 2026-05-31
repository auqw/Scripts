/*
name: Castle Uniforms Merge
description: This bot will farm the items belonging to the selected mode for the Castle Uniforms Merge [2717] in /swordhavengardens
tags: castle, uniforms, merge, swordhavengardens, darkovia, dusk, steward, duskfall, eagle, familiar, wolfsbane, silverware, set, dacian, wolf, darkovian, dainty, silver, meatcarvers, butler, morph, maid, broom, besom, meatcarver
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CastleUniformsMerge
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
        Core.BankingBlackList.AddRange(new[] { "Cavendish Dusk Crest"});
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("swordhavengardens", 2717, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Cavendish Dusk Crest":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10723); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("swordhavengardens", "Queen's Crocus", "Crocus Poison", 4, isTemp: true);
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
        new Option<bool>("100909", "Darkovia Dusk Steward", "Mode: [select] only\nShould the bot buy \"Darkovia Dusk Steward\" ?", false),
        new Option<bool>("100914", "Duskfall Eagle Familiar", "Mode: [select] only\nShould the bot buy \"Duskfall Eagle Familiar\" ?", false),
        new Option<bool>("100915", "Wolfsbane Silverware Set", "Mode: [select] only\nShould the bot buy \"Wolfsbane Silverware Set\" ?", false),
        new Option<bool>("100916", "Dacian Wolf Familiar", "Mode: [select] only\nShould the bot buy \"Dacian Wolf Familiar\" ?", false),
        new Option<bool>("100918", "Darkovian Silverware Set", "Mode: [select] only\nShould the bot buy \"Darkovian Silverware Set\" ?", false),
        new Option<bool>("100920", "Dainty Silver Meatcarvers", "Mode: [select] only\nShould the bot buy \"Dainty Silver Meatcarvers\" ?", false),
        new Option<bool>("100922", "Wolfsbane Silver Daggers", "Mode: [select] only\nShould the bot buy \"Wolfsbane Silver Daggers\" ?", false),
        new Option<bool>("100910", "Dusk Butler Morph", "Mode: [select] only\nShould the bot buy \"Dusk Butler Morph\" ?", false),
        new Option<bool>("100911", "Dusk Maid Visage", "Mode: [select] only\nShould the bot buy \"Dusk Maid Visage\" ?", false),
        new Option<bool>("100912", "Dusk Butler Hair", "Mode: [select] only\nShould the bot buy \"Dusk Butler Hair\" ?", false),
        new Option<bool>("100913", "Dusk Maid Locks", "Mode: [select] only\nShould the bot buy \"Dusk Maid Locks\" ?", false),
        new Option<bool>("100923", "Duskfall Eagle Broom", "Mode: [select] only\nShould the bot buy \"Duskfall Eagle Broom\" ?", false),
        new Option<bool>("100924", "Duskfall Wolf Besom", "Mode: [select] only\nShould the bot buy \"Duskfall Wolf Besom\" ?", false),
        new Option<bool>("100921", "Wolfsbane Silver Dagger", "Mode: [select] only\nShould the bot buy \"Wolfsbane Silver Dagger\" ?", false),
        new Option<bool>("100919", "Dainty Silver Meatcarver", "Mode: [select] only\nShould the bot buy \"Dainty Silver Meatcarver\" ?", false),
        new Option<bool>("100917", "Darkovian Silverware", "Mode: [select] only\nShould the bot buy \"Darkovian Silverware\" ?", false),
   };
}
