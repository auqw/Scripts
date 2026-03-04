/*
name: Camlan Merge
description: This bot will farm the items belonging to the selected mode for the Camlan Merge [2349] in /camlan
tags: camlan, merge, camlan, dark, tithe, chevalier, armet, chevaliers, long, cloak, starbane, lance, fallen, star, shield, apocryphal, shadow, executioner, retribution, eons, broadsword, awakened, carnage, maw, cleaver
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowofDoom/CoreShadowofDoom.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class CamlanMerge
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
    private static CoreShadowofDoom CoreSoD
    {
        get => _CoreSoD ??= new CoreShadowofDoom();
        set => _CoreSoD = value;
    }
    private static CoreShadowofDoom _CoreSoD;
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
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Ouroboros Scale",
                "Advent Darkness Axe",
                "Advent Darkness Blade",
                "Dark Eons Broadsword",
                "Dark Eons Sword",
                "Shrouded Carnage Maw Cleaver",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CoreSoD.DoAll(true);
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("camlan", 2349, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Ouroboros Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Monster? bellona;
                    Monster? sleih;
                    Core.RegisterQuests(9443);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        if (Bot.Map.Name != "camlan")
                            Core.Join("camlan");

                        if (Bot.Player.Cell != "r9")
                            Core.Jump("r9", "Let");

                        bellona = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 22);
                        sleih = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 23);

                        // Priority: missing drop
                        if (Bot.TempInv.Contains("Bellona's Edict of War") && bellona != null)
                            Bot.Combat.Attack(22);

                        else if (Bot.TempInv.Contains("Sleih's Changeling Records") && sleih != null)
                            Bot.Combat.Attack(23);

                        // If both drops owned OR priority mob dead → kill the other for respawn sync
                        else if (bellona != null)
                            Bot.Combat.Attack(22);

                        else if (sleih != null)
                            Bot.Combat.Attack(23);

                        Bot.Sleep(200);

                        // Hunt maw if the other 2 items are collected already.
                        if (Bot.TempInv.Contains("Bellona's Edict of War") && Bot.TempInv.Contains("Sleih's Changeling Records"))
                            Core.HuntMonster("camlan", "Metamorphosis Maw", "Alchemic Snake Scale", log: false);
                        Bot.Wait.ForQuestComplete(9443);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;


                case "Advent Darkness Axe":
                case "Advent Darkness Blade":
                case "Shrouded Carnage Maw Cleaver":
                case "Dark Eons Sword":
                case "Dark Eons Broadsword":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster("camlan", "Metamorphosis Maw", req.Name, quant, req.Temp);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "78484",
            "Dark Tithe Chevalier",
            "Mode: [select] only\nShould the bot buy \"Dark Tithe Chevalier\" ?",
            false
        ),
        new Option<bool>(
            "78485",
            "Dark Tithe Armet",
            "Mode: [select] only\nShould the bot buy \"Dark Tithe Armet\" ?",
            false
        ),
        new Option<bool>(
            "78486",
            "Dark Chevalier's Long Cloak",
            "Mode: [select] only\nShould the bot buy \"Dark Chevalier's Long Cloak\" ?",
            false
        ),
        new Option<bool>(
            "78487",
            "Starbane Lance",
            "Mode: [select] only\nShould the bot buy \"Starbane Lance\" ?",
            false
        ),
        new Option<bool>(
            "78488",
            "Fallen Star Shield",
            "Mode: [select] only\nShould the bot buy \"Fallen Star Shield\" ?",
            false
        ),
        new Option<bool>(
            "66487",
            "Apocryphal Shadow Executioner",
            "Mode: [select] only\nShould the bot buy \"Apocryphal Shadow Executioner\" ?",
            false
        ),
        new Option<bool>(
            "66488",
            "Apocryphal Retribution Blade",
            "Mode: [select] only\nShould the bot buy \"Apocryphal Retribution Blade\" ?",
            false
        ),
        new Option<bool>(
            "66489",
            "Apocryphal Eons Broadsword",
            "Mode: [select] only\nShould the bot buy \"Apocryphal Eons Broadsword\" ?",
            false
        ),
        new Option<bool>(
            "66490",
            "Apocryphal Eons Sword",
            "Mode: [select] only\nShould the bot buy \"Apocryphal Eons Sword\" ?",
            false
        ),
        new Option<bool>(
            "66492",
            "Awakened Carnage Maw Cleaver",
            "Mode: [select] only\nShould the bot buy \"Awakened Carnage Maw Cleaver\" ?",
            false
        ),
    };
}
