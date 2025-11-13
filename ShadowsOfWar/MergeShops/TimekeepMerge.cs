/*
name: TimekeepMerge
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class TimekeepMerge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    public static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    public static CoreAdvanced _sAdv;

    private static CoreSoWMats SOWM
    {
        get => _SOWM ??= new CoreSoWMats();
        set => _SOWM = value;
    }
    private static CoreSoWMats _SOWM;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        SoW.Timekeep();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("timekeep", 2161, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Garish Remnant":
                    SOWM.GarishRemnant(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "71608",
            "Mana Guardian",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian\" ?",
            false
        ),
        new Option<bool>(
            "71609",
            "Mana Guardian's Horn",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Horn\" ?",
            false
        ),
        new Option<bool>(
            "71610",
            "Mana Guardian's Helm",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Helm\" ?",
            false
        ),
        new Option<bool>(
            "71611",
            "Mana Guardian's Morph",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Morph\" ?",
            false
        ),
        new Option<bool>(
            "71612",
            "Mana Guardian's Rift",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Rift\" ?",
            false
        ),
        new Option<bool>(
            "71613",
            "Mana Guardian's Orb",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Orb\" ?",
            false
        ),
        new Option<bool>(
            "71614",
            "Mana Guardian's Spear",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Spear\" ?",
            false
        ),
        new Option<bool>(
            "71615",
            "Mana Guardian's Hammer",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Hammer\" ?",
            false
        ),
        new Option<bool>(
            "71616",
            "Mana Guardian's Hammers",
            "Mode: [select] only\nShould the bot buy \"Mana Guardian's Hammers\" ?",
            false
        ),
    };
}
