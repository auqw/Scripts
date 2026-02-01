/*
name: Blood From the Void
description: Farms Blood From the Void from `Obey Yourself, or be Commanded` in /tercesinvasion
tags: tercesinvasion, Jadzia, Blood From the Void, Nulgath Saga, Nulgath Merge, Nulgath Birthday
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class BloodFromTheVoid
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBFTV();

        Core.SetOptions(false);
    }

    public void GetBFTV()
    {
        int QuestID = Bot.Player.IsMember ? 10583 : 10582;
        Core.Logger(QuestID == 10583 ? "Member method" : "Non-member method");

        if (!Core.isCompletedBefore(10581))
        {
            Core.Logger("This farm requires the story in /tercesinvasion to be completed. Please complete the story and try again.", stopBot: true);
            return;
        }

        List<ItemBase> RewardOptions = Core.EnsureLoad(QuestID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.Name);

        Core.EquipClass(ClassType.Solo);

        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, 300, toInv: false))
                continue;

            Core.Logger(Core.CheckInventory(Reward.ID, 300, toInv: false) ? $"{Reward.Name}: ✅" : $"{Reward.Name} ❌");

            Core.RegisterQuests(QuestID);
            Core.FarmingLogger(Reward.Name, 300);
            while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID, 300))
            {
                Core.HuntMonster("tercesinvasion", "Archfiend Rodeleros", "Rodeleros' Blade Shard", quant: 1, isTemp: true, log: false);
                Core.HuntMonster("tercesinvasion", "Archfiend Vigneron", "Vigneron's Chalice", quant: 1, isTemp: true, log: false);
                Core.HuntMonster("tercesinvasion", "Archfiend Casimir", "Casimir's Pinky", quant: 1, isTemp: true, log: false);
            }
            Core.CancelRegisteredQuests();

            Core.JumpWait();
            Core.ToBank(Reward.Name);
        }
    }
}
