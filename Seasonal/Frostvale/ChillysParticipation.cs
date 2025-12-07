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

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        ChillysParticipation();

        Core.SetOptions(false);
    }

    //Edit for future years quests vv <- No need to edit now, just edit the quest ID in ChillysParticipation.cs
    public void ChillysParticipation(int questID = 10510)
    {
        if (Core.isCompletedBefore(questID))
            return;

        if (!Bot.Flash.CallGameFunction<bool>("world.myAvatar.isEmailVerified"))
            Core.Logger("Your email adres is not verified!", messageBox: true, stopBot: true);
        Farm.Experience(30);

        Core.EnsureAccept(questID);
        Core.HuntMonsterMapID("battleontown", 1, "Reminder Delivered");
        Core.EnsureComplete(questID);
        Bot.Wait.ForQuestComplete(questID);
        // Longer delay as soemtimes ae would throttle ppl with alot of
        //  accs to just get stuck on loading in.
        Bot.Sleep(5000);
    }
}
