/*
name: Victor Matsuri Story (Pre-Masakado)
description: Runs Empress Ai No Miko's questline in /victormatsuri up to (but not including) the Masakado quest (10295). Used as a setup step before handing off to the dedicated Masakado army script. Quest 10295 must be handled separately via MasakadoKingsEchoArmy.
tags: victor matsuri, victormatsuri, empress ai no miko, story, pre-masakado, eclipse, rite of ascension
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class VictorMatsuriStory
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Storyline();
        Core.SetOptions(false);
    }

    /// <summary>
    /// Runs quests 10290–10294 in /victormatsuri. Quest 10295 (Masakado / Kanmu Heishi)
    /// is intentionally NOT handled here — call MasakadoKingsEchoArmy afterwards to
    /// accept, fight, and complete it.
    /// </summary>
    /// <param name="merge">If true, stops after 10291 (matches the original VictorMatsuri.Storyline merge gate).</param>
    public void Storyline(bool merge = false)
    {
        // If 10294 (the last quest we handle) is already done, there's nothing left for us.
        if (merge ? Core.isCompletedBefore(10291) : Core.isCompletedBefore(10294))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Kitsune Himawari",   // UseableMonsters[0]
            "NeOni",              // UseableMonsters[1]
            "Narcis Arrhythmia",  // UseableMonsters[2]
            "Haruki Matsuoka",    // UseableMonsters[3]
            "Lady Laidronette",   // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10290 | In Kuzunoha's Image
        if (!Story.QuestProgression(10290))
            Core.HuntMonsterQuest(10290, ("victormatsuri", UseableMonsters[0], ClassType.Solo));

        // 10291 | NeOni Blue
        if (!Story.QuestProgression(10291))
            Core.HuntMonsterQuest(10291, ("victormatsuri", UseableMonsters[1], ClassType.Solo));

        if (merge)
            return;

        // 10292 | Embodiment of Scarlet
        if (!Story.QuestProgression(10292))
            Core.HuntMonsterQuest(10292, ("victormatsuri", UseableMonsters[2], ClassType.Solo));

        // 10293 | Onihitokuchi
        if (!Story.QuestProgression(10293))
            Core.HuntMonsterQuest(10293, ("victormatsuri", UseableMonsters[3], ClassType.Solo));

        // 10294 | Tsukihime
        if (!Story.QuestProgression(10294))
            Core.HuntMonsterQuest(10294, ("victormatsuri", UseableMonsters[4], ClassType.Solo));

        // 10295 | Kanmu Heishi — intentionally NOT handled. Run MasakadoKingsEchoArmy next.
    }
}
