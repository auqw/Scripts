/*
name: Grace Orb
description: Farms "Grace Orb"
tags: grace orb, hollowborn, vindicator, farm
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class GraceOrb
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    private int GetMax()
    {
        Quest? quest = Bot.Quests.EnsureLoad(9291);
        if (quest == null)
            return 1;
        ItemBase? req = quest.Rewards.FirstOrDefault(r => r.Name == "Grace Orb");
        return req?.MaxStack ?? 1;
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetGraceOrb(GetMax());
        Core.SetOptions(false);
    }

    public void GetGraceOrb(int? quant = null)
    {
        int orbQuant = quant ?? GetMax();
        if (Core.CheckInventory("Grace Orb", orbQuant))
            return;

        Core.RegisterQuests(Core.IsMember ? 10297 : 9291);
        Core.AddDrop("Grace Orb", "Grace Extracted");
        Core.FarmingLogger("Grace Orb", orbQuant);
        Core.EquipClass(ClassType.Farm);

        while (!Bot.ShouldExit && !Core.CheckInventory("Grace Orb", orbQuant))
        {
            Core.HuntMonster(
                "neofortress",
                "Vindicator Recruit",
                "Grace Extracted",
                20,
                log: false
            );
            Bot.Wait.ForPickup("Grace Orb");
        }

        Core.CancelRegisteredQuests();
        Bot.Wait.ForPickup("Grace Orb");
    }
}
