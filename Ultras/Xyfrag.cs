//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Xyfrag
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    string taunter;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Xyfrag";
    public List<IOption> Options = new()
    {
        new Option<string>("taunterClass", "Taunter Class", "Insert the name of the class that will taunt", "")
    };

    public void ScriptMain(IScriptInterface bot)
    {
        taunter = (Bot.Config.Get<string>("taunterClass") ?? "").Trim();
        if (string.IsNullOrEmpty(taunter))
        {
            Core.Log("Setup", "Fill the taunter class in Script Options.");
            Bot.Stop(); return;
        }

        Core.Boot();
        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= Core.ChargeListener;
        }
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(taunter);

    void Prep()
    {
        if (IsTaunter())
            Core.GetScrollOfEnrage();
        else
        {
            Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
            Core.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
    }

    void Fight()
    {
        const string map = "voidxyfrag";
        const string boss = "Xyfrag";

        Core.Join(map);
        Core.WaitForArmy(6, @"C:\SkuaSync\xyfrag_sync.txt");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunter)) Core.TauntCharge(taunter, boss, "Focus", 250);
            else { Core.Kill(boss); Bot.Skills.UseSkill(5); }
        }
    }
}
