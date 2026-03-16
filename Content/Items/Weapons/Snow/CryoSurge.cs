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

namespace CoH.Content.Items.Weapons.Snow
{
    public class CryoSurge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 23;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 0.1f;
            Item.shoot = ModContent.ProjectileType<CryoRang>();
            Item.shootSpeed = 16; //pretty much changes how far the projectile spawns so leave at 1

            Item.mana = 5;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
