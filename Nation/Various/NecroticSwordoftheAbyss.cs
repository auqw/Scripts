/*
name: Necrotic Sword of the Abyss
description: This will farm the required materials and buy the item.
tags: necrotic sword of the abyss, nation, boosted item, nsoa, nsod
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
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
using Skua.Core.Interfaces;

public class NSoA
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreVHL VHL
    {
        get => _VHL ??= new CoreVHL();
        set => _VHL = value;
    }
    private static CoreVHL _VHL;
    private static CoreNSOD NSOD
    {
        get => _NSOD ??= new CoreNSOD();
        set => _NSOD = value;
    }
    private static CoreNSOD _NSOD;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.SetOptions();

        GetNSoA();

        Core.SetOptions(false);
    }

    public void GetNSoA()
    {
        if (Core.CheckInventory("Necrotic Sword of the Abyss"))
            return;

        NSOD.GetNSOD();

        VHL.VHLChallenge(2);

        VHL.VHLCrystals();

        Adv.BuyItem("tercessuinotlim", 1355, "Necrotic Sword of the Abyss");
    }
}
