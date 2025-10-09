/*
name: Void Refuge Story
description: This script will complete the storyline in /voidrefuge
tags: voidrefuge, refuge, nation, nulgath, story, ana di carcano, gravelyn
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class VoidRefuge
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
        if (Core.isCompletedBefore(9531))
            return;

        Story.PreLoad(this);

        // Newbie Boom (9522)
        Story.KillQuest(9522, "voidrefuge", "Lightguard Paladin");

        // Tears Like Honey (9523)
        Story.KillQuest(9523, "voidrefuge", "Paladin Ascendant");

        // Work Fiends (9524)
        Story.MapItemQuest(9524, "voidrefuge", 12572);
        Story.KillQuest(9524, "voidrefuge", "Nation Caster");

        // Rewritten Signatures (9525)
        Story.KillQuest(9525, "voidrefuge", "Nation Outrider");

        // Where the Heart is (9526)
        Story.MapItemQuest(9526, "voidrefuge", 12573);
        Story.KillQuest(9526, "voidrefuge", new[] { "Lightguard Paladin", "Paladin Ascendant" });

        // Rip Space (9527)
        Story.MapItemQuest(9527, "voidrefuge", 12574, 5);
        Story.KillQuest(9527, "voidrefuge", "Nation Caster");

        // Permanent Record (9528)
        Story.KillQuest(9528, "voidrefuge", "Nation Outrider");

        // Blood and Water (9529)
        Story.MapItemQuest(9529, "voidrefuge", new[] { 12575, 12576, 12577 });

        // Family Feud (9530)
        Story.KillQuest(9530, "voidrefuge", new[] { "Paladin Ascendant", "Nation Outrider" });

        // Twisted Helix (9531)
        Story.KillQuest(9531, "voidrefuge", "Carnage");
    }
}
