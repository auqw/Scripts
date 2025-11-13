/*
name: Genesis Garden
description: This will finish the Genesis Garden quest.
tags: story, quest, elegy-of-madness, darkon, genesis-garden
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;

public class CompleteGenesisGarden
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

        Astravia.GenesisGarden();

        Core.SetOptions(false);
    }
}
