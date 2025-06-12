using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Content.NPCs.Prophet;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.Content.Projectiles.Chainsaw;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Projectiles.Pets.Abyss;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityEntropy.Utilities;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static CalamityEntropy.CalamityEntropy;

namespace CalamityEntropy.Common
{
    [VaultLoaden("CalamityEntropy/Assets/Effects/")]
    internal class EffectLoader
    {
        [VaultLoaden("CalamityEntropy/Assets/Extra/cvmask")]
        private static Asset<Texture2D> cvmask;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_blue_base")]
        private static Asset<Texture2D> planetarium_blue_base;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_starfield_1")]
        private static Asset<Texture2D> planetarium_starfield_1;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_starfield_2")]
        private static Asset<Texture2D> planetarium_starfield_2;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_starfield_3")]
        private static Asset<Texture2D> planetarium_starfield_3;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_starfield_4")]
        private static Asset<Texture2D> planetarium_starfield_4;
        [VaultLoaden("CalamityEntropy/Assets/Extra/planetarium_starfield_5")]
        private static Asset<Texture2D> planetarium_starfield_5;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/CruiserSlash")]
        private static Asset<Texture2D> cruiserSlash;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/Cruiser/CruiserBlackholeBullet")]
        private static Asset<Texture2D> cruiserBlackholeBullet;
        [VaultLoaden("CalamityEntropy/Assets/Extra/cruiserSpace2")]
        private static Asset<Texture2D> cruiserSpace2;
        [VaultLoaden("CalamityEntropy/Assets/Extra/ksc1")]
        private static Asset<Texture2D> ksc1;
        [VaultLoaden("CalamityEntropy/Assets/Extra/kscc")]
        private static Asset<Texture2D> kscc;
        [VaultLoaden("CalamityEntropy/Assets/Extra/white")]
        private static Asset<Texture2D> white;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/Cruiser/VoidStar")]
        private static Asset<Texture2D> voidStar;
        public static Asset<Effect> PowerSFShader;
        public static Asset<Effect> KnifeRendering;
        public static Asset<Effect> StarsTrail;
        public static Asset<Effect> RTShader;
        public static Asset<Effect> WarpShader;
        internal static float twistStrength = 0f;
        public const string AssetPath = "CalamityEntropy/Assets/";
        public const string AssetPath2 = "Assets/";
        public static void Load()
        {
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            On_FilterManager.EndCapture += CE_EffectHandler;
        }

        public static void UnLoad()
        {
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            On_FilterManager.EndCapture -= CE_EffectHandler;
        }

        // 确保旧的RenderTarget2D对象被正确释放
        private static void DisposeScreen()
        {
            screen?.Dispose();
            screen = null;
            screen2?.Dispose();
            screen2 = null;
            screen3?.Dispose();
            screen3 = null;
        }

        //在改变屏幕时更新这些字段的值，而不是每帧不断的释放更新浪费性能
        private static void Main_OnResolutionChanged(Vector2 obj)
        {
            DisposeScreen();
            screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            screen2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            screen3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }

