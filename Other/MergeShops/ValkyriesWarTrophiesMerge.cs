/*
name: Valkyries War Trophies Merge
description: This bot will farm the items belonging to the selected mode for the Valkyries War Trophies Merge [2737] in /carcossacourt
tags: valkyries, war, trophies, merge, carcossacourt, corrupt, shadowreaper, doom, unreal, dark, carcossa, doomed, dulcinea, twin, locked, chest, ii, i, iii, iv, winged, weapon, shop, note, scythe, skullstaff
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
//cs_include Scripts/Other/MergeShops/DoomMerge.cs
//cs_include Scripts/Other/MergeShops/MirrorRealmMerge.cs
//cs_include Scripts/Seasonal/Friday13th/MergeShops/ShadowMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ValkyriesWarTrophiesMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static SRoD SRoD
    {
        get => _SRoD ??= new SRoD();
        set => _SRoD = value;
    }
    private static SRoD _SRoD;
    private static CoreSDKA SDKA
    {
        get => _SDKA ??= new CoreSDKA();
        set => _SDKA = value;
    }
    private static CoreSDKA _SDKA;
    private static MirrorRealmMerge MRM
    {
        get => _MRM ??= new MirrorRealmMerge();
        set => _MRM = value;
    }
    private static MirrorRealmMerge _MRM;
    private static ShadowMerge ShadowMerge
    {
        get => _ShadowMerge ??= new ShadowMerge();
        set => _ShadowMerge = value;
    }
    private static ShadowMerge _ShadowMerge;
    private static CoreOasis Oasis
    {
        get => _Oasis ??= new CoreOasis();
        set => _Oasis = value;
    }
    private static CoreOasis _Oasis;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "A Revelation", "Fabled Doom Blade", "ShadowReaper Of Doom", "Doom Aura", "True Doomknight Helm", "Twin Blades of Doom", "SkullStaff of Doom" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {

        // ValkyriesWarTrophiesMerge
        // Adv.MergeItemisinShopExceptions =
        // [
        //     "item",
        //     "item2",
        //     "item3"
        // ];

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("carcossacourt", 2737, findIngredients, buyOnlyThis, buyMode: buyMode);

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


                case "A Revelation":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10778, 10779);
                    Core.KillMonster("carcossacourt", "r5", "Left", "*", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;


                case "Doom Aura":
                    Dictionary<string, int> ptp = new()
                        {
                            { "Necrotic Scythe of Scourge", 2184 },
                            { "Necrotic Daggers of Destruction", 2181 },
                            { "Necrotic Shade Blade", 2182 },
                            { "Necrotic Broadsword of Bane", 2183 },
                            { "Necrotic Mace of Misery", 2185 },
                            { "Necrotic Bow of the Shadow", 2186 }
                        };
                    string? weapon = null;

                    foreach (string item in ptp.Keys)
                    {
                        if (!Core.CheckInventory(item))
                            continue;

                        weapon = item;
                        break;
                    }
                    if (weapon == null)
                    {
                        Core.Logger($"Missing any valid Necrotic weapon to acquire {req.Name}");
                        return;
                    }

                    // Uses the first owned weapon found.
                    SDKA.PinpointthePieces(
                        ptp[weapon],
                        new[] { "Doom Aura" },
                        new[] { req.Quantity }
                    );
                    break;


                case "Fabled Doom Blade":
                    ShadowMerge.BuyAllMerge(req.Name);
                    break;


                case "True Doomknight Helm":
                    Adv.BuyItem("terminatemple", 2343, req.Name);
                    break;


                case "Twin Blades of Doom":
                case "SkullStaff of Doom":
                    MRM.BuyAllMerge(req.Name);
                    break;


                case "ShadowReaper Of Doom":
                    SRoD.ShadowReaperOfDoom();
                    break;

            }
        }
    }

    public List<IOption> Select =
  [
      new Option<bool>("101521", "Valkyrie of Doom", "Mode: [select] only\nShould the bot buy \"Valkyrie of Doom\" ?", false),
        new Option<bool>("101755", "Corrupt ShadowReaper of Doom", "Mode: [select] only\nShould the bot buy \"Corrupt ShadowReaper of Doom\" ?", false),
        new Option<bool>("101756", "Unreal ShadowReaper of Doom", "Mode: [select] only\nShould the bot buy \"Unreal ShadowReaper of Doom\" ?", false),
        new Option<bool>("101526", "Valkyrie's Concordia Wings", "Mode: [select] only\nShould the bot buy \"Valkyrie's Concordia Wings\" ?", false),
        new Option<bool>("101527", "Valkyrie's Doom Wings", "Mode: [select] only\nShould the bot buy \"Valkyrie's Doom Wings\" ?", false),
        new Option<bool>("101528", "Valkyrie's Harmonia Wings", "Mode: [select] only\nShould the bot buy \"Valkyrie's Harmonia Wings\" ?", false),
        new Option<bool>("101770", "Dark Carcossa Daggers", "Mode: [select] only\nShould the bot buy \"Dark Carcossa Daggers\" ?", false),
        new Option<bool>("101766", "Dual Doomed Dulcinea", "Mode: [select] only\nShould the bot buy \"Dual Doomed Dulcinea\" ?", false),
        new Option<bool>("101759", "Corrupt Twin Blades of Doom", "Mode: [select] only\nShould the bot buy \"Corrupt Twin Blades of Doom\" ?", false),
        new Option<bool>("101760", "Unreal Twin Blades of Doom", "Mode: [select] only\nShould the bot buy \"Unreal Twin Blades of Doom\" ?", false),
        new Option<bool>("101522", "Valkyrie Bryn Mask Morph", "Mode: [select] only\nShould the bot buy \"Valkyrie Bryn Mask Morph\" ?", false),
        new Option<bool>("101523", "Valkyrie Bryn Mask", "Mode: [select] only\nShould the bot buy \"Valkyrie Bryn Mask\" ?", false),
        new Option<bool>("101524", "Valkyrie Bryn Locks", "Mode: [select] only\nShould the bot buy \"Valkyrie Bryn Locks\" ?", false),
        new Option<bool>("101525", "Sealed Valkyrie Mask", "Mode: [select] only\nShould the bot buy \"Sealed Valkyrie Mask\" ?", false),
        new Option<bool>("101529", "Grand Spear Lorelei", "Mode: [select] only\nShould the bot buy \"Grand Spear Lorelei\" ?", false),
        new Option<bool>("101771", "Dark Carcossa Scythe", "Mode: [select] only\nShould the bot buy \"Dark Carcossa Scythe\" ?", false),
        new Option<bool>("101757", "Corrupt Skullstaff of Doom", "Mode: [select] only\nShould the bot buy \"Corrupt Skullstaff of Doom\" ?", false),
        new Option<bool>("101758", "Unreal Skullstaff of Doom", "Mode: [select] only\nShould the bot buy \"Unreal Skullstaff of Doom\" ?", false),
        new Option<bool>("101765", "Doomed Dulcinea", "Mode: [select] only\nShould the bot buy \"Doomed Dulcinea\" ?", false),
        new Option<bool>("101769", "Dark Carcossa Dagger", "Mode: [select] only\nShould the bot buy \"Dark Carcossa Dagger\" ?", false),
   ];
}
