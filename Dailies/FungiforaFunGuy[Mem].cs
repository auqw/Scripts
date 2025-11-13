/*
name: Fungi for a Fun Guy Daily
description: Fungi for a Fun Guy
tags: daily, fungi for a fun guy, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class FungiforaFunGuy
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

        Daily.FungiforaFunGuy();

        Core.SetOptions(false);
    }
}
