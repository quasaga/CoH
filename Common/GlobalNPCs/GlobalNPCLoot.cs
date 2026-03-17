using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;

namespace CoH.Content.NPCs
{
    public class DropGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcloot)
        {
            if (npc.type == NPCID.EyeballFlyingFish) // Wandering Eye Fish
            {
                int itemType = ModContent.ItemType<Items.Accessories.DrainingTear>();
                npcloot.Add(ItemDropRule.Common(itemType, 7));
            }
            if (npc.type == NPCID.ZombieMerman)
            {
                int itemType = ModContent.ItemType<Items.Weapons.Bloodmoon.MalevolentHand>();
                npcloot.Add(ItemDropRule.Common(itemType, 7));
            }
        }
    }
}
