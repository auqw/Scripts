/*
name: Void Chasm Story
description: This script will complete the storyline in /voidchasm.
tags: voidchasm, chasm, nation, nulgath, story, ana di carcano, gravelyn, jadzia
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
using Skua.Core.Interfaces;

public class VoidChasm
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static VoidRefuge VR { get => _VR ??= new VoidRefuge(); set => _VR = value; }
    private static VoidRefuge _VR;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9552))
            return;

        VR.Storyline();

        Story.PreLoad(this);

        // Light Headed 9543
        Story.KillQuest(9543, "voidchasm", "Paladin Ascendant");

        // Forgiveness of Debt 9544
        Story.KillQuest(9544, "voidchasm", "Nation Outrider");

        // Hyperspace-Plasia 9545
        Story.MapItemQuest(9545, "voidchasm", 12619, 6);
        Story.KillQuest(9545, "voidchasm", "Paladin Ascendant");

        // Tangled Aortas 9546        
        Story.MapItemQuest(9546, "voidchasm", new[] { 12620, 12621 });

        // Nation Code 9547
        Story.MapItemQuest(9547, "voidchasm", 12622);
        Story.KillQuest(9547, "voidchasm", "Nation Outrider");

        // Snake Ears 9548
        Story.KillQuest(9548, "voidchasm", "Void Fang");

        // Faustian Leftovers 9549
        Story.MapItemQuest(9549, "voidchasm", new[] { 12623, 12627, 12628 });
        Story.KillQuest(9549, "voidchasm", "The Hushed");

        // Broken Lives 9550
        Story.MapItemQuest(9550, "voidchasm", 12624, 4);
        Story.KillQuest(9550, "voidchasm", new[] { "The Hushed", "Void Fang" });

        // Call of the Fiend 9551
        Core.EquipClass(ClassType.Solo);
        Story.MapItemQuest(9551, "voidchasm", new[] { 12625, 12626 });
        Story.KillQuest(9551, "voidchasm", "Carnage");

        // Famiglia di Carcano 9552
        Story.KillQuest(9552, "voidchasm", "Carcano");
    }
}
