using CalamityEntropy.Content.Items;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LELuminaris : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LuminarisLore>();
        public override void UpdateEffects(Player player)
        {
            player.Entropy().WingTimeMult += LuminarisLore.wingTimeAddition;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", LuminarisLore.wingTimeAddition.ToPercent().ToString());
        }
    }
}
