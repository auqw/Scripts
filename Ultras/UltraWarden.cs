//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWarden
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    string taunter;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraWarden";
    public List<IOption> Options = new()
    {
        new Option<string>("taunterClass", "Taunter Class", "Insert the name of the class that will taunt", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        taunter = (Bot.Config.Get<string>("taunterClass") ?? "").Trim();
        if (string.IsNullOrEmpty(taunter))
        {
            Core.Alert("Setup", "Fill the taunter class in Script Options.");
            Bot.Stop(); return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    void Prep()
    {
        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        if (Core.HasClassEquipped(taunter)) Core.GetScrollOfEnrage();
        else { Core.BuyAlchemyPotion("Potent Honor Potion"); Core.EquipConsumable("Potent Honor Potion"); }
    }

    void Fight()
    {
        const string map = "ultrawarden";
        const string boss = "Ultra Warden";

        Core.Join(map);
        Core.WaitForArmy(3);
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunter)) Core.UltraWardenTaunter();
            else Core.Kill(boss);
        }
    }
}
