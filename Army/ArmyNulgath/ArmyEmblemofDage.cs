/*
name: Army Emblem of Dage
description: uses an army to farm Emblem of Dage
tags: emblem of dage, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmyEmblemofDage
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }
    private static CoreLegion _Legion;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public string OptionsStorage = "ArmyEmblemofDage";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Emblem of Dage", "Legion Round 4 Medal" });

        Core.SetOptions(disableClassSwap: true);

        Core.Logger("~\"All\"~ Army Scripts have been disabled by the author.", "**READ ME!!**", stopBot: true);
        // Setup();

        Core.SetOptions(false);
    }

    public void Setup(int quant = 500)
    {
        if (Core.CheckInventory("Emblem of Dage", quant))
            return;

        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Legion.LegionRound4Medal();

        Core.FarmingLogger("Emblem of Dage", quant);
        Core.AddDrop("Emblem of Dage", "Legion Seal", "Gem of Mastery");
        Core.EquipClass(ClassType.Farm);

        Core.RegisterQuests(4742);

        Army.AggroMonMIDs(5, 6, 7, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 41, 43, 49, 50, 51, 52);
        Army.AggroMonStart("shadowblast");
        Army.DivideOnCells("r6", "r10", "r11", "r12", "r16", "r18");

        Bot.Options.AttackWithoutTarget = true;
        while (!Bot.ShouldExit && !Core.CheckInventory("Emblem of Dage", quant))
            Bot.Combat.Attack("*");
        Bot.Options.AttackWithoutTarget = false;
        Army.AggroMonStop(true);
        Core.CancelRegisteredQuests();
    }
}
