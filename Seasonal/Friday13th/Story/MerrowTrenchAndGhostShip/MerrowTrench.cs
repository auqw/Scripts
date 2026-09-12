/*
name: Merrow Trench
description: Completes the Merrow Trench storyline.
tags: merrow trench, story, seasonal, friday the 13th
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class MerrowTrench
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(7084))
            return;

        Story.PreLoad(this);

        Story.KillQuest(7074, "merrowtrench", "Waterlogged Zombie");
        Story.KillQuest(7075, "merrowtrench", "Fishwing");

        if (!Story.QuestProgression(7076))
        {
            Core.EnsureAccept(7076);
            Core.Join("merrowtrench");
            Core.Jump("r4", "Left");
            Bot.Wait.ForPickup("Meet Voltaire");
            Core.EnsureComplete(7076);
        }

        Story.KillQuest(7077, "merrowtrench", "Merdraconian Squatter");
        Story.KillQuest(7078, "merrowtrench", "Electric Eel");
        Story.KillQuest(7079, "merrowtrench", "Waterlogged Zombie");
        Story.KillQuest(7080, "merrowtrench", "Moaney McBoney");
        Story.KillQuest(7081, "merrowtrench", "Merdraconian Squatter");
        Story.MapItemQuest(7082, "merrowtrench", 6691);
        Story.MapItemQuest(7083, "merrowtrench", 6692);
        Story.KillQuest(7083, "merrowtrench", "Electric Eel");
        Story.KillQuest(7084, "merrowtrench", "Cecaelia");
    }
}
