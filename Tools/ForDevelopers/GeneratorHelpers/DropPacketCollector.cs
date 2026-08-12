/*
name: Drop Packet Collector
description: Collects and logs every monster drop in the current map from monsterDrops packets.
tags: developer, drops, packets, monsters, collector
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Tools\ForDevelopers/GeneratorHelpers/Generatesupportutils.cs

using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;

/// <summary>Collects the cell-scoped monsterDrops packets for the current map.</summary>
public sealed class DropPacketCollector
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public sealed record DropItem(
        int ID,
        string Name,
        bool Temp,
        IReadOnlyList<string> QuestGated,
        IReadOnlyList<string> QuestObjectives
    );
    public sealed record MonsterDrops(
        int MonsterID,
        string MonsterName,
        int MaxHP,
        IReadOnlyList<int> MonMapIDs,
        IReadOnlyList<DropItem> Items
    );

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            IReadOnlyList<MonsterDrops> monsters = Collect();
            Core.Logger($"Collected drops for {monsters.Count} monster(s) in /{Bot.Map.Name}.");
            foreach (MonsterDrops monster in monsters)
            {
                string mapIDs = string.Join(", ", monster.MonMapIDs);
                Core.Logger($"{monster.MonsterName} [MonID {monster.MonsterID}; MonMapID(s) {mapIDs}]");
                if (monster.Items.Count == 0)
                {
                    Core.Logger("  (no drops)");
                    continue;
                }

                foreach (DropItem item in monster.Items)
                    Core.Logger($"  {item.ID} | {item.Name}{(item.Temp ? " [Temp]" : string.Empty)}");
            }
        }
        catch (Exception ex)
        {
            Core.Logger($"Drop collection failed: {ex.Message}", messageBox: true);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    public IReadOnlyList<MonsterDrops> Collect(int timeoutSeconds = 10)
    {
        List<Monster> monsters = Bot.Monsters.MapMonsters
            .Where(monster => monster != null)
            .OrderBy(monster => monster.Cell)
            .ThenBy(monster => monster.MapID)
            .ToList();
        if (monsters.Count == 0)
            return Array.Empty<MonsterDrops>();

        string originalCell = Bot.Player.Cell;
        string originalPad = Bot.Player.Pad;
        HashSet<int> queriedMonMapIDs = new();
        List<(Monster Monster, IReadOnlyList<DropItem> Items)> responses = new();

        try
        {
            IEnumerable<string> monsterCells = Bot.Map.Cells
                .Where(cell => monsters.Any(monster => monster.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase)))
                .Concat(monsters.Select(monster => monster.Cell))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (string cell in monsterCells)
            {
                if (Bot.ShouldExit)
                    break;

                if (!Bot.Player.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase))
                {
                    string pad = cell.Equals("Enter", StringComparison.OrdinalIgnoreCase) ? "Spawn" : "Left";
                    Bot.Map.Jump(cell, pad, autoCorrect: false);
                    Bot.Wait.ForCellChange(cell);
                    Bot.Sleep(500);
                }

                foreach (Monster monster in monsters.Where(monster =>
                    monster.Cell.Equals(cell, StringComparison.OrdinalIgnoreCase)
                    && queriedMonMapIDs.Add(monster.MapID)
                ))
                {
                    responses.Add((monster, Request(monster.MapID, timeoutSeconds)));
                }
            }
        }
        finally
        {
            if (!Bot.ShouldExit && !Bot.Player.Cell.Equals(originalCell, StringComparison.OrdinalIgnoreCase))
            {
                Bot.Map.Jump(originalCell, originalPad, autoCorrect: false);
                Bot.Wait.ForCellChange(originalCell);
            }
        }

        List<MonsterDrops> result = new();
        foreach (IGrouping<int, (Monster Monster, IReadOnlyList<DropItem> Items)> group in responses.GroupBy(x => x.Monster.ID))
        {
            var first = group.First();
            result.Add(new MonsterDrops(
                group.Key,
                first.Monster.Name,
                first.Monster.MaxHP,
                group.Select(entry => entry.Monster.MapID).Distinct().OrderBy(id => id).ToArray(),
                group.SelectMany(entry => entry.Items)
                    .GroupBy(item => item.ID)
                    .Select(MergeDropItem)
                    .OrderBy(item => item.ID)
                    .ToArray()
            ));
        }
        return result.OrderBy(monster => monster.MonsterName).ToArray();
    }

    private IReadOnlyList<DropItem> Request(int monMapID, int timeoutSeconds)
    {
        JObject? response = null;
        using ManualResetEventSlim received = new(false);

        void Listener(dynamic packet)
        {
            try
            {
                JToken root = packet is JToken token ? token : JToken.FromObject(packet);
                JToken? data = root["params"]?["dataObj"];
                if (root["params"]?["type"]?.ToString() != "json"
                    || data?["cmd"]?.ToString() != "monsterDrops"
                    || data["MonMapID"]?.Value<int>() != monMapID
                    || data["items"] == null
                    || data["items"]!.Type == JTokenType.Null)
                    return;

                response = (JObject)data;
                received.Set();
            }
            catch { }
        }

        Bot.Events.ExtensionPacketReceived += Listener;
        try
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(Math.Max(1, timeoutSeconds));
            while (!Bot.ShouldExit && !received.IsSet && DateTime.UtcNow < deadline)
            {
                Bot.Send.Packet($"%xt%zm%getMonsterDrops%{Bot.Map.RoomID}%{monMapID}%");
                received.Wait(500);
            }
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= Listener;
        }

        if (response == null)
            throw new TimeoutException($"Timed out loading drops for MonMapID {monMapID}.");

        IEnumerable<DropItem> items = response["items"] switch
        {
            JArray array => array.Children().Select(item => ParseItem(item)),
            // monsterDrops.items is an object keyed by ItemID. Some packet
            // variants omit ItemID from the value, so the property name is the
            // authoritative fallback instead of being discarded.
            JObject obj => obj.Properties().Select(property =>
                ParseItem(property.Value, int.TryParse(property.Name, out int id) ? id : 0)
            ),
            _ => Enumerable.Empty<DropItem>(),
        };
        return items.Where(item => item.ID > 0).OrderBy(item => item.ID).ToArray();
    }

    private static DropItem ParseItem(JToken item, int keyedID = 0)
    {
        int itemID = GeneratorSupportUtils.ReadInt(item, "ItemID", "ID", "iItemID");
        return new(
            itemID > 0 ? itemID : keyedID,
            ReadString(item, "sName", "Name", "strName"),
            GeneratorSupportUtils.ReadInt(item, "bTemp", "Temp", "isTemp") != 0,
            ReadStrings(item, "questGated"),
            ReadStrings(item, "questObjective")
        );
    }

    private static DropItem MergeDropItem(IGrouping<int, DropItem> items)
    {
        DropItem first = items.First();
        return first with
        {
            QuestGated = items.SelectMany(item => item.QuestGated)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            QuestObjectives = items.SelectMany(item => item.QuestObjectives)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray(),
        };
    }

    private static string ReadString(JToken item, params string[] names)
    {
        foreach (string name in names)
            if (!string.IsNullOrWhiteSpace(item[name]?.ToString()))
                return item[name]!.ToString();
        return string.Empty;
    }

    private static IReadOnlyList<string> ReadStrings(JToken item, string name) => item[name] switch
    {
        JArray array => array.Values<string>()
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray(),
        JValue value when !string.IsNullOrWhiteSpace(value.ToString()) => [value.ToString()],
        _ => Array.Empty<string>(),
    };
}