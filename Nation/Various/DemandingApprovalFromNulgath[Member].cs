/*
name: DemandingApprovalFromNulgath[Member]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs

using Skua.Core.Interfaces;

public class DemandingApprovalFromNulgath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Nation.GemStoneReceiptOfNulgath();

        Core.SetOptions(false);
    }
}
