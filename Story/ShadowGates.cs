/*
name: Shadow Gates Story
description: This will finish the Shadow Gates Story.
tags: story, quest, shadowgates
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class ShadowGates
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static Core13LoC Loc
    {
        get => _Loc ??= new Core13LoC();
        set => _Loc = value;
    }
    private static Core13LoC _Loc;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        Loc.Prologue();

        if (Core.isCompletedBefore(3292))
            return;

        Story.PreLoad(this);

        //Re-Stolen Keys 3285
        Story.KillQuest(3285, "ShadowGates", "Chaorrupted Rogue");

        //Reagents of Chaos 3286
        Story.KillQuest(3286, "ShadowGates", "Chaorrupted Mage");

        //Conscripted Chaorruption 3287
        Story.KillQuest(3287, "ShadowGates", "Chaos Warrior");

        //Tools of Chaos 3288
        Story.KillQuest(3288, "ShadowGates", "Chaorrupted Rogue");

        //Disarmingly Roguish 3289
        Story.MapItemQuest(3289, "ShadowGates", 2327);

        //Dock Blocked 3290
        Story.KillQuest(3290, "ShadowGates", "Chaorrupted Mage");

        //Skull Smashing 3291
        Story.MapItemQuest(3291, "ShadowGates", 2328);

        //Destroy Chaorruption 3292
        Story.KillQuest(3292, "ShadowGates", "Chaorruption");
    }
}
