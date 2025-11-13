/*
name: Vindicator Crest
description: Farms "Vindicator Crest"
tags: vindicator crest, hollowborn, vindicator, farm
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class VindicatorCrest
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    private int GetMax()
    {
        Quest? quest = Bot.Quests.EnsureLoad(9865);
        if (quest == null)
            return 1;
        ItemBase? req = quest.Rewards.FirstOrDefault(r => r.Name == "Vindicator Crest");
        return req?.MaxStack ?? 1;
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetVindicatorCrest(GetMax());
        Core.SetOptions(false);
    }

    public void GetVindicatorCrest(int? quant = null)
    {
        int crestQuant = quant ?? GetMax();
        const string item = "Vindicator Crest";
        if (Core.CheckInventory(item, crestQuant))
            return;

        Core.FarmingLogger(item, crestQuant);
        Core.AddDrop(item, "Vindicated Blades", "Vindicated Chain", "Vindicated Scripture");
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(Core.IsMember ? 10298 : 9865);

        while (!Bot.ShouldExit && !Core.CheckInventory(item, crestQuant))
        {
            Core.HuntMonsterMapID("neotower", 12, "Vindicated Blades", log: false);
            Core.HuntMonsterMapID("neotower", 17, "Vindicated Chain", log: false);
            Core.HuntMonsterMapID("neotower", 28, "Vindicated Scripture", log: false);
            Bot.Wait.ForPickup(item);
        }

        Core.CancelRegisteredQuests();
    }
}
