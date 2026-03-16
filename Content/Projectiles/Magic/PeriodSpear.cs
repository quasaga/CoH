using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using System;
using CoH.Content.Items.Weapons.Bloodmoon;

namespace CoH.Content.Projectiles.Magic
{
	public class PeriodSpear : ModProjectile
	{
		Vector2 offset;
		bool isLocked = true;
		int baseDamage = 0;
		float projSpeed = 18f;
		bool spawnedDusts = false;
		float radius = 0f;
		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			DrawOriginOffsetX = -34;
			DrawOriginOffsetY = 18;
			Projectile.aiStyle = -1;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1600;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
			Projectile.alpha = 255;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Player player = Main.player[Projectile.owner];
			baseDamage = Projectile.damage;
			offset = Projectile.Center - player.MountedCenter;


			radius = offset.Length();
			var discharge = player.GetModPlayer<BloodyDischargePlayer>();
			Projectile.ai[0] = MathHelper.TwoPi * discharge.currentCharge / discharge.maxCharge;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 8;
			}

			if (player.channel && isLocked)
			{
				Projectile.Center = player.MountedCenter + offset;
				var discharge = player.GetModPlayer<BloodyDischargePlayer>();
				if (discharge.currentCharge >= discharge.maxCharge)
				{
					Projectile.ai[0] += 0.01f;
					float angle = Projectile.ai[0];

					Vector2 orbitOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
					Projectile.Center = player.MountedCenter + orbitOffset;
				}
				Vector2 dir = Main.MouseWorld - Projectile.Center;
				dir.Normalize();
				Projectile.rotation = dir.ToRotation();
				Projectile.damage = 0;
				Projectile.timeLeft = 1600;
			}
			else
			{
				isLocked = false;
				Projectile.tileCollide = true;
				Projectile.velocity = Projectile.rotation.ToRotationVector2() * projSpeed ;
				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.damage = baseDamage;
				if (Main.rand.NextBool(4))
				{
					int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.TheDestroyer);
					Main.dust[dust].velocity = Vector2.Zero;
					Main.dust[dust].noGravity = true;
				}

				if (spawnedDusts) return;
				for (int i = 0; i < 24; i++)
				{
					int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.TheDestroyer);
					Main.dust[dust].velocity = (Main.rand.NextVector2CircularEdge(0.5f, 1f) * 2f).RotatedBy(Projectile.rotation);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].scale = 2f;
				}
				spawnedDusts = true;
			}
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
			behindNPCsAndTiles.Add(index);
        }

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 24; i++)
			{
				Vector2 dir = Projectile.velocity;
				dir.Normalize();
				Dust dust = Dust.NewDustDirect(Projectile.Center + dir * 32f, 1, 1, DustID.TheDestroyer);
				dust.velocity = (dir * Main.rand.NextFloat(-9f, -2f)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20f, 20f)));
				dust.noGravity = true;
				dust.scale = 2f;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.position);
		}
	}
}