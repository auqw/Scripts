/*
name: 17th Birthday Castle Party Merge
description: This bot will farm the items belonging to the selected mode for the 17th Birthday Castle Party Merge [2627] in /castleparty
tags: 17th, birthday, castle, party, merge, castleparty, arcane, guardian, morph, founder, spectacles, evolution, asclepius, ether, creation, spear, royal, fortune, greatsword, greatswords, bejeweled, bounty, bow, warden, hours, scarf, cowl, shadowbound, magus, cloak, dominating, shadowbinder, sigil, gold, jamboree, top, jubilee, triumph, radiant, euphoria
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CastlePartyMerge
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
        Core.BankingBlackList.AddRange(new[] { "Mana Orb", "Gilded Gem", "Noble Ether Staff", "Mana Creation Orb", "Gleaming Ore", "Royal Fortune Sword", "Royal Fortune Swords", "Iota of Eternity", "Flux Sigil", "Darkness Rune", "Undying Essence", "Shadowbound Magus Cloak", "Dark Descent Rune", "Dominating Shadowbinder", "Dark Descent Sigil", "Giftbox Ribbon", "Golden Euphoria Blade" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("castleparty", 2627, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Items not setup

                case "Mana Orb":
                case "Mana Creation Orb":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Legion Partycrasher", req.Name, quant, req.Temp);
                    break;

                case "Gold Voucher 25k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;


                case "Gilded Gem":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Noxus' Gift", req.Name, quant, req.Temp);
                    break;


                case "Noble Ether Staff":
                case "Gleaming Ore":
                case "Royal Fortune Sword":
                case "Royal Fortune Swords":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Treasure Chest", req.Name, quant, req.Temp);
                    break;



                case "Iota of Eternity":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Kathool's Gift", req.Name, quant, req.Temp, false);
                    break;


                case "Flux Sigil":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Drakath's Gift", req.Name, quant, req.Temp, false);
                    break;


                case "Darkness Rune":
                case "Shadowbound Magus Cloak":
                case "Dark Descent Rune":
                case "Dominating Shadowbinder":
                case "Dark Descent Sigil":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Nulgath's Gift", req.Name, quant, req.Temp);
                    break;


                case "Undying Essence":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Sally's Gift", req.Name, quant, req.Temp);
                    break;


                case "Giftbox Ribbon":
                case "Golden Euphoria Blade":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Lost Giftbox", req.Name, quant, req.Temp);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("96136", "Arcane Guardian", "Mode: [select] only\nShould the bot buy \"Arcane Guardian\" ?", false),
        new Option<bool>("96137", "Arcane Guardian Morph", "Mode: [select] only\nShould the bot buy \"Arcane Guardian Morph\" ?", false),
        new Option<bool>("96138", "Arcane Guardian Visage", "Mode: [select] only\nShould the bot buy \"Arcane Guardian Visage\" ?", false),
        new Option<bool>("96139", "Arcane Founder Morph", "Mode: [select] only\nShould the bot buy \"Arcane Founder Morph\" ?", false),
        new Option<bool>("96140", "Arcane Founder Visage", "Mode: [select] only\nShould the bot buy \"Arcane Founder Visage\" ?", false),
        new Option<bool>("96141", "Arcane Spectacles Morph", "Mode: [select] only\nShould the bot buy \"Arcane Spectacles Morph\" ?", false),
        new Option<bool>("96142", "Arcane Spectacles Visage", "Mode: [select] only\nShould the bot buy \"Arcane Spectacles Visage\" ?", false),
        new Option<bool>("96143", "Arcane Founder Hair", "Mode: [select] only\nShould the bot buy \"Arcane Founder Hair\" ?", false),
        new Option<bool>("96144", "Arcane Founder Locks", "Mode: [select] only\nShould the bot buy \"Arcane Founder Locks\" ?", false),
        new Option<bool>("96147", "Arcane Evolution Blade", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Blade\" ?", false),
        new Option<bool>("96148", "Arcane Evolution Blades", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Blades\" ?", false),
        new Option<bool>("96150", "Asclepius Ether Staff", "Mode: [select] only\nShould the bot buy \"Asclepius Ether Staff\" ?", false),
        new Option<bool>("96151", "Asclepius Creation Staff", "Mode: [select] only\nShould the bot buy \"Asclepius Creation Staff\" ?", false),
        new Option<bool>("96152", "Arcane Evolution Spear", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Spear\" ?", false),
        new Option<bool>("95349", "Royal Fortune Greatsword", "Mode: [select] only\nShould the bot buy \"Royal Fortune Greatsword\" ?", false),
        new Option<bool>("95350", "Royal Fortune Greatswords", "Mode: [select] only\nShould the bot buy \"Royal Fortune Greatswords\" ?", false),
        new Option<bool>("95355", "Royal Fortune Staff", "Mode: [select] only\nShould the bot buy \"Royal Fortune Staff\" ?", false),
        new Option<bool>("95357", "Bejeweled Bounty Bow", "Mode: [select] only\nShould the bot buy \"Bejeweled Bounty Bow\" ?", false),
        new Option<bool>("94831", "Warden of Hours", "Mode: [select] only\nShould the bot buy \"Warden of Hours\" ?", false),
        new Option<bool>("94832", "Warden of Hours Scarf", "Mode: [select] only\nShould the bot buy \"Warden of Hours Scarf\" ?", false),
        new Option<bool>("94833", "Warden of Hours Cowl", "Mode: [select] only\nShould the bot buy \"Warden of Hours Cowl\" ?", false),
        new Option<bool>("94834", "Warden of Hours Morph", "Mode: [select] only\nShould the bot buy \"Warden of Hours Morph\" ?", false),
        new Option<bool>("94835", "Warden of Hours Visage", "Mode: [select] only\nShould the bot buy \"Warden of Hours Visage\" ?", false),
        new Option<bool>("94836", "Warden of Hours Hair", "Mode: [select] only\nShould the bot buy \"Warden of Hours Hair\" ?", false),
        new Option<bool>("94837", "Warden of Hours Locks", "Mode: [select] only\nShould the bot buy \"Warden of Hours Locks\" ?", false),
        new Option<bool>("93782", "Shadowbound Magus", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus\" ?", false),
        new Option<bool>("93783", "Shadowbound Magus Hair", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Hair\" ?", false),
        new Option<bool>("93784", "Shadowbound Magus Locks", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Locks\" ?", false),
        new Option<bool>("93785", "Shadowbound Magus Hood", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Hood\" ?", false),
        new Option<bool>("93788", "Shadowbound Magus Rune Cloak", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Rune Cloak\" ?", false),
        new Option<bool>("93790", "Dominating Shadowbinder Sigil", "Mode: [select] only\nShould the bot buy \"Dominating Shadowbinder Sigil\" ?", false),
        new Option<bool>("73892", "Gold Jamboree Top Hat", "Mode: [select] only\nShould the bot buy \"Gold Jamboree Top Hat\" ?", false),
        new Option<bool>("73893", "Gold Jubilee Top Hat", "Mode: [select] only\nShould the bot buy \"Gold Jubilee Top Hat\" ?", false),
        new Option<bool>("73894", "Gold Triumph Axe", "Mode: [select] only\nShould the bot buy \"Gold Triumph Axe\" ?", false),
        new Option<bool>("73895", "Gold Triumph Axes", "Mode: [select] only\nShould the bot buy \"Gold Triumph Axes\" ?", false),
        new Option<bool>("73897", "Radiant Euphoria Blade", "Mode: [select] only\nShould the bot buy \"Radiant Euphoria Blade\" ?", false),
   };
}
