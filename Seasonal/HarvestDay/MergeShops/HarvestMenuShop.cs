/*
name: Harvest Menu Merge
description: Farms the Harvest Menu Merge [2811] in /charityfeast.
tags: charityfeast, merge, harvest, menu, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class HarvestMenuShop
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

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Bone Harvester Eye",
            "Bone Harvester Morph",
            "Hans' Chore Voucher",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        HarvestDay.CharityFeast();
        Adv.StartBuyAllMerge("charityfeast", 2811, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Bone Harvester Eye":
                case "Bone Harvester Morph":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("charityfeast", "Fall Fae Queen", req.Name, quant, req.Temp);
                    break;
                case "Hans' Chore Voucher":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10949 : 10948);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("charityfeast", "Sluagh King", "Sluagh King's Crescent", 1);
                        Core.HuntMonster("charityfeast", "Fall Fae Queen", "Fall Queen's Acorn", 1);
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
        new Option<bool>("100011", "Bone Harvester", "Mode: [select] only\nShould the bot buy \"Bone Harvester\" ?", false),
        new Option<bool>("102915", "Rogue Executioner", "Mode: [select] only\nShould the bot buy \"Rogue Executioner\" ?", false),
        new Option<bool>("100032", "Bone Meridian", "Mode: [select] only\nShould the bot buy \"Bone Meridian\" ?", false),
        new Option<bool>("102919", "Executioner's Spare Mace", "Mode: [select] only\nShould the bot buy \"Executioner's Spare Mace\" ?", false),
        new Option<bool>("100033", "Bone Meridians", "Mode: [select] only\nShould the bot buy \"Bone Meridians\" ?", false),
        new Option<bool>("102921", "Executioner's Maces", "Mode: [select] only\nShould the bot buy \"Executioner's Maces\" ?", false),
        new Option<bool>("100023", "Bone Harvester Mask", "Mode: [select] only\nShould the bot buy \"Bone Harvester Mask\" ?", false),
        new Option<bool>("100025", "Bone Harvester Skull", "Mode: [select] only\nShould the bot buy \"Bone Harvester Skull\" ?", false),
        new Option<bool>("102916", "Rogue Executioner Mask", "Mode: [select] only\nShould the bot buy \"Rogue Executioner Mask\" ?", false),
        new Option<bool>("102917", "Rogue Executioner Helm", "Mode: [select] only\nShould the bot buy \"Rogue Executioner Helm\" ?", false),
        new Option<bool>("102920", "Executioner's Mace", "Mode: [select] only\nShould the bot buy \"Executioner's Mace\" ?", false),
    ];
}
