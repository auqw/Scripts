/*
name: Helm of Awe
description: helm, awe, gear, reputation, gold, experience, class, points, cp, boost
tags: this bot will farm the helm of awe for you
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class HelmOfAwe
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAwe Awe
    {
        get => _Awe ??= new CoreAwe();
        set => _Awe = value;
    }
    private static CoreAwe _Awe;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetHoA();

        Core.SetOptions(false);
    }

    public void GetHoA()
    {
        if (Core.CheckInventory("Helm of Awe"))
            return;

        Awe.GetAweRelic("Helm", 4175, 10, 5, "doomvaultb", "Undead Raxgore");
        Core.BuyItem("museum", 1129, "Helm of Awe");

        Core.ToBank("Legendary Awe Pass", "Guardian Awe Pass", "Armor of Awe Pass");
    }
}
