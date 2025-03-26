using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ELineParticle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LifeLeaf").Value;

        public override void update()
        {
            base.update();
        }
        public Vector2 b;
        public override void onSpawn()
        {
            b = this.position;
        }
        public float width = 2;
        public float c;
        public float r;
        public ELineParticle(float width, float c = 0.96f, float r = 0.96f)
        {
            this.width = width;
            this.c = c;
            this.r = r;
        }
        public int counter = 0;
        public override void draw()
        {
            counter++;
            Util.Util.drawLine(this.position, b, this.color, width);
            this.velocity *= r;
            b = Vector2.Lerp(this.position, b, this.c);
            if (Util.Util.getDistance(this.position, b) < 2 && counter > 20)
            {
                this.timeLeft = 0;
            }
            else
            {
                this.timeLeft = 2;
            }
        }
    }
}
