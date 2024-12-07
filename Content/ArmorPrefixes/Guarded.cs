using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Guarded : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            
        }
        public override float AddDefense()
        {
            return 0.2f;
        }
        public override int getRollChance()
        {
            return 4;
        }
        public override Color getColor()
        {
            return Color.Orange;
        }
    }
}
