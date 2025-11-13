/*
name: Sanctuary Aiwass Storyline
description: Completes the Sanctuary Aiwass storyline (quests 10375–10384)
tags: sanctuaryaiwass, storyline
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs

using Skua.Core.Interfaces;

public class SanctuaryAiwass
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
        AOR.SanctuaryAiwass();
        Core.SetOptions(false);
    }
}
