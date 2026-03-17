using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CoH.Content.Buffs;
using CoH.Common.Players;
using Microsoft.Xna.Framework.Graphics;

namespace CoH.Content.Projectiles.Magic
{
	public class SandProjectile : ModProjectile
	{
		int dmg = 0;
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 100;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.knockBack = 0f;
			Projectile.rotation = Main.rand.NextFloat(0f, 90f);
		}

		public override string Texture => "Terraria/Images/Projectile_42";

		public override void AI()
		{	
			Projectile.velocity.Y = Projectile.velocity.Y + 0.04f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			Projectile.rotation += 0.4f;

			if (Main.rand.NextBool(12))
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

			if (Main.rand.NextBool(10)) target.AddBuff(BuffID.Confused, 180);
			target.AddBuff(ModContent.BuffType<Blinded>(), 420);
        }

		public override void OnKill(int timeLeft)
		{
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandExplosion>(), 0, 0, Projectile.owner);
		}
	}
}