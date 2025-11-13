/*
name: Vindicator Badge
description: Farms "Vindicator Badge"
tags: vindicator, badge, hollowborn, farm
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class VindicatorBadge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    private int GetMax()
    {
        Quest? quest = Bot.Quests.EnsureLoad(10299);
        if (quest == null)
        {
            return 1; // Default max stack if quest is not found
        }
        ItemBase? req = quest.Requirements.FirstOrDefault(r => r.Name == "Vindicator Badge");
        return req?.MaxStack ?? 1;
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetVindicatorBadge(GetMax());
        Core.SetOptions(false);
    }

    public void GetVindicatorBadge(int? quant = null)
    {
        int badgeQuant = quant ?? GetMax();
        const string item = "Vindicator Badge";
        if (Core.CheckInventory(item, badgeQuant))
            return;

        Bot.Quests.UpdateQuest(8297);
        Core.FarmingLogger(item, badgeQuant);
        Core.AddDrop(item, "Eagle Heart", "Boar Heart", "Vindicator Seal");
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(Core.IsMember ? 10296 : 8299);

        while (!Bot.ShouldExit && !Core.CheckInventory(item, badgeQuant))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("trygve", "r3", "Left", "Blood Eagle", "Eagle Heart", 8, log: false);
            Core.KillMonster("trygve", "r4", "Left", "Rune Boar", "Boar Heart", 8, log: false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("trygve", "Gramiel", "Vindicator Seal", log: false);
            Bot.Wait.ForPickup(item);
        }

        Core.CancelRegisteredQuests();
    }
}
