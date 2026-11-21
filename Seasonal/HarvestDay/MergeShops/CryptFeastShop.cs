/*
name: Crypt Feast Merge
description: Farms the Crypt Feast Merge [2814] in /cryptfeast.
tags: cryptfeast, merge, crypt, feast, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/HarvestMenuShop.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CryptFeastShop
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static CoreHarvestDay HarvestDay
    {
        get => _HarvestDay ??= new CoreHarvestDay();
        set => _HarvestDay = value;
    }
    private static CoreHarvestDay _HarvestDay;
	private static HarvestMenuShop HarvestMenu { get => _HarvestMenu ??= new HarvestMenuShop(); set => _HarvestMenu = value;}
    private static HarvestMenuShop _HarvestMenu;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Autumnal Wonder Blade",
            "Autumnal Wonder Bow",
            "Autumnal Wonder Sword",
            "Bone Harvester",
            "Bone Harvester Eye",
            "Bone Harvester Gaze",
            "Bone Harvester Jaw",
            "Bone Harvester Mask",
            "Bone Harvester Skull",
            "Bone Harvester's Stare",
            "Bone Meridian",
            "Undying Slasher Hair",
            "Undying Slasher Locks",
            "Va[]tr[][]h Bones",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
		HarvestDay.CharityFeast();
		HarvestDay.CryptFeast();
        Adv.StartBuyAllMerge("cryptfeast", 2814, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Autumnal Wonder Blade":
                case "Autumnal Wonder Bow":
                case "Autumnal Wonder Sword":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("cryptfeast", "Fall Unseelie Queen", req.Name, quant, req.Temp);
                    break;
				case "Bone Harvester Eye":
					Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("charityfeast", "Fall Fae Queen", req.Name, quant, req.Temp);
					break;
				case "Bone Harvester Gaze":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("charityfeast", "Sluagh King", req.Name, quant, req.Temp);
					break;
                case "Bone Harvester":
                case "Bone Harvester Jaw":
                case "Bone Harvester Mask":
                case "Bone Harvester Skull":
                case "Bone Harvester's Stare":
                case "Bone Meridian":
					HarvestMenu.BuyAllMerge(req.Name);
                    break;
                case "Undying Slasher Hair":
                case "Undying Slasher Locks":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("cryptfeast", "The Last Lady", req.Name, quant, req.Temp);
                    break;
                case "Va[]tr[][]h Bones":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10963 : 10962);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("cryptfeast", "The Last Lady", "Last Lady's Mask", 1);
                        Core.HuntMonster("cryptfeast", "Sluagh Warrior", "Sluagh's Core", 6);
						Core.HuntMonster("cryptfeast", "Masked Warriors", "Warrior's Carving", 6);
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
        new Option<bool>("100059", "Enchanted Bone Harvester", "Mode: [select] only\nShould the bot buy \"Enchanted Bone Harvester\" ?", false),
        new Option<bool>("105180", "Undying Slasher", "Mode: [select] only\nShould the bot buy \"Undying Slasher\" ?", false),
        new Option<bool>("100074", "Enchanted Bone Meridian", "Mode: [select] only\nShould the bot buy \"Enchanted Bone Meridian\" ?", false),
        new Option<bool>("100075", "Enchanted Bone Meridians", "Mode: [select] only\nShould the bot buy \"Enchanted Bone Meridians\" ?", false),
        new Option<bool>("104398", "Autumnal Wonder Blades", "Mode: [select] only\nShould the bot buy \"Autumnal Wonder Blades\" ?", false),
        new Option<bool>("104402", "Autumnal Wonder Swords", "Mode: [select] only\nShould the bot buy \"Autumnal Wonder Swords\" ?", false),
        new Option<bool>("104409", "Autumnal Wonder Bow and Arrows", "Mode: [select] only\nShould the bot buy \"Autumnal Wonder Bow and Arrows\" ?", false),
        new Option<bool>("105183", "Undying Slasher Morph", "Mode: [select] only\nShould the bot buy \"Undying Slasher Morph\" ?", false),
        new Option<bool>("105184", "Undying Slasher Visage", "Mode: [select] only\nShould the bot buy \"Undying Slasher Visage\" ?", false),
        new Option<bool>("100060", "Enchanted Harvester Eye", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Eye\" ?", false),
        new Option<bool>("100061", "Enchanted Harvester Skull", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Skull\" ?", false),
        new Option<bool>("100062", "Enchanted Harvester Mouth", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Mouth\" ?", false),
        new Option<bool>("100063", "Enchanted Harvester Visor", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Visor\" ?", false),
        new Option<bool>("100064", "Enchanted Harvester Stare", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Stare\" ?", false),
        new Option<bool>("100065", "Enchanted Harvester Gaze", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Gaze\" ?", false),
        new Option<bool>("100066", "Enchanted Harvester Jaw", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Jaw\" ?", false),
        new Option<bool>("100067", "Enchanted Harvester Mandible", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Mandible\" ?", false),
        new Option<bool>("100068", "Enchanted Harvester Disguise", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Disguise\" ?", false),
        new Option<bool>("100069", "Enchanted Harvester Mask", "Mode: [select] only\nShould the bot buy \"Enchanted Harvester Mask\" ?", false),
    ];
}
