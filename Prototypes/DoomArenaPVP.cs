/*
name: Two Player PvP DoomArena
description: Player 1 challenges Player 2 to a duel, then defeats Player 2. Completes the daily quest.
tags: army, pvp, duel, doomarena, 1v1, trophy, daily, lonewolf12
*/

//cs_include Scripts/CoreBots.cs
using System.IO;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Players;
using Skua.Core.Options;

#nullable enable

public class PvPArmy
{
    private const string DoomArenaMap = "doomarena";
    private const int PvPQuestID = 1063;
    private const string QuestTempItem = "Doomwood Token";
    private const int QuestTempQuantity = 10;
    private const string QuestReward = "1v1 PvP Trophy";
    private const string PreferredClass = "Void Highlord";
    private const int KillLimit = 10;

    private CoreBots Core => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;

    private static readonly Option<string> Player1Option = new(
        "player1",
        "Account #1",
        "Username of the account that sends the duel request and attacks.",
        ""
    );

    private static readonly Option<string> Player2Option = new(
        "player2",
        "Account #2",
        "Username of the account that accepts the duel.",
        ""
    );

    private static readonly Option<bool> LoopForeverOption = new(
        "loopForever",
        "Loop Forever",
        "Continue dueling indefinitely instead of stopping after 10 kills.",
        false
    );

    public string OptionsStorage = "PvPArmy";
    public bool DontPreconfigure = true;

    public List<IOption> Options =
    [
        Player1Option,
        Player2Option,
        LoopForeverOption,
        CoreBots.Instance.SkipOptions,
    ];

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        RunDuel();

