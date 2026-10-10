/*
name: 17th Birthday Castle Party Merge
description: This bot will farm the items belonging to the selected mode for the 17th Birthday Castle Party Merge [2627] in /castleparty
tags: 17th, birthday, castle, party, merge, castleparty, arcane, guardian, morph, founder, spectacles, evolution, asclepius, ether, creation, spear, royal, fortune, greatsword, greatswords, bejeweled, bounty, bow, warden, hours, scarf, cowl, shadowbound, magus, cloak, dominating, shadowbinder, sigil, gold, jamboree, top, jubilee, triumph, radiant, euphoria
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/AqwBirthday/CastleParty.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CastlePartyGifts
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static CastleParty CaPa { get => _CaPa ??= new CastleParty(); set => _CaPa = value; }
    private static CastleParty _CaPa;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Castle Courtyard Pool",
            "Dark Descent Rune",
            "Dark Descent Sigil",
            "Darkness Rune",
            "Dominating Shadowbinder",
            "Flux Sigil",
            "Giftbox Ribbon",
            "Gilded Gem",
            "Gleaming Ore",
            "Golden Castle Pool",
            "Golden Euphoria Blade",
            "Iota of Eternity",
            "Mana Creation Orb",
            "Mana Orb",
            "Noble Ether Staff",
            "Royal Fortune Sword",
            "Royal Fortune Swords",
            "Shadowbound Magus Cloak",
            "Undying Essence",
        ]);
        Core.SetOptions();
		CaPa.Storyline();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("castleparty", 2627, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Castle Courtyard Pool":
                case "Gleaming Ore":
                case "Golden Castle Pool":
                case "Noble Ether Staff":
                case "Royal Fortune Sword":
                case "Royal Fortune Swords":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("castleparty", "Treasure Chest", req.Name, quant, req.Temp);
                    break;
                case "Dark Descent Rune":
                case "Dark Descent Sigil":
                case "Darkness Rune":
                case "Dominating Shadowbinder":
                case "Shadowbound Magus Cloak":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("castleparty", "Nulgath's Gift", req.Name, quant, req.Temp);
                    break;
                case "Flux Sigil":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("castleparty", "Drakath's Gift", req.Name, quant, req.Temp);
                    break;
                case "Giftbox Ribbon":
                case "Golden Euphoria Blade":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("castleparty", "Lost Giftbox", req.Name, quant, req.Temp);
                    break;
                case "Gilded Gem":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("castleparty", "Noxus' Gift", req.Name, quant, req.Temp);
                    break;
                case "Iota of Eternity":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("castleparty", "Kathool's Gift", req.Name, quant, req.Temp);
                    break;
                case "Mana Creation Orb":
                case "Mana Orb":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("castleparty", "Legion Partycrasher", req.Name, quant, req.Temp);
                    break;
                case "Undying Essence":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("castleparty", "Sally's Gift", req.Name, quant, req.Temp);
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
        new Option<bool>("96136", "Arcane Guardian", "Mode: [select] only\nShould the bot buy \"Arcane Guardian\" ?", false),
        new Option<bool>("94831", "Warden of Hours", "Mode: [select] only\nShould the bot buy \"Warden of Hours\" ?", false),
        new Option<bool>("93782", "Shadowbound Magus", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus\" ?", false),
        new Option<bool>("103778", "Refined Classic Paladin", "Mode: [select] only\nShould the bot buy \"Refined Classic Paladin\" ?", false),
        new Option<bool>("103786", "Refined Heraldic Paladin", "Mode: [select] only\nShould the bot buy \"Refined Heraldic Paladin\" ?", false),
        new Option<bool>("103787", "Refined Auric Paladin", "Mode: [select] only\nShould the bot buy \"Refined Auric Paladin\" ?", false),
        new Option<bool>("73894", "Gold Triumph Axe", "Mode: [select] only\nShould the bot buy \"Gold Triumph Axe\" ?", false),
        new Option<bool>("95357", "Bejeweled Bounty Bow", "Mode: [select] only\nShould the bot buy \"Bejeweled Bounty Bow\" ?", false),
        new Option<bool>("103783", "Paladin's Tribute Cape", "Mode: [select] only\nShould the bot buy \"Paladin's Tribute Cape\" ?", false),
        new Option<bool>("93788", "Shadowbound Magus Rune Cloak", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Rune Cloak\" ?", false),
        new Option<bool>("93790", "Dominating Shadowbinder Sigil", "Mode: [select] only\nShould the bot buy \"Dominating Shadowbinder Sigil\" ?", false),
        new Option<bool>("96148", "Arcane Evolution Blades", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Blades\" ?", false),
        new Option<bool>("96151", "Asclepius Creation Staff", "Mode: [select] only\nShould the bot buy \"Asclepius Creation Staff\" ?", false),
        new Option<bool>("95350", "Royal Fortune Greatswords", "Mode: [select] only\nShould the bot buy \"Royal Fortune Greatswords\" ?", false),
        new Option<bool>("73895", "Gold Triumph Axes", "Mode: [select] only\nShould the bot buy \"Gold Triumph Axes\" ?", false),
        new Option<bool>("103785", "Refined Classic Destiny Blades", "Mode: [select] only\nShould the bot buy \"Refined Classic Destiny Blades\" ?", false),
        new Option<bool>("96398", "Golden Alteon Statue", "Mode: [select] only\nShould the bot buy \"Golden Alteon Statue\" ?", false),
        new Option<bool>("96399", "Golden Tribute Pool", "Mode: [select] only\nShould the bot buy \"Golden Tribute Pool\" ?", false),
        new Option<bool>("96400", "Golden Tribute Pool of Honor", "Mode: [select] only\nShould the bot buy \"Golden Tribute Pool of Honor\" ?", false),
        new Option<bool>("96402", "Castle Dragon Statue", "Mode: [select] only\nShould the bot buy \"Castle Dragon Statue\" ?", false),
        new Option<bool>("96403", "Festive Dragon Statue", "Mode: [select] only\nShould the bot buy \"Festive Dragon Statue\" ?", false),
        new Option<bool>("96404", "Giftwrapped Dragon Statue", "Mode: [select] only\nShould the bot buy \"Giftwrapped Dragon Statue\" ?", false),
        new Option<bool>("73892", "Gold Jamboree Top Hat", "Mode: [select] only\nShould the bot buy \"Gold Jamboree Top Hat\" ?", false),
        new Option<bool>("73893", "Gold Jubilee Top Hat", "Mode: [select] only\nShould the bot buy \"Gold Jubilee Top Hat\" ?", false),
        new Option<bool>("93783", "Shadowbound Magus Hair", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Hair\" ?", false),
        new Option<bool>("93784", "Shadowbound Magus Locks", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Locks\" ?", false),
        new Option<bool>("93785", "Shadowbound Magus Hood", "Mode: [select] only\nShould the bot buy \"Shadowbound Magus Hood\" ?", false),
        new Option<bool>("94832", "Warden of Hours Scarf", "Mode: [select] only\nShould the bot buy \"Warden of Hours Scarf\" ?", false),
        new Option<bool>("94833", "Warden of Hours Cowl", "Mode: [select] only\nShould the bot buy \"Warden of Hours Cowl\" ?", false),
        new Option<bool>("94834", "Warden of Hours Morph", "Mode: [select] only\nShould the bot buy \"Warden of Hours Morph\" ?", false),
        new Option<bool>("94835", "Warden of Hours Visage", "Mode: [select] only\nShould the bot buy \"Warden of Hours Visage\" ?", false),
        new Option<bool>("94836", "Warden of Hours Hair", "Mode: [select] only\nShould the bot buy \"Warden of Hours Hair\" ?", false),
        new Option<bool>("94837", "Warden of Hours Locks", "Mode: [select] only\nShould the bot buy \"Warden of Hours Locks\" ?", false),
        new Option<bool>("96137", "Arcane Guardian Morph", "Mode: [select] only\nShould the bot buy \"Arcane Guardian Morph\" ?", false),
        new Option<bool>("96138", "Arcane Guardian Visage", "Mode: [select] only\nShould the bot buy \"Arcane Guardian Visage\" ?", false),
        new Option<bool>("96139", "Arcane Founder Morph", "Mode: [select] only\nShould the bot buy \"Arcane Founder Morph\" ?", false),
        new Option<bool>("96140", "Arcane Founder Visage", "Mode: [select] only\nShould the bot buy \"Arcane Founder Visage\" ?", false),
        new Option<bool>("96141", "Arcane Spectacles Morph", "Mode: [select] only\nShould the bot buy \"Arcane Spectacles Morph\" ?", false),
        new Option<bool>("96142", "Arcane Spectacles Visage", "Mode: [select] only\nShould the bot buy \"Arcane Spectacles Visage\" ?", false),
        new Option<bool>("96143", "Arcane Founder Hair", "Mode: [select] only\nShould the bot buy \"Arcane Founder Hair\" ?", false),
        new Option<bool>("96144", "Arcane Founder Locks", "Mode: [select] only\nShould the bot buy \"Arcane Founder Locks\" ?", false),
        new Option<bool>("103788", "Auric Destiny Helm", "Mode: [select] only\nShould the bot buy \"Auric Destiny Helm\" ?", false),
        new Option<bool>("103779", "Refined Classic Helm", "Mode: [select] only\nShould the bot buy \"Refined Classic Helm\" ?", false),
        new Option<bool>("103780", "Refined Destiny Helm", "Mode: [select] only\nShould the bot buy \"Refined Destiny Helm\" ?", false),
        new Option<bool>("103781", "Refined Classic Visor", "Mode: [select] only\nShould the bot buy \"Refined Classic Visor\" ?", false),
        new Option<bool>("103782", "Refined Paladin Morph", "Mode: [select] only\nShould the bot buy \"Refined Paladin Morph\" ?", false),
        new Option<bool>("79358", "Lore's Legendary Hero Rune", "Mode: [select] only\nShould the bot buy \"Lore's Legendary Hero Rune\" ?", false),
        new Option<bool>("96152", "Arcane Evolution Spear", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Spear\" ?", false),
        new Option<bool>("96150", "Asclepius Ether Staff", "Mode: [select] only\nShould the bot buy \"Asclepius Ether Staff\" ?", false),
        new Option<bool>("95355", "Royal Fortune Staff", "Mode: [select] only\nShould the bot buy \"Royal Fortune Staff\" ?", false),
        new Option<bool>("95349", "Royal Fortune Greatsword", "Mode: [select] only\nShould the bot buy \"Royal Fortune Greatsword\" ?", false),
        new Option<bool>("96147", "Arcane Evolution Blade", "Mode: [select] only\nShould the bot buy \"Arcane Evolution Blade\" ?", false),
        new Option<bool>("73897", "Radiant Euphoria Blade", "Mode: [select] only\nShould the bot buy \"Radiant Euphoria Blade\" ?", false),
        new Option<bool>("103784", "Refined Classic Destiny Blade", "Mode: [select] only\nShould the bot buy \"Refined Classic Destiny Blade\" ?", false),
    ];
}
