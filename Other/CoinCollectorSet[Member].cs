/*
name: coin collector set
description: Farms the coin collector set from hollowhalls
tags: coin collector, set
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class CoinCollectorSet
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetItems();

        Core.SetOptions(false);
    }

    public void GetItems()
    {
        Armor();
        Weapon();
        Helm();
    }

    public void Armor()
    {
        if (Core.CheckInventory("Coin Collector", toInv: false))
            return;

        Adv.BuyItem("hyperspace", 194, "Le Chocolat");
        Core.BuyItem("hollowhalls", 335, "Coin Collector");
        Core.ToBank("Coin Collector");
    }

    public void Weapon()
    {
        if (Core.CheckInventory("Coin Collector Gun", toInv: false))
            return;

        Adv.BuyItem("hyperspace", 194, "Chocolate Doubloon");
        Core.BuyItem("hollowhalls", 335, "Coin Collector Gun");
        Core.ToBank("Coin Collector Gun");
    }

    public void Helm()
    {
        if (Core.CheckInventory("Coin Collector Helmet", toInv: false))
            return;

        Adv.BuyItem("hyperspace", 194, "Chocolate Loonie");
        Core.BuyItem("hollowhalls", 335, "Coin Collector Helmet");
        Core.ToBank("Coin Collector Helmet");
    }
}
