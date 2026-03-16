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
            if (npc.type == NPCID.WanderingEye) // Wandering Eye Fish
            {
                int itemType = ModContent.ItemType<Items.Accessories.DrainingTear>();
                npcloot.Add(ItemDropRule.Common(itemType, 8)); // 1 in 8 drop chance
            }
            if (npc.type == NPCID.ZombieMerman) // Zombie Merman
            {
                int itemType = ModContent.ItemType<Items.Weapons.Bloodmoon.MalevolentHand>();
                npcloot.Add(ItemDropRule.Common(itemType, 8)); // 1 in 8 drop chance
            }
        }
    }
}
