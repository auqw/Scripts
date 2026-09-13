/*
name: Four Harbingers LW
description: Completes the available Four Harbingers quests with custom combat behavior or repeatedly farms one selected boss.
tags: story, quest, four harbingers, halosis, bello, fames, mors, lonewolf12
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using System.Reflection;

public class FourHarbingersLW
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv { get => _adv ??= new CoreAdvanced(); set => _adv = value; }
    private static CoreAdvanced _adv;

    private static CoreStory Story { get => _story ??= new CoreStory(); set => _story = value; }
    private static CoreStory _story;

    private bool _dieNow;
    private bool _critInverted;
    private bool _openingSkillUsed;

    public string OptionsStorage = "FourHarbingersLW";
    public bool DontPreconfigure = true;
    public string[] MultiOptions = { "Setup", "Farm" };

    public List<IOption> Setup = new()
    {
        new Option<ClassChoice>("ClassChoice", "Choose Class", "Optimized uses Dragon of Time, ArchPaladin, Yami no Ronin, Legion Revenant, and King's Echo.\nAdditional class strategies will be added later.", ClassChoice.Optimized),
        new Option<bool>("UsePotions", "Use Potions?", "Use the specified potion setup for each implemented boss.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements?", "Apply the specified enhancement setup for each implemented boss.", true),
    };

    public List<IOption> Farm = new()
    {
        new Option<bool>("FarmHalosis", "Farm Halosis?", "Repeatedly complete Intro Halosis Iter.", false),
        new Option<bool>("FarmBello", "Farm Bello?", "Repeatedly complete Intus Bello Pugna.", false),
        new Option<bool>("FarmFames", "Farm Fames?", "Repeatedly complete Intra Fames Abundantia.", false),
        new Option<bool>("FarmMors", "Farm Mors?", "Repeatedly complete Introsus Quietus Mors.", false),
        new Option<bool>("FarmAnethyxosAbsolution", "Farm Anethyx'o's Absolution?", "Repeatedly complete Penitus Absolution.", false),
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Bot.Config?.Configure();
        Core.SetOptions();
        Bot.UltraBossHelper.DisableCounterAttack();

        try
        {
            Run();
        }
        finally
        {
            Bot.Skills.Resume();
            Bot.UltraBossHelper.EnableCounterAttack();
            Bot.Skills.Stop();
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        string[] selectedFarms = new[]
        {
            "FarmHalosis", "FarmBello", "FarmFames", "FarmMors", "FarmAnethyxosAbsolution"
        }.Where(FarmEnabled).ToArray();

        if (selectedFarms.Length > 1)
        {
            Core.Logger("WARNING: Select only one Farm option at a time. Stopping the script.");
            return;
        }

        Story.PreLoad(this);

        if (selectedFarms.Length == 1)
        {
            RunFarm(selectedFarms[0]);
            return;
        }

        CompleteProgression();
    }

    private void CompleteProgression()
    {
        if (!Core.isCompletedBefore(10850) && !FightHalosis())
            return;

        if (!Core.isCompletedBefore(10851) && !FightBello())
            return;

        if (!Core.isCompletedBefore(10852) && !FightFames())
            return;

        if (!Core.isCompletedBefore(10853) && !FightMors())
            return;

        if (!Core.isCompletedBefore(10854) && !FightAnethyxosAbsolution())
            return;

        Core.Logger("Four Harbingers test path finished.");
    }

    private void RunFarm(string selectedFarm)
    {
        Boss boss = selectedFarm switch
        {
            "FarmHalosis" => Boss.Halosis,
            "FarmBello" => Boss.Bello,
            "FarmFames" => Boss.Fames,
            "FarmMors" => Boss.Mors,
            _ => Boss.AnethyxosAbsolution,
        };

        if (!EquipBossClass(boss))
            return;

        PrepareLoadout(boss);

        while (!Bot.ShouldExit)
        {
            bool succeeded = boss switch
            {
                Boss.Halosis => FightHalosis(prepareLoadout: false),
                Boss.Bello => FightBello(prepareLoadout: false),
                Boss.Fames => FightFames(prepareLoadout: false),
                Boss.Mors => FightMors(prepareLoadout: false),
                _ => FightAnethyxosAbsolution(prepareLoadout: false),
            };

            if (!succeeded)
                return;
        }
    }

    private bool FarmEnabled(string option) => Bot.Config?.Get<bool>("Farm", option) ?? false;

    private bool FightHalosis(bool prepareLoadout = true)
    {
        if (!EquipBossClass(Boss.Halosis))
            return false;

        if (prepareLoadout)
            PrepareLoadout(Boss.Halosis);
        else
            PreparePotions(Boss.Halosis);

        if (!Core.EnsureAccept(10850))
        {
            Core.Logger("WARNING: Intro Halosis Iter could not be accepted.");
            return false;
        }

        Core.Logger("Fighting Halosis.");

        if (!Bot.TempInv.Contains("Signet of Inner Conflict") &&
            !FightBoss(1, "r2", "Bottom", "3 | 2 | 1 | 2 | 4 | 2", true, MaintainHonorPotion,
                () => Bot.TempInv.Contains("Signet of Inner Conflict")))
            return false;

        if (!Core.EnsureComplete(10850))
        {
            Core.Logger("WARNING: Intro Halosis Iter could not be completed.");
            return false;
        }

        return true;
    }

    private bool FightBello(bool prepareLoadout = true)
    {
        if (!EquipBossClass(Boss.Bello))
            return false;

        if (prepareLoadout)
            PrepareLoadout(Boss.Bello);
        else
            PreparePotions(Boss.Bello);

        if (!Core.EnsureAccept(10851))
        {
            Core.Logger("WARNING: Intus Bello Pugna could not be accepted.");
            return false;
        }

        Core.Logger("Fighting Bello.");

        if (!Bot.TempInv.Contains("Signet of the Endless Journey") &&
            !FightBoss(2, "r3", "Bottom", "3 | 2 | 1", false, BelloMechanics,
                () => Bot.TempInv.Contains("Signet of the Endless Journey")))
            return false;

        if (!Core.EnsureComplete(10851))
        {
            Core.Logger("WARNING: Intus Bello Pugna could not be completed.");
            return false;
        }

        return true;
    }

    private bool FightFames(bool prepareLoadout = true)
    {
        if (!EquipBossClass(Boss.Fames))
            return false;

        if (prepareLoadout)
            PrepareLoadout(Boss.Fames);
        else
            PreparePotions(Boss.Fames);

        if (!Core.EnsureAccept(10852))
        {
            Core.Logger("WARNING: Intra Fames Abundantia could not be accepted.");
            return false;
        }

        Core.Logger("Fighting Fames.");

        if (!Bot.TempInv.Contains("Signet of the Filled Chalice") &&
            !FightBoss(3, "r4", "Bottom", "", false, FamesMechanics,
                () => Bot.TempInv.Contains("Signet of the Filled Chalice")))
            return false;

        if (!Core.EnsureComplete(10852))
        {
            Core.Logger("WARNING: Intra Fames Abundantia could not be completed.");
            return false;
        }

        return true;
    }

    private bool FightMors(bool prepareLoadout = true)
    {
        if (!EquipBossClass(Boss.Mors))
            return false;

        if (prepareLoadout)
            PrepareLoadout(Boss.Mors);
        else
            PreparePotions(Boss.Mors);

        if (!Core.EnsureAccept(10853))
        {
            Core.Logger("WARNING: Introsus Quietus Mors could not be accepted.");
            return false;
        }

        Core.Logger("Fighting Mors.");

        if (!Bot.TempInv.Contains("Signet of the Long Quiet") &&
            !FightBoss(4, "r5", "Bottom", "3 | 2 | 1 | 4", false, MorsMechanics,
                () => Bot.TempInv.Contains("Signet of the Long Quiet")))
            return false;

        if (!Core.EnsureComplete(10853))
        {
            Core.Logger("WARNING: Introsus Quietus Mors could not be completed.");
            return false;
        }

        return true;
    }

    private bool FightAnethyxosAbsolution(bool prepareLoadout = true)
    {
        if (!EquipBossClass(Boss.AnethyxosAbsolution))
            return false;

        if (prepareLoadout)
            PrepareLoadout(Boss.AnethyxosAbsolution);
        else
            PreparePotions(Boss.AnethyxosAbsolution);

        if (!Core.EnsureAccept(10854))
        {
            Core.Logger("WARNING: Penitus Absolution could not be accepted.");
            return false;
        }

        Core.Logger("Fighting Anethyx'o's Absolution.");
        Core.Logger("ts dont work currently :skull: \n rework still in progress, u gotta manual this boss for now :D \n(it will still attempt to fight tho, but it will prolly fail)", messageBox: true);

        if (!Bot.TempInv.Contains("Signet of the Broken Bond") &&
            !FightAbsolution(() => Bot.TempInv.Contains("Signet of the Broken Bond")))
            return false;

        if (!Core.EnsureComplete(10854))
        {
            Core.Logger("WARNING: Penitus Absolution could not be completed.");
            return false;
        }

        return true;
    }

    // Retries Resume()/Stop() a few times, since a Resume() from a Paused state
    // needs a tick to actually register before Stop() can take effect.
    private bool StopSkillTimer(int attempts = 3, int waitTicks = 100)
    {
        for (int i = 0; i < attempts; i++)
        {
            Bot.Skills.Resume();
            Bot.Skills.Stop();

            if (Bot.Wait.ForTrue(() => !Bot.Skills.TimerRunning, waitTicks))
                return true;
        }

        return false;
    }

    private bool FightBoss(int mapID, string cell, string pad, string combo, bool waitForCooldown,
        Action? mechanics, Func<bool> stopCondition)
    {
        const string map = "fourharbingers";

        try
        {
            Core.Join(map, cell, pad);

            // MorsMechanics can leave skills Paused mid-attack on a target that no
            // longer exists once the fight ends. Cancel any stale auto-attack first
            // so Resume()/Stop() below isn't blocked trying to act on it.
            Bot.Combat.CancelAutoAttack();

            if (!StopSkillTimer())
            {
                Core.Logger("WARNING: The previous skill timer did not stop. The fight cannot start safely.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(combo))
            {
                if (waitForCooldown)
                    Bot.Skills.StartAdvanced(combo, 250, SkillUseMode.WaitForCooldown);
                else
                    Bot.Skills.StartAdvanced(combo);
            }

            while (!Bot.ShouldExit && !stopCondition())
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
                    Core.Join(map, cell, pad);

                if (!string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
                    Core.Jump(cell, pad);

                if (!Bot.Player.HasTarget || Bot.Player.Target?.MapID != mapID)
                    Bot.Combat.Attack(mapID);

                if (Bot.Player.HasTarget && Bot.Player.Target?.MapID == mapID)
                    mechanics?.Invoke();

                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Combat.CancelAutoAttack();
            StopSkillTimer();
        }

        return stopCondition();
    }

    private void BelloMechanics()
    {
        MaintainFelicitousPhiltre();

        var righteousSeal = Bot.Target.GetAura("Righteous Seal");
        if (righteousSeal != null && righteousSeal.RemainingTime <= 1 && Bot.Skills.CanUseSkill(4))
            Bot.Skills.UseSkill(4);
    }

    private void FamesMechanics()
    {
        MaintainFelicitousPhiltre();

        if (Bot.Skills.CanUseSkill(3))
            Bot.Skills.UseSkill(3);
        else if (Bot.Skills.CanUseSkill(2))
            Bot.Skills.UseSkill(2);
        else if (Bot.Skills.CanUseSkill(1))
            Bot.Skills.UseSkill(1);
    }

    private void MorsMechanics()
    {
        bool counterAttack = Bot.Target.GetAura("Counter Attack") != null;

        if (!counterAttack)
        {
            Bot.Skills.Resume();
            MaintainHonorPotion();
            return;
        }

        Bot.Skills.Pause();
        Bot.Combat.CancelAutoAttack();
        if (Bot.Skills.CanUseSkill(3))
            Bot.Skills.UseSkill(3);
    }

    private bool FightAbsolution(Func<bool> stopCondition)
    {
        _dieNow = false;
        _critInverted = false;
        _openingSkillUsed = false;
        Bot.Flash.FlashCall -= AbsolutionFlashListener;
        Bot.Flash.FlashCall += AbsolutionFlashListener;

        try
        {
            return FightBoss(5, "r6", "Bottom", "", false, AbsolutionMechanics, stopCondition);
        }
        finally
        {
            Bot.Flash.FlashCall -= AbsolutionFlashListener;
            _dieNow = false;
            _critInverted = false;
            _openingSkillUsed = false;
        }
    }

    private void AbsolutionMechanics()
    {
        if (!_openingSkillUsed)
        {
            if (Bot.Skills.CanUseSkill(3))
            {
                Bot.Skills.UseSkill(3);
                _openingSkillUsed = true;
                Bot.Skills.StartAdvanced("1 | 2");
            }
            return;
        }

        if (_dieNow || _critInverted || Bot.Self.HasActiveAura("Crits Inverted"))
        {
            AbsolutionHalosisHandler();
            return;
        }

        if (Bot.Target.GetAura("Counter Attack") != null || Bot.Target.GetAura("Execute") != null)
        {
            AbsolutionMorsHandler();
            return;
        }

        if (Bot.Target.GetAura("Declaration") != null)
        {
            AbsolutionBelloHandler();
            return;
        }

        if (Bot.Self.HasActiveAura("Starvation") || Bot.Self.HasActiveAura("Decay"))
        {
            AbsolutionFamesHandler();
            return;
        }

        AbsolutionNormalHandler();
    }

    private void AbsolutionNormalHandler()
    {
        Bot.Skills.Resume();
        MaintainFelicitousPhiltre();

        if (Bot.Player.Mana < 24 && Bot.Skills.CanUseSkill(4))
        {
            Bot.Skills.Pause();
            Bot.Skills.UseSkill(4);
            Bot.Skills.Resume();
        }
        else if (Bot.Player.MaxHealth > 0
            && Bot.Player.Health < Bot.Player.MaxHealth * 0.5
            && Bot.Player.MaxMana > 0
            && Bot.Player.Mana > Bot.Player.MaxMana * 0.5
            && Bot.Skills.CanUseSkill(3))
        {
            Bot.Skills.Pause();
            Bot.Skills.UseSkill(3);
            Bot.Skills.Resume();
        }
    }

    private void AbsolutionHalosisHandler()
    {
        if (_critInverted || Bot.Self.HasActiveAura("Crits Inverted"))
        {
            Bot.Skills.Pause();
            Bot.Combat.CancelAutoAttack();
            return;
        }

        if (_dieNow)
        {
            Bot.Skills.Pause();

            if (Bot.Self.HasActiveAura("Waiting For Corvak"))
            {
                if (Bot.Skills.CanUseSkill(4))
                    Bot.Skills.UseSkill(4);
                return;
            }

            if (Bot.Skills.CanUseSkill(3))
            {
                Bot.Skills.UseSkill(3);
                _dieNow = false;
                Bot.Skills.Resume();
            }
            return;
        }

        Bot.Skills.Resume();
        MaintainFelicitousPhiltre();

        if (Bot.Player.Mana < 24 && Bot.Skills.CanUseSkill(4))
        {
            Bot.Skills.Pause();
            Bot.Skills.UseSkill(4);
            Bot.Skills.Resume();
        }
    }

    private void AbsolutionBelloHandler() => AbsolutionNormalHandler();

    private void AbsolutionFamesHandler() => AbsolutionNormalHandler();

    private void AbsolutionMorsHandler()
    {
        if (Bot.Target.GetAura("Counter Attack") != null)
        {
            Bot.Skills.Pause();
            Bot.Combat.CancelAutoAttack();
            return;
        }

        AbsolutionNormalHandler();
    }

    private void MaintainHonorPotion()
    {
        MaintainPotion("Potent Honor Potion", "Potent Honor Malice");
    }

    private void MaintainFelicitousPhiltre()
    {
        MaintainPotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void MaintainPotion(string itemName, string auraName)
    {
        if (!(Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true)
            || !Bot.Player.Alive
            || !Bot.Inventory.IsEquipped(itemName)
            || !Bot.Skills.CanUseSkill(5))
            return;

        var aura = Bot.Self.GetAura(auraName);
        if (aura != null && aura.ExpiresAt - DateTimeOffset.Now > TimeSpan.FromSeconds(5))
            return;

        Bot.Skills.UseSkill(5);
    }

    private void AbsolutionFlashListener(string name, object[] args)
    {
        try
        {
            if (Bot.ShouldExit
                || !string.Equals(Bot.Map.Name, "fourharbingers", StringComparison.OrdinalIgnoreCase)
                || !name.Equals("packetFromServer", StringComparison.OrdinalIgnoreCase)
                || args.Length == 0
                || args[0] is not string rawPacket)
                return;

            dynamic packet = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(rawPacket);
            dynamic data = packet?["b"]?["o"];
            if (data?.cmd?.ToString() != "ct")
                return;

            if (data?.anims is not null)
            {
                foreach (dynamic anim in data.anims)
                {
                    string? message = anim?.msg?.ToString();
                    if (message == "Die now.")
                    {
                        _dieNow = true;
                        Core.Logger("Die now detected. Preparing King's Shield.");
                    }
                    else if (message == "Heal me.")
                    {
                        _critInverted = true;
                        Core.Logger("Heal me detected. Holding attacks for Crits Inverted.");
                    }
                }
            }

            if (data?.a is not null)
            {
                foreach (dynamic action in data.a)
                {
                    if (action?.cmd?.ToString() == "aura-"
                        && action?.aura?.nam?.ToString() == "Crits Inverted")
                        _critInverted = false;
                }
            }
        }
        catch
        {
        }
    }

    private bool EquipBossClass(Boss boss)
    {
        string className = boss switch
        {
            Boss.Halosis => "Dragon of Time",
            Boss.Fames => "Yami no Ronin",
            Boss.Mors => "Legion Revenant",
            Boss.AnethyxosAbsolution => "King's Echo",
            _ => "ArchPaladin",
        };

        if (!Core.CheckInventory(className))
        {
            Core.Logger($"WARNING: {className} was not found.");
            return false;
        }

        Core.Equip(className);
        Bot.Wait.ForItemEquip(className);

        if (string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase))
            return true;

        Core.Logger($"WARNING: {className} could not be equipped.");
        return false;
    }

    private void PrepareLoadout(Boss boss)
    {
        if (Bot.Config?.Get<bool>("Setup", "DoEnhancements") ?? true)
        {
            Bot.Sleep(3000);
            ApplyEnhancements(boss);
        }

        PreparePotions(boss);
    }

    private void PreparePotions(Boss boss)
    {
        if (!(Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true))
            return;

        if (boss == Boss.Halosis)
        {
            PreparePotion("Sage Tonic", "Gold Voucher 500k", 2, 500_000, 10, requiredAlchemyRank: 8);
            PreparePotion("Potent Malevolence Elixir", "Gold Voucher 500k", 4, 500_000, 8);
            PreparePotion("Potent Honor Potion", "Gold Voucher 500k", 1, 500_000, 5, requiredFaction: "Good", requiredFactionRank: 10);
        }
        else if (boss == Boss.Mors)
        {
            PreparePotion("Sage Tonic", "Gold Voucher 500k", 2, 500_000, 10, requiredAlchemyRank: 8);
            PreparePotion("Potent Revitalize Elixir", "Gold Voucher 500k", 8, 500_000, 20);
            PreparePotion("Potent Honor Potion", "Gold Voucher 500k", 1, 500_000, 5, requiredFaction: "Good", requiredFactionRank: 10);
        }
        else if (boss == Boss.AnethyxosAbsolution)
        {
            PreparePotion("Fate Tonic", "Gold Voucher 500k", 4, 500_000, 10, requiredAlchemyRank: 8);
            PreparePotion("Potent Revitalize Elixir", "Gold Voucher 500k", 8, 500_000, 20);
            PreparePotion("Felicitous Philtre", "Gold Voucher 100k", 2, 100_000, 25);
        }
        else
        {
            PreparePotion("Might Tonic", "Gold Voucher 500k", 2, 500_000, 10, requiredAlchemyRank: 8);
            PreparePotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500_000, 8);
            PreparePotion("Felicitous Philtre", "Gold Voucher 100k", 2, 100_000, 25);
        }

        ApplyPotions(boss, true);
    }

    private void ApplyPotions(Boss boss, bool? usePotions = null)
    {
        if (!(usePotions ?? (Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true)))
            return;

        if (boss == Boss.Halosis)
        {
            UsePotion("Sage Tonic", "Sage");
            UsePotion("Potent Malevolence Elixir", "Potent Malevolence Elixir");
            UsePotion("Potent Honor Potion", "Potent Honor Malice");
        }
        else if (boss == Boss.Mors)
        {
            UsePotion("Sage Tonic", "Sage");
            UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
            UsePotion("Potent Honor Potion", "Potent Honor Malice");
        }
        else if (boss == Boss.AnethyxosAbsolution)
        {
            UsePotion("Fate Tonic", "Fate");
            UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
            UsePotion("Felicitous Philtre", "Felicitous Philtre");
        }
        else
        {
            UsePotion("Might Tonic", "Might");
            UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
            UsePotion("Felicitous Philtre", "Felicitous Philtre");
        }
    }

    private void ApplyEnhancements(Boss boss)
    {
        try
        {
            if (Core.CBOBool("DisableAutoEnhance", out bool disabled) && disabled)
                Core.Logger("AutoEnhance is disabled in CoreBots Options. The custom Four Harbingers setup will override it.");

            EnhancementType baseEnhancement = boss is Boss.Halosis or Boss.Mors ? EnhancementType.Wizard : EnhancementType.Lucky;
            HelmSpecial helmEnhancement = HelmSpecial.None;
            CapeSpecial capeEnhancement = CapeSpecial.None;
            WeaponSpecial weaponEnhancement;

            if (boss == Boss.Halosis)
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
                    Core.Logger("WARNING: Elysium and Valiance are not unlocked. Wizard Awe Blast will be used instead.");
                    weaponEnhancement = WeaponSpecial.Awe_Blast;
                }

                if (Adv.uVainglory())
                    capeEnhancement = CapeSpecial.Vainglory;
                else
                    Core.Logger("WARNING: Vainglory is not unlocked. Wizard will be used on the cape instead.");
            }
            else if (boss == Boss.Mors)
            {
                if (Adv.uElysium())
                    weaponEnhancement = WeaponSpecial.Elysium;
                else
                {
                    Core.Logger("WARNING: Elysium is not unlocked. Wizard Health Vamp will be used instead.");
                    weaponEnhancement = WeaponSpecial.Health_Vamp;
                }

                if (Adv.uPneuma())
                    helmEnhancement = HelmSpecial.Pneuma;
                else
                    Core.Logger("WARNING: Pneuma is not unlocked. Wizard will be used on the helm instead.");

                if (Adv.uAbsolution())
                    capeEnhancement = CapeSpecial.Absolution;
                else
                    Core.Logger("WARNING: Absolution is not unlocked. Wizard will be used on the cape instead.");
            }
            else if (boss == Boss.AnethyxosAbsolution)
            {
                if (Adv.uElysium())
                    weaponEnhancement = WeaponSpecial.Elysium;
                else
                {
                    Core.Logger("WARNING: Elysium is not unlocked. Lucky Health Vamp will be used instead.");
                    weaponEnhancement = WeaponSpecial.Health_Vamp;
                }

                if (Adv.uForgeHelm())
                    helmEnhancement = HelmSpecial.Forge;
                else
                    Core.Logger("WARNING: Forge helm is not unlocked. Lucky will be used on the helm instead.");

                if (Adv.uAbsolution())
                    capeEnhancement = CapeSpecial.Absolution;
                else
                    Core.Logger("WARNING: Absolution is not unlocked. Lucky will be used on the cape instead.");
            }
            else
            {
                if (Adv.uValiance())
                    weaponEnhancement = WeaponSpecial.Valiance;
                else
                {
                    Core.Logger("WARNING: Valiance is not unlocked. Lucky Health Vamp will be used instead.");
                    weaponEnhancement = WeaponSpecial.Health_Vamp;
                }

                if (boss == Boss.Fames)
                {
                    if (Adv.uVim())
                        helmEnhancement = HelmSpecial.Vim;
                    else
                        Core.Logger("WARNING: Vim is not unlocked. Lucky will be used on the helm instead.");
                }
                else if (Adv.uForgeHelm())
                    helmEnhancement = HelmSpecial.Forge;
                else
                    Core.Logger("WARNING: Forge helm is not unlocked. Lucky will be used on the helm instead.");

                if (Adv.uPenitence())
                    capeEnhancement = CapeSpecial.Penitence;
                else
                    Core.Logger("WARNING: Penitence is not unlocked. Lucky will be used on the cape instead.");
            }

            if (weaponEnhancement is WeaponSpecial.Awe_Blast or WeaponSpecial.Health_Vamp && !Adv.uAwe())
                Core.Logger("WARNING: The selected Awe enhancement is not unlocked. Enhancement setup will continue without stopping the script.");

            MethodInfo? autoEnhance = typeof(CoreAdvanced).GetMethod("AutoEnhance", BindingFlags.Instance | BindingFlags.NonPublic);
            if (autoEnhance == null)
                throw new MissingMethodException(nameof(CoreAdvanced), "AutoEnhance");

            List<InventoryItem> equippedItems = Bot.Inventory.Items.FindAll(item => item.Equipped
                && (item.Category is ItemCategory.Class or ItemCategory.Helm or ItemCategory.Cape || item.ItemGroup == "Weapon"));

            autoEnhance.Invoke(Adv, new object[]
            {
                equippedItems, baseEnhancement, capeEnhancement, helmEnhancement, weaponEnhancement, false
            });
        }
        catch (Exception ex)
        {
            Bot.Log($"Four Harbingers enhancement setup failed: {ex}");
            Core.Logger("WARNING: The enhancement setup failed. Continuing with the current enhancements.");
        }
    }

    private void PreparePotion(string itemName, string voucherName, int voucherQuantity, int voucherCost,
        int purchaseQuantity, int requiredAlchemyRank = 0, string? requiredFaction = null, int requiredFactionRank = 0)
    {
        try
        {
            if (Bot.Inventory.Contains(itemName))
                return;

            if (Bot.Bank.Contains(itemName))
            {
                if (Bot.Inventory.FreeSlots <= 0)
                {
                    WarnPotion(itemName, "no free inventory slot is available");
                    return;
                }

                Bot.Bank.EnsureToInventory(itemName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);
                if (!Bot.Inventory.Contains(itemName))
                    WarnPotion(itemName, "it could not be moved from the bank");
                return;
            }

            if (requiredAlchemyRank > 0 && !Bot.Reputation.HasRank("Alchemy", requiredAlchemyRank))
            {
                WarnPotion(itemName, $"Alchemy rank {requiredAlchemyRank} is required");
                return;
            }

            if (requiredFaction != null && requiredFactionRank > 0 && !Bot.Reputation.HasRank(requiredFaction, requiredFactionRank))
            {
                WarnPotion(itemName, $"{requiredFaction} rank {requiredFactionRank} is required");
                return;
            }

            int requiredSlots = Bot.Inventory.Contains(voucherName) ? 1 : 2;
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

            int missingVouchers = Math.Max(0, voucherQuantity - Bot.Inventory.GetQuantity(voucherName));
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

            Core.BuyItem("alchemyacademy", 2036, itemName, purchaseQuantity);
            Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);
            if (!Bot.Inventory.Contains(itemName))
                WarnPotion(itemName, "it could not be purchased");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion preparation failed for {itemName}: {ex}");
            WarnPotion(itemName, "preparation failed");
        }
    }

    private void UsePotion(string itemName, string auraName)
    {
        try
        {
            if (Bot.Self.HasActiveAura(auraName))
                return;

            if (!Bot.Inventory.Contains(itemName))
            {
                WarnPotion(itemName, "it is not in the inventory");
                return;
            }

            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Wait.ForItemEquip(itemName);
            if (!Bot.Inventory.IsEquipped(itemName))
            {
                WarnPotion(itemName, "it could not be equipped");
                return;
            }

            Bot.Sleep(500);

            bool applied = false;
            for (int attempt = 0; attempt < 3 && !Bot.ShouldExit; attempt++)
            {
                int quantityBefore = Bot.Inventory.GetQuantity(itemName);
                Core.UsePotion();

                long started = Environment.TickCount64;
                while (!Bot.ShouldExit && Environment.TickCount64 - started < 1500)
                {
                    if (Bot.Self.HasActiveAura(auraName) || Bot.Inventory.GetQuantity(itemName) < quantityBefore)
                    {
                        applied = true;
                        break;
                    }

                    Bot.Sleep(50);
                }

                if (applied)
                    break;

                Bot.Sleep(250);
            }

            if (!applied)
                WarnPotion(itemName, "its effect could not be verified");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion use failed for {itemName}: {ex}");
            WarnPotion(itemName, "use failed");
        }
    }

    private void WarnPotion(string itemName, string reason) =>
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");

    public void FarmHalosis(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(Boss.Halosis, item, quantity, isTemp, 1, "r2", "Bottom", "3 | 2 | 1 | 2 | 4 | 2", true, MaintainHonorPotion);

    public void FarmBello(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(Boss.Bello, item, quantity, isTemp, 2, "r3", "Bottom", "3 | 2 | 1", false, BelloMechanics);

    public void FarmFames(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(Boss.Fames, item, quantity, isTemp, 3, "r4", "Bottom", "", false, FamesMechanics);

    public void FarmMors(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(Boss.Mors, item, quantity, isTemp, 4, "r5", "Bottom", "3 | 2 | 1 | 4", false, MorsMechanics);

    public void FarmAnethyxosAbsolution(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(Boss.AnethyxosAbsolution, item, quantity, isTemp, 5, "r6", "Bottom", "", false, AbsolutionMechanics);

    private void FarmBossItem(Boss boss, string item, int quantity, bool isTemp, int mapID, string cell,
        string pad, string combo, bool waitForCooldown, Action? mechanics)
    {
        if (string.IsNullOrWhiteSpace(item) || quantity <= 0 || HasFarmItem(item, quantity, isTemp))
            return;

        Adv.GearStore(EnhAfter: true);

        try
        {
            if (!isTemp)
                Core.AddDrop(item);

            Core.FarmingLogger(item, quantity);

            if (!EquipBossClass(boss))
            {
                Core.Logger($"WARNING: The optimized class is unavailable. Falling back to the standard farm method for {BossName(boss)}.");
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("fourharbingers", BossName(boss), item, quantity, isTemp: isTemp);
                return;
            }

            PrepareLoadout(boss, usePotions: false);
            if (boss == Boss.AnethyxosAbsolution)
                FightAbsolution(() => HasFarmItem(item, quantity, isTemp));
            else
                FightBoss(mapID, cell, pad, combo, waitForCooldown, mechanics,
                    () => HasFarmItem(item, quantity, isTemp));
        }
        finally
        {
            Adv.GearStore(true, true);
        }
    }

    private void PrepareLoadout(Boss boss, bool? usePotions)
    {
        if (Bot.Config?.Get<bool>("Setup", "DoEnhancements") ?? true)
        {
            Bot.Sleep(3000);
            ApplyEnhancements(boss);
        }

        if (usePotions ?? (Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true))
            ApplyPotions(boss, true);
    }

    private bool HasFarmItem(string item, int quantity, bool isTemp) =>
        isTemp ? Bot.TempInv.Contains(item, quantity) : Core.CheckInventory(item, quantity);

    private string BossName(Boss boss) => boss switch
    {
        Boss.Halosis => "Halosis",
        Boss.Bello => "Bello",
        Boss.Fames => "Fames",
        Boss.Mors => "Mors",
        _ => "Anethyx’o’s Absolution",
    };

    private enum Boss
    {
        Halosis,
        Bello,
        Fames,
        Mors,
        AnethyxosAbsolution,
    }

    private enum ClassChoice
    {
        Optimized,
    }
}
