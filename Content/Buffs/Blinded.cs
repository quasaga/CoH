using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace CoH.Content.Buffs
{
	public class Blinded : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Confused);
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<BlindedNPC>().slowDebuff = true;
			float area = npc.width * npc.height;
			float scale = MathF.Pow(area, 0.2f) / 3f;

			if (Main.rand.NextBool(5)) // controls spawn rate
			{
				Dust dust = Dust.NewDustDirect(
					npc.position,
					npc.width,
					npc.height,
					DustID.Smoke,
					-npc.velocity.X * scale,
					-npc.velocity.Y * scale,
					0,
					Color.Tan,
					scale
				);
				
				dust.velocity *= 0.1f;
			}
		}
	}

		internal class BlindedNPC : GlobalNPC
	{
		public bool slowDebuff;

		public override bool InstancePerEntity => true;

		public override void ResetEffects(NPC npc)
		{
			slowDebuff = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (slowDebuff)
			{
				npc.velocity *= 0.94f;
			}
		}
	}
}