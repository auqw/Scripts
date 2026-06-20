/*
name: Nova Omiyage Merge
description: This bot will farm the items belonging to the selected mode for the Nova Omiyage Merge [2735] in /vermillioncliffs
tags: nova, omiyage, merge, vermillioncliffs, stardiviner, suzaku, constellation, mystique, house, guard, winged
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NovaOmiyageMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;

    private static CoreOasis COA
    {
        get => _COA ??= new CoreOasis();
        set => _COA = value;
    }
    private static CoreOasis _COA;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Suzaku's Stardust" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        COA.VermillionCliffs();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("vermillioncliffs", 2735, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
        #endregion

                #region Items not setup

                case "v":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(!string.IsNullOrEmpty(Core.BossClass) && System.Enum.TryParse(Core.BossClass, true, out ClassType bossClass) ? bossClass : ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10775 : 10773);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("vermillioncliffs", "Vermillion Phoenix", "Phoenix Drumstick", 18);
                        Core.HuntMonster("vermillioncliffs", "Suzaku", "Suzaku's Beak");
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
        new Option<bool>("101609", "StarDiviner of Suzaku", "Mode: [select] only\nShould the bot buy \"StarDiviner of Suzaku\" ?", false),
        new Option<bool>("101612", "Constellation of Suzaku", "Mode: [select] only\nShould the bot buy \"Constellation of Suzaku\" ?", false),
        new Option<bool>("101613", "Suzaku StarDiviner Cape", "Mode: [select] only\nShould the bot buy \"Suzaku StarDiviner Cape\" ?", false),
        new Option<bool>("101614", "Suzaku StarDiviner Mystique", "Mode: [select] only\nShould the bot buy \"Suzaku StarDiviner Mystique\" ?", false),
        new Option<bool>("101615", "Suzaku StarDiviner Aura", "Mode: [select] only\nShould the bot buy \"Suzaku StarDiviner Aura\" ?", false),
        new Option<bool>("101624", "House Guard Suzaku", "Mode: [select] only\nShould the bot buy \"House Guard Suzaku\" ?", false),
        new Option<bool>("101610", "Suzaku StarDiviner Hair", "Mode: [select] only\nShould the bot buy \"Suzaku StarDiviner Hair\" ?", false),
        new Option<bool>("101611", "Suzaku StarDiviner Locks", "Mode: [select] only\nShould the bot buy \"Suzaku StarDiviner Locks\" ?", false),
        new Option<bool>("101616", "Winged Suzaku Staff", "Mode: [select] only\nShould the bot buy \"Winged Suzaku Staff\" ?", false),
   };
}
