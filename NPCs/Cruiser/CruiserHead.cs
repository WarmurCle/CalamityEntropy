using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using CalamityEntropy.NPCs.Cruiser;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using CalamityEntropy.Projectiles.Cruiser;
using CalamityMod.World;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityEntropy.Buffs;
using CalamityMod.NPCs;
using CalamityEntropy.Items;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityEntropy.Projectiles;
using CalamityMod.Particles;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityEntropy.Items.Accessories;
using Terraria.GameContent.Bestiary;
using System.Diagnostics.Metrics;
using CalamityEntropy.Items.Pets;
using CalamityEntropy.Tiles;
using Terraria.DataStructures;
using CalamityMod.Events;
namespace CalamityEntropy.NPCs.Cruiser
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
        private float lastHurt = 0;
        private int ydmg = 0;
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
        public int maxDistance = 6000;
        public int maxDistanceTarget = 1900;
        public int rotDist = 900;
        public Texture2D disTex = ModContent.Request<Texture2D>("CalamityEntropy/Extra/cruiserSpace").Value;
        public Vector2 rotPos = Vector2.Zero;
        public int phase = 1;
        public int circleDir = 1;
        public float alpha = 1;
        public bool candraw = false;
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkNPC.Add(NPC);
        }
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.7f,
                CustomTexturePath = "CalamityEntropy/Extra/CruiserBes",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
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
            NPC.boss = true;
            NPC.width = 88;
            NPC.height = 88;
            NPC.damage = 160;
            if (Main.expertMode){
                NPC.damage += 16;   
            }if (Main.masterMode){
                NPC.damage += 16;
            }
            NPC.defense = 40;
            NPC.lifeMax = 1100000;
            if (CalamityWorld.death)
            {
                NPC.damage += 22;
                length += 4;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 16;
                length += 2;
            }
            tdamage = NPC.damage;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCHit4;
            NPC.value = 100000f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.95f;
            NPC.dontCountMe = true;
            NPC.scale = 1.1f;
            NPC.netAlways = true;
            NPC.Entropy().damageMul = 0.1f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CruiserBoss");
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CruiserBag>()));

            // Extraneous potions
            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ModContent.ItemType<OmegaHealingPotion>(), 1, 5, 15), hideLootReport: true);


            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                normalOnly.Add(ModContent.ItemType<VoidRelics>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidElytra>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidEcho>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<Silence>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<RuneSong>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidAnnihilate>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<PhantomPlanetKillerEngine>(), new Fraction(3, 5));

                normalOnly.Add(ModContent.ItemType<WingsOfHush>(), new Fraction(3, 5));
                normalOnly.Add(ModContent.ItemType<VoidMonolith>(), 3);
                normalOnly.Add(ModContent.ItemType<VoidToy>(), 3);
                normalOnly.Add(ModContent.ItemType<VoidScales>(), new Fraction(1, 1), 58, 68);
            }
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CruiserRelic>());

            npcLoot.Add(ModContent.ItemType<CruiserTrophy>(), 10);

            
            // Lore
            npcLoot.AddConditionalPerPlayer(() => !EDownedBosses.downedCruiser, ModContent.ItemType<CruiserLore>());
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedMuti);
            writer.Write(targetSpeed);
            writer.Write(slowDownTime);
            writer.Write(nrc);
            writer.Write(aitype);
            writer.Write(changeCounter);
            writer.Write(maxDistanceTarget);
            writer.Write(rotDist);
            writer.WriteVector2(rotPos);
            writer.Write(circleDir);
            writer.Write(flag);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedMuti = reader.ReadSingle();
            targetSpeed = reader.ReadSingle();
            slowDownTime = reader.ReadInt32();
            nrc = reader.ReadInt32();
            aitype = reader.ReadSingle();
            changeCounter = reader.ReadInt32();
            maxDistanceTarget = reader.ReadInt32();
            rotDist = reader.ReadInt32();
            rotPos = reader.ReadVector2();
            circleDir = reader.ReadInt32();
            flag = reader.ReadBoolean();
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {

            modifiers.SourceDamage *= NPC.Entropy().damageMul;
            NPC.Entropy().damageMul *= 0.98f;
            if (phase == 2)
            {
                modifiers.SourceDamage *= 1.5f;
            }
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
            if (phase == 1 && NPC.life < NPC.lifeMax / 2)
            {
                modifiers.SetMaxDamage(1);
            }
            
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            
            modifiers.SourceDamage *= NPC.Entropy().damageMul;
            NPC.Entropy().damageMul *= 0.98f;
            if (phase == 2)
            {
                modifiers.SourceDamage *= 1.5f;
            }
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
            if (phase == 1 && NPC.life < NPC.lifeMax / 2)
            {
                modifiers.SetMaxDamage(1);
            }
        }
        
        public void changeAi()
        {
            if (aitype == 2)
            {
                rotDist = 900;
            }
            circleDir = Main.rand.Next(0, 2);
            if (circleDir == 0)
            {
                circleDir = -1;
            }
            flag = false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return noaitime <= 0 && base.CanHitPlayer(target, ref cooldownSlot);
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
        public override void AI()
        {
            NPC.Entropy().damageMul += 1f / 14000f;
            if (NPC.Entropy().damageMul > 1)
            {
                NPC.Entropy().damageMul = 1;
            }
            counterc++;
            noaitime--;
            if (phase == 1 && NPC.life < NPC.lifeMax / 2)
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
            if (noaitime > 0)
            {
                NPC.life = NPC.lifeMax;
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<VoidBottleThrow>())
                    {
                        NPC.Center = p.Center;
                        NPC.life = NPC.lifeMax;
                        break;
                    }
                }
                speed = 6;
                NPC.target = NPC.FindClosestPlayer();
                if (NPC.HasValidTarget)
                {
                    NPC.rotation = (NPC.target.ToPlayer().Center - NPC.Center).ToRotation();
                }
                return;
            }
            ProgressDraw = ProgressDraw + (((float)NPC.life / (float)NPC.lifeMax) - ProgressDraw) * 0.15f;
            if (ProgressDraw > 0.98f && NPC.life == NPC.lifeMax)
            {
                ProgressDraw = 1;
            }
            if (phase == 2)
            {
                phaseTrans++;
                if (phaseTrans < 120)
                {
                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        NPC.life = NPC.lifeMax / 2 - 1;
                    }
                    Particle p = new Particle();
                    p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                    p.alpha = 0.7f;
                    var r = Main.rand;
                    p.velocity = NPC.rotation.ToRotationVector2() * -3;
                    VoidParticles.particles.Add(p);
                    p = new Particle();
                    p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                    p.alpha = 0.7f;
                    p.velocity = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(70f * (float)Math.Cos(counterc * 0.4f))) * -3;
                    VoidParticles.particles.Add(p);
                    p = new Particle();
                    p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                    p.alpha = 0.7f;
                    p.velocity = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-70f * (float)Math.Cos(counterc * 0.4f))) * -3;
                    VoidParticles.particles.Add(p);
                }
            }
            else if (aitype == 1 || aitype == 2)
            {
                Particle p = new Particle();
                p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                p.alpha = 0.7f;
                var r = Main.rand;
                p.velocity = NPC.rotation.ToRotationVector2() * -3;
                VoidParticles.particles.Add(p);
                p = new Particle();
                p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                p.alpha = 0.7f;
                p.velocity = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(70f * (float)Math.Cos(counterc * 0.4f))) * -3;
                VoidParticles.particles.Add(p);
                p = new Particle();
                p.position = NPC.Center - NPC.rotation.ToRotationVector2() * -14;
                p.alpha = 0.7f;
                p.velocity = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-70f * (float)Math.Cos(counterc * 0.4f))) * -3;
                VoidParticles.particles.Add(p);
            }
            if (phase == 1 && aitype != 2)
            {
                if (NPC.life < NPC.lifeMax / 2)
                {
                    phase = 2;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int ag = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, MathHelper.ToRadians(ag).ToRotationVector2() * 26, ModContent.ProjectileType<VoidSpike>(), NPC.damage / 7, 0);
                            ag += 36;
                        }
                    }
                    NPC.defense = 0;
                    NPC.width = 156;
                    NPC.height = 156;
                    foreach (NPC n in Main.npc)
                    {
                        if (n.realLife == NPC.whoAmI)
                        {
                            n.defense = 4;
                            n.Calamity().DR = 0.1f;
                            if (n.ai[2] <= 8 && n.ai[2] > 4)
                            {
                                n.width = 26;
                                n.height = 26;
                            }
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
            }
            Main.LocalPlayer.Entropy().crSky = 8;
            if (NPC.ai[0] > 10)
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 mp = (NPC.Center + bodies[bodies.Count - 1]) / 2;
                if (aitype == 4)
                {
                    mp = rotPos;
                }
                if (Util.Util.getDistance(mp, player.Center) > maxDistance)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<VoidTouch>(), 5);
                    }
                }
                
            }
            jaslowdown *= 0.8f;
            NPC.netUpdate = true;
            if (slowDownTime > 0)
            {
                speed = speed + (targetSpeed * 0.15f - speed) * 0.07f;
                slowDownTime--;
            }
            else
            {
                speed = speed + (targetSpeed - speed) * 0.07f;
            }
            if (NPC.ai[0] > 180)
            {
                maxDistance = (int)(maxDistance + (maxDistanceTarget - maxDistance) * 0.008f);
            }
            else
            {
                maxDistance = (int)(maxDistance + (maxDistanceTarget - maxDistance) * 0.002f);
            }
            
            NPC.ai[0] += 1;
            int bodyIndex;
            for (int i = 0; i < NPC.buffTime.Length; i++)
            {
                NPC.buffTime[i] = 0;
            }
            if (!b_added)
            {
                b_added = true;
                for (int i = 0; i < length + 1; i++)
                {
                    bodies.Add(NPC.Center - new Vector2(0, 0));
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 1)
                {
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
            if (phaseTrans > 120)
            {
                for (int i = 0; i < 4; i++)
                {
                    Particle p = new Particle();
                    p.shape = 4;
                    p.position = NPC.Center - NPC.rotation.ToRotationVector2() * 60;
                    p.alpha = 1.6f;
                    p.ad = 0.013f;
                    var r = Main.rand;
                    p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                    VoidParticles.particles.Add(p);
                }
                for (int i = 0; i < 4; i++)
                {
                    Particle p = new Particle();
                    p.shape = 4;
                    p.position = NPC.Center - NPC.rotation.ToRotationVector2() * 60 - NPC.velocity * 0.5f; ;
                    p.alpha = 1.6f;
                    p.ad = 0.013f;
                    var r = Main.rand;
                    p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                    VoidParticles.particles.Add(p);
                }
            }
            int closestPlayerIndexd = NPC.FindClosestPlayer();
            NPC.target = closestPlayerIndexd;
            int targetPlayerIndex = NPC.target;
            Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.5f, 0.5f, 1f);
            if (NPC.HasValidTarget)
            {
                if (Util.Util.getDistance(NPC.Center, NPC.target.ToPlayer().Center) < 210)
                {
                    if (mouthRot >= -0.1f)
                    {
                        bite = true;
                    }
                }
                Player targetPlayerr = Main.player[targetPlayerIndex];
                if (aitype == 0)
                {
                    maxDistanceTarget = 1850;
                    if (NPC.ai[1] == 1)
                    {
                        if (phase == 1)
                        {
                            NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.028f, false);
                            NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 1.25f);
                        }
                        else
                        {
                            NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.025f, false);
                            NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.7f);
                        }
                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                        targetSpeed = 18;
                        nrc++;
                        if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 200 || (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 380 && phase == 2) || nrc > 160)
                        {
                            NPC.ai[1] = 0;
                            nrc = 0;
                        }
                        if (changeCounter > 2)
                        {
                            aitype = Main.rand.Next(0, 3);
                            changeAi();
                            changeCounter = 0;
                            nrc = 0;
                        }
                        
                    }
                    if (NPC.ai[1] == 0)
                    {

                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                        targetSpeed = 30;
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.2f);
                        nrc++;
                        if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) > 1400 || nrc > 100)
                        {
                            NPC.ai[1] = 1;
                            changeCounter++;
                            nrc = 0;
                            slowDownTime = 60;
                            tjv = 1;
                        }
                    }
                }

                else if (aitype == 1)
                {
                    maxDistanceTarget = 1400;
                    changeCounter++;
                    targetSpeed = 70;
                    Vector2 targetPos = targetPlayerr.Center + ((NPC.Center - targetPlayerr.Center).SafeNormalize(new Vector2(1, 0)) * 800).RotatedBy(MathHelper.ToRadians(20 * circleDir));
                    NPC.rotation = (targetPos - NPC.Center).ToRotation();
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                    if (tjv == 0 && !jv && jaslowdown < 0.01f)
                    {
                        tjv = 1;
                    }
                    if (changeCounter > 250)
                    {
                        changeCounter = 0;
                        aitype = 0;
                        changeAi();
                    }
                }
                else if (aitype == 2)
                {
                    Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                    if (changeCounter > 10) { maxDistanceTarget = rotDist + 400; }
                    changeCounter++;
                    if (changeCounter == 1)
                    {
                        targetSpeed = 10;
                        rotDist = 1000;
                        rotPos = targetPlayerr.Center + new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(-300, 301));
                    }
                    if (changeCounter == 60)
                    {
                        targetSpeed = 50;
                    }
                    if (changeCounter == 200)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruiserLaserMouth>(), (int)(NPC.damage / 7f), 0, -1, NPC.whoAmI);
                        }
                    }
                    if (changeCounter > 260)
                    {
                        rotDist -= 1;
                    }
                    Vector2 targetPosd = rotPos + ((NPC.Center - rotPos).SafeNormalize(new Vector2(1, 0)) * rotDist).RotatedBy(MathHelper.ToRadians(20 * circleDir));
                    NPC.rotation = (targetPosd - NPC.Center).ToRotation();
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                    if (tjv == 0 && !jv && jaslowdown < 0.007f && changeCounter < 800)
                    {
                        tjv = 1;
                    }
                    if (changeCounter > 950)
                    {
                        rotDist += 4;
                        maxDistanceTarget = 2000;
                        targetSpeed = 20;
                    }
                    if (changeCounter > 1200)
                    {
                        changeCounter = 0;
                        aitype = 0;
                        changeAi();
                    }

                }
                if (aitype == 3)
                {
                    maxDistanceTarget = 5000;
                    if (flag)
                    {
                        speed = 60;
                        targetSpeed = 60;
                        changeCounter++;
                        if (!Util.Util.isAir(NPC.Center + NPC.rotation.ToRotationVector2() * 240) && changeCounter < 40)
                        {
                            changeCounter = 40;
                        }
                        if (changeCounter > 150)
                        {
                            aitype = -1;
                            changeCounter = 0;
                            changeAi();
                        }
                        if (changeCounter < 40)
                        {
                            if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 200) {
                                targetPlayerr.velocity = Vector2.Zero;
                                targetPlayerr.Center = NPC.Center + NPC.rotation.ToRotationVector2() * 120;
                                targetPlayerr.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), 1800f, 1000f, 0f, 4.5f) * 4;
                                mouthRot = -40;
                                for (int i = 0; i < 10; i++)
                                {
                                    Particle p = new Particle();
                                    p.position = NPC.Center;
                                    p.alpha = 1.4f;

                                    var r = Main.rand;
                                    p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * 16), (float)((r.NextDouble() - 0.5) * 16));
                                    VoidParticles.particles.Add(p);
                                }
                            }
                        }
                        else
                        {
                            if (changeCounter == 40)
                            {
                                if (!(Main.netMode == NetmodeID.MultiplayerClient))
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), targetPlayerr.Center, Vector2.Zero, ModContent.ProjectileType<CruiserSlash>(), (int)(NPC.damage / 6f), 0);
                                }
                            }
                            if (changeCounter < 60)
                            {
                                speed = -14;
                                targetSpeed = speed;
                            }
                            else
                            {
                                targetSpeed = 16;
                                NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, ((targetPlayerr.Center + (NPC.Center - targetPlayerr.Center).SafeNormalize(Vector2.One) * 1500) - NPC.Center).ToRotation(), 0.08f, false);
                            }
                        }
                    }
                    else
                    {
                        targetSpeed = 60;
                        NPC.rotation = (targetPlayerr.Center - NPC.Center).ToRotation();
                        if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 100)
                        {
                            flag = true;
                        }
                    }
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                }
                if (aitype == -1)
                {
                    maxDistanceTarget = 4000;
                    if (changeCounter == 20)
                    {

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int ag = Main.rand.Next(0, 360);
                            for (int i = 0; i < 18; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, MathHelper.ToRadians(ag).ToRotationVector2() * 20, ModContent.ProjectileType<VoidSpike>(), NPC.damage / 7, 0);
                                ag += 20;
                            }
                        }
                    }
                    changeCounter++;
                    targetSpeed = 16;
                    Vector2 targetPos = targetPlayerr.Center + ((NPC.Center - targetPlayerr.Center).SafeNormalize(new Vector2(1, 0)) * 800).RotatedBy(MathHelper.ToRadians(20 * circleDir));
                    NPC.rotation = (targetPos - NPC.Center).ToRotation();
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                    if (changeCounter > 40)
                    {
                        changeCounter = 0;
                        aitype = Main.rand.Next(3, 8);
                        changeAi();
                    }

                }
                if (aitype == 4)
                {
                    if (changeCounter > 10) { maxDistanceTarget = rotDist + 400; }
                    changeCounter++;
                    targetSpeed = 10;
                    rotDist = 1800;
                    maxDistanceTarget = 800;
                    if (changeCounter > 100)
                    {
                        maxDistanceTarget = 600;
                    }
                    else
                    {
                        maxDistanceTarget = 4000;
                    }
                    if (changeCounter > 380)
                    {
                        maxDistanceTarget = 8000;
                    }
                    if (changeCounter == 1)
                    {
                        rotPos = NPC.Center;
                        
                        bite = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruiserBlackhole>(), (int)(NPC.damage / 9f), 0);
                        }
                    }
                    
                    if (changeCounter > 450)
                    {
                        changeCounter = 0;
                        aitype = -1;
                        changeAi();
                    }
                    if (changeCounter > 400)
                    {
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.5f, true);
                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                    }
                    else
                    {
                        Vector2 targetPosd = rotPos + ((NPC.Center - rotPos).SafeNormalize(new Vector2(1, 0)) * rotDist).RotatedBy(MathHelper.ToRadians(20 * circleDir));
                        NPC.rotation = (targetPosd - NPC.Center).ToRotation();
                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed;
                    }
                }
                if (aitype == 5)
                {
                    maxDistanceTarget = 1850;
                    if (NPC.ai[1] == 1)
                    {
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.01f, false);
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.5f);
                        
                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                        targetSpeed = 18;
                        nrc++;
                        if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 240 || (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) < 380 && phase == 2) || nrc > 160)
                        {
                            NPC.ai[1] = 0;
                            nrc = 0;
                        }
                        if (changeCounter > 0)
                        {
                            nrc = 0;
                            aitype = -1;
                            changeAi();
                            changeCounter = 0;
                        }

                    }
                    if (NPC.ai[1] == 0)
                    {

                        NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                        targetSpeed = 30;
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 1.7f);
                        nrc++;
                        if (Util.Util.getDistance(NPC.Center, targetPlayerr.Center) > 1400 || nrc > 100)
                        {
                            
                            NPC.ai[1] = 1;
                            changeCounter++;
                        }
                    }
                }

                if (aitype == 6)
                {
                    maxDistanceTarget = 6500;
                    changeCounter++;
                    if (changeCounter == 80 && changeCounter == 200)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Sounds/cr_dash"), NPC.Center);
                    }
                    if (changeCounter > 200)
                    {
                        maxDistanceTarget = 6000;
                        targetSpeed = 80;
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 0.2f);
                    }
                    else if (changeCounter > 140)
                    {
                        maxDistanceTarget = 6000;
                        targetSpeed = 6;
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 5f);
                    }
                    else
                    {
                        if (changeCounter > 80)
                        {
                            targetSpeed = 100;
                        }
                        else
                        {
                            targetSpeed = 0;
                            NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center - NPC.Center).ToRotation(), 1.7f);
                        }
                    }
                    if (changeCounter == 100)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int ag = Main.rand.Next(0, 360);
                            for (int i = 0; i < 10; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, MathHelper.ToRadians(ag).ToRotationVector2() * 26, ModContent.ProjectileType<VoidSpike>(), NPC.damage / 7, 0);
                                ag += 36;
                            }
                        }
                    }
                    if (changeCounter > 270)
                    {
                        aitype = -1;
                        changeAi();
                        changeCounter = 0;
                    }
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                }
                if (aitype == 7)
                {
                    targetSpeed = 0;
                    NPC.velocity = NPC.rotation.ToRotationVector2() * speed * speedMuti;
                    changeCounter++;
                    if (changeCounter == 70 || changeCounter == 210)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Sounds/void_laser"), NPC.Center);
                    }
                    if (changeCounter == 80 || changeCounter == 220)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruiserLaser2>(), NPC.damage / 7, 0, -1, NPC.whoAmI);
                        }
                    }
                    if (changeCounter < 60)
                    {
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center + targetPlayerr.velocity * 15 - NPC.Center).ToRotation(), 1.7f);
                    }
                    if (changeCounter > 140 && changeCounter < 200)
                    {
                        NPC.rotation = Util.Util.rotatedToAngle(NPC.rotation, (targetPlayerr.Center + targetPlayerr.velocity * 15 - NPC.Center).ToRotation(), 1.7f);
                    }
                    if (changeCounter > 260)
                    {
                        aitype = -1;
                        changeAi();
                        changeCounter = 0;
                    }
                    
                }
                if (phase == 2)
                {
                    if (phaseTrans < 120)
                    {
                        aitype = -1;
                    }
                    else if (aitype < 2 && aitype > -1)
                    {
                        
                        aitype = Main.rand.Next(3, 3);
                         
                        changeCounter = 0;
                        changeAi();
                    }
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
            Lighting.AddLight(NPC.Center, 1f, 1f, 1f);
            Lighting.AddLight(NPC.Center + NPC.velocity, 1f, 1f, 1f);
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
                    if (aitype == 1)
                    { num /= 3; speed *= 0.65f; }
                    if (aitype == 2)
                    { num /= 4; speed *= 0.5f; counts -= 1; }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
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
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundStyle sound = new SoundStyle("CalamityEntropy/Sounds/clap");
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
            if (aitype == 3 || aitype == 4) {
                if (alpha > 0.3)
                {
                    alpha -= 0.05f;
                }
            }
            else
            {
                if (alpha < 1)
                {
                    alpha += 0.05f;
                }
            }
            NPC.netUpdate = true;
            NPC.Center = NPC.Hitbox.Center.ToVector2();
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
                if (NPC.ai[0] > 120)
                {
                    rot = Util.Util.rotatedToAngle(rot, oRot, 0.12f, false);
                }
                int spacing = 54;
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
            if (NPC.ai[0] <= 1)
            {
                return false;
            }
            if (phaseTrans > 120)
            {
                int bd = 0;
                if (aitype == 6)
                {
                    if (changeCounter < 80)
                    {
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(166, 111, 255) * ((float)changeCounter / 80f) * 0.7f, 2);

                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(166, 111, 255) * ((float)changeCounter / 80f) * 0.3f, 6);
                    
                    }
                    if (changeCounter > 140 && changeCounter < 200)
                    {
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(166, 111, 255) * ((float)(changeCounter - 140) / 60f) * 0.7f, 2);
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(166, 111, 255) * ((float)(changeCounter - 140) / 60f) * 0.3f, 6);


                    }
                }
                if (aitype == 7)
                {
                    if (changeCounter < 60)
                    {
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(200, 180, 255) * ((float)(changeCounter) / 60f) * 0.7f, 2);
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(200, 180, 255) * ((float)(changeCounter) / 60f) * 0.3f, 6);

                    }
                    if (changeCounter > 140 && changeCounter < 200)
                    {
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(200, 180, 255) * ((float)(changeCounter - 140) / 60f) * 0.7f, 2);
                        Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, NPC.Center, NPC.Center + NPC.rotation.ToRotationVector2() * 6000, new Color(200, 180, 255) * ((float)(changeCounter - 140) / 60f) * 0.3f, 6);

                    }
                }
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
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/P2b" + (bd + 1).ToString()).Value;
                        
                    spriteBatch.Draw(tx, pos - screenPosition, null, Color.White * alpha, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

                    bd += 1;


                }
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/Head2").Value;
                Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserJawUp2").Value;
                Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserJawDown2").Value;
                Vector2 joffset = new Vector2(60, 62);
                Vector2 ofs2 = joffset * new Vector2(1, -1);
                float roth = mouthRot * 0.8f;

                spriteBatch.Draw(j1, vtodraw - screenPosition + joffset.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation + MathHelper.ToRadians(roth), new Vector2(40, 28), NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(j2, vtodraw - screenPosition + ofs2.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White * alpha, NPC.rotation - MathHelper.ToRadians(roth), new Vector2(40, j2.Height - 28), NPC.scale, SpriteEffects.None, 0);

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
                    if (d == bodies.Count - 1)
                    {
                        Texture2D f1 = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/Flagellum").Value;
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserTail").Value;
                        spriteBatch.Draw(tx, pos - screenPosition, null, Color.White, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(f1, pos - screenPosition - new Vector2(36, 0).RotatedBy(rot), null, Color.White, rot + MathHelper.ToRadians(180 - da), new Vector2(0, f1.Height), NPC.scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(f1, pos - screenPosition - new Vector2(36, 0).RotatedBy(rot), null, Color.White, rot + MathHelper.ToRadians(180 + da), new Vector2(0, 0), NPC.scale, SpriteEffects.FlipVertically, 0);

                    }
                    else
                    {
                        Texture2D tx;
                        if (d % 2 == 1)
                        {
                            tx = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserTail").Value;
                        }
                        else
                        {
                            tx = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserBody").Value;
                        }
                        spriteBatch.Draw(tx, pos - screenPosition, null, Color.White, rot, new Vector2(tx.Width, tx.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

                    }


                    

                }
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserHead").Value;
                Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserJawUp").Value;
                Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/NPCs/Cruiser/CruiserJawDown").Value;
                Vector2 joffset = new Vector2(20, 26);
                Vector2 ofs2 = joffset * new Vector2(1, -1);
                float roth = mouthRot;
                spriteBatch.Draw(j1, vtodraw - screenPosition + joffset.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White, NPC.rotation + MathHelper.ToRadians(roth), new Vector2(j2.Width, j2.Height) / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(j2, vtodraw - screenPosition + ofs2.RotatedBy(NPC.rotation) * NPC.scale, null, Color.White, NPC.rotation - MathHelper.ToRadians(roth), new Vector2(j1.Width, j1.Height) / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(txd, vtodraw - screenPosition, null, Color.White, NPC.rotation, new Vector2(txd.Width, txd.Height) / 2, NPC.scale, SpriteEffects.None, 0f);

            }
            
            return false;
        }
        public override void PostDraw(SpriteBatch sbb, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] > 1)
            {
                if (phase == 1)
                {
                    SpriteBatch sb = Main.spriteBatch;
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    Vector2 ddp = (NPC.Center + bodies[bodies.Count - 1]) / 2;
                    if (aitype == 4)
                    {
                        ddp = rotPos;
                    }
                    sb.Draw(disTex, ddp - Main.screenPosition, null, Color.DarkBlue * 0.6f, 0, new Vector2(disTex.Width, disTex.Height) / 2, (float)maxDistance / 900f, SpriteEffects.None, 0);
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }

            }
        }


    }
}
