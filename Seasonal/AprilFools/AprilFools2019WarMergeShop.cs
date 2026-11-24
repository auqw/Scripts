/*
name: April Fools 2019 War Merge
description: Farms the April Fools 2019 War Merge [1709] in /pal9001.
tags: pal9001, merge, april, fools, 2019, war, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AprilFools2019WarMergeShop
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
            "Punadin Badge",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("pal9001", 1709, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Punadin Badge":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("pal9001", "Baby SharkCaster", req.Name, quant, req.Temp);
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
        new Option<bool>("48035", "Nekomancer", "Mode: [select] only\nShould the bot buy \"Nekomancer\" ?", false),
        new Option<bool>("48049", "Nekomancer Deadly Empress Cape", "Mode: [select] only\nShould the bot buy \"Nekomancer Deadly Empress Cape\" ?", false),
        new Option<bool>("48044", "Nekomancer Tail", "Mode: [select] only\nShould the bot buy \"Nekomancer Tail\" ?", false),
        new Option<bool>("48042", "Nekomancer Morph Locks", "Mode: [select] only\nShould the bot buy \"Nekomancer Morph Locks\" ?", false),
        new Option<bool>("48040", "Nekomancer Locks", "Mode: [select] only\nShould the bot buy \"Nekomancer Locks\" ?", false),
        new Option<bool>("48041", "Nekomancer Hood Locks", "Mode: [select] only\nShould the bot buy \"Nekomancer Hood Locks\" ?", false),
        new Option<bool>("48043", "Nekomancer Morph Hood Locks", "Mode: [select] only\nShould the bot buy \"Nekomancer Morph Hood Locks\" ?", false),
        new Option<bool>("48038", "Nekomancer Morph Hair", "Mode: [select] only\nShould the bot buy \"Nekomancer Morph Hair\" ?", false),
        new Option<bool>("48037", "Nekomancer Hood Hair", "Mode: [select] only\nShould the bot buy \"Nekomancer Hood Hair\" ?", false),
        new Option<bool>("48039", "Nekomancer Morph Hood Hair", "Mode: [select] only\nShould the bot buy \"Nekomancer Morph Hood Hair\" ?", false),
        new Option<bool>("48036", "Nekomancer Hair", "Mode: [select] only\nShould the bot buy \"Nekomancer Hair\" ?", false),
        new Option<bool>("48045", "Nekosmasher Mace", "Mode: [select] only\nShould the bot buy \"Nekosmasher Mace\" ?", false),
        new Option<bool>("48046", "Neko Spindle Staff", "Mode: [select] only\nShould the bot buy \"Neko Spindle Staff\" ?", false),
    ];
}
