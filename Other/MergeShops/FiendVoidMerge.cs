/*
name: FiendVoid Merge
description: This bot will farm the items belonging to the selected mode for the FiendVoid Merge [2669] in /fiendvoid
tags: fiendvoid, merge, fiendvoid, baleful, scolex, void, fiendish, frenzy, ascension, bonebound, nulgath, ivory, fiend, skull, shell, voids, death, drive, covenant, bone, covenants, nation, boneforged, spines, todestrieb, undying
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FiendVoidMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreNSOD NSoD
    {
        get => _NSoD ??= new CoreNSOD();
        set => _NSoD = value;
    }
    private static CoreNSOD _NSoD;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Unidentified 13", "Blood Gem of the Archfiend", "Totem of Nulgath", "Bones from the Void Realm", "Unidentified 23", "ArchFiend Bone", "Diamond of Nulgath", "Gem of Nulgath", "Nulgath's Approval", "Voucher of Nulgath (non-mem)", "Dark Crystal Shard", "Tainted Gem" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    HashSet<string> BuildItemCache()
    {
        HashSet<string> cache = new();

        foreach (ItemBase item in Bot.Inventory.Items)
            if (item?.Name != null)
                cache.Add(item.Name);

        if (Bot.Bank.Items != null)
            foreach (ItemBase item in Bot.Bank.Items)
                if (item?.Name != null)
                    cache.Add(item.Name);

        return cache;
    }

    string[] BossDrops =
    {
        "Abyssal Corruption Kama",
        "Abyssal Corruption Kamas",
        "Abyssal Master Backsword",
        "Abyssal Master Backswords",
        "Fiendish Apprentice Grin",
        "Fiendish Deathstalker Cloak",
        "Nation's Legacy Plaque",
        "ShadowBone Covenant",
        "ShadowBone Covenants",
        "ShadowBone Shell",
        "ShadowBone Skull",
        "ShadowBone Void of Nulgath"
    };

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("fiendvoid", 2669, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Bones from the Void Realm":
                    NSoD.BonesVoidRealm(1);

                    break;


                case "ArchFiend Bone":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    HashSet<string> itemCache = BuildItemCache();

                    Core.AddDrop(BossDrops.Where(drop => !itemCache.Contains(drop)).ToArray());

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10571 : 10570);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("fiendvoid", "Void Fang", "Diluted Venom", 6, isTemp: false);
                        Core.HuntMonster("fiendvoid", "Arachnid Seeker", "Seeker Thorax", 6, isTemp: false);
                        Core.HuntMonster("fiendvoid", "Archfiend Casimir", "Covetous Hand", 1, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion

                #region Known items

                case "Unidentified 13":
                    Nation.FarmUni13(quant);
                    break;

                case "Blood Gem of the Archfiend":
                    Nation.FarmBloodGem(quant);
                    break;

                case "Totem of Nulgath":
                    Nation.FarmTotemofNulgath(quant);
                    break;

                case "Unidentified 23":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.TheAssistant("Unidentified 23", quant);
                    break;

                case "Diamond of Nulgath":
                    Nation.FarmDiamondofNulgath(quant);
                    break;

                case "Gem of Nulgath":
                    Nation.FarmGemofNulgath(quant);
                    break;

                case "Nulgath's Approval":
                    Nation.ApprovalAndFavor(quant, 0);
                    break;

                case "Voucher of Nulgath (non-mem)":
                    Nation.FarmVoucher(false);
                    break;

                case "Dark Crystal Shard":
                    Nation.FarmDarkCrystalShard(quant);
                    break;

                case "Tainted Gem":
                    Nation.FarmTaintedGem(quant);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("98491", "Baleful Scolex of the Void", "Mode: [select] only\nShould the bot buy \"Baleful Scolex of the Void\" ?", false),
        new Option<bool>("98492", "Fiendish Frenzy of Ascension", "Mode: [select] only\nShould the bot buy \"Fiendish Frenzy of Ascension\" ?", false),
        new Option<bool>("98646", "Bonebound Void of Nulgath", "Mode: [select] only\nShould the bot buy \"Bonebound Void of Nulgath\" ?", false),
        new Option<bool>("98647", "Ivory Fiend Skull", "Mode: [select] only\nShould the bot buy \"Ivory Fiend Skull\" ?", false),
        new Option<bool>("98648", "Ivory Fiend Shell", "Mode: [select] only\nShould the bot buy \"Ivory Fiend Shell\" ?", false),
        new Option<bool>("98649", "Void's Death Drive", "Mode: [select] only\nShould the bot buy \"Void's Death Drive\" ?", false),
        new Option<bool>("98650", "Fiendish Covenant of Bone", "Mode: [select] only\nShould the bot buy \"Fiendish Covenant of Bone\" ?", false),
        new Option<bool>("98651", "Fiendish Covenants of Bone", "Mode: [select] only\nShould the bot buy \"Fiendish Covenants of Bone\" ?", false),
        new Option<bool>("98652", "Nation Covenant of Bone", "Mode: [select] only\nShould the bot buy \"Nation Covenant of Bone\" ?", false),
        new Option<bool>("98653", "Nation Covenants of Bone", "Mode: [select] only\nShould the bot buy \"Nation Covenants of Bone\" ?", false),
        new Option<bool>("98654", "Boneforged Void of Nulgath", "Mode: [select] only\nShould the bot buy \"Boneforged Void of Nulgath\" ?", false),
        new Option<bool>("98655", "Ivory Fiend Helm", "Mode: [select] only\nShould the bot buy \"Ivory Fiend Helm\" ?", false),
        new Option<bool>("98656", "Ivory Fiend Spines", "Mode: [select] only\nShould the bot buy \"Ivory Fiend Spines\" ?", false),
        new Option<bool>("98657", "Void's Todestrieb", "Mode: [select] only\nShould the bot buy \"Void's Todestrieb\" ?", false),
        new Option<bool>("98658", "Undying Covenant of Bone", "Mode: [select] only\nShould the bot buy \"Undying Covenant of Bone\" ?", false),
        new Option<bool>("98659", "Undying Covenants of Bone", "Mode: [select] only\nShould the bot buy \"Undying Covenants of Bone\" ?", false),
   };
}
