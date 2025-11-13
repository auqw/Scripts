/*
name: Gathering Unstable Essences
description: farms void auras via the "gathering unstable essences" quest.
tags: gathering, unstable, essences, void aura
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
using Skua.Core.Interfaces;

public class GatheringUnstableEssences
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNSOD NSoD
    {
        get => _NSoD ??= new CoreNSOD();
        set => _NSoD = value;
    }
    private static CoreNSOD _NSoD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        NSoD.GatheringUnstableEssences();

        Core.SetOptions(false);
    }
}
