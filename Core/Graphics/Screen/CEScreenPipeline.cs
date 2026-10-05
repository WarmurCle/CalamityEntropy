using CalamityEntropy.Common;
using InnoVault.RenderHandles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Core.Graphics.Screen
{
    /// <summary>
    /// 复古和迷幻下 DoDraw 不捕获主画面,drawToScreen 会释放 screenTarget
    /// EndCaptureDraw 整条不触发,不依赖 RT 的叠加改由 DrawBeforeInfernoRings 补画
    /// </summary>
    internal class CEScreenPipeline : RenderHandle
    {
        public const int MaxScreenSlot = 4;

        public override int ScreenSlot => MaxScreenSlot;

        public static CEScreenPipeline This { get; private set; }

        public static RenderTarget2D Screen0 => Slot(0);
        public static RenderTarget2D Screen1 => Slot(1);
        public static RenderTarget2D Screen2 => Slot(2);
        public static RenderTarget2D Screen3 => Slot(3);

        /// <summary>别直接读,用 EndCaptureDraw 的 screenSwap</summary>
        public static RenderTarget2D StaticScreenSwap => RenderHandleLoader.ScreenSwap;

        /// <summary>
        /// 对应 DoDraw 的 !(drawToScreen || netMode == 2 || worldGen) &amp;&amp; !mapFullscreen &amp;&amp; Lighting.NotRetro
        /// drawToScreen 时 screenTarget 已释放,再判一次未释放
        /// </summary>
        public static bool RTPipelineAvailable => !Main.gameMenu && !Main.mapFullscreen
            && Lighting.NotRetro && !Main.drawToScreen
            && Main.screenTarget != null && !Main.screenTarget.IsDisposed
            && Screen0 != null;

        /// <summary>自己不画、等管线代画的点读这个,不读配置项</summary>
        public static bool PixelPassActive => RTPipelineAvailable && Config.Instance.EnablePixelEffect;

        public override void Load() => This = this;

        private static RenderTarget2D Slot(int index) {
            RenderTarget2D[] targets = This?.ScreenTargets;
            if (targets == null || index >= targets.Length) {
                return null;
            }
            RenderTarget2D target = targets[index];
            return target != null && !target.IsDisposed ? target : null;
        }

        /// <summary>返回后 dest 保持绑定</summary>
        public static void CaptureScreenTo(GraphicsDevice graphicsDevice, RenderTarget2D dest) {
            graphicsDevice.SetRenderTarget(dest);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        public static void CaptureScreenTo0(GraphicsDevice graphicsDevice) => CaptureScreenTo(graphicsDevice, Screen0);

        public override void EndCaptureDraw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D screenSwap) {
            if (!PixelPassActive) {
                return;
            }

            //顺序即历史顺序,任何一段的 RT 绑定都是下一段的前置条件,不要重排
            CaptureScreenTo0(graphicsDevice);

            //虚空:遮罩攒进交换缓冲,再由 cvoid 家族合成回主屏
            CEVoidScreen.DrawVoidMasks(graphicsDevice);
            CEVoidScreen.DrawVoidParticles(graphicsDevice);
            CEVoidScreen.ApplyVoidBackground(graphicsDevice);
            CEVoidScreen.DrawNonPixVoid(graphicsDevice);

            //深渊与血色:各自一套遮罩 + 全屏着色
            CEAbyssScreen.DrawAbyssal(graphicsDevice);
            CEAbyssScreen.DrawBlood(graphicsDevice);

            CEVoidScreen.DrawVoidStarWake(graphicsDevice);

            //像素通道:实体与 IPixelPassPRT 画进 Screen2,再整块过 Pixel shader
            CEPixelScreen.PreparePixelShader(graphicsDevice);
            CEPixelScreen.DrawPixelPassContents();
            CEPixelScreen.ApplyPixelShader(graphicsDevice);

            //无 shader 的实体叠加。复古下这一段由 DrawBeforeInfernoRings 接管
            CEEntityOverlay.DrawScreenOverlay(graphicsDevice);

            //扭曲:刀光与区域碎裂
            CEWarpScreen.DrawSlashWarp(graphicsDevice);
            CEWarpScreen.DrawFragWarp(graphicsDevice);

            //演出层:闪光泛光 → 晚于全部扭曲的实体重绘 → 切屏 → 黑幕
            CECinematicScreen.DrawFlashBloom(graphicsDevice);
            CEEntityOverlay.DrawLateOverlay();
            CECinematicScreen.DrawCutScreen(graphicsDevice);
            CECinematicScreen.DrawBlackMask();
        }

        /// <summary>只补不依赖 RT 的,不做近似,进入时批次活跃,返回前必须重新开批</summary>
        public override void DrawBeforeInfernoRings(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D screenSwap) {
            //RT 管线会跑时实体叠加在 EndCaptureDraw,这里不能再画
            if (RTPipelineAvailable) {
                return;
            }
            //玩家关掉绚丽特效时整条管线本就不画,兜底也不画(既有设计)
            if (!Config.Instance.EnablePixelEffect) {
                return;
            }
            //这两种情况下原本也不会有世界画面可叠,别把实体糊到全屏地图上
            if (Main.gameMenu || Main.mapFullscreen) {
                return;
            }

            spriteBatch.End();

            CEEntityOverlay.DrawScreenOverlayDirect();
            CEEntityOverlay.DrawLateOverlay();
            CECinematicScreen.DrawBlackMask();

            //还原 Deferred/AlphaBlend/Main.Transform
            //本阶段在 DrawInfernoRingsHook 的 orig 链内,DrawAcropolisMechs 收尾重开的也是这一套
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState
                , DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
