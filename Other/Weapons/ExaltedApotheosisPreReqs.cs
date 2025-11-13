/*
name: ExaltedApotheosisPreReqs
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;

public class ExaltedApotheosisPreReqs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
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

    private string[] Weps =
    {
        "Exalted Apotheosis",
        "Exalted Penultima",
        "Exalted Unity",
        "Apostate Ultima",
        "Thaumaturgus Ultima",
        "Apostate Omega",
        "Thaumaturgus Omega",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Ezrajal Insignia",
                "Warden Insignia",
                "Engineer Insignia",
                "Exalted Relic Piece",
                "Exalted Artillery Shard",
                "Exalted Forgemetal",
            }
        );
        Core.SetOptions();

        PreReqs();

        Core.SetOptions(false);
    }

    public void PreReqs()
    {
        // Ensure shop is loaded:
        Core.Join("timeinn");
        while (!Bot.ShouldExit && (Bot.Shops?.Name != "Exaltia Merge"))
        {
            Bot.Shops?.Load(2010);
            Bot.Wait.ForTrue(() => Bot.Shops?.Name == "Exaltia Merge", 20);
            Core.Sleep();
        }

        Bot.Wait.ForActionCooldown(Skua.Core.Models.GameActions.LoadShop);
        ShopItem? exaltedApo = Bot.Shops?.Items?.Find(x => x.Name == "Exalted Apotheosis");

        Core.EquipClass(ClassType.Farm);
        Dictionary<string, int> missingMaterials = new();

        while (!Bot.ShouldExit && !Core.CheckInventory("Exalted Apotheosis"))
        {
            // Define the weapon pairs in each tier
            string[][] weaponPairs = new[]
            {
                new[] { "Apostate Alpha", "Thaumaturgus Alpha" },
                new[] { "Apostate Omega", "Thaumaturgus Omega" },
                new[] { "Apostate Ultima", "Thaumaturgus Ultima" },
                new[] { "Exalted Penultima", "Exalted Unity" },
            };

            bool obtained = false;

            foreach (string[] pair in weaponPairs)
            {
                bool hasPairInInventory = pair.All(wep => Bot.Inventory?.Contains(wep) == true);

                // Check if the pair is already in the inventory
                if (hasPairInInventory)
                {
                    Core.Logger($"Pair already owned: {string.Join(", ", pair)}");
                    continue;
                }

                foreach (string wep in pair)
                {
                    if (Core.CheckInventory(wep))
                    {
                        Core.Logger($"Already owned: {wep}");
                        continue;
                    }

                    Core.Logger($"Working on: {wep}");
                    ShopItem? wepData = Bot.Shops?.Items?.FirstOrDefault(x => x.Name == wep);

                    // Check if the weapon has any requirements before buying
                    if (wepData != null && wepData.Requirements?.Count > 0)
                    {
                        bool canBuy = true;
                        foreach (ItemBase req in wepData.Requirements)
                        {
                            if (Core.CheckInventory(req.ID, req.Quantity))
                                continue;
                            switch (req.Name)
                            {
                                case "Exalted Node":
                                    Core.KillMonster(
                                        "timeinn",
                                        "r3",
                                        "Bottom",
                                        "*",
                                        "Exalted Node",
                                        req.Quantity,
                                        req.Temp
                                    );
                                    break;
                                case "Exalted Artillery Shard":
                                    Core.HuntMonster(
                                        "timeinn",
                                        "The Engineer",
                                        req.Name,
                                        req.Quantity,
                                        req.Temp
                                    );
                                    break;
                                case "Exalted Relic Piece":
                                    Core.HuntMonster(
                                        "timeinn",
                                        "The Warden",
                                        req.Name,
                                        req.Quantity,
                                        req.Temp
                                    );
                                    break;
                                case "Exalted Forgemetal":
                                    Core.HuntMonster(
                                        "timeinn",
                                        "Ezrajal",
                                        req.Name,
                                        req.Quantity,
                                        req.Temp
                                    );
                                    break;
                                default:
                                    missingMaterials[req.Name] =
                                        req.Quantity - Bot.Inventory?.GetQuantity(req.ID) ?? 0;
                                    canBuy = false;
                                    break;
                            }
                        }

                        string status = canBuy ? "\u2713" : "\u274C"; // Checkmark or red X
                        Core.Logger($"Can complete: {status}");

                        // Buy the weapon after fulfilling requirements
                        if (
                            canBuy
                            && wepData.Requirements.All(req =>
                                Core.CheckInventory(req.ID, req.Quantity)
                            )
                        )
                            Adv.BuyItem("timeinn", 2010, wep);

                        if (Core.CheckInventory("Exalted Apotheosis"))
                        {
                            obtained = true;
                            break;
                        }
                    }
                }

                if (obtained || Core.CheckInventory("Exalted Apotheosis"))
                    break;
            }

            if (Core.CheckInventory("Exalted Apotheosis"))
            {
                Core.Logger("Congratulations on completing the Exalted Apotheosis weapon!");
                break;
            }
            else if (!obtained && exaltedApo?.Requirements != null)
            {
                foreach (ItemBase item in exaltedApo.Requirements)
                {
                    int missingQuantity =
                        item.Quantity - (Bot.Inventory?.GetQuantity(item.ID) ?? 0); // It's not handled automatically if Inventory is null, only if item is not in inventory!
                    if (missingQuantity > 0)
                        missingMaterials[item.Name] = missingQuantity;
                }
                break; // Exit the loop if the item cannot be obtained
            }
            else if (!obtained)
            {
                Core.Logger("Exalted Apotheosis item not found in shop.");
                break;
            }
        }

        if (missingMaterials.Count > 0)
        {
            Bot.Log(
                "Missing materials:\n"
                    + string.Join(
                        "\n",
                        missingMaterials.Select(mat => $"\t{mat.Key}: x {mat.Value}")
                    )
            );
        }

        Bot.Wait.ForPickup("Exalted Apotheosis");
    }
}
