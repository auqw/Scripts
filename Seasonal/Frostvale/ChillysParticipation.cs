/*
name: Chilly's Participation
description: This will finish the quest that is required to get free acs throughout the event.
tags: chillys-participation, seasonal, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class ChillysQuest
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    public static int questID = 9988;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        ChillysParticipation();

        Core.SetOptions(false);
    }

    public void ChillysParticipation()
    {
        if (Core.isCompletedBefore(questID))
            return;

        if (!Bot.Flash.CallGameFunction<bool>("world.myAvatar.isEmailVerified"))
            Core.Logger("Your email adres is not verified!", messageBox: true, stopBot: true);
        Farm.Experience(30);

        Core.EnsureAccept(questID);
        Core.HuntMonsterMapID("battleontown", 1, "Reminder Delivered");
        Core.EnsureComplete(questID);
    }
}
