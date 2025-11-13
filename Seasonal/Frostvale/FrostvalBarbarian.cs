/*
name: Frostval Barbarian (Class)
description: This will finish the required quest and farms the required materials in order to get the Frostval Barbarian (Class).
tags: frostval-barbarian, class, seasonal, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class FrostvalBarbarian
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
    private static CoreFrostvale Frostvale
    {
        get => _Frostvale ??= new CoreFrostvale();
        set => _Frostvale = value;
    }
    private static CoreFrostvale _Frostvale;
    private static GlaceraStory Glacera
    {
        get => _Glacera ??= new GlaceraStory();
        set => _Glacera = value;
    }
    private static GlaceraStory _Glacera;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetFB();

        Core.SetOptions(false);
    }

    public void GetFB(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Frostval Barbarian"))
            return;
        if (!Core.isSeasonalMapActive("frostvale"))
            return;

        Glacera.Northstar();
        Frostvale.Battlefield(true);

        if (!Core.CheckInventory("Infernal Ice Heart") && !Core.CheckInventory("Crypto Token", 5))
        {
            Daily.CryptoToken();
            if (!Core.CheckInventory("Crypto Token", 5))
                Core.Logger(
                    $"Please do the Crypto Token Daily {5 - Bot.Inventory.GetQuantity("Crypto Token")} more times before continuing the farm",
                    messageBox: true,
                    stopBot: true
                );
        }

        Core.AddDrop(
            "Frostval Barbarian",
            "Frosty Barbarian's Helm",
            "Frosty Barbarian's Horns",
            "Bearded Barbarian Helm",
            "Frostval Barbarian Cape",
            "Frostval Barbarian Sword"
        );
        Core.EnsureAccept(6649);
        Adv.BuyItem("frostdeep", 520, "Sword Of Hope");
        if (!Core.CheckInventory("Sassafras' War Helm"))
        {
            Core.AddDrop("Sassafras' War Helm");
            while (!Bot.ShouldExit && !Core.CheckInventory("Sassafras' War Helm"))
            {
                Core.EnsureAccept(2570);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("newbie", "Slime", "Potent Dried Slime", 6);
                Core.EnsureComplete(2570);
            }
            Bot.Wait.ForPickup("Sassafras' War Helm");
        }
        if (!Core.CheckInventory("Fur Tuft"))
        {
            Core.AddDrop("Fur Tuft");
            Core.EnsureAccept(1513);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("alpine", "Wendigo", "Woebegone Wendigo");
            Core.EnsureComplete(1513);
            Bot.Wait.ForPickup("Fur Tuft");
        }
        if (!Core.CheckInventory("Icy Holly"))
        {
            Core.AddDrop("Icy Holly");
            Core.EnsureAccept(6132);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("coldwindvalley", "Snow Golem", "Elemental Ice", 5);
            Core.GetMapItem(5557, 8, "coldwindvalley");
            Core.EnsureComplete(6132);
            Bot.Wait.ForPickup("Icy Holly");
        }
        if (!Core.CheckInventory("Glaceran Key"))
        {
            Core.AddDrop("Glaceran Key");
            Core.EnsureAccept(3971);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("northstar", "Karok The Fallen", "Karok defeated", 1);
            Core.EnsureComplete(3971);
            Bot.Wait.ForPickup("Glaceran Key");
        }
        if (!Core.CheckInventory("Infernal Ice Heart"))
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("frostvalfuture", "Wargoth the Frozen", "Frozen Orb", 5, false);
            Adv.BuyItem("curio", 1539, "Infernal Ice Heart");
            Bot.Wait.ForPickup("Infernal Ice Heart");
        }
        Core.EnsureComplete(6649);

        Bot.Wait.ForPickup("Frostval Barbarian");
        if (rankUpClass)
            Adv.RankUpClass("Frostval Barbarian");
    }
}
