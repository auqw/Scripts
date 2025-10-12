/*
name: Bocklin Armory Merge
description: This bot will farm the items belonging to the selected mode for the Bocklin Armory Merge [2577] in /bocklincastle
tags: bocklin, armory, merge, bocklincastle, valens, knightly, golden, anjou, knight, duke, noble, enchanted, antlered, regal, dukes, vaughns, sash, aquitaine, gem, earthvessel, harpoon, rapier, rapiers, kris
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BocklinArmoryMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;

    private static CoreLynaria Lyn { get => _Lyn ??= new CoreLynaria(); set => _Lyn = value; }
    private static CoreLynaria _Lyn;
    private static BocklinGroveMerge BGM { get => _BGM ??= new BocklinGroveMerge(); set => _BGM = value; }
    private static BocklinGroveMerge _BGM;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Bocklin Ornament", "Monster Blood", "Vaughn Crest", "Chiral Valley Knight", "Chiral Valley Duke", "Chiral Valley Noble", "Antlered Golden Anjou Helm", "Regal Golden Anjou Helm", "Golden Anjou Helm", "Anjou Duke's Helm", "Vaughn's Crimson Sash", "Vaughn's Sash and Aquitaine", "Gem of Anjou", "Aquitaine", "Dual Aquitaine", "Forbidden EarthVessel Harpoon", "Forbidden EarthVessel Sword", "Forbidden EarthVessel Swords", "Forbidden EarthVessel Dagger", "Forbidden EarthVessel Daggers", "Forbidden EarthVessel Rapier", "Forbidden EarthVessel Rapiers", "Forbidden EarthVessel Kris", "Forbidden Dual EarthVessel Kris" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Lyn.BocklinCastle();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("bocklincastle", 2577, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Bocklin Ornament":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10255 : 10253,
                            ("bocklincastle", "Faceless Ritualist", ClassType.Farm),
                            ("bocklincastle", "Headless Knight", ClassType.Solo),
                        ("bocklincastle", "Warped Revenant", ClassType.Farm)
                            );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Monster Blood":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("ebondungeon", "Dethrix", req.Name, quant, req.Temp, false);
                    break;

                case "Vaughn Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("bocklincastle", "Garde Wraith", req.Name, quant, req.Temp, false);
                    break;

                case "Chiral Valley Knight":
                case "Chiral Valley Duke":
                case "Chiral Valley Noble":
                case "Antlered Golden Anjou Helm":
                case "Regal Golden Anjou Helm":
                case "Anjou Duke's Helm":
                case "Vaughn's Sash and Aquitaine":
                case "Aquitaine":
                case "Dual Aquitaine":
                    BGM.BuyAllMerge(req.Name);
                    break;

                case "Golden Anjou Helm":
                case "Vaughn's Crimson Sash":
                case "Gem of Anjou":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("bocklingrove", "Elder Necromancer", req.Name, quant, req.Temp, false);
                    break;

                case "Forbidden EarthVessel Harpoon":
                case "Forbidden EarthVessel Sword":
                case "Forbidden EarthVessel Swords":
                case "Forbidden EarthVessel Dagger":
                case "Forbidden EarthVessel Daggers":
                case "Forbidden EarthVessel Rapier":
                case "Forbidden EarthVessel Rapiers":
                case "Forbidden EarthVessel Kris":
                case "Forbidden Dual EarthVessel Kris":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("bocklincastle", "Headless Knight", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("93591", "Valen's Knightly Armor", "Mode: [select] only\nShould the bot buy \"Valen's Knightly Armor\" ?", false),
        new Option<bool>("93628", "Golden Anjou Knight", "Mode: [select] only\nShould the bot buy \"Golden Anjou Knight\" ?", false),
        new Option<bool>("93629", "Golden Anjou Duke", "Mode: [select] only\nShould the bot buy \"Golden Anjou Duke\" ?", false),
        new Option<bool>("93630", "Golden Anjou Noble", "Mode: [select] only\nShould the bot buy \"Golden Anjou Noble\" ?", false),
        new Option<bool>("93631", "Enchanted Antlered Anjou Helm", "Mode: [select] only\nShould the bot buy \"Enchanted Antlered Anjou Helm\" ?", false),
        new Option<bool>("93632", "Enchanted Regal Anjou Helm", "Mode: [select] only\nShould the bot buy \"Enchanted Regal Anjou Helm\" ?", false),
        new Option<bool>("93633", "Enchanted Anjou Helm", "Mode: [select] only\nShould the bot buy \"Enchanted Anjou Helm\" ?", false),
        new Option<bool>("93634", "Enchanted Anjou Duke's Helm", "Mode: [select] only\nShould the bot buy \"Enchanted Anjou Duke's Helm\" ?", false),
        new Option<bool>("93635", "Vaughn's Enchanted Sash", "Mode: [select] only\nShould the bot buy \"Vaughn's Enchanted Sash\" ?", false),
        new Option<bool>("93636", "Enchanted Sash and Aquitaine", "Mode: [select] only\nShould the bot buy \"Enchanted Sash and Aquitaine\" ?", false),
        new Option<bool>("93637", "Enchanted Gem of Anjou", "Mode: [select] only\nShould the bot buy \"Enchanted Gem of Anjou\" ?", false),
        new Option<bool>("93663", "Enchanted Aquitaine", "Mode: [select] only\nShould the bot buy \"Enchanted Aquitaine\" ?", false),
        new Option<bool>("93664", "Enchanted Dual Aquitaine", "Mode: [select] only\nShould the bot buy \"Enchanted Dual Aquitaine\" ?", false),
        new Option<bool>("93697", "EarthVessel Harpoon", "Mode: [select] only\nShould the bot buy \"EarthVessel Harpoon\" ?", false),
        new Option<bool>("93698", "EarthVessel Blade", "Mode: [select] only\nShould the bot buy \"EarthVessel Blade\" ?", false),
        new Option<bool>("93699", "EarthVessel Blades", "Mode: [select] only\nShould the bot buy \"EarthVessel Blades\" ?", false),
        new Option<bool>("93700", "EarthVessel Dagger", "Mode: [select] only\nShould the bot buy \"EarthVessel Dagger\" ?", false),
        new Option<bool>("93701", "EarthVessel Daggers", "Mode: [select] only\nShould the bot buy \"EarthVessel Daggers\" ?", false),
        new Option<bool>("93702", "EarthVessel Rapier", "Mode: [select] only\nShould the bot buy \"EarthVessel Rapier\" ?", false),
        new Option<bool>("93703", "EarthVessel Rapiers", "Mode: [select] only\nShould the bot buy \"EarthVessel Rapiers\" ?", false),
        new Option<bool>("93704", "EarthVessel Kris", "Mode: [select] only\nShould the bot buy \"EarthVessel Kris\" ?", false),
        new Option<bool>("93705", "Dual EarthVessel Kris", "Mode: [select] only\nShould the bot buy \"Dual EarthVessel Kris\" ?", false),
    };
}
