using CalamityEntropy.Util;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Massive : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().moveSpeed -= 0.15f;
        }
        public override float AddDefense()
        {
            return 0.35f;
        }
        public override int getRollChance()
        {
            return 4;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}
