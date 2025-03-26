using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Reckless : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
        }
        public override Color getColor()
        {
            return Color.Red;
        }
        public override int getRollChance()
        {
            return 2;
        }
        public override float AddDefense()
        {
            return -0.4f;
        }
    }
}
