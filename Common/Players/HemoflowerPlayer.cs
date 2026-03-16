using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using CoH.Content.Projectiles.Magic;

namespace CoH.Common.Players
{
    public class HemoflowerPlayer : ModPlayer
    {
        public bool hasHemoflower = false;
        public bool hasRuddyCloak = false;
        public int ruddyCloakHealthLoss = 0;
        public int bloodRainTimer = 0;
        public bool shouldBloodRain = false;
        int rainInterval = 6;

        public override void ResetEffects()
        {
            hasHemoflower = false;
            hasRuddyCloak = false;
        }

        public override void OnRespawn()
        {
            ruddyCloakHealthLoss = 0;
            bloodRainTimer = 0;
            shouldBloodRain = false;
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (hasHemoflower || hasRuddyCloak)
            {
                mult *= 0.5f;
            }
        }

        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            if (!hasHemoflower && !hasRuddyCloak) return;

            var drainingTearPlayer = Player.GetModPlayer<DrainingTearPlayer>();

            float healthCastMultiplier = GetHealthCastMultiplier();

            int lifeToDrainCalc = (int)(manaConsumed * healthCastMultiplier);

            int lifeToDrain = Math.Max(lifeToDrainCalc, 1);

            if (Player.statLife > lifeToDrain && !drainingTearPlayer.isHealthCasting)
            {
                Player.statLife -= lifeToDrain;
                if (ModContent.GetInstance<CoHConfig>().ShowHealthDrainText)
                {
                    CombatText.NewText(Player.Hitbox, Color.Red, lifeToDrain);
                }
            }

            if (!hasRuddyCloak || item.DamageType != DamageClass.Magic) return;
            if (drainingTearPlayer.isHealthCasting)
            {
                ruddyCloakHealthLoss += drainingTearPlayer.healthCost;
            }
            ruddyCloakHealthLoss += lifeToDrain;
            if (ruddyCloakHealthLoss >= 100)
            {
                ruddyCloakHealthLoss = 0;

                if (Player.whoAmI == Main.myPlayer)
                {
                    shouldBloodRain = true;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!shouldBloodRain) return;

            bloodRainTimer++;
            if (bloodRainTimer % rainInterval == 0)
            {
                SpawnBloodRain(ModContent.ProjectileType<BloodRainProjectile>(), 2f, 17f);
            }

            if (bloodRainTimer >= rainInterval * 7)
            {
                shouldBloodRain = false;
                bloodRainTimer = 0;
            }
        }

        public float GetHealthCastMultiplier()
        {
            if (hasHemoflower) return 0.5f;
            if (hasRuddyCloak) return 0.67f;
            return 0f;
        }

        public void SpawnBloodRain(int type, float knockback, float shootSpeed)
        {
            int numProjectiles = Main.rand.Next(2, 3);
            for (int i = 0; i < numProjectiles; i++)
            {
                float offsetX = Main.rand.NextFloat(-175f, 175f);
                float offsetY = Player.Center.Y - 600f;
                Vector2 spawnPos = new Vector2(Main.MouseWorld.X + offsetX, offsetY);

                Vector2 dirToCursor = Vector2.Normalize(Main.MouseWorld - spawnPos);

                // i did this a long time ago and it works so im sorry this  is horrible
                float maxLerp = 1f;
                float lerpFactor = Math.Min(Math.Abs(offsetX) / 175f, maxLerp);
                Vector2 finalDir = Vector2.Lerp(Vector2.UnitY, dirToCursor, lerpFactor).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-2f, 2f))).SafeNormalize(Vector2.UnitY);

                shootSpeed *= 1f + Main.rand.NextFloat(-0.1f, 0.1f);
                Vector2 velocity = finalDir * shootSpeed;

                int baseDamage = 95;
                int damage = (int)(baseDamage * Player.GetDamage(DamageClass.Magic).Multiplicative);

                Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPos, velocity, type, damage, knockback, Player.whoAmI);
            }
        }
    }
}