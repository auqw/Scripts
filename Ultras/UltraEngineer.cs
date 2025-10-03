//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraEngineer
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Fight();
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "ultraengineer";
        const string boss = "Ultra Engineer";
        const string priority1 = "Defense Drone";
        const string priority2 = "Attack Drone";

        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        Core.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        Core.Join(map);
        Core.WaitForArmy(3);
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            Core.KillWithPriority(boss, 3, priority1, 2, priority2, 1);
            Bot.Skills.UseSkill(5);
        }
    }
}
