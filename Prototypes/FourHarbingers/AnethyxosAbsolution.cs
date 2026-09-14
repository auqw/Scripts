/*
name: Anethyx'o's Absolution
description: Defeat Anethyx'o's Absolution using King's Echo or Chaos Avenger
tags: four harbingers, fourharbingers, anethyxos, anethyxos absolution, boss, farm, signet of the broken bond, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Options;

public class AnethyxosAbsolution
{
    public string OptionsStorage = "FourHarbingers_AnethyxosAbsolution";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Anethyx'o's Absolution.", ClassChoice.Kings_Echo),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<bool>("FarmAnethyxosAbsolution", "Farm Anethyx'o's Absolution?", "Farm Anethyx'o's Absolution repeatedly.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private bool _dieNow;
    private DateTimeOffset _dieNowDetectedAt;
    private bool _lowHealthSkills;
    private bool _dieNowHandled;

    public void ScriptMain(IScriptInterface bot)
    {
        if (bot.Config != null)
            bot.Config.Configure();

        Core.SetOptions(disableClassSwap: true);
        Bot.UltraBossHelper.DisableCounterAttack();
        Bot.Flash.FlashCall -= AbsolutionFlashListener;
        Bot.Flash.FlashCall += AbsolutionFlashListener;
        try
        {
            Run();
        }
        finally
        {
            Bot.Flash.FlashCall -= AbsolutionFlashListener;
            Core.CancelRegisteredQuests();
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
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

        Bot.Wait.ForQuestComplete(10854);
        return true;
    }

    private bool FightBoss()
    {
        _dieNow = false;
        _dieNowDetectedAt = DateTimeOffset.MinValue;
        _lowHealthSkills = false;
        _dieNowHandled = false;

        try
        {
            Core.Join("fourharbingers-100000", "r6", "Bottom");
            StartNormalSkills();

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of the Broken Bond"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    if (Bot.Player.Alive)
                    {
                        _dieNow = false;
                        _dieNowDetectedAt = DateTimeOffset.MinValue;
                        _lowHealthSkills = false;
                        StartNormalSkills();
                        Bot.Skills.Resume();
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

                if (HandleDieNow())
                {
                    Bot.Sleep(100);
                    continue;
                }

                ApplyLowHealthSkills();

                if (StopForAuras())
                {
                    Bot.Sleep(100);
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 5)
                    Bot.Combat.Attack(5);

                Bot.Skills.Resume();
                RefreshThirdPotion();
                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Combat.CancelAutoAttack();
            Bot.Skills.Resume();
            _dieNow = false;
            _dieNowDetectedAt = DateTimeOffset.MinValue;
            _lowHealthSkills = false;
            _dieNowHandled = false;
        }

        return Bot.TempInv.Contains("Signet of the Broken Bond");
    }

    private void StartNormalSkills()
    {
        if (GetSelectedClass() == "King's Echo")
            ReplaceSkillProvider("3 | 1 | 2 | 1 | 2 | 3 | 1 | 2 | 1 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
        else
            ReplaceSkillProvider("3 | 4 | 2 | 1", 250, SkillUseMode.UseIfAvailable);
    }

    private void ReplaceSkillProvider(string skills, int skillTimeout, SkillUseMode skillMode)
    {
        Bot.Skills.LoadAdvanced(skills, skillTimeout, skillMode);

        if (Bot.Skills.OverrideProvider != null)
            Bot.Skills.SetProvider(Bot.Skills.OverrideProvider);

        // Start the timer only once. Farming kills only replace the provider.
        if (!Bot.Skills.TimerRunning)
            Bot.Skills.Start();
    }

    private void ApplyLowHealthSkills()
    {
        if (_lowHealthSkills || _dieNowHandled)
            return;

        var boss = Bot.Monsters.MapMonsters.FirstOrDefault(m => m.MapID == 5);
        if (boss == null || boss.MaxHP <= 0 || boss.HP <= 0 || boss.HP > boss.MaxHP * 0.30)
            return;

        _lowHealthSkills = true;
        if (GetSelectedClass() == "King's Echo")
            ReplaceSkillProvider("1 | 2", 250, SkillUseMode.WaitForCooldown);
        else
            ReplaceSkillProvider("3 | 4 | 2", 250, SkillUseMode.UseIfAvailable);
    }

    private bool StopForAuras()
    {
        if (!HasBossAura("Counter Attack") && !Bot.Self.HasActiveAura("Crits Inverted"))
            return false;

        Bot.Skills.Pause();
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();
        return true;
    }

    private bool HandleDieNow()
    {
        if (!_dieNow || _dieNowHandled)
            return false;

        Bot.Skills.Pause();

        if (GetSelectedClass() == "King's Echo"
            && (Bot.Self.HasActiveAura("Waiting For Corvak") || Bot.Player.Mana <= Bot.Player.MaxMana / 2))
        {
            if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 5)
            {
                Bot.Combat.Attack(5);
                return true;
            }

            if (Bot.Skills.CanUseSkill(4))
            {
                Bot.Skills.UseSkill(4);
                Bot.Sleep(100);
            }
            Bot.Combat.CancelAutoAttack();
            return true;
        }

        if (DateTimeOffset.UtcNow - _dieNowDetectedAt < TimeSpan.FromSeconds(2))
        {
            Bot.Combat.CancelAutoAttack();
            return true;
        }

        if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 5)
        {
            Bot.Combat.Attack(5);
            return true;
        }

        if (GetSelectedClass() == "King's Echo")
        {
            if (!Bot.Skills.CanUseSkill(3))
            {
                Bot.Combat.CancelAutoAttack();
                return true;
            }

            if (!Bot.Skills.UseSkill(3))
            {
                Bot.Combat.CancelAutoAttack();
                return true;
            }
        }
        else
        {
            if (!Bot.Skills.CanUseSkill(1))
            {
                Bot.Combat.CancelAutoAttack();
                return true;
            }

            if (!Bot.Skills.UseSkill(1))
            {
                Bot.Combat.CancelAutoAttack();
                return true;
            }
        }

        _dieNow = false;
        _dieNowDetectedAt = DateTimeOffset.MinValue;
        _lowHealthSkills = false;
        _dieNowHandled = true;
        StartNormalSkills();
        Bot.Skills.Resume();
        return true;
    }

    private bool HasBossAura(string auraName)
    {
        try
        {
            List<Aura>? auras = JsonConvert.DeserializeObject<List<Aura>>(Bot.Target.GetMonsterAura(5));
            if (auras == null)
                return false;

            return auras.Any(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    private void AbsolutionFlashListener(string name, object[] args)
    {
        try
        {
            if (Bot.ShouldExit
                || !Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase)
                || !name.Equals("packetFromServer", StringComparison.OrdinalIgnoreCase)
                || args.Length == 0
                || args[0] is not string rawPacket)
                return;

            dynamic packet = JsonConvert.DeserializeObject<dynamic>(rawPacket);
            dynamic data = packet?["b"]?["o"];
            if (data?.cmd?.ToString() != "ct" || data?.anims is null)
                return;

            foreach (dynamic animation in data.anims)
            {
                if (animation?.msg?.ToString() != "Die now.")
                    continue;

                // Ignore duplicate/late Die now packets once this mechanic is active or handled.
                if (_dieNow || _dieNowHandled)
                    return;

                _dieNow = true;
                _dieNowDetectedAt = DateTimeOffset.UtcNow;
                Bot.Skills.Pause();
                Bot.Combat.CancelAutoAttack();
                Core.Logger("Die now detected. Preparing the defensive skill.");
                return;
            }
        }
        catch
        {
        }
    }

    private void AddDrops()
    {
        Bot.Drops.Add(
            "Anethyx’o’s Absolution Orbs",
            "Manticore of Infinity",
            "Scroll of the Heretic",
            "Scroll of the Quartet",
            "Signet of the Broken Bond"
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

        return Bot.Config!.Get<bool>("FarmAnethyxosAbsolution");
    }

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "King's Echo";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.Kings_Echo)
            return "King's Echo";
        else if (classChoice == ClassChoice.Chaos_Avenger)
            return "Chaos Avenger";
        else
            return "King's Echo";
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

        if (GetSelectedClass() == "King's Echo")
        {
            if (Adv.uElysium())
                weaponEnhancement = WeaponSpecial.Elysium;
            else
            {
                Core.Logger("WARNING: Elysium is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uExamen())
                helmEnhancement = HelmSpecial.Examen;
            else
                Core.Logger("WARNING: Examen is not unlocked. Lucky will be used on the helm instead.");
        }
        else
        {
            if (Adv.uPraxis())
                weaponEnhancement = WeaponSpecial.Praxis;
            else
            {
                Core.Logger("WARNING: Praxis is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uAnima())
                helmEnhancement = HelmSpecial.Anima;
            else
                Core.Logger("WARNING: Anima is not unlocked. Lucky will be used on the helm instead.");
        }

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
        GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8, "", 0);
        if (GetSelectedClass() == "King's Echo")
            GetPotion("Potent Revitalize Elixir", "Gold Voucher 500k", 8, 500000, 20, 0, "", 0);
        else
            GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 20, 0, "", 0);
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
        if (GetSelectedClass() == "King's Echo")
            UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
        else
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

        if (GetSelectedClass() == "King's Echo")
        {
            if (Bot.Inventory.GetQuantity("Fate Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Revitalize Elixir") > 1
                && Bot.Inventory.GetQuantity("Felicitous Philtre") > 1)
                return;
        }
        else
        {
            if (Bot.Inventory.GetQuantity("Fate Tonic") > 1
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
        Kings_Echo,
        Chaos_Avenger,
    }
}
