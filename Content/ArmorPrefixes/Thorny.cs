using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Thorny : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().Thorn += 1f;
        }
        public override int getRollChance()
        {
            return 5;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}
