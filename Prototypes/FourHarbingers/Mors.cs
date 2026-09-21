/*
name: Mors
description: null
tags: four harbingers, fourharbingers, mors, boss, farm, signet of the long quiet, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Mors
{
    public string OptionsStorage = "FourHarbingers_Mors_v2";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Mors.", ClassChoice.Legion_Revenant),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<int>("FarmQuantity", "Farm Mors Amount of Times", "0 farms indefinitely. Any positive number farms that many quest completions.", 0),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private CoreFourHarbingers FH = new CoreFourHarbingers();
    private bool _counterCastDetected;
    private DateTimeOffset _counterCastDetectedAt = DateTimeOffset.MinValue;
    private bool _counterStopLogged;
    private bool _counterAttackActive;

    public CoreFourHarbingers.FarmMode FarmModeOverride = CoreFourHarbingers.FarmMode.Once;
    public int FarmQuantityOverride = 1;
    public string FarmItemOverride = "";

    public void ScriptMain(IScriptInterface bot)
    {
        if (!DoAllMode)
            Core.SetOptions(disableClassSwap: true);
        Bot.UltraBossHelper.DisableCounterAttack();
        try
        {
            Run();
        }
        finally
        {
            FH.StopSkills();
            Bot.UltraBossHelper.EnableCounterAttack();
            Core.CancelRegisteredQuests();
            if (!DoAllMode)
                Core.SetOptions(false);
        }
    }

    private void Run()
    {
        if (GetSelectedClass() == "King's Echo")
            Core.Logger("WARNING: King's Echo is still in testing and will probably fail.", messageBox: true);

        FH.AddDrops("Manticore of Darkness", "Scroll of the Innocent", "Scroll of the Quartet", "Signet of the Long Quiet");
        Core.RegisterQuests(10853);
        FH.ReturnToSafeRoom();

        if (!FH.EquipClass(GetSelectedClass()))
            return;

        if (FH.DoEnhancementsEnabled())
            ApplyEnhancements();

        if (FH.UsePotionsEnabled())
            GetPotions();

        Core.Join("fourharbingers-100000", "Enter", "Spawn");

        if (FH.UsePotionsEnabled())
            UsePotions();

        FH.RunQuests(10853, FightBoss, RestockPotions,
            FH.GetFarmMode(DoAllMode, FarmModeOverride),
            FH.GetFarmQuantity(DoAllMode, FarmQuantityOverride),
            FH.GetFarmItem(DoAllMode, FarmItemOverride));
    }

    private bool FightBoss()
    {
        _counterCastDetected = false;
        _counterCastDetectedAt = DateTimeOffset.MinValue;
        _counterStopLogged = false;
        _counterAttackActive = false;
        FH.StopSkills();
        Bot.Flash.FlashCall -= MorsFlashListener;
        Bot.Flash.FlashCall += MorsFlashListener;

        try
        {
            Core.Join("fourharbingers-100000", "r5", "Bottom");
            if (GetSelectedClass() == "King's Echo")
                Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 3 | 1 | 2 | 1 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
            else
                Bot.Skills.StartAdvanced("3 | 4 | 2 | 1", 250, SkillUseMode.UseIfAvailable);

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("Signet of the Long Quiet"))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    if (Bot.Player.Alive)
                    {
                        _counterCastDetected = false;
                        _counterCastDetectedAt = DateTimeOffset.MinValue;
                        _counterStopLogged = false;
                        _counterAttackActive = false;
                        if (GetSelectedClass() == "King's Echo")
                            Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 3 | 1 | 2 | 1 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
                        else
                            Bot.Skills.StartAdvanced("3 | 4 | 2 | 1", 250, SkillUseMode.UseIfAvailable);
                        Bot.Skills.Resume();
                    }
                    continue;
                }

                if (!FH.EnsureBossRoom("r5", "Bottom"))
                    continue;

                if (!CounterAttackMechanics())
                {
                    Bot.Sleep(100);
                    continue;
                }

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 4)
                    Bot.Combat.Attack(4);

                if (GetSelectedClass() == "Legion Revenant")
                    FH.RefreshPotion("Potent Honor Potion", "Potent Honor Malice");
                else
                    FH.RefreshPotion("Felicitous Philtre", "Felicitous Philtre");

                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Flash.FlashCall -= MorsFlashListener;
            Bot.Combat.CancelAutoAttack();
            Bot.Skills.Resume();
            _counterCastDetected = false;
            _counterCastDetectedAt = DateTimeOffset.MinValue;
            _counterStopLogged = false;
            _counterAttackActive = false;
        }

        return Bot.TempInv.Contains("Signet of the Long Quiet");
    }

    private bool CounterAttackMechanics()
    {
        if (FH.HasMonsterAura(4, "Counter Attack"))
        {
            _counterCastDetected = false;
            _counterCastDetectedAt = DateTimeOffset.MinValue;
            _counterAttackActive = true;
            Bot.Skills.Pause();
            Bot.Combat.CancelAutoAttack();
            if (!_counterStopLogged)
            {
                _counterStopLogged = true;
                Core.Logger("Counter Cast: attacks stopped.");
            }
            if (GetSelectedClass() == "King's Echo")
                Bot.Combat.CancelTarget();
            return false;
        }

        if (_counterCastDetected)
        {
            double elapsedMilliseconds = (DateTimeOffset.UtcNow - _counterCastDetectedAt).TotalMilliseconds;
            if (elapsedMilliseconds < 1000)
                return true;

            if (elapsedMilliseconds < 4000)
            {
                Bot.Skills.Pause();
                Bot.Combat.CancelAutoAttack();
                if (!_counterStopLogged)
                {
                    _counterStopLogged = true;
                    Core.Logger("Counter Cast: attacks stopped.");
                }
                return false;
            }

            _counterCastDetected = false;
            _counterCastDetectedAt = DateTimeOffset.MinValue;
            _counterStopLogged = false;
        }

        if (_counterAttackActive)
        {
            _counterAttackActive = false;
            if (GetSelectedClass() == "King's Echo")
                Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 3 | 1 | 2 | 1 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
            Bot.Skills.Resume();
            Core.Logger("Counter Attack ended. Attacks resumed.");
        }

        return true;
    }

    private void MorsFlashListener(string name, object[] args)
    {
        try
        {
            if (Bot.ShouldExit
                || !Bot.Map.Name.Equals("fourharbingers", StringComparison.OrdinalIgnoreCase)
                || (!name.Equals("packetFromServer", StringComparison.OrdinalIgnoreCase)
                    && !name.Equals("pext", StringComparison.OrdinalIgnoreCase))
                || args.Length == 0)
                return;

            string rawPacket = args[0]?.ToString() ?? string.Empty;
            if (rawPacket.IndexOf("Mors prepares a counter attack", StringComparison.OrdinalIgnoreCase) < 0)
                return;
            if (_counterCastDetected || _counterAttackActive)
                return;

            _counterCastDetected = true;
            _counterCastDetectedAt = DateTimeOffset.UtcNow;
            _counterStopLogged = false;
            Core.Logger("Counter Cast detected. Attacks will stop in 1 second.");
        }
        catch
        {
        }
    }

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "Legion Revenant";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.Legion_Revenant)
            return "Legion Revenant";
        else if (classChoice.ToString().Equals("Kings_Echo", StringComparison.OrdinalIgnoreCase))
            return "King's Echo";
        else
            return "Legion Revenant";
    }

    private void ApplyEnhancements()
    {
        WeaponSpecial weaponEnhancement;
        HelmSpecial helmEnhancement = HelmSpecial.None;
        CapeSpecial capeEnhancement = CapeSpecial.None;

        if (Adv.uElysium())
            weaponEnhancement = WeaponSpecial.Elysium;
        else
        {
            FH.WarnEnhancementFallback("Elysium is not unlocked. Health Vamp will be used instead.");
            weaponEnhancement = WeaponSpecial.Health_Vamp;
        }

        if (GetSelectedClass() == "King's Echo")
        {
            if (Adv.uExamen())
                helmEnhancement = HelmSpecial.Examen;
            else
                FH.WarnEnhancementFallback("Examen is not unlocked. Wizard will be used on the helm instead.");
        }
        else
        {
            if (Adv.uPneuma())
                helmEnhancement = HelmSpecial.Pneuma;
            else
                FH.WarnEnhancementFallback("Pneuma is not unlocked. Wizard will be used on the helm instead.");
        }

        if (Adv.uAbsolution())
            capeEnhancement = CapeSpecial.Absolution;
        else
            FH.WarnEnhancementFallback("Absolution is not unlocked. Wizard will be used on the cape instead.");

        if (weaponEnhancement == WeaponSpecial.Health_Vamp)
        {
            if (!Adv.uAwe())
                FH.WarnEnhancementFallback("Awe enhancements are not unlocked. Enhancement setup will continue.");
        }

        if (GetSelectedClass() == "Legion Revenant")
            Adv.EnhanceEquipped(EnhancementType.Wizard, capeEnhancement, helmEnhancement, weaponEnhancement, true);
        else
            Adv.EnhanceEquipped(EnhancementType.Lucky, capeEnhancement, helmEnhancement, weaponEnhancement, true);
    }

    private void GetPotions()
    {
        if (GetSelectedClass() == "Legion Revenant")
            FH.GetPotion("Sage Tonic", "Gold Voucher 500k", 2, 500000, 10, 8);
        else
            FH.GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8);

        FH.GetPotion("Potent Revitalize Elixir", "Gold Voucher 500k", 8, 500000, 20);
        if (GetSelectedClass() == "Legion Revenant")
            FH.GetPotion("Potent Honor Potion", "Gold Voucher 500k", 4, 500000, 20, 0, "Good", 10);
        else
            FH.GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100);
    }

    private void UsePotions()
    {
        if (GetSelectedClass() == "Legion Revenant")
        {
            FH.UsePotion("Sage Tonic", "Sage");
            FH.UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
            FH.UsePotion("Potent Honor Potion", "Potent Honor Malice");
        }
        else
        {
            FH.UsePotion("Fate Tonic", "Fate");
            FH.UsePotion("Potent Revitalize Elixir", "Potent Revitalize Elixir");
            FH.UsePotion("Felicitous Philtre", "Felicitous Philtre");
        }
    }

    private void RestockPotions()
    {
        if (!FH.UsePotionsEnabled())
            return;

        if (GetSelectedClass() == "Legion Revenant")
            FH.RestockPotions(new[] { "Sage Tonic", "Potent Revitalize Elixir", "Potent Honor Potion" }, GetPotions, UsePotions);
        else
            FH.RestockPotions(new[] { "Fate Tonic", "Potent Revitalize Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
    }

    private enum ClassChoice
    {
        Legion_Revenant,
        // Kings_Echo,
    }
}
