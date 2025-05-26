using CalamityEntropy.Utilities;
using CalamityMod.Items.LoreItems;
using System.Collections.Generic;
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
        public override void ModifyTooltip(List<TooltipLine> tooltips)
        {
            tooltips.Replace("{1}", LifeAddition);
        }
    }
}