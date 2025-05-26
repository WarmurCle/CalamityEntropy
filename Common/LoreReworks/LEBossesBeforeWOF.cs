using CalamityEntropy.Utilities;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEKingSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreKingSlime>();
        public override void UpdateEffects(Player player)
        {
            player.jumpSpeedBoost += 0.8f;
            player.Entropy().moveSpeed -= 0.16f;
        }
    }
    public class LEDesertScourge : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreDesertScourge>();
        public override void UpdateEffects(Player player)
        {
            player.breathMax += 40;
        }
    }
}