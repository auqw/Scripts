/*
name: Sentinel
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class Sentinel
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

        GetSentinel();

        Core.SetOptions(false);
    }

    public void GetSentinel(bool rankUpClass = true)
    {
        Core.Logger(
            Core.HasWebBadge("16h Upholder") || Core.HasWebBadge("16th Upholder") ? "true" : "false"
        );
        if (
            (Core.HasWebBadge("16h Upholder") || Core.HasWebBadge("16th Upholder"))
            && Core.CheckInventory("Sentinel")
        )
        {
            if (rankUpClass)
                Adv.RankUpClass("Sentinel");
            return;
        }

        Core.BuyItem(Bot.Map.Name, 2491, "Sentinel");

        if (rankUpClass)
            Adv.RankUpClass("Sentinel");
    }
}
