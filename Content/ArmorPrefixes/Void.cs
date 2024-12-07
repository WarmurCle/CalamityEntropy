using CalamityEntropy.Util;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Void : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().shootSpeed += 0.5f;
            player.Entropy().AttackVoidTouch += 0.1f;
            player.Entropy().Thorn += 0.4f;
        }
        public override int getRollChance()
        {
            return 3;
        }
    }
}
