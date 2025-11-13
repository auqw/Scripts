/*
name: Judgement
description: This will finish the Judgement quest.
tags: story, quest, isle-of-fotia, judgement
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs

using Skua.Core.Interfaces;

public class Judgement
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreIsleOfFotia CoreIsleOfFotia
    {
        get => _CoreIsleOfFotia ??= new CoreIsleOfFotia();
        set => _CoreIsleOfFotia = value;
    }
    private static CoreIsleOfFotia _CoreIsleOfFotia;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreIsleOfFotia.Judgement();

        Core.SetOptions(false);
    }
}
