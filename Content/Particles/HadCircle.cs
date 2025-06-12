using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class HadCircle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadCircle").Value;
        public override void SetProperty()
        {
            this.Lifetime = 16;
        }
        public float CScale = 1;
        public override void AI()
        {
            if (this.Lifetime > 8)
            {
                this.Lifetime--;
            }
            base.AI();
            this.Opacity = (float)Math.Sqrt(1 - (Math.Abs(8f - this.Lifetime) / 8f));
            this.Scale = (((float)Math.Sqrt(1 - (Math.Abs(8f - this.Lifetime) / 8f)))) * 0.94f * CScale;
        }
    }
    public class HadCircle2 : EParticle
    {
        public float CScale = 1;
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/BloomRing").Value;
        public override void SetProperty()
        {
            this.Lifetime = 16;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = (this.Lifetime / 16f);
            this.Scale = (16 - this.Lifetime) / 16f * 2.4f * CScale;
        }
        public float scale2 = 1;
        

        public override void PreDraw()
        {
            this.Scale *= this.scale2;
            base.PreDraw();
            base.PreDraw();
        }
    }
}
