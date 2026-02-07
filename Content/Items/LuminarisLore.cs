using CalamityMod;
using CalamityMod.Items.LoreItems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class LuminarisLore : LoreItem
    {
        public static float wingTimeAddition = 0.05f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new TooltipLine(base.Mod, "CalamityMod:Lore", Language.GetTextValue("Mods.CalamityEntropy.loreLuminaris"));
            if (ExtensionIndicatorColor.HasValue)
            {
                tooltipLine.OverrideColor = ExtensionIndicatorColor.Value;
            }

            CEUtils.HoldShiftTooltip(tooltips, new TooltipLine[1] { tooltipLine }, hideNormalTooltip: true);
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = 1;
        }
    }
}
