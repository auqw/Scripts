/*
name: Fiend's Purgatory
description: Ensures the acceptance requirements are owned, then completes 'Fiend's Purgatory' quest [10790] for the selected reward(s).
tags: hollowborn, hollowborn nation, fiends purgatory, void soul
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class FiendsPurgatory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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

    private const int QuestID = 10790;

    private readonly string[] RequiredItems =
    {
        "DragonFire of Nulgath",
        "Crimson Plate of Nulgath",
        "Crimson Face Plate of Nulgath",
        "Ungodly Reavers of Nulgath",
        "Crystal Phoenix Blade of Nulgath",
        "Overfiend Blade of Nulgath",
        "Dark Makai of Nulgath",
    };

    private readonly string[] SelectableRewards =
    {
        "Hollowborn Ungodly Reavers of Nulgath",
        "Hollowborn Phoenix Blade of Nulgath",
        "Hollowborn Overfiend Blade of Nulgath",
        "Hollowborn DragonBlade of Nulgath",
    };

    private readonly string[] RandomRewards =
    {
        "Hollowborn Makai Pet",
        "Hollowborn Makai Battle Pet",
        "Hollowborn Hooded Makai Pet",
    };

    public bool DontPreconfigure = true;
    public string OptionsStorage = "FiendsPurgatory";
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<Rewards>(
            "RewardSelect",
            "Choose Your Reward",
            "Which selectable reward to farm from 'Fiend's Purgatory'. Choose All to farm every selectable reward.",
            Rewards.All
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(RequiredItems);
        Core.SetOptions();

        Purgatory(Bot.Config!.Get<Rewards>("RewardSelect"));

        Core.SetOptions(false);
    }

    public void Purgatory(Rewards reward = Rewards.All)
    {
        string[] chosenReward =
            reward == Rewards.All
                ? SelectableRewards
                : new[] { reward.ToString().Replace('_', ' ') };

        if (Core.CheckInventory(chosenReward))
            return;

        EnsureRequirements();

        Core.AddDrop(chosenReward);
        Core.AddDrop(RandomRewards);
        Core.AddDrop("Void Soul");

        Core.Logger(
            $"Reward Chosen: {(reward == Rewards.All ? "All" : reward.ToString().Replace('_', ' '))}"
        );

        while (!Bot.ShouldExit && !Core.CheckInventory(chosenReward))
        {
            Core.EnsureAccept(QuestID);

            Core.HuntMonster("deleuzetundra", "Blighted Zubami", "Blighted Zubami Badge", isTemp: false);
            Core.HuntMonster("voidsalek", "Salek Sprayer", "Spoil of Salek", isTemp: false);
            Core.HuntMonster("voidnerfkitten", "Sarah the Nerfkitten", "Sarah's Souvenir", isTemp: false);

            Core.EnsureCompleteChoose(QuestID, chosenReward);
        }
    }

    public void EnsureRequirements()
    {
        JugItem("Ungodly Reavers of Nulgath", JuggernautItemsofNulgath.RewardsSelection.Ungodly_Reavers_of_Nulgath);
        JugItem("Crystal Phoenix Blade of Nulgath", JuggernautItemsofNulgath.RewardsSelection.Crystal_Phoenix_Blade_of_Nulgath);
        JugItem("Overfiend Blade of Nulgath", JuggernautItemsofNulgath.RewardsSelection.Overfiend_Blade_of_Nulgath);
        JugItem("Dark Makai of Nulgath", JuggernautItemsofNulgath.RewardsSelection.Dark_Makaiof_Nulgath);

        TwistedItem("DragonFire of Nulgath", 1316);
        TwistedItem("Crimson Plate of Nulgath", 4695);
        TwistedItem("Crimson Face Plate of Nulgath", 4961);
    }

    private void JugItem(string itemName, JuggernautItemsofNulgath.RewardsSelection selection)
    {
        if (Core.CheckInventory(itemName))
            return;

        Core.Logger($"Missing acceptance requirement \"{itemName}\", farming it.");
        Jug.JuggItems(selection);
    }

    private void TwistedItem(string itemName, int itemID)
    {
        if (Core.CheckInventory(itemName))
            return;

        Core.Logger($"Missing acceptance requirement \"{itemName}\", farming it.");
        Core.EquipClass(ClassType.Farm);
        while (!Bot.ShouldExit && !Core.CheckInventory(itemName))
        {
            Core.EnsureAccept(765);
            Nation.FarmTotemofNulgath(3);
            Core.HuntMonster("underworld", "Skull Warrior", "Skull Warrior Rune");
            Core.EnsureComplete(765, itemID);
        }
    }

    public enum Rewards
    {
        All,
        Hollowborn_Ungodly_Reavers_of_Nulgath,
        Hollowborn_Phoenix_Blade_of_Nulgath,
        Hollowborn_Overfiend_Blade_of_Nulgath,
        Hollowborn_DragonBlade_of_Nulgath,
    }
}
