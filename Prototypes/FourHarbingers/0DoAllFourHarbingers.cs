/*
name: Do All Four Harbingers
description: Completes the Four Harbingers story and optionally farms selected Four Harbingers Trove merge items.
tags: four harbingers, fourharbingers, story, quest, merge, complete, all, halosis, bello, fame, mors, anethyxos, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Prototypes/FourHarbingers/CoreFourHarbingers.cs
//cs_include Scripts/Prototypes/FourHarbingers/Halosis.cs
//cs_include Scripts/Prototypes/FourHarbingers/Bello.cs
//cs_include Scripts/Prototypes/FourHarbingers/Fame.cs
//cs_include Scripts/Prototypes/FourHarbingers/Mors.cs
//cs_include Scripts/Prototypes/FourHarbingers/AnethyxosAbsolution.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class DoAllFourHarbingers
{
    public string OptionsStorage = "FourHarbingers_DoAll_v3";
    public bool DontPreconfigure = true;
    public string[] MultiOptions = { "MergeItems" };

    public List<IOption> MergeItems = new()
    {
        new Option<bool>("CelestialDragonOfTime", "Celestial Dragon of Time", "Merge Celestial Dragon of Time.", false),
        new Option<bool>("InfernalDragonOfTime", "Infernal Dragon of Time", "Merge Infernal Dragon of Time.", false),
        new Option<bool>("HarbingerOfWar", "Harbinger of War", "Merge Harbinger of War.", false),
        new Option<bool>("HarbingerOfFamine", "Harbinger of Famine", "Merge Harbinger of Famine.", false),
        new Option<bool>("HarbingerOfDeath", "Harbinger of Death", "Merge Harbinger of Death.", false),
        new Option<bool>("HarbingerOfConquest", "Harbinger of Conquest", "Merge Harbinger of Conquest.", false),
        new Option<bool>("BowOfHalosis", "Bow of Halosis", "Merge Bow of Halosis.", false),
        new Option<bool>("HalosisDivineWheel", "Halosis' Divine Wheel", "Merge Halosis' Divine Wheel.", false),
        new Option<bool>("WingsOfMors", "Wings of Mors", "Merge Wings of Mors.", false),
        new Option<bool>("BelloArmaments", "Bello's Armaments", "Merge Bello's Armaments.", false),
        new Option<bool>("BelloBraziers", "Bello's Braziers", "Merge Bello's Braziers.", false),
        new Option<bool>("BelloSanctifiedBraziers", "Bello's Sanctified Braziers", "Merge Bello's Sanctified Braziers.", false),
        new Option<bool>("HuntingBowOfHalosis", "Hunting Bow of Halosis", "Merge Hunting Bow of Halosis.", false),
        new Option<bool>("PoiseOfHalosis", "Poise of Halosis", "Merge Poise of Halosis.", false),
        new Option<bool>("PoiseOfFames", "Poise of Fames", "Merge Poise of Fames.", false),
        new Option<bool>("PoiseOfMors", "Poise of Mors", "Merge Poise of Mors.", false),
        new Option<bool>("PoiseOfBello", "Poise of Bello", "Merge Poise of Bello.", false),
        new Option<bool>("InfernalDragonOfTimeHood", "Infernal Dragon of Time Hood", "Merge Infernal Dragon of Time Hood.", false),
        new Option<bool>("CelestialDragonOfTimeHood", "Celestial Dragon of Time Hood", "Merge Celestial Dragon of Time Hood.", false),
        new Option<bool>("BelloBrazier", "Bello's Brazier", "Merge Bello's Brazier.", false),
        new Option<bool>("FamesBarrenScale", "Fames' Barren Scale", "Merge Fames' Barren Scale.", false),
        new Option<bool>("BelloSanctifiedBrazier", "Bello's Sanctified Brazier", "Merge Bello's Sanctified Brazier.", false),
        new Option<bool>("NightstarCompanion", "Nightstar Companion", "Use the Nightstar Companion goal instead.", false),
        new Option<bool>("ScytheOfMors", "Scythe of Mors", "Merge Scythe of Mors.", false),
    };

    public List<IOption> Options = new()
    {
        new Option<ClassChoice>("ClassChoice", "Choose Class", "Optimized uses:\n\nHalosis - Dragon of Time\nBello - ArchPaladin\nFame - Yami no Ronin\nMors - Legion Revenant\nAnethyxos - King's Echo\n\nMore class options are available in the individual boss scripts.", ClassChoice.Optimized),
        new Option<GoalChoice>("Goal", "Goal", "Available goals:\n\nComplete Story Only - Completes the Four Harbingers story.\nNightstar Companion - Farms and merges Nightstar Companion.\nAll Merge Items - Farms and merges every shop item.\nEach Individual Merge Item - Uses the MergeItems panel.", GoalChoice.Complete_Story_Only),
        new Option<bool>("UsePotions", "Use Potions", "Use the optimized potions during each fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the optimized enhancements before each fight.", true),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;
    private CoreFourHarbingers FH = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            Run(bot);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Run(IScriptInterface bot)
    {
        if (!CompleteStory(bot))
            return;

        GoalChoice goal = Bot.Config!.Get<GoalChoice>("Goal");
        if (goal == GoalChoice.Complete_Story_Only)
        {
            Core.Logger("Four Harbingers story complete.");
            return;
        }

        if (!CompleteFabulaVinculumFracti())
            return;

        List<string> selectedItems = GetSelectedMergeItems(goal);
        if (!FH.CompleteMergeItems(selectedItems, FarmScroll))
            return;

        Core.Logger("Four Harbingers goal complete.");
    }

    private bool CompleteStory(IScriptInterface bot)
    {
        if (!Core.isCompletedBefore(10850))
        {
            Halosis halosis = new();
            halosis.DoAllMode = true;
            halosis.ScriptMain(bot);
            if (!Core.isCompletedBefore(10850))
                return false;
        }

        if (!Core.isCompletedBefore(10851))
        {
            Bello bello = new();
            bello.DoAllMode = true;
            bello.ScriptMain(bot);
            if (!Core.isCompletedBefore(10851))
                return false;
        }

        if (!Core.isCompletedBefore(10852))
        {
            Fame fame = new();
            fame.DoAllMode = true;
            fame.ScriptMain(bot);
            if (!Core.isCompletedBefore(10852))
                return false;
        }

        if (!Core.isCompletedBefore(10853))
        {
            Mors mors = new();
            mors.DoAllMode = true;
            mors.ScriptMain(bot);
            if (!Core.isCompletedBefore(10853))
                return false;
        }

        if (!Core.isCompletedBefore(10854))
        {
            AnethyxosAbsolution absolution = new();
            absolution.DoAllMode = true;
            absolution.ScriptMain(bot);
            if (!Core.isCompletedBefore(10854))
                return false;
        }

        return true;
    }

    private bool CompleteFabulaVinculumFracti()
    {
        if (Core.isCompletedBefore(10855))
            return true;

        if (!Core.EnsureAccept(10855))
        {
            Core.Logger("WARNING: Fabula Vinculum Fracti [10855] could not be accepted.", messageBox: true);
            return false;
        }

        if (!FarmScroll("Scroll of the Quartet", 1)
            || !FarmScroll("Scroll of the Wanderer", 1)
            || !FarmScroll("Scroll of the Preacher", 1)
            || !FarmScroll("Scroll of the Benevolent", 1)
            || !FarmScroll("Scroll of the Innocent", 1)
            || !FarmScroll("Scroll of the Heretic", 1))
            return false;

        if (!Core.EnsureComplete(10855))
        {
            Core.Logger("WARNING: Fabula Vinculum Fracti [10855] could not be completed.", messageBox: true);
            return false;
        }

        return true;
    }

    private List<string> GetSelectedMergeItems(GoalChoice goal)
    {
        if (goal == GoalChoice.Nightstar_Companion)
            return new List<string> { "Nightstar Companion" };

        if (goal == GoalChoice.All_Merge_Items)
            return CoreFourHarbingers.AllMergeItems.ToList();

        List<string> selectedItems = new();
        AddSelectedItem(selectedItems, "CelestialDragonOfTime", "Celestial Dragon of Time");
        AddSelectedItem(selectedItems, "InfernalDragonOfTime", "Infernal Dragon of Time");
        AddSelectedItem(selectedItems, "HarbingerOfWar", "Harbinger of War");
        AddSelectedItem(selectedItems, "HarbingerOfFamine", "Harbinger of Famine");
        AddSelectedItem(selectedItems, "HarbingerOfDeath", "Harbinger of Death");
        AddSelectedItem(selectedItems, "HarbingerOfConquest", "Harbinger of Conquest");
        AddSelectedItem(selectedItems, "BowOfHalosis", "Bow of Halosis");
        AddSelectedItem(selectedItems, "HalosisDivineWheel", "Halosis' Divine Wheel");
        AddSelectedItem(selectedItems, "WingsOfMors", "Wings of Mors");
        AddSelectedItem(selectedItems, "BelloArmaments", "Bello's Armaments");
        AddSelectedItem(selectedItems, "BelloBraziers", "Bello's Braziers");
        AddSelectedItem(selectedItems, "BelloSanctifiedBraziers", "Bello's Sanctified Braziers");
        AddSelectedItem(selectedItems, "HuntingBowOfHalosis", "Hunting Bow of Halosis");
        AddSelectedItem(selectedItems, "PoiseOfHalosis", "Poise of Halosis");
        AddSelectedItem(selectedItems, "PoiseOfFames", "Poise of Fames");
        AddSelectedItem(selectedItems, "PoiseOfMors", "Poise of Mors");
        AddSelectedItem(selectedItems, "PoiseOfBello", "Poise of Bello");
        AddSelectedItem(selectedItems, "InfernalDragonOfTimeHood", "Infernal Dragon of Time Hood");
        AddSelectedItem(selectedItems, "CelestialDragonOfTimeHood", "Celestial Dragon of Time Hood");
        AddSelectedItem(selectedItems, "BelloBrazier", "Bello's Brazier");
        AddSelectedItem(selectedItems, "FamesBarrenScale", "Fames' Barren Scale");
        AddSelectedItem(selectedItems, "BelloSanctifiedBrazier", "Bello's Sanctified Brazier");
        AddSelectedItem(selectedItems, "ScytheOfMors", "Scythe of Mors");

        if (Bot.Config!.Get<bool>("MergeItems", "NightstarCompanion"))
            Core.Logger("Nightstar Companion was selected in MergeItems. Use the Nightstar Companion goal instead. Continuing without Nightstar Companion.", messageBox: true);

        return selectedItems;
    }

    private void AddSelectedItem(List<string> selectedItems, string optionName, string itemName)
    {
        if (Bot.Config!.Get<bool>("MergeItems", optionName))
            selectedItems.Add(itemName);
    }

    private bool FarmScroll(string itemName, int quantity)
    {
        if (Core.CheckInventory(itemName, quantity, toInv: false))
            return true;

        if (itemName.Equals("Scroll of the Quartet", StringComparison.OrdinalIgnoreCase)
            || itemName.Equals("Scroll of the Wanderer", StringComparison.OrdinalIgnoreCase))
        {
            Halosis halosis = new();
            ConfigureFarm(halosis, itemName, quantity);
            halosis.ScriptMain(Bot);
        }
        else if (itemName.Equals("Scroll of the Preacher", StringComparison.OrdinalIgnoreCase))
        {
            Bello bello = new();
            ConfigureFarm(bello, itemName, quantity);
            bello.ScriptMain(Bot);
        }
        else if (itemName.Equals("Scroll of the Benevolent", StringComparison.OrdinalIgnoreCase))
        {
            Fame fame = new();
            ConfigureFarm(fame, itemName, quantity);
            fame.ScriptMain(Bot);
        }
        else if (itemName.Equals("Scroll of the Innocent", StringComparison.OrdinalIgnoreCase))
        {
            Mors mors = new();
            ConfigureFarm(mors, itemName, quantity);
            mors.ScriptMain(Bot);
        }
        else if (itemName.Equals("Scroll of the Heretic", StringComparison.OrdinalIgnoreCase))
        {
            AnethyxosAbsolution absolution = new();
            ConfigureFarm(absolution, itemName, quantity);
            absolution.ScriptMain(Bot);
        }
        else
        {
            Core.Logger($"WARNING: No Four Harbingers farm is configured for {itemName}.", messageBox: true);
            return false;
        }

        return Core.CheckInventory(itemName, quantity, toInv: false);
    }

    private void ConfigureFarm(Halosis bossScript, string itemName, int quantity)
    {
        bossScript.DoAllMode = true;
        bossScript.FarmModeOverride = CoreFourHarbingers.FarmMode.Item_Quantity;
        bossScript.FarmQuantityOverride = quantity;
        bossScript.FarmItemOverride = itemName;
    }

    private void ConfigureFarm(Bello bossScript, string itemName, int quantity)
    {
        bossScript.DoAllMode = true;
        bossScript.FarmModeOverride = CoreFourHarbingers.FarmMode.Item_Quantity;
        bossScript.FarmQuantityOverride = quantity;
        bossScript.FarmItemOverride = itemName;
    }

    private void ConfigureFarm(Fame bossScript, string itemName, int quantity)
    {
        bossScript.DoAllMode = true;
        bossScript.FarmModeOverride = CoreFourHarbingers.FarmMode.Item_Quantity;
        bossScript.FarmQuantityOverride = quantity;
        bossScript.FarmItemOverride = itemName;
    }

    private void ConfigureFarm(Mors bossScript, string itemName, int quantity)
    {
        bossScript.DoAllMode = true;
        bossScript.FarmModeOverride = CoreFourHarbingers.FarmMode.Item_Quantity;
        bossScript.FarmQuantityOverride = quantity;
        bossScript.FarmItemOverride = itemName;
    }

    private void ConfigureFarm(AnethyxosAbsolution bossScript, string itemName, int quantity)
    {
        bossScript.DoAllMode = true;
        bossScript.FarmModeOverride = CoreFourHarbingers.FarmMode.Item_Quantity;
        bossScript.FarmQuantityOverride = quantity;
        bossScript.FarmItemOverride = itemName;
    }

    private enum ClassChoice
    {
        Optimized,
    }

    private enum GoalChoice
    {
        Complete_Story_Only,
        Nightstar_Companion,
        All_Merge_Items,
        Each_Individual_Merge_Item,
    }
}
