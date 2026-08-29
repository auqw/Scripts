/*
name: Carcossan Armory Merge
description: Farms the Carcossan Armory Merge [2760] in /templeofdoom.
tags: templeofdoom, merge, carcossan, armory, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs 
//cs_include Scripts/Story/SunSetSaga/CoreSunSet.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CarcossanArmory
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreSunSet CSS { get => _CSS ??= new CoreSunSet(); set => _CSS = value; }
    private static CoreSunSet _CSS;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Aegis of Order",
            "Aegis of Order Veil",
            "Culla's Joyful Touch",
            "Culled Elation",
            "Doomed Victory",
            "Lilies of Destiny Rapier",
            "Seized Destiny",
            "Sentinel of Order's Dream",
            "Sentinel of Order's Phantasm",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CSS.TempleofDoom();
        // FILL_QUEST_UNLOCK: Add the story call that completes "There's an Order to These Things" [10845] to unlock "Weapon of Choice" [10846].
        Adv.StartBuyAllMerge("templeofdoom", 2760, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Aegis of Order":
                case "Aegis of Order Veil":
                case "Seized Destiny":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("templeofdoom", "Aegis of Order", req.Name, quant, req.Temp);
                    break;
                case "Culla's Joyful Touch":
                case "Culled Elation":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("templeofdoom", "Doom Leech", req.Name, quant, req.Temp);
                    break;
                case "Doomed Victory":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10847 : 10846);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("templeofdoom", "Sentinel of Order", "Sentinel Shard", 1);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("templeofdoom", "Aegis of Order", "Aegis Shard", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Lilies of Destiny Rapier":
                case "Sentinel of Order's Dream":
                case "Sentinel of Order's Phantasm":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("templeofdoom", "Sentinel of Order", req.Name, quant, req.Temp);
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
        new Option<bool>("102894", "Ecstasy of Culla", "Mode: [select] only\nShould the bot buy \"Ecstasy of Culla\" ?", false),
        new Option<bool>("102895", "Culla, the Grip of Ecstasy", "Mode: [select] only\nShould the bot buy \"Culla, the Grip of Ecstasy\" ?", false),
        new Option<bool>("102902", "Herald of Vaugha", "Mode: [select] only\nShould the bot buy \"Herald of Vaugha\" ?", false),
        new Option<bool>("102937", "Sentinel of Order", "Mode: [select] only\nShould the bot buy \"Sentinel of Order\" ?", false),
        new Option<bool>("102940", "Sentinel of Order Cape", "Mode: [select] only\nShould the bot buy \"Sentinel of Order Cape\" ?", false),
        new Option<bool>("102904", "Herald of Vaugha's Cape", "Mode: [select] only\nShould the bot buy \"Herald of Vaugha's Cape\" ?", false),
        new Option<bool>("102899", "Culla's Loving Touch", "Mode: [select] only\nShould the bot buy \"Culla's Loving Touch\" ?", false),
        new Option<bool>("102901", "Dual Culled Elation", "Mode: [select] only\nShould the bot buy \"Dual Culled Elation\" ?", false),
        new Option<bool>("102942", "Sentinel of Order's Phantasms", "Mode: [select] only\nShould the bot buy \"Sentinel of Order's Phantasms\" ?", false),
        new Option<bool>("102935", "Lilies of Destiny Rapiers", "Mode: [select] only\nShould the bot buy \"Lilies of Destiny Rapiers\" ?", false),
        new Option<bool>("102938", "Sentinel of Order's Potential", "Mode: [select] only\nShould the bot buy \"Sentinel of Order's Potential\" ?", false),
        new Option<bool>("102932", "Aegis of Order Halo", "Mode: [select] only\nShould the bot buy \"Aegis of Order Halo\" ?", false),
        new Option<bool>("102903", "Herald of Vaugha Hood", "Mode: [select] only\nShould the bot buy \"Herald of Vaugha Hood\" ?", false),
        new Option<bool>("102896", "Culla Visage", "Mode: [select] only\nShould the bot buy \"Culla Visage\" ?", false),
        new Option<bool>("102897", "Culla's Ecstasy Visage", "Mode: [select] only\nShould the bot buy \"Culla's Ecstasy Visage\" ?", false),
        new Option<bool>("102905", "Méadaigh Amhrán", "Mode: [select] only\nShould the bot buy \"Méadaigh Amhrán\" ?", false),
    ];
}
