/*
name: EbilcorpIB Merge
description: Farms the EbilcorpIB Merge [2815] in /ebilcorpib.
tags: ebilcorpib, merge, ebilcorpib, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/BlackFriday/EbilCorpIB.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class EbilcorpIBShop
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static EbilCorpIB EbilCorp{ get => _EbilCorp ??= new EbilCorpIB(); set => _EbilCorp = value; }
    private static EbilCorpIB _EbilCorp;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Broken RAM Stick",
            "Casual Minion Hair",
            "Casual Minion Locks",
            "Casual Minion Skelewings",
            "Dark Cost Cutter Scythe",
            "Dark Cost Cutter Scythes",
            "V01d @ur@ ver7.5k",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
		if (!Core.isSeasonalMapActive("ebilcorpib")){
			Core.Logger("Map is not in season", messageBox: true);
			return;
		}
		
        EbilCorp.Storyline();
        Adv.StartBuyAllMerge("ebilcorpib", 2815, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Broken RAM Stick":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10969);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("ebilcorpib", "IB Printer", "Corrupted Hardware", 5);
                        Core.HuntMonster("ebilcorpib", "Ebilcorp IB Drone", "Corrupted Headsets", 12);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Casual Minion Hair":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("ebilcorpib", "Ebilcorp IB Drone", req.Name, quant, req.Temp);
                    break;
                case "Casual Minion Locks":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("ebilcorpib", "Ebilcorp IB Drone II", req.Name, quant, req.Temp);
                    break;
                case "Casual Minion Skelewings":
                case "Dark Cost Cutter Scythe":
                case "Dark Cost Cutter Scythes":
                case "V01d @ur@ ver7.5k":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ebilcorpib", "IB Printer", req.Name, quant, req.Temp);
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
        new Option<bool>("104382", "Business Casual Minion", "Mode: [select] only\nShould the bot buy \"Business Casual Minion\" ?", false),
        new Option<bool>("104388", "Casual Minion Wings", "Mode: [select] only\nShould the bot buy \"Casual Minion Wings\" ?", false),
        new Option<bool>("104391", "Cost Cutter Scythes", "Mode: [select] only\nShould the bot buy \"Cost Cutter Scythes\" ?", false),
        new Option<bool>("104383", "Casual Minion Morph", "Mode: [select] only\nShould the bot buy \"Casual Minion Morph\" ?", false),
        new Option<bool>("104384", "Casual Minion Visage", "Mode: [select] only\nShould the bot buy \"Casual Minion Visage\" ?", false),
        new Option<bool>("104387", "Casual Minion Hood", "Mode: [select] only\nShould the bot buy \"Casual Minion Hood\" ?", false),
        new Option<bool>("104390", "Cost Cutter Scythe", "Mode: [select] only\nShould the bot buy \"Cost Cutter Scythe\" ?", false),
        new Option<bool>("104971", "Half-Off Necrotic Sword of Doom", "Mode: [select] only\nShould the bot buy \"Half-Off Necrotic Sword of Doom\" ?", false),
    ];
}
