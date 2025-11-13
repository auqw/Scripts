/*
name: Loughshine
description: This script completes the storyline in /loughshine.
tags: age, of, ruin, saga, story, quest, loughshine, lough shine, ruins of loughshine, caoimhe
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;

public class Loughshine
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        AOR.Loughshine();

        Core.SetOptions(false);
    }
}
