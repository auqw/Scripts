/*
name: Item Enhancement Manager V2
description: Unbanks and enhances selected weapons, helms, and capes, leaving them in inventory. Bypasses Disable AutoEnhance. Items are picked from dropdowns of your inventory.
tags: enhancement, unbank, weapon, helm, cape, inventory
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Options;

#nullable enable

public class ItemEnhancementManagerV2
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv => CoreAdvanced.Instance;

    // Its own options file: a dropdown can save a name as "Some_Item", which the original
    // Item Enhancement Manager (typed names, "ItemEnhancementPrototype") would not find.
    private const string Storage = "ItemEnhancementManagerV2";
    private const string OriginalStorage = "ItemEnhancementPrototype";
    public string OptionsStorage = Storage;
    public bool DontPreconfigure = true;
    public string[] MultiOptions = { "Weapon", "Helm", "Cape", "Skip_Options" };

    public List<IOption> Skip_Options = new()
    {
        CoreBots.Instance.SkipOptions,
    };

    // The item dropdowns. Skua's options window makes a dropdown only for an enum option and lists
    // its names, so each list is an enum made when the script loads: None, then the matching items
    // in the inventory (and the bank, when it was loaded this session), then the items already
    // picked, so a pick that was banked or sold is still shown and kept. Skua shows "_" as a space.
    private static Type WeaponItems => ItemList("Weapon");
    private static Type HelmItems => ItemList("Helm");
    private static Type CapeItems => ItemList("Cape");

    private static bool InCategory(string category, InventoryItem item) => category switch
    {
        "Weapon" => item.ItemGroup == "Weapon",
        "Helm" => item.Category == ItemCategory.Helm,
        _ => item.Category == ItemCategory.Cape,
    };

    private static Type ItemList(string category)
    {
        IScriptInterface bot = IScriptInterface.Instance;
        List<string> owned = new();
        try
        {
            owned = bot.Inventory.Items.Concat(bot.Bank.Items)
                .Where(item => item?.Name != null && InCategory(category, item))
                .Select(item => item.Name.Trim())
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            // Not logged in: only None and the saved picks.
        }

        // Owned items differing in more than case each get a row (two names that only differ in
        // punctuation are two items). A saved pick is left out when it looks like a listed item with
        // case, spaces and punctuation ignored: names typed into the original manager were loose.
        List<string> names = new() { "None" };
        HashSet<string> literals = new(StringComparer.OrdinalIgnoreCase) { "None" };
        foreach (string name in owned)
        {
            if (name.Length > 0 && literals.Add(EnumName(name)))
                names.Add(name);
        }
        HashSet<string> keys = new(names.Select(Key));
        foreach (string name in SavedPicks(category))
        {
            if (name.Length > 0 && keys.Add(Key(name)) && literals.Add(EnumName(name)))
                names.Add(name);
        }

        AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("ItemEnhancementManagerLists" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect);
        EnumBuilder list = assembly.DefineDynamicModule("Lists").DefineEnum(category + "Items", TypeAttributes.Public, typeof(int));
        for (int index = 0; index < names.Count; index++)
            list.DefineLiteral(EnumName(names[index]), index);
        return list.CreateTypeInfo()!.AsType();
    }

    // As a name Enum.Parse finds again: spaces as "_" (Skua parses the saved value so), no commas
    // (Enum.Parse reads those as a list), and not starting like a number.
    private static string EnumName(string name)
    {
        string text = name.Trim().Replace(", ", " ").Replace(',', ' ').Replace(' ', '_');
        return text.Length > 0 && (char.IsDigit(text[0]) || text[0] == '-' || text[0] == '+') ? "#" + text : text;
    }

    // The owned item a pick names, in its category: the same name first (as the dropdown saved it,
    // case ignored), else one that matches with spaces and punctuation ignored too (names typed into
    // the original manager).
    private InventoryItem? FindItem(string category, string pick)
    {
        List<InventoryItem> items = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .Where(item => item?.Name != null && InCategory(category, item))
            .ToList();
        return items.FirstOrDefault(item => string.Equals(EnumName(item.Name), EnumName(pick), StringComparison.OrdinalIgnoreCase))
            ?? items.FirstOrDefault(item => Key(item.Name) == Key(pick));
    }

    // Compares an item's name with a pick: case, spaces and punctuation ignored.
    private static string Key(string name) =>
        new string(name.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    // The items picked last time, from the options file (Skua loads it after the lists are made),
    // and the names typed into the original Item Enhancement Manager, so those are listed too.
    private static IEnumerable<string> SavedPicks(string category) =>
        PicksIn(Storage, category).Concat(PicksIn(OriginalStorage, category));

    // VibeSkua keeps one options file per account, "<storage>(<account>).cfg", read when it is there.
    private static List<string> PicksIn(string storage, string category)
    {
        string file = Path.Combine(ClientFileSources.SkuaOptionsDIR, storage + ".cfg");
        List<string> picks = new();
        try
        {
            IScriptInterface bot = IScriptInterface.Instance;
            if (bot.Player.LoggedIn && bot.Player.Username is { Length: > 0 } account)
            {
                string safe = new string(account.Trim().ToLowerInvariant()
                    .Select(c => char.IsLetterOrDigit(c) || c is ' ' or '_' or '-' or '.' ? c : '_').ToArray());
                string own = Path.Combine(ClientFileSources.SkuaOptionsDIR, $"{storage}({safe}).cfg");
                if (File.Exists(own))
                    file = own;
            }
            if (!File.Exists(file))
                return picks;
            foreach (string line in File.ReadLines(file))
            {
                string prefix = category + ":" + category + "Item";
                int equals = line.IndexOf('=');
                if (!line.StartsWith(prefix) || equals < 0)
                    continue;
                string pick = line[(equals + 1)..].Trim().TrimStart('#').Replace('_', ' ');
                if (pick.Length > 0 && pick != "None")
                    picks.Add(pick);
            }
        }
        catch
        {
            // Unreadable: the lists just lack the old picks.
        }
        return picks;
    }

    /// <summary>The item picked in a dropdown, or null for None. Upstream VibeSkua saves it with '_' for spaces.</summary>
    private string? PickedItem(string category, int index)
    {
        string pick = (Bot.Config!.Get<string>(category, category + "Item" + index) ?? string.Empty).Trim().TrimStart('#').Replace('_', ' ');
        return pick.Length == 0 || pick == "None" ? null : pick;
    }

    private sealed class ItemOption : IOption
    {
        public ItemOption(string name, string displayName, string description, Type list)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            Type = list;
        }

        public string Category { get; set; } = "Options";
        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public object? DefaultValue => "None";
        public bool Transient => false;
        public Type Type { get; }
        // A dropdown that filters as you type, on clients that offer one (IOption.Searchable); unused elsewhere.
        public bool Searchable => true;
        public override string ToString() => DisplayName;
    }

    public List<IOption> Weapon = new()
    {
        new ItemOption("WeaponItem1", "Item 1", "Weapon from your inventory. None skips it.", WeaponItems),
        new Option<WeaponEnhancement>("WeaponEnhancement1", "Enhancement 1", "Enhancement for Item 1.", WeaponEnhancement.None),
        new ItemOption("WeaponItem2", "Item 2", "Weapon from your inventory. None skips it.", WeaponItems),
        new Option<WeaponEnhancement>("WeaponEnhancement2", "Enhancement 2", "Enhancement for Item 2.", WeaponEnhancement.None),
        new ItemOption("WeaponItem3", "Item 3", "Weapon from your inventory. None skips it.", WeaponItems),
        new Option<WeaponEnhancement>("WeaponEnhancement3", "Enhancement 3", "Enhancement for Item 3.", WeaponEnhancement.None),
        new ItemOption("WeaponItem4", "Item 4", "Weapon from your inventory. None skips it.", WeaponItems),
        new Option<WeaponEnhancement>("WeaponEnhancement4", "Enhancement 4", "Enhancement for Item 4.", WeaponEnhancement.None),
        new ItemOption("WeaponItem5", "Item 5", "Weapon from your inventory. None skips it.", WeaponItems),
        new Option<WeaponEnhancement>("WeaponEnhancement5", "Enhancement 5", "Enhancement for Item 5.", WeaponEnhancement.None),
    };

    public List<IOption> Helm = new()
    {
        new ItemOption("HelmItem1", "Item 1", "Helm from your inventory. None skips it.", HelmItems),
        new Option<HelmEnhancement>("HelmEnhancement1", "Enhancement 1", "Enhancement for Item 1.", HelmEnhancement.None),
        new ItemOption("HelmItem2", "Item 2", "Helm from your inventory. None skips it.", HelmItems),
        new Option<HelmEnhancement>("HelmEnhancement2", "Enhancement 2", "Enhancement for Item 2.", HelmEnhancement.None),
        new ItemOption("HelmItem3", "Item 3", "Helm from your inventory. None skips it.", HelmItems),
        new Option<HelmEnhancement>("HelmEnhancement3", "Enhancement 3", "Enhancement for Item 3.", HelmEnhancement.None),
        new ItemOption("HelmItem4", "Item 4", "Helm from your inventory. None skips it.", HelmItems),
        new Option<HelmEnhancement>("HelmEnhancement4", "Enhancement 4", "Enhancement for Item 4.", HelmEnhancement.None),
        new ItemOption("HelmItem5", "Item 5", "Helm from your inventory. None skips it.", HelmItems),
        new Option<HelmEnhancement>("HelmEnhancement5", "Enhancement 5", "Enhancement for Item 5.", HelmEnhancement.None),
    };

    public List<IOption> Cape = new()
    {
        new ItemOption("CapeItem1", "Item 1", "Cape from your inventory. None skips it.", CapeItems),
        new Option<CapeEnhancement>("CapeEnhancement1", "Enhancement 1", "Enhancement for Item 1.", CapeEnhancement.None),
        new ItemOption("CapeItem2", "Item 2", "Cape from your inventory. None skips it.", CapeItems),
        new Option<CapeEnhancement>("CapeEnhancement2", "Enhancement 2", "Enhancement for Item 2.", CapeEnhancement.None),
        new ItemOption("CapeItem3", "Item 3", "Cape from your inventory. None skips it.", CapeItems),
        new Option<CapeEnhancement>("CapeEnhancement3", "Enhancement 3", "Enhancement for Item 3.", CapeEnhancement.None),
        new ItemOption("CapeItem4", "Item 4", "Cape from your inventory. None skips it.", CapeItems),
        new Option<CapeEnhancement>("CapeEnhancement4", "Enhancement 4", "Enhancement for Item 4.", CapeEnhancement.None),
        new ItemOption("CapeItem5", "Item 5", "Cape from your inventory. None skips it.", CapeItems),
        new Option<CapeEnhancement>("CapeEnhancement5", "Enhancement 5", "Enhancement for Item 5.", CapeEnhancement.None),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (!bot.Config!.Get<bool>(Core.SkipOptions))
            bot.Config.Configure();
        if (Bot.ShouldExit)
            return;

        List<string> itemNames = new[] { "Weapon", "Helm", "Cape" }
            .SelectMany(category => Enumerable.Range(1, 5).Select(index => PickedItem(category, index)))
            .OfType<string>()
            .ToList();
        Core.BankingBlackList.AddRange(itemNames);
        Core.BankingBlackList.AddRange(Bot.Inventory.Items
            .Where(item => item.Name != null && itemNames.Any(name => Key(item.Name) == Key(name)))
            .Select(item => item.Name));

        Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        Dictionary<int, string> enhanced = new();
        foreach (string category in MultiOptions)
        {
            if (category == "Skip_Options")
                continue;

            for (int index = 1; index <= 5; index++)
            {
                if (Bot.ShouldExit)
                    return;

                if (PickedItem(category, index) is not { } itemName)
                    continue;

                string selection;
                if (category == "Weapon")
                    selection = Bot.Config.Get<WeaponEnhancement>(category, category + "Enhancement" + index).ToString();
                else if (category == "Helm")
                    selection = Bot.Config.Get<HelmEnhancement>(category, category + "Enhancement" + index).ToString();
                else
                    selection = Bot.Config.Get<CapeEnhancement>(category, category + "Enhancement" + index).ToString();

                if (selection == "None")
                    continue;

                InventoryItem? item = FindItem(category, itemName);
                string logName = new string((item?.Name ?? itemName)
                    .Where(character => character >= ' ' && character <= '~' && "%<>&\"'".IndexOf(character) < 0)
                    .ToArray());
                if (item == null)
                {
                    Core.Logger($"{category} item {index} - {logName} was not found among the {category.ToLower()}s in inventory or bank. Check the configured name.");
                    continue;
                }

                // The same item in two slots: the second enhancement would replace the first.
                if (enhanced.TryGetValue(item.ID, out string? earlier))
                {
                    Core.Logger($"{category} item {index} - {logName} [ID {item.ID}] is the same item as {earlier}, enhanced already. Skipping.");
                    continue;
                }

                bool regular = Enum.TryParse(selection, out EnhancementType type)
                    && Enum.IsDefined(typeof(EnhancementType), type);
                WeaponSpecial weapon = WeaponSpecial.None;
                HelmSpecial helm = HelmSpecial.None;
                CapeSpecial cape = CapeSpecial.None;
                if (!regular)
                {
                    type = EnhancementType.Lucky;
                    if (category == "Weapon")
                    {
                        if (!Enum.TryParse(selection, out weapon))
                            continue;
                        if (weapon == WeaponSpecial.Forge)
                        {
                            Core.Logger($"Weapon item {index} - {logName} [ID {item.ID}]: Forge is not supported by the current Core enhancement route. Skipping.");
                            continue;
                        }
                        if ((int)weapon >= 2 && (int)weapon <= 6 && !Adv.uAwe())
                        {
                            Core.Logger($"Weapon item {index} - {logName} [ID {item.ID}] requires unlocked Awe enhancements. Skipping.");
                            continue;
                        }
                        if (Enum.IsDefined(typeof(EnhancementType), item.EnhancementPatternID))
                            type = (EnhancementType)item.EnhancementPatternID;
                    }
                    else if (category == "Helm")
                    {
                        if (!Enum.TryParse(selection, out helm))
                            continue;
                    }
                    else if (!Enum.TryParse(selection, out cape))
                        continue;
                }

                Core.BankingBlackList.Add(item.Name);
                int itemID = item.ID;
                Core.Unbank(itemID);
                item = Bot.Inventory.Items.FirstOrDefault(candidate => candidate.ID == itemID);
                if (item == null)
                {
                    Core.Logger($"{category} item {index} - {logName} [ID {itemID}] could not be moved into inventory. Skipping.");
                    continue;
                }

                enhanced[itemID] = $"{category} item {index}";
                Core.Logger($"Applying {selection.Replace('_', ' ')} to {category} item {index} - {logName} [ID {itemID}].");
                Adv.EnhanceItem(item.Name, type, cape, helm, weapon, logging: true, bypassDisableAutoEnhance: true);
                bool applied = Bot.Wait.ForTrue(() =>
                {
                    InventoryItem? updated = Bot.Inventory.Items.FirstOrDefault(candidate => candidate.ID == itemID);
                    if (updated == null || updated.EnhancementLevel <= 0)
                        return false;
                    if (weapon != WeaponSpecial.None)
                        return Core.GetItemProperty<int>(updated, "ProcID") == (int)weapon;
                    if (helm != HelmSpecial.None)
                        return updated.EnhancementPatternID == (int)helm;
                    if (cape != CapeSpecial.None)
                        return updated.EnhancementPatternID == (int)cape;
                    return updated.EnhancementPatternID == (int)type;
                }, 10);

                if (applied)
                    Core.Logger($"Verified {category} item {index} - {logName} [ID {itemID}]. The item remains in inventory.");
                else
                    Core.Logger($"Could not verify the selected enhancement for {category} item {index} - {logName} [ID {itemID}]. Check the Core enhancement logs.");
            }
        }

        Core.Logger("Item enhancement pass finished.");
    }

    public enum WeaponEnhancement
    {
        None,
        Forge,
        Lacerate,
        Smite,
        Valiance,
        Arcanas_Concerto,
        Acheron,
        Elysium,
        Praxis,
        Dauntless,
        Ravenous,
        Spiral_Carve,
        Awe_Blast,
        Health_Vamp,
        Mana_Vamp,
        Powerword_Die,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }

    public enum HelmEnhancement
    {
        None,
        Forge,
        Vim,
        Examen,
        Anima,
        Pneuma,
        Hearty,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }

    public enum CapeEnhancement
    {
        None,
        Forge,
        Absolution,
        Avarice,
        Vainglory,
        Penitence,
        Lament,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }
}
