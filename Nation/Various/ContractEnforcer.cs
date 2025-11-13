/*
name: Contract Enforcer of Nulgath
description: This script will farm the "Contract Enforcer of Nulgath" armor.
tags: malakai,pearl of nulgath,contract enforcer,katana,renunciation,the contract enforcer,armor
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ContractEnforcer
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static NulgathDiamondMerge NDM
    {
        get => _NDM ??= new NulgathDiamondMerge();
        set => _NDM = value;
    }
    private static NulgathDiamondMerge _NDM;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static DirtlickersMerge DLM
    {
        get => _DLM ??= new DirtlickersMerge();
        set => _DLM = value;
    }
    private static DirtlickersMerge _DLM;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetArmor();

        Core.SetOptions(false);
    }

    public void GetArmor()
    {
        if (Core.CheckInventory("Contract Enforcer of Nulgath"))
            return;

        if (!Core.CheckInventory("Malakai's Katana Pet"))
        {
            Core.Logger("Malakai's Katana Pet required for this armor, getting it now.");
            NDM.BuyAllMerge("Malakai's Katana Pet");
        }
        Core.AddDrop("Contract Enforcer of Nulgath");
        Adornments();
        Armaments();

        Nation.FarmUni13(3);
        Nation.FarmDarkCrystalShard(100);
        Nation.FarmTotemofNulgath(5);
        Nation.FarmBloodGem(25);
        DLM.BuyAllMerge("Shadow Legacy of Nulgath");
        Adv.BuyItem("tercessuinotlim", 1951, "Unmoulded Fiend Essence");

        // Quest must be acepted for contract to drop, droprate is not 100%.
        Core.EnsureAccept(10050);
        Core.HuntMonster("ebilcorphq", "Dage the Evil", "Dage's Contract", isTemp: false);

        if (!Core.CheckInventory("Pearl of Nulgath", 4))
        {
            Daily.PearlOfNulgath();
            if (!Core.CheckInventory("Pearl of Nulgath", 4))
            {
                Core.Logger(
                    $"You need 4 Pearls of Nulgath to complete the quest, you have {Bot.Inventory.GetQuantity("Pearl of Nulgath")}/4"
                );
                return;
            }
        }

        if (!Core.CheckInventory(Core.QuestRequirements<int>(10050)))
            if (!Core.CheckInventory(Core.QuestRequirements<string>(10050)))
            {
                Core.Logger(
                    "You need to have both rewards from both quests in order to complete the quest. Run the script again tomorrow."
                );
                return;
            }

        Core.EnsureComplete(10050);
        Core.ToBank(Core.QuestRewards(10048, 10049));
    }

    public void Adornments()
    {
        if (Core.CheckInventory(Core.QuestRewards(10048)))
            return;

        Core.AddDrop(Core.QuestRewards(10048));

        Nation.FarmUni13(1);
        Nation.FarmDiamondofNulgath(250);
        Nation.FarmVoucher(true, true);
        Core.HuntMonster("evilwarnul", "Undead Legend", "Wings of Revontheus", isTemp: false);
        Nation.ApprovalAndFavor(1000, 1000);
        if (!Core.CheckInventory("Pearl of Nulgath", 2))
        {
            Daily.PearlOfNulgath();
            if (!Core.CheckInventory("Pearl of Nulgath", 2))
            {
                Core.Logger(
                    $"You need 2 Pearls of Nulgath to complete the quest, you have {Bot.Inventory.GetQuantity("Pearl of Nulgath") / 2}"
                );
                return;
            }
        }
        Core.EnsureAccept(10048);
        Core.EnsureCompleteChoose(10048);
    }

    public void Armaments()
    {
        if (Core.CheckInventory(Core.QuestRewards(10049)))
            return;

        Core.AddDrop(Core.QuestRewards(10049));

        Nation.FarmUni13(1);
        Nation.FarmTaintedGem(150);
        Nation.Supplies("Random Weapon of Nulgath");
        Nation.FarmGemofNulgath(35);
        Bot.Quests.UpdateQuest(9531);
        Core.HuntMonster("voidrefuge", "Carnage", "Bloodletter Katana", isTemp: false);
        if (!Core.CheckInventory("Pearl of Nulgath", 3))
        {
            Daily.PearlOfNulgath();
            if (!Core.CheckInventory("Pearl of Nulgath", 3))
            {
                Core.Logger(
                    $"You need 3 Pearls of Nulgath to complete the quest, you have {Bot.Inventory.GetQuantity("Pearl of Nulgath") / 3}"
                );
                return;
            }
        }
        Core.EnsureAccept(10049);
        Core.EnsureCompleteChoose(10049);
    }
}
