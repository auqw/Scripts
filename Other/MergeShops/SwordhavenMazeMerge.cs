/*
name: Swordhaven Maze Merge
description: This bot will farm the items belonging to the selected mode for the Swordhaven Maze Merge [2740] in /swordhavenmaze
tags: swordhaven, maze, merge, swordhavenmaze, extinction, artist, aficionado, house, volkov, uniform, ceremonial, ruby, rapiers, warrior, morph, cap, , jubilant, artists, shadow, artistas, instrument, brush, rapier
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SwordhavenMazeMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
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
        Core.BankingBlackList.AddRange(new[] { "Aspiring Extinction Aficionado", "Extinction Artist's Pigment", "Feral Boar's Hide", "Ghost's Garment", "Prisoner's Chain", "Extinction Artist's Hair", "Extinction Artista's Locks", "90 Degree Rotator", "Instrument of Destruction" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("swordhavenmaze", 2740, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
        #endregion

                case "Feral Boar's Hide":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("swordhavenmaze", "Feral Boar", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Ghost's Garment":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);

                    Core.HuntMonster("swordhavenmaze", "Noble Ghost", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Prisoner's Chain":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);

                    Core.HuntMonster("swordhavenmaze", "Undead Prisoner", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "90 Degree Rotator":
                case "Extinction Artist's Pigment":
                case "Aspiring Extinction Aficionado":
                case "Extinction Artist's Hair":
                case "Extinction Artista's Locks":
                case "Instrument of Destruction":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("swordhavenmaze", "Aficionado Cosima", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("102018", "Extinction Artist Aficionado", "Mode: [select] only\nShould the bot buy \"Extinction Artist Aficionado\" ?", false),
        new Option<bool>("102028", "House Volkov Uniform", "Mode: [select] only\nShould the bot buy \"House Volkov Uniform\" ?", false),
        new Option<bool>("102029", "House Volkov Ceremonial Uniform", "Mode: [select] only\nShould the bot buy \"House Volkov Ceremonial Uniform\" ?", false),
        new Option<bool>("102035", "Ruby Rapiers of Volkov", "Mode: [select] only\nShould the bot buy \"Ruby Rapiers of Volkov\" ?", false),
        new Option<bool>("102030", "Volkov Warrior Morph", "Mode: [select] only\nShould the bot buy \"Volkov Warrior Morph\" ?", false),
        new Option<bool>("102031", "Volkov Warrior Visage", "Mode: [select] only\nShould the bot buy \"Volkov Warrior Visage\" ?", false),
        new Option<bool>("102032", "Volkov Warrior Cap", "Mode: [select] only\nShould the bot buy \"Volkov Warrior Cap\" ?", false),
        new Option<bool>("102033", "Volkov Warrior Cap + Locks", "Mode: [select] only\nShould the bot buy \"Volkov Warrior Cap + Locks\" ?", false),
        new Option<bool>("102019", "Jubilant Artist's Mask", "Mode: [select] only\nShould the bot buy \"Jubilant Artist's Mask\" ?", false),
        new Option<bool>("102020", "Jubilant Artist's Shadow Mask", "Mode: [select] only\nShould the bot buy \"Jubilant Artist's Shadow Mask\" ?", false),
        new Option<bool>("102022", "Jubilant Artista's Mask", "Mode: [select] only\nShould the bot buy \"Jubilant Artista's Mask\" ?", false),
        new Option<bool>("102023", "Jubilant Artista's Shadow Mask", "Mode: [select] only\nShould the bot buy \"Jubilant Artista's Shadow Mask\" ?", false),
        new Option<bool>("102027", "Instrument of Extinction", "Mode: [select] only\nShould the bot buy \"Instrument of Extinction\" ?", false),
        new Option<bool>("102025", "Extinction Artist's Brush", "Mode: [select] only\nShould the bot buy \"Extinction Artist's Brush\" ?", false),
        new Option<bool>("102034", "Ruby Rapier of Volkov", "Mode: [select] only\nShould the bot buy \"Ruby Rapier of Volkov\" ?", false),
   ];
}
