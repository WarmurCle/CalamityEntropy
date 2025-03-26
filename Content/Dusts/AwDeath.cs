using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Dusts
{
    public class AwDeath : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity = new Vector2((float)(Main.rand.Next(-60, 61)) / 26f, -10);
            dust.scale = 2f;
            dust.customData = 0;
            dust.noGravity = true;
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = 0;
            dust.customData = (int)dust.customData + 1;
            dust.velocity.X += (float)Math.Cos((float)(int)dust.customData * 0.07f) * 0.1f + 0.026f;
            return true;
        }
    }
}
