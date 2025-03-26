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
            tail = new List<TailPoint>();
            NPC.boss = true;
            NPC.width = 34;
            NPC.height = 34;
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
            if (tail[0].timeLeft <= 0)
            {
                tail.RemoveAt(0);
            }
            tail.Add(new TailPoint(NPC.Center - NPC.rotation.ToRotationVector2() * 25, (NPC.rotation.ToRotationVector2() * -8).RotatedBy((float)(Math.Sin(Main.GameUpdateCount * 0.2f)) * 0.46f)));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawTail();
            Texture2D tex = TextureAssets.Npc[NPC.type].Value;
            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation + MathHelper.PiOver2, tex.Size() / 2, NPC.scale, SpriteEffects.None);
            return false;
        }
        public void DrawTail()
        {
            List<Vertex> ve = new List<Vertex>();
            Color b = Color.White;
            ve.Add(new Vertex(tail[0].position - Main.screenPosition + (tail[1].position - tail[0].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 10,
                      new Vector3((float)0, 1, 1),
                      b));
            ve.Add(new Vertex(tail[0].position - Main.screenPosition + (tail[1].position - tail[0].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 10,
                  new Vector3((float)0, 0, 1),
                  b));
            for (int i = 1; i < tail.Count; i++)
            {
                ve.Add(new Vertex(tail[i].position - Main.screenPosition + (tail[i].position - tail[i - 1].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 20,
                      new Vector3((((float)i + 1) / tail.Count), 1, 1),
                      b));
                ve.Add(new Vertex(tail[i].position - Main.screenPosition + (tail[i].position - tail[i - 1].position).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 20,
                      new Vector3((((float)i + 1) / tail.Count), 0, 1),
                      b));

            }

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
