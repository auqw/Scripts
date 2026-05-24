/*
name: UltraGramiel
description: Ultra Gramiel - configure which class fills each of the 4 crystal roles (Left T1, Left T2, Right T1, Right T2) directly in the options.
tags: ultra, gramiel, Ultra Gramiel
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraPotions.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraEnhancements.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraGramiel
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    private static UltraPotions Pots
    {
        get => _Pots ??= new UltraPotions();
        set => _Pots = value;
    }
    private static UltraPotions _Pots;

    private static UltraEnhancements Enh
    {
        get => _Enh ??= new UltraEnhancements();
        set => _Enh = value;
    }
    private static UltraEnhancements _Enh;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraGramiel-v2";

    // Gramiel warning tracking
    private bool t1Turn = true; // true = T1 taunts, false = T2 taunts
    private DateTime lastTauntWarningTime = DateTime.MinValue;
    private volatile bool shouldExecuteTaunt = false;
    private readonly System.Threading.ManualResetEventSlim _tauntSignal = new(false);

    // Gramiel boss taunt timer (4 rotation, 5 seconds per taunt)
    private DateTime gramielFightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 16.0; // Full 4-taunt cycle (4 x 4s slots)
    private const double TauntWindowSeconds = 4.0; // Window to execute taunt
    private DateTime lastTauntTime = DateTime.MinValue;

    // Player role assignment (determined once during prep)
    private int crystalMapId = 2;
    private bool isT1Taunter = false;
    private int taunterIndex = -1; // -1 = not a taunter, 0/1/2 = slot
    private int crystalDeathCount = 0;

    public List<IOption> Options = new()
    {
        new Option<string>(
            "LeftCrystalT1",
            "Left Crystal — T1 Taunter",
            "Class name for the Left Crystal T1 taunter (taunts on odd warnings: 1, 3, 5...).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "StoneCrusher"
        ),
        new Option<string>(
            "LeftCrystalT2",
            "Left Crystal — T2 Taunter",
            "Class name for the Left Crystal T2 taunter (taunts on even warnings: 2, 4, 6...).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "ArchPaladin"
        ),
        new Option<string>(
            "RightCrystalT1",
            "Right Crystal — T1 Taunter",
            "Class name for the Right Crystal T1 taunter (taunts on odd warnings: 1, 3, 5...).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "Lord Of Order"
        ),
        new Option<string>(
            "RightCrystalT2",
            "Right Crystal — T2 Taunter",
            "Class name for the Right Crystal T2 taunter (taunts on even warnings: 2, 4, 6...).\n"
                + "Name must be exact including punctuation, spelling, and capitalisation.",
            "Void Highlord"
        ),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<int>("PotionQuantity", "Potion Quantity", "How many potions to keep stocked.", 10),
        new Option<int>("ArmySize", "Army Size", "How many players are in your army (including yourself).", 4),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage("Ultra Gramiel", 
            "This is a technical fight requiring optimal enhancements and classes.\n"
                + "Recommended comp: SC / IT, LoO, AP, VHL.\n"
                + "Alternate comp: SC / IT, LC, LOO, VDK.\n"
                + "The crystal phase is RNG and deaths will occur.\n"
                + "If you are not prepared, please do not run this script.",
            true,
            true
        );

        Bot.Options.LagKiller = true;
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        C.SetOptions();
        Core.Boot();
        
        // Register packet handler for Gramiel warnings
        Bot.Events.ExtensionPacketReceived += GramielMessageListener;
        
        try
        {
            Prep();
            Fight();
        }
        finally
        {
            // Unregister packet handler
            Bot.Events.ExtensionPacketReceived -= GramielMessageListener;
        }
        
        C.SetOptions(false);
        Bot.StopSync();
    }

    void Prep(bool skipEnhancements = false)
    {
        if (Bot.Config!.Get<bool>("DoEnh") && !skipEnhancements)
            DoEnhs();

        // Read role assignments from options
        string leftT1Class  = Bot.Config!.Get<string>("LeftCrystalT1").Trim();
        string leftT2Class  = Bot.Config!.Get<string>("LeftCrystalT2").Trim();
        string rightT1Class = Bot.Config!.Get<string>("RightCrystalT1").Trim();
        string rightT2Class = Bot.Config!.Get<string>("RightCrystalT2").Trim();

        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;

        if (className.Equals(leftT1Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isT1Taunter = true;
        }
        else if (className.Equals(leftT2Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 2;
            isT1Taunter = false;
        }
        else if (className.Equals(rightT1Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isT1Taunter = true;
        }
        else if (className.Equals(rightT2Class, StringComparison.OrdinalIgnoreCase))
        {
            crystalMapId = 3;
            isT1Taunter = false;
        }
        else
        {
            C.Logger(
                $"Your class \"{className}\" doesn't match any of the 4 configured slots.\n"
                    + $"  Left T1: {leftT1Class}\n"
                    + $"  Left T2: {leftT2Class}\n"
                    + $"  Right T1: {rightT1Class}\n"
                    + $"  Right T2: {rightT2Class}\n"
                    + "Please update the options to include your class.",
                "Fix This", true, true
            );
            return;
        }

        C.Logger($"Assigned to crystal ID '{crystalMapId}' as {(isT1Taunter ? "T1" : "T2")} (class: {className})");

        // Calculate taunt offset for timer-based rotation
        // Left T1: 0s, Left T2: 4s, Right T1: 8s, Right T2: 12s
        if (crystalMapId == 2 && isT1Taunter)
            tauntOffsetSeconds = 0;
        else if (crystalMapId == 2 && !isT1Taunter)
            tauntOffsetSeconds = 4;
        else if (crystalMapId == 3 && isT1Taunter)
            tauntOffsetSeconds = 8;
        else // Right T2
            tauntOffsetSeconds = 12;

        C.Logger($"Gramiel taunt offset: {tauntOffsetSeconds}s");

        // Potions — use best available for now
        int potionQuant = Bot.Config!.Get<int>("PotionQuantity");
        Pots.UseRecommendedPotions(potionQuant, skipThird: true);

        Ultra.GetScrollOfEnrage();
        Bot.Sleep(2500);
    }

    void Fight()
    {
        const string map = "ultragramiel";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // ---------------------------
        // MAP SETUP
        // ---------------------------
        C.EnsureAccept(10301);
        C.AddDrop("Gramiel the Graceful Vanquished");
        
        Core.Join(map);
        int armySize = Math.Max(1, Bot.Config!.Get<int>("ArmySize"));
        Ultra.WaitForArmy(armySize - 1, "ultra_gramiel.sync");
        Core.ChooseBestCell("*");
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        // ---------------------------
        // MAIN COMBAT LOOP
        // ---------------------------
        while (!Bot.ShouldExit)
        {
            bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
                .Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

            // player death during crystal phase
            if (!Bot.Player.Alive && anyCrystalAlive)
            {
                crystalDeathCount++;
                C.Logger($"Death during crystal phase ({crystalDeathCount}/2)");
                while (!Bot.Player.Alive && !Bot.ShouldExit)
                    Bot.Sleep(500);
                Bot.Sleep(250);
                
                // 1st death: respawn and continue fighting
                if (crystalDeathCount < 2)
                {
                    continue;
                }
                
                // 2nd death: leave room and restart
                Core.DisableSkills();
                C.Logger("2nd crystal phase death — leaving room to restart and avoid desync.");
                t1Turn = true;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;
                
                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Ultra.ClearSyncFile(syncPath);
                Bot.Sleep(2500);
                Prep(skipEnhancements: true);

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Ultra.WaitForArmy(armySize - 1, "ultra_gramiel.sync");
                Core.ChooseBestCell("*");
                Bot.Player.SetSpawnPoint();
                Core.EnableSkills();
                continue;
            }

            // restart if any army member is missing -- catches deaths during crystal phase that would cause desync
            if (Bot.Map.PlayerCount < 3)
            {
                Core.DisableSkills();
                C.Logger("Army member missing (during crystal phase?) - restarting!");
                t1Turn = true;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;
                
                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Prep(skipEnhancements: true);

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Ultra.WaitForArmy(armySize - 1, "ultra_gramiel.sync");
                Core.ChooseBestCell("*");
                Bot.Player.SetSpawnPoint();
                Core.EnableSkills();
                continue;
            }

            // Dead during Gramiel phase → just respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(1000);
                continue;
            }

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Gramiel the Graceful Vanquished", 1), syncPath))
            {
                Core.DisableSkills();
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(10301))
                    C.EnsureComplete(10301);
                Ultra.JoinHouse();
                break;
            }

            // ---------------------------
            // CRYSTAL & BOSS COMBAT
            // ---------------------------
            AttackWithPriority();
            Pots.ActivateEquippedPotion();
            // Wait up to 250ms, but wake immediately if a taunt is signalled
            _tauntSignal.Wait(250);
        }
    }

    void DoEnhs() => Enh.Apply();

    void AttackWithPriority()
    {
        int gramielMapId = 1; // Gramiel MapID

        // Execute crystal taunt if signalled by the packet listener
        if (shouldExecuteTaunt)
        {
            shouldExecuteTaunt = false;
            _tauntSignal.Reset();

            string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
            C.Logger($"Taunt assigned to {(isT1Taunter ? "T1" : "T2")} — executing on main thread");

            if (Bot.Player.Alive && !Bot.ShouldExit)
            {
                Core.DisableSkills();
                Bot.Combat.Attack(crystalMapId);
                C.Logger($"{className} executing crystal taunt!");

                for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                {
                    if (!Bot.Player.Alive)
                        break;
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(25);
                }

                C.Logger($"Crystal taunt done.");
                Core.EnableSkills();
            }
            return;
        }

        // Check if primary crystal is alive
        bool primaryCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
            .Any(x => x != null && x.Alive && x.MapID == crystalMapId);

        // If primary crystal is dead, switch to the other crystal
        int targetCrystalMapId = crystalMapId;
        if (!primaryCrystalAlive)
        {
            int otherCrystalMapId = crystalMapId == 2 ? 3 : 2;
            bool otherCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
                .Any(x => x != null && x.Alive && x.MapID == otherCrystalMapId);

            if (otherCrystalAlive)
            {
                targetCrystalMapId = otherCrystalMapId;
                C.Logger($"Primary crystal down! Switching to other crystal (MapID {otherCrystalMapId})");
            }
        }
        else
        {
            // Balance check — if our crystal is ahead by > 30 HP, switch to help the other side
            int otherCrystalMapId = crystalMapId == 2 ? 3 : 2;
            int myHP = Bot.Monsters.CurrentAvailableMonsters
                .Where(x => x != null && x.Alive && x.MapID == crystalMapId)
                .Select(x => x.HP)
                .FirstOrDefault();
            int otherHP = Bot.Monsters.CurrentAvailableMonsters
                .Where(x => x != null && x.Alive && x.MapID == otherCrystalMapId)
                .Select(x => x.HP)
                .FirstOrDefault();

            // Only switch if the other crystal is alive and our crystal is ahead by > 30
            if (otherHP > 0 && myHP > 0 && myHP < otherHP - 30)
            {
                targetCrystalMapId = otherCrystalMapId;
            }
        }

        bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters
            .Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

        if (anyCrystalAlive)
        {
            // Attack crystal
            Bot.Combat.Attack(targetCrystalMapId);
        }
        else
        {
            if (gramielFightStartTime == DateTime.MinValue)
            {
                gramielFightStartTime = DateTime.Now;
                C.Logger("Both crystals down! Starting Gramiel fight timer.");
            }

            // No crystal - attack Gramiel with timer-based taunts
            Bot.Combat.Attack(gramielMapId);

            // Timer-based taunt rotation (staggered 4-second intervals)
            TimeSpan timeSinceFightStart = DateTime.Now - gramielFightStartTime;
            double currentTime = timeSinceFightStart.TotalSeconds;
            double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;
            bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
            bool cooldownExpired = (DateTime.Now - lastTauntTime).TotalSeconds >= TauntIntervalSeconds - 1;

            if (inTauntWindow && cooldownExpired && Bot.Player.HasTarget)
            {
                Core.DisableSkills();
                Bot.Combat.Attack(gramielMapId);
                C.Logger($"Gramiel taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s)");

                for (int i = 0; i < 40 && !Bot.ShouldExit; i++)
                {
                    if (!Bot.Player.Alive)
                        break;
                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(gramielMapId);
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(25);
                }

                C.Logger("Gramiel taunt done.");
                lastTauntTime = DateTime.Now;
                Core.EnableSkills();
                Bot.Sleep(300);
            }
        }
    }

    // Packet Handler for Gramiel Warning Messages
    private void GramielMessageListener(dynamic packet)
    {
        try
        {
            string type = packet["params"].type;
            if (type is not "json")
                return;
            
            if (!Bot.Player.Alive)
                return;
            
            dynamic data = packet["params"].dataObj;
            string cmd = data.cmd.ToString();
            
            if (cmd != "ct")
                return;
            
            // Check for messages in anims array (boss messages appear here)
            if (data.anims is null)
                return;
            
            foreach (dynamic anim in data.anims)
            {
                if (anim is null || anim.msg is null)
                    continue;
                
                string message = (string)anim.msg;
                
                // Check for crystal defense shattering attack warning
                if (message.Contains("The Grace Crystal prepares a defense shattering attack!", StringComparison.OrdinalIgnoreCase))
                {
                    // Debounce: both crystals fire the same message ~100ms apart.
                    // 3000ms catches the duplicate while still allowing rapid real warnings
                    // at low crystal HP when the interval between warnings shrinks.
                    TimeSpan timeSinceLastWarning = DateTime.Now - lastTauntWarningTime;
                    if (timeSinceLastWarning.TotalMilliseconds < 3000)
                        return;

                    lastTauntWarningTime = DateTime.Now;

                    // Flip whose turn it is: T1 first, then T2, then T1, etc.
                    bool currentlyT1Turn = t1Turn;
                    t1Turn = !t1Turn;

                    C.Logger($"Crystal attack warning! ({(currentlyT1Turn ? "T1" : "T2")} turn)");

                    // This player taunts if their role matches the current turn
                    bool shouldTaunt = (isT1Taunter && currentlyT1Turn) ||
                                       (!isT1Taunter && !currentlyT1Turn);

                    if (shouldTaunt)
                    {
                        shouldExecuteTaunt = true;
                        _tauntSignal.Set();
                    }
                }
            }
        }
        catch { }
    }

}
