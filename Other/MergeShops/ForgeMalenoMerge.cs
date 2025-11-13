/*
name: Forge Maleno Merge
description: This bot will farm the items belonging to the selected mode for the Forge Maleno Merge [2615] in /mountmaleno
tags: forge, maleno, merge, mountmaleno, idalions, shadowbrand, flame, halberd, shadowcleaver, shadowcleavers, drow, oathkeeper, morph, silver, saber, sabers, blazing, fang, fangs
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ForgeMalenoMerge
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

    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

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
                "Yew Ember",
                "Black Flame of Maleno",
                "Shadowbrand's Edge",
                "Shadowbrand's Edges",
                "Blackfire Halberd",
                "Nightcleaver",
                "Nightcleavers",
                "Maleno Obsidian",
                "Drow Silver",
                "Maleno's Fang",
                "Maleno's Fangs",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.MountMaleno();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("mountmaleno", 2615, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Black Flame of Maleno":
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
                            Core.IsMember ? 10370 : 10369,
                            ("mountmaleno", "Draconian Bandit", ClassType.Farm),
                            ("mountmaleno", "Maleno Elemental", ClassType.Solo),
                            ("mountmaleno", "Idalion", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Shadowbrand's Edge":
                case "Shadowbrand's Edges":
                case "Blackfire Halberd":
                case "Nightcleaver":
                case "Nightcleavers":
                case "Maleno's Fang":
                case "Maleno's Fangs":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("mountmaleno", "Idalion", req.Name, quant, false, false);
                    break;

                #endregion

                #region Known items

                case "Yew Ember":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Maleno Match", req.Name, quant, false, false);
                    break;

                case "Maleno Obsidian":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "thelimacity",
                        "Maleno Elemental",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Drow Silver":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Drow Soldier", req.Name, quant, false, false);
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92799",
            "Idalion's Shadowbrand Blade",
            "Mode: [select] only\nShould the bot buy \"Idalion's Shadowbrand Blade\" ?",
            false
        ),
        new Option<bool>(
            "92800",
            "Idalion's Shadowbrand Blades",
            "Mode: [select] only\nShould the bot buy \"Idalion's Shadowbrand Blades\" ?",
            false
        ),
        new Option<bool>(
            "92810",
            "Idalion's Flame Halberd",
            "Mode: [select] only\nShould the bot buy \"Idalion's Flame Halberd\" ?",
            false
        ),
        new Option<bool>(
            "92814",
            "Idalion's ShadowCleaver",
            "Mode: [select] only\nShould the bot buy \"Idalion's ShadowCleaver\" ?",
            false
        ),
        new Option<bool>(
            "92815",
            "Idalion's ShadowCleavers",
            "Mode: [select] only\nShould the bot buy \"Idalion's ShadowCleavers\" ?",
            false
        ),
        new Option<bool>(
            "94685",
            "Drow Oathkeeper",
            "Mode: [select] only\nShould the bot buy \"Drow Oathkeeper\" ?",
            false
        ),
        new Option<bool>(
            "94686",
            "Drow Oathkeeper Morph",
            "Mode: [select] only\nShould the bot buy \"Drow Oathkeeper Morph\" ?",
            false
        ),
        new Option<bool>(
            "94687",
            "Drow Oathkeeper Visage",
            "Mode: [select] only\nShould the bot buy \"Drow Oathkeeper Visage\" ?",
            false
        ),
        new Option<bool>(
            "94688",
            "Silver Drow Saber",
            "Mode: [select] only\nShould the bot buy \"Silver Drow Saber\" ?",
            false
        ),
        new Option<bool>(
            "94689",
            "Silver Drow Sabers",
            "Mode: [select] only\nShould the bot buy \"Silver Drow Sabers\" ?",
            false
        ),
        new Option<bool>(
            "95110",
            "Idalion's Blazing Fang",
            "Mode: [select] only\nShould the bot buy \"Idalion's Blazing Fang\" ?",
            false
        ),
        new Option<bool>(
            "95111",
            "Idalion's Blazing Fangs",
            "Mode: [select] only\nShould the bot buy \"Idalion's Blazing Fangs\" ?",
            false
        ),
    };
}
