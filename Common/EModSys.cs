﻿using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Items.Weapons.GrassSword;
using CalamityEntropy.Content.NPCs.FriendFinderNPC;
using CalamityEntropy.Content.NPCs.Prophet;
using CalamityEntropy.Content.NPCs.SpiritFountain;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Skies;
using CalamityEntropy.Content.UI;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityMod;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs.SlimeGod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Common
{
    public class EModSys : ModSystem
    {
        private static Texture2D markTex => CEUtils.getExtraTex("EvMark");
        internal static int timer;
        public static bool noItemUse = false;
        public float counter = 0;
        public bool prd = true;
        public static bool mi = false;
        public bool escLast = true;
        public bool rCtrlLast = false;
        public bool eowLast = false;
        public int eowMaxLife = 0;
        public bool slimeGodLast = false;
        public int slimeGodMaxLife = 0;
        public Vector2 LastPlayerPos;
        public Vector2 LastPlayerVel;
        public static Color GetColorForNPCBossbarFromTexture(Color[] data)
        {
            int pixelCount = 0;
            Dictionary<Color, int> dict = new();
            foreach (Color clr in data)
            {
                if (clr.A != 0)
                {
                    if (dict.ContainsKey(clr))
                    {
                        dict[clr]++;
                    }
                    else
                    {
                        dict[clr] = 0;
                    }
                    if (clr.R + clr.G + clr.B >= 80)
                    {
                        dict[clr] += (clr.R + clr.G + clr.B) / 80;
                    }
                    pixelCount++;
                }
            }
            if (pixelCount > 0)
            {
                Color c = Color.White;
                int count = 0;
                foreach (Color color in dict.Keys)
                {
                    if (dict[color] > count)
                    {
                        count = dict[color];
                        c = color;
                    }
                }
                return c;
            }
            return Color.Transparent;

        }
        public override void PostSetupContent()
        {
            Fruitcake.ammoList = new();
            List<int> AmmoIds = new List<int>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (i == ModContent.ItemType<MortarRound>() || i == ModContent.ItemType<RubberMortarRound>())
                {
                    continue;
                }
                if (ContentSamples.ItemsByType[i].ammo != AmmoID.None)
                {
                    if (!AmmoIds.Contains(ContentSamples.ItemsByType[i].ammo))
                    {
                        AmmoIds.Add(ContentSamples.ItemsByType[i].ammo);
                        Fruitcake.ammoList[ContentSamples.ItemsByType[i].ammo] = new();
                    }
                    Fruitcake.ammoList[ContentSamples.ItemsByType[i].ammo].Add(i);
                }
            }

        }
        public override void PostDrawTiles()
        {
            if (CalamityEntropy.SetupBossbarClrAuto)
            {
                CalamityEntropy.SetupBossbarClrAuto = false;
                for (int i = 0; i < NPCLoader.NPCCount; i++)
                {
                    if (!EntropyBossbar.bossbarColor.ContainsKey(i) && ContentSamples.NpcsByNetId[i].boss)
                    {
                        Main.instance.LoadNPC(i);
                        Texture2D tex = TextureAssets.Npc[i].Value;
                        var pixData = new Color[tex.Width * tex.Height];
                        tex.GetData(pixData);
                        Color c = GetColorForNPCBossbarFromTexture(pixData);
                        if (c.A > 0)
                        {
                            EntropyBossbar.bossbarColor[i] = c;
                        }
                    }
                }
            }
            if (ptype == -1)
                ptype = ModContent.NPCType<TheProphet>();
            if (sftype == -1)
                sftype = ModContent.NPCType<SpiritFountain>();

            List<int> HighLightWallTypes = new List<int>() { 94, 98, 96, 95, 99, 97 };
            DWAlpha = float.Lerp(DWAlpha, NPC.AnyNPCs(sftype) ? 0.2f : (NPC.AnyNPCs(ptype) ? 1 : 0), 0.1f);
            if (DWAlpha > 0.02f)
            {
                DrawWallsHL(HighLightWallTypes);
            }
            if (mi)
            {
                Main.instance.IsMouseVisible = true;
            }
            if (!Main.dedServ && Main.LocalPlayer.Entropy().hasAcc(SmartScope.ID))
            {
                if (SmartScope.target != null)
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                    var tx = CEUtils.getExtraTex("SS_Target");
                    Main.spriteBatch.Draw(tx, SmartScope.target.Center - Main.screenPosition, null, Color.White, 0, tx.Size() / 2f, 1, SpriteEffects.None, 0);
                    Main.spriteBatch.End();
                }
            }
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<EventideSniper>())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.dontTakeDamage && !npc.friendly)
                    {
                        Main.spriteBatch.Draw(markTex, npc.Center - Main.screenPosition, null, Color.White, 0, markTex.Size() / 2, 1, SpriteEffects.None, 0f);
                    }
                }
                Main.spriteBatch.End();
            }

        }
        public int ptype = -1;
        public int sftype = -1;
        public static List<int> NeedTiles = new List<int>() { 41, 43, 44 };
        public void DrawWallsHL(List<int> types)
        {
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/WhiteTrans", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader, Main.GameViewMatrix.TransformationMatrix);

            shader.CurrentTechnique.Passes[0].Apply();
            shader.Parameters["strength"].SetValue(0.2f);
            float gfxQuality = Main.gfxQuality;
            int offScreenRange = Main.offScreenRange;
            bool drawToScreen = Main.drawToScreen;
            Vector2 screenPosition = Main.screenPosition;
            int screenWidth = Main.screenWidth;
            int screenHeight = Main.screenHeight;
            int maxTilesX = Main.maxTilesX;
            int maxTilesY = Main.maxTilesY;
            int[] wallBlend = Main.wallBlend;
            SpriteBatch spriteBatch = Main.spriteBatch;
            var _tileArray = Main.tile;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int num = (int)(120f * (1f - gfxQuality) + 40f * gfxQuality);
            int num2 = (int)((float)num * 0.4f);
            int num3 = (int)((float)num * 0.35f);
            int num4 = (int)((float)num * 0.3f);
            Vector2 vector = new Vector2(offScreenRange, offScreenRange);
            if (true)
            {
                vector = Vector2.Zero;
            }
            int num5 = (int)((screenPosition.X - vector.X) / 16f - 1f);
            int num6 = (int)((screenPosition.X + (float)screenWidth + vector.X) / 16f) + 2;
            int num7 = (int)((screenPosition.Y - vector.Y) / 16f - 1f);
            int num8 = (int)((screenPosition.Y + (float)screenHeight + vector.Y) / 16f) + 5;
            int num9 = offScreenRange / 16;
            int num10 = offScreenRange / 16;
            if (num5 - num9 < 4)
            {
                num5 = num9 + 4;
            }
            if (num6 + num9 > maxTilesX - 4)
            {
                num6 = maxTilesX - num9 - 4;
            }
            if (num7 - num10 < 4)
            {
                num7 = num10 + 4;
            }
            if (num8 + num10 > maxTilesY - 4)
            {
                num8 = maxTilesY - num10 - 4;
            }
            VertexColors vertices = default(VertexColors);
            Rectangle value = new Rectangle(0, 0, 16, 16);
            int underworldLayer = Main.UnderworldLayer;
            Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();
            for (int i = num7 - num10 + screenOverdrawOffset.Y; i < num8 + num10 - screenOverdrawOffset.Y; i++)
            {
                for (int j = num5 - num9 + screenOverdrawOffset.X; j < num6 + num9 - screenOverdrawOffset.X; j++)
                {
                    Tile tile = _tileArray[j, i];
                    ushort wall = tile.WallType;
                    if (types.Contains(wall) && tile.WallType > 0 && NeedTiles.Contains(tile.TileType) && !tile.HasUnactuatedTile)
                    {
                        value.X = tile.TileFrameX;
                        value.Y = tile.TileFrameY + Main.tileFrame[tile.TileType] * 0;

                        Texture2D GetTileDrawTexture(Tile tile, int tileX, int tileY)
                        {
                            Texture2D result = TextureAssets.Tile[tile.TileType].Value;
                            int wall = tile.TileType;
                            Texture2D texture2D = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(wall, 0, tile.TileColor);
                            if (texture2D != null)
                            {
                                result = texture2D;
                            }
                            return result;
                        }
                        Texture2D tileDrawTexture = GetTileDrawTexture(tile, j, i);
                        vertices = new VertexColors(Color.LightBlue);
                        var pos = new Vector2(j * 16 - (int)screenPosition.X, i * 16 - (int)screenPosition.Y) + vector;
                        spriteBatch.Draw(tileDrawTexture, pos, value, Color.SkyBlue * DWAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                    }
                }
            }
            Main.spriteBatch.ExitShaderRegion();
            Main.spriteBatch.End();
        }
        public float DWAlpha = 0;
        public static bool sayTip = true;
        public override void UpdateUI(GameTime gameTime)
        {
            lhBarTarget = float.Lerp(lhBarTarget, ((float)Main.LocalPlayer.statLife / (float)Main.LocalPlayer.statLifeMax2), 0.1f);
            lhBarTarget2 = float.Lerp(lhBarTarget2, lhBarTarget, 0.06f);

            if (Lighting.Mode != Terraria.Graphics.Light.LightMode.Color)
            {
                if (sayTip)
                {
                    sayTip = false;
                }
            }
            if (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy)
            {
                Lighting.Mode = Terraria.Graphics.Light.LightMode.Color;
            }
            noItemUse = false;
            counter += 1f;
            if (ArmorForgingStationUI.Visible)
            {
                CalamityEntropy.Instance.userInterface?.Update(gameTime);
            }
        }

        public override void PostUpdateDusts()
        {
            if (CalamityEntropy.FlashEffectStrength > 0)
            {
                CalamityEntropy.FlashEffectStrength -= 0.02f;
            }
            CalamityEntropy.blackMaskTime--;
            PixelParticle.Update();
            VoidParticles.Update();
            AbyssalParticles.Update();
            CalamityEntropy.cutScreen += CalamityEntropy.cutScreenVel;
            if (CalamityEntropy.cutScreen > 0)
            {
                CalamityEntropy.cutScreenVel -= 0.5f;
            }
            if (CalamityEntropy.cutScreen < 0)
            {
                CalamityEntropy.cutScreen = 0;
                CalamityEntropy.cutScreenVel = 0;
            }
        }

        public override void PostUpdatePlayers()
        {
            CECooldowns.Update();
            EBookUI.update();
            if (ModContent.GetInstance<RepMusicScene>().IsSceneEffectActive(Main.LocalPlayer))
            {
                Main.musicFade[MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/RepBossTrack")] = 1;
            }
            if (CalamityEntropy.noMusTime > 0)
            {
                CalamityEntropy.noMusTime--;
                Main.curMusic = 0;
                Main.newMusic = 0;
                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    Main.musicFade[i] = 0;
                }
            }

            bool rCtrl = Keyboard.GetState().IsKeyDown(Keys.RightControl);
            if (!rCtrlLast && rCtrl)
            {

            }
            rCtrlLast = rCtrl;


            if (!Main.playerInventory)
            {
                if (ArmorForgingStationUI.Visible)
                {
                    CalamityEntropy.Instance.armorForgingStationUI.close();
                }
                ArmorForgingStationUI.Visible = false;

            }
            escLast = Keyboard.GetState().IsKeyDown(Keys.Escape);
            if (!Main.dedServ)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    if (!prd)
                    {
                        prd = true;
                        mi = !mi;
                    }
                }
                else
                {
                    prd = false;
                }
            }


            LoopSoundManager.update();
        }
        public void drawChargeBar(Vector2 center, float prog, Color color)
        {
            Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/ui_chargebar").Value;
            Main.spriteBatch.Draw(bar, center, new Rectangle(0, 0, 54, 12), Color.White, 0, new Vector2(27, 6), 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bar, center, new Rectangle(0, 12, (int)Math.Round(54 * prog), 12), color, 0, new Vector2(27, 6), 1, SpriteEffects.None, 0);
        }
        public override void PreUpdateDusts()
        {
            EParticle.updateAll();
        }
        public void drawXythBar()
        {
            if (Main.LocalPlayer.HeldItem.ModItem is Xytheron xr)
            {
                float prog = xr.charge / 20f;
                Vector2 Center = Main.ScreenSize.ToVector2() * 0.5f + new Vector2(0, 56);
                Texture2D bar = CEUtils.getExtraTex("XythBar");
                Main.spriteBatch.Draw(bar, Center, new Rectangle(0, 0, 64, 26), Color.White, 0, new Vector2(32, 13), 1, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(bar, Center, new Rectangle(0, 26, (int)(8 + 48 * prog), 6), Color.White, 0, new Vector2(32, 1), 1, SpriteEffects.None, 0);
                if (Main.MouseScreen.getRectCentered(2, 2).Intersects(Center.getRectCentered(64, 26)))
                {
                    Main.instance.MouseText(Mod.GetLocalization("XythCharge").Value + ": " + xr.charge.ToString() + "/20");
                }
            }
        }
        public static float lhBarTarget2 = 1;
        public static float lhBarTarget = 1;
        public static float lhRedLerp = 0;
        public static Vector2 bbarOffset = Vector2.Zero;
        private static void DrawLifeBarText(SpriteBatch spriteBatch, Vector2 topLeftAnchor)
        {
            Vector2 vector = topLeftAnchor + new Vector2(130f, -24f);
            Player localPlayer = Main.LocalPlayer;
            Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
            string text = Lang.inter[0].Value + " " + localPlayer.statLifeMax2 + "/" + localPlayer.statLifeMax2;
            Vector2 vector2 = FontAssets.MouseText.Value.MeasureString(text);
            spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[0].Value, vector + new Vector2((0f - vector2.X) * 0.5f, 0f), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(FontAssets.MouseText.Value, localPlayer.statLife + "/" + localPlayer.statLifeMax2, vector + new Vector2(vector2.X * 0.5f, 0f), color, 0f, new Vector2(FontAssets.MouseText.Value.MeasureString(localPlayer.statLife + "/" + localPlayer.statLifeMax2).X, 0f), 1f, SpriteEffects.None, 0f);
        }
        public static float bbar = 0;

        public static void DrawBrambleBar()
        {
            Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
            Texture2D bar = CEUtils.getExtraTex("BrambleBar");
            bbar = float.Lerp(bbar, Main.LocalPlayer.Entropy().BrambleBarCharge, 0.16f);
            Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight / 16);
            if((center + bbarOffset).getRectCentered(100, 46).Intersects(Main.MouseScreen.getRectCentered(2, 2)))
            {
                Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("BCBarInfo").Value);
                if(Mouse.GetState().MiddleButton == ButtonState.Pressed)
                    bbarOffset = Main.MouseScreen - center;
            }
            center += bbarOffset;
            Main.spriteBatch.Draw(bar, center, new Rectangle(0, 0, 66, 34), Color.White, 0, new Vector2(33, 17), 1.4f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bar, center, new Rectangle(12, 36, (int)(42 * bbar), 8), Color.White, 0, new Vector2(21, 5), 1.4f, SpriteEffects.None, 0);
            Main.spriteBatch.UseSampleState_UI(SamplerState.AnisotropicClamp);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            for (int ei = 0; ei < layers.Count; ei++)
            {
                var l = layers[ei];

                if (l.Name == "Vanilla: Resource Bars")
                {
                    var drawLHB = new LegacyGameInterfaceLayer("Lost Heirloom HB", () =>
                    {
                        if (Main.LocalPlayer.dead || !Main.LocalPlayer.GetModPlayer<LostHeirloomPlayer>().vanityEquipped)
                        { return true; }
                        Texture2D t1 = CEUtils.getExtraTex("llBar1");
                        Texture2D t2 = CEUtils.getExtraTex("llBar2");
                        Texture2D t3 = CEUtils.getExtraTex("llBar3");
                        Vector2 offset = new Vector2(Main.screenWidth - 610, 10);
                        Main.spriteBatch.Draw(t3, offset, Color.White);
                        Main.spriteBatch.DrawString(CalamityEntropy.efont2, Main.LocalPlayer.statLife.ToString(), offset + new Vector2(30, 24), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(t1, offset + new Vector2(100, 28), Color.White);
                        Main.spriteBatch.Draw(t2, offset + new Vector2(108, 34), new Rectangle(0, 0, (int)((float)t2.Width * lhBarTarget2), t2.Height), new Color(255, 160, 160));
                        Main.spriteBatch.Draw(t2, offset + new Vector2(108, 34), new Rectangle(0, 0, (int)((float)t2.Width * lhBarTarget), t2.Height), Color.White);
                        Vector2 vector = new Vector2(Main.screenWidth - 300 + 4, 15f);
                        vector.Y += 6f;
                        DrawLifeBarText(Main.spriteBatch, vector + new Vector2(-4f, 3f));
                        typeof(FancyClassicPlayerResourcesDisplaySet).GetMethod("PrepareFields", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, new Type[] { typeof(Player) }).Invoke(((FancyClassicPlayerResourcesDisplaySet)((Dictionary<string, IPlayerResourcesDisplaySet>)Main.ResourceSetsManager.GetType().GetField("_sets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(Main.ResourceSetsManager))["New"]), new object[] { Main.LocalPlayer });
                        typeof(FancyClassicPlayerResourcesDisplaySet).GetMethod("DrawManaBar", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, new Type[] { typeof(SpriteBatch) }).Invoke(((FancyClassicPlayerResourcesDisplaySet)((Dictionary<string, IPlayerResourcesDisplaySet>)Main.ResourceSetsManager.GetType().GetField("_sets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(Main.ResourceSetsManager))["New"]), new object[] { Main.spriteBatch });
                        return true;
                    }, InterfaceScaleType.UI);
                    if (Main.LocalPlayer.GetModPlayer<LostHeirloomPlayer>().vanityEquipped)
                    {
                        l.Active = false;
                    }
                    layers.Insert(ei,
                        drawLHB
                        );
                    break;
                }
            }
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Void Charge Bar", () =>
                {
                    DrawVoidChargeBar(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Other Charge Bars", () =>
                {
                    int baroffsety = 84;
                    if (Main.LocalPlayer.GetModPlayer<CapricornBookmarkRecordPlayer>().SandStormCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.GetModPlayer<CapricornBookmarkRecordPlayer>().SandStormCharge, new Color(246, 201, 122));
                        baroffsety += 20;
                    }
                    if (Main.LocalPlayer.Entropy().SnowgraveCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.Entropy().SnowgraveCharge, new Color(170, 170, 255));
                        baroffsety += 20;
                    }
                    if (Main.LocalPlayer.Entropy().mawOfVoidCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.Entropy().mawOfVoidCharge, Color.Red);
                        baroffsety += 20;
                    }
                    if (Main.LocalPlayer.Entropy().revelationCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.Entropy().revelationCharge, new Color(255, 255, 190));
                        baroffsety += 20;
                    }
                    
                    return true;
                }, InterfaceScaleType.None));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Poop UI", () =>
                {
                    if (Main.LocalPlayer.Entropy().brokenAnkh)
                    {
                        Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
                        PoopsUI.Draw();
                        Main.spriteBatch.UseSampleState_UI(SamplerState.AnisotropicClamp);
                    }
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Xyth Bar", () =>
                {
                    if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Bramblecleave>())
                    {
                        DrawBrambleBar();
                    }
                    drawXythBar();
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
                "CalamityEntropy: Armor Reforging Station",
                delegate
                {
                    if (ArmorForgingStationUI.Visible)
                        CalamityEntropy.Instance.armorForgingStationUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.UI)
            );
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
                "CalamityEntropy: EntropyBookItemUI",
                delegate
                {
                    EBookUI.draw();
                    return true;
                },
                InterfaceScaleType.UI)
            );
            }
        }

        public void DrawVoidChargeBar(SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.Entropy().VFSet)
            {
                return;
            }
            Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/VoidChargeBar").Value;
            Texture2D prog = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/VoidChargeProgress").Value;
            Config config = ModContent.GetInstance<Config>();
            Vector2 offset = new Vector2(config.VoidChargeBarX, config.VoidChargeBarY);
            float p = Main.LocalPlayer.Entropy().VoidCharge;
            if (Main.LocalPlayer.Entropy().VoidInspire > 0)
            {
                p = Main.LocalPlayer.Entropy().VoidInspire / 600f;
                offset += new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));
            }
            spriteBatch.Draw(bar, offset, null, Color.White, 0, bar.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(prog, offset, new Rectangle(0, 0, (int)(prog.Width * p), prog.Height), Color.White, 0, prog.Size() / 2, 1, SpriteEffects.None, 0);
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 1, 1);
            if (new Rectangle((int)(offset - bar.Size() / 2).X, (int)((offset - bar.Size() / 2).Y), (int)bar.Size().X, (int)bar.Size().Y).Intersects(mouse))
            {
                string textToDisplay = Language.GetOrRegister("Mods.CalamityEntropy.VoidChargeBar").Value + " : " + ((int)(p * 100)).ToString() + "%";
                Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
            }
        }

        public override void PostUpdateNPCs()
        {
            if (CalamityEntropy.Instance.screenShakeAmp > 0)
            {
                CalamityEntropy.Instance.screenShakeAmp -= 0.5f;
            }
            bool eow = false;
            int maxlifeEows = 0;
            bool sg = false;
            int maxlifeSg = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)
                {
                    eow = true;
                    maxlifeEows += n.lifeMax;
                }
                if (ModContent.NPCType<CrimulanPaladin>() == n.type || ModContent.NPCType<SplitCrimulanPaladin>() == n.type || ModContent.NPCType<EbonianPaladin>() == n.type || ModContent.NPCType<SplitEbonianPaladin>() == n.type)
                {
                    sg = true;
                    maxlifeSg += n.lifeMax;
                }
            }
            if (eow && !eowLast)
            {
                eowMaxLife = maxlifeEows;
            }
            eowLast = eow;

            if (sg && !slimeGodLast)
            {
                slimeGodMaxLife = maxlifeSg;
            }

            slimeGodLast = sg;

        }

        public static void RemoveItemInARecipe(Recipe recipe, int type)
        {
            for (int i = recipe.requiredItem.Count - 1; i >= 0; i--)
            {
                if (recipe.requiredItem[i].type == type)
                {
                    recipe.requiredItem.RemoveAt(i);
                }
            }
        }
        public static void RemoveItemInRecipes(int itemtype, int type)
        {
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe recipe = Main.recipe[i];
                if (recipe.createItem.type == itemtype)
                {
                    RemoveItemInARecipe(recipe, type);
                }
            }
        }

        public override void PostAddRecipes()
        {
            foreach (var recipe in Main.recipe)
            {
                if (recipe.createItem.type == ModContent.ItemType<CalamityMod.Items.Placeables.FurnitureAuric.AuricToilet>())
                {
                    recipe.createItem.type = ModContent.ItemType<Content.Items.AuricToilet>();
                }
            }
            RemoveItemInRecipes(ModContent.ItemType<Sylvestaff>(), ItemID.GenderChangePotion);
        }

        public override void PreUpdateProjectiles()
        {
            timer++;
            if (!Main.dedServ)
            {
                LastPlayerPos = Main.LocalPlayer.Center;
            }
        }

        public override void PostUpdateProjectiles()
        {
            CEUtils.Update();
            if (EGlobalProjectile.SSCD < 3)
            {
                EGlobalProjectile.SSCD++;
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC is FriendFindNPC)
                {
                    npc.Entropy().friendFinderOwner.ToPlayer().slotsMinions += 1;
                    npc.Entropy().friendFinderOwner.ToPlayer().numMinions += 1;
                }
            }
            if (!Main.dedServ)
            {
            }
        }
    }
}
