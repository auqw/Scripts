/*
name: All Dailies
description: Does all the avaiable dailies.
tags: all dailies, dailies, daily, all
*/
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreDailies2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreStory2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs

//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/CoreNationv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/CoreSDKAv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/CoreBLODv3.cs

//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/BattleUnderv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/CitadelRuinsv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/DragonFableOriginsv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/Glacerav3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/Friendshipv3.cs

//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/LordOfOrderv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/MineCraftingv3.cs

//cs_include Scripts/Ultras-v3/Dependencies-Dailies/Dependencies-AllDaillies/BankAllItemsv3.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class FarmAllDailies
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 Core => CoreBots2.Instance;
    private static CoreDailies2 Daily
    {
        get => _Daily ??= new CoreDailies2();
        set => _Daily = value;
    }
    private static CoreDailies2 _Daily;
    private static LordOfOrder LOO
    {
        get => _LOO ??= new LordOfOrder();
        set => _LOO = value;
    }
    private static LordOfOrder _LOO;
    private static GlaceraStory Glac
    {
        get => _Glac ??= new GlaceraStory();
        set => _Glac = value;
    }
    private static GlaceraStory _Glac;
    private static CoreBLOD BLOD
    {
        get => _BLOD ??= new CoreBLOD();
        set => _BLOD = value;
    }
    private static CoreBLOD _BLOD;
    private static Friendship FR
    {
        get => _FR ??= new Friendship();
        set => _FR = value;
    }
    private static Friendship _FR;
    private static CoreSDKA CSDKA
    {
        get => _CSDKA ??= new CoreSDKA();
        set => _CSDKA = value;
    }
    private static CoreSDKA _CSDKA;
    private static MineCrafting MineCrafting
    {
        get => _MineCrafting ??= new MineCrafting();
        set => _MineCrafting = value;
    }
    private static MineCrafting _MineCrafting;

    //private BankAllItems BAI = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "FarmAllDailies";
    public List<IOption> Options = new()
    {
        new Option<DailySet>(
            "Select Dailies Set",
            "Dailies set: Recommended or All?",
            "only do the few that we recommend to make it a bit quicker?",
            DailySet.All
        ),
        CoreBots2.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoAllDailies(Bot.Config!.Get<DailySet>("Select Dailies Set"));

        Core.SetOptions(false);
    }

    public void RunAll()
    {
        DoAllDailies();
    }

    public void DoAllDailies(DailySet Set = DailySet.All)
    {
        Core.EquipClass(ClassType.Solo);

        if (Set == DailySet.Recommended)
        {
            Core.Logger($"Doing selected set of dailies: Recommended");

            // LoO
            if (!Core.CheckInventory(new[] { 50741, 50576 }, any: true, toInv: false))
                LOO.GetLoO();
            else
                Core.ToBank(50741, 50576);

            // Pyromancer
            if (!Core.CheckInventory(new[] { 12811, 12812 }, any: true, toInv: false))
                Daily.Pyromancer();
            else
                Core.ToBank(12811, 12812);

            Daily.ShadowScytheClass();
            Daily.WheelofDoom();
            Daily.FreeDailyBoost();
            Daily.CollectorClass();
            Glac.FrozenTower();

            // Cryomancer
            if (!Core.CheckInventory("Cryomancer", toInv: false))
                Daily.Cryomancer();
            else
                Core.ToBank("Cryomancer");

            Daily.EldersBlood();
            Daily.SparrowsBlood();
            Daily.ShadowShroud();
            Daily.DagesScrollFragment();
            MineCrafting.DoMinecrafting();
            Daily.CryptoToken();

            Core.Logger("Recommended Dailies finished!");
        }
        else
        {
            Core.Logger($"Doing selected set of dailies: All");

            // LoO
            if (!Core.CheckInventory(new[] { 50741, 50576 }, any: true, toInv: false))
                LOO.GetLoO();
            else
                Core.ToBank(50741, 50576);

            // Pyromancer
            if (!Core.CheckInventory(new[] { 12811, 12812 }, any: true, toInv: false))
                Daily.Pyromancer();
            else
                Core.ToBank(12811, 12812);

            Daily.ShadowScytheClass();
            Daily.WheelofDoom();
            Daily.FreeDailyBoost();
            Daily.CollectorClass();
            Glac.FrozenTower();

            // Cryomancer
            if (!Core.CheckInventory("Cryomancer", toInv: false))
                Daily.Cryomancer();
            else
                Core.ToBank("Cryomancer");

            Daily.EldersBlood();
            Daily.PearlOfNulgath();
            Daily.CryptoToken();
            Daily.ShadowShroud();
            MineCrafting.DoMinecrafting();
            Daily.SparrowsBlood();
            Daily.BeastMasterChallenge();
            Daily.FungiforaFunGuy();
            CSDKA.UnlockHardCoreMetals();

            Daily.HardCoreMetals(
                new[]
                {
                "Arsenic",
                "Beryllium",
                "Chromium",
                "Palladium",
                "Rhodium",
                "Thorium",
                "Mercury",
                },
                10,
                ToBank: true
            );

            Daily.GoldenInquisitor();
            Daily.BreakIntotheHoard(false, false);
            Daily.EldenRuby();
            Daily.NCSGem();
            Daily.EnchantedDarkBlood();
            Daily.MadWeaponSmith();
            Daily.CyserosSuperHammer();
            Daily.BrightKnightArmor();
            Daily.GrumbleGrumble();
            Daily.TenacityChallenge();
            Daily.MonthlyTreasureChestKeys();
            Daily.PowerGem();
            Daily.DesignNotes();
            Daily.MoglinPets();

            if (Set == DailySet.All)
            {
                FR.CompleteStory();
                Daily.Friendships();
            }

            Daily.DagesScrollFragment();

            Core.Logger("\"All\" Dailies finished!");
        }
    }
    public enum DailySet
    {
        Recommended,
        All,
        All_Without_Friendship,
    }
}
