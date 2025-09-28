/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class CoreLegion
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private CoreFarms _Farm;

    private CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private CoreStory _Story;

    private CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private CoreAdvanced _Adv;


    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    public string[] legionMedals =
    {
        "Legion Round 1 Medal",
        "Legion Round 2 Medal",
        "Legion Round 3 Medal",
        "Legion Round 4 Medal"
    };

    public void EmblemofDage(int quant = 500)
    {
        if (Core.CheckInventory("Emblem of Dage", quant))
            return;

        if (!Core.CheckInventory("Legion Round 4 Medal"))
            LegionRound4Medal();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.gold);

        Core.FarmingLogger("Emblem of Dage", quant);
        Core.AddDrop("Legion Seal", "Gem of Mastery", "Emblem of Dage");
        Core.RegisterQuests(4742);
        while (!Bot.ShouldExit && !Core.CheckInventory("Emblem of Dage", quant))
        {
            Core.HuntMonster("shadowblast", "Carnage", "Gem of Mastery", 1, false, false);
            Core.KillMonster("shadowblast", "r10", "Left", "*", "Legion Seal", 25, false);
            Bot.Wait.ForPickup("Emblem of Dage");
        }
        Core.CancelRegisteredQuests();
    }

    public void DarkToken(int quant = 10000)
    {
        if (Core.CheckInventory(43266, quant))
            return;

        Core.EquipClass(ClassType.Farm);
        Core.AddDrop(43266);
        Core.FarmingLogger("Dark Token", 10);
        Core.RegisterQuests(6248, 6249, 6251);
        while (!Bot.ShouldExit && !Core.CheckInventory(43266, quant))
        {
            if (Bot.Map.Name != "seraphicwardage")
                Core.Join("seraphicwardage");

            if (Bot.Player.Cell != "r3")
                Core.Jump("r3", "Left");

            List<Monster> M = Bot.Monsters.CurrentAvailableMonsters
                .FindAll(x => x != null && x.Cell == Bot.Player.Cell);

            bool ded = false;
            Bot.Events.MonsterKilled += b => ded = true;
            M.ForEach(monster =>
            {
                ded = false;
                while (!Bot.ShouldExit && monster != null && !ded)
                {
                    Bot.Options.HidePlayers = Core.AggroMonsters();
                    Bot.Options.AggroMonsters = Core.AggroMonsters();

                    Bot.Combat.Attack(monster); // Attack the specific monster
                    Bot.Sleep(500); // Wait after attacking}
                    // Check Inventory for item - quant
                    if (Bot.Inventory.TryGetItem(43266, out InventoryItem? DarkTokenInvItem)
                     && DarkTokenInvItem != null && DarkTokenInvItem.Quantity >= quant

                    // Check Bank for item - quant
                    || Bot.Bank.TryGetItem(43266, out InventoryItem? DarkTokenBankItem)
                     && DarkTokenBankItem != null && DarkTokenBankItem.Quantity >= quant)
                    {
                        Core.JumpWait();
                        break;
                    }

                }
            });
            Bot.Events.MonsterKilled -= b => ded = true;
        }
        Core.CancelRegisteredQuests();

        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;

        // Filter out blacklisted cells, cells with monsters, and prioritize based on conditions
        string? targetCell = Bot.Map.Cells
            .Where(c => c != null &&
                        !Core.BlackListedJumptoCells.Contains(c) &&
                        !Bot.Monsters.MapMonsters.Any(monster => monster != null && monster.Cell == c))
            .FirstOrDefault(c => c != null &&
                                 (Bot.Map.Cells.Count(cell => cell.Contains("Enter")) > 1 || !c.Contains("Enter")))
            ?? "Enter";

        Bot.Map.Jump(targetCell, targetCell == "Enter" ? "Spawn" : "Left");
        Bot.Wait.ForCellChange(targetCell);
        Core.Sleep();
        Core.JumpWait();
        Core.Rest();
        Bot.Options.HidePlayers = false;
    }

    public void DiamondTokenofDage(int quant = 300)
    {
        if (Core.CheckInventory("Diamond Token of Dage", quant))
            return;

        if (!Core.CheckInventory("Legion Round 4 Medal"))
            LegionRound4Medal();
        if (!Core.CheckInventory("Legion Token", 50))
            FarmLegionToken(50);

        Core.FarmingLogger("Diamond Token of Dage", quant);
        Core.AddDrop("Diamond Token of Dage", "Legion Token");
        Core.RegisterQuests(4743);
        while (!Bot.ShouldExit && !Core.CheckInventory("Diamond Token of Dage", quant))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("tercessuinotlim", "m2", "Left", "*", "Defeated Makai", 25, false);

            Core.EquipClass(ClassType.Solo);
            //Adv.BestGear(RacialGearBoost.Chaos);
            Core.KillMonster("aqlesson", "Frame9", "Right", "Carnax", "Carnax Eye", publicRoom: true);
            Core.HuntMonster("deepchaos", "Kathool", "Kathool Tentacle", publicRoom: true);

            //More then one item of the same name as drop btoh temp and non-temp.
            while (!Bot.ShouldExit && !Core.CheckInventory(33257))
                Core.KillMonster("dflesson", "r12", "Right", "Fluffy the Dracolich", publicRoom: true);

            //Adv.BestGear(RacialGearBoost.Dragonkin);
            Core.HuntMonster("lair", "Red Dragon", "Red Dragon's Fang");
            //Adv.BestGear(RacialGearBoost.Human);
            Core.HuntMonster("bloodtitan", "Blood Titan", "Blood Titan's Blade", publicRoom: true);
            foreach (string drop in new[] { "Legion Token", "Diamond Token of Dage" })
                Bot.Wait.ForPickup(drop);
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farms Legion Round 4 Medal in Shadow Blast Arena
    /// </summary>
    public void LegionRound4Medal()
    {
        if (Core.CheckInventory("Legion Round 4 Medal"))
            return;

        Core.AddDrop(legionMedals);
        Core.Logger("Farming Legion Round 4 Medal");
        Core.Join("shadowblast");
        //Adv.BestGear(GenericGearBoost.dmgAll);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Round 4 Medal"))
        {
            if (!Core.CheckInventory("Legion Round 1 Medal") &&
                !Core.CheckInventory("Legion Round 2 Medal") &&
                !Core.CheckInventory("Legion Round 3 Medal"))
            {
                Core.EnsureAccept(4738);
                Core.HuntMonster("shadowblast", "Caesaristhedark", "Nation Rookie Defeated", 5, true);
                Core.HuntMonster("shadowblast", "Shadowrise Guard", "Shadowscythe Rookie Defeated", 5, true);
                Core.EnsureComplete(4738);
                Bot.Wait.ForDrop("Legion Round 1 Medal");
                Core.Logger("Medal 1 acquired");
            }

            if (Core.CheckInventory("Legion Round 1 Medal"))
            {
                Core.EnsureAccept(4739);
                Core.HuntMonster("shadowblast", "Carnage", "Nation Veteran Defeated", 7, true);
                Core.HuntMonster("shadowblast", "Doombringer", "Shadowscythe Veteran Defeated", 7, true);
                Core.EnsureComplete(4739);
                Bot.Wait.ForDrop("Legion Round 2 Medal");
                Core.Logger("Medal 2 acquired");
            }

            if (Core.CheckInventory("Legion Round 2 Medal"))
            {
                Core.EnsureAccept(4740);
                Core.HuntMonster("shadowblast", "Minotaurofwar", "Nation Elite Defeated", 10, true);
                Core.HuntMonster("shadowblast", "Draconic Doomknight", "Shadowscythe Elite Defeated", 10, true);
                Core.EnsureComplete(4740);
                Bot.Wait.ForDrop("Legion Round 3 Medal");
                Core.Logger("Medal 3 acquired");
            }

            if (Core.CheckInventory("Legion Round 3 Medal"))
            {
                Core.EnsureAccept(4741);
                Core.HuntMonster("shadowblast", "Thanatos", "Thanatos Vanquished", 1, true);
                Core.EnsureComplete(4741);
                Bot.Wait.ForDrop("Legion Round 4 Medal");
                Core.Logger("Medal 4 acquired");
            }
        }
    }

    public void ApprovalAndFavor(int quantApproval = 5000, int quantFavor = 5000)
    {
        if (Core.CheckInventory("Dage's Approval", quantApproval) && Core.CheckInventory("Dage's Favor", quantFavor))
            return;

        Core.AddDrop("Dage's Approval", "Dage's Favor");

        Core.EquipClass(ClassType.Farm);

        // Use Dictionary to hold item names and their quantities
        var items = new Dictionary<string, int>
        {
            { "Dage's Approval", quantApproval },
            { "Dage's Favor", quantFavor }
        };

        foreach (var item in items)
            Core.KillMonster("underworld", "r16", "Left", "*", item.Key, item.Value, isTemp: false);
    }


    public void BoneSigil(int quant = 1)
    {
        if (Core.CheckInventory("Bone Sigil", quant))
            return;

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(RacialGearBoost.Undead);

        Core.FarmingLogger("Bone Sigil", quant);
        Core.AddDrop("Bone Sigil");
        Core.RegisterQuests(6739);
        while (!Bot.ShouldExit && !Core.CheckInventory("Bone Sigil", quant))
        {
            Core.HuntMonster("legionarena", "Legion Gladiator", "Legion Grunt Defeated", 5);
            Bot.Wait.ForPickup("Bone Sigil");
        }
        Core.CancelRegisteredQuests();
    }

    public void SoulForgeHammer()
    {
        if (Core.CheckInventory("SoulForge Hammer"))
            return;

        Core.EquipClass(ClassType.Solo);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.AddDrop("SoulForge Hammer");
        Core.EnsureAccept(2741);
        Core.HuntMonster("forest", "Zardman Grunt", "Zardman's StoneHammer", isTemp: false);
        if (Core.CheckInventory(319))
            Core.BuyItem("swordhaven", 179, "Iron Hammer");
        else Core.HuntMonster("battleundera", "Skeletal Warrior", "Iron Hammer", isTemp: false);
        Core.HuntMonster("bludrut", "Rock Elemental", "Elemental Rock Hammer", isTemp: false);
        Core.EnsureComplete(2741);
        Bot.Wait.ForPickup("SoulForge Hammer");
    }

    #region LegionTokens
    public void FarmLegionToken(int quant = 50000)
    {
        //banking Lts as ae fucked the quant when updating teh cap
        // if (!Bot.Bank.Contains("Legion Token"))
        // {
        //     Core.OneTimeMessage("Legion Token stack fix", "Banking [then unbanking if farm is needed] LTs\n" +
        //                         "as when AE updated the cap, they broke shit", messageBox: false);

        //     Core.ToBank("Legion Token");
        // }

        if (Core.CheckInventory("Legion Token", quant))
            return;

        JoinLegion();

        LTBrightParagon(quant);
        LTArcaneParagon(quant);
        LTShogunParagon(quant);
        LTFestiveParagonDracolichRider(quant);
        LTParagon(quant);
        LTMountedParagonPet(quant);
        LTThanatosParagon(quant);
        LTAscendedParagon(quant);
        LTDreadnaughtParagon(quant);
        LTHolidayParagon(quant);
        LTHardCoreParagon(quant);
        LTUW3017(quant);
        LTInfernalLegionBetrayal(quant);
        LTFirstClassEntertainment(quant, true, 4, true);
        LTDreadrock(quant);
    }

    public void LTHardCoreParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Hardcore Paragon Pet"))
            return;
        Core.BankingBlackList.Add("Legion Token");
        Core.EquipClass(ClassType.Solo);
        //Adv.BestGear(RacialGearBoost.Chaos);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop(Core.QuestRewards(3393, 3394));

        if (!Bot.Quests.IsDailyComplete(3394))
        {
            Core.EnsureAccept(3394);
            Core.HuntMonster("chaosboss", "Ultra Chaos Warlord", "Chaorrupted Dark Fire", 20, isTemp: false);
            Core.EnsureComplete(3394);
        }

        // A Single Rib
        Core.RegisterQuests(3393);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Core.HuntMonster("doomvault", "Binky", "Dark Unicorn Rib", isTemp: false);
        Core.CancelRegisteredQuests();
        Core.ToBank(Core.QuestRewards(3393, 3394));

    }

    public void LTInfernalLegionBetrayal(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Infernal Caladbolg"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");

        Core.RegisterQuests(Core.CheckInventory("Shogun Paragon Pet") ? new[] { 3722, 5755 } : new[] { 3722 });
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("fotia", "Fotia Elemental", "Betrayer Extinguished", 5);
            Core.HuntMonster("evilwardage", "Dreadfiend of Nulgath", "Fiend Felled", 2);
        }
        Core.CancelRegisteredQuests();
    }

    public void LTUW3017(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("UW3017 Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(5738);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("underworld", "Bloodfiend", "Foreign Weapon", 20);
            Core.HuntMonster("underworld", "Bloodfiend", "Foreign Equipment", 20);
            Core.HuntMonster("underworld", "Bloodfiend", "Unknown Substance", 20);
        }
        Core.CancelRegisteredQuests();
    }

    public void LTHolidayParagon(int quant = 50000)
    {

        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Holiday Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(3256);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("prison", "King Alteon's Knight", "Spirit of Loyalty", 6);
            Core.HuntMonster("battlewedding", "Silver Knight", "Spirit of Love", 6);
            Core.HuntMonster("lycan", "Lycan Knight", "Spirit of Good Will", 6);
        }
        Core.CancelRegisteredQuests();
    }

    public void LTFestiveParagonDracolichRider(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Festive Paragon Dracolich Rider"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(3968);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.KillMonster("frozentower", "Enter", "Spawn", "*");
            if (Core.CheckInventory("Legion Token", quant))
                break;
        }
        Bot.Wait.ForPickup("Legion Token");
        Core.CancelRegisteredQuests();
    }

    public void LTBrightParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Bright Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token", "Legion Token Pile");
        Core.RegisterQuests(4704, 4703);
        Core.ConfigureAggro();
        Core.KillMonster("brightfortress", "r3", "Right", "*", "Legion Token", quant, isTemp: false);
        Core.ConfigureAggro(false);
        Core.CancelRegisteredQuests();
    }

    public void LTShogunParagon(int quant = 50000, bool DoClearaPath = false, bool Logger = false)
    {
        if (Core.CheckInventory("Legion Token", quant))
        {
            Core.FarmingLogger("Legion Token", quant);
            return;
        }

        // Pairs of quest IDs with their respective accept requirements
        (int, int)[] questPairs = new[]
        {
            // Quest 1: time for spring cleaning
            // Quest 2: Clear a Path
            (9649, 9648), // Hollowborn Paragon Quest Pet [84900]
            (9646, 9645), // Hollowborn Paragon Quest Pet [84899]
            (9663, 9662), // Hollowborn Paragon Quest Pet [84965]
            (7073, 7072), // Paragon Ringbearer [49926]
            (6750, 6749), // Paragon Fiend Quest Pet [47578]
            (6756, 6754), // Paragon Fiend Quest Pet [47614]
            (5756, 5754), // Shogun Dage Pet [38792]
            (5755, 5753)  // Shogun Paragon Pet [38621]
        };

        List<Quest> QuestList = new(); // List to store quest IDs
        List<(ItemBase, int)> QuestItems = new(); // List to store quest items
        bool HasQuestPet = false; // Tracks if the player has the required pet

        // Process quest pairs
        foreach ((int firstQuestID, int secondQuestID) in questPairs)
        {
            // Load and process the first quest in the pair
            Quest? MainQuestID = Core.EnsureLoad(firstQuestID);
            Quest? SideQuest = Core.EnsureLoad(secondQuestID);
            ItemBase? PetToAcceptQuest = MainQuestID.AcceptRequirements.FirstOrDefault();
            if (PetToAcceptQuest != null && Core.CheckInventory(PetToAcceptQuest.ID))
            {
                QuestList.Add(MainQuestID);

                // Add first quest's rewards
                Core.AddDrop(MainQuestID.Rewards.Where(x => x != null && x.Quantity < x.MaxStack).Select(item => item.Name).Distinct().ToArray());

                // If DoClearaPath is true, also add the second quest's rewards
                if (DoClearaPath)
                {
                    QuestList.Add(SideQuest);

                    Core.AddDrop(SideQuest.Rewards.Where(x => x != null && x.Quantity < x.MaxStack).Select(item => item.Name).Distinct().ToArray());
                }

                HasQuestPet = true;
                Bot.Log($"✔️ Pet: {PetToAcceptQuest.Name}\n PetID: [{PetToAcceptQuest.ID}]\n PetQuestID(s): [{string.Join(", ", QuestList)}]");
            }
            if (HasQuestPet) break;
        }

        // Exit if no quest pet is owned
        if (!HasQuestPet)
        {
            Core.Logger("No Pet owned for \"Time for Some Spring Cleaning\" *or* the Pet is missing from our list");
            return;
        }

        // Collect requirements for each quest in the list
        foreach (Quest Q in QuestList)
        {
            Quest quest = Core.EnsureLoad(Q.ID);

            foreach (ItemBase requirement in quest.Requirements)
            {
                ItemBase? questItem = null;
                for (int i = 0; i < 5; i++)
                {
                    questItem = quest.Requirements.FirstOrDefault(i => i != null && i.ID == requirement.ID);
                    if (questItem != null)
                        break;
                    Core.Logger($"Attempt {i + 1}: Quest item with ID {requirement.ID} not found. Retrying...");
                    Core.Sleep(1000); // Wait for 1 second before retrying
                }

                if (questItem == null)
                {
                    Core.Logger($"Quest item with ID {requirement.ID} not found after 5 attempts.");
                    continue;
                }

                QuestItems.Add((questItem, questItem.Quantity));
            }
        }

        // Equip class, log farming, add drop, and register quests
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(QuestList.Where(q => q != null).Select(Q => Q.ID).ToArray());
        // Hunt monsters until the desired quantity of Legion Tokens is obtained
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            foreach ((ItemBase QuestItem, int ItemQuant) in QuestItems)
            {
                if (Bot.TempInv.Contains(QuestItem.Name, ItemQuant))
                    continue;

                Core.FarmingLogger(QuestItem.Name, ItemQuant);

                Core.KillMonster("fotia",
                    // Set cell:
                    QuestItem.Name == "Femme Cult Worshipper's Soul" ? "r5" : "Enter",
                    // Set Pad:
                    QuestItem.Name == "Femme Cult Worshipper's Soul" ? "Left" : "Spawn",
                    // Set Mob:
                    QuestItem.Name == "Femme Cult Worshipper's Soul" ? "Femme Cult Worshiper" : "*",
                    // Set ItemName:
                    QuestItem.Name,
                    // Set ItemName Quant:
                    ItemQuant,
                     log: Logger);

                if (Core.CheckInventory("Legion Token", quant))
                {
                    Core.Logger("Legion Tokens maxed!");
                    break;
                }
            }
        }
        Core.CancelRegisteredQuests();
    }


    public void LTParagon(int quant = 50000) // Paragon Pet
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory(11260))
            return;

        JoinLegion();

        Core.FarmingLogger("Legion Token", quant);
        Core.RegisterQuests(1703);
        Core.AddDrop("Legion Token");
        Core.AddDrop(300, 11189, 11190);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Core.KillMonster("lair", "End", "Right", "Red Dragon");
        Core.CancelRegisteredQuests();
        Core.ToBank(11189, 11190);
    }


    public void LTFirstClassEntertainment(int quant = 50000, bool onlyWithParty = false, int partySize = 4, bool ReturnIfNoPeople = false)
    {
        if (Core.CheckInventory("Legion Token", quant))
            return;

        JoinLegion();

        Core.Join("legionarena", publicRoom: true);
        if (Bot.Map.PlayerCount < partySize && onlyWithParty)
        {
            Core.Join("legionarena", ignoreCheck: true, publicRoom: true);
            if (ReturnIfNoPeople && Bot.Map.PlayerCount < partySize)
                return;
            while (!Bot.ShouldExit && Bot.Map.PlayerCount < partySize) { }
            Core.Logger($"Party gathered [{Bot.Map.PlayerNames!.Count}/{partySize}]");
        }

        Core.EquipClass(ClassType.Solo);
        //Adv.BestGear(RacialGearBoost.Undead);

        Core.FarmingLogger("Legion Token", quant);
        Core.RegisterQuests(6742, 6743);
        Core.AddDrop("Legion Token", "Bone Sigil");
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Core.KillMonster("legionarena", "Boss", "Left", "Legion Fiend Rider");
        Core.CancelRegisteredQuests();
        Core.ToBank("Bone Sigil");
    }

    public void LTDreadrock(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant))
            return;

        JoinLegion();
        Adv.BuyItem("underworld", 216, "Undead Champion");

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(RacialGearBoost.Human);

        Core.FarmingLogger("Legion Token", quant);
        Core.RegisterQuests(4849);
        Core.AddDrop("Legion Token");
        Core.ConfigureAggro();
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Core.KillMonster("dreadrock", "r3", "Bottom", "*", "Dreadrock Enemy Recruited", 6);
        Core.ConfigureAggro(false);
        Core.CancelRegisteredQuests();
    }

    public void LTArcaneParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Arcane Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(RacialGearBoost.Dragonkin);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token", "Granite Dracolich Soul", "Tempest Dracolich Soul", "Deluge Dracolich Soul", "Inferno Dracolich Soul");
        Core.RegisterQuests(4896);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("dragonheart", "Granite Dracolich", "Granite Dracolich Soul", 4, isTemp: false);
            Core.HuntMonster("dragonheart", "Tempest Dracolich", "Tempest Dracolich Soul", 4, isTemp: false);
            Core.HuntMonster("dragonheart", "Inferno Dracolich", "Inferno Dracolich Soul", 4, isTemp: false);
            Core.HuntMonster("dragonheart", "Deluge Dracolich", "Deluge Dracolich Soul", 4, isTemp: false);
        }
        Core.CancelRegisteredQuests();
    }

    public void LTThanatosParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Thanatos Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(RacialGearBoost.Undead);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(4100);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Core.KillMonster("dragonheart", "r6", "Right", "Zombie Dragon", "Elemental Dragon Soul", 20);
        Core.CancelRegisteredQuests();
    }

    public void LTDreadnaughtParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Paragon Dreadnaught Pet"))
            return;

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(5740, 5741);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("laken", "Augmented Guard", "Stolen Guard", 5);
            Core.HuntMonster("laken", "Cyborg Dog", "Stolen Dog", 6);
            Core.HuntMonster("laken", "Mad Scientist", "Taken Axe", 10);
        }
        Core.CancelRegisteredQuests();
    }

    public void LTMountedParagonPet(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Mounted Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(5604);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("frozentower", "Ice Wolf", "Giant Coal Lump", 10);
            Core.HuntMonster("frozentower", "Ice Wolf", "Small Coal Lump", 8);
        }
        Core.CancelRegisteredQuests();

    }

    public void LTAscendedParagon(int quant = 50000)
    {
        if (Core.CheckInventory("Legion Token", quant) || !Core.CheckInventory("Ascended Paragon Pet"))
            return;

        JoinLegion();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.RegisterQuests(2747);
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
        {
            Core.HuntMonster("tournament", "Lord Brentan", "Lord Brentan's Regal Blade");
            Core.HuntMonster("tournament", "Roderick", "Roderick's Chaotic Bane");
            Core.HuntMonster("tournament", "Knight of Thorns", "Knight of Thorns' Sword");
            Core.HuntMonster("tournament", "Johann Wryce", "Platinum of Johann Wryce");
            Core.HuntMonster("tournament", "Khai Khaldun", "Khai Khaldun's Scimitar");
        }
        Core.CancelRegisteredQuests();
    }

    #endregion

    public void JoinLegion()
    {
        if (Core.isCompletedBefore(793))
            return;

        var SellUW = Bot.ShowMessageBox(
                    "Do you want the bot to sell the \"Undead Warrior\" armor after it has succesfully joined the legion. This will return 1080 AC to you",
                    "Sell \"Undead Warrior\"?",
                    true);

        if (!Core.isCompletedBefore(792))
            Farm.BludrutBrawlBoss(quant: 200);
            
        if (!Core.CheckInventory("Undead Warrior"))
            Core.BuyItem("underworld", 215, "Undead Warrior");

        // Undead Champion Initiation
        if (!Story.QuestProgression(789))
        {
            Core.EnsureAccept(789);
            Core.HuntMonster("greenguardwest", "Black Knight", "Black Knight's Eternal Contract");
            Core.EnsureComplete(789);
        }

        // Mourn the Soldiers
        if (!Story.QuestProgression(790))
        {
            Core.EnsureAccept(790);
            Core.HuntMonster("dwarfhold", "Chaos Drow", "Chaos Drow slain");
            Core.HuntMonster("swordhavenundead", "Skeletal Soldier", "Skeletal Soldier slain");
            Core.HuntMonster("pirates", "Fishman Soldier", "Fishman Soldier slain");
            Core.HuntMonster("willowcreek", "Dwakel Soldier", "Dwakel Soldier slain");
            Core.EnsureComplete(790);
        }

        // Understanding Undead Champions
        if (!Story.QuestProgression(791))
        {
            Core.EnsureAccept(791);
            Core.HuntMonster("battleunderb", "Undead Champion", "Ravaged Champion Soul", 80, isTemp: false);
            Core.EnsureComplete(791);
        }

        // Player vs Power
        Story.ChainQuest(792);

        // Fail to the King
        Story.KillQuest(793, "prison", "King Alteon's Knight");

        if (SellUW == true)
            Core.SellItem("Undead Warrior", all: true);

        Adv.BuyItem("underworld", 216, "Undead Champion");
    }

    public void ObsidianRock(int quant = 666)
    {
        if (Core.CheckInventory("Obsidian Rock", quant))
            return;

        SoulForgeHammer();

        Core.EquipClass(ClassType.Farm);
        //Adv.BestGear(GenericGearBoost.dmgAll);

        Core.FarmingLogger("Obsidian Rock", quant);
        Core.AddDrop("Obsidian Rock");
        if (!Core.IsMember)
        {
            Core.Logger("Using Non-Member Method");
            Bot.Quests.UpdateQuest(1542);
        }
        else Core.Logger("Using Members Method");

        Core.RegisterQuests(2742);
        while (!Bot.ShouldExit && !Core.CheckInventory("Obsidian Rock", quant))
        {
            if (Core.IsMember)
                Core.HuntMonster("hydra", "Fire Imp", "Obsidian Deposit", 10);
            else Core.KillMonster("firestorm", "r8", "Left", "Firestorm Hatchling", "Obsidian Deposit", 10);

            Bot.Wait.ForPickup("Obsidian Rock");
        }
        Core.CancelRegisteredQuests();
    }

    public void DagePvP(int trophyQuant = 4000, int techniqueQuant = 1000, int scrollQuant = 1000, bool canSoloBoss = true, bool enableDebug = false)
    {

        if (Core.CheckInventory("Legion Combat Trophy", trophyQuant) &&
            Core.CheckInventory("Technique Observed", techniqueQuant) &&
            Core.CheckInventory("Sword Scroll Fragment", scrollQuant))
            return;

        canSoloBoss = Core.CBOBool("PvP_SoloPvPBoss", out bool _canSoloBoss);

        Core.AddDrop("Legion Combat Trophy", "Technique Observed", "Sword Scroll Fragment");
        Core.EquipClass(ClassType.Solo);


        int exitAttempt = 0;
        bool FarmComplete = false;

        if (enableDebug)
            Core.DL_Enable();

        Start:
        exitAttempt = 0;
        while (!Bot.ShouldExit && !FarmComplete)
        {
            LogFarmingProgress();

            Core.Join("dagepvp-999999", "Enter0", "Spawn");

            Core.PvPMove(1, "r2", 475, 269);
            Core.PvPMove(4, "r4", 963, 351);
            Core.PvPMove(7, "r5", 843, 189);
            Core.PvPMove(9, "r6", 937, 389);

            //if scroll quant > 0, farm scroll, then return to r6 to cotniue
            if (scrollQuant > 0)
                FarmScrollArea();

            if ((trophyQuant == 0 || Core.CheckInventory("Legion Combat Trophy", trophyQuant))
            && (techniqueQuant == 0 || Core.CheckInventory("Technique Observed", techniqueQuant)))
            {
                Exit("Enter0", exitAttempt: ref exitAttempt);
                goto Start;
            }

            Core.PvPMove(12, "r12", 758, 338);

            if (!canSoloBoss)
                Core.PVPKilling();

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
                goto Start;
            }
            Core.PvPMove(23, "r13", 933, 394);
            if (!canSoloBoss)
                Core.PVPKilling();

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
                goto Start;
            }
            Core.PvPMove(25, "r14", 846, 181);

            if (!canSoloBoss)
                Core.PVPKilling();

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
                goto Start;
            }
            Core.PvPMove(28, "r15", 941, 348);

            // Boss kill... GL without the debuff
            Bot.Kill.Monster(27);

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
                goto Start;
            }

            Bot.Wait.ForDrop("Legion Combat Trophy", 40);
            Bot.Wait.ForPickup("Legion Combat Trophy");
            LogFarmingProgress();
            Exit("Enter0", exitAttempt: ref exitAttempt);
            FarmComplete = CheckInventoryCompletion();
        }

        void LogFarmingProgress()
        {
            if (trophyQuant > 0)
                Core.FarmingLogger("Legion Combat Trophy", trophyQuant);
            if (techniqueQuant > 0)
                Core.FarmingLogger("Technique Observed", techniqueQuant);
            if (scrollQuant > 0)
                Core.FarmingLogger("Sword Scroll Fragment", scrollQuant);
        }

        bool CheckInventoryCompletion()
        {
            return Core.CheckInventory("Legion Combat Trophy", trophyQuant) &&
                   Core.CheckInventory("Technique Observed", techniqueQuant) &&
                   Core.CheckInventory("Sword Scroll Fragment", scrollQuant);
        }

        void FarmScrollArea()
        {
            if (scrollQuant == 0)
                return;

            Core.PvPMove(11, "r7", 513, 286);
            Core.PvPMove(15, "r10", 832, 347);

            Core.PVPKilling();

            if (!Bot.Player.Alive)
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
            }

            Core.PvPMove(20, "r11", 943, 391);

            Core.PVPKilling();

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
            }

            Core.PvPMove(21, "r10", 9, 397);

            if (!Bot.Player.Alive || Bot.Player.Cell == "Enter0")
            {
                Core.DebugLogger(this, "You're dead stupid");
                Exit("Enter0", exitAttempt: ref exitAttempt);
            }

            Core.PvPMove(19, "r7", 7, 392);
            Core.PvPMove(14, "r6", 482, 483);
        }

        void Exit(string? cell, ref int exitAttempt)
        {
            int ExitAttempt = exitAttempt;
            int Death = 0;
            if (!Bot.Player.Alive)
                goto Death;
            else goto Exit;

            Exit:
            while (!Bot.ShouldExit && Bot.Map.Name != "battleon")
            {
                Core.Logger($"Attempting Exit {ExitAttempt++}.");
                Bot.Combat.CancelTarget();
                Bot.Wait.ForCombatExit();
                Bot.Map.Join("battleon-999999");
                Core.Sleep(1500);
                if (Bot.Map.Name != "battleon")
                    Core.Logger("Failed!? HOW.. try agian");
                else
                {
                    Core.Logger("Successful!");
                    ExitAttempt = 0;
                    break;
                }
            }

        Death:
            Core.Logger($"Death: {Death++}, resetting");
            while (!Bot.ShouldExit)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 100);
                Core.Logger($"Attempting Death Exit {ExitAttempt++}.");
                Bot.Map.Join("battleon-999999");
                Bot.Wait.ForMapLoad("battleon");
                Core.Sleep(1500);
                if (Bot.Map.Name != "battleon")
                    Core.Logger("Failed!? HOW.. try agian");
                else
                {
                    Core.Logger("Successful!");
                    ExitAttempt = 0;
                    Death = 0;
                    break;
                }
            }
        }


    }
}
