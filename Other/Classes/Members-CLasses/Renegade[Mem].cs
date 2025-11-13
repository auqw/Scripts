/*
name: Renegade[Mem]
description: This script will get Renegade class.
tags: renegade, member, class, metrea
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class Renegade
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Getclass();

        Core.SetOptions(false);
    }

    public void Getclass(bool rankUpClass = true)
    {
        if (Core.CheckInventory(410, toInv: false) || !Core.IsMember)
            return;

        if (!Core.CheckInventory(15652))
            Adv.BuyItem("trainers", 172, 15652); // Rogue
        Adv.RankUpClass("Rogue");

        Adv.BuyItem("trainers", 173, 410);
        if (rankUpClass)
            Adv.RankUpClass("Renegade");
    }
}
