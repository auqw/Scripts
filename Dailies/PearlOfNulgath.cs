/*
name: Pearl of Nulgath Daily
description: Gets the pearl of nulgath from the daily quest.
tags: daily, pearl of nulgath,pearl,malakai,contract enforcer
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class PearlOfNulgath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Daily.PearlOfNulgath();

        Core.SetOptions(false);
    }
}
