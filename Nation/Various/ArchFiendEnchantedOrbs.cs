/*
name: ArchFiendEnchantedOrbs
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
using Skua.Core.Interfaces;

public class ArchFiendEnchantedOrbs
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
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            Nation
                .bagDrops.Concat(Nation.tercessBags)
                .Concat(
                    new[]
                    {
                        "Unidentified 34",
                        "Unidentified 19",
                        "Necrot",
                        "Chaoroot",
                        "Doomatter",
                        "Mortality Cape of Revontheus",
                        "Facebreakers of Nulgath",
                        "SightBlinder Axes of Nulgath",
                        "Mystic Tribal Sword",
                        "King Klunk's Crown",
                        "Golden Shadow Breaker",
                        "Shadow Terror Axe",
                    }
                )
        );
        Core.SetOptions();

        GetAFEO();

        Core.SetOptions(false);
    }

    public void GetAFEO()
    {
        if (Core.CheckInventory("ArchFiend Enchanted Orbs"))
            return;

        Adv.BuyItem("tercessuinotlim", 1951, "Unidentified 25");
        Nation.FarmUni13(1);
        Nation.FarmDiamondofNulgath(150);
        HB.FreshSouls(1, 100); // Also has the uni36
        Nation.FarmBloodGem(10);
        Nation.FarmVoucher(false);

        Core.BuyItem("tercessuinotlim", 1820, "ArchFiend Enchanted Orbs");
    }
}
