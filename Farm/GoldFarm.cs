/*
name: Gold Farm
description: honor hall for members, BGE for f2p
tags: gold, battle ground e, honor hall, berserker bunny
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class GoldFarm
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);

        DoFarmGold();

        Core.SetOptions(false);
    }

    public void DoFarmGold()
    {
        Core.EquipClass(ClassType.Farm);
        Bot.Drops.Start();

        Farm.Gold();
    }
}
