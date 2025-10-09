/*
name: Atlas Kingdom Story
description: This will finish General Slaine's questline in /atlaskingdom.
tags: story, quest, legion,dage,atlas,kingdom,atlas-kingdom,general slaine,slaine,atlaskingdom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
using Skua.Core.Interfaces;

public class AtlasKingdom
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;
    private static AtlasPromenade AP { get => _AP ??= new AtlasPromenade(); set => _AP = value; }
    private static AtlasPromenade _AP;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }


    public void Storyline()
    {
        if (Core.isCompletedBefore(10125))
            return;

        AP.Storyline();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Atlas Knight", // UseableMonsters[0],
	"Atlas Light Magus", // UseableMonsters[1],
	"Coelho", // UseableMonsters[2],
	"Atlas Leo", // UseableMonsters[3],
	"Atlas Elite", // UseableMonsters[4],
	"Executioner Ladon", // UseableMonsters[5]
};
        #endregion Useable Monsters

        // 10116 | Knight's Honor
        Story.MapItemQuest(10116, "atlaskingdom", 14252);
        Story.KillQuest(10116, "atlaskingdom", UseableMonsters[0]);


        // 10117 | Drawn Light
        Story.MapItemQuest(10117, "atlaskingdom", new[] { 14253, 14254, 14255 });



        // 10118 | Sterile Life
        if (!Story.QuestProgression(10118))
        {
            Core.HuntMonsterQuest(10118,
                ("atlaskingdom", UseableMonsters[1], ClassType.Farm));
        }


        // 10119 | Lost Generations
        Story.MapItemQuest(10119, "atlaskingdom", new[] { 14256, 14257 });



        // 10120 | The Snitch
        if (!Story.QuestProgression(10120))
        {
            Core.HuntMonsterQuest(10120,
                ("atlaskingdom", UseableMonsters[2], ClassType.Solo));
        }


        // 10121 | Atlantides' Eve
        Story.MapItemQuest(10121, "atlaskingdom", new[] { 14258, 14259, 14260 });


        // 10122 | Lowly Form
        if (!Story.QuestProgression(10122))
        {
            Core.HuntMonsterQuest(10122,
                ("atlaskingdom", UseableMonsters[3], ClassType.Solo));
        }


        // 10123 | Enduring Sacrilege
        if (!Story.QuestProgression(10123))
        {
            Core.HuntMonsterQuest(10123,
                ("atlaskingdom", UseableMonsters[4], ClassType.Solo));
        }


        // 10124 | Blood of the Hesperides
        if (!Story.QuestProgression(10124))
        {
            Core.HuntMonsterQuest(10124,
                ("atlaskingdom", UseableMonsters[3], ClassType.Solo),
                ("atlaskingdom", UseableMonsters[4], ClassType.Solo));
        }


        // 10125 | Prota Sparasso
        if (!Story.QuestProgression(10125))
        {
            Core.HuntMonsterQuest(10125,
                ("atlaskingdom", UseableMonsters[5], ClassType.Solo));
        }


    }
}
