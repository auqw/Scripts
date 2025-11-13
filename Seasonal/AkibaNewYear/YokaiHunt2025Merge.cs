/*
name: Yokai Hunt 2025 Merge
description: This bot will farm the items belonging to the selected mode for the Yokai Hunt 2025 Merge [2559] in /yokaihunt
tags: yokai, hunt, 2025, merge, yokaihunt, serpents, refinement, fans, fan, silver, serpent, qi, pao, favor, gold, rap, artist, urban, cap, morph, glasses, streetwear, royal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/AkibaNewYear/YokaiHunt.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class YokaiHunt2025Merge
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

    private static YokaiHunt YH
    {
        get => _YH ??= new YokaiHunt();
        set => _YH = value;
    }
    private static YokaiHunt _YH;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Miko's Blessing",
                "Pearlescent Scale",
                "Urban Serpent Cap + Glasses",
                "Urban Serpent Hat + Glasses",
                "Urban Serpent Locks + Glasses",
                "Urban Serpent Hair + Glasses",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        YH.AiNoMiko2();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("yokaihunt", 2559, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Miko's Blessing":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10059,
                            ("shogunwar", "Shadow Samurai", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Pearlescent Scale":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10060, ("yokaihunt", "Zhenzhu Shé", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Urban Serpent Cap + Glasses":
                case "Urban Serpent Hat + Glasses":
                case "Urban Serpent Locks + Glasses":
                case "Urban Serpent Hair + Glasses":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("yokaihunt", "Zhenzhu Shé", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "91791",
            "Serpent's Refinement Fans",
            "Mode: [select] only\nShould the bot buy \"Serpent's Refinement Fans\" ?",
            false
        ),
        new Option<bool>(
            "91790",
            "Serpent's Refinement Fan",
            "Mode: [select] only\nShould the bot buy \"Serpent's Refinement Fan\" ?",
            false
        ),
        new Option<bool>(
            "91789",
            "Silver Serpent Qi Pao",
            "Mode: [select] only\nShould the bot buy \"Silver Serpent Qi Pao\" ?",
            false
        ),
        new Option<bool>(
            "91788",
            "Serpent's Favor Fans",
            "Mode: [select] only\nShould the bot buy \"Serpent's Favor Fans\" ?",
            false
        ),
        new Option<bool>(
            "91787",
            "Serpent's Favor Fan",
            "Mode: [select] only\nShould the bot buy \"Serpent's Favor Fan\" ?",
            false
        ),
        new Option<bool>(
            "91786",
            "Gold Serpent Qi Pao",
            "Mode: [select] only\nShould the bot buy \"Gold Serpent Qi Pao\" ?",
            false
        ),
        new Option<bool>(
            "91738",
            "Serpent Rap Artist",
            "Mode: [select] only\nShould the bot buy \"Serpent Rap Artist\" ?",
            false
        ),
        new Option<bool>(
            "91724",
            "Urban Serpent Cap Visage",
            "Mode: [select] only\nShould the bot buy \"Urban Serpent Cap Visage\" ?",
            false
        ),
        new Option<bool>(
            "91723",
            "Urban Serpent Hat Morph",
            "Mode: [select] only\nShould the bot buy \"Urban Serpent Hat Morph\" ?",
            false
        ),
        new Option<bool>(
            "91722",
            "Urban Serpent Glasses Visage",
            "Mode: [select] only\nShould the bot buy \"Urban Serpent Glasses Visage\" ?",
            false
        ),
        new Option<bool>(
            "91721",
            "Urban Serpent Glasses Morph",
            "Mode: [select] only\nShould the bot buy \"Urban Serpent Glasses Morph\" ?",
            false
        ),
        new Option<bool>(
            "91716",
            "Urban Serpent Streetwear",
            "Mode: [select] only\nShould the bot buy \"Urban Serpent Streetwear\" ?",
            false
        ),
        new Option<bool>(
            "91715",
            "Urban Royal Streetwear",
            "Mode: [select] only\nShould the bot buy \"Urban Royal Streetwear\" ?",
            false
        ),
    };
}
