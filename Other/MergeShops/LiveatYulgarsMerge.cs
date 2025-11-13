/*
name: Live at Yulgars Merge
description: This bot will farm the items belonging to the selected mode for the Live at Yulgars Merge [377] in /yulgar
tags: live, at, yulgars, merge, yulgar, both, directions, groupie, trollok, fan, shirt, blue, hero, , cordless, mic, vinyl, record, platinum, wreckord, stand, dread, trolluk, azalea, rocks, guy
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LiveatYulgarsMerge
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
        Core.BankingBlackList.AddRange(new[] { "Platinum Album Shard" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("yulgar", 377, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Platinum Album Shard":
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
                            4199,
                            ("boxes", "Sneevil Boxer", ClassType.Farm),
                            ("greenguardwest", "Slime", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "29595",
            "Both Directions Groupie",
            "Mode: [select] only\nShould the bot buy \"Both Directions Groupie\" ?",
            false
        ),
        new Option<bool>(
            "29616",
            "Trollok Fan Shirt",
            "Mode: [select] only\nShould the bot buy \"Trollok Fan Shirt\" ?",
            false
        ),
        new Option<bool>(
            "29617",
            "Blue Hero",
            "Mode: [select] only\nShould the bot buy \"Blue Hero\" ?",
            false
        ),
        new Option<bool>(
            "29621",
            "Axe Axe +15",
            "Mode: [select] only\nShould the bot buy \"Axe Axe +15\" ?",
            false
        ),
        new Option<bool>(
            "29622",
            "Cordless Mic",
            "Mode: [select] only\nShould the bot buy \"Cordless Mic\" ?",
            false
        ),
        new Option<bool>(
            "29623",
            "Vinyl Record",
            "Mode: [select] only\nShould the bot buy \"Vinyl Record\" ?",
            false
        ),
        new Option<bool>(
            "29624",
            "Platinum Record",
            "Mode: [select] only\nShould the bot buy \"Platinum Record\" ?",
            false
        ),
        new Option<bool>(
            "29625",
            "Wreckord Stand",
            "Mode: [select] only\nShould the bot buy \"Wreckord Stand\" ?",
            false
        ),
        new Option<bool>(
            "29626",
            "Dread Trolluk",
            "Mode: [select] only\nShould the bot buy \"Dread Trolluk\" ?",
            false
        ),
        new Option<bool>(
            "29627",
            "Blue Hero Mask",
            "Mode: [select] only\nShould the bot buy \"Blue Hero Mask\" ?",
            false
        ),
        new Option<bool>(
            "29628",
            "Azalea Rocks",
            "Mode: [select] only\nShould the bot buy \"Azalea Rocks\" ?",
            false
        ),
        new Option<bool>(
            "29629",
            "Azalea Guy",
            "Mode: [select] only\nShould the bot buy \"Azalea Guy\" ?",
            false
        ),
    };
}
