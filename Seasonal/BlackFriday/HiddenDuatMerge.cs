/*
name: HiddenDuat Merge
description: This bot will farm the items belonging to the selected mode for the HiddenDuat Merge [2501] in /hiddenduat
tags: hiddenduat, merge, hiddenduat, apophis, medjai, bangs, morph, horned, dark, umbral, scythe, aaru, bow
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowofDoom/CoreShadowofDoom.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class HiddenDuatMerge
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

    private static CoreShadowofDoom CSoD
    {
        get => _CSoD ??= new CoreShadowofDoom();
        set => _CSoD = value;
    }
    private static CoreShadowofDoom _CSoD;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Crown of Chaos" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CSoD.HiddenDuat();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("hiddenduat", 2501, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Crown of Chaos":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9965);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "hiddenduat",
                            "Anubian Overseer",
                            "Duanmutef Glyph",
                            6,
                            log: false
                        );
                        Core.HuntMonster(
                            "hiddenduat",
                            "Pharaoh Neith",
                            "Neith's Uraeus",
                            log: false
                        );
                        Core.HuntMonster(
                            "hiddenduat",
                            "Umbral Chaos",
                            "Apophis' Violet Favor",
                            log: false
                        );
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
            "88090",
            "Apophis Medjai",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai\" ?",
            false
        ),
        new Option<bool>(
            "88091",
            "Apophis Medjai Hair",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Hair\" ?",
            false
        ),
        new Option<bool>(
            "88092",
            "Apophis Medjai Locks",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Locks\" ?",
            false
        ),
        new Option<bool>(
            "88093",
            "Apophis Medjai Bangs",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Bangs\" ?",
            false
        ),
        new Option<bool>(
            "88094",
            "Apophis Medjai Morph",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Morph\" ?",
            false
        ),
        new Option<bool>(
            "88095",
            "Apophis Medjai Visage",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Visage\" ?",
            false
        ),
        new Option<bool>(
            "88096",
            "Apophis Medjai Bangs Visage",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Bangs Visage\" ?",
            false
        ),
        new Option<bool>(
            "88097",
            "Apophis Medjai Horned Morph",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Horned Morph\" ?",
            false
        ),
        new Option<bool>(
            "88098",
            "Apophis Medjai Horned Visage",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Horned Visage\" ?",
            false
        ),
        new Option<bool>(
            "88099",
            "Apophis Medjai Helm",
            "Mode: [select] only\nShould the bot buy \"Apophis Medjai Helm\" ?",
            false
        ),
        new Option<bool>(
            "88100",
            "Dark Medjai Cape",
            "Mode: [select] only\nShould the bot buy \"Dark Medjai Cape\" ?",
            false
        ),
        new Option<bool>(
            "88103",
            "Umbral Scythe of Aaru",
            "Mode: [select] only\nShould the bot buy \"Umbral Scythe of Aaru\" ?",
            false
        ),
        new Option<bool>(
            "88104",
            "Umbral Bow of Aaru",
            "Mode: [select] only\nShould the bot buy \"Umbral Bow of Aaru\" ?",
            false
        ),
    };
}
