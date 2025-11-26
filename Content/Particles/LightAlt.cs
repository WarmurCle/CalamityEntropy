using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class LightAlt : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PRT_Light2").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 20;
        }
        public Vector2 ScaleAdd = Vector2.Zero;
        public Vector2 NowScale = Vector2.One;
        public override void AI()
        {
            base.AI();
            NowScale += ScaleAdd;
        }

        public override void Draw()
        {
            Main.spriteBatch.Draw(this.Texture, Position - Main.screenPosition, null, Color * Opacity * ((float)Lifetime / TimeLeftMax), Rotation, getOrigin(), NowScale * Scale, SpriteEffects.None, 0);
        }
    }
}
