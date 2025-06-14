using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LECrimson : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreCrimson>();
        public static int LifeAddition = 10;
        public override void UpdateEffects(Player player)
        {
            player.statLifeMax2 += LifeAddition;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", LifeAddition.ToString());
        }
    }
    public class LECorruption : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreCorruption>();
        public static float DR = 0.01f;
        public override void UpdateEffects(Player player)
        {
            player.endurance += DR;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DR.ToPercent().ToString());
        }
    }
}