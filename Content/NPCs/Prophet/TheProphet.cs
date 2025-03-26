using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using CalamityEntropy.Common;
using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Content.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.Util;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Prophet
{
    [AutoloadBossHead]
    public class TheProphet : ModNPC
    {
        public class Fin
        {
            public Vector2 pos;
            public float rot;
            public float tRot = 0;
            public float tRS = 0.9f;
            public Vector2 offset;
            public Fin(Vector2 pos, Vector2 ofs, float tRot, float tRS)
            {
                this.pos = pos;
                rot = 0;
                this.tRot = tRot;
                this.tRS = tRS;
                offset = ofs;
            }

            public void update(NPC prop)
            {
                Vector2 p = prop.Center + offset.RotatedBy(prop.rotation);
                this.pos = p;
                this.rot = Util.Util.rotatedToAngle(this.rot, prop.rotation + this.tRot - (this.offset.Y > 0 ? -1 : 1) * prop.velocity.Length() * 0.044f, tRS, false);
            }
        }
        public List<Fin> leftFin = new List<Fin>();
        public List<Fin> rightFin = new List<Fin>();
        public class TailPoint
        {
            public int timeLeft = 20;
            public Vector2 position;
            public Vector2 velocity;
            public TailPoint(Vector2 pos, Vector2 vel)
            {
                position = pos;
                velocity = vel;
            }
            public void update()
            {
                position += velocity;
                velocity *= 0.83f;
                timeLeft--;
            }
        }
        public List<TailPoint> tail;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.TheProphetBestiary")
            });
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 60;
            NPC.defense = 46;
            NPC.lifeMax = 36000;
            if (CalamityWorld.death)
            {
                NPC.damage += 9;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 4;
            }
            var snd = Util.Util.GetSound("prophet_hurt");
            var snd2 = Util.Util.GetSound("prophet_death");
            NPC.HitSound = snd;
            NPC.DeathSound = snd2;
            NPC.value = 2000f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.25f;
            NPC.dontCountMe = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/TheProphet");
            }
        }

        public override void AI()
        {
            if (NPC.localAI[1] == 0)
            {
                tail = new List<TailPoint>();
                leftFin.Add(new Fin(NPC.Center, new Vector2(-24, 6), -3.14f * 1.24f, 0.4f));
                leftFin.Add(new Fin(NPC.Center, new Vector2(-24, 6), -3f * 1.24f, 0.3f));
                leftFin.Add(new Fin(NPC.Center, new Vector2(-24, 6), -2.8f * 1.24f, 0.2f));
                leftFin.Add(new Fin(NPC.Center, new Vector2(-24, 6), -2.6f * 1.24f, 0.1f));
                
                rightFin.Add(new Fin(NPC.Center, new Vector2(-24, -6), 3.14f * 1.24f, 0.4f));
                rightFin.Add(new Fin(NPC.Center, new Vector2(-24, -6), 3f * 1.24f, 0.3f));
                rightFin.Add(new Fin(NPC.Center, new Vector2(-24, -6), 2.8f * 1.24f, 0.2f));
                rightFin.Add(new Fin(NPC.Center, new Vector2(-24, -6), 2.6f * 1.24f, 0.1f));
            }
            foreach (var f in leftFin)
            {
                f.update(NPC);
            }
            foreach (var f in rightFin)
            {
                f.update(NPC);
            }
            NPC.localAI[1]++;
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            if (!NPC.HasValidTarget)
            {
                NPC.localAI[0]++;
                NPC.velocity.Y -= 0.4f;
                NPC.velocity *= 0.96f;
                NPC.rotation = NPC.velocity.ToRotation();
                if (NPC.localAI[0] > 180)
                {
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.localAI[0] = 0;
                Player target = NPC.target.ToPlayer();
                NPC.velocity += (target.Center - NPC.Center).normalize() * 0.9f;
                NPC.velocity *= 0.98f;
                NPC.rotation = NPC.velocity.ToRotation();
            }
            foreach (TailPoint p in tail)
            {
                p.update();
            }
            if (tail.Count > 0)
            {
                if (tail[0].timeLeft <= 0)
                {
                    tail.RemoveAt(0);
                }
            }
            tail.Add(new TailPoint(NPC.Center - NPC.rotation.ToRotationVector2() * 26, (NPC.rotation.ToRotationVector2() * -2) + NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (float)(Math.Sin(Main.GameUpdateCount * 0.24f) * 15)));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawTail();
            DrawFins();
            Texture2D tex = TextureAssets.Npc[NPC.type].Value;
            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation + MathHelper.PiOver2, tex.Size() / 2, NPC.scale, SpriteEffects.None);
            return false;
        }
        public void DrawFins()
        {
            List<Texture2D> f = new List<Texture2D>() {
                ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Prophet/fin1").Value,
                ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Prophet/fin2").Value ,
                ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Prophet/fin3").Value ,
                ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Prophet/fin4").Value
            };
            for (int i = 3; i >= 0; i--)
            {
                Main.EntitySpriteDraw(f[i], leftFin[i].pos - Main.screenPosition, null, Color.White, leftFin[i].rot - MathHelper.PiOver2, new Vector2(0, 0), NPC.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(f[i], rightFin[i].pos - Main.screenPosition, null, Color.White, rightFin[i].rot - MathHelper.PiOver2, new Vector2(f[i].Width, 0), NPC.scale, SpriteEffects.FlipHorizontally);
            }

        }
        public void DrawTail()
        {
            if(tail.Count < 3)
            {
                return;
            }
            List<Vertex> ve = new List<Vertex>();
            Color b = Color.White;
            
            for (int i = 0; i < tail.Count - 3; i++)
            {
                ve.Add(new Vertex(tail[i].position - Main.screenPosition + (tail[i + 1].position - tail[i].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 19,
                      new Vector3((((float)i) / tail.Count), 1, 1),
                      b));
                ve.Add(new Vertex(tail[i].position - Main.screenPosition + (tail[i + 1].position - tail[i].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 19,
                      new Vector3((((float)i) / tail.Count), 0, 1),
                      b));

            }
            ve.Add(new Vertex(NPC.Center - Main.screenPosition + (NPC.Center - tail[tail.Count - 1].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 19,
                      new Vector3((float)1, 1, 1),
                      b));
            ve.Add(new Vertex(NPC.Center - Main.screenPosition + (NPC.Center - tail[tail.Count - 1].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 19,
                  new Vector3((float)1, 0, 1),
                  b));
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Prophet/Tail").Value;
                gd.Textures[0] = tx;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
        }

    }
}
