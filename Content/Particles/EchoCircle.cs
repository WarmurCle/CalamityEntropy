using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EchoCircle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadCircle").Value;
        public override void SetProperty()
        {
            this.timeLeft = 6;
        }
        public override void AI()
        {
            base.AI();
            this.alpha = timeLeft / 6f;
            this.scale = timeLeft / 6f * 0.22f;
            this.velocity *= 0.96f;

        }
    }
}
