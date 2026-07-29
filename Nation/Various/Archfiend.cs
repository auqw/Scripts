/*
name: Archfiend
description: Gets and ranks the Archfiend Class
tags: archfiend, class, rankup, arch fiend, yo mama, archfiend class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
using Skua.Core.Interfaces;

public class ArchFiend
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
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreNSOD NSoD
    {
        get => _NSoD ??= new CoreNSOD();
        set => _NSoD = value;
    }
    private static CoreNSOD _NSoD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.SetOptions();

        GetArchfiend();

        Core.SetOptions(false);
    }

    public void GetArchfiend(bool rankUp = true)
    {
        if (Core.CheckInventory("ArchFiend"))
        {
            if (rankUp)
                Adv.RankUpClass("ArchFiend");
            return;
        }
        AbyssalContract();

        Core.BuyItem("tercessuinotlim", 695, 18894, shopItemID: 1925);
        if (rankUp)
            Adv.RankUpClass("ArchFiend");
    }

    public void AbyssalContract()
    {
        if (Core.CheckInventory("Abyssal Contract"))
            return;

        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop("Abyssal Contract");

        Farm.Experience(50);
        Core.EnsureAccept(8476);

        Nation.FarmUni13(3);

        Adv.BuyItem("tercessuinotlim", 1951, "Pink Star Diamond of Nulgath");

        Core.HuntMonster("mercutio", "Mercutio", "Immortal Joe's Black Star", 1, false);

        if (!Core.CheckInventory("Abyssal Star"))
        {
            Nation.FarmDarkCrystalShard(200);
            Nation.FarmTaintedGem(300);
            Nation.FarmGemofNulgath(200);
            Core.BuyItem("evilwarnul", 456, "Abyssal Star");
        }

        Adv.BuyItem("tercessuinotlim", 1951, "Gold Star of Avarice");

        if (!Core.CheckInventory("Blood Star of the Archfiend"))
        {
            Nation.FarmBloodGem(20);
            Nation.FarmTotemofNulgath(8);
            if (!Core.CheckInventory("Sepulchure's DoomKnight Armor"))
                NSoD.RetrieveVoidAuras(2);
            else
                NSoD.VoidAuras(2);
            Nation.ApprovalAndFavor(0, 999);
            Core.BuyItem("shadowblast", 1206, "Blood Star of the Archfiend");
        }
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("fiendshard", "Dirtlicker", "Dirtlicker Demoted", 1, false);

        Core.EnsureComplete(8476);
        Bot.Wait.ForPickup("Abyssal Contract");
    }
}
