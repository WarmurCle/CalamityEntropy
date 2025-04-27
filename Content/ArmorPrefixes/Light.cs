using CalamityEntropy.Utilities;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Light : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.Entropy().moveSpeed += 0.1f;
            player.Entropy().WingSpeed += 0.1f;
        }
    }
}
