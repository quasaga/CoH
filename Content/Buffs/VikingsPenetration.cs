using System;
using CoH.Content.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Buffs
{
	public class VikingsPenetration : ModBuff
	{
		public override void SetStaticDefaults() {
			// NPCs will automatically be immune to this buff if they are immune to BoneJavelin. SkeletronHead and SkeletronPrime are immune to BoneJavelin.
			BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<VikingsPenetrationGlobalNPC>().Penetrated = true;
		}
	}

	internal class VikingsPenetrationGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public bool Penetrated;

		public override void ResetEffects(NPC npc) {
			Penetrated = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (Penetrated) {
				if (npc.lifeRegen > 0) {
					npc.lifeRegen = 0;
				}
				
				if (damage < 10)
					damage = 10;

				npc.lifeRegen -= 20 * 2;
			}
		}
	}
}
