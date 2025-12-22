/*
name: Refreshment Retrieval
description: This will obtain all of the reward items on Refreshment Retrieval quest.
tags: refreshment-retrieval, seasonal, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class RefreshmentRetrieval
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    int questID = 9029;
    int quant = 1;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        RandomReward(questID, quant);

        Core.SetOptions(false);
    }

    private void RandomReward(int questID, int quant)
    {
        QuestPreReq();

        List<ItemBase> RewardOptions = Core.EnsureLoad(questID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Bot.Drops.Add(item.Name);

        string[] QuestRewards = RewardOptions.Select(x => x.Name).ToArray();

        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(questID);
        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, toInv: false))
                Core.Logger($"{Reward.Name} Found.");
            else
            {
                Core.HuntMonster("caroltown", "Frostval Deer", Reward.Name);

            }
        }
    }

    public void QuestPreReq()
    {
        if (Core.isCompletedBefore(9028))
            return;

        Core.AddDrop("Red Ribbon");

        Core.EquipClass(ClassType.Solo);
        Core.EnsureAccept(9028);
        while (!Bot.ShouldExit && !Bot.Quests.CanComplete(9028))
        {
            Core.HuntMonster("caroltown", "Frostval Tree");

        }
        Core.EnsureComplete(9028);
    }
}
