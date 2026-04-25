/*
name: Magnum Opus Merge
description: This bot will farm the items belonging to the selected mode for the Magnum Opus Merge [2709] in /magnumopus
tags: magnum, opus, merge, magnumopus, aleister, dawnseeker, aleisters, champion, plate, magna, visio, emberkissed, son, destructions, awakaned, ars, magnus, imperio, ira, ember, crown, mantle, gloriosa, infamis
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class MagnumOpusMerge
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
        Core.BankingBlackList.AddRange(new[] { "Flame of the Magnum Opus" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("magnumopus", 2709, findIngredients, buyOnlyThis, buyMode: buyMode);

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


                case "Flame of the Magnum Opus":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    if (!string.IsNullOrEmpty(Core.BossClass))
                        Core.EquipClass(ClassType.Boss);
                    else
                    {
                        Bot.Log("BossClass is empty/unselected in CBO, we'll use the Solo class or if thats empty, the currently equipped class.");
                        Core.EquipClass(ClassType.Solo);
                    }
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10702); // TODO: Replace with actual quest ID

                    Bot.Events.ExtensionPacketReceived += FlameoftheBeyond;
                    Bot.Options.AttackWithoutTarget = true;
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        if (Bot.Map.Name != "magnumopus")
                            Core.Join("magnumopus");

                        if (Bot.Player.Cell != "r2")
                            Core.Jump("r2", "bottom");


                        Bot.Combat.Attack("*");

                        Bot.Sleep(200);
                        if (req.Temp ? Bot.TempInv.Contains(req.ID, req.Quantity) : Bot.Inventory.Contains(req.ID, req.Quantity))
                            break;
                    }
                    Bot.Options.AttackWithoutTarget = false;
                    Bot.Events.ExtensionPacketReceived -= FlameoftheBeyond;
                    Core.Jump("Enter", "Spawn");
                    Bot.Wait.ForPickup(req.ID);
                    Core.CancelRegisteredQuests();
                    break;

            }
        }
    }

    public async void FlameoftheBeyond(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;
        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return;

        float px = Bot.Player.X;
        float py = Bot.Player.Y;

        // Zone A active, walk right
        if (string.Equals(zoneSet, "A", StringComparison.OrdinalIgnoreCase))
        {
            /* box of : 
            x: 550, y: 339
            x: 678, y: 465
            */
            if (px >= 550 && px <= 678 && py >= 339 && py <= 465)
                return;

            int randX = Random.Shared.Next(550, 678);
            int randY = Random.Shared.Next(339, 465);
            _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
            return;
        }

        // Zone B active, walk left
        if (string.Equals(zoneSet, "B", StringComparison.OrdinalIgnoreCase))
        {
            /* box of : 
            x: 426, y: 359
            x: 211, y: 445
            */
            if (px >= 211 && px <= 426 && py >= 359 && py <= 445)
                return;

            int randX = Random.Shared.Next(211, 426);
            int randY = Random.Shared.Next(359, 445);
            _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
            return;
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("100488", "Aleister the Dawnseeker", "Mode: [select] only\nShould the bot buy \"Aleister the Dawnseeker\" ?", false),
        new Option<bool>("100489", "Aleister's Champion Plate", "Mode: [select] only\nShould the bot buy \"Aleister's Champion Plate\" ?", false),
        new Option<bool>("100605", "Ranger of Mana", "Mode: [select] only\nShould the bot buy \"Ranger of Mana\" ?", false),
        new Option<bool>("100420", "Magna Visio", "Mode: [select] only\nShould the bot buy \"Magna Visio\" ?", false),
        new Option<bool>("100494", "Ember-Kissed Cape", "Mode: [select] only\nShould the bot buy \"Ember-Kissed Cape\" ?", false),
        new Option<bool>("100495", "Son of Destruction's Cape", "Mode: [select] only\nShould the bot buy \"Son of Destruction's Cape\" ?", false),
        new Option<bool>("100421", "Awakaned Magna Visio", "Mode: [select] only\nShould the bot buy \"Awakaned Magna Visio\" ?", false),
        new Option<bool>("100413", "Dual Ars Magna", "Mode: [select] only\nShould the bot buy \"Dual Ars Magna\" ?", false),
        new Option<bool>("100415", "Dual Magnus Imperio", "Mode: [select] only\nShould the bot buy \"Dual Magnus Imperio\" ?", false),
        new Option<bool>("100417", "Dual Magna Ira", "Mode: [select] only\nShould the bot buy \"Dual Magna Ira\" ?", false),
        new Option<bool>("100490", "Aleister's Hair", "Mode: [select] only\nShould the bot buy \"Aleister's Hair\" ?", false),
        new Option<bool>("100491", "Aleister's Ember Crown", "Mode: [select] only\nShould the bot buy \"Aleister's Ember Crown\" ?", false),
        new Option<bool>("100492", "Aleister's Hood", "Mode: [select] only\nShould the bot buy \"Aleister's Hood\" ?", false),
        new Option<bool>("100493", "Aleister's Mantle", "Mode: [select] only\nShould the bot buy \"Aleister's Mantle\" ?", false),
        new Option<bool>("100606", "Champion Tara Morph", "Mode: [select] only\nShould the bot buy \"Champion Tara Morph\" ?", false),
        new Option<bool>("100607", "Champion Tara's Locks", "Mode: [select] only\nShould the bot buy \"Champion Tara's Locks\" ?", false),
        new Option<bool>("100419", "Ars Gloriosa", "Mode: [select] only\nShould the bot buy \"Ars Gloriosa\" ?", false),
        new Option<bool>("100418", "Ars Infamis", "Mode: [select] only\nShould the bot buy \"Ars Infamis\" ?", false),
        new Option<bool>("100416", "Magna Ira", "Mode: [select] only\nShould the bot buy \"Magna Ira\" ?", false),
        new Option<bool>("100412", "Ars Magna", "Mode: [select] only\nShould the bot buy \"Ars Magna\" ?", false),
        new Option<bool>("100414", "Magnus Imperio", "Mode: [select] only\nShould the bot buy \"Magnus Imperio\" ?", false),
   };
}
