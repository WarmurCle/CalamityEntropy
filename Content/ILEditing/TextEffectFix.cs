using CalamityEntropy.Common;
using CalamityEntropy.Core.ChatTags;
using CalamityMod;
using CalamityMod.ChatTags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Rarities;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace CalamityEntropy.Content.ILEditing
{
    public static class TextEffectFix
    {
        public static bool Active()
        {
            return Config.Instance.CalamityTextEffectCompatibilityFix;
        }
        public static void LoadHooks()
        {
            if (Active())
            {
                var mtd = typeof(DoGTextSnippet).GetMethod("UniqueDraw", BindingFlags.Instance | BindingFlags.Public);
                EModHooks.Add(mtd, hookDoG);
                mtd = typeof(BurnishedAuric.CustomTextSnippet).GetMethod("UniqueDraw", BindingFlags.Instance | BindingFlags.Public);
                EModHooks.Add(mtd, hookBurnishedAuric);
                mtd = typeof(ExoticRainbow.CustomTextSnippet).GetMethod("UniqueDraw", BindingFlags.Instance | BindingFlags.Public);
                EModHooks.Add(mtd, hookExoticRainbow);

                mtd = typeof(DivineSwine).GetMethod("Draw_BestiaryPortrait", BindingFlags.Instance | BindingFlags.Public);
                EModHooks.Add(mtd, hookDivineSwinePortrait);
            }
        }
        public static void hookDivineSwinePortrait(Action<DivineSwine, SpriteBatch> orig, DivineSwine self, SpriteBatch spriteBatch)
        {
            NPC NPC = self.NPC;
            if (Active())
            {
                Texture2D baseTexture = TextureAssets.Npc[self.Type].Value;
                float floatHeight = MathHelper.Lerp(-8f, 8f, MathF.Sin((float)NPC.ai[0] / 150f) * 0.5f + 0.5f);
                Vector2 drawPosition = NPC.Center + Vector2.UnitY * floatHeight;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, CalamityUtils.SubtractiveBlending, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                int backShadowCount = 3;
                for (int i = 0; i < backShadowCount; i++)
                {
                    float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.08f) + NPC.whoAmI;
                    Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount + rotation) * 6f;
                    spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.9f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
                }

                int backShadowCount2 = 6;
                for (int i = 0; i < backShadowCount2; i++)
                {
                    float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.06f) + NPC.whoAmI;
                    Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount2 + rotation) * 12f;
                    spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.7f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                spriteBatch.Draw(baseTexture, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
            }
            else
            {
                orig(self, spriteBatch);
            }
        }

        public delegate bool DoGDelegate(DoGTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale);
        public delegate bool BADelegate(BurnishedAuric.CustomTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale);
        public delegate bool ERDelegate(ExoticRainbow.CustomTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale);


        public static bool hookDoG(DoGDelegate orig, DoGTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            if(!Active()) return orig.Invoke(self, justCheckingString, out size, spriteBatch, position, color, scale);
            var fds = self.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0].GetValue(self);
            string tx = "[Can't get the text]";
            if (fds is string text)
                tx = text;
            return new DoGTextSnippetFix(tx).UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
        }
        public static bool hookBurnishedAuric(BADelegate orig, BurnishedAuric.CustomTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            if (!Active()) return orig.Invoke(self, justCheckingString, out size, spriteBatch, position, color, scale);
            var fds = self.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0].GetValue(self);
            string tx = "[Can't get the text]";
            if (fds is string text)
                tx = text;
            return new BAuricSnippetFix(tx).UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
        }
        public static bool hookExoticRainbow(ERDelegate orig, ExoticRainbow.CustomTextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            if (!Active()) return orig.Invoke(self, justCheckingString, out size, spriteBatch, position, color, scale);
            var fds = self.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0].GetValue(self);
            string tx = "[Can't get the text]";
            if (fds is string text)
                tx = text;
            var ins = new ExoticRainbowSnippetFix(tx);
            ins.IsExpert = self.IsExpert;
            return ins.UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
        }
    }

    public sealed class DoGTextSnippetFix(string text) : TextSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                var pos = position;
                void DrawStr(Vector2 offset, Color clr)
                {
                    string txt = "";
                    foreach (var item in text)
                    {
                        pos = position;
                        pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;
                        float sin = MathHelper.SmoothStep(0, 1, (MathF.Sin(pos.X * 0.02f + Main.GlobalTimeWrappedHourly * -1.5f) + 1) * 0.5f);
                        var c = Color.Lerp(Color.Cyan, Color.Fuchsia, sin);
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos + new Vector2(0, -2 + sin * 4), c.Mult(clr), 0, Vector2.Zero, new Vector2(scale));
                        txt += item;
                    }
                }
                foreach (var item in ChatManager.ShadowDirections)
                {
                    DrawStr(Vector2.Zero + item * 2, Color.Black);
                }
                DrawStr(Vector2.Zero, Color.White);
            }
            Main.spriteBatch.UseSampleState_UI(Main.DefaultSamplerState);
            return true;
        }
        public override float GetStringLength(DynamicSpriteFont font)
        {
            float size = font.MeasureString(text).X;
            return size * Scale;
        }
    }

    public sealed class BAuricSnippetFix(string text) : TextSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            try
            {
                Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
                size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

                if (color == default || color == Main.MouseTextColorReal)
                {
                    color = Colors.AlphaDarken(BurnishedAuric.TextClr);
                }
                if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
                {
                    var borderColor = color * 2f;
                    var coreColor = new Color(77, 0, 33);
                    var shineColor = new Color(254, 231, 117);
                    if ((bool)typeof(BurnishedAuric).GetField("isFlashing", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null))
                    {
                        shineColor = new Color(90, 207, 255);
                        position += Main.rand.NextVector2Circular(8f, 4.8f);
                    }

                    var pos = position;
                    void lDraw(Vector2 offset, Color clr)
                    {
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, pos + offset, Color.White.Mult(clr), 0, Vector2.Zero, new Vector2(scale));
                    }
                    for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.TwoPi * 0.125f)
                    {
                        lDraw(new Vector2(2, 0).RotatedBy(f), borderColor);
                    }
                    lDraw(Vector2.Zero, coreColor);


                    void lDraw2(Vector2 offset, Color clr)
                    {
                        string txt = "";
                        foreach (var item in text)
                        {
                            pos = position;
                            pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;
                            float sin = (MathF.Sin(pos.X * 0.02f + Main.GlobalTimeWrappedHourly * -1.5f) + 1) * 0.5f;
                            var c = shineColor * MathF.Pow(sin, 120);
                            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos + offset, c.Mult(clr), 0, Vector2.Zero, new Vector2(scale));
                            txt += item;
                        }
                    }
                    lDraw2(Vector2.Zero, Color.White);
                    Main.spriteBatch.UseSampleState_UI(Main.DefaultSamplerState);
                }
            }
            catch
            {
                size = Vector2.One;
                return true;
            }
            return true;
        }
        public override float GetStringLength(DynamicSpriteFont font)
        {
            float size = font.MeasureString(text).X;
            return size * Scale;
        }
    }
    public sealed class ExoticRainbowSnippetFix(string text) : TextSnippet
    {
        public bool IsExpert = false;
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

            if (color == default || color == Main.MouseTextColorReal)
            {
                color = Colors.AlphaDarken(ExoticRainbow.TextClr);
            }

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                var font = FontAssets.MouseText.Value;
                var time = Main.GlobalTimeWrappedHourly;

                List<Color> eColors = new List<Color>()
                    {
                        new Color(255,107,107), //Ares
                        new Color(125,196,225), //Thanatos
                        new Color(211,235,108), //Apollo
                        //new Color(255,160,71), //Artemis
                    };

                if (IsExpert)
                    eColors = new List<Color>()
                        {
                        new Color(255,70,70),
                        new Color(255,70,255),
                        new Color(70,70,255),
                        new Color(70,255,255),
                        new Color(70,255,90),
                        new Color(255,255,70)
                    };

                var pos = position;
                void Draw1(Vector2 offset, Color clr)
                {
                    string txt = "";
                    foreach (var item in text)
                    {
                        pos = position;
                        pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;

                        float rate = Main.GlobalTimeWrappedHourly * (IsExpert ? 2 : 1) + pos.X * (IsExpert ? 0.01f : 0.005f);
                        int colorIndex = (int)(rate / 2 % eColors.Count);
                        Color currentColor = eColors[colorIndex];
                        Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                        Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : MathF.Round(rate % 1f));

                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos + offset, usedColor.Mult(clr), 0, Vector2.Zero, new Vector2(scale));
                        txt += item;
                    }
                }

                float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2 / MathHelper.Pi);
                sine = (float)Math.Pow(MathHelper.Lerp(sine, 0, 0.35f), 5);
                int draws = 16;
                for (int i = 0; i < draws; i++)
                {
                    Vector2 backPosition = (MathHelper.TwoPi * i / (float)draws + Main.GlobalTimeWrappedHourly * 1.7f).ToRotationVector2() * (4 + 16 * sine);
                    Draw1(backPosition, Color.White);
                }

                for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.TwoPi * 0.125f)
                {
                    Draw1(new Vector2(2, 0).RotatedBy(f), Color.Black);
                }
                Draw1(Vector2.Zero, Color.White);

                Main.spriteBatch.UseSampleState_UI(Main.DefaultSamplerState);
            }
            return true;
        }
        public override float GetStringLength(DynamicSpriteFont font)
        {
            float size = font.MeasureString(text).X;
            return size * Scale;
        }
    }

}