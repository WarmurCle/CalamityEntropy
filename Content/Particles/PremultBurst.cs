using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PremultBurst : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PremultBurst").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 2;
        }
        public override void AI()
        {
            base.AI();
            this.Lifetime = 2;
            this.Scale -= 0.1f;
            if (this.Scale < 0)
            {
                this.Lifetime = 0;
                this.Scale = 0;
            }
        }
    }
    public class ShockParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/wave2").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 2;
        }
        public override void AI()
        {
            base.AI();
            this.Lifetime = 2;
            this.Scale += 0.4f;
            this.Opacity -= 0.1f;
            if (this.Opacity < 0)
            {
                this.Lifetime = 0;
                this.Scale = 0;
            }
        }
    }
    public class ShockParticle2 : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/wave1").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 2;
        }
        public override void AI()
        {
            base.AI();
            this.Lifetime = 2;
            this.Scale += 0.4f;
            this.Opacity -= 0.1f;
            if (this.Opacity < 0)
            {
                this.Lifetime = 0;
                this.Scale = 0;
            }
        }
    }
}
