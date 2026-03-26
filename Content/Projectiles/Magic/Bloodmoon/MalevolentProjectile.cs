using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Magic.Bloodmoon
{
	public class MalevolentProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 76; // The width of projectile hitbox
			Projectile.height = 72; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = 25; // How many monsters the projectile can penetrate.
			Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
			Projectile.knockBack = 0f;
		}

		public override void AI()
		{	
			Projectile.velocity *= 0.93f;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 knockbackDirection = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
    		target.velocity = knockbackDirection * hit.Knockback;

			int healAmount = (int)(damageDone * 0.1f); // Heal 10% of final damage dealt

			if (healAmount != 0)
			{
			player.statLife += healAmount;
			player.HealEffect(healAmount); // Shows the green healing numbers
			}
		}

		public override void OnKill(int timeLeft)
		{
			// Spawn dust here
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Meteor, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 0, default, 1f);
				dust.velocity *= 2f;
				dust.noGravity = true;
			}
		}
	}
}