/*
name: Victors Reward Merge
description: This bot will farm the items belonging to the selected mode for the Victors Reward Merge [2594] in /coliseum
tags: victors, reward, merge, coliseum, floating, sapphire, orbs, manifestation, void, phantasm, tail, black, moon, deep, marauder, berserker, morph, fugitive, mutative, orb, pet, rippling, katana, katanas, sheathed, ivory, thorn, crux, nyx, cruxes, violet, sharp, crew, cut, professional, acidic, raylock, abandoned, undead, skull, arena, forked, scorpions, stinger, ominous, bloodstaff, arrogant, angel, spines, gilded, gunslinger, bounty, hunter, ornate, gold, pistol, pistols, shotgun, rush, knife, knives, beard, , backup, keelia, master, aeris, battlespire
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class VictorsRewardMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Silver Victory Laurel", "Gold Victory Laurel", "Platinum Victory Laurel" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("coliseum", 2594, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
                #endregion

                #region Items not setup

                case "Silver Victory Laurel":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> svlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10304))
                        svlQuests.Add(10304);
                    if (Bot.Quests.IsAvailable(10303))
                        svlQuests.Add(10303);
                    if (svlQuests.Count > 0)
                    {
                        Core.RegisterQuests(svlQuests.ToArray());
                        for (int i = 0; i < svlQuests.Count; i++)
                        {
                            Core.HuntMonster("coliseum", "Nethersea Shark", "Level 25 Boss Defeated", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster("coliseum", "Nethersea Shark", req.Name, quant, false, false);
                    break;


                case "Gold Victory Laurel":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> gvlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10306))
                        gvlQuests.Add(10306);
                    if (Bot.Quests.IsAvailable(10305))
                        gvlQuests.Add(10305);
                    if (gvlQuests.Count > 0)
                    {
                        Core.RegisterQuests(gvlQuests.ToArray());
                        for (int i = 0; i < gvlQuests.Count; i++)
                        {
                            Core.HuntMonster("coliseum", "Void Dragon", "Level 50 Boss Defeated", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster("coliseum", "Void Dragon", req.Name, quant, false, false);
                    break;


                case "Platinum Victory Laurel":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> pvlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10308))
                        pvlQuests.Add(10308);
                    if (Bot.Quests.IsAvailable(10307))
                        pvlQuests.Add(10307);
                    if (pvlQuests.Count > 0)
                    {
                        Core.RegisterQuests(pvlQuests.ToArray());
                        for (int i = 0; i < pvlQuests.Count; i++)
                        {
                            Core.HuntMonster("coliseum", "Chimera", "Level 75 Boss Defeated", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster("coliseum", "Chimera", req.Name, quant, false, false);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("30953", "Floating Sapphire Orbs", "Mode: [select] only\nShould the bot buy \"Floating Sapphire Orbs\" ?", false),
        new Option<bool>("75306", "Manifestation of the Void", "Mode: [select] only\nShould the bot buy \"Manifestation of the Void\" ?", false),
        new Option<bool>("82170", "Phantasm Tail", "Mode: [select] only\nShould the bot buy \"Phantasm Tail\" ?", false),
        new Option<bool>("83078", "Black Moon Dagger", "Mode: [select] only\nShould the bot buy \"Black Moon Dagger\" ?", false),
        new Option<bool>("83079", "Black Moon Daggers", "Mode: [select] only\nShould the bot buy \"Black Moon Daggers\" ?", false),
        new Option<bool>("83852", "Deep Void Marauder", "Mode: [select] only\nShould the bot buy \"Deep Void Marauder\" ?", false),
        new Option<bool>("83853", "Deep Void Berserker", "Mode: [select] only\nShould the bot buy \"Deep Void Berserker\" ?", false),
        new Option<bool>("83854", "Deep Void Marauder Morph", "Mode: [select] only\nShould the bot buy \"Deep Void Marauder Morph\" ?", false),
        new Option<bool>("83855", "Deep Void Berserker Morph", "Mode: [select] only\nShould the bot buy \"Deep Void Berserker Morph\" ?", false),
        new Option<bool>("83856", "Deep Void Fugitive Morph", "Mode: [select] only\nShould the bot buy \"Deep Void Fugitive Morph\" ?", false),
        new Option<bool>("83857", "Deep Void Marauder Tail", "Mode: [select] only\nShould the bot buy \"Deep Void Marauder Tail\" ?", false),
        new Option<bool>("83859", "Mutative Void Orb Pet", "Mode: [select] only\nShould the bot buy \"Mutative Void Orb Pet\" ?", false),
        new Option<bool>("83860", "Mutative Deep Void", "Mode: [select] only\nShould the bot buy \"Mutative Deep Void\" ?", false),
        new Option<bool>("83862", "Rippling Void Katana", "Mode: [select] only\nShould the bot buy \"Rippling Void Katana\" ?", false),
        new Option<bool>("83863", "Rippling Void Katanas", "Mode: [select] only\nShould the bot buy \"Rippling Void Katanas\" ?", false),
        new Option<bool>("84699", "Sheathed Ivory Thorn", "Mode: [select] only\nShould the bot buy \"Sheathed Ivory Thorn\" ?", false),
        new Option<bool>("84702", "Sheathed Ivory Blade", "Mode: [select] only\nShould the bot buy \"Sheathed Ivory Blade\" ?", false),
        new Option<bool>("87501", "Crux of Nyx", "Mode: [select] only\nShould the bot buy \"Crux of Nyx\" ?", false),
        new Option<bool>("87502", "Cruxes of Nyx", "Mode: [select] only\nShould the bot buy \"Cruxes of Nyx\" ?", false),
        new Option<bool>("87511", "Crux of Violet", "Mode: [select] only\nShould the bot buy \"Crux of Violet\" ?", false),
        new Option<bool>("87512", "Cruxes of Violet", "Mode: [select] only\nShould the bot buy \"Cruxes of Violet\" ?", false),
        new Option<bool>("87751", "Sharp Crew Cut", "Mode: [select] only\nShould the bot buy \"Sharp Crew Cut\" ?", false),
        new Option<bool>("87753", "Sharp Professional Locks", "Mode: [select] only\nShould the bot buy \"Sharp Professional Locks\" ?", false),
        new Option<bool>("87756", "Sharp Crew Cut Morph", "Mode: [select] only\nShould the bot buy \"Sharp Crew Cut Morph\" ?", false),
        new Option<bool>("87757", "Sharp Professional Visage", "Mode: [select] only\nShould the bot buy \"Sharp Professional Visage\" ?", false),
        new Option<bool>("88451", "Acidic Raylock", "Mode: [select] only\nShould the bot buy \"Acidic Raylock\" ?", false),
        new Option<bool>("89419", "Abandoned Undead", "Mode: [select] only\nShould the bot buy \"Abandoned Undead\" ?", false),
        new Option<bool>("89420", "Abandoned Undead Skull", "Mode: [select] only\nShould the bot buy \"Abandoned Undead Skull\" ?", false),
        new Option<bool>("90230", "Arena Victor's Spikes", "Mode: [select] only\nShould the bot buy \"Arena Victor's Spikes\" ?", false),
        new Option<bool>("90231", "Arena Victor's Locks", "Mode: [select] only\nShould the bot buy \"Arena Victor's Locks\" ?", false),
        new Option<bool>("90242", "Forked Scorpion's Stinger", "Mode: [select] only\nShould the bot buy \"Forked Scorpion's Stinger\" ?", false),
        new Option<bool>("90244", "Ominous BloodStaff", "Mode: [select] only\nShould the bot buy \"Ominous BloodStaff\" ?", false),
        new Option<bool>("90245", "Arrogant BloodStaff", "Mode: [select] only\nShould the bot buy \"Arrogant BloodStaff\" ?", false),
        new Option<bool>("91109", "Angel of the Void Spines", "Mode: [select] only\nShould the bot buy \"Angel of the Void Spines\" ?", false),
        new Option<bool>("93398", "Gilded Gunslinger", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger\" ?", false),
        new Option<bool>("93399", "Gilded Bounty Hunter", "Mode: [select] only\nShould the bot buy \"Gilded Bounty Hunter\" ?", false),
        new Option<bool>("93411", "Ornate Gold Pistol", "Mode: [select] only\nShould the bot buy \"Ornate Gold Pistol\" ?", false),
        new Option<bool>("93412", "Ornate Gold Pistols", "Mode: [select] only\nShould the bot buy \"Ornate Gold Pistols\" ?", false),
        new Option<bool>("93413", "Ornate Gold Shotgun", "Mode: [select] only\nShould the bot buy \"Ornate Gold Shotgun\" ?", false),
        new Option<bool>("93414", "Gold Rush Knife", "Mode: [select] only\nShould the bot buy \"Gold Rush Knife\" ?", false),
        new Option<bool>("93415", "Gold Rush Knives", "Mode: [select] only\nShould the bot buy \"Gold Rush Knives\" ?", false),
        new Option<bool>("93416", "Ornate Gold Guns", "Mode: [select] only\nShould the bot buy \"Ornate Gold Guns\" ?", false),
        new Option<bool>("93401", "Gilded Gunslinger Hair", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger Hair\" ?", false),
        new Option<bool>("93402", "Gilded Gunslinger Locks", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger Locks\" ?", false),
        new Option<bool>("93403", "Gilded Gunslinger Beard", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger Beard\" ?", false),
        new Option<bool>("93405", "Gilded Gunslinger Hat + Locks", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger Hat + Locks\" ?", false),
        new Option<bool>("93406", "Gilded Gunslinger Hat", "Mode: [select] only\nShould the bot buy \"Gilded Gunslinger Hat\" ?", false),
        new Option<bool>("93410", "Back-Up Ornate Gold Shotgun", "Mode: [select] only\nShould the bot buy \"Back-Up Ornate Gold Shotgun\" ?", false),
        new Option<bool>("95658", "Keelia the Arena Master", "Mode: [select] only\nShould the bot buy \"Keelia the Arena Master\" ?", false),
        new Option<bool>("95659", "Aeris BattleSpire Arena Master", "Mode: [select] only\nShould the bot buy \"Aeris BattleSpire Arena Master\" ?", false),
   };
}
