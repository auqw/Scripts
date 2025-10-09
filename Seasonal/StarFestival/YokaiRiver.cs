/*
name: Yokai River
description: This script will complete the storyline in /yokaistarriver.
tags: star, festival, yokai, yokaistarriver, river, story, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/StarFestival/Akiba.cs
using Skua.Core.Interfaces;

public class YokaiRiver
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static Akiba Akiba { get => _Akiba ??= new Akiba(); set => _Akiba = value; }
    private static Akiba _Akiba;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(6448) || !Core.isSeasonalMapActive("yokaistarriver"))
            return;

        Akiba.Storyline();

        Story.PreLoad(this);

        // Gather the Spirits (6443)
        Story.MapItemQuest(6443, "yokaistarriver", 5949, 5);
        Story.KillQuest(6443, "yokaistarriver", "Ghostly Chickencow");

        // Spiders and Dreamsilk (6444)
        Story.KillQuest(6444, "yokaistarriver", "Spirit Spider");

        // Dye-ing to Reunite (6445)
        Story.KillQuest(6445, "yokaistarriver", "Fallen Star");

        // Clear Water to Clear Skies (6446)
        Story.KillQuest(6446, "yokaistarriver", "Funa-yurei");

        // 1,000 Cranes to 1,000 Wishes (6447)
        Story.MapItemQuest(6447, "yokaistarriver", 5950, 18);

        // Slay Uji No Hashihime (6448)
        Story.KillQuest(6448, "yokaistarriver", "Uji No Hashihime");
    }
}
