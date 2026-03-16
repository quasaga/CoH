using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.Projectiles;
using CoH.Common.Systems;
using System.Collections.Generic;
using System.Linq;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.Graphics;
using CoH.Content.Projectiles.Bosses;
using Microsoft.Xna.Framework.Graphics;

namespace CoH.Content.NPCs.Bloodmoon.Morana
{
	[AutoloadBossHead]
	public class Morana : ModNPC
	{
		public int bossPhase
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		public Vector2 targetPos {
			get => new Vector2(NPC.ai[1], NPC.ai[2]);
			set {
				NPC.ai[1] = value.X;
				NPC.ai[2] = value.Y;
			}
		}
		bool killedAll = false;
		float accelSpeed = 0.5f;
		float moveSpeed = 50f;
		float friction = 0.95f;
		int firstPhaseTimer = 0;
		const int switchCycle = 600;
		const int switchAttack = 90;
		float orbitAngle = 0f;
		float radiusX = 0f;
		float radiusY = 0f;
		bool SkullQueued = true;
		const int spearAmount = 4;
		const int spearShotsPerAttack = 2;
		int spearShotsCounter = 0;
		const int spearDelay = 60;
		int spearDelayCounter = 100;
		bool spearAttackActive = false;
		bool spearOffsetNext = false;
		bool cultistSpawned = false;
		const int dashDelay = 40;
		int dashDelayCounter = 0;
		int teleportDelay = 45;
		int teleportDelayCounter = 0;
		bool effectsApplied = false;
		Vector2 dir;
		float velFactor;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }
		public override void SetDefaults()
		{
			NPC.width = 96;
			NPC.height = 96;
			NPC.damage = 75;
			NPC.defense = 50;
			NPC.lifeMax = 30000;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(gold: 25);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = false;
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.SpawnWithHigherTime(30);
			NPC.npcSlots = 10f;
			
			NPC.HitSound = SoundID.Item167;
			NPC.DeathSound = SoundID.Item160;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);

			if (Main.netMode == NetmodeID.Server) {
				// We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
				return;
			}

			if (NPC.life <= 0)
			{
				NPC.netUpdate = true;
				Filters.Scene.Deactivate("CoH:DarknessCircle");
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(
						NPC.position,
						NPC.width,
						NPC.height,
						DustID.Frost
					);

					dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
					dust.noGravity = false;
					dust.scale = 1.5f;
				}

				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustDirect(
						NPC.position,
						NPC.width,
						NPC.height,
						DustID.Blood
					);

					dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
					dust.noGravity = false;
					dust.scale = 1.5f;
				}
				SoundEngine.PlaySound(SoundID.Item160, NPC.Center);
			}
        }

		public override void BossLoot(ref int potionType) {
			potionType = ItemID.GreaterHealingPotion;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
			return true;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			//EoL scaling
			NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment * 0.6f);

			NPC.damage = (int)(NPC.damage * 0.8f);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return bossPhase == 2 ? false : null;
		}

		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
				NPC.TargetClosest();
			}

			Player player = Main.player[NPC.target];

			if (player.dead)
			{
				NPC.velocity.Y -= 0.1f;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(10);
				return;
			}

			if (!killedAll && Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.active && npc.whoAmI != NPC.whoAmI && !npc.friendly)
					{
						npc.StrikeInstantKill();
					}
				}
				killedAll = true;
			}

			if (player.Center.X >= NPC.Center.X)
			{
				NPC.spriteDirection = -1;
			}
			else
			{
				NPC.spriteDirection = 1;
			}

			//Phase 1
			if (bossPhase == 0)
			{
				DoPhase1();
			}
			else if (bossPhase == 1)
			{
				DoPhase2();
			}
			else if (bossPhase == 2)
			{
				DoPhase3();
			}

			dir = targetPos - NPC.Center;
			dir.Normalize();

			Vector2 velDir = (NPC.velocity != Vector2.Zero) ? Vector2.Normalize(NPC.velocity) : Vector2.Zero;
			float alignment = Vector2.Dot(velDir, dir);

			velFactor = accelSpeed * Math.Max(alignment, 0.5f);

			NPC.velocity += dir * velFactor;
			NPC.velocity *= friction;
		}

		public void DoPhase1()
		{
			Player player = Main.player[NPC.target];

			firstPhaseTimer++;
			int cycleTime = firstPhaseTimer % switchCycle;
			
			if (cycleTime < switchCycle / 2)
			{
				radiusX = 600f;
				radiusY = 400f;
				orbitAngle -= 0.025f;
			}
			else
			{
				radiusX = 100f;
				radiusY = 67f;
				orbitAngle += 0.04f;
			}

			targetPos = player.Center + new Vector2((float)Math.Cos(orbitAngle) * radiusX, (float)Math.Sin(orbitAngle) * radiusY);

			if (Main.netMode == NetmodeID.MultiplayerClient) return;

			if (cycleTime == switchAttack || cycleTime == switchAttack + switchCycle / 2)
			{
				if (SkullQueued)
				{
					SkullAtk(player.Center);
				}
				else
				{
					spearAttackActive = true;
					spearShotsCounter = 0;
					spearDelayCounter = spearDelay - 1; // so first volley fires immediately
				}
			}

			if (spearAttackActive)
			{
				spearDelayCounter++;
				if (spearDelayCounter >= spearDelay)
				{
					Main.NewText($"until shot: {spearDelay - spearDelayCounter}");

					spearDelayCounter = 0;
					SpearAtk(player.Center);
					spearShotsCounter++;

					if (spearShotsCounter >= spearShotsPerAttack)
					{
						spearShotsCounter = 0;
						spearAttackActive = false;
						SkullQueued = true;
					}
				}
			}

			if (NPC.life <= NPC.lifeMax * .34f)
			{
				bossPhase = 1;
				NPC.netUpdate = true;
			}
		}

		public void DoPhase2()
		{
			Player player = Main.player[NPC.target];

			accelSpeed = 1.5f;
			friction = 0.92f;
			NPC.dontTakeDamage = true;
			SpawnCultists();

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				targetPos = new Vector2(player.Center.X, player.Center.Y - 450f);
				BloodCultistAttackManager.UpdateTick();
				//Main.NewText("boss ready:" + BloodCultistAttackManager.attackTickReady);

				if (BloodCultistAttackManager.attackTickReady)
				{
					// Pick active BloodCultistA NPCs
					List<int> cultists = new List<int>();
					for (int i = 0; i < Main.npc.Length; i++)
					{
						if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<BloodCultistA>())
							cultists.Add(i);
					}

					if (cultists.Count >= 2)
					{
						// Shuffle and select 2 attackers
						cultists = cultists.OrderBy(x => Main.rand.Next()).ToList();
						BloodCultistAttackManager.attackers[0] = cultists[0];
						BloodCultistAttackManager.attackers[1] = cultists[1];

						// Assign unique random attacks
						List<int> options = new List<int> { 0, 1, 2 };
						BloodCultistAttackManager.assignedAttack[0] = options[Main.rand.Next(options.Count)];
						options.Remove(BloodCultistAttackManager.assignedAttack[0]);
						BloodCultistAttackManager.assignedAttack[1] = options[Main.rand.Next(options.Count)];
					}

					if (cultists.Count == 0)
					{
						bossPhase = 2;
						NPC.netUpdate = true;
						Filters.Scene.Deactivate("CoH:ArenaCircle");
					}
				}
			}
		}

		public void DoPhase3()
		{
			Player player = Main.player[NPC.target];

			if (!effectsApplied && Main.netMode != NetmodeID.Server)
			{
				Filters.Scene.Activate("CoH:DarknessCircle");
				effectsApplied = true;
			}
			else if (Main.netMode != NetmodeID.Server)
			{
				Filters.Scene["CoH:DarknessCircle"].GetShader().UseTargetPosition(player.Center).UseIntensity(560f);
			}

			accelSpeed = 0.33f;
			friction = 0.93f;
			NPC.defense = 30;
			NPC.dontTakeDamage = false;

			targetPos = player.Center;

			teleportDelayCounter++;
			if (teleportDelayCounter >= teleportDelay)
			{
				int dashAmount = Main.rand.Next(3, 5 + 1);
				teleportDelay = dashAmount * dashDelay - 1;
				teleportDelayCounter = 0;
				Teleport();
			}

			dashDelayCounter++;
			if (dashDelayCounter >= dashDelay)
			{
				dashDelayCounter = 0;
				NPC.velocity = dir * moveSpeed * .5f;
			}
		}
		public void SkullAtk(Vector2 targetPos)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;

			SkullQueued = false;
			int occultSkull = ModContent.NPCType<OccultSkull>();
			int frigidSkull = ModContent.NPCType<FrigidSkull>();
			float radius = 150f;
			for (int i = 0; i < 3; i++)
			{
				float angle = MathHelper.TwoPi * i / 6f;
				Vector2 offset = angle.ToRotationVector2() * radius;
				Vector2 spawnPos = NPC.Center - offset;

				NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, occultSkull);
			}
			for (int i = 0; i < 2; i++)
			{
				float angle = MathHelper.TwoPi * i / 8f;
				Vector2 offset = angle.ToRotationVector2() * radius;
				Vector2 spawnPos = NPC.Center - offset;

				NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, frigidSkull);
			}
			SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
			Vector2 dir = (targetPos - NPC.Center) * -1;
			dir.Normalize();
			NPC.velocity = dir * moveSpeed * 0.2f;
		}

		public void SpearAtk(Vector2 targetPos)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;

			float radius = Main.screenWidth / 2f + 32f;
			int frostSpear = ModContent.ProjectileType<FrostSpear>();
			int damage = 30;
			float knockBack = 5f;
			float startAngle = spearOffsetNext ? MathHelper.PiOver4 : 0f;

			for (int i = 0; i < spearAmount; i++)
			{
				float angle = startAngle + MathHelper.TwoPi * i / spearAmount;
				Vector2 offset = angle.ToRotationVector2() * radius;
				Vector2 spawnPos = targetPos + offset;
				Vector2 dir = targetPos - spawnPos;
				dir.Normalize();
				Vector2 vel = dir * 22f;

				Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, vel, frostSpear, damage, knockBack);
			}

			SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, targetPos);
			spearOffsetNext = !spearOffsetNext;
		}

		public void SpawnCultists()
		{
			if (cultistSpawned) return;

			if (Main.netMode != NetmodeID.Server)
			{
				Player player = Main.player[NPC.target];
				Filters.Scene["CoH:ArenaCircle"].GetShader().UseTargetPosition(player.Center).UseIntensity(720f).UseProgress(Main.GameUpdateCount * 0.016f).UseImage(AssetLoader.voronoiTexture);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient) return;

			int bloodCultist = ModContent.NPCType<BloodCultistA>();

			for (int cultists = 0; cultists < 3; cultists++)
			{
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, bloodCultist);
			}
			cultistSpawned = true;
		}

		public void Teleport()
		{
			Player player = Main.player[NPC.target];
			int dir = Main.rand.NextBool() ? 1 : -1;
			int screenEdge = 768;
			Vector2 targetPos = new Vector2(player.Center.X + screenEdge * dir, player.Center.Y);

			NPC.Center = targetPos;
			NPC.netUpdate = true;
		}
	}
}
