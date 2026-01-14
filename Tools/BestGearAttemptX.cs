/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class BestGearAttemptX
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public string OptionsStorage = "BestGearAttemptX";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "Restore Equipment After",
            "GearRestore",
            "This will Store Your Current Gear *before* swapping so it can swap back afterwards",
            false
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        EquipBestItemsForMetaTest();

        Core.SetOptions(false);
    }

    public void EquipBestItemsForMetaTest()
    {
        // Initial "READ ME" message
        Bot.ShowMessageBox(
            "For the moment it's purely manual input from me with it being 90% dmg all with some gold/xp/classpoints MetaTypes (MetaTypes = Boost Type)",
            "READ ME"
        );

        // 1/5 chance to show the "ad-like" message
        Random rnd = new();
        if (rnd.Next(1, 6) == 1) // Generates a number between 1 and 100
        {
            bool userConfirmed =
                Bot.ShowMessageBox(
                    $@"
            Initializing ""Semi"" -Best Gear™ (Unoptimized) Pro®\n
            █▓▒░ Now Featuring:\n
            ⚔️ [Gear+™ Edition] ⚔️\n
            ✓ Best-in-class efficiency!\n
            ✓ Highly Rated®: (★★★★★)\n
            ✓ Meta™ Gear™ Booster® 2.0™✓\n
            ➡️ Get: [Instant +™ Power™ + Gear Boost™]⚡\n
            ⬇ Upgrade Now ⬇\n
            ⬛⬜◼ More Logos™, More™ Power™ ◼⬛\n
            Rated: R™ - *Powering Up*\n",
                    "AD-LIKE MESSAGE",
                    true
                ) == true; // Displays Yes/No buttons

            if (userConfirmed)
            {
                Bot.ShowMessageBox(
                    $@"
                ⚡ SUCCESS! ⚡\n
                ✨ You are now in the elite tier! ✨\n
                ░▒▓ Your power level has been boosted! ▓▒░\n
                💥 More Logos™ Incoming: 💥\n
                🌟 - Ultimate Boost™ - 🌟\n
                ⚔ Powered by ChatGPT™ ⚔\n
                ░▒ Power Surge Pro Max® Edition ▒░\n
                🛡 Defense UP! ™\n
                ⚡ - Unlimited Boost™ - ⚡\n
            ",
                    "Power Up Complete!"
                );

                Core.Logger(
                    $@"
                USER CONFIRMED ACTION! ⚡\n
                ✨ Elite Tier Achieved! Powered by ChatGPT™✨\n
                ░▒▓ Boost™ Active ▓▒░\n
            "
                );
            }
            else
            {
                Bot.ShowMessageBox(
                    $@"
                ❌ ACTION DECLINED ❌\n
                You missed out on:\n
                ❗ Power Boost Pro Max™ ❗\n
                ✨ Ultimate Tier™ ✨\n
                ⚔ The Ultimate Logo Infusion™ ⚔\n
                🔥 Upgrade Denied 🔥\n
            ",
                    "Declined Opportunity!"
                );

                Core.Logger(
                    $@"
                USER DECLINED ACTION! ❌\n
                Missed out on ⚔ Ultimate Logo Infusion™ ⚔ and ✨ Elite Power Surge™ ✨\n
            "
                );
            }
        }

        // Store gear if config allows
        if (Bot.Config!.Get<bool>("Restore Equipment After"))
            Adv.GearStore(EnhAfter: true);

        if (Bot.House.Items.Any(h => h.Equipped))
        {
            Bot.Send.Packet($"%xt%zm%house%1%{Core.Username()}%");
            Bot.Wait.ForMapLoad("house");
        }

        // Equip best items based on the meta
        Core.EquipBestItemsForMeta(
            new Dictionary<string, string[]>
            {
                {
                    "Cape",
                    new[]
                    {
                        "dmgAll",
                        "Undead",
                        "Chaos",
                        "Elemental",
                        "Dragonkin",
                        "Human",
                        "gold",
                        "cp",
                        "rep",
                    }
                },
                {
                    "Helm",
                    new[]
                    {
                        "dmgAll",
                        "Undead",
                        "Chaos",
                        "Elemental",
                        "Dragonkin",
                        "Human",
                        "gold",
                        "cp",
                        "rep",
                    }
                },
                {
                    "Armor",
                    Core.CheckInventory("Polly Roger")
                        ? new[] { "gold", "cp", "rep" }
                        : new[] { "dmgAll", "gold", "cp", "rep" }
                },
                { "Weapon", new[] { "dmgAll", "gold", "cp", "rep" } }, // Special case: includes all weapon types
                {
                    "Pet",
                    new[]
                    {
                        "Undead",
                        "Chaos",
                        "Elemental",
                        "Dragonkin",
                        "Human",
                        "gold",
                        "cp",
                        "rep",
                        "dmgAll",
                    }
                },
            }
        );

        // Restore original gear if the config option is enabled
        if (Bot.Config.Get<bool>("Restore Equipment After"))
            Adv.GearStore(true, EnhAfter: true);
    }
}
