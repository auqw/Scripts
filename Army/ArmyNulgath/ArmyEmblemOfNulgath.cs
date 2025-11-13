/*
name: Army Emblem Of Nulgath
description: uses an army to farm Emblem of Nulgath
tags: emblem of nulgah, army, nation round 4 medal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyEmblemOfNulgath
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    private static CoreBots sCore
    {
        get => _sCore ??= new CoreBots();
        set => _sCore = value;
    }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }

    private static CoreArmyLite _sArmy;

    public string OptionsStorage = "ArmyEmblemOfNulgathV2";
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
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Loot);

        Core.SetOptions(disableClassSwap: true);

        Core.Logger(
            "~\"All\"~ Army Scripts have been disabled by the author.",
            "**READ ME!!**",
            stopBot: true
        );
        // FarmingTime();

        Core.SetOptions(false);
    }

    public void FarmingTime()
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.OneTimeMessage(
            "Only for army",
            "This is intended for use with an army, not for solo players."
        );

        if (!Core.CheckInventory("Nation Round 4 Medal"))
        {
            Core.Logger("Nation Round 4 Medal not found, getting it for you");
            Nation.NationRound4Medal();
        }

        Core.EquipClass(ClassType.Farm);
        Core.AddDrop(Loot);
        Core.RegisterQuests(4748);

        if (Bot.Config != null)
        {
            string player5 = Bot.Config.Get<string>("player5") ?? string.Empty;
            string player6 = Bot.Config.Get<string>("player6") ?? string.Empty;

            if (string.IsNullOrEmpty(player5.Trim()) && string.IsNullOrEmpty(player6.Trim()))
                Army.AggroMonCells("r13", "r14", "r15", "r16");
            else
                Army.AggroMonCells("r13", "r14", "r15", "r16", "r17", "r4");
        }
        Army.AggroMonStart("shadowblast");
        Army.DivideOnCells("r13", "r14", "r15", "r16", "r17", "r4");

        while (!Bot.ShouldExit)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Core.CancelRegisteredQuests();
    }

    private string[] Loot = { "Fiend Seal", "Gem of Domination", "Emblem of Nulgath" };
}

// public enum Rewards
// {
//     TotemofNulgath = 5357,
//     GemofNulgath = 6136,
//     EssenceofNulgath = 0
// }
