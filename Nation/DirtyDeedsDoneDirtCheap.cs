/*
name: DirtyDeedsDoneDirtCheap
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Options;
using Skua.Core.Interfaces;

public class DirtyDeedsDoneDirtCheap
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public string OptionsStorage = "DDDDC_SwindlesRipoffEmporiumItem";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "DoSRoE",
            "Do SRoE",
            "Once \"Unidentified 10\" are maxed, buy items from \"Swindle's Ripoff Emporium\"",
            false
        ),
        new Option<Reward>(
            "RewardChoice",
            "SRoE Item",
            "Item to buy from \"Swindle's Ripoff Emporium\" (choose All to max all of {\"Blood Gem of the Archfiend\", \"Dark Crystal Shard\", \"Gem of Nulgath\", \"Tainted Gem\"})",
            Reward.All
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Reward chosenReward = bot.Config!.Get<Reward>("RewardChoice");
        string[] itemsToBuy = GetSRoEItemNames(chosenReward);

        Nation.DirtyDeedsDoneDirtCheap(
            SRoE: bot.Config.Get<bool>("DoSRoE"),
            SRoEItems: itemsToBuy // now it can accept multiple
        );



        Core.SetOptions(false);
    }

    public string[] GetSRoEItemNames(Reward reward)
    {
        return reward switch
        {
            Reward.BloodGemOfTheArchfiend => new[] { "Blood Gem of the Archfiend" },
            Reward.DarkCrystalShard => new[] { "Dark Crystal Shard" },
            Reward.GemOfNulgath => new[] { "Gem of Nulgath" },
            Reward.TaintedGem => new[] { "Tainted Gem" },
            Reward.All => new[]
            {
            "Blood Gem of the Archfiend",
            "Dark Crystal Shard",
            "Gem of Nulgath",
            "Tainted Gem"
        },
            _ => throw new ArgumentOutOfRangeException(nameof(reward), reward, null)
        };
    }


    public enum Reward
    {
        All = 0, // special option
        BloodGemOfTheArchfiend = 1,
        DarkCrystalShard = 2,
        GemOfNulgath = 3,
        TaintedGem = 4
    }


}
