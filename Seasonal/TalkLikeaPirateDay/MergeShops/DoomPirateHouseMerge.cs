/*
name: DoomPirate House Merge
description: This bot will farm the items belonging to the selected mode for the DoomPirate House Merge [2331] in /doompirate
tags: doompirate, house, merge, doompirate, shadowscythe, pirate, warship, commander, gallaeon, guest, statue
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DoomPirateStory.cs
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class DoomPirateHouseMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
private static CoreAdvanced _sAdv;

    private static DoomPirate DP { get => _DP ??= new DoomPirate(); set => _DP = value; }
    private static DoomPirate _DP;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Gallaeon's Piece of Eight", "Doom Doubloon" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        DP.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("doompirate", 2331, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Gallaeon's Piece of Eight":
                    Core.FarmingLogger(req.Name, req.Quantity);
                    Core.RegisterQuests(9355);
                    Core.EquipClass(ClassType.Solo);
                    Core.Join("doompirate", "r5", "Left");

                    bool restartKills = false;

                RestartKills:
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, req.Quantity))
                    {
                        if (restartKills)
                        {
                            restartKills = false;

                            Core.Logger("Send player to house to reset map");
                            Bot.Send.Packet($"%xt%zm%house%1%{Core.Username()}%");
                            Bot.Wait.ForMapLoad("house");

                            Core.Logger("Rejoin map to reset mobs");
                            Core.Join("doompirate", "r5", "Left");
                            Bot.Wait.ForMapLoad("doompirate");
                        }

                        foreach (int mobId in new[] { 5, 4, 7, 6, 9, 8, 11, 10 })
                        {
                            while (!Bot.ShouldExit)
                            {
                                if (!Bot.Player.Alive)
                                {
                                    Core.Logger("Death - Resetting");
                                    while (!Bot.ShouldExit && !Bot.Player.Alive) { Bot.Sleep(1000); }
                                    restartKills = true;
                                    goto RestartKills;
                                }

                                if (Bot.Player.Cell != "r5")
                                {
                                    Core.Jump("r5", "Left");
                                    Core.Sleep();
                                }

                                Monster? mon = Bot.Monsters.CurrentAvailableMonsters.FirstOrDefault(x => x != null && x.MapID == mobId);
                                if (mon == null)
                                {
                                    Core.Logger($"Skipping mob {mobId}, not found.");
                                    continue;
                                }

                                if (!Bot.Player.HasTarget)
                                    Bot.Combat.Attack(mobId);

                                Bot.Sleep(1500);

                                if (Core.GetMonsterHP(mobId.ToString()) <= 0)
                                {
                                    Bot.Combat.CancelTarget();
                                    break;
                                }
                            }
                        }

                        Bot.Kill.Monster(12);
                    }
                    break;

                case "Doom Doubloon":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9354);
                    Core.HuntMonsterMapID("doompirate", 3, req.Name, quant, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public int GetMonsterHP(string monMapID)
    {
        try
        {
            string? jsonData = Bot.Flash.Call("availableMonsters");
            if (string.IsNullOrWhiteSpace(jsonData)) return 0;

            foreach (var mon in JArray.Parse(jsonData))
                if (mon?["MonMapID"]?.ToString() == monMapID)
                    return mon["intHP"]?.ToObject<int>() ?? 0;
        }
        catch { }

        return 0;
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("79508", "Shadowscythe Pirate Warship", "Mode: [select] only\nShould the bot buy \"Shadowscythe Pirate Warship\" ?", false),
        new Option<bool>("79414", "Commander Gallaeon House Guest", "Mode: [select] only\nShould the bot buy \"Commander Gallaeon House Guest\" ?", false),
        new Option<bool>("79623", "Commander Gallaeon Statue", "Mode: [select] only\nShould the bot buy \"Commander Gallaeon Statue\" ?", false),
    };
}
