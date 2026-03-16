using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Melee
{
	public class SicklyHarvestProjectile : ModProjectile
	{
		int plrDir = 0;
		float radius = 0f;

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Type] = 12; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Type] = 2; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 35;
			Projectile.height = 39;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 67; // im being so for real, this is 270 degrees
			Projectile.light = 1f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 6;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Player player = Main.player[Projectile.owner];

			Vector2 dir = Main.MouseWorld - player.MountedCenter;
			dir.Normalize();

			radius = 56f;
			Projectile.ai[0] = dir.ToRotation() + (player.direction * MathHelper.ToRadians(-135f));
			plrDir = player.direction;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			// increase angle
			Projectile.ai[0] += 0.07f * plrDir;

			float angle = Projectile.ai[0];

			Vector2 orbitOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

			Projectile.Center = player.MountedCenter + orbitOffset;

			Projectile.spriteDirection = plrDir;
			Projectile.rotation = angle + MathHelper.PiOver2 + (MathHelper.PiOver4 * -plrDir);

			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(player.MountedCenter + orbitOffset * 2, 1, 1, DustID.TheDestroyer);
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
				dust.scale = 0.8f;
			}
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
			behindNPCsAndTiles.Add(index);
        }

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 9; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TheDestroyer);
				dust.noGravity = true;
				dust.scale = 1.5f;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.position);
		}

		
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets. Projectile[Type].Value;
			Vector2 origin;

			
			SpriteEffects effects;
			if (Projectile.spriteDirection > 0)
			{
				origin = new Vector2(0, Projectile.height);
				effects = SpriteEffects.None;
			}
			else
			{
				origin = new Vector2(Projectile.width, Projectile.height);
				effects = SpriteEffects. FlipHorizontally;
			}
			for (int i = 0; i < Projectile.oldPos.Length; i += 3)
			{
				float progress = (float)i / Projectile.oldPos.Length;
				float alpha = 1f - progress;

				Color color = Color.Lerp(Color.Red, Color.DarkRed, progress);
				color *= alpha * 0.2f;

				Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[i], origin, Projectile.scale, effects, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile. Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
			return false;
		}
	}
}