/*
name: Dawn Vindicators' Sanctum
description: This script will complete the Lae's storyline in /dawnsanctum.
tags: hollowborn, saga, lae, dawn, dawn sanctum, quest,vindicators, sanctum,dawnsanctum
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;

public class DawnSanctum
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreHollowbornStory HB
    {
        get => _HB ??= new CoreHollowbornStory();
        set => _HB = value;
    }
    private static CoreHollowbornStory _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        HB.DawnSanctum();

        Core.SetOptions(false);
    }
}
