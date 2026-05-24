/*
name: Rubedo Peak
description: Completes Rubedo Peak storyline.
tags: RubedoPeak, Rubedo Peak, storyline,aor,age of ruin, albedo,aleister,jaania
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs

using Skua.Core.Interfaces;

public class RubedoPeak
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        AOR.RubedoPeak();
        Core.SetOptions(false);
    }
}
