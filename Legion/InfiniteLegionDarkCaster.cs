/*
name: Infinite Legion Dark Caster (Class)
description: This script will get Infinite Legion Dark Caster class and farm it to rank 10.
tags: ildc, infinite legion dc, dark caster, legion, class, dark birthday, undead legion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;

public class InfiniteLegionDC
{
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
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetILDC();

        Core.SetOptions(false);
    }

    public void GetILDC(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Infinite Legion Dark Caster"))
        {
            if (rankUpClass)
                Adv.RankUpClass("Infinite Legion Dark Caster");
            return;
        }

        Legion.FarmLegionToken(2000);
        Core.BuyItem("underworld", 238, "Infinite Legion Dark Caster");

        if (rankUpClass)
            Adv.RankUpClass("Infinite Legion Dark Caster");
    }
}
