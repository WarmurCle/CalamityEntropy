using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PremultBurst : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PremultBurst").Value;
        public override void SetProperty()
        {
            this.Lifetime = 2;
        }
        public override void AI()
        {
            base.AI();
            this.Lifetime = 2;
            this.Scale -= 0.1f;
            if(this.Scale < 0)
            {
                this.Lifetime = 0;
                this.Scale = 0;
            }
        }
    }
}
