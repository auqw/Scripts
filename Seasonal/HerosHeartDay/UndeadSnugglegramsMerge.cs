/*
name: Undead Snugglegrams Merge
description: This bot will farm the items belonging to the selected mode for the Undead Snugglegrams Merge [2682] in /heartsdaygrave
tags: undead, snugglegrams, merge, heartsdaygrave, swordhaven, pendragon, noble, saber, pure, nobility, emblem, shield, promised, victory, knighthood, conferment, shadowscythe, visionary, plume, tarnished, unwavering, spite, condemnation, doomknight, tyrant, plate, collar, oppression, darkness, doomed, tyrannical
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/HerosHeartDay/heartsdaygrave.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UndeadSnugglegramsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;

    private static heartsdaygrave heartsdaygrave { get => _heartsdaygrave ??= new heartsdaygrave(); set => _heartsdaygrave = value; }
    private static heartsdaygrave _heartsdaygrave;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Heart of Steel", "Doomknight Tyrant Helm", "Doomknight Tyrant Skull", "Ether of Darkness" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        heartsdaygrave.DoStory();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("heartsdaygrave", 2682, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Heart of Steel":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(!Core.IsMember ? 10602 : 10603);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("heartsdaygrave", "Illusionist Zio", "Vaioh's Snugglegram");
                        Core.HuntMonster("heartsdaygrave", "Warlord Vaioh", "Zio's Discount Chocolate");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Doomknight Tyrant Skull":
                case "Doomknight Tyrant Helm":
                case "Ether of Darkness":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("heartsdaygrave", "Warlord Vaioh", req.Name, req.Quantity, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;

                case "Gold Voucher 25k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("99108", "Swordhaven Pendragon", "Mode: [select] only\nShould the bot buy \"Swordhaven Pendragon\" ?", false),
        new Option<bool>("99109", "Noble Swordhaven Pendragon", "Mode: [select] only\nShould the bot buy \"Noble Swordhaven Pendragon\" ?", false),
        new Option<bool>("99112", "Noble Saber Hair", "Mode: [select] only\nShould the bot buy \"Noble Saber Hair\" ?", false),
        new Option<bool>("99113", "Noble Saber Locks", "Mode: [select] only\nShould the bot buy \"Noble Saber Locks\" ?", false),
        new Option<bool>("99117", "Pure Nobility Cape", "Mode: [select] only\nShould the bot buy \"Pure Nobility Cape\" ?", false),
        new Option<bool>("99118", "Swordhaven Emblem Shield", "Mode: [select] only\nShould the bot buy \"Swordhaven Emblem Shield\" ?", false),
        new Option<bool>("99120", "Sword of Promised Victory", "Mode: [select] only\nShould the bot buy \"Sword of Promised Victory\" ?", false),
        new Option<bool>("99122", "Knighthood Conferment", "Mode: [select] only\nShould the bot buy \"Knighthood Conferment\" ?", false),
        new Option<bool>("99125", "Shadowscythe Visionary", "Mode: [select] only\nShould the bot buy \"Shadowscythe Visionary\" ?", false),
        new Option<bool>("99126", "Noble Shadowscythe Visionary", "Mode: [select] only\nShould the bot buy \"Noble Shadowscythe Visionary\" ?", false),
        new Option<bool>("99129", "Shadowscythe Visionary Helm", "Mode: [select] only\nShould the bot buy \"Shadowscythe Visionary Helm\" ?", false),
        new Option<bool>("99131", "Shadowscythe Visionary Plume", "Mode: [select] only\nShould the bot buy \"Shadowscythe Visionary Plume\" ?", false),
        new Option<bool>("99132", "Tarnished Nobility Cape", "Mode: [select] only\nShould the bot buy \"Tarnished Nobility Cape\" ?", false),
        new Option<bool>("99133", "Shadowscythe Emblem Shield", "Mode: [select] only\nShould the bot buy \"Shadowscythe Emblem Shield\" ?", false),
        new Option<bool>("99135", "Sword of Unwavering Spite", "Mode: [select] only\nShould the bot buy \"Sword of Unwavering Spite\" ?", false),
        new Option<bool>("99137", "Knighthood Condemnation", "Mode: [select] only\nShould the bot buy \"Knighthood Condemnation\" ?", false),
        new Option<bool>("99327", "Doomknight Tyrant", "Mode: [select] only\nShould the bot buy \"Doomknight Tyrant\" ?", false),
        new Option<bool>("99329", "Doomknight Tyrant Plate", "Mode: [select] only\nShould the bot buy \"Doomknight Tyrant Plate\" ?", false),
        new Option<bool>("99333", "Doomknight Tyrant Collar", "Mode: [select] only\nShould the bot buy \"Doomknight Tyrant Collar\" ?", false),
        new Option<bool>("99337", "Oppression of Darkness", "Mode: [select] only\nShould the bot buy \"Oppression of Darkness\" ?", false),
        new Option<bool>("99339", "Doomed Tyrannical Gauntlets", "Mode: [select] only\nShould the bot buy \"Doomed Tyrannical Gauntlets\" ?", false),
        new Option<bool>("99340", "Tyrannical Shield Gauntlets", "Mode: [select] only\nShould the bot buy \"Tyrannical Shield Gauntlets\" ?", false),
   };
}
