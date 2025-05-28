using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using CalamityEntropy.Content.Particles;
using Terraria.ModLoader;
using CalamityEntropy.Content.Items.Weapons.Nemesis;

namespace CalamityEntropy.Core
{
    public class EffectLoaderAlt : ICELoader
    {
        internal static EffectLoaderAlt Instance;
        public static Effect PowerSFShader;
        public static Effect KnifeRendering;
        public static Effect StarsTrail;
        public static Effect RTShader;
        public const string AssetPath = "CalamityEntropy/Assets/";
        public const string AssetPath2 = "Assets/";
        internal static RenderTarget2D screen;
        internal static float twistStrength = 0f;
        
        void ICELoader.LoadAsset() {
            AssetRepository assets = CalamityEntropy.Instance.Assets;
            LoadRegularShaders(assets);
        }

        public static void LoadRegularShaders(AssetRepository assets) {
            Asset<Effect> getEffect(string key) => assets.Request<Effect>(AssetPath2 + key, AssetRequestMode.ImmediateLoad);
            void loadFiltersEffect(string filtersKey, string filename, string passname, out Effect effect) {
                Asset<Effect> asset = getEffect(filename);
                Filters.Scene[filtersKey] = new Filter(new(asset, passname), EffectPriority.VeryHigh);
                effect = asset.Value;
            }

            loadFiltersEffect("CalamityEntropy:powerSFShader", "PowerSFShader", "PowerSFShaderPass", out PowerSFShader);
            loadFiltersEffect("CalamityEntropy:KnifeRendering", "KnifeRendering", "KnifeRenderingPass", out KnifeRendering);
            loadFiltersEffect("CalamityEntropy:StarsTrail", "StarsTrail", "StarsTrailPass", out StarsTrail);
            loadFiltersEffect("CalamityEntropy:RTShader", "RTShader", "Tentacle", out RTShader);
        }

        void ICELoader.LoadData() {
            Instance = this;
            On_FilterManager.EndCapture += new On_FilterManager.hook_EndCapture(FilterManager_EndCapture);
            Main.OnResolutionChanged += Main_OnResolutionChanged;
        }

        void ICELoader.UnLoadData() {
            On_FilterManager.EndCapture -= new On_FilterManager.hook_EndCapture(FilterManager_EndCapture);
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            PowerSFShader = null;
            KnifeRendering = null;
        }

        private void Main_OnResolutionChanged(Vector2 obj) {
            screen?.Dispose();
            screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }

        private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self
            , RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor) {
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

            if (screen == null) {
                screen = new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight);
            }

