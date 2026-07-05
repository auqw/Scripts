/*
name: Golden Merge
description: This bot will farm the items belonging to the selected mode for the Golden Merge [1490] in /goldenarena
tags: golden, merge, goldenarena, blessed, inquisitor, karok, queen, hope, dragon, banner, wings, female, horned, mace
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/GoldenArena.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GoldenMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    
    private static GoldenArena GA
    {
        get => _GoldenArena ??= new GoldenArena();
        set => _GoldenArena = value;
    }
    private static GoldenArena _GoldenArena;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Celestial Seal", "Golden Scale", "Golden Rune", "Golden Badge"});
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        GA.StoryLine();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("goldenarena", 1490, findIngredients, buyOnlyThis, buyMode: buyMode);

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

#region Known items

case "Celestial Seal":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Blessed Dragon", req.Name, quant, false, false);
                    break;

case "Golden Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Blessed Dragon", req.Name, quant, false, false);
                    break;

case "Golden Rune":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Blessed Karok", req.Name, quant, false, false);
                    break;

case "Golden Badge":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Blessed Inquisitor", req.Name, quant, false, false);
                    break;
#endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("41864", "Blessed Inquisitor Armor", "Mode: [select] only\nShould the bot buy \"Blessed Inquisitor Armor\" ?", false),
        new Option<bool>("41881", "Blessed Karok", "Mode: [select] only\nShould the bot buy \"Blessed Karok\" ?", false),
        new Option<bool>("41879", "Queen Of Hope", "Mode: [select] only\nShould the bot buy \"Queen Of Hope\" ?", false),
        new Option<bool>("41797", "Golden Dragon Banner", "Mode: [select] only\nShould the bot buy \"Golden Dragon Banner\" ?", false),
        new Option<bool>("41867", "Blessed Inquisitor Cape", "Mode: [select] only\nShould the bot buy \"Blessed Inquisitor Cape\" ?", false),
        new Option<bool>("41884", "Blessed Karok Wings", "Mode: [select] only\nShould the bot buy \"Blessed Karok Wings\" ?", false),
        new Option<bool>("41885", "Queen Of Hope Wings", "Mode: [select] only\nShould the bot buy \"Queen Of Hope Wings\" ?", false),
        new Option<bool>("41865", "Blessed Inquisitor Female Helm", "Mode: [select] only\nShould the bot buy \"Blessed Inquisitor Female Helm\" ?", false),
        new Option<bool>("41866", "Blessed Inquisitor Helm", "Mode: [select] only\nShould the bot buy \"Blessed Inquisitor Helm\" ?", false),
        new Option<bool>("41882", "Blessed Karok Mask", "Mode: [select] only\nShould the bot buy \"Blessed Karok Mask\" ?", false),
        new Option<bool>("41883", "Blessed Karok Horned Mask", "Mode: [select] only\nShould the bot buy \"Blessed Karok Horned Mask\" ?", false),
        new Option<bool>("41880", "Queen Of Hope Helm", "Mode: [select] only\nShould the bot buy \"Queen Of Hope Helm\" ?", false),
        new Option<bool>("41868", "Mace of the Blessed Inquisitor", "Mode: [select] only\nShould the bot buy \"Mace of the Blessed Inquisitor\" ?", false),
   };
}
