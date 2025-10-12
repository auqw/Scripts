/*
name: TarosPrismaticManslayers
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using System.Linq;
public class TarosPrismaticManslayers
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreNation Nation { get => _Nation ??= new CoreNation(); set => _Nation = value; }
    private static CoreNation _Nation;
    private static TarosManslayer Taro { get => _Taro ??= new TarosManslayer(); set => _Taro = value; }
    private static TarosManslayer _Taro;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        TemptationTest();

        Core.SetOptions(false);
    }

    private string[] Rewards = { "Taro's Prismatic Manslayer", "Taro's Dual Prismatic Manslayers", "Taro's BattleBlade" };

    public void TemptationTest()
    {
        Quest? Q = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(8496));
        if (Q == null)
        {
            Core.Logger("Failed to load the quest `A Test of Temptation`");
            return;
        }

        if (Core.CheckInventory(Q.Rewards.Select(x => x.ID).ToArray()) || !Core.IsMember)
        {
            Core.Logger(!Core.IsMember ? "Membership required for the quest `A Test of Temptation`" : "Rewards already in inventory");
            return;
        }

        Core.AddDrop(Q.Rewards.Select(x => x.ID).ToArray());

        Farm.Experience(80);
        Farm.GoodREP();

        // Aquire the accept requirement
        if (!Core.CheckInventory("Taro's Manslayer"))
            Taro.GuardianTaro();

        // Reverse the array so the battlepet is last
        foreach (ItemBase reward in System.Linq.Enumerable.Reverse(Q.Rewards.ToArray()))
        {
            if (Core.CheckInventory(reward.Name, toInv: false))
            {
                Core.Logger($"{reward.Name} obtained.");
                continue;
            }
            else
            {
                Core.AddDrop(reward.ID);
                Core.FarmingLogger(reward.Name, 1);
            }

            Core.EnsureAccept(8496);
            Nation.FarmTaintedGem(200);
            Nation.FarmDarkCrystalShard(125);
            Nation.FarmDiamondofNulgath(300);
            Nation.FarmGemofNulgath(75);
            Nation.FarmBloodGem(35);
            Core.EnsureComplete(8496, reward.ID);
        }
    }
}
