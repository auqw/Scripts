/*
name: BloodSorceress
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class BloodSorceress
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBSorc();

        Core.SetOptions(false);
    }

    public void GetBSorc(bool rankUpClass = true)
    {
        if (Core.CheckInventory(/*Blood Sorceress*/ 36298))
        {
            if (rankUpClass)
            {
                Adv.RankUpClass("Blood Sorceress", itemid: 36298);
            }
            return;
        }

        Core.EquipClass(ClassType.Solo);
        while (!Bot.ShouldExit && !Core.CheckInventory(36298))
            Core.KillMonster("towerofmirrors", "r16", "Top", "*");
        Core.JumpWait();
        Bot.Wait.ForPickup(36298);

        if (rankUpClass)
            Adv.RankUpClass("Blood Sorceress", itemid: 36298);
    }
}
