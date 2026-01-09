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
        public bool colorTrans = false;
        private bool setColor = true;
        public Color endColor = Color.White;
        private Color startColor = Color.White;
        public override void AI()
        {
            if(setColor)
            {
                setColor = false;
                startColor = Color;
            }
            if(colorTrans)
            {
                Color = Color.Lerp(startColor, endColor, 1 - ((float)Lifetime / timeleftmax));
            }
            if (scaleStart < 0)
            {
                scaleStart = Scale;
            }
            if (scaleEnd > 0)
            {
                Scale = float.Lerp(scaleStart, scaleEnd, 1 - (float)this.Lifetime / this.timeleftmax);
            }
            base.AI();
            this.Velocity *= vc;
            this.Opacity = (float)this.Lifetime / (float)timeleftmax;
        }
    }
}
