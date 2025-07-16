using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEAwaken : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAwakening>();
        public override void UpdateEffects(Player player)
        {
            player.Entropy().moveSpeed += 0.005f;
            player.jumpSpeedBoost += 0.05f;
        }
    }
}