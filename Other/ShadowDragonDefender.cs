/*
name: Shadow Dragon Defender
description: if "Manticore Cub Pet" is owned, it will buy "Shadow Dragon Defender"
tags: shadow dragon defender, manticore cub pet, armor
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Other/MysteriousEgg.cs
using Skua.Core.Interfaces;

public class GetSDD
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static HatchTheEgg Egg
    {
        get => _Egg ??= new HatchTheEgg();
        set => _Egg = value;
    }
    private static HatchTheEgg _Egg;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        ShadowDragonDefender();

        Core.SetOptions(false);
    }

    public void ShadowDragonDefender()
    {
        if (Core.CheckInventory("Shadow Dragon Defender"))
            return;

        Egg.Hatch();

        Core.BuyItem("mysteriousegg", 1728, "Shadow Dragon Defender");
        Core.ToBank("Manticore Cub Pet");
    }
}
