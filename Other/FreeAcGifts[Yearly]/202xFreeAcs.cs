/*
name: 2026 birthday free acs
description: money
tags: acs, free, roblox
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/Borgars.cs
using Skua.Core.Interfaces;

public class FreeAcs
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private Borgars borgars => new();
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        GetYourAcsHere();
        // Core.Logger("Quest Isnt aviable yet! if this is untrue, ping @tato2 or @bogalj on disc.", "Quest Isnt aviable yet!", true, true);

        Core.SetOptions(false);
    }

    public void GetYourAcsHere()
    {
        if (Core.isCompletedBefore(10894))
        {
            Core.Logger("Quest Already Complete");
            return;
        }
        Core.OneTimeMessage("WARNING", "This Quest is a ONE-TIME quest (per account).", true, true);

        if (
            !Bot.Flash.CallGameFunction<bool>("world.myAvatar.isEmailVerified")
            || Bot.Player.Level < 20
        )
        {
            Core.Logger("You need to be level 20 and have a verified email!");
            return;
        }
        if (!Core.isCompletedBefore(7522))
            borgars.StoryLine();

        Core.EnsureAccept(10894);
        Core.HuntMonster("eventhub", "Agitated Orb", "Free ACs...");
        Core.EnsureComplete(10894);
    }
}
