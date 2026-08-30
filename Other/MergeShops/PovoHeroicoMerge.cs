/*
name: Povo Heroico Merge
description: This bot will farm the items belonging to the selected mode for the Povo Heroico Merge [2620] in /povoheroico
tags: povo, heroico, merge, povoheroico, brado, encantado, capuz, face, encantada, símbolo, da, ordem, do, progresso, brabo, brabos, encantados
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PovoHeroicoMerge
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
            new[]
            {
                "Moeda Real",
                "Brado Retumbante",
                "Capuz Retumbante",
                "Face Retumbante",
                "Símbolo da Ordem",
                "Símbolo do Progresso",
                "Brabo Retumbante",
                "Brabos Retumbantes",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("povoheroico", 2620, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                // All of these drop from the monster "Brado Retumbante" in povoheroico (cell: r2, pad: Bottom)
                case "Moeda Real":
                case "Brabo Retumbante":
                case "Brabos Retumbantes":
                case "Brado Retumbante":
                case "Capuz Retumbante":
                case "Face Retumbante":
                case "Símbolo da Ordem":
                case "Símbolo do Progresso":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    Core.KillMonster(
                        "povoheroico",
                        "r2",
                        "Bottom",
                        "Brado Retumbante",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "95277",
            "Brado Encantado",
            "Mode: [select] only\nShould the bot buy \"Brado Encantado\" ?",
            false
        ),
        new Option<bool>(
            "95278",
            "Capuz Encantado",
            "Mode: [select] only\nShould the bot buy \"Capuz Encantado\" ?",
            false
        ),
        new Option<bool>(
            "95279",
            "Face Encantada",
            "Mode: [select] only\nShould the bot buy \"Face Encantada\" ?",
            false
        ),
        new Option<bool>(
            "95280",
            "Símbolo da Ordem Encantado",
            "Mode: [select] only\nShould the bot buy \"Símbolo da Ordem Encantado\" ?",
            false
        ),
        new Option<bool>(
            "95281",
            "Símbolo do Progresso Encantado",
            "Mode: [select] only\nShould the bot buy \"Símbolo do Progresso Encantado\" ?",
            false
        ),
        new Option<bool>(
            "95282",
            "Brabo Encantado",
            "Mode: [select] only\nShould the bot buy \"Brabo Encantado\" ?",
            false
        ),
        new Option<bool>(
            "95283",
            "Brabos Encantados",
            "Mode: [select] only\nShould the bot buy \"Brabos Encantados\" ?",
            false
        ),
    };
}
