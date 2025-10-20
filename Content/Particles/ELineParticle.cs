using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ELineParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LifeLeaf").Value;

        public override void AI()
        {
            base.AI();

            counter++;

            this.Velocity *= r;
            b = Vector2.Lerp(this.Position, b, this.c);
            if (CEUtils.getDistance(this.Position, b) < 2 && counter > 20)
            {
                this.Lifetime = 0;
            }
            else
            {
                this.Lifetime = 2;
            }
        }
        public Vector2 b;
        public override void OnSpawn()
        {
            b = this.Position;
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
        public override void Draw()
        {
            CEUtils.drawLine(this.Position, b, this.Color, width);
        }
    }
}
