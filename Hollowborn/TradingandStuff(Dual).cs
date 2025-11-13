/*
name: Trading and Stuff (Dual)
description: This script will complete "Trading and Stuff (Dual)" quest.
tags: hollowborn oblivion blade, trading, trading and stuff, hollowborn, dual oblivion blade, dual
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/CoreNation.cs

using Skua.Core.Interfaces;

public class TradingandStuffDual
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBlade();

        Core.SetOptions(false);
    }

    public void GetBlade()
    {
        if (
            Core.CheckInventory("Hollowborn Oblivion Blade")
            || !Core.CheckInventory("Dual Oblivion Blade of Nulgath")
        )
            return;

        Core.AddDrop("Hollowborn Oblivion Blade");

        Core.EnsureAccept(7295);
        Farm.Experience(80);
        Nation.FarmVoucher(false);
        ArchFiendEnchantedOrbs();
        Core.HuntMonster("shadowattack", "Death", "Death's Power", isTemp: false);
        if (!Core.CheckInventory("Unidentified 25"))
        {
            Farm.Gold(15000000);
            Core.BuyItem("tercessuinotlim", 1951, "Unmoulded Fiend Essence");
            Bot.Wait.ForPickup("Unmoulded Fiend Essence");
            Core.BuyItem("tercessuinotlim", 1951, "Unidentified 25");
        }
        HB.FreshSouls(1, 100);
        Core.EnsureComplete(7295);
        Bot.Wait.ForPickup("Hollowborn Oblivion Blade");
        Adv.EnhanceItem(
            "Hollowborn Oblivion Blade",
            EnhancementType.Lucky,
            CapeSpecial.None,
            HelmSpecial.None,
            WeaponSpecial.Spiral_Carve
        );
    }

    public void ArchFiendEnchantedOrbs()
    {
        if (Core.CheckInventory("ArchFiend Enchanted Orbs"))
            return;

        HB.FreshSouls(1, 100);
        if (!Core.CheckInventory("Unidentified 25"))
        {
            Farm.Gold(15000000);
            Core.BuyItem("tercessuinotlim", 1951, "Unmoulded Fiend Essence");
            Core.BuyItem("tercessuinotlim", 1951, "Unidentified 25");
        }
        Nation.FarmUni13(1);
        Nation.DiamondEvilWar(150);
        Nation.FarmBloodGem(10);
        Nation.FarmVoucher(false);
        Core.BuyItem("tercessuinotlim", 1820, "ArchFiend Enchanted Orbs");
        Bot.Wait.ForPickup("ArchFiend Enchanted Orbs");
    }
}
