using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Buffs
{
	public class HighVoltage : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<HighVoltageNPC>().lifeRegenDebuff = true;

			if (Main.rand.NextBool(3)) // controls spawn rate
			{
				Dust dust = Dust.NewDustDirect(
					npc.position,
					npc.width,
					npc.height,
					DustID.Electric
				);
				
				dust.velocity *= 0.5f;
			}
		}
	}

		internal class HighVoltageNPC : GlobalNPC
	{
		public bool lifeRegenDebuff;

		public override bool InstancePerEntity => true;

		public override void ResetEffects(NPC npc)
		{
			lifeRegenDebuff = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (lifeRegenDebuff)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				if (damage < 3)
					damage = 3;

				npc.lifeRegen -= 30;
			}
		}
	}
}