/*
name: Army Martial Artist
description: Uses your army to get the Martial Artist class
tags: army, martial artist, martial, artist
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class ArmyMartialArtist
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public string OptionsStorage = "ArmyMartialArtist";
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
        Core.BankingBlackList.AddRange(Loot);
        Core.SetOptions();

        Core.Logger("~\"All\"~ Army Scripts have been disabled by the author.", "**READ ME!!**", stopBot: true);

        Core.SetOptions(false);
    }

    public void Setup()
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Farm.Experience(65);
        Core.AddDrop(
           "Dreadhaven General's Soul Fragment",
           "Zakhvatchik's Soul Fragment",
           "Creel's Soul Fragment",
           "Frogzard Defeated",
           "Gorillaphant Defeated",
           "Dragon Defeated"
        );

        Story.ChainQuest(9922);

        // 9923 | 500 Punches and 500 Kicks
        if (!Story.QuestProgression(9923))
        {
            Core.Logger("Quest is required, we'll stack mats via \"Deathly Slow Start [9933]\" After");
            Core.EnsureAccept(9923);
            Core.EquipClass(ClassType.Farm);

            #region Frogzard Defeated
            Army.WaitForParty("nexus", "Enter");
            Army.AggroMonMIDs(1, 2, 3);
            Army.AggroMonStart("nexus");
            Army.DivideOnCells("Enter");
            Bot.Player.SetSpawnPoint();

            while (!Bot.ShouldExit && !Core.CheckInventory("Frogzard Defeated", 500))
            {
                Bot.Combat.Attack("*");
                Bot.Sleep(200);
            }
            Army.AggroMonStop(true);
            Bot.Wait.ForPickup("Frogzard Defeated");
            #endregion Frogzard Defeated

            #region Gorillaphant Defeated
            Army.WaitForParty("arcangrove", "Enter");
            Army.AggroMonMIDs(2, 5, 8, 9, 10);
            Army.AggroMonStart("arcangrove");
            Army.DivideOnCells("Left", "Back", "LeftBack");
            Bot.Player.SetSpawnPoint();

            while (!Bot.ShouldExit && !Core.CheckInventory("Gorillaphant Defeated", 500))
            {
                Bot.Combat.Attack("Gorillaphant");
                Bot.Sleep(200);
            }
            Army.AggroMonStop(true);
            Bot.Wait.ForPickup("Gorillaphant Defeated");

            #endregion Gorillaphant Defeated

            #region Dragon Defeated
            Army.WaitForParty("etherwardes", "Enter");
            Army.AggroMonMIDs(Core.FromTo(1, 18));
            Army.AggroMonStart("etherwardes");
            Army.DivideOnCells("Enter", "r2", "r3", "r4", "r5", "r6");
            Bot.Player.SetSpawnPoint();

            while (!Bot.ShouldExit && !Core.CheckInventory("Dragon Defeated", 500))
            {
                Bot.Combat.Attack("*");
                Bot.Sleep(200);
            }
            Bot.Wait.ForPickup("Dragon Defeated");
            Army.AggroMonStop(true);

            #endregion Dragon Defeated

            Core.EnsureComplete(9923);
        }
        // go back and help if needed
        Army.WaitForParty("party", "Enter");

        // Generate the list of quest IDs
        int[]? questIDs = Core.FromTo(9922, 9927).Append(Core.IsMember ? 9911 : 9902)?.Where(q => q > 0).ToArray();

        // Ensure the quest IDs array is not null before passing it to EnsureLoad
        List<Quest> quests = Core.EnsureLoad(questIDs ?? Array.Empty<int>());

        Core.EnsureAccept(Core.IsMember ? 9911 : 9902);
        Core.EquipClass(ClassType.Solo);

        #region Dreadhaven General's Soul Fragment

        Army.WaitForParty("dreadfight", "Enter");
        Army.AggroMonMIDs(1);
        Army.AggroMonStart("dreadfight");
        Army.DivideOnCells("Enter");
        Bot.Player.SetSpawnPoint();

        while (!Bot.ShouldExit && !Core.CheckInventory("Dreadhaven General's Soul Fragment", Core.IsMember ? 200 : 400))
        {
            Bot.Combat.Attack("Dreadhaven General");
            Bot.Sleep(200);
        }
        Army.AggroMonStop(true);
        #endregion


        #region Zakhvatchik's Soul Fragment
        Bot.Quests.UpdateQuest(9607);
        Army.WaitForParty("hakuwar", "r10");
        Army.AggroMonMIDs(28);
        Army.AggroMonStart("hakuwar");
        Army.DivideOnCells("r10");
        Bot.Player.SetSpawnPoint();

        while (!Bot.ShouldExit && !Core.CheckInventory("Zakhvatchik's Soul Fragment", Core.IsMember ? 200 : 400))
        {
            Bot.Combat.Attack("Zakhvatchik");
            Bot.Sleep(200);
        }
        Army.AggroMonStop(true);
        #endregion


        #region Creel's Soul Fragment
        Army.WaitForParty("towerofdoom5", "r10");
        Army.AggroMonMIDs(28);
        Army.AggroMonStart("towerofdoom5");
        Army.DivideOnCells("r10");
        Bot.Player.SetSpawnPoint();
        while (!Bot.ShouldExit && !Core.CheckInventory("Creel's Soul Fragment", Core.IsMember ? 200 : 400))
        {
            Bot.Combat.Attack("Creel");
            Bot.Sleep(200);
        }
        Army.AggroMonStop(true);
        #endregion

        // Wait & butler back if needed.
        Army.WaitForParty("whitemap", "Enter");

        Core.EnsureCompleteMulti(9933);
        Bot.Wait.ForPickup("*");
        foreach (int Q in Core.FromTo(9923, 9927))
        {
            // 9933 | Deathly Slow Start
            // 9924 | Discount Diploma
            // 9925 | One Million Miles Searching
            // 9926 | Ughhhhhh
            // 9927 | Work Smarter, Not Harder
            Story.ChainQuest(Q);
        }

        // Ensure quest is complete before buying
        if (!Story.QuestProgression(9928))
        {
            Army.WaitForParty("hakuvillage", "r2a");
            Core.EnsureAccept(9928);
            Army.AggroMonMIDs(10);
            Army.AggroMonStart("hakuvillage");
            Army.DivideOnCells("r2a");
            Bot.Player.SetSpawnPoint();

            while (!Bot.ShouldExit && !Bot.TempInv.Contains("The Master Fought"))
            {
                Bot.Combat.Attack("The Master");
                Bot.Sleep(200);
            }
            Bot.Wait.ForPickup("The Master Fought");
            Core.EnsureComplete(9928);
            Bot.Wait.ForQuestComplete(9928);
        }
        Army.WaitForParty("party", "Enter");

        Core.BuyItem("hakuvillage", 2490, "Martial Artist");
        Bot.Wait.ForPickup("Martial Artist");

        Adv.RankUpClass("Martial Artist");

    }

    private string[] Loot = { "Bone Dust", "Undead Energy" };
}
