/*
name: Spookeasy
description: This will finish the Spookeasy quest.
tags: story, quest, memets-realm, spookeasy
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/MemetsRealm/CoreMemet.cs
using Skua.Core.Interfaces;

public class Spookeasy
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static MemetsRealm Memet
    {
        get => _Memet ??= new MemetsRealm();
        set => _Memet = value;
    }
    private static MemetsRealm _Memet;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Memet.Spookeasy();

        Core.SetOptions(false);
    }
}
