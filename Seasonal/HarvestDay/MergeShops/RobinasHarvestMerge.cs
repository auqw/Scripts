/*
name: Robinas Harvest Merge
description: This bot will farm the items belonging to the selected mode for the Robinas Harvest Merge [2366] in /blightharvest
tags: robinas, harvest, merge, blightharvest, turdracolich, hunter, stalker, brimmed, morph, hunting, gear, goldtouched, carver, carvers, rifle, turdraken, carving, cranberry, imp, pet
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class RobinasHarvestMerge
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

    private static CoreHarvestDay CHD
    {
        get => _CHD ??= new CoreHarvestDay();
        set => _CHD = value;
    }
    private static CoreHarvestDay _CHD;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Blight Bone",
                "Turdraken Carver",
                "Harvest Rifle",
                "Turdraken Carvers",
                "Vintage Shotgun",
                "Cranberry Shoulder Imp",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CHD.BlightHarvest();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("blightharvest", 2366, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Blight Bone":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.Name);
                    Core.RegisterQuests(9482);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(
                            "blightharvest",
                            "r5",
                            "Left",
                            "Tantalocust",
                            "Mealy Bug Legs",
                            6
                        );
                        Core.KillMonster(
                            "blightharvest",
                            "r7",
                            "Left",
                            "Fear Gorta",
                            "Hunger Grass",
                            6
                        );
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(
                            "blightharvest",
                            "r10",
                            "Left",
                            "Famine",
                            "Famine's Spice Flakes"
                        );
                        Core.Logger("This item is not setup yet");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Turdraken Carver":
                case "Harvest Rifle":
                case "Turdraken Carvers":
                case "Vintage Shotgun":
                case "Cranberry Shoulder Imp":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(
                        "blightharvest",
                        "r5",
                        "Left",
                        "*",
                        req.Name,
                        isTemp: req.Temp
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "81352",
            "Turdracolich Hunter",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Hunter\" ?",
            false
        ),
        new Option<bool>(
            "81353",
            "Turdracolich Stalker",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Stalker\" ?",
            false
        ),
        new Option<bool>(
            "81354",
            "Brimmed Turdracolich Hunter Hat",
            "Mode: [select] only\nShould the bot buy \"Brimmed Turdracolich Hunter Hat\" ?",
            false
        ),
        new Option<bool>(
            "81355",
            "Brimmed Turdracolich Hunter Hat and Locks",
            "Mode: [select] only\nShould the bot buy \"Brimmed Turdracolich Hunter Hat and Locks\" ?",
            false
        ),
        new Option<bool>(
            "81356",
            "Turdracolich Hunter Morph",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Hunter Morph\" ?",
            false
        ),
        new Option<bool>(
            "81357",
            "Turdracolich Hunter Visage",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Hunter Visage\" ?",
            false
        ),
        new Option<bool>(
            "81638",
            "Turdracolich Stalker Morph",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Stalker Morph\" ?",
            false
        ),
        new Option<bool>(
            "81639",
            "Turdracolich Stalker Visage",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Stalker Visage\" ?",
            false
        ),
        new Option<bool>(
            "81361",
            "Turdracolich Hunting Gear",
            "Mode: [select] only\nShould the bot buy \"Turdracolich Hunting Gear\" ?",
            false
        ),
        new Option<bool>(
            "81364",
            "Gold-Touched Carver",
            "Mode: [select] only\nShould the bot buy \"Gold-Touched Carver\" ?",
            false
        ),
        new Option<bool>(
            "81365",
            "Gold-Touched Carvers",
            "Mode: [select] only\nShould the bot buy \"Gold-Touched Carvers\" ?",
            false
        ),
        new Option<bool>(
            "81367",
            "Gold-Touched Rifle",
            "Mode: [select] only\nShould the bot buy \"Gold-Touched Rifle\" ?",
            false
        ),
        new Option<bool>(
            "81369",
            "Turdraken Carving Gear",
            "Mode: [select] only\nShould the bot buy \"Turdraken Carving Gear\" ?",
            false
        ),
        new Option<bool>(
            "81386",
            "Cranberry Imp Pet",
            "Mode: [select] only\nShould the bot buy \"Cranberry Imp Pet\" ?",
            false
        ),
    };
}
