/*
name: Nulgath Demands Work
description: This bot will do the Nulgath Demands Work quest untill you have a uni35 and the equipment.
tags: archfiend, doomlord, ADFL, nulgath, demands, work, unidentified, uni, 35, essence, fragment
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class NulgathDemandsWork
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static GoldenHanzoVoid GHV
    {
        get => _GHV ??= new GoldenHanzoVoid();
        set => _GHV = value;
    }
    private static GoldenHanzoVoid _GHV;
    private static WillpowerExtraction WillpowerExtraction
    {
        get => _WillpowerExtraction ??= new WillpowerExtraction();
        set => _WillpowerExtraction = value;
    }
    private static WillpowerExtraction _WillpowerExtraction;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public string[] NDWItems =
    {
        "DoomLord's War Mask",
        "ShadowFiend Cloak",
        "Locks of the DoomLord",
        "Doomblade of Destruction",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(NDWItems);
        Core.BankingBlackList.AddRange(new[] { "Archfiend Essence Fragment", "Unidentified 35" });
        Core.SetOptions();

        DoNulgathDemandsWork();

        Core.SetOptions(false);
    }

    public void DoNulgathDemandsWork()
    {
        NDWQuest(NDWItems);
        NDWQuest(new[] { "Unidentified 35" }, 300);
    }

    /// <summary>
    /// Completes "Nulgath Demands Work" until the Desired Items are gotten.
    /// <param name="itemNames">Item names to farm</param>
    /// <param name="quant">Amount of the item [Mostly the Archfiend Ess and Uni 35]</param>
    /// </summary>
    public void NDWQuest(string[]? itemNames = null, int quant = 1)
    {
        itemNames ??= NDWItems.Select(item => item).ToArray();
        if (itemNames.Length == 0 || Core.CheckInventory(itemNames, quant))
            return;

        ItemBase[] Rewards = Core.EnsureLoad(5259).Rewards.ToArray();
        Quest? NDW = Core.InitializeWithRetries(() => Core.EnsureLoad(5259));
        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop(Rewards.Select(x => x.Name).ToArray());

        foreach (string itemName in itemNames)
        {
            ItemBase? item = Rewards.FirstOrDefault(x => x != null && x.Name == itemName);
            if (item == null)
            {
                Core.Logger($"Item '{itemName}' not found in quest rewards, skipping.");
                continue;
            }

            if (Core.CheckInventory(item.Name, quant))
            {
                Core.Logger($"{item.Name}, x[{quant}] owned, continuing.");
                continue;
            }

            Core.FarmingLogger(item.Name, quant);
            while (!Bot.ShouldExit && !Core.CheckInventory(item.Name, quant))
            {
                Core.EnsureAccept(5259);
                WillpowerExtraction.Unidentified34(10);
                Nation.FarmUni13(2);
                Nation.FarmBloodGem(2);
                Nation.FarmDiamondofNulgath(60);
                Nation.FarmDarkCrystalShard(45);
                Uni27();
                Nation.FarmGemofNulgath(15);
                Nation.FarmTaintedGem(50);
                GHV.GetGHV();
                Nation.FarmVoucher(true);

                if (item.Name == "Unidentified 35")
                {
                    // Buy U35 if fragments exist
                    while (
                        !Bot.ShouldExit
                        && Core.CheckInventory("Archfiend Essence Fragment", 9)
                        && !Core.CheckInventory("Unidentified 35", quant)
                    )
                    {
                        Adv.BuyItem("tercessuinotlim", 1951, item.ID, shopItemID: 7912);
                    }
                }

                // Pick ONE NDW reward that isn't maxed
                ItemBase? rewardToPick = NDW
                    ?.Rewards.Where(x =>
                        x != null
                        && NDWItems.Contains(x.Name)
                        && !Core.CheckInventory(x.Name, x.MaxStack)
                    )
                    .FirstOrDefault();

                if (rewardToPick != null)
                    Core.EnsureCompleteChoose(5259, new[] { rewardToPick.Name });
                else
                {
                    Core.Logger(
                        "All NDW items are at MaxStack. Completing quest without selecting a reward.\n"
                            + "(You will still receive Archfiend Essence Fragment and Unidentified 35.)"
                    );
                    Core.EnsureComplete(5259);
                }
            }
        }
    }

    public void Uni27()
    {
        if (Core.CheckInventory("Unidentified 27"))
            return;

        Core.AddDrop("Unidentified 27");
        Nation.Supplies("Unidentified 26", 1);
        Core.EnsureAccept(584);
        Core.ResetQuest(7551);
        Core.DarkMakaiItem("Dark Makai Sigil");
        Core.EnsureComplete(584);
        Bot.Wait.ForDrop("Unidentified 27");
        Bot.Wait.ForPickup("Unidentified 27");
        Core.Logger("Uni 27 acquired");
    }
}
