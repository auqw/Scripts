/*
name: MaxGoldenTicket
description: Will max the Golden Ticket from lucky day.
tags: lucky day, shamrock, golden, ticket, golden ticket, goldenticket, max
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

public class GoldenTicket
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
        Core.BankingBlackList.AddRange(new[] { "Golden Ticket" });
        Core.SetOptions(disableClassSwap: true);

        MaxTickets();

        Core.SetOptions(false);
    }

    void MaxTickets()
    {
        if (!Core.isSeasonalMapActive("luck"))
            return;

        Bot.Events.PlayerAFK += Core.PlayerAFK;

        int CurrentQuant = Bot.Inventory.GetQuantity("Golden Ticket");
        int UpdatedQuant;
        bool SendNextPacket = true;

        Core.Join("luck");

        while (!Bot.ShouldExit && !Core.CheckInventory("Golden Ticket", 65_000))
        {
            if (Bot.Map.Name != "luck")
                Core.Join("luck");

            if (SendNextPacket)
            {
                Core.SendPackets("%xt%zm%getMapItem%10173%101%");
                SendNextPacket = false;
            }

            UpdatedQuant = Bot.Inventory.GetQuantity("Golden Ticket");

            if (UpdatedQuant > CurrentQuant)
            {
                SendNextPacket = true;

                // log every 100
                if (UpdatedQuant / 100 > CurrentQuant / 100)
                    Core.FarmingLogger("Golden Ticket", 65_000);

                CurrentQuant = UpdatedQuant;
            }
        }

        Bot.Events.PlayerAFK -= Core.PlayerAFK;
    }


}



