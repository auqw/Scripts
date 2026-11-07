/*
name: (Chaos Finale Bonus) Pyramid of Pain
description: This will finish the Pyramid of Pain quest.
tags: story, quest, chaos-saga, 13-lords-of-chaos, chaos-finale, bonus, pyramid-of-pain, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class PyramidofPain
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        PyramidofPain_Line();

        Core.SetOptions(false);
    }

    public void PyramidofPain_Line()
    {
        if (Core.isCompletedBefore(3659))
            return;

        if (!Core.IsMember)
        {
            Core.Logger("You need to be a member for complete this questline.");
            return;
        }

        Story.PreLoad(this);

        // Prove Your Worth
        if (!Story.QuestProgression(3640))
        {
            Core.EnsureAccept(3640);
            Core.HuntMonster("pyramidpain", "Cactus Creeper ", "Cup of Cactus Water", 5);
            Core.HuntMonster("pyramidpain", "Golden Scarab ", "Slab of Scarab Meat", 3);
            Core.EnsureComplete(3640);
        }
        // Still Not Impressed?
        Story.KillQuest(3641, "pyramidpain", "Pyramid Vase");
        // A Prickly Situation
        Story.KillQuest(3642, "pyramidpain", "Cactus Creeper ");
        // Beetle Battle Armor
        Story.KillQuest(3643, "pyramidpain", "Golden Scarab ");
        // Pyramid Pilgrimage
        if (!Story.QuestProgression(3644))
        {
            Core.EnsureAccept(3644);
            Core.HuntMonster("pyramidpain", "Cactus Creeper ", "Additional Cup of Cactus Water", 5);
            Core.HuntMonster("pyramidpain", "Golden Scarab ", "Stored Slab of Scarab Meat", 6);
            Core.EnsureComplete(3644);
        }
        // Blood in the Sand
        Story.KillQuest(3645, "pyramidpain", "Sandshark ");
        // Defeat Tribal Traitors
        Story.KillQuest(3646, "pyramidpain", "Kalestri Worshipper ");
        // Marking The Territory
        Story.MapItemQuest(3647, "pyramidpain", 2758, 7);
        // Raiders of the Lost Artifact
        Story.KillQuest(3648, "pyramidpain", "Tomb Robber ");
        // Surviving the Swarms
        Story.KillQuest(3649, "pyramidpain", "Sandshark ");
        // The Legend of Eldo
        Story.KillQuest(3650, "pyramidpain", "Pyramid Vase");
        // There's No Point
        if (!Story.QuestProgression(3651))
        {
            Core.EnsureAccept(3651);
            Core.HuntMonster("pyramidpain", "Cactus Creeper ", "Cacti Cut", 7);
            Core.HuntMonster("pyramidpain", "Golden Scarab ", "Scarab Squashed", 5);
            Core.EnsureComplete(3651);
        }
        // Trail of Naan Crumbs
        Story.MapItemQuest(3652, "pyramidpain", 2770, 13);
        // Tricky Trip Wires
        Story.KillQuest(3653, "pyramidpain", "Kalestri Worshipper ");
        // Shooing Away Scarabs
        Story.KillQuest(3655, "pyramidpain", "Golden Scarab ");
        // Humor is Close to the Heart
        Story.KillQuest(3654, "pyramidpain", "Golden Scarab ");
        // Purple is Not Your Color
        Story.KillQuest(3656, "pyramidpain", "Chaorrupted Robber ");
        // All Alone
        Story.KillQuest(3657, "pyramidpain", "Kalestri Worshipper ");
        // Beginning of the End
        Story.KillQuest(3658, "pyramidpain", "Pyramid Vase");
        // The Ultimate Sacrifice
        Story.KillQuest(3659, "pyramidpain", "Viridi");
    }
}
