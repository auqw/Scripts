/*
name: EnoughDOOMforanArchfiend
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
using Skua.Core.Interfaces;

public class EnoughDOOMforanArchfiend
{
    public static IScriptInterface Bot => IScriptInterface.Instance;
    public static CoreBots Core => CoreBots.Instance;
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
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    private static WillpowerExtraction WillpowerExtraction
    {
        get => _WillpowerExtraction ??= new WillpowerExtraction();
        set => _WillpowerExtraction = value;
    }
    private static WillpowerExtraction _WillpowerExtraction;
    private static NulgathDemandsWork NulgathDemandsWork
    {
        get => _NulgathDemandsWork ??= new NulgathDemandsWork();
        set => _NulgathDemandsWork = value;
    }
    private static NulgathDemandsWork _NulgathDemandsWork;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(
            new[]
            {
                "ArchFiend DoomLord",
                "Undead Essence",
                "Chaorruption Essence",
                "Essence Potion",
                "Essence of Klunk",
                "Living Star Essence",
                "Bone Dust",
                "Undead Energy",
            }
        );
        Core.SetOptions();

        AFDL();

        Core.SetOptions(false);
    }

    public void AFDL()
    {
        if (Core.CheckInventory("ArchFiend DoomLord", toInv: false))
            return;

        string[] NDWRequiredItems =
        {
            "DoomLord's War Mask",
            "ShadowFiend Cloak",
            "Locks of the DoomLord",
            "Doomblade of Destruction",
        };
        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop(
            "ArchFiend DoomLord",
            "Undead Essence",
            "Chaorruption Essence",
            "Essence Potion",
            "Essence of Klunk",
            "Living Star Essence",
            "Bone Dust",
            "Undead Energy",
            "DoomLord's War Mask",
            "ShadowFiend Cloak",
            "Locks of the DoomLord",
            "Doomblade of Destruction"
        );

        Farm.Experience(75);

        // Quest Accept Requirements: "DoomLord's War Mask", "ShadowFiend Cloak", "Locks of the DoomLord", "Doomblade of Destruction"
        Nation.FarmUni13(1);
        Nation.ApprovalAndFavor(0, 1);
        NulgathDemandsWork.NDWQuest(NDWRequiredItems);

        //Quest Turnin Items:
        NulgathDemandsWork.NDWQuest(new[] { "Unidentified 35" });
        // WillpowerExtraction.Unidentified34(4);
        Nation.FarmBloodGem(10);
        Nation.FarmVoucher(false);
        Nation.EssenceofNulgath(100);
        Farm.BattleUnderB("Undead Essence", 1000);
        Core.EnsureAccept(5260);
        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster("orecavern", "Chaorrupted Good Soldier", "Chaorruption Essence", 75, false);
        Core.HuntMonster("starsinc", "Living Star", "Living Star Essence", 100, false);

        if (!Core.CheckInventory("Aelita's Emerald"))
            Core.BuyItem("yulgar", 16, "Aelita's Emerald");
        Adv.BuyItem("alchemyacademy", 2115, "Essence Potion", 5, 8580); // see if this works
        // if (!Core.CheckInventory("Essence Potion", 5))
        // {
        //     Farm.Gold(12500000);
        //     Core.BuyItem("alchemyacademy", 2115, "Gold Voucher 500k", 25);
        //     Core.BuyItem("alchemyacademy", 2115, "Essence Potion", 5, 8580);
        //     Bot.Wait.ForItemBuy();
        // }

        Nation.ApprovalAndFavor(0, 5000);
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("evilwardage", "Klunk", "Essence of Klunk", isTemp: false);

        //Quest Turnin
        Core.ChainComplete(5260);
        Bot.Wait.ForPickup("ArchFiend DoomLord");
    }
}
