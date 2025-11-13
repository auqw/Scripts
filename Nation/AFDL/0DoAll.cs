/*
name: ADFL Do ALl
description: Does the entirety of AFDL
tags: afdl, archfiend doom lord, doall, 0doall
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/EnoughDOOMforanArchfiend.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
using Skua.Core.Interfaces;

public class ADFL
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static WillpowerExtraction WillpowerExtraction
    {
        get => _WillpowerExtraction ??= new WillpowerExtraction();
        set => _WillpowerExtraction = value;
    }
    private static WillpowerExtraction _WillpowerExtraction;
    private static NulgathDemandsWork NulgathDemandsWork
    {
        get => _NulgathDemandsWork ??= new NulgathDemandsWork();
        set => _NulgathDemandsWork = value;
    }
    private static NulgathDemandsWork _NulgathDemandsWork;
    private static EnoughDOOMforanArchfiend DOOM
    {
        get => _DOOM ??= new EnoughDOOMforanArchfiend();
        set => _DOOM = value;
    }
    private static EnoughDOOMforanArchfiend _DOOM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(
            new[]
            {
                "ArchFiend DoomLord",
                "Undead Essence",
                "Chaorruption Essence",
                "Essence Potion",
                "Essence of Klunk",
                "Living Star Essence",
                "Bone Dust",
                "Undead Energy",
            }
        );
        Core.SetOptions();

        DOOM.AFDL();

        Core.SetOptions(false);
    }
}
