/*
name: ShadowslayerSummoningRitual2
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/GiantTaleStory.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Story/ShadowSlayerK.cs
//cs_include Scripts/Other/Various/ShadowslayerSummoningRitual.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ShadowslayerSummoningRitual2
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static ShadowSlayerK ShadowStory
    {
        get => _ShadowStory ??= new ShadowSlayerK();
        set => _ShadowStory = value;
    }
    private static ShadowSlayerK _ShadowStory;
    private static Core7DD DD
    {
        get => _DD ??= new Core7DD();
        set => _DD = value;
    }
    private static Core7DD _DD;
    private static BuyScrolls Scroll
    {
        get => _Scroll ??= new BuyScrolls();
        set => _Scroll = value;
    }
    private static BuyScrolls _Scroll;
    private static ShadowslayerSummoningRitual SSR
    {
        get => _SSR ??= new ShadowslayerSummoningRitual();
        set => _SSR = value;
    }
    private static ShadowslayerSummoningRitual _SSR;
    private static ShadowSlayerK SSK
    {
        get => _SSK ??= new ShadowSlayerK();
        set => _SSK = value;
    }
    private static ShadowSlayerK _SSK;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        LunateSigil();

        Core.SetOptions(false);
    }

    public void LunateSigil(int quant = 30)
    {
        if (Core.CheckInventory("Lunate Sigil", quant))
            return;

        if (!Core.isCompletedBefore(9845))
            SSK.DoAll();

        //Get "Sparkly Shadowslayer Relic"
        SSR.GetAll(true);

        Core.AddDrop("Lunate Sigil");
        Core.FarmingLogger("Lunate Sigil", quant);

        while (!Core.CheckInventory("Lunate Sigil", quant))
        {
            Core.HuntMonsterQuest(
                9846,
                ("chaoscave", "Dracowerepyre", ClassType.Solo),
                ("darkoviaforest", "Lich Of The Stone", ClassType.Solo),
                ("borgars", "Burglinster", ClassType.Solo),
                ("firewar", "Uriax", ClassType.Solo),
                ("maul", "Creature Creation ", ClassType.Solo),
                ("techdungeon", "Kalron the Cryptborg", ClassType.Solo)
            );
            Bot.Wait.ForPickup("Lunate Sigil");
        }
    }
}
