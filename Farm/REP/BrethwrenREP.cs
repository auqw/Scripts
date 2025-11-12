/*
name: Brethwren REP [Seasonal]
description: This script will farm Brethwren reputation to rank 10.
tags: reputation, rep, seasonal, rank, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs

using Skua.Core.Interfaces;
public class BrethwrenREPFarm
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreHarvestDay HarvestDay { get => _HarvestDay ??= new CoreHarvestDay(); set => _HarvestDay = value; }
    private static CoreHarvestDay _HarvestDay;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoRep();

        Core.SetOptions(false);
    }

    public void DoRep()
    {
        HarvestDay.BirdsWithHarms();

        Farm.BrethwrenREP();
    }
}
