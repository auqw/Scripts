/*
name: AbyssalAngelsShadow
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class AbyssalAngelsShadow
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
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAbyssal();

        Core.SetOptions(false);
    }

    public void GetAbyssal(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Abyssal Angel Shadow"))
            return;

        if (!Core.CheckInventory(34584))
        {
            Core.Logger($"This bot requires \"Abyssal Angel [34584]\", stopping the bot");
            return;
        }

        Adv.RankUpClass("Abyssal Angel");
        Core.BuyItem("curio", 1657, "Abyssal Angel Shadow");

        if (rankUpClass)
            Adv.RankUpClass("Abyssal Angel Shadow");
    }
}
