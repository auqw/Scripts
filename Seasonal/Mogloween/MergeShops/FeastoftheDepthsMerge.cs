/*
name: Feast of the Depths Merge
description: This bot will farm the items belonging to the selected mode for the Feast of the Depths Merge [2633] in /elodeatemple
tags: feast, of, the, depths, merge, elodeatemple, gold, voucher, k, pumpkin, warlock, witch, ponytail, long, twintails, twintail, mogloween, forest, mangelwurzel, broom, pet, standing, erudite, plague, doctor, faustian, moustache, cloak, crimson, darkling, lamp, lamps, iron, doctors, cane, forged
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FeastoftheDepthsMerge
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
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Depths Scale" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CoreMogloween.ElodeaTemple();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("elodeatemple", 2633, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Depths Scale":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            Core.IsMember ? 10480 : 10479,
                            "elodeatemple",
                            "Child of the Depths"
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "89695",
            "Pumpkin Warlock",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock\" ?",
            false
        ),
        new Option<bool>(
            "89696",
            "Pumpkin Warlock Hair",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Hair\" ?",
            false
        ),
        new Option<bool>(
            "89697",
            "Pumpkin Witch Locks",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Locks\" ?",
            false
        ),
        new Option<bool>(
            "89698",
            "Pumpkin Witch Ponytail",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Ponytail\" ?",
            false
        ),
        new Option<bool>(
            "89699",
            "Pumpkin Warlock Long Hair",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Long Hair\" ?",
            false
        ),
        new Option<bool>(
            "89700",
            "Pumpkin Witch Twintails",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Twintails\" ?",
            false
        ),
        new Option<bool>(
            "89701",
            "Pumpkin Warlock Hat",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Hat\" ?",
            false
        ),
        new Option<bool>(
            "89702",
            "Pumpkin Witch Hat",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Hat\" ?",
            false
        ),
        new Option<bool>(
            "89703",
            "Pumpkin Witch Ponytail Hat",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Ponytail Hat\" ?",
            false
        ),
        new Option<bool>(
            "89704",
            "Pumpkin Witch Twintail Hat",
            "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Twintail Hat\" ?",
            false
        ),
        new Option<bool>(
            "89707",
            "Mogloween Forest",
            "Mode: [select] only\nShould the bot buy \"Mogloween Forest\" ?",
            false
        ),
        new Option<bool>(
            "89708",
            "Mangelwurzel Broom Pet",
            "Mode: [select] only\nShould the bot buy \"Mangelwurzel Broom Pet\" ?",
            false
        ),
        new Option<bool>(
            "89709",
            "Mangelwurzel Broom",
            "Mode: [select] only\nShould the bot buy \"Mangelwurzel Broom\" ?",
            false
        ),
        new Option<bool>(
            "89711",
            "Standing Mangelwurzel Broom",
            "Mode: [select] only\nShould the bot buy \"Standing Mangelwurzel Broom\" ?",
            false
        ),
        new Option<bool>(
            "95578",
            "Erudite Plague Doctor",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor\" ?",
            false
        ),
        new Option<bool>(
            "95579",
            "Faustian Plague Doctor",
            "Mode: [select] only\nShould the bot buy \"Faustian Plague Doctor\" ?",
            false
        ),
        new Option<bool>(
            "95580",
            "Erudite Plague Doctor Hair",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Hair\" ?",
            false
        ),
        new Option<bool>(
            "95581",
            "Erudite Plague Doctor Locks",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Locks\" ?",
            false
        ),
        new Option<bool>(
            "95582",
            "Erudite Plague Doctor Mask",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Mask\" ?",
            false
        ),
        new Option<bool>(
            "95583",
            "Erudite Plague Doctor Moustache",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Moustache\" ?",
            false
        ),
        new Option<bool>(
            "95584",
            "Erudite Plague Doctor Cloak",
            "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Cloak\" ?",
            false
        ),
        new Option<bool>(
            "95585",
            "Crimson Darkling Lamp",
            "Mode: [select] only\nShould the bot buy \"Crimson Darkling Lamp\" ?",
            false
        ),
        new Option<bool>(
            "95586",
            "Crimson Darkling Lamps",
            "Mode: [select] only\nShould the bot buy \"Crimson Darkling Lamps\" ?",
            false
        ),
        new Option<bool>(
            "95587",
            "Iron Doctor's Cane",
            "Mode: [select] only\nShould the bot buy \"Iron Doctor's Cane\" ?",
            false
        ),
        new Option<bool>(
            "95588",
            "Forged Doctor's Cane",
            "Mode: [select] only\nShould the bot buy \"Forged Doctor's Cane\" ?",
            false
        ),
    };
}
