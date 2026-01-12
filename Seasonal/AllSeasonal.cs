/*
name: Complete All Seasonal Story
description: This will finish all of seasonal story on current month.
tags: seasonal, story, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/AprilFools/DERP!Badge.cs
//cs_include Scripts/Seasonal/AprilFools/MeateorHunt.cs
//cs_include Scripts/Seasonal/AprilFools/SuperSLAYIN'Badge(GardenQuest).cs
//cs_include Scripts/Seasonal/AprilFools/Mmmm,Meaty(or)(MeatyShard).cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Seasonal/HerosHeartDay/Fezzini.cs
//cs_include Scripts/Seasonal/HerosHeartDay/LoveSpellStory.cs
//cs_include Scripts/Seasonal/HerosHeartDay/WheelOfLove.cs
//cs_include Scripts/Seasonal/LuckyDay/Pooka.cs
//cs_include Scripts/Seasonal/MayThe4th/DarkLord.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonStory.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonMerge.cs
//cs_include Scripts/Story/MemetsRealm/CoreMemet.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
//cs_include Scripts/Seasonal/Mogloween/VampireLord(Class).cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/CoreDageBirthday.cs
//cs_include Scripts/Seasonal/StarFestival/StarFestival.cs
//cs_include Scripts/Seasonal/SummerBreak/BeachPartyTokenItems.cs
//cs_include Scripts/Seasonal/SummerBreak/BlazingBeach.cs
//cs_include Scripts/Seasonal/SummerBreak/BlazingBeachMerge.cs
//cs_include Scripts/Seasonal/SummerBreak/BurningBeach.cs
//cs_include Scripts/Seasonal/SummerBreak/CoralBeachMerge.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Seasonal/SummerBreak/LunaCoveMerge.cs
//cs_include Scripts/Seasonal/SummerBreak/SweetSummerTreats.cs
//cs_include Scripts/Seasonal/SummerBreak/Un-LifeguardQuest.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/CelestialPirateCommander[PollyRogers].cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/KaijuWar.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/HeartOfTheSeaStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/CetoleonWarStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DragonPirateStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DragonCapitalStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/AluteaNursery.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/BlazeBeardStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/LowTideStory.cs
//cs_include Scripts/Story/Legion/DageRecruit.cs
using Skua.Core.Interfaces;

public class AllSeasonal
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static DERPBadge Derp
    {
        get => _Derp ??= new DERPBadge();
        set => _Derp = value;
    }
    private static DERPBadge _Derp;
    private static MeateorHunt MeateorHunt
    {
        get => _MeateorHunt ??= new MeateorHunt();
        set => _MeateorHunt = value;
    }
    private static MeateorHunt _MeateorHunt;
    private static SuperSLAYINBadge SSB
    {
        get => _SSB ??= new SuperSLAYINBadge();
        set => _SSB = value;
    }
    private static SuperSLAYINBadge _SSB;
    private static CoreFrostvale Frostvale
    {
        get => _Frostvale ??= new CoreFrostvale();
        set => _Frostvale = value;
    }
    private static CoreFrostvale _Frostvale;
    private static FezziniStory Fezzini
    {
        get => _Fezzini ??= new FezziniStory();
        set => _Fezzini = value;
    }
    private static FezziniStory _Fezzini;
    private static LoveSpell LoveSpell
    {
        get => _LoveSpell ??= new LoveSpell();
        set => _LoveSpell = value;
    }
    private static LoveSpell _LoveSpell;
    private static WheeleOfLove WheeleOfLove
    {
        get => _WheeleOfLove ??= new WheeleOfLove();
        set => _WheeleOfLove = value;
    }
    private static WheeleOfLove _WheeleOfLove;
    private static PookaStory Pooka
    {
        get => _Pooka ??= new PookaStory();
        set => _Pooka = value;
    }
    private static PookaStory _Pooka;
    private static MmmmMeatyQuest Meaty
    {
        get => _Meaty ??= new MmmmMeatyQuest();
        set => _Meaty = value;
    }
    private static MmmmMeatyQuest _Meaty;
    private static DarkLord DarkLord
    {
        get => _DarkLord ??= new DarkLord();
        set => _DarkLord = value;
    }
    private static DarkLord _DarkLord;
    private static MurderMoon MurderMoon
    {
        get => _MurderMoon ??= new MurderMoon();
        set => _MurderMoon = value;
    }
    private static MurderMoon _MurderMoon;
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    private static VampireLord VPL
    {
        get => _VPL ??= new VampireLord();
        set => _VPL = value;
    }
    private static VampireLord _VPL;
    private static DageRecruitStory DageRecruit
    {
        get => _DageRecruit ??= new DageRecruitStory();
        set => _DageRecruit = value;
    }
    private static DageRecruitStory _DageRecruit;
    private static CoreDageBirthday Dage
    {
        get => _Dage ??= new CoreDageBirthday();
        set => _Dage = value;
    }
    private static CoreDageBirthday _Dage;
    private static StarFestival StarFestival
    {
        get => _StarFestival ??= new StarFestival();
        set => _StarFestival = value;
    }
    private static StarFestival _StarFestival;
    private static BeachPartyTokenItems BeachPartyTokenItems
    {
        get => _BeachPartyTokenItems ??= new BeachPartyTokenItems();
        set => _BeachPartyTokenItems = value;
    }
    private static BeachPartyTokenItems _BeachPartyTokenItems;
    private static BlazingBeachStory BlazingBeach
    {
        get => _BlazingBeach ??= new BlazingBeachStory();
        set => _BlazingBeach = value;
    }
    private static BlazingBeachStory _BlazingBeach;

    // public BlazingBeachMerge BlazingBeachMerge = new();
    private static BurningBeachStory BurningBeach
    {
        get => _BurningBeach ??= new BurningBeachStory();
        set => _BurningBeach = value;
    }
    private static BurningBeachStory _BurningBeach;

    // public CoralBeachMerge CoralBeachMerge = new();
    private static CoreSummer LunaCove
    {
        get => _LunaCove ??= new CoreSummer();
        set => _LunaCove = value;
    }
    private static CoreSummer _LunaCove;

    // public LunaCoveMerge LunaCoveMerge = new();
    private static SweetSummerTreats SweetSummerTreats
    {
        get => _SweetSummerTreats ??= new SweetSummerTreats();
        set => _SweetSummerTreats = value;
    }
    private static SweetSummerTreats _SweetSummerTreats;
    private static UnLifeGuardQuest UnLifeguardQuest
    {
        get => _UnLifeguardQuest ??= new UnLifeGuardQuest();
        set => _UnLifeguardQuest = value;
    }
    private static UnLifeGuardQuest _UnLifeguardQuest;
    private static CelestialPirateCommander CelestialPirateCommander
    {
        get => _CelestialPirateCommander ??= new CelestialPirateCommander();
        set => _CelestialPirateCommander = value;
    }
    private static CelestialPirateCommander _CelestialPirateCommander;
    private static KaijuWar KaijuWar
    {
        get => _KaijuWar ??= new KaijuWar();
        set => _KaijuWar = value;
    }
    private static KaijuWar _KaijuWar;
    private static HeartOfTheSeaStory HeartOfTheSeaStory
    {
        get => _HeartOfTheSeaStory ??= new HeartOfTheSeaStory();
        set => _HeartOfTheSeaStory = value;
    }
    private static HeartOfTheSeaStory _HeartOfTheSeaStory;
    private static CetoleonWarStory CetoleonWarStory
    {
        get => _CetoleonWarStory ??= new CetoleonWarStory();
        set => _CetoleonWarStory = value;
    }
    private static CetoleonWarStory _CetoleonWarStory;
    private static DragonPirateStory DragonPirateStory
    {
        get => _DragonPirateStory ??= new DragonPirateStory();
        set => _DragonPirateStory = value;
    }
    private static DragonPirateStory _DragonPirateStory;
    private static DragonCapitalStory DragonCapitalStory
    {
        get => _DragonCapitalStory ??= new DragonCapitalStory();
        set => _DragonCapitalStory = value;
    }
    private static DragonCapitalStory _DragonCapitalStory;
    private static LowTideStory LowTideStory
    {
        get => _LowTideStory ??= new LowTideStory();
        set => _LowTideStory = value;
    }
    private static LowTideStory _LowTideStory;
    private static AluteaNursery AluteaNursery
    {
        get => _AluteaNursery ??= new AluteaNursery();
        set => _AluteaNursery = value;
    }
    private static AluteaNursery _AluteaNursery;
    private static BlazeBeard BlazeBeard
    {
        get => _BlazeBeard ??= new BlazeBeard();
        set => _BlazeBeard = value;
    }
    private static BlazeBeard _BlazeBeard;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Seasonals();

        Core.SetOptions(false);
    }

    public void Seasonals()
    {
        switch (DateTime.Now.Month)
        {
            default:
                DageRecruit.CompleteDageRecruit();
                if (Bot.Quests.IsAvailable(7713))
                    CelestialPirateCommander.GetCPC(CelestialPirateCommander.CPCReward.All);
                break;

            case 1:
                Core.Logger("Starting Scripts for January");
                //insert script voids here
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 2:
                Core.Logger("Starting Scripts for Febuary");
                //insert script voids here
                Fezzini.FezziniScript();
                LoveSpell.LoveSpellScript();
                WheeleOfLove.DoWheeleOfLove();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 3:
                Core.Logger("Starting Scripts for March");
                //insert script voids here
                Pooka.CompletePooka();
                DarkLord.GetDL();
                MurderMoon.MurderMoonStory();
                Dage.DoAll();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");

                break;

            case 4:
                Core.Logger("Starting Scripts for April");
                //insert script voids here
                Derp.GetBadge();
                MeateorHunt.StoryLine();
                SSB.GetBadgeANDDoStory();
                Meaty.CompleteQuests();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 5:
                Core.Logger("Starting Scripts for May");
                //insert script voids here
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 6:
                Core.Logger("Starting Scripts for June");
                // BeachPartyTokenItems.TokenItems();
                BlazingBeach.StoryLine();
                // BlazingBeachMerge.BuyAllMerge();
                BurningBeach.Storyline();
                // CoralBeachMerge.BuyAllMerge();
                LunaCove.LunaCove();
                // LunaCoveMerge.BuyAllMerge();
                // SweetSummerTreats.GetTreats();
                // UnLifeguardQuest.GetItems();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 7:
                Core.Logger("Starting Scripts for July");
                Frostvale.DoAll();
                // BeachPartyTokenItems.TokenItems();
                BlazingBeach.StoryLine();
                // BlazingBeachMerge.BuyAllMerge();
                BurningBeach.Storyline();
                // CoralBeachMerge.BuyAllMerge();
                LunaCove.LunaCove();
                // LunaCoveMerge.BuyAllMerge();
                // SweetSummerTreats.GetTreats();
                // UnLifeguardQuest.GetItems();
                StarFestival.StoryLine();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 8:
                Core.Logger("Starting Scripts for August");
                //insert script voids here
                Frostvale.DoAll();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 9:
                Core.Logger("Starting Scripts for September");
                //insert script voids here
                CelestialPirateCommander.GetCPC(CelestialPirateCommander.CPCReward.All);
                KaijuWar.KaijuItems();
                HeartOfTheSeaStory.HeartOfTheSea();
                CetoleonWarStory.CetoleonWar();
                DragonPirateStory.DragonPirate();
                DragonCapitalStory.DragonCapital();
                LowTideStory.Storyline();
                AluteaNursery.DoAll();
                BlazeBeard.TokenQuests();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 10:
                Core.Logger("Starting Scripts for October");
                CoreMogloween.DoAll();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 11:
                Core.Logger("Starting Scripts for November");
                //insert script voids here
                VPL.GetClass(false);
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;

            case 12:
                Core.Logger("Starting Scripts for December");
                //insert script voids here
                Frostvale.DoAll();
                Core.Logger($"Scripts Finished for {DateTime.Now:MMMM}");
                break;
        }
    }
}
