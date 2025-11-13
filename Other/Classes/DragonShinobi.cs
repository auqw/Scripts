/*
name: DragonShinobi
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class DragonShinobi
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static YokaiQuests Yokai
    {
        get => _Yokai ??= new YokaiQuests();
        set => _Yokai = value;
    }
    private static YokaiQuests _Yokai;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetDSS();

        Core.SetOptions(false);
    }

    public void GetDSS(bool rankUpClass = true)
    {
        if (Core.CheckInventory(59476))
        {
            if (rankUpClass)
                Adv.RankUpClass("DragonSoul Shinobi");
            return;
        }

        Yokai.Quests();
        Core.AddDrop(20561);

        Core.EquipClass(ClassType.Solo);
        Core.FarmingLogger("Dragon Shinobi Token", 300);
        Core.RegisterQuests(7924);
        while (!Bot.ShouldExit && !Core.CheckInventory(20561, 300))
            Core.HuntMonster("shadowfortress", "1st Head of Orochi", log: false);
        Core.CancelRegisteredQuests();

        Adv.BuyItem("shadowfortress", 1968, 59476, shopItemID: 8078);

        if (rankUpClass)
            Adv.RankUpClass("DragonSoul Shinobi");
    }
}
