/*
name: Grimgaol
description: Goes through the Grimgaol dungeon... Testing Phase
tags: grimgoal, dungeon, why, did, we, make, this, Testing, WIP, beta
*/

#region includes
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/MergeShops/InfernalArenaMerge.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/DoomVault.cs
//cs_include Scripts/Story/DoomVaultB.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalArena.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Yokai.cs
#endregion includes

using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Options;
using Skua.Core.Utils;

public class Grimgaol
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static DoomVaultB DVB
    {
        get => _DVB ??= new DoomVaultB();
        set => _DVB = value;
    }
    private static DoomVaultB _DVB;
    private static InfernalArenaMerge InfernalArena
    {
        get => _InfernalArena ??= new InfernalArenaMerge();
        set => _InfernalArena = value;
    }
    private static InfernalArenaMerge _InfernalArena;
    private static J6Saga J6
    {
        get => _J6 ??= new J6Saga();
        set => _J6 = value;
    }
    private static J6Saga _J6;
    private static UnlockForgeEnhancements Forge
    {
        get => _Forge ??= new UnlockForgeEnhancements();
        set => _Forge = value;
    }
    private static UnlockForgeEnhancements _Forge;

    readonly Stopwatch runTimer = new();
    int RunCount = 0;

    // ── Split Timer State ────────────────────────────────────────────────────
    static readonly string Dir =
        Path.Combine(ClientFileSources.SkuaScriptsDIR, "Prototypes");

    static readonly string LogFile =
        Path.Combine(Dir, "GrimGaolRunTimes.txt");

    static TimeSpan _lastSplit;
    static TimeSpan _bestRun = TimeSpan.MaxValue;
    static readonly List<Split> _splits = [];
    static bool _initialized;
    // ────────────────────────────────────────────────────────────────────────

    public string OptionsStorage = "Grimgaol2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        // Skip Options
        CoreBots.Instance.SkipOptions,

        // Skip Enhancements
        new Option<bool>("SkipEnhancements", "Skip Item Enhancing", "If enabled, will not enhance items for the run (this doesnt mean skip enhancements you dont have... these enhancements are **VERY** important).", false ),
        new Option<bool>( "RoomTimers", "Time each room?", "If enabled, this will log the time for each room into chat/the Logs > scripts tab.", false ),
        
        // Helms
        new Option<string>("ForgeHelm", "Helm: Forge", "insert Name of your Forge Helm"),
        new Option<string>("WizHelm", "Helm: Wizard", "insert Name of your Wizard Helm"),
        new Option<string>("HealerHelm", "Helm: HealerHelm", "insert Name of your HealerHelm Helm"),

        //Weapons       
        new Option<string>("Elysium", "Weapon: Elysium", "insert Name of your Elysium Weapon"),

        // Capes
        new Option<string>("Absolution", "Absolution: Cape", "insert Name of your Absolution Cape"),
        new Option<string>("Penitence", "Penitence: Cape", "insert Name of your Penitence Cape"),

    };

    // ── Split Timer Methods ──────────────────────────────────────────────────

    /// <summary>
    /// Load the best run time from disk (runs once per session).
    /// </summary>
    void EnsureInit()
    {
        if (_initialized)
            return;

        Directory.CreateDirectory(Dir);

        if (File.Exists(LogFile))
        {
            foreach (string line in File.ReadLines(LogFile))
            {
                if (TimeSpan.TryParse(line.Trim(), out TimeSpan t) && t < _bestRun)
                    _bestRun = t;
            }
        }

        _initialized = true;
    }

    /// <summary>
    /// Call once at the start of each dungeon run.
    /// Resets the stopwatch and clears previous split data.
    /// </summary>
    void StartRun()
    {
        EnsureInit();

        _splits.Clear();
        _lastSplit = TimeSpan.Zero;

        runTimer.Restart();
    }

    /// <summary>
    /// Record a named checkpoint split based on elapsed combat time.
    /// Only records if RoomTimers is enabled — otherwise silently skips.
    /// </summary>
    void RecordSplit(string name)
    {
        TimeSpan now = runTimer.Elapsed;
        TimeSpan delta = now - _lastSplit;
        _lastSplit = now;

        _splits.Add(new Split { Name = name, Time = delta });

        if (Bot.Config!.Get<bool>("RoomTimers"))
            Core.Logger($"⏱ {name}: {FormatTs(delta)}");
    }

    /// <summary>
    /// Call at the end of a completed run.
    /// Logs total time, compares to PB, appends to file, and prints split breakdown.
    /// </summary>
    void EndRun()
    {
        runTimer.Stop();
        TimeSpan total = runTimer.Elapsed;

        // Append to log file
        File.AppendAllText(LogFile, total.ToString() + Environment.NewLine);

        // PB check
        if (total < _bestRun)
        {
            _bestRun = total;
            Core.Logger($"🔥 NEW PB: {FormatTs(total)}");
        }
        else
        {
            TimeSpan diff = total - _bestRun;
            Core.Logger($"Run complete: {FormatTs(total)} | PB: {FormatTs(_bestRun)} (+{FormatTs(diff)})");
        }

        // Always print split breakdown
        LogSplits(total);
    }

    /// <summary>
    /// Prints the per-room split breakdown and the run total.
    /// </summary>
    void LogSplits(TimeSpan total)
    {
        if (_splits.Count == 0)
            return;

        // Find the slowest split so we can flag it
        TimeSpan slowest = _splits.Max(s => s.Time);

        Core.Logger("────── SPLIT BREAKDOWN ──────");
        foreach (Split s in _splits)
        {
            string flag = s.Time == slowest ? " ◄ slowest" : "";
            Core.Logger($"  {s.Name,-30} {FormatTs(s.Time)}{flag}");
        }
        Core.Logger($"  {"TOTAL",-30} {FormatTs(total)}");
        Core.Logger("─────────────────────────────");
    }

    /// <summary>
    /// Formats a TimeSpan as m:ss.ff — cleaner than the default ToString.
    /// </summary>
    static string FormatTs(TimeSpan t) =>
        $"{(int)t.TotalMinutes}:{t.Seconds:D2}.{t.Milliseconds / 10:D2}";

    // ────────────────────────────────────────────────────────────────────────


    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        DoGrimGaol();

        Core.SetOptions(false);
    }


    public void DoGrimGaol(int rank = 10)
    {
        if (Farm.FactionRank("Grimskull Trolling") >= rank)
        {
            Core.Logger($"You already have rank {rank} Grimskull Trolling reputation.");
            return;
        }
        Core.Logger("Checking prerequisites and configurations...");
        CheckConfig();
        Prereqs();
        Bot.Options.RestPackets = true;
        Core.Logger("Prerequisites and configurations checked successfully.");

        Core.Logger($"Farming rank {rank} Grimskull Trolling reputation.");

        Farm.ToggleBoost(BoostType.Reputation);
        while (!Bot.ShouldExit && Farm.FactionRank("Grimskull Trolling") < rank)
        {
            if (!Bot.Quests.IsDailyComplete(9469) && Core.HasWebBadge("SkullCrusher"))
                Core.EnsureAccept(9469);

            Core.EnsureAccept(
                !Core.HasWebBadge("SkullCrusher") ? 9466 : (Core.IsMember ? 9468 : 9467)
            );

            if (Bot.Map.Name.ToLower() == "grimgaol")
            {
                Core.Logger(
                    "Already in the dungeon (weird flex but ok)... we'll continue from where you left off"
                );
                Init();
            }
            else
            {
                Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%grimgaol-100000%");
                Bot.Wait.ForMapLoad("grimgaol");

                // Incase we're in the Cutscene cell
                if (Bot.Player.Cell != "Enter")
                {
                    Bot.Map.Jump("Enter", "Spawn", autoCorrect: false);
                    Bot.Wait.ForCellChange("Enter");
                }

                Init();
            }
        }

        Core.Logger($"Total Runs Complete: {RunCount}");
        Farm.ToggleBoost(BoostType.Reputation, false);
    }

    private void Init()
    {
        if (Bot.Player.Cell.ToLower().Contains("cut"))
        {
            Core.Logger($"in {Bot.Player.Cell} cell, jumping to enter");
            Bot.Map.Jump("Enter", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("Enter");
        }

        // Start the run timer (resets stopwatch + clears splits)
        StartRun();

        // Stop usage of AdvSkills after story & prereqs. as we'll use our own here.
        while (!Bot.ShouldExit && !Bot.TempInv.Contains("Grimskull's Gaol Cleared"))
        {
            jumpToAvailMonster();
            Bot.Skills.Stop();
            if (Bot.Player != null)
                switch (Bot.Player.Cell)
                {
                    // Grimskull? 
                    case "Enter":
                        RKE(Bot.Player.Cell);
                        RecordSplit("Enter — Grimskull?");
                        if (Bot.Player.Cell != "r2")
                        {
                            Bot.Map.Jump("r2", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r2");
                        }
                        break;

                    // Grim Bomb  
                    case "r2":
                        RLR(Bot.Player.Cell);
                        RecordSplit("r2 — Grim Bomb");
                        if (Bot.Player.Cell != "r3")
                        {
                            Bot.Map.Jump("r3", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r3");
                        }
                        break;

                    // Empress Angler 
                    case "r3":
                        R3();
                        RecordSplit("r3 — Empress Angler");
                        if (Bot.Player.Cell != "r4")
                        {
                            Bot.Map.Jump("r4", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r4");
                        }
                        break;

                    // Treasure Chest 
                    case "r4":
                        RDoT(Bot.Player.Cell);
                        RecordSplit("r4 — Treasure Chest");
                        if (Bot.Player.Cell != "r5")
                        {
                            Bot.Map.Jump("r5", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r5");
                        }
                        break;

                    // Reinforced Shelleton  
                    case "r5":
                        RLR(Bot.Player.Cell);
                        RecordSplit("r5 — Reinforced Shelleton");
                        if (Bot.Player.Cell != "r6")
                        {
                            Bot.Map.Jump("r6", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r6");
                        }
                        break;

                    // Fell Statue 
                    case "r6":
                        RDoT(Bot.Player.Cell);
                        RecordSplit("r6 — Fell Statue");
                        if (Bot.Player.Cell != "r7")
                        {
                            Bot.Map.Jump("r7", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r7");
                        }
                        break;

                    // Emperor Angler 
                    case "r7":
                        RDoT(Bot.Player.Cell);
                        RecordSplit("r7 — Emperor Angler");
                        if (Bot.Player.Cell != "r8")
                        {
                            Bot.Map.Jump("r8", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r8");
                        }
                        break;

                    // Treasure Chest 
                    case "r8":
                        RDoT(Bot.Player.Cell);
                        RecordSplit("r8 — Treasure Chest");
                        if (Bot.Player.Cell != "r9")
                        {
                            Bot.Map.Jump("r9", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r9");
                        }
                        break;

                    // Rick, Grim Soldier 
                    case "r9":
                        RDoT(Bot.Player.Cell);
                        RecordSplit("r9 — Rick, Grim Soldier");
                        if (Bot.Player.Cell != "r10")
                        {
                            Bot.Map.Jump("r10", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r10");
                        }
                        break;

                    // Mechro Lich + Rampaging Cyborg  
                    case "r10":
                        R10();
                        RecordSplit("r10 — Mechro Lich + Cyborg");
                        if (Bot.Player.Cell != "r11")
                        {
                            Bot.Map.Jump("r11", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r11");
                        }
                        break;

                    // Mechabinky & Raxborg 
                    case "r11":
                        RKE(Bot.Player.Cell);
                        RecordSplit("r11 — Mechabinky & Raxborg");
                        if (Bot.Player.Cell != "r12")
                        {
                            Bot.Map.Jump("r12", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r12");
                        }
                        break;

                    // Grimskull 
                    case "r12":
                        RKE(Bot.Player.Cell);
                        RecordSplit("r12 — Grimskull (Final Boss)");
                        if (Bot.Player.Cell != "r12a")
                        {
                            Bot.Map.Jump("r12a", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r12a");
                        }
                        break;

                    default:
                        break;
                }
            else
            {
                Core.Logger("Object Bot.Player is null.");
                break;
            }
            Bot.Sleep(200);
        }

        // End run: log total, compare PB, print splits
        if (Core.CheckInventory("Grimskull's Gaol Cleared"))
        {
            RunCount++;
            Core.Logger($"Runs complete so far: {RunCount}");
            EndRun();
        }

        Core.EnsureComplete(
            !Core.HasWebBadge("SkullCrusher") ? 9466 : (Core.IsMember ? 9468 : 9467)
        );

        if (!Bot.Quests.IsDailyComplete(9469) && Core.HasWebBadge("SkullCrusher"))
            Core.EnsureComplete(9469);

        Core.Join("whitemap-100000");

        Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%Grimgaol-100000%");
        Bot.Wait.ForMapLoad("Grimgaol");
        if (Bot.Player != null)
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);
    }

    #region These are fine
    private void RKE(string cell)
    {
        if (Bot.Player.Cell != cell)
        {
            Core.Logger("jump to enter");
            Bot.Map.Jump("Enter", "Spawn", autoCorrect: false);
            Bot.Wait.ForCellChange("Enter");
        }

        if (Bot.Player.Alive && !monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player.SetSpawnPoint();

        EquipIfAvailable(kingsecho);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("ForgeHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Penitence"));

        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = { 1, 2, 4, 4, 3 };

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                skillIndex = 0;
            }

            if (Bot.Player.Alive && !monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (Bot.Player.Cell != "Enter" && Bot.Player.Cell != "r11" && Bot.Player.Cell != "r12") // lonewolf addition, fix bug causing bot to enter r12 and then back to enter
            {
                Core.Logger("jump back to enter");
                Bot.Map.Jump("Enter", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Enter");
                Bot.Sleep(1000);
            }

            if (Bot.Player.HasTarget && Bot.Target.Auras.Any(x => x.Name == "Talon Twisting") == true)
            {
                Bot.Combat.CancelTarget();
                Bot.Skills.Pause();
                Bot.Sleep(7000); //lonewolf changed, original number was 1000

                Bot.Wait.ForTrue(() => Bot.Player.HasTarget && Bot.Target.HasActiveAura("Retaliate") == true, 20);

                while (!Bot.ShouldExit && Bot.Player.HasTarget && Bot.Target.HasActiveAura("Retaliate") == true)
                    Bot.Sleep(200);

                Bot.Combat.StopAttacking = false;
                skillIndex = 0;
                Bot.Skills.Resume();
                Bot.Sleep(1000);
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
                break;

            // emergency heal (skill 3)
            if (skillIndex == 3 &&
                Bot.Player!.Health <= Bot.Player.MaxHealth / 2 &&
                Bot.Player.Mana > 50 &&
                Bot.Skills.CanUseSkill(3))
                Bot.Skills.UseSkill(3);

            if (Bot.Player?.HasTarget == true
                && Bot.Player.Target?.HP > 0)
            {
                // 1 — Skill 1 : Residual Energy <= 23
                if (Bot.Skills.CanUseSkill(1)
                    && Bot.Self.GetAuraValue("Residual Energy") <= 23)
                {
                    Bot.Skills.UseSkill(1);
                }

                // 2 — Skill 2 : Mana > 25 AND Residual Energy <= 23
                else if (Bot.Skills.CanUseSkill(2)
                    && Bot.Player!.Mana > 25
                    && Bot.Self.GetAuraValue("Residual Energy") <= 23)
                {
                    Bot.Skills.UseSkill(2);
                }

                // 3 — Skill 4 : Residual Energy >= 24
                else if (Bot.Skills.CanUseSkill(4)
                    && Bot.Self.GetAuraValue("Residual Energy") >= 24)
                {
                    Bot.Skills.UseSkill(4);
                }

                // 4 — Skill 4 : Mana <= 24 AND NO Elysium aura
                else if (Bot.Skills.CanUseSkill(4)
                    && Bot.Player!.Mana <= 24
                    && !Bot.Self.HasActiveAura("Elysium"))
                {
                    Bot.Skills.UseSkill(4);
                }

                // 5 — Skill 3 : HP <= 50% AND Mana > 50 AND NO Royal Resolve
                else if (Bot.Skills.CanUseSkill(3)
                    && Bot.Player!.Health <= Bot.Player.MaxHealth * 0.65 //lonewolf changed, original number was 0.5
                    && Bot.Player.Mana > 50
                    && !Bot.Self.HasActiveAura("Royal Resolve"))
                {
                    Bot.Skills.UseSkill(3);
                }

                else if (!Bot.Self.HasActiveAura("Residual Energy"))
                {
                    if (Bot.Skills.CanUseSkill(2))
                        Bot.Skills.UseSkill(2);
                    else
                        if (Bot.Skills.CanUseSkill(1))
                            Bot.Skills.UseSkill(1);
                }
            }
            Bot.Sleep(200);
        }
    }
    //empress room
    private void R3()
    {
        if (Bot.Player.Cell != "r3")
        {
            Core.Logger("jump to r3");
            Bot.Map.Jump("r3", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r3");
        }
        Bot.Sleep(1000);

        if (Bot.Player.Alive && !monsterAvail())
        {
            runTimer.Stop();
            return;
        }
        Bot.Player.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("HealerHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Absolution"));
        #endregion

        runTimer.Start();
        int[] skillList = { 3, 2, 1, 2, 4, 2 };
        int skillIndex = 0;

        while (!Bot.ShouldExit)
        {
            if (Bot.Player.Alive && !monsterAvail())
            {

                runTimer.Stop();
                return;
            }

            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

                skillIndex = 0;
            }

            if (!Bot.Player.HasTarget || !Bot.Player.InCombat)
                Bot.Combat.Attack("*");


            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
            {
                if (Bot.Player.InCombat)
                {
                    Bot.Combat.CancelTarget();
                }

                continue;
            }

            //aura check 
            bool hasConcealedBlade = Bot.Player.HasTarget && Bot.Target?.Auras.Any(x => x != null && x.Name == "Concealed Blade") == true;
            bool hasBottomFeeder = Bot.Player.HasTarget && Bot.Target?.Auras.Any(x => x != null && x.Name == "Bottom Feeder") == true;
            if (hasConcealedBlade && !hasBottomFeeder)
            {
                Core.DebugLogger(this, "EXECUTE DETECTED!!! HEAL UP!!!");

                if (Bot.Skills.CanUseSkill(2))
                    Bot.Skills.UseSkill(2);

                Bot.Sleep(250);
                continue;
            }


            UseSkillRotation(skillList, ref skillIndex);
            Bot.Sleep(100);

        }
    }


    // 3x Lich room
    private void R10()
    {
        if (Bot.Player.Cell != "r10")
        {
            Core.Logger("jump to r10");
            Bot.Map.Jump("r10", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r10");
        }
        Bot.Sleep(1000);

        if (Bot.Player.Alive && !monsterAvail())
        {
            runTimer.Stop();
            return;
        }
        Bot.Player.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("WizHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Absolution"));
        #endregion

        runTimer.Start();
        int[] skillList = { 3, 2, 1, 2, 4, 2 };
        int skillIndex = 0;

        string monsId = "16";
        int monsIdInt = 16;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                skillIndex = 0;
            }

            if (Bot.Player.Alive && !monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (
                monsId != "*"
                && Bot.Monsters.MapMonsters.Any(x =>
                    x != null
                    && x?.Cell == Bot.Player.Cell
                    && x?.MapID.ToString() == monsId
                    && (x?.HP <= 70000 || x?.HP <= 50000 || x?.HP <= 20000)
                )
            )
            {
                switch (monsId)
                {
                    case "16":
                        monsId = "17";
                        monsIdInt = 17;
                        break;
                    case "17":
                        monsId = "18";
                        monsIdInt = 18;
                        break;
                    case "18":
                        monsId = "*";
                        break;
                }
            }

            if (monsId == "*")
                doPriorityAttackId(new int[] { 16, 17, 18, 19 });
            else
                doPriorityAttackId(new int[] { monsIdInt });


            UseSkillRotation(skillList, ref skillIndex);
            Bot.Sleep(100);
        }
    }

    bool UseWizHelm;
    private void RDoT(string cell, bool statues = false, CancellationToken cancellationToken = default)
    {
        // Jump to cell if needed
        if (Bot.Player.Cell != cell)
        {
            Core.Logger($"Jumping to \"{cell}\"");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        string[] WizHelmCells = new[] { "r4", "r7", "r9", "r10", "r12" };
        if (WizHelmCells.Contains(Bot.Player.Cell))
            UseWizHelm = true;

        if (Bot.Player.Alive && !monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>(UseWizHelm ? "WizHelm" : "HealerHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Absolution"));
        #endregion
        runTimer.Start();
        Bot.Skills.Stop();

        int skillIndex = 0;
        int[] skillList = new[] { 3, 2, 1, 2, 4, 2 };
        while (!Bot.ShouldExit)
        {
            if (Bot.Player.Alive && !monsterAvail())
            {
                runTimer.Stop();

                return;
            }

            foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
            {
                if (m == null || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                    continue;

                while (!Bot.ShouldExit)
                {
                    if (Bot.Player.Alive && !monsterAvail())
                    {
                        runTimer.Stop();
                        if (Bot.Player.Cell == "r3")
                            return;
                    }

                    if (!Bot.Player!.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        skillIndex = 0;
                    }


                    if (!Bot.Player.HasTarget || !Bot.Player.InCombat)
                        Bot.Combat.Attack(m!.MapID);

                    // Empress Angler Aura Checks
                    if (Bot.Player.Cell == "r3")
                    {
                        while (!Bot.ShouldExit)
                        {
                            if (!Bot.Player.HasTarget || !Bot.Player.InCombat)
                                Bot.Combat.Attack(m!.MapID);

                            if (Bot.Player.Health < Bot.Player.MaxHealth)
                            {
                                if (Bot.Skills.CanUseSkill(2))
                                    Bot.Skills.UseSkill(2);
                                Bot.Sleep(250);
                            }
                            else
                            {
                                UseSkillRotation(skillList, ref skillIndex);
                            }
                        }
                    }

                    if (Bot.Player.Target?.HP <= 0 || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                    {
                        if (Bot.Player.InCombat)
                            Bot.Combat.CancelTarget();
                        break;
                    }

                    UseSkillRotation(skillList, ref skillIndex);

                    Bot.Sleep(100);
                }
            }
        }
    }

    private void UseSkillRotation(int[] skillList, ref int skillIndex)
    {
        if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
        {
            int skill = skillList[skillIndex];

            if (Bot.Skills.CanUseSkill(skill))
            {
                Bot.Skills.UseSkill(skill);
                skillIndex = (skillIndex + 1) % skillList.Length;

            }
        }
    }

    private void RLR(string cell)
    {
        // Jump to cell if needed
        if (Bot.Player.Cell != cell)
        {
            Core.Logger($"Jumping to \"{cell}\"");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (Bot.Player.Alive && !monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(legionrevenant);
        EquipIfAvailable(Bot.Config!.Get<string>("ForgeHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("Penitence"));
        #endregion
        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = new[] { 3, 4, 1, 2 };

        while (!Bot.ShouldExit)
        {
            if (Bot.Player.Alive && !monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
            {
                if (m == null || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                    continue;

                while (!Bot.ShouldExit)
                {
                    if (Bot.Player.Alive && !monsterAvail())
                    {
                        runTimer.Stop();
                        return;
                    }

                    if (!Bot.Player!.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        skillIndex = 0;
                    }

                    if (!Bot.Player.HasTarget || !Bot.Player.InCombat)
                        Bot.Combat.Attack(m!.MapID);

                    if (Bot.Player.Target?.HP <= 0 || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                    {
                        if (Bot.Player.InCombat)
                            Bot.Combat.CancelTarget();
                        break;
                    }
                    //not using UseSkillRotation on purpose so that every skill is used instead of waiting for cooldowns
                    if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
                    {
                        Bot.Skills.UseSkill(skillList[skillIndex]);
                        skillIndex = (skillIndex + 1) % skillList.Length;
                    }
                    Bot.Sleep(100);
                }
            }
        }
    }

    #endregion

    public void EquipIfAvailable(string? itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        if (!Core.CheckInventory(itemName))
        {
            Core.Logger($"Item not found in inventory or bank: \"{itemName}\"", "EquipIfAvailable");
            return;
        }

        // Equip repeatedly until equipped or script exits
        while (!Bot.ShouldExit && !Bot.Inventory.IsEquipped(itemName))
        {
            Bot.Inventory.EquipItem(itemName);
            Bot.Wait.ForActionCooldown(GameActions.EquipItem);
            Core.Sleep();
        }
    }

    private void doPriorityAttackId(int[] monsterListId)
    {
        foreach (int id in monsterListId)
            if (monsterAvailId(id))
            {
                Bot.Combat.Attack(id);
                return;
            }
    }

    private bool monsterAvailId(int id)
    {
        if (
            Bot.Monsters.MapMonsters.Any(x =>
                x != null
                && x.MapID == id
                && x.Cell == Bot.Player.Cell
                && (x.HP > 0 || x.State != 0)
            )
        )
            return true;
        return false;
    }

    private bool monsterAvail()
    {
        if (
            Bot.Monsters.MapMonsters.Any(x =>
                x != null && x?.Cell == Bot.Player.Cell && (x?.HP > 0 || x?.State != 0)
            )
        )
            return true;
        return false;
    }

    private void jumpToAvailMonster()
    {
        foreach (Monster m in Bot.Monsters.MapMonsters)
        {
            if (m == null || m?.HP <= 0 || m?.State == 0)
                continue;

            if (Bot.Player.Cell != m!.Cell)
            {
                Bot.Map.Jump(m.Cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(m.Cell);
            }
            return;
        }
    }

    // Auto-select IoDA version if available
    readonly string dragonoftime = Core.CheckInventory("Dragon of Time (IoDA)")
        ? "Dragon of Time (IoDA)"
        : "Dragon of Time";
    readonly string legionrevenant = Core.CheckInventory("Legion Revenant (IoDA)")
        ? "Legion Revenant (IoDA)"
        : "Legion Revenant";
    readonly string kingsecho = "King's Echo";



    private void CheckConfig()
    {
        Dictionary<string, string?> gear = new()
        {
            // Weapon Enhancements
            { "Elysium", Bot.Config!.Get<string>("Elysium") },
            // Cape Enhancements
            { "Penitence", Bot.Config!.Get<string>("Penitence") },
            { "Absolution", Bot.Config!.Get<string>("Absolution") },
            // Helm Enhancements
            { "WizHelm", Bot.Config!.Get<string>("WizHelm") },
            { "HealerHelm", Bot.Config!.Get<string>("HealerHelm") },
            { "ForgeHelm", Bot.Config!.Get<string>("ForgeHelm") },
        };

        // Optional: Check all and log missing
        string[] requiredClasses = { dragonoftime, legionrevenant, kingsecho };
        string[] missingClasses = [.. requiredClasses.Where(c => !Core.CheckInventory(c))];

        if (missingClasses.Length > 0)
            Core.Logger(
                $"Missing required classes ({missingClasses.Length}):\n- {string.Join("\n- ", missingClasses)}",
                "Missing required classes",
                stopBot: true,
                messageBox: true
            );

        // Filter non-null and non-whitespace items and cast to non-nullable string
        List<string> requiredItems = gear
            .Values.Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item!) // Cast to non-nullable string
            .Where(item => !Bot.Inventory.IsEquipped(item) && !Bot.Bank.Contains(item))
            .ToList();

        Core.CheckInventory(requiredItems.ToArray());

        // Determine whether to skip enhancements
        bool skipEnh = Bot.Config!.Get<bool>("SkipEnhancements");

        // Validate config values
        foreach (var opt in Options.OfType<Option<string>>())
        {
            if (ReferenceEquals(opt, CoreBots.Instance.SkipOptions))
                continue;

            if (!gear.TryGetValue(opt.Name, out string? value))
                continue;

            if (string.IsNullOrWhiteSpace(value))
            {
                Core.Logger(
                    $"The item for enhancement '{opt.Name}' is missing.\n"
                        + $"Go to: Scripts > [Edit Script Options], then enter the exact item Name (case-sensitive).\n"
                        + $"Use Tools > Grabber > Inventory to get the correct Name.",
                    $"Missing Item: {opt.Name}",
                    stopBot: true
                );
            }
        }

        // Precompile lowercase Name list for comparison
        HashSet<string> allItemNames = Bot
            .Inventory.Items.Concat(Bot.Bank.Items)
            .Select(i => i.Name.ToLowerInvariant().Trim())
            .ToHashSet();

        // Enhancement result log
        List<string> summaryLogs = new();

        void EnhanceIfFound(
            string? Name,
            EnhancementType type,
            CapeSpecial cape = CapeSpecial.None,
            HelmSpecial helm = HelmSpecial.None,
            WeaponSpecial weapon = WeaponSpecial.None
        )
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            if (!Core.CheckInventory(Name))
            {
                string key = Name.ToLowerInvariant().Trim();
                if (allItemNames.Contains(key))
                    Core.Logger(
                        $"[WARN] \"{Name}\" is in inventory/bank but may have a capitalization/spacing mismatch."
                    );
                else
                    Core.Logger(
                        $"[MISSING] Enhancement target \"{Name}\" not found in inventory or bank."
                    );
                return;
            }

            if (!skipEnh)
            {
                Adv.EnhanceItem(Name, type, cape, helm, weapon, logging: false);
                summaryLogs.Add(
                    $"- {Name}: {type}"
                        + (
                            weapon != WeaponSpecial.None ? $" ({weapon})"
                            : cape != CapeSpecial.None ? $" ({cape})"
                            : helm != HelmSpecial.None ? $" ({helm})"
                            : ""
                        )
                );
            }
        }

        // Static class enhancements
        EnhanceIfFound(kingsecho, EnhancementType.Lucky);
        EnhanceIfFound(legionrevenant, EnhancementType.Wizard);
        EnhanceIfFound(dragonoftime, EnhancementType.Wizard);

        // Weapon enhancements
        EnhanceIfFound(gear["Elysium"], EnhancementType.Wizard, weapon: WeaponSpecial.Elysium);

        // Helm enhancements
        EnhanceIfFound(gear["ForgeHelm"], EnhancementType.Lucky, helm: HelmSpecial.Forge);
        EnhanceIfFound(gear["WizHelm"], EnhancementType.Wizard);
        EnhanceIfFound(gear["HealerHelm"], EnhancementType.Healer);

        // Cape enhancements
        EnhanceIfFound(gear["Penitence"], EnhancementType.Wizard, cape: CapeSpecial.Penitence);
        EnhanceIfFound(gear["Absolution"], EnhancementType.Wizard, cape: CapeSpecial.Absolution);

        // Final log
        if (!skipEnh)
            foreach (string log in summaryLogs)
                Core.Logger(log);
    }

    private void Prereqs()
    {
        if (Core.isCompletedBefore(9465))
            return;

        Farm.Experience(80);
        DVB.StoryLine();

        Forge.Smite();

        #region Grimgaol Prereqs
        // Smite the Boulder! (9463)
        if (!Story.QuestProgression(9463))
        {
            Core.EnsureAccept(9463);
            Adv.BuyItem("forge", 2142, 70990);
            Core.GetMapItem(12327, map: "gaolcell");
            Core.EnsureComplete(9463);
        }

        // Strike the Boulder! (9464)
        if (!Story.QuestProgression(9464))
        {
            InfernalArena.BuyAllMerge("Scythe of Azalith");
            Core.EnsureAccept(9464);
            Core.GetMapItem(12328, map: "gaolcell");
            Core.EnsureComplete(9464);
        }

        // Smash the Boulder! (9465)
        if (!Story.QuestProgression(9465))
        {
            J6.J6(true);
            Adv.BuyItem("hyperspace", 194, "J6's Hammer");
            Core.EnsureAccept(9465);
            Core.GetMapItem(12329, map: "gaolcell");
            Core.EnsureComplete(9465);
        }
        #endregion Grimgaol Prereqs
    }
}

class Split
{
    public string Name { get; init; } = "";
    public TimeSpan Time { get; init; }
}
