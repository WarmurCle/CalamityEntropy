using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Humanizer;
using System;
using CalamityMod.Items.Weapons.Rogue;
using CalamityEntropy.Util;
using CalamityMod;
using System.Collections.Generic;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using CalamityEntropy.Projectiles;
using Terraria.DataStructures;
using System.IO;
using Microsoft.CodeAnalysis;
using CalamityEntropy.NPCs;
using CalamityEntropy.NPCs.Cruiser;
using CalamityEntropy.Buffs;
using CalamityEntropy.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.DesertScourge;
using Terraria.Localization;
using CalamityMod.UI;
using CalamityEntropy.Projectiles.Cruiser;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.GameContent.Bestiary;
using CalamityMod.Items.TreasureBags;
using CalamityEntropy.Items.Accessories;
using CalamityEntropy.Projectiles.Pets.Abyss;
using CalamityEntropy.Items.Pets;
using CalamityMod.Skies;
using System.Runtime.Intrinsics.Arm;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Terraria.GameContent;
using CalamityMod.Projectiles.Melee;
using CalamityEntropy.Items.Accessories.Cards;
using CalamityMod.Events;
using Terraria.WorldBuilding;
using System.Threading;
using Steamworks;
using CalamityEntropy.NPCs.AbyssalWraith;
using CalamityEntropy.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.UI;
using Terraria.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityEntropy.ILEditing;
namespace CalamityEntropy
{
	public class CalamityEntropy : Mod
	{
		public static List<int> calDebuffIconDisplayList = new List<int>();
		public static CalamityEntropy Instance;
        public static Effect kscreen;
        public static Effect kscreen2;
        public static Effect cve;
        public static Effect cve2;
        public RenderTarget2D screen = null;
        public RenderTarget2D screen2 = null;
        public RenderTarget2D screen3 = null;
        public int screenShakeAmp = 0;
        public float cvcount = 0;
        public Vector2 screensz = Vector2.Zero;
        public static bool ets = true;
        public static Texture2D pixel;
        public ArmorForgingStationUI armorForgingStationUI;
        public UserInterface userInterface;
        public override void Load()
        {
            armorForgingStationUI = new ArmorForgingStationUI();
            armorForgingStationUI.Activate();
            userInterface = new UserInterface();
            userInterface.SetState(armorForgingStationUI);
            EnchantmentManager.ItemUpgradeRelationship[ModContent.ItemType<VoidEcho>()] = ModContent.ItemType<Mercy>();
            ets = true;
            kscreen = ModContent.Request<Effect>("CalamityEntropy/Effects/kscreen", AssetRequestMode.ImmediateLoad).Value;
            kscreen2 = ModContent.Request<Effect>("CalamityEntropy/Effects/kscreen2", AssetRequestMode.ImmediateLoad).Value;
            cve = ModContent.Request<Effect>("CalamityEntropy/Effects/cvoid", AssetRequestMode.ImmediateLoad).Value;
            cve2 = ModContent.Request<Effect>("CalamityEntropy/Effects/cvoid2", AssetRequestMode.ImmediateLoad).Value;
            pixel = Util.Util.getExtraTex("white");
            for (int i = 0; i < 10; i++)
			{
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/VoidBlade/f" + i.ToString()).Value;
            }
            Instance = this;
            AbyssalWraith.loadHead();
            CUtil.load();
			foreach (int id in CalamityLists.needsDebuffIconDisplayList)
			{
				calDebuffIconDisplayList.Add(id);
			}
            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserBody>());
            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserTail>());
            On_FilterManager.EndCapture += ec;
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:Cruiser"] = new Filter(new CrScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:Cruiser"] = new CrSky();
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:DimensionLens"] = new Filter(new TransScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:DimensionLens"] = new LlSky();
            On_Lighting.AddLight_int_int_int_float += al_iiif;
            On_Lighting.AddLight_int_int_float_float_float += al_iifff;
            On_Lighting.AddLight_Vector2_float_float_float += al_vfff;
            On_Lighting.AddLight_Vector2_Vector3 += al_vv;
            On_Lighting.AddLight_Vector2_int += al_torch;
            On_Player.AddBuff += add_buff;
            EModSys.timer = 0;
            BossRushEvent.Bosses.Insert(41, new BossRushEvent.Boss(ModContent.NPCType<CruiserHead>(), permittedNPCs: new int[] { ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() }));
            EModILEdit.load();
        }

        private void add_buff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
           if (Main.debuff[type])
            {
                if (Main.rand.NextDouble() < self.Entropy().DebuffImmuneChance)
                {
                    return;
                }
            }
           orig(self, type, timeToAdd, quiet, foodHack);
        }

        private void al_torch(On_Lighting.orig_AddLight_Vector2_int orig, Vector2 position, int torchID)
        {
            if (brillianceLightMulti > 1)
            {
                TorchID.TorchColor(torchID, out var R, out var G, out var B);
                Lighting.AddLight((int)position.X / 16, (int)position.Y / 16, R * brillianceLightMulti, G * brillianceLightMulti, B * brillianceLightMulti);
            }
            else
            {
                orig(position, torchID);

            }
        }

        public static bool BrilEnable
        {
            get
            { return Main.LocalPlayer.Entropy().brillianceCard > 0; }
            set
            { if (value) { Main.LocalPlayer.Entropy().brillianceCard = 3; } else { Main.LocalPlayer.Entropy().brillianceCard = 0; } }
        }
        public static float BrillianceCardValue = 1.5f;
        public static float OracleDeskBrilValue = 2f;
        public static float brillianceLightMulti { get { if (Main.LocalPlayer.Entropy().oracleDeck) { return OracleDeskBrilValue; }  else if (BrilEnable) { return BrillianceCardValue; } else { return 1; } } }

        private void al_vv(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            orig(position, rgb * brillianceLightMulti);
        }


        private void al_vfff(On_Lighting.orig_AddLight_Vector2_float_float_float orig, Vector2 position, float r, float g, float b)
        {
            orig(position, r * brillianceLightMulti, g * brillianceLightMulti, b * brillianceLightMulti);
        }

        private void al_iifff(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            orig(i, j, r * brillianceLightMulti, g * brillianceLightMulti, b * brillianceLightMulti);
        }

        
        private void al_iiif(On_Lighting.orig_AddLight_int_int_int_float orig, int i, int j, int torchID, float lightAmount)
        {
            orig(i, j, torchID, lightAmount * brillianceLightMulti);
        }

        private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, Func<bool> downed, object npcTypes, Dictionary<string, object> extraInfo)
            => bossChecklist.Call("LogBoss", hostMod, name, difficulty, downed, npcTypes, extraInfo);
        public override void PostSetupContent()
        {
            
            //TextureAssets.Projectile[ModContent.ProjectileType<MurasamaSlash>()] = ModContent.Request<Texture2D>("CalamityEntropy/Extra/Voidsama");
            Mod bossChecklist;
            if (ModLoader.TryGetMod("BossChecklist", out bossChecklist))
            {
                
                if (bossChecklist != null)
                {
                    {
                        string entryName = "Cruiser";
                        List<int> segments = new List<int>() { ModContent.NPCType<CruiserHead>(), ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() };
                        List<int> collection = new List<int>() { ModContent.ItemType<CruiserBag>(), ModContent.ItemType<CruiserTrophy>(), ModContent.ItemType<VoidScales>(), ModContent.ItemType<VoidMonolith>(), ModContent.ItemType<CruiserRelic>(), ModContent.ItemType<VoidRelics>(), ModContent.ItemType<PhantomPlanetKillerEngine>(), ModContent.ItemType<VoidAnnihilate>(), ModContent.ItemType<VoidElytra>(), ModContent.ItemType<VoidEcho>(), ModContent.ItemType<Silence>(), ModContent.ItemType<RuneSong>(), ModContent.ItemType<WingsOfHush>(), ModContent.ItemType<VoidToy>(), ModContent.ItemType<TheocracyPearlToy>() };
                        Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/BCL/Cruiser").Value;
                            Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                            sb.Draw(texture, centered, color);
                        };
                        Func<bool> cruiser = () => EDownedBosses.downedCruiser;
                        AddBoss(bossChecklist, CalamityEntropy.Instance, entryName, 21.7f, cruiser, segments, new Dictionary<string, object>()
                        {
                            ["displayName"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.EntryName"),
                            ["spawnInfo"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.SpawnInfo"),
                            ["despawnMessage"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.DespawnMessage"),
                            ["spawnItems"] = ModContent.ItemType<VoidBottle>(),
                            ["collectibles"] = collection,
                            ["customPortrait"] = portrait
                        });
                    }

                }
            }
        }
        public static List<Projectile> checkProj = new List<Projectile>();
        public static List<NPC> checkNPC = new List<NPC>();
        static void updateCheck()
        {
            
        }

        public Rope rope;
        public void drawRope()
        {
            Player player = Main.LocalPlayer;
            if (rope == null)
            {
                rope = new Rope(player.Center, Main.MouseWorld, 30, 5, new Vector2(0, 1f), 0.02f, 15, false);
            }
            rope.StartPos = player.Center;
            rope.EndPos = Main.MouseWorld;
            rope.Update();
            List<Vector2> points = rope.GetPoints();
            points.Add(Main.MouseWorld);
            for (int i = 1; i < points.Count; i++) {
                Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value;
                Util.Util.drawLine(Main.spriteBatch, t, points[i - 1], points[i], Color.White, 8);
            }
        }
        private void ec(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {

            Texture2D dt;
            Texture2D dt2;
            Texture2D lb;
            dt = ModContent.Request<Texture2D>("CalamityEntropy/Extra/cvmask").Value;
            dt2 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/cvmask2").Value;
            lb = ModContent.Request<Texture2D>("CalamityEntropy/Extra/lightball").Value;
            checkProj.Clear();
            checkNPC.Clear();
            foreach (Projectile p in Main.projectile)
            {
                checkProj.Add(p);
            }
            foreach (NPC n in Main.npc)
            {
                checkNPC.Add(n);
            }
            /*for (int i = checkNPC.Count - 1; i >= 0; i--)
            {
                if (!checkNPC[i].active)
                {
                    checkNPC.RemoveAt(i);
                }
            }
            for (int i = checkProj.Count - 1; i >= 0; i--)
            {
                if (!checkProj[i].active)
                {
                    checkProj.RemoveAt(i);
                }
            }*/
            /*foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.ModProjectile is Slash || proj.ModProjectile is Slash2 || proj.ModProjectile is VoidBottleThrow || proj.ModProjectile is VoidExplode || proj.ModProjectile is CruiserSlash || proj.ModProjectile is SilenceHook || proj.ModProjectile is CruiserBlackholeBullet || proj.ModProjectile is WohLaser || proj.ModProjectile is WohShot || proj.ModProjectile is VoidStar || proj.ModProjectile is VoidStarF)
                    {
                        checkProj.Add(proj);
                    }
                }
            }*/
            /*foreach (NPC npc in Main.npc)
            {
                if (npc.active)
                {
                    if (npc.type == ModContent.NPCType<CruiserHead>())
                    {
                        checkNPC.Add(npc);
                    }
                }
            }*/
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            
            if (true)
            {
                
                Vector2 sz = screensz;
                if (screen == null || sz != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    //try
                    {
                        screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screen3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screensz = new Vector2(Main.screenWidth, Main.screenHeight);
                    }
                    //catch
                    {
                    }
                }
                if (screen2 == null || sz != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    //try
                    {
                        screen2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screensz = new Vector2(Main.screenWidth, Main.screenHeight);
                    }
                    //catch
                    {
                    }
                }
                
                if (screenShakeAmp > 0)
                {
                    screenShakeAmp -= 1;
                }
            }
            
            
            
            if (screen != null)
            {
                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                cvcount += 3;

                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                

                foreach (Projectile p in checkProj)
                {
                    
                    if (!p.active)
                    {
                        continue;
                    }
                    if (p.ModProjectile is CruiserSlash)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/CruiserSlash").Value;

                        if (((CruiserSlash)p.ModProjectile).ct > 60)
                        {
                            Main.spriteBatch.Draw(tx, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 300, 0).RotatedBy(p.rotation), null, Color.White, p.rotation, new Vector2(tx.Width, tx.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / tx.Width, 1.2f), SpriteEffects.None, 0);
                        }
                    }
                    if (p.ModProjectile is SilenceHook)
                    {
                        Vector2 c = ((int)p.ai[1]).ToProj().Center;
                        Util.Util.drawChain(p.Center, c, 20, Util.Util.getExtraTex("VoidChain"));
                    }

                    if (p.ModProjectile is CruiserBlackholeBullet || p.ModProjectile is VoidBullet)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Cruiser/CruiserBlackholeBullet").Value;
                        Main.spriteBatch.Draw(tx, p.Center - Main.screenPosition, null, Color.White, p.rotation, new Vector2(tx.Width, tx.Height) / 2, p.scale, SpriteEffects.None, 0);
                    }
                    if (p.ModProjectile is VoidMonster vmnpc)
                    {
                        vmnpc.draw();
                    }

                }
                foreach (NPC n in checkNPC)
                {
                    if (!n.active)
                    {
                        continue;
                    }
                    
                    if (n.type == ModContent.NPCType<CruiserHead>() && n.active && ((CruiserHead)n.ModNPC).phaseTrans > 120 && n.ai[0] > 1)
                    {
                        Texture2D disTex = ModContent.Request<Texture2D>("CalamityEntropy/Extra/cruiserSpace2").Value;
                        Vector2 ddp = (n.Center + ((CruiserHead)n.ModNPC).bodies[((CruiserHead)n.ModNPC).bodies.Count - 1]) / 2;
                        if (((CruiserHead)n.ModNPC).aitype == 4)
                        {
                            ddp = ((CruiserHead)n.ModNPC).rotPos;
                        }
                        Main.spriteBatch.Draw(disTex, ddp - Main.screenPosition, null, Color.White * 0.1f, 0, new Vector2(disTex.Width, disTex.Height) / 2, (float)((CruiserHead)n.ModNPC).maxDistance / 900f * 2 - 0.01f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(disTex, ddp - Main.screenPosition, null, Color.White, 0, new Vector2(disTex.Width, disTex.Height) / 2, (float)((CruiserHead)n.ModNPC).maxDistance / 900f * 2, SpriteEffects.None, 0);

                    }
                }
                foreach (Particle pt in VoidParticles.particles)
                {
                    Texture2D draw = dt;
                    float sc = 1;
                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White * 0.06f, pt.rotation, dt.Size() / 2, 5.4f * pt.alpha * sc + 0.6f, SpriteEffects.None, 0);

                }
                foreach (Particle pt in VoidParticles.particles)
                {
                    Texture2D draw = dt;
                    float sc = 1;
                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White, pt.rotation, dt.Size() / 2, 5.4f * pt.alpha * sc, SpriteEffects.None, 0);

                }
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle pt in VoidParticles.particles)
                {
                    if (!(pt.shape == 4))
                    {
                        continue;
                    }
                    Texture2D draw = Util.Util.getExtraTex("cvdt");
                    float sc = 1;

                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White, pt.rotation, draw.Size() / 2, 5.4f * pt.alpha * sc * 0.16f, SpriteEffects.None, 0);

                }
                Main.spriteBatch.End();
                graphicsDevice.SetRenderTarget(screen3);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None
                    , RasterizerState.CullNone);
                kscreen2.CurrentTechnique = kscreen2.Techniques["Technique1"];
                kscreen2.CurrentTechnique.Passes[0].Apply();
                kscreen2.Parameters["tex0"].SetValue(screen2);
                kscreen2.Parameters["tex1"].SetValue(Util.Util.getExtraTex("EternityStreak"));
                kscreen2.Parameters["offset"].SetValue(Main.screenPosition / Main.ScreenSize.ToVector2());
                kscreen2.Parameters["i"].SetValue(0.04f);
                Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
                cve.CurrentTechnique = cve.Techniques["Technique1"];
                cve.CurrentTechnique.Passes[0].Apply();
                cve.Parameters["tex0"].SetValue(screen3);
                Texture2D backg = ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_blue_base").Value;//ModContent.Request<Texture2D>("CalamityEntropy/Extra/Backg").Value;
                /*cve.Parameters["tex1"].SetValue(backg);
                cve.Parameters["tex2"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/Backg1").Value);
                cve.Parameters["tex3"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/Backg2").Value);*/
                cve.Parameters["tex1"].SetValue(backg);
                cve.Parameters["tex2"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_starfield_1").Value);
                cve.Parameters["tex3"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_starfield_2").Value);
                cve.Parameters["tex4"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_starfield_3").Value);
                cve.Parameters["tex5"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_starfield_4").Value);
                cve.Parameters["tex6"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/planetarium_starfield_5").Value);
                cve.Parameters["time"].SetValue((float)cvcount / 50f);
                cve.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2());
                cve.Parameters["offset"].SetValue((Main.screenPosition * Main.GameViewMatrix.Zoom + new Vector2(-cvcount / 6f, cvcount / 6f)) / Main.ScreenSize.ToVector2());
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                //2号shader
                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                //绘制
                
                foreach (Projectile p in checkProj)
                {
                    if (!p.active)
                    {
                        continue;
                    }
                    if (p.active && p.type == ModContent.ProjectileType<WohShot>() && false)
                    {
                        WohShot mp = (WohShot)p.ModProjectile;
                        if (mp.odp.Count > 1)
                        {
                            List<Vertex> ve = new List<Vertex>();
                            Color b = new Color(75, 125, 255);

                            float a = 0;
                            float lr = 0;
                            for (int i = 1; i < mp.odp.Count; i++)
                            {
                                a += 1f / (float)mp.odp.Count;

                                ve.Add(new Vertex(vLToCenter(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18, Main.GameViewMatrix.Zoom.X),
                                      new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                                      b * a));
                                ve.Add(new Vertex(vLToCenter(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18, Main.GameViewMatrix.Zoom.X),
                                      new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                                      b * a));
                                lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                            }
                            a = 1;/*
                            ve.Add(new Vertex(vLToCenter(p.position - mp.dscp + lr.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 26, Main.GameViewMatrix.Zoom.X),
                                      new Vector3((float)0, 1, 1),
                                      b));
                            ve.Add(new Vertex(vLToCenter(p.position - mp.dscp + lr.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 26, Main.GameViewMatrix.Zoom.X),
                                  new Vector3((float)0, 0, 1),
                                  b));*/
                            GraphicsDevice gd = Main.graphics.GraphicsDevice;
                            if (ve.Count >= 3)//因为顶点需要围成一个三角形才能画出来 所以需要判顶点数>=3 否则报错
                            {
                                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Extra/wohslash").Value;
                                gd.Textures[0] = tx;
                                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                            }
                            

                        }
                    }
                    if (p.ModProjectile is AbyssalLaser al)
                    {
                        al.drawLaser();
                    }
                    if (p.ModProjectile is WohLaser)
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
                        Texture2D tx = Util.Util.getExtraTex("wohlaser");
                        int drawCount = (int)(2400f * p.scale / tx.Width) + 1;
                        for (int i = 0; i < drawCount; i++)
                        {
                            Main.spriteBatch.Draw(tx, opos - Main.screenPosition, null, new Color(55, 100, 255) * alp, p.velocity.ToRotation(), new Vector2(0, tx.Height / 2), new Vector2(1, w * 1f), SpriteEffects.None, 0);
                            opos += p.velocity.SafeNormalize(Vector2.One) * tx.Width;
                        }
                    }
                    if (p.active)
                    {
                        if (p.ModProjectile is VoidStar)
                        {
                            if (p.ai[0] >= 60 || p.ai[2] == 0)
                            {
                                VoidStar mp = (VoidStar)p.ModProjectile;
                                mp.odp.Add(p.Center);
                                if (mp.odp.Count > 2)
                                {
                                    float size = 10;
                                    float sizej = size / mp.odp.Count;
                                    Color cl = new Color(100, 200, 255);
                                    for (int i = mp.odp.Count - 1; i >= 1; i--)
                                    {
                                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * ((float)i / (float)mp.odp.Count) * (((float)(255 - p.alpha)) / 255f), size);
                                        size -= sizej;
                                    }
                                }
                                mp.odp.RemoveAt(mp.odp.Count - 1);
                            }

                        }
                        if (p.ModProjectile is VoidStarF)
                        {
                            VoidStarF mp = (VoidStarF)p.ModProjectile;
                            mp.odp.Add(p.Center);
                            if (mp.odp.Count > 2)
                            {
                                float size = 10;
                                float sizej = size / mp.odp.Count;
                                Color cl = new Color(100, 200, 255);
                                for (int i = mp.odp.Count - 1; i >= 1; i--)
                                {
                                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * ((float)i / (float)mp.odp.Count) * (((float)(255 - p.alpha)) / 255f), size);
                                    size -= sizej;
                                }
                            }
                            mp.odp.RemoveAt(mp.odp.Count - 1);
                        }
                    }
                }
                /*                Main.spriteBatch.Draw(lb, new Vector2(400, 200), null, new Color(45, 75, 170), 0, Vector2.Zero, 6, SpriteEffects.None, 0);
                                Main.spriteBatch.Draw(lb, new Vector2(400, 200), null, new Color(45, 75, 170), 0, Vector2.Zero, 6, SpriteEffects.None, 0);
                                Main.spriteBatch.Draw(lb, new Vector2(400, 200), null, new Color(45, 75, 170), 0, Vector2.Zero, 6, SpriteEffects.None, 0);
                                Main.spriteBatch.Draw(lb, new Vector2(400, 200), null, new Color(45, 75, 170), 0, Vector2.Zero, 6, SpriteEffects.None, 0);
                */
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Player p in Main.player)
                {
                    //try
                    {
                        if (p.active && !p.dead && p.Entropy().MagiShield > 0)
                        {
                            Texture2D shieldTexture = Util.Util.getExtraTex("shield");
                            Main.spriteBatch.Draw(shieldTexture, p.Center - Main.screenPosition, null, new Color(186, 120, 255), 0, shieldTexture.Size() / 2, 0.47f, SpriteEffects.None, 0);

                        }
                    }
                    //catch { }
                }
                foreach (Projectile p in checkProj)
                {
                    if (p.active)
                    {
                        if (p.ModProjectile is MoonlightShieldBreak)
                        {
                            Texture2D shieldTexture = Util.Util.getExtraTex("shield");
                            Main.spriteBatch.Draw(shieldTexture, p.Center - Main.screenPosition, null, new Color(186, 120, 255) * p.ai[2], 0, shieldTexture.Size() / 2, 0.47f * (1 + p.ai[1]), SpriteEffects.None, 0);

                        }
                        if (p.ModProjectile is CruiserShadow aw)
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
                foreach (NPC n in checkNPC)
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
                cve2.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/VoidBack").Value);
                cve2.Parameters["time"].SetValue((float)cvcount / 50f);
                cve2.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(cvcount * 1.4f, cvcount * 1.4f)) / new Vector2(1920, 1080));
                Main.spriteBatch.Draw(screen, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);
                Main.spriteBatch.End();

            }
            if (screen2 != null)
            {
                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                //try
                {
                    foreach (Player player in Main.player)
                    {
                        if (player.active && !player.dead && player.Entropy().daPoints.Count > 2)
                        {
                            float scj = 1f / (float)player.Entropy().daPoints.Count;
                            float sc = scj;
                            Color color = Color.Black;
                            if (player.Entropy().VaMoving > 0)
                            {
                                color = Color.Blue;
                            }
                            for (int i = 1; i < player.Entropy().daPoints.Count; i++)
                            {

                                Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, player.Entropy().daPoints[i - 1], player.Entropy().daPoints[i], color * 0.6f, 12 * sc, 0);
                                sc += scj;
                            }
                        }
                    }


                }
                //catch
                {

                }
                



                foreach (Projectile p in checkProj)
                {
                    if (!p.active)
                    {
                        continue;
                    }
                    if (p.ModProjectile is VoidBottleThrow || p.ModProjectile is CruiserShadow)
                    {
                        Color color = Color.White;
                        p.ModProjectile.PreDraw(ref color);
                    }
                    if (p.ModProjectile is VoidWraith vw)
                    {
                        vw.draw();
                    }
                    if (p.ModProjectile is AbyssPet || p.ModProjectile is VoidPalProj)
                    {
                        Color color = Color.White;
                        p.ModProjectile.PreDraw(ref color);
                    }
                    if (p.ModProjectile is ShadewindLanceThrow sp)
                    {
                        sp.draw();
                    }
                    if (p.ModProjectile is VoidStar || p.ModProjectile is VoidStarF)
                    {
                        Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Cruiser/VoidStar").Value;

                        Main.spriteBatch.Draw(t, p.Center - Main.screenPosition, null, Color.White * (((float)(255 - p.alpha)) / 255f), p.rotation, t.Size() / 2, p.scale, SpriteEffects.None, 0);

                    }
                    
                }

                //drawRope();

                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                
                Main.spriteBatch.Draw(screen2, Vector2.Zero, Color.White);
                Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);

                Main.spriteBatch.End();

            }
            if (screen != null && screen2 != null)
            {
                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Texture2D kt = ModContent.Request<Texture2D>("CalamityEntropy/Extra/ksc1").Value;
                Texture2D kt2 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/kscc").Value;
                Texture2D st = ModContent.Request<Texture2D>("CalamityEntropy/Extra/kslash").Value;

                foreach (Projectile p in checkProj)
                {
                    if (p.active)
                    {
                        /*
                        if (p.type == ModContent.ProjectileType<VoidMark>())
                        {
                            Main.spriteBatch.Draw(kt, p.Center - Main.screenPosition, null, Color.White, 0, new Vector2(kt.Width, kt.Height) / 2, 1.2f, SpriteEffects.None, 0);
                        }
                        */
                        if (p.ModProjectile is Slash)
                        {
                            Main.spriteBatch.Draw(kt, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 165, 0).RotatedBy(p.rotation), null, Color.White, p.rotation + (float)Math.PI / 2, new Vector2(kt.Width, kt.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / kt.Width * 0.4f, 0.1f), SpriteEffects.None, 0);

                        }
                        if (p.ModProjectile is Slash2)
                        {
                            Main.spriteBatch.Draw(kt, p.Center - Main.screenPosition + new Vector2((p.ai[0] + p.ai[1]) / 2 - 300, 0).RotatedBy(p.rotation), null, Color.White, p.rotation + (float)Math.PI / 2, new Vector2(kt.Width, kt.Height) / 2, new Vector2((p.ai[0] - p.ai[1]) / kt.Width * 1.4f, 1f), SpriteEffects.None, 0);

                        }


                        if (p.ModProjectile is VoidBottleThrow)
                        {
                            Color color = Color.White;
                            //p.ModProjectile.PreDraw(ref color);
                        }
                        if (p.ModProjectile is VoidExplode)
                        {
                            float ks = (float)p.timeLeft * 0.1f;
                            if (p.timeLeft > 10)
                            {
                                ks = (20 - (float)p.timeLeft) / 10f;
                            }
                            ks *= (1 + p.ai[1]);
                            Main.spriteBatch.Draw(kt, p.Center - Main.screenPosition, null, Color.White, 0, new Vector2(kt.Width, kt.Height) / 2, ks * 2, SpriteEffects.None, 0);

                        }
                        if (p.ModProjectile is VoidRExp)
                        {
                            float ks = (90f - (float)p.timeLeft) * 0.4f;
                            
                            Main.spriteBatch.Draw(kt2, p.Center - Main.screenPosition, null, Color.White, 0, new Vector2(kt2.Width, kt2.Height) / 2, ks, SpriteEffects.None, 0);

                        }
                    }
                }

                //Main.spriteBatch.Draw(Util.Util.getExtraTex("EternityStreak"), new Vector2(0, 0), new Rectangle((int)cvcount * 2, 0, Main.screenWidth, Main.screenHeight), new Color(100, 255, 255), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

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



                /*graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle pt in VoidParticles.particles)
                {
                    if (!(pt.shape == 4))
                    {
                        continue;
                    }
                    Texture2D draw = Util.Util.getExtraTex("cvdt");
                    float sc = 1;
                    
                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White, pt.rotation, draw.Size() / 2, 5.4f * pt.alpha * sc * 0.16f, SpriteEffects.None, 0);

                }
                Main.spriteBatch.End();
                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None
                    , RasterizerState.CullNone);
                kscreen2.CurrentTechnique = kscreen2.Techniques["Technique1"];
                kscreen2.CurrentTechnique.Passes[0].Apply();
                kscreen2.Parameters["tex0"].SetValue(Main.screenTargetSwap);
                kscreen2.Parameters["tex1"].SetValue(Util.Util.getExtraTex("EternityStreak"));
                kscreen2.Parameters["offset"].SetValue(Main.screenPosition / Main.ScreenSize.ToVector2());
                kscreen2.Parameters["i"].SetValue(0.07f);
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();*/

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (NPC npc in checkNPC)
                {
                    if (npc.active)
                    {

                        if (npc.type == ModContent.NPCType<AbyssalWraith>() && npc.ModNPC is AbyssalWraith)
                        {
                            ((AbyssalWraith)npc.ModNPC).Draw();
                        }
                        if (npc.type == ModContent.NPCType<CruiserHead>() && npc.ModNPC is CruiserHead)
                        {

                            if (((CruiserHead)npc.ModNPC).phase == 2)
                            {
                                ((CruiserHead)npc.ModNPC).candraw = true;
                                ((CruiserHead)npc.ModNPC).PreDraw(Main.spriteBatch, Main.screenPosition, Color.White);
                                ((CruiserHead)npc.ModNPC).candraw = false;
                            }
                            /*if (npc.ai[0] > 0 && npc.ai[0] < 100)
                            {
                                Texture2D ctt = ModContent.Request<Texture2D>("CalamityEntropy/Extra/cruiser_title").Value;
                                float alpha = 1;
                                if (npc.ai[0] < 20)
                                    alpha = npc.ai[0] / 20f;
                                if (npc.ai[0] > 80)
                                {
                                    alpha = (100 - npc.ai[0]) / 20f;
                                }
                                Main.spriteBatch.Draw(ctt, new Vector2(Main.screenWidth, Main.screenHeight) / 2, null, Color.White * alpha, 0, ctt.Size() / 2, 3.5f, SpriteEffects.None, 0);
                            }*/
                        }
                    }
                }
                Main.spriteBatch.End();
            }
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        public static Vector2 vLToCenter(Vector2 v, float z)
        {
            return Main.ScreenSize.ToVector2() / 2 + (v - Main.ScreenSize.ToVector2() / 2) * z;
        }
        
        public override void Unload()
        {
            WallpaperHelper.wallpaper = null;
            checkProj = null;
            checkNPC = null;
            kscreen = null;
            kscreen2 = null;
            cve = null;
            cve2 = null;
            Instance = null;
            pixel = null;
            screen = null;
            screen2 = null;
            On_FilterManager.EndCapture -= ec;
            On_Lighting.AddLight_int_int_int_float -= al_iiif;
            On_Lighting.AddLight_int_int_float_float_float -= al_iifff;
            On_Lighting.AddLight_Vector2_float_float_float -= al_vfff;
            On_Lighting.AddLight_Vector2_Vector3 -= al_vv;
            On_Lighting.AddLight_Vector2_int -= al_torch;
        }

    }
}
