global using Microsoft.Xna.Framework;
using CalamityEntropy.Common;
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.ILEditing;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.MusicBoxes;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Content.NPCs.Acropolis;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Content.NPCs.LuminarisMoth;
using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityEntropy.Content.NPCs.Prophet;
using CalamityEntropy.Content.NPCs.VoidInvasion;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Skies;
using CalamityEntropy.Content.UI;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using InnoVault;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;
namespace CalamityEntropy
{
    public partial class CalamityEntropy : Mod
    {

        internal static List<ICELoader> ILoaders { get; private set; }
        public static ref bool EntropyMode => ref EDownedBosses.EntropyMode;
        public static bool AprilFool = false;
        public static List<int> calDebuffIconDisplayList = new List<int>();
        public static CalamityEntropy Instance;
        public static int noMusTime = 0;
        public static Effect kscreen;
        public static Effect kscreen2;
        public static Effect cve;
        public static Effect cve2;
        public static Effect cab = null;
        public static RenderTarget2D screen = null;
        public static RenderTarget2D screen2 = null;
        public static RenderTarget2D screen3 = null;
        public float screenShakeAmp = 0;
        public float cvcount = 0;
        public static bool ets = true;
        public static Texture2D pixel;
        public ArmorForgingStationUI armorForgingStationUI;
        public UserInterface userInterface;
        public static DynamicSpriteFont efont1;
        public static DynamicSpriteFont efont2;
        public static float cutScreenVel = 0;
        public static float cutScreen = 0;
        public static float cutScreenRot = 0;
        public static Vector2 cutScreenCenter = Vector2.Zero;
        public bool ChristmasEvent = false;
        public static float FlashEffectStrength = 0;
        public static Dictionary<int, Projectile> Proj_ID_To_Instance { get; set; } = null;
        public static List<Projectile> CheckProjs { get; set; } = [];
        public static List<NPC> CheckNPCs { get; set; } = [];
        public Rope Rope { get; set; }
        public static SoundEffect ealaserSound = null;
        public static SoundEffect ealaserSound2 = null;
        public static SoundEffect ofCharge = null;
        public string EntropyWikiURL;
        public override void Load()
        {
            CEUtils.TexCache = new Dictionary<string, Texture2D>();

            BookMarkLoader.CustomBMEffectsByName = new Dictionary<string, BookMarkLoader.BookmarkEffectFunctionGroups>();
            BookMarkLoader.CustomBMByID = new Dictionary<int, BookMarkLoader.BookMarkTag>();
            Instance = this;
            Proj_ID_To_Instance = new Dictionary<int, Projectile>();
            DateTime today = DateTime.Now;
            AprilFool = today.Month == 4 && today.Day == 1;


            ILoaders = VaultUtils.GetSubInterface<ICELoader>();
            foreach (ICELoader setup in ILoaders)
            {
                setup.LoadData();
                setup.DompLoadText();
            }
            LoopSoundManager.init();

            efont1 = ModContent.Request<DynamicSpriteFont>("CalamityEntropy/Assets/Fonts/EFont", AssetRequestMode.ImmediateLoad).Value;
            efont2 = ModContent.Request<DynamicSpriteFont>("CalamityEntropy/Assets/Fonts/VCRFont", AssetRequestMode.ImmediateLoad).Value;
            CERecipeGroups.init();
            armorForgingStationUI = new ArmorForgingStationUI();
            armorForgingStationUI.Activate();
            userInterface = new UserInterface();
            userInterface.SetState(armorForgingStationUI);
            EnchantmentManager.ItemUpgradeRelationship[ModContent.ItemType<VoidEcho>()] = ModContent.ItemType<Mercy>();
            ets = true;
            kscreen = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/kscreen", AssetRequestMode.ImmediateLoad).Value;
            kscreen2 = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/kscreen2", AssetRequestMode.ImmediateLoad).Value;
            cve = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/cvoid", AssetRequestMode.ImmediateLoad).Value;
            cab = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/cabyss", AssetRequestMode.ImmediateLoad).Value;
            cve2 = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/cvoid2", AssetRequestMode.ImmediateLoad).Value;
            pixel = CEUtils.getExtraTex("white");

            AbyssalWraith.loadHead();
            CruiserHead.loadHead();

            foreach (int id in CalamityLists.needsDebuffIconDisplayList)
            {
                calDebuffIconDisplayList.Add(id);
            }

            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserBody>());
            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserTail>());
            EntropySkies.setUpSkies();
            EffectLoader.Load();
            On_MapHeadRenderer.DrawPlayerHead += drawPlayerHeadHook;
            On_Lighting.AddLight_int_int_int_float += al_iiif;
            On_Lighting.AddLight_int_int_float_float_float += al_iifff;
            On_Lighting.AddLight_Vector2_float_float_float += al_vfff;
            On_Lighting.AddLight_Vector2_Vector3 += al_vv;
            On_Lighting.AddLight_Vector2_int += al_torch;
            On_Player.AddBuff += add_buff;
            On_NPC.AddBuff += add_buff_npc;
            On_NPC.TargetClosest += targetClost;
            On_NPC.TargetClosestUpgraded += targetClostUpgraded;
            On_NPC.FindFrame += findFrame;
            On_NPC.VanillaAI += vAi;
            On_NPC.UpdateNPC += npcupdate;
            On_NPC.StrikeNPC_HitInfo_bool_bool += StrikeNpc;
            On_Player.getRect += modifyRect;
            On_Main.DrawInfernoRings += drawIr;
            On_Main.DrawProjectiles += DrawBehindPlayer;
            On_Main.DrawMenu += drawmenu;
            On_Player.Heal += player_heal;
            On_Main.DrawTiles += drawtile;
            On_Projectile.FillWhipControlPoints += fill_whip_ctrl_points_hook;
            On_Projectile.GetWhipSettings += get_whip_settings_hook;
            
            On_Player.ApplyDamageToNPC += applydamagetonpc;
            On_Main.DrawCursor += draw_cursor_hook;
            On_Main.DrawThickCursor += draw_thick_cursor_hook;

            EModSys.timer = 0;
            BossRushEvent.Bosses.Insert(35, new BossRushEvent.Boss(ModContent.NPCType<NihilityActeriophage>(), permittedNPCs: new int[] { ModContent.NPCType<ChaoticCell>() }));
            BossRushEvent.Bosses.Insert(42, new BossRushEvent.Boss(ModContent.NPCType<CruiserHead>(), permittedNPCs: new int[] { ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() }));
            BossRushEvent.Bosses.Insert(28, new BossRushEvent.Boss(ModContent.NPCType<TheProphet>()));
            BossRushEvent.Bosses.Insert(24, new BossRushEvent.Boss(ModContent.NPCType<Luminaris>()));

