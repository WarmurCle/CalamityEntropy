using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ULineParticle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LifeLeaf").Value;

        public override void update()
        {
            base.update();
            counter++;
            this.velocity *= r;
            b = Vector2.Lerp(this.position, b, this.c);
            this.alpha -= this.alphaD;
            if (Util.Util.getDistance(this.position, b) < 4 && counter > 3 || this.alpha < 0)
            {
                this.timeLeft = 0;
            }
            else
            {
                this.timeLeft = 4;
            }
        }
        public Vector2 b;
        public override void onSpawn()
        {
            b = this.position;
        }
        public float width = 2;
        public float c;
        public float r;
        public float alphaD;
        public ULineParticle(float width, float c = 0.96f, float r = 0.96f, float alphaDec = 0.05f)
        {
            this.width = width;
            this.c = c;
            this.r = r;
            this.alphaD = alphaDec;
        }
        public int counter = 0;
        public override void draw()
        {
            Util.Util.drawLine(this.position, b, this.color * this.alpha, width * this.alpha);
        }
    }
}
