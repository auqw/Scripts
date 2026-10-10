/*
name: Evil Birthday Presents Merge
description: Farms the Evil Birthday Presents Merge [2779] in /fortressparty.
tags: fortressparty, merge, evil, birthday, presents, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/AqwBirthday/Fortressparty.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class EvilBirthdayPresents
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static Fortressparty FortressParty { get => _FortressParty ??= new Fortressparty(); set => _FortressParty = value; }
    private static Fortressparty _FortressParty;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Costume Supplies",
            "Doomed Memory",
            "Heraldic Deathknight Helm",
            "Shadow Celebrant Hair",
            "Shadow Celebrant Hair + Mask",
            "Shadow Celebrant Locks",
        ]);
        Core.SetOptions();
		FortressParty.Storyline();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("fortressparty", 2779, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Costume Supplies":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("fortressparty", "Drago Cosplayer", req.Name, quant, req.Temp);
                    break;
                case "Doomed Memory":
                case "Shadow Celebrant Hair":
                case "Shadow Celebrant Hair + Mask":
                case "Shadow Celebrant Locks":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("fortressparty", "Hopeful Shadow", req.Name, quant, req.Temp);
                    break;
                case "Heraldic Deathknight Helm":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("fortressparty", "Photoghoulpher", req.Name, quant, req.Temp);
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
        new Option<bool>("47108", "ShadowFall Celebrant", "Mode: [select] only\nShould the bot buy \"ShadowFall Celebrant\" ?", false),
        new Option<bool>("103789", "Heraldic Deathknight", "Mode: [select] only\nShould the bot buy \"Heraldic Deathknight\" ?", false),
        new Option<bool>("103793", "Deathknight's Tribute Cape", "Mode: [select] only\nShould the bot buy \"Deathknight's Tribute Cape\" ?", false),
        new Option<bool>("103795", "Heraldic Deathknight Blades", "Mode: [select] only\nShould the bot buy \"Heraldic Deathknight Blades\" ?", false),
        new Option<bool>("103790", "Heraldic Deathknight Skull", "Mode: [select] only\nShould the bot buy \"Heraldic Deathknight Skull\" ?", false),
        new Option<bool>("103792", "Heraldic Deathknight Morph", "Mode: [select] only\nShould the bot buy \"Heraldic Deathknight Morph\" ?", false),
        new Option<bool>("47109", "Shadow Celebrant Hair + Skull + Mask", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant Hair + Skull + Mask\" ?", false),
        new Option<bool>("47112", "Shadow Celebrant Hair + Skull", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant Hair + Skull\" ?", false),
        new Option<bool>("47114", "Shadow Celebrant Locks + Skull", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant Locks + Skull\" ?", false),
        new Option<bool>("47116", "Shadow Celebrant Locks + Skull + Mask", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant Locks + Skull + Mask\" ?", false),
        new Option<bool>("47118", "Shadow Celebrant Skull Morph", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant Skull Morph\" ?", false),
        new Option<bool>("47121", "ShadowFall Reaper's Scythe", "Mode: [select] only\nShould the bot buy \"ShadowFall Reaper's Scythe\" ?", false),
        new Option<bool>("47120", "Shadow Celebrant's Staff", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant's Staff\" ?", false),
        new Option<bool>("47119", "Shadow Celebrant's Blade", "Mode: [select] only\nShould the bot buy \"Shadow Celebrant's Blade\" ?", false),
        new Option<bool>("103794", "Heraldic Deathknight Blade", "Mode: [select] only\nShould the bot buy \"Heraldic Deathknight Blade\" ?", false),
    ];
}
