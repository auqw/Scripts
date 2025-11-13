/*
name: Lazure's Reawakening
description: This script will complete the Lazure's Reawakening quest.
tags: hollowborn, lae, lazure, reawakening, lazure's awakened skull, quest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
using Skua.Core.Interfaces;

public class LazuresReawakening
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreHollowbornStory HBStory
    {
        get => _HBStory ??= new CoreHollowbornStory();
        set => _HBStory = value;
    }
    private static CoreHollowbornStory _HBStory;
    private static HollowSoul HS
    {
        get => _HS ??= new HollowSoul();
        set => _HS = value;
    }
    private static HollowSoul _HS;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetSkull();

        Core.SetOptions(false);
    }

    public void GetSkull()
    {
        if (Core.isCompletedBefore(9797))
            return;

        // Story
        if (!Core.CheckInventory("Lazure's Skull"))
        {
            Core.AddDrop("Lazure's Skull");
            HBStory.Shadowrealm();
        }

        // Level
        Farm.Experience(70);

        Core.EnsureAccept(9797);

        // Hollow Soul
        HS.GetYaSoulsHeeeere(1500);

        // Gold Voucher 500k
        if (!Core.CheckInventory("Gold Voucher 500k", 40))
            Adv.BuyItem("alchemyacademy", 395, "Gold Voucher 500k", 40);

        // Shadow's Eye
        if (!Core.CheckInventory("Shadow's Eye", 2000))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("shadowrealm", "r2", "Left", "*", "Shadow's Eye", 2000, false);
        }

        // Chaoroot Vitamer
        if (!Core.CheckInventory("Chaoroot Vitamer", 1500))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("hbchallenge", "r5", "Left", "*", "Chaoroot Vitamer", 1500, false);
        }

        // Jury's Statement
        if (!Core.CheckInventory("Jury's Statement", 1000))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("hbchallenge", "r3", "Left", "*", "Jury's Statement", 1000, false);
        }

        Core.EnsureComplete(9797);
    }
}
