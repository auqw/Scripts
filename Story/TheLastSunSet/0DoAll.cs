/*
name: Complete The Last Sun Set
description: This will complete the "The Last Sun Set" arc.
tags: story, quest, The Last Sun Set, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
//cs_include Scripts/Story/TheLastSunSet/TheLastSunSetCore.cs

using Skua.Core.Interfaces;

public class DoAllTLSS
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static TheLastSunSetCore TLSSC
    {
        get => __TLSSC ??= new TheLastSunSetCore();
        set => __TLSSC = value;
    }
    private static TheLastSunSetCore __TLSSC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        TLSSC.DoAll();

        Core.SetOptions(false);
    }
}
