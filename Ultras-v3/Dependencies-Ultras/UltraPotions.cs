/*
name: UltraPotions
description: Ultra potion helper/presets with BuyReagents enabled
tags: ultra,potions
*/

//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreStory2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/GetPotions.cs

using System;
using System.Linq;
using System.Collections.Generic;
using Skua.Core.Interfaces;

public class UltraPotions
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots2 Core => CoreBots2.Instance;

    public CoreEngine2 C => CoreEngine2.Instance;

    private static CoreUltra2 Ultra
    {
        get => _Ultra ??= new CoreUltra2();
        set => _Ultra = value;
    }
    private static CoreUltra2 _Ultra;

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

    private string GetHonorOrMalicePotion()
    {
        if (Bot.Inventory.GetQuantity("Potent Malice Potion") > 30)
            return "Potent Malice Potion";
        return "Potent Honor Potion";
    }

    public string[] GetRecommendedPotions(string context = "")
    {
        if (context.Equals("Dage", StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };
        }

        if (context.Equals("Kolr", StringComparison.OrdinalIgnoreCase))
        {
            if (HasAssignedClass("Lord of Order"))
                return new[]
                {
                    "Fate Tonic",
                    "Unstable Divine Elixir",
                    "Felicitous Philtre"
                };

            if (HasAssignedClass("King's Echo"))
                return new[]
                {
                    "Fate Tonic",
                    "Potent Destruction Elixir",
                    "Felicitous Philtre"
                };

            return new[]
            {
                "Fate Tonic",
                "Potent Destruction Elixir",
                "Felicitous Philtre"
            };
        }

        if (context.Equals("AstralEmpyrean", StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                Ultra.GetBestTonicPotion(),
                Ultra.GetBestElixirPotion(),
                GetHonorOrMalicePotion()
            };
        }

        if (context.Equals("Speaker", StringComparison.OrdinalIgnoreCase))
        {
            if (HasAssignedClass("ArchPaladin"))
                return new[]
                {
                    "Body Tonic",
                    "Potent Destruction Elixir",
                    GetHonorOrMalicePotion()
                };

            if (HasAssignedClass("StoneCrusher"))
                return new[]
                {
                    "Body Tonic",
                    "Unstable Divine Elixir",
                    GetHonorOrMalicePotion()
                };

            if (HasAssignedClass("Lord of Order"))
                return new[]
                {
                    "Body Tonic",
                    "Unstable Divine Elixir",
                    GetHonorOrMalicePotion()
                };

            if (HasAssignedClass("Verus DoomKnight"))
                return new[]
                {
                    "Body Tonic",
                    "Potent Destruction Elixir",
                    GetHonorOrMalicePotion()
                };

            if (HasAssignedClass("Void Highlord"))
                return new[]
                {
                    "Body Tonic",
                    "Potent Destruction Elixir",
                    GetHonorOrMalicePotion()
                };

            if (HasAssignedClass("King's Echo"))
                return new[]
                {
                    "Fate Tonic",
                    "Potent Destruction Elixir",
                    GetHonorOrMalicePotion()
                };

            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };
        }

        if (HasAssignedClass("ArchPaladin"))
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("StoneCrusher"))
            return new[]
            {
                "Body Tonic",
                "Unstable Divine Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("Lord of Order"))
            return new[]
            {
                "Body Tonic",
                "Unstable Divine Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("King's Echo"))
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("Arcana Invoker"))
            return new[]
            {
                "Fate Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("Verus DoomKnight"))
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                GetHonorOrMalicePotion()
            };

        if (HasAssignedClass("Shaman"))
            return new[]
            {
                "Body Tonic",
                "Potent Destruction Elixir",
                "Felicitous Philtre"
            };

        if (HasAssignedClass("ArchFiend"))
            return new[]
            {
                "Body Tonic",
                "Potent Revitalize Elixir",
                "Felicitous Philtre"
            };

        // Default fallback when the current class does not match any preset
        return new[]
        {
            Ultra.GetBestTonicPotion(),
            Ultra.GetBestElixirPotion(),
            GetHonorOrMalicePotion()
        };
    }

    #endregion

    #region Potion Usage
    public void ActivateEquippedPotion()
    {
        // Only activate if a recognized potion is equipped (not a scroll/consumable)
        bool hasPotion = Bot.Inventory.IsEquipped("Potent Honor Potion")
            || Bot.Inventory.IsEquipped("Potent Malice Potion")
            || Bot.Inventory.IsEquipped("Potent Life Potion")
            || Bot.Inventory.IsEquipped("Felicitous Philtre")
            || Bot.Inventory.IsEquipped("Endurance Draught");

        if (hasPotion && Bot.Skills.CanUseSkill(5))
            C.Cast(5);
    }

    private bool HasSelfAura(string auraName)
    {
        if (string.IsNullOrWhiteSpace(auraName))
            return false;

        try
        {
            return Bot?.Self?.Auras?.Any(a => a?.Name != null && auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase)) == true;
        }
        catch
        {
            return false;
        }
    }

    public void UseRecommendedPotions(int desiredQuant = 10, bool skipThird = false, string context = "", bool ensureStock = false)
    {
        string[] potions = GetRecommendedPotions(context);

        if (potions.Length == 0)
            return;

        if (skipThird && potions.Length >= 3)
            potions = potions[..2];

        if (ensureStock)
            EnsurePotions(desiredQuant, skipThird, context);

        foreach (string potion in potions)
        {
            // Already active — aura name matches potion name for most consumables
            if (HasSelfAura(potion))
            {
                Core.Logger($"{potion} aura already active.");
                continue;
            }

            Core.Logger($"Equipping {potion}...");

            for (int attempt = 0; attempt < 2 && !Bot.Inventory.IsEquipped(potion); attempt++)
            {
                C.EquipConsumable(potion);
                Bot.Sleep(500);
            }

            // Some consumables auto-use when equipped
            if (HasSelfAura(potion))
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

        missing = missing
            .Where(potion => Bot.Inventory.GetQuantity(potion) < desiredQuant)
            .ToList();

        if (missing.Count > 0)
        {
            Core.Logger(
                $"Potion purchase ended with missing items: {string.Join(", ", missing)}. " +
                "No further purchase retries are performed to avoid infinite loops.",
                stopBot: false
            );
            return;
        }

        Core.Logger("Potion purchase complete.");
    }

    public void EnsureRecommendedPotions(int desiredQuant = 10, bool skipThird = false, string context = "")
        => EnsurePotions(desiredQuant, skipThird, context);

    public void PreparePotions(int desiredQuant = 10, bool skipThird = false, string context = "")
    {
        var potions = new List<string>
        {
            "Fate Tonic",
            "Body Tonic",
            "Sage Tonic",
            "Might Tonic",
            "Wise Tonic",
            "Potent Destruction Elixir",
            "Divine Elixir",
            "Unstable Divine Elixir",
            "Potent Malevolence Elixir",
            "Potent Battle Elixir",
            "Potent Revitalize Elixir",
            "Potent Honor Potion",
            "Endurance Draught"
        };

        if (skipThird && potions.Count >= 3)
            potions = potions.Take(2).ToList();

        Core.Logger($"Preparing potions: {string.Join(", ", potions)}");
        Pots.INeedYourStrongestPotions(
            Potions: potions.ToArray(),
            PotionQuant: desiredQuant,
            Seperate: true,
            BuyReagents: true
        );
    }

    #endregion
}
