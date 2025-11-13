/*
name: Starhunt Merge
description: This bot will farm the items belonging to the selected mode for the Starhunt Merge [2598] in /ariagreenhouse
tags: starhunt, merge, ariagreenhouse, astromancers, apprentice, morph, astral, compass, fireworks, qipao, star, festival, weapon, gift, i, ii, crimson, staredge, suhail, staredges, novablade, persei, novablades, spectral, saiph, alnitak, celestial
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/AriaGreenhouse.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class StarhuntMerge
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

    private static AriaGreenhouse AG
    {
        get => _AG ??= new AriaGreenhouse();
        set => _AG = value;
    }
    private static AriaGreenhouse _AG;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Delta Fragment" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AG.Kylokos();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "ariagreenhouse",
            2598,
            findIngredients,
            buyOnlyThis,
            buyMode: buyMode
        );

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

                case "Delta Fragment":
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
                        Core.HuntMonsterQuest(
                            10318,
                            ("yokaiportal", "Kitsune Spirits", ClassType.Farm)
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
            "94555",
            "Astromancer's Apprentice",
            "Mode: [select] only\nShould the bot buy \"Astromancer's Apprentice\" ?",
            false
        ),
        new Option<bool>(
            "94556",
            "Astromancer's Apprentice Morph",
            "Mode: [select] only\nShould the bot buy \"Astromancer's Apprentice Morph\" ?",
            false
        ),
        new Option<bool>(
            "94557",
            "Astromancer's Apprentice Visage",
            "Mode: [select] only\nShould the bot buy \"Astromancer's Apprentice Visage\" ?",
            false
        ),
        new Option<bool>(
            "94558",
            "Astral Apprentice Compass",
            "Mode: [select] only\nShould the bot buy \"Astral Apprentice Compass\" ?",
            false
        ),
        new Option<bool>(
            "94559",
            "Fireworks Qipao",
            "Mode: [select] only\nShould the bot buy \"Fireworks Qipao\" ?",
            false
        ),
        new Option<bool>(
            "94560",
            "Astromancer's Apprentice Hair",
            "Mode: [select] only\nShould the bot buy \"Astromancer's Apprentice Hair\" ?",
            false
        ),
        new Option<bool>(
            "94561",
            "Astromancer's Apprentice Locks",
            "Mode: [select] only\nShould the bot buy \"Astromancer's Apprentice Locks\" ?",
            false
        ),
        new Option<bool>(
            "94562",
            "Star Festival Weapon Gift I",
            "Mode: [select] only\nShould the bot buy \"Star Festival Weapon Gift I\" ?",
            false
        ),
        new Option<bool>(
            "94563",
            "Star Festival Weapon Gift II",
            "Mode: [select] only\nShould the bot buy \"Star Festival Weapon Gift II\" ?",
            false
        ),
        new Option<bool>(
            "91467",
            "Crimson StarEdge Suhail",
            "Mode: [select] only\nShould the bot buy \"Crimson StarEdge Suhail\" ?",
            false
        ),
        new Option<bool>(
            "91468",
            "Crimson StarEdges Suhail",
            "Mode: [select] only\nShould the bot buy \"Crimson StarEdges Suhail\" ?",
            false
        ),
        new Option<bool>(
            "91469",
            "Crimson NovaBlade Persei",
            "Mode: [select] only\nShould the bot buy \"Crimson NovaBlade Persei\" ?",
            false
        ),
        new Option<bool>(
            "91470",
            "Crimson NovaBlades Persei",
            "Mode: [select] only\nShould the bot buy \"Crimson NovaBlades Persei\" ?",
            false
        ),
        new Option<bool>(
            "91480",
            "Spectral StarEdge Saiph",
            "Mode: [select] only\nShould the bot buy \"Spectral StarEdge Saiph\" ?",
            false
        ),
        new Option<bool>(
            "91481",
            "Spectral StarEdges Saiph",
            "Mode: [select] only\nShould the bot buy \"Spectral StarEdges Saiph\" ?",
            false
        ),
        new Option<bool>(
            "91482",
            "Spectral NovaBlade Alnitak",
            "Mode: [select] only\nShould the bot buy \"Spectral NovaBlade Alnitak\" ?",
            false
        ),
        new Option<bool>(
            "91483",
            "Spectral NovaBlades Alnitak",
            "Mode: [select] only\nShould the bot buy \"Spectral NovaBlades Alnitak\" ?",
            false
        ),
        new Option<bool>(
            "94574",
            "Celestial Qipao",
            "Mode: [select] only\nShould the bot buy \"Celestial Qipao\" ?",
            false
        ),
    };
}
