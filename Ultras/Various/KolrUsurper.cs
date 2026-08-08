/*
name: KolrUsurper
description: Two-player Kolr, Usurper of Flames script with selectable DPS and optional Great Flame of Yew farming.
tags: ultra, army, two-player, usurper, kolr, usurper of flames, verus doomknight, legion revenant, kings echo, void highlord, hollowborn vindicator, chaos avenger, archpaladin, lonewolf12
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LoneWolf_UltraUsurper
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv = new();
    private CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "LoneWolf_UltraUsurper";

    private const string LogPrefix = "[Ultra Usurper]";

    private const string BossMap = "flameusurper";
    private const string BossCell = "r2";
    private const string BossPad = "Bottom";
    private const string SafeCell = "Enter";
    private const string SafePad = "Spawn";

    private const string BossName = "Kolr, Usurper of Flames";
    private const int BossMapId = 1;
    private const int FightLoopSleepMs = 25;
    private const int HealRequestPollMs = 100;
    private const int BossRespawnTimeoutSeconds = 30;

    private const int AccessQuestId = 10714;
    private const string AccessQuestName = "Wave Goodbye";
    private const int DailyQuestId = 10715;
    private const string DefeatItem = "Choronzonite";
    private const string RewardItem = "Cinders of a Champion";
    private const string BossDropItem = "Great Flame of Yew";
    private const int BossDropMaxStack = 100;
    private const int MaxNoProgressKills = 3;

    private const string LegionRevenantClass = "Legion Revenant";
    private const string KingsEchoClass = "King's Echo";
    private const string VoidHighlordClass = "Void Highlord";
    private const string VerusDoomKnightClass = "Verus DoomKnight";
    private const string HollowbornVindicatorClass = "Hollowborn Vindicator";
    private const string ChaosAvengerClass = "Chaos Avenger";
    private const string ArchPaladinClass = "ArchPaladin";
    private const string Player2Class = "Lord of Order";

    private const string ScrollName = "Scroll of Enrage";
    private const string HonorPotionName = "Potent Honor Potion";
    private const string HonorAuraName = "Potent Honor Malice";
    private const string FelicitousPhiltreName = "Felicitous Philtre";
    private const string UnleashedDoomAura = "Unleashed Doom";
    private const string HollowAura = "Hollow";
    private const string PutrefactionAura = "Putrefaction";

    private const string SyncFilePrefix = "LoneWolf_UltraUsurper";
    private const string SyncMutexPrefix = "AQW_LoneWolf_UltraUsurper";
    private const int SyncPollMs = 250;
    private const int SyncTimeoutMs = 120000;
    private const int StartupSignalFreshnessSeconds = 120;
    private const int DeathsRequiredForReset = 2;

    private static readonly string[] ExpectedPlayers = { "Player1", "Player2" };

    private Mutex? syncMutex;

    private PlayerRole role = PlayerRole.Unselected;
    private DpsClass selectedDpsClass = DpsClass.VerusDoomKnight;
    private int skillIndex;
    private string syncFileName = string.Empty;
    private string syncFilePath = string.Empty;
    private string runId = string.Empty;
    private int privateRoomNumber;
    private int lastHandledResetCycle;
    private int encounterAttempt;
    private bool bossSeenAlive;
    private bool fightActive;
    private DateTime nextDeathCountCheckAt = DateTime.MinValue;

    private UsurperPhase currentPhase = UsurperPhase.None;

    private bool phase1OpeningSkill4Used;
    private bool phase2OpeningTauntDone;
    private bool phase2FocusSeen;
    private bool phase2PostFocusSkill4Done;
    private bool mechanicUsedSkillThisLoop;
    private bool fatalFailureReported;
    private bool phaseFiveStarted;
    private bool? lastPublishedLrHealRequest;
    private bool cachedLrNeedsHeal;
    private DateTime nextHealRequestPollAt = DateTime.MinValue;
    private bool farmGreatFlame;
    private int killCycle;
    private int consecutiveNoProgressKills;

    private int kingsEchoStrictIndex;
    private bool kingsEchoSkill3Pending;
    private int vdkSkillIndex;

    private readonly int[] lrSkills = { 3, 4, 1, 2 };
    private readonly int[] kingsEchoSkills =
    {
        1, 2, 1, 2, 1, 2, 1, 2, 4
    };
    private readonly int[] vhlPhase1Skills = { 2, 4 };
    private readonly int[] vhlCombatSkills = { 1, 2, 4 };
    private readonly int[] vdkSkills =
    {
        1, 2, 3, 4,
        1, 2, 3,
        1, 2, 3,
        4, 1, 2, 3,
        1, 2, 4, 3,
        1, 2, 3
    };
    private readonly int[] hbvSkills = { 1, 2, 3, 4 };
    private readonly int[] cavSkills = { 3, 4, 2, 1 };
    private readonly int[] archPaladinSkills = { 2, 3, 1, 4 };
    private readonly int[] looHeldSkills = { 3, 1 };
    private readonly int[] looCombatSkills = { 4, 3, 1 };
    private readonly int[] looPreparationSkills = { 4, 2, 3, 1 };

    public List<IOption> Options = new()
    {
        new Option<string>(
            "player1",
            "Player 1 - DPS",
            "Username assigned to Player 1 using the selected DPS class.",
            ""
        ),
        new Option<string>(
            "player2",
            "Player 2 - Lord of Order",
            "Username assigned to Player 2 using Lord of Order.",
            ""
        ),
        new Option<DpsClass>(
            "dpsClass",
            "DPS Class",
            "Select the DPS class used by Player 1. Use the same setting on both clients.",
            DpsClass.VerusDoomKnight
        ),
        new Option<bool>(
            "applyEnhancements",
            "Apply Enhancements",
            "Apply the required enhancements for the assigned role.",
            true
        ),
        new Option<bool>(
            "usePotions",
            "Use Potions",
            "Buy and use the selected DPS class's required tonic, elixir, and third potion.",
            true
        ),
        new Option<bool>(
            "farmGreatFlame",
            "Farm Great Flame of Yew",
            "Repeatedly defeat Usurper until both players own 100 Great Flame of Yew. Use the same setting on both clients.",
            false
        )
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Bot.Config?.Configure();
        RunUltra(stopBotWhenFinished: true);
    }

    public bool RunFromMaster()
    {
        return RunUltra(stopBotWhenFinished: false);
    }

    private bool RunUltra(bool stopBotWhenFinished)
    {
        Bot.Options.InfiniteRange = true;
        Bot.Skills.Stop();
        fatalFailureReported = false;

        try
        {
            if (!Initialize())
                return false;

            if (!ValidateSharedConfiguration())
                return false;

            if (!Setup())
                return false;

            return RunConfiguredFlow();
        }
        catch (Exception ex)
        {
            return Fail($"Unexpected script error: {ex.Message}");
        }
        finally
        {
            fightActive = false;
            Bot.Combat.CancelAutoAttack();
            Bot.Combat.CancelTarget();
            Bot.Skills.Stop();

            if (stopBotWhenFinished)
                Bot.StopAsync();
        }
    }

    private bool Initialize()
    {
        string[] configuredPlayers =
        {
            Bot.Config?.Get<string>("player1")?.Trim() ?? string.Empty,
            Bot.Config?.Get<string>("player2")?.Trim() ?? string.Empty
        };

        if (configuredPlayers.Any(string.IsNullOrWhiteSpace))
            return Fail("Both player slots must be configured.");

        if (configuredPlayers
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() != ExpectedPlayers.Length)
        {
            return Fail("Each account must be assigned to exactly one player slot.");
        }

        string username = Bot.Player.Username?.Trim() ?? string.Empty;

        if (username.Equals(configuredPlayers[0], StringComparison.OrdinalIgnoreCase))
            role = PlayerRole.Player1;
        else if (username.Equals(configuredPlayers[1], StringComparison.OrdinalIgnoreCase))
            role = PlayerRole.Player2;

        if (role == PlayerRole.Unselected)
            return Fail("This account is not assigned to a player slot.");

        selectedDpsClass = Bot.Config?.Get<DpsClass>("dpsClass")
            ?? DpsClass.VerusDoomKnight;

        ConfigurePairIdentity(configuredPlayers[0], configuredPlayers[1]);

        string assignedClass = role == PlayerRole.Player1
            ? GetDpsClassName()
            : Player2Class;

        Core.Logger($"{LogPrefix} Assigned role: {role} ({assignedClass}).");
        Core.Logger($"{LogPrefix} Using automatically assigned private room {privateRoomNumber}.");
        Core.Logger(
            $"{LogPrefix} One death will respawn and rejoin. If both players die, the encounter will reset."
        );

        return InitializeSync();
    }

    private void ConfigurePairIdentity(string lrUsername, string looUsername)
    {
        string identity = string.Join(
            "|",
            BossMap,
            lrUsername.Trim().ToLowerInvariant(),
            looUsername.Trim().ToLowerInvariant()
        );

        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(identity));

        uint roomSeed =
            ((uint)hash[0] << 24)
            | ((uint)hash[1] << 16)
            | ((uint)hash[2] << 8)
            | hash[3];

        privateRoomNumber = 1000 + (int)(roomSeed % 99000u);

        string pairId = BitConverter
            .ToString(hash, 4, 6)
            .Replace("-", string.Empty);

        syncFileName = $"{SyncFilePrefix}_{pairId}.sync";
        string syncMutexName = $"{SyncMutexPrefix}_{pairId}_Sync";
        syncMutex = new Mutex(false, syncMutexName);

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = privateRoomNumber;
    }

    private bool ValidateSharedConfiguration()
    {
        bool accessComplete = Core.isCompletedBefore(AccessQuestId);
        bool selectedFarmMode =
            Bot.Config?.Get<bool>("farmGreatFlame") ?? false;

        if (!UpdateSyncEntry($"{role}.AccessQuest", $"{runId}|{(accessComplete ? 1 : 0)}"))
        {
            return Fail($"{role} could not publish the access-quest result.");
        }

        if (!UpdateSyncEntry($"{role}.FarmMode", $"{runId}|{(selectedFarmMode ? 1 : 0)}"))
        {
            return Fail($"{role} could not publish the farming option.");
        }

        if (!UpdateSyncEntry($"{role}.DpsClass", $"{runId}|{(int)selectedDpsClass}"))
        {
            return Fail($"{role} could not publish the DPS class selection.");
        }

        if (!WaitForPhase("Configuration"))
            return false;

        Dictionary<string, string> entries = ReadSyncEntries();

        string requiredAccess = $"{runId}|1";
        string expectedMode = $"{runId}|{(selectedFarmMode ? 1 : 0)}";
        string expectedDpsClass = $"{runId}|{(int)selectedDpsClass}";

        foreach (string player in ExpectedPlayers)
        {
            if (!entries.TryGetValue($"{player}.AccessQuest", out string? accessValue)
                || !accessValue.Equals(requiredAccess, StringComparison.Ordinal))
            {
                return Fail(
                    $"Both players must have completed {AccessQuestName} (quest {AccessQuestId}) before this script can start."
                );
            }

            if (!entries.TryGetValue($"{player}.FarmMode", out string? modeValue)
                || !modeValue.Equals(expectedMode, StringComparison.Ordinal))
            {
                return Fail("Both players must use the same Farm Great Flame of Yew option.");
            }

            if (!entries.TryGetValue($"{player}.DpsClass", out string? classValue)
                || !classValue.Equals(expectedDpsClass, StringComparison.Ordinal))
            {
                return Fail("Both players must select the same Player 1 DPS class.");
            }
        }

        Core.Logger(
            $"{LogPrefix} Access prerequisite complete: {AccessQuestName} [{AccessQuestId}]."
        );

        farmGreatFlame = selectedFarmMode;

        Core.Logger(
            farmGreatFlame
                ? $"{LogPrefix} Great Flame farming is enabled."
                : $"{LogPrefix} Great Flame farming is disabled; running one kill."
        );
        Core.Logger($"{LogPrefix} Player1 DPS class: {GetDpsClassName()}.");

        return true;
    }

    private bool Setup()
    {
        skillIndex = 0;
        lastHandledResetCycle = 0;
        encounterAttempt = 0;
        killCycle = 0;
        consecutiveNoProgressKills = 0;
        ResetEncounterState();

        UnbankTrackedItems();
        PrepareQuest();

        if (!PrepareRequiredClass())
            return false;

        ApplyEnhancements();
        RestockScrolls();
        EnsurePotionStock();

        Core.Join(BossMap);
        Bot.Wait.ForMapLoad(BossMap);

        if (Bot.ShouldExit
            || !string.Equals(Bot.Map.Name, BossMap, StringComparison.OrdinalIgnoreCase))
        {
            return Fail($"Failed to join {BossMap}.");
        }

        if (!MoveTo(SafeCell, SafePad))
            return Fail($"Could not reach {SafeCell}, {SafePad} during setup.");

        Bot.Player.SetSpawnPoint();
        Bot.Sleep(500);

        Core.Logger($"{LogPrefix} Joined {BossMap} and prepared at {SafeCell}, {SafePad}.");

        if (!WaitForPhase("Setup"))
            return false;

        UsePreparedPotions();

        if (!PrepareSkill5Item())
            return false;

        return WaitForPhase("FightReady");
    }

    private void UnbankTrackedItems()
    {
        Bot.Bank.Open();
        Bot.Bank.Load();
        Bot.Sleep(500);

        UnbankItem(RewardItem);
        UnbankItem(BossDropItem);

        void UnbankItem(string itemName)
        {
            InventoryItem? bankItem = Bot.Bank.Items.FirstOrDefault(item =>
                item != null
                && item.Name.Equals(
                    itemName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (bankItem == null)
                return;

            int itemId = bankItem.ID;
            int quantityBefore = Bot.Inventory.Items
                .Where(item => item != null && item.ID == itemId)
                .Sum(item => item.Quantity);

            Bot.Bank.ToInventory(itemId);

            if (Bot.Wait.ForTrue(
                    () => Bot.Inventory.Items
                        .Where(item => item != null && item.ID == itemId)
                        .Sum(item => item.Quantity) > quantityBefore,
                    50
                ))
            {
                Core.Logger($"{LogPrefix} Unbanked {itemName}.");
                return;
            }

            Warn($"{itemName} is still in the bank and needs to be unbanked manually.");
        }
    }

    private void PrepareQuest()
    {
        Core.AddDrop(DefeatItem);
        Core.AddDrop(RewardItem);
        Core.AddDrop(BossDropItem);

        if (Bot.Quests.IsInProgress(DailyQuestId))
        {
            Core.Logger($"{LogPrefix} Quest {DailyQuestId} is already active.");
            return;
        }

        if (Bot.Quests.IsDailyComplete(DailyQuestId))
        {
            Core.Logger(
                $"{LogPrefix} Quest {DailyQuestId} is already complete. Continuing as party support."
            );
            return;
        }

        bool accepted = Core.EnsureAccept(DailyQuestId);

        if (accepted && Bot.Wait.ForTrue(() => Bot.Quests.IsInProgress(DailyQuestId), 20))
        {
            Core.Logger($"{LogPrefix} Quest {DailyQuestId} accepted.");
            return;
        }

        Warn($"Quest {DailyQuestId} could not be accepted. Continuing as party support only.");
    }

    private bool PrepareRequiredClass()
    {
        string requiredClass = role == PlayerRole.Player1
            ? GetDpsClassName()
            : Player2Class;

        InventoryItem? classItem = Bot.Inventory.Items.FirstOrDefault(item =>
            item != null
            && item.Category == ItemCategory.Class
            && item.Name.Equals(requiredClass, StringComparison.OrdinalIgnoreCase)
        );

        if (classItem == null)
        {
            Bot.Bank.Open();
            Bot.Bank.Load();
            Bot.Sleep(500);

            classItem = Bot.Bank.Items.FirstOrDefault(item =>
                item != null
                && item.Category == ItemCategory.Class
                && item.Name.Equals(requiredClass, StringComparison.OrdinalIgnoreCase)
            );

            if (classItem != null)
            {
                int classItemId = classItem.ID;
                Bot.Bank.ToInventory(classItemId);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(classItemId), 20);

                classItem = Bot.Inventory.Items.FirstOrDefault(item =>
                    item != null && item.ID == classItemId
                );
            }
        }

        if (classItem == null)
            return Fail($"{role} does not own the required class: {requiredClass}.");

        if (!Bot.Inventory.IsEquipped(classItem.ID))
        {
            Core.Equip(classItem.ID);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(classItem.ID), 20);
        }

        if (!string.Equals(
                Bot.Player.CurrentClass?.Name,
                requiredClass,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            return Fail($"Failed to equip {requiredClass} for {role}.");
        }

        Core.Logger($"{LogPrefix} {role} equipped {requiredClass}.");
        return true;
    }

    private void ApplyEnhancements()
    {
        if (!(Bot.Config?.Get<bool>("applyEnhancements") ?? true))
            return;

        try
        {
            if (role == PlayerRole.Player2)
            {
                if (!Adv.uPenitence())
                {
                    Warn(
                        "Player2 has not unlocked Penitence. Applying the available enhancements and continuing."
                    );
                }

                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.None,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Penitence
                );

                Core.Logger(
                    $"{LogPrefix} Player2 enhancement setup: Lucky / Lucky / Health Vamp / Penitence."
                );
                return;
            }

            void WarnMissing(string className)
            {
                Warn(
                    $"{className} is missing one or more requested enhancement unlocks. Applying what is available and continuing."
                );
            }

            switch (selectedDpsClass)
            {
                case DpsClass.VerusDoomKnight:
                    if (!Adv.uForgeHelm() || (!Adv.uRavenous() && !Adv.uValiance()))
                    {
                        WarnMissing(VerusDoomKnightClass);
                    }

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: Adv.uRavenous()
                            ? WeaponSpecial.Ravenous
                            : WeaponSpecial.Valiance,
                        cSpecial: CapeSpecial.Vainglory
                    );

                    Core.Logger(
                        $"{LogPrefix} VDK enhancement setup: Lucky / Forge / Ravenous or Valiance / Vainglory."
                    );
                    break;

                case DpsClass.LegionRevenant:
                    if (!Adv.uForgeHelm() || !Adv.uPenitence() || !Adv.uRavenous())
                    {
                        WarnMissing(LegionRevenantClass);
                    }

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Wizard,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: WeaponSpecial.Ravenous,
                        cSpecial: CapeSpecial.Penitence
                    );

                    Core.Logger(
                        $"{LogPrefix} Legion Revenant enhancement setup: Wizard / Forge / Ravenous / Penitence."
                    );
                    break;

                case DpsClass.KingsEcho:
                    if (!Adv.uRavenous() && !Adv.uValiance())
                        WarnMissing(KingsEchoClass);

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Examen,
                        wSpecial: Adv.uRavenous()
                            ? WeaponSpecial.Ravenous
                            : WeaponSpecial.Valiance,
                        cSpecial: CapeSpecial.Vainglory
                    );

                    Core.Logger(
                        $"{LogPrefix} King's Echo enhancement setup: Lucky / Examen / Ravenous or Valiance / Vainglory."
                    );
                    break;

                case DpsClass.VoidHighlord:
                    if (!Adv.uForgeHelm() || (!Adv.uRavenous() && !Adv.uValiance()))
                    {
                        WarnMissing(VoidHighlordClass);
                    }

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: Adv.uRavenous()
                            ? WeaponSpecial.Ravenous
                            : WeaponSpecial.Valiance,
                        cSpecial: CapeSpecial.Vainglory
                    );

                    Core.Logger(
                        $"{LogPrefix} Void Highlord enhancement setup: Lucky / Forge / Ravenous or Valiance / Vainglory."
                    );
                    break;

                case DpsClass.HollowbornVindicator:
                    if (!Adv.uForgeHelm()
                        || (!Adv.uRavenous()
                            && !Adv.uValiance()
                            && !Adv.uPraxis()))
                    {
                        WarnMissing(HollowbornVindicatorClass);
                    }

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: Adv.uRavenous()
                            ? WeaponSpecial.Ravenous
                            : Adv.uValiance()
                                ? WeaponSpecial.Valiance
                                : WeaponSpecial.Praxis,
                        cSpecial: CapeSpecial.Vainglory
                    );

                    Core.Logger(
                        $"{LogPrefix} HBV enhancement setup: Lucky / Forge / Ravenous, Valiance, or Praxis / Vainglory."
                    );
                    break;

                case DpsClass.ChaosAvenger:
                    if (!Adv.uForgeHelm() || !Adv.uValiance())
                        WarnMissing(ChaosAvengerClass);

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: WeaponSpecial.Valiance,
                        cSpecial: CapeSpecial.Vainglory
                    );

                    Core.Logger(
                        $"{LogPrefix} CAV enhancement setup: Lucky / Forge / Valiance / Vainglory."
                    );
                    break;

                case DpsClass.ArchPaladin:
                    if (!Adv.uForgeHelm() || !Adv.uValiance() || !Adv.uLament())
                    {
                        WarnMissing(ArchPaladinClass);
                    }

                    Adv.EnhanceEquipped(
                        type: EnhancementType.Lucky,
                        hSpecial: HelmSpecial.Forge,
                        wSpecial: WeaponSpecial.Valiance,
                        cSpecial: CapeSpecial.Lament
                    );

                    Core.Logger(
                        $"{LogPrefix} AP enhancement setup: Lucky / Forge / Valiance / Lament."
                    );
                    break;
            }
        }
        catch (Exception ex)
        {
            Warn($"Enhancement setup failed: {ex.Message}. Continuing with the current gear.");
        }
    }

    private void RestockScrolls(bool logOnly = false)
    {
        if (role != PlayerRole.Player2)
            return;

        const int restockThreshold = 80;
        const int restockQuantity = 100;
        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";

        if (Core.CheckInventory(ScrollName, restockThreshold))
        {
            Core.Logger(
                $"{LogPrefix} Player2 already has at least {restockThreshold} {ScrollName}."
            );
            return;
        }

        bool hasSpellCraftingRank5 =
            Bot.Reputation?.FactionList?.Any(faction =>
                faction != null
                && string.Equals(
                    faction.Name,
                    "SpellCrafting",
                    StringComparison.OrdinalIgnoreCase
                )
                && faction.Rank >= 5
            ) == true;

        if (!hasSpellCraftingRank5)
        {
            Warn(
                $"SpellCrafting rank 5 is required to restock {ScrollName}. Continuing with the current inventory.",
                logOnly
            );
            return;
        }

        Core.Logger(
            $"{LogPrefix} Player2 has fewer than {restockThreshold} {ScrollName}. Restocking to {restockQuantity}."
        );

        try
        {
            Core.AddDrop(parchment);
            Core.Join("underworld");
            Bot.Wait.ForMapLoad("underworld");

            if (Bot.ShouldExit)
                return;

            if (!string.Equals(Bot.Map.Name, "underworld", StringComparison.OrdinalIgnoreCase))
            {
                Warn(
                    $"Failed to join underworld while restocking {ScrollName}. Continuing with the current inventory.",
                    logOnly
                );
                return;
            }

            DateTime parchmentFarmDeadline = DateTime.UtcNow.AddMinutes(5);

            while (!Bot.ShouldExit
                    && !Core.CheckInventory(parchment, 2)
                    && DateTime.UtcNow < parchmentFarmDeadline)
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                var monster = Bot.Monsters?.MapMonsters?.FirstOrDefault(m =>
                    m != null
                    && m.HP > 0
                    && string.Equals(
                        m.Name,
                        "Undead Infantry",
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (monster == null)
                {
                    Bot.Sleep(250);
                    continue;
                }

                if (!string.Equals(
                        Bot.Player.Cell,
                        monster.Cell,
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    Core.Jump(monster.Cell, "Left");
                    Bot.Wait.ForCellChange(monster.Cell);
                }

                if (!Bot.Player.HasTarget
                    || Bot.Player.Target?.MapID != monster.MapID
                    || Bot.Player.Target?.HP <= 0)
                {
                    Bot.Combat.Attack(monster.MapID);
                }

                CustomSkillEngine();

                if (Bot.Drops.Exists(parchment))
                {
                    Bot.Drops.Pickup(parchment);
                    Bot.Wait.ForPickup(parchment);
                }
            }

            Bot.Combat.CancelTarget();

            if (!Core.CheckInventory(parchment, 2))
            {
                if (!Bot.ShouldExit)
                {
                    Warn(
                        $"Timed out while obtaining 2 {parchment}. Continuing without restocking {ScrollName}.",
                        logOnly
                    );
                }

                return;
            }

            Core.BuyItem("dragonrune", 549, ink, 5);

            if (!Core.CheckInventory(ink, 5))
            {
                Warn(
                    $"Failed to obtain 5 {ink}. The account may not have enough gold. Continuing without restocking.",
                    logOnly
                );
                return;
            }

            Core.Join("spellcraft");
            Bot.Wait.ForMapLoad("spellcraft");
            Core.AddDrop(ScrollName);

            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%");
            Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");

            Bot.Wait.ForTrue(
                () => Core.CheckInventory(ScrollName, restockQuantity)
                    || Bot.Drops.Exists(ScrollName),
                20
            );

            if (Bot.Drops.Exists(ScrollName))
            {
                Bot.Drops.Pickup(ScrollName);
                Bot.Wait.ForPickup(ScrollName);
            }

            if (!Core.CheckInventory(ScrollName, restockQuantity))
            {
                Warn(
                    $"Failed to restock {ScrollName} to {restockQuantity}. Continuing with the current inventory.",
                    logOnly
                );
                return;
            }

            Core.Logger($"{LogPrefix} Player2 restocked {ScrollName} to {restockQuantity}.");
        }
        catch (Exception ex)
        {
            Bot.Combat.CancelTarget();
            Warn(
                $"Scroll restocking failed: {ex.Message}. Continuing with the current inventory.",
                logOnly
            );
        }
    }

    private bool UsesHonorPotion()
    {
        return selectedDpsClass == DpsClass.LegionRevenant
            || selectedDpsClass == DpsClass.VerusDoomKnight;
    }

    private string[] GetRequiredPotions()
    {
        if (!(Bot.Config?.Get<bool>("usePotions") ?? true))
            return Array.Empty<string>();

        if (role != PlayerRole.Player1)
            return Array.Empty<string>();

        return UsesHonorPotion()
            ? new[] { "Fate Tonic", "Potent Battle Elixir", HonorPotionName }
            : new[] { "Fate Tonic", "Potent Battle Elixir", FelicitousPhiltreName };
    }

    private void EnsurePotionStock(bool logOnly = false)
    {
        string[] potions = GetRequiredPotions();

        if (potions.Length == 0)
            return;

        Core.Logger($"{LogPrefix} {role} potion stock: {string.Join(" / ", potions)}.");

        List<string> failures = new();

        foreach (string potion in potions)
        {
            if (!EnsurePotionAvailable(potion))
                failures.Add(potion);
        }

        if (failures.Count > 0)
        {
            Warn(
                $"Could not stock: {string.Join(", ", failures)}. The account may not have enough gold. Continuing with the available inventory.",
                logOnly
            );
        }

        bool EnsurePotionAvailable(string itemName)
        {
            const int shopId = 2036;
            const string shopMap = "alchemyacademy";

            if (Core.CheckInventory(itemName, 1))
                return true;

            bool isTonic = itemName.Equals("Fate Tonic", StringComparison.OrdinalIgnoreCase);

            if (isTonic)
            {
                bool hasAlchemyRank8 =
                    Bot.Reputation?.FactionList?.Any(faction =>
                        faction != null
                        && string.Equals(
                            faction.Name,
                            "Alchemy",
                            StringComparison.OrdinalIgnoreCase
                        )
                        && faction.Rank >= 8
                    ) == true;

                if (!hasAlchemyRank8)
                    return false;
            }

            string voucher;
            int voucherQuantity;
            int potionQuantity;

            switch (itemName)
            {
                case "Fate Tonic":
                    voucher = "Gold Voucher 500k";
                    voucherQuantity = 4;
                    potionQuantity = 10;
                    break;

                case "Potent Battle Elixir":
                    voucher = "Gold Voucher 500k";
                    voucherQuantity = 4;
                    potionQuantity = 8;
                    break;

                case "Potent Honor Potion":
                    voucher = "Gold Voucher 500k";
                    voucherQuantity = 1;
                    potionQuantity = 5;
                    break;

                case "Felicitous Philtre":
                    voucher = "Gold Voucher 100k";
                    voucherQuantity = 2;
                    potionQuantity = 25;
                    break;

                default:
                    return false;
            }

            if (!Core.CheckInventory(voucher, voucherQuantity))
                Core.BuyItem(shopMap, shopId, voucher, voucherQuantity);

            if (!Core.CheckInventory(voucher, voucherQuantity))
                return false;

            Core.BuyItem(shopMap, shopId, itemName, potionQuantity);
            return Core.CheckInventory(itemName, 1);
        }
    }

    private void UsePreparedPotions(bool logOnly = false)
    {
        string[] potions = GetRequiredPotions();

        if (potions.Length == 0)
            return;

        Core.Logger(
            $"{LogPrefix} {role} applying prepared potions: {string.Join(" / ", potions)}."
        );

        List<string> failures = new();

        foreach (string potion in potions)
        {
            if (!UsePotion(potion))
                failures.Add(potion);
            else
                Bot.Sleep(1000);
        }

        if (failures.Count > 0)
        {
            Warn(
                $"Could not apply: {string.Join(", ", failures)}. Continuing with the available effects.",
                logOnly
            );
        }

        bool UsePotion(string itemName)
        {
            string auraName = itemName switch
            {
                "Fate Tonic" => "Fate",
                "Potent Honor Potion" => HonorAuraName,
                _ => itemName
            };

            bool HasAura() => SelfHasAura(auraName);

            if (HasAura())
            {
                Core.Logger($"{LogPrefix} {role} already has {auraName}.");
                return true;
            }

            if (!Core.CheckInventory(itemName, 1))
                return false;

            if (!Bot.Inventory.IsEquipped(itemName))
            {
                Bot.Inventory.EquipUsableItem(itemName);
                Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(itemName), 20);
            }

            if (!Bot.Inventory.IsEquipped(itemName))
                return false;

            Bot.Sleep(1000);

            for (int attempt = 1; attempt <= 3 && !Bot.ShouldExit; attempt++)
            {
                if (HasAura())
                    break;

                Core.UsePotion();

                if (Bot.Wait.ForTrue(HasAura, 5))
                    break;
            }

            if (!HasAura())
                return false;

            Core.Logger($"{LogPrefix} {role} applied {auraName}.");
            return true;
        }
    }

    private bool PrepareSkill5Item(bool logOnly = false)
    {
        string itemName = role == PlayerRole.Player1
            ? UsesHonorPotion()
                ? HonorPotionName
                : FelicitousPhiltreName
            : ScrollName;

        bool required = role == PlayerRole.Player2;

        if (role == PlayerRole.Player1 && !(Bot.Config?.Get<bool>("usePotions") ?? true))
        {
            return true;
        }

        if (!Core.CheckInventory(itemName, 1))
        {
            if (required)
                return Fail($"Player2 does not have {ScrollName}.");

            Warn(
                $"Player1 does not have {itemName}. Continuing without Skill 5 potion maintenance.",
                logOnly
            );
            return true;
        }

        if (!Bot.Inventory.IsEquipped(itemName))
        {
            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(itemName), 20);
        }

        if (!Bot.Inventory.IsEquipped(itemName))
        {
            if (required)
                return Fail($"Failed to equip {ScrollName} for Player2.");

            Warn(
                $"Failed to equip {itemName} for Player1. Continuing without Skill 5 potion maintenance.",
                logOnly
            );
            return true;
        }

        Core.Logger($"{LogPrefix} {role} equipped {itemName} in skill 5.");
        return true;
    }

    private bool RunConfiguredFlow()
    {
        if (!PublishFarmProgress())
            return false;

        if (!WaitForPhase("FarmStart"))
            return false;

        if (!TryReadFarmProgress(
                out int player1Quantity,
                out int player2Quantity,
                out bool player1DailyComplete,
                out bool player2DailyComplete,
                out _,
                out _
            ))
        {
            return Fail("Could not read the starting Great Flame progress.");
        }

        Core.Logger(
            $"{LogPrefix} Great Flame progress: Player1 {player1Quantity}/{BossDropMaxStack}, Player2 {player2Quantity}/{BossDropMaxStack}."
        );

        bool bothAtMax =
            player1Quantity >= BossDropMaxStack
            && player2Quantity >= BossDropMaxStack;

        if (farmGreatFlame && bothAtMax && player1DailyComplete && player2DailyComplete)
        {
            Core.Logger(
                $"{LogPrefix} Both players already own the maximum {BossDropItem} and have completed the daily quest."
            );
            return Finish(successfulKills: 0);
        }

        int successfulKills = 0;

        while (!Bot.ShouldExit)
        {
            killCycle++;
            int quantityBeforeKill = GetBossDropQuantity();

            if (!Fight())
                return false;

            successfulKills++;

            bool dropIncreased = CollectBossDrop(quantityBeforeKill);
            int quantityAfterKill = GetBossDropQuantity();

            if (quantityBeforeKill < BossDropMaxStack
                && quantityAfterKill <= quantityBeforeKill
                && !dropIncreased)
            {
                consecutiveNoProgressKills++;
            }
            else
            {
                consecutiveNoProgressKills = 0;
            }

            HandleQuestCompletion();

            if (!PublishFarmProgress())
                return false;

            if (!WaitForPhase($"Kill{killCycle}Progress"))
                return false;

            if (!TryReadFarmProgress(
                    out player1Quantity,
                    out player2Quantity,
                    out _,
                    out _,
                    out int player1NoProgress,
                    out int player2NoProgress
                ))
            {
                return Fail($"Could not read Great Flame progress after kill {killCycle}.");
            }

            Core.Logger(
                $"{LogPrefix} Great Flame progress after kill {killCycle}: Player1 {player1Quantity}/{BossDropMaxStack}, Player2 {player2Quantity}/{BossDropMaxStack}."
            );

            if (!farmGreatFlame)
                return Finish(successfulKills);

            if (player1Quantity >= BossDropMaxStack && player2Quantity >= BossDropMaxStack)
            {
                return Finish(successfulKills);
            }

            if (player1NoProgress >= MaxNoProgressKills || player2NoProgress >= MaxNoProgressKills)
            {
                return Fail(
                    $"Great Flame farming made no progress for {MaxNoProgressKills} consecutive kills on at least one player."
                );
            }

            if (!PrepareNextKill())
                return false;
        }

        return false;
    }

    private bool PrepareNextKill()
    {
        Bot.Combat.CancelTarget();

        if (!MoveTo(SafeCell, SafePad))
            return Fail($"Could not reach {SafeCell}, {SafePad} between kills.");

        if (!WaitForPhase($"Kill{killCycle}Safe"))
            return false;

        RestockScrolls(logOnly: true);
        EnsurePotionStock(logOnly: true);

        Core.Join(BossMap);
        Bot.Wait.ForMapLoad(BossMap);

        if (!MoveTo(SafeCell, SafePad))
            return Fail($"Could not return to {BossMap} after restocking.");

        UsePreparedPotions(logOnly: true);

        if (!PrepareSkill5Item(logOnly: true))
            return false;

        if (!WaitForPhase($"Kill{killCycle}NextReady"))
            return false;

        Core.Logger($"{LogPrefix} Both players are ready for kill {killCycle + 1}.");
        return true;
    }

    private int GetBossDropQuantity()
    {
        return Math.Min(
            BossDropMaxStack,
            Bot.Inventory.Items
                .Concat(Bot.Bank.Items)
                .Where(item =>
                    item != null
                    && item.Name.Equals(
                        BossDropItem,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .Sum(item => item.Quantity)
        );
    }

    private bool PublishFarmProgress()
    {
        int quantity = GetBossDropQuantity();
        bool dailyComplete = Bot.Quests.IsDailyComplete(DailyQuestId);

        return UpdateSyncEntry(
            $"{role}.FarmProgress",
            $"{runId}|{quantity}|{(dailyComplete ? 1 : 0)}|{consecutiveNoProgressKills}"
        );
    }

    private bool TryReadFarmProgress(
        out int player1Quantity,
        out int player2Quantity,
        out bool player1DailyComplete,
        out bool player2DailyComplete,
        out int player1NoProgress,
        out int player2NoProgress
    )
    {
        player1Quantity = 0;
        player2Quantity = 0;
        player1DailyComplete = false;
        player2DailyComplete = false;
        player1NoProgress = 0;
        player2NoProgress = 0;

        Dictionary<string, string> entries = ReadSyncEntries();

        bool TryParse(string player, out int quantity, out bool dailyComplete, out int noProgress)
        {
            quantity = 0;
            dailyComplete = false;
            noProgress = 0;

            if (!entries.TryGetValue($"{player}.FarmProgress", out string? value))
            {
                return false;
            }

            string[] parts = value.Split('|');

            if (parts.Length != 4
                || !parts[0].Equals(runId, StringComparison.Ordinal)
                || !int.TryParse(parts[1], out quantity)
                || !int.TryParse(parts[3], out noProgress))
            {
                return false;
            }

            dailyComplete =
                parts[2].Equals("1", StringComparison.Ordinal);
            return true;
        }

        return TryParse(
                "Player1",
                out player1Quantity,
                out player1DailyComplete,
                out player1NoProgress
            )
            && TryParse(
                "Player2",
                out player2Quantity,
                out player2DailyComplete,
                out player2NoProgress
            );
    }

    private bool Fight()
    {
        while (!Bot.ShouldExit)
        {
            encounterAttempt++;
            ResetEncounterState();

            if (RunEncounter(out int resetCycle))
                return WaitForPhase($"Kill{killCycle}BossComplete");

            if (resetCycle <= 0)
            {
                if (Bot.ShouldExit || fatalFailureReported)
                    return false;

                return Fail($"Encounter ended without defeating {BossName}.");
            }

            if (!HandleFightReset(resetCycle))
                return false;
        }

        return false;
    }

    private bool RunEncounter(out int resetCycle)
    {
        resetCycle = 0;
        skillIndex = 0;

        if (!SetDeathStatus(false))
            return Fail($"Failed to clear the death status for {role}.");

        if (!MoveTo(BossCell, BossPad))
            return Fail($"Could not reach {BossCell}, {BossPad}.");

        if (killCycle > 1)
        {
            Core.Logger(
                $"{LogPrefix} Waiting up to {BossRespawnTimeoutSeconds} seconds for {BossName} to respawn."
            );
        }

        if (!Bot.Wait.ForTrue(() => GetBossHp() > 0, BossRespawnTimeoutSeconds * 10))
        {
            return Fail(
                $"Could not find {BossName} MapID {BossMapId} after waiting {BossRespawnTimeoutSeconds} seconds."
            );
        }

        if (!WaitForPhase($"EncounterReady{encounterAttempt}"))
            return false;

        bossSeenAlive = GetBossHp() > 0;
        fightActive = true;

        Core.Logger($"{LogPrefix} Fight attempt {encounterAttempt} started against {BossName}.");

        while (!Bot.ShouldExit)
        {
            if (EncounterResetRequested(out resetCycle))
            {
                fightActive = false;
                return false;
            }

            int bossHp = GetBossHp();

            if (bossHp > 0)
                bossSeenAlive = true;

            if (bossSeenAlive && bossHp <= 0)
            {
                fightActive = false;
                Bot.Combat.CancelTarget();
                Core.Logger($"{LogPrefix} {BossName} was defeated.");
                return true;
            }

            HandleFightMechanics(bossHp);
            MaintainCombat();
            Bot.Sleep(FightLoopSleepMs);
        }

        fightActive = false;
        return false;
    }

    private bool MoveTo(string cell, string pad)
    {
        if (!string.Equals(Bot.Map.Name, BossMap, StringComparison.OrdinalIgnoreCase))
        {
            Core.Join(BossMap);
            Bot.Wait.ForMapLoad(BossMap);
        }

        if (!string.Equals(Bot.Map.Name, BossMap, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase))
        {
            Core.Jump(cell, pad);
            Bot.Wait.ForCellChange(cell);
        }

        return string.Equals(Bot.Player.Cell, cell, StringComparison.OrdinalIgnoreCase);
    }

    private bool CollectBossDrop(int quantityBeforeKill)
    {
        int currentQuantity = GetBossDropQuantity();

        if (currentQuantity >= BossDropMaxStack)
        {
            Core.Logger($"{LogPrefix} {role} already owns {BossDropMaxStack} {BossDropItem}.");
            return true;
        }

        DateTime timeout = DateTime.UtcNow.AddSeconds(15);

        while (!Bot.ShouldExit && DateTime.UtcNow < timeout)
        {
            if (Bot.Drops.Exists(BossDropItem))
            {
                Bot.Drops.Pickup(BossDropItem);
                Bot.Wait.ForPickup(BossDropItem);
            }

            currentQuantity = GetBossDropQuantity();

            if (currentQuantity > quantityBeforeKill || currentQuantity >= BossDropMaxStack)
            {
                Core.Logger(
                    $"{LogPrefix} {role} now owns {currentQuantity}/{BossDropMaxStack} {BossDropItem}."
                );
                return true;
            }

            Bot.Sleep(250);
        }

        Warn(
            $"{BossDropItem} did not increase after kill {killCycle}. Continuing to the synchronized progress check.",
            logOnly: true
        );
        return false;
    }

    private bool Finish(int successfulKills)
    {
        Bot.Combat.CancelTarget();

        if (!MoveTo(SafeCell, SafePad))
        {
            Warn($"{role} could not reach {SafeCell}, {SafePad} at the end of the run.");
        }

        if (!WaitForPhase("Finish"))
            return false;

        if (role == PlayerRole.Player1 && !UpdateSyncEntry("Run.Completed", runId))
        {
            Warn("The run-completed marker could not be saved.");
        }

        Core.Logger(
            farmGreatFlame
                ? $"{LogPrefix} Farming complete after {successfulKills} successful kill(s)."
                : $"{LogPrefix} Run complete. {BossName} was defeated."
        );
        return true;
    }

    private void HandleQuestCompletion()
    {
        bool HasDefeatItem() =>
            Bot.TempInv.Contains(DefeatItem, 1)
            || Core.CheckInventory(DefeatItem, 1, false);

        if (Bot.Quests.IsDailyComplete(DailyQuestId))
        {
            Core.Logger(
                $"{LogPrefix} Quest {DailyQuestId} was already completed. Skipping turn-in."
            );
            return;
        }

        if (!Bot.Quests.IsInProgress(DailyQuestId))
        {
            Warn($"Quest {DailyQuestId} is not active, so this account cannot turn it in.");
            return;
        }

        DateTime itemTimeout = DateTime.UtcNow.AddSeconds(20);

        while (!Bot.ShouldExit && !HasDefeatItem() && DateTime.UtcNow < itemTimeout)
        {
            if (Bot.Drops.Exists(DefeatItem))
            {
                Bot.Drops.Pickup(DefeatItem);
                Bot.Wait.ForPickup(DefeatItem);
            }

            Bot.Sleep(250);
        }

        if (!HasDefeatItem())
        {
            Warn($"{DefeatItem} was not received, so quest {DailyQuestId} could not be completed.");
            return;
        }

        for (int attempt = 1; attempt <= 3 && !Bot.ShouldExit; attempt++)
        {
            Core.EnsureComplete(DailyQuestId);

            if (Bot.Wait.ForTrue(
                    () => Bot.Quests.IsDailyComplete(DailyQuestId)
                        || !Bot.Quests.IsInProgress(DailyQuestId),
                    20
                ))
            {
                Core.Logger($"{LogPrefix} Quest {DailyQuestId} completed.");

                if (Bot.Drops.Exists(RewardItem))
                {
                    Bot.Drops.Pickup(RewardItem);
                    Bot.Wait.ForPickup(RewardItem);
                }

                if (!Core.CheckInventory(RewardItem, 1, false))
                {
                    Warn(
                        $"Quest {DailyQuestId} completed, but {RewardItem} was not detected in inventory."
                    );
                }

                return;
            }

            if (attempt < 3)
                Bot.Sleep(1000);
        }

        Warn($"Quest {DailyQuestId} could not be completed after three attempts.");
    }

    private bool EncounterResetRequested(out int resetCycle)
    {
        resetCycle = 0;

        if (TryGetResetCycle(out int sharedResetCycle) && sharedResetCycle > lastHandledResetCycle)
        {
            resetCycle = sharedResetCycle;
            return true;
        }

        if (!Bot.Player.Alive)
            return HandleLocalDeath(out resetCycle);

        if (DateTime.UtcNow < nextDeathCountCheckAt)
            return false;

        nextDeathCountCheckAt = DateTime.UtcNow.AddMilliseconds(SyncPollMs);

        if (CountDeadPlayers() < DeathsRequiredForReset)
            return false;

        resetCycle = GetOrCreateResetCycle();

        if (resetCycle <= 0)
        {
            Fail("Failed to publish the two-player reset signal.");
            resetCycle = 0;
            return true;
        }

        Core.Logger($"{LogPrefix} Both players are dead. Triggered reset cycle {resetCycle}.");
        return true;
    }

    private bool HandleLocalDeath(out int resetCycle)
    {
        resetCycle = 0;
        Bot.Combat.CancelTarget();

        if (!SetDeathStatus(true))
        {
            Fail($"Failed to publish the death status for {role}.");
            return true;
        }

        Core.Logger($"{LogPrefix} {role} died. Waiting to see if a second player dies.");

        while (!Bot.ShouldExit && !Bot.Player.Alive)
        {
            Bot.Combat.CancelTarget();

            if (TryGetResetCycle(out int sharedResetCycle)
                && sharedResetCycle > lastHandledResetCycle)
            {
                resetCycle = sharedResetCycle;
                return true;
            }

            if (CountDeadPlayers() >= DeathsRequiredForReset)
            {
                resetCycle = GetOrCreateResetCycle();

                if (resetCycle <= 0)
                {
                    Fail("Failed to publish the two-player reset signal.");
                    resetCycle = 0;
                    return true;
                }

                Core.Logger(
                    $"{LogPrefix} Both players are dead. Triggered reset cycle {resetCycle}."
                );
                return true;
            }

            Bot.Sleep(SyncPollMs);
        }

        if (Bot.ShouldExit)
            return true;

        if (TryGetResetCycle(out int respawnResetCycle)
            && respawnResetCycle > lastHandledResetCycle)
        {
            resetCycle = respawnResetCycle;
            return true;
        }

        if (!SetDeathStatus(false))
        {
            Fail($"Failed to clear the death status for {role}.");
            return true;
        }

        Core.Logger($"{LogPrefix} {role} respawned. Resuming the current attempt.");

        if (!MoveTo(BossCell, BossPad))
        {
            Fail($"{role} could not return to {BossCell}, {BossPad} after respawning.");
            return true;
        }

        return false;
    }

    private bool HandleFightReset(int resetCycle)
    {
        lastHandledResetCycle = resetCycle;
        fightActive = false;
        Bot.Combat.CancelTarget();

        Core.Logger(
            $"{LogPrefix} Handling reset cycle {resetCycle}. Retreating to {SafeCell}, {SafePad}."
        );

        while (!Bot.ShouldExit && !Bot.Player.Alive)
        {
            Bot.Combat.CancelTarget();
            Bot.Sleep(250);
        }

        if (Bot.ShouldExit)
            return false;

        if (!MoveTo(SafeCell, SafePad))
            return Fail("Could not reach the configured reset room.");

        if (!WaitForPhase($"Reset{resetCycle}"))
            return false;

        ResetEncounterState();

        bool logOnlyResourceFailure = farmGreatFlame && killCycle > 0;
        UsePreparedPotions(logOnlyResourceFailure);

        if (!PrepareSkill5Item(logOnlyResourceFailure))
            return false;

        if (!MoveTo(SafeCell, SafePad))
            return Fail($"Could not return to {BossMap} after reset preparation.");

        if (!WaitForPhase($"RetryReady{resetCycle}"))
            return false;

        Core.Logger(
            $"{LogPrefix} Reset cycle {resetCycle} complete. Starting a new encounter attempt."
        );
        return true;
    }

    private void ResetEncounterState()
    {
        skillIndex = 0;
        bossSeenAlive = false;
        fightActive = false;
        nextDeathCountCheckAt = DateTime.MinValue;

        currentPhase = UsurperPhase.None;

        phase1OpeningSkill4Used = false;
        phase2OpeningTauntDone = false;
        phase2FocusSeen = false;
        phase2PostFocusSkill4Done = false;
        mechanicUsedSkillThisLoop = false;
        phaseFiveStarted = false;
        lastPublishedLrHealRequest = null;
        cachedLrNeedsHeal = false;
        nextHealRequestPollAt = DateTime.MinValue;
        kingsEchoStrictIndex = 0;
        kingsEchoSkill3Pending = false;
        vdkSkillIndex = 0;

        Bot.Combat.CancelTarget();
    }

    private bool SetDeathStatus(bool isDead)
    {
        if (string.IsNullOrWhiteSpace(runId) || encounterAttempt <= 0)
            return false;

        return UpdateSyncEntry(
            $"{role}.Dead",
            $"{runId}|{encounterAttempt}|{(isDead ? "1" : "0")}"
        );
    }

    private int CountDeadPlayers()
    {
        if (string.IsNullOrWhiteSpace(runId) || encounterAttempt <= 0)
            return 0;

        Dictionary<string, string> entries = ReadSyncEntries();
        string deadValue = $"{runId}|{encounterAttempt}|1";

        return ExpectedPlayers.Count(player =>
            entries.TryGetValue($"{player}.Dead", out string? value)
            && value.Equals(deadValue, StringComparison.Ordinal)
        );
    }

    private bool TryGetResetCycle(out int resetCycle)
    {
        resetCycle = 0;

        return TryGetBossSignal("Reset", out string value)
            && int.TryParse(value, out resetCycle)
            && resetCycle > 0;
    }

    private int GetOrCreateResetCycle()
    {
        return WithSyncLock(() =>
        {
            Dictionary<string, string> entries = ReadSyncEntriesUnlocked();
            int currentCycle = 0;

            if (entries.TryGetValue("Boss.Reset", out string? storedValue))
            {
                string prefix = runId + "|";

                if (storedValue.StartsWith(prefix, StringComparison.Ordinal))
                {
                    int.TryParse(
                        storedValue[prefix.Length..],
                        out currentCycle
                    );
                }
            }

            if (currentCycle > lastHandledResetCycle)
                return currentCycle;

            int nextCycle = Math.Max(currentCycle, lastHandledResetCycle) + 1;
            Ultra.UpdateEntry(syncFilePath, "Boss.Reset", $"{runId}|{nextCycle}");

            Dictionary<string, string> updatedEntries = ReadSyncEntriesUnlocked();

            if (!updatedEntries.TryGetValue("Boss.Reset", out string? savedValue)
                || !savedValue.Equals(
                    $"{runId}|{nextCycle}",
                    StringComparison.Ordinal
                ))
            {
                return -1;
            }

            return nextCycle;
        }, -1);
    }

    private void HandleFightMechanics(int bossHp)
    {
        mechanicUsedSkillThisLoop = false;

        if (bossHp <= 0)
            return;

        TargetBoss();

        if (role == PlayerRole.Player1)
        {
            UpdateDpsHealRequest();
            MaintainDpsPotion();

            if (selectedDpsClass != DpsClass.LegionRevenant)
                UpdatePhase();

            return;
        }

        UpdatePhase();
        HandleLooMechanics();
    }

    private void UpdatePhase()
    {
        UsurperPhase detectedPhase = TargetHasAura("Invulnerable")
            ? UsurperPhase.Phase5
            : TargetHasAura("Flames of Rubedo")
                ? UsurperPhase.Phase4
                : TargetHasAura("Flames of Citrinitas")
                    ? UsurperPhase.Phase3
                    : TargetHasAura("Flames of Albedo")
                        ? UsurperPhase.Phase2
                        : TargetHasAura("Flames of Maleno")
                            ? UsurperPhase.Phase1
                            : UsurperPhase.None;

        if (detectedPhase == UsurperPhase.None || detectedPhase <= currentPhase)
        {
            return;
        }

        currentPhase = detectedPhase;

        if (role == PlayerRole.Player2 || selectedDpsClass == DpsClass.VoidHighlord)
        {
            skillIndex = 0;
        }

        if (selectedDpsClass == DpsClass.KingsEcho && detectedPhase >= UsurperPhase.Phase4)
        {
            kingsEchoSkill3Pending = false;
        }

        Core.Logger($"{LogPrefix} Entered {currentPhase}.");
    }

    private void HandleLooMechanics()
    {
        bool UseSkill(int skill)
        {
            if (!Bot.Skills.CanUseSkill(skill))
                return false;

            Bot.Skills.UseSkill(skill);
            mechanicUsedSkillThisLoop = true;
            return true;
        }

        void TauntAndHeal()
        {
            if (Bot.Skills.CanUseSkill(5) && TauntMonster())
            {
                mechanicUsedSkillThisLoop = true;
                return;
            }

            UseSkill(2);
        }

        switch (currentPhase)
        {
            case UsurperPhase.Phase1:
                if (!phase1OpeningSkill4Used)
                {
                    if (UseSkill(4))
                    {
                        phase1OpeningSkill4Used = true;
                        Core.Logger($"{LogPrefix} Player2 used Skill 4 first to begin Phase 1.");
                    }
                    return;
                }

                if (!SelfHasAura("Putrefaction") && UseSkill(2))
                {
                    Core.Logger($"{LogPrefix} Player2 used Skill 2 while Putrefaction was absent.");
                }
                break;

            case UsurperPhase.Phase2:
                if (!phase2OpeningTauntDone)
                {
                    if (TauntMonster())
                    {
                        phase2OpeningTauntDone = true;
                        phase2FocusSeen = TargetHasAura("Focus");
                        mechanicUsedSkillThisLoop = true;
                        Core.Logger($"{LogPrefix} Player2 completed the opening Phase 2 taunt.");
                        return;
                    }

                    UseSkill(2);
                    return;
                }

                if (!phase2FocusSeen)
                {
                    if (TargetHasAura("Focus"))
                    {
                        phase2FocusSeen = true;
                        Core.Logger($"{LogPrefix} Player2 observed the opening Focus aura.");
                    }

                    UseSkill(2);
                    return;
                }

                if (!phase2PostFocusSkill4Done)
                {
                    if (!TargetHasAura("Focus") && UseSkill(4))
                    {
                        phase2PostFocusSkill4Done = true;
                        skillIndex = 0;
                        Core.Logger($"{LogPrefix} Player2 used Skill 4 after Focus faded.");
                        return;
                    }

                    UseSkill(2);
                    return;
                }

                TauntAndHeal();
                break;

            case UsurperPhase.Phase3:
            case UsurperPhase.Phase4:
                TauntAndHeal();
                break;

            case UsurperPhase.Phase5:
                if (Bot.Player.MaxHealth <= 0)
                    break;

                if (Bot.Player.Health * 2 < Bot.Player.MaxHealth && UseSkill(2))
                {
                    Core.Logger(
                        $"{LogPrefix} Player2 used the Phase 5 heal for its own low HP at {Bot.Player.Health:F0}/{Bot.Player.MaxHealth:F0}."
                    );
                    break;
                }

                UpdateHealRequest();

                if (Bot.Player.Health * 2 > Bot.Player.MaxHealth
                    && cachedLrNeedsHeal
                    && UseSkill(2))
                {
                    Core.Logger($"{LogPrefix} Player2 used the Phase 5 heal for Player1.");
                }
                break;
        }
    }

    private void UpdateHealRequest()
    {
        if (DateTime.UtcNow < nextHealRequestPollAt)
            return;

        nextHealRequestPollAt = DateTime.UtcNow.AddMilliseconds(HealRequestPollMs);
        cachedLrNeedsHeal = false;

        if (!TryGetBossSignal("LrNeedsHeal", out string value))
            return;

        string[] parts = value.Split('|');
        cachedLrNeedsHeal =
            parts.Length == 2
            && int.TryParse(parts[0], out int requestAttempt)
            && requestAttempt == encounterAttempt
            && parts[1].Equals("1", StringComparison.Ordinal);
    }

    private void UpdateDpsHealRequest()
    {
        if (!phaseFiveStarted && TargetHasAura("Invulnerable"))
        {
            phaseFiveStarted = true;
            Core.Logger($"{LogPrefix} Player1 detected Phase 5.");
        }

        if (!phaseFiveStarted || Bot.Player.MaxHealth <= 0)
            return;

        bool needsHeal = Bot.Player.Health * 2 < Bot.Player.MaxHealth;

        if (lastPublishedLrHealRequest == needsHeal
            || !PublishBossSignal(
                "LrNeedsHeal",
                $"{encounterAttempt}|{(needsHeal ? 1 : 0)}"
            ))
        {
            return;
        }

        lastPublishedLrHealRequest = needsHeal;

        if (needsHeal)
        {
            Core.Logger(
                $"{LogPrefix} Player1 requested a Phase 5 heal at {Bot.Player.Health:F0}/{Bot.Player.MaxHealth:F0} HP."
            );
        }
    }

    private void MaintainDpsPotion()
    {
        if (!(Bot.Config?.Get<bool>("usePotions") ?? true))
            return;

        bool usesHonorPotion = UsesHonorPotion();

        string itemName = usesHonorPotion
            ? HonorPotionName
            : FelicitousPhiltreName;
        string auraName = usesHonorPotion
            ? HonorAuraName
            : FelicitousPhiltreName;

        if (SelfHasAura(auraName)
            || !Core.CheckInventory(itemName, 1)
            || !Bot.Inventory.IsEquipped(itemName)
            || !Bot.Skills.CanUseSkill(5))
        {
            return;
        }

        Bot.Skills.UseSkill(5);
        mechanicUsedSkillThisLoop = true;
        Core.Logger($"{LogPrefix} Player1 used {itemName} to refresh {auraName}.");
    }

    private void MaintainCombat()
    {
        if (!Bot.Player.Alive || GetBossHp() <= 0)
            return;

        TargetBoss();

        if (mechanicUsedSkillThisLoop)
            return;

        if (role == PlayerRole.Player1 && selectedDpsClass != DpsClass.LegionRevenant)
        {
            if (currentPhase == UsurperPhase.None)
                return;

            switch (selectedDpsClass)
            {
                case DpsClass.KingsEcho:
                    MaintainKingsEchoCombat();
                    break;

                case DpsClass.VoidHighlord:
                    MaintainVoidHighlordCombat();
                    break;

                case DpsClass.VerusDoomKnight:
                    MaintainVerusDoomKnightCombat();
                    break;

                case DpsClass.HollowbornVindicator:
                    MaintainHollowbornVindicatorCombat();
                    break;

                case DpsClass.ChaosAvenger:
                    MaintainChaosAvengerCombat();
                    break;

                case DpsClass.ArchPaladin:
                    MaintainArchPaladinCombat();
                    break;
            }

            return;
        }

        if (role == PlayerRole.Player2)
        {
            if (currentPhase == UsurperPhase.None)
                return;

            if (currentPhase == UsurperPhase.Phase1 && !phase1OpeningSkill4Used)
            {
                return;
            }

            if (currentPhase == UsurperPhase.Phase2 && !phase2OpeningTauntDone)
            {
                return;
            }
        }

        CustomSkillEngine();
    }

    private void MaintainKingsEchoCombat()
    {
        if ((currentPhase == UsurperPhase.Phase2
                || currentPhase == UsurperPhase.Phase3)
            && kingsEchoSkill3Pending)
        {
            if (Bot.Skills.CanUseSkill(3))
            {
                Bot.Skills.UseSkill(3);
                kingsEchoSkill3Pending = false;
            }

            return;
        }

        if ((currentPhase == UsurperPhase.Phase4
                || currentPhase == UsurperPhase.Phase5)
            && Bot.Player.MaxHealth > 0
            && Bot.Player.Health < Bot.Player.MaxHealth * 0.50
            && Bot.Player.Mana > 24
            && Bot.Skills.CanUseSkill(3))
        {
            Bot.Skills.UseSkill(3);
            return;
        }

        int skill = kingsEchoSkills[kingsEchoStrictIndex];
        int previousIndex = kingsEchoStrictIndex;

        StrictSkillRotation(kingsEchoSkills, ref kingsEchoStrictIndex);

        if (kingsEchoStrictIndex != previousIndex
            && skill == 4
            && (currentPhase == UsurperPhase.Phase2
                || currentPhase == UsurperPhase.Phase3))
        {
            kingsEchoSkill3Pending = true;
        }
    }

    private void MaintainVoidHighlordCombat()
    {
        int skill3HealthThreshold =
            currentPhase == UsurperPhase.Phase1 ? 95 : 85;

        if (Bot.Player.MaxHealth > 0
            && Bot.Player.Health * 100
                > Bot.Player.MaxHealth * skill3HealthThreshold
            && Bot.Skills.CanUseSkill(3))
        {
            Bot.Skills.UseSkill(3);
            return;
        }

        CustomSkillEngine();
    }

    private void MaintainVerusDoomKnightCombat()
    {
        int skill = vdkSkills[vdkSkillIndex];

        if (currentPhase == UsurperPhase.Phase1 && skill == 2 && SelfHasAura(UnleashedDoomAura))
        {
            vdkSkillIndex = (vdkSkillIndex + 1) % vdkSkills.Length;
            return;
        }

        StrictSkillRotation(vdkSkills, ref vdkSkillIndex);
    }

    private void MaintainHollowbornVindicatorCombat()
    {
        if (hbvSkills[skillIndex] == 4 && SelfHasAura(HollowAura))
        {
            skillIndex = (skillIndex + 1) % hbvSkills.Length;
            return;
        }

        CustomSkillEngine();
    }

    private void MaintainChaosAvengerCombat()
    {
        for (int offset = 0; offset < cavSkills.Length; offset++)
        {
            int index = (skillIndex + offset) % cavSkills.Length;
            int skill = cavSkills[index];

            if (currentPhase == UsurperPhase.Phase1 && skill == 3)
                continue;

            if (!Bot.Skills.CanUseSkill(skill))
                continue;

            Bot.Skills.UseSkill(skill);
            skillIndex = (index + 1) % cavSkills.Length;
            return;
        }
    }

    private void MaintainArchPaladinCombat()
    {
        bool blockSkill2 =
            currentPhase == UsurperPhase.Phase1
            && SelfHasAura(PutrefactionAura);

        if (currentPhase == UsurperPhase.Phase5
            && Bot.Player.MaxHealth > 0
            && Bot.Player.Health * 2 < Bot.Player.MaxHealth
            && Bot.Skills.CanUseSkill(2))
        {
            Bot.Skills.UseSkill(2);
            return;
        }

        for (int offset = 0; offset < archPaladinSkills.Length; offset++)
        {
            int index = (skillIndex + offset) % archPaladinSkills.Length;
            int skill = archPaladinSkills[index];

            if (blockSkill2 && skill == 2)
                continue;

            if (!Bot.Skills.CanUseSkill(skill))
                continue;

            Bot.Skills.UseSkill(skill);
            skillIndex = (index + 1) % archPaladinSkills.Length;
            return;
        }
    }

    private void StrictSkillRotation(int[] skills, ref int index)
    {
        int skill = skills[index];

        if (!Bot.Skills.CanUseSkill(skill))
            return;

        Bot.Skills.UseSkill(skill);
        index = (index + 1) % skills.Length;
    }

    private bool SelfHasAura(string auraName)
    {
        return Bot.Self.Auras?.Any(aura =>
            aura != null
            && string.Equals(
                aura.Name,
                auraName,
                StringComparison.OrdinalIgnoreCase
            )
        ) == true;
    }

    private bool TargetHasAura(string auraName)
    {
        return Bot.Player.HasTarget
            && Bot.Player.Target?.MapID == BossMapId
            && Bot.Target.Auras?.Any(aura =>
                aura != null
                && string.Equals(
                    aura.Name,
                    auraName,
                    StringComparison.OrdinalIgnoreCase
                )
            ) == true;
    }

    private void TargetBoss()
    {
        if (!Bot.Player.HasTarget
            || Bot.Player.Target?.MapID != BossMapId
            || Bot.Player.Target?.HP <= 0)
        {
            Bot.Combat.Attack(BossMapId);
        }
    }

    private void CustomSkillEngine()
    {
        if (!Bot.Player.Alive || !Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
        {
            return;
        }

        int[] skillList = GetSkillList();

        for (int offset = 0; offset < skillList.Length; offset++)
        {
            int index = (skillIndex + offset) % skillList.Length;
            int skill = skillList[index];

            if (!Bot.Skills.CanUseSkill(skill))
                continue;

            Bot.Skills.UseSkill(skill);
            skillIndex = (index + 1) % skillList.Length;
            return;
        }
    }

    private int[] GetSkillList()
    {
        if (role == PlayerRole.Player1)
        {
            return selectedDpsClass switch
            {
                DpsClass.LegionRevenant => lrSkills,
                DpsClass.KingsEcho => kingsEchoSkills,
                DpsClass.VoidHighlord =>
                    currentPhase == UsurperPhase.Phase1
                        ? vhlPhase1Skills
                        : vhlCombatSkills,
                DpsClass.VerusDoomKnight => vdkSkills,
                DpsClass.HollowbornVindicator => hbvSkills,
                DpsClass.ChaosAvenger => cavSkills,
                DpsClass.ArchPaladin => archPaladinSkills,
                _ => Array.Empty<int>()
            };
        }

        if (currentPhase == UsurperPhase.None)
            return fightActive ? Array.Empty<int>() : looPreparationSkills;

        if (currentPhase == UsurperPhase.Phase1)
            return looCombatSkills;

        if (currentPhase == UsurperPhase.Phase2 && !phase2PostFocusSkill4Done)
        {
            return looHeldSkills;
        }

        return looCombatSkills;
    }

    private bool TauntMonster()
    {
        if (!Bot.Player.Alive || role != PlayerRole.Player2 || GetBossHp() <= 0)
        {
            return false;
        }

        Bot.Combat.Attack(BossMapId);
        Bot.Sleep(100);

        if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
            return false;

        if (Bot.Skills.CanUseSkill(5))
        {
            Bot.Skills.UseSkill(5);
            Core.Logger($"{LogPrefix} Player2 used taunt immediately.");
            Bot.Sleep(150);
            Bot.Combat.Attack(BossMapId);
            return true;
        }

        Bot.Sleep(750);

        if (!Bot.Player.Alive || GetBossHp() <= 0)
            return false;

        Bot.Combat.Attack(BossMapId);
        Bot.Sleep(100);

        if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
            return false;

        if (!Bot.Skills.CanUseSkill(5))
            return false;

        Bot.Skills.UseSkill(5);
        Core.Logger($"{LogPrefix} Player2 used taunt after 750ms.");
        Bot.Sleep(150);
        Bot.Combat.Attack(BossMapId);
        return true;
    }

    private int GetBossHp()
    {
        if (Bot.Monsters?.MapMonsters == null)
            return 0;

        foreach (var monster in Bot.Monsters.MapMonsters)
        {
            if (monster != null && monster.MapID == BossMapId)
                return monster.HP;
        }

        return 0;
    }

    private bool InitializeSync()
    {
        syncFilePath = Ultra.ResolveSyncPath(syncFileName);

        if (string.IsNullOrWhiteSpace(syncFilePath))
            return Fail("Could not resolve the synchronization file path.");

        if (role == PlayerRole.Player1)
        {
            if (!ClearSyncFile())
                return Fail("Player1 could not clear the synchronization file.");

            runId = Guid.NewGuid().ToString("N");

            if (!UpdateSyncEntry("Run.Id", $"{runId}|{DateTime.UtcNow.Ticks}"))
                return Fail("Player1 could not create the synchronized run ID.");

            Core.Logger($"{LogPrefix} Player1 reset the sync file and created the run ID.");
        }
        else if (!WaitForCurrentRun())
        {
            return Fail("Timed out waiting for Player1's synchronized run.");
        }

        return WaitForPhase("Startup", refreshRunId: true);
    }

    private bool WaitForCurrentRun()
    {
        DateTime timeout = DateTime.UtcNow.AddMilliseconds(SyncTimeoutMs);

        while (!Bot.ShouldExit && DateTime.UtcNow < timeout)
        {
            if (TryReadRun(out string? currentRunId))
            {
                Dictionary<string, string> entries = ReadSyncEntries();

                if (entries.TryGetValue($"{role}.Startup", out string? joinedRun)
                    && string.Equals(joinedRun, currentRunId, StringComparison.Ordinal))
                {
                    Bot.Sleep(SyncPollMs);
                    continue;
                }

                runId = currentRunId ?? string.Empty;
                return true;
            }

            Bot.Sleep(SyncPollMs);
        }

        return false;
    }

    private bool WaitForPhase(string phase, bool refreshRunId = false)
    {
        DateTime timeout = DateTime.UtcNow.AddMilliseconds(SyncTimeoutMs);
        string markedRunId = string.Empty;
        int lastReady = -1;

        while (!Bot.ShouldExit && DateTime.UtcNow < timeout)
        {
            if (refreshRunId && TryReadRun(out string? currentRunId))
                runId = currentRunId ?? string.Empty;

            if (string.IsNullOrWhiteSpace(runId))
            {
                Bot.Sleep(SyncPollMs);
                continue;
            }

            if (!markedRunId.Equals(runId, StringComparison.Ordinal))
            {
                if (!UpdateSyncEntry($"{role}.{phase}", runId))
                    return Fail($"Failed to publish the {phase} synchronization marker for {role}.");

                markedRunId = runId;
            }

            Dictionary<string, string> entries = ReadSyncEntries();
            int ready = ExpectedPlayers.Count(player =>
                entries.TryGetValue($"{player}.{phase}", out string? value)
                && string.Equals(value, runId, StringComparison.Ordinal)
            );

            if (ready != lastReady)
            {
                Core.Logger($"{LogPrefix} {phase} sync: {ready}/{ExpectedPlayers.Length}.");
                lastReady = ready;
            }

            if (ready == ExpectedPlayers.Length)
            {
                if (refreshRunId)
                {
                    if (!TryReadRun(out string? confirmedRunId)
                        || !string.Equals(confirmedRunId, runId, StringComparison.Ordinal))
                    {
                        markedRunId = string.Empty;
                        Bot.Sleep(SyncPollMs);
                        continue;
                    }
                }

                Core.Logger($"{LogPrefix} {phase} sync complete.");
                return true;
            }

            Bot.Sleep(SyncPollMs);
        }
        return Fail($"{phase} synchronization timed out.");
    }

    private bool PublishBossSignal(string signal, string value = "1")
    {
        if (string.IsNullOrWhiteSpace(signal) || string.IsNullOrWhiteSpace(runId))
            return false;

        return UpdateSyncEntry($"Boss.{signal}", $"{runId}|{value}");
    }

    private bool TryGetBossSignal(string signal, out string value)
    {
        value = string.Empty;

        if (string.IsNullOrWhiteSpace(signal) || string.IsNullOrWhiteSpace(runId))
            return false;

        Dictionary<string, string> entries = ReadSyncEntries();

        if (!entries.TryGetValue($"Boss.{signal}", out string? storedValue))
            return false;

        string prefix = runId + "|";

        if (!storedValue.StartsWith(prefix, StringComparison.Ordinal))
            return false;

        value = storedValue[prefix.Length..];
        return true;
    }

    private bool TryReadRun(out string? currentRunId)
    {
        currentRunId = string.Empty;
        Dictionary<string, string> entries = ReadSyncEntries();

        if (!entries.TryGetValue("Run.Id", out string? value))
            return false;

        int separator = value.IndexOf('|');

        if (separator <= 0 || !long.TryParse(value[(separator + 1)..], out long ticks))
        {
            return false;
        }

        string id = value[..separator];

        if (entries.TryGetValue("Run.Completed", out string? completedRun)
            && completedRun.Equals(id, StringComparison.Ordinal))
        {
            return false;
        }

        try
        {
            DateTime created = new(ticks, DateTimeKind.Utc);
            TimeSpan age = DateTime.UtcNow - created;

            if (age < TimeSpan.FromSeconds(-5)
                || age > TimeSpan.FromSeconds(StartupSignalFreshnessSeconds))
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        currentRunId = id;
        return true;
    }

    private bool ClearSyncFile() => WithSyncLock(() =>
    {
        Ultra.ClearSyncFile(syncFilePath);
        return File.Exists(syncFilePath)
            && new FileInfo(syncFilePath).Length == 0;
    }, false);

    private Dictionary<string, string> ReadSyncEntries() => WithSyncLock(
        ReadSyncEntriesUnlocked,
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    );

    private bool UpdateSyncEntry(string key, string value) => WithSyncLock(() =>
    {
        Ultra.UpdateEntry(syncFilePath, key, value);
        Dictionary<string, string> entries = ReadSyncEntriesUnlocked();

        return entries.TryGetValue(key, out string? savedValue)
            && savedValue.Equals(value, StringComparison.Ordinal);
    }, false);

    private Dictionary<string, string> ReadSyncEntriesUnlocked()
    {
        Dictionary<string, string> entries =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (string line in Ultra.ReadLines(syncFilePath))
        {
            int firstSeparator = line.IndexOf(':');
            int lastSeparator = line.LastIndexOf(':');

            if (firstSeparator <= 0 || lastSeparator <= firstSeparator)
                continue;

            string key = line[..firstSeparator].Trim();
            string value = line.Substring(
                firstSeparator + 1,
                lastSeparator - firstSeparator - 1
            ).Trim();

            if (!string.IsNullOrWhiteSpace(key))
                entries[key] = value;
        }

        return entries;
    }

    private T WithSyncLock<T>(Func<T> action, T fallback)
    {
        Mutex? mutex = syncMutex;

        if (mutex == null)
            return fallback;

        bool lockTaken = false;

        try
        {
            try
            {
                lockTaken = mutex.WaitOne(5000);
            }
            catch (AbandonedMutexException)
            {
                lockTaken = true;
            }

            if (!lockTaken)
                return fallback;

            return action();
        }
        catch (Exception ex)
        {
            Core.Logger($"{LogPrefix} Sync file error: {ex.Message}");
            return fallback;
        }
        finally
        {
            if (lockTaken)
                mutex.ReleaseMutex();
        }
    }

    private string GetDpsClassName()
    {
        return selectedDpsClass switch
        {
            DpsClass.LegionRevenant => LegionRevenantClass,
            DpsClass.KingsEcho => KingsEchoClass,
            DpsClass.VoidHighlord => VoidHighlordClass,
            DpsClass.VerusDoomKnight => VerusDoomKnightClass,
            DpsClass.HollowbornVindicator => HollowbornVindicatorClass,
            DpsClass.ChaosAvenger => ChaosAvengerClass,
            DpsClass.ArchPaladin => ArchPaladinClass,
            _ => VerusDoomKnightClass
        };
    }

    private void Warn(string message, bool logOnly = false)
    {
        Core.Logger($"{LogPrefix} {message}", messageBox: !logOnly, stopBot: false);
    }

    private bool Fail(string message)
    {
        fatalFailureReported = true;
        Core.Logger($"{LogPrefix} {message}", messageBox: true, stopBot: false);
        return false;
    }

    private enum UsurperPhase
    {
        None = 0,
        Phase1 = 1,
        Phase2 = 2,
        Phase3 = 3,
        Phase4 = 4,
        Phase5 = 5
    }

    private enum DpsClass
    {
        VerusDoomKnight,
        LegionRevenant,
        KingsEcho,
        VoidHighlord,
        HollowbornVindicator,
        ChaosAvenger,
        ArchPaladin
    }

    private enum PlayerRole
    {
        Unselected,
        Player1,
        Player2
    }
}