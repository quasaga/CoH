using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Melee.Desert
{
	public class HealingOrb : ModProjectile
	{
		private bool IsCrit => Projectile.ai[0] > 0f;
		float maxSpeed = 12f;
		float accel = 0.04f;
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Type] = 12; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1800;
			Projectile.light = 1f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			Projectile.alpha = 0;
			Projectile.scale = 0.8f;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 9; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.GreenTorch, -Projectile.velocity.X, -Projectile.velocity.Y);
				dust.noGravity = true;
			}
		}

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

			if (IsCrit)
			{
				Projectile.scale = 1f;
				accel *= 5f;
			}
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			
			float targetAngle = Projectile.AngleTo(player.Center);
			float length = Projectile.velocity.Length();
			length = length >= maxSpeed ? length = maxSpeed : length += accel;
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(length)).ToRotationVector2() * length;

			if (Projectile.Hitbox.Intersects(player.Hitbox))
			{
				int healAmount = Projectile.damage / 10; // or Projectile.damage
				player.statLife += healAmount;
				if (IsCrit)
				{
					CombatText.NewText(Projectile.Hitbox, Color.LawnGreen, healAmount, true, false);
				}
				else
				{
					player.HealEffect(healAmount);
				}

				Projectile.Kill(); // destroy after healing
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				float scale = Projectile.scale * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}