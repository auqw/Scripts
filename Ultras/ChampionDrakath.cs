//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ChampionDrakath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    string taunterPrimary;
    string taunterBackup;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath";
    public List<IOption> Options = new()
    {
        new Option<string>("taunterClass", "Taunter Class (Primary)", "Class name that will taunt first", ""),
        new Option<string>("taunterBackupClass", "Taunter Class (Backup)",  "Backup taunter class", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        taunterPrimary = (Bot.Config.Get<string>("taunterClass") ?? "").Trim();
        taunterBackup = (Bot.Config.Get<string>("taunterBackupClass") ?? "").Trim();

        if (string.IsNullOrEmpty(taunterPrimary) && string.IsNullOrEmpty(taunterBackup))
        {
            Core.Alert("Setup", "Fill at least one taunter class (Primary or Backup) in Script Options.");
            Bot.Stop();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    void Prep()
    {
        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        Core.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (Core.HasClassEquipped(taunterPrimary) || Core.HasClassEquipped(taunterBackup))
            Core.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map = "championdrakath";
        const string boss = "Champion Drakath";

        Core.Join(map);
        Core.WaitForArmy(3);
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunterPrimary))
            {
                Core.DrakathTaunter();
            }
            else if (Core.HasClassEquipped(taunterBackup))
            {
                Core.DrakathTaunter();
            }
            else
            {
                Core.Kill(boss);
                if (Core.GetTargetHealthPercentage() < 10)
                    Bot.Skills.UseSkill(5);
            }
        }
    }
}
