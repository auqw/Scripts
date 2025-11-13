/*
name: Styx
description: This will finish the Styx quest.
tags: story, quest, isle-of-fotia, styx
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs

using Skua.Core.Interfaces;

public class Styx
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

        CoreIsleOfFotia.Styx();

        Core.SetOptions(false);
    }
}
