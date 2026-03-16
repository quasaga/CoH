using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.NPCs.Bloodmoon
{
	public class BloodSpirit : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 48;
			NPC.damage = 15;
			NPC.defense = 20;
			NPC.lifeMax = 250;
			NPC.aiStyle = NPCAIStyleID.DungeonSpirit;
			NPC.knockBackResist = 0.15f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = false;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;

			AIType = NPCID.DungeonSpirit;
		}
		public override void OnKill()
        {
            base.OnKill();

			int bloodSpiritA = ModContent.NPCType<BloodSpiritA>();
			for (int i=-2; i <= 2; i += 4)
			{
				Vector2 spawnPos = new Vector2(NPC.Center.X, NPC.Center.Y + i * 6);
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, bloodSpiritA);
			}

			if (Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 9; i++)
				{
					Dust dust = Dust.NewDustDirect(
						NPC.position,
						NPC.width,
						NPC.height,
						DustID.Blood
					);

					dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
					dust.noGravity = false;
					dust.scale = 2f;
				}
			}
        }
	}
}