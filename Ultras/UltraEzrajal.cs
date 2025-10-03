//cs_include Scripts/Ultras/CoreUltras.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraEzrajal
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
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

        Core.UseAlchemyPotions(Core.GetBestTonicPotion(), Core.GetBestElixirPotion());
        Core.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        Core.Join(map);
        Core.WaitForArmy(3, @"C:\SkuaSync\ultra_ezrajal_sync.txt");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (Core.MonsterAlive(boss) && !Bot.ShouldExit)
        {
            if (Core.HasAura("Counter Attack")) Core.DontAttack();
            else Core.Kill(boss);
        }
    }
}
