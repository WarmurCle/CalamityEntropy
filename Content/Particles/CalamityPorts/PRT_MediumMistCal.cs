using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles.CalamityPorts
{
    //照CalamityMod MediumMistParticle翻的,MistOpacity减到0自己Kill不靠Lifetime到点
    public class PRT_MediumMistCal : BasePRT
    {
        public float MistOpacity;
        public Color ColorFire;
        public Color ColorFade;
        public float Spin;
        public int Variant;

        public override bool CanPool => true;

        public override void Reset()
        {
            base.Reset();
            MistOpacity = 0f;
            ColorFire = default;
            ColorFade = default;
            Spin = 0f;
            Variant = 0;
        }

        //@CalamityMod/Particles/MediumMist,Texture认不了@语法,WhiteTexPath占位真图PreDraw里VaultLoaden拿
        public override string Texture => CEUtils.WhiteTexPath;

        public PRT_MediumMistCal Configure(Color colorFade, float opacity, float rotationSpeed = 0f)
        {
            ColorFire = Color;
            ColorFade = colorFade;
            MistOpacity = opacity;
            Spin = rotationSpeed;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            return this;
        }

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
            if (Lifetime <= 0)
                Lifetime = 200;
            Variant = Main.rand.Next(3);
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            Rotation += Spin * (Velocity.X > 0f ? 1f : -1f);
            Velocity *= 0.85f;

            if (MistOpacity > 90f)
            {
                Lighting.AddLight(Position, Color.ToVector3() * 0.1f);   //高MistOpacity段带微光,旧Update原样
                Scale += 0.01f;
                MistOpacity -= 3f;
            }
            else
            {
                Scale *= 0.975f;
                MistOpacity -= 2f;
            }

            if (MistOpacity < 0f)
                Kill();   //MistOpacity耗尽自杀,不靠Lifetime到点,跟Cal原版一致

            Color = Color.Lerp(ColorFire, ColorFade, MathHelper.Clamp((255f - MistOpacity - 100f) / 80f, 0f, 1f)) * (MistOpacity / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = PRTSharedAssets.MediumMist.Value;   //横排3列,Variant随机0-2
            int frameWidth = tex.Width / 3;
            Rectangle frame = new Rectangle(frameWidth * Variant, 0, frameWidth, tex.Height);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frame, Color, Rotation, new Vector2(frameWidth / 2f, tex.Height / 2f), Scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
