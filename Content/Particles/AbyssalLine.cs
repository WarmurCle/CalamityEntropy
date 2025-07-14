using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class AbyssalLine : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/AbyssalLine").Value;
        public float timemax => this.TimeLeftMax;
        public Color spawnColor = new Color(190, 190, 255);
        public Color endColor = Color.Blue;
        public float xscale = 0;
        public float xdec = 0.87f;
        public float xadd = 3.2f;
        public float lx = 3;
        public override void OnSpawn()
        {
            this.Lifetime = 50;
        }
        public override void AI()
        {
            base.AI();
            lx *= 0.88f;
            xscale += xadd;
            xadd *= xdec;
        }
        public override void Draw()
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color.Lerp(endColor, spawnColor, ((float)Lifetime / timemax)) * ((float)Lifetime / timemax) * 0.7f, this.Rotation, tex.Size() / 2, new Vector2(0.6f * (xscale + 0.1f), 0.56f * lx) * Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color.Lerp(endColor, spawnColor, ((float)Lifetime / timemax)) * ((float)Lifetime / timemax), this.Rotation, tex.Size() / 2, new Vector2(0.6f * xscale, 0.2f * lx) * Scale, SpriteEffects.None, 0);
        }
    }
}
