/*
name: Halosis
description: null
tags: four harbingers, fourharbingers, halosis, boss, farm, signet of inner conflict, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Halosis
{
    public string OptionsStorage = "FourHarbingers_Halosis_v2";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Halosis.", ClassChoice.Dragon_of_Time),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<int>("FarmQuantity", "Farm Halosis Amount of Times", "0 farms indefinitely. Any positive number farms that many quest completions.", 0),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private CoreFourHarbingers FH = new CoreFourHarbingers();
    private bool _dieNowDetected;
    private DateTimeOffset _dieNowDetectedAt;
    private bool _skillOneReserved;
    private bool _autoAttackCancelled;

    public CoreFourHarbingers.FarmMode FarmModeOverride = CoreFourHarbingers.FarmMode.Once;
    public int FarmQuantityOverride = 1;
    public string FarmItemOverride = "";

    public void ScriptMain(IScriptInterface bot)
    {
        if (!DoAllMode)
            Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            FH.StopSkills();
            Core.CancelRegisteredQuests();
            if (!DoAllMode)
                Core.SetOptions(false);
        }
    }

    private void Run()
    {
        FH.AddDrops("Celestial Winged Manticore", "Manticore of Light", "Scroll of the Quartet", "Scroll of the Wanderer", "Signet of Inner Conflict");
        Core.RegisterQuests(10850);
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

        FH.RunQuests(10850, FightBoss, RestockPotions,
            FH.GetFarmMode(DoAllMode, FarmModeOverride),
            FH.GetFarmQuantity(DoAllMode, FarmQuantityOverride),
            FH.GetFarmItem(DoAllMode, FarmItemOverride));
    }

    private bool FightBoss()
    {
        _dieNowDetected = false;
        _dieNowDetectedAt = DateTimeOffset.MinValue;
        _skillOneReserved = false;
        _autoAttackCancelled = false;
        Bot.Flash.FlashCall -= HalosisFlashListener;
        Bot.Flash.FlashCall += HalosisFlashListener;

        try
        {
            Core.Join("fourharbingers-100000", "r2", "Bottom");
            Bot.Skills.Resume();

            if (GetSelectedClass() == "Dragon of Time")
                Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4 | 2", 250, SkillUseMode.WaitForCooldown);
            else
                Bot.Skills.StartAdvanced("3 | 4 | 2 | 1");

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
                        if (GetSelectedClass() == "Dragon of Time")
                            Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4 | 2", 250, SkillUseMode.WaitForCooldown);
                        else
                            Bot.Skills.StartAdvanced("3 | 4 | 2 | 1");
                        Bot.Skills.Resume();
                    }
                    continue;
                }

                if (!FH.EnsureBossRoom("r2", "Bottom"))
                    continue;

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 1)
                    Bot.Combat.Attack(1);

                if (GetSelectedClass() == "Chaos Avenger")
                {
                    if (ChaosAvengerMechanics())
                        FH.RefreshPotion("Felicitous Philtre");
                }
                else
                    FH.RefreshPotion("Potent Honor Malice");

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
        }

        return Bot.TempInv.Contains("Signet of Inner Conflict");
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

        if (_dieNowDetected)
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
                Bot.Skills.UseSkill(1);
                _dieNowDetected = false;
                _dieNowDetectedAt = DateTimeOffset.MinValue;
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

        if (!_skillOneReserved && Bot.Player.Target != null)
        {
            if (Bot.Player.Target.MaxHP > 0 && Bot.Player.Target.HP * 100 <= Bot.Player.Target.MaxHP * 40)
            {
                _skillOneReserved = true;
                FH.StopSkills();
                Bot.Skills.StartAdvanced("3 | 4 | 2");
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
                    _dieNowDetected = true;
                    _dieNowDetectedAt = DateTimeOffset.UtcNow;
                    Core.Logger("Die now detected. Skill 1 will be used after 2 seconds.");
                }
            }
        }
        catch
        {
        }
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
                FH.WarnEnhancementFallback("Elysium is not unlocked. Valiance will be used instead.");
                weaponEnhancement = WeaponSpecial.Valiance;
            }
            else
            {
                FH.WarnEnhancementFallback("Elysium and Valiance are not unlocked. Awe Blast will be used instead.");
                weaponEnhancement = WeaponSpecial.Awe_Blast;
            }

            if (Adv.uPneuma())
                helmEnhancement = HelmSpecial.Pneuma;
            else
                FH.WarnEnhancementFallback("Pneuma is not unlocked. Wizard will be used on the helm instead.");

            if (Adv.uVainglory())
                capeEnhancement = CapeSpecial.Vainglory;
            else
                FH.WarnEnhancementFallback("Vainglory is not unlocked. Wizard will be used on the cape instead.");

            if (weaponEnhancement == WeaponSpecial.Awe_Blast)
            {
                if (!Adv.uAwe())
                    FH.WarnEnhancementFallback("Awe enhancements are not unlocked. Enhancement setup will continue.");
            }

            Adv.EnhanceEquipped(EnhancementType.Wizard, capeEnhancement, helmEnhancement, weaponEnhancement, true);
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
    }

    private void GetPotions()
    {
        if (GetSelectedClass() == "Dragon of Time")
        {
            FH.GetPotion("Sage Tonic", "Gold Voucher 500k", 2, 500000, 10, 8);
            FH.GetPotion("Potent Malevolence Elixir", "Gold Voucher 500k", 4, 500000, 8);
            FH.GetPotion("Potent Honor Potion", "Gold Voucher 500k", 4, 500000, 20, 0, "Good", 10);
        }
        else
        {
            FH.GetPotion("Might Tonic", "Gold Voucher 500k", 2, 500000, 10, 8);
            FH.GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8);
            FH.GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100);
        }
    }

    private void UsePotions()
    {
        if (GetSelectedClass() == "Dragon of Time")
        {
            FH.UsePotion("Sage Tonic", "Sage");
            FH.UsePotion("Potent Malevolence Elixir", "Potent Malevolence Elixir");
            FH.UsePotion("Potent Honor Potion", "Potent Honor Malice");
        }
        else
        {
            FH.UsePotion("Might Tonic", "Might");
            FH.UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
            FH.UsePotion("Felicitous Philtre", "Felicitous Philtre");
        }
    }

    private void RestockPotions()
    {
        if (!FH.UsePotionsEnabled())
            return;

        if (GetSelectedClass() == "Dragon of Time")
            FH.RestockPotions(new[] { "Sage Tonic", "Potent Malevolence Elixir", "Potent Honor Potion" }, GetPotions, UsePotions);
        else
            FH.RestockPotions(new[] { "Might Tonic", "Potent Battle Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
    }

    private enum ClassChoice
    {
        Dragon_of_Time,
        Chaos_Avenger,
    }
}
