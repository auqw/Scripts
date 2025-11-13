/*
name: Adeptis Pslams Merge
description: This bot will farm the items belonging to the selected mode for the Adeptis Pslams Merge [2588] in /sunkencity
tags: adeptis, pslams, merge, sunkencity, riptide, helicoprion, adept, lure, caeruleum, newborn, chimeric, aurichalcum, lance, ceremonial, trident, enlightened
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/SummerBreak/SunkenCity.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AdeptisPslamsMerge
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

    private static SunkenCity SC
    {
        get => _SC ??= new SunkenCity();
        set => _SC = value;
    }
    private static SunkenCity _SC;

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
                "Sea Creature Membrane",
                "Oxidized Steel",
                "Riptide Helicoprion",
                "Riptide Helicoprion Helm",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        SC.SagaName();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("sunkencity", 2588, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Sea Creature Membrane":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(93822);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonsterQuest(10275, "sunkencity", "Merdrathoolian");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Oxidized Steel":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(93823);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonsterQuest(10276, "sunkencity", "Nereid Princess");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Riptide Helicoprion":
                case "Riptide Helicoprion Helm":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(94014);
                    Core.HuntMonster(
                        "sunkencity",
                        "Nereid Princess",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94015",
            "Riptide Helicoprion Adept",
            "Mode: [select] only\nShould the bot buy \"Riptide Helicoprion Adept\" ?",
            false
        ),
        new Option<bool>(
            "94017",
            "Riptide Helicoprion Locks",
            "Mode: [select] only\nShould the bot buy \"Riptide Helicoprion Locks\" ?",
            false
        ),
        new Option<bool>(
            "94018",
            "Riptide Helicoprion Lure",
            "Mode: [select] only\nShould the bot buy \"Riptide Helicoprion Lure\" ?",
            false
        ),
        new Option<bool>(
            "94019",
            "Riptide Helicoprion Lure Locks",
            "Mode: [select] only\nShould the bot buy \"Riptide Helicoprion Lure Locks\" ?",
            false
        ),
        new Option<bool>(
            "94021",
            "Caeruleum Newborn",
            "Mode: [select] only\nShould the bot buy \"Caeruleum Newborn\" ?",
            false
        ),
        new Option<bool>(
            "94024",
            "Chimeric Aurichalcum Lance",
            "Mode: [select] only\nShould the bot buy \"Chimeric Aurichalcum Lance\" ?",
            false
        ),
        new Option<bool>(
            "94025",
            "Ceremonial Aurichalcum Trident",
            "Mode: [select] only\nShould the bot buy \"Ceremonial Aurichalcum Trident\" ?",
            false
        ),
        new Option<bool>(
            "94026",
            "Enlightened Aurichalcum Trident",
            "Mode: [select] only\nShould the bot buy \"Enlightened Aurichalcum Trident\" ?",
            false
        ),
    };
}
