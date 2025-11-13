/*
name: Legion Barracks
description: This script completes the storyline in \legionbarracks.
tags: legionbarracks, legion, barracks, seasonal, dage, story
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/CoreDageBirthday.cs
using Skua.Core.Interfaces;

public class LegionBarracks
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreDageBirthday Dage
    {
        get => _Dage ??= new CoreDageBirthday();
        set => _Dage = value;
    }
    private static CoreDageBirthday _Dage;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Dage.LegionBarracks();
        Core.SetOptions(false);
    }
}
