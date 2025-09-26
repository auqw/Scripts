/*
name: Army leveling
description: Uses your army, and different methods, to level up
tags: army, icestormarena, icestormunder, icewing, aggro
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Models.Monsters;
using System.Collections.Generic;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class ArmyLeveling
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static SevenCircles SC { get => _SC ??= new SevenCircles(); set => _SC = value; }
    private static SevenCircles _SC;
    private static CoreSoW SoW { get => _SoW ??= new CoreSoW(); set => _SoW = value; }
    private static CoreSoW _SoW;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDOY CoreDOY { get => _CoreDOY ??= new CoreDOY(); set => _CoreDOY = value; }
    private static CoreDOY _CoreDOY;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public bool DontPreconfigure = true;
    public string OptionsStorage = "Army Leveling";
    public List<IOption> Options = new()
    {
        new Option<MethodV2>("LevelMethod", "Map selection", "Which map to farm Experience?", MethodV2.IceStormArena),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.player7,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public int level = 75;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Level();
        Core.SetOptions(false);
    }

    public void Level(int level = 101)
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Farm.ToggleBoost(BoostType.Experience);
        Farm.ToggleBoost(BoostType.Gold);

        // Get the selected method from the configuration
        MethodV2 selectedMethod = Bot.Config!.Get<MethodV2>("LevelMethod");
        // Execute the selected method
        switch (selectedMethod)
        {
            case MethodV2.IceStormArena:
                Core.EquipClass(ClassType.Farm);
                //Army.WaitForParty("whitemap");
                Army.AggroMonCells("r22");
                Army.AggroMonStart("icestormarena");
                Army.DivideOnCells("r22");
                Core.RegisterQuests();

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;

            case MethodV2.IceStormUnder:
                if (Bot.Player.Level < 75)
                    Core.Logger("Player is below lvl 75, which is\n" +
                    "required for the map. --stopping", stopBot: true);
                Core.EquipClass(ClassType.Farm);
                //Army.WaitForParty("whitemap");
                Army.AggroMonCells("r2");
                Army.AggroMonStart("icestormunder");
                Army.DivideOnCells("r2");

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("Frost Spirit");
                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;

            case MethodV2.IceWing:
                if (Bot.Player.Level < 75)
                    Core.Logger("Player is below lvl 75, required for\n" +
                    "the map --stopping", stopBot: true);
                Core.EquipClass(ClassType.Solo);
                //Army.WaitForParty("whitemap");
                Army.AggroMonCells("Enter");
                Army.AggroMonStart("icewing");
                Army.DivideOnCells("Enter");
                Core.RegisterQuests(Core.IsMember ? 6635 : 6632);

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;

            case MethodV2.SevenCirclesWar:
            ForWhenWarsGetNerfed:
                SC.CirclesWar(true);
                Core.EquipClass(ClassType.Farm);
                //Army.WaitForParty("whitemap");
                Army.AggroMonCells("Enter", "r2", "r3");
                Army.AggroMonStart("sevencircleswar");
                Army.DivideOnCells("Enter", "r2", "r3");
                Core.RegisterQuests(7979, 7980, 7981);

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;

            case MethodV2.Streamwar:
                SoW.TimestreamWar();
                Core.EquipClass(ClassType.Farm);
                Core.AddDrop("Prismatic Seams");
                //Army.WaitForParty("whitemap");
                Army.AggroMonCells("r3a");
                Army.AggroMonStart("streamwar");
                Army.DivideOnCells("r3a");
                Core.RegisterQuests(8814, 8815);

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;


            case MethodV2.ShadowBattleon_High_Levels:
            case MethodV2.ShadowBattleon_Lower_Levels:
            case MethodV2.ShadowBattleon_Baby_Mode:
                RequiredQuest("shadowbattleon", 9426);
                Core.EquipClass(ClassType.Farm);
                Core.AddDrop("Wisper");

                if (selectedMethod == MethodV2.ShadowBattleon_Baby_Mode)
                    Core.RegisterQuests(9421, 9422, 9423);
                else
                    Core.RegisterQuests(9421, 9422, 9426);

                Core.Logger($"Mode Selected: {selectedMethod}");

                if (selectedMethod == MethodV2.ShadowBattleon_High_Levels || selectedMethod == MethodV2.ShadowBattleon_Lower_Levels)
                {
                    Army.AggroMonCells("r11");
                    Army.AggroMonStart("shadowbattleon");
                    Army.DivideOnCells("r11");
                }
                else if (selectedMethod == MethodV2.ShadowBattleon_Baby_Mode)
                {
                    Army.AggroMonCells("Enter");
                    Army.AggroMonStart("shadowbattleon");
                    Army.DivideOnCells("Enter");
                }

                while (!Bot.ShouldExit && Bot.Player.Level < level)
                {
                    Bot.Combat.Attack("*");
                    Bot.Sleep(200);
                    if (Bot.Player.Target?.HP <= 0)
                        continue;

                }

                Army.AggroMonStop(true);
                Core.JumpWait();
                Farm.ToggleBoost(BoostType.Experience, false);
                Farm.ToggleBoost(BoostType.Gold, false);
                //Army.WaitForParty("whitemap");
                break;

            case MethodV2.HakuWar:
                Quest Quest = Core.EnsureLoad(9601);
                if (Quest.XP < 6000)
                {
                    Core.Logger("XP rates have been nerfed, swapping to method: SCW (its better)");
                    goto ForWhenWarsGetNerfed;
                }
                CoreDOY.DoAll();

                Core.RegisterQuests(9601, 9602, 9603, 9605, 9606);
                Core.EquipClass(ClassType.Farm);
                Army.AggroMonCells("r2", "r4", "r5", "r6", "r7", "r9");
                Army.AggroMonStart("hakuwar");
                Army.DivideOnCells("r2", "r4", "r5", "r6", "r7", "r9");



                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Farm.ToggleBoost(BoostType.Gold, false);
                Core.CancelRegisteredQuests();
                break;
            //add more cases
            case MethodV2.PirateBloodWar:

                Quest? WarQuest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(9873));

                if (WarQuest != null)
                {
                    if (WarQuest.XP < 7500 || !Core.isSeasonalMapActive("piratebloodhub"))
                        goto ForWhenWarsGetNerfed;
                }
                else
                {
                    Core.Logger("Failed to load quest 9873.");
                }

                Core.PrivateRooms = true;
                Core.PrivateRoomNumber = Army.getRoomNr();

                Core.RegisterQuests(9872, 9873);

                Army.AggroMonMIDs(Core.FromTo(1, 6));
                Army.AggroMonStart("piratelycan");
                Army.DivideOnCells("r2", "r3");

                while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Farm.ToggleBoost(BoostType.Gold, false);
                Core.CancelRegisteredQuests();
                break;
                /*
                case Method.Method:
                Core.EquipClass(ClassType.ClassType);
                //Army.WaitForParty("map");
                Army.AggroMonCells(cells);
                Army.AggroMonStart("map");
                Army.DivideOnCells("cell");
                Core.RegisterQuests(questIDs);
                while (!Bot.ShouldExit && Bot.Player.Level < level)
                    Bot.Combat.Attack("*");
                Army.AggroMonStop(true);
                Core.JumpWait();
                //Army.WaitForParty("whitemap");
                break;

                */
        }
        Core.CancelRegisteredQuests();
        Core.JumpWait();
    }

    public enum MethodV2
    {
        IceStormArena = 1,
        IceStormUnder = 2,
        Streamwar = 3,
        SevenCirclesWar = 4,
        IceWing = 5,
        ShadowBattleon_Baby_Mode = 6,
        ShadowBattleon_Lower_Levels = 7,
        ShadowBattleon_High_Levels = 8,
        HakuWar = 9,
        PirateBloodWar = 10
    }


    void RequiredQuest(string map, int Quest)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(Quest));
        if (QuestData == null)
        {
            Core.Logger($"Failed to load quest {Quest} in {map}.");
            return;
        }
        if (Core.isCompletedBefore(Quest))
        {
            Core.Logger($"{QuestData.Name} [ {QuestData.ID}] Already unlocked! onto the gains.");
            return;
        }

        Core.Logger($"Unlocking {QuestData.Name} [ {QuestData.ID}]");
        switch (map)
        {
            case "shadowbattleon":

                Core.EquipClass(ClassType.Solo);

                // Mega Shadow Hunt Medal
                Story.KillQuest(9422, "shadowbattleon", "Doomed Beast");
                // Early Autopsy
                Story.KillQuest(9423, "shadowbattleon", "Doomed Beast");
                // Given Life and Purpose
                Story.KillQuest(9424, "shadowbattleon", "Possessed Armor");
                // Adult Hatchling
                Story.KillQuest(9425, "shadowbattleon", "Ouro Spawn");
                // Solidified Light
                Story.KillQuest(9426, "shadowbattleon", "Tainted Wraith");
                Core.Logger($"{QuestData.Name} [ {QuestData.ID}] Unlocked! Onto the gains.");
                break;

            case "Default":
                //Example Case
                break;
        }
        Core.JumpWait();
        //Army.WaitForParty("Whitemap");
    }

}
