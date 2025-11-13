/*
name: Army Elemental Binding & Gold Farm
description: Farms Elemental Bindings and optionally gold using your army setup.
tags: army, elemental binding, gold farm
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class ArmyElementalBinding
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
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

    public string OptionsStorage = "ArmyElementalBindings";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "Sell Bindings",
            "GoldFarm",
            "Sell the bindings to max gold, then stack bindings. Otherwise stack for Providence",
            true
        ),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.player7,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        ArmyBits();

        Core.SetOptions(false);
    }

    public void ArmyBits()
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();
        bool sellBindings = Bot.Config!.Get<bool>("GoldFarm");

        Core.EquipClass(ClassType.Solo);
        Core.AddDrop("Elemental Binding");

        Army.AggroMonMIDs(new[] { 1, 2 });
        Army.AggroMonStart("archmage", "Enter", "Spawn");
        Army.DivideOnCells("r2");

        while (!Bot.ShouldExit) // Infinite loop
        {
            foreach (
                Monster mon in Bot.Monsters.MapMonsters.Where(x =>
                    x != null && (x.MapID == 1 || x.MapID == 2)
                )
            )
            {
                if (mon == null)
                    continue;

                while (!Bot.ShouldExit)
                {
                    while (!Bot.ShouldExit && !Bot.Player.Alive)
                    {
                        Bot.Sleep(500);
                    }

                    // Ensure player is in the correct cell
                    if (Bot.Player.Cell != "r2")
                    {
                        Bot.Map.Jump("r2", "Right", autoCorrect: false);
                        Bot.Wait.ForCellChange("r2");
                    }

                    // Attack logic
                    Bot.Combat.Attack(mon.MapID);

                    Bot.Sleep(500);

                    if (mon.HP <= 0)
                        break;

                    // Determine stacking vs selling
                    bool startStackingBindings = Bot.Player.Gold >= 96_000_000;

                    // Exit if we need to sell
                    if (
                        sellBindings
                        && !startStackingBindings
                        && Core.CheckInventory("Elemental Binding", 1000)
                    )
                        break;
                }

                // Sell bindings if needed and the option is enabled
                while (
                    !Bot.ShouldExit
                    && sellBindings
                    && Core.CheckInventory("Elemental Binding", 1)
                    && Bot.Player.Gold < 96_000_000
                )
                {
                    Core.Jump("Enter", "Spawn");
                    Core.SellItem("Elemental Binding", all: true);
                    Core.Jump("r2");
                    break; // Exit foreach, selling done
                }
            }
        }

        Army.AggroMonStop(true);
        Core.CancelRegisteredQuests();
    }
}
