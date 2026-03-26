using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using CoH.Content.Items.Weapons.Magic.Bloodmoon;
using CoH.Content.Items.Accessories;

namespace CoH.Content.NPCs
{
    public class DropGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcloot)
        {
            if (npc.type == NPCID.EyeballFlyingFish) // Wandering Eye Fish
            {
                int itemType = ModContent.ItemType<DrainingTear>();
                npcloot.Add(ItemDropRule.Common(itemType, 7));
            }
            if (npc.type == NPCID.ZombieMerman)
            {
                int itemType = ModContent.ItemType<MalevolentHand>();
                npcloot.Add(ItemDropRule.Common(itemType, 7));
            }
        }
    }
}
