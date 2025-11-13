/*
name: Pyromancer Daily
description: Pyromancer
tags: daily, insertthinghere, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class Pyromancer
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

        Daily.Pyromancer();

        Core.SetOptions(false);
    }
}
