/*
name: Swordhaven Chevaliers Merge
description: This bot will farm the items belonging to the selected mode for the Swordhaven Chevaliers Merge [2712] in /swordhavengardens
tags: swordhaven, chevaliers, merge, swordhavengardens, corroded, chevalier, gilded, rebels, relic, reavers, armaments, reawakened, visor, armet, reaver, lance
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SwordhavenChevaliersMerge
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
        Core.BankingBlackList.AddRange(new[] { "Granville Knight's Crest", "Reginolds Datura Crest", "Volkov Gold Crest" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("swordhavengardens", 2712, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Granville Knight's Crest":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10721); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("swordhavengardens", "Swordhaven Knight", "Knight's Card", 10, isTemp: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Reginolds Datura Crest":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10722); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("swordhavengardens", "Blithe Roses", "Violet Rose Honey", isTemp: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Volkov Gold Crest":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10724); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("swordhavengardens", "Noble Ghost", "Antique Brooch", 14, isTemp: true);
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
        new Option<bool>("100738", "Corroded Chevalier", "Mode: [select] only\nShould the bot buy \"Corroded Chevalier\" ?", false),
        new Option<bool>("100746", "Gilded Chevalier", "Mode: [select] only\nShould the bot buy \"Gilded Chevalier\" ?", false),
        new Option<bool>("100744", "Rebel's Relic Reavers", "Mode: [select] only\nShould the bot buy \"Rebel's Relic Reavers\" ?", false),
        new Option<bool>("100745", "Rebel's Relic Armaments", "Mode: [select] only\nShould the bot buy \"Rebel's Relic Armaments\" ?", false),
        new Option<bool>("100752", "Rebel's Reawakened Reavers", "Mode: [select] only\nShould the bot buy \"Rebel's Reawakened Reavers\" ?", false),
        new Option<bool>("100753", "Rebel's Reawakened Armaments", "Mode: [select] only\nShould the bot buy \"Rebel's Reawakened Armaments\" ?", false),
        new Option<bool>("100747", "Gilded Visor", "Mode: [select] only\nShould the bot buy \"Gilded Visor\" ?", false),
        new Option<bool>("100748", "Gilded Chevalier Helm", "Mode: [select] only\nShould the bot buy \"Gilded Chevalier Helm\" ?", false),
        new Option<bool>("100749", "Gilded Chevalier Armet", "Mode: [select] only\nShould the bot buy \"Gilded Chevalier Armet\" ?", false),
        new Option<bool>("100739", "Corroded Chevalier Visor", "Mode: [select] only\nShould the bot buy \"Corroded Chevalier Visor\" ?", false),
        new Option<bool>("100740", "Corroded Chevalier Helm", "Mode: [select] only\nShould the bot buy \"Corroded Chevalier Helm\" ?", false),
        new Option<bool>("100741", "Corroded Chevalier Armet", "Mode: [select] only\nShould the bot buy \"Corroded Chevalier Armet\" ?", false),
        new Option<bool>("100743", "Rebel's Relic Reaver", "Mode: [select] only\nShould the bot buy \"Rebel's Relic Reaver\" ?", false),
        new Option<bool>("100751", "Rebel's Reawakened Reaver", "Mode: [select] only\nShould the bot buy \"Rebel's Reawakened Reaver\" ?", false),
        new Option<bool>("100742", "Rebel's Relic Lance", "Mode: [select] only\nShould the bot buy \"Rebel's Relic Lance\" ?", false),
        new Option<bool>("100750", "Rebel's Reawakened Lance", "Mode: [select] only\nShould the bot buy \"Rebel's Reawakened Lance\" ?", false),
   };
}
