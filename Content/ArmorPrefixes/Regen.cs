using CalamityEntropy.Utilities;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Regen : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.Entropy().lifeRegenPerSec += 1;
        }
        public override int getRollChance()
        {
            return 3;
        }
        public override Color getColor()
        {
            return Color.Pink;
        }
    }
}