        //首先纹理在使用前尽量缓存为静态的，Request函数并非性能的最佳选择，尤其是在每帧调用甚至循环调用中的高频访问
        //这不是最佳的选择，要我说EndCapture就应该去死，该他妈的沉没在历史的粪坑中。万物都有自己的道理唯独它没有
        //如果有机会，我会把Red绑上十字架然后用白磷火刑慢慢的把他净化，神皇会赞许我的行为的，因为那帮家伙全他妈的是异端邪祟
        //----HoCha113 2025-5-6
        private static void CE_EffectHandler(On_FilterManager.orig_EndCapture orig, FilterManager self,
        RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (Main.gameMenu) {
                //调用原始方法
                orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
                return;
            }

            //获取渲染资源
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            //初始化
            InitializeEffectHandler();
            
            //初始化那些他妈的屏幕字段
            EnsureRenderTargets(graphicsDevice);

            //绘制初始屏幕
            DrawInitialScreen(graphicsDevice);

            //绘制 NPC 和投射物
            DrawNPCsAndProjectiles(graphicsDevice);

            //应用像素着色器
            ApplyPixelShader(graphicsDevice);

            //绘制投射物特效
            DrawProjectileEffects(graphicsDevice);

            //绘制粒子效果
            DrawParticleEffects(graphicsDevice);

            //应用背景着色器
            ApplyBackgroundShader(graphicsDevice);

            //深渊类型Shader
            DrawAbyssalEffect(graphicsDevice);

            //我也不知道叫啥的特效 虚寂之翼用了
            DrawRandomEffect(graphicsDevice);

            //绘制玩家和投射物特效
            DrawPlayerAndProjectileEffects(graphicsDevice);

            //绘制切片效果
            DrawSlashEffects(graphicsDevice);

            //应用最终着色器
            ApplyFinalShader(graphicsDevice);

            //处理屏幕切割效果
            HandleCutScreenEffect(graphicsDevice);

            //绘制黑色遮罩
            DrawBlackMask();

            //调用原始方法
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        private static void DrawRandomEffect(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();


            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (!p.active)
                {
                    continue;
                }
                if (p.ModProjectile != null && p.type == ModContent.ProjectileType<WohShot>() && false)
                {
                    WohShot mp = (WohShot)p.ModProjectile;
                    if (mp.odp.Count > 1)
                    {
                        List<ColoredVertex> ve = new List<ColoredVertex>();
                        Color b = new Color(75, 125, 255);

                        float a = 0;
                        float lr = 0;
                        for (int i = 1; i < mp.odp.Count; i++)
                        {
                            a += 1f / mp.odp.Count;

                            ve.Add(new ColoredVertex(vLToCenter(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18, Main.GameViewMatrix.Zoom.X),
                                  new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                                  b * a));
                            ve.Add(new ColoredVertex(vLToCenter(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18, Main.GameViewMatrix.Zoom.X),
                                  new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                                  b * a));
                            lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                        }
                        a = 1;
                        GraphicsDevice gd = Main.graphics.GraphicsDevice;
                        if (ve.Count >= 3)
                        {
                            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                            gd.Textures[0] = tx;
                            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                        }


                    }
                }
                if (p.ModProjectile != null && p.ModProjectile is AbyssalLaser al)
                {
                    al.drawLaser();
                }
                if (p.ModProjectile != null && p.ModProjectile is WohLaser)
                {
                    float alp = p.ai[1];
                    float w = p.scale;
                    if (p.ai[0] == 0)
                    {
                        w = 0f;
                    }
                    if (p.ai[0] == 1)
                    {
                        w = 0.1f;
                    }
                    if (p.ai[0] == 2)
                    {
                        w = 0.16f;
                    }
                    if (p.ai[0] == 3)
                    {
                        w = 0.3f;
                    }
                    if (p.ai[0] == 4)
                    {
                        w = 0.5f;
                    }
                    if (p.ai[0] == 5)
                    {
                        w = 0.85f;
                    }
                    Vector2 opos = p.Center;
                    Texture2D tx = CEUtils.getExtraTex("wohlaser");
                    int drawCount = (int)(2400f * p.scale / tx.Width) + 1;
                    for (int i = 0; i < drawCount; i++)
                    {
                        Main.spriteBatch.Draw(tx, opos - Main.screenPosition, null, new Color(55, 100, 255) * alp, p.velocity.ToRotation(), new Vector2(0, tx.Height / 2), new Vector2(1, w * 1f), SpriteEffects.None, 0);
                        opos += p.velocity.SafeNormalize(Vector2.One) * tx.Width;
                    }
                }
                if (p.ModProjectile != null && p.ModProjectile is VoidStar)
                {
                    if (p.ai[0] >= 60 || p.ai[2] == 0)
                    {
                        VoidStar mp = (VoidStar)p.ModProjectile;
                        mp.odp.Add(p.Center);
                        if (mp.odp.Count > 2)
                        {
                            float size = 10;
                            float sizej = size / mp.odp.Count;
                            Color cl = new Color(200, 235, 255);
                            for (int i = mp.odp.Count - 1; i >= 1; i--)
                            {
                                CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * ((255 - p.alpha) / 255f), size * 0.7f);
                                size -= sizej;
                            }
                        }
                        mp.odp.RemoveAt(mp.odp.Count - 1);
                    }

                }
                if (p.ModProjectile != null && p.ModProjectile is VoidStarF)
                {
                    VoidStarF mp = (VoidStarF)p.ModProjectile;
                    mp.odp.Add(p.Center);
                    if (mp.odp.Count > 2)
                    {
                        float size = 10;
                        float sizej = size / mp.odp.Count;
                        Color cl = new Color(200, 235, 255);
                        if (p.ai[2] > 0)
                        {
                            cl = new Color(255, 160, 160);
                        }
                        for (int i = mp.odp.Count - 1; i >= 1; i--)
                        {
                            CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * ((255 - p.alpha) / 255f), size * 0.7f);
                            size -= sizej;
                        }
                    }
                    mp.odp.RemoveAt(mp.odp.Count - 1);

                }

