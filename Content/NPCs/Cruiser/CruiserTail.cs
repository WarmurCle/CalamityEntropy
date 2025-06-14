using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser
{
    public class CruiserTail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public Vector2 lastPos;
        public override void SetDefaults()
        {

            NPC.width = 45;
            NPC.height = 45;
            NPC.damage = 120;
            NPC.dontCountMe = true;
            NPC.defense = 10;
            NPC.lifeMax = 80000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.value = 50f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.Entropy().VoidTouchDR = 0.6f;
            NPC.scale = 1.1f;
            if (Main.getGoodWorld)
            {
                NPC.scale = 0.5f;
            }
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/CruiserBoss");
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
            ((int)NPC.ai[1]).ToNPC().ModNPC.ModifyHitByProjectile(projectile, ref modifiers);
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= ((int)NPC.ai[1]).ToNPC().Entropy().damageMul;
            if (NPC.ai[0] < 500)
            {
                modifiers.SourceDamage *= ((float)NPC.ai[0] / 500f);
            }
        }
        public override bool CheckActive()
        {
            if (((int)NPC.ai[1]).ToNPC().active)
            {
                return false;
            }
            return true;
        }

        public override void AI()
        {
            NPC.scale = Main.npc[(int)NPC.ai[1]].scale;
            NPC.Entropy().VoidTouchDR = Main.npc[(int)NPC.ai[1]].Entropy().VoidTouchDR;
            NPC.defense = Main.npc[(int)NPC.ai[1]].defense;
            NPC.Calamity().DR = Main.npc[(int)NPC.ai[1]].Calamity().DR;
            NPC.dontTakeDamage = Main.npc[(int)NPC.ai[1]].dontTakeDamage;
            NPC.ai[0] += 1;
            if (NPC.ai[0] < 5)
            {
                return;
            }
            Lighting.AddLight(NPC.Center, 1f, 1f, 1f);
            /*            if (((int)NPC.ai[3]).ToNPC().life < (((int)NPC.ai[3]).ToNPC().lifeMax / 2))
                        {
                            NPC.active = false;
                            NPC.netUpdate = true;
                        }*/
            if (NPC.ai[1] < Main.maxNPCs && Main.npc[(int)NPC.ai[1]].active)
            {
                int spacing = 80;
                NPC follow = Main.npc[(int)NPC.ai[1]];
                if (follow.active)
                {
                    CEUtils.wormFollow(NPC.whoAmI, (int)NPC.ai[1], (int)(spacing * NPC.scale), false);
                    if (NPC.ai[0] > 120)
                    {
                        CEUtils.wormFollow(NPC.whoAmI, (int)NPC.ai[1], (int)(spacing * NPC.scale), true, 0.12f);
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
        /*vel = NPC.Center - lastPos;
        if (NPC.ai[3] == 1)
        {
            NPC.ai[3] = 0;
            jv = true;
            if (da < 0)
            {
                da = 1;
            }
            tail_vj = 20;
        }*/
        /*            if (jv)
                    {
                        da += tail_vj;
                        tail_vj -= 1.5f;
                        if (da < 0)
                        {
                            da = 0;
                            tail_vj = 0;
                            jv = false;
                        }
                    }
                    else
                    {
                        ja -= (float)Math.Sqrt((float)vel.Length() / 120f);
                        if (ja < 0)
                        {
                            ja = 0;
                        }
                        ja = Util.rotatedToAngle(ja, 50, 0.22f, false);
                        ja = (100f / ((float)vel.Length() + 1f)) * 5;
                        da = da + (ja - da) * 0.1f;
                        lastPos = NPC.Center;
                    }*/

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return;
            //Texture2D f1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/Flagellum").Value;

            //spriteBatch.Draw(f1, NPC.Center - Main.screenPosition - new Vector2(32, 0).RotatedBy(NPC.rotation), null, Color.White, NPC.rotation + MathHelper.ToRadians(190 - da), new Vector2(0, f1.Height), NPC.scale, SpriteEffects.None, 0);
            //spriteBatch.Draw(f1, NPC.Center - Main.screenPosition - new Vector2(32, 0).RotatedBy(NPC.rotation), null, Color.White, NPC.rotation + MathHelper.ToRadians(170 + da), new Vector2(0, 0), NPC.scale, SpriteEffects.FlipVertically, 0);
        }
    }
}