            EModILEdit.load();
            Type baseType = typeof(PlayerDashEffect);
            Type[] types = AssemblyManager.GetLoadableTypes(this.Code);
            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(baseType) || type.IsAbstract)
                    continue;

                string id = (string)type.GetProperty("ID").GetValue(null);

                PlayerDashEffect dashEffect = (PlayerDashEffect)Activator.CreateInstance(type);
                PlayerDashManager.TryAddDash(dashEffect);
            }


        }

        private void drawPlayerHeadHook(On_MapHeadRenderer.orig_DrawPlayerHead orig, MapHeadRenderer self, Camera camera, Player drawPlayer, Vector2 position, float alpha, float scale, Color borderColor)
        {
            if(true)
            {
                int origHead = drawPlayer.head;
                drawPlayer.head = EquipLoader.GetEquipSlot(CalamityEntropy.Instance, "AbyssLantern", EquipType.Head);

                orig(self, camera, drawPlayer, position, alpha, scale, borderColor);

                drawPlayer.head = origHead;
            }
            else
            {
                orig(self, camera, drawPlayer, position, alpha, scale, borderColor);
            }
        }

        private void draw_cursor_hook(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            if (!EModSys.mi)
            {
                orig(bonus, smart);
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) => CENetWork.Handle(reader, whoAmI);

        private void get_whip_settings_hook(On_Projectile.orig_GetWhipSettings orig, Projectile proj, out float timeToFlyOut, out int segments, out float rangeMultiplier)
        {
            orig(proj, out timeToFlyOut, out segments, out rangeMultiplier);
            if (proj.ModProjectile != null && proj.ModProjectile is BaseWhip bw)
            {
                bw.ModifyWhipSettings(ref timeToFlyOut, ref segments, ref rangeMultiplier);
            }
        }

        private void fill_whip_ctrl_points_hook(On_Projectile.orig_FillWhipControlPoints orig, Projectile proj, List<Vector2> controlPoints)
        {
            orig(proj, controlPoints);
            if (proj.ModProjectile != null && proj.ModProjectile is BaseWhip bw)
            {
                bw.ModifyControlPoints(controlPoints);
            }
        }

        public static Projectile GetAProjectileInstance(int type)
        {
            if (!Proj_ID_To_Instance.ContainsKey(type))
            {
                Projectile p = new Projectile();
                p.SetDefaults(type);
                Proj_ID_To_Instance[type] = p;
            }
            return Proj_ID_To_Instance[type];
        }

        public override void Unload()
        {
            EModILEdit.edgeTex = null;
            if (ILoaders != null)
            {
                foreach (ICELoader setup in ILoaders)
                {
                    setup.UnLoadData();
                    setup.DompUnLoadText();
                }
            }
            ILoaders = null;
            CERecipeGroups.unload();
            CEUtils.TexCache = null;
            BookMarkLoader.CustomBMEffectsByName = null;
            BookMarkLoader.CustomBMByID = null;
            screen = null;
            screen2 = null;
            screen3 = null;
            Proj_ID_To_Instance = null;
            EModHooks.UnLoadData();
            LoopSoundManager.unload();
            ealaserSound = null;
            ealaserSound2 = null;
            ArmorPrefix.instances = null;
            Poop.instances = null;
            WallpaperHelper.wallpaper = null;
            efont1 = null;
            efont2 = null;
            CheckProjs = null;
            CheckNPCs = null;
            kscreen = null;
            kscreen2 = null;
            cve = null;
            cve2 = null;
            Instance = null;
            pixel = null;
            screen = null;
            screen2 = null;
            EffectLoader.UnLoad();
            On_MapHeadRenderer.DrawPlayerHead -= drawPlayerHeadHook;
            On_Lighting.AddLight_int_int_int_float -= al_iiif;
            On_Lighting.AddLight_int_int_float_float_float -= al_iifff;
            On_Lighting.AddLight_Vector2_float_float_float -= al_vfff;
            On_Lighting.AddLight_Vector2_Vector3 -= al_vv;
            On_Lighting.AddLight_Vector2_int -= al_torch;
            On_Player.AddBuff -= add_buff;
            On_NPC.AddBuff -= add_buff_npc;
            On_NPC.TargetClosest -= targetClost;
            On_NPC.TargetClosestUpgraded -= targetClostUpgraded;
            On_NPC.FindFrame -= findFrame;
            On_NPC.VanillaAI -= vAi;
            On_NPC.UpdateNPC -= npcupdate;
            On_NPC.StrikeNPC_HitInfo_bool_bool -= StrikeNpc;
            On_Player.getRect -= modifyRect;
            On_Main.DrawInfernoRings -= drawIr;
            On_Main.DrawProjectiles -= DrawBehindPlayer;
            On_Main.DrawMenu -= drawmenu;
            On_Player.Heal -= player_heal;
            On_Main.DrawTiles -= drawtile;
            On_Projectile.FillWhipControlPoints -= fill_whip_ctrl_points_hook;
            On_Projectile.GetWhipSettings -= get_whip_settings_hook;
            On_Player.ApplyDamageToNPC -= applydamagetonpc;
            On_Main.DrawCursor -= draw_cursor_hook;
            On_Main.DrawThickCursor -= draw_thick_cursor_hook;
        }

        private Vector2 draw_thick_cursor_hook(On_Main.orig_DrawThickCursor orig, bool smart)
        {
            if (!EModSys.mi)
            {
                return orig(smart);
            }
            return Vector2.Zero;
        }

        private void applydamagetonpc(On_Player.orig_ApplyDamageToNPC orig, Player self, NPC n, int damage, float knockback, int direction, bool crit, DamageClass damageType, bool damageVariation)
        {
            orig(self, n, damage, knockback, direction, crit, damageType, damageVariation);
            n.gimmune().readySyncDashImmune = true;
            n.gimmune().sdPlayer = self;
        }

        private void add_buff_npc(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            if (!(Main.debuff[type] && self.ModNPC is AbyssalWraith))
            {
                orig(self, type, time, quiet);
            }
        }

        public void drawtile(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }

        private void player_heal(On_Player.orig_Heal orig, Player self, int amount)
        {
            if (!self.HasBuff<VoidVirus>() && !(EntropyMode && self.Entropy().HitTCounter > 0))
            {
                orig(self, amount);
            }
        }

        private void DrawBehindPlayer(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.begin_();
            Texture2D shell = CEUtils.getExtraTex("shell");
            Texture2D crystalShield = CEUtils.getExtraTex("MariviniumShield");
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.Entropy().nihShellCount > 0)
                {
                    float rot = player.Entropy().CasketSwordRot * 0.2f;
                    int count = player.Entropy().nihShellCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (rot.ToRotationVector2().Y < 0)
                        {
                            Vector2 center = new Vector2(36, 0).RotatedBy(rot);
                            center.Y = 0;
                            float sizeX = Math.Abs(new Vector2(56, 0).RotatedBy(rot + 0.3f).X - new Vector2(56, 0).RotatedBy(rot - 0.3f).X);
                            Main.spriteBatch.Draw(shell, player.Center + player.gfxOffY * Vector2.UnitY - Main.screenPosition + center, null, Color.White * 0.8f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / count;
                    }
                }
                if (player.Entropy().MariviniumShieldCount > 0)
                {
                    float rot = player.Entropy().CasketSwordRot * -0.2f;
                    int count = player.Entropy().MariviniumShieldCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (rot.ToRotationVector2().Y < 0)
                        {
                            Vector2 center = new Vector2(48, 0).RotatedBy(rot);
                            center.Y = 0;
                            float sizeX = Math.Abs(new Vector2(56, 0).RotatedBy(rot + 0.3f).X - new Vector2(56, 0).RotatedBy(rot - 0.3f).X);
                            Main.spriteBatch.Draw(crystalShield, player.Center + player.gfxOffY * Vector2.UnitY - Main.screenPosition + center, null, Color.White * 0.6f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / count;
                    }
                }
            }
            Main.spriteBatch.End();
        }
        public float AzShieldBarAlpha = 0;
        private void drawIr(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);

            // I'm assuming these are not needed, as they're handled in EffectLoader.EnsureRenderTargets and other methods.
            // Why is screen2 not here?
            /*
            screen?.Dispose();
            screen = null;
            screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            screen3?.Dispose();
            screen3 = null;
            screen3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);*/

            Texture2D shell = CEUtils.getExtraTex("shell");
            Texture2D crystalShield = CEUtils.getExtraTex("MariviniumShield");
            if (Main.LocalPlayer.Entropy().AzafureChargeShieldItem != null)
            {
                var mi = Main.LocalPlayer.Entropy().AzafureChargeShieldItem.ModItem as AzafureChargeShield;
                float charge = mi.charge;
                float maxCharge = mi.maxCharge;
                if (charge >= maxCharge)
                {
                    AzShieldBarAlpha = float.Lerp(AzShieldBarAlpha, 0, 0.1f);
                }
                else
                {
                    AzShieldBarAlpha = float.Lerp(AzShieldBarAlpha, 1, 0.1f);
                }
                CEUtils.DrawChargeBar(1.5f, Main.LocalPlayer.Center - Main.screenPosition + new Vector2(0, -42), ((float)charge / maxCharge), ((charge > 1) ? Color.Lerp(Color.OrangeRed, Color.Orange, (float)Math.Cos(Main.GameUpdateCount * 0.2f) * 0.5f + 0.5f) : Color.Firebrick) * AzShieldBarAlpha);
            }
            else
            {
                AzShieldBarAlpha = float.Lerp(AzShieldBarAlpha, 0, 0.1f);
            }
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.Entropy().nihShellCount > 0)
                {
                    float rot = player.Entropy().CasketSwordRot * 0.2f;
                    int count = player.Entropy().nihShellCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (rot.ToRotationVector2().Y > 0)
                        {
                            Vector2 center = new Vector2(36, 0).RotatedBy(rot);
                            center.Y = 0;
                            float sizeX = Math.Abs(new Vector2(56, 0).RotatedBy(rot + 0.3f).X - new Vector2(56, 0).RotatedBy(rot - 0.3f).X);
                            Main.spriteBatch.Draw(shell, player.Center + player.gfxOffY * Vector2.UnitY - Main.screenPosition + center, null, Color.White * 0.8f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / count;
                    }
                }
                if (player.Entropy().MariviniumShieldCount > 0)
                {
                    float rot = player.Entropy().CasketSwordRot * -0.2f;
                    int count = player.Entropy().MariviniumShieldCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (rot.ToRotationVector2().Y > 0)
                        {
                            Vector2 center = new Vector2(48, 0).RotatedBy(rot);
                            center.Y = 0;
                            float sizeX = Math.Abs(new Vector2(56, 0).RotatedBy(rot + 0.3f).X - new Vector2(56, 0).RotatedBy(rot - 0.3f).X);
                            Main.spriteBatch.Draw(crystalShield, player.Center + player.gfxOffY * Vector2.UnitY - Main.screenPosition + center, null, Color.White * 0.6f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / count;
                    }
                }
                if (pocType == -1)
                {
                    pocType = ModContent.ProjectileType<PrisonOfPermafrostCircle>();
                }
                else
                {
                    if (player.ownedProjectileCounts[pocType] > 0)
                    {
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.type == pocType && p.owner == player.whoAmI)
                            {
                                if (p.ModProjectile is PrisonOfPermafrostCircle poc)
                                {
                                    float alpha = poc.usingTime / 60f;
                                    if (alpha > 1)
                                    {
                                        alpha = 1;
                                    }
                                    Main.spriteBatch.Draw(poc.itemTex, p.Center + p.rotation.ToRotationVector2() * 28 - Main.screenPosition, null, Color.White * alpha, p.rotation + MathHelper.PiOver2, poc.itemTex.Size() / 2, p.scale * 0.5f, SpriteEffects.None, 0);

                                    break;
                                }
                            }
                        }
                    }
                }
            }


            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            Main.spriteBatch.End();


            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            EParticle.drawAll();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform); orig(self);

        }
        public int pocType = -1;
        private void drawmenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            EModSys.mi = false;
            if (LoopSoundManager.sounds != null)
            {
                if (LoopSoundManager.sounds.Count > 0)
                {
                    for (int i = 0; i < LoopSoundManager.sounds.Count; i++)
                    {
                        var sound = LoopSoundManager.sounds[i];
                        sound.stop();
                    }
                    LoopSoundManager.sounds.Clear();
                }
            }
        }

        private void npcupdate(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (self == null || self.type <= NPCID.None)
            {
                return;
            }

            //很显然不活跃的NPC不符合我们的期望
            if (!self.active || !self.TryGetGlobalNPC<EGlobalNPC>(out var ceNPC))
            {
                orig(self, i);
                return;
            }

            if (self.active && self.Entropy().AnimaTrapped > 0)
            {
                ceNPC.AnimaTrapped--;
                self.position += self.velocity;
                self.velocity *= 0.9f;
                for (int ii = 0; ii < self.immune.Length; ii++)
                {
                    if (self.immune[ii] > 0)
                    {
                        self.immune[ii]--;
                    }
                }
            }
            else
            {
                if (self.active && self.TryGetGlobalNPC<DeliriumGlobalNPC>(out var deliriumNPC) && deliriumNPC.delirium)
                {
                    NPC npc = self;
                    npc.damage = deliriumNPC.damage;
                    deliriumNPC.counter--;
                    if (deliriumNPC.counter <= 0)
                    {
                        if (!Main.dedServ)
                        {
                            CEUtils.PlaySound("clicker_static", 1, npc.Center);
                        }
                        deliriumNPC.counter = Main.rand.Next(60, 360);
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                        int npc_ = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, Delirium.npcTurns[Main.rand.Next(Delirium.npcTurns.Count)]);
                        NPC spawn = npc_.ToNPC();
                        spawn.Center = npc.Center;
                        spawn.lifeMax = npc.lifeMax;
                        spawn.life = npc.life;
                        spawn.damage = npc.damage;
                        spawn.GetGlobalNPC<DeliriumGlobalNPC>().delirium = true;
                        spawn.GetGlobalNPC<DeliriumGlobalNPC>().damage = deliriumNPC.damage;
                        spawn.GetGlobalNPC<DeliriumGlobalNPC>().counter = deliriumNPC.counter;
                        spawn.netUpdate = true;
                        spawn.netSpam = 0;
                        npc.active = false;
                    }
                    if (npc.type != NPCID.DukeFishron && npc.type != ModContent.NPCType<OldDuke>() && npc.type != NPCID.Golem && npc.type != ModContent.NPCType<Bumblefuck>() && npc.type != NPCID.SkeletronHead)
                    {
                        orig(self, i);
                        if (npc.type != NPCID.EyeofCthulhu && npc.type != NPCID.QueenBee && npc.type != NPCID.Retinazer && npc.type != NPCID.Spazmatism && npc.type != ModContent.NPCType<Yharon>() && npc.type != NPCID.MoonLordCore && npc.type != ModContent.NPCType<PrimordialWyrmHead>())
                        {
                            orig(self, i);
                        }
                    }
                }
                if (EntropyMode && (self.ModNPC != null && (self.ModNPC is GiantClam || (self.ModNPC is PerforatorHive))))
                {
                    orig(self, i);
                }
                if (self.ModNPC is TheProphet && self.Calamity().CurrentlyEnraged)
                {
                    orig(self, i);
                }
                if (EntropyMode && self.ModNPC != null && (self.ModNPC is BrimstoneElemental || self.ModNPC is AquaticScourgeHead || self.ModNPC is AquaticScourgeTail || self.ModNPC is AquaticScourgeBodyAlt || self.ModNPC is AquaticScourgeBody || self.ModNPC is Cataclysm || self.ModNPC is Catastrophe || (self.ModNPC is CalamitasClone && !self.dontTakeDamage) || self.ModNPC is Leviathan || self.ModNPC is Anahita || self.ModNPC is AstrumAureus || self.ModNPC is PlaguebringerGoliath || self.ModNPC is RavagerBody || self.ModNPC is RavagerHead || self.ModNPC is RavagerHead2 || self.ModNPC is Bumblefuck || self.ModNPC is Polterghast))
                {
                    orig(self, i);
                    self.position -= self.velocity * 0.85f;
                }
                orig(self, i);
            }
        }

        private Rectangle modifyRect(On_Player.orig_getRect orig, Player self)
        {
            /*if (self.Entropy().MariviniumSet)
            {
                return orig(self).Center.ToVector2().getRectCentered(10, 10);
            }*/
            return orig(self);
        }

        private int StrikeNpc(On_NPC.orig_StrikeNPC_HitInfo_bool_bool orig, NPC self, NPC.HitInfo hit, bool fromNet, bool noPlayerInteraction)
        {
            if (!hit.InstantKill)
            {
                if (self.ModNPC != null && self.ModNPC is AbyssalWraith aw)
                {
                    if (aw.getMaxDamageCanTake() > 0)
                    {
                        if (hit.Damage > aw.getMaxDamageCanTake())
                        {
                            hit.Damage = aw.getMaxDamageCanTake();
                        }
                    }
                    hit.Damage = (int)(hit.Damage * aw.getDR());
                }
                if (self.boss && EntropyMode)
                {
                    if (hit.Damage > self.lifeMax * 0.1f)
                    {
                        hit.Damage = (int)(self.lifeMax * 0.1f);
                    }
                    hit.Damage = (int)(hit.Damage * (self.life < (self.Entropy().TDRCounter / (3f * 60 * 60) * self.lifeMax) ? (1 / (1 + ((self.Entropy().TDRCounter / (3f * 60 * 60) * self.lifeMax) - self.life) * (14f / self.lifeMax))) : 1));
                }
            }
            return orig(self, hit, fromNet, noPlayerInteraction);
        }

        private void vAi(On_NPC.orig_VanillaAI orig, NPC self)
        {
            orig(self);
        }

        private void findFrame(On_NPC.orig_FindFrame orig, NPC self)
        {
            if (self.Entropy().ToFriendly)
            {
                self.target = 0;
                NPC npc = self;
                npc.boss = false;

                npc.friendly = true;

                NPC t = null;
                float dist = 4600;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && !n.dontTakeDamage)
                    {
                        if (CEUtils.getDistance(n.Center, npc.Center) < dist)
                        {
                            t = n;
                            dist = CEUtils.getDistance(n.Center, npc.Center);
                        }
                    }
                }
                if (t == null)
                {
                    npc.Entropy().plrOldPos3 = Main.player[0].position;
                    npc.Entropy().plrOldVel3 = Main.player[0].velocity;
                    Main.player[0].Center = npc.Entropy().f_owner.ToPlayer().Center;
                    Main.player[0].velocity = npc.Entropy().f_owner.ToPlayer().velocity;
                }
                else
                {
                    npc.Entropy().plrOldPos3 = Main.player[0].position;
                    npc.Entropy().plrOldVel3 = Main.player[0].velocity;
                    Main.player[0].Center = t.Center;
                    Main.player[0].velocity = t.velocity;
                }
            }
            orig(self);
            if (self.Entropy().plrOldPos3.HasValue)
            {
                Main.player[0].position = self.Entropy().plrOldPos3.Value;
                self.Entropy().plrOldPos3 = null;
            }
            if (self.Entropy().plrOldVel3.HasValue)
            {
                Main.player[0].velocity = self.Entropy().plrOldVel3.Value;
                self.Entropy().plrOldVel3 = null;
            }
        }

        private void targetClostUpgraded(On_NPC.orig_TargetClosestUpgraded orig, NPC self, bool faceTarget, Vector2? checkPosition)
        {
            orig(self, faceTarget, checkPosition);
            /*if (self.Entropy().ToFriendly)
            {
                self.target = 0;
                NPC npc = self;
                npc.boss = false;

                npc.friendly = true;

                SetTargetTrackingValues(self, faceTarget, CEUtils.getDistance(self.Center, Main.player[0].Center), -1);
            }*/
        }

        public static void SetTargetTrackingValues(NPC npc, bool faceTarget, float realDist, int tankTarget)
        {
            if (tankTarget >= 0)
            {
                npc.targetRect = new Rectangle((int)Main.projectile[tankTarget].position.X, (int)Main.projectile[tankTarget].position.Y, Main.projectile[tankTarget].width, Main.projectile[tankTarget].height);
                npc.direction = 1;
                if (npc.targetRect.X + npc.targetRect.Width / 2 < npc.position.X + npc.width / 2)
                    npc.direction = -1;

                npc.directionY = 1;
                if (npc.targetRect.Y + npc.targetRect.Height / 2 < npc.position.Y + npc.height / 2)
                    npc.directionY = -1;
            }
            else
            {
                if (npc.target < 0 || npc.target >= 255)
                    npc.target = 0;

                npc.targetRect = new Rectangle((int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, Main.player[npc.target].width, Main.player[npc.target].height);
                if (Main.player[npc.target].dead)
                    faceTarget = false;

                if (Main.player[npc.target].npcTypeNoAggro[npc.type] && npc.direction != 0)
                    faceTarget = false;

                if (faceTarget)
                {
                    _ = Main.player[npc.target].aggro;
                    _ = (Main.player[npc.target].height + Main.player[npc.target].width + npc.height + npc.width) / 4;
                    bool flag = npc.oldTarget >= 0 && npc.oldTarget <= 254;
                    bool num = Main.player[npc.target].itemAnimation == 0 && Main.player[npc.target].aggro < 0;
                    bool flag2 = !npc.boss;
                    if (!(num && flag && flag2))
                    {
                        npc.direction = 1;
                        if (npc.targetRect.X + npc.targetRect.Width / 2 < npc.position.X + npc.width / 2)
                            npc.direction = -1;

                        npc.directionY = 1;
                        if (npc.targetRect.Y + npc.targetRect.Height / 2 < npc.position.Y + npc.height / 2)
                            npc.directionY = -1;
                    }
                }
            }

            if (npc.confused)
                npc.direction *= -1;

            if ((npc.direction != npc.oldDirection || npc.directionY != npc.oldDirectionY || npc.target != npc.oldTarget) && !npc.collideX && !npc.collideY)
                npc.netUpdate = true;
        }
        private void targetClost(On_NPC.orig_TargetClosest orig, NPC self, bool faceTarget)
        {
            orig(self, faceTarget);
            if (self.Entropy().ToFriendly)
            {
                self.target = 0;
                NPC npc = self;
                npc.boss = false;

                npc.friendly = true;
                SetTargetTrackingValues(self, faceTarget, CEUtils.getDistance(self.Center, Main.player[0].Center), -1);
            }
        }

        private void add_buff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (type != ModContent.BuffType<AdrenalineMode>() && type != ModContent.BuffType<RageMode>())
            {
                if (Main.debuff[type])
                {
                    if (Main.rand.NextDouble() < self.Entropy().DebuffImmuneChance)
                    {
                        return;
                    }
                }
                if (cooldownBuffs.Contains(type))
                {
                    timeToAdd = (int)(timeToAdd * self.Entropy().CooldownTimeMult);
                }
                if (Main.debuff[type])
                {
                    timeToAdd = (int)(timeToAdd * self.Entropy().DebuffTime);
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
            {
                return !Main.gameMenu && Main.LocalPlayer.Entropy().brillianceCard > 0;
            }
            set
            {
                if (Main.gameMenu)
                {
                    return;
                }
                Main.LocalPlayer.Entropy().brillianceCard = value ? 3 : 0;
            }
        }

        public static float BrillianceCardValue = 1.5f;
        public static float OracleDeckBrilValue = 2f;
        public static float brillianceLightMulti { get { if (Main.gameMenu) { return 1; } if (Main.LocalPlayer.Entropy().oracleDeck) { return OracleDeckBrilValue; } else if (BrilEnable) { return BrillianceCardValue; } else { return 1; } } }

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
        private Action<T1> GetAction<T1>(Dictionary<string, object> objects, string key)
        {
            if (objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1>)
            {
                return (Action<T1>)actionObj;
            }
            return null;
        }

        private Action<T1, T2> GetAction<T1, T2>(Dictionary<string, object> objects, string key)
        {
            if (objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1, T2>)
            {
                return (Action<T1, T2>)actionObj;
            }
            return null;
        }

        private Action<T1, T2, T3> GetAction<T1, T2, T3>(Dictionary<string, object> objects, string key)
        {
            if (objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1, T2, T3>)
            {
                return (Action<T1, T2, T3>)actionObj;
            }
            return null;
        }
        public override object Call(params object[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] is string str)
                {
                    if (str.ToLower().Equals("RegisterBookMarkEffect".ToLower()))
                    {
                        if (!(args[1] is Dictionary<string, object>))
                        {
                            this.Logger.Warn("Args[1] Must be a Dictionary<string, object>");
                            return null;
                        }
                        Dictionary<string, object> objects = (Dictionary<string, object>)args[1];
                        if (!objects.TryGetValue("Name", out object nameObj) || !(nameObj is string))
                        {
                            this.Logger.Warn("Name is required and must be a string");
                            return null;
                        }
                        string name = (string)nameObj;

                        Action<ModProjectile> onShoot = GetAction<ModProjectile>(objects, "OnShoot");
                        Action<ModProjectile> onActive = GetAction<ModProjectile>(objects, "OnActive");
                        Action<Projectile, bool> onProjectileSpawn = GetAction<Projectile, bool>(objects, "OnProjectileSpawn");
                        Action<Projectile, bool> updateProjectile = GetAction<Projectile, bool>(objects, "UpdateProjectile");
                        Action<Projectile, NPC, int> onHitNPC = GetAction<Projectile, NPC, int>(objects, "OnHitNPC");
                        Action<Projectile, NPC, NPC.HitModifiers> modifyHitNPC = GetAction<Projectile, NPC, NPC.HitModifiers>(objects, "ModifyHitNPC");
                        Action<Projectile, bool> BookUpdate = GetAction<Projectile, bool>(objects, "BookUpdate");

                        BookMarkLoader.RegisterBookmarkEffect(
                            name,
                            onShoot,
                            onActive,
                            onProjectileSpawn,
                            updateProjectile,
                            onHitNPC,
                            modifyHitNPC,
                            BookUpdate
                        );
                    }
                    if (str.ToLower().Equals("RegisterBookMark".ToLower()))
                    {
                        Func<TInput, TOutput> GetModifierFunc<TInput, TOutput>(Dictionary<string, object> objects, string key)
                        {
                            if (objects.TryGetValue(key, out object funcObj) && funcObj is Func<TInput, TOutput>)
                            {
                                return (Func<TInput, TOutput>)funcObj;
                            }
                            return null;
                        }
                        if (!(args[1] is Dictionary<string, object>))
                        {
                            this.Logger.Warn("Args[1] Must be a Dictionary<string, object>");
                            return null;
                        }
                        Dictionary<string, object> objects = (Dictionary<string, object>)args[1];
                        if (!objects.TryGetValue("ItemType", out object itemTypeObj) || !(itemTypeObj is int))
                        {
                            this.Logger.Warn("ItemType is required and must be an integer");
                            return null;
                        }
                        int itemType = (int)itemTypeObj;

                        if (!objects.TryGetValue("Texture", out object textureObj) || !(textureObj is Asset<Texture2D>))
                        {
                            this.Logger.Warn("Texture is required and must be an Asset<Texture2D>");
                            return null;
                        }
                        Func<Item, Item, bool> func = null;
                        if (objects.TryGetValue("CanBeEquipWithFunc", out var cbew_func))
                        {
                            if(cbew_func is Func<Item, Item, bool> fc)
                            {
                                func = fc;
                            }
                        }
                        Asset<Texture2D> texture = (Asset<Texture2D>)textureObj;

                        string effectName = objects.TryGetValue("EffectName", out object effectNameObj) && effectNameObj is string
                            ? (string)effectNameObj : "";

                        Func<float, float> modifyStat_Damage = GetModifierFunc<float, float>(objects, "ModifyStat_Damage");
                        Func<float, float> modifyStat_Knockback = GetModifierFunc<float, float>(objects, "ModifyStat_Knockback");
                        Func<float, float> modifyStat_ShootSpeed = GetModifierFunc<float, float>(objects, "ModifyStat_ShootSpeed");
                        Func<float, float> modifyStat_Homing = GetModifierFunc<float, float>(objects, "ModifyStat_Homing");
                        Func<float, float> modifyStat_Size = GetModifierFunc<float, float>(objects, "ModifyStat_Size");
                        Func<float, float> modifyStat_Crit = GetModifierFunc<float, float>(objects, "ModifyStat_Crit");
                        Func<float, float> modifyStat_HomingRange = GetModifierFunc<float, float>(objects, "ModifyStat_HomingRange");
                        Func<int, int> modifyStat_PenetrateAddition = GetModifierFunc<int, int>(objects, "ModifyStat_PenetrateAddition");
                        Func<float, float> modifyStat_AttackSpeed = GetModifierFunc<float, float>(objects, "ModifyStat_AttackSpeed");
                        Func<int, int> modifyStat_ArmorPenetration = GetModifierFunc<int, int>(objects, "ModifyStat_ArmorPenetration");
                        Func<int, int> modifyStat_LifeSteal = GetModifierFunc<int, int>(objects, "ModifyStat_LifeSteal");
                        Func<int, int> modifyProjectileType = GetModifierFunc<int, int>(objects, "ModifyProjectileType");
                        Func<int> modifyBaseProjectileType = objects.TryGetValue("ModifyBaseProjectileType", out object mbptObj) && mbptObj is Func<int>
                            ? (Func<int>)mbptObj : null;
                        Func<int, int> modifyShootCooldown = GetModifierFunc<int, int>(objects, "ModifyShootCooldown");

                        BookMarkLoader.RegisterBookmark(
                            itemType,
                            texture,
                            effectName,
                            modifyStat_Damage,
                            modifyStat_Knockback,
                            modifyStat_ShootSpeed,
                            modifyStat_Homing,
                            modifyStat_Size,
                            modifyStat_Crit,
                            modifyStat_HomingRange,
                            modifyStat_PenetrateAddition,
                            modifyStat_AttackSpeed,
                            modifyStat_ArmorPenetration,
                            modifyStat_LifeSteal,
                            modifyProjectileType,
                            modifyBaseProjectileType,
                            modifyShootCooldown,
                            func
                        );

                    }
                    if (str.Equals("IsBookMark"))
                    {
                        Item item = (Item)args[1];
                        return BookMarkLoader.IsABookMark(item);
                    }
                    if (str.Equals("SetBarColor"))
                    {
                        int type = (int)args[1];
                        Color color = (Color)args[2];
                        EntropyBossbar.bossbarColor[type] = color;
                    }
                    if (str.Equals("SetTTHoldoutCheck"))
                    {
                        EGlobalProjectile.checkHoldOut = (bool)args[1];
                    }
                    if (str.Equals("GetTTHoldoutCheck"))
                    {
                        return EGlobalProjectile.checkHoldOut;
                    }
                    if (str.Equals("CopyProjForTTwin"))
                    {
                        Projectile projectile = ((int)args[1]).ToProj();
                        EGlobalProjectile.checkHoldOut = false;
                        foreach (Projectile p in Main.projectile)
                        {
                            if (p.active && p.type == ModContent.ProjectileType<TwistedTwinMinion>() && p.owner == Main.myPlayer)
                            {

                                int phd = Projectile.NewProjectile(Main.LocalPlayer.GetSource_ItemUse(Main.LocalPlayer.HeldItem), p.Center, Vector2.Zero, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                                Projectile ph = phd.ToProj();
                                ph.scale *= 0.8f;
                                ph.Entropy().IndexOfTwistedTwinShootedThisProj = p.identity;
                                ph.netUpdate = true;
                                Projectile projts = ph;
                                ph.damage = (int)(ph.damage * TwistedTwinMinion.damageMul);
                                if (!projts.usesLocalNPCImmunity)
                                {
                                    projts.usesLocalNPCImmunity = true;
                                    projts.localNPCHitCooldown = 12;
                                }
                            }
                        }
                        EGlobalProjectile.checkHoldOut = true;
                    }
                }
            }
            return null;
        }

        private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, Func<bool> downed, object npcTypes, Dictionary<string, object> extraInfo)
            => bossChecklist.Call("LogBoss", hostMod, name, difficulty, downed, npcTypes, extraInfo);
        public static List<MusicBox> mbRegs = null;
        public void RegistryMusicBoxes()
        {
            foreach (var mb in mbRegs)
            {
                MusicBox.AddMusicBox(mb.MusicFile, mb.Type, mb.MusicBoxTile);
            }
            mbRegs = null;
        }
        public static List<int> cooldownBuffs;
        public static void CalEnchantsRegistry()
        {
            EnchantmentManager.EnchantmentList.Add(new Enchantment(Instance.GetLocalization("BloodBoiling"), Instance.GetLocalization("BloodBoilingDescr"),
                    903,
                    "CalamityEntropy/Assets/UI/CalamitasEnchantments/CurseIcon_BloodBoiling",
                    null,
                    player => player.Entropy().bloodBoiling = 3,
                    item => item.IsEnchantable() && item.damage > 0 && item.DamageType != DamageClass.MeleeNoSpeed));
        }
        public void initializeIntro(int type, Color c1, Color c2, string Name, int colorMode = 0)
        {
            Mod infernum;
            if (ModLoader.TryGetMod("InfernumMode", out infernum))
            {
                try
                {
                    Color fc1(float f1, float f2)
                    {
                        return Color.Lerp(c1, c2, f1) * f2;
                    }
                    Color fc2(float f1, float f2)
                    {
                        return Color.Lerp(c1, c2, ((float)Math.Cos((f1 * MathHelper.TwoPi) - MathHelper.Pi) + 1) * 0.5f) * f2;
                    }
                    Func<float, float, Color> func;
                    func = colorMode == 0 ? fc1 : fc2;
                    object obj = infernum.Call(new object[]
                    {
                        "InitializeIntroScreen",
                        Language.GetText("Mods.CalamityEntropy.InfernumIntros." + Name),
                        150,
                        true,
                        () => NPC.AnyNPCs(type) && (bool)infernum.Call(new object[]
                        {
                            "GetInfernumActive"
                        }),
                        func
                    });
                    Mod infernum6 = infernum;
                    object[] array = new object[3];
                    array[0] = "SetupCompletionEffects";
                    array[1] = obj;
                    array[2] = delegate ()
                    {
                    };
                    infernum6.Call(array);
                    Mod infernum2 = infernum;
                    object[] array2 = new object[3];
                    array2[0] = "SetupLetterAdditionSound";
                    array2[1] = obj;
                    array2[2] = (() => SoundID.Run);
                    infernum2.Call(array2);
                    Mod infernum3 = infernum;
                    object[] array3 = new object[3];
                    array3[0] = "SetupLetterDisplayCompletionRatio";
                    array3[1] = obj;
                    array3[2] = ((int at) => (float)at / 120f);
                    infernum3.Call(array3);
                    Mod infernum4 = infernum;
                    object[] array4 = new object[4];
                    array4[0] = "SetupMainSound";
                    array4[1] = obj;
                    array4[2] = ((int atr, int at, float tdi, float ldcr) => false);
                    array4[3] = (() => SoundID.NPCDeath56);
                    infernum4.Call(array4);
                    infernum.Call(new object[]
                    {
                        "SetupScreenCovering",
                        obj,
                        Color.Transparent
                    });
                    infernum.Call(new object[]
                    {
                        "SetupTextScale",
                        obj,
                        1.1f
                    });
                    infernum.Call(new object[]
                    {
                        "RegisterIntroScreen",
                        obj
                    });
                    Mod infernum5 = infernum;
                    object[] array5 = new object[4];
                    array5[0] = "RegisterBossBarPhaseInfo";
                    array5[1] = type;
                    int num = 2;
                    List<float> list = new List<float>();
                    list.Add(1f);
                    list.Add(0.25f);
                    array5[num] = list;
                    array5[3] = ModContent.Request<Texture2D>(ModContent.GetModNPC(type).BossHeadTexture, AssetRequestMode.AsyncLoad).Value;
                    infernum5.Call(array5);
                }
                catch
                {
                }
            }
        }
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("InfernumMode", out var _))
            {
                InfFont.InfernumFont.SetFont();
                initializeIntro(ModContent.NPCType<CruiserHead>(), Color.Purple, Color.LightBlue, "Cruiser");
                initializeIntro(ModContent.NPCType<NihilityActeriophage>(), Color.Blue, Color.LightBlue, "NihilityTwin");
                initializeIntro(ModContent.NPCType<TheProphet>(), Color.LightBlue, Color.SkyBlue, "Prophet", 1);
                initializeIntro(ModContent.NPCType<Luminaris>(), new Color(190, 180, 220), Color.Purple, "Luminaris", 1);
            }
            CalEnchantsRegistry();
            cooldownBuffs = new List<int>() { BuffID.PotionSickness, BuffID.ChaosState, ModContent.BuffType<DivingShieldCooldown>(), ModContent.BuffType<ShatteredOrb>() };
            RegistryDraedonDialogs();
            foreach (ICELoader setup in ILoaders)
            {
                setup.SetupData();
                if (!Main.dedServ)
                {
                    setup.LoadAsset();
                }
            }
            Type baseTypeLR = typeof(LoreEffect);
            Type[] lrTypes = AssemblyManager.GetLoadableTypes(this.Code);
            foreach (Type type in lrTypes)
            {
                if (!type.IsSubclassOf(baseTypeLR) || type.IsAbstract)
                    continue;
                var loreEffect = (LoreEffect)Activator.CreateInstance(type);
                LoreReworkSystem.loreEffects[loreEffect.ItemType] = loreEffect;
                var _ = loreEffect.Decription.Value;
            }
            RegistryMusicBoxes();
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<Content.Buffs.HeatDeath>()] = false;
            }
            if (ModLoader.TryGetMod("IsaacMod", out Mod isaac))
            {
                isaac.Call("HeldProj", ModContent.ProjectileType<RailPulseBowProjectile>());
                isaac.Call("HeldProj", ModContent.ProjectileType<GhostdomWhisperHoldout>());
                isaac.Call("HeldProj", ModContent.ProjectileType<SamsaraCasketProj>());
            }
            if (ModLoader.TryGetMod("CalamityOverhaul", out var co))
            {
                co.Call(0, new string[]
                {"0", "0", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "0", "0",
                    "CalamityEntropy/BookMarkNeutron"
                });
            }
            string MyGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
            string Isaac1 = Path.Combine(MyGameFolder, "Binding of Isaac Repentance").Replace("/", "\\");
            string Isaac2 = Path.Combine(MyGameFolder, "Binding of Isaac Repentance+").Replace("/", "\\");
            BrokenAnkh.isaac = Directory.Exists(Isaac1) || Directory.Exists(Isaac2);

            Mod bossChecklist;
            if (ModLoader.TryGetMod("BossChecklist", out bossChecklist))
            {
                if (bossChecklist != null)
                {
                    {
                        {
                            string entryName = "AcropolisMechine";
                            List<int> collection = new List<int>() { };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/AcropolisMachine").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 0.8f, SpriteEffects.None, 0);
                            };
                            Func<bool> AcropDowned = () => EDownedBosses.downedAcropolis;
                            AddBoss(bossChecklist, Instance, entryName, 0.9f, AcropDowned, ModContent.NPCType<AcropolisMachine>(), new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetText("Mods.CalamityEntropy.NPCs.AcropolisMachine.BossChecklistIntegration.EntryName"),
                                ["spawnInfo"] = Language.GetText("Mods.CalamityEntropy.NPCs.AcropolisMachine.BossChecklistIntegration.SpawnInfo"),
                                ["despawnMessage"] = Language.GetText("Mods.CalamityEntropy.NPCs.AcropolisMachine.BossChecklistIntegration.DespawnMessage"),
                                ["collectibles"] = collection,
                                ["customPortrait"] = portrait
                            });
                        }
                        {
                            string entryName = "Luminaris";
                            List<int> collection = new List<int>() { };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/LuminarisBossCheckList").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 1, SpriteEffects.None, 0);
                            };
                            Func<bool> Luminaris = () => EDownedBosses.downedLuminaris;
                            AddBoss(bossChecklist, Instance, entryName, 9.505f, Luminaris, ModContent.NPCType<Luminaris>(), new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetText("Mods.CalamityEntropy.NPCs.Luminaris.BossChecklistIntegration.EntryName"),
                                ["spawnInfo"] = Language.GetText("Mods.CalamityEntropy.NPCs.Luminaris.BossChecklistIntegration.SpawnInfo"),
                                ["despawnMessage"] = Language.GetText("Mods.CalamityEntropy.NPCs.Luminaris.BossChecklistIntegration.DespawnMessage"),
                                ["spawnItems"] = ModContent.ItemType<IllusionaryDew>(),
                                ["collectibles"] = collection,
                                ["customPortrait"] = portrait
                            });
                        }
                        {
                            string entryName = "TheProphet";
                            List<int> collection = new List<int>() { ModContent.ItemType<RuneSong>(), ModContent.ItemType<UrnOfSouls>(), ModContent.ItemType<SpiritBanner>(), ModContent.ItemType<ProphecyFlyingKnife>(), ModContent.ItemType<RuneMachineGun>(), ModContent.ItemType<ForeseeOrb>(), ModContent.ItemType<RuneWing>() };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/Prophet").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 1, SpriteEffects.None, 0);
                            };
                            Func<bool> prophet = () => EDownedBosses.downedProphet;
                            AddBoss(bossChecklist, Instance, entryName, 11.85f, prophet, ModContent.NPCType<TheProphet>(), new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetText("Mods.CalamityEntropy.NPCs.TheProphet.BossChecklistIntegration.EntryName"),
                                ["spawnInfo"] = Language.GetText("Mods.CalamityEntropy.NPCs.TheProphet.BossChecklistIntegration.SpawnInfo"),
                                ["despawnMessage"] = Language.GetText("Mods.CalamityEntropy.NPCs.TheProphet.BossChecklistIntegration.DespawnMessage"),
                                ["spawnItems"] = ModContent.ItemType<ProphecyToken>(),
                                ["collectibles"] = collection,
                                ["customPortrait"] = portrait
                            });
                        }
                        {
                            string entryName = "NihilityTwin";
                            List<int> segments = new List<int>() { ModContent.NPCType<NihilityActeriophage>(), ModContent.NPCType<ChaoticCell>() };
                            List<int> collection = new List<int>() { ModContent.ItemType<NihilityTwinBag>(), ModContent.ItemType<NihilityTwinTrophy>(), ModContent.ItemType<NihilityTwinRelic>(), ModContent.ItemType<NihilityShell>(), ModContent.ItemType<Voidseeker>(), ModContent.ItemType<EventideSniper>(), ModContent.ItemType<NihilityBacteriophageWand>(), ModContent.ItemType<StarlessNight>(), ModContent.ItemType<VoidPathology>() };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/NihilityTwin").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 0.7f, SpriteEffects.None, 0);
                            };
                            Func<bool> nihtwin = () => EDownedBosses.downedNihilityTwin;
                            AddBoss(bossChecklist, Instance, entryName, 19.3f, nihtwin, segments, new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetText("Mods.CalamityEntropy.NPCs.NihilityActeriophage.BossChecklistIntegration.EntryName"),
                                ["spawnInfo"] = Language.GetText("Mods.CalamityEntropy.NPCs.NihilityActeriophage.BossChecklistIntegration.SpawnInfo"),
                                ["despawnMessage"] = Language.GetText("Mods.CalamityEntropy.NPCs.NihilityActeriophage.BossChecklistIntegration.DespawnMessage"),
                                ["spawnItems"] = ModContent.ItemType<NihilityHorn>(),
                                ["collectibles"] = collection,
                                ["customPortrait"] = portrait
                            });
                        }
                        {
                            string entryName = "Cruiser";
                            List<int> segments = new List<int>() { ModContent.NPCType<CruiserHead>(), ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() };
                            List<int> collection = new List<int>() { ModContent.ItemType<CruiserBag>(), ModContent.ItemType<CruiserTrophy>(), ModContent.ItemType<VoidScales>(), ModContent.ItemType<VoidMonolith>(), ModContent.ItemType<CruiserRelic>(), ModContent.ItemType<VoidRelics>(), ModContent.ItemType<VoidAnnihilate>(), ModContent.ItemType<VoidElytra>(), ModContent.ItemType<VoidEcho>(), ModContent.ItemType<Content.Items.Weapons.Silence>(), ModContent.ItemType<WingsOfHush>(), ModContent.ItemType<WindOfUndertaker>(), ModContent.ItemType<VoidToy>(), ModContent.ItemType<TheocracyPearlToy>(), ModContent.ItemType<CruiserPlush>() };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/Cruiser").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 0.7f, SpriteEffects.None, 0);
                            };
                            Func<bool> cruiser = () => EDownedBosses.downedCruiser;
                            AddBoss(bossChecklist, Instance, entryName, 21.7f, cruiser, segments, new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.EntryName"),
                                ["spawnInfo"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.SpawnInfo"),
                                ["despawnMessage"] = Language.GetTextValue("Mods.CalamityEntropy.NPCs.Cruiser.BossChecklistIntegration.DespawnMessage"),
                                ["spawnItems"] = ModContent.ItemType<VoidBottle>(),
                                ["collectibles"] = collection,
                                ["customPortrait"] = portrait
                            });
                        }
                        {
                            List<int> segments2 = new List<int>() { ModContent.NPCType<PrimordialWyrmHead>(), ModContent.NPCType<PrimordialWyrmBody>(), ModContent.NPCType<PrimordialWyrmBodyAlt>(), ModContent.NPCType<PrimordialWyrmTail>() };

                            List<int> collection2 = new List<int>() { ModContent.ItemType<EidolicWail>(), ModContent.ItemType<VoidEdge>(), ModContent.ItemType<HalibutCannon>(), ModContent.ItemType<AbyssShellFossil>(), ModContent.ItemType<Voidstone>(), ModContent.ItemType<Lumenyl>(), ModContent.ItemType<EidolicWail>(), 1508 };
                            Func<bool> wyd = () => DownedBossSystem.downedPrimordialWyrm;
                            Action<SpriteBatch, Rectangle, Color> portrait2 = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PrimordialWyrm/PrimordialWyrm_BossChecklist").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 1.3f, SpriteEffects.None, 0);
                            };
                            string entryName = "PrimordialWyrm";
                            AddBoss(bossChecklist, ModContent.GetInstance<CalamityMod.CalamityMod>(), entryName, 23.5f, wyd, segments2, new Dictionary<string, object>()
                            {
                                ["displayName"] = Language.GetText("Mods.CalamityMod.NPCs.PrimordialWyrmHead.DisplayName"),
                                ["spawnInfo"] = this.GetLocalization("PWSpawnInfo"),
                                ["collectibles"] = collection2,
                                ["customPortrait"] = portrait2
                            });
                        }
                    }

                }
            }
            if (!Main.dedServ)
            {
                ealaserSound = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/corruptedBeaconLoop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ealaserSound2 = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/portal_loop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ofCharge = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/ElectricLoop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                FableEye.sound = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/prophetlaserloop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                UrnOfSoulsHoldout.loopSnd = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/flamethrower loop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            EntropyBossbar.bossbarColor[NPCID.KingSlime] = new Color(90, 160, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<DesertScourgeHead>()] = new Color(216, 210, 175);
            EntropyBossbar.bossbarColor[ModContent.NPCType<GiantClam>()] = new Color(128, 255, 255);
            EntropyBossbar.bossbarColor[NPCID.EyeofCthulhu] = new Color(255, 40, 40);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsBody] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsHead] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsTail] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.BrainofCthulhu] = new Color(255, 40, 40);
            EntropyBossbar.bossbarColor[NPCID.QueenBee] = new Color(242, 242, 145);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Crabulon>()] = new Color(133, 255, 237);
            EntropyBossbar.bossbarColor[NPCID.DD2DarkMageT1] = new Color(180, 230, 255);
            EntropyBossbar.bossbarColor[NPCID.DD2DarkMageT3] = new Color(180, 230, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<HiveMind>()] = new Color(140, 60, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<PerforatorHive>()] = new Color(155, 60, 60);
            EntropyBossbar.bossbarColor[NPCID.SkeletronHead] = new Color(221, 221, 188);
            EntropyBossbar.bossbarColor[NPCID.Deerclops] = new Color(220, 200, 200);
            EntropyBossbar.bossbarColor[ModContent.NPCType<CrimulanPaladin>()] = new Color(255, 60, 75);
            EntropyBossbar.bossbarColor[ModContent.NPCType<SplitCrimulanPaladin>()] = new Color(255, 60, 75);
            EntropyBossbar.bossbarColor[ModContent.NPCType<EbonianPaladin>()] = new Color(160, 170, 220);
            EntropyBossbar.bossbarColor[ModContent.NPCType<SplitEbonianPaladin>()] = new Color(160, 170, 220);
            EntropyBossbar.bossbarColor[NPCID.WallofFlesh] = new Color(255, 40, 40);
            EntropyBossbar.bossbarColor[NPCID.Retinazer] = new Color(190, 190, 190);
            EntropyBossbar.bossbarColor[NPCID.Spazmatism] = new Color(190, 190, 190);
            EntropyBossbar.bossbarColor[NPCID.TheDestroyer] = new Color(190, 190, 190);
            EntropyBossbar.bossbarColor[NPCID.SkeletronPrime] = new Color(190, 190, 190);
            EntropyBossbar.bossbarColor[491] = new Color(180, 120, 80);
            EntropyBossbar.bossbarColor[NPCID.QueenSlimeBoss] = new Color(200, 160, 240);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Cryogen>()] = new Color(140, 255, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AquaticScourgeHead>()] = new Color(215, 195, 155);
            EntropyBossbar.bossbarColor[ModContent.NPCType<BrimstoneElemental>()] = new Color(255, 145, 115);
            EntropyBossbar.bossbarColor[ModContent.NPCType<CalamitasClone>()] = new Color(255, 145, 115);
            EntropyBossbar.bossbarColor[NPCID.Plantera] = new Color(255, 170, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<GreatSandShark>()] = new Color(225, 190, 130);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Anahita>()] = new Color(180, 180, 230);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Leviathan>()] = new Color(80, 235, 140);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AstrumAureus>()] = new Color(130, 130, 160);
            EntropyBossbar.bossbarColor[NPCID.Golem] = new Color(225, 106, 9);
            EntropyBossbar.bossbarColor[NPCID.GolemHead] = new Color(225, 106, 9);
            EntropyBossbar.bossbarColor[325] = new Color(255, 206, 106);
            EntropyBossbar.bossbarColor[327] = new Color(244, 184, 106);
            EntropyBossbar.bossbarColor[344] = new Color(0, 255, 172);
            EntropyBossbar.bossbarColor[344] = new Color(240, 28, 28);
            EntropyBossbar.bossbarColor[345] = new Color(200, 244, 246);
            EntropyBossbar.bossbarColor[392] = new Color(150, 250, 255);
            EntropyBossbar.bossbarColor[NPCID.DukeFishron] = new Color(80, 146, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<PlaguebringerGoliath>()] = new Color(60, 160, 30);
            EntropyBossbar.bossbarColor[636] = Color.White;
            EntropyBossbar.bossbarColor[551] = new Color(180, 75, 80);
            EntropyBossbar.bossbarColor[ModContent.NPCType<RavagerBody>()] = new Color(190, 180, 155);
            EntropyBossbar.bossbarColor[NPCID.CultistBoss] = new Color(0, 60, 255);
            EntropyBossbar.bossbarColor[422] = new Color(208, 255, 235);
            EntropyBossbar.bossbarColor[493] = new Color(14, 155, 230);
            EntropyBossbar.bossbarColor[507] = new Color(255, 30, 170);
            EntropyBossbar.bossbarColor[517] = new Color(255, 100, 46);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AstrumDeusHead>()] = new Color(96, 230, 190);
            EntropyBossbar.bossbarColor[NPCID.MoonLordCore] = new Color(213, 194, 156);
            EntropyBossbar.bossbarColor[NPCID.MoonLordLeechBlob] = new Color(213, 194, 156);
            EntropyBossbar.bossbarColor[NPCID.MoonLordHead] = new Color(213, 194, 156);
            EntropyBossbar.bossbarColor[NPCID.MoonLordHand] = new Color(213, 194, 156);
            EntropyBossbar.bossbarColor[ModContent.NPCType<ProfanedGuardianCommander>()] = new Color(255, 255, 120);
            EntropyBossbar.bossbarColor[ModContent.NPCType<ProfanedGuardianDefender>()] = new Color(255, 255, 120);
            EntropyBossbar.bossbarColor[ModContent.NPCType<ProfanedGuardianHealer>()] = new Color(255, 255, 120);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Providence>()] = new Color(255, 255, 120);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Bumblefuck>()] = new Color(200, 180, 100);
            EntropyBossbar.bossbarColor[ModContent.NPCType<CeaselessVoid>()] = new Color(180, 210, 220);
            EntropyBossbar.bossbarColor[ModContent.NPCType<StormWeaverHead>()] = new Color(120, 145, 180);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Signus>()] = new Color(223, 75, 170);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Polterghast>()] = new Color(100, 255, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<OldDuke>()] = new Color(190, 170, 130);
            EntropyBossbar.bossbarColor[ModContent.NPCType<DevourerofGodsHead>()] = new Color(121, 230, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<CruiserHead>()] = new Color(150, 60, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Yharon>()] = new Color(255, 220, 100);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AresBody>()] = new Color(242, 112, 73);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Apollo>()] = new Color(146, 200, 130);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Artemis>()] = new Color(146, 200, 130);
            EntropyBossbar.bossbarColor[ModContent.NPCType<ThanatosHead>()] = new Color(135, 220, 240);
            EntropyBossbar.bossbarColor[ModContent.NPCType<SupremeCalamitas>()] = new Color(255, 145, 115);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AbyssalWraith>()] = new Color(200, 40, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<VoidPope>()] = new Color(200, 40, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<PrimordialWyrmHead>()] = new Color(255, 255, 80);
            EntropyBossbar.bossbarColor[ModContent.NPCType<NihilityActeriophage>()] = new Color(255, 155, 248);
            EntropyBossbar.bossbarColor[ModContent.NPCType<ChaoticCell>()] = new Color(255, 155, 248);
            EntropyBossbar.bossbarColor[ModContent.NPCType<TheProphet>()] = new Color(180, 233, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Luminaris>()] = new Color(150, 100, 215);
            EntropyBossbar.bossbarColor[ModContent.NPCType<AcropolisMachine>()] = new Color(255, 93, 13);
            try
            {
                if (!Main.dedServ)
                {
                    if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
                    {
                        AddBossbarColor(catalyst, "Astrageldon", new Color(220, 94, 210));
                    }
                    if (ModLoader.TryGetMod("NoxusBoss", out Mod nxb))
                    {
                        AddBossbarColor(nxb, "AvatarRift", new Color(194, 60, 50));
                        AddBossbarColor(nxb, "AvatarOfEmptiness", new Color(194, 60, 50));
                        AddBossbarColor(nxb, "NamelessDeityBoss", new Color(255, 255, 255));
                    }
                    if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
                    {
                        AddBossbarColor(calHunt, "Goozma", new Color(94, 76, 99));
                    }
                    if (ModLoader.TryGetMod("CalamityFables", out Mod cf))
                    {
                        AddBossbarColor(cf, "Crabulon", new Color(86, 191, 255));
                        AddBossbarColor(cf, "DesertScourge", new Color(172, 154, 146));
                        AddBossbarColor(cf, "SirNautilus", new Color(155, 133, 99));
                    }
                    if (ModLoader.TryGetMod("InfernumMode", out Mod infernum))
                    {
                        EntropyBossbar.bossbarColor[infernum.Find<ModNPC>("BereftVassal").Type] = new Color(225, 190, 130);
                    }
                    if (ModLoader.TryGetMod("SOTS", out Mod sots))
                    {
                        AddBossbarColor(sots, "SubspaceSerpentHead", new Color(115, 114, 160));
                        AddBossbarColor(sots, "PutridPinky1", Color.Pink);
                        AddBossbarColor(sots, "PutridPinkyPhase2", Color.Pink);
                        AddBossbarColor(sots, "Polaris", new Color(200, 250, 250));
                        AddBossbarColor(sots, "NewPolaris", new Color(200, 250, 250));
                        AddBossbarColor(sots, "Lux", new Color(255, 200, 230));
                        AddBossbarColor(sots, "Glowmoth", new Color(255, 240, 200));
                        AddBossbarColor(sots, "PharaohsCurse", Color.Gold);
                        AddBossbarColor(sots, "UnusedAdvisorHead", new Color(238, 208, 255));
                    }
                    if (ModLoader.TryGetMod("FargowiltasSouls", out Mod fs))
                    {
                        AddBossbarColor(fs, "AbomBoss", new Color(249, 226, 77));
                        AddBossbarColor(fs, "BanishedBaron", new Color(230, 240, 242));
                        AddBossbarColor(fs, "CosmosChampion", Color.DarkOrange);
                        AddBossbarColor(fs, "EarthChampion", Color.Orange);
                        AddBossbarColor(fs, "LifeChampion", Color.Gold);
                        AddBossbarColor(fs, "NatureChampion", Color.Green);
                        AddBossbarColor(fs, "ShadowChampion", new Color(143, 100, 234));
                        AddBossbarColor(fs, "SpiritChampion", Color.DarkGoldenrod);
                        AddBossbarColor(fs, "TerraChampion", Color.DarkGreen);
                        AddBossbarColor(fs, "TimberChampion", new Color(230, 240, 242));
                        AddBossbarColor(fs, "WillChampion", new Color(234, 213, 143));
                        AddBossbarColor(fs, "CursedCoffin", Color.Yellow);
                        AddBossbarColor(fs, "LifeChallenger", Color.Gold);
                        AddBossbarColor(fs, "Magmaw", Color.Gray);
                        AddBossbarColor(fs, "MutantBoss", AprilFool ? new Color(217, 142, 67) : new Color(100, 200, 255));
                        AddBossbarColor(fs, "TrojanSquirrel", new Color(147, 108, 85));

                    }
                    
                    Logger.Warn("CalamityEntropy: Bossbar Color setup succesfully");
                }
            }
            catch
            {
                Logger.Warn("CalamityEntropy: Other addons' bossbar color failed to setup");
            }
        }
        public static bool SetupBossbarClrAuto = true;
        
        public static void AddBossbarColor(Mod mod, string name, Color color)
        {
            if (mod == null)
                return;
            if (mod.TryFind<ModNPC>(name, out var mnpc))
            {
                EntropyBossbar.bossbarColor[mnpc.Type] = color;
            }
        }

        public static float blackMaskAlpha = 0;
        public static int blackMaskTime = 0;
        public static Vector2 vLToCenter(Vector2 v, float z)
        {
            return Main.ScreenSize.ToVector2() / 2 + (v - Main.ScreenSize.ToVector2() / 2) * z;
        }
        public bool beegameInited = false;
        private object func;

        public static void SpawnHeavenSpark(Vector2 pos, float rot, float length, float scale, Color color = default, int LifeTime = 24)
        {
            Vector2 norl = rot.ToRotationVector2();
            float sengs = length;
            if (color == default)
            {
                color = Color.BlueViolet;
            }
            for (int j = 0; j < 53; j++)
            {
                var spark = new HeavenfallStar();
                EParticle.NewParticle(spark, pos, norl * (0.1f + j * 0.34f) * sengs, color, Main.rand.NextFloat(0.6f, 1.3f) * scale, 1, true, BlendState.Additive, norl.ToRotation(), LifeTime);
            }
            for (int j = 0; j < 53; j++)
            {
                var spark = new HeavenfallStar();
                EParticle.NewParticle(spark, pos, norl * -(0.1f + j * 0.34f) * sengs, color, Main.rand.NextFloat(0.6f, 1.3f) * scale, 1, true, BlendState.Additive, (-norl).ToRotation(), LifeTime);
            }
        }
    }
}
