/*
name: Necrotic Sword of Doom Daily
description: This bot will do the Encroaching Shadows quest plus Glimpse Into the Dark if you're member.
tags: daily, NSOD, necrotic, sword, doom, encroaching, shadows, void, aura
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class NSODDaily
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Daily.NSoDDaily();

        Core.SetOptions(false);
    }
}
