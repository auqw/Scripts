/*
name: Forge Citrinitas
description: Completes  Forge Citrinitas storyline.
tags: ForgeCitrinitas, Forge Citrinitas, storyline,aor,age of ruin, albedo,aleister,jaania
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs

using Skua.Core.Interfaces;

public class ForgeCitrinitas
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
        AOR.ForgeCitrinitas();
        Core.SetOptions(false);
    }
}
