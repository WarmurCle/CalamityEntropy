using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Content.Projectiles.AbyssalWraithProjs;
using CalamityEntropy.Util;
using CalamityMod;
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
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityEntropy.Content.NPCs.NihilityTwin
{
    [AutoloadBossHead]
    public class ChaoticCell : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.48f,
                PortraitScale = 0.56f,
                CustomTexturePath = "CalamityEntropy/Assets/Extra/CCBes",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.NihilityTwinBestiary")
            });
        }
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 76;
            if (Main.expertMode) {
                NPC.damage += 6;
            } if (Main.masterMode) {
                NPC.damage += 6;
            }
            NPC.defense = 20;
            NPC.lifeMax = 215000;
            if (CalamityWorld.death)
            {
                NPC.damage += 10;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 8;
            }
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCHit1;
            NPC.value = Item.buyPrice(1, 2, 60, 0);
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.5f;
            NPC.dontCountMe = true;
            NPC.netAlways = true;
        }
        public bool init = true;
        public override void SendExtraAI(BinaryWriter writer)
        {
           writer.Write(NPC.realLife);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.realLife = reader.ReadInt32();
        }
        public override void AI()
        {
            NPC.netUpdate = true;
            NPC.velocity *= 0.965f;
            if(NPC.realLife < 0)
            {
                return;
            }
            NPC.rotation += NPC.velocity.X * 0.006f;
            if (init) {
                init = false;
                for (int i = 1; i <= 4; i++)
                {
                    ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/NihilityTwin/ChaoticCell" + i.ToString());
                }
            }
            if (Main.GameUpdateCount % 5 == 0) {
                frame++;
                if(frame > 4)
                {
                    frame = 1;
                }
            }
            if (!owner.active)
            {
                NPC.active = false;
            }
            else
            {

            }
        }
        public NPC owner { get { return NPC.realLife.ToNPC(); } }

        public override bool CheckActive()
        {
            if(NPC.realLife < 0)
            {
                return true;
            }
            return !owner.active;
        }
        public int frame = 1;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if(NPC.realLife >= 0)
            {
                if(owner.ModNPC is NihilityActeriophage na)
                {
                    if (na.spawnAnm > 0)
                    {
                        return false;
                    }
                    na.drawRope();
                }
            }
            else
            {
                return false;
            }
            Texture2D tex = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/NihilityTwin/ChaoticCell" + frame.ToString()).Value;
            Color color = Color.White;
            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition, null, color, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None);
            return false;
        }

    }
}
