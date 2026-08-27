/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class CoreStory
{
    // [Can Change]
    // True = Bot only does its smart checks on quests with Once: True
    // False = Bot does it's smart checks on all quest
    // Recommended: false
    // Used for testing bots, dont toggle this as a user
    public bool TestBot { get; set; } = false;

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    #region KillQuest

    /// <summary>
    /// Completes a quest by killing a single specified monster until all required items are obtained.
    /// </summary>
    /// <param name="QuestID">The ID of the quest to complete.</param>
    /// <param name="MapName">The map in which the quest takes place.</param>
    /// <param name="MonsterName">The name of the monster to hunt.</param>
    /// <param name="GetReward">Whether to receive the quest reward upon completion. Default is true.</param>
    /// <param name="Reward">The reward to pick up; "All" by default.</param>
    /// <param name="AutoCompleteQuest">Whether to automatically complete the quest after farming items. Default is true.</param>
    public void KillQuest(int QuestID, string MapName, string MonsterName, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        if (string.IsNullOrEmpty(MapName))
            throw new ArgumentException("MapName cannot be null or empty", nameof(MapName));

        if (string.IsNullOrEmpty(MonsterName))
            throw new ArgumentException("MonsterName cannot be null or empty", nameof(MonsterName));

        Core.DebugLogger(
            this,
            $"Starting KillQuest: QuestID={QuestID}, Map={MapName}, Monster={MonsterName}"
        );

        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.DebugLogger(this, $"Quest {QuestID} could not be loaded.");
            return;
        }

        // Always use a live quest-state check.
        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        if (QuestData.Requirements.Count == 0)
        {
            Core.DebugLogger(this, $"Quest {QuestID} has no requirements. Nothing to farm.");
            return;
        }

        // Prevent turn-in spam.
        Core.AcceptandCompleteTries = 5;

        // Filter valid requirements and exclude items already obtained.
        List<ItemBase> validRequirements = QuestData.Requirements
            .Where(r => r != null && !string.IsNullOrEmpty(r.Name))
            .Where(r =>
                !(
                    r.Temp
                        ? Bot.TempInv.Contains(r.Name, r.Quantity)
                        : Core.CheckInventory(r.ID, r.Quantity)
                )
            )
            .ToList();

        if (validRequirements.Count == 0)
        {
            Core.DebugLogger(
                this,
                $"All quest requirements for Quest {QuestID} are already satisfied."
            );
            return;
        }

        // Accept the quest and join the map.
        Core.DebugLogger(this, $"Accepting quest {QuestID} and joining map {MapName}");

        Core.EnsureAccept(QuestID);
        Core.Join(MapName);

        // Snapshot requirements.
        CurrentRequirements.Clear();
        CurrentRequirements.AddRange(validRequirements);

        // Add drops for quest items.
        string[] drops = CurrentRequirements
            .Where(r => !r.Temp && !string.IsNullOrEmpty(r.Name))
            .Select(r => r.Name)
            .ToArray();

        if (drops.Length > 0)
        {
            Core.AddDrop(drops);

            Core.DebugLogger(
                this,
                $"Added drops for quest {QuestID}: [{string.Join(", ", drops)}]"
            );
        }

        // Farming loop.
        while (!Bot.ShouldExit)
        {
            // The server may have completed/turned in the quest independently.
            if (QuestProgression(QuestID, false, "All", false))
                return;

            // Remove requirements that are already satisfied.
            CurrentRequirements.RemoveAll(r =>
                r.Temp
                    ? Bot.TempInv.Contains(r.Name, r.Quantity)
                    : Core.CheckInventory(r.ID, r.Quantity)
            );

            if (CurrentRequirements.Count == 0)
                break;

            List<string> itemsToFarm = CurrentRequirements
                .Select(r => r.Name)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            if (itemsToFarm.Count == 0)
                break;

            _MonsterHuntBatch(
                MapName,
                MonsterName,
                itemsToFarm,
                QuestID
            );

            // Catch a server-side turn-in that happened during farming.
            if (QuestProgression(QuestID, false, "All", false))
                return;
        }

        if (Bot.ShouldExit)
            return;

        // Final live completion check before attempting manual completion.
        if (QuestProgression(QuestID, false, "All", false))
            return;

        string[] farmedItems = validRequirements
            .Select(r => r.Name)
            .ToArray();

        Core.DebugLogger(
            this,
            $"Attempting to complete quest {QuestID}"
        );

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(1000);

        CurrentRequirements.Clear();

        // Check once more after the turn-in attempt.
        QuestProgression(QuestID, GetReward, Reward, false);

        Core.DebugLogger(
            this,
            $"Finished KillQuest: QuestID={QuestID}. Items farmed: [{string.Join(", ", farmedItems)}]"
        );
    }

    /// <summary>
    /// Completes a quest by killing one or more specified monsters until all required items are obtained.
    /// </summary>
    /// <param name="QuestID">The ID of the quest to complete.</param>
    /// <param name="MapName">The map in which the quest takes place.</param>
    /// <param name="MonsterNames">Array of monster names corresponding to each quest item.</param>
    /// <param name="GetReward">Whether to receive the quest reward upon completion. Default is true.</param>
    /// <param name="Reward">The reward to pick up; "All" by default.</param>
    /// <param name="AutoCompleteQuest">Whether to automatically complete the quest after farming items. Default is true.</param>
    public void KillQuest(int QuestID, string MapName, string[] MonsterNames, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        if (string.IsNullOrEmpty(MapName))
            throw new ArgumentException("MapName cannot be null or empty", nameof(MapName));

        if (MonsterNames == null || MonsterNames.Length == 0 || MonsterNames.All(string.IsNullOrEmpty))
            throw new ArgumentException("MonsterNames cannot be null or empty", nameof(MonsterNames));

        Core.DebugLogger(this, $"Starting KillQuest: QuestID={QuestID}, Map={MapName}, Monsters=[{string.Join(", ", MonsterNames)}]");

        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));
        if (QuestData == null)
        {
            Core.DebugLogger(this, $"Quest {QuestID} could not be loaded.");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        if (QuestData.Requirements.Count == 0)
        {
            Core.DebugLogger(this, $"Quest {QuestID} has no requirements. Nothing to farm.");
            return;
        }

        Core.AcceptandCompleteTries = 5;

        List<ItemBase> validRequirements = QuestData.Requirements
            .Where(r => r != null && !string.IsNullOrEmpty(r.Name))
            .Where(r => !(r.Temp
                ? Bot.TempInv.Contains(r.Name, r.Quantity)
                : Core.CheckInventory(r.ID, r.Quantity)))
            .ToList();

        if (validRequirements.Count == 0)
        {
            Core.DebugLogger(this, $"All quest requirements for Quest {QuestID} are already satisfied.");
            return;
        }

        Core.DebugLogger(this, $"Accepting quest {QuestID} and joining map {MapName}");
        Core.EnsureAccept(QuestID);
        Core.Join(MapName);

        CurrentRequirements.Clear();
        CurrentRequirements.AddRange(validRequirements);

        string[] drops = CurrentRequirements
            .Where(r => !r.Temp && !string.IsNullOrEmpty(r.Name))
            .Select(r => r.Name)
            .ToArray();

        if (drops.Length > 0)
        {
            Core.AddDrop(drops);
            Core.DebugLogger(this, $"Added drops for quest {QuestID}: [{string.Join(", ", drops)}]");
        }

        // Keep the original requirement -> monster relationship intact.
        for (int i = 0; i < validRequirements.Count && !Bot.ShouldExit; i++)
        {
            ItemBase requirement = validRequirements[i];
            string monster = MonsterNames[Math.Min(i, MonsterNames.Length - 1)];

            if (string.IsNullOrEmpty(monster))
                continue;

            bool alreadyHave = requirement.Temp
                ? Bot.TempInv.Contains(requirement.Name, requirement.Quantity)
                : Core.CheckInventory(requirement.ID, requirement.Quantity);

            if (alreadyHave)
            {
                CurrentRequirements.RemoveAll(r => r.ID == requirement.ID);
                continue;
            }

            if (QuestProgression(QuestID, GetReward, Reward))
                return;

            Core.DebugLogger(
                this,
                $"Farming requirement [{i + 1}/{validRequirements.Count}]: {requirement.Name} x{requirement.Quantity} from {monster}");

            _MonsterHuntBatch(
                MapName,
                monster,
                new[] { requirement.Name },
                QuestID);

            // Do not move to the next monster until this requirement is actually satisfied.
            bool obtained = requirement.Temp
                ? Bot.TempInv.Contains(requirement.Name, requirement.Quantity)
                : Core.CheckInventory(requirement.ID, requirement.Quantity);

            if (obtained)
            {
                CurrentRequirements.RemoveAll(r => r.ID == requirement.ID);

                Core.DebugLogger(
                    this,
                    $"Requirement complete: {requirement.Name} x{requirement.Quantity}. Moving to next monster.");
            }
            else if (QuestProgression(QuestID, GetReward, Reward))
                return;
        }

        if (Bot.ShouldExit)
            return;

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        CurrentRequirements.RemoveAll(r =>
            r.Temp
                ? Bot.TempInv.Contains(r.Name, r.Quantity)
                : Core.CheckInventory(r.ID, r.Quantity));

        if (CurrentRequirements.Count > 0)
        {
            Core.DebugLogger(
                this,
                $"KillQuest {QuestID} still has {CurrentRequirements.Count} requirement(s) remaining.");
            return;
        }

        Core.DebugLogger(this, $"Attempting to complete quest {QuestID}");
        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(1000);
        CurrentRequirements.Clear();

        Core.DebugLogger(
            this,
            $"Finished KillQuest: QuestID={QuestID}. Items farmed: [{string.Join(", ", validRequirements.Select(r => r.Name))}]");
    }
    /// <summary>
    /// Internal method to hunt a monster for specific quest items.
    /// </summary>
    /// <param name="map">The map where the monster is located.</param>
    /// <param name="monster">The name of the monster to hunt.</param>
    /// <param name="itemNames">List of item names required from the monster.</param>
    /// <param name="Qid">The ID of the quest these items belong to.</param>
    private void _MonsterHuntBatch(string map, string monster, IReadOnlyCollection<string> itemNames, int Qid)
    {
        if (string.IsNullOrEmpty(map))
        {
            Bot.Log("Map is null or empty");
            return;
        }

        if (string.IsNullOrEmpty(monster))
        {
            Bot.Log("Monster name is null or empty");
            return;
        }

        if (itemNames == null || itemNames.Count == 0)
        {
            Bot.Log("itemNames list is empty");
            return;
        }

        // Ensure we are on the correct map.
        if (!string.Equals(Bot.Map?.Name, map, StringComparison.OrdinalIgnoreCase))
        {
            Core.Join(map);
            Bot.Wait.ForMapLoad(map);
        }

        List<ItemBase> neededItems = (CurrentRequirements ?? new List<ItemBase>())
            .Where(r =>
                r != null
                && itemNames.Contains(r.Name)
                && !(
                    r.Temp
                        ? Bot.TempInv.Contains(r.Name, r.Quantity)
                        : Core.CheckInventory(r.ID, r.Quantity)
                )
            )
            .ToList();

        if (neededItems.Count == 0)
        {
            Core.Logger(
                $"All requested items for \"{monster}\" are already satisfied: [{string.Join(", ", itemNames)}]",
                "_MonsterHuntBatch"
            );
            return;
        }

        Core.Logger(
            $"Farming monster \"{monster}\" for items: [{string.Join(", ", neededItems.Select(r => $"{r.Name} x{r.Quantity}"))}]",
            "_MonsterHuntBatch"
        );

        while (!Bot.ShouldExit)
        {
            // Most important check: always ask for the current quest state.
            if (QuestProgression(Qid, false, "All", false))
                break;

            neededItems = (CurrentRequirements ?? new List<ItemBase>())
                .Where(r =>
                    r != null
                    && itemNames.Contains(r.Name)
                    && !(
                        r.Temp
                            ? Bot.TempInv.Contains(r.Name, r.Quantity)
                            : Core.CheckInventory(r.ID, r.Quantity)
                    )
                )
                .ToList();

            if (neededItems.Count == 0)
                break;

            var targetCellGroup = Bot.Monsters.MapMonsters?
                .Where(m =>
                    m != null
                    && m.Name.FormatForCompare() == monster.FormatForCompare()
                )
                .GroupBy(m => m.Cell)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            string targetCell = targetCellGroup?.Key ?? "Enter";

            if (!string.Equals(
                    Bot.Player?.Cell,
                    targetCell,
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                Core.DebugLogger(
                    this,
                    $"Jumping to cell '{targetCell}' with {targetCellGroup?.Count() ?? 0} {monster}s",
                    "_MonsterHuntBatch"
                );

                IScriptMap? mapApi = Bot.Map;

                if (!string.Equals(
                        mapApi?.Name,
                        map,
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    Core.Join(map);
                    Bot.Wait.ForMapLoad(map);
                    mapApi = Bot.Map;
                }

                if (!string.Equals(
                        Bot.Player?.Cell,
                        targetCell,
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    mapApi?.Jump(
                        targetCell,
                        "Left",
                        autoCorrect: false
                    );

                    Bot.Wait.ForCellChange(targetCell);
                    Bot.Player?.SetSpawnPoint();
                }
            }

            if (!(Bot.Player?.Alive ?? false))
            {
                Bot.Wait.ForTrue(
                    () => Bot.Player?.Alive ?? false,
                    20
                );

                continue;
            }

            foreach (
                Monster? M in Bot.Monsters.CurrentAvailableMonsters?.Where(m =>
                    m != null
                    && m.Name.FormatForCompare() == monster.FormatForCompare()
                ) ?? Enumerable.Empty<Monster>()
            )
            {
                if (M == null || M.HP <= 0)
                    continue;

                while (!Bot.ShouldExit)
                {
                    // Check for server-side quest completion before attacking.
                    if (QuestProgression(Qid, false, "All", false))
                        break;

                    neededItems = (CurrentRequirements ?? new List<ItemBase>())
                        .Where(r =>
                            r != null
                            && itemNames.Contains(r.Name)
                            && !(
                                r.Temp
                                    ? Bot.TempInv.Contains(r.Name, r.Quantity)
                                    : Core.CheckInventory(r.ID, r.Quantity)
                            )
                        )
                        .ToList();

                    if (neededItems.Count == 0)
                        break;

                    IScriptMap? innerMap = Bot.Map;

                    if (!string.Equals(
                            innerMap?.Name,
                            map,
                            StringComparison.OrdinalIgnoreCase
                        ))
                    {
                        Core.Join(map);
                        Bot.Wait.ForMapLoad(map);
                        innerMap = Bot.Map;
                    }

                    if (!string.Equals(
                            Bot.Player?.Cell,
                            targetCell,
                            StringComparison.OrdinalIgnoreCase
                        ))
                    {
                        innerMap?.Jump(
                            targetCell,
                            "Left",
                            autoCorrect: false
                        );

                        Bot.Wait.ForCellChange(targetCell);
                        Bot.Player?.SetSpawnPoint();
                    }

                    if (
                        !Bot.Player!.HasTarget
                        || Bot.Player?.Target?.MapID != M.MapID
                    )
                        Bot.Combat.Attack(M.MapID);

                    Bot.Sleep(500);

                    // The server may have completed the quest during the kill.
                    if (QuestProgression(Qid, false, "All", false))
                        break;

                    if (
                        !Bot.Player!.HasTarget
                        || Bot.Player?.Target?.HP <= 0
                    )
                        break;

                    string[] names = neededItems
                        .Where(r => r != null)
                        .Select(r => r.Name)
                        .ToArray();

                    if (
                        names.Length > 0
                        && Bot.Drops.CurrentDrops.Any(
                            d => d != null && names.Contains(d)
                        )
                    )
                        Bot.Drops.Pickup(names);

                    neededItems = (CurrentRequirements ?? Enumerable.Empty<ItemBase>())
                        .Where(r =>
                            r != null
                            && itemNames.Contains(r.Name)
                            && !(
                                r.Temp
                                    ? Bot.TempInv.Contains(r.Name, r.Quantity)
                                    : Core.CheckInventory(r.ID, r.Quantity)
                            )
                        )
                        .ToList();
                }

                // Stop checking additional monsters if the quest completed.
                if (QuestProgression(Qid, false, "All", false))
                    return;
            }

            if (CurrentRequirements is { Count: > 0 } reqs)
            {
                reqs.RemoveAll(r =>
                    r != null
                    && itemNames.Contains(r.Name)
                    && (
                        r.Temp
                            ? Bot.TempInv.Contains(r.Name, r.Quantity)
                            : Core.CheckInventory(r.ID, r.Quantity)
                    )
                );
            }
        }
    }

    #endregion

    #region MapItemQuest

    /// <summary>
    /// Gets a MapItem X times for a Quest, and turns in the quest if possible.
    /// Automatically checks if the quest was completed server-side.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the items are</param>
    /// <param name="MapItemID">ID of the item</param>
    /// <param name="Amount">The amount of MapItemID to grab</param>
    /// <param name="GetReward">Whether or not the Reward should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest when possible</param>
    public void MapItemQuest(int QuestID, string MapName, int MapItemID, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        if (string.IsNullOrEmpty(MapName) || Amount <= 0)
            return;

        Core.EnsureAccept(QuestID);

        if (QuestProgression(QuestID, false, "All", false))
            return;

        if (!string.Equals(Bot.Map?.Name, MapName, StringComparison.OrdinalIgnoreCase))
        {
            Core.Join(MapName);
            Bot.Wait.ForMapLoad(MapName);
        }

        Core.GetMapItem(MapItemID, Amount, MapName);

        // MapItem collection can trigger server-side quest completion.
        if (QuestProgression(QuestID, false, "All", false))
            return;

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(500);

        QuestProgression(QuestID, GetReward, Reward, false);
    }

    /// <summary>
    /// Completes a quest by collecting map items.
    /// </summary>
    /// <param name="QuestID">The quest ID to complete</param>
    /// <param name="MapName">The map to grab items from</param>
    /// <param name="MapItemIDs">IDs of the map items required</param>
    /// <param name="Amount">Quantity of each item to collect</param>
    /// <param name="GetReward">Whether to collect the reward if completed</param>
    /// <param name="Reward">Which reward to pick ("All" by default)</param>
    /// <param name="AutoCompleteQuest">Whether to auto-complete the quest after collecting items</param>
    public void MapItemQuest(int QuestID, string MapName, int[] MapItemIDs, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        if (string.IsNullOrEmpty(MapName) || MapItemIDs == null || MapItemIDs.Length == 0 || Amount <= 0)
            return;

        Core.EnsureAccept(QuestID);

        if (QuestProgression(QuestID, false, "All", false))
            return;

        List<(int ItemID, int Quantity)> itemsToGrab = MapItemIDs
            .Where(id => !Bot.TempInv.Contains(id, Amount))
            .Select(id => (ItemID: id, Quantity: Amount))
            .ToList();

        if (itemsToGrab.Count > 0)
        {
            Core.Logger($"Grabbing items from map {MapName}: {string.Join(", ", itemsToGrab.Select(i => $"{i.ItemID} x{i.Quantity}"))}");
            Core.GetMapItems(itemsToGrab, MapName);

            if (QuestProgression(QuestID, false, "All", false))
                return;
        }

        if (QuestProgression(QuestID, false, "All", false))
            return;

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(500);

        QuestProgression(QuestID, GetReward, Reward, false);
    }

    /// <summary>
    /// Completes a quest by collecting multiple map items, batching them per map.
    /// </summary>
    /// <param name="QuestID">The quest ID to complete</param>
    /// <param name="MapItems">Array of tuples containing MapItemID, Amount, and MapName</param>
    /// <param name="GetReward">Whether to collect the reward if completed</param>
    /// <param name="Reward">Which reward to pick ("All" by default)</param>
    /// <param name="AutoCompleteQuest">Whether to auto-complete the quest after collecting items</param>
    public void MapItemQuest(int QuestID, (int MapItemID, int Amount, string MapName)[] MapItems, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        if (MapItems == null || MapItems.Length == 0)
            return;

        Core.EnsureAccept(QuestID);

        if (QuestProgression(QuestID, false, "All", false))
            return;

        IEnumerable<IGrouping<string, (int MapItemID, int Amount, string MapName)>> itemsGroupedByMap = MapItems
            .Where(mi =>
                !string.IsNullOrEmpty(mi.MapName)
                && mi.Amount > 0
                && !Bot.TempInv.Contains(mi.MapItemID, mi.Amount)
            )
            .GroupBy(mi => mi.MapName);

        foreach (IGrouping<string, (int MapItemID, int Amount, string MapName)> group in itemsGroupedByMap)
        {
            if (QuestProgression(QuestID, false, "All", false))
                return;

            string map = group.Key;

            (int ItemID, int Quantity)[] itemsToGrab = group
                .Select(mi => (ItemID: mi.MapItemID, Quantity: mi.Amount))
                .ToArray();

            if (itemsToGrab.Length == 0)
                continue;

            Core.Logger($"Grabbing items from map {map}: {string.Join(", ", itemsToGrab.Select(i => $"{i.ItemID} x{i.Quantity}"))}");

            if (!string.Equals(Bot.Map?.Name, map, StringComparison.OrdinalIgnoreCase))
            {
                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
            }

            Core.GetMapItems(itemsToGrab, map);

            if (QuestProgression(QuestID, false, "All", false))
                return;
        }

        if (QuestProgression(QuestID, false, "All", false))
            return;

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(500);

        QuestProgression(QuestID, GetReward, Reward, false);
    }

    #endregion

    #region MiscQuest

    /// <summary>
    /// Buys an item X times for a Quest, and turns in the quest if possible.
    /// Automatically checks if the quest was completed server-side.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the shop is located</param>
    /// <param name="ShopID">ID of the shop</param>
    /// <param name="ItemName">Name of the item to buy</param>
    /// <param name="Amount">The amount of ItemName to buy</param>
    /// <param name="GetReward">Whether or not the Reward should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest when the quest can be completed</param>
    public void BuyQuest(int QuestID, string MapName, int ShopID, string ItemName, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.EnsureAccept(QuestID);

        // The server may have completed the quest immediately after acceptance.
        if (QuestProgression(QuestID, false, "All", false))
            return;

        Core.BuyItem(MapName, ShopID, ItemName, Amount);

        // Buying the required item may complete the quest server-side.
        if (QuestProgression(QuestID, false, "All", false))
            return;

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(500);

        // Confirm the completion after the turn-in attempt.
        QuestProgression(QuestID, GetReward, Reward, false);
    }

    /// <summary>
    /// Accepts a quest and then turns it in again.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="GetReward">Whether or not the Reward should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest when the quest can be completed</param>
    public void ChainQuest(int QuestID, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest? QuestData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (QuestData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return;
        }

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.EnsureAccept(QuestID);

        // Catch quests that complete immediately after acceptance.
        if (QuestProgression(QuestID, false, "All", false))
            return;

        TryComplete(QuestData, AutoCompleteQuest);

        Bot.Sleep(500);

        // Confirm the server-side completion.
        QuestProgression(QuestID, GetReward, Reward, false);
    }

    #endregion
    public void QuestComplete(int questID) =>
        TryComplete(Core.InitializeWithRetries(() => Core.EnsureLoad(questID), 20), true);

    private void TryComplete(Quest? QuestData, bool autoCompleteQuest)
    {
        if (QuestData == null)
        {
            Core.Logger("QuestData is null, cannot complete quest");
            return;
        }

        Quest? questData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestData.ID));

        if (questData == null)
        {
            Core.Logger($"Quest with ID {QuestData.ID} not found");
            return;
        }

        string[] missingItems = questData
            .Requirements
            .Concat(questData.AcceptRequirements)
            .Where(x =>
                x != null
                && (
                    x.Temp
                        ? !Bot.TempInv.Contains(x.ID, x.Quantity)
                        : !Core.CheckInventory(x.ID, x.Quantity)
                )
            )
            .Select(x =>
            {
                int have = x.Temp
                    ? Bot.TempInv.GetQuantity(x.ID)
                    : Bot.Inventory.GetQuantity(x.Name);

                return $"{x.Name}[{x.ID}] x{x.Quantity} (have {have})";
            })
            .ToArray();

        if (missingItems.Length > 0)
        {
            Core.Logger(
                $"Missing items for quest [{questData.ID}] \"{questData.Name}\": {string.Join(", ", missingItems)}",
                "QuestProgression"
            );
            return;
        }

        Core.Sleep();

        // Always force proper completion.
        Core.EnsureAccept(questData.ID);
        Core.EnsureComplete(questData.ID);

        Bot.Wait.ForQuestComplete(questData.ID);

        if (questData.Rewards != null)
        {
            foreach (string reward in questData.Rewards.Select(r => r.Name).ToArray())
                Bot.Wait.ForPickup(reward);
        }

        Core.Logger(
            $"Completed Quest: [{questData.ID}] - \"{questData.Name}\"",
            "QuestProgression"
        );

        Core.Sleep();
    }

    /// <summary>
    /// Skeleton of KillQuest, MapItemQuest, BuyQuest and ChainQuest.
    /// Only needs to be used inside a script if the quest spans across multiple maps.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="GetReward">Whether or not the Reward should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="Log"></param>
    public bool QuestProgression(int QuestID, bool GetReward = true, string Reward = "All", bool Log = true)
    {
        if (QuestID <= 0)
            return false;

        // Load CBO test flag once.
        if (!CBO_Checked)
        {
            if (Core.CBOBool("BCO_Story_TestBot", out bool testBot))
                TestBot = testBot;

            CBO_Checked = true;
        }

        Quest? questData = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID));

        if (questData == null)
        {
            Core.Logger($"Quest with ID {QuestID} not found");
            return true;
        }

        int attempts = 0;

        // Quest unlock recovery loop.
        while (!Bot.Quests.IsUnlocked(QuestID))
        {
            Core.Sleep(1000);

            int currentValue = questData.Slot > 0
                ? Bot.Flash.CallGameFunction<int>("world.getQuestValue", questData.Slot)
                : 0;

            if (attempts == 0 || attempts % 3 == 0)
            {
                Core.Logger(
                    $"Progress check: Slot {questData.Slot} | Current {currentValue} / Target {questData.Value - 1}",
                    "QuestProgression"
                );
            }

            // Find previous quest in the same chain safely.
            Quest? prevQuest = Bot.Quests.Tree?
                .Where(q => q.Slot == questData.Slot && q.Value < questData.Value)
                .OrderByDescending(q => q.Value)
                .FirstOrDefault();

            if (prevQuest != null)
            {
                // Safely gather requirements.
                string[] prevReqs = (prevQuest.Requirements ?? Enumerable.Empty<ItemBase>())
                    .Concat(prevQuest.AcceptRequirements ?? Enumerable.Empty<ItemBase>())
                    .Where(req => req != null && !string.IsNullOrEmpty(req.Name))
                    .Select(req => req.Name)
                    .ToArray();

                // If we already have the items, re-complete the previous quest
                // to repair the chain.
                if (prevReqs.Length > 0 && Core.CheckInventory(prevReqs))
                {
                    Core.Logger(
                        $"Attempting recovery via re-completing previous quest: [{prevQuest.ID}] \"{prevQuest.Name}\"",
                        "QuestProgression"
                    );

                    TryComplete(prevQuest, true);

                    attempts = 0;
                    continue;
                }

                // Log missing requirements.
                string[] missingReqs = prevReqs
                    .Where(req => !Core.CheckInventory(req))
                    .ToArray();

                if (missingReqs.Length > 0)
                {
                    Bot.Log(
                        $"Missing [{string.Join(", ", missingReqs)}] to accept {questData.Name} [{questData.ID}]"
                    );

                    attempts = 5;
                }
            }

            attempts++;

            // Relog after repeated failures.
            if (attempts >= 5)
            {
                Core.Logger(
                    $"Quest [{QuestID}] \"{questData.Name}\" still not unlocked after retries. Relogging...",
                    "QuestProgression"
                );

                Core.Relogin("Quest progression recovery failed, relogging.");
                attempts = 0;
            }
        }

        // IMPORTANT:
        // This is intentionally a fresh completion check every time.
        // Do NOT cache PreviousQuestID or PreviousQuestState here.
        if (Core.isCompletedBefore(QuestID) && (!TestBot || questData.Once))
        {
            if (Log)
            {
                if (TestBot)
                    Core.Logger(
                        $"Skipped (Once = true): [{QuestID}] - \"{questData.Name}\""
                    );
                else
                    Core.Logger(
                        $"Already Completed: [{QuestID}] - \"{questData.Name}\""
                    );
            }

            return true;
        }

        // Reward handling.
        if (GetReward)
        {
            if (Reward != "All")
            {
                if (Core.CheckInventory(Reward))
                {
                    Core.Logger($"Already have reward \"{Reward}\", skipping quest.");
                    return true;
                }

                Core.AddDrop(Reward);
            }
            else
            {
                Core.AddDrop(Core.QuestRewards(QuestID));
            }
        }

        if (Log)
            Core.Logger($"Doing Quest: [{QuestID}] - \"{questData.Name}\"");

        return false;
    }
    private bool CBO_Checked = false;

    public void LegacyQuestManager(Action questLogic, params int[] questIDs)
    {
        List<Quest>? questData = Core.InitializeWithRetries(() => Core.EnsureLoad(questIDs));
        List<LegacyQuestObject> whereToGet = [];

        if (questData == null || questData.Count == 0)
        {
            Core.Logger("No quests found, cannot run LegacyQuestManager", messageBox: true);
            return;
        }

        _LegacyQuestStop = false;

        Core.DebugLogger(this, "-------------\t");

        foreach (Quest quest in questData)
        {
            List<ItemBase> desiredQuestReward = quest.Rewards
                .Where(r =>
                    questData.Any(q =>
                        q.AcceptRequirements.Any(a => a.ID == r.ID || a.Name == r.Name)
                    )
                )
                .ToList();

            int requiredQuestID = questData
                .Find(q =>
                    q.Rewards.Any(r =>
                        quest.AcceptRequirements != null
                        && quest.AcceptRequirements.Any(a => a.ID == r.ID || a.Name == r.Name)
                    )
                )
                ?.ID ?? 0;

            List<ItemBase>? requiredQuestReward = quest.AcceptRequirements?
                .Where(r =>
                    questData.Any(q =>
                        q.Rewards.Any(a => a.ID == r.ID || a.Name == r.Name)
                    )
                )
                .ToList();

            Core.DebugLogger(this, $"{quest.ID}\t\t");
            Core.DebugLogger(this, $"{desiredQuestReward.FirstOrDefault()?.Name}\t");
            Core.DebugLogger(this, $"{requiredQuestID}\t\t");
            Core.DebugLogger(this, $"{requiredQuestReward?.FirstOrDefault()?.Name}\t");
            Core.DebugLogger(this, "-------------\t");

            if (requiredQuestReward?.Count == 0 && quest.AcceptRequirements?.Count > 0)
            {
                Core.Logger(
                    $"The manager failed to find the location of \"{string.Join("\" \"", quest.AcceptRequirements.Select(a => a.Name))}\" for Quest ID {quest.ID}, is the function missing a Quest ID?",
                    messageBox: true
                );
                return;
            }

            whereToGet.Add(new(quest.ID, desiredQuestReward, requiredQuestID, requiredQuestReward));
        }

        if (
            whereToGet.All(x => x.desiredQuestReward.Count == 0)
            || whereToGet.All(x => x.requiredQuestReward?.Count == 0)
        )
        {
            string ids = string.Join(", ", questData.Select(q => q.ID));

            Core.Logger(
                $"None of the Quest IDs filled in ({ids}) are supposed to be used in the LegacyQuestManager, " +
                "please report to the bot makers that they must make this story line in the normal way."
            );
            return;
        }

        LegacyQuestObject? finalItemQuest = whereToGet.Find(x => x.desiredQuestReward.Count == 0);

        if (finalItemQuest == null || finalItemQuest.desiredQuestID <= 0)
        {
            Core.Logger("Could not find the Quest ID of the last quest in the item chain");
            return;
        }

        Quest? finalQuestData = Core.InitializeWithRetries(
            () => Core.EnsureLoad(finalItemQuest.desiredQuestID)
        );

        if (finalQuestData == null)
        {
            Core.Logger($"Quest with ID {finalItemQuest.desiredQuestID} not found");
            return;
        }

        Core.Logger(
            $"Final quest in Legacy Quest Chain: [{finalItemQuest.desiredQuestID}] \"{finalQuestData.Name}\""
        );

        runQuest(finalItemQuest.desiredQuestID);

        foreach (LegacyQuestObject l in whereToGet)
        {
            if (l.requiredQuestReward != null)
                Core.ToBank(l.requiredQuestReward.Select(i => i.ID).ToArray());
        }

        void runQuest(int questID)
        {
            if (_LegacyQuestStop)
                return;

            LegacyQuestObject? runQuestData = whereToGet.Find(d => d.desiredQuestID == questID);

            if (runQuestData == null)
            {
                Core.Logger($"Could not find LegacyQuestObject for quest {questID}");
                return;
            }

            Quest? questData = Core.InitializeWithRetries(() => Core.EnsureLoad(questID));

            if (questData == null)
            {
                Core.Logger($"Quest with ID {questID} not found");
                return;
            }

            int[] requiredReward = runQuestData.requiredQuestReward?
                .Select(i => i.ID)
                .ToArray() ?? Array.Empty<int>();

            // Final quests don't have a desired intermediate reward.
            if (
                runQuestData.desiredQuestReward.Count == 0
                && questID != finalItemQuest.desiredQuestID
            )
            {
                if (requiredReward.Length > 0 && !Core.CheckInventory(requiredReward))
                    runQuest(runQuestData.requiredQuestID);

                return;
            }

            int[] desiredReward = runQuestData.desiredQuestReward
                .Select(i => i.ID)
                .ToArray();

            int[] finalRewards = questID == finalItemQuest.desiredQuestID
                ? questData.Rewards.Select(x => x.ID).ToArray()
                : desiredReward;

            // Always check the live inventory before doing the quest.
            if (finalRewards.Length > 0 && Core.CheckInventory(finalRewards))
            {
                Core.Logger(
                    $"Already Completed: [{questID}] - \"{questData.Name}\"",
                    "QuestProgression"
                );
                return;
            }

            // Make sure the previous quest in the chain is finished first.
            if (
                requiredReward.Length > 0
                && !Core.CheckInventory(requiredReward)
            )
            {
                runQuest(runQuestData.requiredQuestID);

                if (_LegacyQuestStop)
                    return;

                // The previous quest may have completed server-side while
                // the recursive call was running. Re-check before continuing.
                if (!Core.CheckInventory(requiredReward))
                    return;
            }

            if (_LegacyQuestStop)
                return;

            // The quest may have been completed by the server while recovering
            // the previous quest. Do not run questLogic unnecessarily.
            if (QuestProgression(questID, false, "All", false))
                return;

            Core.Logger(
                $"Doing Quest: [{questID}] - \"{questData.Name}\"",
                "QuestProgression"
            );

            Core.EnsureAccept(questID);

            // Acceptance itself can trigger completion for some quests.
            if (QuestProgression(questID, false, "All", false))
                return;

            if (desiredReward.Length > 0)
                Core.AddDrop(desiredReward);

            LegacyQuestID = questID;

            // Run the actual farming/quest logic.
            questLogic();

            if (_LegacyQuestStop)
                return;

            // IMPORTANT:
            // questLogic() may have caused the server to complete and/or
            // turn in the quest. Check the live quest state BEFORE TryComplete.
            if (QuestProgression(questID, false, "All", false))
            {
                LegacyQuestAutoComplete = true;
                return;
            }

            TryComplete(questData, LegacyQuestAutoComplete);

            // Give the server a moment to process the completion.
            Bot.Sleep(500);

            // Confirm whether the quest actually completed.
            if (!QuestProgression(questID, false, "All", false))
            {
                Core.Logger(
                    $"Quest [{questID}] did not report as completed after TryComplete.",
                    "QuestProgression"
                );
            }

            foreach (int i in desiredReward)
                Bot.Wait.ForPickup(i);

            if (questID == finalItemQuest.desiredQuestID)
            {
                Bot.Drops.Pickup(
                    finalQuestData.Rewards.Select(x => x.ID).ToArray()
                );
            }

            LegacyQuestAutoComplete = true;
        }
    }

    private class LegacyQuestObject
    {
        public int desiredQuestID { get; set; }
        public List<ItemBase> desiredQuestReward { get; set; }
        public int requiredQuestID { get; set; }
        public List<ItemBase>? requiredQuestReward { get; set; }

        public LegacyQuestObject(
            int desiredQuestID,
            List<ItemBase> desiredQuestReward,
            int requiredQuestID,
            List<ItemBase>? requiredQuestReward
        )
        {
            this.desiredQuestID = desiredQuestID;
            this.desiredQuestReward = desiredQuestReward;
            this.requiredQuestID = requiredQuestID;
            this.requiredQuestReward = requiredQuestReward;
        }
    }

    public int LegacyQuestID = -1;
    public bool LegacyQuestAutoComplete = true;
    private bool _LegacyQuestStop = false;

    public void LegacyQuestStop() => _LegacyQuestStop = true;

    /// <summary>
    /// Put this at the start of your story script so that the bot will load all quests that are used in the bot.
    /// This will speed up any progression checks tremendously.
    /// </summary>
    public void PreLoad(object _this, [CallerMemberName] string caller = "")
    {
        List<int> QuestIDs = [];
        string[] ScriptSlice = Core.CompiledScript();

        if (ScriptSlice.Length == 0)
        {
            Core.Logger(
                "PreLoad failed, cannot read Compiled Script. You might not be on the latest version of Skua"
            );
            return;
        }

        int classStartIndex = Array.IndexOf(ScriptSlice, $"public class {_this}");

        if (classStartIndex < 0)
            return;

        int classEndIndex = Array.IndexOf(
            ScriptSlice[classStartIndex..],
            "}"
        ) + classStartIndex + 1;

        if (classEndIndex <= classStartIndex)
            return;

        ScriptSlice = ScriptSlice[classStartIndex..classEndIndex];

        int methodStartIndex = -1;

        foreach (string access in new[] { "public", "private", "protected" })
        {
            foreach (string type in new[] { "void", "bool", "string", "int" })
            {
                methodStartIndex = Array.FindIndex(
                    ScriptSlice,
                    line => line.Contains($"{access} {type} {caller}")
                );

                if (methodStartIndex > -1)
                    break;
            }

            if (methodStartIndex > -1)
                break;
        }

        if (methodStartIndex == -1)
        {
            Core.Logger("Failed to parse methodStartIndex, no quests will be pre-loaded");
            return;
        }

        int methodIndentCount = -1;

        if (methodStartIndex + 1 < ScriptSlice.Length)
            methodIndentCount = ScriptSlice[methodStartIndex + 1].IndexOf('{');

        if (methodIndentCount < 0)
            methodIndentCount = ScriptSlice[methodStartIndex].IndexOf('{');

        if (methodIndentCount < 0)
            methodIndentCount = 0;

        string indent = new(' ', methodIndentCount);

        int methodEndIndex = Array.FindIndex(
            ScriptSlice,
            methodStartIndex + 1,
            line => line == indent + "}"
        );

        if (methodEndIndex < 0)
        {
            Core.Logger("Failed to parse methodEndIndex, no quests will be pre-loaded");
            return;
        }

        ScriptSlice = ScriptSlice[methodStartIndex..(methodEndIndex + 1)];

        string[] SearchParam =
        {
        "Story.KillQuest",
        "Story.MapItemQuest",
        "Story.BuyQuest",
        "Story.ChainQuest",
        "Story.QuestProgression",
        "Core.EnsureAccept",
        "Core.EnsureComplete",
        "Core.EnsureCompleteChoose",
        "Core.ChainComplete"
    };

        foreach (string line in ScriptSlice)
        {
            string editedLine = line.Replace(" ", "")
                .Replace("!", "")
                .Replace("(", "")
                .Replace("if", "")
                .Replace("else", "");

            if (!SearchParam.Any(x => editedLine.StartsWith(x)))
                continue;

            int questStart = editedLine.IndexOf('(');

            if (questStart < 0)
                continue;

            string questArguments = editedLine[(questStart + 1)..];

            string questIDString = new(
                questArguments
                    .SkipWhile(c => !char.IsDigit(c))
                    .TakeWhile(char.IsDigit)
                    .ToArray()
            );

            if (!int.TryParse(questIDString, out int QuestID))
                continue;

            if (
                QuestID > 0
                && !QuestIDs.Contains(QuestID)
                && !Bot.Quests.Tree.Exists(x => x.ID == QuestID)
            )
            {
                QuestIDs.Add(QuestID);
            }
        }

        int availableSlots = Core.LoadedQuestLimit - Bot.Quests.Tree.Count;

        if (QuestIDs.Count > availableSlots)
        {
            if (QuestIDs.Count < Core.LoadedQuestLimit)
            {
                Bot.Flash.SetGameObject("world.questTree", new ExpandoObject());
            }
            else
            {
                Core.Logger(
                    $"Found {QuestIDs.Count} Quests, this exceeds the max amount of loaded quests ({Core.LoadedQuestLimit}). No quests will be loaded."
                );
                return;
            }
        }

        if (QuestIDs.Count == 0)
        {
            Core.Logger("No new quests found to pre-load.");
            return;
        }

        Core.Logger($"Loading {QuestIDs.Count} Quests.");

        if (QuestIDs.Count > 30)
        {
            double estimatedTime = QuestIDs.Count / 30.0 * 1.6;
            Core.Logger($"Estimated Loading Time: {Math.Ceiling(estimatedTime)}s");
        }

        for (int i = 0; i < QuestIDs.Count; i += 30)
        {
            int count = Math.Min(30, QuestIDs.Count - i);

            Bot.Quests.Load(QuestIDs.GetRange(i, count).ToArray());
            Core.Sleep(1500);
        }
    }

    private void _SmartKill(string map, string monster, int iterations = 20)
    {
        if (monster == null)
        {
            Core.Logger("ERROR: monster is null, please report", stopBot: true);
            return;
        }

        bool repeat = true;
        for (int j = 0; j < iterations; j++)
        {
            if (CurrentRequirements.Count == 0)
            {
                break;
            }
            if (CurrentRequirements.Count == 1)
            {
                if (_RepeatCheck(ref repeat, 0))
                {
                    break;
                }
                _MonsterHunt(
                    map,
                    ref repeat,
                    monster,
                    CurrentRequirements[0].Name,
                    CurrentRequirements[0].Quantity,
                    CurrentRequirements[0].Temp,
                    0
                );
                break;
            }
            else
            {
                for (int i = CurrentRequirements.Count - 1; i >= 0; i--)
                {
                    if (
                        j == 0
                        && Core.CheckInventory(
                            CurrentRequirements[i].ID,
                            CurrentRequirements[i].Quantity
                        )
                    )
                    {
                        CurrentRequirements.RemoveAt(i);
                        continue;
                    }
                    if (
                        j != 0
                        && Core.CheckInventory(
                            CurrentRequirements[i].ID,
                            CurrentRequirements[i].Quantity
                        )
                    )
                    {
                        if (_RepeatCheck(ref repeat, i))
                        {
                            break;
                        }
                        _MonsterHunt(
                            map,
                            ref repeat,
                            monster,
                            CurrentRequirements[i].Name,
                            CurrentRequirements[i].Quantity,
                            CurrentRequirements[i].Temp,
                            i
                        );
                        break;
                    }
                }
            }
            if (!repeat)
            {
                break;
            }
            // Find the target monster
            Monster? targetMonster = Core.InitializeWithRetries(() =>
                Bot.Monsters.MapMonsters.Find(x =>
                    x.Name.FormatForCompare() == monster.FormatForCompare()
                )
            );
            if (targetMonster == null)
            {
                Core.Logger(
                    $"Monster \"{monster}\" not found on the map \"{Bot.Map.Name}\" after {j} iterations",
                    stopBot: true
                );
                return;
            }
            if (Bot.Map.Name != map)
            {
                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
            }

            Bot.Hunt.Monster(monster);
            Bot.Drops.Pickup(
                CurrentRequirements.Where(item => !item.Temp).Select(item => item.Name).ToArray()
            );
            Core.Sleep();
        }
    }

    private readonly List<ItemBase> CurrentRequirements = [];

    private void _MonsterHunt(string map, ref bool shouldRepeat, string monster, string itemName, int quantity, bool isTemp, int index)
    {
        if (index < 0 || index >= CurrentRequirements.Count)
        {
            shouldRepeat = false;
            return;
        }

        // Check if the item is already in inventory.
        if (
            string.IsNullOrEmpty(itemName)
            || (
                isTemp
                    ? Bot.TempInv.Contains(itemName, quantity)
                    : Core.CheckInventory(itemName, quantity)
            )
        )
        {
            CurrentRequirements.RemoveAt(index);
            shouldRepeat = false;
            return;
        }

        // Find the target monster.
        Monster? targetMonster = Core.InitializeWithRetries(() =>
            Bot.Monsters.MapMonsters.Find(x =>
                x != null && x.Name.FormatForCompare() == monster.FormatForCompare()
            )
        );

        if (targetMonster == null)
        {
            Core.Logger(
                $"Monster \"{monster}\" not found on the map \"{Bot.Map.Name}\" for \"{itemName}\". " +
                "It's probably been renamed, please report this missing monster to @Tato2 or @bogalj on Discord.",
                "Missing Monster",
                stopBot: true
            );

            shouldRepeat = false;
            return;
        }

        Core.Logger(
            $"Hunting \"{monster}\" for \"{itemName}\" x{quantity}",
            "_MonsterHunt"
        );

        // Main loop for hunting the monster until the item or quest is completed.
        while (!Bot.ShouldExit)
        {
            // The server can complete/turn in the quest while we are killing.
            if (lastQuestID > 0 && QuestProgression(lastQuestID, false, "All", false))
            {
                shouldRepeat = false;
                return;
            }

            bool hasItem = isTemp
                ? Bot.TempInv.Contains(itemName, quantity)
                : Core.CheckInventory(itemName, quantity);

            if (hasItem)
                break;

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
            {
                Core.Join(map);
                Bot.Wait.ForMapLoad(map);

                // Refresh the monster after changing maps.
                targetMonster = Core.InitializeWithRetries(() =>
                    Bot.Monsters.MapMonsters.Find(x =>
                        x != null && x.Name.FormatForCompare() == monster.FormatForCompare()
                    )
                );

                if (targetMonster == null)
                {
                    Core.Logger(
                        $"Monster \"{monster}\" was not found after joining \"{map}\".",
                        "Missing Monster",
                        stopBot: true
                    );

                    shouldRepeat = false;
                    return;
                }
            }

            string cellToJump = targetMonster.Cell ?? "Enter";

            if (!string.Equals(Bot.Player.Cell, cellToJump, StringComparison.OrdinalIgnoreCase))
            {
                Core.Jump(cellToJump, "Left");
                Bot.Wait.ForCellChange(cellToJump);
            }

            if (!Bot.Player.HasTarget || Bot.Player.Target?.MapID != targetMonster.MapID)
                Bot.Combat.Attack(targetMonster.Name);

            Core.Sleep();

            // Check the quest again immediately after combat activity.
            if (lastQuestID > 0 && QuestProgression(lastQuestID, false, "All", false))
            {
                shouldRepeat = false;
                return;
            }

            if (Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
                continue;
        }

        if (Bot.ShouldExit)
        {
            shouldRepeat = false;
            return;
        }

        // Handle item pickup if not temporary.
        if (!isTemp)
            Bot.Wait.ForPickup(itemName);

        // Final quest check in case the pickup itself completed the quest.
        if (lastQuestID > 0 && QuestProgression(lastQuestID, false, "All", false))
        {
            shouldRepeat = false;
            return;
        }

        CurrentRequirements.RemoveAt(index);
        shouldRepeat = false;
    }

    private bool _RepeatCheck(ref bool shouldRepeat, int index)
    {
        if (index < 0 || index >= CurrentRequirements.Count)
        {
            shouldRepeat = false;
            return true;
        }

        ItemBase requirement = CurrentRequirements[index];

        if (
            requirement.Temp
                ? Bot.TempInv.Contains(requirement.Name, requirement.Quantity)
                : Core.CheckInventory(requirement.ID, requirement.Quantity)
        )
        {
            CurrentRequirements.RemoveAt(index);
            shouldRepeat = false;
            return true;
        }

        return false;
    }

    private int lastQuestID;

    private void _AddRequirement(int questID)
    {
        if (questID <= 0 || questID == lastQuestID)
            return;

        lastQuestID = questID;

        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questID));

        if (quest == null)
            return;

        List<string> reqItems = [];

        foreach (ItemBase item in quest.AcceptRequirements ?? Enumerable.Empty<ItemBase>())
        {
            if (!string.IsNullOrEmpty(item.Name))
                reqItems.Add(item.Name);
        }

        foreach (ItemBase item in quest.Requirements ?? Enumerable.Empty<ItemBase>())
        {
            if (
                !string.IsNullOrEmpty(item.Name)
                && !CurrentRequirements.Any(i => i.Name == item.Name)
            )
            {
                if (!item.Temp)
                    reqItems.Add(item.Name);

                CurrentRequirements.Add(item);
            }
        }

        if (reqItems.Count > 0)
            Core.AddDrop(reqItems.ToArray());
    }
}
