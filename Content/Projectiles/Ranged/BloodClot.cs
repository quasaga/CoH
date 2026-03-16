using CoH.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Ranged
{
	public class BloodClot : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Type] = 2; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
			Projectile.light = 0f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;
		}

		int timer = 0;

		public override void AI()
		{
			timer++;
			if (timer >= 5)
			{
				Projectile.tileCollide = true;
			}

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 12;
			}

			Projectile.velocity.Y += 0.01f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			Projectile.rotation += 0.03f * (float)Projectile.direction;
		}

		public override void OnKill(int timeLeft)
		{
			int bulletType = (int)Projectile.ai[0];
			int bulletDamage = (int)Projectile.ai[1];

			int bulletsToShoot = 12;
			bulletDamage = bulletType == ProjectileID.ChlorophyteBullet ? bulletDamage / 4 : bulletDamage / 2;

			if (bulletType > 0)
			{
				for (int i = 0; i < bulletsToShoot; i++)
				{
					float angle = MathHelper.TwoPi * i / bulletsToShoot; //shoot out in a circle
					Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(8f, 16f);
					Projectile.NewProjectile(
						Projectile.GetSource_FromThis(),
						Projectile.Center,
						velocity,
						bulletType,
						bulletDamage,
						Projectile.knockBack,
						Projectile.owner
					);
				}
			}

			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodClotExplosion>(), bulletDamage, 0, Projectile.owner);

			Player player = Main.player[Projectile.owner];

			var impactPlayer = player.GetModPlayer<ImpactPlayer>();

			impactPlayer.StartShake(20, 4.5f);
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}