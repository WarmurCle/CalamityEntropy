using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace CalamityEntropy.Content.Particles
{
    public class EHeavySmoke : EParticle
    {
        private float Spin;

        private bool StrongVisual;

        private bool Glowing;

        private float HueShift;

        private static int FrameAmount = 6;

        public int FrameVariants => 7;
        public int Variant;
        public bool AffectedByLight = true;
        public override Texture2D Texture => CEUtils.RequestTex("CalamityMod/Particles/HeavySmoke");

        public void SetValues(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float opacity, float rotationSpeed = 0f, bool glowing = false, float hueshift = 0f, bool required = false, bool affectedByLight = false)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Variant = Main.rand.Next(7);
            Lifetime = lifetime;
            Opacity = opacity;
            Spin = rotationSpeed;
            StrongVisual = required;
            Glowing = true;
            HueShift = hueshift;
            AffectedByLight = false;
        }
        public int Time = 0;
        public override void AI()
        {
            Time++;
            float LifetimeCompletion = (float)Time / TimeLeftMax;
            if ((float)Time / (float)Lifetime < 0.2f)
            {
                Scale += 0.01f;
            }
            else
            {
                Scale *= 0.975f;
            }

            Color = Main.hslToRgb((Main.rgbToHsl(Color).X + HueShift) % 1f, Main.rgbToHsl(Color).Y, Main.rgbToHsl(Color).Z);
            Opacity *= 0.98f;
            Rotation += Spin * ((Velocity.X > 0f) ? 1f : (-1f));
            Velocity *= 0.85f;
            float lerpValue = Utils.GetLerpValue(1f, 0.85f, LifetimeCompletion, clamped: true);
            Color *= lerpValue;
            Position += Velocity;
            Lifetime--;
        }
        public override void Draw()
        {
            CustomDraw(Main.spriteBatch);
        }
        public void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D value = Texture;
            int num = (int)Math.Floor((float)Time / ((float)Lifetime / (float)FrameAmount));
            Rectangle rectangle = new Rectangle(80 * Variant, 80 * num, 80, 80);
            Color color = Color * Opacity;
            if (AffectedByLight)
            {
                color = color.MultiplyRGBA(Lighting.GetColor((Position / 16f).ToPoint()));
            }

            spriteBatch.Draw(value, Position - Main.screenPosition, rectangle, color, Rotation, rectangle.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }
    }
}
