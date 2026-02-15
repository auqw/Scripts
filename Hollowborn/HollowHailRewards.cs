/*
name: Hollow Hail Quest Rewards
description: Farms quest rewards from "With Every Wind", "Hail To The King", and "Cold, Cold, COLD!" quests
tags: Hollow Hail, hollow, hail, hollowborn, With Every Wind, Hail To The King, Cold Cold COLD
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/MergeShops/AcielsGiftsMerge.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class HollowHailRewards
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static AcielsGiftsMerge AGM
    {
        get => _AGM ??= new AcielsGiftsMerge();
        set => _AGM = value;
    }
    private static AcielsGiftsMerge _AGM;

    private static BuyScrolls BS
    {
        get => _BS ??= new BuyScrolls();
        set => _BS = value;
    }
    private static BuyScrolls _BS;

    private static HollowSoul HS
    {
        get => _HS ??= new HollowSoul();
        set => _HS = value;
    }
    private static HollowSoul _HS;

    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private readonly string[] RequiredForLater = new[]
    {
        // Shared requirement
        "The Hollow Hail",

        // Required for Hail To The King
        "Hollowborn Frost King's Scepter",
        "Hollowborn Frost Queen's Scythe",
        "Hollowborn Frost King's Cowl",
        "Hollowborn Frost Queen's Tiara",

        // Required for Cold, Cold, COLD!
        "Hollowborn Frost King",
        "Hollowborn Frost Queen"
    };

    private const int WithEveryWindQuestID = 10604;
    private const int HailToTheKingQuestID = 10605;
    private const int ColdColdCOLDQuestID = 10606;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetAllRewards();
        Core.SetOptions(false);
    }

    public void GetAllRewards()
    {
        AGM.BuyAllMerge("The Hollow Hail");
        WithEveryWindQuestRewards();
        HailToTheKingQuestRewards();
        ColdColdCOLDQuestRewards();
    }

    public void WithEveryWindQuestRewards()
    {
        Quest quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(WithEveryWindQuestID));

        if (quest?.AcceptRequirements?.Count > 0)
        {
            foreach (ItemBase req in quest.AcceptRequirements)
                Core.Unbank(req.Name);
        }

        List<ItemBase> rewardOptions = quest?.Rewards;
        if (rewardOptions == null || rewardOptions.Count == 0)
        {
            Core.Logger("No With Every Wind quest rewards found.");
            return;
        }

        // Register all drops once
        foreach (ItemBase item in rewardOptions)
            Core.AddDrop(item.Name);

        int rewardCount = rewardOptions.Count;

        for (int i = 0; i < rewardCount; i++)
        {
            ItemBase reward = rewardOptions[i];

            if (Core.CheckInventory(reward.Name, toInv: false))
            {
                Core.Logger($"[✓] {reward.Name} already owned — skipping ({i + 1}/{rewardCount})");
                continue;
            }

            Core.Logger(
                $"╔═[ WITH EVERY WIND :: {i + 1}/{rewardCount} ]═╗\n" +
                $"║  Target : {reward.Name}\n" +
                $"║  ID     : {reward.ID}\n" +
                $"╚══════════════════════════════════════════════╝"
            );

            Core.EnsureAccept(WithEveryWindQuestID);

            // Solo Phase
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("kingcoal", "Frost King", "Frost King's Story", isTemp: false);
            Core.HuntMonster("banished", "Desterrat Moya", "The Scythe of Awe", isTemp: false);
            Core.HuntMonster("northstar", "Karok the Fallen", "FrozenFear Crystal", isTemp: false);

            // Farm Phase
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("marsh2", "Soulseeker", "Soulseeker's Grim Hood", isTemp: false);
            Core.HuntMonster("noobshire", "Kittarian Mouse Flayer", "Pitchfork", isTemp: false);
            Core.HuntMonster("doomwood", "Doomwood Soldier", "Cheering Skull Cape", isTemp: false);

            Core.EnsureComplete(WithEveryWindQuestID, reward.ID);
            Core.JumpWait();

            if (!RequiredForLater.Contains(reward.Name))
            {
                Core.ToBank(reward.Name);
                Core.Logger($"[📦 BANKED] {reward.Name}");
            }
            else
                Core.Logger($"[🔒 KEPT] {reward.Name} (needed for later quest)");

            Core.Logger($"[✔ COMPLETED] {reward.Name} secured.");
        }

        Core.Logger("All With Every Wind rewards processed.");
    }

    public void HailToTheKingQuestRewards()
    {
        Quest quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(HailToTheKingQuestID));

        if (quest?.AcceptRequirements?.Count > 0)
        {
            foreach (ItemBase req in quest.AcceptRequirements)
                Core.Unbank(req.Name);
        }

        List<ItemBase> rewardOptions = quest?.Rewards;
        if (rewardOptions == null || rewardOptions.Count == 0)
        {
            Core.Logger("No Hail to the King rewards found.");
            return;
        }

        foreach (ItemBase item in rewardOptions)
            Core.AddDrop(item.Name);

        int rewardCount = rewardOptions.Count;

        for (int i = 0; i < rewardCount; i++)
        {
            ItemBase reward = rewardOptions[i];

            if (Core.CheckInventory(reward.Name, toInv: false))
            {
                Core.Logger($"[✓] {reward.Name} already owned — skipping ({i + 1}/{rewardCount})");
                continue;
            }

            Core.Logger(
                $"╔═[ HAIL TO THE KING :: {i + 1}/{rewardCount} ]═╗\n" +
                $"║  Target : {reward.Name}\n" +
                $"║  ID     : {reward.ID}\n" +
                $"╚═══════════════════════════════════════════════╝"
            );

            Core.EnsureAccept(HailToTheKingQuestID);

            // Solo Phase
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("icedungeon", "Shade of Kyanos", "Required Solo Drop Here", isTemp: false);
            Core.HuntMonster("yulgarparty", "Highborn Necromancer", "Required Solo Drop Here", isTemp: false);

            // Farm Phase
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("noxustower", "r13", "Left", "*", "Human Souls", 500, isTemp: false);
            BS.BuyScroll(Scrolls.Frostbite, 9, true);

            Core.EnsureComplete(HailToTheKingQuestID, reward.ID);
            Core.JumpWait();

            if (!RequiredForLater.Contains(reward.Name))
            {
                Core.ToBank(reward.Name);
                Core.Logger($"[📦 BANKED] {reward.Name}");
            }
            else
                Core.Logger($"[🔒 KEPT] {reward.Name} (needed for later quest)");

            Core.Logger($"[✔ COMPLETED] {reward.Name} secured.");
        }

        Core.Logger("All Hail to the King rewards processed.");
    }

    public void ColdColdCOLDQuestRewards()
    {
        if (!Core.IsMember)
        {
            Core.Logger("Membership required for \"Cold, Cold, COLD!\" quest.");
            return;
        }

        Quest quest = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(ColdColdCOLDQuestID));

        if (quest?.AcceptRequirements?.Count > 0)
        {
            foreach (ItemBase req in quest.AcceptRequirements)
                Core.Unbank(req.Name);
        }

        List<ItemBase> rewardOptions = quest?.Rewards;
        if (rewardOptions == null || rewardOptions.Count == 0)
        {
            Core.Logger("No Cold, Cold, COLD! rewards found.");
            return;
        }

        foreach (ItemBase item in rewardOptions)
            Core.AddDrop(item.Name);

        int rewardCount = rewardOptions.Count;

        for (int i = 0; i < rewardCount; i++)
        {
            ItemBase reward = rewardOptions[i];

            if (Core.CheckInventory(reward.Name, toInv: false))
            {
                Core.Logger($"[✓] {reward.Name} already owned — skipping ({i + 1}/{rewardCount})");
                continue;
            }

            Core.Logger(
                $"╔═[ COLD COLD COLD :: {i + 1}/{rewardCount} ]═╗\n" +
                $"║  Target : {reward.Name}\n" +
                $"║  ID     : {reward.ID}\n" +
                $"╚══════════════════════════════════════════════╝"
            );

            Core.EnsureAccept(ColdColdCOLDQuestID);

            // Solo/Pre-farm Phase
            Core.EquipClass(ClassType.Solo);
            HS.GetYaSoulsHeeeere(3000);
            Farm.BattleUnderB("Bone Dust", 206);

            // Farm Phase
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("noxustower", "r13", "Left", "*", "Human Souls", 500, isTemp: false);

            Core.EnsureComplete(ColdColdCOLDQuestID, reward.ID);
            Core.JumpWait();

            if (!RequiredForLater.Contains(reward.Name))
            {
                Core.ToBank(reward.Name);
                Core.Logger($"[📦 BANKED] {reward.Name}");
            }
            else
                Core.Logger($"[🔒 KEPT] {reward.Name} (needed for later quest)");

            Core.Logger($"[✔ COMPLETED] {reward.Name} secured.");
        }

        Core.Logger("All Cold, Cold, COLD! rewards processed.");
    }
}