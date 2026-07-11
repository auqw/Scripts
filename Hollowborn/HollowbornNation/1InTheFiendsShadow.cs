/*
name: In The Fiend's Shadow
description: Completes the 'In The Fiend's Shadow' quest [10789] for the selected reward(s), drops Void Soul.
tags: hollowborn, hollowborn nation, in the fiends shadow, void soul
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class InTheFiendsShadow
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static HollowSoul HSoul
    {
        get => _HSoul ??= new HollowSoul();
        set => _HSoul = value;
    }
    private static HollowSoul _HSoul;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "InTheFiendsShadow";
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<Rewards>(
            "RewardSelect",
            "Choose Your Reward",
            "Which reward to farm from 'In The Fiend's Shadow'. Choose All to farm every reward.",
            Rewards.All
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FiendsShadow(Bot.Config!.Get<Rewards>("RewardSelect"));

        Core.SetOptions(false);
    }

    public void FiendsShadow(Rewards reward = Rewards.All)
    {
        string[] chosenReward =
            reward == Rewards.All
                ? Core.QuestRewards(QuestID)
                : new[] { reward.ToString().Replace('_', ' ') };

        if (Core.CheckInventory(chosenReward))
            return;

        Core.AddDrop(chosenReward);
        Core.AddDrop("Void Soul");

        Core.Logger(
            $"Reward Chosen: {(reward == Rewards.All ? "All" : reward.ToString().Replace('_', ' '))}"
        );

        while (!Bot.ShouldExit && !Core.CheckInventory(chosenReward))
        {
            Core.EnsureAccept(QuestID);

            Core.HuntMonster("lair", "Red Dragon", "Phoenix Blade", isTemp: false);
            Core.HuntMonster("stormtemple", "Chaos Lord Lionfang", "Chaotic Tentacles", isTemp: false);
            Nation.FarmTotemofNulgath(1);
            HSoul.GetYaSoulsHeeeere(50);

            Core.EnsureCompleteChoose(QuestID, chosenReward);
        }
    }

    private const int QuestID = 10789;

    public enum Rewards
    {
        All,
        Hollowborn_Void_Sword,
        Hollowborn_Soulreaper_of_Nulgath,
        Hollowborn_Void_of_Nulgath,
        Hollowborn_Void_Horns,
        Hollowborn_Void_Helm,
    }
}
