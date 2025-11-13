/*
name: ThiefofChaos
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/MagicThief.cs
using Skua.Core.Interfaces;

public class ThiefofChaosBadge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static MagicThief MT
    {
        get => _MT ??= new MagicThief();
        set => _MT = value;
    }
    private static MagicThief _MT;

    public void ScriptMain(IScriptInterface Bot)
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

        Core.Logger($"Doing Magic Thief story for {badge} badge");
        MT.Storyline();
    }

    private string badge = "Thief of Chaos";
}
