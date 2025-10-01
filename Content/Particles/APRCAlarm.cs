using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class APRCAlarm : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("APRCAlarm");
        public override void Draw()
        {
            float alpha = (this.TimeLeftMax - this.Lifetime) / (this.TimeLeftMax * 0.6f);
            if (alpha > 1)
                alpha = 1;
            Texture2D o = CEUtils.getExtraTex("APRCAlarm2");
            float orot = 2-CEUtils.Parabola(alpha * 0.5f, 2);
            float scale = 2 - alpha;
            Main.spriteBatch.Draw(o, Position - Main.screenPosition, null, Color * alpha * Opacity, orot + Rotation, o.Size() / 2f, Scale * scale / 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, Color * alpha * Opacity, Rotation, o.Size() / 2f, Scale / 2f, SpriteEffects.None, 0);

        }
    }
}
