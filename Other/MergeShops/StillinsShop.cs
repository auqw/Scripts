/*
name: Stillins Merge
description: Farms the Stillins Merge [2806] in /kiyametgrove.
tags: kiyametgrove, merge, stillins, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/KiyametGrove.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class StillinsShop
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static KiyametGrove Kiyamet{ get => _Kiyamet ??= new KiyametGrove(); set => _Kiyamet = value; }
    private static KiyametGrove _Kiyamet;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Apocalyptic Colossus Guard",
            "Apocalyptic Student’s Hair",
            "Apocalyptic Student’s Locks",
            "Apocalyptic Student’s Mistake",
            "Azure Rune",
            "Golem Hand Gauntlets",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Kiyamet.Storyline();
        Adv.StartBuyAllMerge("kiyametgrove", 2806, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Apocalyptic Colossus Guard":
                case "Apocalyptic Student’s Hair":
                case "Apocalyptic Student’s Locks":
                case "Apocalyptic Student’s Mistake":
                case "Golem Hand Gauntlets":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("kiyametgrove", "Overcharged Colossus", req.Name, quant, req.Temp);
                    break;
                case "Azure Rune":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10937);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("kiyametgrove", "Overcharged Colossus", "Overcharged Golem Stone", 1);
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
        new Option<bool>("104781", "Apocalyptic Student’s Tattoos", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Tattoos\" ?", false),
        new Option<bool>("104787", "Apocalyptic Student’s Accoutrements", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Accoutrements\" ?", false),
        new Option<bool>("104788", "Apocalyptic Pillar Backblade", "Mode: [select] only\nShould the bot buy \"Apocalyptic Pillar Backblade\" ?", false),
        new Option<bool>("104789", "Apocalyptic Student’s Wraps", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Wraps\" ?", false),
        new Option<bool>("104790", "Overcharged Colossus Guard", "Mode: [select] only\nShould the bot buy \"Overcharged Colossus Guard\" ?", false),
        new Option<bool>("104793", "Apocalyptic Student’s Pillars", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Pillars\" ?", false),
        new Option<bool>("104795", "Overcharged Golem Gauntlets", "Mode: [select] only\nShould the bot buy \"Overcharged Golem Gauntlets\" ?", false),
        new Option<bool>("104796", "Back Arm Sword Gauntlet", "Mode: [select] only\nShould the bot buy \"Back Arm Sword Gauntlet\" ?", false),
        new Option<bool>("104783", "Apocalyptic Student’s Blind Hair", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Blind Hair\" ?", false),
        new Option<bool>("104785", "Apocalyptic Student’s Blind Locks", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Blind Locks\" ?", false),
        new Option<bool>("104792", "Apocalyptic Student’s Pillar", "Mode: [select] only\nShould the bot buy \"Apocalyptic Student’s Pillar\" ?", false),
    ];
}
