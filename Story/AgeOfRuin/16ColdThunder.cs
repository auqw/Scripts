/*
name: ColdThunder
description: This script completes the storyline in /ColdThunder.
tags: age, of, ruin, saga, story, quest, cold thunder, sovereign of storms, sos
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;

public class ColdThunder
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

        AOR.ColdThunder();

        Core.SetOptions(false);
    }
}
