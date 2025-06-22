using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class BlackKnifeSlash : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/AbyssalLine").Value;

        public float width = 1;
        public override void AI()
        {
            base.AI();
            width = CEUtils.Parabola(Lifetime / (float)TimeLeftMax, 7);
        }
        public override void PreDraw()
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, this.Rotation, tex.Size() / 2, new Vector2(Scale * 660 / tex.Width, width / tex.Height), SpriteEffects.None, 0);

        }
    }
}
