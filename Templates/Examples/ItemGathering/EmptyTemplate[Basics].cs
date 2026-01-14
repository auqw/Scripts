/*
name: null
description: null
tags: null
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

public class DefaultTemplate
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
        // Core.BankingBlackList.AddRange(new[] { "item1", "Item2", "Etc" });
        Core.SetOptions(disableClassSwap: true);

        Example();

        Core.SetOptions(false);
    }

    void Example()
    {
        HuntMonsterBatch(
                 500,
                 false,
                 false,
                 true,
                 ("necrocavern", 5, "Chaos Vordred Essence"),
                 ("citadel", 21, "Belrot the Fiend Essence"),
                 ("greenguardwest", 22, "Black Knight Essence"),
                 ("mudluk", 18, "Tiger Leech Essence"),
                 ("aqlesson", 17, "Carnax Essence"),
                 ("hachiko", 10, "Dai Tengu Essence"),
                 ("timevoid", 12, "Unending Avatar Essence"),
                 ("dragonchallenge", 4, "Void Dragon Essence"),
                 ("maul", 17, "Creature Creation Essence")
             );
    }


    private void HuntMonsterBatch(
           int quant,
           bool isTemp,
           bool publicRoom,
           bool log,
           params (string map, int monster, string essence)[] monsters
       )
    {
        Core.AddDrop(monsters.Select(x => x.essence).ToArray());
        Core.EquipClass(ClassType.Solo);
        foreach (
            var monster in monsters.Where(x =>
                x.essence != null && x.monster > 0 && !Core.CheckInventory(x.essence, quant)
            )
        )
            Core.HuntMonsterMapID(
                monster.map,
                monster.monster,
                monster.essence,
                quant,
                isTemp,
                log,
                publicRoom
            );
    }


}



