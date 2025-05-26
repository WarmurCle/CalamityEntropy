using CalamityEntropy.Utilities;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEKingSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAwakening>();
        public override void UpdateEffects(Player player)
        {
            player.Entropy().moveSpeed += 0.01f;
            player.jumpSpeedBoost += 0.1f;
        }
    }
}