using CalamityEntropy.Utilities;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEKingSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreKingSlime>();
        public override void UpdateEffects(Player player)
        {
            player.jumpSpeedBoost += 1f;
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
    public class LEEOC : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreEyeofCthulhu>();
        public static float Value = 0.1f;
        public override void UpdateEffects(Player player)
        {
            player.Entropy().DashCD -= Value;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", Value.ToPercent().ToString());
        }
    }
    public class LECabulon : LoreEffect
    {
        public static int BuffTime = 15;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", BuffTime.ToString());
        }
        public override int ItemType => ModContent.ItemType<LoreCrabulon>();
    }
}