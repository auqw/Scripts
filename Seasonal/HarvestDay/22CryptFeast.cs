/*
name: CryptFeast
description: Completes the quest chain in /cryptfeast.
tags: cryptfeast, story, quests, cryptfeast
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using Skua.Core.Interfaces;

public class CryptFeast
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreHarvestDay HarvestDay
    {
        get => _HarvestDay ??= new CoreHarvestDay();
        set => _HarvestDay = value;
    }
    private static CoreHarvestDay _HarvestDay;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
		
		HarvestDay.CharityFeast();
        HarvestDay.CryptFeast();

        Core.SetOptions(false);
    }
}