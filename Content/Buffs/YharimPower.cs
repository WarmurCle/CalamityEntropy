using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class YharimPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.06f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.pickSpeed -= 0.3f;
            player.GetAttackSpeed(DamageClass.Melee) *= 1.1f;
            player.GetKnockback(DamageClass.Summon) *= 2;
            player.moveSpeed *= 1.1f;
            player.statDefense += 10;
            player.lifeRegen += 6;
        }
    }
}
