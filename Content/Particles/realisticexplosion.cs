using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class RealisticExplosion : EParticle
    {
        public override void OnSpawn()
        {
            this.Lifetime = 20;
        }
        public int frame = -1;
        public override Texture2D Texture => CEUtils.pixelTex;
        public Vector2 size = Vector2.One;
        public override void Draw()
        {
            if (frame >= 0 && frame <= 16)
            {
                Texture2D tex = CEUtils.RequestTex("CalamityEntropy/Content/Particles/realisticexplosion/spr_realisticexplosion_" + frame.ToString());
                Main.spriteBatch.Draw(tex, this.Position - Main.screenPosition, null, this.Color, 0, tex.Size() / 2f, size * Scale, SpriteEffects.None, 0);
            }
        }
        public override void AI()
        {
            frame++;
            if (frame == 0)
            {
                CEUtils.PlaySound("badexplosion", 0.8f, Position);
            }
            base.AI();
            if (frame > 16)
            {
                this.Lifetime = 0;
            }
        }
    }
}
