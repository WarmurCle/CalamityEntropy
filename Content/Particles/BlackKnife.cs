using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class BlackKnifeParticle : EParticle
    {
        public override Texture2D Texture => CEUtils.RequestTex("CalamityEntropy/Content/Projectiles/BlackKnife");
        public List<Vector2> oldPos = new List<Vector2>();
        public override void AI()
        {
            base.AI();
            for (float i = 0; i < 1; i += 0.025f)
            {
                oldPos.Add(Position - (1 - i) * Velocity);
                if (oldPos.Count > 400)
                {
                    oldPos.RemoveAt(0);
                }
            }

        }
        public override void Draw()
        {
            Texture2D tex = Texture;
            for (int i = 0; i < oldPos.Count; i++)
            {
                Main.spriteBatch.Draw(tex, oldPos[i] - Main.screenPosition, null, Color * 0.1f * ((float)i / oldPos.Count), this.Rotation, tex.Size() / 2, Scale, SpriteEffects.None, 0);

            }
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, this.Rotation, tex.Size() / 2, Scale, SpriteEffects.None, 0);

        }
    }
}
