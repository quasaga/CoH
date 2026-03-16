using CoH.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
	public class Hemoflower : ModItem
	{
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 75);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<HemoflowerPlayer>().hasHemoflower = true;
		}
	}
}