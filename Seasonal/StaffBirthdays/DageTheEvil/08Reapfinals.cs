/*
name: Reapfinals
description: This script completes the storyline in \Reapfinals.
tags: Reapfinals, legion, tournament, seasonal, dage, story, seasonal, dagebirthday, general vaughn
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/CoreDageBirthday.cs
using Skua.Core.Interfaces;

public class Reapfinals
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

        Dage.Reapfinals();
        Core.SetOptions(false);
    }
}
