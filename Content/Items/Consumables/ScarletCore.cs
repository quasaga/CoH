using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace CoH.Content.Items.Consumables
{
    public class ScarletCore : ModItem
    {
		bool effectApplied = false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults() {
			Item.width = 9;
			Item.height = 22;
			Item.maxStack = 1;
			Item.value = 100;
			Item.rare = ItemRarityID.Green;
			Item.useAnimation = 25;
			Item.useTime = 25;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.holdStyle = ItemHoldStyleID.HoldHeavy;
			Item.consumable = false;
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			// Check if we're on the last tick of the use animation
			if (player.itemTime == 1 && !effectApplied)
			{
				if (!Main.bloodMoon) return;

				int damageAmount = player.statLifeMax / 2;
				
				if (player.statLife <= damageAmount)
				{
					damageAmount = player.statLife - 1;
				}

				player.statLife -= damageAmount;
				Main.bloodMoon = false;
				Main.NewText("Your sacrifice makes the blood moon pass", new Color(0, 255, 125));

				effectApplied = true;
			}
		}

		public override bool? UseItem(Player player)
		{
			// Reset flag so the effect can be applied again next time
			effectApplied = false;
			return true;
		}

		public override void UseItemFrame(Player player)
		{
			player.itemRotation = MathHelper.ToRadians(90f); // Now rotates 90° properly
		}

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
		}
    }
}