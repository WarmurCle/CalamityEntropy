using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class PrefEntropyAI
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool small = false;
        private bool medium = false;
        private bool large = false;
        private int wormsAlive = 0;
        private List<Vector2> playerOldPos = new List<Vector2>();
        public Vector2 getTargetPos()
        {
            return playerOldPos[0];
        }
        public void PerfAI(PerforatorHive mNPC)
        {
            NPC NPC = mNPC.NPC;
            CalamityGlobalNPC.perfHive = NPC.whoAmI;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;
            // Variables for ichor blob phase
            float blobPhaseGateValue = bossRush ? 450f : 600f;
            bool floatAboveToFireBlobs = NPC.ai[2] >= blobPhaseGateValue - 120f;

            Player player = Main.player[NPC.target];
            playerOldPos.Add(player.Center);
            if (playerOldPos.Count > 32)
            {
                playerOldPos.RemoveAt(0);
            }
            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.7f;

            // Enrage
            if ((!player.ZoneCrimson || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!player.ZoneCrimson || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            if (!player.active || player.dead || Vector2.Distance(getTargetPos(), NPC.Center) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(getTargetPos(), NPC.Center) > 5600f)
                {
                    NPC.rotation = NPC.velocity.X * 0.04f;

                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y > 12f)
                        NPC.velocity.Y = 12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            //GFB seed shenanigans: Behavior during the suck
            if (NPC.localAI[1] >= 6f)
            {
                //Leak projectiles everywhere and start healing
                int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                int damage = NPC.GetProjectileDamage(type);
                int spread = Main.rand.Next(-45, 46);
                Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread)), type, damage, 0f, Main.myPlayer, 0f, getTargetPos().Y);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread)), type, damage, 0f, Main.myPlayer, 0f, getTargetPos().Y);

                //Heals 10 times per second for 0.1% of its health each = 1% per second
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healAmt = (int)(NPC.lifeMax / 1000);
                    if (healAmt > NPC.lifeMax - NPC.life)
                        healAmt = NPC.lifeMax - NPC.life;

                    if (healAmt > 0)
                    {
                        NPC.life += healAmt;
                        NPC.HealEffect(healAmt, true);
                        NPC.netUpdate = true;
                    }
                }
                NPC.localAI[1] = 0f;
            }

            bool largeWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>());
            bool mediumWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>());
            bool smallWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadSmall>());
            wormsAlive = 0;

            NPC.Calamity().DR = wormsAlive * 0.3f;

            if (NPC.ai[3] == 0f && NPC.life > 0)
                NPC.ai[3] = NPC.lifeMax;

            bool canSpawnWorms = !small || !medium || !large;
            if (NPC.life > 0 && canSpawnWorms)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int wormSpawnGateValue = (int)(NPC.lifeMax * (Main.getGoodWorld ? 0.15 : 0.25));
                    if ((NPC.life + wormSpawnGateValue) < NPC.ai[3])
                    {
                        NPC.ai[3] = NPC.life;
                        int wormType = ModContent.NPCType<PerforatorHeadSmall>();
                        small = true;
                        medium = true;

                        large = true;

                        int npc_;
                        npc_ = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadLarge>(), 1);
                        npc_.ToNPC().lifeMax *= 4;
                        npc_.ToNPC().life *= 4;

                        npc_ = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadMedium>(), 1);
                        npc_.ToNPC().lifeMax *= 4;
                        npc_.ToNPC().life *= 4;

                        npc_ = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadSmall>(), 1);
                        npc_.ToNPC().lifeMax *= 4;
                        npc_.ToNPC().life *= 4;

                        NPC.TargetClosest();

                        SoundEngine.PlaySound(PerforatorHive.WormSpawn, NPC.Center);

                        for (int i = 0; i < 16; i++)
                        {
                            int ichorDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, 0f, 0f, 100, default, 1f);
                            Main.dust[ichorDust].velocity *= 2f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[ichorDust].scale = 0.25f;
                                Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int j = 0; j < 32; j++)
                        {
                            int bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
                            Main.dust[bloodDust].noGravity = true;
                            Main.dust[bloodDust].velocity *= 3f;
                            bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1f);
                            Main.dust[bloodDust].velocity *= 2f;
                        }
                    }
                }
            }

            if (Math.Abs(NPC.Center.X - getTargetPos().X) > 10f)
            {
                float playerLocation = NPC.Center.X - getTargetPos().X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            NPC.rotation = NPC.velocity.X * 0.04f;

            // Emit ichor blobs
            if (phase2)
            {
                if (wormsAlive == 0 || bossRush || floatAboveToFireBlobs || (CalamityWorld.LegendaryMode && CalamityWorld.revenge))
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= blobPhaseGateValue)
                    {
                        if (NPC.ai[2] < blobPhaseGateValue + 300f)
                        {
                            if (NPC.velocity.Length() > 0.5f)
                                NPC.velocity *= bossRush ? 0.98f : 0.985f;
                            else
                                NPC.ai[2] = blobPhaseGateValue + 300f;
                        }
                        else
                        {
                            NPC.ai[2] = 0f;

                            SoundEngine.PlaySound(PerforatorHive.IchorShoot, NPC.Center);

                            for (int i = 0; i < 32; i++)
                            {
                                int ichorDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, 0f, 0f, 100, default, 1f);
                                float dustVelocityYAdd = Math.Abs(Main.dust[ichorDust].velocity.Y) * 0.5f;
                                if (Main.dust[ichorDust].velocity.Y < 0f)
                                    Main.dust[ichorDust].velocity.Y = 2f + dustVelocityYAdd;
                                if (Main.rand.NextBool())
                                {
                                    Main.dust[ichorDust].scale = 0.25f;
                                    Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                }
                            }

                            int numBlobs = expertMode ? 12 : 8;
                            if (Main.getGoodWorld)
                                numBlobs *= 2;

                            int type = ModContent.ProjectileType<IchorBlob>();
                            int damage = NPC.GetProjectileDamage(type);

                            for (int i = 0; i < numBlobs; i++)
                            {
                                Vector2 blobVelocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                blobVelocity.Normalize();
                                blobVelocity *= Main.rand.Next(400, 801) * (bossRush ? 0.02f : 0.01f);

                                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                                    blobVelocity *= Main.rand.NextFloat() + 1f;

                                float blobVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.5f;
                                if (blobVelocity.Y < 2f)
                                    blobVelocity.Y = 2f + blobVelocityYAdd;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 50f, blobVelocity, type, damage, 0f, Main.myPlayer, 0f, getTargetPos().Y);
                            }
                        }

                        return;
                    }
                }
            }

            // Movement velocities, increased while enraged
            float velocityEnrageIncrease = enrageScale;

            // When firing blobs, float above the target and don't call any other projectile firing or movement code
            if (floatAboveToFireBlobs)
            {
                if (revenge)
                    Movement(NPC, getTargetPos(), 6f + velocityEnrageIncrease, 0.3f, 450f);
                else
                    Movement(NPC, getTargetPos(), 5f + velocityEnrageIncrease, 0.2f, 450f);

                return;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= (revenge ? 200f : 250f) + wormsAlive * 150f && NPC.position.Y + NPC.height < player.position.Y && Vector2.Distance(getTargetPos(), NPC.Center) > 80f)
                {
                    NPC.localAI[0] = 0f;
                    SoundEngine.PlaySound(PerforatorHive.GeyserShoot, NPC.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        int ichorDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, 0f, 0f, 100, default, 1f);
                        Main.dust[ichorDust].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[ichorDust].scale = 0.25f;
                            Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    for (int j = 0; j < 16; j++)
                    {
                        int bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
                        Main.dust[bloodDust].noGravity = true;
                        Main.dust[bloodDust].velocity *= 5f;
                        bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1f);
                        Main.dust[bloodDust].velocity *= 2f;
                    }

                    int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                    int damage = NPC.GetProjectileDamage(type);
                    int numProj = death ? 14 : revenge ? 12 : expertMode ? 10 : 8;
                    if (Main.getGoodWorld)
                        numProj *= 2;
                    numProj *= 2;
                    int spread = 80;
                    float velocity = 10f;
                    Vector2 destination = Main.rand.NextBool() ? getTargetPos() : NPC.Center - Vector2.UnitY * 100f;
                    Vector2 projectileVelocity = new Vector2(Vector2.Normalize(destination - NPC.Center).X * velocity, -velocity);
                    float rotation = MathHelper.ToRadians(spread);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Vector2 randomVelocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() - 0.5f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 50f, perturbedSpeed + randomVelocity, type, damage, 0f, Main.myPlayer, 0f, getTargetPos().Y);
                    }
                }
            }

            if (revenge)
            {
                NPC.damage = NPC.defDamage;
                Movement(NPC, getTargetPos(), 16f + velocityEnrageIncrease, death ? 0.1125f : 0.0975f, 20f);
            }
            else
            {
                Movement(NPC, getTargetPos(), 6f + velocityEnrageIncrease, 0.075f, 350f);
            }
        }

        private void Movement(NPC NPC, Vector2 targetPos, float velocity, float acceleration, float y)
        {
            // Distance from destination where Perf Hive stops moving
            float movementDistanceGateValue = 100f;

            // This is where Perf Hive should be
            Vector2 destination = new Vector2(targetPos.X, targetPos.Y - y);

            // How far Perf Hive is from where it's supposed to be
            Vector2 distanceFromDestination = destination - NPC.Center;

            // Set the velocity
            CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, velocity, acceleration, true);
        }
    }
}
