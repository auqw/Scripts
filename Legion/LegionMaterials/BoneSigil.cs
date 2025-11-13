/*
name: MaxBoneSigil
description: farms BoneSigil till max quantity
tags: legion, dage, BoneSigil, 🖕, you
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs

using Skua.Core.Interfaces;

public class BoneSigil
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreLegion CL
    {
        get => _CL ??= new CoreLegion();
        set => _CL = value;
    }
    private static CoreLegion _CL;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBoneSigil();

        Core.SetOptions(false);
    }

    public void GetBoneSigil()
    {
        CL.BoneSigil();
    }
}
