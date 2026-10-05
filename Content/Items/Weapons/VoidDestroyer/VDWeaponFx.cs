using CalamityEntropy.Assets.Register;
using CalamityEntropy.Content.NPCs.VoidDestroyer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace CalamityEntropy.Content.Items.Weapons.VoidDestroyer
{
    /// <summary>
    /// 手持贴图的锚点换算:Pivot 是贴图像素坐标里的握点,绘制时落在 World。
    /// FNA 的翻转只翻纹理坐标、不翻几何,所以翻转后原点与贴图内各点的相对位置都要按同一条轴镜像
    /// </summary>
    public struct HeldSprite
    {
        public Texture2D Texture;
        public Vector2 World;
        public Vector2 Pivot;
        public float Rotation;
        public SpriteEffects Effects;
        public float Scale;

        public bool FlipH => (Effects & SpriteEffects.FlipHorizontally) != 0;
        public bool FlipV => (Effects & SpriteEffects.FlipVertically) != 0;

        public Vector2 Origin => new Vector2(FlipH ? Texture.Width - Pivot.X : Pivot.X, FlipV ? Texture.Height - Pivot.Y : Pivot.Y);

        /// <summary>贴图内一点的世界坐标</summary>
        public Vector2 ToWorld(Vector2 texPoint) {
            Vector2 d = texPoint - Pivot;
            if (FlipH) {
                d.X = -d.X;
            }
            if (FlipV) {
                d.Y = -d.Y;
            }
            return World + (d * Scale).RotatedBy(Rotation);
        }

        public void Draw(Color color) {
            Main.spriteBatch.Draw(Texture, World - Main.screenPosition, null, color, Rotation, Origin, Scale, Effects, 0f);
        }

        public void Draw(Texture2D tex, Color color) {
            Main.spriteBatch.Draw(tex, World - Main.screenPosition, null, color, Rotation, Origin, Scale, Effects, 0f);
        }

        /// <summary>只画贴图的一块(与整图对齐);发光层局部加亮用</summary>
        public void DrawPart(Texture2D tex, Rectangle src, Color color) {
            float x0 = FlipH ? Texture.Width - (src.X + src.Width) : src.X;
            float y0 = FlipV ? Texture.Height - (src.Y + src.Height) : src.Y;
            Main.spriteBatch.Draw(tex, World - Main.screenPosition, src, color, Rotation, Origin - new Vector2(x0, y0), Scale, Effects, 0f);
        }

        /// <summary>
        /// 沿一条贴图轴线持握的长物(矛、杖):axisAngle 为贴图里握点→尖端方向的角度;facing 为 -1 时水平翻转,保证贴图上沿始终朝上
        /// </summary>
        public static HeldSprite Along(Texture2D tex, Vector2 pivot, float axisAngle, Vector2 world, float aimRotation, int facing, float scale = 1f) {
            bool left = facing < 0;
            float shown = left ? MathHelper.Pi - axisAngle : axisAngle;
            return new HeldSprite {
                Texture = tex,
                World = world,
                Pivot = pivot,
                Rotation = aimRotation - shown,
                Effects = left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                Scale = scale
            };
        }

        /// <summary>横放的枪械:贴图朝右,facing 为 -1 时垂直翻转</summary>
        public static HeldSprite Gun(Texture2D tex, Vector2 pivot, Vector2 world, float rotation, int facing, float scale = 1f) {
            bool left = facing < 0;
            return new HeldSprite {
                Texture = tex,
                World = world,
                Pivot = pivot,
                Rotation = rotation,
                Effects = left ? SpriteEffects.FlipVertically : SpriteEffects.None,
                Scale = scale
            };
        }
    }

    /// <summary>
    /// 驱逐舰武器系列共用的绘制件。顶点带只走加法:BasicTrail 是不透明黑底的横向软带(V 方向横跨带宽),黑底在加法里不贡献。
    /// 调用 <see cref="Ribbon"/> 前先 <see cref="BeginAdditive"/>,画完 <see cref="End"/> 回到 Deferred/AlphaBlend
    /// </summary>
    public static class VDWeaponFx
    {
        private static ColoredVertex[] strip = new ColoredVertex[64];

        public static Texture2D Trail => CEExtraAssets.BasicTrail ?? CEUtils.getExtraTex("BasicTrail");
        public static Texture2D GlowTex => CEExtraAssets.Glow ?? CEUtils.getExtraTex("Glow");
        public static Texture2D RingTex => CEExtraAssets.BloomRing ?? CEUtils.getExtraTex("BloomRing");

        public static void BeginAdditive() => Main.spriteBatch.UseAdditive();

        public static void End() => CEUtils.ReSetToEndShader();

        /// <summary>Glow 贴图按可见半径画(贴图边缘即半径);加法批次内调用</summary>
        public static void Glow(Vector2 world, Color color, float radius) {
            Texture2D tex = GlowTex;
            Main.spriteBatch.Draw(tex, world - Main.screenPosition, null, color, 0f, tex.Size() / 2f, radius * 2f / tex.Width, SpriteEffects.None, 0f);
        }

        /// <summary>Glow 贴图沿方向拉成椭圆光斑(枪口焰、尖端光);加法批次内调用</summary>
        public static void GlowStretched(Vector2 world, Color color, float rotation, float length, float width) {
            Texture2D tex = GlowTex;
            Main.spriteBatch.Draw(tex, world - Main.screenPosition, null, color, rotation, tex.Size() / 2f, new Vector2(length / tex.Width, width / tex.Height), SpriteEffects.None, 0f);
        }

        /// <summary>BloomRing 等比缩放到给定环半径(贴图里环中线约在 0.38 倍边长处);加法批次内调用</summary>
        public static void Ring(Vector2 world, Color color, float radius) {
            Texture2D tex = RingTex;
            Main.spriteBatch.Draw(tex, world - Main.screenPosition, null, color, 0f, tex.Size() / 2f, radius / (tex.Width * 0.38f), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// 沿点列画一条顶点带(点列从头到尾)。width / color 按进度 0(首点)→1(末点)取值,法线取相邻两点的中心差,拐角不断开。
        /// 必须在 <see cref="BeginAdditive"/> 开出的批次里调用
        /// </summary>
        public static void Ribbon(IList<Vector2> points, Func<float, float> width, Func<float, Color> color, Texture2D tex = null) {
            int n = points.Count;
            if (n < 2) {
                return;
            }
            if (strip.Length < n * 2) {
                strip = new ColoredVertex[n * 2 + 16];
            }
            for (int i = 0; i < n; i++) {
                Vector2 prev = points[Math.Max(i - 1, 0)];
                Vector2 next = points[Math.Min(i + 1, n - 1)];
                Vector2 tangent = (next - prev).SafeNormalize(Vector2.UnitX);
                Vector2 normal = new Vector2(-tangent.Y, tangent.X);
                float t = i / (float)(n - 1);
                float half = width(t) * 0.5f;
                Color c = color(t);
                Vector2 p = points[i] - Main.screenPosition;
                strip[i * 2] = new ColoredVertex(p + normal * half, c, new Vector3(t, 0f, 1f));
                strip[i * 2 + 1] = new ColoredVertex(p - normal * half, c, new Vector3(t, 1f, 1f));
            }
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            gd.Textures[0] = tex ?? Trail;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, strip, 0, n * 2 - 2);
        }

        /// <summary>两点之间的锥形曳光:头亮尾淡、头宽尾细(子弹、三叉戟、虚空弹的短拖尾)</summary>
        public static void Streak(Vector2 head, Vector2 tail, float width, Color headColor, Color tailColor) {
            pair[0] = head;
            pair[1] = Vector2.Lerp(head, tail, 0.35f);
            pair[2] = tail;
            Ribbon(pair, t => width * (1f - t * 0.85f), t => Color.Lerp(headColor, tailColor, t) * (1f - t));
        }

        private static readonly Vector2[] pair = new Vector2[3];

        /// <summary>
        /// 在 a 到 b 之间生成一条锯齿电弧,两端固定,中段按正弦包络横向抖动,jitter 是最大横向偏移(像素)
        /// </summary>
        public static void BuildArc(List<Vector2> into, Vector2 a, Vector2 b, int segments, float jitter, UnifiedRandom rand) {
            into.Clear();
            Vector2 d = b - a;
            float len = d.Length();
            Vector2 n = len > 0.01f ? new Vector2(-d.Y, d.X) / len : Vector2.UnitY;
            for (int i = 0; i <= segments; i++) {
                float t = i / (float)segments;
                float env = MathF.Sin(t * MathHelper.Pi);
                float off = i == 0 || i == segments ? 0f : rand.NextFloat(-1f, 1f) * jitter * env;
                into.Add(a + d * t + n * off);
            }
        }

        /// <summary>DrawArc 画出一条电弧,宽色晕加上细白热芯,调用时批次必须是加法</summary>
        public static void DrawArc(IList<Vector2> points, Color glow, Color core, float width, float opacity) {
            if (opacity <= 0.01f) {
                return;
            }
            Ribbon(points, _ => width * 3.2f, _ => glow * (0.55f * opacity));
            Ribbon(points, _ => width, _ => core * opacity);
        }
    }

    /// <summary>
    /// 等离子球(虚空电场的闪电球,杖尖与飞出后共用):暗色球体打底(AlphaBlend 真 alpha 的 Glow 着暗色),
    /// 加法叠外晕、球壳环、从球心伸到球面的锯齿电丝与白热核。电丝每 3 帧重抖一次,端点角度缓慢游走,读作球里爬动的放电
    /// </summary>
    public class PlasmaBall
    {
        public const int MaxFilaments = 10;
        public static readonly Color Deep = new Color(14, 4, 30);

        private readonly float[] endAngle = new float[MaxFilaments];
        private readonly float[] drift = new float[MaxFilaments];
        private readonly List<Vector2>[] filaments = new List<Vector2>[MaxFilaments];
        private readonly List<Vector2> leap = new List<Vector2>();
        private float leapLife;
        private int frame;

        public PlasmaBall() {
            for (int i = 0; i < MaxFilaments; i++) {
                filaments[i] = new List<Vector2>();
                endAngle[i] = i * MathHelper.TwoPi / MaxFilaments + Main.rand.NextFloat(-0.3f, 0.3f);
                drift[i] = Main.rand.NextFloat(-0.08f, 0.08f);
            }
        }

        /// <summary>每帧调一次(纯表现,只在客户端);charge 0..1 决定电丝数量与外跃电弧频率</summary>
        public void Update(float charge) {
            frame++;
            for (int i = 0; i < MaxFilaments; i++) {
                endAngle[i] += drift[i];
                if (Main.rand.NextBool(40)) {
                    drift[i] = Main.rand.NextFloat(-0.08f, 0.08f);
                }
            }
            if (frame % 3 == 0) {
                for (int i = 0; i < MaxFilaments; i++) {
                    Vector2 end = endAngle[i].ToRotationVector2() * Main.rand.NextFloat(0.9f, 1f);
                    VDWeaponFx.BuildArc(filaments[i], CEUtils.randomPointInCircle(0.12f), end, 5, 0.22f, Main.rand);
                }
            }
            leapLife -= 1f;
            if (leapLife <= 0f && Main.rand.NextFloat() < 0.04f + 0.2f * charge) {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 from = a.ToRotationVector2();
                Vector2 to = (a + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2() * Main.rand.NextFloat(1.35f, 1.9f);
                VDWeaponFx.BuildArc(leap, from, to, 4, 0.25f, Main.rand);
                leapLife = 4f;
            }
        }

        /// <summary>
        /// 画球:radius 为球壳半径(像素)。进出批次:进来时是默认 Deferred/AlphaBlend,出去时还原
        /// </summary>
        public void Draw(Vector2 center, float radius, float charge, float opacity, Color main, Color core) {
            if (opacity <= 0.01f || radius < 1f) {
                return;
            }
            float flicker = 0.92f + 0.08f * MathF.Sin(frame * 0.9f) * MathF.Sin(frame * 0.37f);
            Texture2D glow = VDWeaponFx.GlowTex;
            //暗色球体:真 alpha 的 Glow 在 AlphaBlend 里压暗背景,电丝才有底
            Main.spriteBatch.Draw(glow, center - Main.screenPosition, null, Deep * (0.75f * opacity), 0f, glow.Size() / 2f, radius * 2.5f / glow.Width, SpriteEffects.None, 0f);

            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Glow(center, main * (0.32f * opacity * flicker), radius * 2.3f);
            VDWeaponFx.Glow(center, main * (0.35f * opacity), radius * 1.15f);
            int count = 3 + (int)((MaxFilaments - 3) * charge);
            float w = MathHelper.Clamp(radius * 0.06f, 1.2f, 2.6f);
            for (int i = 0; i < count; i++) {
                var f = filaments[i];
                if (f.Count < 2) {
                    continue;
                }
                scratchList.Clear();
                foreach (Vector2 p in f) {
                    scratchList.Add(center + p * radius);
                }
                VDWeaponFx.DrawArc(scratchList, main, core, w, opacity * (0.7f + 0.3f * flicker));
                VDWeaponFx.Glow(scratchList[^1], core * (0.6f * opacity), w * 4f);
            }
            if (leapLife > 0f && leap.Count > 1) {
                scratchList.Clear();
                foreach (Vector2 p in leap) {
                    scratchList.Add(center + p * radius);
                }
                VDWeaponFx.DrawArc(scratchList, main, core, w * 0.9f, opacity * (leapLife / 4f));
            }
            VDWeaponFx.Ring(center, main * (0.65f * opacity), radius);
            VDWeaponFx.Ring(center, core * (0.25f * opacity * flicker), radius * 0.96f);
            VDWeaponFx.Glow(center, core * (0.85f * opacity * flicker), radius * (0.42f + 0.1f * charge));
            VDWeaponFx.Glow(center, Color.White * (0.7f * opacity), radius * 0.18f);
            VDWeaponFx.End();
        }

        private readonly List<Vector2> scratchList = new List<Vector2>(16);
    }
}