        Core.SetOptions(false);
    }

    private void RunDuel()
    {
        string? configuredPlayer1 = Bot.Config?.Get<string>("player1");
        string? configuredPlayer2 = Bot.Config?.Get<string>("player2");

        if (string.IsNullOrWhiteSpace(configuredPlayer1) || string.IsNullOrWhiteSpace(configuredPlayer2))
        {
            Core.Logger("Player 1 and Player 2 must both be configured.");
            return;
        }

        string player1 = configuredPlayer1.ToLower().Trim();
        string player2 = configuredPlayer2.ToLower().Trim();
        string username = Core.Username().ToLower().Trim();
        bool loopForever = Bot.Config?.Get<bool>("loopForever") ?? false;

        if (player1 == player2)
        {
            Core.Logger("Player 1 and Player 2 must be different accounts.");
            return;
        }

        if (username != player1 && username != player2)
        {
            Core.Logger($"This account ({Core.Username()}) is not configured as Player 1 or Player 2.");
            return;
        }

        bool isPlayer1 = username == player1;

        EnableDuelInvites();

        if (isPlayer1)
        {
            EquipPreferredClass();
            Core.AddDrop(QuestReward);

            if (!Bot.Quests.IsDailyComplete(PvPQuestID))
            {
                Core.EnsureAccept(PvPQuestID);
                HandlePvPQuest();
            }
            else
            {
                Core.Logger($"Daily quest {PvPQuestID} has already been completed today.");
            }
        }

        string syncFile = GetSyncFilePath(player1, player2);

        if (isPlayer1)
            ResetRoundSync(syncFile);
        else
            Bot.Skills.Stop();

        try
        {
            if (isPlayer1)
                SendDuelRequest(player2, syncFile, 0);
            else if (WaitForChallengeSent(syncFile, 0))
                AcceptDuelRequest(player1);
            else
                return;

            for (int round = 1; (loopForever || round <= KillLimit) && !Bot.ShouldExit; round++)
            {
                if (isPlayer1)
                {
                    if (!WaitForPlayer(player2))
                    {
                        Core.Logger($"{player2} did not appear after the duel request was accepted.");
                        return;
                    }

                    AttackPlayerUntilDefeated(player2);
                    HandlePvPQuest();

                    if (!loopForever && round == KillLimit)
                    {
                        Core.Logger($"{KillLimit} kills completed.");
                        break;
                    }

                    if (Bot.ShouldExit || !WaitForRoundReady(syncFile, round))
                        return;

                    if (!JoinDoomArena())
                        return;

                    HandlePvPQuest();
                    SendDuelRequest(player2, syncFile, round);
                }
                else
                {
                    if (!WaitForDeathAndRespawn())
                        return;

                    if (!loopForever && round == KillLimit)
                    {
                        Core.Logger($"{KillLimit} deaths completed.");
                        break;
                    }

                    if (!JoinDoomArena())
                        return;

                    Core.Logger("Player 2 reached /doomarena. Waiting 2 seconds.");
                    Core.Sleep(2000);
                    SignalRoundReady(syncFile, round);

                    if (!WaitForChallengeSent(syncFile, round))
                        return;

                    AcceptDuelRequest(player1);
                }
            }
        }
        finally
        {
            if (isPlayer1)
                ResetRoundSync(syncFile);
        }
    }

    private void EquipPreferredClass()
    {
        if (Bot.Inventory.Contains(PreferredClass) || Bot.Bank.Contains(PreferredClass))
            Core.Equip(PreferredClass);
    }

    private void EnableDuelInvites()
    {
        if (Bot.Flash.GetGameObject<bool>("uoPref.bDuel"))
            return;

        Core.Logger("Enabling duel invitations for the PvP script.");
        Core.SendPackets("%xt%zm%cmd%1%uopref%bDuel%true%");
    }

    private void SendDuelRequest(string player2, string syncFile, int round)
    {
        Core.Logger($"Challenging {player2} to a duel.");
        Core.SendPackets($"%xt%zm%duel%1%{player2}%");
        WriteSyncState(syncFile, $"challenged:{round}");

        // Player 2 accepts after two seconds. Allow a small amount of time
        // for the server to place both players into PvP mode.
        Core.Sleep(2500);
    }

    private void AcceptDuelRequest(string player1)
    {
        Core.Sleep(2000);
        Core.Logger($"Accepting {player1}'s duel.");
        Core.SendPackets($"%xt%zm%da%1%{player1}%");
    }

    private bool WaitForDeathAndRespawn()
    {
        Core.Logger("Waiting for Player 2 to be defeated.");

        while (!Bot.ShouldExit && Bot.Player.Alive)
            Core.Sleep(250);

        if (Bot.ShouldExit)
            return false;

        Core.Logger("Player 2 was defeated. Waiting to respawn.");
        while (!Bot.ShouldExit && !Bot.Player.Alive)
            Core.Sleep(250);

        return !Bot.ShouldExit;
    }

    private bool JoinDoomArena()
    {
        string? currentMap = Bot.Map.Name;

        if (!string.Equals(currentMap, DoomArenaMap, StringComparison.OrdinalIgnoreCase))
        {
            Core.Logger($"Joining /{DoomArenaMap}.");
            Core.Join(DoomArenaMap);
        }

        bool joinedDoomArena = Bot.Wait.ForMapLoad(DoomArenaMap);

        if (!joinedDoomArena)
        {
            currentMap = Bot.Map.Name;
            Core.Logger($"Failed to reach /{DoomArenaMap}. Current map: /{currentMap}.");
        }

        return joinedDoomArena && !Bot.ShouldExit;
    }

    private void HandlePvPQuest()
    {
        if (Bot.Quests.IsDailyComplete(PvPQuestID))
            return;

        string? currentMap = Bot.Map.Name;
        if (!string.Equals(currentMap, DoomArenaMap, StringComparison.OrdinalIgnoreCase))
            return;

        if (!Bot.TempInv.Contains(QuestTempItem, QuestTempQuantity))
            return;

        Core.Logger($"Turning in quest {PvPQuestID} for {QuestReward} ({QuestTempQuantity} {QuestTempItem}).");

        if (!Core.EnsureCompleteChoose(PvPQuestID, [QuestReward]))
        {
            Core.Logger($"Failed to turn in quest {PvPQuestID}.");
            return;
        }

        Core.Logger($"Daily quest {PvPQuestID} completed.");
    }

    private bool WaitForPlayer(string playerName, int timeoutSeconds = 20)
    {
        int attempts = timeoutSeconds * 10;

        while (!Bot.ShouldExit && attempts-- > 0)
        {
            if (Bot.Map.TryGetPlayer(playerName, out PlayerInfo? player) && player is not null)
                return true;

            Core.Sleep(100);
        }

        return false;
    }

    private string GetSyncFilePath(string player1, string player2) =>
        Path.Combine(ClientFileSources.SkuaOptionsDIR, $"PvPArmy-{player1}-{player2}.sync");

    private void ResetRoundSync(string syncFile)
    {
        try
        {
            if (File.Exists(syncFile))
                File.Delete(syncFile);
        }
        catch (Exception ex)
        {
            Core.Logger($"Could not reset the PvP synchronization file: {ex.Message}");
        }
    }

    private void SignalRoundReady(string syncFile, int round)
    {
        WriteSyncState(syncFile, $"ready:{round}");
        Core.Logger($"Player 2 is ready for duel round {round + 1}.");
    }

    private bool WaitForRoundReady(string syncFile, int round)
    {
        Core.Logger($"Waiting for Player 2 to prepare duel round {round + 1}.");
        return WaitForSyncState(syncFile, $"ready:{round}");
    }

    private bool WaitForChallengeSent(string syncFile, int round)
    {
        Core.Logger($"Waiting for Player 1 to send duel round {round + 1}.");
        return WaitForSyncState(syncFile, $"challenged:{round}");
    }

    private void WriteSyncState(string syncFile, string state)
    {
        while (!Bot.ShouldExit)
        {
            try
            {
                File.WriteAllText(syncFile, state);
                return;
            }
            catch
            {
                Core.Sleep(250);
            }
        }
    }

    private bool WaitForSyncState(string syncFile, string state)
    {
        while (!Bot.ShouldExit)
        {
            try
            {
                if (File.Exists(syncFile) && File.ReadAllText(syncFile).Trim() == state)
                    return true;
            }
            catch
            {
                // The other client may be writing the file; retry shortly.
            }

            Core.Sleep(250);
        }

        return false;
    }

    private void AttackPlayerUntilDefeated(string playerName)
    {
        int missingAttempts = 0;
        string className = Bot.Player.CurrentClass?.Name ?? "generic";

        Core.Logger($"Starting Advanced Skills for {className}.");
        Bot.Skills.StartAdvanced(className, false);

        try
        {
            while (!Bot.ShouldExit)
            {
                if (!Bot.Map.TryGetPlayer(playerName, out PlayerInfo? player) || player is null)
                {
                    if (++missingAttempts >= 20)
                    {
                        Core.Logger($"{playerName} is no longer in the current map.");
                        return;
                    }

                    Core.Sleep(250);
                    continue;
                }

                missingAttempts = 0;

                if (player.HP <= 0)
                    break;

                // AttackPlayer selects the PvP opponent. Once a target exists,
                // Skua's Advanced Skills timer handles the equipped class's combo.
                Bot.Combat.AttackPlayer(playerName);
                Core.Sleep();
            }
        }
        finally
        {
            Bot.Skills.Stop();
            Bot.Combat.CancelTarget();
        }

        Core.Logger($"{playerName} has 0 HP. Duel sequence complete.");
    }
}
