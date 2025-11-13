/*
name: Argentos Merge
description: This bot will farm the items belonging to the selected mode for the Argentos Merge [2517] in /frozenbalemorale
tags: argentos, merge, frozenbalemorale, sterling, dragonlord, noble, dragonberserker, dragonbulwark, armet, plumed, fury, plated, rage, dragons, blessing, dragonknight, scythe, drake, halberd, masterblade
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArgentosMerge
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
        Core.BankingBlackList.AddRange(new[] { "Sterling Silver" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "frozenbalemorale",
            2517,
            findIngredients,
            buyOnlyThis,
            buyMode: buyMode
        );

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

                case "Sterling Silver":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9991, "frozenbalemorale", "Kall Haxa");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "90081",
            "Sterling Dragonlord",
            "Mode: [select] only\nShould the bot buy \"Sterling Dragonlord\" ?",
            false
        ),
        new Option<bool>(
            "90082",
            "Noble Sterling Dragonlord",
            "Mode: [select] only\nShould the bot buy \"Noble Sterling Dragonlord\" ?",
            false
        ),
        new Option<bool>(
            "90083",
            "Sterling DragonBerserker",
            "Mode: [select] only\nShould the bot buy \"Sterling DragonBerserker\" ?",
            false
        ),
        new Option<bool>(
            "90085",
            "Sterling DragonBulwark",
            "Mode: [select] only\nShould the bot buy \"Sterling DragonBulwark\" ?",
            false
        ),
        new Option<bool>(
            "90087",
            "Sterling Noble Armet",
            "Mode: [select] only\nShould the bot buy \"Sterling Noble Armet\" ?",
            false
        ),
        new Option<bool>(
            "90088",
            "Sterling Plumed Armet",
            "Mode: [select] only\nShould the bot buy \"Sterling Plumed Armet\" ?",
            false
        ),
        new Option<bool>(
            "90089",
            "Sterling DragonBerserker Helm",
            "Mode: [select] only\nShould the bot buy \"Sterling DragonBerserker Helm\" ?",
            false
        ),
        new Option<bool>(
            "90090",
            "Noble DragonBerserker Helm",
            "Mode: [select] only\nShould the bot buy \"Noble DragonBerserker Helm\" ?",
            false
        ),
        new Option<bool>(
            "90091",
            "DragonBerserker Fury Helm",
            "Mode: [select] only\nShould the bot buy \"DragonBerserker Fury Helm\" ?",
            false
        ),
        new Option<bool>(
            "90092",
            "DragonBerserker Plated Helm",
            "Mode: [select] only\nShould the bot buy \"DragonBerserker Plated Helm\" ?",
            false
        ),
        new Option<bool>(
            "90093",
            "Plumed DragonBerserker Helm",
            "Mode: [select] only\nShould the bot buy \"Plumed DragonBerserker Helm\" ?",
            false
        ),
        new Option<bool>(
            "90094",
            "DragonBerserker Rage Helm",
            "Mode: [select] only\nShould the bot buy \"DragonBerserker Rage Helm\" ?",
            false
        ),
        new Option<bool>(
            "90096",
            "Sterling Dragon's Blessing",
            "Mode: [select] only\nShould the bot buy \"Sterling Dragon's Blessing\" ?",
            false
        ),
        new Option<bool>(
            "90102",
            "Sterling Dragonknight Scythe",
            "Mode: [select] only\nShould the bot buy \"Sterling Dragonknight Scythe\" ?",
            false
        ),
        new Option<bool>(
            "90103",
            "Sterling Drake Halberd",
            "Mode: [select] only\nShould the bot buy \"Sterling Drake Halberd\" ?",
            false
        ),
        new Option<bool>(
            "90104",
            "Sterling Dragonlord MasterBlade",
            "Mode: [select] only\nShould the bot buy \"Sterling Dragonlord MasterBlade\" ?",
            false
        ),
    };
}
