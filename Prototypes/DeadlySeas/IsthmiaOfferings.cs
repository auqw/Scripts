/*
name: Isthmia Offerings Merge
description: Farms the Isthmia Offerings Merge [2773] in /isthmiacastle.
tags: isthmiacastle, merge, isthmia, offerings, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/DeadlySeas/Isthmiacastle.cs
//cs_include Scripts/Prototypes/DeadlySeas/RedBettysBudget.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class IsthmiaOfferings
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static Isthmiacastle IsthmiaCastle { get => _IsthmiaCastle ??= new Isthmiacastle(); set => _IsthmiaCastle = value; }
    private static Isthmiacastle _IsthmiaCastle;
    private static RedBettysBudget RBB { get => _RBB ??= new RedBettysBudget(); set => _RBB = value;}
    private static RedBettysBudget _RBB;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
			"Imperial Isthmian Wand",
			"Imperial Isthmian Gauntlet",
			"Imperial Isthmian Gauntlets",
            "BeastBane Cutlass",
            "BeastBane Cutlasses",
            "BeastBane Seahook",
            "BeastBane Seahooks",
            "Isthmian Conch",
            "Pearlescent Cyprinus",
            "Royal Isthmian Axe",
            "Royal Isthmian Gauntlet",
            "Royal Isthmian Gauntlets",
            "Royal Isthmian Mace",
            "Royal Isthmian Wand",
        ]);
        Core.SetOptions();
        IsthmiaCastle.Storyline();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("isthmiacastle", 2773, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
				case "Imperial Isthmian Gauntlet":
				case "Imperial Isthmian Gauntlets":
                case "BeastBane Cutlass":
                case "BeastBane Cutlasses":
                case "BeastBane Seahook":
                case "BeastBane Seahooks":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("isthmiacastle", "Reaper of the Sea", req.Name, quant, req.Temp);
                    break;
                case "Isthmian Conch":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10880 : 10879);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("isthmiacastle", "Mersoul", "Mersoul Reaped", 6);
                        Core.HuntMonster("isthmiacastle", "Reaper of the Sea", "Reaper's Toy Model", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Pearlescent Cyprinus":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("isthmiacastle", "Mersoul", req.Name, quant, req.Temp);
                    break;
                case "Royal Isthmian Axe":
                case "Royal Isthmian Mace":
                    RBB.BuyAllMerge(req.Name);
                    break;
                case "Royal Isthmian Gauntlet":
                case "Royal Isthmian Gauntlets":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("deadlyseas", "Isthmian Leviathan", req.Name, quant, req.Temp);
                    break;
                case "Royal Isthmian Wand":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("deadlyseas", "Isthmian Guard", req.Name, quant, req.Temp);
                    break;
				case "Imperial Isthmian Wand":
					Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("isthmiacastle", "Isthmian Guard", req.Name, quant, req.Temp);
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}.", messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("103716", "Siren of the Nethersea", "Mode: [select] only\nShould the bot buy \"Siren of the Nethersea\" ?", false),
        new Option<bool>("103760", "ShadowSlayer Corsair", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair\" ?", false),
        new Option<bool>("103759", "Imperial Isthmian Axe", "Mode: [select] only\nShould the bot buy \"Imperial Isthmian Axe\" ?", false),
        new Option<bool>("103772", "Reverse BeastBane Cutlasses", "Mode: [select] only\nShould the bot buy \"Reverse BeastBane Cutlasses\" ?", false),
        new Option<bool>("103774", "Reverse BeastBane Seahooks", "Mode: [select] only\nShould the bot buy \"Reverse BeastBane Seahooks\" ?", false),
        new Option<bool>("103777", "ShadowSlayer Fishing Gear", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Fishing Gear\" ?", false),
        new Option<bool>("103761", "ShadowSlayer Corsair Hair", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair Hair\" ?", false),
        new Option<bool>("103762", "ShadowSlayer Corsair Bun", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair Bun\" ?", false),
        new Option<bool>("103763", "ShadowSlayer Corsair Stetson", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair Stetson\" ?", false),
        new Option<bool>("103764", "ShadowSlayer Corsair Hat", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair Hat\" ?", false),
        new Option<bool>("103765", "ShadowSlayer Corsair Bicorne", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Corsair Bicorne\" ?", false),
        new Option<bool>("103717", "Nethersea Siren Morph", "Mode: [select] only\nShould the bot buy \"Nethersea Siren Morph\" ?", false),
        new Option<bool>("103718", "Nethersea Siren Visage", "Mode: [select] only\nShould the bot buy \"Nethersea Siren Visage\" ?", false),
        new Option<bool>("103754", "Imperial Isthmian Mace", "Mode: [select] only\nShould the bot buy \"Imperial Isthmian Mace\" ?", false),
        new Option<bool>("103771", "Reverse BeastBane Cutlass", "Mode: [select] only\nShould the bot buy \"Reverse BeastBane Cutlass\" ?", false),
        new Option<bool>("103773", "Reverse BeastBane Seahook", "Mode: [select] only\nShould the bot buy \"Reverse BeastBane Seahook\" ?", false),
        new Option<bool>("103752", "Imperial Isthmian Scythe", "Mode: [select] only\nShould the bot buy \"Imperial Isthmian Scythe\" ?", false),
        new Option<bool>("103750", "Imperial Isthmian Staff", "Mode: [select] only\nShould the bot buy \"Imperial Isthmian Staff\" ?", false),
        new Option<bool>("103720", "Innocent Cyprinus Staff", "Mode: [select] only\nShould the bot buy \"Innocent Cyprinus Staff\" ?", false),
    ];
}
