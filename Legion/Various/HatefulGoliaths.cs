/*
name: Image of Dage Quest Rewards
description: This script will complete "Image of Dage" quest
tags: underworld,legion,dage,birthday,image of dage,quest,hateful,hateful goliath,hateful goliaths,
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise3.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise4.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Legion/MergeShops/UndeadLegionMerge.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/FortressDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
using Skua.Core.Interfaces;

public class HatefulGoliaths
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static UndeadLegionMerge ULM
    {
        get => _ULM ??= new UndeadLegionMerge();
        set => _ULM = value;
    }
    private static UndeadLegionMerge _ULM;
    private static CoreIsleOfFotia IOF
    {
        get => _IOF ??= new CoreIsleOfFotia();
        set => _IOF = value;
    }
    private static CoreIsleOfFotia _IOF;
    private static HeadoftheLegionBeast HOTLB
    {
        get => _HOTLB ??= new HeadoftheLegionBeast();
        set => _HOTLB = value;
    }
    private static HeadoftheLegionBeast _HOTLB;
    private static FortressDelve FD
    {
        get => _FD ??= new FortressDelve();
        set => _FD = value;
    }
    private static FortressDelve _FD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetHatefulGoliaths();

        Core.SetOptions(false);
    }

    public void GetHatefulGoliaths()
    {
        if (Core.CheckInventory(Core.QuestRewards(10103)))
            return;

        Core.AddDrop(Core.EnsureLoad(10103).AcceptRequirements.Select(item => item.Name).ToArray());
        Core.AddDrop(Core.QuestRewards(10103));

        Core.Logger("Farming Prereqs...");

        // Ultimate Lich King
        ULM.BuyAllMerge("Ultimate Lich King");

        // Palace Map
        IOF.DageFortress(true);

        // Indulgence
        HOTLB.Indulgence(1);

        // Rune of Radiance
        FD.DoStory();
        Core.EnsureAccept(9170);
        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster(
            "fortressdelve",
            "Enlightened Shadow",
            "Shadowscythe Bone Shard",
            10,
            log: false
        );
        Core.HuntMonster(
            "fortressdelve",
            "Delirious Elemental",
            "Elemental Residue",
            10,
            log: false
        );
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("fortressdelve", "Astero", "Glass Wing", log: false);
        Core.EnsureComplete(9170);

        Core.HuntMonsterQuest(
            10103,
            ("shadowblast", "Thanatos", ClassType.Solo),
            ("evilwarnul", "Laken", ClassType.Solo),
            ("envy", "Envy", ClassType.Solo),
            ("darkfortress", "Wilhelm", ClassType.Solo),
            ("darkally", "Underfiend", ClassType.Solo)
        );
    }
}
