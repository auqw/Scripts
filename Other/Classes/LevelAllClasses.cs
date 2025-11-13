/*
name: LevelAllClasses
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class CoreClass
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Level();

        Core.SetOptions(false);
    }

    public void Level()
    {
        List<InventoryItem> itemInv = Bot.Inventory.Items.FindAll(i =>
            i.Category == ItemCategory.Class && i.Quantity != 302500
        );
        foreach (InventoryItem item in itemInv)
        {
            Core.Logger($"Leveling {item.Name} class");
            Adv.RankUpClass(item.Name);
        }
    }
}
