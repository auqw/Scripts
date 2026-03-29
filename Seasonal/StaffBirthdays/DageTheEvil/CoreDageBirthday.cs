/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;

public class CoreDageBirthday
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Core.RunCore();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        if (!Core.isSeasonalMapActive("darkbirthday"))
            return;

        DarkPath();
        FutureLegion();
        Undervoid();
        LegionBarracks();
        CocytusBarracks();
        LegionTournament();
        Phlegethonarena();
        Reapfinals();

        Core.Logger("All Dage Birthday quests have been completed.");
    }

    public void DarkPath()
    {
        if (Core.isCompletedBefore(6234))
            return;

        if (!Core.isSeasonalMapActive("darkpath"))
            return;

        Story.PreLoad(this);

        //Soul Energy (6220)
        Story.KillQuest(6220, "darkpath", "Dark Makai");

        //Open the Portal (6221)
        Story.MapItemQuest(6221, "darkpath", 5663, 5);
        Story.KillQuest(6221, "darkpath", "Void Elemental");

        //Go Through the Portal (6222)
        Story.MapItemQuest(6222, "darkpath", 5664);

        //We Need a Guide (6223)
        if (!Story.QuestProgression(6223))
        {
            Core.EnsureAccept(6223);
            // there are 2 "Void Energy" - `40070` [Wrong] & `43068` [Correct]
            while (!Bot.ShouldExit && !Core.CheckInventory(43068, 10))
                Core.KillMonster("darkpath", "r7", "Left", "Void Makai", log: false);
            Core.EnsureComplete(6223);
        }

        //Darkness is Energy (6224)
        Story.KillQuest(6224, "darkpath", "Void Elemental");

        //Reach the Vault (6225)
        Story.MapItemQuest(6225, "darkpath", 5665);

        //Open the Vault (6226)
        if (!Story.QuestProgression(6226))
        {
            Core.EnsureAccept(6226);
            Core.KillMonster("darkpath", "r7", "Left", "Void Makai", "Void Makai Slain", 10);
            Core.KillMonster("darkpath", "r8a", "Left", "Void Wyrm", "Key to the Vault");
            Story.MapItemQuest(6226, "darkpath", 5666);
            Bot.Wait.ForQuestComplete(6226);
            Bot.Wait.ForTrue(() => Bot.Player.Cell.ToLower().Contains("cut"), 20);
            Core.JumpWait();
        }

        //Examine the Souls (6227)
        Story.KillQuest(6227, "darkpath", "Wandering Soul");

        //Find Another Path (6228)
        Story.MapItemQuest(6228, "darkpath", 5667);

        //More Energy for Zep (6229)
        Story.KillQuest(6229, "darkpath", "Void Makai");

        //Pass Through the Shadows (6230)
        Story.MapItemQuest(6230, "darkpath", 5668, 5);
        Story.KillQuest(6230, "darkpath", new[] { "Void Makai", "Void Elemental" });

        //Find Contract (6231)
        Story.MapItemQuest(6231, "darkpath", 5669);

        //Battle the Void Army (6233)
        Story.MapItemQuest(6233, "voidvault", 5673);
        Story.KillQuest(6233, "voidvault", "Void Knight");

        //Defeat Zeph'gorog (6234)
        Story.KillQuest(6234, "voidvault", "Zeph'gorog");
    }

    public void FutureLegion()
    {
        if (Core.isCompletedBefore(5736))
            return;

        if (!Core.isSeasonalMapActive("futurelegion"))
            return;

        Story.PreLoad(this);

        //Examine the Area 5724
        Story.MapItemQuest(5724, "futurelegion", new[] { 5162, 5163, 5164 });
        Story.KillQuest(5724, "futurelegion", "UW3017 Gunner");

        //Get the Key 5725
        Story.KillQuest(5725, "futurelegion", "UW3017 Gunner");

        //Obtain Agravh's Soul 5726
        Story.MapItemQuest(5726, "futurelegion", 5165);
        Story.KillQuest(5726, "futurelegion", "Commander Agravh");

        //Obtain Uslaw's Soul 5727
        Story.MapItemQuest(5727, "futurelegion", 5166);
        Story.KillQuest(5727, "futurelegion", "Commander Uslaw");

        //Access the Control Room 5728
        Story.MapItemQuest(5728, "futurelegion", 5167);

        //Destory the Force Field 5729
        Story.MapItemQuest(5729, "futurelegion", 5168);
        Story.KillQuest(5729, "futurelegion", "UW3017 Blaster");

        //Obtain Ozar's Soul 5730
        Story.KillQuest(5730, "futurelegion", "Commander Ozar");

        //Obtain Pavon's Soul 5731
        Story.KillQuest(5731, "futurelegion", "Commander Pavon");

        //Activate the Teleporter 5732
        Story.MapItemQuest(5732, "futurelegion", 5169);

        //Keep It Grounded 5733
        Story.MapItemQuest(5733, "futurelegion", 5170, 7);
        Story.KillQuest(5733, "futurelegion", "SF3017 Gunner");

        //Get the Code 5734
        Story.KillQuest(5734, "futurelegion", new[] { "SF3017 Gunner", "SF3017 Blade" });

        //Open the Door 5735
        Story.MapItemQuest(5735, "futurelegion", 5171);

        //Take out the Legionator 5736
        Story.KillQuest(5736, "futurelegion", "Legionator");
    }

    public void Undervoid()
    {
        if (Core.isCompletedBefore(3406))
            return;

        if (!Core.isSeasonalMapActive("undervoid"))
            return;

        Story.PreLoad(this);

        Core.AddDrop("Hollowborn Soul Stealer");

        //Dark, Deadly Warmup
        Story.KillQuest(3399, "underworld", "Dark Makai");

        //Destroy the Good
        Story.KillQuest(3400, "alliance", "Good Soldier");

        //Destroy Chaorrupted Evil
        Story.KillQuest(3401, "alliance", "Chaorrupted Evil Soldier");

        //Legion Fenrir Gauntlet
        Story.KillQuest(3402, "underworld", "Legion Fenrir");

        //Conquer Conquest
        Story.KillQuest(3403, "undervoid", "Conquest");

        //Conquer War
        Story.KillQuest(3404, "undervoid", "War");

        //Conquer Famine
        Story.KillQuest(3405, "undervoid", "Famine");

        //Conquer Death
        Story.KillQuest(3406, "undervoid", "Death");
    }

    public void LegionBarracks()
    {
        if (Core.isCompletedBefore(9619))
            return;

        if (!Core.isSeasonalMapActive("legionbarracks"))
            return;

        Story.PreLoad(this);

        // Backroom Whispers (9611)
        Story.KillQuest(9611, "legionbarracks", "Legion Evocator");

        // Cur Dolorem Sentis (9612)
        Story.MapItemQuest(9612, "legionbarracks", 12774, 3);

        // Genuine Character (9608)
        Story.KillQuest(9608, "legionbarracks", "Legion Knight");

        // Seen the Light (9613)
        Story.KillQuest(9613, "legionbarracks", "Sullied Master");

        // Bad Beef (9614)
        Story.KillQuest(9614, "legionbarracks", "Off-Duty Minos");

        // Tomb of Memories (9615)
        Story.KillQuest(9615, "legionbarracks", "Enlightened Master");

        // Reap What's Sown (9616)
        Story.MapItemQuest(9616, "legionbarracks", 12775);
        Story.KillQuest(9616, "legionbarracks", "Overdriven paladin");

        // Wails of Unrest (9617)
        Story.KillQuest(9617, "legionbarracks", new[] { "Legion Evocator", "Legion Knight" });

        // Lamenting Aestiua (9618)
        Story.MapItemQuest(9618, "legionbarracks", new[] { 12776, 12777 });

        // Unblemished Snow (9619)
        if (!Core.isCompletedBefore(9619))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9619, "legionbarracks", "Paladin Arondight");
        }
    }

    public void CocytusBarracks()
    {
        if (Core.isCompletedBefore(9632))
            return;

        if (!Core.isSeasonalMapActive("cocytusbarracks"))
            return;

        LegionBarracks();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Cocytus Wails (9623)
        Story.KillQuest(9623, "cocytusbarracks", new[] { "Legion Evocator", "Legion Knight" });

        // Light Up Ice Cube (9624)
        Story.MapItemQuest(9624, "cocytusbarracks", 12798, 6);

        // Blessed Blind (9625)
        Story.KillQuest(9625, "cocytusbarracks", "Overdriven Evocator");

        // Falsified Hope (9626)
        Story.KillQuest(9626, "cocytusbarracks", "Overdriven Knight");

        // Saint of the Battlefield (9627)
        Story.MapItemQuest(9627, "cocytusbarracks", 12799);
        Story.KillQuest(9627, "cocytusbarracks", "Overdriven Paladin");

        // River Bends (9628)
        Story.MapItemQuest(9628, "cocytusbarracks", 12800);
        Story.KillQuest(9628, "cocytusbarracks", new[] { "Legion Evocator", "Legion Knight" });

        // Styx and Stones (9629)
        Story.KillQuest(
            9629,
            "cocytusbarracks",
            new[] { "Overdriven Knight", "Overdriven Evocator" }
        );

        // Cry Me a River (9630)
        Story.KillQuest(9630, "cocytusbarracks", "Mourner");

        // Boiling Blood (9631)
        Story.KillQuest(9631, "cocytusbarracks", "Cerberus Pup");

        // The Knight of Summer Set (9632)
        if (!Core.isCompletedBefore(9632))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9632, "cocytusbarracks", "Maleagant");
        }


    }

    public void LegionTournament()
    {
        if (Core.isCompletedBefore(10634) || !Core.isSeasonalMapActive("legiontournament"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Underworld Wolf", // UseableMonsters[0],
	"Legion Gladiator", // UseableMonsters[1],
	"Legion Willbreaker", // UseableMonsters[2],
	"Legion Ritualist", // UseableMonsters[3],
	"Deathwing", // UseableMonsters[4],
	"The WarForge", // UseableMonsters[5]
};
        #endregion Useable Monsters

        // 10625 | Dog Nights of Summer
        if (!Story.QuestProgression(10625))
        {
            Core.HuntMonsterQuest(10625,
                ("legiontournament", UseableMonsters[0], ClassType.Farm));
        }


        // 10626 | Achilles Tendon
        if (!Story.QuestProgression(10626))
        {
            Core.HuntMonsterQuest(10626,
                ("legiontournament", UseableMonsters[1], ClassType.Farm));
        }


        // 10627 | Disciplinary Action
        if (!Story.QuestProgression(10627))
        {
            Core.HuntMonsterQuest(10627,
                ("legiontournament", UseableMonsters[0], ClassType.Farm),
                ("legiontournament", UseableMonsters[1], ClassType.Farm));
        }


        // 10628 | The Fun Police
        if (!Story.QuestProgression(10628))
        {
            Story.MapItemQuest(10628, "legiontournament", 15584);
            Story.KillQuest(10628, "legiontournament", UseableMonsters[2]);
        }


        // 10629 | Despair-ity
        if (!Story.QuestProgression(10629))
        {
            Story.MapItemQuest(10629, "legiontournament", 15585);
            Story.KillQuest(10629, "legiontournament", UseableMonsters[3]);
        }


        // 10630 | Bad Charms
        if (!Story.QuestProgression(10630))
        {
            Story.MapItemQuest(10630, "legiontournament", 15586, 6);
        }


        // 10631 | Self-Appointed Heel
        if (!Story.QuestProgression(10631))
        {
            Core.EquipClass(ClassType.Solo);
            Story.MapItemQuest(10631, "legiontournament", 15587);
            Story.KillQuest(10631, "legiontournament", UseableMonsters[4]);
        }


        // 10632 | Simple Delights
        if (!Story.QuestProgression(10632))
        {
            Story.MapItemQuest(10632, "legiontournament", 15588);
            Story.MapItemQuest(10632, "legiontournament", 15589);
        }


        // 10633 | Vultures of a Feather
        if (!Story.QuestProgression(10633))
        {
            Core.HuntMonsterQuest(10633,
                ("legiontournament", UseableMonsters[3], ClassType.Solo),
                ("legiontournament", UseableMonsters[4], ClassType.Solo));
        }


        // 10634 | All's Fair
        if (!Story.QuestProgression(10634))
        {
            Core.HuntMonsterQuest(10634,
                ("legiontournament", UseableMonsters[5], ClassType.Solo));
        }


    }


    public void Phlegethonarena()
    {
        if (Core.isCompletedBefore(10646) || !Core.isSeasonalMapActive("PhlegethonArena"))
            return;

        LegionTournament();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
            {
        "Cerberus pup", // UseableMonsters[0],
        "Legion Willbreaker", // UseableMonsters[1],
        "Underworld Wolf", // UseableMonsters[2],
        "Abyssal Underbeast", // UseableMonsters[3],
        "Legion Guard", // UseableMonsters[4],
        "Deathwings", // UseableMonsters[5],
        "Underworld Wolf Warrior", // UseableMonsters[6],
        "Devourer of Souls", // UseableMonsters[7],
        "Rayce ", // UseableMonsters[8],
        "Warped Revenant", // UseableMonsters[9],
        "Horseman of Death", // UseableMonsters[10]
    };
        #endregion Useable Monsters

        // 10637 | Sprint to the Starting Line
        if (!Story.QuestProgression(10637))
        {
            Core.HuntMonsterQuest(10637,
                ("Phlegethonarena", UseableMonsters[0], ClassType.Farm));
        }


        // 10638 | Broken Androktasia
        if (!Story.QuestProgression(10638))
        {
            Core.HuntMonsterQuest(10638,
                ("Phlegethonarena", UseableMonsters[1], ClassType.Farm));
        }


        // 10639 | Phlegethon Watchman
        if (!Story.QuestProgression(10639))
        {
            Story.MapItemQuest(10639, "Phlegethonarena", 15610);
            Core.HuntMonsterQuest(10639,
                ("Phlegethonarena", UseableMonsters[1], ClassType.Farm),
                ("Phlegethonarena", UseableMonsters[0], ClassType.Farm));
        }


        // 10640 | Loophole Abuse
        if (!Story.QuestProgression(10640))
        {
            Core.HuntMonsterQuest(10640,
                ("Phlegethonarena", UseableMonsters[2], ClassType.Farm));
        }


        // 10641 | Man of the Pack
        if (!Story.QuestProgression(10641))
        {
            Core.HuntMonsterQuest(10641,
                ("Phlegethonarena", UseableMonsters[6], ClassType.Solo));
        }


        // 10642 | The Ferryman's Offer
        if (!Story.QuestProgression(10642))
        {
            Story.MapItemQuest(10642, "Phlegethonarena", 15611);
            Core.HuntMonsterQuest(10642,
                ("Phlegethonarena", UseableMonsters[3], ClassType.Farm));
        }


        // 10643 | Sloth or Pride
        if (!Story.QuestProgression(10643))
        {
            Core.Logger("hey so appearntly this dude hits like a truck without the pot.. so use something like YNR for your dodge class in CBO");
            Core.HuntMonsterQuest(10643,
                ("Phlegethonarena", UseableMonsters[7], ClassType.Dodge));
        }


        // 10644 | Remember Rayce?
        if (!Story.QuestProgression(10644))
        {
            Core.HuntMonsterQuest(10644,
                ("Phlegethonarena", UseableMonsters[8], ClassType.Solo));
        }


        // 10645 | Suspicious Distraction
        if (!Story.QuestProgression(10645))
        {
            Core.HuntMonsterQuest(10645,
                ("Phlegethonarena", UseableMonsters[5], ClassType.Farm),
                ("Phlegethonarena", UseableMonsters[4], ClassType.Farm));
        }

        // 10646 | Death's Revelation
        if (!Story.QuestProgression(10646))
        {
            Core.HuntMonsterQuest(10646,
                ("Phlegethonarena", UseableMonsters[10], ClassType.Solo));
        }



    }

    public void Reapfinals()
    {
        Phlegethonarena();

        if (Core.isCompletedBefore(10663) || !Core.isSeasonalMapActive("ReapFinals"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
        "Legion Gladiator", // UseableMonsters[0],
        "Legion Knight", // UseableMonsters[1],
        "Legion Willbreaker", // UseableMonsters[2],
        "Deathwings", // UseableMonsters[3],
        "Vulcar", // UseableMonsters[4],
        "Seneschal", // UseableMonsters[5],
        "Legion Ritualist", // UseableMonsters[6],
        "Ritualist Leader", // UseableMonsters[7],
        "The Crimson Rider", // UseableMonsters[8],
        "Rhadamanthys", // UseableMonsters[9],
        "Aeacus", // UseableMonsters[10],
        "Minos", // UseableMonsters[11],
        "Deimos", // UseableMonsters[12],
        "Laken Clone 1.7", // UseableMonsters[13],
        "Laken Clone 2.3", // UseableMonsters[14],
        "The Black Rider", // UseableMonsters[15],
        "General Vaughn", // UseableMonsters[16]
    };
        #endregion Useable Monsters

        // 10654 | Straightforward Wrath
        if (!Story.QuestProgression(10654))
        {
            Core.HuntMonsterQuest(10654,
                ("reapfinals", UseableMonsters[4], ClassType.Solo));
        }

        // 10655 | Sinister Assist
        if (!Story.QuestProgression(10655))
        {
            Core.HuntMonsterQuest(10655,
                ("reapfinals", UseableMonsters[5], ClassType.Solo));
        }

        // 10656 | Disposer of Lots
        if (!Story.QuestProgression(10656))
        {
            Core.EnsureAccept(10656);
            Story.MapItemQuest(10656, "reapfinals", 15638);

            if (Bot.Map.Name != "reapfinals")
                Core.Join("reapfinals");
            if (Bot.Player?.Cell != "Arena3")
                Core.Jump("Arena3");
        Retry:
            Monster CultLeader = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 30);

            while (!Bot.ShouldExit && CultLeader.State == 2 /* invulnerable */)
            {
                Core.KillMonster("reapfinals", "arena3", "Bottom", "Legion Ritualist");
                // Re-snapshop leader for state
                CultLeader = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 30);
                if (CultLeader.State != 2)
                {
                    Core.KillMonster("reapfinals", "arena3", "Bottom", "Ritualist Leader");
                    if (Bot.TempInv.Contains("Ritualist's Candle"))
                    {
                        Core.JumpWait();
                        break;
                    }
                    else goto Retry;
                }
            }
            Core.EnsureComplete(10656);
        }

        // 10657 | War's Revelation
        if (!Story.QuestProgression(10657))
        {
            Core.HuntMonsterQuest(10657,
                ("reapfinals", UseableMonsters[8], ClassType.Solo));
        }

        // 10658 | Judges' Grudges
        if (!Story.QuestProgression(10658))
        {
            Core.EnsureAccept(10658);
            Core.EquipClass(ClassType.Solo);

            if (Bot.Map.Name != "reapfinals")
                Core.Join("reapfinals");
            if (Bot.Player?.Cell != "Arena5")
                Core.Jump("Arena5");

            Monster Rhadamanthys = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 35);
            Monster Minos = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 37);
            Monster Aeacus = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 36);

            while (!Bot.ShouldExit)
            {
                if (Bot.Map.Name != "reapfinals")
                    Core.Join("reapfinals");
                if (Bot.Player?.Cell != "Arena5")
                    Core.Jump("Arena5");

                Rhadamanthys = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 35);
                Minos = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 37);
                Aeacus = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 36);

                if (Rhadamanthys.Alive)
                    Bot.Combat.Attack("Rhadamanthys");
                else if (Minos.Alive)
                    Bot.Combat.Attack("Minos");
                else
                    Bot.Combat.Attack("Aeacus");
                Bot.Sleep(500);

                if (Bot.TempInv.Contains("Minos Defeated") && Bot.TempInv.Contains("Rhadamanthys Defeated") && Bot.TempInv.Contains("Aeacus Defeated"))
                {
                    Core.JumpWait();
                    break;
                }
            }
            Core.EnsureComplete(10658);
        }


        // 10659 | Devastated Deimos
        if (!Story.QuestProgression(10659))
        {
            Core.HuntMonsterQuest(10659,
                ("reapfinals", UseableMonsters[12], ClassType.Solo));
        }

        // 10660 | Laken, the Sequel
        if (!Story.QuestProgression(10660))
        {
            Core.HuntMonsterQuest(10660,
                ("reapfinals", UseableMonsters[14], ClassType.Solo),
                ("reapfinals", UseableMonsters[13], ClassType.Solo));
        }


        // 10661 | Take Ill
        if (!Story.QuestProgression(10661))
        {
            Story.MapItemQuest(10661, "reapfinals", 15639, 2);
            Story.MapItemQuest(10661, "reapfinals", 15640);
        }

        // 10662 | Pallidus Curse
        if (!Story.QuestProgression(10662))
        {
            Story.MapItemQuest(10662, "reapfinals", 15641, 8);
            Core.HuntMonsterQuest(10662,
                ("reapfinals", UseableMonsters[0], ClassType.Farm));
        }

        // 10663 | Famine's Revelation
        if (!Story.QuestProgression(10663))
        {
            Core.HuntMonsterQuest(10663,
                ("reapfinals", UseableMonsters[16], string.IsNullOrEmpty(Core.BossClass) ? ClassType.Solo : ClassType.Boss));


            Core.EnsureAccept(10658);
            Core.EquipClass(ClassType.Solo);

            if (Bot.Map.Name != "reapfinals")
                Core.Join("reapfinals");
            if (Bot.Player?.Cell != "Arena8")
                Core.Jump("Arena8", "Bottom");

            Monster TheBlackRider = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 41);
            Monster GeneralVaughn = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 42);

            while (!Bot.ShouldExit)
            {
                if (Bot.Map.Name != "reapfinals")
                    Core.Join("reapfinals");
                if (Bot.Player?.Cell != "Arena5")
                    Core.Jump("Arena5");

                TheBlackRider = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 41);
                GeneralVaughn = Bot.Monsters.CurrentAvailableMonsters.Find(x => x != null && x.MapID == 42);

                if (GeneralVaughn.Alive)
                    Bot.Combat.Attack(42);
                else
                    Bot.Combat.Attack(41);
                Bot.Sleep(500);

                if (Bot.TempInv.Contains(100145))
                {
                    Core.JumpWait();
                    break;
                }
            }
            Core.EnsureComplete(10658);

        }
    }

}