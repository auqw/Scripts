/*
name: Legion Tournament Rewards Merge
description: This bot will farm the items belonging to the selected mode for the Legion Tournament Rewards Merge [2685] in /legiontournament
tags: legion, tournament, rewards, merge, legiontournament, underworld, proioxis, spiked, baetyl, ostrakon, shield, ker, berethrou, ritual, siculi, sacrificial, scythian, bow, arrows, pursuit, cloak, executioner, skull
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/CoreDageBirthday.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LegionTournamentRewardsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }
    private static CoreLegion _Legion;
    private static CoreDageBirthday Dage { get => _Dage ??= new CoreDageBirthday(); set => _Dage = value; }
    private static CoreDageBirthday _Dage;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "WarForge Coal", "Dark Ostrakon", "Legion Token", "Proioxis Pursuit Cloak" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Dage.LegionTournament();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("legiontournament", 2685, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Dark Ostrakon":
                case "Proioxis Pursuit Cloak":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("legiontournament", "Dark Ostrakon", req.Name, quant, req.Temp, false);
                    break;


                case "WarForge Coal":
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
                        Core.HuntMonsterQuest(Core.IsMember ? 10636 : 10635,
                        ("legiontournament", "Legion Ritualist", ClassType.Solo),
                        ("legiontournament", "The WarForge", ClassType.Solo),
                        ("legiontournament", "Deathwing", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
                #endregion

                #region Known items

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("95589", "Underworld Proioxis", "Mode: [select] only\nShould the bot buy \"Underworld Proioxis\" ?", false),
        new Option<bool>("95590", "Underworld Proioxis Hair", "Mode: [select] only\nShould the bot buy \"Underworld Proioxis Hair\" ?", false),
        new Option<bool>("95591", "Underworld Proioxis Visage", "Mode: [select] only\nShould the bot buy \"Underworld Proioxis Visage\" ?", false),
        new Option<bool>("95597", "Spiked Baetyl", "Mode: [select] only\nShould the bot buy \"Spiked Baetyl\" ?", false),
        new Option<bool>("95598", "Dual Spiked Baetyl", "Mode: [select] only\nShould the bot buy \"Dual Spiked Baetyl\" ?", false),
        new Option<bool>("95600", "Ostrakon and Shield", "Mode: [select] only\nShould the bot buy \"Ostrakon and Shield\" ?", false),
        new Option<bool>("99716", "Ker Berethrou", "Mode: [select] only\nShould the bot buy \"Ker Berethrou\" ?", false),
        new Option<bool>("99717", "Dual Ker Berethrou", "Mode: [select] only\nShould the bot buy \"Dual Ker Berethrou\" ?", false),
        new Option<bool>("99718", "Ritual Siculi", "Mode: [select] only\nShould the bot buy \"Ritual Siculi\" ?", false),
        new Option<bool>("99720", "Sacrificial Siculi", "Mode: [select] only\nShould the bot buy \"Sacrificial Siculi\" ?", false),
        new Option<bool>("99722", "Scythian Bow and Arrows", "Mode: [select] only\nShould the bot buy \"Scythian Bow and Arrows\" ?", false),
        new Option<bool>("99723", "Scythian Bow", "Mode: [select] only\nShould the bot buy \"Scythian Bow\" ?", false),
        new Option<bool>("95594", "Proioxis Pursuit Cloak", "Mode: [select] only\nShould the bot buy \"Proioxis Pursuit Cloak\" ?", false),
        new Option<bool>("99670", "Underworld Executioner Skull Mask", "Mode: [select] only\nShould the bot buy \"Underworld Executioner Skull Mask\" ?", false),
   };
}
