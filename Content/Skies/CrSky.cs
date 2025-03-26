using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class CrSky : CustomSky
    {
        private bool skyActive;
        private float opacity;


        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().crSky > 0;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, new Color(255, 255, 255, inColor.A), opacity);
        }
        public int counter = 0;
        public int awtime = 0;
        public Effect skyEffect = null;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (this.skyEffect == null)
            {
                this.skyEffect = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/AWSkyEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            counter++;
            Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/CrSky").Value;
            float pc = 1f;
            if (SubworldSystem.IsActive<VOIDSubworld>())
            {
                pc *= 0.2f;
            }
            Color ocolor = new Color((int)(12 * pc), (int)(65 * pc), (int)(100 * pc));
            bool drawAWMask = false;
            int AWIndex = -1;
            if (NPC.AnyNPCs(ModContent.NPCType<AbyssalWraith>()))
            {
                awtime = 180;
                ocolor = new Color((int)(190 * pc), (int)(55 * pc), (int)(205 * pc));
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.type == ModContent.NPCType<AbyssalWraith>())
                    {
                        drawAWMask = true;
                        AWIndex = n.whoAmI;
                        break;

                    }
                }
            }
            awtime--;


            Vector2 dp = new Vector2((Main.screenPosition.X * -0.5f + counter * 0.3f) % txd.Width, (Main.screenPosition.Y * -0.5f + counter * -0.1f) % txd.Height);
            spriteBatch.Draw(txd, dp + new Vector2(0, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp - txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if (awtime > 0)
            {

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);
                ocolor = new Color((int)(36 * pc), (int)(36 * pc), (int)(36 * pc));
                if (Main.rand.NextBool(120))
                {
                    if (Main.rand.NextBool(6))
                    {
                        SoundStyle s = SoundID.Thunder;
                        s.Volume = Main.rand.NextFloat() * 0.4f;
                        SoundEngine.PlaySound(s);
                    }
                    LightningParticle.lightningParticles.Add(new LightningParticle());
                }
                ocolor = new Color((int)(40 * pc), (int)(40 * pc), (int)(40 * pc));

                foreach (LightningParticle p in LightningParticle.lightningParticles)
                {
                    p.draw(opacity);
                }
                for (int i = LightningParticle.lightningParticles.Count - 1; i >= 0; i--)
                {
                    if (LightningParticle.lightningParticles[i].timeleft <= 0)
                    {
                        LightningParticle.lightningParticles.RemoveAt(i);
                    }
                }
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);
                ocolor = new Color((int)(12 * pc), (int)(62 * pc), (int)(96 * pc));
            }
            float c = 1f;
            if (SubworldSystem.IsActive<VOIDSubworld>())
            {
                c *= 0.2f;
            }
            dp = new Vector2((Main.screenPosition.X * -0.5f * c + counter * -0.3f * c) % txd.Width, (Main.screenPosition.Y * -0.5f * c + counter * 0.1f * c) % txd.Height);
            spriteBatch.Draw(txd, dp + new Vector2(0, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp - txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if (drawAWMask)
            {
                spriteBatch.End();
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                RenderTarget2D screen = ModContent.GetInstance<CalamityEntropy>().screen;
                RenderTarget2D screen2 = ModContent.GetInstance<CalamityEntropy>().screen2;
                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);
                Texture2D s = Util.Util.getExtraTex("Perlin");
                Texture2D s1 = Util.Util.getExtraTex("AwSky1");
                Texture2D s2 = Util.Util.getExtraTex("AwSky2");
                spriteBatch.Draw(s, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle((int)(counter * 0.1f - Main.screenPosition.X * -0.5f), (int)(counter * 0.1f - Main.screenPosition.Y * -0.5f), Main.screenWidth, Main.screenHeight), Color.White);


                spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);

                spriteBatch.Draw(s1, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle((int)(counter * 1.16f - Main.screenPosition.X * -0.5f), (int)(counter * 1f - Main.screenPosition.Y * -0.5f), Main.screenWidth, Main.screenHeight), Color.White * 0.6f);
                spriteBatch.Draw(s2, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle((int)(counter * 0.4f - Main.screenPosition.X * -0.5f), (int)(counter * -1f - Main.screenPosition.Y * -0.5f), Main.screenWidth, Main.screenHeight), Color.White * 0.6f);


                spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                spriteBatch.Draw(screen2, Vector2.Zero, Color.White);

                skyEffect.CurrentTechnique = skyEffect.Techniques["Technique1"];
                skyEffect.CurrentTechnique.Passes[0].Apply();
                skyEffect.Parameters["tex0"].SetValue(screen);
                skyEffect.Parameters["minAlpha"].SetValue(1.16f);
                skyEffect.Parameters["a"].SetValue(opacity);
                skyEffect.Parameters["r"].SetValue(0.16f);
                skyEffect.Parameters["g"].SetValue(0.2f);
                skyEffect.Parameters["b"].SetValue(0.36f);
                spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White * opacity);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);

                Texture2D m = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;
                float size = ((AbyssalWraith)AWIndex.ToNPC().ModNPC).anmlerp;

                spriteBatch.Draw(m, AWIndex.ToNPC().Center - Main.screenPosition, null, Color.Purple, 0, m.Size() / 2, 16 * size, SpriteEffects.None, 0);
            }
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);

            if (SubworldSystem.IsActive<VOIDSubworld>())
            {
                Util.Util.DrawGlow(Main.screenPosition, Color.White * 0.4f, 40);
                opacity = 1;
            }

        }
        public class LightningParticle
        {
            public List<Vector2> points = new List<Vector2>();
            public List<Vector2> points2 = new List<Vector2>();
            public static List<LightningParticle> lightningParticles = new List<LightningParticle>();
            public LightningParticle()
            {
                Vector2 centerp = Main.screenPosition * 0.5f + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) + new Vector2(Main.rand.Next(-1200, 1201), Main.rand.Next(-1200, 1201));
                float a1 = Util.Util.randomRot();
                float a2 = a1 + MathHelper.ToRadians(180);
                Vector2 p1 = centerp;
                Vector2 p2 = centerp;
                for (int i = 0; i < 20; i++)
                {
                    points.Add(p1);
                    points2.Add(p2);
                    a1 += ((float)Main.rand.NextDouble() - 0.5f) * 1f;
                    a2 += ((float)Main.rand.NextDouble() - 0.5f) * 1f;
                    p1 += a1.ToRotationVector2() * Main.rand.Next(50, 66);
                    p2 += a2.ToRotationVector2() * Main.rand.Next(50, 66);
                }
            }

            public int timeleft = 200;
            public int maxTime = 200;
            public void draw(float op)
            {

                timeleft--;
                Texture2D px = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;

                float jd = 1;
                float lw = 2f * ((float)timeleft / maxTime);
                Color color = Color.Purple;
                for (int i = 1; i < points.Count; i++)
                {
                    Vector2 jv = points[i] - points[i - 1];
                    jv.Normalize();
                    jv *= 2;
                    Util.Util.drawLine(Main.spriteBatch, px, points[i - 1] - Main.screenPosition * 0.5f, points[i] - Main.screenPosition * 0.5f + jv, color * jd * op, 14f * lw, 0, false);
                    lw -= 2f * ((float)timeleft / maxTime) / ((float)points.Count + 1);
                }

                jd = 1;
                lw = 2f * ((float)timeleft / maxTime);
                for (int i = 1; i < points2.Count; i++)
                {
                    Vector2 jv = points2[i] - points2[i - 1];
                    jv.Normalize();
                    jv *= 2;
                    Util.Util.drawLine(Main.spriteBatch, px, points2[i - 1] - Main.screenPosition * 0.5f, points2[i] - Main.screenPosition * 0.5f + jv, color * jd * op, 14f * lw, 0, false);
                    lw -= 2f * ((float)timeleft / maxTime) / ((float)points2.Count + 1);
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().crSky <= 0 || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
