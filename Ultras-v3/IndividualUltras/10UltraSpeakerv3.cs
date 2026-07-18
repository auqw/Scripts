/*
name: UltraSpeaker_v3
description: Ultra First Speaker v3 — chat-driven taunt system with position management and stasis handling.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraCustomClassSync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/GetScrolls.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/PrerequisitesChecker.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Skua.Core.Interfaces;

public class UltraSpeaker_v3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots2 C => CoreBots2.Instance;
    private static CoreEngine2 Engine => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;

    // Chat-driven taunt roles — 1 ListenTaunter, 3 TruthTaunters
    private const string ListenTaunterClass = "ArchPaladin";
    private const string TruthTaunter1Class = "Lord Of Order";
    private const string TruthTaunter2Class = "StoneCrusher";
    private const string TruthTaunter3Class = "Verus DoomKnight";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { ListenTaunterClass },
        new[] { TruthTaunter1Class },
        new[] { TruthTaunter2Class },
        new[] { TruthTaunter3Class }
    };

    // Chat-listener state
    private int truthTauntTurn;
    private int lastTruthTauntActedTurn;
    private const string TruthTurnSyncFile = "UltraSpeakerTruthTurn.sync";
    private int listenTauntTurn;
    private int lastListenTauntActedTurn;
    private const string ListenTurnSyncFile = "UltraSpeakerListenTurn.sync";
    private int equalizeCount;
    private bool _movingToEqualize;
    private bool _positioned;

    private const int EqualizeX = 200;
    private const int EqualizeY = 380;

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);
        Engine.Boot(); // Use CoreEngine auto-rotation

        if (!new PrerequisitesChecker().PrerequisiteSyncGate(4))
            return;

        Bot.Events.ExtensionPacketReceived += SpeakerMessageListener;
        Bot.Flash.FlashCall += SpeakerFlashListener;
        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= SpeakerMessageListener;
            Bot.Flash.FlashCall -= SpeakerFlashListener;
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool IsTaunter()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        return className == ListenTaunterClass || className == TruthTaunter1Class || className == TruthTaunter2Class || className == TruthTaunter3Class;
    }

    private bool IsListenTaunter()
    {
        return Bot.Player.CurrentClass?.Name == ListenTaunterClass;
    }

    private bool IsTruthTaunter1()
    {
        return Bot.Player.CurrentClass?.Name == TruthTaunter1Class;
    }

    private bool IsTruthTaunter2()
    {
        return Bot.Player.CurrentClass?.Name == TruthTaunter2Class;
    }

    private bool IsTruthTaunter3()
    {
        return Bot.Player.CurrentClass?.Name == TruthTaunter3Class;
    }

    private int MyTruthIndex()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        if (className == TruthTaunter1Class) return 0;
        if (className == TruthTaunter2Class) return 1;
        if (className == TruthTaunter3Class) return 2;
        return -1;
    }

    private int MyListenIndex()
    {
        return Bot.Player.CurrentClass?.Name == ListenTaunterClass ? 0 : -1;
    }

    private int MyRoleIndex()
    {
        string? className = Bot.Player.CurrentClass?.Name;
        if (className == ListenTaunterClass) return 0;
        if (className == TruthTaunter1Class) return 1;
        if (className == TruthTaunter2Class) return 2;
        if (className == TruthTaunter3Class) return 3;
        return -1;
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[UltraSpeaker-v3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "ultra_speaker_class-v3.sync", allowDuplicates);
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        Enh.ApplySpeaker();

        string[] roleNames = { "ListenTaunter", "TruthTaunter1", "TruthTaunter2", "TruthTaunter3" };
        int roleIdx = MyRoleIndex();
        C.Logger($"[UltraSpeaker-v3] Role: {(roleIdx >= 0 ? roleNames[roleIdx] : "Unknown")}");
    }

    private void Fight()
    {
        const string map = "ultraspeaker";
        const string boss = "The First Speaker";
        const string bossDefeatedTemp = "The First Speaker Silenced";

        const string waitSyncFile = "ultra_speaker.sync";
        const string completionSyncFile = "UltraSpeakerCompletion.sync";
        int armySize = 4;

        const int questId = 9173;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        if (!Bot.Quests.IsUnlocked(questId))
            Bot.Quests.UpdateQuest(9125);

        Bot.Options.DisableCollisions = true;
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(TruthTurnSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(ListenTurnSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        bool skipThird = IsTaunter();
        Pots.EnsureRecommendedPotions(skipThird: skipThird, context: "Speaker");
        Scrolls.GetScrollOfEnrage();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(skipThird: skipThird, ensureStock: false, context: "Speaker");

        if (skipThird)
        {
            C.Logger("[UltraSpeaker-v3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
        }

        Engine.Join(map);
        Bot.Sleep(2500);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        DateTime fightStartTime = DateTime.UtcNow;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                // Just respawned — re-sync truth taunt turn counter from sync file
                try
                {
                    string syncFile = Ultra.ResolveSyncPath(TruthTurnSyncFile);
                    string[] lines = Ultra.ReadLines(syncFile);
                    int maxTurn = 0;
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length >= 3 && int.TryParse(parts[1], out int turn) && turn > maxTurn)
                            maxTurn = turn;
                    }
                    if (maxTurn > truthTauntTurn)
                    {
                        truthTauntTurn = maxTurn;
                        lastTruthTauntActedTurn = maxTurn;
                        C.Logger($"[UltraSpeaker-v3] Re-synced truth taunt turn to {truthTauntTurn} after respawn.");
                    }
                }
                catch { }
                // Re-sync listen taunt turn counter from sync file
                try
                {
                    string syncFile = Ultra.ResolveSyncPath(ListenTurnSyncFile);
                    string[] lines = Ultra.ReadLines(syncFile);
                    int maxTurn = 0;
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length >= 3 && int.TryParse(parts[1], out int turn) && turn > maxTurn)
                            maxTurn = turn;
                    }
                    if (maxTurn > listenTauntTurn)
                    {
                        listenTauntTurn = maxTurn;
                        lastListenTauntActedTurn = maxTurn;
                        C.Logger($"[UltraSpeaker-v3] Re-synced listen taunt turn to {listenTauntTurn} after respawn.");
                    }
                }
                catch { }
                // Reset position flag so we walk to safe box again after respawn
                _positioned = false;
                continue;
            }

            // Position management — walk to safe box once on first enter or after death
            if (!_positioned && !_movingToEqualize && Bot.Player?.Cell == "Boss")
            {
                const int minX = 0, maxX = 100;
                const int minY = 485, maxY = 500;

                int randomX = Random.Shared.Next(minX, maxX + 1);
                int randomY = Random.Shared.Next(minY, maxY + 1);
                Bot.Player.WalkTo(randomX, randomY);
                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - randomX) < 40, 10);
                _positioned = true;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("The First Speaker defeated. Finishing quest.");
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            Bot.Combat.Attack(boss);
            Pots.ActivateEquippedPotion();

            Bot.Sleep(500);
        }
    }

    private void SpeakerMessageListener(dynamic packet)
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

            if (data.anims is null)
                return;

            foreach (dynamic anim in data.anims)
            {
                if (anim is null || anim.msg is null)
                    continue;

                string message = (string)anim.msg;

                // Energy Draw — ListenTaunters (cascade 1-2-1-2-1-2)
                if (message.Contains("You shall listen.", StringComparison.OrdinalIgnoreCase))
                {
                    listenTauntTurn++;
                    C.Logger($"Detected: 'You shall listen.' (turn {listenTauntTurn})");
                    if (listenTauntTurn % 2 == MyListenIndex())
                    {
                        try
                        {
                            string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name ?? "Unknown"}".Replace(":", "-");
                            Ultra.UpdateEntry(Ultra.ResolveSyncPath(ListenTurnSyncFile), myKey, listenTauntTurn.ToString());
                        }
                        catch { }
                    }
                    _ = ListenTauntAsync();
                }

                // Magia Draw — TruthTaunters (cascade 1-2-1-2-1-2)
                if (message.Contains("I will make you see the truth.", StringComparison.OrdinalIgnoreCase))
                {
                    truthTauntTurn++;
                    // Only write to sync file when it's our turn — 1 writer per truth taunt
                    if (truthTauntTurn % 3 == MyTruthIndex())
                    {
                        try
                        {
                            string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name ?? "Unknown"}".Replace(":", "-");
                            Ultra.UpdateEntry(Ultra.ResolveSyncPath(TruthTurnSyncFile), myKey, truthTauntTurn.ToString());
                        }
                        catch { }
                    }
                    // Fire immediately — no Fight loop latency
                    _ = TruthTauntAsync();
                }
            }
        }
        catch { }
    }

    private void SpeakerFlashListener(string name, object[] args)
    {
        try
        {
            if (name != "packetFromServer")
                return;

            dynamic? data = null;
            var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
            data = packet?["b"]?["o"];

            if (data == null || data["cmd"]?.ToString() != "ct")
                return;

            // FlashCall version: check anim messages
            if (data["anims"] != null)
            {
                foreach (var anim in data["anims"])
                {
                    if (anim?.msg != null)
                    {
                        string msg = (string)anim.msg;
                        if (msg.Contains("All stand equal beneath the eyes of the Eternal.", StringComparison.OrdinalIgnoreCase))
                        {
                            equalizeCount++;
                            C.Logger($"[UltraSpeaker-v3] Detected 'All stand equal beneath the eyes of the Eternal.' (count {equalizeCount})");

                            // Fire Decay for VDK (removes Scintillation)
                            _ = DecayAsync();

                            // Check auras synchronously before spawning background task
                            if (!Bot.Player.Alive)
                                return;

                            float health = Bot.Player.Health;
                            float corruptionStacks = Engine.GetAuraStacksFloat("Corruption", true);
                            float somberStacks = Engine.GetAuraStacksFloat("Somber", true);
                            bool hasMagiaBurn = Engine.HasAura("Magia Burn", true);
                            bool hasStasis = Engine.HasAura("Stasis", true);
                            bool hasSanctity = Engine.HasAura("Sanctity", true);
                            bool hasLowHp = health < 5100f;

                            if (!((corruptionStacks > 0f || somberStacks > 0f)
                                && !hasMagiaBurn
                                && !hasStasis
                                && !hasSanctity
                                && !hasLowHp))
                            {
                                C.Logger($"[UltraSpeaker-v3] Skipping equalize zone: hp={health:F0} corruption={corruptionStacks:F2} somber={somberStacks:F2} magiaBurn={hasMagiaBurn} stasis={hasStasis} sanctity={hasSanctity}");
                                return;
                            }

                            C.Logger($"[UltraSpeaker-v3] Stepping into equalize zone: corruption={corruptionStacks:F2} somber={somberStacks:F2}");
                            _movingToEqualize = true;

                            // Same pattern as UltraDageListener — Task.Run keeps blocking WalkTo/Wait/Sleep off the game thread
                            _ = Task.Run(() =>
                            {
                                Bot.Player.WalkTo(EqualizeX, EqualizeY);
                                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - EqualizeX) < 40, 10);

                                // Wait for Equalize to hit (3s charge + buffer)
                                Bot.Sleep(6000);

                                // Walk back to bottom-left safe box
                                C.Logger("[UltraSpeaker-v3] Returning to bottom-left safe box after Equalize.");
                                int homeX = Random.Shared.Next(0, 100);
                                int homeY = Random.Shared.Next(485, 500);
                                Bot.Player.WalkTo(homeX, homeY);
                                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - homeX) < 40, 10);
                                _movingToEqualize = false;
                            });
                        }
                    }
                }
            }
        }
        catch { }
    }

    private async Task ListenTauntAsync()
    {
        if (!Bot.Player.Alive || !Bot.Player.HasTarget || !IsListenTaunter())
            return;

        // Guard: only act once per turn (single listen taunter always acts)
        if (listenTauntTurn <= lastListenTauntActedTurn)
            return;

        lastListenTauntActedTurn = listenTauntTurn;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(5);
            await Task.Delay(50);
        }
    }

    private async Task TruthTauntAsync()
    {
        int myIndex = MyTruthIndex();
        if (myIndex < 0 || !Bot.Player.Alive || !Bot.Player.HasTarget)
            return;

        // Guard: only act if this turn is ours (3-way rotation) and we haven't acted yet
        if (truthTauntTurn % 3 != myIndex || truthTauntTurn <= lastTruthTauntActedTurn)
            return;

        lastTruthTauntActedTurn = truthTauntTurn;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(5);
            await Task.Delay(50);
        }
    }

    private async Task DecayAsync()
    {
        if (Bot.Player.CurrentClass?.Name != "Verus DoomKnight" || !Bot.Player.Alive)
            return;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(4);
            await Task.Delay(50);
        }
    }

}