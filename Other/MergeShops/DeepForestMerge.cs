/*
name: DeepForest Merge
description: This bot will farm the items belonging to the selected mode for the DeepForest Merge [1999] in /deepforest
tags: deepforest, merge, deepforest, chaotic, monsterhunter, monsterhunters, supreme, arcane, chaos, polished, dragon, necrotic, hanzamune
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DeepForestMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreQOM QOM
    {
        get => _QOM ??= new CoreQOM();
        set => _QOM = value;
    }
    private static CoreQOM _QOM;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Deep Forest Sap", "Fallen MonsterHunter", "Fallen MonsterHunter Cape", "Fallen MonsterHunter Helm", "The Supreme Arcane Staff", "Dragon Sword of Chaos", "Necrotic Blade of Chaos", "Chaotic Hanzamune", "Fallen MonsterHunter Sword" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        QOM.TheBook();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("deepforest", 1999, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Necrotic Blade of Chaos":
                case "Dragon Sword of Chaos":
                    Adv.BuyItem("castleundead", 45, req.Name);
                    break;

                case "Chaotic Hanzamune":
                    Core.KillKitsune(req.Name, req.Quantity, req.Temp);
                    break;

                case "Deep Forest Sap":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(8081);  
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster("deepforest", "r2", "Left", "Deep Truffle");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Fallen MonsterHunter":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DeepForest", "Aberrant Horror", req.Name, isTemp: false);
                    break;

                case "Fallen MonsterHunter Cape":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DeepForest", "Aberrant Horror", req.Name, isTemp: false);
                    break;

                case "Fallen MonsterHunter Helm":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DeepForest", "Aberrant Horror", req.Name, isTemp: false);
                    break;

                case "The Supreme Arcane Staff":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ledgermayne", "Ledgermayne", "The Supreme Arcane Staff", 1, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Fallen MonsterHunter Sword":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DeepForest", "Aberrant Horror", req.Name, isTemp: false);
                    break;

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("61039", "Chaotic MonsterHunter", "Mode: [select] only\nShould the bot buy \"Chaotic MonsterHunter\" ?", false),
        new Option<bool>("61041", "Chaotic MonsterHunter's Cape", "Mode: [select] only\nShould the bot buy \"Chaotic MonsterHunter's Cape\" ?", false),
        new Option<bool>("61040", "Chaotic MonsterHunter's Helm", "Mode: [select] only\nShould the bot buy \"Chaotic MonsterHunter's Helm\" ?", false),
        new Option<bool>("60989", "Supreme Arcane Staff of Chaos", "Mode: [select] only\nShould the bot buy \"Supreme Arcane Staff of Chaos\" ?", false),
        new Option<bool>("60985", "Polished Dragon Sword of Chaos", "Mode: [select] only\nShould the bot buy \"Polished Dragon Sword of Chaos\" ?", false),
        new Option<bool>("60987", "Polished Necrotic Blade of Chaos", "Mode: [select] only\nShould the bot buy \"Polished Necrotic Blade of Chaos\" ?", false),
        new Option<bool>("60986", "Polished Chaotic Hanzamune", "Mode: [select] only\nShould the bot buy \"Polished Chaotic Hanzamune\" ?", false),
        new Option<bool>("61042", "Chaotic MonsterHunter's Sword", "Mode: [select] only\nShould the bot buy \"Chaotic MonsterHunter's Sword\" ?", false),
   };
}
