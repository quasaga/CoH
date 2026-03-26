using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CoH.Common.Players;
using CoH.Common.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using CoH.Content.Projectiles.Ranged;
using CoH.Content.Projectiles.Ranged.Bloodmoon;

namespace CoH.Content.Items.Weapons.Ranged.Bloodmoon
{
    public class CloggedBarrel : ModItem, ICustomDrawnWeapon
    {
        int chargeTicksCounter = 0;
        int chargeTicks = 165;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 32;
            Item.damage = 777;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 165;
            Item.knockBack = 7f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4;

            Item.useAmmo = AmmoID.Bullet;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }

        public bool ShouldRotate => true;
        public Vector2 GetGripPivot(Texture2D texture) // default is the texture center
        {
            return new Vector2(texture.Width / 4f, texture.Height / 2f);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return chargeTicksCounter > chargeTicks % Item.useTime && chargeTicksCounter <= (chargeTicks - Item.useTime) / Item.useTime;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var impactPlayer = player.GetModPlayer<ImpactPlayer>();
            if (player.channel)
            {
                chargeTicksCounter++;
                if (chargeTicksCounter >= chargeTicks % Item.useTime && chargeTicksCounter <= (chargeTicks - Item.useTime * 2) / Item.useTime)
                {
                    SoundEngine.PlaySound(SoundID.Item149, player.position);
                    impactPlayer.recoilRotation += 0.15f;
                }
                if (chargeTicksCounter >= chargeTicks / Item.useTime)
                {
                    chargeTicksCounter = 0;
                    FireChargedShot(player, source, position, velocity, type, damage, knockback);
                }
            }

            return false;
        }

        private void FireChargedShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(
                source,
                position,
                velocity,
                ModContent.ProjectileType<BloodClot>(),
                damage,
                knockback,
                player.whoAmI
            );

            Projectile p = Main.projectile[proj];

            p.ai[0] = type;
            p.ai[1] = damage;

            var impactPlayer = player.GetModPlayer<ImpactPlayer>();
            impactPlayer.recoilRotation -= 1.25f;
            impactPlayer.StartShake(10, 7f, 1);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, position);
        }

        public override void HoldItem(Player player)
        {
            Item.useTime = Item.useAnimation = 15;
            if (!player.channel)
            {
                chargeTicksCounter = 0;
            }
        }
    }
}
