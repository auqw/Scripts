/*
name: Non SpellCrafta Scroll Buyer
description: This script will buy the selected scrolls from the Terminus Temple.
tags: scroll, nsc, life steal, dark flare, nightmares, terminus temple
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class BuyNSCScrolls
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

    public bool DontPreconfigure = true;
    public string OptionsStorage = "BuyNSCScrolls";
    public List<IOption> Options = new()
    {
        new Option<Scrolls>("scrollSelect", "Scroll of", "Select the scroll of your choise"),
        new Option<int>("scrollAmount", "How many", "Write -1 to buy up to max. stack", -1),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        BuyScroll(Bot.Config!.Get<Scrolls>("scrollSelect"), Bot.Config!.Get<int>("scrollAmount"));

        Core.SetOptions(false);
    }

    public void BuyScroll(Scrolls scroll, int quant = -1)
    {
        string scrollName = GetScrollName(scroll);
        int currentQuantity = Bot.Inventory.GetQuantity(scrollName);
        int maxStack = 99;
        int buyAmount = quant == -1 ? maxStack - currentQuantity : quant;
        Bot.Log(
            $"Buying {scrollName} - Current: {currentQuantity}, Buy Amount: {buyAmount}, Max Stack: {maxStack}"
        );
        if (buyAmount < 0)
        {
            Core.Logger("You cannot buy a negative amount of scrolls.");
            return;
        }
        else if (buyAmount > 0)
        {
            if (Core.CheckInventory(scrollName, buyAmount))
            {
                Core.Logger($"You already have {currentQuantity}x {scrollName} in your inventory.");
                return;
            }
            Core.Logger($"Buying {buyAmount}x {scrollName}.");
            Adv.BuyItem("terminatemple", 2328, scrollName, buyAmount);
        }
    }

    public enum Scrolls
    {
        LifeSteal,
        DarkFlare,
        Nightmares,
    }

    private string GetScrollName(Scrolls scroll)
    {
        return scroll switch
        {
            Scrolls.LifeSteal => "Scroll of Life Steal",
            Scrolls.DarkFlare => "Scroll of Dark Flare",
            Scrolls.Nightmares => "Scroll of Nightmares",
            _ => "",
        };
    }
}
