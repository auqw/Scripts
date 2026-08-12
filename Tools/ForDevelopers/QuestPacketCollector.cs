//cs_include Scripts/CoreBots.cs

using System.Threading;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

/// <summary>Loads one quest directly from its getQuests response packet.</summary>
public sealed class QuestPacketCollector
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private static readonly HashSet<int> ChoiceRewardQuestIDs = new();

    public static bool HasChoiceRewards(Quest quest) =>
        ChoiceRewardQuestIDs.Contains(quest.ID)
        || quest.SimpleRewards?.Any(reward => reward != null && reward.Type == 2) == true;

    public Quest Load(int questID, int timeoutSeconds = 10)
    {
        if (questID <= 0)
            throw new ArgumentOutOfRangeException(nameof(questID));

        JToken? questToken = null;
        using ManualResetEventSlim received = new(false);

        void Listener(dynamic packet)
        {
            try
            {
                JToken root = packet is JToken token ? token : JToken.FromObject(packet);
                JToken? data = root["params"]?["dataObj"];
                string command = data?["cmd"]?.ToString() ?? string.Empty;
                if (root["params"]?["type"]?.ToString() != "json"
                    || (command != "getQuests" && command != "getQuests2"))
                    return;

                JToken? quests = data?["quests"];
                JToken? match = quests?[questID.ToString()];
                if (match == null && quests is JArray array)
                    match = array.FirstOrDefault(candidate =>
                        candidate?["QuestID"]?.Value<int>() == questID
                    );
                if (match == null || match.Type == JTokenType.Null)
                    return;

                questToken = match.DeepClone();
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
                Bot.Send.Packet($"%xt%zm%getQuests%{Bot.Map.RoomID}%{questID}%");
                received.Wait(500);
            }
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= Listener;
        }

        if (questToken == null)
            throw new TimeoutException($"Timed out waiting for the getQuests packet for quest {questID}.");

        Quest? quest = questToken.ToObject<Quest>();
        if (quest == null)
            throw new InvalidOperationException($"The getQuests packet for quest {questID} could not be deserialized.");

        // getQuests commonly keys the quest by ID without repeating QuestID in
        // the nested object, so the authoritative requested/keyed ID is used.
        quest.ID = questID;
        quest.AcceptRequirements = ParseItems(questToken["oReqd"]);
        SetRequirements(quest, ParseRequirements(questToken));
        if (HasEntries(questToken["oRewards"]?["itemsC"]))
            ChoiceRewardQuestIDs.Add(questID);
        return quest;
    }

    /// <summary>Converts normalized QuestData.json data without losing requirements.</summary>
    public static Quest FromData(QuestData data)
    {
        List<Skua.Core.Models.Items.ItemBase> requirements = data.Requirements ?? new();
        JObject raw = new()
        {
            ["QuestID"] = data.ID,
            ["iSlot"] = data.Slot,
            ["iValue"] = data.Value,
            ["sName"] = data.Name,
            ["sDesc"] = string.Empty,
            ["sEndText"] = string.Empty,
            ["bOnce"] = data.Once ? "1" : "0",
            ["sField"] = data.Field,
            ["iIndex"] = data.Index,
            ["bUpg"] = data.Upgrade ? "1" : "0",
            ["iLvl"] = data.Level,
            ["iClass"] = data.RequiredClassID,
            ["iReqCP"] = data.RequiredClassPoints,
            ["FactionID"] = data.RequiredFactionId,
            ["iReqRep"] = data.RequiredFactionRep,
            ["iGold"] = data.Gold,
            ["iExp"] = data.XP,
            ["oReqd"] = ItemsByID(data.AcceptRequirements ?? new()),
            ["oItems"] = ItemsByID(requirements),
            ["turnin"] = new JArray(requirements.Select(item => new JObject
            {
                ["ItemID"] = item.ID,
                ["iQty"] = item.Quantity,
            })),
        };
        Quest quest = raw.ToObject<Quest>()
            ?? throw new InvalidOperationException($"Cached quest {data.ID} could not be converted.");
        quest.AcceptRequirements = data.AcceptRequirements ?? new();
        SetRequirements(quest, requirements);
        quest.Rewards = data.Rewards ?? new();
        quest.SimpleRewards = data.SimpleRewards ?? new();
        if (quest.SimpleRewards.Any(reward => reward != null && reward.Type == 2))
            ChoiceRewardQuestIDs.Add(quest.ID);
        return quest;

        static JObject ItemsByID(IEnumerable<Skua.Core.Models.Items.ItemBase> items) =>
            new(items
                .Where(item => item.ID > 0)
                .GroupBy(item => item.ID)
                .Select(group => new JProperty(group.Key.ToString(), JToken.FromObject(group.First()))));
    }

    private static List<ItemBase> ParseRequirements(JToken questToken)
    {
        List<ItemBase> requirements = ParseItems(questToken["oItems"]);
        Dictionary<int, int> quantities = ReadTurninQuantities(questToken["turnin"]);
        foreach (ItemBase item in requirements)
            if (quantities.TryGetValue(item.ID, out int quantity))
                item.Quantity = quantity;
        return requirements;
    }

    private static List<ItemBase> ParseItems(JToken? source)
    {
        IEnumerable<(JToken Token, int KeyedID)> values = source switch
        {
            JObject obj => obj.Properties().Select(property => (
                property.Value,
                int.TryParse(property.Name, out int id) ? id : 0
            )),
            JArray array => array.Select(token => (token, 0)),
            _ => Enumerable.Empty<(JToken, int)>(),
        };

        List<ItemBase> result = new();
        foreach ((JToken token, int keyedID) in values)
        {
            ItemBase? item = token.ToObject<ItemBase>();
            if (item == null)
                continue;
            if (item.ID <= 0)
                item.ID = keyedID;
            if (item.ID > 0)
                result.Add(item);
        }
        return result;
    }

    private static Dictionary<int, int> ReadTurninQuantities(JToken? source)
    {
        IEnumerable<JToken> values = source switch
        {
            JArray array => array.Children(),
            JObject obj => obj.Properties().Select(property => property.Value),
            _ => Enumerable.Empty<JToken>(),
        };
        Dictionary<int, int> result = new();
        foreach (JToken value in values)
        {
            int id = ReadInt(value, "ItemID", "ID", "iItemID");
            int quantity = ReadInt(value, "iQty", "Quantity", "Qty");
            if (id > 0)
                result[id] = Math.Max(1, quantity);
        }
        return result;
    }

    private static int ReadInt(JToken value, params string[] names)
    {
        foreach (string name in names)
            if (int.TryParse(value[name]?.ToString(), out int parsed))
                return parsed;
        return 0;
    }

    private static bool HasEntries(JToken? value) => value switch
    {
        JObject obj => obj.HasValues,
        JArray array => array.Count > 0,
        _ => false,
    };

    private static void SetRequirements(Quest quest, List<ItemBase> requirements)
    {
        FieldInfo? cache = typeof(Quest).GetField("_reqCache", BindingFlags.Instance | BindingFlags.NonPublic);
        if (cache == null)
            throw new InvalidOperationException("The Quest requirements cache field was not found.");
        cache.SetValue(quest, requirements);
    }
}
