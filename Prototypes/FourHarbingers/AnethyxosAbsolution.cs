/*
name: Anethyxos Absolution
description: Defeat Anethyxos using Chaos Avenger or Dragon of Time
tags: four harbingers, fourharbingers, anethyxos, absolution, boss, farm, signet of absolution, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AnethyxosAbsolution
{
    public string OptionsStorage = "FourHarbingers_AnethyxosAbsolution";
    public bool DontPreconfigure = true;
    public bool DoAllMode;
    public int FarmQuantity;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Anethyxos.", ClassChoice.Chaos_Avenger),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<bool>("FarmAnethyxos", "Farm Anethyxos?", "Farm Anethyxos repeatedly.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private string? _resolvedClass;

    private bool _dieNow;
    private bool _dieNowHandled;
    private DateTimeOffset _dieNowDetectedAt;

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Config.Get<bool>(CoreBots.Instance.SkipOptions))
            Bot.Config.Configure();

        if (!DoAllMode)
            Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            StopSkills();
            Bot.Flash.FlashCall -= AbsolutionFlashListener;
            Core.CancelRegisteredQuests();
            if (!DoAllMode)
                Core.SetOptions(false);
        }
    }

    private void Run()
    {
        _resolvedClass = null;
        AddDrops();
        Core.RegisterQuests(10854);

        if (!EquipClass())
            return;

        if (DoEnhancementsEnabled())
            ApplyEnhancements();

        if (UsePotionsEnabled())
            GetPotions();

        Core.Join("fourharbingers-100000", "Enter", "Spawn");

        if (UsePotionsEnabled())
            UsePotions();

        Bot.Flash.FlashCall -= AbsolutionFlashListener;
        Bot.Flash.FlashCall += AbsolutionFlashListener;

        if (FarmQuantity > 0)
        {
            while (!Bot.ShouldExit && Bot.Inventory.GetQuantity("Scroll of the Heretic") < FarmQuantity)
            {
                if (!DoQuest())
                    return;

                RestockPotions();
            }
        }
        else if (FarmBossEnabled())
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
        int scrollsBefore = Bot.Inventory.GetQuantity("Scroll of the Heretic");
        if (!FightBoss())
            return false;

        Bot.Wait.ForQuestComplete(10854);
        if (Bot.Inventory.GetQuantity("Scroll of the Heretic") <= scrollsBefore)
        {
            Bot.Drops.Pickup("Scroll of the Heretic");
            Bot.Wait.ForPickup("Scroll of the Heretic");
            if (Bot.Inventory.GetQuantity("Scroll of the Heretic") <= scrollsBefore)
                return false;
        }
        return true;
    }

    private bool FightBoss()
    {
        StopSkills();

        try
        {
            Core.Join("fourharbingers-100000", "r6", "Bottom");
            Bot.Wait.ForTrue(() =>
                Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase)
                && Bot.Player.Cell.Equals("r6", StringComparison.OrdinalIgnoreCase),
                20
            );

            StartSkills();

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of Absolution"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                    if (Bot.Player.Alive)
                    {
                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.CancelTarget();
                        Bot.Combat.Exit();
                        Bot.Wait.ForCombatExit();

                        Core.Join("fourharbingers-100000", "r6", "Bottom");
                        Bot.Wait.ForTrue(() =>
                            Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase)
                            && Bot.Player.Cell.Equals("r6", StringComparison.OrdinalIgnoreCase),
                            20
                        );

                        if (UsePotionsEnabled())
                        {
                            RestockPotions();
                            UsePotions();
                        }

                        _dieNow = false;
                        _dieNowHandled = false;
                        StartSkills();
                    }
                    continue;
                }

                if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Join("fourharbingers-100000", "r6", "Bottom");
                    continue;
                }

                if (!Bot.Player.Cell.Equals("r6", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Jump("r6", "Bottom");
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 5)
                    Bot.Combat.Attack(5);

                ChaosAvengerMechanics();
                RefreshThirdPotion();
                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Combat.CancelAutoAttack();
            Bot.Skills.Resume();
        }

        return Bot.TempInv.Contains("Signet of Absolution");
    }

    private void StartSkills()
    {
        if (GetSelectedClass() == "Chaos Avenger")
            Bot.Skills.StartAdvanced("3 | 4 | 2 | 1", 250, SkillUseMode.UseIfAvailable);
        else
            Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4 | 2", 250, SkillUseMode.WaitForCooldown);
    }

    private void ChaosAvengerMechanics()
    {
        // If Die Now was detected, handle it safely
        if (_dieNow && !_dieNowHandled)
        {
            double elapsed = (DateTimeOffset.UtcNow - _dieNowDetectedAt).TotalMilliseconds;

            // Stop ALL outgoing actions
            Bot.Skills.Pause();
            Bot.Combat.CancelAutoAttack();

            // Wait for the safe window to fire skill 1
            if (elapsed >= 2400 && Bot.Skills.CanUseSkill(1))
            {
                Bot.Skills.UseSkill(1);

                _dieNowHandled = true;
                _dieNow = false;

                // Resume normal rotation
                StartSkills();
                Bot.Skills.Resume();
            }
            return;
        }

        // Normal rotation
        Bot.Skills.Resume();
    }


    private void AbsolutionFlashListener(string name, object[] args)
    {
        try
        {
            if (Bot.ShouldExit)
                return;
            if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
                return;
            if (!name.Equals("packetFromServer", StringComparison.OrdinalIgnoreCase))
                return;
            if (args.Length == 0)
                return;
            if (GetSelectedClass() != "Chaos Avenger")
                return;

            if (_dieNow || _dieNowHandled)
                return;

            string? rawPacket = args[0] as string;
            if (string.IsNullOrWhiteSpace(rawPacket))
                return;

            dynamic? packet = JsonConvert.DeserializeObject<dynamic>(rawPacket);
            if (packet == null)
                return;

            dynamic body = packet["b"];
            if (body == null)
                return;

            dynamic data = body["o"];
            if (data == null)
                return;

            dynamic cmd = data["cmd"];
            if (cmd == null || cmd.ToString() != "ct")
                return;

            dynamic anims = data["anims"];
            if (anims == null)
                return;

            foreach (dynamic anim in anims)
            {
                dynamic msg = anim["msg"];
                if (msg == null)
                    continue;

                string message = msg.ToString();
                if (message == "Die now." || message == "Die now")
                {
                    _dieNow = true;
                    _dieNowDetectedAt = DateTimeOffset.UtcNow;

                    // STOP attacking immediately
                    Bot.Skills.Pause();
                    Bot.Combat.CancelAutoAttack();

                    Core.Logger("Die now detected. Preparing defensive skill.");
                    return;
                }

            }
        }
        catch
        {
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
            "Scroll of the Heretic",
            "Scroll of the Quartet",
            "Signet of Absolution"
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

        return Bot.Config!.Get<bool>("FarmAnethyxos");
    }

    private string GetSelectedClass()
    {
        if (!string.IsNullOrEmpty(_resolvedClass))
            return _resolvedClass;

        if (DoAllMode)
            return "Chaos Avenger";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.Chaos_Avenger)
            return "Chaos Avenger";
        else if (classChoice == ClassChoice.Dragon_of_Time)
            return "Dragon of Time";
        else
            return "Chaos Avenger";
    }

    private bool EquipClass()
    {
        string className = GetSelectedClass();

        if (!Core.CheckInventory(className, toInv: false))
        {
            if (DoAllMode)
            {
                Core.Logger($"WARNING: {className} is required for this setup.");
                return false;
            }

            string fallbackClass = className == "Chaos Avenger" ? "Dragon of Time" : "Chaos Avenger";
            if (!Core.CheckInventory(fallbackClass, toInv: false))
            {
                Core.Logger(
                    $"WARNING: You do not own {className} or {fallbackClass}. The script will stop.",
                    messageBox: true
                );
                return false;
            }

            Core.Logger(
                $"WARNING: {className} was selected, but you do not own it. Falling back to {fallbackClass}.",
                messageBox: true
            );
            className = fallbackClass;
        }

        _resolvedClass = className;

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

        if (GetSelectedClass() == "Chaos Avenger")
        {
            if (Adv.uPraxis())
                weaponEnhancement = WeaponSpecial.Praxis;
            else
            {
                WarnEnhancementFallback("Praxis is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uAnima())
                helmEnhancement = HelmSpecial.Anima;
            else
                WarnEnhancementFallback("Anima is not unlocked. Lucky will be used on the helm instead.");
        }
        else
        {
            if (Adv.uElysium())
                weaponEnhancement = WeaponSpecial.Elysium;
            else if (Adv.uValiance())
            {
                WarnEnhancementFallback("Elysium is not unlocked. Valiance will be used instead.");
                weaponEnhancement = WeaponSpecial.Valiance;
            }
            else
            {
                WarnEnhancementFallback("Elysium and Valiance are not unlocked. Awe Blast will be used instead.");
                weaponEnhancement = WeaponSpecial.Awe_Blast;
            }

            if (Adv.uPneuma())
                helmEnhancement = HelmSpecial.Pneuma;
            else
                WarnEnhancementFallback("Pneuma is not unlocked. Wizard will be used on the helm instead.");
        }

        if (Adv.uVainglory())
            capeEnhancement = CapeSpecial.Vainglory;
        else
            WarnEnhancementFallback("Vainglory is not unlocked. Wizard will be used on the cape instead.");

        if (weaponEnhancement == WeaponSpecial.Health_Vamp)
        {
            if (!Adv.uAwe())
                WarnEnhancementFallback("Awe enhancements are not unlocked. Enhancement setup will continue.");
        }

        Adv.EnhanceEquipped(
            GetSelectedClass() == "Chaos Avenger" ? EnhancementType.Lucky : EnhancementType.Wizard,
            capeEnhancement,
            helmEnhancement,
            weaponEnhancement,
            true
        );
    }

    private void WarnEnhancementFallback(string message)
    {
        Core.Logger($"WARNING: {message} The script may fail.", messageBox: true);
    }

    private void GetPotions()
    {
        if (GetSelectedClass() == "Chaos Avenger")
        {
            GetPotion("Might Tonic", "Gold Voucher 500k", 2, 500000, 10, 8, "", 0);
            GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8, 0, "", 0);
            GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100, 0, "", 0);
        }
        else
        {
            GetPotion("Sage Tonic", "Gold Voucher 500k", 2, 500000, 10, 8, "", 0);
            GetPotion("Potent Malevolence Elixir", "Gold Voucher 500k", 4, 500000, 8, 0, "", 0);
            GetPotion("Potent Honor Potion", "Gold Voucher 500k", 4, 500000, 20, 0, "Good", 10);
        }
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
        }
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
            Bot.Wait.ForTrue(() =>
                Bot.Self.HasActiveAura(auraName)
                || Bot.Inventory.GetQuantity(itemName) < quantityBefore,
                20
            );

            if (!Bot.Self.HasActiveAura(auraName)
                && Bot.Inventory.GetQuantity(itemName) >= quantityBefore)
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
        {
            Bot.Sleep(500);
            if (Bot.Player.InCombat)
                return;
        }

        if (GetSelectedClass() == "Chaos Avenger")
        {
            if (Bot.Inventory.GetQuantity("Might Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Battle Elixir") > 1
                && Bot.Inventory.GetQuantity("Felicitous Philtre") > 1)
                return;
        }
        else
        {
            if (Bot.Inventory.GetQuantity("Sage Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Malevolence Elixir") > 1
                && Bot.Inventory.GetQuantity("Potent Honor Potion") > 1)
                return;
        }

        GetPotions();
        Core.Join("fourharbingers-100000", "Enter", "Spawn");
        UsePotions();
    }

    private void RefreshThirdPotion()
    {
        if (GetSelectedClass() == "Chaos Avenger")
        {
            if (Bot.Player.InCombat
                && !Bot.Self.HasActiveAura("Felicitous Philtre")
                && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        else
        {
            if (Bot.Player.InCombat
                && !Bot.Self.HasActiveAura("Potent Honor Malice")
                && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
    }

    private void WarnPotion(string itemName, string reason)
    {
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");
    }

    private enum ClassChoice
    {
        Chaos_Avenger,
        Dragon_of_Time,
    }
}

