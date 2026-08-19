
/*
name: Flames of Love
description: Farms the 2 drops from `Flames of Love` 
tags: FlamesofLove, flames of love, 5 percent
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;

public class FlamesofLove
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FoL();

        Core.SetOptions(false);
    }

    public void FoL()
    {
        string[] rewards = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(10787)).Rewards.ToArray();

        if (Core.CheckInventory(rewards))
            return;

        Core.AddDrop(rewards);
        Core.RegisterQuests(10787);

        while (!Bot.ShouldExit && !Core.CheckInventory(rewards))
        {
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("fireplanewar", "Shadefire Onslaught", "Flame of Passion", 8);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("skytower", "Sunstone", "Sunlit Warmth", 8);
        }
        Core.CancelRegisteredQuests();
    }
}
