/*
name: Brimstone
description: This will finish the Brimstone quest.
tags: story, quest, fire-island, brimstone, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/FireIsland/CoreFireIsland.cs

using Skua.Core.Interfaces;

public class Brimstone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFireIsland FI
    {
        get => _FI ??= new CoreFireIsland();
        set => _FI = value;
    }
    private static CoreFireIsland _FI;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FI.Brimstone();

        Core.SetOptions(false);
    }
}
