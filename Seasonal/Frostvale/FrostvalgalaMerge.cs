/*
name: Frostvalgala Merge
description: This bot will farm the items belonging to the selected mode for the Frostvalgala Merge [2645] in /frostvalgala
tags: frostvalgala, merge, frostvalgala, golden, granville, knight, noble, red, sash, carteret, spear, shield, stella, dei, miracoli, royal, frostval, tree, stelle, invernali, sanctolina, halberd, mace, lucevan, le
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FrostvalgalaMerge
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
    private static CoreFrostvale Frost
    {
        get => _Frost ??= new CoreFrostvale();
        set => _Frost = value;
    }
    private static CoreFrostvale _Frost;

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
                "Whistler Bullion",
                "Granville Knight",
                "Granville Helm",
                "Noble Blue Sash",
                "Silver Carteret Spear",
                "Silver Carteret Spear and Shield",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Frost.FrostvalGala();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("frostvalgala", 2645, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Whistler Bullion":
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
                            Core.IsMember ? 10525 : 10524,
                            ("frostvalgala", "Unsung Queen", ClassType.Solo),
                            ("frostvalgala", "Unsung Knight", ClassType.Farm),
                            ("frostvalgala", "Unsung Beast", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Granville Knight":
                case "Granville Helm":
                case "Noble Blue Sash":
                case "Silver Carteret Spear":
                case "Silver Carteret Spear and Shield":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "frostvalgala",
                        "Unsung Queen",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "97499",
            "Golden Granville Knight",
            "Mode: [select] only\nShould the bot buy \"Golden Granville Knight\" ?",
            false
        ),
        new Option<bool>(
            "97500",
            "Golden Granville Helm",
            "Mode: [select] only\nShould the bot buy \"Golden Granville Helm\" ?",
            false
        ),
        new Option<bool>(
            "97501",
            "Noble Red Sash",
            "Mode: [select] only\nShould the bot buy \"Noble Red Sash\" ?",
            false
        ),
        new Option<bool>(
            "97502",
            "Golden Carteret Spear",
            "Mode: [select] only\nShould the bot buy \"Golden Carteret Spear\" ?",
            false
        ),
        new Option<bool>(
            "97503",
            "Golden Carteret Spear and Shield",
            "Mode: [select] only\nShould the bot buy \"Golden Carteret Spear and Shield\" ?",
            false
        ),
        new Option<bool>(
            "97585",
            "Stella dei Miracoli",
            "Mode: [select] only\nShould the bot buy \"Stella dei Miracoli\" ?",
            false
        ),
        new Option<bool>(
            "97586",
            "Dual Stella dei Miracoli",
            "Mode: [select] only\nShould the bot buy \"Dual Stella dei Miracoli\" ?",
            false
        ),
        new Option<bool>(
            "97735",
            "Royal Frostval Tree",
            "Mode: [select] only\nShould the bot buy \"Royal Frostval Tree\" ?",
            false
        ),
        new Option<bool>(
            "97739",
            "Stelle Invernali",
            "Mode: [select] only\nShould the bot buy \"Stelle Invernali\" ?",
            false
        ),
        new Option<bool>(
            "97740",
            "Dual Stelle Invernali",
            "Mode: [select] only\nShould the bot buy \"Dual Stelle Invernali\" ?",
            false
        ),
        new Option<bool>(
            "97602",
            "Sanctolina Staff",
            "Mode: [select] only\nShould the bot buy \"Sanctolina Staff\" ?",
            false
        ),
        new Option<bool>(
            "97604",
            "Sanctolina Halberd",
            "Mode: [select] only\nShould the bot buy \"Sanctolina Halberd\" ?",
            false
        ),
        new Option<bool>(
            "97607",
            "Sanctolina Mace",
            "Mode: [select] only\nShould the bot buy \"Sanctolina Mace\" ?",
            false
        ),
        new Option<bool>(
            "97609",
            "Sanctolina Axe",
            "Mode: [select] only\nShould the bot buy \"Sanctolina Axe\" ?",
            false
        ),
        new Option<bool>(
            "97742",
            "Lucevan Le Stelle",
            "Mode: [select] only\nShould the bot buy \"Lucevan Le Stelle\" ?",
            false
        ),
        new Option<bool>(
            "97743",
            "Dual Lucevan Le Stelle",
            "Mode: [select] only\nShould the bot buy \"Dual Lucevan Le Stelle\" ?",
            false
        ),
    };
}
