using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EXPLOSION : EParticle
    {

        public override void OnSpawn()
        {
            this.Lifetime = 96;
        }
        public int frame = 0;
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/EXPLOSION").Value;
        public override void PreDraw()
        {
            Texture2D tex = Texture;
            Rectangle rect = new Rectangle(frame % 11 * 240, (int)(frame / 11) * 135, 240, 135);
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, rect, Color, this.Rotation, new Vector2(120, 135 / 2f), Scale, SpriteEffects.None, 0);
        }
        public override void AI()
        {
            frame++;
            base.AI();
        }
    }
    public class EXPLOSIONCOSMIC : EParticle
    {

        public override void OnSpawn()
        {
            this.Lifetime = 96;
        }
        public int frame = 0;
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/EXPLOSIONCOSMIC").Value;
        public override void PreDraw()
        {
            Texture2D tex = Texture;
            Rectangle rect = new Rectangle(frame % 11 * 240, (int)(frame / 11) * 135, 240, 135);
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, rect, Color, this.Rotation, new Vector2(120, 135 / 2f), Scale, SpriteEffects.None, 0);
        }
        public override void AI()
        {
            frame++;
            base.AI();
        }
    }
}
