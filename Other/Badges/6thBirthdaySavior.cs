/*
name: 6thBirthdaySavior
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Artixpointe.cs
using Skua.Core.Interfaces;

public class BirthdaySavior
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static Artixpointe AP
    {
        get => _AP ??= new Artixpointe();
        set => _AP = value;
    }
    private static Artixpointe _AP;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (Core.HasWebBadge(badge))
        {
            Core.Logger($"Already have the {badge} badge");
            return;
        }

        Core.Logger($"Doing Artixpointe story for {badge} badge");
        AP.OmniArtifact();
    }

    private string badge = "6th Birthday Savior";
}
