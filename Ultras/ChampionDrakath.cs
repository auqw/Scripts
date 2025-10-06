//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ChampionDrakath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string a, b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath";
    public List<IOption> Options = new()
    {
        new Option<string>("a", "Taunter Class (Primary)", "Class name that will taunt first", ""),
        new Option<string>("b", "Taunter Class (Backup)",  "Backup taunter class", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();

        if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
        {
            Core.Log("Setup", "Fill at least one taunter class (Primary or Backup) in Script Options.");
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
        const string map = "championdrakath";
        const string boss = "Champion Drakath";

        Core.Join(map);
        Ultra.WaitForArmy(3, @"C:\SkuaSync\champion_drakath.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(a))
            {
                Ultra.DrakathTaunter();
            }
            else if (Core.HasClassEquipped(b))
            {
                Ultra.DrakathTaunter();
            }
            else
            {
                Core.Kill(boss);
                if (Core.GetTargetHealthPercentage() < 10)
                    Bot.Skills.UseSkill(5);
            }
            Bot.Sleep(250);
        }
    }
}
