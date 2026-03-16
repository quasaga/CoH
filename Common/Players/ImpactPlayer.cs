using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;

namespace CoH.Common.Players
{
    public class ImpactPlayer : ModPlayer
    {
        public int shakeCooldown;
        public Vector2 shakeOffset;
        public float shakeStrength;
        public int shakeInterval;
        public int shakeTimer;
        public float recoilRotation;

        public override void ModifyScreenPosition()
        {
            Main.screenPosition += shakeOffset;
        }

        public void StartShake(int duration, float strength, int interval = 1)
        {
            shakeTimer = duration;
            shakeStrength = strength;
            shakeInterval = interval;
            shakeCooldown = 0;
        }

        public override void PostUpdate()
        {
            if (shakeTimer > 0)
            {
                shakeTimer--;
                if (shakeCooldown <= 0)
                {
                    shakeCooldown = shakeInterval;

                    shakeOffset = new Vector2(
                        Main.rand.NextFloat(-shakeStrength, shakeStrength),
                        Main.rand.NextFloat(-shakeStrength, shakeStrength)
                    );
                }
                else
                {
                    shakeCooldown--;
                }
            }
            else
            {
                shakeOffset = Vector2.Zero;
            }
        }

        public override void PreUpdate()
        {
            recoilRotation = MathHelper.Lerp(recoilRotation, 0f, 0.2f); // smooth recoil return
        }
    }
}
