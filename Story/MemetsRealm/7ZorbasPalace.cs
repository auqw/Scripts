/*
name: Zorbas Palace
description: This will finish the Zorbas Palace quest.
tags: story, quest, memets-realm, zorbas-palace
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/MemetsRealm/CoreMemet.cs
using Skua.Core.Interfaces;

public class ZorbasPalace
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

        Memet.ZorbasPalace();

        Core.SetOptions(false);
    }
}
