/*
name: QueenIona
description: Queen Iona solo
tags: Ultra, queen, iona, queen iona
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs



using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class QueenIona
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();
    public bool DontPreconfigure = true;
    public string OptionsStorage = "QueenIona";
    public List<IOption> Options = new()
    {
        new Option<bool>("ZoneDebuglog", "Zone Logs",  "En/Disable the Logging of zones in chat/logs.", true),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    bool IsVHL = false;
    bool HasQuestItem => C.CheckInventory("Queen Iona Bank Companion");


    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        Core.Boot();
        Fight();
        C.SetOptions(false);
    }

    void Prep(bool AnotherScript = false)
    {
        Bot.Skills.Stop();
        if (Bot.Player!.Alive && Bot.Player!.CurrentClass?.Name == "Void Highlord"
                    || Bot.Player!.CurrentClass?.Name == "Void Highlord (IoDA)")
            IsVHL = true;

        if (AnotherScript || Bot.Config!.Get<bool>("DoEnh"))
        {
            Adv.GearStore(EnhAfter: true);
            Adv.SmartEnhance(Bot.Player!.CurrentClass!.Name);
        }
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
    }

    public void Fight(string item = "Lothian's Lightning", int quant = 100, bool AnotherScript = false)
    {
        if (item == null || C.CheckInventory(item, quant))
            return;

        Prep(AnotherScript);
        Bot.Events.ExtensionPacketReceived += QueenIonaListener;

        string map = "queeniona";
        string boss = "Queen Iona";

        if (!Bot.Quests.IsDailyComplete(9852))
            C.EnsureAccept(9852);

        C.AddDrop(item);

        C.FarmingLogger(item, quant);

            C.RegisterQuests((!C.IsMember && HasQuestItem) ? 9854 : 9853);
        
        // Always private
        C.Join(map + -100000, "r2", "Left");
        Core.ChooseBestCell(boss);
        Bot.Player!.SetSpawnPoint();

        if (!IsVHL)
            Core.EnableSkills();

        int skillIndex = 0;
        int[] skillList = { 1, 4, 2 };

        while (!Bot.ShouldExit && !C.CheckInventory(item, quant))
        {
            // Dead → wait for respawn
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player!.Alive, 20);
                continue;
            }

            if (Bot.Player!.Cell != "r2")
            {
                Bot.Map.Jump("r2", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("r2");
            }

            if (item == "Lothian's Lightning" && !HasQuestItem && !Bot.Quests.IsDailyComplete(9852) && Bot.TempInv.Contains(87681))
            {
                C.Jump("Enter", "Spawn");
                C.EnsureComplete(9852);
                if (HasQuestItem)
                    continue;

                C.Logger("Daily complete come back tomarrow");
                Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
                return;
            }

            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");

            // VHL skill usage
            if (IsVHL)
            {
                if (Bot.Player!.Health <= 2500 && Bot.Skills.CanUseSkill(2))
                    Bot.Skills.UseSkill(2);

                if (Bot.Player!.HasTarget
                    && Bot.Player!.Target?.HP > 0
                    && !Bot.Self.Auras.Any(a => a?.Name == "Shackled")
                    && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                    skillIndex = (skillIndex + 1) % skillList.Length;
                }
            }

            Bot.Sleep(200);
        }
        Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
        //turn all the stuffs back on from main scripts
        C.SetOptions(true);

        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
    }

    async void QueenIonaListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;

        if (!Bot.Player!.Alive)
            return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return; // ignore empty packets

        // Wait until a charge appears (normal or inverted)
        string? chargeAura = null;
        while (!Bot.ShouldExit)
        {
            if (!Bot.Player!.Alive)
                return;

            chargeAura = Bot.Self?.Auras?.FirstOrDefault(a => a != null &&
                          (a?.Name == "Positive Charge" || a?.Name == "Negative Charge" ||
                           a?.Name == "Positive Charge?" || a?.Name == "Negative Charge?"))?.Name;

            if (!string.IsNullOrEmpty(chargeAura!))
                break;

            await Task.Delay(100);
        }

        if (string.IsNullOrEmpty(chargeAura))
            return;

        // Zone coordinates
        (int x, int y) zoneA = (373, 447);
        (int x, int y) zoneB = (569, 442);

        // Determine if the aura is inverted
        bool inverted = chargeAura!.EndsWith("?");

        // Decide target
        (int x, int y) target;

        if (!inverted)
        {
            // Normal charges
            target = chargeAura == "Positive Charge"
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB);
        }
        else
        {
            // Inverted charges (with ?)
            target = chargeAura == "Positive Charge?"
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA);
        }

        try
        {
            await Task.Run(() => Bot.Player!.WalkTo(target.x, target.y));
        }
        catch (Exception ex)
        {
            if (Bot.Config!.Get<bool>("ZoneDebuglog"))
                C.Logger($"[Iona] WalkTo failed: {ex.Message}", "Listener");
        }

        if (Bot.Config!.Get<bool>("ZoneDebuglog"))
            C.Logger(
                $"Charge={chargeAura} | Zone={zoneSet} | Moving to {(target == zoneA ? "A" : "B")} | Inverted={inverted}",
                "Listener"
            );
    }
}
