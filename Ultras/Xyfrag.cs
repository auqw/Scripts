//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Xyfrag
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string taunter;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Xyfrag";
    public List<IOption> Options = new()
    {
        new Option<string>("taunter", "Taunter Class", "Insert the name of the class that will taunt", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        taunter = (Bot.Config.Get<string>("taunter") ?? "").Trim();
        if (string.IsNullOrEmpty(taunter))
        {
            Core.Log("Setup", "Fill the taunter class in Script Options.");
            Bot.Stop(); return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Events.ExtensionPacketReceived -= Ultra.GenericChargeListener;
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(taunter);

    void Prep()
    {
        if (IsTaunter())
        {
            Bot.Events.ExtensionPacketReceived += Ultra.GenericChargeListener;
            Ultra.GetScrollOfEnrage();
        }
        else
        {
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
    }

    void Fight()
    {
        const string map = "voidxyfrag";
        const string boss = "Xyfrag";

        Core.Join(map);
        Ultra.WaitForArmy(6, @"C:\SkuaSync\xyfrag_sync.txt");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunter)) Ultra.TauntCharge(taunter, boss, "Focus", 250);
            else { Core.Kill(boss); Bot.Skills.UseSkill(5); }
        }
    }
}
