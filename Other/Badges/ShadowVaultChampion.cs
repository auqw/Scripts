/*
name: ShadowVaultChampion
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ShadowVault.cs
using Skua.Core.Interfaces;

public class ShadowVaultChampion
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static ShadowVault SV
    {
        get => _SV ??= new ShadowVault();
        set => _SV = value;
    }
    private static ShadowVault _SV;

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

        Core.Logger($"Doing ShadowVault story for {badge} badge");
        SV.StoryLine();
    }

    private string badge = "ShadowScythe Champion";
}
