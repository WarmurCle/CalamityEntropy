using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.SoulCards
{
    public class BitternessCard : ModItem
    {
        public static float DmgMax = 0.06f;
        public static float enduMax = 0.1f;

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
            player.Entropy().bitternessCard = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[1]", DmgMax.ToPercent());
            tooltips.Replace("[2]", enduMax.ToPercent());
        }
    }
}
