using CoH.Content.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Buffs
{
	public class Bludgeoning : ModBuff
	{
		public override void SetStaticDefaults() {
			// NPCs will automatically be immune to this buff if they are immune to BoneJavelin. SkeletronHead and SkeletronPrime are immune to BoneJavelin.
			BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<BludgeoningGlobalNPC>().Bludgeoning = true;
		}
	}

	internal class BludgeoningGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public bool Bludgeoning;

		public override void ResetEffects(NPC npc) {
			Bludgeoning = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (Bludgeoning) {
				if (npc.lifeRegen > 0) {
					npc.lifeRegen = 0;
				}
				
				int BloodDartCount = 0;
				foreach (var p in Main.ActiveProjectiles) {
					if (p.type == ModContent.ProjectileType<BloodDartProjectile>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
						BloodDartCount++;
					}
				}
				
				npc.lifeRegen -= BloodDartCount * 2 * 3;
				if (damage < BloodDartCount * 3) {
					damage = BloodDartCount * 3;
				}
			}
		}
	}
}
