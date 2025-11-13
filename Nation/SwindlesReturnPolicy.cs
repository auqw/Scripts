/*
name: SwindlesReturnPolicy
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class SwindlesReturnPolicy
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

    public string OptionsStorage = "SwindlesReturnPolicy";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<RewardsSelection>(
            "RewardSelect",
            "Choose Your Quest Reward",
            "Select Your Quest Reward for Swindle's Return Policy.",
            RewardsSelection.All
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.Receipt);
        Core.SetOptions();

        DoSwindlesReturnPolicy(Bot.Config!.Get<RewardsSelection>("RewardSelect"));

        Core.SetOptions(false);
    }

    public void DoSwindlesReturnPolicy(RewardsSelection? reward = null, bool getAll = false)
    {
        Core.Logger(
            $"Reward set to {(reward.HasValue ? reward.Value.ToString().Replace("_", " ") : null)}"
        );

        if (reward == RewardsSelection.All || reward == null)
            getAll = true;

        if (!getAll)
        {
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(7551));
            if (quest == null)
            {
                Core.Logger("Quest 7551 not found.");
                return;
            }
            ItemBase? item = quest.Rewards.FirstOrDefault(x =>
                x.Name == reward!.Value.ToString().Replace("_", " ")
            );
            if (item == null)
            {
                Core.Logger(
                    $"Reward with name {reward!.Value.ToString().Replace("_", " ")} not found in Quest Rewards."
                );
                return;
            }
            Core.Logger($"Item Selected: {item.Name}[{item.ID}]");
            Nation.SwindleReturn(item.Name, item.MaxStack);
        }
        else
        {
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(7551));
            if (quest == null)
            {
                Core.Logger("Quest 7551 not found.");
                return;
            }

            Core.Logger(
                "Maxing All Rewards from Swindles return:\n"
                    + string.Join(
                        '\n',
                        quest.Rewards.Select(r =>
                        {
                            int current =
                                Bot.TempInv.GetQuantity(r.Name) + Bot.Inventory.GetQuantity(r.Name);
                            int target = r.MaxStack;
                            return $"Farming {r.Name} ({current}/{target})";
                        })
                    )
            );

            foreach (ItemBase thing in Core.EnsureLoad(7551).Rewards)
            {
                if (Core.CheckInventory(thing.Name, thing.MaxStack))
                    continue;

                Nation.SwindleReturn(thing.Name, thing.MaxStack);
            }
        }

        Core.CancelRegisteredQuests();
    }

    public enum RewardsSelection
    {
        Dark_Crystal_Shard = 4770,
        Diamond_of_Nulgath = 4771,
        Gem_of_Nulgath = 6136,
        Blood_Gem_of_the_Archfiend = 22332,
        Tainted_Gem = 4769,
        All = 0,
    }
}
