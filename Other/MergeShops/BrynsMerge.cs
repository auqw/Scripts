/*
name: Bryns Merge
description: This bot will farm the items belonging to the selected mode for the Bryns Merge [2748] in /templeofdoom
tags: bryns, merge, templeofdoom, doomed, shadowscythe, knight, ornate, shadow, greataxe, shadows, silk, cloak, wrap, greataxes, soulshredders, carcossan, orbs, morph, determination, trihorned, blazing, darkness, resolution, orb, halberd, dulcinea, soulshredder
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BrynsMerge
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
        Core.BankingBlackList.AddRange(new[] { "Empty Vessel", "Leech's Sucker", "Tainted Armor Scrap", "Doom Lily" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("templeofdoom", 2748, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Empty Vessel":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10811); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("templeofdoom", "Emptiness", "Blank Space", 3, isTemp: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Leech's Sucker":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10812); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("templeofdoom", "Doom Leech", "Leech Innards", 50, isTemp: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Tainted Armor Scrap":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10810); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("templeofdoom", "Tainted Paladin", "Withered Light", 9, isTemp: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Doom Lily":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10813); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("templeofdoom", "Downfall of Empires", "Dahlia's Hairpin", 1, isTemp: true); //unsoloable
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("102209", "Doomed ShadowScythe Knight", "Mode: [select] only\nShould the bot buy \"Doomed ShadowScythe Knight\" ?", false),
        new Option<bool>("102220", "Ornate ShadowScythe Knight", "Mode: [select] only\nShould the bot buy \"Ornate ShadowScythe Knight\" ?", false),
        new Option<bool>("102226", "Ornate Shadow GreatAxe", "Mode: [select] only\nShould the bot buy \"Ornate Shadow GreatAxe\" ?", false),
        new Option<bool>("102218", "Doomed GreatAxe of Shadows", "Mode: [select] only\nShould the bot buy \"Doomed GreatAxe of Shadows\" ?", false),
        new Option<bool>("102216", "Silk ShadowScythe Cloak", "Mode: [select] only\nShould the bot buy \"Silk ShadowScythe Cloak\" ?", false),
        new Option<bool>("102224", "Ornate Silk Shadow Wrap", "Mode: [select] only\nShould the bot buy \"Ornate Silk Shadow Wrap\" ?", false),
        new Option<bool>("102227", "Ornate Shadow GreatAxes", "Mode: [select] only\nShould the bot buy \"Ornate Shadow GreatAxes\" ?", false),
        new Option<bool>("102219", "Doomed GreatAxes of Shadows", "Mode: [select] only\nShould the bot buy \"Doomed GreatAxes of Shadows\" ?", false),
        new Option<bool>("96188", "Doomed Soulshredders", "Mode: [select] only\nShould the bot buy \"Doomed Soulshredders\" ?", false),
        new Option<bool>("96194", "Doomed Carcossan Orbs", "Mode: [select] only\nShould the bot buy \"Doomed Carcossan Orbs\" ?", false),
        new Option<bool>("102210", "Morph of Determination", "Mode: [select] only\nShould the bot buy \"Morph of Determination\" ?", false),
        new Option<bool>("102211", "Visage of Determination", "Mode: [select] only\nShould the bot buy \"Visage of Determination\" ?", false),
        new Option<bool>("102212", "Doomed Hair", "Mode: [select] only\nShould the bot buy \"Doomed Hair\" ?", false),
        new Option<bool>("102213", "Doomed Locks", "Mode: [select] only\nShould the bot buy \"Doomed Locks\" ?", false),
        new Option<bool>("102214", "Tri-Horned Shadow Helm", "Mode: [select] only\nShould the bot buy \"Tri-Horned Shadow Helm\" ?", false),
        new Option<bool>("102215", "Blazing Darkness Helmet", "Mode: [select] only\nShould the bot buy \"Blazing Darkness Helmet\" ?", false),
        new Option<bool>("102221", "Morph of Resolution", "Mode: [select] only\nShould the bot buy \"Morph of Resolution\" ?", false),
        new Option<bool>("102222", "Ornate Tri-Horned Helm", "Mode: [select] only\nShould the bot buy \"Ornate Tri-Horned Helm\" ?", false),
        new Option<bool>("102223", "Ornate Shadow Helmet", "Mode: [select] only\nShould the bot buy \"Ornate Shadow Helmet\" ?", false),
        new Option<bool>("96193", "Doomed Carcossan Orb", "Mode: [select] only\nShould the bot buy \"Doomed Carcossan Orb\" ?", false),
        new Option<bool>("102225", "Ornate Halberd of Shadows", "Mode: [select] only\nShould the bot buy \"Ornate Halberd of Shadows\" ?", false),
        new Option<bool>("102217", "Doomed Halberd of Shadows", "Mode: [select] only\nShould the bot buy \"Doomed Halberd of Shadows\" ?", false),
        new Option<bool>("96189", "Doomed Staff of Dulcinea", "Mode: [select] only\nShould the bot buy \"Doomed Staff of Dulcinea\" ?", false),
        new Option<bool>("96187", "Doomed Soulshredder", "Mode: [select] only\nShould the bot buy \"Doomed Soulshredder\" ?", false),
   };
}
