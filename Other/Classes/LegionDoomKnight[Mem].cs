/*
name: LegionDoomKnight[Mem]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
using Skua.Core.Interfaces;

public class LegionDoomKnight
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static CoreSDKA SDKA
    {
        get => _SDKA ??= new CoreSDKA();
        set => _SDKA = value;
    }
    private static CoreSDKA _SDKA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetLDK();

        Core.SetOptions(false);
    }

    public void GetLDK(bool rankUpClass = true)
    {
        if (Core.CheckInventory(38742))
            return;

        if (!Core.CheckInventory("Sepulchure's DoomKnight Armor"))
            SDKA.DoAll();

        Core.RegisterQuests(4099);
        Core.HuntMonster(
            "sepulchure",
            "Dark Sepulchure",
            "Dark Sepulchure's Badge",
            100,
            isTemp: false
        );
        Core.CancelRegisteredQuests();

        Core.BuyItem("battleon", 1106, 38742);

        if (rankUpClass)
            Adv.RankUpClass("Legion DoomKnight");
    }
}
