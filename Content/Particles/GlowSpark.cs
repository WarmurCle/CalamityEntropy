using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class GlowSpark : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark").Value;
        public override void onSpawn()
        {
            this.timeLeft = 26;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.rotation = this.velocity.ToRotation();
        }
    }
    public class GlowSpark2 : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark2").Value;
        public override void onSpawn()
        {
            this.timeLeft = 26;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.rotation = this.velocity.ToRotation();
        }
    }
    public class GlowSparkDirecting : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark").Value;
        public Vector2 TargetPos;
        public float scaleX = 1f;
        public Vector2 SpawnPos;
        public Entity followOwner;
        public Vector2 ownerLastPos = Vector2.Zero;
        public override void onSpawn()
        {
            SpawnPos = position;
        }
        public override void update()
        {
            if (followOwner != null)
            {
                if(ownerLastPos == Vector2.Zero)
                {
                    ownerLastPos = followOwner.Center;
                }
                else
                {
                    TargetPos += (followOwner.Center - ownerLastPos);
                    SpawnPos += (followOwner.Center - ownerLastPos);
                }
                ownerLastPos = followOwner.Center;
            }
            base.update();
            this.alpha = 1 - (this.timeLeft / (float)TimeLeftMax);
            this.position = Vector2.Lerp(TargetPos, SpawnPos, (this.timeLeft / (float)TimeLeftMax));
            rotation = (TargetPos - SpawnPos).ToRotation();
        }
        public override void draw()
        {

            Color clr = this.color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            if (!this.useAdditive && !this.useAlphaBlend)
            {
                clr.A = (byte)(clr.A * alpha);
            }
            else
            {
                clr *= alpha;
            }
            Main.spriteBatch.Draw(this.texture, this.position - Main.screenPosition, null, clr, rotation, getOrigin(), scale * new Vector2(scaleX, 1), SpriteEffects.None, 0);
            Util.DrawGlow(position, color * 0.8f, scale * 0.4f);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
        }
    }
}
