/*
name: Penny for Your Thoughts (Army)
description: Farms Dark Spirit Orbs from Penny for Your Thoughts quest using your army.
tags: army, penny for your thoughts, dark spirit orb, dso
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyPennyForYourThoughts
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

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

    private string[] Loot = { "DoomCoin", "Dark Spirit Orb" };
    public string OptionsStorage = "ArmyPenny";
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

        Core.OneTimeMessage(
            "Only for army",
            "This is intended for use with an army, not for solo players."
        );
        ArmyBits();

        Core.SetOptions(false);
    }

    public void ArmyBits()
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();
        Core.AddDrop(Loot);
        Core.EquipClass(ClassType.Farm);

        Army.AggroMonMIDs(1, 2, 18, 5, 6, 21, 7, 8, 22);
        Army.AggroMonStart("maul");
        Army.DivideOnCells("r2", "r5", "r6");

        if (Core.IsMember)
            Core.RegisterQuests(2089);
        else
            Core.Logger(
                "Player is not member, farm will continue\n" + "but you wont get the spirit orbs"
            );

        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Spirit Orb", 10500))
            Bot.Combat.Attack("*");
        //Army.WaitForParty("whitemap", "Dark Spirit Orb");
        Army.AggroMonStop(true);
        Core.CancelRegisteredQuests();
    }
}
