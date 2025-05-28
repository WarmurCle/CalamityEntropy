using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class StrikeParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/UpdraftParticle").Value;
        public override void SetProperty()
        {
            end = this.position;
            this.Lifetime = 60;
        }
        public override void AI()
        {
            base.AI();
            this.velocity *= 0.8f;
            end = Vector2.Lerp(end, this.position, 0.16f);
        }
        public Vector2 end;
        public override void PreDraw()
        {
            Main.spriteBatch.Draw(Texture, this.position - Main.screenPosition, null, new Color(255, 206, 180), (this.position - end).ToRotation(), this.getOrigin(), new Vector2(CEUtils.getDistance(this.position, end) / (float)Texture.Width, this.Scale * 0.3f), SpriteEffects.None, 0); ;
        }
    }
}
