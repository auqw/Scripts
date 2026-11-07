/*
name: Witchs Charms Merge
description: Farms the Witchs Charms Merge [2796] in /walpurgisnacht.
tags: walpurgisnacht, merge, witchs, charms, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class WitchsCharms
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Hollyhock Brew",
            "Luna Dial",
            "Silver Stake",
            "Silverblood Blade",
            "Sventura",
            "Witch Princess' Hat",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
		CoreMogloween.BirgittaMogloween();
		CoreMogloween.ShadowSlayerWar();
		CoreMogloween.Walpurgisnacht();
        Adv.StartBuyAllMerge("walpurgisnacht", 2796, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Hollyhock Brew":
                    Core.FarmingLogger(req.Name, quant);
					Core.EquipClass(ClassType.Solo);
					Monster? ensis;
                    Monster? grigor;
                    Core.RegisterQuests(Core.IsMember ? 10907 : 10906);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
						if (Bot.Map.Name != "birgittamogloween")
                            Core.Join("birgittamogloween");

                        if (Bot.Player.Cell != "r6")
                            Core.Jump("r6", "Left");

                        ensis = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 16);
                        grigor = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 17);
						
						if (Bot.TempInv.Contains("Cor Mortis Cosage") && ensis != null)
                            Bot.Combat.Attack(16);

                        else if (Bot.TempInv.Contains("Grigor's Silver Tooth") && grigor != null)
                            Bot.Combat.Attack(17);

                        // If both drops owned OR priority mob dead → kill the other for respawn sync
                        else if (ensis != null)
                            Bot.Combat.Attack(16);

                        else if (grigor != null)
                            Bot.Combat.Attack(17);
						Bot.Sleep(200);
						Bot.Wait.ForQuestComplete(Core.IsMember ? 10907 : 10906);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
					break;
                case "Luna Dial":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 10932 : 10931);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("walpurgisnacht", "The Gentleman", "Father Forest's Tie", 1);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("walpurgisnacht", "Bloodlust", "Vampire's Bloodthirst", 6);
                        Core.HuntMonster("walpurgisnacht", "Beastly Hunger", "Werewolf's Hunger", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Silver Stake":
                    Core.EquipClass(ClassType.Farm);
					Core.RegisterQuests(10908, 10909);
					while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
						Core.KillMonster("shadowslayerwar", "r11", "Left", "*");
						Bot.Wait.ForPickup(req.Name);
					}
                    Core.CancelRegisteredQuests();
                    break;
                case "Silverblood Blade":
                case "Sventura":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("walpurgisnacht", "The Gentleman", req.Name, quant, req.Temp);
                    break;
                case "Witch Princess' Hat":
                    Core.Logger("Use Butler to kill Witch Queen Talia in birgittaspire", messageBox: true);
                    break;
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}.", messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("104334", "Witch Queen Talia", "Mode: [select] only\nShould the bot buy \"Witch Queen Talia\" ?", false),
        new Option<bool>("104624", "ShadowSlayer Z", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Z\" ?", false),
        new Option<bool>("104629", "Blood Lineage Hunter", "Mode: [select] only\nShould the bot buy \"Blood Lineage Hunter\" ?", false),
        new Option<bool>("104338", "Darkovia's Dusk and Dawn", "Mode: [select] only\nShould the bot buy \"Darkovia's Dusk and Dawn\" ?", false),
        new Option<bool>("104637", "Dual Darkovia Dolore", "Mode: [select] only\nShould the bot buy \"Dual Darkovia Dolore\" ?", false),
        new Option<bool>("104638", "Sventura's Hunting Gear", "Mode: [select] only\nShould the bot buy \"Sventura's Hunting Gear\" ?", false),
        new Option<bool>("104630", "Blood Lineage Hat", "Mode: [select] only\nShould the bot buy \"Blood Lineage Hat\" ?", false),
        new Option<bool>("104631", "Blood Lineage Locks", "Mode: [select] only\nShould the bot buy \"Blood Lineage Locks\" ?", false),
        new Option<bool>("104632", "Blood Lineage Morph", "Mode: [select] only\nShould the bot buy \"Blood Lineage Morph\" ?", false),
        new Option<bool>("104633", "Blood Lineage Visage", "Mode: [select] only\nShould the bot buy \"Blood Lineage Visage\" ?", false),
        new Option<bool>("104336", "Witch Queen Talia Hat", "Mode: [select] only\nShould the bot buy \"Witch Queen Talia Hat\" ?", false),
        new Option<bool>("104337", "Witch Queen Talia Locks", "Mode: [select] only\nShould the bot buy \"Witch Queen Talia Locks\" ?", false),
        new Option<bool>("104625", "ShadowSlayer Z Morph", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Z Morph\" ?", false),
        new Option<bool>("104626", "ShadowSlayer Z Visage", "Mode: [select] only\nShould the bot buy \"ShadowSlayer Z Visage\" ?", false),
        new Option<bool>("104636", "Darkovia Dolore", "Mode: [select] only\nShould the bot buy \"Darkovia Dolore\" ?", false),
        new Option<bool>("104339", "Obscuritas", "Mode: [select] only\nShould the bot buy \"Obscuritas\" ?", false),
        new Option<bool>("104628", "Silversun Blade", "Mode: [select] only\nShould the bot buy \"Silversun Blade\" ?", false),
    ];
}
