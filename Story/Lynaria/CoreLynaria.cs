/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CoreLynaria
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void DoAll()
    {
        BocklinGrove();
        BocklinCastle();
        BocklinSanctum();
    }

    public void BocklinGrove()
    {
        if (Core.isCompletedBefore(10239))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Wolf", // UseableMonsters[0],
	"Spider", // UseableMonsters[1],
	"Sp-Eye", // UseableMonsters[2],
	"Garde Grif", // UseableMonsters[3],
	"Undead Garde", // UseableMonsters[4],
	"Garde Wraith", // UseableMonsters[5],
	"Forsaken Necromancer", // UseableMonsters[6],
	"Elder Necromancer", // UseableMonsters[7]
};
        #endregion Useable Monsters

        // 10230 | Sleeping Diana
        Story.MapItemQuest(10230, "bocklingrove", 14429);
        Story.KillQuest(10230, "bocklingrove", UseableMonsters[0]);


        // 10231 | At the Edge of the Forest
        if (!Story.QuestProgression(10231))
        {
            Core.HuntMonsterQuest(10231,
                ("bocklingrove", UseableMonsters[1], ClassType.Farm));
        }


        // 10232 | Freedom Helvetia
        if (!Story.QuestProgression(10232))
        {
            Core.HuntMonsterQuest(10232,
                ("bocklingrove", UseableMonsters[2], ClassType.Farm));
        }


        // 10233 | Sacred Grove
        Story.MapItemQuest(10233, "bocklingrove", new[] { 14430, 14431 });


        // 10234 | Chained Prometheus
        if (!Story.QuestProgression(10234))
        {
            Core.HuntMonsterQuest(10234,
                ("bocklingrove", UseableMonsters[3], ClassType.Farm));
        }


        // 10235 | Ride of Death
        if (!Story.QuestProgression(10235))
        {
            Core.HuntMonsterQuest(10235,
                ("bocklingrove", UseableMonsters[4], ClassType.Farm));
        }


        // 10236 | The Fall and Death
        if (!Story.QuestProgression(10236))
        {
            Core.HuntMonsterQuest(10236,
                ("bocklingrove", UseableMonsters[5], ClassType.Farm));
        }


        // 10237 | Ruins in Moonlight
        Story.MapItemQuest(10237, new[] {
        (14432,1,"bocklingrove"), (14433,5,"bocklingrove")});


        // 10238 | Wandering Light
        if (!Story.QuestProgression(10238))
        {
            Core.HuntMonsterQuest(10238,
                ("bocklingrove", UseableMonsters[5], ClassType.Farm),
                ("bocklingrove", UseableMonsters[4], ClassType.Farm));
        }


        // 10239 | Death Plays the Fiddle
        if (!Story.QuestProgression(10239))
        {
            Core.HuntMonsterQuest(10239,
                ("bocklingrove", UseableMonsters[7], ClassType.Solo));
        }
    }

    public void BocklinCastle()
    {
        if (Core.isCompletedBefore(10252))
            return;

        BocklinGrove();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Undead Garde", // UseableMonsters[0],
	"Garde Wraith", // UseableMonsters[1],
	"Warped Revenant", // UseableMonsters[2],
	"Forsaken Necromancer", // UseableMonsters[3],
	"Faceless Ritualist", // UseableMonsters[4],
	"Headless Knight", // UseableMonsters[5]
};
        #endregion Useable Monsters

        // 10243 | Roger and Angelica
        if (!Story.QuestProgression(10243))
        {
            Story.MapItemQuest(10243, "bocklincastle", 14446);
            Story.KillQuest(10243, "bocklincastle", UseableMonsters[0]);
        }

        // 10244 | The Way to Emmaus
        if (!Story.QuestProgression(10244))
        {
            Core.HuntMonsterQuest(10244,
                ("bocklincastle", UseableMonsters[1], ClassType.Farm));
        }


        // 10245 | Hermit
        Story.MapItemQuest(10245, "bocklincastle", new[] { 14451, 14447 });


        // 10246 | Ruins Near Kehl
        if (!Story.QuestProgression(10246))
        {
            Core.HuntMonsterQuest(10246,
                ("bocklincastle", UseableMonsters[0], ClassType.Farm),
                ("bocklincastle", UseableMonsters[1], ClassType.Farm));
        }


        // 10247 | Whistling Blackbird
        if (!Story.QuestProgression(10247))
        {
            Core.HuntMonsterQuest(10247,
                ("bocklincastle", UseableMonsters[2], ClassType.Farm));
        }


        // 10248 | Killer Pursued by Furies
        Story.MapItemQuest(10248, "bocklincastle", 14448);
        Story.KillQuest(10248, "bocklincastle", UseableMonsters[3]);


        // 10249 | Vestal
        Story.MapItemQuest(10249, "bocklincastle", 14449, 6);


        // 10250 | Abandoned Venus
        Story.MapItemQuest(10250, "bocklincastle", 14450);
        Story.KillQuest(10250, "bocklincastle", UseableMonsters[4]);


        // 10251 | Bacchanalia
        if (!Story.QuestProgression(10251))
        {
            Core.HuntMonsterQuest(10251,
                ("bocklincastle", UseableMonsters[2], ClassType.Farm),
                ("bocklincastle", UseableMonsters[4], ClassType.Farm));
        }


        // 10252 | Euterpe and Deer
        if (!Story.QuestProgression(10252))
        {
            Core.HuntMonsterQuest(10252,
                ("bocklincastle", UseableMonsters[5], ClassType.Solo));
        }


    }

    public void BocklinSanctum()
    {
        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Warped Revenant", // UseableMonsters[0],
            "Faceless Ritualist", // UseableMonsters[1],
            "Tarnished Scion", // UseableMonsters[2],
            "Ghostly Servant", // UseableMonsters[3],
            "Headless Rider", // UseableMonsters[4],
            "Thronekeeper", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        if (Core.isCompletedBefore(10265))
            return;

        BocklinCastle();
        Story.PreLoad(this);


        // 10256 | Shot in the Dark
        if (!Story.QuestProgression(10256))
        {
            Core.HuntMonsterQuest(10256,
                ("bocklinsanctum", UseableMonsters[0], ClassType.Farm));
        }


        // 10257 | Sparkling Changeling
        if (!Story.QuestProgression(10257))
        {
            Core.HuntMonsterQuest(10257,
                ("bocklinsanctum", UseableMonsters[1], ClassType.Farm));
        }


        // 10258 | Silver Proof
        if (!Story.QuestProgression(10258))
        {
            Story.MapItemQuest(10258, "bocklinsanctum", 14478, 6);
        }


        // 10259 | Gilded Peace
        if (!Story.QuestProgression(10259))
        {
            Core.HuntMonsterQuest(10259,
                ("bocklinsanctum", UseableMonsters[1], ClassType.Farm),
                ("bocklinsanctum", UseableMonsters[0], ClassType.Farm));
        }


        // 10260 | Towards Reunion
        if (!Story.QuestProgression(10260))
        {
            Core.EquipClass(ClassType.Solo);
            Story.MapItemQuest(10260, "bocklinsanctum", 14479);
            Story.KillQuest(10260, "bocklinsanctum", UseableMonsters[2]);
        }


        // 10261 | Servants Stay Unseen
        if (!Story.QuestProgression(10261))
        {
            Core.EquipClass(ClassType.Farm);
            Story.MapItemQuest(10261, "bocklinsanctum", 14480);
            Story.KillQuest(10261, "bocklinsanctum", UseableMonsters[3]);
        }


        // 10262 | The Slaying of Sacred Deer
        if (!Story.QuestProgression(10262))
        {
            Core.HuntMonsterQuest(10262,
                ("bocklinsanctum", UseableMonsters[4], ClassType.Farm));
        }


        // 10263 | The Light Protects You
        if (!Story.QuestProgression(10263))
        {
            Story.MapItemQuest(10263, "bocklinsanctum", new[] { 14481, 14482 });
        }


        // 10264 | Cure-All
        if (!Story.QuestProgression(10264))
        {
            Core.HuntMonsterQuest(10264,
                ("bocklinsanctum", UseableMonsters[3], ClassType.Solo),
                ("bocklinsanctum", UseableMonsters[4], ClassType.Solo));
        }

        // 10265 | The Thronekeeper
        if (!Story.QuestProgression(10265))
        {
            Core.HuntMonsterQuest(10265,
                ("bocklinsanctum", UseableMonsters[5], ClassType.Solo));
        }
    }
}
