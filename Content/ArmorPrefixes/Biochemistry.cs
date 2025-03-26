using CalamityEntropy.Util;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Biochemistry : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().DebuffImmuneChance += 0.15f;
        }
        public override int getRollChance()
        {
            return 2;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}
