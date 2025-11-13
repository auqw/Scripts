/*
name: Icerise Past Story
description: This script completes the Icerisepast quests.
tags: saga, story, quest, seasonal, frostval,frostvale,frost,icerisepast
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
using Skua.Core.Interfaces;

public class Icerisepast
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFrostvale Frost
    {
        get => _Frost ??= new CoreFrostvale();
        set => _Frost = value;
    }
    private static CoreFrostvale _Frost;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Frost.Icerisepast();
        Core.SetOptions(false);
    }
}
