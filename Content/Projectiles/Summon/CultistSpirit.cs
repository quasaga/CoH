using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Summon
{
	public class CultistSpirit : ModProjectile
	{
		private float maxDetectRadius = 720f;

		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
            Projectile.damage = 1;
            Projectile.knockBack = 4f;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = 1;
			Projectile.light = 0f;
            Projectile.timeLeft = 420;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; // frames of local i-frames
		}

		public override void AI()
		{
			if (HomingTarget == null)
			{
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			if (HomingTarget != null && !IsValidTarget(HomingTarget))
			{
				HomingTarget = null;
			}

			if (HomingTarget == null) return;

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(4f)).ToRotationVector2() * length;

			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

			target.AddBuff(BuffID.BrokenArmor, 300);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

			for (int i = 0; i < 8; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
			}

			for (int i = 0; i < 2; i++)
			{
				int dir = i == 1 ? 1 : -1;
				Vector2 vel = Projectile.velocity;
				vel.Normalize();
				vel = -vel;
				vel = vel.RotatedBy(MathHelper.PiOver4 * dir);
				vel *= 9f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<CultistSpiritA>(), Projectile.damage / 2, 0.1f, Projectile.owner);
			}
        }

		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs) {
				if (IsValidTarget(target)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target)
		{
			return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
		}
	}
}