/*
name: SunSetDoAll
description: Completes the quest chain in /templeofdoom.
tags: templeofdoom, story, quests, templeofdoom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs 
//cs_include Scripts/Story/SunSetSaga/CoreSunSet.cs
using Skua.Core.Interfaces;

public class DoAllSunSet
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreSunSet CSS { get => _CSS ??= new CoreSunSet(); set => _CSS = value; }
    private static CoreSunSet _CSS;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        CSS.DoAll();
        Core.SetOptions(false);
    }


}
