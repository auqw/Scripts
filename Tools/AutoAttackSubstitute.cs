/*
name: Auto Attack Substitute
description: as per the name, this is a substitute for the currently ~wack~ auto-atk/hunt button in teh menu-bar.
tags: auto-attack, auto, attack, auto attack, why are you attacking manualy?, wack, i hate tags, no wonder this didn't push
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class AutoAttackSubstitute
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public string OptionsStorage = "AutoAttackSubstitute";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<AttackMode>(
            "AttackMode",
            "Choose how to attack",
            "\"Attack All In Cell\" - kills everything/whats specified in the cell, \"Hunt Across Map\" - will hunt the specified mob across the map.",
            AttackMode.Attack_All_In_Cell
        ),
        new Option<string>(
            "Monsters",
            "Mobs to Attack",
            "Fill in the Monsters that the bot Attack (if left blank it will attack all mobs within the current cell.) Split them with a , (comma). CAPITALS & EXACT spelling matter! You can go to tools > grabber > Inventory/Bank > grab to get their EXACT name",
            ""
        ),
        new Option<string>(
            "AddDrops",
            "drops To Pickup",
            "Fill in the Items that the bot should Pickup. Split them with a , (comma). CAPITALS & EXACT spelling matter! You can go to tools > grabber > Inventory/Bank > grab to get their EXACT name",
            ""
        ),
        new Option<int>(
            "QuestToAccept",
            "Quest To Accept",
            "Specify a quest ID to accept before starting the script.",
            0000
        ),
        new Option<int>(
            "QuestReward",
            "Reward To Pick",
            "Quest Reward to pick ( by ItemID)",
            0000
        ),
        new Option<int>(
            "RewardAmount",
            "stop when reward quant owned",
            "when you've reached xxx of reward > stop script.",
            0000
        ),
        new Option<ClassType>(
            "classType",
            "Class Type",
            "This uses the farm or solo class set in [Options] > [CoreBots]",
            ClassType.Solo
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        AASubstitute();

        Core.SetOptions(false);
    }

    public void AASubstitute()
    {
        //Test To See if this goes through.. prayge
        AttackMode attackMode =
            Bot.Config?.Get<AttackMode>("AttackMode") ?? AttackMode.Attack_All_In_Cell;

        string[] dropsToPickupArray =
            Bot.Config?.Get<string>("DropsToPickup")?.Split(',')?.ToArray()
            ?? Array.Empty<string>();

        int questToAccept = Bot.Config!.Get<int>("QuestsToAccept");
        int questReward = Bot.Config!.Get<int>("QuestReward");
        int Rewardquant = Bot.Config!.Get<int>("RewardAmount");

        string? monstersString = Bot.Config?.Get<string>("Monsters");

        string[] monstersArray = !string.IsNullOrEmpty(monstersString)
            ? monstersString.Split(',').Select(m => m.Trim()).ToArray()
            : Array.Empty<string>();


        Core.AddDrop(dropsToPickupArray);

        Core.EquipClass(
            Bot.Config?.Get<ClassType>("classType") == ClassType.Solo
                ? ClassType.Solo
                : ClassType.Farm
        );
        Bot.Player.SetSpawnPoint();

        foreach (AttackMode mode in Enum.GetValues(typeof(AttackMode)))
        {
            if (Bot.Config!.Get<AttackMode>("AttackMode") != mode)
                continue;

            switch (mode)
            {
                case AttackMode.Attack_All_In_Cell:
                    Bot.Options.AggroMonsters = true;
                    Core.Logger(
                        monstersArray.Length == 0
                            ? "Attacking: **EVERYTHING IN THIS CELL**"
                            : $"Aggro: enabled\nAttacking:\n{string.Join("\n", monstersArray.Select(m => $"\"{m}\""))}"
                    );


                    // Continuously attack monsters in the cell until bot should exit
                    while (!Bot.ShouldExit && !Core.CheckInventory(questReward, Rewardquant))
                    {
                        Core.EnsureAccept(questToAccept);
                        // If monstersArray is empty, attack all monsters in the cell
                        if (monstersArray.Length == 0)
                        {
                            Bot.Combat.Attack("*");
                            Bot.Sleep(500);

                        }
                        else
                        {
                            // If monstersArray is not empty, attack only specified monsters in the cell
                            foreach (
                                Monster mob in Bot.Monsters.CurrentAvailableMonsters.Where(m =>
                                    m?.Name != null
                                    && m.Cell == Bot.Player.Cell
                                    && monstersArray.Contains(m.Name)
                                )
                            )
                            {
                                if (mob.Name == null)
                                    continue;
                                Bot.Kill.Monster(mob);
                            }
                        }
                        if (Bot.Quests.CanComplete(questToAccept))
                            Core.EnsureComplete(questToAccept, Rewardquant);
                    }

                    // Reset AggroMonsters option and perform additional actions
                    Bot.Options.AggroMonsters = false;
                    Core.JumpWait();
                    break;

                case AttackMode.Hunt_Across_Map:
                    if (monstersArray.Length == 0)
                        Core.Logger(
                            "No monsters specified to hunt across the entire map.",
                            stopBot: true
                        );
                    else
                        Core.Logger(
                            $"Hunting:\n{string.Join("\n", monstersArray.Select(m => $"\"{m}\""))} across the entire map"
                        );

                    // Continuously hunt monsters across the entire map until bot should exit
                    while (!Bot.ShouldExit && !Core.CheckInventory(questReward, Rewardquant))
                    {
                        Core.EnsureAccept(questToAccept);

                        // Check if monstersArray is null or empty
                        if (monstersArray == null || monstersArray.Length == 0)
                        {
                            Core.Logger("Please Fill in the \"Monsters\" option.");
                            return;
                        }
                        else
                        {
                            foreach (
                                Monster mob in Bot.Monsters.MapMonsters.Where(m =>
                                    m?.Name != null && monstersArray.Contains(m.Name)
                                )
                            )
                            {
                                if (mob.Name == null)
                                    continue;

                                while (!Bot.ShouldExit && !Core.CheckInventory(questReward, Rewardquant))
                                {
                                    if (!Bot.Player.Alive)
                                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                                    if (Bot.Player.Cell != mob.Cell)
                                    {
                                        Core.Jump(mob.Cell, "Left");
                                        Bot.Wait.ForCellChange(mob.Cell);
                                    }

                                    Bot.Combat.Attack(mob);
                                    Core.Sleep();

                                }
                            }

                            if (Bot.Quests.CanComplete(questToAccept))
                                Core.EnsureComplete(questToAccept, Rewardquant);
                        }
                    }
                    // Break out of the loop when bot should exit
                    break;
            }
        }
    }

    public enum AttackMode
    {
        Attack_All_In_Cell,
        Hunt_Across_Map,
    }
}
