using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CoH.Content.Projectiles.Magic;
using CoH.Content.Projectiles.Ranged;

namespace CoH.Content.Items.Weapons.Bloodmoon
{
    public class BloodyDischarge : ModItem
    {
        int chargeTicksCounter = 0;
        int chargeTicks = 3;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.damage = 666;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 35;
            Item.knockBack = 7f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 1f;

            Item.mana = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var discharge = player.GetModPlayer<BloodyDischargePlayer>();
            if (player.channel)
            {
                chargeTicksCounter++;
                if (chargeTicksCounter <= chargeTicks)
                {
                    //spawn effect so the player knows something is happening
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<BloodClotExplosion>(), 0, 0, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, position);
                }
                else
                {
                    discharge.currentCharge++;
                    FireChargedShot(player, source, position, damage, knockback);
                    if (discharge.currentCharge > discharge.maxCharge)
                    {
                        Item.mana = 0;
                    }
                }
            }

            return false;
        }

        private void FireChargedShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, int damage, float knockback)
        {
            var discharge = player.GetModPlayer<BloodyDischargePlayer>();
            if (discharge.currentCharge > discharge.maxCharge) return;
            float angle = MathHelper.TwoPi * discharge.currentCharge / discharge.maxCharge; //shoot out in a circle
            Vector2 offset = angle.ToRotationVector2() * 64f;
            Vector2 spawnPos = player.MountedCenter + offset;
            Projectile.NewProjectile(source, spawnPos, Vector2.Zero, ModContent.ProjectileType<PeriodSpear>(), damage, knockback, player.whoAmI);

            SoundEngine.PlaySound(SoundID.Item8, position);
        }

        public override void HoldItem(Player player)
        {
            var discharge = player.GetModPlayer<BloodyDischargePlayer>();
            if (!player.channel)
            {
                chargeTicksCounter = 0;
                discharge.currentCharge = 0;
                Item.mana = 24;
            }
            
            base.HoldItem(player);
        }
    }

    public class BloodyDischargePlayer : ModPlayer
    {
        public int currentCharge = 0;
        public int maxCharge = 10;
    }
}
