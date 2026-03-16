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
using CoH.Content.Projectiles.Melee;

namespace CoH.Content.Items.Weapons.Desert
{
    public class TheLifeline : ModItem
    {
        public override void SetStaticDefaults() {
			ItemID.Sets.Yoyo[Type] = true;
			ItemID.Sets.GamepadExtraRange[Type] = 15;
			ItemID.Sets.GamepadSmartQuickReach[Type] = true;
		}
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 34;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 3f;
            Item.crit = 8;
            Item.shootSpeed = 16;

            Item.shoot = ModContent.ProjectileType<TheLifelineProjectile>();
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Lime;
        }
    }
}