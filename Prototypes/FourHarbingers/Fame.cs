/*
name: Fame
description: null
tags: four harbingers, fourharbingers, fame, boss, farm, signet of the filled chalice, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Fame
{
    public string OptionsStorage = "FourHarbingers_Fame_v2";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Fame.", ClassChoice.Yami_no_Ronin),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<int>("FarmQuantity", "Farm Fame Amount of Times", "0 farms indefinitely. Any positive number farms that many quest completions.", 0),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreAdvanced Adv = new CoreAdvanced();
    private CoreFourHarbingers FH = new CoreFourHarbingers();

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
        FH.AddDrops("Scroll of the Benevolent", "Scroll of the Quartet", "Signet of the Filled Chalice");
        Core.RegisterQuests(10852);
        FH.ReturnToSafeRoom();

        if (!ValidateClassRequirements())
            return;

        if (!FH.EquipClass(GetSelectedClass()))
            return;

        if (FH.DoEnhancementsEnabled())
            ApplyEnhancements();

        if (FH.UsePotionsEnabled())
            GetPotions();

        Core.Join("fourharbingers-100000", "Enter", "Spawn");

        if (FH.UsePotionsEnabled())
            UsePotions();

        FH.RunQuests(10852, FightBoss, RestockPotions,
            FH.GetFarmMode(DoAllMode, FarmModeOverride),
            FH.GetFarmQuantity(DoAllMode, FarmQuantityOverride),
            FH.GetFarmItem(DoAllMode, FarmItemOverride));
    }

    private bool FightBoss()
    {
        FH.StopSkills();

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

                if (!FH.EnsureBossRoom("r4", "Bottom"))
                    continue;

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 3)
                    Bot.Combat.Attack(3);

                UseSkills();
                FH.RefreshPotion("Felicitous Philtre");
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

    private string GetSelectedClass()
    {
        if (DoAllMode)
            return "Yami no Ronin";

        ClassChoice classChoice = Bot.Config!.Get<ClassChoice>("ClassChoice");
        if (classChoice == ClassChoice.Yami_no_Ronin)
            return "Yami no Ronin";
        else if (classChoice == ClassChoice.Verus_DoomKnight)
            return "Verus DoomKnight";
        else
            return "Yami no Ronin";
    }

    private bool ValidateClassRequirements()
    {
        if (GetSelectedClass() != "Verus DoomKnight")
            return true;

        if (Adv.uDauntless())
            return true;

        Core.Logger("WARNING: Verus DoomKnight requires Dauntless for this fight. The script will stop.", messageBox: true);
        return false;
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
                FH.WarnEnhancementFallback("Valiance is not unlocked. Health Vamp will be used instead.");
                weaponEnhancement = WeaponSpecial.Health_Vamp;
            }

            if (Adv.uVim())
                helmEnhancement = HelmSpecial.Vim;
            else
                FH.WarnEnhancementFallback("Vim is not unlocked. Lucky will be used on the helm instead.");
        }
        else
        {
            weaponEnhancement = WeaponSpecial.Dauntless;

            if (Adv.uAnima())
                helmEnhancement = HelmSpecial.Anima;
            else
                FH.WarnEnhancementFallback("Anima is not unlocked. Lucky will be used on the helm instead.");
        }

        if (Adv.uAvarice())
            capeEnhancement = CapeSpecial.Avarice;
        else
            FH.WarnEnhancementFallback("Avarice is not unlocked. Lucky will be used on the cape instead.");

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
        FH.GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8);
        FH.GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100);
    }

    private void UsePotions()
    {
        FH.UsePotion("Fate Tonic", "Fate");
        FH.UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
        FH.UsePotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void RestockPotions()
    {
        if (!FH.UsePotionsEnabled())
            return;
        FH.RestockPotions(
            new[] { "Fate Tonic", "Potent Battle Elixir", "Felicitous Philtre" },
            GetPotions,
            UsePotions
        );
    }

    private enum ClassChoice
    {
        Yami_no_Ronin,
        Verus_DoomKnight,
    }
}
