/*
name: Nusantara Bosses Badge
description: This script will obtain the Nusantara Bosses Character Page Badge.
tags: nusantara, bosses, badge, seasonal,indonesian day,rangda,kala,kabasaran waranei,nusantara defender medali
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/IndonesianDay/Rangda.cs
using Skua.Core.Interfaces;

public class NusantaraBosses
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreStory Story = new();
    private RangdaSeasonal Rangda = new();

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
        Rangda.StoryLine();

        // Widow's Wail (10371)
        if (!Story.QuestProgression(10371))
            Core.HuntMonsterQuest(10371, "rangda", "Rangda");

        if (!Story.QuestProgression(10374))
            Core.HuntMonsterQuest(
                10374,
                ("rangda", "Rangda", ClassType.Solo),
                ("kala", "Kala", ClassType.Solo),
                ("wentira", "Kabasaran Waranei", ClassType.Solo)
            );
    }

    private readonly string badge = "Nusantara Bosses Badge";
}
