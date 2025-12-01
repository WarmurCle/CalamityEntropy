using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class HadLine : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadLine").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 30;
        }
        public float hm = 1;
        public override Vector2 getOrigin()
        {
            return new Vector2(0, Texture.Height / 2);
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 30f;
        }
        public override void Draw()
        {
            Color clr = this.Color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.Position.X / 16)), ((int)(this.Position.Y / 16)), clr);
            }
            if (!this.useAdditive && !this.useAlphaBlend)
            {
                clr.A = (byte)(clr.A * Opacity);
            }
            else
            {
                clr *= Opacity;
            }
            Main.spriteBatch.Draw(this.Texture, this.Position - Main.screenPosition, null, clr, Rotation, getOrigin(), Scale * new Vector2(1, hm), SpriteEffects.None, 0);
        }
    }
}
