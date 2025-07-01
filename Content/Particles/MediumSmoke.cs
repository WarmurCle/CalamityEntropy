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
        public override void OnSpawn()
        {
            base.OnSpawn();
            this.orgColor = this.Color;
            this.Lifetime = 80;
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
            this.Rotation += rotDir * 0.03f;
            this.Opacity = ((float)this.Lifetime / (float)timeleftmax);
            this.Color = Color.Lerp(this.Color, Color.Black, 0.03f);
            this.Velocity *= 0.9f;
            this.Velocity = this.Velocity + new Vector2(0, -0.1f);
        }
        public override Vector2 getOrigin()
        {
            return base.getOrigin() * new Vector2(1, 0.3333f);
        }
        public override void Draw()
        {
            Color clr = this.Color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.Position.X / 16)), ((int)(this.Position.Y / 16)), clr);
            }
            clr = Color.Lerp(clr, Color.White, wl);
            Main.spriteBatch.Draw(this.Texture, this.Position - Main.screenPosition, CEUtils.GetCutTexRect(Texture, 3, texT, false), clr * Opacity, Rotation, getOrigin(), Scale, SpriteEffects.None, 0);
        }
    }
}
