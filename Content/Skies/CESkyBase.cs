using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    /// <summary>
    /// SkyManager 每帧按视差层回调十余次,贴近地狱还会再来一轮,切片条件一帧可能双触发
    /// ModifyTransformMatrix 每渲染帧恰好一次,且早于全部背景,帧戳用它去重
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class CESkyFrameStamp : ModSystem
    {
        public static int Current { get; private set; }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform) => Current++;
    }

    /// <summary>
    /// DrawBG 里 screenWidth/Height 被 Zoom 预除,screenPosition 加了背景平移,批次是背景矩阵
    /// 调用方空间和原始像素空间不能混用,4K 铺不满就是混出来的
    /// </summary>
    public static class CESkyDrawing
    {
        /// <summary>还原 Main.DoDraw 传给 DrawBG 的背景矩阵(含重力翻转与背景缩放平移补偿)。</summary>
        public static Matrix BackgroundMatrix() {
            Matrix m = Main.BackgroundViewMatrix.TransformationMatrix;
            m.Translation -= Main.BackgroundViewMatrix.ZoomMatrix.Translation
                * new Vector3(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
            return m;
        }

        /// <summary>背景窗口内的真实相机位置(原版在进 DrawBG 前给 screenPosition 加了背景缩放平移)。</summary>
        public static Vector2 RealScreenPosition => Main.screenPosition - Main.BackgroundViewMatrix.Translation;

        /// <summary>调用方空间的全屏矩形:预除后的 screenWidth/Height 配合调用方批次矩阵恰好铺满。</summary>
        public static Rectangle CallerFullscreen => new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        /// <summary>原始像素空间的全屏矩形(配无矩阵批次的全屏 shader pass 用)。</summary>
        public static Rectangle ViewportFullscreen {
            get {
                Viewport vp = Main.instance.GraphicsDevice.Viewport;
                return new Rectangle(0, 0, vp.Width, vp.Height);
            }
        }

        /// <summary>End 后按指定混合/采样在调用方(背景矩阵)空间重开批次。</summary>
        public static void BeginCallerSpace(SpriteBatch sb, BlendState blend, SamplerState sampler, SpriteSortMode sort = SpriteSortMode.Deferred) {
            sb.End();
            sb.Begin(sort, blend, sampler, DepthStencilState.None, Main.Rasterizer, null, BackgroundMatrix());
        }

        /// <summary>
        /// End 后在原始像素空间(无矩阵)重开批次,给全屏 shader pass 用。
        /// 传 effect 时批次自带该效果(Immediate 下 Begin 即 Apply);参数若在 Begin 之后才喂,画之前要再 Passes[0].Apply() 一次。
        /// </summary>
        public static void BeginRawScreen(SpriteBatch sb, BlendState blend, SamplerState sampler, SpriteSortMode sort = SpriteSortMode.Immediate, Effect effect = null) {
            sb.End();
            sb.Begin(sort, blend, sampler, DepthStencilState.None, RasterizerState.CullNone, effect);
        }

        /// <summary>不 End、直接按调用方参数开批次(配合已手动 End 过的场合,如图元渲染之后)。</summary>
        public static void OpenCallerBatch(SpriteBatch sb) {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, BackgroundMatrix());
        }

        /// <summary>按 Main.DoDraw 开 DrawBG 批次的参数还原调用方批次;自开批次画完后必须调用。</summary>
        public static void RestoreCallerBatch(SpriteBatch sb) {
            sb.End();
            OpenCallerBatch(sb);
        }
    }

    /// <summary>
    /// DrawFront 跨 0 切片,DrawFar 画在最远切片,每帧最多各跑一次,自己开的批次用 RestoreCallerBatch 收尾
    /// 最远切片会被原版山峦树影压住,整片换天幕选跨 0;状态推进只放 UpdatePayload
    /// </summary>
    public abstract class CESkyBase : CustomSky
    {
        protected bool skyActive;
        protected float opacity;
        private int lastFarStamp = -1;
        private int lastFrontStamp = -1;

        /// <summary>淡入步长(每 tick)。</summary>
        protected virtual float FadeInStep => 0.02f;

        /// <summary>淡出步长(每 tick),默认与淡入一致。</summary>
        protected virtual float FadeOutStep => FadeInStep;

        /// <summary>Update 每 tick 重算,淡出期 ManageSpecialBiomeVisuals 不再回调 Activate</summary>
        protected abstract bool KeepActive();

        public override void Activate(Vector2 position, params object[] args) => skyActive = true;

        public override void Deactivate(params object[] args) => skyActive = false;

        public override void Reset() {
            skyActive = false;
            opacity = 0f;
            OnReset();
        }

        /// <summary>Reset 时清理载荷状态(粒子列表等)。</summary>
        protected virtual void OnReset() { }

        //淡出尾巴必须算 Active:SkyManager 只更新/绘制 IsActive 的天空,提前返回 false 会冻住渐隐
        public override bool IsActive() => skyActive || opacity > 0f;

        public sealed override void Update(GameTime gameTime) {
            skyActive = !Main.gameMenu && KeepActive();

            if (skyActive && opacity < 1f)
                opacity = Math.Min(1f, opacity + FadeInStep);
            else if (!skyActive && opacity > 0f)
                opacity = Math.Max(0f, opacity - FadeOutStep);

            Opacity = opacity;
            UpdatePayload(gameTime);
        }

        /// <summary>每 tick 的载荷状态推进(在 opacity 步进之后调用)。</summary>
        protected virtual void UpdatePayload(GameTime gameTime) { }

        public sealed override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth) {
            if (opacity <= 0.004f)
                return;
            //捕捉/全屏地图路径的批次是裸 Begin(),背景矩阵还原公式在那里不成立,直接不画
            if (Main.mapFullscreen || CaptureManager.Instance.IsCapturing)
                return;

            //整帧一次 DrawToDepth 都没发生:玩家关掉背景、镜头沉到地表以下、重混/醉酒世界都会走到这里。
            //此时 DrawRemainingDepth 退化成唯一一次全区间回调,跨 0 切片根本不存在,前景载荷得并进来一起画
            bool wholeRange = minDepth <= float.MinValue && maxDepth >= float.MaxValue;

            //最远切片:ResetDepthTracker 后的首个 DrawToDepth,maxDepth 为 float.MaxValue
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue && lastFarStamp != CESkyFrameStamp.Current) {
                lastFarStamp = CESkyFrameStamp.Current;
                DrawFar(spriteBatch);
            }

            //跨 0 切片:DrawRemainingDepth 的 (float.MinValue, ≥0) 调用
            bool frontSlice = minDepth < 0f && maxDepth >= 0f && maxDepth < float.MaxValue;
            if ((frontSlice || wholeRange) && lastFrontStamp != CESkyFrameStamp.Current) {
                lastFrontStamp = CESkyFrameStamp.Current;
                DrawFront(spriteBatch);
            }
        }

        /// <summary>最远切片载荷:画在一切视差背景层后面,会被原版山峦树影压住,只给刻意要躲在背景后的东西用。</summary>
        protected virtual void DrawFar(SpriteBatch spriteBatch) { }

        /// <summary>跨 0 切片;整帧没切片时基座并进那一次全区间回调</summary>
        protected virtual void DrawFront(SpriteBatch spriteBatch) { }
    }
}
