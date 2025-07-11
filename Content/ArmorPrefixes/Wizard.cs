using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Wizard : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Summon) -= 0.10f;
            player.maxMinions += 1;
        }
        public override bool? canApplyTo(Item item)
        {
            if (!DownedBossSystem.downedCalamitas)
            {
                return false;
            }
            return base.canApplyTo(item);
        }
        public override int getRollChance()
        {
            return 1;
        }
    }
}
