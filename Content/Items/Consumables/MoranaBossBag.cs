using CoH.Content.Items.Weapons.Bloodmoon;
using CoH.Content.NPCs.Bloodmoon.Morana;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Items.Consumables
{
	// Basic code for a boss treasure bag
	public class MoranaBossBag : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.BossBag[Type] = true;

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// We have to replicate the expert drops from MinionBossBody here

			//itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MinionBossMask>(), 7));

			int[] weapons = new int[]
			{
				ModContent.ItemType<SicklyHarvest>(),
				ModContent.ItemType<BloodInitiation>(),
				ModContent.ItemType<Circulation>(),
				ModContent.ItemType<CloggedBarrel>(),
				ModContent.ItemType<BloodyDischarge>(),
			};

			itemLoot.Add(ItemDropRule.OneFromOptions(1, weapons));
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Morana>()));
		}
	}
}
