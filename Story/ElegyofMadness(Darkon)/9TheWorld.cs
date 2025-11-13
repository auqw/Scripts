/*
name: The World
description: This will finish the The World quest.
tags: story, quest, elegy-of-madness, darkon, the-world
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;

public class CompleteTheWorld
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Astravia.TheWorld();

        Core.SetOptions(false);
    }
}
