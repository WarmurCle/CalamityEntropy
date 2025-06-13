using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.SoulCards;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.LuminarisShoots;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.LuminarisMoth
{
    [AutoloadBossHead]
    public class Luminaris : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.48f,
                PortraitScale = 0.56f,
                CustomTexturePath = "CalamityEntropy/Assets/BCL/LuminarisBossCheckList",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = -4
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public List<Vector2> odp = new List<Vector2>();
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.LuminarisBestiary")
            });
        }

        public enum AIStyle
        {
            RoundShooting,
            AstralSpike,
            AboveMovingShooting,
            Waiting1Sec,
            Subduction,
            StayAboveAndShooting,
            Dashing,

            Shoot360,
            RoundAndDash,
            SmashDown,
            ShootTriangle
        }
        public Vector2 vec1 = Vector2.Zero;
        public Vector2 vec2 = Vector2.Zero;
        public float num1 = 0;
        public float num2 = 0;
        public float num3 = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(vec1);
            writer.WriteVector2(vec2);
            writer.Write(num1);
            writer.Write(num2);
            writer.Write(num3);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            vec1 = reader.ReadVector2();
            vec2 = reader.ReadVector2();
            num1 = reader.ReadSingle();
            num2 = reader.ReadSingle();
            num3 = reader.ReadSingle();
        }
        public override void SetDefaults()
        {
            
            NPC.boss = true;
            NPC.width = 76;
            NPC.height = 76;
            NPC.damage = 55;
            NPC.Calamity().DR = 0.1f;
            NPC.defense = 10;
            NPC.lifeMax = 22000;
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.value = 1600f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.timeLeft *= 20;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/LuminarisBoss");
            }
        }
        public int frameCounter = 0;
        public Vector2 oldPos = Vector2.Zero;
        public int AIRound = 0;
        public AIStyle ai = AIStyle.RoundShooting;
        public int AfterImageTime = 0;
        public int SD = 4;
        public override void AI()
        {
            if (SD-- > 0)
            {
                return;
            }
            if (MegaTrail > 0)
            {
                MegaTrail -= 0.05f;
            }
            if (oldPos == Vector2.Zero)
            {
                oldPos = NPC.Center;
            }
            frameCounter++;
            if(AfterImageTime > 0)
            {
                AfterImageTime--;
            }
            if (tail1 == null || tail2 == null)
            {
                tail1 = new Rope(NPC.Center, 10, 11.6f, new Vector2(0, 0.14f), 0.054f, 30);
                tail2 = new Rope(NPC.Center, 10, 11.6f, new Vector2(0, 0.14f), 0.054f, 30);
            }
            if (NPC.life <= NPC.lifeMax / 2)
                phase = 2;
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            if (NPC.HasValidTarget)
            {
                AttackPlayer(Main.player[NPC.target]);
            }
            else
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 0.2f;
            }
            for (float i = 0; i <= 1; i += 0.25f)
            {
                Vector2 tail1Pos = NPC.velocity + Vector2.Lerp(oldPos, NPC.Center, i) + new Vector2(-14, 32).RotatedBy(NPC.rotation) * NPC.scale;
                Vector2 tail2Pos = NPC.velocity + Vector2.Lerp(oldPos, NPC.Center, i) + new Vector2(14, 32).RotatedBy(NPC.rotation) * NPC.scale;
                tail1.Start = tail1Pos;
                tail2.Start = tail2Pos;
                tail1.gravity = new Vector2(0, 0.12f);
                tail2.gravity = new Vector2(0, 0.12f);
                tail1.Update();
                tail2.Update();
            }
            oldPos = NPC.Center;
            odp.Add(NPC.Center);
            int odMax = 24 + (int)(MegaTrail) * 16;
            for (int i = 0; i < 3; i++)
            {
                if (odp.Count > odMax)
                {
                    odp.RemoveAt(0);
                }
            }
        }

        public void AttackPlayer(Player player)
        {
            float enrange = 1;
            if(Main.expertMode)
            {
                enrange += 0.1f;
            }
            if (Main.masterMode)
            {
                enrange += 0.1f;
            }
            if (CalamityWorld.revenge)
            {
                enrange += 0.15f;
            }
            if (CalamityWorld.death)
            {
                enrange += 0.15f;
            }
            if (CalamityEntropy.EntropyMode)
            {
                enrange *= 1.4f;
            }
            if (Main.getGoodWorld)
            {
                enrange *= 1.1f;
            }
            if (Main.zenithWorld)
            {
                enrange *= 1.4f;
            }
            if(AIChangeCounter-- < 0)
            {
                vec1 = vec2 = Vector2.Zero;
                num1 = num2 = num3 = 0;
                SetAISyyle();
                AIRound++;
                AIChangeCounter = 20;
                if (ai == AIStyle.RoundShooting)
                {
                    AIChangeCounter = 260;
                }
                if (ai == AIStyle.AboveMovingShooting)
                {
                    AIChangeCounter = 290;
                }
                if(ai == AIStyle.Waiting1Sec)
                {
                    AIChangeCounter = 60;
                }
                if(ai == AIStyle.Subduction)
                {
                    AIChangeCounter = 220;
                }
                if (ai == AIStyle.StayAboveAndShooting)
                {
                    AIChangeCounter = 240;
                }
                if(ai == AIStyle.Dashing)
                {
                    AIChangeCounter = 200;
                }
                if(ai == AIStyle.AstralSpike)
                {
                    AIChangeCounter = 100;
                }
                if(ai == AIStyle.Shoot360)
                {
                    AIChangeCounter = 160;
                }
                if(ai == AIStyle.RoundAndDash)
                {
                    AIChangeCounter = 200;
                }
                if (ai == AIStyle.SmashDown)
                {
                    AIChangeCounter = 400;
                }
                if(ai == AIStyle.ShootTriangle)
                {
                    AIChangeCounter = 200;
                }
;               NPC.netUpdate = true;
            }
            if(ai == AIStyle.RoundShooting)
            {
                NPC.velocity *= 0;
                NPC.rotation = 0;
                AfterImageTime = 16;
                if (AIChangeCounter == 260)
                {
                    num3 = Main.rand.NextBool() ? -1 : 1;
                    vec1 = NPC.Center;
                }
                if(AIChangeCounter > 220 && AIChangeCounter < 259)
                {
                    NPC.Center = Vector2.Lerp(vec1, player.Center + new Vector2(440 * Math.Sign(NPC.Center.X - player.Center.X), -440), CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 220) / 40f, 1));
                }
                if(AIChangeCounter == 220)
                {
                    num1 = NPC.Center.Distance(player.Center);
                    num2 = (NPC.Center - player.Center).ToRotation();
                }
                if(AIChangeCounter < 220)
                {
                    for (int i = 0; i < odp.Count; i++)
                    {
                        odp[i] += player.velocity;
                    }
                    num2 += 0.06f * num3 * enrange;
                    NPC.Center = player.Center + num2.ToRotationVector2() * num1;
                    if(AIChangeCounter % (int)(38 / enrange) == 0)
                    {
                        CEUtils.PlaySound("bne_hit2", 1, NPC.Center);
                        Shoot<LuminarisVortex>(NPC.Center, (player.Center - NPC.Center).normalize() * 8);
                    }
                    NPC.rotation = (NPC.Center - oldPos).ToRotation() + MathHelper.PiOver2;
                }
            }
            if (ai == AIStyle.AboveMovingShooting)
            {
                NPC.rotation = 0;
                NPC.velocity *= 0;
                AfterImageTime = 16;
                if (AIChangeCounter <= 260)
                {
                    if (AIChangeCounter > 240)
                    {
                        if (AIChangeCounter == 260)
                        {
                            num3 = Main.rand.NextBool() ? -1 : 1;
                            vec1 = NPC.Center;
                        }
                        NPC.Center = Vector2.Lerp(vec1, player.Center + new Vector2(360 * Math.Sign(NPC.Center.X - player.Center.X), -440), CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 240) / 20f, 1));
                    }
                    else if (AIChangeCounter > 220)
                    {
                        if (AIChangeCounter == 240)
                        {
                            vec1 = NPC.Center;
                        }
                        NPC.Center = Vector2.Lerp(vec1, player.Center + new Vector2(0, -420), CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 220) / 20f, 1));

                    }
                    if (AIChangeCounter < 220)
                    {
                        float f = AIChangeCounter < 160 ? 1 : 1 - ((AIChangeCounter - 160) / 60f);
                        NPC.Center = new Vector2(player.Center.X + (float)(Math.Cos(AIChangeCounter * 0.056f)) * 800 * f, NPC.Center.Y + ((player.Center.Y - 500) - NPC.Center.Y) * 0.1f);
                        NPC.rotation = (NPC.Center - oldPos).X * 0.01f;
                        if (AIChangeCounter < 156)
                        {
                            if (AIChangeCounter % (int)(8f / enrange) == 0)
                            {
                                Shoot<LuminarisAstralShoot>(player.Center + new Vector2(-1400, 340), Vector2.UnitX * 26 * enrange, 1, Vector2.UnitY.ToRotation(), -0.3f * enrange, 30);
                                Shoot<LuminarisAstralShoot>(player.Center + new Vector2(1400, 340), Vector2.UnitX * -26 * enrange, 1, Vector2.UnitY.ToRotation(), -0.3f * enrange, 30);
                                Shoot<LuminarisAstralShoot>(NPC.Center, Vector2.UnitY * 14 * enrange, 1, (-Vector2.UnitY).ToRotation(), 0.3f * enrange, 30);
                                EParticle.NewParticle(new HadLine(), player.Center + player.velocity + new Vector2((float)(Math.Cos((AIChangeCounter - (int)(8f / enrange) * 1) * 0.056f)) * 800, -516), Vector2.Zero, Color.LightBlue * 0.82f, 1, 1, true, BlendState.Additive, MathHelper.PiOver2);
                            }
                        }
                    }
                    if (AIChangeCounter == 220)
                    {
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), 1800f, 1000f, 0f, 4.5f);
                        CalamityEntropy.FlashEffectStrength = 0.36f;
                        CEUtils.PlaySound("ksLand", 1.3f, NPC.Center);
                        for (int i = 0; i < 14; i++)
                        {
                            Shoot<LuminarisAstralShoot>(NPC.Center, new Vector2(16 * (Main.rand.NextBool() ? 1 : -1), 12).RotatedByRandom(0.3f) * enrange, 1, Vector2.UnitY.ToRotation(), 0.6f * enrange, 6);
                        }
                    }
                }
            }
            if(ai == AIStyle.Waiting1Sec)
            {
                NPC.rotation = 0;
            }
            if (ai == AIStyle.Subduction)
            {
                AfterImageTime = 16;
                if (AIChangeCounter == 220)
                {
                    vec1 = NPC.Center;
                    vec2 = player.Center + new Vector2(Math.Sign(NPC.Center.X - player.Center.X) * 380 * enrange, -400);
                }
                if (AIChangeCounter > 200)
                {
                    vec2 = player.Center + new Vector2(Math.Sign(NPC.Center.X - player.Center.X) * 380 * enrange, -400);
                    NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(Utils.Remap(AIChangeCounter, 220, 200, 0, 1), 1));
                }

                if (AIChangeCounter == 200)
                {
                    vec1 = NPC.Center;
                    vec2 = player.Center - new Vector2(NPC.Center.X - player.Center.X, 0);
                    vec2.Y = NPC.Center.Y;
                }
                if (AIChangeCounter <= 200 && AIChangeCounter >= 170)
                {
                    float p = (1 - (AIChangeCounter - 170f) / 30f);
                    NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(p, 1)) + new Vector2(0, (float)(Math.Cos(MathHelper.TwoPi * p - MathHelper.Pi) * 0.5f + 0.5f) * 400f);
                    NPC.rotation = (NPC.Center - oldPos).ToRotation() + MathHelper.PiOver2;
                }

                if (AIChangeCounter <= 150)
                {
                    if (AIChangeCounter == 150)
                    {
                        vec1 = NPC.Center;
                        vec2 = player.Center + new Vector2(Math.Sign(NPC.Center.X - player.Center.X) * 380 * enrange, -400);
                    }
                    if (AIChangeCounter > 130)
                    {
                        vec2 = player.Center + new Vector2(Math.Sign(NPC.Center.X - player.Center.X) * 380 * enrange, -400);
                        NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(Utils.Remap(AIChangeCounter, 150, 130, 0, 1), 1));
                    }

                    if (AIChangeCounter == 130)
                    {
                        vec1 = NPC.Center;
                        vec2 = player.Center - new Vector2(NPC.Center.X - player.Center.X, 0);
                        vec2.Y = NPC.Center.Y;
                    }
                    if (AIChangeCounter < 130 && AIChangeCounter >= 100)
                    {
                        float p = (1 - (AIChangeCounter - 100f) / 30f);
                        NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(p, 1)) + new Vector2(0, (float)(Math.Cos(MathHelper.TwoPi * p - MathHelper.Pi) * 0.5f + 0.5f) * 400f);
                        NPC.rotation = (NPC.Center - oldPos).ToRotation() + MathHelper.PiOver2;
                    }
                }
                if(AIChangeCounter < 100)
                {
                    NPC.velocity += (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.6f;
                    NPC.velocity *= 0.99f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                }
                if(AIChangeCounter == 1)
                {
                    NPC.velocity *= 0;
                    NPC.rotation = 0;
                }
            }
            if (ai == AIStyle.StayAboveAndShooting)
            {
                NPC.rotation = 0;
                NPC.velocity += (player.Center + new Vector2(600 * Math.Sign(NPC.Center.X - player.Center.X), -400) / enrange - NPC.Center).normalize() * 1f;
                NPC.velocity *= 0.97f;
                if (AIChangeCounter > 60 / enrange && AIChangeCounter % (int)(40 / enrange) == 0 && AIChangeCounter < 150 * enrange)
                {
                    NPC.velocity -= (player.Center - NPC.Center).normalize() * 6;
                    for (int i = 0; i < 10 * enrange; i++)
                    {
                        Shoot<LuminarisAstralShoot>(NPC.Center, (player.Center - NPC.Center).normalize() * 6 * enrange, 0.9f, CEUtils.randomRot(), 0.16f * enrange, 4 * enrange);
                    }
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), 1800f, 1000f, 0f, 2f);
                    CEUtils.PlaySound("ksLand", 1.6f, NPC.Center, volume: 0.5f);

                }
            }
            if(ai == AIStyle.Dashing)
            {
                AfterImageTime = 16;
                if(AIChangeCounter == 200)
                {
                    num1 = NPC.rotation;
                    num2 = (player.Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
                }
                if(AIChangeCounter <= 200 && AIChangeCounter >= 180) 
                {
                    NPC.rotation = CEUtils.rotatedToAngle(num1, num2, CEUtils.GetRepeatedCosFromZeroToOne(Utils.Remap(AIChangeCounter, 200, 180, 0, 1), 1), false);
                }
                if (AIChangeCounter == 130)
                {
                    num1 = NPC.rotation;
                    num2 = (player.Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
                }
                if (AIChangeCounter <= 130 && AIChangeCounter >= 110)
                {
                    NPC.rotation = CEUtils.rotatedToAngle(num1, num2, Utils.Remap(AIChangeCounter, 130, 110, 0, 1), false);
                }
                if(AIChangeCounter < 110 || (AIChangeCounter > 130 && AIChangeCounter < 180))
                {
                    NPC.velocity = (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 40;
                    NPC.rotation = CEUtils.rotatedToAngle(NPC.rotation, (player.Center - NPC.Center).ToRotation() + MathHelper.PiOver2, 1f);
                }
            }
            if(ai == AIStyle.AstralSpike)
            {
                NPC.velocity *= 0;
                NPC.rotation = 0;
                if(AIChangeCounter == 100)
                {
                    vec1 = NPC.Center;
                    vec2 = NPC.Center + CEUtils.randomRot().ToRotationVector2() * 800;
                }
                if(AIChangeCounter > 40)
                {
                    float p = Utils.Remap(AIChangeCounter, 100, 40, 0, 1);
                    NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(p, 1));
                    if(AIChangeCounter % 2 == 0)
                    {
                        if(AIChangeCounter % 4 == 0)
                        {
                            Shoot<LuminarisSpikeBlue>(NPC.Center, (player.Center - NPC.Center).normalize() * 1.4f);
                        }
                        else
                        {
                            Shoot<LuminarisSpikeRed>(NPC.Center, (player.Center - NPC.Center).normalize() * 1.4f);
                        }
                    }
                }
            }
            if(ai == AIStyle.Shoot360)
            {
                NPC.velocity *= 0;
                NPC.rotation = 0;
                if(AIChangeCounter == 160)
                {
                    vec1 = NPC.Center;
                    vec2 = player.Center + (NPC.Center - player.Center).normalize() * 280 / enrange;
                }
                if(AIChangeCounter > 120)
                {
                    float p = Utils.Remap(AIChangeCounter, 160, 120, 0, 1);
                    NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(p, 1));
                }
                if (AIChangeCounter <= 110 && AIChangeCounter >= 80)
                {
                    if (AIChangeCounter % 10 == 0)
                    {
                        Color impactColor = Color.White;
                        float impactParticleScale = 6f;
                        CEUtils.PlaySound("portal_emerge", 1, NPC.Center);
                        SparkleParticle impactParticle2 = new SparkleParticle(NPC.Center, Vector2.Zero, Color.White, Color.SkyBlue, impactParticleScale * 1.2f, 12, 0, 4.5f);
                        GeneralParticleHandler.SpawnParticle(impactParticle2);
                        SparkleParticle impactParticle = new SparkleParticle(NPC.Center, Vector2.Zero, impactColor, Color.SkyBlue, impactParticleScale, 10, 0, 3f);
                        GeneralParticleHandler.SpawnParticle(impactParticle);
                        float ag = CEUtils.randomRot();
                        for(int i = 0; i < 360; i += 60)
                        {
                            float a = ag + MathHelper.ToRadians(i);
                            Shoot<LuminarisAstralShoot>(NPC.Center, a.ToRotationVector2() * 6 * enrange, 1, a + MathHelper.PiOver2, 0.26f * enrange);
                            Shoot<LuminarisAstralShoot>(NPC.Center, a.ToRotationVector2() * 6 * enrange, 1, a - MathHelper.PiOver2, 0.26f * enrange);
                            
                        }
                    }
                }
            }
            if (ai == AIStyle.RoundAndDash)
            {
                if (AIChangeCounter > 1)
                {
                    NPC.velocity *= 0;
                    NPC.rotation = 0;
                    AfterImageTime = 16;
                    if (AIChangeCounter == 200)
                    {
                        vec1 = NPC.Center;
                        vec2 = player.Center + (NPC.Center - player.Center).SafeNormalize(Vector2.Zero) * 700;
                    }
                    if (AIChangeCounter > 140 && AIChangeCounter <= 200)
                    {
                        NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 140) / 60f, 1));
                    }
                    else
                    {
                        if (AIChangeCounter == 140)
                        {
                            num1 = 700;
                            num2 = (NPC.Center - player.Center).ToRotation();
                            num3 = num2 + Main.rand.NextFloat(6, 10) * (Main.rand.NextBool() ? 1 : -1);
                        }

                        if (AIChangeCounter < 50 && AIChangeCounter > 16)
                        {
                            MegaTrail = 1.2f;
                        }
                        if (AIChangeCounter == 50)
                        {
                            CalamityEntropy.FlashEffectStrength = 0.3f;
                            CEUtils.PlaySound("flamethrower end", 1, NPC.Center);
                            vec2 = player.Center;
                            odp.Clear();
                            odp.Add(NPC.Center);
                        }
                        if (AIChangeCounter > 50)
                        {
                            float p = Utils.Remap(AIChangeCounter, 140, 50, 0, 1);
                            float r = float.Lerp(num2, num3, CEUtils.GetRepeatedCosFromZeroToOne(p, 1));
                            NPC.Center = player.Center + r.ToRotationVector2() * num1;
                        }
                        if (AIChangeCounter < 50)
                        {
                            float r = num3;
                            float p = Utils.Remap(AIChangeCounter, 49, 0, 0, 1);
                            num1 = float.Lerp(700, -700, CEUtils.GetRepeatedCosFromZeroToOne(p, 1));
                            NPC.Center = vec2 + r.ToRotationVector2() * num1;
                        }
                        NPC.rotation = (NPC.Center - oldPos).ToRotation() + MathHelper.PiOver2;
                    }
                }
            }
            if(ai == AIStyle.SmashDown)
            {
                int ac = AIChangeCounter % 100 + 1;
                if(ac == 100)
                {
                    vec1 = NPC.Center;
                    vec2 = player.Center + player.velocity * 36 + new Vector2(0, -520 + Main.rand.NextFloat(-80, 80));
                }
                if(ac >= 40)
                {
                    NPC.velocity *= 0;
                    NPC.rotation = 0;
                    NPC.Center = Vector2.Lerp(vec1, vec2, CEUtils.GetRepeatedCosFromZeroToOne(Utils.Remap(ac, 100, 40, 0, 1), 1));
                }
                else
                {
                    NPC.velocity.Y += 2f;
                    if(ac == 39)
                    {
                        CalamityEntropy.FlashEffectStrength = 0.4f;
                        CEUtils.PlaySound("flamethrower end", 1, NPC.Center);
                        odp.Clear();
                        odp.Add(NPC.Center);
                    }
                    if(ac < 34)
                    {
                        if(ac % (int)(7 / enrange) == 0)
                        {
                            Shoot<LuminarisAstralShoot>(NPC.Center, new Vector2(14, 0) * enrange, 1, (-Vector2.UnitY).ToRotation(), enrange * 0.32f, 16 / enrange);
                            Shoot<LuminarisAstralShoot>(NPC.Center, new Vector2(-14, 0) * enrange, 1, (-Vector2.UnitY).ToRotation(), enrange * 0.32f, 16 / enrange);
                        }
                    }
                    if(ac > 10)
                    {
                        MegaTrail = 1;
                    }
                }
            }
            if(ai == AIStyle.ShootTriangle)
            {
                NPC.velocity *= 0;
                NPC.rotation = 0;
                AfterImageTime = 16;
                if (AIChangeCounter == 200)
                {
                    num3 = Main.rand.NextBool() ? -1 : 1;
                    vec1 = NPC.Center;
                }
                if (AIChangeCounter > 160)
                {
                    NPC.Center = Vector2.Lerp(vec1, player.Center + new Vector2(440 * Math.Sign(NPC.Center.X - player.Center.X), -440), CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 160) / 40f, 1));
                }
                if (AIChangeCounter == 160)
                {
                    num1 = NPC.Center.Distance(player.Center);
                    num2 = (NPC.Center - player.Center).ToRotation();
                }
                if (AIChangeCounter < 160)
                {
                    if (AIChangeCounter >= 148)
                    {
                        num2 += MathHelper.ToRadians(30);
                        NPC.Center = player.Center + num2.ToRotationVector2() * num1;
                        if(AIChangeCounter % 2 == 0)
                        {
                            Shoot<LuminarisTriangleShootBlue>(NPC.Center, (player.Center - NPC.Center).normalize().RotatedBy(MathHelper.ToRadians(20) * (Main.rand.NextBool() ? 1 : -1)) * 10 * Main.rand.NextFloat(0.8f, 1.2f) * enrange);
                        }
                        else
                        {
                            Shoot<LuminarisTriangleShootRed>(NPC.Center, (player.Center - NPC.Center).normalize().RotatedBy(MathHelper.ToRadians(20) * (Main.rand.NextBool() ? 1 : -1)) * 10 * Main.rand.NextFloat(0.8f, 1.2f) * enrange);
                        }
                        NPC.rotation = (NPC.Center - oldPos).ToRotation() + MathHelper.PiOver2;
                        vec1 = player.Center;
                    }
                    else
                    {
                        NPC.rotation = 0;
                        NPC.velocity *= 0.9f;
                        num2 += MathHelper.ToRadians(10);
                        NPC.Center = vec1 + num2.ToRotationVector2() * num1;
                    }
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if(ai == AIStyle.RoundShooting)
            {
                return false;
            }
            if(ai == AIStyle.SmashDown && MegaTrail <= 0)
            {
                return false;
            }
            return true;
        }
        public void Shoot<T>(Vector2 pos, Vector2 velocity, float damageMult = 1, float ai0 = 0, float ai1 = 0, float ai2 = 0) where T : ModProjectile
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int baseDamage = (int)(NPC.damage / 6.2f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<T>(), (int)(baseDamage * damageMult), 4, -1, ai0, ai1, ai2);
            }
        }
        public int AIChangeCounter = 0;
        public float MegaTrail = 0;
        public void SetAISyyle()
        {
            if (phase == 1)
            {
                ai = (AIStyle)AIRound;
                if (AIRound >= 6)
                {
                    AIRound = -1;
                }
            }
            else
            {
                if(AIRound == 0)
                {
                    ai = AIStyle.Shoot360;
                }
                if(AIRound == 1)
                {
                    ai = AIStyle.SmashDown;
                }
                if (AIRound == 2)
                {
                    ai = AIStyle.RoundAndDash;
                }
                if (AIRound == 3)
                {
                    ai = AIStyle.Subduction;
                }
                if (AIRound == 4)
                {
                    ai = AIStyle.ShootTriangle;
                }
                if (AIRound == 5)
                {
                    ai = AIStyle.AstralSpike;
                }
                if (AIRound == 6)
                {
                    ai = AIStyle.Dashing;
                }
                if (AIRound >= 6)
                {
                    AIRound = -1;
                }
            }
        }

        public static Texture2D texture = null;
        public static Texture2D texTail1 = null;
        public static Texture2D texTail2 = null;
        public static Texture2D texStar = null;

        public Rope tail1 = null;
        public Rope tail2 = null;
        public override void Unload()
        {
            texture = null;
            texTail1 = null;
            texTail2 = null;
            texStar = null;
        }
        public int phase = 1;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if(texStar == null)
            {
                texStar = CEUtils.getExtraTex("StarTexture_White");
            }
            if(texture == null)
                texture = NPC.getTexture();
            if(texTail1 == null || texTail2 == null)
            {
                texTail1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LuminarisMoth/t1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                texTail2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LuminarisMoth/t2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }

            List<Vector2> afterImagePoints = new List<Vector2>();
            if (AfterImageTime > 0 && odp.Count > 8)
            {
                for (int i = odp.Count - 1; i > odp.Count / 2; i--)
                {
                    for (float j = 0; j < 1; j += 0.2f)
                    {
                        afterImagePoints.Add(Vector2.Lerp(odp[i], odp[i - 1], j));
                    }
                }
                for (int i = 0; i < afterImagePoints.Count; i++)
                {
                    DrawMyself(afterImagePoints[i], Color.White * (1 - ((i + 1f) / afterImagePoints.Count)) * 0.16f * (AfterImageTime / 16f), true);
                }
            }

            DrawMyself(NPC.Center, Color.White);

            return false;
        }
        public void DrawMyself(Vector2 pos, Color color, bool afterImage = false)
        {
            DrawTails(pos - NPC.Center, color);
            if (!afterImage)
            {
                Asset<Texture2D> textured = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Enchanted", AssetRequestMode.ImmediateLoad);
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Transform3", AssetRequestMode.ImmediateLoad).Value;
                shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
                shader.Parameters["color"].SetValue((phase == 1 ? new Color(0, 190, 250, 255) : new Color(160, 80, 255, 255)).ToVector4());
                shader.Parameters["strength"].SetValue(phase == 1 ? 0.2f : 1f);

                shader.CurrentTechnique.Passes["EnchantedPass"].Apply();
                Main.instance.GraphicsDevice.Textures[1] = textured.Value;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(0, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0], Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, shader, Main.Transform);
            }
            Rectangle frame = new Rectangle(0, (texture.Height / Main.npcFrameCount[Type]) * ((frameCounter / 4) % Main.npcFrameCount[Type]), texture.Width, (texture.Height / Main.npcFrameCount[Type]) - 2);
            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color * NPC.Opacity, NPC.rotation, new Vector2(texture.Width / 2, 104), NPC.scale, SpriteEffects.None);

            if (!afterImage)
            {
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                float starX = 1f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 26) * 0.4f;
                Vector2 starScale = new Vector2(starX, starX) * (1 + MegaTrail * 1.6f);
                Main.spriteBatch.Draw(texStar, pos - Main.screenPosition, null, Color.LightBlue * (color.A / 255f) * NPC.Opacity, 0, texStar.Size() * 0.5f, new Vector2(1f, 0.8f * 0.7f) * starScale * NPC.scale * 0.74f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texStar, pos - Main.screenPosition, null, Color.LightBlue * (color.A / 255f) * NPC.Opacity, 0, texStar.Size() * 0.5f, new Vector2(0.8f, 1f * 0.7f) * starScale * NPC.scale * 0.74f, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();
                drawT();
            }
            
        }
        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            odp.Add(NPC.Center);
            if (odp.Count > 2)
            {
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Color.SkyBlue * NPC.Opacity;

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 20 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 20 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MegaStreakBacking2").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Color.White * NPC.Opacity;

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 16 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 16 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                if(MegaTrail > 0)
                {
                    {
                        List<ColoredVertex> ve = new List<ColoredVertex>();
                        Color b = Color.SkyBlue * NPC.Opacity * MegaTrail;
                        var ptd = CEUtils.WrapPoints(odp, 5);
                        float a = 0;
                        float lr = 0;
                        for (int i = 1; i < ptd.Count; i++)
                        {
                            a += 1f / (float)ptd.Count;
                            ve.Add(new ColoredVertex(ptd[i] - Main.screenPosition + (ptd[i] - ptd[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 70 * ((i - 1f) / (ptd.Count - 2f)),
                                  new Vector3((float)(i + 1) / ptd.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                                b * a));
                            ve.Add(new ColoredVertex(ptd[i] - Main.screenPosition + (ptd[i] - ptd[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 70 * ((i - 1f) / (ptd.Count - 2f)),
                                  new Vector3((float)(i + 1) / ptd.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                                  b * a));
                            lr = (ptd[i] - ptd[i - 1]).ToRotation();
                        }
                        a = 1;

                        if (ve.Count >= 3)
                        {
                            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MegaStreakInner").Value;
                            gd.Textures[0] = tx;
                            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                        }
                    }
                    {
                        List<ColoredVertex> ve = new List<ColoredVertex>();
                        Color b = Color.White * NPC.Opacity * MegaTrail;
                        var ptd = CEUtils.WrapPoints(odp, 5);
                        float a = 0;
                        float lr = 0;
                        for (int i = 1; i < ptd.Count; i++)
                        {
                            a += 1f / (float)ptd.Count;
                            ve.Add(new ColoredVertex(ptd[i] - Main.screenPosition + (ptd[i] - ptd[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 64 * ((i - 1f) / (ptd.Count - 2f)),
                                  new Vector3((float)(i + 1) / ptd.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                                b * a));
                            ve.Add(new ColoredVertex(ptd[i] - Main.screenPosition + (ptd[i] - ptd[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 64 * ((i - 1f) / (ptd.Count - 2f)),
                                  new Vector3((float)(i + 1) / ptd.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                                  b * a));
                            lr = (ptd[i] - ptd[i - 1]).ToRotation();
                        }
                        a = 1;

                        if (ve.Count >= 3)
                        {
                            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak2").Value;
                            gd.Textures[0] = tx;
                            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                        }
                    }
                }
            }
            odp.RemoveAt(odp.Count - 1);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        #region drawTail
        public void DrawTails(Vector2 pos, Color color)
        {
            GraphicsDevice gd = Main.spriteBatch.GraphicsDevice;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (tail1 != null)
            {   
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = color * NPC.Opacity;
                List<Vector2> tailPoints = tail1.GetPoints();
                for (int i = 1; i < tailPoints.Count; i++)
                {
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 0, 1),
                          b));

                }
                if (ve.Count >= 3)
                {
                    Texture2D tx = texTail1;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            if (tail2 != null)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = color * NPC.Opacity;
                List<Vector2> tailPoints = tail2.GetPoints();
                for (int i = 1; i < tailPoints.Count; i++)
                {
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 0, 1),
                          b));

                }
                if (ve.Count >= 3)
                {
                    Texture2D tx = texTail2;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.ExitShaderRegion();
        }
        #endregion

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref EDownedBosses.downedLuminaris, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<LuminarisBag>()));

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.GreaterHealingPotion, 1, 5, 15), hideLootReport: true);


            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                normalOnly.Add(ModContent.ItemType<StarlitPiercer>(), new Fraction(4, 5));
                normalOnly.Add(ModContent.ItemType<Luminar>(), new Fraction(4, 5));
            }
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<LuminarisRelic>());

            npcLoot.Add(ModContent.ItemType<LuminarisTrophy>(), 10);

            //npcLoot.AddConditionalPerPlayer(() => !EDownedBosses.downedProphet, ModContent.ItemType<ProphetLore>());
        }
    }
}
