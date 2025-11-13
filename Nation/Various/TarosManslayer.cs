/*
name: TarosManslayer
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
using Skua.Core.Interfaces;

public class TarosManslayer
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
    private static PurifiedClaymoreOfDestiny PCoD
    {
        get => _PCoD ??= new PurifiedClaymoreOfDestiny();
        set => _PCoD = value;
    }
    private static PurifiedClaymoreOfDestiny _PCoD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GuardianTaro(!Core.IsMember);

        Core.SetOptions(false);
    }

    private string[] Rewards = { "Taro's Manslayer", "Taro Blademaster Guardian" };

    public void GuardianTaro(bool ManslayerOnly = true)
    {
        if (ManslayerOnly)
        {
            Rewards = new[] { "Taro's Manslayer" };
        }

        if (Core.CheckInventory(Rewards))
            return;

        Core.AddDrop(Rewards);

        if (ManslayerOnly)
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "tercessuinotlim",
                "Taro Blademaster",
                "Taro's Manslayer",
                isTemp: false
            );
        }
        else
        {
            if (!Bot.Player.IsMember)
            {
                Core.Logger(
                    "Membership REQUIRED for this quest ( \"The Guardian Taro Blademaster\")"
                );
                return;
            }

            Farm.GoodREP();
            PCoD.GetPCoD();

            while (!Bot.ShouldExit && !Core.CheckInventory(Rewards))
            {
                Core.EnsureAccept(1111);
                Nation.FarmGemofNulgath(10);
                Core.ResetQuest(7551);
                Core.DarkMakaiItem("Dark Makai Rune");
                Core.EnsureCompleteChoose(1111, Rewards);
                Core.Sleep();
            }
        }
    }
}
