using Terraria;
using Terraria.ID;

namespace CoH.Common.Systems
{
    public static class BloodCultistAttackManager
    {
        public static int[] attackers = new int[2]; // whoAmI of attacking NPCs
        public static int[] assignedAttack = new int[2]; // 0=SpawnSpirit, 1=MalevolentProj, 2=CoagShot
        public static bool attackTickReady = false;
        public static int attackCooldown = 3 * 60;
        public static int attackCooldownCounter = 0;

        public static void UpdateTick()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                attackCooldownCounter++;
                if (attackCooldownCounter >= attackCooldown)
                {
                    attackCooldownCounter = 0;
                    attackTickReady = true;
                }
                else if (attackTickReady == true)
                {
                    attackTickReady = false;
                }
            }
        }
    }
}
