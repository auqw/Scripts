/*
name: CornelisReborn
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Cornelis[mem].cs
using Skua.Core.Interfaces;

public class CornelisRebornbadge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static Cornelis Corn
    {
        get => _Corn ??= new Cornelis();
        set => _Corn = value;
    }
    private static Cornelis _Corn;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (Core.HasAchievement(13))
        {
            Core.Logger("Already have the Cornelis Reborn badge");
            return;
        }

        Core.Logger($"Doing Cornelis story for the badge");
        Corn.StoryLine();
    }
}
