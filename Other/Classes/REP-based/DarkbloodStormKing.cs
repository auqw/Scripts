/*
name: DarkbloodStormKing
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class DarkbloodStormKing
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
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetDSK();

        Core.SetOptions(false);
    }

    public void GetDSK(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Darkblood StormKing"))
            return;

        LOC.Lionfang();

        Adv.BuyItem("stormtemple", 544, "Darkblood StormKing", shopItemID: 10412);

        if (rankUpClass)
            Adv.RankUpClass("Darkblood StormKing");
    }
}
