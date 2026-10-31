/*
name: Feast of the Depths Merge
description: Farms the Feast of the Depths Merge [2633] in /elodeatemple.
tags: elodeatemple, merge, feast, of, the, depths, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FeastoftheDepths
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
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Acromegalia Seed",
            "Depths Scale",
            "Head of Hair",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        // FILL_QUEST_UNLOCK: Add the story call that completes "The Victor! The Survivor!" [10478] to unlock "Return from Seafoam" [10479].
        Adv.StartBuyAllMerge("elodeatemple", 2633, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Acromegalia Seed": // #FROM CASE STORAGE
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10457 : 10454);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("eldritchbattletown", "Kathool Kultist", "Tier 2 Kathool Member Card", 6, isTemp: false);
                        Core.HuntMonster("eldritchbattletown", "Dzeza Cultist", "Tier 2 Dzeza Member Card", 6, isTemp: false);
                        Core.HuntMonster("eldritchbattletown", "Harvest Acromegalia", "Gourd Twinning", isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Depths Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10480 : 10479);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("elodeatemple", "Child of the Depths", "Goldfish's Wish", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Head of Hair": // #FROM CASE STORAGE
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10468, "eldritchworld", "Mass of Hair");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Shrimp Noodle Pack": // #FROM CASE STORAGE
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
                        Core.HuntMonsterQuest(10460, "eldritchworld", "Kathool Cultist");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
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
        new Option<bool>("89695", "Pumpkin Warlock", "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock\" ?", false),
        new Option<bool>("95578", "Erudite Plague Doctor", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor\" ?", false),
        new Option<bool>("95579", "Faustian Plague Doctor", "Mode: [select] only\nShould the bot buy \"Faustian Plague Doctor\" ?", false),
        new Option<bool>("104743", "Elodea's Dress", "Mode: [select] only\nShould the bot buy \"Elodea's Dress\" ?", false),
        new Option<bool>("95584", "Erudite Plague Doctor Cloak", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Cloak\" ?", false),
        new Option<bool>("95586", "Crimson Darkling Lamps", "Mode: [select] only\nShould the bot buy \"Crimson Darkling Lamps\" ?", false),
        new Option<bool>("95580", "Erudite Plague Doctor Hair", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Hair\" ?", false),
        new Option<bool>("95581", "Erudite Plague Doctor Locks", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Locks\" ?", false),
        new Option<bool>("95582", "Erudite Plague Doctor Mask", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Mask\" ?", false),
        new Option<bool>("95583", "Erudite Plague Doctor Moustache", "Mode: [select] only\nShould the bot buy \"Erudite Plague Doctor Moustache\" ?", false),
        new Option<bool>("89696", "Pumpkin Warlock Hair", "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Hair\" ?", false),
        new Option<bool>("89697", "Pumpkin Witch Locks", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Locks\" ?", false),
        new Option<bool>("89698", "Pumpkin Witch Ponytail", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Ponytail\" ?", false),
        new Option<bool>("89699", "Pumpkin Warlock Long Hair", "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Long Hair\" ?", false),
        new Option<bool>("89700", "Pumpkin Witch Twintails", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Twintails\" ?", false),
        new Option<bool>("89701", "Pumpkin Warlock Hat", "Mode: [select] only\nShould the bot buy \"Pumpkin Warlock Hat\" ?", false),
        new Option<bool>("89702", "Pumpkin Witch Hat", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Hat\" ?", false),
        new Option<bool>("89703", "Pumpkin Witch Ponytail Hat", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Ponytail Hat\" ?", false),
        new Option<bool>("89704", "Pumpkin Witch Twintail Hat", "Mode: [select] only\nShould the bot buy \"Pumpkin Witch Twintail Hat\" ?", false),
        new Option<bool>("96369", "Elodea Visage", "Mode: [select] only\nShould the bot buy \"Elodea Visage\" ?", false),
        new Option<bool>("96370", "Elodea's Befuddlement", "Mode: [select] only\nShould the bot buy \"Elodea's Befuddlement\" ?", false),
        new Option<bool>("96371", "Elodea's Joy", "Mode: [select] only\nShould the bot buy \"Elodea's Joy\" ?", false),
        new Option<bool>("96372", "Elodea Morph", "Mode: [select] only\nShould the bot buy \"Elodea Morph\" ?", false),
        new Option<bool>("96373", "Elodea's Gloom", "Mode: [select] only\nShould the bot buy \"Elodea's Gloom\" ?", false),
        new Option<bool>("96374", "Elodea's Surprise", "Mode: [select] only\nShould the bot buy \"Elodea's Surprise\" ?", false),
        new Option<bool>("96375", "Elodea's Pout", "Mode: [select] only\nShould the bot buy \"Elodea's Pout\" ?", false),
        new Option<bool>("96376", "Elodea's Tears", "Mode: [select] only\nShould the bot buy \"Elodea's Tears\" ?", false),
        new Option<bool>("95585", "Crimson Darkling Lamp", "Mode: [select] only\nShould the bot buy \"Crimson Darkling Lamp\" ?", false),
        new Option<bool>("89707", "Mogloween Forest", "Mode: [select] only\nShould the bot buy \"Mogloween Forest\" ?", false),
        new Option<bool>("89708", "Mangelwurzel Broom Pet", "Mode: [select] only\nShould the bot buy \"Mangelwurzel Broom Pet\" ?", false),
        new Option<bool>("89709", "Mangelwurzel Broom", "Mode: [select] only\nShould the bot buy \"Mangelwurzel Broom\" ?", false),
        new Option<bool>("89711", "Standing Mangelwurzel Broom", "Mode: [select] only\nShould the bot buy \"Standing Mangelwurzel Broom\" ?", false),
        new Option<bool>("95587", "Iron Doctor's Cane", "Mode: [select] only\nShould the bot buy \"Iron Doctor's Cane\" ?", false),
        new Option<bool>("95588", "Forged Doctor's Cane", "Mode: [select] only\nShould the bot buy \"Forged Doctor's Cane\" ?", false),
    ];
}
