/*
name: Scion of Flames (Class)
description: This script will get Scion of Flames class.
tags: ScionofFlames, Scion of Flames
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/MagnumOpusMerge.cs
//cs_include Scripts/Other/MergeShops/CarcossaCanteenMerge.cs
//cs_include Scripts/Other/MergeShops/ForgeMalenoMerge.cs
//cs_include Scripts/Other/MergeShops/FortLumaForgeMerge.cs
//cs_include Scripts/Other/MergeShops/WarwickForestMerge.cs
//cs_include Scripts/Other/MergeShops/RemnantsofachampionMerge.cs
using Skua.Core.Interfaces;

public class ScionofFlames
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;
    private static MagnumOpusMerge MOM
    {
        get => _MOM ??= new MagnumOpusMerge();
        set => _MOM = value;
    }
    private static MagnumOpusMerge _MOM;
    private static RemnantsofachampionMerge RoaCM
    {
        get => _RoaCM ??= new RemnantsofachampionMerge();
        set => _RoaCM = value;
    }
    private static RemnantsofachampionMerge _RoaCM;

    private static WarwickForestMerge WFM
    {
        get => _WFM ??= new WarwickForestMerge();
        set => _WFM = value;
    }
    private static WarwickForestMerge _WFM;
    private static FortLumaForgeMerge FLF
    {
        get => _FLF ??= new FortLumaForgeMerge();
        set => _FLF = value;
    }
    private static FortLumaForgeMerge _FLF;
    private static CarcossaCanteenMerge CCM
    {
        get => _CCM ??= new CarcossaCanteenMerge();
        set => _CCM = value;
    }
    private static CarcossaCanteenMerge _CCM;
    private static ForgeMalenoMerge FMM
    {
        get => _FMM ??= new ForgeMalenoMerge();
        set => _FMM = value;
    }
    private static ForgeMalenoMerge _FMM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetScionofFlames();

        Core.SetOptions(false);
    }

    public void GetScionofFlames(bool rankUpClass = true)
    {
        const string ClassName = "Scion of Flames";

        Core.Logger($"[{ClassName}] Start sequence");
        if (Core.CheckInventory(ClassName))
        {
            Core.Logger($"[Scion of Flames] Already owned");

            if (rankUpClass)
                Adv.RankUpClass(ClassName);

            return;
        }
        Core.AddDrop(ClassName);
        Core.EnsureAccept(10716);
        // P1: Story progression
        Core.Logger("[P1] Story progression (AOR)");
        AOR.DoAll();

        // P2: Leveling
        Core.Logger("[P2] Leveling to 80");
        Farm.Experience(80);

        // P3: Reputation
        Core.Logger("[P3] YewMountains REP to Rank 10");
        Farm.YewMountainsREP();

        // P4: Unlock sigil
        Core.Logger("[P4] Buying Flame Sigil");
        Adv.BuyItem("fireforge", 1142, "Flame Sigil");

        // P5: Aleister the Dawnseeker
        Core.Logger("[P5] mering Aleister the Dawnseeker Via MagnumOpusMerge");
        MOM.BuyAllMerge("Aleister the Dawnseeker");
        WFM.BuyAllMerge("Flame of Rubedo");
        FLF.BuyAllMerge("Flame of Citrinitas");
        CCM.BuyAllMerge("Flame of Albedo");
        FMM.BuyAllMerge("Flame of Maleno");

        Core.EnsureComplete(10716);

        Bot.Wait.ForDrop(ClassName);
        Bot.Wait.ForPickup(ClassName);

        if (rankUpClass)
            Adv.RankUpClass(ClassName);


    }

    void Materials(string item, int quant)
    {
        if (Core.CheckInventory(item, quant))
        {
            Core.Logger($"[MATS] Skipping {item} (already have {quant})");
            return;
        }

        Core.FarmingLogger(item, quant);
        Core.AddDrop(item);
        switch (item)
        {
            case "Red Flame of Rubedo":
                Core.EquipClass(ClassType.Solo);
                Core.RegisterQuests(Core.IsMember ? 10689 : 10688);

                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.KillMonster("warwickforest", "r10", "Bottom", "*", "Rubedo Flicker");
                    Core.HuntMonster("warwickforest", "Idea of a Champion", "Kolr's Needle");
                    Core.HuntMonster("warwickforest", "Rubedo Match", "Alkahest", 100);
                    Bot.Wait.ForPickup(item);
                }

                Core.CancelRegisteredQuests();
                break;

            case "Yew Ember":
                Core.FarmingLogger(item, quant);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("warwickforest", "Rubedo Elemental", item, quant, false);
                break;

            case "Yellow Flame of Citrinitas":
                Core.RegisterQuests(Core.IsMember ? 10620 : 10619);

                Core.EquipClass(ClassType.Solo);
                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.HuntMonster("fortluma", "Flame of Citrinitas", "Citrinitas Flicker");
                    Core.HuntMonster("fortluma", "Luma Dragon Twins", "Draconic Contrasoul");
                    Core.HuntMonsterMapID("fortluma", 10, "King's Yellow");
                    Bot.Wait.ForPickup(item);
                }

                Core.CancelRegisteredQuests();
                break;

            case "White Flame of Albedo":
                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.HuntMonsterQuest(
                        !Core.IsMember ? 10423 : 10424,
                        ("forgealbedo", "Defensive Turret", ClassType.Farm),
                        ("forgealbedo", "Defense Droid", ClassType.Farm),
                        ("forgealbedo", "Flame of Albedo", ClassType.Solo)
                    );

                    Bot.Wait.ForPickup(item);
                }
                break;

            case "Black Flame of Maleno":
                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.HuntMonsterQuest(
                        Core.IsMember ? 10370 : 10369,
                        ("mountmaleno", "Draconian Bandit", ClassType.Farm),
                        ("mountmaleno", "Maleno Elemental", ClassType.Solo),
                        ("mountmaleno", "Idalion", ClassType.Solo)
                    );

                    Bot.Wait.ForPickup(item);
                }
                break;

            case "Steel Ingot":
                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.HuntMonsterQuest(
                        Core.IsMember ? 10358 : 10357,
                        ("thelimacity", "Maleno Elemental", ClassType.Solo),
                        ("thelimacity", "Maleno Match", ClassType.Farm),
                        ("thelimacity", "Flame of Maleno", ClassType.Solo)
                    );

                    Bot.Wait.ForPickup(item);
                }
                break;

            case "Aiwass Diamond":
                Core.EquipClass(ClassType.Farm);
                Core.RegisterQuests(Core.IsMember ? 10387 : 10385);

                while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
                {
                    Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Sal Alembroth", 1, false);
                    Core.KillMonster("sanctuaryaiwass", "r9", "Top", "*", "Milk of Sulfur", 1, false);
                    Core.HuntMonster("sanctuaryaiwass", "Anima Animus Aiwass", "Aeon Dream", 1, false);
                }
                Core.CancelRegisteredQuests();
                break;
        }
    }

}
