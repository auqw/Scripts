/*
name: UltraDrago
description: Ultra King Drago helper with taunter classes and priority adds.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDrago
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

#nullable enable

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDrago";

    // User options
    public List<IOption> Options = new()
    {
        new Option<string>(
            "TaunterOne",
            "First Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
        new Option<bool>(
            "useSecondTaunter",
            "Use Second Taunter",
            "Enable if you have a second taunter.",
            false
        ),
        new Option<string>(
            "TaunterTwo",
            "Second Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
    };

    // Filled at runtime
    private string a = "";
    private string b = "";

    public void ScriptMain(IScriptInterface bot)
    {
        // Pull options
        a = (Bot.Config?.Get<string>("TaunterOne") ?? "").Trim();
        b = (Bot.Config?.Get<string>("TaunterTwo") ?? "").Trim();
        bool useSecond = Bot.Config?.Get<bool>("useSecondTaunter") ?? false;

        // ============================
        //   TAUNTER REQUIREMENT LOGIC
        // ============================

        // Rule 0: T1 must always be filled
        if (string.IsNullOrEmpty(a))
        {
            C.Logger("Setup", "First taunter class is not filled in Script Options.");
            Bot.Stop();
            return;
        }

        // If T2 is required but not provided → STOP
        if (useSecond && string.IsNullOrEmpty(b))
        {
            C.Logger("Setup", "Second taunter class is enabled but not filled.");
            Bot.Stop();
            return;
        }

        Astravia.CompleteCoreAstravia();

        // All conditions satisfied → START script
        Core.Boot();
        Bot.Quests.UpdateQuest(8395);
        Prep();
        Fight();
        Bot.Stop();
    }

    // Check if current player is taunter
    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        // Taunters prep differently
        if (IsTaunter())
        {
            // Only taunters need Scroll of Enrage
            Ultra.GetScrollOfEnrage();
            return;
        }

        // Non-taunters prep with alchemy boosts
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
    }

    void Fight()
    {
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string executioner = "Executioner Dene";
        const string bowmaster = "Bowmaster Algie";

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_drago.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        // Main fight loop
        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (IsTaunter())
            {
                // Taunter's responsibility: keep Executioner locked
                while (Ultra.MonsterAlive(executioner) && !Bot.ShouldExit)
                {
                    // Prefer primary taunter when equipped
                    if (Core.HasClassEquipped(a))
                        Ultra.Taunt(a, executioner, "aura", 250, "Focus");
                    else if (Core.HasClassEquipped(b))
                        Ultra.Taunt(b, executioner, "aura", 700, "Focus");
                }
                continue;
            }

            // Non-taunter role: kill priority mobs + maintain DPS
            Core.KillWithPriority(boss, bowmaster, executioner);
            Bot.Skills.UseSkill(5);
        }
    }
}
