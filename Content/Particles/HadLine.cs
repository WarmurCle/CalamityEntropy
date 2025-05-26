using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class HadLine : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadLine").Value;
        public override void SetProperty()
        {
            this.Lifetime = 30;
        }
        public override Vector2 getOrigin()
        {
            return new Vector2(0, Texture.Height / 2);
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 30f;
        }
    }
}
