/*
name: Akiba New Year DoAll
description: Completes all of the akiba new year quests
tags: seasonal, yokai, akibacny, akiba new year, story, bingwen, fausto, lengjing, senlin-mas, zhu, yokaihunt, ladyluna
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/AkibaNewYear/Bingwen.cs
//cs_include Scripts/Seasonal/AkibaNewYear/LadyLua.cs
//cs_include Scripts/Seasonal/AkibaNewYear/Zhu.cs
//cs_include Scripts/Seasonal/AkibaNewYear/YokaiHunt.cs
//cs_include Scripts/Seasonal/AkibaNewYear/SenlinMas.cs
//cs_include Scripts/Seasonal/AkibaNewYear/Parades.cs
using Skua.Core.Interfaces;

public class AkibaNewYear0DoAll
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    private static Bingwen Bingwen
    {
        get => _Bingwen ??= new Bingwen();
        set => _Bingwen = value;
    }
    private static Bingwen _Bingwen;
    private static LadyLua LadyLua
    {
        get => _LadyLua ??= new LadyLua();
        set => _LadyLua = value;
    }
    private static LadyLua _LadyLua;
    private static Parades Parades
    {
        get => _Parades ??= new Parades();
        set => _Parades = value;
    }
    private static Parades _Parades;
    private static YokaiHunt YokaiHunt
    {
        get => _YokaiHunt ??= new YokaiHunt();
        set => _YokaiHunt = value;
    }
    private static YokaiHunt _YokaiHunt;
    private static Zhu Zhu
    {
        get => _Zhu ??= new Zhu();
        set => _Zhu = value;
    }
    private static Zhu _Zhu;
    private static SenlinMas SenlinMas
    {
        get => _SenlinMas ??= new SenlinMas();
        set => _SenlinMas = value;
    }
    private static SenlinMas _SenlinMas;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        Bingwen.Storyline();
        LadyLua.Storyline();
        Parades.Storyline();
        YokaiHunt.DoAll();
        Zhu.Storyline();
        SenlinMas.Storyline();
    }
}
