using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EXPLOSION : EParticle
    {
        
        public override void onSpawn()
        {
            this.timeLeft = 96;
        }
        public int frame = 0;
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/EXPLOSION").Value;
        public override void draw()
        {
            Texture2D tex = texture;
            Rectangle rect = new Rectangle(frame % 11 * 240, (int)(frame / 11) * 135, 240, 135);
            Main.spriteBatch.Draw(tex, position - Main.screenPosition, rect, color, this.rotation, new Vector2(120, 135 / 2f), scale, SpriteEffects.None, 0);
        }
        public override void update()
        {
            frame++;
            base.update();
        }
    }
    public class EXPLOSIONCOSMIC : EParticle
    {

        public override void onSpawn()
        {
            this.timeLeft = 96;
        }
        public int frame = 0;
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/EXPLOSIONCOSMIC").Value;
        public override void draw()
        {
            Texture2D tex = texture;
            Rectangle rect = new Rectangle(frame % 11 * 240, (int)(frame / 11) * 135, 240, 135);
            Main.spriteBatch.Draw(tex, position - Main.screenPosition, rect, color, this.rotation, new Vector2(120, 135 / 2f), scale, SpriteEffects.None, 0);
        }
        public override void update()
        {
            frame++;
            base.update();
        }
    }
}
