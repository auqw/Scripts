/*
name: Bello
description: null
tags: four harbingers, fourharbingers, bello, boss, farm, signet of the endless journey, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Bello
{
    public string OptionsStorage = "FourHarbingers_Bello_v2";
    public bool DontPreconfigure = true;
    public bool DoAllMode;

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Use Class", "Class used to fight Bello.", ClassChoice.ArchPaladin),
        new Option<bool>("UsePotions", "Use Potions", "Use potions during the fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the class enhancements before fighting.", true),
        new Option<int>("FarmQuantity", "Farm Bello Amount of Times", "0 farms indefinitely. Any positive number farms that many quest completions.", 0),
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
        FH.AddDrops("Infernal Winged Manticore", "Manticore of Malice", "Scroll of the Preacher", "Scroll of the Quartet", "Signet of the Endless Journey");
        Core.RegisterQuests(10851);
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

        FH.RunQuests(10851, FightBoss, RestockPotions,
            FH.GetFarmMode(DoAllMode, FarmModeOverride),
            FH.GetFarmQuantity(DoAllMode, FarmQuantityOverride),
            FH.GetFarmItem(DoAllMode, FarmItemOverride));
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

                if (!FH.EnsureBossRoom("r3", "Bottom"))
                    continue;

                if (!Bot.Player.HasTarget || Bot.Player.Target == null || Bot.Player.Target.MapID != 2)
                    Bot.Combat.Attack(2);

                if (GetSelectedClass() == "ArchPaladin")
                    ArchPaladinMechanics();

                FH.RefreshPotion("Felicitous Philtre");
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

    private void ApplyEnhancements()
    {
        WeaponSpecial weaponEnhancement;
        HelmSpecial helmEnhancement = HelmSpecial.None;
        CapeSpecial capeEnhancement = CapeSpecial.None;

        if (Adv.uValiance())
            weaponEnhancement = WeaponSpecial.Valiance;
        else
        {
            FH.WarnEnhancementFallback("Valiance is not unlocked. Health Vamp will be used instead.");
            weaponEnhancement = WeaponSpecial.Health_Vamp;
        }

        if (Adv.uForgeHelm())
            helmEnhancement = HelmSpecial.Forge;
        else
            FH.WarnEnhancementFallback("Forge helm is not unlocked. Lucky will be used on the helm instead.");

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
        if (GetSelectedClass() == "ArchPaladin")
            FH.GetPotion("Fate Tonic", "Gold Voucher 500k", 4, 500000, 10, 8);
        else
            FH.GetPotion("Might Tonic", "Gold Voucher 500k", 2, 500000, 10, 8);

        FH.GetPotion("Potent Battle Elixir", "Gold Voucher 500k", 4, 500000, 8);
        FH.GetPotion("Felicitous Philtre", "Gold Voucher 100k", 8, 100000, 100);
    }

    private void UsePotions()
    {
        if (GetSelectedClass() == "ArchPaladin")
            FH.UsePotion("Fate Tonic", "Fate");
        else
            FH.UsePotion("Might Tonic", "Might");

        FH.UsePotion("Potent Battle Elixir", "Potent Battle Elixir");
        FH.UsePotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void RestockPotions()
    {
        if (!FH.UsePotionsEnabled())
            return;
        if (GetSelectedClass() == "ArchPaladin")
            FH.RestockPotions(new[] { "Fate Tonic", "Potent Battle Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
        else
            FH.RestockPotions(new[] { "Might Tonic", "Potent Battle Elixir", "Felicitous Philtre" }, GetPotions, UsePotions);
    }

    private enum ClassChoice
    {
        ArchPaladin,
        Chaos_Avenger,
    }
}
