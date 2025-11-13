/*
name: Mount Maleno
description: This script completes the storyline in /MountMaleno.
tags: age, of, ruin, saga, story, quest, mountmaleno,aleister,dahlia
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;

public class MountMaleno
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        AOR.MountMaleno();

        Core.SetOptions(false);
    }
}
