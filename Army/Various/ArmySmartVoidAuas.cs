/*
name: Smart Void Auras (Army)
description: Farms Void Auras with the best method available using your army.
tags: army, void, aura, smart, VA, NSOD, necrotic, sword, doom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmySmartVoidAuras
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

    public string OptionsStorage = "RVAArmy";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "sellToSync",
            "Sell to Sync",
            "Sell items to make sure the army stays syncronized.\nIf off, there is a higher chance your army might desyncornize",
            false
        ),
        new Option<bool>("MemberMethod", "use member method", "Use the member method?", false),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        //limit to 4 due to /dragonchallenge
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public string[] VA =
    {
        "Void Aura",
        "Astral Ephemerite Essence",
        "Belrot the Fiend Essence",
        "Black Knight Essence",
        "Tiger Leech Essence",
        "Carnax Essence",
        "Chaos Vordred Essence",
        "Dai Tengu Essence",
        "Unending Avatar Essence",
        "Void Dragon Essence",
        "Creature Creation Essence",
    };

    public string[] VASDKA = { "Void Aura", "Empowered Essence", "Malignant Essence" };

    public void ScriptMain(IScriptInterface bot)
    {
        if (Core.CheckInventory(14474))
            Core.BankingBlackList.AddRange(VA);
        else
            Core.BankingBlackList.AddRange(VASDKA);

        Core.SetOptions();

        Setup();

        Core.SetOptions(false);
    }

    public void Setup()
    {
        Core.OneTimeMessage(
            "Only for army",
            "This is intended for use with an army, not for solo players."
        );

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Bot.Events.PlayerAFK += PlayerAFK;
        if (Bot.Config!.Get<bool>("MemberMethod") && Core.CheckInventory(14474))
            CommandingShadowEssences(7501);
        else
        {
            Farm.EvilREP();
            RetrieveVoidAurasArmy(7501);
        }
        Bot.Events.PlayerAFK -= PlayerAFK;
    }

    public void CommandingShadowEssences(int Quantity)
    {
        if (Core.CheckInventory("Void Aura", Quantity))
            return;

        Core.AddDrop(VASDKA);
        Core.Logger($"Farming Void Aura's with SDKA Method");
        Core.FarmingLogger($"Void Aura", Quantity);
        Core.RegisterQuests(4439);
        if (Bot.Config!.Get<bool>("sellToSync"))
            Army.SellToSync("Void Aura", Quantity);
        Core.AddDrop("Void Aura");
        Core.Join("shadowrealmpast");

        Army.DivideOnCells("Enter", "r2", "r3", "r4");
        Core.Sleep(1500);
        Bot.Player.SetSpawnPoint();
        Core.EquipClass(Bot.Player.Cell == "r4" ? ClassType.Solo : ClassType.Farm);

        Army.AggroMonMIDs(Core.FromTo(1, 11));
        Army.AggroMonStart("shadowrealmpast");
        while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", Quantity))
        {
            while (!Bot.ShouldExit && !Bot.Player.Alive)
            {
                Core.Sleep();
            }

            if (!Bot.Player.HasTarget && !Core.CheckInventory("Void Aura", Quantity))
                Bot.Combat.Attack("*");

            Bot.Sleep(500);
        }
        Army.AggroMonStop(true);
        Core.JumpWait();
    }

    public void RetrieveVoidAurasArmy(int Quantity)
    {
        if (Core.CheckInventory("Void Aura", Quantity))
            return;

        Core.AddDrop(VA);
        Core.Logger($"Farming Void Aura's with Non-SDKA Method");
        Core.FarmingLogger($"Void Aura", Quantity);

        // Dictionary storing all ArmyHunt parameters
        Dictionary<
            string,
            (
                string map,
                int[] MIDs,
                string[] Cells,
                string item,
                ClassType classType,
                bool isTemp,
                int quant
            )
        > armyHunts = new()
        {
            {
                "Astral",
                (
                    "timespace",
                    new[] { 1, 2, 3, 4, 5 },
                    new[] { "Enter", "Frame1" },
                    "Astral Ephemerite Essence",
                    ClassType.Farm,
                    false,
                    100
                )
            },
            {
                "Belrot",
                (
                    "citadel",
                    new[] { 21 },
                    new[] { "m13" },
                    "Belrot the Fiend Essence",
                    ClassType.Farm,
                    false,
                    100
                )
            },
            {
                "BlackKnight",
                (
                    "greenguardwest",
                    new[] { 22 },
                    new[] { "BKWest15" },
                    "Black Knight Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "TigerLeech",
                (
                    "mudluk",
                    new[] { 18 },
                    new[] { "Boss" },
                    "Tiger Leech Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "Carnax",
                (
                    "aqlesson",
                    new[] { 17 },
                    new[] { "Frame9" },
                    "Carnax Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "ChaosVordred",
                (
                    "necrocavern",
                    new[] { 5 },
                    new[] { "r16" },
                    "Chaos Vordred Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "DaiTengu",
                (
                    "hachiko",
                    new[] { 10 },
                    new[] { "Roof" },
                    "Dai Tengu Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "UnendingAvatar",
                (
                    "timevoid",
                    new[] { 12 },
                    new[] { "Frame8" },
                    "Unending Avatar Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "VoidDragon",
                (
                    "dragonchallenge",
                    new[] { 4 },
                    new[] { "r4" },
                    "Void Dragon Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
            {
                "CreatureCreation",
                (
                    "maul",
                    new[] { 17 },
                    new[] { "r3" },
                    "Creature Creation Essence",
                    ClassType.Solo,
                    false,
                    100
                )
            },
        };

        while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", Quantity))
        {
            Core.EnsureAccept(4432);

            // Loop through the dictionary to call ArmyHunt
            foreach (var hunt in armyHunts.Values)
                ArmyHunt(
                    hunt.map,
                    hunt.MIDs,
                    hunt.Cells,
                    hunt.item,
                    hunt.classType,
                    hunt.isTemp,
                    hunt.quant
                );

            Core.EnsureCompleteMulti(4432);
        }
        Core.ConfigureAggro(false);
    }

    void ArmyHunt(
        string map,
        int[] MIDs,
        string[] Cells,
        string item,
        ClassType classType,
        bool isTemp = false,
        int quant = 1
    )
    {
        if (Bot.Config!.Get<bool>("sellToSync"))
            Army.SellToSync(item, quant);

        Core.AddDrop(item);

        Core.Join(map);

        Army.DivideOnCells(Cells);
        Core.Sleep(1500);
        Bot.Player.SetSpawnPoint();
        Army.AggroMonMIDs(MIDs);
        Core.EquipClass(classType);
        Army.AggroMonStart(map);

        while (
            !Bot.ShouldExit
            && !(isTemp ? Bot.TempInv.Contains(item, quant) : Bot.Inventory.Contains(item, quant))
        )
        {
            while (!Bot.ShouldExit && !Bot.Player.Alive)
            {
                Core.Sleep();
            }

            if (
                !Bot.Player.HasTarget
                && !(
                    isTemp ? Bot.TempInv.Contains(item, quant) : Bot.Inventory.Contains(item, quant)
                )
            )
                Bot.Combat.Attack("*");

            Bot.Sleep(500);
        }

        Army.AggroMonStop(true);
        Core.JumpWait();
    }

    public void PlayerAFK()
    {
        Core.Logger("Anti-AFK engaged");
        Core.Sleep(1500);
        Bot.Send.Packet("%xt%zm%afk%1%false%");
    }
}
