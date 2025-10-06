//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using System;
using System.Linq;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TestZone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Test();

        Bot.Stop();
    }

    void Test()
    {
        //Ultra.Test();
        // priority => ...
        //Core.ChooseBestEnhancement("Weapon", "Valiance", "Spiral Carve", "Fighter");
        //Core.ChooseBestEnhancement("Helm", "Pneuma", "Wizard");
        //Core.ChooseBestEnhancement("Cape", "Vainglory", "Wizard");
        //Core.ChooseBestGear("*"); // for the most common race in the map
        //Core.ChooseBestGear("Random Monster"); // for a specific monster
        //Ultra.BuyAlchemyPotion("Potent Honor Potion");
        //Core.BuyItem("Shriekward Potion", 774, "mirrorportal", 30);
        //Core.ForItem("Onyx Lava Dragon", "lair", "Celestial Staff", useBestGear: true);
        Core.ForItem("Mini Boss Dummy", "classhall", "Celestial Staff");
        //Core.ForItem("Purple Draconian, Venom Draconian, Bronze Draconian", "lair", "Celestial Staff");
        //Core.GetScrollOfEnrage();
        //Core.GetScrollOfDecay();
    }

    void JoinDemo()
    {
        // Just join a map (private room auto)
        Core.Join("greenguardwest");

        // Join and jump to a specific spot
        Core.Join("greenguardwest", cell: "Enter", pad: "Left");

        // Public room (no private suffix)
        Core.Join("battleon", publicRoom: true);

        // Specific room number
        Core.Join("museum", roomNumber: 8383);
    }

    void BestCellDemo()
    {
        // Single name
        Core.ChooseBestCell("Slime");

        // Multiple names (comma or pipe)
        Core.ChooseBestCell("Slime, Frogzard");
        Core.ChooseBestCell("Slime|Frogzard");

        // Wildcard (any monsters)
        Core.ChooseBestCell("*"); // or Core.ChooseBestCell(string.Empty)

        // Most-populated cell (default)
        // Picks the cell with the highest count of those monsters
        Core.ChooseBestCell("Slime, Frogzard");

        // First match’s cell (alt mode)
        Core.ChooseBestCell("Slime, Frogzard", alt: true);

        // Force a specific cell/pad
        Core.ChooseBestCell("Slime", setCell: "Farm1", setPad: "Left");
    }

    void BuyItemDemo()
    {
        // By NAME, join map, all checks on
        Core.BuyItem("Item", 2036, "Map", quantity: 5);

        // By ID, join map
        Core.BuyItem(12345, 2036, "Map", quantity: 3);

        // Already in map → skip join
        Core.BuyItem("Item", 2036, "Map", ensureMap: false);

        // Always buy up to quantity (ignore current owned)
        Core.BuyItem("Item", 2036, "Map", quantity: 10, skipIfHaveEnough: false);

        // Buy EXACT quantity now (don’t calculate remaining)
        Core.BuyItem("Item", 2036, "Map", quantity: 4, calculateRemaining: false);

        // Count items in bank too (pulls in if needed)
        Core.BuyItem("Item", 1200, "Map", considerBank: true);

        // Turn off a specific guard (inventory space)
        Core.BuyItem("Item", 456, "Map", checkInvSpace: false);

        // Increase shop load timeout
        Core.BuyItem("Item", 789, "Map", loadTimeoutMs: 10000);

        // Level-gated item: disable level check
        Core.BuyItem("Item", 111, "Map", checkLevel: false);

        // Gold-gated item: disable gold check
        Core.BuyItem("Item", 222, "Map", checkGold: false);
    }

    void AuraDemo()
    {
        // Check a single aura (self/target)
        bool hasMightSelf = Core.HasAura("Might", self: true);
        bool hasBleedTarget = Core.HasAura("Bleeding", self: false);

        // Check any of several auras
        bool anyBuffSelf = Core.HasAnyAura(new List<string> { "Might", "Sage", "Battle Elixir" }, self: true);

        // Check if target has any aura other than a specific one
        bool targetHasOther = Core.HasAnyAuraOtherThan("Stunned", self: false);

        // Get stacks and seconds remaining
        int mightStacks = Core.GetAuraStacks("Might", self: true);
        int stunLeft = Core.GetAuraSecondsRemaining("Stunned", self: false);

        // Convenience predicates
        bool has3StacksSelf = Core.Stacks("Might", quantity: 3, self: true);
        bool stunEndingSoon = Core.Left("Stunned", duration: 2, self: false); // ≤ 2s remaining

        // Typical patterns:
        if (!Core.HasAura("Might", true))
        {
            // apply a buff, or trigger consumable logic…
        }

        if (Core.Stacks("Bleeding", 5, self: false))
        {
            // burst/finisher because target has ≥5 bleed stacks
        }

        if (Core.Left("Shield", 1, self: true))
        {
            // refresh your shield if it expires in ≤1s
        }

        // Defensive checks
        if (Core.GetAuraStacks("Nonexistent", true) == 0)
        {
            // safe to assume not present (or engine returned 0)
        }
    }

    void DropsDemo()
    {
        // Check by NAME / ID
        bool hasSlime = Core.HasDrop("Slime Gel");
        bool hasId = Core.HasDrop(12345);

        // Get drop info (name/id lookup)
        var infoByName = Core.GetDropItem("Slime Gel");
        var infoById = Core.GetDropItem(12345);

        // Pick up specific drops (name or id)
        Core.Pickup("Slime Gel");
        Core.Pickup(12345);

        // Pick up multiple at once
        Core.Pickup("Slime Gel", "Frogzard Scale", 12345, 67890);

        // Wait for a drop (returns false on timeout)
        if (Core.WaitForDrop("Slime Gel", timeout: 15000))
            Core.Log("Drops", "Got Slime Gel");
        else
            Core.Log("Drops", "⏱Timeout waiting Slime Gel");

        // Check if ANY among a set is on the ground
        bool anyWanted = Core.HasAny("Slime Gel", "Frogzard Scale", 12345);
        if (anyWanted) Core.Pickup("Slime Gel", "Frogzard Scale", 12345);

        // Typical pattern during a farm loop:
        //  - periodically pick up drops
        //  - or block until a key drop appears
        Core.Pickup("Quest Item A", "Quest Item B");
        if (!Core.WaitForDrop("Quest Item A", 10000))
            Core.Log("Drops", "'Quest Item A' didn’t drop in time");
    }

}
