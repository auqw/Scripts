/*
name: Darkons Third Errand
description: Darkons Third Errand
tags: darkon, third, errand
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;

public class ThirdErrand
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Darkon.ThirdErrand();

        Core.SetOptions(false);
    }
}
