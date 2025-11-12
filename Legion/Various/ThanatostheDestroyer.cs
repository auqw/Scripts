/*
name: ThanatostheDestroyer
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ThanatostheDestroyer
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreIsleOfFotia CoreIsleOfFotia { get => _CoreIsleOfFotia ??= new CoreIsleOfFotia(); set => _CoreIsleOfFotia = value; }
    private static CoreIsleOfFotia _CoreIsleOfFotia;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;


    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetRewards();

        Core.SetOptions(false);
    }

    public void GetRewards()
    {
        if (!Core.CheckInventory("Thanatos Paragon Pet") || Core.CheckInventory(Core.EnsureLoad(4101).Rewards.Select(i => i.Name).ToArray()))
            Core.Logger("Pet not owned, or All Items already owned.", stopBot: true);
        else Core.Logger("Thanatos Paragon Pet owned, continuing.");

        List<ItemBase> RewardOptions = Core.EnsureLoad(4101).Rewards;
        string[] QuestRewards = RewardOptions.Select(x => x.Name).ToArray();

        CoreIsleOfFotia.UnderRealm();

        Bot.Drops.Add(QuestRewards);

        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(4101);
        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.ID, toInv: false))
                Core.Logger($"{Reward.Name} Found.");
            else
            {
                Core.FarmingLogger(Reward.Name, 1);
                while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID))
                    Core.HuntMonster("underrealm", "Death", "Become Death", log: false);
                Bot.Wait.ForPickup(Reward.ID);
                Core.ToBank(Reward.ID);
            }
        }
        Core.CancelRegisteredQuests();
        Core.Logger($"Jobs Done 👍");
    }
}
