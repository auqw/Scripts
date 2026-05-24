/*
name: UltraPotions
description: Ultra potion helper/presets with BuyReagents enabled
tags: ultra,potions
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Other/Various/Potions.cs

using System;
using System.Linq;
using System.Collections.Generic;
using Skua.Core.Interfaces;

public class UltraPotions
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public CoreEngine C = new();

    private static CoreUltra Ultra
    {
        get => _Ultra ??= new CoreUltra();
        set => _Ultra = value;
    }
    private static CoreUltra _Ultra;

    private static PotionBuyer Pots
    {
        get => _Pots ??= new PotionBuyer();
        set => _Pots = value;
    }
    private static PotionBuyer _Pots;

    #region Class Detection

    public string NormalizeString(string s) =>
        string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : s.Replace(" ", "").ToLowerInvariant();

    public bool HasAssignedClass(string assignedClass) =>
        NormalizeString(Bot.Player.CurrentClass?.Name)
        == NormalizeString(assignedClass);

    #endregion

    #region Presets

    public string[] GetRecommendedPotions(string context = "")
    {
        if (context.Equals("Darkon", StringComparison.OrdinalIgnoreCase))
        {
            if (HasAssignedClass("King's Echo"))
                return new[]
                {
                    "Body Tonic",
                    "Potent Destruction Elixir",
                    "Potent Honor Potion"
                };

            // fallthrough to default presets if no Darkon-specific override for this class
        }

        if (context.Equals("Dage", StringComparison.OrdinalIgnoreCase))
        {
            if (HasAssignedClass("Lord of Order"))
                return new[]
                {
                    "Fate Tonic",
                    "Unstable Divine Elixir",
                    "Potent Honor Potion"
                };

            // fallthrough to default presets if no Dage-specific override for this class
        }

        if (HasAssignedClass("ArchPaladin"))
            return new[]
            {
                "Fate Tonic",
                "Potent Destruction Elixir",
                "Potent Honor Potion"
            };

        if (HasAssignedClass("StoneCrusher"))
            return new[]
            {
                "Fate Tonic",
                "Potent Malevolence Elixir",
                "Potent Honor Potion"
            };

        if (HasAssignedClass("Lord of Order"))
            return new[]
            {
                "Fate Tonic",
                "Unstable Divine Elixir",
                "Potent Life Potion"
            };

        if (HasAssignedClass("King's Echo"))
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                "Potent Honor Potion"
            };

        return new[]
        {
            Ultra.GetBestTonicPotion(),
            Ultra.GetBestElixirPotion(),
            "Potent Honor Potion"
        };
    }

    #endregion

    #region Potion Usage

    /// <summary>
    /// Activates the currently equipped consumable potion (skill slot 5) if its aura is not already active.
    /// Covers: Potent Honor Potion, Potent Life Potion, Felicitous Philtre.
    /// Call this once per combat loop iteration.
    /// </summary>
    public void ActivateEquippedPotion()
    {
        if (Bot.Inventory.IsEquipped("Potent Honor Potion"))
        {
            if (!C.HasAura("Potent Honor Malice", true) && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        else if (Bot.Inventory.IsEquipped("Potent Life Potion"))
        {
            if (!C.HasAura("Righteous", true) && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        else if (Bot.Inventory.IsEquipped("Felicitous Philtre"))
        {
            if (!C.HasAura("Felicitous Philtre", true) && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        else if (Bot.Inventory.IsEquipped("Endurance Draught"))
        {
            if (!C.HasAura("Endurance Draught", true) && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
    }

    public void UseRecommendedPotions(int desiredQuant = 10, bool skipThird = false, string context = "")
    {
        string[] potions = GetRecommendedPotions(context);

        if (potions.Length == 0)
            return;

        if (skipThird && potions.Length >= 3)
            potions = potions[..2];

        EnsurePotions(desiredQuant, skipThird, context);

        foreach (string potion in potions)
        {
            // Already active — aura name matches potion name for most consumables
            if (Bot.Self.Auras.Any(a => a != null && a.Name == potion))
            {
                Core.Logger($"{potion} aura already active.");
                continue;
            }

            Core.Logger($"Equipping {potion}...");

            C.EquipConsumable(potion);

            Bot.Sleep(500);

            // Some consumables auto-use when equipped
            if (Bot.Self.Auras.Any(a => a != null && a.Name == potion))
            {
                Core.Logger($"{potion} successfully applied.");
                continue;
            }

            // If not equipped somehow, stop here
            if (!Bot.Inventory.IsEquipped(potion))
            {
                Core.Logger($"Failed to equip {potion}.");
                Bot.Sleep(200);
                continue;
            }

            Core.Logger($"Using {potion}...");

            Core.UsePotion();

            Bot.Sleep(500);
        }
    }

    #endregion

    #region Potion Purchase

    public void EnsurePotions(int desiredQuant = 10, bool skipThird = false, string context = "")
    {
        string[] potions = GetRecommendedPotions(context);

        if (potions.Length == 0)
            return;

        if (skipThird && potions.Length >= 3)
            potions = potions[..2];

        List<string> missing = new();

        foreach (string potion in potions)
        {
            int current = Bot.Inventory.GetQuantity(potion);

            if (current < desiredQuant)
            {
                int needed = desiredQuant - current;

                Core.Logger($"Need {needed}x more {potion}.");

                missing.Add(potion);
            }
        }

        if (missing.Count == 0)
        {
            Core.Logger("All recommended potions already stocked.");
            return;
        }

        Pots.INeedYourStrongestPotions(
            Potions: missing.ToArray(),
            PotionQuant: desiredQuant,
            Seperate: true,
            BuyReagents: true
        );

        Core.Logger("Potion purchase complete.");
    }

    #endregion
}