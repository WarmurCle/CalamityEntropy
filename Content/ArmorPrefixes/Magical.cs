using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Magical : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().ManaCost -= 0.1f;
            player.statManaMax2 += 6;
            player.Entropy().enhancedMana += 0.1f;
        }
        public override Color getColor()
        {
            return Color.LightBlue;
        }
        public override int getRollChance()
        {
            return 1;
        }
        public override bool Dramatic()
        {
            return true;
        }
    }
}
