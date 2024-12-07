using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Wizard : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.maxMinions += 1;
        }
        public override int getRollChance()
        {
            return 2;
        }
    }
}
