/*
name: Bright Shadow
description: This will finish the Bright Shadow quest.
tags: story, quest, shadow-chaos, bright-shadow
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs

using Skua.Core.Interfaces;

public class BrightShadow
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreSoC CoreSoC
    {
        get => _CoreSoC ??= new CoreSoC();
        set => _CoreSoC = value;
    }
    private static CoreSoC _CoreSoC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreSoC.BrightShadow();

        Core.SetOptions(false);
    }
}
