using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using CoH.Content.Items.Weapons.Snow;
using CoH.Content.Projectiles.Magic;
using Terraria.DataStructures;

namespace CoH.Content.Projectiles.Magic
{
	public class CryoRang : ModProjectile
	{
		private Vector2 baseVel;
		private int baseDamage;
		private bool damageIncreased = false;
		private int timer = 0;
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
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.light = .2f;
            Projectile.timeLeft = 420;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 25; // frames of local i-frames
		}

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

			baseVel = Projectile.velocity;
			baseDamage = Projectile.damage;
        }

		public override void AI()
		{
			Projectile.velocity -= baseVel * 0.02f;
            Projectile.rotation += 0.1f;

			timer++;
			if (timer >= 103)
			{
				if (!damageIncreased) Projectile.damage += baseDamage; damageIncreased = true;

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
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(0.3f)).ToRotationVector2() * length;
			}

			if (Projectile.velocity.Length() > 16f)
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= 16f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

			if (Main.rand.NextBool(2)) target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

			for (int i = 0; i < 8; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
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