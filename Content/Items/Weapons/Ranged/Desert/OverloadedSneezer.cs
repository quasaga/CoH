using CoH.Content.Projectiles.Ranged.Desert;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace CoH.Content.Items.Weapons.Ranged.Desert
{
    public class OverloadedSneezer : ModItem
    {
        int shotCount;
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 20;
            Item.damage = 38;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.reuseDelay = 45;
            Item.knockBack = 4f;
            Item.shootSpeed = 8;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Lime;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(
            source,
            position,
            velocity,
            type,
            damage,
            knockback,
            player.whoAmI
            );

            shotCount++;
            if (shotCount >= Item.useAnimation / Item.useTime) // end of burst
            {
                Sneeze(player, source, position, velocity, type, damage, knockback);
                shotCount = 0;
            }

            SoundEngine.PlaySound(SoundID.Item11, position);
            return false;
        }

        private void Sneeze(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 vel = velocity / 4 + Main.rand.NextVector2Circular(1f, 1f) + new Vector2(0 , -2);
                Vector2 unitVel = velocity;
                unitVel.Normalize();
                Vector2 spawn = position + unitVel * 56;
                Projectile.NewProjectile(source, spawn, vel, ModContent.ProjectileType<ElectricSneeze>(), damage, knockback, player.whoAmI);
            }
            SoundEngine.PlaySound(SoundID.Item94, position);
        }
    }
}
