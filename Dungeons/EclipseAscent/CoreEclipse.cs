//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs

using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using Skua.Core.Scripts;

namespace SkuaScripts.Scripts.Custom.EclipseAscent;

public class CoreEclipse {
	private IScriptInterface Bot => IScriptInterface.Instance;
	private CoreBots C => CoreBots.Instance;
	private CoreEngine Core = new();
	private CoreUltra Ultra = new();
	private static CoreAdvanced Adv = new();
	private static CoreArmyLite sArmy = new();

	public void EquipWait(string item = "Scroll of Enrage") {
		C.Equip(item);
		Bot.Wait.ForItemEquip(item);
		Bot.Wait.ForActionCooldown(GameActions.EquipItem);
		C.Sleep();
	}

	public void SetupParty() {
		if (!Bot.Player.Username.Equals(Bot.Config!.Get<string>("player1").ToLower())) return;
		
		if (!sArmy.PartyMemberArray().Contains(Bot.Config!.Get<string>("player2")!.ToLower())) {
			sArmy.PartyInvite(Bot.Config!.Get<string>("player2")!);
			Bot.Sleep(2000);
		}
		if (!sArmy.PartyMemberArray().Contains(Bot.Config!.Get<string>("player3")!.ToLower())) {
			sArmy.PartyInvite(Bot.Config!.Get<string>("player3")!);
			Bot.Sleep(2000);
		}
		if (!sArmy.PartyMemberArray().Contains(Bot.Config!.Get<string>("player4")!.ToLower())) {
			sArmy.PartyInvite(Bot.Config!.Get<string>("player4")!);
			Bot.Sleep(2000);
		}
		
		Bot.Sleep(10000);
	}

