/*
name: UltraAvatarTyndarius
description: Ultra Avatar Tyndarius helper with taunter rotation and orb priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraAvatarTyndarius
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string a, b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius";
    public List<IOption> Options = new()
    {
        new Option<string>("a", "First Taunter Class", "Insert the name of the class that will taunt", ""),
        new Option<string>("b", "Second Taunter Class", "Insert the name of the class that will taunt", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            Core.Log("Setup", "Fill both taunter classes in Script Options.");
            Bot.Stop(); return;
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
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_tyndarius.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive) Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            if (Bot.Map.Name != map)
                Core.Join(map);
            if (Bot.Player.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
            }
            if (Core.HasClassEquipped(a))
                Ultra.Taunt(a, boss, "aura", 250, "Focus");
            else if (Core.HasClassEquipped(b))
                Ultra.Taunt(b, boss, "aura", 700, "Focus");
            else
            {
                Ultra.KillWithPriority(boss, 2, "Ultra Fire Orb", 3, "Ultra Fire Orb", 1);
                Bot.Skills.UseSkill(5);
            }
            if (Bot.Player.Alive && !Ultra.MonsterAlive(boss))
                break;
        }
    }
}
