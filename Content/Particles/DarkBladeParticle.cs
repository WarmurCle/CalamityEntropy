using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class DarkBladeParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/DarkBladeParticle").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 24;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 30f * 0.8f;
        }
    }
}
