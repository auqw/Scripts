/*
name: Stubborn
description: null
tags: badge, Stubborn
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class Stubborn
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (Core.HasAchievement(56) || !Core.IsMember)
        {
            Core.Logger(
                !Core.IsMember ? "Player isnt member" : "Already have the \"Stubborn\" badge"
            );
            return;
        }

        //ensure private...because yes
        Core.Join("twig-100000", "r2", "left");
        for (int i = 0; i < 100; i++)
        {
            //leave autocorrect enabled as it double jumps and gets it done quicker.
            Bot.Map.Jump("r2", "left", false);
            Core.Sleep();
            Core.Logger($"Jumping {i} times");
        }
    }
}
