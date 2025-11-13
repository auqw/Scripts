/*
name: Festival of Victors Merge
description: This bot will farm the items belonging to the selected mode for the Festival of Victors Merge [2592] in /victormatsuri
tags: festival, of, victors, merge, victormatsuri, elvish, yukata, matsuri, higasa, vampiric, crimson, wings, , chochin, sensu, floating, midnight, morph
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/VictorMatsuri.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FestivalofVictorsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    private static VictorMatsuri VM
    {
        get => _VM ??= new VictorMatsuri();
        set => _VM = value;
    }
    private static VictorMatsuri _VM;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Jade Silk", "Crimson Silk", "Midnight Silk" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        VM.Storyline(true);
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("victormatsuri", 2592, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                #region Items not setup

                case "Jade Silk":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            10292,
                            ("victormatsuri", "Narcis Arrhythmia", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Crimson Silk":
                case "Midnight Silk":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            10290,
                            ("victormatsuri", "Kitsune Himawari", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94256",
            "Elvish Yukata",
            "Mode: [select] only\nShould the bot buy \"Elvish Yukata\" ?",
            false
        ),
        new Option<bool>(
            "94257",
            "Elvish Matsuri Hair",
            "Mode: [select] only\nShould the bot buy \"Elvish Matsuri Hair\" ?",
            false
        ),
        new Option<bool>(
            "94258",
            "Elvish Matsuri Locks",
            "Mode: [select] only\nShould the bot buy \"Elvish Matsuri Locks\" ?",
            false
        ),
        new Option<bool>(
            "94259",
            "Elvish Higasa",
            "Mode: [select] only\nShould the bot buy \"Elvish Higasa\" ?",
            false
        ),
        new Option<bool>(
            "94260",
            "Vampiric Crimson Yukata",
            "Mode: [select] only\nShould the bot buy \"Vampiric Crimson Yukata\" ?",
            false
        ),
        new Option<bool>(
            "94262",
            "Vampiric Matsuri Hair",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Hair\" ?",
            false
        ),
        new Option<bool>(
            "94263",
            "Vampiric Matsuri Locks",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Locks\" ?",
            false
        ),
        new Option<bool>(
            "94266",
            "Vampiric Matsuri Wings + Crimson Chochin",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Wings + Crimson Chochin\" ?",
            false
        ),
        new Option<bool>(
            "94268",
            "Vampiric Crimson Sensu",
            "Mode: [select] only\nShould the bot buy \"Vampiric Crimson Sensu\" ?",
            false
        ),
        new Option<bool>(
            "94269",
            "Dual Vampiric Crimson Sensu",
            "Mode: [select] only\nShould the bot buy \"Dual Vampiric Crimson Sensu\" ?",
            false
        ),
        new Option<bool>(
            "94272",
            "Vampiric Matsuri Wings",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Wings\" ?",
            false
        ),
        new Option<bool>(
            "94273",
            "Floating Crimson Chochin",
            "Mode: [select] only\nShould the bot buy \"Floating Crimson Chochin\" ?",
            false
        ),
        new Option<bool>(
            "94261",
            "Vampiric Midnight Yukata",
            "Mode: [select] only\nShould the bot buy \"Vampiric Midnight Yukata\" ?",
            false
        ),
        new Option<bool>(
            "94264",
            "Vampiric Matsuri Morph",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Morph\" ?",
            false
        ),
        new Option<bool>(
            "94265",
            "Vampiric Matsuri Visage",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Visage\" ?",
            false
        ),
        new Option<bool>(
            "94267",
            "Vampiric Matsuri Wings + Midnight Chochin",
            "Mode: [select] only\nShould the bot buy \"Vampiric Matsuri Wings + Midnight Chochin\" ?",
            false
        ),
        new Option<bool>(
            "94270",
            "Vampiric Midnight Sensu",
            "Mode: [select] only\nShould the bot buy \"Vampiric Midnight Sensu\" ?",
            false
        ),
        new Option<bool>(
            "94271",
            "Dual Vampiric Midnight Sensu",
            "Mode: [select] only\nShould the bot buy \"Dual Vampiric Midnight Sensu\" ?",
            false
        ),
        new Option<bool>(
            "94274",
            "Floating Midnight Chochin",
            "Mode: [select] only\nShould the bot buy \"Floating Midnight Chochin\" ?",
            false
        ),
    };
}
