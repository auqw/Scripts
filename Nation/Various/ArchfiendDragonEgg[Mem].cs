/*
name: ArchfiendDragonEgg[Mem]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;

public class ArchfiendDragonEgg
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAFDE();

        Core.SetOptions(false);
    }

    public void GetAFDE()
    {
        if (!Core.IsMember)
            return;

        if (Core.CheckInventory("ArchFiend Baby Dragon Pet"))
            return;

        Core.AddDrop("ArchFiend Baby Dragon Pet");

        Core.EnsureAccept(7296);
        Core.BuyItem("Airstorm", 357, "Breath of Life");
        Core.HuntMonster(
            "queenspire",
            "Fire Guardian Dragon",
            "Fire Guardian Dragon Soul",
            isTemp: false
        );
        HB.FreshSouls(1, 10);
        //why the fuck was the class buffed!?
        InventoryItem? usethis = Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(n =>
                n.Name.Equals("Yami no Ronin") || n.Name.StartsWith("Chaos Slayer")
            );
        if (usethis != null)
        {
            Core.Equip(usethis.ID);
            Core.Equip(Core.FarmGear);
            if (usethis.Name.Equals("Yami no Ronin"))
                Bot.Skills.StartAdvanced("1 | 4");
        }
        else
            Core.EquipClass(ClassType.Dodge);
        Core.HuntMonster("Underlair", "ArchFiend DragonLord", "Fiendish Brimstone", isTemp: false);
        Core.BuyItem("Ariapet", 12, "ArchFiend Dragon Egg");
        Core.EnsureComplete(7296);
        Bot.Wait.ForPickup("ArchFiend Baby Dragon Pet");
        Core.EquipClass(ClassType.Solo);
    }
}
