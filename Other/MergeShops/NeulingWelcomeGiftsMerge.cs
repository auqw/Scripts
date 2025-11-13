/*
name: Neuling Welcome Gifts Merge
description: This bot will farm the items belonging to the selected mode for the Neuling Welcome Gifts Merge [2617] in /thelimacity
tags: neuling, welcome, gifts, merge, thelimacity, dwarfhold, explorer, morph, wool, cap, friend, explorers, bag, mountain, range, brandt, aegis, , melano, zircon, catalyst, stahlschwert
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NeulingWelcomeGiftsMerge
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
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Yew Ember", "Steel Ingot" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.ThelimaCity();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("thelimacity", 2617, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                #region Items not setup

                case "Yew Ember":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Maleno Match", req.Name, quant, false, false);
                    break;

                case "Steel Ingot":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            Core.IsMember ? 10358 : 10357,
                            ("thelimacity", "Maleno Elemental", ClassType.Solo),
                            ("thelimacity", "Maleno Match", ClassType.Farm),
                            ("thelimacity", "Flame of Maleno", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92345",
            "Dwarfhold Explorer",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Explorer\" ?",
            false
        ),
        new Option<bool>(
            "92346",
            "Dwarfhold Explorer Morph",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Explorer Morph\" ?",
            false
        ),
        new Option<bool>(
            "92347",
            "Dwarfhold Explorer Visage",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Explorer Visage\" ?",
            false
        ),
        new Option<bool>(
            "92348",
            "Dwarfhold Explorer Hair",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Explorer Hair\" ?",
            false
        ),
        new Option<bool>(
            "92349",
            "Dwarfhold Explorer Locks",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Explorer Locks\" ?",
            false
        ),
        new Option<bool>(
            "92350",
            "Dwarfhold Wool Cap",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Wool Cap\" ?",
            false
        ),
        new Option<bool>(
            "92351",
            "Dwarfhold Wool Friend",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Wool Friend\" ?",
            false
        ),
        new Option<bool>(
            "92352",
            "Explorer's Bag",
            "Mode: [select] only\nShould the bot buy \"Explorer's Bag\" ?",
            false
        ),
        new Option<bool>(
            "92353",
            "Dwarfhold Mountain Range",
            "Mode: [select] only\nShould the bot buy \"Dwarfhold Mountain Range\" ?",
            false
        ),
        new Option<bool>(
            "92354",
            "Neuling Brandt",
            "Mode: [select] only\nShould the bot buy \"Neuling Brandt\" ?",
            false
        ),
        new Option<bool>(
            "92355",
            "Dual Neuling Brandt",
            "Mode: [select] only\nShould the bot buy \"Dual Neuling Brandt\" ?",
            false
        ),
        new Option<bool>(
            "92358",
            "Neuling Aegis",
            "Mode: [select] only\nShould the bot buy \"Neuling Aegis\" ?",
            false
        ),
        new Option<bool>(
            "92359",
            "Neuling Brandt + Aegis",
            "Mode: [select] only\nShould the bot buy \"Neuling Brandt + Aegis\" ?",
            false
        ),
        new Option<bool>(
            "93387",
            "Melano Zircon Catalyst",
            "Mode: [select] only\nShould the bot buy \"Melano Zircon Catalyst\" ?",
            false
        ),
        new Option<bool>(
            "92356",
            "Neuling Stahlschwert",
            "Mode: [select] only\nShould the bot buy \"Neuling Stahlschwert\" ?",
            false
        ),
        new Option<bool>(
            "92357",
            "Dual Neuling Stahlschwert",
            "Mode: [select] only\nShould the bot buy \"Dual Neuling Stahlschwert\" ?",
            false
        ),
        new Option<bool>(
            "92360",
            "Neuling Stahlschwert + Aegis",
            "Mode: [select] only\nShould the bot buy \"Neuling Stahlschwert + Aegis\" ?",
            false
        ),
    };
}
