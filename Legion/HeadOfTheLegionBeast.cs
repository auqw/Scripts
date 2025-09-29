/*
name: Head of the Legion Beast
description: This bot will farm the entire Head of the Legion Beast, including stories and bosses.
tags: head, legion, beast, LOTLB, seven, circles, war, penance, essence, wrath, violence, treachery, soul, heresy, indulgence, beast, helm, violence, wrath, greed, gluttony, luxuria
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class HeadoftheLegionBeast
{
    public string OptionsStorage = "hotlb"; //<--rename this
    public bool DontPreconfigure = true; //<- Leave this alone.
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<bool>("badge", "Farm Badge", "Set to true to farm the Head of the Legion Beast char page badge", false)
    };
    public string[] HeadLegionBeast =
    {
        "Penance",
        "Essence of Wrath",
        "Essence of Violence",
        "Essence of Treachery",
        "Souls of Heresy",
        "Indulgence",
        "Beast Soul",
        "Helms of the Seven Circles",
        "Faces of Violence",
        "Crown of Wrath",
        "Stare of Greed",
        "Gluttony's Maw",
        "Aspect of Luxuria",
        "Face of Treachery"
    };

    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }
    private static CoreLegion _Legion;
    private static SevenCircles Circles { get => _Circles ??= new SevenCircles(); set => _Circles = value; }
    private static SevenCircles _Circles;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(HeadLegionBeast);
        Core.SetOptions();

        LegionBeastHead();

        Core.SetOptions(false);
    }

    public void LegionBeastHead(bool badge = false)
    {
        if (Core.CheckInventory("Head of the Legion Beast") && (!badge || Core.HasWebBadge("Head of the Legion Beast")))
            return;

        Circles.CirclesWar();
        Core.AddDrop(HeadLegionBeast);

        HelmSevenCircles();
        Penance(30);
        Indulgence(30);
        Legion.FarmLegionToken(15000);

        Core.EquipClass(ClassType.Solo);
        //Adv.BestGear(RacialGearBoost.Undead);
        Core.KillMonster("sevencircleswar", "r17", "Left", "The Beast", "Beast Soul", 15, isTemp: false, publicRoom: true, log: false);

        Adv.BuyItem("sevencircleswar", 1984, "Head of the Legion Beast");

        if (badge)
        {
            // Head of the Legion Beast (8082)
            Core.Unbank("Head of the Legion Beast");
            Core.EnsureAccept(8082);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("sevencircleswar", "The Beast", "Beast Slain");
            Core.EnsureComplete(8082);
        }
    }

    public void HelmSevenCircles()
    {
        if (Core.CheckInventory(60137))
            return;

        Core.AddDrop(HeadLegionBeast);

        CircleHelm("Aspect of Luxuria");
        CircleHelm("Gluttony's Maw");
        CircleHelm("Stare of Greed");
        CircleHelm("Crown of Wrath", true);
        CircleHelm("Face of Treachery", true);
        CircleHelm("Faces of Violence", true);

        Adv.BuyItem("sevencircleswar", 1984, "Helms of the Seven Circles");


    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Wrath" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Wrath" items to collect. Default is 300.</param>
    public void EssenceWrath(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Wrath", quant))
        {
            Core.FarmingLogger("Essence of Wrath", quant);
            return;
        }

        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Wrath", quant);

        Core.RegisterQuests(7979);
        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Wrath", quant))
        {
            Core.KillMonster("sevencircleswar", "Enter", "Spawn", "*", log: false);
            Core.Sleep(500); // Add a short delay to ensure the item is collected
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farms the specified quantity of "Circle Helm" items.
    /// </summary>
    /// <param name="helm">The name of the helm to be farmed.</param>
    /// <param name="war">Whether to farm in the "sevencircleswar" zone. Default is false.</param>
    public void CircleHelm(string helm, bool war = false)
    {
        if (Core.CheckInventory(helm))
            return;

        Core.FarmingLogger(helm, 1);
        Legion.FarmLegionToken(1500);

        if (war)
        {
            Penance(10);
            Adv.BuyItem("sevencircleswar", 1984, helm);
        }
        else
        {
            Indulgence(10);
            Adv.BuyItem("sevencircles", 1980, helm);
        }
    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Violence" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Violence" items to collect. Default is 300.</param>
    public void EssenceViolence(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Violence", quant))
        {
            Core.FarmingLogger("Essence of Violence", quant);
            return;
        }

        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Violence", quant);

        Core.RegisterQuests(7985);
        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Violence", quant))
        {
            Core.KillMonster("sevencircleswar", "r9", "Left", "Violence Guard", log: false);
            Core.Sleep(500); // Add a short delay to ensure the item is collected
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Treachery" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Treachery" items to collect. Default is 300.</param>
    public void EssenceTreachery(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Treachery", quant))
        {
            Core.FarmingLogger("Essence of Treachery", quant);
            return;
        }

        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Treachery", quant);

        Core.RegisterQuests(7988);
        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Treachery", quant))
        {
            Core.KillMonster("sevencircleswar", "r13", "Left", "Treachery Guard", log: false);
            Core.Sleep(500); // Add a short delay to ensure the item is collected
        }
        Core.CancelRegisteredQuests();
    }

    /// <summary>
    /// Farms the specified quantity of "Souls of Heresy" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Souls of Heresy" items to collect. Default is 300.</param>
    public void SoulsHeresy(int quant = 300)
    {
        if (Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.FarmingLogger("Souls of Heresy", quant);
            return;
        }

        Core.AddDrop(HeadLegionBeast);
        if (!Bot.Quests.IsUnlocked(7983))
            Circles.CirclesWar(true);
        Core.FarmingLogger("Souls of Heresy", quant);
        Core.RegisterQuests(7983, 7980, 7981); // Blasphemy? Blasphe-you! ID:7983 | War Medals ID:7980 | Mega War Medals ID:7981
        while (!Bot.ShouldExit && !Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.KillMonster("sevencircleswar", "r7", "Left", "*", log: false);
            Bot.Wait.ForDrop("Souls of Heresy");
        }
        Core.CancelRegisteredQuests();
    }


    public void Penance(int quant = 300)
    {
        if (Core.CheckInventory("Penance", quant))
        {
            Core.FarmingLogger("Penance", quant);
            return;
        }

        Core.AddDrop(HeadLegionBeast);
        Core.FarmingLogger("Penance", quant);
        Core.EquipClass(ClassType.Farm);

        while (!Bot.ShouldExit && !Core.CheckInventory("Penance", quant))
        {
            // Farm required materials
            var requiredItems = new Dictionary<string, Action<int>>
            {
                { "Essence of Wrath", EssenceWrath },
                { "Essence of Violence", EssenceViolence },
                { "Essence of Treachery", EssenceTreachery },
                { "Souls of Heresy", amount => SoulsHeresy(Math.Min(300, amount * 15)) }
            };

            foreach (var (item, farmAction) in requiredItems)
                farmAction(Math.Min(quant, 300));


            // Check if SohCount (quant * 15) is greater then 300 (max stack of souls of heresy), if so then do sohcount / 15 (300 [soh max stack] / 15 = 20), else do quant
            Core.BuyItem("sevencircleswar", 1984, "Penance", quant * 15 > 300 ? Bot.Inventory.GetQuantity("Souls of Heresy") / 15 : quant);
            Bot.Wait.ForPickup("Penance");
            if (Core.CheckInventory("Penance", quant))
                break;

            Core.Sleep(1000);
        }
    }

    /// <summary>
    /// Farms the specified quantity of "Indulgence" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Indulgence" items to collect. Default is 100.</param>
    public void Indulgence(int quant = 100)
    {
        if (Core.CheckInventory("Indulgence", quant))
            return;

        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Indulgence", quant);

        int currentQuantity = Bot.Inventory.GetQuantity("Indulgence");
        int deficit = quant - currentQuantity;

        int soulsTarget = deficit >= 3 ? 75 : deficit == 2 ? 50 : 25;
        int essenceTarget = deficit >= 3 ? 3 : deficit;
        while (!Bot.ShouldExit && !Core.CheckInventory("Indulgence", quant))
        {
            Core.EnsureAccept(7978);
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("sevencircles", "r2", "Left", "Limbo Guard", "Souls of Limbo", soulsTarget, log: false);
            Core.EquipClass(ClassType.Solo);
            Core.KillMonster("sevencircles", "r4", "Left", "Luxuria", "Essence of Luxuria", essenceTarget, log: false);
            Core.KillMonster("sevencircles", "r6", "Left", "Gluttony", "Essence of Gluttony", essenceTarget, log: false);
            Core.KillMonster("sevencircles", "r8", "Left", "Avarice", "Essence of Avarice", essenceTarget, log: false);
            Core.EnsureCompleteMulti(7978);
            Bot.Wait.ForPickup("Indulgence");
        }
        Core.CancelRegisteredQuests();
    }
}
