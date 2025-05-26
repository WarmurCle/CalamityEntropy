using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class Smoke : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Smoke").Value;
        public int timeleftmax = 200;
        public float scaleEnd = -1;
        public float scaleStart = -1;
        public float vc = 1;

        public override void AI()
        {
            if (scaleStart < 0)
            {
                scaleStart = scale;
            }
            if (scaleEnd > 0)
            {
                scale = float.Lerp(scaleStart, scaleEnd, 1 - (float)this.timeLeft / this.timeleftmax);
            }
            base.AI();
            this.velocity *= vc;
            this.alpha = (float)this.timeLeft / (float)timeleftmax;
        }
    }
}
