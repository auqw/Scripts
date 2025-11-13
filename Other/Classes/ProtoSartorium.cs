/*
name: ProtoSartorium (Class)
description: This script will get ProtoSartorium class.
tags: proto sartorium, protosartorium, mithril man, dwakel, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class ProtoSartorium
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetPS();

        Core.SetOptions(false);
    }

    public void GetPS(bool rankUpClass = true)
    {
        if (Core.CheckInventory("ProtoSartorium") || !Core.IsMember)
        {
            if (rankUpClass && Core.IsMember)
                Adv.RankUpClass("ProtoSartorium");
            return;
        }

        Core.AddDrop("ProtoSartorium");

        Core.HuntMonster("crashsite", "ProtoSartorium", "ProtoSartorium", isTemp: false);

        if (rankUpClass)
            Adv.RankUpClass("ProtoSartorium");
    }
}
