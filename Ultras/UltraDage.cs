//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDage
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    string a, b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
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
            Core.Alert("Setup", "Fill both taunter classes in Script Options.");
            Bot.Stop(); return;
        }

        Core.Boot();
        Bot.Events.ExtensionPacketReceived += UltraDageListener;

        Bot.Quests.UpdateQuest(793);
        Prep();
        Fight();

        Bot.Events.ExtensionPacketReceived -= UltraDageListener;
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        Core.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        if (IsTaunter()) Core.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map = "ultradage";
        const string boss = "Dage the Dark Lord";

        Core.Join(map);
        Core.WaitForArmy(3);
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(a)) Core.TauntCycle(a, boss, "Focus", 250);
            else if (Core.HasClassEquipped(b)) Core.TauntCycle(b, boss, "Focus", 700);
            else
            {
                Core.Kill(boss);
                Bot.Skills.UseSkill(5);
            }
        }
    }

    async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json") return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event") return;

        string zone = data?.args?.zoneSet?.ToString();

        if (string.Equals(zone, "A", System.StringComparison.OrdinalIgnoreCase))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%122%411%8%"); return; }

        if (string.Equals(zone, "B", System.StringComparison.OrdinalIgnoreCase))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%856%422%8%"); return; }

        if (string.IsNullOrEmpty(zone))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%491%421%8%"); return; }
    }
}
