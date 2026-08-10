/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using System;
using System.Linq;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TestZone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Test();

        Bot.StopSync();
    }

    void Test()
    {
        // Minimal “just go kill and collect” into a specific cell
        // Core.KillQuest(1234, "battleon", "Slime", "Goo", 3, "r2");

        // Same, but jump to pad "Right"
        // Core.KillQuest(1234, "battleon", "Slime", "Goo", 3, "r2", "Right");

        // Rely on defaults: temp item, no best gear, no alt jump, no cell/pad
        // Core.KillQuest(1234, "battleon", "Slime", "Goo"); // quantity defaults to 1

        // Non-temp drop, use best gear, alt jump, still no cell/pad
        // Core.KillQuest(1234, "battleon", "Slime", "Goo", quantity: 5, isTemp: false, useBestGear: true, altJump: true);

        // Full control: set cell, pad, and raise priority
        /*Core.KillQuest(
            questId: 1234,
            map: "battleon",
            monster: "Slime",
            item: "Goo",
            quantity: 10,
            isTemp: false,
            useBestGear: true,
            altJump: true,
            jumpCell: "r5",
            jumpPad: "Spawn",
            priority: true
        );*/

        //Core.KillQuest(1234, "battleon", "Slime", "Goo");

        //  - Dragonslayer Veteran
        Core.KillQuest(165, "lair", "Water Draconian|Wyvern");

        //  - Dragonslayer Sergeant
        Core.KillQuest(166, "lair", "Bronze Draconian|Purple Draconian|Venom Draconian");

        //  - Dragonslayer Captain
        Core.KillQuest(167, "lair", "Dark Draconian|Golden Draconian");

        //  - Dragonslayer Marshal
        Core.KillQuest(168, "lair", "Red Dragon");

        //  - Dragonslayer Reward (Bonus Quest)
        Core.KillQuest(169, "lair", "Bronze Draconian|Purple Draconian|Venom Draconian");

        //  - Dragonbane (Bonus Quest)
        Core.KillQuest(109, "lair", "Bronze Draconian|Purple Draconian|Venom Draconian");

        //  - Dragon Scales (Bonus Quest)
        Core.KillQuest(110, "lair", "Bronze Draconian|Purple Draconian|Venom Draconian");

        //  - Dragon Souvenirs (Bonus Quest)
        Core.KillQuest(111, "lair", "Bronze Draconian|Purple Draconian|Venom Draconian");

        //  - Dragonslayer (Bonus Quest)
        //Core.KillQuest(112, "lair", "Red Dragon");



        //Ultra.Test();
        // priority => ...
        Core.ChooseBestEnhancement("Weapon", "Valiance", "Spiral Carve", "Fighter");
        Core.ChooseBestEnhancement("Helm", "Pneuma", "Wizard");
        Core.ChooseBestEnhancement("Cape", "Vainglory", "Wizard");
        //Core.ChooseBestGear("*"); // for the most common race in the map
        //Core.ChooseBestGear("Random Monster"); // for a specific monster
        //Ultra.BuyAlchemyPotion("Potent Honor Potion");
        //Core.BuyItem("Shriekward Potion", 774, "mirrorportal", 30);
        //Core.ForItem("Onyx Lava Dragon", "lair", "Celestial Staff", useBestGear: true);
        //Core.ForItem("Mini Boss Dummy", "classhall", "Celestial Staff");
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
}
