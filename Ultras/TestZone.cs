//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Linq;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TestZone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Test();

        Bot.Stop();
    }

    void Test()
    {
        // priority => ...
        //Core.ChooseBestEnhancement("Weapon", "Valiance", "Spiral Carve", "Fighter");
        //Core.ChooseBestEnhancement("Helm", "Pneuma", "Wizard");
        //Core.ChooseBestEnhancement("Cape", "Vainglory", "Wizard");

        //Core.ChooseBestGear("*"); // for the most common race in the map
        //Core.ChooseBestGear("Random Monster"); // for a specific monster
        //Core.BuyAlchemyPotion("Potent Honor Potion");
        //Core.BuyItem("Shriekward Potion", 774, "mirrorportal", 30);
        //Core.ForItem("Onyx Lava Dragon", "lair", "Celestial Staff", useBestGear: true);
        Core.ForItem("Boss Dummy", "classhall", "Celestial Staff");
        //Core.GetScrollOfEnrage();
        //Core.GetScrollOfDecay();
    }
}
