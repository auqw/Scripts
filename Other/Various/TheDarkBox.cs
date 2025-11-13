/*
name: TheDarkBox
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class TheDarkBox
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Example();

        Core.SetOptions(false);
    }

    public void Example()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(8375).Rewards;
        List<string> RewardsList = new();
        foreach (Skua.Core.Models.Items.ItemBase Item in RewardOptions)
            RewardsList.Add(Item.Name);

        string[] Rewards = RewardsList.ToArray();
        Bot.Drops.Add(Rewards);

        if (
            Core.CheckInventory(Rewards, toInv: false)
            || !Core.CheckInventory(new[] { "Dark Box", "Dark Key" })
        )
            return;

        //The Dark Box 5710
        Core.RegisterQuests(5710);
        while (!Bot.ShouldExit && !Core.CheckInventory(Rewards, toInv: false))
        {
            if (Core.IsMember)
                Core.HuntMonster("ruins", "Dark Elemental", "Dark Gem", isTemp: false);
            else
                Core.HuntMonster("darkfortress", "Dark Elemental", "Dark Gem", isTemp: false);
            Core.JumpWait();
            Core.ToBank(Rewards);
        }
        Core.CancelRegisteredQuests();
    }
}
