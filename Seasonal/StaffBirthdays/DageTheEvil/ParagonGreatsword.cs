/*
name: Paragon Greatsword BattleGear
description: This script will complete the "Soul Food" [10104] quest for Paragon Greatsword BattleGear.
tags: dage, birthday, seasonal, soul food, bonus quest, paragon,greatsword,battlegear
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class ParagonGS
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetCape();

        Core.SetOptions(false);
    }

    public void GetCape()
    {
        if (Core.CheckInventory(43061))
            return;

        Core.AddDrop(43061);
        Core.HuntMonsterQuest(
            10104,
            ("darkpath", "Void Knight", ClassType.Farm),
            ("darkpath", "Void Wyrm", ClassType.Solo),
            ("darkpath", "Void Elemental", ClassType.Farm)
        );
        Bot.Wait.ForPickup(43061);
    }
}
