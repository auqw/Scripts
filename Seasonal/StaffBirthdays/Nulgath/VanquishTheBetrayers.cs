/*
name: Vanquish The Betrayers Quest Rewards
description: Farms quest rewards from `Vanquish The Betrayers` [9932] in /voidrefuge, /fiendpast and /darkalliance.
tags: voidrefuge,seasonal,nulgath,birthday, quest rewards,vanquish the betrayers,fiend's trophy,fiends trophy,fiend trophy,fiend's draconic,fiends draconic,fiend draconic
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class VanquishTheBetrayers
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetRewards();

        Core.SetOptions(false);
    }

    int QuestID = 9932;

    public void GetRewards()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(QuestID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.Name);

        Core.EquipClass(ClassType.Farm);

        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, toInv: false))
                return;

            Core.FarmingLogger(Reward.Name, 1);

            Core.HuntMonsterQuestChoose(
                QuestID,
                Reward.Name,
                new[]
                {
                    ("voidrefuge", "Nation Outrider", ClassType.Farm),
                    ("fiendpast", "Scarvitas", ClassType.Farm),
                    ("darkalliance", "Shadowblade", ClassType.Farm),
                }
            );
            Core.JumpWait();
            Bot.Wait.ForPickup(Reward.Name);
            Core.ToBank(Reward.Name);
        }
    }
}
