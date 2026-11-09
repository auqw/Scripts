/*
name: Hikaru Quest Rewards
description: farms quest rewards from `Hit List` in /lostvilla
tags: lostvilla, Hikaru, quest rewards, diabolical shadowhunter, diabolical,shadow hunter, hit list, hitlist
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Banished.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
//cs_include Scripts/Story/LostVilla.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class DiabolicalShadowHunterSet
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static LostVilla LV
    {
        get => _LV ??= new LostVilla();
        set => _LV = value;
    }
    private static LostVilla _LV;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        LV.Storyline();
        GetRewards();

        Core.SetOptions(false);
    }

    int QuestID = 10053;

    public void GetRewards()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(QuestID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.ID);

        Core.EquipClass(ClassType.Farm);

        foreach (ItemBase Reward in RewardOptions)
        {
            Core.Logger(
                Core.CheckInventory(Reward.ID, toInv: false)
                    ? $"{Reward.Name}: ✅"
                    : $"{Reward.Name} ❌"
            );

            if (Core.CheckInventory(Reward.ID, toInv: false))
                continue;

            Core.FarmingLogger(Reward.Name, 1);
            while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID))
            {
                Core.EnsureAccept(10053);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("brokenwoods", "Eldritch Amalgamation", "Eldritch Amalgamation's Strained Eye");
                Core.HuntMonster("backroom", "Book Wyrm", "Book Wyrm's Wretched Vein", 2);
                Core.HuntMonster("lostvilla", "Eldritch Parasite", "Eldritch Parasite's Infested Arteries", 12);
                Core.HuntMonster("lostvilla", "Covetous Disgrace", "Covetous Disgrace's Mangled Rib", 3);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("lostvilla", "Mutilated Atrocity", "Mutilated Atrocity's Dismembered Winglet", 5);
                Core.EnsureComplete(10053);
            }

            Core.JumpWait();
            Core.ToBank(Reward.ID);
        }
    }
}
