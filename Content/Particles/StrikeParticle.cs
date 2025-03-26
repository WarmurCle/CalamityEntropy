using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class StrikeParticle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/UpdraftParticle").Value;
        public override void onSpawn()
        {
            end = this.position;
            this.timeLeft = 60;
        }
        public override void update()
        {
            base.update();
            this.velocity *= 0.8f;
            end = Vector2.Lerp(end, this.position, 0.16f);
        }
        public Vector2 end;
        public override void draw()
        {
            Main.spriteBatch.Draw(texture, this.position - Main.screenPosition, null, new Color(255, 206, 180), (this.position - end).ToRotation(), this.getOrigin(), new Vector2(Util.Util.getDistance(this.position, end) / (float)texture.Width, this.scale * 0.3f), SpriteEffects.None, 0); ;
        }
    }
}
