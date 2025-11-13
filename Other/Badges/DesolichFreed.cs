/*
name: DesolichFreed
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/EtherstormWastes.cs
using Skua.Core.Interfaces;

public class DesolichFreed
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static EtherStormWastes ESW
    {
        get => _ESW ??= new EtherStormWastes();
        set => _ESW = value;
    }
    private static EtherStormWastes _ESW;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (!Core.IsMember)
            return;

        if (Core.HasWebBadge("Desoloth Freed"))
        {
            Core.Logger($"Already have the Desoloth Freed badge");
            return;
        }

        Core.Logger(
            "Gotta do the EtherStorm story first, will get the badge during the story don't worry :D"
        );
        ESW.DoAll();
    }
}
