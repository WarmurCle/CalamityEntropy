using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static CalamityEntropy.CalamityEntropy;

namespace CalamityEntropy.Core.Graphics.Screen
{
    /// <summary>
    /// Prepare 返回时持有活跃批次,调用方画完必须自行 End 再 Apply
    /// RT 不可用或关掉绚丽时只留批次管理,内容照画,没有像素化
    /// </summary>
    internal static class CEPixelScreen
    {
        /// <summary>无论门控如何都会开一个批次</summary>
        public static void PreparePixelShader(GraphicsDevice graphicsDevice) {
            if (CEScreenPipeline.PixelPassActive) {
                CEScreenPipeline.CaptureScreenTo0(graphicsDevice);
                graphicsDevice.SetRenderTarget(CEScreenPipeline.Screen2);   //IPixelPassPRT 画进这层,ApplyPixelShader 再过 Pixel shader
                graphicsDevice.Clear(Color.Transparent);
            }
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        /// <summary>进入和返回都不持有批次</summary>
        /// <param name="dye">非 0 时用该染料物品的护甲着色器再过一遍</param>
        /// <param name="dyeEnt">历史参数,当前实现不读取,保留以免改动调用点</param>
        /// <param name="GameZoom">像素尺寸按原始屏幕算而不是除以游戏缩放</param>
        /// <param name="state">最终叠回主屏时用的混合状态,默认 AlphaBlend</param>
        public static void ApplyPixelShader(GraphicsDevice graphicsDevice, int dye = 0, Entity dyeEnt = null, bool GameZoom = false, BlendState state = null) {
            if (!CEScreenPipeline.PixelPassActive) {
                return;
            }

            state ??= BlendState.AlphaBlend;
            graphicsDevice.SetRenderTarget(CEScreenPipeline.Screen1);
            graphicsDevice.Clear(Color.Transparent);
            Effect shader = EffectLoader.PixelShader;
            shader.CurrentTechnique = shader.Techniques["Technique1"];
            shader.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2() / Main.GameViewMatrix.Zoom);
            if (GameZoom)
                shader.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2());
            shader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader);
            Main.spriteBatch.Draw(CEScreenPipeline.Screen2, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(CEScreenPipeline.Screen0, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            Matrix m = Main.GameViewMatrix.ZoomMatrix;
            if (GameZoom)
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, state, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, m);
            else
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, state, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            if (dye != 0) {
                GameShaders.Armor.GetShaderFromItemId(dye).Apply(null, new(CEScreenPipeline.Screen1, Vector2.Zero, Color.White));
            }
            Main.spriteBatch.Draw(CEScreenPipeline.Screen1, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        /// <summary>四类像素化弹幕、三桶 IPixelPassPRT、以及 IAdditivePRT,进入时持有批次,返回时不持有</summary>
        public static void DrawPixelPassContents() {
            int cruiserEnergyBallType = ModContent.ProjectileType<CruiserEnergyBall>();
            int runeTorrentType = ModContent.ProjectileType<RuneTorrent>();
            int runeTorrentRangerType = ModContent.ProjectileType<RuneTorrentRanger>();
            int prophetVoidSpikeType = ModContent.ProjectileType<ProphetVoidSpike>();

            foreach (Projectile proj in Main.ActiveProjectiles) {
                if (proj.type == cruiserEnergyBallType && proj.ModProjectile is CruiserEnergyBall ceb) {
                    ceb.Draw();
                }
                else if (proj.type == runeTorrentType && proj.ModProjectile is RuneTorrent rt) {
                    rt.Draw();
                }
                else if (proj.type == runeTorrentRangerType && proj.ModProjectile is RuneTorrentRanger rt_) {
                    rt_.Draw();
                }
                else if (proj.type == prophetVoidSpikeType && proj.ModProjectile is ProphetVoidSpike vs) {
                    vs.Draw();
                }
            }

            DrawPixelPassPRT();
            Main.spriteBatch.End();

            //IAdditivePRT 自己管绘制,PRT 分桶对不上,攒一批单独开 Additive 画
            List<IAdditivePRT> prtAdditives = new List<IAdditivePRT>();
            foreach (var prt in PRTLoader.PRT_InGame_World_Inds) {
                if (!prt.active || prt.Mod != Instance) {
                    continue;
                }
                if (prt is IAdditivePRT additivePRT) {
                    prtAdditives.Add(additivePRT);
                }
            }
            if (prtAdditives.Count > 0) {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp
                    , DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var iaddDrawin in prtAdditives) {
                    iaddDrawin.Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
        }

        /// <summary>
        /// IPixelPassPRT 画进 Screen2 RT，之后 <see cref="ApplyPixelShader"/> 过 Pixel shader
        /// 三桶顺序 AlphaBlend → NonPremultiplied → Additive，对齐旧 PixelParticle 绘制序
        /// </summary>
        private static void DrawPixelPassPRT() {
            List<IPixelPassPRT> alphaBlendDraw = new();
            List<IPixelPassPRT> nonPremultipliedDraw = new();
            List<IPixelPassPRT> additiveDraw = new();

            foreach (var prt in PRTLoader.PRT_InGame_World_Inds) {
                if (!prt.active || prt.Mod != Instance) {
                    continue;
                }
                if (prt is not IPixelPassPRT { PixelPass: true } pixelPRT) {
                    continue;
                }

                switch (prt.PRTDrawMode) {
                    case PRTDrawModeEnum.AdditiveBlend:
                        additiveDraw.Add(pixelPRT);
                        break;
                    case PRTDrawModeEnum.AlphaBlend:
                        alphaBlendDraw.Add(pixelPRT);
                        break;
                    default:
                        nonPremultipliedDraw.Add(pixelPRT);   //非 Additive 也非 AlphaBlend 的粒子全进这桶,沿用旧 EParticle 的三分支
                        break;
                }
            }

            if (alphaBlendDraw.Count == 0 && nonPremultipliedDraw.Count == 0 && additiveDraw.Count == 0) {
                return;
            }

            //PreparePixelShader开的批次得先End,再按桶重开画像素粒子
            Main.spriteBatch.End();
            if (alphaBlendDraw.Count > 0) {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var p in alphaBlendDraw) {
                    p.DrawPixelPass(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (nonPremultipliedDraw.Count > 0) {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var p in nonPremultipliedDraw) {
                    p.DrawPixelPass(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (additiveDraw.Count > 0) {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var p in additiveDraw) {
                    p.DrawPixelPass(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            //三桶画完接回NonPremultiplied,后面NPC绘制续上PreparePixelShader开的批次
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
