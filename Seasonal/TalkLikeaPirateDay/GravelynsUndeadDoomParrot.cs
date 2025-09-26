/*
name: Gravelyn's Undead Doom Parrot
description: This script will complete the quest to obtain Gravelyn's Undead Doom Parrot pet.
tags: undead doom parrot, gravelyn, seasonal,tlapd, talk like a pirate day, pet, celebrate the successful siege,doom pirate, dusk
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class UndeadDoomParrot
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetItem();

        Core.SetOptions(false);
    }

    public void GetItem()
    {
        if (Core.CheckInventory(79501)) // Gravelyn's Undead Doom Parrot
            return;

        // Celebrate the Successful Siege (10425)
        Core.EnsureAccept(10425);
        Core.AddDrop(Core.QuestRewards(10425));

        // Drink Shipment Found!
        Core.HuntMonster("piratewar", "Blazing Commander", Core.QuestRequirements<string>(10425)[2]);

        // Doomknight Commander Defeated
        Core.HuntMonster("piratewar", "Doomknight Commander", Core.QuestRequirements<string>(10425)[0], 10);

        // Shard of Ice
        Adv.BuyItem("pirates", 724, Core.QuestRequirements<string>(10425)[1]);

        // Gallaeon Defeated for Fun
        Core.HuntMonster("doompirate", "Gallaeon", Core.QuestRequirements<string>(10425)[3]);

        Core.EnsureComplete(10425);
        Bot.Wait.ForPickup(Core.QuestRewards(10425)[0]);
        Core.ToBank(Core.QuestRewards(10425));

    }
}