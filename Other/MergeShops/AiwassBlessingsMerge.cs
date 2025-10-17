/*
name: Aiwass Blessings Merge
description: This bot will farm the items belonging to the selected mode for the Aiwass Blessings Merge [2619] in /sanctuaryaiwass
tags: aiwass, blessings, merge, sanctuaryaiwass, adamas, tenebris, warrior, rex, morph, horns, farblight, spines, ex, unbreakable, fist, fists, enchanted
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AiwassBlessingsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }
    private static CoreAOR _AOR;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] {
            "Yew Ember", "Aiwass Diamond", "Drow Silver", "Adamas Tenebris Drow",
            "Adamas Tenebris Hair", "Adamas Tenebris Locks", "Farblight Fur",
            "Enchanted Tenebris Drow", "Enchanted Tenebris Hair", "Enchanted Tenebris Locks"
        });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.SanctuaryAiwass();
        Adv.StartBuyAllMerge("sanctuaryaiwass", 2619, findIngredients, buyOnlyThis, buyMode: buyMode);

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
                #region Gold
                case "Gold Voucher 100K":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
                #endregion

                #region Yew Ember
                case "Yew Ember":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("thelimacity", "Maleno Match", req.Name, quant, false);
                    break;
                #endregion

                #region Aiwass Diamond
                case "Aiwass Diamond":
                    int questID = Core.IsMember ? 10387 : 10385;
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(questID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Sal Alembroth", 1, false);
                        Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Milk of Sulfur", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Aeon Dream", 1, false);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion

                #region Drow Silver
                case "Drow Silver":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("thelimacity", "Drow Soldier", req.Name, quant, false);
                    break;
                #endregion

                #region Drow, Hair, Locks, Farblight, Enchanted
                case "Adamas Tenebris Drow":
                case "Adamas Tenebris Hair":
                case "Adamas Tenebris Locks":
                case "Enchanted Tenebris Drow":
                case "Enchanted Tenebris Hair":
                case "Enchanted Tenebris Locks":
                case "Farblight Fur":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Adamas Tenebris Drow", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Adamas Tenebris Hair", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Adamas Tenebris Locks", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Enchanted Tenebris Drow", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Enchanted Tenebris Hair", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Enchanted Tenebris Locks", 1, false);
                        Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Farblight Fur", 1, false);
                    }
                    break;
                #endregion

                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("95216", "Adamas Tenebris Warrior", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Warrior\" ?", false),
        new Option<bool>("95217", "Adamas Tenebris Rex", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Rex\" ?", false),
        new Option<bool>("95220", "Adamas Tenebris Morph", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Morph\" ?", false),
        new Option<bool>("95221", "Adamas Tenebris Visage", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Visage\" ?", false),
        new Option<bool>("95222", "Adamas Tenebris Helm", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Helm\" ?", false),
        new Option<bool>("95223", "Adamas Tenebris Horns", "Mode: [select] only\nShould the bot buy \"Adamas Tenebris Horns\" ?", false),
        new Option<bool>("95225", "Farblight Spines", "Mode: [select] only\nShould the bot buy \"Farblight Spines\" ?", false),
        new Option<bool>("95226", "Ex Tenebris Blade", "Mode: [select] only\nShould the bot buy \"Ex Tenebris Blade\" ?", false),
        new Option<bool>("95227", "Dual Ex Tenebris", "Mode: [select] only\nShould the bot buy \"Dual Ex Tenebris\" ?", false),
        new Option<bool>("95228", "Unbreakable Adamas Fist", "Mode: [select] only\nShould the bot buy \"Unbreakable Adamas Fist\" ?", false),
        new Option<bool>("95229", "Unbreakable Adamas Fists", "Mode: [select] only\nShould the bot buy \"Unbreakable Adamas Fists\" ?", false),
        new Option<bool>("95246", "Enchanted Tenebris Warrior", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Warrior\" ?", false),
        new Option<bool>("95247", "Enchanted Tenebris Rex", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Rex\" ?", false),
        new Option<bool>("95250", "Enchanted Tenebris Morph", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Morph\" ?", false),
        new Option<bool>("95251", "Enchanted Tenebris Visage", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Visage\" ?", false),
        new Option<bool>("95252", "Enchanted Tenebris Helm", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Helm\" ?", false),
        new Option<bool>("95253", "Enchanted Tenebris Horns", "Mode: [select] only\nShould the bot buy \"Enchanted Tenebris Horns\" ?", false),
        new Option<bool>("95256", "Enchanted Ex Tenebris", "Mode: [select] only\nShould the bot buy \"Enchanted Ex Tenebris\" ?", false),
        new Option<bool>("95257", "Enchanted Dual Ex Tenebris", "Mode: [select] only\nShould the bot buy \"Enchanted Dual Ex Tenebris\" ?", false),
    };
}
