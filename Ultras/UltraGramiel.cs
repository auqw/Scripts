/*
name: UltraGramiel
description: Ultra Gramiel - auto-assigns role by class: SC/IT→Left T1, LC→Left T2, LOO→Right T1, VDK→Right T2. Off-comp classes use the "Custom Role" dropdown.
tags: ultra, gramiel, Ultra Gramiel
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

#region Recommended Comp

/// <summary>
/// Recommended
/// </summary>
// LEFT CRYSTAL (MapID 2) - T1 & T2 Taunters
// ──────────────────────────────────────────
// StoneCrusher / Infinity Titan (T1)
// ├─ Class: Fighter
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// ├─ Cape: Absolution
// └─ Potions: Sage Tonic + Crusader Elixir
// ArchPaladin (T2)
// ├─ Class: Lucky
// ├─ Helm: Lucky
// ├─ Weapon: Awe Blast
// ├─ Cape: Penitence
// └─ Potions: Potent Malevolence Elixir + Sage Tonic
//
// RIGHT CRYSTAL (MapID 3) - T1 & T2 Taunters
// ──────────────────────────────────────────
// Lord of Order (T1)
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Arcanas Concerto / Valiance / Awe Blast
// ├─ Cape: Penitence
// └─ Potions: Divine Elixir + Sage Tonic
// Void Highlord (T2)
// ├─ Class: Lucky
// ├─ Helm: Lucky
// ├─ Weapon: Valiance
// ├─ Cape: Lament
// └─ Potions: Potent Battle Elixir + Sage Tonic

#endregion

#region Alternate Comp

/// <summary>
/// Alternate
/// </summary>
// LEFT CRYSTAL (MapID 2) - T1 & T2 Taunters
// ──────────────────────────────────────────
// StoneCrusher (T1)
// ├─ Class: Fighter
// ├─ Helm: Wizard
// ├─ Weapon: Valiance
// ├─ Cape: Absolution
// └─ Potions: Sage Tonic + Crusader Elixir
// LightCaster (T2)
// ├─ Class: Lucky
// ├─ Helm: Forge / Pneuma
// ├─ Weapon: Valiance / Awe Blast
// ├─ Cape: Penitence / Lament
// └─ Potions: Potent Malevolence Elixir + Sage Tonic
// RIGHT CRYSTAL (MapID 3) - T1 & T2 Taunters
// ──────────────────────────────────────────
// Lord of Order (T1)
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Arcanas / Valiance / Awe Blast
// ├─ Cape: Penitence / Absolution
// └─ Potions: Divine Elixir + Sage Tonic
// Verus DoomKnight (T2)
// ├─ Class: Lucky
// ├─ Helm: Anima / Forge
// ├─ Weapon: Valiance / Ravenous
// ├─ Cape: Vainglory / Lament
// └─ Potions: Potent Battle Elixir + Sage Tonic

#endregion

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
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraGramiel";

    // Gramiel warning tracking
    private int tauntCounter = 0;
    private DateTime lastTauntWarningTime = DateTime.MinValue;
    private bool shouldExecuteTaunt = false;

    // Gramiel boss taunt timer (4 rotation, 5 seconds per taunt)
    private DateTime gramielFightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds = 0;
    private const double TauntIntervalSeconds = 20.0; // Full 4-taunt cycle
    private const double TauntWindowSeconds = 4.0; // Window to execute taunt

    // Player role assignment (determined once during prep)
    private int crystalMapId = 2;
    private bool isT1Taunter = false;
    private int crystalDeathCount = 0;

    public List<IOption> Options = new()
    {
        new Option<GramielComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Recommended: SC / IT, LoO, AP, VHL\n"
                + "Alternate: SC / IT, LC, LOO, VDK\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            GramielComp.Unselected
        ),
        new Option<CustomRole>(
            "CustomRole",
            "Custom Role",
            "Used if you are not using a pre-defined comp. Pick which slot you're filling.",
            CustomRole.Unselected
        ),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
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

        Core.Boot();
        Adv.GearStore(EnhAfter: true);

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

        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
        Bot.StopSync();
    }

    void Prep(bool skipEnhancements = false)
    {
        // Sync-equip classes if a comp is selected
        GramielComp comp = Bot.Config!.Get<GramielComp>("DoEquipClasses");
        if (comp != GramielComp.Unselected)
        {
            string[][] classes = comp switch
            {
                GramielComp.Recommended => new[] {
                    new[] { "StoneCrusher", "Infinity Titan" },
                    new[] { "ArchPaladin" },
                    new[] { "Lord of Order" },
                    new[] { "Void Highlord" }
                },
                GramielComp.Alternate => new[] {
                    new[] { "StoneCrusher", "Infinity Titan" },
                    new[] { "LightCaster" },
                    new[] { "Lord of Order" },
                    new[] { "Verus DoomKnight" }
                },
                _ => throw new NotImplementedException(),
            };

            Ultra.EquipClassSync(classes, 4, "gramiel_class.sync");
        }

        if (Bot.Config!.Get<bool>("DoEnh") && !skipEnhancements)
            DoEnhs();

        // Auto-detect role from equipped class; fall back to dropdown for off-meta
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        switch (className)
        {
            case "StoneCrusher":
            case "Infinity Titan":
                crystalMapId = 2;
                isT1Taunter = true;
                break;
            case "LightCaster":
            case "ArchPaladin":
                crystalMapId = 2;
                isT1Taunter = false;
                break;
            case "Lord of Order":
                crystalMapId = 3;
                isT1Taunter = true;
                break;
            case "Verus DoomKnight":
            case "Void Highlord":
                crystalMapId = 3;
                isT1Taunter = false;
                break;
            default:
                // Off-comp class — use the "Custom Role" dropdown
                var role = Bot.Config!.Get<CustomRole>("CustomRole");
                if (role == CustomRole.Unselected)
                {
                    C.Logger($"Your class \"{className}\" isn't auto-mapped. Please select a Custom Role in the options.", "Fix This", true, true);
                    return;
                }
                switch (role)
                {
                    case CustomRole.LeftCrystalT1:
                        crystalMapId = 2; isT1Taunter = true; break;
                    case CustomRole.LeftCrystalT2:
                        crystalMapId = 2; isT1Taunter = false; break;
                    case CustomRole.RightCrystalT1:
                        crystalMapId = 3; isT1Taunter = true; break;
                    case CustomRole.RightCrystalT2:
                        crystalMapId = 3; isT1Taunter = false; break;
                }
                C.Logger($"Off-comp class \"{className}\" — using Custom Role: {role}");
                break;
        }

        C.Logger($"Assigned to crystal ID '{crystalMapId}' as {(isT1Taunter ? "T1" : "T2")}");

        // Calculate taunt offset for timer-based rotation
        // Left T1: 0s, Left T2: 5s, Right T1: 10s, Right T2: 15s
        if (crystalMapId == 2 && isT1Taunter)
            tauntOffsetSeconds = 0;
        else if (crystalMapId == 2 && !isT1Taunter)
            tauntOffsetSeconds = 5;
        else if (crystalMapId == 3 && isT1Taunter)
            tauntOffsetSeconds = 10;
        else // Right T2
            tauntOffsetSeconds = 15;

        C.Logger($"Gramiel taunt offset: {tauntOffsetSeconds}s");

        // Potions based on equipped class
        switch (className)
        {
            case "StoneCrusher":
            case "Infinity Titan":
                Ultra.UseAlchemyPotions("Sage Tonic", "Crusader Elixir");
                break;
            case "LightCaster":
            case "ArchPaladin":
                Ultra.UseAlchemyPotions("Potent Malevolence Elixir", "Sage Tonic");
                break;
            case "Lord of Order":
                Ultra.UseAlchemyPotions("Divine Elixir", "Sage Tonic");
                break;
            case "Verus DoomKnight":
            case "Void Highlord":
                Ultra.UseAlchemyPotions("Potent Battle Elixir", "Sage Tonic");
                break;
            default:
                Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
                break;
        }

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
        // WHITEMAP STAGING
        // ---------------------------
        Core.Join("whitemap");
        Bot.Wait.ForMapLoad("whitemap");

        // Wait for army to gather
        Ultra.WaitForArmy(3, "UltraItemCheck.sync");
        Bot.Sleep(1500);

        // ---------------------------
        // MAP SETUP
        // ---------------------------
        C.EnsureAccept(10301);
        C.AddDrop("Gramiel the Graceful Vanquished");

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_gramiel.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell("*");
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
                tauntCounter = 0;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;

                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Ultra.ClearSyncFile(syncPath);
                Bot.Sleep(2500);
                Prep(skipEnhancements: true);
                Ultra.WaitForArmy(3, "UltraItemCheck.sync");

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Ultra.WaitForArmy(3, "ultra_gramiel.sync");
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
                tauntCounter = 0;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;

                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Prep(skipEnhancements: true);
                Ultra.WaitForArmy(3, "ultra_gramiel.sync");

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Core.ChooseBestCell("*");
                Bot.Player.SetSpawnPoint();
                Core.EnableSkills();
                continue;
            }

            // Dead during Gramiel phase → just respawn

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Gramiel the Graceful Vanquished", 1), syncPath))
            {
                Core.DisableSkills();
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(10301))
                    C.EnsureComplete(10301);
                break;
            }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(1000);
                continue;
            }

            // ---------------------------
            // CRYSTAL & BOSS COMBAT
            // ---------------------------
            AttackWithPriority();
            Bot.Sleep(250);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player.CurrentClass?.Name.ToLower() ?? string.Empty;

        if (string.IsNullOrEmpty(className))
            return;

        // Enhance based on currently equipped class
        switch (className)
        {
            // StoneCrusher / Infinity Titan
            case "stonecrusher":
            case "infinity titan":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Lord of Order
            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // LightCaster
            case "lightcaster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // Verus DoomKnight
            case "verus doomknight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "void highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                Adv.SmartEnhance(Bot.Player.CurrentClass!.Name);
                break;
        }
    }

    void AttackWithPriority()
    {
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        int gramielMapId = 1; // Gramiel MapID

        // Execute taunt if flagged (synchronously, blocking other actions)
        if (shouldExecuteTaunt)
        {
            shouldExecuteTaunt = false;
            Core.DisableSkills();
            Bot.Sleep(500);

            C.Logger($"{className} executing taunt #{tauntCounter}!");

            int attempts = 0;
            bool tauntLanded = false;
            while (!Bot.ShouldExit && attempts < 15)
            {
                if (!Bot.Player.Alive)
                    break;

                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack(crystalMapId); // Re-target crystal

                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);

                Bot.Sleep(500);
                attempts++;

                // Check if Focus aura appeared (taunt landed)
                if (Bot.Player.HasTarget && Bot.Target?.Auras?.Any(a => a?.Name == "Focus") == true)
                {
                    C.Logger($"Taunt #{tauntCounter} landed after {attempts} attempts");
                    tauntLanded = true;
                    Bot.Sleep(500);
                    break;
                }
            }

            if (!tauntLanded)
            {
                C.Logger($"WARNING: Taunt #{tauntCounter} FAILED after {attempts} attempts!");
                if (Bot.Player.HasTarget && Bot.Target?.Auras != null)
                {
                    string auraNames = string.Join(", ", Bot.Target.Auras.Select(a => a?.Name ?? "null"));
                    C.Logger($"Target auras present: {auraNames}");
                }
            }

            Core.EnableSkills();
            Bot.Sleep(300);
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

            // Timer-based taunt rotation (staggered 5-second intervals)
            TimeSpan timeSinceFightStart = DateTime.Now - gramielFightStartTime;
            double currentTime = timeSinceFightStart.TotalSeconds;
            double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;

            // Check if we're in our taunt window (0-4 seconds into our slot)
            bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
            bool noFocusAura = Bot.Player.HasTarget &&
                               (Bot.Target?.Auras?.Any(a => a?.Name == "Focus") != true);

            if (inTauntWindow && noFocusAura)
            {
                Core.DisableSkills();
                Bot.Sleep(500);

                C.Logger($"Gramiel taunt window ({currentTime:F1}s into fight, offset {tauntOffsetSeconds}s)");

                int attempts = 0;
                while (!Bot.ShouldExit && attempts < 15)
                {
                    if (!Bot.Player.Alive)
                        break;

                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(gramielMapId);

                    if (Bot.Skills.CanUseSkill(5))
                        Bot.Skills.UseSkill(5);

                    Bot.Sleep(500);
                    attempts++;

                    if (Bot.Player.HasTarget && Bot.Target?.Auras?.Any(a => a?.Name == "Focus") == true)
                    {
                        C.Logger("Gramiel taunt landed - Focus aura detected!");
                        Bot.Sleep(500);
                        break;
                    }
                }

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
                    // Debounce: Ignore if we received this message within the last 2 seconds -- avoid double taunts
                    TimeSpan timeSinceLastWarning = DateTime.Now - lastTauntWarningTime;
                    if (timeSinceLastWarning.TotalSeconds < 2)
                    {
                        return;
                    }

                    lastTauntWarningTime = DateTime.Now;
                    tauntCounter++;
                    C.Logger($"Crystal attack warning detected! (Taunt #{tauntCounter})");

                    // Determine if this player should taunt
                    // T1 taunters taunt on odd counts (1, 3, 5...)
                    // T2 taunters taunt on even counts (2, 4, 6...)
                    bool shouldTaunt = (isT1Taunter && tauntCounter % 2 == 1) ||
                                       (!isT1Taunter && tauntCounter % 2 == 0);

                    if (shouldTaunt)
                    {
                        C.Logger($"Taunt #{tauntCounter} assigned to {(isT1Taunter ? "T1" : "T2")}");
                        shouldExecuteTaunt = true;
                    }
                }
            }
        }
        catch { }
    }

    public enum GramielComp
    {
        Unselected,
        Recommended,
        Alternate
    }

    public enum CustomRole
    {
        Unselected,
        LeftCrystalT1,
        LeftCrystalT2,
        RightCrystalT1,
        RightCrystalT2,
    }
}
