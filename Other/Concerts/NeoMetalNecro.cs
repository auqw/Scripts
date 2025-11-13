/*
name: Neo Metal Necro[class]
description: does the battle concert even quests for the classes, and buys the class.
tags: concert, metal necro, neo metal necro
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Other/Concerts/BattleConcert2023.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Evil/VordredsArmor.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs

using Skua.Core.Interfaces;

public class NeoMetalNecro
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
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
    private static BattleConcertClassQuests BCCQ
    {
        get => _BCCQ ??= new BattleConcertClassQuests();
        set => _BCCQ = value;
    }
    private static BattleConcertClassQuests _BCCQ;
    private static ArchDoomKnight ADK
    {
        get => _ADK ??= new ArchDoomKnight();
        set => _ADK = value;
    }
    private static ArchDoomKnight _ADK;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetClass();

        Core.SetOptions(false);
    }

    public void GetClass(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Neo Metal Necro"))
        {
            if (rankUpClass)
                Adv.RankUpClass("Neo Metal Necro");
            return;
        }

        BCCQ.BattleConcertQuests();

        // Revive the Encore 9327
        Core.AddDrop("Bone Pick");
        Core.RegisterQuests(9327);
        while (!Bot.ShouldExit && !Core.CheckInventory("Bone Pick", 10))
            Core.HuntMonster("brainmeat", "Brain Matter");

        Adv.BuyItem("skulldome", 2312, 78967, shopItemID: 11974);

        if (rankUpClass)
            Adv.RankUpClass("Neo Metal Necro");
    }
}
