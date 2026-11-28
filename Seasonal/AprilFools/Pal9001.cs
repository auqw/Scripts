/*
name: Pal9001
description: Completes the quest chain in /pal9001.
tags: pal9001, story, quests, pal9001
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Pal9001
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
        if (Core.isCompletedBefore(2907))
            return;

        Story.PreLoad(this);

        // 2902 | Slay the Necromancer's Soldiers
        if (!Story.QuestProgression(2902))
        {
            Core.EnsureAccept(2902);
            Core.HuntMonster("ravenscar", "Restless Undead", "Crumpled Book Page", 1);
            Core.HuntMonster("ravenscar", "Restless Undead", "Undead Slain", 6);
            Core.EnsureComplete(2902);
        }

        // 2903 | The Book of Portals
        if (!Story.QuestProgression(2903))
        {
            Core.EnsureAccept(2903);
            Core.GetMapItem(1768, 1, "ravenscar"); // Missing Spellbook
            Core.EnsureComplete(2903);
        }

        // 2904 | Battle for a Lost Love
        if (!Story.QuestProgression(2904))
        {
            Core.EnsureAccept(2904);
            Core.HuntMonster("ravenscar", "Restless Undead", "Photo Found", 1);
            Core.EnsureComplete(2904);
        }

        // 2905 | Into the Blight
        if (!Story.QuestProgression(2905))
        {
            Core.EnsureAccept(2905);
            Core.HuntMonster("ravenscar", "Undead Soldier", "Undead Slain", 3);
            Core.EnsureComplete(2905);
        }

        // 2906 | Gateway Guardian
        if (!Story.QuestProgression(2906))
        {
            Core.EnsureAccept(2906);
            Core.HuntMonster("ravenscar", "Shadowman", "Gate Key", 1);
            Core.EnsureComplete(2906);
        }

        // 2907 | Blightbringer's Doom
        if (!Story.QuestProgression(2907))
        {
            Core.EnsureAccept(2907);
            Core.GetMapItem(1769, 1, "ravenscar"); // Gate Opened
            Core.EnsureComplete(2907);
        }
    }
}
