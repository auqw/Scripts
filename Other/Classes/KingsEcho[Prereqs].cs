/*
name: KingsEchoClassPrerequisites
description: King's Echo Class Prerequisites
tags: Prerequisites, King's, King, Echo, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
using Skua.Core.Interfaces;

public class KingsEchoClassPrerequisites
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }
    private static CoreAOR _AOR;
    private static BocklinTreasuryMerge BTM { get => _BTM ??= new BocklinTreasuryMerge(); set => _BTM = value; }
    private static BocklinTreasuryMerge _BTM;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        Prerequisites();

        Core.SetOptions(false);
    }

    public void Prerequisites(bool rankup = true)
    {
        // if (Core.CheckInventory("King's Echo"))
        // {
        //     if (rankup)
        //         Adv.RankUpClass("King's Echo");
        //     return;
        // }

        // Requirements (That we know):        

        // Level 80
        Farm.Experience(80);

        // Completion of the Rumbling of Cold Thunder saga
        AOR.ColdThunder();

        // Rank 10 in the Good and Swordhaven factions
        Farm.GoodREP();
        Farm.SwordhavenREP();

        // Alden's Liberation Armor
        BTM.BuyAllMerge("Alden's Liberation Armor");

        // if (rankup)
        //     Adv.RankUpClass("King's Echo");
    }


}



