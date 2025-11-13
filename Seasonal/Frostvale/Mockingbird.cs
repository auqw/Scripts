/*
name: Mockingbird Quest Rewards
description: farms quest rewards from `Mockingbird` in /holidayhotel
tags: holidayhotel, aria, quest rewards,frostvale,mockingbird
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Story/Glacera.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class Mockingbird
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFrostvale Frost
    {
        get => _Frost ??= new CoreFrostvale();
        set => _Frost = value;
    }
    private static CoreFrostvale _Frost;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetRewards();

        Core.SetOptions(false);
    }

    int QuestID = 10004;

    public void GetRewards()
    {
        Frost.HolidayHotel();

        List<ItemBase> RewardOptions = Core.EnsureLoad(QuestID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.Name);

        Core.EquipClass(ClassType.Farm);

        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, toInv: false))
                return;

            Core.FarmingLogger(Reward.Name, 1);

            Core.EnsureAccept(QuestID);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("holidayhotel", "Hotel Guest", "Guest's Ornament", 10, log: false);
            Core.HuntMonster("holidayhotel", "Cold Apparition", "Apparition's Eye", 10, log: false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("holidayhotel", "Memory Leech", "Stolen Memory", log: false);

            Core.EnsureComplete(QuestID, Reward.ID);
            Core.JumpWait();
            Core.ToBank(Reward.Name);
        }
    }
}

/*
Possible extras:
    List<ItemBase> RewardOptions1 = Core.EnsureLoad(QuestID).Rewards;
    List<ItemBase> RewardOptions2 = Core.EnsureLoad(QuestID).Rewards;
  foreach (ItemBase item in RewardOptions1.Concat(RewardOptions2).ToArray())
  {
    if(Core.CheckInventory(item.ID, Quant))
        return;
    
    Core.RegisterQuest(QuestID);
    while (!Bot.ShouldExit && !Core.CheckInventory(item.ID))
    {
        dostuff
    }
    Core.CancelRegisteredQuests();
  }
*/
