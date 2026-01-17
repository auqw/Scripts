/*
name: KillandItemTracker
description: keeps track of kills & items ever [interval] with optional multi-quest input, but only a single mob & item.
tags: tools, kph
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
using System.Diagnostics;
using System.Threading;
using Skua.Core.Options;

public class KPHandItemCounter
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;


    public string OptionsStorage = "KPH";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<string>("Map", "Map", "Map name to farm in", "tercessuinotlim"),
        new Option<string>("Cell", "Cell", "Cell to jump to", "m2"),
        new Option<string>("Pad", "Pad", "Pad to jump to", "Left"),
        new Option<string>("Mob", "Mob", "Mob name to kill (exact)", "Dark Makai"),
        new Option<string>("Item", "Item", "Item to track & sell hourly", "Defeated Makai"),
        new Option<bool>( "SellItem", "Sell the Item", "Sell item to reset count, or keep item and subtract starting amount", true),
        new Option<string>( "Quests", "Quests", "Comma-separated quest IDs to register (e.g., 1,11,111)", "570"),
        new Option<double>( "Interval", "Log Interval (minutes)", "How often to log KPH & item counts (can be fractional, e.g., 0.5 = 30 sec)", 60.0),
        new Option<string>("ExtraDrops", "Extra Drops", "Comma-separated list of other items to pick up.", "Essence of Nulgath, Dark Energy"),
       new Option<bool>("OpenFileAfter", "Open Log File After Session", "Open the log file automatically when the session ends", true),

        CoreBots.Instance.SkipOptions,
    };


    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        KPHCounterWithItem();

        Core.SetOptions(false);
    }


    void KPHCounterWithItem()
    {
        int kph = 0;
        int totalKills = 0;
        int totalItemsGained = 0;

        // Config
        string? Map = Bot.Config!.Get<string>("Map");
        string? Mob = Bot.Config!.Get<string>("Mob");
        string? Item = Bot.Config!.Get<string>("Item");
        bool SellItem = Bot.Config!.Get<bool>("SellItem");
        double intervalMinutes = Bot.Config!.Get<double>("Interval"); // fractional minutes supported
        (string?, string?) CellPad = (Bot.Config!.Get<string>("Cell"), Bot.Config!.Get<string>("Pad"));

        // Parse quests from comma-separated string, remove duplicates & invalids
        string questInput = Bot.Config!.Get<string>("Quests") ?? "";
        int[] Quests = questInput
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(q => int.TryParse(q.Trim(), out int id) ? id : -1)
            .Where(id => id > 0)
            .Distinct()
            .ToArray();

        // Setup log
        string basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "skua",
            "scripts",
            "WIP"
        );
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
        string logFile = Path.Combine(basePath, "KPH_Log.txt");

        // Track starting item count for delta logging
        int startingItemCount = !string.IsNullOrEmpty(Item)
            ? Bot.Inventory.GetQuantity(Item)
            : 0;

        TimeSpan intervalTime = TimeSpan.FromMinutes(intervalMinutes);
        DateTime nextLogTime = DateTime.Now + intervalTime;
        DateTime sessionStartTime = DateTime.Now;

        // Session start divider
        File.AppendAllText(
            logFile,
            $"{Environment.NewLine}=== SESSION START [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ==={Environment.NewLine}"
        );

        // Register quests
        if (Quests.Length > 0)
            Core.RegisterQuests(Quests);

        // Add main item to drops
        if (!string.IsNullOrEmpty(Item))
            Core.AddDrop(Item);

        // Add extra drops directly from config
        foreach (string drop in (Bot.Config!.Get<string>("ExtraDrops") ?? "")
                 .Split(',', StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim())
                 .Distinct()) // optional: remove duplicates
            Core.AddDrop(drop);


        void LogAndReset(bool partial = false)
        {
            int currentItemCount = !string.IsNullOrEmpty(Item)
                ? Bot.Inventory.GetQuantity(Item)
                : 0;

            int gained = SellItem
                ? currentItemCount
                : currentItemCount - startingItemCount;

            // Track totals
            totalKills += kph;
            totalItemsGained += gained;

            // Calculate Hour / Minute / Second
            TimeSpan elapsed = DateTime.Now - sessionStartTime;
            int hourCount = (int)elapsed.TotalMinutes / 60;
            int minuteInHour = (int)Math.Floor(elapsed.TotalMinutes) % 60;
            int secondInMinute = elapsed.Seconds;

            File.AppendAllText(
                logFile,
                $"[🕑 Hour {hourCount} | Min {minuteInHour} | Sec {secondInMinute}]{Environment.NewLine}" +
                $"🌍 Map: {Map ?? "N/A"}{Environment.NewLine}" +
                $"🗡️ Mob: {Mob ?? "Any"}{Environment.NewLine}" +
                $"🏹 Cell/Pad: {CellPad.Item1}/{CellPad.Item2}{Environment.NewLine}" +
                $"🔥 KPH: {kph}{Environment.NewLine}" +
                $"💎 {Item}: {gained}{(partial ? " (Partial Interval)" : "")}{Environment.NewLine}" +
                $"----------------------------------------{Environment.NewLine}"
            );

            if (SellItem && !string.IsNullOrEmpty(Item))
            {
                // Pause timer
                DateTime pauseStart = DateTime.Now;

                Core.SellItem(Item, all: true);

                // Wait until inventory is confirmed empty
                while (!Bot.ShouldExit && Bot.Inventory.GetQuantity(Item) > 0)
                    Bot.Sleep(100);

                // Adjust nextLogTime to ignore selling duration
                DateTime pauseEnd = DateTime.Now;
                TimeSpan pauseDuration = pauseEnd - pauseStart;
                nextLogTime += pauseDuration;
            }
            else
            {
                startingItemCount = currentItemCount;
            }

            kph = 0;
        }

        try
        {
            while (!Bot.ShouldExit)
            {
                // Join map if needed
                if (!string.IsNullOrEmpty(Map) && Bot.Map.Name != Map)
                    Core.Join(Map);

                // Jump to Cell/Pad
                if (!string.IsNullOrEmpty(CellPad.Item1) && Bot.Player.Cell != CellPad.Item1)
                    Bot.Map.Jump(CellPad.Item1, CellPad.Item2 ?? "Spawn");

                // Wait for respawn
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                // Attack mob
                if (!string.IsNullOrEmpty(Mob))
                    Bot.Combat.Attack(Mob);

                Bot.Sleep(200);

                // Increment KPH if mob dies
                if (Bot.Player.Target?.HP <= 0)
                    kph++;

                // Wait for next alive mob
                while (!Bot.ShouldExit && !Bot.Monsters.CurrentAvailableMonsters.Any(x => x.Name == Mob && x.HP > 0))
                    Bot.Sleep(200);

                // Interval logging
                if (DateTime.Now >= nextLogTime)
                {
                    LogAndReset();
                    nextLogTime = DateTime.Now + intervalTime;
                }
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(
                logFile,
                $"[❌ EXCEPTION | {DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                $"{ex.Message}{Environment.NewLine}" +
                $"----------------------------------------{Environment.NewLine}"
            );
        }
        finally
        {
            // Partial interval flush on exit
            if (kph > 0 || (!string.IsNullOrEmpty(Item) && Bot.Inventory.GetQuantity(Item) != startingItemCount))
                LogAndReset(partial: true);

            Core.CancelRegisteredQuests();

            // Session summary
            File.AppendAllText(
                logFile,
                $"{Environment.NewLine}=== SESSION SUMMARY [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ==={Environment.NewLine}" +
                $"🌍 Map: {Map ?? "N/A"}{Environment.NewLine}" +
                $"🗡️ Mob: {Mob ?? "Any"}{Environment.NewLine}" +
                $"🔥 Total Kills: {totalKills}{Environment.NewLine}" +
                $"💎 Total {Item}: {totalItemsGained}{Environment.NewLine}" +
                $"========================================{Environment.NewLine}"
            );

            Core.JumpWait();
            // Session end divider
            File.AppendAllText(
                logFile,
                $"=== SESSION END [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ==={Environment.NewLine}"
            );

            if (Bot.Config!.Get<bool>("OpenFileAfter") && File.Exists(logFile))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = logFile,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Core.Logger($"Failed to open log file: {ex.Message}");
                }
            }

        }
    }

}



