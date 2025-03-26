using CalamityEntropy.Common;
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.BeesGame;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.ILEditing;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityEntropy.Content.NPCs.VoidInvasion;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.Content.Projectiles.Chainsaw;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Projectiles.Pets.Abyss;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Skies;
using CalamityEntropy.Content.UI;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Util;
using CalamityMod;
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
namespace CalamityEntropy
{
    public class CalamityEntropy : Mod
    {
        public enum NetPackages : byte
        {
            LotteryMachineRightClicked,
            TurnFriendly,
            Text,
            BossKilled,
            PlayerSetRB,
            PlayerSetPos,
            VoidTouchDamageShow,
            PoopSync,
            SpawnItem,
            PickUpPoop,
            SyncEntropyMode
        }
        public static bool EntropyMode { get { return EDownedBosses.EntropyMode; } set { EDownedBosses.EntropyMode = value; } }
        public static bool AprilFool = false;
        public static List<int> calDebuffIconDisplayList = new List<int>();
        public static CalamityEntropy Instance;
        public static int noMusTime = 0;
        public static Effect kscreen;
        public static Effect kscreen2;
        public static Effect cve;
        public static Effect cve2;
        public static Effect cab;
        public RenderTarget2D screen = null;
        public RenderTarget2D screen2 = null;
        public RenderTarget2D screen3 = null;
        public float screenShakeAmp = 0;
        public float cvcount = 0;
        public Vector2 screensz = Vector2.Zero;
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
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte type = reader.ReadByte();
            if (type == (byte)NetPackages.LotteryMachineRightClicked)
            {
                int plr = reader.ReadInt32();
                int npc = reader.ReadInt32();
                int wai = reader.ReadInt32();
                if (npc.ToNPC().ModNPC is LotteryMachine lm)
                {
                    lm.RightClicked(Main.player[plr]);
                    if (Main.dedServ)
                    {
                        ModPacket packet = CalamityEntropy.Instance.GetPacket();
                        packet.Write((byte)CalamityEntropy.NetPackages.LotteryMachineRightClicked);
                        packet.Write(plr);
                        packet.Write(npc);
                        packet.Write(wai);
                        packet.Send();
                    }
                }
            }
            if (type == (byte)NetPackages.TurnFriendly)
            {
                int id = reader.ReadInt32();
                int owner = reader.ReadInt32();
                id.ToNPC().Entropy().ToFriendly = true;
                id.ToNPC().Entropy().f_owner = owner;
                if (Main.dedServ)
                {
                    ModPacket p = this.GetPacket();
                    p.Write((byte)CalamityEntropy.NetPackages.TurnFriendly);
                    p.Write(id);
                    p.Write(owner);
                    p.Send();
                }
            }
            if (type == (byte)NetPackages.Text)
            {
                Main.NewText(reader.ReadString());
            }
            if (type == (byte)NetPackages.BossKilled)
            {
                bool flag = reader.ReadBoolean();
                if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic && !Main.dedServ && CalamityEntropy.noMusTime <= 0 && !BossRushEvent.BossRushActive && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || flag))
                {
                    CalamityEntropy.noMusTime = 300;
                    SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/Music/RepTrackJingle"));
                }
            }
            if (type == (byte)NetPackages.PlayerSetRB)
            {
                int playerIndex = reader.ReadInt32();
                bool active = reader.ReadBoolean();
                playerIndex.ToPlayer().Entropy().rBadgeActive = active;

                if (Main.dedServ)
                {
                    if (!active)
                    {
                        playerIndex.ToPlayer().velocity *= 0.2f;
                    }
                    ModPacket pack = Instance.GetPacket();
                    pack.Write((byte)CalamityEntropy.NetPackages.PlayerSetRB);
                    pack.Write(playerIndex);
                    pack.Write(active);
                    pack.Send();
                }
                else
                {
                    if (playerIndex != Main.myPlayer)
                    {
                        if (!active)
                        {
                            playerIndex.ToPlayer().velocity *= 0.2f;
                        }
                        if (active)
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantActivate"), playerIndex.ToPlayer().Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantOff"), playerIndex.ToPlayer().Center);
                        }
                    }
                }
            }
            if (type == (byte)NetPackages.PlayerSetPos)
            {
                int id = reader.ReadInt32();
                Vector2 pos = reader.ReadVector2();
                if (id != Main.myPlayer)
                {
                    id.ToPlayer().Center = pos;
                }
                if (Main.dedServ)
                {
                    ModPacket p = Instance.GetPacket();
                    p.Write((byte)NetPackages.PlayerSetPos);
                    p.Write(id);
                    p.WriteVector2(pos);
                    p.Send();
                }
            }
            if (type == (byte)NetPackages.VoidTouchDamageShow)
            {
                if (!Main.dedServ)
                {
                    NPC npc = reader.ReadInt32().ToNPC();
                    int damageDone = reader.ReadInt32();
                    CombatText.NewText(npc.getRect(), new Color(148, 148, 255), damageDone);
                }
            }
            if (type == (byte)NetPackages.PoopSync)
            {
                Player player = reader.ReadInt32().ToPlayer();
                bool holding = reader.ReadBoolean();
                player.Entropy().holdingPoop = holding;
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)NetPackages.PoopSync);
                    packet.Write(player.whoAmI);
                    packet.Write(holding);
                    packet.Send();
                }
            }
            if (type == (byte)NetPackages.SpawnItem)
            {
                int plr = reader.ReadInt32();
                int itemtype = reader.ReadInt32();
                int stack = reader.ReadInt32();
                Player player = plr.ToPlayer();

                if (!Main.dedServ && itemtype == ModContent.ItemType<PoopPickup>())
                {
                    Util.Util.PlaySound("fart", 1, player.Center);
                }
                if (Main.dedServ)
                {
                    int i = Item.NewItem(player.GetSource_FromThis(), player.getRect(), new Item(itemtype, stack), false, true);
                    if (i < Main.item.Length)
                    {
                        Main.item[i].noGrabDelay = 100;
                    }
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)NetPackages.SpawnItem);
                    packet.Write(player.whoAmI);
                    packet.Write(itemtype);
                    packet.Write(stack);
                    packet.Send();
                }
            }
            if (type == (byte)NetPackages.PickUpPoop)
            {
                int plr = reader.ReadInt32();
                string name = reader.ReadString();
                Player player = plr.ToPlayer();
                Poop poop = new PoopNormal();
                foreach (Poop p in Poop.instances)
                {
                    if (p.FullName == name)
                    {
                        poop = p;
                    }
                }
                player.Entropy().poops.Add(poop);
            }
            if (type == (byte)NetPackages.SyncEntropyMode)
            {
                bool enabled = reader.ReadBoolean();
                EntropyMode = enabled;
                if (Main.dedServ)
                {
                    ModPacket packet = this.GetPacket();
                    packet.Write((byte)NetPackages.SyncEntropyMode);
                    packet.Write(enabled);
                    packet.Send();
                }
                else
                {
                    if (EntropyMode)
                    {
                        Main.NewText(this.GetLocalization("EntropyModeActive").Value, new Color(170, 18, 225));
                    }
                    else
                    {
                        Main.NewText(this.GetLocalization("EntropyModeDeactive").Value, new Color(170, 18, 225));
                    }
                }
            }
        }
        public static SoundEffect ealaserSound = null;
        public static SoundEffect ealaserSound2 = null;
        public override void Load()
        {
            Instance = this;

            DateTime today = DateTime.Now;
            AprilFool = today.Month == 4 && today.Day == 1;

            LoopSoundManager.init();

            efont1 = ModContent.Request<DynamicSpriteFont>("CalamityEntropy/Assets/Fonts/EFont", AssetRequestMode.ImmediateLoad).Value;
            efont2 = ModContent.Request<DynamicSpriteFont>("CalamityEntropy/Assets/Fonts/VCRFont", AssetRequestMode.ImmediateLoad).Value;



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
            pixel = Util.Util.getExtraTex("white");
            for (int i = 0; i < 10; i++)
            {
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidBlade/f" + i.ToString()).Value;
            }

            AbyssalWraith.loadHead();
            CruiserHead.loadHead();
            CUtil.load();

            foreach (int id in CalamityLists.needsDebuffIconDisplayList)
            {
                calDebuffIconDisplayList.Add(id);
            }
            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserBody>());
            BossHealthBarManager.BossExclusionList.Add(ModContent.NPCType<CruiserTail>());
            On_FilterManager.EndCapture += ec;
            EntropySkies.setUpSkies();
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
            EModSys.timer = 0;
            BossRushEvent.Bosses.Insert(35, new BossRushEvent.Boss(ModContent.NPCType<NihilityActeriophage>(), permittedNPCs: new int[] { ModContent.NPCType<ChaoticCell>() }));
            BossRushEvent.Bosses.Insert(42, new BossRushEvent.Boss(ModContent.NPCType<CruiserHead>(), permittedNPCs: new int[] { ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() }));
            EModILEdit.load();
        }
        public static Effect whiteTrans => ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/WhiteTrans", AssetRequestMode.ImmediateLoad).Value;


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
            Texture2D shell = Util.Util.getExtraTex("shell");
            Texture2D crystalShield = Util.Util.getExtraTex("MariviniumShield");
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
                            Main.spriteBatch.Draw(shell, player.Center - Main.screenPosition + center, null, Color.White * 0.8f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / (float)shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / (float)count;
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
                            Main.spriteBatch.Draw(crystalShield, player.Center - Main.screenPosition + center, null, Color.White * 0.6f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / (float)shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / (float)count;
                    }
                }
                if (player.Entropy().mariviniumBody)
                {
                    Texture2D back = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Armor/Marivinium/Back").Value;
                    Main.EntitySpriteDraw(back, player.MountedCenter + player.gfxOffY * Vector2.UnitY - Main.screenPosition, null, Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16)), player.fullRotation, (new Vector2(player.direction > 0 ? 31 : 48 - 31, 20)), 1, (player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
                }
            }
            Main.spriteBatch.End();
        }

        private void drawIr(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            Texture2D shell = Util.Util.getExtraTex("shell");
            Texture2D crystalShield = Util.Util.getExtraTex("MariviniumShield");
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
                            Main.spriteBatch.Draw(shell, player.Center - Main.screenPosition + center, null, Color.White * 0.8f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / (float)shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / (float)count;
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
                            Main.spriteBatch.Draw(crystalShield, player.Center - Main.screenPosition + center, null, Color.White * 0.6f * ((((rot.ToRotationVector2().Y) + 1) * 0.5f) * 0.7f + 0.3f), 0, shell.Size() / 2, new Vector2(sizeX / (float)shell.Width, 1), SpriteEffects.None, 0);
                        }
                        rot += MathHelper.TwoPi / (float)count;
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
                                    float alpha = (float)poc.usingTime / 60f;
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
            EParticle.drawAll();
            orig(self);
        }
        public int pocType = -1;
        private void drawmenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (LoopSoundManager.sounds != null)
            {
                if (LoopSoundManager.sounds.Count > 0)
                {
                    foreach (var sound in LoopSoundManager.sounds)
                    {
                        sound.stop();
                    }
                    LoopSoundManager.sounds.Clear();
                }
            }
        }

        private void npcupdate(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<EGlobalNPC>(out var en))
            {
                if (self.active && self.Entropy().AnimaTrapped > 0)
                {
                    en.AnimaTrapped--;
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
                    if (self.active)
                    {
                        if (self.TryGetGlobalNPC<DeliriumGlobalNPC>(out var mn))
                        {
                            if (mn.delirium)
                            {
                                NPC npc = self;
                                npc.damage = mn.damage;
                                mn.counter--;
                                if (mn.counter <= 0)
                                {
                                    if (!Main.dedServ)
                                    {
                                        Util.Util.PlaySound("clicker_static", 1, npc.Center);
                                    }
                                    mn.counter = Main.rand.Next(60, 360);
                                    npc.netUpdate = true;
                                    npc.netSpam = 0;
                                    int npc_ = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, Delirium.npcTurns[Main.rand.Next(Delirium.npcTurns.Count)]);
                                    NPC spawn = npc_.ToNPC();
                                    spawn.Center = npc.Center;
                                    spawn.lifeMax = npc.lifeMax;
                                    spawn.life = npc.life;
                                    spawn.damage = npc.damage;
                                    spawn.GetGlobalNPC<DeliriumGlobalNPC>().delirium = true;
                                    spawn.GetGlobalNPC<DeliriumGlobalNPC>().damage = mn.damage;
                                    spawn.GetGlobalNPC<DeliriumGlobalNPC>().counter = mn.counter;
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
                        }
                    }
                    orig(self, i);
                }
            }
            else { orig(self, i); }
        }

        private Rectangle modifyRect(On_Player.orig_getRect orig, Player self)
        {
            return orig(self);
        }

        private int StrikeNpc(On_NPC.orig_StrikeNPC_HitInfo_bool_bool orig, NPC self, NPC.HitInfo hit, bool fromNet, bool noPlayerInteraction)
        {
            if (!hit.InstantKill)
            {
                if (self.ModNPC is AbyssalWraith aw)
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
                    hit.Damage = (int)(hit.Damage * (self.life < ((float)self.Entropy().TDRCounter / (3f * 60 * 60) * self.lifeMax) ? (1 / (1 + (((float)self.Entropy().TDRCounter / (5f * 60 * 60) * self.lifeMax) - self.life) * (8f / self.lifeMax))) : 1));
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
                        if (Util.Util.getDistance(n.Center, npc.Center) < dist)
                        {
                            t = n;
                            dist = Util.Util.getDistance(n.Center, npc.Center);
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
            if (self.Entropy().ToFriendly)
            {
                self.target = 0;
                NPC npc = self;
                npc.boss = false;

                npc.friendly = true;

                SetTargetTrackingValues(self, faceTarget, Util.Util.getDistance(self.Center, Main.player[0].Center), -1);
            }
        }

        public static void SetTargetTrackingValues(NPC npc, bool faceTarget, float realDist, int tankTarget)
        {
            if (tankTarget >= 0)
            {
                npc.targetRect = new Rectangle((int)Main.projectile[tankTarget].position.X, (int)Main.projectile[tankTarget].position.Y, Main.projectile[tankTarget].width, Main.projectile[tankTarget].height);
                npc.direction = 1;
                if ((float)(npc.targetRect.X + npc.targetRect.Width / 2) < npc.position.X + (float)(npc.width / 2))
                    npc.direction = -1;

                npc.directionY = 1;
                if ((float)(npc.targetRect.Y + npc.targetRect.Height / 2) < npc.position.Y + (float)(npc.height / 2))
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
                        if ((float)(npc.targetRect.X + npc.targetRect.Width / 2) < npc.position.X + (float)(npc.width / 2))
                            npc.direction = -1;

                        npc.directionY = 1;
                        if ((float)(npc.targetRect.Y + npc.targetRect.Height / 2) < npc.position.Y + (float)(npc.height / 2))
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
                SetTargetTrackingValues(self, faceTarget, Util.Util.getDistance(self.Center, Main.player[0].Center), -1);
            }
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
            { if (Main.gameMenu) { return false; } return Main.LocalPlayer.Entropy().brillianceCard > 0; }
            set
            { if (Main.gameMenu) { return; } if (value) { Main.LocalPlayer.Entropy().brillianceCard = 3; } else { Main.LocalPlayer.Entropy().brillianceCard = 0; } }
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

        public override object Call(params object[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] is string str)
                {
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
                                ph.Entropy().ttindex = p.identity;
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
        public override void PostSetupContent()
        {
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
                            string entryName = "NihilityTwin";
                            List<int> segments = new List<int>() { ModContent.NPCType<NihilityActeriophage>(), ModContent.NPCType<ChaoticCell>() };
                            List<int> collection = new List<int>() { ModContent.ItemType<NihilityTwinBag>(), ModContent.ItemType<NihilityTwinTrophy>(), ModContent.ItemType<NihilityTwinRelic>(), ModContent.ItemType<NihilityShell>(), ModContent.ItemType<Voidseeker>(), ModContent.ItemType<EventideSniper>(), ModContent.ItemType<NihilityBacteriophageWand>(), ModContent.ItemType<StarlessNight>(), ModContent.ItemType<VoidPathology>() };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/NihilityTwin").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 0.7f, SpriteEffects.None, 0);
                            };
                            Func<bool> nihtwin = () => EDownedBosses.downedNihilityTwin;
                            AddBoss(bossChecklist, CalamityEntropy.Instance, entryName, 19.3f, nihtwin, segments, new Dictionary<string, object>()
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
                            List<int> collection = new List<int>() { ModContent.ItemType<CruiserBag>(), ModContent.ItemType<CruiserTrophy>(), ModContent.ItemType<VoidScales>(), ModContent.ItemType<VoidMonolith>(), ModContent.ItemType<CruiserRelic>(), ModContent.ItemType<VoidRelics>(), ModContent.ItemType<VoidAnnihilate>(), ModContent.ItemType<VoidElytra>(), ModContent.ItemType<VoidEcho>(), ModContent.ItemType<Content.Items.Weapons.Silence>(), ModContent.ItemType<RuneSong>(), ModContent.ItemType<WingsOfHush>(), ModContent.ItemType<WindOfUndertaker>(), ModContent.ItemType<VoidToy>(), ModContent.ItemType<TheocracyPearlToy>(), ModContent.ItemType<CruiserPlush>() };
                            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                            {
                                Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/BCL/Cruiser").Value;
                                sb.Draw(texture, rect.Center.ToVector2(), null, color, 0, texture.Size() / 2, 0.7f, SpriteEffects.None, 0);
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
                                ["spawnInfo"] = Language.GetText("Mods.CalamityEntropy.PWSpawnInfo"),
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
            }
            EntropyBossbar.bossbarColor[NPCID.KingSlime] = new Color(90, 160, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<DesertScourgeHead>()] = new Color(216, 210, 175);
            EntropyBossbar.bossbarColor[ModContent.NPCType<GiantClam>()] = new Color(128, 255, 255);
            EntropyBossbar.bossbarColor[NPCID.EyeofCthulhu] = new Color(255, 40, 40);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsBody] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsHead] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.EaterofWorldsHead] = new Color(80, 40, 255);
            EntropyBossbar.bossbarColor[NPCID.BrainofCthulhu] = new Color(255, 40, 40);
            EntropyBossbar.bossbarColor[NPCID.QueenBee] = new Color(242, 242, 145);
            EntropyBossbar.bossbarColor[ModContent.NPCType<Crabulon>()] = new Color(133, 255, 237);
            EntropyBossbar.bossbarColor[NPCID.DD2DarkMageT1] = new Color(180, 230, 255);
            EntropyBossbar.bossbarColor[NPCID.DD2DarkMageT3] = new Color(180, 230, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<HiveMind>()] = new Color(140, 60, 255);
            EntropyBossbar.bossbarColor[ModContent.NPCType<PerforatorHive>()] = new Color(155, 60, 60);
            EntropyBossbar.bossbarColor[NPCID.Skeleton] = new Color(221, 221, 188);
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
            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                EntropyBossbar.bossbarColor[catalyst.Find<ModNPC>("Astrageldon").Type] = new Color(220, 94, 210);
            }
            if (ModLoader.TryGetMod("NoxusBoss", out Mod nxb))
            {
                EntropyBossbar.bossbarColor[nxb.Find<ModNPC>("AvatarRift").Type] = new Color(194, 60, 50);
                EntropyBossbar.bossbarColor[nxb.Find<ModNPC>("AvatarOfEmptiness").Type] = new Color(194, 60, 50);
                EntropyBossbar.bossbarColor[nxb.Find<ModNPC>("NamelessDeityBoss").Type] = new Color(255, 255, 255);
            }
            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                EntropyBossbar.bossbarColor[calHunt.Find<ModNPC>("Goozma").Type] = new Color(94, 76, 99);
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
            rope.Start = player.Center;
            rope.End = Main.MouseWorld;
            rope.Update();
            List<Vector2> points = rope.GetPoints();
            points.Add(Main.MouseWorld);
            for (int i = 1; i < points.Count; i++)
            {
                Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
                Util.Util.drawLine(Main.spriteBatch, t, points[i - 1], points[i], Color.White, 8);
            }
        }
        private void ec(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            screenShakeAmp *= 0.9f;
            Texture2D dt;
            Texture2D dt2;
            Texture2D lb;
            dt = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cvmask").Value;
            dt2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cvmask2").Value;
            lb = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;
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

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            if (true)
            {

                Vector2 sz = screensz;
                if (screen == null || sz != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    {
                        screen?.Dispose();
                        screen = null;
                        screen3?.Dispose();
                        screen3 = null;
                        screen = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screen3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screensz = new Vector2(Main.screenWidth, Main.screenHeight);
                    }
                    {
                    }
                }
                if (screen2 == null || sz != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    {
                        screen2?.Dispose();
                        screen2 = null;
                        screen2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        screensz = new Vector2(Main.screenWidth, Main.screenHeight);
                    }
                    {
                    }
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
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);


                foreach (Projectile p in checkProj)
                {

                    if (!p.active)
                    {
                        continue;
                    }
                    if (p.ModProjectile is CruiserSlash)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/CruiserSlash").Value;

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
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Cruiser/CruiserBlackholeBullet").Value;
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
                        Texture2D disTex = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cruiserSpace2").Value;
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
                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White * 0.06f, pt.rotation, dt.Size() / 2, (5.4f * pt.alpha * sc) * 0.05f, SpriteEffects.None, 0);

                }
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile is Pioneer1 p1)
                    {
                        p1.drawVoid();
                    }
                }
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);

                foreach (Particle pt in VoidParticles.particles)
                {
                    if (!(pt.shape == 4))
                    {
                        continue;
                    }
                    Texture2D draw = Util.Util.getExtraTex("cvdt");
                    float sc = 1;

                    Main.spriteBatch.Draw(draw, pt.position - Main.screenPosition, null, Color.White, pt.rotation, draw.Size() / 2, 2.2f * pt.alpha * sc, SpriteEffects.None, 0);

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
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                cve.CurrentTechnique = cve.Techniques["Technique1"];
                cve.CurrentTechnique.Passes[0].Apply();
                Texture2D backg = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_blue_base", AssetRequestMode.ImmediateLoad).Value;                 /*cve.Parameters["tex1"].SetValue(backg);
                cve.Parameters["tex2"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/Backg1").Value);
                cve.Parameters["tex3"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Extra/Backg2").Value);*/
                cve.Parameters["tex1"].SetValue(backg);
                cve.Parameters["tex2"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_starfield_1").Value);
                cve.Parameters["tex3"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_starfield_2").Value);
                cve.Parameters["tex4"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_starfield_3").Value);
                cve.Parameters["tex5"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_starfield_4").Value);
                cve.Parameters["tex6"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/planetarium_starfield_5").Value);
                cve.Parameters["time"].SetValue((float)cvcount / 50f);
                cve.Parameters["scsize"].SetValue(Main.ScreenSize.ToVector2());
                cve.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(-cvcount / 6f, cvcount / 6f)) / Main.ScreenSize.ToVector2());
                Main.spriteBatch.Draw(screen3, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

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
                            if (ve.Count >= 3)
                            {
                                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
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
                                Color cl = new Color(200, 235, 255);
                                for (int i = mp.odp.Count - 1; i >= 1; i--)
                                {
                                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * (((float)(255 - p.alpha)) / 255f), size * 0.7f);
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
                            Color cl = new Color(200, 235, 255);
                            if (p.ai[2] > 0)
                            {
                                cl = new Color(255, 160, 160);
                            }
                            for (int i = mp.odp.Count - 1; i >= 1; i--)
                            {
                                Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, mp.odp[i], mp.odp[i - 1], cl * (((float)(255 - p.alpha)) / 255f), size * 0.7f);
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
                            Texture2D shieldTexture = Util.Util.getExtraTex("shield");
                            Main.spriteBatch.Draw(shieldTexture, p.Center - Main.screenPosition, null, new Color(186, 120, 255), 0, shieldTexture.Size() / 2, 0.47f, SpriteEffects.None, 0);

                        }
                    }
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
                cve2.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/VoidBack").Value);
                cve2.Parameters["time"].SetValue((float)cvcount / 50f);
                cve2.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(cvcount * 1.4f, cvcount * 1.4f)) / new Vector2(1920, 1080));
                Main.spriteBatch.Draw(screen, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

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
                cab.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/AwSky1").Value);
                cab.Parameters["time"].SetValue((float)cvcount / 50f);
                cab.Parameters["scrsize"].SetValue(screen.Size());
                cab.Parameters["offset"].SetValue((Main.screenPosition + new Vector2(cvcount * 1.4f, cvcount * 1.4f)) / new Vector2(1920, 1080));
                Main.spriteBatch.Draw(Main.screenTargetSwap, Main.ScreenSize.ToVector2() / 2, null, Color.White, 0, Main.ScreenSize.ToVector2() / 2, 1, SpriteEffects.None, 0);

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

                                Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, player.Entropy().daPoints[i - 1], player.Entropy().daPoints[i], color * 0.6f, 12 * sc, 0);
                                sc += scj;
                            }
                        }
                    }


                }
                {

                }


                PixelParticle.drawAll();

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
                        Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Cruiser/VoidStar").Value;
                        Color c = Color.White;
                        if (p.ai[2] > 0 && p.ModProjectile is VoidStarF)
                        {
                            c = new Color(255, 100, 100);
                        }
                        Main.spriteBatch.Draw(t, p.Center - Main.screenPosition, null, c * (((float)(255 - p.alpha)) / 255f), p.rotation, t.Size() / 2, p.scale, SpriteEffects.None, 0);

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
                Texture2D kt = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/ksc1").Value;
                Texture2D kt2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/kscc").Value;
                Texture2D st = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/kslash").Value;

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

                if (CalamityEntropy.FlashEffectStrength > 0)
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

                    for (float i = 1; i <= 10; i++)
                    {
                        Main.spriteBatch.Draw(screen, screen.Size() / 2, null, Color.White * ((10f / i) * 0.2f * CalamityEntropy.FlashEffectStrength), 0, screen.Size() / 2, 1 + CalamityEntropy.FlashEffectStrength * 0.1f * i, SpriteEffects.None, 0);
                    }
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    Main.spriteBatch.End();

                }

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

                foreach (NPC npc in Main.ActiveNPCs)
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
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.ModProjectile is StarlessNightProj sl)
                    {
                        sl.drawSword();
                    }
                }
                Main.spriteBatch.End();
            }

            if (cutScreen > 0)
            {
                graphicsDevice.SetRenderTarget(screen);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Util.Util.drawLine(cutScreenCenter, cutScreenCenter + cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 9000, Color.Black, 9000);
                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Util.Util.drawLine(cutScreenCenter, cutScreenCenter + cutScreenRot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * -9000, Color.Black, 9000);

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
            if (BeeGame.Active)
            {
                if (!beegameInited)
                {
                    BeeGame.init();
                    beegameInited = true;
                }
                BeeGame.update();
                graphicsDevice.SetRenderTarget(screen2);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();


                graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                graphicsDevice.Clear(Color.SkyBlue);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);

                BeeGame.draw();

                Main.spriteBatch.End();

                graphicsDevice.SetRenderTarget(Main.screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                Main.spriteBatch.Draw(screen2, Vector2.Zero, Color.White);
                Main.spriteBatch.Draw(Main.screenTargetSwap, new Rectangle((Main.screenWidth - BeeGame.maxWidth) / 2, (Main.screenHeight - BeeGame.maxHeight) / 2, BeeGame.maxWidth, BeeGame.maxHeight), Color.White);

                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (blackMaskTime > 0)
            {
                if (blackMaskAlpha < 1)
                {
                    blackMaskAlpha += 0.05f;
                }
            }
            else
            {
                if (blackMaskAlpha > 0)
                {
                    blackMaskAlpha -= 0.025f;
                }
            }
            Main.spriteBatch.Draw(Util.Util.pixelTex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.5f * blackMaskAlpha);
            Main.spriteBatch.End();
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        public static float blackMaskAlpha = 0;
        public static int blackMaskTime = 0;
        public static Vector2 vLToCenter(Vector2 v, float z)
        {
            return Main.ScreenSize.ToVector2() / 2 + (v - Main.ScreenSize.ToVector2() / 2) * z;
        }
        public bool beegameInited = false;

        public override void Unload()
        {
            LoopSoundManager.unload();
            ealaserSound = null;
            ealaserSound2 = null;
            ArmorPrefix.instances = null;
            Poop.instances = null;
            WallpaperHelper.wallpaper = null;
            efont1 = null;
            efont2 = null;
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
            On_Player.AddBuff -= add_buff;
            On_NPC.AddBuff -= add_buff_npc;
            On_NPC.TargetClosest -= targetClost;
            On_NPC.TargetClosestUpgraded -= targetClostUpgraded;
            On_NPC.FindFrame -= findFrame;
            On_NPC.VanillaAI -= vAi;
            On_NPC.StrikeNPC_HitInfo_bool_bool -= StrikeNpc;
            On_Player.getRect -= modifyRect;
            On_NPC.UpdateNPC -= npcupdate;
            On_Main.DrawInfernoRings -= drawIr;
            On_Main.DrawProjectiles -= DrawBehindPlayer;
            On_Player.Heal -= player_heal;
            On_Main.DrawTiles -= drawtile;
        }

    }
}
