/*
name: Army Gold
description: Uses different methods with your army to farm gold.
tags: army, gold, battle ground e, dark war legion, dark war nation, seven circles war
*/
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmyGold
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static DarkWarLegionandNation DWLN { get => _DWLN ??= new DarkWarLegionandNation(); set => _DWLN = value; }
    private static DarkWarLegionandNation _DWLN;
    private static SevenCircles SC { get => _SC ??= new SevenCircles(); set => _SC = value; }
    private static SevenCircles _SC;
    private static CoreSoW SoW { get => _SoW ??= new CoreSoW(); set => _SoW = value; }
    private static CoreSoW _SoW;
    private static CoreDOY CoreDOY { get => _CoreDOY ??= new CoreDOY(); set => _CoreDOY = value; }
    private static CoreDOY _CoreDOY;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public string OptionsStorage = "Army_Gold";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<Method>("mapname", "Map selection", "Which map to farm gold?", Method.BattleGroundE),
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

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        Setup(Bot.Config!.Get<Method>("mapname"));

        Core.SetOptions(false);
    }

    public void Setup(Method mapname)
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.EquipClass(ClassType.Farm);
        Farm.ToggleBoost(BoostType.Gold);

        switch ((int)mapname)
        {
            case 0:
                BGE(Bot.Config!.Get<Method>("mapname"));
                break;
            case 1:
                DWL();
                break;
            case 2:
                DWN();
                break;
            case 3:
                SCW();
                break;
            case 4:
                StreamWar();
                break;
            case 5:
            case 6:
            case 7:
                ShadowBattleon();
                break;
            case 8:
                HakuWar();
                break;

            case 9:
                PirateBloodWar();
                break;
        }

    }

    public void BGE(Method mapname)
    {
        if (mapname == 0 && Bot.Player.Level < 61)
            Core.Logger($"Minimum level 61 required for this map (Player level: {Bot.Player.Level}).", "Attention Needed. Bot Stopped!", messageBox: true, stopBot: true);

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.RegisterQuests(Core.IsMember ? new[] { 3991, 3992, 3993 } : new[] { 3991, 3992 });

        Army.AggroMonMIDs(1, 2, 3, 4, 5, 6);
        Army.AggroMonStart("battlegrounde");
        Army.DivideOnCells("r4", "r3", "r2", "r1");



        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    public void DWL()
    {
        DWLN.DarkWarLegion();

        Army.AggroMonMIDs(1, 2, 3, 4, 5, 6, 7, 8);
        Army.AggroMonStart("darkwarlegion");
        Army.DivideOnCells("Enter", "r2", "r3");



        Core.RegisterQuests(8584, 8585, 8586, 8587); //Nation Badges 8584, Mega Nation Badges 8585, A Nation Defeated 8586, ManSlayer? More Like ManSLAIN 8587
                                                     // Army.SmartAggroMonStart("darkwarlegion", "Bloodfiend", "Dreadfiend", "Infernal Fiend", "Manslayer Fiend", "Void Fiend");



        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    public void DWN()
    {
        DWLN.DarkWarNation();

        Core.RegisterQuests(8578, 8579, 8580, 8581); //Legion Badges, Mega Legion Badges, Doomed Legion Warriors, Undead Legion Dread

        Army.AggroMonMIDs(1, 2, 3, 4, 5, 6, 7, 8);
        Army.AggroMonStart("darkwarnation");
        Army.DivideOnCells("Enter", "r2", "r3");



        // Army.SmartAggroMonStart("darkwarnation", "High Legion Inquisitor", "Legion Doomknight", "Legion Dread Knight");
        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    public void SCW()
    {
        SC.CirclesWar(true);

        Core.RegisterQuests(7979, 7980, 7981);

        Army.AggroMonMIDs(1, 2, 3, 4, 5, 6);
        Army.AggroMonStart("sevencircleswar");
        Army.DivideOnCells("Enter", "r2", "r3");



        // Army.SmartAggroMonStart("sevencircleswar", "Wrath Guard", "Heresy Guard", "Violence Guard", "Treachery Guard");
        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    public void StreamWar()
    {
        Core.AddDrop("Prismatic Seams");

        SoW.TimestreamWar();

        Army.AggroMonMIDs(8, 9, 10, 11, 12, 13);
        Army.AggroMonStart("streamwar");
        Army.DivideOnCells("r3a");

        Core.RegisterQuests(8814, 8815);



        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");
        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();

        Core.ToBank("Prismatic Seams");
    }

    public void ShadowBattleon()
    {
        RequiredQuest("shadowbattleon", 9426);

        Core.EquipClass(ClassType.Farm);
        Core.AddDrop("Wisper");
        Core.RegisterQuests(9421, 9422, 9426);

        if (Bot.Config!.Get<Method>("mapname") == Method.ShadowBattleonBabyMode)
            Core.RegisterQuests(9421, 9422, 9423);
        else
            Core.RegisterQuests(9421, 9422, 9426);

        Core.Logger($"Mode Selected: {Bot.Config!.Get<Method>("mapname")}");

        if (Bot.Config!.Get<Method>("mapname") == Method.ShadowBattleonHighLevels)
        {
            Army.AggroMonCells("r11", "r12");
            Army.AggroMonStart("shadowbattleon");
            Army.DivideOnCells("r11", "r12");
        }
        if (Bot.Config!.Get<Method>("mapname") == Method.ShadowBattleonLowerLevels)
        {
            Army.AggroMonCells("r11");
            Army.AggroMonStart("shadowbattleon");
            Army.DivideOnCells("r11");
        }
        else if (Bot.Config!.Get<Method>("mapname") == Method.ShadowBattleonBabyMode)
        {
            Army.AggroMonCells("Enter");
            Army.AggroMonStart("shadowbattleon");
            Army.DivideOnCells("Enter");
        }

        Core.Logger("This method is insane atm.. if the rate is ever complete sh*t please use SCW");



        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");

        Bot.Options.AttackWithoutTarget = false;
        Army.AggroMonStop(true);
        Core.CancelRegisteredQuests();
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.JumpWait();
    }

    public void HakuWar()
    {
        Quest Quest = Core.EnsureLoad(9601);
        if (Quest.XP < 6000)
        {
            Core.Logger("XP rates have been nerfed, swapping to method: SCW (its better)");
            SCW();
        }
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        CoreDOY.DoAll();
        Core.RegisterQuests(9601, 9602, 9603, 9605, 9606);

        Core.EquipClass(ClassType.Farm);
        Army.AggroMonMIDs(4, 5, 6, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 25, 26, 27);
        // Army.AggroMonCells("r2", "r4", "r5", "r6", "r7", "r9");
        Army.AggroMonStart("hakuwar");
        Army.DivideOnCells("r2", "r4", "r5", "r6", "r7", "r9");



        while (!Bot.ShouldExit && Bot.Player.Gold < 999999999)
            Bot.Combat.Attack("*");

        Army.AggroMonStop(true);
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    void RequiredQuest(string map, int Quest)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(Quest));
        if (QuestData == null)
        {
            Core.Logger($"Failed to load quest {Quest}.");
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

            case "hakuwar":
                CoreDOY.DoAll();
                break;

            case "Default":
                //Example Case
                break;
        }
        Core.JumpWait();
    }

    void PirateBloodWar()
    {
        Quest? WarQuest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(9873));

        if (WarQuest != null)
        {
            if (WarQuest.Gold < 6000 || !Core.isSeasonalMapActive("piratebloodhub"))
                SCW();
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

    }

    public enum Method
    {
        BattleGroundE = 0,
        DarkWarLegion = 1,
        DarkWarNation = 2,
        SevenCirclesWar = 3,
        StreamWar = 4,
        ShadowBattleonBabyMode = 5,
        ShadowBattleonLowerLevels = 6,
        ShadowBattleonHighLevels = 7,
        HakuWar = 8,
        PirateBloodWar = 9
    }

}
