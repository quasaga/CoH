using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CoH.Common.Players;
using CoH.Common.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using CoH.Content.Projectiles.Ranged;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Terraria.Localization;
using CoH.Content.Items.Weapons.Bloodmoon;
using CoH.Content.Projectiles.Magic;

namespace CoH.Content.Items.Weapons.Desert
{
    public class BlindingBurst : ModItem
    {
        int burstSize = 5;
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 35;
            Item.knockBack = 6.5f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10;

            Item.mana = 18;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < burstSize; i++)
            {
                Vector2 vel = velocity + Main.rand.NextVector2Circular(1f, 1f);
                int proj = Projectile.NewProjectile(
                source,
                position,
                vel,
                ModContent.ProjectileType<SandProjectile>(),
                damage,
                knockback,
                player.whoAmI
                );
            }

            return false;
        }
    }
}
