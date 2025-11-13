/*
name: (Badge) Battle Babysitter
description: This will get the Battle Babysitter badge.
tags: badge, doomwood, battle, baby-sitter
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
using Skua.Core.Interfaces;

public class BattleBabysitter
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDoomwood Doomwood
    {
        get => _Doomwood ??= new CoreDoomwood();
        set => _Doomwood = value;
    }
    private static CoreDoomwood _Doomwood;

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

        Core.Logger($"Doing Doomwood story for {badge} badge");
        Doomwood.DoomwoodPart3();
    }

    private string badge = "Battle Babysitter";
}
