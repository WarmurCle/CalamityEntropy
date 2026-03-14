using CalamityEntropy.Content.Buffs;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Apsychos
{
    public class ApsychosTail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.damage = 32;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 1400;
            NPC.DeathSound = null;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.timeLeft *= 5;
        }
        public NPC owner => ((int)NPC.ai[0]).ToNPC();
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
        public override void AI()
        {
            if(Main.netMode != NetmodeID.MultiplayerClient)
            {
                if(!owner.active)
                {
                    NPC.active = false;
                    if(Main.dedServ)
                    {
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                }
            }
        }
        public override bool CheckActive()
        {
            return !owner.active;
        }
        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            npcHitbox = npcHitbox.Center.ToVector2().getRectCentered((npcHitbox.Width * NPC.scale), (npcHitbox.Height * NPC.scale));
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }
}
