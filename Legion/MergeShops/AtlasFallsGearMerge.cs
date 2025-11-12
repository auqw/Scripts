/*
name: Atlas Falls Gear Merge
description: This bot will farm the items belonging to the selected mode for the Atlas Falls Gear Merge [2568] in /atlasfalls
tags: atlas, falls, gear, merge, atlasfalls, underworld, gatekeeper, galea, flag, sheathed, alexiares, key, territory, unconquerable, anicetus, keys, elite, lone, wolf, jagged, lupo, pet, azione, solo, legion, necromancer, cowl, sovereign, banner, crown, evanescence, hand, hands, hraesvelgr, shield, rider, horns, undead, urla, spear, halberd, ulare, guard, sworn, promised, empowered, cloak, terror, enyo, eclipse, eclipses
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Story/Legion/AtlasFalls.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AtlasFallsGearMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;

    private static AtlasFalls AF { get => _AF ??= new AtlasFalls(); set => _AF = value; }
    private static AtlasFalls _AF;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }
    private static CoreLegion _Legion;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Atlas Regalia", "Arethusa's Black Steel", "Sundered Soul of Atlas", "Atlas Crest", "Legion Token" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AF.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("atlasfalls", 2568, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Atlas Regalia":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.CheckInventory("Chaos Avenger"))
                    {
                        Core.UseBossClass();
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonsterQuest(10137, "atlasfalls", "King Zedek");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                        Core.Logger($"{req.Name} requires army, please farm it manually.", stopBot: true);
                    break;

                case "Arethusa's Black Steel":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("atlasfalls", "Princess Arethusa", req.Name, quant, false, false, true);
                    break;

                case "Sundered Soul of Atlas":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("atlasfalls", "Sundered Soul", req.Name, quant, false, false);
                    break;

                case "Atlas Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("atlaskingdom", "Atlas Light Magus", req.Name, quant, false, false);
                    break;

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("84718", "Underworld Gatekeeper", "Mode: [select] only\nShould the bot buy \"Underworld Gatekeeper\" ?", false),
        new Option<bool>("84719", "Underworld Gatekeeper Galea", "Mode: [select] only\nShould the bot buy \"Underworld Gatekeeper Galea\" ?", false),
        new Option<bool>("84720", "Underworld Gatekeeper Helm", "Mode: [select] only\nShould the bot buy \"Underworld Gatekeeper Helm\" ?", false),
        new Option<bool>("84721", "Underworld Flag", "Mode: [select] only\nShould the bot buy \"Underworld Flag\" ?", false),
        new Option<bool>("84722", "Sheathed Alexiares Key", "Mode: [select] only\nShould the bot buy \"Sheathed Alexiares Key\" ?", false),
        new Option<bool>("84723", "Underworld Territory", "Mode: [select] only\nShould the bot buy \"Underworld Territory\" ?", false),
        new Option<bool>("84724", "Unconquerable Anicetus", "Mode: [select] only\nShould the bot buy \"Unconquerable Anicetus\" ?", false),
        new Option<bool>("84725", "Dual Unconquerable Anicetus", "Mode: [select] only\nShould the bot buy \"Dual Unconquerable Anicetus\" ?", false),
        new Option<bool>("84726", "Alexiares Key", "Mode: [select] only\nShould the bot buy \"Alexiares Key\" ?", false),
        new Option<bool>("84727", "Alexiares Keys", "Mode: [select] only\nShould the bot buy \"Alexiares Keys\" ?", false),
        new Option<bool>("84728", "Anicetus and Alexiares", "Mode: [select] only\nShould the bot buy \"Anicetus and Alexiares\" ?", false),
        new Option<bool>("92786", "Atlas Elite", "Mode: [select] only\nShould the bot buy \"Atlas Elite\" ?", false),
        new Option<bool>("92787", "Atlas Elite Helm", "Mode: [select] only\nShould the bot buy \"Atlas Elite Helm\" ?", false),
        new Option<bool>("92649", "Underworld Lone Wolf", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf\" ?", false),
        new Option<bool>("92652", "Underworld Lone Wolf Mask", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Mask\" ?", false),
        new Option<bool>("92653", "Underworld Lone Wolf Hair", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Hair\" ?", false),
        new Option<bool>("92654", "Underworld Lone Wolf Locks", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Locks\" ?", false),
        new Option<bool>("92655", "Jagged Underworld Spikes", "Mode: [select] only\nShould the bot buy \"Jagged Underworld Spikes\" ?", false),
        new Option<bool>("92657", "Underworld Lupo Pet", "Mode: [select] only\nShould the bot buy \"Underworld Lupo Pet\" ?", false),
        new Option<bool>("92658", "Azione Solo", "Mode: [select] only\nShould the bot buy \"Azione Solo\" ?", false),
        new Option<bool>("92659", "Dual Azione Solo", "Mode: [select] only\nShould the bot buy \"Dual Azione Solo\" ?", false),
        new Option<bool>("92788", "Legion Necromancer", "Mode: [select] only\nShould the bot buy \"Legion Necromancer\" ?", false),
        new Option<bool>("92789", "Legion Necromancer Cowl", "Mode: [select] only\nShould the bot buy \"Legion Necromancer Cowl\" ?", false),
        new Option<bool>("92835", "Legion Sovereign", "Mode: [select] only\nShould the bot buy \"Legion Sovereign\" ?", false),
        new Option<bool>("92837", "Legion Sovereign Helm", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Helm\" ?", false),
        new Option<bool>("92839", "Legion Sovereign Locks", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Locks\" ?", false),
        new Option<bool>("92841", "Legion Sovereign Banner", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Banner\" ?", false),
        new Option<bool>("92843", "Legion Sovereign Crown", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Crown\" ?", false),
        new Option<bool>("92844", "Evanescence", "Mode: [select] only\nShould the bot buy \"Evanescence\" ?", false),
        new Option<bool>("92845", "Dual Evanescence", "Mode: [select] only\nShould the bot buy \"Dual Evanescence\" ?", false),
        new Option<bool>("92849", "Hand of the Legion Sovereign", "Mode: [select] only\nShould the bot buy \"Hand of the Legion Sovereign\" ?", false),
        new Option<bool>("92850", "Hands of the Legion Sovereign", "Mode: [select] only\nShould the bot buy \"Hands of the Legion Sovereign\" ?", false),
        new Option<bool>("92231", "Hraesvelgr", "Mode: [select] only\nShould the bot buy \"Hraesvelgr\" ?", false),
        new Option<bool>("92232", "Dual Hraesvelgr", "Mode: [select] only\nShould the bot buy \"Dual Hraesvelgr\" ?", false),
        new Option<bool>("92237", "Hraesvelgr Shield", "Mode: [select] only\nShould the bot buy \"Hraesvelgr Shield\" ?", false),
        new Option<bool>("93078", "Underworld Lone Wolf Rider", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Rider\" ?", false),
        new Option<bool>("92651", "Underworld Lone Wolf Horns", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Horns\" ?", false),
        new Option<bool>("92663", "Undead Urla Spear", "Mode: [select] only\nShould the bot buy \"Undead Urla Spear\" ?", false),
        new Option<bool>("92664", "Halberd of Ulare", "Mode: [select] only\nShould the bot buy \"Halberd of Ulare\" ?", false),
        new Option<bool>("92665", "Underworld Lone Wolf Guard", "Mode: [select] only\nShould the bot buy \"Underworld Lone Wolf Guard\" ?", false),
        new Option<bool>("92834", "Sworn Legion Sovereign", "Mode: [select] only\nShould the bot buy \"Sworn Legion Sovereign\" ?", false),
        new Option<bool>("92836", "Promised Legion Sovereign", "Mode: [select] only\nShould the bot buy \"Promised Legion Sovereign\" ?", false),
        new Option<bool>("92838", "Legion Sovereign Horns", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Horns\" ?", false),
        new Option<bool>("92840", "Empowered Legion Sovereign Locks", "Mode: [select] only\nShould the bot buy \"Empowered Legion Sovereign Locks\" ?", false),
        new Option<bool>("92842", "Legion Sovereign Cloak", "Mode: [select] only\nShould the bot buy \"Legion Sovereign Cloak\" ?", false),
        new Option<bool>("92846", "Terror of Enyo", "Mode: [select] only\nShould the bot buy \"Terror of Enyo\" ?", false),
        new Option<bool>("92847", "Eclipse of Enyo", "Mode: [select] only\nShould the bot buy \"Eclipse of Enyo\" ?", false),
        new Option<bool>("92848", "Eclipses of Enyo", "Mode: [select] only\nShould the bot buy \"Eclipses of Enyo\" ?", false),
    };
}
