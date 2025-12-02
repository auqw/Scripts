/*
name: UltraNulgath
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraNulgath
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraNulgath";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "a",
            "First Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
        new Option<string>(
            "b",
            "Second Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            Core.Log("Setup", "Fill both taunter classes in Script Options.");
            Bot.Stop();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else
        {
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
    }

    void Fight()
    {
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        const string blade = "Overfiend Blade";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_nulgath.sync");

        if (!IsTaunter())
            Bot.Sleep(5000);

        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Check if the whole army has finished
            if (Ultra.CheckArmyProgress("Nulgath the Archfiend Defeated?", 1, true, syncPath))
            {
                C.Logger("All players finished farm.");
                C.EnsureComplete(8692);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }
            if (Core.HasClassEquipped(a))
                Ultra.Taunt(a, boss, "aura", 250, "Focus");
            else if (Core.HasClassEquipped(b))
                Ultra.Taunt(b, boss, "aura", 700, "Focus");

            Core.KillWithPriority(boss, blade);
            Bot.Skills.UseSkill(5);
        }
    }
}