                if (p.ModProjectile is LightWisperFlame lwf)
                {
                    lwf.draw();
                }
            }
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Player p in Main.ActivePlayers)
            {
                {
                    if (!p.dead && p.Entropy().MagiShield > 0 && p.Entropy().visualMagiShield)
                    {
                        Texture2D shieldTexture = CEUtils.getExtraTex("shield");
                        Main.spriteBatch.Draw(shieldTexture, p.Center - Main.screenPosition, null, new Color(186, 120, 255), 0, shieldTexture.Size() / 2, 0.47f, SpriteEffects.None, 0);

                    }
                }
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.active)
                {
                    if (p.ModProjectile != null && p.ModProjectile is MoonlightShieldBreak)
                    {
                        Texture2D shieldTexture = CEUtils.getExtraTex("shield");
                        Main.spriteBatch.Draw(shieldTexture, p.Center - Main.screenPosition, null, new Color(186, 120, 255) * p.ai[2], 0, shieldTexture.Size() / 2, 0.47f * (1 + p.ai[1]), SpriteEffects.None, 0);

                    }
                    if (p.ModProjectile != null && p.ModProjectile is CruiserShadow aw)
                    {
                        if (aw.alphaPor > 0)
                        {
                            float s = 0;
                            float sj = 1;
                            for (int i = 0; i <= 30; i++)
                            {
                                aw.DrawPortal(aw.spawnPos, new Color(50, 35, 240) * aw.alphaPor, aw.spawnRot, 270 * s, 0.3f, i * 3f);
                                s = s + (sj - s) * 0.05f;
                            }

                        }
                    }
                }
            }
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.active && n.ModNPC is AbyssalWraith aw)
                {
                    if (aw.portalAlpha > 0)
                    {
                        float s = 0;
                        float sj = 1;
                        for (int i = 0; i <= 30; i++)
                        {
                            aw.DrawPortal(aw.portalPos + new Vector2(0, 220 - i * 2.2f), new Color(50, 35, 240) * aw.portalAlpha, 270 * s, 0.3f, i * 3f);
                            s = s + (sj - s) * 0.05f;
                        }

                        s = 0;
                        sj = 1;
                        for (int i = 0; i <= 30; i++)
                        {
                            aw.DrawPortal(aw.portalTarget + new Vector2(0, 220 - i * 2.2f), new Color(50, 35, 240) * aw.portalAlpha, 270 * s, 0.3f, i * 3f);
                            s = s + (sj - s) * 0.05f;
                        }
                    }
                }
            }
            Main.spriteBatch.End();
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
            cve2.CurrentTechnique = cve2.Techniques["Technique1"];
            cve2.CurrentTechnique.Passes[0].Apply();
            cve2.Parameters["tex0"].SetValue(Main.screenTargetSwap);
            cve2.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/VoidBack").Value);
            cve2.Parameters["time"].SetValue(Instance.cvcount / 50f);
            cve2.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(Instance.cvcount * 1.4f, Instance.cvcount * 1.4f)) / new Vector2(1920, 1080));
            Main.spriteBatch.Draw(screen, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.End();
        }


        private static void DrawAbyssalEffect(GraphicsDevice graphicsDevice)
        {
            if (cab == null)
                cab = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/cabyss", AssetRequestMode.ImmediateLoad).Value;

            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();


            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is AbyssalCrack ac)
                {
                    ac.draw();
                }
                if (proj.ModProjectile is AbyssBookmarkCrack ac2)
                {
                    ac2.drawVoid();
                }
                if (proj.ModProjectile is NxCrack nc)
                {
                    nc.drawCrack();
                }
                if (proj.ModProjectile is YstralynProj yst)
                {
                    yst.draw_crack();
                }
            }
            DrawParticleEffectsAlt();

            Main.spriteBatch.End();
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            Main.spriteBatch.Draw(screen, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            cab.CurrentTechnique = cab.Techniques["Technique1"];
            cab.CurrentTechnique.Passes[0].Apply();
            cab.Parameters["clr"].SetValue(new Color(12, 50, 160).ToVector4());
            cab.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/AwSky1", AssetRequestMode.ImmediateLoad).Value);
            cab.Parameters["time"].SetValue(Instance.cvcount / 50f);
            cab.Parameters["scrsize"].SetValue(screen.Size());
            cab.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(Instance.cvcount * 1.4f, Instance.cvcount * 1.4f)) / new Vector2(1920, 1080));
            Main.spriteBatch.Draw(Main.screenTargetSwap, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);

            Main.spriteBatch.End();
        }

        private static void InitializeEffectHandler()
        {
            Instance.screenShakeAmp *= 0.9f;
            CheckProjs.Clear();
            CheckNPCs.Clear();
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                CheckProjs.Add(p);
            }
            foreach (NPC n in Main.ActiveNPCs)
            {
                CheckNPCs.Add(n);
            }
        }

        private static void EnsureRenderTargets(GraphicsDevice graphicsDevice)
        {
            screen ??= new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight);
            screen2 ??= new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight);
            screen3 ??= new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight);
        }

        private static void DrawInitialScreen(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawNPCsAndProjectiles(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen3);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);

            int theProphetType = ModContent.NPCType<TheProphet>();
            int cruiserEnergyBallType = ModContent.ProjectileType<CruiserEnergyBall>();
            int runeTorrentType = ModContent.ProjectileType<RuneTorrent>();
            int runeTorrentRangerType = ModContent.ProjectileType<RuneTorrentRanger>();
            int prophetVoidSpikeType = ModContent.ProjectileType<ProphetVoidSpike>();

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type != theProphetType)
                {
                    continue;
                }

                if (npc.ModNPC is TheProphet tp)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                    NPCLoader.PreDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                    tp.Draw();
                    NPCLoader.PostDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == cruiserEnergyBallType && proj.ModProjectile is CruiserEnergyBall ceb)
                {
                    ceb.Draw();
                }
                else if (proj.type == runeTorrentType && proj.ModProjectile is RuneTorrent rt)
                {
                    rt.Draw();
                }
                else if (proj.type == runeTorrentRangerType && proj.ModProjectile is RuneTorrentRanger rt_)
                {
                    rt_.Draw();
                }
                else if (proj.type == prophetVoidSpikeType && proj.ModProjectile is ProphetVoidSpike vs)
                {
                    vs.Draw();
                }
            }

            EParticle.DrawPixelShaderParticles();
            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void ApplyPixelShader(GraphicsDevice graphicsDevice)
        {
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Pixel", AssetRequestMode.ImmediateLoad).Value;
            shader.CurrentTechnique = shader.Techniques["Technique1"];
            shader.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2() / Main.GameViewMatrix.Zoom);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader);
            Main.spriteBatch.Draw(screen3, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawProjectileEffects(GraphicsDevice graphicsDevice)
        {
            if (screen == null)
            {
                return;
            }

            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            Instance.cvcount += 3;

            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);

            int cruiserSlashType = ModContent.ProjectileType<CruiserSlash>();
            int silenceHookType = ModContent.ProjectileType<SilenceHook>();
            int cruiserBlackholeBulletType = ModContent.ProjectileType<CruiserBlackholeBullet>();
            int voidBulletType = ModContent.ProjectileType<VoidBullet>();
            int voidMonsterType = ModContent.ProjectileType<VoidMonster>();

            foreach (Projectile p in CheckProjs)
            {
                if (p.type == cruiserSlashType)
                {
                    if (p.ModProjectile is CruiserSlash cs && cs.ct > 60)
                    {
                        Main.spriteBatch.Draw(cruiserSlash.Value, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 300, 0).RotatedBy(p.rotation), null, Color.White, p.rotation, new Vector2(cruiserSlash.Value.Width, cruiserSlash.Value.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / cruiserSlash.Value.Width, 1.2f), SpriteEffects.None, 0);
                    }
                }
                else if (p.type == silenceHookType)
                {
                    Vector2 c = ((int)p.ai[1]).ToProj().Center;
                    CEUtils.drawChain(p.Center, c, 20, CEUtils.getExtraTex("VoidChain"));
                }
                else if (p.type == cruiserBlackholeBulletType)
                {
                    Main.spriteBatch.Draw(cruiserBlackholeBullet.Value, p.Center - Main.screenPosition, null, Color.White, p.rotation, new Vector2(cruiserBlackholeBullet.Value.Width, cruiserBlackholeBullet.Value.Height) / 2, p.scale, SpriteEffects.None, 0);
                }
                else if (p.type == voidBulletType)
                {
                    Main.spriteBatch.Draw(cruiserBlackholeBullet.Value, p.Center - Main.screenPosition, null, Color.White, p.rotation, new Vector2(cruiserBlackholeBullet.Value.Width, cruiserBlackholeBullet.Value.Height) / 2, p.scale, SpriteEffects.None, 0);
                }
                else if (p.type == voidMonsterType)
                {
                    if (p.ModProjectile is VoidMonster vmnpc)
                    {
                        vmnpc.draw();
                    }
                }
            }

            int cruiserHeadType = ModContent.NPCType<CruiserHead>();

            foreach (NPC n in CheckNPCs)
            {
                if (n.type != cruiserHeadType)
                {
                    continue;
                }

                if (n.ModNPC is CruiserHead ch && ch.phaseTrans > 120 && n.ai[0] > 1)
                {
                    Vector2 ddp = ch.SpaceCenter;
                    Main.spriteBatch.Draw(cruiserSpace2.Value, ddp - Main.screenPosition, null, Color.White * 0.1f, 0, new Vector2(cruiserSpace2.Value.Width, cruiserSpace2.Value.Height) / 2, ch.maxDistance / 900f * 2 - 0.01f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(cruiserSpace2.Value, ddp - Main.screenPosition, null, Color.White, 0, new Vector2(cruiserSpace2.Value.Width, cruiserSpace2.Value.Height) / 2, ch.maxDistance / 900f * 2, SpriteEffects.None, 0);
                }
            }

            foreach (Particle pt in VoidParticles.particles)
            {
                if (cvmask == null)
                {
                    continue;
                }
                Main.spriteBatch.Draw(cvmask.Value, pt.position - Main.screenPosition, null, Color.White * 0.06f, pt.rotation, cvmask.Value.Size() / 2, (5.4f * pt.alpha) * 0.05f, SpriteEffects.None, 0);
            }

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.ModProjectile != null)
                {
                    if (p.ModProjectile is Pioneer1 p1)
                    {
                        p1.drawVoid();
                    }
                }
            }

            Main.spriteBatch.End();
        }

        private static void DrawParticleEffects(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen2);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);

            foreach (Particle pt in VoidParticles.particles)
            {
                if (pt.shape != 4)
                {
                    continue;
                }
                Texture2D draw = CEUtils.getExtraTex("cvdt");
                Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White, pt.rotation, draw.Size() / 2, 2.2f * pt.alpha, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(screen3);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            kscreen2.CurrentTechnique = kscreen2.Techniques["Technique1"];
            kscreen2.CurrentTechnique.Passes[0].Apply();
            kscreen2.Parameters["tex0"].SetValue(screen2);
            kscreen2.Parameters["tex1"].SetValue(CEUtils.getExtraTex("EternityStreak"));
            kscreen2.Parameters["offset"].SetValue(Main.screenPosition / Main.ScreenSize.ToVector2());
            kscreen2.Parameters["i"].SetValue(0.04f);
            Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawParticleEffectsAlt()
        {
            foreach (Particle pt in AbyssalParticles.particles)
            {
                if (cvmask == null)
                {
                    continue;
                }
                Main.spriteBatch.Draw(cvmask.Value, pt.position - Main.screenPosition, null, Color.White * 0.06f, pt.rotation, cvmask.Value.Size() / 2, (5.4f * pt.alpha) * 0.05f, SpriteEffects.None, 0);
            }
        }

        private static void ApplyBackgroundShader(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
            Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            cve.CurrentTechnique = cve.Techniques["Technique1"];
            cve.CurrentTechnique.Passes[0].Apply();
            cve.Parameters["tex1"].SetValue(planetarium_blue_base.Value);
            cve.Parameters["tex2"].SetValue(planetarium_starfield_1.Value);
            cve.Parameters["tex3"].SetValue(planetarium_starfield_2.Value);
            cve.Parameters["tex4"].SetValue(planetarium_starfield_3.Value);
            cve.Parameters["tex5"].SetValue(planetarium_starfield_4.Value);
            cve.Parameters["tex6"].SetValue(planetarium_starfield_5.Value);
            cve.Parameters["time"].SetValue(Instance.cvcount / 50f);
            cve.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2());
            cve.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(-Instance.cvcount / 6f, Instance.cvcount / 6f)) / Main.ScreenSize.ToVector2());
            Main.spriteBatch.Draw(screen3, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawPlayerAndProjectileEffects(GraphicsDevice graphicsDevice)
        {
            if (screen2 == null)
            {
                return;
            }

            graphicsDevice.SetRenderTarget(screen2);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead && player.Entropy().daPoints.Count > 2)
                {
                    float scj = 1f / player.Entropy().daPoints.Count;
                    float sc = scj;
                    Color color = player.Entropy().VaMoving > 0 ? Color.Blue : Color.Black;
                    for (int i = 1; i < player.Entropy().daPoints.Count; i++)
                    {
                        CEUtils.drawLine(Main.spriteBatch, white.Value, (Vector2)CEUtils.Entropy(player).daPoints[i - 1], (Vector2)CEUtils.Entropy(player).daPoints[i], color * 0.6f, 12 * sc, 0);
                        sc += scj;
                    }
                }
            }

            PixelParticle.drawAll();

            // 获取投射物类型 ID
            int voidBottleThrowType = ModContent.ProjectileType<VoidBottleThrow>();
            int cruiserShadowType = ModContent.ProjectileType<CruiserShadow>();
            int voidWraithType = ModContent.ProjectileType<VoidWraith>();
            int abyssPetType = ModContent.ProjectileType<AbyssPet>();
            int voidPalProjType = ModContent.ProjectileType<VoidPalProj>();
            int shadewindLanceThrowType = ModContent.ProjectileType<ShadewindLanceThrow>();
            int voidStarType = ModContent.ProjectileType<VoidStar>();
            int voidStarFType = ModContent.ProjectileType<VoidStarF>();

            // 遍历投射物，使用类型 ID 判断
            foreach (Projectile p in CheckProjs)
            {
                if (p.ModProjectile == null)
                {
                    continue;
                }

                if (p.type == voidBottleThrowType || p.type == cruiserShadowType)
                {
                    Color color = Color.White;
                    p.ModProjectile.PreDraw(ref color);
                }
                else if (p.type == voidWraithType)
                {
                    if (p.ModProjectile is VoidWraith vw)
                    {
                        vw.draw();
                    }
                }
                else if (p.type == abyssPetType || p.type == voidPalProjType)
                {
                    Color color = Color.White;
                    p.ModProjectile.PreDraw(ref color);
                }
                else if (p.type == shadewindLanceThrowType)
                {
                    if (p.ModProjectile is ShadewindLanceThrow sp)
                    {
                        sp.draw();
                    }
                }
                else if (p.type == voidStarType || p.type == voidStarFType)
                {
                    Color c = p.type == voidStarFType && p.ai[2] > 0 ? new Color(255, 100, 100) : Color.White;
                    Main.spriteBatch.Draw(voidStar.Value, p.Center - Main.screenPosition, null, c * ((255 - p.alpha) / 255f), p.rotation, voidStar.Value.Size() / 2, p.scale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(screen2, Vector2.Zero, Color.White);
            Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawSlashEffects(GraphicsDevice graphicsDevice)
        {
            if (screen == null || screen2 == null) return;

            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            // 获取投射物类型 ID
            int slashType = ModContent.ProjectileType<Slash>();
            int slash2Type = ModContent.ProjectileType<Slash2>();
            int voidExplodeType = ModContent.ProjectileType<VoidExplode>();
            int voidRExpType = ModContent.ProjectileType<VoidRExp>();
            int starlessNightProjType = ModContent.ProjectileType<StarlessNightProj>();

            // 遍历投射物，使用类型 ID 判断
            foreach (Projectile p in CheckProjs)
            {
                if (p.ModProjectile == null)
                {
                    continue;
                }

                if (p.type == slashType)
                {
                    Main.spriteBatch.Draw(ksc1.Value, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 165, 0).RotatedBy(p.rotation), null, Color.White, p.rotation + (float)Math.PI / 2, new Vector2(ksc1.Value.Width, ksc1.Value.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / ksc1.Value.Width * 0.4f, 0.1f), SpriteEffects.None, 0);
                }
                else if (p.type == slash2Type)
                {
                    Main.spriteBatch.Draw(ksc1.Value, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 300, 0).RotatedBy(p.rotation), null, Color.White, p.rotation + (float)Math.PI / 2, new Vector2(ksc1.Value.Width, ksc1.Value.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / ksc1.Value.Width * 1.4f, 1f), SpriteEffects.None, 0);
                }
                else if (p.type == voidExplodeType)
                {
                    if (p.ModProjectile is VoidExplode ve)
                    {
                        float ks = ve.Projectile.timeLeft * 0.1f;
                        if (ve.Projectile.timeLeft > 10) ks = (20 - (float)ve.Projectile.timeLeft) / 10f;
                        ks *= (1 + ve.Projectile.ai[1]);
                        Main.spriteBatch.Draw(ksc1.Value, ve.Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(ksc1.Value.Width, ksc1.Value.Height) / 2, ks * 2, SpriteEffects.None, 0);
                    }
                }
                else if (p.type == voidRExpType)
                {
                    if (p.ModProjectile is VoidRExp vre)
                    {
                        float ks = (90f - vre.Projectile.timeLeft) * 0.4f;
                        Main.spriteBatch.Draw(kscc.Value, vre.Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(kscc.Value.Width, kscc.Value.Height) / 2, ks, SpriteEffects.None, 0);
                    }
                }
                else if (p.type == starlessNightProjType)
                {
                    if (p.ModProjectile is StarlessNightProj sl)
                    {
                        sl.drawSlash();
                    }
                }
            }

            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            kscreen.CurrentTechnique = kscreen.Techniques["Technique1"];
            kscreen.CurrentTechnique.Passes[0].Apply();
            kscreen.Parameters["tex0"].SetValue(Main.screenTargetSwap);
            kscreen.Parameters["i"].SetValue(0.1f);
            Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void ApplyFinalShader(GraphicsDevice graphicsDevice)
        {
            if (FlashEffectStrength > 0)
            {
                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                for (float i = 1; i <= 16; i++)
                {
                    Main.spriteBatch.Draw(screen, screen.Size() / 2, null, Color.White * ((16f / i) * 0.1f * FlashEffectStrength), 0, screen.Size() / 2, 1 + FlashEffectStrength * 0.08f * i, SpriteEffects.None, 0);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.End();
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int abyssalWraithType = ModContent.NPCType<AbyssalWraith>();
            int cruiserHeadType = ModContent.NPCType<CruiserHead>();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == abyssalWraithType && npc.ModNPC is AbyssalWraith aw)
                {
                    NPCLoader.PreDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                    aw.Draw();
                    NPCLoader.PostDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                }
                if (npc.type == cruiserHeadType && npc.ModNPC is CruiserHead ch && ch.phase == 2)
                {
                    ch.candraw = true;
                    NPCLoader.PreDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                    ch.PreDraw(Main.spriteBatch, Main.screenPosition, Color.White);
                    NPCLoader.PostDraw(npc, Main.spriteBatch, Main.screenPosition, Color.White);
                    ch.candraw = false;
                }
            }

            int starlessNightType = ModContent.ProjectileType<StarlessNightProj>();
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type != starlessNightType)
                {
                    continue;
                }

                if (proj.ModProjectile is StarlessNightProj sl)
                {
                    sl.drawSword();
                }
            }

            Main.spriteBatch.End();
        }

        private static void HandleCutScreenEffect(GraphicsDevice graphicsDevice)
        {
            if (cutScreen <= 0)
            {
                return;
            }

            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            CEUtils.drawLine(cutScreenCenter, cutScreenCenter + cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 9000, Color.Black, 9000);
            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(screen2);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            CEUtils.drawLine(cutScreenCenter, cutScreenCenter + cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * -9000, Color.Black, 9000);
            Main.spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Black);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            Effect blur = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/blur", AssetRequestMode.ImmediateLoad).Value;
            blur.CurrentTechnique = blur.Techniques["GaussianBlur"];
            blur.Parameters["resolution"].SetValue(Main.ScreenSize.ToVector2());
            blur.Parameters["blurAmount"].SetValue(cutScreen * 0.036f);
            blur.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(screen, cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * -cutScreen * Main.GameViewMatrix.Zoom.X, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(screen2, cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * cutScreen * Main.GameViewMatrix.Zoom.X, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Main.spriteBatch.End();
        }

        private static void DrawBlackMask()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (blackMaskTime > 0)
            {
                blackMaskAlpha = Math.Min(blackMaskAlpha + 0.05f, 1f);
            }
            else
            {
                blackMaskAlpha = Math.Max(blackMaskAlpha - 0.025f, 0f);
            }
            Main.spriteBatch.Draw(CEUtils.pixelTex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.5f * blackMaskAlpha);
            Main.spriteBatch.End();
        }
    }
}
