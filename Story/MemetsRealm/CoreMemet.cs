/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class MemetsRealm
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void DoAll()
    {
        BeachParty();
        FreakiTiki();
        MoonLab();
        Spookeasy();
        DreamMaster();
        NightmareMem();
        ZorbasPalace();
        ByroDax();
    }

    public void BeachParty()
    {
        if (Core.isCompletedBefore(7009))
            return;

        Story.PreLoad(this);

        //7000 | Sweat - tastic
        Story.KillQuest(7000, "beachparty", "Solar Elemental");

        //7001 | Cool 'em Down
        Story.KillQuest(7001, "beachparty", "Boiling Elemental");

        //7002 | Poor Trees.Pour Trees?
        if (!Story.QuestProgression(7002))
        {
            Core.EnsureAccept(7002);
            Core.GetMapItem(6563, 7, "beachparty");
            Core.HuntMonster("beachparty", "Water Elemental", "Refreshing Water", 7);
            Core.EnsureComplete(7002);
        }

        //7003 | Put out the Flames
        if (!Story.QuestProgression(7003))
        {
            Core.EnsureAccept(7003);
            Core.GetMapItem(6564, 8, "beachparty");
            Core.HuntMonster("beachparty", "Sun Flare", "Sun Flare Slain", 8);
            Core.EnsureComplete(7003);
        }

        //7004 | Sunscream
        Story.KillQuest(7004, "beachparty", "Solar Elemental");

        //7005 | Whoops!
        Story.KillQuest(7005, "beachparty", "Frozen Water");

        //7006 | Milk From Nuts?
        Story.KillQuest(7006, "beachparty", "Palm Treeant");

        //7007 | Free Drinks
        Story.MapItemQuest(7007, "beachparty", 6565, 8);

        //7008 | Ice, Ice Maybe?
        if (!Story.QuestProgression(7008))
        {
            Core.EnsureAccept(7008);
            Core.GetMapItem(6566, 8, "beachparty");
            Core.HuntMonster("beachparty", "Frozen Water", "Elemental Ice Chunks", 8);
            Core.EnsureComplete(7008);
        }

        //7009 | Chill Out!
        Story.KillQuest(7009, "beachparty", "Steaming Dragon");

        //Quest not required -- Farming Quest
        // //7010 | Find Tokens! Win A Prize!
        // if (!Story.QuestProgression(7010))
        // {
        //     Core.EnsureAccept(7010);
        //     Core.KillMonster("beachparty", "r3", "Left", "*", "Tiki Tokens", 5, false);
        //     Core.EnsureComplete(7010);
        // }
    }

    public void FreakiTiki()
    {
        if (Core.isCompletedBefore(5571))
            return;

        Story.PreLoad(this);

        //5558 | Drink #1: the Blue Moglin
        Story.MapItemQuest(5558, "yulgar", 5034);

        //5559 | A Little Something Extra
        Story.KillQuest(5559, "thespan", "Minx Fairy");

        //5560 | Mix a Blue Moglin
        Story.MapItemQuest(5560, "freakitiki", 5038);

        //5561 | Drink #2: the El Captain Rhubarb
        if (!Story.QuestProgression(5561))
        {
            Core.EnsureAccept(5561);
            Core.GetMapItem(5035, 5, "freakitiki");
            Core.HuntMonster("freakitiki", "Spineapple", "Crushed Spineapple", 6);
            Core.HuntMonster("freakitiki", "Palm Treeant", "Coconut Milk", 3);
            Core.EnsureComplete(5561);
        }

        //5562 | Needs a Little Kick
        if (!Story.QuestProgression(5562))
        {
            Core.EnsureAccept(5562);
            Core.HuntMonster("pirates", "Undead Pirate", "Bone Powder", 5);
            Core.HuntMonster("pirates", "Undead Pirate", "Pirate Spice", 5);
            Core.EnsureComplete(5562);
        }
        // if (!Story.QuestProgression(5562))
        // {
        //     Core.EnsureAccept(5562);
        //     Core.HuntMonster("pirates", "Undead Pirate", "Pirate Spice", 5);
        //     Core.HuntMonster("pirates", "Undead Pirate", "Bone Powder", 5);
        //     Core.EnsureComplete(5562);
        // }

        //5563 | Mix an El Captain Rhubarb
        Story.MapItemQuest(5563, "freakitiki", 5039);

        //5564 | Drink #3: the Blazing Beard
        if (!Story.QuestProgression(5564))
        {
            Core.EnsureAccept(5564);
            Core.GetMapItem(5036, 5, "freakitiki");
            Core.HuntMonster("freakitiki", "Tiki Sneak", "Tiki Sneak Venom", 3);
            Core.HuntMonster("freakitiki", "Palm Treeant", "Bananas", 6);
            Core.EnsureComplete(5564);
        }

        //5565 | Let’s Give It Some HEAT
        Story.KillQuest(5565, "fotia", "Fotia Spirit");

        //5566 | Mix a Blazing Beard
        Story.MapItemQuest(5566, "freakitiki", 5040);

        //5567 | System Cleanse
        if (!Story.QuestProgression(5567))
        {
            Core.EnsureAccept(5567);
            Core.HuntMonster("freakitiki", "Sneak Venom", "Sneak Venom Gathered", 3);
            Core.HuntMonster("freakitiki", "Sugar Imp", "Sugar Imp Slain", 3);
            Core.HuntMonster("freakitiki", "Spicy Heat", "Spicy Heat Slain", 3);
            Core.EnsureComplete(5567);
        }

        //5568 | Down the, um, Ear Hole
        Story.MapItemQuest(5568, "freakitiki", 5037);

        //5569 | Tire Him Out!
        if (!Story.QuestProgression(5569))
        {
            Core.EnsureAccept(5569);
            Core.HuntMonsterMapID("freakitiki", 22, "Subdue Memehano");
            Core.EnsureComplete(5569);
            Core.Logger("Completed Quest: [5569] - \"Tire Him Out!\"");
        }

        //5570 | Chase Him Down!
        if (!Story.QuestProgression(5570))
        {
            Core.EnsureAccept(5570);
            Core.HuntMonsterMapID("freakitiki", 22, "Subdue Memehano");
            Core.EnsureComplete(5570);
            Core.Logger("Completed Quest: [5570] - \"Chase Him Down!\"");
        }

        //5571 | Get Him CALM!
        if (!Story.QuestProgression(5571))
        {
            Core.EnsureAccept(5571);
            Core.HuntMonsterMapID("freakitiki", 32, "Subdue Memehano");
            Core.EnsureComplete(5571);
            Core.Logger("Completed Quest: [5571] - \"Get Him CALM!\"");
        }
    }

    public void MoonLab()
    {
        if (Core.isCompletedBefore(6103))
            return;

        Story.PreLoad(this);

        //Needle in a Cedar Stack (6091)
        Story.MapItemQuest(6091, "moonlab", 5523);

        //Pick the Lock (6092)
        Story.MapItemQuest(6092, "moonlab", 5524);

        //Contain the Infection (6093)
        Story.KillQuest(6093, "moonlab", "Infected Rodent");

        //Gather the Reagents (6094)
        if (!Story.QuestProgression(6094))
        {
            Core.EnsureAccept(6094);
            Core.GetMapItem(5527, 5, "moonlab");
            Core.HuntMonster("moonlab", "Chlorine Trifluoride", "Chlorine Trifluoride", 1);
            Core.EnsureComplete(6094);
        }

        //Flame On! (6095)
        if (!Story.QuestProgression(6095))
        {
            Core.EnsureAccept(6095);
            Core.GetMapItem(5525, 8, "moonlab");
            Core.GetMapItem(5526, 1, "moonlab");
            Core.HuntMonster("moonlab", "Slime Mold", "Burn the Wandering Slime Molds", 8);
            Core.EnsureComplete(6095);
        }

        //Open the Cages! (6096)
        if (!Story.QuestProgression(6096))
        {
            Core.EnsureAccept(6096);
            Core.GetMapItem(5528, 8, "moonlab");
            Core.HuntMonster("moonlab", "Engorged Slime Mold", "Cage Key", 8);
            Core.EnsureComplete(6096);
        }

        //Get Rid of the Blockage! (6097)
        Story.KillQuest(6097, "moonlab", "Infected Cat");

        //Keep on Going (6098)
        Story.MapItemQuest(6098, "moonlab", new[] { 5529, 5530 });

        //Find the Source (6099)
        if (!Story.QuestProgression(6099))
        {
            Core.EnsureAccept(6099);
            Core.GetMapItem(5531, 3, "moonlab");
            Core.HuntMonster("moonlab", "Mutated Slime Mold", "Mutated Slime Mold Destroyed", 6);
            Core.EnsureComplete(6099);
        }

        //Find the Key (6100)
        if (!Story.QuestProgression(6100))
        {
            Core.EnsureAccept(6100);
            Core.HuntMonster("moonlab", "Infected Scientist", "Infected Scientist Slain", 5);
            Core.HuntMonster("moonlab", "Infected Scientist", "Storage Room Key", 1);
            Core.EnsureComplete(6100);
        }

        //Clear More Mold (6101)
        Story.MapItemQuest(6101, "moonlab", new[] { 5532, 5533 });

        //Defeat the Escapee! (6102)
        Story.KillQuest(6102, "moonlab", "Escaped Experiment");

        //Reveal the Nightmare (6103)
        Story.KillQuest(6103, "moonlab", "Nightmare Zorbak");
    }

    public void Spookeasy()
    {
        if (Core.isCompletedBefore(6661))
            return;

        Story.PreLoad(this);

        //No More Tears (6656)
        Story.KillQuest(6656, "spookeasy", "Happy Cloud");

        //Everything on it is a Gas (6657)
        Story.KillQuest(6657, "spookeasy", "Happy Balloon");

        //Gooooo-d Job (6658)
        Story.KillQuest(6658, "spookeasy", "Nightmare Goo");

        //Hiding in the Shade (6659)
        Story.KillQuest(6659, "spookeasy", "Nightmare Shade");

        //One Dream, One World (6660)
        if (!Story.QuestProgression(6660))
        {
            Core.EnsureAccept(6660);
            Core.KillMonster("spookeasy", "r6", "Left", "*", "Dream Dust", 8);
            Core.EnsureComplete(6660);
        }

        //A Nocturnal Vacation (6661)
        Story.KillQuest(6661, "spookeasy", "Nightmare Goo");
    }

    public void DreamMaster()
    {
        if (Core.isCompletedBefore(6668))
            return;

        Spookeasy();

        Story.PreLoad(this);

        //Jailbreak! (6662)
        Story.KillQuest(6662, "dreammaster", "Prison Wall");

        //Freedom! (6663)
        if (!Story.QuestProgression(6663))
        {
            Core.EnsureAccept(6663);
            Core.GetMapItem(6176, 1, "dreammaster");
            Core.HuntMonster("dreammaster", "Guard Llama", "Guards Defeated", 3);
            Core.EnsureComplete(6663);
        }

        //Make the Glitter Fade (6664)
        Story.KillQuest(6664, "dreammaster", "Sparkle Guard");

        //Rama Llama Ding Dong! (6665)
        Story.KillQuest(6665, "dreammaster", "Guard Llama");

        //Got to get the Key (6666)
        Story.KillQuest(6666, "dreammaster", "Sparkle Guard");

        //Open the Door (6667)
        Story.MapItemQuest(6667, "dreammaster", new[] { 6177, 6178 });

        //Calico Dreams (6668)
        Story.KillQuest(6668, "dreammaster", "Calico Cobby");
    }

    public void NightmareMem()
    {
        if (Core.isCompletedBefore(4157))
            return;

        if (!Core.IsMember)
        {
            Core.Logger("You need to be a member for complete the /nightmare questline.");
            return;
        }
        Story.PreLoad(this);

        //Fear of Clowns? 4143
        Story.KillQuest(4143, "Nightmare", "Bobble Clown");

        //FEAR OF CLOWNS!!! 4144
        Story.KillQuest(4144, "Nightmare", "Crazy Clown");

        //Fear of Spiders 4145
        Story.KillQuest(4145, "Nightmare", "Castle Spider");

        //Fear of Snakes? 4146
        if (!Story.QuestProgression(4146))
        {
            Core.EnsureAccept(4146);
            Core.HuntMonster("nightmare", "Wrasp", "Grappling Hook");
            Core.HuntMonster("nightmare", "Sneak", "Rope");
            Core.EnsureComplete(4146);
        }

        //Fear of Falling? 4147
        Story.MapItemQuest(4147, "Nightmare", 3262);

        //Fear of Germs? 4148
        Story.KillQuest(4148, "Nightmare", "Germs");

        //Fear of Needles? 4149
        Story.KillQuest(4149, "Nightmare", "Needle");

        //Fear of Dolls? 4150
        Story.KillQuest(4150, "Nightmare", "Broken Toy");

        //Fear of Being Buried Alive? 4151
        if (!Story.QuestProgression(4151))
        {
            Core.EnsureAccept(4151);
            Core.HuntMonster("nightmare", "Unearthed Skeleton", "Skeleton Deboned", 8);
            Core.HuntMonster("nightmare", "Rotfeeder Worm", "Rotfeeder squished", 4);
            Core.EnsureComplete(4151);
        }

        //Fear of Burning? 4152
        Story.KillQuest(4152, "Nightmare", "Fire Imp");

        //FEAR OF BURNING!!! 4153
        Story.KillQuest(4153, "Nightmare", "Flame Elemental");

        //Fear of Drowning? 4154
        if (!Story.QuestProgression(4154))
        {
            Core.EnsureAccept(4154);
            Core.HuntMonster("nightmare", "Anglerfish", "Anglerfish Defeated", 6);
            Core.HuntMonster("nightmare", "Deep Dweller", "Deep Dweller defeated");
            Core.HuntMonster("nightmare", "Merdraconian", "Merdraconian Defeated", 6);
            Core.EnsureComplete(4154);
        }

        //Fear of Inadequacy? 4155
        Story.KillQuest(4155, "Nightmare", "Unearthed Skeleton");

        //Fear of Loneliness? 4156
        Story.KillQuest(4156, "Nightmare", "Nothing");

        //Fear of Failure? 4157
        Story.KillQuest(4157, "Nightmare", "Devourax");
    }

    public void ZorbasPalace()
    {
        if (Core.isCompletedBefore(7484))
            return;

        //7474 | Get me a Drink
        Story.KillQuest(7474, "zorbaspalace", "Cactus Creeper");

        //7475 | MonkeyButt Coffee
        Story.KillQuest(7475, "zorbaspalace", "Oasis Monkey");

        //7476 | Flyer Time
        Story.MapItemQuest(7476, "zorbaspalace", 7301, 10);

        //7477 | Stop the Enforcers
        Story.KillQuest(7477, "zorbaspalace", "Palace Enforcer");

        //7478 | New Clothes
        Story.KillQuest(7478, "zorbaspalace", "Cactus Creeper");

        //7479 | Get a Guard
        Story.KillQuest(7479, "zorbaspalace", "Thwompcat");

        //7480 | Pickpocket the Pickpockets
        Story.KillQuest(7480, "zorbaspalace", "Oasis Monkey");

        //7481 | Get more Enforcers
        Story.KillQuest(7481, "zorbaspalace", "Palace Enforcer");

        //7482 | Find Memet
        Story.MapItemQuest(7476, "zorbaspalace", 7304);

        //7483 | Get the Lem-or
        Story.KillQuest(7481, "zorbaspalace", "Lem-or");

        //7484 | Kick Zorba's Butt!
        Story.KillQuest(7481, "zorbaspalace", "Zorba the Bakk");
    }

    public void ByroDax()
    {
        if (Core.isCompletedBefore(7535))
            return;

        Story.PreLoad(this);

        //Hack the System (7523)
        Story.MapItemQuest(7523, "byrodax", 7395);

        //Get the Code (7524)
        Story.KillQuest(7524, "byrodax", "Security Droid");

        //Unlock the Door (7525)
        Story.MapItemQuest(7525, "byrodax", 7396);

        //Flora and Fauna (7526)
        if (!Story.QuestProgression(7526))
        {
            Core.EnsureAccept(7526);
            Core.HuntMonster("byrodax", "Mutated Critter", "Mutated Animal Slain", 5);
            Core.HuntMonster("byrodax", "Mutated Treeant", "Mutated Plant Slain", 5);
            Core.EnsureComplete(7526);
        }

        //Erasertime (7527)
        if (!Story.QuestProgression(7527))
        {
            Core.EnsureAccept(7527);
            Core.HuntMonster("byrodax", "Mutated Treeant", "Rubber", 8);
            Core.HuntMonster("byrodax", "Security Droid", "Oil", 4);
            Core.EnsureComplete(7527);
        }

        //Erase it! (7528)
        Story.MapItemQuest(7528, "byrodax", 7397, 10);

        //Gather Samples (7529)
        Story.KillQuest(7529, "byrodax", "Mutated Treeant");

        //Parts for a Machine (7530)
        Story.KillQuest(7530, "byrodax", "Security Droid");

        //Test the Goop (7531)
        if (!Story.QuestProgression(7531))
        {
            Core.EnsureAccept(7531);
            Core.GetMapItem(7398, 1, "byrodax");
            Core.HuntMonster("byrodax", "Space Goop", "Space Goop", 10);
            Core.EnsureComplete(7531);
        }

        //Goop-Be-Gone (7532)
        Story.MapItemQuest(7532, "byrodax", 7399, 10);

        //Cleaning Time (7533)
        if (!Story.QuestProgression(7533))
        {
            Core.EnsureAccept(7533);
            Core.HuntMonster("byrodax", "Mutated Treeant", "Mutated Beings Cleaned", 5);
            Core.HuntMonster("byrodax", "Space Goop", "Space-Goop Cleaned", 5);
            Core.EnsureComplete(7533);
        }

        //Chop Chop Fizz Fizz (7534)
        Story.MapItemQuest(7534, "byrodax", 7400);

        //It's THEM! (7535)
        Story.KillQuest(7535, "byrodax", "Byro-Dax Monstrosity");
    }
}
