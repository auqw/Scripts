/*
name: HBDK - A Dark Knight Returns
description: does the 'a dark knight returns' part of hollowborn doomKnight
tags: hollowborn, a dark knight returns, hollowborn doomknight, hollowborn sword of doom, hollowborn doomknight character page badge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/HollowbornPaladin/CoreHollowbornPaladin.cs
//cs_include Scripts/Hollowborn/HollowbornDoomKnight/CoreHollowbornDoomKnight.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/AscendedDrakathGear.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/Artixpointe.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
using Skua.Core.Interfaces;

public class ADKReturns
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreHollowbornDoomKnight HDK { get => _HDK ??= new CoreHollowbornDoomKnight(); set => _HDK = value; }
    private static CoreHollowbornDoomKnight _HDK;
    public static CoreHollowbornDoomKnight sHDK
    {
        get => _sHDK ??= new CoreHollowbornDoomKnight();
        set => _sHDK = value;
    }
    public static CoreHollowbornDoomKnight _sHDK;


    public string OptionsStorage = sHDK.OptionsStorage;
    public bool DontPreconfigure = true;
    public List<IOption> Options = sHDK.Options;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        HDK.GetAll();

        Core.SetOptions(false);
    }
}
