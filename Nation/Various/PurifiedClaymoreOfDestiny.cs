/*
name: PurifiedClaymoreOfDestiny
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class PurifiedClaymoreOfDestiny
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetPCoD();

        Core.SetOptions(false);
    }

    public void GetPCoD()
    {
        if (Core.CheckInventory("Purified Claymore of Destiny"))
            return;

        Core.AddDrop("Purified Claymore of Destiny");

        Farm.Experience(15);
        Farm.GoodREP(8);

        Core.EnsureAccept(548);
        Core.HuntMonster(
            "battleundera",
            "Undead Berserker",
            "Warrior Claymore Blade",
            isTemp: false
        );
        Core.EnsureComplete(548);
    }
}
