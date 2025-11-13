/*
name: Rangda Merge
description: This bot will farm the items belonging to the selected mode for the Rangda Merge [1901] in /rangda
tags: rangda, merge, rangda, gatotkaca, gatot, crown, bearded, sheath, keris, arjunas, bow, mace, wings, warok, warlord, ironbeard, morph, scarlet, singa, barong, waroks, binding, cord, cords, serpent, lash, ponorogo, nyi, roro, kidul, regalia, southern, kings, sea, kiduls, turban, tidebound, maiden, reog, companion, aegis, golden, garuda, sky, guardian, hidden, talon, sovereign, ceremonial
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/IndonesianDay/Rangda.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class RangdaMerge
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

    private static RangdaSeasonal RangdaSeasonal
    {
        get => _RangdaSeasonal ??= new RangdaSeasonal();
        set => _RangdaSeasonal = value;
    }

    private static RangdaSeasonal _RangdaSeasonal;

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
                "Rangda's Mask",
                "Abhorrent Remnant",
                "Batik Fabric",
                "Benang",
                "Elder Warok Morph",
                "Duskwind Warok Visage",
                "Warok's Wicked Snapper",
                "Southern King Hair",
                "Ocean Maiden's Locks",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("rangda"))
            return;

        RangdaSeasonal.StoryLine();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("rangda", 1901, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Rangda's Mask":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Bot.Quests.UpdateQuest(7622);
                    Core.HuntMonster("rangda", "Rangda", req.Name, quant, false, false);
                    break;

                case "Abhorrent Remnant":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("rangda", "Tuyul", req.Name, quant, false, false);
                    break;

                case "Batik Fabric":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10371); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("rangda", "Rangda", req.Name, 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Benang":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10372); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("rangda", "Tuyul", "Tuyul Soul", 9);
                        Core.HuntMonster("rangda", "Leyak", "Leyak Jaw", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Warok's Wicked Snapper":
                case "Duskwind Warok Visage":
                case "Elder Warok Morph":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("rangda", "Rangda", req.Name, quant, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Southern King Hair":
                case "Ocean Maiden’s Locks":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("rangda", "Leyak", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "55784",
            "Gatotkaca",
            "Mode: [select] only\nShould the bot buy \"Gatotkaca\" ?",
            false
        ),
        new Option<bool>(
            "55785",
            "Gatot Crown and Locks",
            "Mode: [select] only\nShould the bot buy \"Gatot Crown and Locks\" ?",
            false
        ),
        new Option<bool>(
            "55786",
            "Gatot Crown",
            "Mode: [select] only\nShould the bot buy \"Gatot Crown\" ?",
            false
        ),
        new Option<bool>(
            "55787",
            "Bearded Gatot Crown",
            "Mode: [select] only\nShould the bot buy \"Bearded Gatot Crown\" ?",
            false
        ),
        new Option<bool>(
            "55795",
            "Gatot Sheath and Keris",
            "Mode: [select] only\nShould the bot buy \"Gatot Sheath and Keris\" ?",
            false
        ),
        new Option<bool>(
            "55791",
            "Arjuna's Bow",
            "Mode: [select] only\nShould the bot buy \"Arjuna's Bow\" ?",
            false
        ),
        new Option<bool>(
            "55794",
            "Gatot Mace",
            "Mode: [select] only\nShould the bot buy \"Gatot Mace\" ?",
            false
        ),
        new Option<bool>(
            "55790",
            "Gatot Wings",
            "Mode: [select] only\nShould the bot buy \"Gatot Wings\" ?",
            false
        ),
        new Option<bool>(
            "94994",
            "Warok Warlord",
            "Mode: [select] only\nShould the bot buy \"Warok Warlord\" ?",
            false
        ),
        new Option<bool>(
            "94996",
            "Ironbeard Warok Morph",
            "Mode: [select] only\nShould the bot buy \"Ironbeard Warok Morph\" ?",
            false
        ),
        new Option<bool>(
            "94997",
            "Scarlet Warok Visage",
            "Mode: [select] only\nShould the bot buy \"Scarlet Warok Visage\" ?",
            false
        ),
        new Option<bool>(
            "95000",
            "Singa Barong Visage",
            "Mode: [select] only\nShould the bot buy \"Singa Barong Visage\" ?",
            false
        ),
        new Option<bool>(
            "95011",
            "Warok's Binding Cord",
            "Mode: [select] only\nShould the bot buy \"Warok's Binding Cord\" ?",
            false
        ),
        new Option<bool>(
            "95012",
            "Warok's Binding Cords",
            "Mode: [select] only\nShould the bot buy \"Warok's Binding Cords\" ?",
            false
        ),
        new Option<bool>(
            "95020",
            "Serpent Lash of Ponorogo",
            "Mode: [select] only\nShould the bot buy \"Serpent Lash of Ponorogo\" ?",
            false
        ),
        new Option<bool>(
            "95057",
            "Nyi Roro Kidul Regalia",
            "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul Regalia\" ?",
            false
        ),
        new Option<bool>(
            "95061",
            "Southern King's Sea Crown",
            "Mode: [select] only\nShould the bot buy \"Southern King's Sea Crown\" ?",
            false
        ),
        new Option<bool>(
            "95062",
            "Nyi Roro Kidul's Sea Crown",
            "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul's Sea Crown\" ?",
            false
        ),
        new Option<bool>(
            "95063",
            "Nyi Roro Kidul's Visage",
            "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul's Visage\" ?",
            false
        ),
        new Option<bool>(
            "95064",
            "Southern Sea King's Visage",
            "Mode: [select] only\nShould the bot buy \"Southern Sea King's Visage\" ?",
            false
        ),
        new Option<bool>(
            "95065",
            "Southern Sea King's Turban",
            "Mode: [select] only\nShould the bot buy \"Southern Sea King's Turban\" ?",
            false
        ),
        new Option<bool>(
            "95056",
            "Tidebound Maiden",
            "Mode: [select] only\nShould the bot buy \"Tidebound Maiden\" ?",
            false
        ),
        new Option<bool>(
            "94995",
            "Reog Companion",
            "Mode: [select] only\nShould the bot buy \"Reog Companion\" ?",
            false
        ),
        new Option<bool>(
            "94983",
            "Aegis of the Golden Garuda",
            "Mode: [select] only\nShould the bot buy \"Aegis of the Golden Garuda\" ?",
            false
        ),
        new Option<bool>(
            "94984",
            "Crown of the Sky Guardian",
            "Mode: [select] only\nShould the bot buy \"Crown of the Sky Guardian\" ?",
            false
        ),
        new Option<bool>(
            "94985",
            "Hood of the Hidden Talon",
            "Mode: [select] only\nShould the bot buy \"Hood of the Hidden Talon\" ?",
            false
        ),
        new Option<bool>(
            "94987",
            "Sovereign Ceremonial Mace",
            "Mode: [select] only\nShould the bot buy \"Sovereign Ceremonial Mace\" ?",
            false
        ),
    };
}
