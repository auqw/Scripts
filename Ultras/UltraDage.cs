//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDage
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string a, b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
    public List<IOption> Options = new()
    {
        new Option<string>("a",   "First Taunter Class",  "Insert the name of the class that will taunt", ""),
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
        Bot.Events.ExtensionPacketReceived -= Ultra.UltraDageListener;
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        Bot.Events.ExtensionPacketReceived += Ultra.UltraDageListener;
        Bot.Quests.UpdateQuest(793);

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
        const string map = "ultradage";
        const string boss = "Dage the Dark Lord";

        Core.Join(map);
        Ultra.WaitForArmy(3, @"C:\SkuaSync\ultra_dage.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(a))
                Ultra.Taunt(a, boss, "aura", 250, "Focus");
            else if (Core.HasClassEquipped(b))
                Ultra.Taunt(b, boss, "aura", 700, "Focus");
            else
            {
                Core.Kill(boss);
                Bot.Skills.UseSkill(5);
            }
        }
    }
}
