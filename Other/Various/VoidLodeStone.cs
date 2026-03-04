/*
name: VoidLodestone
description: pre-forgeenhancements if u don't have darkbox and dark key.
tags: void, lode, stone, void lodestone, Acheron
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class VoidLodestone
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

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[] { "Void Lodestone", "Arcane Lodestone", "Mercury Elixir", "Dark Potion", "Dark Gem" });
        Core.SetOptions();

        GetVoidLodestone();

        Core.SetOptions(false);
    }

    public void GetVoidLodestone()
    {
        if (Core.CheckInventory("Void Lodestone"))
            return;

        // Arcane Lodestone
        if (!Core.CheckInventory("Arcane Lodestone"))
        {
            //(Reward from the 'Open Ebony Chest' quest
            //Requires: ???(38565) to acess quest
            TheDarkBox(38565);

            if (Core.CheckInventory(38565))
            {
                Core.EnsureAccept(5723);
                Core.HuntMonster("dreadfire", "Stray Mana", "Bronze Key", isTemp: false);
                Core.HuntMonster("dreadfire", "Living Brimstone", "Silver Key", isTemp: false);
                Core.Logger("Going to your house to load the shop.\n" + "[there may be a delay]");
                Core.SendPackets($"%xt%zm%house%1%{Bot.Player.Username}%");
                Core.Sleep(5000);
                Core.BuyItem(Bot.Map.Name, 336, "Golden Key");
                Core.EnsureComplete(5723);
            }
            else
            {
                Core.Logger("Cannot Accept Quest Without Item \"???\"");
                return;
            }
        }

        // Mercury Elixir
        if (!Core.CheckInventory("Mercury Elixir"))
        {
            //Reward from the 'Mercury Elixir' quest
            Core.EnsureAccept(5757);
            Core.HuntMonster("Battleunderb", "The Lost", "Mercury Elixir");
            Core.EnsureComplete(5757);
        }

        Core.BuyItem("doomwood", 1381, "Void Lodestone");
    }

    void TheDarkBox(int itemID, int quant = 1)
    {
        ItemBase? Reward = Core.EnsureLoad(5710).Rewards.Find(x => x.ID == itemID);
        if (Reward == null)
        {
            Core.Logger($"ERROR: itemID {itemID} was not found in quest 5710");
            return;
        }

        Core.AddDrop("Dark Potion", Reward.Name);

        Daily.MonthlyTreasureChestKeys();
        if (!Core.CheckInventory(new[] { "Dark Box", "Dark Key" }))
        {
            Core.Logger("Dark Box & Key Not Found, Cannot Continue with Enh");
            return;
        }

        Core.Logger("Pray to RNGsus for your item");
        while (!Bot.ShouldExit && !Core.CheckInventory(Reward.ID, quant))
        {
            Core.EnsureAccept(5710);
            if (Core.IsMember)
                Core.HuntMonster("ruins", "Dark Elemental", "Dark Gem", isTemp: false);
            else
                Core.HuntMonster("darkfortress", "Dark Elemental", "Dark Gem", isTemp: false);
            Core.EnsureComplete(5710);
            Bot.Wait.ForPickup(Reward.ID);
        }
    }

}



