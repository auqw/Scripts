/*
name: NoHollowbornClass
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;

public class NoHollowbornClass
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
    private static CoreHollowbornStory HB
    {
        get => _HB ??= new CoreHollowbornStory();
        set => _HB = value;
    }
    private static CoreHollowbornStory _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetNHBC();

        Core.SetOptions(false);
    }

    public void GetNHBC(bool rankUpClass = true)
    {
        if (!Core.isSeasonalMapActive("trygve"))
        {
            return;
        }

        if (Core.CheckInventory("No Hollowborn Class"))
        {
            if (rankUpClass && (Core.CheckClassRank(false, "No Hollowborn Class") < 10))
                Adv.RankUpClass("No Hollowborn Class");
            return;
        }

        HB.Trygve();

        Core.AddDrop("No Hollowborn Class");
        Bot.Quests.UpdateQuest(8298); // "No Hollowborn Class" quest, this is required to get the class ( if you havent done the questline)
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("trygve", "Gramiel", "No Hollowborn Class", isTemp: false);
        Bot.Wait.ForPickup("No Hollowborn Class");

        if (rankUpClass)
            Adv.RankUpClass("No Hollowborn Class");
    }
}
