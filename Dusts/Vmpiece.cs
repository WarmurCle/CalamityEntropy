using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Dusts
{
    public class Vmpiece : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            var r = Main.rand;
            dust.scale *= 2.2f;
            dust.velocity = new Vector2((float)(r.Next(-60, 61)) / 26f, (float)(r.Next(-60, 61)) / 16f);
        
        }

        public override bool MidUpdate(Dust dust)
        {
            //dust.rotation += MathHelper.ToRadians(dust.velocity.X * 0.1f);
            dust.velocity.Y += 0.065f;
            dust.rotation = 0;
            return true;
        }
    }
}
