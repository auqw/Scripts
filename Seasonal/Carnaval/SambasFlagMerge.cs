/*
name: Sambas Flag Merge
description: This bot will farm the items belonging to the selected mode for the Sambas Flag Merge [2237] in /sambaflag
tags: sambas, flag, merge, sambaflag, sambista, dorival, moglin, jorge, zeca, encanto, encantado, cavaquinho, pandeiro, tantan, dança, das, sombras, dançarina, circlet, diadem, de, ouro, escuro
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Carnaval/SambaFlag.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SambasFlagMerge
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
    private static SambasFlag SF
    {
        get => _SF ??= new SambasFlag();
        set => _SF = value;
    }
    private static SambasFlag _SF;
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
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
        Core.BankingBlackList.AddRange(new[] { "Costume Piece", "Ceremonial Standard", "Cavaquinho", "Pandeiro", "Tantan", "Dança das Sombras Hair", "Dança das Sombras Locks" });

        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("Sambaflag"))
            return;

        //story for materials quest.
        SF.StoryLine();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("sambaflag", 2237, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Ceremonial Standard":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9115);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("sambaflag", "Flag Bearer", "Flag Standard");
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("sambaflag", "Master Of Ceremonies", "Ceremony Feather");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Costume Piece":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9110);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("bloodtusk", "Jungle Vulture", "Vulture Feathers", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("bloodtusk", "Rhison", "Rhison Fur", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Pandeiro":
                case "Tantan":
                case "Cavaquinho":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("sambaflag", "Master Of Ceremonies", req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Dança das Sombras Hair":
                case "Dança das Sombras Locks":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("sambaflag", "Flag Bearer", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("76366", "Sambista Armor", "Mode: [select] only\nShould the bot buy \"Sambista Armor\" ?", false),
        new Option<bool>("76367", "Sambista Helm", "Mode: [select] only\nShould the bot buy \"Sambista Helm\" ?", false),
        new Option<bool>("76371", "Dorival The Moglin", "Mode: [select] only\nShould the bot buy \"Dorival The Moglin\" ?", false),
        new Option<bool>("76372", "Jorge The Moglin", "Mode: [select] only\nShould the bot buy \"Jorge The Moglin\" ?", false),
        new Option<bool>("76373", "Zeca The Moglin", "Mode: [select] only\nShould the bot buy \"Zeca The Moglin\" ?", false),
        new Option<bool>("76375", "Encanto Sambista Armor", "Mode: [select] only\nShould the bot buy \"Encanto Sambista Armor\" ?", false),
        new Option<bool>("76376", "Encantado Sambista Hat", "Mode: [select] only\nShould the bot buy \"Encantado Sambista Hat\" ?", false),
        new Option<bool>("76377", "Encantado Cavaquinho", "Mode: [select] only\nShould the bot buy \"Encantado Cavaquinho\" ?", false),
        new Option<bool>("76378", "Encantado Pandeiro", "Mode: [select] only\nShould the bot buy \"Encantado Pandeiro\" ?", false),
        new Option<bool>("76379", "Encantado Tantan", "Mode: [select] only\nShould the bot buy \"Encantado Tantan\" ?", false),
        new Option<bool>("99284", "Dança das Sombras", "Mode: [select] only\nShould the bot buy \"Dança das Sombras\" ?", false),
        new Option<bool>("99287", "Dança das Sombras Mask", "Mode: [select] only\nShould the bot buy \"Dança das Sombras Mask\" ?", false),
        new Option<bool>("99288", "Dançarina das Sombras Mask", "Mode: [select] only\nShould the bot buy \"Dançarina das Sombras Mask\" ?", false),
        new Option<bool>("99289", "Dança das Sombras Circlet", "Mode: [select] only\nShould the bot buy \"Dança das Sombras Circlet\" ?", false),
        new Option<bool>("99290", "Dança das Sombras Diadem", "Mode: [select] only\nShould the bot buy \"Dança das Sombras Diadem\" ?", false),
        new Option<bool>("99291", "Dança das Sombras Visage", "Mode: [select] only\nShould the bot buy \"Dança das Sombras Visage\" ?", false),
        new Option<bool>("99389", "Tantan de Ouro Encantado", "Mode: [select] only\nShould the bot buy \"Tantan de Ouro Encantado\" ?", false),
        new Option<bool>("99463", "Tantan Escuro", "Mode: [select] only\nShould the bot buy \"Tantan Escuro\" ?", false),
   };
}
