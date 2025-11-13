/*
name: Gramiel's Emblem
description: Farms "Gramiel's Emblem"
tags: gramiel, emblem, hollowborn, vindicator, farm
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class GramielsEmblem
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
        ItemBase? req = quest.Requirements.FirstOrDefault(r => r.Name == "Gramiel's Emblem");
        return req?.MaxStack ?? 1;
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetGramielsEmblem(GetMax());
        Core.SetOptions(false);
    }

    public void GetGramielsEmblem(int? quant = null)
    {
        int emblemQuant = quant ?? GetMax();
        const string item = "Gramiel's Emblem";
        if (Core.CheckInventory(item, emblemQuant))
            return;

        Core.FarmingLogger(item, emblemQuant);
        Core.AddDrop(item);
        Core.EquipClass(ClassType.Solo);

        Core.HuntMonster("dawnsanctum", "Celestial Gramiel", item, emblemQuant, false);
        Bot.Wait.ForPickup(item);
    }
}
