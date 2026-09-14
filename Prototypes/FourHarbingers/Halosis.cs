/*
name: Halosis
description: Defeat Halosis using Dragon of Time or Chaos Avenger
tags: four harbingers, fourharbingers, halosis, boss, farm, signet of inner conflict, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Halosis
{
    public string OptionsStorage = "FourHarbingers_Halosis";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Halosis.", ClassChoice.Dragon_of_Time),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<bool>("FarmHalosis", "Farm Halosis?", "Farm Halosis repeatedly.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private bool _dieNowDetected;
    private DateTimeOffset _dieNowDetectedAt;
    private bool _skillOneReserved;
    private bool _autoAttackCancelled;
    private bool _dieNowHandled;

    public void ScriptMain(IScriptInterface bot)
    {
        if (bot.Config != null)
            bot.Config.Configure();

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
        Core.RegisterQuests(10850);

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

        Bot.Wait.ForQuestComplete(10850);
        return true;
    }

    private bool FightBoss()
    {
        _dieNowDetected = false;
        _dieNowDetectedAt = DateTimeOffset.MinValue;
        _skillOneReserved = false;
        _autoAttackCancelled = false;
        _dieNowHandled = false;
        Bot.Flash.FlashCall -= HalosisFlashListener;
        Bot.Flash.FlashCall += HalosisFlashListener;

        try
        {
            Core.Join("fourharbingers-100000", "r2", "Bottom");
            StartNormalSkills();

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of Inner Conflict"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    if (Bot.Player.Alive)
                    {
                        _dieNowDetected = false;
                        _dieNowDetectedAt = DateTimeOffset.MinValue;
                        _skillOneReserved = false;
                        _autoAttackCancelled = false;
                        StartNormalSkills();
                        Bot.Skills.Resume();
                    }
                    continue;
                }

                if (!Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Join("fourharbingers-100000", "r2", "Bottom");
                    continue;
                }

                if (!Bot.Player.Cell.Equals("r2", StringComparison.OrdinalIgnoreCase))
                {
                    Core.Jump("r2", "Bottom");
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 1)
                    Bot.Combat.Attack(1);

                if (GetSelectedClass() == "Chaos Avenger")
                {
                    if (ChaosAvengerMechanics())
                        RefreshThirdPotion();
                }
                else
                    RefreshThirdPotion();

                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Flash.FlashCall -= HalosisFlashListener;
            Bot.Combat.CancelAutoAttack();
            Bot.Skills.Resume();
            _dieNowDetected = false;
            _dieNowDetectedAt = DateTimeOffset.MinValue;
            _skillOneReserved = false;
            _autoAttackCancelled = false;
            _dieNowHandled = false;
        }

        return Bot.TempInv.Contains("Signet of Inner Conflict");
    }

    private void StartNormalSkills()
    {
        if (GetSelectedClass() == "Dragon of Time")
            ReplaceSkillProvider("3 | 2 | 1 | 2 | 4 | 2", 250, SkillUseMode.WaitForCooldown);
        else
            ReplaceSkillProvider("3 | 4 | 2 | 1");
    }

    private void ReplaceSkillProvider(string skills, int skillTimeout = -1, SkillUseMode skillMode = SkillUseMode.UseIfAvailable)
    {
        Bot.Skills.LoadAdvanced(skills, skillTimeout, skillMode);

        if (Bot.Skills.OverrideProvider != null)
            Bot.Skills.SetProvider(Bot.Skills.OverrideProvider);

        if (!Bot.Skills.TimerRunning)
            Bot.Skills.Start();
    }

    private bool ChaosAvengerMechanics()
    {
        if (Bot.Self.HasActiveAura("Crits Inverted"))
        {
            Bot.Skills.Pause();
            _autoAttackCancelled = true;
            Bot.Combat.CancelAutoAttack();
            return false;
        }

        if (_dieNowDetected && !_dieNowHandled)
        {
            double elapsedMilliseconds = (DateTimeOffset.UtcNow - _dieNowDetectedAt).TotalMilliseconds;

            Bot.Skills.Pause();
            if (_autoAttackCancelled)
            {
                _autoAttackCancelled = false;
                Bot.Combat.Attack(1);
            }

            if (elapsedMilliseconds >= 2000 && Bot.Skills.CanUseSkill(1))
            {
                if (!Bot.Skills.UseSkill(1))
                    return false;

                _dieNowDetected = false;
                _dieNowDetectedAt = DateTimeOffset.MinValue;
                _skillOneReserved = false;
                _dieNowHandled = true;
                StartNormalSkills();
                Bot.Skills.Resume();
            }

            return false;
        }

        if (_autoAttackCancelled)
        {
            _autoAttackCancelled = false;
            Bot.Combat.Attack(1);
        }

        Bot.Skills.Resume();

        if (!_skillOneReserved && !_dieNowHandled && Bot.Player.Target != null)
        {
            if (Bot.Player.Target.MaxHP > 0
                && Bot.Player.Target.HP > 0
                && Bot.Player.Target.HP * 100 <= Bot.Player.Target.MaxHP * 40)
            {
                _skillOneReserved = true;
                ReplaceSkillProvider("3 | 4 | 2");
                Core.Logger("Halosis reached 40% HP. Skill 1 is now reserved for Die now.");
            }
        }

        return true;
    }

    private void HalosisFlashListener(string name, object[] args)
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

            string rawPacket = args[0] as string;
            if (string.IsNullOrWhiteSpace(rawPacket))
                return;

            dynamic packet = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(rawPacket);
            if (packet == null)
                return;

            dynamic body = packet["b"];
            if (body == null)
                return;

            dynamic data = body["o"];
            if (data == null)
                return;

            dynamic command = data["cmd"];
            if (command == null)
                return;
            if (command.ToString() != "ct")
                return;

            dynamic animations = data["anims"];
            if (animations == null)
                return;

            foreach (dynamic animation in animations)
            {
                dynamic messageValue = animation["msg"];
                if (messageValue == null)
                    continue;

                string message = messageValue.ToString();
                if (message == "Die now." || message == "Die now")
                {
                    if (_dieNowHandled || _dieNowDetected)
                        return;

                    _dieNowDetected = true;
                    _dieNowDetectedAt = DateTimeOffset.UtcNow;
                    Bot.Skills.Pause();
                    Core.Logger("Die now detected. Skill 1 will be used after 2 seconds.");
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
            "Celestial Winged Manticore",
            "Manticore of Light",
            "Scroll of the Quartet",
            "Scroll of the Wanderer",
            "Signet of Inner Conflict"
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

        return Bot.Config!.Get<bool>("FarmHalosis");
    }

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "Dragon of Time";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.Dragon_of_Time)
            return "Dragon of Time";
        else if (classChoice == ClassChoice.Chaos_Avenger)
            return "Chaos Avenger";
        else
            return "Dragon of Time";
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

        if (GetSelectedClass() == "Dragon of Time")
        {
            if (Adv.uElysium())
                weaponEnhancement = WeaponSpecial.Elysium;
            else if (Adv.uValiance())
            {
                Core.Logger("WARNING: Elysium is not unlocked. Valiance will be used instead.");
                weaponEnhancement = WeaponSpecial.Valiance;
            }
            else
            {
                Core.Logger("WARNING: Elysium and Valiance are not unlocked. Awe Blast will be used instead.");
                weaponEnhancement = WeaponSpecial.Awe_Blast;
            }

            if (Adv.uPneuma())
                helmEnhancement = HelmSpecial.Pneuma;
            else
                Core.Logger("WARNING: Pneuma is not unlocked. Wizard will be used on the helm instead.");

            if (Adv.uVainglory())
                capeEnhancement = CapeSpecial.Vainglory;
            else
                Core.Logger("WARNING: Vainglory is not unlocked. Wizard will be used on the cape instead.");

            if (weaponEnhancement == WeaponSpecial.Awe_Blast)
            {
                if (!Adv.uAwe())
                    Core.Logger("WARNING: Awe enhancements are not unlocked. Enhancement setup will continue.");
            }

            Adv.EnhanceEquipped(EnhancementType.Wizard, capeEnhancement, helmEnhancement, weaponEnhancement, true);
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
    }

    private void GetPotions()
    {
        if (GetSelectedClass() == "Dragon of Time")
        {
            GetPotion("Sage Tonic", "Gold Voucher 500k", 2, 500000, 10, 8, "", 0);
            GetPotion("Potent Malevolence Elixir", "Gold Voucher 500k", 4, 500000, 8, 0, "", 0);
            GetPotion("Potent Honor Potion", "Gold Voucher 500k", 4, 500000, 20, 0, "Good", 10);
        }
        else
        {
            GetPotion("Might Tonic", "Gold Voucher 500k", 2, 500000, 10, 8, "", 0);
            GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8, 0, "", 0);
            GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100, 0, "", 0);
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
            return;
        }

        if (GetSelectedClass() == "Dragon of Time")
        {
            UsePotion("Sage Tonic", "Sage");
            UsePotion("Potent Malevolence Elixir", "Potent Malevolence Elixir");
            UsePotion("Potent Honor Potion", "Potent Honor Malice");
        }
        else
        {
            UsePotion("Might Tonic", "Might");
            UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
            UsePotion("Felicitous Philtre", "Felicitous Philtre");
        }
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

        if (GetSelectedClass() == "Dragon of Time")
        {
            if (Bot.Inventory.GetQuantity("Sage Tonic") > 1
                && Bot.Inventory.GetQuantity("Potent Malevolence Elixir") > 1
                && Bot.Inventory.GetQuantity("Potent Honor Potion") > 1)
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
        if (GetSelectedClass() == "Dragon of Time")
        {
            if (Bot.Player.InCombat && !Bot.Self.HasActiveAura("Potent Honor Malice") && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        else
        {
            if (Bot.Player.InCombat && !Bot.Self.HasActiveAura("Felicitous Philtre") && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
    }

    private void WarnPotion(string itemName, string reason)
    {
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");
    }

    private enum ClassChoice
    {
        Dragon_of_Time,
        Chaos_Avenger,
    }
}
