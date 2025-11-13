/*
name: Spirit Orbs (Army)
description: Farms Spirit Orbs using your army.
tags: army, spirit, orb, BLOD, blinding, light, destiny, good, undead
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmySpiritOrb
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

    public string OptionsStorage = "ArmySpiritOrb";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<int>("amount", "Amount", "Input the amount of spirit orbs to farm", 65000),
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
        Core.SetOptions();

        Setup(Bot.Config?.Get<int>("amount") ?? default(int));

        Core.SetOptions(false);
    }

    public void Setup(int quant = 65000)
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.OneTimeMessage(
            "Only for army",
            "This is intended for use with an army, not for solo players."
        );

        Core.AddDrop(Loot);
        Core.EquipClass(ClassType.Farm);
        Core.Logger($"Farming for {quant} bone dust");

        Core.RegisterQuests(2082, 2083);
        Army.SmartAggroMonStart(
            "battleunderb",
            "Skeleton Warrior",
            "Skeleton Fighter",
            "Undead Champion"
        );

        while (!Bot.ShouldExit && !Core.CheckInventory("Spirit Orb", quant))
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);

        Core.CancelRegisteredQuests();
    }

    private string[] Loot = { "Bone Dust", "Undead Essence", "Undead Energy", "Spirit Orb" };
}
