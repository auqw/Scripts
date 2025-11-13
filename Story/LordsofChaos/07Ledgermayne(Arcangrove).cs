/*
name: (Arcangrove) Ledgermayne
description: chaos-saga, 13-lords-of-chaos
tags: story, quest, chaos-saga, 13-lords-of-chaos, arcangrove, ledgermayne
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class SagaArcangrove
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        LOC.Ledgermayne();

        Core.SetOptions(false);
    }
}
