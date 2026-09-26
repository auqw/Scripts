/*
name: Red Bettys Budget Merge
description: Farms the Red Bettys Budget Merge [2767] in /deadlyseas.
tags: deadlyseas, merge, red, bettys, budget, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/DeadlySeas/Deadlyseas.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class RedBettysBudget
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static Deadlyseas DeadlySeas { get => _DeadlySeas ??= new Deadlyseas(); set => _DeadlySeas = value; }
    private static Deadlyseas _DeadlySeas;				  

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Isthmian Seashell",
            "Mermaid Hunter Hair",
            "Mermaid Hunter Locks",
            "Royal Isthmian Gauntlet",
            "Royal Isthmian Gauntlets",
            "Royal Isthmian Wand",
        ]);
        Core.SetOptions();
		DeadlySeas.Storyline();		   
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("deadlyseas", 2767, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Isthmian Seashell":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10868 : 10867);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("deadlyseas", "Isthmian Guard", "Guard's Pearl Polish", 4);
                        Core.HuntMonster("deadlyseas", "Isthmian Leviathan", "Leviathan's Egg", 1);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("deadlyseas", "Isthmian Eel", "Smoked Eel", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Mermaid Hunter Hair":
                case "Mermaid Hunter Locks":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("deadlyseas", "Mermaid Hunter", req.Name, quant, req.Temp);
                    break;
                case "Royal Isthmian Gauntlet":
                case "Royal Isthmian Gauntlets":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("deadlyseas", "Isthmian Leviathan", req.Name, quant, req.Temp);
                    break;
                case "Royal Isthmian Wand":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("deadlyseas", "Isthmian Guard", req.Name, quant, req.Temp);
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
        new Option<bool>("103266", "Mermaid Hunter", "Mode: [select] only\nShould the bot buy \"Mermaid Hunter\" ?", false),
        new Option<bool>("103455", "Royal Isthmian Guard", "Mode: [select] only\nShould the bot buy \"Royal Isthmian Guard\" ?", false),
        new Option<bool>("103741", "Royal Isthmian Axe", "Mode: [select] only\nShould the bot buy \"Royal Isthmian Axe\" ?", false),
        new Option<bool>("103273", "Nereid Skin Cape", "Mode: [select] only\nShould the bot buy \"Nereid Skin Cape\" ?", false),
        new Option<bool>("103458", "Isthmian Pearl Shield", "Mode: [select] only\nShould the bot buy \"Isthmian Pearl Shield\" ?", false),
        new Option<bool>("103456", "Isthmian Guard Helm", "Mode: [select] only\nShould the bot buy \"Isthmian Guard Helm\" ?", false),
        new Option<bool>("103457", "Isthmian Guard Visor", "Mode: [select] only\nShould the bot buy \"Isthmian Guard Visor\" ?", false),
        new Option<bool>("103267", "Mermaid Hunter Mask Morph", "Mode: [select] only\nShould the bot buy \"Mermaid Hunter Mask Morph\" ?", false),
        new Option<bool>("103268", "Mermaid Hunter Visage", "Mode: [select] only\nShould the bot buy \"Mermaid Hunter Visage\" ?", false),
        new Option<bool>("103269", "Mermaid Poacher Morph", "Mode: [select] only\nShould the bot buy \"Mermaid Poacher Morph\" ?", false),
        new Option<bool>("103270", "Mermaid Poacher Visage", "Mode: [select] only\nShould the bot buy \"Mermaid Poacher Visage\" ?", false),
        new Option<bool>("103736", "Royal Isthmian Mace", "Mode: [select] only\nShould the bot buy \"Royal Isthmian Mace\" ?", false),
        new Option<bool>("103734", "Royal Isthmian Scythe", "Mode: [select] only\nShould the bot buy \"Royal Isthmian Scythe\" ?", false),
        new Option<bool>("103276", "Siren's Anathema", "Mode: [select] only\nShould the bot buy \"Siren's Anathema\" ?", false),
        new Option<bool>("103732", "Royal Isthmian Staff", "Mode: [select] only\nShould the bot buy \"Royal Isthmian Staff\" ?", false),
    ];
}
