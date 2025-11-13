/*
name: Flock Merge
description: This bot will farm the items belonging to the selected mode for the Flock Merge [1218] in /battlefowl
tags: flock, merge, battlefowl, chicken, morph, self, plucked, chickencow, claws, nugget, on, a, stick, chickenwing, armblades, your, head, archimoodes, house, guest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FlockMerge
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
        Core.BankingBlackList.AddRange(
            new[] { "Golden Egg", "Golden Feather", "Chicken Claw", "Chickenwing ArmBlade" }
        );
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge()
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("battlefowl", 1218, findIngredients);

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

                case "Golden Egg":
                case "Chickenwing ArmBlade":
                case "Golden Feather":
                case "Chicken Claw":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("battlefowl", "Chicken", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "33913",
            "Chicken Morph Helm",
            "Mode: [select] only\nShould the bot buy \"Chicken Morph Helm\" ?",
            false
        ),
        new Option<bool>(
            "33914",
            "Chicken Self",
            "Mode: [select] only\nShould the bot buy \"Chicken Self\" ?",
            false
        ),
        new Option<bool>(
            "33915",
            "Plucked Chicken",
            "Mode: [select] only\nShould the bot buy \"Plucked Chicken\" ?",
            false
        ),
        new Option<bool>(
            "33919",
            "Dual Chickencow Claws",
            "Mode: [select] only\nShould the bot buy \"Dual Chickencow Claws\" ?",
            false
        ),
        new Option<bool>(
            "33923",
            "Nugget on a Stick",
            "Mode: [select] only\nShould the bot buy \"Nugget on a Stick\" ?",
            false
        ),
        new Option<bool>(
            "33929",
            "Dual Chickenwing ArmBlades",
            "Mode: [select] only\nShould the bot buy \"Dual Chickenwing ArmBlades\" ?",
            false
        ),
        new Option<bool>(
            "33926",
            "Chicken on Your Head",
            "Mode: [select] only\nShould the bot buy \"Chicken on Your Head\" ?",
            false
        ),
        new Option<bool>(
            "33927",
            "Chicken on Your Locks",
            "Mode: [select] only\nShould the bot buy \"Chicken on Your Locks\" ?",
            false
        ),
        new Option<bool>(
            "92896",
            "Archimoodes House Guest",
            "Mode: [select] only\nShould the bot buy \"Archimoodes House Guest\" ?",
            false
        ),
    };
}
