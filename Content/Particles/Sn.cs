using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class Sn : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Sn").Value;
        public override void SetProperty()
        {
            this.timeLeft = 16;
        }
        public override void AI()
        {
            base.AI();
            this.alpha = this.timeLeft / 16f;
        }
    }
}
