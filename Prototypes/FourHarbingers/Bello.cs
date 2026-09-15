/*
name: Bello
description: Defeat Bello using ArchPaladin or Chaos Avenger
tags: four harbingers, fourharbingers, bello, boss, farm, signet of the endless journey, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Options;

public class Bello
{
    public string OptionsStorage = "FourHarbingers_Bello";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Bello.", ClassChoice.ArchPaladin),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<bool>("FarmBello", "Farm Bello?", "Farm Bello repeatedly.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Config.Get<bool>(CoreBots.Instance.SkipOptions))
            Bot.Config?.Configure();

        Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            StopSkills();
            Core.CancelRegisteredQuests();
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        AddDrops();
        Core.RegisterQuests(10851);

        if (!EquipClass())
            return;

        if (DoEnhancementsEnabled())
            ApplyEnhancements();

        if (UsePotionsEnabled())
            GetPotions();

        Core.Join("fourharbingers-100000", "Enter", "Spawn");

        if (UsePotionsEnabled())
            UsePotions();

        if (FarmBossEnabled())
        {
            while (!Bot.ShouldExit)
            {
                if (!DoQuest())
                    return;

                RestockPotions();
            }
        }
        else
            DoQuest();
    }

    private bool DoQuest()
    {
        if (!FightBoss())
            return false;

        Bot.Wait.ForQuestComplete(10851);
        return true;
    }

    private bool FightBoss()
    {
        try
        {
            Core.Join("fourharbingers-100000", "r3", "Bottom");
            Bot.Skills.Resume();

            if (GetSelectedClass() == "ArchPaladin")
                Bot.Skills.StartAdvanced("3 | 2 | 1", 250, SkillUseMode.UseIfAvailable);
            else
                Bot.Skills.StartAdvanced("3 | 4 | 2 | 1", 250, SkillUseMode.UseIfAvailable);

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of the Endless Journey"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Join("fourharbingers-100000", "r3", "Bottom");
                    continue;
                }

                if (!Bot.Player.Cell.Equals("r3", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Jump("r3", "Bottom");
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 2)
                    Bot.Combat.Attack(2);

                if (GetSelectedClass() == "ArchPaladin")
                    ArchPaladinMechanics();

                RefreshThirdPotion();
                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Combat.CancelAutoAttack();
            Bot.Skills.Resume();
        }

        return Bot.TempInv.Contains("Signet of the Endless Journey");
    }

    private void ArchPaladinMechanics()
    {
        var righteousSeal = Bot.Target.GetAura("Righteous Seal");
        if (righteousSeal != null && righteousSeal.RemainingTime < 1 && Bot.Skills.CanUseSkill(4))
            Bot.Skills.UseSkill(4);
    }

    private void StopSkills()
    {
        Bot.Skills.Resume();
        Bot.Skills.Stop();
        Bot.Wait.ForTrue(() => !Bot.Skills.TimerRunning, 20);
    }

    private void AddDrops()
    {
        Bot.Drops.Add(
            "Infernal Winged Manticore",
            "Manticore of Malice",
            "Scroll of the Preacher",
            "Scroll of the Quartet",
            "Signet of the Endless Journey"
        );
    }

    private bool UsePotionsEnabled()
    {
        return Bot.Config!.Get<bool>("UsePotions");
    }

    private bool DoEnhancementsEnabled()
    {
        return Bot.Config!.Get<bool>("DoEnhancements");
    }

    private bool FarmBossEnabled()
    {
        if (DoAllMode)
            return false;

        return Bot.Config!.Get<bool>("FarmBello");
    }

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "ArchPaladin";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.ArchPaladin)
            return "ArchPaladin";
        else if (classChoice == ClassChoice.Chaos_Avenger)
            return "Chaos Avenger";
        else
            return "ArchPaladin";
    }

    private bool EquipClass()
    {
        string className = GetSelectedClass();
        if (!Core.CheckInventory(className))
        {
            Core.Logger($"WARNING: {className} is required for this setup.");
            return false;
        }

        if (!Bot.Inventory.Contains(className))
        {
            if (Bot.Inventory.FreeSlots <= 0)
            {
                Core.Logger($"WARNING: {className} is in the bank, but no free inventory slot is available.");
                return false;
            }

            Bot.Bank.EnsureToInventory(className);
            Bot.Wait.ForTrue(() => Bot.Inventory.Contains(className), 20);
            if (!Bot.Inventory.Contains(className))
            {
                Core.Logger($"WARNING: {className} could not be moved from the bank.");
                return false;
            }
        }

        Core.Equip(className);
        Bot.Wait.ForItemEquip(className);
        if (!Bot.Inventory.IsEquipped(className))
        {
            Core.Logger($"WARNING: {className} could not be equipped.");
            return false;
        }

        return true;
    }

    private void ApplyEnhancements()
    {
        WeaponSpecial weaponEnhancement;
        HelmSpecial helmEnhancement = HelmSpecial.None;
        CapeSpecial capeEnhancement = CapeSpecial.None;

        if (Adv.uValiance())
            weaponEnhancement = WeaponSpecial.Valiance;
        else
        {
            Core.Logger("WARNING: Valiance is not unlocked. Health Vamp will be used instead.");
            weaponEnhancement = WeaponSpecial.Health_Vamp;
        }

        if (Adv.uForgeHelm())
            helmEnhancement = HelmSpecial.Forge;
        else
            Core.Logger("WARNING: Forge helm is not unlocked. Lucky will be used on the helm instead.");

        if (Adv.uPenitence())
            capeEnhancement = CapeSpecial.Penitence;
        else
            Core.Logger("WARNING: Penitence is not unlocked. Lucky will be used on the cape instead.");

        if (weaponEnhancement == WeaponSpecial.Health_Vamp)
        {
            if (!Adv.uAwe())
                Core.Logger("WARNING: Awe enhancements are not unlocked. Enhancement setup will continue.");
        }

        Adv.EnhanceEquipped(EnhancementType.Lucky, capeEnhancement, helmEnhancement, weaponEnhancement, true);
    }

    private void GetPotions()
    {
        if (GetSelectedClass() == "ArchPaladin")
            GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8, "", 0);
        else
            GetPotion("Might Tonic", "Gold Voucher 500k", 2, 500000, 10, 8, "", 0);

        GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8, 0, "", 0);
        GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100, 0, "", 0);
    }

    private void GetPotion(string itemName, string voucherName, int voucherQuantity, int voucherCost,
        int targetQuantity, int requiredAlchemyRank, string requiredFaction, int requiredFactionRank)
    {
        try
        {
            if (Bot.Inventory.GetQuantity(itemName) > 1)
                return;

            if (Bot.Bank.Contains(itemName))
            {
                if (!Bot.Inventory.Contains(itemName))
                {
                    if (Bot.Inventory.FreeSlots <= 0)
                    {
                        WarnPotion(itemName, "no free inventory slot is available");
                        return;
                    }
                }

                Bot.Bank.EnsureToInventory(itemName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);
                if (Bot.Inventory.GetQuantity(itemName) > 1)
                    return;
            }

            if (requiredAlchemyRank > 0)
            {
                if (!Bot.Reputation.HasRank("Alchemy", requiredAlchemyRank))
                {
                    WarnPotion(itemName, $"Alchemy rank {requiredAlchemyRank} is required");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(requiredFaction))
            {
                if (!Bot.Reputation.HasRank(requiredFaction, requiredFactionRank))
                {
                    WarnPotion(itemName, $"{requiredFaction} rank {requiredFactionRank} is required");
                    return;
                }
            }

            int requiredSlots = 0;
            if (!Bot.Inventory.Contains(itemName))
                requiredSlots++;
            if (!Bot.Inventory.Contains(voucherName))
                requiredSlots++;
            if (Bot.Inventory.FreeSlots < requiredSlots)
            {
                WarnPotion(itemName, $"{requiredSlots} free inventory slots are required");
                return;
            }

            if (Bot.Bank.Contains(voucherName))
            {
                Bot.Bank.EnsureToInventory(voucherName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(voucherName), 20);
            }

            int missingVouchers = voucherQuantity - Bot.Inventory.GetQuantity(voucherName);
            if (missingVouchers < 0)
                missingVouchers = 0;

            int requiredGold = missingVouchers * voucherCost;
            if (Bot.Player.Gold < requiredGold)
            {
                WarnPotion(itemName, $"{requiredGold} gold is required");
                return;
            }

            Core.Join("alchemyacademy");
            Bot.Shops.Load(2036);
            if (!Bot.Shops.IsLoaded || Bot.Shops.ID != 2036)
            {
                WarnPotion(itemName, "the potion shop could not be loaded");
                return;
            }

            if (missingVouchers > 0)
            {
                Core.BuyItem("alchemyacademy", 2036, voucherName, voucherQuantity);
                Bot.Wait.ForTrue(() => Bot.Inventory.GetQuantity(voucherName) >= voucherQuantity, 20);
            }

            if (Bot.Inventory.GetQuantity(voucherName) < voucherQuantity)
            {
                WarnPotion(itemName, "the required vouchers could not be purchased");
                return;
            }

            Core.BuyItem("alchemyacademy", 2036, itemName, targetQuantity);
            Bot.Wait.ForTrue(() => Bot.Inventory.GetQuantity(itemName) >= targetQuantity, 20);
            if (Bot.Inventory.GetQuantity(itemName) < targetQuantity)
                WarnPotion(itemName, "it could not be purchased");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion preparation failed for {itemName}: {ex}");
            WarnPotion(itemName, "preparation failed");
        }
    }

    private void UsePotions()
    {
        if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
        {
            Core.Logger("WARNING: Potions will only be used inside fourharbingers.");
            return;
        }

        if (GetSelectedClass() == "ArchPaladin")
            UsePotion("Fate Tonic", "Fate");
        else
            UsePotion("Might Tonic", "Might");

        UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
        UsePotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void UsePotion(string itemName, string auraName)
    {
        try
        {
            if (!Bot.Inventory.Contains(itemName))
            {
                WarnPotion(itemName, "it is not in the inventory");
                return;
            }

            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Wait.ForItemEquip(itemName);
            Bot.Sleep(2000);
            if (!Bot.Inventory.IsEquipped(itemName))
            {
                WarnPotion(itemName, "it could not be equipped");
                return;
            }

            if (Bot.Self.HasActiveAura(auraName))
                return;

            int quantityBefore = Bot.Inventory.GetQuantity(itemName);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(2000);
            Bot.Wait.ForTrue(() => Bot.Self.HasActiveAura(auraName) || Bot.Inventory.GetQuantity(itemName) < quantityBefore, 20);
            if (!Bot.Self.HasActiveAura(auraName) && Bot.Inventory.GetQuantity(itemName) >= quantityBefore)
                WarnPotion(itemName, "its effect could not be verified");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion use failed for {itemName}: {ex}");
            WarnPotion(itemName, "use failed");
        }
    }

    private void RestockPotions()
    {
        if (!UsePotionsEnabled())
            return;
        if (Bot.Player.InCombat)
            return;

        if (GetSelectedClass() == "ArchPaladin")
        {
            if (Bot.Inventory.GetQuantity("Fate Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Battle Elixir") > 1
                && Bot.Inventory.GetQuantity("Felicitous Philtre") > 1)
                return;
        }
        else
        {
            if (Bot.Inventory.GetQuantity("Might Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Battle Elixir") > 1
                && Bot.Inventory.GetQuantity("Felicitous Philtre") > 1)
                return;
        }

        GetPotions();
        Core.Join("fourharbingers-100000", "Enter", "Spawn");
        UsePotions();
    }

    private void RefreshThirdPotion()
    {
        if (Bot.Player.InCombat && !Bot.Self.HasActiveAura("Felicitous Philtre") && Bot.Skills.CanUseSkill(5))
            Bot.Skills.UseSkill(5);
    }

    private void WarnPotion(string itemName, string reason)
    {
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");
    }

    private enum ClassChoice
    {
        ArchPaladin,
        Chaos_Avenger,
    }
}
