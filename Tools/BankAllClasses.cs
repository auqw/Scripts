/*
name: Bank All Classes
description: Banks every unequipped class in the inventory.
tags: bank, classes, inventory, utility
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class BankAllClasses
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        BankClasses();

        Core.SetOptions(false);
    }

    private void BankClasses()
    {
        Bot.Inventory.EnsureToBank(
            Bot.Inventory.Items
                .Where(item => item.Category == ItemCategory.Class && !item.Equipped)
                .Select(item => item.ID)
                .ToArray()
        );
    }
}