	public void EquipClasses(bool autoEnhance) {
		if (!Bot.Config!.Get<bool>("autoclass")) return;
		if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player1").ToLower()) && C.CheckInventory("Legion Revenant")) {
			C.Equip("Legion Revenant");
			if (!autoEnhance) return;
			Adv.EnhanceEquipped(
				type: EnhancementType.Wizard,
				hSpecial: HelmSpecial.Pneuma,
				wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast,
				cSpecial: CapeSpecial.Vainglory
			);
		}
		else if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player2").ToLower()) && C.CheckInventory("StoneCrusher")) {
			C.Equip("StoneCrusher");
			if (!autoEnhance) return;
			Adv.EnhanceEquipped(
				type: EnhancementType.Fighter,
				hSpecial: HelmSpecial.Anima,
				wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance,
				cSpecial: CapeSpecial.Absolution
			);
		}
		else if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player3").ToLower()) && C.CheckInventory("ArchPaladin")) {
			C.Equip("ArchPaladin");
			if (!autoEnhance) return;
			Adv.EnhanceEquipped(
				type: EnhancementType.Lucky,
				hSpecial: HelmSpecial.Forge,
				wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast,
				cSpecial: CapeSpecial.Penitence
			);
		}
		else if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player4").ToLower()) && C.CheckInventory("Lord Of Order")) {
			C.Equip("Lord Of Order");
			if (!autoEnhance) return;
			Adv.EnhanceEquipped(
				type: EnhancementType.Lucky,
				hSpecial: HelmSpecial.Forge,
				wSpecial: Adv.uArcanasConcerto()
					? WeaponSpecial.Arcanas_Concerto
					: WeaponSpecial.Awe_Blast,
				cSpecial: CapeSpecial.Penitence
			);
		}
		else {
			C.Logger("Valid class not found");
			Bot.StopSync(true);
		}
	}
	
	public void GetScrollOfEnrage(int count = 100) {
		if (!Core.Faction("SpellCrafting", 5))
			return;

		const string parchment = "Mystic Parchment";
		const string ink = "Zealous Ink";
		const string scroll = "Scroll of Enrage";

		while (!Bot.ShouldExit && !C.CheckInventory(scroll, count)) {
			// Mats
			Core.ForItem("Undead Infantry", "underworld", parchment, 2);
			Core.BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

			// Craft
			Core.Join("spellcraft");
			Bot.Drops.Add(scroll);
			Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%");
			Bot.Sleep(5000);
			Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");

			Core.WaitForDrop(scroll, 10000);
			Core.Pickup(scroll);
		}
	}

	public void GetSliverOfMoonlight(int count = 221) { //220 for the sword, 1 for Rite of Ascension
		new SolsticeMoon(this, count);
	}

	public void GetSliverOfSunlight(int count = 221) { //220 for the sword, 1 for Rite of Ascension
		new MidnightSun(this, count);
	}

	public void GetEclipticOffering(int count = 155) { //155 for the sword
		new AscendEclipse(this, count);
	}

	private bool needsEnrage;
	private bool usedEnrage;
	private bool usedLastEnrage;
	private DateTimeOffset tauntTime;
	private int deathCount;
	//This is mostly a copy of CoreAOR.ColdThunderBoss
	public void EclipseBossHandler(string startingEnragePlayer, string bossName, string enrageMessage, bool alternateEnrage = true, string bossCell = "r3", int tauntOffset = 0, int deathResetCount = 0, params string[] deathMessages) {
		needsEnrage = false;
		usedEnrage = false;
		usedLastEnrage = alternateEnrage
			? Bot.Player.Username.Equals(Bot.Config!.Get<string>(startingEnragePlayer).ToLower())
			: false;
		deathCount = 0;
		
		Bot.Events.ScriptStopping += OnBotStopped;
		Bot.Flash.FlashCall += Listener;
		
		//TODO: startingEnragePlayer should start the fight by using a scroll?
		//TODO: Skip the enrage if they missed it badly enough that the other player's enrage is up

		C.Logger(
			$"About to attack {bossName} boss."
		);

		while (!Bot.ShouldExit && Bot.Player.Cell == bossCell && 
		       Bot.Monsters.CurrentMonsters.Any(x => x.Alive && x.Name.Equals(bossName))) 
		{
			if (needsEnrage && !usedEnrage && !usedLastEnrage && Bot.Player.HasTarget && (tauntOffset <= 0 || DateTimeOffset.Now > tauntTime))
			{
				C.Logger(
					$"Detected '{enrageMessage}' - applying Scroll of Enrage..."
				);
				
				Bot.Skills.Pause();

				// Keep trying to use scroll until it's successfully applied
				while (!Bot.ShouldExit && Bot.Player.HasTarget && Bot.Player.Cell == bossCell && 
				       Bot.Monsters.CurrentMonsters.Any(x => x.Alive && x.Name.Equals(bossName)) && needsEnrage && !usedEnrage && !usedLastEnrage) 
				{
					Bot.Combat.CancelAutoAttack();
					
					C.UsePotion();
					C.Sleep(200); // Small delay to allow scroll to apply

					if (Bot.Player.HasTarget && (Bot.Target.Auras.Any(x => x.Name.Equals("Focus", StringComparison.OrdinalIgnoreCase)  && x.RemainingTime > 4)) || 
					    Bot.Target.Auras.Any(x => x.Name.Equals("Reckless", StringComparison.OrdinalIgnoreCase)  && x.RemainingTime > 4))
					{
						usedEnrage = true;
						needsEnrage = false;
						usedLastEnrage = alternateEnrage;
						C.Logger("Enraged successfully!");
					}
					else
					{
						C.Sleep(200); // Brief pause before retrying
					}
				}
			}
			else if (needsEnrage && !usedEnrage && usedLastEnrage) {
				C.Logger(
					$"Detected '{enrageMessage}' - other player enrages..."
				);
				usedEnrage = true;
				needsEnrage = false;
				usedLastEnrage = false;
			}
			
			// Only attack if no scroll is needed or scroll has been applied
			if (!needsEnrage || usedEnrage || !Bot.Player.HasTarget)
			{
				Bot.Skills.Resume();
				Bot.Combat.Attack(bossName);
			}

			if (deathResetCount > 0) {
				if (deathCount >= deathResetCount) {
					ResetFight();
					goto Stop;
				}
			}
			
			C.Sleep();
		}
		
		Stop:
		
		Bot.Events.ScriptStopping -= OnBotStopped;
		Bot.Flash.FlashCall -= Listener;
		
		usedEnrage = false;
		needsEnrage = false;
		
		#region Packet Listener

		void Listener(string name, object[] args) {
			switch (name) {
				case "pext":
					var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
					if (packet["params"] is not null && packet["params"]["type"] is not null) {
						string type = packet["params"]["type"];
						if (type is "json" && packet["params"]["dataObj"] is not null) {
							DataHandler(packet["params"]["dataObj"]);
						}
					}
					break;
				case "packetFromServer":
					packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
					if (packet["b"] is not null && packet["b"]["o"] is not null) {
						DataHandler(packet["b"]["o"]);
					}
					break;
			}
		}

		void DataHandler(dynamic data) {
			if (data["cmd"] is null) return;
			string cmd = data["cmd"].ToString();
			switch (cmd)
			{
				case "ct":
					if (data["anims"] is not null)
					{
						foreach (var a in data.anims)
						{
							if (a is null)
								continue;

							if (a.msg is not null)
							{
								if (((string)a.msg).ToLower().Contains(enrageMessage.ToLower()))
									goto Enrage;
							}
						}
					}

					if (data["a"] is not null) {
						foreach (var a in data.a)
						{
							if (a is null || a["cmd"].ToString() is not "aura+")
								continue;
							if (a["auras"] is not null) {
								foreach (var aura in a["auras"]) {
									if (
										aura is not null
										&& aura["msgOn"] is not null
									)
									{
										if (((string)aura.msgOn).ToLower().Contains(enrageMessage.ToLower())
										    && ((bool)aura.isNew))
											goto Enrage;
									}
								}
							}
						}
					}
					break;
				case "umsg":
					if (data["s"] is not null && deathResetCount > 0)
						foreach (var deathMessage in deathMessages) {
							if (!((string)data["s"]).ToLower().Contains(deathMessage.ToLower())) continue;
							deathCount++;
							C.Logger("Death detected, logging...");
						}
					break;
			}
			
			return;
			
			Enrage:
			needsEnrage = true;
			usedEnrage = false;
			if (tauntOffset > 0) {
				tauntTime = DateTimeOffset.Now.AddSeconds(tauntOffset);
				C.Logger(
					$"Event detected: {enrageMessage}. Prepare yourself! - Enrage needed in {tauntOffset} seconds."
				);
			}
			else {
				C.Logger(
					$"Event detected: {enrageMessage}. Prepare yourself! - Enrage needed."
				);
			}
		}
		#endregion

		void ResetFight() {
			C.Logger("Too many deaths, resetting the fight...");
			
			Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
			
			C.JumpWait();
			
			//Try to avoid potions bug
			C.Unbank("Healer");
			C.Equip("Healer");
			EquipClasses(false);
			
			const string syncPath = "EclipseAscentReset";
			Ultra.ClearSyncFile(syncPath);
			Bot.Sleep(2500);
			Ultra.WaitForArmy(3, syncPath, timeoutMs: 10000);
		}

		bool OnBotStopped(Exception? e) {
			Bot.Events.ScriptStopping -= OnBotStopped;
			Bot.Flash.FlashCall -= Listener;
			
			return true;
		}
	}

	private class EclipseBase {
		protected CoreEclipse Eclipse;
		protected IScriptInterface Bot => Eclipse.Bot;
		protected CoreBots C => Eclipse.C;
		protected CoreUltra Ultra => Eclipse.Ultra;
		protected bool doGetEnrage = true;

		protected EclipseBase(CoreEclipse eclipse) {
			Eclipse = eclipse;
		}

		protected void Restart(string packet) {
			if (doGetEnrage) {
				doGetEnrage = false;
				if (!C.CheckInventory("Scroll of Enrage", 250))
					Eclipse.GetScrollOfEnrage(1000);
				
				if (C.CheckInventory("Hallowed Remains", 400))
					C.SellItem("Hallowed Remains", all: true);
			
				const string syncPath = "EclipseAscentRestart";
				Ultra.ClearSyncFile(syncPath);
				Ultra.WaitForArmy(3, syncPath);
			}
			
			Bot.Sleep(2000);
			if (sArmy.isPartyLeader()) {
				Bot.Send.Packet(packet);
			}
			else if (sArmy.getPartyLeader() != null) {
				Bot.Player?.Goto(sArmy.getPartyLeader()!);
			}
		}

		protected void Move(string packet) {
			switch (Bot.Player?.Cell) {
				case "Enter":
					C.Jump("r1");
					doGetEnrage = true;
					break;
				case "r1":
					C.Jump("r2");
					break;
				case "r2":
					C.Jump("r3");
					break;
				case "r3a":
				case "r3":
					Restart(packet);
					Bot.Wait.OverrideTimeout = true;
					Bot.Wait.ForCellChange("Enter");
					Bot.Wait.OverrideTimeout = false;
					//Try to avoid potions bug
					C.Unbank("Healer");
					C.Equip("Healer");
					Eclipse.EquipClasses(false);
					break;
			}
		}
	}
	
	private class SolsticeMoon : EclipseBase {
		public SolsticeMoon(CoreEclipse eclipse, int count) : base(eclipse) {
			while (!Bot.ShouldExit && !C.CheckInventory("Sliver of Moonlight", count)) {
				if (Bot.Map.Name != "solsticemoon") {
					Restart("%xt%zm%dungeonQueue%24946%solsticemoon%");
					Bot.Wait.ForMapLoad("solsticemoon");
					//Try to avoid potions bug
					C.Unbank("Healer");
					C.Equip("Healer");
					Eclipse.EquipClasses(false);
				}
				else {
					Kill();
				}
			}
		}

		private bool IsTaunter() {
			return Bot.Player.Username.Equals(Bot.Config!.Get<string>("player3").ToLower()) || Bot.Player.Username.Equals(Bot.Config!.Get<string>("player4").ToLower());
		}

		private void Kill() {
			Bot.Sleep(1000);
			if (Bot.Monsters.CurrentMonsters.Any(x => x.Alive))
				if (Bot.Player.Cell.Equals("r3") && IsTaunter()) 
					Eclipse.EclipseBossHandler("player4", "Hollow Midnight", "The Moon Converges");
				else
					Bot.Combat.Attack("*");
			else
				Move("%xt%zm%dungeonQueue%24946%solsticemoon%");
		}
	}
	
	private class MidnightSun : EclipseBase {
		public MidnightSun(CoreEclipse eclipse, int count) : base(eclipse) {
			while (!Bot.ShouldExit && !C.CheckInventory("Sliver of Sunlight", count)) {
				if (Bot.Map.Name != "midnightsun") {
					Restart("%xt%zm%dungeonQueue%25127%midnightsun%");
					Bot.Wait.ForMapLoad("midnightsun");
					//Try to avoid potions bug
					C.Unbank("Healer");
					C.Equip("Healer");
					Eclipse.EquipClasses(false);
				}
				else {
					Kill();
				}
			}
		}

		private bool IsTaunter() {
			return Bot.Player.Username.Equals(Bot.Config!.Get<string>("player1").ToLower()) || Bot.Player.Username.Equals(Bot.Config!.Get<string>("player2").ToLower());
		}

		private void Kill() {
			Bot.Sleep(1000);
			if (Bot.Monsters.CurrentMonsters.Any(x => x.Alive))
				if (Bot.Player.Cell.Equals("r3") && IsTaunter()) 
					Eclipse.EclipseBossHandler("player2", "Hollow Solstice", "The Sun Converges");
				else if (IsTaunter() && Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Dying Light" }))
					Eclipse.EclipseBossHandler("player1", "Dying Light", "The Light Gathers", bossCell: Bot.Player.Cell); 
				else
					Bot.Combat.Attack("*");
			else
				Move("%xt%zm%dungeonQueue%25127%midnightsun%");
		}
	}
	
	private class AscendEclipse : EclipseBase {
		public AscendEclipse(CoreEclipse eclipse, int count) : base(eclipse) {
			while (!Bot.ShouldExit && !C.CheckInventory("Ecliptic Offering", count)) {
				if (Bot.Map.Name != "ascendeclipse") {
					Restart("%xt%zm%dungeonQueue%15395%ascendeclipse%");
					Bot.Wait.ForMapLoad("ascendeclipse");
					//Try to avoid potions bug
					C.Unbank("Healer");
					C.Equip("Healer");
					Eclipse.EquipClasses(false);
				}
				else {
					Kill();
				}
			}
		}

		private bool IsTeamSun() {
			return Bot.Player.Username.Equals(Bot.Config!.Get<string>("player1").ToLower()) || Bot.Player.Username.Equals(Bot.Config!.Get<string>("player2").ToLower());
		}

		private void Kill() {
			Bot.Sleep(1000);
			if (Bot.Monsters.CurrentMonsters.Any(x => x.Alive))
					switch (Bot.Player.Cell) {
						case "Enter":
							if (Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Fallen Star" }))
								if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player4").ToLower()) && Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Blessless Deer" }))
									Bot.Combat.Attack("Blessless Deer");
								else
									Bot.Combat.Attack("Fallen Star");
							else
								Bot.Combat.Attack("*");
							break;
						case "r1":
							if (IsTeamSun() && Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Suffocated Light" }))
								Eclipse.EclipseBossHandler("player1", "Suffocated Light", "The Light Gathers", bossCell: Bot.Player.Cell);
							else
								Bot.Combat.Attack("*");
							break;
						case "r2":
							if (Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Sunset Knight" })) {
								if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player4").ToLower()) &&
								    Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Moon Haze" }))
									Eclipse.EclipseBossHandler("player4", "Moon Haze", "You gaze into the moon",
										bossCell: Bot.Player.Cell, alternateEnrage: false, tauntOffset: 6);
								else if (Bot.Player.Username.Equals(Bot.Config!.Get<string>("player3").ToLower()) &&
								    Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Sunset Knight" }))
									Eclipse.EclipseBossHandler("player3", "Sunset Knight", "You feel the warmth of the sun",
										bossCell: Bot.Player.Cell, alternateEnrage: false, tauntOffset: 6);
								else
									Bot.Combat.Attack("Sunset Knight");
							}
							else
								Bot.Combat.Attack("*");
							break;
						case "r3":
							if(IsTeamSun() && Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Ascended Solstice" }))
								Eclipse.EclipseBossHandler("player1", "Ascended Solstice", "The Sun Converges", deathResetCount: 2, deathMessages:
									["The Ascended Midnight glows brighter", "The Ascended Solstice burns hotter"]);
							else if (Bot.Monsters.CurrentMonsters.Any(x => x is { Alive: true, Name: "Ascended Midnight" }))
								Eclipse.EclipseBossHandler("player3", "Ascended Midnight", "The Moon Converges", deathResetCount: 2, deathMessages:
									["The Ascended Midnight glows brighter", "The Ascended Solstice burns hotter"]);
							else
								Bot.Combat.Attack("*");
							break;
						default:
							Bot.Combat.Attack("*");
							break;
					}
			else
				Move("%xt%zm%dungeonQueue%15395%ascendeclipse%");
		}
	}
}