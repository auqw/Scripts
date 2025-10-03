/*
name: Grimgoal
description: Goes through the grimgaol dungeon... Testing Phase
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

using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Players;
using Skua.Core.Options;
using Newtonsoft.Json.Linq;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Items;
using Skua.Core.Models.Auras;
using System.Diagnostics;
using Skua.Core.Models;
using System.Globalization;
using Skua.Core.Utils;

public class Grimgaol
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static DoomVaultB DVB { get => _DVB ??= new DoomVaultB(); set => _DVB = value; }
    private static DoomVaultB _DVB;
    private static InfernalArenaMerge InfernalArena { get => _InfernalArena ??= new InfernalArenaMerge(); set => _InfernalArena = value; }
    private static InfernalArenaMerge _InfernalArena;
    private static J6Saga J6 { get => _J6 ??= new J6Saga(); set => _J6 = value; }
    private static J6Saga _J6;
    private static UnlockForgeEnhancements Forge { get => _Forge ??= new UnlockForgeEnhancements(); set => _Forge = value; }
    private static UnlockForgeEnhancements _Forge;

    Stopwatch runTimer = new();
    TimeSpan bestTime = TimeSpan.MaxValue;
    int RunCount = 0;

    public string OptionsStorage = "Grimgaol";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        // VHL : luck
        // VDK : luck
        // Dragon of Time : Healer

        // Skip Options
        CoreBots.Instance.SkipOptions,

        // Skip Enhancements
        new Option<bool>("SkipEnhancements", "Skip Item Enhancing", "If enabled, will not enhance items for the run (this doesnt mean skip enhancements you dont have... these enhancements are **VERY** important).", false),
        new Option<bool>("RoomTimers", "Time each room?", "If enabled, this will log the time for each room into chat/the Logs > scripts tab.", false),

        // Weapons
        new Option<string>("Valiance", "Weapon: Valiance", "insert Name of your Valiance weapon", ""),
        new Option<string>("Dauntless", "Weapon: Dauntless", "insert Name of your Dauntless weapon ( this will be subbed with Valiance if u dont have Daunt so just copy your Valiance weapon Name here)", ""),
        new Option<string>("Elysium", "Weapon: Elysium", "insert Name of your Elysium weapon", ""),
      
        // Helm
        new Option<string>("LuckHelm", "Helm: LuckHelm", "insert Name of your Lucky helm", ""),
        new Option<string>("HealerHelm", "Helm: HealerHelm", "insert Name of your Healer helm (used for DoT Sheltons)", ""),
        new Option<string>("AnimaHelm", "Helm: AnimaHelm", "insert Name of your AnimaHelm helm", ""),
        new Option<string>("PneumaHelm", "Helm: PneumaHelm", "insert Name of your Pneuma helm (used for VHL Sheltons)", ""),
        
        // Cape
        new Option<string>("Penitence", "Cape: Penitence", "insert Name of your Penitence cape", ""),
        new Option<string>("Vainglory", "Cape: Vainglory", "insert Name of your Vainglory cape", ""),
        new Option<string>("HealerCape", "Cape: HealerCape", "insert Name of your Healer cape", ""),
    };

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

            Core.EnsureAccept(!Core.HasWebBadge("SkullCrusher") ? 9466 : (Core.IsMember ? 9468 : 9467));

            if (Bot.Map.Name.ToLower() == "grimgaol")
            {
                Core.Logger("Already in the dungeon (weird flex but ok)... we'll continue from where you left off");
                Init();
            }
            else
            {
                Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%grimgaol-100000%");
                Bot.Wait.ForMapLoad("grimgaol");

                // Incase we're in the Cutscene cell
                if (Bot.Player?.Cell != "Enter")
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

    const int MaxBackupRuns = 50;

    void LogRun()
    {
        runTimer.Stop();
        TimeSpan currentTime = runTimer.Elapsed;
        if (currentTime < TimeSpan.Zero)
            currentTime = TimeSpan.Zero;

        AppendRun(currentTime);

        TimeSpan bestTime = LoadBestTime();
        bool isNewPB = currentTime < bestTime;

        Core.Logger($"Dungeon run took: {currentTime:mm\\:ss\\.fff} | Personal Best: {(bestTime == TimeSpan.MaxValue ? "N/A" : bestTime.ToString("mm\\:ss\\.fff"))}" +
                    (isNewPB ? " (New PB!)" : ""));
    }

    TimeSpan LoadBestTime()
    {
        string path = Path.Combine(ClientFileSources.SkuaScriptsDIR, "Prototypes", "GrimGaolRunTimes.txt");
        string backupPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "Prototypes", "GrimGaolRunTimes_backup.txt");

        // Auto-restore from backup if main file missing or empty
        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, path, overwrite: true);
                Core.Logger("Main run file missing or empty, restored from backup.");
            }
            else
            {
                return TimeSpan.MaxValue; // No data at all
            }
        }

        TimeSpan best = TimeSpan.MaxValue;
        List<string> validLines = new();

        foreach (string line in File.ReadAllLines(path))
        {
            string trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (TimeSpan.TryParseExact(trimmed, "c", CultureInfo.InvariantCulture, out TimeSpan parsed) && parsed >= TimeSpan.Zero)
            {
                validLines.Add(parsed.ToString("c", CultureInfo.InvariantCulture));
                if (parsed < best)
                    best = parsed;
            }
        }

        // If all lines were invalid, try restore from backup
        if (validLines.Count == 0 && File.Exists(backupPath))
        {
            File.Copy(backupPath, path, overwrite: true);
            Core.Logger("All main file entries were invalid, restored from backup.");
            foreach (string line in File.ReadAllLines(path))
            {
                if (TimeSpan.TryParseExact(line.Trim(), "c", CultureInfo.InvariantCulture, out TimeSpan parsed) && parsed >= TimeSpan.Zero)
                {
                    validLines.Add(parsed.ToString("c", CultureInfo.InvariantCulture));
                    if (parsed < best)
                        best = parsed;
                }
            }
        }

        // Rewrite main file with only valid entries
        if (validLines.Count > 0)
            File.WriteAllLines(path, validLines);

        // Maintain backup with last MaxBackupRuns
        if (validLines.Count > 0)
        {
            List<string> backupLines = validLines.Count > MaxBackupRuns
                ? validLines.GetRange(validLines.Count - MaxBackupRuns, MaxBackupRuns)
                : new List<string>(validLines);

            File.WriteAllLines(backupPath, backupLines);
        }

        return best;
    }

    void AppendRun(TimeSpan run)
    {
        if (run < TimeSpan.Zero)
            run = TimeSpan.Zero;

        string path = Path.Combine(ClientFileSources.SkuaScriptsDIR, "Prototypes", "GrimGaolRunTimes.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        try
        {
            File.AppendAllText(path, run.ToString("c", CultureInfo.InvariantCulture) + Environment.NewLine);
        }
        catch (IOException ex)
        {
            Core.Logger($"Failed to write run time: {ex.Message}");
        }
    }

    bool Daunt = false;
    private void Init()
    {
        if (Bot.Player.Cell.ToLower().Contains("cut"))
        {
            Core.Logger($"in {Bot.Player?.Cell} cell, jumping to enter");
            Bot.Map.Jump("Enter", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("Enter");
        }

        Daunt = Adv.uDauntless();

        // Stop usage of AdvSkills after story & prereqs. as we'll use our own here.
        while (!Bot.ShouldExit && !Bot.TempInv.Contains("Grimskull's Gaol Cleared"))
        {
            jumpToAvailMonster();
            Bot.Skills.Stop();
            if (Bot.Player != null)
                switch (Bot.Player?.Cell)
                {
                    // Grimskull? - VHL
                    case "Enter":
                        Enter();
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"Enter\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r2")
                        {
                            Bot.Map.Jump("r2", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r2");
                        }
                        break;

                    // Grim Bomb - DoT
                    case "r2":
                        // if (Core.CheckInventory(legionrevenant))
                        //     RLR(Bot.Player.Cell);
                        // else
                        RDoT(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r2\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r3")
                        {
                            Bot.Map.Jump("r3", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r3");
                        }
                        break;

                    // Empress Angler - VDK
                    case "r3":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r3\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r4")
                        {
                            Bot.Map.Jump("r4", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r4");
                        }
                        break;

                    // Treasure Chest - VHL
                    case "r4":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r4\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r5")
                        {
                            Bot.Map.Jump("r5", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r5");
                        }
                        break;

                    // Reinforced Shelleton (rng trash) - LR/AM
                    case "r5":
                        // Rarchmage(Bot.Player.Cell);
                        R5archmage();
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r5\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r6")
                        {
                            Bot.Map.Jump("r6", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r6");
                        }
                        break;

                    // Fell Statue - ArchMage
                    case "r6":
                        Rarchmage(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r6\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r7")
                        {
                            Bot.Map.Jump("r7", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r7");
                        }
                        break;

                    // Emperor Angler - VDK
                    case "r7":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r7\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r8")
                        {
                            Bot.Map.Jump("r8", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r8");
                        }
                        break;

                    // Treasure Chest - VDK
                    case "r8":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r8\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r9")
                        {
                            Bot.Map.Jump("r9", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r9");
                        }
                        break;

                    // Rick, Grim Soldier - VDK
                    case "r9":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r9\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r10")
                        {
                            Bot.Map.Jump("r10", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r10");
                        }
                        break;

                    // Mechro Lich + Rampaging Cyborg - DoT
                    case "r10":
                        R10();
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r10\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r11")
                        {
                            Bot.Map.Jump("r11", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r11");
                        }
                        break;

                    // Mechabinky &amp; Raxborg - VDK
                    case "r11":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r11\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r12")
                        {
                            Bot.Map.Jump("r12", "Left", autoCorrect: false);
                            Bot.Wait.ForCellChange("r12");
                        }
                        break;

                    // Grimskull - VDK
                    case "r12":
                        RVDK(Bot.Player.Cell);
                        if (Bot.Config!.Get<bool>("RoomTimers"))
                            Core.Logger($"Room \"r12\" Done in: {runTimer.Elapsed}");
                        if (Bot.Player?.Cell != "r12a")
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

        // End runtime here
        if (Core.CheckInventory("Grimskull's Gaol Cleared"))
        {
            Core.Logger($"Runs complete so far: {RunCount++}");
            LogRun();
        }

        Core.EnsureComplete(!Core.HasWebBadge("SkullCrusher") ? 9466 : (Core.IsMember ? 9468 : 9467));

        if (!Bot.Quests.IsDailyComplete(9469) && Core.HasWebBadge("SkullCrusher"))
            Core.EnsureComplete(9469);

        Core.Join("whitemap-100000");

        Bot.Send.Packet($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%grimgaol-100000%");
        Bot.Wait.ForMapLoad("grimgaol");
        if (Bot.Player != null)
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);
    }

    #region These are fine
    private void Enter()
    {
        if (Bot.Player?.Cell != "Enter")
        {
            Core.Logger("jump to enter");
            Bot.Map.Jump("Enter", "Spawn", autoCorrect: false);
            Bot.Wait.ForCellChange("Enter");
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }
        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(voidhighlord);
        EquipIfAvailable(Bot.Config!.Get<string>(Daunt ? "Dauntless" : "Valiance"));
        EquipIfAvailable(Bot.Config.Get<string>("AnimaHelm") ?? Bot.Config.Get<string>("LuckHelm"));
        EquipIfAvailable(Bot.Config.Get<string>("Vainglory") ?? Bot.Config.Get<string>("Penitence"));
        #endregion
        // Run Timer starts here
        runTimer.Restart();

        int skillIndex = 0;
        int[] skillList = { 1, 4, 2 };
        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                skillIndex = 0;
            }

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (Bot.Player?.Cell != "Enter")
            {
                Core.Logger("jump back to enter");
                Bot.Map.Jump("Enter", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Enter");
                Bot.Sleep(1000);
            }

            if (Bot.Player!.HasTarget && Bot.Target?.HasActiveAura("Talon Twisting") == true)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.StopAttacking = true;

                Bot.Sleep(1000);

                // Wait until the target has the "Retaliate" aura
                Bot.Wait.ForTrue(() => Bot.Player.HasTarget && Bot.Target?.HasActiveAura("Retaliate") == true, 20);

                while (!Bot.ShouldExit && Bot.Player.HasTarget && Bot.Target?.HasActiveAura("Retaliate") == true) { Bot.Sleep(100); }

                Bot.Combat.StopAttacking = false;
                skillIndex = 0;
                Bot.Skills.Start();
                Bot.Sleep(1000);
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player?.Target?.HP <= 0)
                break;

            if (Bot.Player!.Health <= 2500 && Bot.Skills.CanUseSkill(2))
                Bot.Skills.UseSkill(2);

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && !Bot.Self.HasActiveAura("Shackled") && skillIndex == 0
            && Bot.Skills.CanUseSkill(skillList[skillIndex]))
            {
                Bot.Skills.UseSkill(skillList[skillIndex]);
                skillIndex = (skillIndex + 1) % skillList.Length;
            }

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && skillIndex != 0
            && Bot.Skills.CanUseSkill(skillList[skillIndex]))
            {
                Bot.Skills.UseSkill(skillList[skillIndex]);
                skillIndex = (skillIndex + 1) % skillList.Length;
            }

            Bot.Sleep(200);
        }
    }

    // private void R2()
    // {
    //     if (Bot.Player?.Cell != "r2")
    //     {
    //         Core.Logger("jump to r2");
    //         Bot.Map.Jump("r2", "Left", autoCorrect: false);
    //         Bot.Wait.ForCellChange("r2");
    //     }
    //     Bot.Sleep(1000);

    //     if (!monsterAvail())
    //     {
    //         runTimer.Stop();
    //         return;
    //     }
    //     Bot.Player?.SetSpawnPoint();

    //     #region Equipment Setup
    //     EquipIfAvailable(voidhighlord);
    //     EquipIfAvailable(Bot.Config!.Get<string>(Daunt ? "Dauntless" : "Valiance"));
    //     EquipIfAvailable(Bot.Config!.Get<string>("AnimaHelm") ?? Bot.Config!.Get<string>("LuckHelm"));
    //     EquipIfAvailable(Bot.Config!.Get<string>("Vainglory") ?? Bot.Config!.Get<string>("Penitence"));

    //     #endregion
    //     runTimer.Start();

    //     int skillIndex = 0;
    //     int[] skillList = { 1, 4, 2, 3 };

    // Restart:
    //     foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
    //     {
    //         if (m == null || m?.HP <= 0 || m?.State == 0)
    //             continue;

    //         while (!Bot.ShouldExit)
    //         {
    //             if (!Bot.Player!.Alive)
    //             {
    //                 Bot.Sleep(100);
    //                 if (Bot.Player.Alive)
    //                     skillIndex = 0;
    //                 goto Restart;
    //             }

    //             // Ensure we're still in the right cell
    //             if (Bot.Player?.Cell != "r2")
    //             {
    //                 Bot.Map.Jump("r2", "Left", autoCorrect: false);
    //                 Bot.Wait.ForCellChange("r2");
    //             }

    //             if (!monsterAvail())
    //             {
    //                 runTimer.Stop();
    //                 return;
    //             }

    //             // Start attack if no target
    //             if (!Bot.Player!.HasTarget)
    //                 Bot.Combat.Attack("*");

    //             if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
    //             {
    //                 Bot.Combat.CancelAutoAttack();
    //                 Bot.Combat.CancelTarget();
    //                 break;
    //             }

    //             if (Bot.Player.Health <= 2500 && Bot.Skills.CanUseSkill(2))
    //                 Bot.Skills.UseSkill(2);

    //             else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && !Bot.Self.HasActiveAura("Shackled") && skillIndex == 0
    //               && Bot.Skills.CanUseSkill(skillList[skillIndex]))
    //             {
    //                 Bot.Skills.UseSkill(skillList[skillIndex]);
    //                 skillIndex = (skillIndex + 1) % skillList.Length;
    //             }

    //             else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
    //               && Bot.Skills.CanUseSkill(skillList[skillIndex]))
    //             {
    //                 Bot.Skills.UseSkill(skillList[skillIndex]);
    //             }
    //             skillIndex = (skillIndex + 1) % skillList.Length;
    //             Bot.Sleep(200);

    //         }
    //     }


    // }

    private void R6()
    {
        if (Bot.Player?.Cell != "r6")
        {
            Core.Logger("Jumping to r6");
            Bot.Map.Jump("r6", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r6");
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        bool usechaosAvenger = Core.CheckInventory("Chaos Avenger");
        #region Equipment Setup
        EquipIfAvailable(usechaosAvenger ? "Chaos Avenger" : voidhighlord);
        EquipIfAvailable(Bot.Config!.Get<string>(Daunt ? "Dauntless" : "Valiance"));
        EquipIfAvailable(Bot.Config!.Get<string>("AnimaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Penitence"));
        #endregion

        runTimer.Start();

        int[] skillList = usechaosAvenger ? new[] { 4, 1, 3 } : new[] { 2, 4 };
        int skillIndex = 0;

        while (!Bot.ShouldExit)
        {
            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
            {
                if (m == null || m?.HP <= 0 || m?.State == 0)
                    continue;

                // Keep fighting the monster until dead or aura appears
                while (!Bot.ShouldExit)
                {
                    if (!monsterAvail())
                    {
                        runTimer.Stop();
                        return;
                    }


                    if (!Bot.Player!.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        skillIndex = 0;
                    }

                    // Check if target is set, otherwise target this monster
                    if (!Bot.Player.HasTarget || Bot.Player.HasTarget && Bot.Player.Target?.MapID != m!.MapID)
                        Bot.Combat.Attack(m!.MapID);

                    Bot.Sleep(500);

                    if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
                    {
                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.CancelTarget();
                        break;
                    }

                    // Exit loop if target has "Crit Damage Amplified" aura
                    if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                     && Bot.Target.HasActiveAura("Crit Damage Amplified"))
                    {
                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.StopAttacking = true;

                        Bot.Wait.ForTrue(() => Bot.Player.HasTarget
                                                && Bot.Player.Target?.HP > 0
                                                && Bot.Target?.HasActiveAura("Crit Damage Amplified") == false, 20);

                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.CancelTarget();
                        Bot.Combat.StopAttacking = false;
                        continue;
                    }


                    if (Bot.Player.Health <= (usechaosAvenger ? 3500 : 2500) && Bot.Skills.CanUseSkill(usechaosAvenger ? 3 : 2))
                        Bot.Skills.UseSkill(2);


                    if (usechaosAvenger)
                    {
                        if (Bot.Player.HasTarget
                          && Bot.Player.Target?.HP > 0
                          && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                        {
                            Bot.Skills.UseSkill(skillList[skillIndex]);
                        }
                        // Keep this her so skills dont get hung up and you die.
                        skillIndex = (skillIndex + 1) % skillList.Length;
                    }
                    else
                    {
                        if (Bot.Player.Health >= 2500 && (skillIndex == 0 || skillIndex == 2) && Bot.Player.HasTarget
                        && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                        {
                            Bot.Skills.UseSkill(skillList[skillIndex]);
                            skillIndex = (skillIndex + 1) % skillList.Length;
                        }
                        else if (skillIndex != 0 && skillIndex != 2 && Bot.Player.HasTarget
                        && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                        {
                            Bot.Skills.UseSkill(skillList[skillIndex]);
                        }
                        skillIndex = (skillIndex + 1) % skillList.Length;
                    }
                }
            }
        }
    }

    private void R9()
    {
        if (Bot.Player?.Cell != "r9")
        {
            Core.Logger("jump to r9");
            Bot.Map.Jump("r9", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r9");
        }
        Bot.Sleep(1000);

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }
        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("PneumaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Vainglory"));
        #endregion
        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = { 3, 2, 1, 2, 4, 2 };

        while (!Bot.ShouldExit)
        {

            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                skillIndex = 0;
            }

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.CancelTarget();
                break;
            }

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
             && Bot.Skills.CanUseSkill(skillList[skillIndex]))
            {
                Bot.Skills.UseSkill(skillList[skillIndex]);
            }
            skillIndex = (skillIndex + 1) % skillList.Length;
        }
    }

    private void R10()
    {
        if (Bot.Player?.Cell != "r10")
        {
            Core.Logger("jump to r10");
            Bot.Map.Jump("r10", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r10");
        }
        Bot.Sleep(1000);

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }
        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("HealerHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("HealerCape"));
        #endregion

        runTimer.Start();
        int[] skillList = { 1, 2, 4, 2, 3, 2 };
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

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (monsId != "*" && Bot.Monsters.MapMonsters.Any(
                x => x != null
                && x?.Cell == Bot.Player?.Cell
                && x?.MapID.ToString() == monsId
                && (x?.HP <= 70000 || x?.HP <= 50000 || x?.HP <= 20000)))
            {
                switch (monsId)
                {
                    case "16":
                        monsId = "17"; monsIdInt = 17;
                        break;
                    case "17":
                        monsId = "18"; monsIdInt = 18;
                        break;
                    case "18":
                        monsId = "*";
                        break;
                }
            }


            if (monsId == "*") doPriorityAttackId(new int[] { 16, 17, 18, 19 });
            else doPriorityAttackId(new int[] { monsIdInt });

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
            && Bot.Skills.CanUseSkill(skillList[skillIndex]))
            {
                // Use next skill in rotation
                Bot.Skills.UseSkill(skillList[skillIndex]);
                skillIndex = (skillIndex + 1) % skillList.Length;
            }
        }
    }

    private void R5archmage()
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != "r5")
        {
            Core.Logger($"Jumping to \"r5\"");
            Bot.Map.Jump("r5", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r5");
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(archmage);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("PneumaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Penitence"));
        #endregion


        runTimer.Start();
    Restart:
        int skillIndex = 0;
        // 1 and 4 will be used conditionaly ( hopefully) 
        int[] skillList = { 2, 3 };
        while (!Bot.ShouldExit)
        {
            foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
            {
                if (m == null || m?.HP <= 0 || m?.State == 0)
                    continue;

                while (!Bot.ShouldExit)
                {
                    if (!Bot.Player!.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        if (Bot.Player.Alive)
                            skillIndex = 0;
                        goto Restart;
                    }

                    // Ensure we're still in the right cell
                    if (Bot.Player?.Cell != "r5")
                    {
                        Bot.Map.Jump("r5", "Left", autoCorrect: false);
                        Bot.Wait.ForCellChange("r5");
                    }

                    if (!monsterAvail())
                    {
                        runTimer.Stop();
                        return;
                    }

                    if (!Bot.Player.HasTarget || !Bot.Player.InCombat)
                        Bot.Combat.Attack(m!.MapID);

                    if (Bot.Player.Target?.HP <= 0 || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                    {
                        if (Bot.Player.InCombat)
                            Bot.Combat.CancelTarget();
                        break;
                    }

                    // 1.3.0
                    // if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && Bot.Target!.GetTotalAuraStacks("Incinerating") < 1 && Bot.Skills.CanUseSkill(1))
                    //     Bot.Skills.UseSkill(1);
                    // else if (Bot.Player.HasTarget&& Bot.Player.Target?.HP > 0 && Bot.Self.GetTotalAuraStacks("Corporeal Ascension") < 1 && Bot.Skills.CanUseSkill(4))
                    //     Bot.Skills.UseSkill(4);

                    // 1.2.5.1
                    if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                    && !Bot.Target!.HasActiveAura("Incinerating")
                    && Bot.Skills.CanUseSkill(1))
                        Bot.Skills.UseSkill(1);

                    else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                    && !Bot.Self.HasActiveAura("Corporeal Ascension")
                    && Bot.Skills.CanUseSkill(4))
                        Bot.Skills.UseSkill(4);

                    else
                    {
                        // Use next skill
                        if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                        && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                        {
                            Bot.Skills.UseSkill(skillList[skillIndex]);
                        }
                        skillIndex = (skillIndex + 1) % skillList.Length;
                    }

                    Bot.Sleep(200);
                }
            }
        }
    }

    // private void RLR(string cell)
    // {
    //     // Jump to cell if needed
    //     if (Bot.Player?.Cell != cell)
    //     {
    //         Core.Logger($"Jumping to {cell}");
    //         Bot.Map.Jump(cell, "Left", autoCorrect: false);
    //         Bot.Wait.ForCellChange(cell);
    //     }

    //     if (!monsterAvail())
    //     {
    //         runTimer.Stop();
    //         return;
    //     }
    //     Bot.Player?.SetSpawnPoint();

    //     #region Equipment Setup  
    //     EquipIfAvailable(legionrevenant);
    //     EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
    //     EquipIfAvailable(Bot.Config!.Get<string>("Wizard"));
    //     EquipIfAvailable(Bot.Config!.Get<string>("Vainglory"));
    //     #endregion

    //     runTimer.Start();
    // Restart:
    //     int skillIndex = 0;
    //     int[] skillList = new[] { 3, 2, 1, 4 };

    //     while (!Bot.ShouldExit)
    //     {
    //         if (!monsterAvail())
    //         {
    //             runTimer.Stop();
    //             return;
    //         }
    //         foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
    //         {
    //             if (m == null || m?.HP <= 0 || m?.State == 0)
    //                 continue;

    //             while (!Bot.ShouldExit)
    //             {
    //                 if (!monsterAvail())
    //                 {
    //                     runTimer.Stop();
    //                     return;
    //                 }

    //                 if (!Bot.Player!.Alive)
    //                 {
    //                     Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
    //                     skillIndex = 0;
    //                     goto Restart;
    //                 }

    //                 if (!Bot.Player.HasTarget || Bot.Player.HasTarget && Bot.Player.Target?.MapID != m!.MapID)
    //                     Bot.Combat.Attack(m!.MapID);

    //                 if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
    //                 {
    //                     Bot.Combat.CancelAutoAttack();
    //                     Bot.Combat.CancelTarget();
    //                     break;
    //                 }

    //                 // HoT if needed
    //                 if (Bot.Player.HasTarget && Bot.Player?.Health < Bot.Player?.MaxHealth * 0.9
    //                 && Bot.Skills.CanUseSkill(3))
    //                     Bot.Skills.UseSkill(3);

    //                 if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
    //                 {
    //                     Bot.Skills.UseSkill(skillList[skillIndex]);
    //                     skillIndex = (skillIndex + 1) % skillList.Length;
    //                 }

    //                 Bot.Sleep(100);
    //             }
    //         }
    //     }
    // }

    private void RVHL(string cell)
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != cell)
        {
            Core.Logger($"Jumping to {cell}");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(voidhighlord);
        EquipIfAvailable(Bot.Config!.Get<string>("Dauntless"));
        EquipIfAvailable(Bot.Config!.Get<string>("AnimaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Vainglory"));
        #endregion


        runTimer.Start();
        int skillIndex = 0;
        string[] SkillsPerCell = new[] { "Enter", "r2", "r11", "r12" };
        int[] skillList = Array.Empty<int>();

        foreach (string skillCell in SkillsPerCell)
        {
            switch (skillCell)
            {
                case "Enter":
                    skillList = new[] { 1, 4, 2 };
                    break;

                case "r2":
                    skillList = new[] { 1, 4, 2, 3 };
                    break;

                case "r11":
                case "r12":
                    skillList = new[] { 1, 2, 3, 4 };
                    break;
            }
        }

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                if (Bot.Player.Alive)
                    skillIndex = 0;
                continue;
            }

            // Ensure we're still in the right cell
            if (Bot.Player?.Cell != cell)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            if (Bot.Player!.HasTarget && Bot.Target?.HasActiveAura("Talon Twisting") == true)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.StopAttacking = true;

                Bot.Sleep(500);

                // Wait until the target has the "Retaliate" aura
                Bot.Wait.ForTrue(() => Bot.Player.HasTarget && Bot.Target?.HasActiveAura("Retaliate") == true, 20);

                while (!Bot.ShouldExit && Bot.Player.HasTarget && Bot.Target?.HasActiveAura("Retaliate") == true) { Bot.Sleep(100); }

                Bot.Combat.StopAttacking = false;
                skillIndex = 0;
                Bot.Skills.Start();
            }

            // Start attack if no target
            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.CancelTarget();
                break;
            }

            // Heal if needed
            if (Bot.Player.Health <= 2500
            && Bot.Skills.CanUseSkill(2))
                Bot.Skills.UseSkill(2);

            // Use next skill
            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
            {
                if (Bot.Player.Health >= 2500 && (skillIndex == 0 || skillIndex == 2) && Bot.Player.HasTarget
                && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                    skillIndex = (skillIndex + 1) % skillList.Length;
                }
                else if (skillIndex != 0 && skillIndex != 2 && Bot.Player.HasTarget
                && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                }
                skillIndex = (skillIndex + 1) % skillList.Length;
                skillIndex = (skillIndex + 1) % skillList.Length;
            }

            Bot.Sleep(200);
        }
    }

    private void Rarchmage(string cell)
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != cell)
        {
            Core.Logger($"Jumping to \"{cell}\"");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(archmage);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("PneumaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Penitence"));
        #endregion


        runTimer.Start();
    Restart:
        int skillIndex = 0;
        int[] skillList = { 2, 3 };

        foreach (Monster m in Bot.Monsters.CurrentAvailableMonsters)
        {
            if (m == null || m?.HP <= 0 || m?.State == 0)
                continue;

            while (!Bot.ShouldExit)
            {
                if (!Bot.Player!.Alive)
                {
                    Bot.Sleep(100);
                    if (Bot.Player.Alive)
                        skillIndex = 0;
                    goto Restart;
                }

                // Ensure we're still in the right cell
                if (Bot.Player?.Cell != cell)
                {
                    Bot.Map.Jump(cell, "Left", autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                }

                if (!monsterAvail())
                {
                    runTimer.Stop();
                    return;
                }

                if (!Bot.Player.HasTarget
                || !Bot.Player.InCombat
                || (Bot.Player.HasTarget && !Bot.Target.HasActiveAura("Crit Damage Amplified")))
                    Bot.Combat.Attack(m!.MapID);

                if (Bot.Player.Target?.HP <= 0 || m?.HP <= 0 || m?.State == 0 || m?.Alive == false)
                {
                    if (Bot.Player.InCombat)
                        Bot.Combat.CancelTarget();
                    break;
                }

                // For Fell Statues; Whilst Fell Statue has "Crit Damage Amplified" aura, wait for it to end, reneable combat
                // sort of like a retaliate or counter atk, except in this case the more u hit them during this, they do ++100 dmg.
                if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                 && Bot.Target.HasActiveAura("Crit Damage Amplified"))
                {
                    Bot.Combat.CancelAutoAttack();
                    Bot.Combat.StopAttacking = true;

                    Bot.Wait.ForTrue(
                    () => Bot.Player.HasTarget &&
                            (Bot.Player.Target?.HP <= 0
                            || !Bot.Target.HasActiveAura("Crit Damage Amplified")), 20);

                    Bot.Combat.StopAttacking = false;
                }

                // Conditional aura skills
                if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 == true
                 && !Bot.Target!.HasActiveAura("Incinerating")
                 && Bot.Skills.CanUseSkill(1))
                    Bot.Skills.UseSkill(1);

                else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                && !Bot.Self.HasActiveAura("Corporeal Ascension")
                && Bot.Skills.CanUseSkill(4))
                    Bot.Skills.UseSkill(4);

                else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    // Use next skill in rotation
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                    skillIndex = (skillIndex + 1) % skillList.Length;
                }

                Bot.Sleep(100);
            }
        }
    }

    private void RVDK(string cell)
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != cell)
        {
            Core.Logger($"Jumping to {cell}");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(verusdoomdnight);
        EquipIfAvailable(Bot.Config!.Get<string>(Daunt ? "Dauntless" : "Valiance"));
        EquipIfAvailable(Bot.Config!.Get<string>("AnimaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Vainglory"));
        #endregion

        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = { 1, 3, 4 };

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Sleep(100);
                if (Bot.Player.Alive)
                    skillIndex = 0;
                continue;
            }

            // Ensure we're still in the right cell
            if (Bot.Player?.Cell != cell)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            // Start attack if no target
            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.CancelTarget();
                break;
            }

            if (Bot.Player.HasTarget && Bot.Player.Health < Bot.Player.MaxHealth * 0.9 && Bot.Skills?.CanUseSkill(2) == true)
                Bot.Skills.UseSkill(2);
            // Use next skill
            else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
            {
                Bot.Skills!.UseSkill(skillList[skillIndex]);
                skillIndex = (skillIndex + 1) % skillList.Length;
            }

            Bot.Sleep(500);
        }
    }

    private void RYNR(string cell)
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != cell)
        {
            Core.Logger($"Jumping to {cell}");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();
        #region Equipment Setup
        EquipIfAvailable("Yami No Ronin");
        EquipIfAvailable(Bot.Config!.Get<string>(Adv.uDauntless() ? "Dauntless" : "Valiance"));
        EquipIfAvailable(Bot.Config!.Get<string>("AnimaHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("Vainglory"));
        #endregion

        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = { 2, 1, 3 };

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
            {
                Bot.Sleep(100);
                if (Bot.Player.Alive)
                    skillIndex = 0;
                continue;
            }

            // Ensure we're still in the right cell
            if (Bot.Player?.Cell != cell)
            {
                Bot.Map.Jump(cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(cell);
            }

            if (!monsterAvail())
            {
                runTimer.Stop();
                return;
            }

            // Start attack if no target
            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.CancelTarget();
                break;
            }

            // Heal if needed
            if (Bot.Player.Health <= 2500)
                Bot.Skills.UseSkill(2);

            // Use next skill
            if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0)
            {
                // 1.3.0
                // if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0&& Bot.Target!.GetTotalAuraStacks("Yami") < 1 && Bot.Skills.CanUseSkill(2))
                //     Bot.Skills.UseSkill(2);
                // else if (Bot.Player.HasTarget&& Bot.Player.Target?.HP > 0 && Bot.Target!.GetTotalAuraStacks("Yami") > 0 && Bot.Skills.CanUseSkill(4))
                //     Bot.Skills.UseSkill(4);

                //1.2.5.1
                if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && !Bot.Target!.HasActiveAura("Yami") && Bot.Skills.CanUseSkill(2))
                    Bot.Skills.UseSkill(2);
                else if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0 && Bot.Target!.HasActiveAura("Yami") && Bot.Skills.CanUseSkill(4))
                    Bot.Skills.UseSkill(4);
                else
                {
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                    skillIndex = (skillIndex + 1) % skillList.Length;
                }
            }

            Bot.Sleep(200);
        }
    }

    private void RDoT(string cell, bool statues = false)
    {
        // Jump to cell if needed
        if (Bot.Player?.Cell != cell)
        {
            Core.Logger($"Jumping to \"{cell}\"");
            Bot.Map.Jump(cell, "Left", autoCorrect: false);
            Bot.Wait.ForCellChange(cell);
        }

        if (!monsterAvail())
        {
            runTimer.Stop();
            return;
        }

        Bot.Player?.SetSpawnPoint();

        #region Equipment Setup
        EquipIfAvailable(dragonoftime);
        EquipIfAvailable(Bot.Config!.Get<string>("Elysium"));
        EquipIfAvailable(Bot.Config!.Get<string>("HealerHelm"));
        EquipIfAvailable(Bot.Config!.Get<string>("HealerCape"));
        #endregion
        runTimer.Start();

        int skillIndex = 0;
        int[] skillList = new[] { 3, 2, 1, 2, 4 };


        while (!Bot.ShouldExit)
        {
            if (!monsterAvail())
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
                    if (!monsterAvail())
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

                    // Exit loop if target has "Crit Damage Amplified" aura
                    if (Bot.Player.HasTarget && Bot.Player.Target?.HP > 0
                     && Bot.Target.HasActiveAura("Crit Damage Amplified"))
                    {
                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.StopAttacking = true;

                        Bot.Wait.ForTrue(() => Bot.Target?.HasActiveAura("Crit Damage Amplified") == false, 20);

                        Bot.Combat.CancelAutoAttack();
                        Bot.Combat.CancelTarget();
                        Bot.Combat.StopAttacking = false;
                        continue;
                    }

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
        if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == id && x.Cell == Bot.Player?.Cell && (x.HP > 0 || x.State != 0)))
            return true;
        return false;
    }

    private bool monsterAvail()
    {
        if (Bot.Monsters.MapMonsters.Any(x => x != null && x?.Cell == Bot.Player?.Cell && (x?.HP > 0 || x?.State != 0)))
            return true;
        return false;
    }

    private void jumpToAvailMonster()
    {
        foreach (Monster m in Bot.Monsters.MapMonsters)
        {
            if (m == null || m?.HP <= 0 || m?.State == 0)
                continue;

            if (Bot.Player?.Cell != m!.Cell)
            {
                Bot.Map.Jump(m.Cell, "Left", autoCorrect: false);
                Bot.Wait.ForCellChange(m.Cell);
            }
            return;
        }
    }


    // Auto-select IoDA version if available
    string dragonoftime = Core.CheckInventory("Dragon of Time (IoDA)") ? "Dragon of Time (IoDA)" : "Dragon of Time";
    string legionrevenant = Core.CheckInventory("Legion Revenant (IoDA)") ? "Legion Revenant (IoDA)" : "Legion Revenant";
    string voidhighlord = Core.CheckInventory("Void HighLord (IoDA)") ? "Void HighLord (IoDA)" : "Void Highlord";
    string verusdoomdnight = Core.CheckInventory("Verus DoomKnight (IoDA)") ? "Verus DoomKnight (IoDA)" : "Verus DoomKnight";
    string chaosAvenger = "Chaos Avenger";
    string archmage = "ArchMage";
    private void CheckConfig()
    {
        // Load config values into dictionary
        Dictionary<string, string?> gear = new()
    {
        // Weapon Enhancements
        { "Dauntless", Bot.Config!.Get<string>("Dauntless") },
        { "Valiance", Bot.Config!.Get<string>("Valiance") },
        { "Elysium",  Bot.Config!.Get<string>("Elysium") },

        // Cape Enhancements
        { "Vainglory", Bot.Config!.Get<string>("Vainglory") },
        { "Penitence", Bot.Config!.Get<string>("Penitence") },
        { "HealerCape", Bot.Config!.Get<string>("HealerCape") },

        // Helm Enhancements
        { "PneumaHelm", Bot.Config!.Get<string>("PneumaHelm")},
        { "AnimaHelm", Bot.Config!.Get<string>("AnimaHelm") },
        { "HealerHelm", Bot.Config!.Get<string>("HealerHelm") },
        { "LuckHelm", Bot.Config!.Get<string>("LuckHelm") },
    };

        // Optional: Check all and log missing
        string[] requiredClasses = { dragonoftime, voidhighlord, verusdoomdnight, archmage };
        string[] missingClasses = requiredClasses.Where(c => !Core.CheckInventory(c)).ToArray();
        if (missingClasses.Length > 0)
            Core.Logger($"Missing required classes ({missingClasses.Length}):\n- {string.Join("\n- ", missingClasses)}", "Missing required classes", stopBot: true, messageBox: true);

        // Filter non-null and non-whitespace items and cast to non-nullable string
        List<string> requiredItems = gear.Values
     .Where(item => !string.IsNullOrWhiteSpace(item))
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
                    $"The item for enhancement '{opt.Name}' is missing.\n" +
                    $"Go to: Scripts > [Edit Script Options], then enter the exact item Name (case-sensitive).\n" +
                    $"Use Tools > Grabber > Inventory to get the correct Name.",
                    $"Missing Item: {opt.Name}",
                    stopBot: true);
            }
        }

        // Precompile lowercase Name list for comparison
        HashSet<string> allItemNames = Bot.Inventory.Items
            .Concat(Bot.Bank.Items)
            .Select(i => i.Name.ToLowerInvariant().Trim())
            .ToHashSet();

        // Enhancement result log
        List<string> summaryLogs = new();

        void EnhanceIfFound(string? Name, EnhancementType type, CapeSpecial cape = CapeSpecial.None, HelmSpecial helm = HelmSpecial.None, WeaponSpecial weapon = WeaponSpecial.None)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            if (!Core.CheckInventory(Name))
            {
                string key = Name.ToLowerInvariant().Trim();
                if (allItemNames.Contains(key))
                    Core.Logger($"[WARN] \"{Name}\" is in inventory/bank but may have a capitalization/spacing mismatch.");
                else
                    Core.Logger($"[MISSING] Enhancement target \"{Name}\" not found in inventory or bank.");
                return;
            }

            if (!skipEnh)
            {
                Adv.EnhanceItem(Name, type, cape, helm, weapon, logging: false);
                summaryLogs.Add($"- {Name}: {type}" +
                    (weapon != WeaponSpecial.None ? $" ({weapon})" :
                     cape != CapeSpecial.None ? $" ({cape})" :
                     helm != HelmSpecial.None ? $" ({helm})" : ""));
            }
        }

        // Static class enhancements
        EnhanceIfFound(voidhighlord, EnhancementType.Lucky);
        EnhanceIfFound(verusdoomdnight, EnhancementType.Lucky);
        EnhanceIfFound(dragonoftime, EnhancementType.Healer);
        if (Core.CheckInventory(chaosAvenger))
            EnhanceIfFound(chaosAvenger, EnhancementType.Lucky);
        if (Core.CheckInventory(archmage))
            EnhanceIfFound(archmage, EnhancementType.Lucky);
        if (Core.CheckInventory(legionrevenant))
            EnhanceIfFound(archmage, EnhancementType.Wizard);

        // Weapon enhancements
        EnhanceIfFound(gear["Valiance"], EnhancementType.Lucky, weapon: WeaponSpecial.Valiance);
        EnhanceIfFound(gear["Dauntless"], EnhancementType.Lucky, weapon: WeaponSpecial.Dauntless);
        EnhanceIfFound(gear["Elysium"], EnhancementType.Healer, weapon: WeaponSpecial.Elysium);

        // Helm enhancements
        EnhanceIfFound(gear["LuckHelm"], EnhancementType.Lucky);
        EnhanceIfFound(gear["HealerHelm"], EnhancementType.Healer);
        EnhanceIfFound(gear["AnimaHelm"], EnhancementType.Lucky, helm: HelmSpecial.Anima);

        // Cape enhancements
        EnhanceIfFound(gear["HealerCape"], EnhancementType.Healer);
        EnhanceIfFound(gear["Penitence"], EnhancementType.Lucky, cape: CapeSpecial.Penitence);
        EnhanceIfFound(gear["Vainglory"], EnhancementType.Lucky, cape: CapeSpecial.Vainglory);

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
