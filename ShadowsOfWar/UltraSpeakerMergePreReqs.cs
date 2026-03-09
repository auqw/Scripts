/*
name: Ultra Speaker Merge PreReqs
description: Gets the prerequisites for the Ultra Speaker merge.
tags: ultra speaker merge, ultra malgor merge, rgow, goddess of war
*/
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Chaos/AscendedDrakathGear.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/BLoD/2UltimateBlindingLightofDestiny.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/HollowbornDoomKnight/CoreHollowbornDoomKnight.cs
//cs_include Scripts/Hollowborn/HollowbornPaladin/CoreHollowbornPaladin.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/FireAvatarFavorFarm.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/Artixpointe.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/BeetleQuests.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiegeMerge.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Chaos/ChampionDrakathMerge.cs

using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Options;

public class UltraSpeakerMergePreReqs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static AscendedDrakathGear ADG
    {
        get => _ADG ??= new AscendedDrakathGear();
        set => _ADG = value;
    }
    private static AscendedDrakathGear _ADG;
    private static CoreBLOD BLOD
    {
        get => _BLOD ??= new CoreBLOD();
        set => _BLOD = value;
    }
    private static CoreBLOD _BLOD;
    private static CoreHollowbornDoomKnight HDK
    {
        get => _HDK ??= new CoreHollowbornDoomKnight();
        set => _HDK = value;
    }
    private static CoreHollowbornDoomKnight _HDK;
    private static CoreSoC SoC
    {
        get => _SoC ??= new CoreSoC();
        set => _SoC = value;
    }
    private static CoreSoC _SoC;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;
    private static CoreSoWMats SOWM
    {
        get => _SOWM ??= new CoreSoWMats();
        set => _SOWM = value;
    }
    private static CoreSoWMats _SOWM;
    private static DragonFableOrigins DFO
    {
        get => _DFO ??= new DragonFableOrigins();
        set => _DFO = value;
    }
    private static DragonFableOrigins _DFO;
    private static DragonslayerGeneral DSG
    {
        get => _DSG ??= new DragonslayerGeneral();
        set => _DSG = value;
    }
    private static DragonslayerGeneral _DSG;
    private static FireAvatarFavorFarm FAFF
    {
        get => _FAFF ??= new FireAvatarFavorFarm();
        set => _FAFF = value;
    }
    private static FireAvatarFavorFarm _FAFF;
    private static UltimateBLoD UBLOD
    {
        get => _UBLOD ??= new UltimateBLoD();
        set => _UBLOD = value;
    }
    private static UltimateBLoD _UBLOD;
    private static WarfuryEmblem WFE
    {
        get => _WFE ??= new WarfuryEmblem();
        set => _WFE = value;
    }
    private static WarfuryEmblem _WFE;
    private static FireChampionsArmor FCA
    {
        get => _FCA ??= new FireChampionsArmor();
        set => _FCA = value;
    }
    private static FireChampionsArmor _FCA;
    private static BeetleQuests BeetleQuests
    {
        get => _BeetleQuests ??= new BeetleQuests();
        set => _BeetleQuests = value;
    }
    private static BeetleQuests _BeetleQuests;
    private static Awescended Awescended
    {
        get => _Awescended ??= new Awescended();
        set => _Awescended = value;
    }
    private static Awescended _Awescended;
    private static CoreHollowbornPaladin CHBP
    {
        get => _CHBP ??= new CoreHollowbornPaladin();
        set => _CHBP = value;
    }
    private static CoreHollowbornPaladin _CHBP;
    private static MalgorsArmorSet MalgorsArmorSet
    {
        get => _MalgorsArmorSet ??= new MalgorsArmorSet();
        set => _MalgorsArmorSet = value;
    }
    private static MalgorsArmorSet _MalgorsArmorSet;
    private static ChampionDrakathMerge ChampionDrakathMerge
    {
        get => _ChampionDrakathMerge ??= new ChampionDrakathMerge();
        set => _ChampionDrakathMerge = value;
    }
    private static ChampionDrakathMerge _ChampionDrakathMerge;
    private static DrakathArmorBot DAB
    {
        get => _DAB ??= new DrakathArmorBot();
        set => _DAB = value;
    }
    private static DrakathArmorBot _DAB;

    public string OptionsStorage = "Rgrow";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "UseInsigsonEmpDrkArm",
            "Use Insig on Emp Drak",
            "Wether to use your Champion Drakath Insignia to buy the \"Empowered Drakath Armor\"",
            false
        ),
        CoreBots.Instance.SkipOptions,
    };


    int AcquiescenceCount;
    int ElementalCoreCount;
    int InsigniasCount;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                //End Goal:
                "Radiant Goddess of War",
                // Non-Bottable
                "Malgor Insignia",
                "Elemental Core",
                "Fire Avatar's Favor ",
                "Acquiescence",
                //last step items:
                "Goddess Of War",
                //2nd to last step items
                "Goddess Of War Prestige Cloak",
                // 2nd step items:
                "Goddess Of War Blades",
                "Goddess of War Cloak",
                //begining steps:
                "War Blade of Strength",
                "War Blade of Courage",
                "War Blade of Power",
                "War Blade of Speed",
                "War Blade of Wisdom",

                //add more here
            }
        );

        Core.SetOptions();
        GetPrereqs();

        Core.SetOptions(false);
    }

    public void GetPrereqs()
    {

        SoW.CompleteCoreSoW();

        if (Core.CheckInventory("Radiant Goddess of War"))
            return;

        GoW();

        if (!Core.CheckInventory("Goddess Of War"))
            return;

        Core.AddDrop("Radiant Goddess of War");
        Core.EnsureAccept(9184);

        Farm.Experience();
        FCA.GetFireChampsArmor();
        BeetleQuests.WarlordRewards("Void Beetle Warlord");
        Awescended.GetAwe();
        CHBP.GetSpecific("Classic Hollowborn Paladin Armor");
        MalgorsArmorSet.GetSet(false, new[] { "Malgor the ShadowLord" });

        #region Radiant Goddess of War Item Check

        string[] requiredItems =
        {
            "Empowered Drakath Armor",
            "Fire Champion's Armor",
            "Void Beetle Warlord",
            "Malgor the ShadowLord",
            "Classic Hollowborn Paladin Armor",
            "Awescended"
        };

        if (Core.CheckInventory(requiredItems))
        {
            Core.EnsureComplete(9184);
        }
        else
        {
            foreach (string item in requiredItems)
                if (!Core.CheckInventory(item))
                    Core.Logger($"Missing {item}");

            Bot.Wait.ForPickup("Radiant Goddess of War");

            if (Core.CheckInventory("Radiant Goddess of War"))
                Core.Logger("Congrats!!!!");
            else
                Core.Logger("Still missing Goddess Of War prereqs.");
        }

        #endregion

        // Goddess Of War Prestige Cloak
        // Requires `Goddess Of War Blades` + `Goddess of War Cloak`
        Core.BuyItem("ultraspeaker", 2248, 72921, shopItemID: 11443);

        // ========================= LOCAL FUNCTIONS =========================

        void GoddessOfWarBlades()
        {
            if (Core.CheckInventory("Goddess Of War Blades"))
                return;

            SoC.LagunaBeach();

            foreach (string blade in new[]
            {
            "War Blade of Courage",
            "War Blade of Power",
            "War Blade of Speed",
            "War Blade of Strength",
            "War Blade of Wisdom"
        })
            {
                if (Core.CheckInventory(blade))
                    continue;

                switch (blade)
                {
                    case "War Blade of Courage":
                        BLOD.BrilliantAura(50);
                        BLOD.BlindingAura(1);
                        InsigniasCount += 7;
                        AcquiescenceCount += 10;
                        Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
                        break;

                    case "War Blade of Power":
                        Core.AddDrop(11475);
                        while (!Core.CheckInventory(11475, 30))
                            Core.KillMonster("lair", "Hole", "Center", "*", isTemp: false, log: false);

                        DSG.EnchantedScaleandClaw(250, 0);
                        InsigniasCount += 7;
                        AcquiescenceCount += 10;
                        Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
                        break;

                    case "War Blade of Speed":
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("shadowfallwar", "Skeletal Fire Mage", "Ultimate Darkness Gem", 75, false);

                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster("shadowattack", "Boss", "Left", "Death", "Death's Oversight", 5, false);

                        InsigniasCount += 7;
                        AcquiescenceCount += 10;
                        Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
                        break;

                    case "War Blade of Strength":
                        SoW.Tyndarius();

                        Core.AddDrop("Fire Avatar's Favor");
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(8244);

                        while (!Bot.ShouldExit && !Core.CheckInventory("Fire Avatar's Favor", 25))
                        {
                            Core.KillMonster("fireavatar", "r4", "Right", "*", "Onslaught Defeated", 6);
                            Core.KillMonster("fireavatar", "r6", "Left", "*", "Elemental Defeated", 6);
                            Bot.Wait.ForPickup("Fire Avatar's Favor");
                        }

                        Core.CancelRegisteredQuests();

                        InsigniasCount += 7;
                        AcquiescenceCount += 10;
                        ElementalCoreCount += 25;
                        Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
                        break;

                    case "War Blade of Wisdom":
                        Core.AddDrop("Fragment of the Queen", "ShadowChaos Mote");

                        Core.EquipClass(ClassType.Solo);
                        Bot.Quests.UpdateQuest(8094);
                        Core.HuntMonster("transformation", "Queen of Monsters", "Fragment of the Queen", 13, false);

                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(7700);
                        Core.HuntMonster("lagunabeach", "Flying Fisheye", "ShadowChaos Mote", 250, false);
                        Bot.Wait.ForPickup("ShadowChaos Mote");
                        Core.CancelRegisteredQuests();

                        InsigniasCount += 7;
                        AcquiescenceCount += 10;
                        Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
                        break;
                }
            }
        }

        void GoddessOfWarCloak()
        {
            if (Core.CheckInventory("Goddess of War Cloak"))
                return;

            AcquiescenceCount += 10;
            InsigniasCount += 10;
            Core.Logger($"New totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");
        }

        void GoW()
        {
            if (Core.CheckInventory("Goddess Of War"))
                return;

            if (Bot.Config!.Get<bool>("UseInsigsonEmpDrkArm")
                && !Core.CheckInventory("Empowered Drakath Armor")
                && Core.CheckInventory("Champion Drakath Insignia", 5)
                && Core.CheckInventory(25779))
            {
                DAB.DrakathArmorQuest();
                Core.Join("championdrakath");
                Bot.Wait.ForMapLoad("championdrakath");

                while (!Bot.ShouldExit && Bot.Shops.ID != 2055)
                {
                    Bot.Shops.Load(2055);
                    Bot.Wait.ForActionCooldown(GameActions.LoadShop);
                    Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == 2055, 20);
                    Core.Sleep(1000);
                }

                Bot.Shops.BuyItem("Empowered Drakath Armor");
                Bot.Wait.ForItemBuy();
            }

            UBLOD.PurifiedUndeadDragonEssence(3);

            if (!Core.CheckInventory(43712, 50))
            {
                Core.EquipClass(ClassType.Solo);
                Core.AddDrop(43712);
                Core.RegisterQuests(6311);

                while (!Bot.ShouldExit && !Core.CheckInventory(43712, 50))
                    Core.KillMonster("northmountain", "r7", "Left", "Izotz");

                Core.CancelRegisteredQuests();
            }

            SOWM.DragonsTear();
            ADG.AscendedGear("Ascended Blade of Awe");
            DFO.DragonFableOriginsAll();
            HDK.ADKFalls(true);

            GoddessOfWarCloak();
            GoddessOfWarBlades();
            Core.Logger($"Final totals: InsigniasCount: {InsigniasCount}, AcquiescenceCount: {AcquiescenceCount}, ElementalCoreCount: {ElementalCoreCount}", "Update Counts");


            SOWM.Acquiescence(AcquiescenceCount);
            SOWM.ElementalCore(ElementalCoreCount);

            // Reset counts
            AcquiescenceCount = 0;
            ElementalCoreCount = 0;

            if (Core.CheckInventory(new[] { "Goddess Of War Blades", "Goddess of War Cloak" }))
                Core.BuyItem("ultraspeaker", 2248, 72921, shopItemID: 11443);
        }
    }

}
