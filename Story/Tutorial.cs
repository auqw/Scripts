/*
name: Tutorial Story
description: This will finish the Tutorial Story.
tags: story, quest, tutorial
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Models;

public class Tutorial
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badges();

        Core.SetOptions(false);
    }

    public void Badges()
    {
        if (Core.HasAchievement(31))
            return;

        string[] achievements = new[]
        {
            "Combat",
            "Interact",
            "Quest",
            "Skill",
            "Shop",
            "Enhance",
            "Rest",
            "World",
            "Emotes",
            "Travel",
        };

        Core.Logger("Doing `Tutorial Badges` to look a bit more \"~legit~\".");

        Core.Join("oaklore");

        // Iterate through achievements and set them
        for (int i = 0; i < achievements.Length; i++)
        {
            string achievement = achievements[i];
            if (Core.HasAchievement(22 + i))
                Core.Logger(
                    $"Achievement: {achievement}, Status: {(Core.HasAchievement(22 + i) ? "✅" : "❌")}"
                );
            Core.SetAchievement(22 + i);
            Bot.Wait.ForActionCooldown(GameActions.DoIA);
            Core.Sleep(1500);
        }
    }
}
