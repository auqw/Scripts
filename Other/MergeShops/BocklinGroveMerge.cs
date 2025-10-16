/*
name: Bocklin Grove Merge
description: This bot will farm the items belonging to the selected mode for the Bocklin Grove Merge [2576] in /bocklingrove
tags: bocklin, grove, merge, bocklingrove, champion, lynaria, chiral, valley, knight, duke, noble, antlered, golden, anjou, regal, visor, dukes, vaughns, sash, aquitaine
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BocklinGroveMerge
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

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Enthralling Gem Shard", "Victorious' Golden Scale", "Vaughn Crest", "Golden Anjou Helm", "Vaughn's Crimson Sash", "Sheathed Aquitaine" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Lyn.BocklinGrove();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("bocklingrove", 2576, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Enthralling Gem Shard":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10242 : 10240, false,
                        ("bocklingrove", "Elder Necromancer", ClassType.Solo),
                        ("bocklingrove", "Undead Garde", ClassType.Farm),
                        ("bocklingrove", "Garde Wraith", ClassType.Farm));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Victorious' Golden Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("darkplane", "Victorious", req.Name, quant, req.Temp, false);
                    break;

                case "Vaughn Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("bocklingrove", "Garde Wraith", req.Name, quant, req.Temp, false);
                    break;

                case "Golden Anjou Helm":
                case "Vaughn's Crimson Sash":
                case "Sheathed Aquitaine":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("bocklingrove", "Elder Necromancer", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("93590", "Champion Lynaria Armor", "Mode: [select] only\nShould the bot buy \"Champion Lynaria Armor\" ?", false),
        new Option<bool>("93613", "Chiral Valley Knight", "Mode: [select] only\nShould the bot buy \"Chiral Valley Knight\" ?", false),
        new Option<bool>("93614", "Chiral Valley Duke", "Mode: [select] only\nShould the bot buy \"Chiral Valley Duke\" ?", false),
        new Option<bool>("93615", "Chiral Valley Noble", "Mode: [select] only\nShould the bot buy \"Chiral Valley Noble\" ?", false),
        new Option<bool>("93616", "Antlered Golden Anjou Helm", "Mode: [select] only\nShould the bot buy \"Antlered Golden Anjou Helm\" ?", false),
        new Option<bool>("93617", "Regal Golden Anjou Helm", "Mode: [select] only\nShould the bot buy \"Regal Golden Anjou Helm\" ?", false),
        new Option<bool>("93619", "Antlered Golden Anjou Visor", "Mode: [select] only\nShould the bot buy \"Antlered Golden Anjou Visor\" ?", false),
        new Option<bool>("93620", "Golden Anjou Visor", "Mode: [select] only\nShould the bot buy \"Golden Anjou Visor\" ?", false),
        new Option<bool>("93621", "Anjou Duke's Helm", "Mode: [select] only\nShould the bot buy \"Anjou Duke's Helm\" ?", false),
        new Option<bool>("93624", "Vaughn's Sash and Aquitaine", "Mode: [select] only\nShould the bot buy \"Vaughn's Sash and Aquitaine\" ?", false),
        new Option<bool>("93625", "Aquitaine", "Mode: [select] only\nShould the bot buy \"Aquitaine\" ?", false),
        new Option<bool>("93626", "Dual Aquitaine", "Mode: [select] only\nShould the bot buy \"Dual Aquitaine\" ?", false),
    };
}
