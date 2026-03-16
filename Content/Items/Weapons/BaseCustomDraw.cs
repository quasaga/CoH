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

namespace CoH.Content.Items.Weapons
{
    public class BaseCustomDraw : ModItem, ICustomDrawnWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item36;
            Item.channel = true;
            Item.noUseGraphic = true; // Important: don't let vanilla draw it
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ProjectileID.Ale;
            Item.shootSpeed = 15;
        }

        public bool ShouldRotate => true;
        public Vector2 GetGripPivot(Texture2D texture) // default is the texture center
        {
            return new Vector2(0, texture.Height / 2f);
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var impactPlayer = player.GetModPlayer<ImpactPlayer>();

            impactPlayer.recoilRotation -= 0.1f;
            impactPlayer.StartShake(0, 0f);

            return true;
        }
    }
}
