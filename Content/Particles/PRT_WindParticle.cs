using CalamityMod;
using CalamityMod.Graphics.Primitives;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityEntropy.Content.Particles
{
    public class PRT_WindParticle : BasePRT
    {
        //rv/r/dir在字段初始化器里掷随机,池化不会重跑,这类带轨迹List干脆不池化
        public List<Vector2> odp = new List<Vector2>();
        public float v1 = 12;
        public float v2 = 3;
        public float rv = Main.rand.NextFloat(0.14f, 0.2f);
        public float r = CEUtils.randomRot();
        public int dir = Main.rand.NextBool() ? 1 : -1;
        public bool Glow = true;

        public override string Texture => "CalamityEntropy/Content/Particles/Wind";

        public PRT_WindParticle Configure(float opacity, bool glow, PRTDrawModeEnum mode,
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
            //Lifetime 46旧Wind默认,-1漏设就永生一路囤odp
            if (Lifetime <= 0)
                Lifetime = 46;
        }

        public override void AI()
        {
            Opacity = 1f - LifetimeCompletion;   //衰减放AI不是PreDraw,跟旧updateAll一致
            Velocity = Rotation.ToRotationVector2() * v1 + r.ToRotationVector2() * v2;
            Rotation += dir * rv;
            //每帧Insert(0)采样,最多16点;旧WindParticle原值
            odp.Insert(0, Position);
            if (odp.Count > 16)
                odp.RemoveAt(odp.Count - 1);
        }

        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            return Color * completionRatio * Opacity * new Vector2(1, 0).RotatedBy(completionRatio * MathHelper.Pi).Y;
        }

        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            return Scale * 26;
        }

        public override bool PreDraw(SpriteBatch sb)
        {
            sb.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(PRTSharedAssets.Wind);
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            //180是RenderTrail最大段数上限,旧drawAll同样传的,跟odp实际16点不是一回事
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            sb.ExitShaderRegion();
            //ArtAttack+RenderTrail会End内部批次,Exit后还得End再BeginDrawingWithMode接回去
            sb.End();
            PRTLoader.BeginDrawingWithMode(PRTDrawMode, sb);
            return false;
        }
    }


}
