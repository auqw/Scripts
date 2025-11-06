/*
name: Atlas Falls Story
description: This will finish Misaru's questline in /atlasfalls.
tags: story, quest, legion,dage,atlas,atlasfalls,atlas-atlasfalls,general slaine,slaine,atlasfalls
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
using Skua.Core.Interfaces;

public class AtlasFalls
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static AtlasKingdom AtlasKingdom { get => _AtlasKingdom ??= new AtlasKingdom(); set => _AtlasKingdom = value; }
    private static AtlasKingdom _AtlasKingdom;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }


    public void Storyline()
    {
        if (Core.isCompletedBefore(10135))
            return;

        AtlasKingdom.Storyline();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Atlas Knight", // UseableMonsters[0],
            "Atlas Light Magus", // UseableMonsters[1],
            "Atlas Light Mage", // UseableMonsters[2],
            "Ladon", // UseableMonsters[3],
            "Atlas Leo", // UseableMonsters[4],
            "Atlas Elite", // UseableMonsters[5],
            "Atlas Executioner", // UseableMonsters[6],
            "Sundered Soul", // UseableMonsters[7],
            "Princess Arethusa", // UseableMonsters[8],
            "King Zedek", // UseableMonsters[9]
        };
        #endregion Useable Monsters

        Core.EquipClass(ClassType.Solo);

        // 10127 | Between Two Worlds
        if (!Story.QuestProgression(10127))
        {
            Story.MapItemQuest(10127, "atlasfalls", new[] { 14283, 14284 });
        }

        // 10128 | Exodus of Dust
        if (!Story.QuestProgression(10128))
        {
            Core.HuntMonsterQuest(10128,
                ("atlasfalls", UseableMonsters[0], ClassType.Solo),
                ("atlasfalls", UseableMonsters[2], ClassType.Solo));
        }


        // 10129 | Impeccable Judgement
        if (!Story.QuestProgression(10129))
        {
            Core.EnsureAccept(10129);
            Core.HuntMonster("atlasfalls", UseableMonsters[3], "Ladon's Shreds", publicRoom: true);
            Story.MapItemQuest(10129, "atlasfalls", 14285, 4);
        }


        // 10130 | Koinà Tà Phílon
        if (!Story.QuestProgression(10130))
        {
            Story.MapItemQuest(10130, "atlasfalls", new[] { 14286, 14287 });
            Bot.Wait.ForCellChange("r5");
        }


        // 10131 | What Walks Ought to Crawl
        if (!Story.QuestProgression(10131))
        {

            Core.EnsureAccept(10131);
            Core.HuntMonster("atlasfalls", UseableMonsters[4], "Leo Heart", 8, publicRoom: true);
            Story.MapItemQuest(10131, "atlasfalls", 14288, 4);
        }


        // 10132 | Vene Vidi Vici
        if (!Story.QuestProgression(10132))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(10132);
            Core.KillMonster("atlasfalls", "r7", "Right", UseableMonsters[5], "Elite's Skull", 8, publicRoom: true);
            Story.MapItemQuest(10132, "atlasfalls", 14289);
        }


        // 10133 | Exocannibalism
        if (!Story.QuestProgression(10133))
        {
            Core.HuntMonsterQuest(10133,
                ("atlasfalls", UseableMonsters[6], ClassType.Farm));
        }

        // 10134 | Condemnatio Purgatorio
        if (!Story.QuestProgression(10134))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(10134);
            Core.HuntMonster("atlasfalls", UseableMonsters[7], "Soul Condemned", 21, publicRoom: true);
            Story.MapItemQuest(10134, "atlasfalls", 14290, 4);
        }


        // 10135 | Miserum Votum
        if (!Story.QuestProgression(10135))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(10135);
            Core.HuntMonster("atlasfalls", UseableMonsters[8], "Arethusa's Crown", publicRoom: true);
            Story.MapItemQuest(10135, "atlasfalls", 14291);
        }


        // ----- dude isnt soloable. ----- yes he is
        // 10136 | Dante to Beatrice
        if (!Story.QuestProgression(10136))
        {
            if (Core.CheckInventory("Chaos Avenger"))
            {
                Core.UseBossClass();
                Story.KillQuest(10136, "atlasfalls", UseableMonsters[9]);
            }
            else Core.Logger("You need to have \"Chaos Avenger\" to kill \"King Zedek\".");

        }

    }
}
