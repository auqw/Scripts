/*
name: Frozen Queen Merge
description: This bot will farm the items belonging to the selected mode for the Frozen Queen Merge [2507] in /frozenqueen
tags: frozen, queen, merge, frozenqueen, northlands, lightcaster, cap, wings, dawning, paradise
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FrozenQueenMerge
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
        Core.BankingBlackList.AddRange(new[] { "Frozen SpiderSilk", "Ice Vapor" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("frozenqueen", 2507, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Frozen SpiderSilk":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10018, "frozenqueen", "Frostspinner Queen");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Ice Vapor":
                    Core.FarmingLogger(req.Name, quant);
                    Core.KillMonster(
                        "lair",
                        "Enter",
                        "Spawn",
                        "*",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "90946",
            "Northlands LightCaster",
            "Mode: [select] only\nShould the bot buy \"Northlands LightCaster\" ?",
            false
        ),
        new Option<bool>(
            "90947",
            "Northlands LightCaster Hat",
            "Mode: [select] only\nShould the bot buy \"Northlands LightCaster Hat\" ?",
            false
        ),
        new Option<bool>(
            "90948",
            "Northlands LightCaster Cap",
            "Mode: [select] only\nShould the bot buy \"Northlands LightCaster Cap\" ?",
            false
        ),
        new Option<bool>(
            "90949",
            "Northlands LightCaster Wings",
            "Mode: [select] only\nShould the bot buy \"Northlands LightCaster Wings\" ?",
            false
        ),
        new Option<bool>(
            "90950",
            "Dawning Paradise Staff",
            "Mode: [select] only\nShould the bot buy \"Dawning Paradise Staff\" ?",
            false
        ),
    };
}
