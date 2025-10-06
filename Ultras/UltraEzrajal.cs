//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraEzrajal
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Fight();
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        Core.Join(map);
        Ultra.WaitForArmy(3, @"C:\SkuaSync\ultra_ezrajal.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Ultra.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasAura("Counter Attack"))
                Ultra.WaitForAuraFade("Counter Attack");
            else
                Core.Kill(boss);
        }
    }
}
