/*
name: LightCaster
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Other/Classes/LightMage.cs
//cs_include Scripts/Other/Weapons/AvatarOfDeathsScythe.cs
//cs_include Scripts/Other/Weapons/GuardianOfSpiritsBlade.cs
//cs_include Scripts/Other/Weapons/LanceOfTime.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
using Skua.Core.Interfaces;

public class LightCaster
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
    private static LightMage LM
    {
        get => _LM ??= new LightMage();
        set => _LM = value;
    }
    private static LightMage _LM;
    private static AvatarOfDeathsScythe AODS
    {
        get => _AODS ??= new AvatarOfDeathsScythe();
        set => _AODS = value;
    }
    private static AvatarOfDeathsScythe _AODS;
    private static GuardianOfSpiritsBlade GOSB
    {
        get => _GOSB ??= new GuardianOfSpiritsBlade();
        set => _GOSB = value;
    }
    private static GuardianOfSpiritsBlade _GOSB;
    private static LanceOfTime LOT
    {
        get => _LOT ??= new LanceOfTime();
        set => _LOT = value;
    }
    private static LanceOfTime _LOT;
    private static BurningBlade BB
    {
        get => _BB ??= new BurningBlade();
        set => _BB = value;
    }
    private static BurningBlade _BB;
    private static BurningBladeOfAbezeth BBOA
    {
        get => _BBOA ??= new BurningBladeOfAbezeth();
        set => _BBOA = value;
    }
    private static BurningBladeOfAbezeth _BBOA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetLC();

        Core.SetOptions(false);
    }

    public void GetLC(bool rankUpClass = true)
    {
        if (Core.CheckInventory("LightCaster"))
        {
            if (rankUpClass && !Core.CheckInventory("LightCaster", 10))
                Adv.RankUpClass("LightCaster");
            return; // Already have the class, no need to
        }

        Core.AddDrop("LightCaster", "Aranx's Pure Light");

        Farm.Experience(80);
        GOSB.GetGoSB();
        AODS.GetAoDS();
        LOT.GetLoT();
        BB.GetBurningBlade();
        LM.GetLM(false);
        BBOA.GetBBoA();

        Core.EquipClass(ClassType.Solo);
        Core.EnsureAccept(6495);
        Core.HuntMonster("celestialarenad", "Aranx", "Aranx's Pure Light", isTemp: false);
        Core.EnsureComplete(6495);
        Bot.Wait.ForPickup("LightCaster");

        if (rankUpClass)
            Adv.RankUpClass("LightCaster");
    }
}
