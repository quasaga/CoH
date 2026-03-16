using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CoH.Content.Buffs;
using CoH.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CoH.Content.Projectiles.Magic
{
	public class BloodRainProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 12;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
		}

		public override void AI()
		{	
			if (Projectile.localAI[0]++ == 35)
			{
				Projectile.tileCollide = true; // Enable collision at tick 35
			}

			Projectile.rotation = Projectile.velocity.ToRotation() -MathHelper.PiOver2;
			
			// spawn some dust
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Meteor);
				dust.color = new Color(150, 0, 0);
				dust.velocity *= 0.1f;
				dust.noGravity = true;
			}
		}

		private HashSet<int> hitNPCs = new HashSet<int>();

		public override bool? CanHitNPC(NPC target)
		{
			return !hitNPCs.Contains(target.whoAmI);
		}

		public static readonly SoundStyle HeavyBloodSplat = new SoundStyle("CoH/Content/Sounds/Custom/liquids_honey_water_", 3) // i did this a long time ago, ok?
		{
			Volume = 1f,
			PitchVariance = 0.05f,
			MaxInstances = 6
		};

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			hitNPCs.Add(target.whoAmI);
			target.immune[Projectile.owner] = 0; // no iframes for this projectile
		}

		public override void OnKill(int timeLeft)
		{
			// Spawn dust here
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Meteor);
				dust.color = new Color(150, 0, 0);
				dust.noGravity = true;
			}

			SoundEngine.PlaySound(HeavyBloodSplat, Projectile.Center);
		}
	}
}