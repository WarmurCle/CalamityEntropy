using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser
{
    public class CruiserBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            ((int)NPC.ai[1]).ToNPC().ModNPC.ModifyHitByProjectile(projectile, ref modifiers);
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= ((int)NPC.ai[1]).ToNPC().Entropy().damageMul;
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.width);
            writer.Write(NPC.height);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.width = reader.ReadInt32();
            NPC.height = reader.ReadInt32();
        }

        public override void SetDefaults()
        {

            NPC.width = 70;
            NPC.height = 70;
            NPC.damage = 100;
            NPC.dontCountMe = true;
            NPC.dontCountMe = true;
            NPC.lifeMax = 80000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.value = 50f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.defense = 60;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.7f;
            NPC.scale = 1.1f;
            NPC.Calamity().DR = 0.4f;
            if (Main.getGoodWorld)
            {
                NPC.scale = 0.5f;
            }
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/CruiserBoss");
            }
        }

        public override void AI()
        {
            NPC.defense = Main.npc[(int)NPC.ai[1]].defense;
            NPC.Calamity().DR = Main.npc[(int)NPC.ai[1]].Calamity().DR;
            NPC.dontTakeDamage = Main.npc[(int)NPC.ai[1]].dontTakeDamage;
            NPC.ai[0] += 1;
            if (NPC.ai[0] < 5)
            {
                return;
            }
            /*            if (((int)NPC.ai[3]).ToNPC().life < (((int)NPC.ai[3]).ToNPC().lifeMax / 2) && NPC.ai[2] > 8)
                        {
                            NPC.active = false;
                            NPC.netUpdate = true;
                        }*/
            if (!Main.dedServ)
            {
                Lighting.AddLight(NPC.Center, 1f, 1f, 1f);
            }
            if (NPC.ai[1] < Main.maxNPCs)
            {
                if (Main.npc[(int)NPC.ai[1]].active)
                {

                    int spacing = 80;
                    NPC follow = Main.npc[(int)NPC.ai[1]];
                    if (follow.active)
                    {
                        Util.Util.wormFollow(NPC.whoAmI, (int)NPC.ai[1], (int)(spacing * NPC.scale), false);
                        if (NPC.ai[0] > 120)
                        {
                            Util.Util.wormFollow(NPC.whoAmI, (int)NPC.ai[1], (int)(spacing * NPC.scale), true, 0.12f);
                        }
                    }
                }
                else
                {
                    NPC.active = false;
                }

            }
            else
            {
                NPC.active = false;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public override bool CheckActive()
        {
            if (((int)NPC.ai[1]).ToNPC().active)
            {
                return false;
            }
            return true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
    }
}
