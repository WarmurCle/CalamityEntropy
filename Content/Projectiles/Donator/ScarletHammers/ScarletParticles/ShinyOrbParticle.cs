using CalamityEntropy.Assets.Register;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles
{
    public class ShinyOrbParticle :BaseParticle
    {
        public override BlendState BlendState => UseBlendState;
        public BlendState UseBlendState;
        public bool AffectedByGravity;
        public bool GlowCenter;
        public float FadeOut;
        public Color InitColor;
        public override string Texture => CEUtils.InvisAsset;
        public ShinyOrbParticle(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale, BlendState blendState, bool affactedByGravity = false, bool glowCenter = true )
        {
            Position = position;
            Velocity = velocity;
            DrawColor = InitColor = color;
            Lifetime = lifeTime;
            Scale = scale;
            UseBlendState = blendState;
            AffectedByGravity = affactedByGravity;
            GlowCenter = glowCenter;
            FadeOut = 1f;
        }
        public override void Update()
        {
            FadeOut -= 0.05f;
            Scale *= 0.93f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            Velocity *= 0.95f;
            if(Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(1f, 1f) * Scale;
            Texture2D texture = TextureRegister.General_WhiteOrb.Value;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            if (GlowCenter)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * FadeOut, Rotation, texture.Size() * 0.5f, scale * new Vector2(0.5f, 0.5f), 0, 0f);
        }
    }
}
