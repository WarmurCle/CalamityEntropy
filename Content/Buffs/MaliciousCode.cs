using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityEntropy.Util;
using Terraria.DataStructures;
using System.Runtime.Serialization.Formatters;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Content.Buffs
{
    public class MaliciousCode : ModBuff
    {
        public static bool CALAMITY__OVERHAUL => ModLoader.HasMod("Calamity" + "Overhaul");
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.Entropy().maliciousCode = true;
            if (Main.rand.NextBool(16))
            {
                EParticle.spawnNew(new MCodeParticle(), player.Center + Util.Util.randomVec(28), Vector2.Zero, Main.rand.NextBool(5) ? Main.DiscoColor : Color.White, Main.rand.NextFloat(0.6f, 1.4f), 1, true, BlendState.AlphaBlend, 0);
            }
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(14))
            {
                EParticle.spawnNew(new MCodeParticle(), npc.Center + new Vector2(Main.rand.NextFloat(npc.width) - npc.width / 2, Main.rand.NextFloat(npc.height) - npc.height / 2), Vector2.Zero, Main.rand.NextBool(5) ? Main.DiscoColor : Color.White, Main.rand.NextFloat(0.6f, 1.4f), 1, true, BlendState.AlphaBlend, 0);
            }
        }
    }
    public class MaliciousCodeNPC : GlobalNPC
    {
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (npc.HasBuff<MaliciousCode>())
            {
                modifiers.FinalDamage *= MaliciousCode.CALAMITY__OVERHAUL ? 0.6f : 0.7f;
            }
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.HasBuff<MaliciousCode>())
            {
                if (Main.rand.NextBool(3))
                {
                    return false;
                }
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
    public class MaliciousCodeProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool malicious = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent ep)
            {
                if (ep.Entity is NPC npc && npc.HasBuff<MaliciousCode>())
                {
                    malicious = true;
                }
            }
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (malicious)
            {
                modifiers.FinalDamage *= MaliciousCode.CALAMITY__OVERHAUL ? 0.86f : 0.9f;
            }
        }
        public bool SHOTSPEEDDOWN = true;
        public override bool PreAI(Projectile projectile)
        {
            if (malicious)
            {
                if (SHOTSPEEDDOWN)
                {
                    SHOTSPEEDDOWN = false;
                    projectile.velocity *= MaliciousCode.CALAMITY__OVERHAUL ? 0.7f : 0.8f;
                }
            }
            return base.PreAI(projectile);
        }
    }
}
