/*
name: Dark Carnax Story
description: This will finish the Dark Carnax story.
tags: story, quest, dark-carnax
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;

public class DarkCarnaxStory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline(bool forUltra = false)
    {
        if (Core.isCompletedBefore(forUltra ? 8871 : 8872))
            return;

        Story.PreLoad(this);

        //8870 The Beast Awakens
        Story.KillQuest(8870, "aqlesson", "Carnax");

        //8871 Nightmare Containment Field
        if (!Story.QuestProgression(8871))
        {
            Core.EnsureAccept(8871);
            Core.HuntMonster("doomvault", "Binky", "Binky's Uni-horn", isTemp: false);
            Core.HuntMonster("deadmoor", "Nightmare", "Nightmare Mane", isTemp: false);
            Core.HuntMonster("somnia", "NightWyrm", "NightWyrm Chitin", isTemp: false);
            Core.HuntMonster("fearhouse", "All Fears", "Sheer Horror", isTemp: false);
            Core.EnsureComplete(8871);
        }
        if (forUltra)
            return;

        //8872 The Last Stand
        if (!Story.QuestProgression(8872))
        {
            Core.Logger($"Doing Quest: [8872] - \"The Last Stand\"");
            SyntheticViscera(1);
            Core.Logger($"Completed Quest: [8872] - \"The Last Stand\"");
        }
        else Core.Logger($"Already Completed: [8872] - \"The Last Stand\"");
    }

    private Task? carnaxMovementTask;
    private CancellationTokenSource? carnaxMovementCts;
    private string currentCarnaxZone = "";
    private DateTime lastCarnaxMove = DateTime.MinValue;
    private readonly TimeSpan CarnaxMoveCooldown = TimeSpan.FromSeconds(2);

    public void SyntheticViscera(int quant = 1000)
    {
        if (Core.CheckInventory("Synthetic Viscera", quant))
            return;

        Core.AddDrop("Synthetic Viscera");
        Core.FarmingLogger("Synthetic Viscera", quant);

        Bot.Options.AttackWithoutTarget = true;

        Bot.Events.RunToArea += DarkCarnaxMove;
        
        // Initialize cancellation token for movement tasks
        carnaxMovementCts = new CancellationTokenSource();

        if (Core.CheckInventory("Dragon of Time"))
        {
            Core.Equip("Dragon of Time");
            Bot.Skills.StartAdvanced("3|2|4|2|1|2", 250, SkillUseMode.WaitForCooldown);
        }
        else if (Core.CheckInventory("Healer (Rare)"))
            Bot.Skills.StartAdvanced("Healer (Rare)", true, ClassUseMode.Base);
        else if (Core.CheckInventory("Healer"))
            Bot.Skills.StartAdvanced("Healer", true, ClassUseMode.Base);
        else
            Core.EquipClass(ClassType.Solo);

        Adv.GearStore();
        Adv.EnhanceEquipped(EnhancementType.Healer, wSpecial: WeaponSpecial.Elysium);

        Core.RegisterQuests(8872);
        while (!Bot.ShouldExit && !Core.CheckInventory("Synthetic Viscera", quant))
            Core.KillMonster("DarkCarnax", "Boss", "Right", "Nightmare Carnax");

        Core.CancelRegisteredQuests();
        Bot.Options.AttackWithoutTarget = false;
        Adv.GearStore(true);

        Bot.Events.RunToArea -= DarkCarnaxMove;
        
        // Cancel any pending movement tasks and dispose resources
        carnaxMovementCts?.Cancel();
        carnaxMovementCts?.Dispose();
        carnaxMovementCts = null;
        carnaxMovementTask = null;
    }

    private void DarkCarnaxMove(string zone)
    {
        string zoneLower = zone?.ToLower() ?? "";
        if (zoneLower == currentCarnaxZone)
            return;

        if (DateTime.Now - lastCarnaxMove < CarnaxMoveCooldown)
            return;

        currentCarnaxZone = zoneLower;
        lastCarnaxMove = DateTime.Now;

        if (carnaxMovementTask is { IsCompleted: false })
            return;

        carnaxMovementTask = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Bot.Random.Next(1000, 1500), carnaxMovementCts?.Token ?? default);

                int y = Bot.Random.Next(380, 475);
                int x = zoneLower switch
                {
                    "a" => Bot.Random.Next(600, 931),
                    "b" => Bot.Random.Next(25, 326),
                    _ => Bot.Random.Next(325, 601)
                };

                Bot.Player.WalkTo(x, y);

                await Task.Delay(Bot.Random.Next(1500, 2500), carnaxMovementCts?.Token ?? default);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested - no action needed
            }
        }, carnaxMovementCts?.Token ?? default);
    }

}
