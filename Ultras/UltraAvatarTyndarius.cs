//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraAvatarTyndarius
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    string a, b; // two taunters on boss
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius";
    public List<IOption> Options = new()
    {
        new Option<string>("primaryTaunter",   "First Taunter Class",  "Insert the name of the class that will taunt", ""),
        new Option<string>("secondaryTaunter", "Second Taunter Class", "Insert the name of the class that will taunt", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config.Get<string>("primaryTaunter") ?? "").Trim();
        b = (Bot.Config.Get<string>("secondaryTaunter") ?? "").Trim();
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
        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        if (IsTaunter()) Core.GetScrollOfEnrage();
        else { Core.BuyAlchemyPotion("Potent Honor Potion"); Core.EquipConsumable("Potent Honor Potion"); }
    }

    void Fight()
    {
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        Core.Join(map);
        Core.WaitForArmy(3, @"C:\SkuaSync\ultra_tyndarius_sync.txt");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(a)) Core.TauntCycle(a, boss, "Focus", 250);
            else if (Core.HasClassEquipped(b)) Core.TauntCycle(b, boss, "Focus", 700);
            else
            {
                Core.KillWithPriority(boss, 2, "Ultra Fire Orb", 3, "Ultra Fire Orb", 1);
                Bot.Skills.UseSkill(5);
            }
        }
    }
}
