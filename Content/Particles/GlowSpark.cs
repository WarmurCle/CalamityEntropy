using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class GlowSpark : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark").Value;
        public override void SetProperty()
        {
            this.Lifetime = 26;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.Rotation = this.velocity.ToRotation();
        }
    }
    public class GlowSpark2 : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark2").Value;
        public override void SetProperty()
        {
            this.Lifetime = 26;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.Rotation = this.velocity.ToRotation();
        }
    }
    public class GlowSparkDirecting : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark").Value;
        public Vector2 TargetPos;
        public float scaleX = 1f;
        public Vector2 SpawnPos;
        public Entity followOwner;
        public Vector2 ownerLastPos = Vector2.Zero;
        public override void SetProperty()
        {
            SpawnPos = position;
        }
        public override void AI()
        {
            if (followOwner != null)
            {
                if (ownerLastPos == Vector2.Zero)
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
            base.AI();
            this.Opacity = 1 - (this.Lifetime / (float)TimeLeftMax);
            this.position = Vector2.Lerp(TargetPos, SpawnPos, (this.Lifetime / (float)TimeLeftMax));
            Rotation = (TargetPos - SpawnPos).ToRotation();
        }
        public override void PreDraw()
        {

            Color clr = this.Color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            if (!this.useAdditive && !this.useAlphaBlend)
            {
                clr.A = (byte)(clr.A * Opacity);
            }
            else
            {
                clr *= Opacity;
            }
            Main.spriteBatch.Draw(this.Texture, this.position - Main.screenPosition, null, clr, Rotation, getOrigin(), Scale * new Vector2(scaleX, 1), SpriteEffects.None, 0);
            CEUtils.DrawGlow(position, Color * 0.8f, Scale * 0.4f);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
        }
    }
}
