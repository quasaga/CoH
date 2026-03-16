using CoH.Common.Players;
using Terraria;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;

namespace CoH.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
	public class ThePainDome : ModItem
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
            var modPlayer = player.GetModPlayer<DrainingTearPlayer>();
            modPlayer.hasPainDome = true;

            player.statManaMax2 -= player.statManaMax2 / 4;
        }
	}    
}