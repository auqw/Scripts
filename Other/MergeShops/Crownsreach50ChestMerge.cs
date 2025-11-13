/*
name: Crownsreach50ChestMerge
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Other/ShadowflameWarMedal.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class Crownsreach50ChestMerge
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

    private static ShadowflameWarMedal SWM
    {
        get => _SWM ??= new ShadowflameWarMedal();
        set => _SWM = value;
    }
    private static ShadowflameWarMedal _SWM;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "ShadowFlame Battle Staff",
                "ShadowFlame War Medal",
                "ShadowFlame Battle Spear",
                "ShadowFlame Broadsword ",
            }
        );
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("chaosamulet", 1915, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "ShadowFlame Battle Spear":
                case "ShadowFlame Battle Staff":
                    Core.FarmingLogger(req.Name, quant);
                    SWM.Medals(100);
                    Adv.BuyItem("chaosamulet", 1914, req.Name);
                    break;

                case "ShadowFlame War Medal":
                    Core.FarmingLogger(req.Name, quant);
                    SWM.Medals(quant);
                    break;

                case "ShadowFlame Broadsword":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("chaosamulet", "Goldun", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "54260",
            "Enchanted Shadow Battle Staff",
            "Mode: [select] only\nShould the bot buy \"Enchanted Shadow Battle Staff\" ?",
            false
        ),
        new Option<bool>(
            "54262",
            "Enchanted Shadow Battle Spear",
            "Mode: [select] only\nShould the bot buy \"Enchanted Shadow Battle Spear\" ?",
            false
        ),
        new Option<bool>(
            "54268",
            "Enchanted ShadowFlame Broadsword",
            "Mode: [select] only\nShould the bot buy \"Enchanted ShadowFlame Broadsword\" ?",
            false
        ),
    };
}
