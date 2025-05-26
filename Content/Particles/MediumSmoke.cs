using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EMediumSmoke : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/MediumSmoke").Value;
        public int timeleftmax = 80;
        public Color orgColor;
        public override void SetProperty()
        {
            base.SetProperty();
            this.orgColor = this.color;
            this.timeLeft = 80;
        }
        public float rotDir = Main.rand.NextBool() ? 1 : -1;
        public int texT = Main.rand.Next(0, 3);
        public float wl = 1;
        public override void AI()
        {
            base.AI();
            if (wl > 0)
            {
                wl -= 0.1f;
            }
            this.rotation += rotDir * 0.03f;
            this.alpha = ((float)this.timeLeft / (float)timeleftmax);
            this.color = Color.Lerp(this.color, Color.Black, 0.03f);
            this.velocity *= 0.9f;
            this.velocity = this.velocity + new Vector2(0, -0.1f);
        }
        public override Vector2 getOrigin()
        {
            return base.getOrigin() * new Vector2(1, 0.3333f);
        }
        public override void PreDraw()
        {
            Color clr = this.color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            clr = Color.Lerp(clr, Color.White, wl);
            Main.spriteBatch.Draw(this.Texture, this.position - Main.screenPosition, Utilities.Util.GetCutTexRect(Texture, 3, texT, false), clr * alpha, rotation, getOrigin(), scale, SpriteEffects.None, 0);
        }
    }
}
