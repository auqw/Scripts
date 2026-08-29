/*
name: RankUpEquippedClass
description: rank up current class
tags: rank, up, class, rank class, rank up class, class rank, class points
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class RankUpEquippedClass
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
        Core.SetOptions(disableClassSwap: true);

        DoRankUpEquippedClass();

        Core.SetOptions(false);
    }

    public void DoRankUpEquippedClass()
    {
        if (Bot.Player.CurrentClass != null)
        {
            if (Bot.Player.CurrentClassRank < 10)
            {
                Adv.RankUpClass(Bot.Player.CurrentClass.Name);
            }
            else
            {
                Core.Logger($"{Bot.Player.CurrentClass.Name} is already at max rank.");
            }
        }
        else
        {
            Core.Logger("No class is currently equipped.");
        }
    }
}
