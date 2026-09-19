/*
name: Anethyx'o's Absolution
description: Defeat Anethyx'o's Absolution using King's Echo or Chaos Avenger
tags: four harbingers, fourharbingers, anethyxos, anethyxos absolution, boss, farm, signet of the broken bond, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Options;

public class AnethyxosAbsolution
{
    public string OptionsStorage = "FourHarbingers_AnethyxosAbsolution_v2";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Anethyx'o's Absolution.", ClassChoice.Kings_Echo),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<int>("FarmQuantity", "Farm Anethyx'o's Absolution Amount of Times", "0 farms indefinitely. Any positive number farms that many quest completions.", 0),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private CoreFourHarbingers FH = new CoreFourHarbingers();
    private string? _resolvedClass;
    private bool _dieNow;
    private DateTimeOffset _dieNowDetectedAt;
    private bool _lowHealthSkills;
    private bool _dieNowHandled;

    public CoreFourHarbingers.FarmMode FarmModeOverride = CoreFourHarbingers.FarmMode.Once;
    public int FarmQuantityOverride = 1;
    public string FarmItemOverride = "";

    public void ScriptMain(IScriptInterface bot)
    {
        if (!DoAllMode)
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
            if (!DoAllMode)
                Core.SetOptions(false);
        }
    }

    private void Run()
    {
        _resolvedClass = null;
        FH.AddDrops("Anethyx’o’s Absolution Orbs", "Manticore of Infinity", "Scroll of the Heretic", "Scroll of the Quartet", "Signet of the Broken Bond");
        Core.RegisterQuests(10854);
        FH.ReturnToSafeRoom();

        string selectedClass = GetSelectedClass();
        string fallbackClass;
        if (selectedClass == "King's Echo")
            fallbackClass = "Chaos Avenger";
        else
            fallbackClass = "King's Echo";

        if (!FH.EquipClass(selectedClass, fallbackClass, !DoAllMode, out string resolvedClass))
            return;

        _resolvedClass = resolvedClass;

        if (FH.DoEnhancementsEnabled())
            ApplyEnhancements();

        if (FH.UsePotionsEnabled())
            GetPotions();

        Core.Join("fourharbingers-100000", "Enter", "Spawn");

        if (FH.UsePotionsEnabled())
            UsePotions();

        FH.RunQuests(10854, FightBoss, RestockPotions,
            FH.GetFarmMode(DoAllMode, FarmModeOverride),
            FH.GetFarmQuantity(DoAllMode, FarmQuantityOverride),
            FH.GetFarmItem(DoAllMode, FarmItemOverride));
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
                        _dieNowHandled = false;
                        StartNormalSkills();
                        Bot.Skills.Resume();
                    }
                    continue;
                }

                if (!FH.EnsureBossRoom("r6", "Bottom"))
                    continue;

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
                FH.RefreshPotion("Felicitous Philtre");
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
        if (boss == null || boss.MaxHP <= 0 || boss.HP <= 0 || boss.HP > boss.MaxHP * 0.25)
            return;

        _lowHealthSkills = true;
        if (GetSelectedClass() == "King's Echo")
            ReplaceSkillProvider("1 | 2", 250, SkillUseMode.WaitForCooldown);
        else
            ReplaceSkillProvider("3 | 4 | 2", 250, SkillUseMode.UseIfAvailable);
    }

    private bool StopForAuras()
    {
        if (!FH.HasMonsterAura(5, "Counter Attack") && !Bot.Self.HasActiveAura("Crits Inverted"))
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
            if (GetSelectedClass() == "King's Echo")
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
                return true;

            if (!Bot.Skills.UseSkill(1))
                return true;
        }

        _dieNow = false;
        _dieNowDetectedAt = DateTimeOffset.MinValue;
        _lowHealthSkills = false;
        _dieNowHandled = true;
        StartNormalSkills();
        Bot.Skills.Resume();
        return true;
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

            dynamic? packet = JsonConvert.DeserializeObject<dynamic>(rawPacket);
            dynamic? data = packet?["b"]?["o"];
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
                Core.Logger("Die now detected. Preparing the defensive skill.");
                return;
            }
        }
        catch
        {
        }
    }

    private string GetSelectedClass()
    {
        if (!string.IsNullOrEmpty(_resolvedClass))
            return _resolvedClass;

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
                FH.WarnEnhancementFallback("Elysium is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uExamen())
                helmEnhancement = HelmSpecial.Examen;
            else
                FH.WarnEnhancementFallback("Examen is not unlocked. Lucky will be used on the helm instead.");
        }
        else
        {
            if (Adv.uPraxis())
                weaponEnhancement = WeaponSpecial.Praxis;
            else
            {
                FH.WarnEnhancementFallback("Praxis is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uAnima())
                helmEnhancement = HelmSpecial.Anima;
            else
                FH.WarnEnhancementFallback("Anima is not unlocked. Lucky will be used on the helm instead.");
        }

        if (Adv.uPenitence())
            capeEnhancement = CapeSpecial.Penitence;
        else
            FH.WarnEnhancementFallback("Penitence is not unlocked. Lucky will be used on the cape instead.");

        if (weaponEnhancement == WeaponSpecial.Health_Vamp)
        {
            if (!Adv.uAwe())
                FH.WarnEnhancementFallback("Awe enhancements are not unlocked. Enhancement setup will continue.");
        }

        Adv.EnhanceEquipped(EnhancementType.Lucky, capeEnhancement, helmEnhancement, weaponEnhancement, true);
    }

    private void GetPotions()
    {
        FH.GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8);
        if (GetSelectedClass() == "King's Echo")
            FH.GetPotion("Potent Revitalize Elixir", "Gold Voucher 500k", 8, 500000, 20);
        else
            FH.GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 20);
        FH.GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100);
    }

    private void UsePotions()
    {
        FH.UsePotion("Fate Tonic", "Fate");
        if (GetSelectedClass() == "King's Echo")
            FH.UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
        else
            FH.UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
        FH.UsePotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void RestockPotions()
    {
        if (!FH.UsePotionsEnabled())
            return;

        if (GetSelectedClass() == "King's Echo")
            FH.RestockPotions(new[] { "Fate Tonic", "Potent Revitalize Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
        else
            FH.RestockPotions(new[] { "Fate Tonic", "Potent Battle Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
    }

    private enum ClassChoice
    {
        Kings_Echo,
        Chaos_Avenger,
    }
}
