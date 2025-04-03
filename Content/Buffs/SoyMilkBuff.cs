using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class SoyMilkBuff : ModBuff
    {
        public int counter;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) *= 2;
            player.GetDamage(DamageClass.Generic) *= 0.35f;
            player.Calamity().rogueStealth = 0;
            player.pickSpeed -= 0.6f;
        }
    }
}
