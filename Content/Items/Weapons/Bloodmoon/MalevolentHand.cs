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
using CoH.Content.Projectiles;
using CoH.Content.Projectiles.Magic;

namespace CoH.Content.Items.Weapons.Bloodmoon
{
    public class MalevolentHand : ModItem, ICustomDrawnWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 12;
            Item.useAnimation = 24;
            Item.reuseDelay = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.White;
            Item.channel = true;
            Item.noUseGraphic = true; // Important: Don't let vanilla draw it
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MalevolentProjectile>();
            Item.shootSpeed = 20;
            Item.mana = 10;
        }

        private Vector2? lastSpawnPosition = null;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitY);
            float targetDistance = 112f;
            Vector2 targetPosition = player.Center + direction * targetDistance;

            float spawnRadius = 112f;

            Vector2 spawnPosition;
            int tries = 0;

            do
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 spawnOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * spawnRadius;
                spawnPosition = targetPosition + spawnOffset;
                tries++;
                if (tries > 10) break; // Avoid infinite loop if conditions impossible
            }
            while (lastSpawnPosition.HasValue && Vector2.Distance(spawnPosition, lastSpawnPosition.Value) < 84f);

            lastSpawnPosition = spawnPosition;

            Vector2 projVelocity = (targetPosition - spawnPosition).SafeNormalize(Vector2.UnitY) * Item.shootSpeed;

            SoundEngine.PlaySound(SoundID.Item8, player.position);
            Projectile.NewProjectile(source, spawnPosition, projVelocity, type, damage, knockback, player.whoAmI);

            return false;
        }


        public bool ShouldRotate => true;
        public Vector2 GetGripPivot(Texture2D texture)
        {
            return new Vector2(texture.Width * 0.2f, texture.Height * 0.66f);
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (player.channel || player.itemAnimation > 0)
            {
                Vector2 toCursor = Main.MouseWorld - player.MountedCenter;
                player.direction = (toCursor.X >= 0) ? 1 : -1;

                float rotation = (float)Math.Atan2(toCursor.Y * player.gravDir, toCursor.X);
                if (player.direction == -1)
                {
                    rotation += MathHelper.Pi;
                }

                rotation = MathHelper.WrapAngle(rotation);
                player.itemRotation = rotation;
            }
        }
    }
}
