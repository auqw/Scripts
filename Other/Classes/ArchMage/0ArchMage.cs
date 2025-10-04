/*
name: 0ArchMage
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Other/Classes/ArchMage/CoreArchMage.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
using Skua.Core.Interfaces;

public class ArchMage
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreArchMage AM { get => _AM ??= new CoreArchMage(); set => _AM = value; }
    private static CoreArchMage _AM;

    public bool DontPreconfigure = true;
    public string OptionsStorage = AM.OptionsStorage;
    public List<IOption> Options = AM.Options;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        AM.GetAM();

        Core.SetOptions(false);
    }
}
