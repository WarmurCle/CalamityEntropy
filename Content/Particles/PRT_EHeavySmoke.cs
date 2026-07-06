using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    //EParticle迁来的HeavySmoke,MercyShoot叠在HeavySmokeCal上面spawn这个做发光层
    public class PRT_EHeavySmoke : BasePRT
    {
        private static int FrameAmount = 6;

        public float Spin;
        public bool StrongVisual;
        public bool Glowing = true;
        public float HueShift;
        public int Variant;
        public bool AffectedByLight;

        public override bool CanPool => true;

        public override void Reset()
        {
            //CanPool,Glow字段Reset里得登记,跟Configure传的glow不是一回事
            base.Reset();
            Spin = 0f;
            StrongVisual = false;
            Glowing = true;
            HueShift = 0f;
            Variant = 0;
            AffectedByLight = false;
            Glow = true;
        }

        //贴图在@CalamityMod/Particles/HeavySmoke,Texture认不了@语法(HasAsset查不到)
        //不报错,日志Warn+游戏里一块红占位图,指WhiteTexPath应付框架,真图PreDraw走PRTSharedAssets.HeavySmoke
        public override string Texture => CEUtils.WhiteTexPath;

        public PRT_EHeavySmoke Configure(float opacity, bool glow, PRTDrawModeEnum mode,
            float rotation = 0f, int lifetime = -1)
        {
            Opacity = opacity;
            Glow = glow;
            PRTDrawMode = mode;
            Rotation = rotation;
            if (lifetime > 0)
                Lifetime = lifetime;
            return this;
        }

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
            if (Lifetime <= 0)
                Lifetime = 200;
            Variant = Main.rand.Next(7);
        }

        public override void AI()
        {
            //前20%寿命膨胀后衰减,跟HeavySmokeCal同一套AI,只是MercyShoot叠这层做Additive发光
            if (LifetimeCompletion < 0.2f)
                Scale += 0.01f;
            else
                Scale *= 0.975f;

            Color = Main.hslToRgb((Main.rgbToHsl(Color).X + HueShift) % 1f, Main.rgbToHsl(Color).Y, Main.rgbToHsl(Color).Z);
            Opacity *= 0.98f;
            Rotation += Spin * (Velocity.X > 0f ? 1f : -1f);
            Velocity *= 0.85f;
            float lerpValue = Utils.GetLerpValue(1f, 0.85f, LifetimeCompletion, clamped: true);   //Completion 0→1,旧fade走remaining别写反
            Color *= lerpValue;
        }

        public override bool PreDraw(SpriteBatch sb)
        {
            //6帧竖排80x80,Variant横排7列,VaultLoaden在PRTSharedAssets
            Texture2D value = PRTSharedAssets.HeavySmoke.Value;
            int num = (int)Math.Floor((float)Time / ((float)Lifetime / (float)FrameAmount));
            Rectangle rectangle = new Rectangle(80 * Variant, 80 * num, 80, 80);
            Color color = Color * Opacity;
            if (AffectedByLight)
                color = color.MultiplyRGBA(Lighting.GetColor((Position / 16f).ToPoint()));

            sb.Draw(value, Position - Main.screenPosition, rectangle, color, Rotation, rectangle.Size() / 2f, Scale, SpriteEffects.None, 0f);
            return false;
        }

        public bool Glow = true;
    }


}
