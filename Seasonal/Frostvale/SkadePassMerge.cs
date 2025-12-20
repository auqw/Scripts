/*
name: SkadePass Merge
description: This bot will farm the items belonging to the selected mode for the SkadePass Merge [2642] in /skadespass
tags: skadepass, merge, skadespass, frostborne, dragonslayer, cloak, dragonblade, shield, luminary, scythe, skade, skades, winged, snowpiercer, dragonblades, permafrost, juggernaut, snowsaga
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SkadePassMerge
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
                "Icy Bone",
                "Skade's Snowpiercer",
                "Permafrost Heart",
                "Frostborne Dragonslayer Helm",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("skadespass", 2642, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Icy Bone":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            Core.IsMember ? 10513 : 10512,
                            "skadespass",
                            "Permafrost Dragon"
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Skade's Snowpiercer":
                case "Permafrost Heart":
                case "Frostborne Dragonslayer Helm":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "skadespass",
                        "Permafrost Dragon",
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
            "97438",
            "Frostborne Dragonslayer",
            "Mode: [select] only\nShould the bot buy \"Frostborne Dragonslayer\" ?",
            false
        ),
        new Option<bool>(
            "97441",
            "Frostborne Dragonslayer Cloak",
            "Mode: [select] only\nShould the bot buy \"Frostborne Dragonslayer Cloak\" ?",
            false
        ),
        new Option<bool>(
            "97447",
            "Frostborne DragonBlade and Shield",
            "Mode: [select] only\nShould the bot buy \"Frostborne DragonBlade and Shield\" ?",
            false
        ),
        new Option<bool>(
            "97439",
            "Frostborne Luminary",
            "Mode: [select] only\nShould the bot buy \"Frostborne Luminary\" ?",
            false
        ),
        new Option<bool>(
            "97442",
            "Frostborne Scythe of Skade",
            "Mode: [select] only\nShould the bot buy \"Frostborne Scythe of Skade\" ?",
            false
        ),
        new Option<bool>(
            "97446",
            "Skade's Winged Snowpiercer",
            "Mode: [select] only\nShould the bot buy \"Skade's Winged Snowpiercer\" ?",
            false
        ),
        new Option<bool>(
            "97443",
            "Frostborne DragonBlade",
            "Mode: [select] only\nShould the bot buy \"Frostborne DragonBlade\" ?",
            false
        ),
        new Option<bool>(
            "97444",
            "Frostborne DragonBlades",
            "Mode: [select] only\nShould the bot buy \"Frostborne DragonBlades\" ?",
            false
        ),
        new Option<bool>(
            "97448",
            "Permafrost Dragonslayer",
            "Mode: [select] only\nShould the bot buy \"Permafrost Dragonslayer\" ?",
            false
        ),
        new Option<bool>(
            "97449",
            "Permafrost Juggernaut",
            "Mode: [select] only\nShould the bot buy \"Permafrost Juggernaut\" ?",
            false
        ),
        new Option<bool>(
            "97450",
            "Permafrost Dragonslayer Helm",
            "Mode: [select] only\nShould the bot buy \"Permafrost Dragonslayer Helm\" ?",
            false
        ),
        new Option<bool>(
            "97451",
            "Permafrost Dragonslayer Cloak",
            "Mode: [select] only\nShould the bot buy \"Permafrost Dragonslayer Cloak\" ?",
            false
        ),
        new Option<bool>(
            "97452",
            "Permafrost Scythe of Skade",
            "Mode: [select] only\nShould the bot buy \"Permafrost Scythe of Skade\" ?",
            false
        ),
        new Option<bool>(
            "97453",
            "Permafrost DragonBlade",
            "Mode: [select] only\nShould the bot buy \"Permafrost DragonBlade\" ?",
            false
        ),
        new Option<bool>(
            "97454",
            "Permafrost DragonBlades",
            "Mode: [select] only\nShould the bot buy \"Permafrost DragonBlades\" ?",
            false
        ),
        new Option<bool>(
            "97455",
            "Skade's Snowsaga",
            "Mode: [select] only\nShould the bot buy \"Skade's Snowsaga\" ?",
            false
        ),
        new Option<bool>(
            "97456",
            "Skade's Winged Snowsaga",
            "Mode: [select] only\nShould the bot buy \"Skade's Winged Snowsaga\" ?",
            false
        ),
        new Option<bool>(
            "97457",
            "Permafrost DragonBlade and Shield",
            "Mode: [select] only\nShould the bot buy \"Permafrost DragonBlade and Shield\" ?",
            false
        ),
    };
}
