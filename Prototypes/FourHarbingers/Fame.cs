/*
name: Fame
description: Defeat Fame using Yami no Ronin or Verus DoomKnight
tags: four harbingers, fourharbingers, fame, boss, farm, signet of the filled chalice, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Fame
{
    public string OptionsStorage = "FourHarbingers_Fame";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Fame.", ClassChoice.Yami_no_Ronin),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<bool>("FarmFame", "Farm Fame?", "Farm Fame repeatedly.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Config!.Get<bool>(CoreBots.Instance.SkipOptions))
            Bot.Config.Configure();

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
        Core.RegisterQuests(10852);

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

        Bot.Wait.ForQuestComplete(10852);
        return true;
    }

    private bool FightBoss()
    {
        StopSkills();

        try
        {
            Core.Join("fourharbingers-100000", "r4", "Bottom");

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of the Filled Chalice"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Join("fourharbingers-100000", "r4", "Bottom");
                    continue;
                }

                if (!Bot.Player.Cell.Equals("r4", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Jump("r4", "Bottom");
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 3)
                    Bot.Combat.Attack(3);

                UseSkills();
                RefreshThirdPotion();
                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Combat.CancelAutoAttack();
        }

        return Bot.TempInv.Contains("Signet of the Filled Chalice");
    }

    private void UseSkills()
    {
        if (GetSelectedClass() == "Yami no Ronin")
        {
            if (Bot.Skills.CanUseSkill(3))
                Bot.Skills.UseSkill(3);
            else if (Bot.Skills.CanUseSkill(2))
                Bot.Skills.UseSkill(2);
            else if (Bot.Skills.CanUseSkill(1))
                Bot.Skills.UseSkill(1);
        }
        else
        {
            if (Bot.Skills.CanUseSkill(2))
                Bot.Skills.UseSkill(2);
            else if (Bot.Skills.CanUseSkill(1))
                Bot.Skills.UseSkill(1);
            else if (Bot.Skills.CanUseSkill(3))
                Bot.Skills.UseSkill(3);
            else if (Bot.Skills.CanUseSkill(4))
                Bot.Skills.UseSkill(4);
        }
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
            "Scroll of the Benevolent",
            "Scroll of the Quartet",
            "Signet of the Filled Chalice"
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

        return Bot.Config!.Get<bool>("FarmFame");
    }

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "Yami no Ronin";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");

        string preferredClass = classChoice == ClassChoice.Verus_DoomKnight ? "Verus DoomKnight" : "Yami no Ronin";
        string alternateClass = classChoice == ClassChoice.Verus_DoomKnight ? "Yami no Ronin" : "Verus DoomKnight";

        if (Core.CheckInventory(preferredClass))
            return preferredClass;

        if (Core.CheckInventory(alternateClass))
        {
            Core.Logger($"WARNING: {preferredClass} is not available. Using {alternateClass} instead.");
            return alternateClass;
        }

        Core.Logger("WARNING: Either Yami no Ronin or Verus DoomKnight is required for this setup.");
        return string.Empty;
    }

    private bool EquipClass()
    {
        string className = GetSelectedClass();

        if (string.IsNullOrEmpty(className))
            return false;

        if (!Bot.Inventory.Contains(className))
        {
            if (!Bot.Bank.Contains(className))
            {
                Core.Logger($"WARNING: {className} is not in your inventory or bank.");
                return false;
            }

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

        if (GetSelectedClass() == "Yami no Ronin")
        {
            if (Adv.uValiance())
                weaponEnhancement = WeaponSpecial.Valiance;
            else
            {
                Core.Logger("WARNING: Valiance is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uVim())
                helmEnhancement = HelmSpecial.Vim;
            else
                Core.Logger("WARNING: Vim is not unlocked. Lucky will be used on the helm instead.");
        }
        else
        {
            if (Adv.uDauntless())
                weaponEnhancement = WeaponSpecial.Dauntless;
            else
            {
                Core.Logger("WARNING: Dauntless is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uAnima())
                helmEnhancement = HelmSpecial.Anima;
            else
                Core.Logger("WARNING: Anima is not unlocked. Lucky will be used on the helm instead.");
        }

        if (Adv.uAvarice())
            capeEnhancement = CapeSpecial.Avarice;
        else
            Core.Logger("WARNING: Avarice is not unlocked. Lucky will be used on the cape instead.");

        if (weaponEnhancement == WeaponSpecial.Health_Vamp)
        {
            if (!Adv.uAwe())
                Core.Logger("WARNING: Awe enhancements are not unlocked. Enhancement setup will continue.");
        }

        Adv.EnhanceEquipped(EnhancementType.Lucky, capeEnhancement, helmEnhancement, weaponEnhancement, true);
    }

    private void GetPotions()
    {
        GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8, "", 0);
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

        UsePotion("Fate Tonic", "Fate");
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

        if (Bot.Inventory.GetQuantity("Fate Tonic") > 1
            && Bot.Inventory.GetQuantity("Potent Battle Elixir") > 1
            && Bot.Inventory.GetQuantity("Felicitous Philtre") > 1)
            return;

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
        Yami_no_Ronin,
        Verus_DoomKnight,
    }
}
