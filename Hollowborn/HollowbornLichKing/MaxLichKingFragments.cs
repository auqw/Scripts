/*
name: Hollowborn Lich King - Lich King Fragment
description: maxes out the "Lich King Fragment" item
tags: hollowborn lich king, hollowborn, Lich King Fragment
*/
//cs_include Scripts/Hollowborn/HollowbornLichKing/CoreHollowbornLichKing.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs

//cs_include Scripts/Legion/MergeShops/UndeadLegionMerge.cs
//cs_include Scripts/Legion/LegionMaterials/SoulSand.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Legion/LegionMaterials/LetitBurn(SoulEssence).cs

//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise3.cs
//cs_include Scripts/Legion/LegionExcercise/LegionExercise4.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Legion/Various/LegionBonfire.cs

using Skua.Core.Interfaces;

public class LichKingFragment
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreHollowbornLichKing sHBLK
    {
        get => _sHBLK ??= new CoreHollowbornLichKing();
        set => _sHBLK = value;
    }
    private static CoreHollowbornLichKing _sHBLK;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.OneTimeMessage("Warning", "this script will **NOT** stop till you manaully stop it.");
        sHBLK.FlowStress(CoreHollowbornLichKing.FlowStressRewards.Lich_King_Fragment, false, 10001);

        Core.SetOptions(false);
    }
}
