/*
name: Bocklin Treasury Merge
description: This bot will farm the items belonging to the selected mode for the Bocklin Treasury Merge [2578] in /bocklinsanctum
tags: bocklin, treasury, merge, bocklinsanctum, aldens, liberation, battle, morph, lynarias, hammer, castle, princess, brittanys, dress
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs


using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BocklinTreasuryMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreLynaria Lynaria { get => _Lynaria ??= new CoreLynaria(); set => _Lynaria = value; }
    private static CoreLynaria _Lynaria;
    private static BocklinGroveMerge BocklinGroveM { get => _BocklinGroveM ??= new BocklinGroveMerge(); set => _BocklinGroveM = value; }
    private static BocklinGroveMerge _BocklinGroveM;
    private static BocklinArmoryMerge BocklinArmoryM { get => _BocklinArmoryM ??= new BocklinArmoryMerge(); set => _BocklinArmoryM = value; }
    private static BocklinArmoryMerge _BocklinArmoryM;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
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
        Core.BankingBlackList.AddRange(new[] { "Thronekeeper's Rune", "Champion Lynaria Armor", "Valen's Knightly Armor", "King Alteon's Armor Fragment", "Enthralling Gem Shard", "Bocklin Ornament", "Scion's Regalia", "Vaughn Crest" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Lynaria.BocklinSanctum();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("bocklinsanctum", 2578, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Thronekeeper's Rune":
                    Core.FarmingLogger("Thronekeeper's Rune", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(92791);
                    // Members get x2, non-members get x1 drops
                    Core.RegisterQuests(Core.IsMember ? 10267 : 10266);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonster("bocklinsanctum", "Thronekeeper", "Black Armorial Fleur", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Champion Lynaria Armor":
                    BocklinGroveM.BuyAllMerge("Champion Lynaria Armor");
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Valen's Knightly Armor":
                    BocklinArmoryM.BuyAllMerge("Valen's Knightly Armor");
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "King Alteon's Armor Fragment":
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(93763);
                    Core.HuntMonster("alteonfight", "King Alteon", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

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

                case "Scion's Regalia":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(93762);
                    Core.HuntMonster("bocklinsanctum", "Tarnished Scion", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Vaughn Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("bocklincastle", "Garde Wraith", req.Name, quant, req.Temp, false);
                    break;

                case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;


            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("93592", "Alden's Liberation Armor", "Mode: [select] only\nShould the bot buy \"Alden's Liberation Armor\" ?", false),
        new Option<bool>("93825", "Alden's Battle Morph", "Mode: [select] only\nShould the bot buy \"Alden's Battle Morph\" ?", false),
        new Option<bool>("93824", "Alden's Battle Helm", "Mode: [select] only\nShould the bot buy \"Alden's Battle Helm\" ?", false),
        new Option<bool>("93767", "Lynaria's Hammer", "Mode: [select] only\nShould the bot buy \"Lynaria's Hammer\" ?", false),
        new Option<bool>("93766", "Lynaria's Locks", "Mode: [select] only\nShould the bot buy \"Lynaria's Locks\" ?", false),
        new Option<bool>("93765", "Lynaria's Visage", "Mode: [select] only\nShould the bot buy \"Lynaria's Visage\" ?", false),
        new Option<bool>("93764", "Castle Bocklin", "Mode: [select] only\nShould the bot buy \"Castle Bocklin\" ?", false),
        new Option<bool>("93638", "Princess Brittany's Dress", "Mode: [select] only\nShould the bot buy \"Princess Brittany's Dress\" ?", false),
        new Option<bool>("93639", "Princess Brittany's Visage", "Mode: [select] only\nShould the bot buy \"Princess Brittany's Visage\" ?", false),
        new Option<bool>("93640", "Princess Brittany's Hair", "Mode: [select] only\nShould the bot buy \"Princess Brittany's Hair\" ?", false),
   };
}
