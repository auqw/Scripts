/*
name: Rangda Merge
description: This bot will farm the items belonging to the selected mode for the Rangda Merge [1901] in /rangda
tags: rangda, merge, sableng, gendeng, archipelago, marshland, gatotkaca, gatot, crown, bearded, sheath, keris, arjunas, bow, mace, wings, warok, warlord, ironbeard, morph, scarlet, singa, barong, waroks, binding, cord, cords, serpent, lash, ponorogo, nyi, roro, kidul, regalia, southern, kings, sea, kiduls, turban, tidebound, maiden, reog, companion, aegis, golden, garuda, sky, guardian, hidden, talon, sovereign, ceremonial
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
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
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
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Abhorrent Remnant",
            "Batik Fabric",
            "Benang",
            "Duskwind Warok Visage",
            "Elder Warok Morph",
            "Gendeng's Wild Axe",
            "Gendeng's Wild Hammer",
            "Ocean Maiden's Locks",
            "Rangda's Mask",
            "Southern King Hair",
            "Warok's Wicked Snapper",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("rangda"))
            return;

        RangdaSeasonal.StoryLine();
        Adv.StartBuyAllMerge("rangda", 1901, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Abhorrent Remnant":
                case "Ocean Maiden's Locks":
                case "Southern King Hair":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("rangda", "Tuyul", req.Name, quant, req.Temp);
                    break;
                case "Batik Fabric":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10371);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("rangda", "Rangda", "Rangda Rematched", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Benang":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10372);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("rangda", "Tuyul", "Tuyul Soul", 9);
                        Core.HuntMonster("rangda", "Leyak", "Leyak Jaw", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Duskwind Warok Visage":
                case "Elder Warok Morph":
                case "Gendeng's Wild Axe":
                case "Gendeng's Wild Hammer":
                case "Rangda's Mask":
                case "Warok's Wicked Snapper":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("rangda", "Rangda", req.Name, quant, req.Temp);
                    break;
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}.", messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("55784", "Gatotkaca", "Mode: [select] only\nShould the bot buy \"Gatotkaca\" ?", false),
        new Option<bool>("94994", "Warok Warlord", "Mode: [select] only\nShould the bot buy \"Warok Warlord\" ?", false),
        new Option<bool>("95057", "Nyi Roro Kidul Regalia", "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul Regalia\" ?", false),
        new Option<bool>("95056", "Tidebound Maiden", "Mode: [select] only\nShould the bot buy \"Tidebound Maiden\" ?", false),
        new Option<bool>("94995", "Reog Companion", "Mode: [select] only\nShould the bot buy \"Reog Companion\" ?", false),
        new Option<bool>("94983", "Aegis of the Golden Garuda", "Mode: [select] only\nShould the bot buy \"Aegis of the Golden Garuda\" ?", false),
        new Option<bool>("102590", "Sableng Archipelago Druid", "Mode: [select] only\nShould the bot buy \"Sableng Archipelago Druid\" ?", false),
        new Option<bool>("102711", "Sableng Marshland Druid", "Mode: [select] only\nShould the bot buy \"Sableng Marshland Druid\" ?", false),
        new Option<bool>("55791", "Arjuna's Bow", "Mode: [select] only\nShould the bot buy \"Arjuna's Bow\" ?", false),
        new Option<bool>("55790", "Gatot Wings", "Mode: [select] only\nShould the bot buy \"Gatot Wings\" ?", false),
        new Option<bool>("95012", "Warok's Binding Cords", "Mode: [select] only\nShould the bot buy \"Warok's Binding Cords\" ?", false),
        new Option<bool>("95020", "Serpent Lash of Ponorogo", "Mode: [select] only\nShould the bot buy \"Serpent Lash of Ponorogo\" ?", false),
        new Option<bool>("55795", "Gatot Sheath and Keris", "Mode: [select] only\nShould the bot buy \"Gatot Sheath and Keris\" ?", false),
        new Option<bool>("102600", "Gendeng's Wild Armaments", "Mode: [select] only\nShould the bot buy \"Gendeng's Wild Armaments\" ?", false),
        new Option<bool>("102712", "Sableng Marshland Hair", "Mode: [select] only\nShould the bot buy \"Sableng Marshland Hair\" ?", false),
        new Option<bool>("102713", "Sableng Marshland Locks", "Mode: [select] only\nShould the bot buy \"Sableng Marshland Locks\" ?", false),
        new Option<bool>("102591", "Sableng Archipelago Hair", "Mode: [select] only\nShould the bot buy \"Sableng Archipelago Hair\" ?", false),
        new Option<bool>("102592", "Sableng Archipelago Locks", "Mode: [select] only\nShould the bot buy \"Sableng Archipelago Locks\" ?", false),
        new Option<bool>("102593", "Sableng Archipelago Blindfold", "Mode: [select] only\nShould the bot buy \"Sableng Archipelago Blindfold\" ?", false),
        new Option<bool>("102594", "Sableng Archipelago Blindfold Locks", "Mode: [select] only\nShould the bot buy \"Sableng Archipelago Blindfold Locks\" ?", false),
        new Option<bool>("94984", "Crown of the Sky Guardian", "Mode: [select] only\nShould the bot buy \"Crown of the Sky Guardian\" ?", false),
        new Option<bool>("94985", "Hood of the Hidden Talon", "Mode: [select] only\nShould the bot buy \"Hood of the Hidden Talon\" ?", false),
        new Option<bool>("55785", "Gatot Crown and Locks", "Mode: [select] only\nShould the bot buy \"Gatot Crown and Locks\" ?", false),
        new Option<bool>("55786", "Gatot Crown", "Mode: [select] only\nShould the bot buy \"Gatot Crown\" ?", false),
        new Option<bool>("55787", "Bearded Gatot Crown", "Mode: [select] only\nShould the bot buy \"Bearded Gatot Crown\" ?", false),
        new Option<bool>("95061", "Southern King's Sea Crown", "Mode: [select] only\nShould the bot buy \"Southern King's Sea Crown\" ?", false),
        new Option<bool>("95062", "Nyi Roro Kidul's Sea Crown", "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul's Sea Crown\" ?", false),
        new Option<bool>("95063", "Nyi Roro Kidul's Visage", "Mode: [select] only\nShould the bot buy \"Nyi Roro Kidul's Visage\" ?", false),
        new Option<bool>("95064", "Southern Sea King's Visage", "Mode: [select] only\nShould the bot buy \"Southern Sea King's Visage\" ?", false),
        new Option<bool>("95065", "Southern Sea King's Turban", "Mode: [select] only\nShould the bot buy \"Southern Sea King's Turban\" ?", false),
        new Option<bool>("94996", "Ironbeard Warok Morph", "Mode: [select] only\nShould the bot buy \"Ironbeard Warok Morph\" ?", false),
        new Option<bool>("94997", "Scarlet Warok Visage", "Mode: [select] only\nShould the bot buy \"Scarlet Warok Visage\" ?", false),
        new Option<bool>("95000", "Singa Barong Visage", "Mode: [select] only\nShould the bot buy \"Singa Barong Visage\" ?", false),
        new Option<bool>("95011", "Warok's Binding Cord", "Mode: [select] only\nShould the bot buy \"Warok's Binding Cord\" ?", false),
        new Option<bool>("55794", "Gatot Mace", "Mode: [select] only\nShould the bot buy \"Gatot Mace\" ?", false),
        new Option<bool>("94987", "Sovereign Ceremonial Mace", "Mode: [select] only\nShould the bot buy \"Sovereign Ceremonial Mace\" ?", false),
    ];
}
