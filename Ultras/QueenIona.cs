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
    public string OptionsStorage = "UltraDage";
    public List<IOption> Options = new()
    {
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    bool IsVHL = false;


    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        // if (!C.IsMember && C.CheckInventory("Queen Iona Bank Companion"))
        // {
        //     C.Logger("Missing \"Queen Iona Bank Companion\" cannot continue");
        //     Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
        //     C.SetOptions(false);
        // }
        Core.Boot();
        Adv.GearStore();
        // Prep();
        Bot.Events.ExtensionPacketReceived += QueenIonaListener;
        Fight();
        Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
        Adv.GearStore(true, true);
        C.SetOptions(false);
    }

    void Prep()
    {
        if (Bot.Player.Alive && Bot.Player.CurrentClass?.Name == "Void Highlord"
                    || Bot.Player.CurrentClass.Name == "Void Highlord (IoDA)")
            IsVHL = true;
        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.SmartEnhance(Bot.Player.CurrentClass.Name);
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
    }



    private bool ionShiftSeen;
    private string? lastZoneSet;

    void Fight()
    {
        const string map = "queeniona";
        const string boss = "Queen Iona";

        Bot.Sleep(2500);
        C.AddDrop("Lothian's Lightning");
        // C.RegisterQuests(C.IsMember ? 9853 : 9854);

        C.Join(map + -100000);
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        if (!IsVHL)
            Core.EnableSkills();

        int skillIndex = 0;
        int[] skillList = { 1, 4, 2 };

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");

            // Track Ion Shift globally (fight-wide)
            if (!ionShiftSeen && Bot.Self.Auras.Any(a => a?.Name == "Ion Shift"))
            {
                C.Logger("ionShiftSeen = True", "Fight");
                ionShiftSeen = true;
            }

            // VHL skill usage
            if (IsVHL)
            {
                if (Bot.Player.Health <= 2500 && Bot.Skills.CanUseSkill(2))
                    Bot.Skills.UseSkill(2);

                if (Bot.Player.HasTarget
                    && Bot.Player.Target?.HP > 0
                    && !Bot.Self.Auras.Any(a => a?.Name == "Shackled")
                    && Bot.Skills.CanUseSkill(skillList[skillIndex]))
                {
                    Bot.Skills.UseSkill(skillList[skillIndex]);
                    skillIndex = (skillIndex + 1) % skillList.Length;
                }
            }

            Bot.Sleep(100);
        }
    }

    public async void QueenIonaListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();

        // Skip packets where there’s no zone set (happens after charge disappears)
        if (string.IsNullOrEmpty(zoneSet))
            return;

        lastZoneSet = zoneSet;

        // Wait until a charge appears
        bool hasPositive = false;
        bool hasNegative = false;

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
                return;

            hasPositive = Bot.Self.Auras.Any(a => a?.Name == "Positive Charge");
            hasNegative = Bot.Self.Auras.Any(a => a?.Name == "Negative Charge");

            // Only move if a charge exists
            if (hasPositive || hasNegative)
                break;

            await Task.Delay(80);
        }

        // If somehow no charge is present, skip this packet
        if (!hasPositive && !hasNegative)
            return;

        // Zone coordinates
        (int x, int y) zoneA = (373, 447);
        (int x, int y) zoneB = (569, 442);

        // Determine target based on charge + IonShiftSeen flag
        (int x, int y) target;

        if (ionShiftSeen)
        {
            // Inverted logic: Positive → same zone, Negative → opposite
            target = hasPositive
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA);
        }
        else
        {
            // Normal logic: Positive → opposite, Negative → same
            target = hasPositive
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB);
        }

        try
        {
            await Task.Run(() => Bot.Player.WalkTo(target.x, target.y));

            // Reset after movement
            ionShiftSeen = false;
            lastZoneSet = null;
        }
        catch (Exception ex)
        {
            C.Logger($"[Iona] WalkTo failed: {ex.Message}", "Listener");
            ionShiftSeen = false;
            lastZoneSet = null;
        }

        C.Logger(
            $"[Iona] Charge={(hasPositive ? "Positive" : "Negative")} | Zone={zoneSet} | Moving to {(target == zoneA ? "A" : "B")} | IonShift={ionShiftSeen}",
            "Listener"
        );
    }








}
