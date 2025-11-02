using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class DOracleSlash : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LargeSpark").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 64;
        }
        public float B = -1;
        public float A = -1;
        public float vel = 0.5f;
        public override void AI()
        {
            base.AI();
            A += vel;
            vel *= 0.88f;
            B = float.Lerp(B, A, 0.02f);
        }
        public override void Draw()
        {
            Vector2 size = new Vector2(float.Min(1, Lifetime / 6f) * Scale / 720f * 0.3f, (A - B) * Scale / 720f);
            Vector2 drawPos = Position + Rotation.ToRotationVector2() * Scale * ((A + B) / 2f);
            Main.EntitySpriteDraw(Texture, drawPos - Main.screenPosition, null, Color, Rotation + MathHelper.PiOver2, Texture.Size() / 2f, size, SpriteEffects.None);
            Main.EntitySpriteDraw(Texture, drawPos - Main.screenPosition, null, Color.Black, Rotation + MathHelper.PiOver2, Texture.Size() / 2f, size * 0.6f, SpriteEffects.None);

        }
    }
}
