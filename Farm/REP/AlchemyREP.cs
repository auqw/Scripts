/*
name: Alchemy REP
description: This script will farm Alchemy reputation to rank 10.
tags: alchemy, rep, reputation, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using System.Collections.Generic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class AlchemyREP
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public string OptionsStorage = "AlchemyREP";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "goldMethod",
            "use Gold?",
            "Using gold to buy material, we'll farm them otherwise. (cost 7.000.000 without boosts rank 1 to 10)",
            false
        ),
        CoreBots.Instance.SkipOptions,
    };

    //Gold Voucher 500k x6 (3.000.000 Gold) => 30x Dragon Runestone => Dragon Scale & Ice Vapor x30
    //Rank 1 to 10 => 6.100.000 Gold w/out boost											 (7KK WITH THE BOT)
    //Rank 1 to 10 => 4.900.000 Gold 25% boost (Cape of Awe)								 (5KK WITH THE BOT)
    //Rank 1 to 10 => 3.100.000 Gold REP Boost											     (4KK WITH THE BOT)
    //Rank 1 to 10 => 2.500.000 Gold REP Boost + 25% boost (Cape of Awe)					 (3KK WITH THE BOT)
    //Rank 1 to 10 => 1.300.000 Gold Server REP Boost + REP Boost + 25% boost (Cape of Awe)  (2KK WITH THE BOT)

    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.Add("Dragon Runestone");
        Core.SetOptions();

        Farm.AlchemyREP(10, Bot.Config!.Get<bool>("goldMethod"));

        Core.SetOptions(false);
    }
}
