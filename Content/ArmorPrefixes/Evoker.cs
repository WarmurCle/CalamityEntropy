using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Evoker : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.maxMinions += 2;
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
