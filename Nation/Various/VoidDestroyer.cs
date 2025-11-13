/*
name: VoidDestroyer
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;

public class VoidDestroyer
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

    public readonly string[] Rewards =
    {
        "Void Destroyer",
        "Void Destruction Blade",
        "Void Spear of War",
        "Horned Void War Helm",
        "Crested Void War Helm",
        "Wrap of the Void",
        "Tainted Destruction Blade",
        "Toxic Void Katana",
        "Dual Toxic Void Katanas",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetDestroyer();

        Core.SetOptions(false);
    }

    public void GetDestroyer()
    {
        if (Core.CheckInventory(Rewards))
        {
            Core.Logger($"All rewards already owned");
            return;
        }

        // Track only relevant rewards from quest 5661
        Bot.Drops.Add(
            Core.EnsureLoad(5661)
                .Rewards.Where(x => x != null && Rewards.Contains(x.Name))
                .Select(x => x.Name)
                .ToArray()
        );

        // Loop until all desired rewards are in inventory
        while (!Bot.ShouldExit && !Core.CheckInventory(Rewards, toInv: false))
        {
            Core.EnsureAccept(5661);
            Nation.Supplies("Unidentified 4");
            Nation.FarmTaintedGem(1);
            Nation.FarmDarkCrystalShard(1);
            Nation.EssenceofNulgath(1);
            Nation.FarmGemofNulgath(1);

            Core.EnsureComplete(5661);

            // Wait for any desired drops that appeared
            foreach (string reward in Rewards)
            {
                if (Bot.Drops.CurrentDrops.Contains(reward))
                    Bot.Wait.ForPickup(reward);
            }
        }
        Core.Logger(
            $"All rewards collected, banking them;\n"
                + $" {string.Join(", ", Bot.Drops.CurrentDrops)}"
        );
        Core.ToBank(Rewards);
    }
}
