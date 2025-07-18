using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Acropolis
{
    [AutoloadBossHead]
    public class AcropolisMachine : ModNPC
    {
        public class AcropolisLeg
        {
            public Vector2 StandPoint = Vector2.Zero;
            public float Scale = 1f;
            public NPC NPC;
            public Vector2 offset;
            public int NoMoveTime = 0;
            public Vector2 targetPos;
            
            public AcropolisLeg(NPC npc, Vector2 offset, float scale = 1)
            {
                NPC = npc;
                this.offset = offset;
                this.Scale = scale;
                StandPoint = npc.Center;
            }
            bool o = false;
            public bool OnTile => !CEUtils.isAir(StandPoint, true) && o;
            public bool Update()
            {
                if (CEUtils.getDistance(StandPoint, targetPos) < ms * (NPC.velocity.Y > 1f ? 3 : 1))
                {
                    StandPoint = targetPos;
                }
                else
                {
                    StandPoint += (targetPos - StandPoint).normalize() * ms * (NPC.velocity.Y > 0.5f ? 3 : 1);
                }
                NoMoveTime--;
                float distToMove = 100;
                if (((AcropolisMachine)NPC.ModNPC).Jumping)
                {
                    targetPos = NPC.Center + new Vector2(offset.X * 0.2f, 200);
                    ms = CEUtils.getDistance(targetPos, StandPoint) * 0.2f;
                    return false;
                }
                if (!OnTile || (NoMoveTime <= 0 && CEUtils.getDistance(StandPoint, NPC.Center + NPC.velocity * 16 + offset) > distToMove))
                {
                    targetPos = FindStandPoint(NPC.Center + NPC.velocity * 16 + offset + new Vector2(Math.Sign(NPC.velocity.X) == Math.Sign(offset.X) ? (Math.Sign(NPC.velocity.X) * 28) : 0, 0), 50 * Scale, 40);
                    ms = CEUtils.getDistance(targetPos, StandPoint) * 0.2f;
                    if (NoMoveTime < 4)
                        NoMoveTime = 4;
                    return true;
                }
                
                return false;
            }
            public float ms;
            public Vector2 FindStandPoint(Vector2 center, float MaxOffset, float MaxTry = 64)
            {
                o = false;
                for(int i = 0; i < MaxTry; i++)
                {
                    Vector2 pos = CEUtils.randomPointInCircle(MaxTry) * new Vector2(0.6f, 1f) + center;
                    if(CanStandOn(pos))
                    {
                        o = true;
                        Vector2 orgPos = pos;
                        int c = 52;
                        while(CanStandOn(pos))
                        {
                            c--;
                            pos.Y -= 2;
                            if(c <= 0)
                            {
                                return orgPos;
                            }
                        }
                        pos.Y += 2;
                        return pos;
                    }
                }
                return NPC.Center + new Vector2(offset.X, 200);
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
                CustomTexturePath = "CalamityEntropy/Assets/BCL/AcropolisMachine",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = -4
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.Acropolis")
            });
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 142;
            NPC.height = 132;
            NPC.damage = 30;
            NPC.defense = 12;
            NPC.lifeMax = 5400;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CEUtils.GetSound("chainsaw_break");
            NPC.value = 1600f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.timeLeft *= 10;
            if (!Main.dedServ)
            {
                Music = MusicID.OtherworldlyBoss1;
            }
        }
        public static bool CanStandOn(Vector2 pos)
        {
            return !CEUtils.isAir(pos, true);
        }
        public bool CanStandOn(int x, int y)
        {
            if(!CEUtils.inWorld(x, y)) return false;
            return CanStandOn(new Vector2(x, y) * 16f);
        }
        public List<AcropolisLeg> legs = null;
        public override void AI()
        {
            JumpCD--;
            if (legs == null)
            {
                legs =
                [
                    new AcropolisLeg(NPC, new Vector2(-70, 120), 0.8f),
                    new AcropolisLeg(NPC, new Vector2(70, 120), 0.8f),
                    new AcropolisLeg(NPC, new Vector2(-130, 120), 1),
                    new AcropolisLeg(NPC, new Vector2(130, 120), 1),
                ];
            }
            foreach (var l in legs)
            {
                if (l.Update())
                {
                    foreach (var l2 in legs)
                    {
                        if (Math.Sign(l2.offset.X) == Math.Sign(l.offset.X))
                        {
                            if (l2.NoMoveTime < 8)
                            {
                                l2.NoMoveTime = 8;
                            }
                        }
                    }
                }
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
                NPC.velocity.X += 0.2f;
                if (CEUtils.CheckSolidTile(NPC.getRect()))
                {
                    NPC.velocity.Y -= 0.4f;
                }
            }
            if (Jumping)
            {
                NPC.velocity.Y += 0.4f;
            }
            else
            {
                NPC.velocity *= 0.97f;
            }
            if(Jumping)
            {
                NPC.rotation = (Math.Abs(NPC.velocity.X * 0.04f).ToRotationVector2() * new Vector2(dir, 1)).ToRotation();
            }
            else
            {
                NPC.rotation = dir == 1 ? 0 : MathHelper.Pi;
            }
        }
        public int JumpCD = 0;
        public bool Jumping = false;
        public void AttackPlayer(Player player)
        {
            float enrange = 1;
            if (Main.expertMode)
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
            if (!Jumping)
            {
                bool flag = false;
                int c = 0;
                foreach (var l in legs)
                {
                    if (l.OnTile)
                    {
                        c++;
                    }
                }
                if (c > 2)
                {
                    flag = true;
                }
                if ((flag || CEUtils.CheckSolidTile(NPC.getRect())))
                {
                    if (player.Center.Y + 200 < NPC.Center.Y)
                    {
                        if (JumpCD <= 0)
                        {
                            Jumping = true;
                            NPC.velocity = new Vector2(0.01f * (player.Center.X - NPC.Center.X), float.Max((player.Center.Y - NPC.Center.Y) * 0.08f, -30));
                            JumpCD = 160;
                        }
                    }
                    float yof = -90;
                    if (NPC.Center.Y - yof + 90 > player.Center.Y)
                    {
                        if (NPC.velocity.Y > 4)
                        {
                            NPC.velocity.Y = 4;
                        }
                        if (NPC.velocity.Y > 0)
                            NPC.velocity.Y *= 0.8f;
                    }
                    float v = 0.2f;
                    if (Math.Abs(NPC.Center.Y + yof - player.Center.Y) > 150)
                    {
                        v = 1;
                    }
                    if (Math.Abs(NPC.Center.Y + yof - player.Center.Y) < 14)
                    {
                        v = 0;
                        NPC.velocity.Y *= 0.8f;
                    }
                    if (Math.Abs(yof + player.Center.Y - NPC.Center.Y) > 20)
                    {
                        if (player.Center.Y + yof > NPC.Center.Y)
                        {
                            NPC.velocity.Y += 0.4f * enrange * v;
                        }
                        else
                        {
                            bool f = true;
                            foreach (var l in legs)
                            {
                                if (l.OnTile && l.StandPoint.Y > NPC.Center.Y + 120)
                                {
                                    f = false;
                                    break;
                                }
                            }
                            if (f || CEUtils.CheckSolidTile(NPC.getRect()))
                                NPC.velocity.Y -= 0.4f * enrange * v;
                            else
                                NPC.velocity.Y += 4f * enrange * v;
                        }
                    }
                }
                else
                {
                    NPC.velocity.Y += 0.42f;
                    if (NPC.velocity.Y > 12)
                        NPC.velocity.Y = 12;
                }
                if (CEUtils.getDistance(NPC.Center, player.Center) > 300)
                {
                    NPC.velocity.X += Math.Sign(player.Center.X - NPC.Center.X) * 0.1f * enrange;
                }
                
            }
            else
            {
                bool flag = false;
                int c = 0;
                foreach (var l in legs)
                {
                    if (l.OnTile)
                    {
                        c++;
                    }
                }
                if (c > 2)
                {
                    flag = true;
                }
                if (NPC.Center.Y < player.Center.Y && flag)
                {
                    Jumping = false;
                    NPC.velocity *= 0;
                }
                if(JumpCD < 20 || (NPC.velocity.Y > 0 && CEUtils.CheckSolidTile(NPC.getRect())) || NPC.velocity.Y > 4)
                {
                    Jumping = false;
                    NPC.velocity *= 0;
                }
            }
            if(NPC.velocity.X > 0)
            {
                dir = 1;
            }
            if(NPC.velocity.X < 0)
            {
                dir = -1;
            }
        }
        public void Shoot<T>(Vector2 pos, Vector2 velocity, float damageMult = 1, float ai0 = 0, float ai1 = 0, float ai2 = 0) where T : ModProjectile
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int baseDamage = (int)(NPC.damage / 6.2f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<T>(), (int)(baseDamage * damageMult), 4, -1, ai0, ai1, ai2);
            }
        }
        public int phase = 1;
        public int dir = 1;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D body = NPC.getTexture();
            /*if (legs != null)
            {
                foreach (var l in legs)
                {
                    CEUtils.DrawGlow(l.StandPoint, Color.White, 0.2f);
                    CEUtils.DrawGlow(l.StandPoint, Color.White, 0.2f);

                    CEUtils.DrawGlow(l.offset + NPC.Center, Color.Blue, 0.2f, false);
                    CEUtils.DrawGlow(l.offset + NPC.Center, Color.Blue, 0.2f, false);
                }
            }*/
            Texture2D t1 = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Acropolis/Leg1");
            Texture2D t2 = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Acropolis/Leg2");
            Texture2D t3 = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Acropolis/Foot");
            Main.EntitySpriteDraw(body, NPC.Center - screenPos, null, drawColor, NPC.rotation, body.Size() / 2f, NPC.scale, dir < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            if (legs == null)
                return false;
            foreach (var leg in legs)
            {
                float l1 = 46 * NPC.scale * leg.Scale;
                float l2 = 70 * NPC.scale * leg.Scale;
                float l3 = 72 * NPC.scale * leg.Scale;
                List<Vector2> points = new List<Vector2>();
                points.Add(NPC.Center + new Vector2(Math.Sign(leg.offset.X) * 20, 60).RotatedBy(dir > 0 ? NPC.rotation : -MathHelper.Pi + NPC.rotation));
                Vector2 e = CalculateLegJoints(points[0], leg.StandPoint, l1, l2, l3, out var p1, out var p2);
                points.Add(p1);
                points.Add(CEUtils.GetCircleIntersection(p1, l2, leg.StandPoint, l3));
                points.Add(points[points.Count - 1] + (leg.StandPoint - points[points.Count - 1]).normalize() * l3);
                Main.EntitySpriteDraw(t1, points[0] - Main.screenPosition, null, drawColor, (points[1] - points[0]).ToRotation(), new Vector2(4, 13), NPC.scale * leg.Scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t2, points[1] - Main.screenPosition, null, drawColor, (points[2] - points[1]).ToRotation(), new Vector2(6, 9), NPC.scale * leg.Scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t3, points[2] - Main.screenPosition, null, drawColor, (points[3] - points[2]).ToRotation() + ((leg.offset.X > 0 ? 1 : -1) * MathHelper.ToRadians(24)), new Vector2(27, t3.Height / 2f), NPC.scale * leg.Scale, leg.offset.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                //CEUtils.DrawLines(points, Color.Blue, 4);
            }
            
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if(NPC.life <= 0)
            {
                for(int i = 0; i < 40; i++)
                {
                    EParticle.NewParticle(new EMediumSmoke(), NPC.Center + CEUtils.randomPointInCircle(60), CEUtils.randomPointInCircle(32), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(1f, 4f), 1, true, BlendState.AlphaBlend, CEUtils.randomRot(), 120);
                }
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore0").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore1").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore2").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore3").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore4").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore4").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore4").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore4").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore5").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore5").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore5").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore5").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore6").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore7").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore7").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore7").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore7").Type); 
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore8").Type);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + CEUtils.randomPointInCircle(46), CEUtils.randomPointInCircle(16), Mod.Find<ModGore>("AcrGore9").Type);
            }
        }
        public Vector2 CalculateLegJoints(Vector2 Center, Vector2 legStandPoint, float l1, float l2, float l3, out Vector2 P1, out Vector2 P2)
        {
            // 初始化输出
            P1 = Vector2.Zero;
            P2 = Vector2.Zero;

            // 输入验证
            if (l1 <= 0 || l2 <= 0 || l3 <= 0)
            {
                throw new ArgumentException("Leg segment lengths must be positive.");
            }

            // 计算目标点相对于根部的向量和距离
            Vector2 D = legStandPoint - Center;
            float dist = D.Length();

            // 处理不可达情况
            Vector2 target = legStandPoint;
            if (dist > l1 + l2 + l3)
            {
                target = Center + Vector2.Normalize(D) * (l1 + l2 + l3);
            }

            // 计算第一节方向（朝下但偏向目标点）
            Vector2 downDirection = new Vector2(0, 1); // 初始朝下
            Vector2 targetDirection = D.Length() > 0 ? Vector2.Normalize(D) : downDirection;

            // 计算偏转角度（限制最大偏转角）
            float maxDeflectionAngle = MathHelper.ToRadians(68);
            float angleToTarget = (float)Math.Atan2(targetDirection.Y, targetDirection.X) - (float)Math.PI / 2; // 相对于Y轴正方向
            float deflectionAngle = MathHelper.Clamp(angleToTarget, -maxDeflectionAngle, maxDeflectionAngle);

            // 旋转第一节方向
            float cosAngle = (float)Math.Cos(deflectionAngle);
            float sinAngle = (float)Math.Sin(deflectionAngle);
            Vector2 firstSegmentDirection = new Vector2(
                downDirection.X * cosAngle - downDirection.Y * sinAngle,
                downDirection.X * sinAngle + downDirection.Y * cosAngle
            );

            // 计算 P1
            P1 = Center + l1 * firstSegmentDirection;

            // 计算 P2
            // 第三节朝下，P2.Y = target.Y - l3
            float y2 = target.Y - l3;
            // 第二节长度约束：|P2 - P1| = l2
            float deltaY = y2 - P1.Y;
            float deltaX;
            try
            {
                deltaX = (float)Math.Sqrt(l2 * l2 - deltaY * deltaY);
            }
            catch
            {
                // 如果无法满足长度约束，说明目标点不可达，调整P2到最近可达点
                deltaX = 0;
                y2 = P1.Y - l2; // 第二节完全朝上
            }

            // 选择与目标点X坐标更接近的解
            float x2_positive = P1.X + deltaX;
            float x2_negative = P1.X - deltaX;
            float x2 = (Math.Abs(x2_positive - target.X) < Math.Abs(x2_negative - target.X)) ? x2_positive : x2_negative;

            P2 = new Vector2(x2, y2);

            // 验证第三节长度约束
            float distP2ToTarget = Vector2.Distance(P2, target);
            if (Math.Abs(distP2ToTarget - l3) > 0.001f)
            {
                // 如果第三节长度不满足，调整P2（第二节完全朝上，第三节朝下）
                P2 = new Vector2(P1.X, P1.Y - l2);
                target = new Vector2(P2.X, P2.Y + l3);
            }

            return target;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref EDownedBosses.downedLuminaris, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<LuminarisBag>()));

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.HealingPotion, 1, 5, 15), hideLootReport: true);


            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
            }
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<LuminarisRelic>());

            npcLoot.Add(ModContent.ItemType<LuminarisTrophy>(), 10);

            npcLoot.AddConditionalPerPlayer(() => !EDownedBosses.downedLuminaris, ModContent.ItemType<LuminarisLore>());
        }

    }
}
