/*
name: serpentine larvae set
description: gets pet, does quest, gets set
tags: serpentine larvae, set
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class SerpentineLarvae
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

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoQuest();

        Core.SetOptions(false);
    }

    public void DoQuest()
    {
        Core.HuntMonster(
            "darkalliance",
            "Shadowflame Nulgath",
            "Serpentine Larvae",
            isTemp: false,
            log: false
        );
        AutoReward(8944);
        Core.TrashCan("Tainted Soul", "Blade of Holy Might", "Infected Dragon Soul");
    }

    public void AutoReward(int questID = 0000, int quant = 1)
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(questID).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.ID);

        Core.EquipClass(ClassType.Solo);

        foreach (ItemBase item in RewardOptions)
        {
            if (!Core.CheckInventory(item.ID))
                Core.FarmingLogger(item.Name, 1);

            while (!Bot.ShouldExit && !Core.CheckInventory(item.ID, quant))
            {
                Core.EnsureAccept(questID);
                Core.HuntMonster(
                    Core.IsMember ? "nulgath" : "evilmarsh",
                    "Tainted Elemental",
                    "Tainted Soul",
                    5,
                    isTemp: false,
                    log: false
                );
                Core.HuntMonster(
                    "northlands",
                    "Aisha's Drake",
                    "Blade of Holy Might",
                    isTemp: false,
                    log: false
                );
                Nation.FarmDarkCrystalShard(10);
                Nation.FarmTaintedGem(15);
                Adv.BuyItem("evilwarnul", 456, "Oversoul Witch of Nulgath");
                Core.HuntMonster(
                    "dragonhame",
                    "Infected Dragon",
                    "Infected Dragon Soul",
                    5,
                    isTemp: false,
                    log: false
                );
                Core.EnsureComplete(questID, item.ID);
            }
        }
    }
}
