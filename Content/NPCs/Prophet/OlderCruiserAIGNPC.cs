using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityMod;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Prophet
{
    public class OlderCruiserAIGNPC
    {
        public void SendExtraAI(NPC npc, BinaryWriter writer)
        {
            writer.Write(this.speedMuti);
            writer.Write(this.targetSpeed);
            writer.Write(this.slowDownTime);
            writer.Write(this.nrc);
            writer.Write(this.aitype);
            writer.Write(this.changeCounter);
            writer.Write(this.maxDistanceTarget);
            writer.Write(this.rotDist);
            Utils.WriteVector2(writer, this.rotPos);
            writer.Write(this.circleDir);
            writer.Write(this.flag);
        }

        public void ReceiveExtraAI(NPC npc, BinaryReader reader)
        {
            this.speedMuti = reader.ReadSingle();
            this.targetSpeed = reader.ReadSingle();
            this.slowDownTime = reader.ReadInt32();
            this.nrc = reader.ReadInt32();
            this.aitype = reader.ReadSingle();
            this.changeCounter = reader.ReadInt32();
            this.maxDistanceTarget = reader.ReadInt32();
            this.rotDist = reader.ReadInt32();
            this.rotPos = Utils.ReadVector2(reader);
            this.circleDir = reader.ReadInt32();
            this.flag = reader.ReadBoolean();
        }

        public void changeAi()
        {
            if (this.aitype == 2f)
            {
                this.rotDist = 900;
            }
            this.circleDir = Main.rand.Next(0, 2);
            if (this.circleDir == 0)
            {
                this.circleDir = -1;
            }
            this.flag = false;
        }



        public bool PreAI(NPC NPC)
        {
            noaitime--;
            if (this.phase == 1 && NPC.life < NPC.lifeMax / 2)
            {
                NPC.dontTakeDamage = true;
                if (NPC.life < NPC.lifeMax / 2)
                {
                    NPC.life = NPC.lifeMax / 2 - 1;
                }
            }
            else
            {
                NPC.dontTakeDamage = false;
            }
            if (this.phase == 2)
            {
                this.phaseTrans++;
                if (this.phaseTrans < 120)
                {
                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        NPC.life = NPC.lifeMax / 2 - 1;
                    }
                    Particle p2 = new Particle();
                    p2.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                    p2.alpha = 0.7f;
                    new Random();
                    p2.velocity = Utils.ToRotationVector2(NPC.rotation) * -3f;
                    VoidParticles.particles.Add(p2);
                    p2 = new Particle();
                    p2.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                    p2.alpha = 0.7f;
                    p2.velocity = Utils.RotatedBy(Utils.ToRotationVector2(NPC.rotation), (double)MathHelper.ToRadians(70f), default(Vector2)) * -3f;
                    VoidParticles.particles.Add(p2);
                    p2 = new Particle();
                    p2.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                    p2.alpha = 0.7f;
                    p2.velocity = Utils.RotatedBy(Utils.ToRotationVector2(NPC.rotation), (double)MathHelper.ToRadians(-70f), default(Vector2)) * -3f;
                    VoidParticles.particles.Add(p2);
                }
            }
            else if (this.aitype == 1f || this.aitype == 2f)
            {
                Particle p3 = new Particle();
                p3.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                p3.alpha = 0.7f;
                new Random();
                p3.velocity = Utils.ToRotationVector2(NPC.rotation) * -3f;
                VoidParticles.particles.Add(p3);
                p3 = new Particle();
                p3.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                p3.alpha = 0.7f;
                p3.velocity = Utils.RotatedBy(Utils.ToRotationVector2(NPC.rotation), (double)MathHelper.ToRadians(70f), default(Vector2)) * -3f;
                VoidParticles.particles.Add(p3);
                p3 = new Particle();
                p3.position = NPC.Center - Utils.ToRotationVector2(NPC.rotation) * -14f;
                p3.alpha = 0.7f;
                p3.velocity = Utils.RotatedBy(Utils.ToRotationVector2(NPC.rotation), (double)MathHelper.ToRadians(-70f), default(Vector2)) * -3f;
                VoidParticles.particles.Add(p3);
            }
            if (this.phase == 1 && this.aitype != 2f && NPC.life < NPC.lifeMax / 2)
            {
                this.phase = 2;
                if (Main.netMode != 1)
                {
                    int ag = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Utils.ToRotationVector2(MathHelper.ToRadians((float)ag)) * 26f, ModContent.ProjectileType<RuneBulletHostile>(), NPC.damage / 7, 0f, -1, 0f, 0f, 0f);
                        ag += 36;
                    }
                }
                NPC.defense = 0;
                NPC.width = 156;
                NPC.height = 156;
                foreach (NPC j in Main.npc)
                {
                    if (j.realLife == NPC.whoAmI)
                    {
                        j.defense = 4;
                        CalamityUtils.Calamity(j).DR = 0.1f;
                        if (j.ai[2] <= 8f && j.ai[2] > 4f)
                        {
                            j.width = 26;
                            j.height = 26;
                        }
                        if (j.ai[2] > 8f)
                        {
                            j.active = false;
                        }
                        if (Main.dedServ)
                        {
                            NetMessage.SendData(23, -1, -1, null, j.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            CalamityUtils.Calamity(Main.LocalPlayer).monolithExoShader = 30;
            if (NPC.ai[0] > 10f)
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 mp = NPC.Center;
                if (this.aitype == 4f)
                {
                    mp = this.rotPos;
                }
            }
            this.jaslowdown *= 0.8f;
            NPC.netUpdate = true;
            if (this.slowDownTime > 0)
            {
                this.speed += (this.targetSpeed * 0.15f - this.speed) * 0.07f;
                this.slowDownTime--;
            }
            else
            {
                this.speed += (this.targetSpeed - this.speed) * 0.07f;
            }
            if (NPC.ai[0] > 180f)
            {
                this.maxDistance = (int)((float)this.maxDistance + (float)(this.maxDistanceTarget - this.maxDistance) * 0.01f);
            }
            else
            {
                this.maxDistance = (int)((float)this.maxDistance + (float)(this.maxDistanceTarget - this.maxDistance) * 0.002f);
            }
            NPC.ai[0] += 1f;
            for (int k = 0; k < NPC.buffTime.Length; k++)
            {
                NPC.buffTime[k] = 0;
            }
            int closestPlayerIndexd = NPC.FindClosestPlayer();
            NPC.target = closestPlayerIndexd;
            int targetPlayerIndex = NPC.target;
            Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.5f, 0.5f, 1f);
            if (NPC.HasValidTarget)
            {
                if (CEUtils.getDistance(NPC.Center, NPC.target.ToPlayer().Center) < 210f && this.mouthRot >= -0.1f)
                {
                    this.bite = true;
                }
                Player targetPlayerr = Main.player[targetPlayerIndex];
                if (this.aitype == 0f)
                {
                    this.maxDistanceTarget = 1850;
                    if (NPC.ai[1] == 1f)
                    {
                        if (this.phase == 1)
                        {
                            NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.028f, false);
                            NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 1.25f.ToRadians(), true);
                        }
                        else
                        {
                            NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.025f, false);
                            NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.7f.ToRadians(), true);
                        }
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                        this.targetSpeed = 18f;
                        this.nrc++;
                        if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 200f || (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 380f && this.phase == 2) || this.nrc > 160)
                        {
                            NPC.ai[1] = 0f;
                            this.nrc = 0;
                        }
                        if (this.changeCounter > 2)
                        {
                            this.aitype = (float)Main.rand.Next(0, 3);
                            this.changeAi();
                            this.changeCounter = 0;
                            this.nrc = 0;
                        }
                    }
                    if (NPC.ai[1] == 0f)
                    {
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                        this.targetSpeed = 30f;
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.2f.ToRadians(), true);
                        this.nrc++;
                        if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) > 1400f || this.nrc > 100)
                        {
                            NPC.ai[1] = 1f;
                            this.changeCounter++;
                            this.nrc = 0;
                            this.slowDownTime = 60;
                            this.tjv = 1;
                        }
                    }
                }
                else if (this.aitype == 1f)
                {
                    this.maxDistanceTarget = 1400;
                    this.changeCounter++;
                    this.targetSpeed = 70f;
                    Vector2 targetPos = targetPlayerr.Center + Utils.RotatedBy(Utils.SafeNormalize(NPC.Center - targetPlayerr.Center, new Vector2(1f, 0f)) * 800f, (double)MathHelper.ToRadians((float)(20 * this.circleDir)), default(Vector2));
                    NPC.rotation = Utils.ToRotation(targetPos - NPC.Center);
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                    if (this.tjv == 0 && !this.jv && this.jaslowdown < 0.01f)
                    {
                        this.tjv = 1;
                    }
                    if (this.changeCounter > 250)
                    {
                        this.changeCounter = 0;
                        this.aitype = 0f;
                        this.changeAi();
                    }
                }
                else if (this.aitype == 2f)
                {
                    Main.LocalPlayer.wingTime = (float)Main.LocalPlayer.wingTimeMax;
                    if (this.changeCounter > 10)
                    {
                        this.maxDistanceTarget = this.rotDist + 400;
                    }
                    this.changeCounter++;
                    if (this.changeCounter == 1)
                    {
                        this.targetSpeed = 10f;
                        this.rotDist = 1000;
                        this.rotPos = targetPlayerr.Center + new Vector2((float)Main.rand.Next(-800, 801), (float)Main.rand.Next(-800, 801));
                    }
                    if (this.changeCounter == 60)
                    {
                        this.targetSpeed = 50f;
                    }
                    if (this.changeCounter == 200 && Main.netMode != 1)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruiserLaserMouth>(), (int)((float)NPC.damage / 7f), 0f, -1, (float)NPC.whoAmI, 0f, 0f);
                    }
                    if (this.changeCounter > 260)
                    {
                        this.rotDist--;
                    }
                    Vector2 targetPosd = this.rotPos + Utils.RotatedBy(Utils.SafeNormalize(NPC.Center - this.rotPos, new Vector2(1f, 0f)) * (float)this.rotDist, (double)MathHelper.ToRadians((float)(20 * this.circleDir)), default(Vector2));
                    NPC.rotation = Utils.ToRotation(targetPosd - NPC.Center);
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                    if (this.tjv == 0 && !this.jv && this.jaslowdown < 0.007f && this.changeCounter < 800)
                    {
                        this.tjv = 1;
                    }
                    if (this.changeCounter > 950)
                    {
                        this.rotDist += 4;
                        this.maxDistanceTarget = 2000;
                        this.targetSpeed = 20f;
                    }
                    if (this.changeCounter > 1200)
                    {
                        this.changeCounter = 0;
                        this.aitype = 0f;
                        this.changeAi();
                    }
                }
                if (this.aitype == 3f)
                {
                    this.maxDistanceTarget = 5000;
                    if (this.flag)
                    {
                        this.speed = 60f;
                        this.targetSpeed = 60f;
                        this.changeCounter++;
                        if (!CEUtils.isAir(NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 240f, false) && this.changeCounter < 40)
                        {
                            this.changeCounter = 40;
                        }
                        if (this.changeCounter > 150)
                        {
                            this.aitype = -1f;
                            this.changeCounter = 0;
                            this.changeAi();
                        }
                        if (this.changeCounter < 40)
                        {
                            if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 240f)
                            {
                                targetPlayerr.velocity = Vector2.Zero;
                                targetPlayerr.Center = NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 120f;
                                CalamityUtils.Calamity(targetPlayerr).GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), 1800f, 1000f, 0f, 4.5f, true) * 4f;
                                this.mouthRot = -40f;
                                for (int i3 = 0; i3 < 10; i3++)
                                {
                                    Particle p6 = new Particle();
                                    p6.position = NPC.Center;
                                    p6.alpha = 1.4f;
                                    Random r2 = new Random();
                                    p6.velocity = new Vector2((float)((r2.NextDouble() - 0.5) * 16.0), (float)((r2.NextDouble() - 0.5) * 16.0));
                                    VoidParticles.particles.Add(p6);
                                }
                            }
                        }
                        else
                        {
                            if (this.changeCounter == 40 && Main.netMode != 1)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(null), targetPlayerr.Center, Vector2.Zero, ModContent.ProjectileType<CruiserSlash>(), (int)((float)NPC.damage / 6f), 0f, -1, 0f, 0f, 0f);
                            }
                            if (this.changeCounter < 60)
                            {
                                this.speed = -14f;
                                this.targetSpeed = this.speed;
                            }
                            else
                            {
                                this.targetSpeed = 16f;
                                NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center + Utils.SafeNormalize(NPC.Center - targetPlayerr.Center, Vector2.One) * 1500f - NPC.Center), 0.08f, false);
                            }
                        }
                    }
                    else
                    {
                        this.targetSpeed = 60f;
                        NPC.rotation = Utils.ToRotation(targetPlayerr.Center - NPC.Center);
                        if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 100f)
                        {
                            this.flag = true;
                        }
                    }
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                }
                if (this.aitype == -1f)
                {
                    this.maxDistanceTarget = 4000;
                    if (this.changeCounter == 20 && Main.netMode != 1)
                    {
                        int ag2 = new Random().Next(0, 360);
                        for (int i4 = 0; i4 < 18; i4++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Utils.ToRotationVector2(MathHelper.ToRadians((float)ag2)) * 20f, ModContent.ProjectileType<RuneBulletHostile>(), NPC.damage / 7, 0f, -1, 0f, 0f, 0f);
                            ag2 += 20;
                        }
                    }
                    this.changeCounter++;
                    this.targetSpeed = 16f;
                    Vector2 targetPos2 = targetPlayerr.Center + Utils.RotatedBy(Utils.SafeNormalize(NPC.Center - targetPlayerr.Center, new Vector2(1f, 0f)) * 800f, (double)MathHelper.ToRadians((float)(20 * this.circleDir)), default(Vector2));
                    NPC.rotation = Utils.ToRotation(targetPos2 - NPC.Center);
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                    if (this.changeCounter > 40)
                    {
                        this.changeCounter = 0;
                        this.aitype = (float)Main.rand.Next(3, 8);
                        this.changeAi();
                    }
                }
                if (this.aitype == 4f)
                {
                    if (this.changeCounter > 10)
                    {
                        this.maxDistanceTarget = this.rotDist + 400;
                    }
                    this.changeCounter++;
                    this.targetSpeed = 10f;
                    this.rotDist = 1800;
                    this.maxDistanceTarget = 800;
                    if (this.changeCounter > 380)
                    {
                        this.maxDistanceTarget = 8000;
                    }
                    if (this.changeCounter == 1)
                    {
                        this.rotPos = NPC.Center;
                        this.maxDistanceTarget = 1700;
                        this.bite = true;
                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruiserBlackhole>(), (int)((float)NPC.damage / 9f), 0f, -1, 0f, 0f, 0f);
                        }
                    }
                    if (this.changeCounter > 450)
                    {
                        this.changeCounter = 0;
                        this.aitype = -1f;
                        this.changeAi();
                    }
                    if (this.changeCounter > 400)
                    {
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.5f.ToRadians(), true);
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                    }
                    else
                    {
                        Vector2 targetPosd2 = this.rotPos + Utils.RotatedBy(Utils.SafeNormalize(NPC.Center - this.rotPos, new Vector2(1f, 0f)) * (float)this.rotDist, (double)MathHelper.ToRadians((float)(20 * this.circleDir)), default(Vector2));
                        NPC.rotation = Utils.ToRotation(targetPosd2 - NPC.Center);
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed;
                    }
                }
                if (this.aitype == 5f)
                {
                    this.maxDistanceTarget = 1850;
                    if (NPC.ai[1] == 1f)
                    {
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.01f, false);
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.5f.ToRadians(), true);
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                        this.targetSpeed = 18f;
                        this.nrc++;
                        if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 240f || (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) < 380f && this.phase == 2) || this.nrc > 160)
                        {
                            NPC.ai[1] = 0f;
                            this.nrc = 0;
                        }
                        if (this.changeCounter > 0)
                        {
                            this.nrc = 0;
                            this.aitype = -1f;
                            this.changeAi();
                            this.changeCounter = 0;
                        }
                    }
                    if (NPC.ai[1] == 0f)
                    {
                        NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                        this.targetSpeed = 30f;
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 1.7f.ToRadians(), true);
                        this.nrc++;
                        if (CEUtils.getDistance(NPC.Center, targetPlayerr.Center) > 1400f || this.nrc > 100)
                        {
                            NPC.ai[1] = 1f;
                            this.changeCounter++;
                        }
                    }
                }
                if (this.aitype == 6f)
                {
                    this.maxDistanceTarget = 6500;
                    this.changeCounter++;
                    if (this.changeCounter > 200)
                    {
                        this.maxDistanceTarget = 6000;
                        this.targetSpeed = 80f;
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 0.2f.ToRadians(), true);
                    }
                    else if (this.changeCounter > 140)
                    {
                        this.maxDistanceTarget = 6000;
                        this.targetSpeed = 6f;
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 5f.ToRadians(), true);
                    }
                    else if (this.changeCounter > 80)
                    {
                        this.targetSpeed = 100f;
                    }
                    else
                    {
                        this.targetSpeed = 0f;
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center - NPC.Center), 1.7f.ToRadians(), true);
                    }
                    if (this.changeCounter == 100 && Main.netMode != 1)
                    {
                        int ag3 = new Random().Next(0, 360);
                        for (int i5 = 0; i5 < 10; i5++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Utils.ToRotationVector2(MathHelper.ToRadians((float)ag3)) * 26f, ModContent.ProjectileType<RuneBulletHostile>(), NPC.damage / 7, 0f, -1, 0f, 0f, 0f);
                            ag3 += 36;
                        }
                    }
                    if (this.changeCounter > 270)
                    {
                        this.aitype = -1f;
                        this.changeAi();
                        this.changeCounter = 0;
                    }
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                }
                if (this.aitype == 7f)
                {
                    this.targetSpeed = 0f;
                    NPC.velocity = Utils.ToRotationVector2(NPC.rotation) * this.speed * this.speedMuti;
                    this.changeCounter++;
                    if ((this.changeCounter == 80 || this.changeCounter == 220) && Main.netMode != 1)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, (targetPlayerr.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 6, ModContent.ProjectileType<CruiserLaser2>(), NPC.damage / 7, 0f, -1, (float)NPC.whoAmI, 0f, 0f);
                    }
                    if (this.changeCounter < 60)
                    {
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center + targetPlayerr.velocity * 15f - NPC.Center), 1.7f.ToRadians(), true);
                    }
                    if (this.changeCounter > 140 && this.changeCounter < 200)
                    {
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, Utils.ToRotation(targetPlayerr.Center + targetPlayerr.velocity * 15f - NPC.Center), 1.7f.ToRadians(), true);
                    }
                    if (this.changeCounter > 260)
                    {
                        this.aitype = -1f;
                        this.changeAi();
                        this.changeCounter = 0;
                    }
                }
                if (this.phase == 2)
                {
                    if (this.phaseTrans < 120)
                    {
                        this.aitype = -1f;
                    }
                    else if (this.aitype < 2f && this.aitype > -1f)
                    {
                        this.aitype = (float)Main.rand.Next(3, 3);
                        this.changeCounter = 0;
                        this.changeAi();
                    }
                }
            }
            else
            {
                this.notargettime++;
                NPC npc2 = NPC;
                npc2.velocity.Y = npc2.velocity.Y + -1f;
                if (this.notargettime > 190)
                {
                    NPC.active = false;
                }
                NPC.rotation = Utils.ToRotation(NPC.velocity);
            }
            if (this.bite)
            {
                this.mouthRot -= 12f;
                if (this.mouthRot < -48f)
                {
                    this.bite = false;
                }
            }
            else
            {
                this.mouthRot *= 0.9f;
            }
            Lighting.AddLight(NPC.Center, 1f, 1f, 1f);
            Lighting.AddLight(NPC.Center + NPC.velocity, 1f, 1f, 1f);
            if (this.tjv == 1)
            {
                this.tjv = 0;
                this.jv = true;
                if (this.da < 0f)
                {
                    this.da = 1f;
                }
                this.tail_vj = 12f;
            }
            if (this.jv)
            {
                this.da += this.tail_vj;
                this.tail_vj -= 1.5f;
                if (this.da < 0f)
                {
                    this.da = 0f;
                    this.tail_vj = 0f;
                    this.jv = false;
                    this.jaslowdown = 1f;
                    int num = 8;
                    int counts = 2;
                    float speed = 24f;
                    if (CalamityWorld.revenge)
                    {
                        num = 10;
                        counts = 3;
                        speed = 25f;
                    }
                    if (CalamityWorld.death)
                    {
                        num = 10;
                        counts = 4;
                        speed = 26f;
                    }
                    if (Main.expertMode)
                    {
                        num++;
                        speed *= 1.1f;
                    }
                    if (Main.masterMode)
                    {
                        num++;
                        speed *= 1.15f;
                    }
                    if (this.aitype == 1f)
                    {
                        num /= 3;
                        speed *= 0.65f;
                    }
                    if (this.aitype == 2f)
                    {
                        num /= 4;
                        speed *= 0.5f;
                        counts--;
                    }
                    speed *= 0.3f;
                    if (Main.netMode != 1)
                    {
                        float angle = 0f;
                        for (int i6 = 0; i6 < counts; i6++)
                        {
                            for (int j2 = 0; j2 < num; j2++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Utils.ToRotationVector2(angle) * speed, ModContent.ProjectileType<RuneTorrent>(), (int)((float)NPC.damage / 7f), 1f, -1, 0f, 0f, 0f);
                                angle += 6.2831855f / (float)num;
                            }
                            angle += 6.2831855f / (float)num / (float)counts;
                            speed *= 0.7f;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), (int)((float)NPC.damage / 7f), 0f, -1, 0f, 0f, 0f);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundStyle sound;
                        sound = new("CalamityEntropy/Assets/Sounds/clap", 0);
                        sound.Pitch = 1.4f;
                        SoundEngine.PlaySound(sound, null, null);
                        SoundEngine.PlaySound(SoundID.Item9, null, null);
                    }
                }
            }
            else
            {
                this.ja = 100f / (NPC.velocity.Length() * 3f) * 5f;
                if (this.ja < 0f)
                {
                    this.ja = 0f;
                }
                this.da += (this.ja - this.da) * 0.1f;
            }
            if (this.aitype == 3f || this.aitype == 4f)
            {
                if ((double)this.alpha > 0.3)
                {
                    this.alpha -= 0.05f;
                }
            }
            else if (this.alpha < 1f)
            {
                this.alpha += 0.05f;
            }
            NPC.netUpdate = true;
            NPC.Center = Utils.ToVector2(NPC.Hitbox.Center);
            this.vtodraw = NPC.Center;

            return false;
        }

        public bool PreDraw(NPC NPC, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }
            if (this.phaseTrans > 120)
            {
                if (this.aitype == 6f)
                {
                    if (this.changeCounter < 80)
                    {
                        CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white", ReLogic.Content.AssetRequestMode.AsyncLoad).Value, NPC.Center, NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 6000f, new Color(166, 111, 255) * ((float)this.changeCounter / 80f) * 0.7f, 8f, 0);
                    }
                    if (this.changeCounter > 140 && this.changeCounter < 200)
                    {
                        CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white", ReLogic.Content.AssetRequestMode.AsyncLoad).Value, NPC.Center, NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 6000f, new Color(166, 111, 255) * ((float)(this.changeCounter - 140) / 60f) * 0.7f, 8f, 0);
                    }
                }
                if (this.aitype == 7f)
                {
                    if (this.changeCounter < 60)
                    {
                        CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white", ReLogic.Content.AssetRequestMode.AsyncLoad).Value, NPC.Center, NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 6000f, new Color(200, 180, 255) * ((float)this.changeCounter / 60f) * 0.7f, 8f, 0);
                    }
                    if (this.changeCounter > 140 && this.changeCounter < 200)
                    {
                        CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white", ReLogic.Content.AssetRequestMode.AsyncLoad).Value, NPC.Center, NPC.Center + Utils.ToRotationVector2(NPC.rotation) * 6000f, new Color(200, 180, 255) * ((float)(this.changeCounter - 140) / 60f) * 0.7f, 8f, 0);
                    }
                }
            }
            return true;
        }
        public float ProgressDraw;

        private int length = 27;

        public float speedMuti = 1f;

        public float speed = 18f;

        public float targetSpeed = 18f;

        public int noaitime = 0;

        public int notargettime;

        public int tjv;

        private float lastHurt;

        private int ydmg;

        public bool bite;

        public float mouthRot;

        public int tail = -1;

        private bool b_added;

        public int nrc;

        private float ja = 50f;

        private float da = 50f;

        private float tail_vj;

        private bool jv;

        public int phaseTrans;

        public bool flag;

        public int slowDownTime;

        public Vector2 vtodraw;

        public float jaslowdown;

        public float aitype;

        public int changeCounter;

        public int maxDistance = 6000;

        public int maxDistanceTarget = 1900;

        public int rotDist = 900;

        public Texture2D disTex = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cruiserSpace", ReLogic.Content.AssetRequestMode.AsyncLoad).Value;

        public Vector2 rotPos = Vector2.Zero;

        public int phase = 1;

        public int circleDir = 1;

        public float alpha = 1f;

        private int tdamage;
    }
}