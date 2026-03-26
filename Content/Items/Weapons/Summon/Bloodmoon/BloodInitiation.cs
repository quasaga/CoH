using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using CoH.Content.Projectiles.Summon.Bloodmoon;

namespace CoH.Content.Items.Weapons.Summon.Bloodmoon
{
    public class BloodInitiation : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.damage = 75;
            Item.knockBack = 0.1f;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = Item.useAnimation = 30;

            Item.shoot = ModContent.ProjectileType<BloodCultistSummon>();
            Item.buffType = ModContent.BuffType<BloodCultistBuff>();
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.AbigailSummon;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, 0);
            projectile.originalDamage = Item.damage;
            projectile.netUpdate = true;

            return false;
        }
    }

    public class BloodCultistBuff : ModBuff
    {
        public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			if (player.ownedProjectileCounts[ModContent.ProjectileType<BloodCultistSummon>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
    }

    public class BloodCultistSummon : ModProjectile
    {
        
        int startAttackRange = 700;
        int attackTimer = 45;
        int attackCooldown = 30;
        int spiritCharge = 0;
        int projIndex = -1;
        int dmg = 0;
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            dmg = Projectile.damage;
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<BloodCultistBuff>());
            if (player.HasBuff(ModContent.BuffType<BloodCultistBuff>()))
                Projectile.timeLeft = 2;

            projIndex = (int)Projectile.ai[0];

            bool mainExists = false;
            int candidate = -1;

            for (int i = Main.maxProjectiles - 1; i >= 0; i--)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    if (p.ai[0] != 0)
                    {
                        mainExists = true;
                        break;
                    }

                    if (candidate == -1 || p.whoAmI < candidate)
                        candidate = p.whoAmI;
                }
            }

            //Main.NewText($"candidate: {candidate} mainExists: {mainExists} index: {projIndex}");

            if (!mainExists && Projectile.whoAmI == candidate)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            if (projIndex == 0)
            {
                Projectile.hide = true;
                Projectile.damage = 0;
                Projectile.Center = player.MountedCenter;
                return;
            }
            else
            {
                Projectile.hide = false;
                Projectile.damage = dmg;
            }

            int level = player.ownedProjectileCounts[ModContent.ProjectileType<BloodCultistSummon>()];
            MoveMinion();
            DoAttack(level);
        }

        private void MoveMinion()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 targetPos = player.MountedCenter + new Vector2(48f * -player.direction, -64f);
            Vector2 dir = targetPos - Projectile.Center;

            if (dir.Length() > 4096f) Projectile.Center = targetPos;
            else
            {
                Projectile.velocity = dir / 24;
            }

            Projectile.rotation = Projectile.velocity.X * 0.08f;
            Projectile.spriteDirection = player.direction;
        }

        private void DoAttack(int level)
        {
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(startAttackRange, ref attackTarget, false);
            if (attackTarget != -1)
            {
                NPC target = Main.npc[attackTarget];

                attackTimer++;
                if (attackTimer >= attackCooldown)
                {
                    spiritCharge++;
                    if (spiritCharge >= 6 && level >= 3)
                    {
                        SpiritAttack(target, level);
                        spiritCharge = 0;
                    }
                    else
                    {
                        SpearAttack(target, level);
                    }
                    attackTimer = 0;
                }
            }
        }

        private void SpearAttack(NPC target, int level)
        {
            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < level; i++)
            {
                float angle = MathHelper.TwoPi * i / level; //shoot out in a circle
                Vector2 offset = angle.ToRotationVector2() * 192f;
                Vector2 spawnPos = target.Center + offset;
                Vector2 vel = target.Center - spawnPos;
                vel.Normalize();

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, vel * 6f, ModContent.ProjectileType<CultistSpear>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            }
        }

        private void SpiritAttack(NPC target, int level)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 vel = target.Center - Projectile.Center;
            vel.Normalize();
            Projectile.velocity = vel * -8f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel * 8f, ModContent.ProjectileType<CultistSpirit>(), Projectile.damage * (level / 2), Projectile.knockBack, player.whoAmI);
        }
    }
}