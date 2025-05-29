using CalamityEntropy.Common;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.SoulCards
{
    public class WisperCard : ModItem
    {
        public static float ImmuneAdd = 0.25f;
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().HitCooldown += ImmuneAdd;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[T]", ImmuneAdd.ToPercent());
        }
    }
}
