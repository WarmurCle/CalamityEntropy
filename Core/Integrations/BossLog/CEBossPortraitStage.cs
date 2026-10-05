using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>场景原点在画布中心,WorldMatrix 含缩放和 UIScale</summary>
    internal readonly struct CEPortraitFrame
    {
        /// <summary>Canvas 在 UI 空间,已经让开 BossChecklist 标题区</summary>
        public readonly Rectangle Canvas;
        /// <summary>WorldMatrix 把场景坐标变到屏幕</summary>
        public readonly Matrix WorldMatrix;
        /// <summary>演员重启批次时沿用这份 Scissor,不然画布会被裁掉</summary>
        public readonly RasterizerState Scissor;
        /// <summary>进度隐藏蒙版色(正常 White,隐藏 Black)</summary>
        public readonly Color Mask;
        /// <summary>Masked 为真时体色归黑,加色辉光层跳过</summary>
        public readonly bool Masked;
        /// <summary>画布在场景坐标下的可视半宽/半高(背景铺满用)</summary>
        public readonly Vector2 SceneHalf;

        public CEPortraitFrame(Rectangle canvas, Matrix worldMatrix, RasterizerState scissor,
            Color mask, bool masked, Vector2 sceneHalf) {
            Canvas = canvas;
            WorldMatrix = worldMatrix;
            Scissor = scissor;
            Mask = mask;
            Masked = masked;
            SceneHalf = sceneHalf;
        }

        /// <summary>剪影时 MultiplyRGB,形状留着</summary>
        public Color Tint(Color color) => Masked ? color.MultiplyRGB(Mask) : color;

        /// <summary>剪影只压 RGB,alpha 不动</summary>
        public Color Dim(Color color, float maskedMul = 0.42f) {
            if (!Masked) {
                return color;
            }
            return new Color((int)(color.R * maskedMul), (int)(color.G * maskedMul), (int)(color.B * maskedMul), color.A);
        }
    }

    /// <summary>演员只在图鉴页驱动,不碰世界和 NPC</summary>
    internal abstract class CEBossPortraitActor
    {
        /// <summary>SceneHalfSize 用场景坐标,舞台靠它定缩放</summary>
        public abstract Vector2 SceneHalfSize { get; }

        /// <summary>Theme 是整本接管的色板和书缘</summary>
        public abstract CEBossLogTheme Theme { get; }

        /// <summary>场景时钟(秒,重置归零)</summary>
        protected float Time { get; private set; }

        /// <summary>LastStamp 存 Stopwatch 的 tick,步进和离页判定都读它</summary>
        internal long LastStamp;

        /// <summary>StepDebt 按秒积累固定步进</summary>
        internal float StepDebt;

        internal void Step(float dt) {
            Time += dt;
            Update(dt);
        }

        internal void ResetScene() {
            Time = 0f;
            Reset();
        }

        /// <summary>Update 收到的 dt 已经限幅,单位是秒</summary>
        protected abstract void Update(float dt);

        /// <summary>Reset 在首次进入或离页太久时调</summary>
        protected abstract void Reset();

        /// <summary>Draw 调用时批次已开,直接用场景坐标</summary>
        public abstract void Draw(SpriteBatch sb, in CEPortraitFrame frame);
    }

    /// <summary>
    /// Draw 是 customPortrait 回退,自己让出标题区,DrawScene 的画布由骨架算好
    /// 暂停时步进也走墙钟
    /// </summary>
    internal static class CEBossPortraitStage
    {
        /// <summary>TopReserve 在回退路径上让开 Boss 名、模组名和头图标</summary>
        private const int TopReserve = 52;
        private const int EdgeInset = 8;
        /// <summary>StepSeconds 对齐 60fps 逻辑帧</summary>
        private const float StepSeconds = 1f / 60f;
        /// <summary>MaxStepsPerDraw 让低帧率减速,不快进</summary>
        private const int MaxStepsPerDraw = 3;
        /// <summary>离页超过 StaleSeconds 才重开,卡顿尖峰不算</summary>
        private const float StaleSeconds = 2.5f;

        private static readonly RasterizerState scissorState = new() {
            CullMode = CullMode.None,
            ScissorTestEnable = true,
        };

        /// <summary>Draw 收到整页矩形,自己让出顶部</summary>
        public static void Draw(SpriteBatch sb, Rectangle pageRect, Color mask, CEBossPortraitActor actor) {
            Rectangle canvas = new(pageRect.X + EdgeInset, pageRect.Y + TopReserve,
                pageRect.Width - EdgeInset * 2, pageRect.Height - TopReserve - EdgeInset);
            DrawScene(sb, canvas, mask, actor);
        }

        public static void DrawScene(SpriteBatch sb, Rectangle canvas, Color mask, CEBossPortraitActor actor) {
            if (Main.dedServ || actor == null || sb == null) {
                return;
            }
            if (canvas.Width < 60 || canvas.Height < 60) {
                return;
            }

            //固定步 1/60,绘制率和同帧多次回调都不改速度
            //别用 TickCount64 毫秒差当 dt,15.6ms 粒度在高刷新下经常读出 0,会当成断绘把开场重置掉
            long now = Stopwatch.GetTimestamp();
            if (actor.LastStamp == 0) {
                actor.ResetScene();
                actor.StepDebt = StepSeconds;
            }
            else {
                double gapSec = (now - actor.LastStamp) / (double)Stopwatch.Frequency;
                if (gapSec > StaleSeconds) {
                    //超过 StaleSeconds 才重开,卡顿只补步
                    actor.ResetScene();
                    actor.StepDebt = StepSeconds;
                }
                else if (gapSec > 0) {
                    actor.StepDebt = MathF.Min(actor.StepDebt + (float)gapSec,
                        StepSeconds * (MaxStepsPerDraw + 0.5f));
                }
            }
            actor.LastStamp = now;

            int steps = 0;
            while (actor.StepDebt >= StepSeconds && steps < MaxStepsPerDraw) {
                actor.StepDebt -= StepSeconds;
                actor.Step(StepSeconds);
                steps++;
            }

            Vector2 half = actor.SceneHalfSize;
            float zoom = MathF.Min(canvas.Width / (half.X * 2f), canvas.Height / (half.Y * 2f));
            if (zoom <= 0f) {
                return;
            }
            Vector2 center = canvas.Center.ToVector2();
            Matrix worldMatrix = Matrix.CreateScale(zoom, zoom, 1f)
                * Matrix.CreateTranslation(center.X, center.Y, 0f)
                * Main.UIScaleMatrix;

            bool masked = mask.R < 40 && mask.G < 40 && mask.B < 40;
            CEPortraitFrame frame = new(canvas, worldMatrix, scissorState, mask, masked,
                new Vector2(canvas.Width * 0.5f / zoom, canvas.Height * 0.5f / zoom));

            GraphicsDevice gd = sb.GraphicsDevice;
            Rectangle prevScissor = gd.ScissorRectangle;
            sb.End();
            gd.ScissorRectangle = UiToScreen(canvas, gd);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, scissorState, null, worldMatrix);

            try {
                actor.Draw(sb, in frame);
            } finally {
                sb.End();
                gd.ScissorRectangle = prevScissor;
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            }
        }

        /// <summary>BeginAlpha 从加色或着色器批切回普通混合</summary>
        public static void BeginAlpha(SpriteBatch sb, in CEPortraitFrame frame) {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, frame.Scissor, null, frame.WorldMatrix);
        }

        /// <summary>BeginAdditive 要求调用方先 End</summary>
        public static void BeginAdditive(SpriteBatch sb, in CEPortraitFrame frame) {
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, frame.Scissor, null, frame.WorldMatrix);
        }

        /// <summary>BeginShader 用 Immediate,调用方先 End</summary>
        public static void BeginShader(SpriteBatch sb, in CEPortraitFrame frame, Effect effect) {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, frame.Scissor, effect, frame.WorldMatrix);
        }

        /// <summary>UiToScreen 按 UIScale 变换,再钳进视口</summary>
        private static Rectangle UiToScreen(Rectangle ui, GraphicsDevice gd) {
            Vector2 tl = Vector2.Transform(new Vector2(ui.X, ui.Y), Main.UIScaleMatrix);
            Vector2 br = Vector2.Transform(new Vector2(ui.Right, ui.Bottom), Main.UIScaleMatrix);
            Rectangle rect = new((int)tl.X, (int)tl.Y,
                (int)MathF.Ceiling(br.X - tl.X), (int)MathF.Ceiling(br.Y - tl.Y));
            return Rectangle.Intersect(rect, gd.Viewport.Bounds);
        }
    }
}
