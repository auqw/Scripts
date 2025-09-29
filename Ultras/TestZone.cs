//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TestZone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Test();

        Bot.Stop();
    }

    void Test()
    {
        /*InventoryItem testItem = Core.EnsureItemWithEnhancement("Healer");
        Bot.Log($"[Enhancement Test] Item with 'Healer' enhancement: {(testItem != null ? $"Found - {testItem.Name}" : "Not found")}");

        InventoryItem testItem2 = Core.EnsureItemWithEnhancement(5); // Hybrid
        Bot.Log($"[Enhancement Test] Item with pattern ID 5: {(testItem2 != null ? $"Found - {testItem2.Name}" : "Not found")}");

        if (testItem != null)
        {
            bool isHealer = Core.EnhancementIs(testItem, "Healer");
            Bot.Log($"[Enhancement Test] Item {testItem.Name} is Healer: {isHealer}");
        }

        InventoryItem testItem3 = Core.EnsureItemWithEnhancement("NonExistentEnhancement");
        Bot.Log($"[Enhancement Test] Item with 'NonExistentEnhancement': {(testItem3 != null ? $"Found - {testItem3.Name}" : "Not found")}");

        var invItems = Bot.Inventory.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        Bot.Log($"[Enhancement Test] Total inventory items: {invItems.Count()}");

        foreach (var item in invItems.Where(i => i?.EnhancementPatternID > 0))
        {
            string enhName = Core.EnhancementName(item.EnhancementPatternID);
            Bot.Log($"[Enhancement Test] Item: {item.Name}, Enhancement ID: {item.EnhancementPatternID}, Name: {enhName ?? "Unknown"}");
        }*/

        Core.ForItem("Khasaanda", "dreamnexus", "Celestial Staff");
        // Core.ForItem("Onyx Lava Dragon", "lair", "Celestial Staff");
    }
}
