using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class Sn : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Sn").Value;
        public override void SetProperty()
        {
            this.Lifetime = 16;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 16f;
        }
    }
}
