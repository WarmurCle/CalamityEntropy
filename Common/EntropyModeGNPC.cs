using CalamityEntropy.Content.NPCs;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityEntropy.Common
{
    public class EntropyModeGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        PrefEntropyAI perfAI = null;
        public override bool PreAI(NPC npc)
        {
            if (CalamityEntropy.EntropyMode)
            {
                if (npc.ModNPC != null)
                {
                    if (npc.ModNPC is PerforatorHive pf)
                    {
                        if (perfAI == null)
                            perfAI = new PrefEntropyAI();
                        perfAI.PerfAI(pf);
                        return false;
                    }
                    if (npc.ModNPC is Signus || npc.ModNPC is CeaselessVoid || npc.ModNPC is StormWeaverHead)
                    {
                        if (CEUtils.getDistance(npc.Center, Main.player[Player.FindClosest(npc.Center, 999999, 999999)].Center) > 6400)
                        {
                            Player plr = Main.player[Player.FindClosest(npc.Center, 999999, 999999)];
                            npc.Center = plr.Center - CEUtils.normalize((plr.Center - npc.Center)) * 900;
                        }
                    }
                    if (npc.ModNPC is StormWeaverHead sw && NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var tailF = typeof(StormWeaverHead).GetField("tail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            var tail = (bool)tailF.GetValue(sw);
                            if (!tail && npc.ai[0] == 0f)
                            {
                                int Previous = npc.whoAmI;
                                int totalLength = (CalamityWorld.death ? 60 : CalamityWorld.revenge ? 50 : Main.expertMode ? 40 : 30) / 3;
                                int npcCounts = 0;
                                if (Main.zenithWorld) // use up every remaining npc but 20 for safety in the zenith seed
                                {
                                    for (int i = 0; i < Main.maxNPCs; i++)
                                    {
                                        if (!Main.npc[i].active)
                                            npcCounts++;
                                    }

                                    totalLength = npcCounts - 20;
                                }

                                for (int segments = 0; segments < totalLength; segments++)
                                {
                                    int lol;
                                    if (segments >= 0 && segments < totalLength - 1)
                                        lol = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverBody>(), npc.whoAmI);
                                    else
                                        lol = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverTail>(), npc.whoAmI);

                                    Main.npc[lol].realLife = npc.whoAmI;
                                    Main.npc[lol].ai[2] = npc.whoAmI;
                                    Main.npc[lol].ai[1] = Previous;
                                    Main.npc[Previous].ai[0] = lol;
                                    npc.netUpdate = true;
                                    Previous = lol;
                                }

                                tailF.SetValue(sw, true);
                            }
                        }
                    }
                }
            }
            return true;
        }
        public static List<int> SlimeGodSlimes = new List<int>
        {
            NPCType<CrimulanPaladin>(),
            NPCType<EbonianPaladin>(),
            NPCType<SplitCrimulanPaladin>(),
            NPCType<SplitEbonianPaladin>()
        };
        public override bool CheckActive(NPC npc)
        {
            if (CalamityEntropy.EntropyMode)
            {
                bool hasPlr = false;
                foreach (var plr in Main.ActivePlayers)
                {
                    if (!plr.dead)
                    {
                        hasPlr = true;
                        break;
                    }
                }
                if (npc.ModNPC != null && hasPlr)
                {
                    if (npc.ModNPC is Signus || npc.ModNPC is CeaselessVoid || npc.ModNPC is StormWeaverHead || npc.ModNPC is StormWeaverTail || npc.ModNPC is StormWeaverBody)
                    {
                        return false;
                    }
                }
            }
            return !SlimeGodSlimes.Contains(npc.type);
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (CalamityEntropy.EntropyMode)
            {
                if (npc.ModNPC != null)
                {
                    if (npc.ModNPC is DevourerofGodsTail || npc.ModNPC is DevourerofGodsHead || npc.ModNPC is DevourerofGodsBody)
                    {
                        if (NPC.AnyNPCs(ModContent.NPCType<Signus>()) || NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) || NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()))
                        {
                            modifiers.FinalDamage *= 0;
                        }
                    }
                }
            }
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (CalamityEntropy.EntropyMode)
            {
                if (npc.ModNPC != null)
                {
                    if (npc.ModNPC is DevourerofGodsTail || npc.ModNPC is DevourerofGodsHead || npc.ModNPC is DevourerofGodsBody)
                    {
                        if (NPC.AnyNPCs(ModContent.NPCType<Signus>()) || NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) || NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool EDogSpawnSignus = true;
        public bool EDogSpawnSWeaver = true;
        public bool EDogSpawnCVoid = true;
        public int ImmuneTimeForAS = 300;
        public override void PostAI(NPC npc)
        {
            if (CalamityEntropy.EntropyMode)
            {
                if (npc.type == NPCID.WallofFleshEye)
                {
                    if (npc.Entropy().counter % 400 < 60 && npc.Entropy().counter % 6 == 0)
                    {
                        Vector2 lookAt = Main.player[npc.target].Center + Main.player[npc.target].velocity * 10;

                        float velocity = 26;
                        int projectileType = ProjectileID.EyeLaser;
                        int damage = npc.GetProjectileDamage(projectileType);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            {
                                Vector2 projectileVelocity = (lookAt - npc.Center).SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(10)) * velocity;
                                Vector2 projectileSpawn = npc.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 150f;

                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;

                                Main.projectile[proj].tileCollide = false;
                            }
                            {
                                Vector2 projectileVelocity = (lookAt - npc.Center).SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-10)) * velocity;
                                Vector2 projectileSpawn = npc.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 150f;

                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;

                                Main.projectile[proj].tileCollide = false;
                            }
                        }
                    }
                }
                if (npc.type == NPCID.EyeofCthulhu)
                {
                    if (init)
                    {
                        npc.scale *= 1.4f;
                    }
                }
                if (SlimeGodSlimes.Contains(npc.type))
                {
                    npc.MaxFallSpeedMultiplier *= 12;
                }
                if (npc.type == NPCID.PlanterasHook)
                {
                    if (Main.GameUpdateCount % 40 == 0 && Main.rand.NextBool(2))
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (NPC.plantBoss.ToNPC().target.ToPlayer().Center - npc.Center).normalize() * 22, ProjectileID.SeedPlantera, npc.GetProjectileDamage(ProjectileID.SeedPlantera), 2, Main.myPlayer);
                    }
                }
                if (npc.type == NPCID.KingSlime)
                {
                    npc.MaxFallSpeedMultiplier *= 36f;

                    if (this.ksFlag && npc.velocity.Y != 0f && npc.velocity.Y < 0f)
                    {
                        this.ksFlag2 = false;
                        npc.velocity.Y = npc.velocity.Y * 3f;
                        npc.velocity.X = npc.velocity.X * 1.4f;
                        if (Utils.NextBool(Main.rand, 3))
                        {
                            npc.velocity.Y = npc.velocity.Y * 1.4f;
                            this.ksFlag2 = true;
                        }
                    }
                    if (npc.velocity.X != 0f && npc.velocity.Y != 0f && this.ksFlag2 && Math.Sign(npc.velocity.X) != Math.Sign(npc.target.ToPlayer().Center.X - npc.Center.X))
                    {
                        npc.velocity.X = npc.velocity.X * 0.1f;
                        npc.velocity.Y = -4f;
                        this.ksFlag2 = false;
                    }
                    if (!this.ksFlag && npc.velocity.Y == 0f && !Main.dedServ)
                    {
                        CEUtils.PlaySound("ksLand", 1f, new Vector2?(npc.Center), 2, 1f);
                        CalamityUtils.Calamity(Main.LocalPlayer).GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(npc.Center), 2000f, 1000f, 0f, 12f, true);
                    }
                    this.ksFlag = (npc.velocity.Y == 0f);
                    if (npc.velocity.Y == 0f)
                    {
                        this.vyAdd = 0f;
                    }
                    if (npc.velocity.Y != 0f)
                    {
                        this.vyAdd = 0.65f;
                        if (this.ksFlag2)
                        {
                            this.vyAdd = 0.4f;
                        }
                        npc.velocity.Y = npc.velocity.Y + this.vyAdd;
                    }
                    if (SpawnAtHalfLife)
                    {
                        SpawnAtHalfLife = false;
                        Vector2 vector = npc.Center + new Vector2(0, -40 * npc.scale);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<TopazJewel>());

                    }
                }
                if (npc.ModNPC != null)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (npc.ModNPC is CryogenShield)
                        {
                            if (npc.Entropy().counter % 22 == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int iceBlast = Main.zenithWorld ? ModContent.ProjectileType<BrimstoneBarrage>() : ModContent.ProjectileType<IceBlast>();

                                    int totalProjectiles = BossRushEvent.BossRushActive ? 8 : 5;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    int type = iceBlast;
                                    int damage = npc.GetProjectileDamage(type);
                                    float velocity = 26f;
                                    float projectileVelocityToPass = 0f;
                                    Vector2 spinningPoint = new Vector2(0f, -velocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k + Main.GlobalTimeWrappedHourly);
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                                    }
                                }
                            }
                        }
                        if (npc.ModNPC is Cryogen)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<CryogenShield>()))
                            {
                                if (npc.Entropy().counter % 600 == 0)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int totalProjectiles = 3;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int type = ModContent.ProjectileType<IceBomb>();
                                        int damage = npc.GetProjectileDamage(type);
                                        float velocity = 2f + npc.ai[0];
                                        double angleA = radians * 0.5;
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                                        Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                                        for (int k = 0; k < totalProjectiles; k++)
                                        {
                                            Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                                if (npc.Entropy().counter % 6 == 0)
                                {
                                    if (npc.Entropy().counter % 30 == 0)
                                    {
                                        int iceBlast = ModContent.ProjectileType<IceBlast>();

                                        int totalProjectiles = BossRushEvent.BossRushActive ? 6 : 4;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int type = iceBlast;
                                        int damage = npc.GetProjectileDamage(type);
                                        float velocity = 10f;
                                        float projectileVelocityToPass = 28f;
                                        Vector2 spinningPoint = new Vector2(0f, -velocity);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {

                                            for (int k = 0; k < totalProjectiles; k++)
                                            {
                                                Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k + Main.GlobalTimeWrappedHourly);
                                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                                            }
                                        }
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (npc.target.ToPlayer().Center - npc.Center).normalize() * 20, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);

                                    }
                                }
                            }
                        }
                    }
                    if (npc.ModNPC is DevourerofGodsTail || npc.ModNPC is DevourerofGodsHead || npc.ModNPC is DevourerofGodsBody)
                    {
                        if (NPC.AnyNPCs(ModContent.NPCType<Signus>()) || NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) || NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()))
                        {
                            if (npc.ModNPC is DevourerofGodsHead && npc.HasValidTarget)
                            {
                                if (CEUtils.getDistance(npc.Center, CEUtils.ToPlayer(npc.target).Center) < 2000)
                                {
                                    npc.velocity += CEUtils.normalize((npc.Center - CEUtils.ToPlayer(npc.target).Center)) * 2;
                                }
                            }
                            npc.Calamity().CurrentlyIncreasingDefenseOrDR = true;
                            if (npc.ModNPC is DevourerofGodsHead dogHead)
                            {
                                dogHead.laserWallPhase = 2;
                            }
                        }
                        else
                        {
                            npc.Calamity().CurrentlyIncreasingDefenseOrDR = false;
                        }
                    }
                    if (npc.ModNPC is DevourerofGodsHead dog)
                    {
                        if (!(Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            float lifePer = (float)npc.life / npc.lifeMax;
                            if (lifePer < 0.4f && EDogSpawnSignus)
                            {
                                EDogSpawnSignus = false;
                                int npc_ = SpawnBoss((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Signus>(), Player.FindClosest(npc.Center, 9999, 9999));
                                npc_.ToNPC().lifeMax /= 6;
                                npc_.ToNPC().life /= 6;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc_);
                            }
                            if (lifePer < 0.25f && EDogSpawnSWeaver)
                            {
                                EDogSpawnSWeaver = false;
                                int npc_ = SpawnBoss((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<StormWeaverHead>(), Player.FindClosest(npc.Center, 9999, 9999));
                                npc_.ToNPC().lifeMax /= 6;
                                npc_.ToNPC().life /= 6;
                                npc_.ToNPC().active = true;
                                npc_.ToNPC().Center = npc.Center;
                                npc_.ToNPC().timeLeft *= 20;
                                npc_.ToNPC().target = 0;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc_);
                            }
                            if (lifePer < 0.15f && EDogSpawnCVoid)
                            {
                                EDogSpawnCVoid = false;
                                int npc_ = SpawnBoss((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CeaselessVoid>(), Player.FindClosest(npc.Center, 9999, 9999));
                                npc_.ToNPC().lifeMax /= 6;
                                npc_.ToNPC().life /= 6;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc_);
                            }
                        }
                    }
                    if (npc.ModNPC is Signus)
                    {
                        if (Main.GameUpdateCount % 400 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npc.HasValidTarget)
                                {
                                    int pt = ModContent.ProjectileType<DoGDeath>();
                                    for (float i = -25; i < 25; i += 5)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (npc.target.ToPlayer().Center - npc.Center).normalize().RotatedBy(MathHelper.ToRadians(i)) * 10, pt, npc.GetProjectileDamage(pt), 2);
                                    }
                                }
                            }
                        }
                    }
                    if (npc.ModNPC is StormWeaverBody)
                    {
                        if (Main.GameUpdateCount % 380 == 0 && (Main.rand.NextBool(6) || NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>())))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npc.realLife.ToNPC().HasValidTarget)
                                {
                                    int pt = ModContent.ProjectileType<DoGDeath>();
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (npc.realLife.ToNPC().target.ToPlayer().Center - npc.Center).normalize() * 12, pt, npc.realLife.ToNPC().GetProjectileDamage(pt), 2);
                                }
                            }
                        }
                    }

                    if (npc.ModNPC is DarkEnergy)
                    {
                        if (Main.GameUpdateCount % 260 == 0 && Main.rand.NextBool(4))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                var parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CeaselessVoid>())];
                                if (parent.HasValidTarget)
                                {
                                    int pt = ModContent.ProjectileType<DoGDeath>();
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (parent.target.ToPlayer().Center - npc.Center).normalize() * 8, pt, parent.GetProjectileDamage(pt), 2);
                                }
                            }
                        }
                    }
                    if (npc.ModNPC is CeaselessVoid)
                    {
                        if (Main.GameUpdateCount % 300 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                var parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CeaselessVoid>())];
                                if (parent.HasValidTarget)
                                {
                                    int pt = ModContent.ProjectileType<DoGDeath>();
                                    for (float i = 0; i < 358; i += 20)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, MathHelper.ToRadians(i).ToRotationVector2() * 8, pt, parent.GetProjectileDamage(pt), 2);
                                    }
                                }
                            }
                        }
                    }
                    if (npc.ModNPC is CrabShroom)
                    {
                        npc.dontTakeDamage = true;
                    }
                    if (npc.ModNPC is DesertScourgeHead && npc.localAI[2] == 1f && this.dScFLag)
                    {
                        this.dScFLag = false;
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHead>());
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHeadYoung>());
                    }
                    if (npc.ModNPC is Crabulon)
                    {
                        npc.MaxFallSpeedMultiplier *= 10f;
                        if (npc.velocity.Length() < 40)
                        {
                            npc.velocity *= 1.01f;
                        }
                        if (this.ksFlag && npc.velocity.Y != 0f && npc.velocity.Y < 0f)
                        {
                            this.ksFlag2 = false;
                            npc.velocity.Y = npc.velocity.Y * 1.34f;
                            npc.velocity.X = npc.velocity.X * 1.5f;
                            if (Utils.NextBool(Main.rand, 3))
                            {
                                npc.velocity.Y = npc.velocity.Y * 1.4f;
                                this.ksFlag2 = true;
                            }
                        }
                        if (npc.velocity.X != 0f && npc.velocity.Y != 0f && this.ksFlag2 && Math.Sign(npc.velocity.X) != Math.Sign(npc.target.ToPlayer().Center.X - npc.Center.X))
                        {
                            npc.velocity.X = npc.velocity.X * 0.1f;
                            npc.velocity.Y = -4f;
                            this.ksFlag2 = false;
                        }
                        this.ksFlag = (npc.velocity.Y == 0f);
                        if (npc.velocity.Y == 0f)
                        {
                            this.vyAdd = 0f;
                        }
                        if (npc.velocity.Y != 0f)
                        {
                            this.vyAdd = 0.65f;
                            if (this.ksFlag2)
                            {
                                this.vyAdd = 0.4f;
                            }
                            npc.velocity.Y = npc.velocity.Y + this.vyAdd;
                        }
                    }
                }
            }
            init = false;
        }
        public static int SpawnBoss(int spawnPositionX, int spawnPositionY, int Type, int targetPlayerIndex)
        {
            int num = 200;
            num = NPC.NewNPC(NPC.GetBossSpawnSource(targetPlayerIndex), spawnPositionX, spawnPositionY, Type, 1);


            if (num == 200)
                return -1;

            Main.npc[num].target = targetPlayerIndex;
            Main.npc[num].timeLeft *= 20;
            string typeName = Main.npc[num].TypeName;
            if (Main.netMode == 2 && num < 200)
                NetMessage.SendData(23, -1, -1, null, num);



            if (Main.netMode == 0)
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), 175, 75);
            else if (Main.netMode == 2)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[num].GetTypeNetName()), new Color(175, 75, 255));
            return num;
        }
        public override void OnKill(NPC npc)
        {
            if (CalamityEntropy.EntropyMode && (npc.ModNPC is KingSlimeJewelEmerald || npc.ModNPC is KingSlimeJewelRuby || npc.ModNPC is KingSlimeJewelSapphire || npc.ModNPC is TopazJewel) && Main.netMode != 1)
            {
                Vector2 vector = npc.Center;
                NPC.NewNPC(npc.GetSource_FromAI(null), (int)vector.X, (int)vector.Y - 60, 288, 0, 0f, 0f, 0f, 0f, 255).ToNPC().life /= 2;
            }
        }

        public bool SpawnAtHalfLife = true;

        public bool ksFlag;

        public float vyAdd;

        public bool init = true;

        public bool ksFlag2;

        public bool dScFLag = true;
    }
}
