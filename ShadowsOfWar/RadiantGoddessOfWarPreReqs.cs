/*
name: Radiant Goddess of War PreReqs
description: Farms the six item requirements needed to accept quest 9184, The Goddess of War. This script does not complete the quest or obtain Radiant Goddess of War.
tags: radiant goddess of war, rgow, goddess of war, prerequisites, prereqs
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Hollowborn/HollowbornPaladin/CoreHollowbornPaladin.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/BeetleQuests.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class RadiantGoddessOfWarPreReqs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private readonly CoreFarms Farm = new();
    private readonly CoreSoW SoW = new();
    private readonly DrakathArmorBot DAB = new();
    private readonly FireChampionsArmor FCA = new();
    private readonly BeetleQuests BeetleQuests = new();
    private readonly Awescended Awescended = new();
    private readonly CoreHollowbornPaladin HBP = new();
    private readonly MalgorsArmorSet MalgorsArmorSet = new();

    public string OptionsStorage = "RGoWPreReqs";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "UseInsigsonEmpDrkArm",
            "Use Insignias for Empowered Drakath Armor?",
            "Spend 5 Champion Drakath Insignias on Empowered Drakath Armor, which is required to accept quest 9184, \"The Goddess of War.\"",
            false
        ),
        CoreBots.Instance.SkipOptions,
    };

    private readonly string[] RequiredItems =
    {
        "Empowered Drakath Armor",
        "Fire Champion's Armor",
        "Void Beetle Warlord",
        "Malgor the ShadowLord",
        "Classic Hollowborn Paladin Armor",
        "Awescended"
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(RequiredItems);
        Core.BankingBlackList.AddRange(new[] { "Drakath Armor", "Champion Drakath Insignia" });
        Core.SetOptions();

        try
        {
            GetPrereqs();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    public void GetPrereqs()
    {
        if (Core.CheckInventory("Radiant Goddess of War"))
        {
            Core.Logger("Radiant Goddess of War is already owned");
            return;
        }

        if (!Core.CheckInventory(RequiredItems))
        {
            SoW.CompleteCoreSoW();
            Farm.Experience();
            GetEmpoweredDrakathArmor();
            FCA.GetFireChampsArmor();
            BeetleQuests.WarlordRewards("Void Beetle Warlord");
            Awescended.GetAwe();
            HBP.GetSpecific("Classic Hollowborn Paladin Armor");
            MalgorsArmorSet.GetSet(false, new[] { "Malgor the ShadowLord" });
        }

        if (Core.CheckInventory(RequiredItems))
            Core.Logger("All item requirements needed to accept the quest \"The Goddess of War\" are ready.");
        else
        {
            foreach (string item in RequiredItems)
            {
                if (!Core.CheckInventory(item))
                    Core.Logger($"Missing requirement for quest 9184, The Goddess of War: {item}");
            }
        }

        Core.Logger("Next: Run UltraSpeakerMergePreReqs.cs to obtain Goddess Of War armor and complete the quest");
    }

    private void GetEmpoweredDrakathArmor()
    {
        if (Core.CheckInventory("Empowered Drakath Armor"))
            return;

        if (!Bot.Config!.Get<bool>("UseInsigsonEmpDrkArm"))
        {
            Core.Logger("Empowered Drakath Armor was skipped because insignia use is disabled");
            return;
        }

        if (!Core.CheckInventory("Champion Drakath Insignia", 5))
        {
            Core.Logger("Empowered Drakath Armor requires 5 Champion Drakath Insignias");
            return;
        }

        DAB.DrakathArmor();

        if (!Core.CheckInventory("Drakath Armor"))
        {
            Core.Logger("Empowered Drakath Armor could not be bought because Drakath Armor is missing");
            return;
        }

        Core.BuyItem("championdrakath", 2055, "Empowered Drakath Armor");
    }
}
