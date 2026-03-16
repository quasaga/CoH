using CoH.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Beard)]
	public class MaskOfSalvation : ModItem
	{
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
			Item.vanity = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 75);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (!player.GetModPlayer<DrainingTearPlayer>().hasPainDome)
			{
				player.GetModPlayer<DrainingTearPlayer>().hasSalvationMask = true;
			}
		}
	}
}