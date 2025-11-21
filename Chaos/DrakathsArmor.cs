/*
name: Drakath Armor
description: Gets the Drakath Armor / Original Drakath Armor
tags: drakath, drakath armor, original drakath armor
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class DrakathArmorBot
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreBLOD BLOD
    {
        get => _BLOD ??= new CoreBLOD();
        set => _BLOD = value;
    }
    private static CoreBLOD _BLOD;
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBoth();

        Core.SetOptions(false);
    }

    public void GetBoth()
    {
        LOC.Complete13LOC();
        DrakathArmorQuest();
        if (!Core.CheckInventory("Dage's Scroll Fragment", 13))
        {
            Core.Logger(
                "Cannot continue with \"Drakath Armor\" not enough \"Dage's Scroll Fragment\""
            );
            return;
        }
        Core.BuyItem(Bot.Map.Name, 994, "Original Drakath Armor");
        Core.BuyItem(Bot.Map.Name, 994, "Drakath Armor");
    }

    public void DrakathArmor()
    {
        if (Core.CheckInventory("Drakath Armor"))
            return;

        LOC.Complete13LOC();
        DrakathArmorQuest();
        Core.BuyItem(Bot.Map.Name, 994, "Drakath Armor");
    }

    public void DrakathOriginalArmor()
    {
        if (Core.CheckInventory("Original Drakath Armor"))
            return;

        LOC.Complete13LOC();
        DrakathArmorQuest();
        Core.BuyItem(Bot.Map.Name, 994, "Original Drakath Armor");
    }

    public void DrakathArmorQuest()
    {
        if (
            Core.CheckInventory(
                60796 /* Get Your Original Drakath's Armor */
            )
        )
            return;

        Core.AddDrop(
            "Dage's Scroll Fragment",
            "Treasure Chest",
            "Face of Chaos",
            "Get Your Original Drakath's Armor"
        );
        Core.EnsureAccept(3882);
        Core.Logger("Getting Quest Accept Requirement: Blinding Light of Destiny");
        BLOD.BlindingLightOfDestiny();
        if (!Core.CheckInventory("Blinding Light of Destiny"))
        {
            Core.Logger(
                "You do not own \"Blinding Light of Destiny\". We'll Farm the materials, but you still need top run this again to get BLoD."
            );
        }

        // Get Accept Requirement: Blind Light of Destiny
        Farm.ChaosREP();

        Farm.BladeofAweREP(6, farmBoA: true);
        Nation.FarmUni13(3);
        Adv.BuyItem("hyperspace", 194, "Le Chocolat");
        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("swordhavenundead", "Left", "Right", "*", "Treasure Chest", 100, false);
        Core.EquipClass(ClassType.Solo);
        Core.KillMonster(
            "ultradrakath",
            "r1",
            "Left",
            "Champion of Chaos",
            "Face of Chaos",
            isTemp: false,
            publicRoom: true
        );

        if (!Core.CheckInventory("Dage's Scroll Fragment", 13))
        {
            Daily.DagesScrollFragment();
            if (!Core.CheckInventory("Dage's Scroll Fragment", 13))
            {
                Core.Logger(
                    "Important",
                    "You do not own \"Dage's Scroll Fragment\". We'll Farm the materials, but you still need to run this again to get Dage's Scroll Fragment -- tomorrow."
                );
                return;
            }
        }

        if (
            Core.CheckInventory("Dage's Scroll Fragment", 13)
            && Core.CheckInventory("Blinding Light of Destiny")
        )
            Core.EnsureComplete(3882); // Complete Quest: Get Your Original Drakath's Armor
        Bot.Wait.ForPickup("Get Your Original Drakath's Armor");
    }
}
