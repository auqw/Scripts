/*
name: Sanctified Light of Destiny
description: This bot will do the entire farm for the Sanctified Light of Destiny *Note*: it uses dailies due to BLOD
tags: BLOD, SLOD, blinding, sanctified, light, destiny, undead, 75, damage, good
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
using Skua.Core.Interfaces;

public class SanctifiedLightofDestiny
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreBLOD BLOD { get => _BLOD ??= new CoreBLOD(); set => _BLOD = value; }
    private static CoreBLOD _BLOD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetSanctifiedLightofDestiny();

        Core.SetOptions(false);
    }

    public void GetSanctifiedLightofDestiny()
    {
        if (Core.CheckInventory("Sanctified Light of Destiny"))
            return;

        BLOD.BlindingLightOfDestiny();

        Core.AddDrop("Sanctified Light of Destiny", "Pious Platinum");
        Core.EnsureAccept(8112);

        if (!Core.CheckInventory("Glorious Gold of Destiny"))
            BLOD.UpgradeMetal(MineCraftingMetalsEnum.Gold);
        if (!Core.CheckInventory("Pious Platinum of Destiny"))
            BLOD.UpgradeMetal(MineCraftingMetalsEnum.Platinum);
        Farm.BattleUnderB("Bone Dust", 300);
        BLOD.LoyalSpiritOrb(25);
        Core.HuntMonster("Extinction", "Cyworg", "Refined Metal", 5);

        Core.EnsureComplete(8112);
    }
}
