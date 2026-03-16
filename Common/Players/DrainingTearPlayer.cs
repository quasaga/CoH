using Terraria;
using Terraria.ModLoader;
using System;
using System.Drawing;
using SteelSeries.GameSense;

namespace CoH.Common.Players
{
    public class DrainingTearPlayer : ModPlayer
    {
        public bool hasDrainingTear = false;
        public bool hasSalvationMask = false;
        public bool hasPainDome = false;
        public bool isHealthCasting = false;
        public int lastManaCost = 0;
        public int healthCost = 0;
        public bool HasHealthCastingAccessory => hasDrainingTear || hasPainDome;

        public override void ResetEffects() {
			hasDrainingTear = false;
            hasSalvationMask = false;
            hasPainDome = false;
		}

        public override bool CanUseItem(Item item)
        {
            if (HasHealthCastingAccessory)
            {
                if (Player.statLife > lastManaCost)
                {
                    return true;
                }
                else
                {
                    return false; // Not enough health to pay the healthCast.lastManaCost
                }
            }
            return base.CanUseItem(item);
        }

        public override void PostUpdate()
        {
            if (Player.HeldItem.DamageType != DamageClass.Magic || Player.itemAnimation == 0)
            {
                isHealthCasting = false;
                return;
            }

            isHealthCasting = (HasHealthCastingAccessory) && IsOutOfMana(Player.HeldItem.mana);
        }

        public bool IsOutOfMana(int manaCost)
        {
            if (manaCost <= 0)
                return false;

            return Player.statMana < manaCost;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (!isHealthCasting) return;

            if (hasDrainingTear)
            {
                damage *= 1.25f;
            }
            else if (hasPainDome)
            {
                damage *= 1.33f;
            }
        }

        public void HandleHealthCasting(Item item, int manaConsumed)
        {
            if (!isHealthCasting || lastManaCost <= 0)
                return;

            // refund mana because we zeroed the cost earlier
            Player.statMana += manaConsumed;

            healthCost = lastManaCost;

            if (Player.statLife > healthCost)
            {
                Player.statLife -= healthCost;

                if (ModContent.GetInstance<CoHConfig>().ShowHealthDrainText)
                {
                    CombatText.NewText(Player.Hitbox, Microsoft.Xna.Framework.Color.Red, healthCost);
                }
            }
        }

        public override void UpdateLifeRegen()
        {
            if (!hasSalvationMask && !hasPainDome) return;

            float h = (float)Player.statLife / Player.statLifeMax2;
            float regenBonus = 8f / (1f + (float)Math.Pow(h / 0.4f, 4f));
            Player.lifeRegen += (int)regenBonus;
        }
    }
}