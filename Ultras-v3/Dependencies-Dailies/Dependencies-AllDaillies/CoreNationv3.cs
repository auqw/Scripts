/*
name: null
description: null
tags: null
*/
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;

public class CoreNation
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots2 Core => CoreBots2.Instance;
    private static CoreFarms2 Farm
    {
        get => _Farm ??= new CoreFarms2();
        set => _Farm = value;
    }
    private static CoreFarms2 _Farm;

    //CanChange: If enabled will sell the "Voucher of Nulgath" item during farms if it's not needed.
    bool sellMemVoucher = true;

    //CanChange: If enabled will do "Swindles Return Policy" passively during "Supplies To Spin The Wheels of Fate".
    bool returnPolicyDuringSupplies = true;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    /// <summary>
    /// Crag and Bamboozle name in game
    /// </summary>
    public string CragName => "Crag &amp; Bamboozle";

    /// <summary>
    /// All principal drops from Nulgath
    /// </summary>
    public string[] bagDrops =
    {
        "Blood Gem of the Archfiend",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Gem of Nulgath",
        "Totem of Nulgath",
        "Tainted Gem",
        "Unidentified 10",
        "Unidentified 13",
        "Voucher of Nulgath",
        "Voucher of Nulgath (non-mem)",
        // extras
        "Unidentified 24",
        "Essence of Nulgath",
        "Unidentified 25",
        "Fiend Token",
        "Emblem of Nulgath",
        "Receipt of Swindle",
        "Bone Dust",
        "Nulgath's Approval",
        "Archfiend's Favor",
        "Unidentified 34",
        "Essence of Nulgath",
    };

    public string[] SuppliesRewards =
    {
        "Tainted Gem",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Voucher of Nulgath",
        "Voucher of Nulgath (non-mem)",
        "Gem of Nulgath",
        "Unidentified 10",
        "Essence of Nulgath",
        "Receipt of Swindle",
    };

    /// <summary>
    /// Drops from the bosses that used to give acess to tercess
    /// </summary>
    public string[] tercessBags = { "Bone Dust" };

    /// <summary>
    /// List of Betrayal Blades
    /// </summary>
    public string[] betrayalBlades =
    {
        "1st Betrayal Blade of Nulgath",
        "2nd Betrayal Blade of Nulgath",
        "3rd Betrayal Blade of Nulgath",
        "4th Betrayal Blade of Nulgath",
        "5th Betrayal Blade of Nulgath",
        "6th Betrayal Blade of Nulgath",
        "7th Betrayal Blade of Nulgath",
        "8th Betrayal Blade of Nulgath",
    };

    /// <summary>
    /// Shadow Blast Arena medals
    /// </summary>
    public string[] nationMedals =
    {
        "Nation Round 1 Medal",
        "Nation Round 2 Medal",
        "Nation Round 3 Medal",
        "Nation Round 4 Medal",
    };

    public string[] Receipt =
    {
        "Unidentified 1",
        "Unidentified 6",
        "Unidentified 9",
        "Unidentified 16",
        "Unidentified 20",
        "Receipt of Swindle",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Gem of Nulgath",
        "Blood Gem of the Archfiend",
    };

    /// <summary>
    /// Misc items to accept during Bloody Chaos if turned on
    /// </summary>
    public string[] BloodyChaosSupplies =
    {
        "Tainted Gem",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Voucher of Nulgath",
        "Voucher of Nulgath (non-mem)",
        "Unidentified 10",
        "Unidentified 13",
        "Gem of Nulgath",
        "Relic of Chaos",
    };

    public string[] SwindlesReturn =
    {
        "Unidentified 1",
        "Unidentified 6",
        "Unidentified 9",
        "Unidentified 16",
        "Unidentified 20",
    };

    public string[] SwindlesReturnRewards =
    {
        "Tainted Gem",
        "Dark Crystal Shard",
        "Diamond of Nulgath",
        "Gem of Nulgath",
        "Blood Gem of the Archfiend",
        "Receipt of Swindle",
    };

    public string Uni(int nr) => $"Unidentified {nr}";

    /// <summary>
    /// Does Essence of Defeat Reagent quest for Dark Crystal Shards
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void EssenceofDefeatReagent(int quant = 1000)
    {
        if (Core.CheckInventory("Dark Crystal Shard", quant))
            return;

        Core.AddDrop(tercessBags.Concat(bagDrops).ToArray());
        Core.FarmingLogger("Dark Crystal Shard", quant);

        Core.RegisterQuests(570);
        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Crystal Shard", quant))
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("faerie", "Aracara", "Aracara's Fang", isTemp: false, log: false);
            Core.HuntMonster("hydra", "Hydra Head", "Hydra Scale", isTemp: false, log: false);
            Core.KillVath("Strand of Vath's Hair", 1, isTemp: false);
            Core.HuntMonster(
                "yokaiwar",
                "O-dokuro's Head",
                "O-dokuro's Tooth",
                isTemp: false,
                log: false
            );
            Core.KillEscherion("Escherion's Chain", publicRoom: true);

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster(
                "tercessuinotlim",
                "m2",
                "Left",
                "*",
                "Defeated Makai",
                50,
                false,
                log: false
            );

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "djinn",
                "Tibicenas",
                "Tibicenas' Chain",
                publicRoom: true,
                log: false
            );
            Bot.Wait.ForPickup("Dark Crystal Shard");
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Does NWNO from Nulgath's Birthday Gift/Bounty Hunter's Drone Pet
    /// </summary>
    /// <param name="item">Desired item to get</param>
    /// <param name="quant">Desired quantity to get</param>
    public void NewWorldsNewOpportunities(string? item = null, int quant = 1)
    {
        if (
            (item != null && Core.CheckInventory(item, quant))
            || !Core.CheckInventory(
                new[] { "Nulgath's Birthday Gift", "Bounty Hunter's Drone Pet" },
                any: true
            )
        )
            return;

        Core.AddDrop(
            Core.QuestRewards(Core.CheckInventory("Bounty Hunter's Drone Pet") ? 6183 : 6697)
        );
        Core.EquipClass(ClassType.Farm);

        Core.RegisterQuests(Core.CheckInventory("Bounty Hunter's Drone Pet") ? 6183 : 6697);
        if (item == null)
        {
            ItemBase[] QuestRewards = Core.EnsureLoad(
                    Core.CheckInventory("Bounty Hunter's Drone Pet") ? 6183 : 6697
                )
                .Rewards.ToArray();
            foreach (ItemBase Item in QuestRewards)
            {
                if (Core.CheckInventory(Item.Name, Item.MaxStack))
                    continue;

                Core.FarmingLogger(Item.Name, Item.MaxStack);

                while (!Bot.ShouldExit && !Core.CheckInventory(Item.ID, Item.MaxStack))
                {
                    if (
                        !Core.CheckInventory("Slugfit Horn", 10)
                        || !Core.CheckInventory("Cyclops Horn", 6)
                    )
                    {
                        Core.JoinSWF(
                            "mobius",
                            "ChiralValley/town-Mobius-21Feb14.swf",
                            "Slugfit",
                            "Bottom"
                        );
                        if (Bot.Player.Cell != "Slugfit")
                            Core.Jump("Slugfit", "Bottom");

                        foreach (
                            (string mobName, string itemName, int quantity) in new[]
                            {
                                ("Slugfit", "Slugfit Horn", 10),
                                ("Cyclops Warlord", "Cyclops Horn", 6),
                            }
                        )
                        {
                            if (Core.CheckInventory(itemName, quantity))
                                continue;

                            while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quantity))
                            {
                                int mapId = mobName == "Slugfit" ? 10 : 9; // Determine the map ID based on the mob name
                                if (
                                    Bot.Monsters.CurrentAvailableMonsters.Any(monster =>
                                        monster.Name == mobName
                                    )
                                )
                                    Bot.Combat.Attack(mobName);
                                else
                                    Core.Sleep();
                            }
                            Bot.Wait.ForPickup(itemName);
                        }
                    }

                    Core.KillMonster(
                        "tercessuinotlim",
                        "m2",
                        "Top",
                        "Dark Makai",
                        "Makai Fang",
                        10,
                        log: false
                    );
                    Core.KillMonster(
                        "hydra",
                        "Rune2",
                        "Left",
                        "Fire Imp",
                        "Imp Flame",
                        6,
                        log: false
                    );
                    Core.KillMonster(
                        "greenguardwest",
                        "West12",
                        "Up",
                        "Big Bad Boar",
                        "Wereboar Tusk",
                        4,
                        log: false
                    );
                }
            }
            Core.Logger("all items quant maxed");
            Core.CancelRegisteredQuests();
        }
        else
        {
            Core.FarmingLogger(item, quant);
            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            {
                if (
                    !Core.CheckInventory("Slugfit Horn", 5)
                    || !Core.CheckInventory("Cyclops Horn", 3)
                )
                {
                    Core.JoinSWF(
                        "mobius",
                        "ChiralValley/town-Mobius-21Feb14.swf",
                        "Slugfit",
                        "Bottom"
                    );

                    foreach (
                        (string mobName, string itemName, int quantity) in new[]
                        {
                            ("Slugfit", "Slugfit Horn", 5),
                            ("Cyclops Warlord", "Cyclops Horn", 3),
                        }
                    )
                    {
                        while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quantity))
                        {
                            if (!Core.CheckInventory(itemName, quantity))
                            {
                                int mapId = mobName == "Slugfit" ? 10 : 9; // Determine the map ID based on the mob name
                                if (
                                    Bot.Monsters.CurrentAvailableMonsters.Any(monster =>
                                        monster.Name == mobName
                                    )
                                )
                                    Bot.Combat.Attack(mobName);
                                else
                                    Core.Sleep();
                            }
                            else
                                break;

                            Core.Sleep();
                        }
                        Bot.Wait.ForPickup(itemName);
                    }
                }

                Core.KillMonster(
                    "tercessuinotlim",
                    "m2",
                    "Top",
                    "Dark Makai",
                    "Makai Fang",
                    5,
                    log: false
                );
                Core.KillMonster("hydra", "Rune2", "Left", "Fire Imp", "Imp Flame", 3, log: false);
                Core.KillMonster(
                    "greenguardwest",
                    "West12",
                    "Up",
                    "Big Bad Boar",
                    "Wereboar Tusk",
                    2,
                    log: false
                );
            }
            Core.Logger($"{item} is now maxed");
            Core.CancelRegisteredQuests();
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farm Diamonds from Evil War Nul quests (does Member one if possible)
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void DiamondEvilWar(int quant = 1000)
    {
        if (Core.CheckInventory("Diamond of Nulgath", quant))
            return;

        Core.AddDrop("Legion Blade", "Dessicated Heart", "Diamond of Nulgath");
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Diamond of Nulgath", quant);
        int i = 1;
        Core.Join("evilwarnul");

        while (!Bot.ShouldExit && !Core.CheckInventory("Diamond of Nulgath", quant))
        {
            if (Core.IsMember)
                Core.EnsureAccept(2221);
            else
                Core.EnsureAccept(2219);
            Core.HuntMonster(
                "evilwarnul",
                "Blade Master",
                "Legion Blade",
                isTemp: false,
                log: false
            );
            Core.HuntMonster(
                "evilwarnul",
                "Blade Master",
                "Dessicated Heart",
                20,
                false,
                log: false
            );
            Core.HuntMonster("underworld", "Skull Warrior", "Legion Helm", 5, log: false);
            Core.HuntMonster("underworld", "Skull Warrior", "Undead Skull", 3, log: false);
            Core.HuntMonster("underworld", "Skull Warrior", "Legion Champion Medal", 5, log: false);
            if (Core.IsMember)
                Core.EnsureComplete(2221);
            else
                Core.EnsureComplete(2219);
            Bot.Wait.ForPickup("Diamond of Nulgath");
            Core.Logger($"Completed x{i++}");
            if (Bot.Inventory.IsMaxStack("Diamond of Nulgath"))
                Core.Logger("Max Stack Hit.");
            else
                Core.Logger(
                    $"Diamond of Nulgath: {Bot.Inventory.GetQuantity("Diamond of Nulgath")}/{quant}"
                );
        }
    }

    /// <summary>
    /// Farms Approvals and Favors in Evil War Nul
    /// </summary>
    /// <param name="quantApproval">Desired quantity for Approvals, 5000 = max stack</param>
    /// <param name="quantFavor">Desired quantity for Favors, 5000 = max stack</param>
    public void ApprovalAndFavor(int quantApproval = 5000, int quantFavor = 5000)
    {
        if (
            Core.CheckInventory("Nulgath's Approval", quantApproval)
            && Core.CheckInventory("Archfiend's Favor", quantFavor)
        )
            return;

        Core.AddDrop("Nulgath's Approval", "Archfiend's Favor");

        Core.FarmingLogger("Nulgath's Approval", quantApproval);
        Core.FarmingLogger("Archfiend's Favor", quantFavor);

        Core.EquipClass(ClassType.Farm);
        while (!Bot.ShouldExit
            && Bot.Inventory.GetQuantity("Nulgath's Approval") < quantApproval
                || Bot.Inventory.GetQuantity("Archfiend's Favor") < quantFavor)
        {
            if (Bot.Map.Name != "evilwarnul")
            {
                Core.Join("evilwarnul");
                Bot.Wait.ForMapLoad("evilwarnul");
            }
            if (Bot.Player.Cell != "r12")
            {
                Bot.Map.Jump("r12", "Left", false);
                Bot.Wait.ForCellChange("r12");
            }
            Bot.Combat.Attack("*");
            Core.Sleep();
        }
    }

    /// <summary>
    /// Farms specific item with Swindles Return Policy quest
    /// </summary>
    /// <param name="item">Desired Item</param>
    /// <param name="quant">Desired Item quantity</param>
    public void SwindleReturn(string? item = null, int quant = 1000)
    {
        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(7551));
        if (quest == null)
        {
            Core.Logger("Swindles Return Policy quest not found.");
            return;
        }
        ItemBase? Item = quest.Rewards.FirstOrDefault(x => x.Name == item);

        if (Item == null || Core.CheckInventory(Item.Name, quant))
            return;

        Core.AddDrop(Receipt);
        if (item != null)
            Core.AddDrop(Item.ID);

        Core.FarmingLogger(Item.Name, quant);

        while (!Bot.ShouldExit && !Core.CheckInventory(Item.Name, quant))
        {
            Core.EnsureAccept(7551);
            Supplies("Unidentified 1", ReturnItem: item ?? null);
            Supplies("Unidentified 6", ReturnItem: item ?? null);
            Supplies("Unidentified 9", ReturnItem: item ?? null);
            Supplies("Unidentified 16", ReturnItem: item ?? null);
            Supplies("Unidentified 20", ReturnItem: item ?? null);
            Core.ResetQuest(7551);
            Core.DarkMakaiItem("Dark Makai Rune");
            Core.EnsureComplete(7551, Item.ID);
            SellVoucherOfNulgath(sellMemVoucher, item);

            Core.FarmingLogger(Item.Name, quant);
        }
    }

    /// <summary>
    /// Farms Tainted Gem with Swindle Bulk quest.
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void SwindleBulk(int quant = 1000)
    {
        if (Core.CheckInventory("Tainted Gem", quant))
            return;

        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Tainted Gem", quant);

        int questId = quant % 25 == 0 ? 7817 : 569;
        int cubeKillCount = quant % 25 == 0 ? 500 : 25;
        int snowGolemKillCount = quant % 25 == 0 ? 6 : 1;

        int attemptCount = 1;
        Core.AddDrop("Cubes", "Tainted Gem");
        Core.AddDrop(bagDrops);

        while (!Bot.ShouldExit && !Core.CheckInventory("Tainted Gem", quant))
        {
            Core.EnsureAccept(questId);
            Core.KillMonster(
                "boxes",
                "Fort2",
                "Left",
                "*",
                "Cubes",
                cubeKillCount,
                false,
                log: false
            );
            Core.KillMonster(
                "mountfrost",
                "War",
                "Left",
                "Snow Golem",
                "Ice Cubes",
                snowGolemKillCount,
                log: false
            );
            Core.EnsureComplete(questId);

            Bot.Wait.ForPickup("Tainted Gem");
            Core.Logger($"Completed x{attemptCount++}");

            if (Bot.Inventory.IsMaxStack("Tainted Gem"))
            {
                Core.Logger("Max Stack Hit.");
                break;
            }
            else
            {
                Core.Logger($"Tainted Gem: {Bot.Inventory.GetQuantity("Tainted Gem")}/{quant}");
            }
        }
    }

    /// <summary>
    /// Farms specified items or a specific item in the specified location.
    /// </summary>
    /// <param name="item">The item to farm. If null, it farms a list of rewards.</param>
    /// <param name="quant">Desired quantity, 1000 = max stack.</param>
    public void FarmContractExchage(string? item = null, int quant = 1)
    {
        if (
            !Core.CheckInventory("Drudgen the Assistant")
            || (item != null && Core.CheckInventory(item, quant))
        )
        {
            if (!Core.CheckInventory("Drudgen the Assistant"))
                Core.Logger("Missing \"Drudgen the Assistant\"");
            return;
        }

        string?[] rewards =
        {
            "Tainted Gem",
            "Dark Crystal Shard",
            "Gem of Nulgath",
            "Blood Gem of the Archfiend",
        };

        Core.EquipClass(ClassType.Farm);
        Core.AddDrop(Core.QuestRewards(870));

        if (item != null)
        {
            ItemBase? Reward = Core.EnsureLoad(870)?.Rewards.Find(x => x != null && x.Name == item);
            if (Reward == null)
            {
                Core.Logger($"Reward item \"{item}\" not found.");
                return;
            }

            string rewardName = Reward.Name;
            Core.FarmingLogger(rewardName, quant > 1 ? quant : Reward.MaxStack);
            while (
                !Bot.ShouldExit
                && !Core.CheckInventory(rewardName, quant > 1 ? quant : Reward.MaxStack)
            )
            {
                switch (Reward.Name)
                {
                    case "Tainted Gem":
                        Supplies("Diamond of Nulgath", 45, ReturnItem: "Diamond of Nulgath");
                        ContractExchange(
                            ContractExchangeRewards.Tainted_Gem,
                            quant > 1 ? quant : Reward.MaxStack
                        );
                        break;
                    case "Dark Crystal Shard":
                        Supplies("Diamond of Nulgath", 45, ReturnItem: "Diamond of Nulgath");
                        ContractExchange(
                            ContractExchangeRewards.Dark_Crystal_Shard,
                            quant > 1 ? quant : Reward.MaxStack
                        );
                        break;
                    case "Gem of Nulgath":
                        Supplies("Diamond of Nulgath", 45, ReturnItem: "Diamond of Nulgath");
                        ContractExchange(
                            ContractExchangeRewards.Gem_of_Nulgath,
                            quant > 1 ? quant : Reward.MaxStack
                        );
                        break;
                    case "Blood Gem of the Archfiend":
                        Supplies("Diamond of Nulgath", 45, ReturnItem: "Diamond of Nulgath");
                        ContractExchange(
                            ContractExchangeRewards.Blood_Gem_of_the_Archfiend,
                            quant > 1 ? quant : Reward.MaxStack
                        );
                        break;
                }
            }
        }
        else
        {
            foreach (string? thing in rewards)
            {
                ItemBase? Reward =
                    Core.EnsureLoad(870)?.Rewards.Find(item => item.Name == thing)
                    ?? new ItemBase();
                Core.FarmingLogger(Reward.Name, quant);
                while (
                    !Bot.ShouldExit
                    && !Core.CheckInventory(Reward.Name, quant > 1 ? quant : Reward.MaxStack)
                )
                {
                    switch (Reward.Name)
                    {
                        case "Tainted Gem":
                            Supplies("Diamond of Nulgath", 45, ReturnItem: "Tainted Gem");
                            ContractExchange(
                                ContractExchangeRewards.Tainted_Gem,
                                quant > 1 ? quant : Reward.MaxStack
                            );
                            break;
                        case "Dark Crystal Shard":
                            Supplies("Diamond of Nulgath", 45, ReturnItem: "Dark Crystal Shard");
                            ContractExchange(
                                ContractExchangeRewards.Dark_Crystal_Shard,
                                quant > 1 ? quant : Reward.MaxStack
                            );
                            break;
                        case "Gem of Nulgath":
                            Supplies("Diamond of Nulgath", 45, ReturnItem: "Gem of Nulgath");
                            ContractExchange(
                                ContractExchangeRewards.Gem_of_Nulgath,
                                quant > 1 ? quant : Reward.MaxStack
                            );
                            break;
                        case "Blood Gem of the Archfiend":
                            Supplies("Diamond of Nulgath", 45, ReturnItem: "Blood Gem of the Archfiend");
                            ContractExchange(
                                ContractExchangeRewards.Blood_Gem_of_the_Archfiend,
                                quant > 1 ? quant : Reward.MaxStack
                            );
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Farms Emblem of Nulgath in Shadow Blast Arena
    /// </summary>
    /// <param name="quant">Desired quantity, 500 = max stack</param>
    public void EmblemofNulgath(int quant = 500)
    {
        if (Core.CheckInventory("Emblem of Nulgath", quant))
            return;
        NationRound4Medal();

        Core.AddDrop("Fiend Seal", "Gem of Domination", "Emblem of Nulgath");
        Core.AddDrop(bagDrops);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Emblem of Nulgath", quant);

        Core.RegisterQuests(4748);
        while (!Bot.ShouldExit && !Core.CheckInventory("Emblem of Nulgath", quant))
        {
            Core.HuntMonster(
                "shadowblast",
                "Shadowrise Guard",
                "Gem of Domination",
                isTemp: false,
                log: false
            );
            Core.HuntMonster(
                "shadowblast",
                "Legion Fenrir",
                "Fiend Seal",
                25,
                isTemp: false,
                log: false
            );
        }
    }

    /// <summary>
    /// Farms the required medals for Nation Round 4 in Shadow Blast Arena.
    /// </summary>
    public void NationRound4Medal()
    {
        if (Core.CheckInventory("Nation Round 4 Medal"))
        {
            Core.Logger("Medal 4 owned, no need to farm it");
            return;
        }

        foreach (
            string medal in new[]
            {
                "Nation Round 1 Medal",
                "Nation Round 2 Medal",
                "Nation Round 3 Medal",
                "Nation Round 4 Medal",
            }
        )
        {
            Core.AddDrop(medal);
            if (Core.CheckInventory(medal))
            {
                Core.Logger($"\"{medal}\" owned.");
            }
            else
            {
                switch (medal)
                {
                    // The Nation Needs YOU!
                    case "Nation Round 1 Medal":
                        Core.EnsureAccept(4744);
                        Core.HuntMonster(
                            "shadowblast",
                            "Legion AirStrike",
                            "Legion Rookie Defeated",
                            5
                        );
                        Core.HuntMonster(
                            "shadowblast",
                            "Shadowrise Guard",
                            "Shadowscythe Rookie Defeated",
                            5
                        );
                        Core.EnsureComplete(4744);
                        break;

                    // Show Me More, Nation-Noob
                    case "Nation Round 2 Medal":
                        Core.EnsureAccept(4745);
                        Core.HuntMonster(
                            "shadowblast",
                            "Legion Fenrir",
                            "Legion Veteran Defeated",
                            7
                        );
                        Core.HuntMonster(
                            "shadowblast",
                            "Doombringer",
                            "Shadowscythe Veteran Defeated",
                            7
                        );
                        Core.EnsureComplete(4745);
                        break;

                    // For the Nation!
                    case "Nation Round 3 Medal":
                        Core.EnsureAccept(4746);
                        Core.HuntMonster(
                            "shadowblast",
                            "Legion Cannon",
                            "Legion Elite Defeated",
                            10
                        );
                        Core.HuntMonster(
                            "shadowblast",
                            "Draconic Doomknight",
                            "Shadowscythe Elite Defeated",
                            10
                        );
                        Core.EnsureComplete(4746);
                        break;

                    // Nulgath Likes Your Style
                    case "Nation Round 4 Medal":
                        Core.EnsureAccept(4747);
                        Core.HuntMonster("shadowblast", "Grimlord Boss", "Grimlord Vanquished");
                        Core.EnsureComplete(4747);
                        break;
                }

                Bot.Wait.ForPickup(medal);
                Core.Logger($"Medal {medal} acquired");
                Bot.Drops.Remove(medal);
            }
        }
    }

    /// <summary>
    /// Farms Totem of Nulgath/Gem of Nulgath with Voucher Item: Totem of Nulgath quest
    /// </summary>
    /// <param name="reward">Which reward to pick (totem or gem)</param>
    /// <param name="quant"></param>
    public void VoucherItemTotemofNulgath(
        VoucherItemTotem reward = VoucherItemTotem.Totem_of_Nulgath,
        int quant = 0
    )
    {
        if (!Core.CheckInventory("Voucher of Nulgath (non-mem)"))
            FarmVoucher(false, true);

        quant = quant == 0 ? (reward == VoucherItemTotem.Totem_of_Nulgath ? 100 : 1000) : quant;

        Quest quest = Core.EnsureLoad(4778);
        ItemBase? Reward = quest.Rewards.FirstOrDefault(x => x.ID == (int)reward);

        if (Reward == null)
        {
            Core.Logger("Reward not found.");
            return;
        }

        foreach (ItemBase item in quest.Requirements.Concat(quest.Rewards))
            Core.AddDrop(item.ID);

        Core.FarmingLogger(Reward.Name, quant);
        while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID, quant))
        {
            Core.EnsureAccept(4778);
            EssenceofNulgath();
            if (!Bot.Quests.EnsureComplete(4778, Reward.ID))
            {
                EssenceofNulgath(
                    Bot.Inventory.GetQuantity("Essence of Nulgath") < 100
                        ? Bot.Inventory.GetQuantity("Essence of Nulgath") + 1
                        : 60
                );
                Core.EnsureComplete(4778, Reward.ID);
            }
            Bot.Wait.ForPickup(Reward.ID);
        }
    }

    /// <summary>
    /// Farms Essences of Nulgath from Dark Makais in Tercessuinotlim
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack</param>
    public void EssenceofNulgath(int quant = 60)
    {
        if (Core.CheckInventory("Essence of Nulgath", quant))
            return;

        Core.AddDrop("Essence of Nulgath");
        Core.EquipClass(ClassType.Farm);
        Core.KillMonster(
            "tercessuinotlim",
            "m2",
            "Left",
            "Dark Makai",
            "Essence of Nulgath",
            quant,
            false
        );
        Core.JumpWait();
    }

    /// <summary>
    /// Farms the specified item or all items from the Nulgath Larvae quest.
    /// </summary>
    /// <param name="item">The item to farm. If null, all items are farmed.</param>
    /// <param name="quant">The quantity of the item to farm.</param>
    public void NulgathLarvae(string? item = null, int quant = 1)
    {
        Quest? larvaeQuest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(2566));
        if (larvaeQuest == null)
        {
            Core.Logger("Nulgath Larvae quest not found.");
            return;
        }
        if (item != null && Core.CheckInventory(item, quant))
            return;

        Quest? voucherQuest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(4778));
        if (voucherQuest == null)
            Core.Logger("Voucher quest not found.");

        if (item != null)
        {
            // Check if the item is a valid drop from quest 2566
            bool isValidItem = larvaeQuest.Rewards.Any(reward =>
                reward != null && reward.Name.FormatForCompare() == item.FormatForCompare()
            );
            if (!isValidItem)
            {
                Core.Logger($"{item} is not a valid drop from Nulgath Larvae quest.");
                return;
            }

            // Farming for a specific item
            FarmItem(larvaeQuest, voucherQuest, item, quant);
        }
        else
        {
            // Farming for all drops
            foreach (
                ItemBase reward in larvaeQuest.Rewards.Where(x =>
                    x != null && !Core.CheckInventory(x.ID, x.MaxStack, false)
                )
            )
                FarmItem(larvaeQuest, voucherQuest, reward.Name, reward.MaxStack);
        }

        void FarmItem(Quest? larvaeQuest, Quest? voucherQuest, string item, int quant)
        {
            voucherQuest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(2566));
            if (voucherQuest == null)
            {
                Core.Logger("Failed to load larvae quest (ID: 2566) after multiple attempts.");
                return;
            }

            // Ensure rewardItem is properly loaded
            ItemBase? rewardItem = Core.InitializeWithRetries(() =>
                voucherQuest.Rewards.FirstOrDefault(x => x != null && x.Name == item)
            );
            if (rewardItem == null)
            {
                Core.Logger($"Reward item '{item}' not found in larvae quest rewards.");
                return;
            }

            bool shouldFarm4778 =
                item != null
                && voucherQuest != null
                && voucherQuest.Rewards.Any(x => x != null && x.Name == item);

            Bot.Drops.Add("Mana Energy for Nulgath", item!);

            Core.FarmingLogger(item, quant);
            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            {
                Core.EnsureAccept(2566);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster(
                    "elemental",
                    "Mana Golem",
                    "Mana Energy for Nulgath",
                    10,
                    isTemp: false,
                    log: false
                );
                Core.EquipClass(ClassType.Farm);

                while (
                    !Bot.ShouldExit
                    && !Core.CheckInventory(item, quant)
                    && Core.CheckInventory("Mana Energy for Nulgath")
                )
                {
                    Core.EnsureAccept(2566);
                    Core.HuntMonster("elemental", "Mana Falcon", "Charged Mana Energy for Nulgath", 5, log: false);
                    Core.EnsureComplete(2566);
                    Bot.Wait.ForPickup(item ?? string.Empty);
                    if (
                        shouldFarm4778
                        && Core.CheckInventory("Voucher of Nulgath (non-mem)")
                        && Core.CheckInventory("Essence of Nulgath", 60)
                    )
                    {
                        Core.EnsureAccept(4778);
                        Core.EnsureCompleteMulti(4778, itemID: rewardItem?.ID ?? -1);
                        Bot.Wait.ForPickup(rewardItem?.ID ?? -1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Automates Nulgath’s Roulette of Misfortune quest to farm a specified item and quantity.
    /// Repeatedly gathers Mana Energy, converts it into Charged Mana Energy, and completes the quest
    /// until the desired item is obtained or the script is stopped.
    /// </summary>
    /// <param name="item">Target item name to acquire.</param>
    /// <param name="quant">Required quantity of the target item.</param>
    public void NulgathsRouletteofMisfortune(string? item, int quant)
    {
        if (item == null)
            return;

        if (Core.CheckInventory(item, quant))
            return;

        Core.FarmingLogger(item, quant);
        Core.AddDrop(item);

        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
        {
            Core.EnsureAccept(2566);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("elemental", "Mana Golem", "Mana Energy for Nulgath", 10, isTemp: false, log: false);
            Core.EquipClass(ClassType.Farm);

            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant) && Core.CheckInventory("Mana Energy for Nulgath"))
            {
                Core.EnsureAccept(2566);
                Core.HuntMonster("elemental", "Mana Falcon", "Charged Mana Energy for Nulgath", 5, log: false);
                Core.EnsureComplete(2566);
            }
        }
    }


    public void Supplies(string? item = null, int quant = 1, bool UltraAlteon = false, bool KeepVoucher = false, bool AssistantDuring = false, string? ReturnItem = null, bool returnPolicyDuringSupplies = false, bool VoucherItemDuring = false)
    {
        #region Early exits

        // Item already owned
        if (item != null && Core.CheckInventory(item, quant))
            return;

        // All rewards already maxed when farming everything
        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(2857));
        if (item == null && quest?.Rewards != null &&
            bagDrops[..^11].All(drop =>
                quest.Rewards.FirstOrDefault(r => r.Name == drop) is { } reward &&
                Core.CheckInventory(drop, reward.MaxStack)))
            return;

        Core.Logger("if Swindles is enabled, it will only accept the quest when it has the required Unis it needs");

        #endregion

        #region Resolve settings

        sellMemVoucher = Core.CBOBool("Nation_SellMemVoucher", out bool cboSell) && cboSell;

        UltraAlteon = UltraAlteon ||
            (Core.CBOBool("UltraAlteonForSupplies", out bool cboAlteon) && cboAlteon);

        returnPolicyDuringSupplies = returnPolicyDuringSupplies ||
            (Core.CBOBool("Nation_ReturnPolicyDuringSupplies", out bool cboReturn) && cboReturn);

        // Voucher logic cleanup
        if (KeepVoucher)
            sellMemVoucher = false;
        else if (sellMemVoucher && Bot.Player.Gold >= 100000000)
            sellMemVoucher = false;

        LogSuppliesConfig(item, quant, UltraAlteon, KeepVoucher, sellMemVoucher, AssistantDuring, returnPolicyDuringSupplies, ReturnItem);

        #endregion

        #region Quest registration

        List<int> quests = new() { 2857 };

        if (item != Uni(13) && Core.CheckInventory(38261))
            quests.Add(9542);

        if (Core.CheckInventory("Drudgen the Assistant"))
            quests.Add(870);

        Core.RegisterQuests(quests.ToArray());

        #endregion

        #region Drop registration (much cleaner)

        List<string> drops = new();

        if (item != null)
            drops.Add(item);

        if (ReturnItem != null)
            drops.Add(ReturnItem);

        drops.AddRange(Core.QuestRewards(9542));
        drops.AddRange(SuppliesRewards);

        if (sellMemVoucher)
            drops.Add("Voucher of Nulgath");

        drops.Add("Relic of Chaos");

        if (returnPolicyDuringSupplies)
            drops.AddRange(new[]
            {
            Uni(1), Uni(6), Uni(9), Uni(16), Uni(20),
            "Receipt of Swindle"
        });

        Core.AddDrop(drops.ToArray());
        Core.EquipClass(ClassType.Solo);

        #endregion

        if (item == null || item == "All")
            FarmAllSupplies(UltraAlteon, KeepVoucher, AssistantDuring, ReturnItem, returnPolicyDuringSupplies);
        else
            FarmSingleSupply(item, quant, UltraAlteon, KeepVoucher, AssistantDuring, ReturnItem, returnPolicyDuringSupplies, VoucherItemDuring);

        Core.CancelRegisteredQuests();
    }

    private void FarmAllSupplies(bool UltraAlteon, bool KeepVoucher, bool AssistantDuring, string? ReturnItem, bool returnPolicyDuringSupplies)
    {
        foreach (string thing in SuppliesRewards)
        {
            List<ItemBase> rewards = Core.EnsureLoad(2857).Rewards;
            ItemBase? rewardItem = rewards.Find(x => x.Name == thing);
            if (rewardItem == null)
                continue;

            if (Core.CheckInventory(CragName) || hasOBoNPet)
            {
                BambloozevsDrudgen(rewardItem.Name, rewardItem.MaxStack, KeepVoucher, AssistantDuring, ReturnItem, true);
                continue;
            }

            while (!Bot.ShouldExit && !Core.CheckInventory(rewardItem.ID, rewardItem.MaxStack))
            {
                if (UltraAlteon)
                    Core.KillMonster("ultraalteon", "r10", "Left", "Ultra Chaos Alteon", "Relic of Chaos", isTemp: false, log: false);
                else
                    Core.KillEscherion("Relic of Chaos", log: false, FromSupplies: true);

                SellVoucherOfNulgath(sellMemVoucher, null);
                AssistantDuringSupplies(AssistantDuring);
                DoSwindlesReturnArea(returnPolicyDuringSupplies, ReturnItem);
                HandleVoucherConversions(rewardItem.Name);
            }
        }
    }

    private void FarmSingleSupply(string item, int quant, bool UltraAlteon, bool KeepVoucher, bool AssistantDuring, string? ReturnItem, bool returnPolicyDuringSupplies, bool VoucherItemDuring)
    {
        if (Core.CheckInventory(CragName) || hasOBoNPet)
        {
            BambloozevsDrudgen(item, quant, KeepVoucher, AssistantDuring, ReturnItem, true);
            return;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
        {
            if (UltraAlteon)
                Core.KillMonster("ultraalteon", "r10", "Left", "*", isTemp: false, log: false);
            else
                Core.KillEscherion(log: false, FromSupplies: true);

            SellVoucherOfNulgath(sellMemVoucher, item);
            AssistantDuringSupplies(AssistantDuring);
            DoSwindlesReturnArea(returnPolicyDuringSupplies, ReturnItem);

            if (VoucherItemDuring)
                HandleVoucherConversions(item);
        }
    }

    private void HandleVoucherConversions(string item)
    {
        if (!Core.CheckInventory("Voucher of Nulgath (non-mem)") || !Core.CheckInventory("Essence of Nulgath", 60))
            return;

        bool diamondsMaxed = Core.CheckInventory("Diamond of Nulgath", 1000);
        bool totemsMaxed = Core.CheckInventory("Totem of Nulgath", 100);

        if (item.Equals("Diamond of Nulgath", StringComparison.OrdinalIgnoreCase))
        {
            if (diamondsMaxed && !totemsMaxed)
                Core.EnsureCompleteMulti(4778, 100, 5357);
        }
        else if (item.Equals("Totem of Nulgath", StringComparison.OrdinalIgnoreCase))
        {
            if (totemsMaxed && !diamondsMaxed)
                Core.EnsureCompleteMulti(4778, 1000, 6136);
        }
        else
        {
            if (!totemsMaxed)
                Core.EnsureCompleteMulti(4778, 100, 5357);
            else if (!diamondsMaxed)
                Core.EnsureCompleteMulti(4778, 1000, 6136);
        }
    }

    private void LogSuppliesConfig(string? item, int quant, bool ultraAlteon, bool keepVoucher, bool sellMemVoucher, bool assistantDuring, bool returnPolicyDuringSupplies, string? returnItem)
    {
        string Flag(bool v) => v ? "True" : "False";

        Core.Logger("[Supplies] =========================", "Supplies");
        Core.Logger($"Item      : {item ?? "All"} x{quant}", "Supplies");
        Core.Logger($"Ultra     : {Flag(ultraAlteon)}", "Supplies");
        Core.Logger($"Voucher K : {Flag(keepVoucher)}", "Supplies");
        Core.Logger($"Voucher S : {Flag(sellMemVoucher)}", "Supplies");
        Core.Logger($"Assist    : {Flag(assistantDuring)}", "Supplies");
        Core.Logger($"Return    : {Flag(returnPolicyDuringSupplies)}", "Supplies");
        Core.Logger($"Return It : {returnItem ?? "None"}", "Supplies");
        Core.Logger("[Supplies] =========================", "Supplies");
    }

    public void SellVoucherOfNulgath(bool sellMemVoucher = true, string? item = null)
    {
        if (!sellMemVoucher || sellMemVoucher && item == "Voucher of Nulgath")
            return;

        if (Core.CheckInventory("Voucher of Nulgath"))
        {
            Core.Jump("Enter", "Spawn");
            Core.SellItem("Voucher of Nulgath", all: true);
        }
    }

    public void AssistantDuringSupplies(bool assistDuring = true)
    {
        if (!assistDuring)
            return;

        if (Bot.Player.Gold >= 100_000)
        {
            Core.Jump("Enter", "Spawn");
            int quantityToBuy = (int)Math.Min(Bot.Player.Gold / 100_000M, 250);
            Core.EnsureAccept(2859);
            Core.BuyItem("yulgar", 41, "War-Torn Memorabilia", quantityToBuy);
            Core.EnsureCompleteMulti(2859, quantityToBuy);
        }
    }
    /// <summary>
    /// Completes the "Swindle's Return Area" quest (ID 7551),
    /// prioritizing a specific reward or the first non-maxed one.
    /// </summary>
    void DoSwindlesReturnArea(bool returnPolicyActive, string? item = null)
    {
        if (!returnPolicyActive)
        {
            return;
        }

        if (!Core.CheckInventory(new[] { Uni(1), Uni(6), Uni(9), Uni(16), Uni(20) }))
        {
            return;
        }

        Quest? quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(7551));
        if (quest?.Rewards == null)
        {
            Core.DebugLogger(this, "Failed to load quest 7551 - quest or rewards are null");
            return;
        }

        // Early exit: check if preferred item is already maxed
        if (item != null)
        {
            ItemBase? preferred = quest.Rewards.FirstOrDefault(r => r.Name == item);
            if (preferred == null)
            {
                Core.DebugLogger(this, $"Preferred item '{item}' not found in quest rewards");
                return;
            }

            if (Core.CheckInventory(preferred.ID, preferred.MaxStack))
            {
                Core.DebugLogger(this, $"Preferred item '{item}' is already maxed ({Bot.Inventory.GetQuantity(preferred.Name)}{preferred.MaxStack}) - skipping quest");
                return;
            }
        }
        // Early exit: check if all rewards are already maxed
        else if (quest.Rewards.All(r => Core.CheckInventory(r.ID, r.MaxStack)))
        {
            Core.DebugLogger(this, "All quest rewards are already maxed - skipping quest");
            return;
        }


        Core.EnsureAccept(7551);
        Core.ResetQuest(7551);
        Core.DarkMakaiItem("Dark Makai Rune");

        ItemBase? reward = item != null
            ? quest.Rewards.FirstOrDefault(r => r.Name == item)
            : quest.Rewards.FirstOrDefault(r => !Core.CheckInventory(r.ID, r.MaxStack));

        if (!Bot.Quests.CanCompleteFullCheck(7551))
        {
            Core.DebugLogger(this, "Quest 7551 cannot be completed - missing requirements");
            return;
        }

        if (reward != null)
        {
            Core.DebugLogger(this, "Completing quest with specific reward: {reward.Name} (ID: {reward.ID})");
            Core.EnsureComplete(7551, reward.ID);
            Bot.Wait.ForQuestComplete(7551);
            Bot.Wait.ForPickup(reward.ID);
            return;
        }

        Core.EnsureComplete(7551);
    }

    /// <summary>
    /// Does "The Assistant" quest for the desired item.
    /// </summary>
    /// <param name="item">Desired item name. Pass null to farm all available drops.</param>
    /// <param name="quant">Desired item quantity.</param>
    /// <param name="farmGold">Whether to farm gold (default: true).</param>
    /// <param name="Reward">Swindles Return Policy quest reward (default: None).</param>
    public void TheAssistant(
        string? item = null,
        int quant = 1000,
        bool farmGold = true,
        SwindlesReturnReward Reward = SwindlesReturnReward.None
    )
    {
        if (item != null && Core.CheckInventory(item, quant))
            return;

    Retry7551:
        Quest? Swindles = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(7551));
        if (Swindles == null)
        {
            Core.Logger("Failed to load quest 7551, retrying...");
            Core.Sleep();
            goto Retry7551;
        }

    Retry2859:
        Quest? Assistant = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(2859));
        if (Assistant == null)
        {
            Core.Logger("Failed to load quest 2859, retrying...");
            Core.Sleep();
            goto Retry2859;
        }

        // List of available drops for "The Assistant" quest
        string[] selectedDrops = item != null ? new string[] { item } : bagDrops[..^11];
        Core.AddDrop(selectedDrops);

        //add `Receipt of Swindle` from swindles return rewards.
        Core.AddDrop("Receipt of Swindle");

        //if running standalone, add the reward slection.
        if (Reward != SwindlesReturnReward.None)
            Core.AddDrop((int)Reward);

        //handle quant if it goes over max stack.
        if (item != null && quant > 0)
        {
            ItemBase? reward = Assistant.Rewards.FirstOrDefault(x => x.Name == item);
            if (reward != null)
            {
                int maxStack = reward.MaxStack;
                quant = quant > maxStack ? maxStack : quant;
            }
            else
            {
                Core.Logger($"Reward item \"{item}\" not found.");
            }
        }

        // Check if return policy is active
        returnPolicyDuringSupplies = Core.CBOBool(
            "Nation_ReturnPolicyDuringSupplies",
            out bool _returnSupplies
        );

        Core.Logger(
            returnPolicyDuringSupplies
                ? "Return Policy During Supplies: true"
                : "Return Policy During Supplies: false"
        );

        string[]? rPDSuni = null;
        if (returnPolicyDuringSupplies)
        {
            rPDSuni = new[] { Uni(1), Uni(6), Uni(9), Uni(16), Uni(20) };
            Core.AddDrop(rPDSuni);
            Core.AddDrop("Blood Gem of the Archfiend");
        }

        // Register the "Swindles Return Policy" quest if specified
        if (returnPolicyDuringSupplies && Reward != SwindlesReturnReward.None)
        {
            DoSwindlesReturnArea(returnPolicyDuringSupplies, Reward.ToString().Replace("_", ""));
        }

        if (item == null)
        {
            Core.Logger("Assistant Item = null, maxing the important rewards.");
            foreach (string Thing in selectedDrops)
            {
                // Find the corresponding item in quest rewards
                List<ItemBase> rewards = Core.EnsureLoad(2859).Rewards;
                ItemBase? Item = rewards.Find(x => x.Name == Thing);

                if (Item == null)
                    continue;

                Core.FarmingLogger(Item.Name, Item.MaxStack);
                // Continue farming until the desired item quantity is obtained
                while (!Bot.ShouldExit && !Core.CheckInventory(Item.Name, Item.MaxStack))
                {
                    LogMobItemQuant2(Item, Item.MaxStack);
                    if (farmGold)
                        Farm.Gold(1000000);

                    Core.EnsureAccept(2859);
                    Core.BuyItem("yulgar", 41, "War-Torn Memorabilia", 10);
                    Core.EnsureCompleteMulti(2859);

                    DoSwindlesReturnArea(
                        returnPolicyDuringSupplies,
                        Reward.ToString().Replace("_", "")
                    );

                    if (
                        Core.CheckInventory("Voucher of Nulgath (non-mem)")
                        && Core.CheckInventory("Essence of Nulgath", 60)
                    )
                    {
                        bool diamondsMaxed = Core.CheckInventory("Diamond of Nulgath", 1000);
                        bool totemsMaxed = Core.CheckInventory("Totem of Nulgath", 100);

                        if (
                            item?.Equals("Diamond of Nulgath", StringComparison.OrdinalIgnoreCase)
                            == true
                        )
                        {
                            if (diamondsMaxed && !totemsMaxed)
                                Core.EnsureCompleteMulti(4778, itemID: 5357); // Totems
                        }
                        else if (
                            item?.Equals("Totem of Nulgath", StringComparison.OrdinalIgnoreCase)
                            == true
                        )
                        {
                            if (totemsMaxed && !diamondsMaxed)
                                Core.EnsureCompleteMulti(4778, itemID: 6136); // Diamonds
                        }
                        else // item == null or any other item
                        {
                            if (!totemsMaxed)
                                Core.EnsureCompleteMulti(4778, itemID: 5357); // Prioritize Totems
                            else if (!diamondsMaxed)
                                Core.EnsureCompleteMulti(4778, itemID: 6136); // Then Diamonds
                        }
                    }
                }
            }
            Core.CancelRegisteredQuests();
        }
        else
        {
            Core.FarmingLogger(item, quant);
            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            {
                LogMobItemQuant(item!, quant);
                if (farmGold)
                    Farm.Gold(1000000);

                Core.EnsureAccept(2859);
                Core.BuyItem("yulgar", 41, "War-Torn Memorabilia", 10);
                Bot.Wait.ForItemBuy(40);
                Core.EnsureCompleteMulti(2859);

                DoSwindlesReturnArea(
                    returnPolicyDuringSupplies,
                    Reward.ToString().Replace("_", "")
                );

                if (
                    Core.CheckInventory("Voucher of Nulgath (non-mem)")
                    && Core.CheckInventory("Essence of Nulgath", 60)
                )
                {
                    bool diamondsMaxed = Core.CheckInventory("Diamond of Nulgath", 1000);
                    bool totemsMaxed = Core.CheckInventory("Totem of Nulgath", 100);

                    if (
                        item?.Equals("Diamond of Nulgath", StringComparison.OrdinalIgnoreCase)
                        == true
                    )
                    {
                        if (diamondsMaxed && !totemsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 5357); // Totems
                    }
                    else if (
                        item?.Equals("Totem of Nulgath", StringComparison.OrdinalIgnoreCase) == true
                    )
                    {
                        if (totemsMaxed && !diamondsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 6136); // Diamonds
                    }
                    else // item == null or any other item
                    {
                        if (!totemsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 5357); // Prioritize Totems
                        else if (!diamondsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 6136); // Then Diamonds
                    }
                }
            }
        }
        if (returnPolicyDuringSupplies && Reward != SwindlesReturnReward.None)
            Bot.Quests.UnregisterQuests(7551);
    }

    /// <summary>
    /// Logs the quantity of the specified item after a time interval.
    /// </summary>
    /// <param name="item">Item name</param>
    /// <param name="quant">Desired item quantity</param>
    void LogMobItemQuant(string item, int quant)
    {
        // Check if the specified item is in inventory
        if (!Core.CheckInventory(item))
            return;

        // Get the initial quantity of the item in the inventory
        int startQuant = Bot.Inventory.GetQuantity(item);

        // Wait for a short period (e.g., 1.5 seconds) to allow the item quantity to change
        // (e.g., after completing a quest, the quantity might increase)
        Core.Sleep(1500);

        // Get the current quantity of the item in the inventory
        int currentQuant = Bot.Inventory.GetQuantity(item);

        // If the quantity changes or increases during the interval, log the updated quantity
        if (currentQuant != startQuant || currentQuant > startQuant)
        {
            Core.FarmingLogger(item, quant);

            // Wait for a short period again (optional)
            Core.Sleep(1500);
        }
    }

    /// <summary>
    /// Logs the quantity of the specified item object after a time interval.
    /// </summary>
    /// <param name="item">Item object</param>
    /// <param name="quant">Desired item quantity</param>
    static void LogMobItemQuant2(ItemBase item, int quant = 1)
    {
        // Check if the specified item is in inventory
        if (!Core.CheckInventory(item.Name))
            return;

        // Get the initial quantity of the item in the inventory
        int startQuant = item.Quantity;

        // Wait for a short period (e.g., 1.5 seconds) to allow the item quantity to change
        // (e.g., after completing a quest, the quantity might increase)
        Core.Sleep(1500);

        // Get the current quantity of the item in the inventory
        int currentQuant = item.Quantity;

        // If the quantity changes or increases during the interval, log the updated quantity
        if (currentQuant > startQuant)
        {
            Core.FarmingLogger(item.Name, quant > 1 ? item.MaxStack : 1);

            // Wait for a short period again (optional)
            Core.Sleep(1500);
        }
    }

    public bool hasOBoNPet => Bot.Player.IsMember && Core.CheckInventory(new[] { 4809, 5373 }, any: true);

    /// <summary>
    /// Performs the "Bamblooze vs. Drudgen" quest for the desired item.
    /// </summary>
    /// <param name="item">Desired item name.</param>
    /// <param name="quant">Desired item quantity.</param>
    /// <param name="KeepVoucher">Flag indicating if the voucher should be kept.</param>
    /// <param name="AssistantDuring">Flag indicating if the assistant should be active during the process.</param>
    /// <param name="ReturnItem">Item to return, if any.</param>
    /// <param name="CamefromSupplies">Flag indicating if the call came from the Supplies method.</param>
    public void BambloozevsDrudgen(string? item = null, int quant = 1, bool KeepVoucher = false, bool AssistantDuring = false, string? ReturnItem = null, bool CamefromSupplies = false)
    {
        if (Core.CheckInventory(item, quant))
        {
            Core.Logger($"{item} x{quant} already owned.");
            return;
        }

        Core.AddDrop("Relic of Chaos", "Tainted Core");
        Core.AddDrop(string.IsNullOrEmpty(item) ? bagDrops : new string[] { item });

        bool returnPolicyDuringSupplies =
            Core.CBOBool("Nation_ReturnPolicyDuringSupplies", out bool _returnSupplies)
            && _returnSupplies == true;

        bool sellMemVoucher =
            Core.CBOBool("Nation_SellMemVoucher", out bool _sellMemVoucher)
            && _sellMemVoucher == true;

        bool HasLogged = false;

        if (!CamefromSupplies)
            Core.Logger($"Bamblooze mode → Item: {item ?? "All"} | Keep:{KeepVoucher} Assist:{AssistantDuring}");

        SellVoucherOfNulgath(sellMemVoucher, item);

        if (returnPolicyDuringSupplies)
            Core.AddDrop(Uni(1), Uni(6), Uni(9), Uni(16), Uni(20));

        Dictionary<string, int> rewardItemIds = new()
        {
            { "Dark Crystal Shard", 123 },
            { "Diamond of Nulgath", 456 },
            { "Gem of Nulgath", 789 },
            { "Tainted Gem", 101 },
            { "Unidentified 10", 202 },
        };

        List<ItemBase> rewards = Core.InitializeWithRetries(() => Core.EnsureLoad(2857).Rewards) ?? new List<ItemBase>();
        ItemBase? itemBase = rewards.Find(x => x != null && x.Name == item);

        if (!string.IsNullOrEmpty(item))
            Core.FarmingLogger(item, quant);

        // Choose the appropriate quest based on pet availability
        List<int> QuestToRegister = new();

        // 2857 - Supplies to Spin The Wheel of Chance
        QuestToRegister.AddRange(new[] { 2857 });

        // 609 - Bamboozle vs Drudgen
        if (Core.CheckInventory(CragName))
            QuestToRegister.AddRange(new[] { 609 });

        if (hasOBoNPet)
        {
            Core.AddDrop("Tainted Soul");
            // The Dark Deal | The Dark Deal (rare)
            QuestToRegister.Add(Core.CheckInventory(4809) ? 599 : 2561);
        }

        // 9542 - Swindle's Bonus Deal - Swindle Bilk's To Go Hut
        if (Core.CheckInventory(38261))
            QuestToRegister.Add(9542);

        QuestToRegister = QuestToRegister.Distinct().ToList();

        // Register unique quests only
        Core.RegisterQuests(QuestToRegister.ToArray());
        Core.EquipClass(ClassType.Solo);
        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
        {
            Core.KillMonster("evilmarsh", "End", "Left", "Tainted Elemental", log: false);

            // Sell Voucher of Nulgath if allowed
            SellVoucherOfNulgath(sellMemVoucher, item);

            Bot.Sleep(500);
            // Spend gold if AssistantDuring
            AssistantDuringSupplies(AssistantDuring);

            Bot.Sleep(500);
            // Do Swindles Return Policy if enabled
            // 7551 - Swindle's Return Policy
            if (ReturnItem != null)
                DoSwindlesReturnArea(returnPolicyDuringSupplies, ReturnItem);

            Bot.Sleep(500);
        Retry:
            //reduce spam
            Quest? quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(7551));
            if (quest != null)
            {
                if (quest.Rewards.All(x => Bot.Inventory.GetQuantity(x.ID) >= x.MaxStack))
                {
                    if (!HasLogged && returnPolicyDuringSupplies)
                    {
                        HasLogged = true;
                    }
                }

                Bot.Sleep(500);
                if (
                    returnPolicyDuringSupplies
                    && (item == "Diamond of Nulgath" || item == null)
                    && !Core.CheckInventory("Diamond of Nulgath", 1000)
                )
                    CragsThirst();

                Bot.Sleep(500);
                if (
                    Core.CheckInventory("Voucher of Nulgath (non-mem)")
                    && Core.CheckInventory("Essence of Nulgath", 60)
                )
                {
                    bool diamondsMaxed = Core.CheckInventory("Diamond of Nulgath", 1000);
                    bool totemsMaxed = Core.CheckInventory("Totem of Nulgath", 100);

                    if (
                        item?.Equals("Diamond of Nulgath", StringComparison.OrdinalIgnoreCase)
                        == true
                    )
                    {
                        if (diamondsMaxed && !totemsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 5357); // Totems
                    }
                    else if (
                        item?.Equals("Totem of Nulgath", StringComparison.OrdinalIgnoreCase) == true
                    )
                    {
                        if (totemsMaxed && !diamondsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 6136); // Diamonds
                    }
                    else // item == null or any other item
                    {
                        if (!totemsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 5357); // Prioritize Totems
                        else if (!diamondsMaxed)
                            Core.EnsureCompleteMulti(4778, itemID: 6136); // Then Diamonds
                    }
                }
            }
            else
            {
                Core.Logger("Failed to load quest 7551.");
                Core.Sleep();
                goto Retry;
            }
            Bot.Sleep(500);
        }
        HasLogged = false;
    }

    /// <summary>
    /// Does the "AssistingDrudgen" Quest for Fiend Tokens (and other possible drops).
    /// Requires either "Drudgen the Assistant" or "Twin Blade of Nulgath" to accept.
    /// </summary>
    /// <param name="item">Desired item name</param>
    /// <param name="quant">Desired item quantity</param>
    public void AssistingDrudgen(string item = "Any", int quant = 1)
    {
        if (
            Core.CheckInventory(item, quant)
            || !Core.CheckInventory("Drudgen the Assistant")
            || !Core.CheckInventory("Twin Blade of Nulgath")
            || !Bot.Player.IsMember
        )
            return;

        if (!Bot.Quests.IsAvailable(3826))
        {
            Core.Logger("Quest \"Seal of Light\"[Daily] is not available yet today.");
            return;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
        {
            Core.EnsureAccept(5816);
            Core.HuntMonster("willowcreek", "Hidden Spy", "The Secret 1", isTemp: false);
            EssenceofNulgath(20);
            ApprovalAndFavor(50, 50);
            Core.KillMonster("boxes", "Fort2", "Left", "*", "Cubes", 50, false);
            Core.KillMonster("shadowblast", "r13", "Left", "*", "Fiend Seal", 10, false);
            Bot.Quests.UpdateQuest(3824);
            if (Bot.Quests.IsAvailable(3826) && !Core.CheckInventory(25026))
            {
                Core.EnsureAccept(3826);
                Core.HuntMonster("alteonbattle", "Ultra Chaos Alteon", "Seal of Light");
                Core.EnsureComplete(3826);
            }
            Core.EnsureComplete(5816);
        }
    }

    /// <summary>
    /// Completes the Feed the Fiend quest to obtain the specified item.
    /// </summary>
    /// <param name="item">The item to obtain (default: "Fiend Token").</param>
    /// <param name="quant">The quantity of the item to obtain (default: 30).</param>
    public void FeedtheFiend(string item = "Fiend Token", int quant = 30)
    {
        // Check if the desired item is already in inventory or if the player is not a member
        if (Core.CheckInventory(item, quant) || !Core.IsMember)
            return;
        Core.AddDrop(item);
        // Update and register the necessary quests
        Bot.Quests.UpdateQuest(2215);
        Core.RegisterQuests(3053);

        // Equip the appropriate class for the quest
        Core.EquipClass(ClassType.Solo);

        // Continue the quest until the desired item and quantity are obtained
        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
        {
            // Hunt monsters to complete the quest
            FarmDiamondofNulgath(1);
            Core.HuntMonster("lair", "Red Dragon", "Dragon Fiend Gem", 13, isTemp: false);
            Core.KillMonster("battleunderd", "r5", "Left", "Glacial Horror", "Glacial Bones", 3, isTemp: false); Core.HuntMonster("dreammaze", "Screamfeeder", "Screamfeeder Heart", isTemp: false);
        }

        // Wait for the item to be picked up and cancel any registered quests
        Bot.Wait.ForPickup(item);
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Completes the Void Knight Sword Quest to obtain the specified item.
    /// </summary>
    /// <param name="item">The item to obtain (default: "Any").</param>
    /// <param name="quant">The quantity of the item to obtain (default: 1).</param>
    public void VoidKnightSwordQuest(string? item = null, int quant = 1)
    {
        if (
            (item != null && Core.CheckInventory(item, quant))
            || (!Core.CheckInventory(new[] { 38275, 38254 }, any: true))
        )
            return;

        Core.AddDrop(bagDrops);
        if (item != null)
            Core.AddDrop(item);

        if (item == null)
        {
            int questId = Core.CheckInventory(38275) ? 5662 : 5659;
            Core.AddDrop(Core.QuestRewards(questId));
            Quest? quest = Bot.Quests.EnsureLoad(questId);
            if (quest == null)
            {
                Core.Logger($"Failed to load quest {questId} for VoidKnightSwordQuest.");
                return;
            }

            foreach (ItemBase Reward in quest.Rewards)
            {
                if (Core.CheckInventory(Reward.ID, Reward.MaxStack))
                    continue;

                while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID, Reward.MaxStack))
                {
                    Core.EnsureAccept(questId);
                    Core.EquipClass(ClassType.Solo);
                    Core.JoinSWF(
                        "mobius",
                        "ChiralValley/town-Mobius-21Feb14.swf",
                        "Slugfit",
                        "Bottom"
                    );
                    Core.HuntMonster("mobius", "Slugfit", "Slugfit Horn", 5);
                    Core.HuntMonster("faerie", "Aracara", "Aracara Silk");

                    // Equip the Farm class and hunt monsters for quest completion
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster("tercessuinotlim", "m2", "Left", "*", "Makai Fang", 5);
                    Core.HuntMonster("hydra", "Fire Imp", "Imp Flame", 3, log: false);
                    Core.HuntMonster(
                        "battleunderc",
                        "Crystalized Jellyfish",
                        "Aquamarine of Nulgath",
                        3,
                        false
                    );
                    Core.EnsureComplete(questId);
                }
            }
        }
        else
        {
            Core.FarmingLogger(item, quant);

            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            {
                Core.EnsureAccept(Core.CheckInventory(38275) ? 5662 : 5659);
                Core.EquipClass(ClassType.Solo);
                Core.JoinSWF("mobius", "ChiralValley/town-Mobius-21Feb14.swf", "Slugfit", "Bottom");
                Core.HuntMonster("mobius", "Slugfit", "Slugfit Horn", 5);
                Core.HuntMonster("faerie", "Aracara", "Aracara Silk");

                // Equip the Farm class and hunt monsters for quest completion
                Core.EquipClass(ClassType.Farm);
                Core.KillMonster("tercessuinotlim", "m2", "Left", "*", "Makai Fang", 5);
                Core.HuntMonster("hydra", "Fire Imp", "Imp Flame", 3, log: false);
                Core.HuntMonster(
                    "battleunderc",
                    "Crystalized Jellyfish",
                    "Aquamarine of Nulgath",
                    3,
                    false
                );
                Core.EnsureComplete(Core.CheckInventory(38275) ? 5662 : 5659);
            }
        }
    }

    /// <summary>
    /// Do Diamond Exchange quest 1 time, if farmDiamond is true, will farm 15 Diamonds before if needed
    /// </summary>
    /// <param name="farmDiamond">Whether or not farm Diamonds</param>
    public void DiamondExchange(bool farmDiamond = true)
    {
        if (
            (!Core.CheckInventory("Diamond of Nulgath", 15) && !farmDiamond)
            || !Core.CheckInventory(CragName)
            || Core.CheckInventory(Uni(13), 13)
        )
            return;

        Core.AddDrop("Diamond of Nulgath");
        // Core.DebugLogger(this);

        if (farmDiamond)
            if (hasOBoNPet || Core.CheckInventory(CragName))
                BambloozevsDrudgen("Diamond of Nulgath", 15);
        // Core.DebugLogger(this);

        Core.EquipClass(ClassType.Farm);
        // Core.DebugLogger(this);
        while (
            !Bot.ShouldExit
            && Core.CheckInventory("Diamond of Nulgath", 15)
            && !Core.CheckInventory(Uni(13), 13)
        )
        {
            // Core.DebugLogger(this);
            Core.ResetQuest(869);
            // Core.DebugLogger(this);
            Core.DarkMakaiItem("Dark Makai Sigil");
            Core.EnsureCompleteMulti(869);
        }
        Bot.Options.AttackWithoutTarget = false;
    }

    /// <summary>
    /// Do Contract Exchange quest 1 time, if <paramref name="farmUni13"/> is true, will farm Uni 13 before if needed
    /// </summary>
    /// <param name="rewardEnum"></param>
    /// <param name="quant"></param>
    /// <param name="farmUni13">Whether or not farm Uni 13</param>
    public void ContractExchange(
        ContractExchangeRewards rewardEnum,
        int quant,
        bool farmUni13 = true
    )
    {
        string reward = rewardEnum.ToString().Replace("_", " ");
        if (
            (!Core.CheckInventory(Uni(13)) && !farmUni13)
            || !Core.CheckInventory("Drudgen the Assistant")
        )
        {
            if (!Core.CheckInventory(Uni(13)) && !farmUni13)
                Core.Logger($"{farmUni13} is probably set to false, please have a dev change it");
            if (!Core.CheckInventory("Drudgen the Assistant"))
                Core.Logger("Missing \"Drudgen the Assistant\"");
            return;
        }

        Core.AddDrop(bagDrops);
        Core.EquipClass(ClassType.Solo);
        Core.Logger($"Reward Chosen: {reward} [{(int)rewardEnum}]");
        Core.FarmingLogger(reward, quant);
        while (!Bot.ShouldExit && !Core.CheckInventory(reward, quant))
        {
            if (farmUni13 && !Core.CheckInventory(Uni(13)))
                FarmUni13(3);
            Core.ResetQuest(870);
            Core.KillMonster("tercessuinotlim", "m4", "Top", "Shadow of Nulgath", log: false);
            Core.EnsureComplete(870, (int)rewardEnum);
        }
    }

    private static readonly HashSet<string> _sroeValidSet =
    [
        "Blood Gem of the Archfiend",
        "Dark Crystal Shard",
        "Gem of Nulgath",
        "Tainted Gem",
        "Totem of Nulgath"
    ];


    /// <summary>
    /// Does Swindle's Dirt-y Deeds Done Dirt Cheap quest.
    /// Only use if /TowerofDoom10 completed and a good solo class.
    /// </summary>
    public void DirtyDeedsDoneDirtCheap(int quant = 1000, bool SRoE = false, string[]? SRoEItems = null)
    {
        Core.AddDrop(
             "Emerald Pickaxe",
             "Seraphic Grave Digger Spade",
             "Unidentified 10",
             "Receipt of Swindle",
             "Blood Gem of the Archfiend",
             "Dark Crystal Shard",
             "Gem of Nulgath",
             "Tainted Gem",
             "Totem of Nulgath"
         );

        Core.EquipClass(ClassType.Solo);
        Core.KillEscherion("Emerald Pickaxe");
        Core.KillMonster("legioncrypt", "r1", "Top", "Gravedigger", "Seraphic Grave Digger Spade", isTemp: false);

        ShopItem[]? itemsToBuy = null;

        if (SRoE)
        {
            ShopItem[] shopItems = Core.GetShopItems("tercessuinotlim", 1951).ToArray();

            bool buyAll = SRoEItems?.Length == 1 &&
                          SRoEItems[0].Equals("All", StringComparison.OrdinalIgnoreCase);

            HashSet<string>? selectedSet = null;

            if (!buyAll && SRoEItems?.Length > 0)
                selectedSet = [.. SRoEItems];

            List<ShopItem> build = new();

            foreach (ShopItem item in shopItems)
            {
                if (item == null)
                    continue;

                if (!_sroeValidSet.Contains(item.Name))
                    continue;

                if (!buyAll && (selectedSet == null || !selectedSet.Contains(item.Name)))
                    continue;

                build.Add(item);
            }

            if (build.Count > 0)
                itemsToBuy = build.ToArray();
        }


        while (!Bot.ShouldExit && !Core.CheckInventory("Unidentified 10", quant))
        {
            Core.EnsureAccept(7818);

            Core.HuntMonster("towerofdoom10", "Slugbutter", "Slugbutter Digging Advice", publicRoom: true, log: false);
            Core.HuntMonster("crownsreach", "Chaos Tunneler", "Chaotic Tunneling Techniques", 2, log: false);
            Core.HuntMonster("downward", "Crystal Mana Construct", "Crystalized Corporate Digging Secrets", 3, log: false);

            Core.EnsureComplete(7818);
            Core.FarmingLogger("Unidentified 10", quant);

            if (!SRoE || itemsToBuy == null || itemsToBuy.Length == 0)
                continue;

            if (Bot.Inventory.GetQuantity("Unidentified 10") < 1000)
                continue;

            bool allMaxed = true;

            foreach (ShopItem item in itemsToBuy)
            {
                if (item == null)
                    continue;

                int buyQty = Core.MaxBuyQuant("tercessuinotlim", 1951, item);
                int currentQty = Bot.Inventory.GetQuantity(item.ID);

                // Cap by max stack
                buyQty = Math.Min(buyQty, item.MaxStack - currentQty);
                if (buyQty <= 0)
                    continue; // Already maxed

                allMaxed = false;

                Core.Logger($"Buying {buyQty}x {item.Name}");
                Core.BuyItem("tercessuinotlim", 1951, item.ID, buyQty, item.ShopItemID);
            }

            if (allMaxed)
            {
                Core.Logger("All selected SRoE items maxed. Disabling SRoE.");
                SRoE = false;
            }
        }
    }


    /// <summary>
    /// Farms Unidentified 13 with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 13 = max stack</param>
    public void FarmUni13(int quant = 13)
    {
        if (Core.CheckInventory(Uni(13), quant))
            return;

        Core.AddDrop(Uni(13));
        quant = quant > 13 ? 13 : quant;

        // Core.DebugLogger(this);
        if (Core.CheckInventory(CragName))
            while (!Bot.ShouldExit && !Core.CheckInventory(Uni(13), quant))
                DiamondExchange();
        NewWorldsNewOpportunities(Uni(13), quant); //1minute turning  = 1x guaranteed
        VoidKnightSwordQuest(Uni(13), quant);
        Supplies(Uni(13), quant);
    }

    /// <summary>
    /// Farms Unidentified 10 with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void FarmUni10(int quant = 1000)
    {
        if (Core.CheckInventory("Unidentified 10", quant))
            return;

        Core.AddDrop("Unidentified 10");
        if (hasOBoNPet || Core.CheckInventory(CragName))
            BambloozevsDrudgen("Unidentified 10", quant);
        DirtyDeedsDoneDirtCheap(quant);
    }

    /// <summary>
    /// Farms Dark Crystal Shard with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void FarmDarkCrystalShard(int quant = 1000)
    {
        if (Core.CheckInventory("Dark Crystal Shard", quant))
            return;

        Core.AddDrop("Dark Crystal Shard");
        FarmContractExchage("Dark Crystal Shard", quant);
        NewWorldsNewOpportunities("Dark Crystal Shard", quant); //1minute turning  = 1x guaranteed
        if (Core.CheckInventory(CragName))
            Supplies("Dark Crystal Shard", quant, ReturnItem: "Dark Crystal Shard"); //xx:xx time turnin = 10% chance
        VoidKnightSwordQuest("Dark Crystal Shard", quant);
        Supplies("Dark Crystal Shard", quant, ReturnItem: "Dark Crystal Shard"); //xx:xx time turnin = 10% chance
        EssenceofDefeatReagent(quant);
    }

    /// <summary>
    /// Farms Diamond of Nulgath with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 1000 = max stack</param>
    public void FarmDiamondofNulgath(int quant = 1000)
    {
        if (Core.CheckInventory("Diamond of Nulgath", quant))
            return;

        Core.AddDrop("Diamond of Nulgath");

        // This Quest is more of an additive Bonus whislt doing supplies
        while (
            !Bot.ShouldExit
            && !Core.CheckInventory("Diamond of Nulgath", quant)
            && Core.CheckInventory(CragName)
            && Core.CheckInventory(Uni(10), 100)
        )
            CragsThirst(quant);
        if (Core.CheckInventory(CragName))
            Supplies("Diamond of Nulgath", quant, ReturnItem: "Diamond of Nulgath");

        VoidKnightSwordQuest("Diamond of Nulgath", quant);
        Supplies("Diamond of Nulgath", quant, ReturnItem: "Diamond of Nulgath");
    }

    /// <summary>
    /// Farms Fiend Tokens using various methods.
    /// </summary>
    /// <param name="quant">Desired quantity of Fiend Tokens, 30 = default stack size.</param>
    public void FarmFiendToken(int quant = 30)
    {
        // Check if Fiend Tokens are already in inventory
        if (Core.CheckInventory("Fiend Token", quant))
            return;

        // Try different quest methods to obtain Fiend Tokens
        VoidKnightSwordQuest("Fiend Token", quant);
        AssistingDrudgen("Fiend Token", quant);
        FeedtheFiend();
    }

    /// <summary>
    /// Farms Gem of Nulgath with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 300 = max stack</param>
    public void FarmGemofNulgath(int quant = 1000)
    {
        if (Core.CheckInventory("Gem of Nulgath", quant))
            return;

        Core.AddDrop("Gem of Nulgath");
        FarmContractExchage("Gem of Nulgath", quant);
        if (Core.CheckInventory(CragName))
            Supplies("Gem of Nulgath", quant, ReturnItem: "Gem of Nulgath");
        VoidKnightSwordQuest("Gem of Nulgath", quant);
        Supplies("Gem of Nulgath", quant, ReturnItem: "Gem of Nulgath");
    }

    /// <summary>
    /// Farms Blood Gem of the Archfiend with the best method available
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack</param>
    /// <param name="HydraLevel"></param>
    public void FarmBloodGem(int quant = 100, int HydraLevel = 85)
    {
        if (Core.CheckInventory("Blood Gem of the Archfiend", quant))
            return;

        Core.AddDrop("Blood Gem of the Archfiend");

        FarmContractExchage("Blood Gem of the Archfiend", quant);
        NewWorldsNewOpportunities("Blood Gem of the Archfiend", quant);
        VoidKnightSwordQuest("Blood Gem of the Archfiend", quant);
        BloodyChaos(quant, true, HydraLevel);
        KisstheVoid(quant);
    }

    public void FarmTaintedGem(int quant = 1000)
    {
        if (Core.CheckInventory("Tainted Gem", quant))
            return;

        Core.AddDrop("Tainted Gem");
        FarmContractExchage("Tainted Gem", quant);
        if (Core.CheckInventory(CragName))
            Supplies("Tainted Gem", quant, ReturnItem: "Tainted Gem");
        ForgeTaintedGems(quant);
        Supplies("Tainted Gem", quant, ReturnItem: "Tainted Gem");
    }

    /// <summary>
    /// Completes the lair questline to unlock Nation mats if not completed.
    /// </summary>
    public void DragonSlayerReward()
    {
        if (Core.isCompletedBefore(169))
            return;

        int[] questIds = { 165, 166, 167, 168, 169 };
        string[] questMonsterNames =
        {
            "Water Draconian",
            "Bronze Draconian",
            "Golden Draconian",
            "Red Dragon",
            "Water Draconian",
        };
        ClassType[] questClasses =
        {
            ClassType.Farm,
            ClassType.Farm,
            ClassType.Farm,
            ClassType.Solo,
            ClassType.Farm,
        };

        for (int i = 0; i < questIds.Length; i++)
        {
            int questId = questIds[i];
            string monsterName = questMonsterNames[i];
            ClassType questClass = questClasses[i];

            // Check if the quest is already completed
            if (Core.isCompletedBefore(questId))
                continue;

            // Equip the required class for the quest
            Core.EquipClass(questClass);
            Core.HuntMonsterQuest(questId, "lair", monsterName);
        }
    }

    /// <summary>
    /// Farms the "Totem of Nulgath" using the most efficient method available:
    /// 1. Checks if the desired quantity is already in inventory.
    /// 2. Uses CragName method if available.
    /// 3. Uses Nulgath's Birthday Gift / Bounty Hunter's Drone Pet method if available.
    /// 4. Falls back to Taro's Manslayer and Essence method if no pets are owned.
    /// </summary>
    /// <param name="quant">Desired quantity of Totems of Nulgath (default: 100, max stack).</param>
    public void FarmTotemofNulgath(int quant = 100)
    {
        // Check if Totem of Nulgath is already in inventory
        if (Core.CheckInventory("Totem of Nulgath", quant))
            return;

        // <CragName> Owned
        Deal(0, quant);

        // Nulgath's Birthday Gift /  Bounty Hunter's Drone Pet Owned
        NewWorldsNewOpportunities("Totem of Nulgath", quant);

        // No Pets Owned
        TotemsViaTaros(quant);

        // This way takes to fucking long... ^ updated method via taro's manslayer and essence.
        // VoucherItemTotemofNulgath(VoucherItemTotem.Totem_of_Nulgath, quant);
    }

    /// <summary>
    /// Farms Totems of Nulgath through Taro’s quest chain (Quest 726).
    /// </summary>
    /// <param name="quant">Desired quantity of Totems of Nulgath (default: 100).</param>
    /// <remarks>
    /// If the player is a member, has less than Rank 10 Good, and lacks the Purified Claymore of Destiny,
    /// the method first ensures prerequisites by farming Good reputation and obtaining the Purified Claymore.
    ///
    /// Steps:
    /// 1. Ensure quest 9541 (Dark Makai unlock) is completed.
    /// 2. Register quest 726 for Totems of Nulgath.
    /// 3. Farm 25 Essence of Nulgath per turn-in.
    /// 4. Obtain "Taro's Manslayer" either by:
    ///    - Completing quest 1111 (member path with Gem of Nulgath + Dark Makai Rune).
    ///    - Hunting Taro Blademasters in Tercessuinotlim (non-member path).
    /// 5. Repeat until desired Totem quantity is obtained.
    /// </remarks>
    public void TotemsViaTaros(int quant = 100)
    {
        if (Core.CheckInventory("Totem of Nulgath", quant))
            return;

        if (Core.IsMember && !Core.CheckInventory("Purified Claymore of Destiny"))
        {
            Core.Logger(
                "Player is a member, we'll use the better farming method for the \"Taro's Manslayer\", first we need to get some prereqs."
            );
            Farm.GoodREP();
            GetPCoD();
        }

        Core.AddDrop("Totem of Nulgath");

        if (!Bot.Quests.HasBeenCompleted(9541))
            Core.ChainComplete(9541);

        while (!Bot.ShouldExit && !Core.CheckInventory("Totem of Nulgath", quant))
        {
            Core.EnsureAccept(726);
            EssenceofNulgath(25);
            if (
                Core.IsMember
                && !Core.CheckInventory(
                    538 /* Taro's Manslayer */
                )
            )
            {
                Core.EnsureAccept(1111);
                FarmGemofNulgath(10);
                Core.ResetQuest(7551);
                Core.DarkMakaiItem("Dark Makai Rune");
                Core.EnsureComplete(
                    1111,
                    538 /* Taro's Manslayer */
                );
                Bot.Wait.ForQuestComplete(1111);
                Bot.Wait.ForPickup(538);
            }
            else
            {
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster(
                    "tercessuinotlim",
                    "Taro Blademaster",
                    "Taro's Manslayer",
                    isTemp: false
                );
            }
            Core.EnsureComplete(726);
            Bot.Wait.ForQuestComplete(726);
            Bot.Wait.ForPickup("Totem of Nulgath");
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farms Gems of Nulgath and/or Totems of Nulgath via the CragName method (Quest 4777).
    /// </summary>
    /// <param name="GemQuant">Number of Gems of Nulgath to farm (0 = skip).</param>
    /// <param name="TotemQuant">Number of Totems of Nulgath to farm (0 = skip).</param>
    public void Deal(int GemQuant = 0, int TotemQuant = 0)
    {
        if (!Core.CheckInventory(CragName))
        {
            return;
        }

        DragonSlayerReward(); // required
        GetPCoD();
        Core.AddDrop("Gem of Nulgath", "Totem of Nulgath");

        void FarmItem(string itemName, int quant, int rewardID)
        {
            if (quant <= 0)
                return;

            Core.FarmingLogger(itemName, quant);
            while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quant))
            {
                Core.EnsureAccept(4777);
                Supplies("Unidentified 3", ReturnItem: "Blood Gem of the Archfiend");
                FarmBloodGem(2);
                FarmUni10(30);
                Core.EnsureComplete(4777, rewardID);
                Bot.Wait.ForPickup(itemName);
            }
        }

        FarmItem("Gem of Nulgath", GemQuant, 6136);
        FarmItem("Totem of Nulgath", TotemQuant, 5357);
    }

    /// <summary>
    /// Acquires the Purified Claymore of Destiny by completing quest 548.
    /// </summary>
    /// <remarks>
    /// Requirements: Level 15 and Good Reputation Rank 8.
    /// Steps:
    /// 1. Accept quest 548.
    /// 2. Hunt Undead Berserkers in 'battleundera' for the Warrior Claymore Blade.
    /// 3. Complete the quest to obtain the Purified Claymore of Destiny.
    /// </remarks>
    public void GetPCoD()
    {
        if (Core.CheckInventory("Purified Claymore of Destiny"))
            return;

        Core.AddDrop("Purified Claymore of Destiny");

        Farm.Experience(15);
        Farm.GoodREP(8);

        Core.EnsureAccept(548);
        Core.HuntMonster(
            "battleundera",
            "Undead Berserker",
            "Warrior Claymore Blade",
            isTemp: false
        );
        Core.EnsureComplete(548);
    }

    /// <summary>
    /// Do Bloody Chaos quest for Blood Gems.
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack.</param>
    /// <param name="relic">Indicates if Relic of Chaos supplies are used.</param>
    /// <param name="HydraLevel"></param>
    public void BloodyChaos(int quant = 100, bool relic = false, int HydraLevel = 85)
    {
        if (Core.CheckInventory("Blood Gem of the Archfiend", quant) || Bot.Player.Level < 80)
            return;

        Core.AddDrop("Blood Gem of the Archfiend", "Hydra Scale Piece");
        if (relic)
            Core.AddDrop(BloodyChaosSupplies);

        Core.FarmingLogger("Blood Gem of the Archfiend", quant);

        Core.RegisterQuests(relic ? new[] { 7816, 2857 } : new[] { 7816 });
        Core.EquipClass(ClassType.Solo);
        while (!Bot.ShouldExit && !Core.CheckInventory("Blood Gem of the Archfiend", quant))
        {
            Core.KillEscherion("Escherion's Helm", isTemp: false);
            Core.KillVath("Shattered Legendary Sword of Dragon Control", isTemp: false);
            Core.HuntMonster(
                "hydrachallenge",
                $"Hydra Head {HydraLevel}",
                "Hydra Scale Piece",
                200,
                false
            );
        }

        Core.CancelRegisteredQuests();
    }

    public void CragsThirst(int quant = 1000)
    {
        if (
            !Core.CheckInventory(CragName)
            || Core.CheckInventory("Diamond of Nulgath", quant)
            || !Core.CheckInventory(Uni(10), 100)
        )
            return;

        Bot.Log("Doing crags thirst");

        while (
            !Bot.ShouldExit
            && Core.CheckInventory(Uni(10), 100)
            && !Core.CheckInventory("Diamond of Nulgath", quant)
        )
        {
            Core.ResetQuest(600);
            Core.EnsureAccept(600);
            Core.DarkMakaiItem("Dark Makai Rune");
            Core.EnsureComplete(600);
            Bot.Wait.ForQuestComplete(600);
            Bot.Wait.ForPickup("Diamond of Nulgath");
        }
    }

    /// <summary>
    /// Do Kiss the Void quest for Blood Gems.
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack</param>
    /// <param name="betrayalBlade"></param>
    /// <param name="KeepVoucher"></param>
    public void KisstheVoid(int quant = 100, string? betrayalBlade = null, bool KeepVoucher = false)
    {
        if (
            betrayalBlade == null
                ? Core.CheckInventory("Blood Gem of the Archfiend", quant)
                : Core.CheckInventory(betrayalBlade)
        )
            return;

        Core.AddDrop(
            betrayalBlade ?? "Tendurrr The Assistant",
            "Fragment of Chaos",
            "Blood Gem of the Archfiend",
            "Broken Betrayal Blade"
        );
        Core.EquipClass(ClassType.Farm);

        if (betrayalBlade == null)
            Core.FarmingLogger("Blood Gem of the Archfiend", quant);
        else
            Core.FarmingLogger(betrayalBlade, 1);

        //warning for idiots that wont read it
        Core.Logger(
            "if Swindles is enabled, it will only accept the quest when it has the required Unis it needs"
        );

        bool returnPolicyDuringSupplies =
            Core.CBOBool("Nation_ReturnPolicyDuringSupplies", out bool _returnSupplies)
            && _returnSupplies == true;

        Core.Logger(
            $"Do Return Policy?: {returnPolicyDuringSupplies}\n"
                + $"Sell Voucher of Nulgath: {sellMemVoucher}"
        );

        Core.AddDrop(
            SuppliesRewards
                .Concat(
                    sellMemVoucher
                        ? new string[] { "Voucher of Nulgath" }
                        : Enumerable.Empty<string>()
                )
                .Append("Relic of Chaos")
                .Concat(
                    returnPolicyDuringSupplies
                        ? new string[]
                        {
                            Uni(1),
                            Uni(6),
                            Uni(9),
                            Uni(16),
                            Uni(20),
                            "Receipt of Swindle",
                        }
                        : Enumerable.Empty<string>()
                )
                .ToArray()
        );

        Core.RegisterQuests(2857);

        while (
            !Bot.ShouldExit
            && (
                betrayalBlade == null
                    ? !Core.CheckInventory("Blood Gem of the Archfiend", quant)
                    : !Core.CheckInventory(betrayalBlade)
            )
        )
        {
            Core.EnsureAccept(3743);

            if (!Core.CheckInventory("Tendurrr The Assistant"))
            {
                Core.KillMonster(
                    "tercessuinotlim",
                    "m2",
                    "Left",
                    "*",
                    "Tendurrr The Assistant",
                    isTemp: false,
                    log: false
                );
                Core.JumpWait();
            }

            Core.KillMonster("blindingsnow", "r17", "Left", "*", "Fragment of Chaos", 80, false);
            Core.KillMonster(
                "evilwarnul",
                "r13",
                "Left",
                "Legion Fenrir",
                "Broken Betrayal Blade",
                8,
                false
            );
            Core.EnsureComplete(3743);
            Bot.Wait.ForQuestComplete(3743);

            string itemToPickup = betrayalBlade ?? "Blood Gem of the Archfiend";
            Bot.Wait.ForDrop(itemToPickup);
            Bot.Wait.ForPickup(itemToPickup);

            SellVoucherOfNulgath(KeepVoucher || sellMemVoucher, itemToPickup);

            // if `Blood Gem of the Archfiend` isnt max stack, do the quest if enabled.
            if (!Core.CheckInventory("Blood Gem of the Archfiend", 100))
                DoSwindlesReturnArea(returnPolicyDuringSupplies, "Blood Gem of the Archfiend");
            if (
                Core.CheckInventory("Voucher of Nulgath (non-mem)")
                && Core.CheckInventory("Essence of Nulgath", 60)
                && (
                    !Core.CheckInventory("Diamond of Nulgath", 1000)
                    || !Core.CheckInventory("Totem of Nulgath", 100)
                )
            )
            {
                Core.EnsureCompleteMulti(
                    4778,
                    !Core.CheckInventory("Diamond of Nulgath", 1000) ? 1000 : 100,
                    !Core.CheckInventory("Diamond of Nulgath", 1000) ? 6136 : 5357
                );
            }

            if (betrayalBlade == null)
            {
                if (Bot.Inventory.IsMaxStack(itemToPickup))
                    Core.Logger("Max Stack Hit.");
                else
                    Core.FarmingLogger(itemToPickup, quant);
            }
        }
    }

    /// <summary>
    /// Farms Gemstone Receipt of Nulgath with specific quantities.
    /// </summary>
    /// <param name="quant">Desired quantity of Gemstone Receipt of Nulgath</param>
    public void GemStoneReceiptOfNulgath(int quant = 10)
    {
        const int demandingApprovalQuest = 4917;
        const int receiptOfNulgathQuest = 4924;
        const int receiptItemId = 33451;

        if (!Core.IsMember)
        {
            Core.Logger("This quest requires membership to be able to accept it.");
            return;
        }

        if (Core.CheckInventory("Gemstone Receipt of Nulgath", quant))
            return;

        Core.AddDrop("Gemstone Receipt of Nulgath", "Receipt of Nulgath");

        while (!Bot.ShouldExit && !Core.CheckInventory("Gemstone Receipt of Nulgath", quant))
        {
            Core.EnsureAccept(demandingApprovalQuest);

            FarmUni13(3);
            Farm.VampireREP();

            if (!Core.CheckInventory(receiptItemId))
            {
                DwoboCoin(100);
                Core.BuyItem("crashedruins", 1212, receiptItemId);
            }

            Core.EnsureAccept(receiptOfNulgathQuest);
            ApprovalAndFavor(0, 100);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("Extinction", "Control Panel", "Coal", 15, isTemp: false, log: false);
            DwoboCoin(10);
            EssenceofNulgath(10);
            Farm.Gold(1500000);
            Core.BuyItem("Tercessuinotlim", 68, "Blade of Affliction");
            Core.EnsureComplete(receiptOfNulgathQuest);
            Bot.Wait.ForPickup("Receipt of Nulgath");

            FarmVoucher(true, true);
            FarmVoucher(false, true);
            EssenceofNulgath(100);
            FarmTotemofNulgath(1);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "ShadowfallWar",
                "Bonemuncher",
                "Ultimate Darkness Gem",
                5,
                isTemp: false
            );
            Core.EnsureComplete(demandingApprovalQuest);
            Bot.Wait.ForPickup("Gemstone Receipt of Nulgath");
        }
    }

    /// <summary>
    /// Farms Dwobo Coins with the specified quest and quantity.
    /// </summary>
    /// <param name="quant">Desired quantity of Dwobo Coins</param>
    public void DwoboCoin(int quant)
    {
        if (Core.CheckInventory("Dwobo Coin", quant))
            return;

        Core.FarmingLogger("Dwobo Coin", quant);
        Core.RegisterQuests(Core.IsMember ? 4798 : 4797);
        Core.AddDrop("Dwobo Coin");

        while (!Bot.ShouldExit && !Core.CheckInventory("Dwobo Coin", quant))
        {
            int unluckyExplorerCount = Core.IsMember ? 8 : 10;
            int spacetimeAnomalyCount = Core.IsMember ? 5 : 7;

            Core.KillMonster(
                "crashruins",
                "r2",
                "Left",
                "Unlucky Explorer",
                "Ancient Treasure",
                unluckyExplorerCount,
                log: false
            );
            Core.KillMonster(
                "crashruins",
                "r2",
                "Left",
                "Spacetime Anomaly",
                "Pieces of Future Tech",
                spacetimeAnomalyCount,
                log: false
            );
            Core.HuntMonster("crashruins", "Cluckmoo Idol", "Idol Heart", log: false);
        }

        Bot.Wait.ForPickup("Dwobo Coin");
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farm Gemstones of Nulgath for specified quantities
    /// </summary>
    /// <param name="bloodStone">Desired quantity of Bloodstone of Nulgath</param>
    /// <param name="quartz">Desired quantity of Quartz of Nulgath</param>
    /// <param name="tanzanite">Desired quantity of Tanzanite of Nulgath</param>
    /// <param name="uniGemStone">Desired quantity of Unidentified Gemstone of Nulgath</param>
    public void GemStonesOfnulgath(
        int bloodStone = 100,
        int quartz = 100,
        int tanzanite = 100,
        int uniGemStone = 1
    )
    {
        const int gemstonesForNulgathQuest = 4918;
        const int skeletalWarriorQuest1 = 374;
        const int skeletalWarriorQuest2 = 375;
        const int boneTerrorQuest = 376;
        // const int unidentifiedWeaponQuest = 377;

        if (!Core.CheckInventory("Gemstone of Nulgath") && !Core.IsMember)
        {
            Core.Logger(
                "This quest requires you to have Gemstone of Nulgath and membership to be able to accept it."
            );
            return;
        }

        FarmUni13(1);
        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 4");

        #region Prerequisites
        // Ensure required quests are unlocked
        if (!Core.isCompletedBefore(boneTerrorQuest))
        {
            Core.EquipClass(ClassType.Farm);
            if (!Core.isCompletedBefore(skeletalWarriorQuest1))
            {
                Core.EnsureAccept(skeletalWarriorQuest1);
                Core.HuntMonster("battleundera", "Skeletal Warrior", "Yara's Ring");
                Core.EnsureComplete(skeletalWarriorQuest1);
            }

            if (!Core.isCompletedBefore(skeletalWarriorQuest2))
            {
                Core.EnsureAccept(skeletalWarriorQuest2);
                Core.HuntMonster("battleundera", "Skeletal Warrior", "Skeletal Claymore", 6);
                Core.HuntMonster("battleundera", "Skeletal Warrior", "Bony Chestplate", 3);
                Core.EnsureComplete(skeletalWarriorQuest2);
            }

            if (!Core.isCompletedBefore(boneTerrorQuest))
            {
                Core.EnsureAccept(boneTerrorQuest);
                Core.HuntMonster("battleundera", "Bone Terror", "Bone Terror's Head");
                Core.EnsureComplete(boneTerrorQuest);
            }
        }
        #endregion Prerequisites

        Core.AddDrop(
            "Gem of Nulgath",
            "Bloodstone of Nulgath",
            "Quartz of Nulgath",
            "Tanzanite of Nulgath",
            "Unidentified Gemstone of Nulgath"
        );

        while (
            !Bot.ShouldExit
            && (
                !Core.CheckInventory("Bloodstone of Nulgath", bloodStone)
                || !Core.CheckInventory("Quartz of Nulgath", quartz)
                || !Core.CheckInventory("Tanzanite of Nulgath", tanzanite)
                || !Core.CheckInventory("Unidentified Gemstone of Nulgath", uniGemStone)
            )
        )
        {
            Core.EnsureAccept(gemstonesForNulgathQuest);

            // get other 3 quest items
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("Twilight", "Abaddon", "Abbadon's Abide", isTemp: false);
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("ShadowfallWar", "Garden1", "Bottom", "*", "Ultimate Darkness Gem", isTemp: false);
            Core.KillMonster("battleundera", "r3", "Left", "*", "Unidentified Weapon", isTemp: false);
            Core.EnsureComplete(gemstonesForNulgathQuest);
        }
    }

    /// <summary>
    /// [Member] Does Forge Tainted Gems for Nulgath [Quest] to get You Tainted Gems with your specific quantities
    /// </summary>
    /// <param name="quant">Desired quantity of Tainted Gems</param>
    public void ForgeTaintedGems(int quant = 1000)
    {
        const int forgeTaintedGemsQuest = 4919;

        if (!Core.CheckInventory("Gemstone of Nulgath") || !Core.IsMember)
        {
            Core.Logger(
                "This quest requires you to have Gemstone of Nulgath and membership to be able to accept it."
            );
            return;
        }

        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 5");

        Core.AddDrop("Tainted Gem", "Unidentified Gemstone of Nulgath");

        while (!Bot.ShouldExit && !Core.CheckInventory("Tainted Gem", quant))
        {
            Core.EnsureAccept(forgeTaintedGemsQuest);
            FarmGemofNulgath(1);
            GemStonesOfnulgath(0, 3, 1, 0);
            Core.EnsureComplete(forgeTaintedGemsQuest);
        }
    }

    /// <summary>
    /// [Member] Forges Dark Crystal Shards for Nulgath [Quest] to obtain Dark Crystal Shards with specific quantities.
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack</param>
    public void ForgeDarkCrystalShards(int quant = 1000)
    {
        if (!Core.CheckInventory("Gemstone of Nulgath") && !Core.IsMember)
        {
            Core.Logger(
                "This quest requires you to have Gemstone of Nulgath and membership to be able to accept it"
            );
            return;
        }

        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 5");

        Core.AddDrop("Dark Crystal Shards", "Unidentified Gemstone of Nulgath");

        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Crystal Shards", quant))
        {
            // Forge Dark Crystal Shards for Nulgath [Member] 4920
            Core.EnsureAccept(4920);
            FarmGemofNulgath(1);
            GemStonesOfnulgath(0, 5, 2, 0);
            Core.EnsureComplete(4920);
        }
    }

    /// <summary>
    /// [Member] Forges Diamonds for Nulgath [Quest] to obtain Diamonds for Nulgath with specific quantities.
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack.</param>
    public void ForgeDiamondsOfNulgath(int quant = 1000)
    {
        if (!Core.CheckInventory("Gemstone of Nulgath") && !Core.IsMember)
        {
            Core.Logger(
                "This quest requires you to have Gemstone of Nulgath and membership to be able to accept it."
            );
            return;
        }

        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 5");

        Core.AddDrop("Diamonds for Nulgath", "Unidentified Gemstone of Nulgath");

        while (!Bot.ShouldExit && !Core.CheckInventory("Diamonds for Nulgath", quant))
        {
            // Forge Diamonds for Nulgath [Member] 4921
            Core.EnsureAccept(4921);
            FarmGemofNulgath(1);
            GemStonesOfnulgath(0, 2, 0, 0);
            Core.EnsureComplete(4921);
        }
    }

    /// <summary>
    /// [Member] Forges Blood Gems for Nulgath [Quest] to obtain Blood Gem of the Archfiend with specific quantities.
    /// </summary>
    /// <param name="quant">Desired quantity, 100 = max stack.</param>
    public void ForgeBloodGems(int quant = 100)
    {
        if (!Core.CheckInventory("Gemstone of Nulgath") && !Core.IsMember)
        {
            Core.Logger(
                "This quest requires you to have Gemstone of Nulgath and membership to be able to accept it"
            );
            return;
        }

        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 5");

        Core.AddDrop("Blood Gem of the Archfiend", "Unidentified Gemstone of Nulgath");

        while (!Bot.ShouldExit && !Core.CheckInventory("Blood Gem of the Archfiend", quant))
        {
            // Forge Blood Gems for Nulgath [Member] 4922
            Core.EnsureAccept(4922);
            FarmGemofNulgath(7);
            GemStonesOfnulgath(3, 5, 0, 0);
            Core.EnsureComplete(4922);
        }
    }

    /// <summary>
    /// [Member] Carves a Uni Gemstone [Quest] to obtain specific items.
    /// </summary>
    /// <param name="item">Desired item name.</param>
    /// <param name="quant">Desired item quantity.</param>
    public void CarveUniGemStone(string? item = null, int quant = 1000)
    {
        string[] questDrops =
        {
            "Tainted Gem",
            "Dark Crystal Shard",
            "Diamond of Nulgath",
            "Gem of Nulgath",
            "Blood Gem of the Archfiend",
        };

        // Check if the player is a member and has the desired items or item.
        if (!Core.IsMember)
        {
            Core.Logger("This quest requires you to have Gemstone of Nulgath and membership to be able to accept it");
            return;
        }

        if (
            (item == null && Core.CheckInventory(questDrops, quant))
            || (item != null && Core.CheckInventory(item, quant))
        )
            return;

        // Required items
        Core.KillMonster(
            "tercessuinotlim",
            "m4",
            "Right",
            "Shadow of Nulgath",
            "Hadean Onyx of Nulgath",
            isTemp: false
        );
        GemStoneReceiptOfNulgath(1);
        Supplies("Unidentified 5");

        if (item != null)
            Core.AddDrop(item);
        else
            Core.AddDrop(questDrops);

        while (!Bot.ShouldExit && (item == null || !Core.CheckInventory(item, quant)))
        {
            // Carve the Unidentified Gemstone [Member] 4923
            Core.EnsureAccept(4923);
            Core.HuntMonster("WillowCreek", "Hidden Spy", "The Secret 1", isTemp: false);
            FarmGemofNulgath(7);
            GemStonesOfnulgath(1, 3, 1, 1);

            static int GetItemIdByName(string? itemName) =>
                itemName switch
                {
                    "Dark Crystal Shard" => 4770,
                    "Diamond of Nulgath" => 4771,
                    "Gem of Nulgath" => 6136,
                    "Blood Gem of the Archfiend" => 22332,
                    "Tainted Gem" => 4769,
                    _ => -1,
                };

            int itemId = GetItemIdByName(item);
            if (itemId != -1)
            {
                Core.EnsureComplete(4923, itemId);
            }
            else
            {
                Core.EnsureCompleteChoose(4923); // Complete the quest without specifying item ID
            }

            if (item != null)
                Core.Logger(
                    Bot.Inventory.IsMaxStack(item)
                        ? "Max Stack Hit."
                        : $"{item}: {Bot.Inventory.GetQuantity(item)}/{quant}"
                );
        }
    }

    /// <summary>
    /// Farms gold through Leery Contract exchange.
    /// </summary>
    /// <param name="quant">Desired gold quantity.</param>
    public void LeeryExchangeGold(int quant = 100000000)
    {
        // Check if the player is a member or already has the desired gold quantity.
        if (!Core.IsMember || Bot.Player.Gold >= quant)
            return;

        // Core.DebugLogger(this);
        // Add Unidentified 13 to the drops list.
        Core.AddDrop(Uni(13));

        // Core.DebugLogger(this);
        // Toggle Gold Boost and register the required quest.
        Farm.ToggleBoost(BoostType.Gold);
        // Core.DebugLogger(this);

        // Core.DebugLogger(this);
        // Continue farming until the desired gold quantity is reached.
        while (!Bot.ShouldExit && Bot.Player.Gold < quant)
        {
            // Core.DebugLogger(this);
            // Farm Unidentified 13 for the exchange.
            FarmUni13(13);
            // Core.DebugLogger(this);

            // Hunt the specified monster to exchange Unidentified 13 for gold.
            while (!Bot.ShouldExit && Core.CheckInventory(Uni(13)))
            {
                Core.EnsureAccept(554);
                Core.KillMonster(
                    "underworld",
                    "r2",
                    "up",
                    "Undead Legend",
                    "Undead Legend Rune",
                    log: false
                );
                Core.EnsureComplete(554);
            }
        }

        // Cancel the registered quest and disable Gold Boost.
        Core.CancelRegisteredQuests();
        Farm.ToggleBoost(BoostType.Gold, false);
    }

    /// <summary>
    /// Hires Nulgath Larvae.
    /// </summary>
    public void HireNulgathLarvae()
    {
        // Check if Nulgath Larvae is already in inventory or the player is not a member.
        if (Core.CheckInventory("Nulgath Larvae") || !Core.IsMember)
            return;

        // Add Nulgath Larvae to the drops list.
        Core.AddDrop("Nulgath Larvae");

        // Accept the required quest.
        Core.EnsureAccept(867);

        // Farm the required vouchers for the quest.
        FarmVoucher(true, true);

        // Hunt the specified monster to complete the quest.
        Core.HuntMonster("underworld", "Undead Legend", "Undead Legend Rune", log: false);

        // Ensure the quest is completed and wait for the pet pickup.
        Core.EnsureComplete(867);
        Bot.Wait.ForPickup("Nulgath Larvae");
    }

    /// <summary>
    /// Swindles Bilk method
    /// </summary>
    /// <param name="item">Desired item name</param>
    public void SwindlesBilk(string item)
    {
        if (string.IsNullOrEmpty(item))
        {
            throw new ArgumentException($"'{nameof(item)}' cannot be null or empty.", nameof(item));
        }

        string[] rPDSuni = new[] { Uni1(1), Uni1(6), Uni1(9), Uni1(16), Uni1(20) };
        Core.AddDrop(rPDSuni);
        Core.AddDrop("Blood Gem of the Archfiend");
    }

    private static string Uni1(int nr) => $"Unidentified {nr}";

    /// <summary>
    /// Farms Voucher of Nulgath (member or not) with the best method available
    /// </summary>
    /// <param name="member">If true will farm Voucher of Nulgath; false Voucher of Nulgath (nom-mem)</param>
    /// <param name="KeepVoucher"></param>
    public void FarmVoucher(bool member, bool KeepVoucher = false)
    {
        if (
            (Core.CheckInventory("Voucher of Nulgath (non-mem)") && !member)
            || (Core.CheckInventory("Voucher of Nulgath") && member)
        )
            return;

        Core.AddDrop(member ? "Voucher of Nulgath" : "Voucher of Nulgath (non-mem)");
        Core.Logger($"KeepVoucher set to {KeepVoucher}");
        if (hasOBoNPet || Core.CheckInventory(CragName))
            BambloozevsDrudgen(
                member ? "Voucher of Nulgath" : "Voucher of Nulgath (non-mem)",
                KeepVoucher: KeepVoucher
            );
        NewWorldsNewOpportunities(member ? "Voucher of Nulgath" : "Voucher of Nulgath (non-mem)");
        VoidKnightSwordQuest(member ? "Voucher of Nulgath" : "Voucher of Nulgath (non-mem)");
        Supplies(
            member ? "Voucher of Nulgath" : "Voucher of Nulgath (non-mem)",
            KeepVoucher: KeepVoucher
        );
    }

    /// <summary>
    /// Farms Tainted Gems using Dreadrock Gem Exchange quest.
    /// </summary>
    /// <param name="quant">The quantity of Tainted Gems to farm.</param>
    public void DreadrockGemExchange(int quant = 1000)
    {
        if (Core.CheckInventory("Tainted Gem", quant))
            return;

        FarmUni13(1);

        Core.AddDrop("Tainted Gem");

        Core.EquipClass(ClassType.Farm);

        Core.FarmingLogger("Tainted Gem", quant);

        Core.RegisterQuests(4853);
        while (!Bot.ShouldExit && !Core.CheckInventory("Tainted Gem", quant))
        {
            Core.KillMonster("dreadrock", "r3", "Bottom", "*", log: false);
        }
        Core.CancelRegisteredQuests();
    }

    public void BloodFromTheVoid(int quant = 300)
    {
        if (Core.CheckInventory("Blood From the Void", quant, toInv: false))
            return;

        if (!Core.isCompletedBefore(10581))
        {
            Core.Logger("This farm requires the story in /tercesinvasion to be completed. if your geting this message please run the `Story\\Nation\\TercesInvasion.cs` script for the story, the rerun w/e is farming this item.");
            return;
        }

        Core.AddDrop("Blood From the Void");
        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(Bot.Player.IsMember ? 10583 : 10582);
        Core.FarmingLogger("Blood From the Void", quant);
        while (!Bot.ShouldExit && !Core.CheckInventory("Blood From the Void", quant))
        {
            Core.HuntMonster("tercesinvasion", "Archfiend Rodeleros", "Rodeleros' Blade Shard", log: false);
            Core.HuntMonster("tercesinvasion", "Archfiend Vigneron", "Vigneron's Chalice", log: false);
            Core.HuntMonster("tercesinvasion", "Archfiend Casimir", "Casimir's Pinky", log: false);
        }
        Core.CancelRegisteredQuests();
    }


    // private int GetQuestRewardMaxStack(string itemName, int QuestID = 555) =>
    //     Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(QuestID), 20)
    //         .Rewards.FirstOrDefault(x => x != null && x.Name == itemName)?.MaxStack ?? 0;
}

public enum ChooseReward
{
    Tainted_Gem = 4769,
    Dark_Crystal_Shard = 4770,
    Diamond_of_Nulgath = 4771,
    Gem_of_Nulgath = 6136,
    Blood_Gem_of_the_Archfiend = 22332,
    Totem_of_Nulgath = 5357,
}

public enum ContractExchangeRewards
{
    Tainted_Gem = 4769,
    Dark_Crystal_Shard = 4770,
    Diamond_of_Nulgath = 4771,
    Gem_of_Nulgath = 6136,
    Blood_Gem_of_the_Archfiend = 22332,
    All = 9999,
}

public enum SwindlesReturnReward
{
    Tainted_Gem = 4769,
    Dark_Crystal_Shard = 4770,
    Diamond_of_Nulgath = 4771,
    Gem_of_Nulgath = 6136,
    Blood_Gem_of_the_Archfiend = 22332,
    None = 0,
};

public enum VoucherItemTotem
{
    Totem_of_Nulgath = 5357,
    Gem_of_Nulgath = 6136,
}

public enum HydraLevel
{
    Hydra_Head_85,
    Hydra_Head_90,
}
