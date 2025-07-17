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
            public AcropolisLeg(NPC npc, Vector2 offset, float scale = 1)
            {
                NPC = npc;
                this.offset = offset;
                this.Scale = scale;
                StandPoint = npc.Center;
            }
            public bool OnTile => CEUtils.CheckSolidTile(StandPoint.getRectCentered(16, 16));
            public bool Update()
            {
                NoMoveTime--;
                float distToMove = 140;
                if(NoMoveTime <= 0)
                {
                    if(CEUtils.getDistance(StandPoint, offset) > distToMove)
                    {
                        StandPoint = FindStandPoint(NPC.Center + offset, 96 * Scale);
                        if (NoMoveTime < 10)
                            NoMoveTime = 10;
                        return true;
                    }
                }
                return false;
            }

            public Vector2 FindStandPoint(Vector2 center, float MaxOffset, float MaxTry = 42)
            {
                for(int i = 0; i < MaxTry; i++)
                {
                    Vector2 pos = CEUtils.randomPointInCircle(MaxTry) + center;
                    if(CanStandOn(pos))
                    {
                        return pos;
                    }
                }
                return NPC.Center + new Vector2(offset.X, 500);
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
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath22;
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
            foreach (var l in legs)
            {
                if(l.Update())
                {
                    foreach(var l2 in legs)
                    {
                        if(Math.Sign(l2.offset.X) == Math.Sign(l.offset.Y))
                        {
                            if(l2.NoMoveTime < 20)
                            {
                                l2.NoMoveTime = 20;
                            }
                        }
                    }
                }
            }
            if (legs == null)
            {
                legs = new List<AcropolisLeg>();
                legs.Add(new AcropolisLeg(NPC, new Vector2(-100, -60), 1));
                legs.Add(new AcropolisLeg(NPC, new Vector2(100, -60), 1));
                legs.Add(new AcropolisLeg(NPC, new Vector2(-80, -60), 0.8f));
                legs.Add(new AcropolisLeg(NPC, new Vector2(80, -60), 0.8f));
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
                    NPC.velocity.Y -= 0.3f;
                }
            }
            NPC.velocity *= 0.98f;
            
        }

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
            bool flag = false;
            int c = 0;
            foreach(var l in legs)
            {
                if(l.OnTile)
                {
                    c++;
                }
            }
            if(c > 3)
            {
                flag = true;
            }
            if ((flag || CEUtils.CheckSolidTile(NPC.getRect())))
            {
                float v = 0.2f;
                if(Math.Abs(NPC.Center.Y - player.Center.Y) > 120)
                {
                    v = 1;
                }
                if(player.Center.Y > NPC.Center.Y)
                {
                    NPC.velocity.Y += 0.2f * enrange * v;
                }
                else
                {
                    NPC.velocity.Y -= 0.2f * enrange * v;
                }
            }
            if(CEUtils.getDistance(NPC.Center, player.Center) > 300)
            {
                NPC.velocity.X += Math.Sign(player.Center.X - NPC.Center.X) * 0.3f * enrange;
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D body = NPC.getTexture();
            Main.EntitySpriteDraw(body, NPC.Center - screenPos, null, drawColor, NPC.rotation, body.Size() / 2f, NPC.scale, NPC.rotation.ToRotationVector2().X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            foreach(var l in legs)
            {
                CEUtils.DrawGlow(l.StandPoint, Color.White, 0.6f);
                CEUtils.DrawGlow(l.StandPoint, Color.White, 0.6f);
            }
            return false;
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
