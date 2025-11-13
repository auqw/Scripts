/*
name: Nation Merge
description: This bot will farm the items belonging to the selected mode for the Nation Merge [1206] in /shadowblast
tags: nation, merge, shadowblast, polish, pet, soulstealer, horned, void, executioner, blood, star, archfiend
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NationMerge
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

    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static NationLoyaltyRewarded NLR
    {
        get => _NLR ??= new NationLoyaltyRewarded();
        set => _NLR = value;
    }
    private static NationLoyaltyRewarded _NLR;
    private static CoreNSOD NSOD
    {
        get => _NSOD ??= new CoreNSOD();
        set => _NSOD = value;
    }
    private static CoreNSOD _NSOD;

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
                "Diamond Badge of Nulgath",
                "Emblem of Nulgath",
                "Blood Gem of the Archfiend",
                "Totem of Nulgath",
                "Void Aura",
                "Archfiend's Favor",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("shadowblast", 1206, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Known items

                case "Diamond Badge of Nulgath":
                    NLR.FarmQuest(new string[] { req.Name }, quant);
                    break;

                case "Emblem of Nulgath":
                    Nation.EmblemofNulgath(quant);
                    break;

                case "Blood Gem of the Archfiend":
                    Nation.FarmBloodGem(quant);
                    break;

                case "Totem of Nulgath":
                    Nation.FarmTotemofNulgath(quant);
                    break;

                case "Void Aura":
                    NSOD.VoidAuras(quant);
                    break;

                case "Archfiend's Favor":
                    Nation.ApprovalAndFavor(0, quant);
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "33172",
            "Polish Pet",
            "Mode: [select] only\nShould the bot buy \"Polish Pet\" ?",
            false
        ),
        new Option<bool>(
            "33176",
            "Nation Soulstealer",
            "Mode: [select] only\nShould the bot buy \"Nation Soulstealer\" ?",
            false
        ),
        new Option<bool>(
            "33177",
            "Nation SoulStealer Hood",
            "Mode: [select] only\nShould the bot buy \"Nation SoulStealer Hood\" ?",
            false
        ),
        new Option<bool>(
            "33178",
            "Nation SoulStealer Horned Hood",
            "Mode: [select] only\nShould the bot buy \"Nation SoulStealer Horned Hood\" ?",
            false
        ),
        new Option<bool>(
            "33162",
            "Void Executioner",
            "Mode: [select] only\nShould the bot buy \"Void Executioner\" ?",
            false
        ),
        new Option<bool>(
            "67269",
            "Blood Star of the Archfiend",
            "Mode: [select] only\nShould the bot buy \"Blood Star of the Archfiend\" ?",
            false
        ),
    };
}
