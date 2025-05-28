using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser
{
    [AutoloadBossHead]
    public class CruiserHead : ModNPC
    {
        public float ProgressDraw = 0;
        private int length = 27;
        public float speedMuti = 1;
        public float speed = 18;
        public float targetSpeed = 18;
        public int noaitime = 280;
        public int notargettime = 0;
        public int tjv = 0;
        public bool bite = false;
        public float mouthRot = 0;
        public int tail = -1;
        private bool b_added = false;
        public int nrc = 0;
        float ja = 50;
        float da = 50;
        float tail_vj = 0;
        bool jv = false;
        public int phaseTrans = 0;
        public bool flag = false;
        public int slowDownTime = 0;
        public List<Vector2> bodies = new List<Vector2>();
        public Vector2 vtodraw = new Vector2();
        public float jaslowdown = 0;
        public float aitype = 0;
        public int changeCounter = 0;
        public float maxDistance = 6000;
        public float maxDistanceTarget = 2900;
        public int rotDist = 900;
        public Texture2D disTex = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cruiserSpace").Value;
        public Vector2 rotPos = Vector2.Zero;
        public int phase = 1;
        public int circleDir = 1;
        public float alpha = 1;
        public bool candraw = false;
        public bool DeathAnm = false;
        public int DeathAnmCount = 200;

        public static int icon = ModContent.GetModBossHeadSlot("CalamityEntropy/Content/NPCs/Cruiser/CruiserHead_Head_Boss");
        public static int iconP2;
        public static void loadHead()
        {
            string path = "CalamityEntropy/Content/NPCs/Cruiser/p2head";
            CalamityEntropy.Instance.AddBossHeadTexture(path, -1);
            iconP2 = ModContent.GetModBossHeadSlot(path);

        }
        public override void BossHeadSlot(ref int index)
        {
            if (phaseTrans >= 120)
            {
                index = iconP2;
            }
            else
            {
                index = icon;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.48f,
                PortraitScale = 0.56f,
                CustomTexturePath = "CalamityEntropy/Assets/Extra/CruiserBes",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<PrimordialWyrmHead>()] = true;
        }
        int tdamage = 0;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.CruiserBestiary")
            });
        }
        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.Calamity().DR = 0.3f;
            NPC.boss = true;
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 180;
            if (Main.expertMode)
            {
                NPC.damage += 10;
            }
            if (Main.masterMode)
            {
                NPC.damage += 10;
            }
            NPC.defense = 100;
            NPC.lifeMax = 1200000;
            if (CalamityWorld.death)
            {
                NPC.damage += 24;
                length += 4;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 18;
                length += 2;
            }
            tdamage = NPC.damage;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCHit4;
            NPC.value = 100000f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.9f;
            NPC.dontCountMe = true;
            NPC.scale = 1f;
            if (Main.masterMode)
            {
                NPC.scale = 1.12f;
            }
            if (Main.getGoodWorld)
            {
                NPC.scale = 0.5f;
                length += 46;
            }
            if (Main.zenithWorld)
            {
                NPC.scale = 1.4f;
            }
            NPC.netAlways = true;
            NPC.Entropy().damageMul = 0.1f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/CruiserBoss");
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(Main.zenithWorld ? ModContent.BuffType<MaliciousCode>() : ModContent.BuffType<VoidTouch>(), 120);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CruiserBag>()));

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ModContent.ItemType<OmegaHealingPotion>(), 1, 5, 15), hideLootReport: true);


            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                normalOnly.Add(ModContent.ItemType<VoidRelics>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidElytra>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidEcho>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<Silence>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidAnnihilate>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<WindOfUndertaker>(), new Fraction(2, 5));
                normalOnly.Add(ModContent.ItemType<WingsOfHush>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidCandle>(), new Fraction(2, 5));
                normalOnly.Add(ModContent.ItemType<VoidMonolith>(), 3);
                normalOnly.Add(ModContent.ItemType<VoidToy>(), 3);
                normalOnly.Add(ModContent.ItemType<VoidScales>(), new Fraction(1, 1), 88, 128);
            }
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CruiserRelic>());

            npcLoot.Add(ModContent.ItemType<CruiserTrophy>(), 10);

            npcLoot.AddConditionalPerPlayer(() => !EDownedBosses.downedCruiser, ModContent.ItemType<CruiserLore>());
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedMuti);
            writer.Write(targetSpeed);
            writer.Write(slowDownTime);
            writer.Write(nrc);
            writer.Write((byte)ai);
            writer.Write(changeCounter);
            writer.Write(maxDistanceTarget);
            writer.Write(rotDist);
            writer.WriteVector2(rotPos);
            writer.Write(circleDir);
            writer.Write(flag);
            writer.Write(noaitime);
            writer.Write(phaseTrans);
            writer.Write(phase);
            writer.WriteVector2(SpaceCenter);
            writer.Write(DeathAnm);
            writer.Write(DeathAnmCount);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedMuti = reader.ReadSingle();
            targetSpeed = reader.ReadSingle();
            slowDownTime = reader.ReadInt32();
            nrc = reader.ReadInt32();
            ai = (AIStyle)reader.ReadByte();
            changeCounter = reader.ReadInt32();
            maxDistanceTarget = reader.ReadSingle();
            rotDist = reader.ReadInt32();
            rotPos = reader.ReadVector2();
            circleDir = reader.ReadInt32();
            flag = reader.ReadBoolean();
            noaitime = reader.ReadInt32();
            phaseTrans = reader.ReadInt32();
            phase = reader.ReadInt32();
            SpaceCenter = reader.ReadVector2();
            DeathAnm = reader.ReadBoolean();
            DeathAnmCount = reader.ReadInt32();
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            NPC.Entropy().damageMul *= 0.98f;
            if (phase == 2)
            {
                modifiers.FinalDamage *= 1.2f;
            }

        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            NPC.Entropy().damageMul *= 0.98f;
            if (phase == 2)
            {
                modifiers.FinalDamage *= 1.2f;
            }
        }
        public override bool CheckDead()
        {
            if (DeathAnmCount <= 0)
            {
                return true;
            }
            DeathAnm = true;
            NPC.damage = 0;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            NPC.active = true;
            NPC.netUpdate = true;
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;
            return false;
        }

        public void changeAi()
        {
            changeCounter = 0;
            NPC.netUpdate = true;
            if (phase == 1)
            {
                aiRound++;
                if (aiRound > 19)
                {
                    aiRound = 0;
                }
                if (aiRound == 1 || aiRound == 3 || aiRound == 5)
                {
                    ai = AIStyle.StayAwayAndShootVoidStar;
                }
                if (aiRound == 0 || aiRound == 2 || aiRound == 4)
                {
                    ai = AIStyle.TryToClosePlayer;
                }
                if (aiRound == 8 || aiRound == 10 || aiRound == 12)
                {
                    ai = AIStyle.StayAwayAndShootVoidStar;
                }
                if (aiRound == 7 || aiRound == 9 || aiRound == 11)
                {
                    ai = AIStyle.TryToClosePlayer;
                }
                if (aiRound == 14 || aiRound == 16 || aiRound == 18)
                {
                    ai = AIStyle.StayAwayAndShootVoidStar;
                }
                if (aiRound == 15 || aiRound == 17)
                {
                    ai = AIStyle.TryToClosePlayer;
                }

                if (aiRound == 6 || aiRound == 19)
                {
                    ai = Main.rand.NextBool() ? AIStyle.EnergyBall : AIStyle.VoidResidue;
                }
                if (aiRound == 13)
                {
                    ai = AIStyle.AroundPlayerAndShootVoidStar;
                }

            }
            else
            {
                aiRound++;
                if (aiRound >= 9)
                {
                    aiRound = 0;
                }
                if (aiRound == 0 || aiRound == 2)
                {
                    ai = AIStyle.VoidSpike;
                }
                if (aiRound == 1)
                {
                    ai = Main.rand.NextBool() ? AIStyle.BiteAndDash : AIStyle.AroundSpawnVoidBomb;
                }
                if (aiRound == 3)
                {
                    ai = AIStyle.SplittingVoidStar;
                }
                if (aiRound == 4)
                {
                    ai = AIStyle.QuickDash;
                }
                if (aiRound == 5)
                {
                    ai = AIStyle.Cruise;
                    if (Main.rand.NextBool())
                    {
                        ai = AIStyle.VoidSpike;
                    }
                }
                if (aiRound == 6)
                {
                    ai = AIStyle.VoidLaser;
                }
                if (aiRound == 7)
                {
                    ai = Main.rand.NextBool() ? AIStyle.VoidResidue : AIStyle.SplittingVoidStar;
                }
                if (aiRound == 8)
                {
                    ai = AIStyle.Cruise;
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return noaitime <= 0 && ai != AIStyle.BiteAndDash;
        }
        public override bool CanHitNPC(NPC target)
        {
            return noaitime <= 0 && base.CanHitNPC(target);
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (noaitime > 0)
            {
                return false;
            }
            return base.CanBeHitByItem(player, item);
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (noaitime > 0)
            {
                return false;
            }
            return base.CanBeHitByProjectile(projectile);
        }
        public override bool CanBeHitByNPC(NPC attacker)
        {
            return noaitime <= 0 && base.CanBeHitByNPC(attacker);
        }
        public int counterc = 0;
        public Vector2 SpaceCenter = Vector2.Zero;
        public enum AIStyle
        {
            TryToClosePlayer,
            StayAwayAndShootVoidStar,
            AroundPlayerAndShootVoidStar,
            EnergyBall,
            VoidResidue,

            PhaseTransing,

            VoidSpike,
            BiteAndDash,
            Cruise,
            SplittingVoidStar,
            QuickDash,
            AroundSpawnVoidBomb,
            VoidLaser
        }
        public int aiRound = 0;
        public AIStyle ai = AIStyle.TryToClosePlayer;
        public void Shoot(int type, Vector2 pos, Vector2 velo, float damageMult = 1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velo, type, (int)(NPC.damage / 8 * damageMult), 3, -1, ai0, ai1, ai2);
        }
        public float whiteLerp = 0;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && DeathAnmCount <= 10)
            {
                CEUtils.PlaySound("VoidAttack", 1, NPC.Center);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 16;
                for (int i = 0; i < 86; i++)
                {
                    Particle p = new Particle();
                    p.position = NPC.Center;
                    p.alpha = Main.rand.NextFloat(1f, 2f);
                    p.shape = 4;
                    p.vd = 0.97f;
                    p.velocity = CEUtils.randomPointInCircle(16);
                    VoidParticles.particles.Add(p);
                }
            }
        }
        public float camLerp = 0;
        public override void AI()
        {
            bool canShoot = Main.netMode != NetmodeID.MultiplayerClient;
            if (DeathAnm)
            {
                if (camLerp < 1)
                {
                    camLerp += 0.025f;
                }
                else
                {
                    camLerp = 24f;
                }
                Main.LocalPlayer.Entropy().screenShift = camLerp;
                Main.LocalPlayer.Entropy().screenPos = NPC.Center;
                if (NPC.velocity.Length() > 6)
                {
                    NPC.velocity *= 0.96f;
                }
                NPC.rotation = NPC.velocity.ToRotation();
                DeathAnmCount--;
                if (whiteLerp < 1)
                    whiteLerp += 1 / 160f;
                if (DeathAnmCount % 6 == 0 && !Main.dedServ)
                {
                    EParticle.NewParticle(new PremultBurst(), NPC.Center, Vector2.Zero, Color.LightBlue, 3.2f, 1, true, BlendState.Additive, 0);
                }
                if (DeathAnmCount <= 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.StrikeInstantKill();
                        NPC.netSpam = 9;
                        NPC.netUpdate = true;
                    }
                }
                vtodraw = NPC.Center;
                for (int i = 0; i < bodies.Count; i++)
                {
                    Vector2 oPos;
                    float oRot;

                    if (i == 0)
                    {
                        oPos = NPC.Center;
                        oRot = NPC.rotation;
                    }
                    else
                    {
                        oPos = bodies[i - 1];
                        if (i == 1)
                        {
                            oRot = (NPC.Center - bodies[0]).ToRotation();
                        }
                        else
                        {
                            oRot = (bodies[i - 2] - bodies[i - 1]).ToRotation();
                        }
                    }
                    float rot = (oPos - bodies[i]).ToRotation();
                    rot = CEUtils.rotatedToAngle(rot, oRot, 0.12f, false);

                    int spacing = 80;
                    bodies[i] = oPos - rot.ToRotationVector2() * spacing * NPC.scale;
                }
                return;
            }
            NPC.Entropy().damageMul += 1f / 10000f;
            if (NPC.Entropy().damageMul > 1)
            {
                NPC.Entropy().damageMul = 1;
            }
            counterc++;
            if (noaitime > 0)
            {
                NPC.dontTakeDamage = true;
                for (int i = 0; i < bodies.Count; i++)
                {
                    bodies[i] = NPC.Center;
                }
                foreach (Projectile pj in Main.ActiveProjectiles)
                {
                    if (pj.ModProjectile is VoidBottleThrow)
                    {
                        NPC.Center = pj.Center;
                        break;
                    }
                }
            }
            else
            {
                if (ai == AIStyle.PhaseTransing)
                {
                    NPC.dontTakeDamage = true;
                }
            }
            noaitime--;

            if (noaitime == 0)
            {
                NPC.dontTakeDamage = false;
            }

            if (noaitime < 0)
            {
                if (!b_added)
                {
                    b_added = true;
                    for (int i = 0; i < length + 1; i++)
                    {
                        bodies.Add(NPC.Center - new Vector2(0, 0));
                    }
                    if (!(Main.netMode == NetmodeID.MultiplayerClient))
                    {
                        int bodyIndex;
                        int syg;
                        syg = NPC.whoAmI;
                        for (int i = 0; i < length + 1; i++)
                        {
                            int type = ModContent.NPCType<CruiserBody>();
                            if (i == length)
                            {
                                type = ModContent.NPCType<CruiserTail>();
                            }

                            bodyIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);

                            Main.npc[bodyIndex].ai[1] = syg;
                            Main.npc[bodyIndex].ai[2] = i;
                            Main.npc[bodyIndex].ai[3] = NPC.whoAmI;
                            Main.npc[bodyIndex].realLife = NPC.whoAmI;
                            syg = bodyIndex;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bodyIndex);
                                Main.npc[bodyIndex].netUpdate = true;
                            }

                        }
                        NPC.ai[3] = syg;
                        tail = syg;
                    }
                }
                Main.LocalPlayer.Entropy().crSky = 30;
                NPC.TargetClosest();
                maxDistance += (maxDistanceTarget - maxDistance) * 0.001f;
                if (noaitime <= 0)
                {
                    foreach (Player p in Main.ActivePlayers)
                    {
                        if (CEUtils.getDistance(SpaceCenter, p.Center) > maxDistance)
                        {
                            if (!Main.dedServ)
                            {
                                p.AddBuff(ModContent.BuffType<VoidTouch>(), 5);
                            }
                        }
                    }
                    if (NPC.HasValidTarget)
                    {
                        Player target = NPC.target.ToPlayer();
                        if (!bite && NPC.Distance(target.Center) < 900 && ai != AIStyle.SplittingVoidStar && ai != AIStyle.VoidResidue && ai != AIStyle.BiteAndDash && ai != AIStyle.EnergyBall && ai != AIStyle.AroundPlayerAndShootVoidStar && ai != AIStyle.AroundSpawnVoidBomb)
                        {
                            mouthRot += Utils.Remap(NPC.Distance(target.Center), 900, 50, 0, 7f);
                            if (NPC.Distance(target.Center) < float.Max(30, NPC.velocity.Length()) * 4.6f)
                            {
                                bite = true;
                            }
                        }

                        int phaseNow = 1;
                        if (NPC.life < NPC.lifeMax / 2)
                        {
                            phaseNow = 2;
                        }
                        phase = phaseNow;
                        if (phaseNow == 2)
                        {

                            if (phaseTrans < 122)
                            {
                                ai = AIStyle.PhaseTransing;
                                phaseTrans++;
                                alpha *= 0.967f;
                                aiRound = 0;
                                if (phaseTrans <= 60)
                                {
                                    da = 0;
                                    tail_vj = 0;
                                    jv = false;
                                    foreach (Projectile p in Main.ActiveProjectiles)
                                    {
                                        if (p.ModProjectile is CruiserEnergyBall || p.ModProjectile is VoidResidue)
                                        {
                                            p.active = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ai == AIStyle.PhaseTransing)
                                {
                                    NPC.Entropy().VoidTouchDR = 0.4f;
                                    ai = AIStyle.VoidSpike;
                                    NPC.dontTakeDamage = false;
                                    NPC.width = 156;
                                    NPC.height = 156;
                                    foreach (NPC n in Main.npc)
                                    {
                                        if (n.realLife == NPC.whoAmI)
                                        {
                                            if (n.ai[2] <= 8 && n.ai[2] > 4)
                                            {
                                                n.width = 26;
                                                n.height = 26;
                                            }
                                            n.netUpdate = true;
                                            if (n.ai[2] > 8)
                                            {
                                                n.active = false;
                                            }
                                            if (Main.dedServ)
                                            {
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI, 0f, 0f, 0f, 0);
                                            }
                                        }
                                    }
                                }
                                if (alpha < 1)
                                {
                                    alpha += 0.02f;
                                    if (alpha > 1)
                                    {
                                        alpha = 1;
                                    }
                                }
                            }
                        }
                        maxDistanceTarget = 6000;
                        SpaceCenter = (NPC.Center + bodies[bodies.Count - 1]) / 2f;
                        if (ai == AIStyle.PhaseTransing)
                        {
                            SpaceCenter = target.Center;
                            maxDistanceTarget = 1000;
                            if (NPC.velocity.Length() < 8)
                            {
                                NPC.velocity *= 1.01f;
                            }
                            else
                            {
                                NPC.velocity *= 0.98f;
                            }
                            foreach (var p in bodies)
                            {
                                VoidParticles.particles.Add(new Particle() { position = p, alpha = Main.rand.NextFloat(0.2f, 1.4f), shape = 4, velocity = CEUtils.randomPointInCircle(6) });
                            }
                        }
                        if (ai == AIStyle.TryToClosePlayer)
                        {
                            if (NPC.velocity.Length() < 36)
                            {
                                NPC.velocity *= 1.046f;
                            }
                            NPC.velocity += (target.Center - NPC.Center).normalize() * 0.1f;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).normalize() * NPC.velocity.Length(), 0.08f);
                            NPC.velocity *= 0.998f;
                            changeCounter++;
                            if (changeCounter > 500 || NPC.Distance(target.Center) < 440)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.StayAwayAndShootVoidStar)
                        {
                            if (NPC.velocity.Length() < 40)
                            {
                                NPC.velocity *= 1.1f;
                            }
                            else
                            {
                                NPC.velocity *= 0.97f;
                            }
                            changeCounter++;
                            if (changeCounter == 90)
                            {
                                tjv = 1;
                            }
                            if (changeCounter > 70)
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).normalize() * NPC.velocity.Length(), 0.07f);
                                if (NPC.velocity.Length() < 30)
                                {
                                    NPC.velocity *= 1.02f;
                                }
                            }
                            if (changeCounter > 130)
                            {
                                if (NPC.velocity.Length() < 30)
                                {
                                    NPC.velocity *= 1.046f;
                                }
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 0.1f;
                                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).normalize() * NPC.velocity.Length(), 0.08f);
                                NPC.velocity *= 0.998f;
                            }
                            if (changeCounter > 170)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.EnergyBall)
                        {
                            if (changeCounter == 0)
                            {
                                if (canShoot)
                                {
                                    Shoot(ModContent.ProjectileType<CruiserEnergyBall>(), NPC.Center, Vector2.Zero, 1.15f, NPC.whoAmI);
                                }
                            }
                            changeCounter++;
                            if (changeCounter > 300)
                            {
                                changeAi();
                            }
                            NPC.velocity += (target.Center - NPC.Center).normalize() * (NPC.Distance(target.Center) > 700 ? 3 : 1);
                            NPC.velocity *= 0.92f;
                        }
                        if (ai == AIStyle.VoidResidue)
                        {
                            if (changeCounter < 100)
                            {
                                mouthRot -= 4.8f;
                            }
                            else
                            {
                                if (changeCounter < 120)
                                {
                                    mouthRot += 5f;
                                }
                            }
                            changeCounter++;
                            if (changeCounter < 100 && NPC.Distance(target.Center) > 800)
                            {
                                NPC.velocity *= 0.98f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 1f;
                            }
                            else
                            {
                                NPC.velocity *= 0.94f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 0.26f;
                            }
                            if (changeCounter == 100)
                            {
                                if (canShoot)
                                {
                                    for (int i = 0; i < 80; i++)
                                    {
                                        Shoot(ModContent.ProjectileType<VoidResidue>(), NPC.Center, NPC.velocity.normalize().RotatedByRandom(2f) * 24 * Main.rand.NextFloat(0.2f, 1f), 0.8f);
                                    }
                                }
                                CEUtils.PlaySound("brimstonevortexshoot", 1, NPC.Center);
                                CEUtils.PlaySound("vbuse", 1, NPC.Center);
                            }
                            if (changeCounter > 140)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.AroundPlayerAndShootVoidStar)
                        {
                            Vector2 targetPos = target.Center + (NPC.Center - target.Center).normalize().RotatedBy(0.6f) * 600;
                            NPC.velocity += (targetPos - NPC.Center).normalize() * 3f;
                            NPC.velocity *= 0.98f;
                            changeCounter++;
                            if (changeCounter % 40 == 0)
                            {
                                tjv = 1;
                            }
                            if (changeCounter > 40 * 8 + 30)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.VoidSpike)
                        {
                            NPC.velocity = NPC.velocity.normalize() * (NPC.velocity.Length() + (46 - NPC.velocity.Length()) * 0.08f);
                            NPC.velocity = CEUtils.rotatedToAngle(NPC.velocity.ToRotation(), (target.Center - NPC.Center).ToRotation(), 0.0376f, false).ToRotationVector2() * NPC.velocity.Length();
                            changeCounter++;
                            if (changeCounter == 40 || changeCounter == 60 || changeCounter == 80 || changeCounter == 100)
                            {
                                if (canShoot)
                                {
                                    for (float i = 0; i < 360; i += 30)
                                    {
                                        Shoot(ModContent.ProjectileType<VoidSpike>(), NPC.Center, MathHelper.ToRadians(i).ToRotationVector2() * 8);
                                    }
                                }
                            }
                            if (changeCounter > 150)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.BiteAndDash)
                        {
                            if (changeCounter == 0)
                            {
                                NPC.velocity *= 0.9f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 6;
                                if (CEUtils.getDistance(NPC.Center + NPC.rotation.ToRotationVector2() * 160, target.Center) < 160)
                                {
                                    changeCounter++;
                                    target.velocity *= 0;
                                    target.Center = NPC.Center + NPC.rotation.ToRotationVector2() * 160;
                                }
                            }
                            else
                            {
                                changeCounter++;
                                if (changeCounter < 20)
                                {
                                    mouthRot -= 5f;
                                    NPC.velocity = NPC.velocity.normalize() * (NPC.velocity.Length() + (80 - NPC.velocity.Length()) * 0.2f);

                                    if (CEUtils.getDistance(NPC.Center + NPC.rotation.ToRotationVector2() * 160, target.Center) < 160)
                                    {
                                        target.velocity *= 0;
                                        target.Entropy().immune = 12;
                                        target.Center = NPC.Center + NPC.rotation.ToRotationVector2() * 160;
                                    }
                                    if (!CEUtils.isAir(NPC.Center + NPC.rotation.ToRotationVector2() * 360))
                                    {
                                        changeCounter = 60;
                                    }
                                }
                                else
                                {
                                    Vector2 targetPos = target.Center + (NPC.Center - target.Center).normalize().RotatedBy(0.6f) * 1600;
                                    NPC.velocity += (targetPos - NPC.Center).normalize() * 1f;
                                    NPC.velocity *= 0.98f;
                                    if (changeCounter > 120)
                                    {
                                        changeAi();
                                    }
                                }
                                if (changeCounter == 20)
                                {
                                    if (CEUtils.getDistance(NPC.Center + NPC.rotation.ToRotationVector2() * 80, target.Center) < 160)
                                    {
                                        target.velocity = NPC.velocity * 1.6f;
                                        target.Entropy().CruiserAntiGravTime = 100;
                                    }

                                    if (canShoot)
                                    {
                                        for (int i = 1; i < 18; i++)
                                        {
                                            for (int j = -6; j < 7; j++)
                                            {
                                                if (j == 0)
                                                {
                                                    Shoot(ModContent.ProjectileType<CruiserSlash>(), NPC.Center + NPC.velocity.normalize() * 300 * i, NPC.velocity);
                                                }
                                                else
                                                {
                                                    Shoot(ModContent.ProjectileType<CruiserSlash>(), NPC.Center + NPC.velocity.normalize().RotatedBy(0.09f * j) * 300 * i, NPC.velocity.RotatedBy(0.09f * j));
                                                }
                                            }
                                        }
                                    }
                                    NPC.velocity *= 0.3f;
                                }

                            }
                        }
                        if (ai == AIStyle.AroundSpawnVoidBomb)
                        {
                            NPC.velocity = NPC.velocity.normalize() * (NPC.velocity.Length() + (32 - NPC.velocity.Length()) * 0.08f);
                            NPC.velocity = CEUtils.rotatedToAngle(NPC.velocity.ToRotation(), (target.Center - NPC.Center).ToRotation(), 0.028f, false).ToRotationVector2() * NPC.velocity.Length();

                            changeCounter++;
                            if (changeCounter < 180)
                            {
                                if (changeCounter % 7 == 0)
                                {
                                    if (canShoot)
                                        Shoot(ModContent.ProjectileType<VoidBomb>(), NPC.Center, CEUtils.randomPointInCircle(8) + (target.Center - NPC.Center).normalize() * 22);
                                }
                            }
                            if (changeCounter > 340)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.Cruise)
                        {
                            NPC.velocity = NPC.velocity.normalize() * (NPC.velocity.Length() + (40 - NPC.velocity.Length()) * 0.2f);

                            NPC.velocity += (target.Center - NPC.Center).normalize() * 0.1f;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).normalize() * NPC.velocity.Length(), 0.07f);
                            NPC.velocity *= 0.998f;
                            changeCounter++;
                            if (changeCounter > 200)
                            {
                                if (Main.rand.NextBool(150) || changeCounter > 500)
                                {
                                    changeAi();
                                }
                            }
                        }
                        if (ai == AIStyle.SplittingVoidStar)
                        {

                            if (changeCounter < 100)
                            {
                                mouthRot -= 4.8f;
                            }
                            else
                            {
                                if (changeCounter < 120)
                                {
                                    mouthRot += 5f;
                                }
                            }
                            changeCounter++;
                            if (changeCounter < 100 && NPC.Distance(target.Center) > 800)
                            {
                                NPC.velocity *= 0.98f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 1f;
                            }
                            else
                            {
                                NPC.velocity *= 0.94f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 0.26f;
                            }
                            if (changeCounter == 100)
                            {
                                if (canShoot)
                                {
                                    for (int i = 0; i < 80; i++)
                                    {
                                        Shoot(ModContent.ProjectileType<VoidStar>(), NPC.Center, NPC.velocity.normalize().RotatedByRandom(2f) * 24 * Main.rand.NextFloat(0.2f, 1f), 0.8f);
                                    }
                                }
                                CEUtils.PlaySound("brimstonevortexshoot", 1, NPC.Center);
                                CEUtils.PlaySound("vbuse", 1, NPC.Center);
                            }
                            if (changeCounter > 140)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.QuickDash)
                        {
                            if (changeCounter == 0)
                            {
                                NPC.rotation = (target.Center - NPC.Center).ToRotation();
                            }
                            changeCounter++;

                            if (changeCounter > 38)
                            {
                                NPC.velocity *= 0.97f;
                                NPC.velocity += (target.Center - NPC.Center).normalize() * 1.4f;
                            }
                            else
                            {
                                NPC.velocity += NPC.rotation.ToRotationVector2() * 3.5f;
                            }
                            if (changeCounter > 100)
                            {
                                changeAi();
                            }
                        }
                        if (ai == AIStyle.VoidLaser)
                        {
                            if (changeCounter % 46 == 0)
                            {
                                NPC.rotation = (target.Center + target.velocity * Main.rand.NextFloat(10, 36) - NPC.Center).ToRotation();
                                NPC.velocity = NPC.rotation.ToRotationVector2();
                                EParticle.NewParticle(new CruiserWarn(), NPC.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, NPC.rotation);
                            }
                            if (changeCounter % 46 == 24)
                            {
                                if (canShoot)
                                {
                                    Shoot(ModContent.ProjectileType<CruiserLaser2>(), NPC.Center, NPC.rotation.ToRotationVector2() * 10, ai0: NPC.whoAmI);
                                }
                                NPC.velocity = NPC.rotation.ToRotationVector2() * ((CEUtils.getDistance(NPC.Center, target.Center) + 1400f) / 22f);
                            }
                            if (changeCounter % 46 == 45)
                            {
                                NPC.velocity = NPC.velocity.normalize();
                            }
                            changeCounter++;
                            if (changeCounter >= 6 * 46)
                            {
                                changeAi();
                            }
                        }
                        if (ai != AIStyle.VoidLaser)
                        {
                            NPC.rotation = NPC.velocity.ToRotation();
                        }
                    }
                    else
                    {
                        notargettime++;
                        NPC.velocity.Y += -1f;
                        if (notargettime > 190)
                        {
                            NPC.active = false;
                        }
                        NPC.rotation = NPC.velocity.ToRotation();
                    }
                }

                if (bite)
                {
                    mouthRot -= 12;
                    if (mouthRot < -48)
                    {
                        bite = false;
                    }
                }
                else
                {
                    mouthRot *= 0.9f;
                }
                if (mouthRot < -48)
                {
                    mouthRot = -48;
                }
                if (phaseTrans > 120)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Particle p = new Particle();
                        p.shape = 4;
                        p.position = NPC.Center - NPC.rotation.ToRotationVector2() * 60;
                        p.alpha = 1.6f * NPC.scale;
                        p.ad = 0.013f;
                        var r = Main.rand;
                        p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                        VoidParticles.particles.Add(p);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Particle p = new Particle();
                        p.shape = 4;
                        p.position = NPC.Center - NPC.rotation.ToRotationVector2() * 60 - NPC.velocity * 0.5f;
                        p.alpha = 1.6f * NPC.scale;
                        p.ad = 0.013f;
                        var r = Main.rand;
                        p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                        VoidParticles.particles.Add(p);
                    }
                }
                if (tjv == 1)
                {
                    tjv = 0;
                    jv = true;
                    if (da < 0)
                    {
                        da = 1;
                    }
                    tail_vj = 12;
                }
                if (jv)
                {
                    da += tail_vj;
                    tail_vj -= 1.5f;
                    if (da < 0)
                    {
                        da = 0;
                        tail_vj = 0;
                        jv = false;
                        jaslowdown = 1;

                        int num = 10;
                        int counts = 3;
                        float speed = 22;
                        if (CalamityWorld.revenge)
                        {
                            num = 10;
                            counts = 3;
                            speed = 25;
                        }
                        if (CalamityWorld.death)
                        {
                            num = 10;
                            counts = 4;
                            speed = 26;
                        }
                        if (Main.expertMode)
                        {
                            num += 1;
                            speed *= 1.1f;
                        }
                        if (Main.masterMode)
                        {
                            num += 1;
                            speed *= 1.15f;
                        }
                        if (ai == AIStyle.AroundPlayerAndShootVoidStar)
                        {
                            counts -= 1;
                            num /= 2;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            {
                                float angle = 0;
                                for (int i = 0; i < counts; i++)
                                {

                                    for (int j = 0; j < num; j++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), bodies[bodies.Count - 1] - (bodies[bodies.Count - 2] - bodies[bodies.Count - 1]).SafeNormalize(Vector2.Zero) * 172 * NPC.scale, angle.ToRotationVector2() * speed, ModContent.ProjectileType<VoidStar>(), (int)(NPC.damage / 7f), 1);
                                        angle += ((float)Math.PI * 2 / (float)num);
                                    }
                                    angle += ((float)Math.PI * 2 / (float)num) / (float)counts;
                                    speed *= 0.7f;
                                }
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), bodies[bodies.Count - 1] - (bodies[bodies.Count - 2] - bodies[bodies.Count - 1]).SafeNormalize(Vector2.Zero) * 172 * NPC.scale, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), (int)(NPC.damage / 7f), 0);
                            }
                            {
                                if (Main.zenithWorld)
                                {
                                    for (int i = 1; i < bodies.Count; i++)
                                    {
                                        for (int _ = 0; _ < Main.rand.Next(-3, 3); _++)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), bodies[i] - (bodies[i - 1] - bodies[i]).SafeNormalize(Vector2.Zero) * 172 * NPC.scale, CEUtils.randomRot().ToRotationVector2() * speed * 3f, ModContent.ProjectileType<VoidStar>(), (int)(NPC.damage / 7f), 1);
                                        }
                                    }
                                }
                            }

                        }
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SoundStyle sound = new SoundStyle("CalamityEntropy/Assets/Sounds/clap");
                            sound.Pitch = 1.4f;
                            SoundEngine.PlaySound(sound);
                            SoundEngine.PlaySound(SoundID.Item9);
                        }

                    }
                }
                else
                {
                    ja = (100f / ((float)NPC.velocity.Length() * 3)) * 5;
                    if (ja < 0)
                    {
                        ja = 0;
                    }

                    da = da + (ja - da) * 0.1f;

                }
            }
            NPC.netSpam = 0;
            NPC.netUpdate = true;
            vtodraw = NPC.Center;
            for (int i = 0; i < bodies.Count; i++)
            {
                Vector2 oPos;
                float oRot;

                if (i == 0)
                {
                    oPos = NPC.Center;
                    oRot = NPC.rotation;
                }
                else
                {
                    oPos = bodies[i - 1];
                    if (i == 1)
                    {
                        oRot = (NPC.Center - bodies[0]).ToRotation();
                    }
                    else
                    {
                        oRot = (bodies[i - 2] - bodies[i - 1]).ToRotation();
                    }
                }
                float rot = (oPos - bodies[i]).ToRotation();
                rot = CEUtils.rotatedToAngle(rot, oRot, 0.12f, false);

                int spacing = 80;
                bodies[i] = oPos - rot.ToRotationVector2() * spacing * NPC.scale;
            }
        }
        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            if (aitype == 3)
            {
                npcHitbox = new Rectangle(0, 0, 0, 0);
                return true;
            }
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (aitype == 3)
            {
                modifiers.SetMaxDamage(0);
                modifiers.FinalDamage *= 0;
                modifiers.DisableSound();
                target.immuneTime = 10;
            }
        }
        public override void OnKill()
        {
            if (!EDownedBosses.downedCruiser)
            {
                if (!BossRushEvent.BossRushActive)
                {
                    VoidOreSystem.BlessWorldWithOre();
                }
            }

            NPC.SetEventFlagCleared(ref EDownedBosses.downedCruiser, -1);

        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPosition, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            if (!candraw && !(phase == 1))
            {
                return false;
            }
            if (noaitime > 0)
            {
                return false;
            }
            if (whiteLerp > 0)
            {
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/WhiteTrans", AssetRequestMode.ImmediateLoad).Value;
                shader.Parameters["strength"].SetValue(whiteLerp);
                Main.spriteBatch.EnterShaderRegion(BlendState.AlphaBlend, shader);
                shader.CurrentTechnique.Passes[0].Apply();
            }
            if (phaseTrans > 120)
            {
                int bd = 0;

                for (int d = 0; d < 9; d++)
                {
                    if (d == 0 || d == 2)
                    {
                        continue;
                    }
                    float rot = 0;
                    if (bd == 0)
                    {
                        rot = (vtodraw - bodies[d]).ToRotation();
                    }
                    else
                    {
                        rot = (bodies[d - 1] - bodies[d]).ToRotation();
                    }
                    Vector2 pos = bodies[d];

                    Texture2D tx;
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/P2b" + (bd + 1).ToString()).Value;

                    spriteBatch.Draw(tx, pos - screenPosition, null, Color.White * alpha, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

                    bd += 1;


                }
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/Head2").Value;
                Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawUp2").Value;
                Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawDown2").Value;
                Vector2 joffset = new Vector2(54, 54);
                Vector2 ofs2 = joffset * new Vector2(1, -1);
                float roth = mouthRot * 0.8f;

                spriteBatch.Draw(j1, vtodraw - screenPosition + joffset.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation + MathHelper.ToRadians(roth), new Vector2(28, 20), NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(j2, vtodraw - screenPosition + ofs2.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation - MathHelper.ToRadians(roth), new Vector2(28, j2.Height - 20), NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(txd, vtodraw - screenPosition, null, Color.White * alpha, NPC.rotation, new Vector2(txd.Width, txd.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

            }
            else
            {
                for (int d = 0; d <= bodies.Count - 1; d++)

                {
                    float rot = 0;
                    if (d == 0)
                    {
                        rot = (vtodraw - bodies[d]).ToRotation();
                    }
                    else
                    {
                        rot = (bodies[d - 1] - bodies[d]).ToRotation();
                    }
                    Vector2 pos = bodies[d];
                    Texture2D f1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/Flagellum").Value;
                    if (d == bodies.Count - 1)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserTail").Value;
                        spriteBatch.Draw(tx, pos - screenPosition, null, Color.White * alpha, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

                    }
                    else
                    {
                        Texture2D tx;
                        if (d % 2 == 1)
                        {
                            tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserBodyAlt").Value;
                        }
                        else
                        {
                            tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserBody").Value;
                        }
                        spriteBatch.Draw(tx, pos - screenPosition, null, Color.White * alpha, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

                    }
                    if (d == bodies.Count - 1 || Main.zenithWorld)
                    {
                        spriteBatch.Draw(f1, pos - screenPosition - new Vector2(36, 0).RotatedBy(rot) * NPC.scale, null, Color.White * alpha, rot + MathHelper.ToRadians(180 - da), new Vector2(0, f1.Height), NPC.scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(f1, pos - screenPosition - new Vector2(36, 0).RotatedBy(rot) * NPC.scale, null, Color.White * alpha, rot + MathHelper.ToRadians(180 + da), new Vector2(0, 0), NPC.scale, SpriteEffects.FlipVertically, 0);

                    }

                }
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserHead").Value;
                Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawUp").Value;
                Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawDown").Value;
                Vector2 joffset = new Vector2(42, 42);
                Vector2 ofs2 = joffset * new Vector2(1, -1);
                float roth = mouthRot;
                spriteBatch.Draw(j1, vtodraw - screenPosition + joffset.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation + MathHelper.ToRadians(roth), new Vector2(58, j2.Height) / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(j2, vtodraw - screenPosition + ofs2.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation - MathHelper.ToRadians(roth), new Vector2(58, j1.Height) / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(txd, vtodraw - screenPosition, null, Color.White * alpha, NPC.rotation, new Vector2(txd.Width, txd.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

            }

            return false;
        }
        public override void PostDraw(SpriteBatch sbb, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.ExitShaderRegion();
            if (phase == 1)
            {
                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Vector2 ddp = SpaceCenter;
                sb.Draw(disTex, ddp - Main.screenPosition, null, Color.DarkBlue * 0.6f, 0, new Vector2(disTex.Width, disTex.Height) / 2, (float)maxDistance / 900f, SpriteEffects.None, 0);
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }


        }


    }
}
