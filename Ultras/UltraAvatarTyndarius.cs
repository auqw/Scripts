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
        a = (Bot.Config.Get<string>("a") ?? "").Trim();
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
        Ultra.WaitForArmy(3, @"C:\SkuaSync\ultra_tyndarius_sync.txt");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(a)) Ultra.TauntCycle(a, boss, "Focus", 250);
            else if (Core.HasClassEquipped(b)) Ultra.TauntCycle(b, boss, "Focus", 700);
            else
            {
                Ultra.KillWithPriority(boss, 2, "Ultra Fire Orb", 3, "Ultra Fire Orb", 1);
                Bot.Skills.UseSkill(5);
            }
        }
    }
}
