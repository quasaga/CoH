using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.NPCs.Bloodmoon
{
	public class BloodSpiritA : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 32;
			NPC.damage = 50;
			NPC.defense = -1; 
			NPC.lifeMax = 125;
			NPC.aiStyle = NPCAIStyleID.DungeonSpirit;
			NPC.knockBackResist = 0.33f;
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