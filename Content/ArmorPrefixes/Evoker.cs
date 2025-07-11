using CalamityMod;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Evoker : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Generic) -= 0.15f;
        }
        public override bool? canApplyTo(Item item)
        {
            if (!DownedBossSystem.downedCalamitas)
            {
                return false;
            }
            return base.canApplyTo(item);
        }
        public override Color getColor()
        {
            return Color.LightBlue;
        }
        public override int getRollChance()
        {
            return 1;
        }
    }
}
