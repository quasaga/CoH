using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using CoH.Common.Players;
using System;
using CoH.Common.Interfaces;
using Terraria.Audio;
using CoH.Content.Projectiles.Melee;
using CoH.Content.Projectiles.Ranged;

namespace CoH.Content.Items.Weapons.Bloodmoon
{
    public class SicklyHarvest : ModItem
    {
        bool sickleSpawned = false;
        int chargeTimer = 0;
        bool soundPlayed = false;
        bool isDashing = false;
        int dashTimer = 0;
        int dashDuration = 35;
        Vector2 dir;
        float dashSpeed = 14f;
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 74;
            Item.damage = 500;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = 90;
            Item.useAnimation = 35;
            Item.knockBack = 3f;
            Item.crit = 8;
            Item.shootSpeed = 1;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Lime;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (player.channel)
            {
                chargeTimer++;
                player.itemAnimation = 1;
                player.itemAnimationMax = 1;
                player.itemTime = 1;

                Vector2 toCursor = Main.MouseWorld - player.MountedCenter;
                player.direction = toCursor.X > 0 ? 1 : -1;

                float rotation = (float)Math.Atan2(toCursor.Y * player.gravDir, toCursor.X);
                if (player.direction == -1)
                    rotation += MathHelper.Pi;
                player.itemRotation = MathHelper.WrapAngle(rotation);

                if (chargeTimer >= Item.useTime && !soundPlayed)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<BloodClotExplosion>(), 0, 0, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.DD2_DrakinShot, player.Center);
                    soundPlayed = true;
                }
            }
            else
            {
                if (chargeTimer >= Item.useTime)
                {
                    dir = Main.MouseWorld - player.MountedCenter;
                    dir.Normalize();

                    isDashing = true;
                }
                chargeTimer = 0;
            }
            if (isDashing)
            {
                dashTimer++;
                if (dashTimer <= dashDuration)
                {
                    if (!sickleSpawned)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SicklyHarvestProjectile>(), Item.damage, 10f);
                        sickleSpawned = true;
                    }

                    player.velocity = dir * dashSpeed;
                    if (player.velocity.Y > 0)
                    {
                        int playerFeetX = (int)(player.position.X + player.width / 2) / 16;
                        int playerFeetY = (int)(player.position.Y + player.height + 1) / 16;

                        Tile tileBelow = Framing.GetTileSafely(playerFeetX, playerFeetY);

                        if (!tileBelow.HasTile || Main.tileSolidTop[tileBelow.TileType])
                        {
                            player.position.Y += 0.001f;
                        }
                    }

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.CanBeChasedBy() && player.Hitbox.Intersects(npc.Hitbox))
                        {
                            player.immune = true;
                            player.immuneTime = 6;
                            if (npc.immune[player.whoAmI] <= 0)
                            {
                                player.ApplyDamageToNPC(npc, Item.damage, 0f, player.direction);
                                npc.immune[player.whoAmI] = 3;
                            }
                        }
                    }
                }
                else
                {
                    sickleSpawned = false;
                    isDashing = false;
                    soundPlayed = false;
                    dashTimer = 0;
                }
            }
        }
    }
}