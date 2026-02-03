/*
name: He Limed Me
description: Farms The Random Rewards from `He Limed Me`
tags: HeLimedMe, He Limed Me, He, Limed, Me
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Extinction.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class HeLimedMe
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private Extinction extinction => new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetItems();

        Core.SetOptions(false);
    }

    public void GetItems()
    {
        extinction.StoryLine();
        int QuestID = 10585;

        List<ItemBase>? RewardOptions = Core.InitializeWithRetries(() => Core.EnsureLoad(QuestID).Rewards);

        if (RewardOptions == null)
        {
            Core.Logger("Failed to load quest rewards.");
            return;
        }

        Core.AddDrop(Core.QuestRewards(QuestID));

        Core.EquipClass(ClassType.Solo);

        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, toInv: false))
            {
                Core.Logger($"{Reward.Name} Owned!");
                continue;
            }

            Core.FarmingLogger(Reward.Name);
            while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID))
            {
                Core.EnsureAccept(QuestID);
                Core.HuntMonster("Ectocave", "Ektorax", "Regurgitated Key");
                Core.KillMonster("ectocave", "r1", "Left", "*", "Ecto Slime", 50);
                Core.EnsureComplete(QuestID);
            }

            Core.JumpWait();
            Core.ToBank(Reward.ID);
        }
        Core.CancelRegisteredQuests();
    }
}