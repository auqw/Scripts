/*
name: Shadowflame War Medal
description: farms "Item" from uest: "shadow medals, mega shadow medals".
tags: shadowflame war medal, shadow medal, mega shadow medal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ShadowflameWarMedal
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;
    private static CoreSoC SoC
    {
        get => _SoC ??= new CoreSoC();
        set => _SoC = value;
    }
    private static CoreSoC _SoC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Medals();

        Core.SetOptions(false);
    }

    public void Medals(int quant = 300)
    {
        if (Core.CheckInventory("ShadowFlame War Medal", quant))
            return;

        SoW.ShadowWar();
        SoC.DualPlane();
        Core.FarmingLogger("ShadowFlame War Medal", quant);
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(7685, 7686);
        while (!Bot.ShouldExit && !Core.CheckInventory("ShadowFlame War Medal", quant))
        {
            Core.HuntMonster("chaosamulet", "Shadowflame Warrior", "Shadow Medal", 5, log: false);
            Core.HuntMonster(
                "chaosamulet",
                "Shadowflame Warrior",
                "Mega Shadow Medal",
                3,
                log: false
            );
            Bot.Wait.ForPickup("ShadowFlame War Medal");
        }
        Core.CancelRegisteredQuests();
    }
}
