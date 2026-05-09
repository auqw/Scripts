/*
name: WarwickForest Merge
description: This bot will farm the items belonging to the selected mode for the WarwickForest Merge [2706] in /warwickforest
tags: warwickforest, merge, warwickforest, adamas, flamespawn, raider, captain, lord, anjou, duke, formalwear, regalia, ashblight, fur, spines, ex, incendium, unbreakable, fist, fists, morph, horns, guard
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs 
//cs_include Scripts/Other/MergeShops/FortLumaForgeMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class WarwickForestMerge
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
    private static FortLumaForgeMerge FLF
    {
        get => _FLF ??= new FortLumaForgeMerge();
        set => _FLF = value;
    }
    private static FortLumaForgeMerge _FLF;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Red Flame of Rubedo", "Yew Ember" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.WarWickForest();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("warwickforest", 2706, findIngredients, buyOnlyThis, buyMode: buyMode);

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


                case "Red Flame of Rubedo":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10689 : 10688);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster("warwickforest", "r10", "Bottom", "*", "Rubedo Flicker");
                        Core.HuntMonster("warwickforest", "Idea of a Champion", "Kolr's Needle");
                        Core.HuntMonster("warwickforest", "Rubedo Match", "Alkahest", 100);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Yew Ember":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("warwickforest", "Rubedo Elemental", req.Name, quant, false);
                    break;

                case "Blade of Citrinitas":
                    FLF.BuyAllMerge(req.Name);
                    break;


                case "Aiwass Diamond":
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(Core.IsMember ? 10387 : 10385);

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Sal Alembroth", 1, false);
                        Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Milk of Sulfur", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Aeon Dream", 1, false);
                    }
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("95230", "Adamas Flamespawn Raider", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Raider\" ?", false),
        new Option<bool>("95231", "Adamas Flamespawn Captain", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Captain\" ?", false),
        new Option<bool>("95232", "Adamas Flamespawn Lord", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Lord\" ?", false),
        new Option<bool>("94858", "Anjou Duke Formalwear", "Mode: [select] only\nShould the bot buy \"Anjou Duke Formalwear\" ?", false),
        new Option<bool>("94859", "Anjou Duke Regalia", "Mode: [select] only\nShould the bot buy \"Anjou Duke Regalia\" ?", false),
        new Option<bool>("100887", "Flame of Rubedo", "Mode: [select] only\nShould the bot buy \"Flame of Rubedo\" ?", false),
        new Option<bool>("100889", "Rubedo Crown", "Mode: [select] only\nShould the bot buy \"Rubedo Crown\" ?", false),
        new Option<bool>("100890", "Vermillion Vertis Cape", "Mode: [select] only\nShould the bot buy \"Vermillion Vertis Cape\" ?", false),
        new Option<bool>("100891", "Rubedo Resonance Cape", "Mode: [select] only\nShould the bot buy \"Rubedo Resonance Cape\" ?", false),
        new Option<bool>("95239", "Ashblight Fur", "Mode: [select] only\nShould the bot buy \"Ashblight Fur\" ?", false),
        new Option<bool>("95240", "Ashblight Spines", "Mode: [select] only\nShould the bot buy \"Ashblight Spines\" ?", false),
        new Option<bool>("95242", "Dual Ex Incendium", "Mode: [select] only\nShould the bot buy \"Dual Ex Incendium\" ?", false),
        new Option<bool>("95243", "Unbreakable Flamespawn Fist", "Mode: [select] only\nShould the bot buy \"Unbreakable Flamespawn Fist\" ?", false),
        new Option<bool>("95244", "Unbreakable Flamespawn Fists", "Mode: [select] only\nShould the bot buy \"Unbreakable Flamespawn Fists\" ?", false),
        new Option<bool>("94860", "Anjou Duke Hair", "Mode: [select] only\nShould the bot buy \"Anjou Duke Hair\" ?", false),
        new Option<bool>("94861", "Anjou Duke Locks", "Mode: [select] only\nShould the bot buy \"Anjou Duke Locks\" ?", false),
        new Option<bool>("94862", "Anjou Duke Morph", "Mode: [select] only\nShould the bot buy \"Anjou Duke Morph\" ?", false),
        new Option<bool>("94863", "Anjou Duke Visage", "Mode: [select] only\nShould the bot buy \"Anjou Duke Visage\" ?", false),
        new Option<bool>("95233", "Adamas Flamespawn Morph", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Morph\" ?", false),
        new Option<bool>("95234", "Adamas Flamespawn Visage", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Visage\" ?", false),
        new Option<bool>("95235", "Adamas Flamespawn Horns Morph", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Horns Morph\" ?", false),
        new Option<bool>("95236", "Adamas Flamespawn Horns Visage", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Horns Visage\" ?", false),
        new Option<bool>("95237", "Adamas Flamespawn Helm", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Helm\" ?", false),
        new Option<bool>("95238", "Adamas Flamespawn Guard", "Mode: [select] only\nShould the bot buy \"Adamas Flamespawn Guard\" ?", false),
        new Option<bool>("100888", "Flame of Rubedo Hood", "Mode: [select] only\nShould the bot buy \"Flame of Rubedo Hood\" ?", false),
        new Option<bool>("100892", "Blade of Rubedo", "Mode: [select] only\nShould the bot buy \"Blade of Rubedo\" ?", false),
        new Option<bool>("95241", "Ex Incendium Blade", "Mode: [select] only\nShould the bot buy \"Ex Incendium Blade\" ?", false),
   };
}
