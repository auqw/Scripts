/*
name: 0EvolvedOrb
description: null
tags: null
*/
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/EvolvedOrb/EvolvedBloodOrb.cs
//cs_include Scripts/Nation/EvolvedOrb/EvolvedHexOrb.cs
//cs_include Scripts/Nation/EvolvedOrb/EvolvedShadowOrb[Mem].cs
//cs_include Scripts/Nation/EvolvedOrb/EvolvedShadowOrbItems[Mem].cs
//cs_include Scripts/Other/Classes/REP-based/Bard.cs
//cs_include Scripts/Other/MergeShops/BattleConGearMerge.cs
//cs_include Scripts/Other/Various/Potions.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class EvolvedOrb
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static EvolvedBloodOrb EBO
    {
        get => _EBO ??= new EvolvedBloodOrb();
        set => _EBO = value;
    }
    private static EvolvedBloodOrb _EBO;
    private static EvolvedHexOrb EHO
    {
        get => _EHO ??= new EvolvedHexOrb();
        set => _EHO = value;
    }
    private static EvolvedHexOrb _EHO;
    private static EvolvedShadowOrb ESO
    {
        get => _ESO ??= new EvolvedShadowOrb();
        set => _ESO = value;
    }
    private static EvolvedShadowOrb _ESO;
    private static EvolvedShadowOrbItems ESOItems
    {
        get => _ESOItems ??= new EvolvedShadowOrbItems();
        set => _ESOItems = value;
    }
    private static EvolvedShadowOrbItems _ESOItems;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoBoth();

        Core.SetOptions(false);
    }

    public void DoBoth()
    {
        GetAllOrb();
        GetAllItems();
    }

    public void GetAllOrb()
    {
        EBO.GetEvolvedBloodOrb();
        EHO.GetEvolvedHexOrb();
        ESO.GetEvolvedShadowOrb();
        Core.Logger($"Done, you have the balls");
    }

    public void GetAllItems()
    {
        ESOItems.GetItems();
        Core.Logger($"Done, you have hatched the balls");
    }
}
