/*
name: Death's Power
description: Farms "Death's Power"
tags: death's power,deaths power, death,power, shadowattack,death, hollowborn
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class DeathsPower
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
        ItemBase? req = quest.Requirements.FirstOrDefault(r => r.Name == "Death's Power");
        return req?.MaxStack ?? 1;
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetDP(GetMax());

        Core.SetOptions(false);
    }

    public void GetDP(int? quant = null)
    {
        int DPQuant = quant ?? GetMax();
        if (Core.CheckInventory("Death's Power", DPQuant))
            return;

        Core.EnsureAccept(10299); // Quest needs to be accepted for Death's Power to drop
        Core.AddDrop("Death's Power");
        Core.FarmingLogger("Death's Power", DPQuant);
        Core.EquipClass(ClassType.Solo);
        Core.KillMonster("shadowattack", "Boss", "Left", "Death", "Death's Power", DPQuant, false);
    }
}
