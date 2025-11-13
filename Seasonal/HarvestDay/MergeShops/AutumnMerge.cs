/*
name: Autumn Merge
description: This bot will farm the items belonging to the selected mode for the Autumn Merge [2368] in /eventhub
tags: autumn, merge, eventhub, spirit, breath, wind, falls, whisper, whispers, tree, scattered, leaves, fallen, forest, log, owl, guest, friend
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AutumnMerge
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

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Fallen Leaf", "Gold Flake", "Gnarled Wood" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("harvest"))
        {
            Core.Logger("This script is only available during Harvest Day");
            return;
        }
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("eventhub", 2368, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Fallen Leaf":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("eventhub", "Leaf Painter", req.Name, quant, req.Temp, false);
                    break;

                case "Gold Flake":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "blightharvest",
                        "Tantalocust",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Gnarled Wood":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("fearfeast", "OverGourd", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "81605",
            "Spirit of Autumn",
            "Mode: [select] only\nShould the bot buy \"Spirit of Autumn\" ?",
            false
        ),
        new Option<bool>(
            "81606",
            "Visage of Autumn",
            "Mode: [select] only\nShould the bot buy \"Visage of Autumn\" ?",
            false
        ),
        new Option<bool>(
            "81607",
            "Breath of Autumn Wind",
            "Mode: [select] only\nShould the bot buy \"Breath of Autumn Wind\" ?",
            false
        ),
        new Option<bool>(
            "81608",
            "Autumn Falls",
            "Mode: [select] only\nShould the bot buy \"Autumn Falls\" ?",
            false
        ),
        new Option<bool>(
            "81609",
            "Whisper of Autumn",
            "Mode: [select] only\nShould the bot buy \"Whisper of Autumn\" ?",
            false
        ),
        new Option<bool>(
            "81631",
            "Whispers of Autumn",
            "Mode: [select] only\nShould the bot buy \"Whispers of Autumn\" ?",
            false
        ),
        new Option<bool>(
            "81610",
            "Autumn Wind Tree",
            "Mode: [select] only\nShould the bot buy \"Autumn Wind Tree\" ?",
            false
        ),
        new Option<bool>(
            "89500",
            "Scattered Autumn Leaves",
            "Mode: [select] only\nShould the bot buy \"Scattered Autumn Leaves\" ?",
            false
        ),
        new Option<bool>(
            "89501",
            "Fallen Forest Log",
            "Mode: [select] only\nShould the bot buy \"Fallen Forest Log\" ?",
            false
        ),
        new Option<bool>(
            "89502",
            "Forest Owl Guest",
            "Mode: [select] only\nShould the bot buy \"Forest Owl Guest\" ?",
            false
        ),
        new Option<bool>(
            "89503",
            "Forest Owl Friend",
            "Mode: [select] only\nShould the bot buy \"Forest Owl Friend\" ?",
            false
        ),
    };
}
