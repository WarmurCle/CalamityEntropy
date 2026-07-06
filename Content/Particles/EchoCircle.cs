using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EchoCircle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadCircle").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 6;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = Lifetime / 6f;
            this.Scale = Lifetime / 6f * 0.22f;
            this.Velocity *= 0.96f;
        }
    }
}
