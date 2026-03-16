using CoH.Common.Players;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CoH.Common.GlobalItems
{
	public class DrainingTearGlobalItem : GlobalItem
    {
        public override void OnConsumeMana(Item item, Player player, int manaConsumed)
        {
            player.GetModPlayer<DrainingTearPlayer>().HandleHealthCasting(item, manaConsumed);
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            var drainingTearPlayer = player.GetModPlayer<DrainingTearPlayer>();

            if (item.DamageType == DamageClass.Magic)
            {
                if (drainingTearPlayer.HasHealthCastingAccessory && 
                    (drainingTearPlayer.IsOutOfMana(item.mana) || drainingTearPlayer.isHealthCasting))
                {
                    // i hope this i good cuz i did this a long time ago and cannot be bothered to improve this
                    // Calculate true mana cost before zeroing it out
                    float reduced = item.mana * (1f - reduce);
                    float modifiedCost = reduced * mult;
                    int manaReduction = (int)Math.Ceiling(modifiedCost);

                    drainingTearPlayer.lastManaCost = item.mana - manaReduction;

                    // Zero out mana cost
                    reduce = 1f;
                    mult = 0f;
                }
                else
                {
                    drainingTearPlayer.lastManaCost = 0;
                }
            }
        }
    }
}