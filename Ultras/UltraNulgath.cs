/*
name: UltraNulgath
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

/* 
# Composition Guide
---

## ⚡ Fast
1. Chrono ShadowSlayer — **Vim | Lucky | Valiance | Vainglory**
2. Verus DoomKnight — **Anima | Lucky | Ravenous | Vainglory**
3. Legion Revenant — **Pneuma | Wizard | Valiance / Ravenous / Arcana | Vainglory**
4. Lord of Order — **Forge | Lucky | Awe Blast / Valiance | Absolution**

---

## ⚡ F2P Fast
1. Dragon of Time — **Pneuma | Wizard | Elysium | Vainglory**
2. Dragon of Time — **Pneuma | Wizard | Elysium | Vainglory**
3. Legion Revenant — **Pneuma | Wizard | Valiance / Ravenous / Arcana | Vainglory**
4. Lord of Order — **Forge | Lucky | Awe Blast / Valiance | Absolution**

---

## ⚖️ Common
1. King’s Echo — **Examen | Lucky | Ravenous | Vainglory**
2. Legion Revenant — **Pneuma | Wizard | Valiance / Ravenous / Arcana | Vainglory**
3. ArchPaladin — **Forge | Lucky | Valiance | Lament**
4. Lord of Order — **Forge | Lucky | Awe Blast / Valiance | Absolution**

---

## 🧪 Other DPS (Slot-ins)
- Arcana Invoker — **Examen | Lucky | Ravenous | Vainglory**
- Archfiend — **Forge | Lucky | Ravenous | Vainglory**
- Lich — **Examen | Lucky | Ravenous | Vainglory**
- Verus DoomKnight — **Anima | Lucky | Ravenous | Vainglory**

*/



public class UltraNulgath
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraNulgath";
    public List<IOption> Options = new()
    {
        new Option<string>( "a", "Taunter 1 ClassName", "Names must be exact including punctuation, spelling, and captitalization", "ArchPaladin"),
        new Option<string>( "b", "Taunter 2 ClassName", "Names must be exact including punctuation, spelling, and captitalization", "Lord Of Order"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "Ultra Nulgath",
            "Deaths more then likely will happen, Suggested class and thier enhs are in the script at the top"
        );

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config!.Get<string>("b") ?? "").Trim();
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            C.Logger("Setup", "Fill both taunter classes in Script Options.");
            C.SetOptions(false);
        }

        Core.Boot();
        Prep();
        Fight();
        C.SetOptions(false);
    }

    string a, b;
    // Overfiend Blade = 1
    // Nulgath = 2
    void Fight()
    {
        #region ignore this
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_nulgath.sync");

        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        #endregion
        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Nulgath the Archfiend Defeated?", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                Core.DisableSkills();
                Bot.Sleep(200);
                C.Jump("Enter", "Spawn");
                C.Join("whitemap");
                if (!Bot.Quests.IsDailyComplete(8692))
                    C.EnsureComplete(8692);
                Adv.GearStore(true, true);
                break;
            }

            // Taunters focus nulgath
            if (Bot.Player.CurrentClass?.Name == a || Bot.Player.CurrentClass?.Name == b)
            {
                //taunters focus Nulgath (MID 2)
                Bot.Combat.Attack(2);
                Bot.Sleep(200);
            }
            else
            {
                //DPSers attack the Overfiend Blade(1) and when it dies, swap to nulgath(2), (refocusing "Overfiend Blade" when it respawns)
                if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
                    Bot.Combat.Attack(1); // Overfiend Blade
                else
                    Bot.Combat.Attack(2); // Nulgath
                Bot.Sleep(200);
            }

            // Taunter logic
            if (Bot.Player.Alive && (Bot.Player.CurrentClass?.Name == a || Bot.Player.CurrentClass?.Name == b) && !Bot.Target.Auras.Any(x => x?.Name == "Focus")
            && Bot.Monsters.MapMonsters.Any(x => (x?.MapID == 2 || x?.MapID == 1) && x.HP > 0))
            {
                Core.DisableSkills();
                while (!Bot.ShouldExit && !Bot.Target.Auras.Any(x => x?.Name == "Focus"))
                {
                    if (!Bot.Player.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        continue;
                    }

                    if (!Bot.Target.Auras.Any(x => x != null && x?.Name == "Focus"))
                        Bot.Skills.UseSkill(5);
                    else
                        break;

                    Bot.Sleep(500);
                }
                Core.EnableSkills();
            }

        }
    }


    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (Bot.Inventory.Items.Any(x => x != null && x.Equipped && (x.Name == a || x.Name == b)))
        {
            Ultra.GetScrollOfEnrage();
            Core.EquipEnrage();
        }
    }
    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Verus DoomKnight
            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lord Of Order
            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Dragon of Time
            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Archfiend
            case "Archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }
}
