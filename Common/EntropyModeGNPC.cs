using System;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EntropyModeGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        public override void PostAI(NPC npc)
        {
            if (CalamityEntropy.EntropyMode)
            {
                if (npc.ModNPC is DesertScourgeHead && npc.localAI[2] == 1f && this.dScFLag)
                {
                    this.dScFLag = false;
                    NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHead>());
                    NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHeadYoung>());
                }
                if (npc.type == 50)
                {
                    if (this.init && npc.type == 50)
                    {
                        npc.MaxFallSpeedMultiplier *= 36f;
                    }
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
                        Util.PlaySound("ksLand", 1f, new Vector2?(npc.Center), 2, 1f);
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
                }
                if (this.SpawnAtHalfLife && npc.life < npc.lifeMax / 2)
                {
                    this.SpawnAtHalfLife = false;
                    if (npc.type == 50)
                    {
                        Vector2 vector = npc.Center + new Vector2(-40f, -(float)npc.height / 2f) * npc.scale;
                        if (Main.netMode != 1)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(null), (int)vector.X, (int)vector.Y - 60, ModContent.NPCType<TopazJewel>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                    }
                }
            }
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
