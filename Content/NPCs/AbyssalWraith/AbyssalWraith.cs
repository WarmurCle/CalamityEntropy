using CalamityEntropy.Common;
using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Content.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.Util;
using CalamityMod.Items.Potions;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.AbyssalWraith
{
    [AutoloadBossHead]
    public class AbyssalWraith : ModNPC
    {
        public static int icon = ModContent.GetModBossHeadSlot("CalamityEntropy/Content/NPCs/AbyssalWraith/AbyssalWraith_Head_Boss");
        public static int iconGather = -1;
        public static void loadHead()
        {
            string path = "CalamityEntropy/Content/NPCs/AbyssalWraith/AbyssalWraith_Head_Boss_GatherWing";
            CalamityEntropy.Instance.AddBossHeadTexture(path, -1);
            iconGather = ModContent.GetModBossHeadSlot(path);

        }
        public override void BossHeadSlot(ref int index)
        {

            if (gatherWing > 0.5f)
            {
                index = iconGather;
            }
            else
            {
                index = icon;
            }
        }
        public int animation = 0;
        public int escape = 0;
        public int wingFrame = 0;
        public float camLerp = 0;
        public int seed = -1;
        public float wingRotLeft = 0;
        public float wingRotRight = 0;
        public int lifeCounter = -1;

        public List<Texture2D> wingflying = new List<Texture2D>();
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkNPC.Add(NPC);
            seed = Main.rand.Next(0, 10000);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
            }
        }


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.3f,
                PortraitScale = 0.4f,
                CustomTexturePath = "CalamityEntropy/Assets/Extra/AWBes",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };

            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.AbyssalWraithBestiary")
            });
        }

        public override void SetDefaults()
        {

            NPC.boss = true;
            NPC.width = 140;
            NPC.height = 140;
            NPC.damage = 136;
            if (Main.expertMode)
            {
                NPC.damage += 20;
            }
            if (Main.masterMode)
            {
                NPC.damage += 20;
            }
            NPC.defense = 60;
            NPC.lifeMax = 5500000;
            if (CalamityWorld.death)
            {
                NPC.damage += 20;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 20;
            }
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCHit4;
            NPC.value = 200000f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.netAlways = true;
            NPC.defense = 60;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/boss2");
            }
            for (int i = 1; i <= 8; i++)
            {
                wingflying.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/Wing" + i.ToString()).Value);
            }
            NPC.Entropy().VoidTouchDR = 1f;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animation);
            writer.Write(seed);
            writer.Write(deathAnm);
            writer.Write(lbj);
            writer.Write(portal);
            writer.Write(portalTime);
            writer.WriteVector2(portalPos);
            writer.WriteVector2(portalTarget);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            seed = reader.ReadInt32();
            deathAnm = reader.ReadBoolean();
            lbj = reader.ReadSingle();
            portal = reader.ReadBoolean();
            portalTime = reader.ReadInt32();
            portalPos = reader.ReadVector2();
            portalTarget = reader.ReadVector2();
        }
        public float anmChange = 0;
        public float anmlerp = 1;
        public long counter = 0;
        public float gatherWing = 0;
        public int spawnAnm = 180;
        public Random random;
        public int deathCount = -60;
        public float alphaPor = 1;
        public float portalAlpha = 0;
        public Color[] pixelData = null;
        public bool deathSoundPlay = true;
        public bool looted = false;
        public override void AI()
        {
            NPC.Entropy().VoidTouchTime = 0;
            if (lifeCounter < 0)
            {
                lifeCounter = NPC.lifeMax;
            }
            lifeCounter -= (int)(((float)NPC.lifeMax) / (3f * 60f * 60f));
            if (lifeCounter < 0)
            {
                lifeCounter = 0;
            }
            if (deathAnm && deathSoundPlay && !Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/awdead"));
                deathSoundPlay = false;
            }
            wingRotLeft *= 0.86f;
            wingRotRight *= 0.86f;
            if (Util.Util.getDistance(NPC.Center, Main.LocalPlayer.Center) < 8000 && !NPC.Entropy().ToFriendly && !Main.dedServ)
            {
                Main.LocalPlayer.Entropy().AWraith = true;
            }

            if (spawnAnm > 0 || deathAnm)
            {
                camLerp = camLerp + (1 - camLerp) * 0.06f;
            }
            else
            {
                camLerp = camLerp + (0 - camLerp) * 0.06f;
            }
            if (!Main.dedServ)
            {
                Main.LocalPlayer.Entropy().screenShift = camLerp;
                Main.LocalPlayer.Entropy().screenPos = NPC.Center;
            }
            if (portalTime > 0)
            {
                if (portalAlpha < 1)
                {
                    portalAlpha += 0.05f;
                }

            }
            else
            {
                if (portalAlpha > 0)
                {
                    portalAlpha -= 0.05f;
                }
            }
            if (!NPC.Entropy().ToFriendly && !Main.dedServ)
            {
                Main.LocalPlayer.Entropy().crSky = 10;
            }
            if (spawnAnm > 0)
            {
                NPC.dontTakeDamage = true;
                spawnAnm -= 3;
                if (spawnAnm <= 0)
                {
                    NPC.dontTakeDamage = false;
                    random = new Random(seed);
                }
                return;
            }
            if (NPC.life < 2)
            {
                if (!deathAnm)
                {
                    deathAnm = true;

                    lbj = 6f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    }
                }
            }
            Texture2D deathTex = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/AWDeath").Value;
            if (deathAnm)
            {
                NPC.dontTakeDamage = true;
                NPC.damage = 0;
                lbsize += lbj;
                lbj -= 0.16f;
                if (lbsize <= 0)
                {
                    lbsize = 0;
                }
                deathCount++;
                animation = 0;
                wingFrame = 0;

                if (pixelData == null && !Main.dedServ)
                {
                    pixelData = new Color[deathTex.Width * deathTex.Height];

                    deathTex.GetData(pixelData);
                }
                if (deathPer >= 1 - 0.005f)
                {
                    deathPer = 1 - 0.005f;
                    Kill();
                    return;
                }
                else
                {
                    deathPer += 0.005f;
                    if (deathPer > 1)
                    {
                        deathPer = 1;
                    }
                    if (deathPer > 1 - 0.005f)
                    {
                        deathPer = 1 - 0.005f;
                    }
                    if (!Main.dedServ)
                    {
                        if (pixelData.Length > 1)
                        {
                            Color GetPixelColor(Texture2D texture, Color[] pixelData, int x, int y)
                            {
                                int width = texture.Width;
                                int height = texture.Height;

                                if (x < 0 || x >= width || y < 0 || y >= height || y * width + x >= pixelData.Length)
                                    throw new ArgumentOutOfRangeException("x or y is out of bounds of the texture:" + x.ToString() + "," + y.ToString() + "/" + pixelData.Length.ToString());

                                int index = y * width + x;
                                return pixelData[index];
                            }

                            if (deathPer < 1)
                            {
                                for (int i = 0; i < deathTex.Width; i += 6)
                                {
                                    if (i >= deathTex.Width)
                                    {
                                        continue;
                                    }
                                    if (GetPixelColor(deathTex, pixelData, i, (int)(deathTex.Height * deathPer)).A != 0)
                                    {
                                        Dust.NewDust(NPC.Center + (-deathTex.Size() / 2 + new Vector2(i, (deathTex.Height * deathPer))) * NPC.scale, 1, 1, ModContent.DustType<AwDeath>());
                                    }
                                }
                            }
                        }
                        else
                        {
                            pixelData = new Color[deathTex.Width * deathTex.Height];

                            deathTex.GetData(pixelData);
                        }
                    }

                }

                Stand();
                return;
            }
            if (!Main.dedServ)
            {
                spawnParticle();
            }
            checkAnm();
            if (portalTime > 0)
            {
                animation = 1;
                portalTime--;
                if (portalTime <= 0)
                {
                    animation = 0;
                }
            }
            if (animation == 0 && gatherWing <= 0)
            {
                updateWingAnm();
            }
            if (portalTime > 0)
            {

                if (portal)
                {
                    NPC.velocity.X *= 0;
                    NPC.rotation = 0;
                    NPC.velocity.Y += 0.5f;
                    if (NPC.Center.Y > portalPos.Y - 120 && NPC.velocity.Y > 0)
                    {
                        alphaPor -= 0.1f;
                        if (alphaPor <= 0)
                        {
                            alphaPor = 0;
                            portal = false;
                            NPC.Center = portalTarget + new Vector2(0, 60);
                            NPC.velocity.Y *= -1;
                            NPC.netUpdate = true;
                            portalTime = 40;
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }
                    }
                }
                if (!portal)
                {
                    NPC.velocity *= 0.95f;
                }
            }
            if (!portal && alphaPor < 1)
            {
                alphaPor += 0.1f;
            }
            NPC.target = NPC.FindClosestPlayer();
            if (portalTime <= 0)
            {
                if (NPC.HasValidTarget)
                {
                    Player target = NPC.target.ToPlayer();
                    escape = 0;
                    if (NPC.Entropy().ToFriendly && NPC.Entropy().f_target == -1)
                    {
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 10;
                        animation = 0;
                        KeepDist(200, 400);
                    }
                    if (NPC.ai[2] <= 0)
                    {
                        if (NPC.ai[3] > 0)
                        {
                            NPC.ai[3]--;
                            stayAtPlayerUp();
                        }
                        else
                        {
                            int t = random.Next(0, 9);
                            NPC.ai[3] = random.Next(30, 100);
                            NPC.netUpdate = true;
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                            NPC.ai[1] = t;
                            if (t == 0)
                            {
                                NPC.ai[2] = 80;
                            }
                            if (t == 1)
                            {
                                NPC.ai[2] = 90;
                            }
                            if (t == 2)
                            {
                                NPC.ai[2] = 240;
                            }
                            if (t == 3)
                            {
                                NPC.ai[2] = 100;
                            }
                            if (t == 4)
                            {
                                NPC.ai[2] = 80;
                            }
                            if (t == 5)
                            {
                                NPC.ai[2] = 290;
                            }
                            if (t == 6)
                            {
                                NPC.ai[2] = 160;
                            }
                            if (t == 7)
                            {
                                NPC.ai[2] = 80;
                            }
                            if (t == 8)
                            {
                                NPC.ai[2] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (NPC.ai[1] == 0)
                        {
                            Stand();
                            if (NPC.ai[2] == 80)
                            {
                                animation = 1;
                            }
                            if (NPC.ai[2] == 30)
                            {
                                animation = 0;
                            }
                            if (NPC.ai[2] == 4 || NPC.ai[2] == 18)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    ThrowALightBall();
                                }
                            }
                        }
                        if (NPC.ai[1] == 1)
                        {

                            if (NPC.ai[2] == 90)
                            {
                                animation = 1;
                            }
                            if (NPC.ai[2] < 50)
                            {
                                Stand();
                                animation = 0;
                                wingFrame = 7;
                                anmChange = 0;
                            }
                            else
                            {
                                KeepDist(300);
                            }
                            if (addlight < 1)
                            {
                                addlight += 0.05f;
                            }

                            if (NPC.ai[2] > 12 && NPC.ai[2] < 50)
                            {
                                int c = (int)((50 - NPC.ai[2]) / 2f);
                                if (NPC.ai[2] % 9 == 0)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        float rot = -MathHelper.ToRadians(8f * (float)c / 2f);
                                        for (int i = 1; i <= c; i++)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedBy(rot) * 4, ModContent.ProjectileType<SighterPin>(), NPC.damage / 8, 4);
                                            rot += MathHelper.ToRadians(16);
                                        }
                                    }
                                }
                            }
                        }
                        if (NPC.ai[1] == 2)
                        {
                            Stand();
                            animation = 1;
                            if (NPC.ai[2] > 5)
                            {
                                NPC.dontTakeDamage = true;
                            }
                            if (NPC.ai[2] < 5)
                            {
                                animation = 0;
                                NPC.dontTakeDamage = false;
                            }
                            if (NPC.ai[2] > 40)
                            {
                                if (NPC.ai[2] % 60 == 0)
                                {
                                    int a = (int)MathHelper.ToDegrees(Util.Util.randomRot());
                                    for (int i = 0; i < 5; i++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + MathHelper.ToRadians(a).ToRotationVector2() * 2200, (MathHelper.ToRadians(a)).ToRotationVector2() * -24, ModContent.ProjectileType<VoidLightBall>(), (int)(NPC.damage * 0.14f), 6, -1, 0, 1, NPC.whoAmI);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + MathHelper.ToRadians(a).ToRotationVector2() * -2200, (MathHelper.ToRadians(a)).ToRotationVector2() * 24, ModContent.ProjectileType<VoidLightBall>(), (int)(NPC.damage * 0.14f), 6, -1, 0, 1, NPC.whoAmI);

                                        }
                                        a += 36;

                                    }
                                }
                            }
                        }
                        if (NPC.ai[1] == 3)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                KeepDist(700);
                                if (NPC.ai[2] < 60 && NPC.ai[2] % 5 == 0)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center) * 0.06f, ModContent.ProjectileType<VoidLightBall>(), (int)(NPC.damage * 0.15f), 6, -1, 0, 0, NPC.whoAmI);
                                }
                            }
                        }
                        if (NPC.ai[1] == 4)
                        {
                            Stand();
                            if (NPC.ai[2] == 80)
                            {
                                animation = 1;
                            }
                            if (NPC.ai[2] > 50)
                            {
                                if (addlight < 1)
                                {
                                    addlight += 0.05f;
                                }
                            }
                            if (NPC.ai[2] == 45)
                            {
                                animation = 0;
                            }
                            if (NPC.ai[2] < 40)
                            {
                                wingFrame = 0;
                                anmChange = 0;
                                float j = (float)Math.Cos(NPC.ai[2]) * 0.2f;
                                if (target.Center.X < NPC.Center.X)
                                {
                                    wingRotLeft += j;
                                }
                                else
                                {
                                    wingRotRight += j;
                                }
                            }
                            if (NPC.ai[2] < 30)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (target.Center.X > NPC.Center.X ? 1 : -1) * new Vector2(140 + Main.rand.Next(0, 60), 0) + new Vector2(0, -50), (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + new Vector2(9 * (Main.rand.NextFloat() - 0.5f), 6 * (Main.rand.NextFloat() - 0.5f)), ModContent.ProjectileType<VoidFeather>(), NPC.damage / 7, 6, -1, 0, NPC.whoAmI);
                                    }

                                }

                                SoundEffect se = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/feathershot").Value;
                                if (se != null && NPC.ai[2] % 5 == 0) { se.Play(Main.soundVolume, 0, 0); }
                            }
                        }
                        if (NPC.ai[1] == 5)
                        {
                            Stand();
                            if (NPC.ai[2] == 290)
                            {
                                NPC.dontTakeDamage = true;
                                animation = 1;
                            }
                            if (NPC.ai[2] == 280)
                            {

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<HugeLightBall>(), NPC.damage / 7 + 5, 10);
                                }
                            }
                            if (NPC.ai[2] == 2)
                            {
                                animation = 0;
                                NPC.dontTakeDamage = false;
                            }
                        }
                        if (NPC.ai[1] == 6)
                        {
                            KeepDist(1200);
                            NPC.ai[2] += 0.5f;
                            if (NPC.ai[2] == 160 || NPC.ai[2] == 120 || NPC.ai[2] == 80 || NPC.ai[2] == 40)
                            {
                                animation = 1;
                            }
                            if (NPC.ai[2] == 140 || NPC.ai[2] == 100 || NPC.ai[2] == 60 || NPC.ai[2] == 20)
                            {
                                animation = 0;
                                float a = Util.Util.randomRot();
                                for (int i = 0; i < 18; i++)
                                {
                                    a += MathHelper.ToRadians(20);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, a.ToRotationVector2() * 48, ModContent.ProjectileType<VoidLightBall>(), (int)(NPC.damage * 0.14f), 6, -1, 0, 1, NPC.whoAmI);
                                    }
                                }
                            }
                        }
                        if (NPC.ai[1] == 7)
                        {
                            Stand();
                            if (NPC.ai[2] == 80)
                            {
                                animation = 1;
                            }
                            if (NPC.ai[2] == 30)
                            {
                                animation = 0;
                            }
                            if (NPC.ai[2] == 4 || NPC.ai[2] == 18)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    ThrowALightBall(ModContent.ProjectileType<HomingLightBall>());
                                }
                            }
                        }
                        if (NPC.ai[1] == 8)
                        {
                            /*
                                                        if (NPC.ai[2] > 220)
                                                        {
                                                            if (addlight < 1)
                                                            {
                                                                addlight += 0.05f;
                                                            }
                                                            animation = 1;
                                                            Stand();
                                                        }
                                                        if (NPC.ai[2] == 220)
                                                        {
                                                            NPC.rotation = (target.Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
                                                            NPC.velocity *= 0;

                                                        }
                                                        if (NPC.ai[2] >= 220)
                                                        {
                                                            animation = 1;
                                                        }
                                                        if (NPC.ai[2] == 220)
                                                        {

                                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                            {
                                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0, 30), (target.Center - NPC.Center).SafeNormalize(new Vector2(1, 0)) * MathHelper.Max(200, MathHelper.Min(Util.Util.getDistance(NPC.Center, target.Center), 660)), ModContent.ProjectileType<AbyssalLaser>(), NPC.damage / 6, 6, -1, 0, 0, NPC.whoAmI);
                                                            }
                                                        }
                                                        if (NPC.ai[2] == 2)
                                                        {
                                                            animation = 0;
                                                        }*/
                        }
                        NPC.ai[2]--;
                    }


                    if (counter % 10 == 0 && Util.Util.getDistance(NPC.Center, target.Center) > 4000)
                    {
                        setPortalTo(target.Center + new Vector2(Main.rand.Next(-300, 301), 100));
                    }
                }
                else
                {
                    escape++;
                    NPC.velocity.Y -= 1;
                    NPC.velocity *= 0.98f;
                    animation = 0;
                    if (escape >= 160)
                    {
                        NPC.active = false;
                    }
                }
            }

            NPC.netUpdate = true;
            counter++;
        }

        private void Kill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.StrikeInstantKill();
                NPC.netSpam = 9;
                NPC.netUpdate = true;
            }
        }

        private void ThrowALightBall()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Util.Util.randomRot().ToRotationVector2() * (float)Main.rand.Next(100, 200) * 0.1f, ModContent.ProjectileType<VoidLightBall>(), (int)(NPC.damage * 0.14f), 6, -1, 0, 0, NPC.whoAmI);
            }
        }
        private void ThrowALightBall(int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Util.Util.randomRot().ToRotationVector2() * (float)Main.rand.Next(100, 200) * 0.1f, type, (int)(NPC.damage * 0.14f), 6, -1, 0, 0, NPC.whoAmI);
            }
        }

        private void checkAnm()
        {
            if (animation == 0)
            {
                if (gatherWing > 0)
                {
                    gatherWing -= 0.05f;
                    if (gatherWing < 0)
                    {
                        gatherWing = 0;
                    }
                    wingFrame = 0;
                    anmChange = 0;
                }
                anmlerp = anmlerp + (1 - anmlerp) * 0.1f;
            }
            else if (animation == 1)
            {
                if (gatherWing < 1)
                {
                    gatherWing += 0.05f;
                    if (gatherWing >= 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/wingflap"), NPC.Center);
                        gatherWing = 1;
                    }
                }

                anmlerp = anmlerp + (0 - anmlerp) * 0.1f;
                wingFrame = 0;
                anmChange = 0;
            }
        }
        public bool portal = false;
        public int portalTime = 0;
        public Vector2 portalPos = Vector2.Zero;
        public Vector2 portalTarget = Vector2.Zero;
        public void setPortalTo(Vector2 targetpos)
        {
            portalTarget = targetpos;
            portal = true;
            portalTime = 360;
            portalPos = NPC.Center + new Vector2(0, 100);
            NPC.velocity = new Vector2(0, -16);
            NPC.netUpdate = true;
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;
        }
        private void updateWingAnm()
        {
            anmChange += 1.5f;
            if (wingFrame == 0 && anmChange >= 9)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 1 && anmChange >= 6)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 2 && anmChange >= 6)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 3 && anmChange >= 6)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 4 && anmChange >= 6)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 5 && anmChange >= 6)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame == 6 && anmChange >= 9)
            {
                anmChange = 0;
                wingFrame++;
            }
            if (wingFrame >= 7 && anmChange >= 12)
            {
                anmChange = 0;
                wingFrame = 0;
            }
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public void stayAtPlayerUp()
        {
            Player target = NPC.target.ToPlayer();
            Vector2 pos = target.Center - new Vector2(0, 120);
            if (Util.Util.getDistance(NPC.Center, pos) > 240 || NPC.velocity.Length() < 4)
            {
                NPC.velocity += (pos - NPC.Center).SafeNormalize(Vector2.Zero) * 0.4f;
                NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 1.4f);
                NPC.velocity *= 0.99f;
            }
        }
        public void Stand()
        {
            NPC.velocity *= 0.9f;
            NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 1.4f);
        }
        public void KeepDist(float dist, float maxDist = -1)
        {
            Player target = NPC.target.ToPlayer();
            if (maxDist >= 0 && Util.Util.getDistance(NPC.Center, target.Center) >= dist && Util.Util.getDistance(NPC.Center, target.Center) <= maxDist)
            {
                NPC.velocity *= 0.94f;
                return;
            }
            Vector2 pos = target.Center + (NPC.Center - target.Center).SafeNormalize(Vector2.One) * dist;
            NPC.velocity += (pos - NPC.Center).SafeNormalize(Vector2.Zero) * 1f;
            NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 1.4f);
            NPC.velocity *= 0.98f;

        }

        public void spawnParticle()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 direction = new Vector2(0, 1).RotatedBy(NPC.rotation);
                Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.1f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                CalamityMod.Particles.Particle smoke = new HeavySmokeParticle(NPC.Center + direction * 46f, smokeSpeed + NPC.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.NextBool(3))
                {
                    CalamityMod.Particles.Particle smokeGlow = new HeavySmokeParticle(NPC.Center + direction * 46f, smokeSpeed + NPC.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0.01f, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }

        }
        public override void OnKill()
        {

            NPC.SetEventFlagCleared(ref EDownedBosses.downedAbyssalWraith, -1);
        }

        public override bool CheckActive()
        {
            return false;
        }
        Effect aweffect = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/aweffect").Value;
        public float addlight = 1;

        public void Draw()
        {
            Vector2 drawCenter = NPC.Center - new Vector2(0, 30);
            Texture2D body = TextureAssets.Npc[NPC.type].Value;
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            aweffect.CurrentTechnique = aweffect.Techniques["Technique1"];
            float alpha = 1;
            if (spawnAnm > 0)
            {
                addlight = 1;
            }
            else
            {
                if (deathAnm)
                {
                    if (addlight < 1)
                    {
                        addlight += 0.02f;

                    }
                    anmlerp = anmlerp + (10 - anmlerp) * 0.01f;
                    Texture2D deathTex = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/AWDeath").Value;

                    sb.Draw(deathTex, NPC.Center - Main.screenPosition + new Vector2(0, deathTex.Height / 2 * deathPer), new Rectangle(0, (int)(deathTex.Height * deathPer), deathTex.Width, (int)(deathTex.Height * (1 - deathPer))), Color.White, 0, new Vector2(deathTex.Width / 2, deathTex.Height * (1 - deathPer) / 2), NPC.scale, SpriteEffects.None, 0);
                    return;
                }
                else
                {
                    if (addlight > 0)
                    {
                        addlight -= 0.02f;
                    }
                }
            }
            aweffect.Parameters["a"].SetValue(addlight);
            aweffect.CurrentTechnique.Passes[0].Apply();


            if (spawnAnm > 20)
            {
                alpha = (200f - (float)spawnAnm) / 180f;
            }
            aweffect.Parameters["alpha"].SetValue(alpha * alphaPor);
            if (gatherWing <= 0)
            {
                Texture2D wing = wingflying[wingFrame];

                float rot = 0;
                if (spawnAnm > 0)
                {
                    rot = MathHelper.ToRadians(spawnAnm * 6f);
                }
                if (spawnAnm <= 20)
                {
                    Vector2 origin = new Vector2(320, 222);
                    sb.Draw(wing, drawCenter + new Vector2(-64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation - rot - wingRotLeft, origin, NPC.scale, SpriteEffects.None, 0);
                    origin = new Vector2(wing.Width - origin.X, origin.Y);
                    sb.Draw(wing, drawCenter + new Vector2(64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation + rot + wingRotRight, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else
            {
                if (gatherWing <= 0.2f)
                {
                    float rot = MathHelper.ToRadians(gatherWing * 160);
                    Texture2D wing = wingflying[0];
                    Vector2 origin = new Vector2(320, 222);
                    sb.Draw(wing, drawCenter + new Vector2(-64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation - rot, origin, NPC.scale, SpriteEffects.None, 0);
                    origin = new Vector2(wing.Width - origin.X, origin.Y);
                    sb.Draw(wing, drawCenter + new Vector2(64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation + rot, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0);

                }
                else if (gatherWing <= 0.5f)
                {

                }
                else if (gatherWing <= 0.8f)
                {

                }
                else
                {
                }
            }

            sb.Draw(body, drawCenter - Main.screenPosition, null, Color.White * alpha, NPC.rotation, body.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            if (gatherWing <= 0.2f)
            {

            }
            else if (gatherWing <= 0.5f)
            {
                float rot = 0;
                Texture2D wing = wingflying[6];
                Vector2 origin = new Vector2(320, 222);
                sb.Draw(wing, drawCenter + new Vector2(-64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation - rot, origin, NPC.scale, SpriteEffects.None, 0);
                origin = new Vector2(wing.Width - origin.X, origin.Y);
                sb.Draw(wing, drawCenter + new Vector2(64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation + rot, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else if (gatherWing <= 0.8f)
            {
                float rot = 0;
                Texture2D wing = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/WingGathering").Value;
                Vector2 origin = new Vector2(320, 222);
                sb.Draw(wing, drawCenter + new Vector2(-64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation - rot, origin, NPC.scale, SpriteEffects.None, 0);
                origin = new Vector2(wing.Width - origin.X, origin.Y);
                sb.Draw(wing, drawCenter + new Vector2(64, 0).RotatedBy(NPC.rotation) - Main.screenPosition, null, Color.White * alpha, NPC.rotation + rot, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else if (gatherWing <= 0.9f)
            {
                Texture2D wing = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/WingGather").Value;
                Vector2 origin = wing.Size() / 2;
                origin.Y = 222;
                sb.Draw(wing, drawCenter - Main.screenPosition, null, Color.White * alpha, NPC.rotation, origin, NPC.scale * new Vector2(1 - (gatherWing - 0.8f) * 10f * 0.5f, 1), SpriteEffects.None, 0);

            }
            else
            {
                Texture2D wing = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/AbyssalWraith/WingGather").Value;
                Vector2 origin = wing.Size() / 2;
                origin.Y = 222;
                sb.Draw(wing, drawCenter - Main.screenPosition, null, Color.White * alpha, NPC.rotation, origin, NPC.scale * new Vector2(0.5f + (gatherWing - 0.9f) * 10f * 0.5f, 1), SpriteEffects.None, 0);

            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D htx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;

            Main.spriteBatch.Draw(htx, NPC.Center - Main.screenPosition, null, Color.White, 0, htx.Size() / 2, 1f * lbsize, SpriteEffects.None, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public float lbsize = 0;
        public float lbj = 0;
        public float deathPer = 0;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !deathAnm;
        }
        public override bool CheckDead()
        {
            if (deathPer >= 0.9f)
            {
                return true;
            }
            animation = 0;
            gatherWing = 0;
            NPC.damage = 0;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            NPC.active = true;

            NPC.netUpdate = true;

            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;
            return false;
        }
        public void DrawPortal(Vector2 pos, Color color, float size, float xmul = 0.3f, float aj = 0)
        {
            Texture2D tx = Util.Util.getExtraTex("SoulVortex");
            float angle = MathHelper.ToDegrees(counter * 0.2f + aj);
            Vector2 lu = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle - 135));
            Vector2 ru = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle - 45));
            Vector2 ld = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle + 135));
            Vector2 rd = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle + 45));

            lu.X *= xmul;
            ru.X *= xmul;
            ld.X *= xmul;
            rd.X *= xmul;

            Vector2 dp = pos - Main.screenPosition;
            float rangle = MathHelper.ToRadians(90);
            lu = lu.RotatedBy(rangle);
            ru = ru.RotatedBy(rangle);
            ld = ld.RotatedBy(rangle);
            rd = rd.RotatedBy(rangle);

            Util.Util.drawTextureToPoint(Main.spriteBatch, tx, color, dp + lu, dp + ru, dp + ld, dp + rd);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPosition, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;
            return false;
        }
        public int lastLife = 0;
        public bool checkLife = false;
        public int maxDmgCanTake = 0;
        public float getDR()
        {
            return (NPC.life < lifeCounter ? (1 / (1 + (lifeCounter - NPC.life) * 0.000004f)) : 1);
        }
        public int getMaxDamageCanTake()
        {
            if (deathAnm)
            {
                return -1;
            }
            return Math.Min(NPC.life - 1, (int)(20000 * getDR()));
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            lastLife = NPC.life;
            checkLife = true;
            maxDmgCanTake = getMaxDamageCanTake();
            if (maxDmgCanTake > 0)
            {
                modifiers.SetMaxDamage(maxDmgCanTake);
            }


        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            //if (false)
            //{
            //    if (lastLife - NPC.life > maxDmgCanTake)
            //    {
            //        NPC.life = lastLife - maxDmgCanTake;
            //    }
            //    checkLife = false;
            //}
        }
        public bool deathAnm = false;

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }


    }
}
