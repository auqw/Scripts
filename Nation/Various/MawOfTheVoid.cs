/*
name: Maw of the Void
description: This script will farm Maw of the Void quest items.
tags: fiend's creed blade,fiend's creed blades,maw,nulgath gifts
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiegeMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class MawOfTheVoid
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
    private static JuggernautItemsofNulgath Jug
    {
        get => _Jug ??= new JuggernautItemsofNulgath();
        set => _Jug = value;
    }
    private static JuggernautItemsofNulgath _Jug;
    private static TempleSiegeMerge TSM
    {
        get => _TSM ??= new TempleSiegeMerge();
        set => _TSM = value;
    }
    private static TempleSiegeMerge _TSM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAll();

        Core.SetOptions(false);
    }

    public void GetAll()
    {
        if (Core.CheckInventory(Core.QuestRewards(10020)))
            return;

        if (!Bot.Quests.HasBeenCompleted(9541))
            Core.ChainComplete(9541);

        Farm.EvilREP(9);

        Core.AddDrop(Core.EnsureLoad(10020).Requirements.Select(req => req.Name).ToArray());

        List<ItemBase> RewardOptions = Core.EnsureLoad(10020).Rewards;

        foreach (ItemBase item in RewardOptions)
            Core.AddDrop(item.Name);

        Core.EquipClass(ClassType.Farm);

        foreach (ItemBase Reward in RewardOptions)
        {
            if (Core.CheckInventory(Reward.Name, toInv: false))
                return;

            Core.FarmingLogger(Reward.Name, 1);

            Core.EnsureAccept(10020);

            // Tainted Gem
            Nation.FarmTaintedGem(175);

            // Essence of Nulgath
            Nation.EssenceofNulgath(40);

            // Dark Makai of Nulgath
            Jug.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Dark_Makaiof_Nulgath);

            // Adventus Wolf
            Core.HuntMonster("legioncrypt", "Brutus", "Adventus Wolf", isTemp: false);

            // Hot Mama
            Core.HuntMonster("battleundere", "Hot Mama", "Hot Mama", isTemp: false);

            // Borgar
            if (!Core.CheckInventory("Borgar"))
            {
                bool LoggedBefore = false;
                while (!Bot.ShouldExit && !Core.CheckInventory("Burger Buns", 5))
                {
                    // Burglinster's Revenge 7522
                    Core.EnsureAccept(7522);
                    Core.HuntMonster(
                        "borgars",
                        "Burglinster",
                        "Burglinster Cured",
                        log: !LoggedBefore
                    );
                    Core.EnsureComplete(7522);
                    Bot.Wait.ForPickup("Burger Buns");
                    LoggedBefore = true;
                }
            }
            Core.BuyItem("borgars", 1884, 54650, shopItemID: 7387);

            // Abyssal Makai
            TSM.BuyAllMerge("Abyssal Makai");

            Core.EnsureComplete(10020, Reward.ID);
            Core.JumpWait();
            Core.ToBank(Reward.Name);
        }
    }
}
