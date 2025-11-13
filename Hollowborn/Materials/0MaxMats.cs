/*
name: Max Hollowborn Materials
description: Farms the maximum quantity of all Hollowborn materials.
tags: hollowborn, vindicator, farm, max, materials, hbv, hollow soul, deaths power, death's power, grace orb, gramiels emblem,gramiel's emblem, vindicator crest, vindicator badge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorBadge.cs
//cs_include Scripts/Hollowborn/Materials/DeathsPower.cs
//cs_include Scripts/Hollowborn/Materials/GraceOrb.cs
//cs_include Scripts/Hollowborn/Materials/GramielsEmblem.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorCrest.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs

using Skua.Core.Interfaces;

public class MaxHollowbornVindicatorMats
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static VindicatorBadge VB
    {
        get => _VB ??= new VindicatorBadge();
        set => _VB = value;
    }
    private static VindicatorBadge _VB;
    private static DeathsPower DP
    {
        get => _DP ??= new DeathsPower();
        set => _DP = value;
    }
    private static DeathsPower _DP;
    private static GraceOrb GO
    {
        get => _GO ??= new GraceOrb();
        set => _GO = value;
    }
    private static GraceOrb _GO;
    private static GramielsEmblem GE
    {
        get => _GE ??= new GramielsEmblem();
        set => _GE = value;
    }
    private static GramielsEmblem _GE;
    private static VindicatorCrest VC
    {
        get => _VC ??= new VindicatorCrest();
        set => _VC = value;
    }
    private static VindicatorCrest _VC;
    private static HollowSoul HS
    {
        get => _HS ??= new HollowSoul();
        set => _HS = value;
    }
    private static HollowSoul _HS;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetMax();

        Core.SetOptions(false);
    }

    public void GetMax()
    {
        HS.GetYaSoulsHeeeere();
        DP.GetDP();
        GO.GetGraceOrb();
        GE.GetGramielsEmblem();
        VC.GetVindicatorCrest();
        VB.GetVindicatorBadge();
    }
}
