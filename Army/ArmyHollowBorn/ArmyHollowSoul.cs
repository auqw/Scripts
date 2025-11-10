/*
name: Army Hollow Soul
description: uses an army to farm Hollow Soul
tags: hollow soul, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyHollowSoul
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public string OptionsStorage = "ArmyHollowSoul";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        // 4 players due to /Shadowrealm
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.BankingBlackList.AddRange(new[] { "Hollow Soul" });
        Core.SetOptions(disableClassSwap: true);

        Core.Logger("~\"All\"~ Army Scripts have been disabled by the author.", "**READ ME!!**", stopBot: true);
        // Setup();

        Core.SetOptions(false);
    }

    public void Setup(int quant = 2500)
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        if (Core.CheckInventory("Hollow Soul", quant))
            return;

        Core.AddDrop("Hollow Soul");
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Hollow Soul", quant);


        while (!Bot.ShouldExit && !Core.CheckInventory("Hollow Soul", quant))
        {
            Core.EnsureAcceptmultiple(new[] { 7553, 7555 });

            ArmyHunt(new[] { 3, 7, 11 }, new[] { "r2", "r4", "r6" }, "shadowrealm", "Darkseed", 8);
            ArmyHunt(new[] { 4, 8, 12 }, new[] { "r2", "r4", "r6" }, "shadowrealm", "Shadow Medallion", 5);

            Core.EnsureComplete(7553);
            Core.EnsureComplete(7555);
        }

        Core.CancelRegisteredQuests();
        Army.AggroMonStop(true);
        //Army.WaitForParty("whitemap", "Hollow Soul");
    }

    void ArmyHunt(int[] MonsterMapIDs, string[] cells, string aggroMonStart, string itemName, int quant = 1)
    {
        Army.AggroMonCells(cells);
        Army.AggroMonStart(aggroMonStart);
        Army.DivideOnCells(cells);

        Core.FarmingLogger(itemName, quant);

        while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quant))
        {
            foreach (int MonsterMapID in MonsterMapIDs)
            {
                if (Bot.Monsters.CurrentAvailableMonsters.Exists(x => x.MapID == MonsterMapID))
                    Bot.Combat.Attack(MonsterMapID);
                Core.Sleep();
            }
        }

        Army.AggroMonClear();
    }











}