            if (!Main.gameMenu) {
                if (HasWarpEffect(out List<IDrawWarp> warpSets, out List<IDrawWarp> warpSetsNoBlueshift)) {
                    if (warpSets.Count > 0) {
                        //绘制屏幕
                        graphicsDevice.SetRenderTarget(screen);
                        graphicsDevice.Clear(Color.Transparent);
                        Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
                        Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                        Main.spriteBatch.End();
                        //绘制需要绘制的内容
                        graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                        graphicsDevice.Clear(Color.Transparent);

                        Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None
                            , RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                        foreach (IDrawWarp p in warpSets) { p.Warp(); }
                        Main.spriteBatch.End();

                        //应用扭曲
                        graphicsDevice.SetRenderTarget(Main.screenTarget);
                        graphicsDevice.Clear(Color.Transparent);
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                        //如果想热加载，最好这样获取值
                        Effect effect = CalamityEntropy.Instance.Assets.Request<Effect>(AssetPath2 + "WarpShader").Value;//EffectsRegistry.WarpShader;
                        effect.Parameters["tex0"].SetValue(Main.screenTargetSwap);
                        effect.Parameters["noBlueshift"].SetValue(false);//这个部分的绘制需要使用蓝移效果
                        effect.Parameters["i"].SetValue(0.02f);
                        effect.CurrentTechnique.Passes[0].Apply();
                        Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                        Main.spriteBatch.End();

                        Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState
                            , default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                        foreach (IDrawWarp p in warpSets) { if (p.canDraw()) { p.costomDraw(Main.spriteBatch); } }
                        Main.spriteBatch.End();
                    }
                    if (warpSetsNoBlueshift.Count > 0) {
                        //绘制屏幕
                        graphicsDevice.SetRenderTarget(screen);
                        graphicsDevice.Clear(Color.Transparent);
                        Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
                        Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                        Main.spriteBatch.End();
                        //绘制需要绘制的内容
                        graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                        graphicsDevice.Clear(Color.Transparent);

                        Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None
                            , RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                        foreach (IDrawWarp p in warpSetsNoBlueshift) { p.Warp(); }
                        Main.spriteBatch.End();

                        //应用扭曲
                        graphicsDevice.SetRenderTarget(Main.screenTarget);
                        graphicsDevice.Clear(Color.Transparent);
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                        //如果想热加载，最好这样获取值
                        Effect effect = CalamityEntropy.Instance.Assets.Request<Effect>(AssetPath2 + "WarpShader").Value;//EffectsRegistry.WarpShader;
                        effect.Parameters["tex0"].SetValue(Main.screenTargetSwap);
                        effect.Parameters["noBlueshift"].SetValue(true);//这个部分的绘制不需要使用蓝移效果
                        effect.Parameters["i"].SetValue(0.02f);
                        effect.CurrentTechnique.Passes[0].Apply();
                        Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                        Main.spriteBatch.End();

                        Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState
                            , default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                        foreach (IDrawWarp p in warpSetsNoBlueshift) { if (p.canDraw()) { p.costomDraw(Main.spriteBatch); } }
                        Main.spriteBatch.End();
                    }
                }

                if (HasPwoerEffect()) {
                    graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                    graphicsDevice.Clear(Color.Transparent);//用透明清除
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                    graphicsDevice.SetRenderTarget(screen);
                    graphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap
                        , DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
                    DrawPwoerEffect(Main.spriteBatch);
                    Main.spriteBatch.End();
                    graphicsDevice.SetRenderTarget(Main.screenTarget);
                    graphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    PowerSFShader.Parameters["tex0"].SetValue(screen);
                    PowerSFShader.Parameters["i"].SetValue(twistStrength);
                    PowerSFShader.CurrentTechnique.Passes[0].Apply();
                    Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                }

                #region RT粒子特效
                if (PRT_RTSpark.HasSet(out List<BasePRT> prts)) {
                    graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                    graphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();

                    graphicsDevice.SetRenderTarget(screen);
                    graphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    PRT_RTSpark.DrawAll(Main.spriteBatch, prts);
                    Main.spriteBatch.End();

                    graphicsDevice.SetRenderTarget(Main.screenTarget);
                    graphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    graphicsDevice.Textures[1] = CEUtils.GetT2DAsset("CalamityEntropy/Assets/StarrySky").Value;
                    RTShader.CurrentTechnique.Passes[0].Apply();
                    RTShader.Parameters["m"].SetValue(0.08f);
                    RTShader.Parameters["n"].SetValue(0.01f);
                    RTShader.Parameters["OffsetX"].SetValue((float)((Main.GlobalTimeWrappedHourly) * 0.11f));
                    Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                }
                #endregion
            }

            orig.Invoke(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        private void DrawPwoerEffect(SpriteBatch sb) {
            int targetProjType = ModContent.ProjectileType<EndSkillOrbOnSpan>();
            foreach (Projectile proj in Main.projectile) {
                Vector2 offsetRotV = proj.rotation.ToRotationVector2() * 1500;
                if (proj.type == targetProjType && proj.active) {
                    Texture2D texture = CEUtils.GetT2DValue(AssetPath + "placeholder2");
                    int length = (int)(Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight) * 4f);
                    sb.Draw(texture,
                        proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition + offsetRotV,
                        new(0, 0, 1, 1),
                        new(CEUtils.GetCorrectRadian(proj.rotation), proj.ai[0], 0f, 0.2f),
                        proj.rotation,
                        Vector2.Zero,
                        length, SpriteEffects.None, 0);
                    sb.Draw(texture,
                        proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition + offsetRotV,
                        new(0, 0, 1, 1),
                        new(CEUtils.GetCorrectRadian(proj.rotation) + Math.Sign(proj.rotation + 0.001f) * 0.5f, proj.ai[0], 0f, 0.2f),
                        proj.rotation,
                        new(0, 1),
                        length,
                        SpriteEffects.None, 0);
                    twistStrength = 0.055f * proj.localAI[0];
                }
            }
        }

        private bool HasPwoerEffect() {
            return true;
        }

        private bool HasWarpEffect(out List<IDrawWarp> warpSets, out List<IDrawWarp> warpSetsNoBlueshift) {
            warpSets = [];
            warpSetsNoBlueshift = [];
            foreach (Projectile p in Main.projectile) {
                if (!p.active) {
                    continue;
                }
                if (p.ModProjectile is IDrawWarp drawWarp) {
                    if (drawWarp.noBlueshift()) {
                        warpSetsNoBlueshift.Add(drawWarp);
                    }
                    else {
                        warpSets.Add(drawWarp);
                    }
                }
            }
            return warpSets.Count > 0 || warpSetsNoBlueshift.Count > 0;
        }

    }
}
