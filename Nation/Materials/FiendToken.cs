/*
name: FiendToken
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/HanzoOrbQuest.cs
//cs_include Scripts/Nation/VoidKnightSwordQuest.cs
using Skua.Core.Interfaces;

public class FiendToken
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static HanzoOrbQuest HanzoOrbQuest
    {
        get => _HanzoOrbQuest ??= new HanzoOrbQuest();
        set => _HanzoOrbQuest = value;
    }
    private static HanzoOrbQuest _HanzoOrbQuest;
    private static VoidKnightSword VoidKnightSword
    {
        get => _VoidKnightSword ??= new VoidKnightSword();
        set => _VoidKnightSword = value;
    }
    private static VoidKnightSword _VoidKnightSword;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FarmFiendToken();

        Core.SetOptions(false);
    }

    public void FarmFiendToken()
    {
        if (!Core.IsMember)
            HanzoOrbQuest.HanzoOrb("FiendToken, 30");
        Nation.FarmFiendToken();
    }
}
