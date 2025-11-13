/*
name: Rustbucket (Class)
description: This script will get ProtoSartorium class.
tags: rustbucket, early game, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class Rustbucket
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

        GetRustbucket();

        Core.SetOptions(false);
    }

    public void GetRustbucket(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Rustbucket"))
        {
            if (rankUpClass)
                Adv.RankUpClass("Rustbucket");
            return;
        }

        Core.AddDrop("Rustbucket");

        if (!Core.CheckInventory("Rustbucket"))
        {
            Core.GetMapItem(12756, 1, "crashsite");
            Bot.Wait.ForPickup("Rustbucket");
        }

        if (rankUpClass)
            Adv.RankUpClass("Rustbucket");
    }
}
