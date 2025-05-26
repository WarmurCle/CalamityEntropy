using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.FurnitureStatigel;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
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
        public override void UpdateEffects(Player player)
        {
            player.Entropy().DashCD -= 0.2f;
        }
    }
    public class LECabulon : LoreEffect
    {
        public static int BuffTime = 15;
        public override void ModifyTooltip(List<TooltipLine> tooltips)
        {
            tooltips.Replace("{1}", BuffTime);
        }
        public override int ItemType => ModContent.ItemType<LoreCrabulon>();
    }
}