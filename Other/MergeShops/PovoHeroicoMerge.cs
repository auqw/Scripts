/*
name: Povo Heroico Merge
description: Farms the Povo Heroico Merge [2620] in /povoheroico.
tags: povoheroico, merge, povo, heroico, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PovoHeroico
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
            "Cabelo do Meio-Dia",
            "Corrente da Liberdade",
            "Corrente do Sol",
            "Correntes da Liberdade",
            "Correntes do Sol",
            "Corte do Meio-Dia",
            "Face do Meio-Dia",
            "Maxado de Assís",
            "Moeda Real do Sol",
            "Nascer do Sol",
            "Olhar do Meio-Dia",
            "Raios de Sol",
            "Raios Fúlgidos",
            "Runa do Sol da Liberdade",
            "Símbolo do Sol da Liberdade",
            "Sol da Liberdade",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("povoheroico", 2620, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Cabelo do Meio-Dia":
                case "Corrente da Liberdade":
                case "Corrente do Sol":
                case "Correntes da Liberdade":
                case "Correntes do Sol":
                case "Corte do Meio-Dia":
                case "Face do Meio-Dia":
                case "Maxado de Assís":
                case "Moeda Real do Sol":
                case "Nascer do Sol":
                case "Olhar do Meio-Dia":
                case "Raios de Sol":
                case "Raios Fúlgidos":
                case "Runa do Sol da Liberdade":
                case "Símbolo do Sol da Liberdade":
                case "Sol da Liberdade":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("povoheroico", "Sol Da Liberdade", req.Name, quant, req.Temp);
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
        new Option<bool>("103081", "Sol Encantado da Liberdade", "Mode: [select] only\nShould the bot buy \"Sol Encantado da Liberdade\" ?", false),
        new Option<bool>("103089", "Maxado de Assís Encantado", "Mode: [select] only\nShould the bot buy \"Maxado de Assís Encantado\" ?", false),
        new Option<bool>("103086", "Raios de Sol Encantados", "Mode: [select] only\nShould the bot buy \"Raios de Sol Encantados\" ?", false),
        new Option<bool>("103087", "Símbolo Encantado do Sol da Liberdade", "Mode: [select] only\nShould the bot buy \"Símbolo Encantado do Sol da Liberdade\" ?", false),
        new Option<bool>("103093", "Correntes Encantadas da Liberdade", "Mode: [select] only\nShould the bot buy \"Correntes Encantadas da Liberdade\" ?", false),
        new Option<bool>("103095", "Correntes Encantadas do Sol", "Mode: [select] only\nShould the bot buy \"Correntes Encantadas do Sol\" ?", false),
        new Option<bool>("103090", "Raios Fúlgidos Encatados", "Mode: [select] only\nShould the bot buy \"Raios Fúlgidos Encatados\" ?", false),
        new Option<bool>("103091", "Nascer do Sol Encantado", "Mode: [select] only\nShould the bot buy \"Nascer do Sol Encantado\" ?", false),
        new Option<bool>("103082", "Olhar Encantado do Meio-Dia", "Mode: [select] only\nShould the bot buy \"Olhar Encantado do Meio-Dia\" ?", false),
        new Option<bool>("103083", "Face Encantada do Meio-Dia", "Mode: [select] only\nShould the bot buy \"Face Encantada do Meio-Dia\" ?", false),
        new Option<bool>("103084", "Cabelo Encantado do Meio-Dia", "Mode: [select] only\nShould the bot buy \"Cabelo Encantado do Meio-Dia\" ?", false),
        new Option<bool>("103085", "Corte Encantado do Meio-Dia", "Mode: [select] only\nShould the bot buy \"Corte Encantado do Meio-Dia\" ?", false),
        new Option<bool>("103092", "Corrente Encantada da Liberdade", "Mode: [select] only\nShould the bot buy \"Corrente Encantada da Liberdade\" ?", false),
        new Option<bool>("103094", "Corrente Encantada do Sol", "Mode: [select] only\nShould the bot buy \"Corrente Encantada do Sol\" ?", false),
        new Option<bool>("103088", "Runa Encantada do Sol da Liberdade", "Mode: [select] only\nShould the bot buy \"Runa Encantada do Sol da Liberdade\" ?", false),
    ];
}
